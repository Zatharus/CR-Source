// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.CrashWatchDog
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using cYo.Common.Threading;
using System;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Runtime
{
  public class CrashWatchDog : DisposableObject
  {
    private const int WatcherTimeSpanMS = 1000;
    private readonly object timeLock = new object();
    private TimeSpan lockTestTime = new TimeSpan(0, 0, 10);
    private DateTime lastTimeRunning = DateTime.Now;
    private bool inBark;
    private Thread lockWatcherThread;
    private readonly EventWaitHandle lockWatcherHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

    protected override void Dispose(bool disposing)
    {
      this.lockWatcherHandle.Close();
      base.Dispose(disposing);
    }

    public void Register()
    {
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.domain_UnhandledException);
      Application.ThreadException += new ThreadExceptionEventHandler(this.Application_ThreadException);
      if (this.lockTestTime.TotalSeconds <= 0.0)
        return;
      this.StartLockWatcher();
    }

    public TimeSpan LockTestTime
    {
      get
      {
        using (ItemMonitor.Lock(this.timeLock))
          return this.lockTestTime;
      }
      set
      {
        using (ItemMonitor.Lock(this.timeLock))
          this.lockTestTime = value;
      }
    }

    public DateTime LastTimeRunning
    {
      get
      {
        using (ItemMonitor.Lock(this.timeLock))
          return this.lastTimeRunning;
      }
      protected set
      {
        using (ItemMonitor.Lock(this.timeLock))
          this.lastTimeRunning = value;
      }
    }

    public event BarkEventHandler Bark;

    protected virtual void OnBark(BarkType bark, Exception e)
    {
      try
      {
        this.inBark = true;
        if (this.Bark == null)
          return;
        this.Bark((object) this, new BarkEventArgs(bark, e));
      }
      finally
      {
        this.inBark = false;
      }
    }

    protected virtual void OnLockDetected()
    {
      this.OnBark(BarkType.Lock, (Exception) null);
      ThreadUtility.BreakForegroundLock();
    }

    protected virtual void OnThreadException(Exception e)
    {
      this.OnBark(BarkType.ThreadException, e);
    }

    protected virtual void OnDomainException(Exception e)
    {
      this.OnBark(BarkType.DomainException, e);
    }

    private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      this.OnThreadException(e.Exception);
    }

    private void domain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      this.OnDomainException(e.ExceptionObject as Exception);
    }

    private void StartLockWatcher()
    {
      this.lockWatcherThread = new Thread(new ThreadStart(this.LockWatcher))
      {
        IsBackground = true,
        Priority = ThreadPriority.Highest
      };
      this.lockWatcherThread.Start();
    }

    private void LockWatcher()
    {
      DateTime dateTime = DateTime.Now;
      while (!this.lockWatcherHandle.WaitOne(1000, false))
      {
        DateTime now = DateTime.Now;
        if (this.inBark || !ThreadUtility.IsForegroundLocked || now - dateTime > this.LockTestTime)
          this.LastTimeRunning = now;
        else if (now - this.LastTimeRunning > this.LockTestTime)
        {
          this.LastTimeRunning = now;
          this.OnLockDetected();
        }
        dateTime = now;
      }
    }
  }
}

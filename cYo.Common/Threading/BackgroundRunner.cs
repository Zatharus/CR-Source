// Decompiled with JetBrains decompiler
// Type: cYo.Common.Threading.BackgroundRunner
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Threading
{
  public class BackgroundRunner : Component
  {
    private readonly ManualResetEvent exitEvent = new ManualResetEvent(false);
    private readonly ManualResetEvent runEvent = new ManualResetEvent(false);
    private readonly ManualResetEvent intervalEvent = new ManualResetEvent(false);
    private Thread thread;
    private volatile bool enabled;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.runEvent.Reset();
        this.exitEvent.Set();
        this.intervalEvent.Set();
        Thread thread = this.thread;
        if (thread != null && !thread.Join(5000))
        {
          thread.Abort();
          thread.Join();
        }
        this.runEvent.Dispose();
        this.exitEvent.Dispose();
      }
      base.Dispose(disposing);
    }

    [DefaultValue(null)]
    public ISynchronizeInvoke Synchronize { get; set; }

    [DefaultValue(0)]
    public int Interval { get; set; }

    [DefaultValue(false)]
    public bool Enabled
    {
      get => this.enabled;
      set
      {
        if (value == this.enabled)
          return;
        this.enabled = value;
        if (this.enabled)
        {
          if (this.thread == null)
            this.thread = ThreadUtility.RunInBackground("BackgroundRunner Thread", new ThreadStart(this.BackgroundMethod));
          this.runEvent.Set();
          this.intervalEvent.Reset();
        }
        else
        {
          this.runEvent.Reset();
          this.intervalEvent.Set();
        }
      }
    }

    public event EventHandler Tick;

    protected virtual void OnTick()
    {
      if (this.Tick == null)
        return;
      this.Tick((object) this, EventArgs.Empty);
    }

    public void Start() => this.Enabled = true;

    public void Stop() => this.Enabled = false;

    private void BackgroundMethod()
    {
      ManualResetEvent[] waitHandles = new ManualResetEvent[2]
      {
        this.exitEvent,
        this.runEvent
      };
      while (WaitHandle.WaitAny((WaitHandle[]) waitHandles) != 0)
      {
        if (this.Synchronize == null)
        {
          this.InvokeTick();
        }
        else
        {
          try
          {
            this.Synchronize.Invoke((Delegate) new MethodInvoker(this.InvokeTick), (object[]) null);
          }
          catch (InvalidOperationException ex)
          {
          }
        }
        this.intervalEvent.WaitOne(this.Interval);
      }
    }

    private void InvokeTick()
    {
      if (!this.enabled)
        return;
      this.OnTick();
    }
  }
}

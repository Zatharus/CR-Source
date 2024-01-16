// Decompiled with JetBrains decompiler
// Type: cYo.Common.Threading.ThreadUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

#nullable disable
namespace cYo.Common.Threading
{
  public static class ThreadUtility
  {
    private static readonly HashSet<Thread> activeThreads = new HashSet<Thread>();
    private const int MaxThreadQueueSize = 64;
    private static readonly HashSet<Thread> threadQueue = new HashSet<Thread>();
    private static HashSet<Action> blocks;

    public static Thread CreateWorkerThread(
      string name,
      ThreadStart threadStart,
      ThreadPriority priority)
    {
      return new Thread((ThreadStart) (() =>
      {
        Thread currentThread = Thread.CurrentThread;
        ThreadUtility.AddActiveThread(currentThread);
        try
        {
          threadStart();
        }
        catch (ThreadInterruptedException ex)
        {
        }
        catch (ThreadAbortException ex)
        {
        }
        finally
        {
          ThreadUtility.RemoveActiveThread(currentThread);
        }
      }))
      {
        Name = name,
        Priority = priority,
        IsBackground = true,
        CurrentCulture = Thread.CurrentThread.CurrentCulture,
        CurrentUICulture = Thread.CurrentThread.CurrentUICulture
      };
    }

    public static Thread RunInBackground(string name, ThreadStart method, ThreadPriority priority = ThreadPriority.BelowNormal)
    {
      Thread workerThread = ThreadUtility.CreateWorkerThread(name, method, priority);
      workerThread.Start();
      return workerThread;
    }

    public static IAsyncResult RunInThreadPool(Action method)
    {
      ThreadUtility.ThreadPoolState threadPoolState = new ThreadUtility.ThreadPoolState();
      try
      {
        ThreadUtility.ThreadPoolState localState = threadPoolState;
        ThreadPool.QueueUserWorkItem((WaitCallback) (o =>
        {
          try
          {
            method();
          }
          catch (Exception ex)
          {
          }
          finally
          {
            localState.Dispose();
          }
        }));
      }
      catch (Exception ex)
      {
        threadPoolState.Dispose();
        threadPoolState = (ThreadUtility.ThreadPoolState) null;
      }
      return (IAsyncResult) threadPoolState;
    }

    public static IAsyncResult RunInThreadQueue(Action method)
    {
      ThreadUtility.ThreadPoolState threadPoolState = new ThreadUtility.ThreadPoolState();
      try
      {
        ThreadUtility.ThreadPoolState localState = threadPoolState;
        Thread thread = (Thread) null;
        Action action = (Action) (() =>
        {
          try
          {
            method();
          }
          catch (Exception ex)
          {
          }
          finally
          {
            localState.Dispose();
          }
        });
        using (ItemMonitor.Lock((object) ThreadUtility.threadQueue))
        {
          if (ThreadUtility.threadQueue.Count < 64)
            thread = new Thread((ThreadStart) (() =>
            {
              action();
              using (ItemMonitor.Lock((object) ThreadUtility.threadQueue))
                ThreadUtility.threadQueue.Remove(thread);
            }));
        }
        if (thread != null)
          thread.Start();
        else
          action();
      }
      catch (Exception ex)
      {
        threadPoolState.Dispose();
        threadPoolState = (ThreadUtility.ThreadPoolState) null;
      }
      return (IAsyncResult) threadPoolState;
    }

    public static void AddActiveThread(Thread t)
    {
      try
      {
        using (ItemMonitor.Lock((object) ThreadUtility.activeThreads))
          ThreadUtility.activeThreads.Add(t);
      }
      catch (Exception ex)
      {
      }
    }

    public static void RemoveActiveThread(Thread t)
    {
      using (ItemMonitor.Lock((object) ThreadUtility.activeThreads))
        ThreadUtility.activeThreads.Remove(t);
    }

    public static IEnumerable<Thread> ActiveThreads
    {
      get
      {
        return (IEnumerable<Thread>) ThreadUtility.activeThreads.Lock<Thread>().Where<Thread>((Func<Thread, bool>) (t => t.IsAlive)).ToArray<Thread>();
      }
    }

    public static Thread ForgroundThread
    {
      get
      {
        return ThreadUtility.ActiveThreads.FirstOrDefault<Thread>((Func<Thread, bool>) (t => !t.IsBackground));
      }
    }

    public static bool IsForegroundLocked
    {
      get
      {
        Thread forgroundThread = ThreadUtility.ForgroundThread;
        return forgroundThread != null && forgroundThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin;
      }
    }

    public static void Abort(Thread thread, int timeOut)
    {
      if (thread == null || !thread.IsAlive || thread.Join(timeOut))
        return;
      thread.Abort();
      thread.Join();
    }

    private static void DumpThread(TextWriter tw, Thread t)
    {
      try
      {
        if (!t.IsAlive)
          return;
        tw.WriteLine(new string('-', 20));
        tw.WriteLine("{0}: {1} ({2})", (object) t.Name, (object) t.ThreadState, t.IsBackground ? (object) "B" : (object) string.Empty);
        StackTrace stackTrace;
        if (t == Thread.CurrentThread)
        {
          stackTrace = new StackTrace();
        }
        else
        {
          t.Suspend();
          try
          {
            stackTrace = new StackTrace(t, true);
          }
          finally
          {
            t.Resume();
          }
        }
        tw.WriteLine(stackTrace.ToString());
      }
      catch
      {
      }
    }

    public static void DumpStacks(TextWriter tw)
    {
      foreach (Thread activeThread in ThreadUtility.ActiveThreads)
      {
        try
        {
          if (activeThread.IsAlive)
            ThreadUtility.DumpThread(tw, activeThread);
        }
        catch
        {
        }
      }
    }

    public static string StacksDump
    {
      get
      {
        StringWriter tw = new StringWriter();
        ThreadUtility.DumpStacks((TextWriter) tw);
        return tw.ToString();
      }
    }

    public static void BreakForegroundLock()
    {
      Thread forgroundThread = ThreadUtility.ForgroundThread;
      if (forgroundThread == null || forgroundThread.ThreadState != System.Threading.ThreadState.WaitSleepJoin)
        return;
      forgroundThread.Interrupt();
    }

    public static int Animate(int startTime, int endTime, Action<float> animate)
    {
      int num1 = endTime - startTime;
      long ticks1 = Machine.Ticks;
      long num2 = ticks1 + (long) num1;
      long ticks2;
      while ((ticks2 = Machine.Ticks) < num2)
        animate((float) (ticks2 - ticks1) / (float) (num2 - ticks1));
      animate(1f);
      return Math.Max((int) (ticks2 - num2), 0);
    }

    public static int Animate(int time, Action<float> animate)
    {
      return ThreadUtility.Animate(0, time, animate);
    }

    public static void Block(Action method)
    {
      if (ThreadUtility.blocks == null)
        ThreadUtility.blocks = new HashSet<Action>();
      if (ThreadUtility.blocks.Contains(method))
        return;
      try
      {
        ThreadUtility.blocks.Add(method);
        method();
      }
      catch
      {
        ThreadUtility.blocks.Remove(method);
      }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern ThreadUtility.EXECUTION_STATE SetThreadExecutionState(
      ThreadUtility.EXECUTION_STATE esFlags);

    public static void KeepAlive(bool withDisplay = false)
    {
      ThreadUtility.EXECUTION_STATE esFlags = ThreadUtility.EXECUTION_STATE.ES_SYSTEM_REQUIRED;
      if (withDisplay)
        esFlags |= ThreadUtility.EXECUTION_STATE.ES_DISPLAY_REQUIRED;
      int num = (int) ThreadUtility.SetThreadExecutionState(esFlags);
    }

    private class ThreadPoolState : IAsyncResult, IDisposable
    {
      public ThreadPoolState()
      {
        this.IsCompleted = false;
        this.AsyncWaitHandle = (WaitHandle) new ManualResetEvent(false);
      }

      public object AsyncState => throw new NotImplementedException();

      public WaitHandle AsyncWaitHandle { get; private set; }

      public bool CompletedSynchronously => throw new NotImplementedException();

      public bool IsCompleted { get; private set; }

      public void Dispose()
      {
        this.IsCompleted = true;
        ((EventWaitHandle) this.AsyncWaitHandle).Set();
        this.AsyncWaitHandle.Dispose();
      }
    }

    [Flags]
    private enum EXECUTION_STATE : uint
    {
      ES_AWAYMODE_REQUIRED = 64, // 0x00000040
      ES_CONTINUOUS = 2147483648, // 0x80000000
      ES_DISPLAY_REQUIRED = 2,
      ES_SYSTEM_REQUIRED = 1,
    }
  }
}

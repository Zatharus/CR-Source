// Decompiled with JetBrains decompiler
// Type: cYo.Common.Threading.ProcessingQueue`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
namespace cYo.Common.Threading
{
  public class ProcessingQueue<K> : DisposableObject
  {
    private bool abort;
    private bool stop;
    private readonly List<ProcessingQueue<K>.ProcessData> processThreads = new List<ProcessingQueue<K>.ProcessData>();
    private readonly LinkedList<K> processQueue = new LinkedList<K>();
    private readonly Dictionary<K, ProcessingQueue<K>.QueueItem> itemDict = new Dictionary<K, ProcessingQueue<K>.QueueItem>();
    private volatile ProcessingQueueAddMode defaultProcessingQueueAddMode;
    private volatile int size = int.MaxValue;

    public ProcessingQueue(int threadCount, string name, ThreadPriority priority, int size)
    {
      this.Size = size;
      for (int index = 0; index < threadCount; ++index)
      {
        string name1 = threadCount < 2 ? name : string.Format("{0} #{1}", (object) name, (object) (index + 1));
        ProcessingQueue<K>.ProcessData pd = new ProcessingQueue<K>.ProcessData()
        {
          Event = new AutoResetEvent(false)
        };
        Thread thread = pd.Thread = ThreadUtility.CreateWorkerThread(name1, (ThreadStart) (() => this.ProcessThread(pd)), priority);
        this.processThreads.Add(pd);
        thread.Start();
      }
    }

    public ProcessingQueue(string name, ThreadPriority priority, int size)
      : this(1, name, priority, size)
    {
    }

    public ProcessingQueue(string name, ThreadPriority priority = ThreadPriority.BelowNormal)
      : this(name, priority, int.MaxValue)
    {
    }

    private void ProcessThread(ProcessingQueue<K>.ProcessData pd)
    {
      while (pd.Event.WaitOne())
      {
        pd.IsActive = true;
        try
        {
          while (true)
          {
            if (!this.abort)
            {
              K k = default (K);
              ProcessingQueue<K>.QueueItem queueItem1 = (ProcessingQueue<K>.QueueItem) null;
              using (ItemMonitor.Lock((object) this.processQueue))
              {
                for (LinkedListNode<K> linkedListNode = this.processQueue.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
                {
                  K key = linkedListNode.Value;
                  ProcessingQueue<K>.QueueItem queueItem2;
                  if (this.itemDict.TryGetValue(key, out queueItem2) && queueItem2.State == ProgressState.Waiting)
                  {
                    k = key;
                    queueItem1 = queueItem2;
                    break;
                  }
                }
                if (queueItem1 == null)
                  goto label_19;
              }
              try
              {
                queueItem1.ProcessCallbacks();
              }
              catch (Exception ex)
              {
              }
              finally
              {
                this.RemoveItem(k, false);
                queueItem1.SetCompleted();
                queueItem1.Dispose();
              }
            }
            else
              break;
          }
          break;
        }
        finally
        {
          pd.IsActive = false;
        }
label_19:
        if (this.stop)
          break;
      }
    }

    public CultureInfo CurrentUICulture
    {
      get => this.processThreads[0].Thread.CurrentUICulture;
      set
      {
        this.processThreads.ForEach((Action<ProcessingQueue<K>.ProcessData>) (pd => pd.Thread.CurrentUICulture = value));
      }
    }

    public ThreadPriority Priority
    {
      get => this.processThreads[0].Thread.Priority;
      set
      {
        this.processThreads.ForEach((Action<ProcessingQueue<K>.ProcessData>) (pd => pd.Thread.Priority = value));
      }
    }

    public ProcessingQueueAddMode DefaultProcessingQueueAddMode
    {
      get => this.defaultProcessingQueueAddMode;
      set => this.defaultProcessingQueueAddMode = value;
    }

    public int Size
    {
      get => this.size;
      set
      {
        if (this.size == value)
          return;
        this.size = value;
        this.Trim(value);
      }
    }

    public int Count
    {
      get
      {
        using (ItemMonitor.Lock((object) this.processQueue))
          return this.processQueue.Count;
      }
    }

    public bool IsActive
    {
      get
      {
        return this.processThreads.Any<ProcessingQueue<K>.ProcessData>((Func<ProcessingQueue<K>.ProcessData, bool>) (pd => pd.IsActive));
      }
    }

    public IAsyncProcessingItem<K> AddItem(
      K item,
      object callbackKey,
      AsyncCallback processCallback,
      ProcessingQueueAddMode mode)
    {
      ProcessingQueue<K>.QueueItem queueItem;
      using (ItemMonitor.Lock((object) this.processQueue))
      {
        if (this.IsDisposed)
          return (IAsyncProcessingItem<K>) null;
        this.itemDict.TryGetValue(item, out queueItem);
        if (queueItem == null)
        {
          this.itemDict[item] = queueItem = new ProcessingQueue<K>.QueueItem(item, callbackKey, processCallback);
          if (mode != ProcessingQueueAddMode.AddToBottom)
          {
            if (mode == ProcessingQueueAddMode.AddToTop)
              ;
            this.processQueue.AddFirst(item);
          }
          else
            this.processQueue.AddLast(item);
        }
        else
        {
          queueItem.AddCallback(processCallback, callbackKey);
          if (mode == ProcessingQueueAddMode.AddToTop)
          {
            this.processQueue.Remove(item);
            this.processQueue.AddFirst(item);
          }
        }
      }
      this.Trim(this.size);
      this.processThreads.ForEach((Action<ProcessingQueue<K>.ProcessData>) (pd => pd.Event.Set()));
      return (IAsyncProcessingItem<K>) queueItem;
    }

    public IAsyncProcessingItem<K> AddItem(
      K item,
      object callbackKey,
      AsyncCallback processCallback)
    {
      return this.AddItem(item, callbackKey, processCallback, this.DefaultProcessingQueueAddMode);
    }

    public IAsyncResult AddItem(K item, AsyncCallback processCallback)
    {
      return (IAsyncResult) this.AddItem(item, (object) null, processCallback);
    }

    public void RemoveItem(K item, bool dispose = true)
    {
      using (ItemMonitor.Lock((object) this.processQueue))
      {
        ProcessingQueue<K>.QueueItem queueItem;
        if (this.itemDict.TryGetValue(item, out queueItem))
        {
          queueItem.Abort = true;
          if (dispose)
            queueItem.Dispose();
          this.itemDict.Remove(item);
        }
        this.processQueue.Remove(item);
      }
    }

    public void RemoveItems<TK>(Predicate<TK> predicate) where TK : K
    {
      using (ItemMonitor.Lock((object) this.processQueue))
        ((IEnumerable<TK>) this.itemDict.Keys.OfType<TK>().Where<TK>((Func<TK, bool>) (k => predicate(k))).ToArray<TK>()).ForEach<TK>((Action<TK>) (k => this.RemoveItem((K) k)));
    }

    public void Trim(int size)
    {
      using (ItemMonitor.Lock((object) this.processQueue))
      {
        while (this.processQueue.Count > size)
          this.RemoveItem(this.processQueue.Last.Value);
      }
    }

    public void Clear() => this.Trim(0);

    public void Stop(bool abort, int timeOut)
    {
      if (abort)
        this.abort = true;
      else
        this.stop = true;
      this.processThreads.ForEach((Action<ProcessingQueue<K>.ProcessData>) (pd => pd.Event.Set()));
      this.processThreads.ForEach((Action<ProcessingQueue<K>.ProcessData>) (pd => pd.Thread.Join(timeOut)));
    }

    public IEnumerable<K> PendingItems
    {
      get
      {
        using (ItemMonitor.Lock((object) this.processQueue))
          return (IEnumerable<K>) this.processQueue.ToList<K>();
      }
    }

    public IEnumerable<IProcessingItem<K>> PendingItemInfos
    {
      get
      {
        using (ItemMonitor.Lock((object) this.processQueue))
        {
          ProcessingQueue<K>.QueueItem q;
          return (IEnumerable<IProcessingItem<K>>) this.PendingItems.Select<K, ProcessingQueue<K>.QueueItem>((Func<K, ProcessingQueue<K>.QueueItem>) (item => !this.itemDict.TryGetValue(item, out q) ? (ProcessingQueue<K>.QueueItem) null : q)).Where<ProcessingQueue<K>.QueueItem>((Func<ProcessingQueue<K>.QueueItem, bool>) (item => item != null)).OfType<IProcessingItem<K>>().ToList<IProcessingItem<K>>();
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        using (ItemMonitor.Lock((object) this.processQueue))
        {
          this.processQueue.Clear();
          this.itemDict.Values.ForEach<ProcessingQueue<K>.QueueItem>((Action<ProcessingQueue<K>.QueueItem>) (qi => qi.Dispose()));
        }
        this.Stop(true, 5000);
        this.processThreads.ForEach((Action<ProcessingQueue<K>.ProcessData>) (pd => pd.Event.Close()));
      }
      base.Dispose(disposing);
    }

    private class QueueItem : 
      DisposableObject,
      IAsyncProcessingItem<K>,
      IAsyncResult,
      IProcessingItem<K>,
      IProgressState
    {
      private readonly object defaultCallbackKey;
      private readonly AsyncCallback defaultCallback;
      private volatile Dictionary<object, AsyncCallback> additionalCallbacks;
      private ManualResetEvent waitHandle;
      private volatile ProgressState state;

      public QueueItem(K item, object callbackKey, AsyncCallback callback)
      {
        this.Item = item;
        this.defaultCallbackKey = callbackKey;
        this.defaultCallback = callback;
      }

      public K Item { get; private set; }

      public void AddCallback(AsyncCallback ac, object key)
      {
        if (key == null || key == this.defaultCallbackKey)
          return;
        if (this.additionalCallbacks == null)
          this.additionalCallbacks = new Dictionary<object, AsyncCallback>();
        using (ItemMonitor.Lock((object) this.additionalCallbacks))
        {
          if (this.additionalCallbacks.ContainsKey(key))
            return;
          this.additionalCallbacks[key] = ac;
        }
      }

      public void ProcessCallbacks()
      {
        this.state = ProgressState.Running;
        this.defaultCallback((IAsyncResult) this);
        if (this.additionalCallbacks == null)
          return;
        using (ItemMonitor.Lock((object) this.additionalCallbacks))
          this.additionalCallbacks.Values.ForEach<AsyncCallback>((Action<AsyncCallback>) (ac => ac((IAsyncResult) this)));
      }

      public void SetCompleted()
      {
        this.state = ProgressState.Completed;
        if (this.waitHandle == null)
          return;
        this.waitHandle.Set();
      }

      public object AsyncState => (object) this.Item;

      public WaitHandle AsyncWaitHandle
      {
        get
        {
          if (this.waitHandle == null)
            this.waitHandle = new ManualResetEvent(this.state == ProgressState.Completed);
          return (WaitHandle) this.waitHandle;
        }
      }

      public bool CompletedSynchronously => false;

      public bool IsCompleted => this.state == ProgressState.Completed;

      protected override void Dispose(bool disposing)
      {
        if (disposing && this.waitHandle != null)
          this.waitHandle.Close();
        base.Dispose(disposing);
      }

      public ProgressState State => this.state;

      public int ProgressPercentage { get; set; }

      public string ProgressMessage { get; set; }

      public bool ProgressAvailable { get; set; }

      public bool Abort { get; set; }
    }

    private class ProcessData
    {
      public Thread Thread { get; set; }

      public AutoResetEvent Event { get; set; }

      public bool IsActive { get; set; }
    }
  }
}

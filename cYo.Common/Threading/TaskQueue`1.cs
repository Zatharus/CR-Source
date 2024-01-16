// Decompiled with JetBrains decompiler
// Type: cYo.Common.Threading.TaskQueue`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace cYo.Common.Threading
{
  public class TaskQueue<K> : DisposableObject
  {
    private CancellationTokenSource cts = new CancellationTokenSource();
    private AutoResetEvent queueChanged = new AutoResetEvent(false);
    private IDictionary<K, TaskQueue<K>.ProcessingItem> working = (IDictionary<K, TaskQueue<K>.ProcessingItem>) new ConcurrentDictionary<K, TaskQueue<K>.ProcessingItem>();
    private ConcurrentBag<Task> runningTasks = new ConcurrentBag<Task>();
    private IProducerConsumerCollection<TaskQueue<K>.ProcessingItem> queue;

    public TaskQueue(
      IProducerConsumerCollection<TaskQueue<K>.ProcessingItem> queue,
      int workerCount = 1)
    {
      this.queue = queue;
      this.AddWorkers(workerCount);
    }

    public TaskQueue(int capacity, int workerCount = 1)
      : this((IProducerConsumerCollection<TaskQueue<K>.ProcessingItem>) new PriorityQueue<TaskQueue<K>.ProcessingItem>()
      {
        Capacity = capacity
      }, workerCount)
    {
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      this.cts.Cancel();
      Task.WaitAll(this.runningTasks.ToArray());
    }

    public bool Enqeue(
      K key,
      Action<IProcessingItem<K>> action,
      Action<IProcessingItem<K>> completed = null)
    {
      TaskQueue<K>.ProcessingItem processingItem1;
      if (this.working.TryGetValue(key, out processingItem1))
      {
        if (completed != null)
          processingItem1.Completed += completed;
      }
      else
      {
        TaskQueue<K>.ProcessingItem processingItem2 = new TaskQueue<K>.ProcessingItem(key, action);
        if (completed != null)
          processingItem2.Completed += completed;
        if (!this.queue.TryAdd(processingItem2))
          return false;
        this.queueChanged.Set();
      }
      return true;
    }

    public void AddWorkers(int count)
    {
      CancellationToken ct = this.cts.Token;
      for (int index = 0; index < count; ++index)
        this.runningTasks.Add(Task.Factory.StartNew((Action) (() =>
        {
          while (true)
          {
            WaitHandle.WaitAny(new WaitHandle[2]
            {
              (WaitHandle) this.queueChanged,
              ct.WaitHandle
            });
            ct.ThrowIfCancellationRequested();
            TaskQueue<K>.ProcessingItem processingItem;
            while (this.queue.TryTake(out processingItem))
            {
              ct.ThrowIfCancellationRequested();
              this.working[processingItem.Item] = processingItem;
              processingItem.Execute();
              this.working.Remove(processingItem.Item);
            }
            ct.ThrowIfCancellationRequested();
          }
        }), ct));
    }

    public class ProcessingItem : IProcessingItem<K>, IProgressState
    {
      private volatile ProgressState state;

      public ProcessingItem(K key, Action<IProcessingItem<K>> action)
      {
        this.Item = key;
        this.Action = action;
      }

      public Action<IProcessingItem<K>> Action { get; private set; }

      public event Action<IProcessingItem<K>> Completed;

      public void Execute()
      {
        this.state = ProgressState.Running;
        try
        {
          this.Action((IProcessingItem<K>) this);
        }
        catch
        {
        }
        finally
        {
          this.state = ProgressState.Completed;
          if (this.Completed != null)
            this.Completed((IProcessingItem<K>) this);
        }
      }

      public override bool Equals(object x)
      {
        return x is TaskQueue<K>.ProcessingItem processingItem && object.Equals((object) processingItem.Item, (object) this.Item);
      }

      public override int GetHashCode() => this.Item.GetHashCode();

      public K Item { get; private set; }

      public ProgressState State => this.state;

      public int ProgressPercentage { get; set; }

      public string ProgressMessage { get; set; }

      public bool ProgressAvailable { get; set; }

      public bool Abort { get; set; }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.PriorityQueue`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Threading;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable disable
namespace cYo.Common.Collections
{
  public class PriorityQueue<T> : 
    IProducerConsumerCollection<T>,
    IEnumerable<T>,
    IEnumerable,
    ICollection
  {
    private readonly ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    private readonly IEqualityComparer<T> equality;
    private readonly LinkedList<T> queue = new LinkedList<T>();
    private int capacity = 100;

    public PriorityQueue(IEqualityComparer<T> equality = null)
    {
      this.equality = equality ?? (IEqualityComparer<T>) EqualityComparer<T>.Default;
      this.AddMode = PriorityQueueAddMode.AddToTop;
    }

    public PriorityQueueAddMode AddMode { get; set; }

    public int Capacity
    {
      get => this.capacity;
      set
      {
        if (this.capacity == value)
          return;
        this.Trim(this.capacity = value);
      }
    }

    public bool Add(T item)
    {
      bool flag = false;
      using (this.rwlock.WriteLock())
      {
        LinkedListNode<T> node;
        if (this.TryFindNode(item, out node))
        {
          this.queue.Remove(node);
          node.Value = item;
        }
        else
        {
          node = new LinkedListNode<T>(item);
          flag = true;
        }
        switch (this.AddMode)
        {
          case PriorityQueueAddMode.AddToBottom:
            if (flag)
              this.Trim(this.Capacity - 1);
            this.queue.AddLast(node);
            break;
          default:
            this.queue.AddFirst(node);
            if (flag)
            {
              this.Trim(this.Capacity);
              break;
            }
            break;
        }
      }
      return flag;
    }

    public bool Remove(T item)
    {
      using (this.rwlock.UpgradeableReadLock())
      {
        LinkedListNode<T> node;
        if (this.TryFindNode(item, out node))
        {
          using (this.rwlock.WriteLock())
            this.queue.Remove(node);
          return true;
        }
      }
      return false;
    }

    public void Trim(int capacity)
    {
      using (this.rwlock.UpgradeableReadLock())
      {
        if (this.queue.Count <= capacity)
          return;
        using (this.rwlock.WriteLock())
        {
          while (this.queue.Count > capacity)
          {
            LinkedListNode<T> last = this.queue.Last;
            this.queue.RemoveLast();
          }
        }
      }
    }

    public void Clear()
    {
      using (this.rwlock.WriteLock())
        this.queue.Clear();
    }

    public void Dispose() => throw new NotImplementedException();

    public IEnumerator<T> GetEnumerator()
    {
      using (this.rwlock.ReadLock())
        return this.ToArray().OfType<T>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public void CopyTo(T[] array, int index)
    {
      using (this.rwlock.ReadLock())
        this.queue.CopyTo(array, index);
    }

    public T[] ToArray()
    {
      using (this.rwlock.ReadLock())
        return this.queue.ToArray<T>();
    }

    public bool TryAdd(T item)
    {
      this.Add(item);
      return true;
    }

    public bool TryTake(out T item)
    {
      item = default (T);
      using (this.rwlock.UpgradeableReadLock())
      {
        if (this.queue.First == null)
          return false;
        item = this.queue.First.Value;
        using (this.rwlock.WriteLock())
          this.queue.RemoveFirst();
        return true;
      }
    }

    public void CopyTo(Array array, int index)
    {
      using (this.rwlock.ReadLock())
      {
        foreach (T obj in this.queue)
          array.SetValue((object) obj, index++);
      }
    }

    public int Count
    {
      get
      {
        using (this.rwlock.ReadLock())
          return this.queue.Count;
      }
    }

    public bool IsSynchronized => true;

    public object SyncRoot => (object) this.rwlock;

    private bool TryFindNode(T value, out LinkedListNode<T> node)
    {
      node = this.queue.First;
      while (node != null)
      {
        if (this.equality.Equals(node.Value, value))
          return true;
        node = node.Next;
      }
      return false;
    }
  }
}

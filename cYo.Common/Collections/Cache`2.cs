// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.Cache`2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using cYo.Common.Runtime;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable disable
namespace cYo.Common.Collections
{
  public class Cache<K, T> : DisposableObject where T : class
  {
    private readonly Dictionary<K, Cache<K, T>.CacheItem> cache = new Dictionary<K, Cache<K, T>.CacheItem>();
    private readonly LinkedList<K> accessList = new LinkedList<K>();
    private readonly Dictionary<K, Cache<K, T>.CacheItem> pending = new Dictionary<K, Cache<K, T>.CacheItem>();
    private readonly ReaderWriterLockSlim lockCache = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    private volatile int itemCapacity;
    private long sizeCapacity;
    private int minimalTimeInCache = 5000;

    private IDisposable ReadLock() => this.lockCache.ReadLock();

    private IDisposable WriteLock() => this.lockCache.WriteLock();

    private IDisposable UpgradeableReadLock() => this.lockCache.UpgradeableReadLock();

    public Cache(int itemCapacity, int sizeCapacity)
    {
      this.itemCapacity = itemCapacity;
      this.sizeCapacity = (long) sizeCapacity;
    }

    public Cache(int itemCapacity)
      : this(itemCapacity, int.MaxValue)
    {
    }

    public int ItemCapacity
    {
      get => this.itemCapacity;
      set
      {
        if (this.itemCapacity == value)
          return;
        this.itemCapacity = value;
        this.Trim();
      }
    }

    public long SizeCapacity
    {
      get => Interlocked.Read(ref this.sizeCapacity);
      set
      {
        if (this.SizeCapacity == value)
          return;
        Interlocked.Exchange(ref this.sizeCapacity, value);
        this.Trim();
      }
    }

    public int MinimalTimeInCache
    {
      get => this.minimalTimeInCache;
      set
      {
        if (this.minimalTimeInCache == value)
          return;
        this.minimalTimeInCache = value;
        this.Trim();
      }
    }

    public int Count
    {
      get
      {
        using (this.ReadLock())
          return this.accessList.Count;
      }
    }

    public long Size
    {
      get
      {
        using (this.ReadLock())
          return this.cache.Values.Sum<Cache<K, T>.CacheItem>((Func<Cache<K, T>.CacheItem, long>) (v => (long) Cache<K, T>.GetDataSize(v.Data)));
      }
    }

    public IItemLock<T> LockItem(K key, Func<K, T> create, bool dontUpdateAccess)
    {
      IItemLock<T> itemLock = this.OpenItem(key, dontUpdateAccess, create != null);
      if (itemLock != null && (object) itemLock.Item == null)
      {
        T obj = default (T);
        Cache<K, T>.CacheItem cacheItem = (Cache<K, T>.CacheItem) null;
        try
        {
          obj = create(key);
        }
        catch (Exception ex)
        {
        }
        using (this.WriteLock())
        {
          if ((object) obj != null)
          {
            cacheItem = itemLock.LockObject as Cache<K, T>.CacheItem;
            cacheItem.Data = itemLock.Item = obj;
            this.cache[key] = cacheItem;
            while (this.accessList.Contains(key))
              this.accessList.Remove(key);
            this.accessList.AddLast(cacheItem.AccessNode);
          }
          else
          {
            itemLock.Dispose();
            itemLock = (IItemLock<T>) null;
          }
          this.pending.Remove(key);
        }
        if ((object) obj != null)
        {
          itemLock.Dispose();
          try
          {
            this.OnItemAdded(new CacheItemEventArgs<K, T>(key, obj));
          }
          catch
          {
          }
          this.Trim(key);
          itemLock = (IItemLock<T>) cacheItem.GetLock();
        }
      }
      return itemLock;
    }

    public IItemLock<T> LockItem(K key, bool dontUpdateAccess)
    {
      return this.LockItem(key, (Func<K, T>) null, dontUpdateAccess);
    }

    public IItemLock<T> LockItem(K key, Func<K, T> create) => this.LockItem(key, create, false);

    public IItemLock<T> LockItem(K key, T data) => this.LockItem(key, (Func<K, T>) (k => data));

    public bool IsCached(K key)
    {
      using (this.ReadLock())
        return this.cache.ContainsKey(key);
    }

    public bool IsLocked(K key)
    {
      using (this.ReadLock())
      {
        Cache<K, T>.CacheItem cacheItem;
        return this.cache.TryGetValue(key, out cacheItem) && cacheItem.IsLocked;
      }
    }

    public bool IsCaching(K key)
    {
      using (this.ReadLock())
        return this.pending.ContainsKey(key);
    }

    public void RemoveItem(K key, bool evenWhenLocked)
    {
      Cache<K, T>.CacheItem cacheItem = this.RemoveItemInternal(key, evenWhenLocked);
      if (cacheItem == null)
        return;
      this.OnItemRemoved(new CacheItemEventArgs<K, T>(key, cacheItem.Data));
    }

    public void RemoveItem(K key) => this.RemoveItem(key, false);

    public void UpdateKeys(Func<K, bool> select, Action<K> update)
    {
      using (this.WriteLock())
      {
        foreach (K key in ((IEnumerable<K>) this.cache.Keys.ToArray<K>()).Where<K>(select))
        {
          Cache<K, T>.CacheItem cacheItem = this.cache[key];
          this.cache.Remove(key);
          update(key);
          this.cache.Add(key, cacheItem);
        }
      }
    }

    public void RemoveKeys(Func<K, bool> select)
    {
      using (this.WriteLock())
        ((IEnumerable<K>) this.cache.Keys.ToArray<K>()).Where<K>(select).ForEach<K>(new Action<K>(this.RemoveItem));
    }

    public K[] GetKeys()
    {
      using (this.ReadLock())
        return this.cache.Keys.ToArray<K>();
    }

    public void Clear(bool evenLocked)
    {
      K[] array;
      using (this.ReadLock())
        array = this.cache.Keys.Where<K>((Func<K, bool>) (k => !this.IsLocked(k) | evenLocked)).ToArray<K>();
      ((IEnumerable<K>) array).ForEach<K>((Action<K>) (key => this.RemoveItem(key, true)));
    }

    public event EventHandler<CacheItemEventArgs<K, T>> ItemAdded;

    public event EventHandler<CacheItemEventArgs<K, T>> ItemRemoved;

    public event EventHandler SizeChanged;

    protected virtual void OnItemAdded(CacheItemEventArgs<K, T> cacheItemEventArgs)
    {
      if (this.ItemAdded != null)
        this.ItemAdded((object) this, cacheItemEventArgs);
      this.OnSizeChanged();
    }

    protected virtual void OnItemRemoved(CacheItemEventArgs<K, T> cacheItemEventArgs)
    {
      if (this.ItemRemoved != null)
        this.ItemRemoved((object) this, cacheItemEventArgs);
      this.OnSizeChanged();
    }

    protected virtual void OnSizeChanged()
    {
      if (this.SizeChanged == null)
        return;
      this.SizeChanged((object) this, EventArgs.Empty);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.Clear(true);
      base.Dispose(disposing);
    }

    private IItemLock<T> OpenItem(K key, bool dontUpdateAccess, bool addNew)
    {
      Cache<K, T>.CacheItem ci;
      using (this.UpgradeableReadLock())
      {
        if (this.cache.TryGetValue(key, out ci))
        {
          if (!dontUpdateAccess)
          {
            using (this.WriteLock())
              this.UpdateAccess(ci);
          }
        }
        else if (addNew)
        {
          if (!this.pending.TryGetValue(key, out ci))
          {
            ci = new Cache<K, T>.CacheItem(default (T));
            ci.AccessNode = new LinkedListNode<K>(key);
            using (this.WriteLock())
              this.pending[key] = ci;
          }
        }
      }
      if (ci != null)
        return (IItemLock<T>) ci.GetLock();
      return (IItemLock<T>) null;
    }

    private Cache<K, T>.CacheItem RemoveItemInternal(K key, bool evenWhenLocked, bool onlyExpired)
    {
      using (this.UpgradeableReadLock())
      {
        Cache<K, T>.CacheItem cacheItem;
        if (!this.cache.TryGetValue(key, out cacheItem) || cacheItem.IsLocked && !evenWhenLocked || DateTime.Now.Ticks - cacheItem.LastAccess < (long) this.MinimalTimeInCache & onlyExpired)
          return (Cache<K, T>.CacheItem) null;
        using (this.WriteLock())
        {
          this.cache.Remove(key);
          this.accessList.Remove(cacheItem.AccessNode);
        }
        return cacheItem;
      }
    }

    private Cache<K, T>.CacheItem RemoveItemInternal(K key, bool evenWhenLocked)
    {
      return this.RemoveItemInternal(key, evenWhenLocked, false);
    }

    private void UpdateAccess(Cache<K, T>.CacheItem ci)
    {
      try
      {
        this.accessList.Remove(ci.AccessNode);
      }
      catch
      {
      }
      this.accessList.AddLast(ci.AccessNode);
    }

    private static int GetDataSize(T o) => o is IDataSize dataSize ? dataSize.DataSize : 0;

    private void Trim(K keep)
    {
      List<CacheItemEventArgs<K, T>> cacheItemEventArgsList = new List<CacheItemEventArgs<K, T>>();
      long ticks = Machine.Ticks;
      using (this.UpgradeableReadLock())
      {
        long size = this.Size;
        LinkedListNode<K> next;
        for (LinkedListNode<K> linkedListNode = this.accessList.First; linkedListNode != null; linkedListNode = next)
        {
          if (this.accessList.Count <= this.itemCapacity)
            goto label_7;
label_2:
          next = linkedListNode.Next;
          Cache<K, T>.CacheItem cacheItem = object.Equals((object) linkedListNode.Value, (object) keep) ? (Cache<K, T>.CacheItem) null : this.RemoveItemInternal(linkedListNode.Value, false, true);
          if (cacheItem != null)
          {
            size -= (long) Cache<K, T>.GetDataSize(cacheItem.Data);
            cacheItemEventArgsList.Add(new CacheItemEventArgs<K, T>(linkedListNode.Value, cacheItem.Data));
            continue;
          }
          continue;
label_7:
          if (size > this.sizeCapacity)
            goto label_2;
          else
            break;
        }
      }
      foreach (CacheItemEventArgs<K, T> cacheItemEventArgs in cacheItemEventArgsList)
        this.OnItemRemoved(cacheItemEventArgs);
    }

    public void Trim() => this.Trim(default (K));

    private class CacheItem
    {
      private int lockCount;
      private long lastAccess;

      public CacheItem(T data) => this.Data = data;

      public int LockCount => this.lockCount;

      public bool IsLocked => this.lockCount > 0;

      public long LastAccess => this.lastAccess;

      public T Data { get; set; }

      public LinkedListNode<K> AccessNode { get; set; }

      public ItemLock<T> GetLock()
      {
        ItemLock<T> itemLock = new ItemLock<T>(this.Data, (object) this)
        {
          Item = this.Data,
          LockObject = (object) this
        };
        ++this.lockCount;
        itemLock.Disposed += new EventHandler(this.LockItemDisposed);
        this.lastAccess = Machine.Ticks;
        return itemLock;
      }

      private void LockItemDisposed(object sender, EventArgs e)
      {
        if (--this.lockCount >= 0)
          return;
        this.lockCount = 0;
      }
    }
  }
}

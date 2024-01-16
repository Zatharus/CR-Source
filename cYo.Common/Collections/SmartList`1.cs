// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.SmartList`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace cYo.Common.Collections
{
  [Serializable]
  public class SmartList<T> : 
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable,
    IList,
    ICollection,
    IMoveable,
    ICloneable
  {
    private List<T> innerList = new List<T>();
    private volatile SmartListOptions flags;
    [NonSerialized]
    private ReaderWriterLockSlim slimLock;

    public SmartList(SmartListOptions flags) => this.flags = flags;

    public SmartList()
      : this(SmartListOptions.Default)
    {
    }

    public SmartList(IEnumerable<T> list) => this.innerList = new List<T>(list);

    public SmartList(SmartList<T> list)
    {
      using (list.GetLock(true))
        this.innerList = new List<T>((IEnumerable<T>) list);
      this.flags = list.flags;
    }

    public void AddRange(IEnumerable<T> list)
    {
      if (list is ICollection<T> objs)
        this.innerList.Capacity = Math.Max(this.innerList.Capacity, this.Count + objs.Count);
      foreach (T obj in list)
        this.Add(obj);
    }

    public void RemoveRange(IEnumerable<T> list)
    {
      foreach (T obj in list)
        this.Remove(obj);
    }

    private void DoSort(Action sortCall)
    {
      using (this.GetLock(true))
        sortCall();
      if ((this.flags & SmartListOptions.DisableOnRefresh) != SmartListOptions.None)
        return;
      this.OnRefreshCompleted();
    }

    public void Sort(IComparer<T> comparer)
    {
      this.DoSort((Action) (() => this.innerList.Sort(comparer)));
    }

    public void Sort(Comparison<T> comparison)
    {
      this.DoSort((Action) (() => this.innerList.Sort(comparison)));
    }

    public void Sort() => this.DoSort(new Action(this.innerList.Sort));

    public SmartList<U> ConvertAll<U>(Converter<T, U> converter)
    {
      SmartList<U> smartList = new SmartList<U>();
      using (this.GetLock(false))
      {
        foreach (T inner in this.innerList)
          smartList.Add(converter(inner));
      }
      return smartList;
    }

    public bool TrueForAll(Predicate<T> predicate)
    {
      using (this.GetLock(false))
        return this.innerList.TrueForAll(predicate);
    }

    public T Find(Predicate<T> predicate)
    {
      using (this.GetLock(false))
        return this.innerList.Find(predicate);
    }

    public SmartList<T> FindAll(Predicate<T> predicate)
    {
      SmartList<T> all = new SmartList<T>();
      using (this.GetLock(false))
      {
        foreach (T inner in this.innerList)
        {
          if (predicate(inner))
            all.Add(inner);
        }
        return all;
      }
    }

    public bool Exists(Predicate<T> predicate)
    {
      using (this.GetLock(false))
        return this.innerList.Exists(predicate);
    }

    public void ForEach(Action<T> action, bool copy = false)
    {
      if (this.Count == 0)
        return;
      if (copy)
      {
        foreach (T obj in this.ToArray())
          action(obj);
      }
      else
      {
        using (this.GetLock(false))
          this.innerList.ForEach(action);
      }
    }

    public T[] ToArray()
    {
      using (this.GetLock(false))
        return this.innerList.ToArray();
    }

    public List<T> ToList()
    {
      using (this.GetLock(false))
        return new List<T>((IEnumerable<T>) this.innerList);
    }

    public void Trim(int count)
    {
      if (count < 0)
        throw new ArgumentException("count must be >= 0");
      if (count == 0)
      {
        this.Clear();
      }
      else
      {
        while (this.Count > count)
          this.RemoveAt(this.Count - 1);
      }
    }

    public void TrimExcess()
    {
      using (this.GetLock(true))
        this.innerList.TrimExcess();
    }

    public void Move(int oldIndex, int newIndex)
    {
      if (oldIndex == newIndex)
        return;
      T inner;
      using (this.GetLock(true))
      {
        if (oldIndex < 0 || oldIndex >= this.Count || newIndex < 0 || newIndex >= this.Count)
          return;
        inner = this.innerList[oldIndex];
        this.innerList.RemoveAt(oldIndex);
        if (newIndex > oldIndex + 1)
          --newIndex;
        this.innerList.Insert(newIndex, inner);
      }
      this.InvokeChanged(SmartListAction.Move, newIndex, inner, inner);
    }

    public void Move(T item, int newIndex) => this.Move(this.IndexOf(item), newIndex);

    public void MoveRelative(int n, int delta) => this.Move(n, n + delta);

    public void MoveRelative(T item, int delta) => this.MoveRelative(this.IndexOf(item), delta);

    public void MoveToBeginning(T item) => this.Move(item, 0);

    public void MoveToEnd(T item)
    {
      using (this.GetLock(true))
        this.Move(item, this.Count - 1);
    }

    public bool IsAtStart(T item) => this.IndexOf(item) == 0;

    public bool IsAtEnd(T item) => this.IndexOf(item) == this.Count - 1;

    public T GetItemOrDefault(int i)
    {
      try
      {
        return this[i];
      }
      catch
      {
        return default (T);
      }
    }

    public SmartListOptions Flags
    {
      get => this.flags;
      set => this.flags = value;
    }

    public T First => this.Count != 0 ? this[0] : default (T);

    public T Last => this.Count != 0 ? this[this.Count - 1] : default (T);

    public void Add(T item)
    {
      bool flag = (this.flags & SmartListOptions.DisableOnInsert) == SmartListOptions.None;
      if (flag)
        this.OnInsert(this.Count, item);
      int index;
      using (this.GetLock(true))
      {
        this.innerList.Add(item);
        index = this.innerList.Count - 1;
      }
      if (!flag)
        return;
      this.OnInsertCompleted(index, item);
    }

    public void Clear()
    {
      if ((this.flags & SmartListOptions.DisableOnClear) != SmartListOptions.None)
        this.OnClear();
      if ((this.flags & SmartListOptions.ClearWithRemove) != SmartListOptions.None)
      {
        for (int index = this.Count - 1; index >= 0; --index)
        {
          try
          {
            this.RemoveAt(index);
          }
          catch (IndexOutOfRangeException ex)
          {
          }
          catch (ArgumentOutOfRangeException ex)
          {
          }
        }
      }
      else
      {
        using (this.GetLock(true))
          this.innerList.Clear();
      }
      if ((this.flags & SmartListOptions.DisableOnClear) != SmartListOptions.None)
        return;
      this.OnClearCompleted();
    }

    public bool Contains(T item)
    {
      using (this.GetLock(false))
        return this.innerList.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      using (this.GetLock(false))
        this.innerList.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get
      {
        using (this.GetLock(false))
          return this.innerList.Count;
      }
    }

    public bool IsReadOnly => false;

    public bool Remove(T item)
    {
      bool flag = (this.flags & SmartListOptions.DisableOnRemove) == SmartListOptions.None;
      int index = this.IndexOf(item);
      if (index == -1)
        return false;
      if (flag)
        this.OnRemove(index, item);
      using (this.GetLock(true))
        this.innerList.Remove(item);
      if (flag)
        this.OnRemoveCompleted(index, item);
      return true;
    }

    public int IndexOf(T item)
    {
      using (this.GetLock(false))
        return this.innerList.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      bool flag = (this.flags & SmartListOptions.DisableOnInsert) == SmartListOptions.None;
      if (flag)
        this.OnInsert(index, item);
      using (this.GetLock(true))
        this.innerList.Insert(index, item);
      if (!flag)
        return;
      this.OnInsertCompleted(index, item);
    }

    public void RemoveAt(int index)
    {
      bool flag = (this.flags & SmartListOptions.DisableOnRemove) == SmartListOptions.None;
      T obj = this[index];
      if (flag)
        this.OnRemove(index, obj);
      using (this.GetLock(true))
        this.innerList.RemoveAt(index);
      if (!flag)
        return;
      this.OnRemoveCompleted(index, obj);
    }

    public T this[int index]
    {
      get
      {
        using (this.GetLock(false))
          return this.innerList[index];
      }
      set
      {
        T obj = this[index];
        if ((this.flags & SmartListOptions.CheckedSet) != SmartListOptions.None && object.Equals((object) obj, (object) value))
          return;
        if ((this.flags & SmartListOptions.DisableOnInsert) == SmartListOptions.None)
          this.OnInsert(index, value);
        if ((this.flags & SmartListOptions.DisableOnRemove) == SmartListOptions.None)
          this.OnRemove(index, obj);
        if ((this.flags & SmartListOptions.DisableOnSet) == SmartListOptions.None)
          this.OnSet(index, value, obj);
        using (this.GetLock(true))
          this.innerList[index] = value;
        if ((this.flags & SmartListOptions.DisableOnSet) == SmartListOptions.None)
          this.OnSetCompleted(index, value, obj);
        if ((this.flags & SmartListOptions.DisableOnRemove) == SmartListOptions.None)
          this.OnRemoveCompleted(index, obj);
        if ((this.flags & SmartListOptions.DisableOnInsert) != SmartListOptions.None)
          return;
        this.OnInsertCompleted(index, value);
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this.IsSynchronized ? this.LockedEnumerable().GetEnumerator() : (IEnumerator<T>) this.innerList.GetEnumerator();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      using (this.GetLock(false))
        ((ICollection) this.innerList).CopyTo(array, index);
    }

    public bool IsSynchronized => (this.flags & SmartListOptions.Synchronized) != 0;

    public object SyncRoot => ((ICollection) this.innerList).SyncRoot;

    int IList.Add(object value)
    {
      this.Add((T) value);
      return this.Count - 1;
    }

    bool IList.Contains(object value) => this.Contains((T) value);

    int IList.IndexOf(object value) => this.IndexOf((T) value);

    void IList.Insert(int index, object value) => this.Insert(index, (T) value);

    bool IList.IsFixedSize => false;

    void IList.Remove(object value) => this.Remove((T) value);

    object IList.this[int index]
    {
      get => (object) this[index];
      set => this[index] = (T) value;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public object Clone() => (object) new SmartList<T>(this);

    protected virtual void OnValidate(T item)
    {
    }

    protected virtual void OnInsert(int index, T item) => this.OnValidate(item);

    protected virtual void OnInsertCompleted(int index, T item)
    {
      this.InvokeChanged(SmartListAction.Insert, index, item, item);
    }

    protected virtual void OnRemove(int index, T item)
    {
    }

    protected virtual void OnRemoveCompleted(int index, T item)
    {
      this.InvokeChanged(SmartListAction.Remove, index, item, item);
      if ((this.flags & SmartListOptions.DisposeOnRemove) == SmartListOptions.None || !(item is IDisposable disposable))
        return;
      disposable.Dispose();
    }

    protected virtual void OnSet(int index, T newItem, T oldItem) => this.OnValidate(newItem);

    protected virtual void OnSetCompleted(int index, T item, T oldItem)
    {
      this.InvokeChanged(SmartListAction.Set, index, item, oldItem);
    }

    protected virtual void OnClear()
    {
    }

    protected virtual void OnClearCompleted()
    {
      this.InvokeChanged(SmartListAction.Clear, -1, default (T), default (T));
    }

    protected virtual void OnRefreshCompleted()
    {
      this.InvokeChanged(SmartListAction.Refresh, -1, default (T), default (T));
    }

    [field: NonSerialized]
    public event EventHandler<SmartListChangedEventArgs<T>> Changed;

    private void InvokeChanged(SmartListAction action, int index, T item, T oldItem)
    {
      if (this.Changed == null || (this.flags & SmartListOptions.DisableCollectionChangedEvent) != SmartListOptions.None)
        return;
      this.Changed((object) this, new SmartListChangedEventArgs<T>(action, index, item, oldItem));
    }

    protected IDisposable GetLock(bool write)
    {
      if (!this.IsSynchronized)
        return (IDisposable) null;
      if (this.slimLock == null)
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.slimLock == null)
            this.slimLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }
      }
      return !write ? this.slimLock.ReadLock() : this.slimLock.WriteLock();
    }

    protected IEnumerable<T> LockedEnumerable()
    {
      using (this.GetLock(false))
      {
        foreach (T inner in this.innerList)
          yield return inner;
      }
    }

    public static SmartList<T> Adapter(List<T> list, SmartListOptions flags)
    {
      return new SmartList<T>(flags) { innerList = list };
    }

    public static SmartList<T> Adapter(List<T> list)
    {
      return SmartList<T>.Adapter(list, SmartListOptions.Default);
    }
  }
}

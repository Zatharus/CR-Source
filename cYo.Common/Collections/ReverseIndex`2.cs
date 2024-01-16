// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.ReverseIndex`2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Common.Collections
{
  [Serializable]
  public class ReverseIndex<T, K>
  {
    private readonly List<T> complete = new List<T>();
    private readonly Dictionary<K, ICollection<T>> index = new Dictionary<K, ICollection<T>>();

    public int Size => this.index.Count;

    public IEnumerable<K> Keys => (IEnumerable<K>) this.index.Keys;

    public void Add(T item, K key)
    {
      ICollection<T> o;
      using (ItemMonitor.Lock((object) this.index))
      {
        if (!this.index.TryGetValue(key, out o))
          this.index[key] = o = (ICollection<T>) new List<T>();
      }
      using (ItemMonitor.Lock((object) o))
        o.Add(item);
      using (ItemMonitor.Lock((object) this.complete))
        this.complete.Add(item);
    }

    public void Add(T item, IEnumerable<K> keys)
    {
      foreach (K key in keys)
        this.Add(item, key);
    }

    public void AddRange(IEnumerable<T> items, Func<T, K> predicate)
    {
      foreach (T obj in items)
        this.Add(obj, predicate(obj));
    }

    public void AddRange(IEnumerable<T> items, Func<T, IEnumerable<K>> predicate)
    {
      items.ParallelForEach<T>((Action<T>) (item =>
      {
        foreach (K key in predicate(item))
          this.Add(item, key);
      }));
    }

    public void Remove(T item)
    {
      using (ItemMonitor.Lock((object) this.index))
      {
        foreach (KeyValuePair<K, ICollection<T>> keyValuePair in this.index.Where<KeyValuePair<K, ICollection<T>>>((Func<KeyValuePair<K, ICollection<T>>, bool>) (kvp => kvp.Value.Contains(item))).ToArray<KeyValuePair<K, ICollection<T>>>())
        {
          using (ItemMonitor.Lock((object) keyValuePair.Value))
          {
            keyValuePair.Value.Remove(item);
            if (keyValuePair.Value.Count == 0)
              this.index.Remove(keyValuePair.Key);
          }
        }
      }
      using (ItemMonitor.Lock((object) this.complete))
        this.complete.Remove(item);
    }

    public IEnumerable<T> Match(K key)
    {
      using (ItemMonitor.Lock((object) this.index))
      {
        ICollection<T> source;
        return this.index.TryGetValue(key, out source) ? (IEnumerable<T>) source.ToArray<T>() : (IEnumerable<T>) new T[0];
      }
    }
  }
}

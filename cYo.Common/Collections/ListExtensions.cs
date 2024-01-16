// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.ListExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable
namespace cYo.Common.Collections
{
  public static class ListExtensions
  {
    public static bool ParallelEnabled = true;

    public static T[] CreateCopy<T>(this T[] data)
    {
      T[] copy = new T[data.Length];
      data.CopyTo((Array) copy, 0);
      return copy;
    }

    public static List<T> SafeAdd<T>(this List<T> list, T item)
    {
      if (list == null)
        list = new List<T>();
      list.Add(item);
      return list;
    }

    public static void Trim(this IList list, int length)
    {
      if (length > list.Count)
        return;
      if (length == 0)
      {
        list.Clear();
      }
      else
      {
        for (int index = list.Count - 1; index >= length; --index)
          list.RemoveAt(index);
      }
    }

    public static IList<T> Randomize<T>(this IList<T> list, int seed = 0)
    {
      Random random = seed == 0 ? new Random() : new Random(seed);
      int count = list.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        int index2 = random.Next(count - 1);
        int index3 = random.Next(count - 1);
        T obj = list[index2];
        list[index2] = list[index3];
        list[index3] = obj;
      }
      return list;
    }

    public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
    {
      foreach (T obj in items)
        list.Add(obj);
    }

    public static void AddRange<T, K>(this IDictionary<T, K> dict, IDictionary<T, K> values)
    {
      foreach (KeyValuePair<T, K> keyValuePair in (IEnumerable<KeyValuePair<T, K>>) values)
        dict[keyValuePair.Key] = keyValuePair.Value;
    }

    public static void RemoveRange<T>(this ICollection<T> list, IEnumerable<T> items)
    {
      foreach (T obj in items)
        list.Remove(obj);
    }

    public static void RemoveAll<T>(this ICollection<T> collection, Predicate<T> filter)
    {
      collection.RemoveRange<T>((IEnumerable<T>) collection.Where<T>((Func<T, bool>) (t => filter(t))).ToArray<T>());
    }

    public static void RemoveAll<T>(this IList collection, Predicate<T> filter)
    {
      foreach (T obj in collection.OfType<T>().Where<T>((Func<T, bool>) (t => filter(t))).ToArray<T>())
        collection.Remove((object) obj);
    }

    public static T[] Sort<T>(this T[] array)
    {
      Array.Sort<T>(array);
      return array;
    }

    public static IEnumerable<T> Recurse<T>(
      this IEnumerable list,
      Func<object, IEnumerable> getItems,
      bool bottomUp = false,
      int maxLevel = -1)
      where T : class
    {
      if (maxLevel != 0)
      {
        foreach (object t in list)
        {
          T ti = t as T;
          if ((object) ti != null && !bottomUp)
            yield return ti;
          IEnumerable list1 = getItems != null ? getItems(t) : (IEnumerable) null;
          if (list1 != null)
          {
            foreach (T obj in list1.Recurse<T>(getItems, bottomUp, maxLevel - 1))
              yield return obj;
          }
          if ((object) ti != null & bottomUp)
            yield return ti;
          ti = default (T);
        }
      }
    }

    public static int GetChildLevel<T>(
      this IEnumerable list,
      T item,
      Func<object, IList> getItems,
      int level)
      where T : class
    {
      foreach (object obj1 in list)
      {
        if (obj1 is T obj2 && obj2.Equals((object) item))
          return level;
        IList list1 = getItems != null ? getItems(obj1) : (IList) null;
        if (list1 != null)
        {
          int childLevel = list1.GetChildLevel<T>(item, getItems, level + 1);
          if (childLevel != -1)
            return childLevel;
        }
      }
      return -1;
    }

    public static void Dispose(this IEnumerable list)
    {
      if (list == null)
        return;
      list.OfType<IDisposable>().SafeForEach<IDisposable>((Action<IDisposable>) (d => d.Dispose()));
    }

    public static void RemoveReference(this IList list, object value)
    {
      int index = list.IndexOfReference(value);
      if (index == -1)
        return;
      list.RemoveAt(index);
    }

    public static void Move(this IList list, int oldIndex, int newIndex)
    {
      if (oldIndex == newIndex)
        return;
      object obj = list[oldIndex];
      list.RemoveAt(oldIndex);
      if (newIndex > oldIndex + 1)
        --newIndex;
      list.Insert(newIndex, obj);
    }

    public static IEnumerable<T> Lock<T>(this IEnumerable<T> list, bool useSyncRoot = false)
    {
      ICollection collection = useSyncRoot ? list as ICollection : (ICollection) null;
      using (ItemMonitor.Lock(collection == null ? (object) list : collection.SyncRoot))
      {
        foreach (T obj in list)
          yield return obj;
      }
    }

    public static IEnumerable<T> AsEnumerable<T>(params T[] data) => (IEnumerable<T>) data;

    public static T Max<T>(this IEnumerable<T> items, Comparison<T> comparision)
    {
      return items.Aggregate<T>((Func<T, T, T>) ((a, b) => comparision(a, b) <= 0 ? b : a));
    }

    public static IEnumerable<T> AddFirst<T>(this IEnumerable<T> list, T item)
    {
      IEnumerable<T> first = ListExtensions.AsEnumerable<T>(item);
      return list != null ? first.Concat<T>(list) : first;
    }

    public static IEnumerable<T> AddLast<T>(this IEnumerable<T> list, T item)
    {
      IEnumerable<T> second = ListExtensions.AsEnumerable<T>(item);
      return list != null ? list.Concat<T>(second) : second;
    }

    public static void ForFirst<T>(this IEnumerable<T> list, Action<T> action)
    {
      if (list == null)
        return;
      list.Take<T>(1).ForEach<T>(action);
    }

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
      if (list == null)
        return;
      foreach (T obj in list)
        action(obj);
    }

    public static void SafeForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
      if (list == null)
        return;
      foreach (T obj in list)
      {
        try
        {
          action(obj);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static bool IsEmpty<T>(this IEnumerable<T> list) => list == null || !list.Any<T>();

    public static int FindIndex<T>(this IEnumerable<T> list, Predicate<T> predicate)
    {
      int index = 0;
      foreach (T obj in list)
      {
        if (predicate(obj))
          return index;
        ++index;
      }
      return -1;
    }

    public static T FirstOrValue<T>(this IEnumerable<T> list, T value)
    {
      using (IEnumerator<T> enumerator = list.GetEnumerator())
      {
        if (enumerator.MoveNext())
          return enumerator.Current;
      }
      return value;
    }

    public static int IndexOfReference(this IEnumerable list, object value)
    {
      int num = 0;
      foreach (object obj in list)
      {
        if (obj == value)
          return num;
        ++num;
      }
      return -1;
    }

    public static ParallelQuery<T> AsParallelSafe<T>(this IEnumerable<T> list)
    {
      return !(list is ParallelQuery<T>) ? list.AsParallel<T>() : (ParallelQuery<T>) list;
    }

    public static void ParallelForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
      if (!ListExtensions.ParallelEnabled)
      {
        items.ForEach<T>(action);
      }
      else
      {
        Exception lastException = (Exception) null;
        Parallel.ForEach<T>(items, (Action<T>) (item =>
        {
          try
          {
            action(item);
          }
          catch (Exception ex)
          {
            lastException = ex;
          }
        }));
        if (lastException != null)
          throw lastException;
      }
    }

    public static void ParallelSort<T>(
      this IList<T> array,
      IComparer<T> comparer,
      bool forceSequential = false)
    {
      array.ParallelSort<T>(new Comparison<T>(comparer.Compare), forceSequential);
    }

    public static void ParallelSort<T>(
      this IList<T> array,
      Comparison<T> comparer,
      bool forceSequential = false)
    {
      array.ParallelSort<T>(0, array.Count - 1, comparer, forceSequential);
    }

    private static void ParallelSort<T>(
      this IList<T> array,
      int left,
      int right,
      Comparison<T> comparer,
      bool forceSequential = false)
    {
      forceSequential |= !ListExtensions.ParallelEnabled;
      if (left >= right || right <= left)
        return;
      ListExtensions.Swap<T>(array, left, (left + right) / 2);
      int last = left;
      for (int index = left + 1; index <= right; ++index)
      {
        if (comparer(array[index], array[left]) < 0)
        {
          ++last;
          ListExtensions.Swap<T>(array, last, index);
        }
      }
      ListExtensions.Swap<T>(array, left, last);
      if (forceSequential || last - left < 512)
      {
        array.ParallelSort<T>(left, last - 1, comparer, forceSequential);
        array.ParallelSort<T>(last + 1, right, comparer, forceSequential);
      }
      else
        Parallel.Invoke((Action) (() => array.ParallelSort<T>(left, last - 1, comparer, forceSequential)), (Action) (() => array.ParallelSort<T>(last + 1, right, comparer, forceSequential)));
    }

    private static void Swap<T>(IList<T> array, int i, int j)
    {
      T obj = array[i];
      array[i] = array[j];
      array[j] = obj;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.ReverseTextIndex`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Common.Text
{
  public class ReverseTextIndex<T>
  {
    public const int MinimumKeyLength = 3;
    private readonly List<T> complete = new List<T>();
    private readonly Dictionary<string, ICollection<T>> index = new Dictionary<string, ICollection<T>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static char[] wordSeparators = " \r\n\t,;.:!?()[]{}-'´`\\/\"Â ‘’“”…".ToArray<char>();
    private static Regex rxWords = new Regex("\\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Dictionary<string, string[]> wordMap = new Dictionary<string, string[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public ReverseTextIndex(ReverseTextIndexMode mode = ReverseTextIndexMode.Words)
    {
      this.ReverseIndexMode = mode;
    }

    public ReverseTextIndexMode ReverseIndexMode { get; private set; }

    public int Size => this.index.Count;

    public IEnumerable<string> Keys => (IEnumerable<string>) this.index.Keys;

    public void Add(T item, string text)
    {
      foreach (string key in this.Split(text))
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
    }

    public void Add(T item, IEnumerable<string> texts)
    {
      foreach (string text in texts)
        this.Add(item, text);
    }

    public void AddRange(IEnumerable<T> items, Func<T, string> predicate)
    {
      foreach (T obj in items)
        this.Add(obj, predicate(obj));
    }

    public void AddRange(IEnumerable<T> items, Func<T, IEnumerable<string>> predicate)
    {
      items.ParallelForEach<T>((Action<T>) (item =>
      {
        foreach (string text in predicate(item))
          this.Add(item, text);
      }));
    }

    public void Remove(T item)
    {
      using (ItemMonitor.Lock((object) this.index))
      {
        foreach (KeyValuePair<string, ICollection<T>> keyValuePair in this.index.Where<KeyValuePair<string, ICollection<T>>>((Func<KeyValuePair<string, ICollection<T>>, bool>) (kvp => kvp.Value.Contains(item))).ToArray<KeyValuePair<string, ICollection<T>>>())
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

    public IEnumerable<T> Match(string text, bool matchAny = false)
    {
      IEnumerable<T> objs1 = (IEnumerable<T>) null;
      foreach (string word in ReverseTextIndex<T>.SplitToWords(text))
      {
        IEnumerable<T> objs2;
        using (ItemMonitor.Lock((object) this.index))
        {
          ICollection<T> objs3;
          objs2 = word.Length >= 3 ? (this.index.TryGetValue(word, out objs3) ? (IEnumerable<T>) objs3 : Enumerable.Empty<T>()) : this.complete.Lock<T>();
        }
        if (objs1 == null)
          objs1 = objs2.Lock<T>();
        else if (matchAny)
        {
          objs1 = objs1.Union<T>(objs2.Lock<T>());
          if (objs1.Count<T>() == this.complete.Count<T>())
            break;
        }
        else
        {
          if (objs2.Count<T>() < objs1.Count<T>())
            objs1 = objs1.Intersect<T>(objs2.Lock<T>());
          if (objs1.Count<T>() == 0)
            break;
        }
      }
      return objs1;
    }

    private static IEnumerable<string> SplitToWords(string text)
    {
      return (IEnumerable<string>) text.Split(ReverseTextIndex<T>.wordSeparators, StringSplitOptions.RemoveEmptyEntries);
    }

    private static IEnumerable<string> SplitWordParts(string w, int mkl)
    {
      for (int l = w.Length; mkl <= l; ++mkl)
      {
        for (int i = 0; i <= l - mkl; ++i)
          yield return w.Substring(i, mkl);
      }
    }

    private IEnumerable<string> Split(string text)
    {
      switch (this.ReverseIndexMode)
      {
        case ReverseTextIndexMode.Letters:
          foreach (string str in ReverseTextIndex<T>.SplitToWords(text).Where<string>((Func<string, bool>) (w => w.Length >= 3)))
          {
            string[] array;
            using (ItemMonitor.Lock((object) ReverseTextIndex<T>.wordMap))
            {
              if (!ReverseTextIndex<T>.wordMap.TryGetValue(str, out array))
                ReverseTextIndex<T>.wordMap[str] = array = ReverseTextIndex<T>.SplitWordParts(str, 3).ToArray<string>();
            }
            string[] strArray = array;
            for (int index = 0; index < strArray.Length; ++index)
              yield return strArray[index];
            strArray = (string[]) null;
          }
          break;
        case ReverseTextIndexMode.Key:
          yield return text;
          break;
        default:
          foreach (string str in ReverseTextIndex<T>.SplitToWords(text).Where<string>((Func<string, bool>) (w => w.Length >= 3)))
            yield return str;
          break;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.ChainedComparer`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class ChainedComparer<T> : Comparer<T>
  {
    private readonly IComparer<T>[] comparers;
    private readonly int len;

    public ChainedComparer(IEnumerable<IComparer<T>> comparers)
    {
      this.comparers = comparers.Where<IComparer<T>>((Func<IComparer<T>, bool>) (c => c != null)).Distinct<IComparer<T>>((IEqualityComparer<IComparer<T>>) Equality<IComparer<T>>.TypeEquality).ToArray<IComparer<T>>();
      this.len = this.comparers.Length;
    }

    public ChainedComparer(params IComparer<T>[] comparers)
      : this((IEnumerable<IComparer<T>>) comparers)
    {
    }

    public override int Compare(T x, T y)
    {
      int num = 0;
      for (int index = 0; index < this.len; ++index)
      {
        IComparer<T> comparer = this.comparers[index];
        if (comparer != null && (num = comparer.Compare(x, y)) != 0)
          break;
      }
      return num;
    }
  }
}

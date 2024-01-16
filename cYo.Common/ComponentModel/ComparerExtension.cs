// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.ComparerExtension
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public static class ComparerExtension
  {
    public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
    {
      return (IComparer<T>) new ReverseComparer<T>(comparer);
    }

    public static IComparer<T> Wrap<T>(this IComparer comparer)
    {
      return (IComparer<T>) new ComparerExtension.WrapComparer<T>(comparer);
    }

    public static IComparer<T> Chain<T>(this IEnumerable<IComparer<T>> comparerList)
    {
      return (IComparer<T>) new ChainedComparer<T>(comparerList);
    }

    public static IComparer<T> Chain<T>(this IComparer<T> comparer, params IComparer<T>[] comparers)
    {
      return ((IEnumerable<IComparer<T>>) comparers).AddFirst<IComparer<T>>(comparer).Chain<T>();
    }

    public static int Compare<T>(this IEnumerable<IComparer<T>> comparerList, T a, T b)
    {
      return comparerList.Select<IComparer<T>, int>((Func<IComparer<T>, int>) (c => c.Compare(a, b))).FirstOrDefault<int>((Func<int, bool>) (r => r != 0));
    }

    public static IComparer<T> Cast<T>(this IComparer comparer)
    {
      return (IComparer<T>) new ComparerExtension.CastComparer<T>(comparer);
    }

    public static int Compare<T>(T a, T b, params Comparison<T>[] comparers)
    {
      return ((IEnumerable<Comparison<T>>) comparers).Select<Comparison<T>, int>((Func<Comparison<T>, int>) (c => c(a, b))).FirstOrDefault<int>((Func<int, bool>) (n => n != 0));
    }

    private class WrapComparer<T> : IComparer<T>
    {
      private readonly IComparer comparer;

      public WrapComparer(IComparer comparer) => this.comparer = comparer;

      public int Compare(T x, T y) => this.comparer.Compare((object) x, (object) y);
    }

    private class CastComparer<T> : IComparer<T>
    {
      private readonly IComparer comparer;

      public CastComparer(IComparer comparer) => this.comparer = comparer;

      public int Compare(T x, T y) => this.comparer.Compare((object) x, (object) y);
    }
  }
}

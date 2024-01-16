// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.Equality`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Common.Collections
{
  public class Equality<T> : EqualityComparer<T>
  {
    private readonly Func<T, T, bool> comparer;
    private readonly Func<T, int> hashCode;

    public Equality(Func<T, T, bool> comparer, Func<T, int> hashCode)
    {
      this.comparer = comparer;
      this.hashCode = hashCode;
    }

    public override int GetHashCode(T x) => this.hashCode(x);

    public override bool Equals(T x, T y) => this.comparer(x, y);

    public static Equality<T> TypeEquality
    {
      get
      {
        return new Equality<T>((Func<T, T, bool>) ((a, b) => a.GetType() == b.GetType()), (Func<T, int>) (a => a.GetType().GetHashCode()));
      }
    }
  }
}

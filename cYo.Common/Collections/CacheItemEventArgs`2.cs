// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.CacheItemEventArgs`2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Collections
{
  public class CacheItemEventArgs<K, T> : EventArgs
  {
    private readonly K key;
    private readonly T item;

    public CacheItemEventArgs(K key, T item)
    {
      this.key = key;
      this.item = item;
    }

    public K Key => this.key;

    public T Item => this.item;
  }
}

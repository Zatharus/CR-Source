// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.SimpleCache`2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Common.Collections
{
  public class SimpleCache<K, T>
  {
    private Dictionary<K, T> dict = new Dictionary<K, T>();

    public T Get(K key, Func<K, T> create)
    {
      if (this.dict == null)
        this.dict = new Dictionary<K, T>();
      T obj;
      if (!this.dict.TryGetValue(key, out obj))
        obj = this.dict[key] = create(key);
      return obj;
    }
  }
}

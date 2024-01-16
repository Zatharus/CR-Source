// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.MruList`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Collections
{
  [Serializable]
  public class MruList<T> : SmartList<T>
  {
    public MruList(int maxCount) => this.MaxCount = maxCount;

    public MruList()
      : this(20)
    {
    }

    public int MaxCount { get; set; }

    public void UpdateMostRecent(T item)
    {
      do
        ;
      while (this.Remove(item));
      this.Insert(0, item);
      this.Trim(this.MaxCount);
    }
  }
}

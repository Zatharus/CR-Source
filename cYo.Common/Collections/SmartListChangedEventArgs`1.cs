// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.SmartListChangedEventArgs`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Collections
{
  public class SmartListChangedEventArgs<T> : EventArgs
  {
    private readonly T item;
    private readonly T oldItem;
    private readonly int index;
    private readonly SmartListAction action;

    public SmartListChangedEventArgs(SmartListAction action, int index, T item, T oldItem)
    {
      this.action = action;
      this.index = index;
      this.item = item;
      this.oldItem = oldItem;
    }

    public T Item => this.item;

    public T OldItem => this.oldItem;

    public int Index => this.index;

    public SmartListAction Action => this.action;
  }
}

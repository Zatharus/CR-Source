// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.GroupContainer`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class GroupContainer<T> : IGroupContainer<T>, IGroupInfo, IComparable<IGroupInfo>
  {
    private readonly List<T> items = new List<T>();

    public IGroupInfo Info { get; set; }

    public List<T> Items => this.items;

    public string Caption => this.Info.Caption;

    public object Key => this.Info.Key;

    public int Index => this.Info.Index;

    public int CompareTo(IGroupInfo other) => this.Info.CompareTo(other);
  }
}

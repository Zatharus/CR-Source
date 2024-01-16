// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.IGrouper`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Collections.Generic;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public interface IGrouper<T>
  {
    bool IsMultiGroup { get; }

    IGroupInfo GetGroup(T item);

    IEnumerable<IGroupInfo> GetGroups(T item);
  }
}

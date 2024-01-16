// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.SearchEngines
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Net.Search;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public static class SearchEngines
  {
    public static readonly IList<INetSearch> Engines = (IList<INetSearch>) new List<INetSearch>((IEnumerable<INetSearch>) new WikiSearch[1]
    {
      new WikiSearch()
    });
  }
}

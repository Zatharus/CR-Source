// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.CoverViewItemBookComparer`1
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Projects.ComicRack.Engine;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class CoverViewItemBookComparer<T> : CoverViewItemComparer where T : IComparer<ComicBook>, new()
  {
    private readonly T comparer = new T();

    protected override int OnCompare(CoverViewItem x, CoverViewItem y)
    {
      return this.comparer.Compare(x.Comic, y.Comic);
    }
  }
}

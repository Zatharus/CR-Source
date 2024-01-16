// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.CoverViewItemPositionComparer
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows.Forms;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class CoverViewItemPositionComparer : Comparer<CoverViewItem>, IComparer<IViewableItem>
  {
    public override int Compare(CoverViewItem x, CoverViewItem y)
    {
      return x.Position.CompareTo(y.Position);
    }

    int IComparer<IViewableItem>.Compare(IViewableItem x, IViewableItem y)
    {
      return this.Compare((CoverViewItem) x, (CoverViewItem) y);
    }
  }
}

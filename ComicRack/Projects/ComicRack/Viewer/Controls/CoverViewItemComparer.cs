// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.CoverViewItemComparer
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows.Forms;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public abstract class CoverViewItemComparer : Comparer<CoverViewItem>, IComparer<IViewableItem>
  {
    protected abstract int OnCompare(CoverViewItem x, CoverViewItem y);

    public override int Compare(CoverViewItem x, CoverViewItem y) => this.OnCompareInternal(x, y);

    public int Compare(IViewableItem x, IViewableItem y)
    {
      return this.OnCompareInternal(x as CoverViewItem, y as CoverViewItem);
    }

    private int OnCompareInternal(CoverViewItem x, CoverViewItem y)
    {
      if (x == null && y == null)
        return 0;
      if (x == null)
        return -1;
      return y == null ? 1 : this.OnCompare(x, y);
    }
  }
}

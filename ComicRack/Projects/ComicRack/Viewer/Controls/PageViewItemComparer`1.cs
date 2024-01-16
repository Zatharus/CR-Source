// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.PageViewItemComparer`1
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class PageViewItemComparer<T> : Comparer<PageViewItem>, IComparer<IViewableItem> where T : IComparer<ComicPageInfo>, new()
  {
    private readonly T comparer = new T();

    int IComparer<IViewableItem>.Compare(IViewableItem x, IViewableItem y)
    {
      return this.Compare((PageViewItem) x, (PageViewItem) y);
    }

    public override int Compare(PageViewItem x, PageViewItem y)
    {
      return this.comparer.Compare(x.Book.Comic.GetPage(x.Page), y.Book.Comic.GetPage(y.Page));
    }
  }
}

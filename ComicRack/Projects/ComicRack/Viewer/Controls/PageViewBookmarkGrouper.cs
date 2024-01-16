// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.PageViewBookmarkGrouper
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.ComponentModel;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class PageViewBookmarkGrouper : IGrouper<IViewableItem>
  {
    public bool IsMultiGroup => false;

    public IGroupInfo GetGroup(IViewableItem item)
    {
      PageViewItem pageViewItem = (PageViewItem) item;
      return (IGroupInfo) new GroupInfo(PageViewBookmarkGrouper.GetBookmark(pageViewItem), pageViewItem.Page);
    }

    public IEnumerable<IGroupInfo> GetGroups(IViewableItem item)
    {
      throw new NotImplementedException();
    }

    private static string GetBookmark(PageViewItem item)
    {
      int page = item.Page;
      ComicBook comic = item.Comic;
      while (page >= 0)
      {
        string bookmark = comic.GetPage(page--).Bookmark;
        if (!string.IsNullOrEmpty(bookmark))
          return bookmark;
      }
      return item.Comic.Caption;
    }
  }
}

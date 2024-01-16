// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.PageViewItemGrouper`1
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.ComponentModel;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class PageViewItemGrouper<T> : IGrouper<IViewableItem> where T : IGrouper<ComicPageInfo>, new()
  {
    private readonly T grouper = new T();

    public bool IsMultiGroup => this.grouper.IsMultiGroup;

    public IGroupInfo GetGroup(IViewableItem item)
    {
      return this.grouper.GetGroup(((PageViewItem) item).PageInfo);
    }

    public IEnumerable<IGroupInfo> GetGroups(IViewableItem item)
    {
      return this.grouper.GetGroups(((PageViewItem) item).PageInfo);
    }
  }
}

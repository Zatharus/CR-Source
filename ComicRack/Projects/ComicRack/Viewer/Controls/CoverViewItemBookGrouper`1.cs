// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.CoverViewItemBookGrouper`1
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
  public class CoverViewItemBookGrouper<T> : IGrouper<IViewableItem>, IBookGrouper where T : IGrouper<ComicBook>, new()
  {
    private readonly IGrouper<ComicBook> bookGrouper = (IGrouper<ComicBook>) new T();

    public IGrouper<ComicBook> BookGrouper => this.bookGrouper;

    public IGroupInfo GetGroup(IViewableItem item)
    {
      return this.bookGrouper.GetGroup(((CoverViewItem) item).Comic);
    }

    public bool IsMultiGroup => this.bookGrouper.IsMultiGroup;

    public IEnumerable<IGroupInfo> GetGroups(IViewableItem item)
    {
      return this.bookGrouper.GetGroups(((CoverViewItem) item).Comic);
    }
  }
}

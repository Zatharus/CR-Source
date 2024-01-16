// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.CoverViewItemStatsGrouper`1
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
  public class CoverViewItemStatsGrouper<T> : IGrouper<IViewableItem> where T : IGrouper<ComicBookSeriesStatistics>, new()
  {
    private readonly IGrouper<ComicBookSeriesStatistics> statsGrouper = (IGrouper<ComicBookSeriesStatistics>) new T();

    public IGroupInfo GetGroup(IViewableItem item)
    {
      return this.statsGrouper.GetGroup(((CoverViewItem) item).SeriesStats);
    }

    public bool IsMultiGroup => this.statsGrouper.IsMultiGroup;

    public IEnumerable<IGroupInfo> GetGroups(IViewableItem item)
    {
      return this.statsGrouper.GetGroups(((CoverViewItem) item).SeriesStats);
    }
  }
}

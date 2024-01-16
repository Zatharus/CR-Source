// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookSeriesComparer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Text;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookSeriesComparer : Comparer<ComicBook>
  {
    private static readonly IComparer<ComicBook>[] list = new IComparer<ComicBook>[3]
    {
      (IComparer<ComicBook>) new ComicBookFormatComparer(),
      (IComparer<ComicBook>) new ComicBookVolumeComparer(),
      (IComparer<ComicBook>) new ComicBookNumberComparer()
    };

    public override int Compare(ComicBook x, ComicBook y)
    {
      int num = ExtendedStringComparer.Compare(x.ShadowSeries, y.ShadowSeries, ExtendedStringComparison.IgnoreArticles | ExtendedStringComparison.IgnoreCase);
      return num != 0 ? num : ((IEnumerable<IComparer<ComicBook>>) ComicBookSeriesComparer.list).Compare<ComicBook>(x, y);
    }
  }
}

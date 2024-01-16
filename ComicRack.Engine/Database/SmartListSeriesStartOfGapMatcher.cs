// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.SmartListSeriesStartOfGapMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [System.ComponentModel.Description("Series: Start of Gap")]
  [ComicBookMatcherHint("Series", "Volume", "FilePath", "EnableProposed", "Number", DisableOptimizedUpdate = true)]
  [Serializable]
  public class SmartListSeriesStartOfGapMatcher : ComicBookYesNoMatcher
  {
    protected override YesNo GetValue(ComicBook comicBook)
    {
      if (this.StatsProvider == null)
        return YesNo.Unknown;
      return !this.StatsProvider.GetSeriesStats(comicBook).IsGapStart(comicBook) ? YesNo.No : YesNo.Yes;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.SmartListSeriesGapsMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [System.ComponentModel.Description("Series: Gaps")]
  [ComicBookMatcherHint("Series", "Volume", "FilePath", "EnableProposed", "Number", DisableOptimizedUpdate = true)]
  [Serializable]
  public class SmartListSeriesGapsMatcher : ComicBookNumericMatcher
  {
    protected override float GetValue(ComicBook comicBook)
    {
      return this.StatsProvider != null ? (float) this.StatsProvider.GetSeriesStats(comicBook).GapCount : 0.0f;
    }
  }
}

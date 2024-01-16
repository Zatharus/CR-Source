﻿// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookSeriesCompleteMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [System.ComponentModel.Description("Series complete")]
  [ComicBookMatcherHint("SeriesComplete")]
  [Serializable]
  public class ComicBookSeriesCompleteMatcher : ComicBookYesNoMatcher
  {
    protected override YesNo GetValue(ComicBook comicBook) => comicBook.SeriesComplete;
  }
}

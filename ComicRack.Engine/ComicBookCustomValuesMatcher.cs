// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookCustomValuesMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [System.ComponentModel.Description("Custom Value")]
  [ComicBookMatcherHint("CustomValuesStore")]
  [Serializable]
  public class ComicBookCustomValuesMatcher : ComicBookStringMatcher
  {
    private static readonly string textName = ComicBookMatcher.TRMatcher["Name", "Name"];

    public override int ArgumentCount => 2;

    protected override bool PreCheck(ComicBook comicBook) => true;

    public override int MatchColumn => 1;

    public override bool SwapOperatorArgument => true;

    public override string UnitDescription => ComicBookCustomValuesMatcher.textName;

    protected override string GetValue(ComicBook comicBook)
    {
      string matchValue2 = this.GetMatchValue2(comicBook);
      return matchValue2 == null ? (string) null : comicBook.GetCustomValue(matchValue2);
    }
  }
}

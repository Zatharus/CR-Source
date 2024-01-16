// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookMangaMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [System.ComponentModel.Description("Manga")]
  [ComicBookMatcherHint("Manga")]
  [Serializable]
  public class ComicBookMangaMatcher : ComicBookValueMatcher<MangaYesNo>
  {
    private static readonly string[] opListNeutral = "equals yes|equals ltr|equals no|equals unknown".Split('|');
    private static readonly string[] opList = ComicBookMatcher.TRMatcher.GetStrings("MangaYesNoOperators", "is Yes|is Yes (Left to Right)|is No|is Unknown", '|');

    protected override MangaYesNo GetValue(ComicBook comicBook) => comicBook.Manga;

    protected override bool MatchBook(ComicBook comicBook, MangaYesNo yesNo)
    {
      switch (this.MatchOperator)
      {
        case 1:
          return yesNo == MangaYesNo.YesAndRightToLeft;
        case 2:
          return yesNo == MangaYesNo.No;
        case 3:
          return yesNo == MangaYesNo.Unknown;
        default:
          return yesNo == MangaYesNo.Yes;
      }
    }

    public override string[] OperatorsListNeutral => ComicBookMangaMatcher.opListNeutral;

    public override string[] OperatorsList => ComicBookMangaMatcher.opList;

    public override int ArgumentCount => 0;
  }
}

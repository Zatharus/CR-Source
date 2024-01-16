// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookYesNoMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public abstract class ComicBookYesNoMatcher : ComicBookValueMatcher<YesNo>
  {
    private static readonly string[] opListNeutral = "equals yes|equals no|equals unknown".Split('|');
    private static readonly string[] opList = ComicBookMatcher.TRMatcher.GetStrings("YesNoOperators", "is Yes|is No|is Unknown", '|');

    protected override bool MatchBook(ComicBook comicBook, YesNo yesNo)
    {
      switch (this.MatchOperator)
      {
        case 1:
          return yesNo == YesNo.No;
        case 2:
          return yesNo == YesNo.Unknown;
        default:
          return yesNo == YesNo.Yes;
      }
    }

    public override string[] OperatorsListNeutral => ComicBookYesNoMatcher.opListNeutral;

    public override string[] OperatorsList => ComicBookYesNoMatcher.opList;

    public override int ArgumentCount => 0;
  }
}

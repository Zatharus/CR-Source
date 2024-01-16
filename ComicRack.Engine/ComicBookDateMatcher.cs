// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookDateMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using System;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public abstract class ComicBookDateMatcher : ComicBookValueMatcher<DateTime>
  {
    private static readonly string[] opListNeutral = "equals|is after|is before|is in last days|is in range".Split('|');
    private static readonly string[] opList = ComicBookMatcher.TRMatcher.GetStrings("DateOperators", "is|is after|is before|is in the last|is in the range", '|');
    private static readonly string daysText = ComicBookMatcher.TRMatcher["Days", "Days"];

    public ComicBookDateMatcher() => this.IgnoreTime = true;

    protected override bool MatchBook(ComicBook comicBook, DateTime date)
    {
      int num = date.CompareTo(this.GetMatchValue(comicBook), this.IgnoreTime);
      switch (this.MatchOperator)
      {
        case 0:
          return num == 0;
        case 1:
          return num == 1;
        case 2:
          return num == -1;
        case 3:
          return num >= 0;
        case 4:
          return num >= 0 && date.CompareTo(this.GetMatchValue2(comicBook), this.IgnoreTime) <= 0;
        default:
          return false;
      }
    }

    public override int ArgumentCount => this.MatchOperator != 4 ? 1 : 2;

    public override string[] OperatorsListNeutral => ComicBookDateMatcher.opListNeutral;

    public override string[] OperatorsList => ComicBookDateMatcher.opList;

    public override string UnitDescription
    {
      get => this.MatchOperator == 3 ? ComicBookDateMatcher.daysText : base.UnitDescription;
    }

    protected override DateTime ConvertMatchValue(string input)
    {
      if (string.IsNullOrEmpty(input))
        return DateTime.MinValue;
      DateTime result1;
      if (DateTime.TryParse(this.MatchValue, out result1))
        return result1;
      int result2;
      int.TryParse(this.MatchValue, out result2);
      return DateTime.Now - TimeSpan.FromDays((double) result2);
    }

    public override bool TimeDependant => this.MatchOperator == 3;

    [XmlIgnore]
    public bool IgnoreTime { get; set; }
  }
}

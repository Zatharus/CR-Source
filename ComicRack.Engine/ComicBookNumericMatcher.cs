// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookNumericMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public abstract class ComicBookNumericMatcher : ComicBookValueMatcher<float>
  {
    public const int Equal = 0;
    public const int Greater = 1;
    public const int Lesser = 2;
    public const int InRange = 3;
    private static readonly string[] opListNeutral = "equals|is greater|is smaller|in range".Split('|');
    private static readonly string[] opList = ComicBookMatcher.TRMatcher.GetStrings("NumericOperators", "is|is greater|is smaller|is in the range", '|');

    protected override bool MatchBook(ComicBook comicBook, float n)
    {
      switch (this.MatchOperator)
      {
        case 0:
          return (double) this.GetMatchValue(comicBook) == (double) n;
        case 1:
          return (double) n > (double) this.GetMatchValue(comicBook);
        case 2:
          return (double) n < (double) this.GetMatchValue(comicBook);
        case 3:
          return (double) n >= (double) this.GetMatchValue(comicBook) && (double) n <= (double) this.GetMatchValue2(comicBook);
        default:
          return false;
      }
    }

    public override int ArgumentCount => this.MatchOperator != 3 ? 1 : 2;

    public override string[] OperatorsListNeutral => ComicBookNumericMatcher.opListNeutral;

    public override string[] OperatorsList => ComicBookNumericMatcher.opList;

    protected override float ConvertMatchValue(string value)
    {
      float result;
      return !float.TryParse(value, out result) ? this.GetInvalidValue() : result;
    }

    protected override float GetInvalidValue() => -1f;
  }
}

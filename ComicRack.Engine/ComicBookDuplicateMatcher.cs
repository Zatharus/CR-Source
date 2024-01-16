// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookDuplicateMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [System.ComponentModel.Description("Only Duplicates")]
  [ComicBookMatcherHint("FilePath, EnableProposed, Series, Format, Count, Number, Volume, LanguageISO, Year, Month, Day", DisableOptimizedUpdate = true)]
  [Serializable]
  public class ComicBookDuplicateMatcher : ComicBookValueMatcher
  {
    private static readonly ComicBookDublicateComparer duplicateComparer = new ComicBookDublicateComparer();
    private static readonly string[] opListNeutral = "on|off".Split('|');
    private static readonly string[] opList = ComicBookMatcher.TRMatcher.GetStrings("OnOffOperators", "On|Off", '|');

    public override IEnumerable<ComicBook> Match(IEnumerable<ComicBook> items)
    {
      if (this.MatchOperator != 0)
        return items;
      List<ComicBook> list = items.ToList<ComicBook>();
      List<ComicBook> comicBookList = new List<ComicBook>();
      list.Sort((IComparer<ComicBook>) ComicBookDuplicateMatcher.duplicateComparer);
      ComicBook x = (ComicBook) null;
      ComicBook comicBook = (ComicBook) null;
      foreach (ComicBook y in list)
      {
        if (x == null || ComicBookDuplicateMatcher.duplicateComparer.Compare(x, y) != 0)
        {
          x = comicBook = y;
        }
        else
        {
          if (comicBook != null)
            comicBookList.Add(comicBook);
          comicBook = (ComicBook) null;
          comicBookList.Add(y);
        }
      }
      return (IEnumerable<ComicBook>) comicBookList;
    }

    public override string[] OperatorsListNeutral => ComicBookDuplicateMatcher.opListNeutral;

    public override string[] OperatorsList => ComicBookDuplicateMatcher.opList;

    public override int ArgumentCount => 0;
  }
}

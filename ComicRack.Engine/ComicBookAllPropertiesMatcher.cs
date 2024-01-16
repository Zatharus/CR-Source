// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookAllPropertiesMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [System.ComponentModel.Description("All")]
  [Serializable]
  public class ComicBookAllPropertiesMatcher : ComicBookStringMatcher
  {
    private ComicBookAllPropertiesMatcher.MatcherOption option;

    public ComicBookAllPropertiesMatcher.MatcherOption Option
    {
      get => this.option;
      set => this.option = value;
    }

    private bool MatchOption(
      ComicBook comicBook,
      ComicBookAllPropertiesMatcher.MatcherOption option)
    {
      return ComicBookAllPropertiesMatcher.GetOptionValueSet(comicBook, option).Any<string>((Func<string, bool>) (t => this.MatchBook(comicBook, t)));
    }

    public override bool Match(ComicBook comicBook) => this.MatchOption(comicBook, this.option);

    protected override string GetValue(ComicBook comicBook)
    {
      throw new InvalidOperationException("This is not valid for this comparer");
    }

    public static ComicBookMatcher Create(
      string query,
      int matchOperator,
      ComicBookAllPropertiesMatcher.MatcherOption searchOption,
      ComicBookAllPropertiesMatcher.ShowOptionType showOption,
      ComicBookAllPropertiesMatcher.ShowComicType showComic,
      params ComicBookMatcher[] additonalMatchers)
    {
      List<ComicBookMatcher> list = new List<ComicBookMatcher>();
      switch (showOption)
      {
        case ComicBookAllPropertiesMatcher.ShowOptionType.Read:
          List<ComicBookMatcher> comicBookMatcherList1 = list;
          ComicBookReadPercentageMatcher percentageMatcher1 = new ComicBookReadPercentageMatcher();
          percentageMatcher1.MatchOperator = 1;
          percentageMatcher1.MatchValue = EngineConfiguration.Default.IsReadCompletionPercentage.ToString();
          comicBookMatcherList1.Add((ComicBookMatcher) percentageMatcher1);
          break;
        case ComicBookAllPropertiesMatcher.ShowOptionType.Reading:
          List<ComicBookMatcher> comicBookMatcherList2 = list;
          ComicBookReadPercentageMatcher percentageMatcher2 = new ComicBookReadPercentageMatcher();
          percentageMatcher2.MatchOperator = 3;
          percentageMatcher2.MatchValue = (EngineConfiguration.Default.IsNotReadCompletionPercentage + 1).ToString();
          percentageMatcher2.MatchValue2 = (EngineConfiguration.Default.IsReadCompletionPercentage - 1).ToString();
          comicBookMatcherList2.Add((ComicBookMatcher) percentageMatcher2);
          break;
        case ComicBookAllPropertiesMatcher.ShowOptionType.Unread:
          List<ComicBookMatcher> comicBookMatcherList3 = list;
          ComicBookReadPercentageMatcher percentageMatcher3 = new ComicBookReadPercentageMatcher();
          percentageMatcher3.MatchOperator = 2;
          percentageMatcher3.MatchValue = EngineConfiguration.Default.IsNotReadCompletionPercentage.ToString();
          comicBookMatcherList3.Add((ComicBookMatcher) percentageMatcher3);
          break;
      }
      switch (showComic)
      {
        case ComicBookAllPropertiesMatcher.ShowComicType.Comics:
          List<ComicBookMatcher> comicBookMatcherList4 = list;
          ComicBookFileMatcher comicBookFileMatcher1 = new ComicBookFileMatcher();
          comicBookFileMatcher1.MatchOperator = 0;
          comicBookFileMatcher1.MatchValue = string.Empty;
          comicBookFileMatcher1.Not = true;
          comicBookMatcherList4.Add((ComicBookMatcher) comicBookFileMatcher1);
          break;
        case ComicBookAllPropertiesMatcher.ShowComicType.FilelessComics:
          List<ComicBookMatcher> comicBookMatcherList5 = list;
          ComicBookFileMatcher comicBookFileMatcher2 = new ComicBookFileMatcher();
          comicBookFileMatcher2.MatchOperator = 0;
          comicBookFileMatcher2.MatchValue = string.Empty;
          comicBookMatcherList5.Add((ComicBookMatcher) comicBookFileMatcher2);
          break;
      }
      if (!string.IsNullOrEmpty(query))
      {
        List<ComicBookMatcher> comicBookMatcherList6 = list;
        ComicBookAllPropertiesMatcher propertiesMatcher = new ComicBookAllPropertiesMatcher();
        propertiesMatcher.MatchValue = query;
        propertiesMatcher.Option = searchOption;
        propertiesMatcher.MatchOperator = matchOperator;
        comicBookMatcherList6.Add((ComicBookMatcher) propertiesMatcher);
      }
      list.AddRange(((IEnumerable<ComicBookMatcher>) additonalMatchers).Where<ComicBookMatcher>((Func<ComicBookMatcher, bool>) (m => m != null)));
      if (list.Count == 0)
        return (ComicBookMatcher) null;
      if (list.Count == 1)
        return list[0];
      ComicBookGroupMatcher bookGroupMatcher = new ComicBookGroupMatcher();
      bookGroupMatcher.Matchers.AddRange((IEnumerable<ComicBookMatcher>) list);
      bookGroupMatcher.MatcherMode = MatcherMode.And;
      return (ComicBookMatcher) bookGroupMatcher;
    }

    public static IEnumerable<string> GetOptionValueSet(
      ComicBook comicBook,
      ComicBookAllPropertiesMatcher.MatcherOption option)
    {
      switch (option)
      {
        case ComicBookAllPropertiesMatcher.MatcherOption.Series:
          yield return comicBook.ShadowSeries;
          yield return comicBook.AlternateSeries;
          yield return comicBook.ShadowFormat;
          yield return comicBook.SeriesGroup;
          yield return comicBook.StoryArc;
          break;
        case ComicBookAllPropertiesMatcher.MatcherOption.Writer:
          yield return comicBook.Writer;
          break;
        case ComicBookAllPropertiesMatcher.MatcherOption.Artists:
          yield return comicBook.Writer;
          yield return comicBook.Penciller;
          yield return comicBook.Inker;
          yield return comicBook.Colorist;
          yield return comicBook.Editor;
          yield return comicBook.Letterer;
          yield return comicBook.CoverArtist;
          break;
        case ComicBookAllPropertiesMatcher.MatcherOption.Descriptive:
          yield return comicBook.Notes;
          yield return comicBook.Summary;
          yield return comicBook.Review;
          yield return comicBook.Tags;
          yield return comicBook.Characters;
          yield return comicBook.Teams;
          yield return comicBook.MainCharacterOrTeam;
          yield return comicBook.Locations;
          yield return comicBook.ScanInformation;
          break;
        case ComicBookAllPropertiesMatcher.MatcherOption.File:
          yield return comicBook.FilePath;
          break;
        case ComicBookAllPropertiesMatcher.MatcherOption.Catalog:
          yield return comicBook.BookAge;
          yield return comicBook.BookCollectionStatus;
          yield return comicBook.BookNotes;
          yield return comicBook.BookOwner;
          yield return comicBook.BookStore;
          yield return comicBook.BookLocation;
          yield return comicBook.ISBN;
          break;
        default:
          yield return comicBook.ShadowSeries;
          yield return comicBook.AlternateSeries;
          yield return comicBook.ShadowTitle;
          yield return comicBook.SeriesGroup;
          yield return comicBook.StoryArc;
          yield return comicBook.Writer;
          yield return comicBook.Penciller;
          yield return comicBook.Inker;
          yield return comicBook.Colorist;
          yield return comicBook.Letterer;
          yield return comicBook.Editor;
          yield return comicBook.CoverArtist;
          yield return comicBook.Summary;
          yield return comicBook.FilePath;
          yield return comicBook.Genre;
          yield return comicBook.Notes;
          yield return comicBook.Review;
          yield return comicBook.Publisher;
          yield return comicBook.Imprint;
          yield return comicBook.ShadowVolumeAsText;
          yield return comicBook.ShadowNumberAsText;
          yield return comicBook.AlternateNumberAsText;
          yield return comicBook.ShadowYearAsText;
          yield return comicBook.ShadowFormat;
          yield return comicBook.AgeRating;
          yield return comicBook.Tags;
          yield return comicBook.Characters;
          yield return comicBook.Teams;
          yield return comicBook.MainCharacterOrTeam;
          yield return comicBook.Locations;
          yield return comicBook.BookAge;
          yield return comicBook.BookCollectionStatus;
          yield return comicBook.BookNotes;
          yield return comicBook.BookOwner;
          yield return comicBook.BookStore;
          yield return comicBook.BookLocation;
          yield return comicBook.ISBN;
          break;
      }
    }

    public enum MatcherOption
    {
      All,
      Series,
      Writer,
      Artists,
      Descriptive,
      File,
      Catalog,
    }

    public enum ShowOptionType
    {
      All,
      Read,
      Reading,
      Unread,
    }

    public enum ShowComicType
    {
      All,
      Comics,
      FilelessComics,
    }
  }
}

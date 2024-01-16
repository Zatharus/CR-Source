// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicSmartListItem
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class ComicSmartListItem : 
    ShareableComicListItem,
    IMatcher<ComicBook>,
    IComicBookGroupMatcher,
    IFilteredComicBookList
  {
    private ComicBookMatcherCollection matchers = new ComicBookMatcherCollection();
    private HashSet<Guid> filteredIds;
    private static readonly Regex rxTokenizer = new Regex("(?<!\\\\)\".*?((?<!\\\\)\"|$)|(?<!\\\\)\\[.*?((?<!\\\\)\\]|$)|(?<=\\]\\s+)\\s*Match|(?<=\\]\\s+)[\\w\\s]+|[\\w]+|{|}|,|;", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    public ComicSmartListItem()
    {
      this.BaseListId = Guid.Empty;
      this.LimitSelectionType = ComicSmartListLimitSelectionType.Random;
      this.LimitValue = 25;
      this.MatcherMode = MatcherMode.And;
      this.LimitType = ComicSmartListLimitType.Count;
    }

    public ComicSmartListItem(string name, string matchAll = null)
      : this()
    {
      this.Name = name;
      if (matchAll == null)
        return;
      this.Matchers.Add(typeof (ComicBookAllPropertiesMatcher), 1, matchAll, string.Empty);
    }

    public ComicSmartListItem(
      string name,
      MatcherMode mode,
      IEnumerable<ComicBookMatcher> matchers)
      : this(name)
    {
      this.MatcherMode = mode;
      this.Matchers.AddRange(matchers.Select<ComicBookMatcher, ComicBookMatcher>((Func<ComicBookMatcher, ComicBookMatcher>) (cbm => cbm.Clone() as ComicBookMatcher)));
    }

    public ComicSmartListItem(ComicSmartListItem comicSmartListItem)
      : this(comicSmartListItem.Name, comicSmartListItem.MatcherMode, (IEnumerable<ComicBookMatcher>) comicSmartListItem.Matchers)
    {
      this.Display = comicSmartListItem.Display;
      this.Limit = comicSmartListItem.Limit;
      this.LimitType = comicSmartListItem.LimitType;
      this.LimitValue = comicSmartListItem.LimitValue;
      this.LimitSelectionType = comicSmartListItem.LimitSelectionType;
      this.BaseListId = comicSmartListItem.BaseListId;
      this.NotInBaseList = comicSmartListItem.NotInBaseList;
      this.QuickOpen = comicSmartListItem.QuickOpen;
      this.Description = comicSmartListItem.Description;
      this.BookCount = comicSmartListItem.BookCount;
      this.NewBookCount = comicSmartListItem.NewBookCount;
      this.UnreadBookCount = comicSmartListItem.UnreadBookCount;
      if (!comicSmartListItem.ShouldSerializeFilteredIds())
        return;
      this.FilteredIds.AddRange<Guid>((IEnumerable<Guid>) comicSmartListItem.FilteredIds);
    }

    public ComicSmartListItem(string name, string query, ComicLibrary library = null)
      : this(name)
    {
      Tokenizer tokens = ComicSmartListItem.TokenizeQuery(query);
      if (tokens.IsOptional("NAME"))
      {
        tokens.Skip();
        this.Name = tokens.TakeString().Text;
      }
      if (tokens.IsOptional("IN", "NOT"))
      {
        if (tokens.IsOptional("NOT"))
        {
          tokens.Skip();
          this.NotInBaseList = true;
        }
        tokens.Skip();
        Tokenizer.Token list = tokens.Take("[", "]");
        list.Text = list.Text.Unescape((IEnumerable<char>) "[]", '\\');
        if (library != null)
        {
          ComicListItem comicListItem = library.ComicLists.GetItems<ComicListItem>().FirstOrDefault<ComicListItem>((Func<ComicListItem, bool>) (cl => string.Equals(cl.Name, list.Text, StringComparison.CurrentCultureIgnoreCase)));
          if (comicListItem != null)
            this.BaseListId = comicListItem.Id;
        }
      }
      ComicBookGroupMatcher.ConvertQueryToParamerters((IComicBookGroupMatcher) this, tokens);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(this.Name))
      {
        stringBuilder.Append("Name ");
        stringBuilder.Append("\"");
        stringBuilder.Append(this.Name.Escape());
        stringBuilder.Append("\"\n");
      }
      if (this.BaseListIdSpecified)
      {
        ComicListItem baseList = this.GetBaseList();
        if (baseList != null && !(baseList is ComicLibraryListItem))
        {
          if (this.NotInBaseList)
            stringBuilder.Append("Not ");
          stringBuilder.Append("In [");
          stringBuilder.Append(baseList.Name.Escape((IEnumerable<char>) "[]", '\\'));
          stringBuilder.Append("]\n");
        }
      }
      stringBuilder.Append(ComicBookGroupMatcher.ConvertParametersToQuery((IComicBookGroupMatcher) this));
      return stringBuilder.ToString();
    }

    protected override IEnumerable<ComicBook> OnGetBooks()
    {
      IEnumerable<ComicBook> comicBooks = (IEnumerable<ComicBook>) null;
      if (this.Library != null && this.Library.Books != null)
      {
        if (this.BaseListId == Guid.Empty)
        {
          comicBooks = (IEnumerable<ComicBook>) this.Library.Books;
        }
        else
        {
          IComicBookList baseList = (IComicBookList) this.GetBaseList();
          if (baseList != null)
            comicBooks = baseList.GetBooks();
        }
        if (comicBooks != null && this.NotInBaseList)
          comicBooks = this.Library.Books.Except<ComicBook>(comicBooks);
      }
      return comicBooks != null ? this.Match(comicBooks) : Enumerable.Empty<ComicBook>();
    }

    public override string ImageKey => "Search";

    public bool IsSame(ComicSmartListItem comicSmartListItem)
    {
      return comicSmartListItem != null && this.Name == comicSmartListItem.Name && this.Display == comicSmartListItem.Display && this.MatcherMode == comicSmartListItem.MatcherMode && this.NotInBaseList == comicSmartListItem.NotInBaseList && this.Limit == comicSmartListItem.Limit && this.LimitType == comicSmartListItem.LimitType && this.LimitValue == comicSmartListItem.LimitValue && this.LimitSelectionType == comicSmartListItem.LimitSelectionType && this.LimitRandomSeed == comicSmartListItem.LimitRandomSeed && this.BaseListId == comicSmartListItem.BaseListId && this.Matchers.SequenceEqual<ComicBookMatcher>((IEnumerable<ComicBookMatcher>) comicSmartListItem.Matchers) && this.QuickOpen == comicSmartListItem.QuickOpen && this.Description == comicSmartListItem.Description && this.HasSameFilteredIds(comicSmartListItem);
    }

    public override object Clone() => (object) new ComicSmartListItem(this);

    public override IEnumerable<string> GetDependentProperties()
    {
      return base.GetDependentProperties().Concat<string>(this.Matchers.SelectMany<ComicBookMatcher, string>((Func<ComicBookMatcher, IEnumerable<string>>) (m => m.GetDependentProperties())));
    }

    public override bool IsUpdateNeeded(string propertyHint)
    {
      return !this.Matchers.All<ComicBookMatcher>((Func<ComicBookMatcher, bool>) (m => !m.UsesProperty(propertyHint))) && base.IsUpdateNeeded(propertyHint);
    }

    public override bool OptimizedCacheUpdateDisabled
    {
      get
      {
        return this.Matchers.Any<ComicBookMatcher>((Func<ComicBookMatcher, bool>) (m => m.IsOptimizedCacheUpdateDisabled));
      }
    }

    protected override IEnumerable<ComicBook> OnCacheMatch(IEnumerable<ComicBook> cbl)
    {
      ICachedComicBookList m = (ICachedComicBookList) this.GetBaseList();
      return this.Match(cbl).Where<ComicBook>((Func<ComicBook, bool>) (cb => m == null || m.GetCache().Contains(cb) ^ this.NotInBaseList));
    }

    public override bool CustomCacheStorage
    {
      get
      {
        return this.Matchers.Any<ComicBookMatcher>((Func<ComicBookMatcher, bool>) (m => m.TimeDependant));
      }
    }

    protected override bool OnRetrieveCustomCache(HashSet<ComicBook> books)
    {
      this.Library.NotifyComicListCacheReset((ComicListItem) this);
      return base.OnRetrieveCustomCache(books);
    }

    public override void Refresh()
    {
      this.LimitRandomSeed = 0;
      base.Refresh();
    }

    public IEnumerable<ComicBook> Match(IEnumerable<ComicBook> items)
    {
      MatcherSet<ComicBook> matcherSet = new MatcherSet<ComicBook>();
      foreach (ComicBookMatcher matcher in (SmartList<ComicBookMatcher>) this.Matchers)
        matcherSet.Add((IMatcher<ComicBook>) matcher, this.MatcherMode, matcher.Not);
      this.Matchers.Recurse<ComicBookMatcher>((Func<object, IEnumerable>) (m => !(m is ComicBookGroupMatcher) ? (IEnumerable) null : (IEnumerable) ((ComicBookGroupMatcher) m).Matchers)).ForEach<ComicBookMatcher>((Action<ComicBookMatcher>) (m => m.StatsProvider = (IComicBookStatsProvider) this));
      IEnumerable<ComicBook> comicBooks = matcherSet.Match(items);
      if (this.Limit)
      {
        switch (this.LimitSelectionType)
        {
          case ComicSmartListLimitSelectionType.Position:
            switch (this.LimitType)
            {
              case ComicSmartListLimitType.MB:
                comicBooks = ComicSmartListItem.LimitBySize(comicBooks, (long) this.LimitValue * 1024L * 1024L);
                break;
              case ComicSmartListLimitType.GB:
                comicBooks = ComicSmartListItem.LimitBySize(comicBooks, (long) this.LimitValue * 1024L * 1024L * 1024L);
                break;
              default:
                comicBooks = comicBooks.Take<ComicBook>(this.LimitValue);
                break;
            }
            break;
          case ComicSmartListLimitSelectionType.SortedBySeries:
            comicBooks = (IEnumerable<ComicBook>) comicBooks.ToList<ComicBook>().OrderBy<ComicBook, ComicBook>((Func<ComicBook, ComicBook>) (cb => cb), (IComparer<ComicBook>) new ComicBookSeriesComparer());
            goto case ComicSmartListLimitSelectionType.Position;
          default:
            if (this.LimitRandomSeed == 0)
              this.LimitRandomSeed = new Random().Next();
            comicBooks = (IEnumerable<ComicBook>) comicBooks.OrderBy<ComicBook, Guid>((Func<ComicBook, Guid>) (l => l.Id)).ToList<ComicBook>().Randomize<ComicBook>(this.LimitRandomSeed);
            goto case ComicSmartListLimitSelectionType.Position;
        }
      }
      if (!this.ShowFiltered && this.ShouldSerializeFilteredIds())
      {
        if (this.Library != null)
        {
          ComicBookCollection books = this.Library.Books;
          this.filteredIds.RemoveWhere((Predicate<Guid>) (id => books[id] == null));
        }
        comicBooks = comicBooks.Where<ComicBook>((Func<ComicBook, bool>) (cb => !this.filteredIds.Contains(cb.Id)));
      }
      return comicBooks;
    }

    [XmlAttribute]
    [DefaultValue(MatcherMode.And)]
    public MatcherMode MatcherMode { get; set; }

    public ComicBookMatcherCollection Matchers => this.matchers;

    [DefaultValue(false)]
    public bool Limit { get; set; }

    [DefaultValue(ComicSmartListLimitType.Count)]
    public ComicSmartListLimitType LimitType { get; set; }

    [DefaultValue(25)]
    public int LimitValue { get; set; }

    [DefaultValue(ComicSmartListLimitSelectionType.Random)]
    public ComicSmartListLimitSelectionType LimitSelectionType { get; set; }

    [DefaultValue(0)]
    public int LimitRandomSeed { get; set; }

    public Guid BaseListId { get; set; }

    public bool BaseListIdSpecified => this.BaseListId != Guid.Empty;

    [DefaultValue(false)]
    public bool NotInBaseList { get; set; }

    public HashSet<Guid> FilteredIds
    {
      get => this.filteredIds ?? (this.filteredIds = new HashSet<Guid>());
    }

    public bool ShouldSerializeFilteredIds()
    {
      return this.filteredIds != null && this.filteredIds.Count > 0;
    }

    public static Tokenizer TokenizeQuery(string query)
    {
      return new Tokenizer(ComicSmartListItem.rxTokenizer, query);
    }

    public ComicListItem GetBaseList(bool withTest = true)
    {
      if (this.Library == null || this.Library.ComicLists == null)
        return (ComicListItem) null;
      ComicListItem baseList = this.Library.ComicLists.FindItem(this.BaseListId);
      if (baseList != null & withTest && baseList.RecursionTest((ComicListItem) this))
        baseList = (ComicListItem) null;
      return baseList;
    }

    public bool SetList(ComicSmartListItem item)
    {
      if (this.IsSame(item))
        return false;
      this.MatcherMode = item.MatcherMode;
      this.NotInBaseList = item.NotInBaseList;
      ComicSmartListItem comicSmartListItem = item.Clone() as ComicSmartListItem;
      this.BaseListId = comicSmartListItem.BaseListId;
      this.matchers = comicSmartListItem.matchers;
      this.CopyExtraValues(item);
      this.ResetCache();
      this.Refresh();
      return true;
    }

    public void CopyExtraValues(ComicSmartListItem item)
    {
      this.Name = item.Name;
      this.Description = item.Description;
      this.Limit = item.Limit;
      this.LimitValue = item.LimitValue;
      this.LimitSelectionType = item.LimitSelectionType;
      this.LimitRandomSeed = item.LimitRandomSeed;
      this.LimitType = item.LimitType;
      this.QuickOpen = item.QuickOpen;
      this.filteredIds = (HashSet<Guid>) null;
      if (!item.ShouldSerializeFilteredIds())
        return;
      this.FilteredIds.AddRange<Guid>((IEnumerable<Guid>) item.FilteredIds);
    }

    public bool HasSameFilteredIds(ComicSmartListItem cli)
    {
      if (this.filteredIds == cli.filteredIds)
        return true;
      return this.filteredIds != null && cli.FilteredIds != null && this.filteredIds.SetEquals((IEnumerable<Guid>) cli.filteredIds);
    }

    private static IEnumerable<ComicBook> LimitBySize(IEnumerable<ComicBook> cbl, long maxSize)
    {
      long size = 0;
      return cbl.TakeWhile<ComicBook>((Func<ComicBook, bool>) (cb => (size += cb.FileSize) < maxSize));
    }

    public bool IsFiltered(ComicBook ci)
    {
      return this.filteredIds != null && this.filteredIds.Contains(ci.Id);
    }

    public void SetFiltered(ComicBook ci, bool filtered)
    {
      if (this.IsFiltered(ci) == filtered)
        return;
      if (filtered)
        this.FilteredIds.Add(ci.Id);
      else
        this.FilteredIds.Remove(ci.Id);
      this.Refresh();
    }

    public void ClearFiltered()
    {
      if (!this.ShouldSerializeFilteredIds())
        return;
      this.filteredIds = (HashSet<Guid>) null;
      this.Refresh();
    }

    [DefaultValue(false)]
    public bool ShowFiltered { get; set; }
  }
}

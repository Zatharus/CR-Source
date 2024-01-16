// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicInfo
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Reflection;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public class ComicInfo
  {
    private static volatile string[] coverKeyFilter = new string[0];
    private string title = string.Empty;
    private string series = string.Empty;
    private string number = string.Empty;
    private int count = -1;
    private int volume = -1;
    private string alternateSeries = string.Empty;
    private string alternateNumber = string.Empty;
    private string storyArc = string.Empty;
    private string seriesGroup = string.Empty;
    private int alternateCount = -1;
    private string summary = string.Empty;
    private string notes = string.Empty;
    private string review = string.Empty;
    private int year = -1;
    private int month = -1;
    private int day = -1;
    private string writer = string.Empty;
    private string penciller = string.Empty;
    private string inker = string.Empty;
    private string colorist = string.Empty;
    private string letterer = string.Empty;
    private string coverArtist = string.Empty;
    private string editor = string.Empty;
    private string publisher = string.Empty;
    private string imprint = string.Empty;
    private string genre = string.Empty;
    private string web = string.Empty;
    private int pageCount;
    private string languageISO = string.Empty;
    private string format = string.Empty;
    private string ageRating = string.Empty;
    private YesNo blackAndWhite = YesNo.Unknown;
    private MangaYesNo manga = MangaYesNo.Unknown;
    private int preferredFrontCover;
    private string characters = string.Empty;
    private string teams = string.Empty;
    private string mainCharacterOrTeam = string.Empty;
    private string locations = string.Empty;
    private float communityRating;
    private string scanInformation = string.Empty;
    private volatile int cachedFrontCoverPageIndex = -1;
    private volatile int cachedFrontCoverCount = -1;
    private ComicPageInfoCollection pages;
    private int cachedBookmarkCount = -1;
    private static readonly Lazy<string> yesText = new Lazy<string>((Func<string>) (() => TR.Default["Yes"]));
    private static readonly Lazy<string> noText = new Lazy<string>((Func<string>) (() => TR.Default["No"]));
    private static readonly Lazy<string> yesRightToLeftText = new Lazy<string>((Func<string>) (() => TR.Load(nameof (ComicInfo))["YesRightToLeft", "Yes (Right to Left)"]));
    private static readonly Regex rxVolume = new Regex("\\bv(ol(ume)?)?\\.?\\s?\\d+\\b\\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex rxSpecial = new Regex("[^a-z0-9]|\\bthe\\b|\\band\\b|", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public ComicInfo()
    {
    }

    public ComicInfo(ComicInfo ci)
      : this()
    {
      this.SetInfo(ci, false);
    }

    public static string CoverKeyFilter
    {
      get => ComicInfo.coverKeyFilter.ToListString(";");
      set
      {
        string[] strArray;
        if (value != null)
          strArray = ((IEnumerable<string>) value.Split(';')).Select<string, string>((Func<string, string>) (s => s.Trim())).RemoveEmpty().ToArray<string>();
        else
          strArray = new string[0];
        ComicInfo.coverKeyFilter = strArray;
      }
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Title
    {
      get => this.title;
      set => this.SetProperty<string>(nameof (Title), ref this.title, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Series
    {
      get => this.series;
      set => this.SetProperty<string>(nameof (Series), ref this.series, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Number
    {
      get => this.number;
      set => this.SetProperty<string>(nameof (Number), ref this.number, value);
    }

    [Browsable(true)]
    [DefaultValue(-1)]
    [ResetValue(0)]
    public int Count
    {
      get => this.count;
      set => this.SetProperty<int>(nameof (Count), ref this.count, value);
    }

    [Browsable(true)]
    [DefaultValue(-1)]
    [ResetValue(0)]
    public int Volume
    {
      get => this.volume;
      set => this.SetProperty<int>(nameof (Volume), ref this.volume, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string AlternateSeries
    {
      get => this.alternateSeries;
      set => this.SetProperty<string>(nameof (AlternateSeries), ref this.alternateSeries, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string AlternateNumber
    {
      get => this.alternateNumber;
      set => this.SetProperty<string>(nameof (AlternateNumber), ref this.alternateNumber, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string StoryArc
    {
      get => this.storyArc;
      set => this.SetProperty<string>(nameof (StoryArc), ref this.storyArc, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string SeriesGroup
    {
      get => this.seriesGroup;
      set => this.SetProperty<string>(nameof (SeriesGroup), ref this.seriesGroup, value);
    }

    [Browsable(true)]
    [DefaultValue(-1)]
    [ResetValue(0)]
    public int AlternateCount
    {
      get => this.alternateCount;
      set => this.SetProperty<int>(nameof (AlternateCount), ref this.alternateCount, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Summary
    {
      get => this.summary;
      set => this.SetProperty<string>(nameof (Summary), ref this.summary, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Notes
    {
      get => this.notes;
      set => this.SetProperty<string>(nameof (Notes), ref this.notes, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Review
    {
      get => this.review;
      set => this.SetProperty<string>(nameof (Review), ref this.review, value);
    }

    [Browsable(true)]
    [DefaultValue(-1)]
    [ResetValue(0)]
    public int Year
    {
      get => this.year;
      set => this.SetProperty<int>(nameof (Year), ref this.year, value);
    }

    [Browsable(true)]
    [DefaultValue(-1)]
    [ResetValue(0)]
    public int Month
    {
      get => this.month;
      set => this.SetProperty<int>(nameof (Month), ref this.month, value);
    }

    [Browsable(true)]
    [DefaultValue(-1)]
    [ResetValue(0)]
    public int Day
    {
      get => this.day;
      set => this.SetProperty<int>(nameof (Day), ref this.day, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Writer
    {
      get => this.writer;
      set => this.SetProperty<string>(nameof (Writer), ref this.writer, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Penciller
    {
      get => this.penciller;
      set => this.SetProperty<string>(nameof (Penciller), ref this.penciller, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Inker
    {
      get => this.inker;
      set => this.SetProperty<string>(nameof (Inker), ref this.inker, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Colorist
    {
      get => this.colorist;
      set => this.SetProperty<string>(nameof (Colorist), ref this.colorist, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Letterer
    {
      get => this.letterer;
      set => this.SetProperty<string>(nameof (Letterer), ref this.letterer, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string CoverArtist
    {
      get => this.coverArtist;
      set => this.SetProperty<string>(nameof (CoverArtist), ref this.coverArtist, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Editor
    {
      get => this.editor;
      set => this.SetProperty<string>(nameof (Editor), ref this.editor, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Publisher
    {
      get => this.publisher;
      set => this.SetProperty<string>(nameof (Publisher), ref this.publisher, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Imprint
    {
      get => this.imprint;
      set => this.SetProperty<string>(nameof (Imprint), ref this.imprint, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Genre
    {
      get => this.genre;
      set => this.SetProperty<string>(nameof (Genre), ref this.genre, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Web
    {
      get => this.web;
      set => this.SetProperty<string>(nameof (Web), ref this.web, value);
    }

    [Browsable(true)]
    [DefaultValue(0)]
    [ResetValue(0)]
    public int PageCount
    {
      get => this.pageCount;
      set => this.SetProperty<int>(nameof (PageCount), ref this.pageCount, value);
    }

    [DefaultValue("")]
    [ResetValue(0)]
    public string LanguageISO
    {
      get => this.languageISO;
      set => this.SetProperty<string>(nameof (LanguageISO), ref this.languageISO, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Format
    {
      get => this.format;
      set => this.SetProperty<string>(nameof (Format), ref this.format, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string AgeRating
    {
      get => this.ageRating;
      set => this.SetProperty<string>(nameof (AgeRating), ref this.ageRating, value);
    }

    [Browsable(true)]
    [DefaultValue(YesNo.Unknown)]
    [ResetValue(0)]
    public YesNo BlackAndWhite
    {
      get => this.blackAndWhite;
      set => this.SetProperty<YesNo>(nameof (BlackAndWhite), ref this.blackAndWhite, value);
    }

    [Browsable(true)]
    [DefaultValue(MangaYesNo.Unknown)]
    [ResetValue(0)]
    public MangaYesNo Manga
    {
      get => this.manga;
      set => this.SetProperty<MangaYesNo>(nameof (Manga), ref this.manga, value);
    }

    [Browsable(true)]
    [DefaultValue(0)]
    [ResetValue(1)]
    public int PreferredFrontCover
    {
      get => this.preferredFrontCover;
      set
      {
        if (!this.SetProperty<int>(nameof (PreferredFrontCover), ref this.preferredFrontCover, value))
          return;
        this.cachedFrontCoverPageIndex = -1;
      }
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Characters
    {
      get => this.characters;
      set => this.SetProperty<string>(nameof (Characters), ref this.characters, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Teams
    {
      get => this.teams;
      set => this.SetProperty<string>(nameof (Teams), ref this.teams, value);
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string MainCharacterOrTeam
    {
      get => this.mainCharacterOrTeam;
      set
      {
        this.SetProperty<string>(nameof (MainCharacterOrTeam), ref this.mainCharacterOrTeam, value);
      }
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Locations
    {
      get => this.locations;
      set => this.SetProperty<string>(nameof (Locations), ref this.locations, value);
    }

    [Browsable(true)]
    [DefaultValue(0)]
    [ResetValue(0)]
    public float CommunityRating
    {
      get => this.communityRating;
      set
      {
        this.SetProperty<float>(nameof (CommunityRating), ref this.communityRating, value.Clamp(0.0f, 5f));
      }
    }

    [Browsable(true)]
    [Searchable]
    [DefaultValue("")]
    [ResetValue(0)]
    public string ScanInformation
    {
      get => this.scanInformation;
      set => this.SetProperty<string>(nameof (ScanInformation), ref this.scanInformation, value);
    }

    public void UpdatePageType(int page, ComicPageType value)
    {
      ComicPageInfo page1 = this.GetPage(page, true);
      if (page1.PageType == value)
        return;
      page1.PageType = value;
      this.Pages[page] = page1;
      this.FirePageChanged(page);
    }

    public void UpdatePageType(ComicPageInfo cpi, ComicPageType value)
    {
      int page = this.Pages.IndexOf(cpi);
      if (page < 0)
        return;
      this.UpdatePageType(page, value);
    }

    public void UpdatePageRotation(int page, ImageRotation value)
    {
      ComicPageInfo page1 = this.GetPage(page, true);
      if (page1.Rotation == value)
        return;
      page1.Rotation = value;
      this.Pages[page] = page1;
      this.FirePageChanged(page);
    }

    public void UpdatePageRotation(ComicPageInfo cpi, ImageRotation value)
    {
      int page = this.Pages.IndexOf(cpi);
      if (page < 0)
        return;
      this.UpdatePageRotation(page, value);
    }

    public void UpdatePagePosition(int page, ComicPagePosition value)
    {
      ComicPageInfo page1 = this.GetPage(page, true);
      if (page1.PagePosition == value)
        return;
      page1.PagePosition = value;
      this.Pages[page] = page1;
      this.FirePageChanged(page);
    }

    public void UpdateBookmark(int page, string bookmark)
    {
      ComicPageInfo page1 = this.GetPage(page, true);
      if (string.IsNullOrEmpty(page1.Bookmark) && string.IsNullOrEmpty(bookmark) || page1.Bookmark == bookmark)
        return;
      page1.Bookmark = bookmark;
      this.Pages[page] = page1;
      this.cachedBookmarkCount = -1;
      this.FirePageChanged(page);
    }

    public void SetPages(IEnumerable<ComicPageInfo> comicPages)
    {
      using (ItemMonitor.Lock((object) this.Pages))
      {
        if (this.Pages.SequenceEqual<ComicPageInfo>(comicPages))
          return;
        this.Pages.Clear();
        this.Pages.AddRange(comicPages);
        this.cachedBookmarkCount = -1;
      }
      this.FirePageChanged(-1);
    }

    public void UpdatePageSize(int page, int width, int height)
    {
      ComicPageInfo page1 = this.GetPage(page, true);
      if (page1.ImageHeight == height && page1.ImageWidth == width)
        return;
      page1.ImageHeight = height;
      page1.ImageWidth = width;
      this.Pages[page] = page1;
      this.FirePageChanged(page, false);
    }

    public void UpdatePageFileSize(int page, int size)
    {
      ComicPageInfo page1 = this.GetPage(page, true);
      if (page1.ImageFileSize == size)
        return;
      page1.ImageFileSize = size;
      this.Pages[page] = page1;
      this.FirePageChanged(page, false);
    }

    public int TranslatePageToImageIndex(int page)
    {
      return page < 0 ? page : this.GetPage(page).ImageIndex;
    }

    public int TranslateImageIndexToPage(int imageIndex)
    {
      using (ItemMonitor.Lock((object) this.Pages))
      {
        int index = this.Pages.FindIndex((Predicate<ComicPageInfo>) (cpi => cpi.ImageIndex == imageIndex));
        return index == -1 ? imageIndex : index;
      }
    }

    public ComicPageInfo GetPage(int page, bool add = false)
    {
      bool updateComicInfo = false;
      using (ItemMonitor.Lock((object) this.Pages))
      {
        if (page < 0)
          return ComicPageInfo.Empty;
        if (page < this.Pages.Count)
          return this.Pages[page];
        if (!add)
          return new ComicPageInfo(page);
        while (this.Pages.Count <= page)
        {
          ComicPageInfo comicPageInfo = this.OnNewComicPageAdded(new ComicPageInfo(this.Pages.Count));
          if (!comicPageInfo.IsDefaultContent(-1))
            updateComicInfo = true;
          this.Pages.Add(comicPageInfo);
        }
      }
      this.FirePageChanged(-1, updateComicInfo);
      return this.GetPage(page);
    }

    public ComicPageInfo GetPageByImageIndex(int imageIndex)
    {
      ComicPageInfo byImageIndex = this.Pages.FindByImageIndex(imageIndex);
      return byImageIndex.IsEmpty ? this.GetPage(imageIndex) : byImageIndex;
    }

    public IEnumerable<ComicPageInfo> GetPageList()
    {
      return Enumerable.Range(0, this.PageCount).Select<int, ComicPageInfo>((Func<int, ComicPageInfo>) (n => this.GetPage(n)));
    }

    public void MovePages(int position, IEnumerable<ComicPageInfo> pages)
    {
      using (ItemMonitor.Lock((object) this.Pages))
      {
        foreach (ComicPageInfo page in (List<ComicPageInfo>) this.Pages)
        {
          if (this.Pages.IndexOf(page) == -1)
          {
            for (int count = this.Count; count < page.ImageIndex - 1; ++count)
              this.Pages.Add(new ComicPageInfo(count));
            this.Pages.Add(page);
          }
        }
        foreach (ComicPageInfo page in pages)
        {
          int index = this.Pages.IndexOf(page);
          this.Pages.RemoveAt(index);
          if (index < position)
            --position;
          if (position == -1)
          {
            this.Pages.Add(page);
          }
          else
          {
            try
            {
              this.Pages.Insert(position++, page);
            }
            catch
            {
              this.Pages.Add(page);
            }
          }
        }
      }
      this.FirePageChanged(-1);
    }

    public int FrontCoverPageIndex
    {
      get
      {
        if (this.cachedFrontCoverPageIndex != -1)
          return this.cachedFrontCoverPageIndex;
        ComicPageInfo[] array1 = this.GetPageList().ToArray<ComicPageInfo>();
        ComicPageInfo[] array2 = ((IEnumerable<ComicPageInfo>) array1).Where<ComicPageInfo>((Func<ComicPageInfo, bool>) (pi => pi.PageType == ComicPageType.FrontCover)).ToArray<ComicPageInfo>();
        int index = this.PreferredFrontCover.Clamp(0, array2.Length - 1);
        ComicPageInfo comicPageInfo = index == -1 || array2.Length == 0 ? ComicPageInfo.Empty : array2[index];
        if (comicPageInfo.IsEmpty)
          comicPageInfo = ((IEnumerable<ComicPageInfo>) array1).Where<ComicPageInfo>((Func<ComicPageInfo, bool>) (p => p.PageType != ComicPageType.Other)).FirstOrDefault<ComicPageInfo>();
        this.cachedFrontCoverPageIndex = comicPageInfo.IsEmpty ? 0 : this.TranslateImageIndexToPage(comicPageInfo.ImageIndex);
        return this.cachedFrontCoverPageIndex;
      }
    }

    public int FrontCoverCount
    {
      get
      {
        int frontCoverCount = this.cachedFrontCoverCount;
        if (frontCoverCount == -1)
          frontCoverCount = this.GetPageList().Where<ComicPageInfo>((Func<ComicPageInfo, bool>) (cpi => cpi.PageType == ComicPageType.FrontCover)).Count<ComicPageInfo>();
        this.cachedFrontCoverCount = frontCoverCount;
        return frontCoverCount;
      }
    }

    public int FirstNonCoverPageIndex => this.FrontCoverPageIndex + 1;

    public void ResetPageSequence()
    {
      if (this.pages == null)
        return;
      this.pages.ResetPageSequence();
      this.FirePageChanged(-1);
    }

    public void SortPages(Comparison<ComicPageInfo> comparison)
    {
      if (this.pages == null)
        return;
      this.pages.Sort(comparison);
      this.FirePageChanged(-1);
    }

    public void TrimExcessPageInfo()
    {
      if (this.pages == null)
        return;
      if (this.pages.Count == 0)
      {
        this.pages = (ComicPageInfoCollection) null;
      }
      else
      {
        using (ItemMonitor.Lock((object) this.pages))
        {
          for (int index = this.pages.Count - 1; index >= 0 && (index >= this.PageCount || this.pages[index].IsDefaultContent(index)); --index)
            this.pages.RemoveAt(index);
          this.pages.TrimExcess();
        }
      }
    }

    [XmlArrayItem("Page")]
    [Browsable(false)]
    public ComicPageInfoCollection Pages
    {
      get => this.pages ?? (this.pages = new ComicPageInfoCollection());
    }

    [XmlIgnore]
    public int BookmarkCount
    {
      get
      {
        if (this.cachedBookmarkCount == -1)
          this.cachedBookmarkCount = this.pages == null ? 0 : this.pages.Lock<ComicPageInfo>().Count<ComicPageInfo>((Func<ComicPageInfo, bool>) (pi => !string.IsNullOrEmpty(pi.Bookmark)));
        return this.cachedBookmarkCount;
      }
    }

    public IEnumerable<string> Bookmarks
    {
      get
      {
        return this.pages != null ? this.pages.Lock<ComicPageInfo>().Where<ComicPageInfo>((Func<ComicPageInfo, bool>) (pi => !string.IsNullOrEmpty(pi.Bookmark))).Select<ComicPageInfo, string>((Func<ComicPageInfo, string>) (pi => pi.Bookmark)) : Enumerable.Empty<string>();
      }
    }

    public void ClearBookmarks()
    {
      using (ItemMonitor.Lock((object) this.pages))
      {
        for (int page = 0; page < this.pages.Count; ++page)
          this.UpdateBookmark(page, string.Empty);
      }
    }

    [field: NonSerialized]
    public event EventHandler<BookChangedEventArgs> BookChanged;

    private bool SetProperty<T>(string name, ref T property, T value)
    {
      if (object.Equals((object) property, (object) value))
        return false;
      T oldValue = property;
      property = value;
      this.FireBookChanged(name, (object) oldValue, (object) value);
      return true;
    }

    private void FireBookChanged(string name, object oldValue, object newValue)
    {
      this.OnBookChanged(new BookChangedEventArgs(name, true, oldValue, newValue));
    }

    private void FirePageChanged(int page, bool updateComicInfo = true)
    {
      this.cachedFrontCoverPageIndex = this.cachedFrontCoverCount = -1;
      this.OnBookChanged(new BookChangedEventArgs("Pages", page, updateComicInfo));
    }

    protected virtual void OnBookChanged(BookChangedEventArgs e)
    {
      if (this.BookChanged == null)
        return;
      this.BookChanged((object) this, e);
    }

    protected virtual ComicPageInfo OnNewComicPageAdded(ComicPageInfo info) => info;

    public void AppendArtistInfo(ComicInfo ci)
    {
      this.Penciller = this.Penciller.AppendWithSeparator(", ", ci.Penciller);
      this.Writer = this.Writer.AppendWithSeparator(", ", ci.Writer);
      this.Inker = this.Inker.AppendWithSeparator(", ", ci.Inker);
      this.Colorist = this.Colorist.AppendWithSeparator(", ", ci.Colorist);
      this.Letterer = this.Letterer.AppendWithSeparator(", ", ci.Letterer);
      this.CoverArtist = this.CoverArtist.AppendWithSeparator(", ", ci.CoverArtist);
    }

    public virtual void SetInfo(ComicInfo ci, bool onlyUpdateEmpty = true, bool updatePages = true)
    {
      if (ci == null)
        return;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Writer))
        this.Writer = ci.Writer;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Publisher))
        this.Publisher = ci.Publisher;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Imprint))
        this.Imprint = ci.Imprint;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Penciller))
        this.Penciller = ci.Penciller;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Inker))
        this.Inker = ci.Inker;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Title))
        this.Title = ci.Title;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Series))
        this.Series = ci.Series;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.AlternateSeries))
        this.AlternateSeries = ci.AlternateSeries;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.StoryArc))
        this.StoryArc = ci.StoryArc;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.SeriesGroup))
        this.SeriesGroup = ci.SeriesGroup;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Summary))
        this.Summary = ci.Summary;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Notes))
        this.Notes = ci.Notes;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Review))
        this.Review = ci.Review;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Genre))
        this.Genre = ci.Genre;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Colorist))
        this.Colorist = ci.Colorist;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Editor))
        this.Editor = ci.Editor;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Letterer))
        this.Letterer = ci.Letterer;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.CoverArtist))
        this.CoverArtist = ci.CoverArtist;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Web))
        this.Web = ci.Web;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.LanguageISO))
        this.LanguageISO = ci.LanguageISO;
      if (!onlyUpdateEmpty || this.BlackAndWhite == YesNo.Unknown)
        this.BlackAndWhite = ci.BlackAndWhite;
      if (!onlyUpdateEmpty || this.Manga == MangaYesNo.Unknown)
        this.Manga = ci.Manga;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Format))
        this.Format = ci.Format;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.AgeRating))
        this.AgeRating = ci.AgeRating;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Characters))
        this.Characters = ci.Characters;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Teams))
        this.Teams = ci.Teams;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.MainCharacterOrTeam))
        this.MainCharacterOrTeam = ci.MainCharacterOrTeam;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Locations))
        this.Locations = ci.Locations;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.ScanInformation))
        this.ScanInformation = ci.ScanInformation;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.Number))
        this.Number = ci.Number;
      if (!onlyUpdateEmpty || this.Count == -1)
        this.Count = ci.Count;
      if (!onlyUpdateEmpty || string.IsNullOrEmpty(this.AlternateNumber))
        this.AlternateNumber = ci.AlternateNumber;
      if (!onlyUpdateEmpty || this.AlternateCount == -1)
        this.AlternateCount = ci.AlternateCount;
      if (!onlyUpdateEmpty || this.Volume == -1)
        this.Volume = ci.Volume;
      if (!onlyUpdateEmpty || this.Year == -1)
        this.Year = ci.Year;
      if (!onlyUpdateEmpty || this.Month == -1)
        this.Month = ci.Month;
      if (!onlyUpdateEmpty || this.Day == -1)
        this.Day = ci.Day;
      if (!onlyUpdateEmpty || (double) this.CommunityRating == 0.0)
        this.CommunityRating = ci.CommunityRating;
      if (!updatePages || ci.PageCount == 0)
        return;
      if (onlyUpdateEmpty)
      {
        if (this.Pages.Count < ci.Pages.Count)
          this.Pages.Clear();
        if (this.PageCount < ci.PageCount)
          this.PageCount = ci.PageCount;
      }
      else
      {
        this.PageCount = ci.PageCount;
        this.Pages.Clear();
      }
      using (ItemMonitor.Lock((object) ci.Pages))
      {
        if (this.Pages.Count == 0)
        {
          ci.Pages.ForEach((Action<ComicPageInfo>) (cpi => this.Pages.Add(cpi)));
        }
        else
        {
          for (int index = 0; index < Math.Min(ci.Pages.Count, this.Pages.Count); ++index)
          {
            ComicPageInfo page = this.Pages[index];
            this.UpdatePageType(index, page.PageType);
            this.UpdateBookmark(index, page.Bookmark);
            this.UpdatePageRotation(index, page.Rotation);
          }
        }
      }
    }

    public ComicInfo GetInfo()
    {
      using (ItemMonitor.Lock((object) this))
      {
        ComicInfo ci = new ComicInfo()
        {
          Writer = this.Writer,
          Publisher = this.Publisher,
          Imprint = this.Imprint,
          Penciller = this.Penciller,
          Inker = this.Inker,
          Series = this.Series,
          Number = this.Number,
          Count = this.Count,
          AlternateSeries = this.AlternateSeries,
          AlternateNumber = this.AlternateNumber,
          AlternateCount = this.AlternateCount,
          SeriesGroup = this.SeriesGroup,
          StoryArc = this.StoryArc,
          Title = this.Title,
          Summary = this.Summary,
          Volume = this.Volume,
          Year = this.Year,
          Month = this.Month,
          Day = this.Day,
          Notes = this.Notes,
          Genre = this.Genre,
          Colorist = this.Colorist,
          Editor = this.Editor,
          Letterer = this.Letterer,
          CoverArtist = this.CoverArtist,
          Web = this.Web,
          PageCount = this.PageCount,
          LanguageISO = this.LanguageISO,
          BlackAndWhite = this.BlackAndWhite,
          Manga = this.Manga,
          Format = this.Format,
          AgeRating = this.AgeRating,
          Characters = this.Characters,
          Teams = this.Teams,
          Locations = this.Locations,
          ScanInformation = this.ScanInformation
        };
        this.Pages.ForEach((Action<ComicPageInfo>) (cpi => ci.Pages.Add(cpi)));
        return ci;
      }
    }

    public bool IsSameContent(ComicInfo ci, bool withPages = true)
    {
      if (ci == null || !(ci.Writer == this.Writer) || !(ci.Publisher == this.Publisher) || !(ci.Imprint == this.Imprint) || !(ci.Inker == this.Inker) || !(ci.Penciller == this.Penciller) || !(ci.Title == this.Title) || !(ci.Number == this.Number) || ci.Count != this.Count || !(ci.Summary == this.Summary) || !(ci.Series == this.Series) || ci.Volume != this.Volume || !(ci.AlternateSeries == this.AlternateSeries) || !(ci.AlternateNumber == this.AlternateNumber) || ci.AlternateCount != this.AlternateCount || !(ci.StoryArc == this.StoryArc) || !(ci.SeriesGroup == this.SeriesGroup) || ci.Year != this.Year || ci.Month != this.Month || ci.Day != this.Day || !(ci.Notes == this.Notes) || !(ci.Review == this.Review) || !(ci.Genre == this.Genre) || !(ci.Colorist == this.Colorist) || !(ci.Editor == this.Editor) || !(ci.Letterer == this.Letterer) || !(ci.CoverArtist == this.CoverArtist) || !(ci.Web == this.Web) || !(ci.LanguageISO == this.LanguageISO) || ci.PageCount != this.PageCount || !(ci.Format == this.Format) || !(ci.AgeRating == this.AgeRating) || ci.BlackAndWhite != this.BlackAndWhite || ci.Manga != this.Manga || !(ci.Characters == this.Characters) || !(ci.Teams == this.Teams) || !(ci.MainCharacterOrTeam == this.MainCharacterOrTeam) || !(ci.Locations == this.Locations) || !(ci.ScanInformation == this.ScanInformation))
        return false;
      return !withPages || ci.Pages.PagesAreEqual(this.Pages);
    }

    public void Serialize(Stream outStream)
    {
      try
      {
        XmlUtility.GetSerializer<ComicInfo>().Serialize(outStream, (object) this);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static ComicInfo Deserialize(Stream inStream)
    {
      try
      {
        return XmlUtility.GetSerializer<ComicInfo>().Deserialize(inStream) as ComicInfo;
      }
      catch
      {
        return (ComicInfo) null;
      }
    }

    public static ComicInfo LoadFromSidecar(string file)
    {
      try
      {
        using (FileStream inStream = File.OpenRead(file + ".xml"))
          return ComicInfo.Deserialize((Stream) inStream);
      }
      catch (Exception ex)
      {
        return (ComicInfo) null;
      }
    }

    public byte[] ToArray()
    {
      using (MemoryStream outStream = new MemoryStream())
      {
        this.Serialize((Stream) outStream);
        return outStream.ToArray();
      }
    }

    public string ToXml() => Encoding.Default.GetString(this.ToArray());

    public static string YesText => ComicInfo.yesText.Value;

    public static string NoText => ComicInfo.noText.Value;

    public static string YesRightToLeftText => ComicInfo.yesRightToLeftText.Value;

    public static string GetYesNoAsText(YesNo yn)
    {
      if (yn == YesNo.No)
        return ComicInfo.noText.Value;
      return yn == YesNo.Yes ? ComicInfo.yesText.Value : string.Empty;
    }

    public static string GetYesNoAsText(MangaYesNo yn)
    {
      return yn == MangaYesNo.YesAndRightToLeft ? ComicInfo.yesRightToLeftText.Value : ComicInfo.GetYesNoAsText((YesNo) yn);
    }

    public static string GetYesNoAsText(bool b)
    {
      return ComicInfo.GetYesNoAsText(b ? YesNo.Yes : YesNo.No);
    }

    public static bool IsValidCoverKey(string fileKey)
    {
      string file = Path.GetFileName(fileKey);
      return !((IEnumerable<string>) ComicInfo.coverKeyFilter).Any<string>((Func<string, bool>) (f => file.Contains(f, StringComparison.OrdinalIgnoreCase)));
    }

    public static bool SeriesEquals(string a, string b, CompareSeriesOptions options)
    {
      if (options.HasFlag((Enum) CompareSeriesOptions.IgnoreVolumeInName))
      {
        a = ComicInfo.rxVolume.Replace(a, string.Empty).Trim();
        b = ComicInfo.rxVolume.Replace(b, string.Empty).Trim();
      }
      if (options.HasFlag((Enum) CompareSeriesOptions.StripDown))
      {
        a = ComicInfo.rxSpecial.Replace(a, string.Empty);
        b = ComicInfo.rxSpecial.Replace(b, string.Empty);
      }
      return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
  }
}

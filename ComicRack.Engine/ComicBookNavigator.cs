// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookNavigator
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookNavigator : DisposableObject, IImageProvider, IDisposable, IImageKeyProvider
  {
    public static bool TrackCurrentPage = true;
    private int initialPage;
    private ImageProvider provider;
    private readonly ComicBook comic;
    private BitmapAdjustment baseColorAdjustment = BitmapAdjustment.Empty;
    private ComicPageType pageFilter = ComicPageType.All;
    private ImagePartInfo pagePart = ImagePartInfo.Empty;
    private YesNo rightToLeftReading = YesNo.Unknown;
    private volatile bool updateCurrentPageEnabled = true;
    private volatile Bitmap thumbnail;
    private volatile int currentPage;
    private volatile int lastPageRead;

    public ComicBookNavigator(ComicBook comic)
    {
      this.comic = comic;
      this.provider = comic.CreateImageProvider();
      if (this.provider == null)
        throw new ArgumentException("No valid comic book");
      this.provider.ImageReady += new EventHandler<ImageIndexReadyEventArgs>(this.ProviderImageIndexReady);
      this.provider.IndexRetrievalCompleted += new EventHandler<IndexRetrievalCompletedEventArgs>(this.ProviderIndexRetrievalCompleted);
      this.comic.FileRenamed += new EventHandler<ComicBookFileRenameEventArgs>(this.ComicFileRenamed);
      this.comic.BookChanged += new EventHandler<BookChangedEventArgs>(this.ComicBookChanged);
    }

    public void Open(bool async, int initialPage)
    {
      try
      {
        this.OnIndexRetrievalStarted();
        this.provider.Open(this.Source, async);
        this.initialPage = initialPage;
      }
      catch (Exception ex)
      {
        this.OnIndexRetrievalCompleted();
        this.OnErrorOpening();
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Thumbnail = (Bitmap) null;
        this.comic.FileRenamed -= new EventHandler<ComicBookFileRenameEventArgs>(this.ComicFileRenamed);
        this.comic.BookChanged -= new EventHandler<BookChangedEventArgs>(this.ComicBookChanged);
        if (this.provider != null)
        {
          this.provider.ImageReady -= new EventHandler<ImageIndexReadyEventArgs>(this.ProviderImageIndexReady);
          this.provider.IndexRetrievalCompleted -= new EventHandler<IndexRetrievalCompletedEventArgs>(this.ProviderIndexRetrievalCompleted);
          ThreadUtility.RunInBackground("Background Provider Dispose", new ThreadStart(((IDisposable) this.provider).Dispose));
        }
        this.provider = (ImageProvider) null;
      }
      base.Dispose(disposing);
    }

    public ComicBook Comic => this.comic;

    public string Source => this.Comic.FilePath;

    public string Caption => this.Comic.Caption;

    public int ProviderPageCount
    {
      get
      {
        this.CheckDisposed();
        return this.provider.Count;
      }
    }

    public BitmapAdjustment BaseColorAdjustment
    {
      get
      {
        this.CheckDisposed();
        using (ItemMonitor.Lock((object) this))
          return this.baseColorAdjustment;
      }
      set
      {
        this.CheckDisposed();
        using (ItemMonitor.Lock((object) this))
        {
          if (this.baseColorAdjustment == value)
            return;
          this.baseColorAdjustment = value;
        }
        this.OnColorAdjustmentChanged();
      }
    }

    public BitmapAdjustment ColorAdjustment
    {
      get
      {
        this.CheckDisposed();
        return BitmapAdjustment.Add(this.BaseColorAdjustment, this.Comic.ColorAdjustment);
      }
    }

    public ComicPageType PageFilter
    {
      get
      {
        this.CheckDisposed();
        return this.pageFilter;
      }
      set
      {
        this.CheckDisposed();
        if (this.pageFilter == value)
          return;
        this.pageFilter = value;
        this.OnPageFilterChanged();
      }
    }

    public ImagePartInfo PagePart
    {
      get => this.pagePart;
      set => this.pagePart = value;
    }

    [DefaultValue(false)]
    public YesNo RightToLeftReading
    {
      get => this.rightToLeftReading;
      set
      {
        if (this.rightToLeftReading == value)
          return;
        this.rightToLeftReading = value;
        this.OnRightToLeftReadingChanged();
      }
    }

    [DefaultValue(true)]
    public bool UpdateCurrentPageEnabled
    {
      get => this.updateCurrentPageEnabled;
      set => this.updateCurrentPageEnabled = value;
    }

    public Bitmap Thumbnail
    {
      get => this.thumbnail;
      set
      {
        if (value == this.thumbnail)
          return;
        Bitmap thumbnail = this.thumbnail;
        this.thumbnail = value;
        thumbnail.SafeDispose();
      }
    }

    public int IndexPagesRetrieved { get; private set; }

    public ImageProviderStatus ProviderStatus
    {
      get
      {
        this.CheckDisposed();
        return this.provider.Status;
      }
    }

    public bool IsIndexRetrievalCompleted
    {
      get
      {
        this.CheckDisposed();
        return this.provider.IsIndexRetrievalCompleted;
      }
    }

    public string GetImageName(int imageIndex)
    {
      ProviderImageInfo imageInfo = this.GetImageInfo(imageIndex);
      return imageInfo != null && imageInfo.Name != null ? imageInfo.Name : string.Empty;
    }

    public string GetImageName(int imageIndex, bool noPath)
    {
      string path = this.GetImageName(imageIndex);
      if (!string.IsNullOrEmpty(path) & noPath)
        path = Path.GetFileName(path);
      return path;
    }

    public int SeekNextPage(int page, int count, int direction, bool noFilter = false)
    {
      int num = Math.Abs(count);
      int currentPage = this.CurrentPage;
      for (; page >= 0 && page < this.Comic.PageCount; page += direction)
      {
        if (noFilter || this.comic.GetPage(page).IsTypeOf(this.pageFilter) || page == currentPage)
        {
          if (num == 0)
            return page;
          --num;
        }
        if (direction == 0)
          break;
      }
      return -1;
    }

    public int SeekNewPage(int offset, PageSeekOrigin pageSeekOrigin, bool noFilter = false)
    {
      int count = Math.Abs(offset);
      int page;
      int direction;
      switch (pageSeekOrigin)
      {
        case PageSeekOrigin.End:
          page = this.ProviderPageCount - 1;
          direction = -1;
          break;
        case PageSeekOrigin.Current:
          page = this.CurrentPage;
          direction = offset == 0 ? 1 : Math.Sign(offset);
          break;
        case PageSeekOrigin.Absolute:
          return offset.Clamp(0, this.comic.PageCount - 1);
        default:
          page = 0;
          direction = 1;
          break;
      }
      return this.SeekNextPage(page, count, direction, noFilter);
    }

    public ComicPageInfo GetPageInfo(int offset, PageSeekOrigin pageSeekOrigin)
    {
      return this.Comic.GetPage(this.SeekNewPage(offset, pageSeekOrigin));
    }

    public IEnumerable<int> GetPages(bool noFilter = false)
    {
      for (int p = this.SeekNextPage(0, 0, 1, noFilter); p != -1; p = this.SeekNextPage(p, 1, 1, noFilter))
        yield return p;
    }

    public IEnumerable<ComicPageInfo> GetPageInfos(bool noFilter = false)
    {
      return (IEnumerable<ComicPageInfo>) this.GetPages(noFilter).Select<int, ComicPageInfo>((Func<int, ComicPageInfo>) (page => this.Comic.GetPage(page))).ToArray<ComicPageInfo>();
    }

    public int CurrentPage
    {
      get => this.currentPage;
      set
      {
        if (this.IsIndexRetrievalCompleted && value >= this.ProviderPageCount)
          value = this.ProviderPageCount - 1;
        if (value < 0)
          value = 0;
        int currentPage = this.currentPage;
        if (value == this.currentPage)
          return;
        this.currentPage = value;
        if (this.updateCurrentPageEnabled && ComicBookNavigator.TrackCurrentPage)
          this.Comic.CurrentPage = this.CurrentPage;
        if (this.Navigation == null)
          return;
        this.Navigation((object) this, new BookPageEventArgs(this.Comic, currentPage, this.CurrentPage, this.CurrentPageInfo, this.CurrentPageName));
      }
    }

    public int LastPageRead
    {
      get => this.lastPageRead;
      set
      {
        if (value == this.LastPageRead)
          return;
        this.lastPageRead = value;
        if (value == this.LastPageRead || !this.updateCurrentPageEnabled || !ComicBookNavigator.TrackCurrentPage)
          return;
        this.Comic.LastPageRead = this.lastPageRead;
      }
    }

    public int NextPage => this.SeekNextPage(this.CurrentPage, 1, 1);

    public ComicPageInfo CurrentPageInfo => this.Comic.GetPage(this.CurrentPage);

    public string CurrentPageAsText
    {
      get
      {
        return string.Format("{0} {1}", (object) TR.Default["Page", "Page"], (object) (this.CurrentPage + 1));
      }
    }

    public string CurrentPageName
    {
      get
      {
        try
        {
          return this.GetImageInfo(this.CurrentPageInfo.ImageIndex).Name;
        }
        catch
        {
          return string.Empty;
        }
      }
    }

    public bool Navigate(int offset, PageSeekOrigin pageSeekOrigin, bool noFilter)
    {
      int num = this.SeekNewPage(offset, pageSeekOrigin, noFilter);
      if (num == -1)
        return false;
      this.CurrentPage = num;
      return true;
    }

    public bool Navigate(int offset, PageSeekOrigin pageSeekOrigin)
    {
      return this.Navigate(offset, pageSeekOrigin, false);
    }

    public bool Navigate(int offset) => this.Navigate(offset, PageSeekOrigin.Current);

    public bool Navigate(PageSeekOrigin pageSeekOrigin) => this.Navigate(0, pageSeekOrigin);

    public bool CanNavigate(int offset, PageSeekOrigin pageSeekOrigin)
    {
      return this.SeekNewPage(offset, pageSeekOrigin) != -1;
    }

    public bool CanNavigate(int offset) => this.CanNavigate(offset, PageSeekOrigin.Current);

    public int SeekBookmark(int page, int count)
    {
      int direction = Math.Sign(count);
      count = Math.Abs(count);
      do
        ;
      while (count-- > 0 && (page = this.Comic.Pages.SeekBookmark(page + direction, direction)) != -1);
      return page;
    }

    public bool NavigateBookmark(int count)
    {
      int num = this.SeekBookmark(this.CurrentPage, count);
      bool flag = num != -1;
      if (flag)
        this.CurrentPage = num;
      return flag;
    }

    public bool CanNavigateBookmark(int count) => this.SeekBookmark(this.CurrentPage, count) != -1;

    public event EventHandler IndexRetrievalStarted;

    public event EventHandler<BookPageEventArgs> IndexOfPageReady;

    public event EventHandler IndexRetrievalCompleted;

    public event EventHandler<BookPageEventArgs> Navigation;

    public event EventHandler ErrorOpening;

    public event EventHandler Opened;

    public event EventHandler ColorAdjustmentChanged;

    public event EventHandler RightToLeftReadingChanged;

    public event EventHandler PageFilterChanged;

    public event EventHandler PagesChanged;

    protected virtual void OnPageFilterChanged()
    {
      if (this.IsDisposed || this.PageFilterChanged == null)
        return;
      this.PageFilterChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnRightToLeftReadingChanged()
    {
      if (this.IsDisposed || this.RightToLeftReadingChanged == null)
        return;
      this.RightToLeftReadingChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnColorAdjustmentChanged()
    {
      if (this.IsDisposed || this.ColorAdjustmentChanged == null)
        return;
      this.ColorAdjustmentChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnPagesChanged()
    {
      if (this.IsDisposed || this.PagesChanged == null)
        return;
      this.PagesChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnIndexRetrievalStarted()
    {
      if (this.IsDisposed || this.IndexRetrievalStarted == null)
        return;
      this.IndexRetrievalStarted((object) this, EventArgs.Empty);
    }

    protected virtual void OnIndexRetrievalCompleted()
    {
      if (this.IsDisposed || this.IndexRetrievalCompleted == null)
        return;
      this.IndexRetrievalCompleted((object) this, EventArgs.Empty);
    }

    protected virtual void OnIndexOfPageReady(BookPageEventArgs bpea)
    {
      if (this.IsDisposed || this.IndexOfPageReady == null)
        return;
      this.IndexOfPageReady((object) this, bpea);
    }

    protected virtual void OnErrorOpening()
    {
      if (this.IsDisposed || this.ErrorOpening == null)
        return;
      this.ErrorOpening((object) this, EventArgs.Empty);
    }

    protected virtual void OnOpened()
    {
      if (this.IsDisposed || this.Opened == null)
        return;
      this.Opened((object) this, EventArgs.Empty);
    }

    private void ProviderImageIndexReady(object sender, ImageIndexReadyEventArgs e)
    {
      if (this.IsDisposed)
        return;
      int page1 = this.comic.TranslateImageIndexToPage(e.ImageNumber);
      ComicPageInfo page2 = this.comic.GetPage(page1);
      this.IndexPagesRetrieved = page1;
      this.comic.PageCount = Math.Max(this.comic.PageCount, page1);
      if (e.ImageInfo.Size > 0L)
        this.comic.UpdatePageFileSize(page1, (int) e.ImageInfo.Size);
      if (!string.IsNullOrEmpty(e.ImageInfo.Name) && page2.IsFrontCover && !ComicInfo.IsValidCoverKey(e.ImageInfo.Name))
        this.comic.UpdatePageType(page1, ComicPageType.Other);
      this.OnIndexOfPageReady(new BookPageEventArgs(this.Comic, page1, page1, this.comic.GetPage(page1), e.ImageInfo.Name));
      if (this.initialPage == 0 || this.initialPage != page1)
        return;
      this.CurrentPage = this.initialPage;
    }

    private void ProviderIndexRetrievalCompleted(object sender, IndexRetrievalCompletedEventArgs e)
    {
      if (this.IsDisposed)
        return;
      this.OnIndexRetrievalCompleted();
      if (e.Status != ImageProviderStatus.Error)
      {
        this.Comic.PageCount = this.ProviderPageCount;
        this.Comic.TrimExcessPageInfo();
        if (this.CurrentPage >= this.ProviderPageCount)
          this.CurrentPage = this.ProviderPageCount - 1;
        this.LastPageRead = this.Comic.LastPageRead;
        if (this.LastPageRead >= this.ProviderPageCount)
          this.LastPageRead = this.ProviderPageCount - 1;
        this.OnOpened();
      }
      else
        this.OnErrorOpening();
    }

    private void ComicFileRenamed(object sender, ComicBookFileRenameEventArgs e)
    {
      if (this.IsDisposed)
        return;
      this.provider.ChangeSourceLocation(e.NewFile);
    }

    private void ComicBookChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.IsDisposed)
        return;
      switch (e.PropertyName)
      {
        case "ColorAdjustment":
          this.OnColorAdjustmentChanged();
          break;
        case "Pages":
          this.OnPagesChanged();
          break;
      }
    }

    public int Count => this.ProviderPageCount;

    public bool IsSlow => this.provider != null && this.provider.IsSlow;

    public Bitmap GetImage(int index)
    {
      return this.provider == null ? (Bitmap) null : this.provider.GetImage(index);
    }

    public byte[] GetByteImage(int index)
    {
      return this.provider == null ? (byte[]) null : this.provider.GetByteImage(index);
    }

    public ProviderImageInfo GetImageInfo(int index)
    {
      return this.provider == null ? (ProviderImageInfo) null : this.provider.GetImageInfo(index);
    }

    public ThumbnailImage GetThumbnail(int index) => this.provider.GetThumbnail(index);

    public PageKey GetPageKey(int page) => this.GetPageKey(page, this.ColorAdjustment);

    public PageKey GetPageKey() => this.GetPageKey(this.CurrentPage);

    public PageKey GetPageKey(int page, BitmapAdjustment colorAdjustment)
    {
      return this.Comic.GetPageKey(page, colorAdjustment);
    }

    public ThumbnailKey GetThumbnailKey(int page) => this.Comic.GetThumbnailKey(page);

    public ImageKey GetImageKey(int page) => (ImageKey) this.GetPageKey(page);
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Controls.ThumbnailControl
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Controls
{
  public class ThumbnailControl : Control, IBitmapDisplayControl, IDisposable
  {
    private ComicBook comicBook;
    private volatile Bitmap image;
    private IThumbnailPool thumbnailPool;
    private int page = -1;
    private ThumbnailDrawingOptions drawingFlags = ThumbnailDrawingOptions.Default;
    private ComicTextElements textElements = ComicTextElements.DefaultComic;
    private bool tile;
    private BitmapAdjustment colorAdjustment = BitmapAdjustment.Empty;
    private bool highQuality = true;
    private bool threeD;
    private string publishedYear;
    private string publisherIcon;
    private string imprintIcon;
    private string ageRatingIcon;
    private string formatIcon;
    private string tagsIcon;
    private YesNo? seriesCompleteIcon;
    private YesNo? blackAndWhiteIcon;
    private MangaYesNo? mangaIcon;

    public ThumbnailControl()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.ResizeRedraw, true);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.comicBook != null)
        this.comicBook.BookChanged -= new EventHandler<BookChangedEventArgs>(this.comicBook_PropertyChanged);
      base.Dispose(disposing);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public ComicBook ComicBook
    {
      get => this.comicBook;
      set
      {
        if (this.ComicBook == value)
          return;
        if (this.comicBook != null)
          this.comicBook.BookChanged -= new EventHandler<BookChangedEventArgs>(this.comicBook_PropertyChanged);
        this.comicBook = value;
        if (this.comicBook != null)
          this.comicBook.BookChanged += new EventHandler<BookChangedEventArgs>(this.comicBook_PropertyChanged);
        this.Invalidate();
      }
    }

    [DefaultValue(null)]
    public Bitmap Bitmap
    {
      get => this.image;
      set
      {
        if (this.image == value)
          return;
        this.image = value;
        this.Invalidate();
      }
    }

    [DefaultValue(null)]
    public IThumbnailPool ThumbnailPool
    {
      get => this.thumbnailPool;
      set
      {
        if (this.thumbnailPool == value)
          return;
        this.thumbnailPool = value;
        this.Invalidate();
      }
    }

    [DefaultValue(-1)]
    public int Page
    {
      get => this.page;
      set
      {
        if (this.page == value)
          return;
        this.page = value;
        this.Invalidate();
      }
    }

    [DefaultValue(ThumbnailDrawingOptions.Default)]
    public ThumbnailDrawingOptions DrawingFlags
    {
      get => this.drawingFlags;
      set
      {
        if (this.drawingFlags == value)
          return;
        this.drawingFlags = value;
        this.Invalidate();
      }
    }

    [DefaultValue(ComicTextElements.DefaultComic)]
    public ComicTextElements TextElements
    {
      get => this.textElements;
      set
      {
        if (this.textElements == value)
          return;
        this.textElements = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool Tile
    {
      get => this.tile;
      set
      {
        if (this.tile == value)
          return;
        this.tile = value;
        this.Invalidate();
      }
    }

    [DefaultValue(typeof (BitmapAdjustment), "0, 0, 0")]
    public BitmapAdjustment ColorAdjustment
    {
      get => this.colorAdjustment;
      set
      {
        if (this.colorAdjustment == value)
          return;
        this.colorAdjustment = value;
        this.Invalidate();
      }
    }

    [DefaultValue(true)]
    public bool HighQuality
    {
      get => this.highQuality;
      set
      {
        if (this.highQuality == value)
          return;
        this.highQuality = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool ThreeD
    {
      get => this.threeD;
      set
      {
        if (value == this.threeD)
          return;
        this.threeD = value;
        this.Invalidate();
      }
    }

    [DefaultValue(null)]
    public string PublishedYear
    {
      get
      {
        string publishedYear = this.publishedYear;
        if (publishedYear != null)
          return publishedYear;
        return this.comicBook == null ? this.publishedYear : this.comicBook.YearAsText;
      }
      set
      {
        if (this.publishedYear == value)
          return;
        this.publishedYear = value;
        this.Invalidate();
      }
    }

    [DefaultValue(null)]
    public string PublisherIcon
    {
      get
      {
        string publisherIcon = this.publisherIcon;
        if (publisherIcon != null)
          return publisherIcon;
        return this.comicBook == null ? this.publisherIcon : this.comicBook.Publisher;
      }
      set
      {
        if (this.publisherIcon == value)
          return;
        this.publisherIcon = value;
        this.Invalidate();
      }
    }

    [DefaultValue(null)]
    public string ImprintIcon
    {
      get
      {
        string imprintIcon = this.imprintIcon;
        if (imprintIcon != null)
          return imprintIcon;
        return this.comicBook == null ? this.imprintIcon : this.comicBook.Imprint;
      }
      set
      {
        if (this.imprintIcon == value)
          return;
        this.imprintIcon = value;
        this.Invalidate();
      }
    }

    [DefaultValue(null)]
    public string AgeRatingIcon
    {
      get
      {
        string ageRatingIcon = this.ageRatingIcon;
        if (ageRatingIcon != null)
          return ageRatingIcon;
        return this.comicBook == null ? this.ageRatingIcon : this.comicBook.AgeRating;
      }
      set
      {
        if (this.ageRatingIcon == value)
          return;
        this.ageRatingIcon = value;
        this.Invalidate();
      }
    }

    [DefaultValue(null)]
    public string FormatIcon
    {
      get
      {
        string formatIcon = this.formatIcon;
        if (formatIcon != null)
          return formatIcon;
        return this.comicBook == null ? this.formatIcon : this.comicBook.Format;
      }
      set
      {
        if (this.formatIcon == value)
          return;
        this.formatIcon = value;
        this.Invalidate();
      }
    }

    [DefaultValue(null)]
    public string TagsIcon
    {
      get
      {
        string tagsIcon = this.tagsIcon;
        if (tagsIcon != null)
          return tagsIcon;
        return this.comicBook == null ? this.tagsIcon : this.comicBook.Tags;
      }
      set
      {
        if (this.tagsIcon == value)
          return;
        this.tagsIcon = value;
        this.Invalidate();
      }
    }

    [DefaultValue(YesNo.Unknown)]
    public YesNo SeriesCompleteIcon
    {
      get
      {
        if (this.seriesCompleteIcon.HasValue)
          return this.seriesCompleteIcon.Value;
        return this.comicBook == null ? YesNo.Unknown : this.comicBook.SeriesComplete;
      }
      set
      {
        if (this.seriesCompleteIcon.HasValue && this.seriesCompleteIcon.Value == value)
          return;
        this.seriesCompleteIcon = new YesNo?(value);
        this.Invalidate();
      }
    }

    [DefaultValue(YesNo.Unknown)]
    public YesNo BlackAndWhiteIcon
    {
      get
      {
        if (this.blackAndWhiteIcon.HasValue)
          return this.blackAndWhiteIcon.Value;
        return this.comicBook == null ? YesNo.Unknown : this.comicBook.BlackAndWhite;
      }
      set
      {
        if (this.blackAndWhiteIcon.HasValue && this.blackAndWhiteIcon.Value == value)
          return;
        this.blackAndWhiteIcon = new YesNo?(value);
        this.Invalidate();
      }
    }

    [DefaultValue(MangaYesNo.Unknown)]
    public MangaYesNo MangaIcon
    {
      get
      {
        if (this.mangaIcon.HasValue)
          return this.mangaIcon.Value;
        return this.comicBook == null ? MangaYesNo.Unknown : this.comicBook.Manga;
      }
      set
      {
        if (this.mangaIcon.HasValue && this.mangaIcon.Value == value)
          return;
        this.mangaIcon = new MangaYesNo?(value);
        this.Invalidate();
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      using (e.Graphics.SaveState())
      {
        if (this.HighQuality)
        {
          e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
          e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }
        Bitmap bitmap1 = (Bitmap) null;
        Bitmap bitmap2 = this.Bitmap;
        if (bitmap2 == null && this.thumbnailPool != null)
        {
          using (IImageProvider provider = (IImageProvider) this.ComicBook.OpenProvider(this.Page))
          {
            using (IItemLock<ThumbnailImage> thumbnail = this.thumbnailPool.GetThumbnail(this.Page == -1 ? this.ComicBook.GetFrontCoverThumbnailKey() : this.ComicBook.GetThumbnailKey(this.Page), provider, false))
            {
              if (thumbnail != null)
              {
                if (thumbnail.Item != null)
                  bitmap1 = bitmap2 = thumbnail.Item.Bitmap.Clone() as Bitmap;
              }
            }
          }
        }
        try
        {
          using (Image adjustedBitmap = (Image) bitmap2.CreateAdjustedBitmap(this.colorAdjustment, PixelFormat.Format32bppArgb, true))
          {
            Image image = adjustedBitmap ?? (Image) bitmap2;
            if (this.tile)
              ThumbTileRenderer.DrawTile(e.Graphics, this.ClientRectangle, image, this.comicBook, this.page, this.Font, this.ForeColor, Color.Transparent, this.drawingFlags, this.textElements, this.ThreeD, this.GetIcons());
            else
              ThumbRenderer.DrawThumbnail(e.Graphics, image, this.ClientRectangle, this.drawingFlags, this.comicBook);
            if (!this.DesignMode)
              return;
            ControlPaint.DrawFocusRectangle(e.Graphics, this.ClientRectangle);
          }
        }
        finally
        {
          bitmap1?.Dispose();
        }
      }
    }

    private IEnumerable<Image> GetIcons()
    {
      if (this.ComicBook == null)
        return Enumerable.Empty<Image>();
      ComicBook comicBook = (ComicBook) this.ComicBook.Clone();
      int result;
      if (!int.TryParse(this.PublishedYear, out result))
        result = -1;
      comicBook.Publisher = this.PublisherIcon;
      comicBook.Year = result;
      comicBook.Imprint = this.ImprintIcon;
      comicBook.AgeRating = this.AgeRatingIcon;
      comicBook.Format = this.FormatIcon;
      comicBook.Tags = this.TagsIcon;
      comicBook.Manga = this.MangaIcon;
      comicBook.SeriesComplete = this.SeriesCompleteIcon;
      comicBook.BlackAndWhite = this.BlackAndWhiteIcon;
      return comicBook.GetIcons();
    }

    public void SetBitmap(Bitmap image)
    {
      Bitmap bitmap = this.Bitmap;
      this.Bitmap = image;
      bitmap?.Dispose();
    }

    private void comicBook_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.Invalidate();
    }

    [SpecialName]
    object IBitmapDisplayControl.get_Tag() => this.Tag;

    [SpecialName]
    void IBitmapDisplayControl.set_Tag(object value) => this.Tag = value;
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.CoverViewItem
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Net.Search;
using cYo.Common.Reflection;
using cYo.Common.Text;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class CoverViewItem : ThumbnailViewItem, IViewableItemHitTest, ISetCustomThumbnail
  {
    public const int GapInfoWidth = 16;
    public const int NumericSpinWidth = 11;
    public const int CheckBoxWidth = 22;
    public static readonly Bitmap MarkerLastOpenedImage = Resources.LastMarker;
    public static readonly Bitmap MarkerIsFileLessImage = Resources.FilelessMarker;
    public static readonly Bitmap MarkerIsOpenImage = Resources.OpenMarker;
    public static readonly Bitmap IsDirtyImage = Resources.UpdateBig;
    private int stackReadPercent = -1;
    private float averageRating = -1f;
    private float averageCommunityRating = -1f;
    private CoverViewItem.ItemGapType gapType = CoverViewItem.ItemGapType.Undefined;
    private string customThumbnailKey;
    private int thumbnailLabelHeight = 40;
    private float thumbnailLabelFontScale = 1f;
    private static readonly Image yellowStar = (Image) Resources.StarYellow;
    private static readonly Image blueStar = (Image) Resources.StarBlue;
    private static readonly Bitmap bitmapGapUp = Resources.GapUp.CropTransparent(height: false).ScaleDpi();
    private static readonly Bitmap bitmapGapDown = Resources.GapDown.CropTransparent(height: false).ScaleDpi();
    private static readonly Bitmap bitmapGapUpDown = Resources.GapUpDown.CropTransparent(height: false).ScaleDpi();
    private const string SeriesStatsPrefix = "SeriesStat";
    private static readonly int SeriesStatsPrefixLength = "SeriesStat".Length;
    private static readonly Bitmap linkArrow = Resources.SmallArrowRight.ScaleDpi();
    private int labelLines = 3;
    private MarkerType marker;
    private Rectangle? drawnRect;
    private bool refreshed;

    public static CoverViewItem Create(
      ComicBook comic,
      int position,
      IComicBookStatsProvider statsProvider)
    {
      CoverViewItem coverViewItem = new CoverViewItem()
      {
        Position = position,
        Comic = comic,
        StatsProvider = statsProvider
      };
      coverViewItem.Comic.BookChanged += new EventHandler<BookChangedEventArgs>(coverViewItem.Comic_PropertyChanged);
      coverViewItem.Tag = (object) comic.FilePath;
      return coverViewItem;
    }

    public int StackReadPercent
    {
      get
      {
        if (this.View == null || !this.View.IsStack((IViewableItem) this))
          return this.Comic.ReadPercentage;
        if (this.stackReadPercent < 0)
        {
          int stackCount = this.View.GetStackCount((IViewableItem) this);
          this.stackReadPercent = this.View.GetStackItems((IViewableItem) this).OfType<CoverViewItem>().Count<CoverViewItem>((Func<CoverViewItem, bool>) (cvi => cvi.Comic.HasBeenRead)) * 100 / stackCount;
        }
        return this.stackReadPercent;
      }
    }

    public float AverageRating
    {
      get
      {
        if (this.View == null || !this.View.IsStack((IViewableItem) this))
          return this.Comic.Rating;
        if ((double) this.averageRating < 0.0)
        {
          try
          {
            this.averageRating = this.View.GetStackItems((IViewableItem) this).OfType<CoverViewItem>().Select<CoverViewItem, float>((Func<CoverViewItem, float>) (cvi => cvi.Comic.Rating)).Where<float>((Func<float, bool>) (r => (double) r > 0.0)).Average<float>((Func<float, float>) (r => r));
          }
          catch (Exception ex)
          {
            this.averageRating = 0.0f;
          }
        }
        return this.averageRating;
      }
    }

    public float AverageCommunityRating
    {
      get
      {
        if (this.View == null || !this.View.IsStack((IViewableItem) this))
          return this.Comic.CommunityRating;
        if ((double) this.averageCommunityRating < 0.0)
        {
          try
          {
            this.averageCommunityRating = this.View.GetStackItems((IViewableItem) this).OfType<CoverViewItem>().Select<CoverViewItem, float>((Func<CoverViewItem, float>) (cvi => cvi.Comic.CommunityRating)).Where<float>((Func<float, bool>) (r => (double) r > 0.0)).Average<float>((Func<float, float>) (r => r));
          }
          catch (Exception ex)
          {
            this.averageCommunityRating = 0.0f;
          }
        }
        return this.averageCommunityRating;
      }
    }

    private CoverViewItem.ItemGapType GapType
    {
      get
      {
        if (this.gapType == CoverViewItem.ItemGapType.Undefined && this.SeriesStats != null)
        {
          this.gapType = CoverViewItem.ItemGapType.None;
          if (this.SeriesStats.IsGapStart(this.Comic))
            this.gapType |= CoverViewItem.ItemGapType.Start;
          if (this.SeriesStats.IsGapEnd(this.Comic))
            this.gapType |= CoverViewItem.ItemGapType.End;
        }
        return this.gapType;
      }
    }

    protected virtual void OnComicPropertyChanged(string name)
    {
      switch (name)
      {
        case "Rating":
          this.averageRating = -1f;
          break;
        case "CommunityRating":
          this.averageCommunityRating = -1f;
          break;
      }
    }

    protected override void OnRefreshImage()
    {
      base.OnRefreshImage();
      Program.ImagePool.Thumbs.RefreshImage((ImageKey) this.Comic.GetThumbnailKey(this.Comic.FirstNonCoverPageIndex));
    }

    protected override System.Drawing.Size GetEstimatedSize(System.Drawing.Size canvasSize)
    {
      if (this.Comic == null || this.Comic.PageCount < 1)
        return base.GetEstimatedSize(canvasSize);
      ComicPageInfo page = this.Comic.GetPage(this.Comic.FrontCoverPageIndex);
      System.Drawing.Size size = new System.Drawing.Size(page.ImageWidth, page.ImageHeight);
      if (size.Width <= 0 || size.Height <= 0)
        return base.GetEstimatedSize(canvasSize);
      size = size.ToRectangle(new System.Drawing.Size(0, 512), RectangleScaleMode.None).Size;
      return ThumbRenderer.GetSafeScaledImageSize(size, canvasSize);
    }

    public override ItemViewStates GetOwnerDrawnStates(ItemViewMode mode)
    {
      return mode != ItemViewMode.Detail ? ItemViewStates.All : base.GetOwnerDrawnStates(mode);
    }

    public override ThumbnailKey ThumbnailKey
    {
      get
      {
        return string.IsNullOrEmpty(this.customThumbnailKey) || !this.View.IsStack((IViewableItem) this) ? this.Comic.GetFrontCoverThumbnailKey() : this.Comic.GetThumbnailKey(0, this.customThumbnailKey);
      }
    }

    public string CustomThumbnailKey
    {
      get => this.customThumbnailKey;
      set
      {
        if (this.customThumbnailKey == value)
          return;
        this.customThumbnailKey = value;
        this.Update(true);
      }
    }

    public bool HasCustomThumbnail => !string.IsNullOrEmpty(this.customThumbnailKey);

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.Comic.BookChanged -= new EventHandler<BookChangedEventArgs>(this.Comic_PropertyChanged);
      base.Dispose(disposing);
    }

    public override string Text
    {
      get => this.Comic.Caption;
      set => base.Text = value;
    }

    public override string Name
    {
      get => this.Comic.CaptionWithoutFormat;
      set => base.Name = value;
    }

    public int ThumbnailLabelHeight => this.thumbnailLabelHeight;

    public float ThumbnailLabelFontScale => this.thumbnailLabelFontScale;

    protected override System.Drawing.Size MeasureItem(
      Graphics graphics,
      System.Drawing.Size defaultSize,
      ItemViewMode displayType)
    {
      defaultSize = base.MeasureItem(graphics, defaultSize, displayType);
      if (displayType != ItemViewMode.Thumbnail)
        return defaultSize;
      System.Drawing.Size thumbnailSizeSafe = this.GetThumbnailSizeSafe(defaultSize);
      if (CoverViewItem.ThumbnailSizing != CoverThumbnailSizing.None)
        thumbnailSizeSafe.Width = (int) ((double) thumbnailSizeSafe.Height * (double) ThumbRenderer.DefaultThumbnailAspect);
      System.Drawing.Size size = this.AddBorder(thumbnailSizeSafe);
      this.thumbnailLabelFontScale = ((float) defaultSize.Height / 192f).Clamp(0.7f, 1f);
      this.thumbnailLabelHeight = this.ThumbnailConfig == null || !this.ThumbnailConfig.HideCaptions ? (int) ((double) this.LabelLines * ((double) this.View.CurrentFontHeight * (double) this.ThumbnailLabelFontScale + 2.0)) : 0;
      size.Height += this.thumbnailLabelHeight;
      return size;
    }

    protected override System.Drawing.Size GetDefaultMaximumSize(System.Drawing.Size defaultSize)
    {
      int height = defaultSize.Height;
      return CoverViewItem.ThumbnailSizing == CoverThumbnailSizing.None ? new System.Drawing.Size(height * 2, height) : new System.Drawing.Size(height * 1000, height);
    }

    protected override System.Drawing.Size MeasureColumn(
      Graphics graphics,
      IColumn header,
      System.Drawing.Size defaultSize)
    {
      defaultSize = base.MeasureColumn(graphics, header, defaultSize);
      ComicListField tag = (ComicListField) header.Tag;
      switch (tag.DisplayProperty)
      {
        case "BlackAndWhiteAsText":
        case "Checked":
        case "EnableProposedAsText":
        case "HasBeenReadAsText":
        case "LinkedAsText":
        case "SeriesCompleteAsText":
          defaultSize.Width = FormUtility.ScaleDpiX(22);
          break;
        case "CommunityRating":
        case "Rating":
        case "SeriesStatAverageCommunityRating":
        case "SeriesStatAverageRating":
          defaultSize.Width = FormUtility.ScaleDpiX(80);
          break;
        case "Cover":
          defaultSize.Width = FormUtility.ScaleDpiX(128);
          break;
        case "GapInformation":
          defaultSize.Width = FormUtility.ScaleDpiX(16);
          break;
        case "Icons":
          defaultSize.Width = FormUtility.ScaleDpiX(64);
          break;
        case "State":
          defaultSize.Width = FormUtility.ScaleDpiX(24);
          break;
        default:
          string text = tag.DisplayProperty == "Position" ? this.Position.ToString() : this.GetColumnStringValue(tag.DisplayProperty, header.FormatId, tag.ValueType, true, tag.DefaultText);
          if (!string.IsNullOrEmpty(text))
          {
            defaultSize = graphics.MeasureString(text, this.View.Font).ToSize();
            defaultSize.Width += 8;
            break;
          }
          break;
      }
      switch (tag.DisplayProperty)
      {
        case "AlternateCountAsText":
        case "AlternateNumberAsText":
        case "CountAsText":
        case "DayAsText":
        case "MonthAsText":
        case "NumberAsText":
        case "PagesAsTextSimple":
        case "VolumeAsText":
        case "YearAsText":
          defaultSize.Width += FormUtility.ScaleDpiX(11);
          break;
      }
      return defaultSize;
    }

    public override void OnDraw(ItemDrawInformation drawInfo)
    {
      base.OnDraw(drawInfo);
      if (drawInfo.SubItem == -1)
        this.OnRefreshComicData();
      bool flag = this.View.IsStack((IViewableItem) this);
      Color textColor = drawInfo.TextColor;
      Rectangle rectangle = drawInfo.Bounds;
      Font font1 = this.View.Font;
      List<Image> imageList = (List<Image>) null;
      if (this.Comic.ComicInfoIsDirty && this.Comic.IsLinked && Program.Settings.UpdateComicFiles)
        imageList = imageList.SafeAdd<Image>((Image) CoverViewItem.IsDirtyImage);
      switch (this.marker)
      {
        case MarkerType.IsOpen:
          imageList = imageList.SafeAdd<Image>((Image) CoverViewItem.MarkerIsOpenImage);
          break;
        case MarkerType.IsLast:
          imageList = imageList.SafeAdd<Image>((Image) CoverViewItem.MarkerLastOpenedImage);
          break;
      }
      if (this.Comic.NewPages > 0)
        imageList = imageList.SafeAdd<Image>((Image) ThumbRenderer.GetNewPageStatusImage(this.Comic.NewPages));
      if (!this.Comic.IsLinked)
        imageList = imageList.SafeAdd<Image>((Image) CoverViewItem.MarkerIsFileLessImage);
      if (this.Comic.EditMode.IsLocalComic() && this.Comic.IsLinked && this.Comic.FileIsMissing)
        imageList = imageList.SafeAdd<Image>((Image) ThumbnailViewItem.DeletedStateImage);
      ThumbnailRatingMode thumbnailRatingMode = !Program.Settings.NumericRatingThumbnails ? (EngineConfiguration.Default.RatingStarsBelowThumbnails ? ThumbnailRatingMode.StarsBelow : ThumbnailRatingMode.StarsOverlay) : ThumbnailRatingMode.Tags;
      using (StringFormat format = new StringFormat())
      {
        this.drawnRect = new Rectangle?();
        int height = drawInfo.DisplayType == ItemViewMode.Detail ? 256 : rectangle.Height;
        using (IItemLock<ThumbnailImage> thumbnail1 = this.GetThumbnail(drawInfo))
        {
          Image image = (Image) null;
          Image thumbnail2 = (Image) thumbnail1?.Item.GetThumbnail(height);
          IItemLock<ThumbnailImage> itemLock = (IItemLock<ThumbnailImage>) null;
          ThumbnailDrawingOptions flags1 = ThumbnailDrawingOptions.Default;
          if (this.Selected)
            flags1 |= ThumbnailDrawingOptions.Selected;
          if (this.Hot)
            flags1 |= ThumbnailDrawingOptions.Hot;
          if (this.Focused)
            flags1 |= ThumbnailDrawingOptions.Focused;
          if (flag)
          {
            if (this.HasCustomThumbnail)
              flags1 |= ThumbnailDrawingOptions.NoOpaqueCover;
            else
              flags1 |= ThumbnailDrawingOptions.Stacked;
          }
          if (this.HasCustomThumbnail)
            flags1 &= ~(ThumbnailDrawingOptions.EnableShadow | ThumbnailDrawingOptions.EnableBorder | ThumbnailDrawingOptions.EnableRating | ThumbnailDrawingOptions.EnableVerticalBookmarks | ThumbnailDrawingOptions.EnableHorizontalBookmarks);
          if (!this.Comic.IsLinked)
            flags1 &= ~(ThumbnailDrawingOptions.EnableVerticalBookmarks | ThumbnailDrawingOptions.EnableHorizontalBookmarks);
          if (this.HasCustomThumbnail & flag)
            flags1 &= ~ThumbnailDrawingOptions.EnableBowShadow;
          if (this.View.InScrollOrResize)
            flags1 |= ThumbnailDrawingOptions.FastMode;
          try
          {
            if (!this.HasCustomThumbnail && thumbnail1 != null && (this.Hot || this.Selected) && drawInfo.DisplayType != ItemViewMode.Detail && this.Comic.IsLinked && Program.Settings.DogEarThumbnails && this.Comic.PageCount > 1 && !this.Comic.FileIsMissing)
            {
              itemLock = this.GetBackThumbnail();
              image = (Image) itemLock?.Item.GetThumbnail(height);
            }
            switch (drawInfo.DisplayType)
            {
              case ItemViewMode.Thumbnail:
                this.Animate(thumbnail2);
                ThumbIconRenderer thumbIconRenderer1 = new ThumbIconRenderer(thumbnail2, flags1);
                thumbIconRenderer1.Border = this.Border;
                thumbIconRenderer1.ForeColor = textColor;
                thumbIconRenderer1.PageCount = this.Comic.PageCount;
                thumbIconRenderer1.RatingMode = thumbnailRatingMode;
                thumbIconRenderer1.Rating1 = this.AverageRating;
                thumbIconRenderer1.Rating2 = this.AverageCommunityRating;
                thumbIconRenderer1.ComicCount = this.View.GetStackCount((IViewableItem) this);
                thumbIconRenderer1.SelectionBackColor = StyledRenderer.GetSelectionColor(drawInfo.ControlFocused);
                thumbIconRenderer1.BookmarkPercentMode = flag;
                ThumbIconRenderer thumbIconRenderer2 = thumbIconRenderer1;
                int[] numArray1;
                if (!flag)
                  numArray1 = new int[2]
                  {
                    this.Comic.CurrentPage,
                    this.Comic.LastPageRead
                  };
                else
                  numArray1 = new int[1]
                  {
                    this.StackReadPercent
                  };
                thumbIconRenderer2.Bookmarks = numArray1;
                thumbIconRenderer1.BackImage = image;
                thumbIconRenderer1.ImageOpacity = this.Opacity;
                ThumbIconRenderer thumbIconRenderer3 = thumbIconRenderer1;
                if (imageList != null)
                  thumbIconRenderer3.StateImages.AddRange((IEnumerable<Image>) imageList);
                if (this.ThumbnailConfig != null && this.ThumbnailConfig.HideCaptions)
                {
                  thumbIconRenderer3.TextHeight = 0;
                }
                else
                {
                  thumbIconRenderer3.TextHeight = this.ThumbnailLabelHeight;
                  this.CreateThumbnailLines((ICollection<TextLine>) thumbIconRenderer3.TextLines, FC.GetRelative(font1, this.ThumbnailLabelFontScale), textColor);
                }
                this.drawnRect = new Rectangle?(thumbIconRenderer3.Draw(drawInfo.Graphics, rectangle));
                thumbIconRenderer3.DisposeTextLines();
                this.OnDrawCustomThumbnailOverlay(drawInfo.Graphics, thumbIconRenderer3.ThumbnailBounds);
                this.DrawFrontCoverButton(drawInfo.Graphics, thumbIconRenderer3.ThumbnailBounds);
                if (thumbIconRenderer3.RatingStripRenderer == null || !this.Comic.EditMode.CanEditProperties())
                  break;
                RatingRenderer r1 = thumbIconRenderer3.RatingStripRenderer;
                this.AddClickRegion(r1.Bounds, (Action<Rectangle, System.Drawing.Point>) ((rect, pt) => this.SetSelectedComics((Action<ComicBook>) (cb => cb.Rating = r1.GetRatingFromStrip(pt.Add(rect.Location))), TR.Load("Columns")["Rating"])));
                break;
              case ItemViewMode.Tile:
                this.Animate(thumbnail2);
                ThumbTileRenderer thumbTileRenderer1 = new ThumbTileRenderer(thumbnail2, flags1);
                thumbTileRenderer1.ImageOpacity = this.Opacity;
                thumbTileRenderer1.Font = font1;
                thumbTileRenderer1.Border = this.Border;
                thumbTileRenderer1.ForeColor = textColor;
                thumbTileRenderer1.BackColor = Color.LightGray;
                thumbTileRenderer1.SelectionBackColor = StyledRenderer.GetSelectionColor(drawInfo.ControlFocused);
                thumbTileRenderer1.PageCount = this.Comic.PageCount;
                thumbTileRenderer1.RatingMode = thumbnailRatingMode;
                thumbTileRenderer1.Rating1 = this.AverageRating;
                thumbTileRenderer1.Rating2 = this.AverageCommunityRating;
                thumbTileRenderer1.ComicCount = this.View.GetStackCount((IViewableItem) this);
                thumbTileRenderer1.BookmarkPercentMode = flag;
                ThumbTileRenderer thumbTileRenderer2 = thumbTileRenderer1;
                int[] numArray2;
                if (!flag)
                  numArray2 = new int[2]
                  {
                    this.Comic.CurrentPage,
                    this.Comic.LastPageRead
                  };
                else
                  numArray2 = new int[1]
                  {
                    this.StackReadPercent
                  };
                thumbTileRenderer2.Bookmarks = numArray2;
                thumbTileRenderer1.BackImage = image;
                thumbTileRenderer1.Icons = this.Comic.GetIcons();
                ThumbTileRenderer thumbTileRenderer3 = thumbTileRenderer1;
                Font font2 = FC.Get(font1, ((float) rectangle.Height * 0.07f).Clamp(font1.Size * 0.8f, font1.Size * 1f));
                ComicBook comicBook;
                ComicTextElements flags2;
                if (flag)
                {
                  comicBook = this.CreateStackInfo();
                  flags2 = ComicTextElements.DefaultStack;
                }
                else
                {
                  flags2 = this.ThumbnailConfig != null ? this.ThumbnailConfig.TextElements : ComicTextElements.DefaultFileComic;
                  comicBook = this.Comic;
                }
                thumbTileRenderer3.TextLines.AddRange(ComicTextBuilder.GetTextBlocks(comicBook, font2, textColor, flags2));
                if (imageList != null)
                  thumbTileRenderer3.StateImages.AddRange((IEnumerable<Image>) imageList);
                thumbTileRenderer3.DrawTile(drawInfo.Graphics, rectangle);
                thumbTileRenderer3.DisposeTextLines();
                this.OnDrawCustomThumbnailOverlay(drawInfo.Graphics, thumbTileRenderer3.ThumbnailBounds);
                this.DrawFrontCoverButton(drawInfo.Graphics, thumbTileRenderer3.ThumbnailBounds);
                if (thumbTileRenderer3.RatingStripRenderer == null || !this.Comic.EditMode.CanEditProperties())
                  break;
                RatingRenderer r2 = thumbTileRenderer3.RatingStripRenderer;
                this.AddClickRegion(r2.Bounds, (Action<Rectangle, System.Drawing.Point>) ((rect, pt) => this.SetSelectedComics((Action<ComicBook>) (cb => cb.Rating = r2.GetRatingFromStrip(pt.Add(rect.Location))), TR.Load("Columns")["Rating"])));
                break;
              case ItemViewMode.Detail:
                if (drawInfo.Header != null)
                {
                  ComicListField tag = (ComicListField) drawInfo.Header.Tag;
                  string displayProperty = tag.DisplayProperty;
                  switch (displayProperty)
                  {
                    case "BlackAndWhiteAsText":
                      this.DrawCheckBox(drawInfo.Graphics, tag, rectangle, this.Comic.BlackAndWhite, (Action<ComicBook, YesNo>) ((cb, x) => cb.BlackAndWhite = x));
                      return;
                    case "Checked":
                      this.DrawCheckBox(drawInfo.Graphics, tag, rectangle, this.Comic.Checked ? YesNo.Yes : YesNo.No, (Action<ComicBook, YesNo>) ((cb, x) => cb.Checked = x == YesNo.Yes), true);
                      return;
                    case "CommunityRating":
                      this.DrawRating(drawInfo.Graphics, tag, rectangle, this.Comic.CommunityRating, CoverViewItem.blueStar, (Action<ComicBook, float>) ((cb, x) => cb.CommunityRating = x));
                      return;
                    case "Cover":
                      if (!drawInfo.ExpandedColumn)
                        this.Animate(thumbnail2);
                      if (thumbnail2 == null)
                        return;
                      rectangle.Height = rectangle.Width * thumbnail2.Height / thumbnail2.Width;
                      rectangle.Inflate(-2, -2);
                      drawInfo.Graphics.IntersectClip(rectangle);
                      ThumbnailDrawingOptions flags3 = ThumbnailDrawingOptions.EnableShadow | ThumbnailDrawingOptions.EnableHorizontalBookmarks;
                      if (this.View.InScrollOrResize)
                        flags3 |= ThumbnailDrawingOptions.FastMode;
                      ThumbRenderer.DrawThumbnail(drawInfo.Graphics, thumbnail2, rectangle, flags3, this.Comic, this.Opacity);
                      return;
                    case "EnableProposedAsText":
                      this.DrawCheckBox(drawInfo.Graphics, tag, rectangle, this.Comic.EnableProposed ? YesNo.Yes : YesNo.No, (Action<ComicBook, YesNo>) ((cb, x) => cb.EnableProposed = x == YesNo.Yes), true);
                      return;
                    case "GapInformation":
                      this.DrawGapInfo(drawInfo.Graphics, tag, rectangle, this.GapType);
                      return;
                    case "HasBeenReadAsText":
                      this.DrawCheckBox(drawInfo.Graphics, tag, rectangle, this.Comic.HasBeenRead ? YesNo.Yes : YesNo.No, (Action<ComicBook, YesNo>) ((cb, x) => cb.HasBeenRead = x == YesNo.Yes), true);
                      return;
                    case "Icons":
                      Graphics graphics = drawInfo.Graphics;
                      using (this.View.InScrollOrResize ? (IDisposable) null : graphics.HighQuality(true))
                      {
                        Rectangle bounds = drawInfo.Bounds;
                        bounds.Inflate(-2, -2);
                        ThumbRenderer.DrawImageList(graphics, this.Comic.GetIcons(), bounds, ContentAlignment.MiddleCenter, -0.1f);
                        return;
                      }
                    case "LinkedAsText":
                      this.DrawCheckBox(drawInfo.Graphics, tag, rectangle, this.Comic.IsLinked ? YesNo.Yes : YesNo.No);
                      return;
                    case "Rating":
                      this.DrawRating(drawInfo.Graphics, tag, rectangle, this.Comic.Rating, CoverViewItem.yellowStar, (Action<ComicBook, float>) ((cb, x) => cb.Rating = x));
                      return;
                    case "SeriesCompleteAsText":
                      this.DrawCheckBox(drawInfo.Graphics, tag, rectangle, this.Comic.SeriesComplete, (Action<ComicBook, YesNo>) ((cb, x) => cb.SeriesComplete = x));
                      return;
                    case "SeriesStatAverageCommunityRating":
                      if (this.SeriesStats == null)
                        return;
                      this.DrawRating(drawInfo.Graphics, tag, rectangle, this.SeriesStats.AverageCommunityRating, CoverViewItem.blueStar);
                      return;
                    case "SeriesStatAverageRating":
                      if (this.SeriesStats == null)
                        return;
                      this.DrawRating(drawInfo.Graphics, tag, rectangle, this.SeriesStats.AverageRating, CoverViewItem.yellowStar);
                      return;
                    case "State":
                      if (imageList == null || imageList.Count <= 0)
                        return;
                      Rectangle bounds1 = drawInfo.Bounds;
                      bounds1.Inflate(-2, -2);
                      ThumbRenderer.DrawImageList(drawInfo.Graphics, (IEnumerable<Image>) imageList, bounds1, ContentAlignment.MiddleCenter, ThumbRenderer.DefaultStateOverlap);
                      return;
                    default:
                      format.Alignment = drawInfo.Header.Alignment;
                      format.LineAlignment = StringAlignment.Center;
                      format.Trimming = tag.Trimming;
                      format.FormatFlags = StringFormatFlags.LineLimit;
                      string columnStringValue;
                      string str;
                      if (displayProperty == "Position")
                      {
                        int position = this.Position;
                        str = columnStringValue = position.ToString();
                      }
                      else
                      {
                        str = this.GetColumnStringValue(displayProperty, drawInfo.Header.FormatId, tag.ValueType, true, tag.DefaultText);
                        columnStringValue = this.GetColumnStringValue(displayProperty, drawInfo.Header.FormatId, tag.ValueType, defaultText: tag.DefaultText);
                      }
                      if (this.Focused)
                      {
                        rectangle = rectangle.Pad(0, 0, this.DrawSearchLinkButton(drawInfo.Graphics, rectangle, displayProperty, str));
                        if (this.Comic.EditMode.CanEditProperties() && !Program.ExtendedSettings.DisableListSpinButtons)
                        {
                          switch (displayProperty)
                          {
                            case "AlternateCountAsText":
                              rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, this.Comic.AlternateCount, (Action<ComicBook, int, int>) ((cb, old, x) => cb.AlternateCount = CoverViewItem.OneClamping(int.MaxValue, old, x)));
                              break;
                            case "AlternateNumberAsText":
                              rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, (int) this.Comic.CompareAlternateNumber.Number, (Action<ComicBook, int, int>) ((cb, old, x) => cb.AlternateNumber = x.ToString()));
                              break;
                            case "CountAsText":
                              rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, this.Comic.Count, (Action<ComicBook, int, int>) ((cb, old, x) => cb.Count = CoverViewItem.OneClamping(int.MaxValue, old, x)));
                              break;
                            case "DayAsText":
                              rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, this.Comic.Day, (Action<ComicBook, int, int>) ((cb, old, x) => cb.Day = CoverViewItem.OneClamping(31, old, x)));
                              break;
                            case "MonthAsText":
                              rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, this.Comic.Month, (Action<ComicBook, int, int>) ((cb, old, x) => cb.Month = CoverViewItem.OneClamping(12, old, x)));
                              break;
                            case "NumberAsText":
                              rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, (int) this.Comic.CompareNumber.Number, (Action<ComicBook, int, int>) ((cb, old, x) => cb.Number = x.ToString()));
                              break;
                            case "PagesAsTextSimple":
                              if (!this.Comic.IsLinked)
                              {
                                rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, this.Comic.PageCount, (Action<ComicBook, int, int>) ((cb, old, x) => cb.PageCount = x.Clamp(0, int.MaxValue)));
                                break;
                              }
                              break;
                            case "VolumeAsText":
                              rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, this.Comic.Volume, (Action<ComicBook, int, int>) ((cb, old, x) => cb.Volume = x.Clamp(-1, int.MaxValue)));
                              break;
                            case "YearAsText":
                              rectangle.Width -= this.DrawSpinUpDownButton(drawInfo.Graphics, tag, rectangle, this.Comic.Year, (Action<ComicBook, int, int>) ((cb, old, x) => cb.Year = CoverViewItem.OneClamping(10000, old, x)));
                              break;
                          }
                        }
                      }
                      rectangle.Inflate(-2, 0);
                      using (Brush brush = (Brush) new SolidBrush(str == columnStringValue ? textColor : Color.FromArgb(128, textColor)))
                      {
                        drawInfo.Graphics.DrawString(str, font1, brush, (RectangleF) rectangle, format);
                        return;
                      }
                  }
                }
                else
                {
                  if (drawInfo.GroupItem % 2 != 0 || (drawInfo.State & ItemViewStates.Selected) != ItemViewStates.None)
                    break;
                  using (Brush brush = (Brush) new SolidBrush(Color.LightGray.Transparent(96)))
                  {
                    drawInfo.Graphics.FillRectangle(brush, rectangle);
                    break;
                  }
                }
            }
          }
          finally
          {
            itemLock?.Dispose();
          }
        }
      }
    }

    private static int OneClamping(int max, int oldValue, int newValue)
    {
      if (newValue > max)
        return max;
      if (newValue >= 1)
        return newValue;
      return oldValue <= newValue ? 1 : -1;
    }

    private static void AddColumnUndo(string columnName)
    {
      if (string.IsNullOrEmpty(columnName))
        return;
      Program.Database.Undo.SetMarker(StringUtility.Format(TR.Messages["UndoEditColumn", "Edit Column '{0}'"], (object) columnName));
    }

    private void SetSelectedComics(Action<ComicBook> setFunction, string columnName = null)
    {
      if (this.View == null)
        return;
      CoverViewItem.AddColumnUndo(columnName);
      ((IEnumerable<CoverViewItem>) this.View.SelectedItems.Concat<IViewableItem>(ListExtensions.AsEnumerable<IViewableItem>(this.View.FocusedItem)).Where<IViewableItem>((Func<IViewableItem, bool>) (item => item != null)).Distinct<IViewableItem>().OfType<CoverViewItem>().ToArray<CoverViewItem>()).ForEach<CoverViewItem>((Action<CoverViewItem>) (cvi => setFunction(cvi.Comic)));
    }

    private int DrawSpinUpDownButton(
      Graphics gr,
      ComicListField clf,
      Rectangle rc,
      int start,
      Action<ComicBook, int, int> setFunction)
    {
      if (rc.Width < 16)
        return 0;
      Rectangle src = new Rectangle(rc.Right - FormUtility.ScaleDpiX(11), rc.Top + 3, FormUtility.ScaleDpiX(11), rc.Height - 5);
      SpinButton.Draw(gr, src, false);
      this.AddClickRegion(src, (Action<Rectangle, System.Drawing.Point>) ((rect, pt) => this.SetSelectedComics((Action<ComicBook>) (cb =>
      {
        switch (SpinButton.HitTest(src, pt.Add(rect.Location)))
        {
          case SpinButton.SpinButtonType.Up:
            setFunction(cb, start, start + 1);
            break;
          case SpinButton.SpinButtonType.Down:
            setFunction(cb, start, start - 1);
            break;
        }
      }), clf.Description)));
      return src.Width;
    }

    private void DrawRating(
      Graphics gr,
      ComicListField clf,
      Rectangle rc,
      float rating,
      Image image,
      Action<ComicBook, float> setFunction = null)
    {
      Rectangle bounds = rc.Pad(2, 4, 2, 4);
      RatingRenderer r = new RatingRenderer(image, bounds)
      {
        Fast = this.View.InScrollOrResize
      };
      if (setFunction != null && this.Comic.EditMode.CanEditProperties())
        this.AddClickRegion(bounds, (Action<Rectangle, System.Drawing.Point>) ((rect, pt) => this.SetSelectedComics((Action<ComicBook>) (cb => setFunction(cb, r.GetRatingFromStrip(pt.Add(rect.Location)))), clf.Description)));
      r.DrawRatingStrip(gr, rating, alpha2: 0.1f);
    }

    private void DrawCheckBox(
      Graphics gr,
      ComicListField clf,
      Rectangle rc,
      YesNo yesNo,
      Action<ComicBook, YesNo> setFunction = null,
      bool onlyYesNo = false)
    {
      int num1 = Math.Min(rc.Width - 4, rc.Height - 6);
      if (num1 < 4)
        return;
      Rectangle rectangle = new Rectangle(0, 0, num1, num1).Align(rc, ContentAlignment.MiddleCenter);
      int num2;
      switch (yesNo)
      {
        case YesNo.Unknown:
          num2 = 1;
          break;
        case YesNo.Yes:
          num2 = 0;
          break;
        default:
          num2 = -1;
          break;
      }
      YesNo newState = (YesNo) num2;
      int num3;
      switch (yesNo)
      {
        case YesNo.Unknown:
          num3 = 256;
          break;
        case YesNo.Yes:
          num3 = 1024;
          break;
        default:
          num3 = 0;
          break;
      }
      ButtonState buttonState = (ButtonState) num3;
      ControlPaint.DrawCheckBox(gr, rectangle, ButtonState.Flat | buttonState);
      if (onlyYesNo && newState == YesNo.Unknown)
        newState = YesNo.Yes;
      if (setFunction == null || !this.Comic.EditMode.CanEditProperties())
        return;
      this.AddClickRegion(rectangle, (Action<Rectangle, System.Drawing.Point>) ((rect, pt) => this.SetSelectedComics((Action<ComicBook>) (cb => setFunction(cb, newState)), clf.Description)));
    }

    private void DrawGapInfo(
      Graphics graphics,
      ComicListField clf,
      Rectangle rc,
      CoverViewItem.ItemGapType gapType)
    {
      Bitmap bitmap = (Bitmap) null;
      if (gapType.HasFlag((Enum) CoverViewItem.ItemGapType.End) && gapType.HasFlag((Enum) CoverViewItem.ItemGapType.Start))
        bitmap = CoverViewItem.bitmapGapUpDown;
      else if (gapType.HasFlag((Enum) CoverViewItem.ItemGapType.End))
        bitmap = CoverViewItem.bitmapGapUp;
      else if (gapType.HasFlag((Enum) CoverViewItem.ItemGapType.Start))
        bitmap = CoverViewItem.bitmapGapDown;
      if (bitmap == null)
        return;
      using (this.View.InScrollOrResize ? (IDisposable) null : graphics.HighQuality(true))
      {
        Rectangle bounds = rc.Pad(2, 4, 2, 4);
        float scale = bitmap.Size.GetScale(bounds.Size, allowOversize: false);
        graphics.DrawImage((Image) bitmap, bitmap.Size.Scale(scale).Align(bounds, ContentAlignment.MiddleCenter));
      }
    }

    private void CreateThumbnailLines(ICollection<TextLine> lines, Font f, Color textColor)
    {
      StringFormat format = new StringFormat()
      {
        Alignment = StringAlignment.Center
      };
      if (this.View.IsStack((IViewableItem) this))
      {
        int stackCount = this.View.GetStackCount((IViewableItem) this);
        lines.Add(new TextLine(this.View.GetStackCaption((IViewableItem) this), f, textColor, format));
        lines.Add(new TextLine(StringUtility.Format("{0} {1}", (object) stackCount, stackCount > 1 ? (object) TR.Default["Books", "books"] : (object) TR.Default["Book", "book"]), FC.GetRelative(f, 0.8f), textColor, format));
      }
      else if (this.ThumbnailConfig == null || this.ThumbnailConfig.CaptionIds.Count == 0)
      {
        format.Trimming = StringTrimming.EllipsisWord;
        lines.Add(new TextLine(this.Text, f, textColor, format));
      }
      else
      {
        int count = this.ThumbnailConfig.CaptionIds.Count;
        for (int index = 0; index < count; ++index)
        {
          IColumn byId = this.View.Columns.FindById(this.ThumbnailConfig.CaptionIds[index]);
          if (byId != null && byId.Tag is ComicListField tag)
          {
            string columnStringValue = this.GetColumnStringValue(tag.DisplayProperty, 0, tag.ValueType, true, tag.DefaultText);
            if (!string.IsNullOrEmpty(columnStringValue))
            {
              format.FormatFlags = count != 1 ? StringFormatFlags.NoWrap : (StringFormatFlags) 0;
              format.Trimming = StringTrimming.EllipsisCharacter;
              if (lines.Count == 1)
                f = FC.GetRelative(f, 0.8f);
              lines.Add(new TextLine(columnStringValue, f, textColor, format));
            }
          }
        }
      }
    }

    private ComicBook CreateStackInfo()
    {
      ComicBook comicBook = new ComicBook();
      comicBook.Series = this.View.GetStackCaption((IViewableItem) this);
      comicBook.Count = this.View.GetStackCount((IViewableItem) this);
      ComicBook stackInfo = comicBook;
      bool flag = true;
      foreach (ComicBook ci in this.View.GetStackItems((IViewableItem) this).OfType<CoverViewItem>().Select<CoverViewItem, ComicBook>((Func<CoverViewItem, ComicBook>) (ci => ci.Comic)))
      {
        if (flag)
        {
          stackInfo.AppendArtistInfo((ComicInfo) ci);
          flag = string.IsNullOrEmpty(stackInfo.ArtistInfo);
        }
        if (ci.HasBeenOpened)
          ++stackInfo.LastPageRead;
        stackInfo.FileSize += Math.Max(0L, ci.FileSize);
        stackInfo.PageCount += ci.PageCount;
      }
      return stackInfo;
    }

    protected override void CreateThumbnail(ThumbnailKey key)
    {
      using (Program.ImagePool.GetThumbnail(key, this.Comic))
        ;
    }

    private void Comic_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.OnPropertyChanged("Comic");
      this.OnComicPropertyChanged(e.PropertyName);
      this.OnBookChanged(e);
    }

    public string GetColumnStringValue(
      string displayProperty,
      int formatId,
      Type type,
      bool proposed = false,
      string defaultText = null)
    {
      try
      {
        return type != typeof (DateTime) ? this.GetColumnValue<string>(displayProperty, proposed) ?? defaultText ?? string.Empty : ComicBook.FormatDate(this.GetColumnValue<DateTime>(displayProperty, proposed), (ComicDateFormat) formatId, missingText: defaultText);
      }
      catch (Exception ex)
      {
        return defaultText ?? string.Empty;
      }
    }

    public T GetColumnValue<T>(string displayProperty, bool proposed = false)
    {
      if (this.StatsProvider == null || !displayProperty.StartsWith("SeriesStat"))
        return this.Comic.GetPropertyValue<T>(displayProperty, proposed);
      ComicBookSeriesStatistics seriesStats = this.StatsProvider.GetSeriesStats(this.Comic);
      displayProperty = displayProperty.Substring(CoverViewItem.SeriesStatsPrefixLength);
      return PropertyCaller.CreateGetMethod<ComicBookSeriesStatistics, T>(displayProperty)(seriesStats);
    }

    public IItemLock<ThumbnailImage> GetBackThumbnail()
    {
      ThumbnailKey tk = this.Comic.GetThumbnailKey(this.Comic.CurrentPage <= 0 ? this.Comic.FirstNonCoverPageIndex : this.Comic.CurrentPage);
      IItemLock<ThumbnailImage> image = Program.ImagePool.Thumbs.GetImage((ImageKey) tk, true);
      if (image == null)
        Program.ImagePool.AddThumbToQueue(tk, (object) this.View, (AsyncCallback) (ar => this.MakePageThumbnail(tk, false)));
      return image;
    }

    public void DrawFrontCoverButton(Graphics graphics, Rectangle thumbnailBounds)
    {
      if (this.HasCustomThumbnail || this.Comic.FrontCoverCount <= 1 || !this.Selected)
        return;
      string str = string.Format("- {0}/{1} +", (object) (this.Comic.PreferredFrontCover + 1), (object) this.Comic.FrontCoverCount);
      Font font = FC.Get("Arial", 7f);
      thumbnailBounds.Inflate(-2, -2);
      Rectangle rectangle1 = new Rectangle(System.Drawing.Point.Empty, graphics.MeasureString(str, font).ToSize());
      rectangle1.Inflate(2, 2);
      Rectangle rectangle2 = rectangle1.Align(thumbnailBounds, ContentAlignment.BottomCenter);
      using (graphics.AntiAlias())
      {
        using (GraphicsPath path = rectangle2.ConvertToPath(4, 4))
        {
          using (StringFormat format = new StringFormat()
          {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
          })
          {
            graphics.FillPath(Brushes.White, path);
            graphics.DrawString(str, font, Brushes.Black, (RectangleF) rectangle2, format);
            graphics.DrawPath(Pens.Black, path);
          }
        }
      }
      this.AddClickRegion(rectangle2, new Action<Rectangle, System.Drawing.Point>(this.ClickCoverButton));
    }

    private void ClickCoverButton(Rectangle bounds, System.Drawing.Point pt)
    {
      this.Comic.PreferredFrontCover = Numeric.Rollover(this.Comic.PreferredFrontCover, this.Comic.FrontCoverCount, pt.X >= bounds.Width / 2 ? 1 : -1);
    }

    private int DrawSearchLinkButton(Graphics graphics, Rectangle rc, string field, string text)
    {
      if (!Program.Settings.ShowSearchLinks || string.IsNullOrEmpty(text) || !this.Comic.IsSearchable(field))
        return 0;
      Rectangle rectangle = CoverViewItem.linkArrow.Size.ToRectangle().Align(rc, ContentAlignment.MiddleRight);
      graphics.DrawImage((Image) CoverViewItem.linkArrow, rectangle);
      this.AddClickRegion(rectangle, (Action<Rectangle, System.Drawing.Point>) ((ib, ipt) =>
      {
        System.Drawing.Point location = this.View.Translate(this.View.GetItemBounds((IViewableItem) this), false).Location;
        location.Offset(ib.Right, ib.Top);
        new SearchContextMenuBuilder().CreateContextMenu((IEnumerable<INetSearch>) SearchEngines.Engines, field, text, (Action<ContextMenuStrip>) (cms =>
        {
          if (string.IsNullOrEmpty(this.Comic.Web))
            return;
          cms.Items.Add((ToolStripItem) new ToolStripSeparator());
          cms.Items.Add(this.Comic.Web, (Image) null, (EventHandler) ((s, e) => Program.StartDocument(this.Comic.Web)));
        })).Show((Control) this.View, location);
      }));
      return rectangle.Width;
    }

    public override Control GetEditControl(int subItem)
    {
      try
      {
        ComicListField tag = (ComicListField) this.View.Columns[subItem].Tag;
        if (!this.Comic.EditMode.CanEditProperties() || string.IsNullOrEmpty(tag.EditProperty))
          return (Control) null;
        CoverViewItem.AddColumnUndo(tag.Description);
        AutoSizeTextBox tb = (AutoSizeTextBox) null;
        this.Comic.RefreshInfoFromFile();
        switch (tag.EditProperty)
        {
          case "AlternateCount":
            tb = new AutoSizeTextBox();
            tb.EnableOnlyNumberKeys();
            EditControlUtility.SetText((Control) tb, this.Comic.AlternateCount);
            break;
          case "AlternateNumber":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((Control) tb, this.Comic.AlternateNumber);
            break;
          case "AlternateSeries":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.AlternateSeries, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.AlternateSeries))));
            break;
          case "BookAge":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.BookAge, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookAgeList()));
            break;
          case "BookCollectionStatus":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.BookCollectionStatus, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookCollectionStatusList()));
            break;
          case "BookCondition":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.BookCondition, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookConditionList()));
            break;
          case "BookLocation":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.BookLocation, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookLocation))));
            break;
          case "BookOwner":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.BookOwner, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookOwner))));
            break;
          case "BookPriceAsText":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.BookPriceAsText, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookPriceAsText))));
            break;
          case "BookStore":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.BookStore, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookStore))));
            break;
          case "Characters":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Characters, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Characters))));
            break;
          case "Colorist":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Colorist, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Colorist))));
            break;
          case "Count":
            tb = new AutoSizeTextBox();
            tb.EnableOnlyNumberKeys();
            EditControlUtility.SetText((Control) tb, this.Comic.ShadowCount);
            break;
          case "CoverArtist":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.CoverArtist, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.CoverArtist))));
            break;
          case "Day":
            tb = new AutoSizeTextBox();
            tb.EnableOnlyNumberKeys();
            EditControlUtility.SetText((Control) tb, this.Comic.Day);
            break;
          case "Editor":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Editor, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Editor))));
            break;
          case "FileName":
            if (this.Comic.IsLinked)
            {
              tb = new AutoSizeTextBox();
              EditControlUtility.SetText((TextBox) tb, this.Comic.FileName, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.FileName))));
              break;
            }
            break;
          case "Format":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.ShadowFormat, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ShadowFormat))));
            break;
          case "Genre":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Genre, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Genre))));
            break;
          case "ISBN":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((Control) tb, this.Comic.ISBN);
            break;
          case "Imprint":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Imprint, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Imprint))));
            break;
          case "Inker":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Inker, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Inker))));
            break;
          case "Letterer":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Letterer, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Letterer))));
            break;
          case "Locations":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Locations, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Locations))));
            break;
          case "MainCharacterOrTeam":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.MainCharacterOrTeam, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.MainCharacterOrTeam))));
            break;
          case "Month":
            tb = new AutoSizeTextBox();
            tb.EnableOnlyNumberKeys();
            EditControlUtility.SetText((Control) tb, this.Comic.Month);
            break;
          case "Number":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((Control) tb, this.Comic.ShadowNumber);
            break;
          case "PagesAsTextSimple":
            if (!this.Comic.IsLinked)
            {
              tb = new AutoSizeTextBox();
              tb.EnableOnlyNumberKeys();
              EditControlUtility.SetText((Control) tb, this.Comic.PagesAsTextSimple);
              break;
            }
            break;
          case "Penciller":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Penciller, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Penciller))));
            break;
          case "Publisher":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Publisher, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Publisher))));
            break;
          case "Review":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Review, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Review))));
            break;
          case "ScanInformation":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.ScanInformation, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ScanInformation))));
            break;
          case "Series":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.ShadowSeries, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ShadowSeries))));
            break;
          case "SeriesGroup":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.SeriesGroup, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.SeriesGroup))));
            break;
          case "StoryArc":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.StoryArc, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.StoryArc))));
            break;
          case "Tags":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Tags, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Tags))));
            break;
          case "Teams":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Teams, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Teams))));
            break;
          case "Title":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.ShadowTitle, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ShadowTitle))));
            break;
          case "Volume":
            tb = new AutoSizeTextBox();
            tb.EnableOnlyNumberKeys();
            EditControlUtility.SetText((Control) tb, this.Comic.ShadowVolume);
            break;
          case "Writer":
            tb = new AutoSizeTextBox();
            EditControlUtility.SetText((TextBox) tb, this.Comic.Writer, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Writer))));
            break;
          case "Year":
            tb = new AutoSizeTextBox();
            tb.EnableOnlyNumberKeys();
            EditControlUtility.SetText((Control) tb, this.Comic.ShadowYear);
            break;
          default:
            if (tag.EditProperty.StartsWith("{"))
            {
              tb = new AutoSizeTextBox();
              EditControlUtility.SetText((Control) tb, this.Comic.GetStringPropertyValue(tag.EditProperty));
              break;
            }
            break;
        }
        if (tb == null)
          return (Control) null;
        TextBoxContextMenu.Register((TextBoxBase) tb);
        tb.AcceptsTab = false;
        tb.Tag = (object) tag.EditProperty;
        tb.MinimumSize = new System.Drawing.Size(40, 10);
        tb.KeyDown += new KeyEventHandler(CoverViewItem.Editor_KeyDown);
        tb.VisibleChanged += new EventHandler(this.Editor_VisibleChanged);
        tb.Multiline = false;
        tb.AutoSizeEnabled = false;
        tb.AutoCompleteMode = AutoCompleteMode.Suggest;
        tb.HandleTab = true;
        return (Control) tb;
      }
      catch
      {
        return (Control) null;
      }
    }

    private void Editor_VisibleChanged(object sender, EventArgs e)
    {
      Control c = sender as Control;
      if (c == null || c.Visible)
        return;
      string name = c.Tag as string;
      if (string.IsNullOrEmpty(name))
        return;
      string text = c.Text.Trim();
      switch (name)
      {
        case "AlternateCount":
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.AlternateCount = EditControlUtility.GetNumber(c)));
          break;
        case "Count":
          if (this.Comic.ShadowCount == EditControlUtility.GetNumber(c))
            break;
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.Count = EditControlUtility.GetNumber(c)));
          break;
        case "Day":
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.Day = EditControlUtility.GetNumber(c)));
          break;
        case "FileName":
          this.Comic.RenameFile(text);
          break;
        case "Month":
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.Month = EditControlUtility.GetNumber(c)));
          break;
        case "Number":
          if (!(this.Comic.ShadowNumber != text))
            break;
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.Number = text));
          break;
        case "Series":
          if (!(this.Comic.ShadowSeries != text))
            break;
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.Series = text));
          break;
        case "Title":
          if (!(this.Comic.ShadowTitle != text))
            break;
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.Title = text));
          break;
        case "Volume":
          if (this.Comic.ShadowVolume == EditControlUtility.GetNumber(c))
            break;
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.Volume = EditControlUtility.GetNumber(c)));
          break;
        case "Year":
          if (this.Comic.ShadowYear == EditControlUtility.GetNumber(c))
            break;
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.Year = EditControlUtility.GetNumber(c)));
          break;
        default:
          if (!name.StartsWith("{"))
          {
            this.SetSelectedComics((Action<ComicBook>) (cb => cb.SetValue(name, (object) text)));
            break;
          }
          name = name.Substring(1, name.Length - 2);
          this.SetSelectedComics((Action<ComicBook>) (cb => cb.SetCustomValue(name, text)));
          break;
      }
    }

    private static void Editor_KeyDown(object sender, KeyEventArgs e)
    {
      Control control = (Control) sender;
      switch (e.KeyCode)
      {
        case Keys.Return:
          e.Handled = true;
          control.Hide();
          break;
        case Keys.Escape:
          e.Handled = true;
          control.Tag = (object) null;
          control.Hide();
          break;
      }
    }

    public ComicBook Comic { get; set; }

    public int LabelLines
    {
      get => this.labelLines;
      set
      {
        if (this.labelLines == value)
          return;
        this.labelLines = value;
        this.Update(true);
      }
    }

    public int Position { get; set; }

    public ThumbnailConfig ThumbnailConfig { get; set; }

    public MarkerType Marker
    {
      get => this.marker;
      set
      {
        if (this.marker == value)
          return;
        this.marker = value;
        this.Update();
      }
    }

    public IGroupInfo CustomGroup { get; set; }

    public IComicBookStatsProvider StatsProvider { get; set; }

    public ComicBookSeriesStatistics SeriesStats
    {
      get
      {
        return this.StatsProvider == null ? (ComicBookSeriesStatistics) null : this.StatsProvider.GetSeriesStats(this.Comic);
      }
    }

    public bool Contains(System.Drawing.Point pt)
    {
      return !this.drawnRect.HasValue || this.drawnRect.Value.Contains(pt);
    }

    public bool IntersectsWith(Rectangle rc)
    {
      return !this.drawnRect.HasValue || this.drawnRect.Value.IntersectsWith(rc);
    }

    protected virtual void OnRefreshComicData()
    {
      if (this.refreshed)
        return;
      Program.QueueManager.AddBookToRefreshComicData(this.Comic);
      this.refreshed = true;
    }

    public static event CoverViewItem.DrawCustomThumbnailOverlayHandler DrawCustomThumbnailOverlay;

    protected void OnDrawCustomThumbnailOverlay(Graphics gr, Rectangle bounds)
    {
      if (CoverViewItem.DrawCustomThumbnailOverlay == null)
        return;
      using (gr.SaveState())
      {
        gr.SetClip(bounds, CombineMode.Intersect);
        gr.TranslateTransform((float) bounds.X, (float) bounds.Y);
        Rectangle bounds1 = new Rectangle(0, 0, bounds.Width + 1, bounds.Height + 1);
        int mask = BitUtility.CreateMask(this.Hot, this.Selected, this.View.IsStack((IViewableItem) this));
        CoverViewItem.DrawCustomThumbnailOverlay(this.Comic, gr, bounds1, mask);
      }
    }

    public static CoverThumbnailSizing ThumbnailSizing { get; set; }

    [Flags]
    private enum ItemGapType
    {
      Undefined = 4096, // 0x00001000
      None = 0,
      Start = 1,
      End = 2,
    }

    public delegate void DrawCustomThumbnailOverlayHandler(
      ComicBook comic,
      Graphics gr,
      Rectangle bounds,
      int flags);
  }
}

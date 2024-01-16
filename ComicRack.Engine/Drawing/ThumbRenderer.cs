// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ThumbRenderer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  public class ThumbRenderer
  {
    public static float DefaultStateOverlap = 0.8f;
    public static Bitmap DefaultRatingImage1 = new Bitmap(16, 16);
    public static Bitmap DefaultRatingImage2 = new Bitmap(16, 16);
    public static Bitmap DefaultTagRatingImage1 = new Bitmap(16, 16);
    public static Bitmap DefaultTagRatingImage2 = new Bitmap(16, 16);
    public static Image DefaultPageCurlImage = (Image) Resources.PageCurl;
    public static Image DefaultPageCurlShadowImage = (Image) Resources.PageCurlShadow.ToOptimized();
    public static float DefaultThumbnailAspect = 0.6666667f;
    public static Color[] DefaultBookmarkColors = new Color[4]
    {
      Color.Orange,
      Color.Green,
      Color.Red,
      Color.Blue
    };
    public static Bitmap[] DefaultNewPagesImages = new Bitmap[6]
    {
      Resources.NewPages1.ToOptimized(),
      Resources.NewPages2.ToOptimized(),
      Resources.NewPages3.ToOptimized(),
      Resources.NewPages4.ToOptimized(),
      Resources.NewPages5.ToOptimized(),
      Resources.NewPages5Plus.ToOptimized()
    };
    private Color selectionBackColor = SystemColors.Highlight;
    private List<Image> stateImages;
    private Rectangle thumbnailBounds = Rectangle.Empty;
    private RatingRenderer ratingStripRenderer;
    private static readonly Bitmap pageBowLeft = PageRendering.CreatePageBow(new System.Drawing.Size(32, 9), 180f).ToOptimized();
    private static readonly Bitmap pageBowRight = PageRendering.CreatePageBow(new System.Drawing.Size(32, 9), 0.0f).ToOptimized();
    private static readonly string loadingThumbnail = TR.Messages[nameof (loadingThumbnail), "Loading Thumbnail..."];
    private static Color cachedPageCurlColor;
    private static Image cachedPageCurl;
    private static Image cachedPageCurlShadow;
    private static Bitmap coloredPageCurl;

    public ThumbRenderer()
    {
      this.ImageOpacity = 1f;
      this.PageNumberAlignment = ContentAlignment.TopRight;
      this.MissingBackColor = Color.White;
      this.ComicCount = 1;
      this.RatingImage1 = (Image) ThumbRenderer.DefaultRatingImage1;
      this.RatingImage2 = (Image) ThumbRenderer.DefaultRatingImage2;
      this.TagRatingImage1 = (Image) ThumbRenderer.DefaultTagRatingImage1;
      this.TagRatingImage2 = (Image) ThumbRenderer.DefaultTagRatingImage2;
      this.StateOverlap = ThumbRenderer.DefaultStateOverlap;
      this.BookmarkColors = EngineConfiguration.Default.BookmarkColors;
      if (this.BookmarkColors != null && this.BookmarkColors.Length != 0)
        return;
      this.BookmarkColors = ThumbRenderer.DefaultBookmarkColors;
    }

    public ThumbRenderer(Image image, ThumbnailDrawingOptions flags)
      : this()
    {
      this.Image = image;
      this.Options = flags;
    }

    public Color SelectionBackColor
    {
      get => this.selectionBackColor;
      set => this.selectionBackColor = value;
    }

    public ThumbnailDrawingOptions Options { get; set; }

    public Image Image { get; set; }

    public Image BackImage { get; set; }

    public Color MissingBackColor { get; set; }

    public float Rating1 { get; set; }

    public float Rating2 { get; set; }

    public ThumbnailRatingMode RatingMode { get; set; }

    public int ComicCount { get; set; }

    public int PageCount { get; set; }

    public bool BookmarkPercentMode { get; set; }

    public int[] Bookmarks { get; set; }

    public Color[] BookmarkColors { get; set; }

    public Image RatingImage1 { get; set; }

    public Image RatingImage2 { get; set; }

    public Image TagRatingImage1 { get; set; }

    public Image TagRatingImage2 { get; set; }

    public int PageNumber { get; set; }

    public ContentAlignment PageNumberAlignment { get; set; }

    public float ImageOpacity { get; set; }

    public float StateOverlap { get; set; }

    public List<Image> StateImages
    {
      get
      {
        if (this.stateImages == null)
          this.stateImages = new List<Image>();
        return this.stateImages;
      }
    }

    public Rectangle ThumbnailBounds => this.thumbnailBounds;

    public RatingRenderer RatingStripRenderer => this.ratingStripRenderer;

    public StyledRenderer.AlphaStyle SelectionAlphaState
    {
      get => StyledRenderer.GetAlphaStyle(this.Selected, this.Hot, this.Focused);
    }

    public bool FastModeEnabled => (this.Options & ThumbnailDrawingOptions.FastMode) != 0;

    public bool BackImageEnabled => (this.Options & ThumbnailDrawingOptions.EnableBackImage) != 0;

    public bool BackgroundEnabled => (this.Options & ThumbnailDrawingOptions.EnableBackground) != 0;

    public bool ShadowEnabled => (this.Options & ThumbnailDrawingOptions.EnableShadow) != 0;

    public bool BorderEnabled => (this.Options & ThumbnailDrawingOptions.EnableBorder) != 0;

    public bool BowShadowEnabled
    {
      get
      {
        return (this.Options & ThumbnailDrawingOptions.EnableBowShadow) != ThumbnailDrawingOptions.None && EngineConfiguration.Default.ThumbnailPageBow;
      }
    }

    public bool RatingEnabled
    {
      get => (this.Options & ThumbnailDrawingOptions.EnableRating) != 0;
      set
      {
        this.Options = this.Options.SetMask<ThumbnailDrawingOptions>(ThumbnailDrawingOptions.EnableRating, value);
      }
    }

    public bool StatesEnabled
    {
      get => (this.Options & ThumbnailDrawingOptions.EnableStates) != 0;
      set
      {
        this.Options = this.Options.SetMask<ThumbnailDrawingOptions>(ThumbnailDrawingOptions.EnableStates, value);
      }
    }

    public bool VerticalBookmarksEnabled
    {
      get => (this.Options & ThumbnailDrawingOptions.EnableVerticalBookmarks) != 0;
    }

    public bool HorizontalBookmarksEnabled
    {
      get => (this.Options & ThumbnailDrawingOptions.EnableHorizontalBookmarks) != 0;
    }

    public bool PageNumberEnabled => (this.Options & ThumbnailDrawingOptions.EnablePageNumber) != 0;

    public bool KeepAspect => (this.Options & ThumbnailDrawingOptions.KeepAspect) != 0;

    public bool FillAspect => (this.Options & ThumbnailDrawingOptions.AspectFill) != 0;

    public bool Selected => (this.Options & ThumbnailDrawingOptions.Selected) != 0;

    public bool Hot => (this.Options & ThumbnailDrawingOptions.Hot) != 0;

    public bool Focused => (this.Options & ThumbnailDrawingOptions.Focused) != 0;

    public bool Bookmarked
    {
      get => (this.Options & ThumbnailDrawingOptions.Bookmarked) != 0;
      set
      {
        this.Options = value ? this.Options | ThumbnailDrawingOptions.Bookmarked : this.Options & ~ThumbnailDrawingOptions.Bookmarked;
      }
    }

    public bool MissingThumbnailDisabled
    {
      get => (this.Options & ThumbnailDrawingOptions.DisableMissingThumbnail) != 0;
    }

    public void DrawPageNumber(
      Graphics graphics,
      Rectangle thumbnailBounds,
      ContentAlignment align)
    {
      if (!this.PageNumberEnabled)
        return;
      thumbnailBounds.Inflate(-2, -2);
      string str = this.PageNumber.ToString();
      Font font = FC.Get("Arial", 7f);
      Rectangle rectangle1 = new Rectangle(System.Drawing.Point.Empty, graphics.MeasureString(str, font).ToSize());
      rectangle1.Width = Math.Max(rectangle1.Width, 20);
      rectangle1.Inflate(2, 2);
      Rectangle rectangle2 = rectangle1.Align(thumbnailBounds, align);
      using (graphics.AntiAlias())
      {
        using (GraphicsPath path = rectangle2.ConvertToPath(3, 3))
        {
          using (StringFormat format = new StringFormat()
          {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
          })
          {
            using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb(192, Color.Black)))
              graphics.FillPath((Brush) solidBrush, path);
            graphics.DrawString(str, font, Brushes.White, (RectangleF) rectangle2, format);
            graphics.DrawPath(Pens.Black, path);
          }
        }
      }
    }

    public void DrawPageNumber(Graphics graphics, Rectangle thumbnailBounds)
    {
      this.DrawPageNumber(graphics, thumbnailBounds, this.PageNumberAlignment);
    }

    public static void DrawRatingStrip(
      Graphics graphics,
      Rectangle thumbnailBounds,
      Image image,
      float rating,
      PointF offset,
      int height = -1)
    {
      if ((double) rating <= 0.0)
        return;
      System.Drawing.Point location = thumbnailBounds.Location;
      location.Offset(2, 2);
      if (height == -1)
        height = ThumbRenderer.GetOverlaysHeight(thumbnailBounds);
      int width = height / 5;
      location.Offset((int) ((double) width * (double) offset.X), (int) ((double) height * (double) offset.Y));
      new RatingRenderer(image, new Rectangle(location, new System.Drawing.Size(width, height)), vertical: true).DrawRatingStrip(graphics, rating);
    }

    public static int GetOverlaysHeight(Rectangle thumbnailBounds)
    {
      return (thumbnailBounds.Height / 10).Clamp(16, 32);
    }

    public static int GetTagHeight(Rectangle thumbnailBounds)
    {
      return (thumbnailBounds.Height / 5).Clamp(16, 64);
    }

    public bool HasStateOverlay
    {
      get => this.StatesEnabled && this.stateImages != null && this.stateImages.Count > 0;
    }

    public bool HasTagRatingOverlay
    {
      get
      {
        if (!this.RatingEnabled || this.RatingMode != ThumbnailRatingMode.Tags)
          return false;
        return (double) this.Rating1 > 0.0 || (double) this.Rating2 > 0.0;
      }
    }

    public static int DrawTagRating(
      Graphics graphics,
      Rectangle thumbnailBounds,
      Image image,
      float rating,
      int height = -1)
    {
      if ((double) rating <= 0.0)
        return 0;
      System.Drawing.Point location = thumbnailBounds.BottomRight();
      if (height == -1)
        height = ThumbRenderer.GetTagHeight(thumbnailBounds);
      location.Offset(-height, -height);
      Rectangle bounds = new Rectangle(location, new System.Drawing.Size(height, height));
      new RatingRenderer(image, bounds).DrawRatingTag(graphics, rating);
      return bounds.Width;
    }

    public int DrawRating(Graphics graphics, Rectangle thumbnailBounds, int height = -1)
    {
      if (!this.RatingEnabled || this.RatingMode == ThumbnailRatingMode.StarsBelow)
        return 0;
      using (graphics.Fast(this.FastModeEnabled))
      {
        switch (this.RatingMode)
        {
          case ThumbnailRatingMode.Tags:
            int num = ThumbRenderer.DrawTagRating(graphics, thumbnailBounds, this.TagRatingImage2, this.Rating2, height);
            thumbnailBounds.Width -= num;
            return num + ThumbRenderer.DrawTagRating(graphics, thumbnailBounds, this.TagRatingImage1, this.Rating1, height);
          case ThumbnailRatingMode.StarsOverlay:
            ThumbRenderer.DrawRatingStrip(graphics, thumbnailBounds, this.RatingImage2, this.Rating2, PointF.Empty, height);
            ThumbRenderer.DrawRatingStrip(graphics, thumbnailBounds, this.RatingImage1, this.Rating1, new PointF(0.25f, 0.0f), height);
            return thumbnailBounds.Width;
          default:
            return 0;
        }
      }
    }

    public int DrawStateImage(Graphics graphics, Rectangle thumbnailBounds)
    {
      if (!this.HasStateOverlay)
        return 0;
      int top = thumbnailBounds.Height - ThumbRenderer.GetOverlaysHeight(thumbnailBounds);
      using (graphics.Fast(this.FastModeEnabled))
        return ThumbRenderer.DrawImageList(graphics, (IEnumerable<Image>) this.stateImages, thumbnailBounds.Pad(0, top), ContentAlignment.MiddleLeft, this.StateOverlap);
    }

    public void DrawBookmarks(Graphics graphics, Rectangle thumbnailBounds)
    {
      int num = this.BookmarkPercentMode ? 100 : this.PageCount;
      if (this.Bookmarks == null || num == 0 || !this.HorizontalBookmarksEnabled && !this.VerticalBookmarksEnabled)
        return;
      for (int index = 0; index < this.Bookmarks.Length; ++index)
      {
        try
        {
          float bookmark = (float) this.Bookmarks[index];
          Color bookmarkColor = this.BookmarkColors[index % this.BookmarkColors.Length];
          Color col2 = Color.FromArgb((int) bookmarkColor.R / 2, (int) bookmarkColor.G / 2, (int) bookmarkColor.B / 2);
          if (this.VerticalBookmarksEnabled)
            ThumbRenderer.DrawBookmarkV(graphics, thumbnailBounds, bookmark / (float) num, bookmarkColor, col2, true);
          if (this.HorizontalBookmarksEnabled)
            ThumbRenderer.DrawBookmarkH(graphics, thumbnailBounds, bookmark / (float) num, bookmarkColor, col2, true);
        }
        catch
        {
        }
      }
    }

    public void DrawThumbnailOverlays(Graphics graphics, Rectangle thumbnailBounds)
    {
      this.DrawPageNumber(graphics, thumbnailBounds);
      this.DrawBookmarks(graphics, thumbnailBounds);
      if (this.Bookmarked)
      {
        Color red = Color.Red;
        Color col2 = Color.FromArgb((int) red.R / 2, (int) red.G / 2, (int) red.B / 2);
        ThumbRenderer.DrawBookmarkH(graphics, thumbnailBounds, 100f, red, col2, true);
      }
      this.DrawStateImage(graphics, thumbnailBounds);
      this.DrawRating(graphics, thumbnailBounds);
    }

    private static void DrawShadow(Graphics graphics, Rectangle bounds, int width)
    {
      bounds.Width += width;
      bounds.Height += width;
      graphics.DrawShadow(bounds, width, Color.Black, 0.33f, BlurShadowType.Outside, BlurShadowParts.Default);
    }

    private void DrawStack(
      Graphics graphics,
      Rectangle thumbnailBounds,
      int stackRotation,
      int shadowSize)
    {
      using (graphics.SaveState())
      {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TranslateTransform((float) (thumbnailBounds.Left + thumbnailBounds.Width / 2), (float) (thumbnailBounds.Top + thumbnailBounds.Height / 2));
        graphics.RotateTransform((float) stackRotation);
        graphics.TranslateTransform((float) (-thumbnailBounds.Left - thumbnailBounds.Width / 2), (float) (-thumbnailBounds.Top - thumbnailBounds.Height / 2));
        if (this.ShadowEnabled)
          ThumbRenderer.DrawShadow(graphics, thumbnailBounds, shadowSize);
        graphics.FillRectangle(SystemBrushes.Control, thumbnailBounds);
        graphics.DrawRectangle(Pens.Black, thumbnailBounds);
      }
    }

    public Rectangle DrawThumbnail(Graphics graphics, Rectangle thumbnailBounds)
    {
      bool flag1 = this.Options.HasFlag((Enum) ThumbnailDrawingOptions.Stacked);
      bool flag2 = this.Options.HasFlag((Enum) ThumbnailDrawingOptions.NoOpaqueCover);
      Rectangle rect = thumbnailBounds;
      RatingRenderer ratingRenderer1 = (RatingRenderer) null;
      RatingRenderer ratingRenderer2 = (RatingRenderer) null;
      if (this.RatingEnabled && this.RatingMode == ThumbnailRatingMode.StarsBelow)
      {
        int overlaysHeight = ThumbRenderer.GetOverlaysHeight(thumbnailBounds);
        Rectangle bounds = new Rectangle(0, 0, thumbnailBounds.Width - 4, overlaysHeight - 4);
        ratingRenderer1 = new RatingRenderer(this.RatingImage1, bounds)
        {
          Fast = this.FastModeEnabled
        };
        ratingRenderer2 = new RatingRenderer(this.RatingImage2, bounds)
        {
          Fast = this.FastModeEnabled
        };
        int height = (int) ratingRenderer1.GetRenderSize().Height;
        ratingRenderer1.X = ratingRenderer2.X = thumbnailBounds.Left + 2;
        ratingRenderer1.Height = ratingRenderer2.Height = height;
        thumbnailBounds = thumbnailBounds.Fit(thumbnailBounds.Pad(0, 0, bottom: height + 4));
      }
      if (this.KeepAspect && this.Image != null)
        thumbnailBounds = this.Image.Size.ToRectangle(thumbnailBounds);
      if (this.ShadowEnabled)
      {
        thumbnailBounds.Width -= 4;
        thumbnailBounds.Height -= 4;
      }
      if (flag1)
        thumbnailBounds.Inflate(-8, -8);
      if (!graphics.IsVisible(rect))
      {
        this.thumbnailBounds = thumbnailBounds;
        return thumbnailBounds;
      }
      using (graphics.Fast(this.FastModeEnabled))
      {
        if (flag1)
        {
          if (this.ComicCount > 2)
            this.DrawStack(graphics, thumbnailBounds, -4, 4);
          if (this.ComicCount > 1)
            this.DrawStack(graphics, thumbnailBounds, 2, 4);
        }
        if (this.ShadowEnabled)
          ThumbRenderer.DrawShadow(graphics, thumbnailBounds, 4);
        if (!this.MissingThumbnailDisabled && (this.Image == null || (double) this.ImageOpacity < 0.949999988079071))
          ThumbRenderer.DrawMissingThumbnail(graphics, thumbnailBounds, this.MissingBackColor);
        if (this.Image != null)
        {
          if ((double) this.ImageOpacity > 0.949999988079071 && !flag2)
            graphics.CompositingMode = CompositingMode.SourceCopy;
          if (this.KeepAspect)
          {
            graphics.DrawImage(this.Image, thumbnailBounds, this.ImageOpacity);
          }
          else
          {
            Rectangle rectangle = this.Image.Size.ToRectangle();
            if (this.Image.Width > this.Image.Height && thumbnailBounds.Height > thumbnailBounds.Width)
            {
              int width = rectangle.Width;
              rectangle.Width = Math.Min(width, rectangle.Height * thumbnailBounds.Width / thumbnailBounds.Height);
              rectangle.X = width - rectangle.Width;
            }
            graphics.DrawImage(this.Image, thumbnailBounds, rectangle, this.ImageOpacity);
          }
          graphics.CompositingMode = CompositingMode.SourceOver;
          if (this.BowShadowEnabled)
          {
            Rectangle bounds = thumbnailBounds;
            bounds.Width /= 10;
            graphics.DrawImage((Image) ThumbRenderer.pageBowLeft, bounds, new Rectangle(0, 0, 32, 8), this.ImageOpacity);
            bounds.X = thumbnailBounds.Right - bounds.Width;
            graphics.DrawImage((Image) ThumbRenderer.pageBowRight, bounds, new Rectangle(0, 0, 32, 8), this.ImageOpacity);
          }
          if (this.BackImageEnabled)
          {
            if (this.Selected)
              ThumbRenderer.DrawPageCurlOverlay(graphics, this.BackImage, thumbnailBounds);
            else if (this.Hot)
              ThumbRenderer.DrawPageCurlOverlay(graphics, this.BackImage, thumbnailBounds, 0.5f);
          }
          else
            graphics.DrawStyledRectangle(thumbnailBounds, this.SelectionAlphaState, this.SelectionBackColor, StyledRenderer.Default.Frame(0, 0));
        }
      }
      if (this.BorderEnabled)
        graphics.DrawRectangle(Pens.Black, thumbnailBounds);
      this.DrawThumbnailOverlays(graphics, thumbnailBounds);
      if (ratingRenderer1 == null)
      {
        this.ratingStripRenderer = (RatingRenderer) null;
      }
      else
      {
        ratingRenderer1.Y = thumbnailBounds.Bottom + 4;
        ratingRenderer2.Y = ratingRenderer1.Y + 2;
        ratingRenderer2.DrawRatingStrip(graphics, this.Rating2, alpha2: (double) this.Rating2 > 0.0 ? 0.25f : 0.0f);
        ratingRenderer1.DrawRatingStrip(graphics, this.Rating1, alpha2: (double) this.Rating2 > 0.0 ? 0.0f : 0.25f);
        this.ratingStripRenderer = ratingRenderer1;
      }
      this.thumbnailBounds = thumbnailBounds;
      return thumbnailBounds;
    }

    private static GraphicsPath GetBookmark(Rectangle rect)
    {
      GraphicsPath bookmark = new GraphicsPath();
      bookmark.AddLines(new System.Drawing.Point[5]
      {
        rect.Location,
        new System.Drawing.Point(rect.Right, rect.Top),
        new System.Drawing.Point(rect.Right, rect.Bottom),
        new System.Drawing.Point(rect.Left, rect.Bottom),
        new System.Drawing.Point(rect.Left + rect.Height, rect.Top + rect.Height / 2)
      });
      bookmark.CloseFigure();
      return bookmark;
    }

    private static void DrawBookmarkV(
      Graphics gr,
      Rectangle tr,
      float percent,
      Color col1,
      Color col2,
      bool withShadow)
    {
      int height = Math.Min(8, tr.Height - 2);
      int width = Math.Min(16, tr.Width - 2);
      int num = tr.Height - height;
      int y = tr.Top + (int) ((double) num * (double) percent);
      Rectangle rect = new Rectangle(tr.Right - width, y, width, height);
      using (gr.SaveState())
      {
        gr.SmoothingMode = SmoothingMode.AntiAlias;
        using (GraphicsPath bookmark = ThumbRenderer.GetBookmark(rect))
        {
          if (withShadow)
          {
            gr.TranslateTransform(1f, 1f);
            gr.FillPath(Brushes.Black, bookmark);
          }
          gr.TranslateTransform(-1f, -1f);
          using (Brush brush = (Brush) new LinearGradientBrush(rect, col1, col2, 90f))
            gr.FillPath(brush, bookmark);
          gr.DrawPath(Pens.Black, bookmark);
        }
      }
    }

    private static void DrawBookmarkH(
      Graphics gr,
      Rectangle tr,
      float percent,
      Color col1,
      Color col2,
      bool withShadow)
    {
      using (gr.SaveState())
      {
        gr.RotateTransform(-90f);
        using (System.Drawing.Drawing2D.Matrix rotationMatrix = MatrixUtility.GetRotationMatrix(System.Drawing.Point.Empty, 90))
        {
          Rectangle tr1 = tr.Rotate(rotationMatrix);
          ThumbRenderer.DrawBookmarkV(gr, tr1, percent, col1, col2, withShadow);
        }
      }
    }

    public static System.Drawing.Size GetSafeScaledImageSize(Image image, System.Drawing.Size canvasSize)
    {
      return ThumbRenderer.GetSafeScaledImageSize(image, canvasSize, ThumbRenderer.DefaultThumbnailAspect);
    }

    public static System.Drawing.Size GetSafeScaledImageSize(
      Image image,
      System.Drawing.Size canvasSize,
      float defaultAspect)
    {
      return ThumbRenderer.GetSafeScaledImageSize(image != null ? image.Size : System.Drawing.Size.Empty, canvasSize, defaultAspect);
    }

    public static System.Drawing.Size GetSafeScaledImageSize(
      System.Drawing.Size imageSize,
      System.Drawing.Size canvasSize,
      float defaultAspect)
    {
      return !imageSize.IsEmpty ? imageSize.ToRectangle(canvasSize, RectangleScaleMode.None).Size : new System.Drawing.Size((int) ((double) canvasSize.Height * (double) defaultAspect), canvasSize.Height);
    }

    public static System.Drawing.Size GetSafeScaledImageSize(System.Drawing.Size imageSize, System.Drawing.Size canvasSize)
    {
      return ThumbRenderer.GetSafeScaledImageSize(imageSize, canvasSize, ThumbRenderer.DefaultThumbnailAspect);
    }

    public static void DrawMissingThumbnail(Graphics graphics, Rectangle bounds, Color backColor)
    {
      using (Brush brush = (Brush) new SolidBrush(backColor))
        graphics.FillRectangle(brush, bounds);
      Rectangle layoutRectangle = bounds;
      layoutRectangle.Inflate(-4, -4);
      using (StringFormat format = new StringFormat()
      {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center
      })
        graphics.DrawString(ThumbRenderer.loadingThumbnail, FC.Get("Arial", 6f), Brushes.LightGray, (RectangleF) layoutRectangle, format);
    }

    public static Rectangle DrawThumbnail(
      Graphics graphics,
      Image image,
      Rectangle bounds,
      ThumbnailDrawingOptions flags,
      ComicBook comicBook,
      float opacity = 1f)
    {
      ThumbRenderer thumbRenderer = new ThumbRenderer(image, flags);
      if (comicBook != null)
      {
        thumbRenderer.PageCount = comicBook.PageCount;
        thumbRenderer.Rating1 = comicBook.Rating;
        thumbRenderer.Rating2 = comicBook.CommunityRating;
        thumbRenderer.Bookmarks = new int[2]
        {
          comicBook.CurrentPage,
          comicBook.LastPageRead
        };
      }
      thumbRenderer.ImageOpacity = opacity;
      return thumbRenderer.DrawThumbnail(graphics, bounds);
    }

    public static void DrawPageCurlOverlay(
      Graphics graphics,
      Image uncoveredImage,
      Image pageCurl,
      Image pageCurlShadow,
      Rectangle rc,
      float width,
      Color pageCurlColor)
    {
      if (uncoveredImage == null || rc.IsEmpty)
        return;
      float num = (float) rc.Height / (float) uncoveredImage.Height;
      RectangleF rect1 = new RectangleF((float) rc.Left, (float) rc.Top, num * (float) uncoveredImage.Width, (float) rc.Height);
      int width1 = rc.Width;
      if (rc.Width > rc.Height)
        width1 /= 2;
      if ((double) rect1.Width < (double) width1)
        width1 = (int) rect1.Width;
      rc = rc.Pad(rc.Width - width1, 0);
      float width2 = (float) Math.Min(rc.Width, rc.Height) * width;
      float height = width2;
      rect1.X = (float) rc.Right - rect1.Width;
      using (GraphicsPath path = new GraphicsPath())
      {
        path.AddPolygon(new PointF[3]
        {
          new PointF((float) rc.Right, (float) rc.Bottom - height),
          new PointF((float) rc.Right, (float) rc.Bottom),
          new PointF((float) rc.Right - width2, (float) rc.Bottom)
        });
        RectangleF rect2 = new RectangleF((float) rc.Right - width2, (float) rc.Bottom - height, width2, height);
        using (graphics.SaveState())
        {
          using (Region region = new Region(path))
          {
            graphics.IntersectClip(region);
            graphics.DrawImage(uncoveredImage, rect1);
          }
        }
        if (ThumbRenderer.coloredPageCurl == null || ThumbRenderer.cachedPageCurlColor != pageCurlColor || ThumbRenderer.cachedPageCurl != pageCurl || ThumbRenderer.cachedPageCurlShadow != pageCurlShadow)
        {
          Bitmap bitmap = new Bitmap(pageCurl);
          if (!pageCurlColor.IsEmpty)
          {
            bitmap.ToGrayScale();
            bitmap.Colorize(pageCurlColor);
          }
          ThumbRenderer.coloredPageCurl = bitmap.ToOptimized();
          ThumbRenderer.cachedPageCurl = pageCurl;
          ThumbRenderer.cachedPageCurlShadow = pageCurlShadow;
          ThumbRenderer.cachedPageCurlColor = pageCurlColor;
        }
        graphics.DrawImage(pageCurlShadow, rect2);
        graphics.DrawImage((Image) ThumbRenderer.coloredPageCurl, rect2);
      }
    }

    public static void DrawPageCurlOverlay(
      Graphics graphics,
      Image uncoveredImage,
      Rectangle rc,
      float width = 0.9f)
    {
      ThumbRenderer.DrawPageCurlOverlay(graphics, uncoveredImage, ThumbRenderer.DefaultPageCurlImage, ThumbRenderer.DefaultPageCurlShadowImage, rc, width, EngineConfiguration.Default.ThumbnailPageCurlColor);
    }

    public static int DrawImageList(
      Graphics graphics,
      IEnumerable<Image> images,
      Rectangle bounds,
      ContentAlignment alignment,
      float overlapPercent = 0.0f,
      bool allowOversize = true)
    {
      if (images.Count<Image>() == 0)
        return 0;
      IEnumerable<\u003C\u003Ef__AnonymousType1<System.Drawing.Size, Image>> source = images.Select(img => new
      {
        Size = img.Size.Scale(img.Size.GetScale(bounds.Size, allowOversize: allowOversize)),
        Image = img
      });
      Func<int, int> getWidth = (Func<int, int>) (w => w - (int) ((double) overlapPercent * (double) w));
      int num1 = source.Max(s => s.Size.Height);
      int num2 = source.Last().Size.Width + source.Reverse().Skip(1).Sum(s => getWidth(s.Size.Width));
      float scale = Math.Min(1f, (float) bounds.Width / (float) num2);
      Rectangle rectangle = new Rectangle(0, 0, (int) ((double) scale * (double) num2), (int) ((double) scale * (double) num1)).Align(bounds, alignment);
      int left = rectangle.Left;
      int top = rectangle.Top;
      foreach (var data in source)
      {
        System.Drawing.Size size = data.Size.Scale(scale);
        int num3 = (rectangle.Height - size.Height) / 2;
        graphics.DrawImage(data.Image, left, top + num3, size.Width, size.Height);
        left += getWidth(size.Width);
      }
      return rectangle.Width;
    }

    public static Bitmap GetNewPageStatusImage(int newPages)
    {
      return newPages <= 0 ? (Bitmap) null : ThumbRenderer.DefaultNewPagesImages[(newPages - 1).Clamp(0, ThumbRenderer.DefaultNewPagesImages.Length - 1)];
    }
  }
}

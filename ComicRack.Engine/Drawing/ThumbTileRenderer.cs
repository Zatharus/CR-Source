// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ThumbTileRenderer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  public class ThumbTileRenderer : ViewItemRenderer
  {
    private static readonly Font DefaultFont = SystemFonts.IconTitleFont;
    private Font font = ThumbTileRenderer.DefaultFont;

    public ThumbTileRenderer(Image image, ThumbnailDrawingOptions flags)
    {
      this.Image = image;
      this.Options = flags;
    }

    public Font Font
    {
      get => this.font;
      set => this.font = value;
    }

    public bool ThreeD { get; set; }

    public IEnumerable<Image> Icons { get; set; }

    public int ScrollOffset { get; set; }

    public void DrawBackground(Graphics graphics, Rectangle tileBounds)
    {
      if (this.SelectionAlphaState == StyledRenderer.AlphaStyle.None)
        graphics.DrawStyledRectangle(tileBounds, 128, this.BackColor, StyledRenderer.Default.NoGradient());
      else
        graphics.DrawStyledRectangle(tileBounds, this.SelectionAlphaState, this.SelectionBackColor);
    }

    public void DrawTile(Graphics graphics, Rectangle tileBounds)
    {
      if (!graphics.IsVisible(tileBounds))
        return;
      ref Rectangle local = ref tileBounds;
      System.Drawing.Size border = this.Border;
      int width = -border.Width;
      border = this.Border;
      int height = -border.Height;
      local.Inflate(width, height);
      if (this.BackgroundEnabled)
      {
        this.DrawBackground(graphics, tileBounds);
        tileBounds.Inflate(-4, -4);
      }
      ThumbnailDrawingOptions options = this.Options;
      if (this.RatingMode == ThumbnailRatingMode.Tags)
        this.RatingEnabled = false;
      Rectangle rectangle1;
      if (this.ThreeD)
      {
        rectangle1 = tileBounds.Pad(tileBounds.Width / 3, 0);
      }
      else
      {
        System.Drawing.Size safeScaledImageSize = ThumbRenderer.GetSafeScaledImageSize(this.Image, new System.Drawing.Size(tileBounds.Width / 2, tileBounds.Height));
        rectangle1 = new Rectangle(tileBounds.X + safeScaledImageSize.Width + 4, tileBounds.Y, tileBounds.Width - safeScaledImageSize.Width - 4, tileBounds.Height);
        this.DrawThumbnail(graphics, new Rectangle(tileBounds.Location, safeScaledImageSize));
      }
      this.Options = options;
      Rectangle rectangle2 = rectangle1;
      int specialTagsHeight = this.GetSpecialTagsHeight(tileBounds);
      bool flag = this.Icons != null && this.Icons.Count<Image>() > 0;
      if (((this.HasTagRatingOverlay ? 1 : (this.HasStateOverlay ? 1 : 0)) | (flag ? 1 : 0)) != 0)
        rectangle2.Height -= (int) ((double) specialTagsHeight * 1.2000000476837158);
      if (this.ThreeD)
        this.DrawEmbossedText(graphics, (IEnumerable<TextLine>) this.TextLines, rectangle2);
      else
        SimpleTextRenderer.DrawText(graphics, (IEnumerable<TextLine>) this.TextLines, rectangle2);
      if (this.RatingMode == ThumbnailRatingMode.Tags)
        rectangle1.Width -= this.DrawRating(graphics, rectangle1, specialTagsHeight);
      if (flag)
      {
        using (this.FastModeEnabled ? graphics.Fast() : graphics.HighQuality(true))
        {
          int top = rectangle1.Height - specialTagsHeight;
          ThumbRenderer.DrawImageList(graphics, this.Icons, rectangle1.Pad(0, top), this.ThreeD ? ContentAlignment.BottomRight : ContentAlignment.BottomLeft, -0.1f);
        }
      }
      if (!this.ThreeD)
        return;
      this.Draw3DComic(graphics, tileBounds);
    }

    private void Draw3DComic(Graphics graphics, Rectangle tileBounds)
    {
      if (this.Image == null || !(this.Image is Bitmap))
        return;
      Bitmap image = this.Image as Bitmap;
      System.Drawing.Size size = new System.Drawing.Size(512, 512);
      try
      {
        using (Bitmap defaultBook = ComicBox3D.CreateDefaultBook(image, (Bitmap) null, size, this.PageCount))
        {
          Rectangle rect = defaultBook.Size.ToRectangle().Scale(defaultBook.Size.GetScale(tileBounds.Size));
          graphics.DrawImage((Image) defaultBook, rect);
        }
      }
      catch (Exception ex)
      {
      }
    }

    private void DrawEmbossedText(Graphics graphics, IEnumerable<TextLine> lines, Rectangle bounds)
    {
      this.TextLines.ForEach((Action<TextLine>) (tl => tl.Format.Alignment = StringAlignment.Far));
      bounds.Offset(1, 1);
      this.TextLines.ForEach((Action<TextLine>) (tl => tl.ForeColor = Color.White));
      SimpleTextRenderer.DrawText(graphics, (IEnumerable<TextLine>) this.TextLines, bounds);
      bounds.Offset(-1, -1);
      this.TextLines.ForEach((Action<TextLine>) (tl => tl.ForeColor = Color.Black));
      this.TextLines.Where<TextLine>((Func<TextLine, bool>) (tl => tl.Font != null && !tl.Font.Bold)).ForEach<TextLine>((Action<TextLine>) (tl => tl.ForeColor = Color.Gray));
      SimpleTextRenderer.DrawText(graphics, (IEnumerable<TextLine>) this.TextLines, bounds);
    }

    private int GetSpecialTagsHeight(Rectangle tileBounds)
    {
      return ThumbRenderer.GetTagHeight(tileBounds);
    }

    public static void DrawTile(
      Graphics graphics,
      Rectangle bounds,
      Image image,
      ComicBook comicBook,
      int page,
      Font font,
      Color foreColor,
      Color backColor,
      ThumbnailDrawingOptions options,
      ComicTextElements elements,
      bool threeD,
      IEnumerable<Image> icons = null)
    {
      ThumbTileRenderer thumbTileRenderer1 = new ThumbTileRenderer(image, options);
      thumbTileRenderer1.ForeColor = foreColor;
      thumbTileRenderer1.BackColor = backColor;
      thumbTileRenderer1.ThreeD = threeD;
      ThumbTileRenderer thumbTileRenderer2 = thumbTileRenderer1;
      if (comicBook == null)
        return;
      thumbTileRenderer2.PageCount = comicBook.PageCount;
      thumbTileRenderer2.Rating1 = comicBook.Rating;
      thumbTileRenderer2.Rating2 = comicBook.CommunityRating;
      thumbTileRenderer2.Bookmarks = new int[2]
      {
        comicBook.CurrentPage,
        comicBook.LastPageRead
      };
      thumbTileRenderer2.Icons = icons;
      if (page >= 0)
      {
        thumbTileRenderer2.PageNumber = page;
        thumbTileRenderer2.TextLines.AddRange(ComicTextBuilder.GetTextBlocks(comicBook.GetPage(page), page, font, foreColor, elements));
      }
      else
        thumbTileRenderer2.TextLines.AddRange(ComicTextBuilder.GetTextBlocks(comicBook, font, foreColor, elements));
      thumbTileRenderer2.DrawTile(graphics, bounds);
      thumbTileRenderer2.DisposeTextLines();
    }

    public static void DrawTile(
      Graphics graphics,
      Rectangle bounds,
      Image image,
      ComicBook comicBook,
      Font font,
      Color foreColor,
      Color backColor,
      ThumbnailDrawingOptions options,
      ComicTextElements elements,
      bool threeD,
      IEnumerable<Image> icons = null)
    {
      ThumbTileRenderer.DrawTile(graphics, bounds, image, comicBook, -1, font, foreColor, backColor, options, elements, threeD, icons);
    }
  }
}

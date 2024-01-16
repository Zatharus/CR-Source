// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.PageViewItem
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Text;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class PageViewItem : ThumbnailViewItem
  {
    private readonly ComicBookNavigator book;
    private readonly ComicBook comic;
    private readonly int imageIndex;
    private bool isCurrentPage;
    private string key = string.Empty;

    public PageViewItem(ComicBookNavigator book, int imageIndex, string key)
    {
      this.book = book;
      this.comic = book.Comic;
      this.key = key;
      this.imageIndex = imageIndex;
      this.UpdateInfo();
      this.comic.BookChanged += new EventHandler<BookChangedEventArgs>(this.comic_BookChanged);
      this.book.Navigation += new EventHandler<BookPageEventArgs>(this.book_Navigation);
    }

    public PageViewItem(ComicBookNavigator book, int imageIndex)
      : this(book, imageIndex, book.GetImageName(imageIndex))
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.comic.BookChanged -= new EventHandler<BookChangedEventArgs>(this.comic_BookChanged);
        this.book.Navigation -= new EventHandler<BookPageEventArgs>(this.book_Navigation);
      }
      base.Dispose(disposing);
    }

    public ComicBookNavigator Book => this.book;

    public ComicBook Comic => this.comic;

    public int ImageIndex => this.imageIndex;

    public ComicPageInfo PageInfo => this.comic.GetPageByImageIndex(this.imageIndex);

    public bool IsCurrentPage
    {
      get => this.isCurrentPage;
      set
      {
        if (this.isCurrentPage == value)
          return;
        this.isCurrentPage = value;
        this.Update();
      }
    }

    public int Page => this.comic.TranslateImageIndexToPage(this.imageIndex);

    public string PageAsText => (this.Page + 1).ToString();

    public string Key
    {
      get => this.key;
      set
      {
        if (this.key == value)
          return;
        this.key = value;
        this.Update();
      }
    }

    public void SetPageType(ComicPageType pageType)
    {
      this.book.Comic.UpdatePageType(this.Page, pageType);
    }

    public void SetPageRotation(ImageRotation rotation)
    {
      this.book.Comic.UpdatePageRotation(this.Page, rotation);
    }

    public void SetPagePosition(ComicPagePosition position)
    {
      this.book.Comic.UpdatePagePosition(this.Page, position);
    }

    protected override System.Drawing.Size GetEstimatedSize(System.Drawing.Size canvasSize)
    {
      ComicPageInfo pageInfo = this.PageInfo;
      System.Drawing.Size imageSize = new System.Drawing.Size(pageInfo.ImageWidth, pageInfo.ImageHeight);
      return imageSize.Width <= 0 || imageSize.Height <= 0 ? base.GetEstimatedSize(canvasSize) : ThumbRenderer.GetSafeScaledImageSize(imageSize, canvasSize);
    }

    public override ItemViewStates GetOwnerDrawnStates(ItemViewMode displayType)
    {
      return displayType == ItemViewMode.Tile ? ItemViewStates.Selected | ItemViewStates.Hot : base.GetOwnerDrawnStates(displayType);
    }

    public override ThumbnailKey ThumbnailKey => this.book.Comic.GetThumbnailKey(this.Page);

    protected override System.Drawing.Size MeasureItem(
      Graphics graphics,
      System.Drawing.Size defaultSize,
      ItemViewMode displayType)
    {
      defaultSize = base.MeasureItem(graphics, defaultSize, displayType);
      return displayType == ItemViewMode.Thumbnail ? this.AddBorder(this.GetThumbnailSizeSafe(defaultSize)) : defaultSize;
    }

    protected override System.Drawing.Size MeasureColumn(
      Graphics graphics,
      IColumn header,
      System.Drawing.Size defaultSize)
    {
      defaultSize = base.MeasureColumn(graphics, header, defaultSize);
      ComicListField tag = (ComicListField) header.Tag;
      if (tag.DisplayProperty == "Thumbnail")
      {
        defaultSize.Width = FormUtility.ScaleDpiX(128);
      }
      else
      {
        string stringValue = this.GetStringValue(tag.DisplayProperty);
        defaultSize = graphics.MeasureString(stringValue, this.View.Font).ToSize();
        defaultSize.Width += 8;
      }
      return defaultSize;
    }

    public override void OnDraw(ItemDrawInformation drawInfo)
    {
      base.OnDraw(drawInfo);
      Color textColor = drawInfo.TextColor;
      Rectangle bounds = drawInfo.Bounds;
      Font font1 = this.View.Font;
      ComicListField tag = drawInfo.Header != null ? drawInfo.Header.Tag as ComicListField : (ComicListField) null;
      List<Image> imageList = (List<Image>) null;
      if (this.PageInfo.IsDeleted)
        imageList = imageList.SafeAdd<Image>((Image) ThumbnailViewItem.DeletedStateImage);
      using (StringFormat format = new StringFormat())
      {
        using (IItemLock<ThumbnailImage> thumbnail1 = tag == null || tag.DisplayProperty == "Thumbnail" ? this.GetThumbnail(drawInfo) : (IItemLock<ThumbnailImage>) null)
        {
          System.Drawing.Size size;
          if (thumbnail1 != null)
          {
            ComicBook comic = this.Comic;
            int page = this.Page;
            size = thumbnail1.Item.OriginalSize;
            int width = size.Width;
            size = thumbnail1.Item.OriginalSize;
            int height = size.Height;
            comic.UpdatePageSize(page, width, height);
          }
          int height1 = drawInfo.DisplayType == ItemViewMode.Detail ? 256 : bounds.Height;
          Image thumbnail2 = (Image) thumbnail1?.Item.GetThumbnail(height1);
          ThumbnailDrawingOptions flags = ThumbnailDrawingOptions.EnableShadow | ThumbnailDrawingOptions.EnableBorder | ThumbnailDrawingOptions.EnableRating | ThumbnailDrawingOptions.EnableVerticalBookmarks | ThumbnailDrawingOptions.EnableBackground | ThumbnailDrawingOptions.EnableStates | ThumbnailDrawingOptions.EnableBowShadow;
          if (this.Selected | this.IsCurrentPage)
            flags |= ThumbnailDrawingOptions.Selected;
          if (this.Hot | this.IsCurrentPage)
            flags |= ThumbnailDrawingOptions.Hot;
          if (this.View.InScrollOrResize)
            flags |= ThumbnailDrawingOptions.FastMode;
          switch (drawInfo.DisplayType)
          {
            case ItemViewMode.Thumbnail:
              this.Animate(thumbnail2);
              ThumbRenderer thumbRenderer = new ThumbRenderer(thumbnail2, flags | ThumbnailDrawingOptions.EnablePageNumber)
              {
                PageNumber = this.Page + 1,
                ImageOpacity = this.Opacity,
                SelectionBackColor = StyledRenderer.GetSelectionColor(drawInfo.ControlFocused),
                Bookmarked = this.PageInfo.IsBookmark
              };
              ref Rectangle local = ref bounds;
              size = this.Border;
              int width1 = -size.Width;
              size = this.Border;
              int height2 = -size.Height;
              local.Inflate(width1, height2);
              if (imageList != null)
                thumbRenderer.StateImages.AddRange((IEnumerable<Image>) imageList);
              thumbRenderer.DrawThumbnail(drawInfo.Graphics, bounds);
              break;
            case ItemViewMode.Tile:
              this.Animate(thumbnail2);
              ThumbTileRenderer thumbTileRenderer1 = new ThumbTileRenderer(thumbnail2, flags);
              thumbTileRenderer1.Font = font1;
              thumbTileRenderer1.Border = this.Border;
              thumbTileRenderer1.ForeColor = textColor;
              thumbTileRenderer1.BackColor = Color.LightGray;
              thumbTileRenderer1.SelectionBackColor = StyledRenderer.GetSelectionColor(drawInfo.ControlFocused);
              thumbTileRenderer1.ImageOpacity = this.Opacity;
              thumbTileRenderer1.Bookmarked = this.PageInfo.IsBookmark;
              ThumbTileRenderer thumbTileRenderer2 = thumbTileRenderer1;
              if (imageList != null)
                thumbTileRenderer2.StateImages.AddRange((IEnumerable<Image>) imageList);
              Font font2 = FC.Get(font1, ((float) bounds.Height * 0.07f).Clamp(font1.Size * 0.8f, font1.Size * 1f));
              thumbTileRenderer2.TextLines.AddRange(ComicTextBuilder.GetTextBlocks(this.PageInfo, this.Page, font2, textColor, ComicTextElements.DefaultPage));
              thumbTileRenderer2.DrawTile(drawInfo.Graphics, bounds);
              thumbTileRenderer2.DisposeTextLines();
              break;
            case ItemViewMode.Detail:
              if (tag != null)
              {
                if (tag.DisplayProperty == "Thumbnail")
                {
                  this.Animate(thumbnail2);
                  if (thumbnail2 == null)
                    break;
                  bounds.Height = bounds.Width * thumbnail2.Height / thumbnail2.Width;
                  bounds.Inflate(-2, -2);
                  drawInfo.Graphics.IntersectClip(bounds);
                  if (this.PageInfo.IsBookmark)
                    flags |= ThumbnailDrawingOptions.Bookmarked;
                  ThumbRenderer.DrawThumbnail(drawInfo.Graphics, thumbnail2, bounds, flags, (ComicBook) null, this.Opacity);
                  break;
                }
                format.Alignment = drawInfo.Header.Alignment;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.LineLimit;
                string stringValue = this.GetStringValue(tag.DisplayProperty);
                bounds.Inflate(-2, 0);
                using (Brush brush = (Brush) new SolidBrush(textColor))
                {
                  drawInfo.Graphics.DrawString(stringValue, font1, brush, (RectangleF) bounds, format);
                  break;
                }
              }
              else
              {
                if (drawInfo.GroupItem % 2 != 0 || (drawInfo.State & ItemViewStates.Selected) != ItemViewStates.None)
                  break;
                using (Brush brush = (Brush) new SolidBrush(Color.LightGray.Transparent(96)))
                {
                  drawInfo.Graphics.FillRectangle(brush, bounds);
                  break;
                }
              }
          }
        }
      }
    }

    protected override void CreateThumbnail(ThumbnailKey key)
    {
      using (Program.ImagePool.GetThumbnail(key, (IImageProvider) this.book, this.book.Comic))
        ;
    }

    private void comic_BookChanged(object sender, BookChangedEventArgs e)
    {
      if (e.Page == -1 || this.comic.TranslatePageToImageIndex(e.Page) != this.imageIndex)
        return;
      this.UpdateInfo();
      this.Update(true);
    }

    private void book_Navigation(object sender, BookPageEventArgs e)
    {
      this.IsCurrentPage = this.PageInfo.Equals((object) e.PageInfo);
      if (!this.IsCurrentPage)
        return;
      this.EnsureVisible();
    }

    private string GetStringValue(string property)
    {
      switch (property)
      {
        case "Key":
          return this.Key;
        case "Page":
          return this.PageAsText;
        default:
          return this.PageInfo.GetStringValue(property);
      }
    }

    private void UpdateInfo()
    {
      this.Text = this.PageAsText;
      this.TooltipText = StringUtility.Format("{0} #{1}", (object) TR.Default["Page", "Page"], (object) this.Text);
    }
  }
}

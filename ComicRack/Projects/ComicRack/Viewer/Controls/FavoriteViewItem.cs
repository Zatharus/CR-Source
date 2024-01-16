// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.FavoriteViewItem
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class FavoriteViewItem : ThumbnailViewItem
  {
    public ComicListItem ComicListItem { get; set; }

    public override string Text => this.ComicListItem.Name;

    public override ItemViewStates GetOwnerDrawnStates(ItemViewMode displayType)
    {
      return ItemViewStates.All;
    }

    public override ThumbnailKey ThumbnailKey
    {
      get
      {
        return new ThumbnailKey((object) this, this.ComicListItem.Id.ToString(), 0, ImageRotation.None);
      }
    }

    protected override System.Drawing.Size MeasureItem(
      Graphics graphics,
      System.Drawing.Size defaultSize,
      ItemViewMode displayType)
    {
      base.MeasureItem(graphics, defaultSize, displayType);
      return defaultSize;
    }

    public override void OnDraw(ItemDrawInformation drawInfo)
    {
      base.OnDraw(drawInfo);
      Rectangle bounds = drawInfo.Bounds;
      Font font = this.View.Font;
      Color foreColor1 = this.View.ForeColor;
      ThumbnailDrawingOptions flags = ThumbnailDrawingOptions.EnableShadow | ThumbnailDrawingOptions.EnableBorder | ThumbnailDrawingOptions.EnableRating | ThumbnailDrawingOptions.EnableVerticalBookmarks | ThumbnailDrawingOptions.EnableBackground | ThumbnailDrawingOptions.EnableStates | ThumbnailDrawingOptions.EnableBowShadow;
      Color foreColor2 = (drawInfo.State & ItemViewStates.Selected) != ItemViewStates.None ? SystemColors.HighlightText : this.View.ForeColor;
      if (this.Selected)
        flags |= ThumbnailDrawingOptions.Selected;
      if (this.Hot)
        flags |= ThumbnailDrawingOptions.Hot;
      if (this.Focused)
        flags |= ThumbnailDrawingOptions.Focused;
      using (IItemLock<ThumbnailImage> thumbnail = this.GetThumbnail(false))
      {
        ThumbTileRenderer thumbTileRenderer = new ThumbTileRenderer((Image) thumbnail?.Item.GetThumbnail(bounds.Height), flags);
        using (StringFormat format = new StringFormat()
        {
          Trimming = StringTrimming.EllipsisPath
        })
        {
          thumbTileRenderer.Font = font;
          thumbTileRenderer.Border = new System.Drawing.Size(2, 2);
          thumbTileRenderer.ForeColor = foreColor1;
          thumbTileRenderer.BackColor = Color.LightGray;
          thumbTileRenderer.SelectionBackColor = StyledRenderer.GetSelectionColor(drawInfo.ControlFocused);
          thumbTileRenderer.TextLines.Add(new TextLine(this.Text, font, foreColor2, afterSpacing: 2));
          thumbTileRenderer.TextLines.Add(new TextLine(this.ComicListItem.GetFullName("/"), FC.GetRelative(font, 0.8f), foreColor2, format, afterSpacing: 5));
          thumbTileRenderer.DrawTile(drawInfo.Graphics, bounds);
          thumbTileRenderer.DisposeTextLines();
        }
      }
    }

    protected override void CreateThumbnail(ThumbnailKey key)
    {
      using (Bitmap bmp = FavoriteViewItem.GetListImage(this.ComicListItem.GetBooks(), new System.Drawing.Size(341, 512), 3, 4))
      {
        using (Program.ImagePool.Thumbs.AddImage((ImageKey) key, (Func<ImageKey, ThumbnailImage>) (k => ThumbnailImage.CreateFrom(bmp, bmp.Size))))
          ;
      }
    }

    private static Bitmap GetListImage(IEnumerable<ComicBook> books, System.Drawing.Size sz, int dx, int dy)
    {
      List<Bitmap> bitmapList = new List<Bitmap>();
      int count = dx * dy;
      try
      {
        foreach (ComicBook comic in books.Take<ComicBook>(count))
        {
          using (IItemLock<ThumbnailImage> thumbnail = Program.ImagePool.GetThumbnail(comic))
            bitmapList.Add(thumbnail.Item.Bitmap.Clone() as Bitmap);
        }
        return bitmapList.CreateMosaicImage(dx, sz, Color.Black);
      }
      finally
      {
        bitmapList.Dispose();
      }
    }

    public static FavoriteViewItem Create(ComicListItem item)
    {
      return new FavoriteViewItem() { ComicListItem = item };
    }
  }
}

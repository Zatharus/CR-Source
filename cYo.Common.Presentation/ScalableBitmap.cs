// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.ScalableBitmap
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Drawing;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation
{
  public class ScalableBitmap
  {
    public ScalableBitmap(Bitmap bitmap, Padding margin)
    {
      this.Bitmap = bitmap;
      this.Margin = margin;
    }

    public ScalableBitmap(Bitmap bitmap, int left, int top, int right, int bottom)
      : this(bitmap, new Padding(left, top, right, bottom))
    {
    }

    public ScalableBitmap(Bitmap bitmap, Rectangle inner)
      : this(bitmap, inner.GetPadding(bitmap.Size))
    {
    }

    public ScalableBitmap(Bitmap bitmap)
      : this(bitmap, Padding.Empty)
    {
    }

    public Bitmap Bitmap { get; set; }

    public Padding Margin { get; set; }

    public Rectangle Inner
    {
      get => new Rectangle(Point.Empty, this.Bitmap.Size).Pad(this.Margin);
      set => this.Margin = value.GetPadding(this.Bitmap.Size);
    }

    public RectangleF Draw(
      IBitmapRenderer gr,
      RectangleF dest,
      RectangleF src,
      BitmapAdjustment itf,
      float opacity)
    {
      return ScalableBitmap.Draw(gr, this.Bitmap, dest, src, this.Margin, itf, opacity);
    }

    public RectangleF Draw(
      IBitmapRenderer gr,
      RectangleF dest,
      BitmapAdjustment itf,
      float opacity)
    {
      return this.Draw(gr, dest, new RectangleF(PointF.Empty, (SizeF) this.Bitmap.Size), itf, opacity);
    }

    public RectangleF Draw(IBitmapRenderer gr, RectangleF dest, float opacity)
    {
      return this.Draw(gr, dest, BitmapAdjustment.Empty, opacity);
    }

    public RectangleF Draw(IBitmapRenderer gr, RectangleF dest) => this.Draw(gr, dest, 1f);

    public static RectangleF Draw(
      IBitmapRenderer gr,
      Bitmap bmp,
      RectangleF dest,
      RectangleF src,
      Padding margin,
      BitmapAdjustment itf,
      float opacity)
    {
      if (margin.All == 0)
      {
        gr?.DrawImage((RendererImage) bmp, dest, src, itf, opacity);
        return dest;
      }
      float left = (float) margin.Left;
      float right = (float) margin.Right;
      float top = (float) margin.Top;
      float bottom = (float) margin.Bottom;
      RectangleF src1 = new RectangleF(src.Left + (float) margin.Left, src.Top + (float) margin.Top, src.Width - (float) margin.Horizontal, src.Height - (float) margin.Vertical);
      RectangleF dest1 = new RectangleF(dest.Left + left, dest.Top + top, dest.Width - left - right, dest.Height - top - bottom);
      if (gr != null)
      {
        gr.DrawImage((RendererImage) bmp, new RectangleF(dest.Left, dest.Top, left, top), new RectangleF(src.Left, src.Top, (float) margin.Left, (float) margin.Top), itf, opacity);
        gr.DrawImage((RendererImage) bmp, new RectangleF(dest1.Left, dest.Top, dest1.Width, top), new RectangleF(src1.Left, src.Top, src1.Width, (float) margin.Top), itf, opacity);
        gr.DrawImage((RendererImage) bmp, new RectangleF(dest1.Right, dest.Top, right, top), new RectangleF(src1.Right, src.Top, (float) margin.Right, (float) margin.Top), itf, opacity);
        gr.DrawImage((RendererImage) bmp, new RectangleF(dest1.Right, dest1.Top, right, dest1.Height), new RectangleF(src1.Right, src1.Top, (float) margin.Right, src1.Height), itf, opacity);
        gr.DrawImage((RendererImage) bmp, new RectangleF(dest1.Right, dest1.Bottom, right, bottom), new RectangleF(src1.Right, src1.Bottom, (float) margin.Right, (float) margin.Bottom), itf, opacity);
        gr.DrawImage((RendererImage) bmp, new RectangleF(dest1.Left, dest1.Bottom, dest1.Width, bottom), new RectangleF(src1.Left, src1.Bottom, src1.Width, (float) margin.Bottom), itf, opacity);
        gr.DrawImage((RendererImage) bmp, new RectangleF(dest.Left, dest1.Bottom, left, bottom), new RectangleF(src.Left, src1.Bottom, (float) margin.Left, (float) margin.Bottom), itf, opacity);
        gr.DrawImage((RendererImage) bmp, new RectangleF(dest.Left, dest1.Top, left, dest1.Height), new RectangleF(src.Left, src1.Top, (float) margin.Left, src1.Height), itf, opacity);
        gr.DrawImage((RendererImage) bmp, dest1, src1, itf, opacity);
      }
      return dest1;
    }

    public static RectangleF Draw(
      IBitmapRenderer gr,
      Bitmap bmp,
      RectangleF dest,
      RectangleF src,
      Padding margin,
      float opacity)
    {
      return ScalableBitmap.Draw(gr, bmp, dest, src, margin, BitmapAdjustment.Empty, opacity);
    }

    public static RectangleF Draw(
      IBitmapRenderer gr,
      Bitmap bmp,
      RectangleF dest,
      Padding margin,
      float opacity)
    {
      return ScalableBitmap.Draw(gr, bmp, dest, new RectangleF((PointF) Point.Empty, (SizeF) bmp.Size), margin, opacity);
    }

    public static implicit operator ScalableBitmap(Bitmap bitmap) => new ScalableBitmap(bitmap);
  }
}

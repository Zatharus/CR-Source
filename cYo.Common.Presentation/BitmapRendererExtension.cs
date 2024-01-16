// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.BitmapRendererExtension
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Drawing;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation
{
  public static class BitmapRendererExtension
  {
    public static void DrawImage(
      this IBitmapRenderer gr,
      RendererImage image,
      RectangleF dest,
      RectangleF src)
    {
      gr.DrawImage(image, dest, src, BitmapAdjustment.Empty, 1f);
    }

    public static void DrawRectangle(
      this IBitmapRenderer gr,
      RectangleF r,
      Color color,
      float width)
    {
      gr.DrawLine((IEnumerable<PointF>) r.ToLineStrip(), color, width);
    }

    public static void DrawRectangle(
      this IBitmapRenderer gr,
      PointF a,
      PointF b,
      Color color,
      float width)
    {
      gr.DrawRectangle(RectangleExtensions.Create(a, b), color, width);
    }

    public static void DrawImage(
      this IBitmapRenderer gr,
      RendererImage image,
      RectangleF dest,
      RectangleF src,
      BitmapAdjustment transform,
      float opacity,
      RectangleF clip)
    {
      if (src.IsEmpty)
        return;
      if (!clip.IsEmpty && clip != dest)
      {
        float num1 = src.Width / dest.Width;
        float num2 = src.Height / dest.Height;
        RectangleF rectangleF = RectangleF.Intersect(dest, clip);
        if (rectangleF.IsEmpty)
          return;
        src.X += (rectangleF.X - dest.X) * num1;
        src.Y += (rectangleF.Y - dest.Y) * num2;
        src.Width += (rectangleF.Width - dest.Width) * num1;
        src.Height += (rectangleF.Height - dest.Height) * num2;
        dest = rectangleF;
      }
      gr.DrawImage(image, dest, src, transform, opacity);
    }

    public static void FillRectangle(
      this IBitmapRenderer gr,
      RendererImage image,
      ImageLayout id,
      RectangleF dest,
      RectangleF src,
      BitmapAdjustment transform,
      float opacity)
    {
      gr.FillRectangle(image, id, dest, src, transform, opacity, dest);
    }

    public static void FillRectangle(
      this IBitmapRenderer gr,
      RendererImage image,
      ImageLayout id,
      RectangleF dest,
      RectangleF src,
      BitmapAdjustment transform,
      float opacity,
      RectangleF clip)
    {
      switch (id)
      {
        case ImageLayout.None:
          dest.Size = src.Size;
          gr.DrawImage(image, dest, src, transform, opacity, clip);
          break;
        case ImageLayout.Tile:
          using (IEnumerator<RectangleF> enumerator = dest.GetSubRectangles(src, true).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              RectangleF current = enumerator.Current;
              gr.DrawImage(image, current, current.Size.ToRectangle(), transform, opacity, clip);
            }
            break;
          }
        case ImageLayout.Center:
          gr.DrawImage(image, src.Align(dest, ContentAlignment.MiddleCenter), src, transform, opacity, clip);
          break;
        case ImageLayout.Stretch:
          gr.DrawImage(image, dest, src, transform, opacity, clip);
          break;
        case ImageLayout.Zoom:
          gr.DrawImage(image, src.Fit(dest, ScaleMode.Center), src, transform, opacity, clip);
          break;
      }
    }

    public static void DrawShadow(
      this IBitmapRenderer graphics,
      RectangleF rectangle,
      int depth,
      Color color,
      float opacity,
      BlurShadowType bst,
      BlurShadowParts parts)
    {
      using (Bitmap shadowBitmap = GraphicsExtensions.CreateShadowBitmap(bst, color, depth, opacity))
        graphics.DrawShadow(rectangle, shadowBitmap, parts);
    }

    public static void DrawShadow(
      this IBitmapRenderer graphics,
      RectangleF rectangle,
      Bitmap shadowBitmap,
      BlurShadowParts parts)
    {
      graphics.DrawShadow(rectangle, shadowBitmap, shadowBitmap.Width / 2, parts);
    }

    public static void DrawShadow(
      this IBitmapRenderer graphics,
      RectangleF rectangle,
      Bitmap shadowBitmap,
      int depth,
      BlurShadowParts parts)
    {
      int num = shadowBitmap.Width / 2;
      if ((parts & BlurShadowParts.Center) != (BlurShadowParts) 0)
        graphics.FillRectangle(new RectangleF(rectangle.Left + (float) depth, rectangle.Top + (float) depth, rectangle.Width - (float) (2 * depth), rectangle.Height - (float) (2 * depth)), shadowBitmap.GetPixel(num, num));
      if ((parts & BlurShadowParts.TopLeft) != (BlurShadowParts) 0)
        graphics.DrawImage((RendererImage) shadowBitmap, new RectangleF(rectangle.Left, rectangle.Top, (float) depth, (float) depth), new RectangleF(0.0f, 0.0f, (float) num, (float) num));
      if ((parts & BlurShadowParts.TopCenter) != (BlurShadowParts) 0)
        graphics.DrawImage((RendererImage) shadowBitmap, new RectangleF(rectangle.Left + (float) depth, rectangle.Top, rectangle.Width - (float) (2 * depth), (float) depth), new RectangleF((float) num, 0.0f, 1f, (float) num));
      if ((parts & BlurShadowParts.TopRight) != (BlurShadowParts) 0)
        graphics.DrawImage((RendererImage) shadowBitmap, new RectangleF(rectangle.Right - (float) depth, rectangle.Top, (float) depth, (float) depth), new RectangleF((float) num, 0.0f, (float) num, (float) num));
      if ((parts & BlurShadowParts.CenterRight) != (BlurShadowParts) 0)
        graphics.DrawImage((RendererImage) shadowBitmap, new RectangleF(rectangle.Right - (float) depth, rectangle.Top + (float) depth, (float) depth, rectangle.Height - (float) (2 * depth)), new RectangleF((float) num, (float) num, (float) num, 1f));
      if ((parts & BlurShadowParts.BottomRight) != (BlurShadowParts) 0)
        graphics.DrawImage((RendererImage) shadowBitmap, new RectangleF(rectangle.Right - (float) depth, rectangle.Bottom - (float) depth, (float) depth, (float) depth), new RectangleF((float) num, (float) num, (float) num, (float) num));
      if ((parts & BlurShadowParts.BottomCenter) != (BlurShadowParts) 0)
        graphics.DrawImage((RendererImage) shadowBitmap, new RectangleF(rectangle.Left + (float) depth, rectangle.Bottom - (float) depth, rectangle.Width - (float) (2 * depth), (float) depth), new RectangleF((float) num, (float) num, 1f, (float) num));
      if ((parts & BlurShadowParts.BottomLeft) != (BlurShadowParts) 0)
        graphics.DrawImage((RendererImage) shadowBitmap, new RectangleF(rectangle.Left, rectangle.Bottom - (float) depth, (float) depth, (float) depth), new RectangleF(0.0f, (float) num, (float) num, (float) num));
      if ((parts & BlurShadowParts.CenterLeft) == (BlurShadowParts) 0)
        return;
      graphics.DrawImage((RendererImage) shadowBitmap, new RectangleF(rectangle.Left, rectangle.Top + (float) depth, (float) depth, rectangle.Height - (float) (2 * depth)), new RectangleF(0.0f, (float) num, (float) num, 1f));
    }
  }
}

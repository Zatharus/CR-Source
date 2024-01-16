// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.GraphicsExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class GraphicsExtensions
  {
    public static IDisposable SaveState(this Graphics graphics)
    {
      if (graphics == null)
        return (IDisposable) null;
      GraphicsState gs = graphics.Save();
      return (IDisposable) new LeanDisposer((Action) (() => graphics.Restore(gs)));
    }

    public static IDisposable AntiAlias(this Graphics graphics, SmoothingMode mode = SmoothingMode.AntiAlias)
    {
      if (graphics == null || mode == graphics.SmoothingMode)
        return (IDisposable) null;
      SmoothingMode sm = graphics.SmoothingMode;
      graphics.SmoothingMode = mode;
      return (IDisposable) new LeanDisposer((Action) (() => graphics.SmoothingMode = sm));
    }

    public static IDisposable TextRendering(this Graphics graphics, TextRenderingHint hint)
    {
      if (graphics == null || graphics.TextRenderingHint == hint)
        return (IDisposable) null;
      TextRenderingHint oldHint = graphics.TextRenderingHint;
      graphics.TextRenderingHint = hint;
      return (IDisposable) new LeanDisposer((Action) (() => graphics.TextRenderingHint = oldHint));
    }

    public static IDisposable HighQuality(
      this Graphics graphics,
      bool enabled,
      float scale = 0.0f,
      bool forceHQ = false)
    {
      InterpolationMode oim = graphics.InterpolationMode;
      InterpolationMode interpolationMode = forceHQ || enabled && (double) scale < 0.5 ? InterpolationMode.HighQualityBicubic : InterpolationMode.Default;
      if (oim == interpolationMode)
        return (IDisposable) null;
      graphics.InterpolationMode = interpolationMode;
      return (IDisposable) new LeanDisposer((Action) (() => graphics.InterpolationMode = oim));
    }

    public static IDisposable HighQuality(
      this Graphics graphics,
      bool enabled,
      SizeF dest,
      SizeF src,
      bool forceHQ = false)
    {
      dest = graphics.TransformRectangle(CoordinateSpace.Device, CoordinateSpace.World, dest.ToRectangle()).Size;
      return graphics.HighQuality(enabled, src.GetScale(dest), forceHQ);
    }

    public static IDisposable Fast(this Graphics graphics, bool ultraFast = false)
    {
      IDisposable disposable = graphics.SaveState();
      if (ultraFast)
      {
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.CompositingQuality = CompositingQuality.HighSpeed;
        graphics.SmoothingMode = SmoothingMode.None;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;
        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      }
      else
      {
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.CompositingQuality = CompositingQuality.HighSpeed;
        graphics.SmoothingMode = SmoothingMode.HighSpeed;
        graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        graphics.InterpolationMode = InterpolationMode.Low;
      }
      return disposable;
    }

    public static Point TransformPoint(
      this Graphics graphics,
      CoordinateSpace destSpace,
      CoordinateSpace srcSpace,
      Point pt)
    {
      Point[] pts = new Point[1]{ pt };
      graphics.TransformPoints(destSpace, srcSpace, pts);
      return pts[0];
    }

    public static RectangleF TransformRectangle(
      this Graphics graphics,
      CoordinateSpace destSpace,
      CoordinateSpace srcSpace,
      RectangleF rect)
    {
      PointF[] points = rect.ToPoints();
      graphics.TransformPoints(destSpace, srcSpace, points);
      return ((IEnumerable<PointF>) points).ToRectangle();
    }

    public static void DrawImage(
      this Graphics graphics,
      Image image,
      Rectangle bounds,
      Rectangle src = default (Rectangle),
      float opacity = 1f)
    {
      if ((double) opacity <= 0.05000000074505806)
        return;
      if (src.IsEmpty())
        src = new Rectangle(0, 0, image.Width, image.Height);
      if ((double) opacity >= 0.949999988079071)
      {
        graphics.DrawImage(image, bounds, src, GraphicsUnit.Pixel);
      }
      else
      {
        using (ImageAttributes imageAttr = new ImageAttributes())
        {
          ColorMatrix newColorMatrix = new ColorMatrix()
          {
            Matrix33 = Math.Max(0.05f, opacity)
          };
          imageAttr.SetColorMatrix(newColorMatrix);
          graphics.DrawImage(image, bounds, src.X, src.Y, src.Width, src.Height, GraphicsUnit.Pixel, imageAttr);
        }
      }
    }

    public static void DrawImage(
      this Graphics graphics,
      Image image,
      Rectangle bounds,
      float opacity = 1f)
    {
      graphics.DrawImage(image, bounds, Rectangle.Empty, opacity);
    }

    public static void DrawImage(
      this Graphics graphics,
      Image image,
      int x,
      int y,
      float opacity = 1f)
    {
      graphics.DrawImage(image, new Rectangle(new Point(x, y), image.Size), opacity);
    }

    public static void DrawImage(
      this Graphics gr,
      Bitmap bitmap,
      Rectangle destination,
      int x,
      int y,
      int width,
      int height,
      BitmapAdjustment adjustment,
      float opacity)
    {
      if ((double) opacity < 0.05000000074505806)
        return;
      if (!adjustment.HasSharpening && !adjustment.HasGamma)
      {
        float wp = 1f;
        float bp = 0.0f;
        if (adjustment.HasAutoContrast)
          bitmap.GetBlackWhitePoint(out bp, out wp);
        if (adjustment.IsEmpty && (double) wp >= 0.949999988079071 && (double) bp <= 0.05000000074505806 && (double) opacity >= 0.949999988079071)
        {
          gr.DrawImage((Image) bitmap, destination, x, y, width, height, GraphicsUnit.Pixel);
        }
        else
        {
          using (ImageAttributes imageAttr = new ImageAttributes())
          {
            ColorMatrix colorMatrix = ImageProcessing.CreateColorMatrix(bp, wp, adjustment.Contrast, adjustment.Brightness, adjustment.Saturation, adjustment.WhitePointColor);
            colorMatrix.Matrix33 = Math.Max(0.05f, opacity);
            imageAttr.SetColorMatrix(colorMatrix);
            gr.DrawImage((Image) bitmap, destination, x, y, width, height, GraphicsUnit.Pixel, imageAttr);
          }
        }
      }
      else
      {
        using (Bitmap adjustedBitmap = bitmap.CreateAdjustedBitmap(adjustment, PixelFormat.Format32bppArgb, true))
          gr.DrawImage((Image) adjustedBitmap, destination, new Rectangle(x, y, width, height), opacity);
      }
    }

    public static void DrawImage(
      this Graphics gr,
      Bitmap bitmap,
      Rectangle destination,
      int x,
      int y,
      int width,
      int height,
      BitmapAdjustment adjustment)
    {
      gr.DrawImage(bitmap, destination, x, y, width, height, adjustment, 1f);
    }

    public static void DrawImage(
      this Graphics gr,
      Bitmap bitmap,
      Rectangle destination,
      BitmapAdjustment it,
      float opacity)
    {
      gr.DrawImage(bitmap, destination, 0, 0, bitmap.Width, bitmap.Height, it, opacity);
    }

    public static void DrawImage(
      this Graphics gr,
      Bitmap bitmap,
      Rectangle destination,
      BitmapAdjustment adjustment)
    {
      gr.DrawImage(bitmap, destination, adjustment, 1f);
    }

    public static void DrawShadow(
      this Graphics graphics,
      Rectangle rectangle,
      int depth,
      Color color,
      float opacity)
    {
      if (graphics == null)
        throw new ArgumentNullException();
      Rectangle rect1 = new Rectangle(rectangle.Right, rectangle.Top + depth, depth, rectangle.Height);
      Rectangle rect2 = new Rectangle(rectangle.Left + depth, rectangle.Bottom, rectangle.Width - depth, depth);
      using (Brush brush = (Brush) new SolidBrush(Color.FromArgb((int) ((double) byte.MaxValue * (double) opacity), color)))
      {
        graphics.FillRectangle(brush, rect1);
        graphics.FillRectangle(brush, rect2);
      }
    }

    public static void DrawShadow(
      this Graphics graphics,
      Rectangle rectangle,
      int depth,
      Color color,
      float opacity,
      BlurShadowType bst,
      BlurShadowParts parts)
    {
      using (Bitmap shadowBitmap = GraphicsExtensions.CreateShadowBitmap(bst, color, depth, opacity))
      {
        if ((parts & BlurShadowParts.Center) != (BlurShadowParts) 0)
        {
          using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb((int) ((double) byte.MaxValue * (double) opacity), color)))
            graphics.FillRectangle((Brush) solidBrush, rectangle.Left + depth, rectangle.Top + depth, rectangle.Width - 2 * depth, rectangle.Height - 2 * depth);
        }
        if ((parts & BlurShadowParts.TopLeft) != (BlurShadowParts) 0)
          graphics.DrawImage((Image) shadowBitmap, new Rectangle(rectangle.Left, rectangle.Top, depth, depth), 0, 0, depth, depth, GraphicsUnit.Pixel);
        if ((parts & BlurShadowParts.TopCenter) != (BlurShadowParts) 0)
          graphics.DrawImage((Image) shadowBitmap, new Rectangle(rectangle.Left + depth, rectangle.Top, rectangle.Width - 2 * depth, depth), depth, 0, 1, depth, GraphicsUnit.Pixel);
        if ((parts & BlurShadowParts.TopRight) != (BlurShadowParts) 0)
          graphics.DrawImage((Image) shadowBitmap, new Rectangle(rectangle.Right - depth, rectangle.Top, depth, depth), depth, 0, depth, depth, GraphicsUnit.Pixel);
        if ((parts & BlurShadowParts.CenterRight) != (BlurShadowParts) 0)
          graphics.DrawImage((Image) shadowBitmap, new Rectangle(rectangle.Right - depth, rectangle.Top + depth, depth, rectangle.Height - 2 * depth), depth, depth, depth, 1, GraphicsUnit.Pixel);
        if ((parts & BlurShadowParts.BottomRight) != (BlurShadowParts) 0)
          graphics.DrawImage((Image) shadowBitmap, new Rectangle(rectangle.Right - depth, rectangle.Bottom - depth, depth, depth), depth, depth, depth, depth, GraphicsUnit.Pixel);
        if ((parts & BlurShadowParts.BottomCenter) != (BlurShadowParts) 0)
          graphics.DrawImage((Image) shadowBitmap, new Rectangle(rectangle.Left + depth, rectangle.Bottom - depth, rectangle.Width - 2 * depth, depth), depth, depth, 1, depth, GraphicsUnit.Pixel);
        if ((parts & BlurShadowParts.BottomLeft) != (BlurShadowParts) 0)
          graphics.DrawImage((Image) shadowBitmap, new Rectangle(rectangle.Left, rectangle.Bottom - depth, depth, depth), 0, depth, depth, depth, GraphicsUnit.Pixel);
        if ((parts & BlurShadowParts.CenterLeft) == (BlurShadowParts) 0)
          return;
        graphics.DrawImage((Image) shadowBitmap, new Rectangle(rectangle.Left, rectangle.Top + depth, depth, rectangle.Height - 2 * depth), 0, depth, depth, 1, GraphicsUnit.Pixel);
      }
    }

    public static Bitmap CreateShadowBitmap(
      BlurShadowType bst,
      Color shadowColor,
      int depth,
      float maxOpacity)
    {
      Color color1 = Color.FromArgb((int) ((double) byte.MaxValue * (double) maxOpacity), shadowColor);
      Color color2 = Color.FromArgb(0, shadowColor);
      if (bst == BlurShadowType.Inside)
      {
        Color color3 = color1;
        color1 = color2;
        color2 = color3;
      }
      depth *= 2;
      using (GraphicsPath path = new GraphicsPath())
      {
        path.AddEllipse(0, 0, depth, depth);
        using (PathGradientBrush pathGradientBrush = new PathGradientBrush(path))
        {
          pathGradientBrush.CenterColor = color1;
          pathGradientBrush.SurroundColors = new Color[1]
          {
            color2
          };
          Bitmap shadowBitmap = new Bitmap(depth, depth);
          using (Graphics graphics = Graphics.FromImage((Image) shadowBitmap))
          {
            graphics.Clear(color2);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.FillEllipse((Brush) pathGradientBrush, 0, 0, depth, depth);
          }
          return shadowBitmap;
        }
      }
    }
  }
}

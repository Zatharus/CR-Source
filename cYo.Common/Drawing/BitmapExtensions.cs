// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.BitmapExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using cYo.Common.Runtime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class BitmapExtensions
  {
    private const float Pi = 3.14159274f;

    public static Bitmap Resize(
      this Bitmap bmp,
      Size size,
      BitmapResampling resampling = BitmapResampling.BilinearHQ,
      PixelFormat format = PixelFormat.Format32bppArgb)
    {
      if (bmp == null || size.IsEmpty())
        return (Bitmap) null;
      if (bmp.Size.IsEmpty())
        return bmp;
      switch (resampling)
      {
        case BitmapResampling.FastAndUgly:
          return ImageProcessing.ResizeFast(bmp, size.Width, size.Height, format, ResizeFastInterpolation.NearestNeighbor);
        case BitmapResampling.FastBilinear:
          return ImageProcessing.ResizeFast(bmp, size.Width, size.Height, format, ResizeFastInterpolation.Bilinear);
        case BitmapResampling.FastBicubic:
          return ImageProcessing.ResizeFast(bmp, size.Width, size.Height, format, ResizeFastInterpolation.Bicubic);
        case BitmapResampling.BilinearHQ:
          return ImageProcessing.ResizeBiliniearHQ(bmp, size.Width, size.Height, format);
        case BitmapResampling.GdiPlus:
          return ImageProcessing.ResizeGdi(bmp, size.Width, size.Height, format);
        case BitmapResampling.GdiPlusHQ:
          return ImageProcessing.ResizeGdi(bmp, size.Width, size.Height, format, true);
        default:
          throw new ArgumentOutOfRangeException(nameof (resampling));
      }
    }

    public static Bitmap Resize(this Bitmap bmp, int width, int height)
    {
      return bmp.Resize(new Size(width, height));
    }

    public static Bitmap Scale(
      this Bitmap bmp,
      Size size,
      BitmapResampling resampling = BitmapResampling.BilinearHQ,
      PixelFormat format = PixelFormat.Format32bppArgb)
    {
      if (bmp == null)
        return (Bitmap) null;
      return bmp.Size.IsEmpty() ? bmp : bmp.Resize(bmp.Size.ToRectangle(size).Size, resampling, format);
    }

    public static Bitmap Scale(this Bitmap bmp, int width, int height)
    {
      return bmp.Scale(new Size(width, height));
    }

    public static Bitmap Scale(this Bitmap bmp, float factor)
    {
      return bmp.Resize(new Size((int) ((double) bmp.Width * (double) factor), (int) ((double) bmp.Height * (double) factor)), BitmapResampling.GdiPlus);
    }

    private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
    {
      return ((IEnumerable<ImageCodecInfo>) ImageCodecInfo.GetImageEncoders()).FirstOrDefault<ImageCodecInfo>((Func<ImageCodecInfo, bool>) (ici => ici.FormatID == format.Guid));
    }

    public static void SaveJpeg(this Image image, Stream s, int quality = -1)
    {
      image.SaveImage(s, ImageFormat.Jpeg, quality: quality);
    }

    public static void SaveJpeg(this Image image, string file, int quality = -1)
    {
      image.SaveImage(file, ImageFormat.Jpeg, quality: quality);
    }

    public static void SaveImage(
      this Image image,
      string file,
      ImageFormat imageFormat,
      int colorDepth = -1,
      int quality = -1)
    {
      using (Stream ms = (Stream) File.Create(file))
        image.SaveImage(ms, imageFormat, colorDepth, quality);
    }

    public static void SaveImage(
      this Image image,
      Stream ms,
      ImageFormat imageFormat,
      int colorDepth = -1,
      int quality = -1)
    {
      int count = 0;
      if (colorDepth > 0)
        ++count;
      if (quality > -1)
        ++count;
      if (count == 0)
      {
        image.Save(ms, imageFormat);
      }
      else
      {
        using (EncoderParameters encoderParams = new EncoderParameters(count))
        {
          ImageCodecInfo encoderInfo = BitmapExtensions.GetEncoderInfo(imageFormat);
          if (quality >= 0)
            encoderParams.Param[--count] = new EncoderParameter(Encoder.Quality, (long) quality);
          if (colorDepth > 0)
          {
            int num;
            encoderParams.Param[num = count - 1] = new EncoderParameter(Encoder.ColorDepth, (long) colorDepth);
          }
          image.Save(ms, encoderInfo, encoderParams);
        }
      }
    }

    public static byte[] ImageToJpegBytes(this Image image, int quality = -1)
    {
      using (MemoryStream s = new MemoryStream())
      {
        image.SaveJpeg((Stream) s, quality);
        return s.ToArray();
      }
    }

    public static byte[] ImageToBytes(
      this Image image,
      ImageFormat imageFormat,
      int colorDepth = -1,
      int quality = -1)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        image.SaveImage((Stream) ms, imageFormat, colorDepth, quality);
        return ms.ToArray();
      }
    }

    public static Bitmap LoadIcon(Stream stream, Color backColor)
    {
      using (Icon icon = new Icon(stream, 1024, 1024))
      {
        Bitmap bitmap = new Bitmap(icon.Width, icon.Height, PixelFormat.Format32bppArgb);
        using (Graphics graphics = Graphics.FromImage((Image) bitmap))
        {
          graphics.Clear(backColor);
          graphics.DrawIcon(icon, new Rectangle(0, 0, icon.Width, icon.Height));
        }
        return bitmap;
      }
    }

    public static Bitmap LoadIcon(string file, Color backColor)
    {
      using (FileStream fileStream = File.OpenRead(file))
        return BitmapExtensions.LoadIcon((Stream) fileStream, backColor);
    }

    public static Bitmap BitmapFromStream(
      Stream stream,
      PixelFormat pixelFormat,
      bool alwaysCreateCopy)
    {
      Bitmap image = (Bitmap) null;
      try
      {
        image = BitmapExtensions.LoadBitmap32BitFix(stream);
        if (pixelFormat == PixelFormat.Undefined)
          pixelFormat = image == null ? PixelFormat.Format32bppArgb : image.PixelFormat;
        return alwaysCreateCopy || image == null || image.PixelFormat != pixelFormat ? image.CreateCopy(pixelFormat, true) : image;
      }
      catch
      {
        image?.Dispose();
        throw;
      }
    }

    private static Bitmap LoadBitmap32BitFix(Stream s)
    {
      if (Machine.Is64Bit)
        return Image.FromStream(s, false, false) as Bitmap;
      try
      {
        if (!(Image.FromStream(s, false, false) is Bitmap bitmap) || bitmap.Width != 0 && bitmap.Height != 0)
          return bitmap;
        throw new IOException();
      }
      catch (Exception ex)
      {
        MemoryStream outStream = new MemoryStream();
        s.Position = 0L;
        if (!JpegFile.RemoveExif(s, (Stream) outStream))
          return (Bitmap) null;
        outStream.Position = 0L;
        return Image.FromStream((Stream) outStream, false, false) as Bitmap;
      }
    }

    public static Bitmap BitmapFromStream(Stream stream, bool alwaysCreateCopy = false)
    {
      return BitmapExtensions.BitmapFromStream(stream, PixelFormat.Format32bppArgb, alwaysCreateCopy);
    }

    public static Bitmap BitmapFromBytes(byte[] data, PixelFormat pixelFormat)
    {
      return BitmapExtensions.BitmapFromStream((Stream) new MemoryStream(data), pixelFormat, false);
    }

    public static Bitmap BitmapFromBytes(byte[] data)
    {
      return BitmapExtensions.BitmapFromStream((Stream) new MemoryStream(data));
    }

    public static Bitmap BitmapFromFile(string file, bool alwaysCreateCopy = true)
    {
      using (FileStream fileStream = File.OpenRead(file))
        return BitmapExtensions.BitmapFromStream((Stream) fileStream, alwaysCreateCopy);
    }

    public static Color GetAverageColor(this Bitmap bitmap, Rectangle rectangle)
    {
      if (bitmap == null)
        return Color.Empty;
      rectangle = Rectangle.Intersect(rectangle, new Rectangle(Point.Empty, bitmap.Size));
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = rectangle.Width * rectangle.Height;
      for (int left = rectangle.Left; left < rectangle.Right; ++left)
      {
        for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
        {
          Color pixel = bitmap.GetPixel(left, top);
          num1 += (int) pixel.R;
          num2 += (int) pixel.G;
          num3 += (int) pixel.B;
        }
      }
      return Color.FromArgb(num1 / num4, num2 / num4, num3 / num4);
    }

    public static Color GetAverageColor(this Bitmap bitmap, Point location, Size size)
    {
      return bitmap.GetAverageColor(new Rectangle(location, size));
    }

    public static Color GetAverageColor(this Bitmap bitmap, int x, int y, int size)
    {
      return bitmap.GetAverageColor(new Point(x, y), new Size(size, size));
    }

    public static Bitmap Clone(this Bitmap bmp, PixelFormat format, bool alwaysClone = false)
    {
      return bmp == null || bmp.PixelFormat == format && !alwaysClone ? bmp : bmp.Clone(bmp.Size.ToRectangle(), format);
    }

    public static Bitmap ToOptimized(this Bitmap bmp, bool disposeOriginal = true)
    {
      Bitmap optimized = bmp.Clone(PixelFormat.Format32bppPArgb);
      if (bmp != optimized)
        bmp.SafeDispose();
      return optimized;
    }

    public static Bitmap CreateCopy(
      this Image image,
      Rectangle clip,
      PixelFormat format,
      bool alwaysTrueCopy = false)
    {
      Bitmap copy = (Bitmap) null;
      try
      {
        if (image == null)
          return (Bitmap) null;
        if (format == PixelFormat.Undefined)
          format = image.PixelFormat;
        if (image is Bitmap source)
        {
          if (!alwaysTrueCopy && image.Size == clip.Size && image.PixelFormat == format)
            return (Bitmap) source.Clone();
          if (format == PixelFormat.Format32bppArgb || format == PixelFormat.Format24bppRgb)
          {
            try
            {
              return source.Copy(clip, format);
            }
            catch
            {
            }
          }
        }
        copy = new Bitmap(clip.Width, clip.Height, format);
        using (Graphics graphics = Graphics.FromImage((Image) copy))
        {
          graphics.SmoothingMode = SmoothingMode.None;
          graphics.InterpolationMode = InterpolationMode.Low;
          graphics.DrawImage(image, new Rectangle(0, 0, clip.Width, clip.Height), clip, GraphicsUnit.Pixel);
        }
        return copy;
      }
      catch
      {
        copy?.Dispose();
        return (Bitmap) null;
      }
    }

    public static Bitmap CreateCopy(this Image image, Rectangle clip, bool alwaysTrueCopy = false)
    {
      return image.CreateCopy(clip, PixelFormat.Format32bppArgb, alwaysTrueCopy);
    }

    public static Bitmap CreateCopy(this Image image, PixelFormat pixelFormat, bool alwaysTrueCopy = false)
    {
      return image.CreateCopy(new Rectangle(0, 0, image.Width, image.Height), pixelFormat, alwaysTrueCopy);
    }

    public static Bitmap CreateCopy(this Image image, bool alwaysTrueCopy = false)
    {
      return image.CreateCopy(PixelFormat.Format32bppArgb, alwaysTrueCopy);
    }

    public static Bitmap Crop(this Image image, Rectangle clip, bool alwaysTrueCopy = false)
    {
      return image.CreateCopy(clip, image.PixelFormat, alwaysTrueCopy);
    }

    public static Bitmap Rotate(this Bitmap image, ImageRotation rotation)
    {
      if (image == null)
        return (Bitmap) null;
      Bitmap bitmap = (Bitmap) image.Clone();
      switch (rotation)
      {
        case ImageRotation.Rotate90:
          bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
          break;
        case ImageRotation.Rotate180:
          bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
          break;
        case ImageRotation.Rotate270:
          bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
          break;
      }
      return bitmap;
    }

    public static Bitmap Distort(
      this Bitmap baseBitmap,
      Point topleft,
      Point topright,
      Point bottomleft,
      Point bottomright)
    {
      Point[] pointArray = new Point[4]
      {
        topleft,
        topright,
        bottomright,
        bottomleft
      };
      int val1_1 = int.MaxValue;
      int num1 = int.MinValue;
      int val1_2 = int.MaxValue;
      int num2 = int.MinValue;
      foreach (Point point in pointArray)
      {
        val1_1 = Math.Min(val1_1, point.X);
        num1 = Math.Max(num1, point.X);
        val1_2 = Math.Min(val1_2, point.Y);
        num2 = Math.Max(num2, point.Y);
      }
      Rectangle rectangle = new Rectangle(0, 0, num1, num2);
      using (Bitmap inputBitmap1 = new Bitmap(rectangle.Width, rectangle.Height))
      {
        using (Bitmap inputBitmap2 = new Bitmap((Image) baseBitmap, rectangle.Width, rectangle.Height))
        {
          PointF pointF1 = (PointF) topleft;
          PointF pointF2 = (PointF) topright;
          PointF pointF3 = (PointF) bottomright;
          PointF pointF4 = (PointF) bottomleft;
          float angularCoefficient1 = BitmapExtensions.GetAngularCoefficient(pointF1, pointF2);
          float angularCoefficient2 = BitmapExtensions.GetAngularCoefficient(pointF4, pointF3);
          float angularCoefficient3 = BitmapExtensions.GetAngularCoefficient(pointF1, pointF4);
          float angularCoefficient4 = BitmapExtensions.GetAngularCoefficient(pointF3, pointF2);
          PointF? intersection1 = BitmapExtensions.GetIntersection(pointF2, angularCoefficient1, pointF3, angularCoefficient2);
          PointF? intersection2 = BitmapExtensions.GetIntersection(pointF1, angularCoefficient3, pointF2, angularCoefficient4);
          using (FastBitmap fastBitmap1 = new FastBitmap(inputBitmap1))
          {
            using (FastBitmap fastBitmap2 = new FastBitmap(inputBitmap2))
            {
              for (int y1 = 0; y1 < rectangle.Height; ++y1)
              {
                for (int x1 = 0; x1 < rectangle.Width; ++x1)
                {
                  PointF pointF5 = new PointF((float) x1, (float) y1);
                  float m1_1 = !intersection1.HasValue ? angularCoefficient1 : BitmapExtensions.GetAngularCoefficient(intersection1.Value, pointF5);
                  float m1_2 = !intersection2.HasValue ? angularCoefficient4 : BitmapExtensions.GetAngularCoefficient(intersection2.Value, pointF5);
                  PointF? intersection3 = BitmapExtensions.GetIntersection(pointF5, m1_1, pointF1, angularCoefficient3);
                  PointF a1 = intersection3.HasValue ? intersection3.Value : pointF1;
                  PointF? intersection4 = BitmapExtensions.GetIntersection(pointF5, m1_1, pointF2, angularCoefficient4);
                  PointF a2 = !intersection4.HasValue ? pointF3 : intersection4.Value;
                  PointF? intersection5 = BitmapExtensions.GetIntersection(pointF5, m1_2, pointF1, angularCoefficient1);
                  PointF a3 = intersection5.HasValue ? intersection5.Value : pointF2;
                  PointF? intersection6 = BitmapExtensions.GetIntersection(pointF5, m1_2, pointF4, angularCoefficient2);
                  PointF a4 = intersection6.HasValue ? intersection6.Value : pointF4;
                  float distance1 = BitmapExtensions.GetDistance(a1, pointF5);
                  float distance2 = BitmapExtensions.GetDistance(a2, pointF5);
                  float distance3 = BitmapExtensions.GetDistance(a3, pointF5);
                  float distance4 = BitmapExtensions.GetDistance(a4, pointF5);
                  int x2 = (int) Math.Round((double) rectangle.Width * (double) distance1 / ((double) distance1 + (double) distance2));
                  int y2 = (int) Math.Round((double) rectangle.Height * (double) distance3 / ((double) distance3 + (double) distance4));
                  if (y2 >= 0 && x2 >= 0 && y2 < rectangle.Height && x2 < rectangle.Width)
                  {
                    Color pixel = fastBitmap2.GetPixel(x2, y2);
                    fastBitmap1.SetPixel(x1, y1, pixel);
                  }
                }
              }
            }
          }
          Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
          using (Graphics graphics = Graphics.FromImage((Image) bitmap))
          {
            using (GraphicsPath path = new GraphicsPath())
            {
              path.AddLines(new PointF[4]
              {
                pointF1,
                pointF2,
                pointF3,
                pointF4
              });
              path.CloseFigure();
              graphics.Clip = new Region(path);
              graphics.DrawImage((Image) inputBitmap1, 0, 0);
            }
          }
          return bitmap;
        }
      }
    }

    public static Bitmap CreateMosaicImage(
      this IEnumerable<Bitmap> images,
      int imagesInEachRow,
      Size size,
      Color backColor)
    {
      Bitmap mosaicImage = new Bitmap(size.Width, size.Height);
      using (Graphics graphics = Graphics.FromImage((Image) mosaicImage))
      {
        Rectangle rect = new Rectangle(Point.Empty, size);
        using (Brush brush = (Brush) new SolidBrush(backColor))
          graphics.FillRectangle(brush, rect);
        float width = (float) size.Width / (float) imagesInEachRow;
        float val2 = float.MaxValue;
        PointF pointF = new PointF(0.0f, 0.0f);
        foreach (Bitmap image in images)
        {
          float num = width / (float) image.Width;
          val2 = Math.Min((float) image.Height * num, val2);
          if ((double) pointF.Y <= (double) size.Height)
          {
            graphics.DrawImage((Image) image, new RectangleF(pointF.X, pointF.Y, width, (float) image.Height * num));
            pointF.X += width;
            if ((double) pointF.X >= (double) size.Width)
            {
              pointF.X = 0.0f;
              pointF.Y += val2;
              val2 = float.MaxValue;
            }
          }
          else
            break;
        }
      }
      return mosaicImage;
    }

    private static float GetDistance(PointF a, PointF b)
    {
      float num1 = a.X - b.X;
      float num2 = a.Y - b.Y;
      return (float) Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2);
    }

    private static PointF? GetIntersection(PointF u, float m1, PointF v, float m2)
    {
      if (u == v)
        return new PointF?(u);
      if ((double) m1 == (double) m2)
        return new PointF?();
      float x = float.Epsilon;
      float y = float.Epsilon;
      if (float.IsInfinity(m1))
      {
        x = u.X;
        y = v.Y + m2 * (-v.X + u.X);
      }
      if (float.IsInfinity(m2))
      {
        x = v.X;
        y = u.Y + m1 * (-u.X + v.X);
      }
      if ((double) x == 1.4012984643248171E-45)
      {
        float num1 = u.Y - m1 * u.X;
        float num2 = v.Y - m2 * v.X;
        x = (float) (((double) num1 - (double) num2) / ((double) m2 - (double) m1));
        y = m1 * x + num1;
      }
      return new PointF?(new PointF(x, y));
    }

    private static float GetAngularCoefficient(PointF u, PointF v)
    {
      float angularCoefficientRads = BitmapExtensions.GetAngularCoefficientRads(u, v);
      if ((double) angularCoefficientRads % 3.1415927410125732 == 1.5707963705062866)
        return float.PositiveInfinity;
      return (double) angularCoefficientRads % 3.1415927410125732 == -1.5707963705062866 ? float.NegativeInfinity : (float) Math.Tan((double) angularCoefficientRads);
    }

    private static float GetAngularCoefficientRads(PointF from, PointF to)
    {
      if ((double) to.Y == (double) from.Y)
        return (double) from.X <= (double) to.X ? 0.0f : 3.14159274f;
      if ((double) to.X == (double) from.X)
        return (double) to.Y >= (double) from.Y ? 1.57079637f : -1.57079637f;
      float angularCoefficientRads = (float) Math.Atan(((double) to.Y - (double) from.Y) / ((double) to.X - (double) from.X));
      if ((double) to.X < 0.0)
      {
        if ((double) angularCoefficientRads > 0.0)
          angularCoefficientRads += 1.57079637f;
        else if ((double) angularCoefficientRads < 0.0)
          angularCoefficientRads -= 3.14159274f;
      }
      return angularCoefficientRads;
    }
  }
}

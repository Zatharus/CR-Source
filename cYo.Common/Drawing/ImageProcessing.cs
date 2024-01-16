// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ImageProcessing
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class ImageProcessing
  {
    private const short blueIndex = 0;
    private const short greenIndex = 1;
    private const short redIndex = 2;
    private const short alphaIndex = 3;
    private const float grayRed = 0.3086f;
    private const float grayGreen = 0.6094f;
    private const float grayBlue = 0.082f;

    private static void CheckFormat(PixelFormat format)
    {
      if (format != PixelFormat.Format24bppRgb && format != PixelFormat.Format32bppArgb && format != PixelFormat.Format32bppRgb && format != PixelFormat.Canonical)
        throw new ArgumentException("Invalid bitmap format");
    }

    private static void CheckFormat(Bitmap bitmap)
    {
      ImageProcessing.CheckFormat(bitmap.PixelFormat);
    }

    public static unsafe Bitmap Copy(this Bitmap source, Rectangle clip, PixelFormat format)
    {
      ImageProcessing.CheckFormat(source);
      ImageProcessing.CheckFormat(format);
      clip.Intersect(source.Size.ToRectangle());
      Bitmap bitmap = new Bitmap(clip.Width, clip.Height, format);
      BitmapData bitmapdata1 = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
      BitmapData bitmapdata2 = bitmap.LockBits(new Rectangle(0, 0, clip.Width, clip.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
      try
      {
        IntPtr scan0 = bitmapdata1.Scan0;
        IntPtr scanTarget = bitmapdata2.Scan0;
        int strideSource = bitmapdata1.Stride;
        int pixelWidthSource = bitmapdata1.Stride / source.Width;
        int strideTarget = bitmapdata2.Stride;
        int num1 = bitmapdata2.Stride / bitmap.Width;
        int l = clip.Left;
        int w = clip.Width;
        int height = clip.Height;
        byte* p = (byte*) ((IntPtr) (void*) scan0 + strideSource * clip.Y);
        if (pixelWidthSource == 3)
        {
          if (num1 == 3)
            Parallel.For(0, height, (Action<int>) (y =>
            {
              byte* numPtr1 = p + y * strideSource + l * pixelWidthSource;
              byte* numPtr2 = (byte*) ((IntPtr) (void*) scanTarget + y * strideTarget);
              for (int index = 0; index < w; ++index)
              {
                byte* numPtr3 = numPtr2;
                byte* numPtr4 = numPtr3 + 1;
                byte* numPtr5 = numPtr1;
                byte* numPtr6 = numPtr5 + 1;
                int num2 = (int) *numPtr5;
                *numPtr3 = (byte) num2;
                byte* numPtr7 = numPtr4;
                byte* numPtr8 = numPtr7 + 1;
                byte* numPtr9 = numPtr6;
                byte* numPtr10 = numPtr9 + 1;
                int num3 = (int) *numPtr9;
                *numPtr7 = (byte) num3;
                byte* numPtr11 = numPtr8;
                numPtr2 = numPtr11 + 1;
                byte* numPtr12 = numPtr10;
                numPtr1 = numPtr12 + 1;
                int num4 = (int) *numPtr12;
                *numPtr11 = (byte) num4;
              }
            }));
          else
            Parallel.For(0, height, (Action<int>) (y =>
            {
              byte* numPtr13 = p + y * strideSource + l * pixelWidthSource;
              byte* numPtr14 = (byte*) ((IntPtr) (void*) scanTarget + y * strideTarget);
              for (int index = 0; index < w; ++index)
              {
                byte* numPtr15 = numPtr14;
                byte* numPtr16 = numPtr15 + 1;
                byte* numPtr17 = numPtr13;
                byte* numPtr18 = numPtr17 + 1;
                int num5 = (int) *numPtr17;
                *numPtr15 = (byte) num5;
                byte* numPtr19 = numPtr16;
                byte* numPtr20 = numPtr19 + 1;
                byte* numPtr21 = numPtr18;
                byte* numPtr22 = numPtr21 + 1;
                int num6 = (int) *numPtr21;
                *numPtr19 = (byte) num6;
                byte* numPtr23 = numPtr20;
                byte* numPtr24 = numPtr23 + 1;
                byte* numPtr25 = numPtr22;
                numPtr13 = numPtr25 + 1;
                int num7 = (int) *numPtr25;
                *numPtr23 = (byte) num7;
                byte* numPtr26 = numPtr24;
                numPtr14 = numPtr26 + 1;
                *numPtr26 = byte.MaxValue;
              }
            }));
        }
        else if (num1 == 3)
          Parallel.For(0, height, (Action<int>) (y =>
          {
            byte* numPtr27 = p + y * strideSource + l * pixelWidthSource;
            byte* numPtr28 = (byte*) ((IntPtr) (void*) scanTarget + y * strideTarget);
            for (int index = 0; index < w; ++index)
            {
              byte* numPtr29 = numPtr28;
              byte* numPtr30 = numPtr29 + 1;
              byte* numPtr31 = numPtr27;
              byte* numPtr32 = numPtr31 + 1;
              int num8 = (int) *numPtr31;
              *numPtr29 = (byte) num8;
              byte* numPtr33 = numPtr30;
              byte* numPtr34 = numPtr33 + 1;
              byte* numPtr35 = numPtr32;
              byte* numPtr36 = numPtr35 + 1;
              int num9 = (int) *numPtr35;
              *numPtr33 = (byte) num9;
              byte* numPtr37 = numPtr34;
              numPtr28 = numPtr37 + 1;
              byte* numPtr38 = numPtr36;
              byte* numPtr39 = numPtr38 + 1;
              int num10 = (int) *numPtr38;
              *numPtr37 = (byte) num10;
              numPtr27 = numPtr39 + 1;
            }
          }));
        else
          Parallel.For(0, height, (Action<int>) (y =>
          {
            int* numPtr40 = (int*) (p + y * strideSource + l * pixelWidthSource);
            int* numPtr41 = (int*) ((IntPtr) (void*) scanTarget + y * strideTarget);
            for (int index = 0; index < w; ++index)
              *numPtr41++ = *numPtr40++;
          }));
      }
      finally
      {
        source.UnlockBits(bitmapdata1);
        bitmap.UnlockBits(bitmapdata2);
      }
      return bitmap;
    }

    public static Bitmap Copy(this Bitmap source, PixelFormat format)
    {
      return source.Copy(source.Size.ToRectangle(), PixelFormat.Format32bppArgb);
    }

    public static Bitmap Copy(this Bitmap source)
    {
      return source.Copy(source.Size.ToRectangle(), PixelFormat.Format32bppArgb);
    }

    public static unsafe void ChangeAlpha(
      this Bitmap bitmap,
      Rectangle clipRectangle,
      float alphaStart,
      float alphaEnd)
    {
      Rectangle rect = bitmap != null && bitmap.PixelFormat == PixelFormat.Format32bppArgb ? bitmap.Size.ToRectangle() : throw new ArgumentException("must be 32 bit bitmap");
      if (clipRectangle.IsEmpty)
        clipRectangle = rect;
      else
        clipRectangle.Intersect(rect);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      float num1 = (alphaEnd - alphaStart) / (float) clipRectangle.Height;
      float num2 = alphaStart * 256f;
      try
      {
        int num3 = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        int num4 = clipRectangle.Width * num3;
        int num5 = stride - num4;
        byte* numPtr1 = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * num3);
        for (byte* numPtr2 = numPtr1 + clipRectangle.Height * stride; numPtr1 < numPtr2; numPtr1 += num5)
        {
          byte* numPtr3 = numPtr1 + num4;
          byte num6 = (byte) num2;
          for (; numPtr1 < numPtr3; numPtr1 += num3)
            numPtr1[3] = (byte) ((int) numPtr1[3] * (int) num6 >> 8);
          num2 += num1;
        }
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
    }

    public static void ChangeAlpha(this Bitmap bitmap, float alphaStart, float alphaEnd)
    {
      bitmap.ChangeAlpha(new Rectangle(0, 0, bitmap.Width, bitmap.Height), alphaStart, alphaEnd);
    }

    public static void ChangeAlpha(this Bitmap bitmap, float alpha)
    {
      bitmap.ChangeAlpha(alpha, alpha);
    }

    public static void ChangeAlpha(this Bitmap bitmap, byte alpha)
    {
      bitmap.ChangeAlpha((float) alpha / (float) byte.MaxValue);
    }

    public static Rectangle GetTransparentBounds(this Bitmap bitmap, byte alpha = 16)
    {
      if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
        return new Rectangle(0, 0, bitmap.Width, bitmap.Height);
      using (FastBitmap fastBitmap = new FastBitmap(bitmap))
      {
        int width = bitmap.Width;
        int height = bitmap.Height;
        int num1 = width;
        int val2_1 = -1;
        int num2 = 0;
        int val2_2 = 0;
        for (int index = 0; index < height; ++index)
        {
          int num3;
          Color pixel;
          for (num3 = 0; num3 < width; ++num3)
          {
            pixel = fastBitmap.GetPixel(num3, index);
            if ((int) pixel.A >= (int) alpha)
              break;
          }
          int num4;
          for (num4 = width - 1; num4 > num3; --num4)
          {
            pixel = fastBitmap.GetPixel(num4, index);
            if ((int) pixel.A >= (int) alpha)
              break;
          }
          num1 = Math.Min(num3, num1);
          val2_1 = Math.Max(num4, val2_1);
          if (num3 < num4)
            val2_2 = Math.Max(index, val2_2);
          else if (val2_2 == 0)
            num2 = Math.Max(index, num2);
        }
        return num1 >= val2_1 || num2 >= val2_2 ? Rectangle.Empty : new Rectangle(num1, num2, val2_1 - num1 + 1, val2_2 - num2 + 1);
      }
    }

    public static Bitmap CropTransparent(this Bitmap bitmap, bool width = true, bool height = true, byte alpha = 16)
    {
      Rectangle transparentBounds = bitmap.GetTransparentBounds(alpha);
      if (!width)
      {
        transparentBounds.X = 0;
        transparentBounds.Width = bitmap.Width;
      }
      if (!height)
      {
        transparentBounds.Y = 0;
        transparentBounds.Height = bitmap.Height;
      }
      return bitmap.Crop(transparentBounds);
    }

    public static unsafe void Invert(this Bitmap bitmap, Rectangle clipRectangle)
    {
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      try
      {
        int pixelWidth = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        byte* pbase = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * pixelWidth);
        int clipScanLen = clipRectangle.Width * pixelWidth;
        Parallel.For(0, clipRectangle.Height, (Action<int>) (h =>
        {
          byte* numPtr1 = pbase + stride * h;
          for (byte* numPtr2 = numPtr1 + clipScanLen; numPtr1 < numPtr2; numPtr1 += pixelWidth)
          {
            *numPtr1 = (byte) ((uint) byte.MaxValue - (uint) *numPtr1);
            numPtr1[1] = (byte) ((uint) byte.MaxValue - (uint) numPtr1[1]);
            numPtr1[2] = (byte) ((uint) byte.MaxValue - (uint) numPtr1[2]);
          }
        }));
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
    }

    public static void Invert(this Bitmap bitmap) => bitmap.Invert(Rectangle.Empty);

    public static unsafe void ToGrayScale(this Bitmap bitmap, Rectangle clipRectangle)
    {
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      try
      {
        int pixelWidth = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        byte* pbase = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * pixelWidth);
        int clipScanLen = clipRectangle.Width * pixelWidth;
        Parallel.For(0, clipRectangle.Height, (Action<int>) (h =>
        {
          byte* numPtr1 = pbase + stride * h;
          for (byte* numPtr2 = numPtr1 + clipScanLen; numPtr1 < numPtr2; numPtr1 += pixelWidth)
            *numPtr1 = (byte) (*(sbyte*) (numPtr1 + 1) = *(sbyte*) (numPtr1 + 2) = (sbyte) (byte) (0.29899999499320984 * (double) numPtr1[2] + 0.58700001239776611 * (double) numPtr1[1] + 57.0 / 500.0 * (double) *numPtr1));
        }));
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
    }

    public static void ToGrayScale(this Bitmap bitmap) => bitmap.ToGrayScale(Rectangle.Empty);

    public static unsafe void ChangeBrightness(
      this Bitmap bitmap,
      Rectangle clipRectangle,
      int brightness)
    {
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      try
      {
        int pixelWidth = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        byte* pbase = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * pixelWidth);
        int clipScanLen = clipRectangle.Width * pixelWidth;
        Parallel.For(0, clipRectangle.Height, (Action<int>) (h =>
        {
          byte* numPtr1 = pbase + stride * h;
          for (byte* numPtr2 = numPtr1 + clipScanLen; numPtr1 < numPtr2; numPtr1 += pixelWidth)
          {
            *numPtr1 = (byte) Math.Min(Math.Max((int) *numPtr1 + brightness, 0), (int) byte.MaxValue);
            numPtr1[1] = (byte) Math.Min(Math.Max((int) numPtr1[1] + brightness, 0), (int) byte.MaxValue);
            numPtr1[2] = (byte) Math.Min(Math.Max((int) numPtr1[2] + brightness, 0), (int) byte.MaxValue);
          }
        }));
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
    }

    public static void ChangeBrightness(this Bitmap bitmap, int brightness)
    {
      bitmap.ChangeBrightness(Rectangle.Empty, brightness);
    }

    public static unsafe void ChangeContrast(
      this Bitmap bitmap,
      Rectangle clipRectangle,
      int contrast)
    {
      if (contrast < -100 || contrast > 100)
        throw new ArgumentException("Must be in the range +/- 100");
      float c = (float) ((100.0 + (double) contrast) / 100.0);
      c *= c;
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      try
      {
        int pixelWidth = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        byte* pbase = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * pixelWidth);
        int clipScanLen = clipRectangle.Width * pixelWidth;
        Parallel.For(0, clipRectangle.Height, (Action<int>) (h =>
        {
          byte* numPtr1 = pbase + stride * h;
          for (byte* numPtr2 = numPtr1 + clipScanLen; numPtr1 < numPtr2; numPtr1 += pixelWidth)
          {
            *numPtr1 = (byte) Math.Min(Math.Max((float) ((double) ((int) *numPtr1 - 128) * (double) c + 128.0), 0.0f), (float) byte.MaxValue);
            numPtr1[1] = (byte) Math.Min(Math.Max((float) ((double) ((int) numPtr1[1] - 128) * (double) c + 128.0), 0.0f), (float) byte.MaxValue);
            numPtr1[2] = (byte) Math.Min(Math.Max((float) ((double) ((int) numPtr1[2] - 128) * (double) c + 128.0), 0.0f), (float) byte.MaxValue);
          }
        }));
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
    }

    public static void ChangeContrast(this Bitmap bitmap, int contrast)
    {
      bitmap.ChangeContrast(Rectangle.Empty, contrast);
    }

    public static unsafe void ChangeGamma(
      this Bitmap bitmap,
      Rectangle clipRectangle,
      double red,
      double green,
      double blue)
    {
      ImageProcessing.CheckFormat(bitmap);
      red = red.Clamp(0.2, 5.0);
      green = green.Clamp(0.2, 5.0);
      blue = blue.Clamp(0.2, 5.0);
      byte[] redGamma = new byte[256];
      byte[] greenGamma = new byte[256];
      byte[] blueGamma = new byte[256];
      for (int index = 0; index < 256; ++index)
      {
        redGamma[index] = (byte) Math.Min((double) byte.MaxValue, (double) byte.MaxValue * Math.Pow((double) index / (double) byte.MaxValue, 1.0 / red) + 0.5);
        greenGamma[index] = (byte) Math.Min((double) byte.MaxValue, (double) byte.MaxValue * Math.Pow((double) index / (double) byte.MaxValue, 1.0 / green) + 0.5);
        blueGamma[index] = (byte) Math.Min((double) byte.MaxValue, (double) byte.MaxValue * Math.Pow((double) index / (double) byte.MaxValue, 1.0 / blue) + 0.5);
      }
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      try
      {
        int pixelWidth = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        byte* pbase = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * pixelWidth);
        int clipScanLen = clipRectangle.Width * pixelWidth;
        Parallel.For(0, clipRectangle.Height, (Action<int>) (h =>
        {
          byte* numPtr1 = pbase + stride * h;
          for (byte* numPtr2 = numPtr1 + clipScanLen; numPtr1 < numPtr2; numPtr1 += pixelWidth)
          {
            *numPtr1 = blueGamma[(int) *numPtr1];
            numPtr1[1] = greenGamma[(int) numPtr1[1]];
            numPtr1[2] = redGamma[(int) numPtr1[2]];
          }
        }));
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
    }

    public static void ChangeGamma(this Bitmap bitmap, double red, double green, double blue)
    {
      bitmap.ChangeGamma(Rectangle.Empty, red, green, blue);
    }

    public static void ChangeGamma(this Bitmap bitmap, float grayGamma)
    {
      bitmap.ChangeGamma(Rectangle.Empty, (double) grayGamma, (double) grayGamma, (double) grayGamma);
    }

    public static unsafe void Colorize(
      this Bitmap bitmap,
      Rectangle clipRectangle,
      int red,
      int green,
      int blue,
      PixelOperation operation = PixelOperation.Multiply)
    {
      ImageProcessing.CheckFormat(bitmap);
      if (red < -255 || red > (int) byte.MaxValue || green < -255 || green > (int) byte.MaxValue || blue < -255 || blue > (int) byte.MaxValue)
        throw new ArgumentException("values must be in the range +/- 255");
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      try
      {
        int pixelWidth = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        byte* pbase = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * pixelWidth);
        int clipScanLen = clipRectangle.Width * pixelWidth;
        Parallel.For(0, clipRectangle.Height, (Action<int>) (h =>
        {
          byte* numPtr1 = pbase + stride * h;
          byte* numPtr2 = numPtr1 + clipScanLen;
          switch (operation)
          {
            case PixelOperation.Add:
              for (; numPtr1 < numPtr2; numPtr1 += pixelWidth)
              {
                *numPtr1 = (byte) Math.Min(Math.Max((int) *numPtr1 + blue, 0), (int) byte.MaxValue);
                numPtr1[1] = (byte) Math.Min(Math.Max((int) numPtr1[1] + green, 0), (int) byte.MaxValue);
                numPtr1[2] = (byte) Math.Min(Math.Max((int) numPtr1[2] + red, 0), (int) byte.MaxValue);
              }
              break;
            case PixelOperation.Replace:
              for (; numPtr1 < numPtr2; numPtr1 += pixelWidth)
              {
                *numPtr1 = (byte) blue;
                numPtr1[1] = (byte) green;
                numPtr1[2] = (byte) red;
              }
              break;
            default:
              for (; numPtr1 < numPtr2; numPtr1 += pixelWidth)
              {
                *numPtr1 = (byte) Math.Min(Math.Max((int) *numPtr1 * blue / 256, 0), (int) byte.MaxValue);
                numPtr1[1] = (byte) Math.Min(Math.Max((int) numPtr1[1] * green / 256, 0), (int) byte.MaxValue);
                numPtr1[2] = (byte) Math.Min(Math.Max((int) numPtr1[2] * red / 256, 0), (int) byte.MaxValue);
              }
              break;
          }
        }));
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
    }

    public static void Colorize(this Bitmap bitmap, int red, int green, int blue)
    {
      bitmap.Colorize(Rectangle.Empty, red, green, blue);
    }

    public static void Colorize(this Bitmap bitmap, Color color)
    {
      bitmap.Colorize((int) color.R, (int) color.G, (int) color.B);
    }

    public static unsafe void ApplyColorMatrix(
      this Bitmap bitmap,
      Rectangle clipRectangle,
      ColorMatrix matrix)
    {
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      float m00 = matrix[0, 0];
      float m01 = matrix[0, 1];
      float m02 = matrix[0, 2];
      float m03 = matrix[0, 3];
      float m10 = matrix[1, 0];
      float m11 = matrix[1, 1];
      float m12 = matrix[1, 2];
      float m13 = matrix[1, 3];
      float m20 = matrix[2, 0];
      float m21 = matrix[2, 1];
      float m22 = matrix[2, 2];
      float m23 = matrix[2, 3];
      float m30 = matrix[3, 0];
      float m31 = matrix[3, 1];
      float m32 = matrix[3, 2];
      float m33 = matrix[3, 3];
      try
      {
        int pixelWidth = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        byte* pbase = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * pixelWidth);
        int clipScanLen = clipRectangle.Width * pixelWidth;
        Parallel.For(0, clipRectangle.Height, (Action<int>) (h =>
        {
          byte* numPtr1 = pbase + stride * h;
          byte* numPtr2 = numPtr1 + clipScanLen;
          if (pixelWidth == 4)
          {
            for (; numPtr1 < numPtr2; numPtr1 += pixelWidth)
            {
              float num1 = (float) *numPtr1;
              float num2 = (float) numPtr1[1];
              float num3 = (float) numPtr1[2];
              float num4 = (float) numPtr1[3];
              numPtr1[2] = (byte) Math.Max(Math.Min((float) byte.MaxValue, (float) ((double) num3 * (double) m00 + (double) num2 * (double) m10 + (double) num1 * (double) m20 + (double) num4 * (double) m30)), 0.0f);
              numPtr1[1] = (byte) Math.Max(Math.Min((float) byte.MaxValue, (float) ((double) num3 * (double) m01 + (double) num2 * (double) m11 + (double) num1 * (double) m21 + (double) num4 * (double) m31)), 0.0f);
              *numPtr1 = (byte) Math.Max(Math.Min((float) byte.MaxValue, (float) ((double) num3 * (double) m02 + (double) num2 * (double) m12 + (double) num1 * (double) m22 + (double) num4 * (double) m32)), 0.0f);
              numPtr1[3] = (byte) Math.Max(Math.Min((float) byte.MaxValue, (float) ((double) num3 * (double) m03 + (double) num2 * (double) m13 + (double) num1 * (double) m23 + (double) num4 * (double) m33)), 0.0f);
            }
          }
          else
          {
            for (; numPtr1 < numPtr2; numPtr1 += pixelWidth)
            {
              float num5 = (float) *numPtr1;
              float num6 = (float) numPtr1[1];
              float num7 = (float) numPtr1[2];
              numPtr1[2] = (byte) Math.Max(Math.Min((float) byte.MaxValue, (float) ((double) num7 * (double) m00 + (double) num6 * (double) m10 + (double) num5 * (double) m20) + m30), 0.0f);
              numPtr1[1] = (byte) Math.Max(Math.Min((float) byte.MaxValue, (float) ((double) num7 * (double) m01 + (double) num6 * (double) m11 + (double) num5 * (double) m21) + m31), 0.0f);
              *numPtr1 = (byte) Math.Max(Math.Min((float) byte.MaxValue, (float) ((double) num7 * (double) m02 + (double) num6 * (double) m12 + (double) num5 * (double) m22) + m32), 0.0f);
            }
          }
        }));
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
    }

    public static void ApplyColorMatrix(this Bitmap bitmap, ColorMatrix matrix)
    {
      bitmap.ApplyColorMatrix(Rectangle.Empty, matrix);
    }

    public static unsafe void Convolute(
      this Bitmap bitmap,
      Rectangle clipRectangle,
      ConvolutionMatrix m)
    {
      if (m.Divisor == 0)
        throw new ArgumentException("factor of matrix can not be 0");
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return;
      using (Bitmap bitmap1 = bitmap.Clone(clipRectangle, bitmap.PixelFormat))
      {
        BitmapData bitmapdata1 = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        BitmapData bitmapdata2 = bitmap1.LockBits(new Rectangle(0, 0, bitmap1.Width, bitmap1.Height), ImageLockMode.ReadWrite, bitmap1.PixelFormat);
        try
        {
          IntPtr scan0_1 = bitmapdata1.Scan0;
          IntPtr scan0_2 = bitmapdata2.Scan0;
          int sourceStride = bitmapdata2.Stride;
          int sourceStride2 = sourceStride * 2;
          int targetStride = bitmapdata1.Stride;
          int pixelWidth = sourceStride / bitmap.Width;
          int pixelWidth2 = 2 * pixelWidth;
          int num = bitmap.Width - 2;
          int toExclusive = bitmap.Height - 2;
          int clipScanLen = pixelWidth * num;
          int mtl = m.TopLeft;
          int mtm = m.TopMid;
          int mtr = m.TopRight;
          int mml = m.MidLeft;
          int mp = m.Pixel;
          int mmr = m.MidRight;
          int mbl = m.BottomLeft;
          int mbm = m.BottomMid;
          int mbr = m.BottomRight;
          byte* porg = (byte*) ((IntPtr) (void*) scan0_1 + clipRectangle.Top * targetStride + clipRectangle.Left * pixelWidth);
          byte* pSrcOrg = (byte*) (void*) scan0_2;
          Parallel.For(0, toExclusive, (Action<int>) (h =>
          {
            byte* numPtr1 = porg + h * targetStride;
            byte* numPtr2 = pSrcOrg + h * sourceStride;
            byte* numPtr3 = numPtr1 + clipScanLen;
            while (numPtr1 < numPtr3)
            {
              numPtr1[pixelWidth + 2 + targetStride] = (byte) Math.Min(Math.Max(((int) numPtr2[2] * mtl + (int) numPtr2[pixelWidth + 2] * mtm + (int) numPtr2[pixelWidth2 + 2] * mtr + (int) numPtr2[2 + sourceStride] * mml + (int) numPtr2[pixelWidth + 2 + sourceStride] * mp + (int) numPtr2[pixelWidth2 + 2 + sourceStride] * mmr + (int) numPtr2[2 + sourceStride2] * mbl + (int) numPtr2[pixelWidth + 2 + sourceStride2] * mbm + (int) numPtr2[pixelWidth2 + 2 + sourceStride2] * mbr) / m.Divisor + m.Offset, 0), (int) byte.MaxValue);
              numPtr1[pixelWidth + 1 + targetStride] = (byte) Math.Min(Math.Max(((int) numPtr2[1] * mtl + (int) numPtr2[pixelWidth + 1] * mtm + (int) numPtr2[pixelWidth2 + 1] * mtr + (int) numPtr2[1 + sourceStride] * mml + (int) numPtr2[pixelWidth + 1 + sourceStride] * mp + (int) numPtr2[pixelWidth2 + 1 + sourceStride] * mmr + (int) numPtr2[1 + sourceStride2] * mbl + (int) numPtr2[pixelWidth + 1 + sourceStride2] * mbm + (int) numPtr2[pixelWidth2 + 1 + sourceStride2] * mbr) / m.Divisor + m.Offset, 0), (int) byte.MaxValue);
              numPtr1[pixelWidth + targetStride] = (byte) Math.Min(Math.Max(((int) *numPtr2 * mtl + (int) numPtr2[pixelWidth] * mtm + (int) numPtr2[pixelWidth2] * mtr + (int) numPtr2[0 + sourceStride] * mml + (int) numPtr2[pixelWidth + sourceStride] * mp + (int) numPtr2[pixelWidth2 + sourceStride] * mmr + (int) numPtr2[0 + sourceStride2] * mbl + (int) numPtr2[pixelWidth + sourceStride2] * mbm + (int) numPtr2[pixelWidth2 + sourceStride2] * mbr) / m.Divisor + m.Offset, 0), (int) byte.MaxValue);
              numPtr1 += pixelWidth;
              numPtr2 += pixelWidth;
            }
          }));
        }
        finally
        {
          bitmap.UnlockBits(bitmapdata1);
          bitmap1.UnlockBits(bitmapdata2);
        }
      }
    }

    public static void Convolute(this Bitmap bitmap, ConvolutionMatrix m)
    {
      bitmap.Convolute(Rectangle.Empty, m);
    }

    public static void Smooth(this Bitmap bitmap, Rectangle clipRectangle, int weight)
    {
      bitmap.Convolute(clipRectangle, new ConvolutionMatrix(1)
      {
        Pixel = weight,
        Divisor = weight + 8
      });
    }

    public static void Smooth(this Bitmap bitmap, int weight)
    {
      bitmap.Smooth(Rectangle.Empty, weight);
    }

    public static void Smooth(this Bitmap bitmap, Rectangle clipRectangle)
    {
      bitmap.Smooth(clipRectangle, 1);
    }

    public static void Smooth(this Bitmap bitmap) => bitmap.Smooth(Rectangle.Empty);

    public static void GaussianBlur(this Bitmap bitmap, Rectangle clipRectangle, int weight)
    {
      ConvolutionMatrix m = new ConvolutionMatrix(1)
      {
        Pixel = weight
      };
      m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
      m.Divisor = weight + 12;
      bitmap.Convolute(clipRectangle, m);
    }

    public static void GaussianBlur(this Bitmap bitmap, int weight)
    {
      bitmap.GaussianBlur(Rectangle.Empty, weight);
    }

    public static void GaussianBlur(this Bitmap bitmap, Rectangle clipRectangle)
    {
      bitmap.GaussianBlur(clipRectangle, 4);
    }

    public static void GaussianBlur(this Bitmap bitmap) => bitmap.GaussianBlur(Rectangle.Empty);

    public static void MeanRemoval(this Bitmap bitmap, Rectangle clipRectangle, int weight)
    {
      bitmap.Convolute(clipRectangle, new ConvolutionMatrix(-1)
      {
        Pixel = weight,
        Divisor = weight - 8
      });
    }

    public static void MeanRemoval(this Bitmap bitmap, int weight)
    {
      bitmap.MeanRemoval(Rectangle.Empty, weight);
    }

    public static void MeanRemoval(this Bitmap bitmap, Rectangle clipRectangle)
    {
      bitmap.MeanRemoval(clipRectangle, 9);
    }

    public static void MeanRemoval(this Bitmap bitmap) => bitmap.MeanRemoval(Rectangle.Empty);

    public static void Sharpen(this Bitmap bitmap, Rectangle clipRectangle, int a, int b)
    {
      ConvolutionMatrix m = new ConvolutionMatrix(0)
      {
        Pixel = a
      };
      m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -b;
      m.Divisor = a + 4 * -b;
      if (m.Divisor == 0)
        m.Divisor = 1;
      bitmap.Convolute(Rectangle.Empty, m);
    }

    public static void Sharpen(this Bitmap bitmap, int a, int b)
    {
      bitmap.Sharpen(Rectangle.Empty, a, b);
    }

    public static void Sharpen(this Bitmap bitmap, Rectangle clipRectangle)
    {
      bitmap.Sharpen(5, -1);
    }

    public static void Sharpen(this Bitmap bitmap) => bitmap.Sharpen(Rectangle.Empty);

    public static void EmbossLaplacian(this Bitmap b, Rectangle clipRectangle)
    {
      ConvolutionMatrix m = new ConvolutionMatrix(-1);
      m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 0;
      m.Pixel = 4;
      m.Offset = (int) sbyte.MaxValue;
      b.Convolute(clipRectangle, m);
    }

    public static void EmbossLaplacian(this Bitmap b) => b.EmbossLaplacian(Rectangle.Empty);

    public static void EdgeDetectQuick(this Bitmap bitmap, Rectangle clipRectangle)
    {
      ConvolutionMatrix m = new ConvolutionMatrix();
      m.TopLeft = m.TopMid = m.TopRight = -1;
      m.MidLeft = m.Pixel = m.MidRight = 0;
      m.BottomLeft = m.BottomMid = m.BottomRight = 1;
      m.Offset = (int) sbyte.MaxValue;
      bitmap.Convolute(clipRectangle, m);
    }

    public static void EdgeDetectQuick(this Bitmap bitmap)
    {
      bitmap.EdgeDetectQuick(Rectangle.Empty);
    }

    public static unsafe Bitmap ResizeFast(
      Bitmap source,
      int newWidth,
      int newHeight,
      PixelFormat format,
      ResizeFastInterpolation method)
    {
      Bitmap image = source;
      if (format != PixelFormat.Format32bppArgb && format != PixelFormat.Format24bppRgb)
        format = PixelFormat.Format32bppArgb;
      int width = image.Width;
      int height = image.Height;
      if (newWidth == width && newHeight == height)
        return image.CreateCopy(format);
      if (image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format24bppRgb)
        image = image.CreateCopy(PixelFormat.Format32bppArgb);
      BitmapData bitmapdata1 = (BitmapData) null;
      BitmapData bitmapdata2 = (BitmapData) null;
      Bitmap bitmap = (Bitmap) null;
      try
      {
        bitmapdata1 = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, image.PixelFormat);
        bitmap = new Bitmap(newWidth, newHeight, format);
        bitmapdata2 = bitmap.LockBits(new Rectangle(0, 0, newWidth, newHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        int srcStride = bitmapdata1.Stride;
        int srcPixelSize = bitmapdata1.Stride / width;
        int num1 = srcStride - srcPixelSize * width;
        int dstStride = bitmapdata2.Stride;
        int dstPixelSize = dstStride / newWidth;
        int minPixelSize = Math.Min(srcPixelSize, dstPixelSize);
        int dstExtra = dstPixelSize - minPixelSize;
        int num2 = dstStride - dstPixelSize * newWidth;
        float xFactor = (float) width / (float) newWidth;
        float yFactor = (float) height / (float) newHeight;
        byte* src = (byte*) (void*) bitmapdata1.Scan0;
        byte* orgdst = (byte*) (void*) bitmapdata2.Scan0;
        if (dstPixelSize == 4)
          ImageProcessing.InitializeAlpha32(orgdst, newWidth, newHeight, dstStride);
        switch (method)
        {
          case ResizeFastInterpolation.NearestNeighbor:
            Parallel.For(0, newHeight, (Action<int>) (y =>
            {
              byte* numPtr1 = orgdst + y * dstStride;
              int num3 = (int) ((double) y * (double) yFactor);
              for (int index1 = 0; index1 < newWidth; ++index1)
              {
                int num4 = (int) ((double) index1 * (double) xFactor);
                byte* numPtr2 = src + num3 * srcStride + num4 * srcPixelSize;
                for (int index2 = 0; index2 < minPixelSize; ++index2)
                  *numPtr1++ = *numPtr2++;
                numPtr1 += dstExtra;
              }
            }));
            break;
          case ResizeFastInterpolation.Bilinear:
            int ymax1 = height - 1;
            int xmax1 = width - 1;
            Parallel.For(0, newHeight, (Action<int>) (y =>
            {
              byte* numPtr3 = orgdst + y * dstStride;
              float num5 = (float) y * yFactor;
              int num6 = (int) num5;
              int num7 = num6 == ymax1 ? num6 : num6 + 1;
              float num8 = num5 - (float) num6;
              float num9 = 1f - num8;
              byte* numPtr4 = src + num6 * srcStride;
              byte* numPtr5 = src + num7 * srcStride;
              for (int index = 0; index < newWidth; ++index)
              {
                float num10 = (float) index * xFactor;
                int num11 = (int) num10;
                int num12 = num11 == xmax1 ? num11 : num11 + 1;
                float num13 = num10 - (float) num11;
                float num14 = 1f - num13;
                byte* numPtr6 = numPtr4 + num11 * srcPixelSize;
                byte* numPtr7 = numPtr4 + num12 * srcPixelSize;
                byte* numPtr8 = numPtr5 + num11 * srcPixelSize;
                byte* numPtr9 = numPtr5 + num12 * srcPixelSize;
                int num15 = 0;
                while (num15 < minPixelSize)
                {
                  byte num16 = (byte) ((double) num14 * (double) *numPtr6 + (double) num13 * (double) *numPtr7);
                  byte num17 = (byte) ((double) num14 * (double) *numPtr8 + (double) num13 * (double) *numPtr9);
                  *numPtr3 = (byte) ((double) num9 * (double) num16 + (double) num8 * (double) num17);
                  ++num15;
                  ++numPtr3;
                  ++numPtr6;
                  ++numPtr7;
                  ++numPtr8;
                  ++numPtr9;
                }
                numPtr3 += dstExtra;
              }
            }));
            break;
          case ResizeFastInterpolation.Bicubic:
            int ymax2 = height - 1;
            int xmax2 = width - 1;
            Parallel.For(0, newHeight, (Action<int>) (y =>
            {
              byte* numPtr10 = orgdst + y * dstStride;
              float num18 = (float) ((double) y * (double) yFactor - 0.5);
              int num19 = (int) num18;
              float num20 = num18 - (float) num19;
              float[] numArray = new float[4];
              int num21 = 0;
              while (num21 < newWidth)
              {
                float num22 = (float) ((double) num21 * (double) xFactor - 0.5);
                int num23 = (int) num22;
                float num24 = num22 - (float) num23;
                for (int index = 0; index < minPixelSize; ++index)
                  numArray[index] = 0.0f;
                for (int index3 = -1; index3 < 3; ++index3)
                {
                  float num25 = ImageProcessing.BiCubicKernel(num20 - (float) index3);
                  int num26 = num19 + index3;
                  if (num26 < 0)
                    num26 = 0;
                  if (num26 > ymax2)
                    num26 = ymax2;
                  for (int index4 = -1; index4 < 3; ++index4)
                  {
                    float num27 = num25 * ImageProcessing.BiCubicKernel((float) index4 - num24);
                    int num28 = num23 + index4;
                    if (num28 < 0)
                      num28 = 0;
                    if (num28 > xmax2)
                      num28 = xmax2;
                    byte* numPtr11 = src + num26 * srcStride + num28 * srcPixelSize;
                    for (int index5 = 0; index5 < minPixelSize; ++index5)
                      numArray[index5] += num27 * (float) numPtr11[index5];
                  }
                }
                for (int index = 0; index < minPixelSize; ++index)
                {
                  byte* numPtr12 = numPtr10 + index;
                  *numPtr12 = (byte) ((uint) *numPtr12 + (uint) (byte) numArray[index]);
                }
                ++num21;
                numPtr10 += dstPixelSize;
              }
            }));
            break;
        }
        return bitmap;
      }
      finally
      {
        if (bitmapdata2 != null)
          bitmap.UnlockBits(bitmapdata2);
        if (bitmapdata1 != null)
          image.UnlockBits(bitmapdata1);
        if (image != source)
          image.Dispose();
      }
    }

    private static unsafe void InitializeAlpha32(byte* scan, int width, int height, int strideLen)
    {
      for (int index1 = 0; index1 < height; ++index1)
      {
        uint* numPtr = (uint*) (scan + index1 * strideLen);
        for (int index2 = 0; index2 < width; ++index2)
          numPtr[index2] = 4278190080U;
      }
    }

    private static float BiCubicKernel(float x)
    {
      if ((double) x > 2.0)
        return 0.0f;
      float num1 = x - 1f;
      float num2 = x + 1f;
      float num3 = x + 2f;
      return (float) (0.1666666716337204 * (((double) num3 <= 0.0 ? 0.0 : (double) (num3 * num3 * num3)) - 4.0 * ((double) num2 <= 0.0 ? 0.0 : (double) (num2 * num2 * num2)) + 6.0 * ((double) x <= 0.0 ? 0.0 : (double) (x * x * x)) - 4.0 * ((double) num1 <= 0.0 ? 0.0 : (double) (num1 * num1 * num1))));
    }

    public static unsafe Bitmap ResizeBiliniearHQ(
      Bitmap srcImg,
      int targetWidth,
      int targetHeight,
      PixelFormat targetFormat)
    {
      Bitmap image = srcImg;
      int height = image.Height;
      int width = image.Width;
      if (targetFormat != PixelFormat.Format32bppArgb && targetFormat != PixelFormat.Format24bppRgb)
        targetFormat = PixelFormat.Format32bppArgb;
      if (targetWidth == width && targetHeight == height)
        return srcImg.CreateCopy(targetFormat);
      if (image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format24bppRgb)
        image = image.CreateCopy(PixelFormat.Format32bppArgb);
      Bitmap bitmap = new Bitmap(targetWidth, targetHeight, targetFormat);
      float num1 = width > targetWidth ? (float) width / (float) targetWidth : (float) (width - 1) / (float) targetWidth;
      float num2 = height > targetHeight ? (float) height / (float) targetHeight : (float) (height - 1) / (float) targetHeight;
      BitmapData bitmapdata1 = (BitmapData) null;
      BitmapData bitmapdata2 = (BitmapData) null;
      try
      {
        bitmapdata1 = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, image.PixelFormat);
        int stride1 = bitmapdata1.Stride;
        int pixelWidth = stride1 / width;
        bitmapdata2 = bitmap.LockBits(new Rectangle(0, 0, targetWidth, targetHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        int stride2 = bitmapdata2.Stride;
        int num3 = stride2 / targetWidth;
        int num4 = targetWidth * num3;
        float num5 = 1f / num1;
        ImageProcessing.RGBA[] rgbaArray = new ImageProcessing.RGBA[width];
        fixed (ImageProcessing.RGBA* pspan = rgbaArray)
        {
          float num6 = 0.0f;
          byte* scan0_1 = (byte*) (void*) bitmapdata1.Scan0;
          byte* scan0_2 = (byte*) (void*) bitmapdata2.Scan0;
          if (num3 == 4)
            ImageProcessing.InitializeAlpha32(scan0_2, targetWidth, targetHeight, stride2);
          int num7 = 0;
          while (num7 < targetHeight)
          {
            float fact = num6 - (float) (int) num6;
            float num8;
            if ((double) num2 >= 1.0)
            {
              float num9 = num6 + num2;
              byte* psrc = scan0_1 + (int) num6 * stride1;
              ImageProcessing.ClearSpan(pspan, width);
              ImageProcessing.AddSpan(pspan, psrc, width, pixelWidth, 1f - fact);
              for (int index = (int) num6; index < (int) num9 - 1; ++index)
              {
                psrc += stride1;
                ImageProcessing.AddSpan(pspan, psrc, width, pixelWidth, 1f);
              }
              ImageProcessing.AddSpan(pspan, psrc, width, pixelWidth, num9 - (float) (int) num9);
              num8 = 1f / num2;
            }
            else
            {
              byte* psrc = scan0_1 + (int) num6 * stride1;
              byte* psrc2 = psrc + stride1;
              ImageProcessing.InterpolateSpan(pspan, psrc, psrc2, width, pixelWidth, fact);
              num8 = 1f;
            }
            byte* numPtr1 = scan0_2 + num7 * stride2;
            byte* numPtr2 = numPtr1 + num4;
            float num10 = 0.0f;
            if ((double) num1 >= 1.0)
            {
              ImageProcessing.RGBA* rgbaPtr = pspan;
              int num11 = 0;
              float num12 = num5 * num8;
              while (numPtr1 < numPtr2)
              {
                int num13 = num11 + 1;
                float num14 = (float) num13 - num10;
                num10 += num1;
                num11 = (int) num10;
                float num15 = num10 - (float) num11;
                float num16 = rgbaPtr->R * num14;
                float num17 = rgbaPtr->G * num14;
                float num18 = rgbaPtr->B * num14;
                for (; num13 < num11; ++num13)
                {
                  ++rgbaPtr;
                  num16 += rgbaPtr->R;
                  num17 += rgbaPtr->G;
                  num18 += rgbaPtr->B;
                }
                float num19 = num16 + rgbaPtr->R * num15;
                float num20 = num17 + rgbaPtr->G * num15;
                float num21 = num18 + rgbaPtr->B * num15;
                *numPtr1 = (byte) ((double) num19 * (double) num12);
                numPtr1[1] = (byte) ((double) num20 * (double) num12);
                numPtr1[2] = (byte) ((double) num21 * (double) num12);
                numPtr1 += num3;
                ++rgbaPtr;
              }
            }
            else
            {
              while (numPtr1 < numPtr2)
              {
                float num22 = num10 - (float) (int) num10;
                ImageProcessing.RGBA* rgbaPtr1 = pspan + (int) num10;
                ImageProcessing.RGBA* rgbaPtr2 = rgbaPtr1 + 1;
                *numPtr1 = (byte) (((double) rgbaPtr1->R + ((double) rgbaPtr2->R - (double) rgbaPtr1->R) * (double) num22) * (double) num8);
                numPtr1[1] = (byte) (((double) rgbaPtr1->G + ((double) rgbaPtr2->G - (double) rgbaPtr1->G) * (double) num22) * (double) num8);
                numPtr1[2] = (byte) (((double) rgbaPtr1->B + ((double) rgbaPtr2->B - (double) rgbaPtr1->B) * (double) num22) * (double) num8);
                numPtr1 += num3;
                num10 += num1;
              }
            }
            ++num7;
            num6 += num2;
          }
        }
        return bitmap;
      }
      finally
      {
        if (bitmapdata1 != null)
          image.UnlockBits(bitmapdata1);
        if (bitmapdata2 != null)
          bitmap.UnlockBits(bitmapdata2);
        if (image != null && image != srcImg)
          image.Dispose();
      }
    }

    private static unsafe void ClearSpan(ImageProcessing.RGBA* pspan, int width)
    {
      int num = 0;
      while (num < width)
      {
        pspan->R = pspan->G = pspan->B = 0.0f;
        ++num;
        ++pspan;
      }
    }

    private static unsafe void AddSpan(
      ImageProcessing.RGBA* pspan,
      byte* psrc,
      int width,
      int pixelWidth,
      float fact)
    {
      if ((double) fact == 0.0)
        return;
      if ((double) fact == 1.0)
      {
        int num = 0;
        while (num < width)
        {
          pspan->R += (float) *psrc;
          pspan->G += (float) psrc[1];
          pspan->B += (float) psrc[2];
          ++num;
          psrc += pixelWidth;
          ++pspan;
        }
      }
      else
      {
        int num = 0;
        while (num < width)
        {
          pspan->R += (float) *psrc * fact;
          pspan->G += (float) psrc[1] * fact;
          pspan->B += (float) psrc[2] * fact;
          ++num;
          psrc += pixelWidth;
          ++pspan;
        }
      }
    }

    private static unsafe void InterpolateSpan(
      ImageProcessing.RGBA* pspan,
      byte* psrc,
      byte* psrc2,
      int width,
      int pixelWidth,
      float fact)
    {
      if ((double) fact == 0.0)
      {
        int num = 0;
        while (num < width)
        {
          pspan->R = (float) *psrc;
          pspan->G = (float) psrc[1];
          pspan->B = (float) psrc[2];
          ++num;
          psrc += pixelWidth;
          ++pspan;
        }
      }
      else
      {
        int num = 0;
        while (num < width)
        {
          pspan->R = (float) *psrc + (float) ((int) *psrc2 - (int) *psrc) * fact;
          pspan->G = (float) psrc[1] + (float) ((int) psrc2[1] - (int) psrc[1]) * fact;
          pspan->B = (float) psrc[2] + (float) ((int) psrc2[2] - (int) psrc[2]) * fact;
          ++num;
          psrc += pixelWidth;
          psrc2 += pixelWidth;
          ++pspan;
        }
      }
    }

    public static Bitmap ResizeGdi(
      Bitmap bitmap,
      int width,
      int height,
      PixelFormat pixelFormat,
      bool highQuality = false)
    {
      Rectangle rectangle = new Rectangle(0, 0, width, height);
      if (rectangle.IsEmpty())
        return (Bitmap) null;
      Bitmap bitmap1 = new Bitmap(width, height, pixelFormat);
      try
      {
        using (Graphics graphics = Graphics.FromImage((Image) bitmap1))
        {
          using (graphics.HighQuality(true, (SizeF) rectangle.Size, (SizeF) bitmap1.Size, highQuality))
            graphics.DrawImage((Image) bitmap, rectangle, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
        }
        return bitmap1;
      }
      catch
      {
        bitmap1.SafeDispose();
        return (Bitmap) null;
      }
    }

    public static unsafe Histogram GetHistogram(this Bitmap bitmap, Rectangle clipRectangle)
    {
      ImageProcessing.CheckFormat(bitmap);
      Rectangle rectangle = bitmap.Size.ToRectangle();
      if (clipRectangle.IsEmpty)
        clipRectangle = rectangle;
      else
        clipRectangle.Intersect(rectangle);
      if (clipRectangle.Width == 0 || clipRectangle.Height == 0)
        return Histogram.Empty;
      int[] reds = new int[256];
      int[] greens = new int[256];
      int[] blues = new int[256];
      int[] grays = new int[256];
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
      try
      {
        int pixelWidth = bitmapdata.Stride / bitmap.Width;
        int stride = bitmapdata.Stride;
        byte* pbase = (byte*) ((IntPtr) (void*) bitmapdata.Scan0 + clipRectangle.Top * stride + clipRectangle.Left * pixelWidth);
        int clipScanLen = clipRectangle.Width * pixelWidth;
        Parallel.For(0, clipRectangle.Height, (Action<int>) (h =>
        {
          byte* numPtr1 = pbase + stride * h;
          for (byte* numPtr2 = numPtr1 + clipScanLen; numPtr1 < numPtr2; numPtr1 += pixelWidth)
          {
            int index1 = (int) *numPtr1;
            int index2 = (int) numPtr1[1];
            int index3 = (int) numPtr1[2];
            ++reds[index3];
            ++greens[index2];
            ++blues[index1];
            ++grays[(int) ((double) index3 * 0.30860000848770142 + (double) index2 * 0.60939997434616089 + (double) index1 * 0.0820000022649765)];
          }
        }));
      }
      finally
      {
        bitmap.UnlockBits(bitmapdata);
      }
      return new Histogram(reds, greens, blues, grays, clipRectangle.Width * clipRectangle.Height);
    }

    public static Histogram GetHistogram(this Bitmap bitmap)
    {
      return bitmap.GetHistogram(Rectangle.Empty);
    }

    public static Matrix CreateColorScaleMatrix(float scale, float offset)
    {
      Matrix colorScaleMatrix = new Matrix(5, 5, 1.0);
      colorScaleMatrix[0, 0] = colorScaleMatrix[1, 1] = colorScaleMatrix[2, 2] = (double) scale;
      colorScaleMatrix[3, 0] = colorScaleMatrix[3, 1] = colorScaleMatrix[3, 2] = (double) offset;
      return colorScaleMatrix;
    }

    public static Matrix CreateColorSaturationMatrix(float sat)
    {
      Matrix saturationMatrix = new Matrix(5, 5, 1.0)
      {
        [0, 0] = (1.0 - (double) sat) * 0.30860000848770142 + (double) sat
      };
      saturationMatrix[0, 1] = saturationMatrix[0, 2] = (1.0 - (double) sat) * 0.30860000848770142;
      saturationMatrix[1, 1] = (1.0 - (double) sat) * 0.60939997434616089 + (double) sat;
      saturationMatrix[1, 0] = saturationMatrix[1, 2] = (1.0 - (double) sat) * 0.60939997434616089;
      saturationMatrix[2, 2] = (1.0 - (double) sat) * 0.0820000022649765 + (double) sat;
      saturationMatrix[2, 0] = saturationMatrix[2, 1] = (1.0 - (double) sat) * 0.0820000022649765;
      return saturationMatrix;
    }

    public static Matrix CreateColorWhitePointMatrix(Color whitePoint)
    {
      try
      {
        Matrix whitePointMatrix = new Matrix(5, 5, 1.0);
        byte r = whitePoint.R;
        byte g = whitePoint.G;
        byte b = whitePoint.B;
        byte num = (byte) (0.30860000848770142 * (double) r + 0.60939997434616089 * (double) g + 0.0820000022649765 * (double) b);
        whitePointMatrix[3, 0] = (double) ((int) num - (int) r) / 256.0;
        whitePointMatrix[3, 1] = (double) ((int) num - (int) g) / 256.0;
        whitePointMatrix[3, 2] = (double) ((int) num - (int) b) / 256.0;
        whitePointMatrix[0, 0] = 1.0 / (1.0 - whitePointMatrix[3, 0]);
        whitePointMatrix[1, 1] = 1.0 / (1.0 - whitePointMatrix[3, 1]);
        whitePointMatrix[2, 2] = 1.0 / (1.0 - whitePointMatrix[3, 1]);
        return whitePointMatrix;
      }
      catch
      {
        Matrix whitePointMatrix = new Matrix(5, 5, 1.0);
        whitePointMatrix[0, 0] = whitePointMatrix[1, 1] = whitePointMatrix[2, 2] = 1.0;
        return whitePointMatrix;
      }
    }

    public static ColorMatrix CreateColorMatrix(
      float blackLevel,
      float whiteLevel,
      float contrast,
      float brightness,
      float saturation,
      Color whitePointColor)
    {
      Matrix matrix = ImageProcessing.CreateColorScaleMatrix((float) (((double) contrast + 1.0) / ((double) whiteLevel - (double) blackLevel)), brightness - blackLevel) * ImageProcessing.CreateColorSaturationMatrix(saturation + 1f) * ImageProcessing.CreateColorWhitePointMatrix(whitePointColor);
      matrix[4, 0] = matrix[4, 1] = matrix[4, 2] = 1.0 / 1000.0;
      float[][] newColorMatrix = new float[5][];
      for (int i = 0; i < 5; ++i)
      {
        newColorMatrix[i] = new float[5];
        for (int j = 0; j < 5; ++j)
          newColorMatrix[i][j] = (float) matrix[i, j];
      }
      return new ColorMatrix(newColorMatrix);
    }

    public static void GetBlackWhitePoint(this Bitmap bitmap, out float bp, out float wp)
    {
      try
      {
        Histogram histogram = bitmap.GetHistogram();
        bp = histogram.GetBlackPointNormalized();
        wp = histogram.GetWhitePointNormalized();
      }
      catch
      {
        wp = 1f;
        bp = 0.0f;
      }
    }

    public static void ApplyAdjustment(
      this Bitmap bitmap,
      Rectangle clipRectangle,
      BitmapAdjustment adjustment)
    {
      ImageProcessing.CheckFormat(bitmap);
      float wp = 1f;
      float bp = 0.0f;
      if (adjustment.HasAutoContrast)
        bitmap.GetBlackWhitePoint(out bp, out wp);
      if (adjustment.HasColorTransformations || (double) wp < 0.949999988079071 || (double) bp > 0.05000000074505806)
        bitmap.ApplyColorMatrix(clipRectangle, ImageProcessing.CreateColorMatrix(bp, wp, adjustment.Contrast, adjustment.Brightness, adjustment.Saturation, adjustment.WhitePointColor));
      if (adjustment.HasGamma)
        bitmap.ChangeGamma(1f + adjustment.Gamma);
      if (adjustment.Sharpen == 0)
        return;
      bitmap.Sharpen(clipRectangle, (4 - adjustment.Sharpen) * 5, 1);
    }

    public static void ApplyAdjustment(this Bitmap bitmap, BitmapAdjustment adjustment)
    {
      bitmap.ApplyAdjustment(Rectangle.Empty, adjustment);
    }

    public static Bitmap CreateAdjustedBitmap(
      this Bitmap bitmap,
      BitmapAdjustment adjustment,
      PixelFormat pixelFormat,
      bool alwaysClone)
    {
      if (bitmap == null)
        return (Bitmap) null;
      float wp = 1f;
      float bp = 0.0f;
      if (adjustment.HasAutoContrast)
        bitmap.GetBlackWhitePoint(out bp, out wp);
      if (!alwaysClone && !adjustment.HasColorTransformations && (double) wp >= 0.949999988079071 && (double) bp <= 0.05000000074505806 && !adjustment.HasSharpening && !adjustment.HasGamma)
        return bitmap;
      Bitmap bitmap1 = (Bitmap) null;
      try
      {
        if (!alwaysClone && !adjustment.HasColorTransformations && (double) wp >= 0.949999988079071 && (double) bp <= 0.05000000074505806)
        {
          bitmap1 = bitmap;
        }
        else
        {
          bitmap1 = bitmap.Copy(pixelFormat);
          bitmap1.ApplyColorMatrix(ImageProcessing.CreateColorMatrix(bp, wp, adjustment.Contrast, adjustment.Brightness, adjustment.Saturation, adjustment.WhitePointColor));
        }
        if (adjustment.HasGamma)
          bitmap1.ChangeGamma(1f + adjustment.Gamma);
        if (adjustment.Sharpen != 0)
          bitmap1.Sharpen((4 - adjustment.Sharpen) * 5, 1);
        return bitmap1;
      }
      catch
      {
        if (bitmap1 != null && bitmap != bitmap1)
          bitmap1.Dispose();
        return (Bitmap) null;
      }
    }

    public static Bitmap CreateAdjustedBitmap(
      this Bitmap bitmap,
      BitmapAdjustment adjustment,
      bool alwaysClone)
    {
      return bitmap.CreateAdjustedBitmap(adjustment, PixelFormat.Format32bppArgb, alwaysClone);
    }

    public static void Process(this Bitmap bitmap, IEnumerable<ImageAction> processingStack)
    {
      processingStack.ForEach<ImageAction>((Action<ImageAction>) (a => a(bitmap)));
    }

    private struct RGBA
    {
      public float R;
      public float G;
      public float B;
    }
  }
}

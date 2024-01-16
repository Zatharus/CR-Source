// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.SizeExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class SizeExtensions
  {
    public static bool IsEmpty(this Size size) => size.Width == 0 || size.Height == 0;

    public static bool IsEmpty(this SizeF size)
    {
      return (double) size.Width == 0.0 || (double) size.Height == 0.0;
    }

    public static float GetScale(
      this SizeF size,
      SizeF targetSize,
      ScaleMode scaleMode = ScaleMode.FitAll,
      bool allowOverSize = true)
    {
      if (scaleMode == ScaleMode.None)
        return 1f;
      float num1 = targetSize.Width / size.Width;
      float num2 = targetSize.Height / size.Height;
      float val2;
      if ((double) num1 == 0.0)
        val2 = num2;
      else if ((double) num2 == 0.0)
      {
        val2 = num1;
      }
      else
      {
        switch (scaleMode)
        {
          case ScaleMode.FitWidth:
            val2 = num1;
            break;
          case ScaleMode.FitHeight:
            val2 = num2;
            break;
          case ScaleMode.Fill:
            val2 = (double) num2 > (double) num1 ? num2 : num1;
            break;
          case ScaleMode.Center:
            val2 = (double) num2 >= (double) num1 ? num2 : num1;
            break;
          default:
            val2 = (double) num2 < (double) num1 ? num2 : num1;
            break;
        }
      }
      return !allowOverSize ? Math.Min(1f, val2) : val2;
    }

    public static float GetScale(
      this Size size,
      Size targetSize,
      ScaleMode scaleMode = ScaleMode.FitAll,
      bool allowOversize = true)
    {
      return ((SizeF) size).GetScale((SizeF) targetSize, scaleMode);
    }

    public static Size Scale(this Size size, float scale)
    {
      return new Size((int) ((double) size.Width * (double) scale), (int) ((double) size.Height * (double) scale));
    }

    public static Size Scale(this Size size, float scaleX, float scaleY)
    {
      return new Size((int) ((double) size.Width * (double) scaleX), (int) ((double) size.Height * (double) scaleY));
    }

    public static Rectangle ToRectangle(this Size size) => new Rectangle(Point.Empty, size);

    public static RectangleF ToRectangle(this SizeF size) => new RectangleF(PointF.Empty, size);

    public static RectangleF ToRectangle(
      this SizeF size,
      SizeF targetSize,
      RectangleScaleMode mode = RectangleScaleMode.Center)
    {
      if (size.IsEmpty())
        return (RectangleF) Rectangle.Empty;
      if (size == targetSize)
        return new RectangleF(0.0f, 0.0f, size.Width, size.Height);
      bool flag1 = (mode & RectangleScaleMode.Center) != 0;
      bool flag2 = (mode & RectangleScaleMode.OnlyShrink) != 0;
      float num = size.GetScale(targetSize);
      if ((double) num > 1.0 & flag2)
        num = 1f;
      RectangleF rectangle = new RectangleF(0.0f, 0.0f, size.Width * num, size.Height * num);
      if (flag1)
      {
        if ((double) targetSize.Width < 1.0)
          targetSize.Width = rectangle.Width;
        if ((double) targetSize.Height < 1.0)
          targetSize.Height = rectangle.Height;
        rectangle.Offset((float) (((double) targetSize.Width - (double) rectangle.Width) / 2.0), (float) (((double) targetSize.Height - (double) rectangle.Height) / 2.0));
      }
      return rectangle;
    }

    public static RectangleF ToRectangle(
      this SizeF size,
      RectangleF targetBounds,
      RectangleScaleMode mode = RectangleScaleMode.Center)
    {
      RectangleF rectangle = size.ToRectangle(targetBounds.Size, mode);
      rectangle.Offset(targetBounds.Location);
      return rectangle;
    }

    public static Rectangle ToRectangle(this Size size, Size targetSize, RectangleScaleMode mode = RectangleScaleMode.Center)
    {
      return Rectangle.Truncate(((SizeF) size).ToRectangle((SizeF) targetSize, mode));
    }

    public static Rectangle ToRectangle(
      this Size size,
      Rectangle targetBounds,
      RectangleScaleMode mode = RectangleScaleMode.Center)
    {
      return Rectangle.Truncate(((SizeF) size).ToRectangle((RectangleF) targetBounds, mode));
    }

    public static Rectangle Align(this Size size, Rectangle bounds, ContentAlignment alignment)
    {
      return size.ToRectangle().Align(bounds, alignment);
    }

    public static Size Rotate(this Size size, ImageRotation rotation)
    {
      return rotation == ImageRotation.Rotate270 || rotation == ImageRotation.Rotate90 ? new Size(size.Height, size.Width) : size;
    }
  }
}

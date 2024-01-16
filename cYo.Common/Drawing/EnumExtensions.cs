// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.EnumExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class EnumExtensions
  {
    private static ImageRotation FromDegrees(int degrees)
    {
      degrees %= 360;
      if (degrees < 0)
        degrees += 360;
      return (ImageRotation) (degrees / 90);
    }

    public static ImageRotation RotateRight(this ImageRotation rotate)
    {
      return (ImageRotation) ((uint) (rotate + (byte) 1) % 4U);
    }

    public static ImageRotation RotateLeft(this ImageRotation rotate)
    {
      return (ImageRotation) ((uint) (rotate - (byte) 1 + (byte) 4) % 4U);
    }

    public static ImageRotation Add(this ImageRotation rotate, int degrees)
    {
      return EnumExtensions.FromDegrees(rotate.ToDegrees() + degrees);
    }

    public static int ToDegrees(this ImageRotation rotation)
    {
      switch (rotation)
      {
        case ImageRotation.Rotate90:
          return 90;
        case ImageRotation.Rotate180:
          return 180;
        case ImageRotation.Rotate270:
          return 270;
        default:
          return 0;
      }
    }

    public static StringAlignment ToAlignment(this ContentAlignment ca)
    {
      switch (ca)
      {
        case ContentAlignment.TopCenter:
        case ContentAlignment.MiddleCenter:
        case ContentAlignment.BottomCenter:
          return StringAlignment.Center;
        case ContentAlignment.TopRight:
        case ContentAlignment.MiddleRight:
        case ContentAlignment.BottomRight:
          return StringAlignment.Far;
        default:
          return StringAlignment.Near;
      }
    }

    public static StringAlignment ToLineAlignment(this ContentAlignment ca)
    {
      switch (ca)
      {
        case ContentAlignment.MiddleLeft:
        case ContentAlignment.MiddleCenter:
        case ContentAlignment.MiddleRight:
          return StringAlignment.Center;
        case ContentAlignment.BottomLeft:
        case ContentAlignment.BottomCenter:
        case ContentAlignment.BottomRight:
          return StringAlignment.Far;
        default:
          return StringAlignment.Near;
      }
    }

    public static ContentAlignment FromAlignments(
      StringAlignment alignment,
      StringAlignment lineAlignment)
    {
      switch (alignment)
      {
        case StringAlignment.Near:
          switch (lineAlignment)
          {
            case StringAlignment.Near:
              return ContentAlignment.TopLeft;
            case StringAlignment.Center:
              return ContentAlignment.MiddleLeft;
            case StringAlignment.Far:
              return ContentAlignment.BottomLeft;
          }
          break;
        case StringAlignment.Center:
          switch (lineAlignment)
          {
            case StringAlignment.Near:
              return ContentAlignment.TopCenter;
            case StringAlignment.Center:
              return ContentAlignment.MiddleCenter;
            case StringAlignment.Far:
              return ContentAlignment.BottomCenter;
          }
          break;
        case StringAlignment.Far:
          switch (lineAlignment)
          {
            case StringAlignment.Near:
              return ContentAlignment.TopRight;
            case StringAlignment.Center:
              return ContentAlignment.MiddleRight;
            case StringAlignment.Far:
              return ContentAlignment.BottomRight;
          }
          break;
      }
      return ContentAlignment.TopLeft;
    }
  }
}

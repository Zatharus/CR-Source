// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.PathUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Drawing;
using System.Drawing.Drawing2D;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class PathUtility
  {
    public static GraphicsPath GetArrowPath(Rectangle bounds, bool up)
    {
      GraphicsPath arrowPath = new GraphicsPath();
      if (up)
      {
        arrowPath.AddLine(bounds.Left, bounds.Bottom, bounds.Left + bounds.Width / 2, bounds.Top);
        arrowPath.AddLine(bounds.Left + bounds.Width / 2, bounds.Top, bounds.Right, bounds.Bottom);
      }
      else
      {
        arrowPath.AddLine(bounds.Left, bounds.Top, bounds.Left + bounds.Width / 2, bounds.Bottom);
        arrowPath.AddLine(bounds.Left + bounds.Width / 2, bounds.Bottom, bounds.Right, bounds.Top);
      }
      arrowPath.CloseFigure();
      return arrowPath;
    }

    public static GraphicsPath ConvertToPath(
      this Rectangle bounds,
      int roundedWidthTopLeft,
      int roundedHeightTopLeft,
      int roundedWidthTopRight,
      int roundedHeightTopRight,
      int roundedWidthBottomRight,
      int roundedHeightBottomRight,
      int roundedWidthBottomLeft,
      int roundedHeightBottomLeft)
    {
      return PathUtility.CreatePath(bounds.X, bounds.Y, bounds.Width, bounds.Height, roundedWidthTopLeft, roundedHeightTopLeft, roundedWidthTopRight, roundedHeightTopRight, roundedWidthBottomRight, roundedHeightBottomRight, roundedWidthBottomLeft, roundedHeightBottomLeft);
    }

    public static GraphicsPath ConvertToPath(
      this Rectangle bounds,
      int roundedWidth,
      int roundedHeight)
    {
      return PathUtility.CreatePath(bounds.X, bounds.Y, bounds.Width, bounds.Height, roundedWidth, roundedHeight, roundedWidth, roundedHeight, roundedWidth, roundedHeight, roundedWidth, roundedHeight);
    }

    public static GraphicsPath CreatePath(
      int left,
      int top,
      int width,
      int height,
      int roundedWidthTopLeft,
      int roundedHeightTopLeft,
      int roundedWidthTopRight,
      int roundedHeightTopRight,
      int roundedWidthBottomRight,
      int roundedHeightBottomRight,
      int roundedWidthBottomLeft,
      int roundedHeightBottomLeft)
    {
      GraphicsPath path = new GraphicsPath();
      int num1 = left + width;
      int num2 = top + height;
      if (roundedWidthTopLeft == 0 || roundedHeightTopLeft == 0)
        path.AddLine(left, top, left + 1, top);
      else
        path.AddArc(left, top, roundedWidthTopLeft * 2, roundedHeightTopLeft * 2, 180f, 90f);
      if (roundedWidthTopRight == 0 || roundedHeightTopRight == 0)
        path.AddLine(num1, top, num1, top + 1);
      else
        path.AddArc(num1 - roundedWidthTopRight * 2, top, roundedWidthTopRight * 2, roundedHeightTopRight * 2, 270f, 90f);
      if (roundedWidthBottomRight == 0 || roundedHeightBottomRight == 0)
        path.AddLine(num1, num2, num1 - 1, num2);
      else
        path.AddArc(num1 - roundedWidthBottomRight * 2, num2 - roundedHeightBottomRight * 2, roundedWidthBottomRight * 2, roundedHeightBottomRight * 2, 0.0f, 90f);
      if (roundedWidthBottomLeft == 0 || roundedHeightBottomLeft == 0)
        path.AddLine(left, num2, left, num2 - 1);
      else
        path.AddArc(left, num2 - roundedHeightBottomLeft * 2, roundedWidthBottomLeft * 2, roundedHeightBottomLeft * 2, 90f, 90f);
      path.CloseFigure();
      return path;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.PointExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class PointExtensions
  {
    public static Rectangle GetBounds(this IEnumerable<Point> points)
    {
      if (points == null)
        throw new ArgumentNullException();
      if (points.Count<Point>() == 0)
        return Rectangle.Empty;
      Point location = new Point(int.MaxValue, int.MaxValue);
      Point point1 = new Point(int.MinValue, int.MinValue);
      foreach (Point point2 in points)
      {
        location.X = Math.Min(point2.X, location.X);
        location.Y = Math.Min(point2.Y, location.Y);
        point1.X = Math.Max(point2.X, point1.X);
        point1.Y = Math.Max(point2.Y, point1.Y);
      }
      return new Rectangle(location, new Size(point1.X - location.X, point1.Y - location.Y));
    }

    public static Point Multiply(this Point point, int mx, int my)
    {
      return new Point(point.X * mx, point.Y * my);
    }

    public static PointF Multiply(this PointF point, float mx, float my)
    {
      return new PointF(point.X * mx, point.Y * my);
    }

    public static Point Add(this Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

    public static Point Add(this Point a, int x, int y) => new Point(a.X + x, a.Y + y);

    public static PointF Add(this PointF a, PointF b) => new PointF(a.X + b.X, a.Y + b.Y);

    public static Point Subtract(this Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);

    public static PointF Subtract(this PointF a, PointF b) => new PointF(a.X - b.X, a.Y - b.Y);

    public static Point Clip(this Point pt, Rectangle clip)
    {
      pt.X = pt.X.Clamp(clip.Left, clip.Right - 1);
      pt.Y = pt.Y.Clamp(clip.Top, clip.Bottom - 1);
      return pt;
    }

    public static int Distance(this Point a, Point b)
    {
      int num1 = a.X - b.X;
      int num2 = a.Y - b.Y;
      return (int) Math.Sqrt((double) (num1 * num1 + num2 * num2));
    }

    public static float Distance(this PointF a, PointF b)
    {
      float num1 = a.X - b.X;
      float num2 = a.Y - b.Y;
      return (float) Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.RectangleExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class RectangleExtensions
  {
    public static Rectangle Create(Point firstCorner, Point secondCorner)
    {
      return Rectangle.FromLTRB(Math.Min(firstCorner.X, secondCorner.X), Math.Min(firstCorner.Y, secondCorner.Y), Math.Max(firstCorner.X, secondCorner.X), Math.Max(firstCorner.Y, secondCorner.Y));
    }

    public static Rectangle Create(Point center, int radiusX, int radiusY)
    {
      return Rectangle.FromLTRB(center.X - radiusX, center.Y - radiusY, center.X + radiusX, center.Y + radiusY);
    }

    public static RectangleF Create(PointF firstCorner, PointF secondCorner)
    {
      return RectangleF.FromLTRB(Math.Min(firstCorner.X, secondCorner.X), Math.Min(firstCorner.Y, secondCorner.Y), Math.Max(firstCorner.X, secondCorner.X), Math.Max(firstCorner.Y, secondCorner.Y));
    }

    public static bool IsEmpty(this Rectangle rc) => rc.Width == 0 || rc.Height == 0;

    public static Point GetCenter(this Rectangle rc)
    {
      return new Point(rc.X + rc.Width / 2, rc.Y + rc.Height / 2);
    }

    public static PointF GetCenter(this RectangleF rc)
    {
      return new PointF(rc.X + rc.Width / 2f, rc.Y + rc.Height / 2f);
    }

    public static int IndexOfBestFit(this IEnumerable<Rectangle> rectangles, Rectangle test)
    {
      int num1 = -1;
      int num2 = 0;
      int num3 = 0;
      foreach (Rectangle rectangle1 in rectangles)
      {
        Rectangle rectangle2 = Rectangle.Intersect(rectangle1, test);
        int num4 = rectangle2.Width * rectangle2.Height;
        if (num4 > num3)
        {
          num3 = num4;
          num1 = num2;
        }
        ++num2;
      }
      return num1;
    }

    public static PointF TopLeft(this RectangleF rect) => rect.Location;

    public static PointF TopRight(this RectangleF rect) => new PointF(rect.Right, rect.Top);

    public static PointF BottomLeft(this RectangleF rect) => new PointF(rect.Left, rect.Bottom);

    public static PointF BottomRight(this RectangleF rect) => new PointF(rect.Right, rect.Bottom);

    public static Point TopLeft(this Rectangle rect) => rect.Location;

    public static Point TopRight(this Rectangle rect) => new Point(rect.Right, rect.Top);

    public static Point BottomLeft(this Rectangle rect) => new Point(rect.Left, rect.Bottom);

    public static Point BottomRight(this Rectangle rect) => new Point(rect.Right, rect.Bottom);

    public static Rectangle Pad(this Rectangle rc, Padding margin)
    {
      rc.X += margin.Left;
      rc.Y += margin.Top;
      rc.Width -= margin.Horizontal;
      rc.Height -= margin.Vertical;
      return rc;
    }

    public static Rectangle Pad(this Rectangle rc, int all) => rc.Pad(new Padding(all));

    public static RectangleF Pad(this RectangleF rc, Padding margin)
    {
      rc.X += (float) margin.Left;
      rc.Y += (float) margin.Top;
      rc.Width -= (float) margin.Horizontal;
      rc.Height -= (float) margin.Vertical;
      return rc;
    }

    public static RectangleF Pad(this RectangleF rc, int all) => rc.Pad(new Padding(all));

    public static Rectangle Pad(this Rectangle rc, int left, int top, int right = 0, int bottom = 0)
    {
      return rc.Pad(new Padding(left, top, right, bottom));
    }

    public static Padding GetPadding(this Rectangle inner, Rectangle outer)
    {
      return new Padding(inner.Left - outer.Left, inner.Top - outer.Top, outer.Right - inner.Right, outer.Bottom - inner.Bottom);
    }

    public static Padding GetPadding(this Rectangle inner, Size outerSize)
    {
      return inner.GetPadding(new Rectangle(Point.Empty, outerSize));
    }

    public static RectangleF Grow(this RectangleF rect, float width, float height)
    {
      rect.Inflate(width, height);
      return rect;
    }

    public static RectangleF Grow(this RectangleF rect, float n) => rect.Grow(n, n);

    public static IEnumerable<RectangleF> GetBorderRectangles(
      RectangleF rd,
      RectangleF rs,
      Rectangle ir)
    {
      if ((double) rs.Width == 0.0 || (double) rs.Height == 0.0)
        return (IEnumerable<RectangleF>) new RectangleF[0];
      float num1 = rd.Width / rs.Width;
      float num2 = rd.Height / rs.Height;
      RectangleF rectangleF1 = RectangleF.Empty;
      RectangleF rectangleF2 = RectangleF.Empty;
      RectangleF rectangleF3 = RectangleF.Empty;
      RectangleF rectangleF4 = RectangleF.Empty;
      if ((double) rs.Bottom > (double) ir.Bottom)
      {
        float num3 = rs.Bottom - (float) ir.Bottom;
        rectangleF3 = new RectangleF(rd.Left, rd.Bottom - num2 * num3, rd.Height, num2 * num3);
      }
      if ((double) rs.Top < (double) ir.Top)
      {
        float num4 = (float) ir.Top - rs.Top;
        rectangleF1 = new RectangleF(rd.Left, rd.Top, rd.Width, num4 * num2);
      }
      if ((double) rs.Right > (double) ir.Right)
      {
        float num5 = rs.Right - (float) ir.Right;
        rectangleF4 = new RectangleF(rd.Right - num1 * num5, rd.Top, num1 * num5, rd.Height);
      }
      if ((double) rs.Left < (double) ir.Left)
      {
        float num6 = (float) ir.Left - rs.Left;
        rectangleF2 = new RectangleF(rd.Left, rd.Top, num1 * num6, rd.Height);
      }
      if (!rectangleF1.IsEmpty)
      {
        float num7 = Math.Max(rd.Top, rectangleF1.Bottom);
        if (!rectangleF4.IsEmpty)
        {
          rectangleF4.Y = num7;
          rectangleF4.Height = rd.Height - (rd.Top - num7);
        }
        if (!rectangleF2.IsEmpty)
        {
          rectangleF2.Y = num7;
          rectangleF2.Height = rd.Height - (rd.Top - num7);
        }
      }
      if (!rectangleF3.IsEmpty)
      {
        float num8 = Math.Min(rd.Bottom, rectangleF3.Top);
        if (!rectangleF4.IsEmpty)
          rectangleF4.Height = num8;
        if (!rectangleF2.IsEmpty)
          rectangleF2.Height = num8;
      }
      List<RectangleF> rectangleFList = new List<RectangleF>(4);
      if (!rectangleF2.IsEmpty)
        rectangleFList.Add(rectangleF2);
      if (!rectangleF4.IsEmpty)
        rectangleFList.Add(rectangleF4);
      if (!rectangleF1.IsEmpty)
        rectangleFList.Add(rectangleF1);
      if (!rectangleF3.IsEmpty)
        rectangleFList.Add(rectangleF3);
      return (IEnumerable<RectangleF>) rectangleFList.ToArray();
    }

    public static IEnumerable<RectangleF> GetSubRectangles(
      this RectangleF rect,
      RectangleF sub,
      bool clip)
    {
      int nx = (int) (((double) rect.Width + (double) sub.Width - 1.0) / (double) sub.Width);
      int ny = (int) (((double) rect.Height + (double) sub.Height - 1.0) / (double) sub.Height);
      RectangleF r = new RectangleF(rect.Location, sub.Size);
      for (int x = 0; x < nx; ++x)
      {
        for (int y = 0; y < ny; ++y)
        {
          yield return clip ? RectangleF.Intersect(rect, r) : r;
          r.Y += sub.Height;
        }
        r.X += sub.Width;
        r.Y = rect.Y;
      }
    }

    public static Rectangle Align(
      this Rectangle rectangle,
      Rectangle bounds,
      ContentAlignment alignment)
    {
      if (alignment <= ContentAlignment.MiddleCenter)
      {
        switch (alignment - 1)
        {
          case (ContentAlignment) 0:
          case ContentAlignment.TopLeft:
          case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
            rectangle.Y = bounds.Y;
            goto label_10;
          case ContentAlignment.TopCenter:
            goto label_10;
          default:
            if (alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.MiddleCenter)
              break;
            goto label_10;
        }
      }
      else
      {
        if (alignment <= ContentAlignment.BottomLeft)
        {
          if (alignment != ContentAlignment.MiddleRight)
          {
            if (alignment != ContentAlignment.BottomLeft)
              goto label_10;
          }
          else
            goto label_8;
        }
        else if (alignment != ContentAlignment.BottomCenter && alignment != ContentAlignment.BottomRight)
          goto label_10;
        rectangle.Y = bounds.Y + bounds.Height - rectangle.Height;
        goto label_10;
      }
label_8:
      rectangle.Y = bounds.Y + (bounds.Height - rectangle.Height) / 2;
label_10:
      if (alignment <= ContentAlignment.MiddleCenter)
      {
        switch (alignment - 1)
        {
          case (ContentAlignment) 0:
            break;
          case ContentAlignment.TopLeft:
            goto label_20;
          case ContentAlignment.TopCenter:
            goto label_22;
          case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
            goto label_21;
          default:
            if (alignment != ContentAlignment.MiddleLeft)
            {
              if (alignment == ContentAlignment.MiddleCenter)
                goto label_20;
              else
                goto label_22;
            }
            else
              break;
        }
      }
      else if (alignment <= ContentAlignment.BottomLeft)
      {
        if (alignment != ContentAlignment.MiddleRight)
        {
          if (alignment != ContentAlignment.BottomLeft)
            goto label_22;
        }
        else
          goto label_21;
      }
      else if (alignment != ContentAlignment.BottomCenter)
      {
        if (alignment == ContentAlignment.BottomRight)
          goto label_21;
        else
          goto label_22;
      }
      else
        goto label_20;
      rectangle.X = bounds.X;
      goto label_22;
label_20:
      rectangle.X = bounds.X + (bounds.Width - rectangle.Width) / 2;
      goto label_22;
label_21:
      rectangle.X = bounds.X + bounds.Width - rectangle.Width;
label_22:
      return rectangle;
    }

    public static Rectangle Center(this Rectangle rectangle, Rectangle bounds)
    {
      return rectangle.Align(bounds, ContentAlignment.MiddleCenter);
    }

    public static Rectangle AlignHorizontal(
      this Rectangle rectangle,
      int offset,
      StringAlignment alignment)
    {
      switch (alignment)
      {
        case StringAlignment.Near:
          return new Rectangle(offset - rectangle.Width, rectangle.Y, rectangle.Width, rectangle.Height);
        case StringAlignment.Far:
          return new Rectangle(offset, rectangle.Y, rectangle.Width, rectangle.Height);
        default:
          return new Rectangle(offset - rectangle.Width / 2, rectangle.Y, rectangle.Width, rectangle.Height);
      }
    }

    public static RectangleF Align(
      this RectangleF rectangle,
      RectangleF bounds,
      ContentAlignment alignment)
    {
      if (alignment <= ContentAlignment.MiddleCenter)
      {
        switch (alignment - 1)
        {
          case (ContentAlignment) 0:
          case ContentAlignment.TopLeft:
          case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
            rectangle.Y = bounds.Y;
            goto label_10;
          case ContentAlignment.TopCenter:
            goto label_10;
          default:
            if (alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.MiddleCenter)
              break;
            goto label_10;
        }
      }
      else
      {
        if (alignment <= ContentAlignment.BottomLeft)
        {
          if (alignment != ContentAlignment.MiddleRight)
          {
            if (alignment != ContentAlignment.BottomLeft)
              goto label_10;
          }
          else
            goto label_8;
        }
        else if (alignment != ContentAlignment.BottomCenter && alignment != ContentAlignment.BottomRight)
          goto label_10;
        rectangle.Y = bounds.Y + bounds.Height - rectangle.Height;
        goto label_10;
      }
label_8:
      rectangle.Y = bounds.Y + (float) (((double) bounds.Height - (double) rectangle.Height) / 2.0);
label_10:
      if (alignment <= ContentAlignment.MiddleCenter)
      {
        switch (alignment - 1)
        {
          case (ContentAlignment) 0:
            break;
          case ContentAlignment.TopLeft:
            goto label_20;
          case ContentAlignment.TopCenter:
            goto label_22;
          case ContentAlignment.TopLeft | ContentAlignment.TopCenter:
            goto label_21;
          default:
            if (alignment != ContentAlignment.MiddleLeft)
            {
              if (alignment == ContentAlignment.MiddleCenter)
                goto label_20;
              else
                goto label_22;
            }
            else
              break;
        }
      }
      else if (alignment <= ContentAlignment.BottomLeft)
      {
        if (alignment != ContentAlignment.MiddleRight)
        {
          if (alignment != ContentAlignment.BottomLeft)
            goto label_22;
        }
        else
          goto label_21;
      }
      else if (alignment != ContentAlignment.BottomCenter)
      {
        if (alignment == ContentAlignment.BottomRight)
          goto label_21;
        else
          goto label_22;
      }
      else
        goto label_20;
      rectangle.X = bounds.X;
      goto label_22;
label_20:
      rectangle.X = bounds.X + (float) (((double) bounds.Width - (double) rectangle.Width) / 2.0);
      goto label_22;
label_21:
      rectangle.X = bounds.X + bounds.Width - rectangle.Width;
label_22:
      return rectangle;
    }

    public static RectangleF Center(this RectangleF rectangle, RectangleF bounds)
    {
      return rectangle.Align(bounds, ContentAlignment.MiddleCenter);
    }

    public static RectangleF AlignHorizontal(
      this RectangleF rectangle,
      float offset,
      StringAlignment alignment)
    {
      switch (alignment)
      {
        case StringAlignment.Near:
          return new RectangleF(offset - rectangle.Width, rectangle.Y, rectangle.Width, rectangle.Height);
        case StringAlignment.Far:
          return new RectangleF(offset, rectangle.Y, rectangle.Width, rectangle.Height);
        default:
          return new RectangleF(offset - rectangle.Width / 2f, rectangle.Y, rectangle.Width, rectangle.Height);
      }
    }

    public static Rectangle Scale(this Rectangle rect, float scaleX, float scaleY)
    {
      rect.X = (int) ((double) rect.X * (double) scaleX);
      rect.Y = (int) ((double) rect.Y * (double) scaleY);
      rect.Width = (int) ((double) rect.Width * (double) scaleX);
      rect.Height = (int) ((double) rect.Height * (double) scaleY);
      return rect;
    }

    public static Rectangle Scale(this Rectangle rect, float scale) => rect.Scale(scale, scale);

    public static Rectangle Fit(this Rectangle rect, Rectangle fit, ScaleMode scaleMode = ScaleMode.FitAll)
    {
      return rect.Scale(rect.Size.GetScale(fit.Size, scaleMode)).Align(fit, ContentAlignment.MiddleCenter);
    }

    public static RectangleF Scale(this RectangleF rect, float scaleX, float scaleY)
    {
      rect.X *= scaleX;
      rect.Y *= scaleY;
      rect.Width *= scaleX;
      rect.Height *= scaleY;
      return rect;
    }

    public static RectangleF Scale(this RectangleF rect, float scale) => rect.Scale(scale, scale);

    public static RectangleF Fit(this RectangleF rect, RectangleF fit, ScaleMode scaleMode = ScaleMode.FitAll)
    {
      return rect.Scale(rect.Size.GetScale(fit.Size, scaleMode)).Align(fit, ContentAlignment.MiddleCenter);
    }

    public static Rectangle Round(this RectangleF rect) => Rectangle.Round(rect);

    public static RectangleF Add(this RectangleF r, float x, float y)
    {
      r.Offset(x, y);
      return r;
    }

    public static Rectangle Add(this Rectangle r, int x, int y)
    {
      r.Offset(x, y);
      return r;
    }

    public static Rectangle Rotate(this Rectangle rectangle, Matrix rotationMatrix)
    {
      if (rotationMatrix == null)
        throw new ArgumentNullException();
      Point[] pts = new Point[4]
      {
        new Point(rectangle.X, rectangle.Y),
        new Point(rectangle.Right, rectangle.Y),
        new Point(rectangle.Right, rectangle.Bottom),
        new Point(rectangle.X, rectangle.Bottom)
      };
      rotationMatrix.TransformPoints(pts);
      return ((IEnumerable<Point>) pts).GetBounds();
    }

    public static Rectangle Rotate(this Rectangle rectangle, int angle)
    {
      return rectangle.Rotate(MatrixUtility.GetRotationMatrix(rectangle.Size, angle));
    }

    public static Rectangle Rotate(this Rectangle rectangle, ImageRotation rotation)
    {
      return rectangle.Rotate(rotation.ToDegrees());
    }

    public static PointF[] ToPoints(this RectangleF rect)
    {
      return new PointF[4]
      {
        rect.TopLeft(),
        rect.TopRight(),
        rect.BottomLeft(),
        rect.BottomRight()
      };
    }

    public static PointF[] ToLineStrip(this RectangleF rect)
    {
      return new PointF[5]
      {
        rect.TopLeft(),
        rect.TopRight(),
        rect.BottomRight(),
        rect.BottomLeft(),
        rect.TopLeft()
      };
    }

    public static RectangleF ToRectangle(this IEnumerable<PointF> points)
    {
      return RectangleF.FromLTRB(points.Min<PointF>((Func<PointF, float>) (pt => pt.X)), points.Min<PointF>((Func<PointF, float>) (pt => pt.Y)), points.Max<PointF>((Func<PointF, float>) (pt => pt.X)), points.Max<PointF>((Func<PointF, float>) (pt => pt.Y)));
    }

    public static Point[] ToPoints(this Rectangle rect)
    {
      return new Point[4]
      {
        rect.TopLeft(),
        rect.TopRight(),
        rect.BottomLeft(),
        rect.BottomRight()
      };
    }

    public static Rectangle ToRectangle(this IEnumerable<Point> points)
    {
      return Rectangle.FromLTRB(points.Min<Point>((Func<Point, int>) (pt => pt.X)), points.Min<Point>((Func<Point, int>) (pt => pt.Y)), points.Max<Point>((Func<Point, int>) (pt => pt.X)), points.Max<Point>((Func<Point, int>) (pt => pt.Y)));
    }

    public static RectangleF Union(this RectangleF a, RectangleF b)
    {
      return !a.IsEmpty ? RectangleF.Union(a, b) : b;
    }

    public static Rectangle Union(this Rectangle a, Rectangle b)
    {
      return !a.IsEmpty ? Rectangle.Union(a, b) : b;
    }

    public static Rectangle Subtract(this Rectangle a, Rectangle b)
    {
      Rectangle rectangle = Rectangle.Intersect(a, b);
      if (rectangle.IsEmpty)
        return a;
      if (rectangle.Top == a.Top)
        a = a.Pad(0, rectangle.Height);
      else if (rectangle.Bottom == a.Bottom)
        a = a.Pad(0, 0, bottom: rectangle.Height);
      if (rectangle.Left == a.Left)
        a = a.Pad(rectangle.Width, 0);
      else if (rectangle.Right == a.Right)
        a = a.Pad(0, 0, rectangle.Width);
      return a;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.XHtmlRenderer
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Presentation.Ceco.Builders;
using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public static class XHtmlRenderer
  {
    private static Cache<XHtmlRenderer.BodyKey, BodyBlock> bodyCache = new Cache<XHtmlRenderer.BodyKey, BodyBlock>(100);

    public static void DrawString(
      Graphics graphics,
      string s,
      Font font,
      Color foreColor,
      int x,
      int y)
    {
      XHtmlRenderer.DrawString(graphics, s, font, foreColor, new Point(x, y));
    }

    public static void DrawString(
      Graphics graphics,
      string s,
      Font font,
      Color foreColor,
      Point location)
    {
      XHtmlRenderer.DrawString(graphics, s, font, foreColor, new Rectangle(location, Size.Empty));
    }

    public static void DrawString(
      Graphics graphics,
      string s,
      Font font,
      Color foreColor,
      Rectangle layoutRectangle)
    {
      XHtmlRenderer.DrawString(graphics, s, font, foreColor, layoutRectangle, ContentAlignment.TopLeft);
    }

    public static void DrawString(
      Graphics graphics,
      string s,
      Font font,
      Color foreColor,
      Rectangle layoutRectangle,
      StringFormat sf)
    {
      XHtmlRenderer.DrawString(graphics, s, font, foreColor, layoutRectangle, EnumExtensions.FromAlignments(sf.Alignment, sf.LineAlignment));
    }

    public static void DrawString(
      Graphics graphics,
      string s,
      Font font,
      Color foreColor,
      Rectangle layoutRectangle,
      ContentAlignment align)
    {
      using (IItemLock<BodyBlock> body = XHtmlRenderer.GetBody(s, font))
      {
        BodyBlock bodyBlock = body.Item;
        bodyBlock.ForeColor = foreColor;
        if (layoutRectangle.Width <= 0)
          layoutRectangle.Width = int.MaxValue;
        if (layoutRectangle.Height <= 0)
          layoutRectangle.Height = int.MaxValue;
        bodyBlock.Align = align.ToAlignment().ToHorizontalAlignment();
        VerticalAlignment verticalAlignment = align.ToLineAlignment().ToVerticalAlignment();
        if (verticalAlignment != VerticalAlignment.None || verticalAlignment != VerticalAlignment.Top)
        {
          bodyBlock.Measure(graphics, layoutRectangle.Width);
          if (verticalAlignment != VerticalAlignment.Middle)
          {
            if (verticalAlignment != VerticalAlignment.Bottom)
              throw new ArgumentOutOfRangeException();
            layoutRectangle.Y += layoutRectangle.Bottom - bodyBlock.ActualSize.Height;
          }
          else
            layoutRectangle.Y += (layoutRectangle.Height - bodyBlock.ActualSize.Height) / 2;
        }
        bodyBlock.Bounds = new Rectangle(Point.Empty, layoutRectangle.Size);
        bodyBlock.Draw(graphics, layoutRectangle.Location);
      }
    }

    public static Size MeasureString(Graphics graphics, string s, Font font)
    {
      return XHtmlRenderer.MeasureString(graphics, s, font, 0);
    }

    public static Size MeasureString(Graphics graphics, string s, Font font, int width)
    {
      using (IItemLock<BodyBlock> body = XHtmlRenderer.GetBody(s, font))
      {
        BodyBlock bodyBlock = body.Item;
        if (width <= 0)
          width = int.MaxValue;
        bodyBlock.Measure(graphics, width);
        return bodyBlock.ActualSize;
      }
    }

    private static IItemLock<BodyBlock> GetBody(string text, Font font)
    {
      Cache<XHtmlRenderer.BodyKey, BodyBlock> bodyCache = XHtmlRenderer.bodyCache;
      XHtmlRenderer.BodyKey key = new XHtmlRenderer.BodyKey();
      key.Text = text;
      key.Font = font;
      Func<XHtmlRenderer.BodyKey, BodyBlock> create = (Func<XHtmlRenderer.BodyKey, BodyBlock>) (bk =>
      {
        BodyBlock body = new BodyBlock();
        body.Inlines.AddRange((IEnumerable<Inline>) XHtmlParser.Parse(bk.Text).Inlines);
        body.Font = font;
        return body;
      });
      return bodyCache.LockItem(key, create);
    }

    public static HorizontalAlignment ToHorizontalAlignment(this StringAlignment align)
    {
      switch (align)
      {
        case StringAlignment.Near:
          return HorizontalAlignment.Left;
        case StringAlignment.Center:
          return HorizontalAlignment.Center;
        case StringAlignment.Far:
          return HorizontalAlignment.Right;
        default:
          return HorizontalAlignment.None;
      }
    }

    public static VerticalAlignment ToVerticalAlignment(this StringAlignment align)
    {
      switch (align)
      {
        case StringAlignment.Near:
          return VerticalAlignment.Top;
        case StringAlignment.Center:
          return VerticalAlignment.Middle;
        case StringAlignment.Far:
          return VerticalAlignment.Bottom;
        default:
          return VerticalAlignment.None;
      }
    }

    private class BodyKey
    {
      public Font Font { get; set; }

      public string Text { get; set; }

      public override bool Equals(object obj)
      {
        return obj is XHtmlRenderer.BodyKey bodyKey && bodyKey.Font == this.Font && bodyKey.Text == this.Text;
      }

      public override int GetHashCode() => this.Font.GetHashCode() ^ this.Text.GetHashCode();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.SimpleTextRenderer
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class SimpleTextRenderer
  {
    public static Rectangle DrawText(
      Graphics gr,
      IEnumerable<TextLine> textLines,
      Rectangle rect,
      int offset = 0)
    {
      return SimpleTextRenderer.WorkText(gr, textLines, rect, false, offset);
    }

    public static Rectangle MeasureText(
      Graphics gr,
      IEnumerable<TextLine> textLines,
      Rectangle rect)
    {
      return SimpleTextRenderer.WorkText(gr, textLines, rect, true, 0);
    }

    private static float MeasureFirstTab(Graphics gr, IEnumerable<TextLine> textLines)
    {
      float val2 = 0.0f;
      foreach (TextLine textLine in textLines)
      {
        if (!textLine.Separator && textLine.Format.Alignment != StringAlignment.Far)
        {
          string[] strArray = textLine.Text.Split('\t');
          if (strArray.Length > 1)
            val2 = Math.Max(gr.MeasureString(strArray[0], textLine.Font).Width, val2);
        }
      }
      return val2;
    }

    private static Rectangle WorkText(
      Graphics gr,
      IEnumerable<TextLine> textLines,
      Rectangle bounds,
      bool onlyMeassure,
      int scrollOffset)
    {
      if (gr == null || textLines == null)
        throw new ArgumentNullException();
      Rectangle rect = bounds;
      using (gr.SaveState())
      {
        Rectangle a = Rectangle.Empty;
        int dy = 0;
        float num = SimpleTextRenderer.MeasureFirstTab(gr, textLines) + 8f;
        gr.PageUnit = GraphicsUnit.Pixel;
        foreach (TextLine textLine in textLines)
        {
          if (textLine.ScrollStart && !onlyMeassure && scrollOffset != 0)
          {
            gr.SetClip(rect, CombineMode.Intersect);
            rect = rect.Pad(0, scrollOffset);
          }
          if (textLine.Separator)
          {
            dy = textLine.BeforeSpacing;
          }
          else
          {
            if (dy != 0)
            {
              SimpleTextRenderer.Space(ref rect, dy);
              dy = 0;
            }
            if (!string.IsNullOrEmpty(textLine.Text))
              SimpleTextRenderer.Space(ref rect, textLine.BeforeSpacing);
            using (Brush br = (Brush) new SolidBrush(textLine.ForeColor))
            {
              StringFormat format = textLine.Format;
              if (format.Alignment != StringAlignment.Far)
                format.SetTabStops(0.0f, new float[1]{ num });
              Rectangle b = SimpleTextRenderer.DrawString(gr, textLine.Text, textLine.Font, br, ref rect, format, onlyMeassure);
              if (rect.Height != 0)
                a = a.IsEmpty ? b : Rectangle.Union(a, b);
              else
                break;
            }
            if (!string.IsNullOrEmpty(textLine.Text))
            {
              SimpleTextRenderer.Space(ref rect, textLine.AfterSpacing);
              a.Height += textLine.AfterSpacing;
            }
          }
        }
        return a;
      }
    }

    private static void Space(ref Rectangle rect, int dy)
    {
      if (rect.Height <= 0)
        return;
      rect.Y += dy;
      rect.Height -= dy;
      if (rect.Height >= 0)
        return;
      rect.Height = 0;
    }

    private static Rectangle DrawString(
      Graphics gr,
      string text,
      Font f,
      Brush br,
      ref Rectangle rect,
      StringFormat sf,
      bool onlyMeassure)
    {
      if (string.IsNullOrEmpty(text) || rect.Height <= 0)
        return Rectangle.Empty;
      sf.FormatFlags |= StringFormatFlags.LineLimit;
      Size size = gr.MeasureString(text, f, (SizeF) rect.Size, sf).ToSize();
      if (size.Height > rect.Height)
        size.Height = rect.Height;
      if (!onlyMeassure)
        gr.DrawString(text, f, br, (RectangleF) rect, sf);
      Rectangle rectangle = new Rectangle(0, 0, size.Width, size.Height).Align(rect, EnumExtensions.FromAlignments(sf.Alignment, sf.LineAlignment));
      SimpleTextRenderer.Space(ref rect, size.Height);
      return rectangle;
    }
  }
}

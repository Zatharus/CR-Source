// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.FlowBlock
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public class FlowBlock : Block
  {
    private Size actualSize;

    public Size ActualSize
    {
      get => this.actualSize;
      set
      {
        if (this.actualSize == value)
          return;
        this.actualSize = value;
        this.OnActualSizeChanged();
      }
    }

    protected override void OnFontChanged()
    {
      base.OnFontChanged();
      this.PendingLayout = LayoutType.Full;
    }

    protected override void OnAlignChanged()
    {
      base.OnAlignChanged();
      this.PendingLayout = LayoutType.Position;
    }

    protected override void OnVAlignChanged()
    {
      base.OnVAlignChanged();
      this.PendingLayout = LayoutType.Position;
    }

    protected override void CoreMeasure(Graphics gr, int maxWidth, LayoutType tbl)
    {
      int size = this.BlockWidth.GetSize(maxWidth);
      int blockHeight = this.BlockHeight;
      int minWidth;
      this.ActualSize = this.Size = FlowBlock.Layout((IEnumerable<Inline>) this.GetSubItems(false), gr, size, out minWidth);
      if (!this.BlockWidth.IsAuto && this.Width < size)
        this.Width = minWidth = size;
      if (this.Height < blockHeight)
        this.Height = blockHeight;
      this.MinimumWidth = minWidth;
    }

    public override void Draw(Graphics gr, Point location)
    {
      base.Draw(gr, location);
      location.Offset(this.Location);
      foreach (Inline subItem in (IEnumerable<Inline>) this.GetSubItems(false))
      {
        IRender render = subItem as IRender;
        if (subItem.Visible && render != null)
        {
          Rectangle bounds = subItem.Bounds;
          bounds.Offset(location);
          if (gr.IsVisible(bounds))
            render.Draw(gr, location);
        }
      }
    }

    public event EventHandler ActualSizeChanged;

    protected virtual void OnActualSizeChanged()
    {
      if (this.ActualSizeChanged == null)
        return;
      this.ActualSizeChanged((object) this, EventArgs.Empty);
    }

    private static Size Layout(
      IEnumerable<Inline> inlines,
      Graphics gr,
      int maxWidth,
      out int minWidth)
    {
      return FlowBlock.Layout(inlines.GetEnumerator(), gr, maxWidth, out minWidth);
    }

    private static Size Layout(
      IEnumerator<Inline> inlines,
      Graphics gr,
      int maxWidth,
      out int minWidth)
    {
      Point empty = Point.Empty;
      Rectangle a = Rectangle.Empty;
      List<Inline> inlineList = new List<Inline>();
      List<Inline> lineItems = new List<Inline>();
      int leftMargin = 0;
      int rightMargin = 0;
      int leftMarginDown = 0;
      int rightMarginDown = 0;
      Inline current1 = inlines.MoveNext() ? inlines.Current : (Inline) null;
      minWidth = 0;
      while (current1 != null)
      {
        for (; current1 != null && inlines.Current is IRender current2 && current1.IsBlock && (current1.Align == HorizontalAlignment.Left || current1.Align == HorizontalAlignment.Right); current1 = inlines.MoveNext() ? inlines.Current : (Inline) null)
        {
          current1.Visible = true;
          current2.Measure(gr, maxWidth);
          inlineList.Add(current1);
        }
        foreach (Inline inline in inlineList)
        {
          inline.Y = empty.Y;
          if (inline.Align == HorizontalAlignment.Left)
          {
            inline.X = leftMargin;
            leftMargin += inline.Width;
            leftMarginDown = Math.Max(leftMarginDown, inline.Height);
          }
          else
          {
            inline.X = maxWidth - rightMargin - inline.Width;
            rightMargin += inline.Width;
            rightMarginDown = Math.Max(rightMarginDown, inline.Height);
          }
          a = a.IsEmpty ? inline.Bounds : Rectangle.Union(a, inline.Bounds);
        }
        inlineList.Clear();
        int width1 = maxWidth - leftMargin - rightMargin;
        for (; current1 != null; current1 = inlines.MoveNext() ? inlines.Current : (Inline) null)
        {
          IRender render = current1 as IRender;
          if ((current1.FlowBreak & FlowBreak.Before) != FlowBreak.None)
          {
            Rectangle b = FlowBlock.Break(current1, (ICollection<Inline>) lineItems, ref empty, ref leftMargin, ref leftMarginDown, ref rightMargin, ref rightMarginDown, ref width1);
            a = a.IsEmpty ? b : Rectangle.Union(a, b);
          }
          if (!current1.IsNode)
          {
            if (render == null || lineItems.Count == 0 && render.IsWhiteSpace)
            {
              if (render != null)
                current1.Visible = false;
            }
            else
            {
              current1.Visible = true;
              render.Measure(gr, maxWidth);
              Rectangle bounds;
              if (empty.X + current1.Bounds.Width > width1)
              {
                if (empty.X == 0)
                {
                  bounds = current1.Bounds;
                  if (bounds.Width < width1 || leftMargin != 0 || rightMargin != 0)
                    break;
                }
                else
                  break;
              }
              current1.Location = empty;
              ref Point local1 = ref empty;
              int x = local1.X;
              bounds = current1.Bounds;
              int width2 = bounds.Width;
              local1.X = x + width2;
              ref int local2 = ref minWidth;
              int val1 = minWidth;
              int num1 = leftMargin + rightMargin;
              bounds = current1.Bounds;
              int width3 = bounds.Width;
              int val2 = num1 + width3;
              int num2 = Math.Max(val1, val2);
              local2 = num2;
              lineItems.Add(current1);
            }
          }
          if ((current1.FlowBreak & FlowBreak.After) != FlowBreak.None)
            break;
        }
        Rectangle b1 = FlowBlock.Break(current1, (ICollection<Inline>) lineItems, ref empty, ref leftMargin, ref leftMarginDown, ref rightMargin, ref rightMarginDown, ref width1);
        a = a.IsEmpty ? b1 : Rectangle.Union(a, b1);
        if (current1 != null && (current1.FlowBreak & FlowBreak.After) != FlowBreak.None)
          current1 = inlines.MoveNext() ? inlines.Current : (Inline) null;
      }
      return a.Size;
    }

    private static Rectangle Break(
      Inline current,
      ICollection<Inline> lineItems,
      ref Point pt,
      ref int leftMargin,
      ref int leftMarginDown,
      ref int rightMargin,
      ref int rightMarginDown,
      ref int width)
    {
      Rectangle rectangle = FlowBlock.LayoutSpan((IEnumerable<Inline>) lineItems, width, leftMargin);
      lineItems.Clear();
      if (rectangle.Height == 0 && current != null)
        rectangle.Height = current.Font.Height;
      int val2 = rectangle.Height;
      if (current != null)
      {
        switch (current.FlowBreak & FlowBreak.BreakMask)
        {
          case FlowBreak.BreakMarginLeft:
            val2 = Math.Max(leftMarginDown, val2);
            break;
          case FlowBreak.BreakMarginRight:
            val2 = Math.Max(rightMarginDown, val2);
            break;
          case FlowBreak.BreakMarginLeftRight:
            val2 = Math.Max(Math.Max(leftMarginDown, rightMarginDown), val2);
            break;
          default:
            val2 += current.FlowBreakOffset;
            break;
        }
      }
      pt.X = 0;
      pt.Y += val2;
      leftMarginDown -= val2;
      rightMarginDown -= val2;
      if (leftMarginDown <= 0)
        leftMargin = leftMarginDown = 0;
      if (rightMarginDown <= 0)
        rightMargin = rightMarginDown = 0;
      return rectangle;
    }

    private static Rectangle LayoutSpan(IEnumerable<Inline> span, int width, int leftMargin)
    {
      int val2_1 = 0;
      int val2_2 = 0;
      HorizontalAlignment horizontalAlignment = HorizontalAlignment.None;
      Rectangle a = Rectangle.Empty;
      foreach (Inline inline in span)
      {
        val2_2 = Math.Max(inline.BaseLine, val2_2);
        val2_1 = Math.Max(inline.Height, val2_1);
        horizontalAlignment = inline.Align;
        a = a.IsEmpty ? inline.Bounds : Rectangle.Union(a, inline.Bounds);
      }
      int num;
      switch (horizontalAlignment)
      {
        case HorizontalAlignment.Center:
          num = (width - a.Width) / 2;
          break;
        case HorizontalAlignment.Right:
          num = width - a.Width;
          break;
        default:
          num = leftMargin;
          break;
      }
      a = Rectangle.Empty;
      foreach (Inline inline in span)
      {
        switch (inline.BaseAlign)
        {
          case BaseAlignment.Bottom:
            inline.Y += val2_1 - inline.Height + inline.DescentHeight;
            break;
          case BaseAlignment.Top:
            inline.Y += -inline.DescentHeight;
            break;
          case BaseAlignment.Center:
            inline.Y += (val2_1 - inline.Height) / 2;
            break;
          default:
            inline.Y += val2_2 - inline.BaseLine;
            break;
        }
        inline.X += num;
        a = a.IsEmpty ? inline.Bounds : Rectangle.Union(a, inline.Bounds);
      }
      return a;
    }
  }
}

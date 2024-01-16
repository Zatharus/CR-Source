// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Table
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public class Table : Block
  {
    private const int MaxTableSize = 64;
    private int cellSpacing = 2;
    private int cellPadding = 1;

    protected override void CoreMeasure(Graphics gr, int maxWidth, LayoutType tbl)
    {
      int border = this.Border >= 0 ? this.Border : 0;
      int num1 = border > 0 ? 1 : 0;
      int num2 = this.BlockWidth.GetSize(maxWidth) - 2 * border;
      int length1 = 0;
      int length2 = 0;
      int padding = this.cellPadding + num1;
      int[] numArray1 = new int[64];
      Table.Cell[,] cellArray = new Table.Cell[64, 64];
      foreach (Table.Row inline1 in (Collection<Inline>) this.Inlines)
      {
        int index1 = 0;
        int num3 = 0;
        foreach (Table.Cell inline2 in (Collection<Inline>) inline1.Inlines)
        {
          while (numArray1[index1] > 0)
            ++index1;
          cellArray[index1, length1] = inline2;
          inline2.Measure(gr, num2);
          num3 = index1;
          int num4 = 0;
          while (num4 < inline2.ColumSpan)
          {
            numArray1[index1] = inline2.RowSpan;
            ++num4;
            ++index1;
          }
        }
        for (int index2 = 0; index2 < numArray1.Length; ++index2)
        {
          if (numArray1[index2] > 0)
            --numArray1[index2];
        }
        length2 = Math.Max(num3 + 1, length2);
        ++length1;
      }
      int[] numArray2 = new int[length2];
      int[] minWidths = new int[length2];
      SizeValue blockWidth;
      for (int index3 = 0; index3 < length1; ++index3)
      {
        for (int index4 = 0; index4 < length2; ++index4)
        {
          Table.Cell cell = cellArray[index4, index3];
          if (cell != null && cell.ColumSpan == 1)
          {
            blockWidth = cell.BlockWidth;
            int val2_1 = blockWidth.IsFixed ? cell.Width - 2 * padding : cell.Width;
            if (val2_1 < cell.MinimumWidth)
              val2_1 = cell.MinimumWidth;
            blockWidth = cell.BlockWidth;
            int val2_2 = blockWidth.IsFixed ? val2_1 : cell.MinimumWidth;
            numArray2[index4] = Math.Max(numArray2[index4], val2_1);
            minWidths[index4] = Math.Max(minWidths[index4], val2_2);
          }
        }
      }
      for (int index5 = 0; index5 < length1; ++index5)
      {
        for (int index6 = 0; index6 < length2; ++index6)
        {
          Table.Cell cell = cellArray[index6, index5];
          if (cell != null && cell.ColumSpan != 1)
          {
            int spanSize = Table.GetSpanSize(numArray2, index6, cell.ColumSpan, this.cellSpacing, padding);
            if (cell.Width > spanSize)
              Table.DistributeWidth(cell.Width, index6, cell.ColumSpan, numArray2, minWidths, spanSize);
          }
        }
      }
      int totalWidth1 = Table.GetSpanSize(numArray2, 0, numArray2.Length, this.cellSpacing, padding) + 2 * this.cellSpacing;
      blockWidth = this.BlockWidth;
      if (blockWidth.IsFixed || totalWidth1 >= num2)
        totalWidth1 = Table.DistributeWidth(num2, 0, length2, numArray2, minWidths, totalWidth1);
      int[] numArray3 = new int[length1];
      for (int index7 = 0; index7 < length1; ++index7)
      {
        int num5 = border + this.cellSpacing;
        for (int index8 = 0; index8 < length2; ++index8)
        {
          Table.Cell cell = cellArray[index8, index7];
          if (cell != null)
          {
            int maxWidth1 = Table.GetSpanSize(numArray2, index8, cell.ColumSpan, this.cellSpacing, padding) - 2 * padding;
            cell.X = num5 + padding;
            cell.Measure(gr, maxWidth1);
            cell.Width = maxWidth1;
            if (cell.RowSpan == 1)
            {
              int val2 = cell.BlockHeight != 0 ? cell.Height - 2 * padding : cell.Height;
              if (val2 < 0)
                val2 = 0;
              numArray3[index7] = Math.Max(numArray3[index7], val2);
            }
          }
          num5 += numArray2[index8] + (2 * padding + this.cellSpacing);
        }
      }
      for (int index9 = 0; index9 < length2; ++index9)
      {
        for (int index10 = 0; index10 < length1; ++index10)
        {
          Table.Cell cell = cellArray[index9, index10];
          if (cell != null && cell.RowSpan != 1)
          {
            int totalWidth2 = Table.GetSpanSize(numArray3, index10, cell.RowSpan, this.cellSpacing, padding) - 2 * padding;
            if (cell.Height > totalWidth2)
              Table.DistributeWidth(cell.Height, index10, cell.RowSpan, numArray3, numArray3, totalWidth2);
          }
        }
      }
      int num6 = border + this.cellSpacing;
      for (int index11 = 0; index11 < length1; ++index11)
      {
        int num7 = num6 + padding;
        for (int index12 = 0; index12 < length2; ++index12)
        {
          Table.Cell cell = cellArray[index12, index11];
          if (cell != null)
          {
            cell.Y = num7;
            cell.Height = Table.GetSpanSize(numArray3, index11, cell.RowSpan, this.cellSpacing, padding) - 2 * padding;
            cell.RecalcVAlign();
          }
        }
        num6 = num7 + (numArray3[index11] + padding + this.cellSpacing);
      }
      this.Bounds = new Rectangle(0, 0, totalWidth1 + 2 * border, num6 + border);
    }

    public override void Draw(Graphics gr, Point location)
    {
      base.Draw(gr, location);
      Rectangle bounds1 = this.Bounds;
      int border = this.Border >= 0 ? this.Border : 0;
      int num = border > 0 ? 1 : 0;
      bounds1.Offset(location);
      Color backColor = this.BackColor;
      if (!backColor.IsEmpty && this.BackColor != Color.Transparent)
      {
        using (Brush brush = (Brush) new SolidBrush(this.BackColor))
          gr.FillRectangle(brush, bounds1);
      }
      if (border > 0)
        ControlPaint.DrawBorder3D(gr, bounds1, Border3DStyle.RaisedOuter);
      foreach (Span inline1 in (Collection<Inline>) this.Inlines)
      {
        foreach (Table.Cell inline2 in (Collection<Inline>) inline1.Inlines)
        {
          Rectangle bounds2 = inline2.Bounds;
          bounds2.Inflate(this.cellPadding, this.cellPadding);
          bounds2.Offset(bounds1.Location);
          backColor = inline2.BackColor;
          if (!backColor.IsEmpty && inline2.BackColor != Color.Transparent)
          {
            using (Brush brush = (Brush) new SolidBrush(inline2.BackColor))
              gr.FillRectangle(brush, bounds2);
          }
          if (num > 0)
          {
            bounds2.Inflate(num, num);
            ControlPaint.DrawBorder3D(gr, bounds2, Border3DStyle.SunkenInner);
          }
          inline2.Draw(gr, bounds1.Location);
        }
      }
    }

    public override FlowBreak FlowBreak
    {
      get
      {
        return this.Align != HorizontalAlignment.None && this.Align != HorizontalAlignment.Center ? FlowBreak.None : FlowBreak.BreakLine | FlowBreak.After;
      }
    }

    public static int GetSpanSize(int[] widths, int index, int length, int spacing, int padding)
    {
      int num1 = 0;
      int num2 = Math.Min(length, widths.Length - index);
      for (int index1 = index; index1 < index + num2; ++index1)
        num1 += padding + widths[index1] + padding;
      return num1 + (num2 - 1) * spacing;
    }

    private static int DistributeWidth(
      int availableWidth,
      int start,
      int cols,
      int[] maxWidths,
      int[] minWidths,
      int totalWidth)
    {
      int num1 = availableWidth - totalWidth;
      cols = Math.Min(cols, maxWidths.Length - start);
      for (int index = start; index < start + cols; ++index)
      {
        int num2 = start + cols - index;
        int num3 = num1 / num2;
        int num4 = Math.Max(minWidths[index], maxWidths[index] + num3);
        int num5 = num4 - maxWidths[index];
        num1 -= num5;
        maxWidths[index] = num4;
      }
      totalWidth = availableWidth - num1;
      return totalWidth;
    }

    public int CellSpacing
    {
      get => this.cellSpacing;
      set => this.cellSpacing = value;
    }

    public int CellPadding
    {
      get => this.cellPadding;
      set => this.cellPadding = value;
    }

    public class Cell : FlowBlock
    {
      private int columnSpan = 1;
      private int rowSpan = 1;

      public int ColumSpan
      {
        get => this.columnSpan;
        set => this.columnSpan = value;
      }

      public int RowSpan
      {
        get => this.rowSpan;
        set => this.rowSpan = value;
      }

      public void RecalcVAlign()
      {
        if (this.Height == 0)
          return;
        switch (this.VAlign)
        {
          case VerticalAlignment.Middle:
            this.OffsetInlines(new Point(0, (this.Height - this.ActualSize.Height) / 2));
            break;
          case VerticalAlignment.Bottom:
            this.OffsetInlines(new Point(0, this.Height - this.ActualSize.Height));
            break;
        }
      }
    }

    public class Row : Block
    {
      private HorizontalAlignment align;

      public Row() => this.VAlign = VerticalAlignment.Middle;

      public override HorizontalAlignment Align
      {
        get => this.align;
        set => this.align = value;
      }

      protected override void CoreMeasure(Graphics gr, int maxWidth, LayoutType tbl)
      {
      }
    }
  }
}

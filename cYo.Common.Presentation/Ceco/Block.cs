// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Block
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public abstract class Block : Span, IRender
  {
    private int lastLayoutWidth;
    private int border = -1;
    private VerticalAlignment vAlign;
    private SizeValue blockWidth;
    private int blockHeight;
    private Size margin;

    public virtual void Measure(Graphics gr, int maxWidth)
    {
      LayoutType tbl = this.PendingLayout;
      maxWidth -= this.margin.Width * 2;
      if (tbl == LayoutType.None && maxWidth != this.lastLayoutWidth)
        tbl = LayoutType.Position;
      if (tbl != LayoutType.None)
      {
        this.CoreMeasure(gr, maxWidth, tbl);
        this.lastLayoutWidth = maxWidth;
      }
      this.X += this.margin.Width;
      this.Y += this.margin.Height;
      this.PendingLayout = LayoutType.None;
    }

    public virtual void Draw(Graphics gr, Point location)
    {
      if (this.ParentInline != null)
        return;
      this.Measure(gr, this.Size.Width);
    }

    public bool IsWhiteSpace => false;

    public int Border
    {
      get
      {
        return this.border == -1 && this.ParentInline is Block parentInline ? parentInline.Border : this.border;
      }
      set
      {
        if (this.border == value)
          return;
        this.border = value;
        this.OnBorderChanged();
      }
    }

    public virtual VerticalAlignment VAlign
    {
      get
      {
        return this.vAlign == VerticalAlignment.None && this.ParentInline is Block parentInline ? parentInline.VAlign : this.vAlign;
      }
      set
      {
        if (this.vAlign == value)
          return;
        this.vAlign = value;
        this.OnVAlignChanged();
      }
    }

    public SizeValue BlockWidth
    {
      get => this.blockWidth;
      set
      {
        if (this.blockWidth == value)
          return;
        this.blockWidth = value;
        this.OnBlockWidthChanged();
      }
    }

    public int BlockHeight
    {
      get => this.blockHeight;
      set
      {
        if (this.blockHeight == value)
          return;
        this.blockHeight = value;
        this.OnBlockHeightChanged();
      }
    }

    public virtual Size Margin
    {
      get => this.margin;
      set
      {
        if (this.margin == value)
          return;
        this.margin = value;
        this.OnMarginChanged();
      }
    }

    public override bool IsBlock => true;

    public int MinimumWidth { get; set; }

    public void SetAlign(ContentAlignment contentAlignment)
    {
      switch (contentAlignment)
      {
        case ContentAlignment.TopCenter:
        case ContentAlignment.MiddleCenter:
        case ContentAlignment.BottomCenter:
          this.Align = HorizontalAlignment.Center;
          break;
        case ContentAlignment.TopRight:
        case ContentAlignment.MiddleRight:
        case ContentAlignment.BottomRight:
          this.Align = HorizontalAlignment.Right;
          break;
        default:
          this.Align = HorizontalAlignment.Left;
          break;
      }
      switch (contentAlignment)
      {
        case ContentAlignment.MiddleLeft:
        case ContentAlignment.MiddleCenter:
        case ContentAlignment.MiddleRight:
          this.VAlign = VerticalAlignment.Middle;
          break;
        case ContentAlignment.BottomLeft:
        case ContentAlignment.BottomCenter:
        case ContentAlignment.BottomRight:
          this.VAlign = VerticalAlignment.Bottom;
          break;
        default:
          this.VAlign = VerticalAlignment.Top;
          break;
      }
    }

    protected abstract void CoreMeasure(Graphics gr, int maxWidth, LayoutType tbl);

    protected virtual void OnVAlignChanged() => this.InvokeLayout(LayoutType.Position);

    protected virtual void OnBorderChanged() => this.InvokeLayout(LayoutType.Full);

    protected virtual void OnBlockWidthChanged() => this.InvokeLayout(LayoutType.Full);

    protected virtual void OnBlockHeightChanged() => this.InvokeLayout(LayoutType.Full);

    protected virtual void OnMarginChanged() => this.InvokeLayout(LayoutType.Full);

    public override bool IsNode => false;
  }
}

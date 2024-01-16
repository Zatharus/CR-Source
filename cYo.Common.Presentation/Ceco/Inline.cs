// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Inline
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.ComponentModel;
using System;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public abstract class Inline : DisposableObject
  {
    private string fontFamily;
    private FontSize fontSize = FontSize.Empty;
    private float fontSizeEM;
    private float fontScale = 1f;
    private FontStyle fontStyle;
    private Color foreColor = Color.Empty;
    private Color backColor = Color.Empty;
    private const float normSize = 12f;
    private static readonly float[] sizeFactors = new float[7]
    {
      0.5833333f,
      0.7916667f,
      1f,
      1.16666663f,
      1.5f,
      2f,
      3f
    };
    private Cursor mouseCursor;
    private Inline parentInline;
    private Rectangle bounds;
    private BaseAlignment baseAlign;
    private HorizontalAlignment align;
    private LayoutType pendingLayout;
    private bool visible = true;
    private bool isHot;

    public virtual string FontFamily
    {
      get
      {
        string fontFamily = this.fontFamily;
        if (string.IsNullOrEmpty(fontFamily) && this.parentInline != null)
          fontFamily = this.parentInline.FontFamily;
        return fontFamily;
      }
      set
      {
        if (this.fontFamily == value)
          return;
        this.fontFamily = value;
        this.OnFontChanged();
      }
    }

    public virtual FontSize FontSize
    {
      get => this.fontSize;
      set
      {
        if (this.fontSize == value)
          return;
        this.fontSize = value;
        this.OnFontChanged();
      }
    }

    public virtual float FontSizeEM
    {
      get
      {
        float fontSizeEm = this.fontSizeEM;
        if ((double) fontSizeEm == 0.0 && this.parentInline != null)
          fontSizeEm = this.parentInline.FontSizeEM;
        return fontSizeEm;
      }
      set
      {
        if ((double) this.fontSizeEM == (double) value)
          return;
        this.fontSizeEM = value;
        this.OnFontChanged();
      }
    }

    public virtual float FontScale
    {
      get
      {
        float fontScale = this.fontScale;
        if ((double) fontScale == 0.0 && this.parentInline != null)
          fontScale = this.parentInline.FontScale;
        return fontScale;
      }
      set
      {
        if ((double) this.fontScale == (double) value)
          return;
        this.fontScale = value;
        this.OnFontChanged();
      }
    }

    public virtual FontStyle FontStyle
    {
      get
      {
        FontStyle fontStyle = this.fontStyle;
        if (fontStyle == FontStyle.Regular && this.parentInline != null)
          fontStyle = this.parentInline.FontStyle;
        return fontStyle;
      }
      set
      {
        if (this.fontStyle == value)
          return;
        this.fontStyle = value;
        this.OnFontChanged();
      }
    }

    public virtual Color ForeColor
    {
      get
      {
        Color foreColor = this.foreColor;
        if (foreColor.IsEmpty && this.parentInline != null)
          foreColor = this.parentInline.ForeColor;
        return foreColor;
      }
      set
      {
        if (this.foreColor == value)
          return;
        this.foreColor = value;
        this.OnForeColorChanged();
      }
    }

    public virtual Color BackColor
    {
      get
      {
        Color backColor = this.backColor;
        if (backColor.IsEmpty && this.parentInline != null)
          backColor = this.parentInline.BackColor;
        return backColor;
      }
      set
      {
        if (this.backColor == value)
          return;
        this.backColor = value;
        this.OnBackColorChanged();
      }
    }

    public virtual Font Font
    {
      get
      {
        return this.GetFont(this.FontFamily, this.FontSizeEM * this.FontScale * Inline.sizeFactors[Math.Max(Math.Min(this.GetFontSize(), 7), 1) - 1], this.FontStyle);
      }
      set
      {
        this.FontFamily = value.FontFamily.Name;
        this.FontSizeEM = value.SizeInPoints;
        this.FontStyle = value.Style;
      }
    }

    public virtual Cursor MouseCursor
    {
      get
      {
        Cursor mouseCursor = this.mouseCursor;
        if (this.mouseCursor == (Cursor) null && this.parentInline != null)
          mouseCursor = this.parentInline.MouseCursor;
        return mouseCursor;
      }
      set
      {
        if (this.mouseCursor == value)
          return;
        this.mouseCursor = value;
        this.OnMouseCursorChanged();
      }
    }

    public Inline ParentInline
    {
      get => this.parentInline;
      set => this.parentInline = value;
    }

    public virtual bool IsNode => false;

    public Rectangle Bounds
    {
      get => this.bounds;
      set => this.bounds = value;
    }

    public Point Location
    {
      get => this.bounds.Location;
      set => this.bounds.Location = value;
    }

    public int X
    {
      get => this.bounds.X;
      set => this.bounds.X = value;
    }

    public int Y
    {
      get => this.bounds.Y;
      set => this.bounds.Y = value;
    }

    public Size Size
    {
      get => this.bounds.Size;
      set => this.bounds.Size = value;
    }

    public int Width
    {
      get => this.bounds.Width;
      set => this.bounds.Width = value;
    }

    public int Height
    {
      get => this.bounds.Height;
      set => this.bounds.Height = value;
    }

    public int BaseLine { get; set; }

    public BaseAlignment BaseAlign
    {
      get
      {
        return this.baseAlign == BaseAlignment.None && this.parentInline != null ? this.parentInline.BaseAlign : this.baseAlign;
      }
      set => this.baseAlign = value;
    }

    public virtual HorizontalAlignment Align
    {
      get
      {
        return this.align == HorizontalAlignment.None && this.parentInline != null ? this.parentInline.Align : this.align;
      }
      set
      {
        if (this.align == value)
          return;
        this.align = value;
        this.OnAlignChanged();
      }
    }

    public virtual int FlowBreakOffset => 0;

    public virtual bool IsBlock => false;

    public virtual FlowBreak FlowBreak => FlowBreak.None;

    public LayoutType PendingLayout
    {
      get => this.pendingLayout;
      set
      {
        if (this.pendingLayout == value)
          return;
        this.pendingLayout = value;
        this.OnPendingLayoutChanged();
      }
    }

    public bool Visible
    {
      get => this.visible;
      set => this.visible = value;
    }

    public virtual Inline GetHitItem(Point location, Point hitPoint)
    {
      if (this.IsNode || !this.Visible || this.IsBlock)
        return (Inline) null;
      Rectangle bounds = this.Bounds;
      bounds.Offset(location);
      return !bounds.Contains(hitPoint) ? (Inline) null : this;
    }

    public void Layout(LayoutType type)
    {
      if (type > this.PendingLayout)
        this.PendingLayout = type;
      if (this.ParentInline == null)
        return;
      this.ParentInline.Layout(type);
    }

    public virtual int GetFontSize()
    {
      int size = this.FontSize.Size;
      if (this.FontSize.Relative && this.parentInline != null)
        size += this.parentInline.GetFontSize();
      return size;
    }

    public virtual Font GetFont(string fontFamily, float fontSize, FontStyle fontStyle)
    {
      return this.GetService<IResources>(true).GetFont(fontFamily, fontSize, fontStyle);
    }

    public virtual Image GetImage(string source)
    {
      return this.GetService<IResources>(true).GetImage(source);
    }

    public event EventHandler PendingLayoutChanged;

    protected virtual void OnForeColorChanged()
    {
    }

    protected virtual void OnBackColorChanged()
    {
    }

    protected virtual void OnAlignChanged() => this.InvokeLayout(LayoutType.Position);

    protected virtual void OnFontChanged() => this.InvokeLayout(LayoutType.Full);

    protected virtual void OnPendingLayoutChanged()
    {
      if (this.PendingLayoutChanged == null)
        return;
      this.PendingLayoutChanged((object) this, EventArgs.Empty);
    }

    protected virtual void InvokeLayout(LayoutType type) => this.Layout(type);

    protected virtual void OnMouseCursorChanged()
    {
    }

    public virtual void MouseEnter()
    {
      if (this.isHot)
        return;
      this.isHot = true;
      this.OnMouseEnter();
      if (this.parentInline == null)
        return;
      this.parentInline.MouseEnter();
    }

    public virtual void MouseLeave()
    {
      if (!this.isHot)
        return;
      this.isHot = false;
      this.OnMouseLeave();
      if (this.parentInline == null)
        return;
      this.parentInline.MouseLeave();
    }

    public virtual void OnMouseClick()
    {
    }

    protected virtual void OnMouseEnter()
    {
    }

    protected virtual void OnMouseLeave()
    {
    }

    public virtual T GetService<T>(bool withException) where T : class
    {
      if (!(this is T obj) && this.parentInline != null)
        obj = this.parentInline.GetService<T>(false);
      return !((object) obj == null & withException) ? obj : throw new InvalidOperationException("Service not found");
    }

    public int FontCellDescent => this.Font.FontFamily.GetCellDescent(this.Font.Style);

    public int FontEmHeight => this.Font.FontFamily.GetEmHeight(this.Font.Style);

    public int FontCellAscent => this.Font.FontFamily.GetCellAscent(this.Font.Style);

    public int FontLineSpacing => this.Font.FontFamily.GetLineSpacing(this.Font.Style);

    public int DescentHeight => this.DesignToPixel(this.FontCellDescent);

    public int AscentHeight => this.DesignToPixel(this.FontCellAscent);

    private int DesignToPixel(int design)
    {
      return (int) Math.Round((double) this.Font.Height / (double) this.FontEmHeight * (double) design);
    }
  }
}

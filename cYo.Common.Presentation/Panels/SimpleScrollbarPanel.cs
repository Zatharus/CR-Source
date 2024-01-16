// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.SimpleScrollbarPanel
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Mathematics;
using System;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class SimpleScrollbarPanel : OverlayPanel
  {
    private int borderWidth = 1;
    private Color borderColor = Color.LightGray;
    private Color backColor = Color.Black;
    private bool mirror;
    private Color knobColor = Color.White;
    private int value;
    private int minimum;
    private int maximum;
    private ScalableBitmap background;
    private ScalableBitmap knob;

    public SimpleScrollbarPanel(Size sz)
      : base(sz)
    {
    }

    public int BorderWidth
    {
      get => this.borderWidth;
      set
      {
        if (this.borderWidth == value)
          return;
        this.borderWidth = value;
        this.Invalidate();
      }
    }

    public Color BorderColor
    {
      get => this.borderColor;
      set
      {
        if (this.borderColor == value)
          return;
        this.borderColor = value;
        this.Invalidate();
      }
    }

    public Color BackColor
    {
      get => this.backColor;
      set
      {
        if (this.backColor == value)
          return;
        this.backColor = value;
        this.Invalidate();
      }
    }

    public bool Mirror
    {
      get => this.mirror;
      set
      {
        if (this.mirror == value)
          return;
        this.mirror = value;
        this.Invalidate();
      }
    }

    public Color KnobColor
    {
      get => this.knobColor;
      set
      {
        if (this.knobColor == value)
          return;
        this.knobColor = value;
        this.Invalidate();
      }
    }

    public int Value
    {
      get => this.value;
      set
      {
        value = value.Clamp(this.minimum, this.maximum);
        if (this.value == value)
          return;
        this.value = value;
        this.Invalidate();
        this.OnValueChanged();
      }
    }

    public int Minimum
    {
      get => this.minimum;
      set
      {
        if (this.minimum == value)
          return;
        this.minimum = value;
        this.Invalidate();
        this.OnMinimumChanged();
      }
    }

    public int Maximum
    {
      get => this.maximum;
      set
      {
        if (this.maximum == value)
          return;
        this.maximum = value;
        this.Invalidate();
        this.OnMaximumChanged();
      }
    }

    public ScalableBitmap Background
    {
      get => this.background;
      set
      {
        if (this.background == value)
          return;
        this.background = value;
        this.Invalidate();
      }
    }

    public ScalableBitmap Knob
    {
      get => this.knob;
      set
      {
        if (this.knob == value)
          return;
        this.knob = value;
        this.Invalidate();
      }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      this.ScrollToPoint(e.Location);
      base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (this.PanelState == PanelState.Selected)
        this.ScrollToPoint(e.Location);
      base.OnMouseMove(e);
    }

    protected override void OnSizeChanged()
    {
      base.OnSizeChanged();
      this.Invalidate();
    }

    private void ScrollToPoint(Point pt)
    {
      int num = (this.minimum + (pt.X - this.borderWidth) * (this.maximum - this.minimum + 1) / (this.Width - 2 * this.borderWidth)).Clamp(this.minimum, this.maximum);
      if (this.mirror)
        num = this.maximum - (num - this.minimum);
      if (num == this.Value)
        return;
      this.Value = num;
      this.OnScroll();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics graphics = e.Graphics;
      IBitmapRenderer gr = (IBitmapRenderer) new BitmapGdiRenderer(graphics);
      Rectangle clientRectangle = this.ClientRectangle;
      Rectangle rectangle = clientRectangle;
      rectangle.Inflate(-this.borderWidth, -this.borderWidth);
      graphics.Clear(this.BackColor);
      if (this.background != null)
      {
        this.background.Draw(gr, (RectangleF) rectangle);
      }
      else
      {
        using (Pen pen = new Pen(this.borderColor, (float) this.borderWidth))
          graphics.DrawRectangle(pen, rectangle);
      }
      int num1 = this.maximum - this.minimum;
      if (num1 == 0)
        return;
      if (this.knob != null)
      {
        int height = clientRectangle.Height - 1;
        int width = this.knob.Bitmap.Width * height / this.knob.Bitmap.Height;
        int num2 = (this.value - this.minimum) * (clientRectangle.Width - width) / num1;
        int x = !this.Mirror ? clientRectangle.Left + num2 : clientRectangle.Right - num2 - width;
        this.knob.Draw(gr, (RectangleF) new Rectangle(x, clientRectangle.Top, width, height));
      }
      else
      {
        using (Brush brush = (Brush) new SolidBrush(this.knobColor))
        {
          int x = (this.value - this.minimum) * clientRectangle.Width / num1;
          int width = (this.value + 1 - this.minimum) * clientRectangle.Width / num1 - x;
          graphics.FillRectangle(brush, x, clientRectangle.Top, width, clientRectangle.Height);
        }
      }
    }

    public event EventHandler Scroll;

    public event EventHandler ValueChanged;

    public event EventHandler MinimumChanged;

    public event EventHandler MaximumChanged;

    protected virtual void OnValueChanged()
    {
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnScroll()
    {
      if (this.Scroll == null)
        return;
      this.Scroll((object) this, EventArgs.Empty);
    }

    protected virtual void OnMinimumChanged()
    {
      if (this.MinimumChanged == null)
        return;
      this.MinimumChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnMaximumChanged()
    {
      if (this.MaximumChanged == null)
        return;
      this.MaximumChanged((object) this, EventArgs.Empty);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TrackBarLite
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Mathematics;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TrackBarLite : Control
  {
    private bool enableVisualStyles = true;
    private bool enableFocusIndicator = true;
    private int minimum;
    private int maximum = 100;
    private int value;
    private int largeChange = 10;
    private int smallChange = 1;
    private int barThickness = 4;
    private int barMargin = 4;
    private System.Drawing.Size thumbSize = new System.Drawing.Size(12, 24);
    private TickStyle tickStyle;
    private int tickThickness = 4;
    private int tickFrequency = 10;
    private TrackBarThumbState trackBarThumbState = TrackBarThumbState.Normal;
    private TrackBarLite.DrawHandler DrawThumb;
    private TrackBarLite.DrawHandler DrawFocus;
    private TrackBarLite.DrawHandler DrawBar;
    private TrackBarLite.DrawHandler DrawTicks;
    private bool mouseDown;

    public TrackBarLite()
    {
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.Selectable, true);
      this.SetDrawHandlers(true);
      SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.SystemEvents_UserPreferenceChanged);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.SystemEvents_UserPreferenceChanged);
      base.Dispose(disposing);
    }

    [Category("Appearance")]
    [DefaultValue(true)]
    public bool EnableVisualStyles
    {
      get => this.enableVisualStyles;
      set
      {
        if (this.enableVisualStyles == value)
          return;
        this.enableVisualStyles = value;
        this.SetDrawHandlers(this.enableVisualStyles);
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(true)]
    public bool EnableFocusIndicator
    {
      get => this.enableFocusIndicator;
      set
      {
        if (this.enableFocusIndicator == value)
          return;
        this.enableFocusIndicator = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [DefaultValue(0)]
    public int Minimum
    {
      get => this.minimum;
      set
      {
        if (this.minimum == value)
          return;
        this.minimum = value;
        this.Value = this.Value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [DefaultValue(100)]
    public int Maximum
    {
      get => this.maximum;
      set
      {
        if (this.maximum == value)
          return;
        this.maximum = value;
        this.Value = this.Value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [DefaultValue(0)]
    public int Value
    {
      get => this.value;
      set
      {
        value = this.Clamp(value);
        if (this.value == value)
          return;
        Rectangle thumbRectangle1 = this.GetThumbRectangle();
        this.value = value;
        Rectangle thumbRectangle2 = this.GetThumbRectangle();
        if (thumbRectangle1 != thumbRectangle2)
        {
          this.Invalidate(thumbRectangle1);
          this.Invalidate(thumbRectangle2);
          this.Update();
        }
        this.OnValueChanged();
      }
    }

    [Category("Behavior")]
    [DefaultValue(10)]
    public int LargeChange
    {
      get => this.largeChange;
      set => this.largeChange = value;
    }

    [Category("Behavior")]
    [DefaultValue(1)]
    public int SmallChange
    {
      get => this.smallChange;
      set => this.smallChange = value;
    }

    [Category("Appearance")]
    [DefaultValue(4)]
    public int BarThickness
    {
      get => this.barThickness;
      set
      {
        if (this.barThickness == value)
          return;
        this.barThickness = value;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(4)]
    public int BarMargin
    {
      get => this.barMargin;
      set
      {
        if (this.barMargin == value)
          return;
        this.barMargin = value;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(typeof (System.Drawing.Size), "12, 24")]
    public System.Drawing.Size ThumbSize
    {
      get => this.thumbSize;
      set
      {
        if (this.thumbSize == value)
          return;
        this.InvalidateThumb();
        this.thumbSize = value;
        this.InvalidateThumb();
      }
    }

    [Category("Appearance")]
    [DefaultValue(TickStyle.None)]
    public TickStyle TickStyle
    {
      get => this.tickStyle;
      set
      {
        if (this.tickStyle == value)
          return;
        this.tickStyle = value;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(4)]
    public int TickThickness
    {
      get => this.tickThickness;
      set
      {
        if (this.tickThickness == value)
          return;
        this.tickThickness = value;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(10)]
    public int TickFrequency
    {
      get => this.tickFrequency;
      set
      {
        if (this.tickFrequency == value)
          return;
        this.tickFrequency = value;
        this.Invalidate();
      }
    }

    protected TrackBarThumbState TrackBarThumbState
    {
      get => this.trackBarThumbState;
      set
      {
        if (this.trackBarThumbState == value)
          return;
        this.InvalidateThumb();
        this.trackBarThumbState = value;
        this.InvalidateThumb();
      }
    }

    protected int TicksCount => (this.Maximum - this.Minimum) / this.tickFrequency + 1;

    public void SetRange(int minimum, int maximum)
    {
      this.Minimum = minimum;
      this.Maximum = maximum;
    }

    [Category("Behavior")]
    public event EventHandler Scroll;

    [Category("Behavior")]
    public event EventHandler ValueChanged;

    protected virtual void OnScroll()
    {
      if (this.Scroll == null)
        return;
      this.Scroll((object) this, EventArgs.Empty);
    }

    protected virtual void OnValueChanged()
    {
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((object) this, EventArgs.Empty);
    }

    public Rectangle GetBarRectangle(Rectangle rc)
    {
      rc.Inflate(-this.barMargin * 2, -this.barMargin * 2);
      return new Rectangle(rc.X, rc.Y + (rc.Height - this.barThickness) / 2, rc.Width, this.barThickness);
    }

    public Rectangle GetBarRectangle() => this.GetBarRectangle(this.ClientRectangle);

    public Rectangle GetTicksRectangle()
    {
      Rectangle barRectangle = this.GetBarRectangle();
      Rectangle ticksRectangle = barRectangle;
      int width = this.GetThumbRectangle().Width;
      ticksRectangle.X += width / 2;
      ticksRectangle.Width -= width;
      switch (this.tickStyle)
      {
        case TickStyle.TopLeft:
          ticksRectangle.Height = this.tickThickness;
          ticksRectangle.Y = barRectangle.Top - ticksRectangle.Height - this.BarMargin;
          break;
        case TickStyle.BottomRight:
          ticksRectangle.Height = this.tickThickness;
          ticksRectangle.Y = barRectangle.Bottom + this.BarMargin;
          break;
        default:
          return Rectangle.Empty;
      }
      return ticksRectangle;
    }

    public Rectangle GetThumbRectangle(Rectangle rc, System.Drawing.Size sz)
    {
      if (this.minimum - this.maximum == 0)
        return Rectangle.Empty;
      Rectangle barRectangle = this.GetBarRectangle(rc);
      int num = barRectangle.Width - sz.Width;
      int y = barRectangle.Y + (barRectangle.Height - sz.Height) / 2;
      return new Rectangle(barRectangle.X + num * (this.value - this.minimum) / (this.maximum - this.minimum), y, sz.Width, sz.Height);
    }

    public Rectangle GetThumbRectangle(Rectangle rc)
    {
      return this.GetThumbRectangle(rc, this.ThumbSize.ScaleDpi());
    }

    public Rectangle GetThumbRectangle() => this.GetThumbRectangle(this.ClientRectangle);

    private int GetValueFromMouse(Rectangle rc, System.Drawing.Point pt)
    {
      Rectangle barRectangle = this.GetBarRectangle(rc);
      System.Drawing.Size size = this.ThumbSize.ScaleDpi();
      int num1 = this.maximum - this.minimum;
      int num2 = barRectangle.Width - size.Width / 2;
      int num3 = num1 == 0 ? 0 : barRectangle.Width / num1;
      return this.minimum + (num2 == 0 ? 0 : (pt.X - barRectangle.Left + num3 / 2) * num1 / num2);
    }

    private int GetValueFromMouse(System.Drawing.Point pt)
    {
      return this.GetValueFromMouse(this.ClientRectangle, pt);
    }

    private int Clamp(int value) => value.Clamp(this.minimum, this.maximum);

    private void SetDrawHandlers(bool visualStyles)
    {
      if (visualStyles && TrackBarRenderer.IsSupported)
      {
        this.DrawThumb = new TrackBarLite.DrawHandler(this.DrawThumbWithVisualStyle);
        this.DrawFocus = new TrackBarLite.DrawHandler(TrackBarLite.DrawFocusWithVisualStyle);
        this.DrawBar = new TrackBarLite.DrawHandler(this.DrawBarWithVisualStyle);
        this.DrawTicks = new TrackBarLite.DrawHandler(this.DrawTicksWithVisualStyle);
      }
      else
      {
        this.DrawThumb = new TrackBarLite.DrawHandler(this.DrawThumbPlain);
        this.DrawFocus = new TrackBarLite.DrawHandler(TrackBarLite.DrawFocusPlain);
        this.DrawBar = new TrackBarLite.DrawHandler(this.DrawBarPlain);
        this.DrawTicks = new TrackBarLite.DrawHandler(this.DrawTicksPlain);
      }
    }

    private void InvalidateThumb()
    {
      this.Invalidate(this.GetThumbRectangle(this.ClientRectangle, this.ThumbSize.ScaleDpi()));
    }

    private void DrawThumbWithVisualStyle(Graphics gr, Rectangle rc)
    {
      try
      {
        TrackBarThumbState trackBarThumbState = this.TrackBarThumbState;
        System.Drawing.Size sz = this.ThumbSize.ScaleDpi();
        switch (this.tickStyle)
        {
          case TickStyle.TopLeft:
            TrackBarRenderer.DrawTopPointingThumb(gr, this.GetThumbRectangle(rc, sz), trackBarThumbState);
            break;
          case TickStyle.BottomRight:
            TrackBarRenderer.DrawBottomPointingThumb(gr, this.GetThumbRectangle(rc, sz), trackBarThumbState);
            break;
          default:
            TrackBarRenderer.DrawHorizontalThumb(gr, this.GetThumbRectangle(rc, sz), trackBarThumbState);
            break;
        }
      }
      catch
      {
        this.DrawThumbPlain(gr, rc);
      }
    }

    private static void DrawFocusWithVisualStyle(Graphics gr, Rectangle rc)
    {
      ControlPaint.DrawFocusRectangle(gr, rc);
    }

    private void DrawBarWithVisualStyle(Graphics gr, Rectangle rc)
    {
      try
      {
        TrackBarRenderer.DrawHorizontalTrack(gr, this.GetBarRectangle(rc));
      }
      catch
      {
        this.DrawBarPlain(gr, rc);
      }
    }

    private void DrawTicksWithVisualStyle(Graphics gr, Rectangle rc)
    {
      try
      {
        Rectangle ticksRectangle = this.GetTicksRectangle();
        ticksRectangle.Inflate(1, 0);
        TrackBarRenderer.DrawHorizontalTicks(gr, ticksRectangle, this.TicksCount, EdgeStyle.Bump);
      }
      catch
      {
        this.DrawTicksPlain(gr, rc);
      }
    }

    private void DrawThumbPlain(Graphics gr, Rectangle rc)
    {
      System.Drawing.Size sz = this.ThumbSize.ScaleDpi();
      ButtonState state;
      switch (this.TrackBarThumbState)
      {
        case TrackBarThumbState.Pressed:
          state = ButtonState.Pushed;
          break;
        case TrackBarThumbState.Disabled:
          state = ButtonState.Inactive;
          break;
        default:
          state = ButtonState.Normal;
          break;
      }
      Rectangle thumbRectangle = this.GetThumbRectangle(rc, sz);
      if (thumbRectangle.Width < 2 || thumbRectangle.Height < 2)
        return;
      ControlPaint.DrawButton(gr, thumbRectangle, state);
    }

    private static void DrawFocusPlain(Graphics gr, Rectangle rc)
    {
      ControlPaint.DrawFocusRectangle(gr, rc);
    }

    private void DrawBarPlain(Graphics gr, Rectangle rc)
    {
      Rectangle barRectangle = this.GetBarRectangle(rc);
      if (barRectangle.Width < 2 || barRectangle.Height < 2)
        return;
      ControlPaint.DrawBorder3D(gr, barRectangle, Border3DStyle.SunkenInner);
    }

    private void DrawTicksPlain(Graphics gr, Rectangle rc)
    {
      if (this.TicksCount == 0)
        return;
      Rectangle ticksRectangle = this.GetTicksRectangle();
      using (Pen pen = new Pen(this.ForeColor))
      {
        for (int index = 0; index <= this.TicksCount; ++index)
        {
          int num = ticksRectangle.Left + ticksRectangle.Width * index / this.TicksCount;
          gr.DrawLine(pen, num, ticksRectangle.Top, num, ticksRectangle.Bottom);
        }
      }
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
      base.OnEnabledChanged(e);
      this.TrackBarThumbState = this.Enabled ? TrackBarThumbState.Normal : TrackBarThumbState.Disabled;
    }

    protected override void OnGotFocus(EventArgs e)
    {
      base.OnGotFocus(e);
      this.TrackBarThumbState = TrackBarThumbState.Hot;
      this.Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
      base.OnLostFocus(e);
      this.TrackBarThumbState = TrackBarThumbState.Normal;
      this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics graphics = e.Graphics;
      Rectangle clientRectangle = this.ClientRectangle;
      this.DrawBar(graphics, clientRectangle);
      if (!this.GetTicksRectangle().IsEmpty)
        this.DrawTicks(graphics, clientRectangle);
      this.DrawThumb(graphics, clientRectangle);
      if (!this.Focused || !this.EnableFocusIndicator)
        return;
      this.DrawFocus(graphics, clientRectangle);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.Focus();
      this.mouseDown = true;
      this.HandleScroll(e.Location);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.mouseDown)
        this.HandleScroll(e.Location);
      else if (this.GetThumbRectangle().Contains(e.Location) || this.Focused)
        this.TrackBarThumbState = TrackBarThumbState.Hot;
      else
        this.TrackBarThumbState = TrackBarThumbState.Normal;
    }

    private void HandleScroll(System.Drawing.Point pt)
    {
      this.TrackBarThumbState = TrackBarThumbState.Pressed;
      int num = this.Clamp(this.GetValueFromMouse(pt));
      if (num == this.Value)
        return;
      this.Value = num;
      this.OnScroll();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.mouseDown = false;
    }

    protected override bool IsInputKey(Keys keyData)
    {
      return keyData == Keys.Left || keyData == Keys.Right || base.IsInputKey(keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      int num1 = this.Value;
      switch (e.KeyCode)
      {
        case Keys.Prior:
          num1 -= this.LargeChange;
          e.Handled = true;
          break;
        case Keys.Next:
          num1 += this.LargeChange;
          e.Handled = true;
          break;
        case Keys.End:
          num1 = this.Maximum;
          e.Handled = true;
          break;
        case Keys.Home:
          num1 = this.Minimum;
          e.Handled = true;
          break;
        case Keys.Left:
          num1 -= this.SmallChange;
          e.Handled = true;
          break;
        case Keys.Right:
          num1 += this.SmallChange;
          e.Handled = true;
          break;
        default:
          base.OnKeyDown(e);
          break;
      }
      int num2 = this.Clamp(num1);
      if (this.Value == num2)
        return;
      this.Value = num2;
      this.OnScroll();
    }

    private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
      this.SetDrawHandlers(true);
    }

    private delegate void DrawHandler(Graphics gr, Rectangle rc);
  }
}

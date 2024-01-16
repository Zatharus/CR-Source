// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ThumbStickControl
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ThumbStickControl : Control
  {
    private readonly Timer scrollTimer;
    private Image stickImage;
    private Image stickImagePressed;
    private System.Drawing.Size stickSize = new System.Drawing.Size(6, 6);
    private SizeF sensitivity = new SizeF(4f, 4f);
    private float accel = 1f;
    private bool autoScroll = true;
    private int autoScrollInterval = 20;
    private PointF movement;
    private System.Drawing.Point clickPoint;

    public ThumbStickControl()
    {
      this.scrollTimer = new Timer() { Interval = 250 };
      this.scrollTimer.Tick += (EventHandler) ((x, y) => this.OnScroll());
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.ResizeRedraw, true);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.scrollTimer.Dispose();
      base.Dispose(disposing);
    }

    [Category("Display")]
    [DefaultValue(null)]
    public Image StickImage
    {
      get => this.stickImage;
      set
      {
        if (this.stickImage == value)
          return;
        this.stickImage = value;
        if (this.IsMouseDown)
          return;
        this.Invalidate();
      }
    }

    [Category("Display")]
    [DefaultValue(null)]
    public Image StickImagePressed
    {
      get => this.stickImagePressed;
      set
      {
        if (this.stickImagePressed == value)
          return;
        this.stickImagePressed = value;
        if (!this.IsMouseDown)
          return;
        this.Invalidate();
      }
    }

    [Category("Display")]
    [DefaultValue(typeof (System.Drawing.Size), "6, 6")]
    public System.Drawing.Size StickSize
    {
      get => this.stickSize;
      set
      {
        if (this.stickSize == value)
          return;
        this.stickSize = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [DefaultValue(typeof (SizeF), "4f, 4f")]
    public SizeF Sensitivity
    {
      get => this.sensitivity;
      set
      {
        if (this.sensitivity == value)
          return;
        this.sensitivity = value;
      }
    }

    [Category("Behavior")]
    [DefaultValue(1f)]
    public float Acceleration
    {
      get => this.accel;
      set
      {
        if ((double) this.accel == (double) value)
          return;
        this.accel = value;
      }
    }

    [Category("Behavior")]
    [DefaultValue(true)]
    public bool AutoScroll
    {
      get => this.autoScroll;
      set => this.autoScroll = value;
    }

    [Category("Behavior")]
    [DefaultValue(20)]
    public int AutoScrollInterval
    {
      get => this.autoScrollInterval;
      set => this.autoScrollInterval = value;
    }

    [Browsable(false)]
    public PointF Movement
    {
      get => this.movement;
      protected set
      {
        if (value == this.movement)
          return;
        this.movement = value;
        this.Invalidate();
        this.OnMovementChanged();
      }
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
      base.OnPaint(pe);
      this.DrawStick(pe.Graphics, this.Movement);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.clickPoint = e.Location;
      this.Movement = PointF.Empty;
      this.Invalidate();
      if (!this.autoScroll)
        return;
      this.scrollTimer.Interval = this.autoScrollInterval;
      this.scrollTimer.Start();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!this.IsMouseDown)
        return;
      float d1 = ((float) (e.X - this.clickPoint.X) / this.sensitivity.Width).Clamp(-1f, 1f);
      float d2 = ((float) (e.Y - this.clickPoint.Y) / this.sensitivity.Height).Clamp(-1f, 1f);
      this.Movement = new PointF(this.CalcAccel(d1), this.CalcAccel(d2));
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.Movement = PointF.Empty;
      this.Invalidate();
      this.scrollTimer.Stop();
    }

    [field: NonSerialized]
    public event EventHandler MovementChanged;

    [field: NonSerialized]
    public event EventHandler Scroll;

    protected virtual void OnMovementChanged()
    {
      if (this.MovementChanged == null)
        return;
      this.MovementChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnScroll()
    {
      if (this.Scroll == null)
        return;
      this.Scroll((object) this, EventArgs.Empty);
    }

    private bool IsMouseDown => this.Capture;

    private void DrawStick(Graphics g, PointF movement)
    {
      Rectangle displayRectangle = this.DisplayRectangle;
      ref Rectangle local = ref displayRectangle;
      System.Drawing.Size stickSize = this.StickSize;
      int width = -stickSize.Width / 2;
      stickSize = this.StickSize;
      int height = -stickSize.Height / 2;
      local.Inflate(width, height);
      RectangleF rect = new RectangleF(new PointF((float) ((double) displayRectangle.Left + (double) displayRectangle.Width / 2.0 + (double) (displayRectangle.Width - 1) / 2.0 * (double) movement.X) - (float) (this.StickSize.Width / 2), (float) ((double) displayRectangle.Top + (double) displayRectangle.Height / 2.0 + (double) (displayRectangle.Height - 1) / 2.0 * (double) movement.Y) - (float) (this.StickSize.Height / 2)), (SizeF) this.stickSize);
      using (g.AntiAlias())
      {
        if (this.IsMouseDown)
        {
          if (this.stickImagePressed == null)
            g.FillEllipse(SystemBrushes.ControlDarkDark, rect);
          else
            g.DrawImage(this.stickImage, rect);
        }
        else if (this.stickImage == null)
          g.FillEllipse(SystemBrushes.ControlDark, rect);
        else
          g.DrawImage(this.stickImagePressed, rect);
      }
    }

    private float CalcAccel(float d)
    {
      return (float) Math.Pow((double) Math.Abs(d), (double) this.accel) * (float) Math.Sign(d);
    }
  }
}

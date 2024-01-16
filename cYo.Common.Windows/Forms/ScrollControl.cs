// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ScrollControl
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ScrollControl : Control
  {
    private readonly HScrollBar hScrollbar = new HScrollBar();
    private readonly VScrollBar vScrollBar = new VScrollBar();
    private readonly ThumbStickControl thumbStick = new ThumbStickControl();
    private readonly Timer scrollTimer = new Timer();
    private readonly Timer scrollEndTimer = new Timer();
    private bool hVisible;
    private bool vVisible;
    private ExtendedBorderStyle borderStyle = ExtendedBorderStyle.Flat;
    private AutoScrollMode autoScrollMode = AutoScrollMode.Drag;
    private bool enableStick = true;
    private int dragScrollRegion = 10;
    private int oldValue = -100000;
    private System.Drawing.Size virtualSize;
    private bool blockOwnResize;
    private bool blockUpdateScrollbars;
    private System.Drawing.Point panStart;
    private System.Drawing.Point panStartScrollPosition;
    private System.Drawing.Point scrollDelta;

    public ScrollControl()
    {
      this.SetStyle(ControlStyles.Selectable, true);
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.scrollTimer.Interval = 20;
      this.scrollTimer.Tick += new EventHandler(this.ScrollTimerTick);
      this.scrollTimer.Enabled = false;
      this.scrollEndTimer.Interval = 250;
      this.scrollEndTimer.Enabled = false;
      this.scrollEndTimer.Tick += (EventHandler) ((s, e) =>
      {
        this.scrollEndTimer.Stop();
        this.InScrollOrResize = false;
        this.Invalidate();
      });
      this.hScrollbar.TabStop = this.vScrollBar.TabStop = this.thumbStick.TabStop = false;
      this.hScrollbar.Visible = this.vScrollBar.Visible = this.thumbStick.Visible = true;
      this.hVisible = this.vVisible = true;
      this.thumbStick.Sensitivity = new SizeF(32f, 32f);
      this.thumbStick.Acceleration = 4f;
      this.hScrollbar.ValueChanged += new EventHandler(this.ScrollValueChanged);
      this.vScrollBar.ValueChanged += new EventHandler(this.ScrollValueChanged);
      this.hScrollbar.VisibleChanged += new EventHandler(this.ScrollbarVisibilityChanged);
      this.vScrollBar.VisibleChanged += new EventHandler(this.ScrollbarVisibilityChanged);
      this.thumbStick.Scroll += new EventHandler(this.ThumbStickScroll);
      this.Controls.Add((Control) this.hScrollbar);
      this.Controls.Add((Control) this.vScrollBar);
      this.Controls.Add((Control) this.thumbStick);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.hScrollbar.Dispose();
        this.vScrollBar.Dispose();
        this.thumbStick.Dispose();
        this.scrollTimer.Dispose();
        this.scrollEndTimer.Dispose();
      }
      base.Dispose(disposing);
    }

    [DefaultValue(ExtendedBorderStyle.Flat)]
    public ExtendedBorderStyle BorderStyle
    {
      get => this.borderStyle;
      set
      {
        if (this.borderStyle == value)
          return;
        this.borderStyle = value;
        this.UpdateScrollbars();
        this.Invalidate();
      }
    }

    [DefaultValue(AutoScrollMode.Drag)]
    public AutoScrollMode AutoScrollMode
    {
      get => this.autoScrollMode;
      set => this.autoScrollMode = value;
    }

    [DefaultValue(true)]
    public bool EnableStick
    {
      get => this.enableStick;
      set
      {
        if (this.enableStick == value)
          return;
        this.enableStick = value;
      }
    }

    [DefaultValue(null)]
    public Cursor PanCursor { get; set; }

    [DefaultValue(10)]
    public int DragScrollRegion
    {
      get => this.dragScrollRegion;
      set => this.dragScrollRegion = value;
    }

    public override Rectangle DisplayRectangle
    {
      get
      {
        Rectangle displayRectangle = BorderUtility.AdjustBorder(base.DisplayRectangle, this.borderStyle);
        displayRectangle.Width -= this.vVisible ? this.vScrollBar.Width : 0;
        displayRectangle.Height -= this.hVisible ? this.hScrollbar.Height : 0;
        if (displayRectangle.Width < 0)
          displayRectangle.Width = 0;
        if (displayRectangle.Height < 0)
          displayRectangle.Height = 0;
        return displayRectangle;
      }
    }

    public virtual Rectangle ViewRectangle => this.DisplayRectangle;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ScrollPositionX
    {
      get => this.hScrollbar.Value;
      set
      {
        this.hScrollbar.Value = value.Clamp(this.hScrollbar.Minimum, this.hScrollbar.Maximum - this.hScrollbar.LargeChange);
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ScrollPositionY
    {
      get => this.vScrollBar.Value;
      set
      {
        if (this.oldValue > value)
          Trace.WriteLine("Scroll Underflow");
        this.oldValue = value;
        this.vScrollBar.Value = value.Clamp(this.vScrollBar.Minimum, this.vScrollBar.Maximum - this.vScrollBar.LargeChange);
      }
    }

    [DefaultValue(typeof (System.Drawing.Point), "0, 0")]
    public System.Drawing.Point ScrollPosition
    {
      get => new System.Drawing.Point(this.ScrollPositionX, this.ScrollPositionY);
      set
      {
        this.ScrollPositionX = value.X;
        this.ScrollPositionY = value.Y;
      }
    }

    [DefaultValue(typeof (System.Drawing.Size), "0, 0")]
    public System.Drawing.Size VirtualSize
    {
      get => this.virtualSize;
      set
      {
        if (this.virtualSize == value)
          return;
        this.virtualSize = value;
        this.UpdateScrollbars();
      }
    }

    [DefaultValue(16)]
    public virtual int LineHeight { get; set; } = 16;

    [DefaultValue(16)]
    public virtual int ColumnWidth { get; set; } = 16;

    [DefaultValue(false)]
    public bool ScrollResizeRefresh { get; set; }

    public bool InScrollOrResize { get; private set; }

    public System.Drawing.Point Translate(System.Drawing.Point pt, bool fromClient)
    {
      if (fromClient)
      {
        pt.Offset(this.ScrollPosition.X, this.ScrollPosition.Y);
        pt.Offset(-this.ViewRectangle.X, -this.ViewRectangle.Y);
      }
      else
      {
        pt.Offset(-this.ScrollPosition.X, -this.ScrollPosition.Y);
        ref System.Drawing.Point local = ref pt;
        Rectangle viewRectangle = this.ViewRectangle;
        int x = viewRectangle.X;
        viewRectangle = this.ViewRectangle;
        int y = viewRectangle.Y;
        local.Offset(x, y);
      }
      return pt;
    }

    public Rectangle Translate(Rectangle rc, bool fromClient)
    {
      rc.Location = this.Translate(rc.Location, fromClient);
      return rc;
    }

    public event EventHandler<AutoScrollEventArgs> AutoScrolling;

    protected virtual void OnAutoScrolling(AutoScrollEventArgs e)
    {
      EventHandler<AutoScrollEventArgs> autoScrolling = this.AutoScrolling;
      if (autoScrolling == null)
        return;
      autoScrolling((object) this, e);
    }

    private void ScrollValueChanged(object sender, EventArgs e)
    {
      this.Invalidate();
      this.OnScroll();
    }

    private void ScrollbarVisibilityChanged(object sender, EventArgs e) => this.Invalidate();

    private void ThumbStickScroll(object sender, EventArgs e)
    {
      if (this.vVisible)
        this.vScrollBar.Value = (this.vScrollBar.Value + (int) ((double) this.thumbStick.Movement.Y * (double) this.vScrollBar.LargeChange * 10.0)).Clamp(this.vScrollBar.Minimum, this.vScrollBar.Maximum - this.vScrollBar.LargeChange);
      if (!this.hVisible)
        return;
      this.hScrollbar.Value = (this.hScrollbar.Value + (int) ((double) this.thumbStick.Movement.X * (double) this.hScrollbar.LargeChange * 10.0)).Clamp(this.hScrollbar.Minimum, this.hScrollbar.Maximum - this.hScrollbar.LargeChange);
    }

    public event EventHandler Scroll;

    public event EventHandler ViewResized;

    protected virtual void OnScroll()
    {
      if (this.ScrollResizeRefresh)
      {
        this.InScrollOrResize = true;
        this.scrollEndTimer.Stop();
        this.scrollEndTimer.Start();
      }
      EventHandler scroll = this.Scroll;
      if (scroll == null)
        return;
      scroll((object) this, EventArgs.Empty);
    }

    protected virtual void OnViewResized()
    {
      if (this.ScrollResizeRefresh)
      {
        this.InScrollOrResize = true;
        this.scrollEndTimer.Stop();
        this.scrollEndTimer.Start();
      }
      this.OnResize(EventArgs.Empty);
      EventHandler viewResized = this.ViewResized;
      if (viewResized == null)
        return;
      viewResized((object) this, EventArgs.Empty);
    }

    protected virtual bool OnPanHitTest(MouseButtons buttons, System.Drawing.Point location)
    {
      return true;
    }

    protected override bool ScaleChildren => false;

    private void UpdateScrollbars()
    {
      if (this.blockUpdateScrollbars)
        return;
      this.blockUpdateScrollbars = true;
      try
      {
        for (int index = 0; index < 2; ++index)
        {
          Rectangle viewRectangle1 = this.ViewRectangle;
          Rectangle viewRectangle2 = this.ViewRectangle;
          if (this.virtualSize.Width <= viewRectangle2.Width)
          {
            this.hVisible = false;
            this.hScrollbar.Value = 0;
            this.hScrollbar.Maximum = int.MaxValue;
          }
          else
          {
            this.hVisible = true;
            this.hScrollbar.Minimum = 0;
            this.hScrollbar.Maximum = this.virtualSize.Width;
            this.hScrollbar.LargeChange = viewRectangle2.Width;
            this.hScrollbar.SmallChange = this.ColumnWidth;
            this.hScrollbar.Value = this.hScrollbar.Value.Clamp(this.hScrollbar.Minimum, this.hScrollbar.Maximum - this.hScrollbar.LargeChange);
          }
          if (this.virtualSize.Height <= viewRectangle2.Height)
          {
            this.vVisible = false;
            this.vScrollBar.Value = 0;
            this.vScrollBar.Maximum = int.MaxValue;
          }
          else
          {
            this.vVisible = true;
            this.vScrollBar.Minimum = 0;
            this.vScrollBar.Maximum = this.virtualSize.Height;
            this.vScrollBar.LargeChange = viewRectangle2.Height;
            this.vScrollBar.SmallChange = this.LineHeight;
            this.vScrollBar.Value = this.vScrollBar.Value.Clamp(this.vScrollBar.Minimum, this.vScrollBar.Maximum - this.vScrollBar.LargeChange);
          }
          if (viewRectangle1 == this.ViewRectangle)
            break;
          this.blockOwnResize = true;
          try
          {
            this.OnViewResized();
          }
          finally
          {
            this.blockOwnResize = false;
          }
        }
      }
      finally
      {
        this.blockUpdateScrollbars = false;
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      this.hScrollbar.Visible = this.hVisible;
      this.vScrollBar.Visible = this.vVisible;
      base.OnPaint(e);
      BorderUtility.DrawBorder(e.Graphics, base.DisplayRectangle, this.borderStyle);
      e.Graphics.IntersectClip(this.DisplayRectangle);
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
      base.OnLayout(levent);
      if (this.blockOwnResize)
        return;
      Rectangle rectangle = BorderUtility.AdjustBorder(base.DisplayRectangle, this.borderStyle);
      this.UpdateScrollbars();
      this.hScrollbar.SetBounds(rectangle.Left, rectangle.Bottom - this.hScrollbar.Height, rectangle.Width - (this.vVisible ? this.vScrollBar.Width : 0), SystemInformation.HorizontalScrollBarHeight);
      this.vScrollBar.SetBounds(rectangle.Right - this.vScrollBar.Width, rectangle.Top, SystemInformation.VerticalScrollBarWidth, rectangle.Height - (this.hVisible ? this.hScrollbar.Height : 0));
      this.thumbStick.Width = this.vScrollBar.Width;
      this.thumbStick.Height = this.hScrollbar.Height;
      if (this.enableStick && (this.vVisible || this.hVisible))
      {
        this.thumbStick.Visible = true;
        this.thumbStick.Location = new System.Drawing.Point(rectangle.Right - this.thumbStick.Width, rectangle.Bottom - this.thumbStick.Height);
        if (this.vVisible && this.hVisible)
          return;
        if (this.vVisible)
          this.vScrollBar.Height -= this.thumbStick.Height;
        if (!this.hVisible)
          return;
        this.hScrollbar.Width -= this.thumbStick.Width;
      }
      else
        this.thumbStick.Visible = false;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.Focus();
      switch (this.autoScrollMode)
      {
        case AutoScrollMode.Pan:
          if (!this.OnPanHitTest(e.Button, e.Location))
            break;
          this.panStartScrollPosition = this.ScrollPosition;
          this.panStart = e.Location;
          if (!(this.PanCursor != (Cursor) null))
            break;
          Cursor.Current = this.PanCursor;
          break;
        case AutoScrollMode.Drag:
          if (e.Button != MouseButtons.Left)
            break;
          this.scrollDelta = ScrollControl.GetDelta(this.DisplayRectangle, e.Location);
          this.scrollTimer.Start();
          break;
      }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.panStart.IsEmpty)
      {
        this.scrollDelta = ScrollControl.GetDelta(this.DisplayRectangle, e.Location);
      }
      else
      {
        if (this.PanCursor != (Cursor) null)
          Cursor.Current = this.PanCursor;
        System.Drawing.Point startScrollPosition = this.panStartScrollPosition;
        startScrollPosition.Offset(this.panStart);
        startScrollPosition.Offset(-e.X, -e.Y);
        this.ScrollPosition = startScrollPosition;
      }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.panStart = System.Drawing.Point.Empty;
      this.scrollTimer.Stop();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      this.scrollTimer.Stop();
    }

    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      this.scrollDelta = ScrollControl.GetDelta(this.ViewRectangle.Pad(this.dragScrollRegion), this.PointToClient(new System.Drawing.Point(e.X, e.Y)));
      this.scrollTimer.Start();
    }

    protected override void OnDragLeave(EventArgs e)
    {
      base.OnDragLeave(e);
      this.scrollTimer.Stop();
    }

    private static System.Drawing.Point GetDelta(Rectangle rc, System.Drawing.Point pos)
    {
      System.Drawing.Point empty = System.Drawing.Point.Empty;
      if (pos.X < rc.Left)
        empty.X = pos.X - rc.Left;
      else if (pos.X > rc.Right)
        empty.X = pos.X - rc.Right;
      if (pos.Y < rc.Top)
        empty.Y = pos.Y - rc.Top;
      else if (pos.Y > rc.Bottom)
        empty.Y = pos.Y - rc.Bottom;
      return empty;
    }

    private void ScrollTimerTick(object sender, EventArgs e)
    {
      System.Drawing.Point scrollPosition = this.ScrollPosition;
      System.Drawing.Point scrollDelta = this.scrollDelta;
      int num1 = Math.Sign(scrollDelta.X);
      int num2 = Math.Sign(scrollDelta.Y);
      scrollDelta.X = Math.Abs(scrollDelta.X);
      scrollDelta.Y = Math.Abs(scrollDelta.Y);
      if (scrollDelta.X > 10)
        scrollDelta.X = 10 + (scrollDelta.X - 10) / 10;
      if (scrollDelta.Y > 10)
        scrollDelta.Y = 10 + (scrollDelta.Y - 10) / 10;
      scrollDelta.X *= num1;
      scrollDelta.Y *= num2;
      AutoScrollEventArgs e1 = new AutoScrollEventArgs()
      {
        Delta = scrollDelta
      };
      this.OnAutoScrolling(e1);
      if (e1.Cancel)
        return;
      scrollPosition.Offset(e1.Delta);
      this.ScrollPosition = scrollPosition;
    }
  }
}

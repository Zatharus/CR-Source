// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.AutoHideManager
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class AutoHideManager : Component
  {
    private readonly Timer timer = new Timer();
    private bool enabled = true;
    private Control control;
    private Control autoHideControl;
    private Rectangle hotBounds;
    private AutoHideBounds autoBounds;
    private int autoWidth = 20;
    private TimeSpan showTime = TimeSpan.FromSeconds(2.0);
    private TimeSpan hideTime = TimeSpan.FromSeconds(2.0);
    private bool mouseInRegion;
    private bool inAutoHideControl;
    private DateTime startInRegion = DateTime.MinValue;
    private DateTime startOutRegion = DateTime.MinValue;

    public AutoHideManager()
    {
      this.timer.Interval = 250;
      this.timer.Enabled = true;
      this.timer.Tick += new EventHandler(this.timer_Tick);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.timer.Dispose();
      base.Dispose(disposing);
    }

    [DefaultValue(true)]
    public bool Enabled
    {
      get => this.enabled;
      set
      {
        this.enabled = value;
        this.HitTest();
        if (this.autoHideControl == null)
          return;
        this.autoHideControl.Hide();
      }
    }

    [DefaultValue(null)]
    public Control Control
    {
      get => this.control;
      set
      {
        if (this.control != null)
        {
          this.control.MouseMove -= new MouseEventHandler(this.control_MouseMove);
          this.control.MouseLeave -= new EventHandler(this.control_MouseLeave);
        }
        this.control = value;
        if (this.control == null)
          return;
        this.control.MouseMove += new MouseEventHandler(this.control_MouseMove);
        this.control.MouseLeave += new EventHandler(this.control_MouseLeave);
      }
    }

    [DefaultValue(null)]
    public Control AutoHideControl
    {
      get => this.autoHideControl;
      set
      {
        if (this.autoHideControl != null)
        {
          this.autoHideControl.Leave -= new EventHandler(this.autoHideControl_Leave);
          this.autoHideControl.VisibleChanged -= new EventHandler(this.autoHideControl_VisibleChanged);
          this.autoHideControl.MouseEnter -= new EventHandler(this.autoHideControl_MouseEnter);
          this.autoHideControl.MouseLeave -= new EventHandler(this.autoHideControl_MouseLeave);
        }
        this.autoHideControl = value;
        if (this.autoHideControl == null)
          return;
        this.autoHideControl.Leave += new EventHandler(this.autoHideControl_Leave);
        this.autoHideControl.VisibleChanged += new EventHandler(this.autoHideControl_VisibleChanged);
        this.autoHideControl.MouseEnter += new EventHandler(this.autoHideControl_MouseEnter);
        this.autoHideControl.MouseLeave += new EventHandler(this.autoHideControl_MouseLeave);
      }
    }

    private void autoHideControl_VisibleChanged(object sender, EventArgs e)
    {
      Control control = (Control) sender;
      if (!control.Visible && control.Focused && control.Parent != null)
        control.Parent.Focus();
      if (control.Visible)
        return;
      this.inAutoHideControl = false;
    }

    [DefaultValue(typeof (Rectangle), "0, 0, 0, 0")]
    public Rectangle HotBounds
    {
      get => this.hotBounds;
      set => this.hotBounds = value;
    }

    [DefaultValue(AutoHideBounds.None)]
    public AutoHideBounds AutoBounds
    {
      get => this.autoBounds;
      set => this.autoBounds = value;
    }

    [DefaultValue(20)]
    public int AutoWidth
    {
      get => this.autoWidth;
      set => this.autoWidth = value;
    }

    [DefaultValue(typeof (TimeSpan), "00:00:02")]
    public TimeSpan ShowTime
    {
      get => this.showTime;
      set => this.showTime = value;
    }

    [DefaultValue(typeof (TimeSpan), "00:00:02")]
    public TimeSpan HideTime
    {
      get => this.hideTime;
      set => this.hideTime = value;
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      if (this.autoHideControl == null || this.Control == null || this.DesignMode)
        return;
      if (this.mouseInRegion)
      {
        if (!(DateTime.Now - this.startInRegion > this.showTime))
          return;
        this.autoHideControl.Show();
        this.timer.Enabled = false;
      }
      else
      {
        if (!(DateTime.Now - this.startOutRegion > this.hideTime))
          return;
        this.autoHideControl.Hide();
        this.timer.Enabled = false;
      }
    }

    private void control_MouseMove(object sender, MouseEventArgs e) => this.HitTest();

    private void autoHideControl_Leave(object sender, EventArgs e)
    {
      ((Control) sender).Hide();
      this.mouseInRegion = false;
      this.startOutRegion = DateTime.Now;
    }

    private void control_MouseLeave(object sender, EventArgs e) => this.HitTest();

    private void autoHideControl_MouseLeave(object sender, EventArgs e)
    {
      this.inAutoHideControl = false;
      this.HitTest();
    }

    private void autoHideControl_MouseEnter(object sender, EventArgs e)
    {
      this.inAutoHideControl = true;
      this.HitTest();
    }

    private void HitTest()
    {
      bool flag = this.inAutoHideControl || this.Control != null && this.TestRegion(this.Control.PointToClient(Cursor.Position));
      if (flag == this.mouseInRegion)
        return;
      if (flag)
        this.startInRegion = DateTime.Now;
      else
        this.startOutRegion = DateTime.Now;
      this.mouseInRegion = flag;
      this.timer.Enabled = true;
    }

    private Rectangle GetHotBounds()
    {
      if (!this.enabled || this.control == null || !this.control.Focused)
        return Rectangle.Empty;
      switch (this.autoBounds)
      {
        case AutoHideBounds.Top:
          return new Rectangle(0, 0, this.control.Width, this.autoWidth);
        case AutoHideBounds.Bottom:
          return new Rectangle(0, this.control.Height - this.autoWidth, this.control.Width, this.autoWidth);
        case AutoHideBounds.Left:
          return new Rectangle(0, 0, this.autoWidth, this.control.Height);
        case AutoHideBounds.Right:
          return new Rectangle(this.control.Width - this.autoWidth, 0, this.autoWidth, this.control.Height);
        default:
          return this.hotBounds;
      }
    }

    private bool TestRegion(System.Drawing.Point pt) => this.GetHotBounds().Contains(pt);
  }
}

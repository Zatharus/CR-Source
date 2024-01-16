// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.FastScrollControl
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
  public class FastScrollControl : UserControl
  {
    private int lineHeight = 16;
    private int columnWidth = 16;
    private IContainer components;

    public FastScrollControl()
    {
      this.InitializeComponent();
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.UserPaint, true);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ScrollPositionX
    {
      get => this.AutoScrollPosition.X;
      set => this.AutoScrollPosition = new System.Drawing.Point(value, -this.AutoScrollPosition.Y);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ScrollPositionY
    {
      get => -this.AutoScrollPosition.Y;
      set => this.AutoScrollPosition = new System.Drawing.Point(this.AutoScrollPosition.X, value);
    }

    [DefaultValue(typeof (System.Drawing.Point), "0, 0")]
    public System.Drawing.Point ScrollPosition
    {
      get
      {
        System.Drawing.Point autoScrollPosition = this.AutoScrollPosition;
        int x = autoScrollPosition.X;
        autoScrollPosition = this.AutoScrollPosition;
        int y = -autoScrollPosition.Y;
        return new System.Drawing.Point(x, y);
      }
      set => this.AutoScrollPosition = new System.Drawing.Point(value.X, value.Y);
    }

    [DefaultValue(typeof (System.Drawing.Size), "0, 0")]
    public System.Drawing.Size VirtualSize
    {
      get => this.AutoScrollMinSize;
      set => this.AutoScrollMinSize = value;
    }

    [DefaultValue(16)]
    public virtual int LineHeight
    {
      get => this.lineHeight;
      set => this.lineHeight = value;
    }

    [DefaultValue(16)]
    public virtual int ColumnWidth
    {
      get => this.columnWidth;
      set => this.columnWidth = value;
    }

    [DefaultValue(true)]
    public bool EnableStick { get; set; }

    public virtual Rectangle ViewRectangle
    {
      get
      {
        Rectangle displayRectangle = this.DisplayRectangle;
        displayRectangle.Offset(this.ScrollPosition);
        return displayRectangle;
      }
    }

    protected override void OnScroll(ScrollEventArgs se)
    {
      base.OnScroll(se);
      this.OnScroll();
    }

    protected virtual void OnScroll()
    {
    }

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

    public event MouseEventHandler MouseHWheel;

    protected virtual void OnAutoScrolling(AutoScrollEventArgs e)
    {
      if (this.AutoScrolling == null)
        return;
      this.AutoScrolling((object) this, e);
    }

    protected virtual void OnMouseHWheel(MouseEventArgs e)
    {
      if (this.MouseHWheel == null)
        return;
      this.MouseHWheel((object) this, e);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.AutoScaleMode = AutoScaleMode.Font;
    }
  }
}

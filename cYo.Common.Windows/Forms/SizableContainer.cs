// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.SizableContainer
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using cYo.Common.Runtime;
using cYo.Common.Windows.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class SizableContainer : ScrollableControl
  {
    public static bool EnableAnimation;
    private System.Drawing.Size dockSize;
    private bool autoGripPosition;
    private bool forceLayout = true;
    private int gripWidth = 6;
    private SizableContainer.GripPosition grip = SizableContainer.GripPosition.Top;
    private bool expanded = true;
    private bool shieldExpanded;
    private int expandedWidth;
    private bool keepHandleVisible = true;
    private readonly Color hotColor = SystemColors.ControlLight;
    private Color pressedColor = SystemColors.Highlight;
    private ExtendedBorderStyle borderStyle;
    private System.Drawing.Point mousePressLocation;
    private Rectangle mousePressBounds;
    private int mousePressWidth;
    private bool first = true;
    private static readonly Bitmap gripImage = Resources.MozillaGrip;
    private SizableContainer.GripState handleState;
    private bool inAnimation;
    private IContainer components;

    public SizableContainer()
    {
      this.SlideTime = 100;
      this.AnimateExpand = true;
      this.InitializeComponent();
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.UserPaint, true);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    [DefaultValue(typeof (System.Drawing.Size), "0, 0")]
    public System.Drawing.Size DockSize
    {
      get => this.dockSize;
      set
      {
        if (this.dockSize == value)
          return;
        this.dockSize = value;
        this.SetDockSize(value);
      }
    }

    [DefaultValue(false)]
    public bool AutoGripPosition
    {
      get => this.autoGripPosition;
      set
      {
        if (this.autoGripPosition == value)
          return;
        this.autoGripPosition = value;
        this.UpdateAutoGripPosition();
      }
    }

    [DefaultValue(true)]
    public bool ForceLayout
    {
      get => this.forceLayout;
      set => this.forceLayout = value;
    }

    [DefaultValue(6)]
    public int GripWidth
    {
      get => this.gripWidth;
      set
      {
        if (this.gripWidth == value)
          return;
        this.gripWidth = value;
        this.PerformLayout();
      }
    }

    [DefaultValue(SizableContainer.GripPosition.Top)]
    public SizableContainer.GripPosition Grip
    {
      get => this.grip;
      set
      {
        if (this.grip == value)
          return;
        this.grip = value;
        this.PerformLayout();
      }
    }

    public bool IsVertical
    {
      get
      {
        return this.Grip == SizableContainer.GripPosition.Right || this.Grip == SizableContainer.GripPosition.Left;
      }
    }

    public bool IsHorizontal
    {
      get
      {
        return this.Grip == SizableContainer.GripPosition.Top || this.Grip == SizableContainer.GripPosition.Bottom;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Expanded
    {
      get => this.expanded;
      set
      {
        if (this.shieldExpanded)
          return;
        this.shieldExpanded = true;
        try
        {
          if (this.expanded == value)
            return;
          this.expanded = value;
          if (this.expanded)
            this.OnExpandedChanged();
          this.UpdateExpanded(this.AnimateExpand && this.IsHandleCreated);
          if (this.expanded)
            return;
          this.OnExpandedChanged();
        }
        finally
        {
          this.shieldExpanded = false;
        }
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Collapsed
    {
      get => !this.Expanded;
      set => this.Expanded = !value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ExpandedWidth
    {
      get => this.expandedWidth;
      set
      {
        if (this.expandedWidth == value)
          return;
        this.SetExpandedWidthInternal(value);
        if (!this.expanded)
          return;
        this.SetExpandedWidth(this.expandedWidth);
      }
    }

    [DefaultValue(true)]
    public bool KeepGripVisible
    {
      get => this.keepHandleVisible;
      set
      {
        if (this.keepHandleVisible == value)
          return;
        this.keepHandleVisible = value;
        if (this.expanded)
          return;
        this.SetExpandedWidth(0);
      }
    }

    [DefaultValue(true)]
    public bool AnimateExpand { get; set; }

    [DefaultValue(100)]
    public int SlideTime { get; set; }

    [DefaultValue(typeof (Color), "ControlLight")]
    public Color HotColor
    {
      get => this.hotColor;
      set
      {
        if (this.hotColor == value || this.State != SizableContainer.GripState.Hoovered)
          return;
        this.Invalidate(this.GripRectangle);
      }
    }

    [DefaultValue(typeof (Color), "Highlight")]
    public Color PressedColor
    {
      get => this.pressedColor;
      set
      {
        if (this.pressedColor == value)
          return;
        this.pressedColor = value;
        if (this.State != SizableContainer.GripState.Pressed)
          return;
        this.Invalidate(this.GripRectangle);
      }
    }

    [DefaultValue(ExtendedBorderStyle.None)]
    public ExtendedBorderStyle BorderStyle
    {
      get => this.borderStyle;
      set
      {
        if (this.borderStyle == value)
          return;
        this.borderStyle = value;
        this.PerformLayout();
        this.Invalidate();
      }
    }

    public override Rectangle DisplayRectangle
    {
      get
      {
        Rectangle bounds = base.DisplayRectangle;
        switch (this.grip)
        {
          case SizableContainer.GripPosition.Top:
            bounds = new Rectangle(bounds.Left, this.gripWidth, bounds.Width, bounds.Height - this.gripWidth);
            break;
          case SizableContainer.GripPosition.Left:
            bounds = new Rectangle(this.gripWidth, bounds.Top, bounds.Width - this.gripWidth, bounds.Height);
            break;
          case SizableContainer.GripPosition.Right:
            bounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width - this.gripWidth, bounds.Height);
            break;
          case SizableContainer.GripPosition.Bottom:
            bounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height - this.gripWidth);
            break;
        }
        return BorderUtility.AdjustBorder(bounds, this.borderStyle);
      }
    }

    public virtual Rectangle GripRectangle
    {
      get
      {
        Rectangle clientRectangle = this.ClientRectangle;
        switch (this.grip)
        {
          case SizableContainer.GripPosition.Top:
            return new Rectangle(clientRectangle.Left, clientRectangle.Top, clientRectangle.Width, this.gripWidth);
          case SizableContainer.GripPosition.Left:
            return new Rectangle(clientRectangle.Left, clientRectangle.Top, this.gripWidth, clientRectangle.Height);
          case SizableContainer.GripPosition.Right:
            return new Rectangle(clientRectangle.Width - this.gripWidth, clientRectangle.Top, this.gripWidth, clientRectangle.Height);
          case SizableContainer.GripPosition.Bottom:
            return new Rectangle(clientRectangle.Left, clientRectangle.Height - this.gripWidth, clientRectangle.Width, this.gripWidth);
          default:
            return clientRectangle;
        }
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      this.OnPaintGrip(e);
      Rectangle displayRectangle = this.DisplayRectangle;
      if (!this.ClientRectangle.IntersectsWith(displayRectangle))
        return;
      BorderUtility.DrawBorder(e.Graphics, BorderUtility.AdjustBorder(displayRectangle, this.borderStyle, false), this.borderStyle);
    }

    private void DrawGrip(Graphics gr, Image image, Rectangle rc)
    {
      float angle = 0.0f;
      if (this.Expanded)
        angle += 180f;
      switch (this.Grip)
      {
        case SizableContainer.GripPosition.Left:
          angle -= 90f;
          break;
        case SizableContainer.GripPosition.Right:
          angle += 90f;
          break;
        case SizableContainer.GripPosition.Bottom:
          angle += 180f;
          break;
      }
      switch (this.State)
      {
        case SizableContainer.GripState.Hoovered:
          using (Brush brush = (Brush) new SolidBrush(this.HotColor))
          {
            gr.FillRectangle(brush, this.GripRectangle);
            break;
          }
        case SizableContainer.GripState.Pressed:
          using (Brush brush = (Brush) new SolidBrush(this.PressedColor))
          {
            gr.FillRectangle(brush, this.GripRectangle);
            break;
          }
        default:
          using (Brush brush = (Brush) new SolidBrush(this.BackColor))
          {
            gr.FillRectangle(brush, this.GripRectangle);
            break;
          }
      }
      using (gr.SaveState())
      {
        gr.TranslateTransform((float) (rc.Left + rc.Width / 2), (float) (rc.Top + rc.Height / 2));
        gr.RotateTransform(angle);
        gr.TranslateTransform((float) (-image.Width / 2), (float) (-image.Height / 2));
        gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
      }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
      base.OnMouseEnter(e);
      if (this.inAnimation)
        return;
      this.State = this.HitTest() ? SizableContainer.GripState.Hoovered : SizableContainer.GripState.None;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (this.inAnimation || (e.Button & MouseButtons.Left) == MouseButtons.None || !this.HitTest(e.Location))
        return;
      this.State = SizableContainer.GripState.Pressed;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.inAnimation)
        return;
      if (this.State != SizableContainer.GripState.Pressed)
      {
        this.State = this.HitTest(e.Location) ? SizableContainer.GripState.Hoovered : SizableContainer.GripState.None;
      }
      else
      {
        System.Drawing.Point position = Cursor.Position;
        System.Drawing.Point mousePressLocation = this.mousePressLocation;
        int width;
        switch (this.Grip)
        {
          case SizableContainer.GripPosition.Top:
            width = this.mousePressBounds.Height - (position.Y - mousePressLocation.Y);
            break;
          case SizableContainer.GripPosition.Left:
            width = this.mousePressBounds.Width - (position.X - mousePressLocation.X);
            break;
          case SizableContainer.GripPosition.Right:
            width = this.mousePressBounds.Width + (position.X - mousePressLocation.X);
            break;
          case SizableContainer.GripPosition.Bottom:
            width = this.mousePressBounds.Height + (position.Y - mousePressLocation.Y);
            break;
          default:
            width = 0;
            break;
        }
        if (width < 0)
          width = 0;
        if (width != 0 && !this.expanded)
        {
          this.expanded = true;
          this.OnExpandedChanged();
        }
        if (width == 0 && this.Expanded)
        {
          this.expanded = false;
          this.SetExpandedWidthInternal(this.mousePressWidth);
          this.OnExpandedChanged();
        }
        this.SetExpandedWidth(width, true, true);
      }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      if (this.inAnimation || (e.Button & MouseButtons.Left) == MouseButtons.None)
        return;
      this.State = this.HitTest(e.Location) ? SizableContainer.GripState.Hoovered : SizableContainer.GripState.None;
      if (Math.Abs(Cursor.Position.X - this.mousePressLocation.X) >= 4 || Math.Abs(Cursor.Position.Y - this.mousePressLocation.Y) >= 4)
        return;
      this.Expanded = !this.Expanded;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      if (this.inAnimation || this.State == SizableContainer.GripState.Pressed)
        return;
      this.State = SizableContainer.GripState.None;
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      if (this.Dock != DockStyle.Fill)
        return;
      this.OnExpandedChanged();
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (this.shieldExpanded || !this.expanded || this.DesignMode)
        return;
      this.expandedWidth = this.IsVertical ? this.Width : this.Height;
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
      base.OnLayout(levent);
      if (!this.first || this.DesignMode)
        return;
      this.first = false;
      this.UpdateExpanded(false);
    }

    protected override void OnDockChanged(EventArgs e)
    {
      base.OnDockChanged(e);
      this.UpdateAutoGripPosition();
      if (!this.Expanded)
        this.SetExpandedWidth(0);
      this.SetDockSize(this.dockSize);
    }

    public event EventHandler ExpandedChanged;

    public event PaintEventHandler PaintGrip;

    protected virtual void OnPaintGrip(PaintEventArgs e)
    {
      this.DrawGrip(e.Graphics, (Image) SizableContainer.gripImage, this.GripRectangle);
      if (this.PaintGrip == null)
        return;
      this.PaintGrip((object) this, e);
    }

    protected virtual void OnExpandedChanged()
    {
      if (this.ExpandedChanged == null)
        return;
      this.ExpandedChanged((object) this, EventArgs.Empty);
    }

    private void SetExpandedWidthInternal(int value)
    {
      this.expandedWidth = value;
      this.dockSize = this.GetDockSize(this.dockSize);
    }

    private System.Drawing.Size GetDockSize(System.Drawing.Size dockSize)
    {
      switch (this.Dock)
      {
        case DockStyle.Top:
        case DockStyle.Bottom:
          dockSize = new System.Drawing.Size(dockSize.Width, this.ExpandedWidth);
          break;
        case DockStyle.Left:
        case DockStyle.Right:
          dockSize = new System.Drawing.Size(this.ExpandedWidth, dockSize.Height);
          break;
      }
      return dockSize;
    }

    private void SetDockSize(System.Drawing.Size dockSize)
    {
      switch (this.Dock)
      {
        case DockStyle.Top:
        case DockStyle.Bottom:
          if (dockSize.Height == 0)
            break;
          this.ExpandedWidth = dockSize.Height;
          break;
        case DockStyle.Left:
        case DockStyle.Right:
          if (dockSize.Width == 0)
            break;
          this.ExpandedWidth = dockSize.Width;
          break;
      }
    }

    private SizableContainer.GripState State
    {
      get => this.handleState;
      set
      {
        if (this.handleState == value)
          return;
        this.handleState = value;
        switch (this.handleState)
        {
          case SizableContainer.GripState.Hoovered:
            this.Cursor = this.IsVertical ? Cursors.VSplit : Cursors.HSplit;
            break;
          case SizableContainer.GripState.Pressed:
            this.mousePressLocation = Cursor.Position;
            this.mousePressBounds = this.Bounds;
            this.mousePressWidth = this.ExpandedWidth;
            this.Cursor = this.IsVertical ? Cursors.VSplit : Cursors.HSplit;
            break;
          default:
            this.Cursor = Cursors.Default;
            break;
        }
        this.Invalidate(this.GripRectangle);
      }
    }

    private bool HitTest(System.Drawing.Point pt) => this.GripRectangle.Contains(pt);

    private bool HitTest() => this.HitTest(this.PointToClient(Cursor.Position));

    private Rectangle GetExpandedBounds(int width)
    {
      Rectangle bounds = this.Bounds;
      switch (this.grip)
      {
        case SizableContainer.GripPosition.Top:
          bounds.Y = bounds.Bottom - width;
          bounds.Height = width;
          break;
        case SizableContainer.GripPosition.Left:
          bounds.X = bounds.Right - width;
          bounds.Width = width;
          break;
        case SizableContainer.GripPosition.Right:
          bounds.Width = width;
          break;
        case SizableContainer.GripPosition.Bottom:
          bounds.Height = width;
          break;
      }
      return bounds;
    }

    private void SetExpandedWidth(int width, bool clip, bool keepHandle)
    {
      width = Math.Max(keepHandle ? this.gripWidth : 0, width);
      this.SetBounds(this.GetExpandedBounds(width), clip);
      this.dockSize = this.GetDockSize(this.dockSize);
    }

    private void SetExpandedWidth(int width, bool clip)
    {
      this.SetExpandedWidth(width, clip, this.keepHandleVisible);
    }

    private void SetExpandedWidth(int width) => this.SetExpandedWidth(width, false);

    private void UpdateExpanded(bool animate)
    {
      animate &= SizableContainer.EnableAnimation;
      if (this.Dock == DockStyle.Fill)
        this.Visible = this.expanded;
      else if (this.expanded)
      {
        if (animate)
          this.AnimateWindow(0, this.expandedWidth, this.SlideTime);
        else
          this.SetExpandedWidth(this.expandedWidth, true);
      }
      else if (animate)
        this.AnimateWindow(this.expandedWidth, 0, this.SlideTime);
      else
        this.SetExpandedWidth(0, true);
    }

    private void AnimateWindow(int start, int end, int slideTime)
    {
      this.inAnimation = true;
      System.Drawing.Size minimumSize = this.Controls[0].MinimumSize;
      try
      {
        int width = start;
        int num1 = end - start;
        long num2 = (long) slideTime;
        long ticks = Machine.Ticks;
        if (this.Controls.Count == 1)
          this.Controls[0].MinimumSize = num1 < 0 ? this.Controls[0].Size : this.GetExpandedBounds(end).Size;
        while (width != end)
        {
          float num3 = Math.Min((float) (Machine.Ticks - ticks) / (float) num2, 1f);
          width = start + (int) ((double) num1 * (double) num3);
          this.SetExpandedWidth(width, true);
        }
      }
      finally
      {
        this.Controls[0].MinimumSize = minimumSize;
        this.inAnimation = false;
      }
    }

    private void SetBounds(Rectangle rc, bool clip)
    {
      Control parent = this.Parent;
      if (clip && parent != null)
        rc = Rectangle.Intersect(parent.DisplayRectangle, rc);
      if (rc == this.Bounds || rc.IsEmpty)
        return;
      this.Bounds = rc;
      if (parent == null)
        return;
      parent.PerformLayout((Control) this, "Bounds");
      if (!this.forceLayout)
        return;
      parent.Update();
    }

    private void UpdateAutoGripPosition()
    {
      if (!this.autoGripPosition)
        return;
      switch (this.Dock)
      {
        case DockStyle.Top:
          this.Grip = SizableContainer.GripPosition.Bottom;
          break;
        case DockStyle.Bottom:
          this.Grip = SizableContainer.GripPosition.Top;
          break;
        case DockStyle.Left:
          this.Grip = SizableContainer.GripPosition.Right;
          break;
        case DockStyle.Right:
          this.Grip = SizableContainer.GripPosition.Left;
          break;
        default:
          this.Grip = SizableContainer.GripPosition.None;
          break;
      }
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.Name = "SplitContainer";
      this.Size = new System.Drawing.Size(336, 285);
      this.ResumeLayout(false);
    }

    public enum GripPosition
    {
      None,
      Top,
      Left,
      Right,
      Bottom,
    }

    public enum GripStyle
    {
      None,
      Mozilla,
      Office,
    }

    private enum GripState
    {
      None,
      Hoovered,
      Pressed,
    }
  }
}

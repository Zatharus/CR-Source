// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.SplitterEx
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class SplitterEx : Control
  {
    private Control sibling;
    private System.Drawing.Point clickOffset;

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Rectangle displayRectangle = this.DisplayRectangle;
      ++displayRectangle.Width;
      ++displayRectangle.Height;
      if (this.Capture)
        ControlPaint.DrawBorder3D(e.Graphics, displayRectangle, Border3DStyle.SunkenInner);
      else
        ControlPaint.DrawBorder3D(e.Graphics, displayRectangle, Border3DStyle.RaisedInner);
    }

    protected static void DrawGrip(Graphics gr, Rectangle bounds, Pen pen, Brush brush)
    {
      for (int x = bounds.Left + 1; x < bounds.Right; x += 8)
      {
        using (GraphicsPath arrowPath = PathUtility.GetArrowPath(new Rectangle(x, 0, 6, bounds.Height), true))
        {
          gr.FillPath(brush, arrowPath);
          gr.DrawPath(pen, arrowPath);
        }
      }
    }

    [DefaultValue(null)]
    public Control Sibling
    {
      get => this.sibling;
      set => this.sibling = value;
    }

    protected override void OnDockChanged(EventArgs e)
    {
      base.OnDockChanged(e);
      this.Cursor = this.IsHorizontal ? Cursors.HSplit : Cursors.VSplit;
    }

    private Control GetSibling(int offset)
    {
      if (this.sibling != null)
        return this.sibling;
      Control parent = this.Parent;
      int index = parent.Controls.IndexOf((Control) this) + offset;
      return index >= 0 && index < parent.Controls.Count ? parent.Controls[index] : (Control) null;
    }

    protected virtual void OnSplitterMoving(SplitterEventArgs sevent)
    {
      int offset = 0;
      Control sibling;
      do
      {
        switch (this.Dock)
        {
          case DockStyle.Top:
          case DockStyle.Left:
            --offset;
            break;
          case DockStyle.Bottom:
          case DockStyle.Right:
            ++offset;
            break;
          default:
            return;
        }
        sibling = this.GetSibling(offset);
      }
      while (sibling != null && sibling.Dock != this.Dock);
      if (sibling == null)
        return;
      this.SuspendLayout();
      try
      {
        switch (this.Dock)
        {
          case DockStyle.Top:
            sibling.Height = sevent.SplitY - this.Height - sibling.Top;
            break;
          case DockStyle.Bottom:
            sibling.Height = sibling.Bottom - sevent.SplitY - this.Height;
            break;
          case DockStyle.Left:
            sibling.Width = sevent.SplitX - this.Width - sibling.Left;
            break;
          case DockStyle.Right:
            sibling.Width = sibling.Right - sevent.SplitX - this.Width;
            break;
        }
      }
      finally
      {
        this.ResumeLayout();
        this.Parent.Update();
      }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.Invalidate();
      this.clickOffset = e.Location;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!this.Capture)
        return;
      System.Drawing.Point location1 = this.Location;
      System.Drawing.Point location2 = e.Location;
      location2.X -= this.clickOffset.X;
      location2.Y -= this.clickOffset.Y;
      if (this.IsHorizontal)
        location1.Y += location2.Y;
      else
        location1.X += location2.X;
      this.OnSplitterMoving(new SplitterEventArgs(e.X, e.Y, location1.X, location1.Y));
    }

    public bool IsHorizontal => this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom;
  }
}

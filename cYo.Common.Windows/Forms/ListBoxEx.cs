// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ListBoxEx
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ListBoxEx : ListBox
  {
    private System.Drawing.Point dragDown;

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.dragDown = e.Location;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.dragDown = System.Drawing.Point.Empty;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (e.Button != MouseButtons.Left)
        return;
      int num1 = Math.Abs(e.X - this.dragDown.X);
      System.Drawing.Size dragSize = SystemInformation.DragSize;
      int width = dragSize.Width;
      if (num1 <= width)
      {
        int num2 = Math.Abs(e.Y - this.dragDown.Y);
        dragSize = SystemInformation.DragSize;
        int height = dragSize.Height;
        if (num2 <= height)
          return;
      }
      if (this.SelectedItem == null)
        return;
      this.OnItemDrag(new ItemDragEventArgs(e.Button, this.SelectedItem));
    }

    public event ItemDragEventHandler ItemDrag;

    protected virtual void OnItemDrag(ItemDragEventArgs e)
    {
      if (this.ItemDrag == null)
        return;
      this.ItemDrag((object) this, e);
    }
  }
}

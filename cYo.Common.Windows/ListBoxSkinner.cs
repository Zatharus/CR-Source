// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.ListBoxSkinner
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Windows.Forms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows
{
  public class ListBoxSkinner : Component
  {
    private readonly ListBox listBox;

    public ListBoxSkinner(ListBox listBox)
    {
      this.listBox = listBox;
      listBox.DrawMode = DrawMode.OwnerDrawFixed;
      listBox.DrawItem += new DrawItemEventHandler(this.DrawItem);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.listBox.DrawItem -= new DrawItemEventHandler(this.DrawItem);
      base.Dispose(disposing);
    }

    private void DrawItem(object sender, DrawItemEventArgs e)
    {
      string itemText = this.listBox.GetItemText(this.listBox.Items[e.Index]);
      e.DrawBackground();
      e.Graphics.DrawStyledRectangle(e.Bounds, StyledRenderer.GetAlphaStyle(e.State.HasFlag((Enum) DrawItemState.Selected), e.State.HasFlag((Enum) DrawItemState.HotLight), e.State.HasFlag((Enum) DrawItemState.Focus)), StyledRenderer.GetSelectionColor(this.listBox.Focused));
      using (SolidBrush solidBrush = new SolidBrush(e.ForeColor))
        e.Graphics.DrawString(itemText, e.Font, (Brush) solidBrush, (RectangleF) e.Bounds);
    }
  }
}

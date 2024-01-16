// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.CheckedListBoxEx
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class CheckedListBoxEx : CheckedListBox
  {
    private bool customDrawing = true;
    private int downIndex = -1;
    private bool downCheck;
    private bool pendingCheck;

    [DefaultValue(true)]
    public bool CustomDrawing
    {
      get => this.customDrawing;
      set => this.customDrawing = value;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
      if (e.Index >= this.Items.Count || e.Index < 0)
        return;
      if (!this.customDrawing)
      {
        base.OnDrawItem(e);
      }
      else
      {
        using (Brush brush = (Brush) new SolidBrush(this.BackColor))
          e.Graphics.FillRectangle(brush, e.Bounds);
        CheckState itemCheckState = this.GetItemCheckState(e.Index);
        System.Drawing.Size size;
        Rectangle bounds;
        if (Application.RenderWithVisualStyles)
        {
          CheckBoxState state = itemCheckState == CheckState.Unchecked || itemCheckState != CheckState.Checked ? CheckBoxState.UncheckedNormal : CheckBoxState.CheckedNormal;
          size = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
          System.Drawing.Point glyphLocation;
          ref System.Drawing.Point local = ref glyphLocation;
          bounds = e.Bounds;
          int x = bounds.X + 1;
          bounds = e.Bounds;
          int y1 = bounds.Y;
          bounds = e.Bounds;
          int num = (bounds.Height - size.Height) / 2;
          int y2 = y1 + num;
          local = new System.Drawing.Point(x, y2);
          CheckBoxRenderer.DrawCheckBox(e.Graphics, glyphLocation, state);
        }
        else
        {
          ButtonState state = itemCheckState == CheckState.Unchecked || itemCheckState != CheckState.Checked ? ButtonState.Normal : ButtonState.Checked;
          size = new System.Drawing.Size(14, 14);
          Rectangle rectangle = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + (e.Bounds.Height - size.Height) / 2, size.Width, size.Height);
          ControlPaint.DrawCheckBox(e.Graphics, rectangle, state);
        }
        Rectangle rectangle1;
        ref Rectangle local1 = ref rectangle1;
        bounds = e.Bounds;
        int x1 = bounds.X + size.Width + 2;
        bounds = e.Bounds;
        int y = bounds.Y;
        bounds = e.Bounds;
        int width = bounds.Width - (size.Width + 2);
        bounds = e.Bounds;
        int height = bounds.Height;
        local1 = new Rectangle(x1, y, width, height);
        using (Brush brush = (Brush) new SolidBrush(e.BackColor))
          e.Graphics.FillRectangle(brush, rectangle1);
        this.OnDrawItemText(new DrawItemEventArgs(e.Graphics, e.Font, rectangle1, e.Index, e.State, e.ForeColor, e.BackColor));
        if ((e.State & DrawItemState.Focus) == DrawItemState.None || (e.State & DrawItemState.NoFocusRect) != DrawItemState.None)
          return;
        ControlPaint.DrawFocusRectangle(e.Graphics, rectangle1);
      }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (this.CheckOnClick)
        return;
      this.downIndex = this.IndexFromPoint(e.Location);
      if (this.downIndex == -1)
        return;
      this.pendingCheck = (this.GetItemRectangle(this.downIndex) with
      {
        Width = 14
      }).Contains(e.Location);
      this.downCheck = this.GetItemChecked(this.downIndex);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      if (this.downIndex == -1 || !this.pendingCheck)
        return;
      this.SetItemChecked(this.downIndex, !this.downCheck);
    }

    public event DrawItemEventHandler DrawItemText;

    protected virtual void OnDrawItemText(DrawItemEventArgs e)
    {
      if (this.DrawItemText != null)
        this.DrawItemText((object) this, e);
      else
        this.DrawDefaultItemText(e);
    }

    public void DrawDefaultItemText(DrawItemEventArgs e)
    {
      using (StringFormat format = new StringFormat()
      {
        LineAlignment = StringAlignment.Center
      })
        this.DrawDefaultItemText(e, format);
    }

    public virtual void DrawDefaultItemText(DrawItemEventArgs e, StringFormat format)
    {
      using (Brush brush = (Brush) new SolidBrush(e.ForeColor))
        e.Graphics.DrawString(this.GetItemText(this.Items[e.Index]), this.Font, brush, (RectangleF) e.Bounds, format);
    }
  }
}

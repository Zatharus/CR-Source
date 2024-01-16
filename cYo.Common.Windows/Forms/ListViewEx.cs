// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ListViewEx
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
  public class ListViewEx : ListView
  {
    private const int ScrollMargin = 10;
    private ListViewItem dragItem;
    private Timer scrollTimer;
    private Color insertLineColor = Color.Black;
    private int insertLineBefore = -1;
    private int insertLineAfter = -1;

    protected override void WndProc(ref Message m)
    {
      base.WndProc(ref m);
      if (m.Msg == 276 || m.Msg == 277)
        this.OnScroll();
      if (m.Msg != 15)
        return;
      if (this.InsertLineBefore >= 0 && this.InsertLineBefore < this.Items.Count)
      {
        Rectangle bounds = this.Items[this.InsertLineBefore].GetBounds(ItemBoundsPortion.Entire);
        this.DrawInsertionLine(bounds.Left, bounds.Right, bounds.Top);
      }
      if (this.InsertLineAfter < 0 || this.InsertLineBefore >= this.Items.Count)
        return;
      Rectangle bounds1 = this.Items[this.InsertLineAfter].GetBounds(ItemBoundsPortion.Entire);
      this.DrawInsertionLine(bounds1.Left, bounds1.Right, bounds1.Bottom);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      this.OnScroll();
      base.OnMouseWheel(e);
    }

    protected override void OnCreateControl()
    {
      base.OnCreateControl();
      this.DoubleBuffered = true;
      this.scrollTimer = new Timer()
      {
        Interval = 100,
        Enabled = false
      };
      this.scrollTimer.Tick += new EventHandler(this.scrollTimer_Tick);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.scrollTimer != null)
        this.scrollTimer.Dispose();
      base.Dispose(disposing);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (!this.EnableMouseReorder || this.View != View.Details)
        return;
      this.dragItem = this.GetItemAt(e.X, e.Y);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.dragItem == null)
        return;
      this.Cursor = Cursors.Hand;
      this.UpdateInsertMarkers();
      if (this.InsertLineBefore == this.TopItem.Index || e.Y > this.Height - 10)
        this.scrollTimer.Start();
      else
        this.scrollTimer.Stop();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      if (this.dragItem == null)
        return;
      this.scrollTimer.Stop();
      try
      {
        if (this.GetItemAt(0, Math.Min(e.Y, this.Items[this.Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1)) == null)
          return;
        int num = -1;
        if (this.InsertLineBefore >= 0)
          num = this.InsertLineBefore;
        else if (this.InsertLineAfter >= 0)
          num = Math.Min(this.InsertLineAfter + 1, this.Items.Count);
        if (this.dragItem.Index < num)
          --num;
        if (num >= 0 && this.dragItem.Index != num)
        {
          ListViewEx.MouseReorderEventArgs e1 = new ListViewEx.MouseReorderEventArgs(this.dragItem, this.dragItem.Index, num);
          this.OnMouseReorder(e1);
          if (!e1.Cancel)
          {
            this.Items.Remove(this.dragItem);
            this.Items.Insert(num, this.dragItem);
          }
        }
        this.InsertLineAfter = this.InsertLineBefore = -1;
      }
      finally
      {
        this.dragItem = (ListViewItem) null;
        this.Cursor = Cursors.Default;
      }
    }

    public event EventHandler Scroll;

    public event EventHandler<ListViewEx.MouseReorderEventArgs> MouseReorder;

    protected virtual void OnScroll()
    {
      if (this.Scroll == null)
        return;
      this.Scroll((object) this, EventArgs.Empty);
    }

    protected virtual void OnMouseReorder(ListViewEx.MouseReorderEventArgs e)
    {
      if (this.MouseReorder == null)
        return;
      this.MouseReorder((object) this, e);
    }

    private void scrollTimer_Tick(object sender, EventArgs e)
    {
      System.Drawing.Point client = this.PointToClient(Cursor.Position);
      if (this.Items.Count == 0)
        return;
      int index1 = this.TopItem.Index;
      int index2 = client.Y >= this.Height / 2 ? index1 + 1 : index1 - 1;
      if (index2 >= 0 && index2 < this.Items.Count)
        this.TopItem = this.Items[index2];
      this.UpdateInsertMarkers();
    }

    [DefaultValue(false)]
    public bool EnableMouseReorder { get; set; }

    [DefaultValue(typeof (Color), "Black")]
    public Color InsertLineColor
    {
      get => this.insertLineColor;
      set
      {
        if (this.insertLineColor == value)
          return;
        this.insertLineColor = value;
        if (this.insertLineBefore < 0 && this.insertLineAfter < 0)
          return;
        this.Invalidate();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int InsertLineBefore
    {
      get => this.insertLineBefore;
      set
      {
        if (this.insertLineBefore == value)
          return;
        this.insertLineBefore = value;
        this.Invalidate();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int InsertLineAfter
    {
      get => this.insertLineAfter;
      set
      {
        if (this.insertLineAfter == value)
          return;
        this.insertLineAfter = value;
        this.Invalidate();
      }
    }

    private bool UpdateInsertMarkers()
    {
      if (this.dragItem == null)
        return false;
      System.Drawing.Point client = this.PointToClient(Cursor.Position);
      ListViewItem itemAt = this.GetItemAt(0, Math.Min(client.Y, this.Items[this.Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1));
      if (itemAt == null)
        return false;
      Rectangle bounds = itemAt.GetBounds(ItemBoundsPortion.Entire);
      if (client.Y < bounds.Top + bounds.Height / 2)
      {
        this.InsertLineBefore = itemAt.Index;
        this.InsertLineAfter = -1;
      }
      else
      {
        this.InsertLineBefore = -1;
        this.InsertLineAfter = itemAt.Index;
      }
      return true;
    }

    private void DrawInsertionLine(int x1, int x2, int y)
    {
      using (Graphics graphics = this.CreateGraphics())
      {
        using (Pen pen = new Pen(this.InsertLineColor))
        {
          using (Brush brush = (Brush) new SolidBrush(this.InsertLineColor))
          {
            graphics.DrawLine(pen, x1, y, x2 - 1, y);
            System.Drawing.Point[] points1 = new System.Drawing.Point[3]
            {
              new System.Drawing.Point(x1, y - 4),
              new System.Drawing.Point(x1 + 7, y),
              new System.Drawing.Point(x1, y + 4)
            };
            System.Drawing.Point[] points2 = new System.Drawing.Point[3]
            {
              new System.Drawing.Point(x2, y - 4),
              new System.Drawing.Point(x2 - 8, y),
              new System.Drawing.Point(x2, y + 4)
            };
            graphics.FillPolygon(brush, points1);
            graphics.FillPolygon(brush, points2);
          }
        }
      }
    }

    private static class Native
    {
      public const int WM_HSCROLL = 276;
      public const int WM_VSCROLL = 277;
      public const int WM_PAINT = 15;
    }

    public class MouseReorderEventArgs : CancelEventArgs
    {
      public MouseReorderEventArgs(ListViewItem item, int from, int to)
      {
        this.Item = item;
        this.FromIndex = from;
        this.ToIndex = to;
      }

      public ListViewItem Item { get; private set; }

      public int FromIndex { get; private set; }

      public int ToIndex { get; private set; }
    }
  }
}

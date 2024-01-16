// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TabControlEx
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TabControlEx : TabControl
  {
    private System.Drawing.Point downPoint;

    [DefaultValue(false)]
    public bool ReorderTabsWhileDragging { get; set; }

    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      this.DragTab(e, this.ReorderTabsWhileDragging);
    }

    private void DragTab(DragEventArgs e, bool reorder)
    {
      TabPage tabPageByTab = this.GetTabPageByTab(this.PointToClient(new System.Drawing.Point(e.X, e.Y)));
      if (tabPageByTab == null || !e.Data.GetDataPresent(typeof (TabPage)))
      {
        e.Effect = DragDropEffects.None;
      }
      else
      {
        e.Effect = DragDropEffects.Move;
        if (!reorder)
          return;
        TabPage data = (TabPage) e.Data.GetData(typeof (TabPage));
        int index1 = this.TabPages.IndexOf(data);
        int index2 = this.TabPages.IndexOf(tabPageByTab);
        if (index1 == index2)
          return;
        this.TabPages.RemoveAt(index1);
        this.TabPages.Insert(index2, data);
        this.SelectedTab = data;
      }
    }

    protected override void OnDragDrop(DragEventArgs e)
    {
      base.OnDragDrop(e);
      this.DragTab(e, true);
      this.downPoint = System.Drawing.Point.Empty;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.downPoint = e.Location;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.downPoint.IsEmpty || this.downPoint.Distance(e.Location) < 5)
        return;
      TabPage tabPageByTab = this.GetTabPageByTab(e.Location);
      if (tabPageByTab == null)
        return;
      int num = (int) this.DoDragDrop((object) tabPageByTab, DragDropEffects.Move);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.downPoint = System.Drawing.Point.Empty;
    }

    private TabPage GetTabPageByTab(System.Drawing.Point pt)
    {
      for (int index = 0; index < this.TabPages.Count; ++index)
      {
        if (this.GetTabRect(index).Contains(pt))
          return this.TabPages[index];
      }
      return (TabPage) null;
    }
  }
}

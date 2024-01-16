// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Controls.ListStyles
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Controls
{
  public static class ListStyles
  {
    private static Rectangle oldBounds;

    public static void SetOwnerDrawn(ListView lv)
    {
      lv.OwnerDraw = true;
      lv.DrawItem += new DrawListViewItemEventHandler(ListStyles.DrawItem);
      lv.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(ListStyles.DrawColumnHeader);
      lv.DrawSubItem += new DrawListViewSubItemEventHandler(ListStyles.DrawSubItem);
      lv.MouseMove += new MouseEventHandler(ListStyles.MouseMove);
    }

    private static void MouseMove(object sender, MouseEventArgs e)
    {
      ListView listView = sender as ListView;
      ListViewItem itemAt = listView.GetItemAt(e.X, e.Y);
      if (itemAt == null || !(itemAt.Bounds != ListStyles.oldBounds))
        return;
      listView.Invalidate(itemAt.Bounds);
      ListStyles.oldBounds = itemAt.Bounds;
    }

    private static void DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
      e.DrawDefault = false;
      TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter;
      switch (e.Header.TextAlign)
      {
        case HorizontalAlignment.Right:
          flags |= TextFormatFlags.Right;
          break;
        case HorizontalAlignment.Center:
          flags |= TextFormatFlags.HorizontalCenter;
          break;
      }
      using (e.Graphics.SaveState())
      {
        e.Graphics.SetClip(e.SubItem.Bounds, CombineMode.Intersect);
        e.DrawText(flags);
      }
    }

    private static void DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
    {
      e.DrawDefault = true;
    }

    private static void DrawItem(object sender, DrawListViewItemEventArgs e)
    {
      e.DrawDefault = false;
      e.DrawBackground();
      using (e.Graphics.SaveState())
      {
        e.Graphics.SetClip(e.Item.Bounds, CombineMode.Intersect);
        StyledRenderer.AlphaStyle alphaStyle = StyledRenderer.GetAlphaStyle(e.Item.Selected, false, e.Item.Focused);
        Color selectionColor = StyledRenderer.GetSelectionColor(e.Item.Focused);
        if (alphaStyle == StyledRenderer.AlphaStyle.None)
          return;
        e.Graphics.DrawStyledRectangle(e.Bounds.Pad(0, 0, 1, 1), alphaStyle, selectionColor);
      }
    }
  }
}

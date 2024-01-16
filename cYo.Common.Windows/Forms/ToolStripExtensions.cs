// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ToolStripExtensions
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public static class ToolStripExtensions
  {
    public static ToolStripMenuItem Clone(this ToolStripMenuItem item)
    {
      ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(item.Text, item.Image, (EventHandler) null, item.ShortcutKeys);
      toolStripMenuItem.Name = item.Name;
      return toolStripMenuItem;
    }

    public static void FixSeparators(this ToolStripItemCollection items)
    {
      ToolStripItem toolStripItem1 = (ToolStripItem) null;
      foreach (ToolStripItem toolStripItem2 in (ArrangedElementCollection) items)
      {
        if (toolStripItem2 is ToolStripSeparator)
        {
          toolStripItem2.Visible = toolStripItem1 == null;
          toolStripItem1 = toolStripItem2;
        }
        else if (toolStripItem2.Available)
          toolStripItem1 = (ToolStripItem) null;
      }
      if (toolStripItem1 == null)
        return;
      toolStripItem1.Visible = false;
    }

    public static void FixSeparators(this ContextMenuStrip contextMenu)
    {
      contextMenu.Items.FixSeparators();
    }
  }
}

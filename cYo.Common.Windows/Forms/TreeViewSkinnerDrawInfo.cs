// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TreeViewSkinnerDrawInfo
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TreeViewSkinnerDrawInfo
  {
    public TreeViewSkinnerDrawInfo(
      Graphics graphics,
      Rectangle itemBounds,
      Rectangle labelBounds,
      TreeNode node,
      TreeNodeStates state,
      Font font)
    {
      this.Graphics = graphics;
      this.ItemBounds = itemBounds;
      this.LabelBounds = labelBounds;
      this.State = state;
      this.Node = node;
      this.Font = font;
    }

    public Graphics Graphics { get; set; }

    public Font Font { get; set; }

    public Rectangle ItemBounds { get; set; }

    public Rectangle LabelBounds { get; set; }

    public TreeNodeStates State { get; set; }

    public TreeNode Node { get; set; }

    public bool HasState(TreeNodeStates treeNodeStates) => (this.State & treeNodeStates) != 0;
  }
}

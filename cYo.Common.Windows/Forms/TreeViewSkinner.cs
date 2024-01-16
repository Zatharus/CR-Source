// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TreeViewSkinner
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using cYo.Common.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TreeViewSkinner : Component
  {
    private TreeNode dropNode;
    private bool separatorDropNodeStyle;
    private TreeView treeView;

    public TreeViewSkinner(TreeView tv) => this.TreeView = tv;

    public TreeViewSkinner()
      : this((TreeView) null)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.TreeView = (TreeView) null;
      base.Dispose(disposing);
    }

    public TreeNode DropNode
    {
      get => this.dropNode;
      set
      {
        if (this.dropNode == value)
          return;
        TreeViewSkinner.InvalidateNode(this.dropNode);
        this.dropNode = value;
        TreeViewSkinner.InvalidateNode(this.dropNode);
      }
    }

    public bool SeparatorDropNodeStyle
    {
      get => this.separatorDropNodeStyle;
      set
      {
        if (this.separatorDropNodeStyle == value)
          return;
        this.separatorDropNodeStyle = value;
        TreeViewSkinner.InvalidateNode(this.dropNode);
      }
    }

    public TreeView TreeView
    {
      get => this.treeView;
      set
      {
        if (this.treeView == value)
          return;
        if (this.treeView != null)
        {
          this.treeView.DrawMode = TreeViewDrawMode.Normal;
          this.treeView.DrawNode -= new DrawTreeNodeEventHandler(this.OnDrawNode);
          this.treeView.Disposed -= new EventHandler(this.TreeViewDisposed);
        }
        this.treeView = value;
        if (this.treeView == null)
          return;
        this.treeView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
        this.treeView.ItemHeight = this.treeView.Font.Height + FormUtility.ScaleDpiY(8);
        this.treeView.DrawNode += new DrawTreeNodeEventHandler(this.OnDrawNode);
        this.treeView.Disposed += new EventHandler(this.TreeViewDisposed);
      }
    }

    private void OnDrawNode(object sender, DrawTreeNodeEventArgs e)
    {
      try
      {
        if (e.Bounds.IsEmpty || e.Node.Bounds.IsEmpty)
          return;
        TreeNodeStates state = e.State;
        if (e.Node.Checked)
          state |= TreeNodeStates.Checked;
        Graphics graphics1 = e.Graphics;
        using (graphics1.SaveState())
        {
          Rectangle bounds1 = e.Bounds;
          Rectangle bounds2 = e.Node.Bounds;
          bounds2.Offset(-e.Bounds.X, -e.Bounds.Y);
          bounds1.Offset(-e.Bounds.X, -e.Bounds.Y);
          graphics1.IntersectClip(e.Bounds);
          Graphics graphics2 = graphics1;
          Rectangle bounds3 = e.Bounds;
          double x = (double) bounds3.X;
          bounds3 = e.Bounds;
          double y = (double) bounds3.Y;
          graphics2.TranslateTransform((float) x, (float) y);
          --bounds1.Width;
          --bounds1.Height;
          TreeViewSkinnerDrawInfo di = new TreeViewSkinnerDrawInfo(graphics1, bounds1, bounds2, e.Node, state, this.treeView.Font);
          di.Graphics.Clear(this.TreeView.BackColor);
          this.DrawNode(di);
        }
      }
      catch (Exception ex)
      {
        e.DrawDefault = true;
      }
    }

    protected virtual void DrawNodeBackground(TreeViewSkinnerDrawInfo di)
    {
    }

    protected virtual void DrawNodeContent(TreeViewSkinnerDrawInfo di)
    {
    }

    protected virtual void DrawNodeCheckBox(TreeViewSkinnerDrawInfo di)
    {
    }

    protected virtual void DrawNodeLabel(TreeViewSkinnerDrawInfo di)
    {
    }

    protected virtual void DrawNodeFrame(TreeViewSkinnerDrawInfo di)
    {
    }

    protected virtual void DrawNode(TreeViewSkinnerDrawInfo di)
    {
      if (di.ItemBounds.Width == 0 || di.ItemBounds.Height == 0)
        return;
      this.DrawNodeBackground(di);
      this.DrawNodeContent(di);
      if (!di.Node.IsEditing)
        this.DrawNodeLabel(di);
      this.DrawNodeFrame(di);
    }

    private void TreeViewDisposed(object sender, EventArgs e) => this.Dispose();

    public static void InvalidateNode(TreeNode node)
    {
      if (node == null)
        return;
      Color backColor = node.BackColor;
      node.BackColor = Color.Gainsboro;
      node.BackColor = backColor;
    }

    public Bitmap GetBitmap(TreeNode node)
    {
      Rectangle bounds = node.Bounds;
      Rectangle itemBounds;
      ref Rectangle local = ref itemBounds;
      Rectangle rectangle = this.treeView.ClientRectangle;
      int width = rectangle.Width;
      rectangle = node.Bounds;
      int height = rectangle.Height;
      local = new Rectangle(0, 0, width, height);
      bounds.Y = 0;
      Bitmap bitmap = new Bitmap(itemBounds.Width, itemBounds.Height);
      try
      {
        using (Graphics graphics = Graphics.FromImage((Image) bitmap))
        {
          TreeNodeStates state = (TreeNodeStates) 0;
          if (node.IsSelected)
            state |= TreeNodeStates.Selected;
          if (node.Checked)
            state |= TreeNodeStates.Checked;
          this.DrawNode(new TreeViewSkinnerDrawInfo(graphics, itemBounds, bounds, node, state, this.treeView.Font));
        }
      }
      catch
      {
      }
      return bitmap;
    }

    public IBitmapCursor GetDragCursor(TreeNode node, byte alpha, System.Drawing.Point cursorLocation)
    {
      IBitmapCursor dragCursor = BitmapCursor.Create(this.GetBitmap(node));
      if (dragCursor != null)
      {
        cursorLocation.Offset(0, -node.Bounds.Y);
        dragCursor.BitmapOwned = true;
        dragCursor.HotSpot = cursorLocation;
        if (dragCursor.Bitmap != null)
          dragCursor.Bitmap.ChangeAlpha(alpha);
      }
      return dragCursor;
    }
  }
}

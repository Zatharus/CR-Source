// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.NiceTreeSkin
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using cYo.Common.Windows.Properties;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class NiceTreeSkin : TreeViewSkinner
  {
    private readonly Bitmap down = Resources.SimpleArrowDown;
    private readonly Bitmap right = Resources.SimpleArrowRight;

    public NiceTreeSkin()
    {
    }

    public NiceTreeSkin(TreeView tv)
      : base(tv)
    {
    }

    protected override void DrawNodeBackground(TreeViewSkinnerDrawInfo di)
    {
      Color selectionColor = StyledRenderer.GetSelectionColor(this.TreeView.Focused);
      if (di.HasState(TreeNodeStates.Selected))
        di.Graphics.DrawStyledRectangle(di.ItemBounds, StyledRenderer.GetAlphaStyle(di.HasState(TreeNodeStates.Selected), di.HasState(TreeNodeStates.Hot), di.HasState(TreeNodeStates.Focused)), selectionColor);
      if (di.Node != this.DropNode)
        return;
      if (!this.SeparatorDropNodeStyle)
      {
        di.Graphics.DrawStyledRectangle(di.ItemBounds, StyledRenderer.AlphaStyle.SelectedHot, selectionColor);
      }
      else
      {
        Rectangle itemBounds = di.ItemBounds with
        {
          Height = 2
        };
        di.Graphics.FillRectangle(Brushes.Black, itemBounds);
      }
    }

    protected override void DrawNodeContent(TreeViewSkinnerDrawInfo di)
    {
      ImageList imageList = this.TreeView.ImageList;
      System.Drawing.Point point;
      ref System.Drawing.Point local = ref point;
      Rectangle rectangle1 = di.LabelBounds;
      int x1 = rectangle1.X;
      rectangle1 = di.ItemBounds;
      int y1 = rectangle1.Y;
      local = new System.Drawing.Point(x1, y1);
      if (imageList != null)
      {
        string key = di.HasState(TreeNodeStates.Selected) ? di.Node.SelectedImageKey : di.Node.ImageKey;
        int index = di.HasState(TreeNodeStates.Selected) ? di.Node.SelectedImageIndex : di.Node.ImageIndex;
        using (Image image = string.IsNullOrEmpty(key) ? imageList.Images[index] : imageList.Images[key])
        {
          if (image != null)
          {
            point.X -= imageList.ImageSize.Width - 2;
            this.DrawNodeIcon(di, image, new Rectangle(point.X, point.Y + (di.ItemBounds.Height - image.Height) / 2, image.Width, image.Height));
          }
        }
      }
      Rectangle rectangle2;
      if (this.TreeView.CheckBoxes)
      {
        rectangle2 = di.LabelBounds;
        int height = rectangle2.Height;
        int num = height - FormUtility.ScaleDpiY(6);
        point.X -= num + 6;
        Rectangle rectangle3 = new Rectangle(point.X, point.Y + (height - num) / 2, num, num);
        ButtonState state = ButtonState.Flat;
        if (di.HasState(TreeNodeStates.Checked))
          state |= ButtonState.Checked;
        if (di.HasState(TreeNodeStates.Grayed))
          state |= ButtonState.Inactive;
        ControlPaint.DrawCheckBox(di.Graphics, rectangle3, state);
      }
      if (di.Node.Nodes.Count == 0 || !this.TreeView.ShowPlusMinus)
        return;
      Image image1 = di.Node.IsExpanded ? (Image) this.down : (Image) this.right;
      System.Drawing.Size size = image1.Size.ScaleDpi();
      point.X -= size.Width - 1;
      Graphics graphics = di.Graphics;
      Image image2 = image1;
      int x2 = point.X;
      int y2 = point.Y;
      rectangle2 = di.ItemBounds;
      int num1 = (rectangle2.Height - size.Height) / 2;
      int y3 = y2 + num1;
      int width = size.Width;
      int height1 = size.Height;
      graphics.DrawImage(image2, x2, y3, width, height1);
    }

    protected virtual void DrawNodeIcon(TreeViewSkinnerDrawInfo di, Image image, Rectangle bounds)
    {
      di.Graphics.DrawImage(image, bounds);
    }

    protected override void DrawNodeLabel(TreeViewSkinnerDrawInfo di)
    {
      Color foreColor = di.HasState(TreeNodeStates.Grayed) ? SystemColors.GrayText : SystemColors.WindowText;
      di.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
      TextRenderer.DrawText((IDeviceContext) di.Graphics, di.Node.Text, di.HasState(TreeNodeStates.Selected) ? FC.Get(di.Font, FontStyle.Bold) : di.Font, di.LabelBounds, foreColor, TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsTranslateTransform);
      Graphics graphics = di.Graphics;
      string text = di.Node.Text;
      Font font = di.HasState(TreeNodeStates.Selected) ? FC.Get(di.Font, FontStyle.Bold) : di.Font;
      Rectangle labelBounds = di.LabelBounds;
      System.Drawing.Size size1 = labelBounds.Size;
      System.Drawing.Size size2 = TextRenderer.MeasureText((IDeviceContext) graphics, text, font, size1, TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsTranslateTransform);
      TreeViewSkinnerDrawInfo viewSkinnerDrawInfo = di;
      labelBounds = di.LabelBounds;
      int x = labelBounds.X;
      labelBounds = di.LabelBounds;
      int y = labelBounds.Y;
      int width = size2.Width;
      labelBounds = di.LabelBounds;
      int height = labelBounds.Height;
      Rectangle rectangle = new Rectangle(x, y, width, height);
      viewSkinnerDrawInfo.LabelBounds = rectangle;
    }

    protected override void DrawNodeFrame(TreeViewSkinnerDrawInfo di)
    {
    }
  }
}

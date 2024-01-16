// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.LibraryTreeSkin
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Sync;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class LibraryTreeSkin : NiceTreeSkin
  {
    private static readonly Image deviceIcon = (Image) Resources.DeviceSync.Scale(8, 8);

    public bool DisableDeviceIcon { get; set; }

    public Func<TreeNode, ComicListItem> GetNodeItem { get; set; }

    protected override void DrawNodeIcon(TreeViewSkinnerDrawInfo di, Image image, Rectangle bounds)
    {
      base.DrawNodeIcon(di, image, bounds);
      ComicListItem cli = this.GetNodeItem == null ? di.Node.Tag as ComicListItem : this.GetNodeItem(di.Node);
      if (this.DisableDeviceIcon || cli == null || Program.Settings.Devices.SelectMany<DeviceSyncSettings, DeviceSyncSettings.SharedList>((Func<DeviceSyncSettings, IEnumerable<DeviceSyncSettings.SharedList>>) (d => d.Lists.Where<DeviceSyncSettings.SharedList>((Func<DeviceSyncSettings.SharedList, bool>) (l => l.ListId == cli.Id)))).IsEmpty<DeviceSyncSettings.SharedList>())
        return;
      di.Graphics.DrawImage(LibraryTreeSkin.deviceIcon, bounds.X, bounds.Y);
    }

    protected override void DrawNodeLabel(TreeViewSkinnerDrawInfo di)
    {
      ComicListItem cli = this.GetNodeItem == null ? di.Node.Tag as ComicListItem : this.GetNodeItem(di.Node);
      base.DrawNodeLabel(di);
      if (cli == null || !cli.CacheEnabled || !Program.Settings.DisplayLibraryGauges || cli.BookCount <= 0 && cli.NewBookCount <= 0 && cli.UnreadBookCount <= 0)
        return;
      Graphics graphics = di.Graphics;
      Font font = FC.Get(di.Font, di.Font.Size * 0.75f);
      Rectangle rc;
      ref Rectangle local = ref rc;
      Rectangle rectangle = di.LabelBounds;
      int x = rectangle.Right + 4;
      rectangle = di.ItemBounds;
      int top = rectangle.Top;
      rectangle = di.ItemBounds;
      int right1 = rectangle.Right;
      rectangle = di.LabelBounds;
      int right2 = rectangle.Right;
      int width = right1 - right2 - 4;
      rectangle = di.ItemBounds;
      int height = rectangle.Height;
      local = new Rectangle(x, top, width, height);
      if ((Program.Settings.LibraryGaugesFormat.HasFlag((Enum) LibraryGauges.Numeric) ? this.DrawMarkers(graphics, rc, font, cli, onlyMeasure: true) : int.MaxValue) < rc.Width)
      {
        this.DrawMarkers(graphics, rc, font, cli);
      }
      else
      {
        int totalSize = Math.Min(rc.Width / 3, 6);
        if (totalSize <= 2)
          return;
        this.DrawMarkers(graphics, rc, font, cli, totalSize);
      }
    }

    public int DrawMarkers(
      Graphics gr,
      Rectangle rc,
      Font font,
      ComicListItem cli,
      int totalSize = 0,
      bool onlyMeasure = false)
    {
      int num1 = 0;
      bool flag1 = Program.Settings.LibraryGaugesFormat.HasFlag((Enum) LibraryGauges.Total) && cli.BookCount != 0;
      bool flag2 = Program.Settings.LibraryGaugesFormat.HasFlag((Enum) LibraryGauges.Unread) && cli.UnreadBookCount != 0;
      bool flag3 = Program.Settings.LibraryGaugesFormat.HasFlag((Enum) LibraryGauges.New) && cli.NewBookCount != 0;
      if (flag1)
      {
        int num2 = this.DrawNumberMarker(gr, cli.BookCount, Color.Green, Color.White, font, rc, !(flag2 & flag3), true, onlyMeasure, totalSize);
        rc.Width -= num2;
        num1 += num2;
      }
      if (flag2)
      {
        int unreadBookCount = cli.UnreadBookCount;
        if (!flag3)
          unreadBookCount += cli.NewBookCount;
        int num3 = this.DrawNumberMarker(gr, unreadBookCount, Color.Orange, Color.White, font, rc, !flag3, !flag1, onlyMeasure, totalSize);
        rc.Width -= num3;
        num1 += num3;
      }
      if (flag3)
      {
        int num4 = this.DrawNumberMarker(gr, cli.NewBookCount, Color.Red, Color.White, font, rc, true, !(flag2 & flag1), onlyMeasure, totalSize);
        num1 += num4;
      }
      return num1;
    }

    public int DrawNumberMarker(
      Graphics gr,
      int n,
      Color backColor,
      Color textColor,
      Font font,
      Rectangle bounds,
      bool roundLeft,
      bool roundRight,
      bool onlyMeasure,
      int fixedSize = 0)
    {
      if (n == 0)
        return 0;
      string str = n.ToString();
      int width = fixedSize == 0 ? (int) gr.MeasureString(str, font, 200).Width + 4 : fixedSize;
      Rectangle rectangle = new Rectangle(bounds.Right - width, bounds.Y + 1, width, bounds.Height - 2);
      if (!onlyMeasure)
      {
        int num1 = roundLeft ? 1 : 0;
        int num2 = roundRight ? 1 : 0;
        using (Brush brush = (Brush) new LinearGradientBrush(rectangle, backColor.Transparent(192), backColor.Transparent(128), 270f))
        {
          using (GraphicsPath path = rectangle.ConvertToPath(num1, num1, num2, num2, num2 * 4, num2 * 4, num1, num1 * 2))
            gr.FillPath(brush, path);
        }
        if (fixedSize == 0)
        {
          using (Brush brush = (Brush) new SolidBrush(textColor))
          {
            using (StringFormat format = new StringFormat()
            {
              Alignment = StringAlignment.Center,
              LineAlignment = StringAlignment.Center
            })
              gr.DrawString(str, font, brush, (RectangleF) rectangle, format);
          }
        }
      }
      return rectangle.Width + 1;
    }
  }
}

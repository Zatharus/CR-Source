// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.GroupHeaderInformation
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class GroupHeaderInformation
  {
    public GroupHeaderInformation(string caption, List<IViewableItem> items, bool collapsed = false)
    {
      this.Caption = caption;
      this.Items = items;
      this.Collapsed = collapsed;
      this.Bounds = Rectangle.Empty;
      this.ItemCount = this.Items.Count;
    }

    public string Caption { get; set; }

    public Rectangle Bounds { get; set; }

    public bool Collapsed { get; set; }

    public List<IViewableItem> Items { get; private set; }

    public int ItemCount { get; set; }

    public Rectangle ArrowBounds { get; set; }

    public Rectangle TextBounds { get; set; }

    public Rectangle ExpandedColumnBounds { get; set; }
  }
}

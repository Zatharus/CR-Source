// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemSizeInformation
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ItemSizeInformation
  {
    private Rectangle bounds;

    public Graphics Graphics { get; set; }

    public Rectangle Bounds
    {
      get => this.bounds;
      set => this.bounds = value;
    }

    public int Width
    {
      get => this.bounds.Width;
      set => this.bounds.Width = value;
    }

    public int Height
    {
      get => this.bounds.Height;
      set => this.bounds.Height = value;
    }

    public System.Drawing.Size Size
    {
      get => this.bounds.Size;
      set => this.bounds.Size = value;
    }

    public ItemViewMode DisplayType { get; set; }

    public int Item { get; set; }

    public int GroupItem { get; set; }

    public int SubItem { get; set; }

    public IColumn Header { get; set; }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.ItemSizeInfo
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class ItemSizeInfo
  {
    public ItemSizeInfo(int min, int max, int value)
    {
      this.Minimum = min;
      this.Maximum = max;
      this.Value = value;
    }

    public int Minimum { get; set; }

    public int Maximum { get; set; }

    public int Value { get; set; }
  }
}

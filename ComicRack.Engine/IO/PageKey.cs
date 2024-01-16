// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.PageKey
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  [Serializable]
  public class PageKey : ImageKey
  {
    private BitmapAdjustment adjustment = BitmapAdjustment.Empty;

    public PageKey(
      object source,
      string location,
      long size,
      DateTime modified,
      int index,
      ImageRotation rotation,
      BitmapAdjustment adjustment)
      : base(source, location, size, modified, index, rotation)
    {
      this.adjustment = adjustment;
    }

    public PageKey(ImageKey key)
      : base(key)
    {
    }

    public BitmapAdjustment Adjustment => this.adjustment;

    protected override int CreateHashCode()
    {
      return base.CreateHashCode() ^ this.adjustment.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      PageKey pageKey = obj as PageKey;
      return base.Equals(obj) && pageKey != null && pageKey.adjustment == this.adjustment;
    }
  }
}

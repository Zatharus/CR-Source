// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ImagePartInfo
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public struct ImagePartInfo
  {
    private readonly int part;
    private readonly Point offset;
    public static readonly ImagePartInfo Empty;

    public ImagePartInfo(int part, int x, int y)
    {
      this.part = part;
      this.offset = new Point(x, y);
    }

    public ImagePartInfo(int part, Point offset)
    {
      this.part = part;
      this.offset = offset;
    }

    public ImagePartInfo(int part)
      : this(part, Point.Empty)
    {
    }

    public int Part => this.part;

    public Point Offset => this.offset;

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      ImagePartInfo imagePartInfo = (ImagePartInfo) obj;
      return this.part == imagePartInfo.part && this.offset == imagePartInfo.offset;
    }

    public override int GetHashCode() => this.part.GetHashCode();

    public override string ToString()
    {
      return string.Format("{0}, {1}", (object) this.part, (object) this.Offset);
    }

    public static bool operator ==(ImagePartInfo a, ImagePartInfo b) => a.Equals((object) b);

    public static bool operator !=(ImagePartInfo a, ImagePartInfo b) => !(a == b);
  }
}

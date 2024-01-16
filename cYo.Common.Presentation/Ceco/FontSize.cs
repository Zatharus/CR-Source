// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.FontSize
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public struct FontSize
  {
    public int Size;
    public bool Relative;
    public static readonly FontSize Empty = new FontSize(0, true);

    public FontSize(int size, bool relative)
    {
      this.Size = size;
      this.Relative = relative;
    }

    public override bool Equals(object obj)
    {
      return obj is FontSize fontSize && fontSize.Relative == this.Relative && fontSize.Size == this.Size;
    }

    public override int GetHashCode() => this.Size.GetHashCode() ^ this.Relative.GetHashCode();

    public static bool operator ==(FontSize a, FontSize b) => object.Equals((object) a, (object) b);

    public static bool operator !=(FontSize a, FontSize b) => !(a == b);
  }
}

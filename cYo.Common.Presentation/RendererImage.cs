// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.RendererImage
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation
{
  public abstract class RendererImage
  {
    public abstract Bitmap Bitmap { get; }

    public virtual bool IsValid => this.Bitmap != null;

    public virtual Size Size => this.Bitmap.Size;

    public int Width => this.Size.Width;

    public int Height => this.Size.Height;

    public override bool Equals(object obj)
    {
      return obj != null && !(obj.GetType() != this.GetType()) && ((RendererImage) obj).Bitmap == this.Bitmap;
    }

    public override int GetHashCode() => this.Bitmap.GetHashCode();

    public static implicit operator Bitmap(RendererImage image) => image.Bitmap;

    public static implicit operator RendererImage(Bitmap image)
    {
      return (RendererImage) new RendererGdiImage(image);
    }
  }
}

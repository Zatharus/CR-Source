// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing3D.ColorizeTexture
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing3D
{
  public class ColorizeTexture : ITexture
  {
    private Color color;
    private readonly ITexture texture;

    public ColorizeTexture(ITexture texture, Color color)
    {
      this.texture = texture;
      this.color = color;
    }

    public Size Size => this.texture.Size;

    public Color GetColor(int x, int y)
    {
      Color color = this.texture.GetColor(x, y);
      return Color.FromArgb((int) this.color.A * (int) color.A / 256, (int) this.color.R * (int) color.R / 256, (int) this.color.G * (int) color.G / 256, (int) this.color.B * (int) color.B / 256);
    }

    public void SetColor(int x, int y, Color color) => this.texture.SetColor(x, y, color);
  }
}

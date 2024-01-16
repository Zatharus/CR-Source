// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing3D.ColorTexture
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing3D
{
  public class ColorTexture : ITexture
  {
    private Color color = Color.White;

    public ColorTexture()
      : this(Color.White)
    {
    }

    public ColorTexture(Color color) => this.color = color;

    public Size Size => new Size(1, 1);

    public Color GetColor(int x, int y) => this.color;

    public void SetColor(int x, int y, Color color) => this.color = color;
  }
}

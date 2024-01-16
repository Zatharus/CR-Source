// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ColorF
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Mathematics;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing
{
  public struct ColorF
  {
    public float A;
    public float R;
    public float G;
    public float B;

    public ColorF(float a, float r, float g, float b)
    {
      this.A = a;
      this.R = r;
      this.G = g;
      this.B = b;
    }

    public ColorF(float g)
      : this(1f, g, g, g)
    {
    }

    public ColorF(Color color)
    {
      this.A = (float) color.A / (float) byte.MaxValue;
      this.R = (float) color.R / (float) byte.MaxValue;
      this.G = (float) color.G / (float) byte.MaxValue;
      this.B = (float) color.B / (float) byte.MaxValue;
    }

    public Color ToColor()
    {
      return Color.FromArgb((int) (byte) ((double) this.A * (double) byte.MaxValue), (int) (byte) ((double) this.R * (double) byte.MaxValue), (int) (byte) ((double) this.G * (double) byte.MaxValue), (int) (byte) ((double) this.B * (double) byte.MaxValue));
    }

    public void Clamp()
    {
      this.A = ColorF.ClampOne(this.A);
      this.R = ColorF.ClampOne(this.R);
      this.G = ColorF.ClampOne(this.G);
      this.B = ColorF.ClampOne(this.B);
    }

    public static float ClampOne(float f) => f.Clamp(0.0f, 1f);

    public static ColorF Multiply(ColorF a, ColorF b)
    {
      return new ColorF(a.A * b.A, a.R * b.R, a.G * b.G, a.B * b.B);
    }

    public static ColorF Multiply(ColorF a, float b) => new ColorF(a.A, a.R * b, a.G * b, a.B * b);

    public static ColorF Add(ColorF a, ColorF b)
    {
      return new ColorF(a.A + b.A, a.R + b.R, a.G + b.G, a.B + b.B);
    }

    public static ColorF operator *(ColorF a, ColorF b) => ColorF.Multiply(a, b);

    public static ColorF operator *(ColorF a, float b) => ColorF.Multiply(a, b);

    public static ColorF operator +(ColorF a, ColorF b) => ColorF.Add(a, b);

    public static implicit operator Color(ColorF color) => color.ToColor();

    public static implicit operator ColorF(Color color) => new ColorF(color);
  }
}

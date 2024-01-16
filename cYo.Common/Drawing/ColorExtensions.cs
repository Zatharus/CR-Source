// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ColorExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class ColorExtensions
  {
    public static Color GetAverage(this IEnumerable<Color> colors)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      foreach (Color color in colors)
      {
        num2 += (int) color.R;
        num3 += (int) color.G;
        num4 += (int) color.B;
        ++num1;
      }
      if (num1 == 0)
        throw new ArgumentException("must be no empty list", nameof (colors));
      return Color.FromArgb(num2 / num1, num3 / num1, num4 / num1);
    }

    public static string IsNamedColor(string color)
    {
      Color color1 = Color.FromName(color);
      return !string.IsNullOrEmpty(color1.Name) ? color1.Name : throw new ArgumentException("Only named colors allowed");
    }

    public static int ToRgb(this Color color) => color.ToArgb() & 16777215;

    public static bool IsBlackOrWhite(this Color color)
    {
      int rgb = color.ToRgb();
      return rgb == Color.White.ToRgb() || rgb == Color.Black.ToRgb();
    }

    public static Color Transparent(this Color color, int alpha) => Color.FromArgb(alpha, color);

    public static Color Brightness(this Color color, float f)
    {
      return Color.FromArgb(((int) ((double) color.R * (double) f)).Clamp(0, (int) byte.MaxValue), ((int) ((double) color.G * (double) f)).Clamp(0, (int) byte.MaxValue), ((int) ((double) color.B * (double) f)).Clamp(0, (int) byte.MaxValue));
    }
  }
}

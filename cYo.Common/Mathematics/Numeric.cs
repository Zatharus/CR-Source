// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Numeric
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#nullable disable
namespace cYo.Common.Mathematics
{
  public static class Numeric
  {
    public static float PI = 3.14159274f;
    public static float FloatEpsilon = float.MinValue;
    public static double DoubleEpsilon = double.MinValue;

    public static float Sqrt(float value) => (float) Math.Sqrt((double) value);

    public static float InvSqrt(float x) => 1f / Numeric.Sqrt(x);

    public static float Abs(float value) => Math.Abs(value);

    public static float Log(double a) => (float) Math.Log(a);

    public static float Log(double a, double newBase) => (float) Math.Log(a, newBase);

    public static float Log10(double a) => (float) Math.Log10(a);

    public static float Exp(double power) => (float) Math.Exp(power);

    public static float Pow(double a, double power) => (float) Math.Pow(a, power);

    public static float Cos(float angle) => (float) Math.Cos((double) angle);

    public static float Acos(float number) => (float) Math.Acos((double) number);

    public static float Sin(float angle) => (float) Math.Sin((double) angle);

    public static float Cot(float angle) => Numeric.Cos(angle) / Numeric.Sin(angle);

    public static float DegToRad(float angle) => angle * (Numeric.PI / 180f);

    public static float RadToDeg(float angle) => angle * (180f / Numeric.PI);

    public static double DegToRad(double angle) => angle * (Math.PI / 180.0);

    public static double RadToDeg(double angle) => angle * (180.0 / Math.PI);

    public static int Clamp(this int value, int min, int max, int minValue, int maxValue = 2147483647)
    {
      if (maxValue == int.MaxValue)
        maxValue = max;
      if (value < min)
        return minValue;
      return value > max ? maxValue : value;
    }

    public static int Clamp(this int value, int min, int max)
    {
      return Math.Max(Math.Min(value, max), min);
    }

    public static float Clamp(this float value, float min, float max)
    {
      return Math.Max(Math.Min(value, max), min);
    }

    public static double Clamp(this double value, double min, double max)
    {
      return Math.Max(Math.Min(value, max), min);
    }

    public static Size Clamp(this Size value, Size min, Size max)
    {
      return new Size(value.Width.Clamp(min.Width, max.Width), value.Height.Clamp(max.Height, max.Width));
    }

    public static bool CompareTo(this float f, float t, float limit)
    {
      return (double) Math.Abs(f - t) < (double) limit;
    }

    public static bool CompareTo(this int f, int t, int limit) => Math.Abs(f - t) < limit;

    public static bool CompareTo(this Size f, Size t, int limit)
    {
      return f.Width.CompareTo(t.Width, limit) && f.Height.CompareTo(t.Height, limit);
    }

    public static bool Equals(float a, float b)
    {
      return (double) a > (double) b - (double) Numeric.FloatEpsilon && (double) a < (double) b + (double) Numeric.FloatEpsilon;
    }

    public static bool Equals(double a, double b)
    {
      return a > b - Numeric.DoubleEpsilon && a < b + Numeric.DoubleEpsilon;
    }

    public static int Rollover(int n, int count, int add)
    {
      add = add.Clamp(-count + 1, count - 1);
      n += add;
      if (n < 0)
        n = count + n;
      n %= count;
      return n;
    }

    public static int Select(int n, int[] values, bool wrap)
    {
      int length = values.Length;
      if (n > values[length - 1])
        return wrap ? values[0] : values[length - 1];
      if (n < values[0])
        return wrap ? values[length - 1] : values[0];
      foreach (int num in values)
      {
        if (n < num)
          return num;
      }
      return values[0];
    }

    public static float Select(float n, float[] values, bool wrap)
    {
      int length = values.Length;
      if ((double) n > (double) values[length - 1])
        return wrap ? values[0] : values[length - 1];
      if ((double) n < (double) values[0])
        return wrap ? values[length - 1] : values[0];
      foreach (float num in values)
      {
        if ((double) n < (double) num)
          return num;
      }
      return values[0];
    }

    public static bool InRange(int n, int min, int count) => n >= min && n <= min + count;

    public static float Min(params float[] n) => ((IEnumerable<float>) n).Min();

    public static float Max(params float[] n) => ((IEnumerable<float>) n).Max();

    public static int BinaryHash(params bool[] flags)
    {
      int num = 0;
      foreach (bool flag in ((IEnumerable<bool>) flags).Reverse<bool>())
      {
        num <<= 1;
        num += flag ? 1 : 0;
      }
      return num;
    }

    public static int HighestBit(int n)
    {
      int num = -1;
      for (; n != 0; n >>= 1)
        ++num;
      return num;
    }
  }
}

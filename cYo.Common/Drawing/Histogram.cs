// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.Histogram
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing
{
  public class Histogram
  {
    private readonly int size;
    private readonly Color white = Color.Empty;
    private readonly int[] reds;
    private readonly int[] greens;
    private readonly int[] blues;
    private readonly int[] grays;
    private readonly float[] redsNormalized;
    private readonly float[] greensNormalized;
    private readonly float[] bluesNormalized;
    private readonly float[] graysNormalized;
    private const float defaultThreshold = 0.005f;
    private const float range = 0.25f;
    private static readonly Histogram empty = new Histogram(new int[1], new int[1], new int[1], new int[1], 1);

    public Histogram(int[] reds, int[] greens, int[] blues, int[] grays, int pixelCount)
    {
      this.size = reds.Length;
      if (greens.Length != this.size || blues.Length != this.size || grays.Length != this.size)
        throw new ArgumentException("Arrays must have all the same length");
      this.reds = reds;
      this.greens = greens;
      this.blues = blues;
      this.grays = grays;
      this.redsNormalized = Histogram.Normalize((IList<int>) reds, (float) pixelCount);
      this.greensNormalized = Histogram.Normalize((IList<int>) greens, (float) pixelCount);
      this.bluesNormalized = Histogram.Normalize((IList<int>) blues, (float) pixelCount);
      this.graysNormalized = Histogram.Normalize((IList<int>) grays, (float) pixelCount);
    }

    public int Size => this.size;

    public Color White => this.white;

    public int[] Reds => this.reds;

    public int[] Greens => this.greens;

    public int[] Blues => this.blues;

    public int[] Grays => this.grays;

    public float[] RedsNormalized => this.redsNormalized;

    public float[] GreensNormalized => this.greensNormalized;

    public float[] BluesNormalized => this.bluesNormalized;

    public float[] GraysNormalized => this.graysNormalized;

    public float GetBlackPointNormalized(float threshold = 0.005f)
    {
      return Math.Min(Histogram.FindLowThreshold((IList<float>) this.graysNormalized, threshold), 0.25f);
    }

    public float GetWhitePointNormalized(float threshold = 0.005f)
    {
      return Math.Max(Histogram.FindTopThreshold((IList<float>) this.graysNormalized, threshold), 0.75f);
    }

    private static float FindLowThreshold(IList<float> array, float threshold)
    {
      float num = 0.0f;
      for (int index = 0; index < array.Count; ++index)
      {
        num += array[index];
        if ((double) num >= (double) threshold)
          return (float) index / (float) array.Count;
      }
      return 0.0f;
    }

    private static float FindTopThreshold(IList<float> array, float threshold)
    {
      float num = 0.0f;
      for (int index = array.Count - 1; index >= 0; --index)
      {
        num += array[index];
        if ((double) num >= (double) threshold)
          return (float) index / (float) array.Count;
      }
      return 1f;
    }

    private static float[] Normalize(IList<int> array, float factor)
    {
      float[] numArray = new float[array.Count];
      for (int index = 0; index < array.Count; ++index)
        numArray[index] = (float) array[index] / factor;
      return numArray;
    }

    private static float[] Normalize(IList<float> array, float factor)
    {
      float[] numArray = new float[array.Count];
      for (int index = 0; index < array.Count; ++index)
        numArray[index] = array[index] / factor;
      return numArray;
    }

    public static Histogram Empty => Histogram.empty;
  }
}

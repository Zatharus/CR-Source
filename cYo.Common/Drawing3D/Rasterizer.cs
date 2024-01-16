// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing3D.Rasterizer
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using System;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing3D
{
  public static class Rasterizer
  {
    public static void RasterizeLine(IFrameBuffer fb, Vertex v1, Vertex v2)
    {
      float x1 = v1.X;
      float y1 = v1.Y;
      float x2 = v2.X;
      float y2 = v2.Y;
      float a1 = v1.A;
      float a2 = v2.A;
      float r1 = v1.R;
      float r2 = v2.R;
      float g1 = v1.G;
      float g2 = v2.G;
      float b1 = v1.B;
      float b2 = v2.B;
      float z1 = v1.Z;
      float z2 = v2.Z;
      float num1 = x2 - x1;
      float num2 = (y2 - y1) / num1;
      bool flag = (double) Math.Abs(num2) > 1.0;
      if (flag)
      {
        CloneUtility.Swap<float>(ref x1, ref y1);
        CloneUtility.Swap<float>(ref x2, ref y2);
        num1 = x2 - x1;
        num2 = (y2 - y1) / num1;
      }
      if ((double) num1 < 0.0)
      {
        CloneUtility.Swap<float>(ref x1, ref x2);
        CloneUtility.Swap<float>(ref y1, ref y2);
        CloneUtility.Swap<float>(ref a1, ref a2);
        CloneUtility.Swap<float>(ref r1, ref r2);
        CloneUtility.Swap<float>(ref g1, ref g2);
        CloneUtility.Swap<float>(ref b1, ref b2);
        CloneUtility.Swap<float>(ref z1, ref z2);
      }
      float num3 = (a2 - a1) / num1;
      float num4 = (r2 - r1) / num1;
      float num5 = (g2 - g1) / num1;
      float num6 = (b2 - b1) / num1;
      float num7 = (z2 - z1) / num1;
      for (float a3 = x1; (double) a3 <= (double) x2; ++a3)
      {
        Point pt = flag ? new Point((int) Math.Round((double) y1), (int) Math.Round((double) a3)) : new Point((int) Math.Round((double) a3), (int) Math.Round((double) y1));
        fb.SetColor(pt, (Color) new ColorF(a1, r1, g1, b1));
        y1 += num2;
        a1 += num3;
        r1 += num4;
        g1 += num5;
        b1 += num6;
        z1 += num7;
      }
    }

    public static void RasterizeTriangle(
      IFrameBuffer fb,
      Vertex v1,
      Vertex v2,
      Vertex v3,
      ITexture texture)
    {
      Size size = fb.Size;
      int yBase = (int) Numeric.Min(v1.Y, v2.Y, v3.Y).Clamp(0.0f, (float) (size.Height - 1));
      int num1 = (int) Numeric.Max(v1.Y, v2.Y, v3.Y).Clamp(0.0f, (float) (size.Height - 1)) - yBase;
      Rasterizer.Span[] spans = new Rasterizer.Span[num1 + 1];
      for (int index = 0; index < spans.Length; ++index)
      {
        spans[index].X1 = int.MaxValue;
        spans[index].X2 = int.MinValue;
      }
      Rasterizer.ScanEdge(v1, v2, spans, yBase, size.Height);
      Rasterizer.ScanEdge(v2, v3, spans, yBase, size.Height);
      Rasterizer.ScanEdge(v3, v1, spans, yBase, size.Height);
      for (int index = 0; index <= num1; ++index)
      {
        int num2 = spans[index].X1;
        int num3 = spans[index].X2;
        if (num2 < size.Width && num3 >= 0)
        {
          float z1 = spans[index].Z1;
          float z2 = spans[index].Z2;
          float u1 = spans[index].U1;
          float u2 = spans[index].U2;
          float v1_1 = spans[index].V1;
          float v2_1 = spans[index].V2;
          float r1 = spans[index].R1;
          float r2 = spans[index].R2;
          float g1 = spans[index].G1;
          float g2 = spans[index].G2;
          float b1 = spans[index].B1;
          float b2 = spans[index].B2;
          float a1 = spans[index].A1;
          float a2 = spans[index].A2;
          if (num2 < 0)
          {
            float num4 = (float) -num2 / (float) (num3 - num2);
            z1 += (z2 - z1) * num4;
            u1 += (u2 - u1) * num4;
            v1_1 += (v2_1 - v1_1) * num4;
            r1 += (r2 - r1) * num4;
            g1 += (g2 - g1) * num4;
            b1 += (b2 - b1) * num4;
            a1 += (a2 - a1) * num4;
            num2 = 0;
          }
          float num5 = 1f / (float) (num3 - num2);
          float num6 = (u2 - u1) * num5;
          float num7 = (v2_1 - v1_1) * num5;
          float num8 = (z2 - z1) * num5;
          float num9 = (r2 - r1) * num5;
          float num10 = (g2 - g1) * num5;
          float num11 = (b2 - b1) * num5;
          float num12 = (a2 - a1) * num5;
          if (num3 >= size.Width)
            num3 = size.Width - 1;
          for (int x = num2; x <= num3; ++x)
          {
            ColorF colorF = new ColorF(a1, r1, g1, b1);
            if (texture != null)
              colorF *= (ColorF) texture.GetColor((int) ((double) u1 / (double) z1), (int) ((double) v1_1 / (double) z1));
            fb.SetColor(x, index + yBase, (Color) colorF);
            u1 += num6;
            v1_1 += num7;
            z1 += num8;
            r1 += num9;
            g1 += num10;
            b1 += num11;
            a1 += num12;
          }
        }
      }
    }

    public static void RasterizeQuad(
      IFrameBuffer fb,
      Vertex v1,
      Vertex v2,
      Vertex v3,
      Vertex v4,
      ITexture texture)
    {
      Rasterizer.RasterizeTriangle(fb, v1, v2, v3, texture);
      Rasterizer.RasterizeTriangle(fb, v3, v4, v1, texture);
    }

    private static void ScanEdge(
      Vertex v1,
      Vertex v2,
      Rasterizer.Span[] spans,
      int yBase,
      int fbHeight)
    {
      if ((double) v1.Y >= (double) v2.Y && (double) v1.Y <= (double) v2.Y)
        return;
      Vertex a1 = new Vertex(v1);
      Vertex b1 = new Vertex(v2);
      if ((double) v1.Y > (double) v2.Y)
        CloneUtility.Swap<Vertex>(ref a1, ref b1);
      int num1 = (int) a1.Y;
      int num2 = (int) b1.Y;
      if (num1 >= fbHeight || num2 < 0)
        return;
      if (num2 >= fbHeight)
        num2 = fbHeight - 1;
      a1.U /= a1.Z;
      a1.V /= a1.Z;
      a1.Z = 1f / a1.Z;
      b1.U /= b1.Z;
      b1.V /= b1.Z;
      b1.Z = 1f / b1.Z;
      float num3 = b1.Y - a1.Y;
      float x = a1.X;
      float u = a1.U;
      float v = a1.V;
      float z = a1.Z;
      float r = a1.R;
      float g = a1.G;
      float b2 = a1.B;
      float a2 = a1.A;
      float num4 = (b1.X - a1.X) / num3;
      float num5 = (b1.Z - a1.Z) / num3;
      float num6 = (b1.V - a1.V) / num3;
      float num7 = (b1.U - a1.U) / num3;
      float num8 = (b1.R - r) / num3;
      float num9 = (b1.G - g) / num3;
      float num10 = (b1.B - b2) / num3;
      float num11 = (b1.A - a2) / num3;
      if (num1 < 0)
      {
        x += num4 * (float) -num1;
        z += num5 * (float) -num1;
        u += num7 * (float) -num1;
        v += num6 * (float) -num1;
        r += num8 * (float) -num1;
        g += num9 * (float) -num1;
        b2 += num10 * (float) -num1;
        a2 += num11 * (float) -num1;
        num1 = 0;
      }
      int num12 = num1 - yBase;
      int num13 = num2 - yBase;
      for (int index = num12; index < num13; ++index)
      {
        int num14 = (int) x;
        if (num14 < spans[index].X1)
        {
          spans[index].X1 = num14;
          spans[index].Z1 = z;
          spans[index].U1 = u;
          spans[index].V1 = v;
          spans[index].R1 = r;
          spans[index].G1 = g;
          spans[index].B1 = b2;
          spans[index].A1 = a2;
        }
        if (num14 > spans[index].X2)
        {
          spans[index].X2 = num14;
          spans[index].Z2 = z;
          spans[index].U2 = u;
          spans[index].V2 = v;
          spans[index].R2 = r;
          spans[index].G2 = g;
          spans[index].B2 = b2;
          spans[index].A2 = a2;
        }
        x += num4;
        z += num5;
        u += num7;
        v += num6;
        r += num8;
        g += num9;
        b2 += num10;
        a2 += num11;
      }
    }

    private struct Span
    {
      public int X1;
      public int X2;
      public float Z1;
      public float Z2;
      public float U1;
      public float U2;
      public float V1;
      public float V2;
      public float R1;
      public float G1;
      public float B1;
      public float A1;
      public float R2;
      public float G2;
      public float B2;
      public float A2;
      public static readonly Rasterizer.Span Empty = new Rasterizer.Span(0, 0);

      public Span(
        int x1,
        int x2,
        float z1,
        float z2,
        float u1,
        float u2,
        float v1,
        float v2,
        float r1,
        float r2,
        float g1,
        float g2,
        float b1,
        float b2,
        float a1,
        float a2)
      {
        this.X1 = x1;
        this.X2 = x2;
        this.U1 = u1;
        this.U2 = u2;
        this.V1 = v1;
        this.V2 = v2;
        this.Z1 = z1;
        this.Z2 = z2;
        this.R1 = r1;
        this.G1 = g1;
        this.B1 = b1;
        this.A1 = a1;
        this.R2 = r2;
        this.G2 = g2;
        this.B2 = b2;
        this.A2 = a2;
      }

      public Span(int start, int end)
        : this(start, end, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f)
      {
      }
    }
  }
}

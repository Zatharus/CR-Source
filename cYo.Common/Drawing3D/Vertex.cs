// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing3D.Vertex
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Drawing;
using cYo.Common.Mathematics;

#nullable disable
namespace cYo.Common.Drawing3D
{
  public class Vertex
  {
    public float X;
    public float Y;
    public float Z;
    public float U;
    public float V;
    public float R;
    public float G;
    public float B;
    public float A;

    public Vertex(
      float x,
      float y,
      float z,
      float u,
      float v,
      float r,
      float g,
      float b,
      float a)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
      this.U = u;
      this.V = v;
      this.R = r;
      this.G = g;
      this.B = b;
      this.A = a;
    }

    public Vertex(float x, float y, float z, ColorF color)
      : this(x, y, z, 1f, 1f, color.R, color.G, color.B, color.A)
    {
    }

    public Vertex(float x, float y, float z, float u, float v)
      : this(x, y, z, u, v, 1f, 1f, 1f, 1f)
    {
    }

    public Vertex(float x, float y, float z)
      : this(x, y, z, 0.0f, 0.0f)
    {
    }

    public Vertex(Vertex v)
      : this(v.X, v.Y, v.Z, v.U, v.V, v.R, v.G, v.B, v.A)
    {
    }

    public ColorF ToColor() => new ColorF(this.A, this.R, this.G, this.B);

    public void SetColor(ColorF color)
    {
      this.A = color.A;
      this.R = color.R;
      this.G = color.G;
      this.B = color.B;
    }

    public static implicit operator Vector3(Vertex v) => new Vector3(v.X, v.Y, v.Z);

    public static bool operator ==(Vertex a, Vertex b) => a.Equals((object) b);

    public static bool operator !=(Vertex a, Vertex b) => !(a == b);

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      Vertex vertex = (Vertex) obj;
      return (double) this.X == (double) vertex.X && (double) this.Y == (double) vertex.Y && (double) this.Z == (double) vertex.Z && (double) this.U == (double) vertex.U && (double) this.V == (double) vertex.V && (double) this.A == (double) vertex.A && (double) this.R == (double) vertex.R && (double) this.G == (double) vertex.G && (double) this.B == (double) vertex.B;
    }

    public override int GetHashCode()
    {
      return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode() & this.U.GetHashCode() & this.V.GetHashCode();
    }
  }
}

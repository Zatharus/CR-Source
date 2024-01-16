// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Vector4
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable
namespace cYo.Common.Mathematics
{
  [TypeConverter(typeof (Vector4Converter))]
  public struct Vector4
  {
    private float x;
    private float y;
    private float z;
    private float w;

    public float X
    {
      get => this.x;
      set => this.x = value;
    }

    public float Y
    {
      get => this.y;
      set => this.y = value;
    }

    public float Z
    {
      get => this.z;
      set => this.z = value;
    }

    public float W
    {
      get => this.w;
      set => this.w = value;
    }

    [DebuggerHidden]
    public static Vector4 Zero => new Vector4();

    public Vector3 Vector3 => new Vector3(this.x, this.y, this.z);

    public Vector4(float x, float y, float z, float w)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.w = w;
    }

    public Vector4(Vector3 vec)
      : this(vec, 0.0f)
    {
    }

    public Vector4(Vector3 vec, float w)
    {
      this.x = vec.X;
      this.y = vec.Y;
      this.z = vec.Z;
      this.w = w;
    }

    public override string ToString()
    {
      return "X: " + (object) this.x + " Y: " + (object) this.y + " Z: " + (object) this.z + " W: " + (object) this.w;
    }

    public static Vector4 Unit(Vector4 a) => a / a.Length();

    public void Normalize() => this = this / this.Length();

    public float Length() => Numeric.Sqrt(this * this);

    public float LengthSquared() => this * this;

    public static Vector4 Lerp(Vector4 a, Vector4 b, float time)
    {
      return new Vector4(a.x + (b.x - a.x) * time, a.y + (b.y - a.y) * time, a.z + (b.z - a.z) * time, a.W + (b.W - a.W) * time);
    }

    public static Vector4 operator +(Vector4 a, Vector4 b)
    {
      return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public static float operator *(Vector4 a, Vector4 b)
    {
      return (float) ((double) a.x * (double) b.x + (double) a.y * (double) b.y + (double) a.z * (double) b.z + (double) a.w * (double) b.w);
    }

    public static Vector4 operator *(Vector4 vec, float scalar)
    {
      return new Vector4(vec.x * scalar, vec.y * scalar, vec.z * scalar, vec.w * scalar);
    }

    public static float Dot(Vector4 a, Vector4 b) => a * b;

    public static Vector4 operator *(Vector4 v, Matrix4 m)
    {
      return new Vector4((float) ((double) v.x * (double) m.A1 + (double) v.y * (double) m.B1 + (double) v.z * (double) m.C1 + (double) v.w * (double) m.D1), (float) ((double) v.x * (double) m.A2 + (double) v.y * (double) m.B2 + (double) v.z * (double) m.C2 + (double) v.w * (double) m.D2), (float) ((double) v.x * (double) m.A3 + (double) v.y * (double) m.B3 + (double) v.z * (double) m.C3 + (double) v.w * (double) m.D3), (float) ((double) v.x * (double) m.A4 + (double) v.y * (double) m.B4 + (double) v.z * (double) m.C4 + (double) v.w * (double) m.D4));
    }

    public static Vector4 operator /(Vector4 vec, float divisor)
    {
      return new Vector4(vec.x / divisor, vec.y / divisor, vec.z / divisor, vec.w / divisor);
    }

    public float this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return this.x;
          case 1:
            return this.y;
          case 2:
            return this.z;
          case 3:
            return this.w;
          default:
            throw new IndexOutOfRangeException("Invalid vector index!");
        }
      }
      set
      {
        switch (index)
        {
          case 0:
            this.x = value;
            break;
          case 1:
            this.y = value;
            break;
          case 2:
            this.z = value;
            break;
          case 3:
            this.w = value;
            break;
          default:
            throw new IndexOutOfRangeException("Invalid vector index!");
        }
      }
    }

    public static bool operator ==(Vector4 a, Vector4 b) => a.Equals((object) b);

    public static bool operator !=(Vector4 a, Vector4 b) => !(a == b);

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      Vector4 vector4 = (Vector4) obj;
      return (double) vector4.x == (double) this.x && (double) vector4.y == (double) this.y && (double) vector4.z == (double) this.z && (double) vector4.w == (double) this.w;
    }

    public override int GetHashCode()
    {
      return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode() ^ this.w.GetHashCode();
    }
  }
}

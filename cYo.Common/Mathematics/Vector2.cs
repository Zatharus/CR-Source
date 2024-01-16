// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Vector2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;
using System.Drawing;

#nullable disable
namespace cYo.Common.Mathematics
{
  [TypeConverter(typeof (Vector2Converter))]
  public struct Vector2
  {
    private float x;
    private float y;

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

    public static Vector2 Zero => new Vector2();

    public static Vector2 One => new Vector2(1f, 1f);

    public Vector2(float x, float y)
    {
      this.x = x;
      this.y = y;
    }

    public static Vector2 From(byte[] bytes) => Vector2.From(bytes, 0);

    public static unsafe Vector2 From(byte[] bytes, int offset)
    {
      fixed (byte* numPtr = &bytes[offset])
        return new Vector2(*(float*) numPtr, ((float*) numPtr)[1]);
    }

    public override string ToString() => "X: " + (object) this.x + " Y: " + (object) this.y;

    public static Vector2 Unit(Vector2 a) => a / a.Length();

    public static Vector2 Ortho(Vector2 a) => new Vector2(a.Y, -a.X);

    public void Normalize() => this = this / this.Length();

    public float Length() => Numeric.Sqrt(this * this);

    public float LengthSquared() => this * this;

    public static Vector2 FromArray(float[] vec) => new Vector2(vec[0], vec[1]);

    public static Vector2 Lerp(Vector2 a, Vector2 b, float time)
    {
      return new Vector2(a.x + (b.x - a.x) * time, a.y + (b.y - a.y) * time);
    }

    public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

    public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

    public static Vector2 operator -(Vector2 a) => new Vector2(-a.x, -a.y);

    public static Vector2 operator /(Vector2 vec, float divisor)
    {
      return new Vector2(vec.x / divisor, vec.y / divisor);
    }

    public static float operator *(Vector2 a, Vector2 b)
    {
      return (float) ((double) a.x * (double) b.x + (double) a.y * (double) b.y);
    }

    public static float Dot(Vector2 a, Vector2 b) => a * b;

    public static Vector2 operator *(Vector2 vec, float scalar)
    {
      return new Vector2(vec.x * scalar, vec.y * scalar);
    }

    public float this[int index]
    {
      get
      {
        if (index == 0)
          return this.x;
        if (index == 1)
          return this.y;
        throw new IndexOutOfRangeException("Invalid vector index!");
      }
      set
      {
        if (index != 0)
        {
          if (index != 1)
            throw new IndexOutOfRangeException("Invalid vector index!");
          this.y = value;
        }
        else
          this.x = value;
      }
    }

    public Vector3 ToVector3() => new Vector3(this);

    public static Vector2 Rotate(Vector2 vec, float angle)
    {
      float num1 = Numeric.Cos(angle);
      float num2 = Numeric.Sin(angle);
      return new Vector2((float) ((double) vec.x * (double) num1 + (double) vec.y * (double) num2), (float) (-(double) vec.x * (double) num2 + (double) vec.y * (double) num1));
    }

    public static Vector2 MultiplyElements(Vector2 a, Vector2 b)
    {
      return new Vector2(a.x * b.x, a.y * b.y);
    }

    public static Vector2 DivideElements(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

    public static bool operator ==(Vector2 a, Vector2 b)
    {
      return (double) a.x == (double) b.x && (double) a.y == (double) b.y;
    }

    public static bool operator !=(Vector2 a, Vector2 b)
    {
      return (double) a.x != (double) b.x || (double) a.y != (double) b.y;
    }

    public static Vector2 Max(Vector2 a, Vector2 b)
    {
      return new Vector2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
    }

    public static explicit operator Vector2(Size size)
    {
      return new Vector2((float) size.Width, (float) size.Height);
    }

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      Vector2 vector2 = (Vector2) obj;
      return (double) vector2.x == (double) this.x && (double) vector2.y == (double) this.y;
    }

    public override int GetHashCode() => this.x.GetHashCode() ^ this.y.GetHashCode();
  }
}

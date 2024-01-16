// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Vector3
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Common.Mathematics
{
  [TypeConverter(typeof (Vector3Converter))]
  public struct Vector3
  {
    private float x;
    private float y;
    private float z;

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

    public static Vector3 Zero => new Vector3();

    public static Vector3 LookAt => new Vector3(0.0f, 0.0f, 1f);

    public static Vector3 Up => new Vector3(0.0f, 1f, 0.0f);

    public static Vector3 Right => new Vector3(1f, 0.0f, 0.0f);

    public Vector2 Vector2 => new Vector2(this.x, this.y);

    public Vector4 Vector4 => new Vector4(this.x, this.y, this.z, 0.0f);

    public Vector3(float x, float y, float z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    public Vector3(Vector2 vec2)
    {
      this.x = vec2.X;
      this.y = vec2.Y;
      this.z = 0.0f;
    }

    public Vector3(Vector2 vec2, float z)
    {
      this.x = vec2.X;
      this.y = vec2.Y;
      this.z = z;
    }

    public static Vector3 From(byte[] bytes) => Vector3.From(bytes, 0);

    public static unsafe Vector3 From(byte[] bytes, int offset)
    {
      fixed (byte* numPtr = &bytes[offset])
        return new Vector3(*(float*) numPtr, ((float*) numPtr)[1], ((float*) numPtr)[2]);
    }

    public void AddSkinned(Vector3 sourceVec, ref Matrix4 joint, float weight)
    {
      this.x += ((float) ((double) sourceVec.x * (double) joint.A1 + (double) sourceVec.y * (double) joint.B1 + (double) sourceVec.z * (double) joint.C1) + joint.D1) * weight;
      this.y += ((float) ((double) sourceVec.x * (double) joint.A2 + (double) sourceVec.y * (double) joint.B2 + (double) sourceVec.z * (double) joint.C2) + joint.D2) * weight;
      this.z += ((float) ((double) sourceVec.x * (double) joint.A3 + (double) sourceVec.y * (double) joint.B3 + (double) sourceVec.z * (double) joint.C3) + joint.D3) * weight;
    }

    public override int GetHashCode()
    {
      return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      Vector3 vector3 = (Vector3) obj;
      return (double) vector3.x == (double) this.x && (double) vector3.y == (double) this.y && (double) vector3.z == (double) this.z;
    }

    public override string ToString()
    {
      return "X: " + (object) this.x + " Y: " + (object) this.y + " Z: " + (object) this.z;
    }

    public static Vector3 Unit(Vector3 a)
    {
      float num = 1f / a.Length();
      return a * num;
    }

    public void Normalize()
    {
      float num = 1f / this.Length();
      this.x *= num;
      this.y *= num;
      this.z *= num;
    }

    public float Length() => Numeric.Sqrt(this * this);

    public float LengthSquared() => this * this;

    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
      return new Vector3((float) ((double) a.y * (double) b.z - (double) a.z * (double) b.y), (float) ((double) a.z * (double) b.x - (double) a.x * (double) b.z), (float) ((double) a.x * (double) b.y - (double) a.y * (double) b.x));
    }

    public static Vector3 CrossUnit(Vector3 a, Vector3 b)
    {
      Vector3 vector3 = new Vector3((float) ((double) a.y * (double) b.z - (double) a.z * (double) b.y), (float) ((double) a.z * (double) b.x - (double) a.x * (double) b.z), (float) ((double) a.x * (double) b.y - (double) a.y * (double) b.x));
      float num = Numeric.InvSqrt((float) ((double) vector3.x * (double) vector3.x + (double) vector3.y * (double) vector3.y * (double) vector3.z * (double) vector3.z));
      vector3.x *= num;
      vector3.y *= num;
      vector3.z *= num;
      return vector3;
    }

    public static Vector3 FromArray(float[] vec) => new Vector3(vec[0], vec[1], vec[2]);

    public static Vector3 Lerp(Vector3 a, Vector3 b, float time)
    {
      return new Vector3(a.x + (b.x - a.x) * time, a.y + (b.y - a.y) * time, a.z + (b.z - a.z) * time);
    }

    public void SetZero()
    {
      this.x = 0.0f;
      this.y = 0.0f;
      this.z = 0.0f;
    }

    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
      return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public void Add(Vector3 a)
    {
      this.x += a.x;
      this.y += a.y;
      this.z += a.z;
    }

    public void AddWeighted(Vector3 a, float scalar)
    {
      this.x += a.x * scalar;
      this.y += a.y * scalar;
      this.z += a.z * scalar;
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
      return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3 operator -(Vector3 a) => new Vector3(-a.x, -a.y, -a.z);

    public static Vector3 operator /(Vector3 vec, float divisor)
    {
      return new Vector3(vec.x / divisor, vec.y / divisor, vec.z / divisor);
    }

    public static float operator *(Vector3 a, Vector3 b)
    {
      return (float) ((double) a.x * (double) b.x + (double) a.y * (double) b.y + (double) a.z * (double) b.z);
    }

    public static float Dot(Vector3 a, Vector3 b) => a * b;

    public static Vector3 operator *(Vector3 v, Matrix4 m)
    {
      return new Vector3((float) ((double) v.x * (double) m.A1 + (double) v.y * (double) m.B1 + (double) v.z * (double) m.C1) + m.D1, (float) ((double) v.x * (double) m.A2 + (double) v.y * (double) m.B2 + (double) v.z * (double) m.C2) + m.D2, (float) ((double) v.x * (double) m.A3 + (double) v.y * (double) m.B3 + (double) v.z * (double) m.C3) + m.D3);
    }

    public void Mul(ref Matrix4 m)
    {
      float x = this.x;
      float y = this.y;
      float z = this.z;
      this.x = (float) ((double) x * (double) m.A1 + (double) y * (double) m.B1 + (double) z * (double) m.C1) + m.D1;
      this.y = (float) ((double) x * (double) m.A2 + (double) y * (double) m.B2 + (double) z * (double) m.C2) + m.D2;
      this.z = (float) ((double) x * (double) m.A3 + (double) y * (double) m.B3 + (double) z * (double) m.C3) + m.D3;
    }

    public static Vector3 operator *(Vector3 v, Matrix3 m)
    {
      return new Vector3((float) ((double) v.x * (double) m.A1 + (double) v.y * (double) m.B1 + (double) v.z * (double) m.C1), (float) ((double) v.x * (double) m.A2 + (double) v.y * (double) m.B2 + (double) v.z * (double) m.C2), (float) ((double) v.x * (double) m.A3 + (double) v.y * (double) m.B3 + (double) v.z * (double) m.C3));
    }

    public static Vector3 Scale(Vector3 a, Vector3 b)
    {
      return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static bool AllLess(Vector3 a, Vector3 b)
    {
      return (double) a.x < (double) b.x && (double) a.y < (double) b.y && (double) a.z < (double) b.z;
    }

    public static bool AllLessOrEqual(Vector3 a, Vector3 b)
    {
      return (double) a.x <= (double) b.x && (double) a.y <= (double) b.y && (double) a.z <= (double) b.z;
    }

    public static bool OneLess(Vector3 a, Vector3 b)
    {
      return (double) a.x < (double) b.x || (double) a.y < (double) b.y || (double) a.z < (double) b.z;
    }

    public static bool OneLessOrEqual(Vector3 a, Vector3 b)
    {
      return (double) a.x <= (double) b.x || (double) a.y <= (double) b.y || (double) a.z <= (double) b.z;
    }

    public static Vector3 operator *(Vector3 vec, float scalar)
    {
      return new Vector3(vec.x * scalar, vec.y * scalar, vec.z * scalar);
    }

    public void Mul(float scalar)
    {
      this.x *= scalar;
      this.y *= scalar;
      this.z *= scalar;
    }

    public static Vector3 operator *(float scalar, Vector3 vec)
    {
      return new Vector3(vec.x * scalar, vec.y * scalar, vec.z * scalar);
    }

    public static bool operator ==(Vector3 vec, Vector3 vec2)
    {
      return (double) vec.x == (double) vec2.x && (double) vec.y == (double) vec2.y && (double) vec.z == (double) vec2.z;
    }

    public static bool operator !=(Vector3 vec, Vector3 vec2)
    {
      return (double) vec.x != (double) vec2.x || (double) vec.y != (double) vec2.y || (double) vec.z != (double) vec2.z;
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
          default:
            throw new IndexOutOfRangeException("Invalid vector index!");
        }
      }
    }
  }
}

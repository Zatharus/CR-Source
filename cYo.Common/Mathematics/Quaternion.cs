// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Quaternion
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

#nullable disable
namespace cYo.Common.Mathematics
{
  public struct Quaternion
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

    public static Quaternion Zero => new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

    public Matrix4 Matrix4
    {
      get
      {
        return Matrix4.Zero with
        {
          A1 = (float) (1.0 - 2.0 * ((double) this.y * (double) this.y + (double) this.z * (double) this.z)),
          A2 = (float) (2.0 * ((double) this.x * (double) this.y + (double) this.w * (double) this.z)),
          A3 = (float) (2.0 * ((double) this.x * (double) this.z - (double) this.w * (double) this.y)),
          A4 = 0.0f,
          B1 = (float) (2.0 * ((double) this.x * (double) this.y - (double) this.w * (double) this.z)),
          B2 = (float) (1.0 - 2.0 * ((double) this.x * (double) this.x + (double) this.z * (double) this.z)),
          B3 = (float) (2.0 * ((double) this.y * (double) this.z + (double) this.w * (double) this.x)),
          B4 = 0.0f,
          C1 = (float) (2.0 * ((double) this.x * (double) this.z + (double) this.w * (double) this.y)),
          C2 = (float) (2.0 * ((double) this.y * (double) this.z - (double) this.w * (double) this.x)),
          C3 = (float) (1.0 - 2.0 * ((double) this.x * (double) this.x + (double) this.y * (double) this.y)),
          C4 = 0.0f,
          D1 = 0.0f,
          D2 = 0.0f,
          D3 = 0.0f,
          D4 = 1f
        };
      }
    }

    public Matrix3 Matrix
    {
      get
      {
        return new Matrix3()
        {
          A1 = (float) (1.0 - 2.0 * ((double) this.y * (double) this.y + (double) this.z * (double) this.z)),
          A2 = (float) (2.0 * ((double) this.x * (double) this.y + (double) this.w * (double) this.z)),
          A3 = (float) (2.0 * ((double) this.x * (double) this.z - (double) this.w * (double) this.y)),
          B1 = (float) (2.0 * ((double) this.x * (double) this.y - (double) this.w * (double) this.z)),
          B2 = (float) (1.0 - 2.0 * ((double) this.x * (double) this.x + (double) this.z * (double) this.z)),
          B3 = (float) (2.0 * ((double) this.y * (double) this.z + (double) this.w * (double) this.x)),
          C1 = (float) (2.0 * ((double) this.x * (double) this.z + (double) this.w * (double) this.y)),
          C2 = (float) (2.0 * ((double) this.y * (double) this.z - (double) this.w * (double) this.x)),
          C3 = (float) (1.0 - 2.0 * ((double) this.x * (double) this.x + (double) this.y * (double) this.y))
        };
      }
    }

    public Quaternion(float x, float y, float z, float w)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.w = w;
    }

    public float Dot(Quaternion b)
    {
      return (float) ((double) this.x * (double) b.x + (double) this.y * (double) b.y + (double) this.z * (double) b.z + (double) this.w * (double) b.w);
    }

    public static float Length(Quaternion q) => Numeric.Sqrt(q.Dot(q));

    public static Quaternion operator *(Quaternion q, float s)
    {
      return new Quaternion(s * q.x, s * q.y, s * q.z, s * q.w);
    }

    public static Quaternion operator *(float s, Quaternion q)
    {
      return new Quaternion(s * q.x, s * q.y, s * q.z, s * q.w);
    }

    public static Quaternion operator +(Quaternion a, Quaternion b)
    {
      return new Quaternion(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public static Quaternion operator -(Quaternion a, Quaternion b)
    {
      return new Quaternion(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }

    public static Quaternion operator -(Quaternion a) => new Quaternion(-a.x, -a.y, -a.z, -a.w);

    public static Quaternion Conjugate(Quaternion q) => new Quaternion(-q.x, -q.y, -q.z, q.w);

    public static float Norm(Quaternion q)
    {
      return (float) ((double) q.x * (double) q.x + (double) q.y * (double) q.y + (double) q.z * (double) q.z + (double) q.w * (double) q.w);
    }

    public static Quaternion operator /(Quaternion a, float scalar)
    {
      float num = 1f / scalar;
      return new Quaternion(a.x * num, a.y + num, a.z * num, a.w * num);
    }

    public static Quaternion Inverse(Quaternion q) => Quaternion.Conjugate(q) / Quaternion.Norm(q);

    public static Quaternion operator /(Quaternion a, Quaternion b) => a * Quaternion.Inverse(b);

    public static Quaternion operator *(Quaternion a, Quaternion b)
    {
      return new Quaternion((float) ((double) a.x * (double) b.w + (double) a.y * (double) b.z - (double) a.z * (double) b.y + (double) a.w * (double) b.x), (float) (-(double) a.x * (double) b.z + (double) a.y * (double) b.w + (double) a.z * (double) b.x + (double) a.w * (double) b.y), (float) ((double) a.x * (double) b.y - (double) a.y * (double) b.x + (double) a.z * (double) b.w + (double) a.w * (double) b.z), (float) (-(double) a.x * (double) b.x - (double) a.y * (double) b.y - (double) a.z * (double) b.z + (double) a.w * (double) b.w));
    }

    public static Quaternion Normalize(Quaternion q)
    {
      float num = 1f / Numeric.Sqrt((float) ((double) q.x * (double) q.x + (double) q.y * (double) q.y + (double) q.z * (double) q.z + (double) q.w * (double) q.w));
      return new Quaternion(q.x * num, q.y * num, q.z * num, q.w * num);
    }

    public static Quaternion Log(Quaternion q)
    {
      float angle = Numeric.Acos(q.w);
      float num = Numeric.Sin(angle);
      return (double) num > 0.0 ? new Quaternion(angle * q.X / num, angle * q.Y / num, angle * q.Z / num, 0.0f) : new Quaternion(q.X, q.Y, q.Z, 0.0f);
    }

    public static Quaternion Exp(Quaternion q)
    {
      float angle = Numeric.Sqrt((float) ((double) q.x * (double) q.x + (double) q.y * (double) q.y + (double) q.z * (double) q.z));
      float num = Numeric.Sin(angle);
      float w = Numeric.Cos(angle);
      return (double) angle > 0.0 ? new Quaternion(num * q.x / angle, num * q.y / angle, num * q.z / angle, w) : new Quaternion(q.x, q.y, q.z, w);
    }

    public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
    {
      return Quaternion.Normalize(a + t * (a - b));
    }

    public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
    {
      float number = a.Dot(b);
      float num1 = 1f;
      if ((double) number < 0.0)
      {
        number = -number;
        num1 = -1f;
      }
      float num2;
      float num3;
      if ((double) number < 0.99900001287460327)
      {
        float num4 = Numeric.Acos(number);
        float num5 = 1f / Numeric.Sqrt((float) (1.0 - (double) number * (double) number));
        num2 = Numeric.Sin((1f - t) * num4) * num5;
        num3 = Numeric.Sin(t * num4) * num5;
      }
      else
      {
        num2 = 1f - t;
        num3 = t;
      }
      return Quaternion.Normalize(num2 * a + num1 * num3 * b);
    }

    public static Quaternion Squad(
      Quaternion a,
      Quaternion b,
      Quaternion ta,
      Quaternion tb,
      float t)
    {
      float t1 = (float) (2.0 * (double) t * (1.0 - (double) t));
      return Quaternion.Slerp(Quaternion.Slerp(a, b, t), Quaternion.Slerp(ta, tb, t), t1);
    }

    public static Quaternion SimpleSquad(
      Quaternion prev,
      Quaternion a,
      Quaternion b,
      Quaternion post,
      float t)
    {
      if ((double) prev.Dot(a) < 0.0)
        a = -a;
      if ((double) a.Dot(b) < 0.0)
        b = -b;
      if ((double) b.Dot(post) < 0.0)
        post = -post;
      Quaternion ta = Quaternion.Spline(prev, a, b);
      Quaternion tb = Quaternion.Spline(a, b, post);
      return Quaternion.Squad(a, b, ta, tb, t);
    }

    public static Quaternion Spline(Quaternion pre, Quaternion q, Quaternion post)
    {
      Quaternion quaternion = Quaternion.Conjugate(q);
      return q * Quaternion.Exp((Quaternion.Log(quaternion * pre) + Quaternion.Log(quaternion * post)) * -0.25f);
    }

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      Quaternion quaternion = (Quaternion) obj;
      return (double) this.x == (double) quaternion.x && (double) this.y == (double) quaternion.y && (double) this.z == (double) quaternion.z && (double) this.w == (double) quaternion.w;
    }

    public override int GetHashCode()
    {
      return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode() ^ this.w.GetHashCode();
    }

    public static bool operator ==(Quaternion a, Quaternion b) => a.Equals((object) b);

    public static bool operator !=(Quaternion a, Quaternion b) => !(a == b);
  }
}

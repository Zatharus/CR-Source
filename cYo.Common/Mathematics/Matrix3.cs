// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Matrix3
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Common.Mathematics
{
  [TypeConverter(typeof (ExpandableObjectConverter))]
  public struct Matrix3
  {
    public float A1;
    public float A2;
    public float A3;
    public float B1;
    public float B2;
    public float B3;
    public float C1;
    public float C2;
    public float C3;

    public Matrix3(
      float a1,
      float a2,
      float a3,
      float b1,
      float b2,
      float b3,
      float c1,
      float c2,
      float c3)
    {
      this.A1 = a1;
      this.B1 = b1;
      this.C1 = c1;
      this.A2 = a2;
      this.B2 = b2;
      this.C2 = c2;
      this.A3 = a3;
      this.B3 = b3;
      this.C3 = c3;
    }

    public Matrix3(Vector3 A, Vector3 B, Vector3 C)
    {
      this.A1 = A.X;
      this.B1 = B.X;
      this.C1 = C.X;
      this.A2 = A.Y;
      this.B2 = B.Y;
      this.C2 = C.Y;
      this.A3 = A.Z;
      this.B3 = B.Z;
      this.C3 = C.Z;
    }

    public static Matrix3 RotationX(float alpha)
    {
      float num = Numeric.Cos(alpha);
      float b3 = Numeric.Sin(alpha);
      return new Matrix3(1f, 0.0f, 0.0f, 0.0f, num, b3, 0.0f, -b3, num);
    }

    public static Matrix3 RotationY(float alpha)
    {
      float num = Numeric.Cos(alpha);
      float c1 = Numeric.Sin(alpha);
      return new Matrix3(num, 0.0f, -c1, 0.0f, 1f, 0.0f, c1, 0.0f, num);
    }

    public static Matrix3 RotationZ(float alpha)
    {
      float num = Numeric.Cos(alpha);
      float a2 = Numeric.Sin(alpha);
      return new Matrix3(num, a2, 0.0f, -a2, num, 0.0f, 0.0f, 0.0f, 1f);
    }

    public static Matrix3 Rotation(float alpha, float beta, float gamma)
    {
      float num1 = Numeric.Sin(alpha);
      float num2 = Numeric.Cos(alpha);
      float num3 = Numeric.Sin(beta);
      float num4 = Numeric.Cos(beta);
      float num5 = Numeric.Sin(gamma);
      float num6 = Numeric.Cos(gamma);
      float num7 = num3 * num1;
      float num8 = num3 * num2;
      return new Matrix3()
      {
        A1 = num6 * num4,
        A2 = num5 * num4,
        A3 = -num3,
        B1 = (float) ((double) num6 * (double) num7 - (double) num5 * (double) num2),
        B2 = (float) ((double) num5 * (double) num7 + (double) num6 * (double) num2),
        B3 = num4 * num1,
        C1 = (float) ((double) num6 * (double) num8 + (double) num5 * (double) num1),
        C2 = (float) ((double) num5 * (double) num8 - (double) num6 * (double) num1),
        C3 = num4 * num2
      };
    }

    public static Matrix3 Scaling(float value) => Matrix3.Scaling(new Vector3(value, value, value));

    public static Matrix3 Scaling(Vector3 vec)
    {
      return new Matrix3()
      {
        A1 = vec.X,
        B2 = vec.Y,
        C3 = vec.Z
      };
    }

    public static Matrix3 LookAt(Vector3 eye, Vector3 at, Vector3 up)
    {
      Vector3 vector3_1 = Vector3.Unit(at - eye);
      Vector3 b = Vector3.Unit(Vector3.Cross(up, vector3_1));
      Vector3 vector3_2 = Vector3.Cross(vector3_1, b);
      return new Matrix3(b.X, b.Y, b.Z, vector3_2.X, vector3_2.Y, vector3_2.Z, vector3_1.X, vector3_1.Y, vector3_1.Z);
    }

    public static Matrix3 Rotation(Vector3 vec, float angle)
    {
      float num1 = Numeric.Cos(angle);
      float num2 = Numeric.Sin(angle);
      float num3 = 1f - num1;
      return Matrix3.Transpose(new Matrix3(num3 * vec.X * vec.X + num1, (float) ((double) num3 * (double) vec.X * (double) vec.Y - (double) num2 * (double) vec.Z), (float) ((double) num3 * (double) vec.X * (double) vec.Z + (double) num2 * (double) vec.Y), (float) ((double) num3 * (double) vec.X * (double) vec.Y + (double) num2 * (double) vec.Z), num3 * vec.Y * vec.Y + num1, (float) ((double) num3 * (double) vec.Y * (double) vec.Z - (double) num2 * (double) vec.X), (float) ((double) num3 * (double) vec.X * (double) vec.Z - (double) num2 * (double) vec.Y), (float) ((double) num3 * (double) vec.Y * (double) vec.Z + (double) num2 * (double) vec.X), num3 * vec.Z * vec.Z + num1));
    }

    public static Matrix3 Transpose(Matrix3 matrix)
    {
      return new Matrix3()
      {
        A1 = matrix.A1,
        A2 = matrix.B1,
        A3 = matrix.C1,
        B1 = matrix.A2,
        B2 = matrix.B2,
        B3 = matrix.C2,
        C1 = matrix.A3,
        C2 = matrix.B3,
        C3 = matrix.C3
      };
    }

    public float Det()
    {
      return (float) ((double) this.A1 * (double) this.B2 * (double) this.C3 + (double) this.A2 * (double) this.B3 * (double) this.C1 + (double) this.A3 * (double) this.B1 * (double) this.C2 - (double) this.A3 * (double) this.B2 * (double) this.C1 - (double) this.A1 * (double) this.B3 * (double) this.C2 - (double) this.A2 * (double) this.B1 * (double) this.C3);
    }

    public static Matrix3 Slerp(Matrix3 a, Matrix3 b, float time)
    {
      return Quaternion.Slerp(a.Quaternion, b.Quaternion, time).Matrix;
    }

    public static Matrix3 Identity
    {
      get
      {
        Matrix3 identity = new Matrix3();
        identity.A1 = identity.B2 = identity.C3 = 1f;
        return identity;
      }
    }

    public static Matrix3 Zero => new Matrix3();

    public Vector3 Column1
    {
      get => new Vector3(this.A1, this.B1, this.C1);
      set
      {
        this.A1 = value.X;
        this.B1 = value.Y;
        this.C1 = value.Z;
      }
    }

    public Vector3 Column2
    {
      get => new Vector3(this.A2, this.B2, this.C2);
      set
      {
        this.A2 = value.X;
        this.B2 = value.Y;
        this.C2 = value.Z;
      }
    }

    public Vector3 Column3
    {
      get => new Vector3(this.A3, this.B3, this.C3);
      set
      {
        this.A3 = value.X;
        this.B3 = value.Y;
        this.C3 = value.Z;
      }
    }

    public Vector3 LookAtVector
    {
      get => new Vector3(this.A3, this.B3, this.C3);
      set
      {
        this.A3 = value.X;
        this.B3 = value.Y;
        this.C3 = value.Z;
      }
    }

    public Vector3 UpVector
    {
      get => new Vector3(this.A2, this.B2, this.C2);
      set
      {
        this.A2 = value.X;
        this.B2 = value.Y;
        this.C2 = value.Z;
      }
    }

    public Vector3 RightVector
    {
      get => new Vector3(this.A1, this.B1, this.C1);
      set
      {
        this.A1 = value.X;
        this.B1 = value.Y;
        this.C1 = value.Z;
      }
    }

    [Browsable(false)]
    public Quaternion Quaternion
    {
      get
      {
        float num1 = this.A1 + this.B2 + this.C3;
        float x;
        float y;
        float z;
        float w;
        if ((double) num1 > 1E-08)
        {
          float num2 = Numeric.Sqrt(num1) * 2f;
          x = (this.B3 - this.C2) / num2;
          y = (this.C1 - this.A3) / num2;
          z = (this.A2 - this.B1) / num2;
          w = 0.25f * num2;
        }
        else if ((double) this.A1 > (double) this.B2 && (double) this.A1 > (double) this.C3)
        {
          float num3 = Numeric.Sqrt(1f + this.A1 - this.B2 - this.C3) * 2f;
          x = 0.25f * num3;
          y = (this.A2 + this.B1) / num3;
          z = (this.C1 + this.A3) / num3;
          w = (this.B3 - this.C2) / num3;
        }
        else if ((double) this.B2 > (double) this.C3)
        {
          float num4 = Numeric.Sqrt(1f + this.B2 - this.A1 - this.C3) * 2f;
          x = (this.A2 + this.B1) / num4;
          y = 0.25f * num4;
          z = (this.B3 + this.C2) / num4;
          w = (this.C1 - this.A3) / num4;
        }
        else
        {
          float num5 = Numeric.Sqrt(1f + this.C3 - this.A1 - this.B2) * 2f;
          x = (this.C1 + this.A3) / num5;
          y = (this.B3 + this.C2) / num5;
          z = 0.25f * num5;
          w = (this.A2 - this.B1) / num5;
        }
        return new Quaternion(x, y, z, w);
      }
    }

    public static Matrix3 operator +(Matrix3 a, Matrix3 b)
    {
      return new Matrix3(a.A1 + b.A1, a.A2 + b.A2, a.A3 + b.A3, a.B1 + b.B1, a.B2 + b.B2, a.B3 + b.B3, a.C1 + b.C1, a.C2 + b.C2, a.C3 + b.C3);
    }

    public float this[int column, int row]
    {
      get => this[column + row * 3];
      set => this[column + row * 3] = value;
    }

    public float this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return this.A1;
          case 1:
            return this.A2;
          case 2:
            return this.A3;
          case 3:
            return this.B1;
          case 4:
            return this.B2;
          case 5:
            return this.B3;
          case 6:
            return this.C1;
          case 7:
            return this.C2;
          case 8:
            return this.C3;
          default:
            throw new IndexOutOfRangeException("Invalid matrix index!");
        }
      }
      set
      {
        switch (index)
        {
          case 0:
            this.A1 = value;
            break;
          case 1:
            this.A2 = value;
            break;
          case 2:
            this.A3 = value;
            break;
          case 3:
            this.B1 = value;
            break;
          case 4:
            this.B2 = value;
            break;
          case 5:
            this.B3 = value;
            break;
          case 6:
            this.C1 = value;
            break;
          case 7:
            this.C2 = value;
            break;
          case 8:
            this.C3 = value;
            break;
          default:
            throw new IndexOutOfRangeException("Invalid matrix index!");
        }
      }
    }

    public static Matrix3 operator *(Matrix3 a, Matrix3 b)
    {
      Matrix3 zero = Matrix3.Zero;
      for (int column = 0; column < 3; ++column)
      {
        for (int row = 0; row < 3; ++row)
        {
          for (int index = 0; index < 3; ++index)
            zero[column, row] += a[index, row] * b[column, index];
        }
      }
      return zero;
    }

    public static bool operator ==(Matrix3 a, Matrix3 b) => a.Equals((object) b);

    public static bool operator !=(Matrix3 a, Matrix3 b) => !(a == b);

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      Matrix3 matrix3 = (Matrix3) obj;
      return (double) this.A1 == (double) matrix3.A1 && (double) this.A2 == (double) matrix3.A2 && (double) this.A3 == (double) matrix3.A3 && (double) this.B1 == (double) matrix3.B1 && (double) this.B2 == (double) matrix3.B2 && (double) this.B3 == (double) matrix3.B3 && (double) this.C1 == (double) matrix3.C1 && (double) this.C2 == (double) matrix3.C2 && (double) this.C3 == (double) matrix3.C3;
    }

    public override int GetHashCode()
    {
      return this.A1.GetHashCode() ^ this.B2.GetHashCode() ^ this.C3.GetHashCode();
    }
  }
}

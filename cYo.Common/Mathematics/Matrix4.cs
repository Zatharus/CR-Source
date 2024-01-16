// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Matrix4
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Common.Mathematics
{
  [TypeConverter(typeof (ExpandableObjectConverter))]
  public struct Matrix4
  {
    public float A1;
    public float A2;
    public float A3;
    public float A4;
    public float B1;
    public float B2;
    public float B3;
    public float B4;
    public float C1;
    public float C2;
    public float C3;
    public float C4;
    public float D1;
    public float D2;
    public float D3;
    public float D4;

    public static Matrix4 Identity
    {
      get
      {
        Matrix4 zero = Matrix4.Zero;
        zero.A1 = zero.B2 = zero.C3 = zero.D4 = 1f;
        return zero;
      }
    }

    public static Matrix4 Zero => new Matrix4();

    public Vector4 Column1
    {
      get => new Vector4(this.A1, this.B1, this.C1, this.D1);
      set
      {
        this.A1 = value.X;
        this.B1 = value.Y;
        this.C1 = value.Z;
        this.D1 = value.W;
      }
    }

    public Vector4 Column2
    {
      get => new Vector4(this.A2, this.B2, this.C2, this.D2);
      set
      {
        this.A2 = value.X;
        this.B2 = value.Y;
        this.C2 = value.Z;
        this.D2 = value.W;
      }
    }

    public Vector4 Column3
    {
      get => new Vector4(this.A3, this.B3, this.C3, this.D3);
      set
      {
        this.A3 = value.X;
        this.B3 = value.Y;
        this.C3 = value.Z;
        this.D3 = value.W;
      }
    }

    public Vector4 Column4
    {
      get => new Vector4(this.A4, this.B4, this.C4, this.D4);
      set
      {
        this.A4 = value.X;
        this.B4 = value.Y;
        this.C4 = value.Z;
        this.D4 = value.W;
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

    public Vector3 TranslationVector
    {
      get => new Vector3(this.D1, this.D2, this.D3);
      set
      {
        this.D1 = value.X;
        this.D2 = value.Y;
        this.D3 = value.Z;
      }
    }

    public Matrix3 RotationMatrix
    {
      get
      {
        return new Matrix3(this.A1, this.A2, this.A3, this.B1, this.B2, this.B3, this.C1, this.C2, this.C3);
      }
      set
      {
        this.A1 = value.A1;
        this.B1 = value.B1;
        this.C1 = value.C1;
        this.A2 = value.A2;
        this.B2 = value.B2;
        this.C2 = value.C2;
        this.A3 = value.A3;
        this.B3 = value.B3;
        this.C3 = value.C3;
      }
    }

    public Quaternion Quaternion
    {
      get
      {
        float num1 = this.A1 + this.B2 + this.C3;
        float x;
        float y;
        float z;
        float w;
        if ((double) num1 > 0.0)
        {
          float num2 = Numeric.Sqrt(num1 + 1f);
          float num3 = 0.5f / num2;
          x = (this.B3 - this.C2) * num3;
          y = (this.C1 - this.A3) * num3;
          z = (this.A2 - this.B1) * num3;
          w = 0.5f * num2;
        }
        else if ((double) this.A1 > (double) this.B2 && (double) this.A1 > (double) this.C3)
        {
          float num4 = Numeric.Sqrt(1f + this.A1 - this.B2 - this.C3);
          float num5 = 0.5f / num4;
          x = 0.5f * num4;
          y = (this.A2 + this.B1) * num5;
          z = (this.C1 + this.A3) * num5;
          w = (this.B3 - this.C2) * num5;
        }
        else if ((double) this.B2 > (double) this.C3)
        {
          float num6 = Numeric.Sqrt(1f + this.B2 - this.A1 - this.C3);
          float num7 = 0.5f / num6;
          x = (this.A2 + this.B1) * num7;
          y = 0.5f * num6;
          z = (this.B3 + this.C2) * num7;
          w = (this.C1 - this.A3) * num7;
        }
        else
        {
          float num8 = Numeric.Sqrt(1f + this.C3 - this.A1 - this.B2);
          float num9 = 0.5f / num8;
          x = (this.C1 + this.A3) * num9;
          y = (this.B3 + this.C2) * num9;
          z = 0.5f * num8;
          w = (this.A2 - this.B1) * num9;
        }
        return new Quaternion(x, y, z, w);
      }
    }

    public Matrix4(
      float a1,
      float a2,
      float a3,
      float a4,
      float b1,
      float b2,
      float b3,
      float b4,
      float c1,
      float c2,
      float c3,
      float c4,
      float d1,
      float d2,
      float d3,
      float d4)
    {
      this.A1 = a1;
      this.B1 = b1;
      this.C1 = c1;
      this.D1 = d1;
      this.A2 = a2;
      this.B2 = b2;
      this.C2 = c2;
      this.D2 = d2;
      this.A3 = a3;
      this.B3 = b3;
      this.C3 = c3;
      this.D3 = d3;
      this.A4 = a4;
      this.B4 = b4;
      this.C4 = c4;
      this.D4 = d4;
    }

    public Matrix4(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {
      this.A1 = A.X;
      this.B1 = B.X;
      this.C1 = C.X;
      this.D1 = D.X;
      this.A2 = A.Y;
      this.B2 = B.Y;
      this.C2 = C.Y;
      this.D2 = D.Y;
      this.A3 = A.Z;
      this.B3 = B.Z;
      this.C3 = C.Z;
      this.D3 = D.Z;
      this.A4 = 0.0f;
      this.B4 = 0.0f;
      this.C4 = 0.0f;
      this.D4 = 1f;
    }

    public Matrix4(Matrix3 rot, Vector3 trans)
    {
      this.A1 = rot.A1;
      this.B1 = rot.B1;
      this.C1 = rot.C1;
      this.D1 = trans.X;
      this.A2 = rot.A2;
      this.B2 = rot.B2;
      this.C2 = rot.C2;
      this.D2 = trans.Y;
      this.A3 = rot.A3;
      this.B3 = rot.B3;
      this.C3 = rot.C3;
      this.D3 = trans.Z;
      this.A4 = 0.0f;
      this.B4 = 0.0f;
      this.C4 = 0.0f;
      this.D4 = 1f;
    }

    public static Matrix4 RotationX(float alpha)
    {
      float num = Numeric.Cos(alpha);
      float b3 = Numeric.Sin(alpha);
      return new Matrix4(1f, 0.0f, 0.0f, 0.0f, 0.0f, num, b3, 0.0f, 0.0f, -b3, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public static Matrix4 RotationY(float alpha)
    {
      float num = Numeric.Cos(alpha);
      float c1 = Numeric.Sin(alpha);
      return new Matrix4(num, 0.0f, -c1, 0.0f, 0.0f, 1f, 0.0f, 0.0f, c1, 0.0f, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public static Matrix4 RotationZ(float alpha)
    {
      float num = Numeric.Cos(alpha);
      float a2 = Numeric.Sin(alpha);
      return new Matrix4(num, a2, 0.0f, 0.0f, -a2, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public static Matrix4 Rotation(float alpha, float beta, float gamma)
    {
      float num1 = Numeric.Sin(alpha);
      float num2 = Numeric.Cos(alpha);
      float num3 = Numeric.Sin(beta);
      float num4 = Numeric.Cos(beta);
      float num5 = Numeric.Sin(gamma);
      float num6 = Numeric.Cos(gamma);
      float num7 = num3 * num1;
      float num8 = num3 * num2;
      return Matrix4.Zero with
      {
        A1 = num6 * num4,
        A2 = num5 * num4,
        A3 = -num3,
        A4 = 0.0f,
        B1 = (float) ((double) num6 * (double) num7 - (double) num5 * (double) num2),
        B2 = (float) ((double) num5 * (double) num7 + (double) num6 * (double) num2),
        B3 = num4 * num1,
        B4 = 0.0f,
        C1 = (float) ((double) num6 * (double) num8 + (double) num5 * (double) num1),
        C2 = (float) ((double) num5 * (double) num8 - (double) num6 * (double) num1),
        C3 = num4 * num2,
        C4 = 0.0f,
        D1 = 0.0f,
        D2 = 0.0f,
        D3 = 0.0f,
        D4 = 1f
      };
    }

    public static Matrix4 Translation(Vector3 vec)
    {
      return new Matrix4(Vector3.Right, Vector3.Up, Vector3.LookAt, vec);
    }

    public static Matrix4 Translation(float x, float y, float z)
    {
      return Matrix4.Translation(new Vector3(x, y, z));
    }

    public static Matrix4 Scaling(float value) => Matrix4.Scaling(new Vector3(value, value, value));

    public static Matrix4 Scaling(Vector3 vec)
    {
      return Matrix4.Zero with
      {
        A1 = vec.X,
        B2 = vec.Y,
        C3 = vec.Z,
        D4 = 1f
      };
    }

    public void Translate(Vector3 vec)
    {
      this.D1 += vec.X;
      this.D2 += vec.Y;
      this.D3 += vec.Z;
    }

    public void Translate(float x, float y, float z)
    {
      this.D1 += x;
      this.D2 += y;
      this.D3 += z;
    }

    public static Matrix4 Perspective(float width, float height, float near, float far)
    {
      return (double) far == double.PositiveInfinity ? Matrix4.PerspectiveInfinity(width, height, near) : new Matrix4(2f * near / width, 0.0f, 0.0f, 0.0f, 0.0f, 2f * near / height, 0.0f, 0.0f, 0.0f, 0.0f, far / (far - near), 1f, 0.0f, 0.0f, (float) ((double) near * (double) far / ((double) near - (double) far)), 0.0f);
    }

    public static Matrix4 PerspectiveInfinity(float width, float height, float near)
    {
      return new Matrix4(2f * near / width, 0.0f, 0.0f, 0.0f, 0.0f, 2f * near / height, 0.0f, 0.0f, 0.0f, 0.0f, 0.999f, 1f, 0.0f, 0.0f, near * -0.999f, 0.0f);
    }

    public static Matrix4 PerspectiveFOV(float fovY, float ratio, float near, float far)
    {
      if ((double) far == double.PositiveInfinity)
        return Matrix4.PerspectiveFOVInfinity(fovY, ratio, near);
      float b2 = Numeric.Cot(fovY / 2f);
      return new Matrix4(b2 / ratio, 0.0f, 0.0f, 0.0f, 0.0f, b2, 0.0f, 0.0f, 0.0f, 0.0f, far / (far - near), 1f, 0.0f, 0.0f, (float) (-(double) near * (double) far / ((double) far - (double) near)), 0.0f);
    }

    public static Matrix4 Orthogonal(float w, float h, float near, float far)
    {
      return new Matrix4(2f / w, 0.0f, 0.0f, 0.0f, 0.0f, 2f / h, 0.0f, 0.0f, 0.0f, 0.0f, (float) (1.0 / ((double) far - (double) near)), 0.0f, 0.0f, 0.0f, near / (near - far), 1f);
    }

    public static Matrix4 PerspectiveFOVInfinity(float fovY, float ratio, float near)
    {
      float b2 = Numeric.Cot(fovY / 2f);
      return new Matrix4(b2 / ratio, 0.0f, 0.0f, 0.0f, 0.0f, b2, 0.0f, 0.0f, 0.0f, 0.0f, 0.999f, 1f, 0.0f, 0.0f, near * -0.999f, 0.0f);
    }

    public static Matrix4 LookAt(Vector3 eye, Vector3 at, Vector3 up)
    {
      Vector3 vector3_1 = Vector3.Unit(at - eye);
      Vector3 b = Vector3.Unit(Vector3.Cross(up, vector3_1));
      Vector3 vector3_2 = Vector3.Cross(vector3_1, b);
      return new Matrix4(b.X, b.Y, b.Z, 0.0f, vector3_2.X, vector3_2.Y, vector3_2.Z, 0.0f, vector3_1.X, vector3_1.Y, vector3_1.Z, 0.0f, -b * eye, -vector3_2 * eye, -vector3_1 * eye, 1f);
    }

    public static Matrix4 Rotation(Vector3 vec, float angle)
    {
      float num1 = Numeric.Cos(angle);
      float num2 = Numeric.Sin(angle);
      float num3 = 1f - num1;
      return Matrix4.Transpose(new Matrix4(num3 * vec.X * vec.X + num1, (float) ((double) num3 * (double) vec.X * (double) vec.Y + (double) num2 * (double) vec.Z), (float) ((double) num3 * (double) vec.X * (double) vec.Z + (double) num2 * (double) vec.Y), 0.0f, (float) ((double) num3 * (double) vec.X * (double) vec.Y - (double) num2 * (double) vec.Z), num3 * vec.Y * vec.Y + num1, (float) ((double) num3 * (double) vec.Y * (double) vec.Z + (double) num2 * (double) vec.X), 0.0f, (float) ((double) num3 * (double) vec.X * (double) vec.Z + (double) num2 * (double) vec.Y), (float) ((double) num3 * (double) vec.Y * (double) vec.Z - (double) num2 * (double) vec.X), num3 * vec.Z * vec.Z + num1, 0.0f, 0.0f, 0.0f, 0.0f, 1f));
    }

    public static Matrix4 Slerp(Matrix4 a, Matrix4 b, float time)
    {
      return Quaternion.Slerp(a.Quaternion, b.Quaternion, time).Matrix4 with
      {
        TranslationVector = Vector3.Lerp(a.TranslationVector, b.TranslationVector, time)
      };
    }

    public static Matrix4 Squad(Matrix4 pre, Matrix4 a, Matrix4 b, Matrix4 post, float time)
    {
      return Quaternion.SimpleSquad(pre.Quaternion, a.Quaternion, b.Quaternion, post.Quaternion, time).Matrix4;
    }

    public static Matrix4 Transpose(Matrix4 matrix)
    {
      return Matrix4.Zero with
      {
        A1 = matrix.A1,
        A2 = matrix.B1,
        A3 = matrix.C1,
        A4 = matrix.D1,
        B1 = matrix.A2,
        B2 = matrix.B2,
        B3 = matrix.C2,
        B4 = matrix.D2,
        C1 = matrix.A3,
        C2 = matrix.B3,
        C3 = matrix.C3,
        C4 = matrix.D3,
        D1 = matrix.A4,
        D2 = matrix.B4,
        D3 = matrix.C4,
        D4 = matrix.D4
      };
    }

    public static Matrix3 Minor(Matrix4 source, int column, int row)
    {
      int row1 = 0;
      Matrix3 matrix3 = new Matrix3();
      for (int row2 = 0; row2 < 4; ++row2)
      {
        int column1 = 0;
        if (row2 != row)
        {
          for (int column2 = 0; column2 < 4; ++column2)
          {
            if (column2 != column)
            {
              matrix3[column1, row1] = source[column2, row2];
              ++column1;
            }
          }
          ++row1;
        }
      }
      return matrix3;
    }

    public static Matrix4 Adjoint(Matrix4 source)
    {
      Matrix4 zero = Matrix4.Zero;
      Matrix3 matrix3;
      for (int column1 = 0; column1 < 4; ++column1)
      {
        for (int row1 = 0; row1 < 4; ++row1)
        {
          if ((row1 + column1) % 2 == 0)
          {
            ref Matrix4 local = ref zero;
            int column2 = row1;
            int row2 = column1;
            matrix3 = Matrix4.Minor(source, column1, row1);
            double num = (double) matrix3.Det();
            local[column2, row2] = (float) num;
          }
          else
          {
            ref Matrix4 local = ref zero;
            int column3 = row1;
            int row3 = column1;
            matrix3 = Matrix4.Minor(source, column1, row1);
            double num = -(double) matrix3.Det();
            local[column3, row3] = (float) num;
          }
        }
      }
      return zero;
    }

    public float Det()
    {
      return (float) ((double) this.A4 * (double) this.B3 * (double) this.C2 * (double) this.D1 - (double) this.A3 * (double) this.B4 * (double) this.C2 * (double) this.D1 - (double) this.A4 * (double) this.B2 * (double) this.C3 * (double) this.D1 + (double) this.A2 * (double) this.B4 * (double) this.C3 * (double) this.D1 + (double) this.A3 * (double) this.B2 * (double) this.C4 * (double) this.D1 - (double) this.A2 * (double) this.B3 * (double) this.C4 * (double) this.D1 - (double) this.A4 * (double) this.B3 * (double) this.C1 * (double) this.D2 + (double) this.A3 * (double) this.B4 * (double) this.C1 * (double) this.D2 + (double) this.A4 * (double) this.B1 * (double) this.C3 * (double) this.D2 - (double) this.A1 * (double) this.B4 * (double) this.C3 * (double) this.D2 - (double) this.A3 * (double) this.B1 * (double) this.C4 * (double) this.D2 + (double) this.A1 * (double) this.B3 * (double) this.C4 * (double) this.D2 + (double) this.A4 * (double) this.B2 * (double) this.C1 * (double) this.D3 - (double) this.A2 * (double) this.B4 * (double) this.C1 * (double) this.D3 - (double) this.A4 * (double) this.B1 * (double) this.C2 * (double) this.D3 + (double) this.A1 * (double) this.B4 * (double) this.C2 * (double) this.D3 + (double) this.A2 * (double) this.B1 * (double) this.C4 * (double) this.D3 - (double) this.A1 * (double) this.B2 * (double) this.C4 * (double) this.D3 - (double) this.A3 * (double) this.B2 * (double) this.C1 * (double) this.D4 + (double) this.A2 * (double) this.B3 * (double) this.C1 * (double) this.D4 + (double) this.A3 * (double) this.B1 * (double) this.C2 * (double) this.D4 - (double) this.A1 * (double) this.B3 * (double) this.C2 * (double) this.D4 - (double) this.A2 * (double) this.B1 * (double) this.C3 * (double) this.D4 + (double) this.A1 * (double) this.B2 * (double) this.C3 * (double) this.D4);
    }

    public static Matrix4 Invert(Matrix4 m)
    {
      return Matrix4.Zero with
      {
        A1 = (float) ((double) m.B3 * (double) m.C4 * (double) m.D2 - (double) m.B4 * (double) m.C3 * (double) m.D2 + (double) m.B4 * (double) m.C2 * (double) m.D3 - (double) m.B2 * (double) m.C4 * (double) m.D3 - (double) m.B3 * (double) m.C2 * (double) m.D4 + (double) m.B2 * (double) m.C3 * (double) m.D4),
        A2 = (float) ((double) m.A4 * (double) m.C3 * (double) m.D2 - (double) m.A3 * (double) m.C4 * (double) m.D2 - (double) m.A4 * (double) m.C2 * (double) m.D3 + (double) m.A2 * (double) m.C4 * (double) m.D3 + (double) m.A3 * (double) m.C2 * (double) m.D4 - (double) m.A2 * (double) m.C3 * (double) m.D4),
        A3 = (float) ((double) m.A3 * (double) m.B4 * (double) m.D2 - (double) m.A4 * (double) m.B3 * (double) m.D2 + (double) m.A4 * (double) m.B2 * (double) m.D3 - (double) m.A2 * (double) m.B4 * (double) m.D3 - (double) m.A3 * (double) m.B2 * (double) m.D4 + (double) m.A2 * (double) m.B3 * (double) m.D4),
        A4 = (float) ((double) m.A4 * (double) m.B3 * (double) m.C2 - (double) m.A3 * (double) m.B4 * (double) m.C2 - (double) m.A4 * (double) m.B2 * (double) m.C3 + (double) m.A2 * (double) m.B4 * (double) m.C3 + (double) m.A3 * (double) m.B2 * (double) m.C4 - (double) m.A2 * (double) m.B3 * (double) m.C4),
        B1 = (float) ((double) m.B4 * (double) m.C3 * (double) m.D1 - (double) m.B3 * (double) m.C4 * (double) m.D1 - (double) m.B4 * (double) m.C1 * (double) m.D3 + (double) m.B1 * (double) m.C4 * (double) m.D3 + (double) m.B3 * (double) m.C1 * (double) m.D4 - (double) m.B1 * (double) m.C3 * (double) m.D4),
        B2 = (float) ((double) m.A3 * (double) m.C4 * (double) m.D1 - (double) m.A4 * (double) m.C3 * (double) m.D1 + (double) m.A4 * (double) m.C1 * (double) m.D3 - (double) m.A1 * (double) m.C4 * (double) m.D3 - (double) m.A3 * (double) m.C1 * (double) m.D4 + (double) m.A1 * (double) m.C3 * (double) m.D4),
        B3 = (float) ((double) m.A4 * (double) m.B3 * (double) m.D1 - (double) m.A3 * (double) m.B4 * (double) m.D1 - (double) m.A4 * (double) m.B1 * (double) m.D3 + (double) m.A1 * (double) m.B4 * (double) m.D3 + (double) m.A3 * (double) m.B1 * (double) m.D4 - (double) m.A1 * (double) m.B3 * (double) m.D4),
        B4 = (float) ((double) m.A3 * (double) m.B4 * (double) m.C1 - (double) m.A4 * (double) m.B3 * (double) m.C1 + (double) m.A4 * (double) m.B1 * (double) m.C3 - (double) m.A1 * (double) m.B4 * (double) m.C3 - (double) m.A3 * (double) m.B1 * (double) m.C4 + (double) m.A1 * (double) m.B3 * (double) m.C4),
        C1 = (float) ((double) m.B2 * (double) m.C4 * (double) m.D1 - (double) m.B4 * (double) m.C2 * (double) m.D1 + (double) m.B4 * (double) m.C1 * (double) m.D2 - (double) m.B1 * (double) m.C4 * (double) m.D2 - (double) m.B2 * (double) m.C1 * (double) m.D4 + (double) m.B1 * (double) m.C2 * (double) m.D4),
        C2 = (float) ((double) m.A4 * (double) m.C2 * (double) m.D1 - (double) m.A2 * (double) m.C4 * (double) m.D1 - (double) m.A4 * (double) m.C1 * (double) m.D2 + (double) m.A1 * (double) m.C4 * (double) m.D2 + (double) m.A2 * (double) m.C1 * (double) m.D4 - (double) m.A1 * (double) m.C2 * (double) m.D4),
        C3 = (float) ((double) m.A2 * (double) m.B4 * (double) m.D1 - (double) m.A4 * (double) m.B2 * (double) m.D1 + (double) m.A4 * (double) m.B1 * (double) m.D2 - (double) m.A1 * (double) m.B4 * (double) m.D2 - (double) m.A2 * (double) m.B1 * (double) m.D4 + (double) m.A1 * (double) m.B2 * (double) m.D4),
        C4 = (float) ((double) m.A4 * (double) m.B2 * (double) m.C1 - (double) m.A2 * (double) m.B4 * (double) m.C1 - (double) m.A4 * (double) m.B1 * (double) m.C2 + (double) m.A1 * (double) m.B4 * (double) m.C2 + (double) m.A2 * (double) m.B1 * (double) m.C4 - (double) m.A1 * (double) m.B2 * (double) m.C4),
        D1 = (float) ((double) m.B3 * (double) m.C2 * (double) m.D1 - (double) m.B2 * (double) m.C3 * (double) m.D1 - (double) m.B3 * (double) m.C1 * (double) m.D2 + (double) m.B1 * (double) m.C3 * (double) m.D2 + (double) m.B2 * (double) m.C1 * (double) m.D3 - (double) m.B1 * (double) m.C2 * (double) m.D3),
        D2 = (float) ((double) m.A2 * (double) m.C3 * (double) m.D1 - (double) m.A3 * (double) m.C2 * (double) m.D1 + (double) m.A3 * (double) m.C1 * (double) m.D2 - (double) m.A1 * (double) m.C3 * (double) m.D2 - (double) m.A2 * (double) m.C1 * (double) m.D3 + (double) m.A1 * (double) m.C2 * (double) m.D3),
        D3 = (float) ((double) m.A3 * (double) m.B2 * (double) m.D1 - (double) m.A2 * (double) m.B3 * (double) m.D1 - (double) m.A3 * (double) m.B1 * (double) m.D2 + (double) m.A1 * (double) m.B3 * (double) m.D2 + (double) m.A2 * (double) m.B1 * (double) m.D3 - (double) m.A1 * (double) m.B2 * (double) m.D3),
        D4 = (float) ((double) m.A2 * (double) m.B3 * (double) m.C1 - (double) m.A3 * (double) m.B2 * (double) m.C1 + (double) m.A3 * (double) m.B1 * (double) m.C2 - (double) m.A1 * (double) m.B3 * (double) m.C2 - (double) m.A2 * (double) m.B1 * (double) m.C3 + (double) m.A1 * (double) m.B2 * (double) m.C3)
      } * (1f / m.Det());
    }

    public static void Decompose(
      Matrix4 mat,
      out Vector3 translation,
      out Vector3 scaling,
      out Matrix4 rotation)
    {
      translation = Vector3.Zero;
      scaling = Vector3.Zero;
      rotation = Matrix4.Identity;
      translation.X = mat.D1;
      translation.Y = mat.D2;
      translation.Z = mat.D3;
      Vector3[] vector3Array = new Vector3[3]
      {
        new Vector3(mat.A1, mat.A2, mat.A3),
        new Vector3(mat.B1, mat.B2, mat.B3),
        new Vector3(mat.C1, mat.C2, mat.C3)
      };
      scaling.X = vector3Array[0].Length();
      scaling.Y = vector3Array[1].Length();
      scaling.Z = vector3Array[2].Length();
      if ((double) scaling.X != 0.0)
      {
        vector3Array[0].X /= scaling.X;
        vector3Array[0].Y /= scaling.X;
        vector3Array[0].Z /= scaling.X;
      }
      if ((double) scaling.Y != 0.0)
      {
        vector3Array[1].X /= scaling.Y;
        vector3Array[1].Y /= scaling.Y;
        vector3Array[1].Z /= scaling.Y;
      }
      if ((double) scaling.Z != 0.0)
      {
        vector3Array[2].X /= scaling.Z;
        vector3Array[2].Y /= scaling.Z;
        vector3Array[2].Z /= scaling.Z;
      }
      rotation.A1 = vector3Array[0].X;
      rotation.B1 = vector3Array[0].Y;
      rotation.C1 = vector3Array[0].Z;
      rotation.A4 = 0.0f;
      rotation.D1 = 0.0f;
      rotation.A2 = vector3Array[1].X;
      rotation.B2 = vector3Array[1].Y;
      rotation.C2 = vector3Array[1].Z;
      rotation.B4 = 0.0f;
      rotation.D2 = 0.0f;
      rotation.A3 = vector3Array[2].X;
      rotation.B3 = vector3Array[2].Y;
      rotation.C3 = vector3Array[2].Z;
      rotation.C4 = 0.0f;
      rotation.D3 = 0.0f;
      rotation.D4 = 1f;
    }

    public static Matrix4 operator +(Matrix4 a, Matrix4 b)
    {
      return new Matrix4(a.A1 + b.A1, a.A2 + b.A2, a.A3 + b.A3, a.A4 + b.A4, a.B1 + b.B1, a.B2 + b.B2, a.B3 + b.B3, a.B4 + b.B4, a.C1 + b.C1, a.C2 + b.C2, a.C3 + b.C3, a.C4 + b.C4, a.D1 + b.D1, a.D2 + b.D2, a.D3 + b.D3, a.D4 + b.D4);
    }

    public float this[int column, int row]
    {
      get => this[column + row * 4];
      set => this[column + row * 4] = value;
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
            return this.A4;
          case 4:
            return this.B1;
          case 5:
            return this.B2;
          case 6:
            return this.B3;
          case 7:
            return this.B4;
          case 8:
            return this.C1;
          case 9:
            return this.C2;
          case 10:
            return this.C3;
          case 11:
            return this.C4;
          case 12:
            return this.D1;
          case 13:
            return this.D2;
          case 14:
            return this.D3;
          case 15:
            return this.D4;
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
            this.A4 = value;
            break;
          case 4:
            this.B1 = value;
            break;
          case 5:
            this.B2 = value;
            break;
          case 6:
            this.B3 = value;
            break;
          case 7:
            this.B4 = value;
            break;
          case 8:
            this.C1 = value;
            break;
          case 9:
            this.C2 = value;
            break;
          case 10:
            this.C3 = value;
            break;
          case 11:
            this.C4 = value;
            break;
          case 12:
            this.D1 = value;
            break;
          case 13:
            this.D2 = value;
            break;
          case 14:
            this.D3 = value;
            break;
          case 15:
            this.D4 = value;
            break;
          default:
            throw new IndexOutOfRangeException("Invalid matrix index!");
        }
      }
    }

    public static Matrix4 operator *(Matrix4 a, Matrix4 b)
    {
      return new Matrix4((float) ((double) a.A1 * (double) b.A1 + (double) a.A2 * (double) b.B1 + (double) a.A3 * (double) b.C1 + (double) a.A4 * (double) b.D1), (float) ((double) a.A1 * (double) b.A2 + (double) a.A2 * (double) b.B2 + (double) a.A3 * (double) b.C2 + (double) a.A4 * (double) b.D2), (float) ((double) a.A1 * (double) b.A3 + (double) a.A2 * (double) b.B3 + (double) a.A3 * (double) b.C3 + (double) a.A4 * (double) b.D3), (float) ((double) a.A1 * (double) b.A4 + (double) a.A2 * (double) b.B4 + (double) a.A3 * (double) b.C4 + (double) a.A4 * (double) b.D4), (float) ((double) a.B1 * (double) b.A1 + (double) a.B2 * (double) b.B1 + (double) a.B3 * (double) b.C1 + (double) a.B4 * (double) b.D1), (float) ((double) a.B1 * (double) b.A2 + (double) a.B2 * (double) b.B2 + (double) a.B3 * (double) b.C2 + (double) a.B4 * (double) b.D2), (float) ((double) a.B1 * (double) b.A3 + (double) a.B2 * (double) b.B3 + (double) a.B3 * (double) b.C3 + (double) a.B4 * (double) b.D3), (float) ((double) a.B1 * (double) b.A4 + (double) a.B2 * (double) b.B4 + (double) a.B3 * (double) b.C4 + (double) a.B4 * (double) b.D4), (float) ((double) a.C1 * (double) b.A1 + (double) a.C2 * (double) b.B1 + (double) a.C3 * (double) b.C1 + (double) a.C4 * (double) b.D1), (float) ((double) a.C1 * (double) b.A2 + (double) a.C2 * (double) b.B2 + (double) a.C3 * (double) b.C2 + (double) a.C4 * (double) b.D2), (float) ((double) a.C1 * (double) b.A3 + (double) a.C2 * (double) b.B3 + (double) a.C3 * (double) b.C3 + (double) a.C4 * (double) b.D3), (float) ((double) a.C1 * (double) b.A4 + (double) a.C2 * (double) b.B4 + (double) a.C3 * (double) b.C4 + (double) a.C4 * (double) b.D4), (float) ((double) a.D1 * (double) b.A1 + (double) a.D2 * (double) b.B1 + (double) a.D3 * (double) b.C1 + (double) a.D4 * (double) b.D1), (float) ((double) a.D1 * (double) b.A2 + (double) a.D2 * (double) b.B2 + (double) a.D3 * (double) b.C2 + (double) a.D4 * (double) b.D2), (float) ((double) a.D1 * (double) b.A3 + (double) a.D2 * (double) b.B3 + (double) a.D3 * (double) b.C3 + (double) a.D4 * (double) b.D3), (float) ((double) a.D1 * (double) b.A4 + (double) a.D2 * (double) b.B4 + (double) a.D3 * (double) b.C4 + (double) a.D4 * (double) b.D4));
    }

    public static Matrix4 operator *(Matrix4 source, float scalar)
    {
      return new Matrix4(source.A1 * scalar, source.A2 * scalar, source.A3 * scalar, source.A4 * scalar, source.B1 * scalar, source.B2 * scalar, source.B3 * scalar, source.B4 * scalar, source.C1 * scalar, source.C2 * scalar, source.C3 * scalar, source.C4 * scalar, source.D1 * scalar, source.D2 * scalar, source.D3 * scalar, source.D4 * scalar);
    }

    public static Matrix4 operator /(Matrix4 source, float scalar) => source * (1f / scalar);

    public static Matrix4 From(float[] values)
    {
      return new Matrix4(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8], values[9], values[10], values[11], values[12], values[13], values[14], values[15]);
    }

    public static unsafe Matrix4[] From(byte[] data, int offset, int count)
    {
      Matrix4[] matrix4Array = new Matrix4[count];
      fixed (byte* numPtr1 = &data[offset])
      {
        float* numPtr2 = (float*) numPtr1;
        for (int index = 0; index < count; ++index)
        {
          matrix4Array[index] = new Matrix4(*numPtr2, numPtr2[1], numPtr2[2], numPtr2[3], numPtr2[4], numPtr2[5], numPtr2[6], numPtr2[7], numPtr2[8], numPtr2[9], numPtr2[10], numPtr2[11], numPtr2[12], numPtr2[13], numPtr2[14], numPtr2[15]);
          numPtr2 += 16;
        }
      }
      return matrix4Array;
    }

    public static Matrix4 From(byte[] bytes) => Matrix4.From(bytes, 0);

    public static unsafe Matrix4 From(byte[] bytes, int offset)
    {
      fixed (byte* numPtr = &bytes[offset])
        return new Matrix4(*(float*) numPtr, ((float*) numPtr)[1], ((float*) numPtr)[2], ((float*) numPtr)[3], ((float*) numPtr)[4], ((float*) numPtr)[5], ((float*) numPtr)[6], ((float*) numPtr)[7], ((float*) numPtr)[8], ((float*) numPtr)[9], ((float*) numPtr)[10], ((float*) numPtr)[11], ((float*) numPtr)[12], ((float*) numPtr)[13], ((float*) numPtr)[14], ((float*) numPtr)[15]);
    }

    public static Matrix4 From(Matrix3 m)
    {
      return new Matrix4(m.A1, m.A2, m.A3, 0.0f, m.B1, m.B2, m.B3, 0.0f, m.C1, m.C2, m.C3, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public static bool operator ==(Matrix4 a, Matrix4 b) => a.Equals((object) b);

    public static bool operator !=(Matrix4 a, Matrix4 b) => !(a == b);

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      Matrix4 matrix4 = (Matrix4) obj;
      return (double) this.A1 == (double) matrix4.A1 && (double) this.A2 == (double) matrix4.A2 && (double) this.A3 == (double) matrix4.A3 && (double) this.A4 == (double) matrix4.A4 && (double) this.B1 == (double) matrix4.B1 && (double) this.B2 == (double) matrix4.B2 && (double) this.B3 == (double) matrix4.B3 && (double) this.B4 == (double) matrix4.B4 && (double) this.C1 == (double) matrix4.C1 && (double) this.C2 == (double) matrix4.C2 && (double) this.C3 == (double) matrix4.C3 && (double) this.C4 == (double) matrix4.C4 && (double) this.D1 == (double) matrix4.D1 && (double) this.D2 == (double) matrix4.D2 && (double) this.D3 == (double) matrix4.D3 && (double) this.D4 == (double) matrix4.D4;
    }

    public override int GetHashCode()
    {
      return this.A1.GetHashCode() ^ this.B2.GetHashCode() ^ this.C3.GetHashCode() ^ this.D4.GetHashCode();
    }
  }
}

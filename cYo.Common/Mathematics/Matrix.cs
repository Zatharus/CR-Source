// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Matrix
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;

#nullable disable
namespace cYo.Common.Mathematics
{
  public class Matrix
  {
    private readonly double[][] data;
    private readonly int rows;
    private readonly int columns;
    private static readonly System.Random random = new System.Random();

    public Matrix(int rows, int columns)
    {
      this.rows = rows;
      this.columns = columns;
      this.data = new double[rows][];
      for (int index = 0; index < rows; ++index)
        this.data[index] = new double[columns];
    }

    public Matrix(int rows, int columns, double value)
    {
      this.rows = rows;
      this.columns = columns;
      this.data = new double[rows][];
      for (int index = 0; index < rows; ++index)
        this.data[index] = new double[columns];
      for (int index = 0; index < rows; ++index)
        this.data[index][index] = value;
    }

    public Matrix(double[][] data)
    {
      this.rows = data != null ? data.Length : throw new ArgumentNullException();
      this.columns = data[0].Length;
      for (int index = 0; index < this.rows; ++index)
      {
        if (data[index].Length != this.columns)
          throw new ArgumentException();
      }
      this.data = data;
    }

    internal double[][] Array => this.data;

    public int Rows => this.rows;

    public int Columns => this.columns;

    public bool IsSquare => this.rows == this.columns;

    public bool IsSymmetric
    {
      get
      {
        if (!this.IsSquare)
          return false;
        for (int index1 = 0; index1 < this.rows; ++index1)
        {
          for (int index2 = 0; index2 <= index1; ++index2)
          {
            if (this.data[index1][index2] != this.data[index2][index1])
              return false;
          }
        }
        return true;
      }
    }

    public double this[int i, int j]
    {
      set => this.data[i][j] = value;
      get => this.data[i][j];
    }

    public Matrix Submatrix(int i0, int i1, int j0, int j1)
    {
      if (i0 > i1 || j0 > j1 || i0 < 0 || i0 >= this.rows || i1 < 0 || i1 >= this.rows || j0 < 0 || j0 >= this.columns || j1 < 0 || j1 >= this.columns)
        throw new ArgumentException();
      Matrix matrix = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
      double[][] array = matrix.Array;
      for (int index1 = i0; index1 <= i1; ++index1)
      {
        for (int index2 = j0; index2 <= j1; ++index2)
          array[index1 - i0][index2 - j0] = this.data[index1][index2];
      }
      return matrix;
    }

    public Matrix Submatrix(int[] r, int[] c)
    {
      if (r == null || c == null)
        throw new ArgumentNullException();
      Matrix matrix = new Matrix(r.Length, c.Length);
      double[][] array = matrix.Array;
      for (int index1 = 0; index1 < r.Length; ++index1)
      {
        for (int index2 = 0; index2 < c.Length; ++index2)
        {
          if (r[index1] < 0 || r[index1] >= this.rows || c[index2] < 0 || c[index2] >= this.columns)
            throw new ArgumentException();
          array[index1][index2] = this.data[r[index1]][c[index2]];
        }
      }
      return matrix;
    }

    public Matrix Submatrix(int i0, int i1, int[] c)
    {
      if (c == null)
        throw new ArgumentNullException();
      if (i0 > i1 || i0 < 0 || i0 >= this.rows || i1 < 0 || i1 >= this.rows)
        throw new ArgumentException();
      Matrix matrix = new Matrix(i1 - i0 + 1, c.Length);
      double[][] array = matrix.Array;
      for (int index1 = i0; index1 <= i1; ++index1)
      {
        for (int index2 = 0; index2 < c.Length; ++index2)
        {
          if (c[index2] < 0 || c[index2] >= this.columns)
            throw new ArgumentException();
          array[index1 - i0][index2] = this.data[index1][c[index2]];
        }
      }
      return matrix;
    }

    public Matrix Submatrix(int[] r, int j0, int j1)
    {
      if (r == null)
        throw new ArgumentNullException(nameof (r), "Array can not be null");
      if (j0 > j1 || j0 < 0 || j0 >= this.columns || j1 < 0 || j1 >= this.columns)
        throw new ArgumentException();
      Matrix matrix = new Matrix(r.Length, j1 - j0 + 1);
      double[][] array = matrix.Array;
      for (int index1 = 0; index1 < r.Length; ++index1)
      {
        for (int index2 = j0; index2 <= j1; ++index2)
        {
          if (r[index1] < 0 || r[index1] >= this.rows)
            throw new ArgumentException();
          array[index1][index2 - j0] = this.data[r[index1]][index2];
        }
      }
      return matrix;
    }

    public Matrix Clone()
    {
      Matrix matrix = new Matrix(this.rows, this.columns);
      double[][] array = matrix.Array;
      for (int index1 = 0; index1 < this.rows; ++index1)
      {
        for (int index2 = 0; index2 < this.columns; ++index2)
          array[index1][index2] = this.data[index1][index2];
      }
      return matrix;
    }

    public Matrix Transpose()
    {
      Matrix matrix = new Matrix(this.columns, this.rows);
      double[][] array = matrix.Array;
      for (int index1 = 0; index1 < this.rows; ++index1)
      {
        for (int index2 = 0; index2 < this.columns; ++index2)
          array[index2][index1] = this.data[index1][index2];
      }
      return matrix;
    }

    public double Norm1
    {
      get
      {
        double val1 = 0.0;
        for (int index1 = 0; index1 < this.columns; ++index1)
        {
          double val2 = 0.0;
          for (int index2 = 0; index2 < this.rows; ++index2)
            val2 += Math.Abs(this.data[index2][index1]);
          val1 = Math.Max(val1, val2);
        }
        return val1;
      }
    }

    public double InfinityNorm
    {
      get
      {
        double val1 = 0.0;
        for (int index1 = 0; index1 < this.rows; ++index1)
        {
          double val2 = 0.0;
          for (int index2 = 0; index2 < this.columns; ++index2)
            val2 += Math.Abs(this.data[index1][index2]);
          val1 = Math.Max(val1, val2);
        }
        return val1;
      }
    }

    public double FrobeniusNorm
    {
      get
      {
        double a = 0.0;
        for (int index1 = 0; index1 < this.rows; ++index1)
        {
          for (int index2 = 0; index2 < this.columns; ++index2)
            a = Matrix.Hypotenuse(a, this.data[index1][index2]);
        }
        return a;
      }
    }

    public static Matrix Negate(Matrix a)
    {
      int rows = a != null ? a.Rows : throw new ArgumentNullException();
      int columns = a.Columns;
      double[][] array1 = a.Array;
      Matrix matrix = new Matrix(rows, columns);
      double[][] array2 = matrix.Array;
      for (int index1 = 0; index1 < rows; ++index1)
      {
        for (int index2 = 0; index2 < columns; ++index2)
          array2[index1][index2] = -array1[index1][index2];
      }
      return matrix;
    }

    public static Matrix operator -(Matrix a) => Matrix.Negate(a);

    public static Matrix Add(Matrix a, Matrix b)
    {
      if (a == null)
        throw new ArgumentNullException("Matrix can not be null", nameof (a));
      if (b == null)
        throw new ArgumentNullException("Matrix can not be null", nameof (b));
      int rows = a.Rows;
      int columns = a.Columns;
      double[][] array1 = a.Array;
      if (rows != b.Rows || columns != b.Columns)
        throw new ArgumentException("Matrix dimension do not match.");
      Matrix matrix = new Matrix(rows, columns);
      double[][] array2 = matrix.Array;
      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < columns; ++j)
          array2[i][j] = array1[i][j] + b[i, j];
      }
      return matrix;
    }

    public static Matrix operator +(Matrix a, Matrix b) => Matrix.Add(a, b);

    public static Matrix Subtract(Matrix a, Matrix b)
    {
      int rows = a != null && b != null ? a.Rows : throw new ArgumentNullException();
      int columns = a.Columns;
      double[][] array1 = a.Array;
      if (rows != b.Rows || columns != b.Columns)
        throw new ArgumentException("Matrix dimension do not match.");
      Matrix matrix = new Matrix(rows, columns);
      double[][] array2 = matrix.Array;
      for (int i = 0; i < rows; ++i)
      {
        for (int j = 0; j < columns; ++j)
          array2[i][j] = array1[i][j] - b[i, j];
      }
      return matrix;
    }

    public static Matrix operator -(Matrix a, Matrix b) => Matrix.Subtract(a, b);

    public static Matrix Multiply(Matrix a, double s)
    {
      int rows = a != null ? a.Rows : throw new ArgumentNullException();
      int columns = a.Columns;
      double[][] array1 = a.Array;
      Matrix matrix = new Matrix(rows, columns);
      double[][] array2 = matrix.Array;
      for (int index1 = 0; index1 < rows; ++index1)
      {
        for (int index2 = 0; index2 < columns; ++index2)
          array2[index1][index2] = array1[index1][index2] * s;
      }
      return matrix;
    }

    public static Matrix operator *(Matrix a, double s)
    {
      return a != null ? Matrix.Multiply(a, s) : throw new ArgumentNullException();
    }

    public static Matrix Multiply(Matrix a, Matrix b)
    {
      int rows = a != null && b != null ? a.Rows : throw new ArgumentNullException();
      double[][] array1 = a.Array;
      if (b.Rows != a.columns)
        throw new ArgumentException("Matrix dimensions are not valid.");
      int columns1 = b.Columns;
      Matrix matrix = new Matrix(rows, columns1);
      double[][] array2 = matrix.Array;
      int columns2 = a.columns;
      double[] numArray1 = new double[columns2];
      for (int j = 0; j < columns1; ++j)
      {
        for (int i = 0; i < columns2; ++i)
          numArray1[i] = b[i, j];
        for (int index1 = 0; index1 < rows; ++index1)
        {
          double[] numArray2 = array1[index1];
          double num = 0.0;
          for (int index2 = 0; index2 < columns2; ++index2)
            num += numArray2[index2] * numArray1[index2];
          array2[index1][j] = num;
        }
      }
      return matrix;
    }

    public static Matrix operator *(Matrix a, Matrix b) => Matrix.Multiply(a, b);

    public Matrix Solve(Matrix rhs)
    {
      if (rhs == null)
        throw new ArgumentNullException();
      return this.rows != this.columns ? new QrDecomposition(this).Solve(rhs) : new LuDecomposition(this).Solve(rhs);
    }

    public Matrix Inverse => this.Solve(Matrix.Diagonal(this.rows, this.rows, 1.0));

    public double Determinant => new LuDecomposition(this).Determinant;

    public double Trace
    {
      get
      {
        double trace = 0.0;
        for (int index = 0; index < Math.Min(this.rows, this.columns); ++index)
          trace += this.data[index][index];
        return trace;
      }
    }

    public static Matrix Random(int rows, int columns)
    {
      Matrix matrix = new Matrix(rows, columns);
      double[][] array = matrix.Array;
      for (int index1 = 0; index1 < rows; ++index1)
      {
        for (int index2 = 0; index2 < columns; ++index2)
          array[index1][index2] = Matrix.random.NextDouble();
      }
      return matrix;
    }

    public static Matrix Diagonal(int rows, int columns, double value)
    {
      Matrix matrix = new Matrix(rows, columns);
      double[][] array = matrix.Array;
      for (int index1 = 0; index1 < rows; ++index1)
      {
        for (int index2 = 0; index2 < columns; ++index2)
          array[index1][index2] = index1 == index2 ? value : 0.0;
      }
      return matrix;
    }

    public string ToString(IFormatProvider provider)
    {
      using (StringWriter stringWriter = new StringWriter(provider))
      {
        for (int index1 = 0; index1 < this.rows; ++index1)
        {
          for (int index2 = 0; index2 < this.columns; ++index2)
            stringWriter.Write(this.data[index1][index2].ToString() + " ");
          stringWriter.WriteLine();
        }
        return stringWriter.ToString();
      }
    }

    public override string ToString() => this.ToString((IFormatProvider) null);

    private static double Hypotenuse(double a, double b)
    {
      if (Math.Abs(a) > Math.Abs(b))
      {
        double num = b / a;
        return Math.Abs(a) * Math.Sqrt(1.0 + num * num);
      }
      if (b == 0.0)
        return 0.0;
      double num1 = a / b;
      return Math.Abs(b) * Math.Sqrt(1.0 + num1 * num1);
    }
  }
}

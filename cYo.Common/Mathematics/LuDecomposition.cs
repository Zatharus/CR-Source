// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.LuDecomposition
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Mathematics
{
  public class LuDecomposition
  {
    private readonly Matrix LU;
    private readonly int pivotSign;
    private readonly int[] pivotVector;

    public LuDecomposition(Matrix A)
    {
      this.LU = A != null ? A.Clone() : throw new ArgumentNullException();
      double[][] array = this.LU.Array;
      int rows = A.Rows;
      int columns = A.Columns;
      this.pivotVector = new int[rows];
      for (int index = 0; index < rows; ++index)
        this.pivotVector[index] = index;
      this.pivotSign = 1;
      double[] numArray1 = new double[rows];
      for (int val2 = 0; val2 < columns; ++val2)
      {
        for (int index = 0; index < rows; ++index)
          numArray1[index] = array[index][val2];
        for (int val1 = 0; val1 < rows; ++val1)
        {
          double[] numArray2 = array[val1];
          int num1 = Math.Min(val1, val2);
          double num2 = 0.0;
          for (int index = 0; index < num1; ++index)
            num2 += numArray2[index] * numArray1[index];
          numArray2[val2] = (numArray1[val1] -= num2);
        }
        int index1 = val2;
        for (int index2 = val2 + 1; index2 < rows; ++index2)
        {
          if (Math.Abs(numArray1[index2]) > Math.Abs(numArray1[index1]))
            index1 = index2;
        }
        if (index1 != val2)
        {
          for (int index3 = 0; index3 < columns; ++index3)
          {
            double num = array[index1][index3];
            array[index1][index3] = array[val2][index3];
            array[val2][index3] = num;
          }
          int num3 = this.pivotVector[index1];
          this.pivotVector[index1] = this.pivotVector[val2];
          this.pivotVector[val2] = num3;
          this.pivotSign = -this.pivotSign;
        }
        if (val2 < rows & array[val2][val2] != 0.0)
        {
          for (int index4 = val2 + 1; index4 < rows; ++index4)
            array[index4][val2] /= array[val2][val2];
        }
      }
    }

    public bool IsNonsingular
    {
      get
      {
        for (int index = 0; index < this.LU.Columns; ++index)
        {
          if (this.LU[index, index] == 0.0)
            return false;
        }
        return true;
      }
    }

    public double Determinant
    {
      get
      {
        if (this.LU.Rows != this.LU.Columns)
          throw new ArgumentException("Matrix must be square.");
        double pivotSign = (double) this.pivotSign;
        for (int index = 0; index < this.LU.Columns; ++index)
          pivotSign *= this.LU[index, index];
        return pivotSign;
      }
    }

    public Matrix LowerTriangularFactor
    {
      get
      {
        int rows = this.LU.Rows;
        int columns = this.LU.Columns;
        Matrix triangularFactor = new Matrix(rows, columns);
        for (int i = 0; i < rows; ++i)
        {
          for (int j = 0; j < columns; ++j)
            triangularFactor[i, j] = i > j ? this.LU[i, j] : (i == j ? 1.0 : 0.0);
        }
        return triangularFactor;
      }
    }

    public Matrix UpperTriangularFactor
    {
      get
      {
        int rows = this.LU.Rows;
        int columns = this.LU.Columns;
        Matrix triangularFactor = new Matrix(rows, columns);
        for (int i = 0; i < rows; ++i)
        {
          for (int j = 0; j < columns; ++j)
            triangularFactor[i, j] = i <= j ? this.LU[i, j] : 0.0;
        }
        return triangularFactor;
      }
    }

    public double[] CreatePivotPermutationVector()
    {
      int rows = this.LU.Rows;
      double[] permutationVector = new double[rows];
      for (int index = 0; index < rows; ++index)
        permutationVector[index] = (double) this.pivotVector[index];
      return permutationVector;
    }

    public Matrix Solve(Matrix B)
    {
      if (B == null)
        throw new ArgumentNullException();
      if (B.Rows != this.LU.Rows)
        throw new ArgumentException("Invalid matrix dimensions.");
      if (!this.IsNonsingular)
        throw new InvalidOperationException("Matrix is singular");
      int columns1 = B.Columns;
      Matrix matrix = B.Submatrix(this.pivotVector, 0, columns1 - 1);
      int columns2 = this.LU.Columns;
      double[][] array = this.LU.Array;
      for (int i1 = 0; i1 < columns2; ++i1)
      {
        for (int i2 = i1 + 1; i2 < columns2; ++i2)
        {
          for (int j = 0; j < columns1; ++j)
            matrix[i2, j] -= matrix[i1, j] * array[i2][i1];
        }
      }
      for (int i3 = columns2 - 1; i3 >= 0; --i3)
      {
        for (int j = 0; j < columns1; ++j)
          matrix[i3, j] /= array[i3][i3];
        for (int i4 = 0; i4 < i3; ++i4)
        {
          for (int j = 0; j < columns1; ++j)
            matrix[i4, j] -= matrix[i3, j] * array[i4][i3];
        }
      }
      return matrix;
    }
  }
}

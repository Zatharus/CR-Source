// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.QrDecomposition
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Mathematics
{
  public class QrDecomposition
  {
    private readonly Matrix QR;
    private readonly double[] Rdiag;

    public QrDecomposition(Matrix A)
    {
      this.QR = A != null ? A.Clone() : throw new ArgumentNullException(nameof (A), "Matrix can not be null");
      double[][] array = this.QR.Array;
      int rows = A.Rows;
      int columns = A.Columns;
      this.Rdiag = new double[columns];
      for (int index1 = 0; index1 < columns; ++index1)
      {
        double a = 0.0;
        for (int index2 = index1; index2 < rows; ++index2)
          a = QrDecomposition.Hypotenuse(a, array[index2][index1]);
        if (a != 0.0)
        {
          if (array[index1][index1] < 0.0)
            a = -a;
          for (int index3 = index1; index3 < rows; ++index3)
            array[index3][index1] /= a;
          ++array[index1][index1];
          for (int index4 = index1 + 1; index4 < columns; ++index4)
          {
            double num1 = 0.0;
            for (int index5 = index1; index5 < rows; ++index5)
              num1 += array[index5][index1] * array[index5][index4];
            double num2 = -num1 / array[index1][index1];
            for (int index6 = index1; index6 < rows; ++index6)
              array[index6][index4] += num2 * array[index6][index1];
          }
        }
        this.Rdiag[index1] = -a;
      }
    }

    public Matrix Solve(Matrix rightHandSideMatrix)
    {
      if (rightHandSideMatrix == null)
        throw new ArgumentNullException(nameof (rightHandSideMatrix), "Matrix must not be null");
      if (rightHandSideMatrix.Rows != this.QR.Rows)
        throw new ArgumentException("Matrix row dimensions must agree.");
      if (!this.IsFullRank)
        throw new InvalidOperationException("Matrix is rank deficient.");
      int columns1 = rightHandSideMatrix.Columns;
      Matrix matrix = rightHandSideMatrix.Clone();
      int rows = this.QR.Rows;
      int columns2 = this.QR.Columns;
      double[][] array = this.QR.Array;
      for (int index = 0; index < columns2; ++index)
      {
        for (int j = 0; j < columns1; ++j)
        {
          double num1 = 0.0;
          for (int i = index; i < rows; ++i)
            num1 += array[i][index] * matrix[i, j];
          double num2 = -num1 / array[index][index];
          for (int i = index; i < rows; ++i)
            matrix[i, j] += num2 * array[i][index];
        }
      }
      for (int i1 = columns2 - 1; i1 >= 0; --i1)
      {
        for (int j = 0; j < columns1; ++j)
          matrix[i1, j] /= this.Rdiag[i1];
        for (int i2 = 0; i2 < i1; ++i2)
        {
          for (int j = 0; j < columns1; ++j)
            matrix[i2, j] -= matrix[i1, j] * array[i2][i1];
        }
      }
      return matrix.Submatrix(0, columns2 - 1, 0, columns1 - 1);
    }

    public bool IsFullRank
    {
      get
      {
        int columns = this.QR.Columns;
        for (int index = 0; index < columns; ++index)
        {
          if (this.Rdiag[index] == 0.0)
            return false;
        }
        return true;
      }
    }

    public Matrix UpperTriangularFactor
    {
      get
      {
        int columns = this.QR.Columns;
        Matrix triangularFactor = new Matrix(columns, columns);
        double[][] array1 = triangularFactor.Array;
        double[][] array2 = this.QR.Array;
        for (int index1 = 0; index1 < columns; ++index1)
        {
          for (int index2 = 0; index2 < columns; ++index2)
            array1[index1][index2] = index1 < index2 ? array2[index1][index2] : (index1 == index2 ? this.Rdiag[index1] : 0.0);
        }
        return triangularFactor;
      }
    }

    public Matrix OrthogonalFactor
    {
      get
      {
        Matrix orthogonalFactor = new Matrix(this.QR.Rows, this.QR.Columns);
        double[][] array1 = orthogonalFactor.Array;
        double[][] array2 = this.QR.Array;
        for (int index1 = this.QR.Columns - 1; index1 >= 0; --index1)
        {
          for (int index2 = 0; index2 < this.QR.Rows; ++index2)
            array1[index2][index1] = 0.0;
          array1[index1][index1] = 1.0;
          for (int index3 = index1; index3 < this.QR.Columns; ++index3)
          {
            if (array2[index1][index1] != 0.0)
            {
              double num1 = 0.0;
              for (int index4 = index1; index4 < this.QR.Rows; ++index4)
                num1 += array2[index4][index1] * array1[index4][index3];
              double num2 = -num1 / array2[index1][index1];
              for (int index5 = index1; index5 < this.QR.Rows; ++index5)
                array1[index5][index3] += num2 * array2[index5][index1];
            }
          }
        }
        return orthogonalFactor;
      }
    }

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

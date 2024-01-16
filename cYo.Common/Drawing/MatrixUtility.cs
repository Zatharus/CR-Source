// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.MatrixUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Drawing;
using System.Drawing.Drawing2D;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class MatrixUtility
  {
    public static Matrix GetRotationMatrix(Point anchor, int pageRotation)
    {
      Matrix rotationMatrix = new Matrix();
      if (pageRotation % 360 == 0)
        return rotationMatrix;
      rotationMatrix.Translate((float) -anchor.X, (float) -anchor.Y, MatrixOrder.Append);
      rotationMatrix.Rotate((float) pageRotation, MatrixOrder.Append);
      rotationMatrix.Translate((float) anchor.X, (float) anchor.Y, MatrixOrder.Append);
      return rotationMatrix;
    }

    public static Matrix GetRotationMatrix(Size page, int pageRotation)
    {
      return MatrixUtility.GetRotationMatrix(new Point(page.Width / 2, page.Height / 2), pageRotation);
    }
  }
}

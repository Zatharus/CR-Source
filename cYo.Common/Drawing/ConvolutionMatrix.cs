// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ConvolutionMatrix
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

#nullable disable
namespace cYo.Common.Drawing
{
  public struct ConvolutionMatrix
  {
    private int topLeft;
    private int topMid;
    private int topRight;
    private int midLeft;
    private int pixel;
    private int midRight;
    private int bottomLeft;
    private int bottomMid;
    private int bottomRight;
    private int divisor;
    private int offset;

    public ConvolutionMatrix(int setToAll)
    {
      this.divisor = 1;
      this.offset = 0;
      this.pixel = setToAll;
      this.topLeft = this.topMid = this.topRight = this.midLeft = this.midRight = this.bottomLeft = this.bottomMid = this.bottomRight = setToAll;
    }

    public int TopLeft
    {
      get => this.topLeft;
      set => this.topLeft = value;
    }

    public int TopMid
    {
      get => this.topMid;
      set => this.topMid = value;
    }

    public int TopRight
    {
      get => this.topRight;
      set => this.topRight = value;
    }

    public int MidLeft
    {
      get => this.midLeft;
      set => this.midLeft = value;
    }

    public int Pixel
    {
      get => this.pixel;
      set => this.pixel = value;
    }

    public int MidRight
    {
      get => this.midRight;
      set => this.midRight = value;
    }

    public int BottomLeft
    {
      get => this.bottomLeft;
      set => this.bottomLeft = value;
    }

    public int BottomMid
    {
      get => this.bottomMid;
      set => this.bottomMid = value;
    }

    public int BottomRight
    {
      get => this.bottomRight;
      set => this.bottomRight = value;
    }

    public int Divisor
    {
      get => this.divisor;
      set => this.divisor = value;
    }

    public int Offset
    {
      get => this.offset;
      set => this.offset = value;
    }

    public void SetAll(int value)
    {
      this.TopLeft = this.TopMid = this.TopRight = this.MidLeft = this.Pixel = this.MidRight = this.BottomLeft = this.BottomMid = this.BottomRight = value;
    }

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      ConvolutionMatrix convolutionMatrix = (ConvolutionMatrix) obj;
      return convolutionMatrix.topLeft == this.topLeft && convolutionMatrix.topMid == this.topMid && convolutionMatrix.topRight == this.topRight && convolutionMatrix.midLeft == this.midLeft && convolutionMatrix.midRight == this.midRight && convolutionMatrix.bottomLeft == this.bottomLeft && convolutionMatrix.bottomMid == this.bottomMid && convolutionMatrix.bottomRight == this.bottomRight;
    }

    public override int GetHashCode()
    {
      return this.TopLeft.GetHashCode() ^ this.topRight.GetHashCode() ^ this.bottomLeft.GetHashCode() ^ this.bottomRight.GetHashCode();
    }

    public static bool operator ==(ConvolutionMatrix a, ConvolutionMatrix b)
    {
      return a.Equals((object) b);
    }

    public static bool operator !=(ConvolutionMatrix a, ConvolutionMatrix b) => !(a == b);
  }
}

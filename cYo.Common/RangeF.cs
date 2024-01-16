// Decompiled with JetBrains decompiler
// Type: cYo.Common.RangeF
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

#nullable disable
namespace cYo.Common
{
  public struct RangeF
  {
    public float Start;
    public float Length;
    public static readonly RangeF Empty;

    public RangeF(float start = 0.0f, float length = 0.0f)
    {
      this.Start = start;
      this.Length = length;
    }

    public float End => this.Start + this.Length;

    public bool IsEmpty => (double) this.Start == 0.0 && (double) this.Length == 0.0;
  }
}

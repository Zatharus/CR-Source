// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Format.Alignment
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

#nullable disable
namespace cYo.Common.Presentation.Ceco.Format
{
  public class Alignment : Span
  {
    public Alignment(HorizontalAlignment lineAlign) => this.Align = lineAlign;

    public Alignment(HorizontalAlignment lineAlign, params Inline[] inlines)
      : base(inlines)
    {
      this.Align = lineAlign;
    }
  }
}

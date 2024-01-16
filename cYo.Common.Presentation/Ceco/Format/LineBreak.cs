// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Format.LineBreak
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

#nullable disable
namespace cYo.Common.Presentation.Ceco.Format
{
  public class LineBreak : Span
  {
    private float lineScale = 1f;

    public LineBreak()
    {
    }

    public LineBreak(float lineScale) => this.lineScale = lineScale;

    public float LineScale
    {
      get => this.lineScale;
      set => this.lineScale = value;
    }

    public bool Clear { get; set; }

    public override int FlowBreakOffset
    {
      get => (int) ((double) this.Font.Height * (double) this.lineScale);
    }

    public override FlowBreak FlowBreak => FlowBreak.After;
  }
}

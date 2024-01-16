// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Format.Italic
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco.Format
{
  public class Italic : Span
  {
    public Italic()
    {
    }

    public Italic(params Inline[] inlines)
      : base(inlines)
    {
    }

    public override FontStyle FontStyle
    {
      get => base.FontStyle | FontStyle.Italic;
      set => base.FontStyle = value;
    }
  }
}

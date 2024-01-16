// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Format.TextFont
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco.Format
{
  public class TextFont : Span
  {
    public TextFont()
    {
    }

    public TextFont(
      string face,
      float scale,
      FontSize fontSize,
      Color color,
      params Inline[] inlines)
      : base(inlines)
    {
      this.FontScale = scale;
      this.FontSize = fontSize;
      this.FontFamily = face;
      this.ForeColor = color;
    }

    public TextFont(int size, params Inline[] inlines)
      : this((string) null, 1f, new FontSize(size, false), Color.Empty, inlines)
    {
    }

    public TextFont(int size, FontStyle fontStyle, params Inline[] inlines)
      : this((string) null, 1f, new FontSize(size, false), Color.Empty, inlines)
    {
      this.FontStyle = fontStyle;
    }

    public TextFont(FontSize fontSize, params Inline[] inlines)
      : this((string) null, 1f, fontSize, Color.Empty, inlines)
    {
    }

    public TextFont(float scale, BaseAlignment align, params Inline[] inlines)
      : this((string) null, scale, new FontSize(0, true), Color.Empty, inlines)
    {
      this.BaseAlign = align;
    }
  }
}

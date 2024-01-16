// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.TextLine
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing
{
  public class TextLine : DisposableObject
  {
    public TextLine(
      string text,
      Font font,
      Color foreColor,
      StringFormat format,
      int beforeSpacing = 0,
      int afterSpacing = 0)
    {
      this.Text = text;
      this.Font = font;
      this.ForeColor = foreColor;
      this.BeforeSpacing = beforeSpacing;
      this.AfterSpacing = afterSpacing;
      this.Format = format;
    }

    public TextLine(
      string text,
      Font font,
      Color foreColor,
      StringFormatFlags options = (StringFormatFlags) 0,
      StringAlignment alignment = StringAlignment.Near,
      int beforeSpacing = 0,
      int afterSpacing = 0)
      : this(text, font, foreColor, TextLine.CreateStringFormat(alignment, options), beforeSpacing, afterSpacing)
    {
    }

    public TextLine(int spacing)
    {
      this.Separator = true;
      this.BeforeSpacing = spacing;
      this.Format = new StringFormat();
    }

    public bool Separator { get; set; }

    public string Text { get; set; }

    public Color ForeColor { get; set; }

    public Font Font { get; set; }

    public int BeforeSpacing { get; set; }

    public int AfterSpacing { get; set; }

    public StringFormat Format { get; set; }

    public bool FontOwned { get; set; }

    public bool ScrollStart { get; set; }

    private static StringFormat CreateStringFormat(StringAlignment align, StringFormatFlags options)
    {
      StringFormat stringFormat = new StringFormat(options)
      {
        Alignment = align
      };
      if ((options & StringFormatFlags.NoWrap) != (StringFormatFlags) 0)
        stringFormat.Trimming = StringTrimming.EllipsisCharacter;
      return stringFormat;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.Format != null)
          this.Format.Dispose();
        if (this.FontOwned && this.Font != null && !this.Font.IsSystemFont)
          this.Font.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}

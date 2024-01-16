// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.BodyBlock
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public class BodyBlock : FlowBlock, IResources
  {
    private readonly Dictionary<BodyBlock.FontKey, Font> fontCache = new Dictionary<BodyBlock.FontKey, Font>();

    private Font GetCachedFont(BodyBlock.FontKey fontKey)
    {
      Font font;
      return this.fontCache.TryGetValue(fontKey, out font) ? font : (this.fontCache[fontKey] = new Font(fontKey.FontFamily, fontKey.FontSize, fontKey.FontStyle));
    }

    public override Font GetFont(string fontFamily, float fontSize, FontStyle fontStyle)
    {
      return this.GetCachedFont(new BodyBlock.FontKey(fontFamily, fontSize, fontStyle));
    }

    public override Image GetImage(string source) => Image.FromFile(source);

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (Font font in this.fontCache.Values)
          font.Dispose();
      }
      base.Dispose(disposing);
    }

    private class FontKey
    {
      public readonly string FontFamily;
      public readonly float FontSize;
      public readonly FontStyle FontStyle;

      public FontKey(string fontFamily, float fontSize, FontStyle fontStyle)
      {
        if (string.IsNullOrEmpty(fontFamily))
          fontFamily = "Arial";
        if ((double) fontSize == 0.0)
          fontSize = 8f;
        this.FontFamily = fontFamily;
        this.FontSize = fontSize;
        this.FontStyle = fontStyle;
      }

      public override bool Equals(object obj)
      {
        return obj is BodyBlock.FontKey fontKey && fontKey.FontFamily == this.FontFamily && (double) fontKey.FontSize == (double) this.FontSize && fontKey.FontStyle == this.FontStyle;
      }

      public override int GetHashCode()
      {
        return this.FontFamily.GetHashCode() ^ this.FontSize.GetHashCode() ^ this.FontStyle.GetHashCode();
      }
    }
  }
}

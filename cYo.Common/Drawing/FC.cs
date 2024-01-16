// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.FC
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing
{
  public static class FC
  {
    public static bool SystemFallBack = true;
    private const int MaxSize = 100;
    private static readonly Dictionary<FC.FontKey, FC.FontItem> fontCache = new Dictionary<FC.FontKey, FC.FontItem>();
    private static readonly LinkedList<FC.FontKey> fontKeyList = new LinkedList<FC.FontKey>();

    public static Font Get(string fontFamily, float fontSize, FontStyle fontStyle)
    {
      FC.FontKey key1 = new FC.FontKey(fontFamily, fontSize, fontStyle);
      FC.FontItem fontItem;
      if (FC.fontCache.TryGetValue(key1, out fontItem))
      {
        if (fontItem.Node != FC.fontKeyList.First)
        {
          FC.fontKeyList.Remove(fontItem.Node);
          FC.fontKeyList.AddFirst(fontItem.Node);
        }
      }
      else
      {
        Font font;
        try
        {
          font = new Font(fontFamily, fontSize, fontStyle);
        }
        catch
        {
          if (!FC.SystemFallBack)
            throw;
          else
            font = SystemFonts.DefaultFont;
        }
        fontItem = new FC.FontItem(font, FC.fontKeyList.AddFirst(key1));
        FC.fontCache.Add(key1, fontItem);
      }
      while (FC.fontKeyList.Count > 100)
      {
        FC.FontKey key2 = FC.fontKeyList.Last.Value;
        FC.fontKeyList.RemoveLast();
        Font font = FC.fontCache[key2].Font;
        if (!font.IsSystemFont)
          font.Dispose();
        FC.fontCache.Remove(key2);
      }
      return fontItem.Font;
    }

    public static Font Get(string fontFamily, float fontSize)
    {
      return FC.Get(fontFamily, fontSize, FontStyle.Regular);
    }

    public static Font Get(Font font, float fontSize, FontStyle fontStyle)
    {
      return FC.Get(font.FontFamily.Name, fontSize, fontStyle);
    }

    public static Font Get(Font font, FontStyle fontStyle) => FC.Get(font, font.Size, fontStyle);

    public static Font Get(Font font, float fontSize) => FC.Get(font, fontSize, font.Style);

    public static Font GetRelative(Font font, float fontSize, FontStyle fontStyle)
    {
      return FC.Get(font, fontSize * font.Size, fontStyle);
    }

    public static Font GetRelative(Font font, float fontSize) => FC.Get(font, fontSize * font.Size);

    private struct FontKey
    {
      private readonly string fontFamily;
      private readonly FontStyle fontStyle;
      private readonly float fontSize;
      private readonly int hashCode;

      public FontKey(string fontFamily, float fontSize, FontStyle fontStyle)
      {
        this.fontFamily = fontFamily;
        this.fontStyle = fontStyle;
        this.fontSize = fontSize;
        this.hashCode = fontFamily.GetHashCode() ^ fontStyle.GetHashCode() ^ fontSize.GetHashCode();
      }

      public string FontFamily => this.fontFamily;

      public FontStyle FontStyle => this.fontStyle;

      public float FontSize => this.fontSize;

      public override bool Equals(object obj)
      {
        FC.FontKey fontKey = (FC.FontKey) obj;
        return this.FontFamily == fontKey.FontFamily && this.FontStyle == fontKey.FontStyle && (double) this.FontSize == (double) fontKey.FontSize;
      }

      public override int GetHashCode() => this.hashCode;
    }

    private struct FontItem
    {
      private readonly LinkedListNode<FC.FontKey> node;
      private readonly Font font;

      public FontItem(Font font, LinkedListNode<FC.FontKey> node)
      {
        this.font = font;
        this.node = node;
      }

      public LinkedListNode<FC.FontKey> Node => this.node;

      public Font Font => this.font;
    }
  }
}

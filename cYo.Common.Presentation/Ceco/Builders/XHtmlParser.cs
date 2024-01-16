// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Builders.XHtmlParser
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Presentation.Ceco.Format;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

#nullable disable
namespace cYo.Common.Presentation.Ceco.Builders
{
  public static class XHtmlParser
  {
    public static FlowBlock Parse(string text)
    {
      return XHtmlParser.Parse((TextReader) new StringReader("<content>" + text + "</content>"));
    }

    public static FlowBlock Parse(TextReader reader)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        IgnoreComments = true,
        IgnoreWhitespace = true
      };
      return XHtmlParser.Parse(XmlReader.Create(reader, settings));
    }

    public static FlowBlock Parse(XmlReader reader)
    {
      FlowBlock flowBlock = new FlowBlock();
      XHtmlParser.Parse((Span) flowBlock, reader);
      return flowBlock;
    }

    private static void DefaultAttributes(Inline inline, IDictionary<string, string> attributes)
    {
      if (attributes == null)
        return;
      try
      {
        if (attributes.ContainsKey("align"))
          inline.Align = (HorizontalAlignment) Enum.Parse(typeof (HorizontalAlignment), attributes["align"], true);
      }
      catch
      {
      }
      try
      {
        if (attributes.ContainsKey("color"))
          inline.ForeColor = ColorTranslator.FromHtml(attributes["color"]);
      }
      catch
      {
      }
      try
      {
        if (!attributes.ContainsKey("bgcolor"))
          return;
        inline.BackColor = ColorTranslator.FromHtml(attributes["bgcolor"]);
      }
      catch
      {
      }
    }

    private static void Parse(Span span, XmlReader reader)
    {
      while (reader.Read())
      {
        Span span1 = (Span) null;
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:
            Dictionary<string, string> attributes = (Dictionary<string, string>) null;
            string lower = reader.Name.ToLower();
            if (reader.HasAttributes)
            {
              while (reader.MoveToNextAttribute())
              {
                if (attributes == null)
                  attributes = new Dictionary<string, string>();
                attributes[reader.Name.ToLower()] = reader.Value;
              }
              reader.MoveToElement();
            }
            switch (lower)
            {
              case "a":
                span1 = (Span) new Anchor();
                if (attributes != null && attributes.ContainsKey("href"))
                {
                  ((Anchor) span1).HRef = attributes["href"];
                  break;
                }
                break;
              case "b":
              case "strong":
                span1 = (Span) new Bold();
                break;
              case "big":
                span1 = (Span) new TextFont(new FontSize(1, true), new Inline[0]);
                break;
              case "br":
                span1 = (Span) new LineBreak(0.0f);
                if (attributes != null && attributes.ContainsKey("clear"))
                {
                  ((LineBreak) span1).Clear = true;
                  break;
                }
                break;
              case "center":
                span1 = (Span) new Alignment(HorizontalAlignment.Center);
                break;
              case "cite":
              case "em":
              case "i":
              case "samp":
                span1 = (Span) new Italic();
                break;
              case "code":
              case "kbd":
              case "tt":
                TextFont textFont1 = new TextFont();
                textFont1.FontFamily = "Courier New";
                span1 = (Span) textFont1;
                break;
              case "content":
                span1 = (Span) new TextFont(new FontSize(3, false), new Inline[0]);
                break;
              case "font":
                if (attributes != null)
                {
                  TextFont textFont2 = new TextFont();
                  span1 = (Span) textFont2;
                  if (attributes.ContainsKey("size"))
                  {
                    int size = int.Parse(attributes["size"]);
                    textFont2.FontSize = new FontSize(size, attributes["size"].StartsWith("+") || size < 0);
                  }
                  if (attributes.ContainsKey("face"))
                  {
                    textFont2.FontFamily = attributes["face"];
                    break;
                  }
                  break;
                }
                break;
              case "h1":
                span1 = (Span) new TextFont(6, FontStyle.Bold, new Inline[0]);
                break;
              case "h2":
                span1 = (Span) new TextFont(5, FontStyle.Bold, new Inline[0]);
                break;
              case "h3":
                span1 = (Span) new TextFont(4, FontStyle.Bold, new Inline[0]);
                break;
              case "h4":
                span1 = (Span) new TextFont(3, FontStyle.Bold, new Inline[0]);
                break;
              case "h5":
                span1 = (Span) new TextFont(2, FontStyle.Bold, new Inline[0]);
                break;
              case "h6":
                span1 = (Span) new TextFont(1, FontStyle.Bold, new Inline[0]);
                break;
              case "hr":
                HorizontalRule horizontalRule = new HorizontalRule();
                span1 = (Span) horizontalRule;
                if (attributes != null)
                {
                  if (attributes.ContainsKey("noshade"))
                    horizontalRule.Noshade = bool.Parse(attributes["noshade"]);
                  if (attributes.ContainsKey("size"))
                  {
                    horizontalRule.Thickness = int.Parse(attributes["size"]);
                    break;
                  }
                  break;
                }
                break;
              case "img":
                if (attributes != null)
                {
                  ImageItem imageItem1 = new ImageItem();
                  span.Inlines.Add((Inline) imageItem1);
                  if (attributes.ContainsKey("src"))
                    imageItem1.Source = attributes["src"];
                  if (attributes.ContainsKey("width"))
                    imageItem1.BlockWidth = new SizeValue(attributes["width"]);
                  if (attributes.ContainsKey("height"))
                    imageItem1.BlockHeight = int.Parse(attributes["height"]);
                  Size padding;
                  if (attributes.ContainsKey("hspace"))
                  {
                    ImageItem imageItem2 = imageItem1;
                    int width = int.Parse(attributes["hspace"]);
                    padding = imageItem1.Padding;
                    int height = padding.Height;
                    Size size = new Size(width, height);
                    imageItem2.Padding = size;
                  }
                  if (attributes.ContainsKey("vspace"))
                  {
                    ImageItem imageItem3 = imageItem1;
                    padding = imageItem1.Padding;
                    Size size = new Size(padding.Width, int.Parse(attributes["vspace"]));
                    imageItem3.Padding = size;
                  }
                  try
                  {
                    if (attributes.ContainsKey("align"))
                      imageItem1.VAlign = (VerticalAlignment) Enum.Parse(typeof (VerticalAlignment), attributes["align"], true);
                  }
                  catch
                  {
                  }
                  XHtmlParser.DefaultAttributes((Inline) imageItem1, (IDictionary<string, string>) attributes);
                  break;
                }
                break;
              case "p":
                span1 = (Span) new LineBreak(1f);
                break;
              case "pre":
                TextFont textFont3 = new TextFont();
                textFont3.FontFamily = "Courier New";
                TextFont textFont4 = textFont3;
                string str1 = reader.ReadString().Replace("\r\n", "\n");
                textFont4.Inlines.Add((Inline) new LineBreak());
                string str2 = str1;
                char[] chArray = new char[1]{ '\n' };
                foreach (string text in str2.Split(chArray))
                {
                  textFont4.Inlines.Add((Inline) new TextRun(text));
                  textFont4.Inlines.Add((Inline) new LineBreak(0.0f));
                }
                textFont4.Inlines.Add((Inline) new LineBreak(0.0f));
                span.Inlines.Add((Inline) textFont4);
                break;
              case "small":
                span1 = (Span) new TextFont(new FontSize(-1, true), new Inline[0]);
                break;
              case nameof (span):
                span1 = new Span();
                break;
              case "strike":
                span1 = (Span) new Strike();
                break;
              case "sub":
                span1 = (Span) new TextFont(0.7f, BaseAlignment.Bottom, new Inline[0]);
                break;
              case "sup":
                span1 = (Span) new TextFont(0.7f, BaseAlignment.Top, new Inline[0]);
                break;
              case "table":
                span1 = (Span) new Table();
                if (attributes != null)
                {
                  Table table = (Table) span1;
                  if (attributes.ContainsKey("width"))
                    table.BlockWidth = new SizeValue(attributes["width"]);
                  if (attributes.ContainsKey("border"))
                    table.Border = int.Parse(attributes["border"]);
                  if (attributes.ContainsKey("cellpadding"))
                    table.CellPadding = int.Parse(attributes["cellpadding"]);
                  if (attributes.ContainsKey("cellspacing"))
                    table.CellSpacing = int.Parse(attributes["cellspacing"]);
                  if (attributes.ContainsKey("valign"))
                  {
                    table.VAlign = (VerticalAlignment) Enum.Parse(typeof (VerticalAlignment), attributes["valign"], true);
                    break;
                  }
                  break;
                }
                break;
              case "td":
              case "th":
                span1 = (Span) new Table.Cell();
                if (attributes != null)
                {
                  Table.Cell cell = (Table.Cell) span1;
                  if (attributes.ContainsKey("width"))
                    cell.BlockWidth = new SizeValue(attributes["width"]);
                  if (attributes.ContainsKey("height"))
                    cell.BlockHeight = int.Parse(attributes["height"]);
                  if (attributes.ContainsKey("valign"))
                    cell.VAlign = (VerticalAlignment) Enum.Parse(typeof (VerticalAlignment), attributes["valign"], true);
                  if (attributes.ContainsKey("colspan"))
                    cell.ColumSpan = int.Parse(attributes["colspan"]);
                  if (attributes.ContainsKey("rowspan"))
                  {
                    cell.RowSpan = int.Parse(attributes["rowspan"]);
                    break;
                  }
                  break;
                }
                break;
              case "tr":
                span1 = (Span) new Table.Row();
                if (attributes != null && attributes.ContainsKey("valign"))
                {
                  ((Block) span1).VAlign = (VerticalAlignment) Enum.Parse(typeof (VerticalAlignment), attributes["valign"], true);
                  break;
                }
                break;
              case "u":
                span1 = (Span) new Underline();
                break;
            }
            if (span1 != null)
            {
              XHtmlParser.DefaultAttributes((Inline) span1, (IDictionary<string, string>) attributes);
              span.Inlines.Add((Inline) span1);
            }
            if (!reader.IsEmptyElement)
            {
              if (span1 == null)
              {
                XHtmlParser.Parse(span, reader);
                continue;
              }
              XHtmlParser.Parse(span1, reader);
              continue;
            }
            continue;
          case XmlNodeType.Text:
            span.Inlines.Add((Inline) new TextRun(reader.Value.Replace("\n", " ")));
            continue;
          case XmlNodeType.EndElement:
            return;
          default:
            continue;
        }
      }
    }
  }
}

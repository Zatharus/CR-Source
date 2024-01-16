// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ColorSchema
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Drawing
{
  public class ColorSchema : IXmlSerializable
  {
    private readonly Dictionary<string, Color> table = new Dictionary<string, Color>();
    private string name;

    public ColorSchema(string name) => this.name = name;

    public ColorSchema()
      : this(string.Empty)
    {
    }

    public string Name
    {
      get => this.name;
      set => this.name = value;
    }

    public Color this[string name]
    {
      get
      {
        Color color;
        return !this.table.TryGetValue(name, out color) ? Color.Empty : color;
      }
      set => this.table[name] = value;
    }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader)
    {
      ColorConverter colorConverter = new ColorConverter();
      this.name = reader.GetAttribute("name");
      reader.ReadStartElement();
      while (reader.IsStartElement())
      {
        string name = reader.Name;
        string text = reader.ReadElementContentAsString();
        try
        {
          this.table[name] = (Color) colorConverter.ConvertFromInvariantString(text);
        }
        catch (Exception ex)
        {
          this.table[name] = Color.Red;
        }
      }
      reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
      ColorConverter colorConverter = new ColorConverter();
      if (!string.IsNullOrEmpty(this.name))
        writer.WriteAttributeString("name", this.name);
      foreach (string key in this.table.Keys)
        writer.WriteElementString(key, colorConverter.ConvertToInvariantString((object) this.table[key]));
    }
  }
}

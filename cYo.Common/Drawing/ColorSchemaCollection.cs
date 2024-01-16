// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ColorSchemaCollection
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Drawing
{
  public class ColorSchemaCollection : List<ColorSchema>
  {
    public ColorSchema this[string name]
    {
      get => this.Find((Predicate<ColorSchema>) (item => item.Name == name));
    }

    public static ColorSchemaCollection Load(string file)
    {
      try
      {
        using (StreamReader streamReader = File.OpenText(file))
          return (ColorSchemaCollection) XmlUtility.GetSerializer<ColorSchemaCollection>().Deserialize((TextReader) streamReader);
      }
      catch (Exception ex)
      {
        return new ColorSchemaCollection();
      }
    }

    public bool Save(string file)
    {
      try
      {
        using (StreamWriter text = File.CreateText(file))
          new XmlSerializer(typeof (ColorSchemaCollection)).Serialize((TextWriter) text, (object) this);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Localize.TREntry
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Localize
{
  public class TREntry : IComparable<TREntry>
  {
    public TREntry()
    {
    }

    public TREntry(string key, string text, string comment)
    {
      this.Key = key;
      this.Text = text;
      this.Comment = comment;
    }

    [XmlAttribute]
    public virtual string Key { get; set; }

    [XmlAttribute]
    public virtual string Text { get; set; }

    [XmlAttribute]
    public virtual string Comment { get; set; }

    [XmlIgnore]
    public virtual TR Resource { get; internal set; }

    public virtual string ResourceName => this.Resource != null ? this.Resource.Name : string.Empty;

    public virtual int CompareTo(TREntry other)
    {
      return string.Compare(this.Key, other.Key, StringComparison.Ordinal);
    }
  }
}

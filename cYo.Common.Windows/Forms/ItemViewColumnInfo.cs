// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemViewColumnInfo
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [Serializable]
  public class ItemViewColumnInfo
  {
    private bool visible = true;
    private int width = 80;
    private readonly string name;
    [NonSerialized]
    private readonly object tag;
    private DateTime lastTimeVisible = DateTime.MinValue;

    public ItemViewColumnInfo()
    {
    }

    public ItemViewColumnInfo(IColumn header)
    {
      this.Id = header.Id;
      this.FormatId = header.FormatId;
      this.Visible = header.Visible;
      this.Width = header.Width;
      this.name = header.Text;
      this.tag = header.Tag;
      this.lastTimeVisible = header.LastTimeVisible;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    public int Id { get; set; }

    [XmlAttribute]
    [DefaultValue(0)]
    public int FormatId { get; set; }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool Visible
    {
      get => this.visible;
      set => this.visible = value;
    }

    [XmlAttribute]
    [DefaultValue(80)]
    public int Width
    {
      get => this.width;
      set => this.width = value;
    }

    public string Name => this.name;

    public object Tag => this.tag;

    [DefaultValue(typeof (DateTime), "0001-01-01T00:00:00")]
    public DateTime LastTimeVisible
    {
      get => this.lastTimeVisible;
      set => this.lastTimeVisible = value;
    }

    public override string ToString()
    {
      return !string.IsNullOrEmpty(this.name) ? this.name : string.Empty;
    }
  }
}

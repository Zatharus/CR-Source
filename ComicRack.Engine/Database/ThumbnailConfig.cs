// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ThumbnailConfig
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Projects.ComicRack.Engine.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class ThumbnailConfig : ICloneable
  {
    private readonly List<int> captionIds = new List<int>();

    public ThumbnailConfig() => this.TextElements = ComicTextElements.DefaultFileComic;

    public ThumbnailConfig(ThumbnailConfig cfg)
    {
      this.CaptionIds.AddRange((IEnumerable<int>) cfg.CaptionIds);
      this.HideCaptions = cfg.HideCaptions;
      this.TextElements = cfg.TextElements;
    }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool HideCaptions { get; set; }

    [XmlArray("Lines")]
    [XmlArrayItem("Id")]
    public List<int> CaptionIds => this.captionIds;

    [DefaultValue(ComicTextElements.DefaultFileComic)]
    public ComicTextElements TextElements { get; set; }

    public object Clone() => (object) new ThumbnailConfig(this);
  }
}

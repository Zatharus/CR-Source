// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.DisplayListConfig
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Windows.Forms;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class DisplayListConfig
  {
    public DisplayListConfig()
    {
    }

    public DisplayListConfig(
      ItemViewConfig view,
      ThumbnailConfig thumbnail,
      TileConfig tile,
      StacksConfig stackConfig,
      string background)
    {
      this.View = view;
      this.Thumbnail = thumbnail;
      this.Tile = tile;
      this.StackConfig = stackConfig;
      this.BackgroundImageSource = background;
    }

    [DefaultValue(null)]
    public ItemViewConfig View { get; set; }

    [DefaultValue(null)]
    public ThumbnailConfig Thumbnail { get; set; }

    [DefaultValue(null)]
    public TileConfig Tile { get; set; }

    [DefaultValue(null)]
    public StacksConfig StackConfig { get; set; }

    [DefaultValue(null)]
    public string BackgroundImageSource { get; set; }

    [DefaultValue(ComicBookAllPropertiesMatcher.ShowOptionType.All)]
    public ComicBookAllPropertiesMatcher.ShowOptionType ShowOptionType { get; set; }

    [DefaultValue(ComicBookAllPropertiesMatcher.ShowComicType.All)]
    public ComicBookAllPropertiesMatcher.ShowComicType ShowComicType { get; set; }

    [DefaultValue(false)]
    public bool ShowOnlyDuplicates { get; set; }

    [DefaultValue(false)]
    public bool ShowGroupHeaders { get; set; }

    [DefaultValue(0)]
    public int ShowGroupHeadersWidth { get; set; }

    [DefaultValue(null)]
    public string QuickSearch { get; set; }

    [DefaultValue(ComicBookAllPropertiesMatcher.MatcherOption.All)]
    public ComicBookAllPropertiesMatcher.MatcherOption QuickSearchType { get; set; }

    [XmlIgnore]
    public System.Drawing.Point ScrollPosition { get; set; }

    [XmlIgnore]
    public Guid FocusedComicId { get; set; }

    [XmlIgnore]
    public System.Drawing.Point StackScrollPosition { get; set; }

    [XmlIgnore]
    public Guid StackFocusedComicId { get; set; }

    [XmlIgnore]
    public Guid StackedComicId { get; set; }
  }
}

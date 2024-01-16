// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemViewConfig
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [Serializable]
  public class ItemViewConfig
  {
    private List<ItemViewColumnInfo> columns = new List<ItemViewColumnInfo>();
    private ItemViewMode itemViewMode = ItemViewMode.Detail;
    private SortOrder itemSortOrder = SortOrder.Ascending;
    private SortOrder groupSortOrder = SortOrder.Ascending;

    [XmlArrayItem("Column")]
    public List<ItemViewColumnInfo> Columns
    {
      get => this.columns;
      set => this.columns = value;
    }

    [XmlAttribute]
    [DefaultValue(ItemViewMode.Detail)]
    public ItemViewMode ItemViewMode
    {
      get => this.itemViewMode;
      set => this.itemViewMode = value;
    }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool Grouping { get; set; }

    [XmlAttribute]
    [DefaultValue(null)]
    public string SortKey { get; set; }

    [XmlAttribute]
    [DefaultValue(null)]
    public string GrouperId { get; set; }

    [XmlAttribute]
    [DefaultValue(null)]
    public string StackerId { get; set; }

    [XmlAttribute]
    [DefaultValue(SortOrder.Ascending)]
    public SortOrder ItemSortOrder
    {
      get => this.itemSortOrder;
      set => this.itemSortOrder = value;
    }

    [XmlAttribute]
    [DefaultValue(SortOrder.Ascending)]
    public SortOrder GroupSortOrder
    {
      get => this.groupSortOrder;
      set => this.groupSortOrder = value;
    }

    [DefaultValue(null)]
    public ItemViewGroupsStatus GroupsStatus { get; set; }

    public System.Drawing.Size ThumbnailSize { get; set; }

    public System.Drawing.Size TileSize { get; set; }

    public int ItemRowHeight { get; set; }
  }
}

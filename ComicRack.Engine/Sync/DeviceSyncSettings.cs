// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.DeviceSyncSettings
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  [Serializable]
  public class DeviceSyncSettings
  {
    public const string ClipboardFormat = "DeviceSyncSettings";

    public DeviceSyncSettings()
    {
    }

    public DeviceSyncSettings(DeviceSyncSettings sds)
    {
      this.DeviceName = sds.DeviceName;
      this.DeviceKey = sds.DeviceKey;
      this.Lists.AddRange(sds.Lists.Select<DeviceSyncSettings.SharedList, DeviceSyncSettings.SharedList>((Func<DeviceSyncSettings.SharedList, DeviceSyncSettings.SharedList>) (sl => new DeviceSyncSettings.SharedList(sl))));
    }

    [XmlAttribute("Name")]
    public string DeviceName { get; set; }

    [XmlAttribute("Key")]
    public string DeviceKey { get; set; }

    public SmartList<DeviceSyncSettings.SharedList> Lists { get; } = new SmartList<DeviceSyncSettings.SharedList>();

    public DeviceSyncSettings.SharedListSettings DefaultListSettings { get; set; } = new DeviceSyncSettings.SharedListSettings()
    {
      OnlyUnread = true
    };

    public override int GetHashCode() => this.DeviceKey.GetHashCode();

    public override bool Equals(object obj)
    {
      return obj is DeviceSyncSettings deviceSyncSettings && deviceSyncSettings.DeviceKey == this.DeviceKey;
    }

    [Serializable]
    public enum LimitType
    {
      Books,
      MB,
      GB,
    }

    [Serializable]
    public enum ListSort
    {
      Random,
      Series,
      AlternateSeries,
      Published,
      Added,
      StoryArc,
    }

    [Serializable]
    public class SharedListSettings
    {
      public SharedListSettings()
      {
        this.LimitValue = 50;
        this.LimitValueType = DeviceSyncSettings.LimitType.Books;
        this.ListSortType = DeviceSyncSettings.ListSort.Series;
        this.Sort = true;
      }

      public SharedListSettings(DeviceSyncSettings.SharedListSettings other)
      {
        this.OptimizePortable = other.OptimizePortable;
        this.OnlyUnread = other.OnlyUnread;
        this.OnlyChecked = other.OnlyChecked;
        this.KeepLastRead = other.KeepLastRead;
        this.Limit = other.Limit;
        this.Sort = other.Sort;
        this.LimitValue = other.LimitValue;
        this.LimitValueType = other.LimitValueType;
        this.ListSortType = other.ListSortType;
      }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool OptimizePortable { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool OnlyUnread { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool KeepLastRead { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool OnlyChecked { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool Limit { get; set; }

      [XmlAttribute]
      [DefaultValue(true)]
      public bool Sort { get; set; }

      [XmlAttribute]
      [DefaultValue(50)]
      public int LimitValue { get; set; }

      [XmlAttribute]
      [DefaultValue(DeviceSyncSettings.LimitType.Books)]
      public DeviceSyncSettings.LimitType LimitValueType { get; set; }

      [XmlAttribute]
      [DefaultValue(DeviceSyncSettings.ListSort.Series)]
      public DeviceSyncSettings.ListSort ListSortType { get; set; }
    }

    [Serializable]
    public class SharedList : DeviceSyncSettings.SharedListSettings
    {
      public SharedList()
      {
      }

      public SharedList(DeviceSyncSettings.SharedList other)
        : base((DeviceSyncSettings.SharedListSettings) other)
      {
        this.ListId = other.ListId;
      }

      public SharedList(Guid listId, DeviceSyncSettings.SharedListSettings other)
        : base(other)
      {
        this.ListId = listId;
      }

      [XmlAttribute]
      public Guid ListId { get; set; }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ShareableComicListItem
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public abstract class ShareableComicListItem : ComicListItem, ICloneable
  {
    public const string ClipboardFormat = "ComicList";
    private bool quickOpen;

    [XmlAttribute]
    [DefaultValue(false)]
    public virtual bool QuickOpen
    {
      get => this.quickOpen;
      set
      {
        if (this.quickOpen == value)
          return;
        this.quickOpen = value;
        this.OnChanged(ComicListItemChange.Other);
      }
    }

    public abstract object Clone();
  }
}

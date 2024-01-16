// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.RemoteShareItem
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Projects.ComicRack.Engine.IO.Network;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  public class RemoteShareItem
  {
    public RemoteShareItem()
    {
    }

    public RemoteShareItem(ShareInformation si)
    {
      this.Name = si.Name;
      this.Uri = si.Uri;
    }

    public RemoteShareItem(string name) => this.Uri = this.Name = name;

    [DefaultValue(null)]
    [XmlAttribute]
    public string Name { get; set; }

    [DefaultValue(null)]
    [XmlAttribute]
    public string Uri { get; set; }

    public override string ToString() => !string.IsNullOrEmpty(this.Name) ? this.Name : this.Uri;

    public override bool Equals(object obj)
    {
      return obj is RemoteShareItem remoteShareItem && remoteShareItem.Name == this.Name && remoteShareItem.Uri == this.Uri;
    }

    public override int GetHashCode()
    {
      return (this.Name ?? string.Empty).GetHashCode() ^ (this.Uri ?? string.Empty).GetHashCode();
    }
  }
}

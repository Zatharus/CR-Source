// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.ComicLibraryServerConfig
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  [Serializable]
  public class ComicLibraryServerConfig
  {
    public const int DefaultPrivateServicePort = 7612;
    public const int DefaultPublicServicePort = 7612;
    public const string DefaultServiceName = "Share";
    private SmartList<Guid> sharedItems = new SmartList<Guid>();

    public ComicLibraryServerConfig()
    {
      this.ServicePort = 7612;
      this.LibraryShareMode = LibraryShareMode.All;
      this.Name = string.Empty;
      this.Description = string.Empty;
      this.Password = string.Empty;
      this.PageQuality = 100;
      this.ThumbnailQuality = 100;
      this.Options = ServerOptions.None;
    }

    [DefaultValue(LibraryShareMode.All)]
    public LibraryShareMode LibraryShareMode { get; set; }

    [DefaultValue("")]
    public string Name { get; set; }

    [DefaultValue("")]
    public string Description { get; set; }

    [DefaultValue("")]
    public string Password { get; set; }

    [DefaultValue(ServerOptions.None)]
    public ServerOptions Options { get; set; }

    [DefaultValue(false)]
    public bool IsInternet { get; set; }

    [DefaultValue(false)]
    public bool IsPrivate { get; set; }

    [DefaultValue(100)]
    public int PageQuality { get; set; }

    [DefaultValue(100)]
    public int ThumbnailQuality { get; set; }

    public SmartList<Guid> SharedItems => this.sharedItems;

    [XmlIgnore]
    [DefaultValue(7612)]
    public int ServicePort { get; set; }

    [XmlIgnore]
    [DefaultValue("Share")]
    public string ServiceName { get; set; }

    [XmlIgnore]
    [DefaultValue(false)]
    public bool OnlyPrivateConnections { get; set; }

    [XmlIgnore]
    [DefaultValue("")]
    public string PrivateListPassword { get; set; }

    public bool IsValidShare
    {
      get
      {
        if (this.LibraryShareMode == LibraryShareMode.None || string.IsNullOrEmpty(this.Name))
          return false;
        return !string.IsNullOrEmpty(this.PrivateListPassword) || !this.IsInternet || !this.IsPrivate;
      }
    }

    public string ProtectionPassword => !this.IsProtected ? string.Empty : this.Password;

    public bool IsProtected
    {
      get
      {
        return this.Options.IsSet<ServerOptions>(ServerOptions.ShareNeedsPassword) && !string.IsNullOrEmpty(this.Password);
      }
      set
      {
        this.Options = this.Options.SetMask<ServerOptions>(ServerOptions.ShareNeedsPassword, value);
      }
    }

    public bool IsEditable
    {
      get => this.Options.IsSet<ServerOptions>(ServerOptions.ShareIsEditable);
      set
      {
        this.Options = this.Options.SetMask<ServerOptions>(ServerOptions.ShareIsEditable, value);
      }
    }

    public bool IsExportable
    {
      get => this.Options.IsSet<ServerOptions>(ServerOptions.ShareIsExportable);
      set
      {
        this.Options = this.Options.SetMask<ServerOptions>(ServerOptions.ShareIsExportable, value);
      }
    }

    public class EqualityComparer : IEqualityComparer<ComicLibraryServerConfig>
    {
      public bool Equals(ComicLibraryServerConfig x, ComicLibraryServerConfig y)
      {
        return x.Name == y.Name && x.Password == y.Password && x.Options == y.Options && x.Description == y.Description && x.IsInternet == y.IsInternet && x.IsPrivate == y.IsPrivate && x.LibraryShareMode == y.LibraryShareMode && x.SharedItems.SequenceEqual<Guid>((IEnumerable<Guid>) y.SharedItems) && x.ThumbnailQuality == y.ThumbnailQuality && x.PageQuality == y.PageQuality;
      }

      public int GetHashCode(ComicLibraryServerConfig obj) => obj.GetHashCode();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.WatchFolder
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class WatchFolder : DisposableObject
  {
    private string folder;
    private bool watch;
    [NonSerialized]
    private FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

    public WatchFolder()
    {
    }

    public WatchFolder(string path, bool watch)
    {
      this.Folder = path;
      this.watch = watch;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => this.UpdateWatcher();

    [XmlAttribute]
    public string Folder
    {
      get => this.folder;
      set
      {
        if (this.folder == value)
          return;
        this.folder = value != null ? value : throw new ArgumentNullException();
        this.UpdateWatcher(this.watch);
      }
    }

    [DefaultValue(false)]
    [XmlAttribute]
    public bool Watch
    {
      get => this.watch;
      set
      {
        if (this.watch == value)
          return;
        this.watch = value;
        this.UpdateWatcher(this.watch);
      }
    }

    public FileSystemWatcher FileSystemWatcher => this.fileSystemWatcher;

    private void UpdateWatcher(bool watch)
    {
      try
      {
        this.fileSystemWatcher.EnableRaisingEvents = watch;
        this.fileSystemWatcher.IncludeSubdirectories = true;
        this.fileSystemWatcher.Path = this.folder;
      }
      catch (Exception ex)
      {
        this.fileSystemWatcher.EnableRaisingEvents = false;
      }
    }

    private void UpdateWatcher() => this.UpdateWatcher(this.watch);

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.UpdateWatcher(false);
        this.fileSystemWatcher.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}

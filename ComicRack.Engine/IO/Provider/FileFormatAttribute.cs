// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.FileFormatAttribute
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class FileFormatAttribute : Attribute
  {
    public FileFormatAttribute(FileFormat format) => this.Format = format;

    public FileFormatAttribute(string name, int id, string extensions)
      : this(new FileFormat(name, id, extensions))
    {
    }

    public FileFormat Format { get; set; }

    public bool Dynamic
    {
      get => this.Format.Dynamic;
      set => this.Format.Dynamic = value;
    }

    public bool EnableUpdate
    {
      get => this.Format.SupportsUpdate;
      set => this.Format.SupportsUpdate = value;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.IStorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public interface IStorageProvider
  {
    ComicInfo Store(
      IImageProvider provider,
      ComicInfo info,
      string target,
      StorageSetting setting);

    string DefaultExtension { get; }

    FileFormat DefaultFileFormat { get; }

    int FormatId { get; }

    event EventHandler<StorageProgressEventArgs> Progress;
  }
}

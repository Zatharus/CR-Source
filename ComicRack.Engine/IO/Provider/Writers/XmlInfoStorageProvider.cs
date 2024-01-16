// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Writers.XmlInfoStorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Writers
{
  [FileFormat("Book Information", 4, ".xml")]
  internal class XmlInfoStorageProvider : StorageProvider
  {
    protected override ComicInfo OnStore(
      IImageProvider provider,
      ComicInfo info,
      string target,
      StorageSetting setting)
    {
      if (info == null)
        return info;
      try
      {
        using (StreamWriter text = File.CreateText(target))
          info.Serialize(text.BaseStream);
        return info;
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}

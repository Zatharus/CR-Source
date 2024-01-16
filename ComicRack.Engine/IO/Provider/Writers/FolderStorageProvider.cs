// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Writers.FolderStorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Writers
{
  [FileFormat("Image Folder", 100, ".")]
  public class FolderStorageProvider : PackedStorageProvider
  {
    private string target;

    protected override void OnCreateFile(string target, StorageSetting setting)
    {
      this.target = target;
      Directory.CreateDirectory(target);
    }

    protected override void AddEntry(string name, byte[] data)
    {
      File.WriteAllBytes(Path.Combine(this.target, name), data);
    }

    protected override void OnCloseFile()
    {
    }
  }
}

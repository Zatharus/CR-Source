// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Cb7StorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Win32;
using cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive;
using cYo.Projects.ComicRack.Engine.IO.Provider.Writers;
using System;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  [FileFormat("eComic (7z)", 6, ".cb7")]
  public class Cb7StorageProvider : PackedStorageProvider
  {
    private string file;

    protected override void OnCreateFile(string target, StorageSetting setting)
    {
      this.file = target;
      File.Delete(this.file);
    }

    protected override void OnCloseFile()
    {
    }

    protected override void AddEntry(string name, byte[] data)
    {
      string parameters = string.Format("a -t7z \"-si{0}\" \"{1}\"", (object) name, (object) this.file);
      try
      {
        if (ExecuteProcess.Execute(SevenZipEngine.PackExe, parameters, data, (string) null, ExecuteProcess.Options.None).ExitCode != 0)
          throw new IOException();
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}

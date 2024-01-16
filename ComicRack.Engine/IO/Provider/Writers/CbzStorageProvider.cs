// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Writers.CbzStorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using ICSharpCode.SharpZipLib.Zip;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Writers
{
  [FileFormat("eComic (ZIP)", 2, ".cbz")]
  public class CbzStorageProvider : PackedStorageProvider
  {
    private const bool Zip64 = false;
    private Stream file;
    private ZipOutputStream zos;

    protected override void OnCreateFile(string target, StorageSetting setting)
    {
      this.file = (Stream) File.Create(target, 100000);
      this.zos = new ZipOutputStream(this.file);
      this.zos.UseZip64 = UseZip64.Off;
      switch (setting.ComicCompression)
      {
        case ExportCompression.Medium:
          this.zos.SetLevel(5);
          break;
        case ExportCompression.Strong:
          this.zos.SetLevel(9);
          break;
        default:
          this.zos.SetLevel(0);
          break;
      }
    }

    protected override void OnCloseFile()
    {
      this.zos.Close();
      this.file.Close();
    }

    protected override void AddEntry(string name, byte[] data)
    {
      this.zos.PutNextEntry(new ZipEntry(name));
      this.zos.Write(data, 0, data.Length);
      this.zos.CloseEntry();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Writers.CbtStorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using ICSharpCode.SharpZipLib.Tar;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Writers
{
  [FileFormat("eComic (TAR)", 5, ".cbt")]
  public class CbtStorageProvider : PackedStorageProvider
  {
    private const int BufferSize = 131072;
    private Stream file;
    private TarOutputStream tos;

    protected override void OnCreateFile(string target, StorageSetting setting)
    {
      this.file = (Stream) File.Create(target, 131072);
      this.tos = new TarOutputStream(this.file);
    }

    protected override void OnCloseFile()
    {
      this.tos.Close();
      this.file.Close();
    }

    protected override void AddEntry(string name, byte[] data)
    {
      this.tos.PutNextEntry(new TarEntry(new TarHeader()
      {
        Name = name,
        UserName = "ComicRack",
        UserId = 666,
        Size = (long) data.Length
      }));
      this.tos.Write(data, 0, data.Length);
      this.tos.CloseEntry();
    }
  }
}

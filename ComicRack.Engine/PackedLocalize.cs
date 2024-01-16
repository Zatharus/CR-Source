// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.PackedLocalize
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class PackedLocalize : IVirtualFolder
  {
    private readonly IVirtualFolder loc;

    public PackedLocalize(IVirtualFolder loc) => this.loc = loc;

    public Stream OpenRead(string path)
    {
      try
      {
        string path1 = Path.GetDirectoryName(path) + ".zip";
        string fileName = Path.GetFileName(path);
        if (this.loc.FileExists(path1))
        {
          using (Stream stream = this.loc.OpenRead(path1))
          {
            using (ZipFile zipFile = new ZipFile(stream))
            {
              ZipEntry entry = zipFile.GetEntry(fileName);
              if (entry != null)
              {
                using (Stream inputStream = zipFile.GetInputStream(entry))
                {
                  byte[] buffer = new byte[entry.Size];
                  inputStream.Read(buffer, 0, buffer.Length);
                  return (Stream) new MemoryStream(buffer);
                }
              }
            }
          }
        }
      }
      catch
      {
      }
      return this.loc.OpenRead(path);
    }

    public Stream Create(string path) => this.loc.Create(path);

    public bool FileExists(string path)
    {
      string path1 = Path.GetDirectoryName(path) + ".zip";
      string fileName = Path.GetFileName(path);
      if (!this.loc.FileExists(path1))
        return this.loc.FileExists(path);
      using (Stream stream = this.loc.OpenRead(path1))
      {
        using (ZipFile zipFile = new ZipFile(stream))
          return zipFile.GetEntry(fileName) != null;
      }
    }

    public bool CreateFolder(string path) => this.loc.CreateFolder(path);

    public IEnumerable<string> GetFiles(string path)
    {
      try
      {
        string path1 = path + ".zip";
        if (this.loc.FileExists(path1))
        {
          List<string> files = new List<string>();
          using (Stream stream = this.loc.OpenRead(path1))
          {
            using (ZipFile zipFile = new ZipFile(stream))
            {
              foreach (ZipEntry zipEntry in zipFile)
                files.Add(Path.Combine(path, zipEntry.Name));
            }
          }
          return (IEnumerable<string>) files;
        }
      }
      catch
      {
      }
      return this.loc.GetFiles(path);
    }
  }
}

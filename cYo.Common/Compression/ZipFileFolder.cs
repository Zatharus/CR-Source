// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.ZipFileFolder
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using cYo.Common.IO;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Common.Compression
{
  public class ZipFileFolder : DisposableObject, IVirtualFolder
  {
    private ZipFile zipFile;

    public ZipFileFolder(Stream zipStream) => this.zipFile = new ZipFile(zipStream);

    public ZipFileFolder(string zipFile) => this.zipFile = new ZipFile(zipFile);

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.zipFile.Close();
      base.Dispose(disposing);
    }

    public Stream OpenRead(string path)
    {
      ZipEntry entry = this.zipFile.GetEntry(path);
      if (entry == null)
        return (Stream) null;
      using (Stream inputStream = this.zipFile.GetInputStream(entry))
      {
        byte[] buffer = new byte[entry.Size];
        inputStream.Read(buffer, 0, buffer.Length);
        return (Stream) new MemoryStream(buffer);
      }
    }

    public bool FileExists(string path) => this.zipFile.GetEntry(path) != null;

    public IEnumerable<string> GetFiles(string path)
    {
      return this.zipFile.OfType<ZipEntry>().Where<ZipEntry>((Func<ZipEntry, bool>) (ze => ze.Name.StartsWith(path))).Select<ZipEntry, string>((Func<ZipEntry, string>) (ze => ze.Name));
    }

    public Stream Create(string path) => throw new NotImplementedException();

    public bool CreateFolder(string path) => throw new NotImplementedException();

    public static ZipFileFolder CreateFromFile(string file)
    {
      try
      {
        return new ZipFileFolder(file);
      }
      catch
      {
        return (ZipFileFolder) null;
      }
    }

    public static IEnumerable<ZipFileFolder> CreateFromFiles(
      IEnumerable<string> folders,
      string searchPattern)
    {
      return (IEnumerable<ZipFileFolder>) folders.SelectMany<string, ZipFileFolder>((Func<string, IEnumerable<ZipFileFolder>>) (folder => FileUtility.SafeGetFiles(folder, searchPattern).OrderBy<string, string>((Func<string, string>) (f => f)).Select<string, ZipFileFolder>(new Func<string, ZipFileFolder>(ZipFileFolder.CreateFromFile)))).ToArray<ZipFileFolder>();
    }
  }
}

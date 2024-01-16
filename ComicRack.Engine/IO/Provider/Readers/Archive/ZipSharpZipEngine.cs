// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive.ZipSharpZipEngine
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive
{
  public class ZipSharpZipEngine : FileBasedAccessor
  {
    public const int BufferSize = 131072;

    public ZipSharpZipEngine()
      : base(2)
    {
    }

    public override IEnumerable<ProviderImageInfo> GetEntryList(string source)
    {
      using (FileStream fs = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, 131072))
      {
        using (ZipFile zf = new ZipFile(fs))
        {
          foreach (ZipEntry zipEntry in zf)
            yield return new ProviderImageInfo((int) zipEntry.ZipFileIndex, zipEntry.Name, zipEntry.Size);
        }
      }
    }

    public override byte[] ReadByteImage(string source, ProviderImageInfo info)
    {
      try
      {
        using (FileStream file = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, 131072))
        {
          using (ZipFile zipFile = new ZipFile(file))
          {
            ZipEntry entry = zipFile.GetEntry(info.Name);
            using (Stream inputStream = zipFile.GetInputStream(entry))
            {
              byte[] buffer = new byte[(int) entry.Size];
              if (inputStream.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new IOException();
              return buffer;
            }
          }
        }
      }
      catch (Exception ex)
      {
        return (byte[]) null;
      }
    }

    public override ComicInfo ReadInfo(string source)
    {
      try
      {
        using (FileStream file = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, 131072))
        {
          using (ZipFile zipFile = new ZipFile(file))
          {
            int entry = zipFile.FindEntry("ComicInfo.xml", true);
            if (entry != -1)
            {
              using (Stream inputStream = zipFile.GetInputStream((long) entry))
                return ComicInfo.Deserialize(inputStream);
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return (ComicInfo) null;
    }
  }
}

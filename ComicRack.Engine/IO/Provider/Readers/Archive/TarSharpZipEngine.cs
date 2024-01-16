// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive.TarSharpZipEngine
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using ICSharpCode.SharpZipLib.Tar;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive
{
  public class TarSharpZipEngine : FileBasedAccessor
  {
    public const int BufferSize = 131072;

    public TarSharpZipEngine()
      : base(5)
    {
    }

    public override bool IsFormat(string source)
    {
      try
      {
        using (FileStream inputStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, 131072))
        {
          using (TarInputStream tarInputStream = new TarInputStream((Stream) inputStream))
            return tarInputStream.GetNextEntry() != null;
        }
      }
      catch
      {
        return false;
      }
    }

    public override IEnumerable<ProviderImageInfo> GetEntryList(string source)
    {
      using (FileStream fs = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, 131072))
      {
        TarInputStream tis = new TarInputStream((Stream) fs);
        try
        {
          while (true)
          {
            TarEntry nextEntry;
            do
            {
              nextEntry = tis.GetNextEntry();
              if (nextEntry == null)
                goto label_8;
            }
            while (nextEntry.IsDirectory);
            yield return new ProviderImageInfo(0, nextEntry.Name, nextEntry.Size);
          }
        }
        finally
        {
          tis?.Dispose();
        }
label_8:
        tis = (TarInputStream) null;
      }
    }

    public override byte[] ReadByteImage(string source, ProviderImageInfo info)
    {
      try
      {
        using (FileStream inputStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, 131072))
        {
          using (TarInputStream tarInputStream = new TarInputStream((Stream) inputStream))
          {
            do
              ;
            while (!(tarInputStream.GetNextEntry().Name == info.Name));
            byte[] buffer = new byte[info.Size];
            if (tarInputStream.Read(buffer, 0, buffer.Length) != buffer.Length)
              throw new IOException();
            return buffer;
          }
        }
      }
      catch
      {
        return (byte[]) null;
      }
    }

    public override ComicInfo ReadInfo(string source)
    {
      try
      {
        using (FileStream inputStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, 131072))
        {
          using (TarInputStream tarInputStream = new TarInputStream((Stream) inputStream))
          {
            TarEntry nextEntry;
            do
            {
              nextEntry = tarInputStream.GetNextEntry();
            }
            while (string.Compare(Path.GetFileName(nextEntry.Name), "ComicInfo.xml", true) != 0);
            byte[] buffer = new byte[nextEntry.Size];
            tarInputStream.Read(buffer, 0, buffer.Length);
            using (MemoryStream inStream = new MemoryStream(buffer))
              return ComicInfo.Deserialize((Stream) inStream);
          }
        }
      }
      catch
      {
        return (ComicInfo) null;
      }
    }
  }
}

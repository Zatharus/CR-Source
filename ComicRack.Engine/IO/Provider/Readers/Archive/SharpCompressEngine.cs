// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive.SharpCompressEngine
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Xml;
using SharpCompress.Archive;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive
{
  public class SharpCompressEngine : FileBasedAccessor
  {
    private int format;

    public SharpCompressEngine(int format)
      : base(format)
    {
      this.format = format;
    }

    public override bool IsFormat(string source)
    {
      if (this.HasSignature)
        return base.IsFormat(source);
      try
      {
        using (IArchive archive = ArchiveFactory.Open(source))
        {
          switch (archive.Type)
          {
            case ArchiveType.Rar:
              return this.format == 3;
            case ArchiveType.Zip:
              return this.format == 2;
            case ArchiveType.Tar:
              return this.format == 5;
            case ArchiveType.SevenZip:
              return this.format == 6;
            default:
              return false;
          }
        }
      }
      catch
      {
        return false;
      }
    }

    public override IEnumerable<ProviderImageInfo> GetEntryList(string source)
    {
      using (IArchive archive = ArchiveFactory.Open(source))
      {
        int n = 0;
        foreach (IArchiveEntry entry in archive.Entries)
        {
          if (!entry.IsDirectory)
            yield return new ProviderImageInfo(n, entry.FilePath, entry.Size);
          ++n;
        }
      }
    }

    public override byte[] ReadByteImage(string source, ProviderImageInfo info)
    {
      using (IArchive archive = ArchiveFactory.Open(source))
      {
        IArchiveEntry archiveEntry = archive.Entries.Skip<IArchiveEntry>(info.Index).First<IArchiveEntry>();
        MemoryStream memoryStream = new MemoryStream((int) archiveEntry.Size);
        archiveEntry.WriteTo((Stream) memoryStream);
        return memoryStream.ToArray();
      }
    }

    public override ComicInfo ReadInfo(string source)
    {
      using (IArchive archive = ArchiveFactory.Open(source))
      {
        IArchiveEntry archiveEntry = archive.Entries.FirstOrDefault<IArchiveEntry>((Func<IArchiveEntry, bool>) (e => Path.GetFileName(e.FilePath).Equals("ComicInfo.xml", StringComparison.OrdinalIgnoreCase)));
        if (archiveEntry == null)
          return (ComicInfo) null;
        using (Stream s = archiveEntry.OpenEntryStream())
          return XmlUtility.Load<ComicInfo>(s, false);
      }
    }
  }
}

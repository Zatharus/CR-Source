// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.CbzComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  [FileFormat("eComic (ZIP)", 2, ".cbz", EnableUpdate = true)]
  [FileFormat("ZIP Archive", 2, ".zip")]
  public class CbzComicProvider : ArchiveComicProvider
  {
    public CbzComicProvider()
    {
      switch (EngineConfiguration.Default.CbzUses)
      {
        case EngineConfiguration.CbEngines.SevenZipExe:
          this.SetArchive((IComicAccessor) new SevenZipEngine(2, false));
          break;
        case EngineConfiguration.CbEngines.SharpCompress:
          this.SetArchive((IComicAccessor) new SharpCompressEngine(2));
          break;
        case EngineConfiguration.CbEngines.SharpZip:
          this.SetArchive((IComicAccessor) new ZipSharpZipEngine());
          break;
        default:
          this.SetArchive((IComicAccessor) new SevenZipEngine(2, true));
          break;
      }
    }
  }
}

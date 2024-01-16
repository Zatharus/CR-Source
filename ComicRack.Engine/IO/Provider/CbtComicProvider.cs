// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.CbtComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Projects.ComicRack.Engine.IO.Provider.Readers;
using cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  [FileFormat("eComic (TAR)", 5, ".cbt", EnableUpdate = true)]
  [FileFormat("TAR Archive", 5, ".tar")]
  public class CbtComicProvider : ArchiveComicProvider
  {
    public CbtComicProvider()
    {
      switch (EngineConfiguration.Default.CbtUses)
      {
        case EngineConfiguration.CbEngines.SevenZipExe:
          this.SetArchive((IComicAccessor) new SevenZipEngine(5, false));
          break;
        case EngineConfiguration.CbEngines.SharpCompress:
          this.SetArchive((IComicAccessor) new SharpCompressEngine(5));
          break;
        case EngineConfiguration.CbEngines.SharpZip:
          this.SetArchive((IComicAccessor) new TarSharpZipEngine());
          break;
        default:
          this.SetArchive((IComicAccessor) new SevenZipEngine(5, true));
          break;
      }
    }
  }
}

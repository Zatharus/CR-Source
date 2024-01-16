// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.CbrComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  [FileFormat("eComic (RAR)", 3, ".cbr")]
  [FileFormat("RAR Archive", 3, ".rar")]
  public class CbrComicProvider : ArchiveComicProvider
  {
    public CbrComicProvider()
    {
      switch (EngineConfiguration.Default.CbrUses)
      {
        case EngineConfiguration.CbEngines.SevenZipExe:
          this.SetArchive((IComicAccessor) new SevenZipEngine(3, false));
          break;
        case EngineConfiguration.CbEngines.SharpCompress:
          this.SetArchive((IComicAccessor) new SharpCompressEngine(3));
          break;
        default:
          this.SetArchive((IComicAccessor) new SevenZipEngine(3, true));
          break;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Cb7ComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  [FileFormat("eComic (7z)", 6, ".cb7", EnableUpdate = true)]
  [FileFormat("7z Archive", 6, ".7z")]
  public class Cb7ComicProvider : ArchiveComicProvider
  {
    public Cb7ComicProvider()
    {
      switch (EngineConfiguration.Default.Cb7Uses)
      {
        case EngineConfiguration.CbEngines.SevenZipExe:
          this.SetArchive((IComicAccessor) new SevenZipEngine(6, false));
          break;
        case EngineConfiguration.CbEngines.SharpCompress:
          this.SetArchive((IComicAccessor) new SharpCompressEngine(6));
          break;
        default:
          this.SetArchive((IComicAccessor) new SevenZipEngine(6, true));
          break;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.ExtraSyncInformation
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public class ExtraSyncInformation
  {
    public bool ReadingStateChanged { get; set; }

    public bool InformationChanged { get; set; }

    public bool BookmarksChanged { get; set; }

    public bool PageTypesChanged { get; set; }

    public bool CheckChanged { get; set; }

    public bool DataChanged { get; set; }
  }
}

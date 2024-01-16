// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.ImageIndexReadyEventArgs
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public class ImageIndexReadyEventArgs : CancelEventArgs
  {
    private readonly int imageNumber;
    private readonly ProviderImageInfo imageInfo;

    public ImageIndexReadyEventArgs(int imageNumber, ProviderImageInfo ii)
    {
      this.imageNumber = imageNumber;
      this.imageInfo = ii;
    }

    public int ImageNumber => this.imageNumber;

    public ProviderImageInfo ImageInfo => this.imageInfo;
  }
}

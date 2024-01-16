// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.RemoteComicBookProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  public class RemoteComicBookProvider : ImageProvider
  {
    private readonly Guid comicId;
    private readonly ComicLibraryClient client;
    private readonly object retrieveLock = new object();

    public RemoteComicBookProvider(Guid comicId, ComicLibraryClient client)
    {
      this.comicId = comicId;
      this.client = client;
    }

    public override ImageProviderCapabilities Capabilities => base.Capabilities;

    public override int FormatId => -1;

    protected override void OnParse()
    {
      int imageCount = this.client.RemoteLibrary.GetImageCount(this.comicId);
      for (int index = 0; index < imageCount; ++index)
        this.FireIndexReady(new ProviderImageInfo());
    }

    protected override byte[] OnRetrieveSourceByteImage(int index)
    {
      try
      {
        using (ItemMonitor.Lock(this.retrieveLock))
          return this.client.RemoteLibrary.GetImage(this.comicId, index);
      }
      catch
      {
        return (byte[]) null;
      }
    }

    protected override void OnCheckSource()
    {
      if (this.client == null || this.client.RemoteLibrary == null)
        throw new InvalidOperationException("No valid remote library");
    }

    public override string CreateHash() => string.Empty;

    protected override ThumbnailImage OnRetrieveThumbnailImage(int index)
    {
      try
      {
        return ThumbnailImage.CreateFrom(this.client.RemoteLibrary.GetThumbnailImage(this.comicId, index));
      }
      catch
      {
        return (ThumbnailImage) null;
      }
    }
  }
}

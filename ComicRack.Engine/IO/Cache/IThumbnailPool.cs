// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Cache.IThumbnailPool
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Cache
{
  public interface IThumbnailPool
  {
    void CacheThumbnail(ThumbnailKey key, bool checkMemoryOnly, IImageProvider provider);

    IItemLock<ThumbnailImage> GetThumbnail(ThumbnailKey key, bool onlyMemory);

    IItemLock<ThumbnailImage> GetThumbnail(
      ThumbnailKey key,
      IImageProvider provider,
      bool onErrorThrowException);

    event EventHandler<CacheItemEventArgs<ImageKey, ThumbnailImage>> ThumbnailCached;
  }
}

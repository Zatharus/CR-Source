// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Cache.IPagePool
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
  public interface IPagePool
  {
    event EventHandler<CacheItemEventArgs<ImageKey, PageImage>> PageCached;

    IItemLock<PageImage> GetPage(PageKey key, bool onlyMemory);

    IItemLock<PageImage> GetPage(PageKey key, IImageProvider provider, bool onErrorThrowException);

    void RefreshPage(PageKey key);

    void CachePage(PageKey key, bool fastMem, IImageProvider provider, bool bottom);

    void RemoveImages(string source, int index = -1);

    bool IsWorking { get; }

    int MaximumMemoryItems { get; }

    void RefreshLastImage(string p);
  }
}

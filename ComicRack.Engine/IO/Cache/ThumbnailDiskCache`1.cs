// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Cache.ThumbnailDiskCache`1
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Cache
{
  public class ThumbnailDiskCache<K> : DiskCache<K, ThumbnailImage>
  {
    public ThumbnailDiskCache(string cacheFolder, int cacheSizeMB)
      : base(cacheFolder, cacheSizeMB)
    {
    }

    protected override ThumbnailImage LoadItem(string file) => ThumbnailImage.CreateFrom(file);

    protected override void StoreItem(string file, ThumbnailImage item) => item.Save(file);
  }
}

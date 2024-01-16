// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.CacheManager
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using System;
using System.Drawing;
using System.Resources;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class CacheManager
  {
    public const int MemoryThumbnailCacheSize = 4096;

    public CacheManager(
      DatabaseManager databaseManager,
      SystemPaths paths,
      ICacheSettings settings,
      ResourceManager resourceManager = null)
    {
      CacheManager cacheManager = this;
      this.ResourceManager = cYo.Projects.ComicRack.Engine.Properties.Resources.ResourceManager;
      this.DatabaseManager = databaseManager;
      this.InternetCache = new FileCache(paths.FileCachePath, 100);
      this.ImagePool = new ImagePool(4096, (long) settings.MemoryThumbCacheSizeMB, settings.MemoryPageCacheCount);
      this.ImagePool.Thumbs.DiskCache = (IDiskCache<ImageKey, ThumbnailImage>) new ThumbnailDiskCache<ImageKey>(paths.ThumbnailCachePath, settings.ThumbCacheSizeMB);
      this.ImagePool.Pages.DiskCache = (IDiskCache<ImageKey, PageImage>) new ImageDiskCache<ImageKey>(paths.ImageCachePath, settings.PageCacheSizeMB);
      this.ImagePool.CustomThumbnailFolder = paths.CustomThumbnailPath;
      this.ImagePool.RequestResourceThumbnail += new EventHandler<ResourceThumbnailEventArgs>(this.RequestResourceThumbnail);
      this.ImagePool.PageCached += new EventHandler<CacheItemEventArgs<ImageKey, PageImage>>(this.ImagePoolPageCached);
      this.ImagePool.ThumbnailCached += new EventHandler<CacheItemEventArgs<ImageKey, ThumbnailImage>>(this.ImagePoolThumbnailCached);
      this.UpdateCacheSettings(settings);
      settings.CacheSettingsChanged += (EventHandler) ((_, __) => cacheManager.UpdateCacheSettings(settings));
    }

    public DatabaseManager DatabaseManager { get; private set; }

    public FileCache InternetCache { get; private set; }

    public ImagePool ImagePool { get; private set; }

    public ResourceManager ResourceManager { get; private set; }

    private void RequestResourceThumbnail(object sender, ResourceThumbnailEventArgs e)
    {
      try
      {
        switch (e.Key.ResourceType)
        {
          case "resource":
            if (this.ResourceManager == null)
              break;
            using (Bitmap image = this.ResourceManager.GetObject(e.Key.ResourceLocation) as Bitmap)
            {
              e.Image = ThumbnailImage.CreateFrom(image, image.Size);
              break;
            }
          case "file":
            using (Bitmap image = BitmapExtensions.BitmapFromFile(e.Key.ResourceLocation))
            {
              e.Image = ThumbnailImage.CreateFrom(image, image.Size);
              break;
            }
          case "custom":
            e.Image = this.ImagePool.GetCustomThumbnail(e.Key.ResourceLocation);
            break;
          default:
            throw new ArgumentException();
        }
      }
      catch (Exception ex)
      {
      }
    }

    private void ImagePoolPageCached(object sender, CacheItemEventArgs<ImageKey, PageImage> e)
    {
      this.UpdateComicBookPageData(e.Key, e.Item.Size);
    }

    private void ImagePoolThumbnailCached(
      object sender,
      CacheItemEventArgs<ImageKey, ThumbnailImage> e)
    {
      this.UpdateComicBookPageData(e.Key, e.Item.OriginalSize);
    }

    public void UpdateCacheSettings(ICacheSettings settings)
    {
      this.InternetCache.Enabled = settings.InternetCacheEnabled;
      this.InternetCache.CacheSizeMB = settings.InternetCacheSizeMB;
      this.ImagePool.Thumbs.DiskCache.CacheSizeMB = settings.ThumbCacheSizeMB;
      this.ImagePool.Thumbs.DiskCache.Enabled = settings.ThumbCacheEnabled;
      this.ImagePool.Pages.DiskCache.CacheSizeMB = settings.PageCacheSizeMB;
      this.ImagePool.Pages.DiskCache.Enabled = settings.PageCacheEnabled;
      this.ImagePool.Thumbs.MemoryCache.SizeCapacity = (long) settings.MemoryThumbCacheSizeMB * 1024L * 1024L;
      this.ImagePool.Pages.MemoryCache.ItemCapacity = settings.MemoryPageCacheCount;
      ThumbnailImage.MemoryOptimized = settings.MemoryThumbCacheOptimized;
      PageImage.MemoryOptimized = settings.MemoryPageCacheOptimized;
    }

    private void UpdateComicBookPageData(ImageKey key, Size size)
    {
      this.UpdateComicBookPageData(this.DatabaseManager.Database.Books[key.Location], key, size);
      this.UpdateComicBookPageData(key.Source as ComicBook, key, size);
      this.UpdateComicBookPageData(key.Source as ComicBookNavigator, key, size);
    }

    private void UpdateComicBookPageData(ComicBook cb, ImageKey key, Size size)
    {
      if (cb == null || size.IsEmpty)
        return;
      int page = cb.TranslateImageIndexToPage(key.Index);
      cb.UpdatePageSize(page, size.Width, size.Height);
    }

    private void UpdateComicBookPageData(ComicBookNavigator nav, ImageKey key, Size size)
    {
      if (nav == null)
        return;
      this.UpdateComicBookPageData(nav.Comic, key, size);
    }
  }
}

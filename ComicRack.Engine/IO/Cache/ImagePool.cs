// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Cache.ImagePool
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Engine.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Cache
{
  public class ImagePool : DisposableObject, IPagePool, IThumbnailPool, ICustomThumbnail
  {
    private const int DefaultThumbCount = 20;
    private const int DefaultThumbSize = 5242880;
    private const int DefaultPageCount = 5;
    private readonly ThumbnailManager thumbs;
    private readonly ImageManager pages;
    private readonly ProcessingQueue<ImageKey> fastThumbnailQueue;
    private readonly ProcessingQueue<ImageKey> slowThumbnailQueue;
    private readonly ProcessingQueue<ImageKey> slowPageQueue;
    private readonly ProcessingQueue<ImageKey> fastPageQueue;
    private bool cacheThumbnailPages = EngineConfiguration.Default.CacheThumbnailPages;
    private string customThumbnailFolder;

    public ImagePool()
      : this(20, 5242880L, 5)
    {
    }

    public ImagePool(int thumbCount, long thumbSize, int pageCount)
    {
      this.thumbs = new ThumbnailManager(thumbCount, thumbSize);
      this.pages = new ImageManager(pageCount);
      int threadCount = Environment.ProcessorCount.Clamp(1, EngineConfiguration.Default.MaximumQueueThreads);
      this.fastPageQueue = new ProcessingQueue<ImageKey>("Background Fast Page Queue", ThreadPriority.BelowNormal, pageCount * 2);
      this.slowPageQueue = new ProcessingQueue<ImageKey>(threadCount, "Background Slow Page Queue", ThreadPriority.BelowNormal, pageCount * 2);
      this.fastThumbnailQueue = new ProcessingQueue<ImageKey>("Background Fast Thumbnails Queue", ThreadPriority.Lowest, 256);
      this.slowThumbnailQueue = new ProcessingQueue<ImageKey>(threadCount, "Background Slow Thumbnails Queue", ThreadPriority.Lowest, 256);
      this.pages.MemoryCache.ItemAdded += new EventHandler<CacheItemEventArgs<ImageKey, PageImage>>(this.MemoryPageCacheItemAdded);
      this.thumbs.MemoryCache.ItemAdded += new EventHandler<CacheItemEventArgs<ImageKey, ThumbnailImage>>(this.MemoryThumbnailCacheItemAdded);
      this.slowThumbnailQueue.DefaultProcessingQueueAddMode = this.fastThumbnailQueue.DefaultProcessingQueueAddMode = this.slowPageQueue.DefaultProcessingQueueAddMode = this.fastPageQueue.DefaultProcessingQueueAddMode = ProcessingQueueAddMode.AddToTop;
    }

    public ThumbnailManager Thumbs => this.thumbs;

    public ImageManager Pages => this.pages;

    public ProcessingQueue<ImageKey> FastThumbnailQueue => this.fastThumbnailQueue;

    public ProcessingQueue<ImageKey> SlowThumbnailQueue => this.slowThumbnailQueue;

    public ProcessingQueue<ImageKey> SlowPageQueue => this.slowPageQueue;

    public ProcessingQueue<ImageKey> FastPageQueue => this.fastPageQueue;

    public bool CacheThumbnailPages
    {
      get => this.cacheThumbnailPages;
      set => this.cacheThumbnailPages = value;
    }

    public bool AreImagesPending(string filePath)
    {
      return this.slowPageQueue.PendingItems.Any<ImageKey>((Func<ImageKey, bool>) (key => key.Location == filePath)) || this.fastPageQueue.PendingItems.Any<ImageKey>((Func<ImageKey, bool>) (key => key.Location == filePath)) || this.fastThumbnailQueue.PendingItems.Any<ImageKey>((Func<ImageKey, bool>) (key => key.Location == filePath)) || this.slowThumbnailQueue.PendingItems.Any<ImageKey>((Func<ImageKey, bool>) (key => key.Location == filePath));
    }

    public void AddThumbToQueue(ThumbnailKey key, object callbackKey, AsyncCallback asyncCallback)
    {
      if (this.thumbs.DiskCache.IsAvailable((ImageKey) key))
        this.fastThumbnailQueue.AddItem((ImageKey) key, callbackKey, asyncCallback);
      else
        this.slowThumbnailQueue.AddItem((ImageKey) key, callbackKey, asyncCallback);
    }

    public IItemLock<ThumbnailImage> GetThumbnail(
      ThumbnailKey key,
      IImageProvider provider,
      ComicBook comic)
    {
      using (IImageProvider provider1 = (IImageProvider) new ComicBookImageProvider(comic, provider, comic.TranslateImageIndexToPage(key.Index)))
        return this.GetThumbnail(key, provider1, false);
    }

    public IItemLock<ThumbnailImage> GetThumbnail(ThumbnailKey key, ComicBook comic)
    {
      return this.GetThumbnail(key, (IImageProvider) null, comic);
    }

    public IItemLock<ThumbnailImage> GetThumbnail(ComicBook comic)
    {
      return this.GetThumbnail(comic.GetFrontCoverThumbnailKey(), (IImageProvider) null, comic);
    }

    public virtual Bitmap CreateErrorPage()
    {
      Bitmap errorPage = Resources.ErrorPage;
      using (Graphics graphics = Graphics.FromImage((Image) errorPage))
        graphics.DrawString(TR.Messages["PageFailedToLoad", "Page failed to load.\nTry refresh to load again..."], FC.Get(SystemFonts.MenuFont, 32f), Brushes.Black, 40f, 40f);
      return errorPage;
    }

    public virtual Bitmap CreateErrorThumbnail(int height)
    {
      Bitmap errorThumbnail = new Bitmap(height * 2 / 3, height);
      using (Graphics graphics = Graphics.FromImage((Image) errorThumbnail))
      {
        using (Image redCross = (Image) Resources.RedCross)
        {
          Rectangle bounds = new Rectangle(0, 0, errorThumbnail.Width, errorThumbnail.Height);
          Rectangle rect = new Rectangle(0, 0, redCross.Width * 3, redCross.Height * 3).Align(bounds, ContentAlignment.MiddleCenter);
          graphics.Clear(Color.White);
          graphics.DrawImage(redCross, rect);
        }
      }
      return errorThumbnail;
    }

    protected virtual void OnPageCached(CacheItemEventArgs<ImageKey, PageImage> e)
    {
      if (this.PageCached == null)
        return;
      this.PageCached((object) this, e);
    }

    protected virtual void OnThumbnailCached(CacheItemEventArgs<ImageKey, ThumbnailImage> e)
    {
      if (this.ThumbnailCached == null)
        return;
      this.ThumbnailCached((object) this, e);
    }

    protected virtual void OnRequestResourceThumbnail(ResourceThumbnailEventArgs e)
    {
      if (this.RequestResourceThumbnail == null)
        return;
      this.RequestResourceThumbnail((object) this, e);
    }

    public event EventHandler<ResourceThumbnailEventArgs> RequestResourceThumbnail;

    protected virtual ThumbnailImage GetResourceThumbnail(ThumbnailKey key)
    {
      ResourceThumbnailEventArgs e = new ResourceThumbnailEventArgs(key);
      this.OnRequestResourceThumbnail(e);
      return e.Image;
    }

    private void MemoryPageCacheItemAdded(object sender, CacheItemEventArgs<ImageKey, PageImage> e)
    {
      this.OnPageCached(e);
    }

    private void MemoryThumbnailCacheItemAdded(
      object sender,
      CacheItemEventArgs<ImageKey, ThumbnailImage> e)
    {
      this.OnThumbnailCached(e);
    }

    public void AddPageToQueue(
      PageKey key,
      object callbackKey,
      AsyncCallback asyncCallback,
      bool bottom)
    {
      ProcessingQueueAddMode mode = bottom ? ProcessingQueueAddMode.AddToBottom : ProcessingQueueAddMode.AddToTop;
      if (this.pages.DiskCache.IsAvailable((ImageKey) key))
        this.fastPageQueue.AddItem((ImageKey) key, callbackKey, asyncCallback, mode);
      else
        this.slowPageQueue.AddItem((ImageKey) key, callbackKey, asyncCallback, mode);
    }

    public IItemLock<PageImage> GetPage(PageKey key, bool onlyMemory)
    {
      return this.pages.GetImage((ImageKey) key, onlyMemory);
    }

    public IItemLock<PageImage> GetPage(
      PageKey key,
      IImageProvider provider,
      bool onErrorThrowException = false)
    {
      IItemLock<PageImage> page = this.pages.AddImage((ImageKey) key, (Func<ImageKey, PageImage>) (k =>
      {
        try
        {
          if (provider != null)
          {
            Bitmap bitmap1 = (Bitmap) null;
            Bitmap bitmap2 = (Bitmap) null;
            BitmapAdjustment bitmapAdjustment = key.Adjustment;
            ImageRotation imageRotation = key.Rotation;
            if (bitmapAdjustment.IsEmpty && imageRotation == ImageRotation.None)
            {
              byte[] byteImage = provider.GetByteImage(key.Index);
              if (byteImage != null)
                return PageImage.CreateFrom(byteImage);
            }
            else
            {
              if ((bitmap2 = this.GetPartialDiskPage(key, ImageRotation.None, bitmapAdjustment)) != null)
                bitmapAdjustment = BitmapAdjustment.Empty;
              else if ((bitmap2 = this.GetPartialDiskPage(key, imageRotation, BitmapAdjustment.Empty)) != null)
                imageRotation = ImageRotation.None;
              else if ((bitmap2 = this.GetPartialDiskPage(key, ImageRotation.None, BitmapAdjustment.Empty)) != null)
              {
                imageRotation = ImageRotation.None;
                bitmapAdjustment = BitmapAdjustment.Empty;
              }
              bitmap1 = bitmap2;
            }
            if (bitmap2 == null)
            {
              bitmap2 = bitmap1 = provider.GetImage(key.Index);
              if (bitmap2 != null && provider.IsSlow)
              {
                using (PageImage from = PageImage.CreateFrom(bitmap2))
                  this.pages.DiskCache.AddItem((ImageKey) new PageKey(key.Source, key.Location, key.Size, key.Modified, key.Index, ImageRotation.None, BitmapAdjustment.Empty), from);
              }
            }
            try
            {
              if (!bitmapAdjustment.IsEmpty)
                bitmap1 = bitmap2.CreateAdjustedBitmap(bitmapAdjustment, PixelFormat.Format32bppArgb, false);
            }
            finally
            {
              if (bitmap2 != bitmap1)
                bitmap2.Dispose();
            }
            if (imageRotation != ImageRotation.None)
            {
              Bitmap bitmap3 = bitmap1.Rotate(imageRotation);
              bitmap1.Dispose();
              bitmap1 = bitmap3;
            }
            return PageImage.Wrap(bitmap1);
          }
        }
        catch
        {
        }
        return (PageImage) null;
      }));
      if (page != null)
        return page;
      if (onErrorThrowException)
        throw new Exception("Could not open image");
      return this.pages.MemoryCache.LockItem((ImageKey) key, (Func<ImageKey, PageImage>) (tk => PageImage.Wrap(this.CreateErrorPage())));
    }

    private Bitmap GetPartialDiskPage(PageKey key, ImageRotation rot, BitmapAdjustment transform)
    {
      using (PageImage pageImage = this.pages.DiskCache.GetItem((ImageKey) new PageKey(key.Source, key.Location, key.Size, key.Modified, key.Index, rot, transform)))
      {
        if (pageImage != null)
        {
          if (pageImage.Bitmap != null)
            return pageImage.Detach();
        }
      }
      return (Bitmap) null;
    }

    public IItemLock<PageImage> GetPage(PageKey key, ComicBook book)
    {
      using (IImageProvider provider = (IImageProvider) book.OpenProvider(key.Index))
        return this.GetPage(key, provider);
    }

    public void RefreshPage(PageKey key) => this.pages.RefreshImage((ImageKey) key);

    public void RefreshLastImage(string source)
    {
      this.pages.RefreshLastImage(source);
      this.thumbs.RefreshLastImage(source);
    }

    public void RemoveImages(string source, int imageIndex = -1)
    {
      this.pages.RemoveKeys((Func<ImageKey, bool>) (k =>
      {
        if (!string.Equals(k.Location, source, StringComparison.OrdinalIgnoreCase))
          return false;
        return imageIndex == -1 || k.Index == imageIndex;
      }));
      this.thumbs.RemoveKeys((Func<ImageKey, bool>) (k =>
      {
        if (!string.Equals(k.Location, source, StringComparison.OrdinalIgnoreCase))
          return false;
        return imageIndex == -1 || k.Index == imageIndex;
      }));
    }

    public void CachePage(PageKey key, bool checkMemoryOnly, IImageProvider provider, bool bottom)
    {
      using (IItemLock<PageImage> image = this.Pages.GetImage((ImageKey) key, checkMemoryOnly))
      {
        if (image != null)
        {
          if (image.Item != null)
            return;
        }
      }
      this.AddPageToQueue(key, (object) null, (AsyncCallback) (ar =>
      {
        try
        {
          if (key.Index >= provider.Count)
            return;
          this.GetPage(key, provider).SafeDispose();
        }
        catch
        {
        }
      }), bottom);
    }

    public bool IsWorking
    {
      get
      {
        return this.slowPageQueue.IsActive || this.slowThumbnailQueue.IsActive || this.fastPageQueue.IsActive || this.fastThumbnailQueue.IsActive;
      }
    }

    public int MaximumMemoryItems => this.pages.MemoryCache.ItemCapacity;

    public event EventHandler<CacheItemEventArgs<ImageKey, PageImage>> PageCached;

    public IItemLock<ThumbnailImage> GetThumbnail(ThumbnailKey key, bool onlyMemory)
    {
      return this.thumbs.GetImage((ImageKey) key, onlyMemory);
    }

    public IItemLock<ThumbnailImage> GetThumbnail(
      ThumbnailKey key,
      IImageProvider provider,
      bool onErrorThrowException)
    {
      IItemLock<ThumbnailImage> thumbnail1 = this.thumbs.AddImage((ImageKey) key, (Func<ImageKey, ThumbnailImage>) (k =>
      {
        PageKey key1 = new PageKey((ImageKey) key);
        if (!string.IsNullOrEmpty(key.ResourceType))
        {
          ThumbnailImage resourceThumbnail = this.GetResourceThumbnail(key);
          if (resourceThumbnail != null)
            return resourceThumbnail;
        }
        if (provider != null)
        {
          ThumbnailImage thumbnail2 = provider.GetThumbnail(key.Index);
          if (thumbnail2 != null)
            return thumbnail2;
        }
        try
        {
          IItemLock<PageImage> itemLock = this.pages.GetImage((ImageKey) key1);
          Image image1 = (Image) null;
          try
          {
            Bitmap image2 = (Bitmap) null;
            if (itemLock != null && itemLock.Item != null)
              image2 = itemLock.Item.Bitmap;
            else if (provider != null)
            {
              if (this.cacheThumbnailPages || provider.IsSlow)
              {
                itemLock = this.pages.AddImage((ImageKey) key1, provider);
                if (itemLock != null && itemLock.Item != null)
                  image2 = itemLock.Item.Bitmap;
              }
              else
              {
                byte[] byteImage = provider.GetByteImage(key.Index);
                image2 = byteImage == null ? provider.GetImage(key.Index) : BitmapExtensions.BitmapFromBytes(byteImage, PixelFormat.Undefined);
                if (image2 != null && key.Rotation != ImageRotation.None)
                {
                  Bitmap bitmap = image2.Rotate(key.Rotation);
                  image2.Dispose();
                  image2 = bitmap;
                }
                image1 = (Image) image2;
              }
            }
            if (image2 != null)
              return ThumbnailImage.CreateFrom(image2, image2.Size);
          }
          finally
          {
            itemLock?.Dispose();
            image1?.Dispose();
          }
        }
        catch
        {
        }
        return (ThumbnailImage) null;
      }));
      if (thumbnail1 != null)
        return thumbnail1;
      if (onErrorThrowException)
        throw new InvalidOperationException("Could not load thumbnail");
      return this.thumbs.MemoryCache.LockItem((ImageKey) key, (Func<ImageKey, ThumbnailImage>) (tk =>
      {
        using (Bitmap errorThumbnail = this.CreateErrorThumbnail(512))
          return ThumbnailImage.CreateFrom(errorThumbnail, Size.Empty);
      }));
    }

    public void CacheThumbnail(ThumbnailKey key, bool checkMemoryOnly, IImageProvider provider)
    {
      using (IItemLock<ThumbnailImage> image = this.thumbs.GetImage((ImageKey) key, checkMemoryOnly))
      {
        if (image != null)
          return;
        this.AddThumbToQueue(key, (object) null, (AsyncCallback) (ar =>
        {
          try
          {
            if (key.Index >= provider.Count)
              return;
            this.GetThumbnail(key, provider, true).SafeDispose();
          }
          catch
          {
          }
        }));
      }
    }

    public event EventHandler<CacheItemEventArgs<ImageKey, ThumbnailImage>> ThumbnailCached;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.fastThumbnailQueue.Dispose();
        this.slowThumbnailQueue.Dispose();
        this.fastPageQueue.Dispose();
        this.slowPageQueue.Dispose();
        this.thumbs.Dispose();
        this.pages.Dispose();
      }
      base.Dispose(disposing);
    }

    public string CustomThumbnailFolder
    {
      get => this.customThumbnailFolder;
      set
      {
        if (value == this.customThumbnailFolder)
          return;
        if (!Directory.Exists(value))
          Directory.CreateDirectory(value);
        this.customThumbnailFolder = value;
      }
    }

    public string AddCustomThumbnail(Bitmap bmp)
    {
      string path2 = Guid.NewGuid().ToString();
      string file = Path.Combine(this.CustomThumbnailFolder, path2);
      using (ThumbnailImage from = ThumbnailImage.CreateFrom(bmp, bmp.Size, true))
        from.Save(file);
      return path2;
    }

    public IEnumerable<string> GetCustomThumbnailKeys()
    {
      return FileUtility.GetFiles(this.CustomThumbnailFolder, SearchOption.TopDirectoryOnly);
    }

    public bool RemoveCustomThumbnail(string key)
    {
      return FileUtility.SafeDelete(Path.Combine(this.CustomThumbnailFolder, key));
    }

    public ThumbnailImage GetCustomThumbnail(string key)
    {
      return ThumbnailImage.CreateFrom(Path.Combine(this.CustomThumbnailFolder, key));
    }

    public bool CustomThumbnailExists(string key)
    {
      return File.Exists(Path.Combine(this.CustomThumbnailFolder, key));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Cache.ImageManagerBase`1
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Cache
{
  public abstract class ImageManagerBase<T> : DisposableObject where T : class, IDisposable
  {
    private readonly cYo.Common.Collections.Cache<ImageKey, T> memoryCache = new cYo.Common.Collections.Cache<ImageKey, T>(256);

    protected ImageManagerBase()
    {
      this.memoryCache.ItemRemoved += new EventHandler<CacheItemEventArgs<ImageKey, T>>(this.memoryCache_ItemRemoved);
    }

    protected ImageManagerBase(int itemCapacity, long sizeCapacity)
      : this()
    {
      this.memoryCache.ItemCapacity = itemCapacity;
      this.memoryCache.SizeCapacity = sizeCapacity;
    }

    protected ImageManagerBase(int itemCapacity)
      : this(itemCapacity, long.MaxValue)
    {
    }

    public virtual IItemLock<T> GetImage(ImageKey key, bool memoryOnly = false, bool dontUpdateAccess = false)
    {
      return this.MemoryCache.LockItem(key, memoryOnly || this.DiskCache == null ? (Func<ImageKey, T>) null : new Func<ImageKey, T>(this.DiskCache.GetItem), dontUpdateAccess);
    }

    public void RefreshImage(ImageKey key)
    {
      this.memoryCache.RemoveItem(key);
      if (this.DiskCache == null)
        return;
      this.DiskCache.RemoveItem(key);
    }

    public void RefreshLastImage(string source)
    {
      Func<ImageKey, bool> predicate = (Func<ImageKey, bool>) (k => string.Equals(k.Location, source, StringComparison.OrdinalIgnoreCase));
      try
      {
        this.RefreshImage(((IEnumerable<ImageKey>) this.MemoryCache.GetKeys()).Where<ImageKey>(predicate).Max<ImageKey>((Comparison<ImageKey>) ((a, b) => Math.Sign(a.Index - b.Index))));
      }
      catch
      {
      }
      try
      {
        this.RefreshImage(((IEnumerable<ImageKey>) this.DiskCache.GetKeys()).Where<ImageKey>(predicate).Max<ImageKey>((Comparison<ImageKey>) ((a, b) => Math.Sign(a.Index - b.Index))));
      }
      catch
      {
      }
    }

    public IItemLock<T> AddImage(ImageKey key, Func<ImageKey, T> getImage)
    {
      return this.MemoryCache.LockItem(key, (Func<ImageKey, T>) (k =>
      {
        if (this.DiskCache != null)
        {
          T obj = this.DiskCache.GetItem(key);
          if ((object) obj != null)
            return obj;
        }
        T obj1 = getImage(key);
        if ((object) obj1 != null && this.DiskCache != null)
          this.DiskCache.AddItem(key, obj1);
        return obj1;
      }));
    }

    public IItemLock<T> AddImage(ImageKey key, IImageProvider imageProvider)
    {
      return this.AddImage(key, (Func<ImageKey, T>) (k => this.CreateNewFromProvider(k, imageProvider)));
    }

    public bool IsAvailable(ImageKey key, bool memoryOnly)
    {
      using (IItemLock<T> image = this.GetImage(key, memoryOnly))
        return image != null && (object) image.Item != null;
    }

    public bool IsAvailable(ImageKey key) => this.IsAvailable(key, false);

    public void UpdateKeys(Func<ImageKey, bool> select, Action<ImageKey> update)
    {
      this.MemoryCache.UpdateKeys(select, update);
      if (this.DiskCache == null)
        return;
      this.DiskCache.UpdateKeys(select, update);
    }

    public void RemoveKeys(Func<ImageKey, bool> select)
    {
      this.MemoryCache.RemoveKeys(select);
      if (this.DiskCache == null)
        return;
      this.DiskCache.RemoveKeys(select);
    }

    public IDiskCache<ImageKey, T> DiskCache { get; set; }

    public cYo.Common.Collections.Cache<ImageKey, T> MemoryCache => this.memoryCache;

    private void memoryCache_ItemRemoved(object sender, CacheItemEventArgs<ImageKey, T> e)
    {
      if ((object) e.Item == null)
        return;
      ImageKey key = e.Key;
      T item = e.Item;
      ThreadPool.QueueUserWorkItem((WaitCallback) (o =>
      {
        using (ItemMonitor.Lock((object) item))
          item.Dispose();
      }));
    }

    protected abstract T CreateNewFromProvider(ImageKey key, IImageProvider provider);

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.memoryCache.Dispose();
        this.DiskCache.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}

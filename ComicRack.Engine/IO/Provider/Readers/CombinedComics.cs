// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.CombinedComics
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  public static class CombinedComics
  {
    public static IImageProvider OpenProvider(IEnumerable<ComicBook> books, IPagePool pool)
    {
      CombinedComics.CombinedImageProvider combinedImageProvider = new CombinedComics.CombinedImageProvider();
      foreach (ComicBook book in books)
      {
        if (book.IsDynamicSource)
          pool.RefreshLastImage(book.FilePath);
        combinedImageProvider.Providers.Add(new CombinedComics.Provider()
        {
          ImageProvider = (IImageProvider) book.OpenProvider(),
          KeyProvider = (IImageKeyProvider) book
        });
      }
      combinedImageProvider.PagePool = pool;
      return (IImageProvider) combinedImageProvider;
    }

    public static ComicInfo GetComicInfo(IEnumerable<ComicBook> books)
    {
      ComicInfo ci = (ComicInfo) null;
      int num = 0;
      bool flag = books.Count<ComicBook>() > 1;
      foreach (ComicBook book in books)
      {
        if (ci == null)
        {
          ci = new ComicInfo((ComicInfo) book);
          ci.Pages.Clear();
          book.SetShadowValues(ci);
        }
        for (int page1 = 0; page1 < book.PageCount; ++page1)
        {
          ComicPageInfo page2 = book.GetPage(page1);
          page2.ImageIndex += num;
          if (flag && page1 == 0)
            page2.Bookmark = book.Caption;
          ci.Pages.Add(page2);
        }
        num += book.PageCount;
      }
      return ci;
    }

    private class Provider
    {
      public IImageProvider ImageProvider { get; set; }

      public IImageKeyProvider KeyProvider { get; set; }
    }

    private class CombinedImageProvider : DisposableObject, IImageProvider, IDisposable
    {
      private readonly IList<CombinedComics.Provider> providers = (IList<CombinedComics.Provider>) new List<CombinedComics.Provider>();

      public IList<CombinedComics.Provider> Providers => this.providers;

      public IPagePool PagePool { get; set; }

      public bool IsSlow
      {
        get
        {
          return this.providers.Any<CombinedComics.Provider>((Func<CombinedComics.Provider, bool>) (p => p.ImageProvider.IsSlow));
        }
      }

      public string Source => this.providers[0].ImageProvider.Source;

      public int Count
      {
        get
        {
          return this.providers.Sum<CombinedComics.Provider>((Func<CombinedComics.Provider, int>) (p => p.ImageProvider.Count));
        }
      }

      public ProviderImageInfo GetImageInfo(int index)
      {
        return this.GetProvider(ref index).ImageProvider.GetImageInfo(index);
      }

      public ThumbnailImage GetThumbnail(int index)
      {
        return this.GetProvider(ref index).ImageProvider.GetThumbnail(index);
      }

      public Bitmap GetImage(int index)
      {
        CombinedComics.Provider provider = this.GetProvider(ref index);
        if (this.PagePool != null && provider.ImageProvider.IsSlow)
        {
          using (IItemLock<PageImage> page = this.PagePool.GetPage(new PageKey(provider.KeyProvider.GetImageKey(index)), false))
          {
            if (page != null)
            {
              if (page.Item != null)
                return page.Item.Bitmap.CreateCopy(true);
            }
          }
        }
        return provider.ImageProvider.GetImage(index);
      }

      public byte[] GetByteImage(int index)
      {
        CombinedComics.Provider provider = this.GetProvider(ref index);
        if (this.PagePool != null)
        {
          using (IItemLock<PageImage> page = this.PagePool.GetPage(new PageKey(provider.KeyProvider.GetImageKey(index)), false))
          {
            if (page != null)
            {
              if (page.Item != null)
                return page.Item.Data;
            }
          }
        }
        return provider.ImageProvider.GetByteImage(index);
      }

      protected override void Dispose(bool disposing)
      {
        foreach (CombinedComics.Provider provider in (IEnumerable<CombinedComics.Provider>) this.providers)
          provider.ImageProvider.SafeDispose();
      }

      private CombinedComics.Provider GetProvider(ref int imageIndex)
      {
        foreach (CombinedComics.Provider provider in (IEnumerable<CombinedComics.Provider>) this.providers)
        {
          if (imageIndex < provider.ImageProvider.Count)
            return provider;
          imageIndex -= provider.ImageProvider.Count;
          if (imageIndex < 0)
            break;
        }
        return (CombinedComics.Provider) null;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.ComicBookImageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  public class ComicBookImageProvider : DisposableObject, IImageProvider, IDisposable
  {
    private IImageProvider provider;
    private IImageProvider ownProvider;
    private readonly ComicBook comic;
    private readonly int lastPageIndex;

    public ComicBookImageProvider(ComicBook comic, IImageProvider provider, int lastPageIndex)
    {
      this.comic = comic;
      this.provider = provider;
      this.lastPageIndex = lastPageIndex;
    }

    public bool IsSlow
    {
      get
      {
        this.CheckProvider();
        return this.provider.IsSlow;
      }
    }

    public string Source
    {
      get
      {
        this.CheckProvider();
        return this.provider.Source;
      }
    }

    public int Count
    {
      get
      {
        this.CheckProvider();
        return this.provider.Count;
      }
    }

    public Bitmap GetImage(int index)
    {
      this.CheckProvider();
      return this.provider.GetImage(index);
    }

    public byte[] GetByteImage(int index)
    {
      this.CheckProvider();
      return this.provider.GetByteImage(index);
    }

    public ProviderImageInfo GetImageInfo(int index)
    {
      this.CheckProvider();
      return this.provider.GetImageInfo(index);
    }

    public ThumbnailImage GetThumbnail(int index)
    {
      this.CheckProvider();
      return this.provider.GetThumbnail(index);
    }

    private void CheckProvider()
    {
      if (this.provider != null || this.ownProvider != null)
        return;
      this.provider = this.ownProvider = (IImageProvider) this.comic.OpenProvider(this.lastPageIndex);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.ownProvider != null)
        this.ownProvider.Dispose();
      base.Dispose(disposing);
    }
  }
}

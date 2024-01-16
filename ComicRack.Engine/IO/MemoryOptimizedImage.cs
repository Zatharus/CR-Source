// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.MemoryOptimizedImage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Presentation;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  public abstract class MemoryOptimizedImage : DisposableObject
  {
    private volatile Bitmap bitmap;
    private byte[] data;
    private int timeToStay = 5;
    private volatile bool optimized = true;
    private Size size;
    private volatile int releaseTimeCounter;
    private static readonly Thread freeMemoryThread = ThreadUtility.CreateWorkerThread("Free Image Memory", new ThreadStart(MemoryOptimizedImage.FreeImageMemory), ThreadPriority.Lowest);
    private static readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
    private static readonly List<MemoryOptimizedImage> openImages = new List<MemoryOptimizedImage>();

    protected MemoryOptimizedImage(byte[] data, Size size)
    {
      this.data = data;
      if (size.IsEmpty)
      {
        JpegFile jpegFile = new JpegFile(data);
        if (jpegFile.IsValid)
          size = jpegFile.Size;
      }
      this.size = size;
      using (ItemMonitor.Lock((object) MemoryOptimizedImage.openImages))
        MemoryOptimizedImage.openImages.Add(this);
    }

    protected MemoryOptimizedImage(byte[] data)
      : this(data, Size.Empty)
    {
    }

    protected MemoryOptimizedImage(Bitmap bitmap)
      : this((byte[]) null, bitmap == null ? Size.Empty : bitmap.Size)
    {
      this.bitmap = bitmap;
    }

    protected override void Dispose(bool disposing)
    {
      using (ItemMonitor.Lock((object) MemoryOptimizedImage.openImages))
        MemoryOptimizedImage.openImages.Remove(this);
      Image bitmap = (Image) this.bitmap;
      using (ItemMonitor.Lock((object) bitmap))
        bitmap?.Dispose();
      base.Dispose(disposing);
    }

    public virtual Bitmap Bitmap
    {
      get
      {
        this.releaseTimeCounter = 0;
        return this.bitmap ?? this.UpdateImageFromData(this.data);
      }
      set
      {
        Image bitmap = (Image) this.bitmap;
        if (bitmap == value)
          return;
        if (value == null)
          this.UpdateDataFromImage();
        this.bitmap = value;
        if (bitmap == null)
          return;
        using (ItemMonitor.Lock((object) bitmap))
          bitmap.Dispose();
      }
    }

    public byte[] Data
    {
      get
      {
        this.UpdateDataFromImage();
        return this.data;
      }
    }

    public int TimeToStay
    {
      get => this.timeToStay;
      set => this.timeToStay = value;
    }

    public bool Optimized
    {
      get => this.optimized;
      set => this.optimized = value;
    }

    public virtual Size Size
    {
      get
      {
        Bitmap bitmap = this.Bitmap;
        if (bitmap != null)
        {
          int num = 0;
          while (this.size.IsEmpty)
          {
            if (num++ < 100)
            {
              try
              {
                this.size = bitmap.Size;
                break;
              }
              catch (InvalidOperationException ex)
              {
                Thread.Sleep(100);
              }
              catch (Exception ex)
              {
                break;
              }
            }
            else
              break;
          }
        }
        return this.size;
      }
    }

    public int Width => this.Size.Width;

    public int Height => this.Size.Height;

    public bool IsImage => this.bitmap != null;

    public bool IsValid => this.bitmap != null || this.data != null;

    public void Save(string file)
    {
      using (FileStream s = File.Create(file))
        this.Save((Stream) s);
    }

    public virtual byte[] ToBytes() => this.Data;

    public virtual void Save(Stream s) => s.Write(this.Data, 0, this.Data.Length);

    public Bitmap Detach()
    {
      Bitmap bitmap = this.Bitmap;
      this.bitmap = (Bitmap) null;
      return bitmap;
    }

    protected Bitmap UpdateImageFromData(byte[] imageData)
    {
      return this.Bitmap = this.OnCreateImage(imageData);
    }

    protected void UpdateDataFromImage()
    {
      using (ItemMonitor.Lock((object) this.bitmap))
      {
        if (this.bitmap == null)
          return;
        if (this.data != null)
          return;
        try
        {
          this.data = this.bitmap.ImageToJpegBytes();
        }
        catch
        {
        }
      }
    }

    protected virtual Bitmap OnCreateImage(byte[] data)
    {
      try
      {
        if (data != null)
          return BitmapExtensions.BitmapFromBytes(data);
      }
      catch (Exception ex)
      {
      }
      return (Bitmap) null;
    }

    protected virtual void OnReleaseImage()
    {
      try
      {
        this.Bitmap = (Bitmap) null;
      }
      catch (Exception ex)
      {
      }
      this.releaseTimeCounter = 0;
    }

    static MemoryOptimizedImage() => MemoryOptimizedImage.freeMemoryThread.Start();

    private static void FreeImageMemory()
    {
      while (!MemoryOptimizedImage.waitHandle.WaitOne(1000, false))
      {
        foreach (MemoryOptimizedImage memoryOptimizedImage in MemoryOptimizedImage.openImages.Lock<MemoryOptimizedImage>().ToArray<MemoryOptimizedImage>())
        {
          if (!memoryOptimizedImage.IsDisposed && memoryOptimizedImage.Optimized && memoryOptimizedImage.TimeToStay != 0 && memoryOptimizedImage.bitmap != null && memoryOptimizedImage.releaseTimeCounter++ > memoryOptimizedImage.TimeToStay)
            memoryOptimizedImage.OnReleaseImage();
        }
      }
    }

    public static implicit operator RendererImage(MemoryOptimizedImage image)
    {
      return (RendererImage) new MemoryOptimizedImage.RendererMemoryOptimizedImage(image);
    }

    private class RendererMemoryOptimizedImage : RendererImage
    {
      private readonly WeakReference<MemoryOptimizedImage> weakReference;

      public RendererMemoryOptimizedImage(MemoryOptimizedImage image)
      {
        this.weakReference = new WeakReference<MemoryOptimizedImage>(image);
      }

      public override bool IsValid => this.Moi != null && !this.Moi.IsDisposed;

      public override Bitmap Bitmap => this.Moi != null ? this.Moi.Bitmap : (Bitmap) null;

      public override Size Size => this.Moi != null ? this.Moi.Size : Size.Empty;

      public override bool Equals(object obj)
      {
        return obj is MemoryOptimizedImage.RendererMemoryOptimizedImage memoryOptimizedImage && memoryOptimizedImage.Moi == this.Moi;
      }

      public override int GetHashCode() => 0;

      private MemoryOptimizedImage Moi => this.weakReference.GetData<MemoryOptimizedImage>();

      public static implicit operator MemoryOptimizedImage.RendererMemoryOptimizedImage(
        MemoryOptimizedImage image)
      {
        return new MemoryOptimizedImage.RendererMemoryOptimizedImage(image);
      }
    }
  }
}

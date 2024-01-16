// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.ImageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Text;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public abstract class ImageProvider : FileProviderBase, IImageProvider, IDisposable
  {
    private const int sourceLockTimeout = 60000;
    private readonly object workLock = new object();
    private Thread parser;
    private volatile string source = string.Empty;
    private volatile ReaderWriterLock sourceLock;
    private volatile ImageProviderStatus status;
    private readonly List<ProviderImageInfo> imageInfos = new List<ProviderImageInfo>();
    private static readonly Dictionary<string, ImageProvider.ReaderWriterLockItem> rwLocks = new Dictionary<string, ImageProvider.ReaderWriterLockItem>();

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        using (ItemMonitor.Lock(this.workLock))
        {
          try
          {
            ThreadUtility.Abort(this.parser, 30000);
          }
          finally
          {
            this.Source = string.Empty;
          }
        }
      }
      base.Dispose(disposing);
    }

    public void Open(string source, bool async)
    {
      if (this.status != ImageProviderStatus.NotStarted)
        throw new InvalidOperationException("Provider already initialized");
      this.Source = source;
      this.status = ImageProviderStatus.Running;
      this.OnOpen();
      if (!async)
      {
        this.Parse();
      }
      else
      {
        this.parser = ThreadUtility.CreateWorkerThread("ImageProvider Parsing", new ThreadStart(this.Parse), ThreadPriority.BelowNormal);
        this.parser.Start();
      }
    }

    public void Open(bool async) => this.Open(this.Source, async);

    public Bitmap GetImage(int index)
    {
      using (ItemMonitor.Lock(this.workLock))
        return this.RetrieveSourceImage(index);
    }

    public byte[] GetByteImage(int index)
    {
      using (ItemMonitor.Lock(this.workLock))
        return this.RetrieveSourceByteImage(index);
    }

    public ThumbnailImage GetThumbnail(int index)
    {
      using (ItemMonitor.Lock(this.workLock))
        return this.RetrieveThumbnailImage(index);
    }

    public ProviderImageInfo GetImageInfo(int index)
    {
      if (index < 0 || index >= this.Count)
        return (ProviderImageInfo) null;
      using (ItemMonitor.Lock((object) this.imageInfos))
        return this.imageInfos[index];
    }

    public bool FastFormatCheck(string source)
    {
      using (ItemMonitor.Lock(this.workLock))
      {
        ReaderWriterLock readerWriterLock = ImageProvider.GetLock(source);
        try
        {
          try
          {
            readerWriterLock.AcquireReaderLock(60000);
          }
          catch (Exception ex)
          {
            return true;
          }
          try
          {
            return this.OnFastFormatCheck(source);
          }
          catch (Exception ex)
          {
            return true;
          }
          finally
          {
            readerWriterLock.ReleaseReaderLock();
          }
        }
        finally
        {
          ImageProvider.ReleaseLock(source);
        }
      }
    }

    public void ChangeSourceLocation(string newSourceLocation) => this.Source = newSourceLocation;

    public abstract string CreateHash();

    public string Source
    {
      get => this.source;
      set
      {
        ImageProvider.ReleaseLock(this.source);
        this.source = value;
        this.sourceLock = ImageProvider.GetLock(this.source);
      }
    }

    public ImageProviderStatus Status => this.status;

    public bool IsRetrievingIndex => this.status <= ImageProviderStatus.Running;

    public bool IsIndexRetrievalCompleted => this.status > ImageProviderStatus.Running;

    public int Count
    {
      get
      {
        using (ItemMonitor.Lock((object) this.imageInfos))
          return this.imageInfos.Count;
      }
    }

    public virtual ImageProviderCapabilities Capabilities => ImageProviderCapabilities.Nothing;

    private void Parse()
    {
      using (this.LockSource(true))
      {
        try
        {
          this.OnCheckSource();
          this.OnParse();
        }
        catch (Exception ex)
        {
        }
      }
      try
      {
        this.FireIndexRetrievalCompleted();
      }
      catch
      {
      }
    }

    private Bitmap RetrieveSourceImage(int index)
    {
      try
      {
        return BitmapExtensions.BitmapFromBytes(this.RetrieveSourceByteImage(index));
      }
      catch (Exception ex)
      {
        return (Bitmap) null;
      }
    }

    private byte[] RetrieveSourceByteImage(int n)
    {
      if (n < 0 || n >= this.Count)
        return (byte[]) null;
      byte[] data = (byte[]) null;
      using (this.LockSource(true))
      {
        try
        {
          data = this.OnRetrieveSourceByteImage(n);
          data = DjVuImage.ConvertToJpeg(data);
          data = WebpImage.ConvertToJpeg(data);
        }
        catch (Exception ex)
        {
        }
      }
      return data;
    }

    private ThumbnailImage RetrieveThumbnailImage(int n)
    {
      if (n < 0 || n >= this.Count)
        return (ThumbnailImage) null;
      ThumbnailImage thumbnailImage = (ThumbnailImage) null;
      using (this.LockSource(true))
      {
        try
        {
          thumbnailImage = this.OnRetrieveThumbnailImage(n);
        }
        catch (Exception ex)
        {
        }
      }
      return thumbnailImage;
    }

    protected static string CreateHashFromImageList(IEnumerable<ProviderImageInfo> images)
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
        {
          foreach (ProviderImageInfo image in images)
          {
            binaryWriter.Write(image.Name);
            binaryWriter.Write(image.Size);
          }
          binaryWriter.Flush();
          binaryWriter.Seek(0, SeekOrigin.Begin);
          return Base32.ToBase32String(new SHA1Managed().ComputeHash(binaryWriter.BaseStream));
        }
      }
    }

    protected IDisposable LockSource(bool readOnly)
    {
      try
      {
        if (readOnly)
        {
          this.sourceLock.AcquireReaderLock(60000);
          return (IDisposable) new Disposer((Action) (() => this.sourceLock.ReleaseReaderLock()), true);
        }
        this.sourceLock.AcquireWriterLock(60000);
        return (IDisposable) new Disposer((Action) (() => this.sourceLock.ReleaseWriterLock()), true);
      }
      catch (Exception ex)
      {
        return (IDisposable) null;
      }
    }

    protected virtual void OnOpen()
    {
    }

    protected virtual void OnCheckSource()
    {
      if (!File.Exists(this.Source))
        throw new ArgumentException("Source file does not exist!");
    }

    protected virtual bool OnFastFormatCheck(string source) => true;

    protected virtual ThumbnailImage OnRetrieveThumbnailImage(int index) => (ThumbnailImage) null;

    public virtual bool IsSlow => false;

    protected abstract void OnParse();

    protected abstract byte[] OnRetrieveSourceByteImage(int index);

    public event EventHandler<ImageIndexReadyEventArgs> ImageReady;

    public event EventHandler<IndexRetrievalCompletedEventArgs> IndexRetrievalCompleted;

    protected virtual void OnIndexReady(ImageIndexReadyEventArgs e)
    {
      if (this.ImageReady == null)
        return;
      this.ImageReady((object) this, e);
    }

    protected virtual void OnIndexRetrievalCompleted(IndexRetrievalCompletedEventArgs e)
    {
      if (this.IndexRetrievalCompleted == null)
        return;
      this.IndexRetrievalCompleted((object) this, e);
    }

    protected bool FireIndexReady(ProviderImageInfo ii)
    {
      using (ItemMonitor.Lock((object) this.imageInfos))
        this.imageInfos.Add(ii);
      ImageIndexReadyEventArgs e = new ImageIndexReadyEventArgs(this.Count - 1, ii);
      try
      {
        this.OnIndexReady(e);
      }
      catch (Exception ex)
      {
      }
      return !e.Cancel;
    }

    private void FireIndexRetrievalCompleted()
    {
      if (this.status == ImageProviderStatus.Running)
        this.status = this.Count == 0 ? ImageProviderStatus.Error : ImageProviderStatus.Completed;
      this.OnIndexRetrievalCompleted(new IndexRetrievalCompletedEventArgs(this.Status, this.Count));
    }

    private static ReaderWriterLock GetLock(string source)
    {
      if (string.IsNullOrEmpty(source))
        return (ReaderWriterLock) null;
      using (ItemMonitor.Lock((object) ImageProvider.rwLocks))
      {
        ImageProvider.ReaderWriterLockItem readerWriterLockItem;
        if (!ImageProvider.rwLocks.TryGetValue(source, out readerWriterLockItem))
          readerWriterLockItem = ImageProvider.rwLocks[source] = new ImageProvider.ReaderWriterLockItem();
        ++readerWriterLockItem.Count;
        return readerWriterLockItem.Lock;
      }
    }

    private static void ReleaseLock(string source)
    {
      using (ItemMonitor.Lock((object) ImageProvider.rwLocks))
      {
        ImageProvider.ReaderWriterLockItem readerWriterLockItem;
        if (!ImageProvider.rwLocks.TryGetValue(source, out readerWriterLockItem) || --readerWriterLockItem.Count != 0)
          return;
        ImageProvider.rwLocks.Remove(source);
      }
    }

    private class ReaderWriterLockItem
    {
      public int Count;
      public readonly ReaderWriterLock Lock = new ReaderWriterLock();
    }
  }
}

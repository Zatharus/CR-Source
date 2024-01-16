// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.SyncProviderBase
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Xml;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public abstract class SyncProviderBase : ISyncProvider
  {
    public const int MinimumAndroidFreeVersion = 100;
    public const int MinimumAndroidFullVersion = 89;
    public const int MinimumIOsVersion = 1;
    public const string SyncInformationFile = "sync_information.xml";
    public const string MarkerFile = "comicrack.ini";
    public const string SyncFormatExtension = ".cbp";
    private readonly ProcessingQueue<ComicBook> writeQueue = new ProcessingQueue<ComicBook>("Write books to Device");
    private Exception pendingException;
    private readonly object deviceAccessLock = new object();

    protected ComicBookCollection BooksOnDevice { get; set; }

    protected abstract void OnStart();

    protected abstract void OnCompleted();

    protected abstract bool FileExists(string file);

    protected abstract void WriteFile(string file, Stream data);

    protected abstract Stream ReadFile(string file);

    protected abstract void DeleteFile(string fileName);

    protected abstract long GetFreeSpace();

    protected abstract IEnumerable<string> GetFileList();

    protected virtual bool OnProgress(int percent) => true;

    public DeviceInfo Device { get; private set; }

    public virtual IEnumerable<ComicBook> GetBooks()
    {
      ComicBookCollection comicBookCollection = new ComicBookCollection();
      string[] array = this.GetFileList().Where<string>(new Func<string, bool>(SyncProviderBase.IsValidSyncFile)).ToArray<string>();
      for (int index = 0; index < array.Length; ++index)
      {
        ComicBook comicBook = this.DeserializeBook(array[index]);
        if (comicBook != null && comicBookCollection.FindItemById(comicBook.Id) == null)
        {
          comicBookCollection.Add(comicBook);
        }
        else
        {
          this.DeleteFile(array[index]);
          this.DeleteFile(SyncProviderBase.MakeSidecar(array[index]));
        }
      }
      return (IEnumerable<ComicBook>) (this.BooksOnDevice = comicBookCollection);
    }

    public virtual void ValidateDevice(DeviceInfo device)
    {
      int num;
      switch (device.Edition)
      {
        case SyncAppEdition.AndroidFree:
          device.BookSyncLimit = 100;
          num = 100;
          break;
        case SyncAppEdition.AndroidFull:
          num = 89;
          break;
        case SyncAppEdition.iOS:
          num = 1;
          break;
        default:
          num = int.MaxValue;
          break;
      }
      if (device.Version < num)
        throw new StorageSync.FatalSyncException("Invalid device");
    }

    public void Start() => this.OnStart();

    public void Add(
      ComicBook book,
      bool optimize,
      IPagePool pagePool,
      Action workingCallback,
      Action sendCallback,
      Action completedCallback)
    {
      if (this.pendingException != null)
      {
        Exception pendingException = this.pendingException;
        this.pendingException = (Exception) null;
        throw pendingException;
      }
      book = (ComicBook) book.Clone();
      book.Series = book.ShadowSeries;
      book.Title = book.ShadowTitle;
      book.Volume = book.ShadowVolume;
      book.Number = book.ShadowNumber;
      book.Count = book.ShadowCount;
      book.Format = book.ShadowFormat;
      ComicBook existing = this.BooksOnDevice.FindItemById(book.Id);
      if (existing != null && SyncProviderBase.PagesAreSame(existing, book))
      {
        if (SyncProviderBase.ContentIsSame(existing, book) && !existing.ComicInfoIsDirty)
        {
          if (completedCallback == null)
            return;
          this.writeQueue.AddItem(book, (AsyncCallback) (a => completedCallback()));
        }
        else
        {
          for (int page1 = 0; page1 < book.Pages.Count; ++page1)
          {
            ComicPageInfo page2 = book.GetPage(page1);
            existing.UpdatePageType(page1, page2.PageType);
            existing.UpdateBookmark(page1, page2.Bookmark);
          }
          book.Pages.Clear();
          book.Pages.AddRange((IEnumerable<ComicPageInfo>) existing.Pages);
          this.writeQueue.AddItem(book, (AsyncCallback) (a =>
          {
            using (ItemMonitor.Lock(this.deviceAccessLock))
              this.WriteBookInfo(book, Path.GetFileName(existing.FilePath));
            if (completedCallback == null)
              return;
            completedCallback();
          }));
        }
      }
      else
      {
        ExportSetting portableFormat = SyncProviderBase.GetPortableFormat(this.Device, optimize);
        string temp = EngineConfiguration.Default.GetTempFileName();
        string fileBaseName = portableFormat.GetTargetFileName(book, 0);
        portableFormat.ForcedName = Path.GetFileName(temp);
        portableFormat.TargetFolder = Path.GetDirectoryName(temp);
        while (this.writeQueue.Count > Math.Max(EngineConfiguration.Default.SyncQueueLength, 5))
          Thread.Sleep(1000);
        ComicExporter export = new ComicExporter(ListExtensions.AsEnumerable<ComicBook>(book), portableFormat, 0);
        export.Progress += (EventHandler<StorageProgressEventArgs>) ((s, e) =>
        {
          if (this.writeQueue.Count != 0 || workingCallback == null)
            return;
          workingCallback();
        });
        try
        {
          export.Export(pagePool);
        }
        catch (Exception ex)
        {
          FileUtility.SafeDelete(temp);
          throw new InvalidOperationException(book.Caption + ": " + ex.Message, ex);
        }
        this.writeQueue.AddItem(book, (AsyncCallback) (a =>
        {
          using (ItemMonitor.Lock(this.deviceAccessLock))
          {
            try
            {
              if (this.pendingException != null)
                return;
              if (completedCallback != null)
                sendCallback();
              if (existing != null)
                this.Remove(existing);
              long num = this.GetFreeSpace() - (long) EngineConfiguration.Default.FreeDeviceMemoryMB * 1024L * 1024L;
              if (FileUtility.GetSize(temp) > num)
                throw new StorageSync.DeviceOutOfSpaceException(StringUtility.Format(TR.Messages["DeviceOutOfSpace", "Device '{0}' does not have enough free space"], (object) this.Device.Name));
              string uniqueFileName = this.GetUniqueFileName(fileBaseName);
              using (FileStream data = File.OpenRead(temp))
                this.WriteFile(uniqueFileName, (Stream) data);
              book.Pages.Clear();
              book.Pages.AddRange((IEnumerable<ComicPageInfo>) export.ComicInfo.Pages);
              book.FilePath = uniqueFileName;
              this.WriteBookInfo(book, uniqueFileName);
              this.BooksOnDevice.Add(book);
              if (completedCallback == null)
                return;
              completedCallback();
            }
            catch (Exception ex)
            {
              this.pendingException = ex;
            }
            finally
            {
              FileUtility.SafeDelete(temp);
            }
          }
        }));
      }
    }

    public void WaitForWritesCompleted()
    {
      while (this.writeQueue.IsActive)
        Thread.Sleep(1000);
      if (this.pendingException != null)
      {
        Exception pendingException = this.pendingException;
        this.pendingException = (Exception) null;
        throw pendingException;
      }
    }

    public void Remove(ComicBook book)
    {
      ComicBook itemById = this.BooksOnDevice.FindItemById(book.Id);
      if (itemById == null)
        return;
      this.DeleteFile(itemById.FilePath);
      this.DeleteFile(SyncProviderBase.MakeSidecar(itemById.FilePath));
      this.BooksOnDevice.Remove(itemById);
    }

    public void SetLists(IEnumerable<ComicIdListItem> lists)
    {
      SyncProviderBase.SyncInformation data = new SyncProviderBase.SyncInformation();
      data.Lists.AddRange(lists.Select<ComicIdListItem, SyncProviderBase.SyncReadingList>((Func<ComicIdListItem, SyncProviderBase.SyncReadingList>) (cli => new SyncProviderBase.SyncReadingList(cli))).Where<SyncProviderBase.SyncReadingList>((Func<SyncProviderBase.SyncReadingList, bool>) (cli => cli.Books.Count > 0)));
      using (MemoryStream memoryStream = new MemoryStream())
      {
        XmlUtility.Store((Stream) memoryStream, (object) data, false);
        memoryStream.Position = 0L;
        this.WriteFile("sync_information.xml", (Stream) memoryStream);
      }
    }

    public bool Progress(int progress)
    {
      ThreadUtility.KeepAlive();
      return this.OnProgress(progress);
    }

    public void Completed()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (Stream stream = this.ReadFile("comicrack.ini"))
          stream.CopyTo((Stream) memoryStream);
        memoryStream.Position = 0L;
        this.WriteFile("comicrack.ini", (Stream) memoryStream);
      }
      this.OnCompleted();
    }

    private void WriteBookInfo(ComicBook book, string fileName)
    {
      book.ComicInfoIsDirty = false;
      using (MemoryStream data = new MemoryStream())
      {
        book.SerializeFull((Stream) data);
        data.Position = 0L;
        this.WriteFile(SyncProviderBase.MakeSidecar(fileName), (Stream) data);
      }
    }

    protected static bool IsValidSyncFile(string file)
    {
      return !string.IsNullOrEmpty(file) && string.Equals(Path.GetExtension(file), ".cbp", StringComparison.OrdinalIgnoreCase);
    }

    protected ComicBook DeserializeBook(string comicPath, Stream inputStream = null)
    {
      string str = SyncProviderBase.MakeSidecar(comicPath);
      try
      {
        using (Stream stream = inputStream ?? this.ReadFile(str))
        {
          ComicBook comicBook = ComicBook.DeserializeFull(stream);
          comicBook.FilePath = comicPath;
          return comicBook;
        }
      }
      catch (Exception ex)
      {
        if (inputStream == null)
        {
          this.DeleteFile(str);
          this.DeleteFile(comicPath);
        }
        return (ComicBook) null;
      }
    }

    protected bool ReadMarkerFile(string deviceKey)
    {
      try
      {
        using (Stream stream = this.ReadFile("comicrack.ini"))
          this.Device = new DeviceInfo((IDictionary<string, string>) IniFile.GetValues((TextReader) new StreamReader(stream)));
      }
      catch (Exception ex)
      {
        return false;
      }
      return deviceKey == null || this.Device.Key == deviceKey;
    }

    private string GetUniqueFileName(string baseName)
    {
      baseName = Path.GetFileNameWithoutExtension(baseName);
      string file = baseName + ".cbp";
      if (this.FileExists(file))
      {
        int num = 1;
        while (this.FileExists(file = string.Format("{0} ({1})", (object) baseName, (object) ++num) + ".cbp"))
          ;
      }
      return file;
    }

    public static bool PagesAreSame(ComicBook device, ComicBook library, bool withBookmarks = false)
    {
      ComicPageInfo[] array1 = device.Pages.ToArray();
      ComicPageInfo[] array2 = library.Pages.ToArray();
      if (array1.Length < array2.Length)
        return false;
      for (int index = 0; index < Math.Min(array1.Length, array2.Length); ++index)
      {
        ComicPageInfo comicPageInfo1 = array1[index];
        ComicPageInfo comicPageInfo2 = array2[index];
        if (comicPageInfo1.PageType != comicPageInfo2.PageType || comicPageInfo2.ImageHeight != 0 && comicPageInfo2.ImageWidth != 0 && comicPageInfo1.IsDoublePage != comicPageInfo2.IsDoublePage || comicPageInfo1.PagePosition != comicPageInfo2.PagePosition || withBookmarks && comicPageInfo1.Bookmark != comicPageInfo2.Bookmark)
          return false;
      }
      return true;
    }

    public static bool ContentIsSame(ComicBook a, ComicBook b)
    {
      return a.IsSameContent((ComicInfo) b, false) && (double) a.Rating == (double) b.Rating && a.OpenedCount == b.OpenedCount && a.LastPageRead == b.LastPageRead && a.OpenedTime == b.OpenedTime && a.AddedTime == b.AddedTime && a.ReleasedTime == b.ReleasedTime && SyncProviderBase.PagesAreSame(a, b, true);
    }

    public static string MakeSidecar(string fileName) => fileName + ".xml";

    public static ExportSetting GetPortableFormat(DeviceInfo device, bool optimized)
    {
      bool flag = device.Capabilites.HasFlag((Enum) DeviceCapabilites.WebP);
      ExportSetting exportSetting = new ExportSetting();
      exportSetting.Naming = ExportNaming.Caption;
      exportSetting.Target = ExportTarget.NewFolder;
      exportSetting.FormatId = 2;
      exportSetting.PageType = !flag || !EngineConfiguration.Default.SyncWebP ? StoragePageType.Jpeg : StoragePageType.Webp;
      exportSetting.EmbedComicInfo = false;
      exportSetting.AddKeyToPageInfo = true;
      exportSetting.Overwrite = true;
      exportSetting.RemovePages = false;
      exportSetting.Resampling = EngineConfiguration.Default.SyncResamping;
      ExportSetting portableFormat = exportSetting;
      if (optimized)
      {
        portableFormat.PageCompression = EngineConfiguration.Default.SyncOptimizeQuality;
        portableFormat.CreateThumbnails = EngineConfiguration.Default.SyncCreateThumbnails;
        portableFormat.PageResize = StoragePageResize.Height;
        portableFormat.PageHeight = EngineConfiguration.Default.SyncOptimizeMaxHeight;
        portableFormat.DontEnlarge = true;
        if (flag)
          portableFormat.PageType = EngineConfiguration.Default.SyncOptimizeWebP ? StoragePageType.Webp : StoragePageType.Jpeg;
        if (EngineConfiguration.Default.SyncOptimizeSharpen)
        {
          portableFormat.ImageProcessingSource = ExportImageProcessingSource.FromComic;
          portableFormat.ImageProcessing = new BitmapAdjustment(0.0f, 0.0f, 0.0f, 0.0f, BitmapAdjustmentOptions.None, 1);
        }
      }
      return portableFormat;
    }

    public class SyncReadingList
    {
      private readonly List<Guid> books = new List<Guid>();

      public SyncReadingList() => this.Description = string.Empty;

      public SyncReadingList(ComicIdListItem list, Func<Guid, bool> validate = null)
      {
        this.Name = list.Name;
        this.Description = list.Description;
        this.books.AddRange(list.BookIds.Where<Guid>((Func<Guid, bool>) (id => validate == null || validate(id))));
      }

      [XmlAttribute]
      public string Name { get; set; }

      [DefaultValue("")]
      public string Description { get; set; }

      [XmlArrayItem("Id")]
      public List<Guid> Books => this.books;
    }

    public class SyncInformation
    {
      private readonly List<SyncProviderBase.SyncReadingList> lists = new List<SyncProviderBase.SyncReadingList>();

      public SyncInformation()
      {
        this.Version = 1;
        this.Name = "ComicRack";
      }

      public string Name { get; set; }

      public int Version { get; set; }

      [XmlArrayItem("List")]
      public List<SyncProviderBase.SyncReadingList> Lists => this.lists;
    }
  }
}

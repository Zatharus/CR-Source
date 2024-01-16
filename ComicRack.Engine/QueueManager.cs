// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.QueueManager
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Win32;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Engine.Sync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class QueueManager : DisposableObject
  {
    private ComicScanner scanner;
    private readonly SmartList<ComicExporter> exportErrors = new SmartList<ComicExporter>();
    private readonly SmartList<DeviceSyncError> deviceSyncErrors = new SmartList<DeviceSyncError>();
    private static string refreshInfoQueueMessage;
    private static string writeInfoQueueMessage;
    private static string fastThumbanilQueueMessage;
    private static string slowThumbnailQueueMessage;
    private static string getImageQueueMessage;
    private static string updateDynamicQueueMessage;
    private static string exportQueueMessage;
    private static string exportAbortText;
    private static string scanComicQueueMessage;
    private static string scanComicAbortText;
    private static string deviceSyncQueueMessage;
    private static string deviceSyncAbortText;
    private static string taskGroupLoadThumbnails;
    private static string taskGroupCreateThumbnails;
    private static string taskGroupLoadPages;
    private static string taskGroupCreatePages;
    private static string taskGroupReadInfo;
    private static string taskGroupWriteInfo;
    private static string taskGroupUpdateDynamic;
    private static string taskGroupExport;
    private static string taskGroupScanning;
    private static string taskGroupDeviceSync;

    public QueueManager(
      DatabaseManager databaseManager,
      CacheManager cacheManager,
      IComicUpdateSettings settings,
      IEnumerable<DeviceSyncSettings> devices)
    {
      this.DatabaseManager = databaseManager;
      this.CacheManager = cacheManager;
      this.Settings = settings;
      this.Devices = devices;
      this.UpdateComicBookDynamicQueue = new ProcessingQueue<ComicBook>("Update Dynamic Books", ThreadPriority.Lowest)
      {
        DefaultProcessingQueueAddMode = ProcessingQueueAddMode.AddToTop
      };
      this.ExportComicsQueue = new ProcessingQueue<ComicBook>("Export Books", ThreadPriority.Lowest);
      this.ReadComicBookInfoFileQueue = new ProcessingQueue<ComicBook>("Read Book File Information", ThreadPriority.Lowest);
      this.WriteComicBookInfoFileQueue = new ProcessingQueue<ComicBook>("Write Book File Information", ThreadPriority.Lowest)
      {
        DefaultProcessingQueueAddMode = ProcessingQueueAddMode.AddToTop
      };
      this.DeviceSyncQueue = new ProcessingQueue<DeviceSyncSettings>("Synchronizing Devices", ThreadPriority.Lowest);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.scanner != null)
          this.Scanner.Dispose();
        this.UpdateComicBookDynamicQueue.Dispose();
        this.ExportComicsQueue.Dispose();
        this.ReadComicBookInfoFileQueue.Dispose();
        this.WriteComicBookInfoFileQueue.Stop(false, 60000);
        this.WriteComicBookInfoFileQueue.Dispose();
        this.DeviceSyncQueue.Dispose();
      }
      base.Dispose(disposing);
    }

    public DatabaseManager DatabaseManager { get; private set; }

    public CacheManager CacheManager { get; private set; }

    public IComicUpdateSettings Settings { get; private set; }

    public IEnumerable<DeviceSyncSettings> Devices { get; private set; }

    public ComicScanner Scanner
    {
      get
      {
        if (this.scanner == null)
        {
          this.scanner = new ComicScanner(this.DatabaseManager.BookFactory);
          this.scanner.ScanNotify += (EventHandler<ComicScanNotifyEventArgs>) ((s, e) =>
          {
            if (this.ComicScanned == null)
              return;
            this.ComicScanned(s, e);
          });
        }
        return this.scanner;
      }
    }

    public ProcessingQueue<ComicBook> UpdateComicBookDynamicQueue { get; private set; }

    public ProcessingQueue<ComicBook> ExportComicsQueue { get; private set; }

    public ProcessingQueue<ComicBook> ReadComicBookInfoFileQueue { get; private set; }

    public ProcessingQueue<ComicBook> WriteComicBookInfoFileQueue { get; private set; }

    public ProcessingQueue<DeviceSyncSettings> DeviceSyncQueue { get; private set; }

    public SmartList<ComicExporter> ExportErrors => this.exportErrors;

    public SmartList<DeviceSyncError> DeviceSyncErrors => this.deviceSyncErrors;

    public event EventHandler<ComicScanNotifyEventArgs> ComicScanned;

    public int PendingComicConversions => this.ExportComicsQueue.Count;

    public bool IsInComicConversion => this.ExportComicsQueue.IsActive;

    public bool IsInComicFileRefresh
    {
      get => this.ReadComicBookInfoFileQueue.IsActive || this.UpdateComicBookDynamicQueue.IsActive;
    }

    public bool IsInComicFileUpdate => this.WriteComicBookInfoFileQueue.IsActive;

    public int PendingDeviceSyncs => this.DeviceSyncQueue.Count;

    public bool IsInDeviceSync => this.DeviceSyncQueue.IsActive;

    public bool IsActive
    {
      get => this.IsInComicFileUpdate || this.IsInComicConversion || this.IsInDeviceSync;
    }

    public void StartScan(bool all, bool removeMissing)
    {
      foreach (WatchFolder watchFolder in (SmartList<WatchFolder>) this.DatabaseManager.Database.WatchFolders)
        this.Scanner.ScanFileOrFolder(watchFolder.Folder, all, removeMissing);
    }

    public void AddBookToDynamicUpdate(ComicBook cb, bool refresh)
    {
      if (cb == null || !cb.IsDynamicSource)
        return;
      this.UpdateComicBookDynamicQueue.AddItem(cb, (AsyncCallback) (ar =>
      {
        if (refresh)
          this.CacheManager.ImagePool.RemoveImages(cb.FilePath, -1);
        if (!refresh && !cb.EnableDynamicUpdate)
          return;
        this.CacheManager.ImagePool.RefreshLastImage(cb.FilePath);
        cb.UpdateDynamicPageCount(refresh, ar as IProgressState);
      }));
    }

    public void ExportComic(ComicBook cb, ExportSetting setting, int sequence)
    {
      this.ExportComic((IEnumerable<ComicBook>) new ComicBook[1]
      {
        cb
      }, setting, sequence);
    }

    public void ExportComic(IEnumerable<ComicBook> cbs, ExportSetting setting, int sequence)
    {
      ComicBook kcb = cbs.FirstOrDefault<ComicBook>();
      if (kcb == null || cbs.Any<ComicBook>((Func<ComicBook, bool>) (cb => cb == null || !cb.EditMode.CanExport())))
        return;
      this.ExportComicsQueue.AddItem(kcb, (AsyncCallback) (ar =>
      {
        foreach (ComicBook cb in cbs)
          cb.RefreshInfoFromFile();
        ComicExporter comicExporter = new ComicExporter(cbs, setting, sequence);
        try
        {
          bool flag1 = kcb.EditMode.IsLocalComic();
          bool flag2 = setting.Target == ExportTarget.ReplaceSource;
          IProgressState ps = ar as IProgressState;
          if (ps != null)
          {
            ps.ProgressAvailable = true;
            comicExporter.Progress += (EventHandler<StorageProgressEventArgs>) ((s, e) =>
            {
              ps.ProgressPercentage = e.PercentDone;
              e.Cancel = ps.Abort;
            });
          }
          IEnumerable<string> array = (IEnumerable<string>) cbs.Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.EditMode.IsLocalComic())).Select<ComicBook, string>((Func<ComicBook, string>) (cb => cb.FilePath)).ToArray<string>();
          string outPath = comicExporter.Export((IPagePool) this.CacheManager.ImagePool);
          if (outPath == null)
            return;
          IEnumerable<string> strings = array.Where<string>((Func<string, bool>) (p => !string.Equals(p, outPath, StringComparison.OrdinalIgnoreCase)));
          if (flag1 & flag2)
          {
            kcb.FilePath = outPath;
            kcb.RefreshFileProperties();
            kcb.SetInfo(comicExporter.ComicInfo, false, true);
            kcb.LastPageRead = kcb.LastPageRead.Clamp(0, kcb.PageCount - 1);
            kcb.CurrentPage = kcb.CurrentPage.Clamp(0, kcb.PageCount - 1);
            if (setting.ImageProcessingSource == ExportImageProcessingSource.FromComic)
              kcb.ColorAdjustment = new BitmapAdjustment();
            foreach (string file in strings)
            {
              ShellFile.DeleteFile(file);
              this.DatabaseManager.Database.Books.Remove(file);
            }
          }
          else
          {
            if (setting.DeleteOriginal & flag1)
            {
              foreach (string file in strings)
              {
                ShellFile.DeleteFile(file);
                this.DatabaseManager.Database.Books.Remove(file);
              }
            }
            if (!(setting.AddToLibrary | flag2))
              return;
            this.DatabaseManager.BookFactory.Create(outPath, CreateBookOption.AddToStorage, RefreshInfoOptions.DontReadInformation)?.SetInfo(comicExporter.ComicInfo, false, true);
          }
        }
        catch
        {
          this.ExportErrors.Add(comicExporter);
        }
      }));
    }

    public void AddBookToRefreshComicData(ComicBook cb)
    {
      if (cb == null || !cb.EditMode.IsLocalComic())
        return;
      this.ReadComicBookInfoFileQueue.AddItem(cb, (AsyncCallback) (ar =>
      {
        try
        {
          cb.RefreshInfoFromFile(RefreshInfoOptions.GetPageCount);
        }
        catch (Exception ex)
        {
        }
      }));
    }

    public void AddBookToFileUpdate(ComicBook cb, bool alwaysWrite)
    {
      if (cb == null || !cb.IsLinked || !cb.ComicInfoIsDirty || !cb.FileInfoRetrieved || !this.Settings.UpdateComicFiles || !(this.Settings.AutoUpdateComicsFiles | alwaysWrite))
        return;
      this.WriteComicBookInfoFileQueue.AddItem(cb, (AsyncCallback) (ar =>
      {
        if (!cb.ComicInfoIsDirty || !this.Settings.UpdateComicFiles || !(this.Settings.AutoUpdateComicsFiles | alwaysWrite))
          return;
        cb.ComicInfoIsDirty = false;
        this.WriteInfoToFileWithCacheUpdate(cb);
      }));
    }

    public void AddBookToFileUpdate(ComicBook cb) => this.AddBookToFileUpdate(cb, false);

    public void WriteInfoToFileWithCacheUpdate(ComicBook cb)
    {
      try
      {
        while (this.CacheManager.ImagePool.AreImagesPending(cb.FilePath))
          Thread.Sleep(1000);
        long oldSize = cb.FileSize;
        DateTime oldWrite = cb.FileModifiedTime;
        if (!cb.WriteInfoToFile(false))
          return;
        this.CacheManager.ImagePool.Pages.UpdateKeys((Func<ImageKey, bool>) (key => key.IsSameFile(cb.FilePath, oldSize, oldWrite)), (Action<ImageKey>) (key => key.UpdateFileInfo()));
        this.CacheManager.ImagePool.Thumbs.UpdateKeys((Func<ImageKey, bool>) (key => key.IsSameFile(cb.FilePath, oldSize, oldWrite)), (Action<ImageKey>) (key => key.UpdateFileInfo()));
        cb.RefreshFileProperties();
      }
      catch (Exception ex)
      {
      }
    }

    public bool SynchronizeDevices()
    {
      foreach (DeviceSyncSettings device in this.Devices)
        this.SynchronizeDevice(device);
      return this.Devices.Any<DeviceSyncSettings>();
    }

    public void SynchronizeDevice(string key, IPAddress address)
    {
      this.SynchronizeDevice(this.Devices.FirstOrDefault<DeviceSyncSettings>((Func<DeviceSyncSettings, bool>) (s => s.DeviceKey == key)), address);
    }

    public void SynchronizeDevice(DeviceSyncSettings dss, IPAddress address = null)
    {
      if (dss == null)
        return;
      DeviceSyncSettings ssc = new DeviceSyncSettings(dss);
      ComicBookContainer library = new ComicBookContainer();
      library.Books.AddRange((IEnumerable<ComicBook>) this.DatabaseManager.Database.Books);
      this.DeviceSyncQueue.AddItem(ssc, (AsyncCallback) (ar =>
      {
        try
        {
          ISyncProvider provider = address != null ? (ISyncProvider) new WirelessSyncProvider(address, ssc.DeviceKey) : DeviceSyncFactory.Create(ssc.DeviceKey);
          if (provider == null)
            return;
          StorageSync storageSync = new StorageSync(provider);
          storageSync.Error += (EventHandler<StorageSync.SyncErrorEventArgs>) ((s, e) => this.DeviceSyncErrors.Add(new DeviceSyncError(ssc.DeviceName, e.Message)));
          storageSync.Synchronize(ssc, library, this.DatabaseManager.Database.ComicLists, (IPagePool) this.CacheManager.ImagePool, ar as IProgressState);
        }
        catch (Exception ex)
        {
        }
      }));
    }

    public IEnumerable<QueueManager.IPendingTasks> GetQueues()
    {
      List<QueueManager.IPendingTasks> queues = new List<QueueManager.IPendingTasks>();
      if (QueueManager.exportQueueMessage == null)
      {
        QueueManager.exportQueueMessage = TR.Messages["ExportQueueMessage", "Export Book '{0}'"];
        QueueManager.refreshInfoQueueMessage = TR.Messages["RefreshInfoQueueMessage", "Refresh information for Book '{0}'"];
        QueueManager.writeInfoQueueMessage = TR.Messages["WriteInfoQueueMessage", "Write information to Book file '{0}'"];
        QueueManager.fastThumbanilQueueMessage = TR.Messages["FastThumbanilQueueMessage", "Retrieve cached thumbnail for page {0} in file '{1}'"];
        QueueManager.slowThumbnailQueueMessage = TR.Messages["SlowThumbnailQueueMessage", "Create thumbnail for page {0} in file '{1}'"];
        QueueManager.getImageQueueMessage = TR.Messages["GetImageQueueMessage", "Get page {0} in file '{1}'"];
        QueueManager.scanComicQueueMessage = TR.Messages["ScanComicQueueMessage", "Scanning '{0}'"];
        QueueManager.deviceSyncQueueMessage = TR.Messages["DeviceSyncQueueMessage", "Syncing Device '{0}'"];
        QueueManager.updateDynamicQueueMessage = TR.Messages["UpdateComicDynamicMessage", "Updating Web Comic '{0}'"];
        QueueManager.exportAbortText = TR.Messages["AbortExport", "Abort Export"];
        QueueManager.scanComicAbortText = TR.Messages["AbortScan", "Abort Scanning"];
        QueueManager.deviceSyncAbortText = TR.Messages["AbortDeviceSync", "Abort syncinc Devices"];
        QueueManager.taskGroupLoadThumbnails = TR.Messages["TaskGroupLoadThumbnails", "Load Thumbnails"];
        QueueManager.taskGroupCreateThumbnails = TR.Messages["TaskGroupCreateThumbnails", "Create Thumbnails"];
        QueueManager.taskGroupLoadPages = TR.Messages["TaskGroupLoadPages", "Load Pages"];
        QueueManager.taskGroupCreatePages = TR.Messages["TaskGroupCreatePages", "Create Pages"];
        QueueManager.taskGroupReadInfo = TR.Messages["TaskGroupReadInfo", "Read Info"];
        QueueManager.taskGroupWriteInfo = TR.Messages["TaskGroupWriteInfo", "Write Info"];
        QueueManager.taskGroupUpdateDynamic = TR.Messages["TaskGroupUpdateDynamic", "Update Web Comics"];
        QueueManager.taskGroupExport = TR.Messages["TaskGroupExport", "Export Books"];
        QueueManager.taskGroupScanning = TR.Messages["TaskGroupScanning", "Scanning"];
        QueueManager.taskGroupDeviceSync = TR.Messages["TaskGroupDeviceSync", "Syncing Devices"];
      }
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<ImageKey>("ReadPagesAnimation", QueueManager.taskGroupLoadThumbnails, this.CacheManager.ImagePool.FastThumbnailQueue, (Func<IProcessingItem<ImageKey>, object>) (ik => (object) new QueueManager.TaskInfo((IProgressState) ik, StringUtility.Format(QueueManager.fastThumbanilQueueMessage, (object) (ik.Item.Index + 1), (object) Path.GetFileName(ik.Item.Location))))));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<ImageKey>("ReadPagesAnimation", QueueManager.taskGroupCreateThumbnails, this.CacheManager.ImagePool.SlowThumbnailQueue, (Func<IProcessingItem<ImageKey>, object>) (ik => (object) new QueueManager.TaskInfo((IProgressState) ik, StringUtility.Format(QueueManager.slowThumbnailQueueMessage, (object) (ik.Item.Index + 1), (object) Path.GetFileName(ik.Item.Location))))));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<ImageKey>("ReadPagesAnimation", QueueManager.taskGroupCreatePages, this.CacheManager.ImagePool.SlowPageQueue, (Func<IProcessingItem<ImageKey>, object>) (ik => (object) new QueueManager.TaskInfo((IProgressState) ik, StringUtility.Format(QueueManager.getImageQueueMessage, (object) (ik.Item.Index + 1), (object) Path.GetFileName(ik.Item.Location))))));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<ImageKey>("ReadPagesAnimation", QueueManager.taskGroupLoadPages, this.CacheManager.ImagePool.FastPageQueue, (Func<IProcessingItem<ImageKey>, object>) (ik => (object) new QueueManager.TaskInfo((IProgressState) ik, StringUtility.Format(QueueManager.getImageQueueMessage, (object) (ik.Item.Index + 1), (object) Path.GetFileName(ik.Item.Location))))));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<ComicBook>("ReadInfoAnimation", QueueManager.taskGroupReadInfo, this.ReadComicBookInfoFileQueue, (Func<IProcessingItem<ComicBook>, object>) (cb => (object) new QueueManager.TaskInfo((IProgressState) cb, StringUtility.Format(QueueManager.refreshInfoQueueMessage, (object) cb.Item.Caption)))));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<ComicBook>("UpdateInfoAnimation", QueueManager.taskGroupWriteInfo, this.WriteComicBookInfoFileQueue, (Func<IProcessingItem<ComicBook>, object>) (cb => (object) new QueueManager.TaskInfo((IProgressState) cb, StringUtility.Format(QueueManager.writeInfoQueueMessage, (object) cb.Item.Caption)))));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<ComicBook>("ReadInfoAnimation", QueueManager.taskGroupUpdateDynamic, this.UpdateComicBookDynamicQueue, (Func<IProcessingItem<ComicBook>, object>) (cb => (object) new QueueManager.TaskInfo((IProgressState) cb, StringUtility.Format(QueueManager.updateDynamicQueueMessage, (object) cb.Item.Caption)))));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<ComicBook>("ExportAnimation", QueueManager.taskGroupExport, this.ExportComicsQueue, (Func<IProcessingItem<ComicBook>, object>) (cb => (object) new QueueManager.TaskInfo((IProgressState) cb, StringUtility.Format(QueueManager.exportQueueMessage, (object) cb.Item.Caption))), QueueManager.exportAbortText, new Action(this.ExportComicsQueue.Clear)));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo<DeviceSyncSettings>("DeviceSyncAnimation", QueueManager.taskGroupDeviceSync, this.DeviceSyncQueue, (Func<IProcessingItem<DeviceSyncSettings>, object>) (sds => (object) new QueueManager.TaskInfo((IProgressState) sds, StringUtility.Format(QueueManager.deviceSyncQueueMessage, (object) sds.Item.DeviceName))), QueueManager.deviceSyncAbortText, new Action(this.DeviceSyncQueue.Clear)));
      queues.Add((QueueManager.IPendingTasks) new QueueManager.PendingTasksInfo("ScanAnimation", QueueManager.taskGroupScanning, (Func<IList>) (() =>
      {
        if (!this.Scanner.IsScanning)
          return (IList) new string[0];
        return (IList) new string[1]
        {
          StringUtility.Format(QueueManager.scanComicQueueMessage, (object) this.Scanner.CurrentLocation)
        };
      }), QueueManager.scanComicAbortText, (Action) (() => this.Scanner.Stop(true))));
      return (IEnumerable<QueueManager.IPendingTasks>) queues;
    }

    public interface IPendingTasks
    {
      IList GetPendingItems();

      string TasksImageKey { get; }

      string Group { get; }

      string AbortCommandText { get; }

      Action Abort { get; }
    }

    private class PendingTasksInfo : QueueManager.IPendingTasks
    {
      private readonly Func<IList> handler;

      public PendingTasksInfo(
        string imageKey,
        string group,
        Func<IList> infoHandler,
        string abortCommandText,
        Action abort)
      {
        this.TasksImageKey = imageKey;
        this.handler = infoHandler;
        this.AbortCommandText = abortCommandText;
        this.Abort = abort;
        this.Group = group;
      }

      public string TasksImageKey { get; private set; }

      public string AbortCommandText { get; private set; }

      public Action Abort { get; private set; }

      public string Group { get; private set; }

      public IList GetPendingItems() => this.handler();
    }

    private class PendingTasksInfo<T> : QueueManager.PendingTasksInfo
    {
      public PendingTasksInfo(
        string imageKey,
        string group,
        ProcessingQueue<T> queue,
        Func<IProcessingItem<T>, object> descriptionHandler,
        string abortText = null,
        Action abort = null)
        : base(imageKey, group, (Func<IList>) (() => (IList) queue.PendingItemInfos.Select<IProcessingItem<T>, object>(descriptionHandler).ToList<object>()), abortText, abort)
      {
      }
    }

    private class TaskInfo : IProgressState
    {
      private readonly string text;

      public TaskInfo(IProgressState ps, string text)
      {
        this.text = text;
        this.State = ps.State;
        this.ProgressAvailable = ps.ProgressAvailable;
        this.ProgressPercentage = ps.ProgressPercentage;
        this.ProgressMessage = ps.ProgressMessage;
      }

      public override string ToString()
      {
        string str = this.text;
        if (!string.IsNullOrEmpty(this.ProgressMessage))
          str = str + " - " + this.ProgressMessage;
        return str;
      }

      public ProgressState State { get; set; }

      public int ProgressPercentage { get; set; }

      public string ProgressMessage { get; set; }

      public bool ProgressAvailable { get; set; }

      public bool Abort { get; set; }
    }
  }
}

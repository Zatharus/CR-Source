// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicScanner
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicScanner : DisposableObject
  {
    private Thread scanningThread;
    private readonly ComicBookFactory factory;
    private bool scanCompleted;
    private readonly SmartList<ScanItem> scanQueue = new SmartList<ScanItem>();
    private volatile ComicScanOptions scanOptions;
    private volatile string currentLocation;
    private volatile bool abortScanning;

    public ComicScanner(ComicBookFactory factory) => this.factory = factory;

    public bool IsScanning
    {
      get => this.scanningThread != null && this.scanningThread.IsAlive && !this.scanCompleted;
    }

    public SmartList<ScanItem> ScanQueue => this.scanQueue;

    public ComicScanOptions ScanOptions
    {
      get => this.scanOptions;
      set => this.scanOptions = value;
    }

    public string CurrentLocation => this.currentLocation;

    public void Scan()
    {
      if (this.IsScanning)
        return;
      this.scanCompleted = false;
      this.scanningThread = ThreadUtility.CreateWorkerThread("Book Scanner", new ThreadStart(this.ScanFolderQueue), ThreadPriority.Lowest);
      this.scanningThread.Start();
    }

    public void ScanFileOrFolder(string fileOrFolder, bool all, bool removeMissing)
    {
      if (string.IsNullOrEmpty(fileOrFolder))
        return;
      this.scanQueue.Add((ScanItem) new ScanItemFileOrFolder(fileOrFolder, all, removeMissing));
      this.Scan();
    }

    public void ScanFilesOrFolders(
      IEnumerable<string> filesOrFolders,
      bool all,
      bool removeMissing)
    {
      filesOrFolders.ForEach<string>((Action<string>) (folder => this.ScanFileOrFolder(folder, all, removeMissing)));
    }

    public void Stop(bool clearQueue)
    {
      if (this.IsScanning)
      {
        this.abortScanning = true;
        if (!this.scanningThread.Join(10000))
        {
          this.scanningThread.Abort();
          this.scanningThread.Join();
        }
      }
      if (clearQueue)
        this.scanQueue.Clear();
      this.scanningThread = (Thread) null;
    }

    private void ScanFolderQueue()
    {
      bool flag1 = false;
      bool flag2 = false;
      try
      {
        this.scanCompleted = false;
        while (this.scanQueue.Count > 0)
        {
          ScanItem scan = this.scanQueue[0];
          flag2 |= scan.AutoRemove;
          try
          {
            foreach (string scanFile in scan.GetScanFiles())
            {
              try
              {
                this.currentLocation = Path.GetFullPath(scanFile);
                if (File.Exists(scanFile))
                {
                  ComicScanNotifyEventArgs e = new ComicScanNotifyEventArgs(scanFile);
                  this.OnScanNotify(e);
                  if (e.Cancel || this.abortScanning)
                  {
                    flag1 = e.ClearQueue;
                    return;
                  }
                  if (!e.IgnoreFile)
                    this.OnProcessScannedFile(scanFile, this.scanOptions);
                }
              }
              catch (Exception ex)
              {
              }
            }
          }
          catch
          {
          }
          this.scanQueue.Remove(scan);
        }
        if (!flag2)
          return;
        DriveChecker driveChecker = new DriveChecker();
        List<ComicBook> list = (List<ComicBook>) null;
        foreach (ComicBook comicBook in this.factory.Storage.ToArray())
        {
          if (comicBook.IsLinked)
          {
            string filePath = comicBook.FilePath;
            if (driveChecker.IsConnected(filePath) && !File.Exists(filePath))
            {
              if (list == null)
                list = new List<ComicBook>();
              list.Add(comicBook);
            }
            ComicScanNotifyEventArgs e = new ComicScanNotifyEventArgs(filePath);
            this.OnScanNotify(e);
            if (e.Cancel || this.abortScanning)
            {
              flag1 = e.ClearQueue;
              return;
            }
          }
        }
        if (list == null)
          return;
        this.factory.Storage.RemoveRange((IEnumerable<ComicBook>) list);
      }
      catch (ThreadAbortException ex)
      {
      }
      finally
      {
        this.scanCompleted = true;
        this.OnScanNotify(new ComicScanNotifyEventArgs(string.Empty));
        if (flag1)
          this.scanQueue.Clear();
      }
    }

    protected virtual void OnScanNotify(ComicScanNotifyEventArgs e)
    {
      if (this.ScanNotify == null)
        return;
      this.ScanNotify((object) this, e);
    }

    protected virtual void OnProcessScannedFile(string file, ComicScanOptions scanOptions)
    {
      ComicBook comicBook = this.factory.Create(file, CreateBookOption.DoNotAdd, RefreshInfoOptions.DontReadInformation);
      if (comicBook == null || comicBook.IsInContainer)
        return;
      ComicBook itemByFileNameSize = this.factory.Storage.FindItemByFileNameSize(comicBook.FilePath);
      if (itemByFileNameSize != null && !File.Exists(itemByFileNameSize.FilePath))
      {
        itemByFileNameSize.FilePath = comicBook.FilePath;
      }
      else
      {
        if (this.factory.Storage[comicBook.FilePath] != null)
          return;
        comicBook.AddedTime = DateTime.Now;
        comicBook.RefreshInfoFromFile();
        this.factory.Storage.Add(comicBook);
      }
    }

    public event EventHandler<ComicScanNotifyEventArgs> ScanNotify;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.Stop(true);
      base.Dispose(disposing);
    }
  }
}

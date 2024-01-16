// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicDatabase
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Threading;
using cYo.Common.Windows.Forms;
using cYo.Common.Xml;
using cYo.Projects.ComicRack.Engine.Database.Storage;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class ComicDatabase : ComicLibrary, IBlackList
  {
    private readonly WatchFolderCollection ownWatchFolders;
    [NonSerialized]
    private ComicStorage comicStorage;
    [NonSerialized]
    private ComicBookContainerUndo undo;
    private WatchFolderCollection watchFolders = new WatchFolderCollection();
    private HashSet<string> blackList = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public static int BackupCount = 10;
    private const string BackupDatabaseName = "ComicDb.xml";
    private const string ThumbnailsBackupFolder = "Thumbnails";

    public ComicDatabase(bool enableWatchfolders)
    {
      this.ownWatchFolders = this.watchFolders;
      if (enableWatchfolders)
        this.watchFolders.Changed += new EventHandler<SmartListChangedEventArgs<WatchFolder>>(this.watchFolders_Changed);
      this.Undo = new ComicBookContainerUndo()
      {
        Container = (ComicBookContainer) this
      };
    }

    public ComicDatabase()
      : this(true)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.ownWatchFolders.Changed -= new EventHandler<SmartListChangedEventArgs<WatchFolder>>(this.watchFolders_Changed);
        this.ownWatchFolders.ForEach((Action<WatchFolder>) (wf => wf.Dispose()));
        if (this.ComicStorage != null)
          this.ComicStorage.Dispose();
      }
      base.Dispose(disposing);
    }

    public void FinalizeLoading()
    {
      if (this.Books.Count > 0)
      {
        int count = this.Books.Count;
        int n = 0;
        AutomaticProgressDialog.Process((Form) null, TR.Messages["CheckingDatabase", "Checking Database"], TR.Messages["CheckingDatabaseText", "Checking and updating missing data in Database"], 10000, (Action) (() => this.Books.ForEach((Action<ComicBook>) (cb =>
        {
          cb.ValidateData();
          cb.FileInfoRetrieved = true;
          AutomaticProgressDialog.Value = n++ * 100 / count;
        }))), AutomaticProgressDialogOptions.None);
      }
      if (ComicLibrary.IsQueryCacheEnabled)
        this.ComicLists.GetItems<ComicListItem>().Where<ComicListItem>((Func<ComicListItem, bool>) (cli => cli.NewBookCount > 0 && cli.NewBookCountDate.CompareTo(DateTime.UtcNow, true) != 0)).ForEach<ComicListItem>((Action<ComicListItem>) (cli => cli.NotifyCacheRetrieval()));
      this.IsLoaded = true;
      this.TemporaryFolder.Items.Clear();
      this.TemporaryFolder.Refresh();
      this.IsDirty = false;
    }

    public void CreateDummyEntries(string baseName, int count, int pages)
    {
      for (int index = 0; index < count; ++index)
      {
        ComicBook comicBook1 = new ComicBook();
        comicBook1.Number = (index + 1).ToString();
        comicBook1.Series = baseName + " Series";
        comicBook1.Volume = 1;
        comicBook1.Year = DateTime.Now.Year;
        comicBook1.Month = index % 12 + 1;
        comicBook1.Day = 1;
        comicBook1.Count = count;
        comicBook1.Writer = "John Writer";
        comicBook1.Inker = "John Inker";
        comicBook1.Penciller = "John Penciller";
        comicBook1.Publisher = baseName + " Publisher";
        comicBook1.Notes = "This is a dummy Book entry";
        ComicBook comicBook2 = comicBook1;
        comicBook2.Title = "Title of " + comicBook2.Series + " Book " + comicBook2.Number;
        comicBook2.FilePath = string.Format("C:\\Documents and Settings\\Documents\\Books\\{0} Series\\{1} #{2} of {3} ({4}) - {5}.cbz", (object) comicBook2.Series, (object) comicBook2.Series, (object) comicBook2.Number, (object) comicBook2.Count, (object) comicBook2.Year, (object) comicBook2.Publisher);
        comicBook2.PageCount = pages;
        for (int page = 0; page < pages; ++page)
          comicBook2.UpdatePageSize(page, 800, 600);
        comicBook2.Pages.TrimExcess();
        this.Books.Add(comicBook2);
      }
    }

    [XmlIgnore]
    public ComicStorage ComicStorage
    {
      get => this.comicStorage;
      set => this.comicStorage = value;
    }

    [XmlIgnore]
    public ComicBookContainerUndo Undo
    {
      get => this.undo;
      set => this.undo = value;
    }

    public WatchFolderCollection WatchFolders => this.watchFolders;

    private void watchFolders_Changed(object sender, SmartListChangedEventArgs<WatchFolder> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          e.Item.FileSystemWatcher.Renamed += new RenamedEventHandler(this.watcherRenamedNotification);
          break;
        case SmartListAction.Remove:
          e.Item.FileSystemWatcher.Renamed -= new RenamedEventHandler(this.watcherRenamedNotification);
          break;
      }
    }

    private void watcherRenamedNotification(object sender, RenamedEventArgs e)
    {
      try
      {
        if (Directory.Exists(e.FullPath))
        {
          foreach (ComicBook book in (SmartList<ComicBook>) this.Books)
          {
            if (book.FilePath.StartsWith(e.OldFullPath, StringComparison.OrdinalIgnoreCase))
              book.FilePath = e.FullPath + book.FilePath.Substring(e.OldFullPath.Length);
          }
        }
        else
        {
          if (!string.Equals(Path.GetExtension(e.OldFullPath), Path.GetExtension(e.FullPath), StringComparison.OrdinalIgnoreCase))
            return;
          ComicBook book = this.Books[e.OldFullPath];
          if (book == null)
            return;
          book.FilePath = e.FullPath;
        }
      }
      catch (PathTooLongException ex)
      {
      }
    }

    [XmlArrayItem("File")]
    public HashSet<string> BlackList => this.blackList;

    public void AddToBlackList(string path)
    {
      using (ItemMonitor.Lock((object) this.blackList))
      {
        if (this.blackList.Contains(path))
          return;
        this.blackList.Add(path);
        this.IsDirty = true;
      }
    }

    public bool IsBlacklisted(string path)
    {
      using (ItemMonitor.Lock((object) this.blackList))
        return this.blackList.Contains(path);
    }

    public void ClearBlackList()
    {
      using (ItemMonitor.Lock((object) this.blackList))
        this.blackList.Clear();
    }

    public IEnumerable<string> GetRecentFiles(int count)
    {
      return this.Books.Where<ComicBook>((Func<ComicBook, bool>) (x => x != null)).OrderBy<ComicBook, DateTime>((Func<ComicBook, DateTime>) (x => x.OpenedTime)).Reverse<ComicBook>().Take<ComicBook>(count).Select<ComicBook, string>((Func<ComicBook, string>) (x => x.FilePath));
    }

    public string GetLastFile() => this.GetRecentFiles(1).FirstOrDefault<string>();

    public void ConsolidateCustomThumbnails(string customThumbnailsPath)
    {
      try
      {
        HashSet<string> files = Directory.Exists(customThumbnailsPath) ? new HashSet<string>((IEnumerable<string>) Directory.GetFiles(customThumbnailsPath), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new HashSet<string>();
        Dictionary<string, IGrouping<string, ComicBook>> bookKeyGroups = this.Books.Where<ComicBook>((Func<ComicBook, bool>) (cb => !string.IsNullOrEmpty(cb.CustomThumbnailKey))).GroupBy<ComicBook, string>((Func<ComicBook, string>) (cb => cb.CustomThumbnailKey), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToDictionary<IGrouping<string, ComicBook>, string>((Func<IGrouping<string, ComicBook>, string>) (gr => gr.Key), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Dictionary<string, IGrouping<string, StacksConfig.StackConfigItem>> stackKeyGroups = this.ComicLists.GetItems<ComicListItem>().Select<ComicListItem, StacksConfig>((Func<ComicListItem, StacksConfig>) (cli => cli.Display.StackConfig)).Where<StacksConfig>((Func<StacksConfig, bool>) (sc => sc != null)).SelectMany<StacksConfig, StacksConfig.StackConfigItem>((Func<StacksConfig, IEnumerable<StacksConfig.StackConfigItem>>) (sc => (IEnumerable<StacksConfig.StackConfigItem>) sc.Configs)).Where<StacksConfig.StackConfigItem>((Func<StacksConfig.StackConfigItem, bool>) (sc => !string.IsNullOrEmpty(sc.ThumbnailKey))).GroupBy<StacksConfig.StackConfigItem, string>((Func<StacksConfig.StackConfigItem, string>) (sc => sc.ThumbnailKey), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToDictionary<IGrouping<string, StacksConfig.StackConfigItem>, string>((Func<IGrouping<string, StacksConfig.StackConfigItem>, string>) (gr => gr.Key), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (string path in files.Where<string>((Func<string, bool>) (f => !bookKeyGroups.ContainsKey(Path.GetFileName(f)) && !stackKeyGroups.ContainsKey(Path.GetFileName(f)))))
          FileUtility.SafeDelete(path);
        foreach (string key in bookKeyGroups.Keys.Where<string>((Func<string, bool>) (k => !files.Contains(Path.Combine(customThumbnailsPath, k)))))
          bookKeyGroups[key].ForEach<ComicBook>((Action<ComicBook>) (cb => cb.CustomThumbnailKey = (string) null));
        foreach (string key in stackKeyGroups.Keys.Where<string>((Func<string, bool>) (k => !files.Contains(Path.Combine(customThumbnailsPath, k)))))
          stackKeyGroups[key].ForEach<StacksConfig.StackConfigItem>((Action<StacksConfig.StackConfigItem>) (sc => sc.ThumbnailKey = (string) null));
      }
      catch (Exception ex)
      {
      }
    }

    public static ComicDatabase LoadXml(
      string file,
      Func<Stream, ComicDatabase> deserializer,
      Action<int> progress = null)
    {
      try
      {
        ComicDatabase comicDatabase;
        if (!File.Exists(file))
        {
          comicDatabase = ComicDatabase.CreateNew();
        }
        else
        {
          using (FileStream baseStream = File.OpenRead(file))
          {
            if (progress == null)
            {
              comicDatabase = deserializer((Stream) baseStream);
            }
            else
            {
              long len = baseStream.Length;
              long total = 0;
              int percent = 0;
              ProgressStream progressStream = new ProgressStream((Stream) baseStream, false);
              progressStream.DataRead += (EventHandler<ProgressStreamReadEventArgs>) ((sender, e) =>
              {
                total += (long) e.Count;
                int num = (int) (total * 100L / len);
                if (num == percent)
                  return;
                percent = num;
                progress(percent);
              });
              comicDatabase = deserializer((Stream) progressStream);
            }
          }
        }
        return comicDatabase;
      }
      catch (Exception ex)
      {
        throw new FileLoadException("Could not open the Database", ex);
      }
    }

    public static ComicDatabase LoadXml(string file, Action<int> progress = null)
    {
      return ComicDatabase.LoadXml(file, (Func<Stream, ComicDatabase>) (fs => XmlUtility.Load<ComicDatabase>(fs, false)), progress);
    }

    public static ComicDatabase CreateNew()
    {
      ComicDatabase comicDatabase = new ComicDatabase();
      comicDatabase.InitializeDefaultLists();
      return comicDatabase;
    }

    public void Save(string file, Action<Stream> serializer, bool commitCache = true)
    {
      string str = file + ".bak";
      try
      {
        if (commitCache)
          this.ComicLists.GetItems<ComicListItem>().ForEach<ComicListItem>((Action<ComicListItem>) (cli => cli.StoreCache()));
        using (ItemMonitor.Lock((object) this.BlackList))
        {
          using (FileStream fileStream = File.Create(str))
            serializer((Stream) fileStream);
        }
      }
      catch
      {
        FileUtility.SafeDelete(str);
        throw;
      }
      File.Copy(str, file, true);
      this.IsDirty = false;
    }

    public void SaveXml(string file, bool commitCache = true)
    {
      this.Save(file, (Action<Stream>) (fs => XmlUtility.Store(fs, (object) this, false)), commitCache);
    }

    public static ComicDatabase Attach(ComicDatabase copy, bool withBooks)
    {
      ComicDatabase comicDatabase1 = new ComicDatabase(false);
      comicDatabase1.Id = copy.Id;
      comicDatabase1.Name = copy.Name;
      ComicDatabase comicDatabase2 = comicDatabase1;
      if (withBooks)
        comicDatabase2.AttachBooks(new ComicBookCollection((IEnumerable<ComicBook>) copy.Books, false));
      comicDatabase2.AttachComicLists(copy.ComicLists);
      comicDatabase2.blackList = copy.BlackList;
      comicDatabase2.watchFolders = copy.WatchFolders;
      return comicDatabase2;
    }

    public static void Backup(
      string backupFile,
      string databaseFile,
      string customThumbnailsFolder)
    {
      using (ZipFile zipFile = ZipFile.Create(backupFile))
      {
        zipFile.BeginUpdate();
        zipFile.SetComment("ComicRack Backup");
        zipFile.Add(databaseFile, Path.GetFileName(databaseFile));
        foreach (string file in Directory.GetFiles(customThumbnailsFolder))
          zipFile.Add(file, Path.Combine("Thumbnails", Path.GetFileName(file)));
        zipFile.CommitUpdate();
      }
    }

    public static void RestoreBackup(
      string backupFile,
      string databaseFile,
      string customThumbnailsFolder)
    {
      using (ZipFile zipFile = new ZipFile(backupFile))
      {
        try
        {
          using (FileStream destination = File.Create(databaseFile))
            zipFile.GetInputStream(zipFile.GetEntry("ComicDb.xml")).CopyTo((Stream) destination);
        }
        catch (Exception ex)
        {
          FileUtility.SafeDelete(databaseFile);
          throw;
        }
        foreach (ZipEntry entry in zipFile)
        {
          if (entry.Name.StartsWith("Thumbnails"))
          {
            string path = Path.Combine(customThumbnailsFolder, Path.GetFileName(entry.Name));
            try
            {
              using (FileStream destination = File.Create(path))
                zipFile.GetInputStream(entry).CopyTo((Stream) destination);
            }
            catch (Exception ex)
            {
              FileUtility.SafeDelete(path);
              throw;
            }
          }
        }
      }
    }
  }
}

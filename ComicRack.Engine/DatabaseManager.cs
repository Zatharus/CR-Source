// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.DatabaseManager
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Database.Storage;
using System;
using System.IO;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class DatabaseManager : DisposableObject
  {
    private Timer timer;
    private ComicDatabase database;
    private ManualResetEvent databaseInitialized = new ManualResetEvent(false);
    private ComicBookFactory comicBookFactory;
    private int backgroundSaveInteral;
    private readonly ProcessingQueue<ComicDatabase> saveDatabaseQueue = new ProcessingQueue<ComicDatabase>("Save Database Queue", ThreadPriority.Lowest);

    public DatabaseManager()
    {
    }

    public DatabaseManager(string file, string connection = null)
    {
      this.Open(file, connection, true, (Action<int>) null);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.timer != null)
          this.timer.Dispose();
        this.Save();
        this.saveDatabaseQueue.Dispose();
        this.Database.Dispose();
      }
      base.Dispose(disposing);
    }

    public ComicDatabase Database
    {
      get
      {
        if (this.databaseInitialized != null)
        {
          if (this.FirstDatabaseAccess != null)
            this.FirstDatabaseAccess((object) this, EventArgs.Empty);
          this.databaseInitialized.WaitOne();
          this.databaseInitialized.Close();
          this.databaseInitialized = (ManualResetEvent) null;
        }
        return this.database;
      }
    }

    public ComicBookFactory BookFactory
    {
      get
      {
        return this.comicBookFactory ?? (this.comicBookFactory = new ComicBookFactory(this.Database.Books));
      }
    }

    public string OpenMessage { get; private set; }

    public string DatabaseFile { get; private set; }

    public int BackgroundSaveInterval
    {
      get => this.backgroundSaveInteral;
      set
      {
        if (value == this.backgroundSaveInteral)
          return;
        this.backgroundSaveInteral = value;
        if (this.timer != null)
          this.timer.Dispose();
        this.timer = (Timer) null;
        if (this.backgroundSaveInteral <= 0)
          return;
        this.timer = new Timer((TimerCallback) (o => this.SaveInBackground()), (object) null, 0, this.backgroundSaveInteral * 1000);
      }
    }

    public bool InitialConnectionError { get; set; }

    public event EventHandler FirstDatabaseAccess;

    public bool Open(
      string file,
      string connection,
      bool dontLoadQueryCaches,
      Action<int> progress)
    {
      this.DatabaseFile = file;
      try
      {
        this.OpenMessage = (string) null;
        ComicDatabase books = (ComicDatabase) null;
        try
        {
          string str1 = file + ".restore";
          if (File.Exists(str1))
          {
            try
            {
              books = ComicDatabase.LoadXml(str1, progress);
            }
            catch (Exception ex)
            {
            }
            FileUtility.SafeDelete(str1);
            if (books != null)
              this.OpenMessage = TR.Messages["DatabaseBackupRestored", "A previous database backup has been successfully restored!"];
          }
          if (books == null)
          {
            file += ".xml";
            try
            {
              books = ComicDatabase.LoadXml(file, progress);
            }
            catch (Exception ex)
            {
              try
              {
                string str2 = file + ".bak";
                books = File.Exists(str2) ? ComicDatabase.LoadXml(str2) : throw new FileNotFoundException();
                this.OpenMessage = TR.Messages["DatabaseRestored", "There was a problem with the Database. The last version to be known good has been restored. You may have lost some entires."];
              }
              catch
              {
                try
                {
                  File.Copy(file, Path.Combine(Path.GetDirectoryName(file), FileUtility.MakeValidFilename("Corrupt Database Backup [" + (object) DateTime.Now + "].xml")));
                }
                catch
                {
                }
                books = ComicDatabase.CreateNew();
                this.OpenMessage = TR.Messages["DatabaseNewEmpty", "There was a problem opening the Database. A new empty Database has been created."];
              }
            }
          }
          if (!string.IsNullOrEmpty(connection))
          {
            try
            {
              books.ComicStorage = ComicStorage.Create((ComicBookContainer) books, connection, (Action<int>) null);
            }
            catch (Exception ex)
            {
              this.InitialConnectionError = true;
              this.OpenMessage = TR.Messages["DataSourceProblem", "There was a problem ({0}) opening the data source. ComicRack is using a temporary database instead."].SafeFormat((object) ex.Message);
            }
          }
        }
        finally
        {
          books.FinalizeLoading();
        }
        if (dontLoadQueryCaches || books.ComicStorage != null)
          books.ComicLists.GetItems<ComicListItem>().ForEach<ComicListItem>((Action<ComicListItem>) (cli => cli.ResetCacheWithStatistics()));
        this.database = books;
        return true;
      }
      finally
      {
        this.databaseInitialized.Set();
      }
    }

    public void Save(string path = null)
    {
      if (this.database == null || this.InitialConnectionError)
        return;
      if (path == null)
        path = this.DatabaseFile;
      try
      {
        path += ".xml";
        if (this.Database.ComicStorage == null)
          this.Database.SaveXml(path);
        else
          ComicDatabase.Attach(this.Database, false).SaveXml(path, false);
      }
      catch (Exception ex)
      {
      }
    }

    public void BackupTo(string file, string customThumbnailPath)
    {
      if (this.database == null || this.InitialConnectionError)
        return;
      string str = this.DatabaseFile + ".xml";
      this.Save(str);
      ComicDatabase.Backup(file, str, customThumbnailPath);
    }

    public void RestoreFrom(string file, string customThumbnailPath)
    {
      if (this.database == null)
        return;
      ComicDatabase.RestoreBackup(file, this.DatabaseFile + ".restore", customThumbnailPath);
    }

    public void SaveInBackground()
    {
      if (this.database == null || this.saveDatabaseQueue.IsActive || !this.Database.IsDirty)
        return;
      this.Database.IsDirty = false;
      this.saveDatabaseQueue.AddItem(this.Database, (AsyncCallback) (result =>
      {
        try
        {
          ComicDatabase.Attach(this.Database, this.Database.ComicStorage == null).SaveXml(this.DatabaseFile + ".xml", false);
        }
        catch (Exception ex)
        {
        }
      }));
    }
  }
}

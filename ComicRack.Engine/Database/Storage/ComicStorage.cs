// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.Storage.ComicStorage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database.Storage
{
  public class ComicStorage : DisposableObject
  {
    private readonly IComicStorage storage;
    private readonly ComicBookContainer container;
    private readonly Thread updateThread;
    private readonly HashSet<ComicBook> deleteSet = new HashSet<ComicBook>();
    private readonly HashSet<ComicBook> writeSet = new HashSet<ComicBook>();
    private readonly AutoResetEvent stop = new AutoResetEvent(false);

    private ComicStorage(
      ComicBookContainer library,
      IComicStorage storage,
      bool copyLocal,
      Action<int> progress)
    {
      this.storage = storage;
      this.container = library;
      if (copyLocal && library.Books.Count > 0)
      {
        storage.BeginTransaction();
        try
        {
          int count = library.Books.Count;
          int num = 0;
          foreach (ComicBook book in (SmartList<ComicBook>) library.Books)
          {
            storage.Write(book);
            if (progress != null)
              progress(++num * 100 / count);
          }
          storage.CommitTransaction();
        }
        catch
        {
          storage.RollbackTransaction();
          throw;
        }
      }
      library.Books.Clear();
      ComicBook[] array = storage.Load().ToArray<ComicBook>();
      library.Books.AddRange((IEnumerable<ComicBook>) array);
      library.Books.Changed += new EventHandler<SmartListChangedEventArgs<ComicBook>>(this.Books_Changed);
      library.BookChanged += new EventHandler<ContainerBookChangedEventArgs>(this.library_BookChanged);
      this.updateThread = ThreadUtility.CreateWorkerThread("Database Update", new ThreadStart(this.UpdateDatabase), ThreadPriority.BelowNormal);
      this.updateThread.Start();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.container.Books.Changed -= new EventHandler<SmartListChangedEventArgs<ComicBook>>(this.Books_Changed);
        this.container.BookChanged -= new EventHandler<ContainerBookChangedEventArgs>(this.library_BookChanged);
        try
        {
          this.stop.Set();
          this.updateThread.Join();
          this.storage.Close();
        }
        catch (Exception ex)
        {
        }
      }
      base.Dispose(disposing);
    }

    public bool IsConnected => this.storage != null && this.storage.IsConnected;

    public string LastConnectionError { get; private set; }

    private void UpdateDatabase()
    {
      while (!this.stop.WaitOne(1000))
        this.WorkQueue();
      this.WorkQueue();
    }

    private void WorkQueue()
    {
      try
      {
        using (ItemMonitor.Lock((object) this.writeSet))
        {
          foreach (ComicBook book in this.writeSet.ToArray<ComicBook>())
          {
            this.storage.Refresh(this.container);
            this.storage.Write(book);
            this.writeSet.Remove(book);
          }
        }
        using (ItemMonitor.Lock((object) this.deleteSet))
        {
          foreach (ComicBook book in this.deleteSet.ToArray<ComicBook>())
          {
            this.storage.Delete(book);
            this.deleteSet.Remove(book);
          }
        }
        bool flag = true;
        if (this.OnShouldRefresh != null)
        {
          CancelEventArgs e = new CancelEventArgs();
          this.OnShouldRefresh((object) this, e);
          flag = !e.Cancel;
        }
        if (!flag)
          return;
        this.storage.Refresh(this.container);
      }
      catch (Exception ex)
      {
        this.LastConnectionError = ex.Message;
      }
    }

    public event CancelEventHandler OnShouldRefresh;

    private void library_BookChanged(object sender, ContainerBookChangedEventArgs e)
    {
      this.AddWriteToQueue(e.Book);
    }

    private void AddWriteToQueue(ComicBook book)
    {
      using (ItemMonitor.Lock((object) this.writeSet))
        this.writeSet.Add(book);
    }

    private void Books_Changed(object sender, SmartListChangedEventArgs<ComicBook> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          this.AddWriteToQueue(e.Item);
          break;
        case SmartListAction.Remove:
          using (ItemMonitor.Lock((object) this.deleteSet))
          {
            this.deleteSet.Add(e.Item);
            break;
          }
      }
    }

    public static ComicStorage Create(
      ComicBookContainer books,
      string connection,
      Action<int> progress)
    {
      IComicStorage storage = (IComicStorage) null;
      if (connection.StartsWith("mysql:", StringComparison.OrdinalIgnoreCase))
      {
        connection = connection.Substring("mysql:".Length);
        storage = (IComicStorage) new ComicStorageMySql();
      }
      else if (connection.StartsWith("mssql:", StringComparison.OrdinalIgnoreCase))
      {
        connection = connection.Substring("mssql:".Length);
        storage = (IComicStorage) new ComicStorageMsSql();
      }
      if (storage == null)
        return (ComicStorage) null;
      storage.Open(connection);
      return new ComicStorage(books, storage, true, progress);
    }
  }
}

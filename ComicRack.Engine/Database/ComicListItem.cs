// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicListItem
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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public abstract class ComicListItem : 
    NamedIdComponent,
    IComicBookListProvider,
    ILiteComponent,
    IDisposable,
    IIdentity,
    IComicBookList,
    IDisplayListConfig,
    IDeserializationCallback,
    IComicLibraryItem,
    IComicBookStatsProvider,
    ICachedComicBookList
  {
    [NonSerialized]
    private ComicLibrary library;
    [NonSerialized]
    private ComicListItemFolder parent;
    private bool favorite;
    private int bookCount;
    private int newBookCount;
    private int unreadBookCount;
    private string description = string.Empty;
    private DisplayListConfig displayListConfig = new DisplayListConfig();
    [NonSerialized]
    private ComicLibrary registeredLibrary;
    [NonSerialized]
    private volatile bool pendingCacheUpdate;
    [NonSerialized]
    private volatile HashSet<ComicBook> booksCache;
    [NonSerialized]
    private HashSet<ComicBook> unreadBooksCache;
    [NonSerialized]
    private HashSet<ComicBook> newBooksCache;
    [NonSerialized]
    private Dictionary<ComicBook, ComicListItem.PendingCacheAction> pendingCacheItems;
    [NonSerialized]
    private DateTime now;
    [NonSerialized]
    private bool pendingCacheRetrieval;
    private volatile bool notifyShield;
    private static readonly string[] defaultDependentProperties = new string[3]
    {
      "AddedTime",
      "LastPageRead",
      "PageCount"
    };
    [NonSerialized]
    private IAsyncResult updateResult;
    [NonSerialized]
    private readonly ThreadLocal<bool> recurseShield = new ThreadLocal<bool>();
    [NonSerialized]
    private Dictionary<ComicBookSeriesStatistics.Key, ComicBookSeriesStatistics> seriesStats;
    [NonSerialized]
    private object seriesStatsLock = new object();

    protected ComicListItem() => this.NewBookCountDate = DateTime.MinValue;

    [XmlIgnore]
    public virtual ComicLibrary Library
    {
      get => this.library;
      set
      {
        if (this.library == value)
          return;
        this.library = value;
        this.OnLibraryChanged();
      }
    }

    [XmlIgnore]
    public ComicListItemFolder Parent
    {
      get => this.parent;
      set
      {
        if (value == this.parent)
          return;
        this.parent = value;
      }
    }

    public virtual string ImageKey => "List";

    [XmlAttribute]
    [DefaultValue(false)]
    public virtual bool Favorite
    {
      get => this.favorite;
      set
      {
        if (value == this.favorite)
          return;
        this.favorite = value;
        this.OnChanged(ComicListItemChange.Edited);
      }
    }

    [DefaultValue(0)]
    public virtual int BookCount
    {
      get => this.bookCount;
      set
      {
        if (this.bookCount == value)
          return;
        this.bookCount = value;
        this.OnChanged(ComicListItemChange.Statistic);
      }
    }

    [DefaultValue(0)]
    public virtual int NewBookCount
    {
      get => this.newBookCount;
      set
      {
        if (this.newBookCount == value)
          return;
        this.newBookCount = value;
        this.OnChanged(ComicListItemChange.Statistic);
      }
    }

    [DefaultValue(typeof (DateTime), "01.01.0001")]
    public DateTime NewBookCountDate { get; set; }

    [DefaultValue(0)]
    public virtual int UnreadBookCount
    {
      get => this.unreadBookCount;
      set
      {
        if (this.unreadBookCount == value)
          return;
        this.unreadBookCount = value;
        this.OnChanged(ComicListItemChange.Statistic);
      }
    }

    [DefaultValue("")]
    public string Description
    {
      get => this.description;
      set
      {
        if (this.description == value)
          return;
        this.description = value;
        this.OnChanged(ComicListItemChange.Edited);
      }
    }

    [DefaultValue(null)]
    public string CacheStorage { get; set; }

    public DisplayListConfig Display
    {
      get => this.displayListConfig;
      set => this.displayListConfig = value;
    }

    protected virtual void OnLibraryChanged()
    {
      if (this.registeredLibrary != null)
        this.registeredLibrary.BookListChanged -= new EventHandler(this.Library_BookListRefreshed);
      if (this.Library != null)
        this.Library.BookListChanged += new EventHandler(this.Library_BookListRefreshed);
      this.ResetCache();
      this.registeredLibrary = this.Library;
    }

    protected virtual void OnBookListRefreshed() => this.OnBookListChanged();

    protected abstract IEnumerable<ComicBook> OnGetBooks();

    public virtual bool Filter(string filter)
    {
      return filter != null && (this.Name ?? string.Empty).IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) != -1;
    }

    [field: NonSerialized]
    public event ComicListChangedEventHandler Changed;

    protected virtual void OnChanged(ComicListItemChangedEventArgs e)
    {
      if (this.Library != null)
        this.Library.NotifyComicListChanged(e.Item, e.Change);
      if (this.Changed == null)
        return;
      this.Changed((object) this, e);
    }

    protected void OnChanged(ComicListItemChange changeType, ComicListItem item = null)
    {
      this.OnChanged(new ComicListItemChangedEventArgs(item ?? this, changeType));
    }

    private void Library_BookListRefreshed(object sender, EventArgs e)
    {
      this.OnBookListRefreshed();
    }

    public IEnumerable<ComicBook> GetBooks()
    {
      this.seriesStats = (Dictionary<ComicBookSeriesStatistics.Key, ComicBookSeriesStatistics>) null;
      try
      {
        if (!this.CacheEnabled)
          return this.InvokeGetBooks();
        this.CommitCache(true);
        return this.booksCache == null ? Enumerable.Empty<ComicBook>() : this.booksCache.Lock<ComicBook>();
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    [field: NonSerialized]
    public event EventHandler BookListChanged;

    public virtual void Refresh()
    {
      this.ResetCache();
      this.OnBookListChanged();
    }

    protected virtual void OnBookListChanged()
    {
      if (this.BookListChanged == null)
        return;
      this.BookListChanged((object) this, EventArgs.Empty);
    }

    public virtual bool CacheEnabled
    {
      get => this.Library != null && this.Library.IsLoaded && ComicLibrary.IsQueryCacheEnabled;
    }

    public virtual bool PendingCacheUpdate
    {
      get
      {
        if (!this.CacheEnabled)
          return false;
        return this.booksCache == null && this.CacheStorage == null || this.pendingCacheUpdate;
      }
    }

    public virtual bool PendingCacheRetrieval
    {
      get => this.CacheEnabled && this.CacheStorage != null && this.pendingCacheRetrieval;
    }

    public virtual bool OptimizedCacheUpdateDisabled => false;

    public virtual bool CustomCacheStorage => false;

    public void NotifyCacheRetrieval() => this.pendingCacheRetrieval = true;

    public void Notify(Action<ComicListItem> action)
    {
      if (this.notifyShield)
        return;
      try
      {
        this.notifyShield = true;
        action(this);
      }
      finally
      {
        this.notifyShield = false;
      }
    }

    public bool ResetCache(bool rebuildNow = false)
    {
      if (!this.CacheEnabled || this.booksCache == null && this.CacheStorage == null)
        return false;
      this.booksCache = (HashSet<ComicBook>) null;
      this.pendingCacheItems = (Dictionary<ComicBook, ComicListItem.PendingCacheAction>) null;
      this.CacheStorage = (string) null;
      this.pendingCacheUpdate = true;
      if (this.Parent != null)
        this.Parent.ResetCache();
      if (this.Library != null)
        this.Library.NotifyComicListCacheReset(this);
      if (rebuildNow)
        this.CommitCache(false);
      return true;
    }

    public void ResetCacheWithStatistics(bool rebuildNow = false)
    {
      this.BookCount = this.UnreadBookCount = this.NewBookCount = 0;
      this.NewBookCountDate = DateTime.MinValue;
      this.ResetCache(rebuildNow);
    }

    public void RetrieveCache()
    {
      try
      {
        this.pendingCacheRetrieval = false;
        if (this.CacheStorage == null || this.Library == null)
          return;
        HashSet<ComicBook> comicBookSet = new HashSet<ComicBook>();
        string cacheStorage = this.CacheStorage;
        this.CacheStorage = (string) null;
        using (ItemMonitor.Lock((object) comicBookSet))
        {
          if (cacheStorage == "Custom")
          {
            if (!this.OnRetrieveCustomCache(comicBookSet))
              throw new NotImplementedException();
          }
          else
          {
            foreach (string g in cacheStorage.Split(",".ToArray<char>(), StringSplitOptions.RemoveEmptyEntries))
            {
              ComicBook book = this.Library.Books[new Guid(g)];
              if (book != null)
                comicBookSet.Add(book);
            }
          }
        }
        this.InitializeBookCounters((ICollection<ComicBook>) comicBookSet);
        this.booksCache = comicBookSet;
      }
      catch (Exception ex)
      {
        this.booksCache = (HashSet<ComicBook>) null;
      }
    }

    public virtual IEnumerable<string> GetDependentProperties()
    {
      return (IEnumerable<string>) ComicListItem.defaultDependentProperties;
    }

    public virtual bool IsUpdateNeeded(string propertyHint) => true;

    public bool InvalidateCache(
      ComicBook cb,
      ComicListItem.PendingCacheAction action,
      string propertyHint = null)
    {
      if (!this.CacheEnabled || this.booksCache == null && this.CacheStorage == null || !string.IsNullOrEmpty(propertyHint) && action == ComicListItem.PendingCacheAction.Update && propertyHint != "AddedTime" && propertyHint != "LastPageRead" && propertyHint != "PageCount" && !this.IsUpdateNeeded(propertyHint))
        return false;
      if (this.OptimizedCacheUpdateDisabled)
      {
        this.ResetCache();
        return true;
      }
      this.pendingCacheUpdate = true;
      bool flag = false;
      using (ItemMonitor.Lock((object) this.pendingCacheItems))
      {
        if (this.pendingCacheItems == null)
          this.pendingCacheItems = new Dictionary<ComicBook, ComicListItem.PendingCacheAction>();
        ComicListItem.PendingCacheAction pendingCacheAction;
        if (this.pendingCacheItems.TryGetValue(cb, out pendingCacheAction))
        {
          if (pendingCacheAction == action)
            goto label_13;
        }
        this.pendingCacheItems[cb] = action;
        flag = true;
      }
label_13:
      if (flag)
      {
        if (this.Parent != null)
          this.Parent.InvalidateCache(cb, action);
        if (this.Library != null)
          this.Library.NotifyComicListCacheUpdate(this, cb, action);
      }
      return flag;
    }

    public void CommitCache(bool block)
    {
      if (!this.CacheEnabled)
        return;
      this.RetrieveCache();
      if (!this.PendingCacheUpdate)
        return;
      HashSet<ComicBook> booksCache1 = this.booksCache;
      Dictionary<ComicBook, ComicListItem.PendingCacheAction> pendingCacheItems = this.pendingCacheItems;
      this.pendingCacheUpdate = false;
      if (booksCache1 != null && pendingCacheItems != null)
      {
        if (!this.OptimizedCacheUpdateDisabled)
        {
          try
          {
            using (ItemMonitor.Lock((object) pendingCacheItems))
            {
              using (ItemMonitor.Lock((object) booksCache1))
              {
                ComicBook[] array1 = pendingCacheItems.Where<KeyValuePair<ComicBook, ComicListItem.PendingCacheAction>>((Func<KeyValuePair<ComicBook, ComicListItem.PendingCacheAction>, bool>) (kvp => kvp.Value == ComicListItem.PendingCacheAction.Remove)).Select<KeyValuePair<ComicBook, ComicListItem.PendingCacheAction>, ComicBook>((Func<KeyValuePair<ComicBook, ComicListItem.PendingCacheAction>, ComicBook>) (kvp => kvp.Key)).ToArray<ComicBook>();
                ComicBook[] array2 = pendingCacheItems.Where<KeyValuePair<ComicBook, ComicListItem.PendingCacheAction>>((Func<KeyValuePair<ComicBook, ComicListItem.PendingCacheAction>, bool>) (kvp => kvp.Value != ComicListItem.PendingCacheAction.Remove)).Select<KeyValuePair<ComicBook, ComicListItem.PendingCacheAction>, ComicBook>((Func<KeyValuePair<ComicBook, ComicListItem.PendingCacheAction>, ComicBook>) (kvp => kvp.Key)).ToArray<ComicBook>();
                HashSet<ComicBook> comicBookSet = (HashSet<ComicBook>) null;
                this.now = DateTime.Now;
                foreach (ComicBook comicBook in ((IEnumerable<ComicBook>) array2).Concat<ComicBook>((IEnumerable<ComicBook>) array1))
                {
                  if (booksCache1.Remove(comicBook))
                  {
                    if (comicBookSet == null)
                      comicBookSet = new HashSet<ComicBook>();
                    comicBookSet.Add(comicBook);
                    if (this.newBooksCache != null && this.newBooksCache.Contains(comicBook))
                      this.newBooksCache.Remove(comicBook);
                    if (this.unreadBooksCache != null && this.unreadBooksCache.Contains(comicBook))
                      this.unreadBooksCache.Remove(comicBook);
                  }
                }
                foreach (ComicBook cb in this.OnCacheMatch((IEnumerable<ComicBook>) array2))
                {
                  this.CreateBookCacheStatus(cb);
                  booksCache1.Add(cb);
                  comicBookSet?.Remove(cb);
                }
              }
            }
            this.BookCount = booksCache1.Count;
            this.NewBookCount = this.newBooksCache == null ? 0 : this.newBooksCache.Count;
            this.UnreadBookCount = this.unreadBooksCache == null ? 0 : this.unreadBooksCache.Count;
            pendingCacheItems.Clear();
            return;
          }
          catch (Exception ex)
          {
            this.ResetCache();
            return;
          }
        }
      }
      this.RunUpdate(block || !ComicLibrary.BackgroundQueryCacheUpdate, (Action) (() =>
      {
        HashSet<ComicBook> booksCache2 = new HashSet<ComicBook>(this.InvokeGetBooks());
        this.InitializeBookCounters((ICollection<ComicBook>) booksCache2);
        this.booksCache = booksCache2;
        this.pendingCacheItems = (Dictionary<ComicBook, ComicListItem.PendingCacheAction>) null;
      }));
    }

    private void RunUpdate(bool block, Action action)
    {
      IAsyncResult updateResult = this.updateResult;
      try
      {
        if (block)
        {
          if (updateResult != null && !updateResult.IsCompleted)
            updateResult.AsyncWaitHandle.WaitOne();
          action();
        }
        else
        {
          if (updateResult != null && !updateResult.IsCompleted)
            return;
          this.updateResult = ThreadUtility.RunInThreadQueue(action);
        }
      }
      catch (Exception ex)
      {
        action();
      }
    }

    public void StoreCache()
    {
      if (this.pendingCacheUpdate)
        this.CommitCache(true);
      if (this.CacheStorage != null)
        return;
      if (this.booksCache == null)
        this.CacheStorage = (string) null;
      else if (this.CustomCacheStorage)
      {
        this.CacheStorage = "Custom";
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (ComicBook comicBook in this.booksCache)
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append(",");
          stringBuilder.Append((object) comicBook.Id);
        }
        this.CacheStorage = stringBuilder.ToString();
      }
    }

    protected abstract IEnumerable<ComicBook> OnCacheMatch(IEnumerable<ComicBook> cbl);

    public virtual ISet<ComicBook> GetCache()
    {
      if (!this.CacheEnabled)
        throw new InvalidOperationException("Caching is not turned on");
      this.CommitCache(true);
      return (ISet<ComicBook>) this.booksCache;
    }

    private void InitializeBookCounters(ICollection<ComicBook> booksCache)
    {
      if (booksCache == null)
        return;
      this.now = DateTime.Now;
      using (ItemMonitor.Lock((object) this))
      {
        this.newBooksCache = this.unreadBooksCache = (HashSet<ComicBook>) null;
        booksCache.ForEach<ComicBook>(new Action<ComicBook>(this.CreateBookCacheStatus));
        this.BookCount = booksCache.Count;
      }
      this.NewBookCount = this.newBooksCache == null ? 0 : this.newBooksCache.Count;
      this.NewBookCountDate = DateTime.UtcNow;
      this.UnreadBookCount = this.unreadBooksCache == null ? 0 : this.unreadBooksCache.Count;
    }

    private void CreateBookCacheStatus(ComicBook cb)
    {
      if (cb.HasBeenRead)
        return;
      if ((this.now - cb.AddedTime).TotalDays < (double) EngineConfiguration.Default.IsRecentInDays)
      {
        if (this.newBooksCache == null)
          this.newBooksCache = new HashSet<ComicBook>();
        this.newBooksCache.Add(cb);
      }
      else
      {
        if (this.unreadBooksCache == null)
          this.unreadBooksCache = new HashSet<ComicBook>();
        this.unreadBooksCache.Add(cb);
      }
    }

    protected virtual bool OnRetrieveCustomCache(HashSet<ComicBook> books) => false;

    public override string ToString() => this.Description ?? string.Empty;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.registeredLibrary != null)
        this.registeredLibrary.BookListChanged -= new EventHandler(this.Library_BookListRefreshed);
      base.Dispose(disposing);
    }

    protected override void OnNameChanged()
    {
      base.OnNameChanged();
      this.OnChanged(ComicListItemChange.Edited);
    }

    protected IEnumerable<ComicBook> InvokeGetBooks() => this.OnGetBooks();

    public bool RecursionTest() => this.RecursionTest(this.Id, true);

    public bool RecursionTest(ComicListItem test) => this.RecursionTest(test.Id);

    public bool RecursionTest(Guid listId, bool ignoreSelfTest = false)
    {
      if (!ignoreSelfTest && this.Id == listId)
        return true;
      if (this.recurseShield.IsValueCreated && this.recurseShield.Value)
        return false;
      try
      {
        this.recurseShield.Value = true;
        switch (this)
        {
          case ComicListItemFolder comicListItemFolder when comicListItemFolder.Items.Any<ComicListItem>((Func<ComicListItem, bool>) (cli => cli.RecursionTest(listId))):
            return true;
          case ComicSmartListItem comicSmartListItem:
            ComicListItem baseList = comicSmartListItem.GetBaseList(false);
            if (baseList != null)
            {
              if (baseList.RecursionTest(listId))
                return true;
              break;
            }
            break;
        }
      }
      finally
      {
        this.recurseShield.Value = false;
      }
      return false;
    }

    void IDeserializationCallback.OnDeserialization(object sender)
    {
      if (this.registeredLibrary == null)
        return;
      this.registeredLibrary.BookListChanged += new EventHandler(this.Library_BookListRefreshed);
    }

    public ComicBookSeriesStatistics GetSeriesStats(ComicBook book)
    {
      if (this.CacheEnabled)
        return this.Library.GetSeriesStats(book);
      using (ItemMonitor.Lock(this.seriesStatsLock))
      {
        this.seriesStats = this.seriesStats ?? ComicBookSeriesStatistics.Create(this.Library.GetBooks());
        return this.seriesStats[new ComicBookSeriesStatistics.Key(book)];
      }
    }

    [SpecialName]
    void IComicBookListProvider.add_NameChanged(EventHandler value) => this.NameChanged += value;

    [SpecialName]
    void IComicBookListProvider.remove_NameChanged(EventHandler value) => this.NameChanged -= value;

    [SpecialName]
    Guid IIdentity.get_Id() => this.Id;

    [SpecialName]
    string IComicBookList.get_Name() => this.Name;

    public enum PendingCacheAction
    {
      Add,
      Remove,
      Update,
    }
  }
}

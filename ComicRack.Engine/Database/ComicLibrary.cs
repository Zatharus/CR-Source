// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicLibrary
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Threading;
using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class ComicLibrary : ComicBookContainer, IDeserializationCallback
  {
    [NonSerialized]
    private bool isDirty;
    private bool isLoaded;
    private HashSet<string> customValues = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private ComicListItemCollection comicLists = new ComicListItemCollection();
    private readonly ComicLibrary.ComicListItemLookup comicListItemLookup = new ComicLibrary.ComicListItemLookup();
    [NonSerialized]
    private Dictionary<ComicBookSeriesStatistics.Key, ComicBookSeriesStatistics> seriesStats;
    [NonSerialized]
    private object seriesStatsLock = new object();
    private static Type[] extraTypes;

    public ComicLibrary()
    {
      this.comicLists.Changed += new EventHandler<SmartListChangedEventArgs<ComicListItem>>(this.comicBookLists_Changed);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.comicLists.Changed -= new EventHandler<SmartListChangedEventArgs<ComicListItem>>(this.comicBookLists_Changed);
      base.Dispose(disposing);
    }

    [XmlIgnore]
    public bool IsDirty
    {
      get => this.isDirty;
      set => this.isDirty = value;
    }

    [XmlIgnore]
    public bool IsLoaded
    {
      get => this.isLoaded;
      set => this.isLoaded = value;
    }

    [XmlIgnore]
    public ComicListItemFolder TemporaryFolder
    {
      get
      {
        ComicListItemFolder temporaryFolder = this.ComicLists.GetItems<ComicListItemFolder>().FirstOrDefault<ComicListItemFolder>((Func<ComicListItemFolder, bool>) (c => c.Temporary));
        if (temporaryFolder == null)
        {
          temporaryFolder = new ComicListItemFolder(TR.Load("ComicBook")["TempLists", "Temporary Lists"])
          {
            Temporary = true,
            Collapsed = false
          };
          this.ComicLists.Add((ComicListItem) temporaryFolder);
        }
        return temporaryFolder;
      }
    }

    [XmlIgnore]
    public IEnumerable<string> CustomValues => (IEnumerable<string>) this.customValues;

    public override bool IsLibrary => true;

    protected override void OnBookChanged(ContainerBookChangedEventArgs e)
    {
      this.isDirty = true;
      if (e.PropertyName == "CustomValuesStore")
        this.AddCustomValues(e.Book);
      base.OnBookChanged(e);
      this.InvalidateComicListCaches(e.Book, ComicListItem.PendingCacheAction.Update, e.PropertyName);
    }

    protected override void OnBookAdded(ComicBook book)
    {
      this.isDirty = true;
      book.Container = (ComicBookContainer) this;
      this.AddCustomValues(book);
      base.OnBookAdded(book);
      this.InvalidateComicListCaches(book, ComicListItem.PendingCacheAction.Add);
    }

    protected override void OnBookRemoved(ComicBook book)
    {
      this.isDirty = true;
      book.Container = (ComicBookContainer) null;
      base.OnBookRemoved(book);
      this.InvalidateComicListCaches(book, ComicListItem.PendingCacheAction.Remove);
    }

    [XmlArrayItem("Item")]
    public ComicListItemCollection ComicLists => this.comicLists;

    [XmlIgnore]
    public bool ComicListsLocked { get; set; }

    private void comicBookLists_Changed(object sender, SmartListChangedEventArgs<ComicListItem> e)
    {
      this.isDirty = true;
      switch (e.Action)
      {
        case SmartListAction.Insert:
          e.Item.Library = this;
          this.OnComicListChanged(new ComicListItemChangedEventArgs(e.Item, ComicListItemChange.Added));
          break;
        case SmartListAction.Remove:
          e.Item.Library = (ComicLibrary) null;
          this.OnComicListChanged(new ComicListItemChangedEventArgs(e.Item, ComicListItemChange.Removed));
          break;
      }
    }

    private void Item_Changed(object sender, ComicListItemChangedEventArgs e)
    {
      this.OnComicListChanged(e);
    }

    public void InitializeDefaultLists()
    {
      this.comicLists.Add((ComicListItem) new ComicLibraryListItem(ComicBook.TR["Library", "Library"]));
      ComicListItemFolder comicListItemFolder = new ComicListItemFolder(ComicBook.TR["SmartLists", "Smart Lists"]);
      this.comicLists.Add((ComicListItem) comicListItemFolder);
      comicListItemFolder.Items.Add((ComicListItem) new ComicSmartListItem(ComicBook.TR["MyFavoritesList", "My Favorites"])
      {
        Matchers = {
          {
            typeof (ComicBookRatingMatcher),
            1,
            "3",
            ""
          }
        }
      });
      comicListItemFolder.Items.Add((ComicListItem) ComicLibrary.DefaultRecentlyAddedList());
      comicListItemFolder.Items.Add((ComicListItem) ComicLibrary.DefaultRecentlyReadList());
      comicListItemFolder.Items.Add((ComicListItem) new ComicSmartListItem(ComicBook.TR["NeverReadList", "Never Read"])
      {
        Matchers = {
          {
            typeof (ComicBookReadPercentageMatcher),
            2,
            EngineConfiguration.Default.IsNotReadCompletionPercentage.ToString(),
            ""
          }
        }
      });
      comicListItemFolder.Items.Add((ComicListItem) ComicLibrary.DefaultReadingList());
      comicListItemFolder.Items.Add((ComicListItem) new ComicSmartListItem(ComicBook.TR["ReadList", "Read"])
      {
        Matchers = {
          {
            typeof (ComicBookReadPercentageMatcher),
            1,
            EngineConfiguration.Default.IsReadCompletionPercentage.ToString(),
            ""
          }
        }
      });
      comicListItemFolder.Items.Add((ComicListItem) new ComicSmartListItem(ComicBook.TR["FilesUpdateList", "Files to update"])
      {
        Matchers = {
          {
            typeof (ComicBookModifiedInfoMatcher),
            0,
            "",
            ""
          }
        }
      });
    }

    public static ShareableComicListItem DefaultReadingList(ComicLibrary lib = null)
    {
      ComicSmartListItem comicSmartListItem = new ComicSmartListItem(ComicBook.TR["ReadingList", "Reading"]);
      ComicBookMatcherCollection matchers = comicSmartListItem.Matchers;
      Type matchType = typeof (ComicBookReadPercentageMatcher);
      int completionPercentage = EngineConfiguration.Default.IsNotReadCompletionPercentage;
      string matchValue1 = completionPercentage.ToString();
      completionPercentage = EngineConfiguration.Default.IsReadCompletionPercentage;
      string matchValue2 = completionPercentage.ToString();
      matchers.Add(matchType, 3, matchValue1, matchValue2);
      comicSmartListItem.Library = lib;
      return (ShareableComicListItem) comicSmartListItem;
    }

    public static ShareableComicListItem DefaultRecentlyReadList(ComicLibrary lib = null)
    {
      ComicSmartListItem comicSmartListItem = new ComicSmartListItem(ComicBook.TR["RecentlyReadList", "Recently Read"]);
      comicSmartListItem.Matchers.Add(typeof (ComicBookOpenedMatcher), 3, EngineConfiguration.Default.IsRecentInDays.ToString(), "");
      comicSmartListItem.Library = lib;
      return (ShareableComicListItem) comicSmartListItem;
    }

    public static ShareableComicListItem DefaultRecentlyAddedList(ComicLibrary lib = null)
    {
      ComicSmartListItem comicSmartListItem = new ComicSmartListItem(ComicBook.TR["RecentlyAddedList", "Recently Added"]);
      comicSmartListItem.Matchers.Add(typeof (ComicBookAddedMatcher), 3, EngineConfiguration.Default.IsRecentInDays.ToString(), "");
      comicSmartListItem.Library = lib;
      return (ShareableComicListItem) comicSmartListItem;
    }

    public void ResetDisplayConfigs(DisplayListConfig cfg)
    {
      ComicLibrary.SetDisplayListConfig((IEnumerable<ComicListItem>) this.ComicLists, cfg);
    }

    private static void SetDisplayListConfig(
      IEnumerable<ComicListItem> comicListItemCollection,
      DisplayListConfig cfg)
    {
      foreach (ComicListItem comicListItem in comicListItemCollection)
      {
        comicListItem.Display = CloneUtility.Clone<DisplayListConfig>(cfg);
        if (comicListItem is ComicListItemFolder comicListItemFolder)
          ComicLibrary.SetDisplayListConfig((IEnumerable<ComicListItem>) comicListItemFolder.Items, cfg);
      }
    }

    [field: NonSerialized]
    public event EventHandler CustomValuesChanged;

    [field: NonSerialized]
    public event ComicListChangedEventHandler ComicListsChanged;

    protected virtual void OnComicListChanged(ComicListItemChangedEventArgs e)
    {
      this.isDirty = true;
      this.UpdateQueryCacheListIndex(e);
      if (this.ComicListsChanged == null)
        return;
      this.ComicListsChanged((object) this, e);
    }

    public void NotifyComicListChanged(ComicListItem item, ComicListItemChange changeType)
    {
      this.OnComicListChanged(new ComicListItemChangedEventArgs(item, changeType));
    }

    protected void AttachComicLists(ComicListItemCollection comicLists)
    {
      this.comicLists = comicLists;
    }

    private void AddCustomValues(ComicBook book)
    {
      if (!ComicLibrary.AddCustomValues((ISet<string>) this.customValues, book) || this.CustomValuesChanged == null)
        return;
      this.CustomValuesChanged((object) this, EventArgs.Empty);
    }

    public static QueryCacheMode QueryCacheMode { get; set; }

    public static bool BackgroundQueryCacheUpdate { get; set; }

    public static bool IsQueryCacheEnabled => ComicLibrary.QueryCacheMode != 0;

    public static bool IsQueryCacheInstantUpdate
    {
      get => ComicLibrary.QueryCacheMode == QueryCacheMode.InstantUpdate;
    }

    [field: NonSerialized]
    public event EventHandler ComicListCachesUpdated;

    protected void UpdateQueryCacheListIndex(ComicListItemChangedEventArgs e)
    {
      if (!ComicLibrary.IsQueryCacheEnabled)
        return;
      switch (e.Change)
      {
        case ComicListItemChange.Added:
          this.comicListItemLookup.AddRange(e.Item);
          break;
        case ComicListItemChange.Removed:
          this.comicListItemLookup.RemoveRange(e.Item);
          break;
        case ComicListItemChange.Edited:
          this.comicListItemLookup.RemoveRange(e.Item);
          this.comicListItemLookup.AddRange(e.Item);
          break;
      }
    }

    protected void InvalidateComicListCaches(
      ComicBook cb,
      ComicListItem.PendingCacheAction pendingCacheAction,
      string propertyHint = null)
    {
      if (!ComicLibrary.IsQueryCacheEnabled || this.ComicLists.Count == 0)
        return;
      if (pendingCacheAction != ComicListItem.PendingCacheAction.Update || string.IsNullOrEmpty(propertyHint) || ComicBookSeriesStatistics.StatisticProperties.Contains(propertyHint))
        this.seriesStats = (Dictionary<ComicBookSeriesStatistics.Key, ComicBookSeriesStatistics>) null;
      IEnumerable<ComicListItem> comicListItems;
      if (pendingCacheAction == ComicListItem.PendingCacheAction.Update)
      {
        comicListItems = this.comicListItemLookup.Match(propertyHint);
        propertyHint = (string) null;
      }
      else
        comicListItems = this.ComicLists.GetItems<ComicListItem>(true);
      bool flag = false;
      foreach (ComicListItem comicListItem in comicListItems)
        flag |= comicListItem.InvalidateCache(cb, pendingCacheAction, propertyHint);
      if (!flag || this.ComicListCachesUpdated == null)
        return;
      this.ComicListCachesUpdated((object) this, EventArgs.Empty);
    }

    public void CommitComicListCacheChanges(Func<ComicListItem, bool> updatePredicate = null)
    {
      if (!ComicLibrary.IsQueryCacheEnabled || this.ComicLists.Count == 0)
        return;
      this.ComicLists.GetItems<ComicListItem>(true).Where<ComicListItem>((Func<ComicListItem, bool>) (cli =>
      {
        if (!cli.PendingCacheUpdate && !cli.PendingCacheRetrieval)
          return false;
        return updatePredicate == null || updatePredicate(cli);
      })).ForEach<ComicListItem>((Action<ComicListItem>) (cli => cli.CommitCache(false)));
    }

    public void NotifyDependingComicList(ComicListItem listItem, Action<ComicListItem> action)
    {
      this.comicListItemLookup.Match(listItem.Id).ForEach<ComicListItem>((Action<ComicListItem>) (item => item.Notify(action)));
    }

    public void NotifyComicListCacheReset(ComicListItem listItem)
    {
      this.NotifyDependingComicList(listItem, (Action<ComicListItem>) (cli => cli.ResetCache()));
    }

    public void NotifyComicListCacheUpdate(
      ComicListItem listItem,
      ComicBook cb,
      ComicListItem.PendingCacheAction pendingCacheAction)
    {
      this.NotifyDependingComicList(listItem, (Action<ComicListItem>) (cli => cli.InvalidateCache(cb, pendingCacheAction)));
    }

    public ComicBookSeriesStatistics GetSeriesStats(ComicBook book)
    {
      using (ItemMonitor.Lock(this.seriesStatsLock))
      {
        this.seriesStats = this.seriesStats ?? ComicBookSeriesStatistics.Create(this.GetBooks());
        return this.seriesStats[new ComicBookSeriesStatistics.Key(book)];
      }
    }

    public byte[] ToByteArray() => XmlUtility.Store((object) this, true);

    public static ComicLibrary FromByteArray(byte[] data) => XmlUtility.Load<ComicLibrary>(data);

    void IDeserializationCallback.OnDeserialization(object sender)
    {
      this.comicLists.Changed += new EventHandler<SmartListChangedEventArgs<ComicListItem>>(this.comicBookLists_Changed);
      this.comicLists.ForEach((Action<ComicListItem>) (cli => cli.Library = this));
    }

    public static ComicLibrary Attach(ComicLibrary library)
    {
      ComicLibrary comicLibrary1 = new ComicLibrary();
      comicLibrary1.Name = library.Name;
      comicLibrary1.Id = library.Id;
      ComicLibrary comicLibrary2 = comicLibrary1;
      comicLibrary2.AttachBooks(library.Books);
      comicLibrary2.AttachComicLists(library.ComicLists);
      return comicLibrary2;
    }

    public static Type[] GetExtraXmlSerializationTypes()
    {
      if (ComicLibrary.extraTypes == null)
      {
        List<Type> typeList = new List<Type>();
        typeList.AddRange(ComicBookValueMatcher.GetAvailableMatcherTypes());
        typeList.Add(typeof (ComicBookGroupMatcher));
        ComicLibrary.extraTypes = typeList.ToArray();
      }
      return ComicLibrary.extraTypes;
    }

    private static bool AddCustomValues(ISet<string> customValues, ComicBook book, bool showAll = true)
    {
      bool flag = false;
      foreach (string source in book.GetCustomValues().Select<StringPair, string>((Func<StringPair, string>) (p => p.Key)))
      {
        if ((showAll || !source.Contains<char>('.')) && !customValues.Contains(source))
        {
          flag = true;
          customValues.Add(source);
        }
      }
      return flag;
    }

    public static IEnumerable<string> GetCustomFields(
      IEnumerable<ComicBook> comicBooks,
      bool showAll)
    {
      HashSet<string> customValues = new HashSet<string>();
      foreach (ComicBook comicBook in comicBooks)
        ComicLibrary.AddCustomValues((ISet<string>) customValues, comicBook, showAll);
      return (IEnumerable<string>) customValues;
    }

    [Serializable]
    private class ComicListItemLookup
    {
      private ReverseIndex<ComicListItem, string> reversePropertyIndex = new ReverseIndex<ComicListItem, string>();
      private ReverseIndex<ComicListItem, Guid> reverseBaseListIndex = new ReverseIndex<ComicListItem, Guid>();

      public void Add(ComicListItem item)
      {
        this.reversePropertyIndex.Add(item, item.GetDependentProperties());
        if (!(item is ComicSmartListItem comicSmartListItem) || !(comicSmartListItem.BaseListId != Guid.Empty))
          return;
        this.reverseBaseListIndex.Add(item, comicSmartListItem.BaseListId);
      }

      public void AddRange(IEnumerable<ComicListItem> items)
      {
        items.ForEach<ComicListItem>(new Action<ComicListItem>(this.Add));
      }

      public void AddRange(ComicListItem item)
      {
        this.Add(item);
        if (!(item is ComicListItemFolder comicListItemFolder))
          return;
        this.AddRange(comicListItemFolder.Items.GetItems<ComicListItem>());
      }

      public void Remove(ComicListItem item)
      {
        this.reversePropertyIndex.Remove(item);
        this.reverseBaseListIndex.Remove(item);
      }

      public void RemoveRange(IEnumerable<ComicListItem> items)
      {
        items.ForEach<ComicListItem>(new Action<ComicListItem>(this.Remove));
      }

      public void RemoveRange(ComicListItem item)
      {
        this.Remove(item);
        if (!(item is ComicListItemFolder comicListItemFolder))
          return;
        this.RemoveRange(comicListItemFolder.Items.GetItems<ComicListItem>());
      }

      public IEnumerable<ComicListItem> Match(string property)
      {
        return this.reversePropertyIndex.Match(property).Concat<ComicListItem>(this.reversePropertyIndex.Match("*"));
      }

      public IEnumerable<ComicListItem> Match(Guid id) => this.reverseBaseListIndex.Match(id);
    }
  }
}

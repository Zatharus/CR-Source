// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.StorageSync
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Localize;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public class StorageSync
  {
    private readonly ISyncProvider provider;
    public EventHandler<StorageSync.SyncErrorEventArgs> Error;
    private long lastUpdateTime;
    private string oldMessage;

    public StorageSync(ISyncProvider provider) => this.provider = provider;

    protected virtual void OnError(StorageSync.SyncErrorEventArgs e)
    {
      if (this.Error == null)
        return;
      this.Error((object) this, e);
    }

    protected bool InvokeError(string message)
    {
      StorageSync.SyncErrorEventArgs e = new StorageSync.SyncErrorEventArgs(message);
      this.OnError(e);
      return !e.Cancel;
    }

    public void Synchronize(
      DeviceSyncSettings settings,
      ComicBookContainer library,
      ComicListItemCollection comicLists,
      IPagePool pagePool,
      IProgressState progress)
    {
      try
      {
        IEnumerable<ComicIdListItem> myBookLists = (IEnumerable<ComicIdListItem>) null;
        try
        {
          this.provider.ValidateDevice(this.provider.Device);
          this.provider.Start();
          this.UpdateProgress(progress, 0, TR.Messages["SyncGetStatus", "Retrieving current status from device"]);
          ComicBook[] array = this.provider.GetBooks().ToArray<ComicBook>();
          if (progress.Abort)
            return;
          int percentStart = 10;
          int k = 0;
          foreach (ComicBook ci in array)
          {
            string message = TR.Messages["SyncUpdateLibrary", "Updating library from '{0}'"].SafeFormat((object) ci.CaptionWithoutTitle);
            if (!this.UpdateProgress(progress, percentStart + ++k * 10 / array.Length, message))
              return;
            ComicBook itemById = library.Books.FindItemById(ci.Id);
            if (itemById != null && ci.ComicInfoIsDirty && ci.ExtraSyncInformation != null)
            {
              if (ci.ExtraSyncInformation.ReadingStateChanged && (ci.OpenedTime > itemById.OpenedTime || ci.OpenedCount == 0))
              {
                itemById.OpenedTime = ci.OpenedTime;
                itemById.OpenedCount = ci.OpenedCount;
                itemById.LastPageRead = ci.LastPageRead;
                itemById.CurrentPage = ci.CurrentPage;
              }
              if (ci.ExtraSyncInformation.InformationChanged)
              {
                itemById.Rating = ci.Rating;
                itemById.Manga = ci.Manga;
                if (!string.IsNullOrEmpty(ci.Review))
                  itemById.Review = ci.Review;
              }
              ComicPageInfo page1;
              if (ci.ExtraSyncInformation.BookmarksChanged)
              {
                for (int page2 = 0; page2 < ci.Pages.Count; ++page2)
                {
                  ComicBook comicBook = itemById;
                  int page3 = page2;
                  page1 = ci.GetPage(page2);
                  string bookmark = page1.Bookmark;
                  comicBook.UpdateBookmark(page3, bookmark);
                }
              }
              if (ci.ExtraSyncInformation.PageTypesChanged)
              {
                for (int page4 = 0; page4 < ci.Pages.Count; ++page4)
                {
                  ComicBook comicBook = itemById;
                  int page5 = page4;
                  page1 = ci.GetPage(page4);
                  int pageType = (int) page1.PageType;
                  comicBook.UpdatePageType(page5, (ComicPageType) pageType);
                }
              }
              if (ci.ExtraSyncInformation.CheckChanged)
                itemById.Checked = ci.Checked;
              if (ci.ExtraSyncInformation.DataChanged)
                itemById.SetInfo((ComicInfo) ci, false, false);
            }
          }
          ComicBook.ClearExtraSyncInformation();
          List<\u003C\u003Ef__AnonymousType2<ComicIdListItem, DeviceSyncSettings.SharedList>> list = settings.Lists.Select(sl => new
          {
            List = comicLists.FindItem(sl.ListId),
            Setting = sl
          }).Where(cli => cli.List != null).Select(cli =>
          {
            cli.List.ResetCache();
            return new
            {
              List = StorageSync.LimitList(new ComicIdListItem(cli.List, (Func<ComicBook, bool>) (cb =>
              {
                if (!cb.IsLinked)
                  return false;
                return cb.Checked || !cli.Setting.OnlyChecked;
              })), library, cli.Setting, cli.List is ComicIdListItem),
              Setting = cli.Setting
            };
          }).ToList();
          int count1 = this.provider.Device.BookSyncLimit > 0 ? this.provider.Device.BookSyncLimit : int.MaxValue;
          Dictionary<Guid, ComicBook> source = new Dictionary<Guid, ComicBook>((IDictionary<Guid, ComicBook>) list.SelectMany(bl => (IEnumerable<Guid>) bl.List.BookIds).Distinct<Guid>().Select<Guid, ComicBook>((Func<Guid, ComicBook>) (id => library.Books[id])).Take<ComicBook>(count1).ToDictionary<ComicBook, Guid>((Func<ComicBook, Guid>) (cb => cb.Id)));
          HashSet<Guid> guidSet = new HashSet<Guid>(list.Where(bl => bl.Setting.OptimizePortable).SelectMany(bl => (IEnumerable<Guid>) bl.List.BookIds).Distinct<Guid>());
          myBookLists = list.Select(bl => bl.List);
          k = 0;
          percentStart += 10;
          foreach (ComicBook book in array)
          {
            string message = TR.Messages["SyncRemoveBook", "Removing '{0}' from device"].SafeFormat((object) book.CaptionWithoutTitle);
            if (!this.UpdateProgress(progress, percentStart + ++k * 10 / array.Length, message))
              return;
            if (!source.ContainsKey(book.Id))
              this.provider.Remove(book);
          }
          int count = source.Count<KeyValuePair<Guid, ComicBook>>();
          k = 0;
          percentStart += 10;
          foreach (ComicBook book in source.Values)
          {
            if (progress.Abort)
              break;
            try
            {
              string msg = TR.Messages["SyncCopyBook", "Copying '{0}' to device"].SafeFormat((object) book.CaptionWithoutTitle);
              this.provider.Add(book, guidSet.Contains(book.Id), pagePool, (Action) (() => this.UpdateProgress(progress, percentStart + k * 65 / count)), (Action) (() => this.UpdateProgress(progress, percentStart + k * 65 / count, msg)), (Action) (() => this.UpdateProgress(progress, percentStart + ++k * 65 / count)));
            }
            catch (StorageSync.FatalSyncException ex)
            {
              throw;
            }
            catch (Exception ex)
            {
              this.InvokeError(ex.Message);
            }
          }
        }
        finally
        {
          try
          {
            this.provider.WaitForWritesCompleted();
            this.provider.SetLists(myBookLists);
          }
          catch (Exception ex)
          {
          }
          this.provider.Completed();
        }
      }
      catch (Exception ex)
      {
        this.InvokeError(ex.Message);
      }
    }

    private bool UpdateProgress(IProgressState progress, int percent, string message = null)
    {
      bool flag = true;
      long ticks = Machine.Ticks;
      if (ticks > this.lastUpdateTime + 5000L)
      {
        flag = this.provider.Progress(percent);
        this.lastUpdateTime = ticks;
      }
      if (progress != null)
      {
        if (!flag)
          progress.Abort = true;
        progress.ProgressPercentage = percent;
        this.oldMessage = progress.ProgressMessage = message ?? this.oldMessage;
        if (!progress.ProgressAvailable)
          progress.ProgressAvailable = true;
        if (progress.Abort)
          return false;
      }
      return true;
    }

    private static ComicIdListItem LimitList(
      ComicIdListItem list,
      ComicBookContainer library,
      DeviceSyncSettings.SharedList setting,
      bool isOrderedList)
    {
      List<ComicBook> list1 = list.BookIds.Select<Guid, ComicBook>((Func<Guid, ComicBook>) (id => library.Books[id])).Where<ComicBook>((Func<ComicBook, bool>) (cb => cb != null)).ToList<ComicBook>();
      IComparer<ComicBook> comparer = (IComparer<ComicBook>) null;
      IGrouper<ComicBook> grouper = (IGrouper<ComicBook>) null;
      if (setting.Sort)
      {
        switch (setting.ListSortType)
        {
          case DeviceSyncSettings.ListSort.Random:
            list1.Randomize<ComicBook>();
            break;
          case DeviceSyncSettings.ListSort.AlternateSeries:
            comparer = (IComparer<ComicBook>) new ChainedComparer<ComicBook>(new IComparer<ComicBook>[3]
            {
              (IComparer<ComicBook>) new ComicBookAlternateSeriesComparer(),
              (IComparer<ComicBook>) new ComicBookSeriesComparer(),
              (IComparer<ComicBook>) new ComicBookPublishedComparer()
            });
            grouper = (IGrouper<ComicBook>) new ComicBookGroupAlternateSeries();
            break;
          case DeviceSyncSettings.ListSort.Published:
            comparer = (IComparer<ComicBook>) new ChainedComparer<ComicBook>(new IComparer<ComicBook>[2]
            {
              (IComparer<ComicBook>) new ComicBookPublishedComparer(),
              (IComparer<ComicBook>) new ComicBookSeriesComparer()
            });
            break;
          case DeviceSyncSettings.ListSort.Added:
            comparer = (IComparer<ComicBook>) new ComicBookAddedComparer();
            break;
          case DeviceSyncSettings.ListSort.StoryArc:
            comparer = (IComparer<ComicBook>) new ChainedComparer<ComicBook>(new IComparer<ComicBook>[4]
            {
              (IComparer<ComicBook>) new ComicBookStoryArcComparer(),
              (IComparer<ComicBook>) new ComicBookAlternateNumberComparer(),
              (IComparer<ComicBook>) new ComicBookSeriesComparer(),
              (IComparer<ComicBook>) new ComicBookPublishedComparer()
            });
            grouper = (IGrouper<ComicBook>) new ComicBookGroupStoryArc();
            break;
          default:
            comparer = (IComparer<ComicBook>) new ChainedComparer<ComicBook>(new IComparer<ComicBook>[2]
            {
              (IComparer<ComicBook>) new ComicBookSeriesComparer(),
              (IComparer<ComicBook>) new ComicBookPublishedComparer()
            });
            grouper = (IGrouper<ComicBook>) new ComicBookGroupSeries();
            break;
        }
      }
      List<ComicBook>[] comicBookListArray;
      if (grouper != null)
        comicBookListArray = new GroupManager<ComicBook>(grouper, (IEnumerable<ComicBook>) list1).GetGroups().OrderBy<GroupContainer<ComicBook>, object>((Func<GroupContainer<ComicBook>, object>) (gc => gc.Key)).Select<GroupContainer<ComicBook>, List<ComicBook>>((Func<GroupContainer<ComicBook>, List<ComicBook>>) (gc => gc.Items)).ToArray<List<ComicBook>>();
      else
        comicBookListArray = new List<ComicBook>[1]
        {
          new List<ComicBook>((IEnumerable<ComicBook>) list1)
        };
      List<ComicBook>[] source = comicBookListArray;
      if (comparer != null)
      {
        foreach (List<ComicBook> comicBookList in source)
          comicBookList.Sort(comparer);
      }
      if (setting.OnlyUnread)
      {
        for (int index = 0; index < source.Length; ++index)
          source[index] = StorageSync.GetUnreadBooks((IList<ComicBook>) source[index], setting.KeepLastRead ? EngineConfiguration.Default.SyncKeepReadComics : 0);
      }
      list1.Clear();
      int num = source.Length != 0 ? ((IEnumerable<List<ComicBook>>) source).Max<List<ComicBook>>((Func<List<ComicBook>, int>) (gr => gr.Count)) : 0;
      for (int n = 0; n < num; n++)
        list1.AddRange(((IEnumerable<List<ComicBook>>) source).Where<List<ComicBook>>((Func<List<ComicBook>, bool>) (gr => n < gr.Count)).Select<List<ComicBook>, ComicBook>((Func<List<ComicBook>, ComicBook>) (gr => gr[n])));
      list.BookIds.Clear();
      if (!setting.Limit)
      {
        list.BookIds.AddRange(list1.Select<ComicBook, Guid>((Func<ComicBook, Guid>) (cb => cb.Id)));
      }
      else
      {
        switch (setting.LimitValueType)
        {
          case DeviceSyncSettings.LimitType.MB:
            list.BookIds.AddRange(StorageSync.CopyIdsWithSizeLimit((IEnumerable<ComicBook>) list1, (long) setting.LimitValue * 1024L * 1024L, setting.OptimizePortable));
            break;
          case DeviceSyncSettings.LimitType.GB:
            list.BookIds.AddRange(StorageSync.CopyIdsWithSizeLimit((IEnumerable<ComicBook>) list1, (long) setting.LimitValue * 1024L * 1024L * 1024L, setting.OptimizePortable));
            break;
          default:
            list.BookIds.AddRange(list1.Take<ComicBook>(setting.LimitValue).Select<ComicBook, Guid>((Func<ComicBook, Guid>) (cb => cb.Id)));
            break;
        }
      }
      return list;
    }

    private static IEnumerable<Guid> CopyIdsWithSizeLimit(
      IEnumerable<ComicBook> books,
      long size,
      bool optimized)
    {
      foreach (ComicBook book in books)
      {
        long num = book.IsDynamicSource ? (long) (book.PageCount * 250000) : book.FileSize;
        if (optimized)
          num /= 2L;
        size -= num;
        if (size >= 0L)
          yield return book.Id;
        else
          break;
      }
    }

    private static List<ComicBook> GetUnreadBooks(IList<ComicBook> books, int prologSize = 0)
    {
      List<ComicBook> unreadBooks = new List<ComicBook>();
      for (int index1 = 0; index1 < books.Count; ++index1)
      {
        if (!books[index1].HasBeenRead)
        {
          if (unreadBooks.Count == 0)
          {
            for (int index2 = Math.Max(0, index1 - prologSize); index2 < index1; ++index2)
              unreadBooks.Add(books[index2]);
          }
          unreadBooks.Add(books[index1]);
        }
      }
      return unreadBooks;
    }

    public class SyncErrorEventArgs : CancelEventArgs
    {
      public SyncErrorEventArgs(string message) => this.Message = message;

      public string Message { get; private set; }
    }

    public class FatalSyncException : Exception
    {
      public FatalSyncException()
      {
      }

      public FatalSyncException(string message, Exception inner = null)
        : base(message, inner)
      {
      }
    }

    public class DeviceOutOfSpaceException : StorageSync.FatalSyncException
    {
      public DeviceOutOfSpaceException(string message, Exception inner = null)
        : base(message, inner)
      {
      }
    }
  }
}

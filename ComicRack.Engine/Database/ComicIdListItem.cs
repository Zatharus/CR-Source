// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicIdListItem
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class ComicIdListItem : 
    ShareableComicListItem,
    IEditableComicBookListProvider,
    IComicBookListProvider,
    ILiteComponent,
    IDisposable,
    IIdentity,
    IComicBookList
  {
    private readonly SmartList<Guid> bookIds = new SmartList<Guid>();

    public ComicIdListItem(string name) => this.Name = name;

    public ComicIdListItem()
      : this(TR.Default["New", "New"])
    {
    }

    public ComicIdListItem(ComicListItem item, Func<ComicBook, bool> predicate = null)
    {
      this.Name = item.Name;
      this.Description = item.Description;
      this.Display = item.Display;
      if (item is ComicIdListItem && predicate == null)
        this.BookIds.AddRange((IEnumerable<Guid>) ((ComicIdListItem) item).BookIds);
      else
        this.BookIds.AddRange(item.GetBooks().Where<ComicBook>((Func<ComicBook, bool>) (cb => predicate == null || predicate(cb))).Select<ComicBook, Guid>((Func<ComicBook, Guid>) (cb => cb.Id)));
    }

    public SmartList<Guid> BookIds => this.bookIds;

    protected override IEnumerable<ComicBook> OnGetBooks()
    {
      HashSet<ComicBook> books = new HashSet<ComicBook>();
      if (this.Library != null && this.Library.Books != null)
      {
        List<Guid> list1 = (List<Guid>) null;
        List<Guid> list2 = (List<Guid>) null;
        foreach (Guid bookId in this.bookIds)
        {
          ComicBook book = this.Library.Books[bookId];
          if (book != null)
          {
            if (books.Contains(book))
            {
              if (list2 == null)
                list2 = new List<Guid>();
              list2.Add(bookId);
            }
            books.Add(book);
          }
          else
          {
            if (list1 == null)
              list1 = new List<Guid>();
            list1.Add(bookId);
          }
        }
        if (list1 != null)
          this.bookIds.RemoveRange((IEnumerable<Guid>) list1);
        if (list2 != null)
          this.bookIds.RemoveRange((IEnumerable<Guid>) list2);
      }
      return (IEnumerable<ComicBook>) books;
    }

    public bool IsLibrary => false;

    public int Add(ComicBook comicBook) => this.Insert(this.bookIds.Count, comicBook);

    public void AddRange(IEnumerable<ComicBook> books)
    {
      books.ForEach<ComicBook>((Action<ComicBook>) (b => this.Add(b)));
    }

    public int Insert(int index, ComicBook comicBook)
    {
      int index1 = this.bookIds.IndexOf(comicBook.Id);
      if (index1 != index)
      {
        if (index1 != -1)
        {
          this.bookIds.RemoveAt(index1);
          if (index > this.bookIds.Count)
            index = this.bookIds.Count;
          if (index1 < index)
            --index;
        }
        this.bookIds.Insert(index, comicBook.Id);
        this.Refresh();
      }
      return index;
    }

    public bool Remove(ComicBook comicBook)
    {
      bool flag = this.bookIds.Remove(comicBook.Id);
      if (flag)
        this.Refresh();
      return flag;
    }

    public override object Clone() => (object) new ComicIdListItem((ComicListItem) this);

    public override bool CustomCacheStorage => true;

    protected override IEnumerable<ComicBook> OnCacheMatch(IEnumerable<ComicBook> cbl)
    {
      return cbl.Where<ComicBook>((Func<ComicBook, bool>) (cb => this.BookIds.Contains(cb.Id)));
    }

    protected override bool OnRetrieveCustomCache(HashSet<ComicBook> books)
    {
      this.InvokeGetBooks().ForEach<ComicBook>((Action<ComicBook>) (b => books.Add(b)));
      return true;
    }

    public static ComicIdListItem CreateFromReadingList(
      ComicBookCollection library,
      IEnumerable<ComicReadingListItem> readingItems,
      IList<ComicBook> booksToAdd = null,
      Func<int, bool> progress = null)
    {
      ComicIdListItem fromReadingList = new ComicIdListItem();
      int num1 = readingItems.Count<ComicReadingListItem>();
      int num2 = 0;
      foreach (ComicReadingListItem readingItem in readingItems)
      {
        ComicReadingListItem crli = readingItem;
        if (progress != null && !progress(num2++ * 100 / num1))
          return (ComicIdListItem) null;
        ComicBook comicBook1 = library[crli.Id];
        if (comicBook1 != null)
        {
          fromReadingList.BookIds.Add(comicBook1.Id);
        }
        else
        {
          if (!string.IsNullOrEmpty(crli.FileName))
          {
            ComicBook itemByFileName = library.FindItemByFileName(crli.FileName);
            if (itemByFileName != null)
            {
              fromReadingList.BookIds.Add(itemByFileName.Id);
              continue;
            }
          }
          crli.SetFileNameInfo();
          IEnumerable<ComicBook> source1 = library.Where<ComicBook>((Func<ComicBook, bool>) (ci => ci.ShadowNumber == crli.Number && ComicInfo.SeriesEquals(ci.ShadowSeries, crli.Series, CompareSeriesOptions.None)));
          if (source1.Count<ComicBook>() == 0)
            source1 = library.Where<ComicBook>((Func<ComicBook, bool>) (ci => ci.ShadowNumber == crli.Number && ComicInfo.SeriesEquals(ci.ShadowSeries, crli.Series, CompareSeriesOptions.IgnoreVolumeInName)));
          if (source1.Count<ComicBook>() == 0)
            source1 = library.Where<ComicBook>((Func<ComicBook, bool>) (ci => ci.ShadowNumber == crli.Number && ComicInfo.SeriesEquals(ci.ShadowSeries, crli.Series, CompareSeriesOptions.IgnoreVolumeInName | CompareSeriesOptions.StripDown)));
          if (source1.Count<ComicBook>() > 1)
          {
            IEnumerable<ComicBook> source2 = source1;
            source1 = source2.Where<ComicBook>((Func<ComicBook, bool>) (ci => Math.Abs(ci.ShadowYear - crli.Year) <= 1));
            if (source1.Count<ComicBook>() == 0)
              source1 = source2;
          }
          if (source1.Count<ComicBook>() > 1)
          {
            IEnumerable<ComicBook> source3 = source1;
            source1 = source3.Where<ComicBook>((Func<ComicBook, bool>) (ci => ci.ShadowVolume == crli.Volume));
            if (source1.Count<ComicBook>() == 0)
              source1 = source3;
          }
          if (source1.Count<ComicBook>() > 1 && !string.IsNullOrEmpty(crli.Format))
          {
            IEnumerable<ComicBook> source4 = source1;
            source1 = source4.Where<ComicBook>((Func<ComicBook, bool>) (ci => string.Compare(ci.ShadowFormat, crli.Format, StringComparison.OrdinalIgnoreCase) == 0));
            if (source1.Count<ComicBook>() == 0)
              source1 = source4;
          }
          ComicBook comicBook2 = source1.FirstOrDefault<ComicBook>();
          if (comicBook2 == null)
          {
            ComicBook comicBook3 = new ComicBook();
            comicBook3.AddedTime = DateTime.Now;
            comicBook3.Series = crli.Series;
            comicBook3.Number = crli.Number;
            comicBook3.Volume = crli.Volume;
            comicBook3.Year = crli.Year;
            comicBook3.Format = crli.Format;
            comicBook2 = comicBook3;
            booksToAdd?.Add(comicBook2);
          }
          fromReadingList.BookIds.Add(comicBook2.Id);
        }
      }
      return fromReadingList;
    }

    [SpecialName]
    void IComicBookListProvider.add_NameChanged(EventHandler value) => this.NameChanged += value;

    [SpecialName]
    void IComicBookListProvider.remove_NameChanged(EventHandler value) => this.NameChanged -= value;

    [SpecialName]
    Guid IIdentity.get_Id() => this.Id;

    [SpecialName]
    string IComicBookList.get_Name() => this.Name;
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookContainer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public class ComicBookContainer : 
    NamedIdComponent,
    IEditableComicBookListProvider,
    IComicBookListProvider,
    ILiteComponent,
    IDisposable,
    IIdentity,
    IComicBookList
  {
    private ComicBookCollection books = new ComicBookCollection();
    [NonSerialized]
    private volatile ComicsEditModes editMode = ComicsEditModes.Default;

    public ComicBookContainer()
    {
      this.books.Changed += new EventHandler<SmartListChangedEventArgs<ComicBook>>(this.books_Changed);
    }

    public ComicBookContainer(string name)
      : this()
    {
      this.Name = name;
    }

    [XmlArrayItem("Book")]
    public ComicBookCollection Books => this.books;

    [XmlIgnore]
    public ComicsEditModes EditMode
    {
      get => this.editMode;
      set => this.editMode = value;
    }

    private void books_Changed(object sender, SmartListChangedEventArgs<ComicBook> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          this.OnBookAdded(e.Item);
          e.Item.BookChanged += new EventHandler<BookChangedEventArgs>(this.bookChanged);
          break;
        case SmartListAction.Remove:
          this.OnBookRemoved(e.Item);
          e.Item.BookChanged -= new EventHandler<BookChangedEventArgs>(this.bookChanged);
          break;
      }
      this.Refresh();
    }

    private void bookChanged(object sender, BookChangedEventArgs e)
    {
      this.OnBookChanged(new ContainerBookChangedEventArgs((ComicBook) sender, e));
    }

    public void Refresh() => this.OnBookListChanged();

    public void Consolidate()
    {
      foreach (ComicBook book in (SmartList<ComicBook>) this.Books)
      {
        book.Pages.Consolidate();
        book.TrimExcessPageInfo();
      }
    }

    protected void AttachBooks(ComicBookCollection list) => this.books = list;

    protected virtual void OnBookChanged(ContainerBookChangedEventArgs e)
    {
      if (this.BookChanged == null)
        return;
      this.BookChanged((object) this, e);
    }

    protected virtual void OnBookListChanged()
    {
      if (this.BookListChanged == null)
        return;
      this.BookListChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnBookAdded(ComicBook book)
    {
    }

    protected virtual void OnBookRemoved(ComicBook book)
    {
    }

    [field: NonSerialized]
    public event EventHandler<ContainerBookChangedEventArgs> BookChanged;

    public IEnumerable<ComicBook> GetBooks() => (IEnumerable<ComicBook>) this.Books;

    public IEnumerable<string> GetBookFiles()
    {
      return this.Books.Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsLinked)).Select<ComicBook, string>((Func<ComicBook, string>) (cb => cb.FilePath));
    }

    [field: NonSerialized]
    public event EventHandler BookListChanged;

    public virtual bool IsLibrary => false;

    public int Add(ComicBook comicBook) => this.Insert(this.Books.Count, comicBook);

    public int Insert(int index, ComicBook comicBook)
    {
      int num1 = this.Books.IndexOf(this.Books[comicBook.FilePath]);
      if (num1 != -1)
        return num1;
      int num2 = this.Books.IndexOf(this.Books[comicBook.Id]);
      if (num2 != -1)
        return num2;
      this.Books.Insert(index, comicBook);
      return index;
    }

    public bool Remove(ComicBook comicBook) => this.Books.Remove(comicBook);

    [XmlIgnore]
    public int BookCount
    {
      get => this.books.Count;
      set
      {
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
  }
}

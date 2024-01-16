// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicLibraryListItem
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class ComicLibraryListItem : 
    ComicListItem,
    IEditableComicBookListProvider,
    IComicBookListProvider,
    ILiteComponent,
    IDisposable,
    IIdentity,
    IComicBookList
  {
    public ComicLibraryListItem(string name) => this.Name = name;

    public ComicLibraryListItem()
      : this("Library")
    {
    }

    public override string ImageKey => "Library";

    public override bool CustomCacheStorage => true;

    protected override IEnumerable<ComicBook> OnCacheMatch(IEnumerable<ComicBook> cbl) => cbl;

    protected override bool OnRetrieveCustomCache(HashSet<ComicBook> books)
    {
      this.Library.Books.ForEach((Action<ComicBook>) (b => books.Add(b)));
      return true;
    }

    protected override IEnumerable<ComicBook> OnGetBooks() => this.Library.GetBooks();

    public bool IsLibrary => true;

    public int Add(ComicBook comicBook) => this.Library.Add(comicBook);

    public int Insert(int index, ComicBook comicBook) => this.Library.Insert(index, comicBook);

    public bool Remove(ComicBook comicBook) => this.Library.Remove(comicBook);

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

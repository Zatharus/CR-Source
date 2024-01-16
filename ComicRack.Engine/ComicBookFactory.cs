// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookFactory
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.IO;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookFactory
  {
    [NonSerialized]
    private ComicBookCollection temporaryBooks;

    public ComicBookFactory(ComicBookCollection storage) => this.Storage = storage;

    public ComicBookCollection Storage { get; private set; }

    [XmlIgnore]
    public ComicBookCollection TemporaryBooks
    {
      get
      {
        if (this.temporaryBooks == null)
        {
          this.temporaryBooks = new ComicBookCollection();
          this.temporaryBooks.Changed += new EventHandler<SmartListChangedEventArgs<ComicBook>>(this.OnTemporaryBooksChanged);
        }
        return this.temporaryBooks;
      }
    }

    public bool TemporaryBookListDirty { get; set; }

    public ComicBook Create(string file, CreateBookOption addOptions, RefreshInfoOptions options)
    {
      if (string.IsNullOrEmpty(file))
        return (ComicBook) null;
      ComicBook comicBook1;
      if (file.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
      {
        comicBook1 = this.Storage.FindItemById(new Guid(file.Substring(3)));
        if (comicBook1 == null)
          return (ComicBook) null;
      }
      else
        comicBook1 = this.Storage.FindItemByFile(file);
      if (comicBook1 != null)
        return comicBook1;
      if (!File.Exists(file) || Providers.Readers.GetSourceProviderType(file) == (Type) null)
      {
        this.TemporaryBooks.Remove(file);
        return (ComicBook) null;
      }
      ComicBook temporaryBook = this.TemporaryBooks[file];
      bool flag = temporaryBook != null;
      ComicBook comicBook2 = temporaryBook ?? ComicBook.Create(file, options);
      switch (addOptions)
      {
        case CreateBookOption.DoNotAdd:
          return comicBook2;
        case CreateBookOption.AddToStorage:
          comicBook2.AddedTime = DateTime.Now;
          if ((options & RefreshInfoOptions.GetFastPageCount) != RefreshInfoOptions.None)
            comicBook2.RefreshInfoFromFile(RefreshInfoOptions.GetFastPageCount);
          this.TemporaryBooks.Remove(comicBook2);
          this.Storage.Add(comicBook2);
          goto case CreateBookOption.DoNotAdd;
        case CreateBookOption.AddToTemporary:
          if (!flag)
          {
            this.TemporaryBooks.Add(comicBook2);
            comicBook2.RefreshInfoFromFile(options);
            comicBook2.BookChanged += new EventHandler<BookChangedEventArgs>(this.OnTemporaryBookChanged);
            goto case CreateBookOption.DoNotAdd;
          }
          else
            goto case CreateBookOption.DoNotAdd;
        default:
          throw new ArgumentOutOfRangeException(nameof (addOptions));
      }
    }

    public ComicBook Create(string file, CreateBookOption addOptions)
    {
      return this.Create(file, addOptions, RefreshInfoOptions.None);
    }

    [field: NonSerialized]
    public event EventHandler<ContainerBookChangedEventArgs> TemporaryBookChanged;

    private void OnTemporaryBookChanged(object sender, BookChangedEventArgs e)
    {
      if (this.TemporaryBookChanged == null)
        return;
      this.TemporaryBookChanged((object) this, new ContainerBookChangedEventArgs((ComicBook) sender, e));
    }

    private void OnTemporaryBooksChanged(object sender, SmartListChangedEventArgs<ComicBook> e)
    {
      if (e.Action != SmartListAction.Insert)
        return;
      this.TemporaryBookListDirty = true;
    }
  }
}

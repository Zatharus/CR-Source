// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookCollection
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public class ComicBookCollection : SmartList<ComicBook>, IDeserializationCallback
  {
    private readonly bool updateDictionaries;
    [NonSerialized]
    private readonly Dictionary<string, ComicBook> fileDictionary = new Dictionary<string, ComicBook>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    [NonSerialized]
    private readonly Dictionary<Guid, ComicBook> guidDictionary = new Dictionary<Guid, ComicBook>();

    public ComicBookCollection(IEnumerable<ComicBook> items = null, bool updateDictionaries = true)
    {
      this.updateDictionaries = updateDictionaries;
      if (items == null)
        return;
      this.AddRange(items);
    }

    protected override void OnInsertCompleted(int index, ComicBook item)
    {
      this.OnBookAdded(item);
      base.OnInsertCompleted(index, item);
    }

    protected override void OnRemoveCompleted(int index, ComicBook item)
    {
      base.OnRemoveCompleted(index, item);
      this.OnBookRemoved(item);
    }

    private void ComicBookFile_Renamed(object sender, ComicBookFileRenameEventArgs e)
    {
      using (ItemMonitor.Lock((object) this.fileDictionary))
      {
        this.fileDictionary.Remove(e.OldFile);
        if (string.IsNullOrEmpty(e.NewFile))
          return;
        this.fileDictionary[e.NewFile] = sender as ComicBook;
      }
    }

    public void Remove(string file) => this.Remove(this[file]);

    public ComicBook this[string file] => this.FindItemByFile(file);

    public ComicBook this[Guid id] => this.FindItemById(id);

    public ComicBook FindItemByFile(string file)
    {
      using (this.GetLock(false))
      {
        using (ItemMonitor.Lock((object) this.fileDictionary))
        {
          ComicBook comicBook;
          return this.fileDictionary.TryGetValue(file, out comicBook) ? comicBook : (ComicBook) null;
        }
      }
    }

    public ComicBook FindItemById(Guid id)
    {
      using (this.GetLock(false))
      {
        using (ItemMonitor.Lock((object) this.guidDictionary))
        {
          ComicBook comicBook;
          return this.guidDictionary.TryGetValue(id, out comicBook) ? comicBook : (ComicBook) null;
        }
      }
    }

    public ComicBook FindItemByFileName(string fileName)
    {
      return this.Find((Predicate<ComicBook>) (cb => string.Equals(cb.FileName, fileName, StringComparison.OrdinalIgnoreCase)));
    }

    public ComicBook FindItemByFileNameSize(string file)
    {
      FileInfo fi = new FileInfo(file);
      string name = Path.GetFileNameWithoutExtension(file);
      return this.Find((Predicate<ComicBook>) (cb => string.Equals(cb.FileName, name, StringComparison.OrdinalIgnoreCase) && cb.FileSize == fi.Length));
    }

    private void OnBookAdded(ComicBook item)
    {
      using (ItemMonitor.Lock((object) this.fileDictionary))
      {
        if (item.IsLinked)
          this.fileDictionary[item.FilePath] = item;
        this.guidDictionary[item.Id] = item;
      }
      if (!this.updateDictionaries)
        return;
      item.FileRenamed += new EventHandler<ComicBookFileRenameEventArgs>(this.ComicBookFile_Renamed);
    }

    private void OnBookRemoved(ComicBook item)
    {
      using (ItemMonitor.Lock((object) this.fileDictionary))
      {
        if (item.IsLinked)
          this.fileDictionary.Remove(item.FilePath);
        this.guidDictionary.Remove(item.Id);
      }
      if (!this.updateDictionaries)
        return;
      item.FileRenamed -= new EventHandler<ComicBookFileRenameEventArgs>(this.ComicBookFile_Renamed);
    }

    void IDeserializationCallback.OnDeserialization(object sender)
    {
      this.ForEach(new Action<ComicBook>(this.OnBookAdded));
    }

    public static IEnumerable<ComicBook> Filter(
      ComicBookFilterType filter,
      IEnumerable<ComicBook> books)
    {
      if (filter.HasFlag((Enum) ComicBookFilterType.Library))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsInContainer));
      if (filter.HasFlag((Enum) ComicBookFilterType.NotInLibrary))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => !cb.IsInContainer));
      if (filter.HasFlag((Enum) ComicBookFilterType.IsLocal))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.EditMode.IsLocalComic()));
      if (filter.HasFlag((Enum) ComicBookFilterType.IsNotLocal))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => !cb.EditMode.IsLocalComic()));
      if (filter.HasFlag((Enum) ComicBookFilterType.IsFileless))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => !cb.IsLinked));
      if (filter.HasFlag((Enum) ComicBookFilterType.IsNotFileless))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsLinked));
      if (filter.HasFlag((Enum) ComicBookFilterType.IsEditable))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.EditMode.CanEditProperties()));
      if (filter.HasFlag((Enum) ComicBookFilterType.IsNotEditable))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => !cb.EditMode.CanEditProperties()));
      if (filter.HasFlag((Enum) ComicBookFilterType.CanExport))
        books = books.Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.EditMode.CanExport()));
      if (filter.HasFlag((Enum) ComicBookFilterType.AsArray))
        books = (IEnumerable<ComicBook>) books.Lock<ComicBook>().ToArray<ComicBook>();
      return books;
    }
  }
}

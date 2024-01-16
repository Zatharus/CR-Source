// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicListItemFolder
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [Serializable]
  public class ComicListItemFolder : ComicListItem, IDeserializationCallback, ICloneable
  {
    private readonly ComicListItemCollection items = new ComicListItemCollection();

    public ComicListItemFolder()
    {
      this.items.Changed += new EventHandler<SmartListChangedEventArgs<ComicListItem>>(this.items_Changed);
    }

    public ComicListItemFolder(string name)
      : this()
    {
      this.Name = name;
    }

    public ComicListItemFolder(ComicListItemFolder item)
      : this(item.Name)
    {
      this.Description = item.Description;
      this.Display = item.Display;
      this.CombineMode = item.CombineMode;
      foreach (ComicListItem data in (SmartList<ComicListItem>) item.Items)
      {
        if (data is ICloneable)
          this.Items.Add(((ICloneable) data).Clone<ComicListItem>());
      }
    }

    [XmlArrayItem("Item")]
    public ComicListItemCollection Items => this.items;

    [XmlAttribute]
    [DefaultValue(false)]
    public bool Collapsed { get; set; }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool Temporary { get; set; }

    [XmlAttribute]
    [DefaultValue(ComicFolderCombineMode.Or)]
    public ComicFolderCombineMode CombineMode { get; set; }

    [XmlIgnore]
    public override ComicLibrary Library
    {
      get => base.Library;
      set
      {
        base.Library = value;
        this.Items.ForEach((Action<ComicListItem>) (cli => cli.Library = value));
      }
    }

    public override string ImageKey => !this.Temporary ? "Folder" : "TempFolder";

    public override bool IsUpdateNeeded(string propertyHint) => false;

    protected override IEnumerable<ComicBook> OnCacheMatch(IEnumerable<ComicBook> cbl)
    {
      switch (this.CombineMode)
      {
        case ComicFolderCombineMode.And:
          foreach (ComicBook comicBook in cbl.Where<ComicBook>((Func<ComicBook, bool>) (cb => this.Items.All<ComicListItem>((Func<ComicListItem, bool>) (list => list.GetCache().Contains(cb))))))
            yield return comicBook;
          break;
        case ComicFolderCombineMode.Empty:
          break;
        default:
          foreach (ComicBook comicBook in cbl.Where<ComicBook>((Func<ComicBook, bool>) (cb => this.Items.Any<ComicListItem>((Func<ComicListItem, bool>) (list => list.GetCache().Contains(cb))))))
            yield return comicBook;
          break;
      }
    }

    public override bool Filter(string filter)
    {
      return this.Items.Any<ComicListItem>((Func<ComicListItem, bool>) (cli => cli.Filter(filter)));
    }

    private void items_Changed(object sender, SmartListChangedEventArgs<ComicListItem> e)
    {
      this.ResetCache();
      switch (e.Action)
      {
        case SmartListAction.Insert:
          e.Item.Changed += new ComicListChangedEventHandler(this.Item_Changed);
          e.Item.Parent = this;
          e.Item.Library = this.Library;
          this.OnChanged(ComicListItemChange.Added, e.Item);
          break;
        case SmartListAction.Remove:
          e.Item.Changed -= new ComicListChangedEventHandler(this.Item_Changed);
          e.Item.Parent = (ComicListItemFolder) null;
          e.Item.Library = (ComicLibrary) null;
          this.OnChanged(ComicListItemChange.Removed, e.Item);
          break;
      }
    }

    private void Item_Changed(object sender, ComicListItemChangedEventArgs e)
    {
      this.ResetCache();
      this.OnChanged(e);
    }

    protected override IEnumerable<ComicBook> OnGetBooks()
    {
      IEnumerable<ComicBook> comicBooks = (IEnumerable<ComicBook>) null;
      switch (this.CombineMode)
      {
        case ComicFolderCombineMode.And:
          using (IEnumerator<ComicListItem> enumerator = this.Items.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ComicListItem current = enumerator.Current;
              comicBooks = comicBooks == null ? current.GetBooks() : comicBooks.Intersect<ComicBook>(current.GetBooks(), (IEqualityComparer<ComicBook>) ComicBook.GuidEquality);
              if (comicBooks.IsEmpty<ComicBook>())
                break;
            }
            goto case ComicFolderCombineMode.Empty;
          }
        case ComicFolderCombineMode.Empty:
          return comicBooks ?? Enumerable.Empty<ComicBook>();
        default:
          using (IEnumerator<ComicListItem> enumerator = this.Items.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ComicListItem current = enumerator.Current;
              comicBooks = comicBooks == null ? current.GetBooks() : comicBooks.Union<ComicBook>(current.GetBooks(), (IEqualityComparer<ComicBook>) ComicBook.GuidEquality);
              if (this.Library != null)
              {
                if (comicBooks.Count<ComicBook>() == this.Library.BookCount)
                  break;
              }
            }
            goto case ComicFolderCombineMode.Empty;
          }
      }
    }

    void IDeserializationCallback.OnDeserialization(object sender)
    {
      this.items.Changed += new EventHandler<SmartListChangedEventArgs<ComicListItem>>(this.items_Changed);
    }

    public object Clone() => (object) new ComicListItemFolder(this);
  }
}

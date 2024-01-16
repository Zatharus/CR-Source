// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookContainerUndo
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookContainerUndo
  {
    private const int UndoSize = 10;
    private readonly LinkedList<ComicBookContainerUndo.UndoItem> items = new LinkedList<ComicBookContainerUndo.UndoItem>();
    private LinkedListNode<ComicBookContainerUndo.UndoItem> currentItem;
    private bool inUpdate;
    private string pendingMarker;
    private ComicBookContainer container;

    public ComicBookContainer Container
    {
      get => this.container;
      set
      {
        if (value == this.container)
          return;
        if (this.container != null)
        {
          this.container.BookChanged -= new EventHandler<ContainerBookChangedEventArgs>(this.BookChanged);
          this.container.Books.Changed -= new EventHandler<SmartListChangedEventArgs<ComicBook>>(this.BookListChanged);
        }
        this.container = value;
        this.Clear();
        if (this.container == null)
          return;
        this.container.BookChanged += new EventHandler<ContainerBookChangedEventArgs>(this.BookChanged);
      }
    }

    public bool CanUndo => this.currentItem != null;

    public bool CanRedo
    {
      get
      {
        using (ItemMonitor.Lock((object) this.items))
          return this.currentItem == null ? this.items.First != null : this.currentItem.Next != null;
      }
    }

    public IEnumerable<string> UndoEntries
    {
      get
      {
        using (ItemMonitor.Lock((object) this.items))
        {
          LinkedListNode<ComicBookContainerUndo.UndoItem> node;
          for (node = this.currentItem; node != null; node = node.Previous)
          {
            if (node.Value.IsMarker)
              yield return node.Value.Marker;
          }
          node = (LinkedListNode<ComicBookContainerUndo.UndoItem>) null;
        }
      }
    }

    public IEnumerable<string> RedoEntries
    {
      get
      {
        using (ItemMonitor.Lock((object) this.items))
        {
          if (this.items.First == null)
            ;
          else
          {
            LinkedListNode<ComicBookContainerUndo.UndoItem> node;
            for (node = this.currentItem ?? this.items.First; node != null; node = node.Next)
            {
              if (node.Value.IsMarker)
                yield return node.Value.Marker;
            }
            node = (LinkedListNode<ComicBookContainerUndo.UndoItem>) null;
          }
        }
      }
    }

    public string UndoLabel => this.UndoEntries.FirstOrDefault<string>();

    public string RedoLabel => this.RedoEntries.FirstOrDefault<string>();

    public void Clear()
    {
      using (ItemMonitor.Lock((object) this.items))
      {
        this.items.Clear();
        this.currentItem = (LinkedListNode<ComicBookContainerUndo.UndoItem>) null;
        this.pendingMarker = (string) null;
      }
    }

    public void SetMarker(string name) => this.pendingMarker = name;

    public void Undo()
    {
      using (ItemMonitor.Lock((object) this.items))
      {
        if (!this.CanUndo)
          return;
        this.inUpdate = true;
        try
        {
          while (this.currentItem != null)
          {
            ComicBookContainerUndo.UndoItem undoItem = this.currentItem.Value;
            bool isMarker = undoItem.IsMarker;
            switch (undoItem.Type)
            {
              case ComicBookContainerUndo.UndoItemType.ComicProperty:
                ComicBook book = this.container.Books[undoItem.BookId];
                if (book != null)
                {
                  book.SetValue(undoItem.Property, undoItem.OldValue);
                  break;
                }
                break;
              case ComicBookContainerUndo.UndoItemType.ComicDeleted:
                this.container.Add(undoItem.Book);
                break;
              case ComicBookContainerUndo.UndoItemType.ComicInserted:
                this.container.Remove(undoItem.Book);
                break;
            }
            this.currentItem = this.currentItem.Previous;
            if (isMarker)
              break;
          }
        }
        finally
        {
          this.inUpdate = false;
        }
      }
    }

    public void Redo()
    {
      using (ItemMonitor.Lock((object) this.items))
      {
        if (!this.CanRedo)
          return;
        this.inUpdate = true;
        try
        {
          this.currentItem = this.currentItem == null ? this.items.First : this.currentItem.Next;
          do
          {
            ComicBookContainerUndo.UndoItem undoItem = this.currentItem.Value;
            switch (undoItem.Type)
            {
              case ComicBookContainerUndo.UndoItemType.ComicProperty:
                ComicBook book = this.container.Books[undoItem.BookId];
                if (book != null)
                {
                  book.SetValue(undoItem.Property, undoItem.NewValue);
                  break;
                }
                break;
              case ComicBookContainerUndo.UndoItemType.ComicDeleted:
                this.container.Remove(undoItem.Book);
                break;
              case ComicBookContainerUndo.UndoItemType.ComicInserted:
                this.container.Add(undoItem.Book);
                break;
            }
            this.currentItem = this.currentItem.Next;
          }
          while (this.currentItem != null && !this.currentItem.Value.IsMarker);
          this.currentItem = this.currentItem == null ? this.items.Last : this.currentItem.Previous;
        }
        finally
        {
          this.inUpdate = false;
        }
      }
    }

    private void BookChanged(object sender, ContainerBookChangedEventArgs e)
    {
      if (this.inUpdate || e.OldValue == null || e.NewValue == null)
        return;
      this.AddItem(new ComicBookContainerUndo.UndoItem()
      {
        Type = ComicBookContainerUndo.UndoItemType.ComicProperty,
        BookId = e.Book.Id,
        Property = e.PropertyName,
        OldValue = e.OldValue,
        NewValue = e.NewValue
      });
    }

    private void BookListChanged(object sender, SmartListChangedEventArgs<ComicBook> e)
    {
      if (this.inUpdate)
        return;
      switch (e.Action)
      {
        case SmartListAction.Insert:
          this.AddItem(new ComicBookContainerUndo.UndoItem()
          {
            Type = ComicBookContainerUndo.UndoItemType.ComicInserted,
            BookId = e.Item.Id,
            Book = e.Item
          });
          break;
        case SmartListAction.Remove:
          this.AddItem(new ComicBookContainerUndo.UndoItem()
          {
            Type = ComicBookContainerUndo.UndoItemType.ComicDeleted,
            BookId = e.Item.Id,
            Book = e.Item
          });
          break;
      }
    }

    private void AddItem(ComicBookContainerUndo.UndoItem undoItem)
    {
      using (ItemMonitor.Lock((object) this.items))
      {
        this.Trim(this.currentItem);
        if (!string.IsNullOrEmpty(this.pendingMarker))
        {
          this.Trim(10);
          undoItem.Marker = this.pendingMarker;
          this.pendingMarker = (string) null;
        }
        this.currentItem = this.items.AddLast(undoItem);
      }
    }

    private void Trim(
      LinkedListNode<ComicBookContainerUndo.UndoItem> trimNode)
    {
      if (trimNode == null)
        return;
      LinkedListNode<ComicBookContainerUndo.UndoItem> linkedListNode = this.items.Last;
      while (linkedListNode != null && trimNode != linkedListNode)
      {
        linkedListNode = linkedListNode.Previous;
        this.items.RemoveLast();
      }
    }

    private void Trim(int size)
    {
      int num = this.UndoEntries.Count<string>();
label_4:
      while (num > size)
      {
        --num;
        this.items.RemoveFirst();
        while (true)
        {
          if (this.items.First != null && !this.items.First.Value.IsMarker)
            this.items.RemoveFirst();
          else
            goto label_4;
        }
      }
    }

    private enum UndoItemType
    {
      ComicProperty,
      ComicDeleted,
      ComicInserted,
    }

    private class UndoItem
    {
      public string Marker { get; set; }

      public ComicBookContainerUndo.UndoItemType Type { get; set; }

      public Guid BookId { get; set; }

      public string Property { get; set; }

      public object OldValue { get; set; }

      public object NewValue { get; set; }

      public ComicBook Book { get; set; }

      public bool IsMarker => !string.IsNullOrEmpty(this.Marker);
    }
  }
}

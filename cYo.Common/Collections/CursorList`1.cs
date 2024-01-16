// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.CursorList`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Collections.Generic;

#nullable disable
namespace cYo.Common.Collections
{
  public class CursorList<T> : LinkedList<T>
  {
    public const int DefaultMaxSize = 50;
    private LinkedListNode<T> cursorNode;

    public CursorList(int maxSize) => this.MaxSize = maxSize;

    public CursorList()
      : this(50)
    {
    }

    public int MaxSize { get; set; }

    public LinkedListNode<T> CursorNode => this.cursorNode;

    public T CursorValue => this.cursorNode != null ? this.cursorNode.Value : default (T);

    public bool CanMoveCursorPrevious
    {
      get => this.cursorNode != null && this.cursorNode.Previous != null;
    }

    public bool CanMoveCursorNext => this.cursorNode != null && this.cursorNode.Next != null;

    public LinkedListNode<T> MoveCursorStart() => this.cursorNode = this.First;

    public LinkedListNode<T> MoveCursorEnd() => this.cursorNode = this.Last;

    public LinkedListNode<T> MoveCursorPrevious()
    {
      if (this.CanMoveCursorPrevious)
        this.cursorNode = this.cursorNode.Previous;
      return this.cursorNode;
    }

    public LinkedListNode<T> MoveCursorNext()
    {
      if (this.CanMoveCursorNext)
        this.cursorNode = this.cursorNode.Next;
      return this.cursorNode;
    }

    public void AddAtCursor(LinkedListNode<T> node)
    {
      if (this.cursorNode != null)
      {
        if (object.Equals((object) this.cursorNode.Value, (object) node.Value))
          return;
        LinkedListNode<T> linkedListNode = this.Last;
        while (linkedListNode != this.cursorNode)
        {
          linkedListNode = linkedListNode.Previous;
          this.RemoveLast();
        }
      }
      this.AddLast(node);
      while (this.Count > this.MaxSize)
        this.RemoveFirst();
      this.cursorNode = this.Last;
    }

    public void AddAtCursor(T value) => this.AddAtCursor(new LinkedListNode<T>(value));
  }
}

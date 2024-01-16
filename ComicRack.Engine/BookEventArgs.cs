// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.BookEventArgs
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class BookEventArgs : EventArgs
  {
    private readonly ComicBook book;

    public BookEventArgs(ComicBook book) => this.book = book;

    public ComicBook Book => this.book;
  }
}

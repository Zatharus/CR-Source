// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.BookPageRetrievalCompletedEventArgs
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class BookPageRetrievalCompletedEventArgs : EventArgs
  {
    private readonly int page;
    private readonly bool twoPage;

    public BookPageRetrievalCompletedEventArgs(Bitmap bitmap, int page, bool twoPage)
    {
      this.Bitmap = bitmap;
      this.page = page;
      this.twoPage = twoPage;
    }

    public Bitmap Bitmap { get; set; }

    public int Page => this.page;

    public bool TwoPage => this.twoPage;
  }
}

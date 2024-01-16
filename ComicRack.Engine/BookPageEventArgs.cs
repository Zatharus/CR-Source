// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.BookPageEventArgs
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class BookPageEventArgs : BookEventArgs
  {
    private readonly int oldPage;
    private readonly int page;
    private readonly ComicPageInfo pageInfo;
    private readonly string pageKey = string.Empty;

    public BookPageEventArgs(
      ComicBook book,
      int oldPage,
      int page,
      ComicPageInfo pageInfo,
      string pageKey)
      : base(book)
    {
      this.oldPage = oldPage;
      this.page = page;
      this.pageInfo = pageInfo;
      this.pageKey = pageKey;
    }

    public int OldPage => this.oldPage;

    public int Page => this.page;

    public ComicPageInfo PageInfo => this.pageInfo;

    public string PageKey => this.pageKey;
  }
}

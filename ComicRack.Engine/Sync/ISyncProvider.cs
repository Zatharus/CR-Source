// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.ISyncProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public interface ISyncProvider
  {
    DeviceInfo Device { get; }

    void ValidateDevice(DeviceInfo device);

    void Start();

    IEnumerable<ComicBook> GetBooks();

    void Add(
      ComicBook book,
      bool optimize,
      IPagePool pagePool,
      Action working,
      Action start,
      Action completed);

    void Remove(ComicBook book);

    void SetLists(IEnumerable<ComicIdListItem> myBookLists);

    void WaitForWritesCompleted();

    bool Progress(int percent);

    void Completed();
  }
}

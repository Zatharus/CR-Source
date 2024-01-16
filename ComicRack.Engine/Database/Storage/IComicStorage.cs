// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.Storage.IComicStorage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database.Storage
{
  public interface IComicStorage
  {
    bool Open(string connection);

    void Close();

    void Delete(ComicBook book);

    bool Write(ComicBook book);

    IEnumerable<ComicBook> Load();

    bool Refresh(ComicBookContainer books);

    void BeginTransaction();

    void CommitTransaction();

    void RollbackTransaction();

    bool IsConnected { get; }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.Automation.IBrowser
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Projects.ComicRack.Engine;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins.Automation
{
  public interface IBrowser
  {
    bool OpenNextComic();

    bool OpenPrevComic();

    bool OpenRandomComic();

    void SelectComics(IEnumerable<ComicBook> books);
  }
}

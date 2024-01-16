// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.IMain
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.Sync;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public interface IMain : IContainerControl
  {
    Control Control { get; }

    NavigatorManager OpenBooks { get; }

    void ConvertComic(IEnumerable<ComicBook> books, ExportSetting setting);

    ComicDisplay ComicDisplay { get; }

    bool BrowserVisible { get; set; }

    bool ReaderUndocked { get; set; }

    bool MinimalGui { get; set; }

    DockStyle BrowserDock { get; set; }

    void ShowInfo();

    bool ShowBookInList(ComicLibrary library, ComicListItem list, ComicBook cb, bool switchToList);

    IEditRating GetRatingEditor();

    IEditPage GetPageEditor();

    void EditListLayout();

    void SaveListLayout();

    void EditListLayouts();

    void UpdateListConfigMenus(ToolStripItemCollection items);

    void ShowComic();

    void UpdateWebComic(ComicBook comic, bool fullRefresh);

    void StoreWorkspace();

    void ToggleBrowser();

    void ShowPortableDevices(DeviceSyncSettings dss, Guid? guid);
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ISmartListDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Projects.ComicRack.Engine.Database;
using System;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public interface ISmartListDialog
  {
    ComicLibrary Library { get; set; }

    Guid EditId { get; set; }

    ComicSmartListItem SmartComicList { get; set; }

    bool EnableNavigation { get; set; }

    bool PreviousEnabled { get; set; }

    bool NextEnabled { get; set; }

    event EventHandler Apply;

    event EventHandler Next;

    event EventHandler Previous;
  }
}

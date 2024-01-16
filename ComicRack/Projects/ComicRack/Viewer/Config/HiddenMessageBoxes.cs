// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.HiddenMessageBoxes
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  [Flags]
  public enum HiddenMessageBoxes
  {
    None = 0,
    RemoveFromList = 1,
    RemoveList = 2,
    RemoveFavorite = 4,
    ConvertComics = 8,
    SetAllListLayouts = 16, // 0x00000010
    CloseExternalReader = 32, // 0x00000020
    ComicRackMinimized = 64, // 0x00000040
    AskDirtyItems = 128, // 0x00000080
    AskClearData = 256, // 0x00000100
  }
}

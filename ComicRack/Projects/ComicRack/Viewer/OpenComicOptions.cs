// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.OpenComicOptions
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  [Flags]
  public enum OpenComicOptions
  {
    None = 0,
    NoRefreshInfo = 1,
    NoIncreaseOpenedCount = 2,
    NoMoveToLastPage = 4,
    NoGlobalColorAdjustment = 8,
    NoUpdateCurrentPage = 16, // 0x00000010
    OpenInNewSlot = 32, // 0x00000020
    AppendNewSlots = 64, // 0x00000040
    NoFileUpdate = 128, // 0x00000080
    DisableAll = NoUpdateCurrentPage | NoGlobalColorAdjustment | NoMoveToLastPage | NoIncreaseOpenedCount | NoRefreshInfo, // 0x0000001F
    Default = 0,
  }
}

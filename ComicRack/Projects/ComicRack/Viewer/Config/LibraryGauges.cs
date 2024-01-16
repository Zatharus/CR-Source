// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.LibraryGauges
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  [Flags]
  public enum LibraryGauges
  {
    None = 0,
    New = 1,
    Unread = 2,
    Total = 4,
    Numeric = 4096, // 0x00001000
    Default = Numeric | Total | Unread | New, // 0x00001007
  }
}

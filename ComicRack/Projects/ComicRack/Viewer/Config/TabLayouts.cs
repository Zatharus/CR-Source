// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.TabLayouts
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  [Flags]
  public enum TabLayouts
  {
    None = 0,
    Paste = 1,
    Export = 2,
    Multiple = 4,
    ReaderSettings = 8,
    BehaviorSettings = 16, // 0x00000010
    LibrarySettings = 32, // 0x00000020
    ScriptSettings = 64, // 0x00000040
    AdvancedSettings = 128, // 0x00000080
  }
}

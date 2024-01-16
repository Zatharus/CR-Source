// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookFilterType
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Flags]
  public enum ComicBookFilterType
  {
    All = 0,
    Library = 1,
    NotInLibrary = 2,
    IsLocal = 4,
    IsNotLocal = 8,
    IsFileless = 16, // 0x00000010
    IsNotFileless = 32, // 0x00000020
    IsEditable = 256, // 0x00000100
    IsNotEditable = 512, // 0x00000200
    CanExport = 1024, // 0x00000400
    Selected = 4096, // 0x00001000
    Sorted = 8192, // 0x00002000
    AsArray = 16384, // 0x00004000
  }
}

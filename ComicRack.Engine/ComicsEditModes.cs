// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicsEditModes
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Flags]
  public enum ComicsEditModes
  {
    None = 0,
    Local = 1,
    EditProperties = 2,
    EditPages = 4,
    DeleteComics = 8,
    ExportComic = 16, // 0x00000010
    Rescan = 32, // 0x00000020
    EditComicList = 64, // 0x00000040
    Default = EditComicList | Rescan | ExportComic | DeleteComics | EditPages | EditProperties | Local, // 0x0000007F
    Remote = 0,
    RemoteEditable = EditProperties, // 0x00000002
  }
}

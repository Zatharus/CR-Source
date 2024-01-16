// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicPageType
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Flags]
  [Serializable]
  public enum ComicPageType : short
  {
    FrontCover = 1,
    InnerCover = 2,
    Roundup = 4,
    Story = 8,
    Advertisement = 16, // 0x0010
    Editorial = 32, // 0x0020
    Letters = 64, // 0x0040
    Preview = 128, // 0x0080
    BackCover = 256, // 0x0100
    Other = 512, // 0x0200
    Deleted = 1024, // 0x0400
    [Browsable(false)] All = Other | BackCover | Preview | Letters | Editorial | Advertisement | Story | Roundup | InnerCover | FrontCover, // 0x03FF
    [Browsable(false)] AllWithDeleted = All | Deleted, // 0x07FF
  }
}

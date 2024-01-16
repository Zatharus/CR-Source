// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ThumbnailDrawingOptions
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  [Flags]
  public enum ThumbnailDrawingOptions
  {
    None = 0,
    EnableShadow = 1,
    EnableBorder = 2,
    EnableRating = 4,
    EnableVerticalBookmarks = 8,
    EnableHorizontalBookmarks = 16, // 0x00000010
    EnablePageNumber = 32, // 0x00000020
    EnableBackImage = 64, // 0x00000040
    EnableBackground = 128, // 0x00000080
    EnableStates = 256, // 0x00000100
    KeepAspect = 512, // 0x00000200
    EnableBowShadow = 1024, // 0x00000400
    DisableMissingThumbnail = 4096, // 0x00001000
    FastMode = 8192, // 0x00002000
    NoOpaqueCover = 16384, // 0x00004000
    Selected = 65536, // 0x00010000
    Hot = 131072, // 0x00020000
    Focused = 262144, // 0x00040000
    Stacked = 524288, // 0x00080000
    Bookmarked = 1048576, // 0x00100000
    AspectFill = 2097152, // 0x00200000
    DefaultWithoutBackground = EnableBowShadow | EnableStates | EnableBackImage | EnableVerticalBookmarks | EnableRating | EnableBorder | EnableShadow, // 0x0000054F
    Default = DefaultWithoutBackground | EnableBackground, // 0x000005CF
  }
}

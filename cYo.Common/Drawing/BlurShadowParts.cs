// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.BlurShadowParts
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Drawing
{
  [Flags]
  public enum BlurShadowParts
  {
    TopLeft = 1,
    TopCenter = 2,
    TopRight = 4,
    CenterRight = 8,
    BottomRight = 16, // 0x00000010
    BottomCenter = 32, // 0x00000020
    BottomLeft = 64, // 0x00000040
    CenterLeft = 128, // 0x00000080
    Center = 256, // 0x00000100
    Top = TopRight | TopCenter | TopLeft, // 0x00000007
    Right = BottomRight | CenterRight | TopRight, // 0x0000001C
    Bottom = BottomLeft | BottomCenter | BottomRight, // 0x00000070
    Left = CenterLeft | BottomLeft | TopLeft, // 0x000000C1
    Edges = Left | Right | BottomCenter | TopCenter, // 0x000000FF
    All = Edges | Center, // 0x000001FF
    Default = Bottom | CenterRight | TopRight, // 0x0000007C
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.FlowBreak
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  [Flags]
  public enum FlowBreak
  {
    None = 0,
    BreakLine = 1,
    BreakMarginLeft = 2,
    BreakMarginRight = BreakMarginLeft | BreakLine, // 0x00000003
    BreakMarginLeftRight = 4,
    Before = 256, // 0x00000100
    After = 512, // 0x00000200
    BreakMask = 15, // 0x0000000F
  }
}

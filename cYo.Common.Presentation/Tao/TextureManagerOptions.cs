// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Tao.TextureManagerOptions
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;

#nullable disable
namespace cYo.Common.Presentation.Tao
{
  [Flags]
  public enum TextureManagerOptions
  {
    None = 0,
    MipMapFilter = 1,
    AnisotropicFilter = 2,
    TextureCompression = 4,
    SquareTextures = 8,
    BigTexturesAs16Bit = 16, // 0x00000010
    BigTexturesAs24Bit = 32, // 0x00000020
    Default = MipMapFilter, // 0x00000001
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Tao.TextureManagerSettings
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Mathematics;
using cYo.Common.Runtime;
using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Common.Presentation.Tao
{
  [Serializable]
  public class TextureManagerSettings : ICloneable
  {
    public TextureManagerSettings()
    {
      this.MaxTextureMemoryMB = 128;
      this.MaxTextureCount = 1024;
      this.MaxTextureTileSizeSquare = 512;
      this.MaxTextureTileSizeArbitrary = 16192;
      this.MinTextureTileSize = 16;
      this.TextureManagerOptions = TextureManagerOptions.MipMapFilter;
    }

    [CommandLineSwitch(ShortName = "hwmtm")]
    [DefaultValue(128)]
    public int MaxTextureMemoryMB { get; set; }

    public bool IsMaxTextureMemoryMBDefault => this.MaxTextureMemoryMB == 128;

    [CommandLineSwitch(ShortName = "hwmtc")]
    [DefaultValue(1024)]
    public int MaxTextureCount { get; set; }

    [CommandLineSwitch(ShortName = "hwmttsa")]
    [DefaultValue(16192)]
    public int MaxTextureTileSizeArbitrary { get; set; }

    [CommandLineSwitch(ShortName = "hwmttss")]
    [DefaultValue(512)]
    public int MaxTextureTileSizeSquare { get; set; }

    [CommandLineSwitch(ShortName = "hwmtts")]
    [DefaultValue(16)]
    public int MinTextureTileSize { get; set; }

    [CommandLineSwitch(ShortName = "hwo")]
    [DefaultValue(TextureManagerOptions.MipMapFilter)]
    public TextureManagerOptions TextureManagerOptions { get; set; }

    public bool IsTextureManagerOptionsDefault
    {
      get => this.TextureManagerOptions == TextureManagerOptions.MipMapFilter;
    }

    public bool MipMapping
    {
      get => (this.TextureManagerOptions & TextureManagerOptions.MipMapFilter) != 0;
      set
      {
        this.TextureManagerOptions = this.TextureManagerOptions.SetMask<TextureManagerOptions>(TextureManagerOptions.MipMapFilter, value);
      }
    }

    public int MaxTextureTileSize
    {
      get
      {
        return !this.IsSquareTextures ? this.MaxTextureTileSizeArbitrary : this.MaxTextureTileSizeSquare;
      }
    }

    public bool IsMipMapFilter
    {
      get => (this.TextureManagerOptions & TextureManagerOptions.MipMapFilter) != 0;
    }

    public bool IsAnisotropicFilter
    {
      get => (this.TextureManagerOptions & TextureManagerOptions.AnisotropicFilter) != 0;
    }

    public bool IsSquareTextures
    {
      get => (this.TextureManagerOptions & TextureManagerOptions.SquareTextures) != 0;
    }

    public bool IsTextureCompression
    {
      get => (this.TextureManagerOptions & TextureManagerOptions.TextureCompression) != 0;
    }

    public bool IsBigTexturesAs16Bit
    {
      get => (this.TextureManagerOptions & TextureManagerOptions.BigTexturesAs16Bit) != 0;
    }

    public bool IsBigTexturesAs24Bit
    {
      get => (this.TextureManagerOptions & TextureManagerOptions.BigTexturesAs24Bit) != 0;
    }

    public void Validate()
    {
      this.MaxTextureCount = this.MaxTextureCount.Clamp(64, 1024);
      this.MaxTextureMemoryMB = this.MaxTextureMemoryMB.Clamp(16, 1024);
      this.MaxTextureTileSizeArbitrary = this.MaxTextureTileSizeArbitrary.Clamp(64, OpenGlInfo.MaxTextureSize);
      this.MaxTextureTileSizeSquare = this.MaxTextureTileSizeSquare.Clamp(64, OpenGlInfo.MaxTextureSize);
      if (!OpenGlInfo.SupportsNonPower2Textures)
        this.TextureManagerOptions |= TextureManagerOptions.SquareTextures;
      if (!OpenGlInfo.SupportsAnisotopricFilter)
        this.TextureManagerOptions &= ~TextureManagerOptions.AnisotropicFilter;
      if (OpenGlInfo.SupportsTextureCompression)
        return;
      this.TextureManagerOptions &= ~TextureManagerOptions.TextureCompression;
    }

    public object Clone() => (object) CloneUtility.Clone<TextureManagerSettings>(this);
  }
}

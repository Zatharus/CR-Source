// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Tao.OpenGlInfo
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using Tao.OpenGl;

#nullable disable
namespace cYo.Common.Presentation.Tao
{
  public static class OpenGlInfo
  {
    private static int maxTextureSize;
    private static float version;
    private static bool? supportsAnisotropicFilter;
    private static bool? supportsNonPower2Textures;
    private static bool? supportsTextureCompression;

    public static int MaxTextureSize
    {
      get
      {
        if (OpenGlInfo.maxTextureSize == 0)
          Gl.glGetIntegerv(3379, out OpenGlInfo.maxTextureSize);
        return OpenGlInfo.maxTextureSize;
      }
    }

    public static float Version
    {
      get
      {
        if ((double) OpenGlInfo.version == 0.0)
        {
          try
          {
            string str = Gl.glGetString(7938).Trim();
            OpenGlInfo.version = float.Parse(str.Substring(0, 1)) + float.Parse(str.Substring(2, 1)) / 10f;
          }
          catch
          {
            OpenGlInfo.version = 1f;
          }
        }
        return OpenGlInfo.version;
      }
    }

    public static bool SupportsAnisotopricFilter
    {
      get
      {
        if (!OpenGlInfo.supportsAnisotropicFilter.HasValue)
          OpenGlInfo.supportsAnisotropicFilter = new bool?(Gl.IsExtensionSupported("GL_EXT_texture_filter_anisotropic"));
        return OpenGlInfo.supportsAnisotropicFilter.Value;
      }
    }

    public static bool SupportsNonPower2Textures
    {
      get
      {
        if (!OpenGlInfo.supportsNonPower2Textures.HasValue)
          OpenGlInfo.supportsNonPower2Textures = new bool?(Gl.IsExtensionSupported("ARB_texture_non_power_of_two"));
        return OpenGlInfo.supportsNonPower2Textures.Value;
      }
    }

    public static bool SupportsTextureCompression
    {
      get
      {
        if (!OpenGlInfo.supportsTextureCompression.HasValue)
          OpenGlInfo.supportsTextureCompression = new bool?(Gl.IsExtensionSupported("GL_ARB_texture_compression"));
        return OpenGlInfo.supportsTextureCompression.Value;
      }
    }
  }
}

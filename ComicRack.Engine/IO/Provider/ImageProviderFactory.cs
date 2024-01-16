// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.ImageProviderFactory
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public class ImageProviderFactory : ProviderFactory<ImageProvider>
  {
    public override ImageProvider CreateSourceProvider(string source)
    {
      try
      {
        ImageProvider sourceProvider1 = base.CreateSourceProvider(source);
        if (sourceProvider1 == null)
          return (ImageProvider) null;
        sourceProvider1.Source = source;
        if ((sourceProvider1.Capabilities & ImageProviderCapabilities.FastFormatCheck) == ImageProviderCapabilities.Nothing || sourceProvider1.FastFormatCheck(source))
          return sourceProvider1;
        ImageProvider sourceProvider2 = this.CreateProviders().FirstOrDefault<ImageProvider>((Func<ImageProvider, bool>) (t => (t.Capabilities & ImageProviderCapabilities.FastFormatCheck) != ImageProviderCapabilities.Nothing && t.FastFormatCheck(source)));
        if (sourceProvider2 != null)
        {
          sourceProvider2.Source = source;
          return sourceProvider2;
        }
      }
      catch
      {
      }
      return (ImageProvider) null;
    }
  }
}

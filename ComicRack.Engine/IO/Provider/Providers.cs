// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Providers
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public static class Providers
  {
    private static ImageProviderFactory readersFactory;
    private static ProviderFactory<StorageProvider> writersFactory;

    public static ImageProviderFactory Readers
    {
      get
      {
        if (Providers.readersFactory == null)
        {
          Providers.readersFactory = new ImageProviderFactory();
          Providers.readersFactory.RegisterProviders();
        }
        return Providers.readersFactory;
      }
    }

    public static ProviderFactory<StorageProvider> Writers
    {
      get
      {
        if (Providers.writersFactory == null)
        {
          Providers.writersFactory = new ProviderFactory<StorageProvider>();
          Providers.writersFactory.RegisterProviders();
        }
        return Providers.writersFactory;
      }
    }
  }
}

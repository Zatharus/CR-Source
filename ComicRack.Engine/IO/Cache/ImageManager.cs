// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Cache.ImageManager
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Cache
{
  public class ImageManager : ImageManagerBase<PageImage>
  {
    public ImageManager(int size)
      : base(size)
    {
    }

    protected override PageImage CreateNewFromProvider(ImageKey key, IImageProvider provider)
    {
      byte[] byteImage = provider.GetByteImage(key.Index);
      PageImage newFromProvider = byteImage != null ? PageImage.CreateFrom(byteImage) : PageImage.Wrap(provider.GetImage(key.Index));
      if (key.Rotation != ImageRotation.None)
      {
        Bitmap newImage = newFromProvider.Bitmap.Rotate(key.Rotation);
        newFromProvider.Dispose();
        newFromProvider = PageImage.Wrap(newImage);
      }
      return newFromProvider;
    }
  }
}

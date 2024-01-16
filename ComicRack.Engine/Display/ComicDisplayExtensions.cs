// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.ComicDisplayExtensions
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display
{
  public static class ComicDisplayExtensions
  {
    public static Bitmap CreateThumbnail(this IComicDisplay display)
    {
      return display.CreateThumbnail(new Size(0, 256));
    }

    public static Bitmap CreateThumbnail(this IComicDisplay display, Size size)
    {
      using (Bitmap pageImage = display.CreatePageImage())
        return pageImage.Scale(size, BitmapResampling.FastBicubic);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.PageImage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.IO;
using System.Drawing;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  public class PageImage : MemoryOptimizedImage, IDataSize
  {
    public static int SecondsToKeepDecodedImage = 5;
    public static bool MemoryOptimized = true;

    private PageImage(byte[] data)
      : base(data)
    {
      this.TimeToStay = PageImage.SecondsToKeepDecodedImage;
      this.Optimized = PageImage.MemoryOptimized;
      this.BackgrounColor = Color.Empty;
    }

    private PageImage(byte[] data, Bitmap newImage)
      : this(data)
    {
      this.Bitmap = newImage;
    }

    public Color BackgrounColor { get; set; }

    public static PageImage CreateFrom(string file) => new PageImage(File.ReadAllBytes(file));

    public static PageImage CreateFrom(Stream s) => new PageImage(s.ReadAllBytes());

    public static PageImage Wrap(Bitmap newImage)
    {
      return new PageImage(newImage.ImageToJpegBytes(), newImage);
    }

    public static PageImage CreateFrom(byte[] data) => new PageImage(data);

    public static PageImage CreateFrom(Bitmap bmp) => new PageImage(bmp.ImageToJpegBytes());

    public int DataSize
    {
      get
      {
        int dataSize = 0;
        if (this.Data != null)
          dataSize += this.Data.Length;
        if (this.IsImage)
          dataSize += this.Width * this.Height * 4;
        return dataSize;
      }
    }
  }
}

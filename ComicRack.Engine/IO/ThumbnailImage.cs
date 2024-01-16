// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.ThumbnailImage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  public class ThumbnailImage : MemoryOptimizedImage, IDataSize
  {
    public const int MaxHeight = 512;
    private int currentHeight;
    public static bool MemoryOptimized = true;
    public static int SecondsToKeepDecodedImage = 15;
    private static int lastRequestHeight;
    private volatile int dataSize;
    public static BitmapResampling Resampling = EngineConfiguration.Default.ThumbnailResampling;
    public static int ThumbnailQuality = EngineConfiguration.Default.ThumbnailQuality;

    public ThumbnailImage(byte[] data, Size size, Size originalSize)
      : base(data, size)
    {
      this.OriginalSize = originalSize;
      this.TimeToStay = ThumbnailImage.SecondsToKeepDecodedImage;
      this.Optimized = ThumbnailImage.MemoryOptimized;
    }

    public Size OriginalSize { get; set; }

    public override Bitmap Bitmap
    {
      get => this.GetThumbnail(512);
      set
      {
        base.Bitmap = value;
        if (value != null)
          return;
        this.dataSize = this.Data.Length;
      }
    }

    public Bitmap GetThumbnail(int height)
    {
      if (this.Data == null)
        return (Bitmap) null;
      float num = 512f;
      while ((double) num * 0.75 >= (double) height)
        num *= 0.75f;
      int height1 = (int) num;
      Bitmap thumbnail = base.Bitmap;
      if (height1 == this.currentHeight && thumbnail != null)
        return thumbnail;
      using (Bitmap bmp = BitmapExtensions.BitmapFromBytes(this.Data))
      {
        Size size = new Size(bmp.Width * height1 / bmp.Height, height1);
        thumbnail = bmp.Scale(size, EngineConfiguration.Default.ThumbnailResampling).ToOptimized();
      }
      ThumbnailImage.lastRequestHeight = this.currentHeight = height1;
      return base.Bitmap = thumbnail;
    }

    protected override Bitmap OnCreateImage(byte[] data) => (Bitmap) null;

    public Size GetThumbnailSize(int height) => this.Size.ToRectangle(new Size(0, height)).Size;

    public override void Save(Stream s)
    {
      BinaryWriter binaryWriter = new BinaryWriter(s);
      binaryWriter.Write(this.Bitmap.Size.Width);
      binaryWriter.Write(this.Bitmap.Size.Height);
      binaryWriter.Write(this.OriginalSize.Width);
      binaryWriter.Write(this.OriginalSize.Height);
      binaryWriter.Write(this.Data.Length);
      s.Write(this.Data, 0, this.Data.Length);
    }

    public override byte[] ToBytes()
    {
      using (MemoryStream s = new MemoryStream(this.Data.Length + 100))
      {
        this.Save((Stream) s);
        return s.ToArray();
      }
    }

    public int DataSize => this.dataSize;

    public static ThumbnailImage CreateFrom(string file)
    {
      using (FileStream fileStream = File.OpenRead(file))
        return ThumbnailImage.CreateFrom((Stream) fileStream);
    }

    public static ThumbnailImage CreateFrom(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream(data))
        return ThumbnailImage.CreateFrom((Stream) memoryStream);
    }

    public static ThumbnailImage CreateFrom(Stream stream)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      Size size = new Size(binaryReader.ReadInt32(), binaryReader.ReadInt32());
      Size originalSize = new Size(binaryReader.ReadInt32(), binaryReader.ReadInt32());
      int count = binaryReader.ReadInt32();
      ThumbnailImage from = new ThumbnailImage(binaryReader.ReadBytes(count), size, originalSize);
      if (ThumbnailImage.lastRequestHeight != 0)
        from.GetThumbnail(ThumbnailImage.lastRequestHeight);
      return from;
    }

    public static ThumbnailImage CreateFrom(
      Bitmap image,
      Size originalSize,
      bool supportTransparent = false)
    {
      if (image == null)
        return (ThumbnailImage) null;
      ThumbnailImage from;
      using (Image image1 = (Image) ThumbnailImage.Scale(image, new Size(0, 512)))
        from = !supportTransparent ? new ThumbnailImage(image1.ImageToJpegBytes(ThumbnailImage.ThumbnailQuality), image1.Size, originalSize) : new ThumbnailImage(image1.ImageToBytes(ImageFormat.Png), image1.Size, originalSize);
      if (ThumbnailImage.lastRequestHeight != 0)
        from.GetThumbnail(ThumbnailImage.lastRequestHeight);
      return from;
    }

    private static Bitmap Scale(Bitmap bitmap, Size size)
    {
      return bitmap.Scale(size, ThumbnailImage.Resampling);
    }
  }
}

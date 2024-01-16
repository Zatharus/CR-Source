// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.FastBitmap
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;

#nullable disable
namespace cYo.Common.Drawing
{
  public class FastBitmap : DisposableObject
  {
    private int width;
    private BitmapData bitmapData;
    private unsafe byte* pBase = (byte*) null;
    private readonly Bitmap bitmap;
    private unsafe FastBitmap.PixelData* pixelData = (FastBitmap.PixelData*) null;

    public FastBitmap(Bitmap inputBitmap, bool lockBitmap = true)
    {
      this.bitmap = inputBitmap;
      if (!lockBitmap)
        return;
      this.LockImage();
    }

    protected override void Dispose(bool disposing)
    {
      this.UnlockImage();
      base.Dispose(disposing);
    }

    public Bitmap Bitmap => this.bitmap;

    public unsafe void LockImage()
    {
      if (this.bitmapData != null)
        return;
      Rectangle rect = new Rectangle(Point.Empty, this.bitmap.Size);
      this.width = rect.Width * sizeof (FastBitmap.PixelData);
      if (this.width % 4 != 0)
        this.width = 4 * (this.width / 4 + 1);
      this.bitmapData = this.bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
      this.pBase = (byte*) this.bitmapData.Scan0.ToPointer();
    }

    public unsafe Color GetPixel(int x, int y)
    {
      this.pixelData = (FastBitmap.PixelData*) (this.pBase + y * this.width + x * sizeof (FastBitmap.PixelData));
      return Color.FromArgb((int) this.pixelData->Alpha, (int) this.pixelData->Red, (int) this.pixelData->Green, (int) this.pixelData->Blue);
    }

    public unsafe Color GetPixelNext()
    {
      ++this.pixelData;
      return Color.FromArgb((int) this.pixelData->Alpha, (int) this.pixelData->Red, (int) this.pixelData->Green, (int) this.pixelData->Blue);
    }

    public unsafe void SetPixel(int x, int y, Color color)
    {
      FastBitmap.PixelData* pixelDataPtr = (FastBitmap.PixelData*) (this.pBase + y * this.width + x * sizeof (FastBitmap.PixelData));
      pixelDataPtr->Alpha = color.A;
      pixelDataPtr->Green = color.G;
      pixelDataPtr->Blue = color.B;
      pixelDataPtr->Red = color.R;
    }

    public unsafe void UnlockImage()
    {
      if (this.bitmapData == null)
        return;
      this.bitmap.UnlockBits(this.bitmapData);
      this.bitmapData = (BitmapData) null;
      this.pBase = (byte*) null;
    }

    private struct PixelData
    {
      public byte Blue;
      public byte Green;
      public byte Red;
      public byte Alpha;

      public override string ToString()
      {
        return "(" + (object) this.Alpha + ", " + (object) this.Red + ", " + (object) this.Green + ", " + (object) this.Blue + ")";
      }
    }
  }
}

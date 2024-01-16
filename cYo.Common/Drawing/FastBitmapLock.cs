// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.FastBitmapLock
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Drawing
{
  public sealed class FastBitmapLock : DisposableObject
  {
    private readonly IntPtr scan = IntPtr.Zero;
    private readonly Bitmap bitmap;
    private readonly BitmapData bitmapData;
    private readonly bool bitmapOwned;
    private readonly IntPtr data;
    private readonly int size;
    private readonly int width;
    private readonly int height;

    public FastBitmapLock(Bitmap bmp, Rectangle rc)
      : this(bmp, rc, false)
    {
    }

    public unsafe FastBitmapLock(Bitmap bmp, Rectangle rc, bool allowWrite)
    {
      try
      {
        if (rc.Width <= 0 || rc.Height <= 0 || rc.X >= bmp.Width || rc.Y >= bmp.Height)
          return;
        this.bitmap = bmp;
        this.width = rc.Width;
        this.height = rc.Height;
        this.size = this.width * this.height * 4;
        if (bmp.PixelFormat != PixelFormat.Format32bppArgb && bmp.PixelFormat != PixelFormat.Format24bppRgb)
        {
          this.bitmap = bmp.CreateCopy(rc, PixelFormat.Format32bppArgb);
          this.bitmapOwned = true;
          rc.Location = Point.Empty;
        }
        this.bitmapData = this.bitmap.LockBits(new Rectangle(0, 0, this.bitmap.Width, this.bitmap.Height), allowWrite ? ImageLockMode.ReadWrite : ImageLockMode.ReadOnly, this.bitmap.PixelFormat);
        this.data = this.scan = this.bitmapData.Scan0;
        if (rc.Location.IsEmpty && rc.Size == this.bitmap.Size && this.bitmap.PixelFormat == PixelFormat.Format32bppArgb)
          return;
        int num1 = Math.Min(this.width, bmp.Width - rc.X);
        int num2 = Math.Min(this.height, bmp.Height - rc.Y);
        int stride = this.bitmapData.Stride;
        int x = rc.X;
        int num3 = stride / this.bitmap.Width;
        this.data = Marshal.AllocHGlobal(this.size);
        try
        {
          byte* numPtr1 = (byte*) ((IntPtr) (void*) this.scan + stride * rc.Y);
          switch (num3)
          {
            case 3:
              byte* data1 = (byte*) (void*) this.data;
              for (int index1 = 0; index1 < num2; ++index1)
              {
                byte* numPtr2 = numPtr1 + x * num3;
                byte* numPtr3 = data1;
                for (int index2 = 0; index2 < num1; ++index2)
                {
                  byte* numPtr4 = numPtr3;
                  byte* numPtr5 = numPtr4 + 1;
                  byte* numPtr6 = numPtr2;
                  byte* numPtr7 = numPtr6 + 1;
                  int num4 = (int) *numPtr6;
                  *numPtr4 = (byte) num4;
                  byte* numPtr8 = numPtr5;
                  byte* numPtr9 = numPtr8 + 1;
                  byte* numPtr10 = numPtr7;
                  byte* numPtr11 = numPtr10 + 1;
                  int num5 = (int) *numPtr10;
                  *numPtr8 = (byte) num5;
                  byte* numPtr12 = numPtr9;
                  byte* numPtr13 = numPtr12 + 1;
                  byte* numPtr14 = numPtr11;
                  numPtr2 = numPtr14 + 1;
                  int num6 = (int) *numPtr14;
                  *numPtr12 = (byte) num6;
                  byte* numPtr15 = numPtr13;
                  numPtr3 = numPtr15 + 1;
                  *numPtr15 = byte.MaxValue;
                }
                data1 += this.width << 2;
                numPtr1 += stride;
              }
              break;
            case 4:
              int* data2 = (int*) (void*) this.data;
              for (int index3 = 0; index3 < num2; ++index3)
              {
                int* numPtr16 = (int*) (numPtr1 + ((IntPtr) x * 4).ToInt64());
                int* numPtr17 = data2;
                for (int index4 = 0; index4 < num1; ++index4)
                  *numPtr17++ = *numPtr16++;
                data2 += this.width;
                numPtr1 += stride;
              }
              break;
          }
        }
        finally
        {
          BitmapData bitmapData = this.bitmapData;
          this.bitmapData = (BitmapData) null;
          this.bitmap.UnlockBits(bitmapData);
        }
      }
      catch (Exception ex)
      {
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (this.data != this.scan)
        Marshal.FreeHGlobal(this.data);
      if (this.bitmapData != null)
        this.bitmap.UnlockBits(this.bitmapData);
      if (this.bitmapOwned && this.bitmap != null)
        this.bitmap.Dispose();
      base.Dispose(disposing);
    }

    public IntPtr Data => this.data;

    public int Size => this.size;

    public int Width => this.width;

    public int Height => this.height;
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.WebpImage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public static class WebpImage
  {
    private static readonly byte[] header1 = Encoding.ASCII.GetBytes("RIFF");
    private static readonly byte[] header2 = Encoding.ASCII.GetBytes("WEBP");

    public static unsafe Bitmap DecodeFromBytes(byte[] data, long length)
    {
      fixed (byte* data1 = data)
        return WebpImage.DecodeFromPointer((IntPtr) (void*) data1, length);
    }

    private static Bitmap DecodeFromPointer(IntPtr data, long length)
    {
      int width = 0;
      int height = 0;
      if (WebpImage.NativeMethods.WebPGetInfo(data, (UIntPtr) (ulong) length, ref width, ref height) == 0)
        throw new Exception("Invalid WebP header detected");
      bool flag = false;
      Bitmap bitmap = (Bitmap) null;
      BitmapData bitmapdata = (BitmapData) null;
      try
      {
        bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        IntPtr num = WebpImage.NativeMethods.WebPDecodeBGRAInto(data, (UIntPtr) (ulong) length, bitmapdata.Scan0, (UIntPtr) (ulong) (bitmapdata.Stride * bitmapdata.Height), bitmapdata.Stride);
        if (bitmapdata.Scan0 != num)
          throw new Exception("Failed to decode WebP image with error " + (object) (long) num);
        flag = true;
      }
      finally
      {
        if (bitmapdata != null)
          bitmap.UnlockBits(bitmapdata);
        if (!flag && bitmap != null)
          bitmap.Dispose();
      }
      return bitmap;
    }

    private static void Encode(Bitmap from, Stream to, int quality)
    {
      IntPtr result;
      long length;
      WebpImage.Encode(from, (float) quality, out result, out length);
      try
      {
        byte[] numArray = new byte[4096];
        for (int index = 0; (long) index < length; index += numArray.Length)
        {
          int num = (int) Math.Min((long) numArray.Length, length - (long) index);
          Marshal.Copy((IntPtr) ((long) result + (long) index), numArray, 0, num);
          to.Write(numArray, 0, num);
        }
      }
      finally
      {
        WebpImage.NativeMethods.WebPFree(result);
      }
    }

    private static void Encode(Bitmap b, float quality, out IntPtr result, out long length)
    {
      if ((double) quality > 100.0)
        quality = 100f;
      int width = b.Width;
      int height = b.Height;
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, b.PixelFormat);
      try
      {
        result = IntPtr.Zero;
        switch (b.PixelFormat)
        {
          case PixelFormat.Format24bppRgb:
            length = (double) quality > 0.0 ? (long) (ulong) WebpImage.NativeMethods.WebPEncodeBGR(bitmapdata.Scan0, width, height, bitmapdata.Stride, quality, ref result) : (long) (ulong) WebpImage.NativeMethods.WebPEncodeLosslessBGR(bitmapdata.Scan0, width, height, bitmapdata.Stride, ref result);
            break;
          case PixelFormat.Format32bppRgb:
          case PixelFormat.Format32bppArgb:
            length = (double) quality > 0.0 ? (long) WebpImage.NativeMethods.WebPEncodeBGRA(bitmapdata.Scan0, width, height, bitmapdata.Stride, quality, ref result) : (long) (ulong) WebpImage.NativeMethods.WebPEncodeLosslessBGRA(bitmapdata.Scan0, width, height, bitmapdata.Stride, ref result);
            break;
          default:
            throw new NotSupportedException("Only Format32bppArgb and Format32bppRgb bitmaps are supported");
        }
        if (length == 0L)
          throw new Exception("WebP encode failed!");
      }
      finally
      {
        b.UnlockBits(bitmapdata);
      }
    }

    public static byte[] ConvertToJpeg(byte[] data)
    {
      if (!WebpImage.IsWebp(data))
        return data;
      try
      {
        using (Bitmap image = WebpImage.DecodeFromBytes(data, (long) data.Length))
          return image.ImageToJpegBytes();
      }
      catch (Exception ex)
      {
        return data;
      }
    }

    public static bool IsWebp(byte[] data)
    {
      if (data.Length < 12)
        return false;
      for (int index = 0; index < WebpImage.header1.Length; ++index)
      {
        if ((int) data[index] != (int) WebpImage.header1[index])
          return false;
      }
      for (int index = 0; index < WebpImage.header2.Length; ++index)
      {
        if ((int) data[index + 8] != (int) WebpImage.header2[index])
          return false;
      }
      return true;
    }

    public static byte[] ConvertoToWebp(Bitmap bmp, int quality = 75)
    {
      if (bmp == null)
        return (byte[]) null;
      Bitmap from = (Bitmap) null;
      try
      {
        using (MemoryStream to = new MemoryStream())
        {
          from = bmp.PixelFormat == PixelFormat.Format24bppRgb ? bmp : bmp.CreateCopy(PixelFormat.Format24bppRgb);
          WebpImage.Encode(from, (Stream) to, quality);
          return to.ToArray();
        }
      }
      finally
      {
        if (bmp != from)
          from.SafeDispose();
      }
    }

    private static class NativeMethods
    {
      [DllImport("Resources\\libwebp32.dll", EntryPoint = "WebPFree")]
      private static extern void WebPFree32(IntPtr toDeallocate);

      [DllImport("Resources\\libwebp64.dll", EntryPoint = "WebPFree")]
      private static extern void WebPFree64(IntPtr toDeallocate);

      [DllImport("Resources\\libwebp32.dll", EntryPoint = "WebPGetInfo")]
      private static extern int WebPGetInfo32(
        [In] IntPtr data,
        UIntPtr dataSize,
        ref int width,
        ref int height);

      [DllImport("Resources\\libwebp64.dll", EntryPoint = "WebPGetInfo")]
      private static extern int WebPGetInfo64(
        [In] IntPtr data,
        UIntPtr dataSize,
        ref int width,
        ref int height);

      [DllImport("Resources\\libwebp32.dll", EntryPoint = "WebPDecodeBGRAInto")]
      private static extern IntPtr WebPDecodeBGRAInto32(
        [In] IntPtr data,
        UIntPtr dataSize,
        IntPtr outputBuffer,
        UIntPtr outputBufferSize,
        int outputStride);

      [DllImport("Resources\\libwebp64.dll", EntryPoint = "WebPDecodeBGRAInto")]
      private static extern IntPtr WebPDecodeBGRAInto64(
        [In] IntPtr data,
        UIntPtr dataSize,
        IntPtr outputBuffer,
        UIntPtr outputBufferSize,
        int outputStride);

      [DllImport("Resources\\libwebp32.dll", EntryPoint = "WebPEncodeLosslessBGR")]
      private static extern UIntPtr WebPEncodeLosslessBGR32(
        [In] IntPtr bgr,
        int width,
        int height,
        int stride,
        ref IntPtr output);

      [DllImport("Resources\\libwebp64.dll", EntryPoint = "WebPEncodeLosslessBGR")]
      private static extern UIntPtr WebPEncodeLosslessBGR64(
        [In] IntPtr bgr,
        int width,
        int height,
        int stride,
        ref IntPtr output);

      [DllImport("Resources\\libwebp32.dll", EntryPoint = "WebPEncodeLosslessBGRA")]
      private static extern UIntPtr WebPEncodeLosslessBGRA32(
        [In] IntPtr bgra,
        int width,
        int height,
        int stride,
        ref IntPtr output);

      [DllImport("Resources\\libwebp64.dll", EntryPoint = "WebPEncodeLosslessBGRA")]
      private static extern UIntPtr WebPEncodeLosslessBGRA64(
        [In] IntPtr bgra,
        int width,
        int height,
        int stride,
        ref IntPtr output);

      [DllImport("Resources\\libwebp32.dll", EntryPoint = "WebPEncodeBGR")]
      private static extern UIntPtr WebPEncodeBGR32(
        [In] IntPtr bgr,
        int width,
        int height,
        int stride,
        float qualityFactor,
        ref IntPtr output);

      [DllImport("Resources\\libwebp64.dll", EntryPoint = "WebPEncodeBGR")]
      private static extern UIntPtr WebPEncodeBGR64(
        [In] IntPtr bgr,
        int width,
        int height,
        int stride,
        float qualityFactor,
        ref IntPtr output);

      [DllImport("Resources\\libwebp32.dll", EntryPoint = "WebPEncodeBGRA")]
      private static extern IntPtr WebPEncodeBGRA32(
        [In] IntPtr bgra,
        int width,
        int height,
        int stride,
        float qualityFactor,
        ref IntPtr output);

      [DllImport("Resources\\libwebp64.dll", EntryPoint = "WebPEncodeBGRA")]
      private static extern IntPtr WebPEncodeBGRA64(
        [In] IntPtr bgra,
        int width,
        int height,
        int stride,
        float qualityFactor,
        ref IntPtr output);

      public static int WebPGetInfo(IntPtr data, UIntPtr dataSize, ref int width, ref int height)
      {
        return !Environment.Is64BitProcess ? WebpImage.NativeMethods.WebPGetInfo32(data, dataSize, ref width, ref height) : WebpImage.NativeMethods.WebPGetInfo64(data, dataSize, ref width, ref height);
      }

      public static void WebPFree(IntPtr toDeallocate)
      {
        if (Environment.Is64BitProcess)
          WebpImage.NativeMethods.WebPFree64(toDeallocate);
        else
          WebpImage.NativeMethods.WebPFree32(toDeallocate);
      }

      public static IntPtr WebPDecodeBGRAInto(
        IntPtr data,
        UIntPtr dataSize,
        IntPtr outputBuffer,
        UIntPtr outputBufferSize,
        int outputStride)
      {
        return !Environment.Is64BitProcess ? WebpImage.NativeMethods.WebPDecodeBGRAInto32(data, dataSize, outputBuffer, outputBufferSize, outputStride) : WebpImage.NativeMethods.WebPDecodeBGRAInto64(data, dataSize, outputBuffer, outputBufferSize, outputStride);
      }

      public static UIntPtr WebPEncodeLosslessBGR(
        IntPtr bgr,
        int width,
        int height,
        int stride,
        ref IntPtr output)
      {
        return !Environment.Is64BitProcess ? WebpImage.NativeMethods.WebPEncodeLosslessBGR32(bgr, width, height, stride, ref output) : WebpImage.NativeMethods.WebPEncodeLosslessBGR64(bgr, width, height, stride, ref output);
      }

      public static UIntPtr WebPEncodeLosslessBGRA(
        IntPtr bgra,
        int width,
        int height,
        int stride,
        ref IntPtr output)
      {
        return !Environment.Is64BitProcess ? WebpImage.NativeMethods.WebPEncodeLosslessBGRA32(bgra, width, height, stride, ref output) : WebpImage.NativeMethods.WebPEncodeLosslessBGRA64(bgra, width, height, stride, ref output);
      }

      public static UIntPtr WebPEncodeBGR(
        IntPtr bgr,
        int width,
        int height,
        int stride,
        float qualityFactor,
        ref IntPtr output)
      {
        return !Environment.Is64BitProcess ? WebpImage.NativeMethods.WebPEncodeBGR32(bgr, width, height, stride, qualityFactor, ref output) : WebpImage.NativeMethods.WebPEncodeBGR64(bgr, width, height, stride, qualityFactor, ref output);
      }

      public static IntPtr WebPEncodeBGRA(
        IntPtr bgra,
        int width,
        int height,
        int stride,
        float qualityFactor,
        ref IntPtr output)
      {
        return !Environment.Is64BitProcess ? WebpImage.NativeMethods.WebPEncodeBGRA32(bgra, width, height, stride, qualityFactor, ref output) : WebpImage.NativeMethods.WebPEncodeBGRA64(bgra, width, height, stride, qualityFactor, ref output);
      }
    }
  }
}

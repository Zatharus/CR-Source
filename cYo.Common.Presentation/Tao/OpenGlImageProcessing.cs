// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Tao.OpenGlImageProcessing
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Drawing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Tao.OpenGl;
using Tao.Platform.Windows;

#nullable disable
namespace cYo.Common.Presentation.Tao
{
  public static class OpenGlImageProcessing
  {
    public static System.Drawing.Bitmap Resize(System.Drawing.Bitmap bmp, int width, int height)
    {
      OpenGlImageProcessing.InitializeOpenGl();
      System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap) null;
      if (bmp.PixelFormat != PixelFormat.Format32bppArgb && bmp.PixelFormat != PixelFormat.Format24bppRgb)
        bmp = bitmap = bmp.CreateCopy(PixelFormat.Format32bppArgb);
      try
      {
        System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb);
        bool flag;
        using (FastBitmapLock fastBitmapLock1 = new FastBitmapLock(bmp, bmp.Size.ToRectangle()))
        {
          using (FastBitmapLock fastBitmapLock2 = new FastBitmapLock(bmp1, bmp1.Size.ToRectangle(), true))
          {
            Glu.gluScaleImage(32993, fastBitmapLock1.Width, fastBitmapLock1.Height, 5121, fastBitmapLock1.Data, fastBitmapLock2.Width, fastBitmapLock2.Height, 5121, fastBitmapLock2.Data);
            flag = Gl.glGetError() != 0;
          }
        }
        if (flag)
        {
          using (Graphics graphics = Graphics.FromImage((Image) bmp1))
          {
            using (graphics.HighQuality(true, (SizeF) new Size(width, height), (SizeF) bmp.Size))
              graphics.DrawImage((Image) bmp, new Rectangle(0, 0, width, height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel);
          }
        }
        return bmp1;
      }
      finally
      {
        if (bitmap == bmp)
          bitmap.Dispose();
      }
    }

    private static void InitializeOpenGl()
    {
      IntPtr num = IntPtr.Zero;
      IntPtr newContext = IntPtr.Zero;
      if (Wgl.wglGetCurrentContext() != IntPtr.Zero)
        return;
      if (num == IntPtr.Zero)
      {
        Gdi.PIXELFORMATDESCRIPTOR pixelFormatDescriptor = new Gdi.PIXELFORMATDESCRIPTOR();
        pixelFormatDescriptor.nSize = (short) Marshal.SizeOf((object) pixelFormatDescriptor);
        pixelFormatDescriptor.nVersion = (short) 1;
        pixelFormatDescriptor.dwFlags = 33;
        pixelFormatDescriptor.cColorBits = (byte) 8;
        num = User.GetDC(IntPtr.Zero);
        int pixelFormat = Gdi.ChoosePixelFormat(num, ref pixelFormatDescriptor);
        Gdi.SetPixelFormat(num, pixelFormat, ref pixelFormatDescriptor);
      }
      if (newContext == IntPtr.Zero)
        newContext = Wgl.wglCreateContext(num);
      Wgl.wglMakeCurrent(num, newContext);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.LayeredForm
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class LayeredForm : Form
  {
    private Bitmap surface;
    private int alpha;

    public Bitmap Surface
    {
      get => this.surface;
      set
      {
        if (this.surface == value)
          return;
        this.surface = value;
        this.Width = this.surface.Width;
        this.Height = this.surface.Height;
        this.UpdateSurface();
      }
    }

    public int Alpha
    {
      get => this.alpha;
      set
      {
        if (this.alpha == value)
          return;
        this.alpha = value;
        this.Invalidate();
      }
    }

    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams createParams = base.CreateParams;
        if (!this.DesignMode)
          createParams.ExStyle |= 524288;
        return createParams;
      }
    }

    protected override void OnInvalidated(InvalidateEventArgs e)
    {
      base.OnInvalidated(e);
      if (this.DesignMode)
        return;
      this.UpdateSurface();
    }

    private void UpdateSurface()
    {
      try
      {
        if (this.InvokeIfRequired(new Action(this.UpdateSurface)))
          return;
        using (Bitmap bitmap = new Bitmap(this.Width, this.Height))
        {
          using (Graphics graphics = Graphics.FromImage((Image) bitmap))
          {
            graphics.Clear(Color.Transparent);
            if (this.surface != null)
              graphics.DrawImage((Image) this.surface, this.surface.Size.ToRectangle());
            this.OnPaint(new PaintEventArgs(graphics, bitmap.Size.ToRectangle()));
          }
          LayeredForm.LayeredApi.SelectBitmap((Form) this, bitmap, this.alpha);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public static class LayeredApi
    {
      public const int WS_EX_LAYERED = 524288;
      public const int HTCAPTION = 2;
      public const int WM_NCHITTEST = 132;
      public const int ULW_ALPHA = 2;
      public const byte AC_SRC_OVER = 0;
      public const byte AC_SRC_ALPHA = 1;

      [DllImport("user32.dll", SetLastError = true)]
      private static extern LayeredForm.LayeredApi.Bool UpdateLayeredWindow(
        IntPtr hwnd,
        IntPtr hdcDst,
        ref LayeredForm.LayeredApi.Point pptDst,
        ref LayeredForm.LayeredApi.Size psize,
        IntPtr hdcSrc,
        ref LayeredForm.LayeredApi.Point pprSrc,
        int crKey,
        ref LayeredForm.LayeredApi.BLENDFUNCTION pblend,
        int dwFlags);

      [DllImport("gdi32.dll", SetLastError = true)]
      private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

      [DllImport("user32.dll", SetLastError = true)]
      private static extern IntPtr GetDC(IntPtr hWnd);

      [DllImport("user32.dll")]
      private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

      [DllImport("gdi32.dll", SetLastError = true)]
      private static extern LayeredForm.LayeredApi.Bool DeleteDC(IntPtr hdc);

      [DllImport("gdi32.dll")]
      private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

      [DllImport("gdi32.dll", SetLastError = true)]
      private static extern LayeredForm.LayeredApi.Bool DeleteObject(IntPtr hObject);

      public static void SelectBitmap(Form form, Bitmap bitmap, int alpha)
      {
        IntPtr dc = LayeredForm.LayeredApi.GetDC(IntPtr.Zero);
        IntPtr compatibleDc = LayeredForm.LayeredApi.CreateCompatibleDC(dc);
        IntPtr hObject1 = IntPtr.Zero;
        IntPtr hObject2 = IntPtr.Zero;
        try
        {
          hObject1 = bitmap.GetHbitmap(Color.FromArgb(0));
          hObject2 = LayeredForm.LayeredApi.SelectObject(compatibleDc, hObject1);
          LayeredForm.LayeredApi.Size psize = new LayeredForm.LayeredApi.Size(bitmap.Width, bitmap.Height);
          LayeredForm.LayeredApi.Point pprSrc = new LayeredForm.LayeredApi.Point(0, 0);
          LayeredForm.LayeredApi.Point pptDst = new LayeredForm.LayeredApi.Point(form.Left, form.Top);
          int num = (int) LayeredForm.LayeredApi.UpdateLayeredWindow(form.Handle, dc, ref pptDst, ref psize, compatibleDc, ref pprSrc, 0, ref new LayeredForm.LayeredApi.BLENDFUNCTION()
          {
            BlendOp = (byte) 0,
            BlendFlags = (byte) 0,
            SourceConstantAlpha = (byte) alpha,
            AlphaFormat = (byte) 1
          }, 2);
        }
        finally
        {
          LayeredForm.LayeredApi.ReleaseDC(IntPtr.Zero, dc);
          if (hObject1 != IntPtr.Zero)
          {
            LayeredForm.LayeredApi.SelectObject(compatibleDc, hObject2);
            int num = (int) LayeredForm.LayeredApi.DeleteObject(hObject1);
          }
          int num1 = (int) LayeredForm.LayeredApi.DeleteDC(compatibleDc);
        }
      }

      private enum Bool
      {
        False,
        True,
      }

      private struct Point
      {
        public readonly int x;
        public readonly int y;

        public Point(int x, int y)
        {
          this.x = x;
          this.y = y;
        }
      }

      private struct Size
      {
        public readonly int cx;
        public readonly int cy;

        public Size(int cx, int cy)
        {
          this.cx = cx;
          this.cy = cy;
        }
      }

      [StructLayout(LayoutKind.Sequential, Pack = 1)]
      private struct ARGB
      {
        public readonly byte Blue;
        public readonly byte Green;
        public readonly byte Red;
        public readonly byte Alpha;
      }

      [StructLayout(LayoutKind.Sequential, Pack = 1)]
      private struct BLENDFUNCTION
      {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Tao.ControlOpenGlRenderer
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

#nullable disable
namespace cYo.Common.Presentation.Tao
{
  public class ControlOpenGlRenderer : 
    DisposableObject,
    IControlRenderer,
    IBitmapRenderer,
    IDisposable,
    IHardwareRenderer
  {
    private TextureManager tm;
    private IntPtr deviceContext = IntPtr.Zero;
    private IntPtr renderingContext = IntPtr.Zero;
    private IntPtr windowHandle = IntPtr.Zero;
    private bool isSoftwareRenderer;
    private int errorCode;
    private int sceneCounter;
    private float opacity = 1f;
    private CompositingMode compositingMode;
    private RectangleF clip = (RectangleF) Rectangle.Empty;
    private StencilMode stencilMode;

    public ControlOpenGlRenderer(
      Control window,
      bool registerPaint,
      TextureManagerSettings settings)
    {
      this.ColorBits = 32;
      this.AutoReshape = true;
      this.AutoFinish = true;
      this.AutoSwapBuffers = true;
      this.AutoMakeCurrent = true;
      this.StencilBits = 1;
      ControlOpenGlRenderer.SetStyle(window, ControlStyles.ResizeRedraw, true);
      ControlOpenGlRenderer.SetStyle(window, ControlStyles.OptimizedDoubleBuffer, false);
      ControlOpenGlRenderer.SetStyle(window, ControlStyles.AllPaintingInWmPaint, true);
      ControlOpenGlRenderer.SetStyle(window, ControlStyles.UserPaint, true);
      this.Settings = settings;
      this.InitializeContexts(window);
      if (registerPaint)
        window.Paint += new PaintEventHandler(this.window_Paint);
      window.Disposed += new EventHandler(this.window_Disposed);
      this.BlendingOperation = BlendingOperation.Blend;
      this.OnInitOpenGl();
    }

    protected override void Dispose(bool disposing)
    {
      if (this.Control != null)
      {
        this.Control.Paint -= new PaintEventHandler(this.window_Paint);
        this.Control.Disposed -= new EventHandler(this.window_Disposed);
        this.Control = (Control) null;
      }
      if (this.tm != null)
      {
        this.tm.Dispose();
        this.tm = (TextureManager) null;
      }
      this.DestroyContexts();
    }

    public int ColorBits { get; set; }

    public int AccumBits { get; set; }

    public int DepthBits { get; set; }

    public int StencilBits { get; set; }

    public bool AutoMakeCurrent { get; set; }

    public bool AutoSwapBuffers { get; set; }

    public bool AutoFinish { get; set; }

    public bool AutoReshape { get; set; }

    public int ErrorCode => this.errorCode;

    public Size Size => this.Control.ClientRectangle.Size;

    public Control Control { get; private set; }

    public TextureManagerSettings Settings { get; private set; }

    public void MakeCurrent()
    {
      if (!Wgl.wglMakeCurrent(this.deviceContext, this.renderingContext))
        throw new InvalidOperationException("Can not activate the GL rendering context.");
    }

    public void SwapBuffers() => Gdi.SwapBuffersFast(this.deviceContext);

    public void Draw() => this.Control.Invalidate();

    public void ReshapeFunc()
    {
      Size size1 = this.Size;
      int width1 = size1.Width;
      size1 = this.Size;
      int height1 = size1.Height;
      Gl.glViewport(0, 0, width1, height1);
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Size size2 = this.Size;
      double width2 = (double) size2.Width;
      size2 = this.Size;
      double height2 = (double) size2.Height;
      Gl.glOrtho(0.0, width2, height2, 0.0, 0.0, 1.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
    }

    private void InitializeContexts(Control window)
    {
      this.Control = window != null ? window : throw new ArgumentNullException(nameof (window), "No valid window handle");
      if (this.Control.Handle == IntPtr.Zero)
        throw new InvalidOperationException("Window not created");
      this.windowHandle = window.Handle;
      Gdi.PIXELFORMATDESCRIPTOR structure = new Gdi.PIXELFORMATDESCRIPTOR();
      structure.nSize = (short) Marshal.SizeOf((object) structure);
      structure.nVersion = (short) 1;
      structure.dwFlags = 37;
      structure.iPixelType = (byte) 0;
      structure.cColorBits = (byte) this.ColorBits;
      structure.cRedBits = (byte) 0;
      structure.cRedShift = (byte) 0;
      structure.cGreenBits = (byte) 0;
      structure.cGreenShift = (byte) 0;
      structure.cBlueBits = (byte) 0;
      structure.cBlueShift = (byte) 0;
      structure.cAlphaBits = (byte) 0;
      structure.cAlphaShift = (byte) 0;
      structure.cAccumBits = (byte) this.AccumBits;
      structure.cAccumRedBits = (byte) 0;
      structure.cAccumGreenBits = (byte) 0;
      structure.cAccumBlueBits = (byte) 0;
      structure.cAccumAlphaBits = (byte) 0;
      structure.cDepthBits = (byte) this.DepthBits;
      structure.cStencilBits = (byte) this.StencilBits;
      structure.cAuxBuffers = (byte) 0;
      structure.iLayerType = (byte) 0;
      structure.bReserved = (byte) 0;
      structure.dwLayerMask = 0;
      structure.dwVisibleMask = 0;
      structure.dwDamageMask = 0;
      this.deviceContext = User.GetDC(this.windowHandle);
      int num = !(this.deviceContext == IntPtr.Zero) ? Gdi.ChoosePixelFormat(this.deviceContext, ref structure) : throw new InvalidOperationException("Can not create a GL device context.");
      if (num == 0)
        throw new InvalidOperationException("Can not find a suitable PixelFormat.");
      if (!Gdi.SetPixelFormat(this.deviceContext, num, ref structure))
        throw new InvalidOperationException("Can not set the chosen PixelFormat.  Chosen PixelFormat was " + (object) num + ".");
      this.renderingContext = Wgl.wglCreateContext(this.deviceContext);
      if (this.renderingContext == IntPtr.Zero)
        throw new InvalidOperationException("Can not create a GL rendering context.");
      this.MakeCurrent();
      Wgl.wglDescribePixelFormat(this.deviceContext, num, Marshal.SizeOf(typeof (Gdi.PIXELFORMATDESCRIPTOR)), ref structure);
      this.isSoftwareRenderer = (structure.dwFlags & 4096) == 0 && (structure.dwFlags & 64) != 0;
      if ((double) OpenGlInfo.Version < 1.2000000476837158)
        this.isSoftwareRenderer = true;
      this.Settings.Validate();
      this.tm = new TextureManager()
      {
        Settings = this.Settings
      };
    }

    public void DestroyContexts()
    {
      if (this.renderingContext != IntPtr.Zero)
      {
        Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        Wgl.wglDeleteContext(this.renderingContext);
        this.renderingContext = IntPtr.Zero;
      }
      if (!(this.deviceContext != IntPtr.Zero))
        return;
      if (this.windowHandle != IntPtr.Zero)
        User.ReleaseDC(this.windowHandle, this.deviceContext);
      this.deviceContext = IntPtr.Zero;
    }

    private static void SetStyle(Control window, ControlStyles styles, bool enable)
    {
      typeof (Control).GetMethod(nameof (SetStyle), BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object) window, new object[2]
      {
        (object) styles,
        (object) enable
      });
    }

    private void PaintFrame()
    {
      if (!this.BeginScene((Graphics) null))
        return;
      this.OnPaint();
      this.EndScene();
    }

    private void SetDefaultBlending()
    {
      if (this.CompositingMode != CompositingMode.SourceOver)
        return;
      Gl.glEnable(3042);
      switch (this.BlendingOperation)
      {
        case BlendingOperation.Multiply:
          Gl.glBlendFuncSeparate(774, 0, 770, 771);
          break;
        default:
          Gl.glBlendFunc(770, 771);
          break;
      }
    }

    private static Matrix GetMatrix(bool modelView)
    {
      float[] @params = new float[16];
      Gl.glGetFloatv(modelView ? 2982 : 2983, @params);
      return new Matrix(@params[0], @params[1], @params[4], @params[5], @params[12], @params[13]);
    }

    private void window_Disposed(object sender, EventArgs e) => this.Dispose();

    private void window_Paint(object sender, PaintEventArgs e) => this.PaintFrame();

    public event EventHandler Paint;

    protected virtual void OnPaint()
    {
      if (this.Paint == null)
        return;
      this.Paint((object) this, EventArgs.Empty);
    }

    protected virtual void OnInitOpenGl()
    {
      Gl.glDisable(2929);
      Gl.glShadeModel(7424);
    }

    public bool BeginScene(Graphics gr)
    {
      if (this.sceneCounter == 0)
      {
        if (this.deviceContext == IntPtr.Zero || this.renderingContext == IntPtr.Zero)
          return false;
        if (this.AutoMakeCurrent)
          this.MakeCurrent();
        if (this.AutoReshape)
          this.ReshapeFunc();
        this.Clear(this.Control.BackColor);
      }
      ++this.sceneCounter;
      return true;
    }

    public void EndScene()
    {
      if (this.sceneCounter == 0)
        return;
      if (this.sceneCounter == 1)
      {
        if (this.AutoFinish)
          Gl.glFinish();
        if (this.AutoSwapBuffers)
          this.SwapBuffers();
        this.errorCode = Gl.glGetError();
      }
      --this.sceneCounter;
    }

    public bool IsLocked => this.sceneCounter > 0;

    public void Clear(Color color)
    {
      Gl.glClearColor((float) color.R / (float) byte.MaxValue, (float) color.G / (float) byte.MaxValue, (float) color.B / (float) byte.MaxValue, (float) color.A / (float) byte.MaxValue);
      Gl.glClear(16384);
    }

    public void DrawImage(
      RendererImage image,
      RectangleF dest,
      RectangleF src,
      BitmapAdjustment ajustment,
      float opacity)
    {
      opacity *= this.Opacity;
      Gl.glPushAttrib(24576);
      this.SetDefaultBlending();
      if (ajustment.IsEmpty)
      {
        this.tm.DrawImage(image, dest, src, opacity);
      }
      else
      {
        using (System.Drawing.Bitmap adjustedBitmap = image.Bitmap.CreateAdjustedBitmap(ajustment, PixelFormat.Format32bppArgb, true))
          this.tm.DrawImage((RendererImage) adjustedBitmap, dest, src, opacity);
      }
      Gl.glPopAttrib();
    }

    public void FillRectangle(RectangleF bounds, Color color)
    {
      color = Color.FromArgb((int) ((double) byte.MaxValue * (double) this.opacity), color);
      if (color.A < (byte) 5)
        return;
      Gl.glPushAttrib(24577);
      this.SetDefaultBlending();
      Gl.glColor4ub(color.R, color.G, color.B, color.A);
      Gl.glRectf(bounds.X, bounds.Y, bounds.Right, bounds.Bottom);
      Gl.glPopAttrib();
    }

    public void DrawLine(IEnumerable<PointF> points, Color color, float width)
    {
      color = Color.FromArgb((int) ((double) byte.MaxValue * (double) this.opacity), color);
      if (color.A < (byte) 5)
        return;
      Gl.glPushAttrib(27425);
      this.SetDefaultBlending();
      Gl.glLineWidth(width);
      Gl.glColor4ub(color.R, color.G, color.B, color.A);
      Gl.glBegin(3);
      foreach (PointF point in points)
        Gl.glVertex2f(point.X, point.Y);
      Gl.glEnd();
      Gl.glPopAttrib();
    }

    public bool IsVisible(RectangleF bounds) => true;

    public IDisposable SaveState()
    {
      Gl.glPushMatrix();
      return (IDisposable) new Disposer(new Action(Gl.glPopMatrix));
    }

    public void TranslateTransform(float dx, float dy) => Gl.glTranslatef(dx, dy, 0.0f);

    public void ScaleTransform(float dx, float dy) => Gl.glScalef(dx, dy, 0.0f);

    public void RotateTransform(float angel) => Gl.glRotatef(angel, 0.0f, 0.0f, 1f);

    public bool HighQuality { get; set; }

    public Matrix Transform
    {
      set
      {
        float[] m = new float[16];
        float[] elements = value.Elements;
        m[0] = elements[0];
        m[4] = elements[2];
        m[8] = 0.0f;
        m[12] = elements[4];
        m[1] = elements[1];
        m[5] = elements[3];
        m[9] = 0.0f;
        m[13] = elements[5];
        m[2] = 0.0f;
        m[6] = 0.0f;
        m[10] = 1f;
        m[14] = 0.0f;
        m[3] = 0.0f;
        m[7] = 0.0f;
        m[11] = 0.0f;
        m[15] = 1f;
        Gl.glLoadMatrixf(m);
      }
      get => ControlOpenGlRenderer.GetMatrix(true);
    }

    public bool IsHardware => true;

    public void DrawBlurredImage(RendererImage image, RectangleF dest, RectangleF src, float blur)
    {
      this.DrawImage(image, dest, src, BitmapAdjustment.Empty, this.opacity);
    }

    public float Opacity
    {
      get => this.opacity;
      set => this.opacity = value;
    }

    public CompositingMode CompositingMode
    {
      get => this.compositingMode;
      set => this.compositingMode = value;
    }

    public RectangleF Clip
    {
      get => this.clip;
      set
      {
        this.clip = value;
        if (this.clip.IsEmpty)
        {
          Gl.glDisable(3089);
        }
        else
        {
          int[] @params = new int[4];
          Gl.glEnable(3089);
          using (Matrix matrix1 = ControlOpenGlRenderer.GetMatrix(true))
          {
            using (Matrix matrix2 = ControlOpenGlRenderer.GetMatrix(false))
            {
              PointF[] pts = new PointF[4]
              {
                new PointF(this.clip.X, this.clip.Y),
                new PointF(this.clip.Right, this.clip.Y),
                new PointF(this.clip.Right, this.clip.Bottom),
                new PointF(this.clip.X, this.clip.Bottom)
              };
              matrix1.TransformPoints(pts);
              matrix2.TransformPoints(pts);
              Gl.glGetIntegerv(2978, @params);
              Point point1 = new Point((int) ((double) @params[0] + (1.0 + (double) pts[3].X) * (double) @params[2] / 2.0), (int) ((double) @params[1] + (1.0 + (double) pts[3].Y) * (double) @params[3] / 2.0));
              Point point2 = new Point((int) ((double) @params[0] + (1.0 + (double) pts[1].X) * (double) @params[2] / 2.0), (int) ((double) @params[1] + (1.0 + (double) pts[1].Y) * (double) @params[3] / 2.0));
              Gl.glScissor(point1.X, point1.Y, point2.X - point1.X, point2.Y - point1.Y);
            }
          }
        }
      }
    }

    public unsafe System.Drawing.Bitmap GetFramebuffer(Rectangle rc, bool flip)
    {
      System.Drawing.Bitmap framebuffer = new System.Drawing.Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
      BitmapData bitmapdata = framebuffer.LockBits(new Rectangle(0, 0, framebuffer.Width, framebuffer.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
      try
      {
        Gl.glReadPixels(rc.X, rc.Y, rc.Right, rc.Bottom, 32993, 5121, bitmapdata.Scan0);
        if (flip)
        {
          IntPtr scan0 = bitmapdata.Scan0;
          int num1 = bitmapdata.Stride / 4;
          int num2 = framebuffer.Height / 2;
          uint* numPtr1 = (uint*) (void*) scan0;
          uint* numPtr2 = numPtr1 + num1 * (rc.Height - 1);
          for (int index1 = 0; index1 < num2; ++index1)
          {
            for (int index2 = 0; index2 < num1; ++index2)
            {
              uint num3 = numPtr1[index2];
              numPtr1[index2] = numPtr2[index2];
              numPtr2[index2] = num3;
            }
            numPtr1 += num1;
            numPtr2 -= num1;
          }
        }
      }
      finally
      {
        framebuffer.UnlockBits(bitmapdata);
      }
      return framebuffer;
    }

    public bool IsSoftwareRenderer => this.isSoftwareRenderer;

    public void ClearStencil()
    {
      Gl.glClearStencil(0);
      Gl.glClear(1024);
    }

    public StencilMode StencilMode
    {
      get => this.stencilMode;
      set
      {
        switch (value)
        {
          case StencilMode.WriteOne:
            Gl.glEnable(2960);
            Gl.glStencilFunc(519, 1, 1);
            Gl.glStencilOp(7681, 7681, 7681);
            break;
          case StencilMode.WriteNull:
            Gl.glEnable(2960);
            Gl.glStencilFunc(519, 0, 0);
            Gl.glStencilOp(7681, 7681, 7681);
            break;
          case StencilMode.TestOne:
            Gl.glEnable(2960);
            Gl.glStencilFunc(514, 1, 1);
            Gl.glStencilOp(7680, 7680, 7680);
            break;
          case StencilMode.TestNull:
            Gl.glEnable(2960);
            Gl.glStencilFunc(514, 0, 1);
            Gl.glStencilOp(7680, 7680, 7680);
            break;
          default:
            Gl.glDisable(2960);
            break;
        }
        this.stencilMode = value;
      }
    }

    public bool OptimizedTextures
    {
      get => this.tm.IsOptimizedTexture;
      set => this.tm.IsOptimizedTexture = value;
    }

    public bool EnableFilter
    {
      get => this.tm.EnableFilter;
      set => this.tm.EnableFilter = value;
    }

    public BlendingOperation BlendingOperation { get; set; }

    public static void SetProcessMemorySize(int size)
    {
      Kernel.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, 0, size);
    }
  }
}

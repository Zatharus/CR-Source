// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.BitmapGdiRenderer
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

#nullable disable
namespace cYo.Common.Presentation
{
  public class BitmapGdiRenderer : IBitmapRenderer
  {
    private readonly bool fixedGraphics;
    private Stack<Graphics> graphicsStack;
    private float opacity = 1f;

    public BitmapGdiRenderer(Graphics graphics, bool highQuality = false)
    {
      this.LowQualityInterpolation = InterpolationMode.Default;
      this.Graphics = graphics;
      this.HighQuality = highQuality;
      this.fixedGraphics = graphics != null;
    }

    public BitmapGdiRenderer()
      : this((Graphics) null)
    {
    }

    public Graphics Graphics { get; set; }

    public InterpolationMode LowQualityInterpolation { get; set; }

    public void Clear(Color color) => this.Graphics.Clear(color);

    public void DrawImage(
      RendererImage image,
      RectangleF dest,
      RectangleF src,
      BitmapAdjustment transform,
      float opacity)
    {
      Rectangle rectangle = Rectangle.Truncate(src);
      Rectangle destination = Rectangle.Truncate(dest);
      opacity *= this.opacity;
      using (this.Graphics.HighQuality(this.HighQuality, dest.Size, src.Size))
      {
        if (!this.HighQuality)
          this.Graphics.InterpolationMode = this.LowQualityInterpolation;
        this.Graphics.DrawImage((Bitmap) image, destination, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, transform, opacity);
      }
    }

    public void FillRectangle(RectangleF bounds, Color color)
    {
      using (Brush brush = (Brush) new SolidBrush(Color.FromArgb((int) ((double) byte.MaxValue * (double) this.opacity), color)))
        this.Graphics.FillRectangle(brush, bounds);
    }

    public void DrawLine(IEnumerable<PointF> points, Color color, float width)
    {
      using (Pen pen = new Pen(color, width))
      {
        bool flag = true;
        PointF pt2 = PointF.Empty;
        foreach (PointF point in points)
        {
          if (flag)
          {
            pt2 = point;
            flag = false;
          }
          else
          {
            PointF pt1 = pt2;
            pt2 = point;
            this.Graphics.DrawLine(pen, pt1, pt2);
          }
        }
      }
    }

    public bool IsVisible(RectangleF bounds) => this.Graphics.IsVisible(bounds);

    public IDisposable SaveState() => this.Graphics.SaveState();

    public void TranslateTransform(float dx, float dy) => this.Graphics.TranslateTransform(dx, dy);

    public void ScaleTransform(float dx, float dy) => this.Graphics.ScaleTransform(dx, dy);

    public void RotateTransform(float angel) => this.Graphics.RotateTransform(angel);

    public bool HighQuality { get; set; }

    public Matrix Transform
    {
      set => this.Graphics.Transform = value;
      get => this.Graphics.Transform;
    }

    public bool IsHardware => false;

    public bool BeginScene(Graphics gr)
    {
      if (this.fixedGraphics)
        return false;
      if (this.graphicsStack == null)
        this.graphicsStack = new Stack<Graphics>();
      this.graphicsStack.Push(this.Graphics);
      this.Graphics = gr;
      return true;
    }

    public void EndScene()
    {
      if (this.fixedGraphics)
        return;
      this.Graphics = this.graphicsStack != null ? this.graphicsStack.Pop() : (Graphics) null;
    }

    public bool IsLocked => false;

    public void DrawBlurredImage(RendererImage image, RectangleF rd, RectangleF rs, float blur)
    {
      try
      {
        RectangleF src = new RectangleF(rs.X, rs.Y, rs.Width * blur, rs.Height * blur);
        RectangleF destRect = new RectangleF(PointF.Empty, src.Size);
        if ((double) destRect.Width < 8.0 || (double) destRect.Height < 8.0)
          return;
        using (this.Graphics.SaveState())
        {
          using (Bitmap image1 = new Bitmap((int) destRect.Width, (int) destRect.Height))
          {
            using (Graphics graphics = Graphics.FromImage((Image) image1))
            {
              graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
              graphics.DrawImage((Image) (Bitmap) image, destRect, rs, GraphicsUnit.Pixel);
            }
            this.DrawImage((RendererImage) image1, rd, src, BitmapAdjustment.Empty, this.opacity);
          }
        }
      }
      catch (Exception ex)
      {
        throw new ArgumentException("Failed to draw blurred image: " + (object) rs + "/" + (object) blur);
      }
    }

    public float Opacity
    {
      get => this.opacity;
      set => this.opacity = value;
    }

    public CompositingMode CompositingMode
    {
      get => this.Graphics.CompositingMode;
      set => this.Graphics.CompositingMode = value;
    }

    public RectangleF Clip
    {
      get => this.Graphics.ClipBounds;
      set => this.Graphics.SetClip(value);
    }

    public static implicit operator BitmapGdiRenderer(Graphics gr) => new BitmapGdiRenderer(gr);
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing3D.BitmapFrameBuffer
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using System.Drawing;

#nullable disable
namespace cYo.Common.Drawing3D
{
  public class BitmapFrameBuffer : DisposableObject, IFrameBuffer, ITexture
  {
    private readonly FastBitmap fastBitmap;
    private readonly Rectangle clip;
    private readonly Rectangle bounds;

    public BitmapFrameBuffer(Bitmap bitmap, Rectangle clip)
    {
      this.bounds = bitmap.Size.ToRectangle();
      if (clip.IsEmpty)
      {
        this.clip = this.bounds;
      }
      else
      {
        clip.Intersect(this.bounds);
        this.clip = clip;
      }
      this.fastBitmap = new FastBitmap(bitmap);
    }

    public BitmapFrameBuffer(Bitmap bitmap)
      : this(bitmap, Rectangle.Empty)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (this.fastBitmap != null)
        this.fastBitmap.Dispose();
      base.Dispose(disposing);
    }

    public Size Size => this.clip.Size;

    public Color GetColor(int x, int y)
    {
      x += this.clip.X;
      y += this.clip.Y;
      return x < this.bounds.Width && y < this.bounds.Height && x >= 0 && y >= 0 ? this.fastBitmap.GetPixel(x, y) : Color.Empty;
    }

    public void SetColor(int x, int y, Color color)
    {
      x += this.clip.X;
      y += this.clip.Y;
      if (x >= this.bounds.Width || y >= this.bounds.Height || x < 0 || y < 0)
        return;
      this.fastBitmap.SetPixel(x, y, color);
    }
  }
}

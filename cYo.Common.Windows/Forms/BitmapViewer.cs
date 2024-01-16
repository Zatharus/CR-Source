// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.BitmapViewer
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class BitmapViewer : ScrollControl, IBitmapDisplayControl, IDisposable
  {
    private volatile Bitmap image;
    private volatile bool pendingImageChange;
    private ScaleMode scaleMode = ScaleMode.FitAll;
    private BitmapAdjustment colorAdjustment = BitmapAdjustment.Empty;
    private ContentAlignment textAlignment = ContentAlignment.TopCenter;

    public BitmapViewer()
    {
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
    }

    [DefaultValue(null)]
    public Bitmap Bitmap
    {
      get => this.image;
      set
      {
        if ((Image) this.image == value)
          return;
        this.image = value;
        this.OnImageChanged();
        this.pendingImageChange = true;
        this.Invalidate();
      }
    }

    [DefaultValue(ScaleMode.FitAll)]
    public ScaleMode ScaleMode
    {
      get => this.scaleMode;
      set
      {
        if (this.scaleMode == value)
          return;
        this.scaleMode = value;
        this.OnScaleModeChanged();
        this.pendingImageChange = true;
        this.Invalidate();
      }
    }

    [DefaultValue(typeof (BitmapAdjustment), "0, 0, 0")]
    public BitmapAdjustment ColorAdjustment
    {
      get => this.colorAdjustment;
      set
      {
        if (this.colorAdjustment == value)
          return;
        this.colorAdjustment = value;
        this.Invalidate();
      }
    }

    [DefaultValue(ContentAlignment.TopCenter)]
    public ContentAlignment TextAlignment
    {
      get => this.textAlignment;
      set
      {
        if (this.textAlignment == value)
          return;
        this.textAlignment = value;
        this.Invalidate();
      }
    }

    public void SetBitmap(Bitmap image)
    {
      Bitmap bitmap = this.Bitmap;
      this.Bitmap = image;
      bitmap?.Dispose();
    }

    public Color GetPixel(System.Drawing.Point location)
    {
      if (this.image == null)
        throw new InvalidOperationException("No valid image for this method");
      try
      {
        System.Drawing.Point point = this.Translate(location, true);
        Rectangle imageDisplayBounds = this.GetImageDisplayBounds();
        return this.image.GetPixel(point.X * this.image.Width / imageDisplayBounds.Width, point.Y * this.image.Height / imageDisplayBounds.Height);
      }
      catch
      {
        throw new ArgumentOutOfRangeException(nameof (location), "Location is not a valid position in the image");
      }
    }

    public event EventHandler ImageChanged;

    public event EventHandler ScaleModeChanged;

    protected virtual void OnImageChanged()
    {
      if (this.ImageChanged == null)
        return;
      this.ImageChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnScaleModeChanged()
    {
      if (this.ScaleModeChanged == null)
        return;
      this.ScaleModeChanged((object) this, EventArgs.Empty);
    }

    private System.Drawing.Size GetImageSize()
    {
      Image image = (Image) this.image;
      if (image == null)
        return System.Drawing.Size.Empty;
      float scale = image.Size.GetScale(this.ViewRectangle.Size, this.scaleMode);
      return new System.Drawing.Size((int) ((double) image.Width * (double) scale), (int) ((double) image.Height * (double) scale));
    }

    private System.Drawing.Point GetImageLeftTop()
    {
      System.Drawing.Point scrollPosition = this.ScrollPosition;
      int x = -scrollPosition.X + this.DisplayRectangle.X;
      scrollPosition = this.ScrollPosition;
      int y = -scrollPosition.Y + this.DisplayRectangle.Y;
      return new System.Drawing.Point(x, y);
    }

    private Rectangle GetImageDisplayBounds()
    {
      return this.image == null ? Rectangle.Empty : new Rectangle(this.GetImageLeftTop(), this.VirtualSize);
    }

    private void UpdateVirtualSize()
    {
      for (int index = 0; index < 2; ++index)
        this.VirtualSize = this.GetImageSize();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      base.OnPaintBackground(e);
      if (this.pendingImageChange)
      {
        this.pendingImageChange = false;
        this.UpdateVirtualSize();
      }
      if (this.image == null)
        return;
      using (e.Graphics.SaveState())
      {
        e.Graphics.IntersectClip(this.DisplayRectangle);
        e.Graphics.DrawImage(this.image, this.GetImageDisplayBounds(), 0, 0, this.image.Width, this.image.Height, this.colorAdjustment);
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Rectangle displayRectangle = this.DisplayRectangle;
      displayRectangle.Inflate(-4, -4);
      using (SolidBrush solidBrush = new SolidBrush(this.ForeColor))
      {
        using (StringFormat format = new StringFormat()
        {
          LineAlignment = this.TextAlignment.ToLineAlignment(),
          Alignment = this.TextAlignment.ToAlignment()
        })
          e.Graphics.DrawString(this.Text, this.Font, (Brush) solidBrush, (RectangleF) displayRectangle, format);
      }
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (!this.IsHandleCreated)
        return;
      this.UpdateVirtualSize();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      base.OnMouseWheel(e);
      System.Drawing.Point scrollPosition = this.ScrollPosition;
      scrollPosition.Y -= e.Delta;
      this.ScrollPosition = scrollPosition;
    }

    [SpecialName]
    object IBitmapDisplayControl.get_Tag() => this.Tag;

    [SpecialName]
    void IBitmapDisplayControl.set_Tag(object value) => this.Tag = value;
  }
}

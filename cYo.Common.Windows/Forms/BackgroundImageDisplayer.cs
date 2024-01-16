// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.BackgroundImageDisplayer
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using System;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class BackgroundImageDisplayer : DisposableObject, IBitmapDisplayControl, IDisposable
  {
    private readonly Control control;
    private Bitmap image;

    public BackgroundImageDisplayer(Control c)
    {
      this.Opacity = 1f;
      this.AutoDispose = true;
      this.Alignment = ContentAlignment.MiddleCenter;
      this.control = c;
    }

    public ContentAlignment Alignment { get; set; }

    public bool AutoDispose { get; set; }

    public Control Control => this.control;

    public float Opacity { get; set; }

    protected virtual void OnSetImage(Bitmap image)
    {
      if (this.control.InvokeIfRequired((Action) (() => this.OnSetImage(image))))
        return;
      Bitmap bitmap1;
      if (image != null)
      {
        Rectangle clientRectangle = this.control.ClientRectangle;
        int width = clientRectangle.Width;
        clientRectangle = this.control.ClientRectangle;
        int height = clientRectangle.Height;
        bitmap1 = new Bitmap(width, height);
      }
      else
        bitmap1 = (Bitmap) null;
      Bitmap bitmap2 = bitmap1;
      if (bitmap2 != null)
      {
        using (Graphics graphics = Graphics.FromImage((Image) bitmap2))
        {
          graphics.Clear(this.control.BackColor);
          Rectangle bounds = new Rectangle(0, 0, image.Width, image.Height).Align(this.control.ClientRectangle, this.Alignment);
          graphics.DrawImage((Image) image, bounds, this.Opacity);
        }
        if (this.control.BackgroundImage != null)
          this.control.BackgroundImage.Dispose();
      }
      this.control.BackgroundImage = (Image) bitmap2;
    }

    public Bitmap Bitmap
    {
      get => this.image;
      set
      {
        this.SetBitmap(value);
        if (this.image != null && this.AutoDispose)
          this.image.Dispose();
        this.image = value;
      }
    }

    public object Tag
    {
      get => this.control.Tag;
      set => this.control.Tag = value;
    }

    public void SetBitmap(Bitmap image) => this.OnSetImage(image);

    protected override void Dispose(bool disposing)
    {
      if (this.control.BackgroundImage != null)
        this.control.BackgroundImage.Dispose();
      this.control.Dispose();
    }
  }
}

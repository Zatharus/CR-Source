// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.SimpleButtonPanel
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class SimpleButtonPanel : OverlayPanel
  {
    private ScalableBitmap background;
    private ScalableBitmap icon;
    private float hilightBrightness = 0.4f;

    public SimpleButtonPanel(Size size)
      : base(size)
    {
    }

    public ScalableBitmap Background
    {
      get => this.background;
      set
      {
        if (this.background == value)
          return;
        this.background = value;
        this.Invalidate();
      }
    }

    public ScalableBitmap Icon
    {
      get => this.icon;
      set
      {
        if (this.icon == value)
          return;
        this.icon = value;
        this.Invalidate();
      }
    }

    public float HilightBrightness
    {
      get => this.hilightBrightness;
      set
      {
        if ((double) this.hilightBrightness == (double) value)
          return;
        this.hilightBrightness = value;
        this.Invalidate();
      }
    }

    protected override void OnPanelStateChanged()
    {
      base.OnPanelStateChanged();
      this.Invalidate();
    }

    protected override void OnMarginChanged()
    {
      base.OnMarginChanged();
      this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics graphics = e.Graphics;
      IBitmapRenderer gr = (IBitmapRenderer) new BitmapGdiRenderer(graphics);
      Rectangle clientRectangle = this.ClientRectangle;
      BitmapAdjustment itf = this.PanelState == PanelState.Selected ? new BitmapAdjustment(0.0f, this.hilightBrightness) : BitmapAdjustment.Empty;
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      if (this.background != null)
        this.background.Draw(gr, (RectangleF) clientRectangle, itf, 1f);
      if (this.icon != null)
      {
        Rectangle rectangle = this.icon.Bitmap.Size.ToRectangle(clientRectangle.Pad(this.Margin), RectangleScaleMode.Center | RectangleScaleMode.OnlyShrink);
        this.icon.Draw(gr, (RectangleF) rectangle, itf, 1f);
      }
      this.Opacity = this.PanelState == PanelState.Normal ? 0.9f : 1f;
    }
  }
}

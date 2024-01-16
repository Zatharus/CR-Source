// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.RatingRenderer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  public class RatingRenderer
  {
    private Rectangle bounds;

    public RatingRenderer(Image image, Rectangle bounds, int count = 5, bool vertical = false)
    {
      this.RatingImage = image;
      this.MaximumRating = count;
      this.VerticalAlignment = vertical;
      this.Bounds = bounds;
      this.RatingTextColor = Color.DimGray;
      this.RatingScaleMode = RectangleScaleMode.Center;
    }

    public bool Fast { get; set; }

    public Image RatingImage { get; set; }

    public Color RatingTextColor { get; set; }

    public int MaximumRating { get; set; }

    public Rectangle Bounds
    {
      get => this.bounds;
      set => this.bounds = value;
    }

    public bool VerticalAlignment { get; set; }

    public RectangleScaleMode RatingScaleMode { get; set; }

    public int X
    {
      get => this.bounds.X;
      set => this.bounds.X = value;
    }

    public int Y
    {
      get => this.bounds.Y;
      set => this.bounds.Y = value;
    }

    public int Height
    {
      get => this.bounds.Height;
      set => this.bounds.Height = value;
    }

    public int Width
    {
      get => this.bounds.Width;
      set => this.bounds.Width = value;
    }

    public RectangleF DrawRatingStrip(Graphics gr, float rating, float alpha1 = 1f, float alpha2 = 0.25f)
    {
      RectangleF stripDisplayBounds = this.GetStripDisplayBounds();
      RectangleF rect = stripDisplayBounds;
      if (this.VerticalAlignment)
      {
        stripDisplayBounds.Height *= rating / (float) this.MaximumRating;
        rect.Height -= stripDisplayBounds.Height;
        rect.Y += stripDisplayBounds.Height;
      }
      else
      {
        stripDisplayBounds.Width *= rating / (float) this.MaximumRating;
        rect.Width -= stripDisplayBounds.Width;
        rect.X += stripDisplayBounds.Width;
      }
      using (gr.Fast(this.Fast))
      {
        if (!stripDisplayBounds.IsEmpty && (double) alpha1 > 0.05000000074505806)
        {
          using (gr.SaveState())
          {
            gr.SetClip(stripDisplayBounds, CombineMode.Intersect);
            this.DrawRatingStrip(gr, alpha1);
          }
        }
        if (!rect.IsEmpty)
        {
          if ((double) alpha2 > 0.05000000074505806)
          {
            using (gr.SaveState())
            {
              gr.SetClip(rect, CombineMode.Intersect);
              this.DrawRatingStrip(gr, alpha2);
            }
          }
        }
      }
      return stripDisplayBounds;
    }

    public float GetRatingFromStrip(Point pt)
    {
      RectangleF stripDisplayBounds = this.GetStripDisplayBounds();
      if (this.VerticalAlignment)
      {
        if ((double) pt.Y < (double) stripDisplayBounds.Y)
          return 0.0f;
        return (double) pt.Y > (double) stripDisplayBounds.Bottom ? (float) this.MaximumRating : ((float) pt.Y - stripDisplayBounds.Y) / stripDisplayBounds.Height * (float) this.MaximumRating;
      }
      if ((double) pt.X < (double) stripDisplayBounds.X)
        return 0.0f;
      return (double) pt.X > (double) stripDisplayBounds.Right ? (float) this.MaximumRating : ((float) pt.X - stripDisplayBounds.X) / stripDisplayBounds.Width * (float) this.MaximumRating;
    }

    public RectangleF DrawRatingTag(Graphics gr, float rating, int ratingDigits = 1)
    {
      if (this.RatingImage == null || this.MaximumRating <= 0)
        return RectangleF.Empty;
      Rectangle rectangle1 = this.RatingImage.Size.ToRectangle(this.Bounds);
      using (gr.HighQuality(true))
        gr.DrawImage(this.RatingImage, rectangle1);
      Color ratingTextColor = this.RatingTextColor;
      Font font = FC.Get("Arial", 12f, FontStyle.Bold | FontStyle.Italic);
      string str = rating.ToString("N" + (object) ratingDigits);
      SizeF sizeF = gr.MeasureString(str, font);
      Rectangle rectangle2 = rectangle1.Pad(2);
      float num = (float) ((double) rectangle2.Width / (double) sizeF.Width * 0.89999997615814209);
      using (gr.SaveState())
      {
        using (SolidBrush solidBrush = new SolidBrush(ratingTextColor))
        {
          gr.ScaleTransform(num, num);
          rectangle2 = rectangle2.Scale(1f / num);
          using (StringFormat format = new StringFormat()
          {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Far
          })
            gr.DrawString(str, font, (Brush) solidBrush, (RectangleF) rectangle2, format);
        }
      }
      return (RectangleF) rectangle1;
    }

    public SizeF GetRenderSize() => this.GetStripDisplayBounds().Size;

    private RectangleF GetStripDisplayBounds()
    {
      return this.RatingImage == null || this.MaximumRating <= 0 ? (RectangleF) Rectangle.Empty : (!this.VerticalAlignment ? (SizeF) new Size(this.MaximumRating * this.RatingImage.Width, this.RatingImage.Height) : (SizeF) new Size(this.RatingImage.Width, this.MaximumRating * this.RatingImage.Height)).ToRectangle((RectangleF) this.Bounds, this.RatingScaleMode);
    }

    private void DrawRatingStrip(Graphics gr, float alpha)
    {
      RectangleF stripDisplayBounds = this.GetStripDisplayBounds();
      float scale;
      PointF pointF;
      if (this.VerticalAlignment)
      {
        scale = stripDisplayBounds.Width / (float) this.RatingImage.Width;
        pointF = new PointF(0.0f, (float) this.RatingImage.Height * scale);
      }
      else
      {
        scale = stripDisplayBounds.Height / (float) this.RatingImage.Height;
        pointF = new PointF((float) this.RatingImage.Width * scale, 0.0f);
      }
      RectangleF rectangleF = new RectangleF(stripDisplayBounds.Location, (SizeF) this.RatingImage.Size.Scale(scale));
      for (int index = 0; index < this.MaximumRating; ++index)
      {
        gr.DrawImage(this.RatingImage, Rectangle.Round(rectangleF), alpha);
        rectangleF.X += pointF.X;
        rectangleF.Y += pointF.Y;
      }
    }
  }
}

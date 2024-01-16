// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ThumbIconRenderer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  public class ThumbIconRenderer : ViewItemRenderer
  {
    public ThumbIconRenderer(Image image, ThumbnailDrawingOptions flags)
    {
      this.TextHeight = 45;
      this.Image = image;
      this.Options = flags;
    }

    public int TextHeight { get; set; }

    public Rectangle Draw(Graphics graphics, Rectangle bounds)
    {
      Rectangle rectangle1 = bounds;
      ref Rectangle local = ref rectangle1;
      System.Drawing.Size border = this.Border;
      int width = -border.Width;
      border = this.Border;
      int height = -border.Height;
      local.Inflate(width, height);
      Rectangle rectangle2 = rectangle1;
      rectangle2.Height -= this.TextHeight;
      System.Drawing.Point location = new System.Drawing.Point(rectangle2.Left, rectangle2.Top);
      Rectangle a = this.DrawThumbnail(graphics, new Rectangle(location, rectangle2.Size));
      if (this.TextLines.Count == 0)
        return a;
      Rectangle rect1 = new Rectangle(rectangle1.X, rectangle2.Bottom, rectangle1.Width, this.TextHeight);
      Rectangle rect2 = rect1;
      rect2.Inflate(-2, -2);
      if (this.Selected || this.Hot || this.Focused)
      {
        Rectangle rc = SimpleTextRenderer.MeasureText(graphics, (IEnumerable<TextLine>) this.TextLines, rect2);
        rc.Inflate(2, 2);
        rc.Intersect(rect1);
        graphics.DrawStyledRectangle(rc, this.SelectionAlphaState, this.SelectionBackColor);
      }
      using (graphics.Fast())
        return Rectangle.Union(a, SimpleTextRenderer.DrawText(graphics, (IEnumerable<TextLine>) this.TextLines, rect2));
    }
  }
}

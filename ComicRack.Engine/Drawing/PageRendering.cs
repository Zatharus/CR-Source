// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.PageRendering
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  public static class PageRendering
  {
    public static Bitmap CreatePageBow(Size size, float angle)
    {
      Bitmap pageBow = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
      using (Graphics graphics = Graphics.FromImage((Image) pageBow))
      {
        Color color1 = Color.FromArgb(EngineConfiguration.Default.PageBowFromAlpha, EngineConfiguration.Default.PageBowColor);
        Color color2 = Color.FromArgb(EngineConfiguration.Default.PageBowToAlpha, EngineConfiguration.Default.PageBowColor);
        Rectangle rect = new Rectangle(0, 0, size.Width, size.Height);
        using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, color1, color2, angle))
        {
          linearGradientBrush.SetSigmaBellShape(0.0f, 1f);
          graphics.FillRectangle((Brush) linearGradientBrush, rect);
        }
      }
      return pageBow;
    }
  }
}

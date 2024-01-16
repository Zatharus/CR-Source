// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.Forms.PanelRenderer
// Assembly: ComicRack.Engine.Display.Forms, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: D83BAE4E-CA55-445A-AD1D-2DF78C341143
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.Display.Forms.dll

using cYo.Common.Presentation;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.Display.Forms.Properties;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display.Forms
{
  public static class PanelRenderer
  {
    private static readonly PanelRenderer.PanelInfo[] panels = new PanelRenderer.PanelInfo[2]
    {
      new PanelRenderer.PanelInfo(Resources.BlackGlassPanel, new Padding(5, 5, 10, 10), new RectangleF(5f, 5f, 215f, 125f), Color.White),
      new PanelRenderer.PanelInfo(Resources.BlueGlassPanel, new Padding(5, 5, 10, 10), new RectangleF(5f, 5f, 215f, 125f), Color.White)
    };

    public static RectangleF Draw(
      IBitmapRenderer gr,
      RectangleF dest,
      float opacity,
      PanelType pt = PanelType.BlackGlass)
    {
      PanelRenderer.PanelInfo panel = PanelRenderer.panels[(int) pt];
      using (ItemMonitor.Lock((object) panel.Bitmap))
        return ScalableBitmap.Draw(gr, panel.Bitmap, dest, panel.Source, panel.Margin, opacity);
    }

    public static RectangleF DrawGraphics(
      Graphics gr,
      RectangleF dest,
      float opacity,
      PanelType pt = PanelType.BlackGlass)
    {
      return PanelRenderer.Draw((IBitmapRenderer) new BitmapGdiRenderer(gr), dest, opacity, pt);
    }

    public static Padding GetMargin(RectangleF dest, PanelType pt = PanelType.BlackGlass)
    {
      RectangleF rectangleF = PanelRenderer.Draw((IBitmapRenderer) null, dest, 1f, pt);
      return new Padding((int) ((double) rectangleF.Left - (double) dest.Left), (int) ((double) rectangleF.Top - (double) dest.Top), (int) ((double) dest.Right - (double) rectangleF.Right), (int) ((double) dest.Bottom - (double) rectangleF.Bottom));
    }

    public static Color GetForeColor(PanelType pt = PanelType.BlackGlass)
    {
      return PanelRenderer.panels[(int) pt].ForeColor;
    }

    private struct PanelInfo
    {
      public readonly Bitmap Bitmap;
      public readonly Padding Margin;
      public readonly RectangleF Source;
      public readonly Color ForeColor;

      public PanelInfo(Bitmap bmp, Padding margin, RectangleF src, Color foreColor)
      {
        this.Bitmap = bmp;
        this.Margin = margin;
        this.Source = src;
        this.ForeColor = foreColor;
      }
    }
  }
}

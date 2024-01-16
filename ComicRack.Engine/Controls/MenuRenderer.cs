// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Controls.MenuRenderer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Controls
{
  public class MenuRenderer : ToolStripProfessionalRenderer
  {
    private Image starImage;

    public MenuRenderer(Image starImage, ProfessionalColorTable colorTable)
      : base(colorTable)
    {
      this.starImage = starImage;
    }

    public MenuRenderer(Image starImage)
      : this(starImage, MenuRenderer.GetManagerColors())
    {
      this.starImage = starImage;
    }

    public static ProfessionalColorTable GetManagerColors()
    {
      return ToolStripManager.Renderer is ToolStripProfessionalRenderer renderer ? renderer.ColorTable : (ProfessionalColorTable) null;
    }

    public Image StarImage
    {
      get => this.starImage;
      set => this.starImage = value;
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
      if (this.starImage == null)
        return;
      string text = e.Text;
      Graphics graphics = e.Graphics;
      if (!text.StartsWith("*"))
      {
        base.OnRenderItemText(e);
      }
      else
      {
        float num = (float) text.Count<char>((Func<char, bool>) (c => c == '*'));
        Rectangle textRectangle = e.TextRectangle;
        textRectangle.Inflate(-2, -2);
        int x = textRectangle.X;
        int y = textRectangle.Y;
        int height = textRectangle.Height;
        int width = height * this.starImage.Width / this.starImage.Height;
        using (graphics.SaveState())
        {
          graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
          for (int index = 0; (double) index < (double) num; ++index)
            graphics.DrawImage(this.starImage, x + width * index, y, width, height);
        }
      }
    }
  }
}

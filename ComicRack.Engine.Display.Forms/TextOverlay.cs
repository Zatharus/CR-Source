// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.Forms.TextOverlay
// Assembly: ComicRack.Engine.Display.Forms, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: D83BAE4E-CA55-445A-AD1D-2DF78C341143
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.Display.Forms.dll

using cYo.Common.Drawing;
using cYo.Common.Presentation.Ceco;
using cYo.Common.Presentation.Panels;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display.Forms
{
  public class TextOverlay : OverlayPanel
  {
    private Bitmap icon;
    private string text;
    private Size maxSize = new Size(400, 400);
    private Size minSize;
    private Font font;
    private static Bitmap hbm = new Bitmap(16, 16);

    public TextOverlay(int width, int height, ContentAlignment align, Font font)
      : base(width, height, align)
    {
      this.font = font;
      this.minSize = new Size(width, height);
    }

    public Bitmap Icon
    {
      get => this.icon;
      set
      {
        if (this.icon == value)
          return;
        this.icon = value;
        this.Resize();
      }
    }

    public string Text
    {
      get => this.text;
      set
      {
        if (this.text == value)
          return;
        this.text = value;
        this.Resize();
      }
    }

    public Size MaxSize
    {
      get => this.maxSize;
      set
      {
        this.maxSize = value;
        if (this.maxSize.Width >= this.Width && this.maxSize.Height >= this.Height)
          return;
        this.Resize();
      }
    }

    public Size MinSize
    {
      get => this.minSize;
      set
      {
        this.minSize = value;
        if (this.minSize.Width <= this.Width && this.minSize.Height <= this.Height)
          return;
        this.Resize();
      }
    }

    public Font Font
    {
      get => this.font;
      set
      {
        if (this.font == value)
          return;
        this.font = value;
        this.Resize();
      }
    }

    public bool Html { get; set; }

    private void Resize()
    {
      Padding margin = PanelRenderer.GetMargin((RectangleF) this.ClientRectangle);
      Size size1 = new Size();
      int num = 0;
      if (this.icon != null)
      {
        size1 = this.icon.Size;
        num = margin.Horizontal;
      }
      Size size2;
      using (Graphics graphics = Graphics.FromImage((Image) TextOverlay.hbm))
        size2 = !this.Html ? graphics.MeasureString(this.text, this.font, this.MaxSize.Width - size1.Width - 2 * num).ToSize() : XHtmlRenderer.MeasureString(graphics, this.text, this.font, this.MaxSize.Width - size1.Width - 2 * num);
      Size size3 = new Size(size1.Width + num + size2.Width + margin.Horizontal, Math.Max(size1.Height, size2.Height) + margin.Vertical);
      size3.Width = Math.Max(this.minSize.Width, size3.Width);
      size3.Height = Math.Max(this.minSize.Height, size3.Height);
      this.Size = size3;
      this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics graphics = e.Graphics;
      Padding margin = PanelRenderer.GetMargin((RectangleF) this.ClientRectangle);
      Rectangle clientRectangle = this.ClientRectangle;
      using (StringFormat stringFormat = new StringFormat()
      {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center
      })
      {
        Rectangle rectangle = Rectangle.Round(PanelRenderer.DrawGraphics(graphics, (RectangleF) clientRectangle, 1f));
        if (this.icon != null)
        {
          graphics.DrawImage((Image) this.icon, rectangle.Location);
          rectangle = rectangle.Pad(this.icon.Width + margin.Horizontal, 0);
        }
        using (graphics.TextRendering(TextRenderingHint.AntiAliasGridFit))
        {
          if (this.Html)
          {
            XHtmlRenderer.DrawString(graphics, this.text, this.font, PanelRenderer.GetForeColor(), rectangle, stringFormat);
          }
          else
          {
            using (SolidBrush solidBrush = new SolidBrush(PanelRenderer.GetForeColor()))
              graphics.DrawString(this.text, this.font, (Brush) solidBrush, (RectangleF) rectangle, stringFormat);
          }
        }
      }
    }
  }
}

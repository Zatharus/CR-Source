// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.LabelPanel
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Drawing;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class LabelPanel : OverlayPanel
  {
    private string text;
    private ContentAlignment textAlignment = ContentAlignment.MiddleLeft;
    private Color textColor = Color.White;
    private string textFont = "Sans Serif";
    private float textSize = 8f;

    public LabelPanel()
      : base(100, 12)
    {
    }

    public string Text
    {
      get => this.text;
      set
      {
        if (this.text == value)
          return;
        this.text = value;
        this.Invalidate();
      }
    }

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

    public Color TextColor
    {
      get => this.textColor;
      set
      {
        if (this.textColor == value)
          return;
        this.textColor = value;
        this.Invalidate();
      }
    }

    public string TextFont
    {
      get => this.textFont;
      set
      {
        if (this.textFont == value)
          return;
        this.textFont = value;
        this.Invalidate();
      }
    }

    public float TextSize
    {
      get => this.textSize;
      set
      {
        if ((double) this.textSize == (double) value)
          return;
        this.textSize = value;
        this.Invalidate();
      }
    }

    protected override void OnSizeChanged()
    {
      base.OnSizeChanged();
      this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics graphics = e.Graphics;
      Rectangle clientRectangle = this.ClientRectangle;
      graphics.Clear(Color.Transparent);
      Font font = FC.Get(this.textFont, this.textSize);
      using (graphics.TextRendering(TextRenderingHint.AntiAliasGridFit))
      {
        using (Brush brush = (Brush) new SolidBrush(this.textColor))
        {
          using (StringFormat format = new StringFormat()
          {
            Alignment = this.textAlignment.ToAlignment(),
            LineAlignment = this.textAlignment.ToLineAlignment()
          })
            graphics.DrawString(this.text, font, brush, (RectangleF) clientRectangle, format);
        }
      }
    }
  }
}

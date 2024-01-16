// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Controls.XHtmlDisplay
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Ceco.Controls
{
  public class XHtmlDisplay : ScrollableControl
  {
    private IContainer components;
    private XHtmlControlRenderer renderer;

    public XHtmlDisplay()
    {
      this.InitializeComponent();
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.DoubleBuffered = true;
      this.AutoScroll = true;
      this.renderer.Body.ActualSizeChanged += new EventHandler(this.TextBlock_ActualSizeChanged);
    }

    [Browsable(true)]
    public override string Text
    {
      get => base.Text;
      set => base.Text = value;
    }

    [Browsable(true)]
    public Size DisplayMargin
    {
      get => this.renderer.Body.Margin;
      set => this.renderer.Body.Margin = value;
    }

    private void TextBlock_ActualSizeChanged(object sender, EventArgs e)
    {
      this.AutoScrollMinSize = this.renderer.Body.ActualSize;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.renderer = new XHtmlControlRenderer();
      this.SuspendLayout();
      this.renderer.Control = (Control) this;
      this.Name = "HtmlDisplay";
      this.Size = new Size(248, 252);
      this.ResumeLayout(false);
    }
  }
}

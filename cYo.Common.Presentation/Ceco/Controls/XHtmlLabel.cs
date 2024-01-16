// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Controls.XHtmlLabel
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
  public class XHtmlLabel : Control
  {
    private ContentAlignment textAlign = ContentAlignment.MiddleLeft;
    private Size textMargin;
    private XHtmlControlRenderer renderer;

    public XHtmlLabel()
    {
      this.InitializeComponent();
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.DoubleBuffered = true;
      this.BackColor = Color.Transparent;
      this.renderer.Control = (Control) this;
    }

    [Browsable(true)]
    public override bool AutoSize
    {
      get => base.AutoSize;
      set => base.AutoSize = value;
    }

    [DefaultValue(typeof (Color), "Transparent")]
    public override Color BackColor
    {
      get => base.BackColor;
      set => base.BackColor = value;
    }

    [Category("Appearance")]
    [Description("The alignment of the text")]
    [DefaultValue(ContentAlignment.MiddleLeft)]
    public ContentAlignment TextAlign
    {
      get => this.textAlign;
      set
      {
        if (this.textAlign == value)
          return;
        this.textAlign = value;
        this.OnTextAlignChanged();
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [Description("The margin between content and border")]
    [DefaultValue(typeof (Size), "0,0")]
    public Size TextMargin
    {
      get => this.textMargin;
      set
      {
        if (this.textMargin == value)
          return;
        this.textMargin = value;
        this.OnTextMarginChanged();
        this.Recalculate();
        this.Invalidate();
      }
    }

    public event EventHandler TextAlignChanged;

    public event EventHandler TextMarginChanged;

    protected virtual void OnTextMarginChanged()
    {
      if (this.TextMarginChanged == null)
        return;
      this.TextMarginChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnTextAlignChanged()
    {
      this.renderer.Body.SetAlign(this.textAlign);
      if (this.TextAlignChanged == null)
        return;
      this.TextAlignChanged((object) this, EventArgs.Empty);
    }

    private void Recalculate()
    {
      if (!this.AutoSize)
        return;
      this.Size = this.GetPreferredSize(this.PreferredSize);
    }

    private void InitializeComponent()
    {
      this.renderer = new XHtmlControlRenderer();
      this.SuspendLayout();
      this.renderer.Control = (Control) null;
      this.ResumeLayout(false);
    }
  }
}

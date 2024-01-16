// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.CaptionControl
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class CaptionControl : UserControl
  {
    private Padding captionMargin = new Padding(2);
    private bool closeButton;
    private bool selected;
    private IContainer components;

    public CaptionControl()
    {
      this.InitializeComponent();
      this.SetStyle(ControlStyles.ResizeRedraw, true);
    }

    [Category("Display")]
    [DefaultValue(null)]
    public string Caption
    {
      get => this.Text;
      set => this.Text = value;
    }

    [Category("Display")]
    [DefaultValue(typeof (Padding), "2")]
    public Padding CaptionMargin
    {
      get => this.captionMargin;
      set
      {
        if (this.captionMargin == value)
          return;
        this.captionMargin = value;
        this.Refresh();
      }
    }

    [Category("Display")]
    [DefaultValue(false)]
    public bool CloseButton
    {
      get => this.closeButton;
      set
      {
        if (this.closeButton == value)
          return;
        this.closeButton = value;
        this.InvalidateCaption();
      }
    }

    private Rectangle CaptionRectangle
    {
      get
      {
        if (string.IsNullOrEmpty(this.Caption))
          return Rectangle.Empty;
        System.Drawing.Size size = TextRenderer.MeasureText(this.Caption, SystemFonts.SmallCaptionFont);
        return this.ClientRectangle with
        {
          Height = size.Height + this.captionMargin.Vertical
        };
      }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      this.DrawCaption(e.Graphics);
      e.Graphics.SetClip(this.CaptionRectangle, CombineMode.Exclude);
      base.OnPaintBackground(e);
    }

    public override Rectangle DisplayRectangle
    {
      get
      {
        Rectangle clientRectangle = this.ClientRectangle;
        Rectangle captionRectangle = this.CaptionRectangle;
        clientRectangle.Height -= captionRectangle.Height;
        clientRectangle.Y = captionRectangle.Height;
        return clientRectangle;
      }
    }

    protected override void OnLeave(EventArgs e)
    {
      base.OnLeave(e);
      this.selected = false;
      this.InvalidateCaption();
    }

    protected override void OnEnter(EventArgs e)
    {
      base.OnEnter(e);
      this.selected = true;
      this.InvalidateCaption();
    }

    private void DrawCaption(Graphics gr)
    {
      Rectangle captionRectangle = this.CaptionRectangle;
      if (captionRectangle.Height == 0)
        return;
      gr.FillRectangle(Brushes.White, captionRectangle);
      gr.DrawStyledRectangle(captionRectangle, this.selected ? (int) byte.MaxValue : 128, StyledRenderer.VistaColor, StyledRenderer.Default.Frame(0, 1));
      TextRenderer.DrawText((IDeviceContext) gr, this.Caption, SystemFonts.SmallCaptionFont, captionRectangle.Pad(this.captionMargin), SystemColors.ActiveCaptionText);
    }

    private void InvalidateCaption() => this.Invalidate(this.CaptionRectangle);

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Name = nameof (CaptionControl);
      this.ResumeLayout(false);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.CollapsibleGroupBox
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using cYo.Common.Windows.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class CollapsibleGroupBox : ContainerControl
  {
    private System.Drawing.Size fullSize;
    private int headerHeight = 24;
    private FontStyle fontStyle = FontStyle.Bold;
    private bool collapsed;
    private bool useTheme = true;
    private Rectangle cachedHeaderRectangle;
    private Bitmap collapsedImage;
    private Bitmap expandedImage;
    private static readonly Image arrowDown = (Image) Resources.SimpleArrowDown.ScaleDpi();
    private static readonly Image arrowRight = (Image) Resources.SimpleArrowRight.ScaleDpi();

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FullHeight => this.fullSize.Height;

    [Browsable(false)]
    [DefaultValue(24)]
    public int HeaderHeight
    {
      get => this.headerHeight;
      set
      {
        if (this.headerHeight == value)
          return;
        this.headerHeight = value;
        if (this.collapsed)
          this.Height = FormUtility.ScaleDpiY(this.headerHeight);
        this.Invalidate();
      }
    }

    [DefaultValue(FontStyle.Bold)]
    public FontStyle HeaderFontStyle
    {
      get => this.fontStyle;
      set
      {
        if (this.fontStyle == value)
          return;
        this.fontStyle = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Collapsed
    {
      get => this.collapsed;
      set
      {
        if (value == this.collapsed)
          return;
        this.collapsed = value;
        if (!value)
        {
          this.Size = this.fullSize;
        }
        else
        {
          this.fullSize = this.Size;
          this.Height = FormUtility.ScaleDpiY(this.headerHeight);
        }
        this.Invalidate();
        this.OnCollapsedChanged();
      }
    }

    [DefaultValue(true)]
    public bool UseTheme
    {
      get => this.useTheme;
      set
      {
        if (this.useTheme == value)
          return;
        this.useTheme = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool TransparentTouch { get; set; }

    protected Rectangle HeaderRectangle
    {
      get
      {
        Rectangle headerRectangle = new Rectangle(0, 0, this.ClientRectangle.Width, FormUtility.ScaleDpiY(this.headerHeight));
        if (headerRectangle != this.cachedHeaderRectangle)
        {
          this.cachedHeaderRectangle = headerRectangle;
          this.RebuildRegion();
        }
        return headerRectangle;
      }
    }

    protected Rectangle ToggleRectange
    {
      get
      {
        Rectangle headerRectangle = this.HeaderRectangle;
        headerRectangle.Width = headerRectangle.Height;
        return headerRectangle;
      }
    }

    [DefaultValue(null)]
    public Bitmap CollapsedImage
    {
      get => this.collapsedImage;
      set
      {
        if (this.collapsedImage == value)
          return;
        this.collapsedImage = value;
        this.Invalidate(this.ToggleRectange);
      }
    }

    [DefaultValue(null)]
    public Bitmap ExpandedImage
    {
      get => this.expandedImage;
      set
      {
        if (this.expandedImage == value)
          return;
        this.expandedImage = value;
        this.Invalidate(this.ToggleRectange);
      }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      if (!this.ToggleRectange.Contains(e.Location))
        return;
      this.Collapsed = !this.Collapsed;
      this.OnCollapseClicked();
    }

    protected override void OnDoubleClick(EventArgs e)
    {
      if (!this.HeaderRectangle.Contains(this.PointToClient(Cursor.Position)))
      {
        base.OnDoubleClick(e);
      }
      else
      {
        this.Collapsed = !this.Collapsed;
        this.OnCollapseClicked();
      }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      base.OnPaintBackground(e);
      if (!this.UsesTheme || this.TransparentTouch)
        return;
      new VisualStyleRenderer(VisualStyleElement.Tab.Body.Normal).DrawBackground((IDeviceContext) e.Graphics, this.ClientRectangle);
    }

    public bool UsesTheme
    {
      get
      {
        return this.useTheme && VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Tab.Body.Normal);
      }
    }

    public override Color BackColor
    {
      get => !this.UsesTheme || this.TransparentTouch ? base.BackColor : Color.Transparent;
      set => base.BackColor = value;
    }

    protected override void OnPaint(PaintEventArgs e) => this.DrawBox(e.Graphics);

    public override Rectangle DisplayRectangle
    {
      get
      {
        Rectangle displayRectangle = base.DisplayRectangle;
        return new Rectangle(0, FormUtility.ScaleDpiY(this.headerHeight), displayRectangle.Width, displayRectangle.Height - FormUtility.ScaleDpiY(this.headerHeight));
      }
    }

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      this.Invalidate(this.HeaderRectangle);
    }

    private void DrawBox(Graphics gr)
    {
      Rectangle headerRectangle = this.HeaderRectangle;
      if (this.UsesTheme)
      {
        using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(headerRectangle, SystemColors.Control, SystemColors.ControlLight, 0.0f))
          gr.FillRectangle((Brush) linearGradientBrush, headerRectangle);
      }
      else
        gr.FillRectangle(SystemBrushes.ControlDark, headerRectangle);
      Image toggleImage = this.GetToggleImage();
      gr.DrawImage(toggleImage, toggleImage.Size.Align(this.ToggleRectange, System.Drawing.ContentAlignment.MiddleCenter));
      using (StringFormat format = new StringFormat()
      {
        LineAlignment = StringAlignment.Center
      })
      {
        using (Brush brush = (Brush) new SolidBrush(this.ForeColor))
          gr.DrawString(this.Text, FC.Get(this.Font, this.HeaderFontStyle), brush, (RectangleF) headerRectangle.Pad(this.ToggleRectange.Width, 0), format);
      }
    }

    private Image GetToggleImage()
    {
      return !this.collapsed ? (Image) this.expandedImage ?? CollapsibleGroupBox.arrowDown : (Image) this.collapsedImage ?? CollapsibleGroupBox.arrowRight;
    }

    public event EventHandler CollapsedChanged;

    public event EventHandler CollapseClicked;

    protected virtual void OnCollapsedChanged()
    {
      if (this.CollapsedChanged == null)
        return;
      this.CollapsedChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnCollapseClicked()
    {
      if (this.CollapseClicked == null)
        return;
      this.CollapseClicked((object) this, EventArgs.Empty);
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
      base.OnControlAdded(e);
      e.Control.SizeChanged += new EventHandler(this.Control_SizeChanged);
      e.Control.LocationChanged += new EventHandler(this.Control_SizeChanged);
      this.RebuildRegion();
    }

    private void Control_SizeChanged(object sender, EventArgs e) => this.RebuildRegion();

    protected override void OnControlRemoved(ControlEventArgs e)
    {
      base.OnControlRemoved(e);
      e.Control.SizeChanged -= new EventHandler(this.Control_SizeChanged);
      e.Control.LocationChanged -= new EventHandler(this.Control_SizeChanged);
      this.RebuildRegion();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      this.RebuildRegion();
    }

    private void RebuildRegion()
    {
      if (!this.TransparentTouch || this.DesignMode)
        return;
      Region region = new Region(this.HeaderRectangle);
      foreach (Control control in (ArrangedElementCollection) this.Controls)
      {
        if (!(control is Label))
          region.Union(control.Bounds);
      }
      this.Region = region;
    }
  }
}

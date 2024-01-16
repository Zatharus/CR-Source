// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Controls.RatingControl
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Controls
{
  public class RatingControl : Control
  {
    private Image ratingImage;
    private int maximumRating = 5;
    private int ratingDigits = 1;
    private bool drawText;
    private bool drawBorder = true;
    private bool centerRating = true;
    private RatingRenderer Renderer;

    public RatingControl()
    {
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.Selectable, true);
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics graphics = e.Graphics;
      if (Application.RenderWithVisualStyles)
      {
        if (this.drawBorder)
          new VisualStyleRenderer(this.Focused ? VisualStyleElement.TextBox.TextEdit.Focused : VisualStyleElement.TextBox.TextEdit.Normal).DrawBackground((IDeviceContext) graphics, this.ClientRectangle);
        this.DrawContent(graphics);
      }
      else
      {
        if (this.drawBorder)
          graphics.Clear(SystemColors.Window);
        this.DrawContent(graphics);
        if (!this.drawBorder)
          return;
        if (this.Focused)
        {
          Rectangle clientRectangle = this.ClientRectangle;
          clientRectangle.Inflate(-2, -2);
          ControlPaint.DrawFocusRectangle(graphics, clientRectangle);
        }
        ControlPaint.DrawBorder3D(graphics, this.ClientRectangle, Border3DStyle.Sunken);
      }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.Focus();
      if (this.Renderer == null)
        return;
      this.Text = Math.Round((double) this.Renderer.GetRatingFromStrip(e.Location), this.ratingDigits).ToString();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!this.Capture || this.Renderer == null)
        return;
      this.Text = Math.Round((double) this.Renderer.GetRatingFromStrip(e.Location), this.ratingDigits).ToString();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      base.OnMouseWheel(e);
      this.AdjustRating(e.Delta / SystemInformation.MouseWheelScrollDelta);
    }

    public override string Text
    {
      get => base.Text;
      set
      {
        if (base.Text == value)
          return;
        base.Text = value;
        this.Invalidate();
      }
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
      if (char.IsNumber(e.KeyChar))
      {
        this.Rating = (float) int.Parse(e.KeyChar.ToString());
        e.Handled = true;
      }
      base.OnKeyPress(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Prior:
          ++this.Rating;
          break;
        case Keys.Next:
          --this.Rating;
          break;
        case Keys.End:
          this.Rating = (float) this.MaximumRating;
          break;
        case Keys.Home:
          this.Rating = 0.0f;
          break;
        case Keys.Left:
        case Keys.Down:
          this.AdjustRating(-1);
          break;
        case Keys.Up:
        case Keys.Right:
          this.AdjustRating(1);
          break;
      }
      base.OnKeyDown(e);
    }

    protected override bool IsInputKey(Keys keyData)
    {
      switch (keyData)
      {
        case Keys.Prior:
        case Keys.Next:
        case Keys.End:
        case Keys.Home:
        case Keys.Left:
        case Keys.Up:
        case Keys.Right:
        case Keys.Down:
          return true;
        default:
          return base.IsInputKey(keyData);
      }
    }

    [Category("Display")]
    [Description("The image to display a rating point")]
    [DefaultValue(null)]
    public Image RatingImage
    {
      get => this.ratingImage;
      set
      {
        if (this.ratingImage == value)
          return;
        this.ratingImage = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [Description("Number of stars to use")]
    [DefaultValue(5)]
    public int MaximumRating
    {
      get => this.maximumRating;
      set
      {
        value = value.Clamp(0, 100);
        if (this.maximumRating == value)
          return;
        this.maximumRating = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [Description("The rating value")]
    [DefaultValue(0)]
    public float Rating
    {
      get
      {
        float result;
        float.TryParse(this.Text, out result);
        return ((float) Math.Round((double) result, this.ratingDigits)).Clamp(0.0f, (float) this.MaximumRating);
      }
      set
      {
        value = value.Clamp(0.0f, (float) this.MaximumRating);
        if ((double) this.Rating == (double) value)
          return;
        this.Text = value.ToString();
      }
    }

    [Category("Behavior")]
    [Description("Digits for the rating")]
    [DefaultValue(1)]
    public int RatingDigits
    {
      get => this.ratingDigits;
      set => this.ratingDigits = value;
    }

    [Category("Display")]
    [Description("Also draw the numeric value")]
    [DefaultValue(false)]
    public bool DrawText
    {
      get => this.drawText;
      set
      {
        if (this.drawText == value)
          return;
        this.drawText = value;
        this.Invalidate();
      }
    }

    [Category("Display")]
    [Description("Draw Border")]
    [DefaultValue(true)]
    public bool DrawBorder
    {
      get => this.drawBorder;
      set
      {
        if (this.drawBorder == value)
          return;
        this.drawBorder = value;
        this.Invalidate();
      }
    }

    [Category("Display")]
    [Description("Center Rating Stars")]
    [DefaultValue(true)]
    public bool CenterRating
    {
      get => this.centerRating;
      set
      {
        if (this.centerRating == value)
          return;
        this.centerRating = value;
        this.Invalidate();
      }
    }

    private void AdjustRating(int direction)
    {
      this.Rating += (float) direction * (float) Math.Pow(10.0, (double) -this.RatingDigits);
    }

    private void DrawContent(Graphics gr)
    {
      Rectangle rectangle1 = this.ClientRectangle;
      rectangle1.Inflate(-2, -2);
      if (this.drawText)
      {
        Color color = (double) this.Rating == 0.0 ? Color.LightGray : this.ForeColor;
        string ratingText = this.GetRatingText(this.Rating);
        System.Drawing.Size size = gr.MeasureString(ratingText, this.Font).ToSize();
        size.Width += 4;
        Rectangle rectangle2 = new Rectangle(rectangle1.Right - size.Width - 2, rectangle1.Top + (rectangle1.Height - size.Height) / 2, size.Width, size.Height + 1);
        using (gr.AntiAlias())
        {
          using (Pen pen = new Pen(color, 1.5f))
          {
            using (Brush brush = (Brush) new SolidBrush(color))
            {
              using (StringFormat format = new StringFormat()
              {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
              })
              {
                using (GraphicsPath path = rectangle2.ConvertToPath(2, 2))
                  gr.DrawPath(pen, path);
                gr.DrawString(ratingText, this.Font, brush, (RectangleF) rectangle2, format);
              }
            }
          }
        }
        rectangle1 = rectangle1.Pad(0, 0, size.Width + 2);
      }
      this.Renderer = new RatingRenderer(this.RatingImage, rectangle1, this.MaximumRating);
      this.Renderer.RatingScaleMode = this.CenterRating ? RectangleScaleMode.Center : RectangleScaleMode.None;
      this.Renderer.DrawRatingStrip(gr, this.Rating);
    }

    private int GetRatingTextWidth(Graphics gr)
    {
      return !this.DrawText ? 0 : (int) Math.Ceiling((double) gr.MeasureString(this.GetRatingText((float) this.MaximumRating), this.Font).Width);
    }

    private string GetRatingText(float rating) => rating.ToString("N" + (object) this.ratingDigits);

    public static ToolStripControlHost InsertRatingControl(
      ContextMenuStrip strip,
      int index,
      Image star,
      Func<IEditRating> rating)
    {
      RatingControl r = new RatingControl();
      r.Height = FormUtility.ScaleDpiY(18);
      r.Width = FormUtility.ScaleDpiX(200);
      r.RatingImage = star;
      r.DrawText = true;
      r.DrawBorder = false;
      r.BackColor = Color.Transparent;
      r.CenterRating = false;
      strip.Opening += (CancelEventHandler) ((s, e) =>
      {
        IEditRating editRating = rating();
        r.Tag = (object) editRating;
        r.Rating = editRating.GetRating();
      });
      strip.Closed += (ToolStripDropDownClosedEventHandler) ((s, e) => (r.Tag as IEditRating).SetRating(r.Rating));
      ToolStripControlHost stripControlHost = new ToolStripControlHost((Control) r);
      strip.Items.Insert(index, (ToolStripItem) stripControlHost);
      return stripControlHost;
    }

    public static ToolStripControlHost AddRatingControl(
      ContextMenuStrip strip,
      Image star,
      Func<IEditRating> rating)
    {
      return RatingControl.InsertRatingControl(strip, strip.Items.Count, star, rating);
    }
  }
}

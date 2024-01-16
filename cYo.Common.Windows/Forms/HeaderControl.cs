// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.HeaderControl
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

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class HeaderControl : Control
  {
    private bool pressed;
    private HeaderAdornments headerAdornments;
    private StringAlignment textAlignment;
    private static readonly Image sortUpImage = (Image) Resources.SortUp;
    private static readonly Image sortDownImage = (Image) Resources.SortDown;
    private static readonly Image dropDownImage = (Image) Resources.SmallArrowDown;

    public HeaderControl()
    {
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.UserPaint, true);
    }

    [DefaultValue(false)]
    public bool Pressed
    {
      get => this.pressed;
      set
      {
        if (this.pressed == value)
          return;
        this.pressed = value;
        this.Invalidate();
      }
    }

    [DefaultValue(HeaderAdornments.None)]
    public HeaderAdornments HeaderAdornments
    {
      get => this.headerAdornments;
      set
      {
        if (this.headerAdornments == value)
          return;
        this.headerAdornments = value;
        this.Invalidate();
      }
    }

    [DefaultValue(StringAlignment.Near)]
    public StringAlignment TextAlignment
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

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      HeaderControl.Draw(e.Graphics, this.ClientRectangle, this.Font, this.TextAlignment, this.Text, HeaderState.Normal, this.HeaderAdornments);
    }

    public static void Draw(
      Graphics graphics,
      Rectangle bounds,
      Font font,
      StringAlignment alignment,
      string text,
      HeaderState state,
      HeaderAdornments adornments)
    {
      using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
      {
        LineAlignment = StringAlignment.Center,
        Alignment = alignment,
        Trimming = StringTrimming.EllipsisCharacter
      })
        HeaderControl.Draw(graphics, bounds, font, text, SystemColors.ControlText, format, state, adornments);
    }

    public static void Draw(
      Graphics graphics,
      Rectangle bounds,
      Font font,
      string text,
      Color textColor,
      StringFormat format,
      HeaderState state,
      HeaderAdornments adornments)
    {
      Color controlDark = SystemColors.ControlDark;
      graphics.FillRectangle(Brushes.White, bounds);
      StyledRenderer.AlphaStyle state1;
      switch (state)
      {
        case HeaderState.Active:
          state1 = StyledRenderer.AlphaStyle.Hot;
          break;
        case HeaderState.Hot:
          state1 = StyledRenderer.AlphaStyle.Selected;
          break;
        case HeaderState.Pressed:
          state1 = StyledRenderer.AlphaStyle.SelectedHot;
          break;
        default:
          state1 = StyledRenderer.AlphaStyle.Hot;
          break;
      }
      --bounds.Width;
      --bounds.Height;
      graphics.DrawStyledRectangle(bounds, state1, controlDark, StyledRenderer.Default.Frame(0, 1));
      bounds.Inflate(-2, 0);
      using (graphics.SaveState())
      {
        if (adornments.IsSet<HeaderAdornments>(HeaderAdornments.DropDown))
        {
          Rectangle dropDownBounds = HeaderControl.GetDropDownBounds(bounds);
          graphics.DrawLine(SystemPens.ControlDark, dropDownBounds.TopLeft().Add(0, 4), dropDownBounds.BottomLeft().Add(0, -4));
          graphics.DrawLine(SystemPens.ControlLight, dropDownBounds.TopLeft().Add(1, 4), dropDownBounds.BottomLeft().Add(1, -4));
          graphics.DrawImage(HeaderControl.dropDownImage, HeaderControl.dropDownImage.Size.Align(dropDownBounds, ContentAlignment.MiddleCenter));
          dropDownBounds.Inflate(4, 4);
          graphics.SetClip(dropDownBounds, CombineMode.Exclude);
        }
        if (adornments.IsSet<HeaderAdornments>(HeaderAdornments.SortDown))
          graphics.DrawImage(HeaderControl.sortDownImage, HeaderControl.sortDownImage.Size.Align(bounds.Pad(0, 1), ContentAlignment.TopCenter), 0.7f);
        else if (adornments.IsSet<HeaderAdornments>(HeaderAdornments.SortUp))
          graphics.DrawImage(HeaderControl.sortUpImage, HeaderControl.sortDownImage.Size.Align(bounds.Pad(0, 1), ContentAlignment.TopCenter), 0.7f);
        using (Brush brush = (Brush) new SolidBrush(textColor))
          graphics.DrawString(text, font, brush, (RectangleF) bounds.Pad(0, 3), format);
      }
    }

    public static Rectangle GetDropDownBounds(Rectangle bounds)
    {
      int width = Math.Min(HeaderControl.dropDownImage.Width + 4, bounds.Width);
      return new Rectangle(bounds.Right - width, bounds.Top, width, bounds.Height);
    }
  }
}

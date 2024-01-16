// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Controls.XHtmlControlRenderer
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Presentation.Ceco.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Ceco.Controls
{
  public class XHtmlControlRenderer : Component
  {
    private Control control;
    private readonly BodyBlock body = new BodyBlock();
    private Inline hotItem;

    public XHtmlControlRenderer()
    {
      this.body.PendingLayoutChanged += new EventHandler(this.textBlock_PendingLayoutChanged);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Body.Dispose();
        this.Control = (Control) null;
      }
      base.Dispose(disposing);
    }

    public Control Control
    {
      get => this.control;
      set
      {
        if (this.control == value)
          return;
        this.OnControlChanging();
        this.control = value;
        this.OnControlChanged();
      }
    }

    public BodyBlock Body => this.body;

    public Inline HotItem => this.hotItem;

    protected virtual void OnControlChanging()
    {
      if (this.control == null)
        return;
      this.RegisterControl(false);
    }

    protected virtual void OnControlChanged()
    {
      if (this.control == null)
        return;
      this.RegisterControl(true);
      this.body.FontFamily = this.control.Font.FontFamily.Name;
      this.body.FontSizeEM = this.control.Font.Size;
      this.body.FontStyle = this.control.Font.Style;
      this.body.ForeColor = this.control.ForeColor;
    }

    public Size GetPreferredSize(Size proposedSize)
    {
      if (proposedSize.Height == 0)
        proposedSize.Height = int.MaxValue;
      if (proposedSize.Width == 0)
        proposedSize.Width = int.MaxValue;
      this.body.Bounds = new Rectangle(Point.Empty, proposedSize);
      using (Graphics graphics = this.control.CreateGraphics())
        this.body.Measure(graphics, this.body.Width);
      return this.body.ActualSize;
    }

    private void RegisterControl(bool register)
    {
      if (register)
      {
        this.control.Paint += new PaintEventHandler(this.control_Paint);
        this.control.Resize += new EventHandler(this.control_Resize);
        this.control.FontChanged += new EventHandler(this.control_FontChanged);
        this.control.TextChanged += new EventHandler(this.control_TextChanged);
        this.control.BackColorChanged += new EventHandler(this.control_BackColorChanged);
        this.control.ForeColorChanged += new EventHandler(this.control_ForeColorChanged);
        this.control.MouseMove += new MouseEventHandler(this.control_MouseMove);
        this.control.AutoSizeChanged += new EventHandler(this.control_AutoSizeChanged);
      }
      else
      {
        this.control.Paint -= new PaintEventHandler(this.control_Paint);
        this.control.Resize -= new EventHandler(this.control_Resize);
        this.control.FontChanged -= new EventHandler(this.control_FontChanged);
        this.control.TextChanged -= new EventHandler(this.control_TextChanged);
        this.control.BackColorChanged -= new EventHandler(this.control_BackColorChanged);
        this.control.ForeColorChanged -= new EventHandler(this.control_ForeColorChanged);
        this.control.MouseMove -= new MouseEventHandler(this.control_MouseMove);
        this.control.AutoSizeChanged += new EventHandler(this.control_AutoSizeChanged);
      }
    }

    private void control_MouseMove(object sender, MouseEventArgs e)
    {
      Point location1 = e.Location;
      ref Point local = ref location1;
      Point location2 = this.control.DisplayRectangle.Location;
      int dx = -location2.X;
      location2 = this.control.DisplayRectangle.Location;
      int dy = -location2.Y;
      local.Offset(dx, dy);
      Inline hitItem = this.body.GetHitItem(Point.Empty, location1);
      Cursor.Current = hitItem == null || hitItem.MouseCursor == (Cursor) null ? this.control.Cursor : hitItem.MouseCursor;
      if (this.hotItem == hitItem)
        return;
      if (this.hotItem != null)
        this.hotItem.MouseLeave();
      hitItem?.MouseEnter();
      this.hotItem = hitItem;
    }

    private void control_ForeColorChanged(object sender, EventArgs e)
    {
      this.body.ForeColor = this.control.ForeColor;
      this.control.Invalidate();
    }

    private void control_BackColorChanged(object sender, EventArgs e) => this.control.Invalidate();

    private void control_TextChanged(object sender, EventArgs e)
    {
      this.body.Inlines.Clear();
      try
      {
        this.body.Inlines.AddRange((IEnumerable<Inline>) XHtmlParser.Parse(this.control.Text).Inlines);
      }
      catch
      {
      }
    }

    private void control_FontChanged(object sender, EventArgs e)
    {
      this.body.FontFamily = this.control.Font.FontFamily.Name;
      this.body.FontSizeEM = this.control.Font.Size;
      this.body.FontStyle = this.control.Font.Style;
      this.control.Invalidate();
    }

    private void control_Resize(object sender, EventArgs e)
    {
      if (this.control.AutoSize)
        this.control.Size = this.GetPreferredSize(this.control.PreferredSize);
      this.control.Invalidate();
    }

    private void control_Paint(object sender, PaintEventArgs e)
    {
      this.body.Bounds = this.control.ClientRectangle;
      this.body.Draw(e.Graphics, this.control.DisplayRectangle.Location);
    }

    private void textBlock_PendingLayoutChanged(object sender, EventArgs e)
    {
      this.control.Invalidate();
    }

    private void control_AutoSizeChanged(object sender, EventArgs e)
    {
      if (!this.control.AutoSize)
        return;
      this.control.Size = this.GetPreferredSize(this.control.PreferredSize);
    }
  }
}

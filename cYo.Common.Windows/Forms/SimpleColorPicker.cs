// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.SimpleColorPicker
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Text;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class SimpleColorPicker : ComboBox
  {
    private IContainer components;

    public SimpleColorPicker()
    {
      this.InitializeComponent();
      this.DrawMode = DrawMode.OwnerDrawFixed;
      this.Items.Clear();
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
      base.OnDrawItem(e);
      Graphics graphics = e.Graphics;
      e.DrawBackground();
      using (StringFormat format = new StringFormat()
      {
        Alignment = StringAlignment.Near,
        LineAlignment = StringAlignment.Center
      })
      {
        try
        {
          Color color = (Color) this.Items[e.Index];
          Rectangle bounds1 = e.Bounds;
          bounds1.Width = bounds1.Height * 3 / 2;
          bounds1.Inflate(-2, -2);
          using (Brush brush = (Brush) new SolidBrush(color))
            graphics.FillRectangle(brush, bounds1);
          using (Pen pen = new Pen(e.ForeColor))
            graphics.DrawRectangle(pen, bounds1);
          Rectangle bounds2 = e.Bounds with
          {
            X = 5 + bounds1.Right
          };
          bounds2.Width = e.Bounds.Width - bounds2.X;
          using (Brush brush = (Brush) new SolidBrush(e.ForeColor))
          {
            string spaced = color.ToKnownColor().ToString().PascalToSpaced();
            graphics.DrawString(spaced, e.Font, brush, (RectangleF) bounds2, format);
          }
        }
        catch (Exception ex)
        {
          using (Brush brush = (Brush) new SolidBrush(e.ForeColor))
            graphics.DrawString("Unknown Color", e.Font, brush, (RectangleF) e.Bounds, format);
        }
      }
    }

    public void FillKnownColors(bool includingSystem)
    {
      foreach (KnownColor color1 in Enum.GetValues(typeof (KnownColor)))
      {
        Color color2 = Color.FromKnownColor(color1);
        if (color2.A == byte.MaxValue && (!color2.IsSystemColor || color2.IsSystemColor & includingSystem))
          this.Items.Add((object) color2);
      }
    }

    public string SelectedColorName
    {
      get => this.SelectedColor.Name;
      set => this.SelectedColor = Color.FromName(value);
    }

    public Color SelectedColor
    {
      get => this.SelectedItem != null ? (Color) this.SelectedItem : Color.Empty;
      set
      {
        foreach (Color color in this.Items)
        {
          if (color == value)
          {
            this.SelectedItem = (object) color;
            break;
          }
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent() => this.components = (IContainer) new System.ComponentModel.Container();
  }
}

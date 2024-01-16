// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.Format.HorizontalRule
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco.Format
{
  public class HorizontalRule : Block
  {
    private int thickness = 2;
    private bool noshade;

    protected override void CoreMeasure(Graphics gr, int maxWidth, LayoutType tbl)
    {
      this.Width = this.BlockWidth.GetSize(maxWidth);
      this.MinimumWidth = this.BlockWidth.IsFixed ? this.Width : 0;
      this.Height = this.Thickness + (this.Noshade ? 0 : 1);
    }

    public override void Draw(Graphics gr, Point location)
    {
      base.Draw(gr, location);
      Rectangle bounds = this.Bounds;
      bounds.Offset(location);
      if (!this.noshade)
      {
        --bounds.Width;
        --bounds.Height;
      }
      if (!this.noshade)
      {
        bounds.Offset(1, 1);
        gr.FillRectangle(Brushes.DarkGray, bounds);
        bounds.Offset(-1, -1);
      }
      using (Brush brush = (Brush) new SolidBrush(this.ForeColor))
        gr.FillRectangle(brush, bounds);
    }

    public override FlowBreak FlowBreak => FlowBreak.BreakLine | FlowBreak.Before | FlowBreak.After;

    public override int FlowBreakOffset => this.Font.Height;

    public int Thickness
    {
      get => this.thickness;
      set
      {
        if (this.thickness == value)
          return;
        this.thickness = value;
        this.OnThicknessChanged();
      }
    }

    public bool Noshade
    {
      get => this.noshade;
      set
      {
        if (this.noshade == value)
          return;
        this.noshade = value;
        this.OnNoShadeChanged();
      }
    }

    protected virtual void OnThicknessChanged() => this.InvokeLayout(LayoutType.Full);

    protected virtual void OnNoShadeChanged() => this.InvokeLayout(LayoutType.Full);
  }
}

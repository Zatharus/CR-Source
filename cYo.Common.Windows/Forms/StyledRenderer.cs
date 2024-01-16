// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.StyledRenderer
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public static class StyledRenderer
  {
    public static readonly Color VistaColor = Color.FromArgb(153, 222, 253);
    public static readonly StyledRenderer.StyleDefinition Vista = new StyledRenderer.StyleDefinition(92, 164, (int) byte.MaxValue, 2, 1, 0.5f, 64);
    public static readonly StyledRenderer.StyleDefinition Windows8 = new StyledRenderer.StyleDefinition(92, 164, (int) byte.MaxValue, 0, 1, 0.25f, 0);
    private static readonly Lazy<int> version = new Lazy<int>((Func<int>) (() => Environment.OSVersion.Version.Major * 100 + Environment.OSVersion.Version.Minor));

    public static void DrawRectangle(
      this Graphics gr,
      Rectangle rc,
      Color baseColor,
      int rounding,
      int frameWidth,
      int frameAlpha,
      int backAlphaStart,
      int backAlphaEnd)
    {
      using (gr.AntiAlias())
      {
        Color color = Color.FromArgb(frameAlpha, baseColor);
        if (backAlphaStart >= 0 && backAlphaEnd >= 0)
        {
          Color color1 = Color.FromArgb(backAlphaStart, baseColor);
          Color color2 = Color.FromArgb(backAlphaEnd, baseColor);
          Rectangle rectangle = rc;
          rectangle.Inflate(-frameWidth, -frameWidth);
          using (GraphicsPath path = rectangle.ConvertToPath(rounding, rounding))
          {
            using (Brush brush = (Brush) new LinearGradientBrush(rectangle, color1, color2, -90f)
            {
              WrapMode = WrapMode.TileFlipXY
            })
              gr.FillPath(brush, path);
          }
        }
        if (frameWidth <= 0)
          return;
        int num = frameWidth / 2;
        rc.Inflate(-num, -num);
        using (GraphicsPath path = rc.ConvertToPath(rounding, rounding))
        {
          using (Pen pen = new Pen(color, (float) frameWidth))
            gr.DrawPath(pen, path);
        }
      }
    }

    public static StyledRenderer.StyleDefinition Default
    {
      get => StyledRenderer.version.Value < 602 ? StyledRenderer.Vista : StyledRenderer.Windows8;
    }

    public static StyledRenderer.AlphaStyle GetAlphaStyle(bool selected, bool hot, bool focused)
    {
      if ((hot | focused) & selected)
        return StyledRenderer.AlphaStyle.SelectedHot;
      if (hot)
        return StyledRenderer.AlphaStyle.Hot;
      if (selected)
        return StyledRenderer.AlphaStyle.Selected;
      return focused ? StyledRenderer.AlphaStyle.Focused : StyledRenderer.AlphaStyle.None;
    }

    public static Color GetSelectionColor(bool focused)
    {
      return !focused ? Color.Gray : SystemColors.Highlight;
    }

    public static void DrawStyledRectangle(
      this Graphics gr,
      Rectangle rc,
      int baseAlpha,
      Color baseColor,
      StyledRenderer.StyleDefinition style = null)
    {
      if (style == null)
        style = StyledRenderer.Default;
      int frameAlpha = Math.Abs(baseAlpha);
      int backAlphaStart = (int) ((double) baseAlpha * (double) style.BackAlpha);
      int backAlphaEnd = (backAlphaStart - style.BackGradient).Clamp(0, (int) byte.MaxValue);
      gr.DrawRectangle(rc, baseColor, style.Rounding, style.FrameWidth, frameAlpha, backAlphaStart, backAlphaEnd);
    }

    public static void DrawStyledRectangle(
      this Graphics gr,
      Rectangle rc,
      StyledRenderer.AlphaStyle state,
      Color baseColor,
      StyledRenderer.StyleDefinition style = null)
    {
      int baseAlpha = 0;
      if (style == null)
        style = StyledRenderer.Default;
      switch (state)
      {
        case StyledRenderer.AlphaStyle.Hot:
          baseAlpha = style.AlphaHot;
          break;
        case StyledRenderer.AlphaStyle.Selected:
          baseAlpha = style.AlphaSelected;
          break;
        case StyledRenderer.AlphaStyle.SelectedHot:
          baseAlpha = style.AlphaSelectedHot;
          break;
        case StyledRenderer.AlphaStyle.Focused:
          baseAlpha = -style.AlphaSelected;
          break;
      }
      gr.DrawStyledRectangle(rc, baseAlpha, baseColor, style);
    }

    public enum AlphaStyle
    {
      None,
      Hot,
      Selected,
      SelectedHot,
      Focused,
    }

    public class StyleDefinition
    {
      public int AlphaHot { get; private set; }

      public int AlphaSelected { get; private set; }

      public int AlphaSelectedHot { get; private set; }

      public int Rounding { get; private set; }

      public int FrameWidth { get; private set; }

      public float BackAlpha { get; private set; }

      public int BackGradient { get; private set; }

      public StyleDefinition(
        int hot,
        int selected,
        int selectedHot,
        int rounding,
        int frameWidth,
        float backAlpha,
        int backGradient)
      {
        this.AlphaHot = hot;
        this.AlphaSelected = selected;
        this.AlphaSelectedHot = selectedHot;
        this.Rounding = rounding;
        this.FrameWidth = frameWidth;
        this.BackAlpha = backAlpha;
        this.BackGradient = backGradient;
      }

      public StyleDefinition(StyledRenderer.StyleDefinition sd)
      {
        this.AlphaHot = sd.AlphaHot;
        this.AlphaSelected = sd.AlphaSelected;
        this.AlphaSelectedHot = sd.AlphaSelectedHot;
        this.Rounding = sd.Rounding;
        this.FrameWidth = sd.FrameWidth;
        this.BackAlpha = sd.BackAlpha;
        this.BackGradient = sd.BackGradient;
      }

      public StyledRenderer.StyleDefinition Frame(int rounding, int width)
      {
        StyledRenderer.StyleDefinition styleDefinition = new StyledRenderer.StyleDefinition(this);
        if (rounding >= 0)
          styleDefinition.Rounding = rounding;
        if (width >= 0)
          styleDefinition.FrameWidth = width;
        return styleDefinition;
      }

      public StyledRenderer.StyleDefinition NoGradient()
      {
        return new StyledRenderer.StyleDefinition(this)
        {
          BackGradient = 0
        };
      }
    }
  }
}

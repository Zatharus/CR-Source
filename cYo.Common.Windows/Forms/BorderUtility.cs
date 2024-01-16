// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.BorderUtility
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public static class BorderUtility
  {
    public static void DrawBorder(Graphics g, Rectangle bounds, ExtendedBorderStyle style)
    {
      Border3DStyle style1;
      switch (style)
      {
        case ExtendedBorderStyle.Flat:
          style1 = Border3DStyle.Flat;
          break;
        case ExtendedBorderStyle.Sunken:
          style1 = Border3DStyle.Sunken;
          break;
        case ExtendedBorderStyle.Raised:
          style1 = Border3DStyle.Raised;
          break;
        default:
          return;
      }
      ControlPaint.DrawBorder3D(g, bounds, style1);
    }

    public static Rectangle AdjustBorder(Rectangle bounds, ExtendedBorderStyle style, bool inwards)
    {
      int num = inwards ? -1 : 1;
      switch (style)
      {
        case ExtendedBorderStyle.Flat:
          bounds.Inflate(num * 2, num * 2);
          break;
        case ExtendedBorderStyle.Sunken:
        case ExtendedBorderStyle.Raised:
          bounds.Inflate(num * 4, num * 4);
          break;
      }
      return bounds;
    }

    public static Rectangle AdjustBorder(Rectangle bounds, ExtendedBorderStyle style)
    {
      return BorderUtility.AdjustBorder(bounds, style, true);
    }
  }
}

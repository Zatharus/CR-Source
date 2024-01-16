// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.ScreenExtension
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows
{
  public static class ScreenExtension
  {
    public static bool IsPortrait(this Screen screen)
    {
      Rectangle bounds = screen.Bounds;
      int width = bounds.Width;
      bounds = screen.Bounds;
      int height = bounds.Height;
      return width < height;
    }

    public static bool IsLandscape(this Screen screen) => !screen.IsPortrait();
  }
}

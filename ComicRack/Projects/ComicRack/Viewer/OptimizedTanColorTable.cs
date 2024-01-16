// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.OptimizedTanColorTable
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows.Forms;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public class OptimizedTanColorTable : TanColorTable
  {
    public override Color MenuStripGradientEnd => this.MenuStripGradientBegin;

    public override Color ToolStripBorder => Color.Empty;
  }
}

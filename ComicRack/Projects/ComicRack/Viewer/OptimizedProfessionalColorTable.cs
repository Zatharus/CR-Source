// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.OptimizedProfessionalColorTable
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public class OptimizedProfessionalColorTable : ProfessionalColorTable
  {
    public override Color MenuStripGradientEnd => this.MenuStripGradientBegin;

    public override Color MenuItemSelectedGradientEnd
    {
      get => Color.FromArgb(128, this.MenuItemSelectedGradientBegin);
    }

    public override Color ButtonSelectedGradientEnd
    {
      get => Color.FromArgb(128, this.ButtonSelectedGradientBegin);
    }
  }
}

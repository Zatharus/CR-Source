// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ControlStyleColorTable
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ControlStyleColorTable : ProfessionalColorTable
  {
    private readonly Color backgroundColor = SystemColors.Control;
    private readonly Color lightColor = SystemColors.ControlLight;
    private readonly Color darkColor = SystemColors.ControlDark;
    private readonly Color borderColor = Color.Black;

    public override Color ToolStripGradientBegin => this.backgroundColor;

    public override Color ToolStripBorder => this.backgroundColor;

    public override Color ToolStripGradientEnd => this.backgroundColor;

    public override Color ToolStripGradientMiddle => this.backgroundColor;

    public override Color ToolStripPanelGradientBegin => this.backgroundColor;

    public override Color ToolStripPanelGradientEnd => this.backgroundColor;

    public override Color ToolStripContentPanelGradientBegin => this.backgroundColor;

    public override Color ToolStripContentPanelGradientEnd => this.backgroundColor;

    public override Color StatusStripGradientBegin => this.backgroundColor;

    public override Color StatusStripGradientEnd => this.backgroundColor;

    public override Color MenuStripGradientBegin => this.backgroundColor;

    public override Color MenuBorder => this.borderColor;

    public override Color MenuStripGradientEnd => this.backgroundColor;

    public override Color GripDark => this.darkColor;

    public override Color GripLight => this.lightColor;

    public override Color OverflowButtonGradientBegin => this.backgroundColor;

    public override Color OverflowButtonGradientEnd => this.backgroundColor;

    public override Color OverflowButtonGradientMiddle => this.backgroundColor;
  }
}

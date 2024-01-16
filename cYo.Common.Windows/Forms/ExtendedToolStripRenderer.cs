// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ExtendedToolStripRenderer
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ExtendedToolStripRenderer : ToolStripRenderer
  {
    private readonly ToolStripRenderer currentRenderer;

    public ExtendedToolStripRenderer(ToolStripRenderer renderer) => this.currentRenderer = renderer;

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
      this.currentRenderer.DrawToolStripBorder(e);
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
      this.currentRenderer.DrawToolStripBackground(e);
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
      this.currentRenderer.DrawButtonBackground(e);
    }

    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
      this.currentRenderer.DrawItemImage(e);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
      this.currentRenderer.DrawItemText(e);
    }

    protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
    {
      this.currentRenderer.DrawGrip(e);
    }

    protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
    {
      this.currentRenderer.DrawLabelBackground(e);
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
      this.currentRenderer.DrawArrow(e);
    }

    protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
      this.currentRenderer.DrawDropDownButtonBackground(e);
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
      this.currentRenderer.DrawImageMargin(e);
    }

    protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
    {
      this.currentRenderer.DrawItemBackground(e);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
      this.currentRenderer.DrawItemCheck(e);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
      this.currentRenderer.DrawMenuItemBackground(e);
    }

    protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
    {
      this.currentRenderer.DrawOverflowButtonBackground(e);
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
      this.currentRenderer.DrawSeparator(e);
    }

    protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
    {
      this.currentRenderer.DrawSplitButton(e);
    }

    protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
    {
      this.currentRenderer.DrawStatusStripSizingGrip(e);
    }

    protected override void OnRenderToolStripContentPanelBackground(
      ToolStripContentPanelRenderEventArgs e)
    {
      this.currentRenderer.DrawToolStripContentPanelBackground(e);
    }

    protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
    {
      this.currentRenderer.DrawToolStripPanelBackground(e);
    }

    protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
    {
      this.currentRenderer.DrawToolStripStatusLabelBackground(e);
    }
  }
}

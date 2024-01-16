// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Menus.ToolStripThumbSize
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows.Forms;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Menus
{
  internal class ToolStripThumbSize : ToolStripControlHost
  {
    public ToolStripThumbSize()
      : base((Control) new TrackBarLite())
    {
      this.Control.BackColor = Color.Transparent;
      this.TrackBar.ThumbSize = new System.Drawing.Size(6, 14);
      this.TrackBar.EnableFocusIndicator = false;
    }

    public TrackBarLite TrackBar => this.Control as TrackBarLite;

    public override System.Drawing.Size GetPreferredSize(System.Drawing.Size constrainingSize)
    {
      return new System.Drawing.Size(120, 16).ScaleDpi();
    }

    public void SetSlider(int min, int max, int val)
    {
      this.TrackBar.SetRange(min, max);
      this.TrackBar.Value = val;
    }
  }
}

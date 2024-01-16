// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ToolStripSearchTextBox
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.Windows.Forms;
using System.Windows.Forms.Design;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
  public class ToolStripSearchTextBox : ToolStripControlHost
  {
    public ToolStripSearchTextBox()
      : base((Control) new SearchTextBox())
    {
    }

    public override System.Drawing.Size GetPreferredSize(System.Drawing.Size constrainingSize)
    {
      return new System.Drawing.Size(130, 20).ScaleDpi();
    }

    public SearchTextBox TextBox => this.Control as SearchTextBox;
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.DropDownHost`1
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class DropDownHost<T> : ToolStripDropDown where T : System.Windows.Forms.Control, new()
  {
    private readonly T control = new T();

    public DropDownHost()
    {
      this.Items.Add((ToolStripItem) new ToolStripControlHost((System.Windows.Forms.Control) this.control));
    }

    protected override void OnOpened(EventArgs e)
    {
      base.OnOpened(e);
      this.control.Focus();
    }

    public T Control => this.control;
  }
}

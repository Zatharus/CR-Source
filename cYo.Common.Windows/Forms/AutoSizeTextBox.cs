// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.AutoSizeTextBox
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class AutoSizeTextBox : TextBox
  {
    private int autoSizePadding = 16;
    private bool autoSizeEnabled = true;

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      if (!this.autoSizeEnabled)
        return;
      this.Width = TextRenderer.MeasureText(this.Text, this.Font).Width + this.autoSizePadding;
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
      if (this.HandleTab && (keyData & Keys.KeyCode) == Keys.Tab)
      {
        KeyEventArgs e = new KeyEventArgs(keyData);
        this.OnKeyDown(e);
        if (e.Handled)
          return true;
      }
      return base.ProcessDialogKey(keyData);
    }

    [DefaultValue(16)]
    public int AutoSizePadding
    {
      get => this.autoSizePadding;
      set => this.autoSizePadding = value;
    }

    [DefaultValue(true)]
    public bool AutoSizeEnabled
    {
      get => this.autoSizeEnabled;
      set => this.autoSizeEnabled = value;
    }

    [DefaultValue(false)]
    public bool HandleTab { get; set; }
  }
}

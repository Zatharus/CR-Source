// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.KeyInputForm
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class KeyInputForm : Form
  {
    private string description;
    private IContainer components;

    public KeyInputForm() => this.InitializeComponent();

    [Browsable(false)]
    public CommandKey Key { get; private set; }

    public string Description
    {
      get => this.description;
      set
      {
        if (this.description == value)
          return;
        this.description = value;
        this.Invalidate();
      }
    }

    protected override bool IsInputKey(Keys keyData) => true;

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu)
        return;
      this.Key = (CommandKey) e.KeyData;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      if (string.IsNullOrEmpty(this.Description))
        return;
      using (SolidBrush solidBrush = new SolidBrush(this.ForeColor))
      {
        using (StringFormat format = new StringFormat()
        {
          Alignment = StringAlignment.Center,
          LineAlignment = StringAlignment.Center
        })
          e.Graphics.DrawString(this.Description, this.Font, (Brush) solidBrush, (RectangleF) this.ClientRectangle, format);
      }
    }

    public static CommandKey Show(IWin32Window parent, string caption, string description)
    {
      using (KeyInputForm keyInputForm = new KeyInputForm())
      {
        keyInputForm.Text = caption;
        keyInputForm.Description = description;
        return keyInputForm.ShowDialog(parent) == DialogResult.Cancel ? CommandKey.None : keyInputForm.Key;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 56);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (KeyInputForm);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Press any Key";
      this.ResumeLayout(false);
    }
  }
}

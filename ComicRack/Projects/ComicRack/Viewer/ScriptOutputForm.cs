// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.ScriptOutputForm
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public class ScriptOutputForm : Form
  {
    private IContainer components;
    public TextBox Log;

    public ScriptOutputForm() => this.InitializeComponent();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.Log = new TextBox();
      this.SuspendLayout();
      this.Log.Dock = DockStyle.Fill;
      this.Log.Font = new Font("Lucida Console", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.Log.Location = new Point(0, 0);
      this.Log.MaxLength = 1000000;
      this.Log.Multiline = true;
      this.Log.Name = "Log";
      this.Log.ScrollBars = ScrollBars.Both;
      this.Log.Size = new Size(606, 471);
      this.Log.TabIndex = 0;
      this.Log.WordWrap = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(606, 471);
      this.Controls.Add((Control) this.Log);
      this.Name = nameof (ScriptOutputForm);
      this.ShowIcon = false;
      this.Text = "Script Output";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

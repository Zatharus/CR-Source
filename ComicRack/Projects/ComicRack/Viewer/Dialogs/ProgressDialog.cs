// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ProgressDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ProgressDialog : Form
  {
    private ProgressBar progressBar;
    private Button btCancel;
    private System.ComponentModel.Container components;
    private bool cancel;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.progressBar = new ProgressBar();
      this.btCancel = new Button();
      this.SuspendLayout();
      this.progressBar.Location = new Point(16, 24);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new Size(384, 24);
      this.progressBar.TabIndex = 0;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new Point(304, 54);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new Size(96, 24);
      this.btCancel.TabIndex = 1;
      this.btCancel.Text = "Cancel";
      this.btCancel.Click += new EventHandler(this.btCancel_Click);
      this.AutoScaleBaseSize = new Size(5, 13);
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new Size(410, 88);
      this.ControlBox = false;
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.progressBar);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ProgressDialog);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = nameof (ProgressDialog);
      this.ResumeLayout(false);
    }

    public ProgressDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
    }

    public bool Progress(int percentDone)
    {
      this.progressBar.Value = percentDone;
      Application.DoEvents();
      return this.cancel;
    }

    private void btCancel_Click(object sender, EventArgs e) => this.cancel = true;
  }
}

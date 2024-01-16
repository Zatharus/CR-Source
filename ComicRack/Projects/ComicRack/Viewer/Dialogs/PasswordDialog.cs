// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.PasswordDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class PasswordDialog : Form
  {
    private IContainer components;
    private Label lblDescription;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel panel1;
    private CheckBox chkRemember;
    private Label labelPassword;
    private TextBox txPassword;
    private FlowLayoutPanel flowLayoutPanel2;
    private Button btOK;
    private Button btCancel;

    public PasswordDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
    }

    public string Description
    {
      get => this.lblDescription.Text;
      set => this.lblDescription.Text = value;
    }

    public string Password => this.txPassword.Text;

    public bool RememberPassword => this.chkRemember.Checked;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.lblDescription = new Label();
      this.flowLayoutPanel1 = new FlowLayoutPanel();
      this.panel1 = new Panel();
      this.chkRemember = new CheckBox();
      this.labelPassword = new Label();
      this.txPassword = new TextBox();
      this.flowLayoutPanel2 = new FlowLayoutPanel();
      this.btOK = new Button();
      this.btCancel = new Button();
      this.flowLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.flowLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      this.lblDescription.AutoSize = true;
      this.lblDescription.Location = new Point(8, 8);
      this.lblDescription.Margin = new Padding(4);
      this.lblDescription.Name = "lblDescription";
      this.lblDescription.Size = new Size(212, 13);
      this.lblDescription.TabIndex = 0;
      this.lblDescription.Text = "A password is needed for the remote library:";
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add((Control) this.lblDescription);
      this.flowLayoutPanel1.Controls.Add((Control) this.panel1);
      this.flowLayoutPanel1.Controls.Add((Control) this.flowLayoutPanel2);
      this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new Point(3, 3);
      this.flowLayoutPanel1.Margin = new Padding(0);
      this.flowLayoutPanel1.MaximumSize = new Size(310, 0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Padding = new Padding(4);
      this.flowLayoutPanel1.Size = new Size(309, 132);
      this.flowLayoutPanel1.TabIndex = 0;
      this.panel1.Controls.Add((Control) this.chkRemember);
      this.panel1.Controls.Add((Control) this.labelPassword);
      this.panel1.Controls.Add((Control) this.txPassword);
      this.panel1.Location = new Point(8, 29);
      this.panel1.Margin = new Padding(4);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(293, 59);
      this.panel1.TabIndex = 1;
      this.chkRemember.AutoSize = true;
      this.chkRemember.Location = new Point(62, 33);
      this.chkRemember.Name = "chkRemember";
      this.chkRemember.Size = new Size(143, 17);
      this.chkRemember.TabIndex = 2;
      this.chkRemember.Text = "Remember for this server";
      this.chkRemember.UseVisualStyleBackColor = true;
      this.labelPassword.AutoSize = true;
      this.labelPassword.Location = new Point(0, 10);
      this.labelPassword.Name = "labelPassword";
      this.labelPassword.Size = new Size(56, 13);
      this.labelPassword.TabIndex = 0;
      this.labelPassword.Text = "Password:";
      this.txPassword.Location = new Point(62, 7);
      this.txPassword.Name = "txPassword";
      this.txPassword.Size = new Size(224, 20);
      this.txPassword.TabIndex = 1;
      this.txPassword.UseSystemPasswordChar = true;
      this.flowLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.flowLayoutPanel2.AutoSize = true;
      this.flowLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel2.Controls.Add((Control) this.btOK);
      this.flowLayoutPanel2.Controls.Add((Control) this.btCancel);
      this.flowLayoutPanel2.Location = new Point(130, 95);
      this.flowLayoutPanel2.Name = "flowLayoutPanel2";
      this.flowLayoutPanel2.Size = new Size(172, 30);
      this.flowLayoutPanel2.TabIndex = 9;
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new Point(3, 3);
      this.btOK.Name = "btOK";
      this.btOK.Size = new Size(80, 24);
      this.btOK.TabIndex = 0;
      this.btOK.Text = "&OK";
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new Point(89, 3);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new Size(80, 24);
      this.btCancel.TabIndex = 1;
      this.btCancel.Text = "&Cancel";
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new Size(315, 138);
      this.Controls.Add((Control) this.flowLayoutPanel1);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (PasswordDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Password";
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.flowLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

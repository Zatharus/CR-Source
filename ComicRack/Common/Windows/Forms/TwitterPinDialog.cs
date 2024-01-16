// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TwitterPinDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TwitterPinDialog : Form
  {
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private Label labelCaption;
    private TextBox textPin;

    public TwitterPinDialog()
    {
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
    }

    public static string GetPin(IWin32Window parent)
    {
      using (TwitterPinDialog twitterPinDialog = new TwitterPinDialog())
        return twitterPinDialog.ShowDialog(parent) == DialogResult.OK ? twitterPinDialog.textPin.Text : (string) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.btCancel = new Button();
      this.btOK = new Button();
      this.labelCaption = new Label();
      this.textPin = new TextBox();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(217, 100);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 7;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(131, 100);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 6;
      this.btOK.Text = "&OK";
      this.labelCaption.AutoSize = true;
      this.labelCaption.Location = new System.Drawing.Point(12, 21);
      this.labelCaption.Name = "labelCaption";
      this.labelCaption.Size = new System.Drawing.Size(206, 13);
      this.labelCaption.TabIndex = 8;
      this.labelCaption.Text = "Please enter the Twitter authorization PIN:";
      this.textPin.Font = new Font("Microsoft Sans Serif", 20.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.textPin.Location = new System.Drawing.Point(15, 50);
      this.textPin.Name = "textPin";
      this.textPin.Size = new System.Drawing.Size(282, 38);
      this.textPin.TabIndex = 9;
      this.textPin.TextAlign = HorizontalAlignment.Center;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(309, 136);
      this.Controls.Add((Control) this.textPin);
      this.Controls.Add((Control) this.labelCaption);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (TwitterPinDialog);
      this.ShowIcon = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Twitter Authorization";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

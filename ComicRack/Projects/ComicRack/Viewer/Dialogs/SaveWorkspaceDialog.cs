// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.SaveWorkspaceDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Viewer.Config;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class SaveWorkspaceDialog : Form
  {
    private IContainer components;
    private TextBox txtName;
    private Label lblName;
    private Button btCancel;
    private Button btOK;
    private CheckBox chkWindowLayouts;
    private Label labelSettings;
    private CheckBox chkListLayouts;
    private CheckBox chkComicDisplayLayout;
    private CheckBox chkComicDisplaySettings;

    public SaveWorkspaceDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.RestorePosition();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
    }

    private void ValidateData(object sender, EventArgs e)
    {
      this.btOK.Enabled = (this.chkWindowLayouts.Checked || this.chkListLayouts.Checked || this.chkComicDisplayLayout.Checked || this.chkComicDisplaySettings.Checked) && !string.IsNullOrEmpty(this.txtName.Text);
    }

    public static bool Show(IWin32Window parent, DisplayWorkspace ws)
    {
      using (SaveWorkspaceDialog saveWorkspaceDialog = new SaveWorkspaceDialog())
      {
        saveWorkspaceDialog.txtName.Text = ws.Name;
        saveWorkspaceDialog.chkWindowLayouts.Checked = ws.IsWindowLayout;
        saveWorkspaceDialog.chkListLayouts.Checked = ws.IsViewsSetup;
        saveWorkspaceDialog.chkComicDisplayLayout.Checked = ws.IsComicPageLayout;
        saveWorkspaceDialog.chkComicDisplaySettings.Checked = ws.IsComicPageDisplay;
        if (saveWorkspaceDialog.ShowDialog(parent) != DialogResult.OK)
          return false;
        ws.Name = saveWorkspaceDialog.txtName.Text;
        ws.IsWindowLayout = saveWorkspaceDialog.chkWindowLayouts.Checked;
        ws.IsViewsSetup = saveWorkspaceDialog.chkListLayouts.Checked;
        ws.IsComicPageLayout = saveWorkspaceDialog.chkComicDisplayLayout.Checked;
        ws.IsComicPageDisplay = saveWorkspaceDialog.chkComicDisplaySettings.Checked;
        return true;
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
      this.txtName = new TextBox();
      this.lblName = new Label();
      this.btCancel = new Button();
      this.btOK = new Button();
      this.chkWindowLayouts = new CheckBox();
      this.labelSettings = new Label();
      this.chkListLayouts = new CheckBox();
      this.chkComicDisplayLayout = new CheckBox();
      this.chkComicDisplaySettings = new CheckBox();
      this.SuspendLayout();
      this.txtName.Location = new System.Drawing.Point(73, 30);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(260, 20);
      this.txtName.TabIndex = 1;
      this.txtName.TextChanged += new EventHandler(this.ValidateData);
      this.lblName.AutoSize = true;
      this.lblName.Location = new System.Drawing.Point(29, 33);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(38, 13);
      this.lblName.TabIndex = 0;
      this.lblName.Text = "Name:";
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(270, 163);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 8;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(184, 163);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 7;
      this.btOK.Text = "&OK";
      this.chkWindowLayouts.AutoSize = true;
      this.chkWindowLayouts.Location = new System.Drawing.Point(32, 97);
      this.chkWindowLayouts.Name = "chkWindowLayouts";
      this.chkWindowLayouts.Size = new System.Drawing.Size(105, 17);
      this.chkWindowLayouts.TabIndex = 3;
      this.chkWindowLayouts.Text = "Window Layouts";
      this.chkWindowLayouts.UseVisualStyleBackColor = true;
      this.chkWindowLayouts.CheckedChanged += new EventHandler(this.ValidateData);
      this.labelSettings.AutoSize = true;
      this.labelSettings.Location = new System.Drawing.Point(29, 71);
      this.labelSettings.Name = "labelSettings";
      this.labelSettings.Size = new System.Drawing.Size(181, 13);
      this.labelSettings.TabIndex = 2;
      this.labelSettings.Text = "This workspace includes settings for:";
      this.chkListLayouts.AutoSize = true;
      this.chkListLayouts.Location = new System.Drawing.Point(32, 120);
      this.chkListLayouts.Name = "chkListLayouts";
      this.chkListLayouts.Size = new System.Drawing.Size(82, 17);
      this.chkListLayouts.TabIndex = 4;
      this.chkListLayouts.Text = "List Layouts";
      this.chkListLayouts.UseVisualStyleBackColor = true;
      this.chkListLayouts.CheckedChanged += new EventHandler(this.ValidateData);
      this.chkComicDisplayLayout.AutoSize = true;
      this.chkComicDisplayLayout.Location = new System.Drawing.Point(167, 97);
      this.chkComicDisplayLayout.Name = "chkComicDisplayLayout";
      this.chkComicDisplayLayout.Size = new System.Drawing.Size((int) sbyte.MaxValue, 17);
      this.chkComicDisplayLayout.TabIndex = 5;
      this.chkComicDisplayLayout.Text = "Book Display Layout";
      this.chkComicDisplayLayout.UseVisualStyleBackColor = true;
      this.chkComicDisplayLayout.CheckedChanged += new EventHandler(this.ValidateData);
      this.chkComicDisplaySettings.AutoSize = true;
      this.chkComicDisplaySettings.Location = new System.Drawing.Point(167, 120);
      this.chkComicDisplaySettings.Name = "chkComicDisplaySettings";
      this.chkComicDisplaySettings.Size = new System.Drawing.Size(133, 17);
      this.chkComicDisplaySettings.TabIndex = 6;
      this.chkComicDisplaySettings.Text = "Book Display Settings";
      this.chkComicDisplaySettings.UseVisualStyleBackColor = true;
      this.chkComicDisplaySettings.CheckedChanged += new EventHandler(this.ValidateData);
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(362, 199);
      this.Controls.Add((Control) this.chkComicDisplaySettings);
      this.Controls.Add((Control) this.chkComicDisplayLayout);
      this.Controls.Add((Control) this.chkListLayouts);
      this.Controls.Add((Control) this.labelSettings);
      this.Controls.Add((Control) this.chkWindowLayouts);
      this.Controls.Add((Control) this.txtName);
      this.Controls.Add((Control) this.lblName);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (SaveWorkspaceDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Save Workspace";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

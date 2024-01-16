// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.EditListDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class EditListDialog : Form
  {
    private IContainer components;
    private Label labelName;
    private TextBox txtName;
    private ComboBox cbCombineMode;
    private Label labelBooks;
    private TextBox txtNotes;
    private Label labelNotes;
    private CheckBox chkShowNotes;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel panel1;
    private Panel panelBooks;
    private Panel panelNotes;
    private Panel bottomPanel;
    private Button btOK;
    private Button btCancel;
    private CheckBox chkQuickOpen;

    public EditListDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.chkShowNotes.Image = (Image) ((Bitmap) this.chkShowNotes.Image).ScaleDpi();
      this.RestorePosition();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.cbCombineMode.Items.AddRange((object[]) TR.Load(this.Name).GetStrings("CombineMode", "All Books from every list|Only Books existing in every list|Empty list", '|'));
    }

    private void chkShowNotes_CheckedChanged(object sender, EventArgs e)
    {
      this.panelNotes.Visible = this.chkShowNotes.Checked;
    }

    public static bool Edit(IWin32Window parent, ComicListItem item)
    {
      using (EditListDialog editListDialog = new EditListDialog())
      {
        ComicListItemFolder comicListItemFolder = item as ComicListItemFolder;
        ShareableComicListItem shareableComicListItem = item as ShareableComicListItem;
        editListDialog.txtName.Text = item.Name;
        editListDialog.txtNotes.Text = StringUtility.MakeEditBoxMultiline(item.Description);
        if (shareableComicListItem != null)
        {
          editListDialog.chkQuickOpen.Visible = true;
          editListDialog.chkQuickOpen.Checked = shareableComicListItem.QuickOpen;
        }
        else
          editListDialog.chkQuickOpen.Visible = false;
        if (comicListItemFolder != null)
        {
          editListDialog.panelBooks.Visible = true;
          editListDialog.cbCombineMode.SelectedIndex = (int) comicListItemFolder.CombineMode;
        }
        else
          editListDialog.panelBooks.Visible = false;
        editListDialog.chkShowNotes.Checked = editListDialog.panelNotes.Visible = !string.IsNullOrEmpty(editListDialog.txtNotes.Text) || editListDialog.chkQuickOpen.Checked;
        if (editListDialog.ShowDialog(parent) == DialogResult.Cancel)
          return false;
        item.Name = editListDialog.txtName.Text.Trim();
        item.Description = editListDialog.txtNotes.Text.Trim();
        if (shareableComicListItem != null)
          shareableComicListItem.QuickOpen = editListDialog.chkQuickOpen.Checked;
        if (comicListItemFolder != null)
          comicListItemFolder.CombineMode = (ComicFolderCombineMode) editListDialog.cbCombineMode.SelectedIndex;
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
      this.labelName = new Label();
      this.txtName = new TextBox();
      this.cbCombineMode = new ComboBox();
      this.labelBooks = new Label();
      this.txtNotes = new TextBox();
      this.labelNotes = new Label();
      this.chkShowNotes = new CheckBox();
      this.flowLayoutPanel1 = new FlowLayoutPanel();
      this.panel1 = new Panel();
      this.panelBooks = new Panel();
      this.panelNotes = new Panel();
      this.chkQuickOpen = new CheckBox();
      this.bottomPanel = new Panel();
      this.btOK = new Button();
      this.btCancel = new Button();
      this.flowLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panelBooks.SuspendLayout();
      this.panelNotes.SuspendLayout();
      this.bottomPanel.SuspendLayout();
      this.SuspendLayout();
      this.labelName.Location = new System.Drawing.Point(0, 10);
      this.labelName.Name = "labelName";
      this.labelName.Size = new System.Drawing.Size(62, 13);
      this.labelName.TabIndex = 0;
      this.labelName.Text = "Name:";
      this.labelName.TextAlign = ContentAlignment.TopRight;
      this.txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txtName.Location = new System.Drawing.Point(68, 7);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(526, 20);
      this.txtName.TabIndex = 1;
      this.cbCombineMode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbCombineMode.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbCombineMode.FormattingEnabled = true;
      this.cbCombineMode.Location = new System.Drawing.Point(68, 3);
      this.cbCombineMode.Name = "cbCombineMode";
      this.cbCombineMode.Size = new System.Drawing.Size(554, 21);
      this.cbCombineMode.TabIndex = 1;
      this.labelBooks.Location = new System.Drawing.Point(0, 9);
      this.labelBooks.Name = "labelBooks";
      this.labelBooks.Size = new System.Drawing.Size(62, 13);
      this.labelBooks.TabIndex = 0;
      this.labelBooks.Text = "Books:";
      this.labelBooks.TextAlign = ContentAlignment.TopRight;
      this.txtNotes.AcceptsReturn = true;
      this.txtNotes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txtNotes.Location = new System.Drawing.Point(68, 0);
      this.txtNotes.Multiline = true;
      this.txtNotes.Name = "txtNotes";
      this.txtNotes.ScrollBars = ScrollBars.Vertical;
      this.txtNotes.Size = new System.Drawing.Size(554, 101);
      this.txtNotes.TabIndex = 1;
      this.labelNotes.Location = new System.Drawing.Point(0, 0);
      this.labelNotes.Name = "labelNotes";
      this.labelNotes.Size = new System.Drawing.Size(62, 13);
      this.labelNotes.TabIndex = 0;
      this.labelNotes.Text = "Notes:";
      this.labelNotes.TextAlign = ContentAlignment.TopRight;
      this.chkShowNotes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkShowNotes.Appearance = Appearance.Button;
      this.chkShowNotes.Image = (Image) Resources.DoubleArrow;
      this.chkShowNotes.Location = new System.Drawing.Point(600, 6);
      this.chkShowNotes.Name = "chkShowNotes";
      this.chkShowNotes.Size = new System.Drawing.Size(22, 22);
      this.chkShowNotes.TabIndex = 2;
      this.chkShowNotes.UseVisualStyleBackColor = true;
      this.chkShowNotes.CheckedChanged += new EventHandler(this.chkShowNotes_CheckedChanged);
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add((Control) this.panel1);
      this.flowLayoutPanel1.Controls.Add((Control) this.panelBooks);
      this.flowLayoutPanel1.Controls.Add((Control) this.panelNotes);
      this.flowLayoutPanel1.Controls.Add((Control) this.bottomPanel);
      this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(625, 217);
      this.flowLayoutPanel1.TabIndex = 9;
      this.panel1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.panel1.AutoSize = true;
      this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add((Control) this.labelName);
      this.panel1.Controls.Add((Control) this.chkShowNotes);
      this.panel1.Controls.Add((Control) this.txtName);
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Margin = new Padding(0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(625, 31);
      this.panel1.TabIndex = 0;
      this.panelBooks.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.panelBooks.Controls.Add((Control) this.labelBooks);
      this.panelBooks.Controls.Add((Control) this.cbCombineMode);
      this.panelBooks.Location = new System.Drawing.Point(0, 31);
      this.panelBooks.Margin = new Padding(0);
      this.panelBooks.Name = "panelBooks";
      this.panelBooks.Size = new System.Drawing.Size(625, 30);
      this.panelBooks.TabIndex = 1;
      this.panelNotes.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.panelNotes.AutoSize = true;
      this.panelNotes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panelNotes.Controls.Add((Control) this.chkQuickOpen);
      this.panelNotes.Controls.Add((Control) this.labelNotes);
      this.panelNotes.Controls.Add((Control) this.txtNotes);
      this.panelNotes.Location = new System.Drawing.Point(0, 61);
      this.panelNotes.Margin = new Padding(0);
      this.panelNotes.Name = "panelNotes";
      this.panelNotes.Size = new System.Drawing.Size(625, (int) sbyte.MaxValue);
      this.panelNotes.TabIndex = 2;
      this.chkQuickOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkQuickOpen.Location = new System.Drawing.Point(68, 107);
      this.chkQuickOpen.Name = "chkQuickOpen";
      this.chkQuickOpen.Size = new System.Drawing.Size(129, 17);
      this.chkQuickOpen.TabIndex = 2;
      this.chkQuickOpen.Text = "Show in Quick Open";
      this.chkQuickOpen.UseVisualStyleBackColor = true;
      this.chkQuickOpen.Visible = false;
      this.bottomPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.bottomPanel.Controls.Add((Control) this.btOK);
      this.bottomPanel.Controls.Add((Control) this.btCancel);
      this.bottomPanel.Location = new System.Drawing.Point(0, 188);
      this.bottomPanel.Margin = new Padding(0);
      this.bottomPanel.Name = "bottomPanel";
      this.bottomPanel.Size = new System.Drawing.Size(625, 29);
      this.bottomPanel.TabIndex = 2;
      this.btOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(456, 3);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 0;
      this.btOK.Text = "&OK";
      this.btCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(542, 3);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 1;
      this.btCancel.Text = "&Cancel";
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(631, 223);
      this.Controls.Add((Control) this.flowLayoutPanel1);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (EditListDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Edit List";
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panelBooks.ResumeLayout(false);
      this.panelNotes.ResumeLayout(false);
      this.panelNotes.PerformLayout();
      this.bottomPanel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

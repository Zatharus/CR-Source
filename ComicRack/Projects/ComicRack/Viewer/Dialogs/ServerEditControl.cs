// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ServerEditControl
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.ComponentModel;
using cYo.Common.Localize;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO.Network;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ServerEditControl : UserControl
  {
    private IContainer components;
    private CheckBox chkClientsCanExport;
    private CheckBox chkClientsCanEdit;
    private PasswordTextBox txPassword;
    private CheckBox chkRequirePassword;
    private TextBox txSharedName;
    private Label labelSharedName;
    private TextBox txPublicServerComment;
    private Label labelShareDescription;
    private CheckBox chkShareInternet;
    private CheckBox chkPrivate;
    private TrackBarLite tbPageQuality;
    private Label labelPageQuality;
    private Label labelThumbQuality;
    private TrackBarLite tbThumbQuality;
    private ToolTip toolTip;
    private TreeViewEx tvSharedLists;
    private ComboBox cbShare;
    private ImageList ilShares;
    private GroupBox grpShareSettings;

    public ServerEditControl()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.InitializeComponent();
      TR tr = TR.Load("PreferencesDialog");
      LocalizeUtility.Localize(tr, (Control) this);
      LocalizeUtility.Localize(tr, this.cbShare);
      this.ilShares.Images.Add("Library", (Image) Resources.Library);
      this.ilShares.Images.Add("Folder", (Image) Resources.SearchFolder);
      this.ilShares.Images.Add("Search", (Image) Resources.SearchDocument);
      this.ilShares.Images.Add("List", (Image) Resources.List);
      this.ilShares.Images.Add("TempFolder", (Image) Resources.TempFolder);
      this.FillListTree(this.tvSharedLists.Nodes, (IEnumerable<ComicListItem>) Program.Database.ComicLists);
      IdleProcess.Idle += new EventHandler(this.IdleProcess_Idle);
      new LibraryTreeSkin().TreeView = (TreeView) this.tvSharedLists;
    }

    private void FillListTree(TreeNodeCollection tnc, IEnumerable<ComicListItem> clic)
    {
      foreach (ComicListItem comicListItem in clic)
      {
        TreeNode treeNode = tnc.Add(comicListItem.Name);
        treeNode.Tag = (object) comicListItem;
        treeNode.ImageKey = treeNode.SelectedImageKey = comicListItem.ImageKey;
        if (comicListItem is ComicListItemFolder)
        {
          this.FillListTree(treeNode.Nodes, (IEnumerable<ComicListItem>) ((ComicListItemFolder) comicListItem).Items);
          treeNode.ExpandAll();
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        IdleProcess.Idle -= new EventHandler(this.IdleProcess_Idle);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public ComicLibraryServerConfig Config
    {
      get
      {
        LibraryShareMode selectedIndex = (LibraryShareMode) this.cbShare.SelectedIndex;
        ComicLibraryServerConfig config = new ComicLibraryServerConfig()
        {
          Name = this.txSharedName.Text,
          LibraryShareMode = selectedIndex,
          IsProtected = this.chkRequirePassword.Checked,
          IsEditable = this.chkClientsCanEdit.Checked,
          IsExportable = this.chkClientsCanExport.Checked,
          IsInternet = this.chkShareInternet.Checked,
          IsPrivate = this.chkPrivate.Checked,
          Description = this.txPublicServerComment.Text,
          Password = this.txPassword.Password,
          ThumbnailQuality = this.tbThumbQuality.Value,
          PageQuality = this.tbPageQuality.Value
        };
        config.SharedItems.AddRange(this.tvSharedLists.AllNodes().Where<TreeNode>((Func<TreeNode, bool>) (tn => tn.Checked)).Select<TreeNode, ComicListItem>((Func<TreeNode, ComicListItem>) (tn => (ComicListItem) tn.Tag)).OfType<ShareableComicListItem>().Select<ShareableComicListItem, Guid>((Func<ShareableComicListItem, Guid>) (clic => clic.Id)));
        return config;
      }
      set
      {
        ComicLibraryServerConfig libraryServerConfig = value;
        this.txSharedName.Text = libraryServerConfig.Name;
        this.chkClientsCanEdit.Checked = libraryServerConfig.IsEditable;
        this.chkClientsCanExport.Checked = libraryServerConfig.IsExportable;
        this.txPassword.Password = libraryServerConfig.Password;
        this.chkRequirePassword.Checked = libraryServerConfig.IsProtected;
        this.cbShare.SelectedIndex = (int) libraryServerConfig.LibraryShareMode;
        this.chkShareInternet.Checked = libraryServerConfig.IsInternet;
        this.chkPrivate.Checked = libraryServerConfig.IsPrivate;
        this.txPublicServerComment.Text = libraryServerConfig.Description;
        foreach (TreeNode allNode in this.tvSharedLists.AllNodes())
          allNode.Checked = libraryServerConfig.SharedItems.Contains(((IdComponent) allNode.Tag).Id);
        this.tbThumbQuality.Value = libraryServerConfig.ThumbnailQuality;
        this.tbPageQuality.Value = libraryServerConfig.PageQuality;
      }
    }

    public string ShareName => this.txSharedName.Text;

    public event EventHandler ShareNameChanged;

    private void IdleProcess_Idle(object sender, EventArgs e)
    {
      this.tvSharedLists.Enabled = this.cbShare.SelectedIndex == 2;
      this.txPassword.Enabled = this.chkRequirePassword.Checked;
      this.chkPrivate.Enabled = this.chkShareInternet.Checked;
    }

    private void txSharedName_TextChanged(object sender, EventArgs e)
    {
      if (this.ShareNameChanged == null)
        return;
      this.ShareNameChanged((object) this, e);
    }

    private void tbPageQuality_ValueChanged(object sender, EventArgs e)
    {
      TrackBarLite trackBarLite = sender as TrackBarLite;
      this.toolTip.SetToolTip((Control) trackBarLite, trackBarLite.Value.ToString());
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.chkClientsCanExport = new CheckBox();
      this.chkClientsCanEdit = new CheckBox();
      this.txPassword = new PasswordTextBox();
      this.chkRequirePassword = new CheckBox();
      this.txSharedName = new TextBox();
      this.labelSharedName = new Label();
      this.txPublicServerComment = new TextBox();
      this.labelShareDescription = new Label();
      this.chkShareInternet = new CheckBox();
      this.chkPrivate = new CheckBox();
      this.tbPageQuality = new TrackBarLite();
      this.labelPageQuality = new Label();
      this.labelThumbQuality = new Label();
      this.tbThumbQuality = new TrackBarLite();
      this.toolTip = new ToolTip(this.components);
      this.tvSharedLists = new TreeViewEx();
      this.ilShares = new ImageList(this.components);
      this.cbShare = new ComboBox();
      this.grpShareSettings = new GroupBox();
      this.grpShareSettings.SuspendLayout();
      this.SuspendLayout();
      this.chkClientsCanExport.AutoSize = true;
      this.chkClientsCanExport.Location = new System.Drawing.Point(5, 138);
      this.chkClientsCanExport.Name = "chkClientsCanExport";
      this.chkClientsCanExport.Size = new System.Drawing.Size(143, 17);
      this.chkClientsCanExport.TabIndex = 11;
      this.chkClientsCanExport.Text = "Clients can export Books";
      this.chkClientsCanExport.UseVisualStyleBackColor = true;
      this.chkClientsCanEdit.AutoSize = true;
      this.chkClientsCanEdit.Location = new System.Drawing.Point(5, 120);
      this.chkClientsCanEdit.Name = "chkClientsCanEdit";
      this.chkClientsCanEdit.Size = new System.Drawing.Size(131, 17);
      this.chkClientsCanEdit.TabIndex = 10;
      this.chkClientsCanEdit.Text = "Clients can edit Books";
      this.chkClientsCanEdit.UseVisualStyleBackColor = true;
      this.txPassword.Location = new System.Drawing.Point(24, 45);
      this.txPassword.Name = "txPassword";
      this.txPassword.Password = (string) null;
      this.txPassword.Size = new System.Drawing.Size(153, 20);
      this.txPassword.TabIndex = 7;
      this.txPassword.UseSystemPasswordChar = true;
      this.chkRequirePassword.AutoSize = true;
      this.chkRequirePassword.Location = new System.Drawing.Point(6, 23);
      this.chkRequirePassword.Name = "chkRequirePassword";
      this.chkRequirePassword.Size = new System.Drawing.Size(115, 17);
      this.chkRequirePassword.TabIndex = 6;
      this.chkRequirePassword.Text = "Require Password:";
      this.chkRequirePassword.UseVisualStyleBackColor = true;
      this.txSharedName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txSharedName.Location = new System.Drawing.Point(106, 7);
      this.txSharedName.Name = "txSharedName";
      this.txSharedName.Size = new System.Drawing.Size(280, 20);
      this.txSharedName.TabIndex = 1;
      this.txSharedName.TextChanged += new EventHandler(this.txSharedName_TextChanged);
      this.labelSharedName.AutoSize = true;
      this.labelSharedName.Location = new System.Drawing.Point(3, 10);
      this.labelSharedName.Name = "labelSharedName";
      this.labelSharedName.Size = new System.Drawing.Size(75, 13);
      this.labelSharedName.TabIndex = 0;
      this.labelSharedName.Text = "Shared Name:";
      this.txPublicServerComment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txPublicServerComment.Location = new System.Drawing.Point(106, 29);
      this.txPublicServerComment.Name = "txPublicServerComment";
      this.txPublicServerComment.Size = new System.Drawing.Size(280, 20);
      this.txPublicServerComment.TabIndex = 3;
      this.labelShareDescription.AutoSize = true;
      this.labelShareDescription.Location = new System.Drawing.Point(3, 32);
      this.labelShareDescription.Name = "labelShareDescription";
      this.labelShareDescription.Size = new System.Drawing.Size(63, 13);
      this.labelShareDescription.TabIndex = 2;
      this.labelShareDescription.Text = "Description:";
      this.chkShareInternet.AutoSize = true;
      this.chkShareInternet.Location = new System.Drawing.Point(6, 78);
      this.chkShareInternet.Name = "chkShareInternet";
      this.chkShareInternet.Size = new System.Drawing.Size(126, 17);
      this.chkShareInternet.TabIndex = 8;
      this.chkShareInternet.Text = "Share on the Internet";
      this.chkShareInternet.UseVisualStyleBackColor = true;
      this.chkPrivate.AutoSize = true;
      this.chkPrivate.Location = new System.Drawing.Point(25, 97);
      this.chkPrivate.Name = "chkPrivate";
      this.chkPrivate.Size = new System.Drawing.Size(59, 17);
      this.chkPrivate.TabIndex = 9;
      this.chkPrivate.Text = "Private";
      this.chkPrivate.UseVisualStyleBackColor = true;
      this.tbPageQuality.Location = new System.Drawing.Point(82, 175);
      this.tbPageQuality.Minimum = 15;
      this.tbPageQuality.Name = "tbPageQuality";
      this.tbPageQuality.Size = new System.Drawing.Size(95, 19);
      this.tbPageQuality.TabIndex = 13;
      this.tbPageQuality.ThumbSize = new System.Drawing.Size(8, 12);
      this.tbPageQuality.Value = 15;
      this.tbPageQuality.ValueChanged += new EventHandler(this.tbPageQuality_ValueChanged);
      this.labelPageQuality.AutoSize = true;
      this.labelPageQuality.Location = new System.Drawing.Point(3, 175);
      this.labelPageQuality.Name = "labelPageQuality";
      this.labelPageQuality.Size = new System.Drawing.Size(70, 13);
      this.labelPageQuality.TabIndex = 12;
      this.labelPageQuality.Text = "Page Quality:";
      this.labelThumbQuality.AutoSize = true;
      this.labelThumbQuality.Location = new System.Drawing.Point(3, 197);
      this.labelThumbQuality.Name = "labelThumbQuality";
      this.labelThumbQuality.Size = new System.Drawing.Size(78, 13);
      this.labelThumbQuality.TabIndex = 14;
      this.labelThumbQuality.Text = "Thumb Quality:";
      this.tbThumbQuality.Location = new System.Drawing.Point(82, 197);
      this.tbThumbQuality.Minimum = 15;
      this.tbThumbQuality.Name = "tbThumbQuality";
      this.tbThumbQuality.Size = new System.Drawing.Size(95, 19);
      this.tbThumbQuality.TabIndex = 15;
      this.tbThumbQuality.ThumbSize = new System.Drawing.Size(8, 12);
      this.tbThumbQuality.Value = 15;
      this.tbThumbQuality.ValueChanged += new EventHandler(this.tbPageQuality_ValueChanged);
      this.tvSharedLists.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.tvSharedLists.CheckBoxes = true;
      this.tvSharedLists.ImageIndex = 0;
      this.tvSharedLists.ImageList = this.ilShares;
      this.tvSharedLists.ItemHeight = 16;
      this.tvSharedLists.Location = new System.Drawing.Point(6, 90);
      this.tvSharedLists.Name = "tvSharedLists";
      this.tvSharedLists.SelectedImageIndex = 0;
      this.tvSharedLists.ShowLines = false;
      this.tvSharedLists.ShowPlusMinus = false;
      this.tvSharedLists.ShowRootLines = false;
      this.tvSharedLists.Size = new System.Drawing.Size(191, 240);
      this.tvSharedLists.TabIndex = 5;
      this.ilShares.ColorDepth = ColorDepth.Depth8Bit;
      this.ilShares.ImageSize = new System.Drawing.Size(16, 16);
      this.ilShares.TransparentColor = Color.Transparent;
      this.cbShare.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.cbShare.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbShare.FormattingEnabled = true;
      this.cbShare.Items.AddRange(new object[3]
      {
        (object) "Share None",
        (object) "Share All",
        (object) "Share Selected"
      });
      this.cbShare.Location = new System.Drawing.Point(6, 67);
      this.cbShare.Name = "cbShare";
      this.cbShare.Size = new System.Drawing.Size(191, 21);
      this.cbShare.TabIndex = 4;
      this.grpShareSettings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
      this.grpShareSettings.Controls.Add((Control) this.chkRequirePassword);
      this.grpShareSettings.Controls.Add((Control) this.txPassword);
      this.grpShareSettings.Controls.Add((Control) this.chkClientsCanEdit);
      this.grpShareSettings.Controls.Add((Control) this.labelThumbQuality);
      this.grpShareSettings.Controls.Add((Control) this.chkClientsCanExport);
      this.grpShareSettings.Controls.Add((Control) this.tbThumbQuality);
      this.grpShareSettings.Controls.Add((Control) this.chkShareInternet);
      this.grpShareSettings.Controls.Add((Control) this.labelPageQuality);
      this.grpShareSettings.Controls.Add((Control) this.chkPrivate);
      this.grpShareSettings.Controls.Add((Control) this.tbPageQuality);
      this.grpShareSettings.Location = new System.Drawing.Point(203, 67);
      this.grpShareSettings.Name = "grpShareSettings";
      this.grpShareSettings.Size = new System.Drawing.Size(183, 263);
      this.grpShareSettings.TabIndex = 16;
      this.grpShareSettings.TabStop = false;
      this.grpShareSettings.Text = "Settings";
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.grpShareSettings);
      this.Controls.Add((Control) this.cbShare);
      this.Controls.Add((Control) this.tvSharedLists);
      this.Controls.Add((Control) this.txPublicServerComment);
      this.Controls.Add((Control) this.labelShareDescription);
      this.Controls.Add((Control) this.txSharedName);
      this.Controls.Add((Control) this.labelSharedName);
      this.Name = nameof (ServerEditControl);
      this.Size = new System.Drawing.Size(389, 333);
      this.grpShareSettings.ResumeLayout(false);
      this.grpShareSettings.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

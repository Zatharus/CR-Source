// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.OpenRemoteDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.IO.Network;
using cYo.Projects.ComicRack.Viewer.Config;
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
  public class OpenRemoteDialog : Form
  {
    private bool showPublic;
    private List<ShareInformation> servers;
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private Label labelServerAddress;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel panel1;
    private ComboBox cbServer;
    private ListView lvServers;
    private Panel panel2;
    private Button btPublic;
    private ColumnHeader colName;
    private ColumnHeader colDescription;
    private Panel panelList;
    private TextBox txFilter;
    private ImageList imageList;
    private Label labelFailedServerList;
    private PasswordTextBox txPassword;
    private Label labelListPassword;
    private ColumnHeader colEdit;
    private ColumnHeader colExport;
    private PictureBox pictureBox1;

    public OpenRemoteDialog()
    {
      this.InitializeComponent();
      this.imageList.Images.Add((Image) Resources.RemoteDatabase);
      this.imageList.Images.Add((Image) Resources.RemoteDatabaseLocked);
      this.lvServers.Columns.ScaleDpi();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.RestorePosition();
      this.txFilter.Text = Program.Settings.OpenRemoteFilter;
      this.txPassword.Password = Program.Settings.OpenRemotePassword;
    }

    private void lvServers_SelectedIndexChanged(object sender, EventArgs e) => this.UpdateServer();

    private void lvServers_ItemActivate(object sender, EventArgs e)
    {
      this.UpdateServer();
      if (string.IsNullOrEmpty(this.cbServer.Text.Trim()))
        return;
      this.DialogResult = DialogResult.OK;
      this.Hide();
    }

    private void btPublic_Click(object sender, EventArgs e)
    {
      if (!this.ShowPublic)
      {
        this.ShowPublic = true;
      }
      else
      {
        this.servers = (List<ShareInformation>) null;
        this.FillServers();
      }
    }

    private void txFilter_TextChanged(object sender, EventArgs e)
    {
      if (!this.ShowPublic)
        this.ShowPublic = true;
      else
        this.FillServers();
    }

    private void cbServer_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.cbServer.SelectedIndex == -1)
        this.CurrentItem = (RemoteShareItem) null;
      else
        this.CurrentItem = this.cbServer.SelectedItem as RemoteShareItem;
    }

    private void cbServer_TextUpdate(object sender, EventArgs e)
    {
      this.CurrentItem = this.cbServer.Items.OfType<RemoteShareItem>().FirstOrDefault<RemoteShareItem>((Func<RemoteShareItem, bool>) (n => n.Name == this.cbServer.Text)) ?? new RemoteShareItem(this.cbServer.Text);
    }

    public bool ShowPublic
    {
      get => this.showPublic;
      set
      {
        this.showPublic = value;
        if (this.showPublic)
          this.FillServers();
        this.panelList.Visible = this.showPublic;
        this.btPublic.Text = this.showPublic ? TR.Load(this.Name)["RefreshList", "Refresh List"] : TR.Load(this.Name)["ShowList", "Show List"];
      }
    }

    private RemoteShareItem CurrentItem { get; set; }

    private void UpdateServer()
    {
      if (this.lvServers.SelectedItems.Count == 0)
        return;
      ListViewItem selectedItem = this.lvServers.SelectedItems[0];
      if (selectedItem.Tag == null)
      {
        this.cbServer.Text = string.Empty;
        this.CurrentItem = (RemoteShareItem) null;
      }
      else
      {
        this.CurrentItem = new RemoteShareItem(selectedItem.Tag as ShareInformation);
        if (!this.cbServer.Items.Contains((object) this.CurrentItem))
          this.cbServer.Items.Insert(0, (object) this.CurrentItem);
        this.cbServer.SelectedItem = (object) this.CurrentItem;
      }
    }

    private void FillServers()
    {
      if (!this.showPublic)
        return;
      string filter = this.txFilter.Text.Trim();
      string password = this.txPassword.Password;
      if (this.servers == null)
      {
        using (new WaitCursor((Form) this))
        {
          this.labelFailedServerList.Visible = false;
          this.servers = new List<ShareInformation>();
          using (ItemMonitor.Lock((object) Program.NetworkManager.LocalShares))
            this.servers.AddRange((IEnumerable<ShareInformation>) Program.NetworkManager.LocalShares.Values);
          try
          {
            this.servers.AddRange((IEnumerable<ShareInformation>) ComicLibraryServer.GetPublicServers(ServerOptions.None, password).OrderBy<ShareInformation, string>((Func<ShareInformation, string>) (s => s.Name)).ToArray<ShareInformation>());
          }
          catch
          {
            this.labelFailedServerList.Visible = this.servers.Count == 0;
          }
        }
      }
      this.lvServers.Items.Clear();
      foreach (ShareInformation shareInformation in this.servers.Where<ShareInformation>((Func<ShareInformation, bool>) (s => string.IsNullOrEmpty(filter) || s.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) || s.Comment.Contains(filter, StringComparison.OrdinalIgnoreCase))))
      {
        ListViewItem listViewItem = this.lvServers.Items.Add(shareInformation.Name, shareInformation.IsProtected ? 1 : 0);
        listViewItem.SubItems.Add(shareInformation.Comment);
        listViewItem.SubItems.Add(shareInformation.IsEditable ? TR.Default["Yes"] : TR.Default["No"]);
        listViewItem.SubItems.Add(shareInformation.IsExportable ? TR.Default["Yes"] : TR.Default["No"]);
        listViewItem.Group = this.lvServers.Groups[shareInformation.IsLocal ? "groupLocal" : "groupInternet"];
        listViewItem.Tag = (object) shareInformation;
        if (Program.NetworkManager.IsOwnServer(shareInformation.Uri))
        {
          listViewItem.ForeColor = SystemColors.GrayText;
          listViewItem.Tag = (object) null;
        }
      }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      Program.Settings.OpenRemoteFilter = this.txFilter.Text;
      Program.Settings.OpenRemotePassword = this.txPassword.Password;
      base.OnClosing(e);
    }

    public static RemoteShareItem GetShare(
      IWin32Window parent,
      RemoteShareItem share,
      IEnumerable<RemoteShareItem> list,
      bool showPublic)
    {
      using (OpenRemoteDialog openRemoteDialog = new OpenRemoteDialog())
      {
        openRemoteDialog.cbServer.Items.AddRange((object[]) list.ToArray<RemoteShareItem>());
        if (share != null)
        {
          openRemoteDialog.cbServer.Text = share.Name;
          openRemoteDialog.CurrentItem = share;
        }
        openRemoteDialog.ShowPublic = showPublic;
        return openRemoteDialog.ShowDialog(parent) == DialogResult.OK ? openRemoteDialog.CurrentItem : (RemoteShareItem) null;
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
      this.components = (IContainer) new System.ComponentModel.Container();
      ListViewGroup listViewGroup1 = new ListViewGroup("Local Shares", HorizontalAlignment.Left);
      ListViewGroup listViewGroup2 = new ListViewGroup("Internet Shares", HorizontalAlignment.Left);
      this.btCancel = new Button();
      this.btOK = new Button();
      this.labelServerAddress = new Label();
      this.flowLayoutPanel1 = new FlowLayoutPanel();
      this.panel1 = new Panel();
      this.cbServer = new ComboBox();
      this.panelList = new Panel();
      this.txPassword = new PasswordTextBox();
      this.labelListPassword = new Label();
      this.labelFailedServerList = new Label();
      this.txFilter = new TextBox();
      this.lvServers = new ListView();
      this.colName = new ColumnHeader();
      this.colDescription = new ColumnHeader();
      this.colEdit = new ColumnHeader();
      this.colExport = new ColumnHeader();
      this.imageList = new ImageList(this.components);
      this.panel2 = new Panel();
      this.btPublic = new Button();
      this.pictureBox1 = new PictureBox();
      this.flowLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panelList.SuspendLayout();
      this.panel2.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(473, 3);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 2;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(387, 3);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 1;
      this.btOK.Text = "&OK";
      this.labelServerAddress.AutoSize = true;
      this.labelServerAddress.Location = new System.Drawing.Point(3, 6);
      this.labelServerAddress.Name = "labelServerAddress";
      this.labelServerAddress.Size = new System.Drawing.Size(82, 13);
      this.labelServerAddress.TabIndex = 0;
      this.labelServerAddress.Text = "Library Address:";
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add((Control) this.panel1);
      this.flowLayoutPanel1.Controls.Add((Control) this.panelList);
      this.flowLayoutPanel1.Controls.Add((Control) this.panel2);
      this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 7);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(553, 331);
      this.flowLayoutPanel1.TabIndex = 7;
      this.panel1.Controls.Add((Control) this.pictureBox1);
      this.panel1.Controls.Add((Control) this.cbServer);
      this.panel1.Controls.Add((Control) this.labelServerAddress);
      this.panel1.Controls.Add((Control) this.txFilter);
      this.panel1.Location = new System.Drawing.Point(0, 3);
      this.panel1.Margin = new Padding(0, 3, 0, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(553, 27);
      this.panel1.TabIndex = 0;
      this.cbServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbServer.FormattingEnabled = true;
      this.cbServer.Location = new System.Drawing.Point(96, 3);
      this.cbServer.Name = "cbServer";
      this.cbServer.Size = new System.Drawing.Size(304, 21);
      this.cbServer.TabIndex = 1;
      this.cbServer.SelectedIndexChanged += new EventHandler(this.cbServer_SelectedIndexChanged);
      this.cbServer.TextUpdate += new EventHandler(this.cbServer_TextUpdate);
      this.panelList.Controls.Add((Control) this.txPassword);
      this.panelList.Controls.Add((Control) this.labelListPassword);
      this.panelList.Controls.Add((Control) this.labelFailedServerList);
      this.panelList.Controls.Add((Control) this.lvServers);
      this.panelList.Location = new System.Drawing.Point(0, 36);
      this.panelList.Margin = new Padding(0, 3, 0, 3);
      this.panelList.Name = "panelList";
      this.panelList.Size = new System.Drawing.Size(553, 256);
      this.panelList.TabIndex = 8;
      this.txPassword.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.txPassword.Location = new System.Drawing.Point(423, 229);
      this.txPassword.Name = "txPassword";
      this.txPassword.Password = (string) null;
      this.txPassword.Size = new System.Drawing.Size(130, 20);
      this.txPassword.TabIndex = 3;
      this.txPassword.UseSystemPasswordChar = true;
      this.labelListPassword.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.labelListPassword.Location = new System.Drawing.Point(245, 232);
      this.labelListPassword.Name = "labelListPassword";
      this.labelListPassword.Size = new System.Drawing.Size(172, 13);
      this.labelListPassword.TabIndex = 2;
      this.labelListPassword.Text = "Private List Password:";
      this.labelListPassword.TextAlign = ContentAlignment.TopRight;
      this.labelFailedServerList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.labelFailedServerList.BackColor = SystemColors.Window;
      this.labelFailedServerList.ForeColor = SystemColors.GrayText;
      this.labelFailedServerList.Location = new System.Drawing.Point(184, 96);
      this.labelFailedServerList.Name = "labelFailedServerList";
      this.labelFailedServerList.Size = new System.Drawing.Size(186, 68);
      this.labelFailedServerList.TabIndex = 1;
      this.labelFailedServerList.Text = "Failed to get the Server List!";
      this.labelFailedServerList.TextAlign = ContentAlignment.MiddleCenter;
      this.labelFailedServerList.Visible = false;
      this.txFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.txFilter.Location = new System.Drawing.Point(444, 3);
      this.txFilter.Name = "txFilter";
      this.txFilter.Size = new System.Drawing.Size(109, 20);
      this.txFilter.TabIndex = 2;
      this.txFilter.TextChanged += new EventHandler(this.txFilter_TextChanged);
      this.lvServers.Columns.AddRange(new ColumnHeader[4]
      {
        this.colName,
        this.colDescription,
        this.colEdit,
        this.colExport
      });
      this.lvServers.FullRowSelect = true;
      listViewGroup1.Header = "Local Shares";
      listViewGroup1.Name = "groupLocal";
      listViewGroup2.Header = "Internet Shares";
      listViewGroup2.Name = "groupInternet";
      this.lvServers.Groups.AddRange(new ListViewGroup[2]
      {
        listViewGroup1,
        listViewGroup2
      });
      this.lvServers.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.lvServers.Location = new System.Drawing.Point(0, 3);
      this.lvServers.Name = "lvServers";
      this.lvServers.Size = new System.Drawing.Size(553, 224);
      this.lvServers.SmallImageList = this.imageList;
      this.lvServers.TabIndex = 0;
      this.lvServers.UseCompatibleStateImageBehavior = false;
      this.lvServers.View = View.Details;
      this.lvServers.ItemActivate += new EventHandler(this.lvServers_ItemActivate);
      this.lvServers.SelectedIndexChanged += new EventHandler(this.lvServers_SelectedIndexChanged);
      this.colName.Text = "Name";
      this.colName.Width = 119;
      this.colDescription.Text = "Description";
      this.colDescription.Width = 310;
      this.colEdit.Text = "Edit";
      this.colEdit.Width = 41;
      this.colExport.Text = "Export";
      this.colExport.Width = 49;
      this.imageList.ColorDepth = ColorDepth.Depth32Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = Color.Transparent;
      this.panel2.AutoSize = true;
      this.panel2.Controls.Add((Control) this.btPublic);
      this.panel2.Controls.Add((Control) this.btCancel);
      this.panel2.Controls.Add((Control) this.btOK);
      this.panel2.Location = new System.Drawing.Point(0, 298);
      this.panel2.Margin = new Padding(0, 3, 0, 3);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(553, 30);
      this.panel2.TabIndex = 2;
      this.btPublic.FlatStyle = FlatStyle.System;
      this.btPublic.Location = new System.Drawing.Point(0, 3);
      this.btPublic.Name = "btPublic";
      this.btPublic.Size = new System.Drawing.Size(117, 24);
      this.btPublic.TabIndex = 0;
      this.btPublic.Text = "Show List";
      this.btPublic.Click += new EventHandler(this.btPublic_Click);
      this.pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.pictureBox1.Image = (Image) Resources.Search;
      this.pictureBox1.Location = new System.Drawing.Point(422, 6);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(16, 16);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 6;
      this.pictureBox1.TabStop = false;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(570, 348);
      this.Controls.Add((Control) this.flowLayoutPanel1);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (OpenRemoteDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Open Remote Library";
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panelList.ResumeLayout(false);
      this.panelList.PerformLayout();
      this.panel2.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

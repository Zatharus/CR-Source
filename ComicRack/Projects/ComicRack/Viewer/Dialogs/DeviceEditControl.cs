// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.DeviceEditControl
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Sync;
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
  public class DeviceEditControl : UserControl
  {
    private ComicLibrary library;
    private string deviceName;
    private bool blockCheck;
    private bool blockListUpdate;
    private IContainer components;
    private TreeViewEx tvSharedLists;
    private CheckBox chkOptimizeSize;
    private ImageList ilShares;
    private ToolTip toolTip;
    private Button btSelectAll;
    private Button btSelectNone;
    private CheckBox chkOnlyUnread;
    private GroupBox grpListOptions;
    private CheckBox chkOnlyChecked;
    private Button btSelectList;
    private Button btDeselectList;
    private ComboBox cbLimitSort;
    private TextBox txLimit;
    private CheckBox chkLimit;
    private ComboBox cbLimitType;
    private CheckBox chkKeepLastRead;
    private CheckBox chkSortBy;
    private WrappingCheckBox chkOnlyShowSelected;
    private Panel panel1;

    public DeviceEditControl()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.InitializeComponent();
      TR tr = TR.Load("DevicesEditDialog");
      LocalizeUtility.Localize(tr, (Control) this);
      LocalizeUtility.Localize(tr, this.cbLimitType);
      LocalizeUtility.Localize(tr, this.cbLimitSort);
      this.ilShares.ImageSize = this.ilShares.ImageSize.ScaleDpi();
      this.ilShares.Images.Add("Library", (Image) Resources.Library);
      this.ilShares.Images.Add("Folder", (Image) Resources.SearchFolder);
      this.ilShares.Images.Add("Search", (Image) Resources.SearchDocument);
      this.ilShares.Images.Add("List", (Image) Resources.List);
      this.ilShares.Images.Add("TempFolder", (Image) Resources.TempFolder);
      LibraryTreeSkin libraryTreeSkin = new LibraryTreeSkin();
      libraryTreeSkin.TreeView = (TreeView) this.tvSharedLists;
      libraryTreeSkin.GetNodeItem = (Func<TreeNode, ComicListItem>) (n => ((DeviceEditControl.TagElement) n.Tag).Item);
      libraryTreeSkin.DisableDeviceIcon = true;
      this.txLimit.EnableOnlyNumberKeys();
      SpinButton.AddUpDown((TextBoxBase) this.txLimit, min: 1);
      this.library = (ComicLibrary) Program.Database;
      if (this.library == null)
        return;
      this.library.ComicListCachesUpdated += new EventHandler(this.Database_ComicListCachesUpdated);
      this.library.ComicListsChanged += new ComicListChangedEventHandler(this.library_ComicListsChanged);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.components.SafeDispose();
        if (this.library != null)
        {
          this.library.ComicListCachesUpdated -= new EventHandler(this.Database_ComicListCachesUpdated);
          this.library.ComicListsChanged -= new ComicListChangedEventHandler(this.library_ComicListsChanged);
        }
        this.library = (ComicLibrary) null;
      }
      base.Dispose(disposing);
    }

    private void Database_ComicListCachesUpdated(object sender, EventArgs e)
    {
      this.tvSharedLists.Invalidate();
    }

    private void library_ComicListsChanged(object sender, ComicListItemChangedEventArgs e)
    {
      this.tvSharedLists.Invalidate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DeviceSyncSettings Settings
    {
      get
      {
        DeviceSyncSettings settings = new DeviceSyncSettings()
        {
          DeviceName = this.DeviceName,
          DeviceKey = this.DeviceKey,
          DefaultListSettings = this.DefaultListSettings
        };
        settings.Lists.AddRange(this.tvSharedLists.AllNodes().Where<TreeNode>((Func<TreeNode, bool>) (tn => tn.Checked)).Select<TreeNode, DeviceEditControl.TagElement>((Func<TreeNode, DeviceEditControl.TagElement>) (tn => (DeviceEditControl.TagElement) tn.Tag)).Select<DeviceEditControl.TagElement, DeviceSyncSettings.SharedList>((Func<DeviceEditControl.TagElement, DeviceSyncSettings.SharedList>) (te => te.List ?? this.CreateDefaultSharedList(te.Item.Id))));
        return settings;
      }
      set
      {
        DeviceSyncSettings settings = value;
        this.DeviceName = settings.DeviceName;
        this.DeviceKey = settings.DeviceKey;
        this.DefaultListSettings = new DeviceSyncSettings.SharedListSettings(settings.DefaultListSettings);
        this.UpdateTree(settings);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DeviceName
    {
      get => this.deviceName;
      set
      {
        if (this.deviceName == value)
          return;
        this.deviceName = value;
        if (this.DeviceNameChanged == null)
          return;
        this.DeviceNameChanged((object) this, EventArgs.Empty);
      }
    }

    public event EventHandler DeviceNameChanged;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DeviceKey { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DeviceSyncSettings.SharedListSettings DefaultListSettings { get; set; }

    public bool CanPaste => Clipboard.ContainsData("DeviceSyncSettings");

    public void SelectList(Guid id)
    {
      this.tvSharedLists.SelectedNode = this.tvSharedLists.Nodes.Find((Predicate<TreeNode>) (tn => ((DeviceEditControl.TagElement) tn.Tag).Item.Id == id));
    }

    public void CopyShareSettings()
    {
      Clipboard.SetData("DeviceSyncSettings", (object) this.Settings);
    }

    public void PasteSharedSettings()
    {
      try
      {
        if (!(Clipboard.GetData("DeviceSyncSettings") is DeviceSyncSettings data))
          return;
        this.UpdateTree(data);
        this.SetEditor(this.tvSharedLists.SelectedNode);
        this.SetButtonStates();
      }
      catch
      {
      }
    }

    private void btSelectAll_Click(object sender, EventArgs e)
    {
      this.blockCheck = true;
      this.tvSharedLists.Nodes.All().ForEach<TreeNode>((Action<TreeNode>) (tn => tn.Checked = true));
      this.blockCheck = false;
    }

    private void btSelectNone_Click(object sender, EventArgs e)
    {
      this.blockCheck = true;
      this.tvSharedLists.Nodes.All().ForEach<TreeNode>((Action<TreeNode>) (tn => tn.Checked = false));
      this.blockCheck = false;
    }

    private void btSelectList_Click(object sender, EventArgs e)
    {
      if (this.tvSharedLists.SelectedNode == null)
        return;
      this.blockCheck = true;
      this.tvSharedLists.SelectedNode.Checked = true;
      this.tvSharedLists.SelectedNode.Nodes.All().ForEach<TreeNode>((Action<TreeNode>) (tn => tn.Checked = true));
      this.blockCheck = false;
    }

    private void btUnselectList_Click(object sender, EventArgs e)
    {
      if (this.tvSharedLists.SelectedNode == null)
        return;
      this.blockCheck = true;
      this.tvSharedLists.SelectedNode.Checked = false;
      this.tvSharedLists.SelectedNode.Nodes.All().ForEach<TreeNode>((Action<TreeNode>) (tn => tn.Checked = false));
      this.blockCheck = false;
    }

    private void tvSharedLists_AfterSelect(object sender, TreeViewEventArgs e)
    {
      this.SetEditor(e.Node);
      this.SetButtonStates();
    }

    private void tvSharedLists_AfterCheck(object sender, TreeViewEventArgs e)
    {
      if (!this.blockCheck)
        this.tvSharedLists.SelectedNode = e.Node;
      if (e.Node == this.tvSharedLists.SelectedNode)
        this.SetEditor(e.Node);
      this.SetButtonStates();
    }

    private void chkOnlyShowSelected_CheckedChanged(object sender, EventArgs e)
    {
      DeviceSyncSettings settings = this.Settings;
      ComicListItem cli = this.GetSelectedComicListItem();
      this.UpdateTree(settings, true);
      TreeNode node = cli == null ? (TreeNode) null : this.tvSharedLists.Nodes.Find((Predicate<TreeNode>) (n => DeviceEditControl.GetSharedList(n) != null && DeviceEditControl.GetComicListItem(n).Id == cli.Id));
      this.tvSharedLists.SelectedNode = node;
      this.SetEditor(node);
    }

    private void chkOptimizeSize_CheckedChanged(object sender, EventArgs e)
    {
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.OptimizePortable = this.chkOptimizeSize.Checked));
    }

    private void chkOnlyUnread_CheckedChanged(object sender, EventArgs e)
    {
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.OnlyUnread = this.chkKeepLastRead.Enabled = this.chkOnlyUnread.Checked));
    }

    private void chkKeepLastRead_CheckedChanged(object sender, EventArgs e)
    {
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.KeepLastRead = this.chkKeepLastRead.Checked));
    }

    private void chkOnlyChecked_CheckedChanged(object sender, EventArgs e)
    {
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.OnlyChecked = this.chkOnlyChecked.Checked));
    }

    private void chkLimit_CheckedChanged(object sender, EventArgs e)
    {
      this.txLimit.Enabled = this.cbLimitType.Enabled = this.chkLimit.Checked;
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.Limit = this.chkLimit.Checked));
    }

    private void chkSortBy_CheckedChanged(object sender, EventArgs e)
    {
      this.cbLimitSort.Enabled = this.chkSortBy.Checked;
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.Sort = this.chkSortBy.Checked));
    }

    private void txLimit_TextChanged(object sender, EventArgs e)
    {
      DeviceEditControl.TagElement tag = this.tvSharedLists.SelectedNode.Tag as DeviceEditControl.TagElement;
      int n;
      if (!int.TryParse(this.txLimit.Text, out n))
        return;
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.LimitValue = n.Clamp(0, 10000)));
    }

    private void cbLimitType_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.LimitValueType = (DeviceSyncSettings.LimitType) this.cbLimitType.SelectedIndex));
    }

    private void cbLimitSort_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SetSelectedListProperty((Action<DeviceSyncSettings.SharedListSettings>) (l => l.ListSortType = (DeviceSyncSettings.ListSort) this.cbLimitSort.SelectedIndex));
    }

    private void UpdateTree(DeviceSyncSettings settings, bool clear = false)
    {
      this.blockCheck = true;
      this.tvSharedLists.BeginUpdate();
      if (clear)
      {
        this.tvSharedLists.Nodes.Clear();
        this.tvSharedLists.Sorted = this.chkOnlyShowSelected.Checked;
      }
      DeviceEditControl.FillListTree(this.tvSharedLists.Nodes, settings, (IEnumerable<ComicListItem>) Program.Database.ComicLists, !this.chkOnlyShowSelected.Checked, this.chkOnlyShowSelected.Checked);
      this.tvSharedLists.EndUpdate();
      this.blockCheck = false;
      this.SetButtonStates();
      if (this.library == null)
        return;
      this.library.CommitComicListCacheChanges();
    }

    private void SetButtonStates()
    {
      IEnumerable<TreeNode> source1 = this.tvSharedLists.AllNodes();
      IEnumerable<TreeNode> source2 = this.tvSharedLists.SelectedNode == null ? Enumerable.Empty<TreeNode>() : this.tvSharedLists.SelectedNode.Nodes.All().AddFirst<TreeNode>(this.tvSharedLists.SelectedNode);
      this.btSelectAll.Enabled = source1.Any<TreeNode>((Func<TreeNode, bool>) (n => !n.Checked));
      this.btSelectNone.Enabled = source1.Any<TreeNode>((Func<TreeNode, bool>) (n => n.Checked));
      this.btSelectList.Enabled = source2.Any<TreeNode>((Func<TreeNode, bool>) (n => !n.Checked));
      this.btDeselectList.Enabled = source2.Any<TreeNode>((Func<TreeNode, bool>) (n => n.Checked));
      this.chkOnlyShowSelected.Enabled = this.chkOnlyShowSelected.Checked || this.chkOnlyChecked.Checked || this.btSelectNone.Enabled;
      this.chkOnlyChecked.Visible = this.chkOnlyChecked.Checked || this.tvSharedLists.Height > 12;
    }

    private void SetEditor(TreeNode node)
    {
      bool flag1 = node != null;
      this.grpListOptions.Enabled = flag1 && node.Checked;
      this.grpListOptions.Text = flag1 ? node.Text : string.Empty;
      if (!flag1)
        return;
      bool flag2 = node.Checked;
      ComicListItem comicListItem = DeviceEditControl.GetComicListItem(node);
      DeviceSyncSettings.SharedList list = DeviceEditControl.GetSharedList(node);
      if (list == null & flag2)
      {
        list = this.CreateDefaultSharedList(comicListItem.Id);
        DeviceEditControl.SetSharedList(node, list);
      }
      if (list == null)
        list = this.CreateDefaultSharedList(Guid.Empty);
      this.blockListUpdate = true;
      this.chkOnlyUnread.Checked = list.OnlyUnread;
      this.chkKeepLastRead.Checked = list.KeepLastRead;
      this.chkOptimizeSize.Checked = list.OptimizePortable;
      this.chkOnlyChecked.Checked = list.OnlyChecked;
      this.chkLimit.Checked = list.Limit;
      this.chkSortBy.Checked = list.Sort;
      this.txLimit.Text = list.LimitValue.ToString();
      this.cbLimitType.SelectedIndex = (int) list.LimitValueType;
      this.cbLimitSort.SelectedIndex = (int) list.ListSortType;
      this.blockListUpdate = false;
    }

    private static int FillListTree(
      TreeNodeCollection tnc,
      DeviceSyncSettings settings,
      IEnumerable<ComicListItem> clic,
      bool fillAll,
      bool flat)
    {
      int num = 0;
      foreach (ComicListItem comicListItem in clic)
      {
        ComicListItem cli = comicListItem;
        ++num;
        DeviceSyncSettings.SharedList other = settings.Lists.FirstOrDefault<DeviceSyncSettings.SharedList>((Func<DeviceSyncSettings.SharedList, bool>) (sl => sl.ListId == cli.Id));
        bool flag = other != null;
        TreeNode treeNode = tnc.Find((Predicate<TreeNode>) (n => ((DeviceEditControl.TagElement) n.Tag).Item == cli), false);
        if (flag | fillAll)
        {
          if (treeNode == null)
            treeNode = tnc.Add(cli.Name);
          treeNode.ImageKey = cli.ImageKey;
          treeNode.SelectedImageKey = cli.ImageKey;
          treeNode.Tag = (object) new DeviceEditControl.TagElement()
          {
            Item = cli,
            List = (flag ? new DeviceSyncSettings.SharedList(other) : (DeviceSyncSettings.SharedList) null)
          };
        }
        if (cli is ComicListItemFolder comicListItemFolder)
          num += DeviceEditControl.FillListTree(flat ? tnc : treeNode.Nodes, settings, (IEnumerable<ComicListItem>) comicListItemFolder.Items, fillAll, flat);
        if (treeNode != null)
        {
          treeNode.Checked = flag;
          treeNode.Expand();
        }
      }
      return num;
    }

    private static DeviceSyncSettings.SharedList GetSharedList(TreeNode node)
    {
      if (node == null)
        return (DeviceSyncSettings.SharedList) null;
      return !(node.Tag is DeviceEditControl.TagElement tag) ? (DeviceSyncSettings.SharedList) null : tag.List;
    }

    private static void SetSharedList(TreeNode node, DeviceSyncSettings.SharedList list)
    {
      if (node == null || !(node.Tag is DeviceEditControl.TagElement tag))
        return;
      tag.List = list;
    }

    private static ComicListItem GetComicListItem(TreeNode node)
    {
      if (node == null)
        return (ComicListItem) null;
      return !(node.Tag is DeviceEditControl.TagElement tag) ? (ComicListItem) null : tag.Item;
    }

    private ComicListItem GetSelectedComicListItem()
    {
      return DeviceEditControl.GetComicListItem(this.tvSharedLists.SelectedNode);
    }

    private DeviceSyncSettings.SharedList GetSelectedSharedList()
    {
      return DeviceEditControl.GetSharedList(this.tvSharedLists.SelectedNode);
    }

    private void SetListProperty(TreeNode node, Action<DeviceSyncSettings.SharedListSettings> set)
    {
      if (this.blockListUpdate)
        return;
      DeviceSyncSettings.SharedList sharedList = DeviceEditControl.GetSharedList(node);
      if (sharedList != null)
        set((DeviceSyncSettings.SharedListSettings) sharedList);
      set(this.Settings.DefaultListSettings);
    }

    private void SetSelectedListProperty(Action<DeviceSyncSettings.SharedListSettings> set)
    {
      this.SetListProperty(this.tvSharedLists.SelectedNode, set);
    }

    private DeviceSyncSettings.SharedList CreateDefaultSharedList(Guid id)
    {
      return new DeviceSyncSettings.SharedList(id, this.DefaultListSettings);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.ilShares = new ImageList(this.components);
      this.chkOptimizeSize = new CheckBox();
      this.toolTip = new ToolTip(this.components);
      this.btSelectAll = new Button();
      this.btSelectNone = new Button();
      this.chkOnlyUnread = new CheckBox();
      this.grpListOptions = new GroupBox();
      this.chkSortBy = new CheckBox();
      this.chkKeepLastRead = new CheckBox();
      this.cbLimitSort = new ComboBox();
      this.txLimit = new TextBox();
      this.chkLimit = new CheckBox();
      this.cbLimitType = new ComboBox();
      this.chkOnlyChecked = new CheckBox();
      this.tvSharedLists = new TreeViewEx();
      this.btSelectList = new Button();
      this.btDeselectList = new Button();
      this.chkOnlyShowSelected = new WrappingCheckBox();
      this.panel1 = new Panel();
      this.grpListOptions.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.ilShares.ColorDepth = ColorDepth.Depth8Bit;
      this.ilShares.ImageSize = new System.Drawing.Size(16, 16);
      this.ilShares.TransparentColor = Color.Transparent;
      this.chkOptimizeSize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkOptimizeSize.CheckAlign = ContentAlignment.TopLeft;
      this.chkOptimizeSize.Location = new System.Drawing.Point(398, 17);
      this.chkOptimizeSize.Name = "chkOptimizeSize";
      this.chkOptimizeSize.Size = new System.Drawing.Size(112, 20);
      this.chkOptimizeSize.TabIndex = 8;
      this.chkOptimizeSize.Text = "Optimize Size";
      this.chkOptimizeSize.UseVisualStyleBackColor = true;
      this.chkOptimizeSize.CheckedChanged += new EventHandler(this.chkOptimizeSize_CheckedChanged);
      this.btSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.btSelectAll.FlatStyle = FlatStyle.System;
      this.btSelectAll.Location = new System.Drawing.Point(3, 3);
      this.btSelectAll.Name = "btSelectAll";
      this.btSelectAll.Size = new System.Drawing.Size(113, 24);
      this.btSelectAll.TabIndex = 1;
      this.btSelectAll.Text = "Select All";
      this.btSelectAll.Click += new EventHandler(this.btSelectAll_Click);
      this.btSelectNone.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.btSelectNone.FlatStyle = FlatStyle.System;
      this.btSelectNone.Location = new System.Drawing.Point(3, 33);
      this.btSelectNone.Name = "btSelectNone";
      this.btSelectNone.Size = new System.Drawing.Size(113, 24);
      this.btSelectNone.TabIndex = 2;
      this.btSelectNone.Text = "Select None";
      this.btSelectNone.Click += new EventHandler(this.btSelectNone_Click);
      this.chkOnlyUnread.CheckAlign = ContentAlignment.TopLeft;
      this.chkOnlyUnread.Location = new System.Drawing.Point(15, 68);
      this.chkOnlyUnread.Name = "chkOnlyUnread";
      this.chkOnlyUnread.Size = new System.Drawing.Size(102, 17);
      this.chkOnlyUnread.TabIndex = 5;
      this.chkOnlyUnread.Text = "Only Unread";
      this.chkOnlyUnread.UseVisualStyleBackColor = true;
      this.chkOnlyUnread.CheckedChanged += new EventHandler(this.chkOnlyUnread_CheckedChanged);
      this.grpListOptions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.grpListOptions.Controls.Add((Control) this.chkSortBy);
      this.grpListOptions.Controls.Add((Control) this.chkKeepLastRead);
      this.grpListOptions.Controls.Add((Control) this.cbLimitSort);
      this.grpListOptions.Controls.Add((Control) this.txLimit);
      this.grpListOptions.Controls.Add((Control) this.chkLimit);
      this.grpListOptions.Controls.Add((Control) this.cbLimitType);
      this.grpListOptions.Controls.Add((Control) this.chkOnlyChecked);
      this.grpListOptions.Controls.Add((Control) this.chkOptimizeSize);
      this.grpListOptions.Controls.Add((Control) this.chkOnlyUnread);
      this.grpListOptions.Enabled = false;
      this.grpListOptions.Location = new System.Drawing.Point(0, 301);
      this.grpListOptions.Name = "grpListOptions";
      this.grpListOptions.Size = new System.Drawing.Size(516, 122);
      this.grpListOptions.TabIndex = 6;
      this.grpListOptions.TabStop = false;
      this.chkSortBy.Location = new System.Drawing.Point(15, 19);
      this.chkSortBy.Name = "chkSortBy";
      this.chkSortBy.Size = new System.Drawing.Size(102, 18);
      this.chkSortBy.TabIndex = 0;
      this.chkSortBy.Text = "Sort by";
      this.chkSortBy.UseVisualStyleBackColor = true;
      this.chkSortBy.CheckedChanged += new EventHandler(this.chkSortBy_CheckedChanged);
      this.chkKeepLastRead.CheckAlign = ContentAlignment.TopLeft;
      this.chkKeepLastRead.Location = new System.Drawing.Point(126, 68);
      this.chkKeepLastRead.Name = "chkKeepLastRead";
      this.chkKeepLastRead.Size = new System.Drawing.Size(144, 17);
      this.chkKeepLastRead.TabIndex = 6;
      this.chkKeepLastRead.Text = "But keep last read";
      this.chkKeepLastRead.UseVisualStyleBackColor = true;
      this.chkKeepLastRead.CheckedChanged += new EventHandler(this.chkKeepLastRead_CheckedChanged);
      this.cbLimitSort.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbLimitSort.Enabled = false;
      this.cbLimitSort.FormattingEnabled = true;
      this.cbLimitSort.Items.AddRange(new object[6]
      {
        (object) "Random",
        (object) "Series",
        (object) "Alternate Series",
        (object) "Published",
        (object) "Added",
        (object) "Story Arc"
      });
      this.cbLimitSort.Location = new System.Drawing.Point(126, 18);
      this.cbLimitSort.Name = "cbLimitSort";
      this.cbLimitSort.Size = new System.Drawing.Size(144, 21);
      this.cbLimitSort.TabIndex = 1;
      this.cbLimitSort.SelectedIndexChanged += new EventHandler(this.cbLimitSort_SelectedIndexChanged);
      this.txLimit.Enabled = false;
      this.txLimit.Location = new System.Drawing.Point(126, 45);
      this.txLimit.Name = "txLimit";
      this.txLimit.Size = new System.Drawing.Size(61, 20);
      this.txLimit.TabIndex = 3;
      this.txLimit.TextAlign = HorizontalAlignment.Right;
      this.txLimit.TextChanged += new EventHandler(this.txLimit_TextChanged);
      this.chkLimit.Location = new System.Drawing.Point(15, 44);
      this.chkLimit.Name = "chkLimit";
      this.chkLimit.Size = new System.Drawing.Size(102, 18);
      this.chkLimit.TabIndex = 2;
      this.chkLimit.Text = "Limit to ";
      this.chkLimit.UseVisualStyleBackColor = true;
      this.chkLimit.CheckedChanged += new EventHandler(this.chkLimit_CheckedChanged);
      this.cbLimitType.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbLimitType.Enabled = false;
      this.cbLimitType.FormattingEnabled = true;
      this.cbLimitType.Items.AddRange(new object[3]
      {
        (object) "Books",
        (object) "MB",
        (object) "GB"
      });
      this.cbLimitType.Location = new System.Drawing.Point(193, 45);
      this.cbLimitType.Name = "cbLimitType";
      this.cbLimitType.Size = new System.Drawing.Size(77, 21);
      this.cbLimitType.TabIndex = 4;
      this.cbLimitType.SelectedIndexChanged += new EventHandler(this.cbLimitType_SelectedIndexChanged);
      this.chkOnlyChecked.CheckAlign = ContentAlignment.TopLeft;
      this.chkOnlyChecked.Location = new System.Drawing.Point(15, 91);
      this.chkOnlyChecked.Name = "chkOnlyChecked";
      this.chkOnlyChecked.Size = new System.Drawing.Size(102, 17);
      this.chkOnlyChecked.TabIndex = 7;
      this.chkOnlyChecked.Text = "Only Checked";
      this.chkOnlyChecked.UseVisualStyleBackColor = true;
      this.chkOnlyChecked.CheckedChanged += new EventHandler(this.chkOnlyChecked_CheckedChanged);
      this.tvSharedLists.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.tvSharedLists.CheckBoxes = true;
      this.tvSharedLists.FullRowSelect = true;
      this.tvSharedLists.HideSelection = false;
      this.tvSharedLists.ImageIndex = 0;
      this.tvSharedLists.ImageList = this.ilShares;
      this.tvSharedLists.ItemHeight = 16;
      this.tvSharedLists.Location = new System.Drawing.Point(0, 3);
      this.tvSharedLists.Name = "tvSharedLists";
      this.tvSharedLists.SelectedImageIndex = 0;
      this.tvSharedLists.ShowLines = false;
      this.tvSharedLists.ShowPlusMinus = false;
      this.tvSharedLists.ShowRootLines = false;
      this.tvSharedLists.Size = new System.Drawing.Size(391, 292);
      this.tvSharedLists.TabIndex = 0;
      this.tvSharedLists.AfterCheck += new TreeViewEventHandler(this.tvSharedLists_AfterCheck);
      this.tvSharedLists.AfterSelect += new TreeViewEventHandler(this.tvSharedLists_AfterSelect);
      this.btSelectList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.btSelectList.FlatStyle = FlatStyle.System;
      this.btSelectList.Location = new System.Drawing.Point(3, 72);
      this.btSelectList.Name = "btSelectList";
      this.btSelectList.Size = new System.Drawing.Size(113, 24);
      this.btSelectList.TabIndex = 3;
      this.btSelectList.Text = "Select List";
      this.btSelectList.Click += new EventHandler(this.btSelectList_Click);
      this.btDeselectList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.btDeselectList.FlatStyle = FlatStyle.System;
      this.btDeselectList.Location = new System.Drawing.Point(3, 102);
      this.btDeselectList.Name = "btDeselectList";
      this.btDeselectList.Size = new System.Drawing.Size(113, 24);
      this.btDeselectList.TabIndex = 4;
      this.btDeselectList.Text = "Deselect List";
      this.btDeselectList.Click += new EventHandler(this.btUnselectList_Click);
      this.chkOnlyShowSelected.Appearance = Appearance.Button;
      this.chkOnlyShowSelected.AutoSize = true;
      this.chkOnlyShowSelected.Dock = DockStyle.Bottom;
      this.chkOnlyShowSelected.Location = new System.Drawing.Point(0, 269);
      this.chkOnlyShowSelected.Name = "chkOnlyShowSelected";
      this.chkOnlyShowSelected.Size = new System.Drawing.Size(119, 23);
      this.chkOnlyShowSelected.TabIndex = 7;
      this.chkOnlyShowSelected.Text = "Only show selected";
      this.chkOnlyShowSelected.TextAlign = ContentAlignment.MiddleCenter;
      this.chkOnlyShowSelected.UseVisualStyleBackColor = true;
      this.chkOnlyShowSelected.CheckedChanged += new EventHandler(this.chkOnlyShowSelected_CheckedChanged);
      this.panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
      this.panel1.Controls.Add((Control) this.btSelectAll);
      this.panel1.Controls.Add((Control) this.chkOnlyShowSelected);
      this.panel1.Controls.Add((Control) this.btSelectNone);
      this.panel1.Controls.Add((Control) this.btDeselectList);
      this.panel1.Controls.Add((Control) this.btSelectList);
      this.panel1.Location = new System.Drawing.Point(397, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(119, 292);
      this.panel1.TabIndex = 8;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.Transparent;
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.grpListOptions);
      this.Controls.Add((Control) this.tvSharedLists);
      this.MinimumSize = new System.Drawing.Size(410, 320);
      this.Name = nameof (DeviceEditControl);
      this.Size = new System.Drawing.Size(519, 426);
      this.grpListOptions.ResumeLayout(false);
      this.grpListOptions.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
    }

    private class TagElement
    {
      public ComicListItem Item;
      public DeviceSyncSettings.SharedList List;
    }
  }
}

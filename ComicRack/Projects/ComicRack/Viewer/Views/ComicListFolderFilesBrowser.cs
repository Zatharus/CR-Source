// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicListFolderFilesBrowser
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicListFolderFilesBrowser : ComicListFilesBrowser, IDisplayWorkspace
  {
    private readonly CommandMapper commands = new CommandMapper();
    private string cachedCurrentFolder = string.Empty;
    private SmartList<string> paths;
    private bool initialEnter = true;
    private IContainer components;
    private ContextMenuStrip contextMenuFolders;
    private ToolStripMenuItem miAddToFavorites;
    private ToolStripMenuItem miRefresh;
    private FolderTreeView tvFolders;
    private ToolStripSeparator menuItem2;
    private ToolStrip toolStrip;
    private ToolStripMenuItem miOpenWindow;
    private ToolStripButton tbOpenWindow;
    private ToolStripButton tbFavorites;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem miAddFolderLibrary;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton tbRefresh;
    private ToolStripButton tbIncludeSubFolders;
    private SizableContainer favContainer;
    private ContextMenuStrip contextMenuFavorites;
    private ToolStripMenuItem miFavRefresh;
    private ToolStripSeparator menuItem1;
    private ToolStripMenuItem miRemove;
    private ItemView favView;
    private ToolStripButton tbOpenTab;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem miOpenTab;

    public ComicListFolderFilesBrowser()
    {
      this.InitializeComponent();
      this.tvFolders.SortNetworkFolders = Program.ExtendedSettings.SortNetworkFolders;
      this.tvFolders.Font = SystemFonts.IconTitleFont;
      this.components.Add((IComponent) this.commands);
      LocalizeUtility.Localize((Control) this, this.components);
    }

    public ComicListFolderFilesBrowser(SmartList<string> paths)
      : this()
    {
      this.Paths = paths;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.components?.Dispose();
      base.Dispose(disposing);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string CurrentFolder
    {
      get
      {
        return this.tvFolders == null || this.tvFolders.Nodes.Count <= 0 ? this.cachedCurrentFolder : this.tvFolders.GetSelectedNodePath();
      }
      set
      {
        if (this.tvFolders == null || this.tvFolders.Nodes.Count == 0)
        {
          this.cachedCurrentFolder = value;
        }
        else
        {
          this.tvFolders.DrillToFolder(value);
          this.tvFolders.SelectedNode.EnsureVisible();
          this.cachedCurrentFolder = string.Empty;
        }
      }
    }

    public SmartList<string> Paths
    {
      get => this.paths;
      set
      {
        if (this.paths == value)
          return;
        if (this.paths != null)
          this.paths.Changed -= new EventHandler<SmartListChangedEventArgs<string>>(this.paths_Changed);
        this.paths = value;
        if (this.paths == null)
          return;
        this.paths.Changed += new EventHandler<SmartListChangedEventArgs<string>>(this.paths_Changed);
      }
    }

    protected virtual void OnInitalDisplay()
    {
      ControlExtensions.BeginInvoke(this, (Action) (() =>
      {
        this.tvFolders.Init();
        this.Update();
        if (string.IsNullOrEmpty(this.cachedCurrentFolder))
          return;
        this.CurrentFolder = this.cachedCurrentFolder;
      }));
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DesignMode)
        return;
      this.commands.Add(new CommandHandler(this.OpenWindow), (object) this.miOpenWindow, (object) this.tbOpenWindow);
      this.commands.Add(new CommandHandler(this.OpenTab), (object) this.miOpenTab, (object) this.tbOpenTab);
      this.commands.Add(new CommandHandler(this.AddToFavorites), (object) this.miAddToFavorites);
      this.commands.Add(new CommandHandler(((ComicListBrowser) this).RefreshDisplay), (object) this.tbRefresh, (object) this.miRefresh);
      this.commands.Add((CommandHandler) (() => Program.Scanner.ScanFileOrFolder(this.CurrentFolder, true, false)), (object) this.miAddFolderLibrary);
      this.commands.Add(new CommandHandler(((ComicListFilesBrowser) this).SwitchIncludeSubFolders), true, (UpdateHandler) (() => this.IncludeSubFolders), (object) this.tbIncludeSubFolders);
      this.commands.Add((CommandHandler) (() => this.TopBrowserVisible = !this.TopBrowserVisible), true, (UpdateHandler) (() => this.TopBrowserVisible), (object) this.tbFavorites);
      this.commands.Add(new CommandHandler(this.RefreshFavorites), (object) this.miFavRefresh);
      this.commands.Add(new CommandHandler(this.RemoveFavorite), (object) this.miRemove);
      this.CurrentFolder = Program.Settings.LastExplorerFolder;
      this.IncludeSubFolders = Program.Settings.ExplorerIncludeSubFolders;
      this.FillFavorites();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      if (!this.initialEnter)
        return;
      this.initialEnter = false;
      this.OnInitalDisplay();
    }

    protected override void OnRefreshDisplay()
    {
      base.OnRefreshDisplay();
      using (new WaitCursor())
      {
        string currentFolder = this.CurrentFolder;
        this.tvFolders.Init();
        this.tvFolders.DrillToFolder(currentFolder);
      }
    }

    public override bool TopBrowserVisible
    {
      get => this.favContainer.Expanded;
      set => this.favContainer.Expanded = value;
    }

    public override int TopBrowserSplit
    {
      get => this.favContainer.ExpandedWidth;
      set => this.favContainer.ExpandedWidth = value;
    }

    private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
    {
      this.FillBooks(this.CurrentFolder);
    }

    private void paths_Changed(object sender, SmartListChangedEventArgs<string> e)
    {
      this.FillFavorites();
    }

    private void favView_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!(this.favView.FocusedItem is ItemViewItem focusedItem))
        return;
      this.CurrentFolder = focusedItem.Tag as string;
    }

    private void favView_Resize(object sender, EventArgs e)
    {
      this.favView.ItemTileSize = new System.Drawing.Size(this.favView.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth - 8, FormUtility.ScaleDpiY(50));
    }

    private void FillFavorites() => this.FillFavorites(false);

    private void FillFavorites(bool refreshThumbnails)
    {
      using (new WaitCursor())
      {
        if (Program.Settings == null)
          return;
        List<string> list = new List<string>();
        this.favView.Items.Clear();
        foreach (string favoriteFolder in Program.Settings.FavoriteFolders)
        {
          if (!Directory.Exists(favoriteFolder))
          {
            list.Add(favoriteFolder);
          }
          else
          {
            FolderViewItem folderViewItem = FolderViewItem.Create(favoriteFolder);
            folderViewItem.Tag = (object) favoriteFolder;
            folderViewItem.TooltipText = favoriteFolder;
            this.favView.Items.Add((IViewableItem) folderViewItem);
            if (refreshThumbnails)
              Program.ImagePool.Thumbs.RefreshImage((ImageKey) folderViewItem.ThumbnailKey);
          }
        }
        Program.Settings.FavoriteFolders.RemoveRange((IEnumerable<string>) list);
      }
    }

    private void RemoveFavorite()
    {
      ItemViewItem focusedItem = this.favView.FocusedItem == null ? (ItemViewItem) null : this.favView.FocusedItem as ItemViewItem;
      if (focusedItem == null || !Program.AskQuestion((IWin32Window) this, TR.Messages["AskRemoveFavorite", "Do you really want to remove this Favorite Folder link?"], TR.Messages["Remove", "Remove"], HiddenMessageBoxes.RemoveFavorite))
        return;
      Program.Settings.FavoriteFolders.Remove(focusedItem.Tag as string);
      this.favView.Items.Remove((IViewableItem) focusedItem);
    }

    private void RefreshFavorites() => this.FillFavorites(true);

    private void AddToFavorites()
    {
      if (Program.Settings.FavoriteFolders.Contains(this.CurrentFolder))
        return;
      Program.Settings.FavoriteFolders.Add(this.CurrentFolder);
    }

    private void OpenWindow()
    {
      try
      {
        this.OpenListInNewWindow();
      }
      catch
      {
      }
    }

    private void OpenTab()
    {
      try
      {
        this.OpenListInNewTab((Image) Resources.SearchFolder);
      }
      catch
      {
      }
    }

    public void SetWorkspace(DisplayWorkspace workspace)
    {
    }

    public void StoreWorkspace(DisplayWorkspace workspace)
    {
      Program.Settings.ExplorerIncludeSubFolders = this.IncludeSubFolders;
      Program.Settings.LastExplorerFolder = this.CurrentFolder;
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ComicListFolderFilesBrowser));
      this.contextMenuFolders = new ContextMenuStrip(this.components);
      this.miOpenWindow = new ToolStripMenuItem();
      this.miOpenTab = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.miAddToFavorites = new ToolStripMenuItem();
      this.miAddFolderLibrary = new ToolStripMenuItem();
      this.menuItem2 = new ToolStripSeparator();
      this.miRefresh = new ToolStripMenuItem();
      this.tvFolders = new FolderTreeView();
      this.toolStrip = new ToolStrip();
      this.tbFavorites = new ToolStripButton();
      this.tbIncludeSubFolders = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.tbOpenWindow = new ToolStripButton();
      this.tbOpenTab = new ToolStripButton();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.tbRefresh = new ToolStripButton();
      this.favContainer = new SizableContainer();
      this.favView = new ItemView();
      this.contextMenuFavorites = new ContextMenuStrip(this.components);
      this.miFavRefresh = new ToolStripMenuItem();
      this.menuItem1 = new ToolStripSeparator();
      this.miRemove = new ToolStripMenuItem();
      this.contextMenuFolders.SuspendLayout();
      this.toolStrip.SuspendLayout();
      this.favContainer.SuspendLayout();
      this.contextMenuFavorites.SuspendLayout();
      this.SuspendLayout();
      this.contextMenuFolders.Items.AddRange(new ToolStripItem[7]
      {
        (ToolStripItem) this.miOpenWindow,
        (ToolStripItem) this.miOpenTab,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.miAddToFavorites,
        (ToolStripItem) this.miAddFolderLibrary,
        (ToolStripItem) this.menuItem2,
        (ToolStripItem) this.miRefresh
      });
      this.contextMenuFolders.Name = "contextMenuFolders";
      this.contextMenuFolders.Size = new System.Drawing.Size(363, 206);
      this.miOpenWindow.Image = (Image) Resources.NewWindow;
      this.miOpenWindow.Name = "miOpenWindow";
      this.miOpenWindow.Size = new System.Drawing.Size(362, 38);
      this.miOpenWindow.Text = "&Open in New Window";
      this.miOpenTab.Image = (Image) Resources.NewTab;
      this.miOpenTab.Name = "miOpenTab";
      this.miOpenTab.Size = new System.Drawing.Size(362, 38);
      this.miOpenTab.Text = "Open in New Tab";
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(359, 6);
      this.miAddToFavorites.Image = (Image) Resources.AddFavorites;
      this.miAddToFavorites.Name = "miAddToFavorites";
      this.miAddToFavorites.Size = new System.Drawing.Size(362, 38);
      this.miAddToFavorites.Text = "&Add Folder to Favorites";
      this.miAddFolderLibrary.Image = (Image) Resources.AddFolder;
      this.miAddFolderLibrary.Name = "miAddFolderLibrary";
      this.miAddFolderLibrary.Size = new System.Drawing.Size(362, 38);
      this.miAddFolderLibrary.Text = "&Add Folder to Library";
      this.menuItem2.Name = "menuItem2";
      this.menuItem2.Size = new System.Drawing.Size(359, 6);
      this.miRefresh.Image = (Image) Resources.Refresh;
      this.miRefresh.Name = "miRefresh";
      this.miRefresh.Size = new System.Drawing.Size(362, 38);
      this.miRefresh.Text = "&Refresh";
      this.tvFolders.ContextMenuStrip = this.contextMenuFolders;
      this.tvFolders.Dock = DockStyle.Fill;
      this.tvFolders.FullRowSelect = true;
      this.tvFolders.HideSelection = false;
      this.tvFolders.ImageIndex = 0;
      this.tvFolders.Indent = 15;
      this.tvFolders.Location = new System.Drawing.Point(0, 199);
      this.tvFolders.Name = "tvFolders";
      this.tvFolders.SelectedImageIndex = 0;
      this.tvFolders.Size = new System.Drawing.Size(379, (int) byte.MaxValue);
      this.tvFolders.TabIndex = 7;
      this.tvFolders.AfterSelect += new TreeViewEventHandler(this.tvFolders_AfterSelect);
      this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;
      this.toolStrip.Items.AddRange(new ToolStripItem[7]
      {
        (ToolStripItem) this.tbFavorites,
        (ToolStripItem) this.tbIncludeSubFolders,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.tbOpenWindow,
        (ToolStripItem) this.tbOpenTab,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.tbRefresh
      });
      this.toolStrip.Location = new System.Drawing.Point(0, 0);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(379, 39);
      this.toolStrip.TabIndex = 8;
      this.toolStrip.Text = "toolStrip1";
      this.tbFavorites.Alignment = ToolStripItemAlignment.Right;
      this.tbFavorites.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbFavorites.Image = (Image) Resources.Favorites;
      this.tbFavorites.Name = "tbFavorites";
      this.tbFavorites.Size = new System.Drawing.Size(36, 36);
      this.tbFavorites.Text = "Show Favorites";
      this.tbFavorites.ToolTipText = "Favorites";
      this.tbIncludeSubFolders.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbIncludeSubFolders.Image = (Image) Resources.IncludeSubFolders;
      this.tbIncludeSubFolders.ImageTransparentColor = Color.Magenta;
      this.tbIncludeSubFolders.Name = "tbIncludeSubFolders";
      this.tbIncludeSubFolders.Size = new System.Drawing.Size(36, 36);
      this.tbIncludeSubFolders.Text = "Include all Subfolders";
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
      this.tbOpenWindow.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbOpenWindow.Image = (Image) Resources.NewWindow;
      this.tbOpenWindow.ImageTransparentColor = Color.Magenta;
      this.tbOpenWindow.Name = "tbOpenWindow";
      this.tbOpenWindow.Size = new System.Drawing.Size(36, 36);
      this.tbOpenWindow.Text = "New Window";
      this.tbOpenWindow.ToolTipText = "Open current list in new Window";
      this.tbOpenTab.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbOpenTab.Image = (Image) Resources.NewTab;
      this.tbOpenTab.ImageTransparentColor = Color.Magenta;
      this.tbOpenTab.Name = "tbOpenTab";
      this.tbOpenTab.Size = new System.Drawing.Size(36, 36);
      this.tbOpenTab.Text = "New Tab";
      this.tbOpenTab.ToolTipText = "Open current list in new Tab";
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
      this.tbRefresh.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbRefresh.Image = (Image) Resources.Refresh;
      this.tbRefresh.ImageTransparentColor = Color.Magenta;
      this.tbRefresh.Name = "tbRefresh";
      this.tbRefresh.Size = new System.Drawing.Size(36, 36);
      this.tbRefresh.Text = "Refresh";
      this.favContainer.AutoGripPosition = true;
      this.favContainer.Controls.Add((Control) this.favView);
      this.favContainer.Dock = DockStyle.Top;
      this.favContainer.Grip = SizableContainer.GripPosition.Bottom;
      this.favContainer.Location = new System.Drawing.Point(0, 39);
      this.favContainer.Name = "favContainer";
      this.favContainer.Padding = new Padding(0, 6, 0, 0);
      this.favContainer.Size = new System.Drawing.Size(379, 160);
      this.favContainer.TabIndex = 9;
      this.favContainer.Text = "favContainer";
      this.favView.BackColor = SystemColors.Window;
      this.favView.ContextMenuStrip = this.contextMenuFavorites;
      this.favView.Dock = DockStyle.Fill;
      this.favView.GroupColumns = new IColumn[0];
      this.favView.GroupColumnsKey = (string) null;
      this.favView.GroupsStatus = (ItemViewGroupsStatus) componentResourceManager.GetObject("favView.GroupsStatus");
      this.favView.ItemViewMode = ItemViewMode.Tile;
      this.favView.Location = new System.Drawing.Point(0, 6);
      this.favView.Multiselect = false;
      this.favView.Name = "favView";
      this.favView.Size = new System.Drawing.Size(379, 148);
      this.favView.SortColumn = (IColumn) null;
      this.favView.SortColumns = new IColumn[0];
      this.favView.SortColumnsKey = (string) null;
      this.favView.StackColumns = new IColumn[0];
      this.favView.StackColumnsKey = (string) null;
      this.favView.TabIndex = 1;
      this.favView.SelectedIndexChanged += new EventHandler(this.favView_SelectedIndexChanged);
      this.favView.Resize += new EventHandler(this.favView_Resize);
      this.contextMenuFavorites.Items.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.miFavRefresh,
        (ToolStripItem) this.menuItem1,
        (ToolStripItem) this.miRemove
      });
      this.contextMenuFavorites.Name = "contextMenuFavorites";
      this.contextMenuFavorites.Size = new System.Drawing.Size(268, 86);
      this.miFavRefresh.Image = (Image) Resources.Refresh;
      this.miFavRefresh.Name = "miFavRefresh";
      this.miFavRefresh.Size = new System.Drawing.Size(267, 38);
      this.miFavRefresh.Text = "&Refresh";
      this.menuItem1.Name = "menuItem1";
      this.menuItem1.Size = new System.Drawing.Size(264, 6);
      this.miRemove.Image = (Image) Resources.EditDelete;
      this.miRemove.Name = "miRemove";
      this.miRemove.ShortcutKeys = Keys.Delete;
      this.miRemove.Size = new System.Drawing.Size(267, 38);
      this.miRemove.Text = "&Remove...";
      this.AutoScaleMode = AutoScaleMode.None;
      this.Controls.Add((Control) this.tvFolders);
      this.Controls.Add((Control) this.favContainer);
      this.Controls.Add((Control) this.toolStrip);
      this.Name = nameof (ComicListFolderFilesBrowser);
      this.Size = new System.Drawing.Size(379, 454);
      this.contextMenuFolders.ResumeLayout(false);
      this.toolStrip.ResumeLayout(false);
      this.toolStrip.PerformLayout();
      this.favContainer.ResumeLayout(false);
      this.contextMenuFavorites.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

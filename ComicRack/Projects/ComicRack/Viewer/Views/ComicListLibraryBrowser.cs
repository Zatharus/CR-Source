// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicListLibraryBrowser
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.Sync;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Dialogs;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicListLibraryBrowser : 
    ComicListBrowser,
    IDisplayWorkspace,
    IImportComicList,
    ILibraryBrowser,
    IBrowseHistory
  {
    private readonly CommandMapper commands = new CommandMapper();
    private readonly Dictionary<Guid, TreeNode> nodeMap = new Dictionary<Guid, TreeNode>();
    private bool treeDirty;
    private readonly NiceTreeSkin treeSkin;
    private ComicLibrary library;
    private bool mouseActivate;
    private TreeNode dragNode;
    private DragDropContainer dragBookContainer;
    private IBitmapCursor dragCursor;
    private IContainer components;
    private TreeViewEx tvQueries;
    private ContextMenuStrip treeContextMenu;
    private ToolStripMenuItem miEditSmartList;
    private ToolStripMenuItem miQueryRename;
    private ToolStripSeparator miRemoveSeparator;
    private ToolStripMenuItem miRemoveListOrFolder;
    private ToolStripSeparator miOpenSeparator;
    private ToolStripMenuItem miNewSmartList;
    private ToolStripMenuItem miNewFolder;
    private ImageList treeImages;
    private ToolStripMenuItem miNewList;
    private ToolStrip toolStrip;
    private ToolStripButton tbNewFolder;
    private ToolStripButton tbNewList;
    private ToolStripButton tbNewSmartList;
    private ToolStripMenuItem miOpenWindow;
    private ToolStripSeparator miNewSeparator;
    private ToolStripSeparator tssOpenWindow;
    private ToolStripButton tbOpenWindow;
    private ToolStripButton tbRefresh;
    private ToolStripMenuItem miRefresh;
    private ToolStripMenuItem miNodeSort;
    private ToolStripSeparator miCopySeparator;
    private ToolStripMenuItem miCopyList;
    private ToolStripMenuItem miPasteList;
    private ToolStripMenuItem miExportReadingList;
    private ToolStripMenuItem miImportReadingList;
    private ToolStripMenuItem miOpenTab;
    private ToolStripButton tbOpenTab;
    private ToolStripSeparator tbRefreshSeparator;
    private Timer updateTimer;
    private SizableContainer favContainer;
    private ItemView favView;
    private ToolStripButton tbFavorites;
    private ContextMenuStrip contextMenuFavorites;
    private ToolStripMenuItem miFavRefresh;
    private ToolStripSeparator menuItem1;
    private ToolStripMenuItem miFavRemove;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem miAddToFavorites;
    private ToolStripButton tbExpandCollapseAll;
    private Timer queryCacheTimer;
    private ToolStripMenuItem cmEditDevices;
    private ToolStripButton tsQuickSearch;
    private Panel quickSearchPanel;
    private SearchTextBox quickSearch;

    public ComicListLibraryBrowser()
    {
      this.InitializeComponent();
      this.treeImages.ImageSize = this.treeImages.ImageSize.ScaleDpi();
      LibraryTreeSkin libraryTreeSkin = new LibraryTreeSkin();
      libraryTreeSkin.TreeView = (TreeView) this.tvQueries;
      this.treeSkin = (NiceTreeSkin) libraryTreeSkin;
      this.tvQueries.Font = SystemFonts.IconTitleFont;
      this.favContainer.Expanded = false;
      LocalizeUtility.Localize((Control) this, this.components);
      this.quickSearch.SetCueText(this.tsQuickSearch.Text);
      this.queryCacheTimer.Interval = ComicLibrary.IsQueryCacheInstantUpdate ? 100 : 2500;
    }

    public ComicListLibraryBrowser(ComicLibrary library)
      : this()
    {
      this.Library = library;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    public ComicLibrary Library
    {
      get => this.library;
      set
      {
        if (this.library == value)
          return;
        if (this.library != null)
        {
          this.library.ComicListCachesUpdated -= new EventHandler(this.library_ListCachesUpdated);
          this.library.ComicListsChanged -= new ComicListChangedEventHandler(this.library_ComicListsChanged);
        }
        this.library = value;
        if (this.library != null)
        {
          this.library.ComicListCachesUpdated += new EventHandler(this.library_ListCachesUpdated);
          this.library.ComicListsChanged += new ComicListChangedEventHandler(this.library_ComicListsChanged);
        }
        this.OnLibraryChanged();
      }
    }

    public ComicsEditModes ComicEditMode
    {
      get => this.Library != null ? this.Library.EditMode : ComicsEditModes.Default;
    }

    public event EventHandler LibraryChanged;

    protected virtual void OnLibraryChanged()
    {
      this.FillListTree();
      if (this.LibraryChanged == null)
        return;
      this.LibraryChanged((object) this, EventArgs.Empty);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DesignMode)
        return;
      this.treeImages.Images.Add("Library", (Image) Resources.Library);
      this.treeImages.Images.Add("Folder", (Image) Resources.SearchFolder);
      this.treeImages.Images.Add("Search", (Image) Resources.SearchDocument);
      this.treeImages.Images.Add("List", (Image) Resources.List);
      this.treeImages.Images.Add("TempFolder", (Image) Resources.TempFolder);
      this.FillListTree();
      TreeNode itemNode = this.FindItemNode(Program.Settings.LastLibraryItem);
      if (itemNode != null)
        this.tvQueries.SelectedNode = itemNode;
      this.UpdateBookList();
      this.commands.Add(new CommandHandler(this.ExpandCollapseAllNodes), (object) this.tbExpandCollapseAll);
      this.commands.Add(new CommandHandler(this.RenameNode), (UpdateHandler) (() => this.tvQueries.SelectedNode != null && this.ComicEditMode.CanEditList()), (object) this.miQueryRename);
      this.commands.Add((CommandHandler) (() => this.EditSelectedComicList()), (UpdateHandler) (() => this.tvQueries.SelectedNode != null && (this.tvQueries.SelectedNode.Tag is ComicSmartListItem || this.tvQueries.SelectedNode.Tag is ComicListItemFolder || this.tvQueries.SelectedNode.Tag is ComicIdListItem) && this.ComicEditMode.CanEditList()), (object) this.miEditSmartList);
      this.commands.Add(new CommandHandler(this.NewSmartList), (UpdateHandler) (() => this.ComicEditMode.CanEditList()), (object) this.miNewSmartList, (object) this.tbNewSmartList);
      this.commands.Add(new CommandHandler(this.NewFolder), (UpdateHandler) (() => this.ComicEditMode.CanEditList()), (object) this.miNewFolder, (object) this.tbNewFolder);
      this.commands.Add(new CommandHandler(this.NewList), (UpdateHandler) (() => this.ComicEditMode.CanEditList()), (object) this.miNewList, (object) this.tbNewList);
      this.commands.Add(new CommandHandler(this.RemoveListOrFolder), (UpdateHandler) (() => this.tvQueries.SelectedNode != null && !this.tvQueries.SelectedNode.IsEditing && !(this.tvQueries.SelectedNode.Tag is ComicLibraryListItem) && this.ComicEditMode.CanEditList()), (object) this.miRemoveListOrFolder);
      this.commands.Add(new CommandHandler(this.OpenWindow), (object) this.miOpenWindow, (object) this.tbOpenWindow);
      this.commands.Add(new CommandHandler(this.OpenTab), (object) this.miOpenTab, (object) this.tbOpenTab);
      this.commands.Add(new CommandHandler(((ComicListBrowser) this).RefreshDisplay), (object) this.tbRefresh);
      this.commands.Add(new CommandHandler(this.SortList), (UpdateHandler) (() => this.tvQueries.SelectedNode != null && this.tvQueries.SelectedNode.Tag is ComicListItemFolder), (object) this.miNodeSort);
      this.commands.Add(new CommandHandler(this.CopyList), (UpdateHandler) (() => this.tvQueries.SelectedNode != null && (this.tvQueries.SelectedNode.Tag is ShareableComicListItem || Program.ExtendedSettings.AllowCopyListFolders && this.tvQueries.SelectedNode.Tag is ComicListItemFolder) && this.ComicEditMode.CanEditList()), (object) this.miCopyList);
      this.commands.Add(new CommandHandler(this.ExportList), (UpdateHandler) (() => this.tvQueries.SelectedNode != null && this.tvQueries.SelectedNode.Tag is ShareableComicListItem && this.ComicEditMode.CanEditList()), (object) this.miExportReadingList);
      this.commands.Add(new CommandHandler(this.ImportLists), (UpdateHandler) (() => this.ComicEditMode.CanEditList()), (object) this.miImportReadingList);
      this.commands.Add(new CommandHandler(this.PasteList), (UpdateHandler) (() => (Clipboard.ContainsData("ComicList") || Clipboard.ContainsText()) && this.ComicEditMode.CanEditList()), (object) this.miPasteList);
      this.commands.Add(new CommandHandler(this.AddToFavorites), (object) this.miAddToFavorites);
      this.commands.Add((CommandHandler) (() => this.TopBrowserVisible = !this.TopBrowserVisible), true, (UpdateHandler) (() => this.TopBrowserVisible), (object) this.tbFavorites);
      this.commands.Add(new CommandHandler(this.ToggleQuickSearch), true, (UpdateHandler) (() => this.quickSearchPanel.Visible), (object) this.tsQuickSearch);
      this.commands.Add(new CommandHandler(this.RefreshFavorites), (object) this.miFavRefresh);
      this.commands.Add(new CommandHandler(this.RemoveFavorite), (object) this.miFavRemove);
      this.miQueryRename.Visible = this.miEditSmartList.Visible = this.miNewSmartList.Visible = this.tbNewSmartList.Visible = this.miNewList.Visible = this.tbNewList.Visible = this.miNewFolder.Visible = this.tbNewFolder.Visible = this.miRemoveListOrFolder.Visible = this.miCopyList.Visible = this.miPasteList.Visible = this.miExportReadingList.Visible = this.miImportReadingList.Visible = this.tssOpenWindow.Visible = this.miNewSeparator.Visible = this.miCopySeparator.Visible = this.miRemoveSeparator.Visible = this.miAddToFavorites.Visible = this.ComicEditMode.CanEditList();
      this.miRefresh.Visible = this.tbRefreshSeparator.Visible = this.tbRefresh.Visible = !this.ComicEditMode.IsLocalComic();
    }

    protected override void OnListServiceRequest(
      IComicBookListProvider senderList,
      ServiceRequestEventArgs e)
    {
      base.OnListServiceRequest(senderList, e);
      if (e.ServiceType == typeof (IDisplayListConfig) && !this.ComicEditMode.IsLocalComic() && senderList is IDisplayListConfig vc)
        e.Service = (object) new ComicListLibraryBrowser.ViewConfigurationHandler(senderList.Id, vc);
      if (e.Service != null || !(e.ServiceType == typeof (IRemoveBooks)))
        return;
      e.Service = (object) new ComicListLibraryBrowser.RemoveBookHandler((IWin32Window) this, this.Library, senderList);
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

    private TreeNode FindItemNode(Guid id)
    {
      TreeNode treeNode;
      return !this.nodeMap.TryGetValue(id, out treeNode) ? (TreeNode) null : treeNode;
    }

    private TreeNode FindItemNode(IComicBookListProvider item)
    {
      return item == null ? (TreeNode) null : this.FindItemNode(item.Id);
    }

    [Conditional("BIGITEMS")]
    private static void SetDoubleHeight(TreeNode node) => node.SetHeight(2);

    private TreeNode AddNode(TreeNodeCollection nodes, ComicListItem item)
    {
      TreeNode treeNode = nodes.Add(item.Name);
      this.nodeMap[item.Id] = treeNode;
      treeNode.Tag = (object) item;
      treeNode.ImageKey = treeNode.SelectedImageKey = item.ImageKey;
      ComicLibraryListItem comicLibraryListItem = item as ComicLibraryListItem;
      return treeNode;
    }

    private bool RemoveNode(ComicListItem item)
    {
      TreeNode itemNode = this.FindItemNode((IComicBookListProvider) item);
      if (itemNode == null)
        return false;
      if (itemNode.Parent == null)
        this.tvQueries.Nodes.Remove(itemNode);
      else
        itemNode.Parent.Nodes.Remove(itemNode);
      this.nodeMap.Remove(item.Id);
      return true;
    }

    private void FillListTree(
      TreeNodeCollection tnc,
      ICollection<ComicListItem> items,
      string filter = null)
    {
      if (!string.IsNullOrEmpty(filter))
        items = (ICollection<ComicListItem>) items.Where<ComicListItem>((Func<ComicListItem, bool>) (cli => cli is ComicLibraryListItem || cli.Filter(filter))).ToArray<ComicListItem>();
      ((IEnumerable<ComicListItem>) tnc.OfType<TreeNode>().Select<TreeNode, ComicListItem>((Func<TreeNode, ComicListItem>) (tn => tn.Tag as ComicListItem)).Where<ComicListItem>((Func<ComicListItem, bool>) (cli => !items.Contains(cli))).ToArray<ComicListItem>()).ForEach<ComicListItem>((Action<ComicListItem>) (cli => this.RemoveNode(cli)));
      int index = 0;
      foreach (ComicListItem comicListItem in (IEnumerable<ComicListItem>) items)
      {
        TreeNode node = this.FindItemNode((IComicBookListProvider) comicListItem);
        bool flag = false;
        if (comicListItem is ComicListItemFolder comicListItemFolder)
          flag = !comicListItemFolder.Collapsed;
        else if (comicListItem is ComicLibraryListItem)
          flag = true;
        if (node == null)
        {
          node = this.AddNode(tnc, comicListItem);
        }
        else
        {
          if (comicListItem.Name != node.Text)
            node.Text = comicListItem.Name;
          string str = comicListItem.Description.LineBreak(60);
          if (str != node.ToolTipText)
            node.ToolTipText = str;
        }
        if (tnc.IndexOf(node) != index)
        {
          tnc.Remove(node);
          tnc.Insert(index, node);
          ComicLibraryListItem comicLibraryListItem = comicListItem as ComicLibraryListItem;
        }
        ++index;
        node.ForeColor = comicListItem.RecursionTest() ? Color.Red : SystemColors.WindowText;
        if (comicListItemFolder != null)
          this.FillListTree(node.Nodes, (ICollection<ComicListItem>) comicListItemFolder.Items, filter);
        if (flag)
          node.Expand();
        else
          node.Collapse();
      }
    }

    private void FillListTree(ICollection<ComicListItem> items, string filter = null)
    {
      Guid bookListId = this.BookListId;
      if (items != null)
        this.FillListTree(this.tvQueries.Nodes, items, filter);
      if (this.tvQueries.Nodes.Count <= 0)
        return;
      this.tvQueries.SelectedNode = this.FindItemNode(bookListId) ?? this.tvQueries.Nodes[0];
    }

    private void FillListTree()
    {
      this.FillListTree((ICollection<ComicListItem>) this.Library.ComicLists, this.quickSearch.Text.Trim());
      this.FillFavorites();
    }

    private bool EditSelectedComicList()
    {
      if (!this.ComicEditMode.CanEditList())
        return false;
      TreeNode selectedNode = this.tvQueries.SelectedNode;
      if (selectedNode == null)
        return false;
      if (selectedNode.Tag is ComicSmartListItem)
        return this.EditSmartListItem(selectedNode, selectedNode.Tag as ComicSmartListItem);
      return (selectedNode.Tag is ComicListItemFolder || selectedNode.Tag is ComicIdListItem) && this.EditListItem(selectedNode.Tag as ComicListItem);
    }

    private bool EditListItem(ComicListItem cli)
    {
      if (cli == null || !EditListDialog.Edit((IWin32Window) this, cli))
        return false;
      cli.Refresh();
      return true;
    }

    private bool EditSmartListItem(TreeNode tn, ComicSmartListItem csli)
    {
      if (csli == null)
        return false;
      bool flag = Control.ModifierKeys.HasFlag((Enum) Keys.Control);
      ComicSmartListItem comicSmartListItem = csli.Clone() as ComicSmartListItem;
      ComicSmartListItem oldList = csli.Clone() as ComicSmartListItem;
      while (comicSmartListItem != null)
      {
        using (Form form = flag ? (Form) new SmartListQueryDialog() : (Form) new SmartListDialog())
        {
          ISmartListDialog sld = form as ISmartListDialog;
          sld.Library = csli.Library;
          sld.EditId = csli.Id;
          sld.SmartComicList = comicSmartListItem;
          TreeNodeCollection tnc = tn.Parent != null ? tn.Parent.Nodes : this.tvQueries.Nodes;
          sld.EnableNavigation = tnc.OfType<TreeNode>().Count<TreeNode>((Func<TreeNode, bool>) (n => n.Tag is ComicSmartListItem)) > 1;
          Func<TreeNode> getNext = (Func<TreeNode>) (() => tnc.OfType<TreeNode>().SkipWhile<TreeNode>((Func<TreeNode, bool>) (n => n != tn)).Skip<TreeNode>(1).FirstOrDefault<TreeNode>((Func<TreeNode, bool>) (n => n.Tag is ComicSmartListItem)));
          Func<TreeNode> getPrev = (Func<TreeNode>) (() => tnc.OfType<TreeNode>().Reverse<TreeNode>().SkipWhile<TreeNode>((Func<TreeNode, bool>) (n => n != tn)).Skip<TreeNode>(1).FirstOrDefault<TreeNode>((Func<TreeNode, bool>) (n => n.Tag is ComicSmartListItem)));
          Action<Func<TreeNode>> setItem = (Action<Func<TreeNode>>) (get =>
          {
            TreeNode treeNode = get();
            if (treeNode != null)
            {
              using (new WaitCursor((Control) this))
              {
                this.tvQueries.SelectedNode = tn = treeNode;
                this.UpdateBookList();
              }
              csli = treeNode.Tag as ComicSmartListItem;
              oldList = csli.Clone() as ComicSmartListItem;
              sld.EditId = csli.Id;
              Application.DoEvents();
              sld.SmartComicList = csli.Clone() as ComicSmartListItem;
            }
            sld.PreviousEnabled = getPrev() != null;
            sld.NextEnabled = getNext() != null;
          });
          sld.PreviousEnabled = getPrev() != null;
          sld.NextEnabled = getNext() != null;
          sld.Apply += (EventHandler) ((sender, e) => csli.SetList(sld.SmartComicList));
          sld.Next += (EventHandler) ((sender, e) => setItem(getNext));
          sld.Previous += (EventHandler) ((sender, e) => setItem(getPrev));
          switch (form.ShowDialog((IWin32Window) this))
          {
            case DialogResult.Cancel:
              csli.SetList(oldList);
              return false;
            case DialogResult.Retry:
              comicSmartListItem = sld.SmartComicList;
              break;
            default:
              comicSmartListItem = (ComicSmartListItem) null;
              break;
          }
        }
        flag = !flag;
      }
      return true;
    }

    protected override void OnIdle()
    {
      base.OnIdle();
      if (this.Library.ComicListsLocked || !this.treeDirty)
        return;
      this.treeDirty = false;
      this.FillListTree();
      this.CommitListCacheChange();
    }

    private ComicListItem GetCurrentNodeComicList()
    {
      return this.tvQueries.SelectedNode != null ? (ComicListItem) this.tvQueries.SelectedNode.Tag : (ComicListItem) null;
    }

    private ComicListItemCollection GetCurrentNodeComicListCollection()
    {
      return this.GetNodeComicListCollection(this.tvQueries.SelectedNode);
    }

    private ComicListItemCollection GetNodeComicListCollection(TreeNode sn)
    {
      if (sn == null)
        return this.Library.ComicLists;
      if (sn.Tag is ComicListItemFolder)
        return ((ComicListItemFolder) sn.Tag).Items;
      return sn.Parent != null ? ((ComicListItemFolder) sn.Parent.Tag).Items : this.Library.ComicLists;
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
        this.OpenListInNewTab(this.treeImages.Images[this.tvQueries.SelectedNode.ImageKey]);
      }
      catch (Exception ex)
      {
        Trace.WriteLine("Failed to open in other tab: " + ex.Message);
      }
    }

    private void library_ComicListsChanged(object sender, ComicListItemChangedEventArgs e)
    {
      if (this.treeDirty)
        return;
      if (e.Change != ComicListItemChange.Statistic)
        this.treeDirty = true;
      else
        this.tvQueries.Invalidate();
    }

    private void tvQueries_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (this.tvQueries.HitTest(e.Location).Location == TreeViewHitTestLocations.PlusMinus)
        return;
      this.EditSelectedComicList();
    }

    private void tvQueries_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (this.dragNode != null)
        return;
      if (this.mouseActivate)
      {
        this.UpdateBookList();
      }
      else
      {
        this.updateTimer.Stop();
        this.updateTimer.Start();
      }
      this.mouseActivate = false;
    }

    private void updateTimer_Tick(object sender, EventArgs e) => this.UpdateBookList();

    private void UpdateBookList()
    {
      this.updateTimer.Stop();
      using (new WaitCursor())
      {
        if (this.tvQueries.SelectedNode == null)
          return;
        this.BookList = this.tvQueries.SelectedNode.Tag as IComicBookListProvider;
      }
    }

    private void tvQueries_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
      e.CancelEdit = !this.ComicEditMode.CanEditList();
    }

    private void tvQueries_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
      ComicListItem tag = e.Node == null ? (ComicListItem) null : e.Node.Tag as ComicListItem;
      if (tag == null || string.IsNullOrEmpty(e.Label))
        e.CancelEdit = true;
      else
        tag.Name = e.Label;
    }

    private void tvQueries_ItemDrag(object sender, ItemDragEventArgs e)
    {
      if (!this.ComicEditMode.CanEditList())
        return;
      if (e.Button == MouseButtons.Left)
        this.dragNode = e.Item as TreeNode;
      if (this.dragNode != null && this.dragNode.Tag is ComicLibraryListItem)
        this.dragNode = (TreeNode) null;
      if (this.dragNode == null)
        return;
      this.dragCursor = this.treeSkin.GetDragCursor(this.dragNode, (byte) 64, this.tvQueries.PointToClient(Cursor.Position));
      try
      {
        DataObjectEx data = new DataObjectEx();
        data.SetData((object) this.dragNode);
        ShareableComicListItem sc = this.dragNode.Tag as ShareableComicListItem;
        if (sc != null)
          data.SetFile(FileUtility.MakeValidFilename(sc.Name + ".cbl"), (Action<Stream>) (stream =>
          {
            try
            {
              new ComicReadingListContainer((ComicListItem) sc, Program.Settings.ExportedListsContainFilenames).Serialize(stream);
            }
            catch
            {
            }
          }));
        DragDropEffects dragDropEffects = this.tvQueries.DoDragDrop((object) data, DragDropEffects.Copy | DragDropEffects.Move);
        this.OnIdle();
        this.tvQueries.SelectedNode = dragDropEffects == DragDropEffects.None ? this.dragNode : this.FindItemNode((IComicBookListProvider) this.dragNode.Tag);
      }
      finally
      {
        if (this.dragCursor != null)
          this.dragCursor.Dispose();
        this.dragCursor = (IBitmapCursor) null;
        this.dragNode = (TreeNode) null;
      }
    }

    private void tvQueries_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        TreeNode nodeAt = this.tvQueries.GetNodeAt(e.X, e.Y);
        if (nodeAt != null)
        {
          this.tvQueries.SelectedNode = nodeAt;
          this.UpdateBookList();
        }
      }
      this.mouseActivate = true;
    }

    private void tvQueries_AfterExpand(object sender, TreeViewEventArgs e)
    {
      if (!(e.Node.Tag is ComicListItemFolder tag))
        return;
      tag.Collapsed = false;
    }

    private void tvQueries_AfterCollapse(object sender, TreeViewEventArgs e)
    {
      if (!(e.Node.Tag is ComicListItemFolder tag))
        return;
      tag.Collapsed = true;
    }

    private void tvQueries_DrawNode(object sender, DrawTreeNodeEventArgs e)
    {
      if (!(e.Node.Tag is ComicListItem tag) || !tag.PendingCacheUpdate)
        return;
      this.OnListCacheChanged();
    }

    private void treeContextMenu_Opening(object sender, CancelEventArgs e)
    {
      ComicListItem cli = this.tvQueries.SelectedNode.Tag as ComicListItem;
      bool flag = cli != null && Program.Settings.Devices.Count > 0;
      this.cmEditDevices.DropDownItems.Clear();
      this.cmEditDevices.Visible = flag;
      if (!flag)
        return;
      if (Program.Settings.Devices.Count == 1)
      {
        this.cmEditDevices.Text = string.Format(TR.Load(this.Name)["SyncDevice", "Sync with {0}..."], (object) Program.Settings.Devices[0].DeviceName);
        this.cmEditDevices.Tag = (object) cli;
      }
      else
      {
        this.cmEditDevices.Text = TR.Load(this.Name)["SyncDevices", "Sync with"];
        this.cmEditDevices.Tag = (object) null;
        foreach (DeviceSyncSettings device in Program.Settings.Devices)
        {
          ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(device.DeviceName + "...")
          {
            Checked = device.Lists.FirstOrDefault<DeviceSyncSettings.SharedList>((Func<DeviceSyncSettings.SharedList, bool>) (l => l.ListId == cli.Id)) != null
          };
          DeviceSyncSettings dss1 = device;
          toolStripMenuItem.Click += (EventHandler) ((s, ex) =>
          {
            this.Main.ShowPortableDevices(dss1, new Guid?(cli.Id));
            this.tvQueries.Refresh();
          });
          this.cmEditDevices.DropDownItems.Add((ToolStripItem) toolStripMenuItem);
        }
      }
    }

    private void cmEditDevices_Click(object sender, EventArgs e)
    {
      if (!(this.cmEditDevices.Tag is ComicListItem tag))
        return;
      this.Main.ShowPortableDevices(Program.Settings.Devices[0], new Guid?(tag.Id));
      this.tvQueries.Refresh();
    }

    private void ToggleQuickSearch()
    {
      if (!this.quickSearchPanel.Visible)
      {
        this.quickSearchPanel.Show();
        this.quickSearch.Focus();
      }
      else
      {
        this.quickSearch.Text = string.Empty;
        this.quickSearchPanel.Hide();
      }
    }

    private void quickSearch_TextChanged(object sender, EventArgs e) => this.FillListTree();

    private void FillFavorites(bool refreshThumbnails = false)
    {
      if (!this.favContainer.Expanded)
        return;
      this.favView.BeginUpdate();
      try
      {
        this.favView.Items.Clear();
        foreach (ComicListItem comicListItem in this.Library.ComicLists.GetItems<ComicListItem>().Where<ComicListItem>((Func<ComicListItem, bool>) (cl => cl.Favorite)))
        {
          FavoriteViewItem favoriteViewItem = FavoriteViewItem.Create(comicListItem);
          favoriteViewItem.Tag = (object) comicListItem.Id;
          this.favView.Items.Add((IViewableItem) favoriteViewItem);
          if (refreshThumbnails)
            Program.ImagePool.Thumbs.RefreshImage((ImageKey) favoriteViewItem.ThumbnailKey);
        }
      }
      finally
      {
        this.favView.EndUpdate();
      }
    }

    private void RefreshFavorites() => this.FillFavorites(true);

    private void AddToFavorites()
    {
      TreeNode selectedNode = this.tvQueries.SelectedNode;
      if (selectedNode == null || !(selectedNode.Tag is ComicListItem tag))
        return;
      tag.Favorite = true;
    }

    private void RemoveFavorite()
    {
      FavoriteViewItem favoriteViewItem = this.favView.SelectedItems.OfType<FavoriteViewItem>().FirstOrDefault<FavoriteViewItem>();
      if (favoriteViewItem == null)
        return;
      favoriteViewItem.ComicListItem.Favorite = false;
    }

    private void favView_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!(this.favView.FocusedItem is ItemViewItem focusedItem))
        return;
      this.SelectList((Guid) focusedItem.Tag);
    }

    private void favView_Resize(object sender, EventArgs e)
    {
      this.favView.ItemTileSize = new System.Drawing.Size(this.favView.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth - 8, FormUtility.ScaleDpiY(50));
    }

    private void favContainer_ExpandedChanged(object sender, EventArgs e)
    {
      if (!this.favContainer.Expanded)
        return;
      this.FillFavorites();
    }

    private bool CreateDragContainter(DragEventArgs e)
    {
      if (this.dragBookContainer == null)
        this.dragBookContainer = DragDropContainer.Create(e.Data);
      return this.dragBookContainer.IsValid;
    }

    private static void InsertBooksIntoToList(
      IEditableComicBookListProvider list,
      int index,
      DragDropContainer bookContainer)
    {
      if (bookContainer.IsFilesContainer)
      {
        Program.Scanner.ScanFilesOrFolders(bookContainer.FilesOrFolders, false, false);
      }
      else
      {
        if (list == null)
          return;
        foreach (ComicBook book in bookContainer.Books.GetBooks())
        {
          ComicBook comicBook = book.IsLinked ? Program.BookFactory.Create(book.FilePath, CreateBookOption.AddToStorage) : book;
          if (index != -1)
            index = list.Insert(index, comicBook) + 1;
          else
            list.Add(comicBook);
        }
      }
    }

    private void SetDropEffects(DragEventArgs e)
    {
      e.Effect = DragDropEffects.None;
      if (!this.ComicEditMode.CanEditList())
        return;
      this.CreateDragContainter(e);
      System.Drawing.Point client = this.tvQueries.PointToClient(new System.Drawing.Point(e.X, e.Y));
      TreeNode node = this.tvQueries.GetNodeAt(client);
      if (this.dragNode != null)
      {
        if (node != this.dragNode && this.dragNode == e.Data.GetData(typeof (TreeNode)) && this.dragNode.Nodes.Find((Predicate<TreeNode>) (cn => cn == node)) == null)
        {
          e.Effect = !(this.dragNode.Tag is ShareableComicListItem) || (e.KeyState & 8) == 0 ? DragDropEffects.Move : DragDropEffects.Copy;
          System.Drawing.Point point = client;
          if (node != null)
            point.Y -= node.Bounds.Y;
          this.treeSkin.SeparatorDropNodeStyle = node != null && (point.Y >= 0 && point.Y < 4 || !(node.Tag is ComicListItemFolder));
        }
      }
      else if (this.dragBookContainer.IsBookContainer)
      {
        if (node == null || node.Tag is IEditableComicBookListProvider || node.Tag is ComicListItemFolder || node.Tag is ComicSmartListItem)
        {
          e.Effect = e.AllowedEffect;
          this.treeSkin.SeparatorDropNodeStyle = false;
        }
      }
      else if (this.dragBookContainer.IsReadingListsContainer)
      {
        if (node == null || node.Tag is ComicListItemFolder)
        {
          e.Effect = e.AllowedEffect;
          this.treeSkin.SeparatorDropNodeStyle = false;
        }
      }
      else if (this.dragBookContainer.IsFilesContainer)
      {
        IEditableComicBookListProvider tag = node == null ? (IEditableComicBookListProvider) null : node.Tag as IEditableComicBookListProvider;
        if (tag != null && tag.IsLibrary)
          e.Effect = e.AllowedEffect;
        this.treeSkin.SeparatorDropNodeStyle = false;
      }
      this.treeSkin.DropNode = e.Effect == DragDropEffects.None ? (TreeNode) null : node;
    }

    private void tvQueries_DragEnter(object sender, DragEventArgs e) => this.SetDropEffects(e);

    private void tvQueries_DragLeave(object sender, EventArgs e)
    {
      this.treeSkin.DropNode = (TreeNode) null;
      this.dragBookContainer = (DragDropContainer) null;
    }

    private void tvQueries_DragOver(object sender, DragEventArgs e) => this.SetDropEffects(e);

    private void tvQueries_DragDrop(object sender, DragEventArgs e)
    {
      TreeNode dropNode = this.treeSkin.DropNode;
      bool separatorDropNodeStyle = this.treeSkin.SeparatorDropNodeStyle;
      DragDropContainer dragBookContainer = this.dragBookContainer;
      this.treeSkin.DropNode = (TreeNode) null;
      this.dragBookContainer = (DragDropContainer) null;
      if (this.dragNode != null)
      {
        if (this.dragNode != e.Data.GetData(typeof (TreeNode)))
          return;
        ComicListItemCollection listItemCollection1 = this.dragNode.Parent == null ? this.Library.ComicLists : ((ComicListItemFolder) this.dragNode.Parent.Tag).Items;
        ComicListItem data = this.dragNode.Tag as ComicListItem;
        if (e.Effect == DragDropEffects.Copy && data is ShareableComicListItem)
          data = (ComicListItem) ((ICloneable) data).Clone<ShareableComicListItem>();
        if (dropNode == null)
        {
          listItemCollection1.Remove(data);
          this.Library.ComicLists.Add(data);
        }
        else if (separatorDropNodeStyle)
        {
          ComicListItemCollection listItemCollection2 = dropNode.Parent == null ? this.Library.ComicLists : ((ComicListItemFolder) dropNode.Parent.Tag).Items;
          int index = dropNode.Index;
          int num = listItemCollection1.IndexOf(data);
          if (listItemCollection1.Remove(data) && listItemCollection1 == listItemCollection2 && num < index)
            --index;
          listItemCollection2.Insert(index, data);
        }
        else
        {
          listItemCollection1.Remove(data);
          ((ComicListItemFolder) dropNode.Tag).Items.Add(data);
        }
        this.dragNode.Tag = (object) data;
        this.OnIdle();
        this.tvQueries.SelectedNode = this.FindItemNode((IComicBookListProvider) data);
        this.BookList = (IComicBookListProvider) null;
        this.UpdateBookList();
      }
      else if (dragBookContainer.IsBookContainer)
      {
        IEditableComicBookListProvider tag1 = dropNode == null ? (IEditableComicBookListProvider) null : dropNode.Tag as IEditableComicBookListProvider;
        ComicSmartListItem tag2 = dropNode == null ? (ComicSmartListItem) null : dropNode.Tag as ComicSmartListItem;
        ComicListItemFolder tag3 = dropNode == null ? (ComicListItemFolder) null : dropNode.Tag as ComicListItemFolder;
        if (tag1 != null)
          ComicListLibraryBrowser.InsertBooksIntoToList(tag1, -1, dragBookContainer);
        else if (tag2 != null)
        {
          tag2.Matchers.AddRange(dragBookContainer.CreateSeriesGroupMatchers());
          tag2.Refresh();
        }
        else
        {
          ComicListItem comicListItem = Control.ModifierKeys.IsSet<Keys>(Keys.Alt) || dragBookContainer.HasMatcher ? (ComicListItem) dragBookContainer.CreateSeriesSmartList() : (ComicListItem) dragBookContainer.CreateComicIdList();
          if (comicListItem == null)
            return;
          if (string.IsNullOrEmpty(comicListItem.Name))
            comicListItem.Name = TR.Load(this.Name)["NewList", "New List"];
          if (tag3 == null)
            this.Library.ComicLists.Add(comicListItem);
          else
            tag3.Items.Add(comicListItem);
        }
      }
      else if (dragBookContainer.IsReadingListsContainer)
      {
        ComicListItemCollection fc = dropNode == null ? this.Library.ComicLists : (dropNode.Tag is ComicListItemFolder ? ((ComicListItemFolder) dropNode.Tag).Items : (ComicListItemCollection) null);
        using (new WaitCursor((Control) this))
        {
          foreach (string readingList in dragBookContainer.ReadingLists)
            this.ImportList(fc, readingList);
        }
      }
      else
      {
        if (!dragBookContainer.IsFilesContainer)
          return;
        ComicListLibraryBrowser.InsertBooksIntoToList((IEditableComicBookListProvider) null, -1, dragBookContainer);
      }
    }

    private void GiveDragCursorFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if (this.dragCursor == null || this.dragCursor.Cursor == (Cursor) null)
        return;
      e.UseDefaultCursors = false;
      this.dragCursor.OverlayCursor = e.Effect == DragDropEffects.None ? Cursors.No : Cursors.Default;
      this.dragCursor.OverlayEffect = e.Effect == DragDropEffects.Copy ? BitmapCursorOverlayEffect.Plus : BitmapCursorOverlayEffect.None;
      Cursor.Current = this.dragCursor.Cursor;
    }

    private void RenameNode()
    {
      if (this.tvQueries.SelectedNode == null)
        return;
      this.tvQueries.SelectedNode.BeginEdit();
    }

    private void ExpandCollapseAllNodes()
    {
      if (this.tvQueries.AllNodes().Any<TreeNode>((Func<TreeNode, bool>) (t => t.IsExpanded)))
        this.tvQueries.CollapseAll();
      else
        this.tvQueries.ExpandAll();
    }

    private void SortList()
    {
      ComicListItemCollection comicListCollection = this.GetCurrentNodeComicListCollection();
      if (comicListCollection == null)
        return;
      comicListCollection.Sort((Comparison<ComicListItem>) ((a, b) =>
      {
        int num1 = a is ComicListItemFolder ? 1 : 0;
        int num2 = Math.Sign((b is ComicListItemFolder ? 1 : 0) - num1);
        return num2 == 0 ? ExtendedStringComparer.Compare(a.Name, b.Name, ExtendedStringComparison.ZeroesFirst | ExtendedStringComparison.IgnoreArticles | ExtendedStringComparison.IgnoreCase) : num2;
      }));
      this.FillListTree();
    }

    private void NewSmartList()
    {
      ComicListItem currentNodeComicList = this.GetCurrentNodeComicList();
      ComicListItemCollection comicListCollection = this.GetCurrentNodeComicListCollection();
      if (comicListCollection == null)
        return;
      string name = TR.Load(this.Name)[nameof (NewSmartList), "New Smart List"];
      TreeNode selectedNode = this.tvQueries.SelectedNode;
      ComicSmartListItem comicSmartListItem = new ComicSmartListItem(name, string.Empty);
      comicListCollection.Insert(comicListCollection.IndexOf(currentNodeComicList) + 1, (ComicListItem) comicSmartListItem);
      this.FillListTree();
      this.tvQueries.SelectedNode = this.FindItemNode((IComicBookListProvider) comicSmartListItem);
      this.UpdateBookList();
      if (this.EditSelectedComicList())
        return;
      comicListCollection.Remove((ComicListItem) comicSmartListItem);
      this.FillListTree();
      this.tvQueries.SelectedNode = selectedNode;
      this.UpdateBookList();
    }

    private void NewFolder()
    {
      ComicListItem currentNodeComicList = this.GetCurrentNodeComicList();
      ComicListItemCollection comicListCollection = this.GetCurrentNodeComicListCollection();
      if (comicListCollection == null)
        return;
      ComicListItemFolder comicListItemFolder = new ComicListItemFolder(TR.Load(this.Name)[nameof (NewFolder), "New Folder"]);
      if (!EditListDialog.Edit((IWin32Window) this, (ComicListItem) comicListItemFolder))
        return;
      comicListCollection.Insert(comicListCollection.IndexOf(currentNodeComicList) + 1, (ComicListItem) comicListItemFolder);
    }

    private void NewList()
    {
      ComicListItem currentNodeComicList = this.GetCurrentNodeComicList();
      ComicListItemCollection comicListCollection = this.GetCurrentNodeComicListCollection();
      if (comicListCollection == null)
        return;
      ComicIdListItem comicIdListItem = new ComicIdListItem(TR.Load(this.Name)[nameof (NewList), "New List"]);
      if (!EditListDialog.Edit((IWin32Window) this, (ComicListItem) comicIdListItem))
        return;
      comicListCollection.Insert(comicListCollection.IndexOf(currentNodeComicList) + 1, (ComicListItem) comicIdListItem);
    }

    private void CopyList()
    {
      if (this.tvQueries.SelectedNode == null)
        return;
      ComicListItem tag = (ComicListItem) (this.tvQueries.SelectedNode.Tag as ShareableComicListItem);
      if (tag == null && Program.ExtendedSettings.AllowCopyListFolders)
        tag = (ComicListItem) (this.tvQueries.SelectedNode.Tag as ComicListItemFolder);
      if (tag == null)
        return;
      try
      {
        DataObject data = new DataObject();
        if (tag is ComicSmartListItem)
          data.SetText(tag.ToString());
        data.SetData("ComicList", (object) tag);
        Clipboard.SetDataObject((object) data);
      }
      catch (Exception ex)
      {
      }
    }

    private void ExportList()
    {
      if (this.tvQueries.SelectedNode == null || !(this.tvQueries.SelectedNode.Tag is ShareableComicListItem tag))
        return;
      using (SaveFileDialog saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Title = this.miExportReadingList.Text.Replace("&", "");
        saveFileDialog.Filter = TR.Load("FileFilter")["ReadingListSaveFilter", "ComicRack Reading List|*.cbl|ComicRack Reading List (Single Entries)|*.cbl"];
        saveFileDialog.DefaultExt = ".cbl";
        saveFileDialog.FileName = FileUtility.MakeValidFilename(tag.Name);
        foreach (string favoritePath in Program.GetFavoritePaths())
          saveFileDialog.CustomPlaces.Add(favoritePath);
        if (saveFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
          return;
        try
        {
          new ComicReadingListContainer((ComicListItem) tag, Program.Settings.ExportedListsContainFilenames, saveFileDialog.FilterIndex != 1).Serialize(saveFileDialog.FileName);
        }
        catch
        {
          int num = (int) MessageBox.Show(StringUtility.Format(TR.Messages["ErrorWritingReadingList", "There was an error exporting the Reading List '{0}'"], (object) Path.GetFileName(saveFileDialog.FileName)));
        }
      }
    }

    private void PasteList()
    {
      ComicListItem currentNodeComicList = this.GetCurrentNodeComicList();
      ComicListItemCollection comicListCollection = this.GetCurrentNodeComicListCollection();
      if (comicListCollection == null)
        return;
      if (Clipboard.GetData("ComicList") is ShareableComicListItem data2)
      {
        ShareableComicListItem shareableComicListItem = data2.Clone<ShareableComicListItem>();
        if (shareableComicListItem == null)
          return;
        comicListCollection.Insert(comicListCollection.IndexOf(currentNodeComicList) + 1, (ComicListItem) shareableComicListItem);
      }
      else if (Program.ExtendedSettings.AllowCopyListFolders && Clipboard.GetData("ComicList") is ComicListItemFolder data1)
      {
        ComicListItemFolder comicListItemFolder = data1.Clone<ComicListItemFolder>();
        if (comicListItemFolder == null)
          return;
        comicListCollection.Insert(comicListCollection.IndexOf(currentNodeComicList) + 1, (ComicListItem) comicListItemFolder);
      }
      else
      {
        string text = Clipboard.GetText();
        try
        {
          ComicSmartListItem comicSmartListItem = new ComicSmartListItem(TR.Load(this.Name)["NewList", "New List"], text, this.Library);
          comicListCollection.Insert(comicListCollection.IndexOf(currentNodeComicList) + 1, (ComicListItem) comicSmartListItem);
        }
        catch (Exception ex)
        {
          int num = (int) MessageBox.Show((IWin32Window) this, TR.Messages["ErrorPasteQuery", "Could not paste List because of following error:"] + "\n\n" + ex.Message, TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
      }
    }

    private void ImportLists()
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Title = this.miImportReadingList.Text.Replace("&", "");
        openFileDialog.Filter = TR.Load("FileFilter")["ReadingListLoad", "ComicRack Reading List|*.cbl|Xml File|*.xml|All Files|*.*"];
        openFileDialog.CheckFileExists = true;
        openFileDialog.Multiselect = true;
        foreach (string favoritePath in Program.GetFavoritePaths())
          openFileDialog.CustomPlaces.Add(favoritePath);
        if (openFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
          return;
        using (new WaitCursor((Control) this))
        {
          foreach (string fileName in openFileDialog.FileNames)
            this.ImportList(this.GetCurrentNodeComicListCollection(), fileName);
        }
      }
    }

    private void RemoveListOrFolder()
    {
      TreeNode selectedNode = this.tvQueries.SelectedNode;
      if (selectedNode == null || selectedNode.IsEditing)
        return;
      ComicListItemCollection comicListCollection = this.GetNodeComicListCollection(selectedNode.Tag is ComicListItemFolder ? selectedNode.Parent : selectedNode);
      if (comicListCollection == null || !Program.AskQuestion((IWin32Window) this, TR.Messages["AskRemoveItem", "Do you really want to remove this item?"], TR.Messages["Remove", "Remove"], HiddenMessageBoxes.RemoveList) || !comicListCollection.Remove(selectedNode.Tag as ComicListItem) || selectedNode.Parent == null)
        return;
      this.tvQueries.SelectedNode = selectedNode.Parent;
    }

    public void SetWorkspace(DisplayWorkspace ws)
    {
      this.quickSearch.AutoCompleteList.AddRange(Program.Settings.LibraryQuickSearchList.ToArray());
    }

    public void StoreWorkspace(DisplayWorkspace ws)
    {
      if (!this.Disposing)
      {
        if (this.quickSearch != null)
        {
          try
          {
            HashSet<string> collection = new HashSet<string>(this.quickSearch.AutoCompleteList.Cast<string>());
            Program.Settings.LibraryQuickSearchList.Clear();
            Program.Settings.LibraryQuickSearchList.AddRange((IEnumerable<string>) collection);
          }
          catch
          {
          }
        }
      }
      if (!this.ComicEditMode.CanEditList())
        return;
      Program.Settings.LastLibraryItem = this.BookListId;
    }

    public ComicListItem ImportList(string file)
    {
      return this.ImportList((ComicListItemCollection) null, file);
    }

    public ComicListItem ImportList(ComicListItemCollection fc, string file)
    {
      using (new WaitCursor((Control) this))
      {
        try
        {
          ComicReadingListContainer crlc = ComicReadingListContainer.Deserialize(file);
          ComicListItem li = (ComicListItem) null;
          if (crlc.Matchers.Count > 0)
          {
            li = (ComicListItem) new ComicSmartListItem(crlc.Name ?? string.Empty, crlc.MatcherMode, (IEnumerable<ComicBookMatcher>) crlc.Matchers);
          }
          else
          {
            List<ComicBook> newBooks = new List<ComicBook>();
            ComicIdListItem idli = (ComicIdListItem) null;
            AutomaticProgressDialog.Process((IWin32Window) this, TR.Messages["ImportReadingList", "Import Reading List"], TR.Messages["MatchBooksWithLibrary", "Matching list with Library"], 3000, (Action) (() => li = (ComicListItem) (idli = ComicIdListItem.CreateFromReadingList(this.Library.Books, (IEnumerable<ComicReadingListItem>) crlc.Items, (IList<ComicBook>) newBooks, (Func<int, bool>) (x =>
            {
              AutomaticProgressDialog.Value = x;
              return !AutomaticProgressDialog.ShouldAbort;
            })))), AutomaticProgressDialogOptions.EnableCancel);
            if (li == null)
              return (ComicListItem) null;
            li.Name = crlc.Name ?? string.Empty;
            if (newBooks.Count > 0)
            {
              string message = TR.Messages["UnsolvedBookItems", "The following Books were not found in the Library:\n{0}\nDo you still want to import the Reading List?"];
              string empty = string.Empty;
              for (int index = 0; index < Math.Min(newBooks.Count, 25); ++index)
              {
                if (index != 0)
                  empty += "\n";
                empty += string.Format("'{0}'", (object) newBooks[index].Caption);
              }
              if (newBooks.Count > 25)
                empty += "\n...";
              string str = empty + "\n";
              switch (QuestionDialog.AskQuestion((IWin32Window) this, StringUtility.Format(message, (object) str), TR.Default["Import", "Import"], TR.Messages["CreateMissingBooks", "Add missing Books to Library"]))
              {
                case QuestionResult.Cancel:
                  return (ComicListItem) null;
                case QuestionResult.OkWithOption:
                  this.Library.Books.AddRange((IEnumerable<ComicBook>) newBooks);
                  break;
                default:
                  idli.BookIds.RemoveRange(newBooks.Select<ComicBook, Guid>((Func<ComicBook, Guid>) (cb => cb.Id)));
                  break;
              }
            }
          }
          (fc ?? this.Library.TemporaryFolder.Items).Add(li);
          this.FillListTree();
          this.SelectList(li.Id);
          return li;
        }
        catch
        {
          int num = (int) MessageBox.Show(StringUtility.Format(TR.Messages["ErrorOpeningReadingList", "There was an error importing the Reading List '{0}'"], (object) Path.GetFileName(file)));
          return (ComicListItem) null;
        }
      }
    }

    public bool SelectList(Guid listId)
    {
      TreeNode itemNode = this.FindItemNode(listId);
      if (itemNode == null)
        return false;
      this.tvQueries.SelectedNode = itemNode;
      this.UpdateBookList();
      return true;
    }

    public bool CanBrowsePrevious() => this.history.CanMoveCursorPrevious;

    public bool CanBrowseNext() => this.history.CanMoveCursorNext;

    public void BrowsePrevious()
    {
      while (this.history.CanMoveCursorPrevious)
      {
        LinkedListNode<IComicBookListProvider> linkedListNode = this.history.MoveCursorPrevious();
        TreeNode itemNode = linkedListNode != null ? this.FindItemNode(linkedListNode.Value) : (TreeNode) null;
        if (itemNode != null)
        {
          this.tvQueries.SelectedNode = itemNode;
          this.UpdateBookList();
          break;
        }
      }
    }

    public void BrowseNext()
    {
      while (this.history.CanMoveCursorNext)
      {
        LinkedListNode<IComicBookListProvider> linkedListNode = this.history.MoveCursorNext();
        TreeNode itemNode = linkedListNode != null ? this.FindItemNode(linkedListNode.Value) : (TreeNode) null;
        if (itemNode != null)
        {
          this.tvQueries.SelectedNode = itemNode;
          this.UpdateBookList();
          break;
        }
      }
    }

    private void library_ListCachesUpdated(object sender, EventArgs e)
    {
      this.QueueListCacheChange();
    }

    private void queryCacheTimer_Tick(object sender, EventArgs e)
    {
      this.queryCacheTimer.Stop();
      this.CommitListCacheChange();
    }

    private void OnListCacheChanged()
    {
      if (!ComicLibrary.IsQueryCacheEnabled)
        return;
      this.queryCacheTimer.Stop();
      this.queryCacheTimer.Start();
    }

    private void QueueListCacheChange()
    {
      if (!ComicLibrary.IsQueryCacheEnabled)
        return;
      this.AddIdleAction(new Action(this.OnListCacheChanged), nameof (QueueListCacheChange));
    }

    private void CommitListCacheChange()
    {
      if (!this.Visible)
        return;
      this.Library.CommitComicListCacheChanges((Func<ComicListItem, bool>) (cli =>
      {
        TreeNode itemNode = this.FindItemNode((IComicBookListProvider) cli);
        return itemNode != null && itemNode.IsVisible;
      }));
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ComicListLibraryBrowser));
      this.tvQueries = new TreeViewEx();
      this.treeContextMenu = new ContextMenuStrip(this.components);
      this.miEditSmartList = new ToolStripMenuItem();
      this.miQueryRename = new ToolStripMenuItem();
      this.miNodeSort = new ToolStripMenuItem();
      this.miNewSeparator = new ToolStripSeparator();
      this.miNewFolder = new ToolStripMenuItem();
      this.miNewList = new ToolStripMenuItem();
      this.miNewSmartList = new ToolStripMenuItem();
      this.miCopySeparator = new ToolStripSeparator();
      this.miCopyList = new ToolStripMenuItem();
      this.miPasteList = new ToolStripMenuItem();
      this.miExportReadingList = new ToolStripMenuItem();
      this.miImportReadingList = new ToolStripMenuItem();
      this.miOpenSeparator = new ToolStripSeparator();
      this.miOpenWindow = new ToolStripMenuItem();
      this.miOpenTab = new ToolStripMenuItem();
      this.toolStripMenuItem2 = new ToolStripSeparator();
      this.cmEditDevices = new ToolStripMenuItem();
      this.miAddToFavorites = new ToolStripMenuItem();
      this.miRefresh = new ToolStripMenuItem();
      this.miRemoveSeparator = new ToolStripSeparator();
      this.miRemoveListOrFolder = new ToolStripMenuItem();
      this.treeImages = new ImageList(this.components);
      this.toolStrip = new ToolStrip();
      this.tbNewFolder = new ToolStripButton();
      this.tbNewList = new ToolStripButton();
      this.tbNewSmartList = new ToolStripButton();
      this.tssOpenWindow = new ToolStripSeparator();
      this.tbOpenWindow = new ToolStripButton();
      this.tbOpenTab = new ToolStripButton();
      this.tbRefreshSeparator = new ToolStripSeparator();
      this.tbExpandCollapseAll = new ToolStripButton();
      this.tbRefresh = new ToolStripButton();
      this.tbFavorites = new ToolStripButton();
      this.tsQuickSearch = new ToolStripButton();
      this.updateTimer = new Timer(this.components);
      this.favContainer = new SizableContainer();
      this.favView = new ItemView();
      this.contextMenuFavorites = new ContextMenuStrip(this.components);
      this.miFavRefresh = new ToolStripMenuItem();
      this.menuItem1 = new ToolStripSeparator();
      this.miFavRemove = new ToolStripMenuItem();
      this.queryCacheTimer = new Timer(this.components);
      this.quickSearchPanel = new Panel();
      this.quickSearch = new SearchTextBox();
      this.treeContextMenu.SuspendLayout();
      this.toolStrip.SuspendLayout();
      this.favContainer.SuspendLayout();
      this.contextMenuFavorites.SuspendLayout();
      this.quickSearchPanel.SuspendLayout();
      this.SuspendLayout();
      this.tvQueries.AllowDrop = true;
      this.tvQueries.ContextMenuStrip = this.treeContextMenu;
      this.tvQueries.Dock = DockStyle.Fill;
      this.tvQueries.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.tvQueries.FullRowSelect = true;
      this.tvQueries.HideSelection = false;
      this.tvQueries.ImageIndex = 0;
      this.tvQueries.ImageList = this.treeImages;
      this.tvQueries.ItemHeight = 18;
      this.tvQueries.LabelEdit = true;
      this.tvQueries.Location = new System.Drawing.Point(0, 227);
      this.tvQueries.Name = "tvQueries";
      this.tvQueries.SelectedImageIndex = 0;
      this.tvQueries.ShowLines = false;
      this.tvQueries.ShowNodeToolTips = true;
      this.tvQueries.Size = new System.Drawing.Size(397, 193);
      this.tvQueries.TabIndex = 0;
      this.tvQueries.BeforeLabelEdit += new NodeLabelEditEventHandler(this.tvQueries_BeforeLabelEdit);
      this.tvQueries.AfterLabelEdit += new NodeLabelEditEventHandler(this.tvQueries_AfterLabelEdit);
      this.tvQueries.AfterCollapse += new TreeViewEventHandler(this.tvQueries_AfterCollapse);
      this.tvQueries.AfterExpand += new TreeViewEventHandler(this.tvQueries_AfterExpand);
      this.tvQueries.DrawNode += new DrawTreeNodeEventHandler(this.tvQueries_DrawNode);
      this.tvQueries.ItemDrag += new ItemDragEventHandler(this.tvQueries_ItemDrag);
      this.tvQueries.AfterSelect += new TreeViewEventHandler(this.tvQueries_AfterSelect);
      this.tvQueries.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.tvQueries_NodeMouseDoubleClick);
      this.tvQueries.DragDrop += new DragEventHandler(this.tvQueries_DragDrop);
      this.tvQueries.DragEnter += new DragEventHandler(this.tvQueries_DragEnter);
      this.tvQueries.DragOver += new DragEventHandler(this.tvQueries_DragOver);
      this.tvQueries.DragLeave += new EventHandler(this.tvQueries_DragLeave);
      this.tvQueries.GiveFeedback += new GiveFeedbackEventHandler(this.GiveDragCursorFeedback);
      this.tvQueries.MouseDown += new MouseEventHandler(this.tvQueries_MouseDown);
      this.treeContextMenu.Items.AddRange(new ToolStripItem[21]
      {
        (ToolStripItem) this.miEditSmartList,
        (ToolStripItem) this.miQueryRename,
        (ToolStripItem) this.miNodeSort,
        (ToolStripItem) this.miNewSeparator,
        (ToolStripItem) this.miNewFolder,
        (ToolStripItem) this.miNewList,
        (ToolStripItem) this.miNewSmartList,
        (ToolStripItem) this.miCopySeparator,
        (ToolStripItem) this.miCopyList,
        (ToolStripItem) this.miPasteList,
        (ToolStripItem) this.miExportReadingList,
        (ToolStripItem) this.miImportReadingList,
        (ToolStripItem) this.miOpenSeparator,
        (ToolStripItem) this.miOpenWindow,
        (ToolStripItem) this.miOpenTab,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.cmEditDevices,
        (ToolStripItem) this.miAddToFavorites,
        (ToolStripItem) this.miRefresh,
        (ToolStripItem) this.miRemoveSeparator,
        (ToolStripItem) this.miRemoveListOrFolder
      });
      this.treeContextMenu.Name = "treeContextMenu";
      this.treeContextMenu.Size = new System.Drawing.Size(485, 642);
      this.treeContextMenu.Opening += new CancelEventHandler(this.treeContextMenu_Opening);
      this.miEditSmartList.Image = (Image) Resources.EditSearchDocument;
      this.miEditSmartList.Name = "miEditSmartList";
      this.miEditSmartList.Size = new System.Drawing.Size(484, 38);
      this.miEditSmartList.Text = "&Edit...";
      this.miQueryRename.Image = (Image) Resources.Rename;
      this.miQueryRename.Name = "miQueryRename";
      this.miQueryRename.ShortcutKeys = Keys.F2;
      this.miQueryRename.Size = new System.Drawing.Size(484, 38);
      this.miQueryRename.Text = "&Rename";
      this.miNodeSort.Image = (Image) Resources.Sort;
      this.miNodeSort.Name = "miNodeSort";
      this.miNodeSort.Size = new System.Drawing.Size(484, 38);
      this.miNodeSort.Text = "&Sort";
      this.miNewSeparator.Name = "miNewSeparator";
      this.miNewSeparator.Size = new System.Drawing.Size(481, 6);
      this.miNewFolder.Image = (Image) Resources.NewSearchFolder;
      this.miNewFolder.Name = "miNewFolder";
      this.miNewFolder.Size = new System.Drawing.Size(484, 38);
      this.miNewFolder.Text = "New &Folder...";
      this.miNewList.Image = (Image) Resources.NewList;
      this.miNewList.Name = "miNewList";
      this.miNewList.Size = new System.Drawing.Size(484, 38);
      this.miNewList.Text = "New &List...";
      this.miNewSmartList.Image = (Image) Resources.NewSearchDocument;
      this.miNewSmartList.Name = "miNewSmartList";
      this.miNewSmartList.Size = new System.Drawing.Size(484, 38);
      this.miNewSmartList.Text = "&New Smart List...";
      this.miCopySeparator.Name = "miCopySeparator";
      this.miCopySeparator.Size = new System.Drawing.Size(481, 6);
      this.miCopyList.Image = (Image) Resources.EditCopy;
      this.miCopyList.Name = "miCopyList";
      this.miCopyList.ShortcutKeys = Keys.C | Keys.Control;
      this.miCopyList.Size = new System.Drawing.Size(484, 38);
      this.miCopyList.Text = "&Copy List";
      this.miPasteList.Image = (Image) Resources.EditPaste;
      this.miPasteList.Name = "miPasteList";
      this.miPasteList.ShortcutKeys = Keys.V | Keys.Control;
      this.miPasteList.Size = new System.Drawing.Size(484, 38);
      this.miPasteList.Text = "&Paste List";
      this.miExportReadingList.Name = "miExportReadingList";
      this.miExportReadingList.ShortcutKeys = Keys.C | Keys.Shift | Keys.Control;
      this.miExportReadingList.Size = new System.Drawing.Size(484, 38);
      this.miExportReadingList.Text = "Export Reading List...";
      this.miImportReadingList.Name = "miImportReadingList";
      this.miImportReadingList.ShortcutKeys = Keys.V | Keys.Shift | Keys.Control;
      this.miImportReadingList.Size = new System.Drawing.Size(484, 38);
      this.miImportReadingList.Text = "Import Reading List...";
      this.miOpenSeparator.Name = "miOpenSeparator";
      this.miOpenSeparator.Size = new System.Drawing.Size(481, 6);
      this.miOpenWindow.Image = (Image) Resources.NewWindow;
      this.miOpenWindow.Name = "miOpenWindow";
      this.miOpenWindow.Size = new System.Drawing.Size(484, 38);
      this.miOpenWindow.Text = "&Open in New Window";
      this.miOpenTab.Image = (Image) Resources.NewTab;
      this.miOpenTab.Name = "miOpenTab";
      this.miOpenTab.Size = new System.Drawing.Size(484, 38);
      this.miOpenTab.Text = "Open in New Tab";
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(481, 6);
      this.cmEditDevices.Image = (Image) Resources.EditDevices;
      this.cmEditDevices.Name = "cmEditDevices";
      this.cmEditDevices.Size = new System.Drawing.Size(484, 38);
      this.cmEditDevices.Text = "Sync with Devices";
      this.cmEditDevices.Click += new EventHandler(this.cmEditDevices_Click);
      this.miAddToFavorites.Image = (Image) Resources.AddListFavorites;
      this.miAddToFavorites.Name = "miAddToFavorites";
      this.miAddToFavorites.Size = new System.Drawing.Size(484, 38);
      this.miAddToFavorites.Text = "Add to Favorites";
      this.miRefresh.Image = (Image) Resources.Refresh;
      this.miRefresh.Name = "miRefresh";
      this.miRefresh.Size = new System.Drawing.Size(484, 38);
      this.miRefresh.Text = "Re&fresh";
      this.miRemoveSeparator.Name = "miRemoveSeparator";
      this.miRemoveSeparator.Size = new System.Drawing.Size(481, 6);
      this.miRemoveListOrFolder.Image = (Image) Resources.EditDelete;
      this.miRemoveListOrFolder.Name = "miRemoveListOrFolder";
      this.miRemoveListOrFolder.ShortcutKeys = Keys.Delete;
      this.miRemoveListOrFolder.Size = new System.Drawing.Size(484, 38);
      this.miRemoveListOrFolder.Text = "&Re&move...";
      this.treeImages.ColorDepth = ColorDepth.Depth8Bit;
      this.treeImages.ImageSize = new System.Drawing.Size(16, 16);
      this.treeImages.TransparentColor = Color.Transparent;
      this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;
      this.toolStrip.Items.AddRange(new ToolStripItem[11]
      {
        (ToolStripItem) this.tbNewFolder,
        (ToolStripItem) this.tbNewList,
        (ToolStripItem) this.tbNewSmartList,
        (ToolStripItem) this.tssOpenWindow,
        (ToolStripItem) this.tbOpenWindow,
        (ToolStripItem) this.tbOpenTab,
        (ToolStripItem) this.tbRefreshSeparator,
        (ToolStripItem) this.tbExpandCollapseAll,
        (ToolStripItem) this.tbRefresh,
        (ToolStripItem) this.tbFavorites,
        (ToolStripItem) this.tsQuickSearch
      });
      this.toolStrip.Location = new System.Drawing.Point(0, 0);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(397, 39);
      this.toolStrip.TabIndex = 0;
      this.toolStrip.Text = "toolStrip";
      this.tbNewFolder.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbNewFolder.Image = (Image) Resources.NewSearchFolder;
      this.tbNewFolder.Name = "tbNewFolder";
      this.tbNewFolder.Size = new System.Drawing.Size(36, 36);
      this.tbNewFolder.Text = "New &Folder";
      this.tbNewFolder.ToolTipText = "Create a new folder to organize your lists";
      this.tbNewList.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbNewList.Image = (Image) Resources.NewList;
      this.tbNewList.Name = "tbNewList";
      this.tbNewList.Size = new System.Drawing.Size(36, 36);
      this.tbNewList.Text = "New &List";
      this.tbNewList.ToolTipText = "Create a new custom List";
      this.tbNewSmartList.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbNewSmartList.Image = (Image) Resources.NewSearchDocument;
      this.tbNewSmartList.Name = "tbNewSmartList";
      this.tbNewSmartList.Size = new System.Drawing.Size(36, 36);
      this.tbNewSmartList.Text = "&New Smart List";
      this.tbNewSmartList.ToolTipText = "Create a new Smart List";
      this.tssOpenWindow.Name = "tssOpenWindow";
      this.tssOpenWindow.Size = new System.Drawing.Size(6, 39);
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
      this.tbRefreshSeparator.Name = "tbRefreshSeparator";
      this.tbRefreshSeparator.Size = new System.Drawing.Size(6, 39);
      this.tbExpandCollapseAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbExpandCollapseAll.Image = (Image) Resources.ExpandCollapseAll;
      this.tbExpandCollapseAll.ImageTransparentColor = Color.Magenta;
      this.tbExpandCollapseAll.Name = "tbExpandCollapseAll";
      this.tbExpandCollapseAll.Size = new System.Drawing.Size(36, 36);
      this.tbExpandCollapseAll.Text = "Expand/Collapse all";
      this.tbRefresh.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbRefresh.Image = (Image) Resources.Refresh;
      this.tbRefresh.ImageTransparentColor = Color.Magenta;
      this.tbRefresh.Name = "tbRefresh";
      this.tbRefresh.Size = new System.Drawing.Size(36, 36);
      this.tbRefresh.Text = "Refresh";
      this.tbFavorites.Alignment = ToolStripItemAlignment.Right;
      this.tbFavorites.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbFavorites.Image = (Image) Resources.Favorites;
      this.tbFavorites.Name = "tbFavorites";
      this.tbFavorites.Size = new System.Drawing.Size(36, 36);
      this.tbFavorites.Text = "Show Favorites";
      this.tbFavorites.ToolTipText = "Favorites";
      this.tsQuickSearch.Alignment = ToolStripItemAlignment.Right;
      this.tsQuickSearch.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsQuickSearch.Image = (Image) Resources.Search;
      this.tsQuickSearch.ImageTransparentColor = Color.Magenta;
      this.tsQuickSearch.Name = "tsQuickSearch";
      this.tsQuickSearch.Size = new System.Drawing.Size(36, 36);
      this.tsQuickSearch.Text = "Quick Search";
      this.updateTimer.Interval = 500;
      this.updateTimer.Tick += new EventHandler(this.updateTimer_Tick);
      this.favContainer.AutoGripPosition = true;
      this.favContainer.Controls.Add((Control) this.favView);
      this.favContainer.Dock = DockStyle.Top;
      this.favContainer.Grip = SizableContainer.GripPosition.Bottom;
      this.favContainer.Location = new System.Drawing.Point(0, 39);
      this.favContainer.Name = "favContainer";
      this.favContainer.Padding = new Padding(0, 6, 0, 0);
      this.favContainer.Size = new System.Drawing.Size(397, 160);
      this.favContainer.TabIndex = 2;
      this.favContainer.Text = "favContainer";
      this.favContainer.ExpandedChanged += new EventHandler(this.favContainer_ExpandedChanged);
      this.favView.BackColor = SystemColors.Window;
      this.favView.Dock = DockStyle.Fill;
      this.favView.GroupColumns = new IColumn[0];
      this.favView.GroupColumnsKey = (string) null;
      this.favView.GroupsStatus = (ItemViewGroupsStatus) componentResourceManager.GetObject("favView.GroupsStatus");
      this.favView.ItemContextMenuStrip = this.contextMenuFavorites;
      this.favView.ItemViewMode = ItemViewMode.Tile;
      this.favView.Location = new System.Drawing.Point(0, 6);
      this.favView.Multiselect = false;
      this.favView.Name = "favView";
      this.favView.Size = new System.Drawing.Size(397, 148);
      this.favView.SortColumn = (IColumn) null;
      this.favView.SortColumns = new IColumn[0];
      this.favView.SortColumnsKey = (string) null;
      this.favView.StackColumns = new IColumn[0];
      this.favView.StackColumnsKey = (string) null;
      this.favView.TabIndex = 0;
      this.favView.SelectedIndexChanged += new EventHandler(this.favView_SelectedIndexChanged);
      this.favView.Resize += new EventHandler(this.favView_Resize);
      this.contextMenuFavorites.Items.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.miFavRefresh,
        (ToolStripItem) this.menuItem1,
        (ToolStripItem) this.miFavRemove
      });
      this.contextMenuFavorites.Name = "contextMenuFavorites";
      this.contextMenuFavorites.Size = new System.Drawing.Size(268, 86);
      this.miFavRefresh.Image = (Image) Resources.Refresh;
      this.miFavRefresh.Name = "miFavRefresh";
      this.miFavRefresh.Size = new System.Drawing.Size(267, 38);
      this.miFavRefresh.Text = "&Refresh";
      this.menuItem1.Name = "menuItem1";
      this.menuItem1.Size = new System.Drawing.Size(264, 6);
      this.miFavRemove.Image = (Image) Resources.EditDelete;
      this.miFavRemove.Name = "miFavRemove";
      this.miFavRemove.ShortcutKeys = Keys.Delete;
      this.miFavRemove.Size = new System.Drawing.Size(267, 38);
      this.miFavRemove.Text = "&Remove...";
      this.queryCacheTimer.Tick += new EventHandler(this.queryCacheTimer_Tick);
      this.quickSearchPanel.AutoSize = true;
      this.quickSearchPanel.Controls.Add((Control) this.quickSearch);
      this.quickSearchPanel.Dock = DockStyle.Top;
      this.quickSearchPanel.Location = new System.Drawing.Point(0, 199);
      this.quickSearchPanel.Name = "quickSearchPanel";
      this.quickSearchPanel.Padding = new Padding(0, 0, 0, 4);
      this.quickSearchPanel.Size = new System.Drawing.Size(397, 28);
      this.quickSearchPanel.TabIndex = 3;
      this.quickSearchPanel.Visible = false;
      this.quickSearch.ClearButtonImage = (Image) Resources.SmallCloseGray;
      this.quickSearch.Dock = DockStyle.Bottom;
      this.quickSearch.GripStyle = ToolStripGripStyle.Hidden;
      this.quickSearch.Location = new System.Drawing.Point(0, 0);
      this.quickSearch.MinimumSize = new System.Drawing.Size(0, 24);
      this.quickSearch.Name = "quickSearch";
      this.quickSearch.Padding = new Padding(0);
      this.quickSearch.SearchButtonImage = (Image) null;
      this.quickSearch.SearchButtonVisible = false;
      this.quickSearch.Size = new System.Drawing.Size(397, 24);
      this.quickSearch.TabIndex = 0;
      this.quickSearch.TextChanged += new EventHandler(this.quickSearch_TextChanged);
      this.AutoScaleMode = AutoScaleMode.None;
      this.Controls.Add((Control) this.tvQueries);
      this.Controls.Add((Control) this.quickSearchPanel);
      this.Controls.Add((Control) this.favContainer);
      this.Controls.Add((Control) this.toolStrip);
      this.Name = nameof (ComicListLibraryBrowser);
      this.Size = new System.Drawing.Size(397, 420);
      this.treeContextMenu.ResumeLayout(false);
      this.toolStrip.ResumeLayout(false);
      this.toolStrip.PerformLayout();
      this.favContainer.ResumeLayout(false);
      this.contextMenuFavorites.ResumeLayout(false);
      this.quickSearchPanel.ResumeLayout(false);
      this.quickSearchPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private class ViewConfigurationHandler : IDisplayListConfig
    {
      private readonly IDisplayListConfig vc;
      private readonly Guid id;

      public ViewConfigurationHandler(Guid id, IDisplayListConfig vc)
      {
        this.id = id;
        this.vc = vc;
      }

      public DisplayListConfig Display
      {
        get => Program.Settings.GetRemoteViewConfig(this.id, this.vc.Display);
        set => Program.Settings.UpdateRemoteViewConfig(this.id, value);
      }
    }

    private class RemoveBookHandler : IRemoveBooks
    {
      private readonly ComicLibrary library;
      private readonly IComicBookListProvider cbl;
      private readonly IWin32Window parent;

      public RemoveBookHandler(
        IWin32Window parent,
        ComicLibrary library,
        IComicBookListProvider cbl)
      {
        this.parent = parent;
        this.library = library;
        this.cbl = cbl;
      }

      public void RemoveBooks(IEnumerable<ComicBook> books, bool ask)
      {
        books = (IEnumerable<ComicBook>) books.ToArray<ComicBook>();
        IEditableComicBookListProvider cbl1 = this.cbl as IEditableComicBookListProvider;
        IFilteredComicBookList cbl2 = this.cbl as IFilteredComicBookList;
        IBlackList library = this.library as IBlackList;
        Image booksImage = (Image) null;
        try
        {
          string moveToRecycleBin = (string) null;
          if (ask)
          {
            if (books.Any<ComicBook>((Func<ComicBook, bool>) (b => b.IsLinked)))
              moveToRecycleBin = (Program.Settings.MoveFilesToRecycleBin ? "!" : string.Empty) + TR.Messages["MoveBin", "&Also move the files to the Recycle Bin"];
            booksImage = Program.MakeBooksImage(books, new System.Drawing.Size(256, 128), 5, false);
          }
          if (this.cbl is ComicLibraryListItem || cbl1 == null && cbl2 == null)
          {
            deleteFromLibrary = true;
            if (ask)
            {
              QuestionResult questionResult = QuestionDialog.AskQuestion(this.parent, TR.Messages["AskRemoveFromLibrary", "Are you sure you want to remove these books from the library?\nAll information not stored in the files will be lost (like Open Count, Last Read etc.)!"], TR.Messages["Remove", "Remove"], moveToRecycleBin, booksImage);
              if (questionResult == QuestionResult.Cancel)
                return;
              Program.Settings.MoveFilesToRecycleBin = questionResult.HasFlag((Enum) QuestionResult.Option);
            }
          }
          else
          {
            deleteFromLibrary = cbl2 != null ? Program.Settings.AlsoRemoveFromLibraryFiltered : Program.Settings.AlsoRemoveFromLibrary;
            if (ask)
            {
              QuestionResult questionResult = QuestionDialog.AskQuestion(this.parent, TR.Messages["AskRemoveComics", "Are you sure you want to remove these books from the list?"], TR.Messages["Remove", "Remove"], (Action<QuestionDialog>) (qd =>
              {
                qd.OptionText = (deleteFromLibrary ? "!" : string.Empty) + TR.Messages["AlsoRemoveFromLibrary", "&Additionally remove the books from the Library (all information not stored in the files will be lost)"];
                qd.Option2Text = moveToRecycleBin;
                qd.Image = booksImage;
                qd.ShowCancel = true;
              }));
              if (questionResult == QuestionResult.Cancel)
                return;
              deleteFromLibrary = questionResult.HasFlag((Enum) QuestionResult.Option);
              if (cbl2 != null)
                Program.Settings.AlsoRemoveFromLibraryFiltered = deleteFromLibrary;
              else
                Program.Settings.AlsoRemoveFromLibrary = deleteFromLibrary;
              Program.Settings.MoveFilesToRecycleBin = questionResult.HasFlag((Enum) QuestionResult.Option2);
            }
            foreach (ComicBook book in books)
            {
              cbl1?.Remove(book);
              if (cbl2 != null && !deleteFromLibrary)
                cbl2.SetFiltered(book, true);
            }
          }
        }
        finally
        {
          booksImage.SafeDispose();
        }
        bool deleteFromLibrary;
        if (!deleteFromLibrary)
          return;
        bool flag = false;
        using (new WaitCursor())
        {
          foreach (ComicBook book in books)
          {
            if (book.IsLinked)
            {
              if (Program.Settings.MoveFilesToRecycleBin)
              {
                try
                {
                  ShellFile.DeleteFile(this.parent, ShellFileDeleteOptions.None, book.FilePath);
                }
                catch (Exception ex)
                {
                }
                if (File.Exists(book.FilePath))
                {
                  flag = true;
                  continue;
                }
              }
            }
            this.library.Remove(book);
            if (book.IsLinked && library != null)
              library.AddToBlackList(book.FilePath);
          }
        }
        if (!flag)
          return;
        int num = (int) MessageBox.Show(this.parent, TR.Messages["FailedDeleteBooks", "Some files could not be deleted (maybe they are in use)!"], Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
    }
  }
}

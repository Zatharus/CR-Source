// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.MainView
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Display.Forms;
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
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class MainView : SubView, IDisplayWorkspace, IListDisplays
  {
    private ComicExplorerView dbView;
    private ComicExplorerView fileView;
    private ComicPagesView pagesView;
    private readonly TabBar.TabBarItem tsbLibrary = new TabBar.TabBarItem("Library")
    {
      Name = nameof (tsbLibrary),
      Image = (Image) Resources.Library,
      Padding = new Padding(8, 0, 0, 0),
      AdjustWidth = false
    };
    private readonly TabBar.TabBarItem tsbFolders = new TabBar.TabBarItem("Folders")
    {
      Name = nameof (tsbFolders),
      Image = (Image) Resources.FileBrowser,
      Padding = new Padding(0, 0, 8, 0),
      AdjustWidth = false
    };
    private readonly TabBar.TabBarItem tsbPages = new TabBar.TabBarItem("Pages")
    {
      Name = nameof (tsbPages),
      Image = (Image) Resources.ComicPage,
      Padding = new Padding(0, 0, 8, 0),
      AdjustWidth = false
    };
    private readonly CommandMapper commands = new CommandMapper();
    private readonly List<ComicBrowserForm> openBrowsers = new List<ComicBrowserForm>();
    private TabBar.TabBarItem lastBrowser;
    private Control comicViewer;
    private readonly HashSet<string> connectedMachines = new HashSet<string>();
    private readonly VisibilityAnimator tabStripVisibility;
    private IContainer components;
    private TabBar tabStrip;
    private ToolStrip tabToolStrip;
    private ToolStripSplitButton tsbAlignment;
    private ToolStripMenuItem tsbAlignBottom;
    private ToolStripMenuItem tsbAlignLeft;
    private ToolStripMenuItem tsbAlignRight;
    private ToolStripMenuItem tsbAlignFill;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem tsbInfoPanelLeft;

    public MainView()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.InitializeComponent();
      this.tabStrip.Items.AddRange((IEnumerable<TabBar.TabBarItem>) new TabBar.TabBarItem[3]
      {
        this.tsbLibrary,
        this.tsbFolders,
        this.tsbPages
      });
      this.tabStripVisibility = new VisibilityAnimator(this.components, (Control) this.tabStrip);
      this.tsbLibrary.CaptionClick += new CancelEventHandler(this.tab_CaptionClick);
      this.tsbFolders.CaptionClick += new CancelEventHandler(this.tab_CaptionClick);
      this.tsbPages.CaptionClick += new CancelEventHandler(this.tab_CaptionClick);
      LocalizeUtility.Localize((Control) this, this.components);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DesignMode)
        return;
      if (this.ParentForm != null)
        this.ParentForm.Resize += new EventHandler(this.ParentForm_Resize);
      FormUtility.EnableRightClickSplitButtons(this.tabToolStrip.Items);
      this.SuspendLayout();
      try
      {
        this.dbView = this.AddExplorerView((ComicLibrary) Program.Database, (ComicListBrowser) new ComicListLibraryBrowser((ComicLibrary) Program.Database), this.tsbLibrary);
        if (Program.ExtendedSettings.DisableFoldersView)
          this.tabStrip.Items.Remove(this.tsbFolders);
        else
          this.fileView = this.AddExplorerView((ComicLibrary) null, (ComicListBrowser) new ComicListFolderFilesBrowser(Program.Settings.FavoriteFolders), this.tsbFolders);
        this.pagesView = new ComicPagesView();
        this.AddView((Control) this.pagesView, this.tsbPages);
        this.dbView.ComicBrowser.ItemView.UpdateItems();
      }
      finally
      {
        this.ResumeLayout();
      }
      this.commands.Add(new CommandHandler(this.SwitchDocking), (object) this.tsbAlignment);
      this.commands.Add((CommandHandler) (() => this.ViewDock = DockStyle.Left), true, (UpdateHandler) (() => this.ViewDock == DockStyle.Left), (object) this.tsbAlignLeft);
      this.commands.Add((CommandHandler) (() => this.ViewDock = DockStyle.Right), true, (UpdateHandler) (() => this.ViewDock == DockStyle.Right), (object) this.tsbAlignRight);
      this.commands.Add((CommandHandler) (() => this.ViewDock = DockStyle.Bottom), true, (UpdateHandler) (() => this.ViewDock == DockStyle.Bottom), (object) this.tsbAlignBottom);
      this.commands.Add((CommandHandler) (() => this.ViewDock = DockStyle.Fill), true, (UpdateHandler) (() => this.ViewDock == DockStyle.Fill), (object) this.tsbAlignFill);
      this.commands.Add((CommandHandler) (() => this.InfoPanelRight = !this.InfoPanelRight), true, (UpdateHandler) (() => this.InfoPanelRight), (object) this.tsbInfoPanelLeft);
      this.ShowView(Program.Settings.SelectedBrowser);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.openBrowsers.ToArray().Dispose();
        this.tabStrip.Items.Select<TabBar.TabBarItem, ComicExplorerView>((Func<TabBar.TabBarItem, ComicExplorerView>) (tsb => tsb.Tag as ComicExplorerView)).Where<ComicExplorerView>((Func<ComicExplorerView, bool>) (c => c != null && c.Tag != null && c.ComicBrowser.Library != null)).Select<ComicExplorerView, ComicLibrary>((Func<ComicExplorerView, ComicLibrary>) (c => c.ComicBrowser.Library)).Dispose();
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public void ShowLibrary(ComicLibrary library = null)
    {
      if (library == null)
        this.ShowView(this.tsbLibrary);
      else
        this.ShowView(this.tabStrip.Items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (ti => ti.Tag is ComicExplorerView && (ti.Tag as ComicExplorerView).ComicBrowser.Library == library)).FirstOrDefault<TabBar.TabBarItem>() ?? this.tsbLibrary);
    }

    public void ShowFolders() => this.ShowView(this.tsbFolders);

    public void ShowPages() => this.ShowView(this.tsbPages);

    public void ShowLast()
    {
      if (this.lastBrowser != null)
        this.ShowView(this.lastBrowser);
      else
        this.ShowLibrary();
    }

    public void ShowView(int n)
    {
      this.OnGuiVisibility();
      if (n == -1)
        this.ShowLast();
      else
        this.tabStrip.Items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (btn => btn.Visible && object.Equals((object) n, btn.Tag))).ForFirst<TabBar.TabBarItem>((Action<TabBar.TabBarItem>) (btn => this.ShowView(btn)));
    }

    public void ClearFileTabs()
    {
      for (int index = this.tabStrip.Items.Count - 1; index >= 0; --index)
      {
        if (this.tabStrip.Items[index].Tag is int)
          this.tabStrip.Items.RemoveAt(index);
      }
    }

    public void RefreshFileTabs() => this.tabStrip.Refresh();

    public IEnumerable<TabBar.TabBarItem> GetFileTabs(bool withPlus = false)
    {
      return (IEnumerable<TabBar.TabBarItem>) this.tabStrip.Items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (tbi => tbi.Tag is int && (int) tbi.Tag >= 0 | withPlus)).ToArray<TabBar.TabBarItem>();
    }

    public void AddFileTab(TabBar.TabBarItem tsb) => this.tabStrip.Items.Add(tsb);

    public void SetComicViewer(Control c)
    {
      if (c == null)
      {
        this.Controls.Remove(this.comicViewer);
        this.comicViewer = (Control) null;
        this.ShowLibrary();
      }
      else
      {
        this.comicViewer = c;
        this.Controls.Add(c);
        c.BringToFront();
        c.Dock = DockStyle.Fill;
        c.SetBounds(0, 0, c.Parent.Width, c.Parent.Height);
      }
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (this.comicViewer == null || this.comicViewer.Visible)
        return;
      this.comicViewer.SetBounds(0, 0, this.comicViewer.Parent.Width, this.comicViewer.Parent.Height);
    }

    public bool IsComicViewer => this.comicViewer != null && this.comicViewer.Visible;

    public bool IsRemoteConnected(string address)
    {
      using (ItemMonitor.Lock((object) this.connectedMachines))
        return this.connectedMachines.Contains(address);
    }

    public IEnumerable<ComicLibrary> GetLibraries(bool includeRemote = true, bool onlyLocalRemote = false)
    {
      return this.tabStrip.Items.Select<TabBar.TabBarItem, object>((Func<TabBar.TabBarItem, object>) (tb => tb.Tag)).OfType<ComicExplorerView>().Select(cev => new
      {
        Client = cev.Tag as ComicLibraryClient,
        Library = cev.ComicBrowser.Library
      }).Where(cev => cev.Client == null | includeRemote).Where(cev => cev.Client == null || !onlyLocalRemote || cev.Client.ShareInformation.IsLocal).Select(cev => cev.Library);
    }

    public void AddRemoteLibrary(
      ComicLibraryClient client,
      MainView.AddRemoteLibraryOptions options)
    {
      if (!Program.ExtendedSettings.OwnRemoteConnect && (client == null || Program.NetworkManager.RunningServers.Any<ComicLibraryServer>((Func<ComicLibraryServer, bool>) (s => s.Id == client.ShareInformation.Id))))
        return;
      this.SuspendLayout();
      this.RemoveRemoteLibrary(client.ShareInformation.Uri);
      try
      {
        RemoteConnectionView c = new RemoteConnectionView(this, client, options);
        TabBar.TabBarItem tabBarItem = new TabBar.TabBarItem(client.ShareInformation.Name)
        {
          Tag = (object) c,
          Image = client.ShareInformation.IsProtected ? (Image) Resources.RemoteDatabaseLocked : (Image) Resources.RemoteDatabase,
          CanClose = true,
          ToolTipText = client.ShareInformation.Comment
        };
        tabBarItem.CloseClick += (EventHandler) ((x, y) => this.RemoveRemoteLibrary(client.ShareInformation.Uri));
        tabBarItem.CaptionClick += new CancelEventHandler(this.tab_CaptionClick);
        c.Main = this.Main;
        c.Tag = (object) tabBarItem;
        this.tabStrip.Items.Insert(1, tabBarItem);
        this.AddView((Control) c, tabBarItem);
        using (ItemMonitor.Lock((object) this.connectedMachines))
          this.connectedMachines.Add(client.ShareInformation.Uri);
        if (!options.HasFlag((Enum) MainView.AddRemoteLibraryOptions.Select))
          return;
        this.ShowView(tabBarItem);
      }
      finally
      {
        this.ResumeLayout();
      }
    }

    public void OnRefreshRemoteLists(object sender, EventArgs e)
    {
      if (!(sender is ComicListLibraryBrowser listLibraryBrowser))
        return;
      ComicLibraryClient clc = listLibraryBrowser.Tag as ComicLibraryClient;
      if (clc == null)
        return;
      ComicLibrary cl = (ComicLibrary) null;
      AutomaticProgressDialog.Process((IWin32Window) this, TR.Messages["RefreshServer", "Refreshing Server Library"], TR.Messages["GetServerLibraryText", "Retrieving the shared Library from the Server"], 1000, (Action) (() => cl = clc.GetRemoteLibrary()), AutomaticProgressDialogOptions.EnableCancel);
      if (cl == null)
        return;
      listLibraryBrowser.Library = cl;
    }

    public void RemoveRemoteLibrary(string address)
    {
      foreach (TabBar.TabBarItem tsb in (SmartList<TabBar.TabBarItem>) this.tabStrip.Items)
      {
        if (tsb.Tag is RemoteConnectionView)
        {
          RemoteConnectionView tag = tsb.Tag as RemoteConnectionView;
          if (string.Equals(tag.Client.ShareInformation.Uri, address))
          {
            this.RemoveView((Control) tag, tsb);
            this.ShowView(this.tsbLibrary);
            using (ItemMonitor.Lock((object) this.connectedMachines))
            {
              this.connectedMachines.Remove(address);
              break;
            }
          }
        }
        else if (tsb.Tag is ComicExplorerView tag1 && object.Equals((object) address, tag1.Tag))
        {
          Program.Settings.UpdateExplorerViewSetting(tag1.ComicBrowser.Library.Id, tag1.ViewSettings);
          this.tabStrip.Items.Remove(tsb);
          this.Controls.Remove((Control) tag1);
          tag1.ComicBrowser.Library.ComicLists.GetItems<ShareableComicListItem>().ForEach<ShareableComicListItem>((Action<ShareableComicListItem>) (l => this.RemoveList((IComicBookListProvider) l, (object) null)));
          if (tag1.ComicBrowser.Library != null)
            tag1.ComicBrowser.Library.Dispose();
          tag1.Dispose();
          using (ItemMonitor.Lock((object) this.connectedMachines))
            this.connectedMachines.Remove(address);
          this.ShowView(this.tsbLibrary);
          break;
        }
      }
    }

    protected override void OnIdle() => this.OnGuiVisibility();

    protected virtual void OnGuiVisibility()
    {
      if (this.Main == null)
        return;
      foreach (TabBar.TabBarItem fileTab in this.GetFileTabs(true))
      {
        fileTab.Visible = this.Main.BrowserDock == DockStyle.Fill && !this.Main.ReaderUndocked;
        int tag = (int) fileTab.Tag;
        fileTab.FontBold = tag >= 0 && tag == this.Main.OpenBooks.CurrentSlot;
      }
      bool flag = this.Main.OpenBooks.CurrentBook != null;
      if (!flag && this.tabStrip.SelectedTab == this.tsbPages)
        this.tabStrip.SelectedTab = this.tabStrip.Items[0];
      this.tsbPages.Visible = flag;
      this.tsbAlignment.Visible = !this.Main.ReaderUndocked;
      switch (this.ViewDock)
      {
        case DockStyle.Bottom:
          this.tsbAlignment.Image = this.tsbAlignBottom.Image;
          break;
        case DockStyle.Left:
          this.tsbAlignment.Image = this.tsbAlignLeft.Image;
          break;
        case DockStyle.Right:
          this.tsbAlignment.Image = this.tsbAlignRight.Image;
          break;
        case DockStyle.Fill:
          this.tsbAlignment.Image = this.tsbAlignFill.Image;
          break;
      }
    }

    private void tabStrip_SelectedTabChanged(object sender, TabBar.SelectedTabChangedEventArgs e)
    {
      if (!e.Cancel)
        this.ShowView(e.NewItem);
      this.OnTabChanged();
    }

    private void ComicBrowserForm_Disposed(object sender, EventArgs e)
    {
      this.openBrowsers.Remove(sender as ComicBrowserForm);
    }

    private void ComicBrowserForm_Resize(object sender, EventArgs e)
    {
      ComicBrowserForm comicBrowserForm = sender as ComicBrowserForm;
      if (comicBrowserForm.WindowState != FormWindowState.Maximized)
        this.ParentForm.WindowState = comicBrowserForm.WindowState;
      comicBrowserForm.Visible = comicBrowserForm.WindowState != FormWindowState.Minimized;
    }

    private void ParentForm_Resize(object sender, EventArgs e)
    {
      Form form1 = (Form) sender;
      if (form1.WindowState == FormWindowState.Maximized)
        return;
      foreach (Form form2 in this.openBrowsers.OfType<Form>())
      {
        if (form1.WindowState != FormWindowState.Minimized)
          form2.Visible = true;
        form2.WindowState = form1.WindowState;
      }
    }

    private void tab_CaptionClick(object sender, CancelEventArgs e)
    {
      TabBar.TabBarItem tabBarItem = sender as TabBar.TabBarItem;
      IMain main = this.Main;
      if (this.Main == null || !tabBarItem.IsSelected)
        return;
      e.Cancel = true;
      this.Main.ToggleBrowser();
    }

    public event EventHandler TabChanged;

    public event EventHandler ViewAdded;

    public event EventHandler ViewRemoved;

    protected virtual void OnTabChanged()
    {
      if (this.TabChanged == null)
        return;
      this.TabChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnViewAdded()
    {
      if (this.ViewAdded == null)
        return;
      this.ViewAdded((object) this, EventArgs.Empty);
    }

    protected virtual void OnViewRemoved()
    {
      if (this.ViewRemoved == null)
        return;
      this.ViewRemoved((object) this, EventArgs.Empty);
    }

    private DockStyle ViewDock
    {
      get => this.Parent.Dock;
      set => this.Parent.Dock = value;
    }

    public void SwitchDocking()
    {
      switch (this.ViewDock)
      {
        case DockStyle.Bottom:
          this.ViewDock = DockStyle.Left;
          break;
        case DockStyle.Left:
          this.ViewDock = DockStyle.Right;
          break;
        case DockStyle.Right:
          this.ViewDock = DockStyle.Fill;
          break;
        case DockStyle.Fill:
          this.ViewDock = DockStyle.Bottom;
          break;
      }
    }

    public void RefreshView()
    {
      TabBar.TabBarItem selectedTab = this.tabStrip.SelectedTab;
      if (selectedTab == null)
        return;
      foreach (TabBar.TabBarItem tabBarItem in (SmartList<TabBar.TabBarItem>) this.tabStrip.Items)
      {
        if (tabBarItem.Tag is Control tag)
          tag.Visible = tabBarItem == selectedTab;
      }
    }

    private bool ShowView(TabBar.TabBarItem tabStripButton)
    {
      if (!this.tabStrip.Items.Contains(tabStripButton))
        return false;
      this.SuspendLayout();
      try
      {
        TabBar.TabBarItem selectedTab = this.tabStrip.SelectedTab;
        bool flag = tabStripButton.Tag is int;
        if (flag && this.comicViewer != null)
        {
          this.comicViewer.Show();
          this.comicViewer.GetControls<ComicDisplayControl>().FirstOrDefault<ComicDisplayControl>().Focus();
        }
        else
        {
          TabBar.TabBarItem tabBarItem = this.tabStrip.Items.FirstOrDefault<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (t => t == tabStripButton && t.Tag is Control));
          if (tabBarItem != null)
          {
            Control tag = tabBarItem.Tag as Control;
            tag.Show();
            tag.Focus();
          }
        }
        foreach (TabBar.TabBarItem tabBarItem in this.tabStrip.Items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (tsi => tsi != tabStripButton && tsi.Tag is Control)))
          ((Control) tabBarItem.Tag).Hide();
        if (!flag)
        {
          this.lastBrowser = tabStripButton;
          if (this.comicViewer != null)
            this.comicViewer.Hide();
        }
        this.tabStrip.SelectedTab = tabStripButton;
        return selectedTab == tabStripButton;
      }
      catch
      {
        return false;
      }
      finally
      {
        this.ResumeLayout();
      }
    }

    private bool ShowView(string name)
    {
      return this.ShowView(this.tabStrip.Items[name] ?? this.tsbLibrary);
    }

    private Control AddView(Control c, TabBar.TabBarItem tsb)
    {
      this.Controls.Add(c);
      c.Visible = false;
      c.Dock = DockStyle.Fill;
      tsb.Tag = (object) c;
      this.Controls.SetChildIndex(c, 0);
      c.Disposed += (EventHandler) ((s, e) => this.OnViewRemoved());
      this.OnViewAdded();
      return c;
    }

    private void RemoveView(Control c, TabBar.TabBarItem tsb)
    {
      this.tabStrip.Items.Remove(tsb);
      this.Controls.Remove(c);
      c.Dispose();
    }

    public ComicExplorerView AddExplorerView(
      ComicLibrary library,
      ComicListBrowser clb,
      TabBar.TabBarItem tsb,
      ComicExplorerViewSettings settings = null)
    {
      ComicExplorerView ev = new ComicExplorerView()
      {
        ComicListBrowser = clb
      };
      ev.ComicBrowser.Library = library;
      ScriptUtility.CreateComicInfoPages().ForEach<ComicPageControl>((Action<ComicPageControl>) (cip => ((ISidebar) ev).AddInfo(cip)));
      ev.ViewSettings = settings;
      this.AddView((Control) ev, tsb);
      return ev;
    }

    private static void SetBackgroundImage(ComicExplorerView view, Image img)
    {
      view.ComicBrowser.ListBackgroundImage = img;
    }

    public TabBar TabBar => this.tabStrip;

    public bool TabBarVisible
    {
      get => this.tabStripVisibility.Visible;
      set => this.tabStripVisibility.Visible = value;
    }

    public bool IsComicVisible
    {
      get => this.tabStrip.SelectedTab != null && this.tabStrip.SelectedTab.Tag is int;
    }

    public bool InfoPanelRight
    {
      get
      {
        ISidebar activeService = this.FindActiveService<ISidebar>();
        return activeService != null && activeService.InfoBrowserRight;
      }
      set
      {
        ISidebar activeService = this.FindActiveService<ISidebar>();
        if (activeService == null)
          return;
        activeService.InfoBrowserRight = value;
      }
    }

    public void SetWorkspace(DisplayWorkspace workspace)
    {
      this.SuspendLayout();
      try
      {
        workspace.DatabaseView.ItemViewConfig = (ItemViewConfig) null;
        this.dbView.ViewSettings = workspace.DatabaseView;
        if (this.fileView == null)
          return;
        this.fileView.ViewSettings = workspace.FileView;
      }
      finally
      {
        this.ResumeLayout(true);
      }
    }

    public void StoreWorkspace(DisplayWorkspace ws)
    {
      if (ws == null || ws.DatabaseView == null || this.dbView == null)
        return;
      ws.DatabaseView = this.dbView.ViewSettings;
      if (this.fileView != null)
        ws.FileView = this.fileView.ViewSettings;
      ws.DatabaseView.ItemViewConfig = (ItemViewConfig) null;
      if (this.tabStrip == null || this.tabStrip.SelectedTab == null)
        return;
      Program.Settings.SelectedBrowser = this.tabStrip.SelectedTab.Name;
    }

    public void AddListWindow(Image windowIcon, IComicBookListProvider bookList)
    {
      ComicBrowserForm comicBrowserForm1 = new ComicBrowserForm();
      comicBrowserForm1.Text = bookList.Name;
      comicBrowserForm1.ShowInTaskbar = false;
      ComicBrowserForm comicBrowserForm2 = comicBrowserForm1;
      comicBrowserForm2.Disposed += new EventHandler(this.ComicBrowserForm_Disposed);
      comicBrowserForm2.Resize += new EventHandler(this.ComicBrowserForm_Resize);
      comicBrowserForm2.Show();
      comicBrowserForm2.Main = this.Main;
      comicBrowserForm2.BookList = bookList;
      this.openBrowsers.Add(comicBrowserForm2);
    }

    public void AddListTab(Image tabImage, IComicBookListProvider bookList)
    {
      this.SuspendLayout();
      try
      {
        string name = NumberedString.StripNumber(bookList.Name);
        int number = NumberedString.MaxNumber(this.tabStrip.Items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (tb => NumberedString.StripNumber(tb.Text) == name)).Select<TabBar.TabBarItem, string>((Func<TabBar.TabBarItem, string>) (tb => tb.Text)));
        TabBar.TabBarItem tsb = new TabBar.TabBarItem(NumberedString.Format(name, number));
        ComicBrowserView c = new ComicBrowserView();
        Bitmap bitmap = tabImage.Clone() as Bitmap;
        bitmap.ToGrayScale();
        tsb.Image = (Image) bitmap;
        tsb.Tag = (object) c;
        tsb.CanClose = true;
        tsb.CloseClick += (EventHandler) ((x, y) => this.RemoveList(bookList, (object) tsb));
        tsb.CaptionClick += new CancelEventHandler(this.tab_CaptionClick);
        this.tabStrip.Items.Insert(1, tsb);
        EventHandler rename = (EventHandler) ((s, e) => tsb.Text = bookList.Name);
        bookList.NameChanged += rename;
        tsb.Removed += (EventHandler) ((s, e) => bookList.NameChanged -= rename);
        this.AddView((Control) c, tsb);
        this.ShowView(tsb);
        c.Main = this.Main;
        c.BookList = bookList;
      }
      finally
      {
        this.ResumeLayout();
      }
    }

    public void RemoveList(IComicBookListProvider bookList, object hint)
    {
      bool flag = false;
      foreach (ComicBrowserForm comicBrowserForm in this.openBrowsers.ToArray())
      {
        if ((hint == null || comicBrowserForm == hint) && comicBrowserForm.BookList == bookList)
          comicBrowserForm.SafeDispose();
      }
      foreach (TabBar.TabBarItem tabBarItem in this.tabStrip.Items.ToArray())
      {
        Control tag = (Control) (tabBarItem.Tag as ComicBrowserView);
        if (tag != null && (hint == null || tabBarItem == hint || tag == hint))
        {
          this.tabStrip.Items.Remove(tabBarItem);
          this.Controls.Remove(tag);
          tag.Dispose();
          flag = true;
        }
      }
      if (!flag)
        return;
      this.ShowLibrary(bookList is IComicLibraryItem ? (bookList as IComicLibraryItem).Library : (ComicLibrary) null);
    }

    private void InitializeComponent()
    {
      this.tabStrip = new TabBar();
      this.tabToolStrip = new ToolStrip();
      this.tsbAlignment = new ToolStripSplitButton();
      this.tsbAlignBottom = new ToolStripMenuItem();
      this.tsbAlignLeft = new ToolStripMenuItem();
      this.tsbAlignRight = new ToolStripMenuItem();
      this.tsbAlignFill = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.tsbInfoPanelLeft = new ToolStripMenuItem();
      this.tabStrip.SuspendLayout();
      this.tabToolStrip.SuspendLayout();
      this.SuspendLayout();
      this.tabStrip.AllowDrop = true;
      this.tabStrip.BackColor = SystemColors.Control;
      this.tabStrip.BottomPadding = 0;
      this.tabStrip.CloseImage = Resources.Close;
      this.tabStrip.Controls.Add((Control) this.tabToolStrip);
      this.tabStrip.Dock = DockStyle.Top;
      this.tabStrip.Location = new System.Drawing.Point(0, 0);
      this.tabStrip.Name = "tabStrip";
      this.tabStrip.OwnerDrawnTooltips = true;
      this.tabStrip.Size = new System.Drawing.Size(895, 25);
      this.tabStrip.TabIndex = 0;
      this.tabStrip.Text = "tabStrip";
      this.tabStrip.TopPadding = 0;
      this.tabStrip.SelectedTabChanged += new EventHandler<TabBar.SelectedTabChangedEventArgs>(this.tabStrip_SelectedTabChanged);
      this.tabToolStrip.BackColor = Color.Transparent;
      this.tabToolStrip.CanOverflow = false;
      this.tabToolStrip.Dock = DockStyle.Right;
      this.tabToolStrip.GripStyle = ToolStripGripStyle.Hidden;
      this.tabToolStrip.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.tsbAlignment
      });
      this.tabToolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
      this.tabToolStrip.Location = new System.Drawing.Point(860, 1);
      this.tabToolStrip.Name = "tabToolStrip";
      this.tabToolStrip.Size = new System.Drawing.Size(35, 23);
      this.tabToolStrip.TabIndex = 1;
      this.tsbAlignment.Alignment = ToolStripItemAlignment.Right;
      this.tsbAlignment.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsbAlignment.DropDownItems.AddRange(new ToolStripItem[6]
      {
        (ToolStripItem) this.tsbAlignBottom,
        (ToolStripItem) this.tsbAlignLeft,
        (ToolStripItem) this.tsbAlignRight,
        (ToolStripItem) this.tsbAlignFill,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.tsbInfoPanelLeft
      });
      this.tsbAlignment.Image = (Image) Resources.AlignBottom;
      this.tsbAlignment.ImageTransparentColor = Color.Magenta;
      this.tsbAlignment.Name = "tsbAlignment";
      this.tsbAlignment.Overflow = ToolStripItemOverflow.Never;
      this.tsbAlignment.Size = new System.Drawing.Size(32, 20);
      this.tsbAlignment.Text = "Docking Mode";
      this.tsbAlignBottom.Alignment = ToolStripItemAlignment.Right;
      this.tsbAlignBottom.Checked = true;
      this.tsbAlignBottom.CheckState = CheckState.Checked;
      this.tsbAlignBottom.Image = (Image) Resources.AlignBottom;
      this.tsbAlignBottom.ImageTransparentColor = Color.Magenta;
      this.tsbAlignBottom.Name = "tsbAlignBottom";
      this.tsbAlignBottom.ShortcutKeys = Keys.D1 | Keys.Shift | Keys.Control;
      this.tsbAlignBottom.Size = new System.Drawing.Size(230, 22);
      this.tsbAlignBottom.Text = "Dock Bottom";
      this.tsbAlignLeft.Alignment = ToolStripItemAlignment.Right;
      this.tsbAlignLeft.Image = (Image) Resources.AlignLeft;
      this.tsbAlignLeft.ImageTransparentColor = Color.Magenta;
      this.tsbAlignLeft.Name = "tsbAlignLeft";
      this.tsbAlignLeft.ShortcutKeys = Keys.D2 | Keys.Shift | Keys.Control;
      this.tsbAlignLeft.Size = new System.Drawing.Size(230, 22);
      this.tsbAlignLeft.Text = "Dock Left";
      this.tsbAlignRight.Alignment = ToolStripItemAlignment.Right;
      this.tsbAlignRight.Image = (Image) Resources.AlignRight;
      this.tsbAlignRight.ImageTransparentColor = Color.Magenta;
      this.tsbAlignRight.Name = "tsbAlignRight";
      this.tsbAlignRight.ShortcutKeys = Keys.D3 | Keys.Shift | Keys.Control;
      this.tsbAlignRight.Size = new System.Drawing.Size(230, 22);
      this.tsbAlignRight.Text = "Dock Right";
      this.tsbAlignFill.Alignment = ToolStripItemAlignment.Right;
      this.tsbAlignFill.Image = (Image) Resources.AlignFill;
      this.tsbAlignFill.ImageTransparentColor = Color.Magenta;
      this.tsbAlignFill.Name = "tsbAlignFill";
      this.tsbAlignFill.ShortcutKeys = Keys.D4 | Keys.Shift | Keys.Control;
      this.tsbAlignFill.Size = new System.Drawing.Size(230, 22);
      this.tsbAlignFill.Text = "Fill";
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(227, 6);
      this.tsbInfoPanelLeft.Name = "tsbInfoPanelLeft";
      this.tsbInfoPanelLeft.ShortcutKeys = Keys.D5 | Keys.Shift | Keys.Control;
      this.tsbInfoPanelLeft.Size = new System.Drawing.Size(230, 22);
      this.tsbInfoPanelLeft.Text = "Info Panel Right";
      this.AutoScaleMode = AutoScaleMode.None;
      this.BackColor = SystemColors.Control;
      this.Controls.Add((Control) this.tabStrip);
      this.Name = nameof (MainView);
      this.Size = new System.Drawing.Size(895, 551);
      this.tabStrip.ResumeLayout(false);
      this.tabStrip.PerformLayout();
      this.tabToolStrip.ResumeLayout(false);
      this.tabToolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    [Flags]
    public enum AddRemoteLibraryOptions
    {
      None = 0,
      Open = 1,
      Select = 2,
      Auto = 4,
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.MainForm
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Net;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Engine.Display.Forms;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Network;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Engine.Sync;
using cYo.Projects.ComicRack.Plugins;
using cYo.Projects.ComicRack.Plugins.Automation;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Dialogs;
using cYo.Projects.ComicRack.Viewer.Menus;
using cYo.Projects.ComicRack.Viewer.Properties;
using cYo.Projects.ComicRack.Viewer.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  [ComVisible(true)]
  public class MainForm : Form, IMain, IContainerControl, IPluginConfig, IApplication, IBrowser
  {
    private readonly CommandMapper commands = new CommandMapper();
    private readonly ToolStripThumbSize thumbSize = new ToolStripThumbSize();
    private string[] recentFiles = new string[0];
    private readonly VisibilityAnimator mainMenuStripVisibility;
    private readonly VisibilityAnimator fileTabsVisibility;
    private readonly VisibilityAnimator statusStripVisibility;
    private EnumMenuUtility pageTypeContextMenu;
    private EnumMenuUtility pageTypeEditMenu;
    private EnumMenuUtility pageRotationContextMenu;
    private EnumMenuUtility pageRotationEditMenu;
    private readonly KeyboardShortcuts mainKeys = new KeyboardShortcuts();
    private bool menuDown;
    private ComicDisplay comicDisplay;
    private bool autoHideMainMenu;
    private bool showMainMenuNoComicOpen = true;
    private bool menuClose;
    private ComicBook[] lastRandomList = new ComicBook[0];
    private List<ComicBook> randomSelectedComics;
    private float lastZoom = 2f;
    private string lastWorkspaceName;
    private WorkspaceType lastWorkspaceType = WorkspaceType.Default;
    private ReaderForm readerForm;
    private DockStyle savedBrowserDockStyle;
    private bool savedBrowserVisible;
    private Rectangle undockedReaderBounds;
    private FormWindowState undockedReaderState;
    private bool shieldReaderFormClosing;
    private Image addTabImage = (Image) Resources.AddTab;
    private Image emptyTabImage = (Image) Resources.Original;
    private TasksDialog taskDialog;
    private static Image SinglePageRtl = (Image) Resources.SinglePageRtl;
    private static Image TwoPagesRtl = (Image) Resources.TwoPageForcedRtl;
    private static Image TwoPagesAdaptiveRtl = (Image) Resources.TwoPageRtl;
    private bool enableAutoHideMenu = true;
    private long menuAutoClosed;
    private static readonly string None = TR.Default[nameof (None), nameof (None)];
    private static readonly string NotAvailable = TR.Default[nameof (NotAvailable), "NA"];
    private static readonly string ExportingComics = TR.Load(typeof (MainForm).Name)[nameof (ExportingComics), "Exporting Books: {0} queued"];
    private static readonly string ExportingErrors = TR.Load(typeof (MainForm).Name)[nameof (ExportingErrors), "{0} errors. Click for details"];
    private static readonly string DeviceSyncing = TR.Load(typeof (MainForm).Name)[nameof (DeviceSyncing), "Syncing Devices: {0} queued"];
    private static readonly string DeviceSyncingErrors = TR.Load(typeof (MainForm).Name)[nameof (DeviceSyncingErrors), "{0} errors. Click for details"];
    private static readonly Image exportErrorAnimation = (Image) Resources.ExportAnimationWithError;
    private static readonly Image exportAnimation = (Image) Resources.ExportAnimation;
    private static readonly Image exportError = (Image) Resources.ExportError;
    private static readonly Image deviceSyncErrorAnimation = (Image) Resources.DeviceSyncAnimationWithError;
    private static readonly Image deviceSyncAnimation = (Image) Resources.DeviceSyncAnimation;
    private static readonly Image deviceSyncError = (Image) Resources.DeviceSyncError;
    private static readonly Image zoomImage = (Image) Resources.Zoom;
    private static readonly Image zoomClearImage = (Image) Resources.ZoomClear;
    private static readonly Image updatePages = (Image) Resources.UpdatePages;
    private static readonly Image greenLight = (Image) Resources.GreenLight;
    private static readonly Image grayLight = (Image) Resources.GrayLight;
    private static readonly Image trackPagesLockedImage = (Image) Resources.Locked;
    private static readonly Image datasourceConnected = (Image) Resources.DataSourceConnected;
    private static readonly Image datasourceDisconnected = (Image) Resources.DataSourceDisconnected;
    private bool maximized;
    private bool shieldTray;
    private readonly NavigatorManager books;
    private bool minimalGui;
    private bool quickUpdateRegistered;
    private IEnumerable<ShareableComicListItem> defaultQuickOpenLists;
    private bool quickListDirty;
    private IContainer components;
    private System.Windows.Forms.Timer mouseDisableTimer;
    private MenuStrip mainMenuStrip;
    private ToolStripMenuItem fileMenu;
    private ToolStripMenuItem miOpenComic;
    private ToolStripSeparator toolStripMenuItem14;
    private ToolStripMenuItem miAddFolderToLibrary;
    private ToolStripMenuItem miScan;
    private ToolStripSeparator toolStripInsertSeperator;
    private ToolStripMenuItem miOpenRecent;
    private ToolStripSeparator toolStripMenuItem4;
    private ToolStripMenuItem miExit;
    private ToolStripMenuItem editMenu;
    private ToolStripMenuItem miShowInfo;
    private ToolStripMenuItem miRating;
    private ToolStripSeparator toolStripSeparator4;
    private ToolStripMenuItem miPreferences;
    private ToolStripMenuItem readMenu;
    private ToolStripMenuItem miFirstPage;
    private ToolStripMenuItem miPrevPage;
    private ToolStripMenuItem miNextPage;
    private ToolStripMenuItem miLastPage;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem miPrevFromList;
    private ToolStripMenuItem miNextFromList;
    private ToolStripMenuItem displayMenu;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripSeparator toolStripMenuItem41;
    private ToolStripMenuItem helpMenu;
    private ToolStripMenuItem miWebHome;
    private ToolStripMenuItem miWebUserForum;
    private ToolStripSeparator toolStripMenuItem5;
    private ToolStripMenuItem miAbout;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel tsText;
    private ToolStripStatusLabel tsScanActivity;
    private ToolStripStatusLabel tsBook;
    private ToolStripStatusLabel tsCurrentPage;
    private ToolStripStatusLabel tsPageCount;
    private MainView mainView;
    private ToolStripMenuItem miZoom;
    private ToolStripMenuItem miRotation;
    private ToolStripMenuItem miRotate0;
    private ToolStripMenuItem miRotate90;
    private ToolStripMenuItem miRotate180;
    private ToolStripMenuItem miRotate270;
    private ContextMenuStrip pageContextMenu;
    private ToolStripMenuItem cmShowInfo;
    private ToolStripMenuItem cmRating;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem cmCopyPage;
    private ToolStripSeparator toolStripSeparator10;
    private ToolStripMenuItem cmRefreshPage;
    private NotifyIcon notifyIcon;
    private ToolStripMenuItem miNews;
    private ToolStripMenuItem miMagnify;
    private ToolStripMenuItem cmMagnify;
    private ToolStripMenuItem miFileAutomation;
    private ToolStripSeparator toolStripMenuItem40;
    private ToolStripMenuItem miViewRefresh;
    private ToolStripMenuItem browseMenu;
    private ToolStripMenuItem miViewLibrary;
    private ToolStripMenuItem miViewFolders;
    private ToolStripMenuItem miViewPages;
    private ToolStripSeparator toolStripMenuItem9;
    private ToolStripMenuItem miSidebar;
    private ToolStripMenuItem miSmallPreview;
    private ToolStripMenuItem miSearchBrowser;
    private ToolStripMenuItem miFullScreen;
    private ToolStripMenuItem miToggleBrowser;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripStatusLabel tsWriteInfoActivity;
    private ToolStripStatusLabel tsExportActivity;
    private ToolStripMenuItem cmPageType;
    private ToolStripMenuItem miSyncBrowser;
    private SizableContainer mainViewContainer;
    private ToolStripSeparator toolStripMenuItem11;
    private ContextMenuStrip notfifyContextMenu;
    private ToolStripMenuItem cmNotifyRestore;
    private ToolStripSeparator toolStripMenuItem15;
    private ToolStripMenuItem cmNotifyExit;
    private TabBar fileTabs;
    private ToolStripMenuItem miCloseComic;
    private ToolStripMenuItem miCloseAllComics;
    private ToolStripSeparator toolStripMenuItem7;
    private ToolStripMenuItem miAddTab;
    private Panel viewContainer;
    private ToolStripMenuItem miOpenNow;
    private ToolStripSeparator toolStripMenuItem17;
    private ToolStripMenuItem miPrevTab;
    private ToolStripMenuItem miNextTab;
    private ToolStripStatusLabel tsPageActivity;
    private ToolStripMenuItem cmExportPage;
    private ToolStripSeparator toolStripMenuItem6;
    private ToolStripMenuItem miListLayouts;
    private ToolStripMenuItem miEditListLayout;
    private ToolStripMenuItem miSaveListLayout;
    private ToolStripMenuItem miEditLayouts;
    private ToolStripMenuItem miSetAllListsSame;
    private ToolStripSeparator miLayoutSep;
    private ToolStripMenuItem cmPageLayout;
    private ToolStripMenuItem cmFitAll;
    private ToolStripMenuItem cmFitWidth;
    private ToolStripMenuItem cmFitHeight;
    private ToolStripMenuItem cmFitBest;
    private ToolStripMenuItem miRestart;
    private ToolStripSeparator toolStripMenuItem24;
    private ToolStripMenuItem miSupport;
    private ToolStripSeparator toolStripMenuItem25;
    private ToolStripMenuItem miReaderUndocked;
    private Panel panelReader;
    private Panel readerContainer;
    private ToolStripMenuItem miPageLayout;
    private ToolStripMenuItem miFitAll;
    private ToolStripMenuItem miFitWidth;
    private ToolStripMenuItem miFitHeight;
    private ToolStripMenuItem miBestFit;
    private ToolStripSeparator toolStripMenuItem29;
    private ToolStripMenuItem miWorkspaces;
    private ToolStripMenuItem miSaveWorkspace;
    private ToolStripMenuItem miEditWorkspaces;
    private ToolStripSeparator miWorkspaceSep;
    private ToolStripMenuItem miOriginal;
    private ToolStripMenuItem cmOriginal;
    private ToolStripMenuItem cmFitWidthAdaptive;
    private ToolStripMenuItem miFitWidthAdaptive;
    private ToolStripMenuItem miOnlyFitOversized;
    private ToolStripMenuItem cmOnlyFitOversized;
    private ToolStripMenuItem miAutoScroll;
    private ToolStripMenuItem miRightToLeft;
    private ToolStripMenuItem miDoublePageAutoScroll;
    private ToolStripMenuItem cmRightToLeft;
    private ToolStripSeparator toolStripMenuItem27;
    private ToolStripMenuItem miMinimalGui;
    private ToolStripMenuItem cmMinimalGui;
    private ToolStripMenuItem miWebHelp;
    private ToolStripSeparator toolStripMenuItem3;
    private ToolStripSeparator toolStripMenuItem18;
    private ToolStripMenuItem cmBookmarks;
    private ToolStripMenuItem cmSetBookmark;
    private ToolStripMenuItem cmRemoveBookmark;
    private ToolStripSeparator toolStripSeparator13;
    private ToolStripMenuItem cmLastPageRead;
    private ToolStripSeparator toolStripMenuItem23;
    private ToolStripSeparator toolStripMenuItem32;
    private ToolStripMenuItem cmPrevBookmark;
    private ToolStripMenuItem cmNextBookmark;
    private ToolStripMenuItem miZoomIn;
    private ToolStripMenuItem miZoomOut;
    private ToolStripSeparator toolStripSeparator14;
    private ToolStripMenuItem miZoom100;
    private ToolStripMenuItem miZoom125;
    private ToolStripMenuItem miZoom150;
    private ToolStripMenuItem miZoom200;
    private ToolStripMenuItem miZoom400;
    private ToolStripSeparator toolStripSeparator15;
    private ToolStripMenuItem miZoomCustom;
    private ToolStripMenuItem miRotateLeft;
    private ToolStripMenuItem miRotateRight;
    private ToolStripSeparator toolStripMenuItem33;
    private ToolStripSeparator toolStripMenuItem36;
    private ToolStripMenuItem miAutoRotate;
    private ContextMenuStrip tabContextMenu;
    private ToolStripMenuItem cmClose;
    private ToolStripMenuItem cmCloseAllButThis;
    private ToolStripSeparator toolStripMenuItem35;
    private ToolStripMenuItem cmCopyPath;
    private ToolStripMenuItem cmRevealInExplorer;
    private ToolStripMenuItem cmSyncBrowser;
    private ToolStripMenuItem miTasks;
    private ToolStripStatusLabel tsReadInfoActivity;
    private ToolStripSeparator toolStripMenuItem22;
    private ToolStripMenuItem miPageType;
    private ToolStripMenuItem miBookmarks;
    private ToolStripMenuItem miSetBookmark;
    private ToolStripMenuItem miRemoveBookmark;
    private ToolStripSeparator toolStripMenuItem26;
    private ToolStripMenuItem miPrevBookmark;
    private ToolStripMenuItem miNextBookmark;
    private ToolStripSeparator toolStripMenuItem8;
    private ToolStripMenuItem miLastPageRead;
    private ToolStripMenuItem miCopyPage;
    private ToolStripMenuItem miExportPage;
    private ToolStripSeparator toolStripMenuItem39;
    private ToolStripSeparator sepBeforeRevealInBrowser;
    private ToolStripMenuItem cmComics;
    private ToolStripMenuItem cmOpenComic;
    private ToolStripMenuItem cmCloseComic;
    private ToolStripSeparator toolStripMenuItem13;
    private ToolStripMenuItem cmPrevFromList;
    private ToolStripMenuItem cmNextFromList;
    private ToolStripMenuItem cmPageRotate;
    private ToolStripSeparator toolStripMenuItem38;
    private ToolStripMenuItem cmRotate0;
    private ToolStripMenuItem cmRotate90;
    private ToolStripMenuItem cmRotate180;
    private ToolStripMenuItem cmRotate270;
    private ToolStripMenuItem miPageRotate;
    private ToolStripSeparator toolStripMenuItem42;
    private ToolStripMenuItem miOpenRemoteLibrary;
    private ToolStripMenuItem miRandomFromList;
    private ToolStripMenuItem cmRandomFromList;
    private ToolStripMenuItem miUpdateAllComicFiles;
    private ToolStripSeparator toolStripMenuItem43;
    private ToolStripMenuItem miUndo;
    private ToolStripMenuItem miRedo;
    private System.Windows.Forms.Timer trimTimer;
    private ToolStripSeparator toolStripMenuItem44;
    private ToolStripMenuItem miComicDisplaySettings;
    private ToolStrip mainToolStrip;
    private ToolStripSplitButton tbFit;
    private ToolStripMenuItem tbOriginal;
    private ToolStripMenuItem tbFitAll;
    private ToolStripMenuItem tbFitWidth;
    private ToolStripMenuItem tbFitWidthAdaptive;
    private ToolStripMenuItem tbFitHeight;
    private ToolStripMenuItem tbBestFit;
    private ToolStripSeparator toolStripMenuItem20;
    private ToolStripMenuItem tbOnlyFitOversized;
    private ToolStripSplitButton tbZoom;
    private ToolStripMenuItem tbZoomIn;
    private ToolStripMenuItem tbZoomOut;
    private ToolStripSeparator toolStripMenuItem30;
    private ToolStripMenuItem tbZoom100;
    private ToolStripMenuItem tbZoom125;
    private ToolStripMenuItem tbZoom150;
    private ToolStripMenuItem tbZoom200;
    private ToolStripMenuItem tbZoom400;
    private ToolStripSeparator toolStripMenuItem31;
    private ToolStripMenuItem tbZoomCustom;
    private ToolStripSplitButton tbRotate;
    private ToolStripMenuItem tbRotateLeft;
    private ToolStripMenuItem tbRotateRight;
    private ToolStripSeparator toolStripSeparator11;
    private ToolStripMenuItem tbRotate0;
    private ToolStripMenuItem tbRotate90;
    private ToolStripMenuItem tbRotate180;
    private ToolStripMenuItem tbRotate270;
    private ToolStripSeparator toolStripMenuItem34;
    private ToolStripMenuItem tbAutoRotate;
    private ToolStripSeparator toolStripSeparator7;
    private ToolStripSplitButton tbMagnify;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripDropDownButton tbTools;
    private ToolStripSeparator toolStripSeparator5;
    private ToolStripSplitButton tbPrevPage;
    private ToolStripMenuItem tbFirstPage;
    private ToolStripMenuItem tbPrevFromList;
    private ToolStripSplitButton tbNextPage;
    private ToolStripMenuItem tbLastPage;
    private ToolStripMenuItem tbNextFromList;
    private ToolStripSeparator toolStripMenuItem49;
    private ToolStripMenuItem tbRandomFromList;
    private ContextMenuStrip toolsContextMenu;
    private ToolStripMenuItem tbOpenComic;
    private ToolStripMenuItem tbShowInfo;
    private ToolStripSeparator toolStripMenuItem47;
    private ToolStripMenuItem tsWorkspaces;
    private ToolStripMenuItem tsSaveWorkspace;
    private ToolStripMenuItem tsEditWorkspaces;
    private ToolStripSeparator tsWorkspaceSep;
    private ToolStripMenuItem tbBookmarks;
    private ToolStripMenuItem tbSetBookmark;
    private ToolStripMenuItem tbRemoveBookmark;
    private ToolStripSeparator tbBookmarkSeparator;
    private ToolStripMenuItem tbAutoScroll;
    private ToolStripSeparator toolStripMenuItem45;
    private ToolStripMenuItem tbMinimalGui;
    private ToolStripMenuItem tbReaderUndocked;
    private ToolStripSeparator toolStripMenuItem52;
    private ToolStripMenuItem tbScan;
    private ToolStripMenuItem tbUpdateAllComicFiles;
    private ToolStripSeparator toolStripMenuItem48;
    private ToolStripMenuItem tbComicDisplaySettings;
    private ToolStripMenuItem tbPreferences;
    private ToolStripMenuItem tbAbout;
    private ToolStripSeparator toolStripMenuItem50;
    private ToolStripMenuItem tbShowMainMenu;
    private ToolStripSeparator toolStripMenuItem51;
    private ToolStripMenuItem tbExit;
    private ToolStripSeparator toolStripMenuItem46;
    private ToolStripMenuItem tbPrevBookmark;
    private ToolStripSeparator toolStripMenuItem19;
    private ToolStripMenuItem tbNextBookmark;
    private ToolStripMenuItem tbLastPageRead;
    private ToolStripSeparator cmBookmarkSeparator;
    private ToolStripSeparator toolStripMenuItem28;
    private ToolStripSeparator toolStripMenuItem53;
    private System.Windows.Forms.Timer updateActivityTimer;
    private ToolStripSplitButton tbPageLayout;
    private ToolStripMenuItem tbTwoPagesAdaptive;
    private ToolStripMenuItem tbSinglePage;
    private ToolStripMenuItem tbTwoPages;
    private ToolStripSeparator toolStripMenuItem54;
    private ToolStripMenuItem tbRightToLeft;
    private ToolStripMenuItem miTwoPagesAdaptive;
    private ToolStripMenuItem miTwoPages;
    private ToolStripMenuItem miSinglePage;
    private ToolStripMenuItem cmSinglePage;
    private ToolStripMenuItem cmTwoPages;
    private ToolStripMenuItem cmTwoPagesAdaptive;
    private ToolStripSeparator toolStripMenuItem55;
    private ToolStripMenuItem tbSupport;
    private ToolStripSeparator toolStripMenuItem56;
    private ToolStripMenuItem miPreviousList;
    private ToolStripMenuItem miNextList;
    private ToolStripMenuItem miUpdateWebComics;
    private ToolStripMenuItem tbUpdateWebComics;
    private ToolStripMenuItem tbOpenRemoteLibrary;
    private ToolStripMenuItem miInfoPanel;
    private ToolStripStatusLabel tsServerActivity;
    private ToolStripSeparator toolStripMenuItem37;
    private ToolStripMenuItem miNewComic;
    private ToolStripMenuItem miToggleZoom;
    private ToolStripSeparator toolStripMenuItem57;
    private ToolStripSeparator toolStripMenuItem10;
    private QuickOpenView quickOpenView;
    private ToolStripButton tbFullScreen;
    private ContextMenuStrip contextRating;
    private ToolStripMenuItem miRate0;
    private ToolStripSeparator toolStripMenuItem12;
    private ToolStripMenuItem miRate1;
    private ToolStripMenuItem miRate2;
    private ToolStripMenuItem miRate3;
    private ToolStripMenuItem miRate4;
    private ToolStripMenuItem miRate5;
    private ContextMenuStrip contextRating2;
    private ToolStripMenuItem cmRate0;
    private ToolStripSeparator toolStripMenuItem16;
    private ToolStripMenuItem cmRate1;
    private ToolStripMenuItem cmRate2;
    private ToolStripMenuItem cmRate3;
    private ToolStripMenuItem cmRate4;
    private ToolStripMenuItem cmRate5;
    private ToolStripMenuItem cmCloseAllToTheRight;
    private ToolStripSeparator cmComicsSep;
    private ToolStripMenuItem miHelp;
    private ToolStripMenuItem miChooseHelpSystem;
    private ToolStripMenuItem miHelpPlugins;
    private ToolStripMenuItem miHelpQuickIntro;
    private ToolStripMenuItem miDevices;
    private ToolStripMenuItem miSynchronizeDevices;
    private ToolStripStatusLabel tsDeviceSyncActivity;
    private ToolStripMenuItem tsSynchronizeDevices;
    private ToolStripSeparator toolStripMenuItem21;
    private ToolStripMenuItem miTrackCurrentPage;
    private ToolStripStatusLabel tsDataSourceState;
    private ToolStripSeparator toolStripMenuItem58;
    private ToolStripMenuItem miQuickRating;
    private ToolStripSeparator toolStripSeparator6;
    private ToolStripMenuItem cmQuickRating;

    public MainForm()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.Size = this.Size.ScaleDpi();
      this.statusStrip.Height = (int) this.tsText.Font.GetHeight() + FormUtility.ScaleDpiY(8);
      SystemEvents.DisplaySettingsChanging += (EventHandler) ((s, e) => this.StoreWorkspace());
      SystemEvents.DisplaySettingsChanged += (EventHandler) ((s, e) => this.SetWorkspaceDisplayOptions(Program.Settings.CurrentWorkspace));
      if (Program.ExtendedSettings.DisableFoldersView)
        this.miViewFolders.GetCurrentParent().Items.Remove((ToolStripItem) this.miViewFolders);
      this.notifyIcon.MouseDoubleClick += new MouseEventHandler(this.NotifyIconMouseDoubleClick);
      FormUtility.EnableRightClickSplitButtons(this.mainToolStrip.Items);
      this.AllowDrop = true;
      this.DragDrop += new DragEventHandler(this.BookDragDrop);
      this.DragEnter += new DragEventHandler(this.BookDragEnter);
      this.ComicDisplay.FirstPageReached += new EventHandler(this.viewer_FirstPageReached);
      this.ComicDisplay.LastPageReached += new EventHandler(this.viewer_LastPageReached);
      this.ComicDisplay.FullScreenChanged += new EventHandler(this.ViewerFullScreenChanged);
      this.ComicDisplay.PageChanged += new EventHandler<BookPageEventArgs>(this.ComicDisplay_PageChanged);
      this.books = new NavigatorManager((IComicDisplay) this.ComicDisplay);
      this.books.BookOpened += new EventHandler<BookEventArgs>(this.OnBookOpened);
      this.books.BookClosed += new EventHandler<BookEventArgs>(this.OnBookClosed);
      this.books.BookClosing += new EventHandler<BookEventArgs>(this.OnBookClosing);
      this.books.Slots.Changed += new EventHandler<SmartListChangedEventArgs<ComicBookNavigator>>(this.OpenBooks_SlotsChanged);
      this.books.CurrentSlotChanged += new EventHandler(this.OpenBooks_CurrentSlotChanged);
      this.books.OpenComicsChanged += new EventHandler(this.OpenBooks_CaptionsChanged);
      this.components.Add((IComponent) this.commands);
      this.tbZoom.Width = 60;
      this.fileTabs.Visible = false;
      DropDownHost<MagnifySetupControl> dropDownHost = new DropDownHost<MagnifySetupControl>();
      this.ComicDisplay.MagnifierOpacity = dropDownHost.Control.MagnifyOpaque = Program.Settings.MagnifyOpaque;
      this.ComicDisplay.MagnifierSize = dropDownHost.Control.MagnifySize = Program.Settings.MagnifySize;
      this.ComicDisplay.MagnifierZoom = dropDownHost.Control.MagnifyZoom = Program.Settings.MagnifyZoom;
      this.ComicDisplay.MagnifierStyle = dropDownHost.Control.MagnifyStyle = Program.Settings.MagnifyStyle;
      this.ComicDisplay.AutoMagnifier = dropDownHost.Control.AutoMagnifier = Program.Settings.AutoMagnifier;
      this.ComicDisplay.AutoHideMagnifier = dropDownHost.Control.AutoHideMagnifier = Program.Settings.AutoHideMagnifier;
      dropDownHost.Control.ValuesChanged += new EventHandler(this.MagnifySetupChanged);
      this.tbMagnify.DropDown = (ToolStripDropDown) dropDownHost;
      this.mainMenuStripVisibility = new VisibilityAnimator(this.components, (Control) this.mainMenuStrip);
      this.fileTabsVisibility = new VisibilityAnimator(this.components, (Control) this.fileTabs);
      this.statusStripVisibility = new VisibilityAnimator(this.components, (Control) this.statusStrip);
      LocalizeUtility.Localize((Control) this, this.components);
      this.quickOpenView.Caption = TR.Load(this.Name)[this.quickOpenView.Name, this.quickOpenView.Caption];
      Program.StartupProgress(TR.Messages["InitScripts", "Initializing Scripts"], 70);
      if (ScriptUtility.Initialize((IWin32Window) this, (IApplication) this, (IBrowser) this, (IComicDisplay) this.ComicDisplay, (IPluginConfig) this, (IOpenBooksManager) this.OpenBooks))
      {
        this.miFileAutomation.DropDownItems.AddRange((ToolStripItem[]) ScriptUtility.CreateToolItems<ToolStripMenuItem>((Control) this, "Library", (Func<IEnumerable<ComicBook>>) (() => (IEnumerable<ComicBook>) Program.Database.Books)).ToArray<ToolStripMenuItem>());
        this.miFileAutomation.Visible = this.miFileAutomation.DropDownItems.Count != 0;
        int num = this.fileMenu.DropDownItems.IndexOf((ToolStripItem) this.miNewComic);
        foreach (ToolStripMenuItem toolStripMenuItem in ScriptUtility.CreateToolItems<ToolStripMenuItem>((Control) this, "NewBooks", (Func<IEnumerable<ComicBook>>) (() => (IEnumerable<ComicBook>) Program.Database.Books)).ToArray<ToolStripMenuItem>())
          this.fileMenu.DropDownItems.Insert(++num, (ToolStripItem) toolStripMenuItem);
        foreach (Command command in ScriptUtility.Scripts.GetCommands("DrawThumbnailOverlay"))
        {
          Command sc = command;
          sc.PreCompile();
          CoverViewItem.DrawCustomThumbnailOverlay += (CoverViewItem.DrawCustomThumbnailOverlayHandler) ((comic, graphics, bounds, flags) => sc.Invoke(new object[4]
          {
            (object) comic,
            (object) graphics,
            (object) bounds,
            (object) flags
          }, true));
        }
      }
      Program.StartupProgress(TR.Messages["InitGUI", "Initializing User Interface"], 80);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        IdleProcess.Idle -= new EventHandler(this.Application_Idle);
        Program.Database.BookChanged -= new EventHandler<ContainerBookChangedEventArgs>(this.WatchedBookHasChanged);
        Program.BookFactory.TemporaryBookChanged -= new EventHandler<ContainerBookChangedEventArgs>(this.WatchedBookHasChanged);
        this.books.BookOpened -= new EventHandler<BookEventArgs>(this.OnBookOpened);
        this.books.Slots.Changed -= new EventHandler<SmartListChangedEventArgs<ComicBookNavigator>>(this.OpenBooks_SlotsChanged);
        this.books.Slots.ForEach((Action<ComicBookNavigator>) (n => n.SafeDispose()));
        this.books.Slots.Clear();
        Program.Settings.SettingsChanged -= new EventHandler(this.SettingsChanged);
        this.tsScanActivity.Visible = this.tsReadInfoActivity.Visible = this.tsWriteInfoActivity.Visible = this.tsPageActivity.Visible = this.tsExportActivity.Visible = false;
        if (this.comicDisplay != null)
          this.comicDisplay.Dispose();
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void UpdateSettings()
    {
      this.ComicDisplay.MouseWheelSpeed = Program.Settings.MouseWheelSpeed;
      this.ComicDisplay.ImageDisplayOptions = Program.Settings.PageImageDisplayOptions;
      this.ComicDisplay.SmoothScrolling = Program.Settings.SmoothScrolling;
      this.ComicDisplay.BlendWhilePaging = Program.Settings.BlendWhilePaging;
      this.ComicDisplay.InfoOverlayScaling = (float) Program.Settings.OverlayScaling / 100f;
      this.ComicDisplay.SetInfoOverlays(InfoOverlays.PartInfo, Program.Settings.ShowVisiblePagePartOverlay);
      this.ComicDisplay.SetInfoOverlays(InfoOverlays.CurrentPage, Program.Settings.ShowCurrentPageOverlay);
      this.ComicDisplay.SetInfoOverlays(InfoOverlays.LoadPage, Program.Settings.ShowStatusOverlay);
      this.ComicDisplay.SetInfoOverlays(InfoOverlays.PageBrowser, Program.Settings.ShowNavigationOverlay);
      this.ComicDisplay.SetInfoOverlays(InfoOverlays.PageBrowserOnTop, Program.Settings.NavigationOverlayOnTop);
      this.ComicDisplay.SetInfoOverlays(InfoOverlays.CurrentPageShowsName, Program.Settings.CurrentPageShowsName);
      this.ComicDisplay.HideCursorFullScreen = Program.Settings.HideCursorFullScreen;
      this.ComicDisplay.AutoScrolling = Program.Settings.AutoScrolling;
      this.ComicDisplay.PageWallTicks = Program.Settings.PageChangeDelay ? 300L : 0L;
      this.ComicDisplay.ScrollingDoesBrowse = Program.Settings.ScrollingDoesBrowse;
      this.ComicDisplay.ResetZoomOnPageChange = Program.Settings.ResetZoomOnPageChange;
      this.ComicDisplay.ZoomInOutOnPageChange = Program.Settings.ZoomInOutOnPageChange;
      this.ComicDisplay.RightToLeftReadingMode = Program.Settings.RightToLeftReadingMode;
      this.ComicDisplay.LeftRightMovementReversed = Program.Settings.LeftRightMovementReversed;
      this.ComicDisplay.DisplayChangeAnimation = Program.Settings.DisplayChangeAnimation;
      this.ComicDisplay.FlowingMouseScrolling = Program.Settings.FlowingMouseScrolling;
      this.ComicDisplay.SoftwareFiltering = Program.Settings.SoftwareFiltering;
      this.ComicDisplay.HardwareFiltering = Program.Settings.HardwareFiltering;
      this.ComicDisplay.SetRenderer(Program.Settings.HardwareAcceleration);
      foreach (ComicBookNavigator slot in this.OpenBooks.Slots)
      {
        if (slot != null)
          slot.BaseColorAdjustment = Program.Settings.GlobalColorAdjustment;
      }
      this.AutoHideMainMenu = Program.Settings.AutoHideMainMenu;
      this.ShowMainMenuNoComicOpen = Program.Settings.ShowMainMenuNoComicOpen;
      this.quickOpenView.ThumbnailSize = Program.Settings.QuickOpenThumbnailSize;
      ComicBookNavigator.TrackCurrentPage = Program.Settings.TrackCurrentPage;
      this.tsCurrentPage.Image = ComicBookNavigator.TrackCurrentPage ? (Image) null : (Image) Resources.Locked;
      CoverViewItem.ThumbnailSizing = Program.Settings.CoverThumbnailsSameSize ? CoverThumbnailSizing.Fit : CoverThumbnailSizing.None;
      ComicBook.NewBooksChecked = Program.Settings.NewBooksChecked;
      DeviceSyncFactory.SetExtraWifiDeviceAddresses(EngineConfiguration.Default.ExtraWifiDeviceAddresses + "," + Program.Settings.ExtraWifiDeviceAddresses);
    }

    private void SettingsChanged(object sender, EventArgs e) => this.UpdateSettings();

    private void OnBookOpened(object sender, BookEventArgs e)
    {
      if (Program.Settings.TrackCurrentPage)
        e.Book.OpenedTime = DateTime.Now;
      e.Book.NewPages = 0;
      this.recentFiles = Program.Database.GetRecentFiles(20).ToArray<string>();
      if (e.Book.EditMode.IsLocalComic())
        Win7.UpdateRecent(e.Book.FilePath);
      this.UpdateBrowserVisibility();
      ScriptUtility.Invoke("BookOpened", (object) e.Book);
      string url = e.Book.FilePath;
      Win7.AddTabbedThumbnail((IWin32Window) this, e.Book.FilePath, (Action) (() => this.books.CurrentSlot = this.books.Slots.FindIndex<ComicBookNavigator>((Predicate<ComicBookNavigator>) (s => s.Comic.FilePath == url))), (Action) (() => this.books.Close(this.books.Slots.FindIndex<ComicBookNavigator>((Predicate<ComicBookNavigator>) (s => s.Comic.FilePath == url)))), (Func<Bitmap>) (() =>
      {
        ComicBookNavigator comicBookNavigator = this.books.Slots.FirstOrDefault<ComicBookNavigator>((Func<ComicBookNavigator, bool>) (s => s.Comic.FilePath == url));
        return comicBookNavigator == this.books.CurrentBook ? this.ComicDisplay.CreateThumbnail() : comicBookNavigator.Thumbnail;
      }));
    }

    private void OnBookClosing(object sender, BookEventArgs e)
    {
      if (!Program.Settings.AutoShowQuickReview || e.Book == null || !e.Book.HasBeenRead || (double) e.Book.Rating != 0.0)
        return;
      new MainForm.RatingEditor((IWin32Window) (Form.ActiveForm ?? (Form) this), ListExtensions.AsEnumerable<ComicBook>(e.Book)).QuickRatingAndReview();
    }

    private void OnBookClosed(object sender, BookEventArgs e)
    {
      Program.ImagePool.SlowPageQueue.RemoveItems<PageKey>((Predicate<PageKey>) (k => k.Location == e.Book.FilePath));
      Program.ImagePool.FastPageQueue.RemoveItems<PageKey>((Predicate<PageKey>) (k => k.Location == e.Book.FilePath));
      Program.QueueManager.AddBookToFileUpdate(e.Book);
      Win7.RemoveThumbnail(e.Book.FilePath);
    }

    private void ComicDisplay_PageChanged(object sender, BookPageEventArgs e)
    {
      Win7.InvalidateThumbnail(this.OpenBooks.CurrentBook.Comic.FilePath);
    }

    private void UpdateBrowserVisibility()
    {
      if (this.ReaderUndocked)
        return;
      if (this.BrowserDock == DockStyle.Fill)
      {
        this.mainView.ShowView(this.books.CurrentSlot);
      }
      else
      {
        if (!Program.Settings.CloseBrowserOnOpen)
          return;
        this.BrowserVisible = false;
      }
    }

    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 28 && m.WParam.ToInt32() == 0)
        this.ComicDisplay.FullScreen = false;
      base.WndProc(ref m);
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);
      Win7.Initialize();
      DonationDialog.Show((IWin32Window) (Form.ActiveForm ?? (Form) this), false);
      if (string.IsNullOrEmpty(Program.ExtendedSettings.InstallPlugin))
        return;
      this.ShowPreferences(Program.ExtendedSettings.InstallPlugin);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      Program.Database.BookChanged += new EventHandler<ContainerBookChangedEventArgs>(this.WatchedBookHasChanged);
      Program.BookFactory.TemporaryBookChanged += new EventHandler<ContainerBookChangedEventArgs>(this.WatchedBookHasChanged);
      Program.Database.Books.ForEach(new Action<ComicBook>(Program.QueueManager.AddBookToFileUpdate));
      if (Program.Settings.ScanStartup)
        Program.Scanner.ScanFilesOrFolders(Program.Database.WatchFolders.Folders, true, Program.Settings.RemoveMissingFilesOnFullScan);
      if (Program.Settings.UpdateWebComicsStartup)
        this.UpdateWebComics();
      this.SuspendLayout();
      this.Icon = Resources.ComicRackAppSmall;
      this.notifyIcon.Icon = this.Icon;
      this.notifyIcon.Text = LocalizeUtility.GetText((Control) this, "NotifyIconText", this.notifyIcon.Text);
      this.miFileAutomation.Visible = this.miFileAutomation.DropDownItems.Count != 0;
      this.mainMenuStrip.SendToBack();
      this.statusStrip.Items.Insert(this.statusStrip.Items.Count - 1, (ToolStripItem) this.thumbSize);
      this.thumbSize.TrackBar.Scroll += new EventHandler(this.TrackBar_Scroll);
      ThumbRenderer.DefaultRatingImage1 = Resources.StarYellow.ToOptimized();
      ThumbRenderer.DefaultRatingImage2 = Resources.StarBlue.ToOptimized();
      ThumbRenderer.DefaultTagRatingImage1 = Resources.RatingYellow.ToOptimized();
      ThumbRenderer.DefaultTagRatingImage2 = Resources.RatingBlue.ToOptimized();
      this.ComicDisplay.PagePool = (IPagePool) Program.ImagePool;
      this.ComicDisplay.ThumbnailPool = (IThumbnailPool) Program.ImagePool;
      this.ComicDisplay.PageFilter = Program.Settings.PageFilter;
      this.mainView.Main = (IMain) this;
      this.miOpenRecent.DropDownItems.Add((ToolStripItem) new ToolStripMenuItem("dummy"));
      IdleProcess.Idle += new EventHandler(this.Application_Idle);
      Program.Settings.SettingsChanged += new EventHandler(this.SettingsChanged);
      this.recentFiles = Program.Database.GetRecentFiles(20).ToArray<string>();
      this.InitializeCommands();
      this.InitializeKeyboard();
      this.InitializeHelp(Program.Settings.HelpSystem);
      Program.Settings.HelpSystemChanged += (EventHandler) ((s, ex) => this.InitializeHelp(Program.Settings.HelpSystem));
      this.InitializePluginHelp();
      this.UpdateSettings();
      this.UpdateWorkspaceMenus();
      this.SetWorkspace(Program.Settings.GetWorkspace(Program.ExtendedSettings.Workspace) ?? Program.Settings.CurrentWorkspace, false);
      this.UpdateListConfigMenus();
      this.pageTypeContextMenu = new EnumMenuUtility((ToolStripDropDownItem) this.cmPageType, typeof (ComicPageType), false, (IDictionary<int, Image>) null, Keys.A | Keys.Shift | Keys.Alt);
      this.pageTypeEditMenu = new EnumMenuUtility((ToolStripDropDownItem) this.miPageType, typeof (ComicPageType), false, (IDictionary<int, Image>) null, Keys.A | Keys.Shift | Keys.Alt);
      this.pageTypeContextMenu.ValueChanged += (EventHandler) ((s, a) => this.GetPageEditor().PageType = (ComicPageType) this.pageTypeContextMenu.Value);
      this.pageTypeEditMenu.ValueChanged += (EventHandler) ((s, a) => this.GetPageEditor().PageType = (ComicPageType) this.pageTypeEditMenu.Value);
      Dictionary<int, Image> images = new Dictionary<int, Image>()
      {
        {
          0,
          (Image) Resources.Rotate0Permanent
        },
        {
          1,
          (Image) Resources.Rotate90Permanent
        },
        {
          2,
          (Image) Resources.Rotate180Permanent
        },
        {
          3,
          (Image) Resources.Rotate270Permanent
        }
      };
      this.pageRotationContextMenu = new EnumMenuUtility((ToolStripDropDownItem) this.cmPageRotate, typeof (ImageRotation), false, (IDictionary<int, Image>) images, Keys.D6 | Keys.Shift | Keys.Alt);
      this.pageRotationEditMenu = new EnumMenuUtility((ToolStripDropDownItem) this.miPageRotate, typeof (ImageRotation), false, (IDictionary<int, Image>) images, Keys.D6 | Keys.Shift | Keys.Alt);
      this.pageRotationContextMenu.ValueChanged += (EventHandler) ((s, a) => this.GetPageEditor().Rotation = (ImageRotation) this.pageRotationContextMenu.Value);
      this.pageRotationEditMenu.ValueChanged += (EventHandler) ((s, a) => this.GetPageEditor().Rotation = (ImageRotation) this.pageRotationEditMenu.Value);
      this.ResumeLayout(true);
      this.contextRating.Items.Insert(this.contextRating.Items.Count - 2, (ToolStripItem) new ToolStripSeparator());
      RatingControl.InsertRatingControl(this.contextRating, this.contextRating.Items.Count - 2, (Image) Resources.StarYellow, new Func<IEditRating>(this.GetRatingEditor));
      this.contextRating2.Items.Insert(this.contextRating2.Items.Count - 2, (ToolStripItem) new ToolStripSeparator());
      RatingControl.InsertRatingControl(this.contextRating2, this.contextRating2.Items.Count - 2, (Image) Resources.StarYellow, new Func<IEditRating>(this.GetRatingEditor));
      this.contextRating.Renderer = (ToolStripRenderer) new MenuRenderer((Image) Resources.StarYellow);
      this.contextRating2.Renderer = (ToolStripRenderer) new MenuRenderer((Image) Resources.StarYellow);
      IdleProcess.CancelIdle += (CancelEventHandler) ((a, b) => b.Cancel = !IdleProcess.ShouldProcess((Form) this) && !IdleProcess.ShouldProcess((Form) this.readerForm));
      Program.StartupProgress(TR.Messages["LoadComic", "Opening Files"], 90);
      this.Refresh();
      foreach (string commandLineFile in Program.CommandLineFiles)
      {
        if (File.Exists(commandLineFile))
          this.OpenSupportedFile(commandLineFile, fromShell: true);
      }
      if (this.books.OpenCount == 0 && Program.Settings.OpenLastFile)
        this.books.Open((IEnumerable<string>) new List<string>((IEnumerable<string>) Program.Settings.LastOpenFiles), OpenComicOptions.NoIncreaseOpenedCount | OpenComicOptions.AppendNewSlots);
      if (Program.Settings.ShowQuickManual)
      {
        Program.Settings.ShowQuickManual = false;
        this.books.Open(Program.QuickHelpManualFile, OpenComicOptions.OpenInNewSlot | OpenComicOptions.NoFileUpdate);
      }
      if (!string.IsNullOrEmpty(Program.ExtendedSettings.ImportList))
        this.ImportComicList(Program.ExtendedSettings.ImportList);
      Program.NetworkManager.BroadcastStart();
      VisibilityAnimator.EnableAnimation = Program.Settings.AnimatePanels && !Program.ExtendedSettings.DisableMenuHideShowAnimation;
      SizableContainer.EnableAnimation = Program.Settings.AnimatePanels;
      Program.Settings.AnimatePanelsChanged += (EventHandler) ((sender, ea) => SizableContainer.EnableAnimation = Program.Settings.AnimatePanels);
      if (this.books.Slots.Count == 0)
        this.RebuildBookTabs();
      this.OnUpdateGui();
      this.IsInitialized = true;
      ControlExtensions.BeginInvoke(this, (Action) (() => ScriptUtility.Invoke("Startup")));
    }

    protected override void OnActivated(EventArgs e)
    {
      base.OnActivated(e);
      if (!this.mainViewContainer.Expanded)
        this.ComicDisplay.Focus();
      else
        this.mainView.Focus();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);
      if (e.CloseReason == CloseReason.UserClosing && Program.Settings.CloseMinimizesToTray && !this.menuClose)
      {
        this.MinimizeToTray();
        if ((Program.Settings.HiddenMessageBoxes & HiddenMessageBoxes.ComicRackMinimized) == HiddenMessageBoxes.None)
        {
          this.notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
          this.notifyIcon.BalloonTipText = TR.Messages["ComicRackMinimized", "You either close ComicRack with File/Exit or you can change this behavior in the Preferences Dialog.\nClick here to not show this message again"];
          this.notifyIcon.BalloonTipTitle = TR.Messages["ComicRackMinimizedTitle", "ComicRack is still running"];
          this.notifyIcon.Tag = (object) HiddenMessageBoxes.ComicRackMinimized;
          this.notifyIcon.ShowBalloonTip(5000);
        }
        e.Cancel = true;
      }
      else
      {
        if (Program.Settings.UpdateComicFiles)
        {
          IEnumerable<ComicBook> dirtyTempList = Program.BookFactory.TemporaryBooks.Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.ComicInfoIsDirty));
          int dirtyCount = dirtyTempList.Count<ComicBook>();
          if (dirtyCount != 0 && Program.AskQuestion((IWin32Window) this, TR.Messages["AskDirtyItems", "Save changed information for Books that are not in the database?\nAll changes not saved now will be lost!"], TR.Default["Save", "Save"], HiddenMessageBoxes.AskDirtyItems, TR.Messages["AlwaysSaveDirty", "Always save changes"], TR.Default["No", "No"]))
            AutomaticProgressDialog.Process((Form) this, TR.Messages["SaveInfo", "Saving Book Information"], TR.Messages["SaveInfoText", "Please wait while all unsaved information is stored!"], 5000, (Action) (() =>
            {
              int num = 0;
              foreach (ComicBook cb in dirtyTempList)
              {
                if (AutomaticProgressDialog.ShouldAbort)
                  break;
                AutomaticProgressDialog.Value = num++ * 100 / dirtyCount;
                Program.QueueManager.WriteInfoToFileWithCacheUpdate(cb);
              }
            }), AutomaticProgressDialogOptions.EnableCancel);
        }
        if (Program.QueueManager.IsActive && !QuestionDialog.Ask((IWin32Window) this, TR.Messages["BackgroundConvert", "Files are still being updated/converted/synchronized in the background. If you close now, some information will not be written!"], TR.Messages["CloseComicRack", "Close ComicRack"]))
        {
          e.Cancel = true;
        }
        else
        {
          if (ScriptUtility.Enabled)
            Program.Settings.PluginsStates = ScriptUtility.Scripts.CommandStates;
          Program.Settings.LastOpenFiles.Clear();
          Program.Settings.LastOpenFiles.AddRange(this.books.OpenFiles);
          this.StoreWorkspace();
          Program.Settings.QuickOpenThumbnailSize = this.quickOpenView.ThumbnailSize;
          Program.Settings.MagnifySize = this.ComicDisplay.MagnifierSize;
          Program.Settings.MagnifyOpaque = this.ComicDisplay.MagnifierOpacity;
          Program.Settings.MagnifyZoom = this.ComicDisplay.MagnifierZoom;
          Program.Settings.MagnifyStyle = this.ComicDisplay.MagnifierStyle;
          Program.Settings.AutoHideMagnifier = this.ComicDisplay.AutoHideMagnifier;
          Program.Settings.AutoMagnifier = this.ComicDisplay.AutoMagnifier;
          Program.Settings.ThumbCacheEnabled = Program.ImagePool.Thumbs.DiskCache.Enabled;
          Program.Settings.PageFilter = this.ComicDisplay.PageFilter;
          Program.Settings.ReaderKeyboardMapping.Clear();
          Program.Settings.ReaderKeyboardMapping.AddRange(this.ComicDisplay.KeyboardMap.GetKeyMapping());
          Program.NetworkManager.BroadcastStop();
          if (this.readerForm == null)
            return;
          this.readerForm.Dispose();
          this.readerForm = (ReaderForm) null;
        }
      }
    }

    private void ConstraintMainView(bool always)
    {
      if (!this.Visible && !always || this.mainView.Dock == DockStyle.Fill || this.WindowState == FormWindowState.Minimized)
        return;
      Rectangle displayRectangle = this.DisplayRectangle;
      if (this.fileTabsVisibility.Visible)
      {
        displayRectangle.Y = this.fileTabs.Bottom;
        displayRectangle.Height -= this.fileTabs.Bottom;
      }
      if (this.statusStrip.Visible)
        displayRectangle.Height -= this.statusStrip.Height;
      this.mainViewContainer.Bounds = Rectangle.Intersect(displayRectangle, this.mainViewContainer.Bounds);
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
      base.OnLayout(levent);
      this.ConstraintMainView(false);
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      if (!this.MinimizedToTray)
        return;
      this.Visible = false;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      this.menuDown = e.KeyCode == Keys.Menu;
      if (this.mainKeys.HandleKey(e.KeyCode | e.Modifiers))
        return;
      base.OnKeyDown(e);
    }

    private void InitializeCommands()
    {
      this.tbTools.DropDownOpening += (EventHandler) ((s, e) => this.mainToolStrip.DefaultDropDownDirection = ToolStripDropDownDirection.BelowLeft);
      this.tbTools.DropDownClosed += (EventHandler) ((s, e) => this.mainToolStrip.DefaultDropDownDirection = ToolStripDropDownDirection.Default);
      this.commands.Add(new CommandHandler(this.ShowOpenDialog), (object) this.miOpenComic, (object) this.cmOpenComic, (object) this.tbOpenComic);
      this.commands.Add(new CommandHandler(this.OpenBooks.Close), (UpdateHandler) (() => this.OpenBooks.Slots.Count > 0), (object) this.miCloseComic, (object) this.cmClose, (object) this.cmCloseComic);
      this.commands.Add(new CommandHandler(this.OpenBooks.CloseAll), (UpdateHandler) (() => this.OpenBooks.Slots.Count > 0), (object) this.miCloseAllComics);
      this.commands.Add(new CommandHandler(this.OpenBooks.AddSlot), (object) this.miAddTab);
      this.commands.Add((CommandHandler) (() => this.AddNewBook()), (object) this.miNewComic);
      this.commands.Add((CommandHandler) (() => this.OpenNextComic()), (object) this.miNextFromList, (object) this.tbNextFromList, (object) this.cmNextFromList);
      this.commands.Add((CommandHandler) (() => this.OpenPrevComic()), (object) this.miPrevFromList, (object) this.tbPrevFromList, (object) this.cmPrevFromList);
      this.commands.Add((CommandHandler) (() => this.OpenRandomComic()), (object) this.miRandomFromList, (object) this.tbRandomFromList, (object) this.cmRandomFromList);
      this.commands.Add((CommandHandler) (() => this.SyncBrowser()), (UpdateHandler) (() => this.ComicDisplay.Book != null), (object) this.miSyncBrowser, (object) this.cmSyncBrowser);
      this.commands.Add(new CommandHandler(this.AddFolderToLibrary), (object) this.miAddFolderToLibrary);
      this.commands.Add(new CommandHandler(this.StartFullScan), (object) this.miScan, (object) this.tbScan);
      this.commands.Add(new CommandHandler(this.UpdateComics), (object) this.miUpdateAllComicFiles, (object) this.tbUpdateAllComicFiles);
      this.commands.Add(new CommandHandler(this.MenuSynchronizeDevices), (object) this.miSynchronizeDevices, (object) this.tsSynchronizeDevices);
      this.commands.Add(new CommandHandler(this.UpdateWebComics), (object) this.miUpdateWebComics, (object) this.tbUpdateWebComics);
      this.commands.Add((CommandHandler) (() => this.ShowPendingTasks()), (object) this.miTasks);
      this.commands.Add(new CommandHandler(this.MenuRestart), (object) this.miRestart);
      this.commands.Add(new CommandHandler(this.MenuClose), (object) this.miExit, (object) this.cmNotifyExit, (object) this.tbExit);
      this.commands.Add(new CommandHandler(this.OpenRemoteLibrary), (object) this.miOpenRemoteLibrary, (object) this.tbOpenRemoteLibrary);
      this.commands.Add(new CommandHandler(this.ComicDisplay.DisplayFirstPage), (UpdateHandler) (() => this.ComicDisplay.Book != null && this.ComicDisplay.Book.CanNavigate(-1)), (object) this.miFirstPage, (object) this.tbFirstPage);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.DisplayPreviousPage(ComicDisplay.PagingMode.Double)), (UpdateHandler) (() => this.ComicDisplay.Book != null), (object) this.miPrevPage, (object) this.tbPrevPage);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.DisplayNextPage(ComicDisplay.PagingMode.Double)), (UpdateHandler) (() => this.ComicDisplay.Book != null), (object) this.miNextPage, (object) this.tbNextPage);
      this.commands.Add(new CommandHandler(this.ComicDisplay.DisplayLastPage), (UpdateHandler) (() => this.ComicDisplay.Book != null && this.ComicDisplay.Book.CanNavigate(1)), (object) this.miLastPage, (object) this.tbLastPage);
      this.commands.Add(new CommandHandler(this.ComicDisplay.DisplayPreviousBookmarkedPage), (UpdateHandler) (() => this.ComicDisplay.Book != null && this.ComicDisplay.Book.CanNavigateBookmark(-1)), (object) this.miPrevBookmark, (object) this.tbPrevBookmark, (object) this.cmPrevBookmark);
      this.commands.Add(new CommandHandler(this.ComicDisplay.DisplayNextBookmarkedPage), (UpdateHandler) (() => this.ComicDisplay.Book != null && this.ComicDisplay.Book.CanNavigateBookmark(1)), (object) this.miNextBookmark, (object) this.tbNextBookmark, (object) this.cmNextBookmark);
      this.commands.Add(new CommandHandler(this.SetBookmark), new UpdateHandler(this.SetBookmarkAvailable), (object) this.miSetBookmark, (object) this.tbSetBookmark, (object) this.cmSetBookmark);
      this.commands.Add(new CommandHandler(this.RemoveBookmark), new UpdateHandler(this.RemoveBookmarkAvailable), (object) this.miRemoveBookmark, (object) this.tbRemoveBookmark, (object) this.cmRemoveBookmark);
      this.commands.Add(new CommandHandler(this.ComicDisplay.DisplayLastPageRead), (UpdateHandler) (() => this.ComicDisplay.Book != null && this.ComicDisplay.Book.CurrentPage != this.ComicDisplay.Book.Comic.LastPageRead), (object) this.miLastPageRead, (object) this.tbLastPageRead, (object) this.cmLastPageRead);
      this.commands.Add(new CommandHandler(this.OpenBooks.PreviousSlot), (UpdateHandler) (() => this.OpenBooks.Slots.Count > 1), (object) this.miPrevTab);
      this.commands.Add(new CommandHandler(this.OpenBooks.NextSlot), (UpdateHandler) (() => this.OpenBooks.Slots.Count > 1), (object) this.miNextTab);
      this.commands.AddService<ILibraryBrowser>((Control) this, (ServiceCommandHandler<ILibraryBrowser>) (s => s.BrowseNext()), (ServiceUpdateHandler<ILibraryBrowser>) (s => s.CanBrowseNext()), (object) this.miNextList);
      this.commands.AddService<ILibraryBrowser>((Control) this, (ServiceCommandHandler<ILibraryBrowser>) (s => s.BrowsePrevious()), (ServiceUpdateHandler<ILibraryBrowser>) (s => s.CanBrowsePrevious()), (object) this.miPreviousList);
      this.commands.Add((CommandHandler) (() => Program.Settings.AutoScrolling = !Program.Settings.AutoScrolling), true, (UpdateHandler) (() => Program.Settings.AutoScrolling), (object) this.miAutoScroll, (object) this.tbAutoScroll);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.TwoPageNavigation = !this.ComicDisplay.TwoPageNavigation), true, (UpdateHandler) (() => this.ComicDisplay.TwoPageNavigation), (object) this.miDoublePageAutoScroll);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.RightToLeftReading = !this.ComicDisplay.RightToLeftReading), true, (UpdateHandler) (() => this.ComicDisplay.RightToLeftReading), (object) this.miRightToLeft, (object) this.tbRightToLeft, (object) this.cmRightToLeft);
      this.commands.Add(new CommandHandler(this.ShowInfo), (UpdateHandler) (() => this.InvokeActiveService<IGetBookList, bool>((Func<IGetBookList, bool>) (bl => !bl.GetBookList(ComicBookFilterType.Selected).IsEmpty<ComicBook>()))), (object) this.miShowInfo, (object) this.tbShowInfo, (object) this.cmShowInfo);
      this.commands.Add(new CommandHandler(Program.Database.Undo.Undo), (UpdateHandler) (() => Program.Database.Undo.CanUndo), (object) this.miUndo);
      this.commands.Add(new CommandHandler(Program.Database.Undo.Redo), (UpdateHandler) (() => Program.Database.Undo.CanRedo), (object) this.miRedo);
      this.commands.Add(new CommandHandler(this.ComicDisplay.CopyPageToClipboard), (UpdateHandler) (() => this.ComicDisplay.Book != null), (object) this.miCopyPage, (object) this.cmCopyPage);
      this.commands.Add(new CommandHandler(this.ExportCurrentImage), (UpdateHandler) (() => this.ComicDisplay.Book != null), (object) this.miExportPage, (object) this.cmExportPage);
      this.commands.Add(new CommandHandler(this.ToggleUndockReader), true, (UpdateHandler) (() => this.ReaderUndocked), (object) this.miReaderUndocked, (object) this.tbReaderUndocked);
      this.commands.Add((CommandHandler) (() => this.MinimalGui = !this.MinimalGui), true, (UpdateHandler) (() => this.MinimalGui), (object) this.miMinimalGui, (object) this.cmMinimalGui, (object) this.tbMinimalGui);
      this.commands.Add(new CommandHandler(this.ComicDisplay.ToggleFullScreen), true, (UpdateHandler) (() => this.ComicDisplay.FullScreen), (object) this.miFullScreen, (object) this.tbFullScreen);
      this.commands.Add(new CommandHandler(this.ComicDisplay.TogglePageLayout), (object) this.tbPageLayout);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.PageLayout = PageLayoutMode.Single), true, (UpdateHandler) (() => this.ComicDisplay.PageLayout == PageLayoutMode.Single), (object) this.miSinglePage, (object) this.tbSinglePage, (object) this.cmSinglePage);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.PageLayout = PageLayoutMode.Double), true, (UpdateHandler) (() => this.ComicDisplay.PageLayout == PageLayoutMode.Double), (object) this.miTwoPages, (object) this.tbTwoPages, (object) this.cmTwoPages);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.PageLayout = PageLayoutMode.DoubleAdaptive), true, (UpdateHandler) (() => this.ComicDisplay.PageLayout == PageLayoutMode.DoubleAdaptive), (object) this.miTwoPagesAdaptive, (object) this.tbTwoPagesAdaptive, (object) this.cmTwoPagesAdaptive);
      this.commands.Add(new CommandHandler(this.ComicDisplay.TogglePageFit), (object) this.tbFit);
      this.commands.Add(new CommandHandler(this.ComicDisplay.SetPageOriginal), true, (UpdateHandler) (() => this.ComicDisplay.ImageFitMode == ImageFitMode.Original), (object) this.miOriginal, (object) this.cmOriginal, (object) this.tbOriginal);
      this.commands.Add(new CommandHandler(this.ComicDisplay.SetPageFitAll), true, (UpdateHandler) (() => this.ComicDisplay.ImageFitMode == ImageFitMode.Fit), (object) this.miFitAll, (object) this.tbFitAll, (object) this.cmFitAll);
      this.commands.Add(new CommandHandler(this.ComicDisplay.SetPageFitWidth), true, new UpdateHandler(this.ComicDisplay.IsPageFitWidth), (object) this.miFitWidth, (object) this.tbFitWidth, (object) this.cmFitWidth);
      this.commands.Add(new CommandHandler(this.ComicDisplay.SetPageFitWidthAdaptive), true, new UpdateHandler(this.ComicDisplay.IsPageFitWidthAdaptive), (object) this.miFitWidthAdaptive, (object) this.tbFitWidthAdaptive, (object) this.cmFitWidthAdaptive);
      this.commands.Add(new CommandHandler(this.ComicDisplay.SetPageFitHeight), true, new UpdateHandler(this.ComicDisplay.IsPageFitHeight), (object) this.miFitHeight, (object) this.tbFitHeight, (object) this.cmFitHeight);
      this.commands.Add(new CommandHandler(this.ComicDisplay.SetPageBestFit), true, new UpdateHandler(this.ComicDisplay.IsPageFitBest), (object) this.miBestFit, (object) this.tbBestFit, (object) this.cmFitBest);
      this.commands.Add(new CommandHandler(this.ComicDisplay.ToggleFitOnlyIfOversized), true, (UpdateHandler) (() => this.ComicDisplay.ImageFitOnlyIfOversized), (object) this.miOnlyFitOversized, (object) this.tbOnlyFitOversized, (object) this.cmOnlyFitOversized);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = Numeric.Select(this.ComicDisplay.ImageZoom, new float[4]
      {
        1f,
        1.25f,
        1.5f,
        2f
      }, true)), (object) this.tbZoom);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = (this.ComicDisplay.ImageZoom + 0.1f).Clamp(1f, 8f)), (UpdateHandler) (() => (double) this.ComicDisplay.ImageZoom < 8.0), (object) this.miZoomIn, (object) this.tbZoomIn);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = (this.ComicDisplay.ImageZoom - 0.1f).Clamp(1f, 8f)), (UpdateHandler) (() => (double) this.ComicDisplay.ImageZoom > 1.0), (object) this.miZoomOut, (object) this.tbZoomOut);
      this.commands.Add((CommandHandler) (() => this.ToggleZoom(CommandKey.None)), (object) this.miToggleZoom);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = 1f), (object) this.miZoom100, (object) this.tbZoom100);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = 1.25f), (object) this.miZoom125, (object) this.tbZoom125);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = 1.5f), (object) this.miZoom150, (object) this.tbZoom150);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = 2f), (object) this.miZoom200, (object) this.tbZoom200);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = 4f), (object) this.miZoom400, (object) this.tbZoom400);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageZoom = ZoomDialog.Show((IWin32Window) this, this.ComicDisplay.ImageZoom)), (object) this.miZoomCustom, (object) this.tbZoomCustom);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageRotation = ImageRotation.None), true, (UpdateHandler) (() => this.ComicDisplay.ImageRotation == ImageRotation.None), (object) this.miRotate0, (object) this.tbRotate0, (object) this.cmRotate0);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageRotation = ImageRotation.Rotate90), true, (UpdateHandler) (() => this.ComicDisplay.ImageRotation == ImageRotation.Rotate90), (object) this.miRotate90, (object) this.tbRotate90, (object) this.cmRotate90);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageRotation = ImageRotation.Rotate180), true, (UpdateHandler) (() => this.ComicDisplay.ImageRotation == ImageRotation.Rotate180), (object) this.miRotate180, (object) this.tbRotate180, (object) this.cmRotate180);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageRotation = ImageRotation.Rotate270), true, (UpdateHandler) (() => this.ComicDisplay.ImageRotation == ImageRotation.Rotate270), (object) this.miRotate270, (object) this.tbRotate270, (object) this.cmRotate270);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageRotation = this.ComicDisplay.ImageRotation.RotateLeft()), (object) this.miRotateLeft, (object) this.tbRotateLeft);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageRotation = this.ComicDisplay.ImageRotation.RotateRight()), (object) this.miRotateRight, (object) this.tbRotateRight, (object) this.tbRotate);
      this.commands.Add((CommandHandler) (() => this.ComicDisplay.ImageAutoRotate = !this.ComicDisplay.ImageAutoRotate), true, (UpdateHandler) (() => this.ComicDisplay.ImageAutoRotate), (object) this.miAutoRotate, (object) this.tbAutoRotate);
      this.commands.Add(new CommandHandler(this.ComicDisplay.ToggleMagnifier), true, (UpdateHandler) (() => this.ComicDisplay.MagnifierVisible), (object) this.miMagnify, (object) this.tbMagnify, (object) this.cmMagnify);
      this.commands.Add((CommandHandler) (() => this.ShowPortableDevices()), (object) this.miDevices);
      this.commands.Add((CommandHandler) (() => this.ShowPreferences()), (object) this.miPreferences, (object) this.tbPreferences);
      this.commands.Add((CommandHandler) (() => Program.Settings.AutoHideMainMenu = !Program.Settings.AutoHideMainMenu), true, (UpdateHandler) (() => !Program.Settings.AutoHideMainMenu), (object) this.tbShowMainMenu);
      this.commands.Add((CommandHandler) (() =>
      {
        this.BrowserVisible = true;
        this.mainView.ShowLibrary();
      }), (object) this.miViewLibrary);
      this.commands.Add((CommandHandler) (() =>
      {
        this.BrowserVisible = true;
        this.mainView.ShowFolders();
      }), (object) this.miViewFolders);
      this.commands.Add((CommandHandler) (() =>
      {
        this.BrowserVisible = true;
        this.mainView.ShowPages();
      }), (UpdateHandler) (() => this.OpenBooks.CurrentBook != null), (object) this.miViewPages);
      this.commands.Add(new CommandHandler(this.ToggleBrowser), true, (UpdateHandler) (() => this.BrowserVisible), (object) this.miToggleBrowser);
      this.commands.Add(new CommandHandler(this.ToggleSidebar), new UpdateHandler(this.CheckSidebarAvailable), new UpdateHandler(this.CheckSidebarEnabled), (object) this.miSidebar);
      this.commands.Add(new CommandHandler(this.ToggleSmallPreview), new UpdateHandler(this.CheckSidebarAvailable), new UpdateHandler(this.CheckSmallPreviewEnabled), (object) this.miSmallPreview);
      this.commands.Add(new CommandHandler(this.ToggleSearchBrowser), new UpdateHandler(this.CheckSearchAvailable), new UpdateHandler(this.CheckSearchBrowserEnabled), (object) this.miSearchBrowser);
      this.commands.Add(new CommandHandler(this.ToggleInfoPanel), new UpdateHandler(this.CheckInfoPanelAvailable), new UpdateHandler(this.CheckInfoPanelEnabled), (object) this.miInfoPanel);
      this.commands.AddService<IRefreshDisplay>((Control) this, (ServiceCommandHandler<IRefreshDisplay>) (c => c.RefreshDisplay()), (object) this.miViewRefresh);
      this.commands.Add((CommandHandler) (() => this.GetRatingEditor().SetRating(0.0f)), (UpdateHandler) (() => this.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.GetRatingEditor().GetRating()) == 0.0), (object) this.miRate0, (object) this.cmRate0);
      this.commands.Add((CommandHandler) (() => this.GetRatingEditor().SetRating(1f)), (UpdateHandler) (() => this.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.GetRatingEditor().GetRating()) == 1.0), (object) this.miRate1, (object) this.cmRate1);
      this.commands.Add((CommandHandler) (() => this.GetRatingEditor().SetRating(2f)), (UpdateHandler) (() => this.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.GetRatingEditor().GetRating()) == 2.0), (object) this.miRate2, (object) this.cmRate2);
      this.commands.Add((CommandHandler) (() => this.GetRatingEditor().SetRating(3f)), (UpdateHandler) (() => this.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.GetRatingEditor().GetRating()) == 3.0), (object) this.miRate3, (object) this.cmRate3);
      this.commands.Add((CommandHandler) (() => this.GetRatingEditor().SetRating(4f)), (UpdateHandler) (() => this.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.GetRatingEditor().GetRating()) == 4.0), (object) this.miRate4, (object) this.cmRate4);
      this.commands.Add((CommandHandler) (() => this.GetRatingEditor().SetRating(5f)), (UpdateHandler) (() => this.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.GetRatingEditor().GetRating()) == 5.0), (object) this.miRate5, (object) this.cmRate5);
      this.commands.Add((CommandHandler) (() => this.GetRatingEditor().QuickRatingAndReview()), (UpdateHandler) (() => this.GetRatingEditor().IsValid()), (object) this.miQuickRating, (object) this.cmQuickRating);
      this.commands.Add((CommandHandler) (() =>
      {
        if (Program.Help.Execute("HelpMain"))
          return;
        Program.StartDocument("http://comicrack.cyolito.com/documentation/wiki");
      }), (object) this.miWebHelp);
      this.commands.Add((CommandHandler) (() => this.books.Open(Program.QuickHelpManualFile, OpenComicOptions.OpenInNewSlot | OpenComicOptions.NoFileUpdate)), (object) this.miHelpQuickIntro);
      this.commands.Add((CommandHandler) (() => Program.StartDocument("http://comicrack.cyolito.com")), (object) this.miWebHome);
      this.commands.Add((CommandHandler) (() => Program.StartDocument("http://comicrack.cyolito.com/user-forum")), (object) this.miWebUserForum);
      this.commands.Add(new CommandHandler(this.ShowAboutDialog), (object) this.miAbout, (object) this.tbAbout);
      this.commands.Add((CommandHandler) (() => DonationDialog.Show((IWin32Window) this, true)), (object) this.miSupport, (object) this.tbSupport);
      this.commands.Add(new CommandHandler(this.ShowNews), (object) this.miNews);
      this.commands.Add(new CommandHandler(this.SaveWorkspace), (object) this.tsSaveWorkspace, (object) this.miSaveWorkspace);
      this.commands.Add(new CommandHandler(this.EditWorkspace), (UpdateHandler) (() => Program.Settings.Workspaces.Count > 0), (object) this.tsEditWorkspaces, (object) this.miEditWorkspaces);
      this.commands.Add(new CommandHandler(this.EditWorkspaceDisplaySettings), (object) this.miComicDisplaySettings, (object) this.tbComicDisplaySettings);
      this.commands.Add(new CommandHandler(this.EditListLayout), new UpdateHandler(this.CheckViewOptionsAvailable), (object) this.miEditListLayout);
      this.commands.Add(new CommandHandler(this.SaveListLayout), (object) this.miSaveListLayout);
      this.commands.Add(new CommandHandler(this.EditListLayouts), (UpdateHandler) (() => Program.Settings.ListConfigurations.Count > 0), (object) this.miEditLayouts);
      this.commands.Add((CommandHandler) (() => this.SetListLayoutToAll()), (object) this.miSetAllListsSame);
      this.commands.Add((CommandHandler) (() => Program.Settings.TrackCurrentPage = !Program.Settings.TrackCurrentPage), true, (UpdateHandler) (() => Program.Settings.TrackCurrentPage), (object) this.miTrackCurrentPage);
      this.commands.Add(new CommandHandler(this.ComicDisplay.RefreshDisplay), (object) this.ComicDisplay.IsValid, (object) this.cmRefreshPage);
      this.commands.Add(new CommandHandler(this.RestoreFromTray), (object) this.cmNotifyRestore);
      this.commands.Add(new CommandHandler(this.OpenBooks.CloseAllButCurrent), (UpdateHandler) (() => this.OpenBooks.Slots.Count > 0), (object) this.cmCloseAllButThis);
      this.commands.Add(new CommandHandler(this.OpenBooks.CloseAllToTheRight), (UpdateHandler) (() => this.OpenBooks.CurrentSlot < this.OpenBooks.Slots.Count - 1), (object) this.cmCloseAllToTheRight);
      this.commands.Add((CommandHandler) (() => Clipboard.SetText(this.ComicDisplay.Book.Comic.FilePath)), (UpdateHandler) (() => this.ComicDisplay.Book != null && this.ComicDisplay.Book.Comic.EditMode.IsLocalComic()), (object) this.cmCopyPath);
      this.commands.Add((CommandHandler) (() => Program.ShowExplorer(this.ComicDisplay.Book.Comic.FilePath)), (UpdateHandler) (() => this.ComicDisplay.Book != null && this.ComicDisplay.Book.Comic.EditMode.IsLocalComic()), (object) this.cmRevealInExplorer);
    }

    private void InitializeKeyboard()
    {
      string group1 = "Library";
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miNextFromList.Image, "NextComic", group1, "Next Book", (Action) (() => this.OpenNextComic()), new CommandKey[1]
      {
        CommandKey.N
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miPrevFromList.Image, "PrevComic", group1, "Previous Book", (Action) (() => this.OpenPrevComic()), new CommandKey[1]
      {
        CommandKey.P
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miRandomFromList.Image, "RandomComic", group1, "Random Book", (Action) (() => this.OpenRandomComic()), new CommandKey[1]
      {
        CommandKey.L
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miToggleBrowser.Image, "ShowBrowser", group1, "Show Browser", new Action(this.ToggleBrowserFromReader), new CommandKey[2]
      {
        CommandKey.MouseLeft,
        CommandKey.Escape
      }));
      string group2 = "Browse";
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miFirstPage.Image, "MoveToFirstPage", group2, "First Page", new Action(this.ComicDisplay.DisplayFirstPage), new CommandKey[2]
      {
        CommandKey.Home | CommandKey.Ctrl,
        CommandKey.GestureDouble1
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miPrevPage.Image, "MoveToPreviousPage", group2, "Previous Page", (Action) (() => this.ComicDisplay.DisplayPreviousPage(ComicDisplay.PagingMode.Double)), new CommandKey[4]
      {
        CommandKey.PageUp,
        CommandKey.Left | CommandKey.Alt,
        CommandKey.Gesture1,
        CommandKey.FlickRight
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miNextPage.Image, "MoveToNextPage", group2, "Next Page", (Action) (() => this.ComicDisplay.DisplayNextPage(ComicDisplay.PagingMode.Double)), new CommandKey[4]
      {
        CommandKey.PageDown,
        CommandKey.Right | CommandKey.Alt,
        CommandKey.Gesture3,
        CommandKey.FlickLeft
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miLastPage.Image, "MoveToLastPage", group2, "Last Page", new Action(this.ComicDisplay.DisplayLastPage), new CommandKey[2]
      {
        CommandKey.End | CommandKey.Ctrl,
        CommandKey.GestureDouble3
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miPrevBookmark.Image, "MoveToPrevBookmark", group2, "Previous Bookmark", new Action(this.ComicDisplay.DisplayPreviousBookmarkedPage), new CommandKey[1]
      {
        CommandKey.PageUp | CommandKey.Ctrl
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miNextBookmark.Image, "MoveToNextBookmark", group2, "Next Bookmark", new Action(this.ComicDisplay.DisplayNextBookmarkedPage), new CommandKey[1]
      {
        CommandKey.PageDown | CommandKey.Ctrl
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miPrevTab.Image, "PrevTab", group2, "Previous Tab", new Action(this.OpenBooks.PreviousSlot), new CommandKey[1]
      {
        CommandKey.Tab | CommandKey.Shift
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miNextTab.Image, "NextTab", group2, "Next Tab", new Action(this.OpenBooks.NextSlot), new CommandKey[1]
      {
        CommandKey.Tab
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveToPrevPageSingle", group2, "Single Page Back", (Action) (() => this.ComicDisplay.DisplayPreviousPage(ComicDisplay.PagingMode.None)), new CommandKey[1]
      {
        CommandKey.PageUp | CommandKey.Shift
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveToNextPageSingle", group2, "Single Page Forward", (Action) (() => this.ComicDisplay.DisplayNextPage(ComicDisplay.PagingMode.None)), new CommandKey[1]
      {
        CommandKey.PageDown | CommandKey.Shift
      }));
      string group3 = "Auto Scroll";
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MovePrevPart", group3, "Previous Part", (Action) (() => this.ComicDisplay.DisplayPreviousPageOrPart()), new CommandKey[2]
      {
        CommandKey.Space | CommandKey.Shift,
        CommandKey.Gesture7
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveNextPart", group3, "Next Part", (Action) (() => this.ComicDisplay.DisplayNextPageOrPart()), new CommandKey[2]
      {
        CommandKey.Space,
        CommandKey.Gesture9
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveFirstPart", group3, "Page Start", (Action) (() => this.ComicDisplay.DisplayPart(PartPageToDisplay.First)), new CommandKey[1]
      {
        CommandKey.Home
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveLastPart", group3, "Page End", (Action) (() => this.ComicDisplay.DisplayPart(PartPageToDisplay.Last)), new CommandKey[1]
      {
        CommandKey.End
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MovePartDown10", group3, "Move Part 10% down", (Action) (() => this.ComicDisplay.MovePartDown(0.1f)), new CommandKey[2]
      {
        CommandKey.V,
        CommandKey.Down | CommandKey.Ctrl
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MovePartUp10", group3, "Move Part 10% up", (Action) (() => this.ComicDisplay.MovePartDown(-0.1f)), new CommandKey[2]
      {
        CommandKey.B,
        CommandKey.Up | CommandKey.Ctrl
      }));
      string group4 = "Scroll";
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miAutoScroll.Image, "ToggleAutoScrolling", group4, "Toggle Auto Scrolling", (Action) (() => Program.Settings.AutoScrolling = !Program.Settings.AutoScrolling), new CommandKey[1]
      {
        CommandKey.S
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("DoublePageAutoScroll", group4, "Double Page Auto Scroll", (Action) (() => this.ComicDisplay.TwoPageNavigation = !this.ComicDisplay.TwoPageNavigation), new CommandKey[1]
      {
        CommandKey.S | CommandKey.Shift
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveUp", group4, "Up", new Action(this.ComicDisplay.ScrollUp), new CommandKey[2]
      {
        CommandKey.Up,
        CommandKey.MouseWheelUp
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveDown", group4, "Down", new Action(this.ComicDisplay.ScrollDown), new CommandKey[2]
      {
        CommandKey.Down,
        CommandKey.MouseWheelDown
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveLeft", group4, "Left", new Action(this.ComicDisplay.ScrollLeft), new CommandKey[2]
      {
        CommandKey.Left,
        CommandKey.MouseTiltLeft
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveRight", group4, "Right", new Action(this.ComicDisplay.ScrollRight), new CommandKey[2]
      {
        CommandKey.Right,
        CommandKey.MouseTiltRight
      }));
      string group5 = "Display Options";
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miReaderUndocked.Image, "ToggleUndockReader", group5, "Toggle Undock Reader", new Action(this.ToggleUndockReader), new CommandKey[1]
      {
        CommandKey.D
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miFullScreen.Image, "ToggleFullScreen", group5, "Toggle Full Screen", new Action(this.ComicDisplay.ToggleFullScreen), new CommandKey[3]
      {
        CommandKey.F,
        CommandKey.MouseDoubleLeft,
        CommandKey.Gesture2
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miTwoPages.Image, "ToggleTwoPages", group5, "Toggle Two Pages", new Action(this.ComicDisplay.TogglePageLayout), new CommandKey[1]
      {
        CommandKey.T
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("ToggleRealisticPages", group5, "Toggle Realistic Display", new Action(this.ComicDisplay.ToogleRealisticPages), new CommandKey[1]
      {
        CommandKey.D | CommandKey.Shift
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miMagnify.Image, "ToggleMagnify", group5, "Toggle Magnifier", (Action) (() => this.ComicDisplay.MagnifierVisible = !this.ComicDisplay.MagnifierVisible), new CommandKey[2]
      {
        CommandKey.M,
        CommandKey.TouchPressAndTap
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("ToggleMenu", group5, "Toggle Menu", (Action) (() => this.MinimalGui = !this.MinimalGui), new CommandKey[1]
      {
        CommandKey.K
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand((Image) null, "ToggleNavigationOverlay", group5, "Toggle Navigation Overlay", new Action(this.ComicDisplay.ToggleNavigationOverlay), new CommandKey[2]
      {
        CommandKey.Gesture8,
        CommandKey.TouchTwoFingerTap
      }));
      string group6 = "Page Display";
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miOriginal.Image, "Original", group6, "Original Size", new Action(this.ComicDisplay.SetPageOriginal), new CommandKey[1]
      {
        CommandKey.D1
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miFitAll.Image, "FitAll", group6, "Fit All", new Action(this.ComicDisplay.SetPageFitAll), new CommandKey[1]
      {
        CommandKey.D2
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miFitWidth.Image, "FitWidth", group6, "Fit Width", new Action(this.ComicDisplay.SetPageFitWidth), new CommandKey[1]
      {
        CommandKey.D3
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miFitWidthAdaptive.Image, "FitWidthAdaptive", group6, "Fit Width (adaptive)", new Action(this.ComicDisplay.SetPageFitWidthAdaptive), new CommandKey[1]
      {
        CommandKey.D4
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miFitHeight.Image, "FitHeight", group6, "Fit Height", new Action(this.ComicDisplay.SetPageFitHeight), new CommandKey[1]
      {
        CommandKey.D5
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miBestFit.Image, "FitBest", group6, "Best Fit", new Action(this.ComicDisplay.SetPageBestFit), new CommandKey[1]
      {
        CommandKey.D6
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miSinglePage.Image, "SinglePage", group6, "Single Page", (Action) (() => this.ComicDisplay.PageLayout = PageLayoutMode.Single), new CommandKey[1]
      {
        CommandKey.D7
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miTwoPages.Image, "TwoPages", group6, "Two Pages", (Action) (() => this.ComicDisplay.PageLayout = PageLayoutMode.Double), new CommandKey[1]
      {
        CommandKey.D8
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miTwoPagesAdaptive.Image, "TwoPagesAdaptive", group6, "Two Pages (adaptive)", (Action) (() => this.ComicDisplay.PageLayout = PageLayoutMode.DoubleAdaptive), new CommandKey[1]
      {
        CommandKey.D9
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miRightToLeft.Image, "RightToLeft", group6, "Right to Left", (Action) (() => this.ComicDisplay.RightToLeftReading = !this.ComicDisplay.RightToLeftReading), new CommandKey[1]
      {
        CommandKey.D0
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miOnlyFitOversized.Image, "OnlyFitIfOversized", group6, "Only Fit if oversized", new Action(this.ComicDisplay.ToggleFitOnlyIfOversized), new CommandKey[1]
      {
        CommandKey.O
      }));
      string group7 = "ZoomAndRotate";
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miRotateRight.Image, "RotateC", group7, "Rotate Right", (Action) (() => this.ComicDisplay.ImageRotation = this.ComicDisplay.ImageRotation.RotateRight()), new CommandKey[1]
      {
        CommandKey.R
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miRotateLeft.Image, "RotateCC", group7, "Rotate Left", (Action) (() => this.ComicDisplay.ImageRotation = this.ComicDisplay.ImageRotation.RotateLeft()), new CommandKey[1]
      {
        CommandKey.R | CommandKey.Shift
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miAutoRotate.Image, "AutoRotate", group7, "Autorotate Double Pages", (Action) (() => this.ComicDisplay.ImageAutoRotate = !this.ComicDisplay.ImageAutoRotate), new CommandKey[1]
      {
        CommandKey.A
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miZoomIn.Image, "ZoomIn", group7, "Zoom In", (Action) (() => this.ComicDisplay.ImageZoom = (this.ComicDisplay.ImageZoom + 0.1f).Clamp(1f, 8f)), new CommandKey[1]
      {
        CommandKey.MouseWheelUp | CommandKey.Ctrl
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miZoomOut.Image, "ZoomOut", group7, "Zoom Out", (Action) (() => this.ComicDisplay.ImageZoom = (this.ComicDisplay.ImageZoom - 0.1f).Clamp(1f, 8f)), new CommandKey[1]
      {
        CommandKey.MouseWheelDown | CommandKey.Ctrl
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miZoomIn.Image, "StepZoomIn", group7, "Step Zoom In", (Action) (() => this.ComicDisplay.ImageZoom = (this.ComicDisplay.ImageZoom + Program.ExtendedSettings.KeyboardZoomStepping).Clamp(1f, 4f)), new CommandKey[1]
      {
        CommandKey.Z
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miZoomOut.Image, "StepZoomOut", group7, "Step Zoom Out", (Action) (() => this.ComicDisplay.ImageZoom = (this.ComicDisplay.ImageZoom - Program.ExtendedSettings.KeyboardZoomStepping).Clamp(1f, 4f)), new CommandKey[1]
      {
        CommandKey.Z | CommandKey.Shift
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand(this.miToggleZoom.Image, "ToggleZoom", group7, "Toggle Zoom", new Action<CommandKey>(this.ToggleZoom), new CommandKey[1]
      {
        CommandKey.TouchDoubleTap
      }));
      string group8 = "Edit";
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand((Image) Resources.Rotate90Permanent, "PageRotateC", group8, "Rotate Page Right", (Action) (() => this.GetPageEditor().Rotation = this.GetPageEditor().Rotation.RotateRight()), new CommandKey[1]
      {
        CommandKey.Y
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand((Image) Resources.Rotate270Permanent, "PageRotateCC", group8, "Rotate Page Left", (Action) (() => this.GetPageEditor().Rotation = this.GetPageEditor().Rotation.RotateLeft()), new CommandKey[1]
      {
        CommandKey.Y | CommandKey.Shift
      }));
      this.ComicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("Exit", "Other", "Exit", new Action(this.ControlExit), new CommandKey[1]
      {
        CommandKey.Q
      }));
      Program.DefaultKeyboardMapping = (IEnumerable<StringPair>) this.ComicDisplay.KeyboardMap.GetKeyMapping().ToArray<StringPair>();
      this.ComicDisplay.KeyboardMap.SetKeyMapping((IEnumerable<StringPair>) Program.Settings.ReaderKeyboardMapping);
      this.mainKeys.Commands.Add(new KeyboardCommand("FocusQuickSearch", "General", "FQS", new Action(this.FocusQuickSearch), new CommandKey[1]
      {
        CommandKey.F | CommandKey.Ctrl
      }));
    }

    public bool AddRemoteLibrary(ShareInformation info, MainView.AddRemoteLibraryOptions options)
    {
      if (info == null)
        return false;
      if (this.mainView.IsRemoteConnected(info.Uri))
        return true;
      this.mainView.AddRemoteLibrary(ComicLibraryClient.Connect(info), options);
      return true;
    }

    public void OnRemoteServerStarted(ShareInformation info)
    {
      MainView.AddRemoteLibraryOptions options = MainView.AddRemoteLibraryOptions.Auto;
      if (Program.Settings.AutoConnectShares && info.IsLocal)
        options |= MainView.AddRemoteLibraryOptions.Open;
      this.AddRemoteLibrary(info, options);
    }

    public void OnRemoteServerStopped(string address) => this.mainView.RemoveRemoteLibrary(address);

    public void OpenRemoteLibrary()
    {
      RemoteShareItem share = OpenRemoteDialog.GetShare((IWin32Window) this, Program.Settings.RemoteShares.First, (IEnumerable<RemoteShareItem>) Program.Settings.RemoteShares, false);
      if (share == null || string.IsNullOrEmpty(share.Uri))
        return;
      string serverName = share.Uri;
      ShareInformation serverInfo = (ShareInformation) null;
      AutomaticProgressDialog.Process((Form) this, TR.Messages["ConnectToServer", "Connecting to Server"], TR.Messages["GetShareInfoText", "Getting information about the shared Library"], 1000, (Action) (() => serverInfo = ComicLibraryClient.GetServerInfo(serverName)), AutomaticProgressDialogOptions.EnableCancel);
      if (serverInfo == null || !this.AddRemoteLibrary(serverInfo, MainView.AddRemoteLibraryOptions.Open | MainView.AddRemoteLibraryOptions.Select))
      {
        int num = (int) MessageBox.Show((IWin32Window) this, StringUtility.Format(TR.Messages["ConnectRemoteError", "Failed to connect to remote Server"], (object) share), TR.Messages["Error", "Error"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else
        Program.Settings.RemoteShares.UpdateMostRecent(new RemoteShareItem(serverInfo));
    }

    private void BookDragEnter(object sender, DragEventArgs e)
    {
      string[] data = (string[]) e.Data.GetData(DataFormats.FileDrop);
      e.Effect = data == null || data.Length != 1 ? DragDropEffects.None : DragDropEffects.Copy;
    }

    private void BookDragDrop(object sender, DragEventArgs e)
    {
      this.OpenSupportedFile(((string[]) e.Data.GetData(DataFormats.FileDrop))[0]);
    }

    [DefaultValue(null)]
    public ComicDisplay ComicDisplay
    {
      get
      {
        if (this.comicDisplay == null)
          this.comicDisplay = this.CreateComicDisplay();
        return this.comicDisplay;
      }
    }

    [DefaultValue(false)]
    public bool AutoHideMainMenu
    {
      get => this.autoHideMainMenu;
      set
      {
        if (this.autoHideMainMenu == value)
          return;
        this.autoHideMainMenu = value;
        this.OnGuiVisibilities();
      }
    }

    [DefaultValue(true)]
    public bool ShowMainMenuNoComicOpen
    {
      get => this.showMainMenuNoComicOpen;
      set
      {
        if (this.showMainMenuNoComicOpen == value)
          return;
        this.showMainMenuNoComicOpen = value;
        this.OnGuiVisibilities();
      }
    }

    [Browsable(false)]
    public bool IsInitialized { get; private set; }

    public ComicBook AddNewBook(bool showDialog = true)
    {
      ComicBook comicBook = new ComicBook()
      {
        AddedTime = DateTime.Now
      };
      if (showDialog && !ComicBookDialog.Show((IWin32Window) (Form.ActiveForm ?? (Form) this), comicBook, (ComicBook[]) null, (Func<ComicBook, bool>) null))
        return (ComicBook) null;
      Program.Database.Add(comicBook);
      return comicBook;
    }

    public ComicListItem ImportComicList(string file)
    {
      return this.FindFirstService<IImportComicList>()?.ImportList(file);
    }

    public void MenuClose()
    {
      this.menuClose = true;
      this.Close();
    }

    public void MenuRestart()
    {
      Program.Restart = true;
      this.MenuClose();
    }

    public void UpdateFeeds()
    {
      using (ItemMonitor.Lock((object) Program.News))
        Program.News.UpdateFeeds(60);
    }

    public void ShowNews(bool always)
    {
      if (always)
      {
        AutomaticProgressDialog.Process((Form) this, TR.Messages["RetrieveNews", "Retrieving News"], TR.Messages["RetrieveNewsText", "Refreshing subscribed News Channels"], 1000, new Action(this.UpdateFeeds), AutomaticProgressDialogOptions.EnableCancel);
        NewsDialog.ShowNews((IWin32Window) this, Program.News);
      }
      else
        ThreadUtility.RunInBackground("Read News", (ThreadStart) (() =>
        {
          this.UpdateFeeds();
          if (!Program.News.HasUnread)
            return;
          ControlExtensions.BeginInvoke(this, (Action) (() => NewsDialog.ShowNews((IWin32Window) this, Program.News)));
        }));
    }

    public void AddFolderToLibrary()
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.Description = TR.Messages["AddFolderLibrary", "Books in this Folder and all sub Folders will be added to the library."];
        folderBrowserDialog.ShowNewFolderButton = true;
        if (folderBrowserDialog.ShowDialog((IWin32Window) this) != DialogResult.OK || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
          return;
        Program.Scanner.ScanFileOrFolder(folderBrowserDialog.SelectedPath, true, false);
      }
    }

    public void StartFullScan()
    {
      Program.QueueManager.StartScan(true, Program.Settings.RemoveMissingFilesOnFullScan);
    }

    public void UpdateComics()
    {
      Program.Database.Books.Concat<ComicBook>((IEnumerable<ComicBook>) Program.BookFactory.TemporaryBooks).ForEach<ComicBook>((Action<ComicBook>) (cb => Program.QueueManager.AddBookToFileUpdate(cb, true)));
    }

    public void UpdateWebComics(bool refresh = false)
    {
      Program.Database.Books.Concat<ComicBook>((IEnumerable<ComicBook>) Program.BookFactory.TemporaryBooks).ForEach<ComicBook>((Action<ComicBook>) (cb => this.UpdateWebComic(cb, refresh)));
    }

    public void UpdateWebComics() => this.UpdateWebComics(false);

    public void UpdateWebComic(ComicBook cb, bool refresh)
    {
      Program.QueueManager.AddBookToDynamicUpdate(cb, refresh);
    }

    public bool OpenNextComic(int relative, OpenComicOptions openOptions)
    {
      if (this.ComicDisplay == null || this.ComicDisplay.Book == null)
        return false;
      ComicBook comic = this.ComicDisplay.Book.Comic;
      if (comic == null)
        return false;
      IComicBrowser comicBrowser = this.FindServices<IComicBrowser>().FirstOrDefault<IComicBrowser>((Func<IComicBrowser, bool>) (cb => cb.Library == comic.Container));
      if (comicBrowser == null)
        return false;
      if (comicBrowser.Library != null && comic.LastOpenedFromListId != Guid.Empty)
      {
        ComicListItem list = comicBrowser.Library.ComicLists.GetItems<ComicListItem>().FirstOrDefault<ComicListItem>((Func<ComicListItem, bool>) (li => li.Id == comic.LastOpenedFromListId));
        if (list != null)
          this.ShowBookInList(comicBrowser.Library, list, comic, false);
      }
      ComicBook[] array1 = comicBrowser.GetBookList(ComicBookFilterType.IsNotFileless).ToArray<ComicBook>();
      if (array1.Length == 0)
        return false;
      int index1 = ((IEnumerable<ComicBook>) array1).FindIndex<ComicBook>((Predicate<ComicBook>) (cb => cb.Id == comic.Id));
      ComicBook cb1;
      if (relative != 0)
      {
        if (index1 == -1)
          return false;
        int index2 = index1 + relative;
        if (index2 < 0 || index2 >= array1.Length)
          return false;
        cb1 = array1[index2];
      }
      else
      {
        if (!((IEnumerable<ComicBook>) this.lastRandomList).SequenceEqual<ComicBook>((IEnumerable<ComicBook>) array1))
        {
          this.lastRandomList = array1;
          this.randomSelectedComics = new List<ComicBook>();
        }
        if (this.lastRandomList.Length == this.randomSelectedComics.Count)
          this.randomSelectedComics.Clear();
        ComicBook[] array2 = ((IEnumerable<ComicBook>) this.lastRandomList).Except<ComicBook>((IEnumerable<ComicBook>) this.randomSelectedComics).ToArray<ComicBook>();
        int index3 = new Random().Next(0, array2.Length);
        cb1 = array2[index3];
        this.randomSelectedComics.Add(cb1);
      }
      if (cb1 == null)
        return false;
      cb1.LastOpenedFromListId = comicBrowser.GetBookListId();
      return this.books.Open(cb1, openOptions);
    }

    public bool OpenNextComic(int relative) => this.OpenNextComic(relative, OpenComicOptions.None);

    public void ShowInfo()
    {
      IGetBookList activeService1 = FormUtility.FindActiveService<IGetBookList>();
      if (activeService1 == null)
        return;
      IEnumerable<ComicBook> bookList = activeService1.GetBookList(ComicBookFilterType.Selected);
      if (bookList.Count<ComicBook>() > 1 && bookList.All<ComicBook>((Func<ComicBook, bool>) (cb => cb.EditMode.CanEditProperties())))
      {
        Program.Database.Undo.SetMarker(TR.Messages["UndoEditMultipleComics", "Edit multiple Books"]);
        using (MultipleComicBooksDialog comicBooksDialog = new MultipleComicBooksDialog(bookList))
        {
          int num = (int) comicBooksDialog.ShowDialog((IWin32Window) this);
        }
      }
      else
      {
        if (bookList.IsEmpty<ComicBook>())
          return;
        IComicBrowser activeService2 = FormUtility.FindActiveService<IComicBrowser>();
        Program.Database.Undo.SetMarker(TR.Messages["UndoShowInfo", "Show Info"]);
        ComicBookDialog.Show((IWin32Window) (Form.ActiveForm ?? (Form) this), bookList.FirstOrDefault<ComicBook>(), activeService1.GetBookList(ComicBookFilterType.All).ToArray<ComicBook>(), activeService2 != null ? new Func<ComicBook, bool>(activeService2.SelectComic) : (Func<ComicBook, bool>) null);
      }
    }

    public void ToggleBrowser(bool alwaysShow, IComicBrowser cb = null)
    {
      this.BrowserVisible = !this.BrowserVisible | alwaysShow;
      if (this.ReaderUndocked)
      {
        if (!this.BrowserVisible)
        {
          this.readerForm.Focus();
        }
        else
        {
          this.mainView.Focus();
          if (cb == null)
            return;
          this.mainView.ShowLibrary(cb.Library);
        }
      }
      else if (!this.BrowserVisible)
        this.UpdateBrowserVisibility();
      else if (cb == null)
        this.mainView.ShowLast();
      else
        this.mainView.ShowLibrary(cb.Library);
    }

    public void ToggleBrowser() => this.ToggleBrowser(false);

    public void ToggleBrowserFromReader()
    {
      if (!Program.ExtendedSettings.MouseSwitchesToFullLibrary && (this.ReaderUndocked || this.mainViewContainer.Dock == DockStyle.Fill))
        this.MinimalGui = !this.MinimalGui;
      else
        this.ToggleBrowser(false);
    }

    public IEditRating GetRatingEditor()
    {
      IGetBookList activeService = FormUtility.FindActiveService<IGetBookList>();
      return (IEditRating) new MainForm.RatingEditor((IWin32Window) (Form.ActiveForm ?? (Form) this), activeService == null ? (IEnumerable<ComicBook>) null : activeService.GetBookList(ComicBookFilterType.IsEditable | ComicBookFilterType.Selected));
    }

    public IEditPage GetPageEditor()
    {
      return (IEditPage) new MainForm.PageEditorWrapper(FormUtility.FindActiveService<IEditPage>());
    }

    private MainForm.BookmarkEditorWrapper GetBookmarkEditor()
    {
      return new MainForm.BookmarkEditorWrapper(FormUtility.FindActiveService<IEditBookmark>());
    }

    private void SetBookmark()
    {
      MainForm.BookmarkEditorWrapper bookmarkEditor = this.GetBookmarkEditor();
      if (!bookmarkEditor.CanBookmark)
        return;
      bookmarkEditor.Bookmark = SelectItemDialog.GetName<string>((IWin32Window) (Form.ActiveForm ?? (Form) this), TR.Default["Bookmark", "Bookmark"], bookmarkEditor.BookmarkProposal, (IEnumerable<string>) null);
    }

    private bool SetBookmarkAvailable() => this.GetBookmarkEditor().CanBookmark;

    private void RemoveBookmark() => this.GetBookmarkEditor().Bookmark = string.Empty;

    private bool RemoveBookmarkAvailable()
    {
      return !string.IsNullOrEmpty(this.GetBookmarkEditor().Bookmark);
    }

    public void ExportImage(string name, Image image)
    {
      using (SaveFileDialog saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Title = LocalizeUtility.GetText((Control) this, "SavePageTitle", "Save Page as");
        saveFileDialog.Filter = TR.Load("FileFilter")["PageImageSave", "JPEG Image|*.jpg|Windows Bitmap Image|*.bmp|PNG Image|*.png|GIF Image|*.gif|TIFF Image|*.tif"];
        saveFileDialog.FileName = FileUtility.MakeValidFilename(name);
        saveFileDialog.FilterIndex = Program.Settings.LastExportPageFilterIndex;
        IWin32Window owner = (IWin32Window) (Form.ActiveForm ?? (Form) this);
        if (saveFileDialog.ShowDialog(owner) != DialogResult.OK)
          return;
        Program.Settings.LastExportPageFilterIndex = saveFileDialog.FilterIndex;
        name = saveFileDialog.FileName;
        try
        {
          switch (saveFileDialog.FilterIndex)
          {
            case 1:
              image.SaveImage(MainForm.AddExtension(name, ".jpg"), ImageFormat.Jpeg, 24);
              break;
            case 2:
              image.SaveImage(MainForm.AddExtension(name, ".bmp"), ImageFormat.Bmp, 24);
              break;
            case 3:
              image.SaveImage(MainForm.AddExtension(name, ".png"), ImageFormat.Png, 24);
              break;
            case 4:
              image.SaveImage(MainForm.AddExtension(name, ".gif"), ImageFormat.Gif, 8);
              break;
            case 5:
              image.SaveImage(MainForm.AddExtension(name, ".tif"), ImageFormat.Tiff, 24);
              break;
          }
        }
        catch (Exception ex)
        {
          int num = (int) MessageBox.Show((IWin32Window) this, StringUtility.Format(TR.Messages["CouldNotSaveImage", "Could not save the page image!\nReason: {0}"], (object) ex.Message), TR.Messages["Error", "Error"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
      }
    }

    private static string AddExtension(string file, string ext)
    {
      return !Path.HasExtension(file) ? file + ext : file;
    }

    private void ControlExit()
    {
      if (Program.Settings.CloseMinimizesToTray)
        this.MinimizeToTray();
      else
        this.Close();
    }

    public void ShowNews() => this.ShowNews(true);

    public void ShowPortableDevices(DeviceSyncSettings dss = null, Guid? guid = null)
    {
      DevicesEditDialog.Show((IWin32Window) (Form.ActiveForm ?? (Form) this), (IList<DeviceSyncSettings>) Program.Settings.Devices, dss, guid);
    }

    public void MenuSynchronizeDevices()
    {
      if (Program.QueueManager.SynchronizeDevices())
        return;
      this.ShowPortableDevices();
    }

    public void ShowPreferences(string autoInstallplugin = null)
    {
      KeyboardShortcuts commands = new KeyboardShortcuts(this.ComicDisplay.KeyboardMap);
      if (!PreferencesDialog.Show((IWin32Window) (Form.ActiveForm ?? (Form) this), commands, ScriptUtility.Scripts, autoInstallplugin))
        return;
      this.ComicDisplay.KeyboardMap = commands;
    }

    public void ShowOpenDialog()
    {
      string file = Program.ShowComicOpenDialog((IWin32Window) (Form.ActiveForm ?? (Form) this), this.miOpenComic.Text.Replace("&", ""), true);
      if (file == null)
        return;
      this.OpenSupportedFile(file, Program.Settings.OpenInNewTab);
    }

    public bool OpenSupportedFile(string file, bool newSlot = false, int page = 0, bool fromShell = false)
    {
      if (Path.GetExtension(file).Equals(".crplugin", StringComparison.OrdinalIgnoreCase))
      {
        this.ShowPreferences(file);
        return true;
      }
      if (!Path.GetExtension(file).Equals(".cbl", StringComparison.OrdinalIgnoreCase))
      {
        bool flag = this.books.Open(file, newSlot, Math.Max(0, page - 1));
        if (fromShell && Program.ExtendedSettings.HideBrowserIfShellOpen)
          this.BrowserVisible = false;
        return flag;
      }
      ComicListItem comicListItem = this.ImportComicList(file);
      if (comicListItem == null)
        return false;
      ComicBook[] array = comicListItem.GetBooks().ToArray<ComicBook>();
      return array.Length != 0 && this.books.Open(((IEnumerable<ComicBook>) array).Aggregate<ComicBook>((Func<ComicBook, ComicBook, ComicBook>) ((a, b) => !(a.OpenedTime > b.OpenedTime) ? b : a)), newSlot);
    }

    public void ShowAboutDialog()
    {
      using (Splash splash = new Splash())
      {
        splash.Fade = true;
        splash.Location = splash.Bounds.Align(Screen.FromPoint(this.Location).Bounds, ContentAlignment.MiddleCenter).Location;
        int num = (int) splash.ShowDialog((IWin32Window) this);
      }
    }

    public void ExportCurrentImage()
    {
      if (this.ComicDisplay.Book == null || this.ComicDisplay.Book.Comic == null)
        return;
      using (Image pageImage = (Image) this.ComicDisplay.CreatePageImage())
      {
        if (pageImage == null)
          return;
        this.ExportImage(StringUtility.Format("{0} - {1} {2}", (object) this.ComicDisplay.Book.Comic.Caption, (object) TR.Default["Page", "Page"], (object) (this.ComicDisplay.Book.CurrentPage + 1)), pageImage);
      }
    }

    public bool SyncBrowser()
    {
      if (this.ComicDisplay == null || this.ComicDisplay.Book == null || this.ComicDisplay.Book.Comic == null)
        return false;
      ComicBook comic = this.ComicDisplay.Book.Comic;
      IComicBrowser cb = this.FindServices<IComicBrowser>().FirstOrDefault<IComicBrowser>((Func<IComicBrowser, bool>) (b => b.Library == comic.Container));
      if (cb == null)
        return false;
      this.ToggleBrowser(true, cb);
      if (cb.SelectComic(this.ComicDisplay.Book.Comic))
        return true;
      if (comic.LastOpenedFromListId != Guid.Empty)
      {
        ComicListItem list = cb.Library.ComicLists.GetItems<ComicListItem>().FirstOrDefault<ComicListItem>((Func<ComicListItem, bool>) (li => li.Id == comic.LastOpenedFromListId));
        if (list != null && this.ShowBookInList(cb.Library, list, comic))
          return true;
      }
      return false;
    }

    public void ToggleSidebar()
    {
      ISidebar activeService = this.FindActiveService<ISidebar>();
      if (activeService == null)
        return;
      activeService.Visible = !activeService.Visible;
    }

    public void ToggleSmallPreview()
    {
      ISidebar activeService = this.FindActiveService<ISidebar>();
      if (activeService == null)
        return;
      activeService.Preview = !activeService.Preview;
    }

    public void ToggleInfoPanel()
    {
      ISidebar activeService = this.FindActiveService<ISidebar>();
      if (activeService == null)
        return;
      activeService.Info = !activeService.Info;
    }

    public void ToggleSearchBrowser()
    {
      ISearchOptions activeService = this.FindActiveService<ISearchOptions>();
      if (activeService == null)
        return;
      activeService.SearchBrowserVisible = !activeService.SearchBrowserVisible;
    }

    public void ConvertComic(IEnumerable<ComicBook> books, ExportSetting setting)
    {
      ExportSetting data = setting ?? ExportComicsDialog.Show((IWin32Window) this, Program.ExportComicRackPresets, Program.Settings.ExportUserPresets, Program.Settings.CurrentExportSetting ?? new ExportSetting());
      if (data == null)
        return;
      bool flag = books.All<ComicBook>((Func<ComicBook, bool>) (b => b.EditMode.IsLocalComic()));
      Program.Settings.CurrentExportSetting = data;
      if (flag && (data.Target == ExportTarget.ReplaceSource || data.DeleteOriginal) && !Program.AskQuestion((IWin32Window) this, TR.Messages["AskExport", "You have chosen to delete or replace existing files during export. Are you sure you want to continue?\nThe deleted files will be moved to the Recycle Bin during export. Please make sure there is enough disk space available and the eComics are not located on a network drive!"], TR.Messages["Export", "Export"], HiddenMessageBoxes.ConvertComics))
        return;
      ExportSetting exportSetting = CloneUtility.Clone<ExportSetting>(data);
      if (!flag)
      {
        exportSetting.DeleteOriginal = false;
        if (exportSetting.Target == ExportTarget.ReplaceSource || exportSetting.Target == ExportTarget.SameAsSource)
        {
          if (exportSetting.Target == ExportTarget.ReplaceSource)
            exportSetting.AddToLibrary = true;
          exportSetting.Target = ExportTarget.NewFolder;
          using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
          {
            folderBrowserDialog.Description = TR.Messages["SelectLocalFolder", "Select a local folder to store the remote Books"];
            folderBrowserDialog.ShowNewFolderButton = true;
            if (folderBrowserDialog.ShowDialog((IWin32Window) this) == DialogResult.Cancel || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
              return;
            exportSetting.TargetFolder = folderBrowserDialog.SelectedPath;
          }
        }
      }
      if (exportSetting.Combine)
      {
        if (exportSetting.Target != ExportTarget.Ask)
        {
          Program.QueueManager.ExportComic(books, exportSetting, 0);
        }
        else
        {
          ComicBook cb = books.FirstOrDefault<ComicBook>();
          if (cb == null)
            return;
          ExportSetting setting1 = this.FileSaveDialog(cb, exportSetting);
          if (setting1 == null)
            return;
          Program.QueueManager.ExportComic(books, setting1, 0);
        }
      }
      else
      {
        int num = 0;
        foreach (ComicBook book in books)
        {
          ExportSetting setting2 = exportSetting.Target == ExportTarget.Ask ? this.FileSaveDialog(book, exportSetting) : exportSetting;
          if (setting2 == null)
            break;
          Program.QueueManager.ExportComic(book, setting2, num++);
        }
      }
    }

    private ExportSetting FileSaveDialog(ComicBook cb, ExportSetting cs)
    {
      using (SaveFileDialog saveFileDialog = new SaveFileDialog())
      {
        FileFormat fileFormat = cs.GetFileFormat(cb);
        saveFileDialog.Title = TR.Messages["ExportComicTitle", "Export Book to"];
        saveFileDialog.Filter = ((IEnumerable<FileFormat>) new FileFormat[1]
        {
          fileFormat
        }).GetDialogFilter(false);
        saveFileDialog.FileName = cs.GetTargetFileName(cb, 0);
        saveFileDialog.DefaultExt = fileFormat.MainExtension;
        if (saveFileDialog.ShowDialog((IWin32Window) this) == DialogResult.Cancel)
          return (ExportSetting) null;
        ExportSetting exportSetting = CloneUtility.Clone<ExportSetting>(cs);
        exportSetting.Target = ExportTarget.NewFolder;
        exportSetting.Naming = ExportNaming.Custom;
        exportSetting.CustomNamingStart = 0;
        exportSetting.TargetFolder = Path.GetDirectoryName(saveFileDialog.FileName);
        exportSetting.CustomName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
        return exportSetting;
      }
    }

    private void ToggleZoom(CommandKey key)
    {
      float zoom;
      if ((double) this.ComicDisplay.ImageZoom < 1.0499999523162842)
      {
        zoom = this.lastZoom;
      }
      else
      {
        this.lastZoom = this.ComicDisplay.ImageZoom;
        zoom = 1f;
      }
      if (key.IsMouseButton())
        this.ComicDisplay.ZoomTo(System.Drawing.Point.Empty, zoom);
      else
        this.ComicDisplay.ImageZoom = zoom;
    }

    private void EditWorkspaceDisplaySettings()
    {
      DisplayWorkspace ws = new DisplayWorkspace();
      this.StoreWorkspace(ws);
      ComicDisplaySettingsDialog.Show((IWin32Window) this, this.ComicDisplay.IsHardwareRenderer, ws, (Action<DisplayWorkspace>) (w => this.SetWorkspaceDisplayOptions(ws)));
    }

    private DisplayWorkspace CreateNewWorkspace()
    {
      DisplayWorkspace displayWorkspace = new DisplayWorkspace();
      this.StoreWorkspace(displayWorkspace);
      displayWorkspace.Name = this.lastWorkspaceName ?? TR.Default["Workspace", "Workspace"];
      displayWorkspace.Type = this.lastWorkspaceType;
      return SaveWorkspaceDialog.Show((IWin32Window) this, displayWorkspace) ? displayWorkspace : (DisplayWorkspace) null;
    }

    private void SaveWorkspace()
    {
      DisplayWorkspace newWs = this.CreateNewWorkspace();
      if (newWs == null)
        return;
      this.lastWorkspaceName = newWs.Name;
      this.lastWorkspaceType = newWs.Type;
      int index = Program.Settings.Workspaces.FindIndex((Predicate<DisplayWorkspace>) (ws => ws.Name == newWs.Name));
      if (index != -1)
      {
        Program.Settings.Workspaces[index] = newWs;
      }
      else
      {
        Program.Settings.Workspaces.Add(newWs);
        this.UpdateWorkspaceMenus();
      }
    }

    private void EditWorkspace()
    {
      if (Program.Settings.Workspaces.Count == 0)
        return;
      IList<DisplayWorkspace> collection = ListEditorDialog.Show<DisplayWorkspace>((IWin32Window) (Form.ActiveForm ?? (Form) this), TR.Default["Workspaces"], (IList<DisplayWorkspace>) Program.Settings.Workspaces, new Func<DisplayWorkspace>(this.CreateNewWorkspace), activateAction: (Action<DisplayWorkspace>) (w => this.SetWorkspace(w, true)));
      if (collection == null)
        return;
      Program.Settings.Workspaces.Clear();
      Program.Settings.Workspaces.AddRange((IEnumerable<DisplayWorkspace>) collection);
      this.UpdateWorkspaceMenus();
    }

    private void UpdateWorkspaceMenus()
    {
      this.UpdateWorkspaceMenus(this.tsWorkspaces.DropDownItems);
      this.UpdateWorkspaceMenus(this.miWorkspaces.DropDownItems);
    }

    private void UpdateWorkspaceMenus(ToolStripItemCollection items)
    {
      ToolStripSeparator toolStripSeparator = (ToolStripSeparator) null;
      for (int index = items.Count - 1; index > 0; --index)
      {
        if (items[index] is ToolStripSeparator)
        {
          toolStripSeparator = items[index] as ToolStripSeparator;
          break;
        }
        items.RemoveAt(index);
      }
      if (toolStripSeparator != null)
        toolStripSeparator.Visible = Program.Settings.Workspaces.Count > 0;
      int num = 0;
      foreach (DisplayWorkspace workspace in Program.Settings.Workspaces)
      {
        DisplayWorkspace itemWs = workspace;
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(FormUtility.FixAmpersand(workspace.Name), (Image) null, (EventHandler) ((sender, e) => this.SetWorkspace(CloneUtility.Clone<DisplayWorkspace>(itemWs), true)));
        if (num < 6)
          toolStripMenuItem.ShortcutKeys = (Keys) (327680 | 112 + num++);
        items.Add((ToolStripItem) toolStripMenuItem);
      }
    }

    private void SetWorkspace(DisplayWorkspace workspace, bool remember)
    {
      if (this.ComicDisplay == null)
        return;
      this.SuspendLayout();
      bool enableAnimation = SizableContainer.EnableAnimation;
      VisibilityAnimator.EnableAnimation = SizableContainer.EnableAnimation = false;
      try
      {
        if (remember)
        {
          this.lastWorkspaceName = workspace.Name;
          this.lastWorkspaceType = workspace.Type;
        }
        this.ComicDisplay.FullScreen = false;
        if (workspace.IsWindowLayout)
        {
          if (!workspace.FormBounds.IsEmpty)
          {
            Rectangle b = workspace.FormBounds;
            if (((IEnumerable<Screen>) Screen.AllScreens).Where<Screen>((Func<Screen, bool>) (scr => scr.Bounds.IntersectsWith(b))).FirstOrDefault<Screen>() == null)
            {
              Rectangle bounds = Screen.PrimaryScreen.Bounds;
              b.Width = Math.Min(b.Width, bounds.Width);
              b.Height = Math.Min(b.Height, bounds.Height);
              b = b.Center(bounds);
            }
            this.Bounds = b;
          }
          this.BrowserVisible = workspace.PanelVisible || !this.ComicDisplay.IsValid && Program.Settings.ShowQuickOpen;
          this.mainViewContainer.DockSize = workspace.PanelSize;
          this.BrowserDock = workspace.PanelDock;
          this.ReaderUndocked = workspace.ReaderUndocked;
          this.UndockedReaderBounds = workspace.UndockedReaderBounds;
          this.UndockedReaderState = workspace.UndockedReaderState;
        }
        if (workspace.IsViewsSetup)
        {
          foreach (IDisplayWorkspace service in this.FindServices<IDisplayWorkspace>())
            service.SetWorkspace(workspace);
        }
        if (workspace.IsWindowLayout)
        {
          this.WindowState = workspace.FormState;
          this.ComicDisplay.FullScreen = workspace.FullScreen;
          this.MinimalGui = workspace.MinimalGui;
          ComicBookDialog.PagesConfig = workspace.ComicBookDialogPagesConfig;
        }
        this.SetWorkspaceDisplayOptions(workspace);
      }
      finally
      {
        this.ResumeLayout();
        VisibilityAnimator.EnableAnimation = SizableContainer.EnableAnimation = enableAnimation;
      }
    }

    private void SetWorkspaceDisplayOptions(DisplayWorkspace workspace)
    {
      if (workspace.IsComicPageLayout)
      {
        bool displayChangeAnimation = this.ComicDisplay.DisplayChangeAnimation;
        this.ComicDisplay.DisplayChangeAnimation = false;
        try
        {
          this.ComicDisplay.PageLayout = workspace.Layout.PageLayout;
          this.ComicDisplay.TwoPageNavigation = workspace.Layout.TwoPageAutoScroll;
          this.ComicDisplay.RightToLeftReading = workspace.RightToLeftReading;
          this.ComicDisplay.ImageFitMode = workspace.Layout.PageDisplayMode;
          this.ComicDisplay.ImageFitOnlyIfOversized = workspace.Layout.FitOnlyIfOversized;
          this.ComicDisplay.ImageRotation = workspace.Layout.PageImageRotation;
          this.ComicDisplay.ImageAutoRotate = workspace.Layout.AutoRotate;
          try
          {
            this.ComicDisplay.ImageZoom = workspace.Layout.PageZoom;
          }
          catch
          {
            this.ComicDisplay.DisplayChangeAnimation = displayChangeAnimation;
          }
        }
        finally
        {
          this.ComicDisplay.DisplayChangeAnimation = displayChangeAnimation;
        }
      }
      if (!workspace.IsComicPageDisplay)
        return;
      this.ComicDisplay.RealisticPages = workspace.DrawRealisticPages;
      this.ComicDisplay.BackColor = workspace.BackColor;
      this.ComicDisplay.BackgroundTexture = workspace.BackgroundTexture;
      this.ComicDisplay.PaperTexture = workspace.PaperTexture;
      this.ComicDisplay.PaperTextureStrength = workspace.PaperTextureStrength;
      this.ComicDisplay.ImageBackgroundMode = workspace.PageImageBackgroundMode;
      this.ComicDisplay.PaperTextureLayout = workspace.PaperTextureLayout;
      this.ComicDisplay.BackgroundImageLayout = workspace.BackgroundImageLayout;
      this.ComicDisplay.PageTransitionEffect = workspace.PageTransitionEffect;
      this.ComicDisplay.PageMargin = workspace.PageMargin;
      this.ComicDisplay.PageMarginPercentWidth = workspace.PageMarginPercentWidth;
    }

    private void StoreWorkspace(DisplayWorkspace workspace)
    {
      workspace.FormState = this.WindowState;
      workspace.FormBounds = this.SafeBounds;
      workspace.MinimalGui = this.MinimalGui;
      workspace.PanelDock = this.BrowserDock;
      workspace.PanelVisible = this.BrowserVisible;
      workspace.PanelSize = this.mainViewContainer.DockSize;
      workspace.RightToLeftReading = this.ComicDisplay.RightToLeftReading;
      workspace.FullScreen = this.ComicDisplay.FullScreen;
      workspace.Layout.PageLayout = this.ComicDisplay.PageLayout;
      workspace.Layout.TwoPageAutoScroll = this.ComicDisplay.TwoPageNavigation;
      workspace.Layout.FitOnlyIfOversized = this.ComicDisplay.ImageFitOnlyIfOversized;
      workspace.Layout.PageZoom = this.ComicDisplay.ImageZoom;
      workspace.Layout.PageDisplayMode = this.ComicDisplay.ImageFitMode;
      workspace.Layout.PageImageRotation = this.ComicDisplay.ImageRotation;
      workspace.Layout.AutoRotate = this.ComicDisplay.ImageAutoRotate;
      workspace.DrawRealisticPages = this.ComicDisplay.RealisticPages;
      workspace.BackColor = this.ComicDisplay.BackColor;
      workspace.BackgroundTexture = this.ComicDisplay.BackgroundTexture;
      workspace.PaperTexture = this.ComicDisplay.PaperTexture;
      workspace.PaperTextureStrength = this.ComicDisplay.PaperTextureStrength;
      workspace.PageImageBackgroundMode = this.ComicDisplay.ImageBackgroundMode;
      workspace.PaperTextureLayout = this.ComicDisplay.PaperTextureLayout;
      workspace.BackgroundImageLayout = this.ComicDisplay.BackgroundImageLayout;
      workspace.PageMargin = this.ComicDisplay.PageMargin;
      workspace.PageMarginPercentWidth = this.ComicDisplay.PageMarginPercentWidth;
      workspace.PageTransitionEffect = this.ComicDisplay.PageTransitionEffect;
      workspace.ReaderUndocked = this.ReaderUndocked;
      if (workspace.ReaderUndocked)
      {
        workspace.UndockedReaderBounds = this.UndockedReaderBounds;
        workspace.UndockedReaderState = this.UndockedReaderState;
      }
      foreach (IDisplayWorkspace service in this.FindServices<IDisplayWorkspace>())
        service.StoreWorkspace(workspace);
      workspace.ComicBookDialogPagesConfig = ComicBookDialog.PagesConfig;
      if (workspace.ComicBookDialogPagesConfig == null)
        return;
      workspace.ComicBookDialogPagesConfig.GroupsStatus = (ItemViewGroupsStatus) null;
    }

    public void StoreWorkspace() => this.StoreWorkspace(Program.Settings.CurrentWorkspace);

    public void SetListLayout(DisplayListConfig cfg)
    {
      IComicBrowser activeService = this.FindActiveService<IComicBrowser>();
      if (activeService == null)
        return;
      activeService.ListConfig = cfg;
    }

    public void SetListLayoutToAll(DisplayListConfig dlc = null)
    {
      if (!Program.AskQuestion((IWin32Window) this, TR.Messages["AskSetAllLists", "Are you sure you want to set all Lists to the current layout?"], TR.Messages["Set", "Set"], HiddenMessageBoxes.SetAllListLayouts))
        return;
      if (dlc == null)
      {
        IComicBrowser activeService = this.FindActiveService<IComicBrowser>();
        if (activeService != null)
          dlc = activeService.ListConfig;
      }
      if (dlc == null)
        return;
      Program.Database.ResetDisplayConfigs(dlc);
    }

    public void EditListLayout()
    {
      IComicBrowser cb = this.FindActiveService<IComicBrowser>();
      if (cb == null)
        return;
      DisplayListConfig cfg = cb.ListConfig;
      if (!ListLayoutDialog.Show((IWin32Window) this, cfg, cfg.View.ItemViewMode, (Action<DisplayListConfig>) (ncfg => cb.ListConfig = cfg)))
        return;
      cb.ListConfig = cfg;
    }

    private ListConfiguration CreateListLayout()
    {
      IComicBrowser activeService = this.FindActiveService<IComicBrowser>();
      if (activeService == null)
        return (ListConfiguration) null;
      string name = SelectItemDialog.GetName<ListConfiguration>((IWin32Window) this, TR.Messages["SaveListLayout", "Save List Layout"], TR.Default["Layout", "Layout"], (IEnumerable<ListConfiguration>) Program.Settings.ListConfigurations);
      if (string.IsNullOrEmpty(name))
        return (ListConfiguration) null;
      return new ListConfiguration(name)
      {
        Config = activeService.ListConfig
      };
    }

    public void SaveListLayout()
    {
      ListConfiguration cfg = this.CreateListLayout();
      if (cfg == null)
        return;
      int index = Program.Settings.ListConfigurations.FindIndex((Predicate<ListConfiguration>) (c => c.Name == cfg.Name));
      if (index != -1)
      {
        Program.Settings.ListConfigurations[index] = cfg;
      }
      else
      {
        Program.Settings.ListConfigurations.Add(cfg);
        this.UpdateListConfigMenus();
      }
    }

    public void EditListLayouts()
    {
      if (Program.Settings.ListConfigurations.Count == 0)
        return;
      IList<ListConfiguration> collection = ListEditorDialog.Show<ListConfiguration>((IWin32Window) (Form.ActiveForm ?? (Form) this), TR.Messages["ListLayouts", "List Layouts"], (IList<ListConfiguration>) Program.Settings.ListConfigurations, new Func<ListConfiguration>(this.CreateListLayout), activateAction: (Action<ListConfiguration>) (elc => this.SetListLayout(elc.Config)), setAllAction: (Action<ListConfiguration>) (elc => this.SetListLayoutToAll(elc.Config)));
      if (collection == null)
        return;
      Program.Settings.ListConfigurations.Clear();
      Program.Settings.ListConfigurations.AddRange((IEnumerable<ListConfiguration>) collection);
      this.UpdateListConfigMenus();
    }

    public void UpdateListConfigMenus(ToolStripItemCollection items)
    {
      items.RemoveAll<ToolStripItem>((Predicate<ToolStripItem>) (c => c.Tag is ListConfiguration));
      ToolStripSeparator toolStripSeparator = items.OfType<ToolStripSeparator>().LastOrDefault<ToolStripSeparator>();
      if (toolStripSeparator != null)
        toolStripSeparator.Visible = Program.Settings.ListConfigurations.Count > 0;
      int num = 0;
      TR tr = TR.Load(this.Name);
      foreach (ListConfiguration listConfiguration in Program.Settings.ListConfigurations)
      {
        ListConfiguration itemCfg = listConfiguration;
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(StringUtility.Format(tr["SetLayoutMenu", "Set '{0}' Layout"], (object) FormUtility.FixAmpersand(listConfiguration.Name)), (Image) null, (EventHandler) ((sender, e) => this.SetListLayout(itemCfg.Config)));
        toolStripMenuItem.Tag = (object) itemCfg;
        if (num < 6)
          toolStripMenuItem.ShortcutKeys = (Keys) (327680 | 117 + num++);
        items.Add((ToolStripItem) toolStripMenuItem);
      }
    }

    private void UpdateListConfigMenus()
    {
      this.UpdateListConfigMenus(this.miListLayouts.DropDownItems);
    }

    private void OnOpenRecent(object sender, EventArgs e)
    {
      this.OpenSupportedFile(this.recentFiles[Convert.ToInt32(((ToolStripItem) sender).Text.Substring(0, 2)) - 1], Program.Settings.OpenInNewTab);
    }

    private void RecentFilesMenuOpening(object sender, EventArgs e)
    {
      int num = 0;
      foreach (ToolStripMenuItem dropDownItem in (ArrangedElementCollection) this.miOpenRecent.DropDownItems)
      {
        if (dropDownItem.Image != null)
          dropDownItem.Image.Dispose();
      }
      FormUtility.SafeToolStripClear(this.miOpenRecent.DropDownItems);
      foreach (string recentFile in this.recentFiles)
      {
        if (File.Exists(recentFile))
        {
          string text = (++num).ToString() + " - " + FormUtility.FixAmpersand(FileUtility.GetSafeFileName(recentFile));
          using (IItemLock<ThumbnailImage> image = Program.ImagePool.Thumbs.GetImage((ImageKey) Program.BookFactory.Create(recentFile, CreateBookOption.DoNotAdd).GetFrontCoverThumbnailKey()))
          {
            try
            {
              this.miOpenRecent.DropDownItems.Add((ToolStripItem) new ToolStripMenuItem(text, image == null || image.Item == null ? (Image) null : (Image) image.Item.Bitmap.Resize(16, 16), new EventHandler(this.OnOpenRecent)));
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
    }

    public bool ReaderUndocked
    {
      get => this.readerForm != null;
      set
      {
        if (value == this.ReaderUndocked)
          return;
        if (value)
        {
          this.savedBrowserDockStyle = this.BrowserDock;
          this.savedBrowserVisible = this.BrowserVisible;
          this.BrowserVisible = true;
          this.BrowserDock = DockStyle.Fill;
          this.panelReader.Controls.Remove((Control) this.readerContainer);
          this.readerForm = new ReaderForm(this.ComicDisplay);
          this.readerForm.FormClosing += new FormClosingEventHandler(this.ReaderFormFormClosing);
          this.readerForm.KeyDown += new KeyEventHandler(this.ReaderFormKeyDown);
          this.readerForm.Controls.Add((Control) this.readerContainer);
          this.readerForm.WindowState = this.undockedReaderState;
          if (this.undockedReaderBounds.IsEmpty)
          {
            this.readerForm.StartPosition = FormStartPosition.WindowsDefaultLocation;
          }
          else
          {
            this.readerForm.StartPosition = FormStartPosition.Manual;
            this.readerForm.SafeBounds = this.undockedReaderBounds;
          }
          this.mainView.ShowLibrary();
          if (this.OpenBooks.OpenCount > 0)
            this.readerForm.Show();
        }
        else
        {
          this.undockedReaderState = this.readerForm.WindowState;
          this.undockedReaderBounds = this.readerForm.SafeBounds;
          this.readerForm.Controls.Remove((Control) this.readerContainer);
          this.readerForm.Dispose();
          this.panelReader.Controls.Add((Control) this.readerContainer);
          this.readerForm = (ReaderForm) null;
          this.BrowserDock = this.savedBrowserDockStyle;
          this.BrowserVisible = this.savedBrowserVisible;
        }
        this.OnGuiVisibilities();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Rectangle UndockedReaderBounds
    {
      get => !this.ReaderUndocked ? this.undockedReaderBounds : this.readerForm.SafeBounds;
      set
      {
        if (this.ReaderUndocked)
          this.readerForm.SafeBounds = value;
        else
          this.undockedReaderBounds = value;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public FormWindowState UndockedReaderState
    {
      get => !this.ReaderUndocked ? this.undockedReaderState : this.readerForm.WindowState;
      set
      {
        if (this.ReaderUndocked)
          this.readerForm.WindowState = value;
        else
          this.undockedReaderState = value;
      }
    }

    private void ToggleUndockReader()
    {
      if (!this.ReaderUndocked)
        this.ComicDisplay.FullScreen = false;
      this.ReaderUndocked = !this.ReaderUndocked;
    }

    private void ReaderFormFormClosing(object sender, FormClosingEventArgs e)
    {
      if (this.shieldReaderFormClosing)
        return;
      try
      {
        this.shieldReaderFormClosing = true;
        this.ReaderUndocked = false;
      }
      finally
      {
        this.shieldReaderFormClosing = false;
      }
    }

    private void ReaderFormKeyDown(object sender, KeyEventArgs e)
    {
      e.Handled = this.commands.InvokeKey(e.KeyData);
    }

    private void RebuildBookTabs()
    {
      using (ItemMonitor.Lock(this.OpenBooks.Slots.SyncRoot))
      {
        this.mainMenuStrip.SuspendLayout();
        this.fileTabs.SuspendLayout();
        for (int index = this.fileTabs.Items.Count - 1; index >= 0; --index)
        {
          if (this.fileTabs.Items[index].Tag != null)
            this.fileTabs.Items.RemoveAt(index);
        }
        FormUtility.SafeToolStripClear(this.miOpenNow.DropDownItems);
        FormUtility.SafeToolStripClear(this.cmComics.DropDownItems, this.cmComics.DropDownItems.IndexOf((ToolStripItem) this.cmComicsSep) + 1);
        this.mainView.ClearFileTabs();
        for (int index = 0; index < this.OpenBooks.Slots.Count; ++index)
        {
          string text = FormUtility.FixAmpersand(this.OpenBooks.GetSlotCaption(index));
          ComicBookNavigator nav = this.OpenBooks.Slots[index];
          string str = text;
          string shortcut = (string) null;
          KeysConverter keysConverter = new KeysConverter();
          ToolStripMenuItem tmi = new ToolStripMenuItem(text);
          tmi.Click += new EventHandler(this.OpenBooks_Clicked);
          tmi.Tag = (object) index;
          if (index < 12)
          {
            tmi.ShortcutKeys = (Keys) (393216 | 112 + index);
            shortcut = keysConverter.ConvertToString((object) tmi.ShortcutKeys);
            str = str + "\r\n(" + shortcut + ")";
          }
          this.miOpenNow.DropDownItems.Add((ToolStripItem) tmi);
          ToolStripMenuItem tmi2 = new ToolStripMenuItem(text);
          tmi2.Click += new EventHandler(this.OpenBooks_Clicked);
          tmi2.Tag = (object) index;
          tmi2.ShortcutKeys = tmi.ShortcutKeys;
          this.cmComics.DropDownItems.Add((ToolStripItem) tmi2);
          MainForm.ComicReaderTab comicReaderTab1 = new MainForm.ComicReaderTab(text, nav, this.Font, shortcut);
          comicReaderTab1.Tag = (object) index;
          comicReaderTab1.MinimumWidth = 100;
          comicReaderTab1.CanClose = true;
          comicReaderTab1.ToolTipText = str;
          comicReaderTab1.ContextMenu = this.tabContextMenu;
          TabBar.TabBarItem tbi = (TabBar.TabBarItem) comicReaderTab1;
          if (nav == null)
            tbi.Image = this.emptyTabImage;
          tbi.Selected += new CancelEventHandler(this.OpenBooks_Selected);
          tbi.CloseClick += new EventHandler(this.btn_CloseClick);
          tbi.CaptionClick += new CancelEventHandler(this.tbi_CaptionClick);
          this.fileTabs.Items.Add(tbi);
          MainForm.ComicReaderTab comicReaderTab2 = new MainForm.ComicReaderTab(text, nav, this.Font, shortcut);
          comicReaderTab2.Image = this.emptyTabImage;
          comicReaderTab2.Tag = (object) index;
          comicReaderTab2.MinimumWidth = 100;
          comicReaderTab2.CanClose = true;
          comicReaderTab2.ToolTipText = str;
          comicReaderTab2.ContextMenu = this.tabContextMenu;
          comicReaderTab2.Visible = this.ViewDock == DockStyle.Fill;
          TabBar.TabBarItem tbi2 = (TabBar.TabBarItem) comicReaderTab2;
          if (nav == null)
            tbi2.Image = this.emptyTabImage;
          tbi2.Selected += new CancelEventHandler(this.OpenBooks_Selected);
          tbi2.CloseClick += new EventHandler(this.btn_CloseClick);
          tbi2.CaptionClick += new CancelEventHandler(this.tbi_CaptionClick);
          this.mainView.AddFileTab(tbi2);
          ThreadUtility.RunInBackground("Create tab thumbnails", (ThreadStart) (() =>
          {
            try
            {
              Bitmap thumb;
              using (IItemLock<ThumbnailImage> thumbnail = Program.ImagePool.GetThumbnail(nav.Comic))
                thumb = thumbnail.Item.Bitmap.Resize(16, 16);
              ControlExtensions.Invoke(this, (Action) (() =>
              {
                if (!tmi.IsDisposed)
                  tmi.Image = (Image) thumb;
                if (!tmi2.IsDisposed)
                  tmi2.Image = (Image) thumb;
                tbi.Image = (Image) thumb;
                tbi2.Image = (Image) thumb;
              }));
            }
            catch
            {
            }
          }));
        }
        string text1 = this.miAddTab.Text.Replace("&", string.Empty);
        TabBar.TabBarItem tabBarItem = new TabBar.TabBarItem(text1)
        {
          Tag = (object) -1,
          Image = this.addTabImage,
          MinimumWidth = 32,
          ShowText = false,
          ToolTipText = text1,
          AdjustWidth = false
        };
        tabBarItem.Click += (EventHandler) ((s, ea) =>
        {
          this.OpenBooks.AddSlot();
          this.OpenBooks.CurrentSlot = this.OpenBooks.Slots.Count - 1;
        });
        this.fileTabs.Items.Add(tabBarItem);
        TabBar.TabBarItem tsb = new TabBar.TabBarItem(text1)
        {
          Tag = (object) -1,
          Image = this.addTabImage,
          MinimumWidth = 32,
          AdjustWidth = false,
          ShowText = false,
          ToolTipText = text1,
          Visible = this.ViewDock == DockStyle.Fill
        };
        tsb.Click += (EventHandler) ((s, ea) =>
        {
          this.OpenBooks.AddSlot();
          this.OpenBooks.CurrentSlot = this.OpenBooks.Slots.Count - 1;
        });
        this.mainView.AddFileTab(tsb);
        this.fileTabs.ResumeLayout(false);
        this.fileTabs.PerformLayout();
        this.mainMenuStrip.ResumeLayout(false);
        this.mainMenuStrip.PerformLayout();
        this.OnGuiVisibilities();
        if (this.books.OpenCount != 0 || Program.Settings.ShowQuickOpen)
          return;
        this.BrowserVisible = true;
        this.mainView.ShowLast();
      }
    }

    private void btn_CloseClick(object sender, EventArgs e)
    {
      this.OpenBooks.Close((int) ((TabBar.TabBarItem) sender).Tag);
    }

    private void tbi_CaptionClick(object sender, CancelEventArgs e)
    {
      if (!(sender as TabBar.TabBarItem).IsSelected)
        return;
      this.ToggleBrowser();
      e.Cancel = true;
    }

    private void OpenBooks_Clicked(object sender, EventArgs e)
    {
      this.OpenBooks.CurrentSlot = (int) (sender is ToolStripItem ? ((ToolStripItem) sender).Tag : ((TabBar.TabBarItem) sender).Tag);
    }

    private void OpenBooks_Selected(object sender, CancelEventArgs e)
    {
      this.OpenBooks.CurrentSlot = (int) (sender is ToolStripItem ? ((ToolStripItem) sender).Tag : ((TabBar.TabBarItem) sender).Tag);
    }

    private void OpenBooks_SlotsChanged(
      object sender,
      SmartListChangedEventArgs<ComicBookNavigator> e)
    {
      this.RebuildBookTabs();
    }

    private void OpenBooks_CurrentSlotChanged(object sender, EventArgs e)
    {
      foreach (TabBar.TabBarItem tabBarItem in (SmartList<TabBar.TabBarItem>) this.fileTabs.Items)
      {
        if (tabBarItem.Tag is int && this.OpenBooks.CurrentSlot == (int) tabBarItem.Tag)
          this.fileTabs.SelectedTab = tabBarItem;
      }
      foreach (ToolStripMenuItem toolStripMenuItem in this.miOpenNow.DropDownItems.OfType<ToolStripMenuItem>().Where<ToolStripMenuItem>((Func<ToolStripMenuItem, bool>) (tmi => tmi.Tag is int)))
        toolStripMenuItem.Checked = this.OpenBooks.CurrentSlot == (int) toolStripMenuItem.Tag;
      foreach (ToolStripMenuItem toolStripMenuItem in this.cmComics.DropDownItems.OfType<ToolStripMenuItem>().Where<ToolStripMenuItem>((Func<ToolStripMenuItem, bool>) (tmi => tmi.Tag is int)))
        toolStripMenuItem.Checked = this.OpenBooks.CurrentSlot == (int) toolStripMenuItem.Tag;
      this.mainView.ShowView(this.OpenBooks.CurrentSlot);
      this.UpdateTabCaptions();
      if (this.OpenBooks.CurrentBook == null)
        return;
      Win7.SetActiveThumbnail(this.OpenBooks.CurrentBook.Comic.FilePath);
    }

    private void OpenBooks_CaptionsChanged(object sender, EventArgs e) => this.UpdateTabCaptions();

    private void UpdateTabCaptions()
    {
      using (ItemMonitor.Lock(this.OpenBooks.Slots.SyncRoot))
      {
        foreach (TabBar.TabBarItem tabBarItem in this.fileTabs.Items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (t => t.Tag is int && (int) t.Tag >= 0)))
          tabBarItem.Text = this.OpenBooks.GetSlotCaption((int) tabBarItem.Tag);
        foreach (TabBar.TabBarItem fileTab in this.mainView.GetFileTabs())
          fileTab.Text = this.OpenBooks.GetSlotCaption((int) fileTab.Tag);
        foreach (ToolStripMenuItem dropDownItem in (ArrangedElementCollection) this.miOpenNow.DropDownItems)
        {
          if (dropDownItem.Tag is int)
            dropDownItem.Text = this.OpenBooks.GetSlotCaption((int) dropDownItem.Tag);
        }
      }
    }

    private void InitializePluginHelp()
    {
      IEnumerable<PackageManager.Package> source = Program.ScriptPackages.GetPackages().Where<PackageManager.Package>((Func<PackageManager.Package, bool>) (p => !string.IsNullOrEmpty(p.HelpLink)));
      this.miHelpPlugins.Visible = source.Count<PackageManager.Package>() > 0;
      foreach (PackageManager.Package package in source)
      {
        PackageManager.Package p = package;
        this.miHelpPlugins.DropDownItems.Add(p.Name, p.Image, (EventHandler) ((s, ex) => Program.StartDocument(p.HelpLink, p.PackagePath)));
      }
    }

    private void InitializeHelp(string helpSystem)
    {
      Program.HelpSystem = helpSystem;
      this.miWebHelp.DropDownItems.Clear();
      this.miHelp.Visible = false;
      if (this.miHelp.DropDownItems.Contains((ToolStripItem) this.miWebHelp))
      {
        this.miHelp.DropDownItems.Remove((ToolStripItem) this.miWebHelp);
        this.helpMenu.DropDownItems.Insert(this.helpMenu.DropDownItems.IndexOf((ToolStripItem) this.miHelp) + 1, (ToolStripItem) this.miWebHelp);
      }
      this.miHelp.DropDownItems.Clear();
      ToolStripItem[] array = Program.Help.GetCustomHelpMenu().ToArray<ToolStripItem>();
      if (array.Length != 0)
      {
        this.helpMenu.DropDownItems.Remove((ToolStripItem) this.miWebHelp);
        this.miHelp.Visible = true;
        this.miHelp.DropDownItems.Add((ToolStripItem) this.miWebHelp);
        this.miHelp.DropDownItems.Add((ToolStripItem) new ToolStripSeparator());
        this.miHelp.DropDownItems.AddRange(array);
      }
      IEnumerable<string> helpSystems = Program.HelpSystems;
      this.miChooseHelpSystem.Visible = helpSystems.Count<string>() > 1;
      this.miChooseHelpSystem.DropDownItems.Clear();
      foreach (string str in helpSystems)
      {
        string name = str;
        ((ToolStripMenuItem) this.miChooseHelpSystem.DropDownItems.Add(name, (Image) null, (EventHandler) ((s, e) => Program.Settings.HelpSystem = name))).Checked = Program.HelpSystem == name;
      }
    }

    private void ShowPendingTasks(int tab = 0)
    {
      if (this.taskDialog != null && !this.taskDialog.IsDisposed)
      {
        this.taskDialog.SelectedTab = tab;
        this.taskDialog.Activate();
      }
      else
        this.taskDialog = TasksDialog.Show((IWin32Window) this, Program.QueueManager.GetQueues(), tab);
    }

    private void FocusQuickSearch() => this.FindActiveService<ISearchOptions>()?.FocusQuickSearch();

    private bool CheckSidebarAvailable() => this.FindActiveService<ISidebar>() != null;

    private bool CheckInfoPanelAvailable()
    {
      ISidebar activeService = this.FindActiveService<ISidebar>();
      return activeService != null && activeService.HasInfoPanels;
    }

    private bool CheckSidebarEnabled()
    {
      ISidebar activeService = this.FindActiveService<ISidebar>();
      return activeService != null && activeService.Visible;
    }

    private bool CheckInfoPanelEnabled()
    {
      ISidebar activeService = this.FindActiveService<ISidebar>();
      return activeService != null && activeService.Info;
    }

    private bool CheckSmallPreviewEnabled()
    {
      ISidebar activeService = this.FindActiveService<ISidebar>();
      return activeService != null && activeService.Preview;
    }

    private bool CheckSearchBrowserEnabled()
    {
      ISearchOptions activeService = this.FindActiveService<ISearchOptions>();
      return activeService != null && activeService.SearchBrowserVisible;
    }

    private bool CheckSearchAvailable() => this.FindActiveService<ISearchOptions>() != null;

    private bool CheckViewOptionsAvailable() => this.FindActiveService<IComicBrowser>() != null;

    private Image GetFitModeImage()
    {
      try
      {
        int imageFitMode = (int) this.ComicDisplay.ImageFitMode;
        foreach (ToolStripItem dropDownItem in (ArrangedElementCollection) this.tbFit.DropDownItems)
        {
          if (dropDownItem.Image != null && imageFitMode-- == 0)
            return dropDownItem.Image;
        }
        return (Image) null;
      }
      catch
      {
        return (Image) null;
      }
    }

    private Image GetLayoutImage()
    {
      switch (this.ComicDisplay.PageLayout)
      {
        case PageLayoutMode.Double:
          return !this.ComicDisplay.RightToLeftReading ? this.miTwoPages.Image : MainForm.TwoPagesRtl;
        case PageLayoutMode.DoubleAdaptive:
          return !this.ComicDisplay.RightToLeftReading ? this.miTwoPagesAdaptive.Image : MainForm.TwoPagesAdaptiveRtl;
        default:
          return !this.ComicDisplay.RightToLeftReading ? this.miSinglePage.Image : MainForm.SinglePageRtl;
      }
    }

    private void readerContainer_Paint(object sender, PaintEventArgs e)
    {
      try
      {
        if (!EngineConfiguration.Default.AeroFullScreenWorkaround)
          return;
        e.Graphics.Clear(System.Drawing.Color.Black);
      }
      catch (Exception ex)
      {
      }
    }

    private void tsCurrentPage_Click(object sender, EventArgs e)
    {
      Program.Settings.TrackCurrentPage = !Program.Settings.TrackCurrentPage;
    }

    private void tabContextMenu_Opening(object sender, CancelEventArgs e)
    {
      this.sepBeforeRevealInBrowser.Visible = this.cmRevealInExplorer.Visible = this.cmCopyPath.Visible = this.ComicDisplay == null || this.ComicDisplay.Book == null || this.ComicDisplay.Book.Comic.EditMode.IsLocalComic();
    }

    private void Application_Idle(object sender, EventArgs e) => this.OnUpdateGui();

    private void viewer_BookChanged(object sender, EventArgs e)
    {
      if (!Program.ExtendedSettings.DoNotResetZoomOnBookOpen)
        this.ComicDisplay.ImageZoom = 1f;
      if (this.ComicDisplay.Book == null)
        return;
      this.ComicDisplay.Focus();
    }

    private void WatchedBookHasChanged(object sender, ContainerBookChangedEventArgs e)
    {
      if (!e.IsComicInfo || !e.Book.EditMode.IsLocalComic() || !e.Book.FileInfoRetrieved)
        return;
      e.Book.ComicInfoIsDirty = true;
      if (this.books.IsOpen(e.Book))
        return;
      Program.QueueManager.AddBookToFileUpdate(e.Book);
    }

    private void MagnifySetupChanged(object sender, EventArgs e)
    {
      MagnifySetupControl magnifySetupControl = (MagnifySetupControl) sender;
      this.ComicDisplay.MagnifierOpacity = magnifySetupControl.MagnifyOpaque;
      this.ComicDisplay.MagnifierSize = magnifySetupControl.MagnifySize;
      this.ComicDisplay.MagnifierZoom = magnifySetupControl.MagnifyZoom;
      this.ComicDisplay.MagnifierStyle = magnifySetupControl.MagnifyStyle;
      this.ComicDisplay.AutoHideMagnifier = magnifySetupControl.AutoHideMagnifier;
      this.ComicDisplay.AutoMagnifier = magnifySetupControl.AutoMagnifier;
    }

    private void viewer_PageDisplayModeChanged(object sender, EventArgs e)
    {
      this.tbZoom.Text = string.Format("{0}%", (object) (int) ((double) this.ComicDisplay.ImageZoom * 100.0));
      this.tbRotate.Text = TR.Translate((Enum) this.ComicDisplay.ImageRotation);
      this.tbRotate.Image = this.ComicDisplay.ImageAutoRotate ? this.miAutoRotate.Image : this.miRotateRight.Image;
    }

    private void viewer_FirstPageReached(object sender, EventArgs e)
    {
      if (!Program.Settings.AutoNavigateComics)
        return;
      this.OpenPrevComic();
    }

    private void viewer_LastPageReached(object sender, EventArgs e)
    {
      if (!Program.Settings.AutoNavigateComics)
        return;
      this.OpenNextComic(1, OpenComicOptions.NoMoveToLastPage);
    }

    private void backgroundSaveTimer_Tick(object sender, EventArgs e)
    {
      Program.DatabaseManager.SaveInBackground();
    }

    private void tsExportActivity_Click(object sender, EventArgs e)
    {
      if (Program.QueueManager.ExportErrors.Count != 0)
        ShowErrorsDialog.ShowErrors<ComicExporter>((IWin32Window) this, Program.QueueManager.ExportErrors, new Func<ComicExporter, ShowErrorsDialog.ErrorItem>(ShowErrorsDialog.ComicExporterConverter));
      else
        this.ShowPendingTasks();
    }

    private void tsDeviceSyncActivity_Click(object sender, EventArgs e)
    {
      if (Program.QueueManager.DeviceSyncErrors.Count != 0)
        ShowErrorsDialog.ShowErrors<DeviceSyncError>((IWin32Window) this, Program.QueueManager.DeviceSyncErrors, new Func<DeviceSyncError, ShowErrorsDialog.ErrorItem>(ShowErrorsDialog.DeviceSyncErrorConverter));
      else
        this.ShowPendingTasks();
    }

    private void tsPageActivity_Click(object sender, EventArgs e) => this.ShowPendingTasks();

    private void tsReadInfoActivity_Click(object sender, EventArgs e) => this.ShowPendingTasks();

    private void tsUpdateInfoActivity_Click(object sender, EventArgs e) => this.ShowPendingTasks();

    private void tsScanActivity_Click(object sender, EventArgs e) => this.ShowPendingTasks();

    private void tsServerActivity_Click(object sender, EventArgs e) => this.ShowPendingTasks(1);

    private void pageContextMenu_Opening(object sender, CancelEventArgs e)
    {
      try
      {
        if (this.ComicDisplay == null)
          e.Cancel = true;
        else if (this.ComicDisplay.SupressContextMenu)
        {
          this.ComicDisplay.SupressContextMenu = false;
          e.Cancel = true;
        }
        else
        {
          IEditPage pageEditor = this.GetPageEditor();
          this.pageTypeContextMenu.Enabled = this.pageRotationContextMenu.Enabled = pageEditor.IsValid;
          this.pageTypeContextMenu.Value = (int) pageEditor.PageType;
          this.pageRotationContextMenu.Value = (int) pageEditor.Rotation;
        }
      }
      catch
      {
        e.Cancel = true;
      }
    }

    private void fileMenu_DropDownOpening(object sender, EventArgs e)
    {
      this.miUpdateAllComicFiles.Visible = !Program.Settings.AutoUpdateComicsFiles;
    }

    private void editMenu_DropDownOpening(object sender, EventArgs e)
    {
      try
      {
        bool flag = this.ComicDisplay != null && this.ComicDisplay.Book != null;
        IEditPage pageEditor = this.GetPageEditor();
        this.pageTypeEditMenu.Enabled = this.pageRotationEditMenu.Enabled = pageEditor.IsValid;
        this.pageTypeEditMenu.Value = (int) pageEditor.PageType;
        this.pageRotationEditMenu.Value = (int) pageEditor.Rotation;
        if (this.miUndo.Tag == null)
          this.miUndo.Tag = (object) this.miUndo.Text;
        string undoLabel = Program.Database.Undo.UndoLabel;
        this.miUndo.Text = (string) this.miUndo.Tag + (string.IsNullOrEmpty(undoLabel) ? string.Empty : ": " + undoLabel);
        if (this.miRedo.Tag == null)
          this.miRedo.Tag = (object) this.miRedo.Text;
        string str = Program.Database.Undo.RedoEntries.FirstOrDefault<string>();
        this.miRedo.Text = (string) this.miRedo.Tag + (string.IsNullOrEmpty(str) ? string.Empty : ": " + str);
      }
      catch (Exception ex)
      {
      }
    }

    private void mainViewContainer_ExpandedChanged(object sender, EventArgs e)
    {
      this.OnGuiVisibilities();
      if (!this.Visible)
        return;
      if (!this.mainViewContainer.Expanded)
        this.ComicDisplay.Focus();
      if (!this.mainViewContainer.Expanded || this.mainViewContainer.Dock != DockStyle.Fill)
        return;
      this.mainView.Focus();
    }

    private void ViewerFullScreenChanged(object sender, EventArgs e)
    {
      if (Program.Settings.AutoMinimalGui && !this.ReaderUndocked)
        this.MinimalGui = this.ComicDisplay.FullScreen;
      this.OnGuiVisibilities();
    }

    private void mainViewContainer_DockChanged(object sender, EventArgs e)
    {
      this.OnGuiVisibilities();
      if (this.ReaderUndocked)
        return;
      if (this.mainViewContainer.Dock == DockStyle.Fill)
      {
        this.mainViewContainer.BringToFront();
        if (!this.Controls.Contains((Control) this.viewContainer))
          return;
        this.Controls.Remove((Control) this.viewContainer);
        this.mainView.SetComicViewer((Control) this.viewContainer);
        this.mainView.ShowView(this.books.CurrentSlot);
      }
      else
      {
        if (!this.Controls.Contains((Control) this.viewContainer))
        {
          this.mainView.SetComicViewer((Control) null);
          this.Controls.Add((Control) this.viewContainer);
        }
        this.viewContainer.Visible = true;
        this.viewContainer.BringToFront();
      }
    }

    private void OnGuiVisibilities()
    {
      bool flag1 = !this.MinimalGui;
      bool flag2 = this.books.OpenCount > 0;
      this.miOpenNow.Enabled = this.miOpenNow.DropDownItems.Count > 0;
      this.cmComicsSep.Visible = this.miOpenNow.DropDownItems.Count > 0;
      if (this.ReaderUndocked)
      {
        this.fileTabsVisibility.Visible = flag1;
        this.fileTabs.TopPadding = 2;
        this.mainView.TabBar.TopPadding = 6;
        this.mainView.TabBar.BottomPadding = 0;
        this.statusStripVisibility.Visible = this.MainToolStripVisible = true;
        this.enableAutoHideMenu = false;
        this.mainView.TabBarVisible = true;
        this.mainMenuStripVisibility.Visible = true;
      }
      else
      {
        bool expanded = this.mainViewContainer.Expanded;
        if (this.mainViewContainer.Dock == DockStyle.Fill)
        {
          this.fileTabsVisibility.Visible = false;
          this.MainToolStripVisible = false;
          bool flag3 = flag1 || !this.mainView.IsComicViewer || this.ShowMainMenuNoComicOpen && !flag2;
          this.statusStripVisibility.Visible = this.mainView.TabBarVisible = flag3;
          this.mainMenuStripVisibility.Visible = flag3 && (!this.AutoHideMainMenu || this.ShowMainMenuNoComicOpen && !flag2);
          this.enableAutoHideMenu = !this.mainMenuStripVisibility.Visible & flag1;
          this.mainView.TabBar.TopPadding = this.mainMenuStripVisibility.Visible ? 2 : 6;
          this.mainView.TabBar.BottomPadding = this.mainView.IsComicViewer ? 4 : 0;
        }
        else
        {
          bool flag4 = flag1 | expanded;
          this.MainToolStripVisible = true;
          bool flag5 = flag4 || this.ShowMainMenuNoComicOpen && !flag2;
          this.statusStripVisibility.Visible = this.fileTabsVisibility.Visible = flag5;
          this.mainMenuStripVisibility.Visible = flag5 && (!this.AutoHideMainMenu || this.ShowMainMenuNoComicOpen && !flag2);
          this.enableAutoHideMenu = !this.mainMenuStripVisibility.Visible & flag4;
          this.fileTabs.TopPadding = this.mainMenuStripVisibility.Visible ? 2 : 6;
          this.fileTabs.BottomPadding = 2;
          this.mainView.TabBarVisible = true;
          this.mainView.TabBar.TopPadding = this.mainViewContainer.Dock == DockStyle.Bottom ? 0 : this.fileTabs.TopPadding;
          this.mainView.TabBar.BottomPadding = this.mainViewContainer.Dock == DockStyle.Bottom ? 0 : this.fileTabs.BottomPadding;
        }
      }
      this.fileTabs.PerformLayout();
      this.mainView.TabBar.PerformLayout();
      if (!this.Visible)
        return;
      this.mainViewContainer.Visible = this.mainViewContainer.Expanded || Program.Settings.AlwaysDisplayBrowserDockingGrip;
    }

    private bool MainToolStripVisible
    {
      get => this.fileTabs.Controls.Contains((Control) this.mainToolStrip);
      set
      {
        bool toolStripVisible = this.MainToolStripVisible;
        if (value == toolStripVisible)
          return;
        if (toolStripVisible)
          this.mainView.TabBar.Controls.Remove((Control) this.mainToolStrip);
        else
          this.fileTabs.Controls.Remove((Control) this.mainToolStrip);
        if (value)
        {
          this.fileTabs.Controls.Add((Control) this.mainToolStrip);
        }
        else
        {
          this.mainView.TabBar.Controls.Add((Control) this.mainToolStrip);
          this.mainView.TabBar.Controls.SetChildIndex((Control) this.mainToolStrip, 0);
        }
      }
    }

    public DockStyle ViewDock
    {
      get => this.mainViewContainer.Dock;
      set => this.mainViewContainer.Dock = value;
    }

    private void TrackBar_Scroll(object sender, EventArgs e)
    {
      this.FindActiveService<IItemSize>()?.SetItemSize(this.thumbSize.TrackBar.Value);
    }

    private void mainView_TabChanged(object sender, EventArgs e)
    {
      if (!this.ReaderUndocked && this.BrowserDock == DockStyle.Fill)
        this.BrowserVisible = !this.mainView.IsComicVisible;
      this.OnGuiVisibilities();
    }

    private void tbBookmarks_DropDownOpening(object sender, EventArgs e)
    {
      this.UpdateBookmarkMenu(this.tbBookmarks.DropDownItems, 0);
    }

    private void cmBookmarks_DropDownOpening(object sender, EventArgs e)
    {
      this.UpdateBookmarkMenu(this.cmBookmarks.DropDownItems, 0);
    }

    private void tbPrevPage_DropDownOpening(object sender, EventArgs e)
    {
      this.UpdateBookmarkMenu(this.tbPrevPage.DropDownItems, -1);
    }

    private void tbNextPage_DropDownOpening(object sender, EventArgs e)
    {
      this.UpdateBookmarkMenu(this.tbNextPage.DropDownItems, 1);
    }

    private void miBookmarks_DropDownOpening(object sender, EventArgs e)
    {
      this.UpdateBookmarkMenu(this.miBookmarks.DropDownItems, 0);
    }

    private void UpdateBookmarkMenu(ToolStripItemCollection items, int direction)
    {
      for (int index = items.Count - 1; index >= 0; --index)
      {
        if ("bm".Equals(items[index].Tag))
          items.RemoveAt(index);
      }
      ToolStripItem toolStripItem = items.OfType<ToolStripItem>().FirstOrDefault<ToolStripItem>((Func<ToolStripItem, bool>) (ti => "bms".Equals(ti.Tag)));
      int num = items.IndexOf(toolStripItem) + 1;
      if (toolStripItem != null)
        toolStripItem.Visible = false;
      if (this.books.CurrentBook == null)
        return;
      int n = 0;
      int currentPage = this.books.CurrentBook.CurrentPage;
      IEnumerable<\u003C\u003Ef__AnonymousType0<int, ComicPageInfo>> source = this.books.CurrentBook.Comic.Pages.Select(p => new
      {
        Page = n++,
        Info = p
      }).Where(pi => pi.Info.IsBookmark);
      if (direction < 0)
        source = source.Reverse();
      try
      {
        foreach (var data in source)
        {
          var cpi = data;
          if (direction < 0 && cpi.Page < currentPage || direction > 0 && cpi.Page > currentPage || direction == 0)
          {
            ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(string.Format("{0} ({1} {2})", (object) FormUtility.FixAmpersand(cpi.Info.Bookmark), (object) TR.Default["Page", "Page"], (object) (cpi.Page + 1)), (Image) null, (EventHandler) ((sender, e) =>
            {
              try
              {
                this.ComicDisplay.Book.Navigate(cpi.Page, PageSeekOrigin.Beginning, true);
              }
              catch
              {
              }
            }));
            toolStripMenuItem1.Tag = (object) "bm";
            toolStripMenuItem1.Enabled = cpi.Page != currentPage;
            ToolStripMenuItem toolStripMenuItem2 = toolStripMenuItem1;
            items.Insert(num++, (ToolStripItem) toolStripMenuItem2);
            if (toolStripItem != null)
              toolStripItem.Visible = true;
          }
        }
      }
      catch
      {
      }
    }

    private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
    {
      if (!(this.notifyIcon.Tag is HiddenMessageBoxes))
        return;
      Program.Settings.HiddenMessageBoxes |= (HiddenMessageBoxes) this.notifyIcon.Tag;
      this.notifyIcon.Tag = (object) null;
    }

    private void trimTimer_Tick(object sender, EventArgs e)
    {
      Program.ImagePool.Thumbs.MemoryCache.Trim();
      Program.ImagePool.Pages.MemoryCache.Trim();
      int num = Math.Min(Program.ExtendedSettings.LimitMemory == 0 ? 1024 : Program.ExtendedSettings.LimitMemory, Program.Settings.MaximumMemoryMB);
      if (num == 1024)
        return;
      try
      {
        using (Process currentProcess = Process.GetCurrentProcess())
          currentProcess.MaxWorkingSet = new IntPtr(num.Clamp(50, 1024) * 1024 * 1024);
      }
      catch
      {
      }
    }

    private void tbTools_DropDownOpening(object sender, EventArgs e)
    {
      this.tbSupport.Visible = !Program.Settings.IsActivated;
      this.tbUpdateWebComics.Visible = Program.Database.Books.FirstOrDefault<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsDynamicSource)) != null;
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      if (this.AutoHideMainMenu && this.enableAutoHideMenu && this.menuDown && e.KeyCode == Keys.Menu && Machine.Ticks - this.menuAutoClosed > 500L)
      {
        this.mainMenuStripVisibility.Visible = !this.mainMenuStripVisibility.Visible;
        if (this.mainMenuStripVisibility.Visible)
          this.mainMenuStrip.Items[0].Select();
        else
          this.menuAutoClosed = Machine.Ticks;
      }
      this.menuDown = false;
      base.OnKeyDown(e);
    }

    private void mainMenuStrip_MenuDeactivate(object sender, EventArgs e)
    {
      if (!this.AutoHideMainMenu || !this.enableAutoHideMenu)
        return;
      this.mainMenuStripVisibility.Visible = false;
      this.menuAutoClosed = Machine.Ticks;
    }

    private static string TotalPageInformation(ComicBookNavigator nav)
    {
      if (nav == null)
        return MainForm.NotAvailable;
      return nav.IsIndexRetrievalCompleted || nav.IndexPagesRetrieved == nav.Comic.PageCount ? nav.Comic.PagesAsText : string.Format("{0} ({1})", (object) nav.Comic.PagesAsText, (object) nav.IndexPagesRetrieved);
    }

    private void OnUpdateGui()
    {
      this.UpdateQuickList();
      this.miOpenRecent.Enabled = this.recentFiles.Length != 0;
      string str = this.ComicDisplay.Book == null ? (string) null : this.ComicDisplay.Book.Caption.Ellipsis(60, "...");
      this.tsBook.Text = string.IsNullOrEmpty(str) ? MainForm.None : str;
      if (this.readerForm != null && !this.MinimizedToTray)
      {
        this.readerForm.Visible = this.books.OpenCount > 0;
        this.readerForm.Text = this.tsBook.Text;
      }
      if (this.ComicDisplay.Book == null || string.IsNullOrEmpty(str))
        this.Text = Application.ProductName;
      else
        this.Text = Application.ProductName + " - " + (this.ComicDisplay.Book.Comic.IsInContainer ? str : this.ComicDisplay.Book.Comic.FileName);
      this.tsCurrentPage.Text = this.ComicDisplay.Book == null ? MainForm.NotAvailable : (this.ComicDisplay.Book.CurrentPage + 1).ToString();
      this.tsPageCount.Text = MainForm.TotalPageInformation(this.ComicDisplay.Book);
      IComicBrowser activeService = this.mainView.FindActiveService<IComicBrowser>();
      this.tsText.Text = FormUtility.FixAmpersand(activeService != null ? activeService.SelectionInfo : string.Empty);
      this.tbFit.Image = this.GetFitModeImage();
      this.tbPageLayout.Image = this.GetLayoutImage();
      ToolStripMenuItem miMagnify = this.miMagnify;
      ToolStripMenuItem cmMagnify = this.cmMagnify;
      Image image1;
      this.tbMagnify.Image = image1 = this.ComicDisplay.MagnifierVisible ? MainForm.zoomImage : MainForm.zoomClearImage;
      Image image2;
      Image image3 = image2 = image1;
      cmMagnify.Image = image2;
      Image image4 = image3;
      miMagnify.Image = image4;
      ItemSizeInfo itemSize = this.FindActiveService<IItemSize>()?.GetItemSize();
      this.thumbSize.Visible = this.mainViewContainer.Expanded && itemSize != null;
      if (itemSize != null)
        this.thumbSize.SetSlider(itemSize.Minimum, itemSize.Maximum, itemSize.Value);
      this.miSynchronizeDevices.Visible = this.tsSynchronizeDevices.Visible = Program.Settings.Devices.Count > 0;
      ToolStripMenuItem readMenu = this.readMenu;
      ToolStripSplitButton tbPrevPage = this.tbPrevPage;
      ToolStripSplitButton tbNextPage = this.tbNextPage;
      ToolStripSeparator toolStripSeparator5 = this.toolStripSeparator5;
      ToolStripSplitButton tbPageLayout = this.tbPageLayout;
      ToolStripSplitButton tbFit = this.tbFit;
      ToolStripSplitButton tbZoom = this.tbZoom;
      ToolStripSplitButton tbRotate = this.tbRotate;
      ToolStripSeparator toolStripSeparator7 = this.toolStripSeparator7;
      bool flag1;
      this.tbMagnify.Visible = flag1 = this.IsComicVisible || this.ComicDisplay.Book != null;
      int num1;
      bool flag2 = (num1 = flag1 ? 1 : 0) != 0;
      toolStripSeparator7.Visible = num1 != 0;
      int num2;
      bool flag3 = (num2 = flag2 ? 1 : 0) != 0;
      tbRotate.Visible = num2 != 0;
      int num3;
      bool flag4 = (num3 = flag3 ? 1 : 0) != 0;
      tbZoom.Visible = num3 != 0;
      int num4;
      bool flag5 = (num4 = flag4 ? 1 : 0) != 0;
      tbFit.Visible = num4 != 0;
      int num5;
      bool flag6 = (num5 = flag5 ? 1 : 0) != 0;
      tbPageLayout.Visible = num5 != 0;
      int num6;
      bool flag7 = (num6 = flag6 ? 1 : 0) != 0;
      toolStripSeparator5.Visible = num6 != 0;
      int num7;
      bool flag8 = (num7 = flag7 ? 1 : 0) != 0;
      tbNextPage.Visible = num7 != 0;
      int num8;
      bool flag9 = (num8 = flag8 ? 1 : 0) != 0;
      tbPrevPage.Visible = num8 != 0;
      int num9 = flag9 ? 1 : 0;
      readMenu.Visible = num9 != 0;
    }

    private void UpdateActivityTimerTick(object sender, EventArgs e)
    {
      ToolStripStatusLabel[] source = new ToolStripStatusLabel[5]
      {
        this.tsReadInfoActivity,
        this.tsWriteInfoActivity,
        this.tsScanActivity,
        this.tsExportActivity,
        this.tsDeviceSyncActivity
      };
      int num = Numeric.BinaryHash(((IEnumerable<ToolStripStatusLabel>) source).Select<ToolStripStatusLabel, bool>((Func<ToolStripStatusLabel, bool>) (l => l.Visible)).ToArray<bool>());
      this.tsScanActivity.Visible = Program.Scanner.IsScanning;
      this.tsWriteInfoActivity.Visible = Program.QueueManager.IsInComicFileUpdate;
      this.tsReadInfoActivity.Visible = Program.QueueManager.IsInComicFileRefresh;
      this.tsPageActivity.Visible = Program.ImagePool.IsWorking;
      bool inComicConversion = Program.QueueManager.IsInComicConversion;
      int comicConversions = Program.QueueManager.PendingComicConversions;
      int count1 = Program.QueueManager.ExportErrors.Count;
      this.tsExportActivity.Visible = inComicConversion || count1 > 0;
      if (this.tsExportActivity.Visible)
      {
        this.tsExportActivity.Image = count1 > 0 ? (comicConversions == 0 ? MainForm.exportError : MainForm.exportErrorAnimation) : MainForm.exportAnimation;
        string str = StringUtility.Format(MainForm.ExportingComics, (object) comicConversions);
        if (count1 > 0)
          str = str + "\n" + StringUtility.Format(MainForm.ExportingErrors, (object) count1);
        this.tsExportActivity.ToolTipText = str;
      }
      bool isInDeviceSync = Program.QueueManager.IsInDeviceSync;
      int pendingDeviceSyncs = Program.QueueManager.PendingDeviceSyncs;
      int count2 = Program.QueueManager.DeviceSyncErrors.Count;
      this.tsDeviceSyncActivity.Visible = isInDeviceSync || count2 > 0;
      if (this.tsDeviceSyncActivity.Visible)
      {
        this.tsDeviceSyncActivity.Image = count2 > 0 ? (pendingDeviceSyncs == 0 ? MainForm.deviceSyncError : MainForm.deviceSyncErrorAnimation) : MainForm.deviceSyncAnimation;
        string str = StringUtility.Format(MainForm.DeviceSyncing, (object) pendingDeviceSyncs);
        if (count1 > 0)
          str = str + "\n" + StringUtility.Format(MainForm.DeviceSyncingErrors, (object) count2);
        this.tsDeviceSyncActivity.ToolTipText = str;
      }
      Image image = (Image) null;
      if (this.comicDisplay != null && this.comicDisplay.Book != null && !this.comicDisplay.Book.IsIndexRetrievalCompleted)
        image = MainForm.updatePages;
      this.tsCurrentPage.Image = Program.Settings.TrackCurrentPage ? (Image) null : MainForm.trackPagesLockedImage;
      this.tsPageCount.Image = image;
      int n = Numeric.BinaryHash(((IEnumerable<ToolStripStatusLabel>) source).Select<ToolStripStatusLabel, bool>((Func<ToolStripStatusLabel, bool>) (l => l.Visible)).ToArray<bool>());
      if (n != num)
      {
        int index = Numeric.HighestBit(n);
        if (index == -1)
          Win7.SetOverlayIcon((Bitmap) null, (string) null);
        else
          Win7.SetOverlayIcon(source[index].Image as Bitmap, (string) null);
      }
      this.tsServerActivity.Visible = Program.NetworkManager.HasActiveServers();
      if (this.tsServerActivity.Visible)
      {
        this.tsServerActivity.ToolTipText = string.Format(TR.Messages["ServerActivity", "{0} Server(s) running"], (object) Program.NetworkManager.RunningServers.Count);
        this.tsServerActivity.Image = Program.NetworkManager.RecentServerActivity() ? MainForm.greenLight : MainForm.grayLight;
      }
      bool flag = Program.Database != null && Program.Database.ComicStorage != null;
      this.tsDataSourceState.Visible = flag;
      if (!flag)
        return;
      this.tsDataSourceState.Image = Program.Database.ComicStorage.IsConnected ? MainForm.datasourceConnected : MainForm.datasourceDisconnected;
      this.tsDataSourceState.ToolTipText = Program.Database.ComicStorage.IsConnected ? TR.Messages["DataSourceConnected", "Connected to data source"] : TR.Messages["DataSourceDisconnected", "Disconnected from data source!"];
    }

    public bool Maximized => this.maximized;

    public Rectangle SafeBounds { get; set; }

    public bool MinimizedToTray => this.notifyIcon.Visible;

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (!this.IsHandleCreated || !this.Visible)
        return;
      switch (this.WindowState)
      {
        case FormWindowState.Normal:
          this.UpdateSafeBounds();
          this.maximized = false;
          break;
        case FormWindowState.Minimized:
          if (Program.Settings.MinimizeToTray)
            this.MinimizeToTray();
          else
            this.maximized = false;
          Program.Collect();
          break;
        case FormWindowState.Maximized:
          this.maximized = true;
          break;
      }
    }

    protected override void OnMove(EventArgs e)
    {
      base.OnMove(e);
      this.UpdateSafeBounds();
    }

    private void UpdateSafeBounds()
    {
      if (!this.IsHandleCreated || this.WindowState != FormWindowState.Normal || this.FormBorderStyle == FormBorderStyle.None)
        return;
      this.SafeBounds = this.Bounds;
    }

    private void MinimizeToTray()
    {
      if (this.shieldTray)
        return;
      this.shieldTray = true;
      try
      {
        if (this.readerForm != null)
          this.readerForm.Visible = false;
        this.Visible = false;
        this.notifyIcon.Visible = true;
      }
      finally
      {
        this.shieldTray = false;
      }
    }

    private void RestoreFromTray()
    {
      if (this.shieldTray)
        return;
      this.shieldTray = true;
      try
      {
        this.notifyIcon.Visible = false;
        if (this.readerForm != null)
          this.readerForm.Visible = this.books.OpenCount > 0;
        this.Visible = true;
        this.Bounds = this.SafeBounds;
        this.WindowState = this.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
      }
      finally
      {
        this.shieldTray = false;
      }
    }

    public void RestoreToFront()
    {
      if (this.MinimizedToTray)
        this.RestoreFromTray();
      else if (this.WindowState == FormWindowState.Minimized)
        this.WindowState = FormWindowState.Normal;
      this.BringToFront();
      this.Activate();
    }

    private void NotifyIconMouseDoubleClick(object sender, MouseEventArgs e)
    {
      this.RestoreFromTray();
    }

    private void StartMouseDisabledTimer()
    {
      this.ComicDisplay.MouseClickEnabled = false;
      this.mouseDisableTimer.Stop();
      this.mouseDisableTimer.Start();
    }

    private void pageContextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
    {
      this.StartMouseDisabledTimer();
    }

    private void showDisableTimer_Tick(object sender, EventArgs e)
    {
      this.ComicDisplay.MouseClickEnabled = true;
    }

    public Control Control => (Control) this;

    public bool IsComicVisible
    {
      get
      {
        return this.ReaderUndocked || this.BrowserDock != DockStyle.Fill || this.mainView.IsComicVisible;
      }
    }

    public bool BrowserVisible
    {
      get
      {
        return !this.ReaderUndocked && this.BrowserDock != DockStyle.Fill ? this.mainViewContainer.Expanded : this.savedBrowserVisible;
      }
      set
      {
        if (!this.ReaderUndocked && this.BrowserDock != DockStyle.Fill)
        {
          this.mainViewContainer.Expanded = value;
        }
        else
        {
          this.savedBrowserVisible = value;
          this.mainViewContainer.Expanded = true;
        }
      }
    }

    public DockStyle BrowserDock
    {
      get => !this.ReaderUndocked ? this.mainViewContainer.Dock : this.savedBrowserDockStyle;
      set
      {
        if (this.ReaderUndocked)
        {
          this.savedBrowserDockStyle = value;
          this.mainViewContainer.Dock = DockStyle.Fill;
        }
        else
          this.mainViewContainer.Dock = value;
      }
    }

    public NavigatorManager OpenBooks => this.books;

    public void ShowComic()
    {
      if (this.ReaderUndocked || this.mainViewContainer.Dock != DockStyle.Fill)
        return;
      this.mainView.ShowView(this.books.CurrentSlot);
    }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool MinimalGui
    {
      get => this.minimalGui;
      set
      {
        if (this.minimalGui == value)
          return;
        this.minimalGui = value;
        this.OnGuiVisibilities();
      }
    }

    public bool ShowBookInList(
      ComicLibrary library,
      ComicListItem list,
      ComicBook cb,
      bool switchToList = true)
    {
      if (list == null || cb == null)
        return false;
      ILibraryBrowser firstService = this.FindFirstService<ILibraryBrowser>();
      if (firstService == null)
        return false;
      if (switchToList)
        this.mainView.ShowLibrary(library);
      if (!firstService.SelectList(list.Id))
        return false;
      IComicBrowser activeService = this.FindActiveService<IComicBrowser>();
      return activeService != null && activeService.SelectComic(cb);
    }

    void IApplication.Restart() => this.MenuRestart();

    void IApplication.ScanFolders() => this.StartFullScan();

    void IApplication.SynchronizeDevices() => Program.QueueManager.SynchronizeDevices();

    public IEnumerable<ComicBook> ReadDatabaseBooks(string file)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public int AskQuestion(string question, string buttonText, string optionText)
    {
      switch (QuestionDialog.AskQuestion((IWin32Window) this, question, buttonText, optionText))
      {
        case QuestionResult.Ok:
          return 1;
        case QuestionResult.OkWithOption:
          return 2;
        default:
          return 0;
      }
    }

    public Bitmap GetComicPage(ComicBook cb, int page)
    {
      try
      {
        using (IItemLock<PageImage> page1 = Program.ImagePool.GetPage(cb.GetPageKey(page, BitmapAdjustment.Empty), cb))
          return page1 == null || page1.Item == null || page1.Item.Bitmap == null ? (Bitmap) null : page1.Item.Bitmap.Clone() as Bitmap;
      }
      catch
      {
        return (Bitmap) null;
      }
    }

    public Bitmap GetComicThumbnail(ComicBook cb, int page)
    {
      try
      {
        using (IItemLock<ThumbnailImage> thumbnail = Program.ImagePool.GetThumbnail(cb.GetThumbnailKey(page), cb))
          return thumbnail == null || thumbnail.Item == null || thumbnail.Item.Bitmap == null ? (Bitmap) null : thumbnail.Item.Bitmap.Clone() as Bitmap;
      }
      catch
      {
        return (Bitmap) null;
      }
    }

    public Bitmap GetComicPublisherIcon(ComicBook cb)
    {
      return (ComicBook.PublisherIcons.GetImage(cb.GetPublisherIconKey()) ?? ComicBook.PublisherIcons.GetImage(cb.Publisher)).CreateCopy(true);
    }

    public Bitmap GetComicImprintIcon(ComicBook cb)
    {
      return (ComicBook.PublisherIcons.GetImage(cb.GetImprintIconKey()) ?? ComicBook.PublisherIcons.GetImage(cb.Imprint)).CreateCopy(true);
    }

    public Bitmap GetComicAgeRatingIcon(ComicBook cb)
    {
      return ComicBook.AgeRatingIcons.GetImage(cb.AgeRating).CreateCopy(true);
    }

    public Bitmap GetComicFormatIcon(ComicBook cb)
    {
      return ComicBook.FormatIcons.GetImage(cb.Format).CreateCopy(true);
    }

    public IDictionary<string, string> GetComicFields()
    {
      return ComicBook.GetTranslatedWritableStringProperties();
    }

    public string ReadInternet(string text) => HttpAccess.ReadText(text);

    public IEnumerable<ComicBook> GetLibraryBooks()
    {
      return (IEnumerable<ComicBook>) Program.Database.Books.ToArray();
    }

    public bool RemoveBook(ComicBook cb) => Program.Database.Remove(cb);

    public bool SetCustomBookThumbnail(ComicBook cb, Bitmap bmp)
    {
      if (cb.IsLinked)
        return false;
      cb.CustomThumbnailKey = Program.ImagePool.AddCustomThumbnail(bmp);
      return true;
    }

    public ComicBook GetBook(string file)
    {
      return Program.BookFactory.Create(file, CreateBookOption.AddToTemporary);
    }

    public bool OpenNextComic() => this.OpenNextComic(1);

    public bool OpenPrevComic() => this.OpenNextComic(-1);

    public bool OpenRandomComic() => this.OpenNextComic(0);

    public void SelectComics(IEnumerable<ComicBook> books)
    {
      this.FindActiveService<IComicBrowser>()?.SelectComics(books);
    }

    public void ShowComicInfo(IEnumerable<ComicBook> books)
    {
      books = (books ?? Enumerable.Empty<ComicBook>()).Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.EditMode.CanEditProperties()));
      if (books.IsEmpty<ComicBook>())
        return;
      if (books.Count<ComicBook>() > 1)
      {
        Program.Database.Undo.SetMarker(TR.Messages["UndoEditMultipleComics", "Edit multiple Books"]);
        using (MultipleComicBooksDialog comicBooksDialog = new MultipleComicBooksDialog(books))
        {
          int num = (int) comicBooksDialog.ShowDialog((IWin32Window) this);
        }
      }
      else
      {
        Program.Database.Undo.SetMarker(TR.Messages["UndoShowInfo", "Show Info"]);
        ComicBookDialog.Show((IWin32Window) (Form.ActiveForm ?? (Form) this), books.FirstOrDefault<ComicBook>(), (ComicBook[]) null, (Func<ComicBook, bool>) null);
      }
    }

    public IEnumerable<string> LibraryPaths
    {
      get
      {
        return (IEnumerable<string>) Program.Settings.ScriptingLibraries.Replace("\n", "").Replace("\r", "").Split(';', StringSplitOptions.RemoveEmptyEntries);
      }
    }

    private ComicDisplay CreateComicDisplay()
    {
      ComicDisplayControl comicDisplayControl = new ComicDisplayControl();
      comicDisplayControl.AllowDrop = true;
      comicDisplayControl.Dock = DockStyle.Fill;
      comicDisplayControl.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      comicDisplayControl.MagnifierSize = new System.Drawing.Size(400, 300);
      comicDisplayControl.Name = "pageDisplay";
      comicDisplayControl.Padding = new Padding(16);
      comicDisplayControl.ShowStatusMessage = true;
      comicDisplayControl.ContextMenuStrip = this.pageContextMenu;
      comicDisplayControl.AnamorphicTolerance = Program.ExtendedSettings.AnamorphicScalingTolerance;
      comicDisplayControl.AutoHideCursorDelay = Program.ExtendedSettings.AutoHideCursorDuration;
      ComicDisplayControl pageDisplay = comicDisplayControl;
      pageDisplay.DragDrop += new DragEventHandler(this.BookDragDrop);
      pageDisplay.DragEnter += new DragEventHandler(this.BookDragEnter);
      pageDisplay.BookChanged += new EventHandler(this.viewer_BookChanged);
      pageDisplay.PageDisplayModeChanged += new EventHandler(this.viewer_PageDisplayModeChanged);
      pageDisplay.Resize += (EventHandler) ((s, e) => ScriptUtility.Invoke("ReaderResized", (object) pageDisplay.Width, (object) pageDisplay.Height));
      pageDisplay.VisibleInfoOverlaysChanged += (EventHandler) ((s, e) => Program.Settings.ShowVisiblePagePartOverlay = pageDisplay.VisibleInfoOverlays.HasFlag((Enum) InfoOverlays.PartInfo));
      if (EngineConfiguration.Default.AeroFullScreenWorkaround)
        pageDisplay.SizeChanged += new EventHandler(this.pageDisplay_SizeChanged);
      this.readerContainer.Controls.Add((Control) pageDisplay);
      this.readerContainer.Controls.SetChildIndex((Control) pageDisplay, 0);
      this.readerContainer.Controls.SetChildIndex((Control) this.quickOpenView, 0);
      ComicDisplay comicDisplay = new ComicDisplay((IComicDisplay) pageDisplay);
      FormUtility.ServiceTranslation[(object) pageDisplay] = (object) comicDisplay;
      return comicDisplay;
    }

    private void pageDisplay_SizeChanged(object sender, EventArgs e)
    {
      Control control = sender as Control;
      Screen screen = Screen.FromControl(control);
      if (!(control.Size == screen.Bounds.Size))
        return;
      --control.Height;
    }

    private void QuickOpenVisibleChanged(object sender, EventArgs e)
    {
      if (!this.quickOpenView.Visible)
        return;
      this.quickListDirty = true;
    }

    private void QuickOpenBookActivated(object sender, EventArgs e)
    {
      ComicBook selectedBook = this.quickOpenView.SelectedBook;
      if (selectedBook == null)
        return;
      this.OpenBooks.Open(selectedBook, false);
    }

    private void QuickOpenBooksChanged(object sender, SmartListChangedEventArgs<ComicBook> e)
    {
      if (e.Action != SmartListAction.Remove)
        return;
      this.quickListDirty = true;
    }

    private void UpdateQuickList()
    {
      this.quickOpenView.Visible = this.quickOpenView.Parent.Visible && Program.Settings.ShowQuickOpen && this.OpenBooks.CurrentBook == null && Program.Database.Books.Count > 0;
      if (!this.quickOpenView.Visible)
        return;
      if (!this.quickUpdateRegistered)
      {
        Program.Database.ComicListsChanged += (ComicListChangedEventHandler) ((s, e) =>
        {
          if (e.Change == ComicListItemChange.Statistic)
            return;
          this.quickListDirty = true;
        });
        Program.Database.Books.Changed += new EventHandler<SmartListChangedEventArgs<ComicBook>>(this.QuickOpenBooksChanged);
        this.mainView.ViewAdded += (EventHandler) ((s, e) => this.quickListDirty = true);
        this.mainView.ViewRemoved += (EventHandler) ((s, e) => this.quickListDirty = true);
        this.quickUpdateRegistered = true;
      }
      while (this.quickListDirty)
      {
        this.quickListDirty = false;
        this.FillWithQuickOpenBooks();
      }
    }

    private void FillWithQuickOpenBooks()
    {
      if (this.InvokeIfRequired(new Action(this.FillWithQuickOpenBooks)))
        return;
      List<ShareableComicListItem> list = Program.Database.ComicLists.GetItems<ShareableComicListItem>().Where<ShareableComicListItem>((Func<ShareableComicListItem, bool>) (cli => cli.QuickOpen)).Select<ShareableComicListItem, ShareableComicListItem>((Func<ShareableComicListItem, ShareableComicListItem>) (cli => cli.Clone() as ShareableComicListItem)).ToList<ShareableComicListItem>();
      if (list.Count == 0 || !Program.ExtendedSettings.ReplaceDefaultListsInQuickOpen)
      {
        object obj = (object) this.defaultQuickOpenLists;
        if (obj == null)
          obj = (object) new ShareableComicListItem[3]
          {
            ComicLibrary.DefaultReadingList((ComicLibrary) Program.Database),
            ComicLibrary.DefaultRecentlyReadList((ComicLibrary) Program.Database),
            ComicLibrary.DefaultRecentlyAddedList((ComicLibrary) Program.Database)
          };
        this.defaultQuickOpenLists = (IEnumerable<ShareableComicListItem>) obj;
        list.AddRange(this.defaultQuickOpenLists);
      }
      this.quickOpenView.BeginUpdate();
      try
      {
        int num = 0;
        foreach (ShareableComicListItem shareableComicListItem in list)
        {
          HashSet<ComicBook> comicBookSet = new HashSet<ComicBook>((IEqualityComparer<ComicBook>) ComicBook.GuidEquality);
          foreach (ComicLibrary library in this.mainView.GetLibraries(Program.ExtendedSettings.RemoteLibrariesInQuickOpen, Program.ExtendedSettings.OnlyLocalRemoteLibrariesInQuickOpen))
          {
            shareableComicListItem.Library = library;
            comicBookSet.AddRange<ComicBook>(shareableComicListItem.GetBooks());
          }
          this.quickOpenView.AddGroup((IGroupInfo) new GroupInfo(shareableComicListItem.Name, num++), (IEnumerable<ComicBook>) comicBookSet, Program.ExtendedSettings.QuickOpenListSize);
        }
      }
      finally
      {
        this.quickOpenView.EndUpdate();
      }
    }

    private void quickOpenView_ShowBrowser(object sender, EventArgs e) => this.ToggleBrowser();

    private void quickOpenView_OpenFile(object sender, EventArgs e) => this.ShowOpenDialog();

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.mouseDisableTimer = new System.Windows.Forms.Timer(this.components);
      this.mainMenuStrip = new MenuStrip();
      this.fileMenu = new ToolStripMenuItem();
      this.miOpenComic = new ToolStripMenuItem();
      this.miCloseComic = new ToolStripMenuItem();
      this.miCloseAllComics = new ToolStripMenuItem();
      this.toolStripMenuItem7 = new ToolStripSeparator();
      this.miAddTab = new ToolStripMenuItem();
      this.toolStripMenuItem14 = new ToolStripSeparator();
      this.miAddFolderToLibrary = new ToolStripMenuItem();
      this.miScan = new ToolStripMenuItem();
      this.miUpdateAllComicFiles = new ToolStripMenuItem();
      this.miUpdateWebComics = new ToolStripMenuItem();
      this.miSynchronizeDevices = new ToolStripMenuItem();
      this.miTasks = new ToolStripMenuItem();
      this.miFileAutomation = new ToolStripMenuItem();
      this.toolStripMenuItem57 = new ToolStripSeparator();
      this.miNewComic = new ToolStripMenuItem();
      this.toolStripMenuItem42 = new ToolStripSeparator();
      this.miOpenRemoteLibrary = new ToolStripMenuItem();
      this.toolStripInsertSeperator = new ToolStripSeparator();
      this.miOpenNow = new ToolStripMenuItem();
      this.miOpenRecent = new ToolStripMenuItem();
      this.toolStripMenuItem4 = new ToolStripSeparator();
      this.miRestart = new ToolStripMenuItem();
      this.toolStripMenuItem24 = new ToolStripSeparator();
      this.miExit = new ToolStripMenuItem();
      this.editMenu = new ToolStripMenuItem();
      this.miShowInfo = new ToolStripMenuItem();
      this.toolStripMenuItem43 = new ToolStripSeparator();
      this.miUndo = new ToolStripMenuItem();
      this.miRedo = new ToolStripMenuItem();
      this.toolStripMenuItem22 = new ToolStripSeparator();
      this.miRating = new ToolStripMenuItem();
      this.contextRating = new ContextMenuStrip(this.components);
      this.miRate0 = new ToolStripMenuItem();
      this.toolStripMenuItem12 = new ToolStripSeparator();
      this.miRate1 = new ToolStripMenuItem();
      this.miRate2 = new ToolStripMenuItem();
      this.miRate3 = new ToolStripMenuItem();
      this.miRate4 = new ToolStripMenuItem();
      this.miRate5 = new ToolStripMenuItem();
      this.toolStripMenuItem58 = new ToolStripSeparator();
      this.miQuickRating = new ToolStripMenuItem();
      this.miPageType = new ToolStripMenuItem();
      this.miPageRotate = new ToolStripMenuItem();
      this.miBookmarks = new ToolStripMenuItem();
      this.miSetBookmark = new ToolStripMenuItem();
      this.miRemoveBookmark = new ToolStripMenuItem();
      this.toolStripMenuItem26 = new ToolStripSeparator();
      this.miPrevBookmark = new ToolStripMenuItem();
      this.miNextBookmark = new ToolStripMenuItem();
      this.toolStripMenuItem8 = new ToolStripSeparator();
      this.miLastPageRead = new ToolStripMenuItem();
      this.toolStripMenuItem37 = new ToolStripSeparator();
      this.toolStripMenuItem40 = new ToolStripSeparator();
      this.miCopyPage = new ToolStripMenuItem();
      this.miExportPage = new ToolStripMenuItem();
      this.toolStripMenuItem39 = new ToolStripSeparator();
      this.miViewRefresh = new ToolStripMenuItem();
      this.toolStripSeparator4 = new ToolStripSeparator();
      this.miDevices = new ToolStripMenuItem();
      this.miPreferences = new ToolStripMenuItem();
      this.browseMenu = new ToolStripMenuItem();
      this.miToggleBrowser = new ToolStripMenuItem();
      this.toolStripMenuItem2 = new ToolStripSeparator();
      this.miViewLibrary = new ToolStripMenuItem();
      this.miViewFolders = new ToolStripMenuItem();
      this.miViewPages = new ToolStripMenuItem();
      this.toolStripMenuItem9 = new ToolStripSeparator();
      this.miSidebar = new ToolStripMenuItem();
      this.miSmallPreview = new ToolStripMenuItem();
      this.miSearchBrowser = new ToolStripMenuItem();
      this.miInfoPanel = new ToolStripMenuItem();
      this.toolStripMenuItem56 = new ToolStripSeparator();
      this.miPreviousList = new ToolStripMenuItem();
      this.miNextList = new ToolStripMenuItem();
      this.toolStripMenuItem6 = new ToolStripSeparator();
      this.miWorkspaces = new ToolStripMenuItem();
      this.miSaveWorkspace = new ToolStripMenuItem();
      this.miEditWorkspaces = new ToolStripMenuItem();
      this.miWorkspaceSep = new ToolStripSeparator();
      this.miListLayouts = new ToolStripMenuItem();
      this.miEditListLayout = new ToolStripMenuItem();
      this.miSaveListLayout = new ToolStripMenuItem();
      this.toolStripMenuItem10 = new ToolStripSeparator();
      this.miEditLayouts = new ToolStripMenuItem();
      this.miSetAllListsSame = new ToolStripMenuItem();
      this.miLayoutSep = new ToolStripSeparator();
      this.readMenu = new ToolStripMenuItem();
      this.miFirstPage = new ToolStripMenuItem();
      this.miPrevPage = new ToolStripMenuItem();
      this.miNextPage = new ToolStripMenuItem();
      this.miLastPage = new ToolStripMenuItem();
      this.toolStripMenuItem18 = new ToolStripSeparator();
      this.miPrevFromList = new ToolStripMenuItem();
      this.miNextFromList = new ToolStripMenuItem();
      this.miRandomFromList = new ToolStripMenuItem();
      this.miSyncBrowser = new ToolStripMenuItem();
      this.toolStripMenuItem17 = new ToolStripSeparator();
      this.miPrevTab = new ToolStripMenuItem();
      this.miNextTab = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.miAutoScroll = new ToolStripMenuItem();
      this.miDoublePageAutoScroll = new ToolStripMenuItem();
      this.toolStripMenuItem21 = new ToolStripSeparator();
      this.miTrackCurrentPage = new ToolStripMenuItem();
      this.displayMenu = new ToolStripMenuItem();
      this.miComicDisplaySettings = new ToolStripMenuItem();
      this.toolStripSeparator3 = new ToolStripSeparator();
      this.miPageLayout = new ToolStripMenuItem();
      this.miOriginal = new ToolStripMenuItem();
      this.miFitAll = new ToolStripMenuItem();
      this.miFitWidth = new ToolStripMenuItem();
      this.miFitWidthAdaptive = new ToolStripMenuItem();
      this.miFitHeight = new ToolStripMenuItem();
      this.miBestFit = new ToolStripMenuItem();
      this.toolStripMenuItem27 = new ToolStripSeparator();
      this.miSinglePage = new ToolStripMenuItem();
      this.miTwoPages = new ToolStripMenuItem();
      this.miTwoPagesAdaptive = new ToolStripMenuItem();
      this.miRightToLeft = new ToolStripMenuItem();
      this.toolStripMenuItem44 = new ToolStripSeparator();
      this.miOnlyFitOversized = new ToolStripMenuItem();
      this.miZoom = new ToolStripMenuItem();
      this.miZoomIn = new ToolStripMenuItem();
      this.miZoomOut = new ToolStripMenuItem();
      this.miToggleZoom = new ToolStripMenuItem();
      this.toolStripSeparator14 = new ToolStripSeparator();
      this.miZoom100 = new ToolStripMenuItem();
      this.miZoom125 = new ToolStripMenuItem();
      this.miZoom150 = new ToolStripMenuItem();
      this.miZoom200 = new ToolStripMenuItem();
      this.miZoom400 = new ToolStripMenuItem();
      this.toolStripSeparator15 = new ToolStripSeparator();
      this.miZoomCustom = new ToolStripMenuItem();
      this.miRotation = new ToolStripMenuItem();
      this.miRotateLeft = new ToolStripMenuItem();
      this.miRotateRight = new ToolStripMenuItem();
      this.toolStripMenuItem33 = new ToolStripSeparator();
      this.miRotate0 = new ToolStripMenuItem();
      this.miRotate90 = new ToolStripMenuItem();
      this.miRotate180 = new ToolStripMenuItem();
      this.miRotate270 = new ToolStripMenuItem();
      this.toolStripMenuItem36 = new ToolStripSeparator();
      this.miAutoRotate = new ToolStripMenuItem();
      this.toolStripMenuItem23 = new ToolStripSeparator();
      this.miMinimalGui = new ToolStripMenuItem();
      this.miFullScreen = new ToolStripMenuItem();
      this.miReaderUndocked = new ToolStripMenuItem();
      this.toolStripMenuItem41 = new ToolStripSeparator();
      this.miMagnify = new ToolStripMenuItem();
      this.helpMenu = new ToolStripMenuItem();
      this.miHelp = new ToolStripMenuItem();
      this.miWebHelp = new ToolStripMenuItem();
      this.miHelpPlugins = new ToolStripMenuItem();
      this.miChooseHelpSystem = new ToolStripMenuItem();
      this.miHelpQuickIntro = new ToolStripMenuItem();
      this.toolStripMenuItem3 = new ToolStripSeparator();
      this.miWebHome = new ToolStripMenuItem();
      this.miWebUserForum = new ToolStripMenuItem();
      this.toolStripMenuItem5 = new ToolStripSeparator();
      this.miNews = new ToolStripMenuItem();
      this.miSupport = new ToolStripMenuItem();
      this.toolStripMenuItem25 = new ToolStripSeparator();
      this.miAbout = new ToolStripMenuItem();
      this.statusStrip = new StatusStrip();
      this.tsText = new ToolStripStatusLabel();
      this.tsDeviceSyncActivity = new ToolStripStatusLabel();
      this.tsExportActivity = new ToolStripStatusLabel();
      this.tsReadInfoActivity = new ToolStripStatusLabel();
      this.tsWriteInfoActivity = new ToolStripStatusLabel();
      this.tsPageActivity = new ToolStripStatusLabel();
      this.tsScanActivity = new ToolStripStatusLabel();
      this.tsDataSourceState = new ToolStripStatusLabel();
      this.tsBook = new ToolStripStatusLabel();
      this.tsCurrentPage = new ToolStripStatusLabel();
      this.tsPageCount = new ToolStripStatusLabel();
      this.tsServerActivity = new ToolStripStatusLabel();
      this.pageContextMenu = new ContextMenuStrip(this.components);
      this.cmShowInfo = new ToolStripMenuItem();
      this.cmRating = new ToolStripMenuItem();
      this.contextRating2 = new ContextMenuStrip(this.components);
      this.cmRate0 = new ToolStripMenuItem();
      this.toolStripMenuItem16 = new ToolStripSeparator();
      this.cmRate1 = new ToolStripMenuItem();
      this.cmRate2 = new ToolStripMenuItem();
      this.cmRate3 = new ToolStripMenuItem();
      this.cmRate4 = new ToolStripMenuItem();
      this.cmRate5 = new ToolStripMenuItem();
      this.toolStripSeparator6 = new ToolStripSeparator();
      this.cmQuickRating = new ToolStripMenuItem();
      this.cmPageType = new ToolStripMenuItem();
      this.cmPageRotate = new ToolStripMenuItem();
      this.cmBookmarks = new ToolStripMenuItem();
      this.cmSetBookmark = new ToolStripMenuItem();
      this.cmRemoveBookmark = new ToolStripMenuItem();
      this.toolStripMenuItem32 = new ToolStripSeparator();
      this.cmPrevBookmark = new ToolStripMenuItem();
      this.cmNextBookmark = new ToolStripMenuItem();
      this.toolStripSeparator13 = new ToolStripSeparator();
      this.cmLastPageRead = new ToolStripMenuItem();
      this.cmBookmarkSeparator = new ToolStripSeparator();
      this.toolStripSeparator10 = new ToolStripSeparator();
      this.cmComics = new ToolStripMenuItem();
      this.cmOpenComic = new ToolStripMenuItem();
      this.cmCloseComic = new ToolStripMenuItem();
      this.toolStripMenuItem13 = new ToolStripSeparator();
      this.cmPrevFromList = new ToolStripMenuItem();
      this.cmNextFromList = new ToolStripMenuItem();
      this.cmRandomFromList = new ToolStripMenuItem();
      this.cmComicsSep = new ToolStripSeparator();
      this.cmPageLayout = new ToolStripMenuItem();
      this.cmOriginal = new ToolStripMenuItem();
      this.cmFitAll = new ToolStripMenuItem();
      this.cmFitWidth = new ToolStripMenuItem();
      this.cmFitWidthAdaptive = new ToolStripMenuItem();
      this.cmFitHeight = new ToolStripMenuItem();
      this.cmFitBest = new ToolStripMenuItem();
      this.toolStripMenuItem29 = new ToolStripSeparator();
      this.cmSinglePage = new ToolStripMenuItem();
      this.cmTwoPages = new ToolStripMenuItem();
      this.cmTwoPagesAdaptive = new ToolStripMenuItem();
      this.cmRightToLeft = new ToolStripMenuItem();
      this.toolStripMenuItem38 = new ToolStripSeparator();
      this.cmRotate0 = new ToolStripMenuItem();
      this.cmRotate90 = new ToolStripMenuItem();
      this.cmRotate180 = new ToolStripMenuItem();
      this.cmRotate270 = new ToolStripMenuItem();
      this.toolStripMenuItem55 = new ToolStripSeparator();
      this.cmOnlyFitOversized = new ToolStripMenuItem();
      this.cmMagnify = new ToolStripMenuItem();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.cmCopyPage = new ToolStripMenuItem();
      this.cmExportPage = new ToolStripMenuItem();
      this.toolStripMenuItem11 = new ToolStripSeparator();
      this.cmRefreshPage = new ToolStripMenuItem();
      this.toolStripMenuItem46 = new ToolStripSeparator();
      this.cmMinimalGui = new ToolStripMenuItem();
      this.notifyIcon = new NotifyIcon(this.components);
      this.notfifyContextMenu = new ContextMenuStrip(this.components);
      this.cmNotifyRestore = new ToolStripMenuItem();
      this.toolStripMenuItem15 = new ToolStripSeparator();
      this.cmNotifyExit = new ToolStripMenuItem();
      this.viewContainer = new Panel();
      this.panelReader = new Panel();
      this.readerContainer = new Panel();
      this.quickOpenView = new QuickOpenView();
      this.fileTabs = new TabBar();
      this.mainToolStrip = new ToolStrip();
      this.tbPrevPage = new ToolStripSplitButton();
      this.tbFirstPage = new ToolStripMenuItem();
      this.tbPrevBookmark = new ToolStripMenuItem();
      this.toolStripMenuItem53 = new ToolStripSeparator();
      this.toolStripMenuItem19 = new ToolStripSeparator();
      this.tbPrevFromList = new ToolStripMenuItem();
      this.tbNextPage = new ToolStripSplitButton();
      this.tbLastPage = new ToolStripMenuItem();
      this.tbNextBookmark = new ToolStripMenuItem();
      this.tbLastPageRead = new ToolStripMenuItem();
      this.toolStripMenuItem28 = new ToolStripSeparator();
      this.toolStripMenuItem49 = new ToolStripSeparator();
      this.tbNextFromList = new ToolStripMenuItem();
      this.tbRandomFromList = new ToolStripMenuItem();
      this.toolStripSeparator5 = new ToolStripSeparator();
      this.tbPageLayout = new ToolStripSplitButton();
      this.tbSinglePage = new ToolStripMenuItem();
      this.tbTwoPages = new ToolStripMenuItem();
      this.tbTwoPagesAdaptive = new ToolStripMenuItem();
      this.toolStripMenuItem54 = new ToolStripSeparator();
      this.tbRightToLeft = new ToolStripMenuItem();
      this.tbFit = new ToolStripSplitButton();
      this.tbOriginal = new ToolStripMenuItem();
      this.tbFitAll = new ToolStripMenuItem();
      this.tbFitWidth = new ToolStripMenuItem();
      this.tbFitWidthAdaptive = new ToolStripMenuItem();
      this.tbFitHeight = new ToolStripMenuItem();
      this.tbBestFit = new ToolStripMenuItem();
      this.toolStripMenuItem20 = new ToolStripSeparator();
      this.tbOnlyFitOversized = new ToolStripMenuItem();
      this.tbZoom = new ToolStripSplitButton();
      this.tbZoomIn = new ToolStripMenuItem();
      this.tbZoomOut = new ToolStripMenuItem();
      this.toolStripMenuItem30 = new ToolStripSeparator();
      this.tbZoom100 = new ToolStripMenuItem();
      this.tbZoom125 = new ToolStripMenuItem();
      this.tbZoom150 = new ToolStripMenuItem();
      this.tbZoom200 = new ToolStripMenuItem();
      this.tbZoom400 = new ToolStripMenuItem();
      this.toolStripMenuItem31 = new ToolStripSeparator();
      this.tbZoomCustom = new ToolStripMenuItem();
      this.tbRotate = new ToolStripSplitButton();
      this.tbRotateLeft = new ToolStripMenuItem();
      this.tbRotateRight = new ToolStripMenuItem();
      this.toolStripSeparator11 = new ToolStripSeparator();
      this.tbRotate0 = new ToolStripMenuItem();
      this.tbRotate90 = new ToolStripMenuItem();
      this.tbRotate180 = new ToolStripMenuItem();
      this.tbRotate270 = new ToolStripMenuItem();
      this.toolStripMenuItem34 = new ToolStripSeparator();
      this.tbAutoRotate = new ToolStripMenuItem();
      this.toolStripSeparator7 = new ToolStripSeparator();
      this.tbMagnify = new ToolStripSplitButton();
      this.tbFullScreen = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.tbTools = new ToolStripDropDownButton();
      this.toolsContextMenu = new ContextMenuStrip(this.components);
      this.tbOpenComic = new ToolStripMenuItem();
      this.tbOpenRemoteLibrary = new ToolStripMenuItem();
      this.tbShowInfo = new ToolStripMenuItem();
      this.toolStripMenuItem47 = new ToolStripSeparator();
      this.tsWorkspaces = new ToolStripMenuItem();
      this.tsSaveWorkspace = new ToolStripMenuItem();
      this.tsEditWorkspaces = new ToolStripMenuItem();
      this.tsWorkspaceSep = new ToolStripSeparator();
      this.tbBookmarks = new ToolStripMenuItem();
      this.tbSetBookmark = new ToolStripMenuItem();
      this.tbRemoveBookmark = new ToolStripMenuItem();
      this.tbBookmarkSeparator = new ToolStripSeparator();
      this.tbAutoScroll = new ToolStripMenuItem();
      this.toolStripMenuItem45 = new ToolStripSeparator();
      this.tbMinimalGui = new ToolStripMenuItem();
      this.tbReaderUndocked = new ToolStripMenuItem();
      this.toolStripMenuItem52 = new ToolStripSeparator();
      this.tbScan = new ToolStripMenuItem();
      this.tbUpdateAllComicFiles = new ToolStripMenuItem();
      this.tbUpdateWebComics = new ToolStripMenuItem();
      this.tsSynchronizeDevices = new ToolStripMenuItem();
      this.toolStripMenuItem48 = new ToolStripSeparator();
      this.tbComicDisplaySettings = new ToolStripMenuItem();
      this.tbPreferences = new ToolStripMenuItem();
      this.tbSupport = new ToolStripMenuItem();
      this.tbAbout = new ToolStripMenuItem();
      this.toolStripMenuItem50 = new ToolStripSeparator();
      this.tbShowMainMenu = new ToolStripMenuItem();
      this.toolStripMenuItem51 = new ToolStripSeparator();
      this.tbExit = new ToolStripMenuItem();
      this.tabContextMenu = new ContextMenuStrip(this.components);
      this.cmClose = new ToolStripMenuItem();
      this.cmCloseAllButThis = new ToolStripMenuItem();
      this.cmCloseAllToTheRight = new ToolStripMenuItem();
      this.toolStripMenuItem35 = new ToolStripSeparator();
      this.cmSyncBrowser = new ToolStripMenuItem();
      this.sepBeforeRevealInBrowser = new ToolStripSeparator();
      this.cmRevealInExplorer = new ToolStripMenuItem();
      this.cmCopyPath = new ToolStripMenuItem();
      this.trimTimer = new System.Windows.Forms.Timer(this.components);
      this.mainViewContainer = new SizableContainer();
      this.mainView = new MainView();
      this.updateActivityTimer = new System.Windows.Forms.Timer(this.components);
      this.mainMenuStrip.SuspendLayout();
      this.contextRating.SuspendLayout();
      this.statusStrip.SuspendLayout();
      this.pageContextMenu.SuspendLayout();
      this.contextRating2.SuspendLayout();
      this.notfifyContextMenu.SuspendLayout();
      this.viewContainer.SuspendLayout();
      this.panelReader.SuspendLayout();
      this.readerContainer.SuspendLayout();
      this.fileTabs.SuspendLayout();
      this.mainToolStrip.SuspendLayout();
      this.toolsContextMenu.SuspendLayout();
      this.tabContextMenu.SuspendLayout();
      this.mainViewContainer.SuspendLayout();
      this.SuspendLayout();
      this.mouseDisableTimer.Interval = 500;
      this.mouseDisableTimer.Tick += new EventHandler(this.showDisableTimer_Tick);
      this.mainMenuStrip.Items.AddRange(new ToolStripItem[6]
      {
        (ToolStripItem) this.fileMenu,
        (ToolStripItem) this.editMenu,
        (ToolStripItem) this.browseMenu,
        (ToolStripItem) this.readMenu,
        (ToolStripItem) this.displayMenu,
        (ToolStripItem) this.helpMenu
      });
      this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
      this.mainMenuStrip.Name = "mainMenuStrip";
      this.mainMenuStrip.Size = new System.Drawing.Size(744, 24);
      this.mainMenuStrip.TabIndex = 0;
      this.mainMenuStrip.MenuDeactivate += new EventHandler(this.mainMenuStrip_MenuDeactivate);
      this.fileMenu.DropDownItems.AddRange(new ToolStripItem[24]
      {
        (ToolStripItem) this.miOpenComic,
        (ToolStripItem) this.miCloseComic,
        (ToolStripItem) this.miCloseAllComics,
        (ToolStripItem) this.toolStripMenuItem7,
        (ToolStripItem) this.miAddTab,
        (ToolStripItem) this.toolStripMenuItem14,
        (ToolStripItem) this.miAddFolderToLibrary,
        (ToolStripItem) this.miScan,
        (ToolStripItem) this.miUpdateAllComicFiles,
        (ToolStripItem) this.miUpdateWebComics,
        (ToolStripItem) this.miSynchronizeDevices,
        (ToolStripItem) this.miTasks,
        (ToolStripItem) this.miFileAutomation,
        (ToolStripItem) this.toolStripMenuItem57,
        (ToolStripItem) this.miNewComic,
        (ToolStripItem) this.toolStripMenuItem42,
        (ToolStripItem) this.miOpenRemoteLibrary,
        (ToolStripItem) this.toolStripInsertSeperator,
        (ToolStripItem) this.miOpenNow,
        (ToolStripItem) this.miOpenRecent,
        (ToolStripItem) this.toolStripMenuItem4,
        (ToolStripItem) this.miRestart,
        (ToolStripItem) this.toolStripMenuItem24,
        (ToolStripItem) this.miExit
      });
      this.fileMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
      this.fileMenu.Name = "fileMenu";
      this.fileMenu.Size = new System.Drawing.Size(37, 20);
      this.fileMenu.Text = "&File";
      this.fileMenu.DropDownOpening += new EventHandler(this.fileMenu_DropDownOpening);
      this.miOpenComic.Image = (Image) Resources.Open;
      this.miOpenComic.Name = "miOpenComic";
      this.miOpenComic.ShortcutKeys = Keys.O | Keys.Control;
      this.miOpenComic.Size = new System.Drawing.Size(296, 38);
      this.miOpenComic.Text = "&Open File...";
      this.miCloseComic.Name = "miCloseComic";
      this.miCloseComic.ShortcutKeys = Keys.X | Keys.Control;
      this.miCloseComic.Size = new System.Drawing.Size(296, 38);
      this.miCloseComic.Text = "&Close";
      this.miCloseAllComics.Name = "miCloseAllComics";
      this.miCloseAllComics.ShortcutKeys = Keys.X | Keys.Shift | Keys.Control;
      this.miCloseAllComics.Size = new System.Drawing.Size(296, 38);
      this.miCloseAllComics.Text = "Close A&ll";
      this.toolStripMenuItem7.Name = "toolStripMenuItem7";
      this.toolStripMenuItem7.Size = new System.Drawing.Size(293, 6);
      this.miAddTab.Image = (Image) Resources.NewTab;
      this.miAddTab.Name = "miAddTab";
      this.miAddTab.ShortcutKeys = Keys.T | Keys.Control;
      this.miAddTab.Size = new System.Drawing.Size(296, 38);
      this.miAddTab.Text = "New &Tab";
      this.toolStripMenuItem14.Name = "toolStripMenuItem14";
      this.toolStripMenuItem14.Size = new System.Drawing.Size(293, 6);
      this.miAddFolderToLibrary.Image = (Image) Resources.AddFolder;
      this.miAddFolderToLibrary.Name = "miAddFolderToLibrary";
      this.miAddFolderToLibrary.ShortcutKeys = Keys.A | Keys.Shift | Keys.Control;
      this.miAddFolderToLibrary.Size = new System.Drawing.Size(296, 38);
      this.miAddFolderToLibrary.Text = "&Add Folder to Library...";
      this.miScan.Image = (Image) Resources.Scan;
      this.miScan.Name = "miScan";
      this.miScan.ShortcutKeys = Keys.S | Keys.Shift | Keys.Control;
      this.miScan.Size = new System.Drawing.Size(296, 38);
      this.miScan.Text = "Scan Book &Folders";
      this.miUpdateAllComicFiles.Image = (Image) Resources.UpdateSmall;
      this.miUpdateAllComicFiles.Name = "miUpdateAllComicFiles";
      this.miUpdateAllComicFiles.ShortcutKeys = Keys.U | Keys.Shift | Keys.Control;
      this.miUpdateAllComicFiles.Size = new System.Drawing.Size(296, 38);
      this.miUpdateAllComicFiles.Text = "Update all Book Files";
      this.miUpdateWebComics.Image = (Image) Resources.UpdateWeb;
      this.miUpdateWebComics.Name = "miUpdateWebComics";
      this.miUpdateWebComics.ShortcutKeys = Keys.W | Keys.Shift | Keys.Control;
      this.miUpdateWebComics.Size = new System.Drawing.Size(296, 38);
      this.miUpdateWebComics.Text = "Update Web Comics";
      this.miSynchronizeDevices.Image = (Image) Resources.DeviceSync;
      this.miSynchronizeDevices.Name = "miSynchronizeDevices";
      this.miSynchronizeDevices.Size = new System.Drawing.Size(296, 38);
      this.miSynchronizeDevices.Text = "Synchronize Devices";
      this.miTasks.Image = (Image) Resources.BackgroundJob;
      this.miTasks.Name = "miTasks";
      this.miTasks.ShortcutKeys = Keys.T | Keys.Shift | Keys.Control;
      this.miTasks.Size = new System.Drawing.Size(296, 38);
      this.miTasks.Text = "&Tasks...";
      this.miFileAutomation.Name = "miFileAutomation";
      this.miFileAutomation.Size = new System.Drawing.Size(296, 38);
      this.miFileAutomation.Text = "A&utomation";
      this.toolStripMenuItem57.Name = "toolStripMenuItem57";
      this.toolStripMenuItem57.Size = new System.Drawing.Size(293, 6);
      this.miNewComic.Name = "miNewComic";
      this.miNewComic.ShortcutKeys = Keys.N | Keys.Shift | Keys.Control;
      this.miNewComic.Size = new System.Drawing.Size(296, 38);
      this.miNewComic.Text = "&New fileless Book Entry...";
      this.toolStripMenuItem42.Name = "toolStripMenuItem42";
      this.toolStripMenuItem42.Size = new System.Drawing.Size(293, 6);
      this.miOpenRemoteLibrary.Image = (Image) Resources.RemoteDatabase;
      this.miOpenRemoteLibrary.Name = "miOpenRemoteLibrary";
      this.miOpenRemoteLibrary.ShortcutKeys = Keys.R | Keys.Shift | Keys.Control;
      this.miOpenRemoteLibrary.Size = new System.Drawing.Size(296, 38);
      this.miOpenRemoteLibrary.Text = "Open Remote Library...";
      this.toolStripInsertSeperator.Name = "toolStripInsertSeperator";
      this.toolStripInsertSeperator.Size = new System.Drawing.Size(293, 6);
      this.miOpenNow.Name = "miOpenNow";
      this.miOpenNow.Size = new System.Drawing.Size(296, 38);
      this.miOpenNow.Text = "Open Books";
      this.miOpenRecent.Name = "miOpenRecent";
      this.miOpenRecent.Size = new System.Drawing.Size(296, 38);
      this.miOpenRecent.Text = "&Recent Books";
      this.miOpenRecent.DropDownOpening += new EventHandler(this.RecentFilesMenuOpening);
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(293, 6);
      this.miRestart.Image = (Image) Resources.Restart;
      this.miRestart.Name = "miRestart";
      this.miRestart.ShortcutKeys = Keys.Q | Keys.Shift | Keys.Control;
      this.miRestart.Size = new System.Drawing.Size(296, 38);
      this.miRestart.Text = "Rest&art";
      this.toolStripMenuItem24.Name = "toolStripMenuItem24";
      this.toolStripMenuItem24.Size = new System.Drawing.Size(293, 6);
      this.miExit.Name = "miExit";
      this.miExit.ShortcutKeys = Keys.Q | Keys.Control;
      this.miExit.Size = new System.Drawing.Size(296, 38);
      this.miExit.Text = "&Exit";
      this.editMenu.DropDownItems.AddRange(new ToolStripItem[17]
      {
        (ToolStripItem) this.miShowInfo,
        (ToolStripItem) this.toolStripMenuItem43,
        (ToolStripItem) this.miUndo,
        (ToolStripItem) this.miRedo,
        (ToolStripItem) this.toolStripMenuItem22,
        (ToolStripItem) this.miRating,
        (ToolStripItem) this.miPageType,
        (ToolStripItem) this.miPageRotate,
        (ToolStripItem) this.miBookmarks,
        (ToolStripItem) this.toolStripMenuItem40,
        (ToolStripItem) this.miCopyPage,
        (ToolStripItem) this.miExportPage,
        (ToolStripItem) this.toolStripMenuItem39,
        (ToolStripItem) this.miViewRefresh,
        (ToolStripItem) this.toolStripSeparator4,
        (ToolStripItem) this.miDevices,
        (ToolStripItem) this.miPreferences
      });
      this.editMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
      this.editMenu.Name = "editMenu";
      this.editMenu.Size = new System.Drawing.Size(39, 20);
      this.editMenu.Text = "&Edit";
      this.editMenu.DropDownOpening += new EventHandler(this.editMenu_DropDownOpening);
      this.miShowInfo.Image = (Image) Resources.GetInfo;
      this.miShowInfo.Name = "miShowInfo";
      this.miShowInfo.ShortcutKeys = Keys.I | Keys.Control;
      this.miShowInfo.Size = new System.Drawing.Size(219, 22);
      this.miShowInfo.Text = "Info...";
      this.toolStripMenuItem43.Name = "toolStripMenuItem43";
      this.toolStripMenuItem43.Size = new System.Drawing.Size(216, 6);
      this.miUndo.Image = (Image) Resources.Undo;
      this.miUndo.Name = "miUndo";
      this.miUndo.ShortcutKeys = Keys.Z | Keys.Control;
      this.miUndo.Size = new System.Drawing.Size(219, 22);
      this.miUndo.Text = "&Undo";
      this.miRedo.Image = (Image) Resources.Redo;
      this.miRedo.Name = "miRedo";
      this.miRedo.ShortcutKeys = Keys.Y | Keys.Control;
      this.miRedo.Size = new System.Drawing.Size(219, 22);
      this.miRedo.Text = "&Redo";
      this.toolStripMenuItem22.Name = "toolStripMenuItem22";
      this.toolStripMenuItem22.Size = new System.Drawing.Size(216, 6);
      this.miRating.DropDown = (ToolStripDropDown) this.contextRating;
      this.miRating.Name = "miRating";
      this.miRating.Size = new System.Drawing.Size(219, 22);
      this.miRating.Text = "My R&ating";
      this.contextRating.Items.AddRange(new ToolStripItem[9]
      {
        (ToolStripItem) this.miRate0,
        (ToolStripItem) this.toolStripMenuItem12,
        (ToolStripItem) this.miRate1,
        (ToolStripItem) this.miRate2,
        (ToolStripItem) this.miRate3,
        (ToolStripItem) this.miRate4,
        (ToolStripItem) this.miRate5,
        (ToolStripItem) this.toolStripMenuItem58,
        (ToolStripItem) this.miQuickRating
      });
      this.contextRating.Name = "contextRating";
      this.contextRating.Size = new System.Drawing.Size(286, 170);
      this.miRate0.Name = "miRate0";
      this.miRate0.ShortcutKeys = Keys.D0 | Keys.Shift | Keys.Alt;
      this.miRate0.Size = new System.Drawing.Size(285, 22);
      this.miRate0.Text = "None";
      this.toolStripMenuItem12.Name = "toolStripMenuItem12";
      this.toolStripMenuItem12.Size = new System.Drawing.Size(282, 6);
      this.miRate1.Name = "miRate1";
      this.miRate1.ShortcutKeys = Keys.D1 | Keys.Shift | Keys.Alt;
      this.miRate1.Size = new System.Drawing.Size(285, 22);
      this.miRate1.Text = "* (1 Star)";
      this.miRate2.Name = "miRate2";
      this.miRate2.ShortcutKeys = Keys.D2 | Keys.Shift | Keys.Alt;
      this.miRate2.Size = new System.Drawing.Size(285, 22);
      this.miRate2.Text = "** (2 Stars)";
      this.miRate3.Name = "miRate3";
      this.miRate3.ShortcutKeys = Keys.D3 | Keys.Shift | Keys.Alt;
      this.miRate3.Size = new System.Drawing.Size(285, 22);
      this.miRate3.Text = "*** (3 Stars)";
      this.miRate4.Name = "miRate4";
      this.miRate4.ShortcutKeys = Keys.D4 | Keys.Shift | Keys.Alt;
      this.miRate4.Size = new System.Drawing.Size(285, 22);
      this.miRate4.Text = "**** (4 Stars)";
      this.miRate5.Name = "miRate5";
      this.miRate5.ShortcutKeys = Keys.D5 | Keys.Shift | Keys.Alt;
      this.miRate5.Size = new System.Drawing.Size(285, 22);
      this.miRate5.Text = "***** (5 Stars)";
      this.toolStripMenuItem58.Name = "toolStripMenuItem58";
      this.toolStripMenuItem58.Size = new System.Drawing.Size(282, 6);
      this.miQuickRating.Name = "miQuickRating";
      this.miQuickRating.ShortcutKeys = Keys.Q | Keys.Shift | Keys.Alt;
      this.miQuickRating.Size = new System.Drawing.Size(285, 22);
      this.miQuickRating.Text = "Quick Rating and Review...";
      this.miPageType.Name = "miPageType";
      this.miPageType.Size = new System.Drawing.Size(219, 22);
      this.miPageType.Text = "&Page Type";
      this.miPageRotate.Name = "miPageRotate";
      this.miPageRotate.Size = new System.Drawing.Size(219, 22);
      this.miPageRotate.Text = "Page Rotation";
      this.miBookmarks.DropDownItems.AddRange(new ToolStripItem[8]
      {
        (ToolStripItem) this.miSetBookmark,
        (ToolStripItem) this.miRemoveBookmark,
        (ToolStripItem) this.toolStripMenuItem26,
        (ToolStripItem) this.miPrevBookmark,
        (ToolStripItem) this.miNextBookmark,
        (ToolStripItem) this.toolStripMenuItem8,
        (ToolStripItem) this.miLastPageRead,
        (ToolStripItem) this.toolStripMenuItem37
      });
      this.miBookmarks.Image = (Image) Resources.Bookmark;
      this.miBookmarks.Name = "miBookmarks";
      this.miBookmarks.Size = new System.Drawing.Size(219, 22);
      this.miBookmarks.Text = "&Bookmarks";
      this.miBookmarks.DropDownOpening += new EventHandler(this.miBookmarks_DropDownOpening);
      this.miSetBookmark.Image = (Image) Resources.NewBookmark;
      this.miSetBookmark.Name = "miSetBookmark";
      this.miSetBookmark.ShortcutKeys = Keys.B | Keys.Shift | Keys.Control;
      this.miSetBookmark.Size = new System.Drawing.Size(249, 22);
      this.miSetBookmark.Text = "Set Bookmark...";
      this.miRemoveBookmark.Image = (Image) Resources.RemoveBookmark;
      this.miRemoveBookmark.Name = "miRemoveBookmark";
      this.miRemoveBookmark.ShortcutKeys = Keys.D | Keys.Shift | Keys.Control;
      this.miRemoveBookmark.Size = new System.Drawing.Size(249, 22);
      this.miRemoveBookmark.Text = "Remove Bookmark";
      this.toolStripMenuItem26.Name = "toolStripMenuItem26";
      this.toolStripMenuItem26.Size = new System.Drawing.Size(246, 6);
      this.miPrevBookmark.Image = (Image) Resources.PreviousBookmark;
      this.miPrevBookmark.Name = "miPrevBookmark";
      this.miPrevBookmark.ShortcutKeys = Keys.P | Keys.Shift | Keys.Control;
      this.miPrevBookmark.Size = new System.Drawing.Size(249, 22);
      this.miPrevBookmark.Text = "Previous Bookmark";
      this.miNextBookmark.Image = (Image) Resources.NextBookmark;
      this.miNextBookmark.Name = "miNextBookmark";
      this.miNextBookmark.ShortcutKeys = Keys.N | Keys.Shift | Keys.Control;
      this.miNextBookmark.Size = new System.Drawing.Size(249, 22);
      this.miNextBookmark.Text = "Next Bookmark";
      this.toolStripMenuItem8.Name = "toolStripMenuItem8";
      this.toolStripMenuItem8.Size = new System.Drawing.Size(246, 6);
      this.miLastPageRead.Name = "miLastPageRead";
      this.miLastPageRead.ShortcutKeys = Keys.L | Keys.Shift | Keys.Control;
      this.miLastPageRead.Size = new System.Drawing.Size(249, 22);
      this.miLastPageRead.Text = "L&ast Page Read";
      this.toolStripMenuItem37.Name = "toolStripMenuItem37";
      this.toolStripMenuItem37.Size = new System.Drawing.Size(246, 6);
      this.toolStripMenuItem37.Tag = (object) "bms";
      this.toolStripMenuItem40.Name = "toolStripMenuItem40";
      this.toolStripMenuItem40.Size = new System.Drawing.Size(216, 6);
      this.miCopyPage.Image = (Image) Resources.Copy;
      this.miCopyPage.Name = "miCopyPage";
      this.miCopyPage.ShortcutKeys = Keys.C | Keys.Control;
      this.miCopyPage.Size = new System.Drawing.Size(219, 22);
      this.miCopyPage.Text = "&Copy Page";
      this.miExportPage.Name = "miExportPage";
      this.miExportPage.ShortcutKeys = Keys.C | Keys.Shift | Keys.Control;
      this.miExportPage.Size = new System.Drawing.Size(219, 22);
      this.miExportPage.Text = "&Export Page...";
      this.toolStripMenuItem39.Name = "toolStripMenuItem39";
      this.toolStripMenuItem39.Size = new System.Drawing.Size(216, 6);
      this.miViewRefresh.Image = (Image) Resources.Refresh;
      this.miViewRefresh.Name = "miViewRefresh";
      this.miViewRefresh.ShortcutKeys = Keys.F5;
      this.miViewRefresh.Size = new System.Drawing.Size(219, 22);
      this.miViewRefresh.Text = "&Refresh";
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(216, 6);
      this.miDevices.Image = (Image) Resources.EditDevices;
      this.miDevices.Name = "miDevices";
      this.miDevices.Size = new System.Drawing.Size(219, 22);
      this.miDevices.Text = "Devices...";
      this.miPreferences.Image = (Image) Resources.Preferences;
      this.miPreferences.Name = "miPreferences";
      this.miPreferences.ShortcutKeys = Keys.F9 | Keys.Control;
      this.miPreferences.Size = new System.Drawing.Size(219, 22);
      this.miPreferences.Text = "&Preferences...";
      this.browseMenu.DropDownItems.AddRange(new ToolStripItem[16]
      {
        (ToolStripItem) this.miToggleBrowser,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.miViewLibrary,
        (ToolStripItem) this.miViewFolders,
        (ToolStripItem) this.miViewPages,
        (ToolStripItem) this.toolStripMenuItem9,
        (ToolStripItem) this.miSidebar,
        (ToolStripItem) this.miSmallPreview,
        (ToolStripItem) this.miSearchBrowser,
        (ToolStripItem) this.miInfoPanel,
        (ToolStripItem) this.toolStripMenuItem56,
        (ToolStripItem) this.miPreviousList,
        (ToolStripItem) this.miNextList,
        (ToolStripItem) this.toolStripMenuItem6,
        (ToolStripItem) this.miWorkspaces,
        (ToolStripItem) this.miListLayouts
      });
      this.browseMenu.Name = "browseMenu";
      this.browseMenu.Size = new System.Drawing.Size(57, 20);
      this.browseMenu.Text = "&Browse";
      this.miToggleBrowser.Image = (Image) Resources.Browser;
      this.miToggleBrowser.Name = "miToggleBrowser";
      this.miToggleBrowser.ShortcutKeys = Keys.F3;
      this.miToggleBrowser.Size = new System.Drawing.Size(205, 22);
      this.miToggleBrowser.Text = "&Browser";
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(202, 6);
      this.miViewLibrary.Image = (Image) Resources.Database;
      this.miViewLibrary.Name = "miViewLibrary";
      this.miViewLibrary.ShortcutKeys = Keys.F6;
      this.miViewLibrary.Size = new System.Drawing.Size(205, 22);
      this.miViewLibrary.Text = "Li&brary";
      this.miViewFolders.Image = (Image) Resources.FileBrowser;
      this.miViewFolders.Name = "miViewFolders";
      this.miViewFolders.ShortcutKeys = Keys.F7;
      this.miViewFolders.Size = new System.Drawing.Size(205, 22);
      this.miViewFolders.Text = "&Folders";
      this.miViewPages.Image = (Image) Resources.ComicPage;
      this.miViewPages.Name = "miViewPages";
      this.miViewPages.ShortcutKeys = Keys.F8;
      this.miViewPages.Size = new System.Drawing.Size(205, 22);
      this.miViewPages.Text = "&Pages";
      this.toolStripMenuItem9.Name = "toolStripMenuItem9";
      this.toolStripMenuItem9.Size = new System.Drawing.Size(202, 6);
      this.miSidebar.Image = (Image) Resources.Sidebar;
      this.miSidebar.Name = "miSidebar";
      this.miSidebar.ShortcutKeys = Keys.F6 | Keys.Shift;
      this.miSidebar.Size = new System.Drawing.Size(205, 22);
      this.miSidebar.Text = "&Sidebar";
      this.miSmallPreview.Image = (Image) Resources.SmallPreview;
      this.miSmallPreview.Name = "miSmallPreview";
      this.miSmallPreview.ShortcutKeys = Keys.F7 | Keys.Shift;
      this.miSmallPreview.Size = new System.Drawing.Size(205, 22);
      this.miSmallPreview.Text = "S&mall Preview";
      this.miSearchBrowser.Image = (Image) Resources.Search;
      this.miSearchBrowser.Name = "miSearchBrowser";
      this.miSearchBrowser.ShortcutKeys = Keys.F8 | Keys.Shift;
      this.miSearchBrowser.Size = new System.Drawing.Size(205, 22);
      this.miSearchBrowser.Text = "S&earch Browser";
      this.miInfoPanel.Image = (Image) Resources.InfoPanel;
      this.miInfoPanel.Name = "miInfoPanel";
      this.miInfoPanel.ShortcutKeys = Keys.F9 | Keys.Shift;
      this.miInfoPanel.Size = new System.Drawing.Size(205, 22);
      this.miInfoPanel.Text = "Info Panel";
      this.toolStripMenuItem56.Name = "toolStripMenuItem56";
      this.toolStripMenuItem56.Size = new System.Drawing.Size(202, 6);
      this.miPreviousList.Image = (Image) Resources.BrowsePrevious;
      this.miPreviousList.Name = "miPreviousList";
      this.miPreviousList.ShortcutKeys = Keys.J | Keys.Control;
      this.miPreviousList.Size = new System.Drawing.Size(205, 22);
      this.miPreviousList.Text = "Previous List";
      this.miNextList.Image = (Image) Resources.BrowseNext;
      this.miNextList.Name = "miNextList";
      this.miNextList.ShortcutKeys = Keys.K | Keys.Control;
      this.miNextList.Size = new System.Drawing.Size(205, 22);
      this.miNextList.Text = "Next List";
      this.toolStripMenuItem6.Name = "toolStripMenuItem6";
      this.toolStripMenuItem6.Size = new System.Drawing.Size(202, 6);
      this.miWorkspaces.DropDownItems.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.miSaveWorkspace,
        (ToolStripItem) this.miEditWorkspaces,
        (ToolStripItem) this.miWorkspaceSep
      });
      this.miWorkspaces.Image = (Image) Resources.Workspace;
      this.miWorkspaces.Name = "miWorkspaces";
      this.miWorkspaces.Size = new System.Drawing.Size(205, 22);
      this.miWorkspaces.Text = "&Workspaces";
      this.miSaveWorkspace.Name = "miSaveWorkspace";
      this.miSaveWorkspace.ShortcutKeys = Keys.W | Keys.Control;
      this.miSaveWorkspace.Size = new System.Drawing.Size(237, 22);
      this.miSaveWorkspace.Text = "&Save Workspace...";
      this.miEditWorkspaces.Name = "miEditWorkspaces";
      this.miEditWorkspaces.ShortcutKeys = Keys.W | Keys.Control | Keys.Alt;
      this.miEditWorkspaces.Size = new System.Drawing.Size(237, 22);
      this.miEditWorkspaces.Text = "&Edit Workspaces...";
      this.miWorkspaceSep.Name = "miWorkspaceSep";
      this.miWorkspaceSep.Size = new System.Drawing.Size(234, 6);
      this.miListLayouts.DropDownItems.AddRange(new ToolStripItem[6]
      {
        (ToolStripItem) this.miEditListLayout,
        (ToolStripItem) this.miSaveListLayout,
        (ToolStripItem) this.toolStripMenuItem10,
        (ToolStripItem) this.miEditLayouts,
        (ToolStripItem) this.miSetAllListsSame,
        (ToolStripItem) this.miLayoutSep
      });
      this.miListLayouts.Image = (Image) Resources.ListLayout;
      this.miListLayouts.Name = "miListLayouts";
      this.miListLayouts.Size = new System.Drawing.Size(205, 22);
      this.miListLayouts.Text = "List Layout";
      this.miEditListLayout.Name = "miEditListLayout";
      this.miEditListLayout.ShortcutKeys = Keys.L | Keys.Control;
      this.miEditListLayout.Size = new System.Drawing.Size(225, 22);
      this.miEditListLayout.Text = "&Edit List Layout...";
      this.miSaveListLayout.Name = "miSaveListLayout";
      this.miSaveListLayout.Size = new System.Drawing.Size(225, 22);
      this.miSaveListLayout.Text = "&Save List Layout...";
      this.toolStripMenuItem10.Name = "toolStripMenuItem10";
      this.toolStripMenuItem10.Size = new System.Drawing.Size(222, 6);
      this.miEditLayouts.Name = "miEditLayouts";
      this.miEditLayouts.ShortcutKeys = Keys.L | Keys.Control | Keys.Alt;
      this.miEditLayouts.Size = new System.Drawing.Size(225, 22);
      this.miEditLayouts.Text = "&Edit Layouts...";
      this.miSetAllListsSame.Name = "miSetAllListsSame";
      this.miSetAllListsSame.Size = new System.Drawing.Size(225, 22);
      this.miSetAllListsSame.Text = "Set all Lists to current Layout";
      this.miLayoutSep.Name = "miLayoutSep";
      this.miLayoutSep.Size = new System.Drawing.Size(222, 6);
      this.readMenu.DropDownItems.AddRange(new ToolStripItem[17]
      {
        (ToolStripItem) this.miFirstPage,
        (ToolStripItem) this.miPrevPage,
        (ToolStripItem) this.miNextPage,
        (ToolStripItem) this.miLastPage,
        (ToolStripItem) this.toolStripMenuItem18,
        (ToolStripItem) this.miPrevFromList,
        (ToolStripItem) this.miNextFromList,
        (ToolStripItem) this.miRandomFromList,
        (ToolStripItem) this.miSyncBrowser,
        (ToolStripItem) this.toolStripMenuItem17,
        (ToolStripItem) this.miPrevTab,
        (ToolStripItem) this.miNextTab,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.miAutoScroll,
        (ToolStripItem) this.miDoublePageAutoScroll,
        (ToolStripItem) this.toolStripMenuItem21,
        (ToolStripItem) this.miTrackCurrentPage
      });
      this.readMenu.Name = "readMenu";
      this.readMenu.Size = new System.Drawing.Size(45, 20);
      this.readMenu.Text = "&Read";
      this.miFirstPage.Image = (Image) Resources.GoFirst;
      this.miFirstPage.Name = "miFirstPage";
      this.miFirstPage.ShortcutKeyDisplayString = "";
      this.miFirstPage.ShortcutKeys = Keys.B | Keys.Control;
      this.miFirstPage.Size = new System.Drawing.Size(287, 22);
      this.miFirstPage.Text = "&First Page";
      this.miPrevPage.Image = (Image) Resources.GoPrevious;
      this.miPrevPage.Name = "miPrevPage";
      this.miPrevPage.ShortcutKeyDisplayString = "";
      this.miPrevPage.ShortcutKeys = Keys.P | Keys.Control;
      this.miPrevPage.Size = new System.Drawing.Size(287, 22);
      this.miPrevPage.Text = "&Previous Page";
      this.miNextPage.Image = (Image) Resources.GoNext;
      this.miNextPage.Name = "miNextPage";
      this.miNextPage.ShortcutKeyDisplayString = "";
      this.miNextPage.ShortcutKeys = Keys.N | Keys.Control;
      this.miNextPage.Size = new System.Drawing.Size(287, 22);
      this.miNextPage.Text = "&Next Page";
      this.miLastPage.Image = (Image) Resources.GoLast;
      this.miLastPage.Name = "miLastPage";
      this.miLastPage.ShortcutKeyDisplayString = "";
      this.miLastPage.ShortcutKeys = Keys.E | Keys.Control;
      this.miLastPage.Size = new System.Drawing.Size(287, 22);
      this.miLastPage.Text = "&Last Page";
      this.toolStripMenuItem18.Name = "toolStripMenuItem18";
      this.toolStripMenuItem18.Size = new System.Drawing.Size(284, 6);
      this.miPrevFromList.Image = (Image) Resources.PrevFromList;
      this.miPrevFromList.Name = "miPrevFromList";
      this.miPrevFromList.ShortcutKeyDisplayString = "";
      this.miPrevFromList.ShortcutKeys = Keys.P | Keys.Control | Keys.Alt;
      this.miPrevFromList.Size = new System.Drawing.Size(287, 22);
      this.miPrevFromList.Text = "Pre&vious Book";
      this.miNextFromList.Image = (Image) Resources.NextFromList;
      this.miNextFromList.Name = "miNextFromList";
      this.miNextFromList.ShortcutKeyDisplayString = "";
      this.miNextFromList.ShortcutKeys = Keys.N | Keys.Control | Keys.Alt;
      this.miNextFromList.Size = new System.Drawing.Size(287, 22);
      this.miNextFromList.Text = "Ne&xt Book";
      this.miRandomFromList.Image = (Image) Resources.RandomComic;
      this.miRandomFromList.Name = "miRandomFromList";
      this.miRandomFromList.ShortcutKeys = Keys.O | Keys.Control | Keys.Alt;
      this.miRandomFromList.Size = new System.Drawing.Size(287, 22);
      this.miRandomFromList.Text = "Random Book";
      this.miSyncBrowser.Image = (Image) Resources.SyncBrowser;
      this.miSyncBrowser.Name = "miSyncBrowser";
      this.miSyncBrowser.ShortcutKeys = Keys.F3 | Keys.Control;
      this.miSyncBrowser.Size = new System.Drawing.Size(287, 22);
      this.miSyncBrowser.Text = "Show in &Browser";
      this.toolStripMenuItem17.Name = "toolStripMenuItem17";
      this.toolStripMenuItem17.Size = new System.Drawing.Size(284, 6);
      this.miPrevTab.Image = (Image) Resources.Previous;
      this.miPrevTab.Name = "miPrevTab";
      this.miPrevTab.ShortcutKeys = Keys.J | Keys.Shift | Keys.Control;
      this.miPrevTab.Size = new System.Drawing.Size(287, 22);
      this.miPrevTab.Text = "&Previous Tab";
      this.miNextTab.Image = (Image) Resources.Next;
      this.miNextTab.Name = "miNextTab";
      this.miNextTab.ShortcutKeys = Keys.K | Keys.Shift | Keys.Control;
      this.miNextTab.Size = new System.Drawing.Size(287, 22);
      this.miNextTab.Text = "Next &Tab";
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(284, 6);
      this.miAutoScroll.Image = (Image) Resources.CursorScroll;
      this.miAutoScroll.Name = "miAutoScroll";
      this.miAutoScroll.ShortcutKeys = Keys.S | Keys.Control;
      this.miAutoScroll.Size = new System.Drawing.Size(287, 22);
      this.miAutoScroll.Text = "&Auto Scrolling";
      this.miDoublePageAutoScroll.Image = (Image) Resources.TwoPageAutoscroll;
      this.miDoublePageAutoScroll.Name = "miDoublePageAutoScroll";
      this.miDoublePageAutoScroll.ShortcutKeys = Keys.S | Keys.Shift | Keys.Alt;
      this.miDoublePageAutoScroll.Size = new System.Drawing.Size(287, 22);
      this.miDoublePageAutoScroll.Text = "Double Page Auto Scrolling";
      this.toolStripMenuItem21.Name = "toolStripMenuItem21";
      this.toolStripMenuItem21.Size = new System.Drawing.Size(284, 6);
      this.miTrackCurrentPage.Name = "miTrackCurrentPage";
      this.miTrackCurrentPage.ShortcutKeys = Keys.T | Keys.Shift | Keys.Alt;
      this.miTrackCurrentPage.Size = new System.Drawing.Size(287, 22);
      this.miTrackCurrentPage.Text = "Track current Page";
      this.displayMenu.DropDownItems.AddRange(new ToolStripItem[11]
      {
        (ToolStripItem) this.miComicDisplaySettings,
        (ToolStripItem) this.toolStripSeparator3,
        (ToolStripItem) this.miPageLayout,
        (ToolStripItem) this.miZoom,
        (ToolStripItem) this.miRotation,
        (ToolStripItem) this.toolStripMenuItem23,
        (ToolStripItem) this.miMinimalGui,
        (ToolStripItem) this.miFullScreen,
        (ToolStripItem) this.miReaderUndocked,
        (ToolStripItem) this.toolStripMenuItem41,
        (ToolStripItem) this.miMagnify
      });
      this.displayMenu.ImageTransparentColor = System.Drawing.Color.Fuchsia;
      this.displayMenu.Name = "displayMenu";
      this.displayMenu.Size = new System.Drawing.Size(57, 20);
      this.displayMenu.Text = "&Display";
      this.miComicDisplaySettings.Image = (Image) Resources.DisplaySettings;
      this.miComicDisplaySettings.Name = "miComicDisplaySettings";
      this.miComicDisplaySettings.ShortcutKeys = Keys.F9;
      this.miComicDisplaySettings.Size = new System.Drawing.Size(237, 38);
      this.miComicDisplaySettings.Text = "Book Display Settings...";
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(234, 6);
      this.miPageLayout.DropDownItems.AddRange(new ToolStripItem[13]
      {
        (ToolStripItem) this.miOriginal,
        (ToolStripItem) this.miFitAll,
        (ToolStripItem) this.miFitWidth,
        (ToolStripItem) this.miFitWidthAdaptive,
        (ToolStripItem) this.miFitHeight,
        (ToolStripItem) this.miBestFit,
        (ToolStripItem) this.toolStripMenuItem27,
        (ToolStripItem) this.miSinglePage,
        (ToolStripItem) this.miTwoPages,
        (ToolStripItem) this.miTwoPagesAdaptive,
        (ToolStripItem) this.miRightToLeft,
        (ToolStripItem) this.toolStripMenuItem44,
        (ToolStripItem) this.miOnlyFitOversized
      });
      this.miPageLayout.Name = "miPageLayout";
      this.miPageLayout.Size = new System.Drawing.Size(237, 38);
      this.miPageLayout.Text = "&Page Layout";
      this.miOriginal.Image = (Image) Resources.Original;
      this.miOriginal.Name = "miOriginal";
      this.miOriginal.ShortcutKeys = Keys.D1 | Keys.Control;
      this.miOriginal.Size = new System.Drawing.Size(247, 22);
      this.miOriginal.Text = "Original Size";
      this.miFitAll.Image = (Image) Resources.FitAll;
      this.miFitAll.Name = "miFitAll";
      this.miFitAll.ShortcutKeys = Keys.D2 | Keys.Control;
      this.miFitAll.Size = new System.Drawing.Size(247, 22);
      this.miFitAll.Text = "Fit &All";
      this.miFitWidth.Image = (Image) Resources.FitWidth;
      this.miFitWidth.Name = "miFitWidth";
      this.miFitWidth.ShortcutKeys = Keys.D3 | Keys.Control;
      this.miFitWidth.Size = new System.Drawing.Size(247, 22);
      this.miFitWidth.Text = "Fit &Width";
      this.miFitWidthAdaptive.Image = (Image) Resources.FitWidthAdaptive;
      this.miFitWidthAdaptive.Name = "miFitWidthAdaptive";
      this.miFitWidthAdaptive.ShortcutKeys = Keys.D4 | Keys.Control;
      this.miFitWidthAdaptive.Size = new System.Drawing.Size(247, 22);
      this.miFitWidthAdaptive.Text = "Fit Width (adaptive)";
      this.miFitHeight.Image = (Image) Resources.FitHeight;
      this.miFitHeight.Name = "miFitHeight";
      this.miFitHeight.ShortcutKeys = Keys.D5 | Keys.Control;
      this.miFitHeight.Size = new System.Drawing.Size(247, 22);
      this.miFitHeight.Text = "Fit &Height";
      this.miBestFit.Image = (Image) Resources.FitBest;
      this.miBestFit.Name = "miBestFit";
      this.miBestFit.ShortcutKeys = Keys.D6 | Keys.Control;
      this.miBestFit.Size = new System.Drawing.Size(247, 22);
      this.miBestFit.Text = "Fit &Best";
      this.toolStripMenuItem27.Name = "toolStripMenuItem27";
      this.toolStripMenuItem27.Size = new System.Drawing.Size(244, 6);
      this.miSinglePage.Image = (Image) Resources.SinglePage;
      this.miSinglePage.Name = "miSinglePage";
      this.miSinglePage.ShortcutKeys = Keys.D7 | Keys.Control;
      this.miSinglePage.Size = new System.Drawing.Size(247, 22);
      this.miSinglePage.Text = "Single Page";
      this.miTwoPages.Image = (Image) Resources.TwoPageForced;
      this.miTwoPages.Name = "miTwoPages";
      this.miTwoPages.ShortcutKeys = Keys.D8 | Keys.Control;
      this.miTwoPages.Size = new System.Drawing.Size(247, 22);
      this.miTwoPages.Text = "Two Pages";
      this.miTwoPagesAdaptive.Image = (Image) Resources.TwoPage;
      this.miTwoPagesAdaptive.Name = "miTwoPagesAdaptive";
      this.miTwoPagesAdaptive.ShortcutKeys = Keys.D9 | Keys.Control;
      this.miTwoPagesAdaptive.Size = new System.Drawing.Size(247, 22);
      this.miTwoPagesAdaptive.Text = "Two Pages (adaptive)";
      this.miTwoPagesAdaptive.ToolTipText = "Show one or two pages";
      this.miRightToLeft.Image = (Image) Resources.RightToLeft;
      this.miRightToLeft.Name = "miRightToLeft";
      this.miRightToLeft.ShortcutKeys = Keys.D0 | Keys.Control;
      this.miRightToLeft.Size = new System.Drawing.Size(247, 22);
      this.miRightToLeft.Text = "Right to Left";
      this.toolStripMenuItem44.Name = "toolStripMenuItem44";
      this.toolStripMenuItem44.Size = new System.Drawing.Size(244, 6);
      this.miOnlyFitOversized.Image = (Image) Resources.Oversized;
      this.miOnlyFitOversized.Name = "miOnlyFitOversized";
      this.miOnlyFitOversized.ShortcutKeys = Keys.D0 | Keys.Shift | Keys.Control;
      this.miOnlyFitOversized.Size = new System.Drawing.Size(247, 22);
      this.miOnlyFitOversized.Text = "&Only fit if oversized";
      this.miZoom.DropDownItems.AddRange(new ToolStripItem[11]
      {
        (ToolStripItem) this.miZoomIn,
        (ToolStripItem) this.miZoomOut,
        (ToolStripItem) this.miToggleZoom,
        (ToolStripItem) this.toolStripSeparator14,
        (ToolStripItem) this.miZoom100,
        (ToolStripItem) this.miZoom125,
        (ToolStripItem) this.miZoom150,
        (ToolStripItem) this.miZoom200,
        (ToolStripItem) this.miZoom400,
        (ToolStripItem) this.toolStripSeparator15,
        (ToolStripItem) this.miZoomCustom
      });
      this.miZoom.Name = "miZoom";
      this.miZoom.Size = new System.Drawing.Size(237, 38);
      this.miZoom.Text = "Zoom";
      this.miZoomIn.Image = (Image) Resources.ZoomIn;
      this.miZoomIn.Name = "miZoomIn";
      this.miZoomIn.ShortcutKeys = Keys.Oemplus | Keys.Control;
      this.miZoomIn.Size = new System.Drawing.Size(222, 22);
      this.miZoomIn.Text = "Zoom &In";
      this.miZoomOut.Image = (Image) Resources.ZoomOut;
      this.miZoomOut.Name = "miZoomOut";
      this.miZoomOut.ShortcutKeys = Keys.OemMinus | Keys.Control;
      this.miZoomOut.Size = new System.Drawing.Size(222, 22);
      this.miZoomOut.Text = "Zoom &Out";
      this.miToggleZoom.Name = "miToggleZoom";
      this.miToggleZoom.ShortcutKeys = Keys.Z | Keys.Control | Keys.Alt;
      this.miToggleZoom.Size = new System.Drawing.Size(222, 22);
      this.miToggleZoom.Text = "Toggle Zoom";
      this.toolStripSeparator14.Name = "toolStripSeparator14";
      this.toolStripSeparator14.Size = new System.Drawing.Size(219, 6);
      this.miZoom100.Name = "miZoom100";
      this.miZoom100.Size = new System.Drawing.Size(222, 22);
      this.miZoom100.Text = "100%";
      this.miZoom125.Name = "miZoom125";
      this.miZoom125.Size = new System.Drawing.Size(222, 22);
      this.miZoom125.Text = "125%";
      this.miZoom150.Name = "miZoom150";
      this.miZoom150.Size = new System.Drawing.Size(222, 22);
      this.miZoom150.Text = "150%";
      this.miZoom200.Name = "miZoom200";
      this.miZoom200.Size = new System.Drawing.Size(222, 22);
      this.miZoom200.Text = "200%";
      this.miZoom400.Name = "miZoom400";
      this.miZoom400.Size = new System.Drawing.Size(222, 22);
      this.miZoom400.Text = "400%";
      this.toolStripSeparator15.Name = "toolStripSeparator15";
      this.toolStripSeparator15.Size = new System.Drawing.Size(219, 6);
      this.miZoomCustom.Name = "miZoomCustom";
      this.miZoomCustom.ShortcutKeys = Keys.Z | Keys.Shift | Keys.Control;
      this.miZoomCustom.Size = new System.Drawing.Size(222, 22);
      this.miZoomCustom.Text = "&Custom...";
      this.miRotation.DropDownItems.AddRange(new ToolStripItem[9]
      {
        (ToolStripItem) this.miRotateLeft,
        (ToolStripItem) this.miRotateRight,
        (ToolStripItem) this.toolStripMenuItem33,
        (ToolStripItem) this.miRotate0,
        (ToolStripItem) this.miRotate90,
        (ToolStripItem) this.miRotate180,
        (ToolStripItem) this.miRotate270,
        (ToolStripItem) this.toolStripMenuItem36,
        (ToolStripItem) this.miAutoRotate
      });
      this.miRotation.Name = "miRotation";
      this.miRotation.Size = new System.Drawing.Size(237, 38);
      this.miRotation.Text = "&Rotation";
      this.miRotateLeft.Image = (Image) Resources.RotateLeft;
      this.miRotateLeft.Name = "miRotateLeft";
      this.miRotateLeft.ShortcutKeys = Keys.OemMinus | Keys.Shift | Keys.Control;
      this.miRotateLeft.Size = new System.Drawing.Size(256, 22);
      this.miRotateLeft.Text = "Rotate Left";
      this.miRotateRight.Image = (Image) Resources.RotateRight;
      this.miRotateRight.Name = "miRotateRight";
      this.miRotateRight.ShortcutKeys = Keys.Oemplus | Keys.Shift | Keys.Control;
      this.miRotateRight.Size = new System.Drawing.Size(256, 22);
      this.miRotateRight.Text = "Rotate Right";
      this.toolStripMenuItem33.Name = "toolStripMenuItem33";
      this.toolStripMenuItem33.Size = new System.Drawing.Size(253, 6);
      this.miRotate0.Image = (Image) Resources.Rotate0;
      this.miRotate0.Name = "miRotate0";
      this.miRotate0.ShortcutKeys = Keys.D7 | Keys.Shift | Keys.Control;
      this.miRotate0.Size = new System.Drawing.Size(256, 22);
      this.miRotate0.Text = "&No Rotation";
      this.miRotate90.Image = (Image) Resources.Rotate90;
      this.miRotate90.Name = "miRotate90";
      this.miRotate90.ShortcutKeys = Keys.D8 | Keys.Shift | Keys.Control;
      this.miRotate90.Size = new System.Drawing.Size(256, 22);
      this.miRotate90.Text = "90°";
      this.miRotate180.Image = (Image) Resources.Rotate180;
      this.miRotate180.Name = "miRotate180";
      this.miRotate180.ShortcutKeys = Keys.D9 | Keys.Shift | Keys.Control;
      this.miRotate180.Size = new System.Drawing.Size(256, 22);
      this.miRotate180.Text = "180°";
      this.miRotate270.Image = (Image) Resources.Rotate270;
      this.miRotate270.Name = "miRotate270";
      this.miRotate270.ShortcutKeys = Keys.D0 | Keys.Shift | Keys.Control;
      this.miRotate270.Size = new System.Drawing.Size(256, 22);
      this.miRotate270.Text = "270°";
      this.toolStripMenuItem36.Name = "toolStripMenuItem36";
      this.toolStripMenuItem36.Size = new System.Drawing.Size(253, 6);
      this.miAutoRotate.Image = (Image) Resources.AutoRotate;
      this.miAutoRotate.Name = "miAutoRotate";
      this.miAutoRotate.Size = new System.Drawing.Size(256, 22);
      this.miAutoRotate.Text = "Autorotate Double Pages";
      this.toolStripMenuItem23.Name = "toolStripMenuItem23";
      this.toolStripMenuItem23.Size = new System.Drawing.Size(234, 6);
      this.miMinimalGui.Image = (Image) Resources.MenuToggle;
      this.miMinimalGui.Name = "miMinimalGui";
      this.miMinimalGui.ShortcutKeys = Keys.F10;
      this.miMinimalGui.Size = new System.Drawing.Size(237, 38);
      this.miMinimalGui.Text = "Minimal User Interface";
      this.miFullScreen.Image = (Image) Resources.FullScreen;
      this.miFullScreen.Name = "miFullScreen";
      this.miFullScreen.ShortcutKeyDisplayString = "";
      this.miFullScreen.ShortcutKeys = Keys.F11;
      this.miFullScreen.Size = new System.Drawing.Size(237, 38);
      this.miFullScreen.Text = "&Full Screen";
      this.miReaderUndocked.Image = (Image) Resources.UndockReader;
      this.miReaderUndocked.Name = "miReaderUndocked";
      this.miReaderUndocked.ShortcutKeys = Keys.F12;
      this.miReaderUndocked.Size = new System.Drawing.Size(237, 38);
      this.miReaderUndocked.Text = "Reader in &own Window";
      this.toolStripMenuItem41.Name = "toolStripMenuItem41";
      this.toolStripMenuItem41.Size = new System.Drawing.Size(234, 6);
      this.miMagnify.Image = (Image) Resources.Zoom;
      this.miMagnify.Name = "miMagnify";
      this.miMagnify.ShortcutKeys = Keys.M | Keys.Control;
      this.miMagnify.Size = new System.Drawing.Size(237, 38);
      this.miMagnify.Text = "&Magnifier";
      this.helpMenu.DropDownItems.AddRange(new ToolStripItem[13]
      {
        (ToolStripItem) this.miHelp,
        (ToolStripItem) this.miWebHelp,
        (ToolStripItem) this.miHelpPlugins,
        (ToolStripItem) this.miChooseHelpSystem,
        (ToolStripItem) this.miHelpQuickIntro,
        (ToolStripItem) this.toolStripMenuItem3,
        (ToolStripItem) this.miWebHome,
        (ToolStripItem) this.miWebUserForum,
        (ToolStripItem) this.toolStripMenuItem5,
        (ToolStripItem) this.miNews,
        (ToolStripItem) this.miSupport,
        (ToolStripItem) this.toolStripMenuItem25,
        (ToolStripItem) this.miAbout
      });
      this.helpMenu.Name = "helpMenu";
      this.helpMenu.Size = new System.Drawing.Size(44, 20);
      this.helpMenu.Text = "&Help";
      this.miHelp.Name = "miHelp";
      this.miHelp.Size = new System.Drawing.Size(256, 22);
      this.miHelp.Text = "Help";
      this.miHelp.Visible = false;
      this.miWebHelp.Image = (Image) Resources.Help;
      this.miWebHelp.Name = "miWebHelp";
      this.miWebHelp.ShortcutKeys = Keys.F1;
      this.miWebHelp.Size = new System.Drawing.Size(256, 22);
      this.miWebHelp.Text = "ComicRack Documentation...";
      this.miHelpPlugins.Name = "miHelpPlugins";
      this.miHelpPlugins.Size = new System.Drawing.Size(256, 22);
      this.miHelpPlugins.Text = "Plugins";
      this.miChooseHelpSystem.Name = "miChooseHelpSystem";
      this.miChooseHelpSystem.Size = new System.Drawing.Size(256, 22);
      this.miChooseHelpSystem.Text = "Choose Help System";
      this.miHelpQuickIntro.Name = "miHelpQuickIntro";
      this.miHelpQuickIntro.Size = new System.Drawing.Size(256, 22);
      this.miHelpQuickIntro.Text = "Quick Introduction";
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(253, 6);
      this.miWebHome.Image = (Image) Resources.WebBlog;
      this.miWebHome.Name = "miWebHome";
      this.miWebHome.ShortcutKeys = Keys.F1 | Keys.Shift;
      this.miWebHome.Size = new System.Drawing.Size(256, 22);
      this.miWebHome.Text = "ComicRack Homepage...";
      this.miWebUserForum.Image = (Image) Resources.WebForum;
      this.miWebUserForum.Name = "miWebUserForum";
      this.miWebUserForum.ShortcutKeys = Keys.F1 | Keys.Control;
      this.miWebUserForum.Size = new System.Drawing.Size(256, 22);
      this.miWebUserForum.Text = "ComicRack User Forum...";
      this.toolStripMenuItem5.Name = "toolStripMenuItem5";
      this.toolStripMenuItem5.Size = new System.Drawing.Size(253, 6);
      this.miNews.Image = (Image) Resources.News;
      this.miNews.Name = "miNews";
      this.miNews.Size = new System.Drawing.Size(256, 22);
      this.miNews.Text = "&News...";
      this.miSupport.Image = (Image) Resources.Heart;
      this.miSupport.Name = "miSupport";
      this.miSupport.Size = new System.Drawing.Size(256, 22);
      this.miSupport.Text = "&Support ComicRack...";
      this.toolStripMenuItem25.Name = "toolStripMenuItem25";
      this.toolStripMenuItem25.Size = new System.Drawing.Size(253, 6);
      this.miAbout.Image = (Image) Resources.About;
      this.miAbout.Name = "miAbout";
      this.miAbout.ShortcutKeys = Keys.F1 | Keys.Alt;
      this.miAbout.Size = new System.Drawing.Size(256, 22);
      this.miAbout.Text = "&About...";
      this.statusStrip.AutoSize = false;
      this.statusStrip.Items.AddRange(new ToolStripItem[12]
      {
        (ToolStripItem) this.tsText,
        (ToolStripItem) this.tsDeviceSyncActivity,
        (ToolStripItem) this.tsExportActivity,
        (ToolStripItem) this.tsReadInfoActivity,
        (ToolStripItem) this.tsWriteInfoActivity,
        (ToolStripItem) this.tsPageActivity,
        (ToolStripItem) this.tsScanActivity,
        (ToolStripItem) this.tsDataSourceState,
        (ToolStripItem) this.tsBook,
        (ToolStripItem) this.tsCurrentPage,
        (ToolStripItem) this.tsPageCount,
        (ToolStripItem) this.tsServerActivity
      });
      this.statusStrip.Location = new System.Drawing.Point(0, 638);
      this.statusStrip.MinimumSize = new System.Drawing.Size(0, 24);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.ShowItemToolTips = true;
      this.statusStrip.Size = new System.Drawing.Size(744, 24);
      this.statusStrip.TabIndex = 3;
      this.tsText.Name = "tsText";
      this.tsText.Size = new System.Drawing.Size(603, 19);
      this.tsText.Spring = true;
      this.tsText.Text = "Ready";
      this.tsText.TextAlign = ContentAlignment.MiddleLeft;
      this.tsDeviceSyncActivity.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsDeviceSyncActivity.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsDeviceSyncActivity.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsDeviceSyncActivity.Image = (Image) Resources.DeviceSyncAnimation;
      this.tsDeviceSyncActivity.Name = "tsDeviceSyncActivity";
      this.tsDeviceSyncActivity.Size = new System.Drawing.Size(36, 19);
      this.tsDeviceSyncActivity.Text = "Exporting";
      this.tsDeviceSyncActivity.Visible = false;
      this.tsDeviceSyncActivity.Click += new EventHandler(this.tsDeviceSyncActivity_Click);
      this.tsExportActivity.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsExportActivity.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsExportActivity.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsExportActivity.Image = (Image) Resources.ExportAnimation;
      this.tsExportActivity.Name = "tsExportActivity";
      this.tsExportActivity.Size = new System.Drawing.Size(36, 19);
      this.tsExportActivity.Text = "Exporting";
      this.tsExportActivity.Visible = false;
      this.tsExportActivity.Click += new EventHandler(this.tsExportActivity_Click);
      this.tsReadInfoActivity.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsReadInfoActivity.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsReadInfoActivity.Image = (Image) Resources.ReadInfoAnimation;
      this.tsReadInfoActivity.Margin = new Padding(2, 3, 2, 2);
      this.tsReadInfoActivity.Name = "tsReadInfoActivity";
      this.tsReadInfoActivity.Size = new System.Drawing.Size(36, 19);
      this.tsReadInfoActivity.ToolTipText = "Reading info data from files...";
      this.tsReadInfoActivity.Visible = false;
      this.tsReadInfoActivity.Click += new EventHandler(this.tsReadInfoActivity_Click);
      this.tsWriteInfoActivity.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsWriteInfoActivity.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsWriteInfoActivity.Image = (Image) Resources.UpdateInfoAnimation;
      this.tsWriteInfoActivity.Margin = new Padding(2, 3, 2, 2);
      this.tsWriteInfoActivity.Name = "tsWriteInfoActivity";
      this.tsWriteInfoActivity.Size = new System.Drawing.Size(36, 19);
      this.tsWriteInfoActivity.ToolTipText = "Writing info data to files...";
      this.tsWriteInfoActivity.Visible = false;
      this.tsWriteInfoActivity.Click += new EventHandler(this.tsUpdateInfoActivity_Click);
      this.tsPageActivity.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsPageActivity.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsPageActivity.Image = (Image) Resources.ReadPagesAnimation;
      this.tsPageActivity.Name = "tsPageActivity";
      this.tsPageActivity.Size = new System.Drawing.Size(36, 19);
      this.tsPageActivity.ToolTipText = "Getting Pages and Thumbnails...";
      this.tsPageActivity.Visible = false;
      this.tsPageActivity.Click += new EventHandler(this.tsPageActivity_Click);
      this.tsScanActivity.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsScanActivity.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsScanActivity.Image = (Image) Resources.ScanAnimation;
      this.tsScanActivity.Margin = new Padding(2, 3, 2, 2);
      this.tsScanActivity.Name = "tsScanActivity";
      this.tsScanActivity.Size = new System.Drawing.Size(36, 19);
      this.tsScanActivity.ToolTipText = "A scan is running...";
      this.tsScanActivity.Visible = false;
      this.tsScanActivity.Click += new EventHandler(this.tsScanActivity_Click);
      this.tsDataSourceState.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsDataSourceState.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsDataSourceState.Margin = new Padding(2, 3, 2, 2);
      this.tsDataSourceState.Name = "tsDataSourceState";
      this.tsDataSourceState.Size = new System.Drawing.Size(4, 19);
      this.tsDataSourceState.Visible = false;
      this.tsBook.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsBook.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsBook.Name = "tsBook";
      this.tsBook.Size = new System.Drawing.Size(38, 19);
      this.tsBook.Text = "Book";
      this.tsBook.ToolTipText = "Name of the opened Book";
      this.tsCurrentPage.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsCurrentPage.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsCurrentPage.Name = "tsCurrentPage";
      this.tsCurrentPage.Size = new System.Drawing.Size(37, 19);
      this.tsCurrentPage.Text = "Page";
      this.tsCurrentPage.ToolTipText = "Current Page of the open Book";
      this.tsCurrentPage.Click += new EventHandler(this.tsCurrentPage_Click);
      this.tsPageCount.BorderSides = ToolStripStatusLabelBorderSides.All;
      this.tsPageCount.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsPageCount.Name = "tsPageCount";
      this.tsPageCount.Size = new System.Drawing.Size(51, 19);
      this.tsPageCount.Text = "0 Pages";
      this.tsPageCount.ToolTipText = "Page count of the open Book";
      this.tsServerActivity.BorderStyle = Border3DStyle.SunkenOuter;
      this.tsServerActivity.Image = (Image) Resources.GrayLight;
      this.tsServerActivity.Name = "tsServerActivity";
      this.tsServerActivity.Size = new System.Drawing.Size(32, 19);
      this.tsServerActivity.Visible = false;
      this.tsServerActivity.Click += new EventHandler(this.tsServerActivity_Click);
      this.pageContextMenu.Items.AddRange(new ToolStripItem[16]
      {
        (ToolStripItem) this.cmShowInfo,
        (ToolStripItem) this.cmRating,
        (ToolStripItem) this.cmPageType,
        (ToolStripItem) this.cmPageRotate,
        (ToolStripItem) this.cmBookmarks,
        (ToolStripItem) this.toolStripSeparator10,
        (ToolStripItem) this.cmComics,
        (ToolStripItem) this.cmPageLayout,
        (ToolStripItem) this.cmMagnify,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.cmCopyPage,
        (ToolStripItem) this.cmExportPage,
        (ToolStripItem) this.toolStripMenuItem11,
        (ToolStripItem) this.cmRefreshPage,
        (ToolStripItem) this.toolStripMenuItem46,
        (ToolStripItem) this.cmMinimalGui
      });
      this.pageContextMenu.Name = "pageContextMenu";
      this.pageContextMenu.Size = new System.Drawing.Size(220, 292);
      this.pageContextMenu.Closed += new ToolStripDropDownClosedEventHandler(this.pageContextMenu_Closed);
      this.pageContextMenu.Opening += new CancelEventHandler(this.pageContextMenu_Opening);
      this.cmShowInfo.Image = (Image) Resources.GetInfo;
      this.cmShowInfo.Name = "cmShowInfo";
      this.cmShowInfo.ShortcutKeys = Keys.I | Keys.Control;
      this.cmShowInfo.Size = new System.Drawing.Size(219, 22);
      this.cmShowInfo.Text = "Info...";
      this.cmRating.DropDown = (ToolStripDropDown) this.contextRating2;
      this.cmRating.Name = "cmRating";
      this.cmRating.Size = new System.Drawing.Size(219, 22);
      this.cmRating.Text = "My R&ating";
      this.contextRating2.Items.AddRange(new ToolStripItem[9]
      {
        (ToolStripItem) this.cmRate0,
        (ToolStripItem) this.toolStripMenuItem16,
        (ToolStripItem) this.cmRate1,
        (ToolStripItem) this.cmRate2,
        (ToolStripItem) this.cmRate3,
        (ToolStripItem) this.cmRate4,
        (ToolStripItem) this.cmRate5,
        (ToolStripItem) this.toolStripSeparator6,
        (ToolStripItem) this.cmQuickRating
      });
      this.contextRating2.Name = "contextRating2";
      this.contextRating2.OwnerItem = (ToolStripItem) this.cmRating;
      this.contextRating2.Size = new System.Drawing.Size(286, 170);
      this.cmRate0.Name = "cmRate0";
      this.cmRate0.ShortcutKeys = Keys.D0 | Keys.Shift | Keys.Alt;
      this.cmRate0.Size = new System.Drawing.Size(285, 22);
      this.cmRate0.Text = "None";
      this.toolStripMenuItem16.Name = "toolStripMenuItem16";
      this.toolStripMenuItem16.Size = new System.Drawing.Size(282, 6);
      this.cmRate1.Name = "cmRate1";
      this.cmRate1.ShortcutKeys = Keys.D1 | Keys.Shift | Keys.Alt;
      this.cmRate1.Size = new System.Drawing.Size(285, 22);
      this.cmRate1.Text = "* (1 Star)";
      this.cmRate2.Name = "cmRate2";
      this.cmRate2.ShortcutKeys = Keys.D2 | Keys.Shift | Keys.Alt;
      this.cmRate2.Size = new System.Drawing.Size(285, 22);
      this.cmRate2.Text = "** (2 Stars)";
      this.cmRate3.Name = "cmRate3";
      this.cmRate3.ShortcutKeys = Keys.D3 | Keys.Shift | Keys.Alt;
      this.cmRate3.Size = new System.Drawing.Size(285, 22);
      this.cmRate3.Text = "*** (3 Stars)";
      this.cmRate4.Name = "cmRate4";
      this.cmRate4.ShortcutKeys = Keys.D4 | Keys.Shift | Keys.Alt;
      this.cmRate4.Size = new System.Drawing.Size(285, 22);
      this.cmRate4.Text = "**** (4 Stars)";
      this.cmRate5.Name = "cmRate5";
      this.cmRate5.ShortcutKeys = Keys.D5 | Keys.Shift | Keys.Alt;
      this.cmRate5.Size = new System.Drawing.Size(285, 22);
      this.cmRate5.Text = "***** (5 Stars)";
      this.toolStripSeparator6.Name = "toolStripSeparator6";
      this.toolStripSeparator6.Size = new System.Drawing.Size(282, 6);
      this.cmQuickRating.Name = "cmQuickRating";
      this.cmQuickRating.ShortcutKeys = Keys.Q | Keys.Shift | Keys.Alt;
      this.cmQuickRating.Size = new System.Drawing.Size(285, 22);
      this.cmQuickRating.Text = "Quick Rating and Review...";
      this.cmPageType.Name = "cmPageType";
      this.cmPageType.Size = new System.Drawing.Size(219, 22);
      this.cmPageType.Text = "&Page Type";
      this.cmPageRotate.Name = "cmPageRotate";
      this.cmPageRotate.Size = new System.Drawing.Size(219, 22);
      this.cmPageRotate.Text = "Page Rotation";
      this.cmBookmarks.DropDownItems.AddRange(new ToolStripItem[8]
      {
        (ToolStripItem) this.cmSetBookmark,
        (ToolStripItem) this.cmRemoveBookmark,
        (ToolStripItem) this.toolStripMenuItem32,
        (ToolStripItem) this.cmPrevBookmark,
        (ToolStripItem) this.cmNextBookmark,
        (ToolStripItem) this.toolStripSeparator13,
        (ToolStripItem) this.cmLastPageRead,
        (ToolStripItem) this.cmBookmarkSeparator
      });
      this.cmBookmarks.Image = (Image) Resources.Bookmark;
      this.cmBookmarks.Name = "cmBookmarks";
      this.cmBookmarks.Size = new System.Drawing.Size(219, 22);
      this.cmBookmarks.Text = "&Bookmarks";
      this.cmBookmarks.DropDownOpening += new EventHandler(this.cmBookmarks_DropDownOpening);
      this.cmSetBookmark.Image = (Image) Resources.NewBookmark;
      this.cmSetBookmark.Name = "cmSetBookmark";
      this.cmSetBookmark.ShortcutKeys = Keys.B | Keys.Shift | Keys.Control;
      this.cmSetBookmark.Size = new System.Drawing.Size(249, 22);
      this.cmSetBookmark.Text = "Set Bookmark...";
      this.cmRemoveBookmark.Image = (Image) Resources.RemoveBookmark;
      this.cmRemoveBookmark.Name = "cmRemoveBookmark";
      this.cmRemoveBookmark.ShortcutKeys = Keys.D | Keys.Shift | Keys.Control;
      this.cmRemoveBookmark.Size = new System.Drawing.Size(249, 22);
      this.cmRemoveBookmark.Text = "Remove Bookmark";
      this.toolStripMenuItem32.Name = "toolStripMenuItem32";
      this.toolStripMenuItem32.Size = new System.Drawing.Size(246, 6);
      this.cmPrevBookmark.Image = (Image) Resources.PreviousBookmark;
      this.cmPrevBookmark.Name = "cmPrevBookmark";
      this.cmPrevBookmark.ShortcutKeys = Keys.P | Keys.Shift | Keys.Control;
      this.cmPrevBookmark.Size = new System.Drawing.Size(249, 22);
      this.cmPrevBookmark.Text = "Previous Bookmark";
      this.cmNextBookmark.Image = (Image) Resources.NextBookmark;
      this.cmNextBookmark.Name = "cmNextBookmark";
      this.cmNextBookmark.ShortcutKeys = Keys.N | Keys.Shift | Keys.Control;
      this.cmNextBookmark.Size = new System.Drawing.Size(249, 22);
      this.cmNextBookmark.Text = "Next Bookmark";
      this.toolStripSeparator13.Name = "toolStripSeparator13";
      this.toolStripSeparator13.Size = new System.Drawing.Size(246, 6);
      this.cmLastPageRead.Name = "cmLastPageRead";
      this.cmLastPageRead.Size = new System.Drawing.Size(249, 22);
      this.cmLastPageRead.Text = "L&ast Page Read";
      this.cmBookmarkSeparator.Name = "cmBookmarkSeparator";
      this.cmBookmarkSeparator.Size = new System.Drawing.Size(246, 6);
      this.cmBookmarkSeparator.Tag = (object) "bms";
      this.toolStripSeparator10.Name = "toolStripSeparator10";
      this.toolStripSeparator10.Size = new System.Drawing.Size(216, 6);
      this.cmComics.DropDownItems.AddRange(new ToolStripItem[7]
      {
        (ToolStripItem) this.cmOpenComic,
        (ToolStripItem) this.cmCloseComic,
        (ToolStripItem) this.toolStripMenuItem13,
        (ToolStripItem) this.cmPrevFromList,
        (ToolStripItem) this.cmNextFromList,
        (ToolStripItem) this.cmRandomFromList,
        (ToolStripItem) this.cmComicsSep
      });
      this.cmComics.Name = "cmComics";
      this.cmComics.Size = new System.Drawing.Size(219, 22);
      this.cmComics.Text = "Books";
      this.cmOpenComic.Image = (Image) Resources.Open;
      this.cmOpenComic.Name = "cmOpenComic";
      this.cmOpenComic.ShortcutKeys = Keys.O | Keys.Control;
      this.cmOpenComic.Size = new System.Drawing.Size(218, 22);
      this.cmOpenComic.Text = "&Open File...";
      this.cmCloseComic.Name = "cmCloseComic";
      this.cmCloseComic.ShortcutKeys = Keys.X | Keys.Control;
      this.cmCloseComic.Size = new System.Drawing.Size(218, 22);
      this.cmCloseComic.Text = "&Close";
      this.toolStripMenuItem13.Name = "toolStripMenuItem13";
      this.toolStripMenuItem13.Size = new System.Drawing.Size(215, 6);
      this.cmPrevFromList.Image = (Image) Resources.PrevFromList;
      this.cmPrevFromList.Name = "cmPrevFromList";
      this.cmPrevFromList.ShortcutKeyDisplayString = "";
      this.cmPrevFromList.ShortcutKeys = Keys.P | Keys.Shift | Keys.Alt;
      this.cmPrevFromList.Size = new System.Drawing.Size(218, 22);
      this.cmPrevFromList.Text = "Pre&vious Book";
      this.cmNextFromList.Image = (Image) Resources.NextFromList;
      this.cmNextFromList.Name = "cmNextFromList";
      this.cmNextFromList.ShortcutKeyDisplayString = "";
      this.cmNextFromList.ShortcutKeys = Keys.N | Keys.Shift | Keys.Alt;
      this.cmNextFromList.Size = new System.Drawing.Size(218, 22);
      this.cmNextFromList.Text = "Ne&xt Book";
      this.cmRandomFromList.Image = (Image) Resources.RandomComic;
      this.cmRandomFromList.Name = "cmRandomFromList";
      this.cmRandomFromList.ShortcutKeys = Keys.N | Keys.Control | Keys.Alt;
      this.cmRandomFromList.Size = new System.Drawing.Size(218, 22);
      this.cmRandomFromList.Text = "Random Book";
      this.cmComicsSep.Name = "cmComicsSep";
      this.cmComicsSep.Size = new System.Drawing.Size(215, 6);
      this.cmPageLayout.DropDownItems.AddRange(new ToolStripItem[18]
      {
        (ToolStripItem) this.cmOriginal,
        (ToolStripItem) this.cmFitAll,
        (ToolStripItem) this.cmFitWidth,
        (ToolStripItem) this.cmFitWidthAdaptive,
        (ToolStripItem) this.cmFitHeight,
        (ToolStripItem) this.cmFitBest,
        (ToolStripItem) this.toolStripMenuItem29,
        (ToolStripItem) this.cmSinglePage,
        (ToolStripItem) this.cmTwoPages,
        (ToolStripItem) this.cmTwoPagesAdaptive,
        (ToolStripItem) this.cmRightToLeft,
        (ToolStripItem) this.toolStripMenuItem38,
        (ToolStripItem) this.cmRotate0,
        (ToolStripItem) this.cmRotate90,
        (ToolStripItem) this.cmRotate180,
        (ToolStripItem) this.cmRotate270,
        (ToolStripItem) this.toolStripMenuItem55,
        (ToolStripItem) this.cmOnlyFitOversized
      });
      this.cmPageLayout.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.cmPageLayout.Name = "cmPageLayout";
      this.cmPageLayout.Size = new System.Drawing.Size(219, 22);
      this.cmPageLayout.Text = "Page Layout";
      this.cmOriginal.Image = (Image) Resources.Original;
      this.cmOriginal.Name = "cmOriginal";
      this.cmOriginal.ShortcutKeys = Keys.D1 | Keys.Control;
      this.cmOriginal.Size = new System.Drawing.Size(241, 22);
      this.cmOriginal.Text = "Original";
      this.cmFitAll.Image = (Image) Resources.FitAll;
      this.cmFitAll.Name = "cmFitAll";
      this.cmFitAll.ShortcutKeys = Keys.D2 | Keys.Control;
      this.cmFitAll.Size = new System.Drawing.Size(241, 22);
      this.cmFitAll.Text = "Fit All";
      this.cmFitWidth.Image = (Image) Resources.FitWidth;
      this.cmFitWidth.Name = "cmFitWidth";
      this.cmFitWidth.ShortcutKeys = Keys.D3 | Keys.Control;
      this.cmFitWidth.Size = new System.Drawing.Size(241, 22);
      this.cmFitWidth.Text = "Fit Width";
      this.cmFitWidthAdaptive.Image = (Image) Resources.FitWidthAdaptive;
      this.cmFitWidthAdaptive.Name = "cmFitWidthAdaptive";
      this.cmFitWidthAdaptive.ShortcutKeys = Keys.D4 | Keys.Control;
      this.cmFitWidthAdaptive.Size = new System.Drawing.Size(241, 22);
      this.cmFitWidthAdaptive.Text = "Fit Width (adaptive)";
      this.cmFitHeight.Image = (Image) Resources.FitHeight;
      this.cmFitHeight.Name = "cmFitHeight";
      this.cmFitHeight.ShortcutKeys = Keys.D5 | Keys.Control;
      this.cmFitHeight.Size = new System.Drawing.Size(241, 22);
      this.cmFitHeight.Text = "Fit Height";
      this.cmFitBest.Image = (Image) Resources.FitBest;
      this.cmFitBest.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.cmFitBest.Name = "cmFitBest";
      this.cmFitBest.ShortcutKeys = Keys.D6 | Keys.Control;
      this.cmFitBest.Size = new System.Drawing.Size(241, 22);
      this.cmFitBest.Text = "Fit Best";
      this.toolStripMenuItem29.Name = "toolStripMenuItem29";
      this.toolStripMenuItem29.Size = new System.Drawing.Size(238, 6);
      this.cmSinglePage.Image = (Image) Resources.SinglePage;
      this.cmSinglePage.Name = "cmSinglePage";
      this.cmSinglePage.ShortcutKeys = Keys.D7 | Keys.Control;
      this.cmSinglePage.Size = new System.Drawing.Size(241, 22);
      this.cmSinglePage.Text = "Single Page";
      this.cmTwoPages.Image = (Image) Resources.TwoPageForced;
      this.cmTwoPages.Name = "cmTwoPages";
      this.cmTwoPages.ShortcutKeys = Keys.D8 | Keys.Control;
      this.cmTwoPages.Size = new System.Drawing.Size(241, 22);
      this.cmTwoPages.Text = "Two Pages";
      this.cmTwoPagesAdaptive.Image = (Image) Resources.TwoPage;
      this.cmTwoPagesAdaptive.Name = "cmTwoPagesAdaptive";
      this.cmTwoPagesAdaptive.ShortcutKeys = Keys.D9 | Keys.Control;
      this.cmTwoPagesAdaptive.Size = new System.Drawing.Size(241, 22);
      this.cmTwoPagesAdaptive.Text = "Two Pages (adaptive)";
      this.cmTwoPagesAdaptive.ToolTipText = "Show one or two pages";
      this.cmRightToLeft.Image = (Image) Resources.RightToLeft;
      this.cmRightToLeft.Name = "cmRightToLeft";
      this.cmRightToLeft.ShortcutKeys = Keys.D0 | Keys.Control;
      this.cmRightToLeft.Size = new System.Drawing.Size(241, 22);
      this.cmRightToLeft.Text = "Right to Left";
      this.toolStripMenuItem38.Name = "toolStripMenuItem38";
      this.toolStripMenuItem38.Size = new System.Drawing.Size(238, 6);
      this.cmRotate0.Image = (Image) Resources.Rotate0;
      this.cmRotate0.Name = "cmRotate0";
      this.cmRotate0.ShortcutKeys = Keys.D7 | Keys.Shift | Keys.Control;
      this.cmRotate0.Size = new System.Drawing.Size(241, 22);
      this.cmRotate0.Text = "&No Rotation";
      this.cmRotate90.Image = (Image) Resources.Rotate90;
      this.cmRotate90.Name = "cmRotate90";
      this.cmRotate90.ShortcutKeys = Keys.D8 | Keys.Shift | Keys.Control;
      this.cmRotate90.Size = new System.Drawing.Size(241, 22);
      this.cmRotate90.Text = "90°";
      this.cmRotate180.Image = (Image) Resources.Rotate180;
      this.cmRotate180.Name = "cmRotate180";
      this.cmRotate180.ShortcutKeys = Keys.D9 | Keys.Shift | Keys.Control;
      this.cmRotate180.Size = new System.Drawing.Size(241, 22);
      this.cmRotate180.Text = "180°";
      this.cmRotate270.Image = (Image) Resources.Rotate270;
      this.cmRotate270.Name = "cmRotate270";
      this.cmRotate270.ShortcutKeys = Keys.D0 | Keys.Shift | Keys.Control;
      this.cmRotate270.Size = new System.Drawing.Size(241, 22);
      this.cmRotate270.Text = "270°";
      this.toolStripMenuItem55.Name = "toolStripMenuItem55";
      this.toolStripMenuItem55.Size = new System.Drawing.Size(238, 6);
      this.cmOnlyFitOversized.Image = (Image) Resources.Oversized;
      this.cmOnlyFitOversized.Name = "cmOnlyFitOversized";
      this.cmOnlyFitOversized.ShortcutKeys = Keys.O | Keys.Control | Keys.Alt;
      this.cmOnlyFitOversized.Size = new System.Drawing.Size(241, 22);
      this.cmOnlyFitOversized.Text = "&Only fit if oversized";
      this.cmMagnify.Image = (Image) Resources.Zoom;
      this.cmMagnify.Name = "cmMagnify";
      this.cmMagnify.ShortcutKeys = Keys.M | Keys.Control;
      this.cmMagnify.Size = new System.Drawing.Size(219, 22);
      this.cmMagnify.Text = "&Magnifier";
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(216, 6);
      this.cmCopyPage.Image = (Image) Resources.Copy;
      this.cmCopyPage.Name = "cmCopyPage";
      this.cmCopyPage.ShortcutKeys = Keys.C | Keys.Control;
      this.cmCopyPage.Size = new System.Drawing.Size(219, 22);
      this.cmCopyPage.Text = "&Copy Page";
      this.cmExportPage.Name = "cmExportPage";
      this.cmExportPage.ShortcutKeys = Keys.C | Keys.Shift | Keys.Control;
      this.cmExportPage.Size = new System.Drawing.Size(219, 22);
      this.cmExportPage.Text = "&Export Page...";
      this.toolStripMenuItem11.Name = "toolStripMenuItem11";
      this.toolStripMenuItem11.Size = new System.Drawing.Size(216, 6);
      this.cmRefreshPage.Image = (Image) Resources.Refresh;
      this.cmRefreshPage.Name = "cmRefreshPage";
      this.cmRefreshPage.ShortcutKeys = Keys.F5;
      this.cmRefreshPage.Size = new System.Drawing.Size(219, 22);
      this.cmRefreshPage.Text = "&Refresh";
      this.toolStripMenuItem46.Name = "toolStripMenuItem46";
      this.toolStripMenuItem46.Size = new System.Drawing.Size(216, 6);
      this.cmMinimalGui.Image = (Image) Resources.MenuToggle;
      this.cmMinimalGui.Name = "cmMinimalGui";
      this.cmMinimalGui.ShortcutKeys = Keys.F10;
      this.cmMinimalGui.Size = new System.Drawing.Size(219, 22);
      this.cmMinimalGui.Text = "&Minimal User Interface";
      this.notifyIcon.ContextMenuStrip = this.notfifyContextMenu;
      this.notifyIcon.Text = "Double Click to restore";
      this.notifyIcon.BalloonTipClicked += new EventHandler(this.notifyIcon_BalloonTipClicked);
      this.notfifyContextMenu.Items.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.cmNotifyRestore,
        (ToolStripItem) this.toolStripMenuItem15,
        (ToolStripItem) this.cmNotifyExit
      });
      this.notfifyContextMenu.Name = "notfifyContextMenu";
      this.notfifyContextMenu.Size = new System.Drawing.Size(114, 54);
      this.cmNotifyRestore.Name = "cmNotifyRestore";
      this.cmNotifyRestore.Size = new System.Drawing.Size(113, 22);
      this.cmNotifyRestore.Text = "&Restore";
      this.toolStripMenuItem15.Name = "toolStripMenuItem15";
      this.toolStripMenuItem15.Size = new System.Drawing.Size(110, 6);
      this.cmNotifyExit.Name = "cmNotifyExit";
      this.cmNotifyExit.Size = new System.Drawing.Size(113, 22);
      this.cmNotifyExit.Text = "&Exit";
      this.viewContainer.Controls.Add((Control) this.panelReader);
      this.viewContainer.Dock = DockStyle.Fill;
      this.viewContainer.Location = new System.Drawing.Point(0, 24);
      this.viewContainer.Name = "viewContainer";
      this.viewContainer.Size = new System.Drawing.Size(744, 364);
      this.viewContainer.TabIndex = 14;
      this.panelReader.Controls.Add((Control) this.readerContainer);
      this.panelReader.Dock = DockStyle.Fill;
      this.panelReader.Location = new System.Drawing.Point(0, 0);
      this.panelReader.Name = "panelReader";
      this.panelReader.Size = new System.Drawing.Size(744, 364);
      this.panelReader.TabIndex = 2;
      this.readerContainer.Controls.Add((Control) this.quickOpenView);
      this.readerContainer.Controls.Add((Control) this.fileTabs);
      this.readerContainer.Dock = DockStyle.Fill;
      this.readerContainer.Location = new System.Drawing.Point(0, 0);
      this.readerContainer.Name = "readerContainer";
      this.readerContainer.Size = new System.Drawing.Size(744, 364);
      this.readerContainer.TabIndex = 0;
      this.readerContainer.Paint += new PaintEventHandler(this.readerContainer_Paint);
      this.quickOpenView.AllowDrop = true;
      this.quickOpenView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.quickOpenView.BackColor = SystemColors.Window;
      this.quickOpenView.BorderStyle = BorderStyle.FixedSingle;
      this.quickOpenView.Caption = "Quick Open";
      this.quickOpenView.CaptionMargin = new Padding(2);
      this.quickOpenView.Location = new System.Drawing.Point(63, 50);
      this.quickOpenView.Margin = new Padding(12);
      this.quickOpenView.MinimumSize = new System.Drawing.Size(300, 250);
      this.quickOpenView.Name = "quickOpenView";
      this.quickOpenView.ShowBrowserCommand = true;
      this.quickOpenView.Size = new System.Drawing.Size(616, 289);
      this.quickOpenView.TabIndex = 2;
      this.quickOpenView.ThumbnailSize = 128;
      this.quickOpenView.Visible = false;
      this.quickOpenView.BookActivated += new EventHandler(this.QuickOpenBookActivated);
      this.quickOpenView.ShowBrowser += new EventHandler(this.quickOpenView_ShowBrowser);
      this.quickOpenView.OpenFile += new EventHandler(this.quickOpenView_OpenFile);
      this.quickOpenView.VisibleChanged += new EventHandler(this.QuickOpenVisibleChanged);
      this.quickOpenView.DragDrop += new DragEventHandler(this.BookDragDrop);
      this.quickOpenView.DragEnter += new DragEventHandler(this.BookDragEnter);
      this.fileTabs.AllowDrop = true;
      this.fileTabs.CloseImage = Resources.Close;
      this.fileTabs.Controls.Add((Control) this.mainToolStrip);
      this.fileTabs.Dock = DockStyle.Top;
      this.fileTabs.DragDropReorder = true;
      this.fileTabs.LeftIndent = 8;
      this.fileTabs.Location = new System.Drawing.Point(0, 0);
      this.fileTabs.Name = "fileTabs";
      this.fileTabs.OwnerDrawnTooltips = true;
      this.fileTabs.Size = new System.Drawing.Size(744, 31);
      this.fileTabs.TabIndex = 1;
      this.mainToolStrip.BackColor = System.Drawing.Color.Transparent;
      this.mainToolStrip.Dock = DockStyle.Right;
      this.mainToolStrip.GripStyle = ToolStripGripStyle.Hidden;
      this.mainToolStrip.Items.AddRange(new ToolStripItem[12]
      {
        (ToolStripItem) this.tbPrevPage,
        (ToolStripItem) this.tbNextPage,
        (ToolStripItem) this.toolStripSeparator5,
        (ToolStripItem) this.tbPageLayout,
        (ToolStripItem) this.tbFit,
        (ToolStripItem) this.tbZoom,
        (ToolStripItem) this.tbRotate,
        (ToolStripItem) this.toolStripSeparator7,
        (ToolStripItem) this.tbMagnify,
        (ToolStripItem) this.tbFullScreen,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.tbTools
      });
      this.mainToolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
      this.mainToolStrip.Location = new System.Drawing.Point(400, 1);
      this.mainToolStrip.MinimumSize = new System.Drawing.Size(0, 24);
      this.mainToolStrip.Name = "mainToolStrip";
      this.mainToolStrip.Size = new System.Drawing.Size(344, 25);
      this.mainToolStrip.TabIndex = 2;
      this.mainToolStrip.Text = "mainToolStrip";
      this.tbPrevPage.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbPrevPage.DropDownItems.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.tbFirstPage,
        (ToolStripItem) this.tbPrevBookmark,
        (ToolStripItem) this.toolStripMenuItem53,
        (ToolStripItem) this.toolStripMenuItem19,
        (ToolStripItem) this.tbPrevFromList
      });
      this.tbPrevPage.Image = (Image) Resources.GoPrevious;
      this.tbPrevPage.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbPrevPage.Name = "tbPrevPage";
      this.tbPrevPage.Size = new System.Drawing.Size(32, 22);
      this.tbPrevPage.Text = "Previous Page";
      this.tbPrevPage.DropDownOpening += new EventHandler(this.tbPrevPage_DropDownOpening);
      this.tbFirstPage.Image = (Image) Resources.GoFirst;
      this.tbFirstPage.Name = "tbFirstPage";
      this.tbFirstPage.ShortcutKeyDisplayString = "";
      this.tbFirstPage.ShortcutKeys = Keys.B | Keys.Control;
      this.tbFirstPage.Size = new System.Drawing.Size(268, 22);
      this.tbFirstPage.Text = "&First Page";
      this.tbPrevBookmark.Image = (Image) Resources.PreviousBookmark;
      this.tbPrevBookmark.Name = "tbPrevBookmark";
      this.tbPrevBookmark.ShortcutKeys = Keys.P | Keys.Shift | Keys.Control;
      this.tbPrevBookmark.Size = new System.Drawing.Size(268, 22);
      this.tbPrevBookmark.Text = "Previous Bookmark";
      this.toolStripMenuItem53.Name = "toolStripMenuItem53";
      this.toolStripMenuItem53.Size = new System.Drawing.Size(265, 6);
      this.toolStripMenuItem53.Tag = (object) "bms";
      this.toolStripMenuItem19.Name = "toolStripMenuItem19";
      this.toolStripMenuItem19.Size = new System.Drawing.Size(265, 6);
      this.tbPrevFromList.Image = (Image) Resources.PrevFromList;
      this.tbPrevFromList.Name = "tbPrevFromList";
      this.tbPrevFromList.ShortcutKeys = Keys.P | Keys.Shift | Keys.Alt;
      this.tbPrevFromList.Size = new System.Drawing.Size(268, 22);
      this.tbPrevFromList.Text = "Previous Book from List";
      this.tbNextPage.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbNextPage.DropDownItems.AddRange(new ToolStripItem[7]
      {
        (ToolStripItem) this.tbLastPage,
        (ToolStripItem) this.tbNextBookmark,
        (ToolStripItem) this.tbLastPageRead,
        (ToolStripItem) this.toolStripMenuItem28,
        (ToolStripItem) this.toolStripMenuItem49,
        (ToolStripItem) this.tbNextFromList,
        (ToolStripItem) this.tbRandomFromList
      });
      this.tbNextPage.Image = (Image) Resources.GoNext;
      this.tbNextPage.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbNextPage.Name = "tbNextPage";
      this.tbNextPage.Size = new System.Drawing.Size(32, 22);
      this.tbNextPage.Text = "Next Page";
      this.tbNextPage.DropDownOpening += new EventHandler(this.tbNextPage_DropDownOpening);
      this.tbLastPage.Image = (Image) Resources.GoLast;
      this.tbLastPage.Name = "tbLastPage";
      this.tbLastPage.ShortcutKeyDisplayString = "";
      this.tbLastPage.ShortcutKeys = Keys.E | Keys.Control;
      this.tbLastPage.Size = new System.Drawing.Size(249, 22);
      this.tbLastPage.Text = "&Last Page";
      this.tbNextBookmark.Image = (Image) Resources.NextBookmark;
      this.tbNextBookmark.Name = "tbNextBookmark";
      this.tbNextBookmark.ShortcutKeys = Keys.N | Keys.Shift | Keys.Control;
      this.tbNextBookmark.Size = new System.Drawing.Size(249, 22);
      this.tbNextBookmark.Text = "Next Bookmark";
      this.tbLastPageRead.Name = "tbLastPageRead";
      this.tbLastPageRead.ShortcutKeys = Keys.L | Keys.Shift | Keys.Control;
      this.tbLastPageRead.Size = new System.Drawing.Size(249, 22);
      this.tbLastPageRead.Text = "L&ast Page Read";
      this.toolStripMenuItem28.Name = "toolStripMenuItem28";
      this.toolStripMenuItem28.Size = new System.Drawing.Size(246, 6);
      this.toolStripMenuItem28.Tag = (object) "bms";
      this.toolStripMenuItem49.Name = "toolStripMenuItem49";
      this.toolStripMenuItem49.Size = new System.Drawing.Size(246, 6);
      this.tbNextFromList.Image = (Image) Resources.NextFromList;
      this.tbNextFromList.Name = "tbNextFromList";
      this.tbNextFromList.ShortcutKeys = Keys.N | Keys.Shift | Keys.Alt;
      this.tbNextFromList.Size = new System.Drawing.Size(249, 22);
      this.tbNextFromList.Text = "Next Book from List";
      this.tbRandomFromList.Image = (Image) Resources.RandomComic;
      this.tbRandomFromList.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbRandomFromList.Name = "tbRandomFromList";
      this.tbRandomFromList.ShortcutKeys = Keys.N | Keys.Control | Keys.Alt;
      this.tbRandomFromList.Size = new System.Drawing.Size(249, 22);
      this.tbRandomFromList.Text = "Random Book";
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
      this.tbPageLayout.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbPageLayout.DropDownItems.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.tbSinglePage,
        (ToolStripItem) this.tbTwoPages,
        (ToolStripItem) this.tbTwoPagesAdaptive,
        (ToolStripItem) this.toolStripMenuItem54,
        (ToolStripItem) this.tbRightToLeft
      });
      this.tbPageLayout.Image = (Image) Resources.SinglePage;
      this.tbPageLayout.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbPageLayout.Name = "tbPageLayout";
      this.tbPageLayout.Size = new System.Drawing.Size(32, 22);
      this.tbPageLayout.Text = "Page Layout";
      this.tbSinglePage.Image = (Image) Resources.SinglePage;
      this.tbSinglePage.Name = "tbSinglePage";
      this.tbSinglePage.ShortcutKeys = Keys.D7 | Keys.Control;
      this.tbSinglePage.Size = new System.Drawing.Size(226, 22);
      this.tbSinglePage.Text = "Single Page";
      this.tbTwoPages.Image = (Image) Resources.TwoPageForced;
      this.tbTwoPages.Name = "tbTwoPages";
      this.tbTwoPages.ShortcutKeys = Keys.D8 | Keys.Control;
      this.tbTwoPages.Size = new System.Drawing.Size(226, 22);
      this.tbTwoPages.Text = "Two Pages";
      this.tbTwoPagesAdaptive.Image = (Image) Resources.TwoPage;
      this.tbTwoPagesAdaptive.Name = "tbTwoPagesAdaptive";
      this.tbTwoPagesAdaptive.ShortcutKeys = Keys.D9 | Keys.Control;
      this.tbTwoPagesAdaptive.Size = new System.Drawing.Size(226, 22);
      this.tbTwoPagesAdaptive.Text = "Two Pages (adaptive)";
      this.tbTwoPagesAdaptive.ToolTipText = "Show one or two pages";
      this.toolStripMenuItem54.Name = "toolStripMenuItem54";
      this.toolStripMenuItem54.Size = new System.Drawing.Size(223, 6);
      this.tbRightToLeft.Image = (Image) Resources.RightToLeft;
      this.tbRightToLeft.Name = "tbRightToLeft";
      this.tbRightToLeft.ShortcutKeys = Keys.D0 | Keys.Control;
      this.tbRightToLeft.Size = new System.Drawing.Size(226, 22);
      this.tbRightToLeft.Text = "Right to Left";
      this.tbFit.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbFit.DropDownItems.AddRange(new ToolStripItem[8]
      {
        (ToolStripItem) this.tbOriginal,
        (ToolStripItem) this.tbFitAll,
        (ToolStripItem) this.tbFitWidth,
        (ToolStripItem) this.tbFitWidthAdaptive,
        (ToolStripItem) this.tbFitHeight,
        (ToolStripItem) this.tbBestFit,
        (ToolStripItem) this.toolStripMenuItem20,
        (ToolStripItem) this.tbOnlyFitOversized
      });
      this.tbFit.Image = (Image) Resources.FitAll;
      this.tbFit.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbFit.Name = "tbFit";
      this.tbFit.Size = new System.Drawing.Size(32, 22);
      this.tbFit.Text = "Fit";
      this.tbFit.ToolTipText = "Toggle Fit Mode";
      this.tbOriginal.Image = (Image) Resources.Original;
      this.tbOriginal.Name = "tbOriginal";
      this.tbOriginal.ShortcutKeys = Keys.D1 | Keys.Control;
      this.tbOriginal.Size = new System.Drawing.Size(247, 22);
      this.tbOriginal.Text = "Original Size";
      this.tbFitAll.Image = (Image) Resources.FitAll;
      this.tbFitAll.Name = "tbFitAll";
      this.tbFitAll.ShortcutKeys = Keys.D2 | Keys.Control;
      this.tbFitAll.Size = new System.Drawing.Size(247, 22);
      this.tbFitAll.Text = "Fit All";
      this.tbFitWidth.Image = (Image) Resources.FitWidth;
      this.tbFitWidth.Name = "tbFitWidth";
      this.tbFitWidth.ShortcutKeys = Keys.D3 | Keys.Control;
      this.tbFitWidth.Size = new System.Drawing.Size(247, 22);
      this.tbFitWidth.Text = "Fit Width";
      this.tbFitWidthAdaptive.Image = (Image) Resources.FitWidthAdaptive;
      this.tbFitWidthAdaptive.Name = "tbFitWidthAdaptive";
      this.tbFitWidthAdaptive.ShortcutKeys = Keys.D4 | Keys.Control;
      this.tbFitWidthAdaptive.Size = new System.Drawing.Size(247, 22);
      this.tbFitWidthAdaptive.Text = "Fit Width (adaptive)";
      this.tbFitHeight.Image = (Image) Resources.FitHeight;
      this.tbFitHeight.Name = "tbFitHeight";
      this.tbFitHeight.ShortcutKeys = Keys.D5 | Keys.Control;
      this.tbFitHeight.Size = new System.Drawing.Size(247, 22);
      this.tbFitHeight.Text = "Fit Height";
      this.tbBestFit.Image = (Image) Resources.FitBest;
      this.tbBestFit.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbBestFit.Name = "tbBestFit";
      this.tbBestFit.ShortcutKeys = Keys.D6 | Keys.Control;
      this.tbBestFit.Size = new System.Drawing.Size(247, 22);
      this.tbBestFit.Text = "Fit Best";
      this.toolStripMenuItem20.Name = "toolStripMenuItem20";
      this.toolStripMenuItem20.Size = new System.Drawing.Size(244, 6);
      this.tbOnlyFitOversized.Image = (Image) Resources.Oversized;
      this.tbOnlyFitOversized.Name = "tbOnlyFitOversized";
      this.tbOnlyFitOversized.ShortcutKeys = Keys.D0 | Keys.Shift | Keys.Control;
      this.tbOnlyFitOversized.Size = new System.Drawing.Size(247, 22);
      this.tbOnlyFitOversized.Text = "&Only fit if oversized";
      this.tbZoom.AutoToolTip = false;
      this.tbZoom.DropDownItems.AddRange(new ToolStripItem[10]
      {
        (ToolStripItem) this.tbZoomIn,
        (ToolStripItem) this.tbZoomOut,
        (ToolStripItem) this.toolStripMenuItem30,
        (ToolStripItem) this.tbZoom100,
        (ToolStripItem) this.tbZoom125,
        (ToolStripItem) this.tbZoom150,
        (ToolStripItem) this.tbZoom200,
        (ToolStripItem) this.tbZoom400,
        (ToolStripItem) this.toolStripMenuItem31,
        (ToolStripItem) this.tbZoomCustom
      });
      this.tbZoom.Image = (Image) Resources.ZoomIn;
      this.tbZoom.Name = "tbZoom";
      this.tbZoom.Size = new System.Drawing.Size(70, 22);
      this.tbZoom.Text = "100 %";
      this.tbZoom.ToolTipText = "Change the page zoom";
      this.tbZoomIn.Image = (Image) Resources.ZoomIn;
      this.tbZoomIn.Name = "tbZoomIn";
      this.tbZoomIn.ShortcutKeys = Keys.Oemplus | Keys.Control;
      this.tbZoomIn.Size = new System.Drawing.Size(222, 22);
      this.tbZoomIn.Text = "Zoom &In";
      this.tbZoomOut.Image = (Image) Resources.ZoomOut;
      this.tbZoomOut.Name = "tbZoomOut";
      this.tbZoomOut.ShortcutKeys = Keys.OemMinus | Keys.Control;
      this.tbZoomOut.Size = new System.Drawing.Size(222, 22);
      this.tbZoomOut.Text = "Zoom &Out";
      this.toolStripMenuItem30.Name = "toolStripMenuItem30";
      this.toolStripMenuItem30.Size = new System.Drawing.Size(219, 6);
      this.tbZoom100.Name = "tbZoom100";
      this.tbZoom100.Size = new System.Drawing.Size(222, 22);
      this.tbZoom100.Text = "100%";
      this.tbZoom125.Name = "tbZoom125";
      this.tbZoom125.Size = new System.Drawing.Size(222, 22);
      this.tbZoom125.Text = "125%";
      this.tbZoom150.Name = "tbZoom150";
      this.tbZoom150.Size = new System.Drawing.Size(222, 22);
      this.tbZoom150.Text = "150%";
      this.tbZoom200.Name = "tbZoom200";
      this.tbZoom200.Size = new System.Drawing.Size(222, 22);
      this.tbZoom200.Text = "200%";
      this.tbZoom400.Name = "tbZoom400";
      this.tbZoom400.Size = new System.Drawing.Size(222, 22);
      this.tbZoom400.Text = "400%";
      this.toolStripMenuItem31.Name = "toolStripMenuItem31";
      this.toolStripMenuItem31.Size = new System.Drawing.Size(219, 6);
      this.tbZoomCustom.Name = "tbZoomCustom";
      this.tbZoomCustom.ShortcutKeys = Keys.Z | Keys.Shift | Keys.Control;
      this.tbZoomCustom.Size = new System.Drawing.Size(222, 22);
      this.tbZoomCustom.Text = "&Custom...";
      this.tbRotate.AutoToolTip = false;
      this.tbRotate.DropDownItems.AddRange(new ToolStripItem[9]
      {
        (ToolStripItem) this.tbRotateLeft,
        (ToolStripItem) this.tbRotateRight,
        (ToolStripItem) this.toolStripSeparator11,
        (ToolStripItem) this.tbRotate0,
        (ToolStripItem) this.tbRotate90,
        (ToolStripItem) this.tbRotate180,
        (ToolStripItem) this.tbRotate270,
        (ToolStripItem) this.toolStripMenuItem34,
        (ToolStripItem) this.tbAutoRotate
      });
      this.tbRotate.Image = (Image) Resources.RotateRight;
      this.tbRotate.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbRotate.Name = "tbRotate";
      this.tbRotate.Size = new System.Drawing.Size(50, 22);
      this.tbRotate.Text = "0°";
      this.tbRotate.ToolTipText = "Change the page rotation";
      this.tbRotateLeft.Image = (Image) Resources.RotateLeft;
      this.tbRotateLeft.Name = "tbRotateLeft";
      this.tbRotateLeft.ShortcutKeys = Keys.OemMinus | Keys.Shift | Keys.Control;
      this.tbRotateLeft.Size = new System.Drawing.Size(256, 22);
      this.tbRotateLeft.Text = "Rotate Left";
      this.tbRotateRight.Image = (Image) Resources.RotateRight;
      this.tbRotateRight.Name = "tbRotateRight";
      this.tbRotateRight.ShortcutKeys = Keys.Oemplus | Keys.Shift | Keys.Control;
      this.tbRotateRight.Size = new System.Drawing.Size(256, 22);
      this.tbRotateRight.Text = "Rotate Right";
      this.toolStripSeparator11.Name = "toolStripSeparator11";
      this.toolStripSeparator11.Size = new System.Drawing.Size(253, 6);
      this.tbRotate0.Image = (Image) Resources.Rotate0;
      this.tbRotate0.Name = "tbRotate0";
      this.tbRotate0.ShortcutKeys = Keys.D7 | Keys.Shift | Keys.Control;
      this.tbRotate0.Size = new System.Drawing.Size(256, 22);
      this.tbRotate0.Text = "&No Rotation";
      this.tbRotate90.Image = (Image) Resources.Rotate90;
      this.tbRotate90.Name = "tbRotate90";
      this.tbRotate90.ShortcutKeys = Keys.D8 | Keys.Shift | Keys.Control;
      this.tbRotate90.Size = new System.Drawing.Size(256, 22);
      this.tbRotate90.Text = "90°";
      this.tbRotate180.Image = (Image) Resources.Rotate180;
      this.tbRotate180.Name = "tbRotate180";
      this.tbRotate180.ShortcutKeys = Keys.D9 | Keys.Shift | Keys.Control;
      this.tbRotate180.Size = new System.Drawing.Size(256, 22);
      this.tbRotate180.Text = "180°";
      this.tbRotate270.Image = (Image) Resources.Rotate270;
      this.tbRotate270.Name = "tbRotate270";
      this.tbRotate270.ShortcutKeys = Keys.D0 | Keys.Shift | Keys.Control;
      this.tbRotate270.Size = new System.Drawing.Size(256, 22);
      this.tbRotate270.Text = "270°";
      this.toolStripMenuItem34.Name = "toolStripMenuItem34";
      this.toolStripMenuItem34.Size = new System.Drawing.Size(253, 6);
      this.tbAutoRotate.Image = (Image) Resources.AutoRotate;
      this.tbAutoRotate.Name = "tbAutoRotate";
      this.tbAutoRotate.Size = new System.Drawing.Size(256, 22);
      this.tbAutoRotate.Text = "Autorotate Double Pages";
      this.toolStripSeparator7.Name = "toolStripSeparator7";
      this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
      this.tbMagnify.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbMagnify.Image = (Image) Resources.Zoom;
      this.tbMagnify.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbMagnify.Name = "tbMagnify";
      this.tbMagnify.Size = new System.Drawing.Size(32, 22);
      this.tbMagnify.Text = "Magnifier";
      this.tbFullScreen.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbFullScreen.Image = (Image) Resources.FullScreen;
      this.tbFullScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbFullScreen.Name = "tbFullScreen";
      this.tbFullScreen.Size = new System.Drawing.Size(23, 22);
      this.tbFullScreen.Text = "Full Screen";
      this.tbFullScreen.ToolTipText = "Full Screen";
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      this.tbTools.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbTools.DropDown = (ToolStripDropDown) this.toolsContextMenu;
      this.tbTools.Image = (Image) Resources.Tools;
      this.tbTools.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbTools.Name = "tbTools";
      this.tbTools.Overflow = ToolStripItemOverflow.Never;
      this.tbTools.ShowDropDownArrow = false;
      this.tbTools.Size = new System.Drawing.Size(20, 22);
      this.tbTools.Text = "Tools";
      this.tbTools.DropDownOpening += new EventHandler(this.tbTools_DropDownOpening);
      this.toolsContextMenu.Items.AddRange(new ToolStripItem[24]
      {
        (ToolStripItem) this.tbOpenComic,
        (ToolStripItem) this.tbOpenRemoteLibrary,
        (ToolStripItem) this.tbShowInfo,
        (ToolStripItem) this.toolStripMenuItem47,
        (ToolStripItem) this.tsWorkspaces,
        (ToolStripItem) this.tbBookmarks,
        (ToolStripItem) this.tbAutoScroll,
        (ToolStripItem) this.toolStripMenuItem45,
        (ToolStripItem) this.tbMinimalGui,
        (ToolStripItem) this.tbReaderUndocked,
        (ToolStripItem) this.toolStripMenuItem52,
        (ToolStripItem) this.tbScan,
        (ToolStripItem) this.tbUpdateAllComicFiles,
        (ToolStripItem) this.tbUpdateWebComics,
        (ToolStripItem) this.tsSynchronizeDevices,
        (ToolStripItem) this.toolStripMenuItem48,
        (ToolStripItem) this.tbComicDisplaySettings,
        (ToolStripItem) this.tbPreferences,
        (ToolStripItem) this.tbSupport,
        (ToolStripItem) this.tbAbout,
        (ToolStripItem) this.toolStripMenuItem50,
        (ToolStripItem) this.tbShowMainMenu,
        (ToolStripItem) this.toolStripMenuItem51,
        (ToolStripItem) this.tbExit
      });
      this.toolsContextMenu.Name = "toolsContextMenu";
      this.toolsContextMenu.OwnerItem = (ToolStripItem) this.tbTools;
      this.toolsContextMenu.Size = new System.Drawing.Size(285, 724);
      this.tbOpenComic.Image = (Image) Resources.Open;
      this.tbOpenComic.Name = "tbOpenComic";
      this.tbOpenComic.ShortcutKeys = Keys.O | Keys.Control;
      this.tbOpenComic.Size = new System.Drawing.Size(284, 38);
      this.tbOpenComic.Text = "&Open Book...";
      this.tbOpenRemoteLibrary.Image = (Image) Resources.RemoteDatabase;
      this.tbOpenRemoteLibrary.Name = "tbOpenRemoteLibrary";
      this.tbOpenRemoteLibrary.ShortcutKeys = Keys.R | Keys.Shift | Keys.Control;
      this.tbOpenRemoteLibrary.Size = new System.Drawing.Size(284, 38);
      this.tbOpenRemoteLibrary.Text = "Open Remote Library...";
      this.tbShowInfo.Image = (Image) Resources.GetInfo;
      this.tbShowInfo.Name = "tbShowInfo";
      this.tbShowInfo.ShortcutKeys = Keys.I | Keys.Control;
      this.tbShowInfo.Size = new System.Drawing.Size(284, 38);
      this.tbShowInfo.Text = "Info...";
      this.tbShowInfo.TextImageRelation = TextImageRelation.TextBeforeImage;
      this.toolStripMenuItem47.Name = "toolStripMenuItem47";
      this.toolStripMenuItem47.Size = new System.Drawing.Size(281, 6);
      this.tsWorkspaces.DropDownItems.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.tsSaveWorkspace,
        (ToolStripItem) this.tsEditWorkspaces,
        (ToolStripItem) this.tsWorkspaceSep
      });
      this.tsWorkspaces.Image = (Image) Resources.Workspace;
      this.tsWorkspaces.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsWorkspaces.Name = "tsWorkspaces";
      this.tsWorkspaces.ShortcutKeys = Keys.W | Keys.Control;
      this.tsWorkspaces.Size = new System.Drawing.Size(284, 38);
      this.tsWorkspaces.Text = "Workspaces";
      this.tsSaveWorkspace.Name = "tsSaveWorkspace";
      this.tsSaveWorkspace.ShortcutKeys = Keys.W | Keys.Control;
      this.tsSaveWorkspace.Size = new System.Drawing.Size(237, 22);
      this.tsSaveWorkspace.Text = "&Save Workspace...";
      this.tsEditWorkspaces.Name = "tsEditWorkspaces";
      this.tsEditWorkspaces.ShortcutKeys = Keys.W | Keys.Control | Keys.Alt;
      this.tsEditWorkspaces.Size = new System.Drawing.Size(237, 22);
      this.tsEditWorkspaces.Text = "&Edit Workspaces...";
      this.tsWorkspaceSep.Name = "tsWorkspaceSep";
      this.tsWorkspaceSep.Size = new System.Drawing.Size(234, 6);
      this.tbBookmarks.DropDownItems.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.tbSetBookmark,
        (ToolStripItem) this.tbRemoveBookmark,
        (ToolStripItem) this.tbBookmarkSeparator
      });
      this.tbBookmarks.Image = (Image) Resources.Bookmark;
      this.tbBookmarks.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbBookmarks.Name = "tbBookmarks";
      this.tbBookmarks.Size = new System.Drawing.Size(284, 38);
      this.tbBookmarks.Text = "Bookmarks";
      this.tbBookmarks.DropDownOpening += new EventHandler(this.tbBookmarks_DropDownOpening);
      this.tbSetBookmark.Image = (Image) Resources.NewBookmark;
      this.tbSetBookmark.Name = "tbSetBookmark";
      this.tbSetBookmark.ShortcutKeys = Keys.B | Keys.Shift | Keys.Control;
      this.tbSetBookmark.Size = new System.Drawing.Size(248, 22);
      this.tbSetBookmark.Text = "Set Bookmark...";
      this.tbRemoveBookmark.Image = (Image) Resources.RemoveBookmark;
      this.tbRemoveBookmark.Name = "tbRemoveBookmark";
      this.tbRemoveBookmark.ShortcutKeys = Keys.D | Keys.Shift | Keys.Control;
      this.tbRemoveBookmark.Size = new System.Drawing.Size(248, 22);
      this.tbRemoveBookmark.Text = "Remove Bookmark";
      this.tbBookmarkSeparator.Name = "tbBookmarkSeparator";
      this.tbBookmarkSeparator.Size = new System.Drawing.Size(245, 6);
      this.tbBookmarkSeparator.Tag = (object) "bms";
      this.tbAutoScroll.Image = (Image) Resources.CursorScroll;
      this.tbAutoScroll.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbAutoScroll.Name = "tbAutoScroll";
      this.tbAutoScroll.ShortcutKeys = Keys.S | Keys.Control;
      this.tbAutoScroll.Size = new System.Drawing.Size(284, 38);
      this.tbAutoScroll.Text = "Auto Scrolling";
      this.toolStripMenuItem45.Name = "toolStripMenuItem45";
      this.toolStripMenuItem45.Size = new System.Drawing.Size(281, 6);
      this.tbMinimalGui.Image = (Image) Resources.MenuToggle;
      this.tbMinimalGui.Name = "tbMinimalGui";
      this.tbMinimalGui.ShortcutKeys = Keys.F10;
      this.tbMinimalGui.Size = new System.Drawing.Size(284, 38);
      this.tbMinimalGui.Text = "Minimal User Interface";
      this.tbReaderUndocked.Image = (Image) Resources.UndockReader;
      this.tbReaderUndocked.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tbReaderUndocked.Name = "tbReaderUndocked";
      this.tbReaderUndocked.ShortcutKeys = Keys.F12;
      this.tbReaderUndocked.Size = new System.Drawing.Size(284, 38);
      this.tbReaderUndocked.Text = "Reader in own Window";
      this.toolStripMenuItem52.Name = "toolStripMenuItem52";
      this.toolStripMenuItem52.Size = new System.Drawing.Size(281, 6);
      this.tbScan.Image = (Image) Resources.Scan;
      this.tbScan.Name = "tbScan";
      this.tbScan.ShortcutKeys = Keys.S | Keys.Shift | Keys.Control;
      this.tbScan.Size = new System.Drawing.Size(284, 38);
      this.tbScan.Text = "Scan Book &Folders";
      this.tbUpdateAllComicFiles.Image = (Image) Resources.UpdateSmall;
      this.tbUpdateAllComicFiles.Name = "tbUpdateAllComicFiles";
      this.tbUpdateAllComicFiles.ShortcutKeys = Keys.U | Keys.Shift | Keys.Control;
      this.tbUpdateAllComicFiles.Size = new System.Drawing.Size(284, 38);
      this.tbUpdateAllComicFiles.Text = "Update all Book Files";
      this.tbUpdateWebComics.Image = (Image) Resources.UpdateWeb;
      this.tbUpdateWebComics.Name = "tbUpdateWebComics";
      this.tbUpdateWebComics.ShortcutKeys = Keys.W | Keys.Shift | Keys.Control;
      this.tbUpdateWebComics.Size = new System.Drawing.Size(284, 38);
      this.tbUpdateWebComics.Text = "Update Web Comics";
      this.tsSynchronizeDevices.Image = (Image) Resources.DeviceSync;
      this.tsSynchronizeDevices.Name = "tsSynchronizeDevices";
      this.tsSynchronizeDevices.Size = new System.Drawing.Size(284, 38);
      this.tsSynchronizeDevices.Text = "Synchronize Devices";
      this.toolStripMenuItem48.Name = "toolStripMenuItem48";
      this.toolStripMenuItem48.Size = new System.Drawing.Size(281, 6);
      this.tbComicDisplaySettings.Image = (Image) Resources.DisplaySettings;
      this.tbComicDisplaySettings.Name = "tbComicDisplaySettings";
      this.tbComicDisplaySettings.ShortcutKeys = Keys.F9;
      this.tbComicDisplaySettings.Size = new System.Drawing.Size(284, 38);
      this.tbComicDisplaySettings.Text = "Book Display Settings...";
      this.tbPreferences.Image = (Image) Resources.Preferences;
      this.tbPreferences.Name = "tbPreferences";
      this.tbPreferences.ShortcutKeys = Keys.F9 | Keys.Control;
      this.tbPreferences.Size = new System.Drawing.Size(284, 38);
      this.tbPreferences.Text = "&Preferences...";
      this.tbSupport.Image = (Image) Resources.Heart;
      this.tbSupport.Name = "tbSupport";
      this.tbSupport.Size = new System.Drawing.Size(284, 38);
      this.tbSupport.Text = "&Support ComicRack...";
      this.tbAbout.Image = (Image) Resources.About;
      this.tbAbout.Name = "tbAbout";
      this.tbAbout.ShortcutKeys = Keys.F1 | Keys.Alt;
      this.tbAbout.Size = new System.Drawing.Size(284, 38);
      this.tbAbout.Text = "&About...";
      this.toolStripMenuItem50.Name = "toolStripMenuItem50";
      this.toolStripMenuItem50.Size = new System.Drawing.Size(281, 6);
      this.tbShowMainMenu.Name = "tbShowMainMenu";
      this.tbShowMainMenu.ShortcutKeys = Keys.F10 | Keys.Shift;
      this.tbShowMainMenu.Size = new System.Drawing.Size(284, 38);
      this.tbShowMainMenu.Text = "Show Main Menu";
      this.toolStripMenuItem51.Name = "toolStripMenuItem51";
      this.toolStripMenuItem51.Size = new System.Drawing.Size(281, 6);
      this.tbExit.Name = "tbExit";
      this.tbExit.ShortcutKeys = Keys.Q | Keys.Control;
      this.tbExit.Size = new System.Drawing.Size(284, 38);
      this.tbExit.Text = "&Exit";
      this.tabContextMenu.Items.AddRange(new ToolStripItem[8]
      {
        (ToolStripItem) this.cmClose,
        (ToolStripItem) this.cmCloseAllButThis,
        (ToolStripItem) this.cmCloseAllToTheRight,
        (ToolStripItem) this.toolStripMenuItem35,
        (ToolStripItem) this.cmSyncBrowser,
        (ToolStripItem) this.sepBeforeRevealInBrowser,
        (ToolStripItem) this.cmRevealInExplorer,
        (ToolStripItem) this.cmCopyPath
      });
      this.tabContextMenu.Name = "tabContextMenu";
      this.tabContextMenu.Size = new System.Drawing.Size(237, 244);
      this.tabContextMenu.Opening += new CancelEventHandler(this.tabContextMenu_Opening);
      this.cmClose.Name = "cmClose";
      this.cmClose.ShortcutKeys = Keys.X | Keys.Control;
      this.cmClose.Size = new System.Drawing.Size(236, 38);
      this.cmClose.Text = "Close";
      this.cmCloseAllButThis.Name = "cmCloseAllButThis";
      this.cmCloseAllButThis.Size = new System.Drawing.Size(236, 38);
      this.cmCloseAllButThis.Text = "Close All But This";
      this.cmCloseAllToTheRight.Name = "cmCloseAllToTheRight";
      this.cmCloseAllToTheRight.Size = new System.Drawing.Size(236, 38);
      this.cmCloseAllToTheRight.Text = "Close All to the Right";
      this.toolStripMenuItem35.Name = "toolStripMenuItem35";
      this.toolStripMenuItem35.Size = new System.Drawing.Size(233, 6);
      this.cmSyncBrowser.Image = (Image) Resources.SyncBrowser;
      this.cmSyncBrowser.Name = "cmSyncBrowser";
      this.cmSyncBrowser.ShortcutKeys = Keys.F3 | Keys.Control;
      this.cmSyncBrowser.Size = new System.Drawing.Size(236, 38);
      this.cmSyncBrowser.Text = "Show in &Browser";
      this.sepBeforeRevealInBrowser.Name = "sepBeforeRevealInBrowser";
      this.sepBeforeRevealInBrowser.Size = new System.Drawing.Size(233, 6);
      this.cmRevealInExplorer.Name = "cmRevealInExplorer";
      this.cmRevealInExplorer.ShortcutKeys = Keys.G | Keys.Control;
      this.cmRevealInExplorer.Size = new System.Drawing.Size(236, 38);
      this.cmRevealInExplorer.Text = "Reveal in Explorer";
      this.cmCopyPath.Name = "cmCopyPath";
      this.cmCopyPath.Size = new System.Drawing.Size(236, 38);
      this.cmCopyPath.Text = "Copy Full Path to Clipboard";
      this.trimTimer.Enabled = true;
      this.trimTimer.Interval = 5000;
      this.trimTimer.Tick += new EventHandler(this.trimTimer_Tick);
      this.mainViewContainer.AutoGripPosition = true;
      this.mainViewContainer.BackColor = SystemColors.Control;
      this.mainViewContainer.Controls.Add((Control) this.mainView);
      this.mainViewContainer.Dock = DockStyle.Bottom;
      this.mainViewContainer.Location = new System.Drawing.Point(0, 388);
      this.mainViewContainer.Name = "mainViewContainer";
      this.mainViewContainer.Size = new System.Drawing.Size(744, 250);
      this.mainViewContainer.TabIndex = 2;
      this.mainViewContainer.ExpandedChanged += new EventHandler(this.mainViewContainer_ExpandedChanged);
      this.mainViewContainer.DockChanged += new EventHandler(this.mainViewContainer_DockChanged);
      this.mainView.BackColor = System.Drawing.Color.Transparent;
      this.mainView.Caption = "";
      this.mainView.CaptionMargin = new Padding(2);
      this.mainView.Dock = DockStyle.Fill;
      this.mainView.InfoPanelRight = false;
      this.mainView.Location = new System.Drawing.Point(0, 6);
      this.mainView.Margin = new Padding(6);
      this.mainView.Name = "mainView";
      this.mainView.Size = new System.Drawing.Size(744, 244);
      this.mainView.TabBarVisible = true;
      this.mainView.TabIndex = 0;
      this.mainView.TabChanged += new EventHandler(this.mainView_TabChanged);
      this.updateActivityTimer.Enabled = true;
      this.updateActivityTimer.Interval = 1000;
      this.updateActivityTimer.Tick += new EventHandler(this.UpdateActivityTimerTick);
      this.AutoScaleMode = AutoScaleMode.None;
      this.ClientSize = new System.Drawing.Size(744, 662);
      this.Controls.Add((Control) this.viewContainer);
      this.Controls.Add((Control) this.mainViewContainer);
      this.Controls.Add((Control) this.statusStrip);
      this.Controls.Add((Control) this.mainMenuStrip);
      this.KeyPreview = true;
      this.Name = nameof (MainForm);
      this.Text = "ComicRack";
      this.mainMenuStrip.ResumeLayout(false);
      this.mainMenuStrip.PerformLayout();
      this.contextRating.ResumeLayout(false);
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.pageContextMenu.ResumeLayout(false);
      this.contextRating2.ResumeLayout(false);
      this.notfifyContextMenu.ResumeLayout(false);
      this.viewContainer.ResumeLayout(false);
      this.panelReader.ResumeLayout(false);
      this.readerContainer.ResumeLayout(false);
      this.readerContainer.PerformLayout();
      this.fileTabs.ResumeLayout(false);
      this.fileTabs.PerformLayout();
      this.mainToolStrip.ResumeLayout(false);
      this.mainToolStrip.PerformLayout();
      this.toolsContextMenu.ResumeLayout(false);
      this.tabContextMenu.ResumeLayout(false);
      this.mainViewContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    [SpecialName]
    string IApplication.get_ProductVersion() => this.ProductVersion;

    public class ComicReaderTab : TabBar.TabBarItem
    {
      private readonly ComicBookNavigator nav;
      private readonly Font font;
      private readonly string shortcut;

      public ComicReaderTab(string text, ComicBookNavigator nav, Font font, string shortcut)
        : base(text)
      {
        this.nav = nav;
        this.font = font;
        this.shortcut = shortcut;
        this.ToolTipSize = new System.Drawing.Size(400, 200).ScaleDpi();
      }

      public override bool ShowToolTip() => this.nav != null;

      public override void DrawTooltip(Graphics gr, Rectangle rc)
      {
        base.DrawTooltip(gr, rc);
        if (this.nav == null)
          return;
        try
        {
          ComicBook comic = this.nav.Comic;
          using (IItemLock<ThumbnailImage> thumbnail = Program.ImagePool.GetThumbnail(comic.GetFrontCoverThumbnailKey(), (IImageProvider) this.nav, false))
          {
            rc.Inflate(-10, -5);
            if (!string.IsNullOrEmpty(this.shortcut))
            {
              using (Brush brush = (Brush) new SolidBrush(System.Drawing.Color.FromArgb(128, SystemColors.InfoText)))
              {
                using (StringFormat format = new StringFormat()
                {
                  Alignment = StringAlignment.Far,
                  LineAlignment = StringAlignment.Far
                })
                  gr.DrawString(this.shortcut, FC.Get(this.font, 6f), brush, (RectangleF) rc, format);
              }
              rc.Height -= 10;
            }
            ThumbTileRenderer.DrawTile(gr, rc, (Image) thumbnail.Item.GetThumbnail(rc.Height), comic, this.font, SystemColors.InfoText, System.Drawing.Color.Transparent, ThumbnailDrawingOptions.DefaultWithoutBackground, ComicTextElements.DefaultComic, false, comic.GetIcons());
          }
        }
        catch
        {
        }
      }
    }

    public class RatingEditor : IEditRating
    {
      private IEnumerable<ComicBook> books;
      private IWin32Window parent;

      public RatingEditor(IWin32Window parent, IEnumerable<ComicBook> books)
      {
        this.parent = parent;
        this.books = books;
      }

      public bool IsValid() => this.books != null && !this.books.IsEmpty<ComicBook>();

      public void SetRating(float rating)
      {
        if (!this.IsValid())
          return;
        Program.Database.Undo.SetMarker(TR.Messages["UndoRating", "Change Rating"]);
        this.books.ForEach<ComicBook>((Action<ComicBook>) (cb => cb.Rating = rating));
      }

      public float GetRating()
      {
        float rating = -1f;
        if (this.IsValid())
        {
          foreach (ComicBook book in this.books)
          {
            if ((double) rating == -1.0)
              rating = book.Rating;
            else if ((double) rating != (double) book.Rating)
            {
              rating = -1f;
              break;
            }
          }
        }
        return rating;
      }

      public bool QuickRatingAndReview()
      {
        Program.Database.Undo.SetMarker(TR.Messages["QuickRating", "Quick Rating"]);
        return QuickRatingDialog.Show(this.parent, this.books.FirstOrDefault<ComicBook>());
      }
    }

    public class PageEditorWrapper : IEditPage
    {
      private IEditPage editor;

      public PageEditorWrapper(IEditPage editor) => this.editor = editor;

      public bool IsValid => this.editor != null && this.editor.IsValid;

      public ComicPageType PageType
      {
        get => !this.IsValid ? ComicPageType.Story : this.editor.PageType;
        set
        {
          if (!this.IsValid)
            return;
          this.editor.PageType = value;
        }
      }

      public ImageRotation Rotation
      {
        get => !this.IsValid ? ImageRotation.None : this.editor.Rotation;
        set
        {
          if (!this.IsValid)
            return;
          this.editor.Rotation = value;
        }
      }
    }

    public class BookmarkEditorWrapper : IEditBookmark
    {
      private IEditBookmark editor;

      public BookmarkEditorWrapper(IEditBookmark editor) => this.editor = editor;

      public bool CanBookmark => this.editor != null && this.editor.CanBookmark;

      public string BookmarkProposal
      {
        get => !this.CanBookmark ? string.Empty : this.editor.BookmarkProposal;
      }

      public string Bookmark
      {
        get => !this.CanBookmark ? string.Empty : this.editor.Bookmark;
        set
        {
          if (!this.CanBookmark)
            return;
          this.editor.Bookmark = value;
        }
      }
    }
  }
}

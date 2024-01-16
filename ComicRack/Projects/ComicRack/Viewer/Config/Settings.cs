// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.Settings
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Cryptography;
using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using cYo.Common.Threading;
using cYo.Common.Xml;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Network;
using cYo.Projects.ComicRack.Engine.Sync;
using cYo.Projects.ComicRack.Viewer.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  [Serializable]
  public class Settings : ICacheSettings, IComicUpdateSettings, ISharesSettings
  {
    public const int RecentFileCount = 20;
    public const int MinimumMemoryPageCacheCount = 20;
    public const int MaximumMemoryPageCacheCount = 100;
    public const int DefaultMemoryPageCacheCount = 25;
    public const int MinimumMemoryThumbnailCacheMB = 5;
    public const int MaximumMemoryThumbnailCacheMB = 100;
    public const int DefaultMemoryThumbnailCacheMB = 50;
    public const int DefaultInternetCacheSizeMB = 1000;
    public const string DefaultHelpSystem = "ComicRack Wiki";
    public const int MinimumSystemMemory = 64;
    public const int UnlimitedSystemMemory = 1024;
    private List<ListConfiguration> listConfigurations = new List<ListConfiguration>();
    private DisplayWorkspace currentWorkspace = new DisplayWorkspace();
    private readonly List<DisplayWorkspace> workspaces = new List<DisplayWorkspace>();
    private string pasteProperties = "Series";
    private ComicPageType pageFilter = ComicPageType.All;
    private string lastExplorerFolder = string.Empty;
    private Guid lastLibraryItem = Guid.Empty;
    private int lastOpenFilterIndex = 1;
    private int lastSaveFilterIndex = 1;
    private int lastExportPageFilterIndex = 1;
    private readonly SmartList<string> favoriteFolders = new SmartList<string>();
    private readonly MruList<RemoteShareItem> remoteShares = new MruList<RemoteShareItem>();
    private readonly SmartList<ComicLibraryServerConfig> shares = new SmartList<ComicLibraryServerConfig>();
    private bool lookForShared = true;
    private bool autoConnectShares = true;
    private readonly SmartList<Settings.PasswordCacheEntry> passwordCache = new SmartList<Settings.PasswordCacheEntry>();
    private string extraWifiDeviceAddresses = string.Empty;
    private ImageDisplayOptions pageImageDisplayOptions = ImageDisplayOptions.HighQuality;
    private volatile int overlayScaling = 100;
    private BitmapAdjustment globalColorAdjustment = BitmapAdjustment.Empty;
    private Size magnifySize = new Size(300, 200);
    private float magnifyOpaque = 1f;
    private float magnifyZoom = 2f;
    private MagnifierStyle magnifyStyle;
    private bool autoMagnifiery = true;
    private bool hardwareAcceleration = true;
    private bool displayChangeAnimation = true;
    private bool flowingMouseScrolling = true;
    private bool softwareFiltering = true;
    private bool hardwareFiltering;
    private float mouseWheelSpeed = 2f;
    private readonly List<StringPair> readerKeyboardMapping = new List<StringPair>();
    private string ignoredCoverImages;
    private bool autoScrolling;
    private HiddenMessageBoxes hiddenMessageBoxes;
    private bool updateComicFiles;
    private bool autoUpdateComicsFiles;
    private string helpSystem = "ComicRack Wiki";
    private bool scripting = true;
    private string scriptingLibraries = string.Empty;
    private bool showSplash = true;
    private bool openLastFile = true;
    private bool scanStartup;
    private bool updateWebComicsStartup;
    private bool newsStartup = true;
    private readonly List<string> lastOpenFiles = new List<string>();
    private bool openLastPage = true;
    private bool closeBrowserOnOpen;
    private bool addToLibraryOnOpen;
    private bool openInNewTab;
    private bool hideCursorFullScreen = true;
    private bool autoNavigateComics = true;
    private bool showCurrentPageOverlay = true;
    private bool showVisiblePagePartOverlay = true;
    private bool showStatusOverlay = true;
    private bool showNavigationOverlay = true;
    private bool navigationOverlayOnTop;
    private bool currentPageShowsName;
    private bool autoHideMagnifier = true;
    private bool pageChangeDelay = true;
    private bool scrollingDoesBrowse = true;
    private bool resetZoomOnPageChange;
    private bool zoomInOutOnPageChange = true;
    private bool smoothScrolling = true;
    private bool blendWhilePaging;
    private bool trackCurrentPage = true;
    private RightToLeftReadingMode rightToLeftReadingMode = RightToLeftReadingMode.FlipPages;
    private bool leftRightMovementReversed;
    private bool showToolTips;
    private bool showSearchLinks = true;
    private bool fadeInThumbnails = true;
    private bool dogEarThumbnails = true;
    private bool numericRatingThumbnails = true;
    private bool localQuickSearch = true;
    private bool coverThumbnailsSameSize;
    private bool commonListStackLayout;
    private bool showQuickOpen = true;
    private bool catalogOnlyForFileless = true;
    private bool showCustomBookFields;
    private bool minimizeToTray;
    private volatile bool closeMinimizeToTray = true;
    private volatile bool autoMinimalGui;
    private volatile bool animatePanels = true;
    private volatile bool alwaysDisplayBrowserDockingGrip;
    private bool autoHideMainMenu = true;
    private bool showMainMenuNoComicOpen = true;
    private bool informationCover3D = true;
    private bool displayLibraryGauges = true;
    private LibraryGauges libraryGaugesFormat = LibraryGauges.Default;
    private bool newBooksChecked = true;
    private bool thumbCacheEnabled = true;
    private int thumbCacheSizeMB = 500;
    private bool pageCacheEnabled = true;
    private int pageCacheSizeMB = 500;
    private bool internetCacheEnabled = true;
    private int internetCacheSizeMB = 1000;
    private int memoryThumbCacheSizeMB = 50;
    private int memoryPageCacheCount = 25;
    private bool memoryThumbCacheOptimized = true;
    private bool memoryPageCacheOptimized = true;
    private bool removeMissingFilesOnFullScan;
    private bool dontAddRemoveFiles;
    private bool overwriteAssociations;
    private readonly List<Settings.RemoteViewConfig> remoteViewConfigList = new List<Settings.RemoteViewConfig>();
    private readonly List<Settings.RemoteExplorerViewSettings> remoteExplorerViewSettingsList = new List<Settings.RemoteExplorerViewSettings>();
    private readonly List<string> quickSearchList = new List<string>();
    private readonly List<string> libraryQuickSearchList = new List<string>();
    private readonly MruList<string> keyboardLayouts = new MruList<string>();
    private readonly MruList<string> thumbnailFiles = new MruList<string>();
    private readonly ExportSettingCollection exportUserPresets = new ExportSettingCollection();
    private readonly SmartList<DeviceSyncSettings> devices = new SmartList<DeviceSyncSettings>();

    public Settings()
    {
      this.ExternalServerAddress = string.Empty;
      this.PrivateListingPassword = string.Empty;
      this.QuickOpenThumbnailSize = 128;
      this.MaximumMemoryMB = 1024;
      this.ShowQuickManual = true;
      this.ValidationDate = DateTime.MinValue;
    }

    public List<ListConfiguration> ListConfigurations
    {
      get => this.listConfigurations;
      set => this.listConfigurations = value;
    }

    public DisplayWorkspace CurrentWorkspace
    {
      get => this.currentWorkspace;
      set => this.currentWorkspace = value;
    }

    public List<DisplayWorkspace> Workspaces => this.workspaces;

    public DisplayWorkspace GetWorkspace(string name)
    {
      return string.IsNullOrEmpty(name) ? (DisplayWorkspace) null : this.workspaces.FirstOrDefault<DisplayWorkspace>((Func<DisplayWorkspace, bool>) (ws => string.Equals(ws.Name, name, StringComparison.OrdinalIgnoreCase)));
    }

    [Browsable(false)]
    [DefaultValue(0)]
    public int RunCount { get; set; }

    [Browsable(false)]
    [DefaultValue("")]
    public string PasteProperties
    {
      get => this.pasteProperties;
      set => this.pasteProperties = value;
    }

    [Browsable(false)]
    [DefaultValue(null)]
    public string SelectedBrowser { get; set; }

    [DefaultValue(ComicPageType.All)]
    [Browsable(false)]
    public ComicPageType PageFilter
    {
      get => this.pageFilter;
      set => this.pageFilter = value;
    }

    [Browsable(false)]
    [DefaultValue("")]
    public string LastExplorerFolder
    {
      get => this.lastExplorerFolder;
      set => this.lastExplorerFolder = value;
    }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool ExplorerIncludeSubFolders { get; set; }

    [Browsable(false)]
    [DefaultValue("")]
    public Guid LastLibraryItem
    {
      get => this.lastLibraryItem;
      set => this.lastLibraryItem = value;
    }

    [Browsable(false)]
    [DefaultValue(-1)]
    public int LastOpenFilterIndex
    {
      get => this.lastOpenFilterIndex;
      set => this.lastOpenFilterIndex = value;
    }

    [Browsable(false)]
    [DefaultValue(1)]
    public int LastSaveFilterIndex
    {
      get => this.lastSaveFilterIndex;
      set => this.lastSaveFilterIndex = value;
    }

    [Browsable(false)]
    [DefaultValue(1)]
    public int LastExportPageFilterIndex
    {
      get => this.lastExportPageFilterIndex;
      set => this.lastExportPageFilterIndex = value;
    }

    [Browsable(false)]
    public SmartList<string> FavoriteFolders => this.favoriteFolders;

    [Browsable(false)]
    [XmlArrayItem("Share")]
    public MruList<RemoteShareItem> RemoteShares => this.remoteShares;

    [Browsable(false)]
    [DefaultValue(null)]
    public string PluginsStates { get; set; }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool MoveFilesToRecycleBin { get; set; }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool AlsoRemoveFromLibrary { get; set; }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool AlsoRemoveFromLibraryFiltered { get; set; }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool RemoveFilesfromDatabase { get; set; }

    [Browsable(false)]
    [DefaultValue(TabLayouts.None)]
    public TabLayouts TabLayouts { get; set; }

    [DefaultValue(128)]
    public int QuickOpenThumbnailSize { get; set; }

    [field: NonSerialized]
    public event EventHandler SettingsChanged;

    [field: NonSerialized]
    public event EventHandler CacheSettingsChanged;

    [field: NonSerialized]
    public event EventHandler PageImageDisplayOptionsChanged;

    [field: NonSerialized]
    public event EventHandler FadeInThumbnailsChanged;

    [field: NonSerialized]
    public event EventHandler DogEarThumbnailsChanged;

    [field: NonSerialized]
    public event EventHandler NumericRatingThumbnailsChanged;

    [field: NonSerialized]
    public event EventHandler CoverThumbnailsSameSizeChanged;

    [field: NonSerialized]
    public event EventHandler CommonListStackLayoutChanged;

    [field: NonSerialized]
    public event EventHandler LocalQuickSearchChanged;

    [field: NonSerialized]
    public event EventHandler AlwaysDisplayBrowserDockingGripChanged;

    [field: NonSerialized]
    public event EventHandler ShowOverlaysChanged;

    [field: NonSerialized]
    public event EventHandler OverlayScalingChanged;

    [field: NonSerialized]
    public event EventHandler HideCursorFullScreenChanged;

    [field: NonSerialized]
    public event EventHandler ShowSplashChanged;

    [field: NonSerialized]
    public event EventHandler OpenLastFileChanged;

    [field: NonSerialized]
    public event EventHandler OpenLastPageChanged;

    [field: NonSerialized]
    public event EventHandler ScanStartupChanged;

    [field: NonSerialized]
    public event EventHandler CheckWebComicsStartupChanged;

    [field: NonSerialized]
    public event EventHandler ScanOptionsChanged;

    [field: NonSerialized]
    public event EventHandler OverwriteAssociationsChanged;

    [field: NonSerialized]
    public event EventHandler ShowToolTipsChanged;

    [field: NonSerialized]
    public event EventHandler ShowSearchLinksChanged;

    [field: NonSerialized]
    public event EventHandler LookForSharedChanged;

    [field: NonSerialized]
    public event EventHandler AutoConnectSharesChanged;

    [field: NonSerialized]
    public event EventHandler MinimizeToTrayChanged;

    [field: NonSerialized]
    public event EventHandler CatalogOnlyForFilelessChanged;

    [field: NonSerialized]
    public event EventHandler ShowCustomBookFieldsChanged;

    [field: NonSerialized]
    public event EventHandler ShowQuickOpenChanged;

    [field: NonSerialized]
    public event EventHandler CloseMinimizesToTrayChanged;

    [field: NonSerialized]
    public event EventHandler CloseBrowserOnOpenChanged;

    [field: NonSerialized]
    public event EventHandler AddToLibraryOnOpenChanged;

    [field: NonSerialized]
    public event EventHandler OpenInNewTabChanged;

    [field: NonSerialized]
    public event EventHandler AutoUpdateComicFilesChanged;

    [field: NonSerialized]
    public event EventHandler UpdateComicFilesChanged;

    [field: NonSerialized]
    public event EventHandler BlendWhilePagingChanged;

    [field: NonSerialized]
    public event EventHandler TrackCurrentPageChanged;

    [field: NonSerialized]
    public event EventHandler AutoNavigateComicsChanged;

    [field: NonSerialized]
    public event EventHandler NewsStartupChanged;

    [field: NonSerialized]
    public event EventHandler AutoScrollingChanged;

    [field: NonSerialized]
    public event EventHandler ColorAdjustmentChanged;

    [field: NonSerialized]
    public event EventHandler IgnoredCoverImagesChanged;

    [field: NonSerialized]
    public event EventHandler ScriptingChanged;

    [field: NonSerialized]
    public event EventHandler ScriptingLibrariesChanged;

    [field: NonSerialized]
    public event EventHandler MagnifySizeChanged;

    [field: NonSerialized]
    public event EventHandler MagnifyOpaqueChanged;

    [field: NonSerialized]
    public event EventHandler MagnifyZoomChanged;

    [field: NonSerialized]
    public event EventHandler MagnifyStyleChanged;

    [field: NonSerialized]
    public event EventHandler AutoMagnifierChanged;

    [field: NonSerialized]
    public event EventHandler AnimatePanelsChanged;

    [field: NonSerialized]
    public event EventHandler AutoHideMagnifierChanged;

    [field: NonSerialized]
    public event EventHandler AutoMinimalGuiChanged;

    [field: NonSerialized]
    public event EventHandler PageChangeDelayChanged;

    [field: NonSerialized]
    public event EventHandler ScrollingDoesBrowseChanged;

    [field: NonSerialized]
    public event EventHandler ResetZoomOnPageChangeChanged;

    [field: NonSerialized]
    public event EventHandler ZoomInOutOnPageChangeChanged;

    [field: NonSerialized]
    public event EventHandler SmoothScrollingChanged;

    [field: NonSerialized]
    public event EventHandler LeftRightMovementReversedChanged;

    [field: NonSerialized]
    public event EventHandler RightToLeftReadingModeChanged;

    [field: NonSerialized]
    public event EventHandler HardwareAccelerationChanged;

    [field: NonSerialized]
    public event EventHandler DisplayChangeAnimationChanged;

    [field: NonSerialized]
    public event EventHandler FlowingMouseScrollingChanged;

    [field: NonSerialized]
    public event EventHandler SoftwareFilteringChanged;

    [field: NonSerialized]
    public event EventHandler HardwareFilteringChanged;

    [field: NonSerialized]
    public event EventHandler MouseWheelSpeedChanged;

    [field: NonSerialized]
    public event EventHandler AutoHideMainMenuChanged;

    [field: NonSerialized]
    public event EventHandler ShowMainMenuNoComicOpenChanged;

    [field: NonSerialized]
    public event EventHandler InformationCover3DChanged;

    [field: NonSerialized]
    public event EventHandler DisplayLibraryGaugesChanged;

    [field: NonSerialized]
    public event EventHandler HelpSystemChanged;

    [field: NonSerialized]
    public event EventHandler NewBooksCheckedChanged;

    [field: NonSerialized]
    public event EventHandler ExtraWirelessIpAddressesChanged;

    public SmartList<ComicLibraryServerConfig> Shares => this.shares;

    public bool IsSharing
    {
      get
      {
        return this.shares.Any<ComicLibraryServerConfig>((Func<ComicLibraryServerConfig, bool>) (sc => sc.IsValidShare));
      }
    }

    [DefaultValue("")]
    public string ExternalServerAddress { get; set; }

    [DefaultValue("")]
    public string PrivateListingPassword { get; set; }

    [Category("Network")]
    [Description("Look for locally shared comic libraries on the network")]
    [DefaultValue(true)]
    [Browsable(false)]
    public bool LookForShared
    {
      get => this.lookForShared;
      set
      {
        if (this.lookForShared == value)
          return;
        this.lookForShared = value;
        this.FireEvent(this.LookForSharedChanged);
      }
    }

    [Category("Network")]
    [Description("Autoconnect shares")]
    [DefaultValue(true)]
    [Browsable(false)]
    public bool AutoConnectShares
    {
      get => this.autoConnectShares;
      set
      {
        if (this.autoConnectShares == value)
          return;
        this.autoConnectShares = value;
        this.FireEvent(this.AutoConnectSharesChanged);
      }
    }

    [Category("Network")]
    [Description("cache for remote passwords")]
    [XmlArrayItem("Item")]
    public SmartList<Settings.PasswordCacheEntry> PasswordCache => this.passwordCache;

    public void AddPasswordToCache(string remote, string password)
    {
      int hash = remote.GetHashCode();
      Settings.PasswordCacheEntry passwordCacheEntry = this.passwordCache.Find((Predicate<Settings.PasswordCacheEntry>) (e => e.RemoteId == hash));
      if (passwordCacheEntry == null)
        this.passwordCache.Add(new Settings.PasswordCacheEntry(remote, password));
      else
        passwordCacheEntry.Password = password;
    }

    public string GetPasswordFromCache(string remote)
    {
      int hash = remote.GetHashCode();
      Settings.PasswordCacheEntry passwordCacheEntry = this.passwordCache.Find((Predicate<Settings.PasswordCacheEntry>) (e => e.RemoteId == hash));
      return passwordCacheEntry == null ? string.Empty : passwordCacheEntry.Password;
    }

    [DefaultValue("")]
    public string ExtraWifiDeviceAddresses
    {
      get => this.extraWifiDeviceAddresses;
      set
      {
        if (this.extraWifiDeviceAddresses == value)
          return;
        this.extraWifiDeviceAddresses = value;
        this.FireEvent(this.ExtraWirelessIpAddressesChanged);
      }
    }

    [Category("Display")]
    [Description("Set how the single pages are rendered")]
    [DefaultValue(ImageDisplayOptions.HighQuality)]
    [XmlElement("PageImageOptions")]
    public ImageDisplayOptions PageImageDisplayOptions
    {
      get => this.pageImageDisplayOptions;
      set
      {
        if (this.pageImageDisplayOptions == value)
          return;
        this.pageImageDisplayOptions = value;
        this.FireEvent(this.PageImageDisplayOptionsChanged);
      }
    }

    [Category("Display")]
    [Description("Scaling of the overlays")]
    [DefaultValue(100)]
    public int OverlayScaling
    {
      get => this.overlayScaling;
      set
      {
        if (this.overlayScaling == value)
          return;
        this.overlayScaling = value;
        this.FireEvent(this.OverlayScalingChanged);
      }
    }

    [DefaultValue(typeof (BitmapAdjustment), "0, 0, 0")]
    public BitmapAdjustment GlobalColorAdjustment
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.globalColorAdjustment;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.globalColorAdjustment == value)
            return;
          this.globalColorAdjustment = value;
        }
        this.FireEvent(this.ColorAdjustmentChanged);
      }
    }

    [DefaultValue(typeof (Size), "300, 200")]
    public Size MagnifySize
    {
      get => this.magnifySize;
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.magnifySize == value)
            return;
          this.magnifySize = value;
        }
        this.FireEvent(this.MagnifySizeChanged);
      }
    }

    [DefaultValue(1f)]
    public float MagnifyOpaque
    {
      get => this.magnifyOpaque;
      set
      {
        if ((double) this.magnifyOpaque == (double) value)
          return;
        this.magnifyOpaque = value;
        this.FireEvent(this.MagnifyOpaqueChanged);
      }
    }

    [DefaultValue(2f)]
    public float MagnifyZoom
    {
      get => this.magnifyZoom;
      set
      {
        if ((double) this.magnifyZoom == (double) value)
          return;
        this.magnifyZoom = value;
        this.FireEvent(this.MagnifyZoomChanged);
      }
    }

    [DefaultValue(MagnifierStyle.Glass)]
    public MagnifierStyle MagnifyStyle
    {
      get => this.magnifyStyle;
      set
      {
        if (this.magnifyStyle == value)
          return;
        this.magnifyStyle = value;
        this.FireEvent(this.MagnifyStyleChanged);
      }
    }

    [DefaultValue(true)]
    public bool AutoMagnifier
    {
      get => this.autoMagnifiery;
      set
      {
        if (this.autoMagnifiery == value)
          return;
        this.autoMagnifiery = value;
        this.FireEvent(this.AutoMagnifierChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool HardwareAcceleration
    {
      get => this.hardwareAcceleration;
      set
      {
        if (this.hardwareAcceleration == value)
          return;
        this.hardwareAcceleration = value;
        this.FireEvent(this.HardwareAccelerationChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool DisplayChangeAnimation
    {
      get => this.displayChangeAnimation;
      set
      {
        if (this.displayChangeAnimation == value)
          return;
        this.displayChangeAnimation = value;
        this.FireEvent(this.DisplayChangeAnimationChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool FlowingMouseScrolling
    {
      get => this.flowingMouseScrolling;
      set
      {
        if (this.flowingMouseScrolling == value)
          return;
        this.flowingMouseScrolling = value;
        this.FireEvent(this.FlowingMouseScrollingChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool SoftwareFiltering
    {
      get => this.softwareFiltering;
      set
      {
        if (this.softwareFiltering == value)
          return;
        this.softwareFiltering = value;
        this.FireEvent(this.SoftwareFilteringChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool HardwareFiltering
    {
      get => this.hardwareFiltering;
      set
      {
        if (this.hardwareFiltering == value)
          return;
        this.hardwareFiltering = value;
        this.FireEvent(this.HardwareFilteringChanged);
      }
    }

    [Category("Behavior")]
    [Description("Lines per mouse scrolling")]
    [DefaultValue(2f)]
    public float MouseWheelSpeed
    {
      get => this.mouseWheelSpeed;
      set
      {
        if ((double) this.mouseWheelSpeed == (double) value)
          return;
        this.mouseWheelSpeed = value;
        this.FireEvent(this.MouseWheelSpeedChanged);
      }
    }

    [Category("Behavior")]
    [Description("Shortcuts for the reader")]
    [XmlArray("ReaderKeyboardV3")]
    [XmlArrayItem("Action")]
    public List<StringPair> ReaderKeyboardMapping => this.readerKeyboardMapping;

    [Category("Behavior")]
    [Description("Images not to use as cover images")]
    [DefaultValue(null)]
    public string IgnoredCoverImages
    {
      get => this.ignoredCoverImages;
      set
      {
        if (this.ignoredCoverImages == value)
          return;
        this.ignoredCoverImages = value;
        this.FireEvent(this.IgnoredCoverImagesChanged);
      }
    }

    [Category("Behavior")]
    [Description("Turns autoscrolling on")]
    [DefaultValue(false)]
    [Browsable(false)]
    public bool AutoScrolling
    {
      get => this.autoScrolling;
      set
      {
        if (this.autoScrolling == value)
          return;
        this.autoScrolling = value;
        this.FireEvent(this.AutoScrollingChanged);
      }
    }

    [Category("Behavior")]
    [Description("Turned off message boxes")]
    [DefaultValue(HiddenMessageBoxes.None)]
    public HiddenMessageBoxes HiddenMessageBoxes
    {
      get => this.hiddenMessageBoxes;
      set => this.hiddenMessageBoxes = value;
    }

    [Category("Behavior")]
    [Description("Update Book Files with new information")]
    [DefaultValue(false)]
    [Browsable(false)]
    public bool UpdateComicFiles
    {
      get => this.updateComicFiles;
      set
      {
        if (this.updateComicFiles == value)
          return;
        this.updateComicFiles = value;
        this.FireEvent(this.UpdateComicFilesChanged);
      }
    }

    [Category("Behavior")]
    [Description("Auto update of Book files")]
    [DefaultValue(false)]
    [Browsable(false)]
    public bool AutoUpdateComicsFiles
    {
      get => this.autoUpdateComicsFiles;
      set
      {
        if (this.autoUpdateComicsFiles == value)
          return;
        this.autoUpdateComicsFiles = value;
        this.FireEvent(this.AutoUpdateComicFilesChanged);
      }
    }

    [DefaultValue("ComicRack Wiki")]
    public string HelpSystem
    {
      get => this.helpSystem;
      set
      {
        if (this.helpSystem == value)
          return;
        this.helpSystem = value;
        this.FireEvent(this.HelpSystemChanged);
      }
    }

    [Category("Scripting")]
    [Description("Enable or disable Scripting")]
    [Browsable(false)]
    [DefaultValue(true)]
    public bool Scripting
    {
      get => this.scripting;
      set
      {
        if (this.scripting == value)
          return;
        this.scripting = value;
        this.FireEvent(this.ScriptingChanged);
      }
    }

    [Category("Scripting")]
    [Description("Enable or disable Scripting")]
    [DefaultValue("")]
    public string ScriptingLibraries
    {
      get => this.scriptingLibraries;
      set
      {
        if (this.scriptingLibraries == value)
          return;
        this.scriptingLibraries = value;
        this.FireEvent(this.ScriptingLibrariesChanged);
      }
    }

    [DefaultValue(false)]
    public bool HideSampleScripts { get; set; }

    [Category("Starting ComicRack")]
    [Description("Show Splash Screen")]
    [DefaultValue(true)]
    public bool ShowSplash
    {
      get => this.showSplash;
      set
      {
        if (this.showSplash == value)
          return;
        this.showSplash = value;
        this.FireEvent(this.ShowSplashChanged);
      }
    }

    [Category("Starting ComicRack")]
    [Description("Reopen Books from last session")]
    [DefaultValue(true)]
    public bool OpenLastFile
    {
      get => this.openLastFile;
      set
      {
        if (this.openLastFile == value)
          return;
        this.openLastFile = value;
        this.FireEvent(this.OpenLastFileChanged);
      }
    }

    [Category("Starting ComicRack")]
    [Description("Rescan the Book Folders for new Books")]
    [DefaultValue(false)]
    public bool ScanStartup
    {
      get => this.scanStartup;
      set
      {
        if (this.scanStartup == value)
          return;
        this.scanStartup = value;
        this.FireEvent(this.ScanStartupChanged);
      }
    }

    [Category("Starting ComicRack")]
    [Description("Update Web Comics")]
    [DefaultValue(false)]
    public bool UpdateWebComicsStartup
    {
      get => this.updateWebComicsStartup;
      set
      {
        if (this.updateWebComicsStartup == value)
          return;
        this.updateWebComicsStartup = value;
        this.FireEvent(this.CheckWebComicsStartupChanged);
      }
    }

    [Category("Starting ComicRack")]
    [Description("Check for latest news on ComicRack")]
    [DefaultValue(true)]
    public bool NewsStartup
    {
      get => this.newsStartup;
      set
      {
        if (this.newsStartup == value)
          return;
        this.newsStartup = value;
        this.FireEvent(this.NewsStartupChanged);
      }
    }

    [Category("Starting ComicRack")]
    [Description("Lasts file opened")]
    public List<string> LastOpenFiles => this.lastOpenFiles;

    [DefaultValue(true)]
    public bool ShowQuickManual { get; set; }

    [Category("Opening a Book")]
    [Description("Open the Book at the page where it was closed")]
    [DefaultValue(true)]
    public bool OpenLastPage
    {
      get => this.openLastPage;
      set
      {
        if (this.openLastPage == value)
          return;
        this.openLastPage = value;
        this.FireEvent(this.OpenLastPageChanged);
      }
    }

    [Category("Opening a Book")]
    [Description("Close the Browser when a new Book is opened")]
    [DefaultValue(false)]
    public bool CloseBrowserOnOpen
    {
      get => this.closeBrowserOnOpen;
      set
      {
        if (this.closeBrowserOnOpen == value)
          return;
        this.closeBrowserOnOpen = value;
        this.FireEvent(this.CloseBrowserOnOpenChanged);
      }
    }

    [Category("Opening a Book")]
    [Description("Opened Files are added to the Library")]
    [DefaultValue(false)]
    public bool AddToLibraryOnOpen
    {
      get => this.addToLibraryOnOpen;
      set
      {
        if (this.addToLibraryOnOpen == value)
          return;
        this.addToLibraryOnOpen = value;
        this.FireEvent(this.AddToLibraryOnOpenChanged);
      }
    }

    [Category("Opening a Book")]
    [Description("Open in new Tab")]
    [DefaultValue(false)]
    public bool OpenInNewTab
    {
      get => this.openInNewTab;
      set
      {
        if (this.openInNewTab == value)
          return;
        this.openInNewTab = value;
        this.FireEvent(this.OpenInNewTabChanged);
      }
    }

    [Category("Reading")]
    [Description("Hide the mouse cursor when reading in Full Screen Mode")]
    [DefaultValue(true)]
    public bool HideCursorFullScreen
    {
      get => this.hideCursorFullScreen;
      set
      {
        if (this.hideCursorFullScreen == value)
          return;
        this.hideCursorFullScreen = value;
        this.FireEvent(this.HideCursorFullScreenChanged);
      }
    }

    [Category("Reading")]
    [Description("Reading beyond the start or end opens the next Book")]
    [DefaultValue(true)]
    public bool AutoNavigateComics
    {
      get => this.autoNavigateComics;
      set
      {
        if (this.autoNavigateComics == value)
          return;
        this.autoNavigateComics = value;
        this.FireEvent(this.AutoNavigateComicsChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool ShowCurrentPageOverlay
    {
      get => this.showCurrentPageOverlay;
      set
      {
        if (this.showCurrentPageOverlay == value)
          return;
        this.showCurrentPageOverlay = value;
        this.FireEvent(this.ShowOverlaysChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool ShowVisiblePagePartOverlay
    {
      get => this.showVisiblePagePartOverlay;
      set
      {
        if (this.showVisiblePagePartOverlay == value)
          return;
        this.showVisiblePagePartOverlay = value;
        this.FireEvent(this.ShowOverlaysChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool ShowStatusOverlay
    {
      get => this.showStatusOverlay;
      set
      {
        if (this.showStatusOverlay == value)
          return;
        this.showStatusOverlay = value;
        this.FireEvent(this.ShowOverlaysChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool ShowNavigationOverlay
    {
      get => this.showNavigationOverlay;
      set
      {
        if (this.showNavigationOverlay == value)
          return;
        this.showNavigationOverlay = value;
        this.FireEvent(this.ShowOverlaysChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool NavigationOverlayOnTop
    {
      get => this.navigationOverlayOnTop;
      set
      {
        if (this.navigationOverlayOnTop == value)
          return;
        this.navigationOverlayOnTop = value;
        this.FireEvent(this.ShowOverlaysChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool CurrentPageShowsName
    {
      get => this.currentPageShowsName;
      set
      {
        if (this.currentPageShowsName == value)
          return;
        this.currentPageShowsName = value;
        this.FireEvent(this.ShowOverlaysChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool AutoHideMagnifier
    {
      get => this.autoHideMagnifier;
      set
      {
        if (this.autoHideMagnifier == value)
          return;
        this.autoHideMagnifier = value;
        this.FireEvent(this.AutoHideMagnifierChanged);
      }
    }

    [Category("Reading")]
    [Description("Mouse Wheel and Cursor Keys delay on page transitions")]
    [DefaultValue(true)]
    public bool PageChangeDelay
    {
      get => this.pageChangeDelay;
      set
      {
        if (this.pageChangeDelay == value)
          return;
        this.pageChangeDelay = value;
        this.FireEvent(this.PageChangeDelayChanged);
      }
    }

    [Category("Reading")]
    [Description("Scrolling to page margin browses to new pages")]
    [DefaultValue(true)]
    public bool ScrollingDoesBrowse
    {
      get => this.scrollingDoesBrowse;
      set
      {
        if (this.scrollingDoesBrowse == value)
          return;
        this.scrollingDoesBrowse = value;
        this.FireEvent(this.ScrollingDoesBrowseChanged);
      }
    }

    [Category("Reading")]
    [Description("Zoom is reset to 100% on page change")]
    [DefaultValue(false)]
    public bool ResetZoomOnPageChange
    {
      get => this.resetZoomOnPageChange;
      set
      {
        if (this.resetZoomOnPageChange == value)
          return;
        this.resetZoomOnPageChange = value;
        this.FireEvent(this.ResetZoomOnPageChangeChanged);
      }
    }

    [Category("Reading")]
    [Description("During page change a zoom out is done")]
    [DefaultValue(true)]
    public bool ZoomInOutOnPageChange
    {
      get => this.zoomInOutOnPageChange;
      set
      {
        if (this.zoomInOutOnPageChange == value)
          return;
        this.zoomInOutOnPageChange = value;
        this.FireEvent(this.ZoomInOutOnPageChangeChanged);
      }
    }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool SmoothScrolling
    {
      get => this.smoothScrolling;
      set
      {
        if (this.smoothScrolling == value)
          return;
        this.smoothScrolling = value;
        this.FireEvent(this.SmoothScrollingChanged);
      }
    }

    [Category("Reading")]
    [Description("Blend animation while fast paging")]
    [DefaultValue(false)]
    public bool BlendWhilePaging
    {
      get => this.blendWhilePaging;
      set
      {
        if (this.blendWhilePaging == value)
          return;
        this.blendWhilePaging = value;
        this.FireEvent(this.BlendWhilePagingChanged);
      }
    }

    [DefaultValue(true)]
    public bool TrackCurrentPage
    {
      get => this.trackCurrentPage;
      set
      {
        if (this.trackCurrentPage == value)
          return;
        this.trackCurrentPage = value;
        this.FireEvent(this.TrackCurrentPageChanged);
      }
    }

    [DefaultValue(RightToLeftReadingMode.FlipPages)]
    [Browsable(false)]
    public RightToLeftReadingMode RightToLeftReadingMode
    {
      get => this.rightToLeftReadingMode;
      set
      {
        if (this.rightToLeftReadingMode == value)
          return;
        this.rightToLeftReadingMode = value;
        this.FireEvent(this.RightToLeftReadingModeChanged);
      }
    }

    [Category("Right to Left")]
    [Description("True right to left reading")]
    [DefaultValue(false)]
    public bool TrueRightToLeftReading
    {
      get => this.RightToLeftReadingMode == RightToLeftReadingMode.FlipParts;
      set
      {
        this.RightToLeftReadingMode = value ? RightToLeftReadingMode.FlipParts : RightToLeftReadingMode.FlipPages;
      }
    }

    [Category("Right to Left")]
    [Description("Left/right movement is also reversed")]
    [DefaultValue(false)]
    public bool LeftRightMovementReversed
    {
      get => this.leftRightMovementReversed;
      set
      {
        if (this.leftRightMovementReversed == value)
          return;
        this.leftRightMovementReversed = value;
        this.FireEvent(this.LeftRightMovementReversedChanged);
      }
    }

    [Category("Browser")]
    [Description("Show Tooltips for Books in the Browser")]
    [DefaultValue(false)]
    public bool ShowToolTips
    {
      get => this.showToolTips;
      set
      {
        if (this.showToolTips == value)
          return;
        this.showToolTips = value;
        this.FireEvent(this.ShowToolTipsChanged);
      }
    }

    [Category("Browser")]
    [Description("Show Search Links")]
    [DefaultValue(true)]
    public bool ShowSearchLinks
    {
      get => this.showSearchLinks;
      set
      {
        if (this.showSearchLinks == value)
          return;
        this.showSearchLinks = value;
        this.FireEvent(this.ShowSearchLinksChanged);
      }
    }

    [Category("Browser")]
    [Description("New loaded Thumbnails slowly fade in")]
    [DefaultValue(true)]
    public bool FadeInThumbnails
    {
      get => this.fadeInThumbnails;
      set
      {
        if (this.fadeInThumbnails == value)
          return;
        this.fadeInThumbnails = value;
        this.FireEvent(this.FadeInThumbnailsChanged);
      }
    }

    [Category("Browser")]
    [Description("Selected Thumbnails have a dog-ear")]
    [DefaultValue(true)]
    public bool DogEarThumbnails
    {
      get => this.dogEarThumbnails;
      set
      {
        if (this.dogEarThumbnails == value)
          return;
        this.dogEarThumbnails = value;
        this.FireEvent(this.DogEarThumbnailsChanged);
      }
    }

    [Category("Browser")]
    [Description("Thumbnails display numeric ratings")]
    [DefaultValue(true)]
    public bool NumericRatingThumbnails
    {
      get => this.numericRatingThumbnails;
      set
      {
        if (this.numericRatingThumbnails == value)
          return;
        this.numericRatingThumbnails = value;
        this.FireEvent(this.NumericRatingThumbnailsChanged);
      }
    }

    [Category("Browser")]
    [Description("Each List has own Quick Search settings")]
    [DefaultValue(true)]
    public bool LocalQuickSearch
    {
      get => this.localQuickSearch;
      set
      {
        if (this.localQuickSearch == value)
          return;
        this.localQuickSearch = value;
        this.FireEvent(this.LocalQuickSearchChanged);
      }
    }

    [Category("Browser")]
    [Description("All Cover Thumbnails have the same Size")]
    [DefaultValue(false)]
    public bool CoverThumbnailsSameSize
    {
      get => this.coverThumbnailsSameSize;
      set
      {
        if (this.coverThumbnailsSameSize == value)
          return;
        this.coverThumbnailsSameSize = value;
        this.FireEvent(this.CoverThumbnailsSameSizeChanged);
      }
    }

    [Category("Browser")]
    [Description("All Stacks in a List have the same Layout")]
    [DefaultValue(false)]
    public bool CommonListStackLayout
    {
      get => this.commonListStackLayout;
      set
      {
        if (this.commonListStackLayout == value)
          return;
        this.commonListStackLayout = value;
        this.FireEvent(this.CommonListStackLayoutChanged);
      }
    }

    [Category("Application")]
    [Description("Show Quick Open when no book is open")]
    [DefaultValue(true)]
    public bool ShowQuickOpen
    {
      get => this.showQuickOpen;
      set
      {
        if (this.showQuickOpen == value)
          return;
        this.showQuickOpen = value;
        this.FireEvent(this.ShowQuickOpenChanged);
      }
    }

    [Category("Application")]
    [Description("Show Catalog fields only for fileless Books")]
    [DefaultValue(true)]
    public bool CatalogOnlyForFileless
    {
      get => this.catalogOnlyForFileless;
      set
      {
        if (this.catalogOnlyForFileless == value)
          return;
        this.catalogOnlyForFileless = value;
        this.FireEvent(this.CatalogOnlyForFilelessChanged);
      }
    }

    [Category("Application")]
    [Description("Show custom Book fields")]
    [DefaultValue(false)]
    public bool ShowCustomBookFields
    {
      get => this.showCustomBookFields;
      set
      {
        if (this.showCustomBookFields == value)
          return;
        this.showCustomBookFields = value;
        this.FireEvent(this.ShowCustomBookFieldsChanged);
      }
    }

    [Category("Application")]
    [Description("Minimize moves ComicRack into the Notification Area")]
    [DefaultValue(false)]
    public bool MinimizeToTray
    {
      get => this.minimizeToTray;
      set
      {
        if (this.minimizeToTray == value)
          return;
        this.minimizeToTray = value;
        this.FireEvent(this.MinimizeToTrayChanged);
      }
    }

    [Category("Application")]
    [Description("Close moves ComicRack into the Notification Area")]
    [DefaultValue(true)]
    public bool CloseMinimizesToTray
    {
      get => this.closeMinimizeToTray;
      set
      {
        if (this.closeMinimizeToTray == value)
          return;
        this.closeMinimizeToTray = value;
        this.FireEvent(this.CloseMinimizesToTrayChanged);
      }
    }

    [Category("Reading")]
    [Description("Fullscreen also toggles Minimal User Interface")]
    [DefaultValue(false)]
    public bool AutoMinimalGui
    {
      get => this.autoMinimalGui;
      set
      {
        if (this.autoMinimalGui == value)
          return;
        this.autoMinimalGui = value;
        this.FireEvent(this.AutoMinimalGuiChanged);
      }
    }

    [Category("Application")]
    [Description("Animate expanding and collapsing Panels")]
    [DefaultValue(true)]
    public bool AnimatePanels
    {
      get => this.animatePanels;
      set
      {
        if (this.animatePanels == value)
          return;
        this.animatePanels = value;
        this.FireEvent(this.AnimatePanelsChanged);
      }
    }

    [Category("Browser")]
    [Description("Always display Browser Docking Grip")]
    [DefaultValue(false)]
    public bool AlwaysDisplayBrowserDockingGrip
    {
      get => this.alwaysDisplayBrowserDockingGrip;
      set
      {
        if (this.alwaysDisplayBrowserDockingGrip == value)
          return;
        this.alwaysDisplayBrowserDockingGrip = value;
        this.FireEvent(this.AlwaysDisplayBrowserDockingGripChanged);
      }
    }

    [DefaultValue(true)]
    [Browsable(false)]
    public bool AutoHideMainMenu
    {
      get => this.autoHideMainMenu;
      set
      {
        if (this.autoHideMainMenu == value)
          return;
        this.autoHideMainMenu = value;
        this.FireEvent(this.AutoHideMainMenuChanged);
      }
    }

    [Category("Application")]
    [Description("Show Main Menu if no Book is open")]
    [DefaultValue(true)]
    [Browsable(true)]
    public bool ShowMainMenuNoComicOpen
    {
      get => this.showMainMenuNoComicOpen;
      set
      {
        if (this.showMainMenuNoComicOpen == value)
          return;
        this.showMainMenuNoComicOpen = value;
        this.FireEvent(this.ShowMainMenuNoComicOpenChanged);
      }
    }

    [Category("Application")]
    [Description("3D display of covers in Book Info Dialog")]
    [DefaultValue(true)]
    [Browsable(true)]
    public bool InformationCover3D
    {
      get => this.informationCover3D;
      set
      {
        if (this.informationCover3D == value)
          return;
        this.informationCover3D = value;
        this.FireEvent(this.InformationCover3DChanged);
      }
    }

    [Category("Application")]
    [DefaultValue(true)]
    [Browsable(false)]
    public bool DisplayLibraryGauges
    {
      get => this.displayLibraryGauges;
      set
      {
        if (this.displayLibraryGauges == value)
          return;
        this.displayLibraryGauges = value;
        this.FireEvent(this.DisplayLibraryGaugesChanged);
      }
    }

    [Category("Application")]
    [Description("Gauge Format in Library Browser")]
    [DefaultValue(LibraryGauges.Default)]
    [Browsable(true)]
    public LibraryGauges LibraryGaugesFormat
    {
      get => this.libraryGaugesFormat;
      set
      {
        if (this.libraryGaugesFormat == value)
          return;
        this.libraryGaugesFormat = value;
        this.FireEvent(this.DisplayLibraryGaugesChanged);
      }
    }

    [Category("Application")]
    [Description("Newly added Books are checked")]
    [DefaultValue(true)]
    [Browsable(true)]
    public bool NewBooksChecked
    {
      get => this.newBooksChecked;
      set
      {
        if (this.newBooksChecked == value)
          return;
        this.newBooksChecked = value;
        this.FireEvent(this.NewBooksCheckedChanged);
      }
    }

    [Category("Caching")]
    [Description("Turn thumbnail caching on or off")]
    [DefaultValue(true)]
    [Browsable(false)]
    public bool ThumbCacheEnabled
    {
      get => this.thumbCacheEnabled;
      set
      {
        if (this.thumbCacheEnabled == value)
          return;
        this.thumbCacheEnabled = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Category("Caching")]
    [Description("Size of the thumbnail cache")]
    [DefaultValue(500)]
    [Browsable(false)]
    public int ThumbCacheSizeMB
    {
      get => this.thumbCacheSizeMB;
      set
      {
        if (this.thumbCacheSizeMB == value)
          return;
        this.thumbCacheSizeMB = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Category("Caching")]
    [Description("Turn page caching on or off")]
    [DefaultValue(true)]
    [Browsable(false)]
    public bool PageCacheEnabled
    {
      get => this.pageCacheEnabled;
      set
      {
        if (this.pageCacheEnabled == value)
          return;
        this.pageCacheEnabled = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Category("Caching")]
    [Description("Size of the page cache")]
    [DefaultValue(500)]
    [Browsable(false)]
    public int PageCacheSizeMB
    {
      get => this.pageCacheSizeMB;
      set
      {
        if (this.pageCacheSizeMB == value)
          return;
        this.pageCacheSizeMB = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Category("Caching")]
    [Description("Turn Internet caching on or off")]
    [DefaultValue(true)]
    [Browsable(false)]
    public bool InternetCacheEnabled
    {
      get => this.internetCacheEnabled;
      set
      {
        if (this.internetCacheEnabled == value)
          return;
        this.internetCacheEnabled = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Category("Caching")]
    [Description("Size of the internet cache")]
    [DefaultValue(1000)]
    [Browsable(false)]
    public int InternetCacheSizeMB
    {
      get => this.internetCacheSizeMB;
      set
      {
        if (this.internetCacheSizeMB == value)
          return;
        this.internetCacheSizeMB = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Category("Caching")]
    [Description("Size of the memory thumbnail cache")]
    [DefaultValue(50)]
    [Browsable(false)]
    public int MemoryThumbCacheSizeMB
    {
      get => this.memoryThumbCacheSizeMB;
      set
      {
        value = value.Clamp(5, 100);
        if (this.memoryThumbCacheSizeMB == value)
          return;
        this.memoryThumbCacheSizeMB = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Category("Caching")]
    [Description("Pages to cache in memory")]
    [DefaultValue(25)]
    [Browsable(false)]
    public int MemoryPageCacheCount
    {
      get => this.memoryPageCacheCount;
      set
      {
        value = value.Clamp(20, 100);
        if (this.memoryPageCacheCount == value)
          return;
        this.memoryPageCacheCount = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Description("Optimize Memory Thumbnail cache")]
    [DefaultValue(true)]
    [Browsable(false)]
    public bool MemoryThumbCacheOptimized
    {
      get => this.memoryThumbCacheOptimized;
      set
      {
        if (this.memoryThumbCacheOptimized == value)
          return;
        this.memoryThumbCacheOptimized = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [Description("Optimize Memory Page cache")]
    [DefaultValue(true)]
    [Browsable(false)]
    public bool MemoryPageCacheOptimized
    {
      get => this.memoryPageCacheOptimized;
      set
      {
        if (this.memoryPageCacheOptimized == value)
          return;
        this.memoryPageCacheOptimized = value;
        this.FireEvent(this.CacheSettingsChanged);
      }
    }

    [DefaultValue(1024)]
    [Browsable(false)]
    public int MaximumMemoryMB { get; set; }

    [DefaultValue(false)]
    public bool RemoveMissingFilesOnFullScan
    {
      get => this.removeMissingFilesOnFullScan;
      set
      {
        if (this.removeMissingFilesOnFullScan == value)
          return;
        this.removeMissingFilesOnFullScan = value;
        this.FireEvent(this.ScanOptionsChanged);
      }
    }

    [DefaultValue(false)]
    public bool DontAddRemoveFiles
    {
      get => this.dontAddRemoveFiles;
      set
      {
        if (this.dontAddRemoveFiles == value)
          return;
        this.dontAddRemoveFiles = value;
        this.FireEvent(this.ScanOptionsChanged);
      }
    }

    [DefaultValue(false)]
    public bool OverwriteAssociations
    {
      get => this.overwriteAssociations;
      set
      {
        if (this.overwriteAssociations == value)
          return;
        this.overwriteAssociations = value;
        this.FireEvent(this.OverwriteAssociationsChanged);
      }
    }

    public List<Settings.RemoteViewConfig> RemoteViewConfigList => this.remoteViewConfigList;

    public List<Settings.RemoteExplorerViewSettings> RemoteExplorerViewSettingsList
    {
      get => this.remoteExplorerViewSettingsList;
    }

    public DisplayListConfig GetRemoteViewConfig(Guid id, DisplayListConfig defaultConfig)
    {
      Settings.RemoteViewConfig remoteViewConfig = this.remoteViewConfigList.Find((Predicate<Settings.RemoteViewConfig>) (item => item.Id == id));
      DisplayListConfig displayListConfig = (DisplayListConfig) null;
      if (remoteViewConfig != null)
        displayListConfig = remoteViewConfig.Display;
      return displayListConfig ?? defaultConfig;
    }

    public void UpdateRemoteViewConfig(Guid id, DisplayListConfig config)
    {
      Settings.RemoteViewConfig remoteViewConfig = this.remoteViewConfigList.Find((Predicate<Settings.RemoteViewConfig>) (item => item.Id == id));
      if (remoteViewConfig != null)
        remoteViewConfig.Display = config;
      else
        this.remoteViewConfigList.Add(new Settings.RemoteViewConfig(id, config));
    }

    public ComicExplorerViewSettings GetRemoteExplorerViewSetting(Guid id)
    {
      return this.RemoteExplorerViewSettingsList.FirstOrDefault<Settings.RemoteExplorerViewSettings>((Func<Settings.RemoteExplorerViewSettings, bool>) (s => s.Id == id))?.Settings;
    }

    public void UpdateExplorerViewSetting(Guid id, ComicExplorerViewSettings setting)
    {
      this.RemoteExplorerViewSettingsList.RemoveAll((Predicate<Settings.RemoteExplorerViewSettings>) (s => s.Id == id));
      this.RemoteExplorerViewSettingsList.Add(new Settings.RemoteExplorerViewSettings(id, setting));
    }

    [DefaultValue(null)]
    public string CultureName { get; set; }

    [Category("Import & Export")]
    [Description("Exported Book Lists contain filenames")]
    [DefaultValue(false)]
    public bool ExportedListsContainFilenames { get; set; }

    public List<string> QuickSearchList => this.quickSearchList;

    public List<string> LibraryQuickSearchList => this.libraryQuickSearchList;

    public MruList<string> KeyboardLayouts => this.keyboardLayouts;

    public MruList<string> ThumbnailFiles => this.thumbnailFiles;

    [DefaultValue(null)]
    public ExportSetting CurrentExportSetting { get; set; }

    public ExportSettingCollection ExportUserPresets => this.exportUserPresets;

    public SmartList<DeviceSyncSettings> Devices => this.devices;

    [DefaultValue(null)]
    public string UserEmail { get; set; }

    [DefaultValue(null)]
    [XmlElement("VK")]
    public string ValidationKey { get; set; }

    [DefaultValue(typeof (DateTime), "01.01.0001")]
    public DateTime ValidationDate { get; set; }

    [DefaultValue(null)]
    public string DonationShown { get; set; }

    internal void SetActivated(bool activated = true)
    {
      if (activated)
        this.ValidationDate = DateTime.UtcNow;
      this.ValidationKey = activated ? Password.CreateHash(Environment.MachineName + this.UserEmail + (object) this.ValidationDate) : (string) null;
    }

    public bool IsActivated
    {
      get
      {
        return !string.IsNullOrEmpty(this.UserEmail) && Password.Verify(Environment.MachineName + this.UserEmail + (object) this.ValidationDate, this.ValidationKey);
      }
    }

    public string OpenRemoteFilter { get; set; }

    public string OpenRemotePassword { get; set; }

    [Category("Reading")]
    [Description("Show Quick Review Dialog after finishing Book")]
    [DefaultValue(false)]
    public bool AutoShowQuickReview { get; set; }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool TweetQuickReview { get; set; }

    [Browsable(false)]
    [DefaultValue(null)]
    public string TwitterAccessToken { get; set; }

    [Browsable(false)]
    [DefaultValue(null)]
    public string TwitterOAuthToken { get; set; }

    [Browsable(false)]
    [DefaultValue(null)]
    public string TwitterScreenName { get; set; }

    [Browsable(false)]
    [DefaultValue(null)]
    public string TwitterUserId { get; set; }

    public void ResetTwitter()
    {
      this.TwitterOAuthToken = this.TwitterAccessToken = this.TwitterUserId = this.TwitterScreenName = (string) null;
    }

    public bool HasTwitterAccess => !string.IsNullOrEmpty(this.TwitterAccessToken);

    private void FireEvent(EventHandler eh)
    {
      if (eh != null)
        eh((object) this, EventArgs.Empty);
      if (this.SettingsChanged == null)
        return;
      this.SettingsChanged((object) this, EventArgs.Empty);
    }

    public void Fix()
    {
      this.Devices.ForEach((Action<DeviceSyncSettings>) (d => d.Lists.RemoveAll<DeviceSyncSettings.SharedList>((Predicate<DeviceSyncSettings.SharedList>) (sl => sl == null))));
    }

    public static Settings LoadBinary(string file)
    {
      try
      {
        using (Stream serializationStream = (Stream) File.OpenRead(file))
          return (Settings) new BinaryFormatter()
          {
            AssemblyFormat = FormatterAssemblyStyle.Simple
          }.Deserialize(serializationStream);
      }
      catch (Exception ex)
      {
        return new Settings();
      }
    }

    public static Settings Load(string file)
    {
      try
      {
        Settings settings = XmlUtility.Load<Settings>(file);
        settings.Fix();
        return settings;
      }
      catch (Exception ex)
      {
        return new Settings();
      }
    }

    public void SaveBinary(string file)
    {
      using (Stream serializationStream = (Stream) File.Create(file))
        new BinaryFormatter()
        {
          TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
          AssemblyFormat = FormatterAssemblyStyle.Simple
        }.Serialize(serializationStream, (object) this);
    }

    public void Save(string file) => XmlUtility.Store(file, (object) this);

    [Serializable]
    public class PasswordCacheEntry
    {
      [XmlAttribute]
      [DefaultValue(0)]
      public int RemoteId;
      [XmlAttribute]
      [DefaultValue(null)]
      public string Password;

      public PasswordCacheEntry()
      {
      }

      public PasswordCacheEntry(string remote, string password)
      {
        this.RemoteId = remote.GetHashCode();
        this.Password = password;
      }
    }

    [Serializable]
    public class RemoteViewConfig : IIdentity, IDisplayListConfig
    {
      public RemoteViewConfig()
      {
      }

      public RemoteViewConfig(Guid id, DisplayListConfig config)
      {
        this.Id = id;
        this.Display = config;
      }

      [XmlAttribute]
      public Guid Id { get; set; }

      public DisplayListConfig Display { get; set; }
    }

    public class RemoteExplorerViewSettings : IIdentity
    {
      public RemoteExplorerViewSettings()
      {
      }

      public RemoteExplorerViewSettings(Guid id, ComicExplorerViewSettings settings)
      {
        this.Id = id;
        this.Settings = settings;
      }

      [XmlAttribute]
      public Guid Id { get; set; }

      [DefaultValue(null)]
      public ComicExplorerViewSettings Settings { get; set; }
    }
  }
}

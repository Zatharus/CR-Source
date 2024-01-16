// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.PreferencesDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Reflection;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Common.Xml;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Engine.IO.Network;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Engine.Sync;
using cYo.Projects.ComicRack.Plugins;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class PreferencesDialog : Form
  {
    private CheckBox chkHighQualityDisplay;
    private CheckBox chkEnableThumbnailCache;
    private NumericUpDown numThumbnailCacheSize;
    private Button btClearThumbnailCache;
    private Button btRemoveFolder;
    private Button btAddFolder;
    private CheckedListBoxEx lbPaths;
    private Button btOK;
    private Button btCancel;
    private Label lblScan;
    private Button btScan;
    private CheckBox chkAutoRemoveMissing;
    private Button btResetMessages;
    private Label labelReshowHidden;
    private Label labelWatchedFolders;
    private CheckedListBox lbFormats;
    private Label labelCheckedFormats;
    private CheckBox chkOverwriteAssociations;
    private CheckBox chkEnablePageCache;
    private Button btClearPageCache;
    private NumericUpDown numPageCacheSize;
    private CheckBox chkLookForShared;
    private Button btChangeFolder;
    private CheckBox chkAutoUpdateComicFiles;
    private TrackBarLite tbContrast;
    private TrackBarLite tbBrightness;
    private TrackBarLite tbSaturation;
    private Label labelContrast;
    private Label labelBrightness;
    private Label labelSaturation;
    private Button btApply;
    private CheckBox chkAutoContrast;
    private TextBox txCoverFilter;
    private Label labelExcludeCover;
    private ImageList imageList;
    private IContainer components;
    private Button btResetColor;
    private TrackBarLite tbOverlayScaling;
    private Label labelOverlaySize;
    private ToolTip toolTip;
    private ListBox lbLanguages;
    private Label labelLanguage;
    private KeyboardShortcutEditor keyboardShortcutEditor;
    private Label labelMemThumbSize;
    private NumericUpDown numMemPageCount;
    private Label labelMemPageCount;
    private NumericUpDown numMemThumbSize;
    private CheckBox chkMemPageOptimized;
    private CheckBox chkMemThumbOptimized;
    private Button btBackupDatabase;
    private Button btRestoreDatabase;
    private Button btTranslate;
    private CheckBox chkEnableHardware;
    private CheckBox chkShowStatusOverlay;
    private CheckBox chkShowNavigationOverlay;
    private CheckBox chkShowVisiblePartOverlay;
    private CheckBox chkShowCurrentPageOverlay;
    private CheckBox chkEnableDisplayChangeAnimation;
    private CheckBox chkEnableInertialMouseScrolling;
    private Panel pageBehavior;
    private Label lblMouseWheel;
    private Label lblFast;
    private Label lblSlow;
    private TrackBarLite tbMouseWheel;
    private Label lblPageCacheUsage;
    private Label lblThumbCacheUsage;
    private CheckBox chkUpdateComicFiles;
    private CheckBox chkAnamorphicScaling;
    private Panel pageReader;
    private CollapsibleGroupBox groupOverlays;
    private CollapsibleGroupBox grpDisplay;
    private CollapsibleGroupBox grpMouse;
    private CollapsibleGroupBox grpKeyboard;
    private CollapsibleGroupBox groupHardwareAcceleration;
    private Panel pageAdvanced;
    private CollapsibleGroupBox grpLanguages;
    private CollapsibleGroupBox groupMessagesAndSocial;
    private CollapsibleGroupBox groupOtherComics;
    private CollapsibleGroupBox grpDatabaseBackup;
    private CollapsibleGroupBox groupMemory;
    private CollapsibleGroupBox grpIntegration;
    private Panel pageLibrary;
    private CollapsibleGroupBox grpSharing;
    private CollapsibleGroupBox groupComicFolders;
    private CollapsibleGroupBox grpScanning;
    private CheckBox chkEnableSoftwareFiltering;
    private Label lblPageMemCacheUsage;
    private Label lblThumbMemCacheUsage;
    private Timer memCacheUpate;
    private CheckBox chkShowPageNames;
    private CheckBox chkEnableHardwareFiltering;
    private Panel pageScripts;
    private CollapsibleGroupBox grpScripts;
    private ListView lvScripts;
    private ColumnHeader chScriptName;
    private ColumnHeader chScriptPackage;
    private CheckBox chkDisableScripting;
    private CollapsibleGroupBox grpScriptSettings;
    private Button btAddLibraryFolder;
    private Label labelScriptPaths;
    private TextBox txLibraries;
    private CollapsibleGroupBox grpPackages;
    private Button btRemovePackage;
    private Button btInstallPackage;
    private ListView lvPackages;
    private ImageList packageImageList;
    private ColumnHeader chPackageName;
    private ColumnHeader chPackageAuthor;
    private ColumnHeader chPackageDescription;
    private Button btAssociateExtensions;
    private Label lblInternetCacheUsage;
    private CheckBox chkEnableInternetCache;
    private NumericUpDown numInternetCacheSize;
    private Button btClearInternetCache;
    private CollapsibleGroupBox grpServerSettings;
    private TextBox txPublicServerAddress;
    private Label labelPublicServerAddress;
    private TabControl tabShares;
    private Button btRemoveShare;
    private Button btAddShare;
    private PasswordTextBox txPrivateListingPassword;
    private Label labelPrivateListPassword;
    private Label labelSharpening;
    private TrackBarLite tbSharpening;
    private CheckBox chkDontAddRemovedFiles;
    private Button btConfigScript;
    private Button btOpenFolder;
    private CheckBox chkAutoConnectShares;
    private Button btExportKeyboard;
    private SplitButton btImportKeyboard;
    private ContextMenuStrip cmKeyboardLayout;
    private ToolStripMenuItem miDefaultKeyboardLayout;
    private ToolStripSeparator toolStripMenuItem1;
    private ComboBox cbNavigationOverlayPosition;
    private Label labelNavigationOverlayPosition;
    private Panel panelReaderOverlays;
    private Label labelVisiblePartOverlay;
    private Label labelNavigationOverlay;
    private Label labelStatusOverlay;
    private Label labelPageOverlay;
    private CheckBox chkHideSampleScripts;
    private CheckBox chkSmoothAutoScrolling;
    private TrackBarLite tbGamma;
    private Label labelGamma;
    private GroupBox grpDiskCache;
    private GroupBox grpMaximumMemoryUsage;
    private Label lblMaximumMemoryUsageValue;
    private TrackBarLite tbMaximumMemoryUsage;
    private Label lblMaximumMemoryUsage;
    private GroupBox grpMemoryCache;
    private CollapsibleGroupBox groupLibraryDisplay;
    private CheckBox chkLibraryGaugesTotal;
    private CheckBox chkLibraryGaugesUnread;
    private CheckBox chkLibraryGaugesNumeric;
    private CheckBox chkLibraryGaugesNew;
    private CheckBox chkLibraryGauges;
    private CheckBox tabReader;
    private CheckBox tabLibraries;
    private CheckBox tabBehavior;
    private CheckBox tabScripts;
    private CheckBox tabAdvanced;
    private CollapsibleGroupBox grpWirelessSetup;
    private Label lblWifiStatus;
    private Label lblWifiAddresses;
    private TextBox txWifiAddresses;
    private Button btTestWifi;
    private Button btResetTwitter;
    private Label labelResetTwitter;
    private const int MaximumMemoryStepSize = 32;
    private static int activeTab = -1;
    private static readonly string DuplicatePackageText = TR.Messages["ScriptPackageExists", "A Script Package with the same name already exists! Do you want to overwrite this Package?"];
    private readonly List<CheckBox> tabButtons = new List<CheckBox>();
    private PluginEngine pluginEngine;
    private bool blockSetTab;

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ListViewGroup listViewGroup1 = new ListViewGroup("Installed", HorizontalAlignment.Left);
      ListViewGroup listViewGroup2 = new ListViewGroup("To be removed (requires restart)", HorizontalAlignment.Left);
      ListViewGroup listViewGroup3 = new ListViewGroup("To be installed (requires restart)", HorizontalAlignment.Left);
      this.btOK = new Button();
      this.btCancel = new Button();
      this.imageList = new ImageList(this.components);
      this.btApply = new Button();
      this.toolTip = new ToolTip(this.components);
      this.pageBehavior = new Panel();
      this.pageReader = new Panel();
      this.groupHardwareAcceleration = new CollapsibleGroupBox();
      this.chkEnableHardwareFiltering = new CheckBox();
      this.chkEnableSoftwareFiltering = new CheckBox();
      this.chkEnableHardware = new CheckBox();
      this.chkEnableDisplayChangeAnimation = new CheckBox();
      this.grpMouse = new CollapsibleGroupBox();
      this.chkSmoothAutoScrolling = new CheckBox();
      this.lblFast = new Label();
      this.lblMouseWheel = new Label();
      this.chkEnableInertialMouseScrolling = new CheckBox();
      this.lblSlow = new Label();
      this.tbMouseWheel = new TrackBarLite();
      this.groupOverlays = new CollapsibleGroupBox();
      this.panelReaderOverlays = new Panel();
      this.labelVisiblePartOverlay = new Label();
      this.labelNavigationOverlay = new Label();
      this.labelStatusOverlay = new Label();
      this.labelPageOverlay = new Label();
      this.cbNavigationOverlayPosition = new ComboBox();
      this.labelNavigationOverlayPosition = new Label();
      this.chkShowPageNames = new CheckBox();
      this.tbOverlayScaling = new TrackBarLite();
      this.chkShowCurrentPageOverlay = new CheckBox();
      this.chkShowStatusOverlay = new CheckBox();
      this.chkShowVisiblePartOverlay = new CheckBox();
      this.chkShowNavigationOverlay = new CheckBox();
      this.labelOverlaySize = new Label();
      this.grpKeyboard = new CollapsibleGroupBox();
      this.btExportKeyboard = new Button();
      this.btImportKeyboard = new SplitButton();
      this.cmKeyboardLayout = new ContextMenuStrip(this.components);
      this.miDefaultKeyboardLayout = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.keyboardShortcutEditor = new KeyboardShortcutEditor();
      this.grpDisplay = new CollapsibleGroupBox();
      this.tbGamma = new TrackBarLite();
      this.labelGamma = new Label();
      this.chkAnamorphicScaling = new CheckBox();
      this.chkHighQualityDisplay = new CheckBox();
      this.labelSharpening = new Label();
      this.tbSharpening = new TrackBarLite();
      this.btResetColor = new Button();
      this.chkAutoContrast = new CheckBox();
      this.labelSaturation = new Label();
      this.tbSaturation = new TrackBarLite();
      this.labelBrightness = new Label();
      this.tbBrightness = new TrackBarLite();
      this.tbContrast = new TrackBarLite();
      this.labelContrast = new Label();
      this.pageAdvanced = new Panel();
      this.grpWirelessSetup = new CollapsibleGroupBox();
      this.btTestWifi = new Button();
      this.lblWifiStatus = new Label();
      this.lblWifiAddresses = new Label();
      this.txWifiAddresses = new TextBox();
      this.grpIntegration = new CollapsibleGroupBox();
      this.btAssociateExtensions = new Button();
      this.labelCheckedFormats = new Label();
      this.chkOverwriteAssociations = new CheckBox();
      this.lbFormats = new CheckedListBox();
      this.groupMessagesAndSocial = new CollapsibleGroupBox();
      this.btResetMessages = new Button();
      this.labelReshowHidden = new Label();
      this.groupMemory = new CollapsibleGroupBox();
      this.grpMaximumMemoryUsage = new GroupBox();
      this.lblMaximumMemoryUsageValue = new Label();
      this.tbMaximumMemoryUsage = new TrackBarLite();
      this.lblMaximumMemoryUsage = new Label();
      this.grpMemoryCache = new GroupBox();
      this.lblPageMemCacheUsage = new Label();
      this.labelMemThumbSize = new Label();
      this.lblThumbMemCacheUsage = new Label();
      this.numMemPageCount = new NumericUpDown();
      this.labelMemPageCount = new Label();
      this.chkMemPageOptimized = new CheckBox();
      this.chkMemThumbOptimized = new CheckBox();
      this.numMemThumbSize = new NumericUpDown();
      this.grpDiskCache = new GroupBox();
      this.chkEnableInternetCache = new CheckBox();
      this.lblInternetCacheUsage = new Label();
      this.btClearPageCache = new Button();
      this.numPageCacheSize = new NumericUpDown();
      this.numInternetCacheSize = new NumericUpDown();
      this.btClearThumbnailCache = new Button();
      this.btClearInternetCache = new Button();
      this.chkEnablePageCache = new CheckBox();
      this.lblPageCacheUsage = new Label();
      this.numThumbnailCacheSize = new NumericUpDown();
      this.chkEnableThumbnailCache = new CheckBox();
      this.lblThumbCacheUsage = new Label();
      this.grpDatabaseBackup = new CollapsibleGroupBox();
      this.btRestoreDatabase = new Button();
      this.btBackupDatabase = new Button();
      this.groupOtherComics = new CollapsibleGroupBox();
      this.chkUpdateComicFiles = new CheckBox();
      this.labelExcludeCover = new Label();
      this.chkAutoUpdateComicFiles = new CheckBox();
      this.txCoverFilter = new TextBox();
      this.grpLanguages = new CollapsibleGroupBox();
      this.btTranslate = new Button();
      this.labelLanguage = new Label();
      this.lbLanguages = new ListBox();
      this.pageLibrary = new Panel();
      this.grpServerSettings = new CollapsibleGroupBox();
      this.txPrivateListingPassword = new PasswordTextBox();
      this.labelPrivateListPassword = new Label();
      this.labelPublicServerAddress = new Label();
      this.txPublicServerAddress = new TextBox();
      this.grpSharing = new CollapsibleGroupBox();
      this.chkAutoConnectShares = new CheckBox();
      this.btRemoveShare = new Button();
      this.btAddShare = new Button();
      this.tabShares = new TabControl();
      this.chkLookForShared = new CheckBox();
      this.groupLibraryDisplay = new CollapsibleGroupBox();
      this.chkLibraryGaugesTotal = new CheckBox();
      this.chkLibraryGaugesUnread = new CheckBox();
      this.chkLibraryGaugesNumeric = new CheckBox();
      this.chkLibraryGaugesNew = new CheckBox();
      this.chkLibraryGauges = new CheckBox();
      this.grpScanning = new CollapsibleGroupBox();
      this.chkDontAddRemovedFiles = new CheckBox();
      this.chkAutoRemoveMissing = new CheckBox();
      this.lblScan = new Label();
      this.btScan = new Button();
      this.groupComicFolders = new CollapsibleGroupBox();
      this.btOpenFolder = new Button();
      this.btChangeFolder = new Button();
      this.lbPaths = new CheckedListBoxEx();
      this.labelWatchedFolders = new Label();
      this.btRemoveFolder = new Button();
      this.btAddFolder = new Button();
      this.memCacheUpate = new Timer(this.components);
      this.pageScripts = new Panel();
      this.grpScriptSettings = new CollapsibleGroupBox();
      this.btAddLibraryFolder = new Button();
      this.chkDisableScripting = new CheckBox();
      this.labelScriptPaths = new Label();
      this.txLibraries = new TextBox();
      this.grpScripts = new CollapsibleGroupBox();
      this.chkHideSampleScripts = new CheckBox();
      this.btConfigScript = new Button();
      this.lvScripts = new ListView();
      this.chScriptName = new ColumnHeader();
      this.chScriptPackage = new ColumnHeader();
      this.grpPackages = new CollapsibleGroupBox();
      this.btRemovePackage = new Button();
      this.btInstallPackage = new Button();
      this.lvPackages = new ListView();
      this.chPackageName = new ColumnHeader();
      this.chPackageAuthor = new ColumnHeader();
      this.chPackageDescription = new ColumnHeader();
      this.packageImageList = new ImageList(this.components);
      this.tabReader = new CheckBox();
      this.tabLibraries = new CheckBox();
      this.tabBehavior = new CheckBox();
      this.tabScripts = new CheckBox();
      this.tabAdvanced = new CheckBox();
      this.btResetTwitter = new Button();
      this.labelResetTwitter = new Label();
      this.pageReader.SuspendLayout();
      this.groupHardwareAcceleration.SuspendLayout();
      this.grpMouse.SuspendLayout();
      this.groupOverlays.SuspendLayout();
      this.panelReaderOverlays.SuspendLayout();
      this.grpKeyboard.SuspendLayout();
      this.cmKeyboardLayout.SuspendLayout();
      this.grpDisplay.SuspendLayout();
      this.pageAdvanced.SuspendLayout();
      this.grpWirelessSetup.SuspendLayout();
      this.grpIntegration.SuspendLayout();
      this.groupMessagesAndSocial.SuspendLayout();
      this.groupMemory.SuspendLayout();
      this.grpMaximumMemoryUsage.SuspendLayout();
      this.grpMemoryCache.SuspendLayout();
      this.numMemPageCount.BeginInit();
      this.numMemThumbSize.BeginInit();
      this.grpDiskCache.SuspendLayout();
      this.numPageCacheSize.BeginInit();
      this.numInternetCacheSize.BeginInit();
      this.numThumbnailCacheSize.BeginInit();
      this.grpDatabaseBackup.SuspendLayout();
      this.groupOtherComics.SuspendLayout();
      this.grpLanguages.SuspendLayout();
      this.pageLibrary.SuspendLayout();
      this.grpServerSettings.SuspendLayout();
      this.grpSharing.SuspendLayout();
      this.groupLibraryDisplay.SuspendLayout();
      this.grpScanning.SuspendLayout();
      this.groupComicFolders.SuspendLayout();
      this.pageScripts.SuspendLayout();
      this.grpScriptSettings.SuspendLayout();
      this.grpScripts.SuspendLayout();
      this.grpPackages.SuspendLayout();
      this.SuspendLayout();
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(351, 422);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 1;
      this.btOK.Text = "&OK";
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(437, 422);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 2;
      this.btCancel.Text = "&Cancel";
      this.imageList.ColorDepth = ColorDepth.Depth32Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = Color.Transparent;
      this.btApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btApply.FlatStyle = FlatStyle.System;
      this.btApply.Location = new System.Drawing.Point(523, 422);
      this.btApply.Name = "btApply";
      this.btApply.Size = new System.Drawing.Size(80, 24);
      this.btApply.TabIndex = 3;
      this.btApply.Text = "&Apply";
      this.btApply.Click += new EventHandler(this.btApply_Click);
      this.pageBehavior.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pageBehavior.AutoScroll = true;
      this.pageBehavior.BorderStyle = BorderStyle.FixedSingle;
      this.pageBehavior.Location = new System.Drawing.Point(84, 8);
      this.pageBehavior.Name = "pageBehavior";
      this.pageBehavior.Size = new System.Drawing.Size(517, 408);
      this.pageBehavior.TabIndex = 6;
      this.pageReader.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pageReader.AutoScroll = true;
      this.pageReader.BorderStyle = BorderStyle.FixedSingle;
      this.pageReader.Controls.Add((Control) this.groupHardwareAcceleration);
      this.pageReader.Controls.Add((Control) this.grpMouse);
      this.pageReader.Controls.Add((Control) this.groupOverlays);
      this.pageReader.Controls.Add((Control) this.grpKeyboard);
      this.pageReader.Controls.Add((Control) this.grpDisplay);
      this.pageReader.Location = new System.Drawing.Point(84, 8);
      this.pageReader.Name = "pageReader";
      this.pageReader.Size = new System.Drawing.Size(517, 408);
      this.pageReader.TabIndex = 8;
      this.groupHardwareAcceleration.Controls.Add((Control) this.chkEnableHardwareFiltering);
      this.groupHardwareAcceleration.Controls.Add((Control) this.chkEnableSoftwareFiltering);
      this.groupHardwareAcceleration.Controls.Add((Control) this.chkEnableHardware);
      this.groupHardwareAcceleration.Controls.Add((Control) this.chkEnableDisplayChangeAnimation);
      this.groupHardwareAcceleration.Dock = DockStyle.Top;
      this.groupHardwareAcceleration.Location = new System.Drawing.Point(0, 1180);
      this.groupHardwareAcceleration.Name = "groupHardwareAcceleration";
      this.groupHardwareAcceleration.Size = new System.Drawing.Size(498, 137);
      this.groupHardwareAcceleration.TabIndex = 3;
      this.groupHardwareAcceleration.Text = "Hardware Acceleration";
      this.chkEnableHardwareFiltering.AutoSize = true;
      this.chkEnableHardwareFiltering.Location = new System.Drawing.Point(33, 70);
      this.chkEnableHardwareFiltering.Name = "chkEnableHardwareFiltering";
      this.chkEnableHardwareFiltering.Size = new System.Drawing.Size(138, 17);
      this.chkEnableHardwareFiltering.TabIndex = 1;
      this.chkEnableHardwareFiltering.Text = "Enable Hardware Filters";
      this.chkEnableHardwareFiltering.UseVisualStyleBackColor = true;
      this.chkEnableSoftwareFiltering.AutoSize = true;
      this.chkEnableSoftwareFiltering.Location = new System.Drawing.Point(33, 88);
      this.chkEnableSoftwareFiltering.Name = "chkEnableSoftwareFiltering";
      this.chkEnableSoftwareFiltering.Size = new System.Drawing.Size(134, 17);
      this.chkEnableSoftwareFiltering.TabIndex = 2;
      this.chkEnableSoftwareFiltering.Text = "Enable Software Filters";
      this.chkEnableSoftwareFiltering.UseVisualStyleBackColor = true;
      this.chkEnableHardware.AutoSize = true;
      this.chkEnableHardware.Location = new System.Drawing.Point(12, 38);
      this.chkEnableHardware.Name = "chkEnableHardware";
      this.chkEnableHardware.Size = new System.Drawing.Size(170, 17);
      this.chkEnableHardware.TabIndex = 0;
      this.chkEnableHardware.Text = "Enable Hardware Acceleration";
      this.chkEnableHardware.UseVisualStyleBackColor = true;
      this.chkEnableDisplayChangeAnimation.AutoSize = true;
      this.chkEnableDisplayChangeAnimation.Location = new System.Drawing.Point(33, 108);
      this.chkEnableDisplayChangeAnimation.Name = "chkEnableDisplayChangeAnimation";
      this.chkEnableDisplayChangeAnimation.Size = new System.Drawing.Size(229, 17);
      this.chkEnableDisplayChangeAnimation.TabIndex = 3;
      this.chkEnableDisplayChangeAnimation.Text = "Enable Animation of Page Display changes";
      this.chkEnableDisplayChangeAnimation.UseVisualStyleBackColor = true;
      this.grpMouse.Controls.Add((Control) this.chkSmoothAutoScrolling);
      this.grpMouse.Controls.Add((Control) this.lblFast);
      this.grpMouse.Controls.Add((Control) this.lblMouseWheel);
      this.grpMouse.Controls.Add((Control) this.chkEnableInertialMouseScrolling);
      this.grpMouse.Controls.Add((Control) this.lblSlow);
      this.grpMouse.Controls.Add((Control) this.tbMouseWheel);
      this.grpMouse.Dock = DockStyle.Top;
      this.grpMouse.Location = new System.Drawing.Point(0, 1046);
      this.grpMouse.Name = "grpMouse";
      this.grpMouse.Size = new System.Drawing.Size(498, 134);
      this.grpMouse.TabIndex = 5;
      this.grpMouse.Text = "Mouse & Scrolling";
      this.chkSmoothAutoScrolling.AutoSize = true;
      this.chkSmoothAutoScrolling.Location = new System.Drawing.Point(9, 39);
      this.chkSmoothAutoScrolling.Name = "chkSmoothAutoScrolling";
      this.chkSmoothAutoScrolling.Size = new System.Drawing.Size(130, 17);
      this.chkSmoothAutoScrolling.TabIndex = 0;
      this.chkSmoothAutoScrolling.Text = "Smooth Auto Scrolling";
      this.chkSmoothAutoScrolling.UseVisualStyleBackColor = true;
      this.lblFast.Location = new System.Drawing.Point(426, 96);
      this.lblFast.Name = "lblFast";
      this.lblFast.Size = new System.Drawing.Size(56, 19);
      this.lblFast.TabIndex = 4;
      this.lblFast.Text = "fast";
      this.lblMouseWheel.AutoSize = true;
      this.lblMouseWheel.Location = new System.Drawing.Point(9, 97);
      this.lblMouseWheel.Name = "lblMouseWheel";
      this.lblMouseWheel.Size = new System.Drawing.Size(117, 13);
      this.lblMouseWheel.TabIndex = 0;
      this.lblMouseWheel.Text = "Mouse Wheel scrolling:";
      this.chkEnableInertialMouseScrolling.AutoSize = true;
      this.chkEnableInertialMouseScrolling.Location = new System.Drawing.Point(9, 62);
      this.chkEnableInertialMouseScrolling.Name = "chkEnableInertialMouseScrolling";
      this.chkEnableInertialMouseScrolling.Size = new System.Drawing.Size(169, 17);
      this.chkEnableInertialMouseScrolling.TabIndex = 1;
      this.chkEnableInertialMouseScrolling.Text = "Enable Inertial Mouse scrolling";
      this.chkEnableInertialMouseScrolling.UseVisualStyleBackColor = true;
      this.lblSlow.Location = new System.Drawing.Point(186, 97);
      this.lblSlow.Name = "lblSlow";
      this.lblSlow.Size = new System.Drawing.Size(55, 19);
      this.lblSlow.TabIndex = 2;
      this.lblSlow.Text = "slow";
      this.lblSlow.TextAlign = ContentAlignment.TopRight;
      this.tbMouseWheel.Location = new System.Drawing.Point(247, 97);
      this.tbMouseWheel.Maximum = 50;
      this.tbMouseWheel.Minimum = 5;
      this.tbMouseWheel.Name = "tbMouseWheel";
      this.tbMouseWheel.Size = new System.Drawing.Size(173, 16);
      this.tbMouseWheel.TabIndex = 3;
      this.tbMouseWheel.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbMouseWheel.TickStyle = TickStyle.BottomRight;
      this.tbMouseWheel.Value = 5;
      this.groupOverlays.Controls.Add((Control) this.panelReaderOverlays);
      this.groupOverlays.Controls.Add((Control) this.cbNavigationOverlayPosition);
      this.groupOverlays.Controls.Add((Control) this.labelNavigationOverlayPosition);
      this.groupOverlays.Controls.Add((Control) this.chkShowPageNames);
      this.groupOverlays.Controls.Add((Control) this.tbOverlayScaling);
      this.groupOverlays.Controls.Add((Control) this.chkShowCurrentPageOverlay);
      this.groupOverlays.Controls.Add((Control) this.chkShowStatusOverlay);
      this.groupOverlays.Controls.Add((Control) this.chkShowVisiblePartOverlay);
      this.groupOverlays.Controls.Add((Control) this.chkShowNavigationOverlay);
      this.groupOverlays.Controls.Add((Control) this.labelOverlaySize);
      this.groupOverlays.Dock = DockStyle.Top;
      this.groupOverlays.Location = new System.Drawing.Point(0, 690);
      this.groupOverlays.Name = "groupOverlays";
      this.groupOverlays.Size = new System.Drawing.Size(498, 356);
      this.groupOverlays.TabIndex = 2;
      this.groupOverlays.Text = "Overlays";
      this.panelReaderOverlays.BackColor = Color.WhiteSmoke;
      this.panelReaderOverlays.BorderStyle = BorderStyle.FixedSingle;
      this.panelReaderOverlays.Controls.Add((Control) this.labelVisiblePartOverlay);
      this.panelReaderOverlays.Controls.Add((Control) this.labelNavigationOverlay);
      this.panelReaderOverlays.Controls.Add((Control) this.labelStatusOverlay);
      this.panelReaderOverlays.Controls.Add((Control) this.labelPageOverlay);
      this.panelReaderOverlays.Location = new System.Drawing.Point(118, 39);
      this.panelReaderOverlays.Name = "panelReaderOverlays";
      this.panelReaderOverlays.Size = new System.Drawing.Size(258, 134);
      this.panelReaderOverlays.TabIndex = 8;
      this.labelVisiblePartOverlay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.labelVisiblePartOverlay.BackColor = Color.Gainsboro;
      this.labelVisiblePartOverlay.BorderStyle = BorderStyle.FixedSingle;
      this.labelVisiblePartOverlay.FlatStyle = FlatStyle.Popup;
      this.labelVisiblePartOverlay.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.labelVisiblePartOverlay.Location = new System.Drawing.Point(204, 75);
      this.labelVisiblePartOverlay.Name = "labelVisiblePartOverlay";
      this.labelVisiblePartOverlay.Size = new System.Drawing.Size(49, 51);
      this.labelVisiblePartOverlay.TabIndex = 3;
      this.labelVisiblePartOverlay.Text = "Visible Part";
      this.labelVisiblePartOverlay.TextAlign = ContentAlignment.MiddleCenter;
      this.labelVisiblePartOverlay.UseMnemonic = false;
      this.labelNavigationOverlay.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.labelNavigationOverlay.BackColor = Color.Gainsboro;
      this.labelNavigationOverlay.BorderStyle = BorderStyle.FixedSingle;
      this.labelNavigationOverlay.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.labelNavigationOverlay.Location = new System.Drawing.Point(55, 100);
      this.labelNavigationOverlay.Name = "labelNavigationOverlay";
      this.labelNavigationOverlay.Size = new System.Drawing.Size(143, 26);
      this.labelNavigationOverlay.TabIndex = 2;
      this.labelNavigationOverlay.Text = "Navigation";
      this.labelNavigationOverlay.TextAlign = ContentAlignment.MiddleCenter;
      this.labelNavigationOverlay.UseMnemonic = false;
      this.labelStatusOverlay.BackColor = Color.Gainsboro;
      this.labelStatusOverlay.BorderStyle = BorderStyle.FixedSingle;
      this.labelStatusOverlay.FlatStyle = FlatStyle.Popup;
      this.labelStatusOverlay.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.labelStatusOverlay.Location = new System.Drawing.Point(60, 49);
      this.labelStatusOverlay.Name = "labelStatusOverlay";
      this.labelStatusOverlay.Size = new System.Drawing.Size(134, 26);
      this.labelStatusOverlay.TabIndex = 1;
      this.labelStatusOverlay.Text = "Messages and Status";
      this.labelStatusOverlay.TextAlign = ContentAlignment.MiddleCenter;
      this.labelStatusOverlay.UseMnemonic = false;
      this.labelPageOverlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.labelPageOverlay.BackColor = Color.Gainsboro;
      this.labelPageOverlay.BorderStyle = BorderStyle.FixedSingle;
      this.labelPageOverlay.FlatStyle = FlatStyle.Popup;
      this.labelPageOverlay.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.labelPageOverlay.Location = new System.Drawing.Point(204, 3);
      this.labelPageOverlay.Name = "labelPageOverlay";
      this.labelPageOverlay.Size = new System.Drawing.Size(49, 36);
      this.labelPageOverlay.TabIndex = 0;
      this.labelPageOverlay.Text = "Page";
      this.labelPageOverlay.TextAlign = ContentAlignment.MiddleCenter;
      this.labelPageOverlay.UseMnemonic = false;
      this.cbNavigationOverlayPosition.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbNavigationOverlayPosition.FormattingEnabled = true;
      this.cbNavigationOverlayPosition.Items.AddRange(new object[2]
      {
        (object) "at Bottom",
        (object) "on Top"
      });
      this.cbNavigationOverlayPosition.Location = new System.Drawing.Point(84, 313);
      this.cbNavigationOverlayPosition.Name = "cbNavigationOverlayPosition";
      this.cbNavigationOverlayPosition.Size = new System.Drawing.Size(121, 21);
      this.cbNavigationOverlayPosition.TabIndex = 6;
      this.labelNavigationOverlayPosition.AutoSize = true;
      this.labelNavigationOverlayPosition.Location = new System.Drawing.Point(18, 316);
      this.labelNavigationOverlayPosition.Name = "labelNavigationOverlayPosition";
      this.labelNavigationOverlayPosition.Size = new System.Drawing.Size(61, 13);
      this.labelNavigationOverlayPosition.TabIndex = 5;
      this.labelNavigationOverlayPosition.Text = "Navigation:";
      this.chkShowPageNames.AutoSize = true;
      this.chkShowPageNames.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.chkShowPageNames.Location = new System.Drawing.Point(283, 193);
      this.chkShowPageNames.Name = "chkShowPageNames";
      this.chkShowPageNames.Size = new System.Drawing.Size(181, 17);
      this.chkShowPageNames.TabIndex = 4;
      this.chkShowPageNames.Text = "Current Page also displays Name";
      this.chkShowPageNames.UseVisualStyleBackColor = true;
      this.tbOverlayScaling.Location = new System.Drawing.Point(288, 316);
      this.tbOverlayScaling.Maximum = 150;
      this.tbOverlayScaling.Minimum = 40;
      this.tbOverlayScaling.Name = "tbOverlayScaling";
      this.tbOverlayScaling.Size = new System.Drawing.Size(184, 16);
      this.tbOverlayScaling.TabIndex = 8;
      this.tbOverlayScaling.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbOverlayScaling.TickStyle = TickStyle.BottomRight;
      this.tbOverlayScaling.Value = 50;
      this.tbOverlayScaling.ValueChanged += new EventHandler(this.tbOverlayScalingChanged);
      this.chkShowCurrentPageOverlay.AutoSize = true;
      this.chkShowCurrentPageOverlay.Location = new System.Drawing.Point(58, 193);
      this.chkShowCurrentPageOverlay.Name = "chkShowCurrentPageOverlay";
      this.chkShowCurrentPageOverlay.Size = new System.Drawing.Size(117, 17);
      this.chkShowCurrentPageOverlay.TabIndex = 0;
      this.chkShowCurrentPageOverlay.Text = "Show current Page";
      this.chkShowCurrentPageOverlay.UseVisualStyleBackColor = true;
      this.chkShowStatusOverlay.AutoSize = true;
      this.chkShowStatusOverlay.Location = new System.Drawing.Point(58, 217);
      this.chkShowStatusOverlay.Name = "chkShowStatusOverlay";
      this.chkShowStatusOverlay.Size = new System.Drawing.Size(158, 17);
      this.chkShowStatusOverlay.TabIndex = 1;
      this.chkShowStatusOverlay.Text = "Show Messages and Status";
      this.chkShowStatusOverlay.UseVisualStyleBackColor = true;
      this.chkShowVisiblePartOverlay.AutoSize = true;
      this.chkShowVisiblePartOverlay.Location = new System.Drawing.Point(58, 241);
      this.chkShowVisiblePartOverlay.Name = "chkShowVisiblePartOverlay";
      this.chkShowVisiblePartOverlay.Size = new System.Drawing.Size(135, 17);
      this.chkShowVisiblePartOverlay.TabIndex = 2;
      this.chkShowVisiblePartOverlay.Text = "Show visible Page Part";
      this.chkShowVisiblePartOverlay.UseVisualStyleBackColor = true;
      this.chkShowNavigationOverlay.AutoSize = true;
      this.chkShowNavigationOverlay.Location = new System.Drawing.Point(58, 264);
      this.chkShowNavigationOverlay.Name = "chkShowNavigationOverlay";
      this.chkShowNavigationOverlay.Size = new System.Drawing.Size(171, 17);
      this.chkShowNavigationOverlay.TabIndex = 3;
      this.chkShowNavigationOverlay.Text = "Show Navigation automatically";
      this.chkShowNavigationOverlay.UseVisualStyleBackColor = true;
      this.labelOverlaySize.AutoSize = true;
      this.labelOverlaySize.Location = new System.Drawing.Point(244, 316);
      this.labelOverlaySize.Name = "labelOverlaySize";
      this.labelOverlaySize.Size = new System.Drawing.Size(38, 13);
      this.labelOverlaySize.TabIndex = 7;
      this.labelOverlaySize.Text = "Sizing:";
      this.grpKeyboard.Controls.Add((Control) this.btExportKeyboard);
      this.grpKeyboard.Controls.Add((Control) this.btImportKeyboard);
      this.grpKeyboard.Controls.Add((Control) this.keyboardShortcutEditor);
      this.grpKeyboard.Dock = DockStyle.Top;
      this.grpKeyboard.Location = new System.Drawing.Point(0, 300);
      this.grpKeyboard.Name = "grpKeyboard";
      this.grpKeyboard.Size = new System.Drawing.Size(498, 390);
      this.grpKeyboard.TabIndex = 4;
      this.grpKeyboard.Text = "Keyboard";
      this.btExportKeyboard.Location = new System.Drawing.Point(274, 357);
      this.btExportKeyboard.Name = "btExportKeyboard";
      this.btExportKeyboard.Size = new System.Drawing.Size(102, 23);
      this.btExportKeyboard.TabIndex = 1;
      this.btExportKeyboard.Text = "Export...";
      this.btExportKeyboard.UseVisualStyleBackColor = true;
      this.btExportKeyboard.Click += new EventHandler(this.btExportKeyboard_Click);
      this.btImportKeyboard.ContextMenuStrip = this.cmKeyboardLayout;
      this.btImportKeyboard.Location = new System.Drawing.Point(382, 357);
      this.btImportKeyboard.Name = "btImportKeyboard";
      this.btImportKeyboard.Size = new System.Drawing.Size(102, 23);
      this.btImportKeyboard.TabIndex = 2;
      this.btImportKeyboard.Text = "Import...";
      this.btImportKeyboard.UseVisualStyleBackColor = true;
      this.btImportKeyboard.ShowContextMenu += new EventHandler(this.btImportKeyboard_ShowContextMenu);
      this.btImportKeyboard.Click += new EventHandler(this.btLoadKeyboard_Click);
      this.cmKeyboardLayout.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.miDefaultKeyboardLayout,
        (ToolStripItem) this.toolStripMenuItem1
      });
      this.cmKeyboardLayout.Name = "cmKeyboardLayout";
      this.cmKeyboardLayout.Size = new System.Drawing.Size(113, 32);
      this.miDefaultKeyboardLayout.Name = "miDefaultKeyboardLayout";
      this.miDefaultKeyboardLayout.Size = new System.Drawing.Size(112, 22);
      this.miDefaultKeyboardLayout.Text = "&Default";
      this.miDefaultKeyboardLayout.Click += new EventHandler(this.miDefaultKeyboardLayout_Click);
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(109, 6);
      this.keyboardShortcutEditor.AllowDrop = true;
      this.keyboardShortcutEditor.Location = new System.Drawing.Point(12, 37);
      this.keyboardShortcutEditor.Name = "keyboardShortcutEditor";
      this.keyboardShortcutEditor.Shortcuts = (KeyboardShortcuts) null;
      this.keyboardShortcutEditor.Size = new System.Drawing.Size(472, 314);
      this.keyboardShortcutEditor.TabIndex = 0;
      this.keyboardShortcutEditor.DragDrop += new DragEventHandler(this.keyboardShortcutEditor_DragDrop);
      this.keyboardShortcutEditor.DragOver += new DragEventHandler(this.keyboardShortcutEditor_DragOver);
      this.grpDisplay.Controls.Add((Control) this.tbGamma);
      this.grpDisplay.Controls.Add((Control) this.labelGamma);
      this.grpDisplay.Controls.Add((Control) this.chkAnamorphicScaling);
      this.grpDisplay.Controls.Add((Control) this.chkHighQualityDisplay);
      this.grpDisplay.Controls.Add((Control) this.labelSharpening);
      this.grpDisplay.Controls.Add((Control) this.tbSharpening);
      this.grpDisplay.Controls.Add((Control) this.btResetColor);
      this.grpDisplay.Controls.Add((Control) this.chkAutoContrast);
      this.grpDisplay.Controls.Add((Control) this.labelSaturation);
      this.grpDisplay.Controls.Add((Control) this.tbSaturation);
      this.grpDisplay.Controls.Add((Control) this.labelBrightness);
      this.grpDisplay.Controls.Add((Control) this.tbBrightness);
      this.grpDisplay.Controls.Add((Control) this.tbContrast);
      this.grpDisplay.Controls.Add((Control) this.labelContrast);
      this.grpDisplay.Dock = DockStyle.Top;
      this.grpDisplay.Location = new System.Drawing.Point(0, 0);
      this.grpDisplay.Name = "grpDisplay";
      this.grpDisplay.Size = new System.Drawing.Size(498, 300);
      this.grpDisplay.TabIndex = 1;
      this.grpDisplay.Text = "Display";
      this.tbGamma.Location = new System.Drawing.Point(150, 193);
      this.tbGamma.Minimum = -100;
      this.tbGamma.Name = "tbGamma";
      this.tbGamma.Size = new System.Drawing.Size(332, 16);
      this.tbGamma.TabIndex = 12;
      this.tbGamma.Text = "tbSaturation";
      this.tbGamma.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbGamma.TickStyle = TickStyle.BottomRight;
      this.tbGamma.ValueChanged += new EventHandler(this.tbColorAdjustmentChanged);
      this.tbGamma.DoubleClick += new EventHandler(this.tbGamma_DoubleClick);
      this.labelGamma.Location = new System.Drawing.Point(14, 193);
      this.labelGamma.Name = "labelGamma";
      this.labelGamma.Size = new System.Drawing.Size(133, 13);
      this.labelGamma.TabIndex = 11;
      this.labelGamma.Text = "Gamma Adjustment:";
      this.labelGamma.TextAlign = ContentAlignment.TopRight;
      this.chkAnamorphicScaling.AutoSize = true;
      this.chkAnamorphicScaling.Location = new System.Drawing.Point(12, 60);
      this.chkAnamorphicScaling.Name = "chkAnamorphicScaling";
      this.chkAnamorphicScaling.Size = new System.Drawing.Size(120, 17);
      this.chkAnamorphicScaling.TabIndex = 0;
      this.chkAnamorphicScaling.Text = "&Anamorphic Scaling";
      this.chkAnamorphicScaling.UseVisualStyleBackColor = true;
      this.chkHighQualityDisplay.AutoSize = true;
      this.chkHighQualityDisplay.Location = new System.Drawing.Point(12, 37);
      this.chkHighQualityDisplay.Name = "chkHighQualityDisplay";
      this.chkHighQualityDisplay.Size = new System.Drawing.Size(83, 17);
      this.chkHighQualityDisplay.TabIndex = 0;
      this.chkHighQualityDisplay.Text = "&High Quality";
      this.chkHighQualityDisplay.UseVisualStyleBackColor = true;
      this.labelSharpening.Location = new System.Drawing.Point(17, 225);
      this.labelSharpening.Name = "labelSharpening";
      this.labelSharpening.Size = new System.Drawing.Size(132, 13);
      this.labelSharpening.TabIndex = 8;
      this.labelSharpening.Text = "Sharpening:";
      this.labelSharpening.TextAlign = ContentAlignment.TopRight;
      this.tbSharpening.LargeChange = 1;
      this.tbSharpening.Location = new System.Drawing.Point(149, 225);
      this.tbSharpening.Maximum = 3;
      this.tbSharpening.Name = "tbSharpening";
      this.tbSharpening.Size = new System.Drawing.Size(333, 18);
      this.tbSharpening.TabIndex = 9;
      this.tbSharpening.Text = "tbSaturation";
      this.tbSharpening.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbSharpening.TickFrequency = 1;
      this.tbSharpening.TickStyle = TickStyle.BottomRight;
      this.tbSharpening.DoubleClick += new EventHandler(this.tbSharpening_DoubleClick);
      this.btResetColor.Location = new System.Drawing.Point(394, 265);
      this.btResetColor.Name = "btResetColor";
      this.btResetColor.Size = new System.Drawing.Size(91, 23);
      this.btResetColor.TabIndex = 10;
      this.btResetColor.Text = "&Reset";
      this.btResetColor.UseVisualStyleBackColor = true;
      this.btResetColor.Click += new EventHandler(this.btReset_Click);
      this.chkAutoContrast.AutoSize = true;
      this.chkAutoContrast.Location = new System.Drawing.Point(12, 95);
      this.chkAutoContrast.Name = "chkAutoContrast";
      this.chkAutoContrast.Size = new System.Drawing.Size(184, 17);
      this.chkAutoContrast.TabIndex = 1;
      this.chkAutoContrast.Text = "Automatic &Contrast Enhancement";
      this.chkAutoContrast.UseVisualStyleBackColor = true;
      this.labelSaturation.Location = new System.Drawing.Point(11, 122);
      this.labelSaturation.Name = "labelSaturation";
      this.labelSaturation.Size = new System.Drawing.Size(136, 13);
      this.labelSaturation.TabIndex = 2;
      this.labelSaturation.Text = "Saturation Adjustment:";
      this.labelSaturation.TextAlign = ContentAlignment.TopRight;
      this.tbSaturation.Location = new System.Drawing.Point(148, 122);
      this.tbSaturation.Minimum = -100;
      this.tbSaturation.Name = "tbSaturation";
      this.tbSaturation.Size = new System.Drawing.Size(334, 16);
      this.tbSaturation.TabIndex = 3;
      this.tbSaturation.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbSaturation.TickStyle = TickStyle.BottomRight;
      this.tbSaturation.ValueChanged += new EventHandler(this.tbColorAdjustmentChanged);
      this.tbSaturation.DoubleClick += new EventHandler(this.tbSaturation_DoubleClick);
      this.labelBrightness.Location = new System.Drawing.Point(14, 144);
      this.labelBrightness.Name = "labelBrightness";
      this.labelBrightness.Size = new System.Drawing.Size(133, 13);
      this.labelBrightness.TabIndex = 4;
      this.labelBrightness.Text = "Brightness Adjustment:";
      this.labelBrightness.TextAlign = ContentAlignment.TopRight;
      this.tbBrightness.Location = new System.Drawing.Point(148, 144);
      this.tbBrightness.Minimum = -100;
      this.tbBrightness.Name = "tbBrightness";
      this.tbBrightness.Size = new System.Drawing.Size(334, 16);
      this.tbBrightness.TabIndex = 5;
      this.tbBrightness.Text = "tbBrightness";
      this.tbBrightness.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbBrightness.TickStyle = TickStyle.BottomRight;
      this.tbBrightness.ValueChanged += new EventHandler(this.tbColorAdjustmentChanged);
      this.tbBrightness.DoubleClick += new EventHandler(this.tbBrightness_DoubleClick);
      this.tbContrast.Location = new System.Drawing.Point(148, 168);
      this.tbContrast.Minimum = -100;
      this.tbContrast.Name = "tbContrast";
      this.tbContrast.Size = new System.Drawing.Size(334, 16);
      this.tbContrast.TabIndex = 7;
      this.tbContrast.Text = "tbSaturation";
      this.tbContrast.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbContrast.TickStyle = TickStyle.BottomRight;
      this.tbContrast.ValueChanged += new EventHandler(this.tbColorAdjustmentChanged);
      this.tbContrast.DoubleClick += new EventHandler(this.tbContrast_DoubleClick);
      this.labelContrast.Location = new System.Drawing.Point(14, 168);
      this.labelContrast.Name = "labelContrast";
      this.labelContrast.Size = new System.Drawing.Size(133, 13);
      this.labelContrast.TabIndex = 6;
      this.labelContrast.Text = "Contrast Adjustment:";
      this.labelContrast.TextAlign = ContentAlignment.TopRight;
      this.pageAdvanced.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pageAdvanced.AutoScroll = true;
      this.pageAdvanced.BorderStyle = BorderStyle.FixedSingle;
      this.pageAdvanced.Controls.Add((Control) this.grpWirelessSetup);
      this.pageAdvanced.Controls.Add((Control) this.grpIntegration);
      this.pageAdvanced.Controls.Add((Control) this.groupMessagesAndSocial);
      this.pageAdvanced.Controls.Add((Control) this.groupMemory);
      this.pageAdvanced.Controls.Add((Control) this.grpDatabaseBackup);
      this.pageAdvanced.Controls.Add((Control) this.groupOtherComics);
      this.pageAdvanced.Controls.Add((Control) this.grpLanguages);
      this.pageAdvanced.Location = new System.Drawing.Point(84, 8);
      this.pageAdvanced.Name = "pageAdvanced";
      this.pageAdvanced.Size = new System.Drawing.Size(517, 408);
      this.pageAdvanced.TabIndex = 9;
      this.grpWirelessSetup.Controls.Add((Control) this.btTestWifi);
      this.grpWirelessSetup.Controls.Add((Control) this.lblWifiStatus);
      this.grpWirelessSetup.Controls.Add((Control) this.lblWifiAddresses);
      this.grpWirelessSetup.Controls.Add((Control) this.txWifiAddresses);
      this.grpWirelessSetup.Dock = DockStyle.Top;
      this.grpWirelessSetup.Location = new System.Drawing.Point(0, 1437);
      this.grpWirelessSetup.Name = "grpWirelessSetup";
      this.grpWirelessSetup.Size = new System.Drawing.Size(498, 136);
      this.grpWirelessSetup.TabIndex = 8;
      this.grpWirelessSetup.Text = "Wireless Setup";
      this.btTestWifi.Location = new System.Drawing.Point(382, 63);
      this.btTestWifi.Name = "btTestWifi";
      this.btTestWifi.Size = new System.Drawing.Size(104, 23);
      this.btTestWifi.TabIndex = 3;
      this.btTestWifi.Text = "Test";
      this.btTestWifi.UseVisualStyleBackColor = true;
      this.btTestWifi.Click += new EventHandler(this.btTestWifi_Click);
      this.lblWifiStatus.Location = new System.Drawing.Point(6, 93);
      this.lblWifiStatus.Name = "lblWifiStatus";
      this.lblWifiStatus.Size = new System.Drawing.Size(370, 21);
      this.lblWifiStatus.TabIndex = 2;
      this.lblWifiStatus.TextAlign = ContentAlignment.MiddleCenter;
      this.lblWifiAddresses.AutoSize = true;
      this.lblWifiAddresses.Location = new System.Drawing.Point(4, 41);
      this.lblWifiAddresses.Name = "lblWifiAddresses";
      this.lblWifiAddresses.Size = new System.Drawing.Size(490, 13);
      this.lblWifiAddresses.TabIndex = 1;
      this.lblWifiAddresses.Text = "Semicolon separated list of IP addresses for Wireless Devices which where not detected automatically:";
      this.txWifiAddresses.Location = new System.Drawing.Point(6, 65);
      this.txWifiAddresses.Name = "txWifiAddresses";
      this.txWifiAddresses.Size = new System.Drawing.Size(370, 20);
      this.txWifiAddresses.TabIndex = 0;
      this.grpIntegration.Controls.Add((Control) this.btAssociateExtensions);
      this.grpIntegration.Controls.Add((Control) this.labelCheckedFormats);
      this.grpIntegration.Controls.Add((Control) this.chkOverwriteAssociations);
      this.grpIntegration.Controls.Add((Control) this.lbFormats);
      this.grpIntegration.Dock = DockStyle.Top;
      this.grpIntegration.Location = new System.Drawing.Point(0, 1097);
      this.grpIntegration.Name = "grpIntegration";
      this.grpIntegration.Size = new System.Drawing.Size(498, 340);
      this.grpIntegration.TabIndex = 0;
      this.grpIntegration.Text = "Explorer Integration";
      this.btAssociateExtensions.Location = new System.Drawing.Point(382, 57);
      this.btAssociateExtensions.Name = "btAssociateExtensions";
      this.btAssociateExtensions.Size = new System.Drawing.Size(104, 23);
      this.btAssociateExtensions.TabIndex = 4;
      this.btAssociateExtensions.Text = "Change...";
      this.btAssociateExtensions.UseVisualStyleBackColor = true;
      this.btAssociateExtensions.Click += new EventHandler(this.btAssociateExtensions_Click);
      this.labelCheckedFormats.AutoSize = true;
      this.labelCheckedFormats.Location = new System.Drawing.Point(3, 35);
      this.labelCheckedFormats.Name = "labelCheckedFormats";
      this.labelCheckedFormats.Size = new System.Drawing.Size(253, 13);
      this.labelCheckedFormats.TabIndex = 0;
      this.labelCheckedFormats.Text = "Checked formats will be associated with ComicRack";
      this.chkOverwriteAssociations.AutoSize = true;
      this.chkOverwriteAssociations.CheckAlign = ContentAlignment.TopLeft;
      this.chkOverwriteAssociations.Location = new System.Drawing.Point(6, 307);
      this.chkOverwriteAssociations.Name = "chkOverwriteAssociations";
      this.chkOverwriteAssociations.Size = new System.Drawing.Size(289, 17);
      this.chkOverwriteAssociations.TabIndex = 2;
      this.chkOverwriteAssociations.Text = "Overwrite existing associations instead of 'Open With ...'";
      this.chkOverwriteAssociations.TextAlign = ContentAlignment.TopLeft;
      this.chkOverwriteAssociations.UseVisualStyleBackColor = true;
      this.lbFormats.CheckOnClick = true;
      this.lbFormats.FormattingEnabled = true;
      this.lbFormats.Location = new System.Drawing.Point(6, 57);
      this.lbFormats.Name = "lbFormats";
      this.lbFormats.Size = new System.Drawing.Size(371, 244);
      this.lbFormats.TabIndex = 1;
      this.groupMessagesAndSocial.Controls.Add((Control) this.btResetTwitter);
      this.groupMessagesAndSocial.Controls.Add((Control) this.labelResetTwitter);
      this.groupMessagesAndSocial.Controls.Add((Control) this.btResetMessages);
      this.groupMessagesAndSocial.Controls.Add((Control) this.labelReshowHidden);
      this.groupMessagesAndSocial.Dock = DockStyle.Top;
      this.groupMessagesAndSocial.Location = new System.Drawing.Point(0, 996);
      this.groupMessagesAndSocial.Name = "groupMessagesAndSocial";
      this.groupMessagesAndSocial.Size = new System.Drawing.Size(498, 101);
      this.groupMessagesAndSocial.TabIndex = 6;
      this.groupMessagesAndSocial.Text = "Messages and Social";
      this.btResetMessages.Location = new System.Drawing.Point(382, 41);
      this.btResetMessages.Name = "btResetMessages";
      this.btResetMessages.Size = new System.Drawing.Size(104, 23);
      this.btResetMessages.TabIndex = 1;
      this.btResetMessages.Text = "Reset";
      this.btResetMessages.UseVisualStyleBackColor = true;
      this.btResetMessages.Click += new EventHandler(this.btResetMessages_Click);
      this.labelReshowHidden.Location = new System.Drawing.Point(6, 46);
      this.labelReshowHidden.Name = "labelReshowHidden";
      this.labelReshowHidden.Size = new System.Drawing.Size(370, 17);
      this.labelReshowHidden.TabIndex = 0;
      this.labelReshowHidden.Text = "To reshow hidden messages press";
      this.groupMemory.Controls.Add((Control) this.grpMaximumMemoryUsage);
      this.groupMemory.Controls.Add((Control) this.grpMemoryCache);
      this.groupMemory.Controls.Add((Control) this.grpDiskCache);
      this.groupMemory.Dock = DockStyle.Top;
      this.groupMemory.Location = new System.Drawing.Point(0, 641);
      this.groupMemory.Name = "groupMemory";
      this.groupMemory.Size = new System.Drawing.Size(498, 355);
      this.groupMemory.TabIndex = 1;
      this.groupMemory.Text = "Caches & Memory Usage";
      this.grpMaximumMemoryUsage.Controls.Add((Control) this.lblMaximumMemoryUsageValue);
      this.grpMaximumMemoryUsage.Controls.Add((Control) this.tbMaximumMemoryUsage);
      this.grpMaximumMemoryUsage.Controls.Add((Control) this.lblMaximumMemoryUsage);
      this.grpMaximumMemoryUsage.Location = new System.Drawing.Point(7, (int) byte.MaxValue);
      this.grpMaximumMemoryUsage.Name = "grpMaximumMemoryUsage";
      this.grpMaximumMemoryUsage.Size = new System.Drawing.Size(476, 86);
      this.grpMaximumMemoryUsage.TabIndex = 14;
      this.grpMaximumMemoryUsage.TabStop = false;
      this.grpMaximumMemoryUsage.Text = "Maximum Memory Usage";
      this.lblMaximumMemoryUsageValue.AutoSize = true;
      this.lblMaximumMemoryUsageValue.Location = new System.Drawing.Point(397, 31);
      this.lblMaximumMemoryUsageValue.Name = "lblMaximumMemoryUsageValue";
      this.lblMaximumMemoryUsageValue.Size = new System.Drawing.Size(63, 13);
      this.lblMaximumMemoryUsageValue.TabIndex = 2;
      this.lblMaximumMemoryUsageValue.Text = "Slider Value";
      this.tbMaximumMemoryUsage.LargeChange = 4;
      this.tbMaximumMemoryUsage.Location = new System.Drawing.Point(7, 24);
      this.tbMaximumMemoryUsage.Maximum = 64;
      this.tbMaximumMemoryUsage.Name = "tbMaximumMemoryUsage";
      this.tbMaximumMemoryUsage.Size = new System.Drawing.Size(379, 29);
      this.tbMaximumMemoryUsage.TabIndex = 1;
      this.tbMaximumMemoryUsage.ThumbSize = new System.Drawing.Size(10, 20);
      this.tbMaximumMemoryUsage.TickFrequency = 8;
      this.tbMaximumMemoryUsage.TickStyle = TickStyle.BottomRight;
      this.tbMaximumMemoryUsage.TickThickness = 2;
      this.tbMaximumMemoryUsage.ValueChanged += new EventHandler(this.tbSystemMemory_ValueChanged);
      this.lblMaximumMemoryUsage.Dock = DockStyle.Bottom;
      this.lblMaximumMemoryUsage.Location = new System.Drawing.Point(3, 58);
      this.lblMaximumMemoryUsage.Name = "lblMaximumMemoryUsage";
      this.lblMaximumMemoryUsage.Size = new System.Drawing.Size(470, 25);
      this.lblMaximumMemoryUsage.TabIndex = 0;
      this.lblMaximumMemoryUsage.Text = "Limiting the memory can adversely affect the performance.";
      this.lblMaximumMemoryUsage.TextAlign = ContentAlignment.MiddleCenter;
      this.grpMemoryCache.Controls.Add((Control) this.lblPageMemCacheUsage);
      this.grpMemoryCache.Controls.Add((Control) this.labelMemThumbSize);
      this.grpMemoryCache.Controls.Add((Control) this.lblThumbMemCacheUsage);
      this.grpMemoryCache.Controls.Add((Control) this.numMemPageCount);
      this.grpMemoryCache.Controls.Add((Control) this.labelMemPageCount);
      this.grpMemoryCache.Controls.Add((Control) this.chkMemPageOptimized);
      this.grpMemoryCache.Controls.Add((Control) this.chkMemThumbOptimized);
      this.grpMemoryCache.Controls.Add((Control) this.numMemThumbSize);
      this.grpMemoryCache.Location = new System.Drawing.Point(6, 162);
      this.grpMemoryCache.Name = "grpMemoryCache";
      this.grpMemoryCache.Size = new System.Drawing.Size(476, 85);
      this.grpMemoryCache.TabIndex = 13;
      this.grpMemoryCache.TabStop = false;
      this.grpMemoryCache.Text = "Memory Cache";
      this.lblPageMemCacheUsage.AutoSize = true;
      this.lblPageMemCacheUsage.Location = new System.Drawing.Point(299, 52);
      this.lblPageMemCacheUsage.Name = "lblPageMemCacheUsage";
      this.lblPageMemCacheUsage.Size = new System.Drawing.Size(124, 13);
      this.lblPageMemCacheUsage.TabIndex = 8;
      this.lblPageMemCacheUsage.Text = "usage Page Mem Cache";
      this.labelMemThumbSize.AutoSize = true;
      this.labelMemThumbSize.Location = new System.Drawing.Point(19, 29);
      this.labelMemThumbSize.Name = "labelMemThumbSize";
      this.labelMemThumbSize.Size = new System.Drawing.Size(86, 13);
      this.labelMemThumbSize.TabIndex = 0;
      this.labelMemThumbSize.Text = "Thumbnails [MB]";
      this.lblThumbMemCacheUsage.AutoSize = true;
      this.lblThumbMemCacheUsage.Location = new System.Drawing.Point(299, 26);
      this.lblThumbMemCacheUsage.Name = "lblThumbMemCacheUsage";
      this.lblThumbMemCacheUsage.Size = new System.Drawing.Size(132, 13);
      this.lblThumbMemCacheUsage.TabIndex = 7;
      this.lblThumbMemCacheUsage.Text = "usage Thumb Mem Cache";
      this.numMemPageCount.Location = new System.Drawing.Point(145, 51);
      this.numMemPageCount.Maximum = new Decimal(new int[4]
      {
        25,
        0,
        0,
        0
      });
      this.numMemPageCount.Minimum = new Decimal(new int[4]
      {
        5,
        0,
        0,
        0
      });
      this.numMemPageCount.Name = "numMemPageCount";
      this.numMemPageCount.Size = new System.Drawing.Size(67, 20);
      this.numMemPageCount.TabIndex = 4;
      this.numMemPageCount.TextAlign = HorizontalAlignment.Right;
      this.numMemPageCount.Value = new Decimal(new int[4]
      {
        5,
        0,
        0,
        0
      });
      this.labelMemPageCount.AutoSize = true;
      this.labelMemPageCount.Location = new System.Drawing.Point(19, 52);
      this.labelMemPageCount.Name = "labelMemPageCount";
      this.labelMemPageCount.Size = new System.Drawing.Size(73, 13);
      this.labelMemPageCount.TabIndex = 3;
      this.labelMemPageCount.Text = "Pages [count]";
      this.chkMemPageOptimized.AutoSize = true;
      this.chkMemPageOptimized.CheckAlign = ContentAlignment.MiddleRight;
      this.chkMemPageOptimized.Location = new System.Drawing.Point(218, 51);
      this.chkMemPageOptimized.Name = "chkMemPageOptimized";
      this.chkMemPageOptimized.Size = new System.Drawing.Size(70, 17);
      this.chkMemPageOptimized.TabIndex = 5;
      this.chkMemPageOptimized.Text = "optimized";
      this.chkMemPageOptimized.UseVisualStyleBackColor = true;
      this.chkMemThumbOptimized.AutoSize = true;
      this.chkMemThumbOptimized.CheckAlign = ContentAlignment.MiddleRight;
      this.chkMemThumbOptimized.Location = new System.Drawing.Point(218, 25);
      this.chkMemThumbOptimized.Name = "chkMemThumbOptimized";
      this.chkMemThumbOptimized.Size = new System.Drawing.Size(70, 17);
      this.chkMemThumbOptimized.TabIndex = 2;
      this.chkMemThumbOptimized.Text = "optimized";
      this.chkMemThumbOptimized.UseVisualStyleBackColor = true;
      this.numMemThumbSize.Increment = new Decimal(new int[4]
      {
        5,
        0,
        0,
        0
      });
      this.numMemThumbSize.Location = new System.Drawing.Point(145, 24);
      this.numMemThumbSize.Minimum = new Decimal(new int[4]
      {
        20,
        0,
        0,
        0
      });
      this.numMemThumbSize.Name = "numMemThumbSize";
      this.numMemThumbSize.Size = new System.Drawing.Size(67, 20);
      this.numMemThumbSize.TabIndex = 1;
      this.numMemThumbSize.TextAlign = HorizontalAlignment.Right;
      this.numMemThumbSize.Value = new Decimal(new int[4]
      {
        25,
        0,
        0,
        0
      });
      this.grpDiskCache.Controls.Add((Control) this.chkEnableInternetCache);
      this.grpDiskCache.Controls.Add((Control) this.lblInternetCacheUsage);
      this.grpDiskCache.Controls.Add((Control) this.btClearPageCache);
      this.grpDiskCache.Controls.Add((Control) this.numPageCacheSize);
      this.grpDiskCache.Controls.Add((Control) this.numInternetCacheSize);
      this.grpDiskCache.Controls.Add((Control) this.btClearThumbnailCache);
      this.grpDiskCache.Controls.Add((Control) this.btClearInternetCache);
      this.grpDiskCache.Controls.Add((Control) this.chkEnablePageCache);
      this.grpDiskCache.Controls.Add((Control) this.lblPageCacheUsage);
      this.grpDiskCache.Controls.Add((Control) this.numThumbnailCacheSize);
      this.grpDiskCache.Controls.Add((Control) this.chkEnableThumbnailCache);
      this.grpDiskCache.Controls.Add((Control) this.lblThumbCacheUsage);
      this.grpDiskCache.Location = new System.Drawing.Point(6, 35);
      this.grpDiskCache.Name = "grpDiskCache";
      this.grpDiskCache.Size = new System.Drawing.Size(476, 120);
      this.grpDiskCache.TabIndex = 12;
      this.grpDiskCache.TabStop = false;
      this.grpDiskCache.Text = "Disk Cache";
      this.chkEnableInternetCache.AutoSize = true;
      this.chkEnableInternetCache.Location = new System.Drawing.Point(22, 31);
      this.chkEnableInternetCache.Name = "chkEnableInternetCache";
      this.chkEnableInternetCache.Size = new System.Drawing.Size(87, 17);
      this.chkEnableInternetCache.TabIndex = 0;
      this.chkEnableInternetCache.Text = "Internet [MB]";
      this.chkEnableInternetCache.UseVisualStyleBackColor = true;
      this.lblInternetCacheUsage.AutoSize = true;
      this.lblInternetCacheUsage.Location = new System.Drawing.Point(298, 31);
      this.lblInternetCacheUsage.Name = "lblInternetCacheUsage";
      this.lblInternetCacheUsage.Size = new System.Drawing.Size(109, 13);
      this.lblInternetCacheUsage.TabIndex = 3;
      this.lblInternetCacheUsage.Text = "usage Internet Cache";
      this.btClearPageCache.Location = new System.Drawing.Point(218, 80);
      this.btClearPageCache.Name = "btClearPageCache";
      this.btClearPageCache.Size = new System.Drawing.Size(74, 21);
      this.btClearPageCache.TabIndex = 10;
      this.btClearPageCache.Text = "Clear";
      this.btClearPageCache.UseVisualStyleBackColor = true;
      this.btClearPageCache.Click += new EventHandler(this.btClearPageCache_Click);
      this.numPageCacheSize.Increment = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.numPageCacheSize.Location = new System.Drawing.Point(145, 82);
      this.numPageCacheSize.Maximum = new Decimal(new int[4]
      {
        1000000,
        0,
        0,
        0
      });
      this.numPageCacheSize.Minimum = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.numPageCacheSize.Name = "numPageCacheSize";
      this.numPageCacheSize.Size = new System.Drawing.Size(67, 20);
      this.numPageCacheSize.TabIndex = 9;
      this.numPageCacheSize.TextAlign = HorizontalAlignment.Right;
      this.numPageCacheSize.Value = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.numInternetCacheSize.Increment = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.numInternetCacheSize.Location = new System.Drawing.Point(145, 29);
      this.numInternetCacheSize.Maximum = new Decimal(new int[4]
      {
        1000000,
        0,
        0,
        0
      });
      this.numInternetCacheSize.Minimum = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.numInternetCacheSize.Name = "numInternetCacheSize";
      this.numInternetCacheSize.Size = new System.Drawing.Size(67, 20);
      this.numInternetCacheSize.TabIndex = 1;
      this.numInternetCacheSize.TextAlign = HorizontalAlignment.Right;
      this.numInternetCacheSize.Value = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.btClearThumbnailCache.Location = new System.Drawing.Point(218, 54);
      this.btClearThumbnailCache.Name = "btClearThumbnailCache";
      this.btClearThumbnailCache.Size = new System.Drawing.Size(74, 21);
      this.btClearThumbnailCache.TabIndex = 6;
      this.btClearThumbnailCache.Text = "Clear";
      this.btClearThumbnailCache.UseVisualStyleBackColor = true;
      this.btClearThumbnailCache.Click += new EventHandler(this.btClearThumbnailCache_Click);
      this.btClearInternetCache.Location = new System.Drawing.Point(218, 27);
      this.btClearInternetCache.Name = "btClearInternetCache";
      this.btClearInternetCache.Size = new System.Drawing.Size(74, 21);
      this.btClearInternetCache.TabIndex = 2;
      this.btClearInternetCache.Text = "Clear";
      this.btClearInternetCache.UseVisualStyleBackColor = true;
      this.btClearInternetCache.Click += new EventHandler(this.btClearInternetCache_Click);
      this.chkEnablePageCache.AutoSize = true;
      this.chkEnablePageCache.Location = new System.Drawing.Point(22, 84);
      this.chkEnablePageCache.Name = "chkEnablePageCache";
      this.chkEnablePageCache.Size = new System.Drawing.Size(81, 17);
      this.chkEnablePageCache.TabIndex = 8;
      this.chkEnablePageCache.Text = "&Pages [MB]";
      this.chkEnablePageCache.UseVisualStyleBackColor = true;
      this.lblPageCacheUsage.AutoSize = true;
      this.lblPageCacheUsage.Location = new System.Drawing.Point(298, 86);
      this.lblPageCacheUsage.Name = "lblPageCacheUsage";
      this.lblPageCacheUsage.Size = new System.Drawing.Size(98, 13);
      this.lblPageCacheUsage.TabIndex = 11;
      this.lblPageCacheUsage.Text = "usage Page Cache";
      this.numThumbnailCacheSize.Increment = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.numThumbnailCacheSize.Location = new System.Drawing.Point(145, 55);
      this.numThumbnailCacheSize.Maximum = new Decimal(new int[4]
      {
        1000000,
        0,
        0,
        0
      });
      this.numThumbnailCacheSize.Minimum = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.numThumbnailCacheSize.Name = "numThumbnailCacheSize";
      this.numThumbnailCacheSize.Size = new System.Drawing.Size(67, 20);
      this.numThumbnailCacheSize.TabIndex = 5;
      this.numThumbnailCacheSize.TextAlign = HorizontalAlignment.Right;
      this.numThumbnailCacheSize.Value = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.chkEnableThumbnailCache.AutoSize = true;
      this.chkEnableThumbnailCache.Location = new System.Drawing.Point(22, 57);
      this.chkEnableThumbnailCache.Name = "chkEnableThumbnailCache";
      this.chkEnableThumbnailCache.Size = new System.Drawing.Size(105, 17);
      this.chkEnableThumbnailCache.TabIndex = 4;
      this.chkEnableThumbnailCache.Text = "&Thumbnails [MB]";
      this.chkEnableThumbnailCache.UseVisualStyleBackColor = true;
      this.lblThumbCacheUsage.AutoSize = true;
      this.lblThumbCacheUsage.Location = new System.Drawing.Point(298, 58);
      this.lblThumbCacheUsage.Name = "lblThumbCacheUsage";
      this.lblThumbCacheUsage.Size = new System.Drawing.Size(106, 13);
      this.lblThumbCacheUsage.TabIndex = 7;
      this.lblThumbCacheUsage.Text = "usage Thumb Cache";
      this.grpDatabaseBackup.Controls.Add((Control) this.btRestoreDatabase);
      this.grpDatabaseBackup.Controls.Add((Control) this.btBackupDatabase);
      this.grpDatabaseBackup.Dock = DockStyle.Top;
      this.grpDatabaseBackup.Location = new System.Drawing.Point(0, 548);
      this.grpDatabaseBackup.Name = "grpDatabaseBackup";
      this.grpDatabaseBackup.Size = new System.Drawing.Size(498, 93);
      this.grpDatabaseBackup.TabIndex = 4;
      this.grpDatabaseBackup.Text = "Database Backup";
      this.btRestoreDatabase.Location = new System.Drawing.Point(259, 41);
      this.btRestoreDatabase.Name = "btRestoreDatabase";
      this.btRestoreDatabase.Size = new System.Drawing.Size(227, 23);
      this.btRestoreDatabase.TabIndex = 1;
      this.btRestoreDatabase.Text = "Restore Database...";
      this.btRestoreDatabase.UseVisualStyleBackColor = true;
      this.btRestoreDatabase.Click += new EventHandler(this.btRestoreDatabase_Click);
      this.btBackupDatabase.Location = new System.Drawing.Point(9, 41);
      this.btBackupDatabase.Name = "btBackupDatabase";
      this.btBackupDatabase.Size = new System.Drawing.Size(247, 23);
      this.btBackupDatabase.TabIndex = 0;
      this.btBackupDatabase.Text = "Backup Database...";
      this.btBackupDatabase.UseVisualStyleBackColor = true;
      this.btBackupDatabase.Click += new EventHandler(this.btBackupDatabase_Click);
      this.groupOtherComics.Controls.Add((Control) this.chkUpdateComicFiles);
      this.groupOtherComics.Controls.Add((Control) this.labelExcludeCover);
      this.groupOtherComics.Controls.Add((Control) this.chkAutoUpdateComicFiles);
      this.groupOtherComics.Controls.Add((Control) this.txCoverFilter);
      this.groupOtherComics.Dock = DockStyle.Top;
      this.groupOtherComics.Location = new System.Drawing.Point(0, 372);
      this.groupOtherComics.Name = "groupOtherComics";
      this.groupOtherComics.Size = new System.Drawing.Size(498, 176);
      this.groupOtherComics.TabIndex = 5;
      this.groupOtherComics.Text = "Books";
      this.chkUpdateComicFiles.AutoSize = true;
      this.chkUpdateComicFiles.Location = new System.Drawing.Point(9, 42);
      this.chkUpdateComicFiles.Name = "chkUpdateComicFiles";
      this.chkUpdateComicFiles.Size = new System.Drawing.Size(185, 17);
      this.chkUpdateComicFiles.TabIndex = 0;
      this.chkUpdateComicFiles.Text = "Allow writing of Book info into files";
      this.chkUpdateComicFiles.UseVisualStyleBackColor = true;
      this.labelExcludeCover.AutoSize = true;
      this.labelExcludeCover.Location = new System.Drawing.Point(6, 93);
      this.labelExcludeCover.Name = "labelExcludeCover";
      this.labelExcludeCover.Size = new System.Drawing.Size(381, 13);
      this.labelExcludeCover.TabIndex = 2;
      this.labelExcludeCover.Text = "Semicolon separated list of image names never to be used as cover thumbnails:";
      this.chkAutoUpdateComicFiles.AutoSize = true;
      this.chkAutoUpdateComicFiles.Location = new System.Drawing.Point(9, 65);
      this.chkAutoUpdateComicFiles.Name = "chkAutoUpdateComicFiles";
      this.chkAutoUpdateComicFiles.Size = new System.Drawing.Size(196, 17);
      this.chkAutoUpdateComicFiles.TabIndex = 1;
      this.chkAutoUpdateComicFiles.Text = "Book files are updated automatically";
      this.chkAutoUpdateComicFiles.UseVisualStyleBackColor = true;
      this.txCoverFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txCoverFilter.Location = new System.Drawing.Point(9, 112);
      this.txCoverFilter.Multiline = true;
      this.txCoverFilter.Name = "txCoverFilter";
      this.txCoverFilter.Size = new System.Drawing.Size(482, 54);
      this.txCoverFilter.TabIndex = 3;
      this.grpLanguages.Controls.Add((Control) this.btTranslate);
      this.grpLanguages.Controls.Add((Control) this.labelLanguage);
      this.grpLanguages.Controls.Add((Control) this.lbLanguages);
      this.grpLanguages.Dock = DockStyle.Top;
      this.grpLanguages.Location = new System.Drawing.Point(0, 0);
      this.grpLanguages.Name = "grpLanguages";
      this.grpLanguages.Size = new System.Drawing.Size(498, 372);
      this.grpLanguages.TabIndex = 7;
      this.grpLanguages.Text = "Languages";
      this.btTranslate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btTranslate.Location = new System.Drawing.Point(207, 339);
      this.btTranslate.Name = "btTranslate";
      this.btTranslate.Size = new System.Drawing.Size(284, 23);
      this.btTranslate.TabIndex = 12;
      this.btTranslate.Text = "Help localizing ComicRack...";
      this.btTranslate.UseVisualStyleBackColor = true;
      this.btTranslate.Click += new EventHandler(this.btTranslate_Click);
      this.labelLanguage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.labelLanguage.Location = new System.Drawing.Point(6, 33);
      this.labelLanguage.Name = "labelLanguage";
      this.labelLanguage.Size = new System.Drawing.Size(485, 35);
      this.labelLanguage.TabIndex = 11;
      this.labelLanguage.Text = "Select the User Interface language for ComicRack (ComicRack must be restarted for any change to take effect):";
      this.lbLanguages.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.lbLanguages.DrawMode = DrawMode.OwnerDrawFixed;
      this.lbLanguages.FormattingEnabled = true;
      this.lbLanguages.ItemHeight = 15;
      this.lbLanguages.Location = new System.Drawing.Point(6, 75);
      this.lbLanguages.Name = "lbLanguages";
      this.lbLanguages.Size = new System.Drawing.Size(485, 259);
      this.lbLanguages.TabIndex = 0;
      this.lbLanguages.DrawItem += new DrawItemEventHandler(this.lbLanguages_DrawItem);
      this.pageLibrary.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pageLibrary.AutoScroll = true;
      this.pageLibrary.BorderStyle = BorderStyle.FixedSingle;
      this.pageLibrary.Controls.Add((Control) this.grpServerSettings);
      this.pageLibrary.Controls.Add((Control) this.grpSharing);
      this.pageLibrary.Controls.Add((Control) this.groupLibraryDisplay);
      this.pageLibrary.Controls.Add((Control) this.grpScanning);
      this.pageLibrary.Controls.Add((Control) this.groupComicFolders);
      this.pageLibrary.Location = new System.Drawing.Point(84, 8);
      this.pageLibrary.Name = "pageLibrary";
      this.pageLibrary.Size = new System.Drawing.Size(517, 408);
      this.pageLibrary.TabIndex = 10;
      this.grpServerSettings.Controls.Add((Control) this.txPrivateListingPassword);
      this.grpServerSettings.Controls.Add((Control) this.labelPrivateListPassword);
      this.grpServerSettings.Controls.Add((Control) this.labelPublicServerAddress);
      this.grpServerSettings.Controls.Add((Control) this.txPublicServerAddress);
      this.grpServerSettings.Dock = DockStyle.Top;
      this.grpServerSettings.Location = new System.Drawing.Point(0, 910);
      this.grpServerSettings.Name = "grpServerSettings";
      this.grpServerSettings.Size = new System.Drawing.Size(498, 148);
      this.grpServerSettings.TabIndex = 3;
      this.grpServerSettings.Text = "Server Settings";
      this.txPrivateListingPassword.Location = new System.Drawing.Point(12, 114);
      this.txPrivateListingPassword.Name = "txPrivateListingPassword";
      this.txPrivateListingPassword.Password = (string) null;
      this.txPrivateListingPassword.Size = new System.Drawing.Size(379, 20);
      this.txPrivateListingPassword.TabIndex = 3;
      this.txPrivateListingPassword.UseSystemPasswordChar = true;
      this.labelPrivateListPassword.AutoSize = true;
      this.labelPrivateListPassword.Location = new System.Drawing.Point(13, 96);
      this.labelPrivateListPassword.Name = "labelPrivateListPassword";
      this.labelPrivateListPassword.Size = new System.Drawing.Size(307, 13);
      this.labelPrivateListPassword.TabIndex = 2;
      this.labelPrivateListPassword.Text = "Password used to protect your private Internet Share list entries:";
      this.labelPublicServerAddress.AutoSize = true;
      this.labelPublicServerAddress.Location = new System.Drawing.Point(14, 41);
      this.labelPublicServerAddress.Name = "labelPublicServerAddress";
      this.labelPublicServerAddress.Size = new System.Drawing.Size(368, 13);
      this.labelPublicServerAddress.TabIndex = 0;
      this.labelPublicServerAddress.Text = "External IP address of your server if ComicRack should not guess it correctly:";
      this.txPublicServerAddress.Location = new System.Drawing.Point(12, 60);
      this.txPublicServerAddress.Name = "txPublicServerAddress";
      this.txPublicServerAddress.Size = new System.Drawing.Size(379, 20);
      this.txPublicServerAddress.TabIndex = 1;
      this.grpSharing.Controls.Add((Control) this.chkAutoConnectShares);
      this.grpSharing.Controls.Add((Control) this.btRemoveShare);
      this.grpSharing.Controls.Add((Control) this.btAddShare);
      this.grpSharing.Controls.Add((Control) this.tabShares);
      this.grpSharing.Controls.Add((Control) this.chkLookForShared);
      this.grpSharing.Dock = DockStyle.Top;
      this.grpSharing.Location = new System.Drawing.Point(0, 509);
      this.grpSharing.Name = "grpSharing";
      this.grpSharing.Size = new System.Drawing.Size(498, 401);
      this.grpSharing.TabIndex = 1;
      this.grpSharing.Text = "Sharing";
      this.chkAutoConnectShares.AutoSize = true;
      this.chkAutoConnectShares.Location = new System.Drawing.Point(261, 36);
      this.chkAutoConnectShares.Name = "chkAutoConnectShares";
      this.chkAutoConnectShares.Size = new System.Drawing.Size(130, 17);
      this.chkAutoConnectShares.TabIndex = 1;
      this.chkAutoConnectShares.Text = "Connect automatically";
      this.chkAutoConnectShares.UseVisualStyleBackColor = true;
      this.btRemoveShare.Location = new System.Drawing.Point(398, 89);
      this.btRemoveShare.Name = "btRemoveShare";
      this.btRemoveShare.Size = new System.Drawing.Size(92, 23);
      this.btRemoveShare.TabIndex = 4;
      this.btRemoveShare.Text = "Remove";
      this.btRemoveShare.UseVisualStyleBackColor = true;
      this.btRemoveShare.Click += new EventHandler(this.btRmoveShare_Click);
      this.btAddShare.Location = new System.Drawing.Point(398, 61);
      this.btAddShare.Name = "btAddShare";
      this.btAddShare.Size = new System.Drawing.Size(92, 23);
      this.btAddShare.TabIndex = 3;
      this.btAddShare.Text = "Add Share";
      this.btAddShare.UseVisualStyleBackColor = true;
      this.btAddShare.Click += new EventHandler(this.btAddShare_Click);
      this.tabShares.Location = new System.Drawing.Point(12, 59);
      this.tabShares.Name = "tabShares";
      this.tabShares.SelectedIndex = 0;
      this.tabShares.Size = new System.Drawing.Size(381, 336);
      this.tabShares.TabIndex = 2;
      this.chkLookForShared.AutoSize = true;
      this.chkLookForShared.Location = new System.Drawing.Point(12, 36);
      this.chkLookForShared.Name = "chkLookForShared";
      this.chkLookForShared.Size = new System.Drawing.Size(154, 17);
      this.chkLookForShared.TabIndex = 0;
      this.chkLookForShared.Text = "Look for local Book Shares";
      this.chkLookForShared.UseVisualStyleBackColor = true;
      this.groupLibraryDisplay.Controls.Add((Control) this.chkLibraryGaugesTotal);
      this.groupLibraryDisplay.Controls.Add((Control) this.chkLibraryGaugesUnread);
      this.groupLibraryDisplay.Controls.Add((Control) this.chkLibraryGaugesNumeric);
      this.groupLibraryDisplay.Controls.Add((Control) this.chkLibraryGaugesNew);
      this.groupLibraryDisplay.Controls.Add((Control) this.chkLibraryGauges);
      this.groupLibraryDisplay.Dock = DockStyle.Top;
      this.groupLibraryDisplay.Location = new System.Drawing.Point(0, 339);
      this.groupLibraryDisplay.Name = "groupLibraryDisplay";
      this.groupLibraryDisplay.Size = new System.Drawing.Size(498, 170);
      this.groupLibraryDisplay.TabIndex = 4;
      this.groupLibraryDisplay.Text = "Display";
      this.chkLibraryGaugesTotal.AutoSize = true;
      this.chkLibraryGaugesTotal.Location = new System.Drawing.Point(33, 111);
      this.chkLibraryGaugesTotal.Name = "chkLibraryGaugesTotal";
      this.chkLibraryGaugesTotal.Size = new System.Drawing.Size(113, 17);
      this.chkLibraryGaugesTotal.TabIndex = 1;
      this.chkLibraryGaugesTotal.Text = "For Total of Books";
      this.chkLibraryGaugesTotal.UseVisualStyleBackColor = true;
      this.chkLibraryGaugesUnread.AutoSize = true;
      this.chkLibraryGaugesUnread.Location = new System.Drawing.Point(33, 92);
      this.chkLibraryGaugesUnread.Name = "chkLibraryGaugesUnread";
      this.chkLibraryGaugesUnread.Size = new System.Drawing.Size(112, 17);
      this.chkLibraryGaugesUnread.TabIndex = 1;
      this.chkLibraryGaugesUnread.Text = "For Unread Books";
      this.chkLibraryGaugesUnread.UseVisualStyleBackColor = true;
      this.chkLibraryGaugesNumeric.AutoSize = true;
      this.chkLibraryGaugesNumeric.Location = new System.Drawing.Point(33, 131);
      this.chkLibraryGaugesNumeric.Name = "chkLibraryGaugesNumeric";
      this.chkLibraryGaugesNumeric.Size = new System.Drawing.Size(201, 17);
      this.chkLibraryGaugesNumeric.TabIndex = 1;
      this.chkLibraryGaugesNumeric.Text = "Also show numbers and not only bars";
      this.chkLibraryGaugesNumeric.UseVisualStyleBackColor = true;
      this.chkLibraryGaugesNew.AutoSize = true;
      this.chkLibraryGaugesNew.Location = new System.Drawing.Point(33, 72);
      this.chkLibraryGaugesNew.Name = "chkLibraryGaugesNew";
      this.chkLibraryGaugesNew.Size = new System.Drawing.Size(99, 17);
      this.chkLibraryGaugesNew.TabIndex = 1;
      this.chkLibraryGaugesNew.Text = "For New Books";
      this.chkLibraryGaugesNew.UseVisualStyleBackColor = true;
      this.chkLibraryGauges.AutoSize = true;
      this.chkLibraryGauges.Location = new System.Drawing.Point(12, 42);
      this.chkLibraryGauges.Name = "chkLibraryGauges";
      this.chkLibraryGauges.Size = new System.Drawing.Size((int) sbyte.MaxValue, 17);
      this.chkLibraryGauges.TabIndex = 0;
      this.chkLibraryGauges.Text = "Enable Live Counters";
      this.chkLibraryGauges.UseVisualStyleBackColor = true;
      this.chkLibraryGauges.CheckedChanged += new EventHandler(this.chkLibraryGauges_CheckedChanged);
      this.grpScanning.Controls.Add((Control) this.chkDontAddRemovedFiles);
      this.grpScanning.Controls.Add((Control) this.chkAutoRemoveMissing);
      this.grpScanning.Controls.Add((Control) this.lblScan);
      this.grpScanning.Controls.Add((Control) this.btScan);
      this.grpScanning.Dock = DockStyle.Top;
      this.grpScanning.Location = new System.Drawing.Point(0, 203);
      this.grpScanning.Name = "grpScanning";
      this.grpScanning.Size = new System.Drawing.Size(498, 136);
      this.grpScanning.TabIndex = 0;
      this.grpScanning.Text = "Scanning";
      this.chkDontAddRemovedFiles.AutoSize = true;
      this.chkDontAddRemovedFiles.CheckAlign = ContentAlignment.TopLeft;
      this.chkDontAddRemovedFiles.Location = new System.Drawing.Point(12, 58);
      this.chkDontAddRemovedFiles.Name = "chkDontAddRemovedFiles";
      this.chkDontAddRemovedFiles.Size = new System.Drawing.Size(322, 17);
      this.chkDontAddRemovedFiles.TabIndex = 1;
      this.chkDontAddRemovedFiles.Text = "Files manually removed from the Library will not be added again";
      this.chkDontAddRemovedFiles.TextAlign = ContentAlignment.TopLeft;
      this.chkDontAddRemovedFiles.UseVisualStyleBackColor = true;
      this.chkAutoRemoveMissing.AutoSize = true;
      this.chkAutoRemoveMissing.CheckAlign = ContentAlignment.TopLeft;
      this.chkAutoRemoveMissing.Location = new System.Drawing.Point(12, 35);
      this.chkAutoRemoveMissing.Name = "chkAutoRemoveMissing";
      this.chkAutoRemoveMissing.Size = new System.Drawing.Size(301, 17);
      this.chkAutoRemoveMissing.TabIndex = 0;
      this.chkAutoRemoveMissing.Text = "Automatically remove missing files from Library during Scan";
      this.chkAutoRemoveMissing.TextAlign = ContentAlignment.TopLeft;
      this.chkAutoRemoveMissing.UseVisualStyleBackColor = true;
      this.lblScan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.lblScan.Location = new System.Drawing.Point(9, 83);
      this.lblScan.Name = "lblScan";
      this.lblScan.Size = new System.Drawing.Size(480, 43);
      this.lblScan.TabIndex = 8;
      this.lblScan.TextAlign = ContentAlignment.MiddleCenter;
      this.btScan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btScan.Location = new System.Drawing.Point(403, 33);
      this.btScan.Name = "btScan";
      this.btScan.Size = new System.Drawing.Size(88, 23);
      this.btScan.TabIndex = 2;
      this.btScan.Text = "Scan";
      this.btScan.UseVisualStyleBackColor = true;
      this.btScan.Click += new EventHandler(this.btScan_Click);
      this.groupComicFolders.Controls.Add((Control) this.btOpenFolder);
      this.groupComicFolders.Controls.Add((Control) this.btChangeFolder);
      this.groupComicFolders.Controls.Add((Control) this.lbPaths);
      this.groupComicFolders.Controls.Add((Control) this.labelWatchedFolders);
      this.groupComicFolders.Controls.Add((Control) this.btRemoveFolder);
      this.groupComicFolders.Controls.Add((Control) this.btAddFolder);
      this.groupComicFolders.Dock = DockStyle.Top;
      this.groupComicFolders.Location = new System.Drawing.Point(0, 0);
      this.groupComicFolders.Name = "groupComicFolders";
      this.groupComicFolders.Size = new System.Drawing.Size(498, 203);
      this.groupComicFolders.TabIndex = 0;
      this.groupComicFolders.Text = "Book Folders";
      this.btOpenFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btOpenFolder.Location = new System.Drawing.Point(400, 134);
      this.btOpenFolder.Name = "btOpenFolder";
      this.btOpenFolder.Size = new System.Drawing.Size(89, 23);
      this.btOpenFolder.TabIndex = 4;
      this.btOpenFolder.Text = "Open";
      this.btOpenFolder.UseVisualStyleBackColor = true;
      this.btOpenFolder.Click += new EventHandler(this.btOpenFolder_Click);
      this.btChangeFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btChangeFolder.Location = new System.Drawing.Point(400, 66);
      this.btChangeFolder.Name = "btChangeFolder";
      this.btChangeFolder.Size = new System.Drawing.Size(89, 23);
      this.btChangeFolder.TabIndex = 2;
      this.btChangeFolder.Text = "&Change...";
      this.btChangeFolder.UseVisualStyleBackColor = true;
      this.btChangeFolder.Click += new EventHandler(this.btChangeFolder_Click);
      this.lbPaths.AllowDrop = true;
      this.lbPaths.FormattingEnabled = true;
      this.lbPaths.IntegralHeight = false;
      this.lbPaths.Location = new System.Drawing.Point(12, 37);
      this.lbPaths.Name = "lbPaths";
      this.lbPaths.Size = new System.Drawing.Size(377, 120);
      this.lbPaths.TabIndex = 0;
      this.lbPaths.DrawItemText += new DrawItemEventHandler(this.lbPaths_DrawItemText);
      this.lbPaths.DrawItem += new DrawItemEventHandler(this.lbPaths_DrawItem);
      this.lbPaths.SelectedIndexChanged += new EventHandler(this.lbPaths_SelectedIndexChanged);
      this.lbPaths.DragDrop += new DragEventHandler(this.lbPaths_DragDrop);
      this.lbPaths.DragOver += new DragEventHandler(this.lbPaths_DragOver);
      this.labelWatchedFolders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.labelWatchedFolders.Location = new System.Drawing.Point(9, 163);
      this.labelWatchedFolders.Name = "labelWatchedFolders";
      this.labelWatchedFolders.Size = new System.Drawing.Size(480, 26);
      this.labelWatchedFolders.TabIndex = 0;
      this.labelWatchedFolders.Text = "Checked folders will be watched for changes (rename, move) while the program is running.";
      this.labelWatchedFolders.TextAlign = ContentAlignment.BottomCenter;
      this.btRemoveFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btRemoveFolder.Location = new System.Drawing.Point(400, 95);
      this.btRemoveFolder.Name = "btRemoveFolder";
      this.btRemoveFolder.Size = new System.Drawing.Size(89, 23);
      this.btRemoveFolder.TabIndex = 3;
      this.btRemoveFolder.Text = "&Remove";
      this.btRemoveFolder.UseVisualStyleBackColor = true;
      this.btRemoveFolder.Click += new EventHandler(this.btRemoveFolder_Click);
      this.btAddFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btAddFolder.Location = new System.Drawing.Point(400, 37);
      this.btAddFolder.Name = "btAddFolder";
      this.btAddFolder.Size = new System.Drawing.Size(89, 23);
      this.btAddFolder.TabIndex = 1;
      this.btAddFolder.Text = "&Add...";
      this.btAddFolder.UseVisualStyleBackColor = true;
      this.btAddFolder.Click += new EventHandler(this.btAddFolder_Click);
      this.memCacheUpate.Enabled = true;
      this.memCacheUpate.Interval = 1000;
      this.memCacheUpate.Tick += new EventHandler(this.memCacheUpate_Tick);
      this.pageScripts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pageScripts.AutoScroll = true;
      this.pageScripts.BorderStyle = BorderStyle.FixedSingle;
      this.pageScripts.Controls.Add((Control) this.grpScriptSettings);
      this.pageScripts.Controls.Add((Control) this.grpScripts);
      this.pageScripts.Controls.Add((Control) this.grpPackages);
      this.pageScripts.Location = new System.Drawing.Point(84, 8);
      this.pageScripts.Name = "pageScripts";
      this.pageScripts.Size = new System.Drawing.Size(517, 408);
      this.pageScripts.TabIndex = 11;
      this.grpScriptSettings.Controls.Add((Control) this.btAddLibraryFolder);
      this.grpScriptSettings.Controls.Add((Control) this.chkDisableScripting);
      this.grpScriptSettings.Controls.Add((Control) this.labelScriptPaths);
      this.grpScriptSettings.Controls.Add((Control) this.txLibraries);
      this.grpScriptSettings.Dock = DockStyle.Top;
      this.grpScriptSettings.Location = new System.Drawing.Point(0, 752);
      this.grpScriptSettings.Name = "grpScriptSettings";
      this.grpScriptSettings.Size = new System.Drawing.Size(498, 192);
      this.grpScriptSettings.TabIndex = 5;
      this.grpScriptSettings.Text = "Script Settings";
      this.btAddLibraryFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btAddLibraryFolder.Location = new System.Drawing.Point(369, 163);
      this.btAddLibraryFolder.Name = "btAddLibraryFolder";
      this.btAddLibraryFolder.Size = new System.Drawing.Size(121, 23);
      this.btAddLibraryFolder.TabIndex = 3;
      this.btAddLibraryFolder.Text = "Add Folder...";
      this.btAddLibraryFolder.UseVisualStyleBackColor = true;
      this.btAddLibraryFolder.Click += new EventHandler(this.btAddLibraryFolder_Click);
      this.chkDisableScripting.AutoSize = true;
      this.chkDisableScripting.Location = new System.Drawing.Point(9, 39);
      this.chkDisableScripting.Name = "chkDisableScripting";
      this.chkDisableScripting.Size = new System.Drawing.Size(109, 17);
      this.chkDisableScripting.TabIndex = 0;
      this.chkDisableScripting.Text = "Disable all Scripts";
      this.chkDisableScripting.UseVisualStyleBackColor = true;
      this.labelScriptPaths.Location = new System.Drawing.Point(6, 60);
      this.labelScriptPaths.Name = "labelScriptPaths";
      this.labelScriptPaths.Size = new System.Drawing.Size(478, 29);
      this.labelScriptPaths.TabIndex = 1;
      this.labelScriptPaths.Text = "Semicolon separated list of library paths for scripts (e.g. python libraries):";
      this.labelScriptPaths.TextAlign = ContentAlignment.BottomLeft;
      this.txLibraries.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txLibraries.Location = new System.Drawing.Point(7, 92);
      this.txLibraries.Multiline = true;
      this.txLibraries.Name = "txLibraries";
      this.txLibraries.Size = new System.Drawing.Size(482, 63);
      this.txLibraries.TabIndex = 2;
      this.grpScripts.Controls.Add((Control) this.chkHideSampleScripts);
      this.grpScripts.Controls.Add((Control) this.btConfigScript);
      this.grpScripts.Controls.Add((Control) this.lvScripts);
      this.grpScripts.Dock = DockStyle.Top;
      this.grpScripts.Location = new System.Drawing.Point(0, 378);
      this.grpScripts.Name = "grpScripts";
      this.grpScripts.Size = new System.Drawing.Size(498, 374);
      this.grpScripts.TabIndex = 4;
      this.grpScripts.Text = "Available Scripts";
      this.chkHideSampleScripts.AutoSize = true;
      this.chkHideSampleScripts.Location = new System.Drawing.Point(9, 345);
      this.chkHideSampleScripts.Name = "chkHideSampleScripts";
      this.chkHideSampleScripts.Size = new System.Drawing.Size(119, 17);
      this.chkHideSampleScripts.TabIndex = 8;
      this.chkHideSampleScripts.Text = "Hide sample Scripts";
      this.chkHideSampleScripts.UseVisualStyleBackColor = true;
      this.chkHideSampleScripts.CheckedChanged += new EventHandler(this.chkHideSampleScripts_CheckedChanged);
      this.btConfigScript.Enabled = false;
      this.btConfigScript.Location = new System.Drawing.Point(398, 339);
      this.btConfigScript.Name = "btConfigScript";
      this.btConfigScript.Size = new System.Drawing.Size(87, 23);
      this.btConfigScript.TabIndex = 7;
      this.btConfigScript.Text = "Configure...";
      this.btConfigScript.UseVisualStyleBackColor = true;
      this.btConfigScript.Click += new EventHandler(this.btConfigScript_Click);
      this.lvScripts.CheckBoxes = true;
      this.lvScripts.Columns.AddRange(new ColumnHeader[2]
      {
        this.chScriptName,
        this.chScriptPackage
      });
      this.lvScripts.FullRowSelect = true;
      this.lvScripts.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.lvScripts.Location = new System.Drawing.Point(9, 42);
      this.lvScripts.MultiSelect = false;
      this.lvScripts.Name = "lvScripts";
      this.lvScripts.ShowItemToolTips = true;
      this.lvScripts.Size = new System.Drawing.Size(476, 291);
      this.lvScripts.SmallImageList = this.imageList;
      this.lvScripts.Sorting = SortOrder.Ascending;
      this.lvScripts.TabIndex = 6;
      this.lvScripts.UseCompatibleStateImageBehavior = false;
      this.lvScripts.View = View.Details;
      this.lvScripts.ItemChecked += new ItemCheckedEventHandler(this.lvScripts_ItemChecked);
      this.lvScripts.SelectedIndexChanged += new EventHandler(this.lvScripts_SelectedIndexChanged);
      this.chScriptName.Text = "Name";
      this.chScriptName.Width = 250;
      this.chScriptPackage.Text = "Package";
      this.chScriptPackage.Width = 190;
      this.grpPackages.Controls.Add((Control) this.btRemovePackage);
      this.grpPackages.Controls.Add((Control) this.btInstallPackage);
      this.grpPackages.Controls.Add((Control) this.lvPackages);
      this.grpPackages.Dock = DockStyle.Top;
      this.grpPackages.Location = new System.Drawing.Point(0, 0);
      this.grpPackages.Name = "grpPackages";
      this.grpPackages.Size = new System.Drawing.Size(498, 378);
      this.grpPackages.TabIndex = 13;
      this.grpPackages.Text = "Script Packages";
      this.btRemovePackage.Location = new System.Drawing.Point(398, 344);
      this.btRemovePackage.Name = "btRemovePackage";
      this.btRemovePackage.Size = new System.Drawing.Size(86, 23);
      this.btRemovePackage.TabIndex = 2;
      this.btRemovePackage.Text = "Remove";
      this.btRemovePackage.UseVisualStyleBackColor = true;
      this.btRemovePackage.Click += new EventHandler(this.btRemovePackage_Click);
      this.btInstallPackage.Location = new System.Drawing.Point(306, 344);
      this.btInstallPackage.Name = "btInstallPackage";
      this.btInstallPackage.Size = new System.Drawing.Size(86, 23);
      this.btInstallPackage.TabIndex = 1;
      this.btInstallPackage.Text = "Install...";
      this.btInstallPackage.UseVisualStyleBackColor = true;
      this.btInstallPackage.Click += new EventHandler(this.btInstallPackage_Click);
      this.lvPackages.AllowDrop = true;
      this.lvPackages.Columns.AddRange(new ColumnHeader[3]
      {
        this.chPackageName,
        this.chPackageAuthor,
        this.chPackageDescription
      });
      listViewGroup1.Header = "Installed";
      listViewGroup1.Name = "packageGroupInstalled";
      listViewGroup2.Header = "To be removed (requires restart)";
      listViewGroup2.Name = "packageGroupRemove";
      listViewGroup3.Header = "To be installed (requires restart)";
      listViewGroup3.Name = "packageGroupInstall";
      this.lvPackages.Groups.AddRange(new ListViewGroup[3]
      {
        listViewGroup1,
        listViewGroup2,
        listViewGroup3
      });
      this.lvPackages.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.lvPackages.LargeImageList = this.packageImageList;
      this.lvPackages.Location = new System.Drawing.Point(16, 37);
      this.lvPackages.Name = "lvPackages";
      this.lvPackages.ShowItemToolTips = true;
      this.lvPackages.Size = new System.Drawing.Size(468, 301);
      this.lvPackages.SmallImageList = this.packageImageList;
      this.lvPackages.Sorting = SortOrder.Ascending;
      this.lvPackages.TabIndex = 0;
      this.lvPackages.UseCompatibleStateImageBehavior = false;
      this.lvPackages.View = View.Details;
      this.lvPackages.DragDrop += new DragEventHandler(this.lvPackages_DragDrop);
      this.lvPackages.DragOver += new DragEventHandler(this.lvPackages_DragOver);
      this.lvPackages.DoubleClick += new EventHandler(this.lvPackages_DoubleClick);
      this.chPackageName.Text = "Package";
      this.chPackageName.Width = 130;
      this.chPackageAuthor.Text = "Author";
      this.chPackageAuthor.Width = 89;
      this.chPackageDescription.Text = "Description";
      this.chPackageDescription.Width = 217;
      this.packageImageList.ColorDepth = ColorDepth.Depth32Bit;
      this.packageImageList.ImageSize = new System.Drawing.Size(32, 32);
      this.packageImageList.TransparentColor = Color.Transparent;
      this.tabReader.Appearance = Appearance.Button;
      this.tabReader.AutoEllipsis = true;
      this.tabReader.Image = (Image) Resources.ReaderPref;
      this.tabReader.ImageAlign = ContentAlignment.TopCenter;
      this.tabReader.Location = new System.Drawing.Point(3, 7);
      this.tabReader.Name = "tabReader";
      this.tabReader.Size = new System.Drawing.Size(75, 56);
      this.tabReader.TabIndex = 13;
      this.tabReader.Text = "Reader";
      this.tabReader.TextAlign = ContentAlignment.BottomCenter;
      this.tabReader.UseVisualStyleBackColor = true;
      this.tabReader.CheckedChanged += new EventHandler(this.chkAdvanced_CheckedChanged);
      this.tabLibraries.Appearance = Appearance.Button;
      this.tabLibraries.AutoEllipsis = true;
      this.tabLibraries.Image = (Image) Resources.LibraryPref;
      this.tabLibraries.ImageAlign = ContentAlignment.TopCenter;
      this.tabLibraries.Location = new System.Drawing.Point(3, 66);
      this.tabLibraries.Name = "tabLibraries";
      this.tabLibraries.Size = new System.Drawing.Size(75, 56);
      this.tabLibraries.TabIndex = 14;
      this.tabLibraries.Text = "Libraries";
      this.tabLibraries.TextAlign = ContentAlignment.BottomCenter;
      this.tabLibraries.UseVisualStyleBackColor = true;
      this.tabLibraries.CheckedChanged += new EventHandler(this.chkAdvanced_CheckedChanged);
      this.tabBehavior.Appearance = Appearance.Button;
      this.tabBehavior.AutoEllipsis = true;
      this.tabBehavior.Image = (Image) Resources.BehaviorPref;
      this.tabBehavior.ImageAlign = ContentAlignment.TopCenter;
      this.tabBehavior.Location = new System.Drawing.Point(3, 126);
      this.tabBehavior.Name = "tabBehavior";
      this.tabBehavior.Size = new System.Drawing.Size(75, 56);
      this.tabBehavior.TabIndex = 15;
      this.tabBehavior.Text = "Behavior";
      this.tabBehavior.TextAlign = ContentAlignment.BottomCenter;
      this.tabBehavior.UseVisualStyleBackColor = true;
      this.tabBehavior.CheckedChanged += new EventHandler(this.chkAdvanced_CheckedChanged);
      this.tabScripts.Appearance = Appearance.Button;
      this.tabScripts.AutoEllipsis = true;
      this.tabScripts.Image = (Image) Resources.ScriptingPref;
      this.tabScripts.ImageAlign = ContentAlignment.TopCenter;
      this.tabScripts.Location = new System.Drawing.Point(3, 187);
      this.tabScripts.Name = "tabScripts";
      this.tabScripts.Size = new System.Drawing.Size(75, 56);
      this.tabScripts.TabIndex = 16;
      this.tabScripts.Text = "Scripts";
      this.tabScripts.TextAlign = ContentAlignment.BottomCenter;
      this.tabScripts.UseVisualStyleBackColor = true;
      this.tabScripts.CheckedChanged += new EventHandler(this.chkAdvanced_CheckedChanged);
      this.tabAdvanced.Appearance = Appearance.Button;
      this.tabAdvanced.AutoEllipsis = true;
      this.tabAdvanced.Image = (Image) Resources.AdvancedPref;
      this.tabAdvanced.ImageAlign = ContentAlignment.TopCenter;
      this.tabAdvanced.Location = new System.Drawing.Point(3, 248);
      this.tabAdvanced.Name = "tabAdvanced";
      this.tabAdvanced.Size = new System.Drawing.Size(75, 56);
      this.tabAdvanced.TabIndex = 17;
      this.tabAdvanced.Text = "Advanced";
      this.tabAdvanced.TextAlign = ContentAlignment.BottomCenter;
      this.tabAdvanced.UseVisualStyleBackColor = true;
      this.tabAdvanced.CheckedChanged += new EventHandler(this.chkAdvanced_CheckedChanged);
      this.btResetTwitter.Location = new System.Drawing.Point(382, 66);
      this.btResetTwitter.Name = "btResetTwitter";
      this.btResetTwitter.Size = new System.Drawing.Size(104, 23);
      this.btResetTwitter.TabIndex = 3;
      this.btResetTwitter.Text = "Reset";
      this.btResetTwitter.UseVisualStyleBackColor = true;
      this.btResetTwitter.Click += new EventHandler(this.btResetTwitter_Click);
      this.labelResetTwitter.Location = new System.Drawing.Point(6, 71);
      this.labelResetTwitter.Name = "labelResetTwitter";
      this.labelResetTwitter.Size = new System.Drawing.Size(370, 17);
      this.labelResetTwitter.TabIndex = 2;
      this.labelResetTwitter.Text = "To reset the Twitter authorization, press";
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(610, 453);
      this.Controls.Add((Control) this.pageAdvanced);
      this.Controls.Add((Control) this.tabAdvanced);
      this.Controls.Add((Control) this.tabScripts);
      this.Controls.Add((Control) this.tabBehavior);
      this.Controls.Add((Control) this.tabLibraries);
      this.Controls.Add((Control) this.tabReader);
      this.Controls.Add((Control) this.pageReader);
      this.Controls.Add((Control) this.pageLibrary);
      this.Controls.Add((Control) this.pageScripts);
      this.Controls.Add((Control) this.pageBehavior);
      this.Controls.Add((Control) this.btApply);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (PreferencesDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Preferences";
      this.pageReader.ResumeLayout(false);
      this.groupHardwareAcceleration.ResumeLayout(false);
      this.groupHardwareAcceleration.PerformLayout();
      this.grpMouse.ResumeLayout(false);
      this.grpMouse.PerformLayout();
      this.groupOverlays.ResumeLayout(false);
      this.groupOverlays.PerformLayout();
      this.panelReaderOverlays.ResumeLayout(false);
      this.grpKeyboard.ResumeLayout(false);
      this.cmKeyboardLayout.ResumeLayout(false);
      this.grpDisplay.ResumeLayout(false);
      this.grpDisplay.PerformLayout();
      this.pageAdvanced.ResumeLayout(false);
      this.grpWirelessSetup.ResumeLayout(false);
      this.grpWirelessSetup.PerformLayout();
      this.grpIntegration.ResumeLayout(false);
      this.grpIntegration.PerformLayout();
      this.groupMessagesAndSocial.ResumeLayout(false);
      this.groupMemory.ResumeLayout(false);
      this.grpMaximumMemoryUsage.ResumeLayout(false);
      this.grpMaximumMemoryUsage.PerformLayout();
      this.grpMemoryCache.ResumeLayout(false);
      this.grpMemoryCache.PerformLayout();
      this.numMemPageCount.EndInit();
      this.numMemThumbSize.EndInit();
      this.grpDiskCache.ResumeLayout(false);
      this.grpDiskCache.PerformLayout();
      this.numPageCacheSize.EndInit();
      this.numInternetCacheSize.EndInit();
      this.numThumbnailCacheSize.EndInit();
      this.grpDatabaseBackup.ResumeLayout(false);
      this.groupOtherComics.ResumeLayout(false);
      this.groupOtherComics.PerformLayout();
      this.grpLanguages.ResumeLayout(false);
      this.pageLibrary.ResumeLayout(false);
      this.grpServerSettings.ResumeLayout(false);
      this.grpServerSettings.PerformLayout();
      this.grpSharing.ResumeLayout(false);
      this.grpSharing.PerformLayout();
      this.groupLibraryDisplay.ResumeLayout(false);
      this.groupLibraryDisplay.PerformLayout();
      this.grpScanning.ResumeLayout(false);
      this.grpScanning.PerformLayout();
      this.groupComicFolders.ResumeLayout(false);
      this.pageScripts.ResumeLayout(false);
      this.grpScriptSettings.ResumeLayout(false);
      this.grpScriptSettings.PerformLayout();
      this.grpScripts.ResumeLayout(false);
      this.grpScripts.PerformLayout();
      this.grpPackages.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    public PreferencesDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.lvPackages.Columns.ScaleDpi();
      this.lvScripts.Columns.ScaleDpi();
      this.RestorePosition();
      this.RestorePanelStates();
      this.tabReader.Tag = (object) this.pageReader;
      this.tabBehavior.Tag = (object) this.pageBehavior;
      this.tabLibraries.Tag = (object) this.pageLibrary;
      this.tabScripts.Tag = (object) this.pageScripts;
      this.tabAdvanced.Tag = (object) this.pageAdvanced;
      this.tabButtons.AddRange((IEnumerable<CheckBox>) new CheckBox[5]
      {
        this.tabReader,
        this.tabBehavior,
        this.tabLibraries,
        this.tabScripts,
        this.tabAdvanced
      });
      this.lbLanguages.ItemHeight = FormUtility.ScaleDpiY(this.lbLanguages.ItemHeight);
      this.tabReader.Image = (Image) ((Bitmap) this.tabReader.Image).ScaleDpi();
      this.tabAdvanced.Image = (Image) ((Bitmap) this.tabAdvanced.Image).ScaleDpi();
      this.tabBehavior.Image = (Image) ((Bitmap) this.tabBehavior.Image).ScaleDpi();
      this.tabLibraries.Image = (Image) ((Bitmap) this.tabLibraries.Image).ScaleDpi();
      this.tabScripts.Image = (Image) ((Bitmap) this.tabScripts.Image).ScaleDpi();
      FormUtility.FillPanelWithOptions((Control) this.pageBehavior, (object) Program.Settings, TR.Load("Settings"));
      this.packageImageList.Images.Add((Image) Resources.Package);
      LocalizeUtility.Localize((Control) this, this.components);
      LocalizeUtility.Localize(TR.Load(this.Name), this.cbNavigationOverlayPosition);
      FormUtility.RegisterPanelToTabToggle((Control) this.pageReader, PropertyCaller.CreateFlagsValueStore<Settings, TabLayouts>(Program.Settings, "TabLayouts", TabLayouts.ReaderSettings));
      FormUtility.RegisterPanelToTabToggle((Control) this.pageBehavior, PropertyCaller.CreateFlagsValueStore<Settings, TabLayouts>(Program.Settings, "TabLayouts", TabLayouts.BehaviorSettings));
      FormUtility.RegisterPanelToTabToggle((Control) this.pageLibrary, PropertyCaller.CreateFlagsValueStore<Settings, TabLayouts>(Program.Settings, "TabLayouts", TabLayouts.LibrarySettings));
      FormUtility.RegisterPanelToTabToggle((Control) this.pageScripts, PropertyCaller.CreateFlagsValueStore<Settings, TabLayouts>(Program.Settings, "TabLayouts", TabLayouts.ScriptSettings));
      FormUtility.RegisterPanelToTabToggle((Control) this.pageAdvanced, PropertyCaller.CreateFlagsValueStore<Settings, TabLayouts>(Program.Settings, "TabLayouts", TabLayouts.AdvancedSettings));
      Program.Scanner.ScanNotify += new EventHandler<ComicScanNotifyEventArgs>(this.DatabaseScanNotify);
      IdleProcess.Idle += new EventHandler(this.ApplicationIdle);
      this.numMemPageCount.Minimum = 20M;
      this.numMemPageCount.Maximum = 100M;
      this.numMemThumbSize.Minimum = 5M;
      this.numMemThumbSize.Maximum = 100M;
      this.lbLanguages.Items.Add((object) new TRInfo());
      this.lbLanguages.Items.Add((object) new TRInfo("en"));
      foreach (object installedLanguage in Program.InstalledLanguages)
        this.lbLanguages.Items.Add(installedLanguage);
      this.SetSettings();
      foreach (WatchFolder watchFolder in (SmartList<WatchFolder>) Program.Database.WatchFolders)
        this.lbPaths.Items.Add((object) watchFolder.Folder, watchFolder.Watch);
      this.lbPaths_SelectedIndexChanged((object) this.lbPaths, EventArgs.Empty);
      this.SetScanButtonText();
      this.btResetMessages.Enabled = Program.Settings.HiddenMessageBoxes != 0;
      this.btResetTwitter.Enabled = !string.IsNullOrEmpty(Program.Settings.TwitterAccessToken);
      this.FillExtensionsList();
      this.chkOverwriteAssociations.Checked = Program.Settings.OverwriteAssociations;
      if (!FileFormat.CanRegisterShell)
      {
        Win7.ShowShield(this.btAssociateExtensions);
      }
      else
      {
        this.btAssociateExtensions.Visible = false;
        this.lbFormats.Width = this.btAssociateExtensions.Right - this.lbFormats.Left;
      }
      Program.InternetCache.SizeChanged += new EventHandler(this.UpdateDiskCacheStatus);
      Program.ImagePool.Pages.DiskCache.SizeChanged += new EventHandler(this.UpdateDiskCacheStatus);
      Program.ImagePool.Thumbs.DiskCache.SizeChanged += new EventHandler(this.UpdateDiskCacheStatus);
      this.UpdateDiskCacheStatus((object) this, (EventArgs) null);
      this.UpdateMemoryCacheStatus();
      this.RefreshPackageList();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        PreferencesDialog.activeTab = this.tabButtons.FindIndex((Predicate<CheckBox>) (c => c.Checked));
        Program.Scanner.ScanNotify -= new EventHandler<ComicScanNotifyEventArgs>(this.DatabaseScanNotify);
        IdleProcess.Idle -= new EventHandler(this.ApplicationIdle);
        Program.InternetCache.SizeChanged -= new EventHandler(this.UpdateDiskCacheStatus);
        Program.ImagePool.Pages.DiskCache.SizeChanged -= new EventHandler(this.UpdateDiskCacheStatus);
        Program.ImagePool.Thumbs.DiskCache.SizeChanged -= new EventHandler(this.UpdateDiskCacheStatus);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public string AutoInstallPlugin { get; set; }

    public bool NeedsRestart { get; set; }

    public string BackupFile { get; set; }

    public PluginEngine Plugins
    {
      get => this.pluginEngine;
      set
      {
        this.pluginEngine = value;
        this.FillScriptsList();
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.SetTab(PreferencesDialog.activeTab != -1 ? this.tabButtons[PreferencesDialog.activeTab] : this.tabReader);
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);
      if (string.IsNullOrEmpty(this.AutoInstallPlugin))
        return;
      this.SetTab(this.tabScripts);
      if (!this.InstallPlugin(this.AutoInstallPlugin))
        return;
      this.RefreshPackageList();
    }

    private void btTestWifi_Click(object sender, EventArgs e)
    {
      string text = string.Empty;
      using (new WaitCursor((Form) this))
      {
        foreach (ISyncProvider syncProvider in DeviceSyncFactory.Discover(DeviceSyncFactory.ParseWifiAddressList(this.txWifiAddresses.Text)))
          text = text.AppendWithSeparator(", ", syncProvider.Device.Model);
      }
      Label lblWifiStatus = this.lblWifiStatus;
      string str;
      if (!string.IsNullOrEmpty(text))
        str = TR.Load(this.Name)["msgDevicesFound", "{0} found!"].SafeFormat((object) text);
      else
        str = TR.Load(this.Name)["msgNoDevicesFound", "No devices found!"];
      lblWifiStatus.Text = str;
    }

    private void chkAdvanced_CheckedChanged(object sender, EventArgs e)
    {
      CheckBox cb = sender as CheckBox;
      if (!cb.Checked)
        return;
      this.SetTab(cb);
    }

    private void chkLibraryGauges_CheckedChanged(object sender, EventArgs e)
    {
      this.chkLibraryGaugesNew.Enabled = this.chkLibraryGaugesUnread.Enabled = this.chkLibraryGaugesTotal.Enabled = this.chkLibraryGaugesNumeric.Enabled = this.chkLibraryGauges.Checked;
    }

    private void tbSystemMemory_ValueChanged(object sender, EventArgs e)
    {
      int num = this.tbMaximumMemoryUsage.Value * 32;
      if (num == 1024)
        this.lblMaximumMemoryUsageValue.Text = TR.Default["Unlimited"];
      else
        this.lblMaximumMemoryUsageValue.Text = string.Format("{0} MB", (object) num);
    }

    private void lbPaths_DragDrop(object sender, DragEventArgs e)
    {
      if (!(e.Data.GetData(DataFormats.FileDrop) is string[] data))
        return;
      ((IEnumerable<string>) data).Select<string, string>((Func<string, string>) (d => !Directory.Exists(d) ? Path.GetDirectoryName(d) : d)).Where<string>((Func<string, bool>) (d => !this.lbPaths.Items.Contains((object) d))).ForEach<string>((Action<string>) (d => this.lbPaths.Items.Add((object) d)));
    }

    private void lbPaths_DragOver(object sender, DragEventArgs e)
    {
      e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void lvScripts_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
      if (!(e.Item.Tag is Command tag))
        return;
      tag.Enabled = e.Item.Checked;
    }

    private void lvScripts_SelectedIndexChanged(object sender, EventArgs e)
    {
      Command command = this.lvScripts.SelectedItems.OfType<ListViewItem>().Select<ListViewItem, Command>((Func<ListViewItem, Command>) (lvi => lvi.Tag as Command)).FirstOrDefault<Command>();
      this.btConfigScript.Enabled = command != null && command.Configure != null;
      if (!this.btConfigScript.Enabled)
        return;
      this.btConfigScript.Tag = (object) command.Configure;
    }

    private void btConfigScript_Click(object sender, EventArgs e)
    {
      if (!(this.btConfigScript.Tag is Command tag))
        return;
      tag.Invoke(new object[0], true);
    }

    private void lbPaths_DrawItemText(object sender, DrawItemEventArgs e)
    {
      using (StringFormat format = new StringFormat()
      {
        LineAlignment = StringAlignment.Center,
        Trimming = StringTrimming.EllipsisPath
      })
        ((CheckedListBoxEx) sender).DrawDefaultItemText(e, format);
    }

    private void DatabaseScanNotify(object sender, ComicScanNotifyEventArgs e)
    {
      try
      {
        if (this.BeginInvokeIfRequired((Action) (() => this.DatabaseScanNotify(sender, e))))
          return;
        if (string.IsNullOrEmpty(e.File))
          this.lblScan.Text = string.Empty;
        else
          this.lblScan.Text = StringUtility.Format(LocalizeUtility.GetText((Control) this, "Scanning", "Scanning '{0}' ..."), (object) e.File);
        this.SetScanButtonText();
      }
      catch (Exception ex)
      {
      }
    }

    private void btClearThumbnailCache_Click(object sender, EventArgs e)
    {
      using (new WaitCursor())
        Program.ImagePool.Thumbs.DiskCache.Clear();
    }

    private void btClearPageCache_Click(object sender, EventArgs e)
    {
      using (new WaitCursor())
        Program.ImagePool.Pages.DiskCache.Clear();
    }

    private void btClearInternetCache_Click(object sender, EventArgs e)
    {
      using (new WaitCursor())
        Program.InternetCache.Clear();
    }

    private void btAddFolder_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.Description = LocalizeUtility.GetText((Control) this, "SelectComicFolder", "Please select a folder containing Books");
        folderBrowserDialog.ShowNewFolderButton = true;
        if (folderBrowserDialog.ShowDialog((IWin32Window) this) != DialogResult.OK || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
          return;
        this.lbPaths.Items.Add((object) folderBrowserDialog.SelectedPath);
        if (this.lbPaths.SelectedIndex != -1)
          return;
        this.lbPaths.SelectedIndex = 0;
      }
    }

    private void btChangeFolder_Click(object sender, EventArgs e)
    {
      if (!(this.lbPaths.SelectedItem is string selectedItem))
        return;
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.Description = LocalizeUtility.GetText((Control) this, "SelectComicFolder", "Please select a folder containing Books");
        folderBrowserDialog.ShowNewFolderButton = true;
        folderBrowserDialog.SelectedPath = selectedItem;
        if (folderBrowserDialog.ShowDialog((IWin32Window) this) != DialogResult.OK || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
          return;
        this.lbPaths.Items[this.lbPaths.SelectedIndex] = (object) folderBrowserDialog.SelectedPath;
      }
    }

    private void btAddLibraryFolder_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.Description = LocalizeUtility.GetText((Control) this, "SelectScriptFolder", "Please select a script library folder");
        folderBrowserDialog.ShowNewFolderButton = false;
        if (folderBrowserDialog.ShowDialog((IWin32Window) this) != DialogResult.OK || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
          return;
        if (this.txLibraries.Text.Trim().Length > 0)
          this.txLibraries.Text += ";";
        this.txLibraries.Text += folderBrowserDialog.SelectedPath;
      }
    }

    private void btRemoveFolder_Click(object sender, EventArgs e)
    {
      int selectedIndex = this.lbPaths.SelectedIndex;
      if (selectedIndex == -1)
        return;
      this.lbPaths.Items.RemoveAt(selectedIndex);
    }

    private void btOpenFolder_Click(object sender, EventArgs e)
    {
      Program.ShowExplorer(this.lbPaths.SelectedItem as string);
    }

    private void btScan_Click(object sender, EventArgs e)
    {
      if (Program.Scanner.IsScanning)
      {
        Program.Scanner.Stop(true);
      }
      else
      {
        foreach (string fileOrFolder in (ListBox.ObjectCollection) this.lbPaths.Items)
          Program.Scanner.ScanFileOrFolder(fileOrFolder, true, this.chkAutoRemoveMissing.Checked);
      }
    }

    private void lbPaths_DrawItem(object sender, DrawItemEventArgs e)
    {
      e.DrawBackground();
      string s = this.lbPaths.Items[e.Index] as string;
      using (StringFormat format = new StringFormat()
      {
        Trimming = StringTrimming.EllipsisPath
      })
      {
        using (Brush brush = (Brush) new SolidBrush(e.ForeColor))
          e.Graphics.DrawString(s, e.Font, brush, (RectangleF) e.Bounds, format);
      }
      e.DrawFocusRectangle();
    }

    private void lbPaths_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.btOpenFolder.Enabled = this.btRemoveFolder.Enabled = this.lbPaths.SelectedIndex != -1;
      this.btScan.Enabled = this.lbPaths.Items.Count > 0;
    }

    private void btResetMessages_Click(object sender, EventArgs e)
    {
      Program.Settings.HiddenMessageBoxes = HiddenMessageBoxes.None;
      this.btResetMessages.Enabled = false;
    }

    private void btResetTwitter_Click(object sender, EventArgs e)
    {
      Program.Settings.ResetTwitter();
    }

    private void ApplicationIdle(object sender, EventArgs e)
    {
      this.chkEnableDisplayChangeAnimation.Enabled = this.chkEnableHardwareFiltering.Enabled = this.chkEnableSoftwareFiltering.Enabled = this.chkEnableInertialMouseScrolling.Enabled = this.chkEnableHardware.Checked;
      this.chkAutoConnectShares.Enabled = this.chkLookForShared.Checked;
      this.btRemovePackage.Enabled = this.lvPackages.SelectedItems.Count > 0;
      this.labelPageOverlay.Enabled = this.chkShowCurrentPageOverlay.Checked;
      this.labelVisiblePartOverlay.Enabled = this.chkShowVisiblePartOverlay.Checked;
      this.labelStatusOverlay.Enabled = this.chkShowStatusOverlay.Checked;
      this.labelNavigationOverlay.Top = this.cbNavigationOverlayPosition.SelectedIndex != 0 ? this.labelPageOverlay.Top : this.labelVisiblePartOverlay.Bottom - this.labelNavigationOverlay.Height;
      this.labelPageOverlay.Text = this.chkShowPageNames.Checked ? LocalizeUtility.GetText((Control) this, "PageNumberAndName", "Page\nName") : LocalizeUtility.GetText((Control) this, "PageNumberOnly", "Page");
    }

    private void btApply_Click(object sender, EventArgs e) => this.Apply();

    private void tbSaturation_DoubleClick(object sender, EventArgs e)
    {
      this.tbSaturation.Value = 0;
    }

    private void tbBrightness_DoubleClick(object sender, EventArgs e)
    {
      this.tbBrightness.Value = 0;
    }

    private void tbContrast_DoubleClick(object sender, EventArgs e) => this.tbContrast.Value = 0;

    private void tbSharpening_DoubleClick(object sender, EventArgs e)
    {
      this.tbSharpening.Value = 0;
    }

    private void tbGamma_DoubleClick(object sender, EventArgs e) => this.tbGamma.Value = 0;

    private void btReset_Click(object sender, EventArgs e)
    {
      this.tbSaturation.Value = this.tbBrightness.Value = this.tbContrast.Value = this.tbGamma.Value = 0;
    }

    private void tbOverlayScalingChanged(object sender, EventArgs e)
    {
      this.toolTip.SetToolTip((Control) this.tbOverlayScaling, string.Format("{0}%", (object) this.tbOverlayScaling.Value));
    }

    private void tbColorAdjustmentChanged(object sender, EventArgs e)
    {
      TrackBarLite trackBarLite = (TrackBarLite) sender;
      this.toolTip.SetToolTip((Control) trackBarLite, string.Format("{1}{0}%", (object) trackBarLite.Value, trackBarLite.Value > 0 ? (object) "+" : (object) string.Empty));
    }

    private void lbLanguages_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (e.Index == -1)
        return;
      TRInfo trInfo = (TRInfo) this.lbLanguages.Items[e.Index];
      e.DrawBackground();
      using (Brush brush = (Brush) new SolidBrush((double) trInfo.CompletionPercent > 95.0 ? this.ForeColor : Color.Red))
      {
        Rectangle bounds = e.Bounds;
        using (Image flagFromCulture = cYo.Common.Presentation.Flags.GetFlagFromCulture(trInfo.CultureName ?? CultureInfo.InstalledUICulture.Name))
        {
          if (flagFromCulture != null)
          {
            float scale = flagFromCulture.Size.GetScale(bounds.Pad(2).Size);
            e.Graphics.DrawImage(flagFromCulture, (float) (bounds.X + 1), (float) (bounds.Y + 2), (float) flagFromCulture.Width * scale, (float) flagFromCulture.Height * scale);
          }
        }
        bounds.X += FormUtility.ScaleDpiX(20);
        bounds.Width -= FormUtility.ScaleDpiX(20);
        string[] strArray = trInfo.ToString().Split('\t');
        using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
        {
          Trimming = StringTrimming.Character,
          LineAlignment = StringAlignment.Center
        })
        {
          e.Graphics.DrawString(strArray[0], e.Font, brush, (RectangleF) bounds, format);
          if (strArray.Length > 1)
          {
            format.Alignment = StringAlignment.Far;
            e.Graphics.DrawString(strArray[1], e.Font, brush, (RectangleF) bounds, format);
          }
        }
      }
      if ((e.State & DrawItemState.Focus) == DrawItemState.None)
        return;
      ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
    }

    private void btBackupDatabase_Click(object sender, EventArgs e)
    {
      using (SaveFileDialog dlg = new SaveFileDialog())
      {
        dlg.Title = this.btBackupDatabase.Text.Replace(".", string.Empty);
        dlg.FileName = string.Format("ComicDB Backup {0}.zip", (object) DateTime.Now.ToString("yyyy-MM-dd"));
        dlg.Filter = TR.Load("FileFilter")["ComicRackBackup", "ComicRack Database|*.zip"];
        dlg.DefaultExt = ".zip";
        if (dlg.ShowDialog((IWin32Window) this) != DialogResult.OK)
          return;
        try
        {
          AutomaticProgressDialog.Process((Form) this, TR.Messages["DatabaseBackup", "Database Backup"], TR.Messages["DatabaseBackupText", "Creating and saving the Backup File"], 1000, (Action) (() => Program.DatabaseManager.BackupTo(dlg.FileName, Program.Paths.CustomThumbnailPath)), AutomaticProgressDialogOptions.None);
        }
        catch
        {
          int num = (int) MessageBox.Show((IWin32Window) this, TR.Messages["DatabaseBackupError", "There was an error saving the Database backup"], TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
      }
    }

    private void btRestoreDatabase_Click(object sender, EventArgs e)
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Title = this.btRestoreDatabase.Text.Replace(".", string.Empty);
        openFileDialog.Filter = TR.Load("FileFilter")["ComicRackBackup", "ComicRack Backup|*.zip"];
        openFileDialog.CheckFileExists = true;
        if (openFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
          return;
        this.BackupFile = openFileDialog.FileName;
      }
    }

    private void btTranslate_Click(object sender, EventArgs e)
    {
      Program.StartDocument("http://comicrack.cyolito.com/faqs/12-how-to-create-language-packs");
    }

    private void memCacheUpate_Tick(object sender, EventArgs e) => this.UpdateMemoryCacheStatus();

    private void btInstallPackage_Click(object sender, EventArgs e)
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Title = this.btInstallPackage.Text.Replace(".", string.Empty);
        openFileDialog.Filter = TR.Load("FileFilter")["ScriptPackageOpen", "ComicRack Plugin|*.crplugin|Script Archive|*.zip"];
        openFileDialog.CheckFileExists = true;
        if (openFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK || !this.InstallPlugin(openFileDialog.FileName))
          return;
        this.RefreshPackageList();
      }
    }

    private void btRemovePackage_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem selectedItem in this.lvPackages.SelectedItems)
      {
        this.NeedsRestart = true;
        if (!Program.ScriptPackages.Uninstall(selectedItem.Tag as PackageManager.Package))
        {
          int num = (int) MessageBox.Show((IWin32Window) this, TR.Messages["FailedRemovePackage", "Failed to uninstall package. Please restart ComicRack and try again!"], TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
      }
      this.RefreshPackageList();
    }

    private void lvPackages_DoubleClick(object sender, EventArgs e)
    {
      ListViewItem listViewItem = this.lvPackages.SelectedItems.OfType<ListViewItem>().FirstOrDefault<ListViewItem>();
      if (listViewItem == null)
        return;
      PackageManager.Package tag = listViewItem.Tag as PackageManager.Package;
      if (tag.PackageType != PackageManager.PackageType.Installed)
        return;
      Program.ShowExplorer(tag.PackagePath);
    }

    private void lvPackages_DragDrop(object sender, DragEventArgs e)
    {
      try
      {
        string[] data = e.Data.GetData(DataFormats.FileDrop) as string[];
        int index = 0;
        while (index < data.Length && this.InstallPlugin(data[index]))
          ++index;
        this.RefreshPackageList();
      }
      catch (Exception ex)
      {
      }
    }

    private void lvPackages_DragOver(object sender, DragEventArgs e)
    {
      e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void keyboardShortcutEditor_DragOver(object sender, DragEventArgs e)
    {
      e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void keyboardShortcutEditor_DragDrop(object sender, DragEventArgs e)
    {
      try
      {
        this.LoadKeyboard(((string[]) e.Data.GetData(DataFormats.FileDrop))[0]);
      }
      catch (Exception ex)
      {
      }
    }

    private void btExportKeyboard_Click(object sender, EventArgs e)
    {
      using (SaveFileDialog saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Title = this.btExportKeyboard.Text.Replace("&", string.Empty);
        saveFileDialog.FileName = TR.Load("FileFilter")["KeyboardLayout", "Keyboard Layout"] + ".xml";
        saveFileDialog.Filter = TR.Load("FileFilter")["KeyboardLayoutFilter", "Keyboard Layout|*.xml"];
        saveFileDialog.DefaultExt = ".xml";
        saveFileDialog.CheckPathExists = true;
        saveFileDialog.OverwritePrompt = true;
        if (saveFileDialog.ShowDialog((IWin32Window) this) == DialogResult.Cancel)
          return;
        try
        {
          List<StringPair> list = this.keyboardShortcutEditor.Shortcuts.GetKeyMapping().ToList<StringPair>();
          XmlUtility.Store(saveFileDialog.FileName, (object) list);
          Program.Settings.KeyboardLayouts.UpdateMostRecent(saveFileDialog.FileName);
        }
        catch (Exception ex)
        {
          int num = (int) MessageBox.Show((IWin32Window) this, string.Format(TR.Messages["CouldNotExportKeyboardLayout", "Could not export Keyboard Layout!\nReason: {0}"], (object) ex.Message), TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
      }
    }

    private void btLoadKeyboard_Click(object sender, EventArgs e)
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Title = this.btImportKeyboard.Text.Replace("&", string.Empty);
        openFileDialog.Filter = TR.Load("FileFilter")["KeyboardLayoutFilter", "Keyboard Layout|*.xml"];
        openFileDialog.DefaultExt = ".xml";
        openFileDialog.CheckFileExists = true;
        if (openFileDialog.ShowDialog((IWin32Window) this) == DialogResult.Cancel)
          return;
        this.LoadKeyboard(openFileDialog.FileName);
      }
    }

    private void btImportKeyboard_ShowContextMenu(object sender, EventArgs e)
    {
      FormUtility.SafeToolStripClear(this.cmKeyboardLayout.Items, 2);
      foreach (string keyboardLayout in (SmartList<string>) Program.Settings.KeyboardLayouts)
      {
        string file = keyboardLayout;
        this.cmKeyboardLayout.Items.Add(keyboardLayout, (Image) null, (EventHandler) ((s, ea) => this.LoadKeyboard(file)));
      }
      this.cmKeyboardLayout.Items[1].Visible = this.cmKeyboardLayout.Items.Count > 2;
    }

    private void miDefaultKeyboardLayout_Click(object sender, EventArgs e)
    {
      this.keyboardShortcutEditor.Shortcuts.SetKeyMapping(Program.DefaultKeyboardMapping);
      this.keyboardShortcutEditor.RefreshList();
    }

    private void chkHideSampleScripts_CheckedChanged(object sender, EventArgs e)
    {
      this.FillScriptsList();
    }

    private void FillScriptsList()
    {
      this.lvScripts.BeginUpdate();
      try
      {
        this.lvScripts.Items.Clear();
        if (this.pluginEngine == null)
          return;
        foreach (Command allCommand in this.pluginEngine.GetAllCommands())
        {
          if (!this.lvScripts.Items.ContainsKey(allCommand.Key) && (!allCommand.Name.Contains("[Code Sample]") || !this.chkHideSampleScripts.Checked))
          {
            string text = IniFile.GetValue<string>(Path.Combine(allCommand.Environment.CommandPath, "package.ini"), "Name", "Other");
            string hookDescription = PluginEngine.GetHookDescription(allCommand.Hook);
            ListViewGroup listViewGroup = this.lvScripts.Groups[hookDescription] ?? this.lvScripts.Groups.Add(hookDescription, hookDescription);
            ListViewItem listViewItem = this.lvScripts.Items.Add(allCommand.Key, allCommand.GetLocalizedName(), allCommand.Key);
            listViewItem.SubItems.Add(text);
            listViewItem.Tag = (object) allCommand;
            listViewItem.Checked = allCommand.Enabled;
            listViewItem.ToolTipText = allCommand.GetLocalizedDescription();
            listViewItem.Group = listViewGroup;
            Image commandImage = allCommand.CommandImage;
            if (commandImage != null)
              this.imageList.Images.Add(allCommand.Key, commandImage);
          }
        }
        this.lvScripts.SortGroups();
      }
      finally
      {
        this.lvScripts.EndUpdate();
      }
    }

    private void LoadKeyboard(string f)
    {
      try
      {
        this.keyboardShortcutEditor.Shortcuts.SetKeyMapping((IEnumerable<StringPair>) XmlUtility.Load<List<StringPair>>(f));
        this.keyboardShortcutEditor.RefreshList();
        Program.Settings.KeyboardLayouts.UpdateMostRecent(f);
      }
      catch (Exception ex)
      {
        Program.Settings.KeyboardLayouts.Remove(f);
        int num = (int) MessageBox.Show((IWin32Window) this, string.Format(TR.Messages["CouldNotImportKeyboardLayout", "Could not import Keyboard Layout!\nReason: {0}"], (object) ex.Message), TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    private bool InstallPlugin(string f)
    {
      if (Program.ScriptPackages.PackageFileExists(f) && !QuestionDialog.Ask((IWin32Window) this, PreferencesDialog.DuplicatePackageText, TR.Default["Yes", "Yes"]))
        return false;
      bool flag = Program.ScriptPackages.Install(f);
      this.NeedsRestart |= flag;
      return flag;
    }

    private void FillExtensionsList()
    {
      this.lbFormats.Items.Clear();
      foreach (FileFormat fileFormat in (IEnumerable<FileFormat>) Providers.Readers.GetSourceFormats().OrderBy<FileFormat, FileFormat>((Func<FileFormat, FileFormat>) (f => f)))
        this.lbFormats.SetItemChecked(this.lbFormats.Items.Add((object) fileFormat), fileFormat.IsShellRegistered("cYo.ComicRack"));
    }

    public void Apply()
    {
      string cultureName = ((TRInfo) this.lbLanguages.SelectedItem).CultureName;
      if (cultureName != Program.Settings.CultureName)
      {
        this.NeedsRestart = true;
        Program.Settings.CultureName = cultureName;
      }
      this.NeedsRestart = ((this.NeedsRestart ? 1 : 0) | (this.Plugins == null || string.IsNullOrEmpty(Program.Settings.PluginsStates) ? 0 : (this.Plugins.CommandStates != Program.Settings.PluginsStates ? 1 : 0))) != 0;
      FormUtility.RetrieveOptionsFromPanel((Control) this.pageBehavior, (object) Program.Settings);
      Program.Settings.MouseWheelSpeed = (float) this.tbMouseWheel.Value / 10f;
      Program.Settings.LibraryGaugesFormat = LibraryGauges.None.SetMask<LibraryGauges>(LibraryGauges.Unread, this.chkLibraryGaugesUnread.Checked).SetMask<LibraryGauges>(LibraryGauges.New, this.chkLibraryGaugesNew.Checked).SetMask<LibraryGauges>(LibraryGauges.Total, this.chkLibraryGaugesTotal.Checked).SetMask<LibraryGauges>(LibraryGauges.Numeric, this.chkLibraryGaugesNumeric.Checked);
      Program.Settings.DisplayLibraryGauges = this.chkLibraryGauges.Checked;
      Program.Settings.PageImageDisplayOptions = Program.Settings.PageImageDisplayOptions.SetMask<ImageDisplayOptions>(ImageDisplayOptions.AnamorphicScaling, this.chkAnamorphicScaling.Checked);
      Program.Settings.PageImageDisplayOptions = Program.Settings.PageImageDisplayOptions.SetMask<ImageDisplayOptions>(ImageDisplayOptions.HighQuality, this.chkHighQualityDisplay.Checked);
      Program.Settings.InternetCacheEnabled = this.chkEnableInternetCache.Checked;
      Program.Settings.InternetCacheSizeMB = (int) this.numInternetCacheSize.Value;
      Program.Settings.ThumbCacheEnabled = this.chkEnableThumbnailCache.Checked;
      Program.Settings.ThumbCacheSizeMB = (int) this.numThumbnailCacheSize.Value;
      Program.Settings.PageCacheEnabled = this.chkEnablePageCache.Checked;
      Program.Settings.PageCacheSizeMB = (int) this.numPageCacheSize.Value;
      Program.Settings.MaximumMemoryMB = this.tbMaximumMemoryUsage.Value * 32;
      Program.Settings.MemoryPageCacheOptimized = this.chkMemPageOptimized.Checked;
      Program.Settings.MemoryPageCacheCount = (int) this.numMemPageCount.Value;
      Program.Settings.MemoryThumbCacheOptimized = this.chkMemThumbOptimized.Checked;
      Program.Settings.MemoryThumbCacheSizeMB = (int) this.numMemThumbSize.Value;
      Program.Settings.GlobalColorAdjustment = new BitmapAdjustment((float) this.tbSaturation.Value / 100f, (float) this.tbBrightness.Value / 100f, (float) this.tbContrast.Value / 100f, (float) this.tbGamma.Value / 100f, this.chkAutoContrast.Checked ? BitmapAdjustmentOptions.AutoContrast : BitmapAdjustmentOptions.None, this.tbSharpening.Value);
      Program.Settings.ShowCurrentPageOverlay = this.chkShowCurrentPageOverlay.Checked;
      Program.Settings.ShowVisiblePagePartOverlay = this.chkShowVisiblePartOverlay.Checked;
      Program.Settings.ShowStatusOverlay = this.chkShowStatusOverlay.Checked;
      Program.Settings.ShowNavigationOverlay = this.chkShowNavigationOverlay.Checked;
      Program.Settings.NavigationOverlayOnTop = this.cbNavigationOverlayPosition.SelectedIndex == 1;
      Program.Settings.CurrentPageShowsName = this.chkShowPageNames.Checked;
      Program.Settings.HardwareAcceleration = this.chkEnableHardware.Checked;
      Program.Settings.SmoothScrolling = this.chkSmoothAutoScrolling.Checked;
      Program.Settings.DisplayChangeAnimation = this.chkEnableDisplayChangeAnimation.Checked;
      Program.Settings.SoftwareFiltering = this.chkEnableSoftwareFiltering.Checked;
      Program.Settings.HardwareFiltering = this.chkEnableHardwareFiltering.Checked;
      Program.Settings.FlowingMouseScrolling = this.chkEnableInertialMouseScrolling.Checked;
      Program.Settings.OverlayScaling = this.tbOverlayScaling.Value;
      Program.Settings.RemoveMissingFilesOnFullScan = this.chkAutoRemoveMissing.Checked;
      Program.Settings.DontAddRemoveFiles = this.chkDontAddRemovedFiles.Checked;
      if (!Program.Settings.DontAddRemoveFiles)
        Program.Database.ClearBlackList();
      Program.Settings.OverwriteAssociations = this.chkOverwriteAssociations.Checked;
      Program.Settings.LookForShared = this.chkLookForShared.Checked;
      Program.Settings.AutoConnectShares = this.chkAutoConnectShares.Checked;
      this.CopyWatchFoldersToDatabase();
      this.RegisterFileTypes();
      Program.Settings.UpdateComicFiles = this.chkUpdateComicFiles.Checked;
      Program.Settings.AutoUpdateComicsFiles = this.chkAutoUpdateComicFiles.Checked;
      Program.Settings.IgnoredCoverImages = string.IsNullOrEmpty(this.txCoverFilter.Text) ? (string) null : this.txCoverFilter.Text;
      Program.Settings.ScriptingLibraries = this.txLibraries.Text;
      Program.Settings.Scripting = !this.chkDisableScripting.Checked;
      Program.Settings.HideSampleScripts = this.chkHideSampleScripts.Checked;
      if (!string.IsNullOrEmpty(this.BackupFile))
      {
        try
        {
          try
          {
            AutomaticProgressDialog.Process((Form) this, TR.Messages["DatabaseRestore", "Database Restore"], TR.Messages["DatabaseRestoreText", "Restoring database from Backup file"], 1000, (Action) (() => Program.DatabaseManager.RestoreFrom(this.BackupFile, Program.Paths.CustomThumbnailPath)), AutomaticProgressDialogOptions.None);
          }
          catch (Exception ex)
          {
            int num = (int) MessageBox.Show((IWin32Window) this, TR.Messages["DatabaseRestoreError", "There was an error restoring the Database Backup"], TR.Messages["Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          }
          this.BackupFile = (string) null;
          this.NeedsRestart = true;
        }
        catch
        {
        }
      }
      List<ComicLibraryServerConfig> list = this.tabShares.TabPages.OfType<Control>().Select<Control, ServerEditControl>((Func<Control, ServerEditControl>) (p => p.Controls[0] as ServerEditControl)).Select<ServerEditControl, ComicLibraryServerConfig>((Func<ServerEditControl, ComicLibraryServerConfig>) (se => se.Config)).ToList<ComicLibraryServerConfig>();
      if (!list.SequenceEqual<ComicLibraryServerConfig>((IEnumerable<ComicLibraryServerConfig>) Program.Settings.Shares, (IEqualityComparer<ComicLibraryServerConfig>) new ComicLibraryServerConfig.EqualityComparer()))
      {
        this.NeedsRestart = true;
        Program.Settings.Shares.Clear();
        Program.Settings.Shares.AddRange((IEnumerable<ComicLibraryServerConfig>) list);
      }
      string str = this.txPublicServerAddress.Text.Trim();
      if (str != Program.Settings.ExternalServerAddress)
      {
        this.NeedsRestart = true;
        Program.Settings.ExternalServerAddress = str;
      }
      if (this.txPrivateListingPassword.Password != Program.Settings.PrivateListingPassword)
      {
        this.NeedsRestart = true;
        Program.Settings.PrivateListingPassword = this.txPrivateListingPassword.Password;
      }
      Program.Settings.ExtraWifiDeviceAddresses = this.txWifiAddresses.Text;
      Program.RefreshAllWindows();
      Program.ForAllForms((Action<Form>) (f => f.FindServices<ISettingsChanged>().ForEach<ISettingsChanged>((Action<ISettingsChanged>) (s => s.SettingsChanged()))));
    }

    private void SetScanButtonText()
    {
      this.btScan.Text = Program.Scanner.IsScanning ? LocalizeUtility.GetText((Control) this, "Stop", "Stop") : LocalizeUtility.GetText((Control) this, "Scan", "Scan");
    }

    private void SetSettings()
    {
      FormUtility.FillPanelWithOptions((Control) this.pageBehavior, (object) Program.Settings, TR.Load("Settings"));
      this.chkLibraryGauges.Checked = Program.Settings.DisplayLibraryGauges;
      this.chkLibraryGaugesUnread.Checked = Program.Settings.LibraryGaugesFormat.IsSet<LibraryGauges>(LibraryGauges.Unread);
      this.chkLibraryGaugesNew.Checked = Program.Settings.LibraryGaugesFormat.IsSet<LibraryGauges>(LibraryGauges.New);
      this.chkLibraryGaugesTotal.Checked = Program.Settings.LibraryGaugesFormat.IsSet<LibraryGauges>(LibraryGauges.Total);
      this.chkLibraryGaugesNumeric.Checked = Program.Settings.LibraryGaugesFormat.IsSet<LibraryGauges>(LibraryGauges.Numeric);
      this.tbMouseWheel.Value = (int) ((double) Program.Settings.MouseWheelSpeed * 10.0);
      this.chkHighQualityDisplay.Checked = Program.Settings.PageImageDisplayOptions.IsSet<ImageDisplayOptions>(ImageDisplayOptions.HighQuality);
      this.chkAnamorphicScaling.Checked = Program.Settings.PageImageDisplayOptions.IsSet<ImageDisplayOptions>(ImageDisplayOptions.AnamorphicScaling);
      this.chkAutoRemoveMissing.Checked = Program.Settings.RemoveMissingFilesOnFullScan;
      this.chkDontAddRemovedFiles.Checked = Program.Settings.DontAddRemoveFiles;
      this.tbSaturation.Value = (int) ((double) Program.Settings.GlobalColorAdjustment.Saturation * 100.0);
      this.tbBrightness.Value = (int) ((double) Program.Settings.GlobalColorAdjustment.Brightness * 100.0);
      this.tbContrast.Value = (int) ((double) Program.Settings.GlobalColorAdjustment.Contrast * 100.0);
      this.tbGamma.Value = (int) ((double) Program.Settings.GlobalColorAdjustment.Gamma * 100.0);
      this.tbSharpening.Value = Program.Settings.GlobalColorAdjustment.Sharpen;
      this.chkAutoContrast.Checked = Program.Settings.GlobalColorAdjustment.Options.IsSet<BitmapAdjustmentOptions>(BitmapAdjustmentOptions.AutoContrast);
      this.chkShowCurrentPageOverlay.Checked = Program.Settings.ShowCurrentPageOverlay;
      this.chkShowVisiblePartOverlay.Checked = Program.Settings.ShowVisiblePagePartOverlay;
      this.chkShowStatusOverlay.Checked = Program.Settings.ShowStatusOverlay;
      this.chkShowNavigationOverlay.Checked = Program.Settings.ShowNavigationOverlay;
      this.cbNavigationOverlayPosition.SelectedIndex = Program.Settings.NavigationOverlayOnTop ? 1 : 0;
      this.chkShowPageNames.Checked = Program.Settings.CurrentPageShowsName;
      this.chkEnableHardware.Checked = Program.Settings.HardwareAcceleration;
      this.chkSmoothAutoScrolling.Checked = Program.Settings.SmoothScrolling;
      this.chkEnableDisplayChangeAnimation.Checked = Program.Settings.DisplayChangeAnimation;
      this.chkEnableSoftwareFiltering.Checked = Program.Settings.SoftwareFiltering;
      this.chkEnableHardwareFiltering.Checked = Program.Settings.HardwareFiltering;
      this.chkEnableInertialMouseScrolling.Checked = Program.Settings.FlowingMouseScrolling;
      this.chkEnableInternetCache.Checked = Program.Settings.InternetCacheEnabled;
      this.numInternetCacheSize.Value = (Decimal) this.numInternetCacheSize.Clamp(Program.Settings.InternetCacheSizeMB);
      this.chkEnableThumbnailCache.Checked = Program.Settings.ThumbCacheEnabled;
      this.numThumbnailCacheSize.Value = (Decimal) this.numThumbnailCacheSize.Clamp(Program.Settings.ThumbCacheSizeMB);
      this.chkEnablePageCache.Checked = Program.Settings.PageCacheEnabled;
      this.numPageCacheSize.Value = (Decimal) this.numPageCacheSize.Clamp(Program.Settings.PageCacheSizeMB);
      this.chkMemPageOptimized.Checked = Program.Settings.MemoryPageCacheOptimized;
      this.numMemPageCount.Value = (Decimal) this.numMemPageCount.Clamp(Program.Settings.MemoryPageCacheCount);
      this.chkMemThumbOptimized.Checked = Program.Settings.MemoryThumbCacheOptimized;
      this.numMemThumbSize.Value = (Decimal) this.numMemThumbSize.Clamp(Program.Settings.MemoryThumbCacheSizeMB);
      this.tbMaximumMemoryUsage.SetRange(2, 32);
      this.tbMaximumMemoryUsage.Value = Program.Settings.MaximumMemoryMB / 32;
      this.tbOverlayScaling.Value = Program.Settings.OverlayScaling;
      this.chkOverwriteAssociations.Checked = Program.Settings.OverwriteAssociations;
      this.chkUpdateComicFiles.Checked = Program.Settings.UpdateComicFiles;
      this.chkAutoUpdateComicFiles.Checked = Program.Settings.AutoUpdateComicsFiles;
      this.txCoverFilter.Text = Program.Settings.IgnoredCoverImages;
      this.txLibraries.Text = Program.Settings.ScriptingLibraries;
      this.chkDisableScripting.Checked = !Program.Settings.Scripting;
      this.chkHideSampleScripts.Checked = Program.Settings.HideSampleScripts;
      this.lbLanguages.SelectedIndex = 0;
      foreach (TRInfo trInfo in this.lbLanguages.Items)
      {
        if (trInfo.CultureName == Program.Settings.CultureName)
        {
          this.lbLanguages.SelectedItem = (object) trInfo;
          break;
        }
      }
      this.txPublicServerAddress.Text = Program.Settings.ExternalServerAddress;
      this.txPrivateListingPassword.Password = Program.Settings.PrivateListingPassword;
      this.chkLookForShared.Checked = Program.Settings.LookForShared;
      this.chkAutoConnectShares.Checked = Program.Settings.AutoConnectShares;
      foreach (ComicLibraryServerConfig share in Program.Settings.Shares)
        this.AddSharePage(share);
      this.txWifiAddresses.Text = Program.Settings.ExtraWifiDeviceAddresses;
    }

    private void AddSharePage(ComicLibraryServerConfig cfg)
    {
      TabPage tab = new TabPage(cfg.Name)
      {
        UseVisualStyleBackColor = true
      };
      ServerEditControl serverEditControl = new ServerEditControl();
      serverEditControl.Dock = DockStyle.Fill;
      serverEditControl.Config = cfg;
      serverEditControl.BackColor = Color.Transparent;
      ServerEditControl sc = serverEditControl;
      sc.ShareNameChanged += (EventHandler) ((s, e) => tab.Text = sc.ShareName);
      tab.Controls.Add((Control) sc);
      this.tabShares.TabPages.Add(tab);
    }

    private void RemoveSharePage(TabPage tab)
    {
      Control control = tab.Controls[0];
      tab.Controls.Remove(control);
      control.Dispose();
      this.tabShares.TabPages.Remove(tab);
      tab.Dispose();
    }

    private void CopyWatchFoldersToDatabase()
    {
      Program.Database.WatchFolders.Clear();
      for (int index = 0; index < this.lbPaths.Items.Count; ++index)
        Program.Database.WatchFolders.Add(new WatchFolder(this.lbPaths.Items[index].ToString(), this.lbPaths.GetItemChecked(index)));
    }

    private void RegisterFileTypes()
    {
      for (int index = 0; index < this.lbFormats.Items.Count; ++index)
      {
        FileFormat fileFormat = (FileFormat) this.lbFormats.Items[index];
        if (this.lbFormats.GetItemChecked(index))
          fileFormat.RegisterShell("cYo.ComicRack", "eComic", Program.Settings.OverwriteAssociations);
        else
          fileFormat.UnregisterShell("cYo.ComicRack");
      }
    }

    private void SetTab(CheckBox cb)
    {
      if (this.blockSetTab)
        return;
      this.blockSetTab = true;
      try
      {
        foreach (CheckBox tabButton in this.tabButtons)
        {
          tabButton.Checked = tabButton == cb;
          Control tag = tabButton.Tag as Control;
          if (tag.Tag is Control)
            tag = tag.Tag as Control;
          tag.Visible = tabButton.Checked;
        }
      }
      finally
      {
        this.blockSetTab = false;
      }
    }

    private void UpdateDiskCacheStatus(object sender, EventArgs e)
    {
      if (this.BeginInvokeIfRequired((Action) (() => this.UpdateDiskCacheStatus(sender, e))))
        return;
      this.lblInternetCacheUsage.Text = string.Format("({0}/{1})", (object) Program.InternetCache.Count, (object) string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
      {
        (object) Program.InternetCache.Size
      }));
      this.lblPageCacheUsage.Text = string.Format("({0}/{1})", (object) Program.ImagePool.Pages.DiskCache.Count, (object) string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
      {
        (object) Program.ImagePool.Pages.DiskCache.Size
      }));
      this.lblThumbCacheUsage.Text = string.Format("({0}/{1})", (object) Program.ImagePool.Thumbs.DiskCache.Count, (object) string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
      {
        (object) Program.ImagePool.Thumbs.DiskCache.Size
      }));
    }

    private void UpdateMemoryCacheStatus()
    {
      if (this.BeginInvokeIfRequired(new Action(this.UpdateMemoryCacheStatus)))
        return;
      this.lblPageMemCacheUsage.Text = string.Format("({0}/{1})", (object) Program.ImagePool.Pages.MemoryCache.Count, (object) string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
      {
        (object) Program.ImagePool.Pages.MemoryCache.Size
      }));
      this.lblThumbMemCacheUsage.Text = string.Format("({0}/{1})", (object) Program.ImagePool.Thumbs.MemoryCache.Count, (object) string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
      {
        (object) Program.ImagePool.Thumbs.MemoryCache.Size
      }));
    }

    private void RefreshPackageList()
    {
      this.lvPackages.Items.Clear();
      foreach (PackageManager.Package package in Program.ScriptPackages.GetPackages().OrderBy<PackageManager.Package, PackageManager.PackageType>((Func<PackageManager.Package, PackageManager.PackageType>) (p => p.PackageType)).Reverse<PackageManager.Package>())
      {
        ListViewItem listViewItem1 = this.lvPackages.Items.Add(package.Name, package.Name, 0);
        if (package.Image != null)
        {
          this.packageImageList.Images.Add(package.Image);
          listViewItem1.ImageIndex = this.packageImageList.Images.Count - 1;
        }
        listViewItem1.Tag = (object) package;
        if (!string.IsNullOrEmpty(package.Version))
        {
          ListViewItem listViewItem2 = listViewItem1;
          listViewItem2.Text = listViewItem2.Text + " V" + package.Version;
        }
        listViewItem1.SubItems.Add(package.Author);
        listViewItem1.SubItems.Add(package.Description);
        switch (package.PackageType)
        {
          case PackageManager.PackageType.PendingInstall:
            listViewItem1.Group = this.lvPackages.Groups["packageGroupInstall"];
            continue;
          case PackageManager.PackageType.PendingRemove:
            listViewItem1.Group = this.lvPackages.Groups["packageGroupRemove"];
            continue;
          default:
            listViewItem1.Group = this.lvPackages.Groups["packageGroupInstalled"];
            continue;
        }
      }
    }

    private void btAssociateExtensions_Click(object sender, EventArgs e)
    {
      string str = "-rf \"" + (this.chkOverwriteAssociations.Checked ? "!" : string.Empty);
      for (int index = 0; index < this.lbFormats.Items.Count; ++index)
      {
        FileFormat fileFormat = (FileFormat) this.lbFormats.Items[index];
        if (index != 0)
          str += ",";
        if (!this.lbFormats.GetItemChecked(index))
          str += "-";
        str += fileFormat.Name;
      }
      if (ProcessRunner.RunElevated(Application.ExecutablePath, str + "\"") != 0)
        return;
      this.FillExtensionsList();
    }

    private void btAddShare_Click(object sender, EventArgs e)
    {
      ComicLibraryServerConfig cfg = new ComicLibraryServerConfig();
      cfg.Name = string.Format("{0}'s Library", (object) Environment.UserName);
      if (this.tabShares.TabCount > 1)
        cfg.Name += string.Format(" ({0})", (object) (this.tabShares.TabCount + 1));
      this.AddSharePage(cfg);
      this.tabShares.SelectedIndex = this.tabShares.TabPages.Count - 1;
    }

    private void btRmoveShare_Click(object sender, EventArgs e)
    {
      TabPage selectedTab = this.tabShares.SelectedTab;
      if (selectedTab == null)
        return;
      this.RemoveSharePage(selectedTab);
    }

    public static bool Show(
      IWin32Window parent,
      KeyboardShortcuts commands,
      PluginEngine pe,
      string autoInstallPlugin = null)
    {
      using (PreferencesDialog preferencesDialog = new PreferencesDialog())
      {
        preferencesDialog.keyboardShortcutEditor.Shortcuts = commands;
        preferencesDialog.Plugins = pe;
        preferencesDialog.AutoInstallPlugin = File.Exists(autoInstallPlugin) ? autoInstallPlugin : (string) null;
        bool flag = preferencesDialog.ShowDialog(parent) == DialogResult.OK;
        if (flag)
        {
          preferencesDialog.Apply();
          if (preferencesDialog.NeedsRestart && QuestionDialog.Ask(parent, TR.Messages["RestartQuestion", "ComicRack needs to restart to complete the operation! Do you want to restart now?"], TR.Default["Restart", "Restart"]))
            Program.MainForm.MenuRestart();
        }
        else
          Program.ScriptPackages.RemovePending();
        return flag;
      }
    }
  }
}

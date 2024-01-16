// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ExportComicsDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Reflection;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Provider;
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
  public class ExportComicsDialog : Form
  {
    private readonly EnumMenuUtility enumUtil;
    private ExportSettingCollection defaultPresets = new ExportSettingCollection();
    private ExportSettingCollection userPresets = new ExportSettingCollection();
    private static List<bool> expandedStates;
    private IContainer components;
    private CollapsibleGroupBox grpExportLocation;
    private ComboBox cbExport;
    private Label labelExportTo;
    private Button btChooseFolder;
    private Label txFolder;
    private Label labelFolder;
    private CollapsibleGroupBox grpFileFormat;
    private ComboBox cbPageFormat;
    private Label labelPageFormat;
    private Label labelPageQuality;
    private Label labelRemovePageFilter;
    private TrackBarLite tbQuality;
    private ComboBox cbComicFormat;
    private Label labelComicFormat;
    private Label txRemovedPages;
    private CheckBox chkEmbedComicInfo;
    private CollapsibleGroupBox grpPageFormat;
    private Label labelPageResize;
    private ComboBox cbPageResize;
    private CheckBox chkDontEnlarge;
    private NumericUpDown txHeight;
    private NumericUpDown txWidth;
    private CheckBox chkAddNewToLibrary;
    private CheckBox chkDeleteOriginal;
    private ComboBox cbCompression;
    private Label labelCompression;
    private Button btCancel;
    private Button btOK;
    private Label labelX;
    private TreeView tvPresets;
    private Button btSavePreset;
    private Button btRemovePreset;
    private ContextMenuStrip contextRemovePageFilter;
    private Button btRemovePageFilter;
    private Panel exportSettings;
    private CollapsibleGroupBox grpFileNaming;
    private ComboBox cbNamingTemplate;
    private Label labelNamingTemplate;
    private Label labelCustomNaming;
    private TextBox txCustomName;
    private NumericUpDown txCustomStartIndex;
    private Label labelCustomStartIndex;
    private CheckBox chkOverwrite;
    private CollapsibleGroupBox grpImageProcessing;
    private CheckBox chkAutoContrast;
    private TrackBarLite tbSharpening;
    private Label labelSharpening;
    private Button btResetColors;
    private Label labelBrightness;
    private TrackBarLite tbContrast;
    private Label labelSaturation;
    private TrackBarLite tbBrightness;
    private TrackBarLite tbSaturation;
    private Label labelContrast;
    private ComboBox cbImageProcessingSource;
    private Label labelImagProcessingSource;
    private GroupBox grpCustomProcessing;
    private Label labelImageProcessingCustom;
    private CheckBox chkIgnoreErrorPages;
    private ToolTip toolTip;
    private CheckBox chkCombine;
    private TextBox txIncludePages;
    private Label labelIncludePages;
    private Label labelGamma;
    private TrackBarLite tbGamma;
    private Label labelDoublePages;
    private ComboBox cbDoublePages;
    private CheckBox chkKeepOriginalNames;

    public ExportComicsDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      foreach (ComboBox control in this.GetControls<ComboBox>())
        LocalizeUtility.Localize(TR.Load(this.Name), control);
      this.RestorePosition();
      this.RestorePanelStates();
      FormUtility.RegisterPanelToTabToggle((Control) this.exportSettings, PropertyCaller.CreateFlagsValueStore<Settings, TabLayouts>(Program.Settings, "TabLayouts", TabLayouts.Export));
      foreach (object obj in (IEnumerable<FileFormat>) Providers.Writers.GetSourceFormats().OrderBy<FileFormat, FileFormat>((Func<FileFormat, FileFormat>) (f => f)))
        this.cbComicFormat.Items.Add(obj);
      this.enumUtil = new EnumMenuUtility((ToolStripDropDown) this.contextRemovePageFilter, typeof (ComicPageType), true, (IDictionary<int, Image>) null, Keys.None);
      this.enumUtil.ValueChanged += new EventHandler(this.enumUtil_ValueChanged);
      NiceTreeSkin niceTreeSkin = new NiceTreeSkin(this.tvPresets);
      IdleProcess.Idle += new EventHandler(this.OnIdle);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
      {
        IdleProcess.Idle -= new EventHandler(this.OnIdle);
        this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public ExportSettingCollection DefaultPresets
    {
      get => this.defaultPresets;
      set
      {
        this.defaultPresets = value;
        this.BuildPresetsList();
      }
    }

    public ExportSettingCollection UserPresets
    {
      get => this.userPresets;
      set
      {
        this.userPresets = value;
        this.BuildPresetsList();
      }
    }

    public int FormatId
    {
      get
      {
        object selectedItem = this.cbComicFormat.SelectedItem;
        return !(selectedItem is string) ? ((FileFormat) selectedItem).Id : 0;
      }
      set
      {
        foreach (object obj in this.cbComicFormat.Items)
        {
          if ((obj is string ? 0 : ((FileFormat) obj).Id) == value)
          {
            this.cbComicFormat.SelectedItem = obj;
            return;
          }
        }
        this.cbComicFormat.SelectedIndex = 0;
      }
    }

    public string SettingName { get; set; }

    public ExportSetting Setting
    {
      get
      {
        ExportSetting setting = new ExportSetting();
        setting.Name = this.SettingName;
        setting.Target = (ExportTarget) this.cbExport.SelectedIndex;
        setting.TargetFolder = this.txFolder.Text;
        setting.DeleteOriginal = this.chkDeleteOriginal.Checked;
        setting.AddToLibrary = this.chkAddNewToLibrary.Checked;
        setting.Overwrite = this.chkOverwrite.Checked;
        setting.Combine = this.chkCombine.Checked;
        setting.Naming = (ExportNaming) this.cbNamingTemplate.SelectedIndex;
        setting.CustomName = this.txCustomName.Text.Trim();
        setting.CustomNamingStart = (int) this.txCustomStartIndex.Value;
        setting.FormatId = this.FormatId;
        setting.ComicCompression = (ExportCompression) this.cbCompression.SelectedIndex;
        setting.EmbedComicInfo = this.chkEmbedComicInfo.Checked;
        setting.RemovePageFilter = (ComicPageType) this.enumUtil.Value;
        setting.IncludePages = this.txIncludePages.Text;
        setting.PageType = (StoragePageType) this.cbPageFormat.SelectedIndex;
        setting.PageCompression = this.tbQuality.Value;
        setting.PageResize = (StoragePageResize) this.cbPageResize.SelectedIndex;
        setting.PageWidth = (int) this.txWidth.Value;
        setting.PageHeight = (int) this.txHeight.Value;
        setting.DontEnlarge = this.chkDontEnlarge.Checked;
        setting.DoublePages = (DoublePageHandling) this.cbDoublePages.SelectedIndex;
        setting.IgnoreErrorPages = this.chkIgnoreErrorPages.Checked;
        setting.KeepOriginalImageNames = this.chkKeepOriginalNames.Checked;
        setting.ImageProcessingSource = (ExportImageProcessingSource) this.cbImageProcessingSource.SelectedIndex;
        setting.ImageProcessing = new BitmapAdjustment((float) this.tbSaturation.Value / 100f, (float) this.tbBrightness.Value / 100f, (float) this.tbContrast.Value / 100f, (float) this.tbGamma.Value / 100f, Color.White, this.chkAutoContrast.Checked ? BitmapAdjustmentOptions.AutoContrast : BitmapAdjustmentOptions.None, this.tbSharpening.Value);
        return setting;
      }
      set
      {
        this.SettingName = value.Name;
        this.cbExport.SelectedIndex = (int) value.Target;
        this.txFolder.Text = string.IsNullOrEmpty(value.TargetFolder) ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : value.TargetFolder;
        this.chkDeleteOriginal.Checked = value.DeleteOriginal;
        this.chkAddNewToLibrary.Checked = value.AddToLibrary;
        this.chkOverwrite.Checked = value.Overwrite;
        this.chkCombine.Checked = value.Combine;
        this.cbNamingTemplate.SelectedIndex = (int) value.Naming;
        this.txCustomName.Text = value.CustomName;
        this.txCustomStartIndex.Value = (Decimal) value.CustomNamingStart;
        this.FormatId = value.FormatId;
        this.cbCompression.SelectedIndex = (int) value.ComicCompression;
        this.chkEmbedComicInfo.Checked = value.EmbedComicInfo;
        this.enumUtil.Value = (int) value.RemovePageFilter;
        this.txIncludePages.Text = value.IncludePages;
        this.cbPageFormat.SelectedIndex = (int) value.PageType;
        this.tbQuality.Value = value.PageCompression;
        this.cbPageResize.SelectedIndex = (int) value.PageResize;
        this.txWidth.Value = (Decimal) value.PageWidth;
        this.txHeight.Value = (Decimal) value.PageHeight;
        this.chkDontEnlarge.Checked = value.DontEnlarge;
        this.cbDoublePages.SelectedIndex = (int) value.DoublePages;
        this.chkIgnoreErrorPages.Checked = value.IgnoreErrorPages;
        this.chkKeepOriginalNames.Checked = value.KeepOriginalImageNames;
        this.cbImageProcessingSource.SelectedIndex = (int) value.ImageProcessingSource;
        this.tbSaturation.Value = (int) ((double) value.ImageProcessing.Saturation * 100.0);
        this.tbBrightness.Value = (int) ((double) value.ImageProcessing.Brightness * 100.0);
        this.tbContrast.Value = (int) ((double) value.ImageProcessing.Contrast * 100.0);
        this.tbGamma.Value = (int) ((double) value.ImageProcessing.Gamma * 100.0);
        this.tbSharpening.Value = value.ImageProcessing.Sharpen;
        this.chkAutoContrast.Checked = (value.ImageProcessing.Options & BitmapAdjustmentOptions.AutoContrast) != 0;
      }
    }

    private void OnIdle(object sender, EventArgs e)
    {
      ExportSetting setting = this.Setting;
      this.btChooseFolder.Enabled = setting.Target == ExportTarget.NewFolder;
      this.chkOverwrite.Enabled = this.chkDeleteOriginal.Enabled = this.chkAddNewToLibrary.Enabled = setting.Target != ExportTarget.ReplaceSource;
      this.txCustomStartIndex.Enabled = setting.Naming == ExportNaming.Custom;
      this.txCustomName.Enabled = setting.Naming == ExportNaming.Custom || setting.Naming == ExportNaming.Caption;
      this.tbQuality.Enabled = setting.PageType == StoragePageType.Jpeg || setting.PageType == StoragePageType.Webp;
      this.txWidth.Enabled = setting.PageResize != StoragePageResize.Height && setting.PageResize != 0;
      this.txHeight.Enabled = setting.PageResize != StoragePageResize.Width && setting.PageResize != 0;
      this.chkDontEnlarge.Enabled = setting.PageResize != 0;
      this.btRemovePreset.Enabled = this.tvPresets.SelectedNode != null && this.tvPresets.SelectedNode.Parent != null && (bool) this.tvPresets.SelectedNode.Parent.Tag;
      this.grpCustomProcessing.Enabled = setting.ImageProcessingSource == ExportImageProcessingSource.Custom;
    }

    private void tbQuality_ValueChanged(object sender, EventArgs e)
    {
      this.toolTip.SetToolTip((Control) this.tbQuality, this.tbQuality.Value.ToString());
    }

    private void btRemovePreset_Click(object sender, EventArgs e)
    {
      this.OnIdle((object) this, EventArgs.Empty);
      if (!this.btRemovePreset.Enabled)
        return;
      this.userPresets.Remove(this.tvPresets.SelectedNode.Tag as ExportSetting);
      this.tvPresets.SelectedNode.Remove();
    }

    private void btAddPreset_Click(object sender, EventArgs e)
    {
      string itemValue = string.IsNullOrEmpty(this.Setting.Name) ? ExportSetting.DefaultName : this.Setting.Name;
      string name = SelectItemDialog.GetName((IWin32Window) this, TR.Load(this.Name)["AddConvertPreset", "Add Export Preset"], itemValue);
      if (string.IsNullOrEmpty(name))
        return;
      this.SettingName = name;
      this.UserPresets.RemoveAll((Predicate<ExportSetting>) (x => x.Name == name));
      this.UserPresets.Add(this.Setting);
      this.BuildPresetsList();
    }

    private void enumUtil_ValueChanged(object sender, EventArgs e)
    {
      this.txRemovedPages.Text = this.enumUtil.Text;
    }

    private void btRemovePageFilter_Click(object sender, EventArgs e)
    {
      this.contextRemovePageFilter.Show((Control) this.btRemovePageFilter, 0, this.btRemovePageFilter.Height);
    }

    private void tvPresets_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (!(this.tvPresets.SelectedNode.Tag is ExportSetting tag))
        return;
      this.Setting = tag;
    }

    private void btChooseFolder_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
        folderBrowserDialog.Description = TR.Load(this.Name)["SelectExportFolder", "Please select the Export Folder"];
        folderBrowserDialog.SelectedPath = this.txFolder.Text;
        folderBrowserDialog.ShowNewFolderButton = true;
        if (folderBrowserDialog.ShowDialog((IWin32Window) this) == DialogResult.Cancel)
          return;
        this.txFolder.Text = folderBrowserDialog.SelectedPath;
      }
    }

    private void btResetColors_Click(object sender, EventArgs e)
    {
      this.tbSaturation.Value = this.tbBrightness.Value = this.tbContrast.Value = this.tbGamma.Value = 0;
      this.tbSharpening.Value = 0;
    }

    private void AdjustmentSliderChanged(object sender, EventArgs e)
    {
      TrackBarLite trackBarLite = (TrackBarLite) sender;
      this.toolTip.SetToolTip((Control) trackBarLite, string.Format("{1}{0}%", (object) trackBarLite.Value, trackBarLite.Value > 0 ? (object) "+" : (object) string.Empty));
    }

    private void cbNamingTemplate_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.cbNamingTemplate.SelectedIndex != 1)
        this.txCustomName.Text = string.Empty;
      else
        this.txCustomName.Text = EngineConfiguration.Default.ComicExportFileNameFormat;
    }

    private void BuildPresetsList()
    {
      this.tvPresets.Nodes.Clear();
      ExportComicsDialog.AddNodes(this.tvPresets, false, TR.Load(this.Name)["ComicRackExportPresets", "ComicRack Presets"], (ICollection<ExportSetting>) this.DefaultPresets);
      ExportComicsDialog.AddNodes(this.tvPresets, true, TR.Load(this.Name)["UserExportPresets", "User Presets"], (ICollection<ExportSetting>) this.UserPresets);
      this.tvPresets.ExpandAll();
    }

    private static void AddNodes(
      TreeView tvPresets,
      bool canDelete,
      string groupName,
      ICollection<ExportSetting> settings)
    {
      if (settings.Count == 0)
        return;
      TreeNode treeNode = tvPresets.Nodes.Add(groupName);
      treeNode.Tag = (object) canDelete;
      foreach (ExportSetting setting in (IEnumerable<ExportSetting>) settings)
        treeNode.Nodes.Add(setting.Name).Tag = (object) setting;
    }

    public static ExportSetting Show(
      IWin32Window parent,
      ExportSettingCollection defaultPresets,
      ExportSettingCollection userPresets,
      ExportSetting setting)
    {
      using (ExportComicsDialog container = new ExportComicsDialog())
      {
        container.DefaultPresets = defaultPresets;
        container.UserPresets = userPresets;
        container.Setting = setting;
        if (ExportComicsDialog.expandedStates != null)
        {
          int k = 0;
          container.ForEachControl<CollapsibleGroupBox>((Action<CollapsibleGroupBox>) (x => x.Collapsed = ExportComicsDialog.expandedStates[k++]));
        }
        ExportSetting setting1 = container.ShowDialog(parent) == DialogResult.OK ? container.Setting : (ExportSetting) null;
        ExportComicsDialog.expandedStates = new List<bool>();
        container.ForEachControl<CollapsibleGroupBox>((Action<CollapsibleGroupBox>) (x => ExportComicsDialog.expandedStates.Add(x.Collapsed)));
        return setting1;
      }
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.contextRemovePageFilter = new ContextMenuStrip(this.components);
      this.btCancel = new Button();
      this.btOK = new Button();
      this.tvPresets = new TreeView();
      this.btSavePreset = new Button();
      this.btRemovePreset = new Button();
      this.exportSettings = new Panel();
      this.grpImageProcessing = new CollapsibleGroupBox();
      this.grpCustomProcessing = new GroupBox();
      this.labelGamma = new Label();
      this.tbGamma = new TrackBarLite();
      this.tbSaturation = new TrackBarLite();
      this.labelContrast = new Label();
      this.tbBrightness = new TrackBarLite();
      this.labelSaturation = new Label();
      this.tbSharpening = new TrackBarLite();
      this.tbContrast = new TrackBarLite();
      this.labelSharpening = new Label();
      this.labelBrightness = new Label();
      this.btResetColors = new Button();
      this.labelImageProcessingCustom = new Label();
      this.cbImageProcessingSource = new ComboBox();
      this.labelImagProcessingSource = new Label();
      this.chkAutoContrast = new CheckBox();
      this.grpPageFormat = new CollapsibleGroupBox();
      this.labelDoublePages = new Label();
      this.cbDoublePages = new ComboBox();
      this.chkIgnoreErrorPages = new CheckBox();
      this.txHeight = new NumericUpDown();
      this.txWidth = new NumericUpDown();
      this.chkDontEnlarge = new CheckBox();
      this.labelPageResize = new Label();
      this.cbPageResize = new ComboBox();
      this.labelPageFormat = new Label();
      this.cbPageFormat = new ComboBox();
      this.labelPageQuality = new Label();
      this.tbQuality = new TrackBarLite();
      this.labelX = new Label();
      this.grpFileFormat = new CollapsibleGroupBox();
      this.txIncludePages = new TextBox();
      this.labelIncludePages = new Label();
      this.btRemovePageFilter = new Button();
      this.cbCompression = new ComboBox();
      this.labelCompression = new Label();
      this.chkEmbedComicInfo = new CheckBox();
      this.cbComicFormat = new ComboBox();
      this.labelComicFormat = new Label();
      this.txRemovedPages = new Label();
      this.labelRemovePageFilter = new Label();
      this.grpFileNaming = new CollapsibleGroupBox();
      this.txCustomStartIndex = new NumericUpDown();
      this.labelCustomStartIndex = new Label();
      this.txCustomName = new TextBox();
      this.labelCustomNaming = new Label();
      this.cbNamingTemplate = new ComboBox();
      this.labelNamingTemplate = new Label();
      this.grpExportLocation = new CollapsibleGroupBox();
      this.chkCombine = new CheckBox();
      this.chkOverwrite = new CheckBox();
      this.chkAddNewToLibrary = new CheckBox();
      this.chkDeleteOriginal = new CheckBox();
      this.btChooseFolder = new Button();
      this.txFolder = new Label();
      this.labelFolder = new Label();
      this.cbExport = new ComboBox();
      this.labelExportTo = new Label();
      this.toolTip = new ToolTip(this.components);
      this.chkKeepOriginalNames = new CheckBox();
      this.exportSettings.SuspendLayout();
      this.grpImageProcessing.SuspendLayout();
      this.grpCustomProcessing.SuspendLayout();
      this.grpPageFormat.SuspendLayout();
      this.txHeight.BeginInit();
      this.txWidth.BeginInit();
      this.grpFileFormat.SuspendLayout();
      this.grpFileNaming.SuspendLayout();
      this.txCustomStartIndex.BeginInit();
      this.grpExportLocation.SuspendLayout();
      this.SuspendLayout();
      this.contextRemovePageFilter.Name = "contextRemovePageFilter";
      this.contextRemovePageFilter.Size = new System.Drawing.Size(61, 4);
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(660, 440);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 7;
      this.btCancel.Text = "&Cancel";
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(574, 440);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 6;
      this.btOK.Text = "&OK";
      this.tvPresets.Location = new System.Drawing.Point(14, 18);
      this.tvPresets.Name = "tvPresets";
      this.tvPresets.ShowNodeToolTips = true;
      this.tvPresets.Size = new System.Drawing.Size(220, 387);
      this.tvPresets.TabIndex = 3;
      this.tvPresets.AfterSelect += new TreeViewEventHandler(this.tvPresets_AfterSelect);
      this.btSavePreset.Location = new System.Drawing.Point(14, 411);
      this.btSavePreset.Name = "btSavePreset";
      this.btSavePreset.Size = new System.Drawing.Size(107, 23);
      this.btSavePreset.TabIndex = 4;
      this.btSavePreset.Text = "Save,,,";
      this.btSavePreset.UseVisualStyleBackColor = true;
      this.btSavePreset.Click += new EventHandler(this.btAddPreset_Click);
      this.btRemovePreset.Location = new System.Drawing.Point((int) sbyte.MaxValue, 411);
      this.btRemovePreset.Name = "btRemovePreset";
      this.btRemovePreset.Size = new System.Drawing.Size(107, 23);
      this.btRemovePreset.TabIndex = 5;
      this.btRemovePreset.Text = "Remove";
      this.btRemovePreset.UseVisualStyleBackColor = true;
      this.btRemovePreset.Click += new EventHandler(this.btRemovePreset_Click);
      this.exportSettings.AutoScroll = true;
      this.exportSettings.BorderStyle = BorderStyle.FixedSingle;
      this.exportSettings.Controls.Add((Control) this.grpImageProcessing);
      this.exportSettings.Controls.Add((Control) this.grpPageFormat);
      this.exportSettings.Controls.Add((Control) this.grpFileFormat);
      this.exportSettings.Controls.Add((Control) this.grpFileNaming);
      this.exportSettings.Controls.Add((Control) this.grpExportLocation);
      this.exportSettings.Location = new System.Drawing.Point(240, 18);
      this.exportSettings.Name = "exportSettings";
      this.exportSettings.Padding = new Padding(4);
      this.exportSettings.Size = new System.Drawing.Size(500, 416);
      this.exportSettings.TabIndex = 8;
      this.grpImageProcessing.Controls.Add((Control) this.grpCustomProcessing);
      this.grpImageProcessing.Controls.Add((Control) this.labelImageProcessingCustom);
      this.grpImageProcessing.Controls.Add((Control) this.cbImageProcessingSource);
      this.grpImageProcessing.Controls.Add((Control) this.labelImagProcessingSource);
      this.grpImageProcessing.Controls.Add((Control) this.chkAutoContrast);
      this.grpImageProcessing.Dock = DockStyle.Top;
      this.grpImageProcessing.Location = new System.Drawing.Point(4, 737);
      this.grpImageProcessing.Name = "grpImageProcessing";
      this.grpImageProcessing.Size = new System.Drawing.Size(473, 290);
      this.grpImageProcessing.TabIndex = 9;
      this.grpImageProcessing.Text = "Image Processing";
      this.grpCustomProcessing.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.grpCustomProcessing.Controls.Add((Control) this.labelGamma);
      this.grpCustomProcessing.Controls.Add((Control) this.tbGamma);
      this.grpCustomProcessing.Controls.Add((Control) this.tbSaturation);
      this.grpCustomProcessing.Controls.Add((Control) this.labelContrast);
      this.grpCustomProcessing.Controls.Add((Control) this.tbBrightness);
      this.grpCustomProcessing.Controls.Add((Control) this.labelSaturation);
      this.grpCustomProcessing.Controls.Add((Control) this.tbSharpening);
      this.grpCustomProcessing.Controls.Add((Control) this.tbContrast);
      this.grpCustomProcessing.Controls.Add((Control) this.labelSharpening);
      this.grpCustomProcessing.Controls.Add((Control) this.labelBrightness);
      this.grpCustomProcessing.Controls.Add((Control) this.btResetColors);
      this.grpCustomProcessing.Location = new System.Drawing.Point(110, 66);
      this.grpCustomProcessing.Name = "grpCustomProcessing";
      this.grpCustomProcessing.Size = new System.Drawing.Size(346, 183);
      this.grpCustomProcessing.TabIndex = 3;
      this.grpCustomProcessing.TabStop = false;
      this.labelGamma.AutoSize = true;
      this.labelGamma.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelGamma.Location = new System.Drawing.Point(6, 97);
      this.labelGamma.Name = "labelGamma";
      this.labelGamma.Size = new System.Drawing.Size(43, 12);
      this.labelGamma.TabIndex = 6;
      this.labelGamma.Text = "Gamma";
      this.tbGamma.Location = new System.Drawing.Point(95, 91);
      this.tbGamma.Minimum = -100;
      this.tbGamma.Name = "tbGamma";
      this.tbGamma.Size = new System.Drawing.Size(245, 18);
      this.tbGamma.TabIndex = 7;
      this.tbGamma.Text = "tbSaturation";
      this.tbGamma.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbGamma.TickFrequency = 16;
      this.tbGamma.TickStyle = TickStyle.BottomRight;
      this.tbGamma.ValueChanged += new EventHandler(this.AdjustmentSliderChanged);
      this.tbSaturation.Location = new System.Drawing.Point(95, 19);
      this.tbSaturation.Minimum = -100;
      this.tbSaturation.Name = "tbSaturation";
      this.tbSaturation.Size = new System.Drawing.Size(245, 18);
      this.tbSaturation.TabIndex = 1;
      this.tbSaturation.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbSaturation.TickFrequency = 16;
      this.tbSaturation.TickStyle = TickStyle.BottomRight;
      this.tbSaturation.ValueChanged += new EventHandler(this.AdjustmentSliderChanged);
      this.labelContrast.AutoSize = true;
      this.labelContrast.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelContrast.Location = new System.Drawing.Point(6, 73);
      this.labelContrast.Name = "labelContrast";
      this.labelContrast.Size = new System.Drawing.Size(49, 12);
      this.labelContrast.TabIndex = 4;
      this.labelContrast.Text = "Contrast";
      this.tbBrightness.Location = new System.Drawing.Point(95, 43);
      this.tbBrightness.Minimum = -100;
      this.tbBrightness.Name = "tbBrightness";
      this.tbBrightness.Size = new System.Drawing.Size(245, 18);
      this.tbBrightness.TabIndex = 3;
      this.tbBrightness.Text = "trackBarLite3";
      this.tbBrightness.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbBrightness.TickFrequency = 16;
      this.tbBrightness.TickStyle = TickStyle.BottomRight;
      this.tbBrightness.ValueChanged += new EventHandler(this.AdjustmentSliderChanged);
      this.labelSaturation.AutoSize = true;
      this.labelSaturation.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelSaturation.Location = new System.Drawing.Point(6, 25);
      this.labelSaturation.Name = "labelSaturation";
      this.labelSaturation.Size = new System.Drawing.Size(57, 12);
      this.labelSaturation.TabIndex = 0;
      this.labelSaturation.Text = "Saturation";
      this.tbSharpening.LargeChange = 1;
      this.tbSharpening.Location = new System.Drawing.Point(95, 117);
      this.tbSharpening.Maximum = 3;
      this.tbSharpening.Name = "tbSharpening";
      this.tbSharpening.Size = new System.Drawing.Size(245, 18);
      this.tbSharpening.TabIndex = 9;
      this.tbSharpening.Text = "tbSaturation";
      this.tbSharpening.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbSharpening.TickFrequency = 1;
      this.tbSharpening.TickStyle = TickStyle.BottomRight;
      this.tbContrast.Location = new System.Drawing.Point(95, 67);
      this.tbContrast.Minimum = -100;
      this.tbContrast.Name = "tbContrast";
      this.tbContrast.Size = new System.Drawing.Size(245, 18);
      this.tbContrast.TabIndex = 5;
      this.tbContrast.Text = "tbSaturation";
      this.tbContrast.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbContrast.TickFrequency = 16;
      this.tbContrast.TickStyle = TickStyle.BottomRight;
      this.tbContrast.ValueChanged += new EventHandler(this.AdjustmentSliderChanged);
      this.labelSharpening.AutoSize = true;
      this.labelSharpening.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelSharpening.Location = new System.Drawing.Point(6, 123);
      this.labelSharpening.Name = "labelSharpening";
      this.labelSharpening.Size = new System.Drawing.Size(61, 12);
      this.labelSharpening.TabIndex = 8;
      this.labelSharpening.Text = "Sharpening";
      this.labelBrightness.AutoSize = true;
      this.labelBrightness.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelBrightness.Location = new System.Drawing.Point(6, 49);
      this.labelBrightness.Name = "labelBrightness";
      this.labelBrightness.Size = new System.Drawing.Size(59, 12);
      this.labelBrightness.TabIndex = 2;
      this.labelBrightness.Text = "Brightness";
      this.btResetColors.Location = new System.Drawing.Point(257, 149);
      this.btResetColors.Name = "btResetColors";
      this.btResetColors.Size = new System.Drawing.Size(77, 24);
      this.btResetColors.TabIndex = 10;
      this.btResetColors.Text = "Reset";
      this.btResetColors.UseVisualStyleBackColor = true;
      this.btResetColors.Click += new EventHandler(this.btResetColors_Click);
      this.labelImageProcessingCustom.Location = new System.Drawing.Point(0, 66);
      this.labelImageProcessingCustom.Name = "labelImageProcessingCustom";
      this.labelImageProcessingCustom.Size = new System.Drawing.Size(104, 21);
      this.labelImageProcessingCustom.TabIndex = 2;
      this.labelImageProcessingCustom.Text = "Custom:";
      this.labelImageProcessingCustom.TextAlign = ContentAlignment.MiddleRight;
      this.cbImageProcessingSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbImageProcessingSource.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbImageProcessingSource.FormattingEnabled = true;
      this.cbImageProcessingSource.Items.AddRange(new object[2]
      {
        (object) "Custom Settings",
        (object) "Book Settings are applied"
      });
      this.cbImageProcessingSource.Location = new System.Drawing.Point(110, 40);
      this.cbImageProcessingSource.Name = "cbImageProcessingSource";
      this.cbImageProcessingSource.Size = new System.Drawing.Size(346, 21);
      this.cbImageProcessingSource.TabIndex = 1;
      this.labelImagProcessingSource.Location = new System.Drawing.Point(3, 39);
      this.labelImagProcessingSource.Name = "labelImagProcessingSource";
      this.labelImagProcessingSource.Size = new System.Drawing.Size(101, 21);
      this.labelImagProcessingSource.TabIndex = 0;
      this.labelImagProcessingSource.Text = "Source:";
      this.labelImagProcessingSource.TextAlign = ContentAlignment.MiddleRight;
      this.chkAutoContrast.AutoSize = true;
      this.chkAutoContrast.Location = new System.Drawing.Point(110, (int) byte.MaxValue);
      this.chkAutoContrast.Name = "chkAutoContrast";
      this.chkAutoContrast.Size = new System.Drawing.Size(184, 17);
      this.chkAutoContrast.TabIndex = 4;
      this.chkAutoContrast.Text = "Automatic Contrast Enhancement";
      this.chkAutoContrast.UseVisualStyleBackColor = true;
      this.grpPageFormat.Controls.Add((Control) this.chkKeepOriginalNames);
      this.grpPageFormat.Controls.Add((Control) this.labelDoublePages);
      this.grpPageFormat.Controls.Add((Control) this.cbDoublePages);
      this.grpPageFormat.Controls.Add((Control) this.chkIgnoreErrorPages);
      this.grpPageFormat.Controls.Add((Control) this.txHeight);
      this.grpPageFormat.Controls.Add((Control) this.txWidth);
      this.grpPageFormat.Controls.Add((Control) this.chkDontEnlarge);
      this.grpPageFormat.Controls.Add((Control) this.labelPageResize);
      this.grpPageFormat.Controls.Add((Control) this.cbPageResize);
      this.grpPageFormat.Controls.Add((Control) this.labelPageFormat);
      this.grpPageFormat.Controls.Add((Control) this.cbPageFormat);
      this.grpPageFormat.Controls.Add((Control) this.labelPageQuality);
      this.grpPageFormat.Controls.Add((Control) this.tbQuality);
      this.grpPageFormat.Controls.Add((Control) this.labelX);
      this.grpPageFormat.Dock = DockStyle.Top;
      this.grpPageFormat.Location = new System.Drawing.Point(4, 532);
      this.grpPageFormat.Name = "grpPageFormat";
      this.grpPageFormat.Size = new System.Drawing.Size(473, 205);
      this.grpPageFormat.TabIndex = 2;
      this.grpPageFormat.TabStop = false;
      this.grpPageFormat.Text = "Page Format";
      this.labelDoublePages.Location = new System.Drawing.Point(6, 96);
      this.labelDoublePages.Name = "labelDoublePages";
      this.labelDoublePages.Size = new System.Drawing.Size(98, 21);
      this.labelDoublePages.TabIndex = 9;
      this.labelDoublePages.Text = "Double Pages:";
      this.labelDoublePages.TextAlign = ContentAlignment.MiddleRight;
      this.cbDoublePages.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbDoublePages.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbDoublePages.FormattingEnabled = true;
      this.cbDoublePages.Items.AddRange(new object[4]
      {
        (object) "Keep",
        (object) "Split",
        (object) "Rotate 90°",
        (object) "Adapt Width"
      });
      this.cbDoublePages.Location = new System.Drawing.Point(110, 96);
      this.cbDoublePages.Name = "cbDoublePages";
      this.cbDoublePages.Size = new System.Drawing.Size(157, 21);
      this.cbDoublePages.TabIndex = 10;
      this.chkIgnoreErrorPages.AutoSize = true;
      this.chkIgnoreErrorPages.Location = new System.Drawing.Point(110, 155);
      this.chkIgnoreErrorPages.Name = "chkIgnoreErrorPages";
      this.chkIgnoreErrorPages.Size = new System.Drawing.Size(237, 17);
      this.chkIgnoreErrorPages.TabIndex = 12;
      this.chkIgnoreErrorPages.Text = "Ignore Pages with errors and continue export";
      this.chkIgnoreErrorPages.UseVisualStyleBackColor = true;
      this.txHeight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.txHeight.Location = new System.Drawing.Point(391, 71);
      this.txHeight.Maximum = new Decimal(new int[4]
      {
        8000,
        0,
        0,
        0
      });
      this.txHeight.Name = "txHeight";
      this.txHeight.Size = new System.Drawing.Size(65, 20);
      this.txHeight.TabIndex = 8;
      this.txHeight.TextAlign = HorizontalAlignment.Right;
      this.txHeight.Value = new Decimal(new int[4]
      {
        1000,
        0,
        0,
        0
      });
      this.txWidth.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.txWidth.Location = new System.Drawing.Point(291, 72);
      this.txWidth.Maximum = new Decimal(new int[4]
      {
        8000,
        0,
        0,
        0
      });
      this.txWidth.Name = "txWidth";
      this.txWidth.Size = new System.Drawing.Size(67, 20);
      this.txWidth.TabIndex = 6;
      this.txWidth.TextAlign = HorizontalAlignment.Right;
      this.txWidth.Value = new Decimal(new int[4]
      {
        1000,
        0,
        0,
        0
      });
      this.chkDontEnlarge.AutoSize = true;
      this.chkDontEnlarge.Location = new System.Drawing.Point(110, 132);
      this.chkDontEnlarge.Name = "chkDontEnlarge";
      this.chkDontEnlarge.Size = new System.Drawing.Size(89, 17);
      this.chkDontEnlarge.TabIndex = 11;
      this.chkDontEnlarge.Text = "Don't enlarge";
      this.chkDontEnlarge.UseVisualStyleBackColor = true;
      this.labelPageResize.Location = new System.Drawing.Point(6, 69);
      this.labelPageResize.Name = "labelPageResize";
      this.labelPageResize.Size = new System.Drawing.Size(98, 21);
      this.labelPageResize.TabIndex = 4;
      this.labelPageResize.Text = "Resize Pages:";
      this.labelPageResize.TextAlign = ContentAlignment.MiddleRight;
      this.cbPageResize.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbPageResize.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbPageResize.FormattingEnabled = true;
      this.cbPageResize.Items.AddRange(new object[4]
      {
        (object) "Preserve Original",
        (object) "Best fit Width & Height",
        (object) "Set Width",
        (object) "Set Height"
      });
      this.cbPageResize.Location = new System.Drawing.Point(110, 69);
      this.cbPageResize.Name = "cbPageResize";
      this.cbPageResize.Size = new System.Drawing.Size(157, 21);
      this.cbPageResize.TabIndex = 5;
      this.labelPageFormat.Location = new System.Drawing.Point(3, 42);
      this.labelPageFormat.Name = "labelPageFormat";
      this.labelPageFormat.Size = new System.Drawing.Size(101, 21);
      this.labelPageFormat.TabIndex = 0;
      this.labelPageFormat.Text = "Format:";
      this.labelPageFormat.TextAlign = ContentAlignment.MiddleRight;
      this.cbPageFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbPageFormat.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbPageFormat.FormattingEnabled = true;
      this.cbPageFormat.Items.AddRange(new object[8]
      {
        (object) "Preserve Original",
        (object) "JPEG",
        (object) "PNG",
        (object) "GIF",
        (object) "TIFF",
        (object) "BMP",
        (object) "DJVU",
        (object) "WEBP"
      });
      this.cbPageFormat.Location = new System.Drawing.Point(110, 42);
      this.cbPageFormat.Name = "cbPageFormat";
      this.cbPageFormat.Size = new System.Drawing.Size(157, 21);
      this.cbPageFormat.TabIndex = 1;
      this.labelPageQuality.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.labelPageQuality.Location = new System.Drawing.Point(273, 41);
      this.labelPageQuality.Name = "labelPageQuality";
      this.labelPageQuality.Size = new System.Drawing.Size(77, 21);
      this.labelPageQuality.TabIndex = 2;
      this.labelPageQuality.Text = "Quality:";
      this.labelPageQuality.TextAlign = ContentAlignment.MiddleRight;
      this.tbQuality.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.tbQuality.Location = new System.Drawing.Point(349, 41);
      this.tbQuality.Name = "tbQuality";
      this.tbQuality.Size = new System.Drawing.Size(107, 21);
      this.tbQuality.TabIndex = 3;
      this.tbQuality.ThumbSize = new System.Drawing.Size(8, 14);
      this.tbQuality.ValueChanged += new EventHandler(this.tbQuality_ValueChanged);
      this.labelX.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.labelX.Location = new System.Drawing.Point(364, 71);
      this.labelX.Name = "labelX";
      this.labelX.Size = new System.Drawing.Size(21, 21);
      this.labelX.TabIndex = 7;
      this.labelX.Text = "x";
      this.labelX.TextAlign = ContentAlignment.MiddleCenter;
      this.grpFileFormat.Controls.Add((Control) this.txIncludePages);
      this.grpFileFormat.Controls.Add((Control) this.labelIncludePages);
      this.grpFileFormat.Controls.Add((Control) this.btRemovePageFilter);
      this.grpFileFormat.Controls.Add((Control) this.cbCompression);
      this.grpFileFormat.Controls.Add((Control) this.labelCompression);
      this.grpFileFormat.Controls.Add((Control) this.chkEmbedComicInfo);
      this.grpFileFormat.Controls.Add((Control) this.cbComicFormat);
      this.grpFileFormat.Controls.Add((Control) this.labelComicFormat);
      this.grpFileFormat.Controls.Add((Control) this.txRemovedPages);
      this.grpFileFormat.Controls.Add((Control) this.labelRemovePageFilter);
      this.grpFileFormat.Dock = DockStyle.Top;
      this.grpFileFormat.Location = new System.Drawing.Point(4, 356);
      this.grpFileFormat.Name = "grpFileFormat";
      this.grpFileFormat.Size = new System.Drawing.Size(473, 176);
      this.grpFileFormat.TabIndex = 1;
      this.grpFileFormat.TabStop = false;
      this.grpFileFormat.Text = "File Format";
      this.txIncludePages.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txIncludePages.Location = new System.Drawing.Point(116, 112);
      this.txIncludePages.Name = "txIncludePages";
      this.txIncludePages.Size = new System.Drawing.Size(340, 20);
      this.txIncludePages.TabIndex = 6;
      this.labelIncludePages.Location = new System.Drawing.Point(15, 111);
      this.labelIncludePages.Name = "labelIncludePages";
      this.labelIncludePages.Size = new System.Drawing.Size(98, 21);
      this.labelIncludePages.TabIndex = 5;
      this.labelIncludePages.Text = "Include Pages:";
      this.labelIncludePages.TextAlign = ContentAlignment.MiddleRight;
      this.btRemovePageFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btRemovePageFilter.Image = (Image) Resources.SmallArrowDown;
      this.btRemovePageFilter.Location = new System.Drawing.Point(434, 139);
      this.btRemovePageFilter.Name = "btRemovePageFilter";
      this.btRemovePageFilter.Size = new System.Drawing.Size(22, 23);
      this.btRemovePageFilter.TabIndex = 9;
      this.btRemovePageFilter.UseVisualStyleBackColor = true;
      this.btRemovePageFilter.Click += new EventHandler(this.btRemovePageFilter_Click);
      this.cbCompression.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbCompression.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbCompression.FormattingEnabled = true;
      this.cbCompression.Items.AddRange(new object[3]
      {
        (object) "None",
        (object) "Medium",
        (object) "Strong"
      });
      this.cbCompression.Location = new System.Drawing.Point(116, 70);
      this.cbCompression.Name = "cbCompression";
      this.cbCompression.Size = new System.Drawing.Size(151, 21);
      this.cbCompression.TabIndex = 3;
      this.labelCompression.Location = new System.Drawing.Point(12, 70);
      this.labelCompression.Name = "labelCompression";
      this.labelCompression.Size = new System.Drawing.Size(98, 21);
      this.labelCompression.TabIndex = 2;
      this.labelCompression.Text = "Compression:";
      this.labelCompression.TextAlign = ContentAlignment.MiddleRight;
      this.chkEmbedComicInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkEmbedComicInfo.AutoSize = true;
      this.chkEmbedComicInfo.Location = new System.Drawing.Point(295, 73);
      this.chkEmbedComicInfo.Name = "chkEmbedComicInfo";
      this.chkEmbedComicInfo.Size = new System.Drawing.Size(108, 17);
      this.chkEmbedComicInfo.TabIndex = 4;
      this.chkEmbedComicInfo.Text = "Embed Book Info";
      this.chkEmbedComicInfo.UseVisualStyleBackColor = true;
      this.cbComicFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbComicFormat.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbComicFormat.FormattingEnabled = true;
      this.cbComicFormat.Items.AddRange(new object[1]
      {
        (object) "Same as Original"
      });
      this.cbComicFormat.Location = new System.Drawing.Point(116, 43);
      this.cbComicFormat.Name = "cbComicFormat";
      this.cbComicFormat.Size = new System.Drawing.Size(340, 21);
      this.cbComicFormat.TabIndex = 1;
      this.labelComicFormat.Location = new System.Drawing.Point(9, 43);
      this.labelComicFormat.Name = "labelComicFormat";
      this.labelComicFormat.Size = new System.Drawing.Size(101, 21);
      this.labelComicFormat.TabIndex = 0;
      this.labelComicFormat.Text = "Format:";
      this.labelComicFormat.TextAlign = ContentAlignment.MiddleRight;
      this.txRemovedPages.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txRemovedPages.AutoEllipsis = true;
      this.txRemovedPages.BorderStyle = BorderStyle.Fixed3D;
      this.txRemovedPages.Location = new System.Drawing.Point(116, 141);
      this.txRemovedPages.Name = "txRemovedPages";
      this.txRemovedPages.Size = new System.Drawing.Size(312, 21);
      this.txRemovedPages.TabIndex = 8;
      this.txRemovedPages.Text = "Lorem Ipsum";
      this.txRemovedPages.TextAlign = ContentAlignment.MiddleLeft;
      this.txRemovedPages.UseMnemonic = false;
      this.labelRemovePageFilter.Location = new System.Drawing.Point(9, 141);
      this.labelRemovePageFilter.Name = "labelRemovePageFilter";
      this.labelRemovePageFilter.Size = new System.Drawing.Size(101, 21);
      this.labelRemovePageFilter.TabIndex = 7;
      this.labelRemovePageFilter.Text = "Remove Pages:";
      this.labelRemovePageFilter.TextAlign = ContentAlignment.MiddleRight;
      this.grpFileNaming.Controls.Add((Control) this.txCustomStartIndex);
      this.grpFileNaming.Controls.Add((Control) this.labelCustomStartIndex);
      this.grpFileNaming.Controls.Add((Control) this.txCustomName);
      this.grpFileNaming.Controls.Add((Control) this.labelCustomNaming);
      this.grpFileNaming.Controls.Add((Control) this.cbNamingTemplate);
      this.grpFileNaming.Controls.Add((Control) this.labelNamingTemplate);
      this.grpFileNaming.Dock = DockStyle.Top;
      this.grpFileNaming.Location = new System.Drawing.Point(4, 226);
      this.grpFileNaming.Name = "grpFileNaming";
      this.grpFileNaming.Size = new System.Drawing.Size(473, 130);
      this.grpFileNaming.TabIndex = 9;
      this.grpFileNaming.Text = "File Naming";
      this.txCustomStartIndex.Location = new System.Drawing.Point(116, 98);
      this.txCustomStartIndex.Maximum = new Decimal(new int[4]
      {
        100000,
        0,
        0,
        0
      });
      this.txCustomStartIndex.Name = "txCustomStartIndex";
      this.txCustomStartIndex.Size = new System.Drawing.Size(67, 20);
      this.txCustomStartIndex.TabIndex = 5;
      this.txCustomStartIndex.TextAlign = HorizontalAlignment.Right;
      this.txCustomStartIndex.Value = new Decimal(new int[4]
      {
        1000,
        0,
        0,
        0
      });
      this.labelCustomStartIndex.Location = new System.Drawing.Point(6, 98);
      this.labelCustomStartIndex.Name = "labelCustomStartIndex";
      this.labelCustomStartIndex.Size = new System.Drawing.Size(104, 21);
      this.labelCustomStartIndex.TabIndex = 4;
      this.labelCustomStartIndex.Text = "Start:";
      this.labelCustomStartIndex.TextAlign = ContentAlignment.MiddleRight;
      this.txCustomName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txCustomName.Location = new System.Drawing.Point(116, 72);
      this.txCustomName.Name = "txCustomName";
      this.txCustomName.Size = new System.Drawing.Size(340, 20);
      this.txCustomName.TabIndex = 3;
      this.labelCustomNaming.Location = new System.Drawing.Point(6, 71);
      this.labelCustomNaming.Name = "labelCustomNaming";
      this.labelCustomNaming.Size = new System.Drawing.Size(104, 21);
      this.labelCustomNaming.TabIndex = 2;
      this.labelCustomNaming.Text = "Custom:";
      this.labelCustomNaming.TextAlign = ContentAlignment.MiddleRight;
      this.cbNamingTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbNamingTemplate.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbNamingTemplate.FormattingEnabled = true;
      this.cbNamingTemplate.Items.AddRange(new object[3]
      {
        (object) "Filename",
        (object) "Book Caption",
        (object) "Custom"
      });
      this.cbNamingTemplate.Location = new System.Drawing.Point(116, 45);
      this.cbNamingTemplate.Name = "cbNamingTemplate";
      this.cbNamingTemplate.Size = new System.Drawing.Size(340, 21);
      this.cbNamingTemplate.TabIndex = 1;
      this.cbNamingTemplate.SelectedIndexChanged += new EventHandler(this.cbNamingTemplate_SelectedIndexChanged);
      this.labelNamingTemplate.Location = new System.Drawing.Point(6, 45);
      this.labelNamingTemplate.Name = "labelNamingTemplate";
      this.labelNamingTemplate.Size = new System.Drawing.Size(104, 21);
      this.labelNamingTemplate.TabIndex = 0;
      this.labelNamingTemplate.Text = "Template:";
      this.labelNamingTemplate.TextAlign = ContentAlignment.MiddleRight;
      this.grpExportLocation.Controls.Add((Control) this.chkCombine);
      this.grpExportLocation.Controls.Add((Control) this.chkOverwrite);
      this.grpExportLocation.Controls.Add((Control) this.chkAddNewToLibrary);
      this.grpExportLocation.Controls.Add((Control) this.chkDeleteOriginal);
      this.grpExportLocation.Controls.Add((Control) this.btChooseFolder);
      this.grpExportLocation.Controls.Add((Control) this.txFolder);
      this.grpExportLocation.Controls.Add((Control) this.labelFolder);
      this.grpExportLocation.Controls.Add((Control) this.cbExport);
      this.grpExportLocation.Controls.Add((Control) this.labelExportTo);
      this.grpExportLocation.Dock = DockStyle.Top;
      this.grpExportLocation.Location = new System.Drawing.Point(4, 4);
      this.grpExportLocation.Name = "grpExportLocation";
      this.grpExportLocation.Size = new System.Drawing.Size(473, 222);
      this.grpExportLocation.TabIndex = 0;
      this.grpExportLocation.TabStop = false;
      this.grpExportLocation.Text = "Export Location";
      this.chkCombine.AutoSize = true;
      this.chkCombine.Location = new System.Drawing.Point(117, 113);
      this.chkCombine.Name = "chkCombine";
      this.chkCombine.Size = new System.Drawing.Size(147, 17);
      this.chkCombine.TabIndex = 5;
      this.chkCombine.Text = "Combine all selected Files";
      this.chkCombine.UseVisualStyleBackColor = true;
      this.chkOverwrite.AutoSize = true;
      this.chkOverwrite.Location = new System.Drawing.Point(117, 136);
      this.chkOverwrite.Name = "chkOverwrite";
      this.chkOverwrite.Size = new System.Drawing.Size(133, 17);
      this.chkOverwrite.TabIndex = 6;
      this.chkOverwrite.Text = "Overwrite existing Files";
      this.chkOverwrite.UseVisualStyleBackColor = true;
      this.chkAddNewToLibrary.AutoSize = true;
      this.chkAddNewToLibrary.Location = new System.Drawing.Point(117, 182);
      this.chkAddNewToLibrary.Name = "chkAddNewToLibrary";
      this.chkAddNewToLibrary.Size = new System.Drawing.Size(188, 17);
      this.chkAddNewToLibrary.TabIndex = 8;
      this.chkAddNewToLibrary.Text = "Add newly created Book to Library";
      this.chkAddNewToLibrary.UseVisualStyleBackColor = true;
      this.chkDeleteOriginal.AutoSize = true;
      this.chkDeleteOriginal.Location = new System.Drawing.Point(117, 159);
      this.chkDeleteOriginal.Name = "chkDeleteOriginal";
      this.chkDeleteOriginal.Size = new System.Drawing.Size(231, 17);
      this.chkDeleteOriginal.TabIndex = 7;
      this.chkDeleteOriginal.Text = "Delete original Book after successful Export";
      this.chkDeleteOriginal.UseVisualStyleBackColor = true;
      this.btChooseFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btChooseFolder.Location = new System.Drawing.Point(381, 70);
      this.btChooseFolder.Name = "btChooseFolder";
      this.btChooseFolder.Size = new System.Drawing.Size(75, 23);
      this.btChooseFolder.TabIndex = 4;
      this.btChooseFolder.Text = "Choose...";
      this.btChooseFolder.UseVisualStyleBackColor = true;
      this.btChooseFolder.Click += new EventHandler(this.btChooseFolder_Click);
      this.txFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txFolder.AutoEllipsis = true;
      this.txFolder.BorderStyle = BorderStyle.Fixed3D;
      this.txFolder.Location = new System.Drawing.Point(116, 72);
      this.txFolder.Name = "txFolder";
      this.txFolder.Size = new System.Drawing.Size(259, 21);
      this.txFolder.TabIndex = 3;
      this.txFolder.Text = "Lorem Ipsum";
      this.txFolder.TextAlign = ContentAlignment.MiddleLeft;
      this.txFolder.UseMnemonic = false;
      this.labelFolder.Location = new System.Drawing.Point(9, 73);
      this.labelFolder.Name = "labelFolder";
      this.labelFolder.Size = new System.Drawing.Size(101, 21);
      this.labelFolder.TabIndex = 2;
      this.labelFolder.Text = "Folder:";
      this.labelFolder.TextAlign = ContentAlignment.MiddleRight;
      this.cbExport.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbExport.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbExport.FormattingEnabled = true;
      this.cbExport.Items.AddRange(new object[4]
      {
        (object) "Select a Folder",
        (object) "Same Folder as the original Book",
        (object) "Same Folder and replace in Library",
        (object) "Ask before Export"
      });
      this.cbExport.Location = new System.Drawing.Point(116, 43);
      this.cbExport.Name = "cbExport";
      this.cbExport.Size = new System.Drawing.Size(340, 21);
      this.cbExport.TabIndex = 1;
      this.labelExportTo.Location = new System.Drawing.Point(6, 43);
      this.labelExportTo.Name = "labelExportTo";
      this.labelExportTo.Size = new System.Drawing.Size(104, 21);
      this.labelExportTo.TabIndex = 0;
      this.labelExportTo.Text = "Export To:";
      this.labelExportTo.TextAlign = ContentAlignment.MiddleRight;
      this.chkKeepOriginalNames.AutoSize = true;
      this.chkKeepOriginalNames.Location = new System.Drawing.Point(110, 178);
      this.chkKeepOriginalNames.Name = "chkKeepOriginalNames";
      this.chkKeepOriginalNames.Size = new System.Drawing.Size(148, 17);
      this.chkKeepOriginalNames.TabIndex = 13;
      this.chkKeepOriginalNames.Text = "Keep original page names";
      this.chkKeepOriginalNames.UseVisualStyleBackColor = true;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(752, 472);
      this.Controls.Add((Control) this.exportSettings);
      this.Controls.Add((Control) this.btRemovePreset);
      this.Controls.Add((Control) this.btSavePreset);
      this.Controls.Add((Control) this.tvPresets);
      this.Controls.Add((Control) this.btOK);
      this.Controls.Add((Control) this.btCancel);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ExportComicsDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Export Books";
      this.exportSettings.ResumeLayout(false);
      this.grpImageProcessing.ResumeLayout(false);
      this.grpImageProcessing.PerformLayout();
      this.grpCustomProcessing.ResumeLayout(false);
      this.grpCustomProcessing.PerformLayout();
      this.grpPageFormat.ResumeLayout(false);
      this.grpPageFormat.PerformLayout();
      this.txHeight.EndInit();
      this.txWidth.EndInit();
      this.grpFileFormat.ResumeLayout(false);
      this.grpFileFormat.PerformLayout();
      this.grpFileNaming.ResumeLayout(false);
      this.grpFileNaming.PerformLayout();
      this.txCustomStartIndex.EndInit();
      this.grpExportLocation.ResumeLayout(false);
      this.grpExportLocation.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}

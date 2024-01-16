// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ComicDisplaySettingsDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Viewer.Config;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ComicDisplaySettingsDialog : Form
  {
    private IContainer components;
    private Button btBrowseTexture;
    private ComboBox cbBackgroundTexture;
    private Label labelBackgroundTexture;
    private ComboBox cbBackgroundType;
    private Label labelBackgroundType;
    private SimpleColorPicker cpBackgroundColor;
    private Label labelBackgroundColor;
    private CheckBox chkPageMargin;
    private CheckBox chkRealisticPages;
    private ComboBox cbPageTransition;
    private Button btCancel;
    private Button btOK;
    private Label labelPaging;
    private GroupBox grpGeneral;
    private GroupBox grpEffects;
    private GroupBox grpBackground;
    private Button btApply;
    private ComboBox cbPaperTexture;
    private Label labelPaper;
    private Button btBrowsePaper;
    private TrackBarLite tbPaperStrength;
    private Label labelPaperStrength;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel panel1;
    private ComboBox cbPaperLayout;
    private ComboBox cbTextureLayout;
    private TrackBarLite tbMargin;
    private ToolTip toolTip;

    public ComicDisplaySettingsDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.RestorePosition();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      ComboBoxSkinner comboBoxSkinner1 = new ComboBoxSkinner(this.cbPaperTexture);
      ComboBoxSkinner comboBoxSkinner2 = new ComboBoxSkinner(this.cbBackgroundTexture);
      this.labelBackgroundTexture.Location = this.labelBackgroundColor.Location;
      this.cbBackgroundTexture.Location = this.cpBackgroundColor.Location;
      this.btBrowseTexture.Top = this.cbBackgroundTexture.Top;
      this.cpBackgroundColor.FillKnownColors(false);
      this.cbBackgroundTexture.Items.Add((object) new ComicDisplaySettingsDialog.TextureFileItem());
      foreach (string backgroundTexture in Program.LoadDefaultBackgroundTextures())
        this.cbBackgroundTexture.Items.Add((object) new ComicDisplaySettingsDialog.TextureFileItem(backgroundTexture, false));
      this.cbPaperTexture.Items.Add((object) new ComicDisplaySettingsDialog.TextureFileItem()
      {
        Default = TR.Default["Default", "Default"]
      });
      foreach (string defaultPaperTexture in Program.LoadDefaultPaperTextures())
        this.cbPaperTexture.Items.Add((object) new ComicDisplaySettingsDialog.TextureFileItem(defaultPaperTexture, false));
      LocalizeUtility.Localize(TR.Load(this.Name), this.cbPageTransition);
      LocalizeUtility.Localize(TR.Load(this.Name), this.cbBackgroundType);
      LocalizeUtility.Localize(TR.Load(this.Name), this.cbPaperLayout);
      LocalizeUtility.Localize(TR.Load(this.Name), this.cbTextureLayout);
    }

    protected override void OnClosed(EventArgs e)
    {
      foreach (ComicDisplaySettingsDialog.TextureFileItem textureFileItem in this.cbPaperTexture.Items)
        textureFileItem.Sample.SafeDispose();
      foreach (ComicDisplaySettingsDialog.TextureFileItem textureFileItem in this.cbBackgroundTexture.Items)
        textureFileItem.Sample.SafeDispose();
      base.OnClosed(e);
    }

    private DisplayWorkspace Workspace { get; set; }

    private Action<DisplayWorkspace> ApplyAction { get; set; }

    private void btBroweTexture_Click(object sender, EventArgs e)
    {
      string texture = this.GetTexture();
      if (string.IsNullOrEmpty(texture))
        return;
      this.SelectTextureFile(this.cbBackgroundTexture, texture);
    }

    private void btBrowsePaper_Click(object sender, EventArgs e)
    {
      string texture = this.GetTexture();
      if (string.IsNullOrEmpty(texture))
        return;
      this.SelectTextureFile(this.cbPaperTexture, texture);
    }

    private void cbPaperTexture_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.labelPaperStrength.Visible = this.cbPaperLayout.Visible = this.tbPaperStrength.Visible = this.cbPaperTexture.SelectedIndex != 0;
      ComicDisplaySettingsDialog.TextureFileItem selectedItem = (ComicDisplaySettingsDialog.TextureFileItem) this.cbPaperTexture.SelectedItem;
      if (!selectedItem.IsCustom)
        this.cbPaperLayout.SelectedIndex = (int) selectedItem.Layout;
      this.cbPaperLayout.Visible = selectedItem.IsCustom;
    }

    private void cbBackgroundTexture_SelectedIndexChanged(object sender, EventArgs e)
    {
      ComicDisplaySettingsDialog.TextureFileItem selectedItem = (ComicDisplaySettingsDialog.TextureFileItem) this.cbBackgroundTexture.SelectedItem;
      if (!selectedItem.IsCustom)
        this.cbTextureLayout.SelectedIndex = (int) selectedItem.Layout;
      this.cbTextureLayout.Visible = selectedItem.IsCustom;
    }

    private void cbBackgroundType_SelectedIndexChanged(object sender, EventArgs e)
    {
      int selectedIndex = this.cbBackgroundType.SelectedIndex;
      this.labelBackgroundColor.Visible = this.cpBackgroundColor.Visible = selectedIndex == 1;
      this.labelBackgroundTexture.Visible = this.cbBackgroundTexture.Visible = this.btBrowseTexture.Visible = selectedIndex == 2;
      ComicDisplaySettingsDialog.TextureFileItem selectedItem = this.cbBackgroundTexture.SelectedItem as ComicDisplaySettingsDialog.TextureFileItem;
      this.cbTextureLayout.Visible = selectedIndex == 2 && (selectedItem == null || selectedItem.IsCustom);
    }

    private void btApply_Click(object sender, EventArgs e)
    {
      if (this.ApplyAction == null)
        return;
      this.Apply(this.Workspace);
      this.ApplyAction(this.Workspace);
    }

    private void PercentTrackbarValueChanged(object sender, EventArgs e)
    {
      TrackBarLite trackBarLite = sender as TrackBarLite;
      this.toolTip.SetToolTip((Control) trackBarLite, string.Format("{0}%", (object) trackBarLite.Value));
    }

    private void Apply(DisplayWorkspace ws)
    {
      ws.PageTransitionEffect = (PageTransitionEffect) this.cbPageTransition.SelectedIndex;
      ws.DrawRealisticPages = this.chkRealisticPages.Checked;
      ws.PageMargin = this.chkPageMargin.Checked;
      ws.PageMarginPercentWidth = (float) this.tbMargin.Value / 100f;
      ws.PageImageBackgroundMode = (ImageBackgroundMode) this.cbBackgroundType.SelectedIndex;
      ws.BackgroundColor = this.cpBackgroundColor.SelectedColorName;
      ws.BackgroundTexture = ((ComboBoxSkinner.ComboBoxItem<string>) this.cbBackgroundTexture.SelectedItem).Item;
      ws.PaperTexture = ((ComboBoxSkinner.ComboBoxItem<string>) this.cbPaperTexture.SelectedItem).Item;
      ws.PaperTextureStrength = (float) this.tbPaperStrength.Value / 100f;
      ws.PaperTextureLayout = (ImageLayout) this.cbPaperLayout.SelectedIndex;
      ws.BackgroundImageLayout = (ImageLayout) this.cbTextureLayout.SelectedIndex;
    }

    private void Update(DisplayWorkspace ws)
    {
      this.Workspace = ws;
      this.cbPageTransition.SelectedIndex = (int) ws.PageTransitionEffect;
      this.chkRealisticPages.Checked = ws.DrawRealisticPages;
      this.chkPageMargin.Checked = ws.PageMargin;
      this.tbMargin.Value = (int) ((double) ws.PageMarginPercentWidth * 100.0);
      this.cpBackgroundColor.SelectedColorName = ws.BackgroundColor;
      this.cbBackgroundType.SelectedIndex = (int) ws.PageImageBackgroundMode;
      this.SelectTextureFile(this.cbBackgroundTexture, ws.BackgroundTexture);
      this.SelectTextureFile(this.cbPaperTexture, ws.PaperTexture);
      this.tbPaperStrength.Value = (int) ((double) ws.PaperTextureStrength * 100.0);
      this.cbPaperLayout.SelectedIndex = (int) ws.PaperTextureLayout;
      this.cbTextureLayout.SelectedIndex = (int) ws.BackgroundImageLayout;
    }

    private void SelectTextureFile(ComboBox cb, string texture)
    {
      int index = cb.Items.OfType<ComicDisplaySettingsDialog.TextureFileItem>().FindIndex<ComicDisplaySettingsDialog.TextureFileItem>((Predicate<ComicDisplaySettingsDialog.TextureFileItem>) (i => string.Equals(i.Item, texture, StringComparison.OrdinalIgnoreCase)));
      if (index != -1)
      {
        cb.SelectedIndex = index;
      }
      else
      {
        ComicDisplaySettingsDialog.TextureFileItem textureFileItem1 = cb.Items[cb.Items.Count - 1] as ComicDisplaySettingsDialog.TextureFileItem;
        if (textureFileItem1.IsCustom)
        {
          textureFileItem1.Sample.SafeDispose();
          textureFileItem1.Sample = (Bitmap) null;
          cb.Items.Remove((object) textureFileItem1);
        }
        ComicDisplaySettingsDialog.TextureFileItem textureFileItem2 = new ComicDisplaySettingsDialog.TextureFileItem();
        textureFileItem2.Item = texture;
        textureFileItem2.IsCustom = true;
        ComicDisplaySettingsDialog.TextureFileItem textureFileItem3 = textureFileItem2;
        cb.Items.Add((object) textureFileItem3);
        cb.SelectedItem = (object) textureFileItem3;
      }
    }

    private string GetTexture()
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Filter = TR.Load("FileFilter")["PageImageSave", "JPEG Image|*.jpg|Windows Bitmap Image|*.bmp|PNG Image|*.png|GIF Image|*.gif|TIFF Image|*.tif"];
        openFileDialog.CheckFileExists = true;
        if (openFileDialog.ShowDialog((IWin32Window) this) == DialogResult.OK)
          return openFileDialog.FileName;
      }
      return (string) null;
    }

    public static bool Show(
      IWin32Window parent,
      bool enableHardware,
      DisplayWorkspace ws,
      Action<DisplayWorkspace> apply)
    {
      using (ComicDisplaySettingsDialog displaySettingsDialog = new ComicDisplaySettingsDialog())
      {
        displaySettingsDialog.Update(ws);
        displaySettingsDialog.ApplyAction = apply;
        displaySettingsDialog.grpEffects.Visible = enableHardware;
        if (displaySettingsDialog.ShowDialog(parent) != DialogResult.OK)
          return false;
        displaySettingsDialog.Apply(ws);
        if (apply != null)
          apply(ws);
        return true;
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
      this.btBrowseTexture = new Button();
      this.cbBackgroundTexture = new ComboBox();
      this.labelBackgroundTexture = new Label();
      this.cbBackgroundType = new ComboBox();
      this.labelBackgroundType = new Label();
      this.cpBackgroundColor = new SimpleColorPicker();
      this.labelBackgroundColor = new Label();
      this.chkPageMargin = new CheckBox();
      this.chkRealisticPages = new CheckBox();
      this.cbPageTransition = new ComboBox();
      this.btCancel = new Button();
      this.btOK = new Button();
      this.labelPaging = new Label();
      this.grpGeneral = new GroupBox();
      this.tbMargin = new TrackBarLite();
      this.grpEffects = new GroupBox();
      this.cbPaperLayout = new ComboBox();
      this.labelPaperStrength = new Label();
      this.tbPaperStrength = new TrackBarLite();
      this.btBrowsePaper = new Button();
      this.cbPaperTexture = new ComboBox();
      this.labelPaper = new Label();
      this.grpBackground = new GroupBox();
      this.cbTextureLayout = new ComboBox();
      this.btApply = new Button();
      this.flowLayoutPanel1 = new FlowLayoutPanel();
      this.panel1 = new Panel();
      this.toolTip = new ToolTip(this.components);
      this.grpGeneral.SuspendLayout();
      this.grpEffects.SuspendLayout();
      this.grpBackground.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.btBrowseTexture.Location = new System.Drawing.Point(357, 51);
      this.btBrowseTexture.Name = "btBrowseTexture";
      this.btBrowseTexture.Size = new System.Drawing.Size(32, 22);
      this.btBrowseTexture.TabIndex = 6;
      this.btBrowseTexture.Text = "...";
      this.btBrowseTexture.UseVisualStyleBackColor = true;
      this.btBrowseTexture.Click += new EventHandler(this.btBroweTexture_Click);
      this.cbBackgroundTexture.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbBackgroundTexture.FormattingEnabled = true;
      this.cbBackgroundTexture.Location = new System.Drawing.Point(103, 52);
      this.cbBackgroundTexture.Name = "cbBackgroundTexture";
      this.cbBackgroundTexture.Size = new System.Drawing.Size(248, 21);
      this.cbBackgroundTexture.TabIndex = 4;
      this.cbBackgroundTexture.SelectedIndexChanged += new EventHandler(this.cbBackgroundTexture_SelectedIndexChanged);
      this.labelBackgroundTexture.Location = new System.Drawing.Point(15, 55);
      this.labelBackgroundTexture.Name = "labelBackgroundTexture";
      this.labelBackgroundTexture.Size = new System.Drawing.Size(67, 13);
      this.labelBackgroundTexture.TabIndex = 3;
      this.labelBackgroundTexture.Text = "Texture:";
      this.labelBackgroundTexture.TextAlign = ContentAlignment.MiddleLeft;
      this.cbBackgroundType.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbBackgroundType.FormattingEnabled = true;
      this.cbBackgroundType.Items.AddRange(new object[3]
      {
        (object) "Adjust Color to current Page",
        (object) "Solid Color",
        (object) "Texture"
      });
      this.cbBackgroundType.Location = new System.Drawing.Point(103, 27);
      this.cbBackgroundType.Name = "cbBackgroundType";
      this.cbBackgroundType.Size = new System.Drawing.Size(286, 21);
      this.cbBackgroundType.TabIndex = 1;
      this.cbBackgroundType.SelectedIndexChanged += new EventHandler(this.cbBackgroundType_SelectedIndexChanged);
      this.labelBackgroundType.Location = new System.Drawing.Point(15, 30);
      this.labelBackgroundType.Name = "labelBackgroundType";
      this.labelBackgroundType.Size = new System.Drawing.Size(67, 13);
      this.labelBackgroundType.TabIndex = 0;
      this.labelBackgroundType.Text = "Type:";
      this.labelBackgroundType.TextAlign = ContentAlignment.MiddleLeft;
      this.cpBackgroundColor.DrawMode = DrawMode.OwnerDrawFixed;
      this.cpBackgroundColor.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cpBackgroundColor.FormattingEnabled = true;
      this.cpBackgroundColor.Location = new System.Drawing.Point(103, 52);
      this.cpBackgroundColor.Name = "cpBackgroundColor";
      this.cpBackgroundColor.SelectedColor = Color.Empty;
      this.cpBackgroundColor.SelectedColorName = "0";
      this.cpBackgroundColor.Size = new System.Drawing.Size(286, 21);
      this.cpBackgroundColor.TabIndex = 5;
      this.labelBackgroundColor.Location = new System.Drawing.Point(15, 56);
      this.labelBackgroundColor.Name = "labelBackgroundColor";
      this.labelBackgroundColor.Size = new System.Drawing.Size(67, 13);
      this.labelBackgroundColor.TabIndex = 2;
      this.labelBackgroundColor.Text = "Color:";
      this.labelBackgroundColor.TextAlign = ContentAlignment.MiddleLeft;
      this.chkPageMargin.AutoSize = true;
      this.chkPageMargin.Location = new System.Drawing.Point(15, 51);
      this.chkPageMargin.Name = "chkPageMargin";
      this.chkPageMargin.Size = new System.Drawing.Size(181, 17);
      this.chkPageMargin.TabIndex = 1;
      this.chkPageMargin.Text = "Leave margins around the pages";
      this.chkPageMargin.UseVisualStyleBackColor = true;
      this.chkRealisticPages.AutoSize = true;
      this.chkRealisticPages.Location = new System.Drawing.Point(15, 28);
      this.chkRealisticPages.Name = "chkRealisticPages";
      this.chkRealisticPages.Size = new System.Drawing.Size(131, 17);
      this.chkRealisticPages.TabIndex = 0;
      this.chkRealisticPages.Text = "Realistic Book Display";
      this.chkRealisticPages.UseVisualStyleBackColor = true;
      this.cbPageTransition.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbPageTransition.FormattingEnabled = true;
      this.cbPageTransition.Items.AddRange(new object[5]
      {
        (object) "No Page Transition Effect",
        (object) "New Page fades in",
        (object) "New Page scrolls in horizontally",
        (object) "New Page scrolls in vertically",
        (object) "Page Turn Effect"
      });
      this.cbPageTransition.Location = new System.Drawing.Point(103, 26);
      this.cbPageTransition.Name = "cbPageTransition";
      this.cbPageTransition.Size = new System.Drawing.Size(286, 21);
      this.cbPageTransition.TabIndex = 1;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(89, 3);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 4;
      this.btCancel.Text = "&Cancel";
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(3, 3);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 3;
      this.btOK.Text = "&OK";
      this.labelPaging.AutoSize = true;
      this.labelPaging.Location = new System.Drawing.Point(15, 29);
      this.labelPaging.Name = "labelPaging";
      this.labelPaging.Size = new System.Drawing.Size(84, 13);
      this.labelPaging.TabIndex = 0;
      this.labelPaging.Text = "Page Transition:";
      this.labelPaging.TextAlign = ContentAlignment.MiddleLeft;
      this.grpGeneral.Controls.Add((Control) this.tbMargin);
      this.grpGeneral.Controls.Add((Control) this.chkRealisticPages);
      this.grpGeneral.Controls.Add((Control) this.chkPageMargin);
      this.grpGeneral.Location = new System.Drawing.Point(3, 3);
      this.grpGeneral.Name = "grpGeneral";
      this.grpGeneral.Size = new System.Drawing.Size(396, 84);
      this.grpGeneral.TabIndex = 0;
      this.grpGeneral.TabStop = false;
      this.grpGeneral.Text = "General";
      this.tbMargin.Location = new System.Drawing.Point(202, 50);
      this.tbMargin.Margin = new Padding(3, 3, 3, 0);
      this.tbMargin.Maximum = 50;
      this.tbMargin.Name = "tbMargin";
      this.tbMargin.Size = new System.Drawing.Size(187, 18);
      this.tbMargin.TabIndex = 2;
      this.tbMargin.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbMargin.ValueChanged += new EventHandler(this.PercentTrackbarValueChanged);
      this.grpEffects.AutoSize = true;
      this.grpEffects.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.grpEffects.Controls.Add((Control) this.cbPaperLayout);
      this.grpEffects.Controls.Add((Control) this.labelPaperStrength);
      this.grpEffects.Controls.Add((Control) this.tbPaperStrength);
      this.grpEffects.Controls.Add((Control) this.btBrowsePaper);
      this.grpEffects.Controls.Add((Control) this.cbPaperTexture);
      this.grpEffects.Controls.Add((Control) this.labelPaper);
      this.grpEffects.Controls.Add((Control) this.labelPaging);
      this.grpEffects.Controls.Add((Control) this.cbPageTransition);
      this.grpEffects.Location = new System.Drawing.Point(3, 93);
      this.grpEffects.Name = "grpEffects";
      this.grpEffects.Padding = new Padding(3, 3, 3, 0);
      this.grpEffects.Size = new System.Drawing.Size(395, 138);
      this.grpEffects.TabIndex = 0;
      this.grpEffects.TabStop = false;
      this.grpEffects.Text = "Effects";
      this.cbPaperLayout.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbPaperLayout.FormattingEnabled = true;
      this.cbPaperLayout.Items.AddRange(new object[5]
      {
        (object) "None",
        (object) "Tile",
        (object) "Center",
        (object) "Stretch",
        (object) "Zoom"
      });
      this.cbPaperLayout.Location = new System.Drawing.Point(103, 80);
      this.cbPaperLayout.Name = "cbPaperLayout";
      this.cbPaperLayout.Size = new System.Drawing.Size(248, 21);
      this.cbPaperLayout.TabIndex = 5;
      this.labelPaperStrength.AutoSize = true;
      this.labelPaperStrength.Location = new System.Drawing.Point(100, 107);
      this.labelPaperStrength.Name = "labelPaperStrength";
      this.labelPaperStrength.Size = new System.Drawing.Size(50, 13);
      this.labelPaperStrength.TabIndex = 6;
      this.labelPaperStrength.Text = "Strength:";
      this.tbPaperStrength.Location = new System.Drawing.Point(156, 107);
      this.tbPaperStrength.Margin = new Padding(3, 3, 3, 0);
      this.tbPaperStrength.Name = "tbPaperStrength";
      this.tbPaperStrength.Size = new System.Drawing.Size(195, 18);
      this.tbPaperStrength.TabIndex = 7;
      this.tbPaperStrength.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbPaperStrength.ValueChanged += new EventHandler(this.PercentTrackbarValueChanged);
      this.btBrowsePaper.Location = new System.Drawing.Point(357, 53);
      this.btBrowsePaper.Name = "btBrowsePaper";
      this.btBrowsePaper.Size = new System.Drawing.Size(32, 21);
      this.btBrowsePaper.TabIndex = 4;
      this.btBrowsePaper.Text = "...";
      this.btBrowsePaper.UseVisualStyleBackColor = true;
      this.btBrowsePaper.Click += new EventHandler(this.btBrowsePaper_Click);
      this.cbPaperTexture.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbPaperTexture.FormattingEnabled = true;
      this.cbPaperTexture.Location = new System.Drawing.Point(103, 53);
      this.cbPaperTexture.Name = "cbPaperTexture";
      this.cbPaperTexture.Size = new System.Drawing.Size(248, 21);
      this.cbPaperTexture.TabIndex = 3;
      this.cbPaperTexture.SelectedIndexChanged += new EventHandler(this.cbPaperTexture_SelectedIndexChanged);
      this.labelPaper.AutoSize = true;
      this.labelPaper.Location = new System.Drawing.Point(15, 55);
      this.labelPaper.Name = "labelPaper";
      this.labelPaper.Size = new System.Drawing.Size(38, 13);
      this.labelPaper.TabIndex = 2;
      this.labelPaper.Text = "Paper:";
      this.labelPaper.TextAlign = ContentAlignment.MiddleLeft;
      this.grpBackground.AutoSize = true;
      this.grpBackground.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.grpBackground.Controls.Add((Control) this.labelBackgroundType);
      this.grpBackground.Controls.Add((Control) this.labelBackgroundColor);
      this.grpBackground.Controls.Add((Control) this.cbBackgroundType);
      this.grpBackground.Controls.Add((Control) this.cbTextureLayout);
      this.grpBackground.Controls.Add((Control) this.labelBackgroundTexture);
      this.grpBackground.Controls.Add((Control) this.btBrowseTexture);
      this.grpBackground.Controls.Add((Control) this.cbBackgroundTexture);
      this.grpBackground.Controls.Add((Control) this.cpBackgroundColor);
      this.grpBackground.Location = new System.Drawing.Point(3, 237);
      this.grpBackground.Name = "grpBackground";
      this.grpBackground.Padding = new Padding(3, 3, 3, 0);
      this.grpBackground.Size = new System.Drawing.Size(395, 102);
      this.grpBackground.TabIndex = 0;
      this.grpBackground.TabStop = false;
      this.grpBackground.Text = "Background";
      this.cbTextureLayout.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbTextureLayout.FormattingEnabled = true;
      this.cbTextureLayout.Items.AddRange(new object[5]
      {
        (object) "None",
        (object) "Tile",
        (object) "Center",
        (object) "Stretch",
        (object) "Zoom"
      });
      this.cbTextureLayout.Location = new System.Drawing.Point(103, 79);
      this.cbTextureLayout.Name = "cbTextureLayout";
      this.cbTextureLayout.Size = new System.Drawing.Size(248, 21);
      this.cbTextureLayout.TabIndex = 7;
      this.btApply.FlatStyle = FlatStyle.System;
      this.btApply.Location = new System.Drawing.Point(175, 3);
      this.btApply.Name = "btApply";
      this.btApply.Size = new System.Drawing.Size(80, 24);
      this.btApply.TabIndex = 5;
      this.btApply.Text = "&Apply";
      this.btApply.Click += new EventHandler(this.btApply_Click);
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add((Control) this.grpGeneral);
      this.flowLayoutPanel1.Controls.Add((Control) this.grpEffects);
      this.flowLayoutPanel1.Controls.Add((Control) this.grpBackground);
      this.flowLayoutPanel1.Controls.Add((Control) this.panel1);
      this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(402, 378);
      this.flowLayoutPanel1.TabIndex = 6;
      this.panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.panel1.AutoSize = true;
      this.panel1.Controls.Add((Control) this.btOK);
      this.panel1.Controls.Add((Control) this.btApply);
      this.panel1.Controls.Add((Control) this.btCancel);
      this.panel1.Location = new System.Drawing.Point(141, 345);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(258, 30);
      this.panel1.TabIndex = 3;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(421, 413);
      this.Controls.Add((Control) this.flowLayoutPanel1);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ComicDisplaySettingsDialog);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Book Display Settings";
      this.grpGeneral.ResumeLayout(false);
      this.grpGeneral.PerformLayout();
      this.grpEffects.ResumeLayout(false);
      this.grpEffects.PerformLayout();
      this.grpBackground.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private class TextureFileItem : ComboBoxSkinner.ComboBoxItem<string>
    {
      private Regex rxFormatCode = new Regex("\\s*\\[(?<code>[CSTZ])\\]\\z", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private bool failed;

      public TextureFileItem(string file, bool custom = true)
        : base(file)
      {
        this.Default = TR.Default["None", "None"];
        this.IsOwnerDrawn = true;
        this.IsCustom = custom;
        if (this.IsCustom)
          return;
        this.ParseFileName(file);
      }

      public TextureFileItem()
        : this((string) null, false)
      {
      }

      public string Name { get; set; }

      public bool IsCustom { get; set; }

      public string Default { get; set; }

      public ImageLayout Layout { get; set; }

      public Bitmap Sample { get; set; }

      public override string ToString()
      {
        if (string.IsNullOrEmpty(this.Item))
          return this.Default;
        return !this.IsCustom ? this.Name : Path.GetFileName(this.Item);
      }

      public override bool Equals(object obj)
      {
        return obj is ComicDisplaySettingsDialog.TextureFileItem && ((ComboBoxSkinner.ComboBoxItem<string>) obj).Item == this.Item;
      }

      public override int GetHashCode() => base.GetHashCode();

      private void ParseFileName(string path)
      {
        if (string.IsNullOrEmpty(path))
          return;
        string str = Path.GetFileNameWithoutExtension(path);
        this.Layout = ImageLayout.Tile;
        Match match = this.rxFormatCode.Match(str);
        if (match.Success)
        {
          switch (match.Groups["code"].Value.ToUpper())
          {
            case "C":
              this.Layout = ImageLayout.Center;
              break;
            case "S":
              this.Layout = ImageLayout.Stretch;
              break;
            case "Z":
              this.Layout = ImageLayout.Zoom;
              break;
          }
          str = this.rxFormatCode.Replace(str, string.Empty);
        }
        this.Name = TR.Load("Textures")[str, str.PascalToSpaced()];
      }

      public override System.Drawing.Size Measure(Graphics gr, Font font)
      {
        System.Drawing.Size size = base.Measure(gr, font);
        size.Height *= 2;
        return size;
      }

      public override void Draw(Graphics gr, Rectangle bounds, Color foreColor, Font font)
      {
        int height = this.Measure(gr, font).Height;
        if (this.Sample == null)
        {
          if (!this.failed)
          {
            try
            {
              using (Bitmap image = Image.FromFile(this.Item) as Bitmap)
                this.Sample = image.CreateCopy(new System.Drawing.Size(height * 2, height).ToRectangle(), true);
            }
            catch
            {
              this.failed = true;
            }
          }
        }
        try
        {
          if (this.Sample != null)
            gr.DrawImage((Image) this.Sample, bounds.X, bounds.Y);
        }
        catch (Exception ex)
        {
        }
        using (SolidBrush solidBrush = new SolidBrush(foreColor))
        {
          using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
          {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
          })
          {
            if (this.IsCustom)
              format.Trimming = StringTrimming.EllipsisPath;
            gr.DrawString(this.ToString(), font, (Brush) solidBrush, (RectangleF) bounds.Pad(height * 2 + 4, 0), format);
          }
        }
      }
    }
  }
}

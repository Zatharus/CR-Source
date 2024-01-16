// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.SmartListDialog
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
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
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
  public class SmartListDialog : Form, ISmartListDialog
  {
    private ComicSmartListItem smartComicList;
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private FlowLayoutPanel matcherControls;
    private Panel topPanel;
    private Label labelRules;
    private FlowLayoutPanel flowLayout;
    private GroupBox matcherGroup;
    private Panel bottomPanel;
    private ComboBox cbMatchMode;
    private Label labelMatch;
    private Button btApply;
    private ComboBox cbLimitType;
    private TextBox txLimit;
    private CheckBox chkLimit;
    private ComboBox cbBaseList;
    private ImageList baseImages;
    private Label labelName;
    private TextBox txtName;
    private Button btNext;
    private Button btPrev;
    private CheckBox chkNotBaseList;
    private CheckBox chkQuickOpen;
    private TextBox txtNotes;
    private Label labelNotes;
    private CheckBox chkShowNotes;
    private Button btFilterReset;
    private Label labelFilterReset;
    private Button btQuery;

    public SmartListDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.chkShowNotes.Image = (Image) ((Bitmap) this.chkShowNotes.Image).ScaleDpi();
      this.baseImages.ImageSize = this.baseImages.ImageSize.ScaleDpi();
      this.baseImages.Images.Add(nameof (Library), (Image) Resources.Library);
      this.baseImages.Images.Add("Folder", (Image) Resources.SearchFolder);
      this.baseImages.Images.Add("Search", (Image) Resources.SearchDocument);
      this.baseImages.Images.Add("List", (Image) Resources.List);
      this.baseImages.Images.Add("TempFolder", (Image) Resources.TempFolder);
      this.RestorePosition();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      LocalizeUtility.Localize(TR.Load(this.Name), this.cbLimitType);
      ComboBoxSkinner comboBoxSkinner = new ComboBoxSkinner(this.cbBaseList);
      this.txLimit.EnableOnlyNumberKeys();
      SpinButton.AddUpDown((TextBoxBase) this.txLimit, min: 1);
      this.cbMatchMode.Items.AddRange((object[]) TR.Load(this.Name).GetStrings("MatchMode", "All|Any", '|'));
    }

    public ComicLibrary Library { get; set; }

    public Guid EditId { get; set; }

    public ComicSmartListItem SmartComicList
    {
      get => this.smartComicList;
      set
      {
        this.matcherControls.SuspendLayout();
        try
        {
          if (this.smartComicList != null)
            this.smartComicList.Matchers.Changed -= new EventHandler<SmartListChangedEventArgs<ComicBookMatcher>>(this.Matchers_Changed);
          this.smartComicList = (ComicSmartListItem) null;
          this.FillBaseCombo(value);
          this.smartComicList = value;
          this.txtName.Text = this.smartComicList.Name;
          this.txtNotes.Text = StringUtility.MakeEditBoxMultiline(this.smartComicList.Description);
          this.cbMatchMode.SelectedIndex = this.smartComicList.MatcherMode == MatcherMode.And ? 0 : 1;
          this.chkNotBaseList.Checked = this.smartComicList.NotInBaseList;
          this.chkLimit.Checked = this.smartComicList.Limit;
          this.cbLimitType.SelectedIndex = (int) this.smartComicList.LimitType;
          this.txLimit.Text = this.smartComicList.LimitValue.ToString();
          this.chkQuickOpen.Checked = this.smartComicList.QuickOpen;
          this.chkShowNotes.Checked = !string.IsNullOrEmpty(this.txtNotes.Text) || this.chkLimit.Checked || this.chkQuickOpen.Checked;
          this.btFilterReset.Visible = this.labelFilterReset.Visible = this.smartComicList.ShouldSerializeFilteredIds() && this.chkShowNotes.Checked;
          this.smartComicList.Matchers.Changed += new EventHandler<SmartListChangedEventArgs<ComicBookMatcher>>(this.Matchers_Changed);
          this.matcherControls.Clear(true);
          foreach (ComicBookMatcher matcher in (SmartList<ComicBookMatcher>) this.smartComicList.Matchers)
            this.AddMatcherControl(matcher);
        }
        finally
        {
          this.matcherControls.ResumeLayout(true);
        }
      }
    }

    public bool EnableNavigation
    {
      get => this.btPrev.Visible;
      set
      {
        this.btPrev.Visible = this.btNext.Visible = value;
        if (value)
          this.btQuery.Left = this.btNext.Right + (this.btNext.Left - this.btPrev.Right);
        else
          this.btQuery.Left = this.btPrev.Left;
      }
    }

    public bool PreviousEnabled
    {
      get => this.btPrev.Enabled;
      set => this.btPrev.Enabled = value;
    }

    public bool NextEnabled
    {
      get => this.btNext.Enabled;
      set => this.btNext.Enabled = value;
    }

    public event EventHandler Apply;

    public event EventHandler Next;

    public event EventHandler Previous;

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      IMatcherEditor im = this.FindActiveService<IMatcherEditor>() ?? this.FindFirstService<IMatcherEditor>();
      if (im == null)
        return;
      if (e.Control)
      {
        e.Handled = this.HandleControlSequences(im, e.KeyCode);
      }
      else
      {
        if (this.ActiveControl is ComboBox)
          return;
        switch (e.KeyCode)
        {
          case Keys.Up:
            List<IMatcherEditor> list1 = this.FindServices<IMatcherEditor>().ToList<IMatcherEditor>();
            int index1 = list1.IndexOf(im) - 1;
            if (index1 < 0)
              break;
            list1[index1].SetFocus();
            e.Handled = true;
            break;
          case Keys.Down:
            List<IMatcherEditor> list2 = this.FindServices<IMatcherEditor>().ToList<IMatcherEditor>();
            int index2 = list2.IndexOf(im) + 1;
            if (index2 >= list2.Count)
              break;
            list2[index2].SetFocus();
            e.Handled = true;
            break;
        }
      }
    }

    private bool HandleControlSequences(IMatcherEditor im, Keys keyCode)
    {
      switch (keyCode)
      {
        case Keys.C:
          im.CopyClipboard();
          break;
        case Keys.D:
          im.MoveDown();
          break;
        case Keys.G:
          im.AddGroup();
          break;
        case Keys.R:
          im.AddRule();
          break;
        case Keys.U:
          im.MoveUp();
          break;
        case Keys.V:
          im.PasteClipboard();
          break;
        case Keys.X:
          im.CutClipboard();
          break;
        default:
          return false;
      }
      return true;
    }

    private void Matchers_Changed(object sender, SmartListChangedEventArgs<ComicBookMatcher> e)
    {
      int dialogEditorOffset = this.DialogEditorOffset;
      switch (e.Action)
      {
        case SmartListAction.Insert:
          if (this.matcherControls.Controls.OfType<Control>().FirstOrDefault<Control>((Func<Control, bool>) (c => c.Tag == e.Item)) == null)
          {
            this.AddMatcherControl(e.Item);
            break;
          }
          break;
        case SmartListAction.Remove:
          Control control = this.matcherControls.Controls.OfType<Control>().FirstOrDefault<Control>((Func<Control, bool>) (cc => cc.Tag == e.Item));
          if (control != null)
          {
            this.matcherControls.Controls.Remove(control);
            break;
          }
          break;
        case SmartListAction.Move:
          Control child = this.matcherControls.Controls.OfType<Control>().FirstOrDefault<Control>((Func<Control, bool>) (cc => cc.Tag == e.Item));
          if (child != null)
          {
            this.matcherControls.Controls.SetChildIndex(child, e.Index);
            break;
          }
          break;
      }
      this.DialogEditorOffset = dialogEditorOffset;
    }

    private void cbMatchMode_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SmartComicList.MatcherMode = this.cbMatchMode.SelectedIndex == 0 ? MatcherMode.And : MatcherMode.Or;
    }

    private void chkLimit_CheckedChanged(object sender, EventArgs e)
    {
      this.SmartComicList.Limit = this.txLimit.Enabled = this.cbLimitType.Enabled = this.chkLimit.Checked;
    }

    private void cbLimitType_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SmartComicList.LimitType = (ComicSmartListLimitType) this.cbLimitType.SelectedIndex;
    }

    private void txLimit_TextChanged(object sender, EventArgs e)
    {
      int result;
      int.TryParse(this.txLimit.Text, out result);
      if (result < 0)
        result = 0;
      this.SmartComicList.LimitValue = result;
    }

    private void btApply_Click(object sender, EventArgs e)
    {
      if (this.Apply == null)
        return;
      this.Apply((object) this, EventArgs.Empty);
    }

    private void btOK_Click(object sender, EventArgs e)
    {
      if (this.Apply == null)
        return;
      this.Apply((object) this, EventArgs.Empty);
    }

    private void btPrev_Click(object sender, EventArgs e)
    {
      if (this.Apply != null)
        this.Apply((object) this, EventArgs.Empty);
      if (this.Previous == null)
        return;
      this.Previous((object) this, EventArgs.Empty);
    }

    private void btNext_Click(object sender, EventArgs e)
    {
      if (this.Apply != null)
        this.Apply((object) this, EventArgs.Empty);
      if (this.Next == null)
        return;
      this.Next((object) this, EventArgs.Empty);
    }

    private void cbBaseList_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.smartComicList == null)
        return;
      this.smartComicList.BaseListId = ((SmartListDialog.ReferenceItem) this.cbBaseList.SelectedItem).Id;
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
      this.SmartComicList.Name = this.txtName.Text.Trim();
    }

    private void txtNotes_TextChanged(object sender, EventArgs e)
    {
      this.SmartComicList.Description = this.txtNotes.Text.Trim();
    }

    private void chkNotBaseList_CheckedChanged(object sender, EventArgs e)
    {
      this.SmartComicList.NotInBaseList = this.chkNotBaseList.Checked;
    }

    private void chkQuickOpen_CheckedChanged(object sender, EventArgs e)
    {
      this.SmartComicList.QuickOpen = this.chkQuickOpen.Checked;
    }

    private void chkShowNotes_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowNotes(this.chkShowNotes.Checked);
    }

    private void btFilterReset_Click(object sender, EventArgs e)
    {
      this.SmartComicList.ClearFiltered();
      this.btFilterReset.Visible = this.labelFilterReset.Visible = false;
    }

    private void ShowNotes(bool show)
    {
      this.chkQuickOpen.Visible = this.cbLimitType.Visible = this.txLimit.Visible = this.chkLimit.Visible = this.labelNotes.Visible = this.txtNotes.Visible = show;
      this.labelFilterReset.Visible = this.btFilterReset.Visible = this.SmartComicList.ShouldSerializeFilteredIds() & show;
    }

    private void FillBaseCombo(ComicSmartListItem scl)
    {
      this.cbBaseList.Items.Clear();
      foreach (ComicListItem comicListItem in this.Library.ComicLists.GetItems<ComicListItem>())
      {
        Guid id = comicListItem is ComicLibraryListItem ? Guid.Empty : comicListItem.Id;
        if (!comicListItem.RecursionTest(this.EditId))
        {
          this.cbBaseList.Items.Add((object) new SmartListDialog.ReferenceItem(comicListItem.GetLevel(), comicListItem.Name, id, this.baseImages.Images[comicListItem.ImageKey]));
          if (id == scl.BaseListId)
            this.cbBaseList.SelectedIndex = this.cbBaseList.Items.Count - 1;
        }
      }
      if (this.cbBaseList.Items.Count <= 0 || this.cbBaseList.SelectedIndex != -1)
        return;
      this.cbBaseList.SelectedIndex = 0;
    }

    private void AddMatcherControl(ComicBookMatcher icbm)
    {
      int width = this.cbBaseList.Right - this.matcherControls.Left;
      Control child = icbm is ComicBookGroupMatcher ? this.CreateGroupMatchPanel(icbm as ComicBookGroupMatcher, width) : this.CreateMatchPanel(icbm as ComicBookValueMatcher, width);
      this.matcherControls.Controls.Add(child);
      this.matcherControls.Controls.SetChildIndex(child, this.smartComicList.Matchers.IndexOf(icbm));
      this.matcherControls.AutoTabIndex();
    }

    private Control CreateMatchPanel(ComicBookValueMatcher matcher, int width)
    {
      return (Control) new MatcherEditor(this.smartComicList.Matchers, matcher, 0, width);
    }

    private Control CreateGroupMatchPanel(ComicBookGroupMatcher matcher, int width)
    {
      return (Control) new MatcherGroupEditor(this.smartComicList.Matchers, matcher, 0, width);
    }

    public int DialogEditorOffset
    {
      get => -this.matcherControls.AutoScrollPosition.Y;
      set => this.matcherControls.AutoScrollPosition = new System.Drawing.Point(0, value);
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
      this.btCancel = new Button();
      this.btOK = new Button();
      this.matcherControls = new FlowLayoutPanel();
      this.topPanel = new Panel();
      this.btFilterReset = new Button();
      this.labelFilterReset = new Label();
      this.chkShowNotes = new CheckBox();
      this.txLimit = new TextBox();
      this.txtNotes = new TextBox();
      this.labelNotes = new Label();
      this.chkLimit = new CheckBox();
      this.labelName = new Label();
      this.chkQuickOpen = new CheckBox();
      this.cbLimitType = new ComboBox();
      this.txtName = new TextBox();
      this.chkNotBaseList = new CheckBox();
      this.cbMatchMode = new ComboBox();
      this.cbBaseList = new ComboBox();
      this.labelMatch = new Label();
      this.labelRules = new Label();
      this.flowLayout = new FlowLayoutPanel();
      this.matcherGroup = new GroupBox();
      this.bottomPanel = new Panel();
      this.btNext = new Button();
      this.btPrev = new Button();
      this.btApply = new Button();
      this.baseImages = new ImageList(this.components);
      this.btQuery = new Button();
      this.topPanel.SuspendLayout();
      this.flowLayout.SuspendLayout();
      this.matcherGroup.SuspendLayout();
      this.bottomPanel.SuspendLayout();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(435, 3);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 4;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(351, 3);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 3;
      this.btOK.Text = "&OK";
      this.btOK.Click += new EventHandler(this.btOK_Click);
      this.matcherControls.AutoScroll = true;
      this.matcherControls.AutoSize = true;
      this.matcherControls.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.matcherControls.FlowDirection = FlowDirection.TopDown;
      this.matcherControls.Location = new System.Drawing.Point(6, 41);
      this.matcherControls.Margin = new Padding(3, 3, 3, 0);
      this.matcherControls.MaximumSize = new System.Drawing.Size(0, 400);
      this.matcherControls.MinimumSize = new System.Drawing.Size(588, 20);
      this.matcherControls.Name = "matcherControls";
      this.matcherControls.Padding = new Padding(0, 0, 20, 0);
      this.matcherControls.Size = new System.Drawing.Size(588, 20);
      this.matcherControls.TabIndex = 0;
      this.matcherControls.WrapContents = false;
      this.topPanel.AutoSize = true;
      this.topPanel.Controls.Add((Control) this.btFilterReset);
      this.topPanel.Controls.Add((Control) this.labelFilterReset);
      this.topPanel.Controls.Add((Control) this.chkShowNotes);
      this.topPanel.Controls.Add((Control) this.txLimit);
      this.topPanel.Controls.Add((Control) this.txtNotes);
      this.topPanel.Controls.Add((Control) this.labelNotes);
      this.topPanel.Controls.Add((Control) this.chkLimit);
      this.topPanel.Controls.Add((Control) this.labelName);
      this.topPanel.Controls.Add((Control) this.chkQuickOpen);
      this.topPanel.Controls.Add((Control) this.cbLimitType);
      this.topPanel.Controls.Add((Control) this.txtName);
      this.topPanel.Dock = DockStyle.Top;
      this.topPanel.Location = new System.Drawing.Point(3, 3);
      this.topPanel.Name = "topPanel";
      this.topPanel.Size = new System.Drawing.Size(602, 125);
      this.topPanel.TabIndex = 0;
      this.btFilterReset.Location = new System.Drawing.Point(344, 101);
      this.btFilterReset.Name = "btFilterReset";
      this.btFilterReset.Size = new System.Drawing.Size(90, 21);
      this.btFilterReset.TabIndex = 10;
      this.btFilterReset.Text = "Reset";
      this.btFilterReset.UseVisualStyleBackColor = true;
      this.btFilterReset.Click += new EventHandler(this.btFilterReset_Click);
      this.labelFilterReset.AutoSize = true;
      this.labelFilterReset.Location = new System.Drawing.Point(69, 105);
      this.labelFilterReset.Name = "labelFilterReset";
      this.labelFilterReset.Size = new System.Drawing.Size(269, 13);
      this.labelFilterReset.TabIndex = 9;
      this.labelFilterReset.Text = "Some Books have been manually removed from this list.";
      this.chkShowNotes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkShowNotes.Appearance = Appearance.Button;
      this.chkShowNotes.Image = (Image) Resources.DoubleArrow;
      this.chkShowNotes.Location = new System.Drawing.Point(574, 0);
      this.chkShowNotes.Name = "chkShowNotes";
      this.chkShowNotes.Size = new System.Drawing.Size(22, 22);
      this.chkShowNotes.TabIndex = 2;
      this.chkShowNotes.UseVisualStyleBackColor = true;
      this.chkShowNotes.CheckedChanged += new EventHandler(this.chkShowNotes_CheckedChanged);
      this.txLimit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.txLimit.Enabled = false;
      this.txLimit.Location = new System.Drawing.Point(443, 74);
      this.txLimit.Name = "txLimit";
      this.txLimit.Size = new System.Drawing.Size(45, 20);
      this.txLimit.TabIndex = 7;
      this.txLimit.TextAlign = HorizontalAlignment.Right;
      this.txLimit.Visible = false;
      this.txLimit.TextChanged += new EventHandler(this.txLimit_TextChanged);
      this.txtNotes.AcceptsReturn = true;
      this.txtNotes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txtNotes.Location = new System.Drawing.Point(68, 26);
      this.txtNotes.Multiline = true;
      this.txtNotes.Name = "txtNotes";
      this.txtNotes.ScrollBars = ScrollBars.Vertical;
      this.txtNotes.Size = new System.Drawing.Size(366, 69);
      this.txtNotes.TabIndex = 4;
      this.txtNotes.Visible = false;
      this.txtNotes.TextChanged += new EventHandler(this.txtNotes_TextChanged);
      this.labelNotes.Location = new System.Drawing.Point(0, 26);
      this.labelNotes.Name = "labelNotes";
      this.labelNotes.Size = new System.Drawing.Size(62, 13);
      this.labelNotes.TabIndex = 3;
      this.labelNotes.Text = "Notes:";
      this.labelNotes.TextAlign = ContentAlignment.TopRight;
      this.labelNotes.Visible = false;
      this.chkLimit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkLimit.Location = new System.Drawing.Point(443, 51);
      this.chkLimit.Name = "chkLimit";
      this.chkLimit.Size = new System.Drawing.Size(129, 17);
      this.chkLimit.TabIndex = 6;
      this.chkLimit.Text = "Limit to ";
      this.chkLimit.UseVisualStyleBackColor = true;
      this.chkLimit.Visible = false;
      this.chkLimit.CheckedChanged += new EventHandler(this.chkLimit_CheckedChanged);
      this.labelName.Location = new System.Drawing.Point(3, 4);
      this.labelName.Name = "labelName";
      this.labelName.Size = new System.Drawing.Size(59, 13);
      this.labelName.TabIndex = 0;
      this.labelName.Text = "Name:";
      this.labelName.TextAlign = ContentAlignment.TopRight;
      this.chkQuickOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkQuickOpen.Location = new System.Drawing.Point(443, 28);
      this.chkQuickOpen.Name = "chkQuickOpen";
      this.chkQuickOpen.Size = new System.Drawing.Size(129, 17);
      this.chkQuickOpen.TabIndex = 5;
      this.chkQuickOpen.Text = "Show in Quick Open";
      this.chkQuickOpen.UseVisualStyleBackColor = true;
      this.chkQuickOpen.Visible = false;
      this.chkQuickOpen.CheckedChanged += new EventHandler(this.chkQuickOpen_CheckedChanged);
      this.cbLimitType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.cbLimitType.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbLimitType.Enabled = false;
      this.cbLimitType.FormattingEnabled = true;
      this.cbLimitType.Items.AddRange(new object[3]
      {
        (object) "Books",
        (object) "MB",
        (object) "GB"
      });
      this.cbLimitType.Location = new System.Drawing.Point(494, 74);
      this.cbLimitType.Name = "cbLimitType";
      this.cbLimitType.Size = new System.Drawing.Size(78, 21);
      this.cbLimitType.TabIndex = 8;
      this.cbLimitType.Visible = false;
      this.cbLimitType.SelectedIndexChanged += new EventHandler(this.cbLimitType_SelectedIndexChanged);
      this.txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txtName.Location = new System.Drawing.Point(68, 1);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(504, 20);
      this.txtName.TabIndex = 1;
      this.txtName.TextChanged += new EventHandler(this.txtName_TextChanged);
      this.chkNotBaseList.Appearance = Appearance.Button;
      this.chkNotBaseList.Location = new System.Drawing.Point(6, 16);
      this.chkNotBaseList.Name = "chkNotBaseList";
      this.chkNotBaseList.Size = new System.Drawing.Size(21, 23);
      this.chkNotBaseList.TabIndex = 2;
      this.chkNotBaseList.Text = "!";
      this.chkNotBaseList.TextAlign = ContentAlignment.MiddleCenter;
      this.chkNotBaseList.UseVisualStyleBackColor = true;
      this.chkNotBaseList.CheckedChanged += new EventHandler(this.chkNotBaseList_CheckedChanged);
      this.cbMatchMode.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbMatchMode.FormattingEnabled = true;
      this.cbMatchMode.Location = new System.Drawing.Point(103, 18);
      this.cbMatchMode.Name = "cbMatchMode";
      this.cbMatchMode.Size = new System.Drawing.Size(65, 21);
      this.cbMatchMode.TabIndex = 4;
      this.cbMatchMode.SelectedIndexChanged += new EventHandler(this.cbMatchMode_SelectedIndexChanged);
      this.cbBaseList.DisplayMember = "FullName";
      this.cbBaseList.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbBaseList.Location = new System.Drawing.Point(317, 16);
      this.cbBaseList.Name = "cbBaseList";
      this.cbBaseList.Size = new System.Drawing.Size((int) byte.MaxValue, 21);
      this.cbBaseList.TabIndex = 6;
      this.cbBaseList.ValueMember = "FullName";
      this.cbBaseList.SelectedIndexChanged += new EventHandler(this.cbBaseList_SelectedIndexChanged);
      this.labelMatch.Location = new System.Drawing.Point(33, 21);
      this.labelMatch.Name = "labelMatch";
      this.labelMatch.Size = new System.Drawing.Size(69, 13);
      this.labelMatch.TabIndex = 3;
      this.labelMatch.Text = "Match";
      this.labelRules.Location = new System.Drawing.Point(174, 21);
      this.labelRules.Name = "labelRules";
      this.labelRules.Size = new System.Drawing.Size(126, 13);
      this.labelRules.TabIndex = 5;
      this.labelRules.Text = "of the following rules on";
      this.flowLayout.AutoSize = true;
      this.flowLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayout.Controls.Add((Control) this.topPanel);
      this.flowLayout.Controls.Add((Control) this.matcherGroup);
      this.flowLayout.Controls.Add((Control) this.bottomPanel);
      this.flowLayout.FlowDirection = FlowDirection.TopDown;
      this.flowLayout.Location = new System.Drawing.Point(6, 12);
      this.flowLayout.Name = "flowLayout";
      this.flowLayout.Size = new System.Drawing.Size(608, 244);
      this.flowLayout.TabIndex = 0;
      this.matcherGroup.AutoSize = true;
      this.matcherGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.matcherGroup.Controls.Add((Control) this.chkNotBaseList);
      this.matcherGroup.Controls.Add((Control) this.matcherControls);
      this.matcherGroup.Controls.Add((Control) this.labelRules);
      this.matcherGroup.Controls.Add((Control) this.cbMatchMode);
      this.matcherGroup.Controls.Add((Control) this.labelMatch);
      this.matcherGroup.Controls.Add((Control) this.cbBaseList);
      this.matcherGroup.Location = new System.Drawing.Point(3, 131);
      this.matcherGroup.Margin = new Padding(3, 0, 3, 3);
      this.matcherGroup.MinimumSize = new System.Drawing.Size(600, 0);
      this.matcherGroup.Name = "matcherGroup";
      this.matcherGroup.Padding = new Padding(3, 0, 3, 0);
      this.matcherGroup.Size = new System.Drawing.Size(600, 74);
      this.matcherGroup.TabIndex = 1;
      this.matcherGroup.TabStop = false;
      this.bottomPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.bottomPanel.AutoSize = true;
      this.bottomPanel.Controls.Add((Control) this.btQuery);
      this.bottomPanel.Controls.Add((Control) this.btNext);
      this.bottomPanel.Controls.Add((Control) this.btPrev);
      this.bottomPanel.Controls.Add((Control) this.btOK);
      this.bottomPanel.Controls.Add((Control) this.btCancel);
      this.bottomPanel.Controls.Add((Control) this.btApply);
      this.bottomPanel.Location = new System.Drawing.Point(3, 211);
      this.bottomPanel.Name = "bottomPanel";
      this.bottomPanel.Size = new System.Drawing.Size(602, 30);
      this.bottomPanel.TabIndex = 2;
      this.btNext.FlatStyle = FlatStyle.System;
      this.btNext.Location = new System.Drawing.Point(86, 3);
      this.btNext.Name = "btNext";
      this.btNext.Size = new System.Drawing.Size(80, 24);
      this.btNext.TabIndex = 1;
      this.btNext.Text = "&Next";
      this.btNext.Click += new EventHandler(this.btNext_Click);
      this.btPrev.FlatStyle = FlatStyle.System;
      this.btPrev.Location = new System.Drawing.Point(0, 3);
      this.btPrev.Name = "btPrev";
      this.btPrev.Size = new System.Drawing.Size(80, 24);
      this.btPrev.TabIndex = 0;
      this.btPrev.Text = "&Previous";
      this.btPrev.Click += new EventHandler(this.btPrev_Click);
      this.btApply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btApply.FlatStyle = FlatStyle.System;
      this.btApply.Location = new System.Drawing.Point(519, 3);
      this.btApply.Name = "btApply";
      this.btApply.Size = new System.Drawing.Size(80, 24);
      this.btApply.TabIndex = 5;
      this.btApply.Text = "&Apply";
      this.btApply.Click += new EventHandler(this.btApply_Click);
      this.baseImages.ColorDepth = ColorDepth.Depth8Bit;
      this.baseImages.ImageSize = new System.Drawing.Size(16, 16);
      this.baseImages.TransparentColor = Color.Transparent;
      this.btQuery.DialogResult = DialogResult.Retry;
      this.btQuery.FlatStyle = FlatStyle.System;
      this.btQuery.Location = new System.Drawing.Point(172, 3);
      this.btQuery.Name = "btQuery";
      this.btQuery.Size = new System.Drawing.Size(80, 24);
      this.btQuery.TabIndex = 2;
      this.btQuery.Text = "&Query";
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(617, 258);
      this.Controls.Add((Control) this.flowLayout);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (SmartListDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Edit Smart List";
      this.topPanel.ResumeLayout(false);
      this.topPanel.PerformLayout();
      this.flowLayout.ResumeLayout(false);
      this.flowLayout.PerformLayout();
      this.matcherGroup.ResumeLayout(false);
      this.matcherGroup.PerformLayout();
      this.bottomPanel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private class ReferenceItem : ComboBoxSkinner.ComboBoxItem<string>
    {
      private const int ImageSpacing = 4;

      public ReferenceItem(int level, string name, Guid id, Image image)
        : base(name)
      {
        this.IsOwnerDrawn = true;
        this.Level = level;
        this.Id = id;
        this.Image = image;
      }

      public int Level { get; set; }

      public Guid Id { get; set; }

      public Image Image { get; set; }

      public override void Draw(Graphics gr, Rectangle bounds, Color foreColor, Font font)
      {
        bounds = bounds.Pad(this.Level * this.Image.Width, 0);
        gr.DrawImage(this.Image, this.Image.Size.Align(bounds, ContentAlignment.MiddleLeft));
        bounds = bounds.Pad(this.Image.Width + 4, 0);
        using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
        {
          LineAlignment = StringAlignment.Center
        })
        {
          using (SolidBrush solidBrush = new SolidBrush(foreColor))
            gr.DrawString(this.Item, font, (Brush) solidBrush, (RectangleF) bounds, format);
        }
      }

      public override System.Drawing.Size Measure(Graphics gr, Font font)
      {
        System.Drawing.Size size = gr.MeasureString(this.Item, font).ToSize();
        size.Width += this.Level * this.Image.Width + this.Image.Width + 4;
        size.Height = Math.Max(size.Height, this.Image.Height);
        return size;
      }
    }
  }
}

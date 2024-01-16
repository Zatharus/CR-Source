// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ListLayoutDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Localize;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Viewer.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ListLayoutDialog : Form
  {
    private Action<DisplayListConfig> apply;
    private IContainer components;
    private Button btOK;
    private Button btCancel;
    private TabPage tabDetails;
    private Label labelSelectColumn;
    private Button btHideAll;
    private Button btShowAll;
    private Label labelHint;
    private AutoRepeatButton btMoveDown;
    private AutoRepeatButton btMoveUp;
    private TabControl tab;
    private TabPage tabThumbnails;
    private ComboBox cbSecondLine;
    private Label labelSecondLine;
    private ComboBox cbFirstLine;
    private Label labelFirstLine;
    private CheckBox chkHideCaption;
    private Label labelSelectThumbnailText;
    private ComboBox cbThirdLine;
    private Label labelThirdLine;
    private ListViewEx lvColumns;
    private ColumnHeader chName;
    private ColumnHeader chDescription;
    private TabPage tabTiles;
    private CheckedListBox lbTileItems;
    private AutoRepeatButton btTilesDefault;
    private Button btApply;

    public ListLayoutDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.lvColumns.Columns.ScaleDpi();
      this.RestorePosition();
      IdleProcess.Idle += new EventHandler(this.Application_Idle);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
      {
        IdleProcess.Idle -= new EventHandler(this.Application_Idle);
        this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public DisplayListConfig DisplayListConfig { get; set; }

    public ItemViewConfig Config
    {
      get => this.DisplayListConfig.View;
      set => this.DisplayListConfig.View = value;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      string text = string.Format("<{0}>", (object) TR.Default["None"]);
      foreach (ItemViewColumnInfo column in this.Config.Columns)
      {
        this.AddItem(column);
        if (this.DisplayListConfig.Thumbnail != null)
        {
          this.cbFirstLine.Items.Add((object) new ListLayoutDialog.CaptionData(column.Id, column.Name));
          this.cbSecondLine.Items.Add((object) new ListLayoutDialog.CaptionData(column.Id, column.Name));
          this.cbThirdLine.Items.Add((object) new ListLayoutDialog.CaptionData(column.Id, column.Name));
        }
      }
      if (this.DisplayListConfig.Thumbnail == null)
      {
        this.tab.TabPages.Remove(this.tabThumbnails);
        this.tab.TabPages.Remove(this.tabTiles);
      }
      else
      {
        this.cbFirstLine.Items.Add((object) new ListLayoutDialog.CaptionData(-1, text));
        this.cbSecondLine.Items.Add((object) new ListLayoutDialog.CaptionData(-1, text));
        this.cbThirdLine.Items.Add((object) new ListLayoutDialog.CaptionData(-1, text));
        ListLayoutDialog.SelectCaption(this.cbFirstLine, this.DisplayListConfig.Thumbnail.CaptionIds.Count > 0 ? this.DisplayListConfig.Thumbnail.CaptionIds[0] : -1);
        ListLayoutDialog.SelectCaption(this.cbSecondLine, this.DisplayListConfig.Thumbnail.CaptionIds.Count > 1 ? this.DisplayListConfig.Thumbnail.CaptionIds[1] : -1);
        ListLayoutDialog.SelectCaption(this.cbThirdLine, this.DisplayListConfig.Thumbnail.CaptionIds.Count > 2 ? this.DisplayListConfig.Thumbnail.CaptionIds[2] : -1);
        this.chkHideCaption.Checked = this.DisplayListConfig.Thumbnail.HideCaptions;
        foreach (int bitValue in BitUtility.GetBitValues(4194303))
          this.lbTileItems.Items.Add((object) new ListLayoutDialog.TileTextItem()
          {
            Value = bitValue,
            Text = LocalizeUtility.LocalizeEnum(typeof (ComicTextElements), bitValue)
          });
        this.SetTileTextElements(this.DisplayListConfig.Thumbnail.TextElements);
      }
    }

    private void Application_Idle(object sender, EventArgs e)
    {
      this.btMoveUp.Enabled = this.SelectedColumnIndex() >= 1;
      this.btMoveDown.Enabled = this.SelectedColumnIndex() >= 0 && this.SelectedColumnIndex() <= this.lvColumns.Items.Count - 2;
    }

    private void btMoveUp_Click(object sender, EventArgs e)
    {
      int num1 = this.SelectedColumnIndex();
      if (num1 <= 0)
        return;
      this.lvColumns.BeginUpdate();
      ListViewItem selectedItem = this.lvColumns.SelectedItems[0];
      try
      {
        this.lvColumns.Items.Remove(selectedItem);
        int num2;
        this.lvColumns.Items.Insert(num2 = num1 - 1, selectedItem);
        selectedItem.Selected = true;
        selectedItem.EnsureVisible();
      }
      finally
      {
        this.lvColumns.EndUpdate();
      }
    }

    private void btMoveDown_Click(object sender, EventArgs e)
    {
      int num1 = this.SelectedColumnIndex();
      if (num1 < 0 || num1 >= this.lvColumns.Items.Count - 1)
        return;
      this.lvColumns.BeginUpdate();
      ListViewItem selectedItem = this.lvColumns.SelectedItems[0];
      try
      {
        this.lvColumns.Items.Remove(selectedItem);
        int num2;
        this.lvColumns.Items.Insert(num2 = num1 + 1, selectedItem);
        selectedItem.Selected = true;
        selectedItem.EnsureVisible();
      }
      finally
      {
        this.lvColumns.EndUpdate();
      }
    }

    private void btShowAll_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem listViewItem in this.lvColumns.Items)
        listViewItem.Checked = true;
    }

    private void btHideAll_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem listViewItem in this.lvColumns.Items)
        listViewItem.Checked = false;
    }

    private void btDefaultTile_Click(object sender, EventArgs e)
    {
      this.SetTileTextElements(ComicTextElements.DefaultFileComic);
    }

    private void btApply_Click(object sender, EventArgs e)
    {
      this.Apply();
      if (this.apply == null)
        return;
      this.apply(this.DisplayListConfig);
    }

    private static int GetCaptionId(ComboBox cb)
    {
      return cb.SelectedItem is ListLayoutDialog.CaptionData selectedItem ? selectedItem.Id : -1;
    }

    private void Apply()
    {
      this.Config.Columns.Clear();
      foreach (ListViewItem listViewItem in this.lvColumns.Items)
      {
        ItemViewColumnInfo tag = (ItemViewColumnInfo) listViewItem.Tag;
        tag.Visible = listViewItem.Checked;
        this.Config.Columns.Add(tag);
      }
      this.DisplayListConfig.Thumbnail.HideCaptions = this.chkHideCaption.Checked;
      this.DisplayListConfig.Thumbnail.CaptionIds.Clear();
      if (ListLayoutDialog.GetCaptionId(this.cbFirstLine) != -1)
        this.DisplayListConfig.Thumbnail.CaptionIds.Add(ListLayoutDialog.GetCaptionId(this.cbFirstLine));
      if (ListLayoutDialog.GetCaptionId(this.cbSecondLine) != -1)
        this.DisplayListConfig.Thumbnail.CaptionIds.Add(ListLayoutDialog.GetCaptionId(this.cbSecondLine));
      if (ListLayoutDialog.GetCaptionId(this.cbThirdLine) != -1)
        this.DisplayListConfig.Thumbnail.CaptionIds.Add(ListLayoutDialog.GetCaptionId(this.cbThirdLine));
      this.DisplayListConfig.Thumbnail.TextElements = (ComicTextElements) this.lbTileItems.Items.OfType<ListLayoutDialog.TileTextItem>().Where<ListLayoutDialog.TileTextItem>((Func<ListLayoutDialog.TileTextItem, bool>) (tti => this.lbTileItems.GetItemChecked(this.lbTileItems.Items.IndexOf((object) tti)))).Sum<ListLayoutDialog.TileTextItem>((Func<ListLayoutDialog.TileTextItem, int>) (tti => tti.Value));
    }

    private void AddItem(ItemViewColumnInfo ci)
    {
      ListViewItem listViewItem = this.lvColumns.Items.Add(ci.Name);
      listViewItem.Checked = ci.Visible;
      listViewItem.Tag = (object) ci;
      if (!(ci.Tag is ComicListField tag))
        return;
      listViewItem.SubItems.Add(tag.Description);
    }

    private int SelectedColumnIndex()
    {
      return this.lvColumns.SelectedIndices.Count <= 0 ? -1 : this.lvColumns.SelectedIndices[0];
    }

    private void SetTileTextElements(ComicTextElements texts)
    {
      for (int index = 0; index < this.lbTileItems.Items.Count; ++index)
      {
        int num = ((ListLayoutDialog.TileTextItem) this.lbTileItems.Items[index]).Value;
        this.lbTileItems.SetItemChecked(index, ((ComicTextElements) num & texts) != 0);
      }
    }

    private static void SelectCaption(ComboBox cb, int id)
    {
      for (int index = 0; index < cb.Items.Count; ++index)
      {
        if (((ListLayoutDialog.CaptionData) cb.Items[index]).Id == id)
        {
          cb.SelectedIndex = index;
          break;
        }
      }
    }

    public static bool Show(
      IWin32Window parent,
      DisplayListConfig displayListConfig,
      ItemViewMode mode,
      Action<DisplayListConfig> apply = null)
    {
      using (ListLayoutDialog listLayoutDialog = new ListLayoutDialog())
      {
        listLayoutDialog.apply = apply;
        listLayoutDialog.DisplayListConfig = displayListConfig;
        switch (mode)
        {
          case ItemViewMode.Thumbnail:
            listLayoutDialog.tab.SelectedIndex = 1;
            break;
          case ItemViewMode.Tile:
            listLayoutDialog.tab.SelectedIndex = 2;
            break;
        }
        if (listLayoutDialog.ShowDialog(parent) != DialogResult.OK)
          return false;
        listLayoutDialog.Apply();
        return true;
      }
    }

    private void InitializeComponent()
    {
      this.btOK = new Button();
      this.btCancel = new Button();
      this.tabDetails = new TabPage();
      this.lvColumns = new ListViewEx();
      this.chName = new ColumnHeader();
      this.chDescription = new ColumnHeader();
      this.labelSelectColumn = new Label();
      this.btHideAll = new Button();
      this.btShowAll = new Button();
      this.labelHint = new Label();
      this.btMoveDown = new AutoRepeatButton();
      this.btMoveUp = new AutoRepeatButton();
      this.tab = new TabControl();
      this.tabThumbnails = new TabPage();
      this.chkHideCaption = new CheckBox();
      this.labelSelectThumbnailText = new Label();
      this.cbThirdLine = new ComboBox();
      this.labelThirdLine = new Label();
      this.cbSecondLine = new ComboBox();
      this.labelSecondLine = new Label();
      this.cbFirstLine = new ComboBox();
      this.labelFirstLine = new Label();
      this.tabTiles = new TabPage();
      this.btTilesDefault = new AutoRepeatButton();
      this.lbTileItems = new CheckedListBox();
      this.btApply = new Button();
      this.tabDetails.SuspendLayout();
      this.tab.SuspendLayout();
      this.tabThumbnails.SuspendLayout();
      this.tabTiles.SuspendLayout();
      this.SuspendLayout();
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(252, 401);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 1;
      this.btOK.Text = "&OK";
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(338, 401);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 2;
      this.btCancel.Text = "&Cancel";
      this.tabDetails.Controls.Add((Control) this.lvColumns);
      this.tabDetails.Controls.Add((Control) this.labelSelectColumn);
      this.tabDetails.Controls.Add((Control) this.btHideAll);
      this.tabDetails.Controls.Add((Control) this.btShowAll);
      this.tabDetails.Controls.Add((Control) this.labelHint);
      this.tabDetails.Controls.Add((Control) this.btMoveDown);
      this.tabDetails.Controls.Add((Control) this.btMoveUp);
      this.tabDetails.Location = new System.Drawing.Point(4, 22);
      this.tabDetails.Name = "tabDetails";
      this.tabDetails.Padding = new Padding(3);
      this.tabDetails.Size = new System.Drawing.Size(484, 357);
      this.tabDetails.TabIndex = 0;
      this.tabDetails.Text = "Details";
      this.tabDetails.UseVisualStyleBackColor = true;
      this.lvColumns.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.lvColumns.CheckBoxes = true;
      this.lvColumns.Columns.AddRange(new ColumnHeader[2]
      {
        this.chName,
        this.chDescription
      });
      this.lvColumns.EnableMouseReorder = true;
      this.lvColumns.FullRowSelect = true;
      this.lvColumns.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.lvColumns.HideSelection = false;
      this.lvColumns.Location = new System.Drawing.Point(12, 40);
      this.lvColumns.MultiSelect = false;
      this.lvColumns.Name = "lvColumns";
      this.lvColumns.Size = new System.Drawing.Size(378, 291);
      this.lvColumns.TabIndex = 7;
      this.lvColumns.UseCompatibleStateImageBehavior = false;
      this.lvColumns.View = View.Details;
      this.chName.Text = "Name";
      this.chName.Width = (int) sbyte.MaxValue;
      this.chDescription.Text = "Description";
      this.chDescription.Width = 221;
      this.labelSelectColumn.AutoSize = true;
      this.labelSelectColumn.Location = new System.Drawing.Point(6, 21);
      this.labelSelectColumn.Name = "labelSelectColumn";
      this.labelSelectColumn.Size = new System.Drawing.Size(219, 13);
      this.labelSelectColumn.TabIndex = 0;
      this.labelSelectColumn.Text = "Please select the columns you want to show:";
      this.btHideAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btHideAll.Location = new System.Drawing.Point(398, 140);
      this.btHideAll.Name = "btHideAll";
      this.btHideAll.Size = new System.Drawing.Size(80, 24);
      this.btHideAll.TabIndex = 5;
      this.btHideAll.Text = "&Hide All";
      this.btHideAll.UseVisualStyleBackColor = true;
      this.btHideAll.Click += new EventHandler(this.btHideAll_Click);
      this.btShowAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btShowAll.Location = new System.Drawing.Point(398, 110);
      this.btShowAll.Name = "btShowAll";
      this.btShowAll.Size = new System.Drawing.Size(80, 24);
      this.btShowAll.TabIndex = 4;
      this.btShowAll.Text = "&Show All";
      this.btShowAll.UseVisualStyleBackColor = true;
      this.btShowAll.Click += new EventHandler(this.btShowAll_Click);
      this.labelHint.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.labelHint.Location = new System.Drawing.Point(6, 334);
      this.labelHint.Name = "labelHint";
      this.labelHint.Size = new System.Drawing.Size(469, 20);
      this.labelHint.TabIndex = 6;
      this.labelHint.Text = "Hint: You can also change the displayed columns by right clicking on a column header.";
      this.labelHint.TextAlign = ContentAlignment.MiddleCenter;
      this.btMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btMoveDown.Location = new System.Drawing.Point(398, 66);
      this.btMoveDown.Name = "btMoveDown";
      this.btMoveDown.Size = new System.Drawing.Size(80, 23);
      this.btMoveDown.TabIndex = 3;
      this.btMoveDown.Text = "Move &Down";
      this.btMoveDown.UseVisualStyleBackColor = true;
      this.btMoveDown.Click += new EventHandler(this.btMoveDown_Click);
      this.btMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btMoveUp.Location = new System.Drawing.Point(398, 37);
      this.btMoveUp.Name = "btMoveUp";
      this.btMoveUp.Size = new System.Drawing.Size(80, 23);
      this.btMoveUp.TabIndex = 2;
      this.btMoveUp.Text = "Move &Up";
      this.btMoveUp.UseVisualStyleBackColor = true;
      this.btMoveUp.Click += new EventHandler(this.btMoveUp_Click);
      this.tab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.tab.Controls.Add((Control) this.tabDetails);
      this.tab.Controls.Add((Control) this.tabThumbnails);
      this.tab.Controls.Add((Control) this.tabTiles);
      this.tab.Location = new System.Drawing.Point(12, 12);
      this.tab.Name = "tab";
      this.tab.SelectedIndex = 0;
      this.tab.Size = new System.Drawing.Size(492, 383);
      this.tab.TabIndex = 0;
      this.tabThumbnails.Controls.Add((Control) this.chkHideCaption);
      this.tabThumbnails.Controls.Add((Control) this.labelSelectThumbnailText);
      this.tabThumbnails.Controls.Add((Control) this.cbThirdLine);
      this.tabThumbnails.Controls.Add((Control) this.labelThirdLine);
      this.tabThumbnails.Controls.Add((Control) this.cbSecondLine);
      this.tabThumbnails.Controls.Add((Control) this.labelSecondLine);
      this.tabThumbnails.Controls.Add((Control) this.cbFirstLine);
      this.tabThumbnails.Controls.Add((Control) this.labelFirstLine);
      this.tabThumbnails.Location = new System.Drawing.Point(4, 22);
      this.tabThumbnails.Name = "tabThumbnails";
      this.tabThumbnails.Size = new System.Drawing.Size(484, 357);
      this.tabThumbnails.TabIndex = 1;
      this.tabThumbnails.Text = "Thumbnails";
      this.tabThumbnails.UseVisualStyleBackColor = true;
      this.chkHideCaption.AutoSize = true;
      this.chkHideCaption.Location = new System.Drawing.Point(186, 165);
      this.chkHideCaption.Name = "chkHideCaption";
      this.chkHideCaption.Size = new System.Drawing.Size(130, 17);
      this.chkHideCaption.TabIndex = 7;
      this.chkHideCaption.Text = "Do not show any Text";
      this.chkHideCaption.UseVisualStyleBackColor = true;
      this.labelSelectThumbnailText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.labelSelectThumbnailText.Location = new System.Drawing.Point(17, 20);
      this.labelSelectThumbnailText.Name = "labelSelectThumbnailText";
      this.labelSelectThumbnailText.Size = new System.Drawing.Size(445, 26);
      this.labelSelectThumbnailText.TabIndex = 6;
      this.labelSelectThumbnailText.Text = "Please select the text you want to display below the thumbnails:";
      this.cbThirdLine.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbThirdLine.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbThirdLine.FormattingEnabled = true;
      this.cbThirdLine.Location = new System.Drawing.Point(91, 114);
      this.cbThirdLine.Name = "cbThirdLine";
      this.cbThirdLine.Size = new System.Drawing.Size(371, 21);
      this.cbThirdLine.Sorted = true;
      this.cbThirdLine.TabIndex = 5;
      this.labelThirdLine.AutoSize = true;
      this.labelThirdLine.Location = new System.Drawing.Point(15, 117);
      this.labelThirdLine.Name = "labelThirdLine";
      this.labelThirdLine.Size = new System.Drawing.Size(57, 13);
      this.labelThirdLine.TabIndex = 4;
      this.labelThirdLine.Text = "Third Line:";
      this.cbSecondLine.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbSecondLine.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbSecondLine.FormattingEnabled = true;
      this.cbSecondLine.Location = new System.Drawing.Point(91, 87);
      this.cbSecondLine.Name = "cbSecondLine";
      this.cbSecondLine.Size = new System.Drawing.Size(371, 21);
      this.cbSecondLine.Sorted = true;
      this.cbSecondLine.TabIndex = 3;
      this.labelSecondLine.AutoSize = true;
      this.labelSecondLine.Location = new System.Drawing.Point(15, 90);
      this.labelSecondLine.Name = "labelSecondLine";
      this.labelSecondLine.Size = new System.Drawing.Size(70, 13);
      this.labelSecondLine.TabIndex = 2;
      this.labelSecondLine.Text = "Second Line:";
      this.cbFirstLine.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbFirstLine.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbFirstLine.FormattingEnabled = true;
      this.cbFirstLine.Location = new System.Drawing.Point(91, 60);
      this.cbFirstLine.Name = "cbFirstLine";
      this.cbFirstLine.Size = new System.Drawing.Size(371, 21);
      this.cbFirstLine.Sorted = true;
      this.cbFirstLine.TabIndex = 1;
      this.labelFirstLine.AutoSize = true;
      this.labelFirstLine.Location = new System.Drawing.Point(15, 63);
      this.labelFirstLine.Name = "labelFirstLine";
      this.labelFirstLine.Size = new System.Drawing.Size(52, 13);
      this.labelFirstLine.TabIndex = 0;
      this.labelFirstLine.Text = "First Line:";
      this.tabTiles.Controls.Add((Control) this.btTilesDefault);
      this.tabTiles.Controls.Add((Control) this.lbTileItems);
      this.tabTiles.Location = new System.Drawing.Point(4, 22);
      this.tabTiles.Name = "tabTiles";
      this.tabTiles.Padding = new Padding(3);
      this.tabTiles.Size = new System.Drawing.Size(484, 357);
      this.tabTiles.TabIndex = 2;
      this.tabTiles.Text = "Tiles";
      this.tabTiles.UseVisualStyleBackColor = true;
      this.btTilesDefault.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btTilesDefault.Location = new System.Drawing.Point(397, 13);
      this.btTilesDefault.Name = "btTilesDefault";
      this.btTilesDefault.Size = new System.Drawing.Size(80, 23);
      this.btTilesDefault.TabIndex = 3;
      this.btTilesDefault.Text = "Default";
      this.btTilesDefault.UseVisualStyleBackColor = true;
      this.btTilesDefault.Click += new EventHandler(this.btDefaultTile_Click);
      this.lbTileItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.lbTileItems.CheckOnClick = true;
      this.lbTileItems.FormattingEnabled = true;
      this.lbTileItems.IntegralHeight = false;
      this.lbTileItems.Location = new System.Drawing.Point(9, 13);
      this.lbTileItems.Name = "lbTileItems";
      this.lbTileItems.Size = new System.Drawing.Size(382, 334);
      this.lbTileItems.TabIndex = 0;
      this.btApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btApply.FlatStyle = FlatStyle.System;
      this.btApply.Location = new System.Drawing.Point(424, 401);
      this.btApply.Name = "btApply";
      this.btApply.Size = new System.Drawing.Size(80, 24);
      this.btApply.TabIndex = 3;
      this.btApply.Text = "&Apply";
      this.btApply.Click += new EventHandler(this.btApply_Click);
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(516, 432);
      this.Controls.Add((Control) this.btApply);
      this.Controls.Add((Control) this.tab);
      this.Controls.Add((Control) this.btOK);
      this.Controls.Add((Control) this.btCancel);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(355, 361);
      this.Name = nameof (ListLayoutDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "List Options";
      this.tabDetails.ResumeLayout(false);
      this.tabDetails.PerformLayout();
      this.tab.ResumeLayout(false);
      this.tabThumbnails.ResumeLayout(false);
      this.tabThumbnails.PerformLayout();
      this.tabTiles.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    public class TileTextItem
    {
      public int Value { get; set; }

      public string Text { get; set; }

      public override string ToString() => this.Text;
    }

    private class CaptionData
    {
      public CaptionData(int id, string text)
      {
        this.Id = id;
        this.Text = text;
      }

      public int Id { get; set; }

      public string Text { get; set; }

      public override string ToString() => this.Text;
    }
  }
}

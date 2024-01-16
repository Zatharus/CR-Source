// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicPagesView
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicPagesView : SubView, IDisplayWorkspace, IRefreshDisplay, IItemSize
  {
    private readonly string None;
    private readonly string ArrangedBy;
    private readonly string NotArranged;
    private readonly string GroupedBy;
    private readonly string NotGrouped;
    private readonly Image groupUp = (Image) Resources.GroupUp;
    private readonly Image groupDown = (Image) Resources.GroupDown;
    private readonly Image sortUp = (Image) Resources.SortUp;
    private readonly Image sortDown = (Image) Resources.SortDown;
    private readonly CommandMapper command = new CommandMapper();
    private EnumMenuUtility filterMenu;
    private IContainer components = (IContainer) new System.ComponentModel.Container();
    private ToolStrip toolStrip;
    private ToolStripSplitButton tbbView;
    private ToolStripMenuItem miViewThumbnails;
    private ToolStripMenuItem miViewTiles;
    private ToolStripMenuItem miViewDetails;
    private ToolStripSplitButton tbbSort;
    private ToolStripSplitButton tbbGroup;
    private ToolStripDropDownButton tbFilter;
    private PagesView pagesView;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem miExpandAllGroups;

    public ComicPagesView()
    {
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, this.components);
      this.components.Add((IComponent) this.command);
      this.None = TR.Load(this.Name)[nameof (None), nameof (None)];
      this.ArrangedBy = TR.Load(this.Name)[nameof (ArrangedBy), "Arranged by {0}"];
      this.NotArranged = TR.Load(this.Name)[nameof (NotArranged), "Not sorted"];
      this.GroupedBy = TR.Load(this.Name)[nameof (GroupedBy), "Grouped by {0}"];
      this.NotGrouped = TR.Load(this.Name)[nameof (NotGrouped), "Not grouped"];
      SubView.TranslateColumns((IEnumerable<IColumn>) this.pagesView.ItemView.Columns);
      foreach (ItemViewColumn column in (SmartList<IColumn>) this.pagesView.ItemView.Columns)
        column.TooltipText = ((ComicListField) column.Tag).Description;
      this.pagesView.ItemView.ItemActivate += new EventHandler(this.ItemView_ItemActivate);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicPageType PageFilter
    {
      get
      {
        try
        {
          return this.Main.ComicDisplay.PageFilter;
        }
        catch
        {
          return this.pagesView.PageFilter;
        }
      }
      set
      {
        try
        {
          if (this.Main.ComicDisplay.PageFilter == value)
            return;
          this.Main.ComicDisplay.PageFilter = this.pagesView.PageFilter = value;
        }
        catch
        {
        }
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemViewConfig ViewConfig
    {
      get => this.pagesView.ViewConfig;
      set => this.pagesView.ViewConfig = value;
    }

    protected override void OnMainFormChanged()
    {
      base.OnMainFormChanged();
      this.Main.ComicDisplay.BookChanged += new EventHandler(this.Viewer_BookChanged);
      this.filterMenu.Value = (int) this.PageFilter;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DesignMode)
        return;
      this.command.Add((CommandHandler) (() => this.pagesView.ItemView.ItemViewMode = ItemViewMode.Thumbnail), true, (UpdateHandler) (() => this.pagesView.ItemView.ItemViewMode == ItemViewMode.Thumbnail), (object) this.miViewThumbnails);
      this.command.Add((CommandHandler) (() => this.pagesView.ItemView.ItemViewMode = ItemViewMode.Tile), true, (UpdateHandler) (() => this.pagesView.ItemView.ItemViewMode == ItemViewMode.Tile), (object) this.miViewTiles);
      this.command.Add((CommandHandler) (() => this.pagesView.ItemView.ItemViewMode = ItemViewMode.Detail), true, (UpdateHandler) (() => this.pagesView.ItemView.ItemViewMode == ItemViewMode.Detail), (object) this.miViewDetails);
      this.command.Add(new CommandHandler(this.pagesView.ItemView.ToggleGroups), (UpdateHandler) (() => this.pagesView.ItemView.AreGroupsVisible), (object) this.miExpandAllGroups);
      this.filterMenu = new EnumMenuUtility((ToolStripDropDownItem) this.tbFilter, typeof (ComicPageType), true, (IDictionary<int, Image>) null, Keys.None);
      this.filterMenu.ValueChanged += new EventHandler(this.filterMenu_ValueChanged);
    }

    private void filterMenu_ValueChanged(object sender, EventArgs e)
    {
      this.PageFilter = (ComicPageType) this.filterMenu.Value;
    }

    private void Viewer_BookChanged(object sender, EventArgs e)
    {
      this.pagesView.PageFilter = this.Main.ComicDisplay.PageFilter;
      this.pagesView.Book = this.Main.ComicDisplay.Book;
    }

    private void ItemView_ItemActivate(object sender, EventArgs e)
    {
      if (!(this.pagesView.ItemView.FocusedItem is PageViewItem focusedItem))
        return;
      this.pagesView.Book.Navigate(focusedItem.Page, PageSeekOrigin.Absolute);
      this.Main.ShowComic();
    }

    protected override void OnIdle()
    {
      if (!this.Visible)
        return;
      ItemView itemView = this.pagesView.ItemView;
      this.tbbSort.Enabled = itemView.Columns.Count != 0;
      this.tbbSort.Text = itemView.SortColumn != null ? itemView.SortColumn.Text : this.None;
      this.tbbSort.ToolTipText = StringUtility.Format(this.ArrangedBy, (object) this.tbbSort.Text);
      this.tbbGroup.Enabled = itemView.Columns.Count != 0;
      this.tbbGroup.Text = itemView.GroupColumn != null ? itemView.GroupColumn.Text : this.None;
      this.tbbGroup.ToolTipText = StringUtility.Format(this.GroupedBy, (object) this.tbbGroup.Text);
      this.tbbSort.Image = itemView.ItemSortOrder == SortOrder.Ascending ? this.sortUp : this.sortDown;
      this.tbbGroup.Image = itemView.GroupSortingOrder == SortOrder.Ascending ? this.groupDown : this.groupUp;
    }

    private void tbbSort_DropDownOpening(object sender, EventArgs e)
    {
      FormUtility.SafeToolStripClear(this.tbbSort.DropDownItems);
      this.pagesView.ItemView.CreateArrangeMenu(this.tbbSort.DropDownItems);
    }

    private void tbbGroup_DropDownOpening(object sender, EventArgs e)
    {
      FormUtility.SafeToolStripClear(this.tbbGroup.DropDownItems);
      this.pagesView.ItemView.CreateGroupMenu(this.tbbGroup.DropDownItems);
    }

    private void tbbGroup_ButtonClick(object sender, EventArgs e)
    {
      this.pagesView.ItemView.GroupSortingOrder = ItemView.FlipSortOrder(this.pagesView.ItemView.GroupSortingOrder);
    }

    private void tbbSort_ButtonClick(object sender, EventArgs e)
    {
      this.pagesView.ItemView.ItemSortOrder = ItemView.FlipSortOrder(this.pagesView.ItemView.ItemSortOrder);
    }

    private void tbbView_ButtonClick(object sender, EventArgs e)
    {
      ItemViewMode itemViewMode = this.pagesView.ItemView.ItemViewMode;
      switch (itemViewMode)
      {
        case ItemViewMode.Thumbnail:
          itemViewMode = ItemViewMode.Tile;
          break;
        case ItemViewMode.Tile:
          itemViewMode = ItemViewMode.Detail;
          break;
        case ItemViewMode.Detail:
          itemViewMode = ItemViewMode.Thumbnail;
          break;
      }
      this.pagesView.ItemView.ItemViewMode = itemViewMode;
    }

    public virtual ItemSizeInfo GetItemSize()
    {
      switch (this.pagesView.ItemView.ItemViewMode)
      {
        case ItemViewMode.Thumbnail:
          return new ItemSizeInfo(FormUtility.ScaleDpiY(96), FormUtility.ScaleDpiY(512), this.pagesView.ItemView.ItemThumbSize.Height);
        case ItemViewMode.Tile:
          return new ItemSizeInfo(FormUtility.ScaleDpiY(64), FormUtility.ScaleDpiY(256), this.pagesView.ItemView.ItemTileSize.Height);
        case ItemViewMode.Detail:
          return new ItemSizeInfo(FormUtility.ScaleDpiY(12), FormUtility.ScaleDpiY(48), this.pagesView.ItemView.ItemRowHeight);
        default:
          return (ItemSizeInfo) null;
      }
    }

    public virtual void SetItemSize(int value) => this.pagesView.SetItemSize(value);

    public void SetWorkspace(DisplayWorkspace ws) => this.ViewConfig = ws.PagesViewConfig;

    public void StoreWorkspace(DisplayWorkspace ws)
    {
      ws.PagesViewConfig = this.ViewConfig;
      if (ws.PagesViewConfig == null)
        return;
      ws.PagesViewConfig.GroupsStatus = (ItemViewGroupsStatus) null;
    }

    public void RefreshDisplay() => this.pagesView.RefreshDisplay();

    private void InitializeComponent()
    {
      this.toolStrip = new ToolStrip();
      this.tbbView = new ToolStripSplitButton();
      this.miViewThumbnails = new ToolStripMenuItem();
      this.miViewTiles = new ToolStripMenuItem();
      this.miViewDetails = new ToolStripMenuItem();
      this.tbbGroup = new ToolStripSplitButton();
      this.tbbSort = new ToolStripSplitButton();
      this.tbFilter = new ToolStripDropDownButton();
      this.pagesView = new PagesView();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.miExpandAllGroups = new ToolStripMenuItem();
      this.toolStrip.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;
      this.toolStrip.Items.AddRange(new ToolStripItem[4]
      {
        (ToolStripItem) this.tbbView,
        (ToolStripItem) this.tbbGroup,
        (ToolStripItem) this.tbbSort,
        (ToolStripItem) this.tbFilter
      });
      this.toolStrip.Location = new System.Drawing.Point(0, 0);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(656, 25);
      this.toolStrip.TabIndex = 1;
      this.toolStrip.Text = "toolStrip1";
      this.tbbView.DropDownItems.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.miViewThumbnails,
        (ToolStripItem) this.miViewTiles,
        (ToolStripItem) this.miViewDetails,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.miExpandAllGroups
      });
      this.tbbView.Image = (Image) Resources.View;
      this.tbbView.ImageTransparentColor = Color.Magenta;
      this.tbbView.Name = "tbbView";
      this.tbbView.Size = new System.Drawing.Size(69, 22);
      this.tbbView.Text = "Views";
      this.tbbView.ToolTipText = "Change how Books are displayed";
      this.tbbView.ButtonClick += new EventHandler(this.tbbView_ButtonClick);
      this.miViewThumbnails.Image = (Image) Resources.ThumbView;
      this.miViewThumbnails.Name = "miViewThumbnails";
      this.miViewThumbnails.Size = new System.Drawing.Size(218, 22);
      this.miViewThumbnails.Text = "T&humbnails";
      this.miViewTiles.Image = (Image) Resources.TileView;
      this.miViewTiles.Name = "miViewTiles";
      this.miViewTiles.Size = new System.Drawing.Size(218, 22);
      this.miViewTiles.Text = "&Tiles";
      this.miViewDetails.Image = (Image) Resources.DetailView;
      this.miViewDetails.Name = "miViewDetails";
      this.miViewDetails.Size = new System.Drawing.Size(218, 22);
      this.miViewDetails.Text = "&Details";
      this.tbbGroup.Image = (Image) Resources.Group;
      this.tbbGroup.ImageTransparentColor = Color.Magenta;
      this.tbbGroup.Name = "tbbGroup";
      this.tbbGroup.Size = new System.Drawing.Size(72, 22);
      this.tbbGroup.Text = "Group";
      this.tbbGroup.ToolTipText = "Group Books by different criteria";
      this.tbbGroup.ButtonClick += new EventHandler(this.tbbGroup_ButtonClick);
      this.tbbGroup.DropDownOpening += new EventHandler(this.tbbGroup_DropDownOpening);
      this.tbbSort.Image = (Image) Resources.SortUp;
      this.tbbSort.ImageTransparentColor = Color.Magenta;
      this.tbbSort.Name = "tbbSort";
      this.tbbSort.Size = new System.Drawing.Size(81, 22);
      this.tbbSort.Text = "Arrange";
      this.tbbSort.ToolTipText = "Change the sort order of the Books";
      this.tbbSort.ButtonClick += new EventHandler(this.tbbSort_ButtonClick);
      this.tbbSort.DropDownOpening += new EventHandler(this.tbbSort_DropDownOpening);
      this.tbFilter.Alignment = ToolStripItemAlignment.Right;
      this.tbFilter.Image = (Image) Resources.Search;
      this.tbFilter.ImageTransparentColor = Color.Magenta;
      this.tbFilter.Name = "tbFilter";
      this.tbFilter.Size = new System.Drawing.Size(91, 22);
      this.tbFilter.Text = "Page Filter";
      this.pagesView.Bookmark = (string) null;
      this.pagesView.Dock = DockStyle.Fill;
      this.pagesView.Location = new System.Drawing.Point(0, 25);
      this.pagesView.Name = "pagesView";
      this.pagesView.Size = new System.Drawing.Size(656, 399);
      this.pagesView.TabIndex = 2;
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(215, 6);
      this.miExpandAllGroups.Name = "miExpandAllGroups";
      this.miExpandAllGroups.Size = new System.Drawing.Size(218, 22);
      this.miExpandAllGroups.Text = "Collapse/Expand all Groups";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.Controls.Add((Control) this.pagesView);
      this.Controls.Add((Control) this.toolStrip);
      this.Name = nameof (ComicPagesView);
      this.Size = new System.Drawing.Size(656, 424);
      this.toolStrip.ResumeLayout(false);
      this.toolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

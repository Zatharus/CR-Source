// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemView
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Threading;
using cYo.Common.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ItemView : ScrollControl
  {
    private const int maxPopupSize = 20;
    private const int longClickDelta = 2;
    private static readonly TR tr = TR.Load(typeof (ItemView).Name);
    private volatile bool pendingSelectedIndexChanged;
    private volatile bool positionsInvalidated;
    private volatile bool itemsResort;
    private readonly ItemView.StateInfo itemStates = new ItemView.StateInfo();
    private Dictionary<IViewableItem, ItemView.ItemInformation> itemInfos = new Dictionary<IViewableItem, ItemView.ItemInformation>();
    private Dictionary<IViewableItem, ItemView.StackInfo> stackInfo = new Dictionary<IViewableItem, ItemView.StackInfo>();
    private List<GroupHeaderInformation> displayedGroups = new List<GroupHeaderInformation>();
    private volatile bool multiselect = true;
    private volatile bool hideSelection = true;
    private volatile SortOrder itemSortOrder = SortOrder.Ascending;
    private volatile bool groupDisplayEnabled;
    private volatile SortOrder groupSortingOrder = SortOrder.Ascending;
    private bool stackDisplayEnabled;
    private HorizontalAlignment horizontalItemAlignment;
    private Bitmap groupExpandedImage;
    private Bitmap groupCollapsedImage;
    private bool showHeader = true;
    private int columnHeaderHeight = 20;
    private ContentAlignment backgroundImageAlignment = ContentAlignment.TopLeft;
    private volatile ItemViewLayout itemViewLayout;
    private volatile ItemViewMode itemViewMode;
    private volatile string expandedDetailColumnName;
    private volatile int expandedDetailColumnMinimumHeight = -160;
    private System.Drawing.Size itemPadding = new System.Drawing.Size(1, 1);
    private System.Drawing.Size itemThumbSize = new System.Drawing.Size(128, 128);
    private System.Drawing.Size itemTileSize = new System.Drawing.Size(192, 96);
    private volatile int itemRowHeight = 16;
    private volatile int groupHeaderHeight = 40;
    private volatile bool showGroupCount = true;
    private volatile IViewableItem markerItem;
    private bool markerVisible;
    private volatile bool groupHeaderTrueCount;
    private readonly ViewableItemCollection<IViewableItem> items = new ViewableItemCollection<IViewableItem>();
    private readonly ItemViewColumnCollection<IColumn> columns = new ItemViewColumnCollection<IColumn>();
    private readonly List<IComparer<IViewableItem>> itemSorters = new List<IComparer<IViewableItem>>((IEnumerable<IComparer<IViewableItem>>) new IComparer<IViewableItem>[1]);
    private volatile IGrouper<IViewableItem> itemGrouper;
    private volatile IGrouper<IViewableItem> itemStacker;
    private volatile IComparer<IViewableItem> itemStackSorter;
    private ItemViewGroupsStatus groupsStatus = new ItemViewGroupsStatus((IEnumerable<GroupHeaderInformation>) null);
    private List<IViewableItem> displayedItems = new List<IViewableItem>();
    private readonly List<IViewableItem> selectedItems = new List<IViewableItem>();
    private readonly List<IViewableItem> visibleItems = new List<IViewableItem>();
    private int currentFontHeight;
    private MouseButtons activateButton;
    private IViewableItem anchorItem;
    private int updateBlockCount;
    private volatile bool pendingUpdate;
    private const int ColumnOffsetX = 8;
    private const int markerWidth = 2;
    private IColumn resizeColumn;
    private int resizeColumnPos;
    private int resizeColumnWidth;
    private IColumn pressedHeader;
    private System.Drawing.Point pressedHeaderPoint;
    private IColumn hotHeader;
    private bool customClick;
    private IViewableItem clickItem;
    private IViewableItem dragItem;
    private System.Drawing.Point pressetViewPoint;
    private ItemView.StateInfo selectItemState;
    private IColumn dragHeader;
    private int longClickSubItem = -1;
    private System.Drawing.Point mouseDownPoint;
    private MouseButtons lastMouseButton;
    private IViewableItem longClickItem;
    private Rectangle selectionRect = Rectangle.Empty;
    private bool doubleGroupClick;
    private IViewableItem currentInplaceEditItem;
    private int currentInplaceEditSubItem;
    private Control currentInplaceEditControl;
    private IContainer components;
    private ToolStripMenuItem dummyItem;
    private ContextMenuStrip autoHeaderContextMenuStrip;
    private ContextMenuStrip autoViewContextMenuStrip;
    private ToolStripMenuItem miViewMode;
    private ToolStripMenuItem miViewThumbs;
    private ToolStripMenuItem miViewTiles;
    private ToolStripMenuItem miViewDetails;
    private ToolStripMenuItem miArrange;
    private ToolStripMenuItem miGroup;
    private Timer longClickTimer;
    private ToolStripMenuItem miStack;
    private ToolTip toolTip;

    public ItemView()
    {
      this.SelectionMode = SelectionMode.MultiSimple;
      this.ItemsOwned = true;
      this.AutomaticViewMenu = true;
      this.AutomaticHeaderMenu = true;
      this.HeaderToolTips = true;
      this.LabelEdit = true;
      this.InitializeComponent();
      LocalizeUtility.Localize(ItemView.tr, (ToolStrip) this.autoViewContextMenuStrip);
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.items.Changed += new EventHandler<SmartListChangedEventArgs<IViewableItem>>(this.ItemsChanged);
      this.columns.Changed += new EventHandler<SmartListChangedEventArgs<IColumn>>(this.HeadersChanged);
      this.itemStates.StateChanged += new EventHandler<ItemView.StateChangedEventArgs>(this.ItemStatesStateChanged);
      this.miViewMode.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.ViewDropDownItemClicked);
      this.currentFontHeight = this.Font.Height;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        try
        {
          this.items.Clear();
          this.columns.Clear();
        }
        catch (Exception ex)
        {
        }
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    [Category("Behavior")]
    [DefaultValue(true)]
    public bool LabelEdit { get; set; }

    [Category("Behavior")]
    [DefaultValue(true)]
    public bool HeaderToolTips { get; set; }

    [Category("Behavior")]
    [DefaultValue(true)]
    public bool AutomaticHeaderMenu { get; set; }

    [Category("Behavior")]
    [DefaultValue(null)]
    public ContextMenuStrip HeaderContextMenuStrip { get; set; }

    [Category("Behavior")]
    [DefaultValue(true)]
    public bool AutomaticViewMenu { get; set; }

    [Category("Behavior")]
    [DefaultValue(null)]
    public ContextMenuStrip ViewContextMenuStrip { get; set; }

    [Category("Behavior")]
    [DefaultValue(null)]
    public ContextMenuStrip ItemContextMenuStrip { get; set; }

    [Category("Behavior")]
    [DefaultValue(true)]
    public bool ItemsOwned { get; set; }

    [Category("Behavior")]
    [DefaultValue(true)]
    public bool Multiselect
    {
      get => this.multiselect;
      set
      {
        if (this.multiselect == value)
          return;
        this.multiselect = value;
        this.SafeInvalidate();
      }
    }

    [Category("Behavior")]
    [DefaultValue(true)]
    public bool HideSelection
    {
      get => this.hideSelection;
      set
      {
        if (this.hideSelection == value)
          return;
        this.hideSelection = value;
        this.SafeInvalidate();
      }
    }

    [Category("Behavior")]
    [DefaultValue(SortOrder.Ascending)]
    public SortOrder ItemSortOrder
    {
      get => this.itemSortOrder;
      set
      {
        if (value == this.itemSortOrder)
          return;
        this.itemSortOrder = value;
        if (this.ItemSorter == null)
          return;
        this.SafeInvalidate(ItemViewInvalidateOptions.Full);
      }
    }

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool GroupDisplayEnabled
    {
      get => this.groupDisplayEnabled;
      set
      {
        if (value == this.groupDisplayEnabled)
          return;
        this.groupDisplayEnabled = value;
        this.SafeInvalidate(ItemViewInvalidateOptions.Full);
        this.OnGroupDisplayChanged();
      }
    }

    [Category("Behavior")]
    [DefaultValue(SortOrder.Ascending)]
    public SortOrder GroupSortingOrder
    {
      get => this.groupSortingOrder;
      set
      {
        if (value == this.groupSortingOrder)
          return;
        this.groupSortingOrder = value;
        if (this.ItemGrouper != null)
          this.SafeInvalidate(ItemViewInvalidateOptions.Full);
        this.OnGroupDisplayChanged();
      }
    }

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool StackDisplayEnabled
    {
      get => this.stackDisplayEnabled;
      set
      {
        if (value == this.stackDisplayEnabled)
          return;
        this.stackDisplayEnabled = value;
        this.SafeInvalidate(ItemViewInvalidateOptions.Full);
      }
    }

    [Category("Behavior")]
    [DefaultValue(SelectionMode.MultiSimple)]
    public SelectionMode SelectionMode { get; set; }

    [Category("Appearance")]
    [DefaultValue(HorizontalAlignment.Left)]
    public HorizontalAlignment HorizontalItemAlignment
    {
      get => this.horizontalItemAlignment;
      set
      {
        if (this.horizontalItemAlignment == value)
          return;
        this.horizontalItemAlignment = value;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(null)]
    public Bitmap GroupExpandedImage
    {
      get => this.groupExpandedImage;
      set
      {
        if (this.groupExpandedImage == value)
          return;
        this.groupExpandedImage = value;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(null)]
    public Bitmap GroupCollapsedImage
    {
      get => this.groupCollapsedImage;
      set
      {
        if (this.groupCollapsedImage == value)
          return;
        this.groupCollapsedImage = value;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(true)]
    public bool ShowHeader
    {
      get => this.showHeader;
      set
      {
        if (this.showHeader == value)
          return;
        this.showHeader = value;
        if (this.ItemViewMode != ItemViewMode.Detail)
          return;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(20)]
    public int ColumnHeaderHeight
    {
      get => this.columnHeaderHeight;
      set
      {
        if (this.columnHeaderHeight == value)
          return;
        this.columnHeaderHeight = value;
        if (!this.IsHeaderVisible)
          return;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(ContentAlignment.TopLeft)]
    public ContentAlignment BackgroundImageAlignment
    {
      get => this.backgroundImageAlignment;
      set
      {
        if (this.backgroundImageAlignment == value)
          return;
        this.backgroundImageAlignment = value;
        if (this.BackgroundImage == null)
          return;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(ItemViewLayout.Top)]
    public ItemViewLayout ItemViewLayout
    {
      get => this.itemViewLayout;
      set
      {
        if (value == this.itemViewLayout)
          return;
        this.itemViewLayout = value;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(ItemViewMode.Thumbnail)]
    public ItemViewMode ItemViewMode
    {
      get => this.itemViewMode;
      set
      {
        if (value == this.itemViewMode)
          return;
        this.itemViewMode = value;
        IViewableItem focusedItem = this.FocusedItem;
        this.SafeInvalidate(ItemViewInvalidateOptions.Full);
        if (focusedItem != null)
          this.EnsureItemVisible(focusedItem);
        this.OnItemDisplayChanged();
      }
    }

    [Category("Appearance")]
    [DefaultValue(null)]
    public string ExpandedDetailColumnName
    {
      get => this.expandedDetailColumnName;
      set
      {
        if (value == this.expandedDetailColumnName)
          return;
        this.expandedDetailColumnName = value;
        if (this.GetExpandedColumn() == null)
          return;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(-160)]
    public int ExpandedDetailColumnMinimumHeight
    {
      get => this.expandedDetailColumnMinimumHeight;
      set
      {
        if (value == this.expandedDetailColumnMinimumHeight)
          return;
        this.expandedDetailColumnMinimumHeight = value;
        if (this.GetExpandedColumn() == null)
          return;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(typeof (System.Drawing.Size), "1, 1")]
    public System.Drawing.Size ItemPadding
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.itemPadding;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (value == this.itemPadding)
            return;
          this.itemPadding = value;
        }
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(typeof (System.Drawing.Size), "128, 128")]
    public System.Drawing.Size ItemThumbSize
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.itemThumbSize;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.itemThumbSize == value)
            return;
          this.itemThumbSize = value;
        }
        if (this.ItemViewMode != ItemViewMode.Thumbnail)
          return;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(typeof (System.Drawing.Size), "192, 96")]
    public System.Drawing.Size ItemTileSize
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.itemTileSize;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.itemTileSize == value)
            return;
          this.itemTileSize = value;
        }
        if (this.ItemViewMode != ItemViewMode.Tile)
          return;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(16)]
    public int ItemRowHeight
    {
      get => this.itemRowHeight;
      set
      {
        if (this.itemRowHeight == value)
          return;
        this.itemRowHeight = value;
        if (this.ItemViewMode != ItemViewMode.Detail)
          return;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(40)]
    public int GroupHeaderHeight
    {
      get => this.groupHeaderHeight;
      set
      {
        if (this.groupHeaderHeight == value)
          return;
        this.groupHeaderHeight = value;
        if (!this.AreGroupsVisible)
          return;
        this.SafeInvalidate();
      }
    }

    [Category("Appearance")]
    [DefaultValue(true)]
    public bool ShowGroupCount
    {
      get => this.showGroupCount;
      set
      {
        if (this.showGroupCount == value)
          return;
        this.showGroupCount = value;
        if (!this.AreGroupsVisible)
          return;
        this.SafeInvalidate();
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewableItem MarkerItem
    {
      get => this.markerItem;
      set
      {
        if (this.markerItem == value)
          return;
        this.InvalidateMarker();
        this.markerItem = value;
        this.InvalidateMarker();
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool MarkerVisible
    {
      get => this.markerVisible;
      set
      {
        if (this.markerVisible == value)
          return;
        this.InvalidateMarker();
        this.markerVisible = value;
        this.InvalidateMarker();
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool GroupHeaderTrueCount
    {
      get => this.groupHeaderTrueCount;
      set
      {
        if (this.groupHeaderTrueCount == value)
          return;
        this.groupHeaderTrueCount = value;
        if (!this.AreGroupsVisible)
          return;
        this.SafeInvalidate();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ViewableItemCollection<IViewableItem> Items => this.items;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemViewColumnCollection<IColumn> Columns => this.columns;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IComparer<IViewableItem> ItemSorter
    {
      get
      {
        using (ItemMonitor.Lock((object) this.itemSorters))
          return this.itemSorters[0];
      }
      set
      {
        using (ItemMonitor.Lock((object) this.itemSorters))
        {
          if (this.itemSorters[0] == value)
            return;
          this.itemSorters.Remove(value);
          this.itemSorters.Insert(0, value);
          this.itemSorters.Trim(3);
        }
        this.SafeInvalidate(ItemViewInvalidateOptions.Full);
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IGrouper<IViewableItem> ItemGrouper
    {
      get => this.itemGrouper;
      set
      {
        if (this.itemGrouper == value)
          return;
        this.itemGrouper = value;
        this.SafeInvalidate(ItemViewInvalidateOptions.Full);
        this.OnGroupDisplayChanged();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IGrouper<IViewableItem> ItemStacker
    {
      get => this.itemStacker;
      set
      {
        if (this.itemStacker == value)
          return;
        this.itemStacker = value;
        IViewableItem focusedItem = this.FocusedItem;
        this.SafeInvalidate(ItemViewInvalidateOptions.Full);
        if (focusedItem == null)
          return;
        this.EnsureItemVisible(focusedItem);
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IComparer<IViewableItem> ItemStackSorter
    {
      get => this.itemStackSorter;
      set
      {
        if (this.itemStackSorter == value)
          return;
        this.itemStackSorter = value;
        if (!this.IsStacked)
          return;
        this.SafeInvalidate(ItemViewInvalidateOptions.Full);
      }
    }

    [Browsable(false)]
    public ItemViewGroupsStatus GroupsStatus
    {
      get => this.groupsStatus;
      set
      {
        if (this.groupsStatus == value)
          return;
        this.groupsStatus = value;
        if (!this.AreGroupsVisible)
          return;
        this.SafeInvalidate();
      }
    }

    [Browsable(false)]
    public bool IsHeaderVisible
    {
      get => this.showHeader && this.ItemViewMode == ItemViewMode.Detail && this.IsTopLayout;
    }

    [Browsable(false)]
    public bool AreGroupsVisible
    {
      get => this.IsTopLayout && this.GroupDisplayEnabled && this.ItemGrouper != null;
    }

    [Browsable(false)]
    public bool IsStacked
    {
      get
      {
        return this.ItemViewMode != ItemViewMode.Detail && this.ItemStacker != null && this.StackDisplayEnabled;
      }
    }

    [Browsable(false)]
    public bool IsTopLayout
    {
      get => this.ItemViewLayout == ItemViewLayout.Top || this.ItemViewMode == ItemViewMode.Detail;
    }

    [Browsable(false)]
    public IEnumerable<IViewableItem> DisplayedItems
    {
      get
      {
        this.UpdatePositions((Graphics) null);
        return this.displayedItems.Lock<IViewableItem>();
      }
    }

    public IEnumerable<string> DisplayedGroups
    {
      get
      {
        if (this.AreGroupsVisible)
        {
          foreach (GroupHeaderInformation headerInformation in this.displayedGroups.Lock<GroupHeaderInformation>())
            yield return headerInformation.Caption;
        }
      }
    }

    public int GetGroupSizeFromCaption(string caption)
    {
      GroupHeaderInformation headerInformation = this.displayedGroups.Lock<GroupHeaderInformation>().FirstOrDefault<GroupHeaderInformation>((Func<GroupHeaderInformation, bool>) (g => g.Caption == caption));
      return headerInformation == null ? 0 : headerInformation.ItemCount;
    }

    [Browsable(false)]
    public int SelectedCount
    {
      get
      {
        using (ItemMonitor.Lock((object) this.selectedItems))
          return this.IsStacked ? this.selectedItems.Sum<IViewableItem>((Func<IViewableItem, int>) (si => this.GetStackCount(si))) : this.selectedItems.Count;
      }
    }

    [Browsable(false)]
    public IEnumerable<IViewableItem> SelectedItems
    {
      get
      {
        using (ItemMonitor.Lock((object) this.selectedItems))
        {
          foreach (IViewableItem item in this.selectedItems)
          {
            if (this.IsStack(item))
            {
              IViewableItem[] viewableItemArray = this.GetStackItems(item);
              for (int index = 0; index < viewableItemArray.Length; ++index)
                yield return viewableItemArray[index];
              viewableItemArray = (IViewableItem[]) null;
            }
            else
              yield return item;
          }
        }
      }
    }

    [Browsable(false)]
    public ReadOnlyCollection<IViewableItem> VisibleItems
    {
      get
      {
        using (ItemMonitor.Lock((object) this.visibleItems))
          return this.visibleItems.AsReadOnly();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewableItem FocusedItem
    {
      get => this.itemStates.FindFirst(ItemViewStates.Focused);
      set
      {
        if (value == this.FocusedItem)
          return;
        this.SetItemState(this.FocusedItem, ItemViewStates.None);
        if (value == null)
          return;
        this.SetItemState(value, this.GetItemState(value) | ItemViewStates.Focused);
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FocusedItemIndex
    {
      get => this.items.IndexOf(this.FocusedItem);
      set
      {
        try
        {
          IViewableItem viewableItem;
          using (ItemMonitor.Lock((object) this.items))
            viewableItem = this.items[value];
          this.FocusedItem = viewableItem;
        }
        catch
        {
          this.FocusedItem = (IViewableItem) null;
        }
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FocusedItemDisplayIndex
    {
      get
      {
        using (ItemMonitor.Lock((object) this.displayedItems))
          return this.displayedItems.IndexOf(this.FocusedItem);
      }
    }

    public IColumn SortColumn
    {
      get => this.ItemSorter == null ? (IColumn) null : this.Columns.FindBySorter(this.ItemSorter);
      set => this.ItemSorter = value?.ColumnSorter;
    }

    public IColumn[] SortColumns
    {
      get
      {
        return this.itemSorters.Lock<IComparer<IViewableItem>>().Select<IComparer<IViewableItem>, IColumn>((Func<IComparer<IViewableItem>, IColumn>) (comp => this.Columns.FindBySorter(comp))).TakeWhile<IColumn>((Func<IColumn, bool>) (ic => ic != null)).ToArray<IColumn>();
      }
      set
      {
        using (ItemMonitor.Lock((object) this.itemSorters))
        {
          this.ItemSorter = (IComparer<IViewableItem>) null;
          for (int index = value.Length - 1; index >= 0; --index)
            this.ItemSorter = value[index].ColumnSorter;
        }
      }
    }

    public string CovnertColumnsToKey(IEnumerable<IColumn> cols)
    {
      if (cols == null || !cols.Any<IColumn>())
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (IColumn col in cols)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append(",");
        stringBuilder.Append(col.Id);
      }
      return stringBuilder.ToString();
    }

    public IEnumerable<IColumn> ConvertKeyToColumns(string key)
    {
      if (!string.IsNullOrEmpty(key))
      {
        string[] strArray = key.Split(',');
        for (int index = 0; index < strArray.Length; ++index)
        {
          int result;
          if (int.TryParse(strArray[index], out result))
          {
            IColumn byId = this.columns.FindById(result);
            if (byId != null)
              yield return byId;
          }
        }
        strArray = (string[]) null;
      }
    }

    public string SortColumnsKey
    {
      get => this.CovnertColumnsToKey((IEnumerable<IColumn>) this.SortColumns);
      set => this.SortColumns = this.ConvertKeyToColumns(value).ToArray<IColumn>();
    }

    public IColumn[] GroupColumns
    {
      get
      {
        return this.ItemGrouper.GetGroupers<IViewableItem>().Select<IGrouper<IViewableItem>, IColumn>((Func<IGrouper<IViewableItem>, IColumn>) (comp => this.Columns.FindByGrouper(comp))).TakeWhile<IColumn>((Func<IColumn, bool>) (ic => ic != null)).ToArray<IColumn>();
      }
      set
      {
        if (value == null || value.Length == 0)
          this.ItemGrouper = (IGrouper<IViewableItem>) null;
        else if (value.Length == 1)
          this.ItemGrouper = value[0].ColumnGrouper;
        else
          this.ItemGrouper = (IGrouper<IViewableItem>) new CompoundSingleGrouper<IViewableItem>(((IEnumerable<IColumn>) value).Select<IColumn, IGrouper<IViewableItem>>((Func<IColumn, IGrouper<IViewableItem>>) (c => c.ColumnGrouper)).ToArray<IGrouper<IViewableItem>>());
      }
    }

    public string GroupColumnsKey
    {
      get => this.CovnertColumnsToKey((IEnumerable<IColumn>) this.GroupColumns);
      set => this.GroupColumns = this.ConvertKeyToColumns(value).ToArray<IColumn>();
    }

    public IColumn GroupColumn
    {
      get
      {
        IGrouper<IViewableItem> grouper = this.ItemGrouper is CompoundSingleGrouper<IViewableItem> ? ((IEnumerable<IGrouper<IViewableItem>>) ((CompoundSingleGrouper<IViewableItem>) this.ItemGrouper).Groupers).FirstOrDefault<IGrouper<IViewableItem>>() : this.ItemGrouper;
        return grouper != null ? this.Columns.FirstOrDefault<IColumn>((Func<IColumn, bool>) (h => h.ColumnGrouper == grouper)) : (IColumn) null;
      }
    }

    public IColumn StackColumn
    {
      get
      {
        IGrouper<IViewableItem> stacker = this.ItemStacker is CompoundSingleGrouper<IViewableItem> ? ((IEnumerable<IGrouper<IViewableItem>>) ((CompoundSingleGrouper<IViewableItem>) this.ItemStacker).Groupers).FirstOrDefault<IGrouper<IViewableItem>>() : this.ItemStacker;
        return this.ItemStacker != null ? this.Columns.FirstOrDefault<IColumn>((Func<IColumn, bool>) (h => h.ColumnGrouper == stacker)) : (IColumn) null;
      }
    }

    public IColumn[] StackColumns
    {
      get
      {
        return this.ItemStacker.GetGroupers<IViewableItem>().Select<IGrouper<IViewableItem>, IColumn>((Func<IGrouper<IViewableItem>, IColumn>) (comp => this.Columns.FindByGrouper(comp))).TakeWhile<IColumn>((Func<IColumn, bool>) (ic => ic != null)).ToArray<IColumn>();
      }
      set
      {
        if (value == null || value.Length == 0)
          this.ItemStacker = (IGrouper<IViewableItem>) null;
        else if (value.Length == 1)
          this.ItemStacker = value[0].ColumnGrouper;
        else
          this.ItemStacker = (IGrouper<IViewableItem>) new CompoundSingleGrouper<IViewableItem>(((IEnumerable<IColumn>) value).Select<IColumn, IGrouper<IViewableItem>>((Func<IColumn, IGrouper<IViewableItem>>) (c => c.ColumnGrouper)).ToArray<IGrouper<IViewableItem>>());
      }
    }

    public string StackColumnsKey
    {
      get => this.CovnertColumnsToKey((IEnumerable<IColumn>) this.StackColumns);
      set => this.StackColumns = this.ConvertKeyToColumns(value).ToArray<IColumn>();
    }

    public IColumn StackSorterColum
    {
      get
      {
        return this.ItemStackSorter != null ? this.Columns.Find((Predicate<IColumn>) (h => h.ColumnSorter == this.ItemStackSorter)) : (IColumn) null;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    private IEnumerable<ItemViewColumnInfo> ColumnHeaderConfiguration
    {
      get
      {
        return this.Columns.Select<IColumn, ItemViewColumnInfo>((Func<IColumn, ItemViewColumnInfo>) (ivch => new ItemViewColumnInfo(ivch)));
      }
      set
      {
        int index = 0;
        foreach (ItemViewColumnInfo itemViewColumnInfo in value)
        {
          IColumn byId = this.Columns.FindById(itemViewColumnInfo.Id);
          if (byId != null)
          {
            byId.FormatId = itemViewColumnInfo.FormatId;
            byId.Visible = itemViewColumnInfo.Visible;
            byId.Width = itemViewColumnInfo.Width;
            byId.LastTimeVisible = byId.Visible ? DateTime.UtcNow : itemViewColumnInfo.LastTimeVisible;
            this.columns.Remove(byId);
            this.columns.Insert(index, byId);
            ++index;
          }
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public ItemViewConfig ViewConfig
    {
      get
      {
        return new ItemViewConfig()
        {
          Columns = this.ColumnHeaderConfiguration.ToList<ItemViewColumnInfo>(),
          Grouping = this.GroupDisplayEnabled,
          ItemSortOrder = this.ItemSortOrder,
          GroupSortOrder = this.GroupSortingOrder,
          GroupsStatus = this.GroupsStatus,
          SortKey = this.SortColumnsKey,
          GrouperId = this.GroupColumnsKey,
          StackerId = this.StackColumnsKey,
          ItemViewMode = this.ItemViewMode,
          ThumbnailSize = this.ItemThumbSize,
          TileSize = this.ItemTileSize,
          ItemRowHeight = this.ItemRowHeight
        };
      }
      set
      {
        ItemViewConfig itemViewConfig = value;
        if (itemViewConfig == null)
          return;
        this.ColumnHeaderConfiguration = (IEnumerable<ItemViewColumnInfo>) itemViewConfig.Columns;
        this.GroupDisplayEnabled = itemViewConfig.Grouping;
        this.ItemSortOrder = itemViewConfig.ItemSortOrder;
        this.GroupSortingOrder = itemViewConfig.GroupSortOrder;
        this.SortColumnsKey = itemViewConfig.SortKey;
        this.GroupColumnsKey = itemViewConfig.GrouperId;
        this.StackColumnsKey = this.StackDisplayEnabled ? itemViewConfig.StackerId : (string) null;
        this.ItemStackSorter = this.StackColumn == null ? (IComparer<IViewableItem>) null : this.StackColumn.ColumnSorter;
        this.ItemViewMode = itemViewConfig.ItemViewMode;
        this.GroupsStatus = itemViewConfig.GroupsStatus ?? new ItemViewGroupsStatus();
        System.Drawing.Size size = itemViewConfig.ThumbnailSize;
        if (size.Height >= 16)
        {
          size = itemViewConfig.ThumbnailSize;
          if (size.Width >= 16)
            this.ItemThumbSize = itemViewConfig.ThumbnailSize;
        }
        size = itemViewConfig.TileSize;
        if (size.Width >= 16)
        {
          size = itemViewConfig.TileSize;
          if (size.Height >= 16)
            this.ItemTileSize = itemViewConfig.TileSize;
        }
        if (itemViewConfig.ItemRowHeight < 8)
          return;
        this.ItemRowHeight = itemViewConfig.ItemRowHeight;
      }
    }

    [Browsable(false)]
    public int CurrentFontHeight => this.currentFontHeight;

    [Browsable(false)]
    public MouseButtons ActivateButton => this.activateButton;

    public void ResetMouse() => this.ResetMouseEventArgs();

    public ItemViewStates GetItemState(IViewableItem item)
    {
      return item == null ? ItemViewStates.None : this.itemStates[item];
    }

    public bool IsItemSelected(IViewableItem item)
    {
      return (this.GetItemState(item) & ItemViewStates.Selected) != 0;
    }

    public bool IsItemFocused(IViewableItem item)
    {
      return (this.GetItemState(item) & ItemViewStates.Focused) != 0;
    }

    public void SetItemState(IViewableItem item, ItemViewStates state, bool multiSelect)
    {
      if (item == null)
        return;
      ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
      if ((state & ItemViewStates.Selected) != ItemViewStates.None && !multiSelect)
        si.Clear(ItemViewStates.Selected);
      si[item] = state;
      if ((state & ItemViewStates.Focused) != ItemViewStates.None || !multiSelect)
        si.Focus(item);
      this.itemStates.Update(si);
    }

    public void SetItemState(IViewableItem item, ItemViewStates state)
    {
      this.SetItemState(item, state, this.Multiselect);
    }

    public void SelectAll(bool selectionState)
    {
      if (!this.Multiselect)
        return;
      ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
      foreach (IViewableItem viewableItem in this.displayedItems.Lock<IViewableItem>())
        si.Set(viewableItem, ItemViewStates.Selected, selectionState);
      this.itemStates.Update(si);
    }

    public void SelectAll() => this.SelectAll(true);

    public void SelectNone() => this.SelectAll(false);

    public void Select(IViewableItem item, bool selectionState)
    {
      ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
      si.Set(item, ItemViewStates.Selected, selectionState);
      this.itemStates.Update(si);
    }

    public void Select(IViewableItem item) => this.Select(item, true);

    public void InvertSelection()
    {
      if (!this.Multiselect)
        return;
      ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
      foreach (IViewableItem viewableItem in this.displayedItems.Lock<IViewableItem>())
        si.Flip(viewableItem, ItemViewStates.Selected);
      this.itemStates.Update(si);
    }

    private void SelectFromAnchorItem(
      ItemView.StateInfo si,
      IViewableItem item,
      bool overideAnchor = false)
    {
      using (ItemMonitor.Lock((object) this.displayedItems))
      {
        if (this.displayedItems.Count == 0)
          return;
        if (overideAnchor)
          this.anchorItem = (IViewableItem) null;
        int a = this.displayedItems.IndexOf(this.anchorItem);
        int b = this.displayedItems.IndexOf(item);
        if (b == -1)
          return;
        if (a == -1)
        {
          a = this.displayedItems.IndexOf(this.FocusedItem);
          if (a == -1)
            a = 0;
          this.anchorItem = this.displayedItems[a];
        }
        if (a > b)
          CloneUtility.Swap<int>(ref a, ref b);
        for (int index = a; index <= b; ++index)
          si.Set(this.displayedItems[index], ItemViewStates.Selected, true);
      }
    }

    private void UpdateHotItemState(MouseButtons button, int x, int y)
    {
      ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
      si.Clear(ItemViewStates.Hot);
      IViewableItem viewableItem = this.ItemHitTest(x, y);
      if (viewableItem != null && button == MouseButtons.None)
        si.Set(viewableItem, ItemViewStates.Hot, true);
      this.itemStates.Update(si);
    }

    public void InvokeActivate()
    {
      if (this.FocusedItem == null)
        return;
      this.OnItemActivate();
    }

    protected bool InUpdateBlock => this.updateBlockCount > 0;

    public void BeginUpdate()
    {
      using (ItemMonitor.Lock((object) this))
        ++this.updateBlockCount;
    }

    public void EndUpdate()
    {
      using (ItemMonitor.Lock((object) this))
      {
        if (this.updateBlockCount > 0)
          --this.updateBlockCount;
      }
      if (!this.pendingUpdate)
        return;
      this.pendingUpdate = false;
      if (!this.InvokeRequired)
        this.UpdateItems();
      this.SafeInvalidate();
    }

    private void SafeInvalidate(ItemViewInvalidateOptions options = ItemViewInvalidateOptions.Position, Rectangle bounds = default (Rectangle))
    {
      this.itemsResort |= options == ItemViewInvalidateOptions.Full;
      this.positionsInvalidated |= options != 0;
      if (this.InUpdateBlock)
        this.pendingUpdate = true;
      else if (bounds.IsEmpty)
        this.Invalidate();
      else
        this.Invalidate(bounds);
    }

    public void UpdateItem(IViewableItem item, bool sizeChanged)
    {
      this.InvalidateItem(item, sizeChanged);
    }

    public void UpdateItems() => this.UpdatePositions((Graphics) null);

    public void EnsureGroupVisible(string caption)
    {
      if (!this.AreGroupsVisible)
        return;
      GroupHeaderInformation headerInformation = this.displayedGroups.Lock<GroupHeaderInformation>().FirstOrDefault<GroupHeaderInformation>((Func<GroupHeaderInformation, bool>) (h => h.Caption == caption));
      if (headerInformation == null)
        return;
      this.UpdateItems();
      this.ScrollPosition = new System.Drawing.Point(0, headerInformation.Bounds.Top);
    }

    public void EnsureItemVisible(IViewableItem item, int subItem = -1)
    {
      if (item == null)
        return;
      this.UpdateItems();
      Rectangle rectangle = this.Translate(this.ViewRectangle, true);
      Rectangle itemBounds = this.GetItemBounds(item, subItem);
      if (itemBounds.IsEmpty)
        return;
      itemBounds.Inflate(this.GetItemBorderSize());
      if (rectangle.Contains(itemBounds))
        return;
      System.Drawing.Point scrollPosition = this.ScrollPosition;
      if (itemBounds.Bottom > rectangle.Bottom)
        scrollPosition.Y += itemBounds.Bottom - rectangle.Bottom;
      else if (itemBounds.Top < rectangle.Top)
        scrollPosition.Y -= rectangle.Top - itemBounds.Top;
      if (this.ItemViewMode != ItemViewMode.Detail || subItem != -1)
      {
        if (itemBounds.Right > rectangle.Right)
          scrollPosition.X += itemBounds.Right - rectangle.Right;
        else if (itemBounds.Left < rectangle.Left)
          scrollPosition.X -= rectangle.Left - itemBounds.Left;
      }
      this.ScrollPosition = scrollPosition;
    }

    public void ScrollView(float linesOrColumns)
    {
      System.Drawing.Point scrollPosition = this.ScrollPosition;
      if (this.IsTopLayout)
        scrollPosition.Offset(0, (int) ((double) this.LineHeight * (double) linesOrColumns));
      else
        scrollPosition.Offset((int) ((double) this.ColumnWidth * (double) linesOrColumns), 0);
      this.ScrollPosition = scrollPosition;
    }

    public int GetAutoHeaderSize(IColumn header)
    {
      int autoHeaderSize = 0;
      int num = 0;
      ItemSizeInformation sizeInfo = new ItemSizeInformation();
      using (sizeInfo.Graphics = this.CreateGraphics())
      {
        sizeInfo.Header = header;
        sizeInfo.SubItem = this.Columns.IndexOf(header);
        foreach (IViewableItem viewableItem in this.displayedItems.Lock<IViewableItem>())
        {
          sizeInfo.Item = num;
          sizeInfo.Width = header.Width;
          viewableItem.OnMeasure(sizeInfo);
          Rectangle bounds = sizeInfo.Bounds;
          if (bounds.Width > autoHeaderSize)
          {
            bounds = sizeInfo.Bounds;
            autoHeaderSize = bounds.Width;
          }
          ++num;
        }
      }
      return autoHeaderSize;
    }

    public void AutoSizeHeader(IColumn header) => header.Width = this.GetAutoHeaderSize(header);

    public void AutoSizeHeaders(bool all)
    {
      foreach (IColumn column in (SmartList<IColumn>) this.Columns)
      {
        if (all || column.Visible)
          this.AutoSizeHeader(column);
      }
    }

    public void AutoFitHeaders(bool withAutosize)
    {
      if (withAutosize)
        this.AutoSizeHeaders(false);
      int columnHeadersWidth = this.GetColumnHeadersWidth();
      if (columnHeadersWidth == 0)
        return;
      float num = (float) this.DisplayRectangle.Width / (float) (columnHeadersWidth + 8);
      foreach (IColumn column in (SmartList<IColumn>) this.Columns)
        column.Width = (int) ((double) column.Width * (double) num);
    }

    public Bitmap GetDisplayBitmap(DrawItemViewOptions flags)
    {
      try
      {
        System.Drawing.Size size = this.ClientRectangle.Size;
        Bitmap displayBitmap = new Bitmap(size.Width, size.Height);
        using (Graphics gr = Graphics.FromImage((Image) displayBitmap))
          this.DrawItems(gr, flags);
        return displayBitmap;
      }
      catch
      {
        return (Bitmap) null;
      }
    }

    public IBitmapCursor GetDragCursor(float alpha)
    {
      Bitmap displayBitmap = this.GetDisplayBitmap(DrawItemViewOptions.SelectedOnly);
      IBitmapCursor dragCursor = BitmapCursor.Create(displayBitmap, this.PointToClient(Cursor.Position));
      if (dragCursor == null)
      {
        displayBitmap.Dispose();
      }
      else
      {
        dragCursor.BitmapOwned = true;
        if (dragCursor.Bitmap != null)
          dragCursor.Bitmap.ChangeAlpha(alpha);
      }
      return dragCursor;
    }

    public void ExpandGroups(bool expand, string caption = null)
    {
      if (!this.AreGroupsVisible)
        return;
      bool flag = false;
      foreach (GroupHeaderInformation headerInformation in this.displayedGroups.Lock<GroupHeaderInformation>())
      {
        if (headerInformation.Collapsed != !expand && (string.IsNullOrEmpty(caption) || headerInformation.Caption == caption))
        {
          headerInformation.Collapsed = !expand;
          flag = true;
        }
      }
      if (!flag)
        return;
      this.SafeInvalidate();
    }

    public void ToggleGroups()
    {
      try
      {
        this.ExpandGroups(this.displayedGroups.Lock<GroupHeaderInformation>().FirstOrDefault<GroupHeaderInformation>().Collapsed);
      }
      catch
      {
      }
    }

    public event EventHandler<ItemView.StackEventArgs> ProcessStack;

    public event EventHandler ItemActivate;

    public event EventHandler SelectedIndexChanged;

    public event EventHandler ItemDisplayChanged;

    public event EventHandler GroupDisplayChanged;

    public event EventHandler<ItemViewColumnHeaderClickEventArgs> HeaderClick;

    public event ItemDragEventHandler ItemDrag;

    public event PaintEventHandler PostPaint;

    protected virtual void OnItemDrag(ItemDragEventArgs itemDragEventArgs)
    {
      if (this.ItemDrag == null)
        return;
      this.ItemDrag((object) this, itemDragEventArgs);
    }

    protected virtual void OnItemActivate()
    {
      if (this.ItemActivate == null)
        return;
      this.ItemActivate((object) this, EventArgs.Empty);
    }

    protected virtual void OnSelectedIndexChanged()
    {
      if (this.SelectedIndexChanged == null)
        return;
      this.SelectedIndexChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnItemDisplayChanged()
    {
      if (this.ItemDisplayChanged == null)
        return;
      this.ItemDisplayChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnGroupDisplayChanged()
    {
      if (this.GroupDisplayChanged == null)
        return;
      this.GroupDisplayChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnHeaderClick(IColumn header)
    {
      if (this.HeaderClick != null)
        this.HeaderClick((object) this, new ItemViewColumnHeaderClickEventArgs(header));
      if (header.FormatTexts.Length != 0)
      {
        Rectangle dropDownBounds = HeaderControl.GetDropDownBounds(this.GetColumnHeaderRectangle(header));
        dropDownBounds.Offset(-this.ScrollPositionX, 0);
        if (dropDownBounds.Contains(this.pressedHeaderPoint))
        {
          ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
          for (int index = 0; index < header.FormatTexts.Length; ++index)
          {
            int n = index;
            contextMenuStrip.Items.Add((ToolStripItem) new ToolStripMenuItem(header.FormatTexts[index], (Image) null, (EventHandler) ((s, e) =>
            {
              if (header.FormatId == n)
                return;
              header.FormatId = n;
              this.Invalidate();
            }))
            {
              Checked = (header.FormatId == index)
            });
          }
          contextMenuStrip.Show((Control) this, dropDownBounds.BottomRight(), ToolStripDropDownDirection.BelowLeft);
          return;
        }
      }
      if (header.ColumnSorter == null)
        return;
      if (this.ItemSorter == header.ColumnSorter)
      {
        this.ItemSortOrder = ItemView.FlipSortOrder(this.ItemSortOrder);
      }
      else
      {
        this.ItemSortOrder = SortOrder.Ascending;
        this.ItemSorter = header.ColumnSorter;
      }
    }

    protected virtual void OnProcessStack(ItemView.StackInfo lsi)
    {
      if (this.ProcessStack == null)
        return;
      this.ProcessStack((object) this, new ItemView.StackEventArgs(lsi));
    }

    protected override void OnFontChanged(EventArgs e)
    {
      base.OnFontChanged(e);
      this.currentFontHeight = this.Font.Height;
    }

    public override Rectangle ViewRectangle
    {
      get
      {
        Rectangle viewRectangle = base.ViewRectangle;
        if (this.IsHeaderVisible)
        {
          viewRectangle.Y += this.columnHeaderHeight;
          viewRectangle.Height -= this.columnHeaderHeight;
          if (viewRectangle.Height < 0)
            viewRectangle.Height = 0;
        }
        return viewRectangle;
      }
    }

    protected override void OnScroll()
    {
      base.OnScroll();
      System.Drawing.Point client = this.PointToClient(Cursor.Position);
      this.UpdateHotItemState(Control.MouseButtons, client.X, client.Y);
      this.UpdateSelection(client.X, client.Y);
      this.ExitEdit(this.currentInplaceEditControl);
    }

    private void ItemsChanged(object sender, SmartListChangedEventArgs<IViewableItem> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          e.Item.View = this;
          break;
        case SmartListAction.Remove:
          e.Item.View = (ItemView) null;
          using (ItemMonitor.Lock((object) this.itemStates))
            this.itemStates.Set(e.Item, ItemViewStates.All, false);
          if (this.ItemsOwned && e.Item is IDisposable disposable)
          {
            disposable.Dispose();
            break;
          }
          break;
      }
      this.SafeInvalidate(ItemViewInvalidateOptions.Full);
    }

    private void HeadersChanged(object sender, SmartListChangedEventArgs<IColumn> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          e.Item.View = this;
          e.Item.PropertyChanged += new PropertyChangedEventHandler(this.HeaderPropertyChanged);
          break;
        case SmartListAction.Remove:
          e.Item.View = (ItemView) null;
          e.Item.PropertyChanged -= new PropertyChangedEventHandler(this.HeaderPropertyChanged);
          break;
      }
      if (this.ItemViewMode != ItemViewMode.Detail)
        return;
      this.SafeInvalidate();
    }

    private void HeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.ItemViewMode != ItemViewMode.Detail)
        return;
      this.SafeInvalidate();
    }

    private void ItemStatesStateChanged(object sender, ItemView.StateChangedEventArgs e)
    {
      this.InvalidateItem(e.Item, false);
      if (this.pendingSelectedIndexChanged || (e.NewState & ItemViewStates.Selected) == (e.OldState & ItemViewStates.Selected))
        return;
      this.pendingSelectedIndexChanged = true;
    }

    private int GetColumnHeadersWidth()
    {
      return this.columns.Where<IColumn>((Func<IColumn, bool>) (ivch => ivch.Visible)).Sum<IColumn>((Func<IColumn, int>) (ivch => ivch.Width));
    }

    public IColumn GetExpandedColumn()
    {
      if (this.ItemViewMode != ItemViewMode.Detail || string.IsNullOrEmpty(this.ExpandedDetailColumnName) || !this.AreGroupsVisible)
        return (IColumn) null;
      IColumn expandedColumn = this.columns.FirstOrDefault<IColumn>((Func<IColumn, bool>) (ch => ch.Visible));
      if (expandedColumn != null && expandedColumn.Name == this.ExpandedDetailColumnName)
        return expandedColumn;
      IColumn column = this.columns.LastOrDefault<IColumn>((Func<IColumn, bool>) (ch => ch.Visible));
      return column != null && column.Name == this.ExpandedDetailColumnName ? column : (IColumn) null;
    }

    public int GetExpandedColumnMinimumHeight(int width)
    {
      return this.expandedDetailColumnMinimumHeight < 0 ? width * -this.ExpandedDetailColumnMinimumHeight / 100 : this.ExpandedDetailColumnMinimumHeight;
    }

    private Rectangle GetColumnHeaderRectangle(IColumn header)
    {
      if (header.Visible)
      {
        int num = 8;
        foreach (IColumn column in (SmartList<IColumn>) this.columns)
        {
          if (column == header)
          {
            Rectangle displayRectangle = this.DisplayRectangle;
            return new Rectangle(displayRectangle.X + num, displayRectangle.Y, column.Width, this.ColumnHeaderHeight);
          }
          if (column.Visible)
            num += column.Width;
        }
      }
      return Rectangle.Empty;
    }

    protected Rectangle ColumnHeadersRectangle
    {
      get
      {
        if (!this.IsHeaderVisible)
          return Rectangle.Empty;
        Rectangle displayRectangle = this.DisplayRectangle;
        displayRectangle.X += 8;
        displayRectangle.Height = this.columnHeaderHeight;
        displayRectangle.Width = this.GetColumnHeadersWidth();
        return displayRectangle;
      }
    }

    private void MoveColumnHeader(IColumn columnHeader, int position)
    {
      if (columnHeader == null || !columnHeader.Visible)
        return;
      int index1 = 0;
      int num = 0;
      int index2 = 0;
      while (index1 < this.columns.Count - 1)
      {
        if (position >= num && position < num + columnHeader.Width)
        {
          if (this.columns[index1] == columnHeader)
            break;
          this.columns.Remove(columnHeader);
          this.columns.Insert(index1, columnHeader);
          break;
        }
        if (this.columns[index2] == columnHeader)
          ++index2;
        if (this.columns[index2].Visible)
          num += this.columns[index2].Width;
        ++index1;
        ++index2;
      }
    }

    private void InvalidateMarker()
    {
      if (!this.MarkerVisible || this.Items.Count == 0 || this.MarkerItem == null)
        return;
      Rectangle markerBounds = this.GetMarkerBounds(this.MarkerItem, 2);
      if (markerBounds.IsEmpty)
        return;
      markerBounds.Inflate(1, 1);
      this.SafeInvalidate(ItemViewInvalidateOptions.None, this.Translate(markerBounds, false));
    }

    private Rectangle GetMarkerBounds(IViewableItem item, int width)
    {
      Rectangle itemBounds = this.GetItemBounds(item);
      switch (this.ItemViewMode)
      {
        case ItemViewMode.Thumbnail:
        case ItemViewMode.Tile:
          return this.ItemViewLayout != ItemViewLayout.Top ? new Rectangle(itemBounds.Left, itemBounds.Top - width / 2, itemBounds.Width, width) : new Rectangle(itemBounds.Left - width / 2, itemBounds.Top, width, itemBounds.Height);
        case ItemViewMode.Detail:
          return new Rectangle(itemBounds.Left, itemBounds.Top - width / 2, itemBounds.Width, width);
        default:
          return Rectangle.Empty;
      }
    }

    private System.Drawing.Size GetItemBorderSize()
    {
      return this.ItemViewMode == ItemViewMode.Detail ? System.Drawing.Size.Empty : this.ItemPadding;
    }

    private System.Drawing.Size GetDefaultItemSize(int clientWidth)
    {
      switch (this.ItemViewMode)
      {
        case ItemViewMode.Thumbnail:
          return this.itemThumbSize;
        case ItemViewMode.Tile:
          return this.itemTileSize;
        default:
          return new System.Drawing.Size(clientWidth, this.itemRowHeight);
      }
    }

    public IViewableItem GetFirstGroupItem(string group)
    {
      if (this.displayedGroups == null)
        return (IViewableItem) null;
      foreach (GroupHeaderInformation displayedGroup in this.displayedGroups)
      {
        if (displayedGroup.Caption == group)
          return displayedGroup.Items.FirstOrDefault<IViewableItem>();
      }
      return (IViewableItem) null;
    }

    private ItemView.ItemInformation GetItemInformation(IViewableItem item)
    {
      if (item == null)
        return (ItemView.ItemInformation) null;
      if (!this.IsStacked || this.IsStack(item))
      {
        ItemView.ItemInformation itemInformation;
        using (ItemMonitor.Lock((object) this.itemInfos))
          this.itemInfos.TryGetValue(item, out itemInformation);
        return itemInformation;
      }
      using (ItemMonitor.Lock((object) this.stackInfo))
      {
        foreach (IViewableItem key in this.stackInfo.Keys)
        {
          if (this.stackInfo[key].Items.Contains(item))
            return this.GetItemInformation(key);
        }
        return (ItemView.ItemInformation) null;
      }
    }

    protected Rectangle GetItemBounds(IViewableItem item, int subItem)
    {
      Rectangle itemBounds = this.GetItemBounds(item);
      if (itemBounds.IsEmpty || this.ItemViewMode != ItemViewMode.Detail || subItem < 0 || subItem > this.columns.Count)
        return itemBounds;
      Rectangle columnHeaderRectangle = this.GetColumnHeaderRectangle(this.columns[subItem]);
      itemBounds.X = columnHeaderRectangle.X;
      itemBounds.Width = columnHeaderRectangle.Width;
      return itemBounds;
    }

    public Rectangle GetItemBounds(IViewableItem item)
    {
      ItemView.ItemInformation itemInformation = this.GetItemInformation(item);
      return itemInformation != null ? itemInformation.Bounds : Rectangle.Empty;
    }

    public bool IsStack(IViewableItem item)
    {
      if (item == null || !this.IsStacked)
        return false;
      using (ItemMonitor.Lock((object) this.stackInfo))
        return this.stackInfo.ContainsKey(item);
    }

    public IGroupInfo GetStackGroupInfo(IViewableItem item)
    {
      if (item == null)
        return (IGroupInfo) null;
      using (ItemMonitor.Lock((object) this.stackInfo))
      {
        ItemView.StackInfo stackInfo;
        return !this.stackInfo.TryGetValue(item, out stackInfo) ? (IGroupInfo) null : stackInfo.GroupInfo;
      }
    }

    public string GetStackCaption(IViewableItem item)
    {
      if (item == null)
        return (string) null;
      using (ItemMonitor.Lock((object) this.stackInfo))
      {
        ItemView.StackInfo stackInfo;
        return !this.stackInfo.TryGetValue(item, out stackInfo) ? item.Text : stackInfo.Text;
      }
    }

    public object GetStackKey(IViewableItem item)
    {
      if (item == null)
        return (object) null;
      using (ItemMonitor.Lock((object) this.stackInfo))
      {
        ItemView.StackInfo stackInfo;
        return !this.stackInfo.TryGetValue(item, out stackInfo) ? (object) null : stackInfo.Key;
      }
    }

    public IViewableItem[] GetStackItems(IViewableItem item)
    {
      if (item == null)
        return (IViewableItem[]) null;
      using (ItemMonitor.Lock((object) this.stackInfo))
      {
        ItemView.StackInfo stackInfo;
        IViewableItem[] stackItems;
        if (this.stackInfo.TryGetValue(item, out stackInfo))
          stackItems = stackInfo.Items.ToArray();
        else
          stackItems = new IViewableItem[1]{ item };
        return stackItems;
      }
    }

    public int GetStackCount(IViewableItem item)
    {
      if (item == null)
        return 0;
      using (ItemMonitor.Lock((object) this.stackInfo))
      {
        ItemView.StackInfo stackInfo;
        return !this.stackInfo.TryGetValue(item, out stackInfo) ? 1 : stackInfo.Items.Count;
      }
    }

    private Rectangle CalcItemPositions(Graphics gr, bool withSort)
    {
      bool areGroupsVisible = this.AreGroupsVisible;
      bool flag;
      using (ItemMonitor.Lock((object) this.displayedItems))
        flag = this.displayedItems == null || this.displayedItems.Count == 0;
      List<IViewableItem> viewableItemList;
      List<GroupHeaderInformation> headerInformationList;
      if (withSort | flag)
      {
        viewableItemList = new List<IViewableItem>(this.items.Lock<IViewableItem>());
        headerInformationList = new List<GroupHeaderInformation>();
        if (areGroupsVisible)
        {
          IGrouper<IViewableItem> grouper = this.ItemGrouper;
          if (this.IsStacked && this.ItemStacker.First<IViewableItem>() == this.ItemGrouper.First<IViewableItem>())
            grouper = (IGrouper<IViewableItem>) new ItemView.AlphabetGrouper(grouper);
          IEnumerable<GroupContainer<IViewableItem>> source = new GroupManager<IViewableItem>(grouper, (IEnumerable<IViewableItem>) viewableItemList).GetGroups();
          if (this.GroupSortingOrder != SortOrder.None)
            source = (IEnumerable<GroupContainer<IViewableItem>>) source.OrderBy<GroupContainer<IViewableItem>, GroupContainer<IViewableItem>>((Func<GroupContainer<IViewableItem>, GroupContainer<IViewableItem>>) (a => a));
          if (this.GroupSortingOrder == SortOrder.Descending)
            source = source.Reverse<GroupContainer<IViewableItem>>();
          foreach (GroupContainer<IViewableItem> groupContainer in source)
          {
            headerInformationList.Add(new GroupHeaderInformation(groupContainer.Caption, groupContainer.Items, this.GroupsStatus.IsCollapsed(groupContainer.Caption)));
            viewableItemList.AddRange((IEnumerable<IViewableItem>) groupContainer.Items);
          }
        }
        else
          headerInformationList.Add(new GroupHeaderInformation((string) null, new List<IViewableItem>((IEnumerable<IViewableItem>) viewableItemList)));
        if (this.IsStacked)
        {
          Dictionary<IViewableItem, ItemView.StackInfo> dictionary = new Dictionary<IViewableItem, ItemView.StackInfo>();
          foreach (GroupHeaderInformation headerInformation in headerInformationList)
          {
            GroupManager<IViewableItem> groupManager = new GroupManager<IViewableItem>(this.ItemStacker, (IEnumerable<IViewableItem>) headerInformation.Items);
            headerInformation.Items.Clear();
            headerInformation.ItemCount = 0;
            foreach (GroupContainer<IViewableItem> group in groupManager.GetGroups())
            {
              if (this.ItemStackSorter != null)
                group.Items.Sort(this.ItemStackSorter);
              ItemView.StackInfo lsi = new ItemView.StackInfo(group);
              this.OnProcessStack(lsi);
              IViewableItem key = group.Items[0];
              dictionary[key] = lsi;
              headerInformation.Items.Add(group.Items[0]);
              if (this.GroupHeaderTrueCount)
                headerInformation.ItemCount += group.Items.Count;
              else
                ++headerInformation.ItemCount;
            }
          }
          using (ItemMonitor.Lock((object) this.stackInfo))
            this.stackInfo = dictionary;
        }
        else
        {
          using (ItemMonitor.Lock((object) this.stackInfo))
            this.stackInfo.Clear();
        }
        IComparer<IViewableItem> comparer = (IComparer<IViewableItem>) null;
        using (ItemMonitor.Lock((object) this.itemSorters))
        {
          if (this.itemSorters[0] != null)
            comparer = (IComparer<IViewableItem>) new ChainedComparer<IViewableItem>((IEnumerable<IComparer<IViewableItem>>) this.itemSorters);
        }
        if (comparer != null && this.ItemSortOrder == SortOrder.Descending)
          comparer = comparer.Reverse<IViewableItem>();
        if (comparer != null)
        {
          try
          {
            headerInformationList.ParallelForEach<GroupHeaderInformation>((Action<GroupHeaderInformation>) (ghi => ghi.Items.Sort(comparer)));
          }
          catch
          {
          }
        }
        viewableItemList.Clear();
        foreach (GroupHeaderInformation headerInformation in headerInformationList)
          viewableItemList.AddRange((IEnumerable<IViewableItem>) headerInformation.Items);
      }
      else
      {
        using (ItemMonitor.Lock((object) this.displayedItems))
          viewableItemList = this.displayedItems;
        using (ItemMonitor.Lock((object) this.displayedGroups))
          headerInformationList = this.displayedGroups;
      }
      if (headerInformationList.Count > 0)
        this.groupsStatus = new ItemViewGroupsStatus((IEnumerable<GroupHeaderInformation>) headerInformationList);
      Dictionary<IViewableItem, ItemView.ItemInformation> infos = new Dictionary<IViewableItem, ItemView.ItemInformation>();
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int row = 0;
      int num4 = 0;
      IColumn expandedColumn = this.GetExpandedColumn();
      IColumn column = this.columns.FirstOrDefault<IColumn>((Func<IColumn, bool>) (c => c.Visible));
      Rectangle rectangle1 = expandedColumn != null ? this.GetColumnHeaderRectangle(expandedColumn) : Rectangle.Empty;
      Rectangle viewRectangle = this.ViewRectangle;
      System.Drawing.Size itemBorderSize = this.GetItemBorderSize();
      Rectangle rectangle2;
      if (this.ItemViewMode == ItemViewMode.Detail)
      {
        viewRectangle.Width = this.GetColumnHeadersWidth();
        ref Rectangle local = ref viewRectangle;
        int y1 = local.Y;
        rectangle2 = this.ViewRectangle;
        int y2 = rectangle2.Y;
        rectangle2 = this.DisplayRectangle;
        int y3 = rectangle2.Y;
        int num5 = y2 - y3;
        local.Y = y1 - num5;
      }
      System.Drawing.Point empty = System.Drawing.Point.Empty;
      Rectangle a1 = new Rectangle(empty, System.Drawing.Size.Empty);
      for (int index = 0; index < headerInformationList.Count; ++index)
      {
        GroupHeaderInformation group = headerInformationList[index];
        if (areGroupsVisible && !string.IsNullOrEmpty(group.Caption))
        {
          if (empty.X != 0)
          {
            empty.Y += itemBorderSize.Height * 2 + num2;
            empty.X = 0;
            ++row;
            num3 = 0;
          }
          group.Bounds = new Rectangle(empty.X, empty.Y, viewRectangle.Width, this.GroupHeaderHeight);
          group.ExpandedColumnBounds = Rectangle.Empty;
          empty.Y += this.GroupHeaderHeight;
          a1 = Rectangle.Union(a1, group.Bounds);
        }
        if (!group.Collapsed)
        {
          int startIndex = 0;
          int num6 = 0;
          foreach (IViewableItem key in group.Items)
          {
            if (key.View != null)
            {
              System.Drawing.Size size = this.GetDefaultItemSize(viewRectangle.Width);
              int num7 = 0;
              switch (this.ItemViewMode)
              {
                case ItemViewMode.Thumbnail:
                case ItemViewMode.Tile:
                  ItemSizeInformation sizeInfo = new ItemSizeInformation()
                  {
                    Graphics = gr,
                    Size = size,
                    DisplayType = this.ItemViewMode,
                    Item = num4,
                    GroupItem = num6,
                    SubItem = -1,
                    Header = (IColumn) null
                  };
                  key.OnMeasure(sizeInfo);
                  rectangle2 = sizeInfo.Bounds;
                  size = rectangle2.Size;
                  break;
                case ItemViewMode.Detail:
                  num7 = 8;
                  break;
              }
              if (size.Width > num1)
                num1 = size.Width;
              if (size.Height > num2)
                num2 = size.Height;
              if (this.IsTopLayout)
              {
                if (empty.X + 2 * itemBorderSize.Width + size.Width >= viewRectangle.Width && num3 > 0)
                {
                  ItemView.RealignItems((IList<IViewableItem>) group.Items, (IDictionary<IViewableItem, ItemView.ItemInformation>) infos, startIndex, num3, viewRectangle.Left, viewRectangle.Right, this.HorizontalItemAlignment);
                  ++row;
                  startIndex = num6;
                  num3 = 0;
                  empty.X = 0;
                  empty.Y += itemBorderSize.Height * 2 + num2;
                  num2 = size.Height;
                }
              }
              else if (empty.Y + 2 * itemBorderSize.Height + size.Height >= viewRectangle.Height && row > 0)
              {
                ++num3;
                row = 0;
                empty.Y = 0;
                empty.X += itemBorderSize.Width * 2 + num1;
                num1 = size.Width;
              }
              Rectangle rectangle3 = new Rectangle(empty.X + itemBorderSize.Width + num7, empty.Y + itemBorderSize.Height, size.Width, size.Height);
              rectangle3.Offset(viewRectangle.Location);
              if (expandedColumn != null)
              {
                Rectangle b = new Rectangle(rectangle1.X, rectangle3.Y, rectangle1.Width, rectangle3.Height);
                group.ExpandedColumnBounds = group.ExpandedColumnBounds.Union(b);
                rectangle3.Width -= rectangle1.Width;
                if (expandedColumn == column)
                  rectangle3.X += rectangle1.Width;
              }
              infos[key] = new ItemView.ItemInformation(key, rectangle3, num3, row, group);
              a1 = Rectangle.Union(a1, rectangle3);
              if (this.IsTopLayout)
              {
                empty.X += itemBorderSize.Width * 2 + size.Width;
                ++num3;
              }
              else
              {
                empty.Y += itemBorderSize.Height * 2 + size.Height;
                ++row;
              }
              ++num6;
              ++num4;
            }
          }
          if (this.IsTopLayout)
            ItemView.RealignItems((IList<IViewableItem>) group.Items, (IDictionary<IViewableItem, ItemView.ItemInformation>) infos, startIndex, num3, viewRectangle.Left, viewRectangle.Right, this.HorizontalItemAlignment);
          if (expandedColumn != null)
          {
            Rectangle expandedColumnBounds = group.ExpandedColumnBounds;
            int columnMinimumHeight = this.GetExpandedColumnMinimumHeight(group.ExpandedColumnBounds.Width);
            rectangle2 = group.ExpandedColumnBounds;
            if (rectangle2.Height < columnMinimumHeight)
            {
              ref System.Drawing.Point local = ref empty;
              int y = local.Y;
              int num8 = columnMinimumHeight;
              rectangle2 = group.ExpandedColumnBounds;
              int height = rectangle2.Height;
              int num9 = num8 - height;
              local.Y = y + num9;
              expandedColumnBounds.Height = columnMinimumHeight;
              group.ExpandedColumnBounds = expandedColumnBounds;
              a1 = Rectangle.Union(a1, expandedColumnBounds);
            }
          }
        }
      }
      using (ItemMonitor.Lock((object) this.itemInfos))
        this.itemInfos = infos;
      using (ItemMonitor.Lock((object) this.displayedItems))
        this.displayedItems = viewableItemList;
      using (ItemMonitor.Lock((object) this.displayedGroups))
        this.displayedGroups = headerInformationList;
      return a1;
    }

    private static void RealignItems(
      IList<IViewableItem> list,
      IDictionary<IViewableItem, ItemView.ItemInformation> infos,
      int startIndex,
      int count,
      int left,
      int right,
      HorizontalAlignment horizontalAlignment)
    {
      if (list.Count == 0 || horizontalAlignment == HorizontalAlignment.Left)
        return;
      int x = 0;
      switch (horizontalAlignment)
      {
        case HorizontalAlignment.Right:
          x = right - infos[list[startIndex + count - 1]].Bounds.Right;
          break;
        case HorizontalAlignment.Center:
          Rectangle a = infos[list[startIndex]].Bounds;
          for (int index = 1; index < count; ++index)
            a = Rectangle.Union(a, infos[list[startIndex + index]].Bounds);
          x = (right - left - a.Width) / 2;
          break;
      }
      if (x == 0)
        return;
      for (int index = 0; index < count; ++index)
        infos[list[startIndex + index]].Offset(x, 0);
    }

    public IViewableItem[] GetColumnRowItems(int column, int row)
    {
      using (ItemMonitor.Lock((object) this.itemInfos))
        return this.itemInfos.Values.Where<ItemView.ItemInformation>((Func<ItemView.ItemInformation, bool>) (info =>
        {
          if (info.Bounds.IsEmpty || info.Row != row && row != -1)
            return false;
          return info.Column == column || column == -1;
        })).Select<ItemView.ItemInformation, IViewableItem>((Func<ItemView.ItemInformation, IViewableItem>) (info => info.Item)).ToArray<IViewableItem>();
    }

    protected bool UpdatePositions(Graphics gr)
    {
      if (!this.positionsInvalidated || this.IsDisposed)
        return false;
      bool itemsResort = this.itemsResort;
      this.positionsInvalidated = false;
      this.itemsResort = false;
      bool flag = gr == null;
      try
      {
        if (flag)
          gr = this.CreateGraphics();
        System.Drawing.Size size = this.CalcItemPositions(gr, itemsResort).Size;
        if (size == this.VirtualSize)
          return false;
        this.VirtualSize = size;
        return true;
      }
      finally
      {
        if (flag && gr != null)
          gr.Dispose();
      }
    }

    public IColumn ColumnHeaderHitTest(int x, int y)
    {
      if (!this.IsHeaderVisible)
        return (IColumn) null;
      if (y > this.ColumnHeaderHeight)
        return (IColumn) null;
      x += this.ScrollPosition.X;
      return this.columns.FirstOrDefault<IColumn>((Func<IColumn, bool>) (ivch => this.GetColumnHeaderRectangle(ivch).Contains(x, y)));
    }

    public int ColumnHeaderSeparatorHitTest(int x, int y)
    {
      if (!this.IsHeaderVisible || y > this.ColumnHeaderHeight)
        return -1;
      x += this.ScrollPosition.X;
      for (int index = this.columns.Count - 1; index >= 0; --index)
      {
        if (this.columns[index].Visible)
        {
          Rectangle columnHeaderRectangle = this.GetColumnHeaderRectangle(this.columns[index]);
          if (x >= columnHeaderRectangle.Right - 2 && x <= columnHeaderRectangle.Right + 2 && y >= columnHeaderRectangle.Top && y < columnHeaderRectangle.Bottom)
            return index;
        }
      }
      return -1;
    }

    public IViewableItem ItemHitTest(System.Drawing.Point pt) => this.ItemHitTest(pt.X, pt.Y);

    public IViewableItem ItemHitTest(int x, int y)
    {
      if (!this.ViewRectangle.Contains(x, y))
        return (IViewableItem) null;
      System.Drawing.Point test = this.Translate(new System.Drawing.Point(x, y), true);
      try
      {
        return this.visibleItems.Lock<IViewableItem>().FirstOrDefault<IViewableItem>((Func<IViewableItem, bool>) (item => this.ItemIntersects(item, test)));
      }
      catch
      {
      }
      return (IViewableItem) null;
    }

    public IViewableItem ItemHitTest(int x, int y, out int subItem)
    {
      subItem = -1;
      IViewableItem viewableItem = this.ItemHitTest(x, y);
      if (viewableItem != null && this.ItemViewMode == ItemViewMode.Detail)
      {
        System.Drawing.Point pt = this.Translate(new System.Drawing.Point(x, y), true);
        using (ItemMonitor.Lock(this.columns.SyncRoot))
        {
          for (int subItem1 = 0; subItem1 < this.columns.Count; ++subItem1)
          {
            if (this.GetItemBounds(viewableItem, subItem1).Contains(pt))
            {
              subItem = subItem1;
              break;
            }
          }
        }
      }
      return viewableItem;
    }

    public bool ItemIntersects(IViewableItem item, System.Drawing.Point pt)
    {
      Rectangle itemBounds = this.GetItemBounds(item);
      if (!itemBounds.Contains(pt))
        return false;
      if (!(item is IViewableItemHitTest viewableItemHitTest))
        return true;
      pt.Offset(-itemBounds.X, -itemBounds.Y);
      return viewableItemHitTest.Contains(pt);
    }

    public bool ItemIntersects(IViewableItem item, Rectangle rc)
    {
      Rectangle itemBounds = this.GetItemBounds(item);
      if (!itemBounds.IntersectsWith(rc))
        return false;
      if (!(item is IViewableItemHitTest viewableItemHitTest))
        return true;
      rc.Offset(-itemBounds.X, -itemBounds.Y);
      return viewableItemHitTest.IntersectsWith(rc);
    }

    public bool InvokeItemClick(IViewableItem item, System.Drawing.Point pt)
    {
      Rectangle itemBounds = this.GetItemBounds(item);
      pt = this.Translate(pt, true);
      if (!itemBounds.Contains(pt))
        return false;
      pt.Offset(-itemBounds.X, -itemBounds.Y);
      return item.OnClick(pt);
    }

    protected void InvalidateItem(IViewableItem item, bool withSize)
    {
      if (withSize)
      {
        this.SafeInvalidate();
      }
      else
      {
        Rectangle rc = this.GetItemBounds(item);
        rc.Inflate(this.GetItemBorderSize());
        rc.Inflate(1, 1);
        this.SafeInvalidate(ItemViewInvalidateOptions.None, this.Translate(rc, false));
        ItemView.ItemInformation itemInformation = this.GetItemInformation(item);
        if (itemInformation == null || itemInformation.Group == null)
          return;
        rc = itemInformation.Group.ExpandedColumnBounds;
        if (rc.IsEmpty)
          return;
        this.SafeInvalidate(ItemViewInvalidateOptions.None, this.Translate(rc, false));
      }
    }

    protected virtual void OnDrawColumnHeaders(Graphics gr)
    {
      foreach (ItemViewColumn column in (SmartList<IColumn>) this.columns)
      {
        if (column.Visible)
        {
          try
          {
            Rectangle columnHeaderRectangle = this.GetColumnHeaderRectangle((IColumn) column);
            using (gr.SaveState())
            {
              gr.IntersectClip(columnHeaderRectangle);
              gr.TranslateTransform((float) columnHeaderRectangle.X, (float) columnHeaderRectangle.Y);
              columnHeaderRectangle.Offset(-columnHeaderRectangle.X, -columnHeaderRectangle.Y);
              if (this.pressedHeader == column)
                column.DrawHeader(gr, columnHeaderRectangle, HeaderState.Pressed);
              else if (this.hotHeader == column)
                column.DrawHeader(gr, columnHeaderRectangle, HeaderState.Hot);
              else
                column.DrawHeader(gr, columnHeaderRectangle, this.SortColumn == column ? HeaderState.Active : HeaderState.Normal);
            }
          }
          catch
          {
          }
        }
      }
    }

    protected virtual void OnDrawGroupHeader(
      Graphics graphics,
      GroupHeaderInformation groupHeaderInformation)
    {
      Rectangle bounds = groupHeaderInformation.Bounds;
      string str = this.ShowGroupCount ? string.Format("{0} ({1})", (object) groupHeaderInformation.Caption, (object) groupHeaderInformation.ItemCount) : groupHeaderInformation.Caption;
      Font font = FC.Get(this.Font, this.Font.Size * 1.15f);
      Color darkBlue = Color.DarkBlue;
      Color controlDark = SystemColors.ControlDark;
      System.Drawing.Size size = graphics.MeasureString(str, font).ToSize();
      Bitmap bitmap = groupHeaderInformation.Collapsed ? this.groupCollapsedImage : this.groupExpandedImage;
      int width = size.Width;
      int height = size.Height;
      int y1 = (bounds.Height - height) / 2 + 2;
      int y2 = y1 + (height - 1) / 2;
      int x1 = 2;
      if (bitmap != null)
        width += bitmap.Width + 2;
      if (this.HorizontalItemAlignment == HorizontalAlignment.Center)
        x1 += (bounds.Width - width) / 2;
      int num = x1;
      if (bitmap != null)
      {
        int y3 = y1 + (height - bitmap.Height) / 2;
        Rectangle rect = new Rectangle(x1, y3, bitmap.Width, bitmap.Height);
        graphics.DrawImage((Image) bitmap, rect);
        rect.Offset(bounds.Location);
        groupHeaderInformation.ArrowBounds = rect;
        x1 += bitmap.Width;
      }
      else
        groupHeaderInformation.ArrowBounds = Rectangle.Empty;
      using (Brush brush = (Brush) new SolidBrush(darkBlue))
      {
        graphics.DrawString(str, font, brush, (float) x1, (float) y1);
        groupHeaderInformation.TextBounds = new Rectangle(bounds.X + x1, bounds.Y + y1, size.Width, size.Height);
      }
      int x2 = num + width + 5;
      Rectangle rect1 = new Rectangle(x2, y2, bounds.Width - x2 - 5, 1);
      if (rect1.Width > 5)
      {
        using (Brush brush = (Brush) new SolidBrush(controlDark))
          graphics.FillRectangle(brush, rect1);
      }
      rect1 = new Rectangle(5, y2, num - 10, 1);
      if (rect1.Width <= 5)
        return;
      using (Brush brush = (Brush) new SolidBrush(controlDark))
        graphics.FillRectangle(brush, rect1);
    }

    protected virtual void OnDrawItemSelection(Graphics gr, Rectangle rc, ItemViewStates drawState)
    {
      Rectangle rc1 = rc;
      rc1.Inflate(-1, -1);
      gr.DrawStyledRectangle(rc1, StyledRenderer.GetAlphaStyle(drawState.HasFlag((Enum) ItemViewStates.Selected), drawState.HasFlag((Enum) ItemViewStates.Hot), drawState.HasFlag((Enum) ItemViewStates.Focused)), this.Focused ? SystemColors.Highlight : Color.Gray);
    }

    protected virtual void OnDrawItemStates(
      Graphics gr,
      IViewableItem item,
      Rectangle rc,
      ItemViewStates drawState)
    {
      this.OnDrawItemSelection(gr, rc, drawState & ~item.GetOwnerDrawnStates(this.ItemViewMode));
    }

    protected void DrawBackground(Graphics gr, DrawItemViewOptions drawItemsFlags = DrawItemViewOptions.Default)
    {
      if ((drawItemsFlags & DrawItemViewOptions.Background) != (DrawItemViewOptions) 0)
        gr.Clear(this.BackColor);
      if ((drawItemsFlags & DrawItemViewOptions.BackgroundImage) == (DrawItemViewOptions) 0 || this.BackgroundImage == null)
        return;
      Rectangle rectangle = new Rectangle(0, 0, this.BackgroundImage.Width, this.BackgroundImage.Height);
      gr.DrawImage(this.BackgroundImage, rectangle.Align(this.DisplayRectangle, this.BackgroundImageAlignment), rectangle, GraphicsUnit.Pixel);
    }

    protected virtual void DrawMarker(Graphics gr, Rectangle bounds)
    {
      using (Brush brush = (Brush) new SolidBrush(Color.FromArgb(128, SystemColors.ControlDarkDark)))
      {
        gr.FillRectangle(brush, bounds);
        gr.DrawRectangle(Pens.Black, bounds);
      }
    }

    protected void DrawItems(Graphics gr, DrawItemViewOptions drawItemsFlags = DrawItemViewOptions.Default)
    {
      Rectangle viewRectangle = this.ViewRectangle;
      using (gr.SaveState())
      {
        gr.TranslateTransform((float) -this.ScrollPosition.X, 0.0f);
        viewRectangle.Offset(this.ScrollPosition.X, 0);
        if (this.IsHeaderVisible && (drawItemsFlags & DrawItemViewOptions.ColumnHeaders) != (DrawItemViewOptions) 0)
        {
          this.OnDrawColumnHeaders(gr);
          gr.IntersectClip(viewRectangle);
        }
        Graphics graphics = gr;
        System.Drawing.Point scrollPosition = this.ScrollPosition;
        double dy = (double) (-scrollPosition.Y + viewRectangle.Top);
        graphics.TranslateTransform(0.0f, (float) dy);
        ref Rectangle local = ref viewRectangle;
        scrollPosition = this.ScrollPosition;
        int y = scrollPosition.Y - viewRectangle.Top;
        local.Offset(0, y);
        using (ItemMonitor.Lock((object) this.visibleItems))
        {
          using (ItemMonitor.Lock((object) this.selectedItems))
          {
            using (ItemMonitor.Lock((object) this.displayedGroups))
            {
              using (ItemMonitor.Lock(this.items.SyncRoot))
              {
                this.visibleItems.Clear();
                this.selectedItems.Clear();
                ItemDrawInformation itemDrawInformation1 = new ItemDrawInformation();
                itemDrawInformation1.Item = -1;
                itemDrawInformation1.Graphics = gr;
                itemDrawInformation1.DisplayType = this.ItemViewMode;
                ItemDrawInformation drawInfo1 = itemDrawInformation1;
                IColumn expandedColumn = this.GetExpandedColumn();
                IViewableItem focusedItem = this.FocusedItem;
                ItemView.ItemInformation itemInformation = this.GetItemInformation(focusedItem);
                foreach (GroupHeaderInformation displayedGroup in this.displayedGroups)
                {
                  if (this.AreGroupsVisible && !displayedGroup.Bounds.IsEmpty && gr.IsVisible(displayedGroup.Bounds) && drawItemsFlags.HasFlag((Enum) DrawItemViewOptions.GroupHeaders))
                  {
                    using (gr.SaveState())
                    {
                      Rectangle bounds = displayedGroup.Bounds;
                      gr.IntersectClip(bounds);
                      gr.TranslateTransform((float) bounds.X, (float) bounds.Y);
                      bounds.Offset(-bounds.X, -bounds.Y);
                      this.OnDrawGroupHeader(gr, displayedGroup);
                    }
                  }
                  if (expandedColumn != null && gr.IsVisible(displayedGroup.ExpandedColumnBounds) && drawItemsFlags.HasFlag((Enum) DrawItemViewOptions.GroupHeaders))
                  {
                    using (gr.SaveState())
                    {
                      Rectangle expandedColumnBounds = displayedGroup.ExpandedColumnBounds;
                      gr.IntersectClip(expandedColumnBounds);
                      gr.TranslateTransform((float) expandedColumnBounds.X, (float) expandedColumnBounds.Y);
                      expandedColumnBounds.Offset(-expandedColumnBounds.X, -expandedColumnBounds.Y);
                      ItemDrawInformation itemDrawInformation2 = new ItemDrawInformation();
                      itemDrawInformation2.Item = drawInfo1.Item + 1;
                      itemDrawInformation2.Graphics = gr;
                      itemDrawInformation2.DisplayType = ItemViewMode.Detail;
                      itemDrawInformation2.Bounds = expandedColumnBounds;
                      itemDrawInformation2.State = ItemViewStates.None;
                      itemDrawInformation2.DrawBorder = false;
                      itemDrawInformation2.SubItem = 0;
                      itemDrawInformation2.Header = expandedColumn;
                      itemDrawInformation2.TextColor = this.GetTextColor(ItemViewStates.None);
                      itemDrawInformation2.ControlFocused = this.Focused;
                      itemDrawInformation2.ExpandedColumn = true;
                      ItemDrawInformation drawInfo2 = itemDrawInformation2;
                      (itemInformation == null || itemInformation.Group != displayedGroup ? displayedGroup.Items.FirstOrDefault<IViewableItem>() : focusedItem)?.OnDraw(drawInfo2);
                    }
                  }
                  drawInfo1.GroupItem = -1;
                  foreach (IViewableItem viewableItem in displayedGroup.Items)
                  {
                    ++drawInfo1.Item;
                    ++drawInfo1.GroupItem;
                    if (viewableItem.View != null)
                    {
                      Rectangle itemBounds = this.GetItemBounds(viewableItem);
                      bool flag = false;
                      if (!itemBounds.IsEmpty)
                      {
                        if ((this.itemStates[viewableItem] & ItemViewStates.Selected) != ItemViewStates.None)
                        {
                          this.selectedItems.Add(viewableItem);
                          flag = true;
                        }
                        if (viewRectangle.IntersectsWith(itemBounds))
                        {
                          this.visibleItems.Add(viewableItem);
                          if (gr.IsVisible(itemBounds) && (flag || (drawItemsFlags & DrawItemViewOptions.SelectedOnly) == (DrawItemViewOptions) 0))
                          {
                            ItemViewStates itemState = this.GetItemState(viewableItem);
                            if ((drawItemsFlags & DrawItemViewOptions.FocusRectangle) == (DrawItemViewOptions) 0)
                              itemState &= ~ItemViewStates.Focused;
                            if (!this.Focused)
                              itemState &= ~ItemViewStates.Focused;
                            if (!this.Focused && this.hideSelection)
                              itemState &= ~ItemViewStates.Selected;
                            this.OnDrawItemStates(gr, viewableItem, itemBounds, itemState);
                            using (gr.SaveState())
                            {
                              try
                              {
                                Rectangle rect = itemBounds;
                                gr.IntersectClip(rect);
                                gr.TranslateTransform((float) itemBounds.X, (float) itemBounds.Y);
                                itemBounds.Offset(-itemBounds.X, -itemBounds.Y);
                                drawInfo1.Bounds = itemBounds;
                                drawInfo1.State = itemState;
                                drawInfo1.SubItem = -1;
                                drawInfo1.Header = (IColumn) null;
                                drawInfo1.TextColor = this.GetTextColor(itemState);
                                drawInfo1.ControlFocused = this.Focused;
                                viewableItem.OnDraw(drawInfo1);
                                drawInfo1.DrawBorder = false;
                                if (this.itemViewMode == ItemViewMode.Detail)
                                {
                                  foreach (ItemViewColumn column in (SmartList<IColumn>) this.columns)
                                  {
                                    ++drawInfo1.SubItem;
                                    if (column.Visible && column != expandedColumn)
                                    {
                                      itemBounds.Width = column.Width;
                                      if (gr.IsVisible(itemBounds))
                                      {
                                        drawInfo1.Bounds = itemBounds;
                                        drawInfo1.Header = (IColumn) column;
                                        using (gr.SaveState())
                                          viewableItem.OnDraw(drawInfo1);
                                      }
                                      drawInfo1.DrawBorder = true;
                                      itemBounds.X += itemBounds.Width;
                                    }
                                  }
                                }
                              }
                              catch (Exception ex)
                              {
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    private Color GetTextColor(ItemViewStates drawState) => this.ForeColor;

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      try
      {
        this.UpdatePositions(e.Graphics);
        this.DrawBackground(e.Graphics);
      }
      catch (Exception ex)
      {
        base.OnPaintBackground(e);
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics graphics = e.Graphics;
      using (graphics.SaveState())
      {
        try
        {
          if (this.UpdatePositions(graphics))
            return;
          this.DrawItems(graphics);
          if (this.MarkerVisible)
          {
            Rectangle bounds = this.Translate(this.GetMarkerBounds(this.MarkerItem, 2), false);
            this.DrawMarker(graphics, bounds);
          }
          if (!this.selectionRect.IsEmpty)
          {
            Rectangle rect = this.Translate(this.selectionRect, false);
            rect.Inflate(-2, -2);
            using (Brush brush = (Brush) new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
              graphics.FillRectangle(brush, rect);
            graphics.DrawRectangle(SystemPens.Highlight, rect);
          }
          if (this.resizeColumn != null)
          {
            Rectangle viewRectangle = this.ViewRectangle;
            Rectangle columnHeaderRectangle = this.GetColumnHeaderRectangle(this.resizeColumn);
            viewRectangle.X = columnHeaderRectangle.Right - this.ScrollPositionX;
            using (Pen pen = new Pen(Color.Black))
            {
              pen.DashStyle = DashStyle.DashDot;
              graphics.DrawLine(pen, viewRectangle.Location, new System.Drawing.Point(viewRectangle.X, viewRectangle.Bottom));
            }
          }
          if (this.dragHeader != null)
          {
            Rectangle viewRectangle = this.ViewRectangle;
            Rectangle columnHeaderRectangle = this.GetColumnHeaderRectangle(this.dragHeader);
            viewRectangle.X = columnHeaderRectangle.X - this.ScrollPositionX;
            viewRectangle.Width = columnHeaderRectangle.Width;
            using (Brush brush = (Brush) new SolidBrush(Color.FromArgb(128, SystemColors.ControlDark)))
              graphics.FillRectangle(brush, viewRectangle);
          }
          while (this.pendingSelectedIndexChanged)
          {
            this.pendingSelectedIndexChanged = false;
            this.OnSelectedIndexChanged();
          }
        }
        catch
        {
        }
      }
      this.OnPostPaint(e);
    }

    protected virtual void OnPostPaint(PaintEventArgs e)
    {
      if (this.PostPaint == null)
        return;
      this.PostPaint((object) this, e);
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (this.ItemViewMode != ItemViewMode.Detail)
        this.SafeInvalidate();
      System.Drawing.Point client = this.PointToClient(Cursor.Position);
      this.UpdateHotItemState(Control.MouseButtons, client.X, client.Y);
    }

    protected override void OnGotFocus(EventArgs e)
    {
      base.OnGotFocus(e);
      if (this.displayedItems.Count <= 0)
        return;
      if (this.itemStates.FindFirst(ItemViewStates.Focused) == null)
        this.itemStates.Focus(this.displayedItems[0]);
      this.Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
      base.OnLostFocus(e);
      this.Invalidate();
    }

    private ContextMenuStrip GetHeaderMenu()
    {
      if (this.HeaderContextMenuStrip != null)
        return this.HeaderContextMenuStrip;
      return this.AutomaticHeaderMenu ? this.autoHeaderContextMenuStrip : (ContextMenuStrip) null;
    }

    public ContextMenuStrip GetViewMenu()
    {
      if (this.ViewContextMenuStrip != null)
        return this.ViewContextMenuStrip;
      return this.AutomaticViewMenu && this.Columns.Count > 0 ? this.autoViewContextMenuStrip : (ContextMenuStrip) null;
    }

    public ContextMenuStrip AutoViewContextMenuStrip => this.autoViewContextMenuStrip;

    public void CreateGroupMenu(ToolStripItemCollection toolStripItemCollection)
    {
      ContextMenuBuilder contextMenuBuilder = new ContextMenuBuilder();
      ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(ItemView.tr["NotGrouped", "Not Grouped"]);
      toolStripMenuItem1.Checked = this.ItemGrouper == null;
      toolStripMenuItem1.Tag = (object) null;
      ToolStripMenuItem toolStripMenuItem2 = toolStripMenuItem1;
      toolStripMenuItem2.Click += new EventHandler(this.GroupMenuItemClicked);
      toolStripItemCollection.Add((ToolStripItem) toolStripMenuItem2);
      toolStripItemCollection.Add((ToolStripItem) new ToolStripSeparator());
      foreach (ItemViewColumn column in (SmartList<IColumn>) this.Columns)
      {
        if (column.ColumnGrouper != null)
        {
          bool chk = this.ItemGrouper.Contains<IViewableItem>(column.ColumnGrouper);
          bool topLevel = column.Visible | chk;
          contextMenuBuilder.Add(FormUtility.FixAmpersand(column.Text), topLevel, chk, new EventHandler(this.GroupMenuItemClicked), (object) column, column.LastTimeVisible);
        }
      }
      toolStripItemCollection.AddRange(contextMenuBuilder.Create(20));
    }

    public void CreateArrangeMenu(ToolStripItemCollection toolStripItemCollection)
    {
      ContextMenuBuilder contextMenuBuilder = new ContextMenuBuilder();
      ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(ItemView.tr["NotSorted", "Not Sorted"])
      {
        Checked = this.ItemSorter == null
      };
      toolStripMenuItem.Click += new EventHandler(this.ArrangeMenuItemClicked);
      toolStripItemCollection.Add((ToolStripItem) toolStripMenuItem);
      toolStripItemCollection.Add((ToolStripItem) new ToolStripSeparator());
      foreach (ItemViewColumn column in (SmartList<IColumn>) this.Columns)
      {
        if (column.ColumnSorter != null)
        {
          bool chk = column.ColumnSorter == this.ItemSorter;
          bool topLevel = column.Visible | chk;
          contextMenuBuilder.Add(FormUtility.FixAmpersand(column.Text), topLevel, chk, new EventHandler(this.ArrangeMenuItemClicked), (object) column, column.LastTimeVisible);
        }
      }
      toolStripItemCollection.AddRange(contextMenuBuilder.Create(20));
    }

    public void CreateHeaderMenu(
      ToolStripItemCollection toolStripItemCollection,
      IColumn sizeColumn = null)
    {
      if (sizeColumn != null)
        toolStripItemCollection.Add(ItemView.tr["AutoSizeColumn", "Auto Size Column"], (Image) null, (EventHandler) ((sender, e) => this.AutoSizeHeader(sizeColumn)));
      toolStripItemCollection.Add(ItemView.tr["AutoSizeAllColumns", "Auto Size All Columns"], (Image) null, (EventHandler) ((sender, e) =>
      {
        using (new WaitCursor())
          this.AutoSizeHeaders(false);
      }));
      toolStripItemCollection.Add(ItemView.tr["AutoFitAllColumns", "Auto Fit All Columns"], (Image) null, (EventHandler) ((sender, e) =>
      {
        using (new WaitCursor())
          this.AutoFitHeaders(true);
      }));
      toolStripItemCollection.Add((ToolStripItem) new ToolStripSeparator());
      ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(ItemView.tr["Layout", "Layout"], (Image) null);
      toolStripMenuItem1.DropDown = (ToolStripDropDown) this.GetViewMenu();
      ToolStripMenuItem toolStripMenuItem2 = toolStripMenuItem1;
      if (toolStripMenuItem2.DropDown != null)
      {
        toolStripItemCollection.Add((ToolStripItem) toolStripMenuItem2);
        toolStripItemCollection.Add((ToolStripItem) new ToolStripSeparator());
      }
      ContextMenuBuilder contextMenuBuilder = new ContextMenuBuilder();
      foreach (ItemViewColumn column in (SmartList<IColumn>) this.columns)
        contextMenuBuilder.Add(FormUtility.FixAmpersand(column.Text), column.Visible, column.Visible, new EventHandler(this.HeaderMenuItemClicked), (object) column.Id, column.LastTimeVisible);
      toolStripItemCollection.AddRange(contextMenuBuilder.Create(20));
    }

    public void CreateStackMenu(ToolStripItemCollection toolStripItemCollection)
    {
      ((ToolStripMenuItem) toolStripItemCollection.Add(ItemView.tr["NotStacked", "Not Stacked"], (Image) null, new EventHandler(this.StackMenuItemClicked))).Checked = this.ItemStacker == null;
      toolStripItemCollection.Add((ToolStripItem) new ToolStripSeparator());
      ContextMenuBuilder contextMenuBuilder = new ContextMenuBuilder();
      foreach (ItemViewColumn column in (SmartList<IColumn>) this.columns)
      {
        if (column.ColumnGrouper != null && column.ColumnSorter != null)
        {
          bool chk = this.ItemStacker.Contains<IViewableItem>(column.ColumnGrouper);
          bool topLevel = column.Visible | chk;
          contextMenuBuilder.Add(FormUtility.FixAmpersand(column.Text), topLevel, chk, new EventHandler(this.StackMenuItemClicked), (object) column, column.LastTimeVisible);
        }
      }
      toolStripItemCollection.AddRange(contextMenuBuilder.Create(20));
    }

    private void GroupMenuItemClicked(object sender, EventArgs e)
    {
      if (!(((ToolStripItem) sender).Tag is ItemViewColumn tag))
      {
        this.GroupDisplayEnabled = false;
        this.ItemGrouper = (IGrouper<IViewableItem>) null;
      }
      else
      {
        if (Control.ModifierKeys.HasFlag((Enum) Keys.Control))
        {
          this.ItemGrouper = this.ItemGrouper.Append<IViewableItem>(tag.ColumnGrouper, 3, true);
          this.GroupSortingOrder = SortOrder.Ascending;
        }
        else if (this.ItemGrouper == tag.ColumnGrouper)
        {
          this.GroupSortingOrder = ItemView.FlipSortOrder(this.GroupSortingOrder);
        }
        else
        {
          this.ItemGrouper = tag.ColumnGrouper;
          this.GroupSortingOrder = SortOrder.Ascending;
        }
        this.GroupDisplayEnabled = this.ItemGrouper != null;
      }
    }

    private void ArrangeMenuItemClicked(object sender, EventArgs e)
    {
      if (!(((ToolStripItem) sender).Tag is ItemViewColumn tag))
        this.ItemSorter = (IComparer<IViewableItem>) null;
      else if (this.ItemSorter == tag.ColumnSorter)
      {
        this.ItemSortOrder = ItemView.FlipSortOrder(this.ItemSortOrder);
      }
      else
      {
        this.ItemSorter = tag.ColumnSorter;
        this.itemSortOrder = SortOrder.Ascending;
      }
    }

    private void HeaderMenuItemClicked(object sender, EventArgs e)
    {
      IColumn byId = this.Columns.FindById((int) ((ToolStripItem) sender).Tag);
      if (byId == null)
        return;
      byId.Visible = !byId.Visible;
    }

    private void StackMenuItemClicked(object sender, EventArgs e)
    {
      if (!(((ToolStripItem) sender).Tag is ItemViewColumn tag))
      {
        this.ItemStacker = (IGrouper<IViewableItem>) null;
      }
      else
      {
        this.ItemStackSorter = tag.ColumnSorter;
        if (Control.ModifierKeys.HasFlag((Enum) Keys.Control))
          this.ItemStacker = this.ItemStacker.Append<IViewableItem>(tag.ColumnGrouper, 3, true);
        else
          this.ItemStacker = tag.ColumnGrouper;
      }
    }

    private void autoHeaderContextMenuStrip_Opening(object sender, CancelEventArgs e)
    {
      FormUtility.SafeToolStripClear(this.autoHeaderContextMenuStrip.Items);
      System.Drawing.Point client = this.PointToClient(Cursor.Position);
      this.CreateHeaderMenu(this.autoHeaderContextMenuStrip.Items, this.ColumnHeaderHitTest(client.X, client.Y));
    }

    private void autoViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
    {
      this.miViewDetails.Checked = this.ItemViewMode == ItemViewMode.Detail;
      this.miViewThumbs.Checked = this.ItemViewMode == ItemViewMode.Thumbnail;
      this.miViewTiles.Checked = this.ItemViewMode == ItemViewMode.Tile;
      FormUtility.SafeToolStripClear(this.miArrange.DropDownItems);
      this.CreateArrangeMenu(this.miArrange.DropDownItems);
      FormUtility.SafeToolStripClear(this.miGroup.DropDownItems);
      this.CreateGroupMenu(this.miGroup.DropDownItems);
      this.miStack.Visible = this.StackDisplayEnabled && this.ItemViewMode == ItemViewMode.Thumbnail;
      FormUtility.SafeToolStripClear(this.miStack.DropDownItems);
      this.CreateStackMenu(this.miStack.DropDownItems);
    }

    private void ViewDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
      switch (e.ClickedItem.Name)
      {
        case "miViewDetails":
          this.ItemViewMode = ItemViewMode.Detail;
          break;
        case "miViewThumbs":
          this.ItemViewMode = ItemViewMode.Thumbnail;
          break;
        case "miViewTiles":
          this.ItemViewMode = ItemViewMode.Tile;
          break;
      }
    }

    protected virtual void OnDoubleClickColumnHeaderSeperator(IColumn column, System.Drawing.Point point)
    {
      this.AutoSizeHeader(column);
    }

    protected virtual void OnMouseDownColumnHeaderSeparator(IColumn column, System.Drawing.Point point)
    {
      this.resizeColumn = column;
      this.resizeColumnPos = point.X;
      this.resizeColumnWidth = this.resizeColumn.Width;
      this.Invalidate();
    }

    protected virtual void OnMouseMoveResizeColumnHeader(MouseEventArgs e)
    {
      this.resizeColumn.Width = (this.resizeColumnWidth + (e.X - this.resizeColumnPos)).Clamp(0, 10000);
    }

    protected virtual void OnMouseUpResizeColumnHeader(IColumn column, MouseEventArgs e)
    {
      this.resizeColumn = (IColumn) null;
      this.Invalidate();
    }

    protected virtual void OnMouseDownColumnHeader(IColumn column, System.Drawing.Point pt)
    {
      this.pressedHeader = column;
      this.pressedHeaderPoint = pt;
      this.InvalidateHeader(column);
    }

    private void SetHotHeader(IColumn header)
    {
      if (this.hotHeader == header)
        return;
      if (this.hotHeader != null)
        this.InvalidateHeader(this.hotHeader);
      this.hotHeader = header;
      if (this.hotHeader == null)
        return;
      this.InvalidateHeader(this.hotHeader);
    }

    private void InvalidateHeader(IColumn column)
    {
      Rectangle columnHeaderRectangle = this.GetColumnHeaderRectangle(column);
      if (columnHeaderRectangle.IsEmpty)
        return;
      columnHeaderRectangle.Offset(-this.ScrollPosition.X, 0);
      this.Invalidate(columnHeaderRectangle);
    }

    private void UpdateSelectionFromMouse(IViewableItem item, MouseEventArgs e)
    {
      bool flag1 = (Control.ModifierKeys & Keys.Control) != 0;
      bool flag2 = (Control.ModifierKeys & Keys.Shift) != 0;
      bool flag3 = (Control.ModifierKeys & Keys.Alt) != 0;
      bool flag4 = flag1 | flag2;
      if (item == null)
      {
        if (!this.multiselect)
          return;
        if (!flag4)
          this.itemStates.Clear(ItemViewStates.Selected | ItemViewStates.Focused);
        this.pressetViewPoint = this.Translate(new System.Drawing.Point(e.X, e.Y), true);
        this.selectItemState = new ItemView.StateInfo(this.itemStates);
      }
      else
      {
        ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
        if (((flag3 || this.SelectionMode != SelectionMode.MultiSimple || (e.Button & MouseButtons.Left) == MouseButtons.None ? (!this.IsItemSelected(item) ? 1 : 0) : 1) | (flag4 ? 1 : 0)) != 0)
        {
          if (!flag4)
            this.anchorItem = item;
          if (!flag1 || !this.multiselect)
            si.Clear(ItemViewStates.Selected);
          if (flag2 && this.multiselect)
            this.SelectFromAnchorItem(si, item, true);
          else if (flag1 && this.multiselect)
            si.Flip(item, ItemViewStates.Selected);
          else
            si.Set(item, ItemViewStates.Selected, true);
        }
        si.Focus(item);
        this.itemStates.Update(si);
      }
    }

    protected virtual void OnMouseDownView(MouseEventArgs e)
    {
      IViewableItem viewableItem = this.ItemHitTest(e.X, e.Y);
      this.clickItem = (IViewableItem) null;
      this.customClick = false;
      if (viewableItem == null || !this.IsItemSelected(viewableItem))
      {
        this.UpdateSelectionFromMouse(viewableItem, e);
      }
      else
      {
        this.customClick = e.Button.IsSet<MouseButtons>(MouseButtons.Left) && this.InvokeItemClick(viewableItem, e.Location);
        if (!this.customClick)
          this.clickItem = viewableItem;
      }
      if (viewableItem == null || this.customClick)
        return;
      this.dragItem = viewableItem;
    }

    protected virtual void OnMouseUpView(MouseEventArgs e)
    {
      if (this.clickItem != null)
        this.UpdateSelectionFromMouse(this.clickItem, e);
      if (this.selectItemState != null)
      {
        this.Invalidate(this.Translate(this.selectionRect, false));
        this.selectionRect = Rectangle.Empty;
        this.pressetViewPoint = System.Drawing.Point.Empty;
        this.selectItemState = (ItemView.StateInfo) null;
      }
      if (this.FocusedItem != null)
        this.EnsureItemVisible(this.FocusedItem);
      this.dragItem = (IViewableItem) null;
    }

    protected virtual void OnStartDragColumnHeader(IColumn column, MouseEventArgs e)
    {
      this.dragHeader = column;
      this.Invalidate();
    }

    protected virtual void OnDragColumnHeader(IColumn column, MouseEventArgs e)
    {
      this.MoveColumnHeader(column, e.X + this.ScrollPosition.X);
    }

    protected virtual void OnMouseUpColumnHeader(IColumn column, MouseEventArgs e)
    {
      this.pressedHeader = (IColumn) null;
      this.dragHeader = (IColumn) null;
    }

    protected virtual void OnMouseClickGroupHeader(
      GroupHeaderInformation groupHeaderInformation,
      bool arrow)
    {
      if (arrow)
      {
        groupHeaderInformation.Collapsed = !groupHeaderInformation.Collapsed;
        this.SafeInvalidate();
      }
      else
      {
        if (this.SelectionMode == SelectionMode.One || this.SelectionMode == SelectionMode.None)
          return;
        ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
        ItemViewStates itemViewStates = ItemViewStates.Focused;
        si.Clear(ItemViewStates.Selected | ItemViewStates.Focused);
        foreach (IViewableItem viewableItem in groupHeaderInformation.Items.Lock<IViewableItem>())
        {
          si.Set(viewableItem, ItemViewStates.Selected | itemViewStates, true);
          itemViewStates = ItemViewStates.None;
        }
        this.itemStates.Update(si);
      }
    }

    protected virtual void OnMouseDoubleClickGroupHeader(
      GroupHeaderInformation groupHeaderInformation,
      bool arrow)
    {
      if (arrow)
      {
        this.ExpandGroups(groupHeaderInformation.Collapsed);
      }
      else
      {
        groupHeaderInformation.Collapsed = !groupHeaderInformation.Collapsed;
        this.SafeInvalidate();
      }
    }

    protected override void OnAutoScrolling(AutoScrollEventArgs e)
    {
      base.OnAutoScrolling(e);
      if (this.resizeColumn == null && this.dragHeader == null)
        return;
      e.Y = 0;
    }

    protected override void OnDoubleClick(EventArgs e)
    {
      base.OnDoubleClick(e);
      this.StopLongClick();
      System.Drawing.Point client = this.PointToClient(Cursor.Position);
      int index = this.ColumnHeaderSeparatorHitTest(client.X, client.Y);
      if (index != -1)
      {
        this.OnDoubleClickColumnHeaderSeperator(this.columns[index], client);
      }
      else
      {
        if (!this.ViewRectangle.Contains(client))
          return;
        if (this.AreGroupsVisible)
        {
          System.Drawing.Point pt = this.Translate(client, true);
          foreach (GroupHeaderInformation groupHeaderInformation in this.displayedGroups.Lock<GroupHeaderInformation>())
          {
            if (groupHeaderInformation.Bounds.Contains(pt))
            {
              this.OnMouseDoubleClickGroupHeader(groupHeaderInformation, groupHeaderInformation.ArrowBounds.Contains(pt));
              this.doubleGroupClick = true;
              return;
            }
          }
        }
        if (this.customClick)
          return;
        this.activateButton = this.lastMouseButton;
        try
        {
          this.InvokeActivate();
        }
        finally
        {
          this.activateButton = MouseButtons.None;
        }
      }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.mouseDownPoint = new System.Drawing.Point(e.X, e.Y);
      this.lastMouseButton = e.Button;
      IViewableItem focusedItem = this.FocusedItem;
      this.longClickTimer.Stop();
      this.longClickSubItem = -1;
      if (e.Button == MouseButtons.Left)
      {
        int index = this.ColumnHeaderSeparatorHitTest(e.X, e.Y);
        if (index != -1)
        {
          this.OnMouseDownColumnHeaderSeparator(this.columns[index], e.Location);
          return;
        }
      }
      if (e.Button == MouseButtons.Left)
      {
        IColumn column = this.ColumnHeaderHitTest(e.X, e.Y);
        if (column != null)
        {
          this.OnMouseDownColumnHeader(column, e.Location);
          return;
        }
      }
      if (!this.ViewRectangle.Contains(e.X, e.Y))
        return;
      this.OnMouseDownView(e);
      if (this.customClick || (e.Button & MouseButtons.Left) == MouseButtons.None)
        return;
      int subItem;
      IViewableItem viewableItem = this.ItemHitTest(e.X, e.Y, out subItem);
      if (subItem == -1 || viewableItem != focusedItem)
        return;
      this.longClickItem = viewableItem;
      this.longClickSubItem = subItem;
    }

    private void UpdateSelection(int x, int y)
    {
      if (this.pressetViewPoint.IsEmpty)
        return;
      System.Drawing.Point point = this.Translate(new System.Drawing.Point(x, y), true);
      Rectangle rectangle = RectangleExtensions.Create(this.pressetViewPoint, point);
      if (this.selectionRect == rectangle)
        return;
      this.Invalidate(this.Translate(this.selectionRect, false));
      this.selectionRect = rectangle;
      this.Invalidate(this.Translate(this.selectionRect, false));
      ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
      si.Clear(ItemViewStates.Focused);
      bool flag1 = false;
      bool flag2 = (Control.ModifierKeys & Keys.Control) != 0;
      IEnumerable<IViewableItem> viewableItems = this.visibleItems.Lock<IViewableItem>();
      Rectangle rc = this.Translate(this.ViewRectangle, true);
      foreach (IViewableItem viewableItem in viewableItems)
      {
        if (this.ItemIntersects(viewableItem, rc))
        {
          if ((Control.ModifierKeys & (Keys.Shift | Keys.Control)) != Keys.None)
            si.Set(viewableItem, ItemViewStates.Selected, this.selectItemState[viewableItem].HasFlag((Enum) ItemViewStates.Selected));
          else
            si.Set(viewableItem, ItemViewStates.Selected, false);
          if (this.ItemIntersects(viewableItem, this.selectionRect))
          {
            if (flag2)
              si.Flip(viewableItem, ItemViewStates.Selected);
            else
              si.Set(viewableItem, ItemViewStates.Selected, true);
            if (!flag1 && this.ItemIntersects(viewableItem, point))
            {
              flag1 = true;
              si.Focus(viewableItem);
            }
          }
        }
      }
      this.itemStates.Update(si);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      string caption = this.hotHeader == null || !this.HeaderToolTips ? string.Empty : this.hotHeader.TooltipText;
      if (caption != this.toolTip.GetToolTip((Control) this))
        this.toolTip.SetToolTip((Control) this, caption);
      if (this.longClickSubItem != -1 && (Math.Abs(e.X - this.mouseDownPoint.X) > 2 || Math.Abs(e.Y - this.mouseDownPoint.Y) > 2))
        this.StopLongClick();
      if (this.resizeColumn != null)
        this.OnMouseMoveResizeColumnHeader(e);
      else if (this.dragHeader != null)
        this.OnDragColumnHeader(this.dragHeader, e);
      else if (this.pressedHeader != null)
      {
        System.Drawing.Size dragSize = SystemInformation.DragSize;
        if (dragSize.Width >= Math.Abs(e.X - this.pressedHeaderPoint.X))
        {
          dragSize = SystemInformation.DragSize;
          if (dragSize.Height >= Math.Abs(e.Y - this.pressedHeaderPoint.Y))
            return;
        }
        this.OnStartDragColumnHeader(this.pressedHeader, e);
      }
      else
      {
        if (this.dragItem != null)
        {
          int num1 = Math.Abs(this.mouseDownPoint.X - e.X);
          System.Drawing.Size dragSize = SystemInformation.DragSize;
          int width = dragSize.Width;
          if (num1 <= width)
          {
            int num2 = Math.Abs(this.mouseDownPoint.Y - e.Y);
            dragSize = SystemInformation.DragSize;
            int height = dragSize.Height;
            if (num2 <= height)
              goto label_17;
          }
          this.OnItemDrag(new ItemDragEventArgs(e.Button, (object) this.dragItem));
          this.clickItem = (IViewableItem) null;
          this.dragItem = (IViewableItem) null;
        }
label_17:
        this.SetHotHeader(this.ColumnHeaderHitTest(e.X, e.Y));
        this.UpdateContextMenu(e.Location);
        this.Cursor = this.ColumnHeaderSeparatorHitTest(e.X, e.Y) != -1 ? Cursors.VSplit : Cursors.Default;
        this.UpdateHotItemState(e.Button, e.X, e.Y);
        if (e.Button == MouseButtons.Middle)
          return;
        this.UpdateSelection(e.X, e.Y);
      }
    }

    private void UpdateContextMenu(System.Drawing.Point location)
    {
      if (location == System.Drawing.Point.Empty)
        location = this.PointToClient(Cursor.Position);
      if (this.InplaceEditItem != null)
        this.ContextMenuStrip = (ContextMenuStrip) null;
      else if (this.GetHeaderMenu() != null && this.hotHeader != null)
        this.ContextMenuStrip = this.GetHeaderMenu();
      else if (this.ItemContextMenuStrip != null && this.ItemHitTest(location.X, location.Y) != null)
      {
        this.ContextMenuStrip = this.ItemContextMenuStrip;
      }
      else
      {
        if (this.GetViewMenu() == null)
          return;
        this.ContextMenuStrip = this.GetViewMenu();
      }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      if (this.resizeColumn != null)
        this.OnMouseUpResizeColumnHeader(this.resizeColumn, e);
      else if (this.pressedHeader != null)
      {
        if (this.dragHeader == null)
        {
          this.OnHeaderClick(this.pressedHeader);
          this.Invalidate(this.ColumnHeadersRectangle);
        }
        else
          this.Invalidate();
        this.OnMouseUpColumnHeader(this.pressedHeader, e);
      }
      else
        this.OnMouseUpView(e);
      if (e.Button == MouseButtons.Left && this.AreGroupsVisible && !this.doubleGroupClick)
      {
        System.Drawing.Point pt = this.Translate(this.mouseDownPoint, true);
        foreach (GroupHeaderInformation groupHeaderInformation in this.displayedGroups.Lock<GroupHeaderInformation>())
        {
          Rectangle rectangle = groupHeaderInformation.ArrowBounds;
          if (rectangle.Contains(pt))
          {
            this.OnMouseClickGroupHeader(groupHeaderInformation, true);
            break;
          }
          rectangle = groupHeaderInformation.TextBounds;
          if (rectangle.Contains(pt))
          {
            this.OnMouseClickGroupHeader(groupHeaderInformation, false);
            break;
          }
        }
      }
      this.doubleGroupClick = false;
      if (this.longClickSubItem != -1)
        this.longClickTimer.Start();
      base.OnMouseUp(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      this.StopLongClick();
      this.SetHotHeader((IColumn) null);
      ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
      IViewableItem first;
      while ((first = si.FindFirst(ItemViewStates.Hot)) != null)
        si.Set(first, ItemViewStates.Hot, false);
      this.itemStates.Update(si);
      base.OnMouseLeave(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      base.OnMouseWheel(e);
      this.StopLongClick();
      if (Control.ModifierKeys != Keys.None)
        return;
      this.ScrollView((float) -e.Delta / (float) SystemInformation.MouseWheelScrollDelta * (float) SystemInformation.MouseWheelScrollLines);
    }

    public IViewableItem InplaceEditItem => this.currentInplaceEditItem;

    public int InplaceEditSubItem => this.currentInplaceEditSubItem;

    public bool EditItem(System.Drawing.Point screenCursorLocation)
    {
      screenCursorLocation = this.PointToClient(screenCursorLocation);
      int subItem;
      IViewableItem editItem = this.ItemHitTest(screenCursorLocation.X, screenCursorLocation.Y, out subItem);
      return editItem != null && subItem != -1 && this.EditItem(editItem, subItem);
    }

    public bool EditItem(IViewableItem editItem, int editSubItem = -1)
    {
      if (!this.LabelEdit || editItem == null || this.ItemViewMode != ItemViewMode.Detail)
        return false;
      if (editSubItem == -1)
        editSubItem = this.GetNextEditSubItem(editItem, 0, 1);
      if (editSubItem == -1)
        return false;
      Rectangle rc = this.GetItemBounds(editItem, editSubItem);
      if (rc.IsEmpty)
        return false;
      rc = this.Translate(rc, false);
      Control editControl = editItem.GetEditControl(editSubItem);
      if (editControl == null)
        return false;
      this.currentInplaceEditItem = editItem;
      this.currentInplaceEditSubItem = editSubItem;
      this.currentInplaceEditControl = editControl;
      this.Controls.Add(editControl);
      editControl.Bounds = rc;
      this.MoveEditControl(editControl);
      editControl.Show();
      editControl.Focus();
      editControl.LostFocus += new EventHandler(this.EditorControlLostFocus);
      editControl.SizeChanged += new EventHandler(this.EditorControlSizeChanged);
      editControl.KeyDown += new KeyEventHandler(this.EditorControlKeyDown);
      this.UpdateContextMenu(System.Drawing.Point.Empty);
      return true;
    }

    protected virtual void OnLongClick(IViewableItem editItem, int editSubItem)
    {
      this.EditItem(editItem, editSubItem);
    }

    private void MoveEditControl(Control c)
    {
      Rectangle bounds = c.Bounds;
      Rectangle viewRectangle = this.ViewRectangle;
      if (bounds.Y < viewRectangle.Y)
        bounds.Y = viewRectangle.Y;
      if (bounds.X < viewRectangle.X)
        bounds.X = viewRectangle.X;
      if (bounds.Right > viewRectangle.Right)
        bounds.X = viewRectangle.Right - bounds.Width;
      if (bounds.Bottom > viewRectangle.Bottom)
        bounds.Y = viewRectangle.Bottom - bounds.Height;
      c.Bounds = bounds;
    }

    private void ExitEdit(Control c)
    {
      if (c == null)
        return;
      c.LostFocus -= new EventHandler(this.EditorControlLostFocus);
      c.SizeChanged -= new EventHandler(this.EditorControlSizeChanged);
      this.Controls.Remove(c);
      c.Dispose();
      this.Focus();
      this.currentInplaceEditItem = (IViewableItem) null;
      this.currentInplaceEditSubItem = -1;
      this.currentInplaceEditControl = (Control) null;
      this.UpdateContextMenu(System.Drawing.Point.Empty);
    }

    private void longClickTimer_Tick(object sender, EventArgs e)
    {
      this.longClickTimer.Stop();
      if (this.longClickItem == null || this.longClickSubItem == -1)
        return;
      this.OnLongClick(this.longClickItem, this.longClickSubItem);
    }

    private void EditorControlSizeChanged(object sender, EventArgs e)
    {
      this.MoveEditControl(sender as Control);
    }

    private void EditorControlLostFocus(object sender, EventArgs e)
    {
      this.ExitEdit(sender as Control);
    }

    private void EditorControlKeyDown(object sender, KeyEventArgs e)
    {
      Control c = (Control) sender;
      switch (e.KeyCode)
      {
        case Keys.Tab:
        case Keys.Left:
        case Keys.Right:
          int relative;
          if (e.KeyCode == Keys.Tab)
          {
            relative = e.Modifiers.HasFlag((Enum) Keys.Shift) ? -1 : 1;
          }
          else
          {
            if (!e.Alt)
              break;
            relative = e.KeyCode == Keys.Left ? -1 : 1;
          }
          IViewableItem currentInplaceEditItem = this.currentInplaceEditItem;
          int nextEditSubItem = this.GetNextEditSubItem(currentInplaceEditItem, this.currentInplaceEditSubItem + relative, relative);
          if (nextEditSubItem == -1)
            break;
          this.ExitEdit(c);
          this.EnsureItemVisible(currentInplaceEditItem, nextEditSubItem);
          this.EditItem(currentInplaceEditItem, nextEditSubItem);
          break;
        case Keys.Up:
          IViewableItem relativeItem1 = this.GetRelativeItem(this.currentInplaceEditItem, 0, -1);
          if (relativeItem1 == this.currentInplaceEditItem)
            break;
          this.ExitEdit(c);
          this.SetItemState(relativeItem1, ItemViewStates.Selected | ItemViewStates.Focused, false);
          this.EnsureItemVisible(relativeItem1, this.currentInplaceEditSubItem);
          this.EditItem(relativeItem1, this.currentInplaceEditSubItem);
          break;
        case Keys.Down:
          IViewableItem relativeItem2 = this.GetRelativeItem(this.currentInplaceEditItem, 0, 1);
          if (relativeItem2 == this.currentInplaceEditItem)
            break;
          this.ExitEdit(c);
          this.SetItemState(relativeItem2, ItemViewStates.Selected | ItemViewStates.Focused, false);
          this.EnsureItemVisible(relativeItem2, this.currentInplaceEditSubItem);
          this.EditItem(relativeItem2, this.currentInplaceEditSubItem);
          break;
      }
    }

    private int GetNextEditSubItem(IViewableItem item, int current, int relative)
    {
      if (item == null)
        return -1;
      for (; current >= 0 && current < this.Columns.Count; current += relative)
      {
        if (this.Columns[current].Visible)
        {
          Control editControl = item.GetEditControl(current);
          if (editControl != null)
          {
            editControl.Dispose();
            return current;
          }
        }
      }
      return -1;
    }

    private void StopLongClick()
    {
      if (this.longClickSubItem == -1)
        return;
      this.longClickSubItem = -1;
      this.longClickTimer.Stop();
    }

    public IViewableItem GetRelativeItem(IViewableItem item, int deltaColumns, int deltaRows)
    {
      ItemView.ItemInformation itemInformation = this.GetItemInformation(item);
      if (itemInformation == null)
        return (IViewableItem) null;
      int length1 = this.GetColumnRowItems(0, -1).Length;
      int row1 = itemInformation.Row;
      int num1 = itemInformation.Column;
      int row2;
      int column;
      if (row1 + deltaRows < 0)
      {
        row2 = 0;
        column = (num1 + deltaColumns).Clamp(0, this.GetColumnRowItems(-1, row2).Length - 1);
      }
      else if (row1 + deltaRows >= length1)
      {
        row2 = length1 - 1;
        column = (num1 + deltaColumns).Clamp(0, this.GetColumnRowItems(-1, row2).Length - 1);
      }
      else
      {
        int length2 = this.GetColumnRowItems(-1, row1).Length;
        int num2 = Math.Sign(deltaColumns);
        deltaColumns = Math.Abs(deltaColumns);
        while (--deltaColumns >= 0)
        {
          num1 += num2;
          if (num1 < 0)
          {
            if (row1 == 0)
            {
              num1 = 0;
              break;
            }
            num1 = this.GetColumnRowItems(-1, --row1).Length - 1;
          }
          else if (num1 >= length2)
          {
            if (row1 == length1 - 1)
            {
              num1 = length2 - 1;
              break;
            }
            num1 = 0;
            length2 = this.GetColumnRowItems(-1, ++row1).Length;
          }
        }
        row2 = (row1 + deltaRows).Clamp(0, length1 - 1);
        column = num1.Clamp(0, this.GetColumnRowItems(-1, row2).Length - 1);
      }
      return ((IEnumerable<IViewableItem>) this.GetColumnRowItems(column, row2)).FirstOrDefault<IViewableItem>();
    }

    private IViewableItem GetRelativeItem(IViewableItem focus, Keys key)
    {
      int deltaRows = 0;
      int deltaColumns = 0;
      if (this.IsTopLayout)
      {
        deltaRows = this.ViewRectangle.Height / this.GetDefaultItemSize(0).Height;
      }
      else
      {
        Rectangle viewRectangle = this.ViewRectangle;
        int width1 = viewRectangle.Width;
        viewRectangle = this.ViewRectangle;
        int width2 = this.GetDefaultItemSize(viewRectangle.Width).Width;
        deltaColumns = width1 / width2;
      }
      switch (key)
      {
        case Keys.Prior:
          return this.GetRelativeItem(focus, -deltaColumns, -deltaRows);
        case Keys.Next:
          return this.GetRelativeItem(focus, deltaColumns, deltaRows);
        case Keys.End:
          return this.GetRelativeItem(focus, 10000000, 10000000);
        case Keys.Home:
          return ((IEnumerable<IViewableItem>) this.GetColumnRowItems(0, 0)).FirstOrDefault<IViewableItem>();
        case Keys.Left:
          return this.GetRelativeItem(focus, -1, 0);
        case Keys.Up:
          return this.GetRelativeItem(focus, 0, -1);
        case Keys.Right:
          return this.GetRelativeItem(focus, 1, 0);
        case Keys.Down:
          return this.GetRelativeItem(focus, 0, 1);
        default:
          return focus;
      }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      int count;
      using (ItemMonitor.Lock((object) this.displayedItems))
        count = this.displayedItems.Count;
      if (count == 0)
      {
        base.OnKeyDown(e);
      }
      else
      {
        ItemView.StateInfo si = new ItemView.StateInfo(this.itemStates);
        IViewableItem focus = si.FindFirst(ItemViewStates.Focused);
        using (ItemMonitor.Lock((object) this.displayedItems))
        {
          if (focus == null)
            focus = this.displayedItems[0];
        }
        switch (e.KeyCode)
        {
          case Keys.Return:
            this.InvokeActivate();
            e.Handled = true;
            break;
          case Keys.Space:
            if (!this.multiselect)
              si.Clear(ItemViewStates.Selected);
            if (e.Control)
              si.Flip(focus, ItemViewStates.Selected);
            else
              si.Set(focus, ItemViewStates.Selected, true);
            e.Handled = true;
            break;
          case Keys.Prior:
          case Keys.Next:
          case Keys.End:
          case Keys.Home:
          case Keys.Left:
          case Keys.Up:
          case Keys.Right:
          case Keys.Down:
            focus = this.GetRelativeItem(focus, e.KeyCode);
            si.Focus(focus);
            if (!e.Control && !e.Shift)
              this.anchorItem = focus;
            if (!e.Control)
            {
              si.Clear(ItemViewStates.Selected);
              si.Set(focus, ItemViewStates.Selected, true);
            }
            if (e.Shift && this.multiselect)
              this.SelectFromAnchorItem(si, focus);
            e.Handled = true;
            break;
          default:
            base.OnKeyDown(e);
            return;
        }
        if (e.Handled)
          this.StopLongClick();
        this.EnsureItemVisible(focus);
        this.itemStates.Update(si);
      }
    }

    protected override bool IsInputKey(Keys keyData)
    {
      switch (keyData & ~Keys.Shift)
      {
        case Keys.Left:
        case Keys.Up:
        case Keys.Right:
        case Keys.Down:
          return true;
        default:
          return base.IsInputKey(keyData);
      }
    }

    public static SortOrder FlipSortOrder(SortOrder sortOrder)
    {
      switch (sortOrder)
      {
        case SortOrder.Ascending:
          return SortOrder.Descending;
        case SortOrder.Descending:
          return SortOrder.Ascending;
        default:
          return SortOrder.None;
      }
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.autoHeaderContextMenuStrip = new ContextMenuStrip(this.components);
      this.dummyItem = new ToolStripMenuItem();
      this.autoViewContextMenuStrip = new ContextMenuStrip(this.components);
      this.miViewMode = new ToolStripMenuItem();
      this.miViewThumbs = new ToolStripMenuItem();
      this.miViewTiles = new ToolStripMenuItem();
      this.miViewDetails = new ToolStripMenuItem();
      this.miArrange = new ToolStripMenuItem();
      this.miGroup = new ToolStripMenuItem();
      this.miStack = new ToolStripMenuItem();
      this.longClickTimer = new Timer(this.components);
      this.toolTip = new ToolTip(this.components);
      this.autoHeaderContextMenuStrip.SuspendLayout();
      this.autoViewContextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      this.autoHeaderContextMenuStrip.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.dummyItem
      });
      this.autoHeaderContextMenuStrip.Name = "autoHeaderContextMenuStrip";
      this.autoHeaderContextMenuStrip.Size = new System.Drawing.Size(181, 26);
      this.autoHeaderContextMenuStrip.Opening += new CancelEventHandler(this.autoHeaderContextMenuStrip_Opening);
      this.dummyItem.Name = "dummyItem";
      this.dummyItem.Size = new System.Drawing.Size(180, 22);
      this.dummyItem.Text = "toolStripMenuItem1";
      this.autoViewContextMenuStrip.Items.AddRange(new ToolStripItem[4]
      {
        (ToolStripItem) this.miViewMode,
        (ToolStripItem) this.miArrange,
        (ToolStripItem) this.miGroup,
        (ToolStripItem) this.miStack
      });
      this.autoViewContextMenuStrip.Name = "autoViewContextMenuStrip";
      this.autoViewContextMenuStrip.Size = new System.Drawing.Size(133, 92);
      this.autoViewContextMenuStrip.Opening += new CancelEventHandler(this.autoViewContextMenuStrip_Opening);
      this.miViewMode.DropDownItems.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.miViewThumbs,
        (ToolStripItem) this.miViewTiles,
        (ToolStripItem) this.miViewDetails
      });
      this.miViewMode.Name = "miViewMode";
      this.miViewMode.Size = new System.Drawing.Size(132, 22);
      this.miViewMode.Text = "View";
      this.miViewThumbs.Name = "miViewThumbs";
      this.miViewThumbs.Size = new System.Drawing.Size(137, 22);
      this.miViewThumbs.Text = "Thumbnails";
      this.miViewTiles.Name = "miViewTiles";
      this.miViewTiles.Size = new System.Drawing.Size(137, 22);
      this.miViewTiles.Text = "Tiles";
      this.miViewDetails.Name = "miViewDetails";
      this.miViewDetails.Size = new System.Drawing.Size(137, 22);
      this.miViewDetails.Text = "Details";
      this.miArrange.Name = "miArrange";
      this.miArrange.Size = new System.Drawing.Size(132, 22);
      this.miArrange.Text = "Arrange by";
      this.miGroup.Name = "miGroup";
      this.miGroup.Size = new System.Drawing.Size(132, 22);
      this.miGroup.Text = "Group by";
      this.miStack.Name = "miStack";
      this.miStack.Size = new System.Drawing.Size(132, 22);
      this.miStack.Text = "Stack by";
      this.longClickTimer.Interval = 1000;
      this.longClickTimer.Tick += new EventHandler(this.longClickTimer_Tick);
      this.BackColor = SystemColors.Window;
      this.Size = new System.Drawing.Size(624, 600);
      this.autoHeaderContextMenuStrip.ResumeLayout(false);
      this.autoViewContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    private class DisplayItem
    {
      private Rectangle bounds;

      public DisplayItem(Rectangle bounds) => this.bounds = bounds;

      public Rectangle Bounds => this.bounds;

      public void Offset(int x, int y) => this.bounds.Offset(x, y);
    }

    private class ItemInformation : ItemView.DisplayItem
    {
      public ItemInformation(
        IViewableItem item,
        Rectangle bounds,
        int column,
        int row,
        GroupHeaderInformation group)
        : base(bounds)
      {
        this.Item = item;
        this.Column = column;
        this.Row = row;
        this.Group = group;
      }

      public IViewableItem Item { get; private set; }

      public int Column { get; private set; }

      public int Row { get; private set; }

      public GroupHeaderInformation Group { get; private set; }
    }

    private class StateChangedEventArgs : EventArgs
    {
      public StateChangedEventArgs(
        IViewableItem item,
        ItemViewStates oldState,
        ItemViewStates newState)
      {
        this.Item = item;
        this.OldState = oldState;
        this.NewState = newState;
      }

      public IViewableItem Item { get; private set; }

      public ItemViewStates OldState { get; private set; }

      public ItemViewStates NewState { get; private set; }
    }

    private class StateInfo
    {
      private readonly Dictionary<IViewableItem, ItemViewStates> stateDict = new Dictionary<IViewableItem, ItemViewStates>();

      public StateInfo()
      {
      }

      public StateInfo(ItemView.StateInfo org)
      {
        using (ItemMonitor.Lock((object) org))
          this.stateDict = new Dictionary<IViewableItem, ItemViewStates>((IDictionary<IViewableItem, ItemViewStates>) org.stateDict);
      }

      public event EventHandler<ItemView.StateChangedEventArgs> StateChanged;

      public ItemViewStates this[IViewableItem item]
      {
        get
        {
          using (ItemMonitor.Lock((object) this.stateDict))
          {
            ItemViewStates itemViewStates = ItemViewStates.None;
            if (item != null)
              this.stateDict.TryGetValue(item, out itemViewStates);
            return itemViewStates;
          }
        }
        set
        {
          if (item == null)
            return;
          ItemViewStates oldState = this[item];
          if (oldState == value)
            return;
          using (ItemMonitor.Lock((object) this.stateDict))
          {
            if (value == ItemViewStates.None)
              this.stateDict.Remove(item);
            else
              this.stateDict[item] = value;
          }
          if (this.StateChanged == null)
            return;
          this.StateChanged((object) this, new ItemView.StateChangedEventArgs(item, oldState, value));
        }
      }

      public void Set(IViewableItem item, ItemViewStates mask, bool on)
      {
        ItemViewStates itemViewStates1 = this[item];
        ItemViewStates itemViewStates2 = !on ? itemViewStates1 & ~mask : itemViewStates1 | mask;
        this[item] = itemViewStates2;
      }

      public void Flip(IViewableItem item, ItemViewStates mask) => this[item] = this[item] ^ mask;

      public void Clear(ItemViewStates mask)
      {
        ((IEnumerable<IViewableItem>) this.GetItems()).ForEach<IViewableItem>((Action<IViewableItem>) (vi => this.Set(vi, mask, false)));
      }

      public void Focus(IViewableItem item)
      {
        this.Clear(ItemViewStates.Focused);
        if (item == null)
          return;
        this.Set(item, ItemViewStates.Focused, true);
      }

      public IViewableItem FindFirst(ItemViewStates mask)
      {
        using (ItemMonitor.Lock((object) this.stateDict))
          return this.stateDict.Keys.FirstOrDefault<IViewableItem>((Func<IViewableItem, bool>) (item => (this[item] & mask) != 0));
      }

      private IViewableItem[] GetItems()
      {
        using (ItemMonitor.Lock((object) this.stateDict))
          return this.stateDict.Keys.ToArray<IViewableItem>();
      }

      public void Update(ItemView.StateInfo si)
      {
        using (ItemMonitor.Lock((object) si.stateDict))
        {
          using (ItemMonitor.Lock((object) this.stateDict))
          {
            foreach (IViewableItem key in this.stateDict.Keys.ToArray<IViewableItem>())
            {
              if (!si.stateDict.ContainsKey(key))
                this[key] = ItemViewStates.None;
            }
          }
          foreach (IViewableItem key in si.stateDict.Keys)
            this[key] = si[key];
        }
      }
    }

    public class StackInfo
    {
      private readonly GroupContainer<IViewableItem> container;

      public StackInfo(GroupContainer<IViewableItem> groupContainer)
      {
        this.container = groupContainer;
      }

      public string Text => this.GroupInfo.Caption;

      public object Key => this.GroupInfo.Key;

      public List<IViewableItem> Items => this.container.Items;

      public IGroupInfo GroupInfo => this.container.Info;
    }

    public class StackEventArgs : EventArgs
    {
      private readonly ItemView.StackInfo stack;

      public StackEventArgs(ItemView.StackInfo stack) => this.stack = stack;

      public ItemView.StackInfo Stack => this.stack;
    }

    private class AlphabetGrouper : IGrouper<IViewableItem>
    {
      private readonly IGrouper<IViewableItem> grouper;
      private readonly Dictionary<object, IGroupInfo> groupDict = new Dictionary<object, IGroupInfo>();

      public AlphabetGrouper(IGrouper<IViewableItem> grouper) => this.grouper = grouper;

      public bool IsMultiGroup => false;

      public IEnumerable<IGroupInfo> GetGroups(IViewableItem item)
      {
        throw new NotImplementedException();
      }

      public IGroupInfo GetGroup(IViewableItem item)
      {
        IGroupInfo group = this.grouper.GetGroup(item);
        using (ItemMonitor.Lock((object) this.groupDict))
        {
          if (!this.groupDict.ContainsKey(group.Key))
            this.groupDict[group.Key] = GroupInfo.GetAlphabetGroup(group.Caption, true);
          return this.groupDict[group.Key];
        }
      }
    }
  }
}

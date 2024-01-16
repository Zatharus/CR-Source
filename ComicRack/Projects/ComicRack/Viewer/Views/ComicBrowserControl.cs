// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicBrowserControl
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Plugins;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Dialogs;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicBrowserControl : 
    SubView,
    IComicBrowser,
    IGetBookList,
    IRefreshDisplay,
    ISearchOptions,
    IDisplayWorkspace,
    IItemSize,
    ISettingsChanged
  {
    private readonly string noneText;
    private readonly string arrangedByText;
    private readonly string notArrangedText;
    private readonly string groupedByText;
    private readonly string notGroupedText;
    private readonly string stackedByText;
    private readonly string notStackedText;
    private readonly string eComicText;
    private readonly string eComicsText;
    private readonly string selectedText;
    private readonly string filteredText;
    private readonly string customFieldDescription;
    private volatile bool bookListDirty;
    private volatile bool updateGroupList;
    private volatile int newGroupListWidth;
    private volatile int totalCount;
    private long totalSize;
    private long selectedSize;
    private readonly CommandMapper commands = new CommandMapper();
    private readonly Image groupUp = (Image) Resources.GroupUp;
    private readonly Image groupDown = (Image) Resources.GroupDown;
    private readonly Image sortUp = (Image) Resources.SortUp;
    private readonly Image sortDown = (Image) Resources.SortDown;
    private ToolStripMenuItem miCopyListSetup;
    private ToolStripMenuItem miPasteListSetup;
    private ComicBookMatcher quickFilter;
    private ComicBookMatcher stackFilter;
    private IViewableItem stackItem;
    private StacksConfig stacksConfig;
    private ItemViewConfig preStackConfig;
    private System.Drawing.Point preStackScrollPosition;
    private Guid preStackFocusedId;
    private string currentStackName;
    private IComicBookListProvider bookList;
    private string quickSearch;
    private ComicBookAllPropertiesMatcher.MatcherOption quickSearchType;
    private ComicBookAllPropertiesMatcher.ShowOptionType showOptionType;
    private ComicBookAllPropertiesMatcher.ShowComicType showComicType;
    private bool showOnlyDuplicates;
    private bool showGroupHeaders;
    private ComicsEditModes comicEditMode = ComicsEditModes.Default;
    private ThumbnailConfig thumbnailConfig;
    private Image listBackgroundImage;
    private IGrouper<IViewableItem> oldStacker;
    private string[] quickSearchCueTexts;
    private System.Drawing.Point contextMenuMouseLocation;
    private long contextMenuCloseTime;
    private int oldQuickWidth;
    private int savedOptimizeToolstripRight;
    private int savedOptimizeToolstripWitdh;
    private string backgroundImageSource;
    private IBitmapCursor dragCursor;
    private bool ownDrop;
    private DragDropContainer dragBookContainer;
    private readonly ManualResetEvent abortBuildMenu = new ManualResetEvent(false);
    private Thread buildMenuThread;
    private bool blockQuickSearchUpdate;
    private CoverViewItem toolTipItem;
    private bool searchBrowserVisible;
    private ComicLibrary library;
    private IContainer components;
    private ItemView itemView;
    private SearchBrowserControl bookSelectorPanel;
    private ToolStrip toolStrip;
    private ToolStripSplitButton tbbView;
    private ToolStripMenuItem miViewThumbnails;
    private ToolStripMenuItem miViewTiles;
    private ToolStripMenuItem miViewDetails;
    private ToolStripSplitButton tbbSort;
    private ToolStripSplitButton tbbGroup;
    private ContextMenuStrip contextRating;
    private ToolStripMenuItem miRating0;
    private ToolStripMenuItem miRating1;
    private ToolStripMenuItem miRating2;
    private ToolStripMenuItem miRating3;
    private ToolStripMenuItem miRating4;
    private ToolStripMenuItem miRating5;
    private ToolStripMenuItem miRateMenu;
    private ContextMenuStrip contextMarkAs;
    private ToolStripMenuItem miMarkRead;
    private ToolStripMenuItem miMarkUnread;
    private ToolStripMenuItem miMarkAs;
    private System.Windows.Forms.ToolTip toolTip;
    private ContextMenuStrip contextMenuItems;
    private ToolStripMenuItem miRead;
    private ToolStripMenuItem miProperties;
    private ToolStripMenuItem miRevealBrowser;
    private ToolStripMenuItem miRefreshInformation;
    private ToolStripSeparator tsMarkAsSeparator;
    private ToolStripSeparator tsCopySeparator;
    private ToolStripMenuItem miSelectAll;
    private ToolStripMenuItem miInvertSelection;
    private ToolStripSeparator toolStripRemoveSeparator;
    private ToolStripMenuItem miRemove;
    private System.Windows.Forms.Timer quickSearchTimer;
    private ToolStripMenuItem miShowOnly;
    private ToolStripMenuItem miAutomation;
    private ToolStripSearchTextBox tsQuickSearch;
    private ContextMenuStrip contextQuickSearch;
    private ToolStripMenuItem miSearchAll;
    private ToolStripSeparator toolStripSeparator5;
    private ToolStripMenuItem miSearchSeries;
    private ToolStripMenuItem miSearchWriter;
    private ToolStripMenuItem miSearchArtists;
    private ToolStripMenuItem miSearchDescriptive;
    private ToolStripMenuItem miSearchFile;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem miShowOnlyAllComics;
    private ToolStripMenuItem miShowOnlyUnreadComics;
    private ToolStripMenuItem miShowOnlyReadingComics;
    private ToolStripMenuItem miShowOnlyReadComics;
    private ToolStripSeparator toolStripMenuItem3;
    private ToolStripMenuItem miAddLibrary;
    private ToolStripSplitButton tbbStack;
    private SizableContainer searchBrowserContainer;
    private ToolStripMenuItem miReadTab;
    private ToolStripSeparator toolStripMenuItem5;
    private ToolStripMenuItem miCopyData;
    private ToolStripMenuItem miPasteData;
    private ToolStripMenuItem miSetTopOfStack;
    private ToolStripDropDownButton tsListLayouts;
    private ToolStripMenuItem tsSaveListLayout;
    private ToolStripMenuItem tsEditLayouts;
    private ToolStripSeparator toolStripMenuItem23;
    private ToolStripMenuItem tsEditListLayout;
    private ToolStripMenuItem miExportComics;
    private ToolStripSeparator toolStripMenuItem7;
    private ToolStripMenuItem dummyToolStripMenuItem;
    private ToolStripMenuItem dummyToolStripMenuItem1;
    private ToolStripMenuItem dummyToolStripMenuItem2;
    private ToolStripMenuItem miSetListBackground;
    private ToolStripSeparator sepListBackground;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem miShowOnlyDuplicates;
    private Panel displayOptionPanel;
    private Label lblDisplayOptionText;
    private System.Windows.Forms.Button btDisplayAll;
    private ToolStripMenuItem miUpdateComicFiles;
    private Panel openStackPanel;
    private Label lblOpenStackText;
    private System.Windows.Forms.Button btCloseStack;
    private ToolStripMenuItem miAddList;
    private ToolStripMenuItem dummyEntryToolStripMenuItem;
    private ToolStripMenuItem miShowInList;
    private ToolStripMenuItem dummyEntryToolStripMenuItem1;
    private ToolStripMenuItem miEdit;
    private ToolStripSeparator sepDuplicateList;
    private ToolStripSeparator sepUndo;
    private ToolStripButton tbUndo;
    private ToolStripButton tbRedo;
    private System.Windows.Forms.Button btPrevStack;
    private System.Windows.Forms.Button btNextStack;
    private ToolStripMenuItem miResetListBackground;
    private ToolStripSeparator separatorListLayout;
    private ContextMenuStrip contextExport;
    private ToolStripMenuItem miExportComicsAs;
    private ToolStripMenuItem miExportComicsWithPrevious;
    private ToolStripSeparator toolStripMenuItem4;
    private ToolStripMenuItem miShowOnlyComics;
    private ToolStripMenuItem miShowOnlyFileless;
    private ToolStripButton btBrowsePrev;
    private ToolStripButton btBrowseNext;
    private ToolStripSeparator tbBrowseSeparator;
    private ToolStripMenuItem miSearchCatalog;
    private ToolStripSeparator toolStripMenuItem6;
    private ToolStripMenuItem miExpandAllGroups;
    private ToolStripMenuItem miSetStackThumbnail;
    private ToolStripMenuItem miRemoveStackThumbnail;
    private ToolStripButton tbSidebar;
    private ToolStripMenuItem miEditList;
    private ToolStripMenuItem miEditListMoveToTop;
    private ToolStripMenuItem miEditListMoveToBottom;
    private ToolStripSeparator toolStripMenuItem8;
    private ToolStripMenuItem miEditListApplyOrder;
    private ToolStripMenuItem miClearData;
    private ToolStripMenuItem miShowWeb;
    private ToolStripSeparator toolStripMenuItem9;
    private ToolStripMenuItem miMarkChecked;
    private ToolStripMenuItem miMarkUnchecked;
    private ToolStripDropDownButton tbbDuplicateList;
    private ToolStripMenuItem dummyEntryToolStripMenuItem2;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem miQuickRating;
    private ListViewEx lvGroupHeaders;
    private ToolStripMenuItem miShowGroupHeaders;
    private SplitContainer browserContainer;
    private ColumnHeader lvGroupsName;
    private ColumnHeader lvGroupsCount;

    public ComicBrowserControl()
    {
      this.InitializeComponent();
      this.searchBrowserContainer.Collapsed = true;
      this.itemView.ScrollResizeRefresh = Program.ExtendedSettings.OptimizedListScrolling;
      this.toolTip.OwnerDraw = true;
      this.toolTip.Draw += new DrawToolTipEventHandler(this.toolTip_Draw);
      ListStyles.SetOwnerDrawn((System.Windows.Forms.ListView) this.lvGroupHeaders);
      KeySearch.Create((System.Windows.Forms.ListView) this.lvGroupHeaders);
      this.browserContainer.Panel2Collapsed = true;
      LocalizeUtility.Localize((Control) this, this.components);
      this.noneText = TR.Load(this.Name)["None", "None"];
      this.arrangedByText = TR.Load(this.Name)["ArrangedBy", "Arranged by {0}"];
      this.notArrangedText = TR.Load(this.Name)["NotArranged", "Not sorted"];
      this.groupedByText = TR.Load(this.Name)["GroupedBy", "Grouped by {0}"];
      this.notGroupedText = TR.Load(this.Name)["NotGrouped", "Not grouped"];
      this.stackedByText = TR.Load(this.Name)["StackedBy", "Stacked by {0}"];
      this.notStackedText = TR.Load(this.Name)["NotStacked", "Not stacked"];
      this.eComicText = TR.Load(this.Name)["ComicSingle", "{0} Book"];
      this.eComicsText = TR.Load(this.Name)["ComicMulti", "{0} Books"];
      this.selectedText = TR.Load(this.Name)["ComicSelected", "{0} selected"];
      this.filteredText = TR.Load(this.Name)["ComicFiltered", "{0} filtered"];
      this.customFieldDescription = TR.Load(this.Name)["CustomFieldDescription", "Shows the value of the custom field '{0}'"];
      this.contextRating.Renderer = (ToolStripRenderer) new MenuRenderer((Image) Resources.StarYellow);
      this.contextRating.Items.Insert(this.contextRating.Items.Count - 2, (ToolStripItem) new ToolStripSeparator());
      RatingControl.InsertRatingControl(this.contextRating, this.contextRating.Items.Count - 2, (Image) Resources.StarYellow, (Func<IEditRating>) (() => this.Main.GetRatingEditor()));
      FormUtility.EnableRightClickSplitButtons(this.toolStrip.Items);
      string[] strings = TR.Load("Columns").GetStrings("DateFormats", "Long|Short|Relative", '|');
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(101, "State", 60, (object) new ComicListField("State", "State indicators for the Book"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookStatusComparer>()));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(100, "Position", 30, (object) new ComicListField("Position", "Position of the Book in the list"), (IComparer<IViewableItem>) new CoverViewItemPositionComparer(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(102, "Checked", 22, (object) new ComicListField("Checked", "Book is checked"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookCheckedComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupChecked>()));
      ItemViewColumnCollection<IColumn> columns = this.itemView.Columns;
      ItemViewColumn itemViewColumn = new ItemViewColumn(0, "Cover", 40, (object) new ComicListField("Cover", "Cover image of the Book"), align: StringAlignment.Center);
      itemViewColumn.Name = "Cover";
      columns.Add((IColumn) itemViewColumn);
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(1, "Series", 200, (object) new ComicListField("Series", "Series name", "Series"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookSeriesComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupSeries>()));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(2, "Number", 40, (object) new ComicListField("NumberAsText", "Number of the Book in the Series", "Number"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookNumberComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupNumber>(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(3, "Volume", 40, (object) new ComicListField("VolumeAsText", "Volume number of the Series", "Volume"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookVolumeComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupVolume>()));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(4, "Title", 200, (object) new ComicListField("Title", "Title of the Book", "Title"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookTitleComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupTitle>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(5, "Opened", 40, (object) new ComicListField("OpenedTime", "Last time the Book was opened", trimming: StringTrimming.Word, valueType: typeof (DateTime)), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookOpenedComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupOpened>(), align: StringAlignment.Far, formatTexts: strings));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(6, "Added", 40, (object) new ComicListField("AddedTime", "Time the Book was added to the Database", trimming: StringTrimming.Word, valueType: typeof (DateTime)), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookAddedComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupAdded>(), align: StringAlignment.Far, formatTexts: strings));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(7, "Pages", 40, (object) new ComicListField("PagesAsTextSimple", "The total count of pages"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookPageCountComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupPages>(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(39, "Published", 40, (object) new ComicListField("PublishedAsText", "Publication date of the Book"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookPublishedComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupPublished>(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(9, "File Path", 200, (object) new ComicListField("FilePath", "Location where the Book is stored", trimming: StringTrimming.EllipsisPath), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookFileComparer>(), visible: false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(10, "File Name", 200, (object) new ComicListField("FileName", "Filename without the extension", "FileName"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookFileNameComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupFileName>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(11, "Writer", 80, (object) new ComicListField("Writer", "Writer of the Book", "Writer"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookWriterComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupWriter>()));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(12, "Penciller", 80, (object) new ComicListField("Penciller", "Penciller of the Book", "Penciller"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookPencillerComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupPenciller>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(13, "Inker", 80, (object) new ComicListField("Inker", "Inker of the Book", "Inker"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookInkerComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupInker>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(14, "Colorist", 80, (object) new ComicListField("Colorist", "Colorist of the Book", "Colorist"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookColoristComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupColorist>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(15, "My Rating", 50, (object) new ComicListField("Rating", "Your rating of the Book"), (IComparer<IViewableItem>) new CoverViewItemRatingComparer(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupRating>()));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(16, "Opened Count", 40, (object) new ComicListField("OpenedCountAsText", "Times the Book has been opened"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookOpenCountComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupOpenCount>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(17, "Read Percentage", 40, (object) new ComicListField("ReadPercentageAsText", "How much has been read"), (IComparer<IViewableItem>) new CoverViewItemReadPercentageComparer(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupReadPercentage>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(18, "File Modified", 40, (object) new ComicListField("FileModifiedTime", "Time the Book was modified the last time", trimming: StringTrimming.Word, valueType: typeof (DateTime)), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookModifiedComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupModified>(), false, StringAlignment.Far, strings));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(19, "Genre", 40, (object) new ComicListField("Genre", "Genre of the Book", "Genre"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookGenreComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupGenre>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(20, "Publisher", 40, (object) new ComicListField("Publisher", "Publisher of the Book", "Publisher"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookPublisherComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupPublisher>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(21, "Count", 40, (object) new ComicListField("CountAsText", "Total number of issues in this series", "Count"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookCountComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupCount>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(22, "Letterer", 80, (object) new ComicListField("Letterer", "Letterer of the Book", "Letterer"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookLettererComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupLetterer>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(23, "Cover Artist", 80, (object) new ComicListField("CoverArtist", "The artist responsible for the cover", "CoverArtist"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookCoverArtistComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupCoverArtist>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(24, "Editor", 80, (object) new ComicListField("Editor", "Editor of the Book", "Editor"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookEditorComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupEditor>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(25, "File Size", 40, (object) new ComicListField("FileSizeAsText", "Size of the Book file in bytes"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookSizeComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupSize>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(26, "Alternate Series", 200, (object) new ComicListField("AlternateSeries", "Crossover/story name", "AlternateSeries"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookAlternateSeriesComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupAlternateSeries>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(27, "Alternate Number", 40, (object) new ComicListField("AlternateNumberAsText", "Issue number in the crossover/story", "AlternateNumber"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookAlternateNumberComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupAlternateNumber>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(28, "Alternate Count", 40, (object) new ComicListField("AlternateCountAsText", "Total issues of the crossover/story", "AlternateCount"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookAlternateCountComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupAlternateCount>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(29, "Month", 40, (object) new ComicListField("MonthAsText", "Month the Book was published", "Month"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookMonthComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupMonth>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(30, "Caption", 200, (object) new ComicListField("Caption", "Complete caption of the Book", "Series"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookSeriesComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupSeries>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(31, "Tags", 60, (object) new ComicListField("Tags", "Tags of the Book", "Tags"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookTagsComparer>(), visible: false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(32, "Imprint", 40, (object) new ComicListField("Imprint", "Imprint of the Book", "Imprint"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookImprintComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupImprint>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(33, "Language", 40, (object) new ComicListField("LanguageAsText", "Language of the Book"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookLanguageComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupLanguage>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(34, "Format", 40, (object) new ComicListField("Format", "Format of the Book", "Format"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookFormatComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupFormat>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(35, "B&W", 22, (object) new ComicListField("BlackAndWhiteAsText", "Yes if the Book is black and white"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBlackAndWhiteComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupBlackAndWhite>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(36, "Manga", 22, (object) new ComicListField("MangaAsText", "Yes if the Book is a Manga"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookMangaComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupManga>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(37, "File Format", 40, (object) new ComicListField("FileFormat", "File format of the Book"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookFileFormatComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupFileFormat>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(38, "Age Rating", 40, (object) new ComicListField("AgeRating", "Age rating of the Book"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookAgeRatingComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupAgeRating>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(8, "Year", 60, (object) new ComicListField("YearAsText", "Year the Book was published", "Year"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookYearComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupYear>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(40, "Characters", 60, (object) new ComicListField("Characters", "Plot characters of the Books", "Characters"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookCharactersComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupCharacters>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(41, "File Directory", 60, (object) new ComicListField("FileDirectory", "Directory of the Book"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookDirectoryComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupDirectory>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(42, "File Created", 60, (object) new ComicListField("FileCreationTime", "Time the Book has been created", trimming: StringTrimming.Word, valueType: typeof (DateTime)), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookCreationComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupCreation>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(43, "Bookmark Count", 60, (object) new ComicListField("BookmarkCountAsText", "Count of Bookmarks for this Book"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBookmarkCountComparer>(), visible: false, align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(44, "New Pages", 60, (object) new ComicListField("NewPagesAsText", "Count of new Pages"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookNewPagesComparer>(), visible: false, align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(45, "Teams", 60, (object) new ComicListField("Teams", "Plot teams of the Books", "Teams"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookTeamsComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupTeams>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(46, "Locations", 60, (object) new ComicListField("Locations", "Plot locations of the Books", "Locations"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookLocationsComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupLocations>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(47, "Web", 60, (object) new ComicListField("Web", "A Web Link", "Web"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookWebComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupWeb>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(48, "Community Rating", 50, (object) new ComicListField("CommunityRating", "Rating of the Book"), (IComparer<IViewableItem>) new CoverViewItemCommunityRatingComparer(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupCommunityRating>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(49, "Linked", 50, (object) new ComicListField("IsLinkedAsText", "Book is linked to a file"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookLinkedComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupLinked>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(50, "Book Price", 50, (object) new ComicListField("BookPriceAsText", "Price of the Book", "BookPriceAsText"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBookPriceComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupBookPrice>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(51, "Book Age", 50, (object) new ComicListField("BookAge", "Age of the Book", "BookAge"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBookAgeComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupBookAge>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(52, "Book Store", 50, (object) new ComicListField("BookStore", "Store the Book was bought", "BookStore"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBookStoreComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupBookStore>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(53, "Book Owner", 50, (object) new ComicListField("BookOwner", "Owner of the Book", "BookOwner"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBookOwnerComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupBookOwner>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(54, "Book Condition", 50, (object) new ComicListField("BookCondition", "Condition of the Book", "BookCondition"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBookConditionComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupBookCondition>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(55, "Book Collection Status", 50, (object) new ComicListField("BookCollectionStatus", "Status of the Book in the Collection", "BookCollectionStatus"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBookCollectionStatusComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupBookCollectionStatus>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(56, "Book Location", 50, (object) new ComicListField("BookLocation", "Location of the Book", "BookLocation"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookBookLocationComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupBookLocation>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(57, "ISBN", 50, (object) new ComicListField("ISBN", "ISBN Number", "ISBN"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookISBNComparer>(), visible: false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(58, "Series complete", 22, (object) new ComicListField("SeriesCompleteAsText", "Series is complete"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookSeriesCompleteComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupSeriesComplete>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(59, "Proposed Values", 22, (object) new ComicListField("EnableProposedAsText", "Uses proposed Values from Filename"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookSeriesEnableProposedComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupEnableProposed>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(60, "Gap Information", 16, (object) new ComicListField("GapInformation", "Information if this issue is the end or start of a gap in the series"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookSeriesComparer>(), visible: false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(61, "Read", 22, (object) new ComicListField("HasBeenReadAsText", "Book has been read"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookHasBeenReadComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupHasBeenRead>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(62, "Icons", 100, (object) new ComicListField("Icons", "Icons for this Book"), visible: false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(63, "Scan Information", 100, (object) new ComicListField("ScanInformation", "Information about the Scan", "ScanInformation"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookScanInformationComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupScanInformation>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(64, "Story Arc", 100, (object) new ComicListField("StoryArc", "Story Arc the book is part of", "StoryArc"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookStoryArcComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupStoryArc>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(65, "Series Group", 100, (object) new ComicListField("SeriesGroup", "Series Group the Book is part of", "SeriesGroup"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookSeriesGroupComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupSeriesGroup>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(66, "Main Character/Team", 100, (object) new ComicListField("MainCharacterOrTeam", "Main Character or Team of the Book", "MainCharacterOrTeam"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookMainCharacterOrTeamComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupMainCharacterOrTeam>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(67, "Review", 100, (object) new ComicListField("Review", "A short Review of the Book"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookReviewComparer>(), visible: false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(68, "Day", 40, (object) new ComicListField("DayAsText", "Day the Book was published", "Day"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookDayComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupDay>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(69, "Week", 40, (object) new ComicListField("WeekAsText", "Week the Book was published"), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookWeekComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupWeek>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(70, "Released", 60, (object) new ComicListField("ReleasedTime", "Time the Book was released", trimming: StringTrimming.Word, valueType: typeof (DateTime), defaultText: ComicBook.TR["Unknown"]), (IComparer<IViewableItem>) new CoverViewItemBookComparer<ComicBookReleasedComparer>(), (IGrouper<IViewableItem>) new CoverViewItemBookGrouper<ComicBookGroupReleased>(), false, StringAlignment.Far, strings));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(200, "Series: Books", 50, (object) new ComicListField("SeriesStatCountAsText", "Books in the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsCountComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupCount>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(201, "Series: Pages", 50, (object) new ComicListField("SeriesStatPageCountAsText", "Pages in the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsPageCountComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupPageCount>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(202, "Series: Pages Read", 50, (object) new ComicListField("SeriesStatPageReadCountAsText", "Pages of the Series read"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsPageReadCountComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupPageReadCount>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(203, "Series: Percent Read", 50, (object) new ComicListField("SeriesStatReadPercentageAsText", "Percentage of the Series read"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsReadPercentageComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupReadPercentage>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(204, "Series: First Number", 50, (object) new ComicListField("SeriesStatMinNumberAsText", "First Number of the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsMinNumberComparer>(), visible: false, align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(205, "Series: Last Number", 50, (object) new ComicListField("SeriesStatMaxNumberAsText", "Last Number of the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsMaxNumberComparer>(), visible: false, align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(206, "Series: First Year", 50, (object) new ComicListField("SeriesStatMinYearAsText", "First Year of the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsMinYearComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupMinYear>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(207, "Series: Last Year", 50, (object) new ComicListField("SeriesStatMaxYearAsText", "Last Year of the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsMaxYearComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupMaxYear>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(208, "Series: Average Rating", 50, (object) new ComicListField("SeriesStatAverageRating", "Average Rating of the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsAverageRatingComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupAverageRating>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(209, "Series: Average Community Rating", 50, (object) new ComicListField("SeriesStatAverageCommunityRating", "Average Community Rating of the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsAverageCommunityRatingComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupAverageCommunityRating>(), false));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(210, "Series: Gaps", 50, (object) new ComicListField("SeriesStatGapCountAsText", "Count of Gaps in the Series"), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsGapCountComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupGapCount>(), false, StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(211, "Series: Book added", 50, (object) new ComicListField("SeriesStatLastAddedTime", "Last time a book was added to the Series", trimming: StringTrimming.Word, valueType: typeof (DateTime)), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsLastAddedTimeComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupLastAddedTime>(), false, StringAlignment.Far, strings));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(212, "Series: Opened", 50, (object) new ComicListField("SeriesStatLastOpenedTime", "Last time a book was opened from the Series", trimming: StringTrimming.Word, valueType: typeof (DateTime)), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsLastOpenedTimeComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupLastOpenedTime>(), false, StringAlignment.Far, strings));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(213, "Series: Book released", 50, (object) new ComicListField("SeriesStatLastReleasedTime", "Last time a book of this Series was released", trimming: StringTrimming.Word, valueType: typeof (DateTime)), (IComparer<IViewableItem>) new CoverViewItemStatsComparer<ComicBookSeriesStatsLastReleasedTimeComparer>(), (IGrouper<IViewableItem>) new CoverViewItemStatsGrouper<ComicBookStatsGroupLastReleasedTime>(), false, StringAlignment.Far, strings));
      SubView.TranslateColumns((IEnumerable<IColumn>) this.itemView.Columns);
      foreach (ItemViewColumn column in (SmartList<IColumn>) this.itemView.Columns)
      {
        column.TooltipText = ((ComicListField) column.Tag).Description;
        column.Width = FormUtility.ScaleDpiX(column.Width);
      }
      this.ThumbnailConfig = new ThumbnailConfig();
      this.ThumbnailConfig.CaptionIds.Add(30);
      this.tsQuickSearch.TextBox.SearchButtonImage = (Image) Resources.Search.ScaleDpi();
      this.tsQuickSearch.TextBox.ClearButtonImage = (Image) Resources.SmallCloseGray.ScaleDpi();
      this.itemView.Font = SystemFonts.IconTitleFont;
      this.itemView.ItemContextMenuStrip = this.contextMenuItems;
      this.itemView.ItemRowHeight = this.itemView.Font.Height + FormUtility.ScaleDpiY(6);
      this.itemView.ColumnHeaderHeight = this.itemView.ItemRowHeight;
      this.itemView.Items.Changed += new EventHandler<SmartListChangedEventArgs<IViewableItem>>(this.ComicItemAdded);
      this.itemView.MouseWheel += new MouseEventHandler(this.itemView_MouseWheel);
      KeySearch.Create(this.itemView);
    }

    private void ComicItemAdded(object sender, SmartListChangedEventArgs<IViewableItem> e)
    {
      if (!(e.Item is CoverViewItem coverViewItem))
        return;
      coverViewItem.ThumbnailConfig = this.ThumbnailConfig;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.UnregisterBookList();
        this.bookList = (IComicBookListProvider) null;
        if (this.Main != null)
        {
          this.Main.OpenBooks.BookClosed -= new EventHandler<BookEventArgs>(this.OpenBooksChanged);
          this.Main.OpenBooks.BookOpened -= new EventHandler<BookEventArgs>(this.OpenBooksChanged);
        }
        this.groupUp.Dispose();
        this.groupDown.Dispose();
        this.sortUp.Dispose();
        this.sortDown.Dispose();
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IComicBookListProvider BookList
    {
      get => this.bookList;
      set
      {
        if (this.bookList == value)
          return;
        try
        {
          this.itemView.BeginUpdate();
          this.UnregisterBookList();
          this.bookList = value;
          this.RegisterBookList();
          this.OnCurrentBookListChanged();
          if (this.Main == null)
            return;
          this.UpdatePending();
        }
        finally
        {
          this.itemView.EndUpdate();
          if (this.bookList != null)
          {
            IDisplayListConfig displayListConfig = this.bookList.QueryService<IDisplayListConfig>();
            if (displayListConfig != null && displayListConfig.Display != null)
            {
              this.itemView.ScrollPosition = displayListConfig.Display.ScrollPosition;
              this.SetFocusedItem(displayListConfig.Display.FocusedComicId);
              Guid id = displayListConfig.Display.StackedComicId;
              if (id != Guid.Empty)
              {
                CoverViewItem coverViewItem = this.itemView.Items.OfType<CoverViewItem>().FirstOrDefault<CoverViewItem>((Func<CoverViewItem, bool>) (i => i.Comic.Id == id));
                if (coverViewItem != null)
                {
                  this.itemView.Select((IViewableItem) coverViewItem);
                  this.OpenStack((IViewableItem) coverViewItem);
                  this.itemView.ScrollPosition = displayListConfig.Display.StackScrollPosition;
                  this.itemView.SelectAll(false);
                  this.SetFocusedItem(displayListConfig.Display.StackFocusedComicId);
                }
              }
            }
          }
        }
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemViewConfig ViewConfig
    {
      get => this.itemView.ViewConfig;
      set => this.itemView.ViewConfig = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemViewMode ItemViewMode
    {
      get => this.itemView.ItemViewMode;
      set => this.itemView.ItemViewMode = value;
    }

    [DefaultValue(null)]
    public string QuickSearch
    {
      get => this.quickSearch;
      set
      {
        if (this.quickSearch == value)
          return;
        this.quickSearch = value;
        this.OnQuickSearchChanged();
      }
    }

    public ItemView ItemView => this.itemView;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicBookAllPropertiesMatcher.MatcherOption QuickSearchType
    {
      get => this.quickSearchType;
      set
      {
        if (this.quickSearchType == value)
          return;
        this.quickSearchType = value;
        this.UpdateSearch();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicBookAllPropertiesMatcher.ShowOptionType ShowOptionType
    {
      get => this.showOptionType;
      set
      {
        if (this.showOptionType == value)
          return;
        this.showOptionType = value;
        this.UpdateSearch();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicBookAllPropertiesMatcher.ShowComicType ShowComicType
    {
      get => this.showComicType;
      set
      {
        if (this.showComicType == value)
          return;
        this.showComicType = value;
        this.UpdateSearch();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(false)]
    public bool ShowOnlyDuplicates
    {
      get => this.showOnlyDuplicates;
      set
      {
        if (this.showOnlyDuplicates == value)
          return;
        this.showOnlyDuplicates = value;
        this.UpdateSearch();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(false)]
    public bool ShowGroupHeaders
    {
      get => this.showGroupHeaders;
      set
      {
        if (this.showGroupHeaders == value)
          return;
        this.showGroupHeaders = value;
        this.updateGroupList = true;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicsEditModes ComicEditMode
    {
      get => this.comicEditMode;
      set
      {
        if (this.comicEditMode == value)
          return;
        this.comicEditMode = value;
        this.OnComicEditModeChanged();
      }
    }

    [DefaultValue(false)]
    public bool DisableViewConfigUpdate { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int SearchBrowserColumn1
    {
      get => this.bookSelectorPanel.Column1;
      set => this.bookSelectorPanel.Column1 = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int SearchBrowserColumn2
    {
      get => this.bookSelectorPanel.Column2;
      set => this.bookSelectorPanel.Column2 = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int SearchBrowserColumn3
    {
      get => this.bookSelectorPanel.Column3;
      set => this.bookSelectorPanel.Column3 = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ThumbnailConfig ThumbnailConfig
    {
      get => this.thumbnailConfig;
      set
      {
        if (value == null)
          return;
        this.thumbnailConfig = value;
      }
    }

    [DefaultValue(null)]
    public Image ListBackgroundImage
    {
      get => this.listBackgroundImage;
      set
      {
        this.listBackgroundImage = value;
        if (this.itemView.BackgroundImage != null)
          return;
        this.SetListBackgroundImage();
      }
    }

    [DefaultValue(false)]
    public bool HideNavigation { get; set; }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DesignMode)
        return;
      this.commands.Add((CommandHandler) (() => this.QuickSearchType = ComicBookAllPropertiesMatcher.MatcherOption.All), true, (UpdateHandler) (() => this.QuickSearchType == ComicBookAllPropertiesMatcher.MatcherOption.All), (object) this.miSearchAll);
      this.commands.Add((CommandHandler) (() => this.QuickSearchType = ComicBookAllPropertiesMatcher.MatcherOption.Series), true, (UpdateHandler) (() => this.QuickSearchType == ComicBookAllPropertiesMatcher.MatcherOption.Series), (object) this.miSearchSeries);
      this.commands.Add((CommandHandler) (() => this.QuickSearchType = ComicBookAllPropertiesMatcher.MatcherOption.Writer), true, (UpdateHandler) (() => this.QuickSearchType == ComicBookAllPropertiesMatcher.MatcherOption.Writer), (object) this.miSearchWriter);
      this.commands.Add((CommandHandler) (() => this.QuickSearchType = ComicBookAllPropertiesMatcher.MatcherOption.Artists), true, (UpdateHandler) (() => this.QuickSearchType == ComicBookAllPropertiesMatcher.MatcherOption.Artists), (object) this.miSearchArtists);
      this.commands.Add((CommandHandler) (() => this.QuickSearchType = ComicBookAllPropertiesMatcher.MatcherOption.Descriptive), true, (UpdateHandler) (() => this.QuickSearchType == ComicBookAllPropertiesMatcher.MatcherOption.Descriptive), (object) this.miSearchDescriptive);
      this.commands.Add((CommandHandler) (() => this.QuickSearchType = ComicBookAllPropertiesMatcher.MatcherOption.File), true, (UpdateHandler) (() => this.QuickSearchType == ComicBookAllPropertiesMatcher.MatcherOption.File), (object) this.miSearchFile);
      this.commands.Add((CommandHandler) (() => this.QuickSearchType = ComicBookAllPropertiesMatcher.MatcherOption.Catalog), true, (UpdateHandler) (() => this.QuickSearchType == ComicBookAllPropertiesMatcher.MatcherOption.Catalog), (object) this.miSearchCatalog);
      ContextMenuStrip contextMenuStrip = this.itemView.AutoViewContextMenuStrip;
      ToolStripMenuItem toolStripMenuItem1 = (ToolStripMenuItem) contextMenuStrip.Items.Add(TR.Default["Layout", "Layout"]);
      toolStripMenuItem1.DropDown.Opening += new CancelEventHandler(this.LayoutMenuOpening);
      this.miCopyListSetup = toolStripMenuItem1.DropDown.Items.Add(TR.Default["Copy", "Copy"]) as ToolStripMenuItem;
      this.miPasteListSetup = toolStripMenuItem1.DropDown.Items.Add(TR.Default["Paste", "Paste"]) as ToolStripMenuItem;
      contextMenuStrip.Items.Add((ToolStripItem) new ToolStripSeparator());
      ToolStripMenuItem toolStripMenuItem2;
      contextMenuStrip.Items.Add((ToolStripItem) (toolStripMenuItem2 = this.miSelectAll.Clone()));
      ToolStripMenuItem toolStripMenuItem3;
      contextMenuStrip.Items.Add((ToolStripItem) (toolStripMenuItem3 = this.miResetListBackground.Clone()));
      this.tsQuickSearch.TextBox.SearchMenu = (ToolStripDropDown) this.contextQuickSearch;
      this.components.Add((IComponent) this.commands);
      this.commands.Add((CommandHandler) (() => this.Main.Control.InvokeActiveService<IBrowseHistory>((Action<IBrowseHistory>) (t => t.BrowsePrevious()))), (UpdateHandler) (() => this.Main.Control.InvokeActiveService<IBrowseHistory, bool>((Func<IBrowseHistory, bool>) (t => t.CanBrowsePrevious()))), (object) this.btBrowsePrev);
      this.commands.Add((CommandHandler) (() => this.Main.Control.InvokeActiveService<IBrowseHistory>((Action<IBrowseHistory>) (t => t.BrowseNext()))), (UpdateHandler) (() => this.Main.Control.InvokeActiveService<IBrowseHistory, bool>((Func<IBrowseHistory, bool>) (t => t.CanBrowseNext()))), (object) this.btBrowseNext);
      this.commands.Add((CommandHandler) (() => this.ShowOptionType = ComicBookAllPropertiesMatcher.ShowOptionType.All), true, (UpdateHandler) (() => this.ShowOptionType == ComicBookAllPropertiesMatcher.ShowOptionType.All), (object) this.miShowOnlyAllComics);
      this.commands.Add((CommandHandler) (() => this.ShowOptionType = ComicBookAllPropertiesMatcher.ShowOptionType.Unread), true, (UpdateHandler) (() => this.ShowOptionType == ComicBookAllPropertiesMatcher.ShowOptionType.Unread), (object) this.miShowOnlyUnreadComics);
      this.commands.Add((CommandHandler) (() => this.ShowOptionType = ComicBookAllPropertiesMatcher.ShowOptionType.Reading), true, (UpdateHandler) (() => this.ShowOptionType == ComicBookAllPropertiesMatcher.ShowOptionType.Reading), (object) this.miShowOnlyReadingComics);
      this.commands.Add((CommandHandler) (() => this.ShowOptionType = ComicBookAllPropertiesMatcher.ShowOptionType.Read), true, (UpdateHandler) (() => this.ShowOptionType == ComicBookAllPropertiesMatcher.ShowOptionType.Read), (object) this.miShowOnlyReadComics);
      this.commands.Add((CommandHandler) (() => this.ShowOnlyDuplicates = !this.ShowOnlyDuplicates), true, (UpdateHandler) (() => this.ShowOnlyDuplicates), (object) this.miShowOnlyDuplicates);
      this.commands.Add(new CommandHandler(this.ShowWeb), (object) this.miShowWeb);
      this.commands.Add((CommandHandler) (() => this.ShowComicType = this.ShowComicType == ComicBookAllPropertiesMatcher.ShowComicType.Comics ? ComicBookAllPropertiesMatcher.ShowComicType.All : ComicBookAllPropertiesMatcher.ShowComicType.Comics), true, (UpdateHandler) (() => this.ShowComicType == ComicBookAllPropertiesMatcher.ShowComicType.Comics), (object) this.miShowOnlyComics);
      this.commands.Add((CommandHandler) (() => this.ShowComicType = this.ShowComicType == ComicBookAllPropertiesMatcher.ShowComicType.FilelessComics ? ComicBookAllPropertiesMatcher.ShowComicType.All : ComicBookAllPropertiesMatcher.ShowComicType.FilelessComics), true, (UpdateHandler) (() => this.ShowComicType == ComicBookAllPropertiesMatcher.ShowComicType.FilelessComics), (object) this.miShowOnlyFileless);
      this.commands.Add(new CommandHandler(this.itemView.ToggleGroups), (UpdateHandler) (() => this.itemView.AreGroupsVisible), (object) this.miExpandAllGroups);
      this.commands.Add((CommandHandler) (() => this.ShowGroupHeaders = !this.ShowGroupHeaders), true, (UpdateHandler) (() => this.ShowGroupHeaders), (object) this.miShowGroupHeaders);
      this.commands.Add((CommandHandler) (() => this.Main.Control.InvokeActiveService<ISidebar>((Action<ISidebar>) (s => s.Visible = !s.Visible))), true, (UpdateHandler) (() => this.Main.Control.InvokeActiveService<ISidebar, bool>((Func<ISidebar, bool>) (s => s.Visible))), (object) this.tbSidebar);
      this.commands.Add(new CommandHandler(this.OpenComic), new UpdateHandler(this.AllSelectedLinked), (object) this.miRead);
      this.commands.Add(new CommandHandler(this.OpenComicNewTab), new UpdateHandler(this.AllSelectedLinked), (object) this.miReadTab);
      this.commands.Add(new CommandHandler(this.itemView.SelectAll), (object) this.miSelectAll, (object) toolStripMenuItem2);
      this.commands.Add(new CommandHandler(this.itemView.InvertSelection), (object) this.miInvertSelection);
      this.commands.Add(new CommandHandler(this.RemoveBooks), new UpdateHandler(this.CanRemoveBooks), (object) this.miRemove);
      this.commands.Add(new CommandHandler(this.RevealInExplorer), (UpdateHandler) (() => this.ComicEditMode.CanShowComics() && this.AllSelectedLinked()), (object) this.miRevealBrowser);
      this.commands.Add((CommandHandler) (() => this.Main.ShowInfo()), (UpdateHandler) (() => this.itemView.SelectedCount > 0), (object) this.miProperties);
      this.commands.Add(new CommandHandler(this.EditItem), (UpdateHandler) (() => this.ComicEditMode.CanEditProperties() && this.itemView.SelectedCount > 0), (object) this.miEdit);
      this.commands.Add(new CommandHandler(this.RefreshInformation), (object) this.miRefreshInformation);
      this.commands.Add(new CommandHandler(this.AddFilesToLibrary), (UpdateHandler) (() => !this.GetBookList(ComicBookFilterType.NotInLibrary | ComicBookFilterType.IsLocal | ComicBookFilterType.IsNotFileless).IsEmpty<ComicBook>()), (object) this.miAddLibrary);
      this.commands.Add((CommandHandler) (() => this.Main.ConvertComic(this.GetBookList(ComicBookFilterType.IsNotFileless | ComicBookFilterType.CanExport | ComicBookFilterType.Selected, true), (ExportSetting) null)), (UpdateHandler) (() => this.ComicEditMode.CanExport() && this.AnySelectedLinked()), (object) this.miExportComicsAs);
      this.commands.Add((CommandHandler) (() => this.Main.ConvertComic(this.GetBookList(ComicBookFilterType.IsNotFileless | ComicBookFilterType.CanExport | ComicBookFilterType.Selected, true), Program.Settings.CurrentExportSetting)), (UpdateHandler) (() => Program.Settings.CurrentExportSetting != null && this.ComicEditMode.CanExport() && this.AnySelectedLinked()), (object) this.miExportComicsWithPrevious);
      this.commands.Add(new CommandHandler(this.MarkSelectedRead), (object) (bool) (!this.ComicEditMode.CanEditProperties() ? 0 : (this.itemView.SelectedCount > 0 ? 1 : 0)), (object) this.miMarkRead);
      this.commands.Add(new CommandHandler(this.MarkSelectedNotRead), (object) (bool) (!this.ComicEditMode.CanEditProperties() ? 0 : (this.itemView.SelectedCount > 0 ? 1 : 0)), (object) this.miMarkUnread);
      this.commands.Add(new CommandHandler(this.MarkSelectedChecked), (object) (bool) (!this.ComicEditMode.CanEditProperties() ? 0 : (this.itemView.SelectedCount > 0 ? 1 : 0)), (object) this.miMarkChecked);
      this.commands.Add(new CommandHandler(this.MarkSelectedUnchecked), (object) (bool) (!this.ComicEditMode.CanEditProperties() ? 0 : (this.itemView.SelectedCount > 0 ? 1 : 0)), (object) this.miMarkUnchecked);
      this.commands.Add(new CommandHandler(this.SetSelectedComicAsListBackground), new UpdateHandler(this.AllSelectedLinked), (object) this.miSetListBackground);
      this.commands.Add(new CommandHandler(this.ResetListBackgroundImage), (UpdateHandler) (() => this.itemView.BackgroundImage != this.ListBackgroundImage), (object) this.miResetListBackground, (object) toolStripMenuItem3);
      this.commands.Add((CommandHandler) (() => this.MoveBooks(this.GetBookList(ComicBookFilterType.Library | ComicBookFilterType.Selected), false)), (UpdateHandler) (() => this.CanReorderList()), (object) this.miEditListMoveToTop);
      this.commands.Add((CommandHandler) (() => this.MoveBooks(this.GetBookList(ComicBookFilterType.Library | ComicBookFilterType.Selected), true)), (UpdateHandler) (() => this.CanReorderList()), (object) this.miEditListMoveToBottom);
      this.commands.Add((CommandHandler) (() => this.ApplyBookOrder(this.GetBookList(ComicBookFilterType.Library))), (UpdateHandler) (() => this.CanReorderList(false)), (object) this.miEditListApplyOrder);
      this.commands.Add(new CommandHandler(this.PasteComicData), (UpdateHandler) (() => this.ComicEditMode.CanEditProperties() && Clipboard.ContainsData("ComicBook") && !this.GetBookList(ComicBookFilterType.Selected).IsEmpty<ComicBook>()), (object) this.miPasteData);
      this.commands.Add(new CommandHandler(this.CopyComicData), (UpdateHandler) (() => this.itemView.SelectedCount > 0), (object) this.miCopyData);
      this.commands.Add(new CommandHandler(this.ClearComicData), (UpdateHandler) (() => this.ComicEditMode.CanEditProperties() && !this.GetBookList(ComicBookFilterType.Selected).IsEmpty<ComicBook>()), (object) this.miClearData);
      this.commands.Add(new CommandHandler(this.UpdateFiles), new UpdateHandler(this.AnySelectedLinked), (object) this.miUpdateComicFiles);
      this.commands.Add((CommandHandler) (() => this.itemView.ItemViewMode = ItemViewMode.Thumbnail), true, (UpdateHandler) (() => this.itemView.ItemViewMode == ItemViewMode.Thumbnail), (object) this.miViewThumbnails);
      this.commands.Add((CommandHandler) (() => this.itemView.ItemViewMode = ItemViewMode.Tile), true, (UpdateHandler) (() => this.itemView.ItemViewMode == ItemViewMode.Tile), (object) this.miViewTiles);
      this.commands.Add((CommandHandler) (() => this.itemView.ItemViewMode = ItemViewMode.Detail), true, (UpdateHandler) (() => this.itemView.ItemViewMode == ItemViewMode.Detail), (object) this.miViewDetails);
      this.commands.Add((CommandHandler) (() => this.Main.GetRatingEditor().SetRating(0.0f)), (UpdateHandler) (() => this.Main.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.Main.GetRatingEditor().GetRating()) == 0.0), (object) this.miRating0);
      this.commands.Add((CommandHandler) (() => this.Main.GetRatingEditor().SetRating(1f)), (UpdateHandler) (() => this.Main.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.Main.GetRatingEditor().GetRating()) == 1.0), (object) this.miRating1);
      this.commands.Add((CommandHandler) (() => this.Main.GetRatingEditor().SetRating(2f)), (UpdateHandler) (() => this.Main.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.Main.GetRatingEditor().GetRating()) == 2.0), (object) this.miRating2);
      this.commands.Add((CommandHandler) (() => this.Main.GetRatingEditor().SetRating(3f)), (UpdateHandler) (() => this.Main.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.Main.GetRatingEditor().GetRating()) == 3.0), (object) this.miRating3);
      this.commands.Add((CommandHandler) (() => this.Main.GetRatingEditor().SetRating(4f)), (UpdateHandler) (() => this.Main.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.Main.GetRatingEditor().GetRating()) == 4.0), (object) this.miRating4);
      this.commands.Add((CommandHandler) (() => this.Main.GetRatingEditor().SetRating(5f)), (UpdateHandler) (() => this.Main.GetRatingEditor().IsValid()), (UpdateHandler) (() => Math.Round((double) this.Main.GetRatingEditor().GetRating()) == 5.0), (object) this.miRating5);
      this.commands.Add((CommandHandler) (() => this.Main.GetRatingEditor().QuickRatingAndReview()), (UpdateHandler) (() => this.Main.GetRatingEditor().IsValid()), (object) this.miQuickRating);
      this.commands.Add(new CommandHandler(this.CopyListSetup), (object) this.miCopyListSetup);
      this.commands.Add(new CommandHandler(this.PasteListSetup), (object) this.miPasteListSetup);
      this.commands.Add(new CommandHandler(this.SetTopOfStack), (UpdateHandler) (() => this.itemView.SelectedCount == 1), (object) this.miSetTopOfStack);
      this.commands.Add(new CommandHandler(this.SetStackThumbnail), (object) true, (object) this.miSetStackThumbnail);
      this.commands.Add(new CommandHandler(this.RemoveStackThumbnail), (object) true, (object) this.miRemoveStackThumbnail);
      this.miAutomation.DropDownItems.AddRange((ToolStripItem[]) ScriptUtility.CreateToolItems<ToolStripMenuItem>((Control) this, "Books", (Func<IEnumerable<ComicBook>>) (() => this.GetBookList(ComicBookFilterType.Selected))).ToArray<ToolStripMenuItem>());
      this.miAutomation.Visible = this.miAutomation.DropDownItems.Count != 0;
      List<ToolStripItem> toolStripItemList = new List<ToolStripItem>();
      toolStripItemList.AddRange((IEnumerable<ToolStripItem>) ScriptUtility.CreateToolItems<ToolStripButton>((Control) this, "Books", (Func<IEnumerable<ComicBook>>) (() => this.GetBookList(ComicBookFilterType.Selected)), (Func<Command, bool>) (c => c.Image != null && c.Configure == null)));
      toolStripItemList.AddRange((IEnumerable<ToolStripItem>) ScriptUtility.CreateToolItems<ToolStripSplitButton>((Control) this, "Books", (Func<IEnumerable<ComicBook>>) (() => this.GetBookList(ComicBookFilterType.Selected)), (Func<Command, bool>) (c => c.Image != null && c.Configure != null)));
      if (toolStripItemList.Count != 0)
      {
        toolStripItemList.ForEach((Action<ToolStripItem>) (bt => bt.DisplayStyle = ToolStripItemDisplayStyle.Image));
        this.toolStrip.Items.Add((ToolStripItem) new ToolStripSeparator());
        this.toolStrip.Items.AddRange(toolStripItemList.ToArray());
      }
      this.sepDuplicateList.Visible = this.tbbDuplicateList.Visible = this.Library != null && this.Library.EditMode.CanEditList();
      this.miShowInList.Visible = this.Library != null;
      if (this.Library != Program.Database)
      {
        this.sepUndo.Visible = this.tbUndo.Visible = this.tbRedo.Visible = false;
      }
      else
      {
        this.commands.Add(new CommandHandler(Program.Database.Undo.Undo), (UpdateHandler) (() => Program.Database.Undo.CanUndo), (object) this.tbUndo);
        this.commands.Add(new CommandHandler(Program.Database.Undo.Redo), (UpdateHandler) (() => Program.Database.Undo.CanRedo), (object) this.tbRedo);
      }
      this.Controls.SetChildIndex((Control) this.toolStrip, this.Controls.Count - 1);
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      this.toolTip.SetToolTip((Control) this.itemView, (string) null);
    }

    protected override void OnMainFormChanged()
    {
      base.OnMainFormChanged();
      if (this.Main == null)
        return;
      this.commands.Add(new CommandHandler(this.Main.EditListLayout), (object) this.tsEditListLayout);
      this.commands.Add(new CommandHandler(this.Main.SaveListLayout), (object) this.tsSaveListLayout);
      this.commands.Add(new CommandHandler(this.Main.EditListLayouts), (UpdateHandler) (() => Program.Settings.ListConfigurations.Count > 0), (object) this.tsEditLayouts);
      this.Main.OpenBooks.BookClosed += new EventHandler<BookEventArgs>(this.OpenBooksChanged);
      this.Main.OpenBooks.BookOpened += new EventHandler<BookEventArgs>(this.OpenBooksChanged);
      this.UpdatePending();
    }

    protected virtual void OnComicEditModeChanged()
    {
      this.itemView.LabelEdit = this.ComicEditMode.CanEditProperties();
      this.miRevealBrowser.Visible = this.ComicEditMode.CanShowComics();
      this.miCopyData.Visible = this.miPasteData.Visible = this.miClearData.Visible = this.tsCopySeparator.Visible = this.ComicEditMode.CanEditProperties();
      this.toolStripRemoveSeparator.Visible = this.miRemove.Visible = this.ComicEditMode.CanDeleteComics();
    }

    private void lvGroupHeaders_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      this.itemView.GroupSortingOrder = ItemView.FlipSortOrder(this.itemView.GroupSortingOrder);
    }

    private void lvGroupHeaders_ClientSizeChanged(object sender, EventArgs e)
    {
      int num = this.lvGroupHeaders.ClientRectangle.Width - (this.lvGroupsCount.Width + 2);
      if (num <= 0)
        return;
      this.lvGroupsName.Width = num;
    }

    private void browserContainer_DoubleClick(object sender, EventArgs e)
    {
      this.ShowGroupHeaders = !this.ShowGroupHeaders;
    }

    private void OpenBooksChanged(object sender, BookEventArgs e) => this.UpdateBookMarkers();

    private void btCloseStack_Click(object sender, EventArgs e) => this.CloseStack(true);

    private void btPrevStack_Click(object sender, EventArgs e)
    {
      CoverViewItem oldItem = this.stackItem as CoverViewItem;
      this.CloseStack(true);
      CoverViewItem[] array = this.itemView.DisplayedItems.OfType<CoverViewItem>().ToArray<CoverViewItem>();
      int index = ((IEnumerable<CoverViewItem>) array).FindIndex<CoverViewItem>((Predicate<CoverViewItem>) (ci => this.itemView.GetStackItems((IViewableItem) ci).OfType<CoverViewItem>().Select<CoverViewItem, ComicBook>((Func<CoverViewItem, ComicBook>) (sci => sci.Comic)).Contains<ComicBook>(oldItem.Comic))) - 1;
      if (index < 0)
        index = array.Length - 1;
      try
      {
        this.OpenStack((IViewableItem) array[index]);
      }
      catch (Exception ex)
      {
      }
    }

    private void btNextStack_Click(object sender, EventArgs e)
    {
      CoverViewItem oldItem = this.stackItem as CoverViewItem;
      this.CloseStack(true);
      CoverViewItem[] array = this.itemView.DisplayedItems.OfType<CoverViewItem>().ToArray<CoverViewItem>();
      int index = ((IEnumerable<CoverViewItem>) array).FindIndex<CoverViewItem>((Predicate<CoverViewItem>) (ci => this.itemView.GetStackItems((IViewableItem) ci).OfType<CoverViewItem>().Select<CoverViewItem, ComicBook>((Func<CoverViewItem, ComicBook>) (sci => sci.Comic)).Contains<ComicBook>(oldItem.Comic))) + 1;
      if (index >= array.Length)
        index = 0;
      try
      {
        this.OpenStack((IViewableItem) array[index]);
      }
      catch (Exception ex)
      {
      }
    }

    private void btDisplayAll_Click(object sender, EventArgs e)
    {
      this.ShowOnlyDuplicates = false;
      this.ShowOptionType = ComicBookAllPropertiesMatcher.ShowOptionType.All;
      this.ShowComicType = ComicBookAllPropertiesMatcher.ShowComicType.All;
    }

    private void itemView_MouseWheel(object sender, MouseEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Control) == Keys.None || this.itemView.ItemViewMode == ItemViewMode.Detail)
        return;
      this.SetItemSize(this.GetItemSize().Value + e.Delta / SystemInformation.MouseWheelScrollDelta * 16);
    }

    private void tbbSort_DropDownOpening(object sender, EventArgs e)
    {
      FormUtility.SafeToolStripClear(this.tbbSort.DropDownItems);
      this.itemView.CreateArrangeMenu(this.tbbSort.DropDownItems);
    }

    private void tbbGroup_DropDownOpening(object sender, EventArgs e)
    {
      FormUtility.SafeToolStripClear(this.tbbGroup.DropDownItems);
      this.itemView.CreateGroupMenu(this.tbbGroup.DropDownItems);
    }

    private void tbbGroup_ButtonClick(object sender, EventArgs e)
    {
      this.itemView.GroupSortingOrder = ItemView.FlipSortOrder(this.itemView.GroupSortingOrder);
    }

    private void tbbStack_ButtonClick(object sender, EventArgs e)
    {
      this.itemView.ItemStacker = this.itemView.ItemStacker != null ? (IGrouper<IViewableItem>) null : this.oldStacker;
    }

    private void tbbStack_DropDownOpening(object sender, EventArgs e)
    {
      FormUtility.SafeToolStripClear(this.tbbStack.DropDownItems);
      this.itemView.CreateStackMenu(this.tbbStack.DropDownItems);
    }

    private void tbbSort_ButtonClick(object sender, EventArgs e)
    {
      this.itemView.ItemSortOrder = ItemView.FlipSortOrder(this.itemView.ItemSortOrder);
    }

    private void tbbView_ButtonClick(object sender, EventArgs e)
    {
      ItemViewMode itemViewMode = this.itemView.ItemViewMode;
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
      this.itemView.ItemViewMode = itemViewMode;
    }

    private void tsQuickSearch_TextChanged(object sender, EventArgs e)
    {
      this.QuickSearch = this.tsQuickSearch.TextBox.Text;
    }

    private void bookList_BookListRefreshed(object sender, EventArgs e)
    {
      this.OnCurrentBookListChanged();
    }

    private void bookSelectorPanel_FilterChanged(object sender, EventArgs e)
    {
      this.bookListDirty = true;
    }

    protected override void OnIdle()
    {
      if (this.DesignMode || !this.Visible)
        return;
      this.UpdatePending();
      if (this.updateGroupList)
      {
        this.updateGroupList = false;
        this.FillGroupList();
      }
      if (this.newGroupListWidth != 0 && this.showGroupHeaders)
      {
        int num = this.browserContainer.ClientRectangle.Width - this.newGroupListWidth;
        if (num > 0)
          this.browserContainer.SplitterDistance = num;
        this.newGroupListWidth = 0;
      }
      if (this.quickSearchCueTexts == null)
      {
        this.quickSearchCueTexts = new string[6]
        {
          this.miSearchAll.Text,
          this.miSearchSeries.Text,
          this.miSearchWriter.Text,
          this.miSearchArtists.Text,
          this.miSearchDescriptive.Text,
          this.miSearchFile.Text
        };
        for (int index = 0; index < this.quickSearchCueTexts.Length; ++index)
          this.quickSearchCueTexts[index] = TR.Default["Search", "Search"] + " " + this.quickSearchCueTexts[index].Replace("&", string.Empty);
      }
      this.tsQuickSearch.TextBox.SetCueText(this.quickSearchCueTexts[(int) this.QuickSearchType]);
      this.tbSidebar.Visible = !this.HideNavigation && this.Main != null && this.Main.Control.FindActiveService<ISidebar>() != null;
      ToolStripSeparator tbBrowseSeparator = this.tbBrowseSeparator;
      ToolStripButton btBrowseNext = this.btBrowseNext;
      bool flag1;
      this.btBrowsePrev.Visible = flag1 = !this.HideNavigation && this.Main != null && this.Main.Control.FindActiveService<IBrowseHistory>() != null;
      int num1;
      bool flag2 = (num1 = flag1 ? 1 : 0) != 0;
      btBrowseNext.Visible = num1 != 0;
      int num2 = flag2 ? 1 : 0;
      tbBrowseSeparator.Visible = num2 != 0;
      this.tbbSort.Enabled = this.itemView.Columns.Count != 0;
      this.tbbSort.Text = this.itemView.SortColumn != null ? this.itemView.SortColumn.Text : this.noneText;
      ToolStripSplitButton tbbSort = this.tbbSort;
      string str1;
      if (this.itemView.SortColumn == null)
        str1 = this.notArrangedText;
      else
        str1 = StringUtility.Format(this.arrangedByText, (object) this.tbbSort.Text);
      tbbSort.ToolTipText = str1;
      this.tbbGroup.Enabled = this.itemView.Columns.Count != 0;
      this.tbbGroup.Text = this.itemView.GroupColumn != null ? this.itemView.GroupColumn.Text : this.noneText;
      ToolStripSplitButton tbbGroup = this.tbbGroup;
      string str2;
      if (this.itemView.GroupColumn == null)
        str2 = this.notGroupedText;
      else
        str2 = StringUtility.Format(this.groupedByText, (object) this.tbbGroup.Text);
      tbbGroup.ToolTipText = str2;
      this.openStackPanel.Visible = this.stackFilter is ComicBrowserControl.StackMatcher;
      if (this.openStackPanel.Visible)
      {
        this.currentStackName = ((ComicBrowserControl.StackMatcher) this.stackFilter).Caption;
        this.lblOpenStackText.Text = this.currentStackName;
      }
      this.tbbStack.Visible = this.itemView.ItemViewMode != ItemViewMode.Detail && !this.openStackPanel.Visible;
      if (this.tbbStack.Visible)
      {
        this.tbbStack.Enabled = this.itemView.Columns.Count != 0;
        this.tbbStack.Text = this.itemView.StackColumn != null ? this.itemView.StackColumn.Text : this.noneText;
        ToolStripSplitButton tbbStack = this.tbbStack;
        string str3;
        if (this.itemView.StackColumn == null)
          str3 = this.notStackedText;
        else
          str3 = StringUtility.Format(this.stackedByText, (object) this.tbbStack.Text);
        tbbStack.ToolTipText = str3;
      }
      if (this.itemView.ItemStacker != null)
        this.oldStacker = this.itemView.ItemStacker;
      this.tbbSort.Image = this.itemView.ItemSortOrder == SortOrder.Ascending ? this.sortUp : this.sortDown;
      this.tbbGroup.Image = this.itemView.GroupSortingOrder == SortOrder.Ascending ? this.groupDown : this.groupUp;
      if (this.tbUndo.Visible)
      {
        if (this.tbUndo.Tag == null)
          this.tbUndo.Tag = (object) this.tbUndo.Text;
        string undoLabel = Program.Database.Undo.UndoLabel;
        this.tbUndo.ToolTipText = (string) this.tbUndo.Tag + (string.IsNullOrEmpty(undoLabel) ? string.Empty : ": " + undoLabel);
        if (this.tbRedo.Tag == null)
          this.tbRedo.Tag = (object) this.tbRedo.Text;
        string str4 = Program.Database.Undo.RedoEntries.FirstOrDefault<string>();
        this.tbRedo.ToolTipText = (string) this.tbRedo.Tag + (string.IsNullOrEmpty(str4) ? string.Empty : ": " + str4);
      }
      this.OptimizeToolstripDisplay();
    }

    private void UpdatePending()
    {
      if (!this.bookListDirty)
        return;
      this.bookListDirty = false;
      this.FillBookList();
    }

    private void itemView_ItemActivate(object sender, EventArgs e)
    {
      IViewableItem focusedItem = this.itemView.FocusedItem;
      if (!this.itemView.IsStack(focusedItem) || this.itemView.GetStackCount(focusedItem) == 1)
      {
        if (focusedItem is CoverViewItem coverViewItem && coverViewItem.Comic.IsLinked)
          this.OpenComic();
        else
          this.Main.ShowInfo();
      }
      else
        this.OpenStack(focusedItem);
    }

    private void itemView_PostPaint(object sender, PaintEventArgs e)
    {
      e.Graphics.DrawShadow(this.itemView.DisplayRectangle, 8, Color.Black, 0.125f, BlurShadowType.Inside, BlurShadowParts.Edges);
    }

    private void searchBrowserContainer_ExpandedChanged(object sender, EventArgs e)
    {
      this.SearchBrowserVisible = this.searchBrowserContainer.Expanded;
    }

    private void itemView_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.selectedSize = this.itemView.SelectedItems.Cast<CoverViewItem>().Sum<CoverViewItem>((Func<CoverViewItem, long>) (cvi => Math.Max(cvi.Comic.FileSize, 0L)));
    }

    private void BookInListChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "OpenedTime"))
        return;
      this.UpdateBookMarkers();
    }

    private void itemView_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Delete)
        return;
      this.RemoveBooks();
    }

    private void contextMenuItems_Opened(object sender, EventArgs e)
    {
      this.contextMenuMouseLocation = Cursor.Position;
    }

    private void contextMenuItems_Closed(object sender, ToolStripDropDownClosedEventArgs e)
    {
      this.contextMenuCloseTime = Machine.Ticks;
    }

    private void quickSearchTimer_Tick(object sender, EventArgs e)
    {
      this.quickSearchTimer.Stop();
      this.UpdateSearch();
    }

    private void itemView_ProcessStack(object sender, ItemView.StackEventArgs e)
    {
      if (this.stacksConfig == null)
        return;
      StacksConfig.StackConfigItem stackConfigItem = this.stacksConfig.FindItem(e.Stack.Text);
      if (stackConfigItem == null)
        return;
      if (!string.IsNullOrEmpty(stackConfigItem.ThumbnailKey) && e.Stack.Items.FirstOrDefault<IViewableItem>() is ISetCustomThumbnail setCustomThumbnail)
        setCustomThumbnail.CustomThumbnailKey = ThumbnailKey.GetResource("custom", stackConfigItem.ThumbnailKey);
      Guid id = stackConfigItem.TopId;
      if (!(id != Guid.Empty))
        return;
      int index = e.Stack.Items.FindIndex((Predicate<IViewableItem>) (item => ((CoverViewItem) item).Comic.Id == id));
      if (index == -1)
        return;
      IViewableItem viewableItem = e.Stack.Items[index];
      e.Stack.Items.RemoveAt(index);
      e.Stack.Items.Insert(0, viewableItem);
    }

    private void tsQuickSearch_Enter(object sender, EventArgs e)
    {
      this.oldQuickWidth = this.tsQuickSearch.Width;
      this.tsQuickSearch.AutoSize = false;
      this.tsQuickSearch.Width *= 2;
    }

    private void tsQuickSearch_Leave(object sender, EventArgs e)
    {
      this.tsQuickSearch.AutoSize = true;
      this.tsQuickSearch.Width = this.oldQuickWidth;
    }

    private void itemView_GroupDisplayChanged(object sender, EventArgs e)
    {
      this.updateGroupList = true;
    }

    private void itemView_ItemDisplayChanged(object sender, EventArgs e)
    {
      this.updateGroupList = true;
    }

    private void lvGroupHeaders_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.lvGroupHeaders.SelectedIndices.Count == 0)
        return;
      string text = this.lvGroupHeaders.SelectedItems[0].Text;
      if (this.itemView.GetFirstGroupItem(text) == null)
        return;
      this.itemView.ExpandGroups(true, text);
      this.itemView.EnsureGroupVisible(text);
    }

    private void SetCustomColumns()
    {
      Dictionary<int, ItemViewColumn> dictionary = (Dictionary<int, ItemViewColumn>) null;
      if (this.Library != null && Program.Settings.ShowCustomBookFields)
        dictionary = this.Library.CustomValues.Where<string>((Func<string, bool>) (customKey => Program.ExtendedSettings.ShowCustomScriptValues || !customKey.Contains<char>('.'))).Select(customKey => new
        {
          Id = 10000 + Math.Abs(customKey.GetHashCode()),
          Prop = "{" + customKey + "}",
          Key = customKey
        }).Select(customField => new ItemViewColumn(customField.Id, customField.Key, 100, (object) new ComicListField(customField.Prop, this.customFieldDescription.SafeFormat((object) customField.Key), customField.Prop), (IComparer<IViewableItem>) new ComicBrowserControl.CoverViewItemPropertyComparer(customField.Prop), (IGrouper<IViewableItem>) new ComicBrowserControl.CoverViewItemPropertyGrouper(customField.Prop), false)).ToDictionary<ItemViewColumn, int>((Func<ItemViewColumn, int>) (item => item.Id));
      for (int index = this.itemView.Columns.Count - 1; index >= 0; --index)
      {
        int id = this.itemView.Columns[index].Id;
        if (id >= 10000)
        {
          if (dictionary != null && dictionary.ContainsKey(id))
            dictionary.Remove(id);
          else
            this.itemView.Columns.RemoveAt(index);
        }
      }
      if (dictionary == null)
        return;
      foreach (ItemViewColumn itemViewColumn in dictionary.Values)
      {
        itemViewColumn.TooltipText = ((ComicListField) itemViewColumn.Tag).Description;
        this.itemView.Columns.Add((IColumn) itemViewColumn);
      }
    }

    private bool CanReorderList(bool mustBeOrdered = true)
    {
      IEditableComicBookListProvider bookListProvider = this.BookList.QueryService<IEditableComicBookListProvider>();
      if (!this.ComicEditMode.CanEditList() || bookListProvider == null || bookListProvider.IsLibrary)
        return false;
      return !mustBeOrdered || this.IsViewSortedByPosition();
    }

    private bool MoveBooks(IEnumerable<ComicBook> books, bool bottom)
    {
      if (!this.CanReorderList())
        return false;
      IEditableComicBookListProvider bookListProvider = this.BookList.QueryService<IEditableComicBookListProvider>();
      ComicBook[] array = books.Where<ComicBook>(new Func<ComicBook, bool>(bookListProvider.Remove)).ToArray<ComicBook>();
      if (bottom)
      {
        foreach (ComicBook book in books)
          bookListProvider.Add(book);
      }
      else
      {
        foreach (ComicBook comicBook in ((IEnumerable<ComicBook>) array).Reverse<ComicBook>())
          bookListProvider.Insert(0, comicBook);
      }
      return true;
    }

    private void ApplyBookOrder(IEnumerable<ComicBook> books)
    {
      if (!this.CanReorderList(false))
        return;
      books = (IEnumerable<ComicBook>) books.ToArray<ComicBook>();
      IEditableComicBookListProvider bookListProvider = this.BookList.QueryService<IEditableComicBookListProvider>();
      foreach (ComicBook comicBook in ((IEnumerable<ComicBook>) books.Where<ComicBook>(new Func<ComicBook, bool>(bookListProvider.Remove)).ToArray<ComicBook>()).Reverse<ComicBook>())
        bookListProvider.Insert(0, comicBook);
    }

    private bool AllSelectedLinked()
    {
      return this.GetBookList(ComicBookFilterType.Selected).All<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsLinked));
    }

    private bool AnySelectedLinked()
    {
      return this.GetBookList(ComicBookFilterType.Selected).Any<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsLinked));
    }

    private void OptimizeToolstripDisplay()
    {
      IEnumerable<ToolStripItem> source = this.toolStrip.Items.OfType<ToolStripItem>();
      int num1 = source.Sum<ToolStripItem>((Func<ToolStripItem, int>) (n => n.Width));
      int num2 = this.toolStrip.Width - 20;
      if (num2 == this.savedOptimizeToolstripRight && num1 == this.savedOptimizeToolstripWitdh)
        return;
      if (num1 > num2)
        ComicBrowserControl.SaveStyleSet(ToolStripItemDisplayStyle.Image, (ToolStripItem) this.tbbView, (ToolStripItem) this.tbbGroup, (ToolStripItem) this.tbbSort, (ToolStripItem) this.tbbStack);
      if (num1 < num2)
      {
        ComicBrowserControl.SaveStyleSet(ToolStripItemDisplayStyle.ImageAndText, (ToolStripItem) this.tbbView, (ToolStripItem) this.tbbGroup, (ToolStripItem) this.tbbSort, (ToolStripItem) this.tbbStack);
        if (source.Sum<ToolStripItem>((Func<ToolStripItem, int>) (n => n.Width)) > num2)
          ComicBrowserControl.SaveStyleSet(ToolStripItemDisplayStyle.Image, (ToolStripItem) this.tbbView, (ToolStripItem) this.tbbGroup, (ToolStripItem) this.tbbSort, (ToolStripItem) this.tbbStack);
      }
      this.savedOptimizeToolstripRight = num2;
      this.savedOptimizeToolstripWitdh = source.Sum<ToolStripItem>((Func<ToolStripItem, int>) (n => n.Width));
    }

    private static void SaveStyleSet(ToolStripItemDisplayStyle style, params ToolStripItem[] items)
    {
      foreach (ToolStripItem toolStripItem in items)
      {
        if (toolStripItem.DisplayStyle != style)
          toolStripItem.DisplayStyle = style;
      }
    }

    private void SetSelectedComicAsListBackground()
    {
      ComicBook cb = this.GetBookList(ComicBookFilterType.Selected).FirstOrDefault<ComicBook>();
      if (cb == null)
        return;
      this.SetListBackgroundImage(cb);
    }

    private void ResetListBackgroundImage() => this.SetListBackgroundImage((string) null);

    private void SetTopOfStack()
    {
      ComicBook cb = this.GetBookList(ComicBookFilterType.Selected).FirstOrDefault<ComicBook>();
      if (cb == null)
        return;
      if (this.stacksConfig == null)
        this.stacksConfig = new StacksConfig();
      this.stacksConfig.SetStackTop(this.currentStackName, cb);
    }

    private void SetStackThumbnail()
    {
      IViewableItem viewableItem1 = this.itemView.SelectedItems.FirstOrDefault<IViewableItem>();
      if (viewableItem1 == null || !this.itemView.IsStack(viewableItem1))
        return;
      IViewableItem viewableItem2 = ((IEnumerable<IViewableItem>) this.itemView.GetStackItems(viewableItem1)).FirstOrDefault<IViewableItem>();
      string key = Program.LoadCustomThumbnail((string) null, (IWin32Window) this, this.miSetStackThumbnail.Text.Replace("&", string.Empty));
      if (key == null)
        return;
      if (this.stacksConfig == null)
        this.stacksConfig = new StacksConfig();
      this.stacksConfig.SetStackThumbnailKey(this.itemView.GetStackCaption(viewableItem2), key);
      this.FillBookList();
    }

    private void RemoveStackThumbnail()
    {
      IViewableItem viewableItem = this.itemView.SelectedItems.FirstOrDefault<IViewableItem>();
      if (viewableItem == null || !this.itemView.IsStack(viewableItem))
        return;
      this.stacksConfig.SetStackThumbnailKey(this.itemView.GetStackCaption(viewableItem), (string) null);
      this.FillBookList();
    }

    private void CloseStack(bool withUpdate)
    {
      using (new WaitCursor((Control) this))
      {
        if (this.stackFilter == null)
          return;
        this.stackItem = (IViewableItem) null;
        if (withUpdate)
        {
          this.ItemView.BeginUpdate();
          try
          {
            if (this.stacksConfig != null)
              this.stacksConfig.SetStackViewConfig(Program.Settings.CommonListStackLayout ? this.BookList.Name : this.currentStackName, this.itemView.ViewConfig);
            this.itemView.StackDisplayEnabled = true;
            if (this.preStackConfig != null)
              this.itemView.ViewConfig = this.preStackConfig;
            this.stackFilter = (ComicBookMatcher) null;
            this.FillBookList();
          }
          finally
          {
            this.itemView.EndUpdate();
            this.itemView.ScrollPosition = this.preStackScrollPosition;
          }
        }
        else
        {
          this.itemView.ItemStacker = ((ComicBrowserControl.StackMatcher) this.stackFilter).Grouper;
          this.stackFilter = (ComicBookMatcher) null;
        }
      }
    }

    private void OpenStack(IViewableItem item) => this.OpenStack(item, true);

    private void OpenStack(IViewableItem item, bool storeConfig)
    {
      using (new WaitCursor((Control) this))
      {
        if (item == null)
          return;
        this.itemView.BeginUpdate();
        try
        {
          this.stackItem = item;
          HashSet<ComicBook> items = new HashSet<ComicBook>();
          foreach (CoverViewItem stackItem in this.itemView.GetStackItems(item))
            items.Add(stackItem.Comic);
          string stackCaption = this.itemView.GetStackCaption(item);
          this.stackFilter = (ComicBookMatcher) new ComicBrowserControl.StackMatcher(this.itemView.ItemStacker, stackCaption, items);
          if (storeConfig)
          {
            this.preStackConfig = this.itemView.ViewConfig;
            this.preStackScrollPosition = this.itemView.ScrollPosition;
            this.preStackFocusedId = this.GetFocusedId();
          }
          this.itemView.ItemStacker = (IGrouper<IViewableItem>) null;
          if (this.stacksConfig == null)
            this.stacksConfig = new StacksConfig();
          ItemViewConfig stackViewConfig = this.stacksConfig.GetStackViewConfig(Program.Settings.CommonListStackLayout ? this.BookList.Name : stackCaption);
          if (stackViewConfig != null)
            this.itemView.ViewConfig = stackViewConfig;
          this.itemView.StackDisplayEnabled = false;
          this.FillBookList();
        }
        finally
        {
          this.itemView.EndUpdate();
        }
      }
    }

    private void RegisterBookList()
    {
      if (this.bookList == null)
        return;
      IDisplayListConfig displayListConfig = this.bookList.QueryService<IDisplayListConfig>();
      this.ResetListBackgroundImage();
      if (displayListConfig != null && displayListConfig.Display != null)
      {
        this.stacksConfig = CloneUtility.Clone<StacksConfig>(displayListConfig.Display.StackConfig);
        this.itemView.ViewConfig = displayListConfig.Display.View;
        this.ThumbnailConfig = displayListConfig.Display.Thumbnail;
        if (Program.Settings.LocalQuickSearch)
        {
          this.blockQuickSearchUpdate = true;
          this.ShowOptionType = displayListConfig.Display.ShowOptionType;
          this.ShowComicType = displayListConfig.Display.ShowComicType;
          this.ShowOnlyDuplicates = displayListConfig.Display.ShowOnlyDuplicates;
          this.QuickSearchType = displayListConfig.Display.QuickSearchType;
          this.QuickSearch = displayListConfig.Display.QuickSearch;
          this.blockQuickSearchUpdate = false;
          this.ShowGroupHeaders = displayListConfig.Display.ShowGroupHeaders;
          this.newGroupListWidth = displayListConfig.Display.ShowGroupHeadersWidth;
        }
        this.UpdateQuickFilter();
        if (displayListConfig.Display.View == null && this.bookList.QueryService<IEditableComicBookListProvider>() != null)
        {
          this.itemView.ItemGrouper = (IGrouper<IViewableItem>) null;
          this.itemView.SortColumn = this.itemView.Columns.FindById(100);
        }
        this.SetListBackgroundImage(displayListConfig.Display.BackgroundImageSource);
      }
      this.bookList.BookListChanged += new EventHandler(this.bookList_BookListRefreshed);
    }

    private void UnregisterBookList()
    {
      this.itemView.FocusedItem = (IViewableItem) null;
      this.StoreWorkspace((DisplayWorkspace) null);
      this.CloseStack(true);
      if (this.bookList == null)
        return;
      this.bookList.BookListChanged -= new EventHandler(this.bookList_BookListRefreshed);
    }

    private void FillBookSelector() => this.FillBookSelector(this.GetCurrentList(false), true);

    private void FillBookSelector(IEnumerable<ComicBook> books, bool updateNow)
    {
      this.bookSelectorPanel.Books.Clear();
      if (this.searchBrowserVisible && this.BookList != null)
        this.bookSelectorPanel.Books.AddRange(books);
      if (!updateNow)
        return;
      this.bookSelectorPanel.UpdateLists();
    }

    private IEnumerable<ComicBook> GetCurrentList(bool withSelector)
    {
      if (this.BookList == null)
        return Enumerable.Empty<ComicBook>();
      ComicBookGroupMatcher currentMatcher = this.GetCurrentMatcher(true, withSelector);
      IEnumerable<ComicBook> books = this.BookList.GetBooks();
      return currentMatcher != null ? currentMatcher.Match(books) : books;
    }

    private ComicBookGroupMatcher GetCurrentMatcher(bool withStack, bool withSelector)
    {
      ComicBookGroupMatcher currentMatcher = new ComicBookGroupMatcher();
      if (this.stackFilter != null & withStack)
        currentMatcher.Matchers.Add(this.stackFilter);
      if (this.quickFilter != null)
        currentMatcher.Matchers.Add(this.quickFilter);
      if (this.bookSelectorPanel.CurrentMatcher != null & withSelector)
        currentMatcher.Matchers.Add(this.bookSelectorPanel.CurrentMatcher);
      if (this.ShowOnlyDuplicates)
        currentMatcher.Matchers.Add((ComicBookMatcher) new ComicBookDuplicateMatcher());
      return currentMatcher;
    }

    private void FillBookList()
    {
      IViewableItem focusedItem = this.itemView.FocusedItem;
      int num1 = this.itemView.FocusedItemDisplayIndex;
      ComicBookGroupMatcher currentMatcher1 = this.GetCurrentMatcher(true, false);
      ComicBookMatcher currentMatcher2 = this.bookSelectorPanel.CurrentMatcher;
      ComicBook comic1 = ((CoverViewItem) focusedItem)?.Comic;
      this.itemView.BeginUpdate();
      try
      {
        int num2 = 1;
        this.itemView.Items.Clear();
        this.totalCount = 0;
        this.totalSize = 0L;
        this.selectedSize = 0L;
        if (this.BookList != null)
        {
          ComicBook[] array = this.BookList.GetBooks().ToArray<ComicBook>();
          IComicBookStatsProvider bookList = this.BookList as IComicBookStatsProvider;
          IEnumerable<ComicBook> comicBooks = (IEnumerable<ComicBook>) currentMatcher1.Match((IEnumerable<ComicBook>) array).ToArray<ComicBook>();
          this.totalCount = array.Length;
          this.FillBookSelector(comicBooks, true);
          if (currentMatcher2 != null)
            comicBooks = currentMatcher2.Match(comicBooks);
          foreach (ComicBook comic2 in comicBooks)
          {
            CoverViewItem coverViewItem = CoverViewItem.Create(comic2, num2++, bookList);
            coverViewItem.BookChanged += new PropertyChangedEventHandler(this.BookInListChanged);
            this.itemView.Items.Add((IViewableItem) coverViewItem);
            this.totalSize += Math.Max(comic2.FileSize, 0L);
          }
          this.UpdateBookMarkers();
        }
        if (comic1 == null || this.itemView.Items.Count == 0)
          return;
        foreach (CoverViewItem displayedItem in this.itemView.DisplayedItems)
        {
          foreach (CoverViewItem stackItem in this.itemView.GetStackItems((IViewableItem) displayedItem))
          {
            if (stackItem.Comic == comic1)
            {
              displayedItem.Selected = true;
              displayedItem.Focused = true;
              this.itemView.EnsureItemVisible((IViewableItem) displayedItem);
              return;
            }
          }
        }
        if (num1 < 0)
          return;
        if (num1 >= this.itemView.DisplayedItems.Count<IViewableItem>())
          num1 = this.itemView.DisplayedItems.Count<IViewableItem>() - 1;
        foreach (CoverViewItem displayedItem in this.itemView.DisplayedItems)
        {
          if (num1 == 0)
          {
            displayedItem.Selected = true;
            displayedItem.Focused = true;
            this.itemView.EnsureItemVisible((IViewableItem) displayedItem);
            break;
          }
          --num1;
        }
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this.itemView.EndUpdate();
        this.updateGroupList = true;
      }
    }

    private void FillGroupList()
    {
      if (this.showGroupHeaders && this.itemView.AreGroupsVisible)
      {
        this.lvGroupHeaders.BeginUpdate();
        IColumn column = this.itemView.Columns.FirstOrDefault<IColumn>((Func<IColumn, bool>) (c => c.ColumnGrouper == this.itemView.ItemGrouper));
        this.lvGroupsName.Text = column == null ? string.Empty : column.Text;
        this.lvGroupHeaders.Items.Clear();
        foreach (string displayedGroup in this.itemView.DisplayedGroups)
          this.lvGroupHeaders.Items.Add(displayedGroup).SubItems.Add(this.itemView.GetGroupSizeFromCaption(displayedGroup).ToString());
        this.lvGroupHeaders.EndUpdate();
        this.browserContainer.Panel2Collapsed = false;
      }
      else
        this.browserContainer.Panel2Collapsed = true;
    }

    public IEnumerable<ComicBook> GetOpenBooks()
    {
      try
      {
        return this.Main.OpenBooks.Slots.Where<ComicBookNavigator>((Func<ComicBookNavigator, bool>) (nav => nav != null)).Select<ComicBookNavigator, ComicBook>((Func<ComicBookNavigator, ComicBook>) (nav => nav.Comic));
      }
      catch (NullReferenceException ex)
      {
        return Enumerable.Empty<ComicBook>();
      }
    }

    private void UpdateBookMarkers()
    {
      DateTime dateTime = DateTime.MinValue;
      ComicBook comicBook = (ComicBook) null;
      foreach (CoverViewItem coverViewItem in (SmartList<IViewableItem>) this.itemView.Items)
      {
        if (dateTime < coverViewItem.Comic.OpenedTime)
        {
          dateTime = coverViewItem.Comic.OpenedTime;
          comicBook = coverViewItem.Comic;
        }
      }
      IEnumerable<ComicBook> openBooks = this.GetOpenBooks();
      foreach (CoverViewItem coverViewItem in (SmartList<IViewableItem>) this.itemView.Items)
      {
        coverViewItem.Marker = coverViewItem.Comic == comicBook ? MarkerType.IsLast : MarkerType.None;
        coverViewItem.Marker = openBooks.Contains<ComicBook>(coverViewItem.Comic) ? MarkerType.IsOpen : coverViewItem.Marker;
      }
    }

    private void AddFilesToLibrary()
    {
      Program.Scanner.ScanFilesOrFolders(this.GetBookList(ComicBookFilterType.NotInLibrary | ComicBookFilterType.IsNotFileless | ComicBookFilterType.Selected, true).Select<ComicBook, string>((Func<ComicBook, string>) (cb => cb.FilePath)), false, false);
    }

    private void DuplicateList(ComicListItemFolder clif = null)
    {
      if (this.Library == null || !this.Library.EditMode.CanEditList())
        return;
      ComicBookGroupMatcher currentMatcher = this.GetCurrentMatcher(false, true);
      if (currentMatcher == null || currentMatcher.Matchers.Count == 0)
        return;
      string name = string.Empty;
      ComicSmartListItem comicSmartListItem = new ComicSmartListItem("")
      {
        BaseListId = this.BookList.Id,
        MatcherMode = currentMatcher.MatcherMode
      };
      foreach (ComicBookMatcher matcher in (SmartList<ComicBookMatcher>) currentMatcher.Matchers)
        comicSmartListItem.Matchers.Add(matcher.Clone() as ComicBookMatcher);
      foreach (ComicBookValueMatcher bookValueMatcher in comicSmartListItem.Matchers.Recurse<ComicBookValueMatcher>((Func<object, IEnumerable>) (cbm => !(cbm is ComicBookGroupMatcher) ? (IEnumerable) null : (IEnumerable) ((ComicBookGroupMatcher) cbm).Matchers)))
      {
        string str = bookValueMatcher.MatchValue.Trim();
        if (!string.IsNullOrEmpty(str))
        {
          if (!string.IsNullOrEmpty(name))
            name += "/";
          name += str;
        }
      }
      if (string.IsNullOrEmpty(name))
        name = NumberedString.StripNumber(this.BookList.Name);
      int number = NumberedString.MaxNumber(this.Library.ComicLists.GetItems<ComicListItem>().Where<ComicListItem>((Func<ComicListItem, bool>) (cli => NumberedString.StripNumber(cli.Name) == name)).Select<ComicListItem, string>((Func<ComicListItem, string>) (cli => cli.Name)));
      comicSmartListItem.Name = NumberedString.Format(name, number);
      if (clif == null)
        this.Library.TemporaryFolder.Items.Add((ComicListItem) comicSmartListItem);
      else
        clif.Items.Add((ComicListItem) comicSmartListItem);
    }

    private void SetListBackgroundImage(ComicBook cb)
    {
      this.SetListBackgroundImage(cb?.Id.ToString());
    }

    private void SetListBackgroundImage(string source)
    {
      this.backgroundImageSource = source;
      try
      {
        ThreadUtility.RunInBackground("Create library backdrop", (ThreadStart) (() =>
        {
          bool flag = false;
          try
          {
            if (this.IsDisposed || source == null)
              return;
            ComicBook book = this.Library.Books[new Guid(source)];
            if (book == null)
              return;
            using (IItemLock<ThumbnailImage> thumbnail = Program.ImagePool.GetThumbnail(book.GetFrontCoverThumbnailKey(), book))
            {
              Bitmap defaultBook = ComicBox3D.CreateDefaultBook(thumbnail.Item.Bitmap, (Bitmap) null, EngineConfiguration.Default.ListCoverSize, book.PageCount);
              defaultBook.ChangeAlpha(EngineConfiguration.Default.ListCoverAlpha);
              this.SetListBackgroundImage((Image) defaultBook);
              flag = true;
            }
          }
          catch (Exception ex)
          {
          }
          finally
          {
            if (!flag)
              this.SetListBackgroundImage();
          }
        }));
      }
      catch
      {
        this.SetListBackgroundImage();
      }
    }

    private void SetListBackgroundImage(Image image)
    {
      if (this.itemView.BeginInvokeIfRequired((Action) (() => this.SetListBackgroundImage(image))) || this.itemView.BackgroundImage == image)
        return;
      if (this.itemView.BackgroundImage != null && this.itemView.BackgroundImage != this.ListBackgroundImage)
      {
        Image backgroundImage = this.itemView.BackgroundImage;
        this.itemView.BackgroundImage = (Image) null;
        backgroundImage.Dispose();
      }
      if (image == null || (double) this.itemView.BackColor.GetBrightness() < 0.949999988079071)
        return;
      this.itemView.BackgroundImage = image;
      this.itemView.BackgroundImageAlignment = System.Drawing.ContentAlignment.BottomRight;
    }

    private void SetListBackgroundImage() => this.SetListBackgroundImage(this.ListBackgroundImage);

    private void bookSelectorPanel_ItemDrag(object sender, ItemDragEventArgs e)
    {
      if (!this.ComicEditMode.CanEditList())
        return;
      int num = (int) ((Control) sender).DoDragDrop(e.Item, DragDropEffects.Copy);
    }

    private void DragGiveDragCursorFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if (this.dragCursor == null || this.dragCursor.Cursor == (Cursor) null)
        return;
      e.UseDefaultCursors = false;
      this.dragCursor.OverlayCursor = e.Effect == DragDropEffects.None ? Cursors.No : Cursors.Default;
      this.dragCursor.OverlayEffect = e.Effect == DragDropEffects.Copy ? BitmapCursorOverlayEffect.Plus : BitmapCursorOverlayEffect.None;
      Cursor.Current = this.dragCursor.Cursor;
    }

    private void DragQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
      if (e.Action != DragAction.Drop)
        return;
      Cursor.Current = Cursors.WaitCursor;
    }

    private void itemView_ItemDrag(object sender, ItemDragEventArgs e)
    {
      if (!this.ComicEditMode.CanEditList())
        return;
      ComicBookContainer data1 = new ComicBookContainer();
      ComicBookGroupMatcher bookGroupMatcher1 = new ComicBookGroupMatcher();
      data1.Books.AddRange(this.GetBookList(ComicBookFilterType.Selected));
      if (data1.Books.Count == 0)
        return;
      if (this.itemView.IsStacked)
      {
        IViewableItem[] array1 = this.itemView.SelectedItems.Where<IViewableItem>((Func<IViewableItem, bool>) (si => this.itemView.IsStack(si))).ToArray<IViewableItem>();
        data1.Name = ((IEnumerable<IViewableItem>) array1).Select<IViewableItem, string>((Func<IViewableItem, string>) (si => this.itemView.GetStackCaption(si))).FirstOrDefault<string>();
        SingleComicGrouper[] array2 = this.itemView.ItemStacker.GetGroupers<IViewableItem>().OfType<IBookGrouper>().Select<IBookGrouper, IGrouper<ComicBook>>((Func<IBookGrouper, IGrouper<ComicBook>>) (bg => bg.BookGrouper)).OfType<SingleComicGrouper>().ToArray<SingleComicGrouper>();
        foreach (IViewableItem viewableItem in array1)
        {
          ComicBookGroupMatcher bookGroupMatcher2 = new ComicBookGroupMatcher()
          {
            MatcherMode = MatcherMode.And
          };
          IGroupInfo stackGroupInfo = this.itemView.GetStackGroupInfo(viewableItem);
          IEnumerable<IGroupInfo> source;
          if (!(stackGroupInfo is ICompoundGroupInfo))
            source = ListExtensions.AsEnumerable<IGroupInfo>(stackGroupInfo);
          else
            source = (IEnumerable<IGroupInfo>) ((ICompoundGroupInfo) stackGroupInfo).Infos;
          IGroupInfo[] array3 = source.ToArray<IGroupInfo>();
          for (int index = 0; index < array2.Length; ++index)
          {
            ComicBookMatcher matcher = array2[index].CreateMatcher(array3[index]);
            if (matcher != null)
              bookGroupMatcher2.Matchers.Add(matcher);
          }
          if (bookGroupMatcher2.Matchers.Count > 1)
            bookGroupMatcher1.Matchers.Add((ComicBookMatcher) bookGroupMatcher2);
          else if (bookGroupMatcher2.Matchers.Count == 1)
            bookGroupMatcher1.Matchers.Add(bookGroupMatcher2.Matchers[0]);
        }
      }
      this.dragCursor = this.itemView.GetDragCursor(Program.ExtendedSettings.DragDropCursorAlpha);
      try
      {
        IEditableComicBookListProvider bookListProvider = this.BookList.QueryService<IEditableComicBookListProvider>();
        bool flag1 = bookListProvider != null;
        bool flag2 = bookListProvider != null && bookListProvider.IsLibrary;
        bool flag3 = !flag2 & flag1;
        DragDropEffects allowedEffects = DragDropEffects.Copy;
        this.itemView.AllowDrop = !flag2 & flag1 && this.IsViewSortedByPosition();
        if (flag3)
          allowedEffects |= DragDropEffects.Move;
        DataObject data2 = new DataObject();
        data2.SetData((object) data1);
        StringCollection filePaths = new StringCollection();
        filePaths.AddRange(data1.GetBookFiles().ToArray<string>());
        data2.SetFileDropList(filePaths);
        if (bookGroupMatcher1.Matchers.Count > 0)
          data2.SetData("ComicBookMatcher", bookGroupMatcher1.Matchers.Count == 1 ? (object) bookGroupMatcher1.Matchers[0] : (object) bookGroupMatcher1);
        this.ownDrop = false;
        if (this.itemView.DoDragDrop((object) data2, allowedEffects) != DragDropEffects.Move || bookListProvider == null || this.ownDrop)
          return;
        foreach (ComicBook book in (SmartList<ComicBook>) data1.Books)
          bookListProvider.Remove(book);
      }
      finally
      {
        if (this.dragCursor != null)
        {
          this.dragCursor.Dispose();
          this.dragCursor = (IBitmapCursor) null;
        }
        this.itemView.AllowDrop = true;
      }
    }

    private bool CreateDragContainter(DragEventArgs e)
    {
      if (this.dragBookContainer == null)
        this.dragBookContainer = DragDropContainer.Create(e.Data);
      return this.dragBookContainer.IsValid;
    }

    private void InsertBooksIntoToList(IEditableComicBookListProvider list, int index)
    {
      if (list == null)
        return;
      if (list.IsLibrary && this.dragBookContainer.IsFilesContainer)
      {
        Program.Scanner.ScanFilesOrFolders(this.dragBookContainer.FilesOrFolders, true, false);
      }
      else
      {
        if (!this.dragBookContainer.IsBookContainer)
          return;
        foreach (ComicBook book in this.dragBookContainer.Books.GetBooks())
        {
          ComicBook comicBook = Program.BookFactory.Create(book.FilePath, CreateBookOption.AddToStorage);
          if (comicBook != null)
          {
            if (index != -1)
              index = list.Insert(index, comicBook) + 1;
            else
              list.Add(comicBook);
          }
        }
        this.FillBookList();
      }
    }

    private bool IsViewSortedByPosition()
    {
      return this.itemView.SortColumn != null && this.itemView.SortColumn.Id == 100 && this.itemView.ItemSortOrder == SortOrder.Ascending && this.itemView.GroupColumn == null && !this.itemView.IsStacked;
    }

    private void SetDropEffects(DragEventArgs e)
    {
      IEditableComicBookListProvider bookListProvider = this.BookList.QueryService<IEditableComicBookListProvider>();
      e.Effect = DragDropEffects.None;
      if (this.ComicEditMode.CanEditList() && bookListProvider != null && this.CreateDragContainter(e))
        e.Effect = this.dragCursor == null || !this.IsViewSortedByPosition() ? (!bookListProvider.IsLibrary || !this.dragBookContainer.IsFilesContainer ? e.AllowedEffect : DragDropEffects.Link) : DragDropEffects.Move;
      if (e.Effect == DragDropEffects.None)
        this.itemView.MarkerVisible = false;
      else if (this.itemView.ItemHitTest(this.itemView.PointToClient(new System.Drawing.Point(e.X, e.Y))) is CoverViewItem coverViewItem && this.IsViewSortedByPosition() && this.dragBookContainer.IsBookContainer)
      {
        this.itemView.MarkerItem = (IViewableItem) coverViewItem;
        this.itemView.MarkerVisible = true;
      }
      else
        this.itemView.MarkerVisible = false;
    }

    private void itemView_DragEnter(object sender, DragEventArgs e) => this.SetDropEffects(e);

    private void itemView_DragOver(object sender, DragEventArgs e) => this.SetDropEffects(e);

    private void itemView_DragDrop(object sender, DragEventArgs e)
    {
      CoverViewItem coverViewItem = this.itemView.ItemHitTest(this.itemView.PointToClient(new System.Drawing.Point(e.X, e.Y))) as CoverViewItem;
      int index = -1;
      if (coverViewItem != null && this.IsViewSortedByPosition())
        index = coverViewItem.Position - 1;
      this.InsertBooksIntoToList(this.BookList.QueryService<IEditableComicBookListProvider>(), index);
      this.itemView.MarkerVisible = false;
      this.dragBookContainer = (DragDropContainer) null;
      this.ownDrop = true;
    }

    private void itemView_DragLeave(object sender, EventArgs e)
    {
      this.itemView.MarkerVisible = false;
      this.dragBookContainer = (DragDropContainer) null;
    }

    public virtual ItemSizeInfo GetItemSize()
    {
      switch (this.itemView.ItemViewMode)
      {
        case ItemViewMode.Thumbnail:
          return new ItemSizeInfo(FormUtility.ScaleDpiY(96), FormUtility.ScaleDpiY(512), this.itemView.ItemThumbSize.Height);
        case ItemViewMode.Tile:
          return new ItemSizeInfo(FormUtility.ScaleDpiY(64), FormUtility.ScaleDpiY(256), this.itemView.ItemTileSize.Height);
        case ItemViewMode.Detail:
          return new ItemSizeInfo(FormUtility.ScaleDpiY(12), FormUtility.ScaleDpiY(48), this.itemView.ItemRowHeight);
        default:
          return (ItemSizeInfo) null;
      }
    }

    public void SetItemSize(int height)
    {
      switch (this.itemView.ItemViewMode)
      {
        case ItemViewMode.Thumbnail:
          height = height.Clamp(FormUtility.ScaleDpiY(96), FormUtility.ScaleDpiY(512));
          this.itemView.ItemThumbSize = new System.Drawing.Size(height, height);
          break;
        case ItemViewMode.Tile:
          height = height.Clamp(FormUtility.ScaleDpiY(64), FormUtility.ScaleDpiY(256));
          this.itemView.ItemTileSize = new System.Drawing.Size(height * 2, height);
          break;
        case ItemViewMode.Detail:
          height = height.Clamp(FormUtility.ScaleDpiY(12), FormUtility.ScaleDpiY(48));
          this.itemView.ItemRowHeight = height;
          break;
      }
    }

    public Guid GetBookListId() => this.BookList != null ? this.BookList.Id : Guid.Empty;

    public void RefreshInformation()
    {
      IEnumerable<CoverViewItem> source = new List<IViewableItem>(this.itemView.SelectedItems).Cast<CoverViewItem>().Where<CoverViewItem>((Func<CoverViewItem, bool>) (cvi => cvi.Comic.IsInContainer));
      foreach (CoverViewItem coverViewItem in source)
      {
        if (coverViewItem.Comic.IsDynamicSource)
          this.Main.UpdateWebComic(coverViewItem.Comic, true);
        else
          coverViewItem.RefreshImage();
      }
      if (!this.ComicEditMode.CanScan())
        return;
      Program.Scanner.ScanFilesOrFolders(source.Select<CoverViewItem, string>((Func<CoverViewItem, string>) (cvi => cvi.Comic.FilePath)), false, false);
    }

    public void MarkSelectedRead()
    {
      foreach (ComicBook book in this.GetBookList(ComicBookFilterType.Library | ComicBookFilterType.Selected, true))
        book.MarkAsRead();
    }

    public void MarkSelectedNotRead()
    {
      foreach (ComicBook book in this.GetBookList(ComicBookFilterType.Library | ComicBookFilterType.Selected, true))
        book.MarkAsNotRead();
    }

    public void MarkSelectedChecked()
    {
      foreach (ComicBook book in this.GetBookList(ComicBookFilterType.Library | ComicBookFilterType.Selected, true))
        book.Checked = true;
    }

    public void MarkSelectedUnchecked()
    {
      foreach (ComicBook book in this.GetBookList(ComicBookFilterType.Library | ComicBookFilterType.Selected, true))
        book.Checked = false;
    }

    public void ShowWeb()
    {
      if (!(this.itemView.FocusedItem is CoverViewItem focusedItem) || string.IsNullOrEmpty(focusedItem.Comic.Web))
        return;
      Program.StartDocument(focusedItem.Comic.Web);
    }

    public void OpenComic()
    {
      if (!(this.itemView.FocusedItem is CoverViewItem focusedItem) || this.Main == null)
        return;
      focusedItem.Comic.LastOpenedFromListId = this.BookList.Id;
      this.Main.OpenBooks.Open(focusedItem.Comic, ((Program.Settings.OpenInNewTab ? 1 : 0) ^ ((Control.ModifierKeys & Keys.Control) != Keys.None ? 1 : ((this.itemView.ActivateButton & MouseButtons.Middle) != 0 ? 1 : 0))) != 0);
    }

    public void OpenComicNewTab()
    {
      if (!(this.itemView.FocusedItem is CoverViewItem focusedItem) || this.Main == null)
        return;
      focusedItem.Comic.LastOpenedFromListId = this.BookList.Id;
      this.Main.OpenBooks.Open(focusedItem.Comic, true);
    }

    public void EditItem()
    {
      if (!this.itemView.LabelEdit || this.itemView.ItemViewMode != ItemViewMode.Detail || this.itemView.SelectedCount == 0)
        return;
      if (Machine.Ticks - this.contextMenuCloseTime < 250L)
        this.itemView.EditItem(this.contextMenuMouseLocation);
      else
        this.itemView.EditItem(this.itemView.SelectedItems.FirstOrDefault<IViewableItem>());
    }

    public void CopyListSetup()
    {
      Clipboard.SetDataObject((object) new DisplayListConfig(this.ViewConfig, this.ThumbnailConfig, (TileConfig) null, this.stacksConfig, this.backgroundImageSource));
    }

    public void PasteListSetup()
    {
      try
      {
        IDataObject dataObject = Clipboard.GetDataObject();
        if (dataObject == null || !(dataObject.GetData(typeof (DisplayListConfig)) is DisplayListConfig data))
          return;
        this.itemView.ViewConfig = data.View;
        this.ThumbnailConfig = data.Thumbnail;
        this.stacksConfig = data.StackConfig;
        this.FillBookList();
        this.SetListBackgroundImage(data.BackgroundImageSource);
      }
      catch
      {
      }
    }

    public void RevealInExplorer()
    {
      ComicBook comicBook = this.GetBookList(ComicBookFilterType.IsNotFileless | ComicBookFilterType.Selected).FirstOrDefault<ComicBook>();
      if (comicBook == null)
        return;
      Program.ShowExplorer(comicBook.FilePath);
    }

    public void ShowBook(ComicBook comicBook)
    {
      foreach (CoverViewItem coverViewItem in (SmartList<IViewableItem>) this.itemView.Items)
      {
        if (coverViewItem.Comic == comicBook)
        {
          coverViewItem.Selected = true;
          coverViewItem.Focused = true;
          coverViewItem.EnsureVisible();
          break;
        }
      }
    }

    public void UpdateQuickFilter()
    {
      this.quickFilter = (ComicBookMatcher) null;
      if (this.QuickSearchType == ComicBookAllPropertiesMatcher.MatcherOption.All)
      {
        try
        {
          string query = this.QuickSearch.Trim();
          if (!query.StartsWith("NOT", StringComparison.OrdinalIgnoreCase))
          {
            if (!query.StartsWith("MATCH", StringComparison.OrdinalIgnoreCase))
              goto label_6;
          }
          this.quickFilter = ComicBookGroupMatcher.CreateMatcherFromQuery(ComicSmartListItem.TokenizeQuery(query));
        }
        catch
        {
        }
      }
label_6:
      if (this.quickFilter != null)
        return;
      this.quickFilter = ComicBookAllPropertiesMatcher.Create(this.QuickSearch, 3, this.QuickSearchType, this.ShowOptionType, this.ShowComicType);
    }

    public void UpdateSearch()
    {
      this.UpdateQuickFilter();
      this.FillBookList();
      this.displayOptionPanel.Visible = this.ShowOptionType != ComicBookAllPropertiesMatcher.ShowOptionType.All || this.ShowComicType != ComicBookAllPropertiesMatcher.ShowComicType.All || this.ShowOnlyDuplicates;
    }

    public void RemoveBooks(IEnumerable<ComicBook> books)
    {
      IRemoveBooks removeBooks = this.BookList.QueryService<IRemoveBooks>();
      if (removeBooks == null)
        return;
      this.ItemView.BeginUpdate();
      try
      {
        removeBooks.RemoveBooks(books, Control.ModifierKeys != Keys.Control);
      }
      finally
      {
        this.ItemView.EndUpdate();
      }
    }

    private bool CanRemoveBooks()
    {
      return this.itemView.InplaceEditItem == null && this.ComicEditMode.CanDeleteComics() && this.BookList != null && this.BookList.QueryService<IRemoveBooks>() != null;
    }

    private void RemoveBooks()
    {
      this.RemoveBooks(this.GetBookList(ComicBookFilterType.Selected, true));
    }

    public void CopyComicData()
    {
      try
      {
        ((CoverViewItem) this.itemView.SelectedItems.First<IViewableItem>()).Comic.ToClipboard();
      }
      catch (Exception ex)
      {
      }
    }

    public void ClearComicData()
    {
      if (!Program.AskQuestion((IWin32Window) this, TR.Messages["AskClearComicData", "Do you want to remove all entered data from the selected Books (can be reverted with Undo)?"], TR.Default["Clear"], HiddenMessageBoxes.AskClearData, cancelButton: TR.Default["No"]))
        return;
      Program.Database.Undo.SetMarker(this.miClearData.Text);
      this.GetBookList(ComicBookFilterType.Selected).ForEachProgress<ComicBook>((Action<ComicBook>) (cb => cb.ResetProperties()), (IWin32Window) this, TR.Messages[nameof (ClearComicData), "Clear Books"], TR.Messages["ClearComicDataText", "Removing all entered data from the Books"]);
    }

    public void UpdateFiles()
    {
      this.GetBookList(ComicBookFilterType.IsNotFileless | ComicBookFilterType.Selected).ForEach<ComicBook>((Action<ComicBook>) (cb => Program.QueueManager.AddBookToFileUpdate(cb, true)));
    }

    public void PasteComicData()
    {
      try
      {
        ComicBook data = Clipboard.GetData("ComicBook") as ComicBook;
        IEnumerable<ComicBook> bookList = this.GetBookList(ComicBookFilterType.Selected, true);
        if (data == null || bookList.IsEmpty<ComicBook>())
          return;
        ComicDataPasteDialog.ShowAndPaste((IWin32Window) this, data, bookList);
      }
      catch
      {
      }
    }

    private void tsListLayouts_DropDownOpening(object sender, EventArgs e)
    {
      this.Main.UpdateListConfigMenus(this.tsListLayouts.DropDownItems);
    }

    private void contextMenuItems_Opening(object sender, CancelEventArgs e)
    {
      IEnumerable<ComicBook> bookList1 = this.GetBookList(ComicBookFilterType.Selected);
      IEnumerable<ComicBook> bookList2 = this.GetBookList(ComicBookFilterType.Library | ComicBookFilterType.Selected);
      IEnumerable<ComicBook> bookList3 = this.GetBookList(ComicBookFilterType.NotInLibrary | ComicBookFilterType.Selected);
      CoverViewItem focusedItem = this.itemView.FocusedItem as CoverViewItem;
      bool flag1 = this.ComicEditMode.CanEditProperties();
      this.ComicEditMode.CanEditList();
      bool flag2 = !bookList2.IsEmpty<ComicBook>();
      this.miAddLibrary.Visible = !bookList3.IsEmpty<ComicBook>();
      this.miEdit.Visible = flag1 && this.itemView.ItemViewMode == ItemViewMode.Detail;
      this.miShowWeb.Visible = focusedItem != null && focusedItem.Comic != null && !string.IsNullOrEmpty(focusedItem.Comic.Web);
      this.miMarkAs.Visible = this.miRateMenu.Visible = flag1 & flag2;
      this.miSetTopOfStack.Visible = this.openStackPanel.Visible;
      this.miSetStackThumbnail.Visible = this.itemView.IsStack(this.itemView.SelectedItems.FirstOrDefault<IViewableItem>());
      this.miRemoveStackThumbnail.Visible = this.stacksConfig != null && !string.IsNullOrEmpty(this.stacksConfig.GetStackCustomThumbnail(this.itemView.GetStackCaption(this.itemView.SelectedItems.FirstOrDefault<IViewableItem>())));
      ComicLibrary comicLibrary = bookList2.Select<ComicBook, ComicLibrary>((Func<ComicBook, ComicLibrary>) (cb => cb.Container as ComicLibrary)).FirstOrDefault<ComicLibrary>();
      this.miAddList.Visible = ((comicLibrary == null ? 0 : (!comicLibrary.ComicLists.GetItems<ComicIdListItem>().IsEmpty<ComicIdListItem>() ? 1 : 0)) & (flag2 ? 1 : 0)) != 0;
      this.miEditList.Visible = this.CanReorderList(false);
      this.miExportComics.Visible = this.ComicEditMode.CanExport();
      this.miSetListBackground.Visible = this.BookList is ComicListItem;
      this.miCopyData.Visible = this.miPasteData.Visible = this.ComicEditMode.CanEditProperties();
      FormUtility.SafeToolStripClear(this.miShowOnly.DropDownItems);
      for (int column = 0; column < 3; ++column)
      {
        SearchBrowserControl.SelectionEntry selectionColumn = this.bookSelectorPanel.GetSelectionColumn(column);
        string text = (string) null;
        if (selectionColumn != null)
        {
          foreach (ComicBook comicBook in bookList1)
          {
            string stringPropertyValue = comicBook.GetStringPropertyValue(selectionColumn.Property);
            if (text == null)
              text = stringPropertyValue;
            else if (text != stringPropertyValue)
            {
              text = (string) null;
              break;
            }
          }
          if (!string.IsNullOrEmpty(text))
          {
            int n = column;
            this.miShowOnly.DropDownItems.Add(string.Format("{0} '{1}'", (object) selectionColumn.Caption, (object) text.Ellipsis(60 - selectionColumn.Caption.Length, "...")), (Image) null, (EventHandler) ((sd, ea) =>
            {
              this.SearchBrowserVisible = true;
              this.bookSelectorPanel.SelectEntry(n, text);
            }));
          }
        }
      }
      this.miShowOnly.Visible = this.miShowOnly.DropDownItems.Count != 0;
      this.miUpdateComicFiles.Visible = bookList1.Any<ComicBook>((Func<ComicBook, bool>) (cb => cb.ComicInfoIsDirty));
      this.contextMenuItems.FixSeparators();
    }

    private void LayoutMenuOpening(object sender, CancelEventArgs e)
    {
      try
      {
        IDataObject dataObject = Clipboard.GetDataObject();
        this.miPasteListSetup.Enabled = dataObject != null && dataObject.GetDataPresent(typeof (DisplayListConfig));
      }
      catch
      {
        this.miPasteListSetup.Enabled = false;
      }
    }

    private void contextExport_Opening(object sender, CancelEventArgs e)
    {
      while (this.contextExport.Items.Count > 2)
      {
        ToolStripItem toolStripItem = this.contextExport.Items[2];
        this.contextExport.Items.RemoveAt(2);
        toolStripItem.Dispose();
      }
      bool enabled = this.AllSelectedLinked();
      this.AddConverterEntries((ICollection<ExportSetting>) Program.ExportComicRackPresets, enabled);
      this.AddConverterEntries((ICollection<ExportSetting>) Program.Settings.ExportUserPresets, enabled);
    }

    private void AddConverterEntries(
      ICollection<ExportSetting> converterSettingCollection,
      bool enabled)
    {
      if (converterSettingCollection.Count == 0)
        return;
      this.contextExport.Items.Add((ToolStripItem) new ToolStripSeparator());
      foreach (ExportSetting converterSetting in (IEnumerable<ExportSetting>) converterSettingCollection)
      {
        ExportSetting copy = converterSetting;
        this.contextExport.Items.Add(converterSetting.Name, (Image) null, (EventHandler) ((snd, ea) => this.Main.ConvertComic(this.GetBookList(ComicBookFilterType.Selected, true), copy))).Enabled = enabled;
      }
    }

    private void miAddList_DropDownOpening(object sender, EventArgs e)
    {
      int listMenuSize = Program.ExtendedSettings.ListMenuSize;
      ToolStripMenuItem toolStripMenuItem1 = (ToolStripMenuItem) sender;
      int num = 0;
      IEnumerable<ComicBook> selectedBooks = this.GetBookList(ComicBookFilterType.Library | ComicBookFilterType.Selected);
      ComicLibrary comicLibrary = selectedBooks.Select<ComicBook, ComicLibrary>((Func<ComicBook, ComicLibrary>) (cb => cb.Container as ComicLibrary)).FirstOrDefault<ComicLibrary>();
      if (comicLibrary == null || !comicLibrary.EditMode.CanEditList())
        return;
      FormUtility.SafeToolStripClear(toolStripMenuItem1.DropDownItems);
      foreach (ComicIdListItem comicIdListItem in comicLibrary.ComicLists.GetItems<ComicIdListItem>().Where<ComicIdListItem>((Func<ComicIdListItem, bool>) (l => l != this.BookList)))
      {
        if (++num != listMenuSize + 1)
        {
          ComicIdListItem li = comicIdListItem;
          string str = new string(' ', comicLibrary.ComicLists.GetChildLevel<ComicListItem>((ComicListItem) li) * 4);
          if (num == listMenuSize)
          {
            ToolStripItemCollection dropDownItems = toolStripMenuItem1.DropDownItems;
            ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("...");
            toolStripMenuItem2.Enabled = false;
            dropDownItems.Add((ToolStripItem) toolStripMenuItem2);
          }
          else
            toolStripMenuItem1.DropDownItems.Add(str + li.Name, this.GetComicListImage((ComicListItem) li), (EventHandler) ((s, ea) => li.AddRange(selectedBooks)));
        }
        else
          break;
      }
      this.AddNoneEntry(toolStripMenuItem1.DropDownItems);
    }

    private void tbbDuplicateList_DropDownOpening(object sender, EventArgs e)
    {
      if (this.Library == null)
        return;
      ToolStripDropDownItem stripDropDownItem = (ToolStripDropDownItem) sender;
      FormUtility.SafeToolStripClear(stripDropDownItem.DropDownItems);
      foreach (ComicListItemFolder comicListItemFolder in this.Library.ComicLists.GetItems<ComicListItemFolder>())
      {
        ComicListItemFolder li = comicListItemFolder;
        string str = new string(' ', this.Library.ComicLists.GetChildLevel<ComicListItem>((ComicListItem) li) * 4);
        stripDropDownItem.DropDownItems.Add(str + li.Name, this.GetComicListImage((ComicListItem) li), (EventHandler) ((s, ea) => this.DuplicateList(li)));
      }
      this.AddNoneEntry(stripDropDownItem.DropDownItems);
    }

    private void miShowInList_DropDownOpening(object sender, EventArgs e)
    {
      if (this.Library == null)
        return;
      int maxLists = Program.ExtendedSettings.ListMenuSize;
      ToolStripMenuItem menu = (ToolStripMenuItem) sender;
      ComicBook cb = this.GetBookList(ComicBookFilterType.Selected).FirstOrDefault<ComicBook>();
      if (cb == null)
        return;
      try
      {
        this.buildMenuThread.Join(5000);
      }
      catch
      {
      }
      FormUtility.SafeToolStripClear(menu.DropDownItems);
      ToolStripItem dummy = menu.DropDownItems.Add(TR.Load(this.Name)["SearchingLists", "Searching list..."]);
      dummy.Enabled = false;
      this.abortBuildMenu.Reset();
      this.buildMenuThread = ThreadUtility.RunInBackground("Build list menu", (ThreadStart) (() =>
      {
        try
        {
          this.Library.ComicListsLocked = true;
          int count = 0;
          foreach (ComicListItem comicListItem in this.Library.ComicLists.GetItems<ComicListItem>().Where<ComicListItem>((Func<ComicListItem, bool>) (l => l.GetBooks().Contains<ComicBook>(cb))))
          {
            if (this.abortBuildMenu.WaitOne(0))
              return;
            if (++count != maxLists + 1)
            {
              ComicListItem li = comicListItem;
              string prefix = new string(' ', this.Library.ComicLists.GetChildLevel<ComicListItem>(li) * 4);
              ControlExtensions.Invoke(this, (Action) (() =>
              {
                ToolStripMenuItem toolStripMenuItem;
                if (count == maxLists)
                  toolStripMenuItem = new ToolStripMenuItem("...")
                  {
                    Enabled = false
                  };
                else
                  toolStripMenuItem = new ToolStripMenuItem(prefix + li.Name, this.GetComicListImage(li), (EventHandler) ((s, ea) => this.ShowBookInList(li, cb)));
                menu.DropDownItems.Insert(menu.DropDownItems.Count - 1, (ToolStripItem) toolStripMenuItem);
              }));
            }
            else
              break;
          }
          ControlExtensions.Invoke(this, (Action) (() => menu.DropDownItems.Remove(dummy)));
        }
        catch (Exception ex)
        {
        }
        finally
        {
          this.Library.ComicListsLocked = false;
        }
      }));
    }

    private void miShowInList_DropDownClosed(object sender, EventArgs e)
    {
      this.abortBuildMenu.Set();
    }

    private void ShowBookInList(ComicListItem list, ComicBook cb)
    {
      if (this.Main == null)
        return;
      this.Main.ShowBookInList(this.Library, list, cb, true);
    }

    private void AddNoneEntry(ToolStripItemCollection ic)
    {
      if (ic.Count != 0)
        return;
      ic.Add(TR.Default["None", "None"]).Enabled = false;
    }

    private Image GetComicListImage(ComicListItem cli)
    {
      switch (cli.ImageKey)
      {
        case "Library":
          return (Image) Resources.Library;
        case "Folder":
          return (Image) Resources.SearchFolder;
        case "Search":
          return (Image) Resources.SearchDocument;
        case "List":
          return (Image) Resources.List;
        case "TempFolder":
          return (Image) Resources.TempFolder;
        default:
          return (Image) null;
      }
    }

    public IEnumerable<ComicBook> GetBookList(ComicBookFilterType cbft, bool asArray)
    {
      if (asArray)
        cbft |= ComicBookFilterType.AsArray;
      return this.GetBookList(cbft);
    }

    public event EventHandler CurrentBookListChanged;

    public event EventHandler SearchBrowserVisibleChanged;

    public event EventHandler QuickSearchChanged;

    protected virtual void OnCurrentBookListChanged()
    {
      this.bookSelectorPanel.ClearNot();
      this.bookListDirty = true;
      if (this.CurrentBookListChanged == null)
        return;
      this.CurrentBookListChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnQuickSearchChanged()
    {
      if (this.tsQuickSearch.TextBox.Text != this.quickSearch)
        this.tsQuickSearch.TextBox.Text = this.quickSearch;
      if (this.blockQuickSearchUpdate)
        return;
      this.quickSearchTimer.Stop();
      this.quickSearchTimer.Start();
      if (this.QuickSearchChanged == null)
        return;
      this.QuickSearchChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnSearchBrowserVisibleChanged()
    {
      IMatcher<ComicBook> currentMatcher = (IMatcher<ComicBook>) this.bookSelectorPanel.CurrentMatcher;
      if (this.searchBrowserContainer.Expanded)
        this.searchBrowserContainer.Expanded = this.SearchBrowserVisible;
      this.FillBookSelector();
      if (!this.searchBrowserContainer.Expanded)
        this.searchBrowserContainer.Expanded = this.SearchBrowserVisible;
      this.FillBookList();
      if (this.SearchBrowserVisibleChanged == null)
        return;
      this.SearchBrowserVisibleChanged((object) this, EventArgs.Empty);
    }

    private void toolTip_Popup(object sender, PopupEventArgs e)
    {
      e.ToolTipSize = new System.Drawing.Size(360, 120);
      e.Cancel = !Program.Settings.ShowToolTips || this.toolTipItem == null || this.itemView.ItemViewMode == ItemViewMode.Tile;
    }

    private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
    {
      if (this.toolTipItem == null)
        return;
      using (IItemLock<ThumbnailImage> thumbnail = this.toolTipItem.GetThumbnail(true))
      {
        ComicBook comic = this.toolTipItem.Comic;
        VisualStyleElement normal = VisualStyleElement.ToolTip.Standard.Normal;
        if (VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(normal))
        {
          new VisualStyleRenderer(normal).DrawBackground((IDeviceContext) e.Graphics, e.Bounds);
        }
        else
        {
          e.DrawBackground();
          e.DrawBorder();
        }
        Rectangle bounds = e.Bounds;
        bounds.Inflate(-10, -10);
        ThumbTileRenderer.DrawTile(e.Graphics, bounds, (Image) thumbnail.Item.GetThumbnail(bounds.Height), comic, FC.GetRelative(this.Font, 1.2f), SystemColors.InfoText, Color.Transparent, ThumbnailDrawingOptions.DefaultWithoutBackground, ComicTextElements.DefaultComic, false);
      }
    }

    private void foundView_MouseLeave(object sender, EventArgs e)
    {
      this.toolTip.SetToolTip((Control) this, (string) null);
    }

    private void foundView_MouseHover(object sender, EventArgs e)
    {
      System.Drawing.Point client = this.itemView.PointToClient(Cursor.Position);
      this.toolTipItem = this.itemView.ItemHitTest(client.X, client.Y) as CoverViewItem;
      if (this.toolTipItem == null || this.toolTipItem.Comic == null)
        return;
      string caption = this.toolTipItem.Comic.Id.ToString();
      if (!(this.toolTip.GetToolTip((Control) this.itemView) != caption) || !Program.Settings.ShowToolTips)
        return;
      this.toolTip.SetToolTip((Control) this.itemView, caption);
    }

    private void foundView_MouseMove(object sender, MouseEventArgs e) => this.itemView.ResetMouse();

    public void RefreshDisplay()
    {
      AutomaticProgressDialog.Process((IWin32Window) this, TR.Messages["GettingList", "Getting Books List"], TR.Messages["GettingListText", "Retrieving all Books from the selected folder"], 1000, new Action(this.BookList.Refresh), AutomaticProgressDialogOptions.EnableCancel);
      this.FillBookList();
    }

    public void SettingsChanged()
    {
      this.SetCustomColumns();
      this.FillBookList();
    }

    [DefaultValue(false)]
    public bool SearchBrowserVisible
    {
      get => this.searchBrowserVisible;
      set
      {
        if (this.searchBrowserVisible == value)
          return;
        this.searchBrowserVisible = value;
        this.OnSearchBrowserVisibleChanged();
      }
    }

    public void FocusQuickSearch() => this.tsQuickSearch.TextBox.Focus();

    public void SetWorkspace(DisplayWorkspace ws)
    {
      this.tsQuickSearch.TextBox.AutoCompleteList.AddRange(Program.Settings.QuickSearchList.ToArray());
    }

    public void StoreWorkspace(DisplayWorkspace ws)
    {
      if (this.bookList == null)
        return;
      this.UpdateViewConfig();
      if (this.Disposing)
        return;
      if (this.tsQuickSearch.TextBox == null)
        return;
      try
      {
        HashSet<string> collection = new HashSet<string>(this.tsQuickSearch.TextBox.AutoCompleteList.Cast<string>());
        Program.Settings.QuickSearchList.Clear();
        Program.Settings.QuickSearchList.AddRange((IEnumerable<string>) collection);
      }
      catch
      {
      }
    }

    private void UpdateViewConfig()
    {
      if (this.DisableViewConfigUpdate)
        return;
      IDisplayListConfig displayListConfig = this.bookList.QueryService<IDisplayListConfig>();
      if (displayListConfig == null)
        return;
      if (this.stackItem is CoverViewItem stackItem)
        displayListConfig.Display = new DisplayListConfig(this.preStackConfig, this.ThumbnailConfig, (TileConfig) null, this.stacksConfig, this.backgroundImageSource)
        {
          ScrollPosition = this.preStackScrollPosition,
          FocusedComicId = this.preStackFocusedId,
          StackedComicId = stackItem.Comic.Id,
          StackScrollPosition = this.itemView.ScrollPosition,
          StackFocusedComicId = this.GetFocusedId()
        };
      else
        displayListConfig.Display = new DisplayListConfig(this.itemView.ViewConfig, this.ThumbnailConfig, (TileConfig) null, this.stacksConfig, this.backgroundImageSource)
        {
          ScrollPosition = this.itemView.ScrollPosition,
          FocusedComicId = this.GetFocusedId()
        };
      displayListConfig.Display.QuickSearch = this.QuickSearch;
      displayListConfig.Display.QuickSearchType = this.QuickSearchType;
      displayListConfig.Display.ShowOptionType = this.ShowOptionType;
      displayListConfig.Display.ShowComicType = this.ShowComicType;
      displayListConfig.Display.ShowOnlyDuplicates = this.ShowOnlyDuplicates;
      displayListConfig.Display.ShowGroupHeaders = this.ShowGroupHeaders;
      displayListConfig.Display.ShowGroupHeadersWidth = this.newGroupListWidth != 0 ? this.newGroupListWidth : this.browserContainer.ClientRectangle.Width - this.browserContainer.SplitterDistance;
    }

    private Guid GetFocusedId()
    {
      return (this.itemView.FocusedItem ?? this.itemView.SelectedItems.FirstOrDefault<IViewableItem>()) is CoverViewItem coverViewItem ? coverViewItem.Comic.Id : Guid.Empty;
    }

    private void SetFocusedItem(Guid id)
    {
      if (id == Guid.Empty)
        return;
      CoverViewItem coverViewItem = this.itemView.DisplayedItems.OfType<CoverViewItem>().FirstOrDefault<CoverViewItem>((Func<CoverViewItem, bool>) (ci => ci.Comic.Id == id));
      if (coverViewItem == null)
        return;
      coverViewItem.Focused = true;
      coverViewItem.Selected = true;
      coverViewItem.EnsureVisible();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicLibrary Library
    {
      get => this.library;
      set
      {
        if (this.library == value)
          return;
        this.library = value;
        this.library.CustomValuesChanged += (EventHandler) ((s, e) => this.SetCustomColumns());
        this.SetCustomColumns();
      }
    }

    public string SelectionInfo
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        int count = this.itemView.Items.Count;
        int selectedCount = this.itemView.SelectedCount;
        IComicBookListProvider bookList = this.BookList;
        if (bookList != null && !string.IsNullOrEmpty(bookList.Name))
        {
          stringBuilder.AppendFormat(bookList.Name);
          stringBuilder.AppendFormat(": ");
        }
        stringBuilder.AppendFormat(count == 1 ? this.eComicText : this.eComicsText, (object) count);
        if (this.totalCount != count)
        {
          stringBuilder.Append(" (");
          try
          {
            stringBuilder.AppendFormat(this.filteredText, (object) (this.totalCount - count));
          }
          catch
          {
          }
          stringBuilder.Append(")");
        }
        if (this.totalSize != 0L)
        {
          stringBuilder.Append(" / ");
          stringBuilder.AppendFormat(string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
          {
            (object) this.totalSize
          }));
        }
        if (selectedCount != 0)
        {
          stringBuilder.Append(" - ");
          bool flag = true;
          if (selectedCount == 1)
          {
            ComicBook comicBook = this.GetBookList(ComicBookFilterType.IsLocal | ComicBookFilterType.IsNotFileless | ComicBookFilterType.Selected).FirstOrDefault<ComicBook>();
            if (comicBook != null)
            {
              stringBuilder.Append(comicBook.FilePath);
              flag = false;
            }
          }
          if (flag)
            stringBuilder.AppendFormat(this.selectedText, (object) selectedCount);
        }
        if (this.selectedSize != 0L)
        {
          stringBuilder.Append(" / ");
          stringBuilder.AppendFormat(string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
          {
            (object) this.selectedSize
          }));
        }
        return stringBuilder.ToString();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DisplayListConfig ListConfig
    {
      get
      {
        return new DisplayListConfig()
        {
          View = this.ViewConfig,
          Thumbnail = new ThumbnailConfig(this.ThumbnailConfig)
        };
      }
      set
      {
        this.ViewConfig = value.View;
        this.ThumbnailConfig = new ThumbnailConfig(value.Thumbnail);
        this.FillBookList();
      }
    }

    public bool SelectComic(ComicBook comic)
    {
      this.CloseStack(true);
      CoverViewItem coverViewItem = this.itemView.DisplayedItems.OfType<CoverViewItem>().FirstOrDefault<CoverViewItem>((Func<CoverViewItem, bool>) (c => !this.itemView.IsStack((IViewableItem) c) ? c.Comic == comic : this.itemView.GetStackItems((IViewableItem) c).OfType<CoverViewItem>().FirstOrDefault<CoverViewItem>((Func<CoverViewItem, bool>) (sc => sc.Comic == comic)) != null));
      if (coverViewItem == null)
      {
        if (string.IsNullOrEmpty(this.QuickSearch) && this.ShowOptionType == ComicBookAllPropertiesMatcher.ShowOptionType.All && this.ShowComicType == ComicBookAllPropertiesMatcher.ShowComicType.All && !this.ShowOnlyDuplicates)
          return false;
        this.QuickSearch = string.Empty;
        this.ShowOptionType = ComicBookAllPropertiesMatcher.ShowOptionType.All;
        this.ShowComicType = ComicBookAllPropertiesMatcher.ShowComicType.All;
        this.ShowOnlyDuplicates = false;
        this.UpdateSearch();
        return this.SelectComic(comic);
      }
      if (this.itemView.IsStack((IViewableItem) coverViewItem))
      {
        this.OpenStack((IViewableItem) coverViewItem);
        coverViewItem = this.itemView.DisplayedItems.OfType<CoverViewItem>().FirstOrDefault<CoverViewItem>((Func<CoverViewItem, bool>) (c => c.Comic == comic));
      }
      this.itemView.SelectAll(false);
      if (coverViewItem != null)
      {
        coverViewItem.Selected = true;
        coverViewItem.Focused = true;
        coverViewItem.EnsureVisible();
      }
      return true;
    }

    public bool SelectComics(IEnumerable<ComicBook> books)
    {
      bool flag = true;
      HashSet<ComicBook> comicBookSet = new HashSet<ComicBook>(books);
      this.UpdatePending();
      this.itemView.SelectAll(false);
      foreach (CoverViewItem displayedItem in this.itemView.DisplayedItems)
      {
        displayedItem.Selected = comicBookSet.Contains(displayedItem.Comic);
        if (flag && displayedItem.Selected)
          displayedItem.EnsureVisible();
        flag = false;
      }
      return true;
    }

    public ToolStripItemCollection ListConfigMenu => this.tsListLayouts.DropDownItems;

    public IEnumerable<ComicBook> GetBookList(ComicBookFilterType cbft)
    {
      IEnumerable<ComicBook> books = !cbft.HasFlag((Enum) ComicBookFilterType.Selected) ? this.itemView.DisplayedItems.Cast<CoverViewItem>().Select<CoverViewItem, ComicBook>((Func<CoverViewItem, ComicBook>) (vi => vi.Comic)) : this.itemView.SelectedItems.Cast<CoverViewItem>().Select<CoverViewItem, ComicBook>((Func<CoverViewItem, ComicBook>) (vi => vi.Comic));
      IEnumerable<ComicBook> source = ComicBookCollection.Filter(cbft, books);
      if (cbft.HasFlag((Enum) ComicBookFilterType.Sorted) && this.itemView.ItemSortOrder == SortOrder.Descending)
        source = source.Reverse<ComicBook>();
      return source;
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ComicBrowserControl));
      this.contextRating = new ContextMenuStrip(this.components);
      this.miRating0 = new ToolStripMenuItem();
      this.toolStripMenuItem3 = new ToolStripSeparator();
      this.miRating1 = new ToolStripMenuItem();
      this.miRating2 = new ToolStripMenuItem();
      this.miRating3 = new ToolStripMenuItem();
      this.miRating4 = new ToolStripMenuItem();
      this.miRating5 = new ToolStripMenuItem();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.miQuickRating = new ToolStripMenuItem();
      this.miRateMenu = new ToolStripMenuItem();
      this.contextMarkAs = new ContextMenuStrip(this.components);
      this.miMarkUnread = new ToolStripMenuItem();
      this.miMarkRead = new ToolStripMenuItem();
      this.toolStripMenuItem9 = new ToolStripSeparator();
      this.miMarkChecked = new ToolStripMenuItem();
      this.miMarkUnchecked = new ToolStripMenuItem();
      this.miMarkAs = new ToolStripMenuItem();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.contextMenuItems = new ContextMenuStrip(this.components);
      this.miRead = new ToolStripMenuItem();
      this.miReadTab = new ToolStripMenuItem();
      this.miProperties = new ToolStripMenuItem();
      this.miShowWeb = new ToolStripMenuItem();
      this.miEdit = new ToolStripMenuItem();
      this.toolStripMenuItem5 = new ToolStripSeparator();
      this.miEditList = new ToolStripMenuItem();
      this.miEditListMoveToTop = new ToolStripMenuItem();
      this.miEditListMoveToBottom = new ToolStripMenuItem();
      this.toolStripMenuItem8 = new ToolStripSeparator();
      this.miEditListApplyOrder = new ToolStripMenuItem();
      this.miAddList = new ToolStripMenuItem();
      this.dummyEntryToolStripMenuItem = new ToolStripMenuItem();
      this.tsMarkAsSeparator = new ToolStripSeparator();
      this.miAddLibrary = new ToolStripMenuItem();
      this.miShowOnly = new ToolStripMenuItem();
      this.miShowInList = new ToolStripMenuItem();
      this.dummyEntryToolStripMenuItem1 = new ToolStripMenuItem();
      this.miExportComics = new ToolStripMenuItem();
      this.contextExport = new ContextMenuStrip(this.components);
      this.miExportComicsAs = new ToolStripMenuItem();
      this.miExportComicsWithPrevious = new ToolStripMenuItem();
      this.miAutomation = new ToolStripMenuItem();
      this.miUpdateComicFiles = new ToolStripMenuItem();
      this.miRevealBrowser = new ToolStripMenuItem();
      this.toolStripMenuItem7 = new ToolStripSeparator();
      this.miCopyData = new ToolStripMenuItem();
      this.miPasteData = new ToolStripMenuItem();
      this.miClearData = new ToolStripMenuItem();
      this.tsCopySeparator = new ToolStripSeparator();
      this.miSelectAll = new ToolStripMenuItem();
      this.miInvertSelection = new ToolStripMenuItem();
      this.miRefreshInformation = new ToolStripMenuItem();
      this.sepListBackground = new ToolStripSeparator();
      this.miSetTopOfStack = new ToolStripMenuItem();
      this.miSetStackThumbnail = new ToolStripMenuItem();
      this.miRemoveStackThumbnail = new ToolStripMenuItem();
      this.miSetListBackground = new ToolStripMenuItem();
      this.toolStripRemoveSeparator = new ToolStripSeparator();
      this.miRemove = new ToolStripMenuItem();
      this.quickSearchTimer = new System.Windows.Forms.Timer(this.components);
      this.contextQuickSearch = new ContextMenuStrip(this.components);
      this.miSearchAll = new ToolStripMenuItem();
      this.toolStripSeparator5 = new ToolStripSeparator();
      this.miSearchSeries = new ToolStripMenuItem();
      this.miSearchWriter = new ToolStripMenuItem();
      this.miSearchArtists = new ToolStripMenuItem();
      this.miSearchDescriptive = new ToolStripMenuItem();
      this.miSearchCatalog = new ToolStripMenuItem();
      this.miSearchFile = new ToolStripMenuItem();
      this.itemView = new ItemView();
      this.displayOptionPanel = new Panel();
      this.lblDisplayOptionText = new Label();
      this.btDisplayAll = new System.Windows.Forms.Button();
      this.searchBrowserContainer = new SizableContainer();
      this.bookSelectorPanel = new SearchBrowserControl();
      this.openStackPanel = new Panel();
      this.btPrevStack = new System.Windows.Forms.Button();
      this.btNextStack = new System.Windows.Forms.Button();
      this.lblOpenStackText = new Label();
      this.btCloseStack = new System.Windows.Forms.Button();
      this.toolStrip = new ToolStrip();
      this.tbSidebar = new ToolStripButton();
      this.btBrowsePrev = new ToolStripButton();
      this.btBrowseNext = new ToolStripButton();
      this.tbBrowseSeparator = new ToolStripSeparator();
      this.tbbView = new ToolStripSplitButton();
      this.miViewThumbnails = new ToolStripMenuItem();
      this.miViewTiles = new ToolStripMenuItem();
      this.miViewDetails = new ToolStripMenuItem();
      this.toolStripMenuItem6 = new ToolStripSeparator();
      this.miExpandAllGroups = new ToolStripMenuItem();
      this.miShowGroupHeaders = new ToolStripMenuItem();
      this.toolStripMenuItem2 = new ToolStripSeparator();
      this.miShowOnlyAllComics = new ToolStripMenuItem();
      this.miShowOnlyUnreadComics = new ToolStripMenuItem();
      this.miShowOnlyReadingComics = new ToolStripMenuItem();
      this.miShowOnlyReadComics = new ToolStripMenuItem();
      this.toolStripMenuItem4 = new ToolStripSeparator();
      this.miShowOnlyComics = new ToolStripMenuItem();
      this.miShowOnlyFileless = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.miShowOnlyDuplicates = new ToolStripMenuItem();
      this.tbbGroup = new ToolStripSplitButton();
      this.dummyToolStripMenuItem = new ToolStripMenuItem();
      this.tbbStack = new ToolStripSplitButton();
      this.dummyToolStripMenuItem1 = new ToolStripMenuItem();
      this.tbbSort = new ToolStripSplitButton();
      this.dummyToolStripMenuItem2 = new ToolStripMenuItem();
      this.tsQuickSearch = new ToolStripSearchTextBox();
      this.tsListLayouts = new ToolStripDropDownButton();
      this.tsEditListLayout = new ToolStripMenuItem();
      this.tsSaveListLayout = new ToolStripMenuItem();
      this.miResetListBackground = new ToolStripMenuItem();
      this.toolStripMenuItem23 = new ToolStripSeparator();
      this.tsEditLayouts = new ToolStripMenuItem();
      this.separatorListLayout = new ToolStripSeparator();
      this.sepDuplicateList = new ToolStripSeparator();
      this.tbbDuplicateList = new ToolStripDropDownButton();
      this.dummyEntryToolStripMenuItem2 = new ToolStripMenuItem();
      this.sepUndo = new ToolStripSeparator();
      this.tbUndo = new ToolStripButton();
      this.tbRedo = new ToolStripButton();
      this.lvGroupHeaders = new ListViewEx();
      this.lvGroupsName = new ColumnHeader();
      this.lvGroupsCount = new ColumnHeader();
      this.browserContainer = new SplitContainer();
      this.contextRating.SuspendLayout();
      this.contextMarkAs.SuspendLayout();
      this.contextMenuItems.SuspendLayout();
      this.contextExport.SuspendLayout();
      this.contextQuickSearch.SuspendLayout();
      this.displayOptionPanel.SuspendLayout();
      this.searchBrowserContainer.SuspendLayout();
      this.openStackPanel.SuspendLayout();
      this.toolStrip.SuspendLayout();
      this.browserContainer.BeginInit();
      this.browserContainer.Panel1.SuspendLayout();
      this.browserContainer.Panel2.SuspendLayout();
      this.browserContainer.SuspendLayout();
      this.SuspendLayout();
      this.contextRating.Items.AddRange(new ToolStripItem[9]
      {
        (ToolStripItem) this.miRating0,
        (ToolStripItem) this.toolStripMenuItem3,
        (ToolStripItem) this.miRating1,
        (ToolStripItem) this.miRating2,
        (ToolStripItem) this.miRating3,
        (ToolStripItem) this.miRating4,
        (ToolStripItem) this.miRating5,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.miQuickRating
      });
      this.contextRating.Name = "contextRating";
      this.contextRating.OwnerItem = (ToolStripItem) this.miRateMenu;
      this.contextRating.Size = new System.Drawing.Size(286, 170);
      this.miRating0.Name = "miRating0";
      this.miRating0.ShortcutKeys = Keys.D0 | Keys.Shift | Keys.Alt;
      this.miRating0.Size = new System.Drawing.Size(285, 22);
      this.miRating0.Tag = (object) "0";
      this.miRating0.Text = "None";
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(282, 6);
      this.miRating1.Name = "miRating1";
      this.miRating1.ShortcutKeys = Keys.D1 | Keys.Shift | Keys.Alt;
      this.miRating1.Size = new System.Drawing.Size(285, 22);
      this.miRating1.Tag = (object) "1";
      this.miRating1.Text = "* (1 Star)";
      this.miRating2.Name = "miRating2";
      this.miRating2.ShortcutKeys = Keys.D2 | Keys.Shift | Keys.Alt;
      this.miRating2.Size = new System.Drawing.Size(285, 22);
      this.miRating2.Tag = (object) "2";
      this.miRating2.Text = "** (2 Stars)";
      this.miRating3.Name = "miRating3";
      this.miRating3.ShortcutKeys = Keys.D3 | Keys.Shift | Keys.Alt;
      this.miRating3.Size = new System.Drawing.Size(285, 22);
      this.miRating3.Tag = (object) "3";
      this.miRating3.Text = "*** (3 Stars)";
      this.miRating4.Name = "miRating4";
      this.miRating4.ShortcutKeys = Keys.D4 | Keys.Shift | Keys.Alt;
      this.miRating4.Size = new System.Drawing.Size(285, 22);
      this.miRating4.Tag = (object) "4";
      this.miRating4.Text = "**** (4 Stars)";
      this.miRating5.Name = "miRating5";
      this.miRating5.ShortcutKeys = Keys.D5 | Keys.Shift | Keys.Alt;
      this.miRating5.Size = new System.Drawing.Size(285, 22);
      this.miRating5.Tag = (object) "5";
      this.miRating5.Text = "***** (5 Stars)";
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(282, 6);
      this.miQuickRating.Name = "miQuickRating";
      this.miQuickRating.ShortcutKeys = Keys.Q | Keys.Shift | Keys.Alt;
      this.miQuickRating.Size = new System.Drawing.Size(285, 22);
      this.miQuickRating.Text = "Quick Rating and Review...";
      this.miRateMenu.DropDown = (ToolStripDropDown) this.contextRating;
      this.miRateMenu.Name = "miRateMenu";
      this.miRateMenu.Size = new System.Drawing.Size(252, 22);
      this.miRateMenu.Text = "My &Rating";
      this.contextMarkAs.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.miMarkUnread,
        (ToolStripItem) this.miMarkRead,
        (ToolStripItem) this.toolStripMenuItem9,
        (ToolStripItem) this.miMarkChecked,
        (ToolStripItem) this.miMarkUnchecked
      });
      this.contextMarkAs.Name = "contextMarkAs";
      this.contextMarkAs.OwnerItem = (ToolStripItem) this.miMarkAs;
      this.contextMarkAs.Size = new System.Drawing.Size(203, 98);
      this.miMarkUnread.Name = "miMarkUnread";
      this.miMarkUnread.ShortcutKeys = Keys.U | Keys.Shift | Keys.Alt;
      this.miMarkUnread.Size = new System.Drawing.Size(202, 22);
      this.miMarkUnread.Text = "&Unread";
      this.miMarkRead.Name = "miMarkRead";
      this.miMarkRead.ShortcutKeys = Keys.R | Keys.Shift | Keys.Alt;
      this.miMarkRead.Size = new System.Drawing.Size(202, 22);
      this.miMarkRead.Text = "&Read";
      this.toolStripMenuItem9.Name = "toolStripMenuItem9";
      this.toolStripMenuItem9.Size = new System.Drawing.Size(199, 6);
      this.miMarkChecked.Name = "miMarkChecked";
      this.miMarkChecked.ShortcutKeys = Keys.M | Keys.Shift | Keys.Alt;
      this.miMarkChecked.Size = new System.Drawing.Size(202, 22);
      this.miMarkChecked.Text = "Checked";
      this.miMarkUnchecked.Name = "miMarkUnchecked";
      this.miMarkUnchecked.ShortcutKeys = Keys.V | Keys.Shift | Keys.Alt;
      this.miMarkUnchecked.Size = new System.Drawing.Size(202, 22);
      this.miMarkUnchecked.Text = "Unchecked";
      this.miMarkAs.DropDown = (ToolStripDropDown) this.contextMarkAs;
      this.miMarkAs.Name = "miMarkAs";
      this.miMarkAs.Size = new System.Drawing.Size(252, 22);
      this.miMarkAs.Text = "&Mark as";
      this.toolTip.AutomaticDelay = 1000;
      this.toolTip.Popup += new PopupEventHandler(this.toolTip_Popup);
      this.contextMenuItems.Items.AddRange(new ToolStripItem[33]
      {
        (ToolStripItem) this.miRead,
        (ToolStripItem) this.miReadTab,
        (ToolStripItem) this.miProperties,
        (ToolStripItem) this.miShowWeb,
        (ToolStripItem) this.miEdit,
        (ToolStripItem) this.toolStripMenuItem5,
        (ToolStripItem) this.miRateMenu,
        (ToolStripItem) this.miMarkAs,
        (ToolStripItem) this.miEditList,
        (ToolStripItem) this.miAddList,
        (ToolStripItem) this.tsMarkAsSeparator,
        (ToolStripItem) this.miAddLibrary,
        (ToolStripItem) this.miShowOnly,
        (ToolStripItem) this.miShowInList,
        (ToolStripItem) this.miExportComics,
        (ToolStripItem) this.miAutomation,
        (ToolStripItem) this.miUpdateComicFiles,
        (ToolStripItem) this.miRevealBrowser,
        (ToolStripItem) this.toolStripMenuItem7,
        (ToolStripItem) this.miCopyData,
        (ToolStripItem) this.miPasteData,
        (ToolStripItem) this.miClearData,
        (ToolStripItem) this.tsCopySeparator,
        (ToolStripItem) this.miSelectAll,
        (ToolStripItem) this.miInvertSelection,
        (ToolStripItem) this.miRefreshInformation,
        (ToolStripItem) this.sepListBackground,
        (ToolStripItem) this.miSetTopOfStack,
        (ToolStripItem) this.miSetStackThumbnail,
        (ToolStripItem) this.miRemoveStackThumbnail,
        (ToolStripItem) this.miSetListBackground,
        (ToolStripItem) this.toolStripRemoveSeparator,
        (ToolStripItem) this.miRemove
      });
      this.contextMenuItems.Name = "contextMenuFiles";
      this.contextMenuItems.Size = new System.Drawing.Size(253, 634);
      this.contextMenuItems.Closed += new ToolStripDropDownClosedEventHandler(this.contextMenuItems_Closed);
      this.contextMenuItems.Opening += new CancelEventHandler(this.contextMenuItems_Opening);
      this.contextMenuItems.Opened += new EventHandler(this.contextMenuItems_Opened);
      this.miRead.Image = (Image) Resources.Open;
      this.miRead.Name = "miRead";
      this.miRead.ShortcutKeys = Keys.O | Keys.Control;
      this.miRead.Size = new System.Drawing.Size(252, 22);
      this.miRead.Text = "&Open";
      this.miReadTab.Name = "miReadTab";
      this.miReadTab.ShortcutKeys = Keys.O | Keys.Shift | Keys.Control;
      this.miReadTab.Size = new System.Drawing.Size(252, 22);
      this.miReadTab.Text = "Open in new Tab";
      this.miProperties.Image = (Image) Resources.GetInfo;
      this.miProperties.Name = "miProperties";
      this.miProperties.ShortcutKeys = Keys.I | Keys.Control;
      this.miProperties.Size = new System.Drawing.Size(252, 22);
      this.miProperties.Text = "Info...";
      this.miShowWeb.Image = (Image) Resources.WebBlog;
      this.miShowWeb.Name = "miShowWeb";
      this.miShowWeb.ShortcutKeys = Keys.R | Keys.Control;
      this.miShowWeb.Size = new System.Drawing.Size(252, 22);
      this.miShowWeb.Text = "Web...";
      this.miEdit.Name = "miEdit";
      this.miEdit.ShortcutKeys = Keys.F2;
      this.miEdit.Size = new System.Drawing.Size(252, 22);
      this.miEdit.Text = "Edit";
      this.toolStripMenuItem5.Name = "toolStripMenuItem5";
      this.toolStripMenuItem5.Size = new System.Drawing.Size(249, 6);
      this.miEditList.DropDownItems.AddRange(new ToolStripItem[4]
      {
        (ToolStripItem) this.miEditListMoveToTop,
        (ToolStripItem) this.miEditListMoveToBottom,
        (ToolStripItem) this.toolStripMenuItem8,
        (ToolStripItem) this.miEditListApplyOrder
      });
      this.miEditList.Name = "miEditList";
      this.miEditList.Size = new System.Drawing.Size(252, 22);
      this.miEditList.Text = "Edit List";
      this.miEditListMoveToTop.Name = "miEditListMoveToTop";
      this.miEditListMoveToTop.ShortcutKeys = Keys.T | Keys.Control | Keys.Alt;
      this.miEditListMoveToTop.Size = new System.Drawing.Size(225, 22);
      this.miEditListMoveToTop.Text = "Move to Top";
      this.miEditListMoveToBottom.Name = "miEditListMoveToBottom";
      this.miEditListMoveToBottom.ShortcutKeys = Keys.B | Keys.Control | Keys.Alt;
      this.miEditListMoveToBottom.Size = new System.Drawing.Size(225, 22);
      this.miEditListMoveToBottom.Text = "Move to Bottom";
      this.toolStripMenuItem8.Name = "toolStripMenuItem8";
      this.toolStripMenuItem8.Size = new System.Drawing.Size(222, 6);
      this.miEditListApplyOrder.Name = "miEditListApplyOrder";
      this.miEditListApplyOrder.Size = new System.Drawing.Size(225, 22);
      this.miEditListApplyOrder.Text = "Apply current Order";
      this.miAddList.DropDownItems.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.dummyEntryToolStripMenuItem
      });
      this.miAddList.Name = "miAddList";
      this.miAddList.Size = new System.Drawing.Size(252, 22);
      this.miAddList.Text = "Add to List";
      this.miAddList.DropDownOpening += new EventHandler(this.miAddList_DropDownOpening);
      this.dummyEntryToolStripMenuItem.Name = "dummyEntryToolStripMenuItem";
      this.dummyEntryToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
      this.dummyEntryToolStripMenuItem.Text = "dummyEntry";
      this.tsMarkAsSeparator.Name = "tsMarkAsSeparator";
      this.tsMarkAsSeparator.Size = new System.Drawing.Size(249, 6);
      this.miAddLibrary.Name = "miAddLibrary";
      this.miAddLibrary.Size = new System.Drawing.Size(252, 22);
      this.miAddLibrary.Text = "&Add to Library";
      this.miShowOnly.Name = "miShowOnly";
      this.miShowOnly.Size = new System.Drawing.Size(252, 22);
      this.miShowOnly.Text = "&Browse Books";
      this.miShowInList.DropDownItems.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.dummyEntryToolStripMenuItem1
      });
      this.miShowInList.Name = "miShowInList";
      this.miShowInList.Size = new System.Drawing.Size(252, 22);
      this.miShowInList.Text = "Show in List";
      this.miShowInList.DropDownClosed += new EventHandler(this.miShowInList_DropDownClosed);
      this.miShowInList.DropDownOpening += new EventHandler(this.miShowInList_DropDownOpening);
      this.dummyEntryToolStripMenuItem1.Name = "dummyEntryToolStripMenuItem1";
      this.dummyEntryToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
      this.dummyEntryToolStripMenuItem1.Text = "dummyEntry";
      this.miExportComics.DropDown = (ToolStripDropDown) this.contextExport;
      this.miExportComics.Name = "miExportComics";
      this.miExportComics.Size = new System.Drawing.Size(252, 22);
      this.miExportComics.Text = "Export Books";
      this.contextExport.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.miExportComicsAs,
        (ToolStripItem) this.miExportComicsWithPrevious
      });
      this.contextExport.Name = "contextExport";
      this.contextExport.OwnerItem = (ToolStripItem) this.miExportComics;
      this.contextExport.Size = new System.Drawing.Size(245, 48);
      this.contextExport.Opening += new CancelEventHandler(this.contextExport_Opening);
      this.miExportComicsAs.Image = (Image) Resources.Save;
      this.miExportComicsAs.Name = "miExportComicsAs";
      this.miExportComicsAs.ShortcutKeys = Keys.E | Keys.Shift | Keys.Control;
      this.miExportComicsAs.Size = new System.Drawing.Size(244, 22);
      this.miExportComicsAs.Text = "Export Books...";
      this.miExportComicsWithPrevious.Name = "miExportComicsWithPrevious";
      this.miExportComicsWithPrevious.ShortcutKeys = Keys.E | Keys.Control | Keys.Alt;
      this.miExportComicsWithPrevious.Size = new System.Drawing.Size(244, 22);
      this.miExportComicsWithPrevious.Text = "Export with Previous";
      this.miAutomation.Name = "miAutomation";
      this.miAutomation.Size = new System.Drawing.Size(252, 22);
      this.miAutomation.Text = "Automation";
      this.miUpdateComicFiles.Image = (Image) Resources.UpdateSmall;
      this.miUpdateComicFiles.Name = "miUpdateComicFiles";
      this.miUpdateComicFiles.Size = new System.Drawing.Size(252, 22);
      this.miUpdateComicFiles.Text = "Update Book Files";
      this.miRevealBrowser.Image = (Image) Resources.RevealExplorer;
      this.miRevealBrowser.Name = "miRevealBrowser";
      this.miRevealBrowser.ShortcutKeys = Keys.G | Keys.Control;
      this.miRevealBrowser.Size = new System.Drawing.Size(252, 22);
      this.miRevealBrowser.Text = "Reveal Book in &Explorer";
      this.toolStripMenuItem7.Name = "toolStripMenuItem7";
      this.toolStripMenuItem7.Size = new System.Drawing.Size(249, 6);
      this.miCopyData.Image = (Image) Resources.EditCopy;
      this.miCopyData.Name = "miCopyData";
      this.miCopyData.ShortcutKeys = Keys.C | Keys.Control;
      this.miCopyData.Size = new System.Drawing.Size(252, 22);
      this.miCopyData.Text = "&Copy Data";
      this.miPasteData.Image = (Image) Resources.EditPaste;
      this.miPasteData.Name = "miPasteData";
      this.miPasteData.ShortcutKeys = Keys.V | Keys.Control;
      this.miPasteData.Size = new System.Drawing.Size(252, 22);
      this.miPasteData.Text = "&Paste Data...";
      this.miClearData.Name = "miClearData";
      this.miClearData.Size = new System.Drawing.Size(252, 22);
      this.miClearData.Text = "Clear Data";
      this.tsCopySeparator.Name = "tsCopySeparator";
      this.tsCopySeparator.Size = new System.Drawing.Size(249, 6);
      this.miSelectAll.Name = "miSelectAll";
      this.miSelectAll.ShortcutKeys = Keys.A | Keys.Control;
      this.miSelectAll.Size = new System.Drawing.Size(252, 22);
      this.miSelectAll.Text = "Select &All";
      this.miInvertSelection.Name = "miInvertSelection";
      this.miInvertSelection.Size = new System.Drawing.Size(252, 22);
      this.miInvertSelection.Text = "&Invert Selection";
      this.miRefreshInformation.Image = (Image) Resources.RefreshThumbnail;
      this.miRefreshInformation.Name = "miRefreshInformation";
      this.miRefreshInformation.Size = new System.Drawing.Size(252, 22);
      this.miRefreshInformation.Text = "Refresh";
      this.sepListBackground.Name = "sepListBackground";
      this.sepListBackground.Size = new System.Drawing.Size(249, 6);
      this.miSetTopOfStack.Name = "miSetTopOfStack";
      this.miSetTopOfStack.Size = new System.Drawing.Size(252, 22);
      this.miSetTopOfStack.Text = "Set as Top of Stack";
      this.miSetStackThumbnail.Name = "miSetStackThumbnail";
      this.miSetStackThumbnail.Size = new System.Drawing.Size(252, 22);
      this.miSetStackThumbnail.Text = "Set custom Stack Thumbnail...";
      this.miRemoveStackThumbnail.Name = "miRemoveStackThumbnail";
      this.miRemoveStackThumbnail.Size = new System.Drawing.Size(252, 22);
      this.miRemoveStackThumbnail.Text = "Remove custom Stack Thumbnail";
      this.miSetListBackground.Name = "miSetListBackground";
      this.miSetListBackground.Size = new System.Drawing.Size(252, 22);
      this.miSetListBackground.Text = "Set as List Background";
      this.toolStripRemoveSeparator.Name = "toolStripRemoveSeparator";
      this.toolStripRemoveSeparator.Size = new System.Drawing.Size(249, 6);
      this.miRemove.Image = (Image) Resources.EditDelete;
      this.miRemove.Name = "miRemove";
      this.miRemove.ShortcutKeys = Keys.Delete;
      this.miRemove.Size = new System.Drawing.Size(252, 22);
      this.miRemove.Text = "Re&move...";
      this.quickSearchTimer.Interval = 500;
      this.quickSearchTimer.Tick += new EventHandler(this.quickSearchTimer_Tick);
      this.contextQuickSearch.Items.AddRange(new ToolStripItem[8]
      {
        (ToolStripItem) this.miSearchAll,
        (ToolStripItem) this.toolStripSeparator5,
        (ToolStripItem) this.miSearchSeries,
        (ToolStripItem) this.miSearchWriter,
        (ToolStripItem) this.miSearchArtists,
        (ToolStripItem) this.miSearchDescriptive,
        (ToolStripItem) this.miSearchCatalog,
        (ToolStripItem) this.miSearchFile
      });
      this.contextQuickSearch.Name = "contextQuickSearch";
      this.contextQuickSearch.Size = new System.Drawing.Size(133, 164);
      this.miSearchAll.Checked = true;
      this.miSearchAll.CheckState = CheckState.Checked;
      this.miSearchAll.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.miSearchAll.ImageTransparentColor = Color.Magenta;
      this.miSearchAll.Name = "miSearchAll";
      this.miSearchAll.Size = new System.Drawing.Size(132, 22);
      this.miSearchAll.Text = "All";
      this.miSearchAll.ToolTipText = "Quick Search is checking all available data";
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(129, 6);
      this.miSearchSeries.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.miSearchSeries.ImageTransparentColor = Color.Magenta;
      this.miSearchSeries.Name = "miSearchSeries";
      this.miSearchSeries.Size = new System.Drawing.Size(132, 22);
      this.miSearchSeries.Text = "Series";
      this.miSearchSeries.ToolTipText = "Quick Search is only checking the Series name";
      this.miSearchWriter.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.miSearchWriter.ImageTransparentColor = Color.Magenta;
      this.miSearchWriter.Name = "miSearchWriter";
      this.miSearchWriter.Size = new System.Drawing.Size(132, 22);
      this.miSearchWriter.Text = "Writer";
      this.miSearchWriter.ToolTipText = "Quick Search is only checking the Writer";
      this.miSearchArtists.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.miSearchArtists.ImageTransparentColor = Color.Magenta;
      this.miSearchArtists.Name = "miSearchArtists";
      this.miSearchArtists.Size = new System.Drawing.Size(132, 22);
      this.miSearchArtists.Text = "Artists";
      this.miSearchArtists.ToolTipText = "Quick Search is checking all Artists";
      this.miSearchDescriptive.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.miSearchDescriptive.ImageTransparentColor = Color.Magenta;
      this.miSearchDescriptive.Name = "miSearchDescriptive";
      this.miSearchDescriptive.Size = new System.Drawing.Size(132, 22);
      this.miSearchDescriptive.Text = "Descriptive";
      this.miSearchDescriptive.ToolTipText = "Quick Search is checking all notes and summaries";
      this.miSearchCatalog.Name = "miSearchCatalog";
      this.miSearchCatalog.Size = new System.Drawing.Size(132, 22);
      this.miSearchCatalog.Text = "Catalog";
      this.miSearchCatalog.ToolTipText = "Quick Search is only checking the Catalog entries";
      this.miSearchFile.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.miSearchFile.ImageTransparentColor = Color.Magenta;
      this.miSearchFile.Name = "miSearchFile";
      this.miSearchFile.Size = new System.Drawing.Size(132, 22);
      this.miSearchFile.Text = "Filename";
      this.miSearchFile.ToolTipText = "Quick Search is only checking the Filename";
      this.itemView.AllowDrop = true;
      this.itemView.BackColor = SystemColors.Window;
      this.itemView.ColumnHeaderHeight = 19;
      this.itemView.Dock = DockStyle.Fill;
      this.itemView.ExpandedDetailColumnName = "Cover";
      this.itemView.GroupCollapsedImage = Resources.ArrowRight;
      this.itemView.GroupColumns = new IColumn[0];
      this.itemView.GroupColumnsKey = (string) null;
      this.itemView.GroupExpandedImage = Resources.ArrowDown;
      this.itemView.GroupsStatus = (ItemViewGroupsStatus) componentResourceManager.GetObject("itemView.GroupsStatus");
      this.itemView.HideSelection = false;
      this.itemView.ItemRowHeight = 19;
      this.itemView.Location = new System.Drawing.Point(0, 0);
      this.itemView.Name = "itemView";
      this.itemView.Size = new System.Drawing.Size(710, 172);
      this.itemView.SortColumn = (IColumn) null;
      this.itemView.SortColumns = new IColumn[0];
      this.itemView.SortColumnsKey = (string) null;
      this.itemView.StackColumns = new IColumn[0];
      this.itemView.StackColumnsKey = (string) null;
      this.itemView.StackDisplayEnabled = true;
      this.itemView.TabIndex = 0;
      this.itemView.ProcessStack += new EventHandler<ItemView.StackEventArgs>(this.itemView_ProcessStack);
      this.itemView.ItemActivate += new EventHandler(this.itemView_ItemActivate);
      this.itemView.SelectedIndexChanged += new EventHandler(this.itemView_SelectedIndexChanged);
      this.itemView.ItemDisplayChanged += new EventHandler(this.itemView_ItemDisplayChanged);
      this.itemView.GroupDisplayChanged += new EventHandler(this.itemView_GroupDisplayChanged);
      this.itemView.ItemDrag += new ItemDragEventHandler(this.itemView_ItemDrag);
      this.itemView.PostPaint += new PaintEventHandler(this.itemView_PostPaint);
      this.itemView.DragDrop += new DragEventHandler(this.itemView_DragDrop);
      this.itemView.DragEnter += new DragEventHandler(this.itemView_DragEnter);
      this.itemView.DragOver += new DragEventHandler(this.itemView_DragOver);
      this.itemView.DragLeave += new EventHandler(this.itemView_DragLeave);
      this.itemView.GiveFeedback += new GiveFeedbackEventHandler(this.DragGiveDragCursorFeedback);
      this.itemView.QueryContinueDrag += new QueryContinueDragEventHandler(this.DragQueryContinueDrag);
      this.itemView.KeyDown += new KeyEventHandler(this.itemView_KeyDown);
      this.itemView.MouseLeave += new EventHandler(this.foundView_MouseLeave);
      this.itemView.MouseHover += new EventHandler(this.foundView_MouseHover);
      this.itemView.MouseMove += new MouseEventHandler(this.foundView_MouseMove);
      this.displayOptionPanel.Controls.Add((Control) this.lblDisplayOptionText);
      this.displayOptionPanel.Controls.Add((Control) this.btDisplayAll);
      this.displayOptionPanel.Dock = DockStyle.Bottom;
      this.displayOptionPanel.Location = new System.Drawing.Point(0, 396);
      this.displayOptionPanel.Name = "displayOptionPanel";
      this.displayOptionPanel.Size = new System.Drawing.Size(710, 39);
      this.displayOptionPanel.TabIndex = 7;
      this.displayOptionPanel.Visible = false;
      this.lblDisplayOptionText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.lblDisplayOptionText.Location = new System.Drawing.Point(5, 11);
      this.lblDisplayOptionText.Name = "lblDisplayOptionText";
      this.lblDisplayOptionText.Size = new System.Drawing.Size(598, 18);
      this.lblDisplayOptionText.TabIndex = 1;
      this.lblDisplayOptionText.Text = "Because of active Views options, not all Books are displayed.";
      this.btDisplayAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btDisplayAll.Location = new System.Drawing.Point(609, 6);
      this.btDisplayAll.Name = "btDisplayAll";
      this.btDisplayAll.Size = new System.Drawing.Size(90, 23);
      this.btDisplayAll.TabIndex = 0;
      this.btDisplayAll.Text = "Display &All";
      this.btDisplayAll.UseVisualStyleBackColor = true;
      this.btDisplayAll.Click += new EventHandler(this.btDisplayAll_Click);
      this.searchBrowserContainer.Controls.Add((Control) this.bookSelectorPanel);
      this.searchBrowserContainer.Dock = DockStyle.Top;
      this.searchBrowserContainer.Grip = SizableContainer.GripPosition.Bottom;
      this.searchBrowserContainer.Location = new System.Drawing.Point(0, 64);
      this.searchBrowserContainer.Name = "searchBrowserContainer";
      this.searchBrowserContainer.Padding = new Padding(0, 6, 0, 0);
      this.searchBrowserContainer.Size = new System.Drawing.Size(710, 160);
      this.searchBrowserContainer.TabIndex = 6;
      this.searchBrowserContainer.Text = "sizableContainer1";
      this.searchBrowserContainer.ExpandedChanged += new EventHandler(this.searchBrowserContainer_ExpandedChanged);
      this.bookSelectorPanel.Dock = DockStyle.Fill;
      this.bookSelectorPanel.Location = new System.Drawing.Point(0, 6);
      this.bookSelectorPanel.Name = "bookSelectorPanel";
      this.bookSelectorPanel.Size = new System.Drawing.Size(710, 148);
      this.bookSelectorPanel.TabIndex = 0;
      this.bookSelectorPanel.CurrentMatcherChanged += new EventHandler(this.bookSelectorPanel_FilterChanged);
      this.bookSelectorPanel.ItemDrag += new ItemDragEventHandler(this.bookSelectorPanel_ItemDrag);
      this.bookSelectorPanel.GiveFeedback += new GiveFeedbackEventHandler(this.DragGiveDragCursorFeedback);
      this.bookSelectorPanel.QueryContinueDrag += new QueryContinueDragEventHandler(this.DragQueryContinueDrag);
      this.openStackPanel.BorderStyle = BorderStyle.FixedSingle;
      this.openStackPanel.Controls.Add((Control) this.btPrevStack);
      this.openStackPanel.Controls.Add((Control) this.btNextStack);
      this.openStackPanel.Controls.Add((Control) this.lblOpenStackText);
      this.openStackPanel.Controls.Add((Control) this.btCloseStack);
      this.openStackPanel.Dock = DockStyle.Top;
      this.openStackPanel.Location = new System.Drawing.Point(0, 25);
      this.openStackPanel.Name = "openStackPanel";
      this.openStackPanel.Size = new System.Drawing.Size(710, 39);
      this.openStackPanel.TabIndex = 8;
      this.openStackPanel.Visible = false;
      this.btPrevStack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btPrevStack.Location = new System.Drawing.Point(541, 6);
      this.btPrevStack.Name = "btPrevStack";
      this.btPrevStack.Size = new System.Drawing.Size(75, 23);
      this.btPrevStack.TabIndex = 2;
      this.btPrevStack.Text = "&Previous";
      this.btPrevStack.UseVisualStyleBackColor = true;
      this.btPrevStack.Click += new EventHandler(this.btPrevStack_Click);
      this.btNextStack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btNextStack.Location = new System.Drawing.Point(622, 6);
      this.btNextStack.Name = "btNextStack";
      this.btNextStack.Size = new System.Drawing.Size(75, 23);
      this.btNextStack.TabIndex = 3;
      this.btNextStack.Text = "&Next";
      this.btNextStack.UseVisualStyleBackColor = true;
      this.btNextStack.Click += new EventHandler(this.btNextStack_Click);
      this.lblOpenStackText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.lblOpenStackText.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.lblOpenStackText.Location = new System.Drawing.Point(119, 8);
      this.lblOpenStackText.Name = "lblOpenStackText";
      this.lblOpenStackText.Size = new System.Drawing.Size(416, 18);
      this.lblOpenStackText.TabIndex = 1;
      this.lblOpenStackText.Text = "Lorem Ipsum";
      this.lblOpenStackText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.btCloseStack.Location = new System.Drawing.Point(6, 6);
      this.btCloseStack.Name = "btCloseStack";
      this.btCloseStack.Size = new System.Drawing.Size(107, 23);
      this.btCloseStack.TabIndex = 0;
      this.btCloseStack.Text = "&Close Stack";
      this.btCloseStack.UseVisualStyleBackColor = true;
      this.btCloseStack.Click += new EventHandler(this.btCloseStack_Click);
      this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;
      this.toolStrip.Items.AddRange(new ToolStripItem[15]
      {
        (ToolStripItem) this.tbSidebar,
        (ToolStripItem) this.btBrowsePrev,
        (ToolStripItem) this.btBrowseNext,
        (ToolStripItem) this.tbBrowseSeparator,
        (ToolStripItem) this.tbbView,
        (ToolStripItem) this.tbbGroup,
        (ToolStripItem) this.tbbStack,
        (ToolStripItem) this.tbbSort,
        (ToolStripItem) this.tsQuickSearch,
        (ToolStripItem) this.tsListLayouts,
        (ToolStripItem) this.sepDuplicateList,
        (ToolStripItem) this.tbbDuplicateList,
        (ToolStripItem) this.sepUndo,
        (ToolStripItem) this.tbUndo,
        (ToolStripItem) this.tbRedo
      });
      this.toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
      this.toolStrip.Location = new System.Drawing.Point(0, 0);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(710, 25);
      this.toolStrip.TabIndex = 1;
      this.tbSidebar.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbSidebar.Image = (Image) Resources.Sidebar;
      this.tbSidebar.ImageTransparentColor = Color.Magenta;
      this.tbSidebar.Name = "tbSidebar";
      this.tbSidebar.Size = new System.Drawing.Size(23, 22);
      this.tbSidebar.Text = "Sidebar";
      this.btBrowsePrev.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.btBrowsePrev.Enabled = false;
      this.btBrowsePrev.Image = (Image) Resources.BrowsePrevious;
      this.btBrowsePrev.ImageTransparentColor = Color.Magenta;
      this.btBrowsePrev.Name = "btBrowsePrev";
      this.btBrowsePrev.Size = new System.Drawing.Size(23, 22);
      this.btBrowsePrev.Text = "Previous";
      this.btBrowseNext.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.btBrowseNext.Enabled = false;
      this.btBrowseNext.Image = (Image) Resources.BrowseNext;
      this.btBrowseNext.ImageTransparentColor = Color.Magenta;
      this.btBrowseNext.Name = "btBrowseNext";
      this.btBrowseNext.Size = new System.Drawing.Size(23, 22);
      this.btBrowseNext.Text = "Next";
      this.tbBrowseSeparator.Name = "tbBrowseSeparator";
      this.tbBrowseSeparator.Size = new System.Drawing.Size(6, 25);
      this.tbbView.DropDownItems.AddRange(new ToolStripItem[16]
      {
        (ToolStripItem) this.miViewThumbnails,
        (ToolStripItem) this.miViewTiles,
        (ToolStripItem) this.miViewDetails,
        (ToolStripItem) this.toolStripMenuItem6,
        (ToolStripItem) this.miExpandAllGroups,
        (ToolStripItem) this.miShowGroupHeaders,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.miShowOnlyAllComics,
        (ToolStripItem) this.miShowOnlyUnreadComics,
        (ToolStripItem) this.miShowOnlyReadingComics,
        (ToolStripItem) this.miShowOnlyReadComics,
        (ToolStripItem) this.toolStripMenuItem4,
        (ToolStripItem) this.miShowOnlyComics,
        (ToolStripItem) this.miShowOnlyFileless,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.miShowOnlyDuplicates
      });
      this.tbbView.Image = (Image) Resources.View;
      this.tbbView.ImageTransparentColor = Color.Magenta;
      this.tbbView.Name = "tbbView";
      this.tbbView.Size = new System.Drawing.Size(69, 22);
      this.tbbView.Text = "Views";
      this.tbbView.ToolTipText = "Change how and what Books are displayed";
      this.tbbView.ButtonClick += new EventHandler(this.tbbView_ButtonClick);
      this.miViewThumbnails.Image = (Image) Resources.ThumbView;
      this.miViewThumbnails.Name = "miViewThumbnails";
      this.miViewThumbnails.Size = new System.Drawing.Size(259, 22);
      this.miViewThumbnails.Text = "T&humbnails";
      this.miViewTiles.Image = (Image) Resources.TileView;
      this.miViewTiles.Name = "miViewTiles";
      this.miViewTiles.Size = new System.Drawing.Size(259, 22);
      this.miViewTiles.Text = "&Tiles";
      this.miViewDetails.Image = (Image) Resources.DetailView;
      this.miViewDetails.Name = "miViewDetails";
      this.miViewDetails.Size = new System.Drawing.Size(259, 22);
      this.miViewDetails.Text = "&Details";
      this.toolStripMenuItem6.Name = "toolStripMenuItem6";
      this.toolStripMenuItem6.Size = new System.Drawing.Size(256, 6);
      this.miExpandAllGroups.Name = "miExpandAllGroups";
      this.miExpandAllGroups.Size = new System.Drawing.Size(259, 22);
      this.miExpandAllGroups.Text = "Collapse/Expand all Groups";
      this.miShowGroupHeaders.Name = "miShowGroupHeaders";
      this.miShowGroupHeaders.ShortcutKeys = Keys.G | Keys.Shift | Keys.Control;
      this.miShowGroupHeaders.Size = new System.Drawing.Size(259, 22);
      this.miShowGroupHeaders.Text = "Show Group Headers";
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(256, 6);
      this.miShowOnlyAllComics.Checked = true;
      this.miShowOnlyAllComics.CheckState = CheckState.Checked;
      this.miShowOnlyAllComics.Name = "miShowOnlyAllComics";
      this.miShowOnlyAllComics.Size = new System.Drawing.Size(259, 22);
      this.miShowOnlyAllComics.Text = "Show All";
      this.miShowOnlyUnreadComics.Name = "miShowOnlyUnreadComics";
      this.miShowOnlyUnreadComics.Size = new System.Drawing.Size(259, 22);
      this.miShowOnlyUnreadComics.Text = "Show not Read";
      this.miShowOnlyReadingComics.Name = "miShowOnlyReadingComics";
      this.miShowOnlyReadingComics.Size = new System.Drawing.Size(259, 22);
      this.miShowOnlyReadingComics.Text = "Show Reading";
      this.miShowOnlyReadComics.Name = "miShowOnlyReadComics";
      this.miShowOnlyReadComics.Size = new System.Drawing.Size(259, 22);
      this.miShowOnlyReadComics.Text = "Show Read";
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(256, 6);
      this.miShowOnlyComics.Name = "miShowOnlyComics";
      this.miShowOnlyComics.Size = new System.Drawing.Size(259, 22);
      this.miShowOnlyComics.Text = "Show only Books";
      this.miShowOnlyFileless.Name = "miShowOnlyFileless";
      this.miShowOnlyFileless.Size = new System.Drawing.Size(259, 22);
      this.miShowOnlyFileless.Text = "Show only fileless Entries";
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(256, 6);
      this.miShowOnlyDuplicates.Name = "miShowOnlyDuplicates";
      this.miShowOnlyDuplicates.Size = new System.Drawing.Size(259, 22);
      this.miShowOnlyDuplicates.Text = "Show Duplicates";
      this.tbbGroup.DropDownItems.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.dummyToolStripMenuItem
      });
      this.tbbGroup.Image = (Image) Resources.Group;
      this.tbbGroup.ImageTransparentColor = Color.Magenta;
      this.tbbGroup.Name = "tbbGroup";
      this.tbbGroup.Size = new System.Drawing.Size(72, 22);
      this.tbbGroup.Text = "Group";
      this.tbbGroup.ToolTipText = "Group Books by different criteria";
      this.tbbGroup.ButtonClick += new EventHandler(this.tbbGroup_ButtonClick);
      this.tbbGroup.DropDownOpening += new EventHandler(this.tbbGroup_DropDownOpening);
      this.dummyToolStripMenuItem.Name = "dummyToolStripMenuItem";
      this.dummyToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
      this.dummyToolStripMenuItem.Text = "Dummy";
      this.tbbStack.DropDownItems.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.dummyToolStripMenuItem1
      });
      this.tbbStack.Image = (Image) Resources.Stacking;
      this.tbbStack.ImageTransparentColor = Color.Magenta;
      this.tbbStack.Name = "tbbStack";
      this.tbbStack.Size = new System.Drawing.Size(67, 22);
      this.tbbStack.Text = "Stack";
      this.tbbStack.ButtonClick += new EventHandler(this.tbbStack_ButtonClick);
      this.tbbStack.DropDownOpening += new EventHandler(this.tbbStack_DropDownOpening);
      this.dummyToolStripMenuItem1.Name = "dummyToolStripMenuItem1";
      this.dummyToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
      this.dummyToolStripMenuItem1.Text = "Dummy";
      this.tbbSort.DropDownItems.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.dummyToolStripMenuItem2
      });
      this.tbbSort.Image = (Image) Resources.SortUp;
      this.tbbSort.ImageTransparentColor = Color.Magenta;
      this.tbbSort.Name = "tbbSort";
      this.tbbSort.Size = new System.Drawing.Size(81, 22);
      this.tbbSort.Text = "Arrange";
      this.tbbSort.ToolTipText = "Change the sort order of the Books";
      this.tbbSort.ButtonClick += new EventHandler(this.tbbSort_ButtonClick);
      this.tbbSort.DropDownOpening += new EventHandler(this.tbbSort_DropDownOpening);
      this.dummyToolStripMenuItem2.Name = "dummyToolStripMenuItem2";
      this.dummyToolStripMenuItem2.Size = new System.Drawing.Size(117, 22);
      this.dummyToolStripMenuItem2.Text = "Dummy";
      this.tsQuickSearch.Alignment = ToolStripItemAlignment.Right;
      this.tsQuickSearch.CausesValidation = false;
      this.tsQuickSearch.Margin = new Padding(0, 1, 1, 0);
      this.tsQuickSearch.Name = "tsQuickSearch";
      this.tsQuickSearch.Overflow = ToolStripItemOverflow.Never;
      this.tsQuickSearch.Size = new System.Drawing.Size(130, 24);
      this.tsQuickSearch.Enter += new EventHandler(this.tsQuickSearch_Enter);
      this.tsQuickSearch.Leave += new EventHandler(this.tsQuickSearch_Leave);
      this.tsQuickSearch.TextChanged += new EventHandler(this.tsQuickSearch_TextChanged);
      this.tsListLayouts.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsListLayouts.DropDownItems.AddRange(new ToolStripItem[6]
      {
        (ToolStripItem) this.tsEditListLayout,
        (ToolStripItem) this.tsSaveListLayout,
        (ToolStripItem) this.miResetListBackground,
        (ToolStripItem) this.toolStripMenuItem23,
        (ToolStripItem) this.tsEditLayouts,
        (ToolStripItem) this.separatorListLayout
      });
      this.tsListLayouts.Image = (Image) Resources.ListLayout;
      this.tsListLayouts.ImageTransparentColor = Color.Magenta;
      this.tsListLayouts.Name = "tsListLayouts";
      this.tsListLayouts.RightToLeft = RightToLeft.No;
      this.tsListLayouts.Size = new System.Drawing.Size(29, 22);
      this.tsListLayouts.Text = "List Layouts";
      this.tsListLayouts.ToolTipText = "Manage List Layouts";
      this.tsListLayouts.DropDownOpening += new EventHandler(this.tsListLayouts_DropDownOpening);
      this.tsEditListLayout.Name = "tsEditListLayout";
      this.tsEditListLayout.ShortcutKeys = Keys.L | Keys.Control;
      this.tsEditListLayout.Size = new System.Drawing.Size(210, 22);
      this.tsEditListLayout.Text = "&Edit List Layout...";
      this.tsSaveListLayout.Name = "tsSaveListLayout";
      this.tsSaveListLayout.Size = new System.Drawing.Size(210, 22);
      this.tsSaveListLayout.Text = "&Save List Layout...";
      this.miResetListBackground.Name = "miResetListBackground";
      this.miResetListBackground.Size = new System.Drawing.Size(210, 22);
      this.miResetListBackground.Text = "Reset List Background";
      this.toolStripMenuItem23.Name = "toolStripMenuItem23";
      this.toolStripMenuItem23.Size = new System.Drawing.Size(207, 6);
      this.tsEditLayouts.Name = "tsEditLayouts";
      this.tsEditLayouts.ShortcutKeys = Keys.L | Keys.Control | Keys.Alt;
      this.tsEditLayouts.Size = new System.Drawing.Size(210, 22);
      this.tsEditLayouts.Text = "&Edit Layouts...";
      this.separatorListLayout.Name = "separatorListLayout";
      this.separatorListLayout.Size = new System.Drawing.Size(207, 6);
      this.sepDuplicateList.Name = "sepDuplicateList";
      this.sepDuplicateList.Size = new System.Drawing.Size(6, 25);
      this.tbbDuplicateList.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbbDuplicateList.DropDownItems.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.dummyEntryToolStripMenuItem2
      });
      this.tbbDuplicateList.Image = (Image) Resources.AddList;
      this.tbbDuplicateList.ImageTransparentColor = Color.Magenta;
      this.tbbDuplicateList.Name = "tbbDuplicateList";
      this.tbbDuplicateList.Size = new System.Drawing.Size(29, 22);
      this.tbbDuplicateList.Text = "Duplicate";
      this.tbbDuplicateList.ToolTipText = "Duplicate current List";
      this.tbbDuplicateList.DropDownOpening += new EventHandler(this.tbbDuplicateList_DropDownOpening);
      this.dummyEntryToolStripMenuItem2.Name = "dummyEntryToolStripMenuItem2";
      this.dummyEntryToolStripMenuItem2.Size = new System.Drawing.Size(143, 22);
      this.dummyEntryToolStripMenuItem2.Text = "dummyEntry";
      this.sepUndo.Name = "sepUndo";
      this.sepUndo.Size = new System.Drawing.Size(6, 25);
      this.tbUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbUndo.Image = (Image) Resources.Undo;
      this.tbUndo.ImageTransparentColor = Color.Magenta;
      this.tbUndo.Name = "tbUndo";
      this.tbUndo.Size = new System.Drawing.Size(23, 22);
      this.tbUndo.Text = "Undo";
      this.tbRedo.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbRedo.Image = (Image) Resources.Redo;
      this.tbRedo.ImageTransparentColor = Color.Magenta;
      this.tbRedo.Name = "tbRedo";
      this.tbRedo.Size = new System.Drawing.Size(23, 22);
      this.tbRedo.Text = "Redo";
      this.lvGroupHeaders.Columns.AddRange(new ColumnHeader[2]
      {
        this.lvGroupsName,
        this.lvGroupsCount
      });
      this.lvGroupHeaders.Dock = DockStyle.Fill;
      this.lvGroupHeaders.FullRowSelect = true;
      this.lvGroupHeaders.Location = new System.Drawing.Point(0, 0);
      this.lvGroupHeaders.MultiSelect = false;
      this.lvGroupHeaders.Name = "lvGroupHeaders";
      this.lvGroupHeaders.Size = new System.Drawing.Size(234, 172);
      this.lvGroupHeaders.TabIndex = 1;
      this.lvGroupHeaders.UseCompatibleStateImageBehavior = false;
      this.lvGroupHeaders.View = View.Details;
      this.lvGroupHeaders.ColumnClick += new ColumnClickEventHandler(this.lvGroupHeaders_ColumnClick);
      this.lvGroupHeaders.SelectedIndexChanged += new EventHandler(this.lvGroupHeaders_SelectedIndexChanged);
      this.lvGroupHeaders.ClientSizeChanged += new EventHandler(this.lvGroupHeaders_ClientSizeChanged);
      this.lvGroupsName.Text = "Group";
      this.lvGroupsName.Width = 162;
      this.lvGroupsCount.Text = "#";
      this.lvGroupsCount.TextAlign = HorizontalAlignment.Right;
      this.browserContainer.Dock = DockStyle.Fill;
      this.browserContainer.FixedPanel = FixedPanel.Panel2;
      this.browserContainer.Location = new System.Drawing.Point(0, 224);
      this.browserContainer.Name = "browserContainer";
      this.browserContainer.Panel1.Controls.Add((Control) this.itemView);
      this.browserContainer.Panel2.Controls.Add((Control) this.lvGroupHeaders);
      this.browserContainer.Panel2Collapsed = true;
      this.browserContainer.Panel2MinSize = 200;
      this.browserContainer.Size = new System.Drawing.Size(710, 172);
      this.browserContainer.SplitterDistance = 472;
      this.browserContainer.TabIndex = 2;
      this.browserContainer.DoubleClick += new EventHandler(this.browserContainer_DoubleClick);
      this.AutoScaleMode = AutoScaleMode.None;
      this.Controls.Add((Control) this.browserContainer);
      this.Controls.Add((Control) this.displayOptionPanel);
      this.Controls.Add((Control) this.searchBrowserContainer);
      this.Controls.Add((Control) this.openStackPanel);
      this.Controls.Add((Control) this.toolStrip);
      this.Name = nameof (ComicBrowserControl);
      this.Size = new System.Drawing.Size(710, 435);
      this.contextRating.ResumeLayout(false);
      this.contextMarkAs.ResumeLayout(false);
      this.contextMenuItems.ResumeLayout(false);
      this.contextExport.ResumeLayout(false);
      this.contextQuickSearch.ResumeLayout(false);
      this.displayOptionPanel.ResumeLayout(false);
      this.searchBrowserContainer.ResumeLayout(false);
      this.openStackPanel.ResumeLayout(false);
      this.toolStrip.ResumeLayout(false);
      this.toolStrip.PerformLayout();
      this.browserContainer.Panel1.ResumeLayout(false);
      this.browserContainer.Panel2.ResumeLayout(false);
      this.browserContainer.EndInit();
      this.browserContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private class StackMatcher : ComicBookMatcher
    {
      private readonly HashSet<ComicBook> items;

      public StackMatcher(
        IGrouper<IViewableItem> grouper,
        string caption,
        HashSet<ComicBook> items)
      {
        this.items = items;
        this.Grouper = grouper;
        this.Caption = caption;
      }

      public bool Match(ComicBook item) => this.items.Contains(item);

      public override IEnumerable<ComicBook> Match(IEnumerable<ComicBook> items)
      {
        return items.Where<ComicBook>(new Func<ComicBook, bool>(this.Match));
      }

      public override object Clone()
      {
        return (object) new ComicBrowserControl.StackMatcher(this.Grouper, this.Caption, this.items);
      }

      public string Caption { get; private set; }

      public IGrouper<IViewableItem> Grouper { get; private set; }
    }

    private class CoverViewItemPropertyComparer : CoverViewItemComparer
    {
      private readonly string property;

      public CoverViewItemPropertyComparer(string property) => this.property = property;

      protected override int OnCompare(CoverViewItem x, CoverViewItem y)
      {
        return ExtendedStringComparer.Compare(x.Comic.GetStringPropertyValue(this.property), y.Comic.GetStringPropertyValue(this.property));
      }
    }

    private class CoverViewItemPropertyGrouper : IGrouper<IViewableItem>
    {
      private readonly string property;

      public CoverViewItemPropertyGrouper(string property) => this.property = property;

      public bool IsMultiGroup => false;

      public IEnumerable<IGroupInfo> GetGroups(IViewableItem item)
      {
        throw new NotImplementedException();
      }

      public IGroupInfo GetGroup(IViewableItem item)
      {
        return SingleComicGrouper.GetNameGroup(((CoverViewItem) item).Comic.GetStringPropertyValue(this.property));
      }
    }
  }
}

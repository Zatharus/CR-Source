// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Controls.PagesView
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Controls
{
  public class PagesView : UserControl, IEditBookmark, IEditPage
  {
    private volatile bool listDirty;
    private readonly CommandMapper command = new CommandMapper();
    private EnumMenuUtility pageMenu;
    private EnumMenuUtility rotateMenu;
    private bool createBackdrop = true;
    private ComicBookNavigator book;
    private ComicPageType pageFilter = ComicPageType.All;
    private ComicPageInfo[] dragPages;
    private IBitmapCursor dragCursor;
    private IContainer components;
    private ItemView itemView;
    private ContextMenuStrip contextPages;
    private ToolStripMenuItem miPageType;
    private ToolStripSeparator tsPageTypeSeparator;
    private ToolStripMenuItem miCopy;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem miSelectAll;
    private ToolStripMenuItem miInvertSelection;
    private ToolStripMenuItem miRefreshThumbnail;
    private ToolStripSeparator toolStripMenuItem3;
    private ToolStripMenuItem miMarkDeleted;
    private ToolStripSeparator tsMovePagesSeparator;
    private ToolStripMenuItem miSetBookmark;
    private ToolStripMenuItem miRemoveBookmark;
    private ToolStripSeparator tsBookmarkSeparator;
    private ToolStripMenuItem miMoveToTop;
    private ToolStripMenuItem miMoveToBottom;
    private ToolStripMenuItem miResetOriginalOrder;
    private ToolStripMenuItem cmPageRotate;
    private ToolStripMenuItem miPagePosition;
    private ToolStripMenuItem miPagePositionDefault;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem miPagePositionNear;
    private ToolStripMenuItem miPagePositionFar;

    public PagesView()
    {
      this.InitializeComponent();
      this.components.Add((IComponent) this.command);
      LocalizeUtility.Localize((Control) this, this.components);
      this.itemView.ScrollResizeRefresh = Program.ExtendedSettings.OptimizedListScrolling;
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(0, "Page", 40, (object) new ComicListField("Page", "The page number of the image"), (IComparer<IViewableItem>) new PageViewItemPageComparer(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(1, "Thumbnail", 80, (object) new ComicListField("Thumbnail", "The thumbnail image of the page")));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(2, "Type", 80, (object) new ComicListField("PageTypeAsText", "The type of the page (story, cover, etc.)"), (IComparer<IViewableItem>) new PageViewItemComparer<ComicPageInfoTypeComparer>()));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(3, "Size", 80, (object) new ComicListField("ImageFileSizeAsText", "Size of the page in bytes"), (IComparer<IViewableItem>) new PageViewItemComparer<ComicPageInfoImageSizeComparer>(), (IGrouper<IViewableItem>) new PageViewItemGrouper<ComicPageInfoGroupImageSize>(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(4, "Width", 60, (object) new ComicListField("ImageWidthAsText", "Width of the page in pixels"), (IComparer<IViewableItem>) new PageViewItemComparer<ComicPageInfoImageWidthComparer>(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(5, "Height", 60, (object) new ComicListField("ImageHeightAsText", "Height of the page in pixels"), (IComparer<IViewableItem>) new PageViewItemComparer<ComicPageInfoImageHeightComparer>(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(6, "Name", 150, (object) new ComicListField("Key", "Unique key for this page in the Book"), (IComparer<IViewableItem>) new PageViewItemKeyComparer()));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(7, nameof (Bookmark), 150, (object) new ComicListField(nameof (Bookmark), "Bookmark Description"), (IComparer<IViewableItem>) new PageViewItemComparer<ComicPageBookmarkComparer>(), (IGrouper<IViewableItem>) new PageViewBookmarkGrouper()));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(8, "Rotation", 60, (object) new ComicListField("RotationAsText", "Permanent rotation of this page"), (IComparer<IViewableItem>) new PageViewItemComparer<ComicPageRotationComparer>(), align: StringAlignment.Far));
      this.itemView.Columns.Add((IColumn) new ItemViewColumn(9, "Position", 60, (object) new ComicListField("PagePositionAsText", "Layout Position of this page"), (IComparer<IViewableItem>) new PageViewItemComparer<ComicPagePositionComparer>()));
      foreach (ItemViewColumn column in (SmartList<IColumn>) this.itemView.Columns)
        column.Width = FormUtility.ScaleDpiX(column.Width);
      this.itemView.SortColumn = this.itemView.Columns[0];
      this.itemView.Font = SystemFonts.IconTitleFont;
      this.itemView.ItemRowHeight = this.itemView.Font.Height + FormUtility.ScaleDpiY(6);
      this.itemView.ItemThumbSize = this.itemView.ItemThumbSize.ScaleDpi();
      this.itemView.ColumnHeaderHeight = this.itemView.ItemRowHeight;
      this.itemView.MouseWheel += new MouseEventHandler(this.ItemViewMouseWheel);
      IdleProcess.Idle += new EventHandler(this.Application_Idle);
      KeySearch.Create(this.itemView);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        IdleProcess.Idle -= new EventHandler(this.Application_Idle);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DesignMode)
        return;
      ContextMenuStrip contextMenuStrip = this.itemView.AutoViewContextMenuStrip;
      contextMenuStrip.Items.Add((ToolStripItem) new ToolStripSeparator());
      ToolStripMenuItem toolStripMenuItem;
      contextMenuStrip.Items.Add((ToolStripItem) (toolStripMenuItem = this.miSelectAll.Clone()));
      this.command.Add(new CommandHandler(this.itemView.SelectAll), (object) this.miSelectAll, (object) toolStripMenuItem);
      this.command.Add(new CommandHandler(this.itemView.InvertSelection), (object) this.miInvertSelection);
      this.command.Add(new CommandHandler(this.CopyPage), (object) this.miCopy);
      this.command.Add(new CommandHandler(this.RefreshDisplay), (object) this.miRefreshThumbnail);
      this.command.Add(new CommandHandler(this.MarkAsDeleted), (object) this.miMarkDeleted);
      this.command.Add(new CommandHandler(this.SetBookmark), (UpdateHandler) (() => this.CanBookmark), (object) this.miSetBookmark);
      this.command.Add((CommandHandler) (() => this.Bookmark = string.Empty), (UpdateHandler) (() => this.CanBookmark && !string.IsNullOrEmpty(this.Bookmark)), (object) this.miRemoveBookmark);
      this.command.Add(new CommandHandler(this.MoveSelectedPageStart), (UpdateHandler) (() => this.itemView.SelectedCount > 0), (object) this.miMoveToTop);
      this.command.Add(new CommandHandler(this.MoveSelectedPageEnd), (UpdateHandler) (() => this.itemView.SelectedCount > 0), (object) this.miMoveToBottom);
      this.command.Add(new CommandHandler(this.ResetPageOrder), (UpdateHandler) (() => this.itemView.SelectedCount > 0), (object) this.miResetOriginalOrder);
      this.command.Add((CommandHandler) (() => this.SetPagePosition(ComicPagePosition.Default)), (UpdateHandler) (() => this.itemView.SelectedCount > 0), (object) this.miPagePositionDefault);
      this.command.Add((CommandHandler) (() => this.SetPagePosition(ComicPagePosition.Near)), (UpdateHandler) (() => this.itemView.SelectedCount > 0), (object) this.miPagePositionNear);
      this.command.Add((CommandHandler) (() => this.SetPagePosition(ComicPagePosition.Far)), (UpdateHandler) (() => this.itemView.SelectedCount > 0), (object) this.miPagePositionFar);
      this.pageMenu = new EnumMenuUtility((ToolStripDropDownItem) this.miPageType, typeof (ComicPageType), false, (IDictionary<int, Image>) null, Keys.A | Keys.Shift | Keys.Alt);
      this.pageMenu.ValueChanged += new EventHandler(this.PageMenuValueChanged);
      this.rotateMenu = new EnumMenuUtility((ToolStripDropDownItem) this.cmPageRotate, typeof (ImageRotation), false, (IDictionary<int, Image>) new Dictionary<int, Image>()
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
      }, Keys.D6 | Keys.Shift | Keys.Alt);
      this.rotateMenu.ValueChanged += new EventHandler(this.RotateMenuValueChanged);
    }

    public ItemView ItemView => this.itemView;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemViewConfig ViewConfig
    {
      get => this.itemView.ViewConfig;
      set
      {
        int itemRowHeight = this.itemView.ItemRowHeight;
        this.itemView.ViewConfig = value;
        this.itemView.ItemRowHeight = itemRowHeight;
      }
    }

    [DefaultValue(true)]
    public bool CreateBackdrop
    {
      get => this.createBackdrop;
      set => this.createBackdrop = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicBookNavigator Book
    {
      get => this.book;
      set
      {
        if (this.book == value)
          return;
        if (this.book != null)
        {
          this.book.IndexOfPageReady -= new EventHandler<BookPageEventArgs>(this.Book_PageReady);
          this.book.IndexRetrievalCompleted -= new EventHandler(this.Book_IndexRetrievalCompleted);
        }
        Image backgroundImage = this.itemView.BackgroundImage;
        this.itemView.BackgroundImage = (Image) null;
        backgroundImage?.Dispose();
        this.itemView.Items.Clear();
        this.book = value;
        if (this.book == null)
          return;
        using (ItemMonitor.Lock((object) this.book))
        {
          if (this.book.ProviderStatus != ImageProviderStatus.NotStarted)
          {
            for (int index = 0; index < this.book.Count; ++index)
            {
              ComicPageInfo page = this.book.Comic.GetPage(index);
              this.Book_PageReady((object) this.book, new BookPageEventArgs(this.book.Comic, index, index, page, this.book.GetImageName(page.ImageIndex)));
            }
          }
          this.book.IndexOfPageReady += new EventHandler<BookPageEventArgs>(this.Book_PageReady);
          this.book.IndexRetrievalCompleted += new EventHandler(this.Book_IndexRetrievalCompleted);
        }
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicPageType PageFilter
    {
      get => this.pageFilter;
      set
      {
        if (this.pageFilter == value)
          return;
        this.pageFilter = value;
        this.UpdateList(false);
      }
    }

    private void PageMenuValueChanged(object sender, EventArgs e)
    {
      if (this.pageMenu.Value == -1)
        return;
      foreach (PageViewItem selectedItem in this.itemView.SelectedItems)
        selectedItem.SetPageType((ComicPageType) this.pageMenu.Value);
    }

    private void RotateMenuValueChanged(object sender, EventArgs e)
    {
      if (this.rotateMenu.Value == -1)
        return;
      foreach (PageViewItem selectedItem in this.itemView.SelectedItems)
        selectedItem.SetPageRotation((ImageRotation) this.rotateMenu.Value);
    }

    private void contextPages_Opening(object sender, CancelEventArgs e)
    {
      this.miPageType.Visible = this.tsPageTypeSeparator.Visible = this.book.Comic.EditMode.CanEditPages();
      this.miSetBookmark.Visible = this.miRemoveBookmark.Visible = this.tsBookmarkSeparator.Visible = this.book.Comic.EditMode.CanEditPages();
      this.miMoveToTop.Visible = this.miMoveToBottom.Visible = this.miResetOriginalOrder.Visible = this.tsMovePagesSeparator.Visible = this.book.Comic.EditMode.CanEditPages();
      this.miCopy.Enabled = this.itemView.FocusedItem != null;
      int num1 = -1;
      foreach (ComicPageInfo selectedPage in this.GetSelectedPages())
      {
        if (num1 == -1)
          num1 = (int) selectedPage.PageType;
        else if ((ComicPageType) num1 != selectedPage.PageType)
        {
          num1 = -1;
          break;
        }
      }
      this.pageMenu.Value = num1;
      int num2 = -1;
      foreach (ComicPageInfo selectedPage in this.GetSelectedPages())
      {
        if (num2 == -1)
          num2 = (int) selectedPage.Rotation;
        else if ((ImageRotation) num2 != selectedPage.Rotation)
        {
          num2 = -1;
          break;
        }
      }
      this.rotateMenu.Value = num2;
      int num3 = -1;
      foreach (ComicPageInfo selectedPage in this.GetSelectedPages())
      {
        if (num3 == -1)
          num3 = (int) selectedPage.PagePosition;
        else if ((ComicPagePosition) num3 != selectedPage.PagePosition)
        {
          num3 = -1;
          break;
        }
      }
      this.miPagePositionDefault.Checked = num3 == 0;
      this.miPagePositionNear.Checked = num3 == 1;
      this.miPagePositionFar.Checked = num3 == 2;
    }

    private void ItemViewMouseWheel(object sender, MouseEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Control) == Keys.None || this.itemView.ItemViewMode == ItemViewMode.Detail)
        return;
      this.SetItemSize(this.itemView.ItemThumbSize.Height + e.Delta / SystemInformation.MouseWheelScrollDelta * 16);
    }

    private void Application_Idle(object sender, EventArgs e)
    {
      while (this.listDirty)
      {
        this.listDirty = false;
        this.FillList(this.book, (IEnumerable<ComicPageInfo>) null);
      }
    }

    private void itemView_PostPaint(object sender, PaintEventArgs e)
    {
      Rectangle displayRectangle = this.itemView.DisplayRectangle;
      e.Graphics.DrawShadow(displayRectangle, 8, Color.Black, 0.125f, BlurShadowType.Inside, BlurShadowParts.Edges);
    }

    private void Book_PageReady(object sender, BookPageEventArgs e)
    {
      ComicBookNavigator comicBookNavigator = sender as ComicBookNavigator;
      if (e.PageInfo.IsFrontCover && this.createBackdrop)
        this.CreateBackground();
      ComicPageInfo pageInfo = e.PageInfo;
      if (!pageInfo.IsTypeOf(this.PageFilter))
        return;
      PageViewItem pageViewItem1 = this.itemView.Items.Cast<PageViewItem>().FirstOrDefault<PageViewItem>((Func<PageViewItem, bool>) (iv => iv.ImageIndex == e.PageInfo.ImageIndex));
      if (pageViewItem1 == null)
      {
        ViewableItemCollection<IViewableItem> items = this.itemView.Items;
        ComicBookNavigator book = comicBookNavigator;
        pageInfo = e.PageInfo;
        int imageIndex = pageInfo.ImageIndex;
        string pageKey = e.PageKey;
        PageViewItem pageViewItem2 = new PageViewItem(book, imageIndex, pageKey);
        items.Add((IViewableItem) pageViewItem2);
      }
      else
        pageViewItem1.Key = e.PageKey;
    }

    private void Book_IndexRetrievalCompleted(object sender, EventArgs e)
    {
    }

    private void FillList(ComicBookNavigator nav, IEnumerable<ComicPageInfo> selectedPages)
    {
      this.itemView.BeginUpdate();
      try
      {
        this.itemView.Items.Clear();
        try
        {
          int num = nav.IsIndexRetrievalCompleted ? Math.Max(nav.Count, nav.Comic.PageCount) : nav.Count;
          for (int page = 0; page < num; ++page)
          {
            ComicPageInfo cpi = nav.Comic.GetPage(page);
            if (cpi.IsTypeOf(this.PageFilter))
            {
              PageViewItem pageViewItem = new PageViewItem(nav, cpi.ImageIndex);
              this.itemView.Items.Add((IViewableItem) pageViewItem);
              if (selectedPages != null && selectedPages.FindIndex<ComicPageInfo>((Predicate<ComicPageInfo>) (c => c.ImageIndex == cpi.ImageIndex)) != -1)
                pageViewItem.Selected = true;
            }
          }
        }
        catch
        {
        }
      }
      finally
      {
        this.itemView.EndUpdate();
      }
    }

    public IEnumerable<PageViewItem> GetItems() => this.itemView.Items.OfType<PageViewItem>();

    public IEnumerable<ComicPageInfo> GetSelectedPages()
    {
      return this.itemView.SelectedItems.Cast<PageViewItem>().Select<PageViewItem, ComicPageInfo>((Func<PageViewItem, ComicPageInfo>) (pvi => pvi.PageInfo));
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

    public void CopyPage()
    {
      using (new WaitCursor())
      {
        try
        {
          Clipboard.Clear();
          Clipboard.SetDataObject((object) this.CreateDataObjectFromPages(this.GetSelectedPages()));
        }
        catch (Exception ex)
        {
        }
      }
    }

    public void UpdateList(bool now)
    {
      if (now)
        this.FillList(this.book, (IEnumerable<ComicPageInfo>) null);
      else
        this.listDirty = true;
    }

    public void RefreshDisplay()
    {
      foreach (ThumbnailViewItem thumbnailViewItem in this.itemView.SelectedItems.ToArray<IViewableItem>())
        thumbnailViewItem.RefreshImage();
    }

    public void MarkAsDeleted()
    {
      foreach (PageViewItem pageViewItem in this.itemView.SelectedItems.ToArray<IViewableItem>())
        pageViewItem.SetPageType(pageViewItem.PageInfo.PageType == ComicPageType.Deleted ? ComicPageType.Story : ComicPageType.Deleted);
    }

    public void RotatePage(ImageRotation rotation)
    {
      foreach (PageViewItem pageViewItem in this.itemView.SelectedItems.ToArray<IViewableItem>())
        pageViewItem.SetPageRotation(rotation);
    }

    public void SetPagePosition(ComicPagePosition position)
    {
      foreach (PageViewItem pageViewItem in this.itemView.SelectedItems.ToArray<IViewableItem>())
        pageViewItem.SetPagePosition(position);
    }

    public void MoveSelectedPageStart() => this.MovePages(0, this.GetSelectedPages());

    public void MoveSelectedPageEnd()
    {
      ComicPageInfo[] array = this.GetSelectedPages().ToArray<ComicPageInfo>();
      if (((IEnumerable<ComicPageInfo>) array).All<ComicPageInfo>((Func<ComicPageInfo, bool>) (p => p.PageType == ComicPageType.FrontCover)))
      {
        foreach (ComicPageInfo cpi in array)
          this.book.Comic.UpdatePageType(cpi, ComicPageType.Other);
        if (array.Length < this.book.Comic.Pages.Count)
          this.book.Comic.UpdatePageType(array.Length, ComicPageType.FrontCover);
      }
      this.MovePages(this.book.Comic.PageCount, this.GetSelectedPages());
    }

    public void ResetPageOrder()
    {
      try
      {
        this.book.Comic.ResetPageSequence();
        this.FillList(this.book, this.GetSelectedPages());
      }
      catch
      {
      }
    }

    private void SetBookmark()
    {
      if (!this.CanBookmark)
        return;
      string name = SelectItemDialog.GetName((IWin32Window) this, TR.Default["Bookmark", "Bookmark"], this.BookmarkProposal);
      if (string.IsNullOrEmpty(name))
        return;
      this.Bookmark = name;
    }

    private void MovePages(int position, IEnumerable<ComicPageInfo> pages)
    {
      try
      {
        this.book.Comic.MovePages(position, pages);
        this.FillList(this.book, pages);
      }
      catch
      {
      }
    }

    private void CreateBackground()
    {
      try
      {
        Image backgroundImage1 = this.itemView.BackgroundImage;
        this.itemView.BackgroundImage = (Image) null;
        backgroundImage1?.Dispose();
        ComicBookNavigator newNav = this.book;
        ThreadUtility.RunInBackground("Create pages backdrop", (ThreadStart) (() =>
        {
          try
          {
            if (this.IsDisposed || newNav == null || newNav.IsDisposed)
              return;
            using (IItemLock<ThumbnailImage> thumbnail = Program.ImagePool.GetThumbnail(newNav.Comic.GetFrontCoverThumbnailKey(), (IImageProvider) newNav, true))
            {
              Bitmap bmp = ComicBox3D.CreateDefaultBook(thumbnail.Item.Bitmap, (Bitmap) null, EngineConfiguration.Default.ListCoverSize.ScaleDpi(), newNav.Comic.PageCount);
              bmp.ChangeAlpha(EngineConfiguration.Default.ListCoverAlpha);
              ControlExtensions.BeginInvoke(this.itemView, (Action) (() =>
              {
                Image backgroundImage3 = this.itemView.BackgroundImage;
                this.itemView.BackgroundImage = (Image) bmp;
                backgroundImage3.SafeDispose();
              }));
            }
          }
          catch
          {
          }
        }));
      }
      catch
      {
      }
    }

    private bool IsPageSorted()
    {
      return this.itemView.SortColumn != null && this.itemView.SortColumn.Id == 0 && this.itemView.ItemSortOrder == SortOrder.Ascending && this.itemView.GroupColumn == null;
    }

    private void itemView_ItemDrag(object sender, ItemDragEventArgs e)
    {
      if (this.itemView.SelectedItems.IsEmpty<IViewableItem>())
        return;
      this.dragCursor = this.itemView.GetDragCursor(Program.ExtendedSettings.DragDropCursorAlpha);
      bool flag = this.IsPageSorted();
      try
      {
        this.dragPages = this.GetSelectedPages().ToArray<ComicPageInfo>();
        this.itemView.AllowDrop = true;
        int num = (int) this.itemView.DoDragDrop((object) this.CreateDataObjectFromPages((IEnumerable<ComicPageInfo>) this.dragPages), flag ? DragDropEffects.Copy | DragDropEffects.Move : DragDropEffects.Copy);
      }
      finally
      {
        this.itemView.AllowDrop = false;
        if (this.dragCursor != null)
          this.dragCursor.Dispose();
        this.dragCursor = (IBitmapCursor) null;
        this.dragPages = (ComicPageInfo[]) null;
      }
    }

    private DataObjectEx CreateDataObjectFromPages(IEnumerable<ComicPageInfo> dragPages)
    {
      DataObjectEx dataObjectFromPages = new DataObjectEx();
      ComicBook comic = this.Book.Comic;
      foreach (ComicPageInfo dragPage in dragPages)
      {
        int page1 = comic.TranslateImageIndexToPage(dragPage.ImageIndex);
        string fileName = FileUtility.MakeValidFilename(StringUtility.Format("{0} - {1} {2}.jpg", (object) comic.Caption, (object) TR.Default["Page", "Page"], (object) (page1 + 1)));
        PageKey key = this.Book.GetPageKey(page1);
        dataObjectFromPages.SetFile(fileName, (Action<Stream>) (s =>
        {
          try
          {
            using (IItemLock<PageImage> page2 = Program.ImagePool.GetPage(key, comic))
              page2.Item.Bitmap.Save(s, ImageFormat.Jpeg);
          }
          catch
          {
          }
        }));
      }
      using (IItemLock<PageImage> page = Program.ImagePool.GetPage(this.book.GetPageKey(comic.TranslateImageIndexToPage(dragPages.First<ComicPageInfo>().ImageIndex)), (IImageProvider) this.Book))
      {
        if (page != null)
        {
          if (page.Item != null)
          {
            if (page.Item.Bitmap != null)
              dataObjectFromPages.SetImage((Image) page.Item.Bitmap);
          }
        }
      }
      return dataObjectFromPages;
    }

    private void itemView_DragDrop(object sender, DragEventArgs e)
    {
      if (this.itemView.MarkerItem == null)
        return;
      this.MovePages(((PageViewItem) this.itemView.MarkerItem).Page, (IEnumerable<ComicPageInfo>) this.dragPages);
    }

    private void itemView_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = this.dragPages == null || !this.IsPageSorted() ? DragDropEffects.None : DragDropEffects.Move;
    }

    private void itemView_DragLeave(object sender, EventArgs e)
    {
      this.itemView.MarkerVisible = false;
    }

    private void itemView_DragOver(object sender, DragEventArgs e)
    {
      this.itemView.MarkerItem = (IViewableItem) (this.itemView.ItemHitTest(this.itemView.PointToClient(new System.Drawing.Point(e.X, e.Y))) as PageViewItem);
      this.itemView.MarkerVisible = e.Effect == DragDropEffects.Move;
    }

    private void itemView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if (this.dragCursor == null || this.dragCursor.Cursor == (Cursor) null)
        return;
      e.UseDefaultCursors = false;
      this.dragCursor.OverlayCursor = e.Effect == DragDropEffects.None ? Cursors.No : Cursors.Default;
      this.dragCursor.OverlayEffect = e.Effect == DragDropEffects.Copy ? BitmapCursorOverlayEffect.Plus : BitmapCursorOverlayEffect.None;
      Cursor.Current = this.dragCursor.Cursor;
    }

    public bool CanBookmark
    {
      get
      {
        return this.Book != null && this.Book.Comic != null && this.Book.Comic.EditMode.CanEditPages() && this.itemView.SelectedCount == 1;
      }
    }

    public string BookmarkProposal
    {
      get
      {
        if (!this.CanBookmark)
          return (string) null;
        return !string.IsNullOrEmpty(this.Bookmark) ? this.Bookmark : string.Format("{0} {1}", (object) TR.Default["Page", "Page"], (object) this.itemView.SelectedItems.First<IViewableItem>().Text);
      }
    }

    public string Bookmark
    {
      get
      {
        return !this.CanBookmark ? (string) null : this.GetSelectedPages().First<ComicPageInfo>().Bookmark;
      }
      set
      {
        if (!this.CanBookmark)
          return;
        this.Book.Comic.UpdateBookmark(this.Book.Comic.TranslateImageIndexToPage(this.GetSelectedPages().First<ComicPageInfo>().ImageIndex), value ?? string.Empty);
      }
    }

    private bool HasValidPages
    {
      get
      {
        return this.Book != null && this.Book.Comic != null && this.Book.Comic.EditMode.CanEditPages() && this.itemView.SelectedCount > 0;
      }
    }

    bool IEditPage.IsValid => this.HasValidPages;

    ComicPageType IEditPage.PageType
    {
      get
      {
        return !this.HasValidPages ? ComicPageType.Other : this.itemView.SelectedItems.OfType<PageViewItem>().Select<PageViewItem, ComicPageInfo>((Func<PageViewItem, ComicPageInfo>) (pvi => pvi.PageInfo)).FirstOrDefault<ComicPageInfo>().PageType;
      }
      set
      {
        if (!this.HasValidPages)
          return;
        this.GetSelectedPages().ForEach<ComicPageInfo>((Action<ComicPageInfo>) (pi => this.Book.Comic.UpdatePageType(pi, value)));
      }
    }

    ImageRotation IEditPage.Rotation
    {
      get
      {
        return !this.HasValidPages ? ImageRotation.None : this.itemView.SelectedItems.OfType<PageViewItem>().Select<PageViewItem, ComicPageInfo>((Func<PageViewItem, ComicPageInfo>) (pvi => pvi.PageInfo)).FirstOrDefault<ComicPageInfo>().Rotation;
      }
      set
      {
        if (!this.HasValidPages)
          return;
        this.GetSelectedPages().ForEach<ComicPageInfo>((Action<ComicPageInfo>) (pi => this.Book.Comic.UpdatePageRotation(pi, value)));
      }
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.itemView = new ItemView();
      this.contextPages = new ContextMenuStrip(this.components);
      this.miPageType = new ToolStripMenuItem();
      this.cmPageRotate = new ToolStripMenuItem();
      this.miPagePosition = new ToolStripMenuItem();
      this.miPagePositionDefault = new ToolStripMenuItem();
      this.toolStripMenuItem2 = new ToolStripSeparator();
      this.miPagePositionNear = new ToolStripMenuItem();
      this.miPagePositionFar = new ToolStripMenuItem();
      this.tsPageTypeSeparator = new ToolStripSeparator();
      this.miSetBookmark = new ToolStripMenuItem();
      this.miRemoveBookmark = new ToolStripMenuItem();
      this.tsBookmarkSeparator = new ToolStripSeparator();
      this.miMoveToTop = new ToolStripMenuItem();
      this.miMoveToBottom = new ToolStripMenuItem();
      this.miResetOriginalOrder = new ToolStripMenuItem();
      this.tsMovePagesSeparator = new ToolStripSeparator();
      this.miCopy = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.miSelectAll = new ToolStripMenuItem();
      this.miInvertSelection = new ToolStripMenuItem();
      this.miRefreshThumbnail = new ToolStripMenuItem();
      this.toolStripMenuItem3 = new ToolStripSeparator();
      this.miMarkDeleted = new ToolStripMenuItem();
      this.contextPages.SuspendLayout();
      this.SuspendLayout();
      this.itemView.AllowDrop = true;
      this.itemView.BackColor = SystemColors.Window;
      this.itemView.BackgroundImageAlignment = ContentAlignment.BottomRight;
      this.itemView.Dock = DockStyle.Fill;
      this.itemView.HideSelection = false;
      this.itemView.ItemContextMenuStrip = this.contextPages;
      this.itemView.Location = new System.Drawing.Point(0, 0);
      this.itemView.Name = "itemView";
      this.itemView.Size = new System.Drawing.Size(413, 406);
      this.itemView.SortColumn = (IColumn) null;
      this.itemView.SortColumns = new IColumn[0];
      this.itemView.SortColumnsKey = "";
      this.itemView.TabIndex = 1;
      this.itemView.ItemDrag += new ItemDragEventHandler(this.itemView_ItemDrag);
      this.itemView.PostPaint += new PaintEventHandler(this.itemView_PostPaint);
      this.itemView.DragDrop += new DragEventHandler(this.itemView_DragDrop);
      this.itemView.DragEnter += new DragEventHandler(this.itemView_DragEnter);
      this.itemView.DragOver += new DragEventHandler(this.itemView_DragOver);
      this.itemView.DragLeave += new EventHandler(this.itemView_DragLeave);
      this.itemView.GiveFeedback += new GiveFeedbackEventHandler(this.itemView_GiveFeedback);
      this.contextPages.Items.AddRange(new ToolStripItem[18]
      {
        (ToolStripItem) this.miPageType,
        (ToolStripItem) this.cmPageRotate,
        (ToolStripItem) this.miPagePosition,
        (ToolStripItem) this.tsPageTypeSeparator,
        (ToolStripItem) this.miSetBookmark,
        (ToolStripItem) this.miRemoveBookmark,
        (ToolStripItem) this.tsBookmarkSeparator,
        (ToolStripItem) this.miMoveToTop,
        (ToolStripItem) this.miMoveToBottom,
        (ToolStripItem) this.miResetOriginalOrder,
        (ToolStripItem) this.tsMovePagesSeparator,
        (ToolStripItem) this.miCopy,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.miSelectAll,
        (ToolStripItem) this.miInvertSelection,
        (ToolStripItem) this.miRefreshThumbnail,
        (ToolStripItem) this.toolStripMenuItem3,
        (ToolStripItem) this.miMarkDeleted
      });
      this.contextPages.Name = "cmPages";
      this.contextPages.Size = new System.Drawing.Size(249, 342);
      this.contextPages.Opening += new CancelEventHandler(this.contextPages_Opening);
      this.miPageType.Name = "miPageType";
      this.miPageType.Size = new System.Drawing.Size(248, 22);
      this.miPageType.Text = "Page Type";
      this.cmPageRotate.Name = "cmPageRotate";
      this.cmPageRotate.Size = new System.Drawing.Size(248, 22);
      this.cmPageRotate.Text = "Page Rotation";
      this.miPagePosition.DropDownItems.AddRange(new ToolStripItem[4]
      {
        (ToolStripItem) this.miPagePositionDefault,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.miPagePositionNear,
        (ToolStripItem) this.miPagePositionFar
      });
      this.miPagePosition.Name = "miPagePosition";
      this.miPagePosition.Size = new System.Drawing.Size(248, 22);
      this.miPagePosition.Text = "Page Position";
      this.miPagePositionDefault.Name = "miPagePositionDefault";
      this.miPagePositionDefault.Size = new System.Drawing.Size(112, 22);
      this.miPagePositionDefault.Text = "Default";
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(109, 6);
      this.miPagePositionNear.Name = "miPagePositionNear";
      this.miPagePositionNear.Size = new System.Drawing.Size(112, 22);
      this.miPagePositionNear.Text = "Near";
      this.miPagePositionFar.Name = "miPagePositionFar";
      this.miPagePositionFar.Size = new System.Drawing.Size(112, 22);
      this.miPagePositionFar.Text = "Far";
      this.tsPageTypeSeparator.Name = "tsPageTypeSeparator";
      this.tsPageTypeSeparator.Size = new System.Drawing.Size(245, 6);
      this.miSetBookmark.Name = "miSetBookmark";
      this.miSetBookmark.ShortcutKeys = Keys.B | Keys.Shift | Keys.Control;
      this.miSetBookmark.Size = new System.Drawing.Size(248, 22);
      this.miSetBookmark.Text = "Set Bookmark...";
      this.miRemoveBookmark.Name = "miRemoveBookmark";
      this.miRemoveBookmark.ShortcutKeys = Keys.D | Keys.Shift | Keys.Control;
      this.miRemoveBookmark.Size = new System.Drawing.Size(248, 22);
      this.miRemoveBookmark.Text = "Remove Bookmark";
      this.tsBookmarkSeparator.Name = "tsBookmarkSeparator";
      this.tsBookmarkSeparator.Size = new System.Drawing.Size(245, 6);
      this.miMoveToTop.Name = "miMoveToTop";
      this.miMoveToTop.ShortcutKeys = Keys.T | Keys.Control | Keys.Alt;
      this.miMoveToTop.Size = new System.Drawing.Size(248, 22);
      this.miMoveToTop.Text = "&Move to Top";
      this.miMoveToBottom.Name = "miMoveToBottom";
      this.miMoveToBottom.ShortcutKeys = Keys.B | Keys.Control | Keys.Alt;
      this.miMoveToBottom.Size = new System.Drawing.Size(248, 22);
      this.miMoveToBottom.Text = "Move to Bottom";
      this.miResetOriginalOrder.Name = "miResetOriginalOrder";
      this.miResetOriginalOrder.Size = new System.Drawing.Size(248, 22);
      this.miResetOriginalOrder.Text = "Reset original Order";
      this.tsMovePagesSeparator.Name = "tsMovePagesSeparator";
      this.tsMovePagesSeparator.Size = new System.Drawing.Size(245, 6);
      this.miCopy.Image = (Image) Resources.Copy;
      this.miCopy.Name = "miCopy";
      this.miCopy.ShortcutKeys = Keys.C | Keys.Control;
      this.miCopy.Size = new System.Drawing.Size(248, 22);
      this.miCopy.Text = "&Copy Page";
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(245, 6);
      this.miSelectAll.Name = "miSelectAll";
      this.miSelectAll.ShortcutKeys = Keys.A | Keys.Control;
      this.miSelectAll.Size = new System.Drawing.Size(248, 22);
      this.miSelectAll.Text = "Select &All";
      this.miInvertSelection.Name = "miInvertSelection";
      this.miInvertSelection.Size = new System.Drawing.Size(248, 22);
      this.miInvertSelection.Text = "&Invert Selection";
      this.miRefreshThumbnail.Image = (Image) Resources.RefreshThumbnail;
      this.miRefreshThumbnail.Name = "miRefreshThumbnail";
      this.miRefreshThumbnail.Size = new System.Drawing.Size(248, 22);
      this.miRefreshThumbnail.Text = "&Refresh";
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(245, 6);
      this.miMarkDeleted.Image = (Image) Resources.EditDelete;
      this.miMarkDeleted.Name = "miMarkDeleted";
      this.miMarkDeleted.ShortcutKeys = Keys.Delete;
      this.miMarkDeleted.Size = new System.Drawing.Size(248, 22);
      this.miMarkDeleted.Text = "Mark as &Deleted";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.itemView);
      this.Name = nameof (PagesView);
      this.Size = new System.Drawing.Size(413, 406);
      this.contextPages.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

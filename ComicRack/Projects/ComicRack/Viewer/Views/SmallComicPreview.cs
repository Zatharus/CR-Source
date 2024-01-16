// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.SmallComicPreview
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Engine.Display.Forms;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class SmallComicPreview : CaptionControl, IRefreshDisplay
  {
    private readonly CommandMapper commands = new CommandMapper();
    private readonly string noneSelectedText;
    private readonly string previewOnlyForComics;
    private ComicDisplay comicDisplay;
    private IContainer components;
    private ComicDisplayControl pageViewer;
    private ToolStrip toolStripPreview;
    private ToolStripButton tsbFirst;
    private ToolStripButton tsbPrev;
    private ToolStripButton tsbNext;
    private ToolStripButton tsbLast;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripButton tsbTwoPages;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton tsbRefresh;
    private ToolStripButton tbClose;
    private ToolStripButton tsbOpen;
    private ToolStripSeparator toolStripSeparator3;

    public SmallComicPreview()
    {
      this.InitializeComponent();
      if (this.components == null)
        this.components = (IContainer) new System.ComponentModel.Container();
      this.components.Add((IComponent) this.commands);
      LocalizeUtility.Localize((Control) this, this.components);
      this.noneSelectedText = TR.Load(this.Name)[this.pageViewer.Name, this.pageViewer.Text];
      this.previewOnlyForComics = TR.Load(this.Name)["PreviewOnlyForComics", "Preview is only available for Books"];
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (!this.DesignMode)
          Program.Settings.PageImageDisplayOptionsChanged -= new EventHandler(this.Settings_DisplayOptionsChanged);
        if (this.comicDisplay != null)
          this.comicDisplay.Dispose();
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
      this.comicDisplay = new ComicDisplay((IComicDisplay) this.pageViewer);
      this.comicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveUp", "", "Up", new Action(this.comicDisplay.ScrollUp), new CommandKey[2]
      {
        CommandKey.Up,
        CommandKey.MouseWheelUp
      }));
      this.comicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("MoveDown", "", "Down", new Action(this.comicDisplay.ScrollDown), new CommandKey[2]
      {
        CommandKey.Down,
        CommandKey.MouseWheelDown
      }));
      this.comicDisplay.KeyboardMap.Commands.Add(new KeyboardCommand("OpenBook", "", "Open", new Action(this.OpenBook), new CommandKey[1]
      {
        CommandKey.MouseDoubleLeft
      }));
      this.commands.Add(new CommandHandler(this.comicDisplay.DisplayFirstPage), (UpdateHandler) (() => this.comicDisplay.Book != null && this.comicDisplay.Book.CanNavigate(-1)), (object) this.tsbFirst);
      this.commands.Add((CommandHandler) (() => this.comicDisplay.DisplayPreviousPage(ComicDisplay.PagingMode.Double)), (UpdateHandler) (() => this.comicDisplay.Book != null && this.comicDisplay.Book.CanNavigate(-1)), (object) this.tsbPrev);
      this.commands.Add((CommandHandler) (() => this.comicDisplay.DisplayNextPage(ComicDisplay.PagingMode.Double)), (UpdateHandler) (() => this.comicDisplay.Book != null && this.comicDisplay.Book.CanNavigate(1)), (object) this.tsbNext);
      this.commands.Add(new CommandHandler(this.comicDisplay.DisplayLastPage), (UpdateHandler) (() => this.comicDisplay.Book != null && this.comicDisplay.Book.CanNavigate(1)), (object) this.tsbLast);
      this.commands.Add((CommandHandler) (() => this.TwoPageDisplay = !this.TwoPageDisplay), (UpdateHandler) (() => this.comicDisplay.Book != null), (UpdateHandler) (() => this.TwoPageDisplay), (object) this.tsbTwoPages);
      this.commands.Add(new CommandHandler(this.RefreshDisplay), (UpdateHandler) (() => this.comicDisplay.Book != null), (object) this.tsbRefresh);
      this.commands.Add(new CommandHandler(this.OnCloseClicked), (object) this.tbClose);
      this.commands.Add(new CommandHandler(this.OpenBook), (UpdateHandler) (() => this.comicDisplay.Book != null), (object) this.tsbOpen);
      this.pageViewer.ContextMenuStrip = (ContextMenuStrip) null;
      this.pageViewer.PagePool = (IPagePool) Program.ImagePool;
      Program.Settings.PageImageDisplayOptionsChanged += new EventHandler(this.Settings_DisplayOptionsChanged);
      this.UpdateSettings();
    }

    private void Settings_DisplayOptionsChanged(object sender, EventArgs e)
    {
      this.UpdateSettings();
    }

    private void UpdateSettings()
    {
      this.pageViewer.ImageBackgroundMode = ImageBackgroundMode.Color;
    }

    private void OpenBook()
    {
      IMain parentService = this.FindParentService<IMain>();
      if (parentService == null || this.pageViewer.Book == null || this.pageViewer.Book.Comic == null)
        return;
      parentService.OpenBooks.Open(this.pageViewer.Book.Comic.FilePath, OpenComicOptions.OpenInNewSlot);
    }

    public event EventHandler CloseClicked;

    protected virtual void OnCloseClicked()
    {
      if (this.CloseClicked == null)
        return;
      this.CloseClicked((object) this, EventArgs.Empty);
    }

    public bool TwoPageDisplay
    {
      get => this.pageViewer.PageLayout != 0;
      set
      {
        this.pageViewer.PageLayout = value ? PageLayoutMode.DoubleAdaptive : PageLayoutMode.Single;
      }
    }

    public void ShowPreview(ComicBook comicBook)
    {
      if (comicBook == null)
      {
        if (this.pageViewer.Book != null)
          this.pageViewer.Book.Dispose();
        this.pageViewer.Book = (ComicBookNavigator) null;
      }
      else if (this.pageViewer.Book == null || comicBook.FilePath != this.pageViewer.Book.Comic.FilePath)
      {
        if (this.pageViewer.Book != null)
        {
          this.pageViewer.Book.Dispose();
          this.pageViewer.Book = (ComicBookNavigator) null;
        }
        this.pageViewer.Book = NavigatorManager.OpenComic(comicBook, 0, OpenComicOptions.DisableAll);
      }
      if (this.pageViewer.Book != null)
        this.pageViewer.Text = string.Empty;
      else
        this.pageViewer.Text = comicBook == null || comicBook.IsLinked ? this.noneSelectedText : this.previewOnlyForComics;
    }

    public void RefreshDisplay() => this.comicDisplay.RefreshDisplay();

    private void InitializeComponent()
    {
      this.pageViewer = new ComicDisplayControl();
      this.toolStripPreview = new ToolStrip();
      this.tsbOpen = new ToolStripButton();
      this.toolStripSeparator3 = new ToolStripSeparator();
      this.tsbFirst = new ToolStripButton();
      this.tsbPrev = new ToolStripButton();
      this.tsbNext = new ToolStripButton();
      this.tsbLast = new ToolStripButton();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.tsbTwoPages = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.tsbRefresh = new ToolStripButton();
      this.tbClose = new ToolStripButton();
      this.toolStripPreview.SuspendLayout();
      this.SuspendLayout();
      this.pageViewer.BackColor = SystemColors.Window;
      this.pageViewer.DisableHardwareAcceleration = true;
      this.pageViewer.Dock = DockStyle.Fill;
      this.pageViewer.Font = new Font("Arial", 15.75f, FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.pageViewer.ForeColor = Color.LightGray;
      this.pageViewer.HardwareFiltering = false;
      this.pageViewer.Location = new System.Drawing.Point(0, 25);
      this.pageViewer.MagnifierSize = new System.Drawing.Size(400, 300);
      this.pageViewer.Name = "pageViewer";
      this.pageViewer.PaperTextureLayout = ImageLayout.None;
      this.pageViewer.PreCache = false;
      this.pageViewer.Size = new System.Drawing.Size(285, 267);
      this.pageViewer.TabIndex = 2;
      this.pageViewer.Text = "Nothing Selected";
      this.toolStripPreview.GripStyle = ToolStripGripStyle.Hidden;
      this.toolStripPreview.Items.AddRange(new ToolStripItem[11]
      {
        (ToolStripItem) this.tsbOpen,
        (ToolStripItem) this.toolStripSeparator3,
        (ToolStripItem) this.tsbFirst,
        (ToolStripItem) this.tsbPrev,
        (ToolStripItem) this.tsbNext,
        (ToolStripItem) this.tsbLast,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.tsbTwoPages,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.tsbRefresh,
        (ToolStripItem) this.tbClose
      });
      this.toolStripPreview.Location = new System.Drawing.Point(0, 0);
      this.toolStripPreview.Name = "toolStripPreview";
      this.toolStripPreview.Size = new System.Drawing.Size(285, 25);
      this.toolStripPreview.TabIndex = 3;
      this.toolStripPreview.Text = "toolStrip1";
      this.tsbOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsbOpen.Image = (Image) Resources.Open;
      this.tsbOpen.ImageTransparentColor = Color.Magenta;
      this.tsbOpen.Name = "tsbOpen";
      this.tsbOpen.Size = new System.Drawing.Size(23, 22);
      this.tsbOpen.Text = "Open";
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
      this.tsbFirst.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsbFirst.Image = (Image) Resources.GoFirst;
      this.tsbFirst.ImageTransparentColor = Color.Magenta;
      this.tsbFirst.Name = "tsbFirst";
      this.tsbFirst.Size = new System.Drawing.Size(23, 22);
      this.tsbFirst.Text = "First";
      this.tsbFirst.ToolTipText = "Go to first page";
      this.tsbPrev.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsbPrev.Image = (Image) Resources.GoPrevious;
      this.tsbPrev.ImageTransparentColor = Color.Magenta;
      this.tsbPrev.Name = "tsbPrev";
      this.tsbPrev.Size = new System.Drawing.Size(23, 22);
      this.tsbPrev.Text = "Previous";
      this.tsbPrev.ToolTipText = "Go to previous page";
      this.tsbNext.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsbNext.Image = (Image) Resources.GoNext;
      this.tsbNext.ImageTransparentColor = Color.Magenta;
      this.tsbNext.Name = "tsbNext";
      this.tsbNext.Size = new System.Drawing.Size(23, 22);
      this.tsbNext.Text = "Next";
      this.tsbNext.ToolTipText = "Go to next page";
      this.tsbLast.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsbLast.Image = (Image) Resources.GoLast;
      this.tsbLast.ImageTransparentColor = Color.Magenta;
      this.tsbLast.Name = "tsbLast";
      this.tsbLast.Size = new System.Drawing.Size(23, 22);
      this.tsbLast.Text = "Last";
      this.tsbLast.ToolTipText = "Go to last page";
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
      this.tsbTwoPages.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsbTwoPages.Image = (Image) Resources.TwoPage;
      this.tsbTwoPages.ImageTransparentColor = Color.Magenta;
      this.tsbTwoPages.Name = "tsbTwoPages";
      this.tsbTwoPages.Size = new System.Drawing.Size(23, 22);
      this.tsbTwoPages.Text = "Two Pages";
      this.tsbTwoPages.ToolTipText = "Show one or two pages";
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      this.tsbRefresh.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tsbRefresh.Image = (Image) Resources.Refresh;
      this.tsbRefresh.ImageTransparentColor = Color.Magenta;
      this.tsbRefresh.Name = "tsbRefresh";
      this.tsbRefresh.Size = new System.Drawing.Size(23, 22);
      this.tsbRefresh.Text = "Refresh";
      this.tbClose.Alignment = ToolStripItemAlignment.Right;
      this.tbClose.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.tbClose.Image = (Image) Resources.SmallClose;
      this.tbClose.ImageTransparentColor = Color.Magenta;
      this.tbClose.Name = "tbClose";
      this.tbClose.Overflow = ToolStripItemOverflow.Never;
      this.tbClose.Size = new System.Drawing.Size(23, 22);
      this.tbClose.Text = "Close";
      this.AutoScaleMode = AutoScaleMode.None;
      this.Controls.Add((Control) this.pageViewer);
      this.Controls.Add((Control) this.toolStripPreview);
      this.Name = nameof (SmallComicPreview);
      this.Size = new System.Drawing.Size(285, 292);
      this.toolStripPreview.ResumeLayout(false);
      this.toolStripPreview.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

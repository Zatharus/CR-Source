// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicExplorerView
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicExplorerView : SubView, ISidebar
  {
    private ComicListBrowser comicListBrowser;
    private ComicBook[] comicInfoBooks = new ComicBook[0];
    private System.Drawing.Size infoBrowserSize;
    private IContainer components;
    private ComicBrowserControl comicBrowser;
    private Timer previewTimer;
    private SmallComicPreview smallComicPreview;
    private SizableContainer sidePanel;
    private Panel treePanel;
    private SizableContainer previewPane;
    private SizableContainer pluginContainer;
    private ComicPageContainerControl comicInfo;
    private Panel pluginPlaceholder;

    public ComicExplorerView()
    {
      this.InitializeComponent();
      this.pluginContainer.Expanded = false;
      ((ISidebar) this).Preview = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.ComicBrowser.ItemView.SelectedIndexChanged += new EventHandler(this.ItemView_SelectedIndexChanged);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Horizontal
    {
      get => this.sidePanel.Dock == DockStyle.Left;
      set
      {
        this.sidePanel.Dock = value ? DockStyle.Left : DockStyle.Top;
        this.previewPane.Dock = value ? DockStyle.Bottom : DockStyle.Right;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicListBrowser ComicListBrowser
    {
      get => this.comicListBrowser;
      set
      {
        if (this.comicListBrowser == value)
          return;
        if (this.comicListBrowser != null)
          this.comicListBrowser.BookListChanged -= new EventHandler(this.browserControl_BookListChanged);
        this.comicBrowser.BookList = (IComicBookListProvider) null;
        this.treePanel.Controls.Remove((Control) this.comicListBrowser);
        this.comicListBrowser = value;
        if (this.comicListBrowser == null)
          return;
        this.treePanel.Controls.Add((Control) this.comicListBrowser);
        this.comicListBrowser.Dock = DockStyle.Fill;
        this.comicListBrowser.BookListChanged += new EventHandler(this.browserControl_BookListChanged);
        this.comicBrowser.BookList = this.comicListBrowser.BookList;
      }
    }

    public ComicBrowserControl ComicBrowser => this.comicBrowser;

    public int SplitterDistance
    {
      get => this.sidePanel.ExpandedWidth;
      set => this.sidePanel.ExpandedWidth = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicExplorerViewSettings ViewSettings
    {
      get
      {
        ComicExplorerViewSettings viewSettings = new ComicExplorerViewSettings();
        ISidebar sidebar = (ISidebar) this;
        try
        {
          viewSettings.BrowserSplit = this.SplitterDistance;
          viewSettings.ShowBrowser = this.sidePanel.Expanded;
          viewSettings.PreviewSplit = this.previewPane.ExpandedWidth;
          viewSettings.ShowSearchBrowser = this.comicBrowser.SearchBrowserVisible;
          viewSettings.ShowPreview = sidebar.Preview;
          viewSettings.ShowTopBrowser = sidebar.TopBrowser;
          viewSettings.TopBrowserSplit = sidebar.TopBrowserSplit;
          viewSettings.ItemViewConfig = this.comicBrowser.ViewConfig;
          if (viewSettings.ItemViewConfig != null)
            viewSettings.ItemViewConfig.GroupsStatus = (ItemViewGroupsStatus) null;
          viewSettings.TwoPagePreview = this.smallComicPreview.TwoPageDisplay;
          viewSettings.SearchBrowserColumn1 = this.comicBrowser.SearchBrowserColumn1;
          viewSettings.SearchBrowserColumn2 = this.comicBrowser.SearchBrowserColumn2;
          viewSettings.SearchBrowserColumn3 = this.comicBrowser.SearchBrowserColumn3;
          viewSettings.ShowInfo = sidebar.Info;
          viewSettings.InfoBrowserSize = sidebar.InfoBrowserSize;
          viewSettings.InfoBrowserRight = sidebar.InfoBrowserRight;
        }
        catch (Exception ex)
        {
        }
        return viewSettings;
      }
      set
      {
        ComicExplorerViewSettings explorerViewSettings = value;
        if (explorerViewSettings == null)
          return;
        bool enableAnimation = SizableContainer.EnableAnimation;
        SizableContainer.EnableAnimation = false;
        try
        {
          ISidebar sidebar = (ISidebar) this;
          sidebar.Preview = explorerViewSettings.ShowPreview;
          sidebar.TopBrowser = explorerViewSettings.ShowTopBrowser;
          sidebar.TopBrowserSplit = explorerViewSettings.TopBrowserSplit;
          sidebar.Info = explorerViewSettings.ShowInfo;
          sidebar.InfoBrowserRight = explorerViewSettings.InfoBrowserRight;
          sidebar.InfoBrowserSize = explorerViewSettings.InfoBrowserSize;
          this.SplitterDistance = explorerViewSettings.BrowserSplit;
          this.sidePanel.Expanded = explorerViewSettings.ShowBrowser;
          this.previewPane.ExpandedWidth = explorerViewSettings.PreviewSplit;
          if (explorerViewSettings.ItemViewConfig != null)
            this.comicBrowser.ViewConfig = explorerViewSettings.ItemViewConfig;
          this.smallComicPreview.TwoPageDisplay = explorerViewSettings.TwoPagePreview;
          this.comicBrowser.SearchBrowserColumn1 = explorerViewSettings.SearchBrowserColumn1;
          this.comicBrowser.SearchBrowserColumn2 = explorerViewSettings.SearchBrowserColumn2;
          this.comicBrowser.SearchBrowserColumn3 = explorerViewSettings.SearchBrowserColumn3;
          this.comicBrowser.SearchBrowserVisible = explorerViewSettings.ShowSearchBrowser;
        }
        catch (Exception ex)
        {
        }
        finally
        {
          SizableContainer.EnableAnimation = enableAnimation;
        }
      }
    }

    private void UpdatePreviewPadding()
    {
      this.previewPane.Padding = new Padding(0, 0, 0, !this.pluginContainer.IsVisibleSet() || this.pluginContainer.Dock != DockStyle.Bottom ? 0 : 6);
      this.pluginContainer.Padding = new Padding(0, 0, 0, this.pluginContainer.Dock == DockStyle.Bottom ? 6 : 0);
      this.pluginPlaceholder.Visible = !this.pluginContainer.IsVisibleSet() || this.pluginContainer.Dock != DockStyle.Bottom;
    }

    private void browserControl_BookListChanged(object sender, EventArgs e)
    {
      this.comicBrowser.BookList = this.comicListBrowser.BookList;
    }

    private void ItemView_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.RestartPreviewTimer();
    }

    private void RestartPreviewTimer()
    {
      if (!this.IsHandleCreated || this.BeginInvokeIfRequired(new Action(this.RestartPreviewTimer)))
        return;
      this.previewTimer.Stop();
      this.previewTimer.Start();
    }

    private void previewTimer_Tick(object sender, EventArgs e)
    {
      this.previewTimer.Stop();
      this.UpdatePreview();
    }

    private void sidePanel_ExpandedChanged(object sender, EventArgs e) => this.UpdatePreview();

    private void pluginContainer_ExpandedChanged(object sender, EventArgs e)
    {
      this.UpdatePreview();
    }

    private void UpdatePreview()
    {
      ISidebar sidebar = (ISidebar) this;
      bool flag = sidebar.Preview && sidebar.Visible;
      bool info = sidebar.Info;
      ((IEnumerable<ComicBook>) this.comicInfoBooks).ForEach<ComicBook>((Action<ComicBook>) (cb => cb.BookChanged -= new EventHandler<BookChangedEventArgs>(this.previewBookChanged)));
      this.comicInfoBooks = new ComicBook[0];
      if (!flag && !info)
        return;
      IEnumerable<ComicBook> bookList = this.ComicBrowser.GetBookList(ComicBookFilterType.Selected);
      if (flag)
      {
        if (bookList.IsEmpty<ComicBook>())
          this.smallComicPreview.ShowPreview((ComicBook) null);
        else
          this.smallComicPreview.ShowPreview(new ComicBook(bookList.First<ComicBook>()));
      }
      if (!info)
        return;
      this.comicInfoBooks = bookList.ToArray<ComicBook>();
      ((IEnumerable<ComicBook>) this.comicInfoBooks).ForEach<ComicBook>((Action<ComicBook>) (cb => cb.BookChanged += new EventHandler<BookChangedEventArgs>(this.previewBookChanged)));
      this.comicInfo.ShowInfo(bookList);
    }

    private void previewBookChanged(object sender, BookChangedEventArgs e)
    {
      this.RestartPreviewTimer();
    }

    private void smallComicPreview_CloseClicked(object sender, EventArgs e)
    {
      ((ISidebar) this).Preview = false;
    }

    bool ISidebar.Visible
    {
      get => this.sidePanel.Expanded;
      set => this.sidePanel.Expanded = value;
    }

    bool ISidebar.Preview
    {
      get => this.previewPane.Expanded;
      set => this.previewPane.Expanded = value;
    }

    bool ISidebar.TopBrowser
    {
      get => this.comicListBrowser.TopBrowserVisible;
      set => this.comicListBrowser.TopBrowserVisible = value;
    }

    int ISidebar.TopBrowserSplit
    {
      get => this.comicListBrowser.TopBrowserSplit;
      set => this.comicListBrowser.TopBrowserSplit = value;
    }

    bool ISidebar.Info
    {
      get => this.pluginContainer.Expanded;
      set => this.pluginContainer.Expanded = value;
    }

    bool ISidebar.HasInfoPanels => this.comicInfo.Pages.Count<ComicPageControl>() > 0;

    bool ISidebar.InfoBrowserRight
    {
      get => this.pluginContainer.Dock == DockStyle.Right;
      set
      {
        DockStyle dockStyle = value ? DockStyle.Right : DockStyle.Bottom;
        if (this.pluginContainer.Dock == dockStyle)
          return;
        this.pluginContainer.Dock = dockStyle;
        this.UpdatePreviewPadding();
        if (this.infoBrowserSize.IsEmpty)
          return;
        ((ISidebar) this).InfoBrowserSize = this.infoBrowserSize;
      }
    }

    System.Drawing.Size ISidebar.InfoBrowserSize
    {
      get
      {
        return this.pluginContainer.Dock == DockStyle.Right ? new System.Drawing.Size(this.pluginContainer.ExpandedWidth, this.infoBrowserSize.Height) : new System.Drawing.Size(this.infoBrowserSize.Width, this.pluginContainer.ExpandedWidth);
      }
      set
      {
        this.infoBrowserSize = value;
        if (this.pluginContainer.Dock == DockStyle.Right)
          this.pluginContainer.ExpandedWidth = value.Width;
        else
          this.pluginContainer.ExpandedWidth = value.Height;
      }
    }

    void ISidebar.AddInfo(ComicPageControl page)
    {
      this.comicInfo.Controls.Add((Control) page);
      this.pluginContainer.Visible = true;
      this.UpdatePreviewPadding();
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.smallComicPreview = new SmallComicPreview();
      this.comicBrowser = new ComicBrowserControl();
      this.previewTimer = new Timer(this.components);
      this.sidePanel = new SizableContainer();
      this.treePanel = new Panel();
      this.previewPane = new SizableContainer();
      this.pluginContainer = new SizableContainer();
      this.comicInfo = new ComicPageContainerControl();
      this.pluginPlaceholder = new Panel();
      this.sidePanel.SuspendLayout();
      this.previewPane.SuspendLayout();
      this.pluginContainer.SuspendLayout();
      this.SuspendLayout();
      this.smallComicPreview.Caption = "";
      this.smallComicPreview.CaptionMargin = new Padding(2);
      this.smallComicPreview.Dock = DockStyle.Fill;
      this.smallComicPreview.Location = new System.Drawing.Point(2, 8);
      this.smallComicPreview.Name = "smallComicPreview";
      this.smallComicPreview.Size = new System.Drawing.Size(242, 197);
      this.smallComicPreview.TabIndex = 0;
      this.smallComicPreview.TwoPageDisplay = false;
      this.smallComicPreview.CloseClicked += new EventHandler(this.smallComicPreview_CloseClicked);
      this.comicBrowser.Caption = "";
      this.comicBrowser.CaptionMargin = new Padding(2);
      this.comicBrowser.Dock = DockStyle.Fill;
      this.comicBrowser.Location = new System.Drawing.Point(252, 0);
      this.comicBrowser.Name = "comicBrowser";
      this.comicBrowser.Size = new System.Drawing.Size(448, 370);
      this.comicBrowser.TabIndex = 0;
      this.previewTimer.Interval = 500;
      this.previewTimer.Tick += new EventHandler(this.previewTimer_Tick);
      this.sidePanel.AutoGripPosition = true;
      this.sidePanel.Controls.Add((Control) this.treePanel);
      this.sidePanel.Controls.Add((Control) this.previewPane);
      this.sidePanel.Dock = DockStyle.Left;
      this.sidePanel.Grip = SizableContainer.GripPosition.Right;
      this.sidePanel.Location = new System.Drawing.Point(0, 0);
      this.sidePanel.Name = "sidePanel";
      this.sidePanel.Size = new System.Drawing.Size(252, 538);
      this.sidePanel.TabIndex = 1;
      this.sidePanel.ExpandedChanged += new EventHandler(this.sidePanel_ExpandedChanged);
      this.treePanel.Dock = DockStyle.Fill;
      this.treePanel.Location = new System.Drawing.Point(0, 0);
      this.treePanel.Name = "treePanel";
      this.treePanel.Size = new System.Drawing.Size(246, 331);
      this.treePanel.TabIndex = 0;
      this.previewPane.AutoGripPosition = true;
      this.previewPane.BorderStyle = ExtendedBorderStyle.Flat;
      this.previewPane.Controls.Add((Control) this.smallComicPreview);
      this.previewPane.Dock = DockStyle.Bottom;
      this.previewPane.Location = new System.Drawing.Point(0, 331);
      this.previewPane.Name = "previewPane";
      this.previewPane.Size = new System.Drawing.Size(246, 207);
      this.previewPane.TabIndex = 1;
      this.previewPane.Text = "sizableContainer1";
      this.previewPane.ExpandedChanged += new EventHandler(this.sidePanel_ExpandedChanged);
      this.pluginContainer.AutoGripPosition = true;
      this.pluginContainer.Controls.Add((Control) this.comicInfo);
      this.pluginContainer.Dock = DockStyle.Bottom;
      this.pluginContainer.Location = new System.Drawing.Point(252, 370);
      this.pluginContainer.Name = "pluginContainer";
      this.pluginContainer.Size = new System.Drawing.Size(448, 162);
      this.pluginContainer.TabIndex = 2;
      this.pluginContainer.Visible = false;
      this.pluginContainer.ExpandedChanged += new EventHandler(this.pluginContainer_ExpandedChanged);
      this.comicInfo.Dock = DockStyle.Fill;
      this.comicInfo.Location = new System.Drawing.Point(0, 6);
      this.comicInfo.Name = "comicInfo";
      this.comicInfo.Size = new System.Drawing.Size(448, 156);
      this.comicInfo.TabIndex = 0;
      this.pluginPlaceholder.Dock = DockStyle.Bottom;
      this.pluginPlaceholder.Location = new System.Drawing.Point(252, 532);
      this.pluginPlaceholder.Name = "pluginPlaceholder";
      this.pluginPlaceholder.Size = new System.Drawing.Size(448, 6);
      this.pluginPlaceholder.TabIndex = 3;
      this.AutoScaleMode = AutoScaleMode.None;
      this.Controls.Add((Control) this.comicBrowser);
      this.Controls.Add((Control) this.pluginContainer);
      this.Controls.Add((Control) this.pluginPlaceholder);
      this.Controls.Add((Control) this.sidePanel);
      this.Name = nameof (ComicExplorerView);
      this.Size = new System.Drawing.Size(700, 538);
      this.sidePanel.ResumeLayout(false);
      this.previewPane.ResumeLayout(false);
      this.pluginContainer.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

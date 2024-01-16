// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.QuickOpenView
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class QuickOpenView : CaptionControl
  {
    private readonly ThumbnailConfig tc = new ThumbnailConfig()
    {
      HideCaptions = true
    };
    private IContainer components;
    private ItemView itemView;
    private Panel panelStatus;
    private Button btBrowser;
    private Button btOpen;
    private Button btOpenFile;
    private ComicPageContainerControl comicPageContainer;

    public QuickOpenView()
    {
      this.InitializeComponent();
      this.itemView.ItemGrouper = (IGrouper<IViewableItem>) new QuickOpenView.CoverItemCustomGroupGrouper();
      this.itemView.MouseWheel += new MouseEventHandler(this.itemView_MouseWheel);
      LocalizeUtility.Localize((Control) this, this.components);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.components != null)
          this.components.Dispose();
        this.itemView.Items.Clear();
      }
      base.Dispose(disposing);
    }

    public void BeginUpdate()
    {
      this.itemView.Items.Clear();
      this.btOpen.Enabled = false;
    }

    public void AddGroup(IGroupInfo group, IEnumerable<ComicBook> books, int maxCount)
    {
      HashSet<Guid> h = new HashSet<Guid>(this.itemView.Items.OfType<CoverViewItem>().Select<CoverViewItem, Guid>((Func<CoverViewItem, Guid>) (item => item.Comic.Id)));
      int n = this.itemView.Items.Count;
      foreach (CoverViewItem coverViewItem in books.OrderBy<ComicBook, ComicBook>((Func<ComicBook, ComicBook>) (cb => cb), (IComparer<ComicBook>) new QuickOpenView.ComicBookOpenedSorter()).Where<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsLinked)).Where<ComicBook>((Func<ComicBook, bool>) (cb => !h.Contains(cb.Id))).Take<ComicBook>(maxCount).Select<ComicBook, CoverViewItem>((Func<ComicBook, CoverViewItem>) (cb => CoverViewItem.Create(cb, ++n, (IComicBookStatsProvider) null))))
      {
        coverViewItem.CustomGroup = group;
        coverViewItem.ThumbnailConfig = this.tc;
        this.itemView.Items.Add((IViewableItem) coverViewItem);
      }
    }

    public void EndUpdate()
    {
      this.comicPageContainer.ShowInfo((IEnumerable<ComicBook>) this.itemView.Items.OfType<CoverViewItem>().Select<CoverViewItem, ComicBook>((Func<CoverViewItem, ComicBook>) (cvi => cvi.Comic)).ToArray<ComicBook>());
    }

    public event EventHandler BookActivated;

    public event EventHandler ShowBrowser;

    public event EventHandler OpenFile;

    public ComicBook SelectedBook
    {
      get
      {
        return this.itemView.SelectedItems.FirstOrDefault<IViewableItem>() is CoverViewItem coverViewItem ? coverViewItem.Comic : (ComicBook) null;
      }
    }

    public bool ShowBrowserCommand
    {
      get => this.btBrowser.Visible;
      set => this.btBrowser.Visible = value;
    }

    public int ThumbnailSize
    {
      get => this.itemView.ItemThumbSize.Height;
      set
      {
        value = value.Clamp(96, 512);
        this.itemView.ItemThumbSize = new System.Drawing.Size(value, value);
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      try
      {
        this.itemView.Text = TR.Messages["Books"];
        ScriptUtility.CreateQuickOpenPages().ForEach<ComicPageControl>((Action<ComicPageControl>) (p =>
        {
          p.MarkAsDirty();
          p.Visible = false;
          this.comicPageContainer.Controls.Add((Control) p);
        }));
      }
      catch
      {
      }
    }

    protected virtual void OnItemActivate()
    {
      if (this.BookActivated == null)
        return;
      this.BookActivated((object) this, EventArgs.Empty);
    }

    protected virtual void OnShowBrowser()
    {
      if (this.ShowBrowser == null)
        return;
      this.ShowBrowser((object) this, EventArgs.Empty);
    }

    protected virtual void OnOpenFile()
    {
      if (this.OpenFile == null)
        return;
      this.OpenFile((object) this, EventArgs.Empty);
    }

    private void itemView_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.btOpen.Enabled = this.SelectedBook != null;
    }

    private void itemView_ItemActivate(object sender, EventArgs e) => this.OnItemActivate();

    private void btOpen_Click(object sender, EventArgs e) => this.OnItemActivate();

    private void btBrowser_Click(object sender, EventArgs e) => this.OnShowBrowser();

    private void btOpenFile_Click(object sender, EventArgs e) => this.OnOpenFile();

    private void itemView_MouseWheel(object sender, MouseEventArgs e)
    {
      if (!Control.ModifierKeys.HasFlag((Enum) Keys.Control))
        return;
      this.ThumbnailSize += e.Delta / SystemInformation.MouseWheelScrollDelta * 16;
    }

    private void itemView_PostPaint(object sender, PaintEventArgs e)
    {
      e.Graphics.DrawShadow(this.itemView.DisplayRectangle, 8, Color.Black, 0.125f, BlurShadowType.Inside, BlurShadowParts.Edges);
    }

    private void itemView_VisibleChanged(object sender, EventArgs e)
    {
      this.btOpen.Visible = this.itemView.Visible;
    }

    private void InitializeComponent()
    {
      this.itemView = new ItemView();
      this.panelStatus = new Panel();
      this.btOpenFile = new Button();
      this.btBrowser = new Button();
      this.btOpen = new Button();
      this.comicPageContainer = new ComicPageContainerControl();
      this.panelStatus.SuspendLayout();
      this.comicPageContainer.SuspendLayout();
      this.SuspendLayout();
      this.itemView.AutomaticHeaderMenu = false;
      this.itemView.AutomaticViewMenu = false;
      this.itemView.BackColor = SystemColors.Window;
      this.itemView.Dock = DockStyle.Fill;
      this.itemView.EnableStick = false;
      this.itemView.GroupCollapsedImage = Resources.ArrowRight;
      this.itemView.GroupDisplayEnabled = true;
      this.itemView.GroupExpandedImage = Resources.ArrowDown;
      this.itemView.HorizontalItemAlignment = HorizontalAlignment.Center;
      this.itemView.LabelEdit = false;
      this.itemView.Location = new System.Drawing.Point(0, 0);
      this.itemView.Multiselect = false;
      this.itemView.Name = "itemView";
      this.itemView.SelectionMode = SelectionMode.One;
      this.itemView.ShowGroupCount = false;
      this.itemView.Size = new System.Drawing.Size(573, 383);
      this.itemView.SortColumn = (IColumn) null;
      this.itemView.SortColumns = new IColumn[0];
      this.itemView.SortColumnsKey = "";
      this.itemView.TabIndex = 0;
      this.itemView.ItemActivate += new EventHandler(this.itemView_ItemActivate);
      this.itemView.SelectedIndexChanged += new EventHandler(this.itemView_SelectedIndexChanged);
      this.itemView.PostPaint += new PaintEventHandler(this.itemView_PostPaint);
      this.itemView.VisibleChanged += new EventHandler(this.itemView_VisibleChanged);
      this.panelStatus.BorderStyle = BorderStyle.FixedSingle;
      this.panelStatus.Controls.Add((Control) this.btOpenFile);
      this.panelStatus.Controls.Add((Control) this.btBrowser);
      this.panelStatus.Controls.Add((Control) this.btOpen);
      this.panelStatus.Dock = DockStyle.Bottom;
      this.panelStatus.Location = new System.Drawing.Point(0, 402);
      this.panelStatus.Name = "panelStatus";
      this.panelStatus.Size = new System.Drawing.Size(573, 37);
      this.panelStatus.TabIndex = 1;
      this.btOpenFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btOpenFile.Location = new System.Drawing.Point(3, 6);
      this.btOpenFile.Name = "btOpenFile";
      this.btOpenFile.Size = new System.Drawing.Size(90, 23);
      this.btOpenFile.TabIndex = 0;
      this.btOpenFile.Text = "Open File...";
      this.btOpenFile.UseVisualStyleBackColor = true;
      this.btOpenFile.Click += new EventHandler(this.btOpenFile_Click);
      this.btBrowser.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btBrowser.Location = new System.Drawing.Point(478, 6);
      this.btBrowser.Name = "btBrowser";
      this.btBrowser.Size = new System.Drawing.Size(90, 23);
      this.btBrowser.TabIndex = 2;
      this.btBrowser.Text = "Browser";
      this.btBrowser.UseVisualStyleBackColor = true;
      this.btBrowser.Click += new EventHandler(this.btBrowser_Click);
      this.btOpen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btOpen.Location = new System.Drawing.Point(99, 6);
      this.btOpen.Name = "btOpen";
      this.btOpen.Size = new System.Drawing.Size(90, 23);
      this.btOpen.TabIndex = 1;
      this.btOpen.Text = "Open";
      this.btOpen.UseVisualStyleBackColor = true;
      this.btOpen.Click += new EventHandler(this.btOpen_Click);
      this.comicPageContainer.Controls.Add((Control) this.itemView);
      this.comicPageContainer.Dock = DockStyle.Fill;
      this.comicPageContainer.Location = new System.Drawing.Point(0, 19);
      this.comicPageContainer.Name = "comicPageContainer";
      this.comicPageContainer.Size = new System.Drawing.Size(573, 383);
      this.comicPageContainer.TabIndex = 2;
      this.comicPageContainer.Text = "Books";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.Caption = "Quick Open";
      this.Controls.Add((Control) this.comicPageContainer);
      this.Controls.Add((Control) this.panelStatus);
      this.Name = nameof (QuickOpenView);
      this.Size = new System.Drawing.Size(573, 439);
      this.panelStatus.ResumeLayout(false);
      this.comicPageContainer.ResumeLayout(false);
      this.comicPageContainer.PerformLayout();
      this.ResumeLayout(false);
    }

    private class CoverItemCustomGroupGrouper : IGrouper<IViewableItem>
    {
      public bool IsMultiGroup => false;

      public IGroupInfo GetGroup(IViewableItem item) => ((CoverViewItem) item).CustomGroup;

      public IEnumerable<IGroupInfo> GetGroups(IViewableItem item)
      {
        throw new NotImplementedException();
      }
    }

    private class ComicBookOpenedSorter : IComparer<ComicBook>
    {
      public int Compare(ComicBook cx, ComicBook cy)
      {
        int num = cy.OpenedTime.CompareTo(cx.OpenedTime);
        return num != 0 ? num : cy.AddedTime.CompareTo(cx.AddedTime);
      }
    }
  }
}

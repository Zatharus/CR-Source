// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.Forms.NavigationOverlay
// Assembly: ComicRack.Engine.Display.Forms, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: D83BAE4E-CA55-445A-AD1D-2DF78C341143
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.Display.Forms.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Presentation;
using cYo.Common.Presentation.Panels;
using cYo.Common.Threading;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Display.Forms.Properties;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display.Forms
{
  public class NavigationOverlay : OverlayPanel
  {
    private static readonly int ButtonSize = FormUtility.ScaleDpiX(28);
    private static readonly int PagePadding = FormUtility.ScaleDpiX(6);
    private static readonly int w1 = FormUtility.ScaleDpiX(1);
    private static readonly int w2 = FormUtility.ScaleDpiX(2);
    private static readonly int w3 = FormUtility.ScaleDpiX(3);
    private static readonly int w4 = FormUtility.ScaleDpiX(4);
    private readonly SimpleScrollbarPanel pageSlider;
    private readonly LabelPanel comicNameLabel;
    private readonly LabelPanel timeLabel;
    private readonly BatteryStatus batteryStatus;
    private readonly ScalableBitmap thumbBack = new ScalableBitmap(Resources.GradientFramedBackground, 0, 3, 0, 3);
    private readonly BitmapAdjustment adjustment = new BitmapAdjustment(-1f);
    private bool mirror;
    private int[] pages = new int[0];
    private bool isDoublePage;
    private IThumbnailPool pool;
    private IImageProvider provider;
    private int selectedPage = -1;
    private System.Drawing.Point downPoint;
    private int downPage = -1;
    private bool thumbnailScroll;
    private readonly List<NavigationOverlay.IndexRectangle> thumbnailAreas = new List<NavigationOverlay.IndexRectangle>();

    public NavigationOverlay(System.Drawing.Size size)
      : base(size)
    {
      this.Margin = PanelRenderer.GetMargin((RectangleF) this.ClientRectangle);
      this.AddButton(NavigationOverlay.ButtonSize, ContentAlignment.BottomLeft, 0, Resources.GoFirst.ScaleDpi(), new EventHandler(this.PageBrowseLeftDoubleClick));
      this.AddButton(NavigationOverlay.ButtonSize, ContentAlignment.BottomLeft, NavigationOverlay.ButtonSize + NavigationOverlay.w4, Resources.GoPrevious.ScaleDpi(), new EventHandler(this.PageBrowseLeftClick));
      this.AddButton(NavigationOverlay.ButtonSize, ContentAlignment.BottomRight, -NavigationOverlay.ButtonSize - NavigationOverlay.w4, Resources.GoNext.ScaleDpi(), new EventHandler(this.PageBrowseRightClick));
      this.AddButton(NavigationOverlay.ButtonSize, ContentAlignment.BottomRight, 0, Resources.GoLast.ScaleDpi(), new EventHandler(this.PageBrowseRightDoubleClick));
      SimpleScrollbarPanel simpleScrollbarPanel = new SimpleScrollbarPanel(this.GetScrollbarSize(size));
      simpleScrollbarPanel.HitTestType = cYo.Common.Presentation.Panels.HitTestType.Bounds;
      simpleScrollbarPanel.BackColor = Color.Transparent;
      simpleScrollbarPanel.Visible = true;
      simpleScrollbarPanel.AutoAlign = true;
      simpleScrollbarPanel.Alignment = ContentAlignment.BottomCenter;
      simpleScrollbarPanel.AlignmentOffset = new System.Drawing.Point(0, -NavigationOverlay.w2);
      simpleScrollbarPanel.Background = new ScalableBitmap(Resources.BlackGlassScrollbarBack, new Padding(3));
      simpleScrollbarPanel.Knob = (ScalableBitmap) Resources.GrayGlassButton.Resize(new System.Drawing.Size(32, 32).ScaleDpi(), BitmapResampling.GdiPlusHQ);
      this.pageSlider = simpleScrollbarPanel;
      this.pageSlider.BorderWidth = this.pageSlider.Height / 2 - NavigationOverlay.w3;
      this.pageSlider.Scroll += new EventHandler(this.PageSliderScroll);
      this.pageSlider.ValueChanged += (EventHandler) ((x, y) => this.Invalidate());
      this.pageSlider.MinimumChanged += (EventHandler) ((x, y) => this.Invalidate());
      this.pageSlider.MouseUp += new MouseEventHandler(this.PageSliderMouseUp);
      this.Panels.Add((OverlayPanel) this.pageSlider);
      LabelPanel labelPanel1 = new LabelPanel();
      labelPanel1.Size = NavigationOverlay.GetNameLabelSize(size);
      labelPanel1.TextSize = 9f;
      labelPanel1.Alignment = ContentAlignment.TopLeft;
      labelPanel1.AlignmentOffset = new System.Drawing.Point(0, -NavigationOverlay.w1);
      labelPanel1.AutoAlign = true;
      labelPanel1.Visible = true;
      this.comicNameLabel = labelPanel1;
      this.Panels.Add((OverlayPanel) this.comicNameLabel);
      int num = 0;
      if (SystemInformation.PowerStatus.BatteryChargeStatus != BatteryChargeStatus.NoSystemBattery)
      {
        BatteryStatus batteryStatus = new BatteryStatus();
        batteryStatus.Alignment = ContentAlignment.TopRight;
        batteryStatus.AlignmentOffset = new System.Drawing.Point(0, NavigationOverlay.w3);
        batteryStatus.AutoAlign = true;
        batteryStatus.Visible = true;
        this.batteryStatus = batteryStatus;
        this.Panels.Add((OverlayPanel) this.batteryStatus);
        num = this.batteryStatus.Width + NavigationOverlay.w4;
      }
      LabelPanel labelPanel2 = new LabelPanel();
      labelPanel2.Size = NavigationOverlay.GetTimeLabelSize();
      labelPanel2.TextSize = 9f;
      labelPanel2.TextAlignment = ContentAlignment.MiddleRight;
      labelPanel2.Alignment = ContentAlignment.TopRight;
      labelPanel2.AlignmentOffset = new System.Drawing.Point(-num, -NavigationOverlay.w1);
      labelPanel2.AutoAlign = true;
      labelPanel2.Visible = true;
      this.timeLabel = labelPanel2;
      this.timeLabel.Drawing += (EventHandler) ((s, e) => this.timeLabel.Text = DateTime.Now.ToString("t"));
      this.Panels.Add((OverlayPanel) this.timeLabel);
    }

    private System.Drawing.Size GetScrollbarSize(System.Drawing.Size size)
    {
      return new System.Drawing.Size(size.Width - this.Margin.Horizontal - 4 * NavigationOverlay.w4 - 2 * NavigationOverlay.w4 - 4 * NavigationOverlay.ButtonSize, NavigationOverlay.ButtonSize - NavigationOverlay.w4);
    }

    private static System.Drawing.Size GetNameLabelSize(System.Drawing.Size size)
    {
      return new System.Drawing.Size(size.Width - FormUtility.ScaleDpiX(80), FormUtility.ScaleDpiY(15));
    }

    private static System.Drawing.Size GetTimeLabelSize() => new System.Drawing.Size(50, 15).ScaleDpi();

    private void AddButton(
      int buttonSize,
      ContentAlignment align,
      int offset,
      Bitmap bi,
      EventHandler click)
    {
      System.Drawing.Size size = new System.Drawing.Size(buttonSize, buttonSize);
      SimpleButtonPanel simpleButtonPanel1 = new SimpleButtonPanel(size);
      simpleButtonPanel1.Background = (ScalableBitmap) Resources.GrayGlassButton.Resize(size, BitmapResampling.GdiPlusHQ);
      simpleButtonPanel1.Icon = (ScalableBitmap) bi.CreateAdjustedBitmap(this.adjustment, true);
      simpleButtonPanel1.Margin = new Padding(NavigationOverlay.w4);
      simpleButtonPanel1.Visible = true;
      simpleButtonPanel1.AutoAlign = true;
      simpleButtonPanel1.AlignmentOffset = new System.Drawing.Point(offset, 0);
      simpleButtonPanel1.Alignment = align;
      SimpleButtonPanel simpleButtonPanel2 = simpleButtonPanel1;
      simpleButtonPanel2.Click += click;
      this.Panels.Add((OverlayPanel) simpleButtonPanel2);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Panels.Dispose();
        this.Pool = (IThumbnailPool) null;
      }
      base.Dispose(disposing);
    }

    public bool Mirror
    {
      get => this.mirror;
      set
      {
        if (this.mirror == value)
          return;
        this.mirror = value;
        this.pageSlider.Mirror = this.mirror;
        this.Invalidate();
      }
    }

    public int[] Pages
    {
      get => this.pages;
      set
      {
        this.pages = value;
        this.pageSlider.Maximum = this.pages.Length - 1;
      }
    }

    public int DisplayedPageIndex
    {
      get => this.pageSlider.Value;
      set
      {
        if (value == -1 || this.pageSlider.Value == value)
          return;
        this.pageSlider.Value = value;
      }
    }

    public bool IsDoublePage
    {
      get => this.isDoublePage;
      set
      {
        if (this.isDoublePage == value)
          return;
        this.isDoublePage = value;
        this.Invalidate();
      }
    }

    public IThumbnailPool Pool
    {
      get => this.pool;
      set
      {
        if (this.pool == value)
          return;
        if (this.pool != null)
          this.pool.ThumbnailCached -= new EventHandler<CacheItemEventArgs<ImageKey, ThumbnailImage>>(this.MemoryThumbnailCacheItemAdded);
        this.pool = value;
        if (this.pool != null)
          this.pool.ThumbnailCached += new EventHandler<CacheItemEventArgs<ImageKey, ThumbnailImage>>(this.MemoryThumbnailCacheItemAdded);
        this.Invalidate();
      }
    }

    public IImageProvider Provider
    {
      get => this.provider;
      set
      {
        if (this.provider == value)
          return;
        this.provider = value;
        this.Invalidate();
      }
    }

    public IImageKeyProvider ImageKeyProvider { get; set; }

    public string Caption
    {
      get => this.comicNameLabel.Text;
      set => this.comicNameLabel.Text = value;
    }

    public int SelectedPage
    {
      get => this.selectedPage;
      set
      {
        if (this.selectedPage == value)
          return;
        this.Invalidate(this.selectedPage);
        this.selectedPage = value;
        this.Invalidate(this.selectedPage);
      }
    }

    private void MemoryThumbnailCacheItemAdded(
      object sender,
      CacheItemEventArgs<ImageKey, ThumbnailImage> e)
    {
      this.Invalidate();
    }

    private void PageBrowseLeftClick(object sender, EventArgs e)
    {
      this.OnBrowse(new BrowseEventArgs(PageSeekOrigin.Current, -1));
    }

    private void PageBrowseRightClick(object sender, EventArgs e)
    {
      this.OnBrowse(new BrowseEventArgs(PageSeekOrigin.Current, 1));
    }

    private void PageBrowseRightDoubleClick(object sender, EventArgs e)
    {
      this.OnBrowse(new BrowseEventArgs(PageSeekOrigin.End, 0));
    }

    private void PageBrowseLeftDoubleClick(object sender, EventArgs e)
    {
      this.OnBrowse(new BrowseEventArgs(PageSeekOrigin.Beginning, 0));
    }

    private void PageSliderScroll(object sender, EventArgs e) => this.Invalidate();

    private void PageSliderMouseUp(object sender, MouseEventArgs e)
    {
      this.OnBrowse(new BrowseEventArgs(PageSeekOrigin.Absolute, this.pages[this.pageSlider.Value]));
    }

    public void Invalidate(int page)
    {
      if (page < 0)
        return;
      using (ItemMonitor.Lock((object) this.thumbnailAreas))
        this.Invalidate(this.thumbnailAreas.FirstOrDefault<NavigationOverlay.IndexRectangle>((Func<NavigationOverlay.IndexRectangle, bool>) (ta => ta.Page == page)).Bounds);
    }

    protected override void OnSizeChanged()
    {
      base.OnSizeChanged();
      this.pageSlider.Size = this.GetScrollbarSize(this.Size);
      this.comicNameLabel.Size = NavigationOverlay.GetNameLabelSize(this.Size);
      this.timeLabel.Size = NavigationOverlay.GetTimeLabelSize();
      this.Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.downPoint = e.Location;
      this.downPage = -1;
      this.thumbnailScroll = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.downPoint.IsEmpty)
      {
        this.SelectedPage = this.PageHitTest(e.Location);
      }
      else
      {
        if (this.downPage == -1)
          this.downPage = this.PageHitTest(e.Location);
        if (this.downPage == -1)
          return;
        this.SelectedPage = this.downPage;
        NavigationOverlay.IndexRectangle indexRectangle = this.thumbnailAreas.FirstOrDefault<NavigationOverlay.IndexRectangle>((Func<NavigationOverlay.IndexRectangle, bool>) (ta => ta.Page == this.downPage));
        if (indexRectangle.Bounds.Width == 0 || e.Y < indexRectangle.Bounds.Top || e.Y > indexRectangle.Bounds.Bottom)
          return;
        int num = 0;
        if (e.X > indexRectangle.Bounds.Right + NavigationOverlay.PagePadding)
          num = -1;
        else if (e.X < indexRectangle.Bounds.Left - NavigationOverlay.PagePadding)
          num = 1;
        if (num == 0)
          return;
        this.thumbnailScroll = true;
        this.downPage = -1;
        if (this.Mirror)
          this.pageSlider.Value -= num;
        else
          this.pageSlider.Value += num;
      }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      this.SelectedPage = -1;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.downPoint = System.Drawing.Point.Empty;
      int offset = this.thumbnailScroll ? this.pages[this.pageSlider.Value] : this.PageHitTest(e.Location);
      if (offset == -1)
        return;
      this.OnBrowse(new BrowseEventArgs(PageSeekOrigin.Absolute, offset));
    }

    protected override void OnScaleChanged()
    {
      this.Panels.ForEach((Action<OverlayPanel>) (p => p.Scale = this.Scale));
      base.OnScaleChanged();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics graphics = e.Graphics;
      Rectangle clientRectangle = this.ClientRectangle;
      bool isDoublePage = this.IsDoublePage;
      Rectangle dest = Rectangle.Round(PanelRenderer.DrawGraphics(graphics, (RectangleF) clientRectangle, 0.7f)).Pad(0, this.comicNameLabel.Height, bottom: NavigationOverlay.ButtonSize + 4);
      Rectangle rectangle1 = Rectangle.Round(this.thumbBack.Draw((IBitmapRenderer) (BitmapGdiRenderer) graphics, (RectangleF) dest)).Pad(4, 2, 4);
      graphics.IntersectClip(rectangle1);
      using (ItemMonitor.Lock((object) this.thumbnailAreas))
        this.thumbnailAreas.Clear();
      bool flag1 = false;
      int offset1 = rectangle1.X + rectangle1.Width / 2;
      int offset2 = offset1;
      int displayedPageIndex = this.DisplayedPageIndex;
      int pageIndex = this.DisplayedPageIndex + 1;
      bool flag2 = true;
      if (isDoublePage)
      {
        flag1 = true;
      }
      else
      {
        Rectangle rectangle2 = this.GetPageSize(displayedPageIndex, rectangle1).ToRectangle().AlignHorizontal(offset1, StringAlignment.Center) with
        {
          Y = rectangle1.Y
        };
        if (rectangle1.IntersectsWith(rectangle2))
        {
          this.DrawPage(graphics, rectangle2, displayedPageIndex, true);
          this.AddPageArea(rectangle2, displayedPageIndex);
          flag1 = true;
        }
        offset1 -= (rectangle2.Width / 2 + NavigationOverlay.PagePadding) * (this.mirror ? -1 : 1);
        offset2 += (rectangle2.Width / 2 + NavigationOverlay.PagePadding) * (this.mirror ? -1 : 1);
        --displayedPageIndex;
        flag2 = false;
      }
      while (flag1)
      {
        flag1 = false;
        System.Drawing.Size pageSize = this.GetPageSize(displayedPageIndex, rectangle1);
        if (!pageSize.IsEmpty)
        {
          Rectangle rectangle3 = pageSize.ToRectangle().AlignHorizontal(offset1, this.mirror ? StringAlignment.Far : StringAlignment.Near) with
          {
            Y = rectangle1.Y
          };
          if (rectangle1.IntersectsWith(rectangle3))
          {
            this.DrawPage(graphics, rectangle3, displayedPageIndex, flag2 || displayedPageIndex == this.SelectedPage);
            this.AddPageArea(rectangle3, displayedPageIndex);
            offset1 -= (rectangle3.Width + NavigationOverlay.PagePadding) * (this.mirror ? -1 : 1);
            --displayedPageIndex;
            flag1 = true;
          }
        }
        pageSize = this.GetPageSize(pageIndex, rectangle1);
        if (!pageSize.IsEmpty)
        {
          Rectangle rectangle4 = pageSize.ToRectangle().AlignHorizontal(offset2, this.mirror ? StringAlignment.Near : StringAlignment.Far) with
          {
            Y = rectangle1.Y
          };
          if (rectangle1.IntersectsWith(rectangle4))
          {
            this.DrawPage(graphics, rectangle4, pageIndex, flag2 || pageIndex == this.SelectedPage);
            this.AddPageArea(rectangle4, pageIndex);
            offset2 += (rectangle4.Width + NavigationOverlay.PagePadding) * (this.mirror ? -1 : 1);
            ++pageIndex;
            flag1 = true;
          }
        }
        flag2 = false;
      }
    }

    public event EventHandler<BrowseEventArgs> Browse;

    protected virtual void OnBrowse(BrowseEventArgs e)
    {
      if (this.Browse == null)
        return;
      this.Browse((object) this, e);
    }

    private int PageHitTest(System.Drawing.Point pt)
    {
      return this.thumbnailAreas.Lock<NavigationOverlay.IndexRectangle>().Where<NavigationOverlay.IndexRectangle>((Func<NavigationOverlay.IndexRectangle, bool>) (ir => ir.Bounds.Contains(pt))).Select<NavigationOverlay.IndexRectangle, int>((Func<NavigationOverlay.IndexRectangle, int>) (ir => ir.Page)).FirstOrValue<int>(-1);
    }

    private IItemLock<ThumbnailImage> GetThumbnail(int pageIndex)
    {
      if (this.pool == null || this.ImageKeyProvider == null || pageIndex >= this.pages.Length)
        return (IItemLock<ThumbnailImage>) new ItemLock<ThumbnailImage>((ThumbnailImage) null);
      ThumbnailKey key = new ThumbnailKey(this.ImageKeyProvider.GetImageKey(this.pages[pageIndex]));
      IItemLock<ThumbnailImage> thumbnail = this.pool.GetThumbnail(key, true);
      if (thumbnail != null)
        return thumbnail;
      this.pool.CacheThumbnail(key, true, this.provider);
      return (IItemLock<ThumbnailImage>) new ItemLock<ThumbnailImage>((ThumbnailImage) null);
    }

    private void AddPageArea(Rectangle trc, int pageIndex)
    {
      using (ItemMonitor.Lock((object) this.thumbnailAreas))
        this.thumbnailAreas.Add(new NavigationOverlay.IndexRectangle()
        {
          Bounds = trc,
          Page = this.pages[pageIndex]
        });
    }

    private System.Drawing.Size GetPageSize(int pageIndex, Rectangle rc)
    {
      if (pageIndex < 0 || pageIndex >= this.pages.Length)
        return System.Drawing.Size.Empty;
      using (IItemLock<ThumbnailImage> thumbnail = this.GetThumbnail(pageIndex))
        return thumbnail.Item == null ? new System.Drawing.Size(rc.Height * 3 / 4, rc.Height) : thumbnail.Item.OriginalSize.ToRectangle(new System.Drawing.Size(0, rc.Height), RectangleScaleMode.None).Size;
    }

    private void DrawPage(Graphics gr, Rectangle trc, int pageIndex, bool isSelected)
    {
      if (!gr.IsVisible(trc) || pageIndex < 0 || pageIndex >= this.pages.Length)
        return;
      int page = this.pages[pageIndex];
      using (IItemLock<ThumbnailImage> thumbnail1 = this.GetThumbnail(pageIndex))
      {
        Image thumbnail2 = thumbnail1.Item == null ? (Image) null : (Image) thumbnail1.Item.GetThumbnail(trc.Height);
        float num = isSelected ? 1f : 0.3f;
        if (thumbnail2 == null)
        {
          using (StringFormat format = new StringFormat()
          {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
          })
          {
            using (Brush brush = (Brush) new SolidBrush(Color.FromArgb((int) ((double) byte.MaxValue * (double) num), PanelRenderer.GetForeColor())))
              gr.DrawString((page + 1).ToString(), FC.Get("Times", 32f), brush, (RectangleF) trc, format);
          }
        }
        else
        {
          ThumbnailDrawingOptions flags = (ThumbnailDrawingOptions) (6127 & -2);
          new ThumbRenderer(thumbnail2, flags)
          {
            PageNumber = (page + 1),
            ImageOpacity = num
          }.DrawThumbnail(gr, trc);
        }
      }
    }

    private struct IndexRectangle
    {
      public Rectangle Bounds;
      public int Page;
    }
  }
}

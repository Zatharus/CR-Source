// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TabBar
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using cYo.Common.Threading;
using cYo.Common.Win32;
using cYo.Common.Windows.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using Windows7.Multitouch;
using Windows7.Multitouch.WinForms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TabBar : ContainerControl
  {
    private static readonly int arrowWidth = SystemInformation.VerticalScrollBarWidth;
    private static readonly int dropDownWidth = SystemInformation.VerticalScrollBarWidth;
    public static bool StyleEnabled = true;
    private static Bitmap arrowLeft = Resources.SimpleArrowLeft;
    private static Bitmap arrowRight = Resources.SimpleArrowRight;
    private static Bitmap arrowDown = Resources.SimpleArrowDown;
    private static Bitmap insertArrow = Resources.InsertArrow;
    private readonly Timer scrollTimer = new Timer();
    private readonly Timer toolTipTimer = new Timer();
    private readonly System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
    private System.Drawing.Point clickPoint;
    private int inDrag = -1;
    private readonly TabBar.TabBarItemCollection items = new TabBar.TabBarItemCollection();
    private TabBar.TabBarItem selectedTab;
    private bool showDropDown = true;
    private Bitmap closeImage;
    private int tabHeight;
    private int topPadding = 2;
    private int bottomPadding = 4;
    private bool drawBaseLine = true;
    private int leftIndent;
    private int minimumTabWidth = 250;
    private int markerPosition = -1;
    private bool showArrows;
    private TabBar.ItemState leftArrowState;
    private TabBar.ItemState rightArrowState;
    private TabBar.ItemState dropDownState;
    private int tabsOffset;
    private Rectangle tabBounds;
    private int tabsWidth;
    private TabBar.TabBarItem toolTipItem;
    private readonly Dictionary<TabBar.TabBarItem, Image> animatedImages = new Dictionary<TabBar.TabBarItem, Image>();
    private GestureHandler gestureHandler;

    public TabBar()
    {
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.Selectable, true);
      this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      this.items.Changed += new EventHandler<SmartListChangedEventArgs<TabBar.TabBarItem>>(this.items_Changed);
      this.AutoSize = true;
      this.AllowDrop = true;
      this.scrollTimer.Interval = 25;
      this.scrollTimer.Enabled = false;
      this.scrollTimer.Tick += new EventHandler(this.scrollTimer_Tick);
      this.toolTipTimer.Interval = SystemInformation.MouseHoverTime;
      this.toolTipTimer.Enabled = false;
      this.toolTipTimer.Tick += new EventHandler(this.toolTipTimer_Tick);
      this.toolTip.OwnerDraw = true;
      this.toolTip.Draw += new DrawToolTipEventHandler(this.toolTip_Draw);
      this.toolTip.Popup += new PopupEventHandler(this.toolTip_Popup);
    }

    protected override void OnCreateControl()
    {
      this.InitWindowsTouch();
      base.OnCreateControl();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.animatedImages.Values.ForEach<Image>((Action<Image>) (img => ImageAnimator.StopAnimate(img, new EventHandler(this.AnimationHandler))));
        this.scrollTimer.Dispose();
        this.toolTipTimer.Dispose();
        this.toolTip.Dispose();
      }
      base.Dispose(disposing);
    }

    public TabBar.TabBarItemCollection Items => this.items;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public TabBar.TabBarItem SelectedTab
    {
      get => this.selectedTab;
      set
      {
        if (value != null)
        {
          value.State = TabItemState.Selected;
        }
        else
        {
          TabBar.TabBarItem selectedTab = this.selectedTab;
          this.selectedTab = (TabBar.TabBarItem) null;
          this.items.ForEach((Action<TabBar.TabBarItem>) (x =>
          {
            if (x.State != TabItemState.Selected)
              return;
            x.State = TabItemState.Normal;
          }));
          this.OnSelectedTabChanged(new TabBar.SelectedTabChangedEventArgs(selectedTab, (TabBar.TabBarItem) null));
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int SelectedTabIndex
    {
      get => this.items.IndexOf(this.selectedTab);
      set
      {
        if (value < 0 || value >= this.items.Count)
          return;
        this.SelectedTab = this.items[value];
      }
    }

    [DefaultValue(true)]
    public bool ShowDropDown
    {
      get => this.showDropDown;
      set => this.SetValue<bool>(ref this.showDropDown, value);
    }

    [DefaultValue(null)]
    public Bitmap CloseImage
    {
      get => this.closeImage;
      set => this.SetValue<Bitmap>(ref this.closeImage, value);
    }

    [DefaultValue(0)]
    public int TabHeight
    {
      get => this.tabHeight;
      set => this.SetValue<int>(ref this.tabHeight, value);
    }

    [DefaultValue(2)]
    public int TopPadding
    {
      get => this.topPadding;
      set => this.SetValue<int>(ref this.topPadding, value);
    }

    [DefaultValue(4)]
    public int BottomPadding
    {
      get => this.bottomPadding;
      set => this.SetValue<int>(ref this.bottomPadding, value);
    }

    [DefaultValue(true)]
    public bool DrawBaseLine
    {
      get => this.drawBaseLine;
      set => this.SetValue<bool>(ref this.drawBaseLine, value);
    }

    [DefaultValue(0)]
    public int LeftIndent
    {
      get => this.leftIndent;
      set => this.SetValue<int>(ref this.leftIndent, value);
    }

    [DefaultValue(false)]
    public bool OwnerDrawnTooltips { get; set; }

    [DefaultValue(250)]
    public int MinimumTabWidth
    {
      get => this.minimumTabWidth;
      set => this.SetValue<int>(ref this.minimumTabWidth, value);
    }

    [DefaultValue(-1)]
    public int MarkerPosition
    {
      get => this.markerPosition;
      set => this.SetValue<int>(ref this.markerPosition, value);
    }

    [DefaultValue(false)]
    public bool DragDropReorder { get; set; }

    public void EnsureVisible(TabBar.TabBarItem item)
    {
      if (!this.items.Contains(item))
        return;
      Rectangle tabBounds = this.GetTabBounds(this.TabsRectangle);
      Rectangle bounds = item.Bounds;
      if (bounds.Left < tabBounds.Left)
        this.TabsOffset += tabBounds.Left - bounds.Left;
      if (bounds.Right <= tabBounds.Right)
        return;
      this.TabsOffset -= bounds.Right - tabBounds.Right;
    }

    public bool SelectTab(int tab, int offset, bool rollover)
    {
      tab = tab.Clamp(0, this.items.Count - 1);
      TabBar.TabBarItem[] array = this.items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (i => i.Visible)).ToArray<TabBar.TabBarItem>();
      if (array.Length == 0)
        return false;
      int n = ((IEnumerable<TabBar.TabBarItem>) array).FindIndex<TabBar.TabBarItem>((Predicate<TabBar.TabBarItem>) (t => t == this.items[tab]));
      if (n == -1)
        n = tab >= array.Length ? array.Length - 1 : 0;
      this.SelectedTab = array[rollover ? Numeric.Rollover(n, array.Length, offset) : n.Clamp(0, array.Length)];
      return true;
    }

    public bool SelectNextTab() => this.SelectNextTab(true);

    public bool SelectNextTab(bool rollOver) => this.SelectTab(this.SelectedTabIndex, 1, rollOver);

    public bool SelectPreviousTab() => this.SelectPreviousTab(true);

    public bool SelectPreviousTab(bool rollOver)
    {
      return this.SelectTab(this.SelectedTabIndex, -1, rollOver);
    }

    public void SelectFirstTab() => this.SelectTab(0, 0, false);

    public void SelectLastTab() => this.SelectTab(this.items.Count - 1, 0, false);

    private void scrollTimer_Tick(object sender, EventArgs e)
    {
      Rectangle tabsRectangle = this.TabsRectangle;
      System.Drawing.Point client = this.PointToClient(Cursor.Position);
      if (this.GetRightArrowBounds(tabsRectangle).Contains(client))
      {
        this.TabsOffset -= 20;
      }
      else
      {
        if (!this.GetLeftArrowBounds(tabsRectangle).Contains(client))
          return;
        this.TabsOffset += 20;
      }
    }

    private void toolTipTimer_Tick(object sender, EventArgs e)
    {
      this.toolTipTimer.Stop();
      this.ShowToolTip(true);
    }

    private void items_Changed(object sender, SmartListChangedEventArgs<TabBar.TabBarItem> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          e.Item.State = TabItemState.Normal;
          e.Item.Changed += new EventHandler(this.Item_Changed);
          e.Item.Selected += new CancelEventHandler(this.Item_Selected);
          this.ImageAnimate(e.Item, true);
          break;
        case SmartListAction.Remove:
          e.Item.Changed -= new EventHandler(this.Item_Changed);
          e.Item.Selected -= new CancelEventHandler(this.Item_Selected);
          if (this.selectedTab == e.Item)
            this.SelectedTab = (TabBar.TabBarItem) null;
          e.Item.InvokeRemoved();
          this.ImageAnimate(e.Item, false);
          break;
      }
      this.Invalidate();
    }

    private void Item_Selected(object sender, CancelEventArgs e)
    {
      TabBar.TabBarItem selectedTab = this.selectedTab;
      TabBar.TabBarItem newItem = sender as TabBar.TabBarItem;
      TabBar.SelectedTabChangedEventArgs e1 = new TabBar.SelectedTabChangedEventArgs(this.selectedTab, newItem);
      this.selectedTab = newItem;
      try
      {
        this.OnSelectedTabChanged(e1);
      }
      finally
      {
        this.selectedTab = selectedTab;
      }
      e.Cancel = e1.Cancel;
    }

    private void Item_Changed(object sender, EventArgs e)
    {
      TabBar.TabBarItem tbi = (TabBar.TabBarItem) sender;
      this.ImageAnimate(tbi, true);
      if (tbi.State == TabItemState.Selected)
      {
        this.selectedTab = tbi;
        this.items.ForEach((Action<TabBar.TabBarItem>) (x =>
        {
          if (x == tbi)
            return;
          x.State = TabItemState.Normal;
        }));
        this.EnsureVisible(tbi);
      }
      this.Invalidate();
    }

    private void toolTip_Popup(object sender, PopupEventArgs e)
    {
      if (!this.OwnerDrawnTooltips)
        return;
      if (this.toolTipItem == null || !this.toolTipItem.ShowToolTip())
      {
        e.Cancel = true;
      }
      else
      {
        System.Drawing.Size toolTipSize = this.toolTipItem.ToolTipSize;
        if (toolTipSize.IsEmpty)
          return;
        e.ToolTipSize = toolTipSize;
      }
    }

    private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
    {
      VisualStyleRenderer vr = (VisualStyleRenderer) null;
      VisualStyleElement normal = VisualStyleElement.ToolTip.Standard.Normal;
      if (VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(normal))
      {
        vr = new VisualStyleRenderer(normal);
        vr.DrawBackground((IDeviceContext) e.Graphics, e.Bounds);
      }
      else
      {
        e.DrawBackground();
        e.DrawBorder();
      }
      if (this.OwnerDrawnTooltips && this.toolTipItem != null && !this.toolTipItem.ToolTipSize.IsEmpty)
        this.toolTipItem.DrawTooltip(e.Graphics, e.Bounds);
      else
        TabBar.DrawTooltipText(vr, e);
    }

    private static void DrawTooltipText(VisualStyleRenderer vr, DrawToolTipEventArgs e)
    {
      if (vr == null)
      {
        e.DrawText();
      }
      else
      {
        using (FontDC dc = new FontDC(e.Graphics, e.Font))
        {
          Rectangle contentRectangle = vr.GetBackgroundContentRectangle((IDeviceContext) dc, e.Bounds);
          vr.DrawText((IDeviceContext) dc, contentRectangle, e.ToolTipText);
        }
      }
    }

    private static Color BorderColor
    {
      get
      {
        Color borderColor = SystemColors.AppWorkspace;
        if (Application.RenderWithVisualStyles)
          borderColor = new VisualStyleRenderer(VisualStyleElement.Tab.Pane.Normal).GetColor(ColorProperty.BorderColorHint);
        return borderColor;
      }
    }

    private void SetValue<T>(ref T old, T value)
    {
      if (object.Equals((object) old, (object) value))
        return;
      old = value;
      this.Invalidate();
    }

    private bool ShowArrows
    {
      get => this.showArrows;
      set
      {
        if (this.showArrows == value)
          return;
        this.showArrows = value;
        this.Invalidate();
      }
    }

    private TabBar.ItemState LeftArrowState
    {
      get => this.leftArrowState;
      set
      {
        if (this.leftArrowState == value)
          return;
        this.leftArrowState = value;
        this.Invalidate(this.GetLeftArrowBounds(this.TabsRectangle));
      }
    }

    private TabBar.ItemState RightArrowState
    {
      get => this.rightArrowState;
      set
      {
        if (this.rightArrowState == value)
          return;
        this.rightArrowState = value;
        this.Invalidate(this.GetRightArrowBounds(this.TabsRectangle));
      }
    }

    private TabBar.ItemState DropDownState
    {
      get => this.dropDownState;
      set
      {
        if (this.dropDownState == value)
          return;
        this.dropDownState = value;
        this.Invalidate(this.GetDropDownBounds(this.TabsRectangle));
      }
    }

    private int TabsOffset
    {
      get => this.tabsOffset;
      set
      {
        int num = value;
        int width = this.tabBounds.Width;
        if (num + this.tabsWidth < width)
          num = width - this.tabsWidth;
        if (num > 0)
          num = 0;
        if (this.tabsOffset == num)
          return;
        this.tabsOffset = num;
        this.Invalidate();
      }
    }

    private void ShowTabDropDown(System.Drawing.Point location)
    {
      ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
      bool flag = false;
      int num = this.items.Count<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (x => x.Visible));
      for (int index = 0; index < this.items.Count; ++index)
      {
        TabBar.TabBarItem tbi = this.items[index];
        if (tbi.ShowInDropDown && tbi.Visible)
        {
          Padding padding;
          if (!flag && index > 0)
          {
            padding = tbi.Padding;
            if (padding.Left != 0)
              contextMenuStrip.Items.Add((ToolStripItem) new ToolStripSeparator());
          }
          ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(tbi.Text, tbi.Image, (EventHandler) ((x, y) => this.SelectedTab = tbi));
          if (tbi.State == TabItemState.Selected)
            toolStripMenuItem.Checked = true;
          if (tbi.State == TabItemState.Disabled)
            toolStripMenuItem.Enabled = false;
          contextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem);
          flag = false;
          if (index < num - 1)
          {
            padding = tbi.Padding;
            if (padding.Right != 0)
            {
              contextMenuStrip.Items.Add((ToolStripItem) new ToolStripSeparator());
              flag = true;
            }
          }
        }
      }
      if (contextMenuStrip.Items.Count == 0)
        contextMenuStrip.Dispose();
      else
        contextMenuStrip.Show(this.PointToScreen(location), ToolStripDropDownDirection.BelowLeft);
    }

    private void DrawButton(Graphics gr, Rectangle rc, Image image, TabBar.ItemState state)
    {
      TabItemState tabItemState;
      switch (state)
      {
        case TabBar.ItemState.None:
          tabItemState = TabItemState.Normal;
          break;
        case TabBar.ItemState.Selected:
          tabItemState = TabItemState.Selected;
          break;
        default:
          tabItemState = TabItemState.Hot;
          break;
      }
      this.DrawTabItem(gr, rc, tabItemState, true);
      Rectangle rect = image.Size.Align(rc, System.Drawing.ContentAlignment.MiddleCenter);
      gr.DrawImage(image, rect);
    }

    private void DrawTabItem(
      Graphics gr,
      Rectangle rc,
      TabItemState tabItemState,
      bool buttonMode)
    {
      if (TabRenderer.IsSupported)
      {
        TabRenderer.DrawTabItem(gr, rc, tabItemState);
        if (!buttonMode)
          return;
        using (Pen pen = new Pen(TabBar.BorderColor))
          gr.DrawLine(pen, rc.Left, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
      }
      else
      {
        Border3DSide sides = Border3DSide.Left | Border3DSide.Top | Border3DSide.Right;
        Border3DStyle style = Border3DStyle.Raised;
        if (buttonMode)
        {
          if (tabItemState == TabItemState.Selected)
            style = Border3DStyle.Sunken;
          sides |= Border3DSide.Bottom;
        }
        using (Brush brush = (Brush) new SolidBrush(tabItemState == TabItemState.Selected ? SystemColors.ControlLightLight : this.BackColor))
          gr.FillRectangle(brush, rc);
        ControlPaint.DrawBorder3D(gr, rc, style, sides);
      }
    }

    private void UpdateTabAndButtonStates(MouseEventArgs e)
    {
      Rectangle tabsRectangle = this.TabsRectangle;
      System.Drawing.Point pt = e.Location;
      this.RightArrowState = TabBar.GetItemState(this.GetRightArrowBounds(tabsRectangle), pt, e.Button);
      this.LeftArrowState = TabBar.GetItemState(this.GetLeftArrowBounds(tabsRectangle), pt, e.Button);
      this.DropDownState = TabBar.GetItemState(this.GetDropDownBounds(tabsRectangle), pt, e.Button);
      TabBar.TabBarItem tbi = this.items.Find((Predicate<TabBar.TabBarItem>) (x => x.Bounds.Contains(pt)));
      if (tbi != null && this.GetTabBounds(tabsRectangle).Contains(pt))
      {
        this.items.ForEach((Action<TabBar.TabBarItem>) (x =>
        {
          if (x.State != TabItemState.Selected)
            x.State = x == tbi ? TabItemState.Hot : TabItemState.Normal;
          x.CloseButtonHot = false;
        }));
        if (tbi.State != TabItemState.Selected)
          tbi.State = TabItemState.Hot;
        if (!tbi.CloseBounds.Contains(pt))
          return;
        tbi.CloseButtonHot = true;
      }
      else
        this.items.ForEach((Action<TabBar.TabBarItem>) (x =>
        {
          if (x.State != TabItemState.Selected)
            x.State = TabItemState.Normal;
          x.CloseButtonHot = false;
        }));
    }

    private void ShowToolTip(bool always, TabBar.TabBarItem item)
    {
      System.Drawing.Point pt = this.PointToClient(Cursor.Position);
      item = item ?? this.items.Find((Predicate<TabBar.TabBarItem>) (x => x.Bounds.Contains(pt)));
      if (item == null)
      {
        this.HideToolTip();
      }
      else
      {
        if (item == this.toolTipItem || !always && !this.toolTip.Active)
          return;
        this.toolTipItem = item;
        System.Windows.Forms.ToolTip toolTip = this.toolTip;
        string toolTipText = item.ToolTipText;
        Rectangle bounds = item.Bounds;
        int left = bounds.Left;
        bounds = item.Bounds;
        int bottom = bounds.Bottom;
        System.Drawing.Point point = new System.Drawing.Point(left, bottom);
        toolTip.Show(toolTipText, (IWin32Window) this, point);
      }
    }

    private void ShowToolTip(bool always) => this.ShowToolTip(always, (TabBar.TabBarItem) null);

    private void HideToolTip()
    {
      this.toolTip.Hide((IWin32Window) this);
      this.toolTipItem = (TabBar.TabBarItem) null;
    }

    private void ImageAnimate(TabBar.TabBarItem tbi, bool enable)
    {
      using (ItemMonitor.Lock((object) this.animatedImages))
      {
        Image image;
        if (this.animatedImages.TryGetValue(tbi, out image))
        {
          if (enable && tbi.Image == image)
            return;
          ImageAnimator.StopAnimate(image, new EventHandler(this.AnimationHandler));
          this.animatedImages.Remove(tbi);
        }
        if (!enable || tbi.Image == null || !ImageAnimator.CanAnimate(tbi.Image))
          return;
        ImageAnimator.Animate(tbi.Image, new EventHandler(this.AnimationHandler));
        this.animatedImages[tbi] = tbi.Image;
      }
    }

    private void AnimationHandler(object sender, EventArgs e)
    {
      Image img = sender as Image;
      ImageAnimator.UpdateFrames(img);
      using (ItemMonitor.Lock((object) this.animatedImages))
      {
        TabBar.TabBarItem tabBarItem = this.animatedImages.Keys.FirstOrDefault<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (tbi => tbi.Image == img));
        if (tabBarItem == null)
          return;
        this.Invalidate(tabBarItem.Bounds);
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      try
      {
        Rectangle clientRectangle = this.ClientRectangle;
        Rectangle rectangle1 = clientRectangle;
        this.DrawBackground(e.Graphics, rectangle1);
        Rectangle rectangle2 = rectangle1.Pad(0, this.topPadding, bottom: this.bottomPadding);
        if (this.drawBaseLine)
        {
          using (Pen pen = new Pen(TabBar.BorderColor))
          {
            if (this.BottomPadding == 0)
              e.Graphics.DrawLine(pen, rectangle2.Left, rectangle2.Bottom - 1, rectangle2.Width, rectangle2.Bottom - 1);
            else
              e.Graphics.DrawRectangle(pen, rectangle2.Left, rectangle2.Bottom - 1, rectangle2.Width - 1, clientRectangle.Bottom - rectangle2.Bottom);
          }
        }
        Rectangle rectangle3 = this.TabsRectangle.Pad(0, this.topPadding, bottom: this.bottomPadding);
        this.tabBounds = this.GetTabBounds(rectangle3);
        this.tabsWidth = this.LayoutTabs(this.TabsOffset, this.tabBounds, 0);
        if (this.tabsWidth > this.tabBounds.Width)
          this.tabsWidth = this.LayoutTabs(this.TabsOffset, this.tabBounds, this.tabsWidth - this.tabBounds.Width);
        this.ShowArrows = this.tabsWidth > this.tabBounds.Width;
        using (e.Graphics.SaveState())
        {
          e.Graphics.IntersectClip(rectangle3);
          using (e.Graphics.SaveState())
          {
            e.Graphics.IntersectClip(this.tabBounds.Pad(0, 0, -1));
            this.items.ForEach((Action<TabBar.TabBarItem>) (x => this.DrawTab(e.Graphics, x)));
            if (this.MarkerPosition >= 0)
            {
              TabBar.TabBarItem[] array = this.items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (x => x.Visible)).ToArray<TabBar.TabBarItem>();
              if (array.Length != 0)
              {
                int num = this.MarkerPosition < array.Length ? array[this.MarkerPosition].Bounds.Left : array[array.Length - 1].Bounds.Right;
                this.DrawMarker(e.Graphics, new Rectangle(num - 2, rectangle3.Top, 4, rectangle3.Height));
              }
            }
          }
          this.DrawArrows(e.Graphics, rectangle3);
          this.DrawDropDown(e.Graphics, rectangle3);
        }
      }
      catch (Exception ex)
      {
        this.Invalidate();
      }
    }

    private static TabBar.ItemState GetItemState(Rectangle rc, System.Drawing.Point pt, MouseButtons mb)
    {
      if (!rc.Contains(pt))
        return TabBar.ItemState.None;
      return (mb & MouseButtons.Left) == MouseButtons.None ? TabBar.ItemState.Hot : TabBar.ItemState.Selected;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      int index = this.items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (x => x.Visible)).FindIndex<TabBar.TabBarItem>((Predicate<TabBar.TabBarItem>) (x => x.Bounds.Contains(e.Location)));
      if (this.DragDropReorder && this.inDrag < 0 && !this.clickPoint.IsEmpty && this.items.Count<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (x => x.Visible)) > 1)
      {
        System.Drawing.Point point = e.Location.Subtract(this.clickPoint);
        if (Math.Abs(point.X) > SystemInformation.DragSize.Width / 2 || Math.Abs(point.Y) > SystemInformation.DragSize.Height / 2)
          this.inDrag = index;
      }
      if (this.inDrag >= 0)
      {
        if (index > this.inDrag)
          ++index;
        this.MarkerPosition = index;
      }
      else
      {
        this.UpdateTabAndButtonStates(e);
        if (this.toolTipItem != null)
        {
          this.ShowToolTip(false);
        }
        else
        {
          if (e.Location.Equals(this.toolTipTimer.Tag))
            return;
          this.toolTipTimer.Stop();
          this.toolTipTimer.Start();
          this.toolTipTimer.Tag = (object) e.Location;
        }
      }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      this.items.ForEach((Action<TabBar.TabBarItem>) (x =>
      {
        if (x.State != TabItemState.Selected)
          x.State = TabItemState.Normal;
        x.CloseButtonHot = false;
      }));
      this.RightArrowState = this.LeftArrowState = this.DropDownState = TabBar.ItemState.None;
      this.HideToolTip();
      this.toolTipTimer.Stop();
      this.toolTipTimer.Tag = (object) null;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.scrollTimer.Enabled = true;
      this.UpdateTabAndButtonStates(e);
      this.clickPoint = e.Location;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      this.clickPoint = System.Drawing.Point.Empty;
      this.scrollTimer.Enabled = false;
      if (this.inDrag >= 0)
      {
        TabBar.TabBarItem[] array = this.items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (x => x.Visible)).ToArray<TabBar.TabBarItem>();
        if (this.inDrag != this.MarkerPosition && this.MarkerPosition >= 0 && this.MarkerPosition <= array.Length)
        {
          TabBar.TabBarItem tabBarItem = array[this.inDrag];
          int newIndex = this.MarkerPosition < array.Length ? this.items.IndexOf(array[this.MarkerPosition]) : this.items.Count;
          this.items.Move(this.items.IndexOf(tabBarItem), newIndex);
        }
        this.MarkerPosition = -1;
        this.inDrag = -1;
      }
      else
      {
        this.UpdateTabAndButtonStates(e);
        this.Focus();
        Rectangle tabsRectangle = this.TabsRectangle;
        System.Drawing.Point pt = this.PointToClient(Cursor.Position);
        Rectangle dropDownBounds = this.GetDropDownBounds(tabsRectangle);
        if (dropDownBounds.Contains(pt))
        {
          this.ShowTabDropDown(new System.Drawing.Point(dropDownBounds.Right, dropDownBounds.Bottom));
        }
        else
        {
          Rectangle rectangle = this.GetTabBounds(tabsRectangle);
          if (!rectangle.Contains(pt))
            return;
          TabBar.TabBarItem tabBarItem = this.items.Find((Predicate<TabBar.TabBarItem>) (x => x.Visible && x.Bounds.Contains(pt)));
          if (tabBarItem == null)
            return;
          if ((e.Button & MouseButtons.Left) != MouseButtons.None)
          {
            rectangle = tabBarItem.CloseBounds;
            if (rectangle.Contains(pt))
            {
              tabBarItem.InvokeCloseClick();
              return;
            }
            if (tabBarItem.InvokeCaptionClick())
              return;
            tabBarItem.State = TabItemState.Selected;
            tabBarItem.InvokeClick();
          }
          if ((e.Button & MouseButtons.Right) != MouseButtons.None && tabBarItem.ContextMenu != null)
            tabBarItem.ContextMenu.Show(this.PointToScreen(e.Location));
          if ((e.Button & MouseButtons.Middle) == MouseButtons.None)
            return;
          tabBarItem.InvokeCloseClick();
        }
      }
    }

    protected override bool IsInputKey(Keys keyData)
    {
      switch (keyData)
      {
        case Keys.End:
        case Keys.Home:
        case Keys.Left:
        case Keys.Right:
          return true;
        default:
          return base.IsInputKey(keyData);
      }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.End:
          this.SelectLastTab();
          e.Handled = true;
          break;
        case Keys.Home:
          this.SelectFirstTab();
          e.Handled = true;
          break;
        case Keys.Left:
          this.SelectPreviousTab();
          e.Handled = true;
          break;
        case Keys.Right:
          this.SelectNextTab();
          e.Handled = true;
          break;
      }
      base.OnKeyDown(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      base.OnMouseWheel(e);
      if (e.Delta > 0)
        this.SelectPreviousTab();
      else
        this.SelectNextTab();
    }

    protected override void OnEnter(EventArgs e)
    {
      base.OnEnter(e);
      this.Invalidate();
    }

    protected override void OnLeave(EventArgs e)
    {
      base.OnLeave(e);
      this.Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      this.TabsOffset = 0;
    }

    public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
    {
      if (!this.AutoSize)
        return base.GetPreferredSize(proposedSize);
      proposedSize.Height = this.Font.Height + FormUtility.ScaleDpiY(12) + this.topPadding + this.bottomPadding;
      return proposedSize;
    }

    [Browsable(true)]
    public override bool AutoSize
    {
      get => base.AutoSize;
      set => base.AutoSize = value;
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
      base.OnLayout(e);
      Rectangle rectangle = this.ClientRectangle.Pad(this.MinimumTabWidth, 0, bottom: this.bottomPadding + 1);
      IEnumerable<ToolStrip> source = this.Controls.OfType<ToolStrip>();
      if (source.Count<ToolStrip>() == 0)
        return;
      int right = rectangle.Right;
      System.Drawing.Size size = rectangle.Size;
      foreach (ToolStrip toolStrip in source)
      {
        if (toolStrip.AutoSize)
        {
          System.Drawing.Size preferredSize = toolStrip.GetPreferredSize(size) with
          {
            Height = rectangle.Height - 1
          };
          toolStrip.Size = preferredSize;
        }
        if (toolStrip.Dock == DockStyle.Right)
        {
          right -= toolStrip.Width;
          toolStrip.Location = new System.Drawing.Point(right, 1);
        }
        else
          toolStrip.Location = new System.Drawing.Point(0, 1);
        size.Width -= toolStrip.Width;
      }
      this.Invalidate();
    }

    protected override void OnDragOver(DragEventArgs drgevent)
    {
      base.OnDragOver(drgevent);
      System.Drawing.Point pt = this.PointToClient(new System.Drawing.Point(drgevent.X, drgevent.Y));
      TabBar.TabBarItem tabBarItem = this.items.Find((Predicate<TabBar.TabBarItem>) (x => x.Bounds.Contains(pt)));
      if (tabBarItem == null)
        return;
      this.SelectedTab = tabBarItem;
    }

    private Rectangle TabsRectangle
    {
      get
      {
        if (this.Controls.Count == 0)
          return this.ClientRectangle;
        int left = this.Controls.Cast<Control>().Max<Control>((Func<Control, int>) (c => c.Dock != DockStyle.Left ? 0 : c.Right));
        int right = this.Controls.Cast<Control>().Min<Control>((Func<Control, int>) (c => c.Dock != DockStyle.Right ? 0 : c.Left));
        if (left != 0)
          left += 20;
        if (right != 0)
          right -= 20;
        return Rectangle.FromLTRB(left, 0, right, this.ClientRectangle.Bottom);
      }
    }

    private Rectangle GetTabBounds(Rectangle rc, bool includeArrows = true)
    {
      if (this.tabHeight > FormUtility.ScaleDpiY(8))
        rc = rc.Pad(0, rc.Height - this.tabHeight);
      if (this.ShowArrows & includeArrows)
        rc = rc.Pad(TabBar.arrowWidth, 0, TabBar.arrowWidth);
      if (this.showDropDown)
        rc = rc.Pad(0, 0, TabBar.dropDownWidth);
      return rc;
    }

    private Rectangle GetLeftArrowBounds(Rectangle rc)
    {
      return !this.ShowArrows ? Rectangle.Empty : new Rectangle(rc.Left, rc.Top, TabBar.arrowWidth, rc.Height);
    }

    private Rectangle GetRightArrowBounds(Rectangle rc)
    {
      return !this.ShowArrows ? Rectangle.Empty : new Rectangle(rc.Right - this.GetDropDownBounds(rc).Width + 1 - TabBar.arrowWidth, rc.Top, TabBar.arrowWidth, rc.Height);
    }

    private Rectangle GetDropDownBounds(Rectangle rc)
    {
      return !this.ShowArrows || !this.ShowDropDown || !this.items.Exists((Predicate<TabBar.TabBarItem>) (x => x.ShowInDropDown)) ? Rectangle.Empty : new Rectangle(rc.Right - TabBar.dropDownWidth, rc.Top, TabBar.dropDownWidth, rc.Height);
    }

    private int LayoutTabs(int offset, Rectangle rc, int decreaseSize)
    {
      int num1 = 0;
      int num2 = rc.Left + offset;
      int num3 = num2 + this.leftIndent;
      float num4 = 1f;
      if (decreaseSize > 0)
      {
        int num5 = this.items.Where<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (tbi => tbi.AdjustWidth && tbi.Bounds.Width > FormUtility.ScaleDpiX(tbi.MinimumWidth))).Sum<TabBar.TabBarItem>((Func<TabBar.TabBarItem, int>) (tbi => tbi.Bounds.Width));
        num4 = num5 <= 0 ? 0.0f : Math.Max(0.0f, (float) (num5 - (decreaseSize + 10))) / (float) num5;
      }
      foreach (TabBar.TabBarItem tabBarItem1 in (SmartList<TabBar.TabBarItem>) this.items)
      {
        if (tabBarItem1.Visible)
        {
          int num6 = num3;
          Padding padding = tabBarItem1.Padding;
          int left = padding.Left;
          int x1 = num6 + left;
          System.Drawing.Size size;
          int num7;
          if (tabBarItem1.ShowText)
          {
            size = TextRenderer.MeasureText(tabBarItem1.Text, tabBarItem1.GetFont(this.Font), new System.Drawing.Size(int.MaxValue, int.MaxValue), tabBarItem1.TextFormat);
            num7 = size.Width;
          }
          else
            num7 = 0;
          int num8 = num7 + 4 + 2;
          size = tabBarItem1.ImageSize;
          int num9 = FormUtility.ScaleDpiX(size.Width);
          int width1 = num8 + num9;
          if (tabBarItem1.CanClose && this.closeImage != null)
            width1 += FormUtility.ScaleDpiX(this.closeImage.Width) + 4;
          if (tabBarItem1.AdjustWidth)
          {
            width1 = (int) ((double) width1 * (double) num4);
            if (width1 < FormUtility.ScaleDpiX(tabBarItem1.MinimumWidth))
              width1 = FormUtility.ScaleDpiX(tabBarItem1.MinimumWidth);
          }
          tabBarItem1.Bounds = new Rectangle(x1, rc.Top, width1, rc.Height + 1);
          if (tabBarItem1.State != TabItemState.Selected)
            tabBarItem1.Bounds = tabBarItem1.Bounds.Pad(0, 2, bottom: this.drawBaseLine ? 2 : 0);
          Rectangle bounds;
          if (tabBarItem1.CanClose && this.closeImage != null && (tabBarItem1.State == TabItemState.Hot || tabBarItem1.State == TabItemState.Selected))
          {
            TabBar.TabBarItem tabBarItem2 = tabBarItem1;
            bounds = tabBarItem1.Bounds;
            int x2 = bounds.Right - FormUtility.ScaleDpiX(this.closeImage.Width) - 2;
            bounds = tabBarItem1.Bounds;
            int y = bounds.Top + 2;
            int width2 = FormUtility.ScaleDpiX(this.closeImage.Width);
            int height = FormUtility.ScaleDpiY(this.closeImage.Height);
            Rectangle rectangle = new Rectangle(x2, y, width2, height);
            tabBarItem2.CloseBounds = rectangle;
          }
          else
            tabBarItem1.CloseBounds = Rectangle.Empty;
          int num10 = x1;
          bounds = tabBarItem1.Bounds;
          int num11 = bounds.Width - 1;
          int num12 = num10 + num11;
          num1 = num12 - num2;
          int num13 = num12;
          padding = tabBarItem1.Padding;
          int right = padding.Right;
          num3 = num13 + right;
        }
      }
      return num1;
    }

    protected virtual void DrawBackground(Graphics gr, Rectangle bounds)
    {
      using (Brush brush = (Brush) new SolidBrush(this.BackColor))
        gr.FillRectangle(brush, bounds);
    }

    protected virtual void DrawTab(Graphics gr, TabBar.TabBarItem item)
    {
      if (!item.Visible)
        return;
      Rectangle bounds1 = item.Bounds;
      this.DrawTabItem(gr, bounds1, item.State, false);
      Rectangle rectangle = bounds1.Pad(2, 2, 2);
      if (this.Focused && item.State == TabItemState.Selected)
        ControlPaint.DrawFocusRectangle(gr, rectangle);
      Rectangle rc1 = rectangle.Pad(1);
      if (item.Image != null)
      {
        try
        {
          gr.DrawImage(item.Image, rc1.Left, rc1.Top, FormUtility.ScaleDpiX(item.ImageSize.Width), FormUtility.ScaleDpiY(item.ImageSize.Height));
        }
        catch (Exception ex)
        {
        }
        rc1 = rc1.Pad(FormUtility.ScaleDpiX(item.ImageSize.Width), 0);
      }
      Rectangle rc2 = rc1;
      Rectangle closeBounds = item.CloseBounds;
      int width = closeBounds.Width;
      Rectangle bounds2 = rc2.Pad(0, 2, width);
      TextRenderer.DrawText((IDeviceContext) gr, item.Text, item.GetFont(this.Font), bounds2, this.ForeColor, item.TextFormat);
      closeBounds = item.CloseBounds;
      if (closeBounds.IsEmpty)
        return;
      gr.DrawImage(this.closeImage, item.CloseBounds, new BitmapAdjustment(item.CloseButtonHot ? 0.0f : -0.8f));
    }

    protected virtual void DrawArrows(Graphics gr, Rectangle rc)
    {
      if (!this.ShowArrows)
        return;
      this.DrawButton(gr, this.GetLeftArrowBounds(rc), (Image) TabBar.arrowLeft, this.LeftArrowState);
      this.DrawButton(gr, this.GetRightArrowBounds(rc), (Image) TabBar.arrowRight, this.RightArrowState);
    }

    protected virtual void DrawDropDown(Graphics gr, Rectangle rc)
    {
      Rectangle dropDownBounds = this.GetDropDownBounds(rc);
      if (dropDownBounds.IsEmpty)
        return;
      this.DrawButton(gr, this.GetDropDownBounds(dropDownBounds), (Image) TabBar.arrowDown, this.DropDownState);
    }

    protected virtual void DrawMarker(Graphics gr, Rectangle rc)
    {
      gr.DrawImage((Image) TabBar.insertArrow, rc.Left + rc.Width / 2 - TabBar.insertArrow.Width / 2, rc.Top);
    }

    public event EventHandler<TabBar.SelectedTabChangedEventArgs> SelectedTabChanged;

    protected virtual void OnSelectedTabChanged(TabBar.SelectedTabChangedEventArgs e)
    {
      if (this.SelectedTabChanged == null)
        return;
      this.SelectedTabChanged((object) this, e);
    }

    private void InitWindowsTouch()
    {
      try
      {
        this.gestureHandler = Factory.CreateHandler<GestureHandler>((Control) this);
        this.gestureHandler.DisableGutter = true;
        this.gestureHandler.Pan += new EventHandler<GestureEventArgs>(this.gestureHandler_Pan);
      }
      catch (Exception ex)
      {
      }
    }

    private void gestureHandler_Pan(object sender, GestureEventArgs e)
    {
      this.TabsOffset += e.PanTranslation.Width;
    }

    private enum ItemState
    {
      None,
      Hot,
      Selected,
    }

    public class TabBarItem
    {
      private string text;
      private Image image;
      private bool enabled = true;
      private TabItemState state = TabItemState.Normal;
      private Padding padding = Padding.Empty;
      private bool showInDropDown = true;
      private bool canClose;
      private bool visible = true;
      private bool fontBold;
      private TextFormatFlags textFormat = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping;
      private int minimumWidth = 60;
      private bool adjustWidth = true;
      private bool showText = true;
      private bool closeButtonHot;

      public TabBarItem()
      {
      }

      public TabBarItem(string text) => this.text = text;

      [DefaultValue(null)]
      public string Name { get; set; }

      [DefaultValue(null)]
      public string Text
      {
        get => this.text;
        set => this.SetValue<string>(ref this.text, value);
      }

      [DefaultValue(null)]
      public virtual string ToolTipText { get; set; }

      [DefaultValue(typeof (System.Drawing.Size), "Empty")]
      public virtual System.Drawing.Size ToolTipSize { get; set; }

      [DefaultValue(null)]
      public object Tag { get; set; }

      [DefaultValue(null)]
      public Image Image
      {
        get => this.image;
        set
        {
          this.SetValue<Image>(ref this.image, value);
          this.ImageSize = this.image == null ? System.Drawing.Size.Empty : this.image.Size;
        }
      }

      [Browsable(false)]
      public System.Drawing.Size ImageSize { get; private set; }

      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public Rectangle Bounds { get; internal set; }

      [DefaultValue(true)]
      public bool Enabled
      {
        get => this.enabled;
        set => this.SetValue<bool>(ref this.enabled, value);
      }

      [DefaultValue(TabItemState.Normal)]
      public TabItemState State
      {
        get => this.state;
        set
        {
          if (this.state == value)
            return;
          if (value == TabItemState.Selected)
          {
            TabItemState state = this.state;
            this.state = value;
            CancelEventArgs e = new CancelEventArgs();
            this.OnSelected(e);
            if (e.Cancel)
            {
              this.state = state;
              return;
            }
          }
          this.state = value;
          this.OnChanged();
        }
      }

      [DefaultValue(typeof (Padding), "Empty")]
      public Padding Padding
      {
        get => this.padding;
        set => this.SetValue<Padding>(ref this.padding, value);
      }

      [DefaultValue(true)]
      public bool ShowInDropDown
      {
        get => this.showInDropDown;
        set => this.showInDropDown = value;
      }

      [DefaultValue(false)]
      public bool CanClose
      {
        get => this.canClose;
        set => this.SetValue<bool>(ref this.canClose, value);
      }

      [DefaultValue(null)]
      public ContextMenuStrip ContextMenu { get; set; }

      [DefaultValue(true)]
      public bool Visible
      {
        get => this.visible;
        set => this.SetValue<bool>(ref this.visible, value);
      }

      [DefaultValue(false)]
      public bool FontBold
      {
        get => this.fontBold;
        set => this.SetValue<bool>(ref this.fontBold, value);
      }

      [DefaultValue(TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine)]
      public TextFormatFlags TextFormat
      {
        get => this.textFormat;
        set => this.SetValue<TextFormatFlags>(ref this.textFormat, value);
      }

      [DefaultValue(60)]
      public int MinimumWidth
      {
        get => this.minimumWidth;
        set => this.SetValue<int>(ref this.minimumWidth, value);
      }

      [DefaultValue(true)]
      public bool AdjustWidth
      {
        get => this.adjustWidth;
        set => this.SetValue<bool>(ref this.adjustWidth, value);
      }

      [DefaultValue(true)]
      public bool ShowText
      {
        get => this.showText;
        set => this.SetValue<bool>(ref this.showText, value);
      }

      public bool IsSelected => this.State == TabItemState.Selected;

      internal Rectangle CloseBounds { get; set; }

      internal bool CloseButtonHot
      {
        get => this.closeButtonHot;
        set => this.SetValue<bool>(ref this.closeButtonHot, value);
      }

      internal Font GetFont(Font font) => !this.FontBold ? font : FC.Get(font, FontStyle.Bold);

      public event EventHandler Changed;

      public event CancelEventHandler Selected;

      public event CancelEventHandler CaptionClick;

      public event EventHandler Click;

      public event EventHandler CloseClick;

      public event EventHandler Removed;

      protected virtual void OnChanged()
      {
        if (this.Changed == null)
          return;
        this.Changed((object) this, EventArgs.Empty);
      }

      protected virtual void OnClick()
      {
        if (this.Click == null)
          return;
        this.Click((object) this, EventArgs.Empty);
      }

      protected virtual void OnCaptionClick(CancelEventArgs e)
      {
        if (this.CaptionClick == null)
          return;
        this.CaptionClick((object) this, e);
      }

      protected virtual void OnCloseClick()
      {
        if (this.CloseClick == null)
          return;
        this.CloseClick((object) this, EventArgs.Empty);
      }

      protected virtual void OnSelected(CancelEventArgs e)
      {
        if (this.Selected == null)
          return;
        this.Selected((object) this, e);
      }

      protected virtual void OnRemoved()
      {
        if (this.Removed == null)
          return;
        this.Removed((object) this, EventArgs.Empty);
      }

      private bool SetValue<T>(ref T value, T newValue)
      {
        if (object.Equals((object) value, (object) newValue))
          return false;
        value = newValue;
        this.OnChanged();
        return true;
      }

      public bool InvokeCaptionClick()
      {
        CancelEventArgs e = new CancelEventArgs();
        this.OnCaptionClick(e);
        return e.Cancel;
      }

      public void InvokeClick() => this.OnClick();

      public void InvokeCloseClick() => this.OnCloseClick();

      public void InvokeRemoved() => this.OnRemoved();

      public virtual bool ShowToolTip() => true;

      public virtual void DrawTooltip(Graphics gr, Rectangle rc)
      {
      }
    }

    public class TabBarItemCollection : SmartList<TabBar.TabBarItem>
    {
      public TabBarItemCollection()
        : base(SmartListOptions.DisableOnSet | SmartListOptions.ClearWithRemove)
      {
      }

      public TabBar.TabBarItem this[string name]
      {
        get => this.Find((Predicate<TabBar.TabBarItem>) (x => x.Name == name));
      }
    }

    public class SelectedTabChangedEventArgs : CancelEventArgs
    {
      private readonly TabBar.TabBarItem newItem;
      private readonly TabBar.TabBarItem oldItem;

      public SelectedTabChangedEventArgs(TabBar.TabBarItem oldItem, TabBar.TabBarItem newItem)
      {
        this.newItem = newItem;
        this.oldItem = oldItem;
      }

      public TabBar.TabBarItem NewItem => this.newItem;

      public TabBar.TabBarItem OldItem => this.oldItem;
    }

    public class ToolStripHost : ToolStripControlHost
    {
      private bool spring = true;

      public ToolStripHost(TabBar tabBar)
        : base((Control) tabBar)
      {
        this.AutoSize = true;
        this.Overflow = ToolStripItemOverflow.Never;
        tabBar.BackColor = Color.Transparent;
        tabBar.TabHeight = 24;
        tabBar.DrawBaseLine = false;
      }

      public TabBar TabBar => this.Control as TabBar;

      public bool Spring
      {
        get => this.spring;
        set
        {
          this.spring = value;
          if (this.Owner == null)
            return;
          this.Owner.PerformLayout();
        }
      }

      public override System.Drawing.Size GetPreferredSize(System.Drawing.Size constrainingSize)
      {
        if (!this.Spring || this.IsOnOverflow || this.Owner.Orientation == Orientation.Vertical)
          return this.DefaultSize;
        int num1 = this.Owner.DisplayRectangle.Width;
        if (this.Owner.OverflowButton.Visible)
          num1 = num1 - this.Owner.OverflowButton.Width - this.Owner.OverflowButton.Margin.Horizontal;
        int num2 = 0;
        foreach (ToolStripItem toolStripItem in (ArrangedElementCollection) this.Owner.Items)
        {
          if (!toolStripItem.IsOnOverflow)
          {
            if (toolStripItem is TabBar.ToolStripHost)
            {
              ++num2;
              num1 -= toolStripItem.Margin.Horizontal;
            }
            else
              num1 -= toolStripItem.Width + toolStripItem.Margin.Horizontal;
          }
        }
        if (num2 > 1)
          num1 /= num2;
        System.Drawing.Size preferredSize = base.GetPreferredSize(constrainingSize);
        if (this.TabBar.TabHeight != 0)
          preferredSize.Height = this.TabBar.TabHeight;
        preferredSize.Width = num1;
        return preferredSize;
      }
    }

    public class TabBarToolStripRenderer : ExtendedToolStripRenderer
    {
      public TabBarToolStripRenderer(ToolStripRenderer renderer)
        : base(renderer)
      {
      }

      protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
      {
        base.OnRenderToolStripBorder(e);
        Rectangle affectedBounds = e.AffectedBounds;
        using (Pen pen = new Pen(TabBar.BorderColor))
          e.Graphics.DrawLine(pen, affectedBounds.Left, affectedBounds.Bottom - 2, affectedBounds.Right, affectedBounds.Bottom - 2);
      }

      protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
      {
        if (!Application.RenderWithVisualStyles || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.Tab.Body.Normal))
          base.OnRenderToolStripBackground(e);
        else
          new VisualStyleRenderer(VisualStyleElement.Tab.Body.Normal).DrawBackground((IDeviceContext) e.Graphics, e.AffectedBounds);
      }
    }
  }
}

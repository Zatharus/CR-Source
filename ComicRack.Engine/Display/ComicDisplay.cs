// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.ComicDisplay
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Runtime;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display
{
  public class ComicDisplay : 
    DisposableObject,
    IComicDisplay,
    IComicDisplayConfig,
    IEditBookmark,
    IGetBookList,
    IRefreshDisplay,
    IEditPage
  {
    public const int DefaultPageWallTicks = 300;
    public const int AnimationTime = 300;
    private readonly IComicDisplay display;
    private ContainerControl control;
    private float scrollLines = 1f;
    private KeySearch pageKeys;
    private long lastPartNavigation;
    private long transitionStart;
    private ComicDisplay.WallState wallState;
    private long lastPaging;
    private bool fullScreen;
    private Rectangle orgRect = Rectangle.Empty;
    private FormBorderStyle borderStyleOld = FormBorderStyle.Fixed3D;
    private FormWindowState oldFormState;
    private bool hideCursorFullScreen = true;
    private KeyboardShortcuts keyboardMap = new KeyboardShortcuts();
    private long pageWallTicks = 300;
    private bool scrollingDoesBrowse = true;
    private bool autoScrolling = true;
    private ComicPageType pageFilter = ComicPageType.All;
    private volatile bool mouseClickEnabled = true;
    private volatile bool resetZoomOnPageChange;
    private volatile bool zoomInOutOnPageChange;
    private float oldZoom;
    private float mouseWheelSpeed = 2f;

    public ComicDisplay(IComicDisplay display)
    {
      this.display = display;
      this.display.BookChanged += new EventHandler(this.display_BookChanged);
      this.display.PageChange += new EventHandler<BookPageEventArgs>(this.display_PageChange);
      this.display.PageChanged += new EventHandler<BookPageEventArgs>(this.display_PageChanged);
      this.display.Browse += new EventHandler<BrowseEventArgs>(this.display_Browse);
      this.display.PreviewGesture += new EventHandler<GestureEventArgs>(this.display_PreviewGesture);
      this.display.Gesture += new EventHandler<GestureEventArgs>(this.display_Gesture);
      this.control = display as ContainerControl;
      if (this.control != null)
      {
        this.control.KeyDown += new KeyEventHandler(this.display_KeyDown);
        this.control.MouseDown += new MouseEventHandler(this.display_MouseDown);
        this.control.KeyUp += new KeyEventHandler(this.control_KeyUp);
        this.control.MouseWheel += new MouseEventHandler(this.display_MouseWheel);
      }
      if (display is IMouseHWheel mouseHwheel)
        mouseHwheel.MouseHWheel += new MouseEventHandler(this.display_MouseHWheel);
      int result;
      this.pageKeys = new KeySearch((Func<string, bool>) (s => this.Book != null && int.TryParse(s, out result) && this.Book.Navigate(result - 1, PageSeekOrigin.Beginning)))
      {
        SearchDelay = 1000
      };
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.display.BookChanged -= new EventHandler(this.display_BookChanged);
        this.display.PageChange -= new EventHandler<BookPageEventArgs>(this.display_PageChange);
        this.display.PageChanged -= new EventHandler<BookPageEventArgs>(this.display_PageChanged);
        this.display.Browse -= new EventHandler<BrowseEventArgs>(this.display_Browse);
        this.display.Gesture -= new EventHandler<GestureEventArgs>(this.display_Gesture);
        this.control = this.display as ContainerControl;
        if (this.control != null)
        {
          this.control.KeyDown -= new KeyEventHandler(this.display_KeyDown);
          this.control.MouseDown -= new MouseEventHandler(this.display_MouseDown);
          this.control.KeyUp -= new KeyEventHandler(this.control_KeyUp);
          this.control.MouseWheel -= new MouseEventHandler(this.display_MouseWheel);
        }
        if (this.display is IMouseHWheel display)
          display.MouseHWheel -= new MouseEventHandler(this.display_MouseHWheel);
        this.pageKeys.Dispose();
      }
      base.Dispose(disposing);
    }

    public event EventHandler FirstPageReached;

    public event EventHandler LastPageReached;

    public event EventHandler FullScreenChanged;

    protected virtual void OnFullScreenChanged()
    {
      if (this.FullScreenChanged == null)
        return;
      this.FullScreenChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnFirstPageReached()
    {
      if (this.FirstPageReached == null)
        return;
      this.FirstPageReached((object) this, EventArgs.Empty);
    }

    protected virtual void OnLastPageReached()
    {
      if (this.LastPageReached == null)
        return;
      this.LastPageReached((object) this, EventArgs.Empty);
    }

    public void DisplayNextPageOrPart(bool forceNewPage = false)
    {
      if (this.EatScrolling() || !forceNewPage && this.DisplayPart(PartPageToDisplay.Next))
        return;
      this.DisplayNextPage(ComicDisplay.PagingMode.Double | ComicDisplay.PagingMode.Walled);
    }

    public void DisplayPreviousPageOrPart(bool forceNewPage = false)
    {
      if (this.EatScrolling() || !forceNewPage && this.DisplayPart(PartPageToDisplay.Previous))
        return;
      this.DisplayPreviousPage(ComicDisplay.PagingMode.Double | ComicDisplay.PagingMode.Walled);
    }

    public void DisplayNextPage(ComicDisplay.PagingMode mode)
    {
      bool flag1 = (mode & ComicDisplay.PagingMode.Double) == ComicDisplay.PagingMode.None;
      bool flag2 = (mode & ComicDisplay.PagingMode.Walled) != 0;
      if (!this.IsValid || flag2 && this.IsPageChangeWalled())
        return;
      int offset = flag1 || !this.IsDoubleImage ? 1 : 2;
      if (offset == 2 && this.Book.Comic.GetPage(this.Book.SeekNewPage(1, PageSeekOrigin.Current)).PagePosition == ComicPagePosition.Near)
        offset = 1;
      if (!this.Book.Navigate(offset))
        this.OnLastPageReached();
      this.lastPaging = Machine.Ticks;
    }

    public void DisplayPreviousPage(ComicDisplay.PagingMode mode)
    {
      bool flag1 = (mode & ComicDisplay.PagingMode.Double) == ComicDisplay.PagingMode.None;
      bool flag2 = (mode & ComicDisplay.PagingMode.Walled) != 0;
      if (!this.IsValid || flag2 && this.IsPageChangeWalled())
        return;
      int page1 = this.Book.SeekNewPage(-1, PageSeekOrigin.Current);
      int page2 = this.Book.SeekNewPage(-2, PageSeekOrigin.Current);
      int offset;
      if (!this.TwoPageDisplay | flag1 || page2 == -1)
      {
        offset = -1;
      }
      else
      {
        ComicPageInfo page3 = this.Book.Comic.GetPage(page1);
        ComicPageInfo page4 = this.Book.Comic.GetPage(page2);
        offset = page3.IsSinglePageType || page3.IsDoublePage || page4.IsSinglePageType || page4.IsDoublePage || page3.PagePosition == ComicPagePosition.Near && page4.PagePosition != ComicPagePosition.Far ? -1 : -2;
      }
      if (!this.Book.Navigate(offset))
        this.OnFirstPageReached();
      this.lastPaging = Machine.Ticks;
    }

    public void DisplayFirstPage()
    {
      if (!this.IsValid)
        return;
      this.Book.Navigate(PageSeekOrigin.Beginning);
    }

    public void DisplayLastPage()
    {
      if (!this.IsValid)
        return;
      this.Book.Navigate(PageSeekOrigin.End);
    }

    public void DisplayLastPageRead()
    {
      if (!this.IsValid)
        return;
      this.ImageVisiblePart = ImagePartInfo.Empty;
      this.Book.Navigate(this.Book.Comic.LastPageRead, PageSeekOrigin.Beginning);
    }

    public void DisplayPreviousBookmarkedPage()
    {
      if (!this.IsValid)
        return;
      this.ImageVisiblePart = ImagePartInfo.Empty;
      this.Book.NavigateBookmark(-1);
    }

    public void DisplayNextBookmarkedPage()
    {
      if (!this.IsValid)
        return;
      this.ImageVisiblePart = ImagePartInfo.Empty;
      this.Book.NavigateBookmark(1);
    }

    public void ScrollLeft(float lines)
    {
      if (this.EatScrolling())
        return;
      if (!this.AutoScrolling)
        this.MovePart(new System.Drawing.Point((int) ((double) lines * (double) -this.GetLineSize().Width), 0));
      else if (this.IsMovementFlipped)
        this.DisplayNextPageOrPart();
      else
        this.DisplayPreviousPageOrPart();
    }

    public void ScrollLeft() => this.ScrollLeft(this.scrollLines);

    public void ScrollRight(float lines)
    {
      if (this.EatScrolling())
        return;
      if (!this.AutoScrolling)
        this.MovePart(new System.Drawing.Point((int) ((double) lines * (double) this.GetLineSize().Width), 0));
      else if (this.IsMovementFlipped)
        this.DisplayPreviousPageOrPart();
      else
        this.DisplayNextPageOrPart();
    }

    public void ScrollRight() => this.ScrollRight(this.scrollLines);

    public void ScrollUp(float lines)
    {
      if (this.EatScrolling())
        return;
      if (this.AutoScrolling)
        this.DisplayPreviousPageOrPart();
      else
        this.ScrollUp(lines, this.ScrollingDoesBrowse);
    }

    public void ScrollUp() => this.ScrollUp(this.scrollLines);

    public void ScrollDown(float lines)
    {
      if (this.EatScrolling())
        return;
      if (this.AutoScrolling)
        this.DisplayNextPageOrPart();
      else
        this.ScrollDown(lines, this.ScrollingDoesBrowse);
    }

    public void ScrollDown() => this.ScrollDown(this.scrollLines);

    public void ToggleNavigationOverlay()
    {
      this.NavigationOverlayVisible = !this.NavigationOverlayVisible;
    }

    public bool TwoPageDisplay => this.PageLayout != 0;

    public void TogglePageFit()
    {
      this.ImageFitMode = (ImageFitMode) ((int) (this.ImageFitMode + 1) % 6);
    }

    public void CopyPageToClipboard()
    {
      try
      {
        Clipboard.SetImage((Image) this.CreatePageImage());
      }
      catch
      {
      }
    }

    public void ToggleFullScreen() => this.FullScreen = !this.FullScreen;

    public void TogglePageLayout()
    {
      switch (this.PageLayout)
      {
        case PageLayoutMode.Single:
          this.PageLayout = PageLayoutMode.Double;
          break;
        case PageLayoutMode.Double:
          this.PageLayout = PageLayoutMode.DoubleAdaptive;
          break;
        case PageLayoutMode.DoubleAdaptive:
          this.PageLayout = PageLayoutMode.Single;
          break;
      }
    }

    public void ToogleRealisticPages() => this.RealisticPages = !this.RealisticPages;

    public void ToggleFitOnlyIfOversized()
    {
      this.ImageFitOnlyIfOversized = !this.ImageFitOnlyIfOversized;
    }

    public void ToggleMagnifier() => this.MagnifierVisible = !this.MagnifierVisible;

    public void SetPageOriginal() => this.ImageFitMode = ImageFitMode.Original;

    public void SetPageFitAll() => this.ImageFitMode = ImageFitMode.Fit;

    public void SetPageFitWidth() => this.ImageFitMode = ImageFitMode.FitWidth;

    public void SetPageFitWidthAdaptive() => this.ImageFitMode = ImageFitMode.FitWidthAdaptive;

    public void SetPageFitHeight() => this.ImageFitMode = ImageFitMode.FitHeight;

    public void SetPageBestFit() => this.ImageFitMode = ImageFitMode.BestFit;

    public bool IsPageFitHeight() => this.ImageFitMode == ImageFitMode.FitHeight;

    public bool IsPageFitWidth() => this.ImageFitMode == ImageFitMode.FitWidth;

    public bool IsPageFitWidthAdaptive() => this.ImageFitMode == ImageFitMode.FitWidthAdaptive;

    public bool IsPageFitBest() => this.ImageFitMode == ImageFitMode.BestFit;

    public void SetInfoOverlays(InfoOverlays overlays, bool enable)
    {
      if (enable)
        this.VisibleInfoOverlays |= overlays;
      else
        this.VisibleInfoOverlays &= ~overlays;
    }

    public bool GetInfoOverays(InfoOverlays overlays)
    {
      return this.VisibleInfoOverlays.HasFlag((Enum) overlays);
    }

    private bool IsPageChangeWalled()
    {
      if (this.pageWallTicks == 0L || this.ImagePartCount == 1)
        return false;
      long ticks = Machine.Ticks;
      if (ticks - this.lastPartNavigation > this.pageWallTicks)
      {
        this.wallState = ComicDisplay.WallState.Initial;
        return false;
      }
      switch (this.wallState)
      {
        case ComicDisplay.WallState.Pending:
          if (ticks - this.transitionStart < this.pageWallTicks)
            return true;
          this.wallState = ComicDisplay.WallState.Initial;
          this.transitionStart = ticks;
          return false;
        default:
          this.transitionStart = ticks;
          this.wallState = ComicDisplay.WallState.Pending;
          return true;
      }
    }

    private bool EatScrolling()
    {
      return this.ImagePartCount != 1 && Machine.Ticks - this.lastPaging < this.pageWallTicks;
    }

    private bool ScrollUp(float lines, bool withPageChange)
    {
      if (this.MovePart(new System.Drawing.Point(0, (int) (-(double) lines * (double) this.GetLineSize().Height))))
        return true;
      if (!withPageChange || this.EatScrolling())
        return false;
      this.DisplayPreviousPageOrPart(true);
      return true;
    }

    private bool ScrollDown(float lines, bool withPageChange)
    {
      if (this.MovePart(new System.Drawing.Point(0, (int) ((double) lines * (double) this.GetLineSize().Height))))
        return true;
      if (!withPageChange)
        return false;
      this.DisplayNextPageOrPart(true);
      return true;
    }

    private System.Drawing.Size GetLineSize()
    {
      bool isDoubleImage = this.IsDoubleImage;
      System.Drawing.Size imageSize = this.ImageSize;
      return new System.Drawing.Size(imageSize.Width / (isDoubleImage ? 32 : 16), imageSize.Height / 32);
    }

    public bool SupressContextMenu { get; set; }

    public bool FullScreen
    {
      get => this.fullScreen;
      set
      {
        if (this.control == null)
          return;
        Form parentForm = this.control.ParentForm;
        if (parentForm == null || this.fullScreen == value)
          return;
        this.fullScreen = value;
        if (this.fullScreen)
        {
          this.oldFormState = parentForm.WindowState;
          this.borderStyleOld = parentForm.FormBorderStyle;
          parentForm.WindowState = FormWindowState.Normal;
          parentForm.FormBorderStyle = FormBorderStyle.None;
          this.orgRect = parentForm.Bounds;
          parentForm.Bounds = Screen.GetBounds((Control) parentForm);
          parentForm.TopMost = true;
          this.display.AutoHideCursor = this.HideCursorFullScreen;
        }
        else
        {
          parentForm.TopMost = false;
          parentForm.Bounds = this.orgRect;
          parentForm.FormBorderStyle = this.borderStyleOld;
          parentForm.WindowState = this.oldFormState;
          this.display.AutoHideCursor = false;
        }
        this.OnFullScreenChanged();
      }
    }

    public bool HideCursorFullScreen
    {
      get => this.hideCursorFullScreen;
      set => this.hideCursorFullScreen = value;
    }

    public KeyboardShortcuts KeyboardMap
    {
      get => this.keyboardMap;
      set => this.keyboardMap = value;
    }

    public long PageWallTicks
    {
      get => this.pageWallTicks;
      set => this.pageWallTicks = value;
    }

    public bool ScrollingDoesBrowse
    {
      get => this.scrollingDoesBrowse;
      set => this.scrollingDoesBrowse = value;
    }

    public bool AutoScrolling
    {
      get => this.autoScrolling;
      set => this.autoScrolling = value;
    }

    public ComicPageType PageFilter
    {
      get => this.pageFilter;
      set
      {
        this.pageFilter = value;
        if (this.Book == null)
          return;
        this.Book.PageFilter = value;
      }
    }

    public bool MouseClickEnabled
    {
      get => this.mouseClickEnabled;
      set => this.mouseClickEnabled = value;
    }

    public bool ResetZoomOnPageChange
    {
      get => this.resetZoomOnPageChange;
      set => this.resetZoomOnPageChange = value;
    }

    public bool ZoomInOutOnPageChange
    {
      get => this.zoomInOutOnPageChange;
      set => this.zoomInOutOnPageChange = value;
    }

    private void display_MouseDown(object sender, MouseEventArgs e) => this.scrollLines = 1f;

    private void display_BookChanged(object sender, EventArgs e)
    {
      if (this.Book != null)
        this.Book.PageFilter = this.PageFilter;
      if (!this.resetZoomOnPageChange)
        return;
      this.ImageZoom = 1f;
    }

    private void display_PageChange(object sender, BookPageEventArgs e)
    {
      this.oldZoom = 0.0f;
      if ((double) this.ImageZoom == 1.0)
        return;
      if (this.resetZoomOnPageChange)
      {
        this.ImageZoom = 1f;
      }
      else
      {
        if (!this.zoomInOutOnPageChange || !this.IsHardwareRenderer || !this.ShouldPagingBlend)
          return;
        this.oldZoom = this.ImageZoom;
        this.ImageZoom = 1f;
      }
    }

    private void display_PageChanged(object sender, BookPageEventArgs e)
    {
      if (this.zoomInOutOnPageChange && (double) this.oldZoom != 0.0)
      {
        bool flag = e.OldPage < e.Page;
        if (this.RightToLeftReading && this.RightToLeftReadingMode == RightToLeftReadingMode.FlipParts)
          flag = !flag;
        if (flag)
          this.ZoomTo(new System.Drawing.Point(-500000, -500000), this.oldZoom);
        else
          this.ZoomTo(new System.Drawing.Point(500000, 500000), this.oldZoom);
      }
      this.oldZoom = 0.0f;
    }

    private CommandKey TranslateTouchGesture(GestureEventArgs e)
    {
      switch (e.Area)
      {
        case ContentAlignment.TopLeft:
          return !e.Double ? CommandKey.Gesture1 : CommandKey.GestureDouble1;
        case ContentAlignment.TopCenter:
          return !e.Double ? CommandKey.Gesture2 : CommandKey.GestureDouble2;
        case ContentAlignment.TopRight:
          return !e.Double ? CommandKey.Gesture3 : CommandKey.GestureDouble3;
        case ContentAlignment.MiddleLeft:
          return !e.Double ? CommandKey.Gesture4 : CommandKey.GestureDouble4;
        case ContentAlignment.MiddleCenter:
          return !e.Double ? CommandKey.Gesture5 : CommandKey.GestureDouble5;
        case ContentAlignment.MiddleRight:
          return !e.Double ? CommandKey.Gesture6 : CommandKey.GestureDouble6;
        case ContentAlignment.BottomLeft:
          return !e.Double ? CommandKey.Gesture7 : CommandKey.GestureDouble7;
        case ContentAlignment.BottomCenter:
          return !e.Double ? CommandKey.Gesture8 : CommandKey.GestureDouble8;
        case ContentAlignment.BottomRight:
          return !e.Double ? CommandKey.Gesture9 : CommandKey.GestureDouble9;
        default:
          return CommandKey.None;
      }
    }

    private void display_PreviewGesture(object sender, GestureEventArgs e)
    {
      if (e.Gesture != GestureType.Touch)
        return;
      e.Handled = this.keyboardMap.Commands.Any<KeyboardCommand>((Func<KeyboardCommand, bool>) (c => c.Handles(this.TranslateTouchGesture(e))));
    }

    private void display_Gesture(object sender, GestureEventArgs e)
    {
      switch (e.Gesture)
      {
        case GestureType.Click:
          if (!this.MouseClickEnabled)
            break;
          e.Handled = this.keyboardMap.HandleKey(e.MouseButton, e.Double, e.IsTouch);
          break;
        case GestureType.Touch:
          e.Handled = this.keyboardMap.HandleKey(this.TranslateTouchGesture(e));
          break;
        case GestureType.TwoFingerTap:
          e.Handled = this.keyboardMap.HandleKey(CommandKey.TouchTwoFingerTap);
          break;
        case GestureType.PressAndTap:
          e.Handled = this.keyboardMap.HandleKey(CommandKey.TouchPressAndTap);
          break;
        case GestureType.FlickLeft:
          e.Handled = this.keyboardMap.HandleKey(CommandKey.FlickLeft);
          break;
        case GestureType.FlickRight:
          e.Handled = this.keyboardMap.HandleKey(CommandKey.FlickRight);
          break;
      }
    }

    private void display_KeyDown(object sender, KeyEventArgs e)
    {
      this.scrollLines = 1f;
      e.Handled = this.keyboardMap.HandleKey(e.KeyCode | e.Modifiers);
      if (e.Handled)
        return;
      char numberFromKey = this.GetNumberFromKey(e.KeyCode);
      e.Handled = numberFromKey != char.MinValue && this.pageKeys.Select(numberFromKey);
    }

    private char GetNumberFromKey(Keys key)
    {
      if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
        return (char) (48 + (key - 96));
      return key >= Keys.D0 && key <= Keys.D9 ? (char) (48 + (key - 48)) : char.MinValue;
    }

    private void display_MouseWheel(object sender, MouseEventArgs e)
    {
      if (Control.MouseButtons == MouseButtons.Right)
      {
        this.keyboardMap.HandleKey(e.Delta > 0 ? CommandKey.Tab : CommandKey.Tab | CommandKey.Shift);
        this.SupressContextMenu = true;
      }
      else
      {
        this.scrollLines = (float) Math.Abs(e.Delta / SystemInformation.MouseWheelScrollDelta) * this.MouseWheelSpeed;
        this.keyboardMap.HandleKey(e.Delta > 0 ? CommandKey.MouseWheelUp : CommandKey.MouseWheelDown, Control.ModifierKeys);
      }
    }

    private void display_MouseHWheel(object sender, MouseEventArgs e)
    {
      this.keyboardMap.HandleKey(e.Delta < 0 ? CommandKey.MouseTiltLeft : CommandKey.MouseTiltRight, Control.ModifierKeys);
    }

    private void display_Browse(object sender, BrowseEventArgs e)
    {
      PageSeekOrigin pageSeekOrigin = e.SeekOrigin;
      int offset = e.Offset;
      if (this.IsMovementFlipped && pageSeekOrigin != PageSeekOrigin.Absolute)
      {
        offset = -offset;
        if (pageSeekOrigin == PageSeekOrigin.Beginning || pageSeekOrigin == PageSeekOrigin.End)
          pageSeekOrigin = pageSeekOrigin == PageSeekOrigin.Beginning ? PageSeekOrigin.End : PageSeekOrigin.Beginning;
      }
      switch (pageSeekOrigin)
      {
        case PageSeekOrigin.Beginning:
          this.DisplayFirstPage();
          break;
        case PageSeekOrigin.End:
          this.DisplayLastPage();
          break;
        case PageSeekOrigin.Current:
          if (offset < 0)
          {
            this.DisplayPreviousPage(ComicDisplay.PagingMode.Double);
            break;
          }
          this.DisplayNextPage(ComicDisplay.PagingMode.Double);
          break;
        case PageSeekOrigin.Absolute:
          this.Book.Navigate(offset, pageSeekOrigin);
          break;
      }
    }

    private void control_KeyUp(object sender, KeyEventArgs e) => this.lastPaging = 0L;

    public void DisplayOpenMessage() => this.display.DisplayOpenMessage();

    public float MouseWheelSpeed
    {
      get => this.mouseWheelSpeed;
      set => this.mouseWheelSpeed = value;
    }

    public InfoOverlays VisibleInfoOverlays
    {
      get => this.display.VisibleInfoOverlays;
      set => this.display.VisibleInfoOverlays = value;
    }

    public int ImagePartCount => this.display.ImagePartCount;

    public Bitmap CreatePageImage() => this.display.CreatePageImage();

    public bool NavigationOverlayVisible
    {
      get => this.display.NavigationOverlayVisible;
      set => this.display.NavigationOverlayVisible = value;
    }

    public int CurrentPage => this.display.CurrentPage;

    public int CurrentMousePage => this.display.CurrentMousePage;

    public ImageRotation CurrentImageRotation => this.display.CurrentImageRotation;

    public ComicBookNavigator Book
    {
      get => this.display.Book;
      set => this.display.Book = value;
    }

    public event EventHandler BookChanged
    {
      add => this.display.BookChanged += value;
      remove => this.display.BookChanged -= value;
    }

    public event EventHandler<GestureEventArgs> Gesture
    {
      add => this.display.Gesture += value;
      remove => this.display.Gesture -= value;
    }

    public event EventHandler VisibleInfoOverlaysChanged
    {
      add => this.display.VisibleInfoOverlaysChanged += value;
      remove => this.display.VisibleInfoOverlaysChanged -= value;
    }

    public event EventHandler<GestureEventArgs> PreviewGesture
    {
      add => this.display.PreviewGesture += value;
      remove => this.display.PreviewGesture -= value;
    }

    public event EventHandler<BookPageEventArgs> PageChange
    {
      add => this.display.PageChange += value;
      remove => this.display.PageChange -= value;
    }

    public event EventHandler<BookPageEventArgs> PageChanged
    {
      add => this.display.PageChanged += value;
      remove => this.display.PageChanged -= value;
    }

    public event EventHandler DrawnPageCountChanged
    {
      add => this.display.DrawnPageCountChanged += value;
      remove => this.display.DrawnPageCountChanged -= value;
    }

    public event EventHandler<BrowseEventArgs> Browse
    {
      add => this.display.Browse += value;
      remove => this.display.Browse -= value;
    }

    public bool IsValid => this.display.IsValid;

    public bool ShouldPagingBlend => this.display.ShouldPagingBlend;

    public bool IsDoubleImage => this.display.IsDoubleImage;

    public System.Drawing.Size ImageSize => this.display.ImageSize;

    public ImagePartInfo ImageVisiblePart
    {
      get => this.display.ImageVisiblePart;
      set => this.display.ImageVisiblePart = value;
    }

    public bool DisplayPart(PartPageToDisplay ptd)
    {
      bool flag = this.display.DisplayPart(ptd);
      if (flag)
      {
        this.wallState = ComicDisplay.WallState.Initial;
        this.lastPartNavigation = Machine.Ticks;
      }
      return flag;
    }

    public bool MovePart(System.Drawing.Point offset)
    {
      bool flag = this.display.MovePart(offset);
      if (flag)
      {
        this.wallState = ComicDisplay.WallState.Initial;
        this.lastPartNavigation = Machine.Ticks;
      }
      return flag;
    }

    public void MovePartDown(float percent) => this.display.MovePartDown(percent);

    public bool Focus() => this.control != null && this.control.Focus();

    public IPagePool PagePool
    {
      get => this.display.PagePool;
      set => this.display.PagePool = value;
    }

    public IThumbnailPool ThumbnailPool
    {
      get => this.display.ThumbnailPool;
      set => this.display.ThumbnailPool = value;
    }

    public bool SetRenderer(bool hardware) => this.display.SetRenderer(hardware);

    public bool IsHardwareRenderer => this.display.IsHardwareRenderer;

    public object GetState() => this.display.GetState();

    public void Animate(object a, object b, int time) => this.display.Animate(a, b, time);

    public void Animate(Action<float> animate, int time) => this.display.Animate(animate, time);

    public float DoublePageOverlap
    {
      get => this.display.DoublePageOverlap;
      set => this.display.DoublePageOverlap = value;
    }

    public void ZoomTo(System.Drawing.Point location, float zoom)
    {
      if ((double) this.display.ImageZoom == (double) zoom)
        return;
      this.Animate((Action) (() => this.display.ZoomTo(location, zoom)), (int) ((double) EngineConfiguration.Default.AnimationDuration * 1.0));
    }

    public void RefreshDisplay()
    {
      if (this.IsValid)
      {
        this.display.PagePool.RefreshPage(this.Book.GetPageKey(this.Book.CurrentPage));
        if (this.TwoPageDisplay)
          this.display.PagePool.RefreshPage(this.Book.GetPageKey(this.Book.NextPage));
      }
      if (this.control == null)
        return;
      this.control.Invalidate();
    }

    private IEnumerable<ComicBook> GetBookAsList()
    {
      if (this.Book != null && this.Book.Comic != null)
        yield return this.Book.Comic;
    }

    public IEnumerable<ComicBook> GetBookList(ComicBookFilterType cbft)
    {
      return ComicBookCollection.Filter(cbft, this.GetBookAsList());
    }

    public bool CanBookmark
    {
      get
      {
        return this.Book != null && this.Book.Comic != null && this.Book.Comic.EditMode.CanEditPages();
      }
    }

    public string BookmarkProposal
    {
      get
      {
        if (!this.CanBookmark)
          return string.Empty;
        return !string.IsNullOrEmpty(this.Bookmark) ? this.Bookmark : this.Book.CurrentPageAsText;
      }
    }

    public string Bookmark
    {
      get => !this.CanBookmark ? (string) null : this.Book.CurrentPageInfo.Bookmark;
      set
      {
        if (!this.CanBookmark)
          return;
        this.Book.Comic.UpdateBookmark(this.Book.CurrentPage, value);
      }
    }

    public float MagnifierZoom
    {
      get => this.display.MagnifierZoom;
      set => this.display.MagnifierZoom = value;
    }

    public float MagnifierOpacity
    {
      get => this.display.MagnifierOpacity;
      set => this.display.MagnifierOpacity = value;
    }

    public System.Drawing.Size MagnifierSize
    {
      get => this.display.MagnifierSize;
      set => this.display.MagnifierSize = value;
    }

    public bool AutoHideMagnifier
    {
      get => this.display.AutoHideMagnifier;
      set => this.display.AutoHideMagnifier = value;
    }

    public bool AutoMagnifier
    {
      get => this.display.AutoMagnifier;
      set => this.display.AutoMagnifier = value;
    }

    public ImageDisplayOptions ImageDisplayOptions
    {
      get => this.display.ImageDisplayOptions;
      set => this.display.ImageDisplayOptions = value;
    }

    public bool PageMargin
    {
      get => this.display.PageMargin;
      set
      {
        if (this.display.PageMargin == value)
          return;
        this.Animate((Action) (() => this.display.PageMargin = value));
      }
    }

    public float PageMarginPercentWidth
    {
      get => this.display.PageMarginPercentWidth;
      set
      {
        if ((double) this.display.PageMarginPercentWidth == (double) value)
          return;
        this.Animate((Action) (() => this.display.PageMarginPercentWidth = value));
      }
    }

    public Color BackColor
    {
      get => this.display.BackColor;
      set => this.display.BackColor = value;
    }

    public string BackgroundTexture
    {
      get => this.display.BackgroundTexture;
      set => this.display.BackgroundTexture = value;
    }

    public string PaperTexture
    {
      get => this.display.PaperTexture;
      set => this.display.PaperTexture = value;
    }

    public float PaperTextureStrength
    {
      get => this.display.PaperTextureStrength;
      set => this.display.PaperTextureStrength = value;
    }

    public ImageLayout PaperTextureLayout
    {
      get => this.display.PaperTextureLayout;
      set => this.display.PaperTextureLayout = value;
    }

    public ImageLayout BackgroundImageLayout
    {
      get => this.display.BackgroundImageLayout;
      set => this.display.BackgroundImageLayout = value;
    }

    public ImageBackgroundMode ImageBackgroundMode
    {
      get => this.display.ImageBackgroundMode;
      set => this.display.ImageBackgroundMode = value;
    }

    public bool SmoothScrolling
    {
      get => this.display.SmoothScrolling;
      set => this.display.SmoothScrolling = value;
    }

    public bool RealisticPages
    {
      get => this.display.RealisticPages;
      set => this.display.RealisticPages = value;
    }

    public float InfoOverlayScaling
    {
      get => this.display.InfoOverlayScaling;
      set => this.display.InfoOverlayScaling = value;
    }

    public RightToLeftReadingMode RightToLeftReadingMode
    {
      get => this.display.RightToLeftReadingMode;
      set
      {
        if (this.display.RightToLeftReadingMode == value)
          return;
        this.display.RightToLeftReadingMode = value;
      }
    }

    public bool LeftRightMovementReversed
    {
      get => this.display.LeftRightMovementReversed;
      set => this.display.LeftRightMovementReversed = value;
    }

    public bool TwoPageNavigation
    {
      get => this.display.TwoPageNavigation;
      set => this.display.TwoPageNavigation = value;
    }

    public bool AutoHideCursor
    {
      get => this.display.AutoHideCursor;
      set => this.display.AutoHideCursor = value;
    }

    public bool RightToLeftReading
    {
      get => this.display.RightToLeftReading;
      set
      {
        if (this.display.RightToLeftReading == value)
          return;
        if (this.TwoPageDisplay)
          this.Animate((Action<float>) (p => this.display.DoublePageOverlap = p));
        this.display.RightToLeftReading = value;
        if (!this.TwoPageDisplay)
          return;
        this.Animate((Action<float>) (p => this.display.DoublePageOverlap = 1f - p));
      }
    }

    public bool ImageAutoRotate
    {
      get => this.display.ImageAutoRotate;
      set => this.display.ImageAutoRotate = value;
    }

    public ImageRotation ImageRotation
    {
      get => this.display.ImageRotation;
      set => this.display.ImageRotation = value;
    }

    public PageLayoutMode PageLayout
    {
      get => this.display.PageLayout;
      set
      {
        if (this.display.PageLayout == value)
          return;
        bool imageAutoRotate = this.display.ImageAutoRotate;
        ImageRotation imageRotation = this.display.ImageRotation;
        this.display.ImageAutoRotate = false;
        this.display.ImageRotation = this.display.CurrentImageRotation;
        switch (value)
        {
          case PageLayoutMode.Double:
            if (!this.TwoPageDisplay || this.TwoPageDisplay && !this.IsDoubleImage)
            {
              this.display.PageLayout = value;
              this.Animate((Action<float>) (p => this.display.DoublePageOverlap = 1f - p));
              break;
            }
            break;
          case PageLayoutMode.DoubleAdaptive:
            if (this.TwoPageDisplay)
            {
              if (!this.IsDoubleImage)
              {
                this.Animate((Action<float>) (p => this.display.DoublePageOverlap = p));
                break;
              }
              break;
            }
            this.display.PageLayout = value;
            this.Animate((Action<float>) (p => this.display.DoublePageOverlap = 1f - p));
            break;
          default:
            this.Animate((Action<float>) (p => this.display.DoublePageOverlap = p));
            break;
        }
        this.display.PageLayout = value;
        this.display.DoublePageOverlap = 0.0f;
        this.display.ImageRotation = imageRotation;
        this.display.ImageAutoRotate = imageAutoRotate;
      }
    }

    public bool ImageFitOnlyIfOversized
    {
      get => this.display.ImageFitOnlyIfOversized;
      set
      {
        if (this.display.ImageFitOnlyIfOversized == value)
          return;
        this.Animate((Action) (() => this.display.ImageFitOnlyIfOversized = value));
      }
    }

    public bool MagnifierVisible
    {
      get => this.display.MagnifierVisible;
      set => this.display.MagnifierVisible = value;
    }

    public float ImageZoom
    {
      get => this.display.ImageZoom;
      set
      {
        if ((double) this.display.ImageZoom == (double) value)
          return;
        this.Animate((Action) (() => this.display.ImageZoom = value), (int) ((double) EngineConfiguration.Default.AnimationDuration * 1.0));
      }
    }

    public ImageFitMode ImageFitMode
    {
      get => this.display.ImageFitMode;
      set
      {
        if (this.display.ImageFitMode == value)
          return;
        this.Animate((Action) (() => this.display.ImageFitMode = value));
      }
    }

    public PageTransitionEffect PageTransitionEffect
    {
      get => this.display.PageTransitionEffect;
      set => this.display.PageTransitionEffect = value;
    }

    public bool DisplayChangeAnimation
    {
      get => this.display.DisplayChangeAnimation;
      set => this.display.DisplayChangeAnimation = value;
    }

    public bool FlowingMouseScrolling
    {
      get => this.display.FlowingMouseScrolling;
      set => this.display.FlowingMouseScrolling = value;
    }

    public bool SoftwareFiltering
    {
      get => this.display.SoftwareFiltering;
      set => this.display.SoftwareFiltering = value;
    }

    public bool HardwareFiltering
    {
      get => this.display.HardwareFiltering;
      set => this.display.HardwareFiltering = value;
    }

    public bool IsMovementFlipped => this.display.IsMovementFlipped;

    public bool BlendWhilePaging
    {
      get => this.display.BlendWhilePaging;
      set => this.display.BlendWhilePaging = value;
    }

    public MagnifierStyle MagnifierStyle
    {
      get => this.display.MagnifierStyle;
      set => this.display.MagnifierStyle = value;
    }

    public void Animate(Action m, int time)
    {
      if (!this.DisplayChangeAnimation)
      {
        m();
      }
      else
      {
        object state1 = this.display.GetState();
        m();
        object state2 = this.display.GetState();
        this.display.Animate(state1, state2, time);
      }
    }

    public void Animate(Action m) => this.Animate(m, EngineConfiguration.Default.AnimationDuration);

    public void Animate(Action<float> action)
    {
      this.Animate(action, EngineConfiguration.Default.AnimationDuration);
    }

    bool IEditPage.IsValid => this.IsValid && this.Book.Comic.EditMode.CanEditPages();

    ComicPageType IEditPage.PageType
    {
      get
      {
        if (!this.IsValid)
          return ComicPageType.Other;
        return this.CurrentMousePage != -1 ? this.Book.Comic.GetPage(this.CurrentMousePage).PageType : this.Book.Comic.GetPage(this.CurrentPage).PageType;
      }
      set
      {
        if (!this.IsValid)
          return;
        if (this.CurrentMousePage != -1)
          this.Book.Comic.UpdatePageType(this.CurrentMousePage, value);
        else
          this.Book.Comic.UpdatePageType(this.CurrentPage, value);
      }
    }

    ImageRotation IEditPage.Rotation
    {
      get
      {
        if (!this.IsValid)
          return ImageRotation.None;
        return this.CurrentMousePage != -1 ? this.Book.Comic.GetPage(this.CurrentMousePage).Rotation : this.Book.Comic.GetPage(this.CurrentPage).Rotation;
      }
      set
      {
        if (!this.IsValid)
          return;
        if (this.CurrentMousePage != -1)
          this.Book.Comic.UpdatePageRotation(this.CurrentMousePage, value);
        else
          this.Book.Comic.UpdatePageRotation(this.CurrentPage, value);
      }
    }

    [Flags]
    public enum PagingMode
    {
      None = 0,
      Double = 1,
      Walled = 2,
    }

    private enum WallState
    {
      Initial,
      Pending,
    }
  }
}

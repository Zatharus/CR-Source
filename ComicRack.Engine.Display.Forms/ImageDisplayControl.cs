// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.Forms.ImageDisplayControl
// Assembly: ComicRack.Engine.Display.Forms, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: D83BAE4E-CA55-445A-AD1D-2DF78C341143
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.Display.Forms.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using cYo.Common.Presentation;
using cYo.Common.Presentation.Tao;
using cYo.Common.Runtime;
using cYo.Common.Threading;
using cYo.Common.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Windows7.Multitouch;
using Windows7.Multitouch.WinForms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display.Forms
{
  public class ImageDisplayControl : ContainerControl, IMouseHWheel, IPanableControl
  {
    public const float MinimumZoom = 1f;
    public const float MaximumZoom = 8f;
    private volatile bool lowQualityOverride;
    protected IBitmapRenderer renderer;
    private readonly BackgroundRunner flowRunner = new BackgroundRunner();
    private readonly BackgroundRunner partScrollRunner = new BackgroundRunner();
    private int pageScrollingTime = 1000;
    private bool smoothScrolling = true;
    private ContentAlignment textAlignment = ContentAlignment.MiddleCenter;
    private ImageFitMode imageFitMode = ImageFitMode.Fit;
    private bool imageFitOnlyIfOversized;
    private bool rightToLeftReading;
    private RightToLeftReadingMode rightToLeftReadingMode;
    private bool twoPageNavigation = true;
    private ImagePartInfo imageVisiblePart = ImagePartInfo.Empty;
    private float imageZoom = 1f;
    private float pageMarginPercent = 0.05f;
    private bool pageMargin;
    private volatile ImageDisplayOptions imageDisplayOptions;
    private volatile ImageBackgroundMode imageBackgroundMode = ImageBackgroundMode.Color;
    private string backgroundTexture;
    private ImageRotation imageRotation;
    private bool imageAutoRotate;
    private int autoHideCursorDelay = 5000;
    private bool cursorAutoHide;
    private float anamorphicTolerance = 0.25f;
    private bool clipToDestination;
    private ImageDisplayControl.DisplayOutput display;
    private ImageDisplayControl.DisplayOutput lastRenderedDisplay;
    private bool hardwareFiltering;
    private PointF scrollStartOffs;
    private PointF scrollEndOffs;
    private PointF scrollDelta;
    private long scrollLastTime;
    private ImagePartInfo scrollPartEnd;
    private bool cursorVisible = true;
    private int autoHideCounter;
    private MouseButtons lastMouseButton;
    private MouseButtons pendingClick;
    private bool inPaint;
    private bool blockPaint;
    private bool mouseInView;
    private System.Drawing.Point clickPoint;
    private ImagePartInfo orgPart;
    private float orgZoom;
    private System.Drawing.Point flowLastPoint;
    private long flowLastPointTime;
    private long flowLastTime;
    private PointF flowMouseDelta;
    private PointF flowMinDelta;
    private GestureHandler gestureHandler;
    private ImageRotation gestureRotation;
    private double gestureRotationStart;
    private float gestureZoomStart;
    private bool zoomStart;
    private float zoomOffset;
    private System.Drawing.Point panStart;
    private System.Drawing.Point panLocation;
    private ImagePartInfo panPart;
    private Windows7.Multitouch.GestureEventArgs ignoreEvent;
    private static ImageDisplayControl.HardwareAccelerationType hardwareAcceleration = ImageDisplayControl.HardwareAccelerationType.Enabled;
    private static readonly TextureManagerSettings hardwareSettings = new TextureManagerSettings();
    private IContainer components;
    private Timer autoHideCursorTimer;
    private Timer mouseClickTimer;

    public ImageDisplayControl()
    {
      this.FlowingMouseScrolling = true;
      this.DisplayChangeAnimation = true;
      this.InitializeComponent();
      this.components.Add((IComponent) this.flowRunner);
      this.flowRunner.Synchronize = (ISynchronizeInvoke) this;
      this.flowRunner.Interval = 25;
      this.flowRunner.Tick += new EventHandler(this.FlowTimerTick);
      this.components.Add((IComponent) this.flowRunner);
      this.partScrollRunner.Synchronize = (ISynchronizeInvoke) this;
      this.partScrollRunner.Interval = 25;
      this.partScrollRunner.Tick += new EventHandler(this.PartScrollTimerTick);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.Selectable, true);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.display != null)
          this.display.Dispose();
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public bool IsHardwareRenderer => this.renderer != null && this.renderer.IsHardware;

    public virtual bool IsValid => true;

    [Category("Behavior")]
    [Description("Time full page scrolling needs in ms")]
    [DefaultValue(1000)]
    public virtual int PageScrollingTime
    {
      get => this.pageScrollingTime;
      set => this.pageScrollingTime = value;
    }

    [Category("Behavior")]
    [Description("Scroll parts")]
    [DefaultValue(true)]
    public bool SmoothScrolling
    {
      get => this.smoothScrolling;
      set => this.smoothScrolling = value;
    }

    [Category("Behavior")]
    [Description("Text alignment")]
    [DefaultValue(ContentAlignment.MiddleCenter)]
    public ContentAlignment TextAlignment
    {
      get => this.textAlignment;
      set
      {
        if (this.textAlignment == value)
          return;
        this.textAlignment = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [Description("Page Display Mode")]
    [DefaultValue(ImageFitMode.Fit)]
    public ImageFitMode ImageFitMode
    {
      get => this.imageFitMode;
      set
      {
        if (this.imageFitMode == value)
          return;
        this.imageFitMode = value;
        this.OnPageDisplayModeChanged();
        this.ImageVisiblePart = ImagePartInfo.Empty;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [Description("Fit only if oversized")]
    [DefaultValue(false)]
    public bool ImageFitOnlyIfOversized
    {
      get => this.imageFitOnlyIfOversized;
      set
      {
        if (this.imageFitOnlyIfOversized == value)
          return;
        this.imageFitOnlyIfOversized = value;
        this.OnPageDisplayModeChanged();
        this.ImageVisiblePart = ImagePartInfo.Empty;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool RightToLeftReading
    {
      get => this.rightToLeftReading;
      set
      {
        if (this.rightToLeftReading == value)
          return;
        this.rightToLeftReading = value;
        this.OnReadingModeChanged();
        this.Invalidate();
      }
    }

    [DefaultValue(RightToLeftReadingMode.FlipParts)]
    public RightToLeftReadingMode RightToLeftReadingMode
    {
      get => this.rightToLeftReadingMode;
      set
      {
        if (this.rightToLeftReadingMode == value)
          return;
        this.rightToLeftReadingMode = value;
        this.OnReadingModeChanged();
        this.Invalidate();
      }
    }

    [DefaultValue(true)]
    public bool TwoPageNavigation
    {
      get => this.twoPageNavigation;
      set
      {
        if (this.twoPageNavigation == value)
          return;
        this.twoPageNavigation = value;
        this.Invalidate();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ImagePartInfo ImageVisiblePart
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.imageVisiblePart;
      }
      set
      {
        this.StopPartScrolling();
        this.flowRunner.Enabled = false;
        this.SetVisiblePart(value);
      }
    }

    [Description("Zoom percentage")]
    [DefaultValue(1f)]
    public float ImageZoom
    {
      get => this.imageZoom;
      set => this.DoZoom(this.Display.PartBounds.GetCenter(), value);
    }

    [DefaultValue(0.05f)]
    public float PageMarginPercentWidth
    {
      get => this.pageMarginPercent;
      set
      {
        if ((double) this.pageMarginPercent == (double) value)
          return;
        this.pageMarginPercent = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool PageMargin
    {
      get => this.pageMargin;
      set
      {
        if (this.pageMargin == value)
          return;
        this.pageMargin = value;
        this.Invalidate();
      }
    }

    [Browsable(false)]
    public virtual int ImagePartCount => this.Display.PartCount;

    [Category("Appearance")]
    [Description("Options for the display")]
    [DefaultValue(ImageDisplayOptions.None)]
    public ImageDisplayOptions ImageDisplayOptions
    {
      get => this.imageDisplayOptions;
      set
      {
        if (this.imageDisplayOptions == value)
          return;
        this.imageDisplayOptions = value;
        this.OnImageDisplayOptionsChanged();
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [Description("Options for the background display")]
    [DefaultValue(ImageBackgroundMode.Color)]
    public ImageBackgroundMode ImageBackgroundMode
    {
      get => this.imageBackgroundMode;
      set
      {
        if (this.imageBackgroundMode == value)
          return;
        this.imageBackgroundMode = value;
        this.OnImageDisplayOptionsChanged();
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [Description("background texture file")]
    [DefaultValue(null)]
    public string BackgroundTexture
    {
      get => this.backgroundTexture;
      set
      {
        if (this.backgroundTexture == value)
          return;
        this.backgroundTexture = value;
        Image backgroundImage = this.BackgroundImage;
        try
        {
          this.BackgroundImage = Image.FromFile(this.backgroundTexture);
        }
        catch (Exception ex)
        {
          this.BackgroundImage = (Image) null;
        }
        backgroundImage.SafeDispose();
        this.OnImageDisplayOptionsChanged();
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [Description("Rotation of the page")]
    [DefaultValue(ImageRotation.None)]
    public ImageRotation ImageRotation
    {
      get => this.imageRotation;
      set
      {
        if (this.imageRotation == value)
          return;
        this.imageRotation = value;
        this.OnPageDisplayModeChanged();
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [Description("Auto Rotate page")]
    [DefaultValue(false)]
    public bool ImageAutoRotate
    {
      get => this.imageAutoRotate;
      set
      {
        if (this.imageAutoRotate == value)
          return;
        this.imageAutoRotate = value;
        this.OnPageDisplayModeChanged();
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [Description("Time in ms after the Cursor gets hidden in seconds")]
    [DefaultValue(5000)]
    public int AutoHideCursorDelay
    {
      get => this.autoHideCursorDelay;
      set
      {
        this.autoHideCursorDelay = value;
        this.UpdateCursorAutoHide();
      }
    }

    [Category("Behavior")]
    [Description("Turns Automatic Hiding of the cursor on or off")]
    [DefaultValue(false)]
    public bool AutoHideCursor
    {
      get => this.cursorAutoHide;
      set
      {
        this.cursorAutoHide = value;
        this.UpdateCursorAutoHide();
      }
    }

    [DefaultValue(true)]
    public bool DisplayChangeAnimation { get; set; }

    [DefaultValue(true)]
    public bool FlowingMouseScrolling { get; set; }

    [DefaultValue(false)]
    public bool DisableHardwareAcceleration { get; set; }

    [DefaultValue(0.25f)]
    public float AnamorphicTolerance
    {
      get => this.anamorphicTolerance;
      set
      {
        if ((double) this.anamorphicTolerance == (double) value)
          return;
        this.anamorphicTolerance = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool ClipToDestination
    {
      get => this.clipToDestination;
      set
      {
        if (this.clipToDestination == value)
          return;
        this.clipToDestination = value;
        this.Invalidate();
      }
    }

    public float CurrentAnamorphicTolerance
    {
      get
      {
        return (this.ImageDisplayOptions & ImageDisplayOptions.AnamorphicScaling) == ImageDisplayOptions.None ? 0.0f : this.AnamorphicTolerance;
      }
    }

    public ImageRotation CurrentImageRotation => this.LastRenderedDisplay.Config.Rotation;

    public Rectangle PagePartBounds => this.Display.PartBounds;

    public System.Drawing.Size ImageSize => this.GetImageSize();

    public bool FullImageVisible
    {
      get
      {
        System.Drawing.Size imageSize = this.ImageSize;
        System.Drawing.Size size = this.PagePartBounds.Size;
        return size.Width >= imageSize.Width - 2 && size.Height >= imageSize.Height - 2;
      }
    }

    public bool MouseActionHappened { get; protected set; }

    protected bool DisableScrolling { get; set; }

    protected ImageDisplayControl.DisplayOutputConfig DisplayConfig
    {
      get
      {
        this.OnUpdateDisplayConfig();
        System.Drawing.Size imageSize = this.GetImageSize();
        bool flag = imageSize.Width > imageSize.Height && !this.IsDoubleImage;
        ImageRotation rotation = !this.ImageAutoRotate || imageSize.Width <= imageSize.Height ? this.ImageRotation : this.ImageRotation.RotateLeft();
        return new ImageDisplayControl.DisplayOutputConfig(this.ClientRectangle.Size, imageSize, this.ImageFitMode, this.ImageFitOnlyIfOversized, flag ? RightToLeftReadingMode.FlipParts : this.RightToLeftReadingMode, this.RightToLeftReading, this.ImageVisiblePart, this.ImageZoom, this.ImageZoom * (this.PageMargin ? 1f - this.PageMarginPercentWidth : 1f), rotation, flag && this.TwoPageNavigation);
      }
    }

    protected ImageDisplayControl.DisplayOutput Display
    {
      get
      {
        if (this.display == null || !object.Equals((object) this.display.Config, (object) this.DisplayConfig))
        {
          if (this.display != null)
            this.display.Dispose();
          this.display = ImageDisplayControl.DisplayOutput.Create(this.DisplayConfig, this.CurrentAnamorphicTolerance);
        }
        return this.display;
      }
    }

    protected ImageDisplayControl.DisplayOutput LastRenderedDisplay
    {
      get => this.lastRenderedDisplay ?? this.Display;
      set
      {
        this.lastRenderedDisplay.SafeDispose();
        this.lastRenderedDisplay = value.Clone() as ImageDisplayControl.DisplayOutput;
      }
    }

    public Color CurrentBackColor
    {
      get
      {
        Color currentBackColor = this.ImageBackgroundMode == ImageBackgroundMode.Auto ? this.GetAutoBackgroundColor() : this.BackColor;
        if (currentBackColor == Color.Empty)
          currentBackColor = this.BackColor;
        return currentBackColor;
      }
    }

    protected virtual void RenderImage(
      IBitmapRenderer renderer,
      ImageDisplayControl.DisplayOutput display,
      ImageDisplayControl.RenderType renderType = ImageDisplayControl.RenderType.Default)
    {
      if ((renderType & ImageDisplayControl.RenderType.Background) != ImageDisplayControl.RenderType.None)
        this.RenderImageBackground(renderer, display);
      using (renderer.SaveState())
      {
        try
        {
          renderer.HighQuality = this.HasDisplayOption(ImageDisplayOptions.HighQuality) && !this.lowQualityOverride;
          if (display.IsEmpty)
            return;
          System.Drawing.Drawing2D.Matrix transform = renderer.Transform;
          transform.Multiply(display.Transform);
          renderer.Transform = transform;
          if (!renderer.IsVisible((RectangleF) display.OutputBounds))
            return;
          if ((renderType & ImageDisplayControl.RenderType.Image) != ImageDisplayControl.RenderType.None)
            this.DrawImage(renderer, display.OutputBounds, display.PartBounds);
          if ((renderType & ImageDisplayControl.RenderType.Effect) == ImageDisplayControl.RenderType.None)
            return;
          this.RenderImageEffect(renderer, display);
        }
        catch (Exception ex)
        {
          throw;
        }
      }
    }

    protected bool IsTexturedBackground
    {
      get
      {
        return this.imageBackgroundMode == ImageBackgroundMode.Texture && this.BackgroundImage is Bitmap;
      }
    }

    protected bool IsConstantBackground => this.imageBackgroundMode != 0;

    protected virtual void RenderImageBackground(
      IBitmapRenderer bitmapRenderer,
      ImageDisplayControl.DisplayOutput output)
    {
      if (!this.IsTexturedBackground)
      {
        bitmapRenderer.FillRectangle((RectangleF) this.ClientRectangle, this.CurrentBackColor);
      }
      else
      {
        Bitmap backgroundImage = (Bitmap) this.BackgroundImage;
        using (bitmapRenderer.SaveState())
        {
          float num = output != null ? output.ImageZoom : this.ImageZoom;
          bitmapRenderer.TranslateTransform((float) this.ClientRectangle.Width / 2f, (float) this.ClientRectangle.Height / 2f);
          bitmapRenderer.ScaleTransform(num, num);
          bitmapRenderer.TranslateTransform((float) -this.ClientRectangle.Width / 2f, (float) -this.ClientRectangle.Height / 2f);
          bitmapRenderer.FillRectangle((RendererImage) backgroundImage, this.BackgroundImageLayout, (RectangleF) this.ClientRectangle, (RectangleF) backgroundImage.Size.ToRectangle(), BitmapAdjustment.Empty, 1f);
        }
      }
    }

    protected virtual void RenderImageEffect(
      IBitmapRenderer renderer,
      ImageDisplayControl.DisplayOutput display)
    {
    }

    protected virtual void RenderImageOverlay(
      IBitmapRenderer renderer,
      ImageDisplayControl.DisplayOutput output)
    {
      this.OnRenderImageOverlay(new ImageDisplayControl.RenderEventArgs(renderer, output));
    }

    protected void RenderImageSafe(
      IBitmapRenderer bitmapRenderer,
      ImageDisplayControl.DisplayOutput output,
      ImageDisplayControl.RenderType renderType = ImageDisplayControl.RenderType.Default)
    {
      try
      {
        this.RenderImage(bitmapRenderer, output, renderType);
      }
      catch
      {
      }
    }

    public void RenderScene(Graphics gr, ImageDisplayControl.DisplayOutput output)
    {
      IBitmapRenderer renderer = this.renderer;
      if (!this.IsHandleCreated || renderer == null)
        return;
      if (renderer.IsLocked)
      {
        this.Invalidate();
      }
      else
      {
        try
        {
          if (!renderer.BeginScene(gr))
            return;
          this.RenderImage(renderer, output);
          this.RenderImageOverlay(renderer, output);
        }
        catch (Exception ex)
        {
          if (this.HandleRendererError(ex))
            renderer = (IBitmapRenderer) null;
          this.Invalidate();
        }
        finally
        {
          try
          {
            renderer.EndScene();
          }
          catch
          {
          }
          this.LastRenderedDisplay = output;
        }
      }
    }

    public virtual Bitmap CreatePageImage()
    {
      Bitmap pageImage = (Bitmap) null;
      try
      {
        Rectangle rectangle = new Rectangle(System.Drawing.Point.Empty, this.GetImageSize());
        pageImage = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage((Image) pageImage))
          this.DrawImage((IBitmapRenderer) new BitmapGdiRenderer(graphics), rectangle, rectangle);
        return pageImage;
      }
      catch
      {
        pageImage.SafeDispose();
        return (Bitmap) null;
      }
    }

    public System.Drawing.Point ClientToImage(System.Drawing.Point pt, bool withOffset = true)
    {
      return this.ClientToImage(this.LastRenderedDisplay ?? this.Display, pt, withOffset);
    }

    public System.Drawing.Point ClientToImage(
      ImageDisplayControl.DisplayOutput output,
      System.Drawing.Point pt,
      bool withOffset = true)
    {
      if (output.Transform == null)
        return System.Drawing.Point.Empty;
      try
      {
        System.Drawing.Point[] pts = new System.Drawing.Point[1]{ pt };
        System.Drawing.Drawing2D.Matrix transform = output.Transform;
        transform.Invert();
        transform.TransformPoints(pts);
        transform.Invert();
        pt = pts[0];
        if (withOffset)
          pt.Offset(output.PartBounds.Location);
        return pt;
      }
      catch
      {
        return System.Drawing.Point.Empty;
      }
    }

    public void MovePartDown(float percent)
    {
      this.MovePart(new System.Drawing.Point(0, (int) ((double) this.Display.OutputBounds.Height * (double) percent)));
    }

    public bool MovePart(System.Drawing.Point offset)
    {
      ImagePartInfo ipi1 = this.partScrollRunner.Enabled ? this.scrollPartEnd : this.ImageVisiblePart;
      System.Drawing.Point offset1 = offset;
      offset1.Offset(ipi1.Offset);
      System.Drawing.Point partOffset = this.display.GetPartOffset(ipi1.Part, offset1);
      ImagePartInfo ipi2 = new ImagePartInfo(ipi1.Part, partOffset);
      if (this.SmoothScrolling)
        this.ScrollToPart(ipi2);
      else
        this.ImageVisiblePart = ipi2;
      Rectangle part1 = this.Display.GetPart(ipi2);
      Rectangle part2 = this.Display.GetPart(ipi1);
      return Math.Abs(part1.X - part2.X) > 1 || Math.Abs(part1.Y - part2.Y) > 1;
    }

    public bool DisplayPart(PartPageToDisplay ptd)
    {
      if (!this.IsValid)
        return false;
      ImagePartInfo imagePartInfo = ptd == PartPageToDisplay.First || ptd == PartPageToDisplay.Last || !this.partScrollRunner.Enabled ? this.ImageVisiblePart : this.scrollPartEnd;
      ImagePartInfo ipi;
      switch (ptd)
      {
        case PartPageToDisplay.Previous:
          if (this.Display.IsStartPart(imagePartInfo))
            return false;
          ipi = this.Display.GetBestPartFit(imagePartInfo);
          ipi = new ImagePartInfo(ipi.Part - 1, 0, ipi.Offset.Y);
          break;
        case PartPageToDisplay.Next:
          if (this.Display.IsEndPart(imagePartInfo))
            return false;
          ipi = this.Display.GetBestPartFit(imagePartInfo);
          ipi = new ImagePartInfo(ipi.Part + 1, 0, ipi.Offset.Y);
          break;
        case PartPageToDisplay.Last:
          if (this.Display.IsEndPart(imagePartInfo))
            return false;
          ipi = new ImagePartInfo(this.Display.PartCount - 1, 0, 0);
          break;
        default:
          if (this.Display.IsStartPart(imagePartInfo))
            return false;
          ipi = new ImagePartInfo(0, 0, 0);
          break;
      }
      if (this.SmoothScrolling)
        this.ScrollToPart(ipi);
      else
        this.ImageVisiblePart = ipi;
      return true;
    }

    public object GetState()
    {
      return (object) ImageDisplayControl.DisplayOutput.Create(this.DisplayConfig, this.CurrentAnamorphicTolerance);
    }

    public void Animate(object state1, object state2, int time)
    {
      if (!this.DisplayChangeAnimation || !this.IsHandleCreated || this.renderer == null || !this.renderer.IsHardware)
        return;
      ImageDisplayControl.DisplayOutput a = state1 as ImageDisplayControl.DisplayOutput;
      ImageDisplayControl.DisplayOutput b = state2 as ImageDisplayControl.DisplayOutput;
      if (a == null || a.IsEmpty || b == null || b.IsEmpty || a.Equals((object) b))
        return;
      this.RenderScene((Graphics) null, a);
      ThreadUtility.Animate(time, (Action<float>) (p => this.RenderScene((Graphics) null, ImageDisplayControl.DisplayOutput.Interpolate(a, b, p))));
      this.RenderScene((Graphics) null, b);
    }

    public void Animate(Action<float> animate, int time)
    {
      if (!this.DisplayChangeAnimation || !this.IsHandleCreated || this.renderer == null)
        return;
      if (!this.renderer.IsHardware)
        return;
      try
      {
        this.blockPaint = true;
        ThreadUtility.Animate(time, (Action<float>) (p =>
        {
          animate(p);
          this.RenderScene((Graphics) null, this.Display);
        }));
      }
      finally
      {
        this.blockPaint = false;
      }
      this.Invalidate();
    }

    public bool HardwareFiltering
    {
      get => this.hardwareFiltering;
      set
      {
        this.hardwareFiltering = value;
        if (!(this.renderer is IHardwareRenderer renderer))
          return;
        renderer.EnableFilter = this.hardwareFiltering;
      }
    }

    public bool SetRenderer(bool hardware)
    {
      try
      {
        if (hardware && ImageDisplayControl.HardwareAcceleration != ImageDisplayControl.HardwareAccelerationType.Disabled && !this.DisableHardwareAcceleration)
        {
          if (this.renderer != null && this.renderer.IsHardware)
            return true;
          if (this.renderer != null && this.renderer is IDisposable)
          {
            IDisposable renderer = this.renderer as IDisposable;
            this.renderer = (IBitmapRenderer) null;
            renderer.Dispose();
          }
          ControlOpenGlRenderer controlOpenGlRenderer = (ControlOpenGlRenderer) null;
          bool flag;
          try
          {
            controlOpenGlRenderer = new ControlOpenGlRenderer((Control) this, false, (TextureManagerSettings) ImageDisplayControl.HardwareSettings.Clone())
            {
              EnableFilter = this.hardwareFiltering
            };
            flag = controlOpenGlRenderer.IsSoftwareRenderer;
          }
          catch
          {
            flag = true;
          }
          if (!flag || ImageDisplayControl.HardwareAcceleration == ImageDisplayControl.HardwareAccelerationType.Forced)
          {
            this.renderer = (IBitmapRenderer) controlOpenGlRenderer;
            return true;
          }
          controlOpenGlRenderer?.Dispose();
        }
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        if (this.renderer != null)
        {
          if (!this.renderer.IsHardware)
            return true;
          (this.renderer as IDisposable).SafeDispose();
        }
        this.renderer = (IBitmapRenderer) new BitmapGdiRenderer();
        return !hardware;
      }
      finally
      {
        this.Invalidate();
      }
    }

    public void ZoomTo(System.Drawing.Point location, float zoom)
    {
      if (location.IsEmpty)
        location = this.PointToClient(Cursor.Position);
      this.DoZoom(this.ClientToImage(location), zoom.Clamp(1f, 8f));
    }

    private void ScrollToPart(ImagePartInfo ipi)
    {
      ipi = this.Display.GetBestPartFit(ipi);
      System.Drawing.Point location1 = this.Display.PartBounds.Location;
      System.Drawing.Point location2 = this.Display.GetPart(ipi.Part).Location;
      this.scrollStartOffs = (PointF) new System.Drawing.Point(location1.X - location2.X, location1.Y - location2.Y);
      this.scrollEndOffs = (PointF) ipi.Offset;
      this.scrollDelta.X = this.scrollEndOffs.X - this.scrollStartOffs.X;
      this.scrollDelta.Y = this.scrollEndOffs.Y - this.scrollStartOffs.Y;
      if (!this.partScrollRunner.Enabled)
        this.partScrollRunner.Start();
      this.scrollPartEnd = ipi;
    }

    private void StopPartScrolling()
    {
      this.scrollLastTime = 0L;
      this.lowQualityOverride = false;
      this.partScrollRunner.Stop();
    }

    private void PartScrollTimerTick(object sender, EventArgs e)
    {
      long ticks = Machine.Ticks;
      int num1 = this.scrollLastTime != 0L ? (int) (ticks - this.scrollLastTime) : this.partScrollRunner.Interval;
      System.Drawing.Size imageSize = this.Display.Config.ImageSize;
      float num2 = imageSize.Width == 0 ? 0.0f : Math.Abs(this.scrollDelta.X) / (float) imageSize.Width;
      float num3 = imageSize.Height == 0 ? 0.0f : Math.Abs(this.scrollDelta.Y) / (float) imageSize.Height;
      float num4 = num2 * (float) this.PageScrollingTime;
      float num5 = num3 * (float) this.PageScrollingTime;
      float num6 = (double) num4 > 0.0 ? this.scrollDelta.X * (float) num1 / num4 : 0.0f;
      float num7 = (double) num5 > 0.0 ? this.scrollDelta.Y * (float) num1 / num5 : 0.0f;
      this.scrollStartOffs.X += num6;
      this.scrollStartOffs.Y += num7;
      this.scrollStartOffs.X = (double) this.scrollDelta.X < 0.0 ? Math.Max(this.scrollStartOffs.X, this.scrollEndOffs.X) : Math.Min(this.scrollStartOffs.X, this.scrollEndOffs.X);
      this.scrollStartOffs.Y = (double) this.scrollDelta.Y < 0.0 ? Math.Max(this.scrollStartOffs.Y, this.scrollEndOffs.Y) : Math.Min(this.scrollStartOffs.Y, this.scrollEndOffs.Y);
      if (this.scrollStartOffs == this.scrollEndOffs)
      {
        this.ImageVisiblePart = this.scrollPartEnd;
      }
      else
      {
        this.lowQualityOverride = !this.renderer.IsHardware;
        this.SetVisiblePart(new ImagePartInfo(this.scrollPartEnd.Part, (int) this.scrollStartOffs.X, (int) this.scrollStartOffs.Y));
        this.scrollLastTime = ticks;
      }
    }

    private void DoZoom(System.Drawing.Point center, float zoom)
    {
      if ((double) zoom < 1.0 || (double) zoom > 8.0)
        throw new ArgumentOutOfRangeException("value", "zoom value is out of range");
      if ((double) this.imageZoom == (double) zoom)
        return;
      Rectangle partBounds = this.Display.PartBounds;
      System.Drawing.Point point1 = center;
      float num1 = (float) (center.X - partBounds.X) / (float) partBounds.Width;
      float num2 = (float) (center.Y - partBounds.Y) / (float) partBounds.Height;
      this.imageZoom = zoom;
      Rectangle part = this.Display.GetPart(0);
      System.Drawing.Point point2 = new System.Drawing.Point((int) ((double) part.X + (double) part.Width * (double) num1), (int) ((double) part.Y + (double) part.Height * (double) num2));
      this.ImageVisiblePart = new ImagePartInfo(0, point1.X - point2.X, point1.Y - point2.Y);
      this.OnPageDisplayModeChanged();
      this.Invalidate();
    }

    protected bool HandleRendererError(Exception e)
    {
      if (e == null || !e.ToString().Contains("Tao."))
        return false;
      this.SetRenderer(false);
      return true;
    }

    private void SetVisiblePart(ImagePartInfo value)
    {
      using (ItemMonitor.Lock((object) this))
      {
        value = this.Display.GetBestPartFit(value);
        if (object.Equals((object) this.imageVisiblePart, (object) value))
          return;
        this.imageVisiblePart = value;
      }
      this.OnVisiblePartChanged();
      this.Invalidate();
    }

    private void FireMouseHWheel(int wParam, int lParam)
    {
      System.Drawing.Point point = new System.Drawing.Point(lParam);
      int delta = wParam >> 16;
      this.OnMouseHWheel(new MouseEventArgs(MouseButtons.None, 0, point.X, point.Y, delta));
    }

    private void AutoHideCursorTimerTick(object sender, EventArgs e)
    {
      if (!this.cursorAutoHide || this.autoHideCounter < 0)
        return;
      this.autoHideCounter -= this.autoHideCursorTimer.Interval;
      if (this.autoHideCounter >= 0 || !this.mouseInView)
        return;
      this.ShowCursor(false);
    }

    private void ShowCursor(bool visible)
    {
      if (this.cursorVisible == visible)
        return;
      this.cursorVisible = visible;
      if (this.cursorVisible)
        Cursor.Show();
      else
        Cursor.Hide();
    }

    private void UpdateCursorAutoHide()
    {
      this.ShowCursor(true);
      if (!this.cursorAutoHide || this.autoHideCursorDelay <= 0 || !this.mouseInView)
        return;
      this.autoHideCounter = this.autoHideCursorDelay;
    }

    protected bool DisplayEventsDisabled { get; set; }

    public event EventHandler PageDisplayModeChanged;

    public event EventHandler VisiblePartChanged;

    public event MouseEventHandler MouseHWheel;

    public event EventHandler UpdateDisplayConfig;

    public event EventHandler<ImageDisplayControl.RenderEventArgs> RendeImageOverlay;

    public event EventHandler<cYo.Projects.ComicRack.Engine.Display.GestureEventArgs> PreviewGesture;

    public event EventHandler<cYo.Projects.ComicRack.Engine.Display.GestureEventArgs> Gesture;

    protected virtual void OnMouseHWheel(MouseEventArgs e)
    {
      if (this.MouseHWheel == null)
        return;
      this.MouseHWheel((object) this, e);
    }

    protected virtual void OnPageDisplayModeChanged()
    {
      if (this.PageDisplayModeChanged == null)
        return;
      this.PageDisplayModeChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnVisiblePartChanged()
    {
      if (this.VisiblePartChanged == null)
        return;
      this.VisiblePartChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnUpdateDisplayConfig()
    {
      if (this.UpdateDisplayConfig == null)
        return;
      this.UpdateDisplayConfig((object) this, EventArgs.Empty);
    }

    protected virtual void OnPreviewGesture(cYo.Projects.ComicRack.Engine.Display.GestureEventArgs e)
    {
      if (this.PreviewGesture == null)
        return;
      this.PreviewGesture((object) this, e);
    }

    protected virtual void OnGesture(cYo.Projects.ComicRack.Engine.Display.GestureEventArgs e)
    {
      e.Handled = true;
      this.OnPreviewGesture(e);
      if (!e.Handled)
        return;
      e.Handled = false;
      if (this.Gesture == null)
        return;
      this.Gesture((object) this, e);
    }

    protected virtual bool IsImageValid() => false;

    protected virtual System.Drawing.Size GetImageSize() => System.Drawing.Size.Empty;

    public virtual bool IsDoubleImage => false;

    protected virtual void DrawImage(
      IBitmapRenderer renderer,
      Rectangle destination,
      Rectangle source,
      bool clipToDestination)
    {
    }

    protected void DrawImage(IBitmapRenderer renderer, Rectangle destination, Rectangle source)
    {
      this.DrawImage(renderer, destination, source, this.ClipToDestination);
    }

    protected virtual Color GetAutoBackgroundColor() => Color.Empty;

    protected virtual void OnRenderImageOverlay(ImageDisplayControl.RenderEventArgs e)
    {
      if (this.RendeImageOverlay == null)
        return;
      this.RendeImageOverlay((object) this, e);
    }

    protected virtual bool IsMouseOk(System.Drawing.Point point) => true;

    protected virtual void OnImageDisplayOptionsChanged()
    {
    }

    protected virtual void OnReadingModeChanged()
    {
    }

    protected virtual bool MouseHandled => false;

    protected override void OnCreateControl()
    {
      base.OnCreateControl();
      this.InitWindowsTouch();
      this.SetRenderer(true);
    }

    private void StopTimer() => this.mouseClickTimer.Stop();

    private void StartTimer()
    {
      this.mouseClickTimer.Stop();
      this.mouseClickTimer.Start();
    }

    private void mouseClickTimer_Tick(object sender, EventArgs e)
    {
      if (this.pendingClick != MouseButtons.None)
      {
        this.HandleClick(this.pendingClick, false);
        this.pendingClick = MouseButtons.None;
      }
      this.StopTimer();
    }

    protected override void OnDoubleClick(EventArgs e)
    {
      this.StopTimer();
      this.pendingClick = MouseButtons.None;
      this.HandleClick(this.lastMouseButton, true, this.IsTouchMessage());
    }

    protected override void OnClick(EventArgs e)
    {
      this.pendingClick = this.lastMouseButton;
      this.StartTimer();
    }

    public ImageDisplayControl.GestureArea GestureHitTest(System.Drawing.Point pt)
    {
      Rectangle rectangle = new Rectangle(0, 0, FormUtility.ScaleDpiX(EngineConfiguration.Default.GestureAreaSize), FormUtility.ScaleDpiY(EngineConfiguration.Default.GestureAreaSize));
      foreach (ContentAlignment alignment in Enum.GetValues(typeof (ContentAlignment)))
      {
        Rectangle area = rectangle.Align(this.ClientRectangle, alignment);
        if (area.Contains(pt))
          return new ImageDisplayControl.GestureArea(alignment, area);
      }
      return (ImageDisplayControl.GestureArea) null;
    }

    private void HandleClick(MouseButtons button, bool doubleClick, bool isTouch = false)
    {
      if (this.MouseHandled)
        return;
      System.Drawing.Point client = this.PointToClient(Cursor.Position);
      ImageDisplayControl.GestureArea gestureArea = this.GestureHitTest(client);
      bool flag = false;
      if (gestureArea != null)
      {
        cYo.Projects.ComicRack.Engine.Display.GestureEventArgs e = new cYo.Projects.ComicRack.Engine.Display.GestureEventArgs(GestureType.Touch)
        {
          Area = gestureArea.Alignment,
          AreaBounds = gestureArea.Area,
          Double = doubleClick
        };
        this.OnGesture(e);
        flag = e.Handled;
      }
      if (flag)
        return;
      this.OnGesture(new cYo.Projects.ComicRack.Engine.Display.GestureEventArgs(GestureType.Click)
      {
        MouseButton = button,
        Location = client,
        Double = doubleClick,
        IsTouch = isTouch
      });
    }

    protected override void DefWndProc(ref Message m)
    {
      if (m.Msg == 526)
      {
        try
        {
          this.FireMouseHWheel(m.WParam.ToInt32(), m.LParam.ToInt32());
          m.Result = (IntPtr) 1;
        }
        catch (Exception ex)
        {
        }
      }
      base.DefWndProc(ref m);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (this.inPaint)
      {
        this.Invalidate();
      }
      else
      {
        if (this.blockPaint)
          return;
        try
        {
          this.inPaint = true;
          base.OnPaint(e);
          ImageDisplayControl.DisplayOutput display = this.Display;
          ImageDisplayControl.DisplayOutputConfig config = this.LastRenderedDisplay.Config;
          if (this.DisplayChangeAnimation && this.IsHandleCreated && this.renderer != null && this.renderer.IsHardware && this.IsImageValid() && display.Config.Rotation != config.Rotation)
          {
            using (ImageDisplayControl.DisplayOutput state1 = ImageDisplayControl.DisplayOutput.Create(config, this.CurrentAnamorphicTolerance))
              this.Animate((object) state1, (object) display, EngineConfiguration.Default.AnimationDuration);
          }
          this.RenderScene(e.Graphics, display);
          if (string.IsNullOrEmpty(this.Text))
            return;
          using (StringFormat format = new StringFormat()
          {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
          })
          {
            using (SolidBrush solidBrush = new SolidBrush(this.ForeColor))
              e.Graphics.DrawString(this.Text, this.Font, (Brush) solidBrush, (RectangleF) this.ClientRectangle, format);
          }
        }
        finally
        {
          this.inPaint = false;
        }
      }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.lastMouseButton = e.Button;
      this.Focus();
      if (e.Button != MouseButtons.Right)
      {
        this.orgPart = this.ImageVisiblePart;
        this.orgZoom = this.ImageZoom;
        this.clickPoint = e.Location;
        this.flowMouseDelta = this.flowMinDelta = PointF.Empty;
        this.MouseActionHappened = false;
        if (this.renderer != null && this.renderer.IsHardware && this.FlowingMouseScrolling)
          this.flowRunner.Start();
      }
      this.UpdateCursorAutoHide();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (e.Button == MouseButtons.Left && this.IsMouseOk(e.Location) && !this.clickPoint.IsEmpty && (Math.Abs(this.clickPoint.X - e.X) > 5 || Math.Abs(this.clickPoint.Y - e.Y) > 5))
      {
        this.MouseActionHappened = true;
        if (!this.FullImageVisible && !this.DisableScrolling)
        {
          Cursor.Current = Cursors.Hand;
          System.Drawing.Point image1 = this.ClientToImage(this.clickPoint);
          System.Drawing.Point image2 = this.ClientToImage(e.Location);
          this.SetVisiblePart(new ImagePartInfo(this.orgPart.Part, new System.Drawing.Point(this.orgPart.Offset.X + (image1.X - image2.X), this.orgPart.Offset.Y + (image1.Y - image2.Y))));
        }
      }
      if (e.Button == MouseButtons.Middle)
      {
        this.DoZoom(this.ClientToImage(this.clickPoint), (this.orgZoom + (float) (e.Location.Y - this.clickPoint.Y) / 100f).Clamp(1f, 8f));
        this.MouseActionHappened = true;
      }
      this.UpdateCursorAutoHide();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      this.clickPoint = System.Drawing.Point.Empty;
      base.OnMouseUp(e);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
      if (this.MouseActionHappened)
        return;
      base.OnMouseClick(e);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
      base.OnMouseEnter(e);
      this.mouseInView = true;
      this.UpdateCursorAutoHide();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      this.mouseInView = false;
      this.UpdateCursorAutoHide();
    }

    private void FlowTimerTick(object sender, EventArgs e)
    {
      long ticks = Machine.Ticks;
      if (!this.clickPoint.IsEmpty || !this.panStart.IsEmpty)
      {
        this.flowRunner.Interval = 25;
        this.flowLastTime = 0L;
        this.flowMinDelta = PointF.Empty;
        System.Drawing.Point client = this.PointToClient(this.panStart.IsEmpty ? Cursor.Position : this.panLocation);
        if (!this.flowLastPoint.IsEmpty)
        {
          System.Drawing.Point image1 = this.ClientToImage(this.flowLastPoint);
          System.Drawing.Point image2 = this.ClientToImage(client);
          float num = (float) (ticks - this.flowLastPointTime) * 2f;
          this.flowMouseDelta = new PointF((float) (image1.X - image2.X) / num, (float) (image1.Y - image2.Y) / num);
        }
        this.flowLastPoint = client;
        this.flowLastPointTime = ticks;
      }
      else
      {
        this.flowRunner.Interval = 10;
        this.flowLastPointTime = 0L;
        if (this.flowMinDelta.IsEmpty)
        {
          float num = 1000f / (float) this.flowRunner.Interval;
          this.flowMinDelta.X = this.flowMouseDelta.X / num;
          this.flowMinDelta.Y = this.flowMouseDelta.Y / num;
        }
        ImagePartInfo imageVisiblePart = this.ImageVisiblePart;
        float num1 = this.flowLastTime <= 0L ? (float) this.flowRunner.Interval : (float) (ticks - this.flowLastTime);
        this.flowMouseDelta.X -= this.flowMinDelta.X;
        this.flowMouseDelta.Y -= this.flowMinDelta.Y;
        this.flowMouseDelta.X = (double) this.flowMinDelta.X > 0.0 ? Math.Max(0.0f, this.flowMouseDelta.X) : Math.Min(0.0f, this.flowMouseDelta.X);
        this.flowMouseDelta.Y = (double) this.flowMinDelta.Y > 0.0 ? Math.Max(0.0f, this.flowMouseDelta.Y) : Math.Min(0.0f, this.flowMouseDelta.Y);
        if (this.flowMouseDelta.IsEmpty)
        {
          this.flowRunner.Stop();
          this.flowRunner.Interval = 25;
        }
        else
        {
          System.Drawing.Point offset = new System.Drawing.Point((int) ((double) imageVisiblePart.Offset.X + (double) this.flowMouseDelta.X * (double) num1), (int) ((double) imageVisiblePart.Offset.Y + (double) this.flowMouseDelta.Y * (double) num1));
          this.SetVisiblePart(new ImagePartInfo(imageVisiblePart.Part, offset));
        }
        this.flowLastTime = ticks;
      }
    }

    public void FlipDisplayOption(ImageDisplayOptions mask)
    {
      this.SetDisplayOption(mask, !this.HasDisplayOption(mask));
    }

    public void SetDisplayOption(ImageDisplayOptions mask, bool on)
    {
      if (on)
        this.ImageDisplayOptions |= mask;
      else
        this.ImageDisplayOptions ^= mask;
    }

    public bool HasDisplayOption(ImageDisplayOptions mask)
    {
      return ImageDisplayControl.HasDisplayOption(this.ImageDisplayOptions, mask);
    }

    private void InitWindowsTouch()
    {
      try
      {
        this.gestureHandler = Factory.CreateHandler<GestureHandler>((Control) this);
        this.gestureHandler.DisableGutter = true;
        this.gestureHandler.RotateBegin += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_RotateBegin);
        this.gestureHandler.Rotate += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_Rotate);
        this.gestureHandler.ZoomBegin += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_ZoomBegin);
        this.gestureHandler.Zoom += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_Zoom);
        this.gestureHandler.PanBegin += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_PanBegin);
        this.gestureHandler.Pan += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_Pan);
        this.gestureHandler.PanEnd += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_PanEnd);
        this.gestureHandler.TwoFingerTap += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_TwoFingerTap);
        this.gestureHandler.PressAndTap += new EventHandler<Windows7.Multitouch.GestureEventArgs>(this.gestureHandler_PressAndTap);
      }
      catch (Exception ex)
      {
      }
    }

    private void gestureHandler_RotateBegin(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      this.GestureLocation = e.Location;
      this.OnGestureStart();
      this.gestureRotation = this.ImageRotation;
      this.gestureRotationStart = e.RotateAngle;
    }

    private void gestureHandler_Rotate(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      this.ImageRotation = this.gestureRotation.Add((int) (this.gestureRotationStart / Math.PI * 180.0 - e.RotateAngle / Math.PI * 180.0) + 45);
    }

    private void gestureHandler_ZoomBegin(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      this.GestureLocation = e.Location;
      this.OnGestureStart();
      this.gestureZoomStart = this.ImageZoom;
      this.zoomStart = true;
    }

    private void gestureHandler_Zoom(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      if (this.zoomStart)
      {
        this.zoomStart = false;
        this.zoomOffset = (float) e.ZoomFactor - 1f;
      }
      float num = ((float) e.ZoomFactor - this.zoomOffset) * this.gestureZoomStart;
      this.DoZoom(this.ClientToImage(this.GestureLocation), num.Clamp(1f, 8f));
    }

    private void gestureHandler_PanBegin(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      this.panLocation = e.Location;
      this.GestureLocation = e.Location;
      this.OnGestureStart();
      this.OnPanStart();
      if (this.MouseHandled)
        return;
      this.panStart = this.panLocation;
      this.panPart = this.ImageVisiblePart;
    }

    private void gestureHandler_Pan(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      if (this.ignoreEvent == e.LastBeginEvent)
        return;
      this.panLocation = e.Location;
      this.OnPan();
      if (this.MouseHandled || this.panStart.IsEmpty)
        return;
      if (this.Display.IsAllVisible && Math.Abs(e.PanVelocity.Width) > 2 && e.PanVelocity.Height == 0)
      {
        this.ignoreEvent = e.LastBeginEvent;
        if (e.PanVelocity.Width < 0)
          this.OnGesture(new cYo.Projects.ComicRack.Engine.Display.GestureEventArgs(GestureType.FlickLeft));
        else
          this.OnGesture(new cYo.Projects.ComicRack.Engine.Display.GestureEventArgs(GestureType.FlickRight));
      }
      else
      {
        System.Drawing.Point image1 = this.ClientToImage(this.panStart);
        System.Drawing.Point image2 = this.ClientToImage(this.panLocation);
        this.SetVisiblePart(new ImagePartInfo(this.panPart.Part, new System.Drawing.Point(this.panPart.Offset.X + (image1.X - image2.X), this.panPart.Offset.Y + (image1.Y - image2.Y))));
      }
    }

    private void gestureHandler_PanEnd(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      this.panLocation = e.Location;
      this.OnPanEnd();
      this.panStart = System.Drawing.Point.Empty;
    }

    private void gestureHandler_PressAndTap(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      this.GestureLocation = this.panLocation = e.Location;
      if (e.IsBegin)
      {
        this.OnGestureStart();
        this.OnGesture(new cYo.Projects.ComicRack.Engine.Display.GestureEventArgs(GestureType.PressAndTap));
        this.OnPanStart();
      }
      else if (e.IsEnd)
        this.OnPanEnd();
      else
        this.OnPan();
    }

    private void gestureHandler_TwoFingerTap(object sender, Windows7.Multitouch.GestureEventArgs e)
    {
      this.GestureLocation = this.panLocation = e.Location;
      if (!e.IsBegin)
        return;
      this.OnGestureStart();
      this.OnGesture(new cYo.Projects.ComicRack.Engine.Display.GestureEventArgs(GestureType.TwoFingerTap));
      this.OnPanStart();
      this.OnPan();
      this.OnPanEnd();
    }

    protected System.Drawing.Point GestureLocation { get; private set; }

    protected virtual void OnGestureStart()
    {
    }

    public static ImageDisplayControl.HardwareAccelerationType HardwareAcceleration
    {
      get => ImageDisplayControl.hardwareAcceleration;
      set => ImageDisplayControl.hardwareAcceleration = value;
    }

    public static TextureManagerSettings HardwareSettings => ImageDisplayControl.hardwareSettings;

    public static bool HasDisplayOption(ImageDisplayOptions option, ImageDisplayOptions mask)
    {
      return (option & mask) != 0;
    }

    public System.Drawing.Point PanLocation => this.panLocation;

    public event EventHandler PanStart;

    public event EventHandler PanEnd;

    public event EventHandler Pan;

    protected virtual void OnPanStart()
    {
      if (this.PanStart == null)
        return;
      this.PanStart((object) this, EventArgs.Empty);
    }

    protected virtual void OnPan()
    {
      if (this.Pan == null)
        return;
      this.Pan((object) this, EventArgs.Empty);
    }

    protected virtual void OnPanEnd()
    {
      if (this.PanEnd != null)
        this.PanEnd((object) this, EventArgs.Empty);
      this.panStart = System.Drawing.Point.Empty;
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.autoHideCursorTimer = new Timer(this.components);
      this.mouseClickTimer = new Timer(this.components);
      this.SuspendLayout();
      this.autoHideCursorTimer.Enabled = true;
      this.autoHideCursorTimer.Interval = 1000;
      this.autoHideCursorTimer.Tick += new EventHandler(this.AutoHideCursorTimerTick);
      this.mouseClickTimer.Interval = 250;
      this.mouseClickTimer.Tick += new EventHandler(this.mouseClickTimer_Tick);
      this.BackColor = Color.Black;
      this.Name = "BookView";
      this.ResumeLayout(false);
    }

    [Flags]
    public enum RenderType
    {
      None = 0,
      Background = 1,
      Image = 2,
      Effect = 4,
      Default = Effect | Image | Background, // 0x00000007
      WithoutBackground = Effect | Image, // 0x00000006
      WithoutEffect = Image | Background, // 0x00000003
    }

    public struct DisplayOutputConfig
    {
      private readonly System.Drawing.Size viewSize;
      private readonly System.Drawing.Size imageSize;
      private ImageRotation rotation;
      private readonly ImageFitMode imageDisplayMode;
      private readonly bool fitOnlyIfOversized;
      private readonly float imageZoom;
      private readonly float zoom;
      private readonly ImagePartInfo part;
      private readonly bool rightToLeftReading;
      private readonly RightToLeftReadingMode rightToLeftReadingMode;
      private readonly bool twoPageAutoScroll;
      public static readonly ImageDisplayControl.DisplayOutputConfig Empty = new ImageDisplayControl.DisplayOutputConfig(System.Drawing.Size.Empty, System.Drawing.Size.Empty, ImageFitMode.Original, true, RightToLeftReadingMode.FlipPages, false, ImagePartInfo.Empty, 1f, 1f, ImageRotation.None, true);

      public DisplayOutputConfig(
        System.Drawing.Size viewSize,
        System.Drawing.Size imageSize,
        ImageFitMode imageDisplayMode,
        bool fitOnlyIfOversized,
        RightToLeftReadingMode rightToLeftReadingMode,
        bool rightToLeftReading,
        ImagePartInfo part,
        float imageZoom,
        float zoom,
        ImageRotation rotation,
        bool twoPageAutoScroll)
      {
        this.viewSize = viewSize;
        this.imageSize = imageSize;
        this.rotation = rotation;
        this.imageZoom = imageZoom;
        this.zoom = zoom;
        this.imageDisplayMode = imageDisplayMode;
        this.fitOnlyIfOversized = fitOnlyIfOversized;
        this.part = part;
        this.rightToLeftReadingMode = rightToLeftReadingMode;
        this.rightToLeftReading = rightToLeftReading;
        this.twoPageAutoScroll = twoPageAutoScroll;
      }

      public System.Drawing.Size ViewSize => this.viewSize;

      public System.Drawing.Size ImageSize => this.imageSize;

      public ImageRotation Rotation
      {
        get => this.rotation;
        set => this.rotation = value;
      }

      public ImageFitMode ImageDisplayMode => this.imageDisplayMode;

      public bool FitOnlyIfOversized => this.fitOnlyIfOversized;

      public float ImageZoom => this.imageZoom;

      public float Zoom => this.zoom;

      public ImagePartInfo Part => this.part;

      public bool RightToLeftReading => this.rightToLeftReading;

      public RightToLeftReadingMode RightToLeftReadingMode => this.rightToLeftReadingMode;

      public bool TwoPageAutoScroll => this.twoPageAutoScroll;

      public bool IsEmpty => this.viewSize.IsEmpty || this.imageSize.IsEmpty;
    }

    public class DisplayOutput : DisposableObject, ICloneable
    {
      private ImageDisplayControl.DisplayOutputConfig config;
      private System.Drawing.Drawing2D.Matrix transform;
      private SizeF scale = ImageDisplayControl.DisplayOutput.CreateScale(1f);
      private Rectangle partBounds;
      private int part;
      private int partCount;
      private Rectangle[] parts = new Rectangle[0];

      private DisplayOutput()
      {
      }

      private DisplayOutput(ImageDisplayControl.DisplayOutput copy)
        : this()
      {
        this.config = copy.config;
        this.transform = ImageDisplayControl.DisplayOutput.SafeMatrixCopy(copy.transform);
        this.scale = copy.scale;
        this.partBounds = copy.partBounds;
        this.part = copy.part;
        this.parts = copy.parts;
        this.partCount = copy.partCount;
        this.ImageZoom = copy.ImageZoom;
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing && this.transform != null)
          this.transform.Dispose();
        base.Dispose(disposing);
      }

      public ImageDisplayControl.DisplayOutputConfig Config => this.config;

      public System.Drawing.Drawing2D.Matrix Transform => this.transform;

      public SizeF Scale => this.scale;

      public Rectangle PartBounds => this.partBounds;

      public Rectangle OutputBounds => this.PartBounds.Size.ToRectangle();

      public Rectangle OutputBoundsScreen
      {
        get
        {
          try
          {
            System.Drawing.Point[] points = this.OutputBounds.ToPoints();
            this.Transform.TransformPoints(points);
            return ((IEnumerable<System.Drawing.Point>) points).ToRectangle();
          }
          catch (Exception ex)
          {
            return Rectangle.Empty;
          }
        }
      }

      public int Part => this.part;

      public int PartCount => this.partCount;

      public float ImageZoom { get; private set; }

      public bool PartIsStart => this.IsStartPart(this.partBounds);

      public bool PartIsEnd => this.IsEndPart(this.partBounds);

      public bool IsEmpty => this.OutputBounds.IsEmpty || this.Transform == null;

      public bool IsAllVisible => this.parts.Length < 2;

      public Rectangle GetPart(int index)
      {
        return this.parts.Length == 0 ? Rectangle.Empty : this.parts[index.Clamp(0, this.parts.Length - 1)];
      }

      public System.Drawing.Point GetPartOffset(int partIndex, System.Drawing.Point offset)
      {
        return ImageDisplayControl.DisplayOutput.GetClampedPartOffset(offset, this.config.ImageSize, this.GetPart(partIndex));
      }

      public Rectangle GetPart(int partIndex, System.Drawing.Point offset)
      {
        Rectangle part = this.GetPart(partIndex);
        part.Offset(this.GetPartOffset(partIndex, offset));
        return part;
      }

      public Rectangle GetPart(ImagePartInfo ipi) => this.GetPart(ipi.Part, ipi.Offset);

      public bool IsStartPart(Rectangle part)
      {
        return ImageDisplayControl.DisplayOutput.RectangleEquals(part, this.GetPart(0));
      }

      public bool IsEndPart(Rectangle part)
      {
        return ImageDisplayControl.DisplayOutput.RectangleEquals(part, this.GetPart(this.PartCount));
      }

      public bool IsEndPart(ImagePartInfo ipi) => this.IsEndPart(this.GetPart(ipi));

      public bool IsStartPart(ImagePartInfo ipi) => this.IsStartPart(this.GetPart(ipi));

      public Rectangle[] GetParts(int startIndex, int endIndex)
      {
        if (startIndex >= endIndex)
          return new Rectangle[0];
        startIndex = Math.Max(startIndex, 0);
        endIndex = Math.Min(endIndex, this.parts.Length);
        Rectangle[] parts = new Rectangle[endIndex - startIndex];
        for (int index = startIndex; index < endIndex; ++index)
          parts[index - startIndex] = this.parts[index];
        return parts;
      }

      public Rectangle[] GetParts(int startIndex) => this.GetParts(startIndex, this.parts.Length);

      public ImagePartInfo GetBestPartFit(ImagePartInfo newPart)
      {
        int num = newPart.Part;
        System.Drawing.Point offset = newPart.Offset;
        if (num < 0)
        {
          offset = System.Drawing.Point.Empty;
          num = 0;
        }
        if (num >= this.PartCount)
        {
          offset = System.Drawing.Point.Empty;
          num = this.PartCount - 1;
        }
        if (num < 0)
          return newPart;
        System.Drawing.Point partOffset = this.GetPartOffset(num, offset);
        Rectangle part1 = this.GetPart(num);
        part1.Offset(partOffset);
        int index = ((IEnumerable<Rectangle>) this.GetParts(0)).IndexOfBestFit(part1);
        if (num != index)
        {
          Rectangle part2 = this.GetPart(num);
          Rectangle part3 = this.GetPart(index);
          partOffset.Offset(part2.Location);
          partOffset.Offset(-part3.X, -part3.Y);
          num = index;
        }
        return new ImagePartInfo(num, partOffset);
      }

      private static System.Drawing.Drawing2D.Matrix SafeMatrixCopy(System.Drawing.Drawing2D.Matrix matrix)
      {
        if (matrix == null)
          return (System.Drawing.Drawing2D.Matrix) null;
        try
        {
          float[] elements = matrix.Elements;
          return new System.Drawing.Drawing2D.Matrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
        }
        catch
        {
          return (System.Drawing.Drawing2D.Matrix) null;
        }
      }

      private static bool Eps(int a, int b) => Math.Abs(a - b) < 2;

      private static bool RectangleEquals(Rectangle part, Rectangle rectangle)
      {
        return ImageDisplayControl.DisplayOutput.Eps(part.X, rectangle.X) && ImageDisplayControl.DisplayOutput.Eps(part.Y, rectangle.Y) && ImageDisplayControl.DisplayOutput.Eps(part.Right, rectangle.Right) && ImageDisplayControl.DisplayOutput.Eps(part.Bottom, rectangle.Bottom);
      }

      public static ImageDisplayControl.DisplayOutput Create(
        ImageDisplayControl.DisplayOutputConfig dp,
        float anamorphicTolerance)
      {
        ImageDisplayControl.DisplayOutput displayOutput = new ImageDisplayControl.DisplayOutput();
        int degrees = dp.Rotation.ToDegrees();
        displayOutput.config = dp;
        displayOutput.ImageZoom = dp.ImageZoom;
        if (dp.ImageSize.Width != 0)
        {
          if (dp.ImageSize.Height != 0)
          {
            try
            {
              System.Drawing.Size size;
              using (System.Drawing.Drawing2D.Matrix rotationMatrix = MatrixUtility.GetRotationMatrix(dp.ViewSize, degrees))
                size = new Rectangle(System.Drawing.Point.Empty, dp.ViewSize).Rotate(rotationMatrix).Size;
              SizeF scale = ImageDisplayControl.DisplayOutput.GetScale(size, dp.ImageSize, dp.ImageDisplayMode, dp.FitOnlyIfOversized, dp.Zoom, anamorphicTolerance);
              System.Drawing.Size partGridSize = ImageDisplayControl.DisplayOutput.GetPartGridSize(size, dp.ImageSize, scale, !dp.TwoPageAutoScroll);
              int length = partGridSize.Width * partGridSize.Height;
              int index = dp.Part.Part.Clamp(0, length - 1);
              Rectangle[] rectangleArray = new Rectangle[length];
              for (int part = 0; part < length; ++part)
                rectangleArray[part] = ImageDisplayControl.DisplayOutput.GetPartRectangle(size, dp.ImageSize, scale, part, partGridSize, !dp.TwoPageAutoScroll, dp.RightToLeftReadingMode, dp.RightToLeftReading);
              Rectangle partRectangle = rectangleArray[index];
              partRectangle.Offset(ImageDisplayControl.DisplayOutput.GetClampedPartOffset(dp.Part.Offset, dp.ImageSize, partRectangle));
              System.Drawing.Drawing2D.Matrix rotationMatrix1 = MatrixUtility.GetRotationMatrix(partRectangle.Size, degrees);
              Rectangle rectangle = new Rectangle(0, 0, partRectangle.Width, partRectangle.Height).Rotate(rotationMatrix1);
              rotationMatrix1.Translate((float) -rectangle.X, (float) -rectangle.Y, MatrixOrder.Append);
              rotationMatrix1.Scale(scale.Width, scale.Height, MatrixOrder.Append);
              rotationMatrix1.Translate((float) (((double) dp.ViewSize.Width - (double) rectangle.Width * (double) scale.Width) / 2.0), (float) (((double) dp.ViewSize.Height - (double) rectangle.Height * (double) scale.Height) / 2.0), MatrixOrder.Append);
              displayOutput.transform = rotationMatrix1;
              displayOutput.scale = scale;
              displayOutput.partBounds = partRectangle;
              displayOutput.parts = rectangleArray;
              displayOutput.part = index;
              displayOutput.partCount = rectangleArray.Length;
            }
            catch
            {
            }
          }
        }
        return displayOutput;
      }

      public static ImageDisplayControl.DisplayOutput Interpolate(
        ImageDisplayControl.DisplayOutput a,
        ImageDisplayControl.DisplayOutput b,
        float p)
      {
        try
        {
          ImageDisplayControl.DisplayOutput displayOutput = new ImageDisplayControl.DisplayOutput()
          {
            part = b.part,
            parts = b.parts,
            config = b.config,
            scale = a.scale + new SizeF((b.scale.Width - a.scale.Width) * p, (b.scale.Height - a.scale.Height) * p),
            ImageZoom = a.ImageZoom + (b.ImageZoom - a.ImageZoom) * p
          };
          float num1 = p * (b.ImageZoom / displayOutput.ImageZoom);
          float num2 = (float) (b.partBounds.X - a.PartBounds.X) * num1;
          float num3 = (float) (b.partBounds.Y - a.PartBounds.Y) * num1;
          float num4 = (float) (b.partBounds.Width - a.PartBounds.Width) * num1;
          float num5 = (float) (b.partBounds.Height - a.PartBounds.Height) * num1;
          displayOutput.partBounds.X = (int) ((double) a.partBounds.X + (double) num2);
          displayOutput.partBounds.Y = (int) ((double) a.partBounds.Y + (double) num3);
          displayOutput.partBounds.Width = (int) ((double) a.partBounds.Width + (double) num4);
          displayOutput.partBounds.Height = (int) ((double) a.partBounds.Height + (double) num5);
          float[] elements1 = a.transform.Elements;
          float[] elements2 = b.transform.Elements;
          float[] numArray = new float[elements1.Length];
          for (int index = 0; index < elements1.Length; ++index)
            numArray[index] = elements1[index] + (elements2[index] - elements1[index]) * p;
          displayOutput.transform = new System.Drawing.Drawing2D.Matrix(numArray[0], numArray[1], numArray[2], numArray[3], numArray[4], numArray[5]);
          return displayOutput;
        }
        catch (Exception ex)
        {
          return b;
        }
      }

      private static SizeF CreateScale(float scale) => new SizeF(scale, scale);

      private static SizeF GetScale(
        System.Drawing.Size clientSize,
        System.Drawing.Size bitmapSize,
        ImageFitMode pageDisplayMode,
        bool onlyFitOversized,
        float zoom,
        float anamorphicTolerance)
      {
        bool flag = bitmapSize.Width > bitmapSize.Height;
        float width = (float) bitmapSize.Width;
        float height = (float) bitmapSize.Height;
        float num1 = (float) clientSize.Width / width * zoom;
        float num2 = (float) clientSize.Height / height * zoom;
        float num3 = zoom;
        SizeF scale = new SizeF(num3, num3);
        switch (pageDisplayMode)
        {
          case ImageFitMode.Fit:
            if (onlyFitOversized && clientSize.Height > bitmapSize.Height && clientSize.Width > bitmapSize.Width)
              return scale;
            float t1 = Math.Min(num1, num2);
            if (!num2.CompareTo(t1, t1 * anamorphicTolerance))
              num2 = t1;
            if (!num1.CompareTo(t1, t1 * anamorphicTolerance))
              num1 = t1;
            return new SizeF(num1, num2);
          case ImageFitMode.FitWidth:
            if (onlyFitOversized && clientSize.Width > bitmapSize.Width)
              return scale;
            return num2.CompareTo(num1, num1 * anamorphicTolerance) ? new SizeF(num1, num2) : ImageDisplayControl.DisplayOutput.CreateScale(num1);
          case ImageFitMode.FitWidthAdaptive:
            if (onlyFitOversized && clientSize.Width > bitmapSize.Width)
              return scale;
            if (flag)
              num1 *= 2f;
            return num2.CompareTo(num1, num1 * anamorphicTolerance) ? new SizeF(num1, num2) : ImageDisplayControl.DisplayOutput.CreateScale(num1);
          case ImageFitMode.FitHeight:
            if (onlyFitOversized && clientSize.Height > bitmapSize.Height)
              return scale;
            return num1.CompareTo(num2, num2 * anamorphicTolerance) ? new SizeF(num1, num2) : ImageDisplayControl.DisplayOutput.CreateScale(num2);
          case ImageFitMode.BestFit:
            if (onlyFitOversized && (clientSize.Height > bitmapSize.Height || clientSize.Width > bitmapSize.Width))
              return scale;
            float t2 = Math.Max(num1, num2);
            if (!num2.CompareTo(t2, t2 * anamorphicTolerance))
              num2 = t2;
            if (!num1.CompareTo(t2, t2 * anamorphicTolerance))
              num1 = t2;
            return new SizeF(num1, num2);
          default:
            return scale;
        }
      }

      private static System.Drawing.Size GetPartGridSize(
        System.Drawing.Size clientSize,
        System.Drawing.Size bitmapSize,
        SizeF realZoom,
        bool doubleSpread)
      {
        int num1 = (int) ((double) bitmapSize.Height * (double) realZoom.Height);
        int num2 = (int) ((double) bitmapSize.Width * (double) realZoom.Width);
        if (clientSize.Width == 0 || clientSize.Height == 0)
          return new System.Drawing.Size(1, 1);
        if (num1 <= clientSize.Height && num2 <= clientSize.Width)
          return new System.Drawing.Size(1, 1);
        return !doubleSpread || num1 > num2 ? new System.Drawing.Size((num2 - 1) / clientSize.Width + 1, (num1 - 1) / clientSize.Height + 1) : new System.Drawing.Size(((num2 / 2 - 1) / clientSize.Width + 1) * 2, (num1 - 1) / clientSize.Height + 1);
      }

      private static Rectangle GetPartRectangle(
        System.Drawing.Size clientSize,
        System.Drawing.Size bitmapSize,
        SizeF realZoom,
        int part,
        System.Drawing.Size partGrid,
        bool doubleSpread,
        RightToLeftReadingMode rightToLeftReadingMode,
        bool rightToLeftReading)
      {
        bool flag = bitmapSize.Width > bitmapSize.Height;
        int num1 = partGrid.Width * partGrid.Height;
        part = part.Clamp(0, num1 - 1);
        if (doubleSpread & flag)
        {
          System.Drawing.Size partGridSize = ImageDisplayControl.DisplayOutput.GetPartGridSize(clientSize, bitmapSize, realZoom, false);
          int num2 = (partGridSize.Width - 1) / 2 + 1;
          int num3 = num2 * partGridSize.Height;
          int num4 = part % num3;
          int num5 = part / num3 * (partGridSize.Width / 2);
          int part1 = partGridSize.Width * (num4 / num2) + num4 % num2 + num5;
          Rectangle partRectangle = ImageDisplayControl.DisplayOutput.GetPartRectangle(clientSize, bitmapSize, realZoom, part1, partGridSize, false, rightToLeftReadingMode, false);
          if (rightToLeftReading)
          {
            if (rightToLeftReadingMode == RightToLeftReadingMode.FlipParts)
            {
              partRectangle.X = bitmapSize.Width - partRectangle.Right;
              if (part < num3)
              {
                if (partRectangle.X < bitmapSize.Width / 2)
                  partRectangle.X = bitmapSize.Width / 2;
                if (partRectangle.Right > bitmapSize.Width)
                  partRectangle.X -= partRectangle.Right - bitmapSize.Width;
              }
              else
              {
                if (partRectangle.Right > bitmapSize.Width / 2)
                  partRectangle.X -= partRectangle.Right - bitmapSize.Width / 2;
                if (partRectangle.X < 0)
                  partRectangle.X = 0;
              }
            }
            else if (part < num3)
            {
              partRectangle.X = bitmapSize.Width / 2 - partRectangle.Right;
              if (partRectangle.X < 0)
                partRectangle.X = 0;
            }
            else
            {
              partRectangle.X -= bitmapSize.Width / 2;
              partRectangle.X = bitmapSize.Width / 2 - partRectangle.Right;
              partRectangle.X += bitmapSize.Width / 2;
              if (partRectangle.Right > bitmapSize.Width)
                partRectangle.X -= partRectangle.Right - bitmapSize.Width;
            }
          }
          else if (part < num3)
          {
            if (partRectangle.Right > bitmapSize.Width / 2)
              partRectangle.X -= partRectangle.Right - bitmapSize.Width / 2;
            if (partRectangle.X < 0)
              partRectangle.X = 0;
          }
          else
          {
            if (partRectangle.X < bitmapSize.Width / 2)
              partRectangle.X = bitmapSize.Width / 2;
            if (partRectangle.Right > bitmapSize.Width)
              partRectangle.X -= partRectangle.Right - bitmapSize.Width;
          }
          return partRectangle;
        }
        clientSize.Width = (int) ((double) clientSize.Width / (double) realZoom.Width);
        clientSize.Height = (int) ((double) clientSize.Height / (double) realZoom.Height);
        int num6 = bitmapSize.Height / partGrid.Height;
        Rectangle rect = new Rectangle(bitmapSize.Width / partGrid.Width * (part % partGrid.Width), num6 * (part / partGrid.Width), clientSize.Width, clientSize.Height);
        Rectangle partRectangle1 = ImageDisplayControl.DisplayOutput.Clamp(bitmapSize, rect);
        if (rightToLeftReading)
          partRectangle1.X = bitmapSize.Width - partRectangle1.Right;
        return partRectangle1;
      }

      private static System.Drawing.Point GetClampedPartOffset(
        System.Drawing.Point offset,
        System.Drawing.Size imageSize,
        Rectangle partRectangle)
      {
        int x = offset.X;
        int y = offset.Y;
        if (x + partRectangle.Right > imageSize.Width)
          x = imageSize.Width - partRectangle.Right;
        if (y + partRectangle.Bottom > imageSize.Height)
          y = imageSize.Height - partRectangle.Bottom;
        if (x + partRectangle.Left < 0)
          x = -partRectangle.Left;
        if (y + partRectangle.Top < 0)
          y = -partRectangle.Top;
        return new System.Drawing.Point(x, y);
      }

      private static Rectangle Clamp(System.Drawing.Size size, Rectangle rect)
      {
        if (rect.Right > size.Width)
          rect.X = size.Width - rect.Width;
        if (rect.Bottom > size.Height)
          rect.Y = size.Height - rect.Height;
        if (rect.Y < 0)
        {
          rect.Height += rect.Y;
          rect.Y = 0;
        }
        if (rect.X < 0)
        {
          rect.Width += rect.X;
          rect.X = 0;
        }
        return rect;
      }

      public override bool Equals(object obj)
      {
        if (!(obj is ImageDisplayControl.DisplayOutput displayOutput) || !object.Equals((object) this.Config, (object) displayOutput.Config) || this.part != displayOutput.part || this.partBounds != displayOutput.partBounds || this.partCount != displayOutput.partCount || this.scale != displayOutput.scale)
          return false;
        return this.transform == null && displayOutput.transform == null || object.Equals((object) this.transform, (object) displayOutput.transform);
      }

      public override int GetHashCode() => base.GetHashCode();

      public object Clone() => (object) new ImageDisplayControl.DisplayOutput(this);
    }

    public class GestureArea
    {
      public GestureArea(ContentAlignment alignment, Rectangle area)
      {
        this.Alignment = alignment;
        this.Area = area;
      }

      public ContentAlignment Alignment { get; set; }

      public Rectangle Area { get; set; }
    }

    public class RenderEventArgs : EventArgs
    {
      private readonly IBitmapRenderer renderer;
      private readonly ImageDisplayControl.DisplayOutput display;

      public RenderEventArgs(IBitmapRenderer renderer, ImageDisplayControl.DisplayOutput display)
      {
        this.renderer = renderer;
        this.display = display;
      }

      public IBitmapRenderer Graphics => this.renderer;

      public ImageDisplayControl.DisplayOutput Display => this.display;
    }

    public enum HardwareAccelerationType
    {
      Disabled,
      Enabled,
      Forced,
    }
  }
}

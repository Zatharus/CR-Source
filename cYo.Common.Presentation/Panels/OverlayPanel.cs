// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.OverlayPanel
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class OverlayPanel : DisposableObject
  {
    private Bitmap surface;
    private readonly object surfaceLock = new object();
    private volatile OverlayPanelCollection panels;
    private volatile object tag;
    private volatile bool enabled = true;
    private volatile bool alignmentEnabled;
    private volatile ContentAlignment alignment = ContentAlignment.MiddleCenter;
    private Point alignmentOffset;
    private bool ignoreParentMargin;
    private Padding margin = new Padding(4);
    private volatile bool visible = true;
    private volatile bool destroyAfterCompletion;
    private volatile float opacity = 1f;
    private volatile float saturation;
    private volatile float contrast;
    private volatile float brightness;
    private volatile float scale = 1f;
    private Point location;
    private Size size;
    private readonly AnimatorCollection animators = new AnimatorCollection();
    private PanelState panelState;
    private HitTestType hitTestType = HitTestType.Alpha;
    private Color backgroundColor = Color.Transparent;
    private OverlayManager manager;
    private volatile OverlayPanel parent;
    private readonly List<Rectangle> invalidatedBounds = new List<Rectangle>();
    private bool dirty;

    public OverlayPanel()
    {
      this.Animators.Changed += new EventHandler<SmartListChangedEventArgs<Animator>>(this.Animators_Changed);
    }

    public OverlayPanel(Size size)
      : this()
    {
      this.size = size;
    }

    public OverlayPanel(int width, int height)
      : this(new Size(width, height))
    {
    }

    public OverlayPanel(Image image)
      : this()
    {
      this.surface = image.CreateCopy();
    }

    public OverlayPanel(Image image, ContentAlignment alignment)
      : this(image)
    {
      this.alignment = alignment;
    }

    public OverlayPanel(int width, int height, ContentAlignment alignment)
      : this(width, height)
    {
      this.alignment = alignment;
    }

    public OverlayPanel(
      int width,
      int height,
      ContentAlignment alignment,
      IEnumerable<Animator> animators)
      : this(width, height, alignment)
    {
      this.Animators.AddRange(animators);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        using (ItemMonitor.Lock(this.surfaceLock))
        {
          if (this.surface != null)
            this.surface.Dispose();
          this.surface = (Bitmap) null;
        }
      }
      base.Dispose(disposing);
    }

    public OverlayPanelCollection Panels
    {
      get
      {
        if (this.panels == null)
        {
          this.panels = new OverlayPanelCollection();
          this.panels.Changed += new EventHandler<SmartListChangedEventArgs<OverlayPanel>>(this.overlayPanels_Changed);
        }
        return this.panels;
      }
    }

    public object Tag
    {
      get => this.tag;
      set => this.tag = value;
    }

    public bool Enabled
    {
      get => this.enabled;
      set => this.enabled = value;
    }

    public bool AutoAlign
    {
      get => this.alignmentEnabled;
      set
      {
        if (this.alignmentEnabled == value)
          return;
        this.alignmentEnabled = value;
        this.InvalidatePanel();
        this.OnAlignmentChanged();
      }
    }

    public ContentAlignment Alignment
    {
      get => this.alignment;
      set
      {
        if (this.alignment == value)
          return;
        this.alignment = value;
        this.InvalidatePanel();
        this.OnAlignmentChanged();
      }
    }

    public Point AlignmentOffset
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.alignmentOffset;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.alignmentOffset == value)
            return;
          this.alignmentOffset = value;
          this.InvalidatePanel();
          this.OnAlignmentChanged();
        }
      }
    }

    public bool IgnoreParentMargin
    {
      get => this.ignoreParentMargin;
      set
      {
        if (this.ignoreParentMargin == value)
          return;
        this.ignoreParentMargin = value;
        this.InvalidatePanel();
        this.OnAlignmentChanged();
      }
    }

    public Padding Margin
    {
      get => this.margin;
      set
      {
        if (this.margin == value)
          return;
        this.margin = value;
        this.InvalidatePanel();
        this.OnMarginChanged();
      }
    }

    public bool Visible
    {
      get => this.visible;
      set
      {
        if (this.visible == value)
          return;
        this.visible = value;
        this.InvalidatePanel(true);
        this.OnVisibleChanged();
      }
    }

    public bool IsVisible => this.visible && (double) this.opacity > 0.05000000074505806;

    public bool DestroyAfterCompletion
    {
      get => this.destroyAfterCompletion;
      set => this.destroyAfterCompletion = value;
    }

    public float Opacity
    {
      get => this.opacity;
      set
      {
        value = value.Clamp(0.0f, 1f);
        if ((double) this.opacity == (double) value)
          return;
        this.opacity = value;
        this.OnOpacityChanged();
      }
    }

    public float Saturation
    {
      get => this.saturation;
      set
      {
        if ((double) this.saturation == (double) value)
          return;
        this.saturation = value;
        this.OnSaturationChanged();
      }
    }

    public float Contrast
    {
      get => this.contrast;
      set
      {
        if ((double) this.contrast == (double) value)
          return;
        this.contrast = value;
        this.OnContrastChanged();
      }
    }

    public float Brightness
    {
      get => this.brightness;
      set
      {
        if ((double) this.brightness == (double) value)
          return;
        this.brightness = value;
        this.OnBrightnessChanged();
      }
    }

    public float Scale
    {
      get => this.scale;
      set
      {
        if ((double) this.scale == (double) value)
          return;
        this.FireInvalidate(this.Bounds, false);
        this.scale = value;
        this.FireInvalidate(this.Bounds, false);
        this.OnScaleChanged();
      }
    }

    public Point Location
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.location;
      }
      set
      {
        if (this.Location == value)
          return;
        Point location;
        using (ItemMonitor.Lock((object) this))
        {
          location = this.Location;
          this.location = value;
        }
        this.FireInvalidate(new Rectangle(location, this.GetScaledSize(this.Size)), false);
        this.FireInvalidate(new Rectangle(value, this.GetScaledSize(this.Size)), false);
        this.OnLocationChanged();
      }
    }

    public Point CenterLocation
    {
      get
      {
        Point location = this.Location;
        location.Offset(this.Width / 2, this.Height / 2);
        return location;
      }
      set
      {
        value.Offset(-this.Width / 2, -this.Height / 2);
        this.Location = value;
      }
    }

    public Size Size
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.size;
      }
      set
      {
        if (this.Size == value)
          return;
        using (ItemMonitor.Lock(this.surfaceLock))
        {
          if (this.surface != null)
          {
            if (this.surface.Size != value)
            {
              Bitmap bitmap = new Bitmap(value.Width, value.Height, PixelFormat.Format32bppArgb);
              using (Graphics graphics = Graphics.FromImage((Image) bitmap))
                graphics.DrawImage((Image) this.surface, new Rectangle(Point.Empty, value));
              this.surface.SafeDispose();
              this.surface = bitmap;
            }
          }
        }
        Size size;
        using (ItemMonitor.Lock((object) this))
        {
          size = this.Size;
          this.size = value;
        }
        this.FireInvalidate(new Rectangle(this.Location, this.GetScaledSize(size)), false);
        this.FireInvalidate(new Rectangle(this.Location, this.GetScaledSize(value)), false);
        this.OnSizeChanged();
      }
    }

    public Rectangle Bounds
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return new Rectangle(this.Location, this.GetScaledSize());
      }
    }

    public Rectangle PhysicalBounds
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return new Rectangle(this.Location, this.Size);
      }
    }

    public Rectangle ClientRectangle => new Rectangle(Point.Empty, this.Size);

    public Rectangle DisplayRectangle => this.ClientRectangle.Pad(this.margin);

    public int X
    {
      get => this.Location.X;
      set => this.Location = new Point(value, this.Y);
    }

    public int Y
    {
      get => this.Location.Y;
      set => this.Location = new Point(this.X, value);
    }

    public int Width
    {
      get => this.Size.Width;
      set => this.Size = new Size(value, this.Height);
    }

    public int Height
    {
      get => this.Size.Height;
      set => this.Size = new Size(this.Width, value);
    }

    public AnimatorCollection Animators => this.animators;

    public PanelState PanelState
    {
      get => this.panelState;
      set
      {
        if (this.panelState == value)
          return;
        this.panelState = value;
        this.OnPanelStateChanged();
      }
    }

    public HitTestType HitTestType
    {
      get => this.hitTestType;
      set => this.hitTestType = value;
    }

    public bool HasMouse
    {
      get
      {
        if (this.PanelState != PanelState.Normal)
          return true;
        return this.panels != null && this.panels.Find((Predicate<OverlayPanel>) (x => x.HasMouse)) != null;
      }
    }

    public Color BackgroundColor
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.backgroundColor;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.backgroundColor == value)
            return;
          this.backgroundColor = value;
        }
        this.Invalidate();
      }
    }

    public OverlayManager Manager
    {
      get
      {
        for (OverlayPanel overlayPanel = this; overlayPanel != null; overlayPanel = overlayPanel.Parent)
        {
          if (overlayPanel.manager != null)
            return overlayPanel.manager;
        }
        return (OverlayManager) null;
      }
      set => this.manager = value;
    }

    protected OverlayPanel Parent
    {
      get => this.parent;
      set => this.parent = value;
    }

    public OverlayPanel HitTest(Point pt)
    {
      if (!this.Enabled || !this.IsVisible)
        return (OverlayPanel) null;
      if (this.panels != null)
      {
        Point pt1 = new Point(pt.X - this.X, pt.Y - this.Y);
        foreach (OverlayPanel overlayPanel1 in ((IEnumerable<OverlayPanel>) this.panels.ToArray()).Reverse<OverlayPanel>())
        {
          OverlayPanel overlayPanel2 = overlayPanel1.HitTest(pt1);
          if (overlayPanel2 != null)
            return overlayPanel2;
        }
      }
      if (this.hitTestType != HitTestType.Disabled && this.Bounds.Contains(pt))
      {
        using (ItemMonitor.Lock(this.surfaceLock))
        {
          if (this.HitTestType == HitTestType.Bounds)
            return this;
          if (this.surface == null)
            return (OverlayPanel) null;
          int x = (int) ((double) (pt.X - this.X) / (double) this.scale);
          int y = (int) ((double) (pt.Y - this.Y) / (double) this.scale);
          if (x >= 0)
          {
            if (y >= 0)
            {
              if (x < this.surface.Width)
              {
                if (y < this.surface.Height)
                {
                  if ((double) this.surface.GetPixel(x, y).A * (double) this.Opacity > 0.0)
                    return this;
                }
              }
            }
          }
        }
      }
      return (OverlayPanel) null;
    }

    public void Align(Rectangle rc, ContentAlignment alignment)
    {
      Point location = new Rectangle(rc.Location, this.Bounds.Size).Align(rc, alignment).Location;
      Point alignmentOffset = this.AlignmentOffset;
      location.Offset((int) ((double) alignmentOffset.X * (double) this.Scale), (int) ((double) alignmentOffset.Y * (double) this.Scale));
      this.Location = location;
    }

    public PanelSurface CreateSurface(bool empty)
    {
      using (ItemMonitor.Lock(this.surfaceLock))
      {
        Bitmap bmp = this.surface == null | empty ? new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb) : this.surface.Clone() as Bitmap;
        PanelSurface surface = new PanelSurface(bmp);
        surface.Disposed += (EventHandler) ((o, e) =>
        {
          using (ItemMonitor.Lock(this.surfaceLock))
          {
            if (this.surface != null)
              this.surface.Dispose();
            this.surface = bmp;
          }
          this.Size = bmp.Size;
        });
        return surface;
      }
    }

    public PanelSurface CreateSurface() => this.CreateSurface(false);

    public virtual void Draw(IBitmapRenderer gr, Rectangle rc)
    {
      if (gr == null)
        throw new ArgumentNullException();
      if (!this.Visible)
        return;
      try
      {
        Rectangle bounds = this.Bounds;
        if (gr.IsVisible((RectangleF) bounds))
        {
          this.OnDrawing();
          using (ItemMonitor.Lock(this.surfaceLock))
          {
            if (this.surface != null)
              gr.DrawImage((RendererImage) this.surface, (RectangleF) bounds, (RectangleF) new Rectangle(0, 0, this.surface.Width, this.surface.Height), new BitmapAdjustment(this.saturation, this.brightness, this.contrast), this.opacity);
          }
          using (gr.SaveState())
          {
            gr.TranslateTransform((float) bounds.Location.X, (float) bounds.Location.Y);
            gr.ScaleTransform(this.scale, this.scale);
            this.OnRenderSurface(new PanelRenderEventArgs(gr));
          }
        }
      }
      catch
      {
        throw;
      }
      if (this.panels == null)
        return;
      using (gr.SaveState())
      {
        gr.TranslateTransform((float) this.X, (float) this.Y);
        this.panels.ForEach((Action<OverlayPanel>) (p => p.Draw(gr, rc)), true);
      }
    }

    public bool Animate()
    {
      bool stillRunning = false;
      this.animators.ForEach((Action<Animator>) (a => stillRunning |= a.Animate(this)), true);
      if (this.panels != null)
        this.panels.ForEach((Action<OverlayPanel>) (p => stillRunning |= p.Animate()), true);
      return stillRunning;
    }

    public void FireMouseEnter(MouseEventArgs e)
    {
      this.OnMouseEnter(this.GetMouseEventArgsScaled(e));
    }

    public void FireMouseLeave(MouseEventArgs e)
    {
      this.OnMouseLeave(this.GetMouseEventArgsScaled(e));
    }

    public void FireMouseMove(MouseEventArgs e)
    {
      this.OnMouseMove(this.GetMouseEventArgsScaled(e));
    }

    public void FireMouseDown(MouseEventArgs e)
    {
      this.OnMouseDown(this.GetMouseEventArgsScaled(e));
    }

    public void FireMouseUp(MouseEventArgs e) => this.OnMouseUp(this.GetMouseEventArgsScaled(e));

    public void FireClick() => this.OnClick();

    public void FireDoubleClick() => this.OnDoubleClick();

    public Point GetAbsoluteLocation()
    {
      OverlayPanel parent = this.Parent;
      Point location = this.Location;
      for (; parent != null; parent = parent.Parent)
        location.Offset(parent.Location);
      return location;
    }

    public void Fill(Color color)
    {
      using (PanelSurface surface = this.CreateSurface())
        surface.Graphics.Clear(color);
    }

    internal void InvalidatePanel() => this.InvalidatePanel(false);

    private void InvalidatePanel(bool always) => this.InvalidatePanel(this.ClientRectangle, always);

    private void InvalidatePanel(Rectangle bounds, bool always)
    {
      Rectangle panelBounds = bounds;
      panelBounds.Offset(this.Location);
      this.FireInvalidate(panelBounds, always);
    }

    private Size GetScaledSize(Size sz)
    {
      return new Size((int) ((double) sz.Width * (double) this.scale), (int) ((double) sz.Height * (double) this.scale));
    }

    private MouseEventArgs GetMouseEventArgsScaled(MouseEventArgs e)
    {
      int x = (int) ((double) e.X / (double) this.scale);
      int y = (int) ((double) e.Y / (double) this.scale);
      return new MouseEventArgs(e.Button, e.Clicks, x, y, e.Delta);
    }

    private Size GetScaledSize() => this.GetScaledSize(this.Size);

    private void AlignPanels()
    {
      Rectangle b = this.DisplayRectangle.Scale(this.scale);
      Rectangle f = this.ClientRectangle.Scale(this.scale);
      if (this.panels == null)
        return;
      this.panels.ForEach((Action<OverlayPanel>) (op =>
      {
        if (!op.AutoAlign)
          return;
        op.Align(op.IgnoreParentMargin ? f : b, op.Alignment);
      }));
    }

    protected void FireInvalidate(Rectangle panelBounds, bool always)
    {
      if (!(this.IsVisible | always))
        return;
      this.OnPanelInvalidated(new PanelInvalidateEventArgs(panelBounds, always));
    }

    protected virtual void OnVisibleChanged()
    {
      if (this.VisibleChanged == null)
        return;
      this.VisibleChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnLocationChanged()
    {
      if (this.LocationChanged == null)
        return;
      this.LocationChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnSizeChanged()
    {
      if (this.SizeChanged != null)
        this.SizeChanged((object) this, EventArgs.Empty);
      this.AlignPanels();
    }

    protected virtual void OnScaleChanged()
    {
      if (this.ScaleChanged != null)
        this.ScaleChanged((object) this, EventArgs.Empty);
      this.AlignPanels();
    }

    protected virtual void OnOpacityChanged()
    {
      if (this.Visible)
        this.InvalidatePanel(true);
      if (this.OpacityChanged == null)
        return;
      this.OpacityChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnContrastChanged()
    {
      if (this.Visible)
        this.InvalidatePanel(true);
      if (this.ContrastChanged == null)
        return;
      this.ContrastChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnBrightnessChanged()
    {
      if (this.Visible)
        this.InvalidatePanel(true);
      if (this.BrightnessChanged == null)
        return;
      this.BrightnessChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnSaturationChanged()
    {
      if (this.Visible)
        this.InvalidatePanel(true);
      if (this.SaturationChanged == null)
        return;
      this.SaturationChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnAlignmentChanged()
    {
      if (this.AlignmentChanged == null)
        return;
      this.AlignmentChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnMarginChanged()
    {
      if (this.MarginChanged != null)
        this.MarginChanged((object) this, EventArgs.Empty);
      this.AlignPanels();
    }

    protected virtual void OnPanelStateChanged()
    {
      if (this.PanelStateChanged == null)
        return;
      this.PanelStateChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnMouseEnter(MouseEventArgs e)
    {
      this.PanelState = PanelState.Hot;
      if (this.MouseEnter == null)
        return;
      this.MouseEnter((object) this, e);
    }

    protected virtual void OnMouseLeave(MouseEventArgs e)
    {
      this.PanelState = PanelState.Normal;
      if (this.MouseLeave == null)
        return;
      this.MouseLeave((object) this, e);
    }

    protected virtual void OnMouseDown(MouseEventArgs e)
    {
      this.PanelState = PanelState.Selected;
      if (this.MouseDown == null)
        return;
      this.MouseDown((object) this, e);
    }

    protected virtual void OnMouseUp(MouseEventArgs e)
    {
      this.PanelState = PanelState.Hot;
      if (this.MouseUp == null)
        return;
      this.MouseUp((object) this, e);
    }

    protected virtual void OnMouseMove(MouseEventArgs e)
    {
      if (this.MouseMove == null)
        return;
      this.MouseMove((object) this, e);
    }

    protected virtual void OnClick()
    {
      if (this.Click == null)
        return;
      this.Click((object) this, EventArgs.Empty);
    }

    protected virtual void OnDoubleClick()
    {
      if (this.DoubleClick == null)
        return;
      this.DoubleClick((object) this, EventArgs.Empty);
    }

    public event EventHandler<PanelInvalidateEventArgs> PanelInvalidated;

    public event EventHandler VisibleChanged;

    public event EventHandler LocationChanged;

    public event EventHandler SizeChanged;

    public event EventHandler ScaleChanged;

    public event EventHandler OpacityChanged;

    public event EventHandler BrightnessChanged;

    public event EventHandler ContrastChanged;

    public event EventHandler SaturationChanged;

    public event EventHandler AlignmentChanged;

    public event EventHandler MarginChanged;

    public event EventHandler PanelStateChanged;

    public event MouseEventHandler MouseEnter;

    public event MouseEventHandler MouseLeave;

    public event MouseEventHandler MouseDown;

    public event MouseEventHandler MouseUp;

    public event MouseEventHandler MouseMove;

    public event EventHandler Click;

    public event EventHandler DoubleClick;

    public event EventHandler Drawing;

    public event PaintEventHandler Paint;

    public event PaintEventHandler PaintBackground;

    public event EventHandler<PanelRenderEventArgs> RenderSurface;

    protected virtual void OnPaintBackground(PaintEventArgs e)
    {
      if (this.PaintBackground != null)
        this.PaintBackground((object) this, e);
      e.Graphics.Clear(this.BackgroundColor);
    }

    protected virtual void OnPaint(PaintEventArgs e)
    {
      if (this.Paint == null)
        return;
      this.Paint((object) this, e);
    }

    protected virtual void OnDrawing()
    {
      if (this.Drawing != null)
        this.Drawing((object) this, EventArgs.Empty);
      while (this.dirty)
      {
        this.dirty = false;
        this.PaintSurface();
      }
    }

    protected virtual void OnRenderSurface(PanelRenderEventArgs e)
    {
      if (this.RenderSurface == null)
        return;
      this.RenderSurface((object) this, e);
    }

    protected virtual void OnPanelInvalidated(PanelInvalidateEventArgs e)
    {
      if (this.PanelInvalidated == null)
        return;
      this.PanelInvalidated((object) this, e);
    }

    private void Animators_Changed(object sender, SmartListChangedEventArgs<Animator> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          e.Item.Started += new EventHandler(this.AnimatorStarted);
          break;
        case SmartListAction.Remove:
          e.Item.Started -= new EventHandler(this.AnimatorStarted);
          break;
      }
    }

    private void NotifyManagerStart() => this.Manager?.NotifyAnimationStart();

    private void AnimatorStarted(object sender, EventArgs e) => this.NotifyManagerStart();

    private void overlayPanels_Changed(object sender, SmartListChangedEventArgs<OverlayPanel> e)
    {
      this.RegisterEvents(e.Item, e.Action == SmartListAction.Insert);
      this.NotifyManagerStart();
    }

    protected void RegisterEvents(OverlayPanel op, bool register)
    {
      if (op == null)
        return;
      if (register)
      {
        op.Parent = this;
        op.PanelInvalidated += new EventHandler<PanelInvalidateEventArgs>(this.OnPanelInvalidated);
        op.SizeChanged += new EventHandler(this.OnPanelSizeChanged);
        op.AlignmentChanged += new EventHandler(this.OnPanelAlignmentChanged);
        this.AlignPanels();
        op.InvalidatePanel();
      }
      else
      {
        op.Parent = (OverlayPanel) null;
        op.InvalidatePanel();
        op.PanelInvalidated -= new EventHandler<PanelInvalidateEventArgs>(this.OnPanelInvalidated);
        op.SizeChanged -= new EventHandler(this.OnPanelSizeChanged);
        op.AlignmentChanged -= new EventHandler(this.OnPanelAlignmentChanged);
      }
    }

    private void OnPanelInvalidated(object sender, PanelInvalidateEventArgs e)
    {
      this.InvalidatePanel(e.Bounds, e.Always);
    }

    private void OnPanelSizeChanged(object sender, EventArgs e) => this.AlignPanels();

    private void OnPanelAlignmentChanged(object sender, EventArgs e) => this.AlignPanels();

    public void Invalidate() => this.Invalidate(Rectangle.Empty);

    public void Invalidate(Rectangle bounds)
    {
      this.dirty = true;
      using (ItemMonitor.Lock((object) this.invalidatedBounds))
        this.invalidatedBounds.Add(bounds);
      this.InvalidatePanel();
    }

    private void PaintSurface()
    {
      using (PanelSurface surface = this.CreateSurface())
      {
        Graphics graphics = surface.Graphics;
        graphics.SetClip(Rectangle.Empty);
        using (ItemMonitor.Lock((object) this.invalidatedBounds))
        {
          foreach (Rectangle invalidatedBound in this.invalidatedBounds)
          {
            if (!invalidatedBound.IsEmpty)
            {
              if (graphics.IsClipEmpty)
                graphics.SetClip(invalidatedBound);
              else
                graphics.SetClip(invalidatedBound, CombineMode.Union);
            }
            else
            {
              graphics.SetClip(this.ClientRectangle);
              break;
            }
          }
          this.invalidatedBounds.Clear();
        }
        PaintEventArgs e = new PaintEventArgs(graphics, Rectangle.Round(graphics.ClipBounds));
        this.OnPaintBackground(e);
        this.OnPaint(e);
      }
    }
  }
}

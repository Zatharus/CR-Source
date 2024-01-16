// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.OverlayManager
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Collections;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class OverlayManager : Component
  {
    private Size alignmentBorder = new Size(10, 10);
    private Control control;
    private readonly OverlayPanelCollection panels = new OverlayPanelCollection();
    private bool mouseHandled;
    private const int animationSlice = 25;
    private Thread animationThread;
    private readonly ManualResetEvent animationSignal = new ManualResetEvent(false);
    private readonly ManualResetEvent animationStopSignal = new ManualResetEvent(false);
    private OverlayPanel currentPanel;
    private bool mouseDown;

    public OverlayManager(Control control)
    {
      this.Control = control;
      this.panels.Changed += new EventHandler<SmartListChangedEventArgs<OverlayPanel>>(this.OverlayPanelsChanged);
    }

    public OverlayManager()
      : this((Control) null)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Control = (Control) null;
        this.AnimationEnabled = false;
      }
      base.Dispose(disposing);
    }

    public Size AlignmentBorder
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.alignmentBorder;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.alignmentBorder == value)
            return;
          this.alignmentBorder = value;
        }
        this.AlignPanels();
      }
    }

    public Control Control
    {
      get => this.control;
      set
      {
        if (this.control == value)
          return;
        if (this.control != null)
        {
          this.control.SizeChanged -= new EventHandler(this.control_SizeChanged);
          this.control.MouseDown -= new MouseEventHandler(this.control_MouseDown);
          this.control.MouseUp -= new MouseEventHandler(this.control_MouseUp);
          this.control.MouseEnter -= new EventHandler(this.control_MouseEnter);
          this.control.MouseMove -= new MouseEventHandler(this.control_MouseMove);
          this.control.MouseLeave -= new EventHandler(this.control_MouseLeave);
          this.control.MouseClick -= new MouseEventHandler(this.control_MouseClick);
          this.control.MouseDoubleClick -= new MouseEventHandler(this.control_MouseDoubleClick);
        }
        if (this.control is IPanableControl)
        {
          IPanableControl control = this.control as IPanableControl;
          control.PanStart -= new EventHandler(this.control_PanStart);
          control.Pan -= new EventHandler(this.control_Pan);
          control.PanEnd -= new EventHandler(this.control_PanEnd);
        }
        this.control = value;
        if (this.control == null)
          return;
        this.control.SizeChanged += new EventHandler(this.control_SizeChanged);
        this.control.MouseDown += new MouseEventHandler(this.control_MouseDown);
        this.control.MouseUp += new MouseEventHandler(this.control_MouseUp);
        this.control.MouseEnter += new EventHandler(this.control_MouseEnter);
        this.control.MouseMove += new MouseEventHandler(this.control_MouseMove);
        this.control.MouseLeave += new EventHandler(this.control_MouseLeave);
        this.control.MouseClick += new MouseEventHandler(this.control_MouseClick);
        this.control.MouseDoubleClick += new MouseEventHandler(this.control_MouseDoubleClick);
        if (!(this.control is IPanableControl))
          return;
        IPanableControl control1 = this.control as IPanableControl;
        control1.PanStart += new EventHandler(this.control_PanStart);
        control1.Pan += new EventHandler(this.control_Pan);
        control1.PanEnd += new EventHandler(this.control_PanEnd);
      }
    }

    public OverlayPanelCollection Panels => this.panels;

    public bool MouseHandled => this.mouseHandled;

    public bool AnimationEnabled
    {
      get => this.animationThread != null;
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (value)
          {
            if (this.animationThread != null)
              return;
            this.animationThread = ThreadUtility.CreateWorkerThread("Overlay Animation Thread", new ThreadStart(this.RunAnimation), ThreadPriority.Lowest);
            this.animationSignal.Set();
            this.animationStopSignal.Reset();
            this.animationThread.Start();
          }
          else
          {
            if (this.animationThread == null)
              return;
            this.animationStopSignal.Set();
            this.animationThread.Join();
            this.animationThread = (Thread) null;
          }
        }
      }
    }

    private void RunAnimation()
    {
      ManualResetEvent[] waitHandles = new ManualResetEvent[2]
      {
        this.animationSignal,
        this.animationStopSignal
      };
      while (WaitHandle.WaitAny((WaitHandle[]) waitHandles) == 0)
      {
        this.animationSignal.Reset();
        while (!this.animationStopSignal.WaitOne(25))
        {
          bool stillRunning = false;
          this.panels.ForEach((Action<OverlayPanel>) (op => stillRunning |= op.Animate()), true);
          this.panels.RemoveRange((IEnumerable<OverlayPanel>) this.panels.FindAll((Predicate<OverlayPanel>) (op => op.Animators.AllCompleted && op.DestroyAfterCompletion)));
          if (!stillRunning)
            break;
        }
      }
    }

    public void NotifyAnimationStart() => this.animationSignal.Set();

    public void Draw(IBitmapRenderer graphics)
    {
      foreach (OverlayPanel overlayPanel in this.panels.ToArray())
      {
        if (overlayPanel.IsVisible && graphics.IsVisible((RectangleF) overlayPanel.Bounds))
          overlayPanel.Draw(graphics, overlayPanel.Bounds);
      }
    }

    public void Draw(Graphics graphics)
    {
      this.Draw((IBitmapRenderer) new BitmapGdiRenderer(graphics));
    }

    public OverlayPanel HitTest(Point pt)
    {
      return ((IEnumerable<OverlayPanel>) this.panels.ToArray()).Reverse<OverlayPanel>().Select<OverlayPanel, OverlayPanel>((Func<OverlayPanel, OverlayPanel>) (op => op.HitTest(pt))).FirstOrDefault<OverlayPanel>((Func<OverlayPanel, bool>) (hit => hit != null));
    }

    protected virtual Rectangle ClientRectangle => this.control.ClientRectangle;

    protected virtual Rectangle DisplayRectangle
    {
      get
      {
        Rectangle clientRectangle = this.ClientRectangle;
        clientRectangle.Inflate(-this.alignmentBorder.Width, -this.alignmentBorder.Height);
        return clientRectangle;
      }
    }

    public virtual void Invalidate()
    {
      if (this.Control == null)
        return;
      this.Control.Invalidate();
    }

    public virtual void Invalidate(Rectangle rc)
    {
      if (this.Control == null)
        return;
      this.Control.Invalidate(rc);
    }

    private void AlignPanels()
    {
      this.panels.ForEach((Action<OverlayPanel>) (op =>
      {
        if (!op.AutoAlign)
          return;
        op.Align(op.IgnoreParentMargin ? this.ClientRectangle : this.DisplayRectangle, op.Alignment);
      }));
    }

    private void control_SizeChanged(object sender, EventArgs e) => this.AlignPanels();

    private void OnPanelInvalidated(object sender, PanelInvalidateEventArgs e)
    {
      this.Invalidate(e.Bounds);
    }

    private void OnPanelSizeChanged(object sender, EventArgs e) => this.AlignPanels();

    private void OnPanelAlignmentChanged(object sender, EventArgs e) => this.AlignPanels();

    private void OverlayPanelsChanged(object sender, SmartListChangedEventArgs<OverlayPanel> e)
    {
      this.RegisterEvents(e.Item, e.Action == SmartListAction.Insert);
    }

    protected void RegisterEvents(OverlayPanel op, bool register)
    {
      if (op == null)
        return;
      if (register)
      {
        op.Manager = this;
        op.PanelInvalidated += new EventHandler<PanelInvalidateEventArgs>(this.OnPanelInvalidated);
        op.SizeChanged += new EventHandler(this.OnPanelSizeChanged);
        op.AlignmentChanged += new EventHandler(this.OnPanelAlignmentChanged);
        this.AlignPanels();
        op.InvalidatePanel();
      }
      else
      {
        op.InvalidatePanel();
        op.Manager = (OverlayManager) null;
        op.PanelInvalidated -= new EventHandler<PanelInvalidateEventArgs>(this.OnPanelInvalidated);
        op.SizeChanged -= new EventHandler(this.OnPanelSizeChanged);
        op.AlignmentChanged -= new EventHandler(this.OnPanelAlignmentChanged);
      }
    }

    private void control_PanStart(object sender, EventArgs e)
    {
      IPanableControl control = this.control as IPanableControl;
      Point panLocation = control.PanLocation;
      int x = panLocation.X;
      panLocation = control.PanLocation;
      int y = panLocation.Y;
      MouseEventArgs e1 = new MouseEventArgs(MouseButtons.Left, 1, x, y, 0);
      this.control_MouseMove(sender, e1);
      this.control_MouseDown(sender, e1);
    }

    private void control_Pan(object sender, EventArgs e)
    {
      IPanableControl control = this.control as IPanableControl;
      Point panLocation = control.PanLocation;
      int x = panLocation.X;
      panLocation = control.PanLocation;
      int y = panLocation.Y;
      MouseEventArgs e1 = new MouseEventArgs(MouseButtons.Left, 1, x, y, 0);
      this.control_MouseMove(sender, e1);
    }

    private void control_PanEnd(object sender, EventArgs e)
    {
      IPanableControl control = this.control as IPanableControl;
      Point panLocation = control.PanLocation;
      int x = panLocation.X;
      panLocation = control.PanLocation;
      int y = panLocation.Y;
      MouseEventArgs e1 = new MouseEventArgs(MouseButtons.Left, 1, x, y, 0);
      this.control_MouseUp(sender, e1);
    }

    private static MouseEventArgs GetMouseEventArgsOffset(MouseEventArgs e, Point offset)
    {
      Point point = new Point(e.X - offset.X, e.Y - offset.Y);
      return new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta);
    }

    private void control_MouseClick(object sender, MouseEventArgs e)
    {
      OverlayPanel overlayPanel = this.HitTest(this.Control.PointToClient(Cursor.Position));
      overlayPanel?.FireClick();
      this.mouseHandled = overlayPanel != null;
    }

    private void control_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      OverlayPanel overlayPanel = this.HitTest(this.Control.PointToClient(Cursor.Position));
      overlayPanel?.FireDoubleClick();
      this.mouseHandled = overlayPanel != null;
    }

    private void control_MouseEnter(object sender, EventArgs e)
    {
      Point client = this.Control.PointToClient(Cursor.Position);
      this.mouseHandled = this.HandleMouseMove(new MouseEventArgs(MouseButtons.None, 0, client.X, client.Y, 0));
    }

    private void control_MouseLeave(object sender, EventArgs e)
    {
      this.mouseHandled = this.HandleMouseMove(new MouseEventArgs(MouseButtons.None, 0, int.MaxValue, int.MinValue, 0));
    }

    private void control_MouseMove(object sender, MouseEventArgs e)
    {
      this.mouseHandled = this.HandleMouseMove(e);
    }

    private void control_MouseUp(object sender, MouseEventArgs e)
    {
      if (this.currentPanel != null)
      {
        this.currentPanel.FireMouseUp(OverlayManager.GetMouseEventArgsOffset(e, this.currentPanel.GetAbsoluteLocation()));
        this.mouseHandled = this.HandleMouseMove(e);
      }
      else
        this.mouseHandled = false;
      this.mouseDown = false;
    }

    private void control_MouseDown(object sender, MouseEventArgs e)
    {
      this.mouseDown = true;
      if (this.currentPanel != null)
      {
        this.mouseHandled = true;
        this.currentPanel.FireMouseDown(OverlayManager.GetMouseEventArgsOffset(e, this.currentPanel.GetAbsoluteLocation()));
      }
      else
        this.mouseHandled = false;
    }

    private bool HandleMouseMove(MouseEventArgs e)
    {
      if (this.mouseDown)
      {
        if (this.currentPanel != null)
        {
          this.currentPanel.FireMouseMove(OverlayManager.GetMouseEventArgsOffset(e, this.currentPanel.GetAbsoluteLocation()));
          return true;
        }
      }
      else
      {
        OverlayPanel overlayPanel = this.HitTest(e.Location);
        if (this.currentPanel != null && overlayPanel != this.currentPanel)
        {
          this.currentPanel.FireMouseLeave(OverlayManager.GetMouseEventArgsOffset(e, this.currentPanel.GetAbsoluteLocation()));
          this.currentPanel = (OverlayPanel) null;
        }
        if (overlayPanel != null)
        {
          this.currentPanel = overlayPanel;
          this.currentPanel.FireMouseEnter(OverlayManager.GetMouseEventArgsOffset(e, this.currentPanel.GetAbsoluteLocation()));
        }
        if (this.currentPanel != null)
        {
          this.currentPanel.FireMouseMove(OverlayManager.GetMouseEventArgsOffset(e, this.currentPanel.GetAbsoluteLocation()));
          return true;
        }
      }
      return false;
    }
  }
}

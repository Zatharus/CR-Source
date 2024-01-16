// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.VisibilityAnimator
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class VisibilityAnimator : Component
  {
    private readonly Timer timer = new Timer();
    private VisibilityAnimator.AnimationInfo[] animations;
    private long animationStart;
    private bool? pendingVisible;

    static VisibilityAnimator()
    {
      VisibilityAnimator.EnableAnimation = true;
      VisibilityAnimator.AnimationDuration = 100;
    }

    public VisibilityAnimator()
    {
      this.Controls = new List<Control>();
      this.Enabled = true;
      this.timer.Tick += new EventHandler(this.AnimationEvent);
    }

    public VisibilityAnimator(IContainer container, Control control = null, int duration = 0)
      : this()
    {
      container.Add((IComponent) this);
      if (control != null)
        this.Controls.Add(control);
      if (duration == 0)
        return;
      VisibilityAnimator.AnimationDuration = duration;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.timer.Dispose();
      base.Dispose(disposing);
    }

    public bool Visible
    {
      get => this.pendingVisible.HasValue ? this.pendingVisible.Value : this.GetVisibility();
      set
      {
        if (value == this.Visible)
          return;
        this.StartAnimation(value);
      }
    }

    public bool Enabled { get; set; }

    public List<Control> Controls { get; private set; }

    private bool GetVisibility()
    {
      return this.Controls.Any<Control>((Func<Control, bool>) (t => t.IsVisibleSet()));
    }

    private void StartAnimation(bool targetVisibility)
    {
      if (!this.Enabled || !VisibilityAnimator.EnableAnimation || !this.IsContainerVisible())
      {
        this.CleanUp();
        this.Controls.ForEach((Action<Control>) (c => c.Visible = targetVisibility));
      }
      else
      {
        this.animationStart = 0L;
        this.animations = this.Controls.Select<Control, VisibilityAnimator.AnimationInfo>((Func<Control, VisibilityAnimator.AnimationInfo>) (t => new VisibilityAnimator.AnimationInfo()
        {
          Control = t,
          Height = t.Height,
          AutoSize = t.AutoSize,
          MinimumSize = t.MinimumSize
        })).ToArray<VisibilityAnimator.AnimationInfo>();
        this.pendingVisible = new bool?(targetVisibility);
        this.timer.Start();
      }
    }

    private bool IsContainerVisible()
    {
      return !(this.Container is Control container) || container.TopLevelControl == null || container.TopLevelControl.Visible;
    }

    private void SetHeight(Control control, int h)
    {
      control.SuspendLayout();
      control.AutoSize = false;
      control.MinimumSize = System.Drawing.Size.Empty;
      control.Height = h;
      control.Visible = true;
      control.ResumeLayout();
    }

    private void CleanUp()
    {
      if (!this.pendingVisible.HasValue)
        return;
      foreach (VisibilityAnimator.AnimationInfo animation in this.animations)
      {
        animation.Control.SuspendLayout();
        if (!this.pendingVisible.Value)
          animation.Control.Visible = false;
        animation.Control.AutoSize = animation.AutoSize;
        animation.Control.Height = animation.Height;
        animation.Control.MinimumSize = animation.MinimumSize;
        if (this.pendingVisible.Value)
          animation.Control.Visible = true;
        animation.Control.ResumeLayout();
      }
      this.pendingVisible = new bool?();
    }

    private void AnimationEvent(object sender, EventArgs e)
    {
      if (!this.pendingVisible.HasValue)
      {
        this.timer.Stop();
      }
      else
      {
        if (this.animationStart == 0L)
          this.animationStart = VisibilityAnimator.Ticks;
        float num1 = (float) (VisibilityAnimator.Ticks - this.animationStart) / (float) VisibilityAnimator.AnimationDuration;
        float val2 = num1;
        if (!this.pendingVisible.Value)
          val2 = 1f - num1;
        float num2 = Math.Max(Math.Min(1f, val2), 0.0f);
        foreach (VisibilityAnimator.AnimationInfo animation in this.animations)
          this.SetHeight(animation.Control, (int) ((double) animation.Height * (double) num2));
        if ((double) num1 < 1.0)
          return;
        this.CleanUp();
      }
    }

    private static long Ticks => DateTime.Now.Ticks / 10000L;

    public static bool EnableAnimation { get; set; }

    public static int AnimationDuration { get; set; }

    private class AnimationInfo
    {
      public Control Control { get; set; }

      public int Height { get; set; }

      public bool AutoSize { get; set; }

      public System.Drawing.Size MinimumSize { get; set; }
    }
  }
}

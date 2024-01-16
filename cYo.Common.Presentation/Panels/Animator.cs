// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.Animator
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Mathematics;
using cYo.Common.Runtime;
using System;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class Animator
  {
    private volatile int delay;
    private volatile int time;
    private volatile int span = 1000;
    private volatile bool isRunning;
    private volatile float animationValue;
    private volatile float lastAnimationValue;
    private AnimationValueHandler animationValueGenerator = new AnimationValueHandler(Animator.LinearRise);
    private AnimationHandler animationHandler;
    private long startTime;

    public int Delay
    {
      get => this.delay;
      set => this.delay = value;
    }

    public int Time => this.time;

    public int Span
    {
      get => this.span;
      set => this.span = value;
    }

    public bool HasBeenStarted => this.startTime != 0L;

    public bool IsRunning => this.isRunning;

    public bool IsCompleted => this.HasBeenStarted && this.time >= this.span;

    public float AnimationValue => this.animationValue;

    public float LastAnimationValue => this.lastAnimationValue;

    public AnimationValueHandler AnimationValueGenerator
    {
      get => this.animationValueGenerator;
      set => this.animationValueGenerator = value;
    }

    public AnimationHandler AnimationHandler
    {
      get => this.animationHandler;
      set => this.animationHandler = value;
    }

    public void Start()
    {
      this.isRunning = true;
      this.startTime = Animator.Now;
      this.time = 0;
      this.animationValue = this.GetAnimationValue().Clamp(0.0f, 1f);
      this.OnStarted();
    }

    public void Stop() => this.isRunning = false;

    public virtual bool Animate(OverlayPanel panel)
    {
      if (!this.IsRunning || this.IsCompleted)
        return false;
      this.time = (int) Math.Max(0L, Animator.Now - this.startTime - (long) this.Delay);
      this.lastAnimationValue = this.animationValue;
      this.animationValue = this.GetAnimationValue().Clamp(0.0f, 1f);
      this.OnAnimate(panel);
      return true;
    }

    public event EventHandler Started;

    protected virtual float GetAnimationValue()
    {
      return this.animationValueGenerator == null ? 1f : this.animationValueGenerator(this.Time, this.Span);
    }

    protected virtual void OnAnimate(OverlayPanel panel)
    {
      if (this.animationHandler == null)
        return;
      this.animationHandler(panel, this.AnimationValue, this.AnimationValue - this.LastAnimationValue);
    }

    protected virtual void OnStarted()
    {
      if (this.Started == null)
        return;
      this.Started((object) this, EventArgs.Empty);
    }

    public static float Constant1(int time, int span) => 1f;

    public static float LinearRise(int time, int span) => (float) time / (float) span;

    public static float SinusRise(int time, int span)
    {
      float num = (float) time / (float) span;
      return (double) num <= 0.0 || (double) num >= 1.0 ? num : (float) Math.Sin((double) num * Math.PI / 2.0);
    }

    public static float LinearDrop(int time, int span) => 1f - Animator.LinearRise(time, span);

    public static AnimationValueHandler CreateBouncer(
      int inTime,
      int stayTime,
      int outTime,
      AnimationValueHandler inHandler,
      AnimationValueHandler stayHandler,
      AnimationValueHandler outHandler)
    {
      return (AnimationValueHandler) ((time, span) =>
      {
        if (time < inTime)
          return inHandler(time, inTime);
        time -= inTime;
        if (time < stayTime)
          return stayHandler(time, stayTime);
        time -= stayTime;
        return time < outTime ? outHandler(time, outTime) : 0.0f;
      });
    }

    public static AnimationValueHandler CreateLinearBouncer(int inTime, int stayTime, int outTime)
    {
      return Animator.CreateBouncer(inTime, stayTime, outTime, new AnimationValueHandler(Animator.LinearRise), new AnimationValueHandler(Animator.Constant1), new AnimationValueHandler(Animator.LinearDrop));
    }

    public static long Now => Machine.Ticks;
  }
}

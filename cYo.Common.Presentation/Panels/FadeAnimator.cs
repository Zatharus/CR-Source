// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.FadeAnimator
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class FadeAnimator : Animator
  {
    public FadeAnimator(int fadeInTime, int visibilityTime, int fadeOutTime)
    {
      this.Span = fadeInTime + visibilityTime + fadeOutTime;
      this.AnimationValueGenerator = Animator.CreateLinearBouncer(fadeInTime, visibilityTime, fadeOutTime);
      this.AnimationHandler = (AnimationHandler) ((p, x, d) => p.Opacity += d);
    }
  }
}

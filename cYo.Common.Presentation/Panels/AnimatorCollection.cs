// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.AnimatorCollection
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Collections;
using System;
using System.Linq;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class AnimatorCollection : SmartList<Animator>
  {
    public bool AllCompleted => this.All<Animator>((Func<Animator, bool>) (a => a.IsCompleted));

    public void Start() => this.ForEach((Action<Animator>) (a => a.Start()));
  }
}

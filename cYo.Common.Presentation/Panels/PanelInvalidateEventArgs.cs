// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.PanelInvalidateEventArgs
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;
using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class PanelInvalidateEventArgs : EventArgs
  {
    private readonly bool always;
    private readonly Rectangle bounds;

    public PanelInvalidateEventArgs(Rectangle bounds, bool always)
    {
      this.bounds = bounds;
      this.always = always;
    }

    public bool Always => this.always;

    public Rectangle Bounds => this.bounds;
  }
}

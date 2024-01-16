// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.GestureEventArgs
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display
{
  public class GestureEventArgs : EventArgs
  {
    private readonly GestureType gesture;

    public GestureEventArgs(GestureType gesture) => this.gesture = gesture;

    public GestureType Gesture => this.gesture;

    public ContentAlignment Area { get; set; }

    public Rectangle AreaBounds { get; set; }

    public Point Location { get; set; }

    public MouseButtons MouseButton { get; set; }

    public bool Double { get; set; }

    public bool Handled { get; set; }

    public bool IsTouch { get; set; }
  }
}

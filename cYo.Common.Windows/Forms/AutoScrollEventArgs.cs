// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.AutoScrollEventArgs
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.ComponentModel;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class AutoScrollEventArgs : CancelEventArgs
  {
    private System.Drawing.Point delta;

    public System.Drawing.Point Delta
    {
      get => this.delta;
      set => this.delta = value;
    }

    public int X
    {
      get => this.delta.X;
      set => this.delta.X = value;
    }

    public int Y
    {
      get => this.delta.Y;
      set => this.delta.Y = value;
    }
  }
}

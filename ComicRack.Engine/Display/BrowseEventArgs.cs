// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.BrowseEventArgs
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display
{
  public class BrowseEventArgs : EventArgs
  {
    private readonly PageSeekOrigin seekOrigin;
    private readonly int offset;

    public BrowseEventArgs(PageSeekOrigin origin, int offset)
    {
      this.seekOrigin = origin;
      this.offset = offset;
    }

    public PageSeekOrigin SeekOrigin => this.seekOrigin;

    public int Offset => this.offset;
  }
}

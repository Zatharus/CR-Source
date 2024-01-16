// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.ImageRetrievedEventArgs
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public class ImageRetrievedEventArgs : EventArgs
  {
    private readonly Image image;
    private readonly int index;

    public ImageRetrievedEventArgs(int index, Image image)
    {
      this.image = image;
      this.index = index;
    }

    public Image Image => this.image;

    public int Index => this.index;
  }
}

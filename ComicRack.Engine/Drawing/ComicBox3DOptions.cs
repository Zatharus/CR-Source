// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ComicBox3DOptions
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  [Flags]
  public enum ComicBox3DOptions
  {
    Trim = 1,
    Filter = 2,
    SimpleShadow = 4,
    Wireless = 8,
    SplitDoublePages = 16, // 0x00000010
    Default = SplitDoublePages | SimpleShadow | Filter | Trim, // 0x00000017
  }
}

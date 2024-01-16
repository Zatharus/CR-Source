// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.WorkspaceType
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  [Flags]
  public enum WorkspaceType
  {
    WindowLayout = 1,
    ViewsSetup = 2,
    ComicPageLayout = 4,
    ComicPageDisplay = 8,
    Default = ComicPageDisplay | ComicPageLayout | ViewsSetup | WindowLayout, // 0x0000000F
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.IDisplayWorkspace
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Projects.ComicRack.Viewer.Config;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public interface IDisplayWorkspace
  {
    void SetWorkspace(DisplayWorkspace ws);

    void StoreWorkspace(DisplayWorkspace ws);
  }
}

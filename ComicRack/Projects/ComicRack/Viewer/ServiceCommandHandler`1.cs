﻿// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.ServiceCommandHandler`1
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public delegate void ServiceCommandHandler<in T>(T service) where T : class;
}

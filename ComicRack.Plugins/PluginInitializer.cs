// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.PluginInitializer
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  public abstract class PluginInitializer
  {
    public abstract IEnumerable<Command> GetCommands(string file);
  }
}

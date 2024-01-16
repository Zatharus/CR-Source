// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.XmlPluginInitializer
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  public class XmlPluginInitializer : PluginInitializer
  {
    public override IEnumerable<Command> GetCommands(string file)
    {
      try
      {
        if (".xml".Equals(Path.GetExtension(file), StringComparison.OrdinalIgnoreCase))
          return (IEnumerable<Command>) XmlUtility.Load<CommandCollection>(file, false);
      }
      catch (Exception ex)
      {
      }
      return (IEnumerable<Command>) new Command[0];
    }
  }
}

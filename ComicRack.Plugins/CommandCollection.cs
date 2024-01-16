// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.CommandCollection
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Common.Collections;
using System;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  [XmlInclude(typeof (PythonCommand))]
  public class CommandCollection : SmartList<Command>
  {
    public Command this[string key]
    {
      get => this.FirstOrDefault<Command>((Func<Command, bool>) (cmd => cmd.Key == key));
    }
  }
}

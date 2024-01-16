// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.IPluginEnvironment
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Plugins.Automation;
using System;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  public interface IPluginEnvironment : IPluginConfig, ICloneable
  {
    IWin32Window MainWindow { get; }

    IApplication App { get; }

    IOpenBooksManager OpenBooks { get; }

    IBrowser Browser { get; }

    IComicDisplay ComicDisplay { get; }

    string CommandPath { get; set; }

    string Localize(string resourceKey, string elementKey, string text);
  }
}

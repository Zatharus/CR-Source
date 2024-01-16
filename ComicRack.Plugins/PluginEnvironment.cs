// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.PluginEnvironment
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Plugins.Automation;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  [ComVisible(true)]
  public class PluginEnvironment : IPluginEnvironment, IPluginConfig, ICloneable
  {
    private TRDictionary localization;
    private readonly IPluginConfig config;

    public PluginEnvironment(
      IWin32Window mainWindow,
      IApplication app,
      IBrowser browser,
      IComicDisplay comicDisplay,
      IPluginConfig config,
      IOpenBooksManager openBooksManager)
    {
      this.App = app;
      this.Browser = browser;
      this.ComicDisplay = comicDisplay;
      this.MainWindow = mainWindow;
      this.OpenBooks = openBooksManager;
      this.config = config;
    }

    public IEnumerable<ComicBook> ReadDatabaseBooks(string file)
    {
      return (IEnumerable<ComicBook>) ComicDatabase.LoadXml(file).Books;
    }

    public string Localize(string resourceKey, string nameKey, string text)
    {
      if (string.IsNullOrEmpty(resourceKey))
        return text;
      if (this.localization == null)
        this.localization = new TRDictionary()
        {
          ResourceFolder = (IVirtualFolder) new PackedLocalize((IVirtualFolder) new VirtualFileFolder(this.CommandPath))
        };
      TR tr = this.localization.Load(resourceKey);
      if (tr.IsEmpty)
      {
        tr = TR.Load("Script." + resourceKey);
        if (tr.IsEmpty)
          tr = TR.Load(resourceKey);
      }
      return tr[nameKey, text];
    }

    public IWin32Window MainWindow { get; private set; }

    public IApplication App { get; private set; }

    public IBrowser Browser { get; private set; }

    public IOpenBooksManager OpenBooks { get; set; }

    public IComicDisplay ComicDisplay { get; private set; }

    public string CommandPath { get; set; }

    public IEnumerable<string> LibraryPaths => this.config.LibraryPaths;

    public object Clone()
    {
      return (object) new PluginEnvironment(this.MainWindow, this.App, this.Browser, this.ComicDisplay, this.config, this.OpenBooks);
    }
  }
}

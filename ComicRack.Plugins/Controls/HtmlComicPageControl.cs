// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.Controls.HtmlComicPageControl
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins.Controls
{
  public class HtmlComicPageControl : ComicPageControl
  {
    private string lastResult;
    private string originalConfig;
    private IContainer components;
    private WebBrowser webBrowser;

    public HtmlComicPageControl()
    {
      this.Script = new HtmlComicPageControl.ScriptProvider();
      this.InitializeComponent();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.SaveConfigFunction != null && this.originalConfig != this.ScriptConfig)
          this.SaveConfigFunction(this.ScriptConfig);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public Func<ComicBook[], string> InfoFunction { get; set; }

    public Action<string> SaveConfigFunction { get; set; }

    public HtmlComicPageControl.ScriptProvider Script { get; private set; }

    public object ScriptEngine
    {
      get => this.Script.ComicRack;
      set => this.Script.ComicRack = value;
    }

    public string ScriptConfig
    {
      get => this.Script.Config;
      set => this.originalConfig = this.Script.Config = value;
    }

    protected override void OnShowInfo(IEnumerable<ComicBook> books)
    {
      base.OnShowInfo(books);
      if (this.InfoFunction == null)
        return;
      try
      {
        string str = this.InfoFunction(books.ToArray<ComicBook>());
        if (str == this.lastResult)
          return;
        this.webBrowser.ObjectForScripting = (object) this.Script;
        this.webBrowser.ScriptErrorsSuppressed = !EngineConfiguration.Default.EnableHtmlScriptErrors;
        this.webBrowser.IsWebBrowserContextMenuEnabled = EngineConfiguration.Default.HtmlInfoContextMenu;
        if (str.StartsWith("!"))
          this.webBrowser.Url = new Uri(str.Substring(1));
        else
          this.webBrowser.DocumentText = str;
        this.lastResult = str;
      }
      catch (Exception ex)
      {
        this.webBrowser.DocumentText = string.Empty;
        this.lastResult = (string) null;
      }
    }

    private void InitializeComponent()
    {
      this.webBrowser = new WebBrowser();
      this.SuspendLayout();
      this.webBrowser.Dock = DockStyle.Fill;
      this.webBrowser.Location = new Point(0, 0);
      this.webBrowser.MinimumSize = new Size(20, 20);
      this.webBrowser.Name = "webBrowser";
      this.webBrowser.Size = new Size(540, 402);
      this.webBrowser.TabIndex = 1;
      this.webBrowser.WebBrowserShortcutsEnabled = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.webBrowser);
      this.Name = "HtmlInfoControl";
      this.Size = new Size(540, 402);
      this.ResumeLayout(false);
    }

    [ComVisible(true)]
    public class ScriptProvider
    {
      public object ComicRack { get; set; }

      public string Config { get; set; }
    }
  }
}

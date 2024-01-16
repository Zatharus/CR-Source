// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.ScriptUtility
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Net.Search;
using cYo.Common.Text;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Plugins;
using cYo.Projects.ComicRack.Plugins.Automation;
using cYo.Projects.ComicRack.Plugins.Controls;
using cYo.Projects.ComicRack.Viewer.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public static class ScriptUtility
  {
    public static bool Initialize(
      IWin32Window mainWindow,
      IApplication app,
      IBrowser browser,
      IComicDisplay comicDisplay,
      IPluginConfig config,
      IOpenBooksManager openBooks)
    {
      ScriptUtility.Scripts = new PluginEngine();
      if (!ScriptUtility.Enabled)
        return false;
      PluginEnvironment env = new PluginEnvironment(mainWindow, app, browser, comicDisplay, config, openBooks);
      ScriptUtility.Scripts.Initialize((IPluginEnvironment) env, Program.Paths.ScriptPath);
      ScriptUtility.Scripts.Initialize((IPluginEnvironment) env, Program.Paths.ScriptPathSecondary);
      ScriptUtility.Scripts.CommandStates = Program.Settings.PluginsStates;
      ComicBookDialog.ScriptEngine = ScriptUtility.Scripts;
      ComicBookPluginMatcher.PluginEngine = ScriptUtility.Scripts;
      foreach (Command command in ScriptUtility.Scripts.GetCommands("ParseComicPath"))
      {
        Command c = command;
        ComicBook.ParseFilePath += (EventHandler<ComicBook.ParseFilePathEventArgs>) ((sender, ea) => c.Invoke(new object[2]
        {
          (object) ea.Path,
          (object) ea.NameInfo
        }, true));
      }
      foreach (Command command in ScriptUtility.Scripts.GetCommands("NetSearch"))
        SearchEngines.Engines.Add((INetSearch) new ScriptUtility.ScriptSearch()
        {
          Command = command
        });
      return true;
    }

    public static PluginEngine Scripts { get; private set; }

    public static bool Enabled => Program.Settings.Scripting;

    public static void CreateBookCode(
      Control parent,
      Command command,
      Func<IEnumerable<ComicBook>> books)
    {
      Program.Database.Undo.SetMarker(StringUtility.Format(TR.Messages["UndoScript", "Automation '{0}'"], (object) command.GetLocalizedName()));
      try
      {
        books().ToArray<ComicBook>();
        using (new WaitCursor(parent))
          command.Invoke(new object[1]
          {
            (object) books().ToArray<ComicBook>()
          });
      }
      catch (Exception ex)
      {
        ScriptUtility.ShowError((IWin32Window) parent, ex);
      }
    }

    public static T CreateToolItem<T>(
      Control parent,
      Command command,
      Func<IEnumerable<ComicBook>> books)
      where T : ToolStripItem, new()
    {
      T obj = new T();
      obj.Text = command.GetLocalizedName();
      T toolItem = obj;
      toolItem.Image = command.CommandImage;
      toolItem.ToolTipText = command.GetLocalizedDescription();
      if ((object) toolItem is ToolStripSplitButton)
      {
        ToolStripSplitButton tssb = (ToolStripSplitButton) (object) toolItem;
        tssb.ButtonClick += (EventHandler) ((s, e) => ScriptUtility.CreateBookCode(parent, command, books));
        tssb.MouseDown += (MouseEventHandler) ((s, e) =>
        {
          if (e.Button != MouseButtons.Right)
            return;
          tssb.ShowDropDown();
        });
        if (command.Configure != null)
          tssb.DropDownItems.Add(TR.Default["Configure"] + "...", (Image) null, (EventHandler) ((s, e) => command.Configure.Invoke((object[]) null, true)));
      }
      else
        toolItem.Click += (EventHandler) ((s, e) => ScriptUtility.CreateBookCode(parent, command, books));
      if ((object) toolItem is ToolStripMenuItem)
        ((ToolStripMenuItem) (object) toolItem).ShortcutKeys = command.ShortCutKeys;
      return toolItem;
    }

    public static IEnumerable<T> CreateToolItems<T>(
      Control parent,
      string scriptType,
      Func<IEnumerable<ComicBook>> books,
      Func<Command, bool> predicate = null)
      where T : ToolStripItem, new()
    {
      return ScriptUtility.Scripts == null ? Enumerable.Empty<T>() : ScriptUtility.Scripts.GetCommands(scriptType).Where<Command>((Func<Command, bool>) (command => predicate == null || predicate(command))).Select<Command, T>((Func<Command, T>) (command => ScriptUtility.CreateToolItem<T>(parent, command, books)));
    }

    public static void ShowError(IWin32Window parent, Exception ex)
    {
      int num = (int) MessageBox.Show(parent, ex.Message, TR.Messages["ScriptFailed", "Execution of the script failed!"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }

    public static void Invoke(string hook, params object[] p)
    {
      try
      {
        if (ScriptUtility.Scripts == null)
          return;
        ScriptUtility.Scripts.Invoke(hook, p);
      }
      catch (Exception ex)
      {
      }
    }

    public static IEnumerable<Command> GetCommands(string hook)
    {
      try
      {
        return ScriptUtility.Scripts.GetCommands(hook);
      }
      catch (Exception ex)
      {
        return Enumerable.Empty<Command>();
      }
    }

    public static IEnumerable<ComicPageControl> CreatePagesHtml(string type)
    {
      if (ScriptUtility.Scripts != null)
      {
        foreach (Command command1 in ScriptUtility.Scripts.GetCommands(type))
        {
          Command command = command1;
          HtmlComicPageControl comicPageControl = new HtmlComicPageControl();
          comicPageControl.Text = command.Name;
          comicPageControl.Icon = command.CommandImage;
          comicPageControl.ScriptEngine = (object) command.Environment;
          comicPageControl.ScriptConfig = command.LoadConfig();
          comicPageControl.InfoFunction = (Func<ComicBook[], string>) (b =>
          {
            try
            {
              return command.Invoke(new object[1]
              {
                (object) b
              }) as string;
            }
            catch (Exception ex)
            {
              return ex.ToString();
            }
          });
          comicPageControl.SaveConfigFunction = (Action<string>) (cfg => command.SaveConfig(cfg));
          yield return (ComicPageControl) comicPageControl;
        }
      }
    }

    public static IEnumerable<ComicPageControl> CreatePagesUI(string type)
    {
      if (ScriptUtility.Scripts != null)
      {
        foreach (Command command1 in ScriptUtility.Scripts.GetCommands(type))
        {
          Command command = command1;
          Control c = (Control) null;
          Func<Control> func = (Func<Control>) (() => command.Invoke((object[]) null) as Control);
          try
          {
            c = func();
          }
          catch (Exception ex)
          {
          }
          if (c != null)
          {
            UIComicPageControl comicPageControl = new UIComicPageControl(c);
            comicPageControl.Text = command1.Name;
            comicPageControl.Icon = command1.CommandImage;
            comicPageControl.CreatePlugin = func;
            yield return (ComicPageControl) comicPageControl;
          }
        }
      }
    }

    public static IEnumerable<ComicPageControl> CreateComicInfoPages()
    {
      foreach (ComicPageControl comicInfoPage in ScriptUtility.CreatePagesHtml("ComicInfoHtml"))
        yield return comicInfoPage;
      foreach (ComicPageControl comicInfoPage in ScriptUtility.CreatePagesHtml("ComicInfoUI"))
        yield return comicInfoPage;
    }

    public static IEnumerable<ComicPageControl> CreateQuickOpenPages()
    {
      foreach (ComicPageControl quickOpenPage in ScriptUtility.CreatePagesHtml("QuickOpenHtml"))
        yield return quickOpenPage;
      foreach (ComicPageControl quickOpenPage in ScriptUtility.CreatePagesHtml("QuickOpenUI"))
        yield return quickOpenPage;
    }

    private class ScriptSearch : CachedSearch
    {
      public Command Command { get; set; }

      public override string Name => this.Command.GetLocalizedName();

      public override Image Image => this.Command.CommandImage;

      protected override IEnumerable<SearchResult> OnSearch(string hint, string text, int limit)
      {
        return (this.Command.Invoke(new object[3]
        {
          (object) hint,
          (object) text,
          (object) limit
        }) as Dictionary<string, string>).Select<KeyValuePair<string, string>, SearchResult>((Func<KeyValuePair<string, string>, SearchResult>) (kvp => new SearchResult()
        {
          Name = kvp.Key,
          Result = kvp.Value
        }));
      }

      protected override string OnGenericSearchLink(string hint, string text)
      {
        return (this.Command.Invoke(new object[3]
        {
          (object) hint,
          (object) text,
          (object) -1
        }) as Dictionary<string, string>).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Value)).FirstOrDefault<string>();
      }
    }
  }
}

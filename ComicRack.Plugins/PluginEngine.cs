// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.PluginEngine
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Common.Collections;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  public class PluginEngine
  {
    private readonly PluginInitializer[] initializers = new PluginInitializer[2]
    {
      (PluginInitializer) new XmlPluginInitializer(),
      (PluginInitializer) new PythonPluginInitializer()
    };
    public const string ScriptTypeCreateBookList = "CreateBookList";
    public const string ScriptTypeParseComicPath = "ParseComicPath";
    public const string ScriptTypeLibrary = "Library";
    public const string ScriptTypeEditor = "Editor";
    public const string ScriptTypeBooks = "Books";
    public const string ScriptTypeNewBooks = "NewBooks";
    public const string ScriptTypeBookOpened = "BookOpened";
    public const string ScriptTypeReaderResized = "ReaderResized";
    public const string ScriptTypeSearch = "NetSearch";
    public const string ScriptTypeStartup = "Startup";
    public const string ScriptTypeShutdown = "Shutdown";
    public const string ScriptTypeConfig = "ConfigScript";
    public const string ScriptTypeComicInfoHtml = "ComicInfoHtml";
    public const string ScriptTypeComicInfoUI = "ComicInfoUI";
    public const string ScriptTypeQuickOpenHtml = "QuickOpenHtml";
    public const string ScriptTypeQuickOpenUI = "QuickOpenUI";
    public const string ScriptTypeDrawThumbnailOverlay = "DrawThumbnailOverlay";
    public const string ScriptDescEditBooks = "Edit/Update Books Commands";
    public const string ScriptDescNewBooks = "Create New Books Commands";
    public const string ScriptDescParsePath = "Book Path Parsers";
    public const string ScriptDescBookOpened = "Actions when Books are opened";
    public const string ScriptDescReaderResized = "Actions when Reader is resized";
    public const string ScriptDescSearch = "Additional Search Providers";
    public const string ScriptDescInfo = "Book Information Panels";
    public const string ScriptDescStartup = "Actions when ComicRack starts";
    public const string ScriptDescShutdown = "Actions when ComicRack shuts down";
    public const string ScriptDescQuickOpen = "Quick Open Panels";
    public const string ScriptDescThumbOverlay = "Custom Book Thumbnail Overlays";
    public static readonly Dictionary<string, string> ValidHooks = new Dictionary<string, string>()
    {
      {
        "CreateBookList",
        "Edit/Update Books Commands"
      },
      {
        "ParseComicPath",
        "Book Path Parsers"
      },
      {
        "Library",
        "Edit/Update Books Commands"
      },
      {
        "Editor",
        "Edit/Update Books Commands"
      },
      {
        "Books",
        "Edit/Update Books Commands"
      },
      {
        "NewBooks",
        "Create New Books Commands"
      },
      {
        "BookOpened",
        "Actions when Books are opened"
      },
      {
        "ReaderResized",
        "Actions when Reader is resized"
      },
      {
        "NetSearch",
        "Additional Search Providers"
      },
      {
        "ConfigScript",
        string.Empty
      },
      {
        "Startup",
        "Actions when ComicRack starts"
      },
      {
        "Shutdown",
        "Actions when ComicRack shuts down"
      },
      {
        "ComicInfoHtml",
        "Book Information Panels"
      },
      {
        "ComicInfoUI",
        "Book Information Panels"
      },
      {
        "QuickOpenHtml",
        "Quick Open Panels"
      },
      {
        "QuickOpenUI",
        "Quick Open Panels"
      },
      {
        "DrawThumbnailOverlay",
        "Custom Book Thumbnail Overlays"
      }
    };
    private readonly CommandCollection commands = new CommandCollection();

    private CommandCollection Commands => this.commands;

    public string CommandStates
    {
      get
      {
        return this.Commands.Select<Command, string>((Func<Command, string>) (cmd => (cmd.Enabled ? "+" : "-") + cmd.Key)).ToListString(",");
      }
      set
      {
        value.FromListString(',').SafeForEach<string>((Action<string>) (s => this.Commands[s.Substring(1)].Enabled = s[0] == '+'));
      }
    }

    public IEnumerable<Command> GetAllCommands() => (IEnumerable<Command>) this.Commands;

    public IEnumerable<Command> GetCommands(string hook)
    {
      return this.Commands.Where<Command>((Func<Command, bool>) (cmd =>
      {
        if (!cmd.Enabled)
          return false;
        return cmd.IsHook(hook);
      }));
    }

    public void Initialize(IPluginEnvironment env, string path)
    {
      List<Command> commandList = new List<Command>();
      string[] array = PluginEngine.ValidHooks.Keys.ToArray<string>();
      foreach (string file1 in FileUtility.GetFiles(path, SearchOption.AllDirectories))
      {
        string file = file1;
        foreach (Command command in ((IEnumerable<PluginInitializer>) this.initializers).SelectMany<PluginInitializer, Command>((Func<PluginInitializer, IEnumerable<Command>>) (si => si.GetCommands(file))))
        {
          Command cmd = command;
          try
          {
            if (cmd.IsHook(array))
            {
              if (cmd.Initialize(env, Path.GetDirectoryName(file)))
              {
                if (cmd.Hook == "ConfigScript")
                  commandList.Add(cmd);
                else if (!this.commands.Exists((Predicate<Command>) (c => c.Key == cmd.Key)))
                {
                  if (cmd.Enabled)
                  {
                    int num = this.commands.Count<Command>((Func<Command, bool>) (c => c.Enabled));
                    cmd.ShortCutKeys = num < 12 ? (Keys) (196608 | 112 + num) : Keys.None;
                  }
                  this.commands.Add(cmd);
                }
              }
            }
          }
          catch
          {
          }
        }
        foreach (Command command1 in commandList)
        {
          Command cfg = command1;
          Command command2 = this.commands.FirstOrDefault<Command>((Func<Command, bool>) (c => c.Key == cfg.Key));
          if (command2 != null)
            command2.Configure = cfg;
        }
      }
    }

    public void Invoke(string hook, object[] data)
    {
      this.GetCommands(hook).ForEach<Command>((Action<Command>) (c => c.Invoke(data)));
    }

    public static string GetHookDescription(string hook)
    {
      string key = ((IEnumerable<string>) hook.Split(',')).TrimStrings().RemoveEmpty().FirstOrDefault<string>();
      string s;
      return string.IsNullOrEmpty(key) || !PluginEngine.ValidHooks.TryGetValue(key, out s) ? string.Empty : TR.Load(nameof (PluginEngine))[s.ReplaceAny("/ ", string.Empty), s];
    }
  }
}

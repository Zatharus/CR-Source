// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.PythonCommand
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Common.Collections;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Projects.ComicRack.Engine;
using IronPython.Hosting;
using IronPython.Modules;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  public class PythonCommand : Command
  {
    private const string HostName = "ComicRack";
    private const string ScriptPathName = "ScriptPath";
    private Delegate script;
    private DateTime scriptModificationTime;
    private static readonly Dictionary<string, PythonCommand.ScriptCreateHandler> hookTypes = new Dictionary<string, PythonCommand.ScriptCreateHandler>()
    {
      {
        "Library",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action<ComicBook[]>>(n))
      },
      {
        "Books",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action<ComicBook[]>>(n))
      },
      {
        "NewBooks",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action<ComicBook[]>>(n))
      },
      {
        "ParseComicPath",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action<string, ComicNameInfo>>(n))
      },
      {
        "BookOpened",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action<ComicBook>>(n))
      },
      {
        "CreateBookList",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Func<ComicBook[], string, string, IEnumerable<ComicBook>>>(n))
      },
      {
        "ReaderResized",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action<int, int>>(n))
      },
      {
        "NetSearch",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Func<string, string, int, Dictionary<string, string>>>(n))
      },
      {
        "ConfigScript",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action>(n))
      },
      {
        "Startup",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action>(n))
      },
      {
        "Shutdown",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Func<bool, bool>>(n))
      },
      {
        "ComicInfoHtml",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Func<ComicBook[], string>>(n))
      },
      {
        "ComicInfoUI",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Func<Control>>(n))
      },
      {
        "QuickOpenHtml",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Func<ComicBook[], string>>(n))
      },
      {
        "QuickOpenUI",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Func<Control>>(n))
      },
      {
        "DrawThumbnailOverlay",
        (PythonCommand.ScriptCreateHandler) ((s, n) => (Delegate) s.GetVariable<Action<ComicBook, Graphics, Rectangle, int>>(n))
      }
    };
    private static readonly PythonCommand.PythonSettings settings = IniFile.Default.Register<PythonCommand.PythonSettings>();
    private static ScriptEngine expressionEngine;
    private ScriptScope scope;
    private PythonCommand.ScriptCreateHandler hookType;

    static PythonCommand() => PythonCommand.Optimized = true;

    public static bool Optimized { get; set; }

    public static Stream Output { get; set; }

    public static bool EnableLog { get; set; }

    private static ScriptEngine CreateEngine()
    {
      ScriptRuntimeSetup setup = new ScriptRuntimeSetup()
      {
        DebugMode = PythonCommand.settings.PythonDebug
      };
      Dictionary<string, object> options = new Dictionary<string, object>();
      if (PythonCommand.settings.PythonEnableFrames)
        options["Frames"] = (object) true;
      if (PythonCommand.settings.PythonEnableFullFrames)
        options["FullFrames"] = (object) true;
      setup.LanguageSetups.Add(Python.CreateLanguageSetup((IDictionary<string, object>) options));
      ScriptRuntime scriptRuntime = new ScriptRuntime(setup);
      ScriptEngine engineByTypeName = scriptRuntime.GetEngineByTypeName(typeof (PythonContext).AssemblyQualifiedName);
      engineByTypeName.Runtime.LoadAssembly(typeof (ArgumentNullException).Assembly);
      engineByTypeName.Runtime.LoadAssembly(typeof (ArrayModule).Assembly);
      engineByTypeName.Runtime.LoadAssembly(typeof (ComicBook).Assembly);
      if (PythonCommand.Output != null)
      {
        scriptRuntime.IO.SetErrorOutput(PythonCommand.Output, Encoding.Default);
        scriptRuntime.IO.SetOutput(PythonCommand.Output, Encoding.Default);
      }
      return engineByTypeName;
    }

    public static T CompileExpression<T>(string source, params string[] parameters) where T : class
    {
      source = "def f(" + parameters.ToListString(",") + "):\n\treturn " + source.Trim();
      try
      {
        ScriptEngine scriptEngine = PythonCommand.expressionEngine ?? (PythonCommand.expressionEngine = PythonCommand.CreateEngine());
        ScriptSource sourceFromString = scriptEngine.CreateScriptSourceFromString(source);
        ScriptScope scope = scriptEngine.CreateScope();
        sourceFromString.Execute(scope);
        T obj;
        return scope.TryGetVariable<T>("f", out obj) ? obj : default (T);
      }
      catch (Exception ex)
      {
        return default (T);
      }
    }

    public static void LogDefault(string text, params object[] o)
    {
      if (PythonCommand.Output == null || !PythonCommand.EnableLog)
        return;
      using (StreamWriter streamWriter = new StreamWriter(PythonCommand.Output))
        streamWriter.WriteLine(text, o);
    }

    public string ScriptFile { get; set; }

    public string Method { get; set; }

    [XmlIgnore]
    public string LibPath { get; private set; }

    private ScriptScope Scope
    {
      get
      {
        this.PreCompile(false);
        return this.scope;
      }
    }

    private PythonCommand.ScriptCreateHandler HookType
    {
      get
      {
        if (this.hookType == null)
        {
          string hook = this.Hook;
          char[] chArray = new char[1]{ ',' };
          foreach (string str in hook.Split(chArray))
          {
            PythonCommand.ScriptCreateHandler scriptCreateHandler;
            if (PythonCommand.hookTypes.TryGetValue(str.Trim(), out scriptCreateHandler))
              this.hookType = scriptCreateHandler;
          }
        }
        return this.hookType;
      }
    }

    protected override void Log(string text, params object[] o)
    {
      base.Log(text, o);
      PythonCommand.LogDefault(text, o);
    }

    protected override void OnInitialize(IPluginEnvironment env, string path)
    {
      base.OnInitialize(env, path);
      try
      {
        this.LibPath = path;
        this.Log("\nInitialzing script '{0}' from '{1}'", (object) this.Method, (object) this.ScriptFile);
        this.ScriptFile = Command.GetFile(path, this.ScriptFile);
        if (!File.Exists(this.ScriptFile))
          throw new FileNotFoundException("Script file not found");
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
    }

    protected override object OnInvoke(object[] data)
    {
      try
      {
        this.Log("Calling '{0}'...", (object) this.Method);
        if (!PythonCommand.Optimized)
          this.CheckScript();
        if ((object) this.script == null && this.HookType != null)
          this.script = this.HookType(this.Scope, this.Method);
        if ((object) this.script != null)
          return this.script.DynamicInvoke(data);
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        throw;
      }
      return (object) null;
    }

    protected override void MakeDefaults()
    {
      if (string.IsNullOrEmpty(this.Key))
        this.Key = this.Method;
      if (!string.IsNullOrEmpty(this.Name))
        return;
      this.Name = this.Method;
    }

    protected override bool IsValid
    {
      get
      {
        return base.IsValid && !string.IsNullOrEmpty(this.Method) && !string.IsNullOrEmpty(this.ScriptFile);
      }
    }

    public override string LoadConfig()
    {
      try
      {
        return File.ReadAllText(this.ConfigFileName);
      }
      catch
      {
        return string.Empty;
      }
    }

    public override bool SaveConfig(string config)
    {
      try
      {
        File.WriteAllText(this.ConfigFileName, config);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public override void OnPreCompile(bool handleException)
    {
      try
      {
        if (this.scope != null)
          return;
        this.Log("Compilation of '{0}'", (object) this.ScriptFile);
        ScriptEngine engine = PythonCommand.CreateEngine();
        if (!string.IsNullOrEmpty(this.LibPath) || !this.Environment.LibraryPaths.IsEmpty<string>())
          engine.SetSearchPaths((ICollection<string>) engine.GetSearchPaths().Concat<string>(this.Environment.LibraryPaths).AddLast<string>(this.LibPath).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<string>());
        ScriptSource scriptSourceFromFile = engine.CreateScriptSourceFromFile(this.ScriptFile);
        this.scriptModificationTime = File.GetLastWriteTimeUtc(this.ScriptFile);
        this.scope = engine.CreateScope();
        this.scope.SetVariable("ComicRack", (object) this.Environment);
        this.scope.SetVariable("ScriptPath", (object) Path.GetDirectoryName(this.ScriptFile));
        scriptSourceFromFile.Execute(this.scope);
        this.script = (Delegate) null;
      }
      catch (Exception ex)
      {
        if (handleException)
          this.HandleException(ex);
        else
          throw;
      }
    }

    private string ConfigFileName => this.ScriptFile + "-" + this.Method + ".config";

    private void HandleException(Exception e)
    {
      if (e.InnerException != null)
        e = e.InnerException;
      if (!(e is SyntaxErrorException syntaxErrorException))
      {
        this.Log(e.Message);
      }
      else
      {
        this.Log("Syntax error at [{0}, {1}]: {2}", (object) syntaxErrorException.Line, (object) syntaxErrorException.Column, (object) syntaxErrorException.Message);
        this.Log("\tin file '{0}'", (object) syntaxErrorException.SourcePath);
      }
    }

    private void CheckScript()
    {
      if (!(File.GetLastWriteTimeUtc(this.ScriptFile) != this.scriptModificationTime))
        return;
      this.scope = (ScriptScope) null;
      this.script = (Delegate) null;
    }

    private class PythonSettings
    {
      public bool PythonDebug { get; set; }

      public bool PythonEnableFrames { get; set; }

      public bool PythonEnableFullFrames { get; set; }
    }

    private delegate Delegate ScriptCreateHandler(ScriptScope scope, string method);
  }
}

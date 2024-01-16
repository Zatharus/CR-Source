// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.Command
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Common.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  public abstract class Command
  {
    private bool enabled = true;
    private Keys shortCutKeys;

    [XmlAttribute]
    [DefaultValue(null)]
    public string Hook { get; set; }

    [XmlAttribute]
    [DefaultValue(0)]
    public int PCount { get; set; }

    [XmlAttribute]
    [DefaultValue(null)]
    public string Key { get; set; }

    public string Name { get; set; }

    [DefaultValue(null)]
    public string Description { get; set; }

    [DefaultValue(null)]
    public string Image { get; set; }

    [XmlIgnore]
    public System.Drawing.Image CommandImage { get; set; }

    [XmlIgnore]
    public IPluginEnvironment Environment { get; set; }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool Enabled
    {
      get => this.enabled;
      set => this.enabled = value;
    }

    [XmlIgnore]
    public Keys ShortCutKeys
    {
      get => this.shortCutKeys;
      set => this.shortCutKeys = value;
    }

    [XmlIgnore]
    public Command Configure { get; set; }

    public bool Initialize(IPluginEnvironment env, string path)
    {
      try
      {
        this.MakeDefaults();
        if (!this.IsValid)
          return false;
        this.OnInitialize(env, path);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public object Invoke(object[] data, bool catchErrors = false)
    {
      try
      {
        return this.OnInvoke(data);
      }
      catch (Exception ex)
      {
        if (catchErrors)
          return (object) null;
        throw;
      }
    }

    public bool IsHook(params string[] hooks)
    {
      return ((IEnumerable<string>) hooks).Any<string>((Func<string, bool>) (hook => this.Hook.Contains(hook)));
    }

    public void PreCompile(bool handleException = true) => this.OnPreCompile(handleException);

    public string GetLocalizedName()
    {
      return this.Environment != null ? this.Environment.Localize(this.Key, "Name", this.Name) : this.Name;
    }

    public string GetLocalizedDescription()
    {
      return this.Environment != null ? this.Environment.Localize(this.Key, "Description", this.Description) : this.Name;
    }

    protected static string GetFile(string basePath, string file)
    {
      return !File.Exists(file) ? Path.Combine(basePath, Path.GetFileName(file)) : file;
    }

    protected virtual void Log(string text, params object[] o)
    {
    }

    protected virtual void OnInitialize(IPluginEnvironment env, string path)
    {
      this.Environment = env.Clone() as IPluginEnvironment;
      if (this.Environment != null)
        this.Environment.CommandPath = path;
      try
      {
        if (string.IsNullOrEmpty(this.Image))
          return;
        this.CommandImage = (System.Drawing.Image) System.Drawing.Image.FromFile(Command.GetFile(path, this.Image)).CreateCopy(true);
      }
      catch
      {
        this.CommandImage = (System.Drawing.Image) null;
      }
    }

    protected virtual void MakeDefaults()
    {
    }

    protected virtual bool IsValid
    {
      get => !string.IsNullOrEmpty(this.Hook) && !string.IsNullOrEmpty(this.Name);
    }

    public virtual string LoadConfig() => string.Empty;

    public virtual bool SaveConfig(string config) => false;

    public virtual void OnPreCompile(bool handleException)
    {
    }

    protected abstract object OnInvoke(object[] data);
  }
}

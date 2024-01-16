// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.IniFile
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Runtime
{
  public class IniFile : DisposableObject
  {
    public const string MultiInitFileSeparator = "|";
    private FileSystemWatcher[] fsw;
    private readonly Dictionary<string, string> values = new Dictionary<string, string>();
    private static readonly Regex rxCommand = new Regex("[/-](?<switch>[a-z]+)[:=](?<value>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static List<string> extraDefaultLocation;
    private static string defaultIniFile;
    private static IniFile defaultIni;

    public IniFile(string file = null, string section = null, object data = null)
    {
      if (string.IsNullOrEmpty(file))
        return;
      this.InitializeFile(file, section);
      this.Register(data);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.CloseWatcher();
      base.Dispose(disposing);
    }

    public Dictionary<string, string> Values => this.values;

    public void InitializeFile(string file, string section = null)
    {
      this.CloseWatcher();
      try
      {
        this.UpdateValues((IDictionary<string, string>) IniFile.ReadFile(file, section));
        List<FileSystemWatcher> fileSystemWatcherList = new List<FileSystemWatcher>();
        try
        {
          foreach (string file1 in IniFile.GetFiles(file))
          {
            string watchFile = file1;
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(watchFile))
            {
              EnableRaisingEvents = true,
              NotifyFilter = NotifyFilters.LastWrite
            };
            fileSystemWatcher.Changed += (FileSystemEventHandler) ((x, y) =>
            {
              if (!(y.FullPath == watchFile))
                return;
              this.UpdateValues((IDictionary<string, string>) IniFile.ReadFile(watchFile, section));
            });
            fileSystemWatcherList.Add(fileSystemWatcher);
          }
        }
        catch (Exception ex)
        {
        }
        this.fsw = fileSystemWatcherList.ToArray();
      }
      catch (Exception ex)
      {
      }
    }

    public T GetValue<T>(string name, T def = null) => IniFile.GetValue<T>(this.values, name, def);

    public T Register<T>(IniFileRegisterOptions options) where T : class
    {
      T instance = Activator.CreateInstance<T>();
      this.Register((object) instance, options);
      return instance;
    }

    public T Register<T>() where T : class
    {
      return this.Register<T>(IniFileRegisterOptions.ReadCommandLine);
    }

    public void Register(object data, IniFileRegisterOptions options = IniFileRegisterOptions.ReadCommandLine)
    {
      if (data == null)
        return;
      if (options.IsSet<IniFileRegisterOptions>(IniFileRegisterOptions.ReadCommandLine))
        this.UpdateValues((IDictionary<string, string>) IniFile.ReadCommandLine());
      IniFile.UpdateProperties(data, this.values);
      if (!options.IsSet<IniFileRegisterOptions>(IniFileRegisterOptions.WatchIniFile))
        return;
      this.ValuesChanged += (EventHandler) ((x, y) => IniFile.UpdateProperties(data, this.Values));
    }

    public event EventHandler ValuesChanged;

    protected virtual void OnValuesChanged()
    {
      if (this.ValuesChanged == null)
        return;
      this.ValuesChanged((object) this, EventArgs.Empty);
    }

    private void CloseWatcher()
    {
      if (this.fsw == null)
        return;
      this.fsw.Dispose();
      this.fsw = (FileSystemWatcher[]) null;
    }

    private void UpdateValues(IDictionary<string, string> values)
    {
      try
      {
        this.values.AddRange<string, string>(values);
        this.OnValuesChanged();
      }
      catch
      {
      }
    }

    private static PropertyInfo GetProperty(Type t, string name)
    {
      foreach (PropertyInfo property in t.GetProperties())
      {
        string name1 = property.Name;
        if (property.CanWrite && (!(Attribute.GetCustomAttribute((MemberInfo) property, typeof (IniFileAttribute)) is IniFileAttribute customAttribute) || customAttribute.Enabled))
        {
          if (customAttribute != null && !string.IsNullOrEmpty(customAttribute.Name))
            name1 = customAttribute.Name;
          if (name1.Equals(name, StringComparison.OrdinalIgnoreCase))
            return property;
        }
      }
      return (PropertyInfo) null;
    }

    public static object GetValue(
      Dictionary<string, string> values,
      string name,
      Type type,
      object def)
    {
      return IniFile.GetValue(values, name, TypeDescriptor.GetConverter(type), def);
    }

    public static object GetValue(
      Dictionary<string, string> values,
      string name,
      TypeConverter converter,
      object def)
    {
      string text;
      if (!values.TryGetValue(name, out text))
        return def;
      try
      {
        return converter.ConvertFromInvariantString(text);
      }
      catch (InvalidCastException ex)
      {
        return def;
      }
    }

    public static T GetValue<T>(Dictionary<string, string> values, string name, T def)
    {
      return (T) IniFile.GetValue(values, name, typeof (T), (object) def);
    }

    public static T GetValue<T>(string file, string name, T defaultValue, string section = null)
    {
      try
      {
        return !IniFile.FileExists(file) ? defaultValue : IniFile.GetValue<T>(IniFile.ReadFile(file, section), name, defaultValue);
      }
      catch (Exception ex)
      {
        return defaultValue;
      }
    }

    public static Dictionary<string, string> GetValues(TextReader tr, string section = null)
    {
      Dictionary<string, string> values = new Dictionary<string, string>();
      bool flag1 = !string.IsNullOrEmpty(section);
      bool flag2 = !flag1;
      foreach (string s in tr.ReadLines().TrimStrings().RemoveEmpty().Where<string>((Func<string, bool>) (s => !s.StartsWith(";") && !s.StartsWith("#"))))
      {
        if (s.StartsWith("["))
        {
          if (flag1)
          {
            if (!flag2)
              flag2 = string.Equals(s.Substring(1, s.Length - 2), section, StringComparison.OrdinalIgnoreCase);
            else
              break;
          }
          else
            continue;
        }
        if (flag2)
        {
          int lengthFirstPart = s.IndexOf('=');
          if (lengthFirstPart != -1)
          {
            string[] strArray = s.Split(lengthFirstPart, 1);
            string key = strArray[0].Trim();
            if (!string.IsNullOrEmpty(key))
              values[key] = strArray[1].TrimStart();
          }
        }
      }
      return values;
    }

    public static Dictionary<string, string> ReadFile(string file, string section = null)
    {
      Dictionary<string, string> dict = new Dictionary<string, string>();
      try
      {
        foreach (string path in IniFile.GetFiles(file).Where<string>(new Func<string, bool>(File.Exists)))
        {
          using (StreamReader tr = File.OpenText(path))
            dict.AddRange<string, string>((IDictionary<string, string>) IniFile.GetValues((TextReader) tr, section));
        }
      }
      catch (Exception ex)
      {
      }
      return dict;
    }

    public static IEnumerable<string> ReadSections(TextReader tr)
    {
      return tr.ReadLines().TrimStrings().RemoveEmpty().Where<string>((Func<string, bool>) (s => s.StartsWith("["))).Select<string, string>((Func<string, string>) (s => s.Substring(1, s.Length - 2)));
    }

    public static IEnumerable<string> ReadSections(string file)
    {
      List<string> source = new List<string>();
      try
      {
        foreach (string path in IniFile.GetFiles(file).Where<string>(new Func<string, bool>(File.Exists)))
        {
          using (StreamReader tr = File.OpenText(path))
          {
            foreach (string readSection in IniFile.ReadSections((TextReader) tr))
            {
              if (!source.Contains<string>(readSection, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
                source.Add(readSection);
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return (IEnumerable<string>) source;
    }

    public static Dictionary<string, string> ReadCommandLine()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (string commandLineArg in Environment.GetCommandLineArgs())
      {
        Match match = IniFile.rxCommand.Match(commandLineArg);
        if (match.Success)
          dictionary[match.Groups["switch"].Value] = match.Groups["value"].Value;
      }
      return dictionary;
    }

    public static void UpdateProperties(
      object data,
      Dictionary<string, string> values,
      bool throwException = false)
    {
      if (data == null)
        return;
      Type type = data.GetType();
      foreach (string key in values.Keys)
      {
        PropertyInfo property = IniFile.GetProperty(type, key);
        if (!(property == (PropertyInfo) null))
        {
          try
          {
            TypeConverter converter = property.GetCustomAttributes(true).OfType<TypeConverterAttribute>().Select<TypeConverterAttribute, object>((Func<TypeConverterAttribute, object>) (x => Activator.CreateInstance(Type.GetType(x.ConverterTypeName)))).Cast<TypeConverter>().FirstOrDefault<TypeConverter>() ?? TypeDescriptor.GetConverter(property.PropertyType);
            object obj = IniFile.GetValue(values, key, converter, (object) null);
            if (obj != null)
              property.SetValue(data, obj, (object[]) null);
          }
          catch
          {
            if (throwException)
              throw;
          }
        }
      }
    }

    public static void UpdateProperties(object data, string file)
    {
      IniFile.UpdateProperties(data, IniFile.ReadFile(file));
    }

    public static void UpdateProperties(object data, bool withCommandLine)
    {
      IniFile.UpdateProperties(data, IniFile.DefaultIniFile);
      if (!withCommandLine)
        return;
      IniFile.UpdateProperties(data, IniFile.ReadCommandLine());
    }

    public static IEnumerable<string> GetDefaultLocations(string file)
    {
      yield return Path.Combine(IniFile.StartupFolder, file);
      yield return Path.Combine(IniFile.CommonApplicationDataFolder, file);
      if (IniFile.extraDefaultLocation == null)
      {
        yield return Path.Combine(IniFile.ApplicationDataFolder, file);
      }
      else
      {
        foreach (string path1 in IniFile.extraDefaultLocation)
          yield return Path.Combine(path1, file);
      }
    }

    public static void AddDefaultLocation(string path)
    {
      if (IniFile.extraDefaultLocation == null)
        IniFile.extraDefaultLocation = new List<string>();
      IniFile.extraDefaultLocation.Add(path);
      IniFile.defaultIniFile = (string) null;
      IniFile.defaultIni = (IniFile) null;
    }

    private static IEnumerable<string> GetFiles(string file)
    {
      return (IEnumerable<string>) file.Split("|".ToCharArray());
    }

    private static bool FileExists(string file)
    {
      return IniFile.GetFiles(file).Any<string>((Func<string, bool>) (f => File.Exists(file)));
    }

    private static string MakeApplicationPath(string folder)
    {
      folder = Path.Combine(folder, Application.CompanyName);
      folder = Path.Combine(folder, Application.ProductName);
      return folder;
    }

    public static string StartupFolder
    {
      get => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    }

    public static string CommonApplicationDataFolder
    {
      get
      {
        return IniFile.MakeApplicationPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
      }
    }

    public static string ApplicationDataFolder
    {
      get
      {
        return IniFile.MakeApplicationPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
      }
    }

    public static string DefaultIniFile
    {
      get
      {
        if (IniFile.defaultIniFile == null)
        {
          try
          {
            IniFile.defaultIniFile = IniFile.GetDefaultLocations(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".ini").ToListString("|");
          }
          catch (Exception ex)
          {
            IniFile.defaultIniFile = string.Empty;
          }
        }
        return IniFile.defaultIniFile;
      }
    }

    public static IniFile Default
    {
      get
      {
        if (IniFile.defaultIni == null)
          IniFile.defaultIni = new IniFile(IniFile.DefaultIniFile);
        return IniFile.defaultIni;
      }
    }
  }
}

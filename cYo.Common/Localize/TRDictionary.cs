// Decompiled with JetBrains decompiler
// Type: cYo.Common.Localize.TRDictionary
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.IO;
using cYo.Common.Threading;
using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable disable
namespace cYo.Common.Localize
{
  public class TRDictionary : Dictionary<string, TR>
  {
    public const string LanguageInfoFile = "LanguageInfo.xml";
    private IVirtualFolder resourceFolder = (IVirtualFolder) TRDictionary.GetDefaultResourceFolder();
    private static TRInfo defaultInfo;

    public TRDictionary()
    {
    }

    public TRDictionary(IVirtualFolder folder, string path)
    {
      foreach (string file in folder.GetFiles(path))
      {
        if (".xml".Equals(Path.GetExtension(file), StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            using (Stream s = folder.OpenRead(file))
            {
              TR tr = XmlUtility.Load<TR>(s, false);
              tr.File = file;
              this.Add(Path.GetFileName(file), tr);
            }
          }
          catch
          {
          }
        }
      }
    }

    public TRDictionary(string path)
      : this((IVirtualFolder) new VirtualFileFolder(), path)
    {
    }

    public CultureInfo DefaultCulture { get; set; }

    public IVirtualFolder ResourceFolder
    {
      get => this.resourceFolder;
      set => this.resourceFolder = value;
    }

    public TRInfo Info
    {
      get
      {
        if (TRDictionary.defaultInfo == null)
        {
          CultureInfo culture = this.DefaultCulture ?? CultureInfo.CurrentUICulture;
          TRDictionary.defaultInfo = this.GetLanguageInfo(culture);
          if (TRDictionary.defaultInfo == null)
            TRDictionary.defaultInfo = new TRInfo(culture.Name);
        }
        return TRDictionary.defaultInfo;
      }
    }

    public void Save(string path)
    {
      foreach (string file in Directory.GetFiles(path, "*.xml"))
      {
        string fileName = Path.GetFileName(file);
        if (!string.Equals(fileName, "LanguageInfo.xml", StringComparison.OrdinalIgnoreCase) && !this.ContainsKey(fileName))
          File.Delete(file);
      }
      foreach (string key in this.Keys)
        XmlUtility.Store(Path.Combine(path, key), (object) this[key]);
    }

    public HashSet<TREntry> CreateSet()
    {
      HashSet<TREntry> list = new HashSet<TREntry>();
      foreach (TR tr in this.Values)
        list.AddRange<TREntry>((IEnumerable<TREntry>) tr.Texts);
      return list;
    }

    public void SaveAllText(string path)
    {
      using (StreamWriter text = File.CreateText(path))
      {
        foreach (TREntry trEntry in this.CreateSet())
          text.WriteLine(trEntry.Text);
      }
    }

    public void Update(TRDictionary newPack)
    {
      foreach (string key in this.Keys.ToArray<string>())
      {
        if (!newPack.ContainsKey(key))
          this.Remove(key);
      }
      foreach (string key in newPack.Keys)
      {
        TR tr1 = newPack[key];
        TR tr2;
        if (!this.TryGetValue(key, out tr2))
        {
          tr2 = new TR(tr1.Name, tr1.Culture);
          foreach (TREntry text in (SmartList<TREntry>) tr1.Texts)
            tr2.Texts.Add(new TREntry(text.Key, text.Comment, text.Comment));
          this.Add(key, tr2);
        }
        else
          tr2.Texts.Update(tr1.Texts);
      }
    }

    public float CompletionPercent(TRDictionary newPack)
    {
      int num1 = 0;
      int num2 = 0;
      foreach (string key in newPack.Keys)
      {
        TR tr1 = newPack[key];
        num1 += tr1.Texts.Count;
        TR tr2;
        if (this.TryGetValue(key, out tr2))
        {
          foreach (TREntry text1 in (SmartList<TREntry>) tr1.Texts)
          {
            TREntry text2 = tr2.Texts[text1.Key];
            if (text2 != null && text2.Comment == text1.Comment)
              ++num2;
          }
        }
      }
      return num1 != 0 ? 100f * (float) num2 / (float) num1 : 100f;
    }

    public TR Load(string name, CultureInfo culture)
    {
      using (ItemMonitor.Lock((object) this))
      {
        TR tr;
        if (this.TryGetValue(name, out tr))
          return tr;
        CultureInfo topLevelCulture = TRDictionary.GetTopLevelCulture(culture);
        tr = this.LoadFile(name, topLevelCulture);
        if (culture.Name != topLevelCulture.Name)
          tr.Texts.Merge((IEnumerable<TREntry>) this.LoadFile(name, culture).Texts);
        return this[name] = tr;
      }
    }

    public TR Load(string name)
    {
      return this.Load(name, this.DefaultCulture ?? CultureInfo.CurrentUICulture);
    }

    public void Save()
    {
      using (ItemMonitor.Lock((object) this))
      {
        foreach (TR tr in this.Values)
          tr.Save(this.ResourceFolder, TRDictionary.GetFileName(tr.Name, tr.Culture));
      }
    }

    public TRInfo GetLanguageInfo(CultureInfo culture)
    {
      string path = Path.Combine(culture.Name, "LanguageInfo.xml");
      TRInfo languageInfo = (TRInfo) null;
      try
      {
        if (this.ResourceFolder.FileExists(path))
        {
          using (Stream s = this.ResourceFolder.OpenRead(path))
          {
            languageInfo = XmlUtility.Load<TRInfo>(s, false);
            languageInfo.CultureName = culture.Name;
          }
        }
      }
      catch
      {
      }
      return languageInfo;
    }

    public IEnumerable<TRInfo> GetLanguageInfos()
    {
      return ((IEnumerable<CultureInfo>) CultureInfo.GetCultures(CultureTypes.AllCultures)).Select<CultureInfo, TRInfo>(new Func<CultureInfo, TRInfo>(this.GetLanguageInfo)).Where<TRInfo>((Func<TRInfo, bool>) (tri => tri != null));
    }

    public static TR LoadFile(IVirtualFolder folder, string name, CultureInfo culture)
    {
      TR tr;
      try
      {
        string fileName = TRDictionary.GetFileName(name, culture);
        using (Stream s = folder.OpenRead(fileName))
          tr = XmlUtility.Load<TR>(s, false);
        tr.File = fileName;
        foreach (TREntry text in (SmartList<TREntry>) tr.Texts)
          text.Text = text.Text.Replace("\\n", "\r\n").Replace("\\r", "").Replace("\\t", "\t");
      }
      catch (Exception ex)
      {
        tr = new TR(name, culture);
      }
      return tr;
    }

    public TR LoadFile(string name, CultureInfo culture)
    {
      return TRDictionary.LoadFile(this.ResourceFolder, name, culture);
    }

    private static VirtualFileFolder GetDefaultResourceFolder()
    {
      try
      {
        return new VirtualFileFolder(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Languages"));
      }
      catch (Exception ex)
      {
        return (VirtualFileFolder) null;
      }
    }

    private static CultureInfo GetTopLevelCulture(CultureInfo culture)
    {
      try
      {
        return new CultureInfo(culture.Name.Split('-')[0]);
      }
      catch
      {
        return culture;
      }
    }

    private static string GetFileName(string name, CultureInfo culture)
    {
      return Path.Combine(culture.Name, name + ".xml");
    }
  }
}

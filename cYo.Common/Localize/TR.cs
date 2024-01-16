// Decompiled with JetBrains decompiler
// Type: cYo.Common.Localize.TR
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.IO;
using cYo.Common.Reflection;
using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Localize
{
  public class TR
  {
    private readonly TREntryList texts;
    private CultureInfo culture = CultureInfo.CurrentUICulture;
    private static readonly TRDictionary languageResources = new TRDictionary();
    private static TR defaultTR;
    private static TR messagesTR;

    public TR() => this.texts = new TREntryList(this);

    public TR(string name, CultureInfo culture)
      : this()
    {
      this.Name = name;
      this.culture = culture;
    }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public string CultureName
    {
      get => this.culture.Name;
      set => this.culture = new CultureInfo(value);
    }

    [XmlArray("Texts")]
    [XmlArrayItem("Text")]
    public TREntryList Texts => this.texts;

    [XmlIgnore]
    public CultureInfo Culture
    {
      get => this.culture;
      set => this.culture = value;
    }

    [XmlIgnore]
    public string File { get; set; }

    public string FileName => Path.GetFileName(this.File);

    public bool IsEmpty => this.texts.Count == 0;

    public string this[string key, string value] => this.texts.GetText(key, value);

    public string this[string key] => this.texts.GetText(key, key);

    public string[] GetStrings(string key, string array, char sep)
    {
      string[] strings1 = array.Split(sep);
      try
      {
        string[] strings2 = this[key, array].Split(sep);
        if (strings1.Length == strings2.Length)
          return strings2;
      }
      catch (Exception ex)
      {
      }
      return strings1;
    }

    public void Save(IVirtualFolder folder, string path)
    {
      this.Texts.Sort();
      byte[] second = XmlUtility.Store((object) this, false);
      try
      {
        using (Stream stream = folder.OpenRead(path))
        {
          byte[] numArray = new byte[stream.Length];
          stream.Read(numArray, 0, (int) stream.Length);
          if (((IEnumerable<byte>) numArray).SequenceEqual<byte>((IEnumerable<byte>) second))
            return;
        }
      }
      catch (Exception ex)
      {
      }
      try
      {
        folder.CreateFolder(Path.GetDirectoryName(path));
        XmlUtility.Store(folder.Create(path), (object) this, false);
      }
      catch (Exception ex)
      {
      }
    }

    public static string Translate(Enum e) => TR.Load(e.GetType().Name)[e.Name(), e.Description()];

    public static TRDictionary LanguageResources => TR.languageResources;

    public static TRInfo Info => TR.LanguageResources.Info;

    public static TR Load(string name) => TR.LanguageResources.Load(name);

    public static IEnumerable<TRInfo> GetLanguageInfos() => TR.LanguageResources.GetLanguageInfos();

    public static CultureInfo DefaultCulture
    {
      get => TR.LanguageResources.DefaultCulture;
      set => TR.LanguageResources.DefaultCulture = value;
    }

    public static IVirtualFolder ResourceFolder
    {
      get => TR.LanguageResources.ResourceFolder;
      set => TR.LanguageResources.ResourceFolder = value;
    }

    public static TR Default
    {
      get
      {
        if (TR.defaultTR == null)
          TR.defaultTR = TR.Load(nameof (Default));
        return TR.defaultTR;
      }
    }

    public static TR Messages
    {
      get
      {
        if (TR.messagesTR == null)
          TR.messagesTR = TR.Load(nameof (Messages));
        return TR.messagesTR;
      }
    }
  }
}

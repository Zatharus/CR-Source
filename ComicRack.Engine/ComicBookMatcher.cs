// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Localize;
using cYo.Common.Reflection;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public abstract class ComicBookMatcher : IComicBookMatcher, IMatcher<ComicBook>, ICloneable
  {
    public const string ClipboardFormat = "ComicBookMatcher";
    private static TR trMatcher;
    public const string SeriesStatsPropertyPrefix = "Stats";
    public static readonly string[] ComicProperties = ComicBook.GetProperties(true).ToArray<string>();
    public static readonly string[] SeriesStatsProperties = ComicBookSeriesStatistics.GetProperties().Select<string, string>((Func<string, string>) (name => "Stats" + name)).ToArray<string>();
    [NonSerialized]
    private IComicBookStatsProvider statsProvider;
    private bool propertyCheckInitialized;
    private ISet<string> usedProperties;
    private bool isOptimizedCacheUpdateDisabled;
    private readonly string[] wildCardProperty = new string[1]
    {
      "*"
    };

    public static TR TRMatcher
    {
      get
      {
        if (ComicBookMatcher.trMatcher == null)
          ComicBookMatcher.trMatcher = TR.Load("Matchers");
        return ComicBookMatcher.trMatcher;
      }
    }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool Not { get; set; }

    [XmlIgnore]
    public IComicBookStatsProvider StatsProvider
    {
      get => this.statsProvider;
      set => this.statsProvider = value;
    }

    public abstract IEnumerable<ComicBook> Match(IEnumerable<ComicBook> items);

    public abstract object Clone();

    public virtual bool IsSame(ComicBookMatcher cbm)
    {
      return cbm != null && cbm.GetType() == this.GetType() && cbm.Not == this.Not;
    }

    private void InitializeFromProperty()
    {
      if (this.propertyCheckInitialized)
        return;
      this.propertyCheckInitialized = true;
      if (!(Attribute.GetCustomAttribute((MemberInfo) this.GetType(), typeof (ComicBookMatcherHintAttribute)) is ComicBookMatcherHintAttribute customAttribute))
        return;
      this.usedProperties = customAttribute.Properties;
      this.isOptimizedCacheUpdateDisabled = customAttribute.DisableOptimizedUpdate;
    }

    public virtual IEnumerable<string> GetDependentProperties()
    {
      this.InitializeFromProperty();
      return this.usedProperties == null ? (IEnumerable<string>) this.wildCardProperty : (IEnumerable<string>) this.usedProperties;
    }

    public virtual bool UsesProperty(string propertyHint)
    {
      this.InitializeFromProperty();
      return this.usedProperties == null || this.usedProperties.Contains(propertyHint);
    }

    public virtual bool IsOptimizedCacheUpdateDisabled
    {
      get
      {
        this.InitializeFromProperty();
        return this.isOptimizedCacheUpdateDisabled;
      }
    }

    public virtual bool TimeDependant => false;

    public override string ToString() => ComicBookMatcher.ConvertToString((IComicBookMatcher) this);

    public static string ConvertToString(IComicBookMatcher matcher)
    {
      return (matcher.Not ? "Not " : string.Empty) + "[" + (matcher.GetType().Description().Escape((IEnumerable<char>) "[]", '\\') ?? matcher.GetType().Name) + "]";
    }

    public static bool IsComicProperty(string prop)
    {
      return ((IEnumerable<string>) ComicBookMatcher.ComicProperties).Contains<string>(prop);
    }

    public static bool IsSeriesStatsProperty(string prop)
    {
      return ((IEnumerable<string>) ComicBookMatcher.SeriesStatsProperties).Contains<string>(prop);
    }

    public static string ParseSeriesProperty(string prop) => prop.Substring("Stats".Length);
  }
}

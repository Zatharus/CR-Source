// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookValueMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public abstract class ComicBookValueMatcher : 
    ComicBookMatcher,
    IComicBookValueMatcher,
    IComicBookMatcher,
    IMatcher<ComicBook>,
    ICloneable,
    IComparable
  {
    private string description;
    private string descriptionNeutral;
    private string matchValue;
    private string matchValue2;
    private int matchOperator;
    private static HashSet<Type> cachedTypeList;

    protected ComicBookValueMatcher() => this.Name = string.Empty;

    [XmlAttribute]
    [DefaultValue("")]
    public string Name { get; set; }

    public override IEnumerable<ComicBook> Match(IEnumerable<ComicBook> items)
    {
      this.OnInitializeMatch();
      return ListExtensions.ParallelEnabled && items.Count<ComicBook>() > 100 ? (IEnumerable<ComicBook>) ((IEnumerable<ComicBook>) items.Lock<ComicBook>().ToArray<ComicBook>()).AsParallelSafe<ComicBook>().Where<ComicBook>(new Func<ComicBook, bool>(this.Match)) : items.Where<ComicBook>(new Func<ComicBook, bool>(this.Match));
    }

    public override object Clone()
    {
      IComicBookValueMatcher instance = (IComicBookValueMatcher) Activator.CreateInstance(this.GetType());
      instance.Not = this.Not;
      instance.MatchOperator = this.MatchOperator;
      instance.MatchValue = this.MatchValue;
      instance.MatchValue2 = this.MatchValue2;
      return (object) instance;
    }

    public override bool IsSame(ComicBookMatcher cbm)
    {
      return cbm is ComicBookValueMatcher bookValueMatcher && base.IsSame(cbm) && bookValueMatcher.MatchOperator == this.MatchOperator && bookValueMatcher.MatchValue == this.MatchValue && bookValueMatcher.MatchValue2 == this.MatchValue2;
    }

    public string Description
    {
      get
      {
        return this.description ?? (this.description = ComicBookMatcher.TRMatcher[this.GetType().Name, this.DescriptionNeutral]);
      }
    }

    public string DescriptionNeutral
    {
      get
      {
        if (this.descriptionNeutral == null)
          this.descriptionNeutral = ((DescriptionAttribute) Attribute.GetCustomAttribute((MemberInfo) this.GetType(), typeof (DescriptionAttribute))).Description ?? string.Empty;
        return this.descriptionNeutral;
      }
    }

    public virtual string MatchValue
    {
      get => this.matchValue ?? string.Empty;
      set
      {
        if (this.matchValue == value)
          return;
        this.matchValue = value;
        this.OnMatchValueChanged();
      }
    }

    [DefaultValue("")]
    public virtual string MatchValue2
    {
      get => this.matchValue2 ?? string.Empty;
      set
      {
        if (this.matchValue2 == value)
          return;
        this.matchValue2 = value;
        this.OnMatchValue2Changed();
      }
    }

    [XmlAttribute]
    [DefaultValue(0)]
    public virtual int MatchOperator
    {
      get => this.matchOperator;
      set
      {
        if (this.matchOperator == value)
          return;
        this.matchOperator = value;
        this.OnMatchOperatorChanged();
      }
    }

    public abstract string[] OperatorsListNeutral { get; }

    public abstract string[] OperatorsList { get; }

    public abstract int ArgumentCount { get; }

    public virtual bool SwapOperatorArgument => false;

    public virtual string UnitDescription => string.Empty;

    public override string ToString()
    {
      return base.ToString() + " " + ComicBookValueMatcher.ConvertParametersToString(this);
    }

    protected virtual void OnInitializeMatch()
    {
    }

    protected virtual void OnMatchValueChanged()
    {
    }

    protected virtual void OnMatchValue2Changed()
    {
    }

    protected virtual void OnMatchOperatorChanged()
    {
    }

    public virtual bool Set(ComicBookValueMatcher matcher)
    {
      Type type1 = this.GetType();
      Type type2 = matcher.GetType();
      if (type2 != type1 && type2.BaseType != type1.BaseType)
        return false;
      this.matchOperator = matcher.matchOperator;
      this.matchValue = matcher.matchValue;
      this.matchValue2 = matcher.matchValue2;
      return true;
    }

    public virtual bool Match(ComicBook item) => true;

    public int CompareTo(object obj)
    {
      return obj is IComicBookValueMatcher bookValueMatcher ? string.Compare(this.Description, bookValueMatcher.Description, true) : 1;
    }

    private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"");

    public static string ConvertParametersToString(ComicBookValueMatcher m)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(m.OperatorsListNeutral[m.MatchOperator]);
      for (int index = 0; index < m.ArgumentCount; ++index)
      {
        stringBuilder.Append(' ');
        stringBuilder.Append('"');
        stringBuilder.Append(ComicBookValueMatcher.Escape(index == 0 ? m.MatchValue : m.MatchValue2));
        stringBuilder.Append('"');
      }
      return stringBuilder.ToString();
    }

    public static IEnumerable<Type> GetAvailableMatcherTypes()
    {
      if (ComicBookValueMatcher.cachedTypeList == null)
        ComicBookValueMatcher.cachedTypeList = new HashSet<Type>(((IEnumerable<Type>) typeof (IComicBookValueMatcher).Assembly.GetExportedTypes()).Where<Type>((Func<Type, bool>) (t => t.GetInterface(typeof (IComicBookValueMatcher).Name) != (Type) null && !t.IsAbstract)));
      return (IEnumerable<Type>) ComicBookValueMatcher.cachedTypeList;
    }

    public static void RegisterMatcherType(Type type)
    {
      ComicBookValueMatcher.GetAvailableMatcherTypes();
      ComicBookValueMatcher.cachedTypeList.Add(type);
    }

    public static IEnumerable<IComicBookValueMatcher> GetAvailableMatchers()
    {
      IComicBookValueMatcher[] array = ComicBookValueMatcher.GetAvailableMatcherTypes().Select<Type, IComicBookValueMatcher>((Func<Type, IComicBookValueMatcher>) (t => Activator.CreateInstance(t) as IComicBookValueMatcher)).ToArray<IComicBookValueMatcher>();
      Array.Sort<IComicBookValueMatcher>(array);
      return (IEnumerable<IComicBookValueMatcher>) array;
    }

    public static ComicBookValueMatcher Create(
      Type matchType,
      int matchOperator,
      string matchValue1,
      string matchValue2)
    {
      if (!(Activator.CreateInstance(matchType) is ComicBookValueMatcher instance))
        throw new ArgumentException("Must be a ComicBookValueMatcher", nameof (matchType));
      instance.MatchOperator = matchOperator;
      instance.MatchValue = matchValue1;
      instance.MatchValue2 = matchValue2;
      return instance;
    }

    public static IComicBookValueMatcher Create(string description)
    {
      Type type = ComicBookValueMatcher.GetAvailableMatcherTypes().FirstOrDefault<Type>((Func<Type, bool>) (t => string.Equals(t.Description(), description, StringComparison.OrdinalIgnoreCase)));
      return type != (Type) null ? (IComicBookValueMatcher) Activator.CreateInstance(type) : (IComicBookValueMatcher) (object) null;
    }
  }
}

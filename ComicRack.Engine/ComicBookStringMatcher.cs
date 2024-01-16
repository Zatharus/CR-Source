// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookStringMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public abstract class ComicBookStringMatcher : ComicBookValueMatcher<string>
  {
    public const int OperatorEquals = 0;
    public const int OperatorContains = 1;
    public const int OperatorContainsAny = 2;
    public const int OperatorContainsAll = 3;
    public const int OperatorStartsWith = 4;
    public const int OperatorEndsWith = 5;
    public const int OperatorListContains = 6;
    public const int OperatorRegex = 7;
    private static readonly string[] opListNeutral = "equals|contains|contains any of|contains all of|starts with|ends with|list contains|regex".Split('|');
    private static readonly string[] opList = ComicBookMatcher.TRMatcher.GetStrings("StringOperators", "is|contains|contains any of|contains all of|starts with|ends with|list contains|regular expression", '|');
    private string[] parsedMatchValues = new string[0];
    private Regex rxList;
    private Regex rxMatch;
    private bool ignoreCase = true;

    protected override bool MatchBook(ComicBook comicBook, string value)
    {
      if (!this.PreCheck(comicBook))
        return false;
      StringComparison sc = this.ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
      value = value ?? string.Empty;
      switch (this.MatchOperator)
      {
        case 0:
          return string.Equals(value, this.GetMatchValue(comicBook), sc);
        case 1:
          string matchValue1 = this.GetMatchValue(comicBook);
          return string.IsNullOrEmpty(matchValue1) || value.IndexOf(matchValue1, sc) != -1;
        case 2:
          return this.parsedMatchValues.Length == 0 || ((IEnumerable<string>) this.parsedMatchValues).Any<string>((Func<string, bool>) (s => value.IndexOf(s, sc) != -1));
        case 3:
          return !string.IsNullOrEmpty(value) && ((IEnumerable<string>) this.parsedMatchValues).All<string>((Func<string, bool>) (s => value.IndexOf(s, sc) != -1));
        case 4:
          string matchValue2 = this.GetMatchValue(comicBook);
          return string.IsNullOrEmpty(matchValue2) || value.StartsWith(matchValue2, sc);
        case 5:
          string matchValue3 = this.GetMatchValue(comicBook);
          return string.IsNullOrEmpty(matchValue3) || value.EndsWith(matchValue3, sc);
        case 6:
          return this.rxList != null && this.rxList.Match(value).Success;
        case 7:
          return this.rxMatch != null && this.rxMatch.Match(value).Success;
        default:
          return false;
      }
    }

    protected override string GetMatchValue(ComicBook comicBook)
    {
      return (this.MatchColumn == 0 ? base.GetMatchValue(comicBook) : base.GetMatchValue2(comicBook)) ?? string.Empty;
    }

    protected override string GetMatchValue2(ComicBook comicBook)
    {
      return (this.MatchColumn != 0 ? base.GetMatchValue(comicBook) : base.GetMatchValue2(comicBook)) ?? string.Empty;
    }

    public override int ArgumentCount => 1;

    protected virtual bool PreCheck(ComicBook comicBook) => true;

    public virtual int MatchColumn => 0;

    public override string[] OperatorsListNeutral => ComicBookStringMatcher.opListNeutral;

    public override string[] OperatorsList => ComicBookStringMatcher.opList;

    [XmlAttribute]
    [DefaultValue(true)]
    public bool IgnoreCase
    {
      get => this.ignoreCase;
      set => this.ignoreCase = value;
    }

    protected override void OnMatchValueChanged()
    {
      base.OnMatchValueChanged();
      this.parsedMatchValues = (this.MatchColumn == 0 ? this.MatchValue : this.MatchValue2).Split(' ', StringSplitOptions.RemoveEmptyEntries);
      try
      {
        this.rxList = new Regex(string.Format("(?<=^|[,;])\\s*{0}\\s*(?=$|[,;])", (object) Regex.Escape(this.MatchValue.Trim())), RegexOptions.IgnoreCase | RegexOptions.Singleline);
      }
      catch
      {
        this.rxList = (Regex) null;
      }
      try
      {
        this.rxMatch = new Regex(this.MatchColumn == 0 ? this.MatchValue : this.MatchValue2);
      }
      catch
      {
        this.rxMatch = (Regex) null;
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookGroupMatcher
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public class ComicBookGroupMatcher : ComicBookMatcher, IComicBookGroupMatcher
  {
    private MatcherMode matcherMode;
    private readonly ComicBookMatcherCollection matchers = new ComicBookMatcherCollection();

    public override string ToString()
    {
      return (this.Not ? "Not " : string.Empty) + ComicBookGroupMatcher.ConvertParametersToQuery((IComicBookGroupMatcher) this);
    }

    public override IEnumerable<ComicBook> Match(IEnumerable<ComicBook> items)
    {
      if (this.Matchers.Count == 0)
        return items;
      MatcherSet<ComicBook> matcherSet = new MatcherSet<ComicBook>();
      foreach (ComicBookMatcher matcher in (SmartList<ComicBookMatcher>) this.Matchers)
        matcherSet.Add((IMatcher<ComicBook>) matcher, this.MatcherMode, matcher.Not);
      return matcherSet.Match(items);
    }

    public override object Clone()
    {
      ComicBookGroupMatcher bookGroupMatcher1 = new ComicBookGroupMatcher();
      bookGroupMatcher1.Not = this.Not;
      bookGroupMatcher1.MatcherMode = this.MatcherMode;
      bookGroupMatcher1.Collapsed = this.Collapsed;
      ComicBookGroupMatcher bookGroupMatcher2 = bookGroupMatcher1;
      bookGroupMatcher2.Matchers.AddRange(this.Matchers.Select<ComicBookMatcher, ComicBookMatcher>((Func<ComicBookMatcher, ComicBookMatcher>) (matcher => matcher.Clone() as ComicBookMatcher)));
      return (object) bookGroupMatcher2;
    }

    public override bool IsSame(ComicBookMatcher cbm)
    {
      return cbm is ComicBookGroupMatcher bookGroupMatcher && base.IsSame(cbm) && bookGroupMatcher.MatcherMode == this.MatcherMode && bookGroupMatcher.Collapsed == this.Collapsed && this.Matchers.SequenceEqual<ComicBookMatcher>((IEnumerable<ComicBookMatcher>) this.Matchers);
    }

    public override IEnumerable<string> GetDependentProperties()
    {
      return this.Matchers.SelectMany<ComicBookMatcher, string>((Func<ComicBookMatcher, IEnumerable<string>>) (m => m.GetDependentProperties()));
    }

    public override bool UsesProperty(string propertyHint)
    {
      return this.Matchers.Any<ComicBookMatcher>((Func<ComicBookMatcher, bool>) (m => m.UsesProperty(propertyHint)));
    }

    public override bool TimeDependant
    {
      get
      {
        return this.Matchers.Any<ComicBookMatcher>((Func<ComicBookMatcher, bool>) (m => m.TimeDependant));
      }
    }

    [XmlAttribute]
    [DefaultValue(MatcherMode.And)]
    public MatcherMode MatcherMode
    {
      get => this.matcherMode;
      set => this.matcherMode = value;
    }

    public ComicBookMatcherCollection Matchers => this.matchers;

    [XmlAttribute]
    [DefaultValue(false)]
    public bool Collapsed { get; set; }

    public ComicBookMatcher Optimized()
    {
      if (this.Matchers.Count == 0)
        return (ComicBookMatcher) null;
      return this.Matchers.Count == 1 ? this.Matchers[0] : (ComicBookMatcher) this;
    }

    public static void ConvertQueryToParamerters(IComicBookGroupMatcher gm, Tokenizer tokens)
    {
      tokens.Expect("MATCH");
      gm.MatcherMode = MatcherMode.And;
      if (tokens.IsOptional("ANY"))
      {
        gm.MatcherMode = MatcherMode.Or;
        tokens.Skip();
      }
      else if (tokens.IsOptional("ALL"))
        tokens.Skip();
      bool flag = tokens.IsOptional("{");
      if (flag)
        tokens.Skip();
      gm.Matchers.Clear();
      while (true)
      {
        if (flag)
        {
          if (tokens.Is("}"))
            break;
        }
        gm.Matchers.Add(ComicBookGroupMatcher.CreateMatcherFromQuery(tokens));
        if (flag)
        {
          if (!tokens.Is("}"))
            tokens.Expect(",");
          else
            goto label_12;
        }
        else
          goto label_14;
      }
      tokens.Skip();
      return;
label_14:
      return;
label_12:
      tokens.Skip();
    }

    public static ComicBookMatcher CreateMatcherFromQuery(Tokenizer tokens)
    {
      bool flag = false;
      if (tokens.IsOptional("NOT"))
      {
        flag = true;
        tokens.Skip();
      }
      if (tokens.Is("MATCH"))
      {
        ComicBookGroupMatcher bookGroupMatcher = new ComicBookGroupMatcher();
        bookGroupMatcher.Not = flag;
        ComicBookGroupMatcher gm = bookGroupMatcher;
        ComicBookGroupMatcher.ConvertQueryToParamerters((IComicBookGroupMatcher) gm, tokens);
        return (ComicBookMatcher) gm;
      }
      Tokenizer.Token token = tokens.Take("[", "]");
      token.Text = token.Text.Unescape((IEnumerable<char>) "[]", '\\');
      IComicBookValueMatcher matcherFromQuery = ComicBookValueMatcher.Create(token.Text);
      if (matcherFromQuery == null)
        token.ThrowParserException("Invalid name {0} encountered");
      matcherFromQuery.Not = flag;
      Tokenizer.Token op = tokens.Expect(matcherFromQuery.OperatorsListNeutral);
      int index1 = ((IEnumerable<string>) matcherFromQuery.OperatorsListNeutral).FindIndex<string>((Predicate<string>) (o => string.Equals(o, op.Text, StringComparison.OrdinalIgnoreCase)));
      if (index1 == -1)
        token.ThrowParserException("Invalid operator {0} encountered");
      matcherFromQuery.MatchOperator = index1;
      for (int index2 = 0; index2 < matcherFromQuery.ArgumentCount; ++index2)
      {
        if (index2 == 0)
          matcherFromQuery.MatchValue = tokens.TakeString().Text;
        else
          matcherFromQuery.MatchValue2 = tokens.TakeString().Text;
      }
      return matcherFromQuery as ComicBookMatcher;
    }

    public static string ConvertParametersToQuery(IComicBookGroupMatcher gm, bool format = true)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int count = gm.Matchers.Count;
      stringBuilder.Append("Match");
      if (count > 0)
      {
        stringBuilder.Append(" ");
        if (count > 1)
        {
          switch (gm.MatcherMode)
          {
            case MatcherMode.And:
              stringBuilder.Append("All");
              break;
            case MatcherMode.Or:
              stringBuilder.Append("Any");
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
          if (format)
            stringBuilder.Append(Environment.NewLine);
          else
            stringBuilder.Append(' ');
          stringBuilder.Append("{");
          if (format)
            stringBuilder.Append(Environment.NewLine);
        }
        for (int index = 0; index < count; ++index)
        {
          string s = gm.Matchers[index].ToString();
          if (format && count > 1)
            s = s.Intent(4);
          stringBuilder.Append(s);
          if (index != count - 1 && count > 1)
          {
            stringBuilder.Append(",");
            stringBuilder.Append(format ? Environment.NewLine : " ");
          }
        }
        if (count > 1)
        {
          if (format)
            stringBuilder.Append(Environment.NewLine);
          stringBuilder.Append("}");
        }
      }
      return stringBuilder.ToString();
    }
  }
}

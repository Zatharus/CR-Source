// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.MatcherSet`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class MatcherSet<T> : IMatcher<T>
  {
    private readonly List<MatcherSetItem<T>> matchers = new List<MatcherSetItem<T>>();

    public List<MatcherSetItem<T>> Matchers => this.matchers;

    public void Add(IMatcher<T> matcher, MatcherMode mode, bool not)
    {
      if (matcher == null)
        return;
      this.matchers.Add(new MatcherSetItem<T>(mode, not, matcher));
    }

    public void And(IMatcher<T> matcher, bool not = false)
    {
      this.Add(matcher, MatcherMode.And, not);
    }

    public void AndNot(IMatcher<T> matcher) => this.And(matcher, true);

    public void Or(IMatcher<T> matcher, bool not = false) => this.Add(matcher, MatcherMode.Or, not);

    public void OrNot(IMatcher<T> matcher) => this.Or(matcher, true);

    public void And(IEnumerable<IMatcher<T>> em, bool not)
    {
      foreach (IMatcher<T> matcher in em)
        this.And(matcher, not);
    }

    public void And(IEnumerable<IMatcher<T>> em) => this.And(em, false);

    public void AndNot(IEnumerable<IMatcher<T>> em) => this.And(em, true);

    public void Or(IEnumerable<IMatcher<T>> em, bool not = false)
    {
      foreach (IMatcher<T> matcher in em)
        this.Or(matcher, not);
    }

    public void OrNot(IEnumerable<IMatcher<T>> em) => this.Or(em, true);

    public IEnumerable<T> Match(IEnumerable<T> items)
    {
      return MatcherSet<T>.Match((IEnumerable<MatcherSetItem<T>>) this.Matchers, items);
    }

    public static IEnumerable<T> Match(
      IEnumerable<MatcherSetItem<T>> matchers,
      IEnumerable<T> items)
    {
      IEnumerable<T> objs1 = (IEnumerable<T>) null;
      foreach (MatcherSetItem<T> matcher in matchers)
      {
        switch (matcher.Mode)
        {
          case MatcherMode.And:
            IEnumerable<T> objs2 = objs1 ?? items;
            IEnumerable<T> array1 = (IEnumerable<T>) matcher.Matcher.Match(objs2).ToArray<T>();
            objs1 = matcher.Not ? (IEnumerable<T>) objs2.Except<T>(array1).ToArray<T>() : array1;
            continue;
          case MatcherMode.Or:
            IEnumerable<T> objs3 = objs1 == null ? items : items.Except<T>(objs1);
            IEnumerable<T> array2 = (IEnumerable<T>) matcher.Matcher.Match(objs3).ToArray<T>();
            if (matcher.Not)
              array2 = (IEnumerable<T>) objs3.Except<T>(array2).ToArray<T>();
            objs1 = objs1 == null ? array2 : objs1.Concat<T>(array2);
            continue;
          default:
            continue;
        }
      }
      return objs1;
    }
  }
}

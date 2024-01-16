// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookValueMatcher`1
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public abstract class ComicBookValueMatcher<T> : ComicBookValueMatcher
  {
    private string comicProperty;
    private string comicProperty2;
    private string seriesStatsProperty;
    private string seriesStatsProperty2;
    private T matchValue;
    private T matchValue2;
    protected static readonly Regex FieldExpression = new Regex("{(?<name>[a-z]+)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override IEnumerable<string> GetDependentProperties()
    {
      foreach (string dependentProperty in base.GetDependentProperties())
        yield return dependentProperty;
      if (this.comicProperty != null)
        yield return this.comicProperty;
      if (this.comicProperty2 != null)
        yield return this.comicProperty2;
    }

    public override bool UsesProperty(string propertyHint)
    {
      return this.comicProperty != null && this.comicProperty == propertyHint || this.comicProperty2 != null && this.comicProperty2 == propertyHint || base.UsesProperty(propertyHint);
    }

    public override bool IsOptimizedCacheUpdateDisabled
    {
      get
      {
        return base.IsOptimizedCacheUpdateDisabled || this.seriesStatsProperty != null || this.seriesStatsProperty2 != null;
      }
    }

    public override bool Match(ComicBook item)
    {
      return this.MatchBook(item, this.PreparseValue(this.GetValue(item)));
    }

    protected override void OnMatchValueChanged()
    {
      base.OnMatchValueChanged();
      System.Text.RegularExpressions.Match match = ComicBookValueMatcher<T>.FieldExpression.Match(this.MatchValue ?? string.Empty);
      this.comicProperty = this.seriesStatsProperty = (string) null;
      if (!match.Success)
      {
        this.matchValue = this.ConvertMatchValue(this.PreparseMatchValue(this.MatchValue));
      }
      else
      {
        string prop = match.Groups[1].Value;
        if (ComicBookMatcher.IsComicProperty(prop))
        {
          this.comicProperty = prop;
        }
        else
        {
          if (!ComicBookMatcher.IsSeriesStatsProperty(prop))
            return;
          this.seriesStatsProperty = ComicBookMatcher.ParseSeriesProperty(prop);
        }
      }
    }

    protected override void OnMatchValue2Changed()
    {
      base.OnMatchValue2Changed();
      System.Text.RegularExpressions.Match match = ComicBookValueMatcher<T>.FieldExpression.Match(this.MatchValue2 ?? string.Empty);
      this.comicProperty2 = this.seriesStatsProperty2 = (string) null;
      if (!match.Success)
      {
        this.matchValue2 = this.ConvertMatchValue(this.PreparseMatchValue(this.MatchValue2));
      }
      else
      {
        string prop = match.Groups[1].Value;
        if (ComicBookMatcher.IsComicProperty(prop))
        {
          this.comicProperty = prop;
        }
        else
        {
          if (!ComicBookMatcher.IsSeriesStatsProperty(prop))
            return;
          this.seriesStatsProperty = ComicBookMatcher.ParseSeriesProperty(prop);
        }
      }
    }

    protected virtual string PreparseMatchValue(string value) => value;

    protected virtual T PreparseValue(T value) => value;

    protected virtual T GetMatchValue(ComicBook comicBook)
    {
      if (this.comicProperty != null)
        return comicBook.GetPropertyValue<T>(this.comicProperty);
      return this.StatsProvider != null && this.seriesStatsProperty != null ? this.StatsProvider.GetSeriesStats(comicBook).GetTypedValue<T>(this.seriesStatsProperty) : this.matchValue;
    }

    protected virtual T GetMatchValue2(ComicBook comicBook)
    {
      if (this.comicProperty2 != null)
        return comicBook.GetPropertyValue<T>(this.comicProperty2);
      return this.StatsProvider != null && this.seriesStatsProperty2 != null ? this.StatsProvider.GetSeriesStats(comicBook).GetTypedValue<T>(this.seriesStatsProperty2) : this.matchValue2;
    }

    protected virtual T ConvertMatchValue(string input)
    {
      try
      {
        return (T) Convert.ChangeType((object) input, typeof (T));
      }
      catch
      {
        return this.GetInvalidValue();
      }
    }

    protected virtual T GetInvalidValue() => default (T);

    protected abstract bool MatchBook(ComicBook book, T value);

    protected abstract T GetValue(ComicBook comicBook);
  }
}

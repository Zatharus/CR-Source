// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.ComicBookExpressionMatcher
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Projects.ComicRack.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins
{
  [System.ComponentModel.Description("Expression")]
  [ComicBookMatcherHint(true)]
  [Serializable]
  public class ComicBookExpressionMatcher : ComicBookValueMatcher<bool>
  {
    private const string BookVariableName = "__book";
    private const string BookStatsVariableName = "__bookStats";
    private static readonly string[] opListNeutral = "is true|is false".Split('|');
    private static readonly string[] opList = ComicBookMatcher.TRMatcher.GetStrings("TrueFalseOperators", "is True|is False", '|');
    private readonly HashSet<string> properties = new HashSet<string>();
    private bool usesStatistics;
    private string parsedMatchValue;
    [NonSerialized]
    private Func<ComicBook, IComicBookStatsProvider, bool> expression;
    [NonSerialized]
    private bool error;

    public override bool IsOptimizedCacheUpdateDisabled
    {
      get => base.IsOptimizedCacheUpdateDisabled || this.usesStatistics;
    }

    public override string[] OperatorsListNeutral => ComicBookExpressionMatcher.opListNeutral;

    public override string[] OperatorsList => ComicBookExpressionMatcher.opList;

    public override int ArgumentCount => 1;

    protected override bool MatchBook(ComicBook book, bool value)
    {
      switch (this.MatchOperator)
      {
        case 1:
          return !value;
        default:
          return value;
      }
    }

    protected override void OnMatchValueChanged()
    {
      base.OnMatchValueChanged();
      this.properties.Clear();
      this.parsedMatchValue = ComicBookValueMatcher<bool>.FieldExpression.Replace(this.MatchValue, (MatchEvaluator) (e =>
      {
        string newName = e.Groups[1].Value;
        if (ComicBookMatcher.IsComicProperty(newName))
        {
          if (ComicBook.MapPropertyName(newName, out newName, ComicValueType.Shadow))
          {
            this.properties.Add(newName);
            this.properties.Add("EnableProposed");
            this.properties.Add("FilePath");
          }
          return "__book." + newName;
        }
        if (!ComicBookMatcher.IsSeriesStatsProperty(newName))
          return string.Empty;
        this.usesStatistics = true;
        return "__bookStats.GetSeriesStats(__book)." + ComicBookMatcher.ParseSeriesProperty(newName);
      }));
      this.expression = (Func<ComicBook, IComicBookStatsProvider, bool>) null;
    }

    public override IEnumerable<string> GetDependentProperties()
    {
      return base.GetDependentProperties().Concat<string>((IEnumerable<string>) this.properties);
    }

    public override bool UsesProperty(string propertyHint)
    {
      return this.usesStatistics || this.properties.Contains(propertyHint) || base.UsesProperty(propertyHint);
    }

    protected override void OnInitializeMatch()
    {
      base.OnInitializeMatch();
      this.error = false;
      try
      {
        if (this.expression != null)
          return;
        this.expression = PythonCommand.CompileExpression<Func<ComicBook, IComicBookStatsProvider, bool>>(this.parsedMatchValue, "__book", "__bookStats");
      }
      catch (Exception ex)
      {
        this.error = true;
      }
    }

    protected override bool GetValue(ComicBook comicBook)
    {
      if (this.error)
        return false;
      try
      {
        return this.expression(comicBook, this.StatsProvider);
      }
      catch
      {
        this.error = false;
        return false;
      }
    }
  }
}

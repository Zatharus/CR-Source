// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicNameInfo
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public class ComicNameInfo
  {
    private string series = string.Empty;
    private string title = string.Empty;
    private string number = string.Empty;
    private string format = string.Empty;
    private int volume = -1;
    private int count = -1;
    private int year = -1;
    private int coverCount = 1;

    public ComicNameInfo(
      string series,
      string title,
      string number,
      int count,
      int volume,
      int year,
      string format)
    {
      this.Series = series;
      this.Number = number;
      this.Year = year;
      this.Count = count;
      this.Volume = volume;
      this.Format = format;
      this.Title = title;
    }

    public ComicNameInfo()
      : this(string.Empty, string.Empty, string.Empty, -1, -1, -1, string.Empty)
    {
    }

    public string Series
    {
      get => this.series;
      set => this.series = value;
    }

    public string Title
    {
      get => this.title;
      set => this.title = value;
    }

    public string Number
    {
      get => this.number;
      set => this.number = value;
    }

    public string Format
    {
      get => this.format;
      set => this.format = value;
    }

    public int Volume
    {
      get => this.volume;
      set => this.volume = value;
    }

    public int Count
    {
      get => this.count;
      set => this.count = value;
    }

    public int Year
    {
      get => this.year;
      set => this.year = value;
    }

    public int CoverCount
    {
      get => this.coverCount;
      set => this.coverCount = value;
    }

    public override int GetHashCode() => this.Series.GetHashCode() ^ this.Number.GetHashCode();

    public override bool Equals(object obj)
    {
      return obj is ComicNameInfo comicNameInfo && comicNameInfo.Series == this.Series && comicNameInfo.Number == this.Number && comicNameInfo.Count == this.Count && comicNameInfo.Volume == this.Volume && comicNameInfo.Title == this.Title && comicNameInfo.Format == this.Format && comicNameInfo.Year == this.Year;
    }

    public static ComicNameInfo FromFilePath(string path, bool legacy)
    {
      return !legacy ? ComicNameInfo.NewParser.FromFilePath(path) : ComicNameInfo.LegacyParser.FromFilePath(path);
    }

    public static ComicNameInfo FromFilePath(string path)
    {
      return ComicNameInfo.FromFilePath(path, EngineConfiguration.Default.LegacyFilenameParser);
    }

    private static class NewParser
    {
      private const RegexOptions RxOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline;
      private static readonly Regex rxDotReplace = new Regex("((?<!\\d)\\.|\\.(?!\\d)|_)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxRemove = new Regex("\\b(ctc|c2c|\\d+p|\\d{1,2}\\-\\d{1,2}|\\d{1,2}\\-(?=\\d{4}))\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxBrackets = new Regex("\\(.*?\\)|\\[.*?\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static Regex rxCount;
      private static readonly Regex rxNumber = new Regex("(?<!part\\s+)(\\b|#|(c\\w*\\s*))\\d[\\d\\.]*\\b(?!\\s*(pa|cov))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.RightToLeft);
      private static readonly Regex rxVolume = new Regex("\\b(v|vol\\.?|volume)\\s*\\d+\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxYear = new Regex("\\b(?<!#)(19|2[0-3])\\d\\d\\b(?!\\spa)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.RightToLeft);
      private static readonly Regex rxYearWithMonth = new Regex("(19|2[0-3])\\d\\d[-/\\\\\\s]\\d{1,2}\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.RightToLeft);
      private static readonly Regex rxFormat = new Regex("\\b(annual|director's cut|preview|b(lack)?\\s*&\\s*w(hite)?|king\\s*size|giant\\s*size)|sketch\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxCoverCount = new Regex("(?<covers>\\d+)\\s+cover", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxnum = new Regex("\\d+\\.?\\d*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.RightToLeft);

      public static ComicNameInfo FromFilePath(string path)
      {
        ComicNameInfo comicNameInfo = new ComicNameInfo();
        try
        {
          string input = Path.GetFileNameWithoutExtension(path);
          if (!input.Contains(" ") || input.Contains("_"))
            input = ComicNameInfo.NewParser.rxDotReplace.Replace(input, " ");
          string str1 = ComicNameInfo.NewParser.rxRemove.Replace(input, string.Empty);
          if (ComicNameInfo.NewParser.rxCount == null)
            ComicNameInfo.NewParser.rxCount = new Regex(string.Format("\\b({0})\\s*\\d+\\b", (object) ((IEnumerable<string>) (EngineConfiguration.Default.OfValues ?? "of,von,de").Split(',')).TrimStrings().ToListString("|")), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
          Match match1 = ComicNameInfo.NewParser.rxCount.Match(str1);
          if (match1.Success)
          {
            str1 = str1.Remove(match1.Index, match1.Length);
            if (!int.TryParse(ComicNameInfo.NewParser.GetNumber(match1.Value), out comicNameInfo.count))
              comicNameInfo.count = -1;
          }
          Match match2 = ComicNameInfo.NewParser.rxVolume.Match(str1);
          if (match2.Success)
          {
            str1 = str1.Remove(match2.Index, match2.Length);
            if (!int.TryParse(ComicNameInfo.NewParser.GetNumber(match2.Value), out comicNameInfo.volume))
              comicNameInfo.volume = -1;
          }
          Match match3 = ComicNameInfo.NewParser.rxYearWithMonth.Match(str1);
          if (!match3.Success)
            match3 = ComicNameInfo.NewParser.rxYear.Match(str1);
          if (match3.Success)
          {
            str1 = str1.Remove(match3.Index, match3.Length);
            if (!int.TryParse(ComicNameInfo.NewParser.GetNumber(match3.Value.Substring(0, 4)), out comicNameInfo.year))
              comicNameInfo.year = -1;
          }
          Match match4 = ComicNameInfo.NewParser.rxFormat.Match(str1);
          if (match4.Success)
          {
            str1 = str1.Remove(match4.Index, match4.Length);
            comicNameInfo.Format = match4.Value;
          }
          Match match5 = ComicNameInfo.NewParser.rxNumber.Match(str1);
          if (match5.Success)
            str1 = str1.Remove(match5.Index, match5.Length);
          comicNameInfo.number = ComicNameInfo.NewParser.GetNumber(match5.Value);
          float result;
          if (float.TryParse(comicNameInfo.number, NumberStyles.AllowDecimalPoint, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            comicNameInfo.number = result.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          Match match6 = ComicNameInfo.NewParser.rxCoverCount.Match(str1);
          if (match6.Success)
            comicNameInfo.CoverCount = int.Parse(match6.Groups["covers"].Value);
          else if (str1.Contains("two cove", StringComparison.OrdinalIgnoreCase))
            comicNameInfo.CoverCount = 2;
          else if (str1.Contains("three cove", StringComparison.OrdinalIgnoreCase))
            comicNameInfo.CoverCount = 3;
          else if (str1.Contains("four cove", StringComparison.OrdinalIgnoreCase))
            comicNameInfo.CoverCount = 4;
          string str2 = str1.CutOff('(', '[', '#', ',').Trim();
          comicNameInfo.series = str2.Trim(' ', '-', '.');
          if (!string.IsNullOrEmpty(comicNameInfo.series))
          {
            if (!comicNameInfo.Series.IsNumber())
              goto label_34;
          }
          comicNameInfo.series = ComicNameInfo.NewParser.rxBrackets.Replace(Path.GetFileName(Path.GetDirectoryName(path)), string.Empty);
        }
        catch (Exception ex)
        {
        }
label_34:
        return comicNameInfo;
      }

      private static string GetNumber(string text)
      {
        return !string.IsNullOrEmpty(text) ? ComicNameInfo.NewParser.rxnum.Match(text).Value : string.Empty;
      }
    }

    private static class LegacyParser
    {
      private const RegexOptions RxOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline;
      private static readonly Regex rxSeries = new Regex("^(\\d+)?(([&\\w\\s'-])(?!v\\d|(?<=[ #])(\\d(?!\\d*\\s[#\\d]))+(?=(\\W|$))(?!\\))))*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxVolume = new Regex("(?<=\\bv)\\d(?=\\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxNumber = new Regex("(?<=[ #]|c|ch)(\\d(?!\\d*\\s[#\\d]))+(?=(\\W|$))(?!\\))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxCount = new Regex("(?<=[\\(\\[\\s]of\\s)\\d+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
      private static readonly Regex rxYear = new Regex("(?<=[\\(\\[])\\d{4}\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

      public static ComicNameInfo FromFilePath(string path)
      {
        ComicNameInfo comicNameInfo = new ComicNameInfo();
        try
        {
          string input = Path.GetFileNameWithoutExtension(path).Replace('.', ' ').Replace('_', ' ');
          comicNameInfo.series = ComicNameInfo.LegacyParser.rxSeries.Match(input).Value;
          if (string.IsNullOrEmpty(comicNameInfo.series) || char.IsDigit(comicNameInfo.series[0]) && string.IsNullOrEmpty(ComicNameInfo.LegacyParser.rxNumber.Match(input).Value))
          {
            input = Path.GetFileName(Path.GetDirectoryName(path)) + " " + input;
            comicNameInfo.series = ComicNameInfo.LegacyParser.rxSeries.Match(input).Value;
          }
          if (string.IsNullOrEmpty(comicNameInfo.series))
            comicNameInfo.series = input;
          comicNameInfo.Series = comicNameInfo.Series.Trim();
          string number = ComicNameInfo.LegacyParser.rxNumber.Match(input).Value;
          float f;
          comicNameInfo.number = number.TryParse(out f, true) ? f.ToString() : number;
          if (!int.TryParse(ComicNameInfo.LegacyParser.rxCount.Match(input).Value, out comicNameInfo.count))
            comicNameInfo.count = -1;
          if (!int.TryParse(ComicNameInfo.LegacyParser.rxVolume.Match(input).Value, out comicNameInfo.volume))
            comicNameInfo.volume = -1;
          if (!int.TryParse(ComicNameInfo.LegacyParser.rxYear.Match(input).Value, out comicNameInfo.year))
            comicNameInfo.year = -1;
        }
        catch (Exception ex)
        {
        }
        return comicNameInfo;
      }
    }
  }
}

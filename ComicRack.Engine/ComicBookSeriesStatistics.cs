// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookSeriesStatistics
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookSeriesStatistics
  {
    private static TR tr;
    private static readonly string noneText = ComicBookSeriesStatistics.TR["None", "None"];
    private readonly Lazy<int> count;
    private readonly Lazy<int> readCount;
    private readonly Lazy<int> pageCount;
    private readonly Lazy<int> pageReadCount;
    private readonly Lazy<int> readPercentage;
    private readonly Lazy<int> ratingCount;
    private readonly Lazy<int> communityRatingCount;
    private readonly Lazy<float> averageRating;
    private readonly Lazy<float> averageCommunityRating;
    private readonly Lazy<float> firstNumber;
    private readonly Lazy<float> lastNumber;
    private readonly Lazy<int> minYear;
    private readonly Lazy<int> maxYear;
    private readonly Lazy<int> runningTimeYears;
    private readonly Lazy<IEnumerable<RangeF>> gaps;
    private readonly Lazy<int> maxGapSize;
    private readonly Lazy<int> gapCount;
    private readonly Lazy<YesNo> complete;
    private readonly Lazy<DateTime> lastOpenedTime;
    private readonly Lazy<DateTime> lastAddedTime;
    private readonly Lazy<DateTime> lastPublishedTime;
    private readonly Lazy<DateTime> lastReleasedTime;
    private readonly Lazy<int> maxCount;
    private readonly Lazy<int> minCount;
    private static readonly HashSet<string> statisticProperties = new HashSet<string>((IEnumerable<string>) new string[15]
    {
      "Series",
      "Volume",
      "FilePath",
      "Number",
      nameof (Count),
      "Year",
      "Month",
      "AddedTime",
      "OpenedTime",
      "ReleasedTime",
      "SeriesComplete",
      nameof (PageCount),
      "LastPageRead",
      "Rating",
      "CommunityRating"
    });

    public static TR TR
    {
      get
      {
        if (ComicBookSeriesStatistics.tr == null)
          ComicBookSeriesStatistics.tr = TR.Load("ComicBook");
        return ComicBookSeriesStatistics.tr;
      }
    }

    public ComicBookSeriesStatistics(IEnumerable<ComicBook> books)
    {
      this.Books = books;
      this.count = new Lazy<int>((Func<int>) (() => this.Books.Count<ComicBook>()));
      this.minCount = new Lazy<int>((Func<int>) (() => this.Books.Min<ComicBook>((Func<ComicBook, int>) (cb => cb.ShadowCount))));
      this.maxCount = new Lazy<int>((Func<int>) (() => this.Books.Max<ComicBook>((Func<ComicBook, int>) (cb => cb.ShadowCount))));
      this.readCount = new Lazy<int>((Func<int>) (() => this.Books.Count<ComicBook>((Func<ComicBook, bool>) (cb => cb.HasBeenRead))));
      this.pageCount = new Lazy<int>((Func<int>) (() => this.Books.Sum<ComicBook>((Func<ComicBook, int>) (cb => cb.PageCount))));
      this.pageReadCount = new Lazy<int>((Func<int>) (() => this.Books.Sum<ComicBook>((Func<ComicBook, int>) (cb => cb.LastPageRead))));
      this.readPercentage = new Lazy<int>((Func<int>) (() => this.Count != 0 ? this.ReadCount * 100 / this.Count : 0));
      this.firstNumber = new Lazy<float>((Func<float>) (() => this.Books.Min<ComicBook>((Func<ComicBook, float>) (cb => ComicBookSeriesStatistics.GetSafeNumber(cb)))));
      this.lastNumber = new Lazy<float>((Func<float>) (() => this.Books.Max<ComicBook>((Func<ComicBook, float>) (cb => ComicBookSeriesStatistics.GetSafeNumber(cb)))));
      this.ratingCount = new Lazy<int>((Func<int>) (() => this.Books.Count<ComicBook>((Func<ComicBook, bool>) (cb => (double) cb.Rating > 0.0))));
      this.communityRatingCount = new Lazy<int>((Func<int>) (() => this.Books.Count<ComicBook>((Func<ComicBook, bool>) (cb => (double) cb.CommunityRating > 0.0))));
      this.averageRating = new Lazy<float>((Func<float>) (() => this.RatingCount != 0 ? this.Books.Where<ComicBook>((Func<ComicBook, bool>) (cb => (double) cb.Rating > 0.0)).Average<ComicBook>((Func<ComicBook, float>) (cb => cb.Rating)) : 0.0f));
      this.averageCommunityRating = new Lazy<float>((Func<float>) (() => this.CommunityRatingCount != 0 ? this.Books.Where<ComicBook>((Func<ComicBook, bool>) (cb => (double) cb.CommunityRating > 0.0)).Average<ComicBook>((Func<ComicBook, float>) (cb => cb.CommunityRating)) : 0.0f));
      this.minYear = new Lazy<int>((Func<int>) (() => this.Books.Min<ComicBook>((Func<ComicBook, int>) (cb => cb.ShadowYear))));
      this.maxYear = new Lazy<int>((Func<int>) (() => this.Books.Max<ComicBook>((Func<ComicBook, int>) (cb => cb.ShadowYear))));
      this.runningTimeYears = new Lazy<int>((Func<int>) (() => this.LastYear >= 0 ? this.LastYear - this.FirstYear : 0));
      this.gaps = new Lazy<IEnumerable<RangeF>>((Func<IEnumerable<RangeF>>) (() => (IEnumerable<RangeF>) ComicBookSeriesStatistics.GetGaps(this.Books).ToArray<RangeF>()));
      this.gapCount = new Lazy<int>((Func<int>) (() => this.Gaps.Count<RangeF>()));
      this.maxGapSize = new Lazy<int>((Func<int>) (() => this.GapCount != 0 ? this.Gaps.Max<RangeF>((Func<RangeF, int>) (g => (int) g.Length)) : 0));
      this.complete = new Lazy<YesNo>((Func<YesNo>) (() => ComicBookSeriesStatistics.SumComplete(this.Books)));
      this.lastAddedTime = new Lazy<DateTime>((Func<DateTime>) (() => this.Books.Max<ComicBook, DateTime>((Func<ComicBook, DateTime>) (cb => cb.AddedTime))));
      this.lastOpenedTime = new Lazy<DateTime>((Func<DateTime>) (() => this.Books.Max<ComicBook, DateTime>((Func<ComicBook, DateTime>) (cb => cb.OpenedTime))));
      this.lastPublishedTime = new Lazy<DateTime>((Func<DateTime>) (() => this.Books.Max<ComicBook, DateTime>((Func<ComicBook, DateTime>) (cb => cb.Published))));
      this.lastReleasedTime = new Lazy<DateTime>((Func<DateTime>) (() => this.Books.Max<ComicBook, DateTime>((Func<ComicBook, DateTime>) (cb => cb.ReleasedTime))));
    }

    public IEnumerable<ComicBook> Books { get; private set; }

    public int Count => this.count.Value;

    public int ReadCount => this.readCount.Value;

    public int PageCount => this.pageCount.Value;

    public int PageReadCount => this.pageReadCount.Value;

    public int ReadPercentage => this.readPercentage.Value;

    public int RatingCount => this.ratingCount.Value;

    public int CommunityRatingCount => this.communityRatingCount.Value;

    public float AverageRating => this.averageRating.Value;

    public float AverageCommunityRating => this.averageCommunityRating.Value;

    public float FirstNumber => this.firstNumber.Value;

    public float LastNumber => this.lastNumber.Value;

    public int FirstYear => this.minYear.Value;

    public int LastYear => this.maxYear.Value;

    public int RunningTimeYears => this.runningTimeYears.Value;

    public IEnumerable<RangeF> Gaps => this.gaps.Value;

    public int MaxGapSize => this.maxGapSize.Value;

    public int GapCount => this.gapCount.Value;

    public YesNo AllComplete => this.complete.Value;

    public DateTime LastOpenedTime => this.lastOpenedTime.Value;

    public DateTime LastAddedTime => this.lastAddedTime.Value;

    public DateTime LastPublishedTime => this.lastPublishedTime.Value;

    public DateTime LastReleasedTime => this.lastReleasedTime.Value;

    public int MaxCount => this.maxCount.Value;

    public int MinCount => this.minCount.Value;

    public string CountAsText => this.Count.ToString();

    public string PageCountAsText => ComicBook.FormatPages(this.PageCount);

    public string PageReadCountAsText => ComicBook.FormatPages(this.PageReadCount);

    public string ReadPercentageAsText => string.Format("{0}%", (object) this.ReadPercentage);

    public string MinYearAsText => ComicBook.FormatYear(this.FirstYear);

    public string MaxYearAsText => ComicBook.FormatYear(this.LastYear);

    public string RunningTimeYearsAsText => ComicBook.FormatYear(this.RunningTimeYears);

    public string GapCountAsText
    {
      get => this.GapCount > 0 ? this.GapCount.ToString() : ComicBookSeriesStatistics.noneText;
    }

    public string MinNumberAsText
    {
      get => (double) this.FirstNumber >= 0.0 ? this.FirstNumber.ToString() : string.Empty;
    }

    public string MaxNumberAsText
    {
      get => (double) this.LastNumber >= 0.0 ? this.LastNumber.ToString() : string.Empty;
    }

    public bool IsGapStart(ComicBook book)
    {
      float n = ComicBookSeriesStatistics.GetSafeNumber(book);
      return this.Gaps.Any<RangeF>((Func<RangeF, bool>) (g => (double) g.Start == (double) n));
    }

    public bool IsGapEnd(ComicBook book)
    {
      float n = ComicBookSeriesStatistics.GetSafeNumber(book);
      return this.Gaps.Any<RangeF>((Func<RangeF, bool>) (g => (double) g.End == (double) n));
    }

    public T GetTypedValue<T>(string propName)
    {
      try
      {
        return PropertyCaller.CreateGetMethod<ComicBookSeriesStatistics, T>(propName)(this);
      }
      catch (Exception ex)
      {
        return default (T);
      }
    }

    private static YesNo SumComplete(IEnumerable<ComicBook> books)
    {
      bool flag = true;
      YesNo yesNo = YesNo.Unknown;
      foreach (ComicBook book in books)
      {
        if (flag)
        {
          flag = false;
          yesNo = book.SeriesComplete;
        }
        else if (yesNo != book.SeriesComplete)
          return YesNo.Unknown;
      }
      return yesNo;
    }

    public static IEnumerable<string> GetProperties()
    {
      return ((IEnumerable<PropertyInfo>) typeof (ComicBookSeriesStatistics).GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (pi => pi.Browsable())).Select<PropertyInfo, string>((Func<PropertyInfo, string>) (pi => pi.Name));
    }

    public static Dictionary<ComicBookSeriesStatistics.Key, ComicBookSeriesStatistics> Create(
      IEnumerable<ComicBook> books)
    {
      return books.Lock<ComicBook>().GroupBy<ComicBook, ComicBookSeriesStatistics.Key>((Func<ComicBook, ComicBookSeriesStatistics.Key>) (cb => new ComicBookSeriesStatistics.Key(cb))).ToDictionary<IGrouping<ComicBookSeriesStatistics.Key, ComicBook>, ComicBookSeriesStatistics.Key, ComicBookSeriesStatistics>((Func<IGrouping<ComicBookSeriesStatistics.Key, ComicBook>, ComicBookSeriesStatistics.Key>) (gr => gr.Key), (Func<IGrouping<ComicBookSeriesStatistics.Key, ComicBook>, ComicBookSeriesStatistics>) (gr => new ComicBookSeriesStatistics((IEnumerable<ComicBook>) gr)));
    }

    public static IEnumerable<RangeF> GetGaps(IEnumerable<ComicBook> books)
    {
      IOrderedEnumerable<float> source = books.Select<ComicBook, float>(new Func<ComicBook, float>(ComicBookSeriesStatistics.GetSafeNumber)).Where<float>((Func<float, bool>) (n => (double) n > 0.0)).OrderBy<float, float>((Func<float, float>) (n => n));
      if (source.Any<float>())
      {
        float start = source.First<float>();
        foreach (float n in source.Skip<float>(1))
        {
          float length = n - start;
          if ((double) length > 1.0)
            yield return new RangeF(start, length);
          start = n;
        }
      }
    }

    public static ISet<string> StatisticProperties
    {
      get => (ISet<string>) ComicBookSeriesStatistics.statisticProperties;
    }

    private static float GetSafeNumber(ComicBook cb)
    {
      return !cb.CompareNumber.IsNumber ? -1f : cb.CompareNumber.Number;
    }

    public class Key : IEquatable<ComicBookSeriesStatistics.Key>
    {
      public Key(ComicBook book)
      {
        this.Series = book.ShadowSeries ?? string.Empty;
        this.Volume = book.ShadowVolume;
      }

      public string Series { get; private set; }

      public int Volume { get; private set; }

      public override int GetHashCode() => this.Series.GetHashCode() ^ this.Volume.GetHashCode();

      public override bool Equals(object obj)
      {
        return obj is ComicBookSeriesStatistics.Key other && this.Equals(other);
      }

      public bool Equals(ComicBookSeriesStatistics.Key other)
      {
        return other != null && this.Series == other.Series && this.Volume == other.Volume;
      }
    }
  }
}

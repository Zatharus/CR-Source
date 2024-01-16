// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.Search.WikiSearch
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Common.Net.Search
{
  public class WikiSearch : CachedSearch
  {
    private const string Wiki = "http://en.wikipedia.org";
    private static readonly Regex rx = new Regex("\\\"(?<text>.*?)\\\"", RegexOptions.Compiled);
    private static readonly Image image = (Image) Resources.Wikipedia;

    public override string Name => "Wikipedia";

    public override Image Image => WikiSearch.image;

    protected override IEnumerable<SearchResult> OnSearch(string hint, string text, int limit)
    {
      string uri = "http://en.wikipedia.org/w/api.php?action=opensearch&limit=" + (object) limit + "&search=" + WebUtility.UrlEncode(text);
      return WikiSearch.rx.Matches(HttpAccess.ReadText(uri)).OfType<Match>().Skip<Match>(1).Select<Match, string>((Func<Match, string>) (m => m.Groups[nameof (text)].Value)).Select<string, SearchResult>((Func<string, SearchResult>) (t => new SearchResult()
      {
        Name = t,
        Result = "http://en.wikipedia.org/wiki/" + t.Replace(" ", "_")
      }));
    }

    protected override string OnGenericSearchLink(string hint, string text)
    {
      return "http://en.wikipedia.org/w/index.php?title=Special:Search&fulltext=Search&search=" + WebUtility.UrlEncode(text);
    }
  }
}

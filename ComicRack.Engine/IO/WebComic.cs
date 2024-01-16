// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.WebComic
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.Net;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider.Readers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  public class WebComic
  {
    private static readonly Regex rxNumbering = new Regex("\\[(?<format>\\d+):(?<start>\\d+)-(?<end>\\d+)\\]", RegexOptions.Compiled);
    private WebComic.ValuePairCollection variables;
    private WebComic.PageLinkCollection images;
    private static readonly Dictionary<string, Regex> rxCache = new Dictionary<string, Regex>();
    private static readonly Regex rxBaseMatcher = new Regex("<base\\s+?href\\s*?=\\s*?\"(?<base>.+?)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Stream logStream;

    static WebComic()
    {
      try
      {
        MethodInfo method = typeof (UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
        FieldInfo field = typeof (UriParser).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
        if (!(method != (MethodInfo) null) || !(field != (FieldInfo) null))
          return;
        string[] strArray = new string[2]{ "http", "https" };
        foreach (string str in strArray)
        {
          UriParser uriParser = (UriParser) method.Invoke((object) null, new object[1]
          {
            (object) str
          });
          if (uriParser != null)
          {
            int num = (int) field.GetValue((object) uriParser);
            if ((num & 16777216) != 0)
              field.SetValue((object) uriParser, (object) (num & -16777217));
          }
        }
      }
      catch (Exception ex)
      {
      }
    }

    public WebComic() => this.Compositing = new WebComic.PageCompositing();

    [DefaultValue(null)]
    public ComicInfo Info { get; set; }

    [XmlArray("Variables")]
    [XmlArrayItem("Variable")]
    public WebComic.ValuePairCollection Variables
    {
      get => this.variables ?? (this.variables = new WebComic.ValuePairCollection());
    }

    [XmlArrayItem("Image")]
    public WebComic.PageLinkCollection Images
    {
      get => this.images ?? (this.images = new WebComic.PageLinkCollection());
    }

    public WebComic.PageCompositing Compositing { get; set; }

    [XmlIgnore]
    public bool ImagesSpecified => this.images != null && this.images.Count > 0;

    [XmlIgnore]
    public bool VariablesSpecified => this.variables != null && this.variables.Count > 0;

    public IEnumerable<WebComic.WebComicImage> GetParsedImages(bool refresh)
    {
      WebComic.WebComicImage parsedImage = new WebComic.WebComicImage()
      {
        Compositing = new WebComic.PageCompositing(this.Compositing)
      };
      int num1 = 0;
      int num2 = 0;
      int rh = 0;
      foreach (WebComic.PageLink pl in (List<WebComic.PageLink>) this.images)
      {
        if (pl.Compositing != null)
        {
          if (parsedImage.Urls.Count != 0)
            yield return parsedImage;
          this.Compositing = pl.Compositing;
          parsedImage = new WebComic.WebComicImage()
          {
            Compositing = new WebComic.PageCompositing(this.Compositing)
          };
          num1 = num2 = rh = 0;
        }
        foreach (WebComic.PageLink p in this.GetImages(pl, refresh))
        {
          if (this.Compositing.PageSize.IsEmpty)
          {
            int num3 = Math.Max(1, this.Compositing.Rows) * Math.Max(1, this.Compositing.Columns);
            parsedImage.Urls.Add(p);
            if (parsedImage.Urls.Count >= num3)
            {
              yield return parsedImage;
              parsedImage = new WebComic.WebComicImage()
              {
                Compositing = new WebComic.PageCompositing(this.Compositing)
              };
              num1 = num2 = rh = 0;
            }
          }
          else
          {
            using (Bitmap bmp = WebComicProvider.BitmapFromBytes(p.Url, refresh))
            {
              if (bmp != null)
              {
                parsedImage.Compositing.PageWidth = Math.Max(parsedImage.Compositing.PageWidth, bmp.Width);
                parsedImage.Compositing.PageHeight = Math.Max(parsedImage.Compositing.PageHeight, bmp.Height);
                while (num1 + bmp.Width > parsedImage.Compositing.PageWidth)
                {
                  if (num2 + bmp.Height <= parsedImage.Compositing.PageHeight)
                  {
                    num2 += bmp.Height;
                    num1 = 0;
                    rh = bmp.Height;
                  }
                  else
                  {
                    yield return parsedImage;
                    parsedImage = new WebComic.WebComicImage()
                    {
                      Compositing = new WebComic.PageCompositing(this.Compositing)
                    };
                    num1 = 0;
                    num2 = 0;
                  }
                }
                p.Left = num1;
                p.Top = num2;
                rh = Math.Max(rh, bmp.Height);
                num1 += bmp.Width;
                parsedImage.Urls.Add(p);
              }
              else
                continue;
            }
          }
        }
      }
      if (parsedImage.Urls.Count != 0)
        yield return parsedImage;
    }

    protected IEnumerable<WebComic.PageLink> GetImages(WebComic.PageLink p, bool refresh)
    {
      WebComic.LogSeparator();
      WebComic.Log("Parsing image: {0}", (object) p.Url);
      WebComic.PagePartCollection parts = new WebComic.PagePartCollection();
      parts.AddRange(((IEnumerable<string>) p.Url.Split('|')).Select<string, WebComic.PagePart>((Func<string, WebComic.PagePart>) (ip => new WebComic.PagePart()
      {
        Pattern = ip
      })));
      parts.AddRange((IEnumerable<WebComic.PagePart>) p.Parts);
      foreach (WebComic.PagePart pagePart in (List<WebComic.PagePart>) parts)
      {
        WebComic.PagePart ip = pagePart;
        this.variables.ForEach((Action<ValuePair<string, string>>) (v => ip.Pattern = ip.Pattern.Replace("{" + v.Key + "}", v.Value)));
      }
      if (WebComic.HasOption(parts[0], "!"))
        p.Type = WebComic.PageLinkType.IndexScraper;
      else if (WebComic.HasOption(parts[0], "?"))
        p.Type = WebComic.PageLinkType.BrowseScraper;
      string s = parts[0].Pattern;
      Match m;
      switch (p.Type)
      {
        case WebComic.PageLinkType.BrowseScraper:
          WebComic.Log("Browse Scraper: {0}", (object) s);
          foreach (WebComic.PageLink browseScraperPage in WebComic.GetBrowseScraperPages(refresh, parts.ToArray()))
            yield return browseScraperPage;
          break;
        case WebComic.PageLinkType.IndexScraper:
          WebComic.Log("Index Scraper: {0}", (object) s);
          foreach (WebComic.PageLink indexScraperPage in WebComic.GetIndexScraperPages(refresh, parts.ToArray()))
            yield return indexScraperPage;
          break;
        default:
          m = WebComic.rxNumbering.Match(s);
          if (!m.Success)
          {
            WebComic.Log("Page Link: {0}", (object) s);
            yield return new WebComic.PageLink() { Url = s };
            break;
          }
          string format = "{0:" + m.Groups["format"].Value + "}";
          int num = int.Parse(m.Groups["start"].Value);
          int end = int.Parse(m.Groups["end"].Value);
          WebComic.Log("Multi Page Link: {0}", (object) s);
          for (int i = num; i <= end; ++i)
          {
            string str = WebComic.rxNumbering.Replace(s, string.Format(format, (object) i));
            WebComic.Log("Link {0}: {1}", (object) i, (object) str);
            yield return new WebComic.PageLink()
            {
              Url = str
            };
          }
          format = (string) null;
          break;
      }
      m = (Match) null;
    }

    public static IEnumerable<WebComic.PageLink> GetIndexScraperPages(
      bool refresh,
      params WebComic.PagePart[] patterns)
    {
      return WebComic.GetIndexScraperPages(refresh, patterns[0].Pattern, WebComic.ReadText(patterns[0].Pattern, true), ((IEnumerable<WebComic.PagePart>) patterns).Skip<WebComic.PagePart>(1), new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
    }

    public static IEnumerable<WebComic.PageLink> GetIndexScraperPages(
      bool refresh,
      string url,
      string pageText,
      IEnumerable<WebComic.PagePart> parts,
      HashSet<string> trackback)
    {
      if (!string.IsNullOrEmpty(pageText))
      {
        WebComic.Log((Action<TextWriter>) (t => t.WriteLine("Parts: {0} [{1}]", (object) parts.Count<WebComic.PagePart>(), (object) parts.ToListString(", "))));
        int num = parts.Count<WebComic.PagePart>();
        bool end = num == 1;
        WebComic.PagePart pagePart = parts.First<WebComic.PagePart>();
        pagePart.Reverse |= WebComic.HasOption(pagePart, "!");
        pagePart.AddOwn |= WebComic.HasOption(pagePart, "+");
        bool reverse = pagePart.Reverse;
        bool flag = pagePart.AddOwn && num > 1;
        bool sort = pagePart.Sort;
        Uri uri = new Uri(url);
        string headerBaseUri = WebComic.GetHeaderBaseUri(pageText);
        IEnumerable<string> matches = WebComic.MatchesRegex(pageText, pagePart).Select<string, string>((Func<string, string>) (m => WebComic.MakeAbsolute(uri, m, headerBaseUri).ToString())).Where<string>((Func<string, bool>) (m => !trackback.Contains(m))).Distinct<string>();
        WebComic.Log((Action<TextWriter>) (t =>
        {
          if (matches.Count<string>() != 0)
            return;
          t.WriteLine("No Matches!");
        }));
        if (flag)
          matches = matches.AddFirst<string>(url).Distinct<string>();
        if (sort)
          matches = (IEnumerable<string>) matches.OrderBy<string, string>((Func<string, string>) (u => u), (IComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (reverse)
          matches = matches.Reverse<string>();
        foreach (string u in matches)
        {
          trackback.Add(u);
          if (end)
          {
            WebComic.Log("Final Match: {0}", (object) u);
            yield return new WebComic.PageLink() { Url = u };
          }
          else
          {
            WebComic.Log("Match Next Page: {0}", (object) u);
            foreach (WebComic.PageLink indexScraperPage in WebComic.GetIndexScraperPages(refresh, u, WebComic.ReadText(u, refresh), parts.Skip<WebComic.PagePart>(1), trackback))
              yield return indexScraperPage;
          }
        }
      }
    }

    public static IEnumerable<WebComic.PageLink> GetBrowseScraperPages(
      bool refresh,
      params WebComic.PagePart[] patterns)
    {
      return WebComic.GetBrowseScraperPages(refresh, patterns[0].Pattern, ((IEnumerable<WebComic.PagePart>) patterns).Last<WebComic.PagePart>(), ((IEnumerable<WebComic.PagePart>) patterns).Skip<WebComic.PagePart>(1).Take<WebComic.PagePart>(patterns.Length - 2), new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
    }

    public static IEnumerable<WebComic.PageLink> GetBrowseScraperPages(
      bool refresh,
      string startPage,
      WebComic.PagePart nextLink,
      IEnumerable<WebComic.PagePart> imageLinks,
      HashSet<string> trackback)
    {
      WebComic.Log("Next Link: {0}", (object) nextLink);
      WebComic.Log((Action<TextWriter>) (t => t.WriteLine("Image Links: {0} [{1}]", (object) imageLinks.Count<WebComic.PagePart>(), (object) imageLinks.ToListString(", "))));
      Uri current = new Uri(startPage);
      bool pageRefresh = refresh;
      string pageText;
      string page;
      while (true)
      {
        page = current.ToString();
        WebComic.Log("Current Page: {0}{1}", (object) page, pageRefresh ? (object) " (refreshing)" : (object) string.Empty);
        if (!trackback.Contains(page))
        {
          try
          {
            pageText = WebComic.ReadText(page, pageRefresh);
          }
          catch
          {
            yield break;
          }
          WebComic.Log("Calling Index scraper on the page");
          IEnumerable<string> strings = WebComic.MatchesRegex(pageText, nextLink);
          if (!strings.IsEmpty<string>())
          {
            string headerBaseUri = WebComic.GetHeaderBaseUri(pageText);
            Uri uri = WebComic.MakeAbsolute(current, strings.First<string>(), headerBaseUri);
            trackback.Add(page);
            if (!pageRefresh && trackback.Contains(uri.ToString()))
            {
              trackback.Remove(page);
              pageRefresh = true;
            }
            else
            {
              current = uri;
              foreach (WebComic.PageLink indexScraperPage in WebComic.GetIndexScraperPages(pageRefresh, page, pageText, imageLinks, trackback))
                yield return indexScraperPage;
              pageText = (string) null;
              page = (string) null;
            }
          }
          else if (!pageRefresh)
          {
            WebComic.Log("Could not find next link: refreshing the page");
            pageRefresh = true;
          }
          else
            goto label_15;
        }
        else
          break;
      }
      yield break;
label_15:
      foreach (WebComic.PageLink indexScraperPage in WebComic.GetIndexScraperPages(true, page, pageText, imageLinks, trackback))
        yield return indexScraperPage;
    }

    private static IEnumerable<string> MatchesRegex(string text, WebComic.PagePart regex)
    {
      if (!string.IsNullOrEmpty(regex.Cut))
      {
        Match m = WebComic.CreateRegex(regex.Cut).Match(text);
        if (!m.Success)
          return Enumerable.Empty<string>();
        text = WebComic.GetValue(m);
      }
      return WebComic.CreateRegex(regex.Pattern).Matches(text).OfType<Match>().Take<Match>(regex.MaximumMatches).Select<Match, string>(new Func<Match, string>(WebComic.GetValue));
    }

    private static Regex CreateRegex(string pattern)
    {
      using (ItemMonitor.Lock((object) WebComic.rxCache))
      {
        Regex regex;
        if (WebComic.rxCache.TryGetValue(pattern, out regex))
          return regex;
        regex = WebComic.rxCache[pattern] = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        return regex;
      }
    }

    private static bool HasOption(ref string s, string option)
    {
      if (!s.StartsWith(option))
        return false;
      s = s.Substring(option.Length);
      return true;
    }

    private static bool HasOption(WebComic.PagePart part, string option)
    {
      string pattern = part.Pattern;
      bool flag = WebComic.HasOption(ref pattern, option);
      part.Pattern = pattern;
      return flag;
    }

    private static string GetValue(Match m)
    {
      string s = m.Groups["link"].Value;
      if (string.IsNullOrEmpty(s))
        s = m.Value;
      return s.Remove("\"").Remove("'").Remove("\r").Remove("\n").Replace("&amp;", "&");
    }

    private static Uri MakeAbsolute(Uri uri, string relative, string baseUri = null)
    {
      if (!string.IsNullOrEmpty(baseUri))
        uri = new Uri(uri, baseUri);
      return new Uri(uri, relative);
    }

    private static string GetHeaderBaseUri(string pageContent)
    {
      Match match = WebComic.rxBaseMatcher.Match(pageContent);
      return !match.Success ? (string) null : match.Groups["base"].Value;
    }

    private static string ReadText(string uri, bool refresh)
    {
      WebComic.Log("Reading Page from '{0}'", (object) uri);
      try
      {
        if (FileCache.Default != null && !refresh)
        {
          string text = FileCache.Default.GetText(uri);
          if (text != null)
            return text;
        }
        try
        {
          string text = HttpAccess.ReadText(uri);
          if (FileCache.Default != null)
            FileCache.Default.AddText(uri, text);
          return text;
        }
        catch
        {
          if (FileCache.Default == null)
          {
            throw;
          }
          else
          {
            string text = FileCache.Default.GetText(uri);
            if (text != null)
              return text;
            throw;
          }
        }
      }
      catch
      {
        WebComic.Log("Failed to read page from '{0}'!", (object) uri);
        throw;
      }
    }

    public static void SetLogOutput(Stream ts) => WebComic.logStream = ts;

    protected static void Log(Action<TextWriter> writeAction)
    {
      if (WebComic.logStream == null || writeAction == null)
        return;
      using (StreamWriter streamWriter = new StreamWriter(WebComic.logStream))
        writeAction((TextWriter) streamWriter);
    }

    protected static void Log(string s) => WebComic.Log((Action<TextWriter>) (t => t.WriteLine(s)));

    protected static void Log(string s, params object[] data)
    {
      WebComic.Log((Action<TextWriter>) (t => t.WriteLine(s, data)));
    }

    protected static void LogSeparator(char c = '-', int width = 40)
    {
      WebComic.Log(new string(c, width));
    }

    public class PageCompositing
    {
      public PageCompositing() => this.Rows = this.Columns = 1;

      public PageCompositing(WebComic.PageCompositing pc)
      {
        this.Rows = pc.Rows;
        this.Columns = pc.Columns;
        this.PageWidth = pc.PageWidth;
        this.PageHeight = pc.PageHeight;
        this.BorderWidth = pc.BorderWidth;
        this.BackgroundColor = pc.BackgroundColor;
        this.RightToLeft = pc.RightToLeft;
      }

      [XmlAttribute]
      [DefaultValue(0)]
      public int PageWidth { get; set; }

      [XmlAttribute]
      [DefaultValue(0)]
      public int PageHeight { get; set; }

      [XmlAttribute]
      [DefaultValue(1)]
      public int Rows { get; set; }

      [XmlAttribute]
      [DefaultValue(1)]
      public int Columns { get; set; }

      [XmlAttribute]
      [DefaultValue(0)]
      public int BorderWidth { get; set; }

      [XmlIgnore]
      [XmlAttribute]
      public string BackgroundColor { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool RightToLeft { get; set; }

      public Size PageSize => new Size(this.PageWidth, this.PageHeight);

      public bool IsEmpty
      {
        get
        {
          if (this.Columns < 1 || this.Rows < 1)
            return true;
          return this.Columns == 1 && this.Rows == 1 && this.BorderWidth == 0 && this.PageSize.IsEmpty;
        }
      }

      public Color BackColor
      {
        get
        {
          if (string.IsNullOrEmpty(this.BackgroundColor))
            return Color.White;
          try
          {
            return ColorTranslator.FromHtml(this.BackgroundColor);
          }
          catch
          {
            return Color.White;
          }
        }
      }
    }

    public enum PageLinkType
    {
      Url,
      BrowseScraper,
      IndexScraper,
    }

    public class PagePart
    {
      public PagePart() => this.MaximumMatches = int.MaxValue;

      [XmlAttribute]
      [DefaultValue(2147483647)]
      public int MaximumMatches { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool AddOwn { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool Reverse { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool Sort { get; set; }

      [XmlAttribute]
      [DefaultValue(null)]
      public string Cut { get; set; }

      [XmlText]
      public string Pattern { get; set; }
    }

    public class PagePartCollection : List<WebComic.PagePart>
    {
    }

    public class PageLink
    {
      private WebComic.PagePartCollection parts;

      [XmlAttribute]
      public string Url { get; set; }

      [XmlAttribute]
      [DefaultValue(WebComic.PageLinkType.Url)]
      public WebComic.PageLinkType Type { get; set; }

      [XmlArrayItem("Part")]
      public WebComic.PagePartCollection Parts
      {
        get => this.parts ?? (this.parts = new WebComic.PagePartCollection());
      }

      [DefaultValue(null)]
      public WebComic.PageCompositing Compositing { get; set; }

      [XmlIgnore]
      public int Left { get; set; }

      [XmlIgnore]
      public int Top { get; set; }

      [XmlIgnore]
      public bool PartSpecified => this.parts != null && this.parts.Count > 0;
    }

    public class PageLinkCollection : List<WebComic.PageLink>
    {
    }

    public class ValuePairCollection : List<ValuePair<string, string>>
    {
    }

    public class WebComicImage
    {
      public WebComicImage() => this.Urls = new List<WebComic.PageLink>();

      public Size PageSize { get; set; }

      public int Rows { get; set; }

      public int Columns { get; set; }

      public List<WebComic.PageLink> Urls { get; private set; }

      public WebComic.PageCompositing Compositing { get; set; }

      public string Name => Path.GetFileName(this.Urls[0].Url);
    }
  }
}

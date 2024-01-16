// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.Search.CachedSearch
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#nullable disable
namespace cYo.Common.Net.Search
{
  public abstract class CachedSearch : INetSearch
  {
    private Dictionary<CachedSearch.CacheKey, SearchResult[]> cache = new Dictionary<CachedSearch.CacheKey, SearchResult[]>();

    public abstract string Name { get; }

    public abstract Image Image { get; }

    public IEnumerable<SearchResult> Search(string hint, string text, int limit)
    {
      CachedSearch.CacheKey key = new CachedSearch.CacheKey()
      {
        Hint = hint,
        Text = text,
        Limit = limit
      };
      SearchResult[] searchResultArray1;
      try
      {
        SearchResult[] searchResultArray2;
        if (this.cache.TryGetValue(key, out searchResultArray2))
          return (IEnumerable<SearchResult>) searchResultArray2;
        searchResultArray1 = this.OnSearch(hint, text, limit).ToArray<SearchResult>();
      }
      catch (Exception ex)
      {
        searchResultArray1 = new SearchResult[0];
      }
      this.cache[key] = searchResultArray1;
      return (IEnumerable<SearchResult>) searchResultArray1;
    }

    public string GenericSearchLink(string hint, string text)
    {
      try
      {
        return this.OnGenericSearchLink(hint, text);
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    protected abstract IEnumerable<SearchResult> OnSearch(string hint, string text, int limit);

    protected abstract string OnGenericSearchLink(string hint, string text);

    public struct CacheKey
    {
      public string Hint;
      public string Text;
      public int Limit;

      public override bool Equals(object obj)
      {
        CachedSearch.CacheKey cacheKey = (CachedSearch.CacheKey) obj;
        return this.Hint == cacheKey.Hint && this.Limit == cacheKey.Limit && this.Text == cacheKey.Text;
      }

      public override int GetHashCode()
      {
        return (this.Text + this.Hint + this.Limit.ToString()).GetHashCode();
      }
    }
  }
}

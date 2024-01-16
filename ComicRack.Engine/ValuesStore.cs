// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ValuesStore
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ValuesStore
  {
    private static readonly IEqualityComparer<string> keyEquality = (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase;
    private readonly Dictionary<string, string> lookup = new Dictionary<string, string>(ValuesStore.keyEquality);

    public ValuesStore(string store)
    {
      if (store == null)
        return;
      foreach (StringPair stringPair in ValuesStore.GetValues(store))
        this.lookup.Add(stringPair.Key, stringPair.Value);
    }

    public string Get(string key)
    {
      string str;
      return !this.lookup.TryGetValue(key, out str) ? (string) null : str;
    }

    public ValuesStore Add(string key, string value)
    {
      if (!string.IsNullOrEmpty(key))
        this.lookup[key] = value;
      return this;
    }

    public ValuesStore Remove(string key)
    {
      this.lookup.Remove(key);
      return this;
    }

    public ValuesStore Clear()
    {
      this.lookup.Clear();
      return this;
    }

    public IEnumerable<StringPair> GetValues()
    {
      return this.lookup.Keys.Select<string, StringPair>((Func<string, StringPair>) (key => new StringPair(key, this.lookup[key])));
    }

    public override string ToString()
    {
      if (this.lookup.Keys.Count == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in (IEnumerable<string>) this.lookup.Keys.OrderBy<string, string>((Func<string, string>) (s => s)))
      {
        if (str.Length != 0)
          stringBuilder.Append(',');
        stringBuilder.Append(ValuesStore.Encode(str));
        stringBuilder.Append("=");
        stringBuilder.Append(ValuesStore.Encode(this.lookup[str]));
      }
      return stringBuilder.ToString();
    }

    private static string Decode(string s) => s?.Replace("&#61;", "=").Replace("&#44;", ",");

    private static string Encode(string s) => s?.Replace("=", "&#61;").Replace(",", "&#44;");

    public static IEnumerable<StringPair> GetValues(string store)
    {
      if (store == null)
        return Enumerable.Empty<StringPair>();
      return ((IEnumerable<string>) store.Split(',')).Select<string, string[]>((Func<string, string[]>) (l => l.Split('='))).Where<string[]>((Func<string[], bool>) (kvp => kvp.Length == 2)).Select<string[], StringPair>((Func<string[], StringPair>) (kvp => new StringPair(ValuesStore.Decode(kvp[0]), ValuesStore.Decode(kvp[1]))));
    }

    public static string GetValue(string store, string key)
    {
      return ValuesStore.GetValues(store).Where<StringPair>((Func<StringPair, bool>) (vp => ValuesStore.keyEquality.Equals(vp.Key, key))).Select<StringPair, string>((Func<StringPair, string>) (vp => vp.Value)).FirstOrDefault<string>();
    }
  }
}

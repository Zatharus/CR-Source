// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.ServerStatistics
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  public class ServerStatistics
  {
    private SmartList<ServerStatistics.StatisticItem> items = new SmartList<ServerStatistics.StatisticItem>();

    public IEnumerable<ServerStatistics.StatisticItem> Items
    {
      get => (IEnumerable<ServerStatistics.StatisticItem>) this.items;
    }

    public void Add(string client, ServerStatistics.StatisticType type, int size = 0)
    {
      this.items.Add(new ServerStatistics.StatisticItem(client, type, size));
    }

    public ServerStatistics.StatisticResult GetResult(TimeSpan timeSpan)
    {
      return new ServerStatistics.StatisticResult(this.Items, timeSpan);
    }

    public ServerStatistics.StatisticResult GetResult(int seconds = 2147483647)
    {
      return new ServerStatistics.StatisticResult(this.Items, TimeSpan.FromSeconds((double) seconds));
    }

    public bool WasActive(int seconds = 10)
    {
      return this.WasActive(TimeSpan.FromSeconds((double) seconds));
    }

    public bool WasActive(TimeSpan timeSpan)
    {
      DateTime now = DateTime.Now;
      return this.items.Reverse<ServerStatistics.StatisticItem>().Any<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => now - n.TimeStamp < timeSpan));
    }

    public enum StatisticType
    {
      InfoRequest,
      LibraryRequest,
      PageRequest,
      ThumbnailRequest,
      FailedAuthentication,
    }

    public class StatisticItem
    {
      public StatisticItem(string client, ServerStatistics.StatisticType type, int size = 0)
      {
        this.TimeStamp = DateTime.Now;
        this.Client = client;
        this.Type = type;
        this.Size = size;
      }

      public DateTime TimeStamp { get; private set; }

      public ServerStatistics.StatisticType Type { get; set; }

      public string Client { get; set; }

      public int Size { get; set; }
    }

    public class StatisticResult
    {
      public StatisticResult()
      {
      }

      public StatisticResult(IEnumerable<ServerStatistics.StatisticItem> items, TimeSpan timeSpan)
      {
        DateTime now = DateTime.Now;
        IEnumerable<ServerStatistics.StatisticItem> source = items.Reverse<ServerStatistics.StatisticItem>().TakeWhile<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => now - n.TimeStamp < timeSpan));
        this.ClientCount = source.Select<ServerStatistics.StatisticItem, string>((Func<ServerStatistics.StatisticItem, string>) (n => n.Client)).Distinct<string>().Count<string>();
        this.InfoRequestCount = source.Count<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => n.Type == ServerStatistics.StatisticType.InfoRequest));
        this.LibraryRequestCount = source.Count<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => n.Type == ServerStatistics.StatisticType.LibraryRequest));
        this.PageRequestCount = source.Count<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => n.Type == ServerStatistics.StatisticType.PageRequest));
        this.ThumbnailRequestCount = source.Count<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => n.Type == ServerStatistics.StatisticType.ThumbnailRequest));
        this.FailedAuthenticationCount = source.Count<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => n.Type == ServerStatistics.StatisticType.FailedAuthentication));
        this.PageRequestSize = source.Where<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => n.Type == ServerStatistics.StatisticType.PageRequest)).Sum<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, long>) (n => (long) n.Size));
        this.LibraryRequestSize = source.Where<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => n.Type == ServerStatistics.StatisticType.LibraryRequest)).Sum<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, long>) (n => (long) n.Size));
        this.ThumbnailRequestSize = source.Where<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, bool>) (n => n.Type == ServerStatistics.StatisticType.ThumbnailRequest)).Sum<ServerStatistics.StatisticItem>((Func<ServerStatistics.StatisticItem, long>) (n => (long) n.Size));
      }

      public int ClientCount { get; private set; }

      public int InfoRequestCount { get; private set; }

      public int LibraryRequestCount { get; private set; }

      public int PageRequestCount { get; private set; }

      public int ThumbnailRequestCount { get; private set; }

      public long PageRequestSize { get; private set; }

      public long ThumbnailRequestSize { get; private set; }

      public long LibraryRequestSize { get; private set; }

      public int FailedAuthenticationCount { get; private set; }

      public long TotalRequestSize
      {
        get => this.PageRequestSize + this.ThumbnailRequestSize + this.LibraryRequestSize;
      }

      public void Add(ServerStatistics.StatisticResult sr)
      {
        this.InfoRequestCount += sr.InfoRequestCount;
        this.LibraryRequestCount += sr.LibraryRequestCount;
        this.PageRequestCount += sr.PageRequestCount;
        this.ThumbnailRequestCount += sr.ThumbnailRequestCount;
        this.FailedAuthenticationCount += sr.FailedAuthenticationCount;
        this.PageRequestSize += sr.PageRequestSize;
        this.LibraryRequestSize += sr.LibraryRequestSize;
        this.ThumbnailRequestSize += sr.ThumbnailRequestSize;
      }
    }
  }
}

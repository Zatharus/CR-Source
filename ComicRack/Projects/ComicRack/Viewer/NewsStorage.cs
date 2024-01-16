// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.NewsStorage
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Net.News;
using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  [Serializable]
  public class NewsStorage
  {
    private readonly NewsStorage.SubscriptionCollection subscriptions = new NewsStorage.SubscriptionCollection();
    private readonly NewsStorage.NewsChannelItemInfoCollection newsChannelItemInfos = new NewsStorage.NewsChannelItemInfoCollection();

    public bool HasUnread
    {
      get
      {
        return this.Items.Find((Predicate<NewsChannelItem>) (item => !this.NewsChannelItemInfos[item].IsRead)) != null;
      }
    }

    public NewsStorage.SubscriptionCollection Subscriptions => this.subscriptions;

    public NewsStorage.NewsChannelItemInfoCollection NewsChannelItemInfos
    {
      get => this.newsChannelItemInfos;
    }

    [XmlIgnore]
    public NewsChannelCollection Channels
    {
      get
      {
        NewsChannelCollection channels = new NewsChannelCollection();
        this.subscriptions.ForEach((Action<NewsStorage.Subscription>) (s => channels.AddRange((IEnumerable<NewsChannel>) s.Channels)));
        return channels;
      }
    }

    [XmlIgnore]
    public NewsChannelItemCollection Items
    {
      get
      {
        NewsChannelItemCollection items = new NewsChannelItemCollection();
        this.Channels.ForEach((Action<NewsChannel>) (nc => items.AddRange((IEnumerable<NewsChannelItem>) nc.Items)));
        return items;
      }
    }

    public void UpdateFeeds(int minutes)
    {
      DateTime universalTime = DateTime.Now.ToUniversalTime();
      try
      {
        foreach (NewsStorage.Subscription subscription in (List<NewsStorage.Subscription>) this.subscriptions)
        {
          if (!(TimeSpan.FromMinutes((double) minutes) > universalTime - subscription.LastUpdate))
          {
            RssNewsFeed rssNewsFeed = new RssNewsFeed();
            try
            {
              rssNewsFeed.ReadFeed(NewsFeed.LoadFeed(subscription.Url));
            }
            catch
            {
            }
            finally
            {
              subscription.Channels.Clear();
              subscription.Channels.AddRange((IEnumerable<NewsChannel>) rssNewsFeed.Channels);
              subscription.LastUpdate = DateTime.UtcNow;
            }
          }
        }
      }
      finally
      {
        this.UpdateChannelInfo();
      }
    }

    public void MarkAllRead()
    {
      this.Items.ForEach((Action<NewsChannelItem>) (item => this.NewsChannelItemInfos[item].IsRead = true));
    }

    private void UpdateChannelInfo()
    {
      NewsChannelItemCollection items = this.Items;
      this.newsChannelItemInfos.RemoveAll((Predicate<NewsStorage.NewsChannelItemInfo>) (ci => items[ci.Guid] == null));
    }

    public static NewsStorage Load(string file)
    {
      try
      {
        return XmlUtility.Load<NewsStorage>(file);
      }
      catch (Exception ex)
      {
        return new NewsStorage();
      }
    }

    public void Save(string file) => XmlUtility.Store(file, (object) this);

    [Serializable]
    public class Subscription
    {
      private string url = string.Empty;
      private string comment = string.Empty;
      private readonly NewsChannelCollection channels = new NewsChannelCollection();
      private DateTime lastTimeRead = DateTime.MinValue;

      public Subscription()
      {
      }

      public Subscription(string url, string comment)
      {
        this.url = url;
        this.comment = comment;
      }

      [XmlAttribute]
      public string Url
      {
        get => this.url;
        set => this.url = value;
      }

      [DefaultValue("")]
      public string Comment
      {
        get => this.comment;
        set => this.comment = value;
      }

      public NewsChannelCollection Channels => this.channels;

      public DateTime LastUpdate
      {
        get => this.lastTimeRead;
        set => this.lastTimeRead = value;
      }
    }

    [Serializable]
    public class SubscriptionCollection : List<NewsStorage.Subscription>
    {
    }

    [Serializable]
    public class NewsChannelItemInfo
    {
      public NewsChannelItemInfo()
      {
      }

      public NewsChannelItemInfo(NewsChannelItem nci) => this.Guid = nci.Guid;

      [XmlAttribute]
      public string Guid { get; set; }

      [XmlAttribute]
      [DefaultValue(false)]
      public bool IsRead { get; set; }
    }

    [Serializable]
    public class NewsChannelItemInfoCollection : List<NewsStorage.NewsChannelItemInfo>
    {
      public NewsStorage.NewsChannelItemInfo this[NewsChannelItem item]
      {
        get
        {
          foreach (NewsStorage.NewsChannelItemInfo newsChannelItemInfo in (List<NewsStorage.NewsChannelItemInfo>) this)
          {
            if (newsChannelItemInfo.Guid == item.Guid)
              return newsChannelItemInfo;
          }
          NewsStorage.NewsChannelItemInfo newsChannelItemInfo1 = new NewsStorage.NewsChannelItemInfo(item);
          this.Add(newsChannelItemInfo1);
          return newsChannelItemInfo1;
        }
      }
    }
  }
}

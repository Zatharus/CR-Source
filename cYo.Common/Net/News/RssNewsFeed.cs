// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.News.RssNewsFeed
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;
using System.Xml;

#nullable disable
namespace cYo.Common.Net.News
{
  public class RssNewsFeed : NewsFeed
  {
    protected override NewsChannelCollection ParseFeed(string xmlFeed)
    {
      NewsChannelCollection feed = new NewsChannelCollection();
      using (XmlTextReader reader = new XmlTextReader((TextReader) new StringReader(xmlFeed)))
      {
        int content = (int) reader.MoveToContent();
        reader.ReadStartElement("rss");
        while (reader.IsStartElement("channel"))
        {
          NewsChannel newsChannel = new NewsChannel();
          reader.ReadStartElement();
          while (reader.IsStartElement())
          {
            switch (reader.Name)
            {
              case "title":
                newsChannel.Title = reader.ReadElementContentAsString();
                continue;
              case "description":
                newsChannel.Description = reader.ReadElementContentAsString();
                continue;
              case "link":
                newsChannel.Link = reader.ReadElementContentAsString();
                continue;
              case "managingEditor":
                newsChannel.Editor = reader.ReadElementContentAsString();
                continue;
              case "item":
                newsChannel.Items.Add(RssNewsFeed.ParseItem((XmlReader) reader));
                continue;
              default:
                reader.Skip();
                continue;
            }
          }
          reader.ReadEndElement();
          feed.Add(newsChannel);
        }
        reader.ReadEndElement();
      }
      return feed;
    }

    protected static NewsChannelItem ParseItem(XmlReader reader)
    {
      NewsChannelItem newsChannelItem = new NewsChannelItem();
      reader.ReadStartElement("item");
      while (reader.IsStartElement())
      {
        switch (reader.Name)
        {
          case "author":
            newsChannelItem.Author = reader.ReadElementContentAsString();
            continue;
          case "category":
            newsChannelItem.Category = reader.ReadElementContentAsString();
            continue;
          case "description":
            newsChannelItem.Description = reader.ReadElementContentAsString();
            continue;
          case "guid":
            newsChannelItem.Guid = reader.ReadElementContentAsString();
            continue;
          case "link":
            newsChannelItem.Link = reader.ReadElementContentAsString();
            continue;
          case "pubDate":
            DateTime result;
            newsChannelItem.Published = !DateTime.TryParse(reader.ReadElementContentAsString(), out result) ? DateTime.Now : result;
            continue;
          case "title":
            newsChannelItem.Title = reader.ReadElementContentAsString();
            continue;
          default:
            reader.Skip();
            continue;
        }
      }
      reader.ReadEndElement();
      if (string.IsNullOrEmpty(newsChannelItem.Guid))
      {
        int hashCode1 = string.IsNullOrEmpty(newsChannelItem.Title) ? 0 : newsChannelItem.Title.GetHashCode();
        int hashCode2 = newsChannelItem.Published.GetHashCode();
        int hashCode3 = string.IsNullOrEmpty(newsChannelItem.Description) ? 0 : newsChannelItem.Description.GetHashCode();
        newsChannelItem.Guid = (hashCode1 ^ hashCode2 ^ hashCode3).ToString();
      }
      return newsChannelItem;
    }
  }
}

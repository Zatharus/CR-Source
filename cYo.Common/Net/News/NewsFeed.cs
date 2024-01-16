// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.News.NewsFeed
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

#nullable disable
namespace cYo.Common.Net.News
{
  public abstract class NewsFeed
  {
    private string rawFeed;
    private NewsChannelCollection channels = new NewsChannelCollection();

    public void ReadFeed(string feed)
    {
      if (this.rawFeed == feed)
        return;
      this.rawFeed = feed;
      this.channels = this.ParseFeed(feed);
    }

    public string RawFeed => this.rawFeed;

    public NewsChannelCollection Channels => this.channels;

    protected abstract NewsChannelCollection ParseFeed(string xmlFeed);

    public static string LoadFeed(string url) => HttpAccess.ReadText(url);
  }
}

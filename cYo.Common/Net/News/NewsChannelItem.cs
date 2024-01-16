// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.News.NewsChannelItem
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Common.Net.News
{
  public class NewsChannelItem
  {
    [DefaultValue(null)]
    public string Guid { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Link { get; set; }

    [DefaultValue(null)]
    public string Author { get; set; }

    [DefaultValue(null)]
    public string Category { get; set; }

    public DateTime Published { get; set; }
  }
}

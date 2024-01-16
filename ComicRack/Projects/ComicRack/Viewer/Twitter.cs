// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Twitter
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using LinqToTwitter;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public static class Twitter
  {
    private const int TweetLength = 144;
    private const int TweetHashSize = 25;
    private const string TweetComicRackTag = "#comicrack";

    public static void Tweet(IWin32Window parent, string text, Bitmap thumbnail)
    {
      if (!Program.Settings.HasTwitterAccess)
      {
        int num1 = (int) MessageBox.Show(parent, TR.Messages["TwitterAuth", "A Browser window will open to authorize ComicRack to post on Twitter."], TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      PinAuthorizer pinAuthorizer = new PinAuthorizer();
      pinAuthorizer.Credentials = (IOAuthCredentials) new InMemoryCredentials()
      {
        ConsumerKey = "lc7K38IqLGD9tlvj9DP42w",
        ConsumerSecret = "RgiRdp7tjUg1F3120P1N7cWB2AN7tSD3HjjOVuQtaW4",
        OAuthToken = Program.Settings.TwitterOAuthToken,
        AccessToken = Program.Settings.TwitterAccessToken,
        UserId = Program.Settings.TwitterUserId,
        ScreenName = Program.Settings.TwitterScreenName
      };
      pinAuthorizer.UseCompression = true;
      pinAuthorizer.AuthAccessType = AuthAccessType.NoChange;
      pinAuthorizer.GoToTwitterAuthorization = (Action<string>) (pageLink => Process.Start(pageLink));
      pinAuthorizer.GetPin = (Func<string>) (() => TwitterPinDialog.GetPin(parent));
      PinAuthorizer authorization = pinAuthorizer;
      try
      {
        authorization.Authorize();
        TwitterContext twitterContext = new TwitterContext((ITwitterAuthorizer) authorization);
        if (thumbnail == null)
          twitterContext.UpdateStatus(text);
        else
          twitterContext.TweetWithMedia(text, false, cYo.Common.Collections.ListExtensions.AsEnumerable<Media>(new Media()
          {
            ContentType = MediaContentType.Jpeg,
            Data = thumbnail.ImageToBytes(ImageFormat.Jpeg)
          }).ToList<Media>());
        Program.Settings.TwitterOAuthToken = authorization.Credentials.OAuthToken;
        Program.Settings.TwitterAccessToken = authorization.Credentials.AccessToken;
        Program.Settings.TwitterUserId = authorization.Credentials.UserId;
        Program.Settings.TwitterScreenName = authorization.Credentials.ScreenName;
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("duplicate"))
          return;
        int num2 = (int) MessageBox.Show(parent, ex.Message, TR.Messages["Error", "Error"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        if (!ex.Message.Contains("auth"))
          return;
        bool hasTwitterAccess = Program.Settings.HasTwitterAccess;
        Program.Settings.ResetTwitter();
        if (!hasTwitterAccess)
          return;
        Twitter.Tweet(parent, text, thumbnail);
      }
    }

    public static void Tweet(IWin32Window parent, ComicBook cb, Bitmap thumbnail)
    {
      Twitter.Tweet(parent, Twitter.CreateReviewTweet(cb), thumbnail);
    }

    public static string CreateTweet(string seriesHashTag, string text, float rating)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((double) rating > 0.0)
      {
        stringBuilder.Append(Twitter.CreateRatingText(rating, true));
        stringBuilder.Append(" ");
      }
      stringBuilder.Append("#");
      stringBuilder.Append(seriesHashTag);
      stringBuilder.Append(" ");
      stringBuilder.Append("#comicrack");
      stringBuilder.Append(" ");
      if (!string.IsNullOrEmpty(text))
      {
        int num = 143 - stringBuilder.Length;
        if (text.Length > num)
          text = text.Substring(0, num - 3) + "...";
        stringBuilder.Insert(0, " ");
        stringBuilder.Insert(0, text);
      }
      return stringBuilder.ToString();
    }

    public static string CreateReviewTweet(ComicBook cb)
    {
      string seriesHashTag = Twitter.CreateSeriesHashTag(cb, false);
      return seriesHashTag == null ? (string) null : Twitter.CreateTweet(seriesHashTag, cb.Review, cb.Rating);
    }

    public static string CreateRatingText(float rating, bool symbolic)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = (int) rating;
      if (!symbolic)
      {
        stringBuilder.Append(num);
      }
      else
      {
        for (int index = 0; index < num; ++index)
          stringBuilder.Append("*");
      }
      rating -= (float) num;
      if ((double) rating >= 0.75)
        stringBuilder.Append("\u00BE");
      else if ((double) rating >= 0.5)
        stringBuilder.Append("\u00BD");
      else if ((double) rating >= 0.25)
        stringBuilder.Append("\u00BC");
      return stringBuilder.ToString();
    }

    public static string CreateSeriesHashTag(ComicBook cb, bool includeVolume)
    {
      if (string.IsNullOrEmpty(cb.ShadowSeries))
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder(cb.ShadowSeries.ShortenText(25));
      if (!string.IsNullOrEmpty(cb.ShadowNumber))
      {
        if (char.IsDigit(stringBuilder[stringBuilder.Length - 1]))
          stringBuilder.Append("_");
        stringBuilder.Append(cb.ShadowNumber.ShortenText());
      }
      if (includeVolume && cb.ShadowVolume > 0)
      {
        stringBuilder.Append("V");
        stringBuilder.Append(cb.ShadowVolume);
      }
      return stringBuilder.ToString();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.WebComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Net;
using cYo.Common.Text;
using cYo.Common.Xml;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  [FileFormat("eComic (WebComic)", 7, ".cbw", EnableUpdate = true, Dynamic = true)]
  public class WebComicProvider : ComicProvider, IDynamicImages
  {
    private readonly List<WebComic.WebComicImage> images = new List<WebComic.WebComicImage>();

    public WebComicProvider()
    {
      this.DisableNtfs = true;
      this.DisableSidecar = true;
    }

    protected override bool IsSupportedImage(string file) => true;

    public override bool IsSlow => true;

    public override ImageProviderCapabilities Capabilities
    {
      get => ImageProviderCapabilities.FastFormatCheck;
    }

    protected override bool OnFastFormatCheck(string source)
    {
      return WebComicProvider.Load(source) != null;
    }

    public override string CreateHash()
    {
      using (FileStream inputStream = File.OpenRead(this.Source))
        return Base32.ToBase32String(new SHA1Managed().ComputeHash((Stream) inputStream));
    }

    protected override void OnParse()
    {
      WebComic webComic = WebComicProvider.Load(this.Source);
      if (webComic == null)
        return;
      webComic.Variables.Add(new ValuePair<string, string>("ComicFileName", Path.GetFileName(this.Source)));
      webComic.Variables.Add(new ValuePair<string, string>("ComicFilePath", "file://" + Path.GetDirectoryName(this.Source)));
      this.images.Clear();
      foreach (WebComic.WebComicImage parsedImage in webComic.GetParsedImages(this.RefreshMode))
      {
        this.images.Add(parsedImage);
        if (!this.FireIndexReady(new ProviderImageInfo(this.images.Count - 1, parsedImage.Name, 0L)))
          break;
      }
    }

    protected override byte[] OnRetrieveSourceByteImage(int index)
    {
      WebComic.WebComicImage image1 = this.images[index];
      if (image1.Compositing.IsEmpty)
        return WebComicProvider.RetrieveSourceByteImage(image1.Urls[0].Url, this.RefreshMode);
      WebComic.PageCompositing compositing = image1.Compositing;
      \u003C\u003Ef__AnonymousType0<WebComic.PageLink, Bitmap>[] array = image1.Urls.Select(uri => new
      {
        Uri = uri,
        Bitmap = WebComicProvider.BitmapFromBytes(uri.Url, this.RefreshMode)
      }).Where(bmp => bmp.Bitmap != null).ToArray();
      Bitmap image2 = (Bitmap) null;
      try
      {
        if (compositing.PageSize.IsEmpty)
        {
          int val1_1 = 0;
          int num1 = 0;
          int val1_2 = 0;
          int val2 = 0;
          int num2 = 0;
          int index1 = 0;
          while (true)
          {
            if (index1 % compositing.Columns == 0 || index1 >= array.Length)
            {
              val1_1 = Math.Max(val1_1, val2);
              num1 += val1_2;
              val1_2 = 0;
              val2 = 0;
              num2 = 0;
              if (index1 >= array.Length)
                break;
            }
            Bitmap bitmap = array[index1].Bitmap;
            val1_2 = Math.Max(val1_2, bitmap.Height);
            val2 += bitmap.Width;
            ++index1;
            ++num2;
          }
          int num3 = (int) (Math.Sqrt((double) (num1 * num1 + val1_1 * val1_1)) * (double) compositing.BorderWidth / 100.0);
          image2 = new Bitmap(val1_1 + num3 * 2, num1 + num3 * 2);
          using (Graphics graphics = Graphics.FromImage((Image) image2))
          {
            graphics.Clear(compositing.BackColor);
            int num4 = num3;
            int y = num3;
            int num5 = 0;
            int val1_3 = 0;
            foreach (var data in array)
            {
              int x = compositing.RightToLeft ? val1_1 - data.Bitmap.Width - num4 : num4;
              graphics.DrawImage((Image) data.Bitmap, x, y, data.Bitmap.Width, data.Bitmap.Height);
              num4 += data.Bitmap.Width;
              val1_3 = Math.Max(val1_3, data.Bitmap.Height);
              if (++num5 >= compositing.Columns)
              {
                num4 = num3;
                y += val1_3;
                val1_3 = 0;
                num5 = 0;
              }
            }
          }
        }
        else
        {
          int pageWidth = compositing.PageWidth;
          int pageHeight = compositing.PageHeight;
          int num6 = (int) (Math.Sqrt((double) (pageHeight * pageHeight + pageWidth * pageWidth)) * (double) compositing.BorderWidth / 100.0);
          image2 = new Bitmap(pageWidth + num6 * 2, pageHeight + num6 * 2);
          Rectangle seed = new Rectangle();
          Rectangle rectangle = array.Select(bmp => new Rectangle(bmp.Uri.Left, bmp.Uri.Top, bmp.Bitmap.Width, bmp.Bitmap.Height)).Aggregate<Rectangle, Rectangle>(seed, (Func<Rectangle, Rectangle, Rectangle>) ((current, rb) => !current.IsEmpty ? Rectangle.Union(current, rb) : rb));
          int num7 = (image2.Width - rectangle.Width) / 2;
          int num8 = (image2.Height - rectangle.Height) / 2;
          using (Graphics graphics = Graphics.FromImage((Image) image2))
          {
            graphics.Clear(compositing.BackColor);
            foreach (var data in array)
            {
              int x = compositing.RightToLeft ? pageWidth - num7 - data.Uri.Left - data.Bitmap.Width : num7 + data.Uri.Left;
              int y = num8 + data.Uri.Top;
              graphics.DrawImage((Image) data.Bitmap, x, y, data.Bitmap.Width, data.Bitmap.Height);
            }
          }
        }
        return image2.ImageToBytes(ImageFormat.Jpeg);
      }
      catch (Exception ex)
      {
        return (byte[]) null;
      }
      finally
      {
        array.Select(bmp => bmp.Bitmap).Dispose();
        image2.SafeDispose();
      }
    }

    protected override ComicInfo OnLoadInfo()
    {
      try
      {
        return WebComicProvider.Load(this.Source).Info;
      }
      catch
      {
        return (ComicInfo) null;
      }
    }

    protected override bool OnStoreInfo(ComicInfo comicInfo)
    {
      try
      {
        WebComic data = WebComicProvider.Load(this.Source);
        data.Info = comicInfo;
        using (FileStream s = File.Create(this.Source))
          XmlUtility.Store((Stream) s, (object) data, false);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static Bitmap BitmapFromBytes(string url, bool refresh)
    {
      try
      {
        return BitmapExtensions.BitmapFromBytes(WebComicProvider.RetrieveSourceByteImage(url, refresh));
      }
      catch (Exception ex)
      {
        return (Bitmap) null;
      }
    }

    public static byte[] RetrieveSourceByteImage(string uri, bool refreshMode)
    {
      try
      {
        if (FileCache.Default != null && !refreshMode)
        {
          byte[] numArray = FileCache.Default.GetItem(uri);
          if (numArray != null)
            return numArray;
        }
        byte[] numArray1 = HttpAccess.ReadBinary(uri);
        if (numArray1 != null && FileCache.Default != null)
          FileCache.Default.AddItem(uri, numArray1);
        return numArray1;
      }
      catch
      {
        return (byte[]) null;
      }
    }

    private static WebComic Load(string file)
    {
      try
      {
        return XmlUtility.Load<WebComic>(file);
      }
      catch (Exception ex)
      {
        return (WebComic) null;
      }
    }

    public bool RefreshMode { get; set; }
  }
}

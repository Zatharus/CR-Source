// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.WebImage
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

#nullable disable
namespace cYo.Common.Net
{
  public class WebImage : Component
  {
    public static string DefaultCacheLocation;
    private bool isLoading;
    private string name = nameof (Image);
    private Uri uri;
    private string cacheLocation;
    private TimeSpan checkIntervall = new TimeSpan(7, 0, 0, 0);
    private volatile Bitmap image;

    public WebImage()
    {
    }

    public WebImage(IContainer container) => container.Add((IComponent) this);

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.Image != null)
        this.Image.Dispose();
      base.Dispose(disposing);
    }

    [DefaultValue("Image")]
    public string Name
    {
      get => this.name;
      set => this.name = value;
    }

    [DefaultValue(null)]
    public Uri Uri
    {
      get => this.uri;
      set => this.uri = value;
    }

    [DefaultValue(null)]
    public string CacheLocation
    {
      get => this.cacheLocation;
      set => this.cacheLocation = value;
    }

    [DefaultValue(null)]
    public System.Drawing.Image DefaultImage { get; set; }

    public TimeSpan CheckIntervall
    {
      get => this.checkIntervall;
      set => this.checkIntervall = value;
    }

    public Bitmap Image
    {
      get => this.image;
      protected set => this.image = value;
    }

    public void LoadImage(string uri)
    {
      this.Uri = new Uri(uri);
      this.LoadImage();
    }

    public void LoadImage()
    {
      if (!this.CacheValid())
      {
        if (this.isLoading)
          return;
        this.isLoading = true;
        ThreadPool.QueueUserWorkItem(new WaitCallback(this.LoadWebImage));
      }
      else
      {
        this.Image = this.GetCachedImage();
        this.OnImageLoaded();
      }
    }

    public event EventHandler ImageLoaded;

    protected virtual void OnImageLoaded()
    {
      if (this.ImageLoaded == null)
        return;
      this.ImageLoaded((object) this, EventArgs.Empty);
    }

    private void LoadWebImage(object state)
    {
      try
      {
        using (Bitmap image = (Bitmap) System.Drawing.Image.FromStream(new HttpAccess()
        {
          AskProxyCredentials = false
        }.GetStream(this.Uri)))
        {
          Bitmap copy = image.CreateCopy(PixelFormat.Format32bppArgb);
          this.CacheImage((System.Drawing.Image) copy);
          this.Image = copy;
        }
        this.OnImageLoaded();
      }
      catch
      {
      }
      finally
      {
        this.isLoading = false;
      }
    }

    private string GetCacheLocation()
    {
      return string.IsNullOrEmpty(this.cacheLocation) ? WebImage.DefaultCacheLocation : this.cacheLocation;
    }

    private string GetCacheFilename() => Path.Combine(this.GetCacheLocation(), this.Name);

    private Bitmap GetCachedImage()
    {
      try
      {
        return (Bitmap) System.Drawing.Image.FromFile(this.GetCacheFilename());
      }
      catch
      {
        return (Bitmap) null;
      }
    }

    private bool CacheValid()
    {
      try
      {
        return DateTime.Now - new FileInfo(this.GetCacheFilename()).LastWriteTime < this.checkIntervall;
      }
      catch
      {
        return false;
      }
    }

    private void CacheImage(System.Drawing.Image image)
    {
      try
      {
        image.Save(this.GetCacheFilename(), ImageFormat.Png);
      }
      catch
      {
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.StorageSetting
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.Text;
using System;
using System.ComponentModel;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  [Serializable]
  public class StorageSetting
  {
    public StorageSetting()
    {
      this.DontEnlarge = true;
      this.PageHeight = 1000;
      this.PageWidth = 1000;
      this.PageResize = StoragePageResize.Original;
      this.PageCompression = 75;
      this.PageType = StoragePageType.Original;
      this.RemovePages = true;
      this.RemovePageFilter = ComicPageType.Deleted;
      this.EmbedComicInfo = true;
      this.ComicCompression = ExportCompression.None;
      this.ThumbnailSize = new Size(0, 256);
      this.DoublePages = DoublePageHandling.Keep;
      this.Resampling = EngineConfiguration.Default.ExportResampling;
    }

    [DefaultValue(0)]
    public int FormatId { get; set; }

    [DefaultValue(ExportCompression.None)]
    public ExportCompression ComicCompression { get; set; }

    [DefaultValue(true)]
    public bool EmbedComicInfo { get; set; }

    [DefaultValue(true)]
    public bool RemovePages { get; set; }

    [DefaultValue(ComicPageType.Deleted)]
    public ComicPageType RemovePageFilter { get; set; }

    [DefaultValue(null)]
    public string IncludePages { get; set; }

    [DefaultValue(false)]
    public bool IgnoreErrorPages { get; set; }

    [DefaultValue(StoragePageType.Original)]
    public StoragePageType PageType { get; set; }

    [DefaultValue(75)]
    public int PageCompression { get; set; }

    [DefaultValue(StoragePageResize.Original)]
    public StoragePageResize PageResize { get; set; }

    [DefaultValue(1000)]
    public int PageWidth { get; set; }

    [DefaultValue(1000)]
    public int PageHeight { get; set; }

    [DefaultValue(true)]
    public bool DontEnlarge { get; set; }

    [DefaultValue(DoublePageHandling.Keep)]
    public DoublePageHandling DoublePages { get; set; }

    [DefaultValue(false)]
    public bool AddKeyToPageInfo { get; set; }

    [DefaultValue(BitmapResampling.GdiPlusHQ)]
    public BitmapResampling Resampling { get; set; }

    [DefaultValue(false)]
    public bool KeepOriginalImageNames { get; set; }

    [DefaultValue(typeof (BitmapAdjustment), "0,0,0")]
    public BitmapAdjustment ImageProcessing { get; set; }

    [DefaultValue(false)]
    public bool CreateThumbnails { get; set; }

    [DefaultValue(typeof (Size), "0, 256")]
    public Size ThumbnailSize { get; set; }

    public bool IsValidPage(int page) => this.IncludePages.TestRangeString(page + 1);
  }
}

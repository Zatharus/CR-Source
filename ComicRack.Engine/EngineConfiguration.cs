// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.EngineConfiguration
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using cYo.Common.Runtime;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class EngineConfiguration
  {
    private string tempPath = Path.GetTempPath();
    private float pageBowWidth = 0.07f;
    private int pageBowFromAlpha = 92;
    private int pageBowToAlpha;
    private int maximumQueueThreads = 4;
    private static EngineConfiguration defaultConfig;

    public EngineConfiguration()
    {
      this.PageScrollingDuration = 1000;
      this.AnimationDuration = 250;
      this.BlendDuration = 400;
      this.SoftwareFilterDelay = 1000;
      this.ListCoverSize = new Size(512, 512);
      this.ListCoverAlpha = 0.3f;
      this.NavigationPanelWidth = 0.9f;
      this.BookmarkColors = new Color[4]
      {
        Color.Orange,
        Color.Green,
        Color.Red,
        Color.Blue
      };
      this.ThumbnailResampling = BitmapResampling.FastBilinear;
      this.ThumbnailQuality = 60;
      this.ThumbnailPageBow = true;
      this.ExportResampling = BitmapResampling.GdiPlusHQ;
      this.SyncResamping = BitmapResampling.GdiPlus;
      this.SoftwareFilter = BitmapResampling.GdiPlusHQ;
      this.ComicCaptionFormat = "[{format} ][{series}][ {volume}][ #{number}][ - {title}][ ({year}[/{month}[/{day}]])]";
      this.ComicExportFileNameFormat = "[{format} ][{series}][ {volume}][ #{number}][ ({year}[/{month}])]";
      this.PageBowColor = Color.Black;
      this.PageBowCenter = true;
      this.PageBowBorder = true;
      this.SoftwareFilterMinScale = 0.05f;
      this.PageShadowWidthPercentage = 1f;
      this.PageShadowOpacity = 0.6f;
      this.IsRecentInDays = 14;
      this.IsReadCompletionPercentage = 95;
      this.IsNotReadCompletionPercentage = 10;
      this.OperationTimeout = 300;
      this.ServerProviderCacheSize = 100;
      this.DjVuSizeLimit = new Size(2000, 2000);
      this.GestureAreaSize = 80;
      this.ShowGestureHint = true;
      this.AeroFullScreenWorkaround = true;
      this.BlankPageColor = Color.White;
      this.RatingStarsBelowThumbnails = true;
      this.EnableParallelQueries = true;
      this.SyncOptimizeQuality = 65;
      this.SyncOptimizeMaxHeight = 1500;
      this.SyncOptimizeSharpen = false;
      this.SyncQueueLength = 50;
      this.SyncCreateThumbnails = true;
      this.SyncOptimizeWebP = true;
      this.SyncKeepReadComics = 1;
      this.PageCachingDelay = 1000;
      this.CbzUses = this.CbrUses = this.Cb7Uses = this.CbtUses = EngineConfiguration.CbEngines.SevenZip;
      this.FreeDeviceMemoryMB = 128;
      this.ParallelConversions = 32;
      this.WifiSyncReceiveTimeout = 5000;
      this.WifiSyncSendTimeout = 5000;
      this.WifiSyncConnectionTimeout = 2500;
      this.WifiSyncConnectionRetries = 1;
    }

    [DefaultValue(true)]
    public bool EnableParallelQueries { get; set; }

    public bool IsEnableParallelQueriesDefault => this.EnableParallelQueries;

    [DefaultValue(null)]
    public string IgnoredArticles { get; set; }

    [DefaultValue(null)]
    public string OfValues { get; set; }

    [DefaultValue(false)]
    public bool LegacyFilenameParser { get; set; }

    [DefaultValue(1000)]
    public int PageScrollingDuration { get; set; }

    [DefaultValue(300)]
    public int AnimationDuration { get; set; }

    [DefaultValue(250)]
    public int BlendDuration { get; set; }

    [DefaultValue(1000)]
    public int SoftwareFilterDelay { get; set; }

    [DefaultValue(typeof (Size), "512, ´512")]
    public Size ListCoverSize { get; set; }

    [DefaultValue(0.3f)]
    public float ListCoverAlpha { get; set; }

    [DefaultValue(0.9f)]
    public float NavigationPanelWidth { get; set; }

    public string TempPath
    {
      get => this.tempPath;
      set
      {
        if (!Directory.Exists(value))
          return;
        this.tempPath = value;
      }
    }

    [TypeConverter(typeof (ArrayConverter<Color>))]
    [DefaultValue(typeof (Color[]), "Orange, Green, Red, Blue")]
    public Color[] BookmarkColors { get; set; }

    [DefaultValue(false)]
    public bool CacheThumbnailPages { get; set; }

    [DefaultValue(BitmapResampling.FastBilinear)]
    public BitmapResampling ThumbnailResampling { get; set; }

    [DefaultValue(60)]
    public int ThumbnailQuality { get; set; }

    [DefaultValue(BitmapResampling.GdiPlusHQ)]
    public BitmapResampling ExportResampling { get; set; }

    [DefaultValue(BitmapResampling.GdiPlus)]
    public BitmapResampling SyncResamping { get; set; }

    [DefaultValue(BitmapResampling.GdiPlusHQ)]
    public BitmapResampling SoftwareFilter { get; set; }

    [DefaultValue("[{format} ][{series}][ {volume}][ #{number}][ - {title}][ ({year}[/{month}[/{day}]])]")]
    public string ComicCaptionFormat { get; set; }

    [DefaultValue("[{format} ][{series}][ {volume}][ #{number}][ ({year}[/{month}])]")]
    public string ComicExportFileNameFormat { get; set; }

    [DefaultValue(false)]
    public bool DisableGhostscript { get; set; }

    [DefaultValue(null)]
    public string GhostscriptExecutable { get; set; }

    [DefaultValue(null)]
    public string DjVuLibreInstall { get; set; }

    [DefaultValue(typeof (Size), "2000, 2000")]
    public Size DjVuSizeLimit { get; set; }

    [DefaultValue(false)]
    public bool MirroredPageTurnAnimation { get; set; }

    [DefaultValue(0.07f)]
    public float PageBowWidth
    {
      get => this.pageBowWidth;
      set => this.pageBowWidth = value.Clamp(0.01f, 0.5f);
    }

    [DefaultValue(92)]
    public int PageBowFromAlpha
    {
      get => this.pageBowFromAlpha;
      set => this.pageBowFromAlpha = value.Clamp(0, (int) byte.MaxValue);
    }

    [DefaultValue(0)]
    public int PageBowToAlpha
    {
      get => this.pageBowToAlpha;
      set => this.pageBowToAlpha = value.Clamp(0, (int) byte.MaxValue);
    }

    [DefaultValue(typeof (Color), "Black")]
    public Color PageBowColor { get; set; }

    [DefaultValue(true)]
    public bool PageBowCenter { get; set; }

    [DefaultValue(true)]
    public bool PageBowBorder { get; set; }

    [DefaultValue(0.05f)]
    public float SoftwareFilterMinScale { get; set; }

    [DefaultValue(4)]
    public int MaximumQueueThreads
    {
      get => this.maximumQueueThreads;
      set => this.maximumQueueThreads = value.Clamp(1, 32);
    }

    [DefaultValue(1)]
    public float PageShadowWidthPercentage { get; set; }

    [DefaultValue(0.6f)]
    public float PageShadowOpacity { get; set; }

    [DefaultValue(14)]
    public int IsRecentInDays { get; set; }

    [DefaultValue(95)]
    public int IsReadCompletionPercentage { get; set; }

    [DefaultValue(10)]
    public int IsNotReadCompletionPercentage { get; set; }

    [DefaultValue(300)]
    public int OperationTimeout { get; set; }

    [DefaultValue(100)]
    public int ServerProviderCacheSize { get; set; }

    [DefaultValue(80)]
    public int GestureAreaSize { get; set; }

    [DefaultValue(true)]
    public bool ShowGestureHint { get; set; }

    [DefaultValue(false)]
    public bool HtmlInfoContextMenu { get; set; }

    [DefaultValue(false)]
    public bool EnableHtmlScriptErrors { get; set; }

    [DefaultValue(false)]
    public bool HideVisiblePartOverlayClose { get; set; }

    [DefaultValue(true)]
    public bool AeroFullScreenWorkaround { get; set; }

    [DefaultValue(typeof (Color), "Empty")]
    public Color ThumbnailPageCurlColor { get; set; }

    [DefaultValue(true)]
    public bool ThumbnailPageBow { get; set; }

    [DefaultValue(typeof (Color), "White")]
    public Color BlankPageColor { get; set; }

    [DefaultValue(false)]
    public bool SearchBrowserCaseSensitive { get; set; }

    [DefaultValue(true)]
    public bool RatingStarsBelowThumbnails { get; set; }

    public int SyncOptimizeQuality { get; set; }

    public int SyncOptimizeMaxHeight { get; set; }

    public bool SyncOptimizeSharpen { get; set; }

    public bool SyncWebP { get; set; }

    [DefaultValue(true)]
    public bool SyncOptimizeWebP { get; set; }

    [DefaultValue(true)]
    public bool SyncCreateThumbnails { get; set; }

    [DefaultValue(null)]
    public string ExtraWifiDeviceAddresses { get; set; }

    [DefaultValue(50)]
    public int SyncQueueLength { get; set; }

    [DefaultValue(1)]
    public int SyncKeepReadComics { get; set; }

    [DefaultValue(1000)]
    public int PageCachingDelay { get; set; }

    [DefaultValue(EngineConfiguration.CbEngines.SevenZip)]
    public EngineConfiguration.CbEngines CbzUses { get; set; }

    [DefaultValue(EngineConfiguration.CbEngines.SevenZip)]
    public EngineConfiguration.CbEngines CbrUses { get; set; }

    [DefaultValue(EngineConfiguration.CbEngines.SevenZip)]
    public EngineConfiguration.CbEngines Cb7Uses { get; set; }

    [DefaultValue(EngineConfiguration.CbEngines.SevenZip)]
    public EngineConfiguration.CbEngines CbtUses { get; set; }

    [DefaultValue(128)]
    public int FreeDeviceMemoryMB { get; set; }

    [DefaultValue(4)]
    public int ParallelConversions { get; set; }

    [DefaultValue(5000)]
    public int WifiSyncReceiveTimeout { get; set; }

    [DefaultValue(5000)]
    public int WifiSyncSendTimeout { get; set; }

    [DefaultValue(2500)]
    public int WifiSyncConnectionTimeout { get; set; }

    [DefaultValue(1)]
    public int WifiSyncConnectionRetries { get; set; }

    public string GetTempFileName()
    {
      return Path.Combine(this.TempPath, Guid.NewGuid().ToString() + ".tmp");
    }

    public static EngineConfiguration Default
    {
      get
      {
        return EngineConfiguration.defaultConfig ?? (EngineConfiguration.defaultConfig = IniFile.Default.Register<EngineConfiguration>());
      }
    }

    public enum CbEngines
    {
      SevenZip,
      SevenZipExe,
      SharpCompress,
      SharpZip,
    }
  }
}

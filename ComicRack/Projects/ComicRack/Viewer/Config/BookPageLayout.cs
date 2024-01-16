// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.BookPageLayout
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Drawing;
using cYo.Common.Mathematics;
using cYo.Projects.ComicRack.Engine.Display;
using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  [Serializable]
  public class BookPageLayout
  {
    private float pageZoom = 1f;

    public BookPageLayout()
    {
      this.FitOnlyIfOversized = true;
      this.PageDisplayMode = ImageFitMode.FitWidth;
    }

    [Browsable(false)]
    [DefaultValue(ImageFitMode.FitWidth)]
    public ImageFitMode PageDisplayMode { get; set; }

    [DefaultValue(true)]
    public bool FitOnlyIfOversized { get; set; }

    [DefaultValue(false)]
    public bool AutoRotate { get; set; }

    [Browsable(false)]
    [DefaultValue(ImageRotation.None)]
    public ImageRotation PageImageRotation { get; set; }

    [Browsable(false)]
    [DefaultValue(PageLayoutMode.Single)]
    public PageLayoutMode PageLayout { get; set; }

    [Browsable(false)]
    [DefaultValue(1f)]
    public float PageZoom
    {
      get => this.pageZoom;
      set => this.pageZoom = value.Clamp(1f, 8f);
    }

    [DefaultValue(false)]
    public bool TwoPageAutoScroll { get; set; }
  }
}

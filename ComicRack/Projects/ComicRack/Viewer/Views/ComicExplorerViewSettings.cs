// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicExplorerViewSettings
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Windows.Forms;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  [Serializable]
  public class ComicExplorerViewSettings
  {
    public ComicExplorerViewSettings()
    {
      this.SearchBrowserColumn3 = 2;
      this.SearchBrowserColumn1 = 1;
      this.TopBrowserSplit = FormUtility.ScaleDpiY(150);
      this.PreviewSplit = FormUtility.ScaleDpiY(200);
      this.BrowserSplit = FormUtility.ScaleDpiY(250);
      this.InfoBrowserSize = new System.Drawing.Size(200, 150).ScaleDpi();
      this.ShowBrowser = true;
    }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool ShowBrowser { get; set; }

    [XmlAttribute]
    [DefaultValue(150)]
    public int BrowserSplit { get; set; }

    [XmlAttribute]
    [DefaultValue(150)]
    public int PreviewSplit { get; set; }

    [XmlAttribute]
    [DefaultValue(150)]
    public int TopBrowserSplit { get; set; }

    [DefaultValue(typeof (System.Drawing.Size), "200, 150")]
    public System.Drawing.Size InfoBrowserSize { get; set; }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool InfoBrowserRight { get; set; }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool ShowPreview { get; set; }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool ShowInfo { get; set; }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool ShowTopBrowser { get; set; }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool ShowSearchBrowser { get; set; }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool ShowSearchBar { get; set; }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool TwoPagePreview { get; set; }

    [DefaultValue(null)]
    public ItemViewConfig ItemViewConfig { get; set; }

    [DefaultValue(1)]
    public int SearchBrowserColumn1 { get; set; }

    [DefaultValue(0)]
    public int SearchBrowserColumn2 { get; set; }

    [DefaultValue(2)]
    public int SearchBrowserColumn3 { get; set; }
  }
}

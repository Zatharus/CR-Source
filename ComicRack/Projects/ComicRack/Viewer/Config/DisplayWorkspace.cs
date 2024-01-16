// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.DisplayWorkspace
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Viewer.Views;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  [Serializable]
  public class DisplayWorkspace : IComparable<DisplayWorkspace>, INamed, IDescription
  {
    private string backgroundColor = "WhiteSmoke";
    private float paperTextureStrength = 1f;
    private ImageLayout paperTextureLayout = ImageLayout.Tile;
    private ImageLayout backgroundImageLayout = ImageLayout.Tile;

    public DisplayWorkspace(string name)
    {
      this.Name = name;
      this.LandscapeLayout = new BookPageLayout();
      this.PortraitLayout = new BookPageLayout();
      this.PageImageBackgroundMode = ImageBackgroundMode.Color;
      this.DrawRealisticPages = true;
      this.PageTransitionEffect = PageTransitionEffect.Fade;
      this.PagesViewConfig = new ItemViewConfig();
      this.FileView = new ComicExplorerViewSettings();
      this.DatabaseView = new ComicExplorerViewSettings();
      this.UndockedReaderState = FormWindowState.Normal;
      this.FormState = FormWindowState.Normal;
      this.PanelVisible = true;
      this.PanelDock = DockStyle.Bottom;
      this.PanelSize = new System.Drawing.Size(400, 250).ScaleDpi();
      this.Type = WorkspaceType.Default;
      this.PageMarginPercentWidth = 0.05f;
      this.MinimalGui = true;
    }

    public DisplayWorkspace()
      : this(string.Empty)
    {
    }

    [DefaultValue("")]
    public string Name { get; set; }

    [DefaultValue(WorkspaceType.Default)]
    public WorkspaceType Type { get; set; }

    public string Description
    {
      get
      {
        string separator = ", ";
        string text = string.Empty;
        if (this.Type.IsSet<WorkspaceType>(WorkspaceType.WindowLayout))
          text = text.AppendWithSeparator(separator, TR.Default["Windows"]);
        if (this.Type.IsSet<WorkspaceType>(WorkspaceType.ViewsSetup))
          text = text.AppendWithSeparator(separator, TR.Default["Lists"]);
        if (this.Type.IsSet<WorkspaceType>(WorkspaceType.ComicPageLayout))
          text = text.AppendWithSeparator(separator, TR.Default["Layout"]);
        if (this.Type.IsSet<WorkspaceType>(WorkspaceType.ComicPageDisplay))
          text = text.AppendWithSeparator(separator, TR.Default["Display"]);
        return text;
      }
    }

    [Browsable(false)]
    [DefaultValue(typeof (System.Drawing.Size), "400, 250")]
    public System.Drawing.Size PanelSize { get; set; }

    [Browsable(false)]
    [DefaultValue(DockStyle.Bottom)]
    public DockStyle PanelDock { get; set; }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool PanelVisible { get; set; }

    [Browsable(false)]
    public Rectangle FormBounds { get; set; }

    [Browsable(false)]
    [DefaultValue(FormWindowState.Normal)]
    public FormWindowState FormState { get; set; }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool FullScreen { get; set; }

    [Browsable(false)]
    [DefaultValue(true)]
    public bool MinimalGui { get; set; }

    [Browsable(false)]
    public ItemViewConfig ComicBookDialogPagesConfig { get; set; }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool ReaderUndocked { get; set; }

    [Browsable(false)]
    public Rectangle UndockedReaderBounds { get; set; }

    [Browsable(false)]
    [DefaultValue(FormWindowState.Normal)]
    public FormWindowState UndockedReaderState { get; set; }

    [Browsable(false)]
    public ComicExplorerViewSettings DatabaseView { get; set; }

    [Browsable(false)]
    public ComicExplorerViewSettings FileView { get; set; }

    [Browsable(false)]
    public ItemViewConfig PagesViewConfig { get; set; }

    public BookPageLayout LandscapeLayout { get; set; }

    public BookPageLayout PortraitLayout { get; set; }

    public BookPageLayout Layout
    {
      get => !Screen.PrimaryScreen.IsLandscape() ? this.PortraitLayout : this.LandscapeLayout;
    }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool RightToLeftReading { get; set; }

    [Browsable(false)]
    [DefaultValue(PageTransitionEffect.Fade)]
    public PageTransitionEffect PageTransitionEffect { get; set; }

    [DefaultValue(true)]
    public bool DrawRealisticPages { get; set; }

    [DefaultValue(ImageBackgroundMode.Color)]
    public ImageBackgroundMode PageImageBackgroundMode { get; set; }

    [DefaultValue("WhiteSmoke")]
    public string BackgroundColor
    {
      get => this.backgroundColor;
      set => this.backgroundColor = ColorExtensions.IsNamedColor(value);
    }

    [XmlIgnore]
    public Color BackColor
    {
      get => Color.FromName(this.BackgroundColor);
      set => this.BackgroundColor = value.Name;
    }

    [DefaultValue(null)]
    public string BackgroundTexture { get; set; }

    [DefaultValue(false)]
    public bool PageMargin { get; set; }

    [DefaultValue(0.05f)]
    public float PageMarginPercentWidth { get; set; }

    [DefaultValue(null)]
    public string PaperTexture { get; set; }

    [DefaultValue(1f)]
    public float PaperTextureStrength
    {
      get => this.paperTextureStrength;
      set => this.paperTextureStrength = value.Clamp(0.0f, 1f);
    }

    [DefaultValue(ImageLayout.Tile)]
    public ImageLayout PaperTextureLayout
    {
      get => this.paperTextureLayout;
      set => this.paperTextureLayout = value;
    }

    [DefaultValue(ImageLayout.Tile)]
    public ImageLayout BackgroundImageLayout
    {
      get => this.backgroundImageLayout;
      set => this.backgroundImageLayout = value;
    }

    [XmlIgnore]
    public bool IsWindowLayout
    {
      get => this.IsType(WorkspaceType.WindowLayout);
      set => this.SetType(WorkspaceType.WindowLayout, value);
    }

    [XmlIgnore]
    public bool IsViewsSetup
    {
      get => this.IsType(WorkspaceType.ViewsSetup);
      set => this.SetType(WorkspaceType.ViewsSetup, value);
    }

    [XmlIgnore]
    public bool IsComicPageLayout
    {
      get => this.IsType(WorkspaceType.ComicPageLayout);
      set => this.SetType(WorkspaceType.ComicPageLayout, value);
    }

    [XmlIgnore]
    public bool IsComicPageDisplay
    {
      get => this.IsType(WorkspaceType.ComicPageDisplay);
      set => this.SetType(WorkspaceType.ComicPageDisplay, value);
    }

    public bool IsType(WorkspaceType type) => this.Type.IsSet<WorkspaceType>(type);

    public void SetType(WorkspaceType type, bool set)
    {
      this.Type = this.Type.SetMask<WorkspaceType>(type, set);
    }

    public int CompareTo(DisplayWorkspace other) => string.Compare(this.Name, other.Name);

    public override string ToString() => this.Name;
  }
}

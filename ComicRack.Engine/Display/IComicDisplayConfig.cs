// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.IComicDisplayConfig
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display
{
  public interface IComicDisplayConfig
  {
    ImageBackgroundMode ImageBackgroundMode { get; set; }

    Color BackColor { get; set; }

    string BackgroundTexture { get; set; }

    ImageLayout BackgroundImageLayout { get; set; }

    string PaperTexture { get; set; }

    float PaperTextureStrength { get; set; }

    ImageLayout PaperTextureLayout { get; set; }

    bool PageMargin { get; set; }

    float PageMarginPercentWidth { get; set; }

    PageLayoutMode PageLayout { get; set; }

    float DoublePageOverlap { get; set; }

    bool ImageAutoRotate { get; set; }

    ImageRotation ImageRotation { get; set; }

    float ImageZoom { get; set; }

    bool ImageFitOnlyIfOversized { get; set; }

    ImageFitMode ImageFitMode { get; set; }

    ImageDisplayOptions ImageDisplayOptions { get; set; }

    bool RealisticPages { get; set; }

    bool AutoHideCursor { get; set; }

    bool LeftRightMovementReversed { get; set; }

    bool RightToLeftReading { get; set; }

    RightToLeftReadingMode RightToLeftReadingMode { get; set; }

    bool TwoPageNavigation { get; set; }

    bool BlendWhilePaging { get; set; }

    float MagnifierOpacity { get; set; }

    Size MagnifierSize { get; set; }

    bool MagnifierVisible { get; set; }

    float MagnifierZoom { get; set; }

    MagnifierStyle MagnifierStyle { get; set; }

    bool AutoHideMagnifier { get; set; }

    bool AutoMagnifier { get; set; }

    float InfoOverlayScaling { get; set; }

    InfoOverlays VisibleInfoOverlays { get; set; }

    bool SmoothScrolling { get; set; }

    PageTransitionEffect PageTransitionEffect { get; set; }

    bool DisplayChangeAnimation { get; set; }

    bool FlowingMouseScrolling { get; set; }

    bool SoftwareFiltering { get; set; }

    bool HardwareFiltering { get; set; }
  }
}

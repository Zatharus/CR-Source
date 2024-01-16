// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.IComicDisplay
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using System;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display
{
  public interface IComicDisplay : IComicDisplayConfig
  {
    ComicBookNavigator Book { get; set; }

    IPagePool PagePool { get; set; }

    IThumbnailPool ThumbnailPool { get; set; }

    bool IsValid { get; }

    bool IsMovementFlipped { get; }

    int CurrentPage { get; }

    int CurrentMousePage { get; }

    ImageRotation CurrentImageRotation { get; }

    Size ImageSize { get; }

    int ImagePartCount { get; }

    bool IsDoubleImage { get; }

    ImagePartInfo ImageVisiblePart { get; set; }

    bool SetRenderer(bool hardware);

    bool IsHardwareRenderer { get; }

    bool ShouldPagingBlend { get; }

    bool NavigationOverlayVisible { get; set; }

    object GetState();

    void Animate(object a, object b, int time);

    void Animate(Action<float> animate, int time);

    event EventHandler BookChanged;

    event EventHandler DrawnPageCountChanged;

    event EventHandler<BrowseEventArgs> Browse;

    event EventHandler<BookPageEventArgs> PageChange;

    event EventHandler<BookPageEventArgs> PageChanged;

    event EventHandler<GestureEventArgs> Gesture;

    event EventHandler<GestureEventArgs> PreviewGesture;

    event EventHandler VisibleInfoOverlaysChanged;

    Bitmap CreatePageImage();

    void MovePartDown(float percent);

    bool MovePart(Point offset);

    bool DisplayPart(PartPageToDisplay ptd);

    void DisplayOpenMessage();

    void ZoomTo(Point location, float zoom);
  }
}

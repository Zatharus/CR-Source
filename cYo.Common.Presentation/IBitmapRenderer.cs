// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.IBitmapRenderer
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

#nullable disable
namespace cYo.Common.Presentation
{
  public interface IBitmapRenderer
  {
    void Clear(Color color);

    bool BeginScene(Graphics gr);

    void EndScene();

    void DrawImage(
      RendererImage image,
      RectangleF dest,
      RectangleF src,
      BitmapAdjustment transform,
      float opacity);

    void DrawBlurredImage(RendererImage image, RectangleF dest, RectangleF src, float blur);

    void FillRectangle(RectangleF bounds, Color color);

    void DrawLine(IEnumerable<PointF> points, Color color, float width);

    bool IsVisible(RectangleF bounds);

    IDisposable SaveState();

    Matrix Transform { set; get; }

    void TranslateTransform(float dx, float dy);

    void ScaleTransform(float dx, float dy);

    void RotateTransform(float angel);

    bool HighQuality { get; set; }

    float Opacity { get; set; }

    CompositingMode CompositingMode { get; set; }

    RectangleF Clip { get; set; }

    bool IsHardware { get; }

    bool IsLocked { get; }
  }
}

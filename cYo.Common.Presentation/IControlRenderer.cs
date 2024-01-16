// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.IControlRenderer
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation
{
  public interface IControlRenderer : IBitmapRenderer, IDisposable
  {
    event EventHandler Paint;

    Control Control { get; }

    Size Size { get; }

    void Draw();
  }
}

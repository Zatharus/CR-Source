// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.PanelSurface
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.ComponentModel;
using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class PanelSurface : DisposableObject
  {
    private readonly Graphics graphics;

    public PanelSurface(Bitmap bitmap) => this.graphics = Graphics.FromImage((Image) bitmap);

    public Graphics Graphics => this.graphics;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.graphics.Dispose();
      base.Dispose(disposing);
    }
  }
}

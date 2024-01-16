// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.RendererGdiImage
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.ComponentModel;
using System;
using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation
{
  public class RendererGdiImage : RendererImage
  {
    private readonly WeakReference<Bitmap> weakReference;

    public RendererGdiImage(Bitmap bitmap)
    {
      this.weakReference = new WeakReference<Bitmap>(bitmap);
    }

    public override bool IsValid => this.weakReference.IsAlive<Bitmap>();

    public override Bitmap Bitmap => this.weakReference.GetData<Bitmap>();
  }
}

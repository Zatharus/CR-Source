// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.IHardwareRenderer
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation
{
  public interface IHardwareRenderer : IBitmapRenderer
  {
    Bitmap GetFramebuffer(Rectangle rc, bool flip);

    bool IsSoftwareRenderer { get; }

    void ClearStencil();

    StencilMode StencilMode { get; set; }

    bool OptimizedTextures { get; set; }

    bool EnableFilter { get; set; }

    BlendingOperation BlendingOperation { get; set; }
  }
}

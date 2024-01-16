// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.IBitmapDisplayControl
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Drawing;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public interface IBitmapDisplayControl : IDisposable
  {
    Bitmap Bitmap { get; set; }

    object Tag { get; set; }

    void SetBitmap(Bitmap bitmap);
  }
}

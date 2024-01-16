// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemViewStates
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [Flags]
  public enum ItemViewStates
  {
    None = 0,
    Selected = 1,
    Focused = 2,
    Hot = 4,
    All = Hot | Focused | Selected, // 0x00000007
  }
}

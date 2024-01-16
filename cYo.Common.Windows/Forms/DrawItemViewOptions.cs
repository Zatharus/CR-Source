// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.DrawItemViewOptions
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [Flags]
  public enum DrawItemViewOptions
  {
    Background = 1,
    BackgroundImage = 2,
    ColumnHeaders = 4,
    GroupHeaders = 8,
    SelectedOnly = 16, // 0x00000010
    FocusRectangle = 32, // 0x00000020
    Default = FocusRectangle | GroupHeaders | ColumnHeaders | BackgroundImage | Background, // 0x0000002F
  }
}

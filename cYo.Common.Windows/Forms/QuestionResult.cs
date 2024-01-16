// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.QuestionResult
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [Flags]
  public enum QuestionResult
  {
    Cancel = 1,
    Ok = 2,
    Option = 4,
    Option2 = 8,
    OkWithOption = Option | Ok, // 0x00000006
  }
}

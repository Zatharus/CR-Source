// Decompiled with JetBrains decompiler
// Type: cYo.Common.Collections.SmartListOptions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Collections
{
  [Flags]
  public enum SmartListOptions
  {
    None = 0,
    Synchronized = 1,
    DisableOnSet = 2,
    DisableOnInsert = 4,
    DisableOnRemove = 8,
    DisableOnClear = 16, // 0x00000010
    DisableOnRefresh = 32, // 0x00000020
    ClearWithRemove = 64, // 0x00000040
    DisposeOnRemove = 128, // 0x00000080
    DisableCollectionChangedEvent = 256, // 0x00000100
    CheckedSet = 512, // 0x00000200
    Default = CheckedSet | ClearWithRemove | DisableOnSet | Synchronized, // 0x00000243
  }
}

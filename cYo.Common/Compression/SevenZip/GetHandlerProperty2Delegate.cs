﻿// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.GetHandlerProperty2Delegate
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate int GetHandlerProperty2Delegate(
    int formatIndex,
    ArchivePropId propID,
    ref PropVariant value);
}

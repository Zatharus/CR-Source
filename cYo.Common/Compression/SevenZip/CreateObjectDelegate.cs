// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.CreateObjectDelegate
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate int CreateObjectDelegate(
    [In] ref Guid classID,
    [In] ref Guid interfaceID,
    [MarshalAs(UnmanagedType.Interface)] out object outObject);
}

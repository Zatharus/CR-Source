// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.IArchiveUpdateCallback2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  [Guid("23170F69-40C1-278A-0000-000600820000")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IArchiveUpdateCallback2
  {
    void SetTotal(long total);

    void SetCompleted([In] ref long completeValue);

    void GetUpdateItemInfo(
      int index,
      out int newData,
      out int newProperties,
      out int indexInArchive);

    void GetProperty(int index, ItemPropId propID, IntPtr value);

    void GetStream(int index, out ISequentialInStream inStream);

    void SetOperationResult(int operationResult);

    void GetVolumeSize(int index, out long size);

    void GetVolumeStream(int index, out ISequentialOutStream volumeStream);
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.IArchiveOpenVolumeCallback
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  [Guid("23170F69-40C1-278A-0000-000600300000")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IArchiveOpenVolumeCallback
  {
    void GetProperty(ItemPropId propID, IntPtr value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetStream([MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.Interface)] out IInStream inStream);
  }
}

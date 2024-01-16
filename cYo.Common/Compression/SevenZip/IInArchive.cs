// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.IInArchive
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  [Guid("23170F69-40C1-278A-0000-000600600000")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IInArchive
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Open(
      IInStream stream,
      [In] ref long maxCheckStartPosition,
      [MarshalAs(UnmanagedType.Interface)] IArchiveOpenCallback openArchiveCallback);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Close();

    int GetNumberOfItems();

    void GetProperty(int index, ItemPropId propID, ref PropVariant value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Extract(
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] indices,
      int numItems,
      int testMode,
      [MarshalAs(UnmanagedType.Interface)] IArchiveExtractCallback extractCallback);

    void GetArchiveProperty(int propID, ref PropVariant value);

    int GetNumberOfProperties();

    void GetPropertyInfo(int index, [MarshalAs(UnmanagedType.BStr)] out string name, out ItemPropId propID, out short varType);

    int GetNumberOfArchiveProperties();

    void GetArchivePropertyInfo(int index, [MarshalAs(UnmanagedType.BStr)] string name, ref int propID, ref short varType);
  }
}

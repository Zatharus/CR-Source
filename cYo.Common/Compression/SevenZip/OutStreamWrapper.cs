// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.OutStreamWrapper
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  public class OutStreamWrapper : StreamWrapper, ISequentialOutStream, IOutStream
  {
    public OutStreamWrapper(Stream baseStream)
      : base(baseStream)
    {
    }

    public int SetSize(long newSize)
    {
      this.BaseStream.SetLength(newSize);
      return 0;
    }

    public int Write(byte[] data, int size, IntPtr processedSize)
    {
      this.BaseStream.Write(data, 0, size);
      if (processedSize != IntPtr.Zero)
        Marshal.WriteInt32(processedSize, size);
      return 0;
    }
  }
}

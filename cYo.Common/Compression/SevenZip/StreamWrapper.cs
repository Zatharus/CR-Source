// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.StreamWrapper
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System;
using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  public class StreamWrapper : DisposableObject
  {
    private Stream baseStream;

    protected StreamWrapper(Stream baseStream) => this.baseStream = baseStream;

    protected override void Dispose(bool disposing)
    {
      if (this.baseStream == null)
        return;
      this.baseStream.Dispose();
    }

    public virtual void Seek(long offset, int seekOrigin, IntPtr newPosition)
    {
      long val = this.baseStream.Seek(offset, (SeekOrigin) seekOrigin);
      if (!(newPosition != IntPtr.Zero))
        return;
      Marshal.WriteInt64(newPosition, val);
    }

    public Stream BaseStream
    {
      get => this.baseStream;
      protected set => this.baseStream = value;
    }
  }
}

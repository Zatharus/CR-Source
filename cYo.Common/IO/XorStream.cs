// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.XorStream
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class XorStream : Stream
  {
    private int mask;
    private Stream stream;

    public XorStream(Stream stream, int mask)
    {
      this.stream = stream;
      this.mask = mask;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.stream != null)
      {
        this.stream.Dispose();
        this.stream = (Stream) null;
      }
      base.Dispose(disposing);
    }

    public override void Close()
    {
      base.Close();
      this.stream.Close();
    }

    public override void Flush() => this.stream.Flush();

    public override long Seek(long offset, SeekOrigin origin) => this.stream.Seek(offset, origin);

    public override void SetLength(long value) => this.stream.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num = this.stream.Read(buffer, offset, count);
      for (int index = 0; index < num; ++index)
        buffer[offset + index] ^= (byte) this.mask;
      return num;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public override bool CanRead => this.stream.CanRead;

    public override bool CanSeek => this.stream.CanSeek;

    public override bool CanWrite => false;

    public override long Length => this.stream.Length;

    public override long Position
    {
      get => this.stream.Position;
      set => this.stream.Position = value;
    }
  }
}

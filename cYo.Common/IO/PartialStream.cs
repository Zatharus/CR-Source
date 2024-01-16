// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.PartialStream
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class PartialStream : Stream
  {
    private readonly long length;
    private readonly long start;
    private readonly Stream baseStream;

    public PartialStream(Stream baseStream, long start, long length)
    {
      this.baseStream = baseStream != null ? baseStream : throw new ArgumentNullException(nameof (baseStream));
      this.length = length;
      if (start >= 0L)
        baseStream.Seek(start, SeekOrigin.Begin);
      this.start = baseStream.Position;
    }

    public PartialStream(string file, long start, long length)
      : this((Stream) File.OpenRead(file), start, length)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.baseStream != null)
        this.baseStream.Dispose();
      base.Dispose(disposing);
    }

    public override void Close()
    {
      if (this.baseStream != null)
        this.baseStream.Close();
      base.Close();
    }

    public override bool CanRead => this.baseStream.CanRead;

    public override bool CanSeek => this.baseStream.CanSeek;

    public override bool CanWrite => false;

    public override void Flush() => throw new IOException("Can not write");

    public override long Length => this.length;

    public override long Position
    {
      get => this.baseStream.Position - this.start;
      set => this.baseStream.Position = value + this.start;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return this.baseStream.Read(buffer, offset, Math.Min(count, (int) (this.length - this.Position)));
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long num = 0;
      switch (origin)
      {
        case SeekOrigin.Begin:
          num = offset;
          break;
        case SeekOrigin.Current:
          num = this.Position + offset;
          break;
        case SeekOrigin.End:
          num = this.length + offset;
          break;
      }
      return this.baseStream.Seek(this.start + num, SeekOrigin.Begin) - this.start;
    }

    public override void SetLength(long value)
    {
      throw new Exception("This stream does not support this action");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new Exception("This stream does not support this action");
    }
  }
}

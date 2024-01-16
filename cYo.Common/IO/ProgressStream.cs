// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.ProgressStream
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class ProgressStream : Stream
  {
    private readonly Stream baseStream;

    public ProgressStream(Stream baseStream, bool baseStreamOwned = true)
    {
      this.baseStream = baseStream;
      this.BaseStreamOwned = baseStreamOwned;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.BaseStreamOwned)
        this.baseStream.Dispose();
      base.Dispose(disposing);
    }

    public bool BaseStreamOwned { get; set; }

    public Stream BaseStream => this.baseStream;

    public override bool CanRead => this.baseStream.CanRead;

    public override bool CanSeek => this.baseStream.CanSeek;

    public override bool CanWrite => this.baseStream.CanWrite;

    public override void Flush() => this.baseStream.Flush();

    public override long Length => this.baseStream.Length;

    public override long Position
    {
      get => this.baseStream.Position;
      set => this.baseStream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      count = this.baseStream.Read(buffer, offset, count);
      this.OnDataRead(count);
      return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this.baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value) => this.baseStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.baseStream.Write(buffer, offset, count);
    }

    public event EventHandler<ProgressStreamReadEventArgs> DataRead;

    protected virtual void OnDataRead(int count)
    {
      if (this.DataRead == null)
        return;
      try
      {
        this.DataRead((object) this, new ProgressStreamReadEventArgs(count));
      }
      catch
      {
      }
    }
  }
}

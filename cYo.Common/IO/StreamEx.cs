// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.StreamEx
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class StreamEx : Stream
  {
    private Stream baseStream;

    public StreamEx(Stream baseStream) => this.baseStream = baseStream;

    public override void Close()
    {
      base.Close();
      this.OnClosed();
    }

    public event EventHandler Closed;

    protected virtual void OnClosed()
    {
      if (this.Closed == null)
        return;
      this.Closed((object) this, EventArgs.Empty);
    }

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
      return this.baseStream.Read(buffer, offset, count);
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
  }
}

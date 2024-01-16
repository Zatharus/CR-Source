// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.InStreamTimedWrapper
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  public class InStreamTimedWrapper : StreamWrapper, ISequentialInStream, IInStream
  {
    private const int KeepAliveInterval = 5000;
    private long baseStreamLastPosition;
    private Timer closeTimer;
    private string baseStreamFileName;

    public InStreamTimedWrapper(Stream baseStream)
      : base(baseStream)
    {
      if (!(baseStream is FileStream fileStream) || baseStream.CanWrite || !baseStream.CanSeek)
        return;
      this.baseStreamFileName = fileStream.Name;
      this.closeTimer = new Timer(new TimerCallback(this.CloseStream), (object) null, 5000, -1);
    }

    protected override void Dispose(bool disposing)
    {
      this.CloseStream((object) null);
      this.baseStreamFileName = (string) null;
    }

    private void CloseStream(object state)
    {
      if (this.closeTimer != null)
      {
        this.closeTimer.Dispose();
        this.closeTimer = (Timer) null;
      }
      if (this.BaseStream == null)
        return;
      if (this.BaseStream.CanSeek)
        this.baseStreamLastPosition = this.BaseStream.Position;
      this.BaseStream.Close();
      this.BaseStream = (Stream) null;
    }

    private void ReopenStream()
    {
      if (this.BaseStream == null || !this.BaseStream.CanRead)
      {
        this.BaseStream = this.baseStreamFileName != null ? (Stream) new FileStream(this.baseStreamFileName, FileMode.Open, FileAccess.Read, FileShare.Read) : throw new ObjectDisposedException("StreamWrapper");
        this.BaseStream.Position = this.baseStreamLastPosition;
        this.closeTimer = new Timer(new TimerCallback(this.CloseStream), (object) null, 5000, -1);
      }
      else
      {
        if (this.closeTimer == null)
          return;
        this.closeTimer.Change(5000, -1);
      }
    }

    public void Flush() => this.CloseStream((object) null);

    public int Read(byte[] data, int size)
    {
      this.ReopenStream();
      return this.BaseStream.Read(data, 0, size);
    }

    public override void Seek(long offset, int seekOrigin, IntPtr newPosition)
    {
      if (this.BaseStream == null && this.baseStreamFileName != null && offset == 0L && seekOrigin == 0)
      {
        this.baseStreamLastPosition = 0L;
        if (!(newPosition != IntPtr.Zero))
          return;
        Marshal.WriteInt64(newPosition, this.baseStreamLastPosition);
      }
      else
      {
        this.ReopenStream();
        base.Seek(offset, seekOrigin, newPosition);
      }
    }

    public string BaseStreamFileName => this.baseStreamFileName;
  }
}

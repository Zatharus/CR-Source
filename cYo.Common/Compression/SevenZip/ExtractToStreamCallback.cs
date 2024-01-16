// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.ExtractToStreamCallback
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.IO;

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  public class ExtractToStreamCallback : IProgress, IArchiveExtractCallback
  {
    private readonly int fileNumber;
    private readonly Stream stream;
    private OutStreamWrapper fileStream;

    public ExtractToStreamCallback(int fileNumber, Stream ms)
    {
      this.fileNumber = fileNumber;
      this.stream = ms;
    }

    public void SetTotal(long total)
    {
    }

    public void SetCompleted(ref long completeValue)
    {
    }

    public int GetStream(int index, out ISequentialOutStream outStream, AskMode askExtractMode)
    {
      outStream = (ISequentialOutStream) null;
      if (index == this.fileNumber && askExtractMode == AskMode.kExtract)
      {
        this.fileStream = new OutStreamWrapper(this.stream);
        outStream = (ISequentialOutStream) this.fileStream;
      }
      return 0;
    }

    public void PrepareOperation(AskMode askExtractMode)
    {
    }

    public void SetOperationResult(OperationResult resultEOperationResult)
    {
      if (this.fileStream == null)
        return;
      this.fileStream.Dispose();
    }
  }
}

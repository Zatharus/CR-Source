// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Pdf.PdfNative
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Pdf
{
  public class PdfNative : IComicAccessor
  {
    public bool IsFormat(string source) => throw new NotImplementedException();

    public IEnumerable<ProviderImageInfo> GetEntryList(string source)
    {
      PdfNative.Match streamStartTag = new PdfNative.Match("stream");
      PdfNative.Match streamEndTag = new PdfNative.Match("endstream");
      using (FileStream readStream = File.OpenRead(source))
      {
        PdfNative.Reader reader = new PdfNative.Reader((Stream) readStream);
        int b = 0;
        while (b != -1)
        {
          do
            ;
          while ((b = reader.ReadByte()) != -1 && !streamStartTag.IsMatch(b));
          b = reader.ReadByte();
          while (b != -1 && (b == 10 || b == 13))
            b = reader.ReadByte();
          long offset = reader.GetPosition() - 1L;
          if (b != (int) byte.MaxValue || reader.ReadByte() != 216)
          {
            b = reader.ReadByte();
          }
          else
          {
            do
              ;
            while ((b = reader.ReadByte()) != -1 && !streamEndTag.IsMatch(b));
            yield return (ProviderImageInfo) new PdfNative.ImageStreamInfo(reader.GetPosition() - offset - (long) streamEndTag.Length - 1L, offset);
          }
        }
        reader = (PdfNative.Reader) null;
      }
    }

    public byte[] ReadByteImage(string source, ProviderImageInfo info)
    {
      return PdfNative.LoadBitmapData(source, (PdfNative.ImageStreamInfo) info);
    }

    public ComicInfo ReadInfo(string source) => (ComicInfo) null;

    public bool WriteInfo(string source, ComicInfo info) => false;

    private static byte[] LoadBitmapData(string file, PdfNative.ImageStreamInfo si)
    {
      try
      {
        byte[] buffer = new byte[si.Size];
        using (FileStream fileStream = File.OpenRead(file))
        {
          fileStream.Seek(si.Offset, SeekOrigin.Begin);
          return fileStream.Read(buffer, 0, buffer.Length) != buffer.Length ? (byte[]) null : buffer;
        }
      }
      catch (Exception ex)
      {
        return (byte[]) null;
      }
    }

    private class ImageStreamInfo : ProviderImageInfo
    {
      private readonly long offset;

      public ImageStreamInfo(string name, long size, long offset)
        : base(0, name, size)
      {
        this.offset = offset;
      }

      public ImageStreamInfo(long size, long offset)
        : this((string) null, size, offset)
      {
      }

      public long Offset => this.offset;
    }

    private class Match
    {
      private int matchPos;
      private readonly char[] tag;

      public Match(string tag) => this.tag = tag.ToCharArray();

      public bool IsMatch(int b)
      {
        if ((int) this.tag[this.matchPos] == b)
          ++this.matchPos;
        else if (this.matchPos != 0)
        {
          this.matchPos = 0;
          if ((int) this.tag[this.matchPos] == b)
            ++this.matchPos;
        }
        if (this.matchPos != this.tag.Length)
          return false;
        this.matchPos = 0;
        return true;
      }

      public int Length => this.tag.Length;
    }

    private class Reader
    {
      private byte[] readBuffer = new byte[100000];
      private long bufferOffset;
      private int bufferLen;
      private int bufferPos;
      private Stream readStream;

      public Reader(Stream readStream) => this.readStream = readStream;

      public long GetPosition() => this.bufferOffset + (long) this.bufferPos;

      public int ReadByte()
      {
        if (this.bufferPos >= this.bufferLen)
        {
          this.bufferOffset += (long) this.bufferLen;
          this.bufferLen = this.readStream.Read(this.readBuffer, 0, this.readBuffer.Length);
          this.bufferPos = 0;
          if (this.bufferLen == 0)
            return -1;
        }
        return (int) this.readBuffer[this.bufferPos++];
      }
    }
  }
}

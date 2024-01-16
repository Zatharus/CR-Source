// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.JpegFile
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.IO;
using System;
using System.Drawing;
using System.IO;

#nullable disable
namespace cYo.Common.Drawing
{
  public class JpegFile
  {
    public JpegFile(Stream s) => this.Initialize(s);

    public JpegFile(byte[] data)
    {
      if (data == null || data.Length < 2 || data[0] != byte.MaxValue || data[1] != (byte) 216)
        return;
      using (MemoryStream s = new MemoryStream(data))
        this.Initialize((Stream) s);
    }

    private void Initialize(Stream s)
    {
      Size size;
      if (!(this.IsValid = JpegFile.GetImageSize(s, out size)))
        return;
      this.Size = size;
    }

    public bool IsValid { get; private set; }

    public Size Size { get; private set; }

    public int Width => this.Size.Width;

    public int Height => this.Size.Height;

    public static bool GetImageSize(Stream s, out Size size)
    {
      size = Size.Empty;
      try
      {
        using (BinaryReader br = new BinaryReader(s))
        {
          if (br.ReadByte() != byte.MaxValue || br.ReadByte() != (byte) 216)
            return false;
          while (true)
          {
            byte num;
            do
              ;
            while ((num = br.ReadByte()) != byte.MaxValue);
            while (true)
            {
              switch (num)
              {
                case 192:
                case 193:
                case 194:
                case 195:
                  goto label_7;
                case byte.MaxValue:
                  num = br.ReadByte();
                  continue;
                default:
                  goto label_8;
              }
            }
label_8:
            br.BaseStream.Seek((long) (br.ReadUInt16BigEndian() - 2), SeekOrigin.Current);
          }
label_7:
          br.BaseStream.Seek(3L, SeekOrigin.Current);
          int height = br.ReadUInt16BigEndian();
          int width = br.ReadUInt16BigEndian();
          size = new Size(width, height);
        }
        return !size.IsEmpty;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool RemoveExif(Stream inStream, Stream outStream)
    {
      byte[] buffer = new byte[2];
      inStream.Read(buffer, 0, buffer.Length);
      if (buffer[0] != byte.MaxValue || buffer[1] != (byte) 216)
        return false;
      outStream.WriteByte(byte.MaxValue);
      outStream.WriteByte((byte) 216);
      for (inStream.Read(buffer, 0, buffer.Length); buffer[0] == byte.MaxValue && buffer[1] >= (byte) 224 && buffer[1] <= (byte) 239; inStream.Read(buffer, 0, buffer.Length))
      {
        int num = inStream.ReadByte() << 8 | inStream.ReadByte();
        inStream.Position += (long) (num - 2);
      }
      inStream.Position -= 2L;
      inStream.CopyTo(outStream);
      return true;
    }
  }
}

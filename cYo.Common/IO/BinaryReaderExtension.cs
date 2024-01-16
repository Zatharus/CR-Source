// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.BinaryReaderExtension
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public static class BinaryReaderExtension
  {
    public static int ReadUInt16BigEndian(this BinaryReader br)
    {
      return (int) (ushort) ((uint) br.ReadByte() << 8) | (int) br.ReadByte();
    }
  }
}

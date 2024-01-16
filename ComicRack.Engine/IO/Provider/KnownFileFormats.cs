// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.KnownFileFormats
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public static class KnownFileFormats
  {
    public const int PDF = 1;
    public const int CBZ = 2;
    public const int CBR = 3;
    public const int XML = 4;
    public const int CBT = 5;
    public const int CB7 = 6;
    public const int CBW = 7;
    public const int DJVU = 8;
    public const int FOLDER = 100;

    public static IEnumerable<byte> GetSignature(int format)
    {
      switch (format)
      {
        case 2:
          return (IEnumerable<byte>) new byte[2]
          {
            (byte) 80,
            (byte) 75
          };
        case 3:
          return (IEnumerable<byte>) new byte[4]
          {
            (byte) 82,
            (byte) 97,
            (byte) 114,
            (byte) 33
          };
        case 6:
          return (IEnumerable<byte>) new byte[2]
          {
            (byte) 55,
            (byte) 122
          };
        default:
          return (IEnumerable<byte>) null;
      }
    }
  }
}

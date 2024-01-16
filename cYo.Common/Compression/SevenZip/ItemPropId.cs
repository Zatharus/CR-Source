// Decompiled with JetBrains decompiler
// Type: cYo.Common.Compression.SevenZip.ItemPropId
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

#nullable disable
namespace cYo.Common.Compression.SevenZip
{
  public enum ItemPropId
  {
    kpidNoProperty = 0,
    kpidHandlerItemIndex = 2,
    kpidPath = 3,
    kpidName = 4,
    kpidExtension = 5,
    kpidIsFolder = 6,
    kpidSize = 7,
    kpidPackedSize = 8,
    kpidAttributes = 9,
    kpidCreationTime = 10, // 0x0000000A
    kpidLastAccessTime = 11, // 0x0000000B
    kpidLastWriteTime = 12, // 0x0000000C
    kpidSolid = 13, // 0x0000000D
    kpidCommented = 14, // 0x0000000E
    kpidEncrypted = 15, // 0x0000000F
    kpidSplitBefore = 16, // 0x00000010
    kpidSplitAfter = 17, // 0x00000011
    kpidDictionarySize = 18, // 0x00000012
    kpidCRC = 19, // 0x00000013
    kpidType = 20, // 0x00000014
    kpidIsAnti = 21, // 0x00000015
    kpidMethod = 22, // 0x00000016
    kpidHostOS = 23, // 0x00000017
    kpidFileSystem = 24, // 0x00000018
    kpidUser = 25, // 0x00000019
    kpidGroup = 26, // 0x0000001A
    kpidBlock = 27, // 0x0000001B
    kpidComment = 28, // 0x0000001C
    kpidPosition = 29, // 0x0000001D
    kpidPrefix = 30, // 0x0000001E
    kpidNumSubFolders = 31, // 0x0000001F
    kpidNumSubFiles = 32, // 0x00000020
    kpidUnpackVer = 33, // 0x00000021
    kpidVolume = 34, // 0x00000022
    kpidIsVolume = 35, // 0x00000023
    kpidOffset = 36, // 0x00000024
    kpidLinks = 37, // 0x00000025
    kpidNumBlocks = 38, // 0x00000026
    kpidNumVolumes = 39, // 0x00000027
    kpidTimeType = 40, // 0x00000028
    kpidBit64 = 41, // 0x00000029
    kpidBigEndian = 42, // 0x0000002A
    kpidCpu = 43, // 0x0000002B
    kpidPhySize = 44, // 0x0000002C
    kpidHeadersSize = 45, // 0x0000002D
    kpidChecksum = 46, // 0x0000002E
    kpidCharacts = 47, // 0x0000002F
    kpidVa = 48, // 0x00000030
    kpidTotalSize = 4352, // 0x00001100
    kpidFreeSpace = 4353, // 0x00001101
    kpidClusterSize = 4354, // 0x00001102
    kpidVolumeName = 4355, // 0x00001103
    kpidLocalName = 4608, // 0x00001200
    kpidProvider = 4609, // 0x00001201
    kpidUserDefined = 65536, // 0x00010000
  }
}

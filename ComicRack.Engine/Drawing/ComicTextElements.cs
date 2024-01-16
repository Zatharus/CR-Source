// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ComicTextElements
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  [Flags]
  public enum ComicTextElements
  {
    None = 0,
    Caption = 1,
    [Description("Caption without Title")] CaptionWithoutTitle = 2,
    AlternateCaption = 4,
    Title = 8,
    [Description("Artists")] ArtistInfo = 16, // 0x00000010
    Summary = 32, // 0x00000020
    FileSize = 64, // 0x00000040
    Opened = 128, // 0x00000080
    FileName = 256, // 0x00000100
    FileFormat = 512, // 0x00000200
    Added = 1024, // 0x00000400
    [Description("Publisher and Imprint")] PublisherAndImprint = 2048, // 0x00000800
    AgeRating = 4096, // 0x00001000
    Genre = 8192, // 0x00002000
    [Description("Characters and Teams")] CharactersAndTeams = 16384, // 0x00004000
    Locations = 32768, // 0x00008000
    Notes = 65536, // 0x00010000
    PurchaseInformation = 131072, // 0x00020000
    StorageInfoformation = 262144, // 0x00040000
    CollectionStatus = 524288, // 0x00080000
    ScanInformation = 1048576, // 0x00100000
    Released = 2097152, // 0x00200000
    StackTitle = 16777216, // 0x01000000
    StackBookCount = 33554432, // 0x02000000
    StackBooksOpened = 67108864, // 0x04000000
    NoEmptyDates = 268435456, // 0x10000000
    DefaultComic = FileFormat | FileSize | Summary | ArtistInfo | Title | CaptionWithoutTitle, // 0x0000027A
    DefaultFileComic = DefaultComic | FileName, // 0x0000037A
    AllComic = DefaultFileComic | Released | ScanInformation | CollectionStatus | StorageInfoformation | PurchaseInformation | Notes | Locations | CharactersAndTeams | Genre | AgeRating | PublisherAndImprint | Added | Opened | AlternateCaption | Caption, // 0x003FFFFF
    DefaultStack = StackBooksOpened | StackBookCount | StackTitle | FileSize | ArtistInfo, // 0x07000050
    LinkedElements = FileFormat | FileName | FileSize, // 0x00000340
    DefaultPage = 134217728, // 0x08000000
  }
}

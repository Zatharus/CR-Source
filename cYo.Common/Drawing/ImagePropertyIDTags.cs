﻿// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ImagePropertyIDTags
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

#nullable disable
namespace cYo.Common.Drawing
{
  public enum ImagePropertyIDTags
  {
    GpsVer = 0,
    GpsLatitudeRef = 1,
    GpsLatitude = 2,
    GpsLongitudeRef = 3,
    GpsLongitude = 4,
    GpsAltitudeRef = 5,
    GpsAltitude = 6,
    GpsGpsTime = 7,
    GpsGpsSatellites = 8,
    GpsGpsStatus = 9,
    GpsGpsMeasureMode = 10, // 0x0000000A
    GpsGpsDop = 11, // 0x0000000B
    GpsSpeedRef = 12, // 0x0000000C
    GpsSpeed = 13, // 0x0000000D
    GpsTrackRef = 14, // 0x0000000E
    GpsTrack = 15, // 0x0000000F
    GpsImgDirRef = 16, // 0x00000010
    GpsImgDir = 17, // 0x00000011
    GpsMapDatum = 18, // 0x00000012
    GpsDestLatRef = 19, // 0x00000013
    GpsDestLat = 20, // 0x00000014
    GpsDestLongRef = 21, // 0x00000015
    GpsDestLong = 22, // 0x00000016
    GpsDestBearRef = 23, // 0x00000017
    GpsDestBear = 24, // 0x00000018
    GpsDestDistRef = 25, // 0x00000019
    GpsDestDist = 26, // 0x0000001A
    NewSubfileType = 254, // 0x000000FE
    SubfileType = 255, // 0x000000FF
    ImageWidth = 256, // 0x00000100
    ImageHeight = 257, // 0x00000101
    BitsPerSample = 258, // 0x00000102
    Compression = 259, // 0x00000103
    PhotometricInterp = 262, // 0x00000106
    ThreshHolding = 263, // 0x00000107
    CellWidth = 264, // 0x00000108
    CellHeight = 265, // 0x00000109
    FillOrder = 266, // 0x0000010A
    DocumentName = 269, // 0x0000010D
    ImageDescription = 270, // 0x0000010E
    EquipMake = 271, // 0x0000010F
    EquipModel = 272, // 0x00000110
    StripOffsets = 273, // 0x00000111
    Orientation = 274, // 0x00000112
    SamplesPerPixel = 277, // 0x00000115
    RowsPerStrip = 278, // 0x00000116
    StripBytesCount = 279, // 0x00000117
    MinSampleValue = 280, // 0x00000118
    MaxSampleValue = 281, // 0x00000119
    XResolution = 282, // 0x0000011A
    YResolution = 283, // 0x0000011B
    PlanarConfig = 284, // 0x0000011C
    PageName = 285, // 0x0000011D
    XPosition = 286, // 0x0000011E
    YPosition = 287, // 0x0000011F
    FreeOffset = 288, // 0x00000120
    FreeByteCounts = 289, // 0x00000121
    GrayResponseUnit = 290, // 0x00000122
    GrayResponseCurve = 291, // 0x00000123
    T4Option = 292, // 0x00000124
    T6Option = 293, // 0x00000125
    ResolutionUnit = 296, // 0x00000128
    PageNumber = 297, // 0x00000129
    TransferFuncition = 301, // 0x0000012D
    SoftwareUsed = 305, // 0x00000131
    DateTime = 306, // 0x00000132
    Artist = 315, // 0x0000013B
    HostComputer = 316, // 0x0000013C
    Predictor = 317, // 0x0000013D
    WhitePoint = 318, // 0x0000013E
    PrimaryChromaticities = 319, // 0x0000013F
    ColorMap = 320, // 0x00000140
    HalftoneHints = 321, // 0x00000141
    TileWidth = 322, // 0x00000142
    TileLength = 323, // 0x00000143
    TileOffset = 324, // 0x00000144
    TileByteCounts = 325, // 0x00000145
    InkSet = 332, // 0x0000014C
    InkNames = 333, // 0x0000014D
    NumberOfInks = 334, // 0x0000014E
    DotRange = 336, // 0x00000150
    TargetPrinter = 337, // 0x00000151
    ExtraSamples = 338, // 0x00000152
    SampleFormat = 339, // 0x00000153
    SMinSampleValue = 340, // 0x00000154
    SMaxSampleValue = 341, // 0x00000155
    TransferRange = 342, // 0x00000156
    JPEGProc = 512, // 0x00000200
    JPEGInterFormat = 513, // 0x00000201
    JPEGInterLength = 514, // 0x00000202
    JPEGRestartInterval = 515, // 0x00000203
    JPEGLosslessPredictors = 517, // 0x00000205
    JPEGPointTransforms = 518, // 0x00000206
    JPEGQTables = 519, // 0x00000207
    JPEGDCTables = 520, // 0x00000208
    JPEGACTables = 521, // 0x00000209
    YCbCrCoefficients = 529, // 0x00000211
    YCbCrSubsampling = 530, // 0x00000212
    YCbCrPositioning = 531, // 0x00000213
    REFBlackWhite = 532, // 0x00000214
    Gamma = 769, // 0x00000301
    ICCProfileDescriptor = 770, // 0x00000302
    SRGBRenderingIntent = 771, // 0x00000303
    ImageTitle = 800, // 0x00000320
    ResolutionXUnit = 20481, // 0x00005001
    ResolutionYUnit = 20482, // 0x00005002
    ResolutionXLengthUnit = 20483, // 0x00005003
    ResolutionYLengthUnit = 20484, // 0x00005004
    PrintFlags = 20485, // 0x00005005
    PrintFlagsVersion = 20486, // 0x00005006
    PrintFlagsCrop = 20487, // 0x00005007
    PrintFlagsBleedWidth = 20488, // 0x00005008
    PrintFlagsBleedWidthScale = 20489, // 0x00005009
    HalftoneLPI = 20490, // 0x0000500A
    HalftoneLPIUnit = 20491, // 0x0000500B
    HalftoneDegree = 20492, // 0x0000500C
    HalftoneShape = 20493, // 0x0000500D
    HalftoneMisc = 20494, // 0x0000500E
    HalftoneScreen = 20495, // 0x0000500F
    JPEGQuality = 20496, // 0x00005010
    GridSize = 20497, // 0x00005011
    ThumbnailFormat = 20498, // 0x00005012
    ThumbnailWidth = 20499, // 0x00005013
    ThumbnailHeight = 20500, // 0x00005014
    ThumbnailColorDepth = 20501, // 0x00005015
    ThumbnailPlanes = 20502, // 0x00005016
    ThumbnailRawBytes = 20503, // 0x00005017
    ThumbnailSize = 20504, // 0x00005018
    ThumbnailCompressedSize = 20505, // 0x00005019
    ColorTransferFunction = 20506, // 0x0000501A
    ThumbnailData = 20507, // 0x0000501B
    ThumbnailImageWidth = 20512, // 0x00005020
    ThumbnailImageHeight = 20513, // 0x00005021
    ThumbnailBitsPerSample = 20514, // 0x00005022
    ThumbnailCompression = 20515, // 0x00005023
    ThumbnailPhotometricInterp = 20516, // 0x00005024
    ThumbnailImageDescription = 20517, // 0x00005025
    ThumbnailEquipMake = 20518, // 0x00005026
    ThumbnailEquipModel = 20519, // 0x00005027
    ThumbnailStripOffsets = 20520, // 0x00005028
    ThumbnailOrientation = 20521, // 0x00005029
    ThumbnailSamplesPerPixel = 20522, // 0x0000502A
    ThumbnailRowsPerStrip = 20523, // 0x0000502B
    ThumbnailStripBytesCount = 20524, // 0x0000502C
    ThumbnailResolutionX = 20525, // 0x0000502D
    ThumbnailResolutionY = 20526, // 0x0000502E
    ThumbnailPlanarConfig = 20527, // 0x0000502F
    ThumbnailResolutionUnit = 20528, // 0x00005030
    ThumbnailTransferFunction = 20529, // 0x00005031
    ThumbnailSoftwareUsed = 20530, // 0x00005032
    ThumbnailDateTime = 20531, // 0x00005033
    ThumbnailArtist = 20532, // 0x00005034
    ThumbnailWhitePoint = 20533, // 0x00005035
    ThumbnailPrimaryChromaticities = 20534, // 0x00005036
    ThumbnailYCbCrCoefficients = 20535, // 0x00005037
    ThumbnailYCbCrSubsampling = 20536, // 0x00005038
    ThumbnailYCbCrPositioning = 20537, // 0x00005039
    ThumbnailRefBlackWhite = 20538, // 0x0000503A
    ThumbnailCopyRight = 20539, // 0x0000503B
    LuminanceTable = 20624, // 0x00005090
    ChrominanceTable = 20625, // 0x00005091
    FrameDelay = 20736, // 0x00005100
    LoopCount = 20737, // 0x00005101
    PixelUnit = 20752, // 0x00005110
    PixelPerUnitX = 20753, // 0x00005111
    PixelPerUnitY = 20754, // 0x00005112
    PaletteHistogram = 20755, // 0x00005113
    Copyright = 33432, // 0x00008298
    ExifExposureTime = 33434, // 0x0000829A
    ExifFNumber = 33437, // 0x0000829D
    ExifIFD = 34665, // 0x00008769
    ICCProfile = 34675, // 0x00008773
    ExifExposureProg = 34850, // 0x00008822
    ExifSpectralSense = 34852, // 0x00008824
    GpsIFD = 34853, // 0x00008825
    ExifISOSpeed = 34855, // 0x00008827
    ExifOECF = 34856, // 0x00008828
    ExifVer = 36864, // 0x00009000
    ExifDTOrig = 36867, // 0x00009003
    ExifDTDigitized = 36868, // 0x00009004
    ExifCompConfig = 37121, // 0x00009101
    ExifCompBPP = 37122, // 0x00009102
    ExifShutterSpeed = 37377, // 0x00009201
    ExifAperture = 37378, // 0x00009202
    ExifBrightness = 37379, // 0x00009203
    ExifExposureBias = 37380, // 0x00009204
    ExifMaxAperture = 37381, // 0x00009205
    ExifSubjectDist = 37382, // 0x00009206
    ExifMeteringMode = 37383, // 0x00009207
    ExifLightSource = 37384, // 0x00009208
    ExifFlash = 37385, // 0x00009209
    ExifFocalLength = 37386, // 0x0000920A
    ExifMakerNote = 37500, // 0x0000927C
    ExifUserComment = 37510, // 0x00009286
    ExifDTSubsec = 37520, // 0x00009290
    ExifDTOrigSS = 37521, // 0x00009291
    ExifDTDigSS = 37522, // 0x00009292
    FileExplorerTitle = 40091, // 0x00009C9B
    FileExplorerComments = 40092, // 0x00009C9C
    FileExplorerKeywords = 40094, // 0x00009C9E
    FileExplorerSubject = 40095, // 0x00009C9F
    ExifFPXVer = 40960, // 0x0000A000
    ExifColorSpace = 40961, // 0x0000A001
    ExifPixXDim = 40962, // 0x0000A002
    ExifPixYDim = 40963, // 0x0000A003
    ExifRelatedWav = 40964, // 0x0000A004
    ExifInterop = 40965, // 0x0000A005
    ExifFlashEnergy = 41483, // 0x0000A20B
    ExifSpatialFR = 41484, // 0x0000A20C
    ExifFocalXRes = 41486, // 0x0000A20E
    ExifFocalYRes = 41487, // 0x0000A20F
    ExifFocalResUnit = 41488, // 0x0000A210
    ExifSubjectLoc = 41492, // 0x0000A214
    ExifExposureIndex = 41493, // 0x0000A215
    ExifSensingMethod = 41495, // 0x0000A217
    ExifFileSource = 41728, // 0x0000A300
    ExifSceneType = 41729, // 0x0000A301
    ExifCfaPattern = 41730, // 0x0000A302
  }
}
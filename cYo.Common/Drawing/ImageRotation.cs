// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ImageRotation
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.ComponentModel;

#nullable disable
namespace cYo.Common.Drawing
{
  public enum ImageRotation : byte
  {
    None,
    [Description("90°")] Rotate90,
    [Description("180°")] Rotate180,
    [Description("270°")] Rotate270,
  }
}

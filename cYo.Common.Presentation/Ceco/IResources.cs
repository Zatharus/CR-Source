// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.IResources
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Drawing;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public interface IResources
  {
    Font GetFont(string fontFamily, float fontSize, FontStyle fontStyle);

    Image GetImage(string source);
  }
}

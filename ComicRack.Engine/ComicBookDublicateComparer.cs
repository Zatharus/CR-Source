// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookDublicateComparer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookDublicateComparer : Comparer<ComicBook>
  {
    public override int Compare(ComicBook x, ComicBook y)
    {
      int num1 = string.Compare(GroupInfo.CompressedName(x.ShadowSeries), GroupInfo.CompressedName(y.ShadowSeries), true);
      if (num1 != 0)
        return num1;
      int num2 = string.Compare(x.ShadowFormat, y.ShadowFormat);
      if (num2 != 0)
        return num2;
      int num3 = x.ShadowVolume.CompareTo(y.ShadowVolume);
      if (num3 != 0)
        return num3;
      int num4 = string.Compare(x.ShadowNumber, y.ShadowNumber);
      if (num4 != 0)
        return num4;
      int num5 = string.Compare(x.LanguageISO, y.LanguageISO);
      if (num5 != 0)
        return num5;
      if (x.ShadowYear >= 0 || y.ShadowYear >= 0)
      {
        int num6 = x.ShadowYear.CompareTo(y.ShadowYear);
        if (num6 != 0)
          return num6;
      }
      if (x.Month >= 0 || y.Month >= 0)
      {
        int num7 = x.Month.CompareTo(y.Month);
        if (num7 != 0)
          return num7;
      }
      if (x.Day >= 0 || y.Day >= 0)
      {
        int num8 = x.Day.CompareTo(y.Day);
        if (num8 != 0)
          return num8;
      }
      return x.BlackAndWhite.CompareTo((object) y.BlackAndWhite);
    }
  }
}

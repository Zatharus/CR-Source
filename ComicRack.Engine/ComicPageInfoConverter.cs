// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicPageInfoConverter
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicPageInfoConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (string) || base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      if (!(value is string s))
        return base.ConvertFrom(context, culture, value);
      if (s == "Empty")
        return (object) ComicPageInfo.Empty;
      string[] array = ((IEnumerable<string>) s.Split(culture.TextInfo.ListSeparator, StringSplitOptions.None)).TrimStrings().ToArray<string>();
      ComicPageInfo comicPageInfo = new ComicPageInfo(int.Parse(array[0]))
      {
        ImageWidth = int.Parse(array[1]),
        ImageHeight = int.Parse(array[2]),
        ImageFileSize = int.Parse(array[3]),
        Bookmark = array[7]
      };
      ComicPageType result1;
      if (Enum.TryParse<ComicPageType>(array[4], out result1))
        comicPageInfo.PageType = result1;
      ComicPagePosition result2;
      if (Enum.TryParse<ComicPagePosition>(array[5], out result2))
        comicPageInfo.PagePosition = result2;
      ImageRotation result3;
      if (Enum.TryParse<ImageRotation>(array[6], out result3))
        comicPageInfo.Rotation = result3;
      return (object) comicPageInfo;
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      if (!(destinationType == typeof (string)))
        return base.ConvertTo(context, culture, value, destinationType);
      ComicPageInfo comicPageInfo = (ComicPageInfo) value;
      return (object) string.Format((IFormatProvider) culture, "{1}{0} {2}{0} {3}{0} {4}{0} {5}{0} {6}{0} {7}{0} {8}{0}", (object) culture.TextInfo.ListSeparator, (object) comicPageInfo.ImageIndex, (object) comicPageInfo.ImageWidth, (object) comicPageInfo.ImageHeight, (object) comicPageInfo.ImageFileSize, (object) comicPageInfo.PageType, (object) comicPageInfo.PagePosition, (object) comicPageInfo.Rotation, (object) comicPageInfo.Bookmark);
    }
  }
}

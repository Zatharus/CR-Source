// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.BitmapAdjustmentConverter
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Text;
using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace cYo.Common.Drawing
{
  public class BitmapAdjustmentConverter : TypeConverter
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
        return (object) BitmapAdjustment.Empty;
      string[] strArray = s.Split(culture.TextInfo.ListSeparator, StringSplitOptions.RemoveEmptyEntries);
      return (object) new BitmapAdjustment(strArray.Length != 0 ? float.Parse(strArray[0], (IFormatProvider) culture) : 0.0f, strArray.Length > 1 ? float.Parse(strArray[1], (IFormatProvider) culture) : 0.0f, strArray.Length > 2 ? float.Parse(strArray[2], (IFormatProvider) culture) : 0.0f, strArray.Length > 3 ? float.Parse(strArray[3], (IFormatProvider) culture) : 0.0f);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      if (!(destinationType == typeof (string)))
        return base.ConvertTo(context, culture, value, destinationType);
      BitmapAdjustment bitmapAdjustment = (BitmapAdjustment) value;
      return (object) string.Format((IFormatProvider) culture, "{1}{0} {2}{0} {3}{0} {4}", (object) culture.TextInfo.ListSeparator, (object) bitmapAdjustment.Saturation, (object) bitmapAdjustment.Brightness, (object) bitmapAdjustment.Contrast, (object) bitmapAdjustment.Gamma);
    }
  }
}

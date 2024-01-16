// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Vector3Converter
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Text;
using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace cYo.Common.Mathematics
{
  public class Vector3Converter : ExpandableObjectConverter
  {
    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      if (!(destinationType == typeof (string)) || !(value is Vector3 vector3))
        return base.ConvertTo(context, culture, value, destinationType);
      string str = culture.TextInfo.ListSeparator + " ";
      string[] strArray = new string[5];
      float num = vector3.X;
      strArray[0] = num.ToString((IFormatProvider) culture);
      strArray[1] = str;
      num = vector3.Y;
      strArray[2] = num.ToString((IFormatProvider) culture);
      strArray[3] = str;
      num = vector3.Z;
      strArray[4] = num.ToString((IFormatProvider) culture);
      return (object) string.Concat(strArray);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      if (!(value.GetType() == typeof (string)))
        return base.ConvertFrom(context, culture, value);
      try
      {
        string[] strArray = ((string) value).Split(culture.TextInfo.ListSeparator, StringSplitOptions.None);
        return (object) new Vector3(float.Parse(strArray[0], (IFormatProvider) culture), float.Parse(strArray[1], (IFormatProvider) culture), float.Parse(strArray[2], (IFormatProvider) culture));
      }
      catch
      {
        throw new ArgumentException("Invalid value: " + value);
      }
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
    }
  }
}

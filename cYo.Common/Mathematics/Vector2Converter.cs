// Decompiled with JetBrains decompiler
// Type: cYo.Common.Mathematics.Vector2Converter
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace cYo.Common.Mathematics
{
  public class Vector2Converter : ExpandableObjectConverter
  {
    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return destinationType == typeof (string) && value is Vector2 vector2 ? (object) (vector2.X.ToString((IFormatProvider) culture) + "; " + vector2.Y.ToString((IFormatProvider) culture)) : base.ConvertTo(context, culture, value, destinationType);
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
        string[] strArray = ((string) value).Split(';');
        return (object) new Vector2(float.Parse(strArray[0], (IFormatProvider) culture), float.Parse(strArray[1], (IFormatProvider) culture));
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

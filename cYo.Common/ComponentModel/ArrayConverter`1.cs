// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.ArrayConverter`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class ArrayConverter<T> : TypeConverter
  {
    private readonly TypeConverter tc = TypeDescriptor.GetConverter(typeof (T));

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string) && this.tc.CanConvertFrom(context, sourceType) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (string) && this.tc.CanConvertTo(context, destinationType) || base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      return value is string s ? (object) ((IEnumerable<string>) s.Split(culture.TextInfo.ListSeparator, StringSplitOptions.RemoveEmptyEntries)).Select<string, T>((Func<string, T>) (x => (T) this.tc.ConvertFrom(context, culture, (object) x))).ToArray<T>() : base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      IEnumerable<T> objs = value as IEnumerable<T>;
      if (!(destinationType == typeof (string)) || objs == null)
        return base.ConvertTo(context, culture, value, destinationType);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (T obj in objs)
      {
        if (stringBuilder.Length != 0)
        {
          stringBuilder.Append(culture.TextInfo.ListSeparator);
          stringBuilder.Append(" ");
        }
        stringBuilder.Append(this.tc.ConvertTo(context, culture, (object) obj, destinationType));
      }
      return (object) stringBuilder.ToString();
    }
  }
}

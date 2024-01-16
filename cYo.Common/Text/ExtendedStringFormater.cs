// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.ExtendedStringFormater
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace cYo.Common.Text
{
  public static class ExtendedStringFormater
  {
    public static string Format(string format, Func<string, object> getValue)
    {
      return ExtendedStringFormater.Format(format, getValue, out bool _);
    }

    public static string Format(string format, IDictionary<string, object> values)
    {
      return ExtendedStringFormater.Format(format, (Func<string, object>) (s => ExtendedStringFormater.GetValue(values, s)));
    }

    private static object GetValue(IDictionary<string, object> values, string key)
    {
      object obj;
      return !values.TryGetValue(key, out obj) ? (object) null : obj;
    }

    private static string Format(string format, Func<string, object> getValue, out bool success)
    {
      StringBuilder stringBuilder = new StringBuilder();
      success = true;
      for (int index = 0; index < format.Length; ++index)
      {
        char ch = format[index];
        switch (ch)
        {
          case '[':
            bool success1;
            string str1 = ExtendedStringFormater.Format(ExtendedStringFormater.GetPart(format, ref index, '[', ']'), getValue, out success1);
            if (success1)
              stringBuilder.Append(str1);
            success |= success1;
            break;
          case '\\':
            stringBuilder.Append(format[++index]);
            break;
          case '{':
            string str2 = ExtendedStringFormater.FormatValue(ExtendedStringFormater.GetPart(format, ref index, '{', '}'), getValue);
            success &= !string.IsNullOrEmpty(str2);
            stringBuilder.Append(str2);
            break;
          default:
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    private static string FormatValue(string format, Func<string, object> getValue)
    {
      string[] strArray = format.Split(':');
      object obj = getValue(strArray[0]);
      if (obj == null)
        return string.Empty;
      if (strArray.Length == 1)
        return obj.ToString();
      try
      {
        obj = (object) Convert.ToDouble(obj);
      }
      catch
      {
      }
      try
      {
        return string.Format("{0:" + strArray[1] + "}", obj);
      }
      catch (Exception ex)
      {
        return obj.ToString();
      }
    }

    private static string GetPart(string text, ref int index, char openChar, char closeChar)
    {
      int startIndex = 0;
      int num1 = 0;
      int num2 = 0;
      bool flag = (int) openChar == (int) closeChar;
      while (index < text.Length)
      {
        char ch = text[index];
        if ((int) ch == (int) openChar && (!flag || num2 <= 0))
        {
          if (num2++ == 0)
            startIndex = index + 1;
        }
        else if ((int) ch == (int) closeChar && --num2 == 0)
        {
          num1 = index;
          break;
        }
        ++index;
      }
      return text.Substring(startIndex, num1 - startIndex);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Text.FileLengthFormat
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Text
{
  public class FileLengthFormat : IFormatProvider, ICustomFormatter
  {
    public object GetFormat(Type formatType) => (object) this;

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
      long num;
      try
      {
        num = (long) arg;
      }
      catch (Exception ex)
      {
        throw new ArgumentException(string.Format("The argument \"{0}\" cannot be converted to an integer value.", arg), ex);
      }
      if (num < 1024L)
        return string.Format("{0} Bytes", (object) num);
      if (num < 1048576L)
        return string.Format("{0:.00} kB", (object) (float) ((double) num / 1024.0));
      return num < 1073741824L ? string.Format("{0:.00} MB", (object) (float) ((double) num / 1024.0 / 1024.0)) : string.Format("{0:.00} GB", (object) (float) ((double) num / 1024.0 / 1024.0 / 1024.0));
    }
  }
}

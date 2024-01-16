// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.FileFormatExtensions
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public static class FileFormatExtensions
  {
    public static string GetDialogFilter(this IEnumerable<FileFormat> formats, bool withAllFilter)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = withAllFilter ? new StringBuilder() : (StringBuilder) null;
      foreach (FileFormat format in formats)
      {
        if (format != null)
        {
          if (stringBuilder1.Length != 0)
            stringBuilder1.Append("|");
          stringBuilder1.Append(format.Name);
          stringBuilder1.Append("|");
          stringBuilder1.Append(format.ExtensionFilter);
          if (stringBuilder2 != null)
          {
            if (stringBuilder2.Length != 0)
              stringBuilder2.Append(";");
            stringBuilder2.Append(format.ExtensionFilter);
          }
        }
      }
      if (stringBuilder2 != null)
      {
        stringBuilder1.Append("|");
        stringBuilder1.Append(TR.Load("FileFilter")["AllSupportedFiles", "All supported Files"]);
        stringBuilder1.Append("|");
        stringBuilder1.Append((object) stringBuilder2);
        stringBuilder1.Append("|");
        stringBuilder1.Append(TR.Load("FileFilter")["AllFiles", "All Files"]);
        stringBuilder1.Append("|*.*");
      }
      return stringBuilder1.ToString();
    }

    public static IEnumerable<string> GetExtensions(this IEnumerable<FileFormat> formats)
    {
      return formats.SelectMany<FileFormat, string>((Func<FileFormat, IEnumerable<string>>) (ff => ff.Extensions));
    }
  }
}

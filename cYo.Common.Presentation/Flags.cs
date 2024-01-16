// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Flags
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Presentation.Properties;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Common.Presentation
{
  public static class Flags
  {
    private static readonly Dictionary<string, Image> flagDict = new Dictionary<string, Image>();
    private static readonly bool available;

    static Flags()
    {
      try
      {
        using (MemoryStream memoryStream = new MemoryStream(Resources.Flags))
        {
          using (ZipFile zipFile = new ZipFile((Stream) memoryStream))
          {
            foreach (ZipEntry entry in zipFile)
            {
              if (entry.IsFile)
              {
                using (Stream inputStream = zipFile.GetInputStream(entry))
                  Flags.flagDict[Path.GetFileNameWithoutExtension(entry.Name)] = Image.FromStream(inputStream, false, false);
              }
            }
          }
        }
        Flags.available = true;
      }
      catch (Exception ex)
      {
      }
    }

    public static Image GetFlagFromCountry(string countryCode)
    {
      Image image;
      return !Flags.available || countryCode == null || !Flags.flagDict.TryGetValue(countryCode.ToLower(), out image) ? (Image) null : image.Clone() as Image;
    }

    public static Image GetFlagFromCulture(string cultureCode)
    {
      if (!Flags.available)
        return (Image) null;
      if (cultureCode == null)
        cultureCode = CultureInfo.CurrentUICulture.Name;
      string[] strArray = cultureCode.Split('-');
      Image flagFromCountry = Flags.GetFlagFromCountry(strArray[strArray.Length - 1]);
      return flagFromCountry != null || strArray.Length == 2 ? flagFromCountry : ((IEnumerable<CultureInfo>) CultureInfo.GetCultures(CultureTypes.SpecificCultures)).Where<CultureInfo>((Func<CultureInfo, bool>) (ci => ci.Name.StartsWith(cultureCode))).Select<CultureInfo, Image>((Func<CultureInfo, Image>) (ci => Flags.GetFlagFromCulture(ci.Name))).FirstOrDefault<Image>((Func<Image, bool>) (ci => ci != null));
    }

    public static Image GetFlag(CultureInfo ci) => Flags.GetFlagFromCulture(ci.Name);

    public static bool Available => Flags.available;
  }
}

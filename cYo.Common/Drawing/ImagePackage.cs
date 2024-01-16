// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.ImagePackage
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Runtime;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Common.Drawing
{
  public class ImagePackage : DisposableObject, IImagePackage
  {
    private const string MapFile = "map.ini";
    private readonly Dictionary<string, ImagePackage.ImageItem> imageDict;

    public ImagePackage(IVirtualFolder package = null, bool caseSensitive = false)
    {
      this.imageDict = !caseSensitive ? new Dictionary<string, ImagePackage.ImageItem>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, ImagePackage.ImageItem>();
      if (package == null)
        return;
      this.Add(package);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (ImagePackage.ImageItem imageItem in this.imageDict.Values)
          imageItem.Image.SafeDispose();
      }
      base.Dispose(disposing);
    }

    public bool EnableWidthCropping { get; set; }

    public bool EnableHeightCropping { get; set; }

    public IEnumerable<string> Keys => (IEnumerable<string>) this.imageDict.Keys;

    private void AddImage(
      IVirtualFolder package,
      string key,
      string value,
      Func<string, IEnumerable<string>> mapKeys)
    {
      ImagePackage.ImageItem imageItem = new ImagePackage.ImageItem()
      {
        File = key,
        Package = package
      };
      foreach (string key1 in mapKeys(value).TrimStrings().RemoveEmpty())
        this.imageDict[key1] = imageItem;
    }

    public void Add(IVirtualFolder package, Func<string, IEnumerable<string>> mapKeys = null)
    {
      if (package == null)
        return;
      if (mapKeys == null)
        mapKeys = (Func<string, IEnumerable<string>>) (s => ListExtensions.AsEnumerable<string>(s));
      if (package.FileExists("map.ini"))
      {
        using (Stream stream = package.OpenRead("map.ini"))
        {
          using (StreamReader tr = new StreamReader(stream))
          {
            foreach (KeyValuePair<string, string> keyValuePair in IniFile.GetValues((TextReader) tr))
              this.AddImage(package, keyValuePair.Key, keyValuePair.Value, mapKeys);
          }
        }
      }
      foreach (string str in package.GetFiles(string.Empty).Where<string>((Func<string, bool>) (f => string.Compare(Path.GetExtension(f), ".ini", true) != 0)))
        this.AddImage(package, str, Path.GetFileNameWithoutExtension(str), mapKeys);
    }

    public void AddRange(
      IEnumerable<IVirtualFolder> packages,
      Func<string, IEnumerable<string>> mapKeys = null)
    {
      packages.SafeForEach<IVirtualFolder>((Action<IVirtualFolder>) (p => this.Add(p, mapKeys)));
    }

    public bool ImageExists(string key)
    {
      return this.imageDict.TryGetValue(key, out ImagePackage.ImageItem _);
    }

    public bool ImageLoaded(string key)
    {
      ImagePackage.ImageItem imageItem;
      return this.imageDict.TryGetValue(key, out imageItem) && imageItem.Image != null;
    }

    public Image GetImage(string key)
    {
      ImagePackage.ImageItem imageItem;
      if (!this.imageDict.TryGetValue(key, out imageItem))
        return (Image) null;
      if (imageItem.Image == null && imageItem.Package != null)
      {
        using (Stream stream = imageItem.Package.OpenRead(imageItem.File))
        {
          try
          {
            imageItem.Image = (Image) Image.FromStream(stream).CreateCopy();
            if (!this.EnableWidthCropping)
            {
              if (!this.EnableHeightCropping)
                goto label_12;
            }
            Bitmap bitmap = ((Bitmap) imageItem.Image).CropTransparent(this.EnableWidthCropping, this.EnableHeightCropping);
            if (bitmap != imageItem.Image)
            {
              imageItem.Image.Dispose();
              imageItem.Image = (Image) bitmap;
            }
          }
          catch
          {
            imageItem.Package = (IVirtualFolder) null;
          }
        }
      }
label_12:
      return imageItem.Image;
    }

    private class ImageItem
    {
      public IVirtualFolder Package { get; set; }

      public string File { get; set; }

      public Image Image { get; set; }
    }
  }
}

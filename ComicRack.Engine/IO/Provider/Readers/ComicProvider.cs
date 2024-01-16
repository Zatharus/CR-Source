// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.ComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.IO;
using cYo.Common.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  public abstract class ComicProvider : ImageProvider, IInfoStorage
  {
    private static readonly string[] supportedTypes = new string[11]
    {
      "jpg",
      "jpeg",
      "jif",
      "jiff",
      "gif",
      "png",
      "tif",
      "tiff",
      "bmp",
      "djvu",
      "webp"
    };

    public bool UpdateEnabled
    {
      get
      {
        FileFormatAttribute fileFormatAttribute = ReflectionExtension.GetAttributes<FileFormatAttribute>(this.GetType(), true).FirstOrDefault<FileFormatAttribute>((Func<FileFormatAttribute, bool>) (f => f.Format.Supports(this.Source)));
        return fileFormatAttribute != null && fileFormatAttribute.EnableUpdate;
      }
    }

    protected bool DisableNtfs { get; set; }

    protected bool DisableSidecar { get; set; }

    public ComicInfo LoadInfo(InfoLoadingMethod method)
    {
      using (this.LockSource(true))
      {
        ComicInfo comicInfo = this.DisableNtfs ? (ComicInfo) null : NtfsInfoStorage.LoadInfo(this.Source);
        if (comicInfo == null && !this.DisableSidecar)
          comicInfo = ComicInfo.LoadFromSidecar(this.Source);
        return comicInfo != null && method == InfoLoadingMethod.Fast ? comicInfo : this.OnLoadInfo() ?? comicInfo;
      }
    }

    public bool StoreInfo(ComicInfo comicInfo)
    {
      bool flag = false;
      using (this.LockSource(false))
      {
        if (this.UpdateEnabled)
        {
          if (!this.OnStoreInfo(comicInfo))
            return false;
          flag = true;
        }
        if (!this.DisableNtfs)
          flag |= NtfsInfoStorage.StoreInfo(this.Source, comicInfo);
        return flag;
      }
    }

    protected virtual ComicInfo OnLoadInfo() => (ComicInfo) null;

    protected virtual bool OnStoreInfo(ComicInfo comicInfo) => false;

    protected virtual bool IsSupportedImage(string file)
    {
      string fileExt = Path.GetExtension(FileUtility.MakeValidFilename(file));
      return ((IEnumerable<string>) ComicProvider.supportedTypes).Any<string>((Func<string, bool>) (ext => string.Equals(fileExt, "." + ext, StringComparison.OrdinalIgnoreCase)));
    }
  }
}

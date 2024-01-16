// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.ThumbnailKey
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  [Serializable]
  public class ThumbnailKey : ImageKey
  {
    public const string ResourceKey = "resource";
    public const string FileKey = "file";
    public const string CustomKey = "custom";
    private static readonly Regex rxResource = new Regex("\\A\\s*(?<type>[a-z]{4,}):\\\\\\\\(?<resource>.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    [NonSerialized]
    private string resourceType;
    [NonSerialized]
    private string resourceLocation;

    public ThumbnailKey(
      object source,
      string location,
      long size,
      DateTime modified,
      int index,
      ImageRotation rotation)
      : base(source, location, size, modified, index, rotation)
    {
    }

    public ThumbnailKey(object source, string file, int index, ImageRotation rotation)
      : this(source, file, ImageKey.GetSafeSize(file), ImageKey.GetSafeModifiedTime(file), index, rotation)
    {
    }

    public ThumbnailKey(ImageKey key)
      : base(key)
    {
    }

    public string ResourceType
    {
      get
      {
        if (this.resourceType == null)
          this.CalcResource();
        return this.resourceType;
      }
    }

    public string ResourceLocation
    {
      get
      {
        if (this.resourceLocation == null)
          this.CalcResource();
        return this.resourceLocation;
      }
    }

    private void CalcResource()
    {
      Match match = ThumbnailKey.rxResource.Match(this.Location);
      if (match.Success)
      {
        this.resourceType = match.Groups["type"].Value;
        this.resourceLocation = match.Groups["resource"].Value;
      }
      else
      {
        this.resourceType = string.Empty;
        this.resourceLocation = this.Location;
      }
    }

    public static string GetResource(string key, string resource)
    {
      return string.Format("{0}:\\\\{1}", (object) key, (object) resource);
    }
  }
}

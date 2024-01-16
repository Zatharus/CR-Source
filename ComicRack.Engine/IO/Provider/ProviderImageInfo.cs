// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.ProviderImageInfo
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  [Serializable]
  public class ProviderImageInfo : IComparable<ProviderImageInfo>
  {
    public ProviderImageInfo()
    {
    }

    public ProviderImageInfo(int index) => this.Index = index;

    public ProviderImageInfo(int index, string name, long size)
    {
      this.Index = index;
      this.Name = name;
      this.Size = size;
    }

    public int Index { get; set; }

    public string Name { get; set; }

    public long Size { get; set; }

    public int CompareTo(ProviderImageInfo other)
    {
      return other == null ? 1 : string.Compare(this.Name, other.Name);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.ImageKey
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  [Serializable]
  public abstract class ImageKey
  {
    private readonly string location;
    private DateTime modified;
    private long size;
    private readonly int index;
    [NonSerialized]
    private WeakReference source;
    private readonly ImageRotation rotation;
    private int hashCode;

    protected ImageKey(
      object source,
      string location,
      long size,
      DateTime modified,
      int index,
      ImageRotation rotation)
    {
      this.Source = source;
      this.location = location;
      this.modified = modified;
      this.size = size;
      this.index = index;
      this.rotation = rotation;
    }

    protected ImageKey(ImageKey key)
      : this(key.Source, key.location, key.size, key.modified, key.index, key.rotation)
    {
    }

    public string Location => this.location;

    public DateTime Modified => this.modified;

    public long Size => this.size;

    public int Index => this.index;

    public object Source
    {
      get => this.source != null ? this.source.Target : (object) null;
      set => this.source = new WeakReference(value);
    }

    public ImageRotation Rotation => this.rotation;

    public void UpdateFileInfo()
    {
      this.modified = ImageKey.GetSafeModifiedTime(this.location);
      this.size = ImageKey.GetSafeSize(this.location);
      this.hashCode = 0;
    }

    public bool IsSameFile(string location, long size, DateTime modified)
    {
      return this.location == location && this.size == size && this.modified == modified;
    }

    protected virtual int CreateHashCode()
    {
      return this.location.GetHashCode() ^ this.index.GetHashCode() ^ this.modified.GetHashCode() ^ this.rotation.GetHashCode();
    }

    public override int GetHashCode()
    {
      if (this.hashCode == 0)
        this.hashCode = this.CreateHashCode();
      return this.hashCode;
    }

    public override bool Equals(object obj)
    {
      return obj is ImageKey imageKey && this.IsSameFile(imageKey.location, imageKey.size, imageKey.modified) && this.index == imageKey.index && this.rotation == imageKey.rotation;
    }

    public override string ToString()
    {
      return string.Format("[{0}:{1}]", (object) this.location, (object) this.index);
    }

    public static DateTime GetSafeModifiedTime(string file)
    {
      try
      {
        return File.GetLastWriteTimeUtc(file);
      }
      catch
      {
        return DateTime.MinValue;
      }
    }

    public static long GetSafeSize(string file)
    {
      try
      {
        return new FileInfo(file).Length;
      }
      catch
      {
        return 0;
      }
    }
  }
}

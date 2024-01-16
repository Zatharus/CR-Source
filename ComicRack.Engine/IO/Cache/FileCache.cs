// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Cache.FileCache
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.IO;
using System.IO;
using System.Text;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Cache
{
  public class FileCache : DiskCache<string, byte[]>
  {
    public FileCache(string path, int sizeMB)
      : base(path, sizeMB)
    {
    }

    protected override byte[] LoadItem(string file) => File.ReadAllBytes(file);

    protected override void StoreItem(string file, byte[] item) => File.WriteAllBytes(file, item);

    public bool AddText(string file, string text)
    {
      return this.AddItem(file, Encoding.UTF8.GetBytes(text));
    }

    public string GetText(string file)
    {
      byte[] bytes = this.GetItem(file);
      return bytes != null ? Encoding.UTF8.GetString(bytes) : (string) null;
    }

    public static FileCache Default { get; set; }
  }
}

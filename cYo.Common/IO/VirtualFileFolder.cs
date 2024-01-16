// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.VirtualFileFolder
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class VirtualFileFolder : IVirtualFolder
  {
    private readonly string basePath;

    public VirtualFileFolder(string basePath) => this.basePath = basePath;

    public VirtualFileFolder()
      : this(string.Empty)
    {
    }

    public string BasePath => this.basePath;

    public Stream OpenRead(string path)
    {
      return (Stream) File.OpenRead(Path.Combine(this.basePath, path));
    }

    public Stream Create(string path) => (Stream) File.Create(Path.Combine(this.basePath, path));

    public bool FileExists(string path) => File.Exists(Path.Combine(this.basePath, path));

    public bool CreateFolder(string path)
    {
      try
      {
        Directory.CreateDirectory(Path.Combine(this.basePath, path));
        return true;
      }
      catch
      {
        return false;
      }
    }

    public IEnumerable<string> GetFiles(string path)
    {
      return (IEnumerable<string>) Directory.GetFiles(Path.Combine(this.basePath, path));
    }
  }
}

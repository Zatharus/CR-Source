// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.FileKey
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class FileKey
  {
    public FileKey(string file)
    {
      FileInfo fileInfo = new FileInfo(file);
      this.File = file;
      this.Modified = fileInfo.LastWriteTimeUtc;
      this.Size = fileInfo.Length;
    }

    private string File { get; set; }

    private long Size { get; set; }

    private DateTime Modified { get; set; }

    public override bool Equals(object obj)
    {
      return obj is FileKey fileKey && this.File == fileKey.File && this.Size == fileKey.Size && this.Modified == fileKey.Modified;
    }

    public override int GetHashCode()
    {
      return this.File.GetHashCode() ^ this.Size.GetHashCode() ^ this.Modified.GetHashCode();
    }
  }
}

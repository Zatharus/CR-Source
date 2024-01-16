// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.AutoDeleteFileStream
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class AutoDeleteFileStream : FileStream
  {
    public AutoDeleteFileStream(string file)
      : base(file, FileMode.Open, FileAccess.Read, FileShare.Read)
    {
      this.File = file;
    }

    public string File { get; private set; }

    public override void Close()
    {
      base.Close();
      this.DeleteFile();
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      this.DeleteFile();
    }

    private void DeleteFile() => FileUtility.SafeDelete(this.File);
  }
}

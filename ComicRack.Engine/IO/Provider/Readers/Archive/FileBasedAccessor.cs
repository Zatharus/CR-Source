// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive.FileBasedAccessor
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive
{
  public abstract class FileBasedAccessor : IComicAccessor
  {
    private IEnumerable<byte> signature;

    public FileBasedAccessor(int format)
    {
      this.Format = format;
      this.signature = KnownFileFormats.GetSignature(format);
    }

    protected bool HasSignature => this.signature != null;

    public int Format { get; private set; }

    public abstract IEnumerable<ProviderImageInfo> GetEntryList(string source);

    public abstract byte[] ReadByteImage(string source, ProviderImageInfo info);

    public abstract ComicInfo ReadInfo(string source);

    public virtual bool WriteInfo(string source, ComicInfo info)
    {
      return SevenZipEngine.UpdateComicInfo(source, this.Format, info);
    }

    public virtual bool IsFormat(string source)
    {
      if (this.signature == null)
        return true;
      try
      {
        using (FileStream fileStream = File.OpenRead(source))
        {
          IEnumerable<byte> signature = this.signature;
          byte[] numArray = new byte[signature.Count<byte>()];
          fileStream.Read(numArray, 0, numArray.Length);
          return signature.SequenceEqual<byte>((IEnumerable<byte>) numArray);
        }
      }
      catch
      {
      }
      return true;
    }
  }
}

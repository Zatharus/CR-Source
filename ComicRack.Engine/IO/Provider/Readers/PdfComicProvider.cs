// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.PdfComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Text;
using cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Pdf;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  [FileFormat("PDF Document (PDF)", 1, ".pdf")]
  public class PdfComicProvider : ComicProvider
  {
    private IComicAccessor pdfReader;
    private List<ProviderImageInfo> infos = new List<ProviderImageInfo>();

    public PdfComicProvider()
    {
      PdfGhostScript pdfGhostScript = new PdfGhostScript();
      if (pdfGhostScript.IsAvailable())
        this.pdfReader = (IComicAccessor) pdfGhostScript;
      else
        this.pdfReader = (IComicAccessor) new PdfNative();
    }

    public override ImageProviderCapabilities Capabilities
    {
      get => ImageProviderCapabilities.FastFormatCheck;
    }

    protected override bool OnFastFormatCheck(string source)
    {
      try
      {
        using (FileStream fileStream = File.OpenRead(source))
        {
          byte[] buffer = new byte[4];
          fileStream.Read(buffer, 0, buffer.Length);
          return buffer[0] == (byte) 37 && buffer[1] == (byte) 80 && buffer[2] == (byte) 68 && buffer[3] == (byte) 70;
        }
      }
      catch
      {
        return true;
      }
    }

    public override string CreateHash()
    {
      using (FileStream inputStream = File.OpenRead(this.Source))
        return Base32.ToBase32String(new SHA1Managed().ComputeHash((Stream) inputStream));
    }

    protected override void OnParse()
    {
      foreach (ProviderImageInfo entry in this.pdfReader.GetEntryList(this.Source))
      {
        this.infos.Add(entry);
        if (!this.FireIndexReady(entry))
          break;
      }
    }

    protected override byte[] OnRetrieveSourceByteImage(int index)
    {
      return this.pdfReader.ReadByteImage(this.Source, this.infos[index]);
    }
  }
}

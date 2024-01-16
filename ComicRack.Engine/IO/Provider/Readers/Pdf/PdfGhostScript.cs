// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Pdf.PdfGhostScript
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Pdf
{
  public class PdfGhostScript : IComicAccessor
  {
    private int currentDpi = 75;

    public bool IsAvailable()
    {
      if (!string.IsNullOrEmpty(EngineConfiguration.Default.GhostscriptExecutable))
        PdfImages.GhostscriptPath = EngineConfiguration.Default.GhostscriptExecutable;
      return !EngineConfiguration.Default.DisableGhostscript && PdfImages.IsGhostscriptAvailable;
    }

    public bool IsFormat(string source) => throw new NotImplementedException();

    public IEnumerable<ProviderImageInfo> GetEntryList(string source)
    {
      PdfImages pdf = new PdfImages(source, EngineConfiguration.Default.TempPath);
      for (int i = 0; i < pdf.PageCount; ++i)
        yield return new ProviderImageInfo(i);
    }

    public byte[] ReadByteImage(string source, ProviderImageInfo info)
    {
      int index = info.Index;
      PdfImages pdfImages = new PdfImages(source, EngineConfiguration.Default.TempPath);
      byte[] pageData = pdfImages.GetPageData(index, this.currentDpi);
      if (pageData == null)
        return (byte[]) null;
      JpegFile jpegFile = new JpegFile(pageData);
      if (!jpegFile.IsValid)
        return (byte[]) null;
      if (jpegFile.Height >= 1024 && jpegFile.Height < 2048)
        return pageData;
      this.currentDpi = this.currentDpi * 1536 / jpegFile.Height;
      return pdfImages.GetPageData(index, this.currentDpi);
    }

    public ComicInfo ReadInfo(string source) => (ComicInfo) null;

    public bool WriteInfo(string source, ComicInfo info) => false;
  }
}

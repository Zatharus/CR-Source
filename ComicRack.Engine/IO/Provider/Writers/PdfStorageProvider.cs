// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Writers.PdfStorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using sharpPDF;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Writers
{
  [FileFormat("PDF Document (PDF)", 1, ".pdf")]
  public class PdfStorageProvider : StorageProvider
  {
    protected override ComicInfo OnStore(
      IImageProvider provider,
      ComicInfo info,
      string target,
      StorageSetting setting)
    {
      pdfDocument pdfDocument = new pdfDocument(Path.GetFileNameWithoutExtension(target), Application.ProductName);
      ComicInfo comicInfo = new ComicInfo(info);
      comicInfo.Pages.Clear();
      int num = 0;
      for (int page1 = 0; page1 < provider.Count; ++page1)
      {
        ComicPageInfo page2 = info.GetPage(page1);
        if (setting.IsValidPage(page1))
        {
          if (!this.FireProgressEvent(page1 * 100 / provider.Count))
          {
            foreach (StorageProvider.PageResult image1 in StorageProvider.GetImages(provider, page2, (string) null, setting, info.Manga == MangaYesNo.YesAndRightToLeft, false))
            {
              using (Bitmap image2 = image1.GetImage())
                pdfDocument.addPage(image2.Height, image2.Width).addImage((Image) image2, 0, 0);
              ComicPageInfo info1 = image1.Info with
              {
                ImageIndex = num++
              };
              comicInfo.Pages.Add(info1);
            }
          }
          else
            break;
        }
      }
      pdfDocument.createPDF(target);
      comicInfo.PageCount = comicInfo.Pages.Count;
      return comicInfo;
    }
  }
}

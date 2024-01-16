// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Writers.DjVuStorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.IO;
using cYo.Common.Win32;
using System.Drawing;
using System.IO;
using System.Reflection;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Writers
{
  [FileFormat("DjVu Document (DJVU)", 8, ".djvu")]
  public class DjVuStorageProvider : StorageProvider, IValidateProvider
  {
    private static readonly string combineExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\djvm.exe");

    protected override ComicInfo OnStore(
      IImageProvider provider,
      ComicInfo info,
      string target,
      StorageSetting setting)
    {
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
              {
                string tempFileName = Path.GetTempFileName();
                try
                {
                  DjVuImage.SaveDjVu(image2, tempFileName);
                  if (ExecuteProcess.Execute(DjVuStorageProvider.combineExe, string.Format("-{0} \"{1}\" \"{2}\"", num == 0 ? (object) "c" : (object) "i", (object) target, (object) tempFileName), ExecuteProcess.Options.None).ExitCode != 0)
                    throw new InvalidDataException();
                }
                finally
                {
                  FileUtility.SafeDelete(tempFileName);
                }
              }
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
      comicInfo.PageCount = comicInfo.Pages.Count;
      return comicInfo;
    }

    public bool IsValid => File.Exists(DjVuStorageProvider.combineExe);
  }
}

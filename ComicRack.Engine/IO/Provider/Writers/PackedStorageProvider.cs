// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Writers.PackedStorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Mathematics;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Writers
{
  public abstract class PackedStorageProvider : StorageProvider
  {
    protected abstract void OnCreateFile(string target, StorageSetting setting);

    protected abstract void OnCloseFile();

    protected abstract void AddEntry(string name, byte[] data);

    protected override ComicInfo OnStore(
      IImageProvider provider,
      ComicInfo info,
      string target,
      StorageSetting setting)
    {
      this.OnCreateFile(target, setting);
      List<PackedStorageProvider.IndexedPageResult> pages = new List<PackedStorageProvider.IndexedPageResult>();
      try
      {
        int loopCount = 0;
        long totalPageMemory = 0;
        Exception ce = (Exception) null;
        Parallel.For(0, provider.Count, new ParallelOptions()
        {
          MaxDegreeOfParallelism = EngineConfiguration.Default.ParallelConversions.Clamp(1, 8)
        }, (Action<int, ParallelLoopState>) ((n, ls) =>
        {
          try
          {
            if (!setting.IsValidPage(n))
              return;
            ComicPageInfo page = info.GetPage(n);
            ProviderImageInfo imageInfo = provider.GetImageInfo(page.ImageIndex);
            string ext = imageInfo == null || string.IsNullOrEmpty(imageInfo.Name) ? ".jpg" : Path.GetExtension(imageInfo.Name);
            int num = 0;
            foreach (StorageProvider.PageResult image in StorageProvider.GetImages(provider, page, ext, setting, info.Manga == MangaYesNo.YesAndRightToLeft, setting.CreateThumbnails))
            {
              bool flag;
              lock (this)
              {
                totalPageMemory += (long) image.Data.Length;
                flag = totalPageMemory > 52428800L;
              }
              if (flag)
                image.Store();
              lock (pages)
                pages.Add(new PackedStorageProvider.IndexedPageResult()
                {
                  Page = image,
                  Index = n,
                  Offset = num++,
                  OriginalName = imageInfo.Name
                });
            }
            if (!this.FireProgressEvent(++loopCount * 100 / provider.Count))
              return;
            ls.Break();
          }
          catch (Exception ex)
          {
            ce = ex;
            ls.Break();
          }
        }));
        if (ce != null)
          throw ce;
        int num1 = 0;
        ComicInfo comicInfo = new ComicInfo(info);
        comicInfo.Pages.Clear();
        pages.Sort();
        HashSet<string> nameTable = new HashSet<string>();
        foreach (PackedStorageProvider.IndexedPageResult ipr in pages)
        {
          ipr.Page.Restore();
          try
          {
            this.WritePage(ipr, num1++, comicInfo, setting, (ISet<string>) nameTable);
          }
          finally
          {
            ipr.Page.Clear();
          }
        }
        comicInfo.PageCount = comicInfo.Pages.Count;
        if (setting.EmbedComicInfo)
          this.AddInfo(comicInfo);
        return comicInfo;
      }
      finally
      {
        foreach (PackedStorageProvider.IndexedPageResult indexedPageResult in pages)
          indexedPageResult.Page.Clear();
        this.OnCloseFile();
      }
    }

    private void WritePage(
      PackedStorageProvider.IndexedPageResult ipr,
      int k,
      ComicInfo myInfo,
      StorageSetting setting,
      ISet<string> nameTable)
    {
      StorageProvider.PageResult page = ipr.Page;
      ComicPageInfo info = page.Info;
      string str1 = (string) null;
      if (setting.KeepOriginalImageNames && !string.IsNullOrEmpty(ipr.OriginalName))
        str1 = Path.GetFileNameWithoutExtension(ipr.OriginalName);
      if (string.IsNullOrEmpty(str1))
        str1 = string.Format("P{0:00000}", (object) (k + 1));
      if (!setting.KeepOriginalImageNames && info.IsBookmark)
        str1 = str1 + " - " + FileUtility.MakeValidFilename(info.Bookmark.Left(25));
      string str2 = str1;
      int num = 2;
      while (nameTable.Contains(str1))
        str1 = string.Format("{0}_{1}", (object) str2, (object) num++);
      nameTable.Add(str1);
      string name = str1 + page.Extension;
      this.AddEntry(name, page.Data);
      if (setting.CreateThumbnails)
        this.AddEntry(string.Format("Thumbnails\\{0}.tb", (object) str1), page.GetThumbnailData(setting));
      info.ImageIndex = k++;
      info.Rotation = ImageRotation.None;
      if (setting.AddKeyToPageInfo)
        info.Key = name;
      myInfo.Pages.Add(info);
    }

    protected virtual void AddInfo(ComicInfo comicInfo)
    {
      this.AddEntry("ComicInfo.xml", comicInfo.ToArray());
    }

    private class IndexedPageResult : IComparable<PackedStorageProvider.IndexedPageResult>
    {
      public int Index { get; set; }

      public int Offset { get; set; }

      public StorageProvider.PageResult Page { get; set; }

      public string OriginalName { get; set; }

      public int CompareTo(PackedStorageProvider.IndexedPageResult other)
      {
        int num = this.Index.CompareTo(other.Index);
        return num != 0 ? num : this.Offset.CompareTo(other.Offset);
      }
    }
  }
}

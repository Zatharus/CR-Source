// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive.ArchiveComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive
{
  public abstract class ArchiveComicProvider : ComicProvider
  {
    private List<ProviderImageInfo> foundImageList = new List<ProviderImageInfo>();
    private IComicAccessor imageArchive;
    private static Cache<FileKey, List<ProviderImageInfo>> imageInfoCache;

    public override ImageProviderCapabilities Capabilities
    {
      get => ImageProviderCapabilities.FastPageInfo | ImageProviderCapabilities.FastFormatCheck;
    }

    public override string CreateHash()
    {
      return ImageProvider.CreateHashFromImageList((IEnumerable<ProviderImageInfo>) this.foundImageList);
    }

    protected IEnumerable<ProviderImageInfo> GetFileList()
    {
      return this.imageArchive.GetEntryList(this.Source);
    }

    protected override byte[] OnRetrieveSourceByteImage(int index)
    {
      return this.imageArchive.ReadByteImage(this.Source, this.GetFile(index));
    }

    protected override ComicInfo OnLoadInfo() => this.imageArchive.ReadInfo(this.Source);

    protected override bool OnStoreInfo(ComicInfo comicInfo)
    {
      return this.imageArchive.WriteInfo(this.Source, comicInfo);
    }

    protected override bool OnFastFormatCheck(string source) => this.imageArchive.IsFormat(source);

    protected override void OnParse()
    {
      using (IItemLock<List<ProviderImageInfo>> cachedFileList = this.GetCachedFileList())
      {
        List<ProviderImageInfo> providerImageInfoList = new List<ProviderImageInfo>(cachedFileList.Item.Where<ProviderImageInfo>((Func<ProviderImageInfo, bool>) (ii => this.IsSupportedImage(ii.Name))));
        providerImageInfoList.Sort();
        this.foundImageList = providerImageInfoList;
      }
      using (List<ProviderImageInfo>.Enumerator enumerator = this.foundImageList.GetEnumerator())
      {
        do
          ;
        while (enumerator.MoveNext() && this.FireIndexReady(enumerator.Current));
      }
    }

    protected void SetArchive(IComicAccessor imageArchive) => this.imageArchive = imageArchive;

    private IItemLock<List<ProviderImageInfo>> GetCachedFileList()
    {
      using (ItemMonitor.Lock((object) typeof (ArchiveComicProvider)))
      {
        if (ArchiveComicProvider.imageInfoCache == null)
          ArchiveComicProvider.imageInfoCache = new Cache<FileKey, List<ProviderImageInfo>>(100);
      }
      return ArchiveComicProvider.imageInfoCache.LockItem(new FileKey(this.Source), (Func<FileKey, List<ProviderImageInfo>>) (fi => this.GetFileList().ToList<ProviderImageInfo>()));
    }

    public ProviderImageInfo GetFile(int index) => this.foundImageList[index];
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicPageInfoCollection
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Threading;
using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [Serializable]
  public class ComicPageInfoCollection : List<ComicPageInfo>
  {
    public ComicPageInfo FindByPage(int page)
    {
      using (ItemMonitor.Lock((object) this))
        return page < 0 || page >= this.Count ? ComicPageInfo.Empty : this[page];
    }

    public ComicPageInfo FindByImageIndex(int imageIndex)
    {
      return this.Find((Predicate<ComicPageInfo>) (cpi => cpi.ImageIndex == imageIndex));
    }

    public bool PagesAreEqual(ComicPageInfoCollection pages)
    {
      if (this.Count != pages.Count)
        return false;
      for (int index = 0; index < this.Count; ++index)
      {
        if (!object.Equals((object) this[index], (object) pages[index]))
          return false;
      }
      return true;
    }

    public void ResetPageSequence()
    {
      this.Sort((Comparison<ComicPageInfo>) ((a, b) => a.ImageIndex.CompareTo(b.ImageIndex)));
    }

    public void Consolidate()
    {
      if (this.Count < 2)
        return;
      HashSet<int> intSet = new HashSet<int>();
      using (ItemMonitor.Lock((object) this))
      {
        for (int index = 0; index < this.Count; ++index)
        {
          ComicPageInfo comicPageInfo = this[index];
          if (intSet.Contains(comicPageInfo.ImageIndex))
            this.RemoveAt(index--);
          else
            intSet.Add(comicPageInfo.ImageIndex);
        }
      }
    }

    public int SeekBookmark(int page, int direction)
    {
      direction = Math.Sign(direction);
      for (; page >= 0 && page < this.Count; page += direction)
      {
        if (this[page].IsBookmark)
          return page;
      }
      return -1;
    }
  }
}

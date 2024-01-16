// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookGroupBookPrice
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookGroupBookPrice : SingleComicGrouper
  {
    private readonly string[] captions = GroupInfo.TRGroup.GetStrings("PriceGroups", "Unknown|Free|0-10|10-20|20-30|30-40|40-50|50-100|over 100", '|');

    public override IGroupInfo GetGroup(ComicBook item)
    {
      int index = (double) item.BookPrice >= 0.0 ? ((double) item.BookPrice != 0.0 ? ((double) item.BookPrice < 0.0 || (double) item.BookPrice >= 10.0 ? ((double) item.BookPrice < 10.0 || (double) item.BookPrice >= 20.0 ? ((double) item.BookPrice < 20.0 || (double) item.BookPrice >= 30.0 ? ((double) item.BookPrice < 30.0 || (double) item.BookPrice >= 40.0 ? ((double) item.BookPrice < 40.0 || (double) item.BookPrice >= 50.0 ? ((double) item.BookPrice < 50.0 || (double) item.BookPrice >= 100.0 ? 8 : 7) : 6) : 5) : 4) : 3) : 2) : 1) : 0;
      return (IGroupInfo) new GroupInfo(this.captions[index], index);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ItemGroupCount
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public abstract class ItemGroupCount : SingleComicGrouper
  {
    private static readonly string[] captions = GroupInfo.TRGroup.GetStrings("CountGroups", "0-20|21-50|51-100|101-200|201-500|501-1000|>1000|Unspecified", '|');

    public override IGroupInfo GetGroup(ComicBook item)
    {
      return ItemGroupCount.GetNumberGroup(this.GetInt(item));
    }

    protected abstract int GetInt(ComicBook item);

    public static IGroupInfo GetNumberGroup(int n)
    {
      int index = 0;
      if (n < 0)
      {
        index = ItemGroupCount.captions.Length - 1;
      }
      else
      {
        if (n > 20)
          ++index;
        if (n > 50)
          ++index;
        if (n > 100)
          ++index;
        if (n > 200)
          ++index;
        if (n > 500)
          ++index;
        if (n > 1000)
          ++index;
      }
      return (IGroupInfo) new GroupInfo(ItemGroupCount.captions[index], index);
    }
  }
}

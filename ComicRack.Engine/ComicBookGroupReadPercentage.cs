// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookGroupReadPercentage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookGroupReadPercentage : SingleComicGrouper
  {
    private static readonly string[] captions = GroupInfo.TRGroup.GetStrings("ReadPercentageGroups", "Not Read|0%|10%|20%|30%|40%|50%|60%|70%|80%|90%|Completed", '|');

    public override IGroupInfo GetGroup(ComicBook item)
    {
      return ComicBookGroupReadPercentage.CreatePercentageGroup(item.ReadPercentage);
    }

    public static IGroupInfo CreatePercentageGroup(int p)
    {
      if (p != 0)
        p = p < 100 ? p / 10 + 1 : ComicBookGroupReadPercentage.captions.Length - 1;
      return (IGroupInfo) new GroupInfo(ComicBookGroupReadPercentage.captions[p], p);
    }
  }
}

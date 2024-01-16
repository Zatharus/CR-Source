// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookGroupRatingBase
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Mathematics;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public abstract class ComicBookGroupRatingBase : SingleComicGrouper
  {
    private static readonly string[] captions = GroupInfo.TRGroup.GetStrings("RatingGroups", "Not Rated|No Stars|1 Star|2 Stars|3 Stars|4 Stars|5 Stars", '|');

    public override IGroupInfo GetGroup(ComicBook item)
    {
      return ComicBookGroupRatingBase.GetRatingGroup(this.GetRating(item));
    }

    protected abstract int GetRating(ComicBook item);

    public static IGroupInfo GetRatingGroup(int rating)
    {
      int index = rating.Clamp(-1, ComicBookGroupRatingBase.captions.Length - 2) + 1;
      return (IGroupInfo) new GroupInfo(ComicBookGroupRatingBase.captions[index], ComicBookGroupRatingBase.captions.Length - index);
    }
  }
}

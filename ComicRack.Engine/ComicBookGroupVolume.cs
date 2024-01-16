// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookGroupVolume
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Text;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookGroupVolume : SingleComicGrouper
  {
    private readonly string[] captions = GroupInfo.TRGroup.GetStrings("VolumeGroups", "None|V{0}|Other", '|');

    public override IGroupInfo GetGroup(ComicBook item)
    {
      int shadowVolume = item.ShadowVolume;
      if (shadowVolume < 0)
        return (IGroupInfo) new GroupInfo(this.captions[0], 0);
      if (shadowVolume > 10000)
        return (IGroupInfo) new GroupInfo(this.captions[2], 1002);
      return (IGroupInfo) new GroupInfo(StringUtility.Format(this.captions[1], (object) shadowVolume), shadowVolume + 1);
    }

    public override ComicBookMatcher CreateMatcher(IGroupInfo info)
    {
      ComicBookVolumeMatcher matcher = new ComicBookVolumeMatcher();
      if (info.Index == 0)
        return (ComicBookMatcher) matcher;
      if (info.Index == 1002)
      {
        matcher.MatchOperator = 1;
        matcher.MatchValue = "10000";
      }
      else
      {
        matcher.MatchOperator = 0;
        matcher.MatchValue = (info.Index - 1).ToString();
      }
      return (ComicBookMatcher) matcher;
    }
  }
}

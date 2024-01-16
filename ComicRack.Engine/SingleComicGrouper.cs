// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.SingleComicGrouper
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public abstract class SingleComicGrouper : SingleGrouper<ComicBook>
  {
    public static IGroupInfo GetNameGroup(string text)
    {
      return ComicBook.EnableGroupNameCompression ? GroupInfo.GetCompressedNameGroup(text) : (IGroupInfo) new GroupInfo(string.IsNullOrEmpty(text) ? GroupInfo.Unspecified : text);
    }

    public virtual ComicBookMatcher CreateMatcher(IGroupInfo info) => (ComicBookMatcher) null;
  }
}

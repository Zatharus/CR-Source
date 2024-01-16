// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicListItemExtension
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  public static class ComicListItemExtension
  {
    public static ComicListItemFolder GetItemParent(this ComicListItem item)
    {
      return item.Library == null ? (ComicListItemFolder) null : item.Library.ComicLists.GetItems<ComicListItemFolder>().FirstOrDefault<ComicListItemFolder>((Func<ComicListItemFolder, bool>) (clif => clif.Items.Contains(item)));
    }

    public static IEnumerable<ComicListItem> GetItemPath(this ComicListItem item)
    {
      yield return item;
      ComicListItemFolder p;
      for (; (p = item.GetItemParent()) != null; item = (ComicListItem) p)
        yield return (ComicListItem) p;
    }

    public static string GetFullName(this ComicListItem item, string separator = ".")
    {
      string s = string.Empty;
      item.GetItemPath().Reverse<ComicListItem>().ForEach<ComicListItem>((Action<ComicListItem>) (n => s = s.AppendWithSeparator(separator, n.Name)));
      return s;
    }

    public static int GetLevel(this ComicListItem item)
    {
      return item.Library == null ? 0 : item.Library.ComicLists.GetChildLevel<ComicListItem>(item);
    }
  }
}

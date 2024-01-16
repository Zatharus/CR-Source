// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicListItemCollection
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  [XmlInclude(typeof (ComicIdListItem))]
  [XmlInclude(typeof (ComicLibraryListItem))]
  [XmlInclude(typeof (ComicSmartListItem))]
  [XmlInclude(typeof (ComicListItemFolder))]
  [Serializable]
  public class ComicListItemCollection : SmartList<ComicListItem>
  {
    public ComicListItemCollection()
      : base(SmartListOptions.Default | SmartListOptions.DisposeOnRemove)
    {
    }

    public IEnumerable<T> GetItems<T>(bool bottomUp = false) where T : ComicListItem
    {
      return this.Recurse<T>((Func<object, IEnumerable>) (o => !(o is ComicListItemFolder) ? (IEnumerable) null : (IEnumerable) ((ComicListItemFolder) o).Items), bottomUp);
    }

    public int GetChildLevel<T>(T cli) where T : ComicListItem
    {
      return this.GetChildLevel<T>(cli, (Func<object, IList>) (o => !(o is ComicListItemFolder) ? (IList) null : (IList) ((ComicListItemFolder) o).Items), 0);
    }

    public ComicListItem FindItem(Guid id)
    {
      return this.GetItems<ComicListItem>().FirstOrDefault<ComicListItem>((Func<ComicListItem, bool>) (cli => cli.Id == id));
    }
  }
}

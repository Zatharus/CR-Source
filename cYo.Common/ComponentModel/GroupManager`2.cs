// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.GroupManager`2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class GroupManager<T, G> where G : IGroupContainer<T>, new()
  {
    private readonly Dictionary<object, G> groups = new Dictionary<object, G>();
    private IGrouper<T> grouper;

    public GroupManager(IGrouper<T> grouper = null, IEnumerable<T> items = null)
    {
      this.grouper = grouper;
      if (items == null)
        return;
      this.AddRange(items);
    }

    private void AddGroupItem(IGroupInfo gi, T item)
    {
      if (gi == null)
        return;
      using (ItemMonitor.Lock((object) this.groups))
      {
        G g1;
        if (!this.groups.ContainsKey(gi.Key))
        {
          g1 = new G();
          g1.Info = gi;
          G g2 = g1;
          this.groups[gi.Key] = g2;
        }
        g1 = this.groups[gi.Key];
        using (ItemMonitor.Lock((object) g1.Items))
        {
          g1 = this.groups[gi.Key];
          g1.Items.Add(item);
        }
      }
    }

    private void AddGroupsItems(IEnumerable<IGroupInfo> gis, T item)
    {
      if (gis == null)
        return;
      foreach (IGroupInfo gi in gis)
        this.AddGroupItem(gi, item);
    }

    public void Add(T item)
    {
      if (item is IGroupable groupable)
      {
        if (groupable.IsMultiGroup)
          this.AddGroupsItems(groupable.GetGroups(), item);
        else
          this.AddGroupItem(groupable.GetGroup(), item);
      }
      else
      {
        if (this.grouper == null)
          throw new ArgumentException("No grouper available");
        if (this.grouper.IsMultiGroup)
          this.AddGroupsItems(this.grouper.GetGroups(item), item);
        else
          this.AddGroupItem(this.grouper.GetGroup(item), item);
      }
    }

    public void AddRange(IEnumerable<T> items) => items.ParallelForEach<T>(new Action<T>(this.Add));

    public void Reset()
    {
      using (ItemMonitor.Lock((object) this.groups))
        this.groups.Clear();
    }

    public IEnumerable<G> GetGroups()
    {
      using (ItemMonitor.Lock((object) this.groups))
        return (IEnumerable<G>) new List<G>((IEnumerable<G>) this.groups.Values);
    }

    public IGrouper<T> Grouper
    {
      get => this.grouper;
      set
      {
        if (this.grouper == value)
          return;
        this.grouper = value;
        this.Reset();
      }
    }
  }
}

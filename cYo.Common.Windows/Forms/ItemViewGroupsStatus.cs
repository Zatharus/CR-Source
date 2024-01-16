// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemViewGroupsStatus
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [Serializable]
  public class ItemViewGroupsStatus
  {
    private HashSet<int> keys = new HashSet<int>();

    public ItemViewGroupsStatus()
      : this((IEnumerable<GroupHeaderInformation>) null)
    {
    }

    public ItemViewGroupsStatus(IEnumerable<GroupHeaderInformation> headers)
    {
      this.Status = ItemViewGroupsStatus.GroupStatus.AllExpanded;
      if (headers == null)
        return;
      headers = (IEnumerable<GroupHeaderInformation>) headers.ToArray<GroupHeaderInformation>();
      int num1 = headers.Count<GroupHeaderInformation>();
      int num2 = headers.Count<GroupHeaderInformation>((Func<GroupHeaderInformation, bool>) (h => h.Collapsed));
      int num3 = num1 - num2;
      if (num3 == num1)
        this.Status = ItemViewGroupsStatus.GroupStatus.AllExpanded;
      else if (num2 == num1)
        this.Status = ItemViewGroupsStatus.GroupStatus.AllCollapsed;
      else if (num2 < num3)
      {
        this.Status = ItemViewGroupsStatus.GroupStatus.KeysCollapsed;
        this.keys.AddRange<int>(headers.Where<GroupHeaderInformation>((Func<GroupHeaderInformation, bool>) (h => h.Collapsed)).Select<GroupHeaderInformation, int>((Func<GroupHeaderInformation, int>) (h => h.Caption.GetHashCode())));
      }
      else
      {
        this.Status = ItemViewGroupsStatus.GroupStatus.KeysExpanded;
        this.keys.AddRange<int>(headers.Where<GroupHeaderInformation>((Func<GroupHeaderInformation, bool>) (h => !h.Collapsed)).Select<GroupHeaderInformation, int>((Func<GroupHeaderInformation, int>) (h => h.Caption.GetHashCode())));
      }
    }

    public ItemViewGroupsStatus.GroupStatus Status { get; set; }

    public HashSet<int> Keys => this.keys;

    public bool IsCollapsed(string caption)
    {
      switch (this.Status)
      {
        case ItemViewGroupsStatus.GroupStatus.AllExpanded:
          return false;
        case ItemViewGroupsStatus.GroupStatus.KeysExpanded:
          return !this.keys.Contains(caption.GetHashCode());
        case ItemViewGroupsStatus.GroupStatus.KeysCollapsed:
          return this.keys.Contains(caption.GetHashCode());
        default:
          return true;
      }
    }

    public bool IsCollapsed(GroupHeaderInformation header) => this.IsCollapsed(header.Caption);

    public bool IsExpanded(string caption) => !this.IsCollapsed(caption);

    public bool IsExpanded(GroupHeaderInformation header) => this.IsExpanded(header.Caption);

    public enum GroupStatus
    {
      AllExpanded,
      AllCollapsed,
      KeysExpanded,
      KeysCollapsed,
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemViewColumnCollection`1
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ItemViewColumnCollection<T> : SmartList<T> where T : IColumn
  {
    public T FindById(int id) => this.Find((Predicate<T>) (h => h.Id == id));

    public T FindBySorter(IComparer<IViewableItem> comp)
    {
      return comp != null ? this.Find((Predicate<T>) (h => h.ColumnSorter == comp)) : default (T);
    }

    public T FindByGrouper(IGrouper<IViewableItem> comp)
    {
      return comp != null ? this.Find((Predicate<T>) (h => h.ColumnGrouper == comp)) : default (T);
    }
  }
}

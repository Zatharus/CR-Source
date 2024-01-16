// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Ceco.InlineCollection
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

#nullable disable
namespace cYo.Common.Presentation.Ceco
{
  public class InlineCollection : Collection<Inline>
  {
    public void AddRange(IEnumerable<Inline> items)
    {
      foreach (Inline inline in items)
        this.Add(inline);
    }

    protected override void InsertItem(int index, Inline item)
    {
      base.InsertItem(index, item);
      this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, (object) item));
    }

    protected override void RemoveItem(int index)
    {
      Inline element = this[index];
      base.RemoveItem(index);
      this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, (object) element));
    }

    protected override void SetItem(int index, Inline item)
    {
      Inline element = this[index];
      base.SetItem(index, item);
      this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, (object) element));
      this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, (object) item));
    }

    protected override void ClearItems()
    {
      foreach (object element in (Collection<Inline>) this)
        this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
      base.ClearItems();
      this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, (object) null));
    }

    public event CollectionChangeEventHandler Changed;

    protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
    {
      if (this.Changed == null)
        return;
      this.Changed((object) this, e);
    }
  }
}

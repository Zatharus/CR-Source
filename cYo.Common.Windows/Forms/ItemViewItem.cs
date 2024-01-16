// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemViewItem
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ItemViewItem : BaseViewItem, IViewableItem, IBaseViewItem, INotifyPropertyChanged
  {
    private ItemViewMode cachedType;
    private System.Drawing.Size cachedSize;
    private System.Drawing.Size cachedInSize;

    public ItemViewItem()
    {
    }

    public ItemViewItem(string text)
      : base(text)
    {
    }

    protected ItemViewStates State
    {
      get => this.View != null ? this.View.GetItemState((IViewableItem) this) : ItemViewStates.None;
      set
      {
        if (this.View == null)
          return;
        this.View.SetItemState((IViewableItem) this, value);
      }
    }

    public bool Focused
    {
      get => (this.State & ItemViewStates.Focused) != 0;
      set => this.State |= ItemViewStates.Focused;
    }

    public bool Selected
    {
      get => (this.State & ItemViewStates.Selected) != 0;
      set
      {
        this.State = value ? this.State | ItemViewStates.Selected : this.State & ~ItemViewStates.Selected;
      }
    }

    public bool Hot
    {
      get => (this.State & ItemViewStates.Hot) != 0;
      set
      {
        this.State = value ? this.State | ItemViewStates.Hot : this.State & ~ItemViewStates.Hot;
      }
    }

    public int Index => this.View != null ? this.View.Items.IndexOf((IViewableItem) this) : -1;

    public void EnsureVisible()
    {
      if (this.View == null)
        return;
      this.View.EnsureItemVisible((IViewableItem) this);
    }

    public void Update() => this.Update(false);

    public void Update(bool sizeChanged)
    {
      if (sizeChanged)
        this.ForceRecalcSize();
      if (this.View == null)
        return;
      this.View.UpdateItem((IViewableItem) this, sizeChanged);
    }

    protected override void OnPropertyChanged(string name)
    {
      base.OnPropertyChanged(name);
      this.Update(true);
    }

    [field: NonSerialized]
    public event PropertyChangedEventHandler BookChanged;

    protected virtual void OnBookChanged(PropertyChangedEventArgs e)
    {
      if (this.BookChanged == null)
        return;
      this.BookChanged((object) this, e);
    }

    private void ForceRecalcSize() => this.cachedSize = System.Drawing.Size.Empty;

    public virtual bool OnClick(System.Drawing.Point pt) => false;

    public void OnMeasure(ItemSizeInformation sizeInfo)
    {
      try
      {
        if (sizeInfo.SubItem == -1)
          sizeInfo.Size = this.Measure(sizeInfo.Graphics, sizeInfo.Bounds.Size, sizeInfo.DisplayType);
        else
          sizeInfo.Size = this.MeasureColumn(sizeInfo.Graphics, sizeInfo.Header, sizeInfo.Bounds.Size);
      }
      catch (Exception ex)
      {
      }
    }

    public System.Drawing.Size Measure(
      Graphics graphics,
      System.Drawing.Size defaultSize,
      ItemViewMode displayType)
    {
      if (this.cachedSize.IsEmpty || this.cachedInSize != defaultSize || this.cachedType != displayType)
      {
        this.cachedInSize = defaultSize;
        this.cachedType = displayType;
        this.cachedSize = this.MeasureItem(graphics, defaultSize, displayType);
      }
      return this.cachedSize;
    }

    public virtual Control GetEditControl(int subItem) => (Control) null;

    public virtual ItemViewStates GetOwnerDrawnStates(ItemViewMode displayType)
    {
      return ItemViewStates.None;
    }

    protected virtual System.Drawing.Size MeasureItem(
      Graphics graphics,
      System.Drawing.Size defaultSize,
      ItemViewMode displayType)
    {
      return defaultSize;
    }

    protected virtual System.Drawing.Size MeasureColumn(
      Graphics graphics,
      IColumn header,
      System.Drawing.Size defaultSize)
    {
      return defaultSize;
    }

    public virtual void OnDraw(ItemDrawInformation drawInfo)
    {
    }
  }
}

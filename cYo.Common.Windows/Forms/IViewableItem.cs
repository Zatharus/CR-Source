// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.IViewableItem
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public interface IViewableItem : IBaseViewItem, INotifyPropertyChanged
  {
    void OnDraw(ItemDrawInformation drawInfo);

    void OnMeasure(ItemSizeInformation sizeInfo);

    bool OnClick(System.Drawing.Point pt);

    Control GetEditControl(int subItem);

    ItemViewStates GetOwnerDrawnStates(ItemViewMode mode);
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.IBaseViewItem
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System.ComponentModel;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public interface IBaseViewItem : INotifyPropertyChanged
  {
    string Text { get; }

    string Name { get; }

    string TooltipText { get; }

    object Tag { get; set; }

    object Data { get; set; }

    ItemView View { get; set; }

    int HitTest(System.Drawing.Point pt);
  }
}

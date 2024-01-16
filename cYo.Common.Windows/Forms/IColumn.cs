// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.IColumn
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public interface IColumn : IBaseViewItem, INotifyPropertyChanged
  {
    int Id { get; set; }

    int FormatId { get; set; }

    string[] FormatTexts { get; }

    bool Visible { get; set; }

    int Width { get; set; }

    DateTime LastTimeVisible { get; set; }

    IComparer<IViewableItem> ColumnSorter { get; set; }

    IGrouper<IViewableItem> ColumnGrouper { get; set; }

    StringAlignment Alignment { get; set; }

    void DrawHeader(Graphics gr, Rectangle rc, HeaderState style);
  }
}

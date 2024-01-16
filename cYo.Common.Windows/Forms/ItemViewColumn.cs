// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ItemViewColumn
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ItemViewColumn : BaseViewItem, IColumn, IBaseViewItem, INotifyPropertyChanged
  {
    private int id;
    private int formatId;
    private bool visible = true;
    private int width;
    private StringAlignment alignment;
    private DateTime lastTimeVisible = DateTime.MinValue;
    private readonly string[] formatTexts = new string[0];

    public ItemViewColumn()
    {
    }

    public ItemViewColumn(
      int id,
      string text,
      int width,
      object tag,
      IComparer<IViewableItem> sorter = null,
      IGrouper<IViewableItem> grouper = null,
      bool visible = true,
      StringAlignment align = StringAlignment.Near,
      string[] formatTexts = null)
      : base(text, tag)
    {
      this.id = id;
      this.width = width;
      this.ColumnSorter = sorter;
      this.ColumnGrouper = grouper;
      this.visible = visible;
      this.alignment = align;
      if (formatTexts == null)
        return;
      this.formatTexts = formatTexts;
    }

    public int Id
    {
      get => this.id;
      set
      {
        if (this.id == value)
          return;
        this.id = value;
        this.OnPropertyChanged(nameof (Id));
      }
    }

    public int FormatId
    {
      get => this.formatId;
      set
      {
        if (this.formatId == value)
          return;
        this.formatId = value;
        this.OnPropertyChanged(nameof (FormatId));
      }
    }

    public IComparer<IViewableItem> ColumnSorter { get; set; }

    public IGrouper<IViewableItem> ColumnGrouper { get; set; }

    public bool Visible
    {
      get => this.visible;
      set
      {
        if (this.visible == value)
          return;
        this.visible = value;
        if (!this.visible)
          this.lastTimeVisible = DateTime.Now;
        this.OnPropertyChanged(nameof (Visible));
      }
    }

    public int Width
    {
      get => this.width;
      set
      {
        if (value < 0 || this.width == value)
          return;
        this.width = value;
        this.OnPropertyChanged(nameof (Width));
      }
    }

    public StringAlignment Alignment
    {
      get => this.alignment;
      set
      {
        if (this.alignment == value)
          return;
        this.alignment = value;
        this.OnPropertyChanged(nameof (Alignment));
      }
    }

    public DateTime LastTimeVisible
    {
      get => this.lastTimeVisible;
      set => this.lastTimeVisible = value;
    }

    public string[] FormatTexts => this.formatTexts;

    public void DrawHeader(Graphics gr, Rectangle rc, HeaderState state)
    {
      HeaderAdornments adornments = this.FormatTexts.Length == 0 || state != HeaderState.Hot && state != HeaderState.Pressed ? HeaderAdornments.None : HeaderAdornments.DropDown;
      if (this.View.ItemSorter != null && this.View.ItemSorter == this.ColumnSorter && this.View.ItemSortOrder != SortOrder.None)
        adornments |= this.View.ItemSortOrder == SortOrder.Ascending ? HeaderAdornments.SortDown : HeaderAdornments.SortUp;
      HeaderControl.Draw(gr, rc, this.View.Font, this.Alignment, this.Text, state, adornments);
    }
  }
}

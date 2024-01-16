// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.BaseViewItem
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public abstract class BaseViewItem : IBaseViewItem, INotifyPropertyChanged
  {
    private string name = string.Empty;
    private string text = string.Empty;
    private string tooltipText = string.Empty;
    private object tag;
    private object data;

    protected BaseViewItem()
    {
    }

    protected BaseViewItem(string text) => this.text = text;

    protected BaseViewItem(string text, object tag)
      : this(text)
    {
      this.tag = tag;
    }

    protected BaseViewItem(string text, object tag, object data)
      : this(text, tag)
    {
      this.data = data;
    }

    public virtual string Name
    {
      get => this.name;
      set
      {
        if (this.name == value)
          return;
        this.name = value;
        this.OnPropertyChanged(nameof (Name));
      }
    }

    public virtual string Text
    {
      get => this.text;
      set
      {
        if (this.text == value)
          return;
        this.text = value;
        this.OnPropertyChanged(nameof (Text));
      }
    }

    public string TooltipText
    {
      get => this.tooltipText;
      set
      {
        if (this.tooltipText == value)
          return;
        this.tooltipText = value;
        this.OnPropertyChanged(nameof (TooltipText));
      }
    }

    public object Tag
    {
      get => this.tag;
      set
      {
        if (this.tag == value)
          return;
        this.tag = value;
        this.OnPropertyChanged(nameof (Tag));
      }
    }

    public object Data
    {
      get => this.data;
      set
      {
        if (this.data == value)
          return;
        this.data = value;
        this.OnPropertyChanged(nameof (Data));
      }
    }

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string name)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    public ItemView View { get; set; }

    public virtual int HitTest(System.Drawing.Point pt) => -1;
  }
}

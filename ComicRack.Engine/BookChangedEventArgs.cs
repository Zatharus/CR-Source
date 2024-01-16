// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.BookChangedEventArgs
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class BookChangedEventArgs : PropertyChangedEventArgs
  {
    public BookChangedEventArgs(string propertyName, int page, bool isComicInfo)
      : base(propertyName)
    {
      this.IsComicInfo = isComicInfo;
      this.Page = page;
    }

    public BookChangedEventArgs(string propertyName, bool isComicInfo)
      : this(propertyName, -1, isComicInfo)
    {
    }

    public BookChangedEventArgs(
      string propertyName,
      bool isComicInfo,
      object oldValue,
      object newValue)
      : this(propertyName, -1, isComicInfo)
    {
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    public BookChangedEventArgs(BookChangedEventArgs e)
      : base(e.PropertyName)
    {
      this.OldValue = e.OldValue;
      this.NewValue = e.NewValue;
      this.IsComicInfo = e.IsComicInfo;
      this.Page = e.Page;
    }

    public object OldValue { get; private set; }

    public object NewValue { get; private set; }

    public bool IsComicInfo { get; private set; }

    public int Page { get; private set; }
  }
}

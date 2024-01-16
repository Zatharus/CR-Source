// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicPageInfo
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [TypeConverter(typeof (ComicPageInfoConverter))]
  [Serializable]
  public struct ComicPageInfo
  {
    private static readonly string UnknownText = TR.Default["Unknown", "Unknown"];
    private volatile short imageIndex;
    private volatile ComicPageType pageType;
    private volatile string bookmark;
    private volatile int imageFileSize;
    private volatile short imageWidth;
    private volatile short imageHeight;
    private volatile ImageRotation rotation;
    private volatile ComicPagePosition pagePosition;
    private volatile string key;
    public static readonly ComicPageInfo Empty;

    public ComicPageInfo(int index)
    {
      this.imageHeight = (short) 0;
      this.imageWidth = (short) 0;
      this.bookmark = (string) null;
      this.imageFileSize = 0;
      this.rotation = ImageRotation.None;
      this.pagePosition = ComicPagePosition.Default;
      this.imageIndex = (short) (index + 1);
      this.pageType = index == 0 ? ComicPageType.FrontCover : ComicPageType.Story;
      this.key = (string) null;
    }

    [XmlAttribute("Image")]
    public int ImageIndex
    {
      get => (int) this.imageIndex - 1;
      set => this.imageIndex = (short) (value + 1);
    }

    [XmlIgnore]
    public ComicPageType PageType
    {
      get => this.pageType != (ComicPageType) 0 ? this.pageType : ComicPageType.Story;
      set => this.pageType = value == (ComicPageType) 0 ? ComicPageType.Story : value;
    }

    [XmlAttribute]
    [DefaultValue(null)]
    public string Bookmark
    {
      get => this.bookmark;
      set => this.bookmark = string.IsNullOrEmpty(value) ? (string) null : value;
    }

    [DefaultValue(0)]
    [XmlAttribute("ImageSize")]
    public int ImageFileSize
    {
      get => this.imageFileSize;
      set => this.imageFileSize = value;
    }

    [DefaultValue(0)]
    [XmlAttribute]
    public int ImageWidth
    {
      get => (int) this.imageWidth;
      set => this.imageWidth = (short) value;
    }

    [DefaultValue(0)]
    [XmlAttribute]
    public int ImageHeight
    {
      get => (int) this.imageHeight;
      set => this.imageHeight = (short) value;
    }

    [DefaultValue(ImageRotation.None)]
    [XmlAttribute]
    public ImageRotation Rotation
    {
      get => this.rotation;
      set => this.rotation = value;
    }

    [DefaultValue(ComicPagePosition.Default)]
    [XmlAttribute]
    public ComicPagePosition PagePosition
    {
      get => this.pagePosition;
      set => this.pagePosition = value;
    }

    [DefaultValue(null)]
    [XmlAttribute]
    public string Key
    {
      get => this.key;
      set => this.key = value;
    }

    [XmlAttribute("Type")]
    [Browsable(false)]
    [DefaultValue("Story")]
    public string TypeSerialized
    {
      get => this.PageType.ToString();
      set
      {
        ComicPageType result;
        if (!Enum.TryParse<ComicPageType>(value.Replace("Advertisment", "Advertisement"), out result))
          return;
        this.pageType = result;
      }
    }

    public string PageTypeAsText
    {
      get => LocalizeUtility.LocalizeEnum(typeof (ComicPageType), (int) this.PageType);
    }

    public string PagePositionAsText
    {
      get => LocalizeUtility.LocalizeEnum(typeof (ComicPagePosition), (int) this.PagePosition);
    }

    public string ImageFileSizeAsText
    {
      get
      {
        long imageFileSize = (long) this.ImageFileSize;
        if (imageFileSize <= 0L)
          return ComicPageInfo.UnknownText;
        return string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
        {
          (object) imageFileSize
        });
      }
    }

    public string ImageWidthAsText
    {
      get => this.ImageWidth != 0 ? this.ImageWidth.ToString() : ComicPageInfo.UnknownText;
    }

    public string ImageHeightAsText
    {
      get => this.ImageHeight != 0 ? this.ImageHeight.ToString() : ComicPageInfo.UnknownText;
    }

    public string RotationAsText => string.Format("{0}°", (object) this.rotation.ToDegrees());

    public bool IsBookmark => !string.IsNullOrEmpty(this.Bookmark);

    public bool IsEmpty => this.Equals((object) ComicPageInfo.Empty);

    public bool IsDoublePage => this.ImageWidth > this.ImageHeight;

    public bool IsFrontCover => this.IsTypeOf(ComicPageType.FrontCover);

    public bool IsDeleted => this.IsTypeOf(ComicPageType.Deleted);

    public bool IsTypeOf(ComicPageType type) => (this.PageType & type) != 0;

    public bool IsDefaultContent(int index)
    {
      return (index == -1 || (int) this.imageIndex == index + 1) && this.ImageWidth == 0 && this.ImageHeight == 0 && this.PageType == ComicPageType.Story && this.ImageFileSize == 0 && this.Bookmark == null;
    }

    public bool IsSinglePageType
    {
      get => this.IsTypeOf(ComicPageType.FrontCover | ComicPageType.BackCover);
    }

    public bool IsSingleRightPageType => this.IsTypeOf(ComicPageType.FrontCover);

    public string GetStringValue(string propName)
    {
      try
      {
        return this.GetType().GetProperty(propName).GetValue((object) this, (object[]) null) as string;
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }

    public override bool Equals(object obj)
    {
      return obj != null && obj is ComicPageInfo comicPageInfo && (int) this.imageIndex == (int) comicPageInfo.imageIndex && this.pageType == comicPageInfo.pageType && (int) this.imageHeight == (int) comicPageInfo.imageHeight && (int) this.imageWidth == (int) comicPageInfo.imageWidth && this.bookmark == comicPageInfo.bookmark && this.pagePosition == comicPageInfo.pagePosition;
    }

    public override int GetHashCode() => this.imageIndex.GetHashCode();

    public override string ToString()
    {
      return TypeDescriptor.GetConverter((object) this).ConvertToString((object) this);
    }

    public static bool operator ==(ComicPageInfo a, ComicPageInfo b) => a.Equals((object) b);

    public static bool operator !=(ComicPageInfo a, ComicPageInfo b) => !(a == b);

    public static ComicPageInfo Parse(string text)
    {
      return (ComicPageInfo) TypeDescriptor.GetConverter(typeof (ComicPageInfo)).ConvertFromString(text);
    }
  }
}

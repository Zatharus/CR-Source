// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBook
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Reflection;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Xml;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Engine.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  [ComVisible(true)]
  [Serializable]
  public class ComicBook : ComicInfo, IImageKeyProvider, ICloneable
  {
    public const int ReadPercentageAsRead = 95;
    public const string ClipboardFormat = "ComicBook";
    public const string DefaultCaptionFormat = "[{format} ][{series}][ {volume}][ #{number}][ - {title}][ ({year}[/{month}[/{day}]])]";
    public const string DefaultAlternateCaptionFormat = "[{alternateseries}][ #{alternatenumber}]";
    public const string DefaultComicExportFileNameFormat = "[{format} ][{series}][ {volume}][ #{number}][ ({year}[/{month}])]";
    public static readonly Equality<ComicBook> GuidEquality = new Equality<ComicBook>((Func<ComicBook, ComicBook, bool>) ((a, b) => a.Id == b.Id), (Func<ComicBook, int>) (a => a.Id.GetHashCode()));
    public static bool EnableGroupNameCompression = false;
    private static TR tr;
    private static readonly Lazy<string> unkownText = new Lazy<string>((Func<string>) (() => ComicBook.TR["Unknown"]));
    private static readonly Lazy<string> pagesText = new Lazy<string>((Func<string>) (() => ComicBook.TR["Pages", "{0} Page(s)"]));
    private static readonly Lazy<string> lastTimeOpenedAtText = new Lazy<string>((Func<string>) (() => ComicBook.TR["LastTimeOpenedAt", "Last time opened at {0}"]));
    private static readonly Lazy<string> readingAtPageText = new Lazy<string>((Func<string>) (() => ComicBook.TR["ReadingAtPage", "Reading at page {0}"]));
    private static readonly Lazy<string> lastPageReadIsText = new Lazy<string>((Func<string>) (() => ComicBook.TR["LastPageReadIs", "Last page read is {0}"]));
    private static readonly Lazy<string> noneText = new Lazy<string>((Func<string>) (() => ComicBook.TR["None", "None"]));
    private static readonly Lazy<string> notFoundText = new Lazy<string>((Func<string>) (() => ComicBook.TR["NotFound", "not found"]));
    private static readonly Lazy<string> neverText = new Lazy<string>((Func<string>) (() => ComicBook.TR["Never", "never"]));
    private static readonly Lazy<string> volumeFormat = new Lazy<string>((Func<string>) (() => ComicBook.TR["Volume", "V{0}"]));
    private static readonly Lazy<string> ofText = new Lazy<string>((Func<string>) (() => ComicBook.TR["Of", "of"]));
    [NonSerialized]
    private volatile ComicBookContainer container;
    private Guid id = Guid.NewGuid();
    private DateTime addedTime = DateTime.MinValue;
    private DateTime releasedTime = DateTime.MinValue;
    private DateTime openedTime = DateTime.MinValue;
    private volatile int openCount;
    private volatile int currentPage;
    private volatile int lastPage;
    private float rating;
    private string tags = string.Empty;
    private BitmapAdjustment colorAdjustment = BitmapAdjustment.Empty;
    private bool enableProposed = true;
    private YesNo seriesComplete = YesNo.Unknown;
    private bool enableDynamicUpdate = true;
    private bool check = ComicBook.NewBooksChecked;
    [NonSerialized]
    private volatile bool fileInfoRetrieved;
    private volatile bool comicInfoIsDirty;
    private volatile string filePath = string.Empty;
    private long fileSize = -1;
    private volatile bool fileIsMissing;
    private DateTime fileModifiedTime = DateTime.MinValue;
    private DateTime fileCreationTime = DateTime.MinValue;
    private string customThumbnailKey;
    private float bookPrice = -1f;
    private string bookAge = string.Empty;
    private string bookCondition = string.Empty;
    private string bookStore = string.Empty;
    private string bookOwner = string.Empty;
    private string bookCollectionStatus = string.Empty;
    private string bookNotes = string.Empty;
    private string bookLocation = string.Empty;
    private string isbn = string.Empty;
    private volatile string fileName;
    private volatile string fileNameWithExtension;
    private volatile string fileFormat;
    private volatile string fileDirectory;
    private static readonly Calendar weekCalendar = (Calendar) new GregorianCalendar();
    private string fileLocation;
    private int newPages;
    private ComicNameInfo proposed;
    [NonSerialized]
    private TextNumberFloat compareNumber;
    [NonSerialized]
    private TextNumberFloat compareAlternateNumber;
    private static readonly Dictionary<ComicBook, ExtraSyncInformation> syncInfo = new Dictionary<ComicBook, ExtraSyncInformation>();
    private string customValuesStore = string.Empty;
    private static HashSet<string> searchableProperties;
    private static readonly Regex rxField = new Regex("{(?<name>[a-z]+)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Dictionary<string, CultureInfo> languages;
    public static readonly ComicBook Default = new ComicBook();
    private static readonly Dictionary<string, string> hasAsText = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly ImagePackage publisherIcons = new ImagePackage()
    {
      EnableWidthCropping = true
    };
    private static readonly ImagePackage ageRatingIcons = new ImagePackage()
    {
      EnableWidthCropping = true
    };
    private static readonly ImagePackage formatIcons = new ImagePackage()
    {
      EnableWidthCropping = true
    };
    private static readonly ImagePackage specialIcons = new ImagePackage()
    {
      EnableWidthCropping = true
    };

    public static TR TR
    {
      get
      {
        if (ComicBook.tr == null)
          ComicBook.tr = TR.Load(nameof (ComicBook));
        return ComicBook.tr;
      }
    }

    static ComicBook() => ComicBook.NewBooksChecked = true;

    public ComicBook()
    {
    }

    public ComicBook(ComicBook cb)
    {
      this.CopyFrom(cb);
      if (cb.CreateComicProvider != null)
        this.CreateComicProvider += cb.CreateComicProvider;
      if (cb.ComicProviderCreated == null)
        return;
      this.ComicProviderCreated += cb.ComicProviderCreated;
    }

    public static ComicBook Create(string file, RefreshInfoOptions options)
    {
      ComicBook comicBook = new ComicBook()
      {
        FilePath = file
      };
      comicBook.RefreshInfoFromFile(options);
      return comicBook;
    }

    [XmlIgnore]
    public ComicBookContainer Container
    {
      get => this.container;
      internal set => this.container = value;
    }

    [XmlAttribute]
    public Guid Id
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.id;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.id == value)
            return;
          this.id = value;
        }
        this.FireBookChanged(nameof (Id));
      }
    }

    [Browsable(true)]
    [XmlElement("Added")]
    [DefaultValue(typeof (DateTime), "01.01.0001")]
    [ResetValue(1)]
    public DateTime AddedTime
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.addedTime;
      }
      set
      {
        this.SetProperty<DateTime>(nameof (AddedTime), ref this.addedTime, value, true, !this.IsLinked);
      }
    }

    [Browsable(true)]
    [XmlElement("Released")]
    [DefaultValue(typeof (DateTime), "01.01.0001")]
    [ResetValue(1)]
    public DateTime ReleasedTime
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.releasedTime.DateOnly();
      }
      set => this.SetProperty<DateTime>(nameof (ReleasedTime), ref this.releasedTime, value, true);
    }

    [Browsable(true)]
    [XmlElement("Opened")]
    [DefaultValue(typeof (DateTime), "01.01.0001")]
    [ResetValue(1)]
    public DateTime OpenedTime
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.openedTime;
      }
      set
      {
        this.SetProperty<DateTime>(nameof (OpenedTime), ref this.openedTime, value, true, !this.IsLinked);
      }
    }

    [Browsable(true)]
    [XmlElement("OpenCount")]
    [DefaultValue(0)]
    [ResetValue(1)]
    public int OpenedCount
    {
      get => this.openCount;
      set
      {
        if (this.openCount == value)
          return;
        this.openCount = value;
        this.FireBookChanged(nameof (OpenedCount));
      }
    }

    [Browsable(true)]
    [DefaultValue(0)]
    [ResetValue(1)]
    public int CurrentPage
    {
      get => this.currentPage;
      set
      {
        value = Math.Max(0, value);
        if (this.currentPage == value)
          return;
        this.currentPage = value;
        this.FireBookChanged(nameof (CurrentPage));
        if (this.currentPage <= this.LastPageRead)
          return;
        this.LastPageRead = value;
      }
    }

    [Browsable(true)]
    [DefaultValue(0)]
    [ResetValue(1)]
    public int LastPageRead
    {
      get => this.lastPage;
      set
      {
        value = Math.Max(0, value);
        if (this.lastPage == value)
          return;
        this.lastPage = value;
        this.FireBookChanged(nameof (LastPageRead));
      }
    }

    [Browsable(true)]
    [DefaultValue(0)]
    [ResetValue(0)]
    public float Rating
    {
      get => this.rating;
      set => this.SetProperty<float>(nameof (Rating), ref this.rating, value.Clamp(0.0f, 5f));
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string Tags
    {
      get => this.tags;
      set => this.SetProperty<string>(nameof (Tags), ref this.tags, value);
    }

    [DefaultValue(typeof (BitmapAdjustment), "Empty")]
    public BitmapAdjustment ColorAdjustment
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.colorAdjustment;
      }
      set
      {
        BitmapAdjustment colorAdjustment;
        using (ItemMonitor.Lock((object) this))
        {
          if (this.colorAdjustment == value)
            return;
          colorAdjustment = this.colorAdjustment;
          this.colorAdjustment = value;
        }
        this.FireBookChanged(nameof (ColorAdjustment), (object) colorAdjustment, (object) this.colorAdjustment);
      }
    }

    public bool ColorAdjustmentSpecified => this.ColorAdjustment != BitmapAdjustment.Empty;

    [Browsable(true)]
    [DefaultValue(true)]
    [ResetValue(0)]
    public bool EnableProposed
    {
      get => this.enableProposed;
      set => this.SetProperty<bool>(nameof (EnableProposed), ref this.enableProposed, value);
    }

    [Browsable(true)]
    [DefaultValue(YesNo.Unknown)]
    [ResetValue(0)]
    public YesNo SeriesComplete
    {
      get => this.seriesComplete;
      set => this.SetProperty<YesNo>(nameof (SeriesComplete), ref this.seriesComplete, value);
    }

    [Browsable(true)]
    [DefaultValue(true)]
    [ResetValue(1)]
    public bool EnableDynamicUpdate
    {
      get => this.enableDynamicUpdate;
      set
      {
        this.SetProperty<bool>(nameof (EnableDynamicUpdate), ref this.enableDynamicUpdate, value);
      }
    }

    public Guid LastOpenedFromListId { get; set; }

    public bool LastOpenedFromListIdSpecified => this.LastOpenedFromListId != Guid.Empty;

    [XmlAttribute]
    [DefaultValue(true)]
    public bool Checked
    {
      get => this.check;
      set => this.SetProperty<bool>(nameof (Checked), ref this.check, value);
    }

    [Browsable(true)]
    [XmlIgnore]
    public bool FileInfoRetrieved
    {
      get => this.fileInfoRetrieved;
      set => this.fileInfoRetrieved = value;
    }

    [Browsable(true)]
    [DefaultValue(false)]
    public bool ComicInfoIsDirty
    {
      get => this.comicInfoIsDirty;
      set
      {
        if (this.comicInfoIsDirty == value)
          return;
        this.comicInfoIsDirty = value;
        this.FireBookChanged(nameof (ComicInfoIsDirty));
      }
    }

    [Browsable(true)]
    [XmlAttribute("File")]
    [DefaultValue("")]
    public string FilePath
    {
      get => this.filePath;
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        if (this.filePath == value)
          return;
        string filePath = this.FilePath;
        this.filePath = value;
        this.fileName = this.fileNameWithExtension = this.fileFormat = this.fileDirectory = (string) null;
        this.proposed = (ComicNameInfo) null;
        this.FireBookChanged(nameof (FilePath), (object) filePath, (object) this.filePath);
        if (string.IsNullOrEmpty(filePath))
          return;
        this.OnFileRenamed(new ComicBookFileRenameEventArgs(filePath, this.filePath));
      }
    }

    [Browsable(true)]
    [DefaultValue(-1)]
    public long FileSize
    {
      get => Interlocked.Read(ref this.fileSize);
      set
      {
        if (Interlocked.Read(ref this.fileSize) == value)
          return;
        Interlocked.Exchange(ref this.fileSize, value);
        this.FireBookChanged(nameof (FileSize));
      }
    }

    [Browsable(true)]
    [XmlElement("Missing")]
    [DefaultValue(false)]
    public bool FileIsMissing
    {
      get => this.fileIsMissing;
      set
      {
        if (this.fileIsMissing == value)
          return;
        this.fileIsMissing = value;
        this.FireBookChanged(nameof (FileIsMissing));
      }
    }

    [Browsable(true)]
    [DefaultValue(typeof (DateTime), "01.01.0001")]
    public DateTime FileModifiedTime
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.fileModifiedTime;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.fileModifiedTime == value)
            return;
          this.fileModifiedTime = value;
        }
        this.FireBookChanged(nameof (FileModifiedTime));
      }
    }

    [Browsable(true)]
    [DefaultValue(typeof (DateTime), "01.01.0001")]
    public DateTime FileCreationTime
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.fileCreationTime;
      }
      set
      {
        using (ItemMonitor.Lock((object) this))
        {
          if (this.fileCreationTime == value)
            return;
          this.fileCreationTime = value;
        }
        this.FireBookChanged(nameof (FileCreationTime));
      }
    }

    [DefaultValue(null)]
    [ResetValue(0)]
    public string CustomThumbnailKey
    {
      get => this.customThumbnailKey;
      set
      {
        if (this.customThumbnailKey == value)
          return;
        this.customThumbnailKey = value;
        this.FireBookChanged(nameof (CustomThumbnailKey));
      }
    }

    [Browsable(true)]
    [DefaultValue(-1f)]
    [ResetValue(0)]
    public float BookPrice
    {
      get => this.bookPrice;
      set => this.SetProperty<float>(nameof (BookPrice), ref this.bookPrice, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string BookAge
    {
      get => this.bookAge;
      set => this.SetProperty<string>(nameof (BookAge), ref this.bookAge, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string BookCondition
    {
      get => this.bookCondition;
      set => this.SetProperty<string>(nameof (BookCondition), ref this.bookCondition, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string BookStore
    {
      get => this.bookStore;
      set => this.SetProperty<string>(nameof (BookStore), ref this.bookStore, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string BookOwner
    {
      get => this.bookOwner;
      set => this.SetProperty<string>(nameof (BookOwner), ref this.bookOwner, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string BookCollectionStatus
    {
      get => this.bookCollectionStatus;
      set
      {
        this.SetProperty<string>(nameof (BookCollectionStatus), ref this.bookCollectionStatus, value);
      }
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string BookNotes
    {
      get => this.bookNotes;
      set => this.SetProperty<string>(nameof (BookNotes), ref this.bookNotes, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string BookLocation
    {
      get => this.bookLocation;
      set => this.SetProperty<string>(nameof (BookLocation), ref this.bookLocation, value);
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string ISBN
    {
      get => this.isbn;
      set => this.SetProperty<string>(nameof (ISBN), ref this.isbn, value);
    }

    [XmlIgnore]
    public string PagesAsTextSimple
    {
      get => this.PageCount > 0 ? this.PageCount.ToString() : "-";
      set
      {
        int result;
        if (!int.TryParse(value, out result) || result < 0)
          return;
        this.PageCount = result;
      }
    }

    [Browsable(true)]
    public string FileName
    {
      get
      {
        if (this.fileName != null)
          return this.fileName;
        try
        {
          this.fileName = Path.GetFileNameWithoutExtension(this.filePath);
        }
        catch (Exception ex)
        {
          this.fileName = string.Empty;
        }
        return this.fileName;
      }
    }

    [Browsable(true)]
    public string FileNameWithExtension
    {
      get
      {
        if (this.fileNameWithExtension != null)
          return this.fileNameWithExtension;
        try
        {
          this.fileNameWithExtension = Path.GetFileName(this.filePath);
        }
        catch (Exception ex)
        {
          this.fileNameWithExtension = string.Empty;
        }
        return this.fileNameWithExtension;
      }
    }

    [Browsable(true)]
    public string FileFormat
    {
      get
      {
        if (this.fileFormat != null)
          return this.fileFormat;
        try
        {
          this.fileFormat = Providers.Readers.GetSourceFormatName(this.filePath);
        }
        catch (Exception ex)
        {
          this.fileFormat = string.Empty;
        }
        return this.fileFormat;
      }
    }

    [Browsable(true)]
    public string FileDirectory
    {
      get
      {
        if (this.fileDirectory != null)
          return this.fileDirectory;
        try
        {
          this.fileDirectory = Path.GetDirectoryName(this.filePath);
        }
        catch (Exception ex)
        {
          this.fileDirectory = string.Empty;
        }
        return this.fileDirectory;
      }
    }

    [Browsable(true)]
    public bool IsValidComicBook
    {
      get
      {
        return !string.IsNullOrEmpty(this.FilePath) && Providers.Readers.GetSourceProviderType(this.FilePath) != (Type) null;
      }
    }

    [Browsable(true)]
    public string Caption => this.GetFullTitle(EngineConfiguration.Default.ComicCaptionFormat);

    [Browsable(true)]
    public string CaptionWithoutTitle
    {
      get => this.GetFullTitle(EngineConfiguration.Default.ComicCaptionFormat, "title");
    }

    [Browsable(true)]
    public string CaptionWithoutFormat
    {
      get => this.GetFullTitle(EngineConfiguration.Default.ComicCaptionFormat, "format");
    }

    [Browsable(true)]
    public string AlternateCaption => this.GetFullTitle("[{alternateseries}][ #{alternatenumber}]");

    public string TargetFilename
    {
      get => this.GetFullTitle(EngineConfiguration.Default.ComicExportFileNameFormat);
    }

    [Browsable(true)]
    public int ReadPercentage
    {
      get
      {
        return this.PageCount <= 0 || this.LastPageRead <= 0 ? 0 : ((this.LastPageRead + 1) * 100 / this.PageCount).Clamp(1, 100);
      }
    }

    public string ReadPercentageAsText => string.Format("{0}%", (object) this.ReadPercentage);

    [Browsable(true)]
    public bool HasBeenOpened => this.OpenedTime != DateTime.MinValue;

    [Browsable(true)]
    [XmlIgnore]
    public bool HasBeenRead
    {
      get => this.ReadPercentage >= 95;
      set
      {
        if (value)
          this.MarkAsRead();
        else
          this.MarkAsNotRead();
      }
    }

    public string PagesAsText => ComicBook.FormatPages(this.PageCount);

    public string OpenedCountAsText => this.OpenedCount.ToString();

    public string InfoText
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("{0}\n", (object) this.FileName);
        stringBuilder.AppendFormat("{0} ({1})\n\n", (object) this.FileSizeAsText, (object) this.PagesAsText);
        stringBuilder.AppendFormat("{0}\n", (object) this.FileFormat);
        stringBuilder.AppendFormat("{0}\n\n", (object) this.FileDirectory);
        stringBuilder.Append(StringUtility.Format(ComicBook.lastTimeOpenedAtText.Value, (object) this.OpenedTimeAsText));
        stringBuilder.AppendLine();
        stringBuilder.Append(StringUtility.Format(ComicBook.readingAtPageText.Value, (object) (this.CurrentPage + 1)));
        stringBuilder.AppendLine();
        stringBuilder.Append(StringUtility.Format(ComicBook.lastPageReadIsText.Value, (object) (this.LastPageRead + 1)));
        return stringBuilder.ToString();
      }
    }

    public string NumberAsText => ComicBook.FormatNumber(this.Number, this.ShadowCount);

    public string NumberOnly => this.ShadowNumber;

    public string AlternateNumberAsText
    {
      get => ComicBook.FormatNumber(this.AlternateNumber, this.AlternateCount);
    }

    public string VolumeAsText => ComicBook.FormatVolume(this.Volume);

    public string VolumeOnly => this.Volume >= 0 ? this.Volume.ToString() : string.Empty;

    public string LastPageReadAsText
    {
      get => this.LastPageRead > 0 ? (this.LastPageRead + 1).ToString() : ComicBook.noneText.Value;
    }

    public string LanguageAsText
    {
      get
      {
        return !string.IsNullOrEmpty(this.LanguageISO) ? ComicBook.GetLanguageName(this.LanguageISO) : string.Empty;
      }
    }

    public string ArtistInfo
    {
      get
      {
        StringBuilder s = new StringBuilder(this.Writer);
        ComicBook.AppendString(s, "/", this.Penciller);
        ComicBook.AppendString(s, "/", this.Inker);
        ComicBook.AppendString(s, "/", this.Colorist);
        ComicBook.AppendString(s, "/", this.Letterer);
        ComicBook.AppendString(s, "/", this.CoverArtist);
        return s.ToString();
      }
    }

    public string YearAsText => ComicBook.FormatYear(this.Year);

    public string MonthAsText => this.Month != -1 ? this.Month.ToString() : string.Empty;

    public string DayAsText => this.Day != -1 ? this.Day.ToString() : string.Empty;

    public int Week
    {
      get
      {
        DateTime published = this.Published;
        return published == DateTime.MinValue ? -1 : ComicBook.weekCalendar.GetWeekOfYear(published, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
      }
    }

    public string WeekAsText
    {
      get
      {
        int week = this.Week;
        return week != -1 ? week.ToString() : string.Empty;
      }
    }

    public string PublishedAsText
    {
      get
      {
        string shadowYearAsText = this.ShadowYearAsText;
        string publishedAsText = string.Empty;
        if (!string.IsNullOrEmpty(shadowYearAsText))
        {
          string monthAsText = this.MonthAsText;
          if (!string.IsNullOrEmpty(monthAsText))
          {
            string dayAsText = this.DayAsText;
            if (!string.IsNullOrEmpty(dayAsText))
              publishedAsText = publishedAsText + dayAsText + "/";
            publishedAsText = publishedAsText + monthAsText + "/";
          }
          publishedAsText += shadowYearAsText;
        }
        return publishedAsText;
      }
    }

    public DateTime Published
    {
      get
      {
        if (this.ShadowYear <= 0)
          return DateTime.MinValue;
        int year = this.ShadowYear.Clamp(1, 10000);
        int month = this.Month.Clamp(1, 12);
        int day = this.Day.Clamp(1, DateTime.DaysInMonth(year, month));
        return new DateTime(year, month, day);
      }
    }

    public string CountAsText => this.Count != -1 ? this.Count.ToString() : string.Empty;

    public string NewPagesAsText
    {
      get => !this.IsDynamicSource || this.NewPages <= 0 ? string.Empty : this.NewPages.ToString();
    }

    public string AlternateCountAsText
    {
      get => this.AlternateCount != -1 ? this.AlternateCount.ToString() : string.Empty;
    }

    public string RatingAsText => ComicBook.FormatRating(this.Rating);

    public string CommunityRatingAsText => ComicBook.FormatRating(this.CommunityRating);

    public string CoverAsText
    {
      get => ComicInfo.GetYesNoAsText(this.FrontCoverCount != 0 ? YesNo.Yes : YesNo.No);
    }

    public string FileSizeAsText
    {
      get
      {
        long fileSize = this.FileSize;
        if (fileSize == -1L)
          return ComicBook.notFoundText.Value;
        return string.Format((IFormatProvider) new FileLengthFormat(), "{0}", new object[1]
        {
          (object) fileSize
        });
      }
    }

    public string MangaAsText => ComicInfo.GetYesNoAsText(this.Manga);

    public string SeriesCompleteAsText => ComicInfo.GetYesNoAsText(this.SeriesComplete);

    public string EnableProposedAsText
    {
      get => ComicInfo.GetYesNoAsText(this.EnableProposed ? YesNo.Yes : YesNo.No);
    }

    public string HasBeenReadAsText
    {
      get => ComicInfo.GetYesNoAsText(this.HasBeenRead ? YesNo.Yes : YesNo.No);
    }

    public string IsLinkedAsText => ComicInfo.GetYesNoAsText(this.IsLinked);

    public string BlackAndWhiteAsText => ComicInfo.GetYesNoAsText(this.BlackAndWhite);

    public string BookmarkCountAsText
    {
      get => this.BookmarkCount > 0 ? this.BookmarkCount.ToString() : ComicBook.noneText.Value;
    }

    public ComicPageInfo CurrentPageInfo => this.GetPage(this.CurrentPage);

    public string FileLocation
    {
      get => !string.IsNullOrEmpty(this.fileLocation) ? this.fileLocation : this.FilePath;
    }

    public string DisplayFileLocation => !this.IsInContainer ? this.FilePath : this.Caption;

    public int Status
    {
      get
      {
        int status = 0;
        if (this.FileIsMissing && this.IsLinked)
          status |= 1;
        if (this.ComicInfoIsDirty)
          status |= 2;
        return status;
      }
    }

    [Browsable(true)]
    public bool IsLinked => !string.IsNullOrEmpty(this.filePath);

    [XmlIgnore]
    public string BookPriceAsText
    {
      get
      {
        return (double) this.bookPrice >= 0.0 ? string.Format("{0:0.00}", (object) this.bookPrice) : ComicBook.unkownText.Value;
      }
      set
      {
        float result;
        if (float.TryParse(value, out result))
          this.BookPrice = result;
        else
          this.BookPrice = -1f;
      }
    }

    public string OpenedTimeAsText => ComicBook.FormatDate(this.OpenedTime);

    public string AddedTimeAsText
    {
      get => ComicBook.FormatDate(this.AddedTime, missingText: ComicBook.unkownText.Value);
    }

    public string ReleasedTimeAsText
    {
      get
      {
        return ComicBook.FormatDate(this.ReleasedTime, ComicDateFormat.Short, missingText: ComicBook.unkownText.Value);
      }
    }

    public string FileCreationTimeAsText => ComicBook.FormatFileDate(this.FileCreationTime);

    public string FileModifiedTimeAsText => ComicBook.FormatFileDate(this.FileModifiedTime);

    public bool IsInContainer => this.container != null;

    public ComicsEditModes EditMode
    {
      get => !this.IsInContainer ? ComicsEditModes.Default : this.Container.EditMode;
    }

    [DefaultValue(false)]
    [XmlAttribute]
    public bool IsDynamicSource { get; set; }

    [DefaultValue(0)]
    public int NewPages
    {
      get => this.newPages;
      set => this.SetProperty<int>(nameof (NewPages), ref this.newPages, value);
    }

    public string ProposedSeries
    {
      get
      {
        this.UpdateProposed();
        return this.proposed.Series;
      }
    }

    public string ProposedTitle
    {
      get
      {
        this.UpdateProposed();
        return this.proposed.Title;
      }
    }

    public string ProposedFormat
    {
      get
      {
        this.UpdateProposed();
        return this.proposed.Format;
      }
    }

    public int ProposedVolume
    {
      get
      {
        this.UpdateProposed();
        return this.proposed.Volume;
      }
    }

    public string ProposedNumber
    {
      get
      {
        this.UpdateProposed();
        return !(this.Number == "-") ? this.proposed.Number : string.Empty;
      }
    }

    public int ProposedCount
    {
      get
      {
        this.UpdateProposed();
        return this.proposed.Count;
      }
    }

    public int ProposedYear
    {
      get
      {
        this.UpdateProposed();
        return this.proposed.Year;
      }
    }

    public string ProposedYearAsText => ComicBook.FormatYear(this.ProposedYear);

    public string ProposedNumberAsText
    {
      get => ComicBook.FormatNumber(this.ProposedNumber, this.ProposedCount);
    }

    public string ProposedVolumeAsText => ComicBook.FormatVolume(this.ProposedVolume);

    public string ProposedNakedVolumeAsText
    {
      get => this.ProposedVolume != -1 ? this.ProposedVolume.ToString() : string.Empty;
    }

    public string ProposedCountAsText
    {
      get => this.ProposedCount != -1 ? this.ProposedCount.ToString() : string.Empty;
    }

    public string ShadowSeries
    {
      get
      {
        return !this.EnableProposed || !string.IsNullOrEmpty(this.Series) ? this.Series : this.ProposedSeries;
      }
    }

    public string ShadowTitle
    {
      get
      {
        return !this.EnableProposed || !string.IsNullOrEmpty(this.Title) ? this.Title : this.ProposedTitle;
      }
    }

    public string ShadowFormat
    {
      get
      {
        return !this.EnableProposed || !string.IsNullOrEmpty(this.Format) ? this.Format : this.ProposedFormat;
      }
    }

    public int ShadowVolume
    {
      get => !this.EnableProposed || this.Volume != -1 ? this.Volume : this.ProposedVolume;
    }

    public string ShadowNumber
    {
      get
      {
        return !this.EnableProposed || !string.IsNullOrEmpty(this.Number) ? this.Number : this.ProposedNumber;
      }
    }

    public int ShadowCount
    {
      get => !this.EnableProposed || this.Count != -1 ? this.Count : this.ProposedCount;
    }

    public int ShadowYear
    {
      get => !this.EnableProposed || this.Year != -1 ? this.Year : this.ProposedYear;
    }

    [Browsable(true)]
    public string ShadowYearAsText => ComicBook.FormatYear(this.ShadowYear);

    public string ShadowNumberAsText => ComicBook.FormatNumber(this.ShadowNumber, this.ShadowCount);

    public string ShadowVolumeAsText => ComicBook.FormatVolume(this.ShadowVolume);

    public string ShadowCountAsText
    {
      get => this.ShadowCount != -1 ? this.ShadowCount.ToString() : string.Empty;
    }

    public TextNumberFloat CompareNumber
    {
      get
      {
        return this.compareNumber ?? (this.compareNumber = (TextNumberFloat) new ComicBook.ComicTextNumberFloat(this.ShadowNumber));
      }
    }

    public TextNumberFloat CompareAlternateNumber
    {
      get
      {
        return this.compareAlternateNumber ?? (this.compareAlternateNumber = (TextNumberFloat) new ComicBook.ComicTextNumberFloat(this.AlternateNumber));
      }
    }

    private void ResetOptimizedNumbers()
    {
      this.compareNumber = this.compareAlternateNumber = (TextNumberFloat) null;
    }

    [DefaultValue(null)]
    public ExtraSyncInformation ExtraSyncInformation
    {
      get
      {
        using (ItemMonitor.Lock((object) ComicBook.syncInfo))
        {
          ExtraSyncInformation extraSyncInformation;
          return ComicBook.syncInfo.TryGetValue(this, out extraSyncInformation) ? extraSyncInformation : (ExtraSyncInformation) null;
        }
      }
      set
      {
        using (ItemMonitor.Lock((object) ComicBook.syncInfo))
          ComicBook.syncInfo[this] = value;
      }
    }

    public static void ClearExtraSyncInformation()
    {
      using (ItemMonitor.Lock((object) ComicBook.syncInfo))
        ComicBook.syncInfo.Clear();
    }

    [Browsable(true)]
    [DefaultValue("")]
    [ResetValue(0)]
    public string CustomValuesStore
    {
      get => this.customValuesStore;
      set
      {
        this.SetProperty<string>(nameof (CustomValuesStore), ref this.customValuesStore, value);
      }
    }

    public IEnumerable<StringPair> GetCustomValues()
    {
      return ValuesStore.GetValues(this.CustomValuesStore);
    }

    public void SetCustomValue(string key, string value)
    {
      if (string.IsNullOrEmpty(key))
        return;
      if (string.IsNullOrEmpty(value))
        this.DeleteCustomValue(key);
      else
        this.CustomValuesStore = new ValuesStore(this.CustomValuesStore).Add(key, value).ToString();
    }

    public string GetCustomValue(string key) => ValuesStore.GetValue(this.CustomValuesStore, key);

    public void DeleteCustomValue(string key)
    {
      this.CustomValuesStore = new ValuesStore(this.CustomValuesStore).Remove(key).ToString();
    }

    public ThumbnailKey GetThumbnailKey(int page)
    {
      string locationKey = !this.IsLinked ? (string.IsNullOrEmpty(this.CustomThumbnailKey) ? ThumbnailKey.GetResource("resource", "Unknown") : ThumbnailKey.GetResource("custom", this.CustomThumbnailKey)) : this.FileLocation;
      return this.GetThumbnailKey(page, locationKey);
    }

    public ThumbnailKey GetThumbnailKey(int page, string locationKey)
    {
      return new ThumbnailKey((object) this, locationKey, this.FileSize, this.FileModifiedTime, this.TranslatePageToImageIndex(page), this.GetPage(page).Rotation);
    }

    public ThumbnailKey GetFrontCoverThumbnailKey()
    {
      return this.GetThumbnailKey(this.FrontCoverPageIndex);
    }

    public PageKey GetFrontCoverKey(BitmapAdjustment bitmapAdjustment)
    {
      return this.GetPageKey(this.FrontCoverPageIndex, bitmapAdjustment);
    }

    public PageKey GetPageKey(int page, BitmapAdjustment bitmapAdjustment)
    {
      return new PageKey((object) this, this.FileLocation, this.FileSize, this.FileModifiedTime, this.TranslatePageToImageIndex(page), this.GetPage(page).Rotation, bitmapAdjustment);
    }

    public ImageKey GetImageKey(int image)
    {
      return (ImageKey) new PageKey((object) this, this.FileLocation, this.FileSize, this.FileModifiedTime, image, ImageRotation.None, BitmapAdjustment.Empty);
    }

    public ImageProvider CreateImageProvider()
    {
      CreateComicProviderEventArgs cpea = new CreateComicProviderEventArgs();
      this.OnCreateComicProvider(cpea);
      cpea.Provider = cpea.Provider ?? Providers.Readers.CreateSourceProvider(this.FilePath);
      this.OnComicProviderCreated(cpea);
      return cpea.Provider;
    }

    public ImageProvider OpenProvider(int lastPageIndexToRead = -1)
    {
      ImageProvider imageProvider = this.CreateImageProvider();
      try
      {
        if (lastPageIndexToRead != -1)
        {
          int imageIndex = this.TranslatePageToImageIndex(lastPageIndexToRead);
          imageProvider.ImageReady += (EventHandler<ImageIndexReadyEventArgs>) ((s, e) => e.Cancel = e.ImageNumber == imageIndex);
        }
        imageProvider.Open(false);
        return imageProvider;
      }
      catch
      {
        imageProvider?.Dispose();
        return (ImageProvider) null;
      }
    }

    public string GetPublisherIconKey()
    {
      string publisherIconKey = this.Publisher;
      if (this.Year >= 0)
        publisherIconKey = publisherIconKey + "(" + this.YearAsText + ")";
      return publisherIconKey;
    }

    public string GetImprintIconKey()
    {
      string imprintIconKey = this.Imprint;
      if (this.Year >= 0)
        imprintIconKey = imprintIconKey + "(" + this.YearAsText + ")";
      return imprintIconKey;
    }

    private IEnumerable<Image> GetIconsInternal()
    {
      Image img = ComicBook.PublisherIcons.GetImage(this.GetPublisherIconKey());
      if (img != null)
        yield return img;
      else if ((img = ComicBook.PublisherIcons.GetImage(this.Publisher)) != null)
        yield return img;
      if ((img = ComicBook.PublisherIcons.GetImage(this.GetImprintIconKey())) != null)
        yield return img;
      else if ((img = ComicBook.PublisherIcons.GetImage(this.Imprint)) != null)
        yield return img;
      if ((img = ComicBook.AgeRatingIcons.GetImage(this.AgeRating)) != null)
        yield return img;
      if ((img = ComicBook.FormatIcons.GetImage(this.ShadowFormat)) != null)
        yield return img;
      if (!string.IsNullOrEmpty(this.Tags))
      {
        foreach (string key in this.Tags.ListStringToSet(','))
        {
          if ((img = ComicBook.SpecialIcons.GetImage(key)) != null)
            yield return img;
        }
      }
      if (this.SeriesComplete == YesNo.Yes && (img = ComicBook.SpecialIcons.GetImage("SeriesComplete")) != null)
        yield return img;
      if (this.BlackAndWhite == YesNo.Yes && (img = ComicBook.SpecialIcons.GetImage("BlackAndWhite")) != null)
        yield return img;
      if (this.Manga == MangaYesNo.Yes && (img = ComicBook.SpecialIcons.GetImage("Manga")) != null)
        yield return img;
      if (this.Manga == MangaYesNo.YesAndRightToLeft && (img = ComicBook.SpecialIcons.GetImage("MangaRightToLeft")) != null)
        yield return img;
    }

    public IEnumerable<Image> GetIcons() => this.GetIconsInternal();

    public void CopyFrom(ComicBook cb)
    {
      this.SetInfo((ComicInfo) cb, false, true);
      this.Id = cb.Id;
      this.AddedTime = cb.AddedTime;
      this.ReleasedTime = cb.ReleasedTime;
      this.OpenedTime = cb.OpenedTime;
      this.OpenedCount = cb.OpenedCount;
      this.CurrentPage = cb.CurrentPage;
      this.LastPageRead = cb.LastPageRead;
      this.Rating = cb.Rating;
      this.ColorAdjustment = cb.ColorAdjustment;
      this.EnableDynamicUpdate = cb.EnableDynamicUpdate;
      this.EnableProposed = cb.EnableProposed;
      this.SeriesComplete = cb.SeriesComplete;
      this.Checked = cb.Checked;
      this.FilePath = cb.FilePath;
      this.FileSize = cb.FileSize;
      this.FileModifiedTime = cb.FileModifiedTime;
      this.FileCreationTime = cb.FileCreationTime;
      this.fileLocation = cb.FileLocation;
      this.customThumbnailKey = cb.CustomThumbnailKey;
      this.LastOpenedFromListId = cb.LastOpenedFromListId;
      this.CustomValuesStore = cb.CustomValuesStore;
    }

    public void CopyTo(ComicBook cb) => cb.CopyFrom(this);

    public string GetFullTitle(string textFormat, params string[] ignore)
    {
      try
      {
        return ExtendedStringFormater.Format(textFormat, (Func<string, object>) (s =>
        {
          try
          {
            if (ignore != null && ignore.Length != 0 && ((IEnumerable<string>) ignore).Contains<string>(s, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
              return (object) null;
            string newName;
            ComicBook.MapPropertyNameToAsText(s, out newName);
            return (object) this.GetPropertyValue<string>(newName, ComicValueType.Shadow);
          }
          catch
          {
            return (object) null;
          }
        }));
      }
      catch
      {
        return string.Empty;
      }
    }

    public void SetShadowValues(ComicInfo ci)
    {
      ci.Series = this.ShadowSeries;
      ci.Count = this.ShadowCount;
      ci.Title = this.ShadowTitle;
      ci.Year = this.ShadowYear;
      ci.Number = this.ShadowNumber;
      ci.Format = this.ShadowFormat;
      ci.Volume = this.ShadowVolume;
    }

    public void SetFileLocation(string fileLocation) => this.fileLocation = fileLocation;

    public void MarkAsNotRead()
    {
      this.OpenedTime = DateTime.MinValue;
      this.OpenedCount = this.LastPageRead = this.CurrentPage = 0;
    }

    public void MarkAsRead()
    {
      if (this.OpenedCount == 0)
        this.OpenedCount = 1;
      this.OpenedTime = DateTime.Now;
      if (this.PageCount <= 0)
        return;
      this.CurrentPage = this.LastPageRead = this.PageCount - 1;
    }

    public void ResetProperties(int level = 0)
    {
      ResetValueAttribute.ResetProperties((object) this, level);
    }

    public bool RemoveFromContainer()
    {
      ComicBookContainer container = this.Container;
      return container != null && container.Books.Remove(this);
    }

    public bool IsSearchable(string propName)
    {
      if (ComicBook.searchableProperties == null)
        ComicBook.searchableProperties = new HashSet<string>(((IEnumerable<PropertyInfo>) this.GetType().GetProperties()).Where<PropertyInfo>(new Func<PropertyInfo, bool>(SearchableAttribute.IsSearchable)).Select<PropertyInfo, string>((Func<PropertyInfo, string>) (pi => pi.Name)));
      return ComicBook.searchableProperties.Contains(propName);
    }

    public object GetUntypedPropertyValue(string propName)
    {
      return this.GetType().GetProperty(propName).GetValue((object) this, (object[]) null);
    }

    public T GetPropertyValue<T>(string propName, ComicValueType cvt = ComicValueType.Standard)
    {
      ComicBook.MapPropertyName(propName, out propName, cvt);
      try
      {
        if (!propName.StartsWith("{"))
          return PropertyCaller.CreateGetMethod<ComicBook, T>(propName)(this);
        propName = propName.Substring(1, propName.Length - 2);
        return (T) this.GetCustomValue(propName);
      }
      catch (Exception ex)
      {
        return default (T);
      }
    }

    public string GetStringPropertyValue(string propName, ComicValueType cvt = ComicValueType.Standard)
    {
      return this.GetPropertyValue<string>(propName, cvt) ?? string.Empty;
    }

    public T GetPropertyValue<T>(string propName, bool proposed)
    {
      T propertyValue = this.GetPropertyValue<T>(propName);
      return !proposed || !this.EnableProposed || !ComicBook.IsDefaultPropertyValue((object) propertyValue) || !ComicBook.MapPropertyName(propName, out propName, ComicValueType.Proposed) ? propertyValue : this.GetPropertyValue<T>(propName);
    }

    public string FormatString(string format)
    {
      return ComicBook.rxField.Replace(format, (MatchEvaluator) (m =>
      {
        try
        {
          return PropertyCaller.CreateGetMethod<ComicBook, string>(m.Groups[1].Value)(this) ?? string.Empty;
        }
        catch (Exception ex)
        {
          return m.Value;
        }
      }));
    }

    public void WriteProposedValues(bool overwriteAll)
    {
      if (overwriteAll || string.IsNullOrEmpty(this.Series))
        this.Series = this.ProposedSeries;
      if (overwriteAll || string.IsNullOrEmpty(this.Title))
        this.Title = this.ProposedTitle;
      if (overwriteAll || this.Year == -1)
        this.Year = this.ProposedYear;
      if (overwriteAll || string.IsNullOrEmpty(this.Number))
        this.Number = this.ProposedNumber;
      if (overwriteAll || this.Volume == -1)
        this.Volume = this.ProposedVolume;
      if (overwriteAll || this.Count == -1)
        this.Count = this.ProposedCount;
      if (overwriteAll || string.IsNullOrEmpty(this.Format))
        this.Format = this.ProposedFormat;
      this.EnableProposed = false;
    }

    public bool RenameFile(string newName)
    {
      if (this.IsLinked)
      {
        if (this.EditMode.IsLocalComic())
        {
          try
          {
            newName = FileUtility.MakeValidFilename(newName);
            string str = Path.Combine(this.FileDirectory, newName + Path.GetExtension(this.FilePath));
            if (string.Equals(this.FilePath, str, StringComparison.OrdinalIgnoreCase))
              return true;
            File.Move(this.FilePath, str);
            this.FilePath = str;
            return true;
          }
          catch
          {
            return false;
          }
        }
      }
      return false;
    }

    public ComicBookNavigator CreateNavigator()
    {
      try
      {
        ComicBookNavigator navigator = new ComicBookNavigator(this);
        switch (this.Manga)
        {
          case MangaYesNo.Unknown:
            navigator.RightToLeftReading = YesNo.Unknown;
            break;
          case MangaYesNo.No:
          case MangaYesNo.Yes:
            navigator.RightToLeftReading = YesNo.No;
            break;
          case MangaYesNo.YesAndRightToLeft:
            navigator.RightToLeftReading = YesNo.Yes;
            break;
        }
        return navigator;
      }
      catch
      {
        return (ComicBookNavigator) null;
      }
    }

    public void SetValue(string propertyName, object value)
    {
      try
      {
        this.GetType().GetProperty(propertyName).SetValue((object) this, value, (object[]) null);
      }
      catch (Exception ex)
      {
      }
    }

    public string Hash()
    {
      try
      {
        if (this.EditMode.IsLocalComic())
        {
          if (!string.IsNullOrEmpty(this.FilePath))
          {
            if (File.Exists(this.FilePath))
              return this.CreateFileHash();
          }
        }
      }
      catch
      {
      }
      return (string) null;
    }

    public void CopyDataFrom(ComicBook data, IEnumerable<string> properties)
    {
      Type type = data.GetType();
      foreach (string property1 in properties)
      {
        try
        {
          if (property1.StartsWith("{"))
          {
            string key = property1.Substring(1, property1.Length - 2);
            this.SetCustomValue(key, data.GetCustomValue(key));
          }
          else
          {
            PropertyInfo property2 = type.GetProperty(property1);
            property2.SetValue((object) this, property2.GetValue((object) data, (object[]) null), (object[]) null);
          }
        }
        catch
        {
        }
      }
    }

    public void ToClipboard()
    {
      ComicBook data1 = CloneUtility.Clone<ComicBook>(this);
      data1.Id = Guid.NewGuid();
      DataObject data2 = new DataObject();
      data2.SetData(DataFormats.UnicodeText, (object) this.GetInfo().ToXml());
      data2.SetData(nameof (ComicBook), (object) data1);
      Clipboard.SetDataObject((object) data2);
    }

    public bool IsDefaultValue(string property)
    {
      PropertyInfo property1 = this.GetType().GetProperty(property);
      return object.Equals(property1.GetValue((object) ComicBook.Default, (object[]) null), property1.GetValue((object) this, (object[]) null));
    }

    public void ValidateData()
    {
      this.TrimExcessPageInfo();
      if (this.FileIsMissing)
        return;
      if (!(this.FileCreationTime == DateTime.MinValue))
        return;
      try
      {
        this.FileCreationTime = File.GetCreationTimeUtc(this.FilePath);
      }
      catch (Exception ex)
      {
      }
    }

    public void UpdateDynamicPageCount(bool refresh, IProgressState ps = null)
    {
      try
      {
        using (ImageProvider sourceProvider = Providers.Readers.CreateSourceProvider(this.FilePath))
        {
          if (sourceProvider is IDynamicImages dynamicImages)
            dynamicImages.RefreshMode = refresh;
          if (ps != null)
            sourceProvider.ImageReady += (EventHandler<ImageIndexReadyEventArgs>) ((s, e) =>
            {
              ps.ProgressAvailable = this.PageCount > 0;
              if (!ps.ProgressAvailable)
                return;
              ps.ProgressPercentage = 100 * e.ImageNumber / this.PageCount;
            });
          sourceProvider.Open(false);
          this.NewPages = Math.Max(sourceProvider.Count - this.PageCount, 0);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public bool WriteInfoToFile(bool withRefreshFileProperties = true)
    {
      if (!this.EditMode.IsLocalComic())
        return false;
      FileInfo fileInfo = new FileInfo(this.FilePath);
      if (!fileInfo.Exists || fileInfo.IsReadOnly)
        return false;
      using (ImageProvider imageProvider = this.CreateImageProvider())
      {
        if (!(imageProvider is IInfoStorage infoStorage))
          return false;
        infoStorage.StoreInfo(this.GetInfo());
        this.FileInfoRetrieved = true;
      }
      if (withRefreshFileProperties)
        this.RefreshFileProperties();
      return true;
    }

    public void ResetInfoRetrieved() => this.FileInfoRetrieved = false;

    public void RefreshInfoFromFile(RefreshInfoOptions options)
    {
      if ((options & RefreshInfoOptions.DontReadInformation) != RefreshInfoOptions.None || !this.EditMode.IsLocalComic() || !this.IsLinked)
        return;
      DateTime fileModifiedTime = this.FileModifiedTime;
      long fileSize = this.FileSize;
      this.RefreshFileProperties();
      if (this.FileIsMissing)
        return;
      bool flag = this.FileModifiedTime != fileModifiedTime;
      try
      {
        this.IsDynamicSource = Providers.Readers.GetSourceProviderInfo(this.FilePath).Formats.All<cYo.Projects.ComicRack.Engine.IO.Provider.FileFormat>((Func<cYo.Projects.ComicRack.Engine.IO.Provider.FileFormat, bool>) (f => f.Dynamic));
      }
      catch (Exception ex)
      {
        this.IsDynamicSource = false;
      }
      if (!(!this.FileInfoRetrieved | flag) && (options & RefreshInfoOptions.ForceRefresh) == RefreshInfoOptions.None && ((options & (RefreshInfoOptions.GetFastPageCount | RefreshInfoOptions.GetPageCount)) == RefreshInfoOptions.None || this.PageCount != 0))
        return;
      using (ImageProvider imageProvider = this.CreateImageProvider())
      {
        if (!(imageProvider is IInfoStorage infoStorage))
          return;
        if (!this.ComicInfoIsDirty)
        {
          int method = flag || !this.FileInfoRetrieved ? 1 : 0;
          this.SetInfo(infoStorage.LoadInfo((InfoLoadingMethod) method), true, true);
        }
        if (!imageProvider.IsSlow)
        {
          if (this.PageCount != 0)
          {
            if (fileSize == this.FileSize)
              goto label_26;
          }
          if ((options & RefreshInfoOptions.GetFastPageCount) == RefreshInfoOptions.None || (imageProvider.Capabilities & ImageProviderCapabilities.FastPageInfo) == ImageProviderCapabilities.Nothing)
          {
            if ((options & RefreshInfoOptions.GetPageCount) == RefreshInfoOptions.None)
              goto label_26;
          }
          try
          {
            imageProvider.Open(false);
            if (imageProvider.Count > 0)
              this.PageCount = imageProvider.Count;
          }
          catch
          {
          }
        }
      }
label_26:
      this.FileInfoRetrieved = true;
    }

    public void RefreshInfoFromFile()
    {
      this.RefreshInfoFromFile(RefreshInfoOptions.GetFastPageCount);
    }

    public string CreateFileHash()
    {
      using (ImageProvider imageProvider = this.CreateImageProvider())
      {
        imageProvider.Open(false);
        return imageProvider.CreateHash();
      }
    }

    public void RefreshFileProperties()
    {
      if (!this.EditMode.IsLocalComic())
        return;
      try
      {
        FileInfo fileInfo = new FileInfo(this.FilePath);
        this.FileIsMissing = !fileInfo.Exists;
        if (!fileInfo.Exists)
          return;
        bool flag1 = this.fileSize != fileInfo.Length;
        bool flag2 = this.fileModifiedTime != fileInfo.LastWriteTimeUtc;
        this.fileSize = fileInfo.Length;
        this.fileModifiedTime = fileInfo.LastWriteTimeUtc;
        if (flag1)
          this.FireBookChanged("FileSize");
        if (flag2)
          this.FireBookChanged("FileModifiedTime");
        this.FileCreationTime = fileInfo.CreationTimeUtc;
      }
      catch
      {
        this.FileIsMissing = true;
      }
    }

    private bool SetProperty<T>(
      string name,
      ref T property,
      T value,
      bool lockItem = false,
      bool addUndo = true)
    {
      if (object.Equals((object) property, (object) value))
        return false;
      T oldValue = property;
      using (lockItem ? ItemMonitor.Lock((object) this) : (IDisposable) null)
        property = value;
      if (addUndo)
        this.FireBookChanged(name, (object) oldValue, (object) value);
      else
        this.FireBookChanged(name);
      return true;
    }

    private void FireBookChanged(string name)
    {
      this.OnBookChanged(new BookChangedEventArgs(name, false));
    }

    private void FireBookChanged(string name, object oldValue, object newValue)
    {
      this.OnBookChanged(new BookChangedEventArgs(name, false, oldValue, newValue));
    }

    private void UpdateProposed()
    {
      if (this.proposed != null)
        return;
      this.OnParseFilePath();
    }

    [field: NonSerialized]
    public event EventHandler<ComicBookFileRenameEventArgs> FileRenamed;

    [field: NonSerialized]
    public event EventHandler<CreateComicProviderEventArgs> CreateComicProvider;

    [field: NonSerialized]
    public event EventHandler<CreateComicProviderEventArgs> ComicProviderCreated;

    protected virtual void OnFileRenamed(ComicBookFileRenameEventArgs e)
    {
      if (this.FileRenamed == null)
        return;
      this.FileRenamed((object) this, e);
    }

    protected virtual void OnCreateComicProvider(CreateComicProviderEventArgs cpea)
    {
      if (this.CreateComicProvider == null)
        return;
      this.CreateComicProvider((object) this, cpea);
    }

    protected virtual void OnComicProviderCreated(CreateComicProviderEventArgs cpea)
    {
      if (this.ComicProviderCreated == null)
        return;
      this.ComicProviderCreated((object) this, cpea);
    }

    protected override void OnBookChanged(BookChangedEventArgs e)
    {
      base.OnBookChanged(e);
      if (e.PropertyName == "FilePath" || e.PropertyName == "Number" || e.PropertyName == "EnableProposed")
      {
        this.compareNumber = (TextNumberFloat) null;
      }
      else
      {
        if (!(e.PropertyName == "AlternateNumber"))
          return;
        this.compareAlternateNumber = (TextNumberFloat) null;
      }
    }

    public override void SetInfo(ComicInfo ci, bool onlyUpdateEmpty = true, bool updatePages = true)
    {
      base.SetInfo(ci, onlyUpdateEmpty, updatePages);
      this.LastPageRead = Math.Min(this.PageCount - 1, this.LastPageRead);
      this.CurrentPage = Math.Min(this.PageCount - 1, this.CurrentPage);
    }

    protected override ComicPageInfo OnNewComicPageAdded(ComicPageInfo info)
    {
      if (this.proposed != null && info.ImageIndex < this.proposed.CoverCount)
        info.PageType = ComicPageType.FrontCover;
      return info;
    }

    public static ComicBook DeserializeFull(Stream stream)
    {
      return XmlUtility.Load<ComicBook>(stream, false);
    }

    public static ComicBook DeserializeFull(string file) => XmlUtility.Load<ComicBook>(file, false);

    public void SerializeFull(Stream stream) => XmlUtility.Store(stream, (object) this, false);

    public void SerializeFull(string file) => XmlUtility.Store(file, (object) this, false);

    public static string FormatPages(int pages)
    {
      if (pages <= 0)
        return ComicBook.unkownText.Value;
      return StringUtility.Format(ComicBook.pagesText.Value, (object) pages);
    }

    public static string FormatRating(float rating)
    {
      return (double) rating > 0.0 ? rating.ToString("0.0") : ComicBook.noneText.Value;
    }

    public static string FormatYear(int year) => year != -1 ? year.ToString() : string.Empty;

    public static string FormatNumber(string number, int count)
    {
      if (string.IsNullOrEmpty(number))
        return string.Empty;
      string str = number == "-" ? string.Empty : number;
      if (count >= 0)
        str += StringUtility.Format(" ({0} {1})", (object) ComicBook.ofText.Value, (object) count);
      return str;
    }

    public static string FormatVolume(int volume)
    {
      if (volume == -1)
        return string.Empty;
      return StringUtility.Format(ComicBook.volumeFormat.Value, (object) volume);
    }

    public static string FormatTitle(
      string textFormat,
      string series,
      string title = null,
      string volumeText = null,
      string numberText = null,
      string yearText = null,
      string monthText = null,
      string dayText = null,
      string format = null,
      string fileName = null)
    {
      if (!string.IsNullOrEmpty(series))
      {
        try
        {
          return ExtendedStringFormater.Format(textFormat, (Func<string, object>) (s =>
          {
            switch (s)
            {
              case "day":
                return (object) dayText;
              case "filename":
                return (object) fileName;
              case nameof (format):
                return !series.Contains(format, StringComparison.OrdinalIgnoreCase) ? (object) format : (object) string.Empty;
              case "month":
                return (object) monthText;
              case "number":
                return (object) numberText;
              case nameof (series):
                return (object) series;
              case nameof (title):
                return (object) title;
              case "volume":
                return (object) volumeText;
              case "year":
                return (object) yearText;
              default:
                return (object) null;
            }
          })).Trim();
        }
        catch
        {
        }
      }
      return fileName ?? string.Empty;
    }

    private static void AppendString(StringBuilder s, string delimiter, string text)
    {
      if (string.IsNullOrEmpty(text))
        return;
      if (s.Length != 0)
        s.Append(delimiter);
      s.Append(text);
    }

    public static string FormatDate(
      DateTime date,
      ComicDateFormat dateFormat = ComicDateFormat.Long,
      bool toLocal = false,
      string missingText = null)
    {
      if (date == DateTime.MinValue)
        return missingText ?? ComicBook.neverText.Value;
      if (toLocal)
        date = date.ToLocalTime();
      switch (dateFormat)
      {
        case ComicDateFormat.Short:
          return date.ToShortDateString();
        case ComicDateFormat.Relative:
          return date.ToRelativeDateString(DateTime.Now);
        default:
          return !date.IsDateOnly() ? date.ToString() : date.ToShortDateString();
      }
    }

    public static string FormatFileDate(DateTime date, ComicDateFormat dateFormat = ComicDateFormat.Long)
    {
      return ComicBook.FormatDate(date, dateFormat, true, ComicBook.notFoundText.Value);
    }

    public static string GetLanguageName(string iso)
    {
      try
      {
        return ComicBook.GetIsoCulture(iso).DisplayName;
      }
      catch
      {
        return string.Empty;
      }
    }

    public static CultureInfo GetIsoCulture(string iso)
    {
      iso = iso.ToLower();
      if (ComicBook.languages == null)
        ComicBook.languages = new Dictionary<string, CultureInfo>();
      Dictionary<string, CultureInfo> dictionary = new Dictionary<string, CultureInfo>();
      CultureInfo isoCulture;
      if (ComicBook.languages.TryGetValue(iso, out isoCulture))
        return isoCulture;
      CultureInfo cultureInfo = ((IEnumerable<CultureInfo>) CultureInfo.GetCultures(CultureTypes.NeutralCultures)).FirstOrDefault<CultureInfo>((Func<CultureInfo, bool>) (info => info.TwoLetterISOLanguageName == iso));
      return cultureInfo != null ? (ComicBook.languages[iso] = cultureInfo) : new CultureInfo(string.Empty);
    }

    public static IEnumerable<string> GetProperties(bool onlyWritable, Type t = null)
    {
      return ((IEnumerable<PropertyInfo>) typeof (ComicBook).GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (pi => pi.CanRead && (pi.CanWrite || !onlyWritable) && (t == (Type) null || pi.PropertyType == t) && pi.Browsable(true))).Select<PropertyInfo, string>((Func<PropertyInfo, string>) (pi => pi.Name));
    }

    public static IEnumerable<string> GetWritableStringProperties()
    {
      return ComicBook.GetProperties(true, typeof (string));
    }

    public static IDictionary<string, string> GetTranslatedWritableStringProperties()
    {
      TR tr = TR.Load("Columns");
      return (IDictionary<string, string>) ComicBook.GetWritableStringProperties().ToDictionary<string, string>((Func<string, string>) (s => tr[s].PascalToSpaced()));
    }

    public static bool MapPropertyName(string propName, out string newName, ComicValueType cvt)
    {
      string lower = propName.ToLower();
      if (lower == "cover" || lower == "rating")
        propName += "AsText";
      if (cvt != ComicValueType.Standard)
      {
        switch (lower)
        {
          case "count":
          case "countastext":
          case "format":
          case "number":
          case "numberastext":
          case "series":
          case "title":
          case "volumeastext":
          case "year":
          case "yearastext":
            newName = (cvt == ComicValueType.Proposed ? "Proposed" : "Shadow") + propName;
            return true;
        }
      }
      newName = propName;
      return false;
    }

    public static bool MapPropertyNameToAsText(string propName, out string newName)
    {
      if (ComicBook.hasAsText.TryGetValue(propName, out newName))
        return true;
      string name = propName + "AsText";
      if (typeof (ComicBook).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public) != (PropertyInfo) null)
        propName = name;
      ComicBook.hasAsText[propName] = propName;
      newName = propName;
      return true;
    }

    public static bool IsDefaultPropertyValue(object value)
    {
      switch (value)
      {
        case null:
          return true;
        case string _:
          return string.IsNullOrEmpty((string) value);
        case int _:
        case double _:
        case float _:
          return (int) value == -1;
        case DateTime dateTime:
          return dateTime == DateTime.MinValue;
        default:
          return false;
      }
    }

    protected virtual void OnParseFilePath()
    {
      ComicBook.ParseFilePathEventArgs e = new ComicBook.ParseFilePathEventArgs(this.FilePath);
      if (ComicBook.ParseFilePath != null)
        ComicBook.ParseFilePath((object) this, e);
      this.proposed = e.NameInfo;
    }

    public static event EventHandler<ComicBook.ParseFilePathEventArgs> ParseFilePath;

    public static ImagePackage PublisherIcons => ComicBook.publisherIcons;

    public static ImagePackage AgeRatingIcons => ComicBook.ageRatingIcons;

    public static ImagePackage FormatIcons => ComicBook.formatIcons;

    public static ImagePackage SpecialIcons => ComicBook.specialIcons;

    public static bool NewBooksChecked { get; set; }

    public object Clone() => (object) new ComicBook(this);

    private class ComicTextNumberFloat : TextNumberFloat
    {
      public ComicTextNumberFloat(string text)
        : base(text)
      {
      }

      protected override void OnParseText(string s)
      {
        if (s == "1/2")
          this.Number = 0.5f;
        else
          base.OnParseText(s);
      }
    }

    public class ParseFilePathEventArgs : EventArgs
    {
      private readonly string path;
      private readonly ComicNameInfo nameInfo;

      public ParseFilePathEventArgs(string path)
      {
        this.path = path;
        this.nameInfo = ComicNameInfo.FromFilePath(path);
      }

      public string Path => this.path;

      public ComicNameInfo NameInfo => this.nameInfo;
    }
  }
}

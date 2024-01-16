// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.Forms.ComicDisplayControl
// Assembly: ComicRack.Engine.Display.Forms, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: D83BAE4E-CA55-445A-AD1D-2DF78C341143
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.Display.Forms.dll

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Presentation;
using cYo.Common.Presentation.Panels;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Display.Forms.Properties;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display.Forms
{
  public class ComicDisplayControl : ImageDisplayControl, IComicDisplay, IComicDisplayConfig
  {
    public const int DefaultFadeTime = 100;
    private static readonly int NavigationOverlayDefaultHeight = FormUtility.ScaleDpiY(200);
    private static readonly System.Drawing.Size pageInfoSize = new System.Drawing.Size(150, 25).ScaleDpi();
    private static readonly System.Drawing.Size partInfoSize = new System.Drawing.Size(200, 200).ScaleDpi();
    private static readonly System.Drawing.Size loadInfoSize = new System.Drawing.Size(200, 30).ScaleDpi();
    private static readonly System.Drawing.Size messageSize = new System.Drawing.Size(300, 30).ScaleDpi();
    public static readonly Cursor EmptyCursor = new Cursor((Stream) new MemoryStream(Resources.EmptyCursor));
    private readonly OverlayManager overlayManager;
    private readonly TextOverlay currentPageOverlay;
    private readonly TextOverlay loadPageOverlay;
    private readonly TextOverlay messageOverlay;
    private readonly OverlayPanel visiblePartOverlay;
    private readonly OverlayPanel magnifierOverlay;
    private readonly NavigationOverlay navigationOverlay;
    private readonly OverlayPanel gestureOverlay;
    private bool firstPageHasBeenLoaded = true;
    private bool preCache = true;
    private bool navigationOverlayVisible;
    private volatile IPagePool pagePool;
    private ComicBookNavigator book;
    private readonly Color blindOutColor = Color.FromArgb(192, Color.Black);
    private bool blindOut;
    private bool showStatusMessage;
    private bool leftRightMovementReversed;
    private volatile PageLayoutMode pageLayout;
    private volatile float doublePageOverlap;
    private float magnifierZoom = 2f;
    private bool magnifierVisible;
    private bool autoHideMagnifier = true;
    private bool autoMagnifier = true;
    private bool realisticPages = true;
    private float infoOverlayScaling = 1f;
    private InfoOverlays visibleInfoOverlays;
    private PageTransitionEffect pageTransitionEffect = PageTransitionEffect.Fade;
    private bool disableBlending;
    private bool softwareFiltering = true;
    private MagnifierStyle magnifierStyle;
    private Bitmap paperTextureBitmap;
    private ImageLayout paperTextureLayout;
    private float paperTextureStrength = 1f;
    private string paperTexture;
    private int displayHash;
    private volatile int currentPage;
    private int currentMousePage = -1;
    private Bitmap workingPaperTexture;
    private static readonly Bitmap pageBowLeft = PageRendering.CreatePageBow(new System.Drawing.Size(256, 256), 180f);
    private static readonly Bitmap pageBowRight = PageRendering.CreatePageBow(new System.Drawing.Size(256, 256), 0.0f);
    private Bitmap shadowBitmap;
    private float innerBowLeftOffsetInPercent;
    private float innerBowRightOffsetInPercent;
    private int[] displayedPages = new int[0];
    private Rectangle[] displayedPageAreas = new Rectangle[0];
    private RectangleF displayedPageBounds;
    private PageKey lastValidKey;
    private bool drawBlankPagesOverride;
    private cYo.Common.Collections.Cache<ComicDisplayControl.ScaledPageKey, ComicDisplayControl.ScaledPageItem> scaledCache;
    private System.Drawing.Point mouseDown;
    private bool panMagnifier;
    private int cachedPartOverlay = -1;
    private System.Drawing.Point cachedPartOffset;
    private int currentPageOverlayHash;
    private static ComicDisplayControl.Magnifier[] magnifiers = new ComicDisplayControl.Magnifier[2]
    {
      new ComicDisplayControl.Magnifier()
      {
        Bitmap = Resources.Magnifier,
        Inner = new Rectangle(20, 20, 573, 327),
        Outer = new Rectangle(5, 5, 592, 350)
      },
      new ComicDisplayControl.Magnifier()
      {
        Bitmap = Resources.MagnifierLight,
        Inner = new Rectangle(6, 5, 102, 56),
        Outer = new Rectangle(3, 2, 108, 61)
      }
    };
    private RectangleF partRect;
    private int currentPartHash;
    private Bitmap smallBitmap;
    private long lastBlend;
    private const bool moveAnim = true;
    private bool inBlendAnmation;
    private IContainer components;
    private System.Windows.Forms.Timer imageScaleTimer;
    private System.Windows.Forms.Timer longClickTimer;
    private System.Windows.Forms.Timer cacheUpdateTimer;

    public ComicDisplayControl()
    {
      this.InitializeComponent();
      this.cacheUpdateTimer.Interval = EngineConfiguration.Default.PageCachingDelay;
      this.overlayManager = new OverlayManager((Control) this)
      {
        AnimationEnabled = true
      };
      this.components.Add((IComponent) this.overlayManager);
      this.magnifierOverlay = new OverlayPanel(new System.Drawing.Size(300, 200).ScaleDpi())
      {
        Opacity = 1f,
        Visible = false,
        Enabled = false
      };
      this.magnifierOverlay.RenderSurface += new EventHandler<PanelRenderEventArgs>(this.magnifierOverlay_RenderSurface);
      this.overlayManager.Panels.Add(this.magnifierOverlay);
      System.Drawing.Size partInfoSize = ComicDisplayControl.partInfoSize;
      int width1 = partInfoSize.Width;
      partInfoSize = ComicDisplayControl.partInfoSize;
      int height1 = partInfoSize.Height;
      Animator[] animatorArray = new Animator[1]
      {
        (Animator) new FadeAnimator(100, 2000, 100)
      };
      this.visiblePartOverlay = new OverlayPanel(width1, height1, ContentAlignment.BottomRight, (IEnumerable<Animator>) animatorArray)
      {
        Opacity = 0.0f,
        AutoAlign = true,
        Enabled = true,
        HitTestType = cYo.Common.Presentation.Panels.HitTestType.Disabled
      };
      this.visiblePartOverlay.Drawing += new EventHandler(this.visiblePartOverlay_Drawing);
      this.visiblePartOverlay.RenderSurface += new EventHandler<PanelRenderEventArgs>(this.visiblePartOverlay_RenderSurface);
      if (!EngineConfiguration.Default.HideVisiblePartOverlayClose)
      {
        SimpleButtonPanel simpleButtonPanel1 = new SimpleButtonPanel(new System.Drawing.Size(16, 16).ScaleDpi());
        simpleButtonPanel1.Margin = Padding.Empty;
        simpleButtonPanel1.Alignment = ContentAlignment.TopRight;
        simpleButtonPanel1.Icon = (ScalableBitmap) Resources.Close;
        simpleButtonPanel1.AutoAlign = true;
        simpleButtonPanel1.AlignmentOffset = new System.Drawing.Point(-3, -2).ScaleDpi();
        SimpleButtonPanel simpleButtonPanel2 = simpleButtonPanel1;
        simpleButtonPanel2.Click += (EventHandler) ((s, e) =>
        {
          this.visiblePartOverlay.Opacity = 0.0f;
          this.VisibleInfoOverlays &= ~InfoOverlays.PartInfo;
        });
        this.visiblePartOverlay.Panels.Add((OverlayPanel) simpleButtonPanel2);
      }
      this.overlayManager.Panels.Add(this.visiblePartOverlay);
      int width2 = ComicDisplayControl.pageInfoSize.Width;
      System.Drawing.Size size = ComicDisplayControl.pageInfoSize;
      int height2 = size.Height;
      Font font1 = this.Font;
      TextOverlay textOverlay1 = new TextOverlay(width2, height2, ContentAlignment.TopRight, font1);
      textOverlay1.Enabled = false;
      textOverlay1.Opacity = 0.0f;
      textOverlay1.AutoAlign = true;
      textOverlay1.Html = true;
      this.currentPageOverlay = textOverlay1;
      this.currentPageOverlay.Animators.Add((Animator) new FadeAnimator(100, 2000, 100));
      this.overlayManager.Panels.Add((OverlayPanel) this.currentPageOverlay);
      size = ComicDisplayControl.loadInfoSize;
      int width3 = size.Width;
      size = ComicDisplayControl.loadInfoSize;
      int height3 = size.Height;
      Font font2 = this.Font;
      TextOverlay textOverlay2 = new TextOverlay(width3, height3, ContentAlignment.MiddleCenter, font2);
      textOverlay2.Enabled = false;
      textOverlay2.AutoAlign = true;
      textOverlay2.Visible = false;
      this.loadPageOverlay = textOverlay2;
      this.overlayManager.Panels.Add((OverlayPanel) this.loadPageOverlay);
      size = ComicDisplayControl.messageSize;
      int width4 = size.Width;
      size = ComicDisplayControl.messageSize;
      int height4 = size.Height;
      Font font3 = this.Font;
      TextOverlay textOverlay3 = new TextOverlay(width4, height4, ContentAlignment.MiddleCenter, font3);
      textOverlay3.Enabled = false;
      textOverlay3.AutoAlign = true;
      textOverlay3.Visible = false;
      this.messageOverlay = textOverlay3;
      this.overlayManager.Panels.Add((OverlayPanel) this.messageOverlay);
      NavigationOverlay navigationOverlay = new NavigationOverlay(new System.Drawing.Size(500, ComicDisplayControl.NavigationOverlayDefaultHeight).ScaleDpi());
      navigationOverlay.Visible = false;
      this.navigationOverlay = navigationOverlay;
      this.navigationOverlay.Browse += (EventHandler<BrowseEventArgs>) ((x, y) => this.OnBrowse(y));
      this.overlayManager.Panels.Add((OverlayPanel) this.navigationOverlay);
      GestureOverlay gestureOverlay = new GestureOverlay();
      gestureOverlay.Enabled = false;
      gestureOverlay.Opacity = 0.0f;
      gestureOverlay.AutoAlign = true;
      gestureOverlay.IgnoreParentMargin = true;
      this.gestureOverlay = (OverlayPanel) gestureOverlay;
      this.gestureOverlay.Animators.Add((Animator) new FadeAnimator(0, 500, 100));
      this.overlayManager.Panels.Add(this.gestureOverlay);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.Book = (ComicBookNavigator) null;
      this.overlayManager.Panels.Dispose();
      this.overlayManager.Dispose();
      base.Dispose(disposing);
    }

    [DefaultValue(true)]
    public bool PreCache
    {
      get => this.preCache;
      set => this.preCache = value;
    }

    [DefaultValue(false)]
    public bool NavigationOverlayVisible
    {
      get => this.navigationOverlayVisible;
      set
      {
        if (this.navigationOverlayVisible == value)
          return;
        this.navigationOverlayVisible = value;
        this.UpdateNavigationOverlay();
      }
    }

    [DefaultValue(null)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IPagePool PagePool
    {
      get => this.pagePool;
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        if (this.pagePool == value)
          return;
        if (this.pagePool != null)
          this.pagePool.PageCached -= new EventHandler<CacheItemEventArgs<ImageKey, PageImage>>(this.MemoryPageCache_ItemAdded);
        this.pagePool = value;
        value.PageCached += new EventHandler<CacheItemEventArgs<ImageKey, PageImage>>(this.MemoryPageCache_ItemAdded);
      }
    }

    [DefaultValue(null)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IThumbnailPool ThumbnailPool
    {
      get => this.navigationOverlay.Pool;
      set => this.navigationOverlay.Pool = value;
    }

    [DefaultValue(null)]
    public ComicBookNavigator Book
    {
      get => this.book;
      set
      {
        ComicBookNavigator comicBookNavigator = value;
        ComicBookNavigator book = this.Book;
        if (book == value)
          return;
        this.StopPendingImageCacheUpdate();
        if (book != null)
        {
          book.Disposing -= new EventHandler(this.book_Disposing);
          book.Navigation -= new EventHandler<BookPageEventArgs>(this.book_Navigation);
          book.IndexOfPageReady -= new EventHandler<BookPageEventArgs>(this.book_IndexOfPageReady);
          book.ColorAdjustmentChanged -= new EventHandler(this.book_ColorAdjustmentChanged);
          book.RightToLeftReadingChanged -= new EventHandler(this.book_RightToLeftReadingChanged);
          book.IndexRetrievalCompleted -= new EventHandler(this.book_IndexRetrievalCompleted);
          book.PageFilterChanged -= new EventHandler(this.book_PageFilterOrPagesChanged);
          book.PagesChanged -= new EventHandler(this.book_PageFilterOrPagesChanged);
          book.Comic.BookChanged -= new EventHandler<BookChangedEventArgs>(this.Comic_BookChanged);
          book.PagePart = this.ImageVisiblePart;
          book.RightToLeftReading = this.RightToLeftReading ? YesNo.Yes : YesNo.No;
        }
        this.book = comicBookNavigator;
        this.OnBookChanged();
        this.lastValidKey = (PageKey) null;
        if (comicBookNavigator != null)
        {
          comicBookNavigator.Disposing += new EventHandler(this.book_Disposing);
          comicBookNavigator.Navigation += new EventHandler<BookPageEventArgs>(this.book_Navigation);
          comicBookNavigator.ColorAdjustmentChanged += new EventHandler(this.book_ColorAdjustmentChanged);
          comicBookNavigator.RightToLeftReadingChanged += new EventHandler(this.book_RightToLeftReadingChanged);
          comicBookNavigator.IndexRetrievalCompleted += new EventHandler(this.book_IndexRetrievalCompleted);
          comicBookNavigator.IndexOfPageReady += new EventHandler<BookPageEventArgs>(this.book_IndexOfPageReady);
          comicBookNavigator.PageFilterChanged += new EventHandler(this.book_PageFilterOrPagesChanged);
          comicBookNavigator.PagesChanged += new EventHandler(this.book_PageFilterOrPagesChanged);
          comicBookNavigator.Comic.BookChanged += new EventHandler<BookChangedEventArgs>(this.Comic_BookChanged);
          this.CurrentPage = comicBookNavigator.CurrentPage;
          this.ImageVisiblePart = comicBookNavigator.PagePart;
          if (comicBookNavigator.RightToLeftReading != YesNo.Unknown)
            this.RightToLeftReading = comicBookNavigator.RightToLeftReading == YesNo.Yes;
        }
        this.navigationOverlay.Provider = (IImageProvider) comicBookNavigator;
        this.navigationOverlay.ImageKeyProvider = (IImageKeyProvider) comicBookNavigator;
        this.UpdateNavigationOverlay(true);
        this.Invalidate();
      }
    }

    [DefaultValue(typeof (Color), "192, 0, 0, 0")]
    public Color BlindOutColor
    {
      get => this.blindOutColor;
      set
      {
        if (this.blindOutColor == value || !this.blindOut)
          return;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool BlindOut
    {
      get => this.blindOut;
      set
      {
        if (this.blindOut == value)
          return;
        this.blindOut = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool ShowStatusMessage
    {
      get => this.showStatusMessage;
      set
      {
        if (this.showStatusMessage == value)
          return;
        this.showStatusMessage = value;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool LeftRightMovementReversed
    {
      get => this.leftRightMovementReversed;
      set
      {
        if (this.leftRightMovementReversed == value)
          return;
        this.leftRightMovementReversed = value;
        this.OnReadingModeChanged();
      }
    }

    [DefaultValue(PageLayoutMode.Single)]
    public PageLayoutMode PageLayout
    {
      get => this.pageLayout;
      set
      {
        if (this.pageLayout == value)
          return;
        this.pageLayout = value;
        this.OnDisplayChanged();
        this.Invalidate();
      }
    }

    [Browsable(false)]
    [DefaultValue(0.0f)]
    public float DoublePageOverlap
    {
      get => this.doublePageOverlap;
      set
      {
        if ((double) this.doublePageOverlap == (double) value)
          return;
        this.doublePageOverlap = value;
      }
    }

    [DefaultValue(2f)]
    public float MagnifierZoom
    {
      get => this.magnifierZoom;
      set
      {
        if ((double) this.magnifierZoom == (double) value)
          return;
        this.magnifierZoom = value;
        if (!this.MagnifierVisible)
          return;
        this.Invalidate();
      }
    }

    [DefaultValue(false)]
    public bool MagnifierVisible
    {
      get => this.magnifierVisible;
      set
      {
        if (this.magnifierVisible == value)
          return;
        this.magnifierVisible = value;
        this.UpdateMagnifierVisibility();
      }
    }

    [DefaultValue(1f)]
    public float MagnifierOpacity
    {
      get => this.magnifierOverlay.Opacity;
      set => this.magnifierOverlay.Opacity = value;
    }

    [DefaultValue(typeof (System.Drawing.Size), "200, 200")]
    public System.Drawing.Size MagnifierSize
    {
      get => this.magnifierOverlay.Size;
      set => this.magnifierOverlay.Size = value;
    }

    [DefaultValue(true)]
    public bool AutoHideMagnifier
    {
      get => this.autoHideMagnifier;
      set
      {
        if (this.autoHideMagnifier == value)
          return;
        this.autoHideMagnifier = value;
        this.UpdateMagnifierVisibility();
      }
    }

    [DefaultValue(true)]
    public bool AutoMagnifier
    {
      get => this.autoMagnifier;
      set
      {
        if (this.autoMagnifier == value)
          return;
        this.autoMagnifier = value;
        this.UpdateMagnifierVisibility();
      }
    }

    [DefaultValue(true)]
    public bool RealisticPages
    {
      get => this.realisticPages;
      set
      {
        if (this.realisticPages == value)
          return;
        this.realisticPages = value;
        this.Invalidate();
      }
    }

    [DefaultValue(1f)]
    public float InfoOverlayScaling
    {
      get => this.infoOverlayScaling;
      set
      {
        if ((double) this.infoOverlayScaling == (double) value)
          return;
        this.infoOverlayScaling = value;
        this.currentPageOverlay.Scale = this.visiblePartOverlay.Scale = this.loadPageOverlay.Scale = this.messageOverlay.Scale = this.infoOverlayScaling;
        this.navigationOverlay.Size = this.CalcNavigationOverlaySize();
      }
    }

    [DefaultValue(InfoOverlays.None)]
    public InfoOverlays VisibleInfoOverlays
    {
      get => this.visibleInfoOverlays;
      set
      {
        if (this.visibleInfoOverlays == value)
          return;
        this.visibleInfoOverlays = value;
        this.UpdateNavigationOverlay();
        this.OnVisibleInfoOverlaysChanged();
      }
    }

    [DefaultValue(PageTransitionEffect.Fade)]
    public PageTransitionEffect PageTransitionEffect
    {
      get => this.pageTransitionEffect;
      set => this.pageTransitionEffect = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComicDisplayControl.BlendAnimationHandler Blender { get; set; }

    [DefaultValue(false)]
    public bool DisableBlending
    {
      get => this.disableBlending;
      set => this.disableBlending = value;
    }

    [DefaultValue(true)]
    public bool SoftwareFiltering
    {
      get => this.softwareFiltering;
      set => this.softwareFiltering = value;
    }

    public bool IsFlipped
    {
      get
      {
        return this.RightToLeftReading && this.RightToLeftReadingMode == RightToLeftReadingMode.FlipParts;
      }
    }

    public bool IsMovementFlipped => this.LeftRightMovementReversed && this.IsFlipped;

    [DefaultValue(false)]
    public bool BlendWhilePaging { get; set; }

    [DefaultValue(MagnifierStyle.Glass)]
    public MagnifierStyle MagnifierStyle
    {
      get => this.magnifierStyle;
      set => this.magnifierStyle = value;
    }

    public Color BlankPageColor => EngineConfiguration.Default.BlankPageColor;

    [Category("Appearance")]
    [Description("Paper overlay texture")]
    [DefaultValue(null)]
    public Bitmap PaperTextureBitmap
    {
      get => this.paperTextureBitmap;
      set
      {
        if (this.paperTextureBitmap == value)
          return;
        this.paperTextureBitmap = value;
        this.CreateWorkingPaperTexture();
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [Description("Paper texture layout")]
    [DefaultValue(null)]
    public ImageLayout PaperTextureLayout
    {
      get => this.paperTextureLayout;
      set
      {
        if (this.paperTextureLayout == value)
          return;
        this.paperTextureLayout = value;
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [Description("Paper texture alpha value")]
    [DefaultValue(1f)]
    public float PaperTextureStrength
    {
      get => this.paperTextureStrength;
      set
      {
        if ((double) value == (double) this.paperTextureStrength)
          return;
        this.paperTextureStrength = value;
        this.CreateWorkingPaperTexture();
        this.Invalidate();
      }
    }

    [Category("Appearance")]
    [Description("Paper texture file")]
    [DefaultValue(null)]
    public string PaperTexture
    {
      get => this.paperTexture;
      set
      {
        if (this.paperTexture == value)
          return;
        this.paperTexture = value;
        Image paperTextureBitmap = (Image) this.PaperTextureBitmap;
        try
        {
          this.PaperTextureBitmap = (Bitmap) Image.FromFile(this.paperTexture);
        }
        catch (Exception ex)
        {
          this.PaperTextureBitmap = (Bitmap) null;
        }
        paperTextureBitmap.SafeDispose();
        this.OnImageDisplayOptionsChanged();
        this.Invalidate();
      }
    }

    public override bool IsValid => this.Book != null && this.PagePool != null;

    public int DisplayHash => this.displayHash;

    public int CurrentPage
    {
      get => this.currentPage;
      protected set => this.currentPage = value;
    }

    public int CurrentMousePage => this.currentMousePage;

    protected int NextPage => !this.TwoPageDisplay ? -1 : this.SeekPage(this.CurrentPage, 1);

    public bool TwoPageDisplay => this.PageLayout != 0;

    public bool ShouldPagingBlend { get; private set; }

    public event EventHandler BookChanged;

    public event EventHandler DrawnPageCountChanged;

    public event EventHandler<BrowseEventArgs> Browse;

    public event EventHandler<BookPageEventArgs> PageChange;

    public event EventHandler<BookPageEventArgs> PageChanged;

    public event EventHandler VisibleInfoOverlaysChanged;

    protected virtual void OnBookChanged()
    {
      if (this.BookChanged == null)
        return;
      this.BookChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnDrawnPageCountChanged()
    {
      this.UpdateCurrentPageOverlay();
      if (this.DrawnPageCountChanged == null)
        return;
      this.DrawnPageCountChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnBrowse(BrowseEventArgs e)
    {
      if (this.Browse == null)
        return;
      this.Browse((object) this, e);
    }

    protected virtual void OnPageChange(BookPageEventArgs e)
    {
      if (this.PageChange == null)
        return;
      this.PageChange((object) this, e);
    }

    protected virtual void OnPageChanged(BookPageEventArgs e)
    {
      this.currentMousePage = -1;
      if (this.PageChanged == null)
        return;
      this.PageChanged((object) this, e);
    }

    protected virtual void OnVisibleInfoOverlaysChanged()
    {
      if (this.VisibleInfoOverlaysChanged == null)
        return;
      this.VisibleInfoOverlaysChanged((object) this, EventArgs.Empty);
    }

    private void CreateWorkingPaperTexture()
    {
      this.workingPaperTexture.SafeDispose();
      this.workingPaperTexture = (Bitmap) null;
      if (this.paperTextureBitmap == null || (double) this.paperTextureStrength < 0.05000000074505806)
        return;
      this.workingPaperTexture = new Bitmap(this.paperTextureBitmap.Width, this.paperTextureBitmap.Height);
      using (Graphics graphics = Graphics.FromImage((Image) this.workingPaperTexture))
      {
        graphics.Clear(Color.White);
        graphics.DrawImage((Image) this.PaperTextureBitmap, 0, 0, this.paperTextureStrength);
      }
    }

    private int GetPageFromPoint(System.Drawing.Point pt)
    {
      pt = this.ClientToImage(pt, false);
      for (int index = 0; index < this.DisplayedPageAreas.Length; ++index)
      {
        if (this.DisplayedPageAreas[index].Contains(pt))
          return this.DisplayedPages[index];
      }
      return this.CurrentPage;
    }

    protected override bool MouseHandled
    {
      get => this.overlayManager.MouseHandled || this.MouseActionHappened;
    }

    private System.Drawing.Size CalcNavigationOverlaySize()
    {
      return new System.Drawing.Size(Math.Max(200, (int) ((double) this.ClientRectangle.Width * (double) (EngineConfiguration.Default.NavigationPanelWidth * this.infoOverlayScaling).Clamp(0.2f, 1f))), Math.Max((int) ((double) ComicDisplayControl.NavigationOverlayDefaultHeight * (double) this.infoOverlayScaling), 120));
    }

    private static void DrawPageBow(IBitmapRenderer gr, RectangleF rect, bool rightSide)
    {
      rect.Inflate(2f, 0.0f);
      gr.DrawImage((RendererImage) (rightSide ? ComicDisplayControl.pageBowRight : ComicDisplayControl.pageBowLeft), rect, new RectangleF(0.0f, 0.0f, (float) (ComicDisplayControl.pageBowLeft.Width - 1), (float) (ComicDisplayControl.pageBowLeft.Height - 1)), BitmapAdjustment.Empty, 1f);
    }

    private RectangleF DrawPageOrnaments(
      IBitmapRenderer gr,
      Rectangle destination,
      Rectangle source,
      RectangleF ri1,
      RectangleF ri2,
      bool leftOk,
      bool rightOk,
      bool fillLeft,
      bool fillRight)
    {
      float pageBowWidth = EngineConfiguration.Default.PageBowWidth;
      bool pageBowBorder = EngineConfiguration.Default.PageBowBorder;
      bool pageBowCenter = EngineConfiguration.Default.PageBowCenter;
      float scaleX = (float) destination.Width / (float) source.Width;
      float scaleY = (float) destination.Height / (float) source.Height;
      float num1 = ri1.Top - (float) source.Y;
      float height = ri1.Height;
      RectangleF rectangleF1 = (RectangleF) Rectangle.Empty;
      if (leftOk)
      {
        RectangleF rectangleF2 = new RectangleF((float) destination.X + ri1.Left - (float) source.X, (float) destination.Y + num1, ri1.Width, ri1.Height).Scale(scaleX, scaleY);
        rectangleF1 = rectangleF1.Union(rectangleF2);
        if (fillLeft)
          gr.FillRectangle(rectangleF2, this.BlankPageColor);
        if (this.RealisticPages)
        {
          if (pageBowBorder)
            ComicDisplayControl.DrawPageBow(gr, new RectangleF((float) destination.X + ri1.Left - (float) source.X, (float) destination.Y + num1, ri1.Width * pageBowWidth, height).Scale(scaleX, scaleY), false);
          if (pageBowCenter || !rightOk)
            ComicDisplayControl.DrawPageBow(gr, new RectangleF((float) ((double) destination.X + ((double) ri1.Right - (double) source.X) * (1.0 - (double) this.innerBowLeftOffsetInPercent) - (double) ri1.Width * (double) pageBowWidth), (float) destination.Y + num1, ri1.Width * pageBowWidth, height).Scale(scaleX, scaleY), true);
          gr.DrawRectangle(rectangleF2, Color.Black, 1f);
        }
      }
      if (rightOk)
      {
        RectangleF rectangleF3 = new RectangleF((float) destination.X + ri1.Right + ri2.Right - ri2.Width - (float) source.X, (float) destination.Y + num1, ri2.Width, ri1.Height).Scale(scaleX, scaleY);
        rectangleF1 = rectangleF1.Union(rectangleF3);
        if (fillRight)
          gr.FillRectangle(rectangleF3, this.BlankPageColor);
        if (this.RealisticPages)
        {
          if (pageBowBorder)
            ComicDisplayControl.DrawPageBow(gr, new RectangleF((float) ((double) destination.X + (double) ri1.Right + (double) ri2.Right - (double) ri2.Width * (double) pageBowWidth) - (float) source.X, (float) destination.Y + num1, ri2.Width * pageBowWidth, height).Scale(scaleX, scaleY), true);
          if (pageBowCenter || !leftOk)
          {
            float width = ri2.Width * pageBowWidth;
            float num2 = ri2.Width * this.innerBowRightOffsetInPercent;
            if ((double) ri1.Width - (double) num2 < (double) width)
              num2 = 0.0f;
            RectangleF rect = new RectangleF((float) destination.X + ri1.Right - (float) source.X + num2, (float) destination.Y + num1, width, height).Scale(scaleX, scaleY);
            ComicDisplayControl.DrawPageBow(gr, rect, false);
          }
          gr.DrawRectangle(rectangleF3, Color.Black, 1f);
        }
      }
      if (this.RealisticPages && !rectangleF1.IsEmpty)
      {
        int depth = (int) ((float) ((double) Math.Min(rectangleF1.Width, rectangleF1.Height) * (double) EngineConfiguration.Default.PageShadowWidthPercentage / 100.0)).Clamp(0.0f, (float) byte.MaxValue);
        if (depth != 0)
        {
          float maxOpacity = EngineConfiguration.Default.PageShadowOpacity.Clamp(0.0f, 1f);
          if (this.shadowBitmap == null)
            this.shadowBitmap = GraphicsExtensions.CreateShadowBitmap(BlurShadowType.Outside, Color.Black, 64, maxOpacity);
          gr.DrawShadow(rectangleF1.Pad(-depth), this.shadowBitmap, depth, BlurShadowParts.Edges);
        }
      }
      return rectangleF1;
    }

    private ComicDisplayControl.ImageInfo GetImageInfo(
      int page,
      PageImage image1,
      PageImage image2)
    {
      ComicDisplayControl.ImageInfo imageInfo = new ComicDisplayControl.ImageInfo();
      if (!this.IsValid)
        return imageInfo;
      try
      {
        int page1 = this.SeekPage(page, 1);
        if (!this.TwoPageDisplay || this.IsPageSingleType(page) || page1 == -1 || image1 != null && image1.Width > image1.Height || image2 == null || image2.Width > image2.Height || this.IsPageSingleType(page1))
        {
          if (image1 != null)
          {
            System.Drawing.Size size = image1.Size;
            if (this.PageLayout == PageLayoutMode.Double && size.Height > size.Width)
            {
              imageInfo.IsForcedDoublePage = true;
              size.Width += (int) ((double) size.Width * (1.0 - (double) this.DoublePageOverlap));
            }
            imageInfo.ImageCount = 1;
            imageInfo.Size = size;
          }
        }
        else if (image1 != null)
        {
          if (image2 != null)
          {
            int height = Math.Max(image1.Height, image2.Height);
            int num1 = image1.Width * height / image1.Height;
            int num2 = image2.Width * height / image2.Height;
            imageInfo.ImageCount = 2;
            imageInfo.Size = new System.Drawing.Size(num1 + num2 - (int) ((double) this.DoublePageOverlap * (double) num1), height);
          }
        }
      }
      catch
      {
      }
      return imageInfo;
    }

    private ComicDisplayControl.ImageInfo GetImageInfo(int page)
    {
      using (IItemLock<PageImage> image = this.GetImage(page))
      {
        using (IItemLock<PageImage> itemLock = this.TwoPageDisplay ? this.GetImage(this.SeekPage(page, 1)) : (IItemLock<PageImage>) new ItemLock<PageImage>((PageImage) null))
          return this.GetImageInfo(page, image.Item, itemLock.Item);
      }
    }

    private ComicDisplayControl.ImageInfo GetImageInfo() => this.GetImageInfo(this.CurrentPage);

    private bool IsPageSingleType(int page)
    {
      if (!this.IsValid)
        return false;
      try
      {
        return page < this.Book.Comic.PageCount && this.Book.Comic.GetPage(page).IsSinglePageType;
      }
      catch
      {
        return false;
      }
    }

    private bool IsPageSingleRightType(int page)
    {
      if (!this.IsValid)
        return false;
      try
      {
        return page < this.Book.Comic.PageCount && this.Book.Comic.GetPage(page).IsSingleRightPageType;
      }
      catch
      {
        return false;
      }
    }

    private int[] DisplayedPages => this.displayedPages;

    private Rectangle[] DisplayedPageAreas => this.displayedPageAreas;

    private RectangleF DisplayedPageBounds => this.displayedPageBounds;

    private void PositionMagnifier(System.Drawing.Point location)
    {
      this.magnifierOverlay.CenterLocation = location;
      this.UpdateMagnifierVisibility();
    }

    private void UpdateMagnifierVisibility()
    {
      if (true.Equals(this.magnifierOverlay.Tag))
      {
        this.magnifierOverlay.Visible = this.MagnifierVisible;
      }
      else
      {
        Rectangle clientRectangle = this.ClientRectangle;
        clientRectangle.Inflate(-32, -32);
        this.magnifierOverlay.Visible = this.MagnifierVisible && (!this.autoHideMagnifier || clientRectangle.Contains(this.magnifierOverlay.CenterLocation));
      }
      if (!this.magnifierOverlay.Visible)
        return;
      this.navigationOverlayVisible = false;
    }

    private void PositionMagnifier() => this.PositionMagnifier(this.PointToClient(Cursor.Position));

    private bool IsCurrentPageOverlayEnabled
    {
      get => (this.visibleInfoOverlays & InfoOverlays.CurrentPage) != 0;
    }

    private bool IsPartInfoOverlayEnabled
    {
      get => (this.visibleInfoOverlays & InfoOverlays.PartInfo) != 0;
    }

    private bool IsLoadPageOverlayEnabled
    {
      get => (this.visibleInfoOverlays & InfoOverlays.LoadPage) != 0;
    }

    private bool IsNavigationOverlayEnabled
    {
      get => (this.visibleInfoOverlays & InfoOverlays.PageBrowser) != 0;
    }

    private bool IsPageBrowsersOnTop
    {
      get => (this.visibleInfoOverlays & InfoOverlays.PageBrowserOnTop) != 0;
    }

    private bool CurrentPageShowsName
    {
      get => (this.visibleInfoOverlays & InfoOverlays.CurrentPageShowsName) != 0;
    }

    private PageKey GetPageKey(int page)
    {
      if (!this.IsValid)
        return (PageKey) null;
      PageKey pageKey = this.Book.GetPageKey(page);
      pageKey.Source = (object) this;
      return pageKey;
    }

    private bool IsPageInCache(int page, int offset = 0, bool fastMem = true, bool putInCache = true)
    {
      int page1 = this.SeekPage(page, offset);
      if (page1 == -1 || page1 == page)
        return true;
      PageKey pageKey = this.GetPageKey(page1);
      using (IItemLock<PageImage> page2 = this.pagePool.GetPage(pageKey, fastMem))
      {
        if (page2 != null)
        {
          if (page2.Item != null)
            return true;
        }
      }
      if (putInCache && this.Book != null)
        this.pagePool.CachePage(pageKey, fastMem, (IImageProvider) this.Book, false);
      return false;
    }

    private int CachePage(int page, int offset, bool fastMem, bool bottom)
    {
      int page1 = this.SeekPage(page, offset);
      if (page1 == page)
        page1 = -1;
      if (page1 != -1)
        this.pagePool.CachePage(this.GetPageKey(page1), fastMem, (IImageProvider) this.Book, bottom);
      return page1;
    }

    private bool CacheBackPage(ref int page, int offset)
    {
      int num = this.CachePage(page, offset, true, true);
      if (num == -1)
        return false;
      page = num;
      return true;
    }

    private int SeekPage(int page, int offset)
    {
      return this.Book == null ? -1 : this.Book.SeekNextPage(page, Math.Abs(offset), Math.Sign(offset));
    }

    public IItemLock<PageImage> GetImage(int page, bool withCaching = false)
    {
      bool flag = this.PreCache & withCaching;
      if (!this.IsValid || page < 0 || page > this.Book.ProviderPageCount)
        return (IItemLock<PageImage>) new ItemLock<PageImage>((PageImage) null);
      PageKey pageKey = this.GetPageKey(page);
      IItemLock<PageImage> page1 = this.pagePool.GetPage(pageKey, true);
      if (page1 == null)
        this.PagePool.CachePage(pageKey, true, (IImageProvider) this.book, false);
      if (flag)
      {
        this.CachePage(page, 1, true, false);
        this.InvalidatePendingImageCacheUpdate();
      }
      if (page1 != null)
      {
        this.lastValidKey = pageKey;
        this.firstPageHasBeenLoaded = true;
        page1.Tag = (object) true;
        return page1;
      }
      if (this.lastValidKey != null)
        page1 = this.pagePool.GetPage(this.lastValidKey, false);
      return page1 ?? (IItemLock<PageImage>) new ItemLock<PageImage>((PageImage) null);
    }

    private void InvalidatePendingImageCacheUpdate()
    {
      this.cacheUpdateTimer.Stop();
      this.cacheUpdateTimer.Start();
    }

    private void StopPendingImageCacheUpdate() => this.cacheUpdateTimer.Stop();

    private void cacheUpdateTimer_Tick(object sender, EventArgs e)
    {
      this.cacheUpdateTimer.Stop();
      if (!this.IsValid)
        return;
      try
      {
        int currentPage = this.CurrentPage;
        int num = (this.pagePool.MaximumMemoryItems - 15) / 2;
        int page1 = this.CachePage(currentPage, 1, true, false);
        int page2 = currentPage;
        bool flag1 = page1 != -1;
        bool flag2 = true;
        do
          ;
        while (num > 0 && (!flag1 || !(flag1 = this.CacheBackPage(ref page1, 1)) || --num != 0) && (!flag1 || !(flag1 = this.CacheBackPage(ref page1, 1)) || --num != 0) && (!flag2 || !(flag2 = this.CacheBackPage(ref page2, -1)) || --num != 0) && (flag1 || flag2));
      }
      catch (Exception ex)
      {
      }
    }

    private Matrix4 GetMatrix(System.Drawing.Drawing2D.Matrix matrix)
    {
      float[] elements = matrix.Elements;
      return new Matrix4(elements[0], elements[2], 0.0f, elements[4], elements[1], elements[3], 0.0f, elements[5], 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    private System.Drawing.Drawing2D.Matrix GetMatrix(Matrix4 matrix)
    {
      return new System.Drawing.Drawing2D.Matrix(matrix[0], matrix[1], matrix[4], matrix[5], matrix[12], matrix[13]);
    }

    private bool DrawPage(
      IBitmapRenderer gr,
      IItemLock<PageImage> il,
      RectangleF rd,
      RectangleF rs)
    {
      float opacity = 1f;
      if (il.Tag is bool && (bool) il.Tag)
      {
        if (this.drawBlankPagesOverride)
        {
          gr.FillRectangle(rd, EngineConfiguration.Default.BlankPageColor);
          opacity = 0.1f;
        }
        try
        {
          bool flag = false;
          if (gr.IsHardware && this.SoftwareFiltering && !this.inBlendAnmation)
          {
            Vector3 scaling;
            Matrix4.Decompose(this.GetMatrix(gr.Transform), out Vector3 _, out scaling, out Matrix4 _);
            float sx = (float) Math.Round((double) scaling[0], 4);
            float sy = (float) Math.Round((double) scaling[1], 4);
            float softwareFilterMinScale = EngineConfiguration.Default.SoftwareFilterMinScale;
            if ((double) sx >= (double) softwareFilterMinScale && (double) sy >= (double) softwareFilterMinScale && ((double) sx < 1.0 || (double) sy < 1.0))
            {
              using (IItemLock<ComicDisplayControl.ScaledPageItem> scaledPage = this.GetScaledPage(il.Item, sx, sy))
              {
                if (scaledPage != null)
                {
                  if (scaledPage.Item.IsValid)
                  {
                    ComicDisplayControl.ScaledPageItem image = scaledPage.Item;
                    System.Drawing.Size size = il.Item.Size;
                    float num1 = (float) image.Width / (float) size.Width;
                    float num2 = (float) image.Height / (float) size.Height;
                    rs.X *= num1;
                    rs.Y *= num2;
                    rs.Width *= num1;
                    rs.Height *= num2;
                    rd.Width *= num1 / scaling[0];
                    rd.Height *= num2 / scaling[1];
                    ((IHardwareRenderer) gr).OptimizedTextures = true;
                    try
                    {
                      gr.DrawImage((RendererImage) (MemoryOptimizedImage) image, rd, rs, BitmapAdjustment.Empty, opacity);
                    }
                    finally
                    {
                      ((IHardwareRenderer) gr).OptimizedTextures = false;
                    }
                    flag = true;
                  }
                }
              }
            }
          }
          if (!flag)
            gr.DrawImage((RendererImage) (MemoryOptimizedImage) il.Item, rd, rs, BitmapAdjustment.Empty, opacity);
        }
        catch (Exception ex)
        {
        }
        return true;
      }
      gr.FillRectangle(rd, Color.FromArgb(128, this.BackColor));
      return false;
    }

    private IItemLock<ComicDisplayControl.ScaledPageItem> GetScaledPage(
      PageImage bmp,
      float sx,
      float sy)
    {
      if (bmp == null)
        return (IItemLock<ComicDisplayControl.ScaledPageItem>) null;
      if (this.scaledCache == null)
      {
        this.scaledCache = new cYo.Common.Collections.Cache<ComicDisplayControl.ScaledPageKey, ComicDisplayControl.ScaledPageItem>(2);
        this.scaledCache.MinimalTimeInCache = 0;
        this.scaledCache.ItemRemoved += (EventHandler<CacheItemEventArgs<ComicDisplayControl.ScaledPageKey, ComicDisplayControl.ScaledPageItem>>) ((s, e) => e.Item.Dispose());
      }
      IItemLock<ComicDisplayControl.ScaledPageItem> scaledPage = this.scaledCache.LockItem(new ComicDisplayControl.ScaledPageKey()
      {
        ScaleX = sx,
        ScaleY = sy,
        Bitmap = bmp
      }, (Func<ComicDisplayControl.ScaledPageKey, ComicDisplayControl.ScaledPageItem>) (b => new ComicDisplayControl.ScaledPageItem()));
      int num = EngineConfiguration.Default.SoftwareFilterDelay.Clamp(100, 5000);
      long ticks = Machine.Ticks;
      if (!scaledPage.Item.IsValid)
      {
        if (scaledPage.Item.Ticks != 0L && ticks - scaledPage.Item.Ticks > (long) num)
        {
          System.Drawing.Size size = new System.Drawing.Size((int) Math.Round((double) bmp.Width * (double) sx), (int) Math.Round((double) bmp.Height * (double) sy));
          scaledPage.Item.Optimized = PageImage.MemoryOptimized;
          scaledPage.Item.Bitmap = bmp.Bitmap.Resize(size, EngineConfiguration.Default.SoftwareFilter);
        }
        else
        {
          scaledPage.Item.Ticks = ticks;
          this.imageScaleTimer.Interval = num + 100;
          this.imageScaleTimer.Stop();
          this.imageScaleTimer.Start();
        }
      }
      return scaledPage;
    }

    private void imageScaleTimer_Tick(object sender, EventArgs e)
    {
      this.imageScaleTimer.Stop();
      this.Invalidate();
    }

    public override Bitmap CreatePageImage()
    {
      bool realisticPages = this.RealisticPages;
      this.RealisticPages = false;
      try
      {
        return base.CreatePageImage();
      }
      finally
      {
        this.RealisticPages = realisticPages;
      }
    }

    public override int PageScrollingTime
    {
      get => EngineConfiguration.Default.PageScrollingDuration;
      set => base.PageScrollingTime = value;
    }

    protected override void DrawImage(
      IBitmapRenderer gr,
      Rectangle destination,
      Rectangle source,
      bool clipToDestination)
    {
      if (!this.IsValid)
        return;
      int displayHash = this.displayHash;
      int currentPage = this.CurrentPage;
      int nextPage = this.NextPage;
      int[] numArray = new int[0];
      Rectangle[] rectangleArray = new Rectangle[0];
      using (IItemLock<PageImage> image1 = this.GetImage(currentPage, true))
      {
        using (IItemLock<PageImage> image2 = this.GetImage(nextPage, true))
        {
          if (image1.Item == null)
          {
            this.displayHash = 0;
          }
          else
          {
            ComicDisplayControl.ImageInfo imageInfo = this.GetImageInfo(currentPage, image1.Item, image2.Item);
            SizeF size1 = (SizeF) imageInfo.Size;
            if (!clipToDestination)
            {
              float num1 = (float) source.Width / (float) destination.Width;
              float num2 = (float) source.Height / (float) destination.Height;
              destination = destination.Pad(-(int) ((double) num1 * (double) source.X), -(int) ((double) num2 * (double) source.Top), -(int) ((double) num1 * ((double) size1.Width - (double) source.Right)), -(int) ((double) num2 * ((double) size1.Height - (double) source.Bottom)));
              source = new Rectangle(System.Drawing.Point.Empty, size1.ToSize());
            }
            if (imageInfo.IsSingleImage && !imageInfo.IsForcedDoublePage)
            {
              this.DrawPage(gr, image1, (RectangleF) destination, (RectangleF) source);
              if (this.RealisticPages)
              {
                if (imageInfo.IsDoublePage)
                {
                  RectangleF rectangleF = new RectangleF(0.0f, 0.0f, (float) image1.Item.Width / 2f, (float) image1.Item.Height);
                  this.displayedPageBounds = this.DrawPageOrnaments(gr, destination, source, rectangleF, rectangleF, true, true, false, false);
                }
                else
                {
                  RectangleF rectangleF = new RectangleF(0.0f, 0.0f, (float) image1.Item.Width, (float) image1.Item.Height);
                  this.displayedPageBounds = this.DrawPageOrnaments(gr, destination, source, rectangleF, rectangleF, true, false, false, false);
                }
              }
              this.displayHash = image1.Item.GetHashCode();
              numArray = new int[1]{ currentPage };
              rectangleArray = new Rectangle[1]
              {
                destination
              };
            }
            else
            {
              bool flag1 = this.RightToLeftReading && this.RightToLeftReadingMode == RightToLeftReadingMode.FlipPages;
              bool flag2 = this.IsPageSingleType(currentPage) && nextPage == -1;
              bool flag3 = (((currentPage == 0 ? 0 : (!this.IsPageSingleRightType(currentPage) ? 1 : 0)) & (flag1 ? 1 : 0) | (flag2 ? 1 : 0)) ^ (this.IsFlipped ? 1 : 0)) != 0;
              bool a1 = !imageInfo.IsSingleImage;
              bool b1 = true;
              bool fillLeft = false;
              bool fillRight = false;
              IItemLock<PageImage> a2 = image1;
              IItemLock<PageImage> b2 = image2;
              if (imageInfo.IsSingleImage)
                b2 = a2;
              if (flag3)
              {
                CloneUtility.Swap<IItemLock<PageImage>>(ref a2, ref b2);
                CloneUtility.Swap<bool>(ref a1, ref b1);
              }
              System.Drawing.Size size2 = a2.Item.Size;
              System.Drawing.Size size3 = b2.Item.Size;
              float num3 = size1.Height / (float) size2.Height;
              float num4 = size1.Height / (float) size3.Height;
              RectangleF ri1 = new RectangleF(0.0f, 0.0f, (float) size2.Width, (float) size2.Height).Scale(num3);
              RectangleF ri2 = new RectangleF(0.0f, 0.0f, (float) size3.Width, (float) size3.Height).Scale(num4);
              float num5 = this.DoublePageOverlap * ri1.Width;
              if (currentPage == 0 | flag2)
                flag3 = !flag3;
              if (flag3)
                ri1.Width -= num5;
              else
                ri2.Width -= num5;
              RectangleF rectangleF = new RectangleF((float) source.X, (float) source.Y, Math.Min(ri1.Right, (float) source.Right) - (float) source.X, (float) source.Height);
              RectangleF rect1 = new RectangleF(rectangleF.Right - ri1.Width, rectangleF.Y, (float) source.Right - rectangleF.Right, rectangleF.Height);
              if (!flag3)
                rect1.X += num5;
              RectangleF rect2 = new RectangleF((float) destination.X + rectangleF.Left / (float) source.Width, (float) destination.Y, (float) destination.Width * rectangleF.Width / (float) source.Width, (float) destination.Height);
              RectangleF rect3 = new RectangleF(rect2.Right, (float) destination.Y, (float) destination.Right - rect2.Right, (float) destination.Height);
              rectangleF = rectangleF.Scale(1f / num3);
              RectangleF rs = rect1.Scale(1f / num4);
              using (gr.SaveState())
              {
                if (a1)
                {
                  gr.ScaleTransform(num3, num3);
                  a1 = this.DrawPage(gr, a2, rect2.Scale(1f / num3), rectangleF);
                }
                else
                  fillLeft = currentPage != 0 && !flag2;
              }
              using (gr.SaveState())
              {
                if (b1)
                {
                  gr.ScaleTransform(num4, num4);
                  b1 = this.DrawPage(gr, b2, rect3.Scale(1f / num4), rs);
                }
                else
                  fillRight = currentPage != 0 && !flag2;
              }
              this.displayedPageBounds = this.DrawPageOrnaments(gr, destination, source, ri1, ri2, a1 | fillLeft, b1 | fillRight, fillLeft, fillRight);
              this.displayHash = a2.Item.GetHashCode() ^ b2.Item.GetHashCode() << 1;
              List<int> intList = new List<int>();
              List<Rectangle> rectangleList = new List<Rectangle>();
              if (a1)
              {
                intList.Add(currentPage);
                rectangleList.Add(rect2.Round());
              }
              if (b1)
              {
                intList.Add(a1 ? nextPage : currentPage);
                rectangleList.Add(rect3.Round());
              }
              numArray = intList.ToArray();
              rectangleArray = rectangleList.ToArray();
              if (flag3)
                Array.Reverse((Array) numArray);
            }
          }
        }
      }
      if (displayHash != this.displayHash)
        this.OnDisplayChanged();
      ComicBookNavigator book = this.Book;
      if (book != null)
      {
        int num = nextPage != -1 ? nextPage : currentPage;
        if (num > book.LastPageRead)
          book.LastPageRead = num;
      }
      this.displayedPages = numArray;
      this.displayedPageAreas = rectangleArray;
    }

    protected override void RenderImageEffect(
      IBitmapRenderer bitmapRenderer,
      ImageDisplayControl.DisplayOutput display)
    {
      base.RenderImageEffect(bitmapRenderer, display);
      if (!bitmapRenderer.IsHardware || this.workingPaperTexture == null)
        return;
      IHardwareRenderer gr = bitmapRenderer as IHardwareRenderer;
      gr.BlendingOperation = BlendingOperation.Multiply;
      try
      {
        gr.FillRectangle((RendererImage) this.workingPaperTexture, this.PaperTextureLayout, this.displayedPageBounds, (RectangleF) this.paperTextureBitmap.Size.ToRectangle(), BitmapAdjustment.Empty, 1f);
      }
      catch (Exception ex)
      {
      }
      gr.BlendingOperation = BlendingOperation.Blend;
    }

    protected virtual void OnDisplayChanged()
    {
      if (this.DisplayEventsDisabled)
        return;
      this.UpdatePartOverlay(true);
      this.navigationOverlay.IsDoublePage = this.IsDoubleImage;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (e.Button == MouseButtons.Left && this.IsMouseOk(e.Location))
      {
        if (!this.MagnifierVisible)
        {
          this.mouseDown = e.Location;
          this.longClickTimer.Start();
        }
        if (this.NavigationOverlayVisible)
        {
          this.NavigationOverlayVisible = false;
          this.MouseActionHappened = true;
        }
        this.ShowGestureIndicator(e.Location);
      }
      this.currentMousePage = this.GetPageFromPoint(e.Location);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      if (!this.mouseDown.IsEmpty)
      {
        this.mouseDown = System.Drawing.Point.Empty;
        this.longClickTimer.Stop();
      }
      if (true.Equals(this.magnifierOverlay.Tag))
      {
        this.MagnifierVisible = false;
        this.magnifierOverlay.Tag = (object) false;
        this.DisableScrolling = false;
      }
      base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!this.mouseDown.IsEmpty && (Math.Abs(this.mouseDown.X - e.X) > 5 || Math.Abs(this.mouseDown.Y - e.Y) > 5))
      {
        this.longClickTimer.Stop();
        this.mouseDown = System.Drawing.Point.Empty;
      }
      if (this.magnifierOverlay.Visible)
        this.Cursor = Cursor.Current = ComicDisplayControl.EmptyCursor;
      else
        this.Cursor = Cursor.Current = Cursors.Default;
      this.UpdateNavigationOverlay();
      this.PositionMagnifier(e.Location);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      this.longClickTimer.Stop();
      this.UpdateNavigationOverlay(new System.Drawing.Point(-1, -1));
      this.PositionMagnifier();
    }

    protected override void OnGestureStart()
    {
      this.MouseActionHappened = false;
      base.OnGestureStart();
      if (!this.magnifierOverlay.Visible || this.magnifierOverlay.Bounds.Contains(this.GestureLocation))
        return;
      this.MagnifierVisible = false;
    }

    protected override void OnPanStart()
    {
      base.OnPanStart();
      this.panMagnifier = false;
      if (!this.magnifierOverlay.Visible || !this.magnifierOverlay.Bounds.Contains(this.GestureLocation))
        return;
      this.panMagnifier = true;
      this.MouseActionHappened = true;
    }

    protected override void OnPan()
    {
      base.OnPan();
      if (!this.panMagnifier)
        return;
      this.PositionMagnifier(this.PanLocation);
      this.MouseActionHappened = true;
    }

    protected override void OnPageDisplayModeChanged()
    {
      base.OnPageDisplayModeChanged();
      this.UpdatePartOverlay(true);
    }

    protected override void OnVisiblePartChanged()
    {
      base.OnVisiblePartChanged();
      this.UpdatePartOverlay(false);
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      this.navigationOverlay.Size = this.CalcNavigationOverlaySize();
      if (this.navigationOverlay.Visible)
      {
        this.navigationOverlay.Y = this.NavigationOverlayVisibleY;
        this.navigationOverlay.X = (this.ClientRectangle.Width - this.navigationOverlay.Width) / 2;
      }
      this.UpdatePartOverlay(true);
    }

    protected override void OnRenderImageOverlay(ImageDisplayControl.RenderEventArgs e)
    {
      base.OnRenderImageOverlay(e);
      this.UpdateCurrentPageOverlay();
      this.UpdateMessageOverlay();
      if (this.messageOverlay.Visible)
        this.loadPageOverlay.Visible = false;
      else
        this.UpdateLoadPageOverlay();
      this.overlayManager.Draw(e.Graphics);
      if (!this.blindOut)
        return;
      e.Graphics.FillRectangle((RectangleF) this.ClientRectangle, this.blindOutColor);
    }

    protected override bool IsInputKey(Keys keyData)
    {
      switch (keyData)
      {
        case Keys.Tab:
        case Keys.End:
        case Keys.Home:
        case Keys.Left:
        case Keys.Up:
        case Keys.Right:
        case Keys.Down:
        case Keys.Tab | Keys.Shift:
          return true;
        default:
          return base.IsInputKey(keyData);
      }
    }

    protected override Color GetAutoBackgroundColor()
    {
      try
      {
        if (this.IsValid)
        {
          using (IItemLock<PageImage> image = this.GetImage(this.CurrentPage))
          {
            Color backgrounColor = image.Item.BackgrounColor;
            if (backgrounColor.IsEmpty)
            {
              Bitmap bitmap = image.Item != null ? image.Item.Bitmap : (Bitmap) null;
              if (bitmap != null)
              {
                Color[] items = new Color[4]
                {
                  bitmap.GetAverageColor(2, 2, 4),
                  bitmap.GetAverageColor(bitmap.Width - 2 - 4, 2, 4),
                  bitmap.GetAverageColor(bitmap.Width - 2 - 4, bitmap.Height - 2 - 4, 4),
                  bitmap.GetAverageColor(2, bitmap.Height - 2 - 4, 4)
                };
                image.Item.BackgrounColor = (double) ((IEnumerable<Color>) items).GetAverage().GetBrightness() >= 0.5 ? ((IEnumerable<Color>) items).Max<Color>((Comparison<Color>) ((a, b) => b.GetBrightness().CompareTo(a.GetBrightness()))) : ((IEnumerable<Color>) items).Max<Color>((Comparison<Color>) ((a, b) => a.GetBrightness().CompareTo(b.GetBrightness())));
              }
            }
            backgrounColor = image.Item.BackgrounColor;
            return backgrounColor;
          }
        }
      }
      catch
      {
      }
      return Color.Empty;
    }

    protected override bool IsImageValid() => this.GetImageInfo().IsValid;

    protected override System.Drawing.Size GetImageSize() => this.GetImageInfo().Size;

    public override bool IsDoubleImage => this.GetImageInfo().IsDoubleImage;

    protected override void OnDoubleClick(EventArgs e)
    {
      if (this.MouseHandled)
        return;
      base.OnDoubleClick(e);
    }

    protected override bool IsMouseOk(System.Drawing.Point point)
    {
      return this.overlayManager.Panels.Find((Predicate<OverlayPanel>) (x => x.HasMouse)) == null;
    }

    protected override void OnImageDisplayOptionsChanged() => this.UpdateNavigationOverlay(false);

    protected override void OnReadingModeChanged()
    {
      base.OnReadingModeChanged();
      this.navigationOverlay.Mirror = this.IsMovementFlipped;
    }

    private void ShowGestureIndicator(System.Drawing.Point pt)
    {
      if (!EngineConfiguration.Default.ShowGestureHint)
        return;
      ImageDisplayControl.GestureArea gestureArea = this.GestureHitTest(pt);
      if (gestureArea == null)
        return;
      GestureEventArgs e = new GestureEventArgs(GestureType.Touch)
      {
        Area = gestureArea.Alignment,
        AreaBounds = gestureArea.Area,
        Double = false
      };
      this.OnPreviewGesture(e);
      if (!e.Handled)
      {
        e.Double = true;
        this.OnPreviewGesture(e);
      }
      if (!e.Handled)
        return;
      this.gestureOverlay.Alignment = gestureArea.Alignment;
      this.gestureOverlay.Size = gestureArea.Area.Size;
      this.gestureOverlay.Opacity = 1f;
      this.Update();
      this.gestureOverlay.Animators[0].Start();
    }

    public void DisplayOpenMessage() => this.firstPageHasBeenLoaded = false;

    private void UpdateNavigationOverlay(bool redraw)
    {
      if (!this.IsValid)
        return;
      this.navigationOverlay.Pages = this.Book.GetPages().ToArray<int>();
      if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.None)
        this.navigationOverlay.DisplayedPageIndex = this.navigationOverlay.Pages.IndexOf((object) this.CurrentPage);
      this.navigationOverlay.IsDoublePage = this.IsDoubleImage;
      this.navigationOverlay.Caption = this.book.Comic.Caption;
      if (!redraw)
        return;
      this.navigationOverlay.Invalidate();
    }

    private void UpdatePartOverlay(bool always)
    {
      int num = this.CurrentPage * 100 + this.ImageVisiblePart.Part;
      System.Drawing.Point offset = this.ImageVisiblePart.Offset;
      if (num == this.cachedPartOverlay && this.cachedPartOffset == offset && !always)
        return;
      using (ItemMonitor.Lock((object) this.visiblePartOverlay))
      {
        if (this.ImagePartCount == 1)
        {
          if (!this.visiblePartOverlay.IsVisible)
            goto label_12;
        }
        if (!this.IsImageValid())
        {
          this.smallBitmap.SafeDispose();
          this.smallBitmap = (Bitmap) null;
        }
        else if (this.IsPartInfoOverlayEnabled)
          this.visiblePartOverlay.Animators[0].Start();
      }
label_12:
      this.cachedPartOverlay = num;
      this.cachedPartOffset = offset;
    }

    private void UpdateCurrentPageOverlay()
    {
      this.UpdateCurrentPageOverlay((IEnumerable<int>) this.DisplayedPages);
    }

    private void UpdateCurrentPageOverlay(IEnumerable<int> pageNumbers)
    {
      if (this.Book == null || pageNumbers == null)
        return;
      int[] array = pageNumbers.Where<int>((Func<int, bool>) (n => n >= 0)).ToArray<int>();
      if (array.Length == 0 || this.currentPageOverlayHash == this.DisplayHash)
        return;
      this.currentPageOverlayHash = this.DisplayHash;
      string str = ComicBook.FormatNumber(array.Length == 1 ? (array[0] + 1).ToString() : string.Format("{0}/{1}", (object) (array[0] + 1), (object) (array[1] + 1)), this.Book.IsIndexRetrievalCompleted ? this.Book.ProviderPageCount : -1);
      if (this.CurrentPageShowsName)
      {
        string text = (str + "<small>").AppendWithSeparator("<br/>", this.Book.GetImageName(this.Book.Comic.TranslatePageToImageIndex(array[0]), true).ToXmlString());
        if (array.Length > 1)
          text = text.AppendWithSeparator("<br/>", this.Book.GetImageName(this.Book.Comic.TranslatePageToImageIndex(array[1]), true).ToXmlString());
        str = text + "</small>";
      }
      this.currentPageOverlay.Text = str;
      if (!this.IsCurrentPageOverlayEnabled)
        return;
      this.currentPageOverlay.Animators[0].Start();
    }

    private void UpdateLoadPageOverlay()
    {
      if (!this.IsLoadPageOverlayEnabled || !this.IsValid || this.CurrentPage < 0)
      {
        this.loadPageOverlay.Visible = false;
      }
      else
      {
        int currentPage = this.CurrentPage;
        int nextPage = this.NextPage;
        bool flag1 = false;
        bool flag2 = false;
        using (IItemLock<PageImage> image1 = this.GetImage(currentPage))
        {
          using (IItemLock<PageImage> image2 = this.GetImage(this.NextPage))
          {
            ComicDisplayControl.ImageInfo imageInfo = this.GetImageInfo(currentPage, image1.Item, image2.Item);
            bool flag3;
            int num1;
            if (image1.Item != null)
            {
              flag3 = true;
              num1 = !flag3.Equals(image1.Tag) ? 1 : 0;
            }
            else
              num1 = 1;
            flag1 = num1 != 0;
            if (!imageInfo.IsSingleImage)
            {
              int num2;
              if (image2.Item != null)
              {
                flag3 = true;
                num2 = !flag3.Equals(image2.Tag) ? 1 : 0;
              }
              else
                num2 = 1;
              flag2 = num2 != 0;
            }
          }
        }
        if (!flag1 && !flag2)
        {
          this.loadPageOverlay.Visible = false;
        }
        else
        {
          string pageText = string.Empty;
          int num;
          if (flag1)
          {
            num = this.CurrentPage + 1;
            pageText = num.ToString();
          }
          if (flag2 && nextPage != -1)
          {
            string text = pageText;
            string[] strArray = new string[1];
            num = nextPage + 1;
            strArray[0] = num.ToString();
            pageText = text.AppendWithSeparator(", ", strArray);
          }
          if (string.IsNullOrEmpty(pageText))
            return;
          this.UpdateLoadPageOverlay(pageText);
        }
      }
    }

    private void UpdateLoadPageOverlay(string pageText)
    {
      if (this.Book == null || string.IsNullOrEmpty(pageText))
        return;
      this.loadPageOverlay.Text = string.Format(TR.Messages["LoadingPage", "Loading Page {0}..."], (object) pageText);
      this.loadPageOverlay.Visible = true;
    }

    private void UpdateMessageOverlay()
    {
      string str = (string) null;
      Bitmap bitmap = (Bitmap) null;
      if (this.Book == null)
      {
        str = TR.Messages["NoComicOpen", "No book is open"];
      }
      else
      {
        if (this.Book.ProviderStatus == ImageProviderStatus.Error)
          str = StringUtility.Format(TR.Messages["OpenError", "Could not open the book '{0}'!"], (object) this.Book.Comic.DisplayFileLocation);
        else if (!this.firstPageHasBeenLoaded)
          str = StringUtility.Format(TR.Messages["OpeningComic", "Opening the book '{0}'..."], (object) this.Book.Comic.DisplayFileLocation);
        if (str != null && this.ThumbnailPool != null)
        {
          IItemLock<ThumbnailImage> thumbnail;
          using (thumbnail = this.ThumbnailPool.GetThumbnail(this.Book.Comic.GetFrontCoverThumbnailKey(), true))
          {
            if (thumbnail != null)
            {
              if (thumbnail.Item != null)
              {
                if (this.messageOverlay.Tag == thumbnail.Item && this.messageOverlay.Icon != null)
                {
                  bitmap = this.messageOverlay.Icon;
                }
                else
                {
                  this.messageOverlay.Tag = (object) thumbnail.Item;
                  bitmap = ComicBox3D.CreateDefaultBook(thumbnail.Item.GetThumbnail(128), (Bitmap) null, new System.Drawing.Size(128, 128), this.Book.Comic.PageCount);
                }
              }
            }
          }
        }
      }
      if (this.messageOverlay.Icon != bitmap)
      {
        Bitmap icon = this.messageOverlay.Icon;
        this.messageOverlay.Icon = bitmap;
        icon?.Dispose();
      }
      this.messageOverlay.Text = str;
      this.messageOverlay.Visible = !string.IsNullOrEmpty(str) && this.showStatusMessage;
    }

    private void DrawMagnifier(IBitmapRenderer gr, System.Drawing.Point location, Rectangle mrc, float zoom)
    {
      ImageDisplayControl.DisplayOutput output = this.LastRenderedDisplay ?? this.Display;
      Rectangle rectangle = mrc;
      mrc.Width = mrc.Height = Math.Max(mrc.Height, mrc.Width);
      Rectangle source = mrc;
      ref Rectangle local1 = ref source;
      double width1 = (double) source.Width;
      SizeF scale = output.Scale;
      double width2 = (double) scale.Width;
      int num1 = (int) (width1 / width2 / (double) zoom);
      local1.Width = num1;
      ref Rectangle local2 = ref source;
      double height1 = (double) source.Height;
      scale = output.Scale;
      double height2 = (double) scale.Height;
      int num2 = (int) (height1 / height2 / (double) zoom);
      local2.Height = num2;
      source.Offset(this.ClientToImage(output, location));
      source.Offset(-source.Width / 2, -source.Height / 2);
      using (gr.SaveState())
      {
        using (gr.SaveState())
        {
          gr.TranslateTransform((float) rectangle.Width / 2f, (float) rectangle.Height / 2f);
          gr.ScaleTransform(zoom, zoom);
          gr.TranslateTransform((float) -location.X, (float) -location.Y);
          this.RenderImageBackground(gr, (ImageDisplayControl.DisplayOutput) null, -1);
        }
        gr.TranslateTransform((float) ((double) mrc.Width / 2.0 - (double) (mrc.Width - rectangle.Width) / 2.0), (float) ((double) mrc.Height / 2.0 - (double) (mrc.Height - rectangle.Height) / 2.0));
        gr.RotateTransform((float) output.Config.Rotation.ToDegrees());
        gr.TranslateTransform((float) -mrc.Width / 2f, (float) -mrc.Height / 2f);
        this.DrawImage(gr, mrc, source, true);
        this.RenderImageEffect(gr, (ImageDisplayControl.DisplayOutput) null);
      }
    }

    private void magnifierOverlay_RenderSurface(object sender, PanelRenderEventArgs e)
    {
      ComicDisplayControl.Magnifier magnifier = ComicDisplayControl.magnifiers[(int) this.MagnifierStyle];
      IBitmapRenderer renderer = e.Renderer;
      Padding padding1 = magnifier.Outer.GetPadding(magnifier.Bitmap.Size);
      Padding padding2 = magnifier.Inner.GetPadding(magnifier.Bitmap.Size);
      Rectangle mrc = this.magnifierOverlay.ClientRectangle.Pad(padding1);
      System.Drawing.Point location1 = this.magnifierOverlay.Location;
      location1.Offset(mrc.Location);
      location1.Offset(mrc.Width / 2, mrc.Height / 2);
      System.Drawing.Point location2 = location1.Clip(this.ClientRectangle);
      float magnifierZoom = this.MagnifierZoom;
      renderer.Opacity = this.MagnifierOpacity;
      RectangleF clip = renderer.Clip;
      renderer.Clip = (RectangleF) mrc;
      this.DrawMagnifier(renderer, location2, mrc, magnifierZoom);
      renderer.Clip = clip;
      renderer.Opacity = 1f;
      ScalableBitmap.Draw(renderer, magnifier.Bitmap, (RectangleF) this.magnifierOverlay.ClientRectangle, padding2, 1f);
    }

    private void visiblePartOverlay_Drawing(object sender, EventArgs e)
    {
      System.Drawing.Size size = this.ImageSize.ToRectangle(ComicDisplayControl.partInfoSize, RectangleScaleMode.None).Size;
      size.Width += 16;
      size.Height += 16;
      if (this.visiblePartOverlay.Size == size)
        return;
      this.visiblePartOverlay.Size = size;
      using (PanelSurface surface = this.visiblePartOverlay.CreateSurface())
      {
        surface.Graphics.Clear(Color.Transparent);
        this.partRect = PanelRenderer.DrawGraphics(surface.Graphics, (RectangleF) new Rectangle(System.Drawing.Point.Empty, size), 1f);
      }
    }

    private void visiblePartOverlay_RenderSurface(object sender, PanelRenderEventArgs e)
    {
      IBitmapRenderer renderer = e.Renderer;
      System.Drawing.Size size = this.partRect.Size.ToSize();
      System.Drawing.Size imageSize = this.GetImageSize();
      if (imageSize.IsEmpty)
        return;
      Rectangle rectangle = imageSize.ToRectangle(size);
      rectangle.Offset((int) this.partRect.Left, (int) this.partRect.Top);
      if ((double) this.DoublePageOverlap == 0.0 && (this.currentPartHash != this.displayHash || this.smallBitmap == null))
      {
        Image smallBitmap = (Image) this.smallBitmap;
        try
        {
          this.smallBitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);
          Bitmap bitmap = (this.ImageDisplayOptions & ImageDisplayOptions.HighQuality) != 0 ? new Bitmap(this.smallBitmap.Width * 2, this.smallBitmap.Height * 2) : this.smallBitmap;
          using (Graphics graphics = Graphics.FromImage((Image) bitmap))
          {
            graphics.ScaleTransform((float) bitmap.Width / (float) imageSize.Width, (float) bitmap.Height / (float) imageSize.Height);
            this.DrawImage((IBitmapRenderer) new BitmapGdiRenderer(graphics)
            {
              LowQualityInterpolation = InterpolationMode.Low
            }, imageSize.ToRectangle(), imageSize.ToRectangle());
          }
          if (bitmap != this.smallBitmap)
          {
            using (Graphics graphics = Graphics.FromImage((Image) this.smallBitmap))
            {
              graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
              graphics.DrawImage((Image) bitmap, this.smallBitmap.Size.ToRectangle());
            }
            bitmap.Dispose();
          }
        }
        catch (Exception ex)
        {
        }
        smallBitmap?.Dispose();
        this.currentPartHash = this.displayHash;
      }
      Rectangle src = this.PagePartBounds.Scale(imageSize.GetScale(size));
      Rectangle dest = src;
      dest.Offset(rectangle.Location);
      renderer.Opacity = this.visiblePartOverlay.Opacity;
      if (this.smallBitmap != null)
      {
        try
        {
          renderer.DrawImage((RendererImage) this.smallBitmap, (RectangleF) rectangle, (RectangleF) new Rectangle(0, 0, this.smallBitmap.Width, this.smallBitmap.Height), BitmapAdjustment.Empty, 0.3f);
          renderer.DrawImage((RendererImage) this.smallBitmap, (RectangleF) dest, (RectangleF) src, BitmapAdjustment.Empty, 1f);
        }
        catch (Exception ex)
        {
        }
      }
      renderer.Opacity = 1f;
    }

    private void MemoryPageCache_ItemAdded(object sender, CacheItemEventArgs<ImageKey, PageImage> e)
    {
      if (!this.IsValid || !object.Equals((object) e.Key, (object) this.GetPageKey(this.CurrentPage)) && (!this.TwoPageDisplay || !object.Equals((object) e.Key, (object) this.GetPageKey(this.NextPage))) && ((IEnumerable<int>) this.DisplayedPages).Where<int>((Func<int, bool>) (dp => object.Equals((object) e.Key, (object) this.GetPageKey(dp)))).IsEmpty<int>())
        return;
      this.Invalidate();
    }

    private void book_Disposing(object sender, EventArgs e)
    {
      this.Book = (ComicBookNavigator) null;
    }

    private void book_Navigation(object sender, BookPageEventArgs e)
    {
      bool flag = e.OldPage < e.Page;
      if (this.IsFlipped)
        flag = !flag;
      switch (this.PageTransitionEffect)
      {
        case PageTransitionEffect.Fade:
          this.Blender = new ComicDisplayControl.BlendAnimationHandler(this.FadeInBlending);
          break;
        case PageTransitionEffect.LeftRight:
          this.Blender = !flag ? new ComicDisplayControl.BlendAnimationHandler(this.ScrollToRightBlending) : new ComicDisplayControl.BlendAnimationHandler(this.ScrollToLeftBlending);
          break;
        case PageTransitionEffect.TopDown:
          this.Blender = !flag ? new ComicDisplayControl.BlendAnimationHandler(this.ScrollToBottomBlending) : new ComicDisplayControl.BlendAnimationHandler(this.ScrollToTopBlending);
          break;
        case PageTransitionEffect.Paging:
          this.Blender = !flag ? new ComicDisplayControl.BlendAnimationHandler(this.PageBackward) : new ComicDisplayControl.BlendAnimationHandler(this.PageForward);
          break;
        default:
          this.Blender = (ComicDisplayControl.BlendAnimationHandler) null;
          break;
      }
      this.ShouldPagingBlend = !this.InvokeRequired && (this.BlendWhilePaging || Machine.Ticks - this.lastBlend > 100L);
      this.OnPageChange(e);
      int currentPage = this.CurrentPage;
      ImageDisplayControl.DisplayOutputConfig displayConfig = this.DisplayConfig;
      this.currentPage = this.Book.CurrentPage;
      int part = 0;
      int num = this.TwoPageDisplay ? 2 : 1;
      if (e.OldPage != -1 && Math.Abs(e.OldPage - e.Page) <= num)
        part = e.OldPage < e.Page ? 0 : this.ImagePartCount - 1;
      this.ImageVisiblePart = new ImagePartInfo(part);
      if (this.ShouldPagingBlend)
        this.BlendAnimation(currentPage, displayConfig);
      else
        this.Invalidate();
      this.OnPageChanged(e);
      this.UpdateNavigationOverlay(false);
      this.Update();
      this.lastBlend = Machine.Ticks;
    }

    private void book_PageFilterOrPagesChanged(object sender, EventArgs e)
    {
      this.Invalidate();
      this.UpdateNavigationOverlay(true);
    }

    private void Comic_BookChanged(object sender, BookChangedEventArgs e)
    {
      this.navigationOverlay.Caption = this.book.Comic.Caption;
    }

    private void book_IndexOfPageReady(object sender, BookPageEventArgs e)
    {
      try
      {
        this.UpdateNavigationOverlay(true);
        if (e.Page != this.CurrentPage)
          return;
        this.UpdateCurrentPageOverlay();
        this.Invalidate();
      }
      catch (Exception ex)
      {
      }
    }

    private void book_IndexRetrievalCompleted(object sender, EventArgs e)
    {
      this.UpdateNavigationOverlay(true);
      this.Invalidate();
    }

    private void book_ColorAdjustmentChanged(object sender, EventArgs e) => this.Invalidate();

    private void book_RightToLeftReadingChanged(object sender, EventArgs e)
    {
      if (this.Book.RightToLeftReading == YesNo.Unknown)
        return;
      this.RightToLeftReading = this.Book.RightToLeftReading == YesNo.Yes;
    }

    private void longClickTimer_Tick(object sender, EventArgs e)
    {
      if (this.AutoMagnifier)
      {
        this.magnifierOverlay.Tag = (object) true;
        this.MagnifierVisible = true;
        this.MouseActionHappened = true;
        this.DisableScrolling = true;
      }
      this.longClickTimer.Stop();
    }

    private void UpdateNavigationOverlay()
    {
      this.UpdateNavigationOverlay(this.PointToClient(Cursor.Position));
    }

    private int NavigationOverlayVisibleY
    {
      get
      {
        Rectangle clientRectangle = this.ClientRectangle;
        Rectangle bounds = this.navigationOverlay.Bounds;
        return !this.IsPageBrowsersOnTop ? clientRectangle.Height - bounds.Height - this.Margin.Bottom : this.Margin.Top;
      }
    }

    private void UpdateNavigationOverlay(System.Drawing.Point pt)
    {
      if (!this.IsValid)
      {
        this.navigationOverlay.Visible = false;
      }
      else
      {
        Rectangle clientRectangle = this.ClientRectangle;
        Rectangle bounds = this.navigationOverlay.Bounds;
        System.Drawing.Size size = new System.Drawing.Size(500, 50);
        if (this.IsPageBrowsersOnTop)
          this.UpdateNavigationOverlay(pt, (OverlayPanel) this.navigationOverlay, new System.Drawing.Point((clientRectangle.Width - bounds.Width) / 2, -bounds.Height), new System.Drawing.Point((clientRectangle.Width - bounds.Width) / 2, this.NavigationOverlayVisibleY), new Rectangle((clientRectangle.Width - size.Width) / 2, 0, size.Width, size.Height));
        else
          this.UpdateNavigationOverlay(pt, (OverlayPanel) this.navigationOverlay, new System.Drawing.Point((clientRectangle.Width - bounds.Width) / 2, clientRectangle.Height), new System.Drawing.Point((clientRectangle.Width - bounds.Width) / 2, this.NavigationOverlayVisibleY), new Rectangle((clientRectangle.Width - size.Width) / 2, clientRectangle.Height - size.Height, size.Width, size.Height));
      }
    }

    private void UpdateNavigationOverlay(
      System.Drawing.Point pt,
      OverlayPanel panel,
      System.Drawing.Point start,
      System.Drawing.Point end,
      Rectangle hotBounds)
    {
      bool flag = this.IsImageValid() && (this.NavigationOverlayVisible || this.IsNavigationOverlayEnabled && (panel.HasMouse || Control.MouseButtons == MouseButtons.None && !this.magnifierOverlay.Visible && hotBounds.Contains(pt)));
      if (flag.Equals(panel.Tag) || !flag && !panel.Visible)
        return;
      this.overlayManager.AnimationEnabled = true;
      panel.Animators.Clear();
      if (!flag)
        CloneUtility.Swap<System.Drawing.Point>(ref start, ref end);
      if (!panel.Visible)
      {
        panel.Location = start;
        panel.Visible = true;
      }
      panel.Animators.Add((Animator) new ComicDisplayControl.MoveAnimator(flag ? 300 : 200, panel.Location, end, !flag));
      panel.Tag = (object) flag;
      panel.Animators.Start();
    }

    public void BlendAnimation(
      int oldPage,
      ImageDisplayControl.DisplayOutputConfig oldConfig,
      ComicDisplayControl.BlendAnimationHandler blender,
      ComicDisplayControl.BlendAnimationMode mode = ComicDisplayControl.BlendAnimationMode.Default)
    {
      if (this.renderer != null && this.renderer.IsHardware && !this.disableBlending && blender != null && this.Visible)
      {
        this.DisplayEventsDisabled = true;
        try
        {
          if (mode != ComicDisplayControl.BlendAnimationMode.Default)
            oldPage = this.currentPage;
          int num = 50;
          while (!this.IsPageInCache(oldPage) || !this.IsPageInCache(oldPage, 1) || !this.IsPageInCache(this.currentPage) || !this.IsPageInCache(this.currentPage, 1))
          {
            if (--num < 0)
              return;
            Thread.Sleep(50);
          }
          ImageDisplayControl.DisplayOutput display = ImageDisplayControl.DisplayOutput.Create(this.DisplayConfig with
          {
            Rotation = this.LastRenderedDisplay.Config.Rotation
          }, this.CurrentAnamorphicTolerance);
          ImageDisplayControl.DisplayOutput oldOut = oldConfig.IsEmpty ? display : ImageDisplayControl.DisplayOutput.Create(oldConfig, this.CurrentAnamorphicTolerance);
          switch (mode)
          {
            case ComicDisplayControl.BlendAnimationMode.CurrentAsNew:
              oldOut = (ImageDisplayControl.DisplayOutput) null;
              break;
            case ComicDisplayControl.BlendAnimationMode.CurrentAsOld:
              oldOut = display;
              display = (ImageDisplayControl.DisplayOutput) null;
              break;
          }
          this.inBlendAnmation = true;
          this.renderer.BeginScene((Graphics) null);
          try
          {
            using (this.renderer.SaveState())
              blender(this.renderer, oldPage, oldOut, display, 0.0f);
            this.RenderImageOverlay(this.renderer, display ?? oldOut);
          }
          finally
          {
            this.renderer.EndScene();
          }
          ThreadUtility.Animate(EngineConfiguration.Default.BlendDuration, (Action<float>) (x =>
          {
            IBitmapRenderer bitmapRenderer = this.renderer;
            try
            {
              bitmapRenderer.BeginScene((Graphics) null);
              using (bitmapRenderer.SaveState())
                blender(bitmapRenderer, oldPage, oldOut, display, x);
              this.RenderImageOverlay(bitmapRenderer, display ?? oldOut);
            }
            catch (Exception ex)
            {
              if (!this.HandleRendererError(ex))
                return;
              bitmapRenderer = (IBitmapRenderer) null;
            }
            finally
            {
              try
              {
                bitmapRenderer.EndScene();
              }
              catch
              {
              }
            }
          }));
        }
        catch (Exception ex)
        {
          this.HandleRendererError(ex);
        }
        finally
        {
          this.DisplayEventsDisabled = false;
          this.inBlendAnmation = false;
        }
      }
      this.Invalidate();
    }

    public void BlendAnimation(int oldPage, ImageDisplayControl.DisplayOutputConfig oldConfig)
    {
      this.BlendAnimation(oldPage, oldConfig, this.Blender);
    }

    private void RenderImageBackground(
      IBitmapRenderer bitmapRenderer,
      ImageDisplayControl.DisplayOutput output,
      int page = -1)
    {
      int currentPage = this.currentPage;
      if (page != -1)
        this.currentPage = page;
      try
      {
        this.RenderImageBackground(bitmapRenderer, output);
      }
      finally
      {
        this.currentPage = currentPage;
      }
    }

    public void FadeInBlending(
      IBitmapRenderer hr,
      int oldPage,
      ImageDisplayControl.DisplayOutput oldOut,
      ImageDisplayControl.DisplayOutput display,
      float percent)
    {
      float opacity = hr.Opacity;
      float num = percent.Clamp(0.05f, 1f);
      RectangleF clip = hr.Clip;
      if (this.currentPage < oldPage)
      {
        this.RenderImageBackground(hr, display, -1);
        if (!this.IsConstantBackground)
        {
          hr.Opacity = 1f - num;
          this.RenderImageBackground(hr, oldOut, oldPage);
        }
        hr.Opacity = num;
        this.RenderImageSafe(hr, display, false);
        hr.Opacity = 1f;
        hr.Clip = (RectangleF) oldOut.OutputBoundsScreen;
        this.RenderImageSafe(hr, display, false);
        hr.Clip = clip;
        hr.Opacity = 1f - num;
        this.RenderImageSafe(hr, oldOut, oldPage, false);
      }
      else
      {
        this.RenderImageBackground(hr, oldOut, oldPage);
        if (!this.IsConstantBackground)
        {
          hr.Opacity = num;
          this.RenderImageBackground(hr, display, -1);
        }
        hr.Opacity = 1f - num;
        this.RenderImageSafe(hr, oldOut, oldPage, false);
        hr.Opacity = 1f;
        hr.Clip = (RectangleF) display.OutputBoundsScreen;
        this.RenderImageSafe(hr, oldOut, oldPage, false);
        hr.Clip = clip;
        hr.Opacity = num;
        this.RenderImageSafe(hr, display, false);
      }
      hr.Opacity = opacity;
    }

    public void PageForward(
      IBitmapRenderer hr,
      int oldPage,
      ImageDisplayControl.DisplayOutput oldOut,
      ImageDisplayControl.DisplayOutput display,
      float percent)
    {
      bool withBackground = true;
      Rectangle outputBoundsScreen = oldOut.OutputBoundsScreen;
      int width = outputBoundsScreen.Width;
      outputBoundsScreen = oldOut.OutputBoundsScreen;
      int height = outputBoundsScreen.Height;
      bool flag1 = width >= height;
      bool flag2 = oldOut.Config.Rotation != ImageRotation.None || display.Config.Rotation != 0;
      bool flag3 = !flag1 | flag2;
      Rectangle rectangle1;
      if (!this.IsConstantBackground | flag2)
      {
        rectangle1 = this.ClientRectangle;
      }
      else
      {
        rectangle1 = Rectangle.Union(display.OutputBoundsScreen, oldOut.OutputBoundsScreen).Pad(-10);
        this.RenderImageBackground(hr, (ImageDisplayControl.DisplayOutput) null, -1);
        withBackground = false;
        hr.Clip = (RectangleF) rectangle1;
      }
      Rectangle rectangle2 = rectangle1;
      float num = (float) rectangle2.Width * percent;
      this.RenderImageSafe(hr, oldOut, oldPage, withBackground ? ImageDisplayControl.RenderType.Default : ImageDisplayControl.RenderType.WithoutBackground);
      if (flag3)
      {
        num *= 2f;
        this.drawBlankPagesOverride = true;
      }
      this.innerBowLeftOffsetInPercent = 1f - percent;
      hr.TranslateTransform((float) rectangle2.Width - num, 0.0f);
      this.RenderImageSafe(hr, display, withBackground);
      hr.TranslateTransform((float) -((double) rectangle2.Width - (double) num), 0.0f);
      this.innerBowLeftOffsetInPercent = 0.0f;
      this.drawBlankPagesOverride = false;
      if ((double) num > 5.0)
      {
        hr.Clip = new RectangleF((float) rectangle2.Right - num / 2f, (float) rectangle2.Top, (float) ((double) num / 2.0 + 1.0), (float) (rectangle2.Height + 1));
        this.RenderImageSafe(hr, display);
      }
      hr.Clip = (RectangleF) Rectangle.Empty;
    }

    public void PageBackward(
      IBitmapRenderer hr,
      int oldPage,
      ImageDisplayControl.DisplayOutput oldOut,
      ImageDisplayControl.DisplayOutput display,
      float percent)
    {
      if (EngineConfiguration.Default.MirroredPageTurnAnimation)
      {
        int currentPage = this.currentPage;
        this.currentPage = oldPage;
        this.PageForward(hr, currentPage, display, oldOut, 1f - percent);
        this.currentPage = currentPage;
      }
      else
      {
        bool withBackground = true;
        bool flag1 = oldOut.OutputBoundsScreen.Width >= oldOut.OutputBoundsScreen.Height;
        ImageDisplayControl.DisplayOutputConfig config = oldOut.Config;
        int num1;
        if (config.Rotation == ImageRotation.None)
        {
          config = display.Config;
          num1 = config.Rotation != 0 ? 1 : 0;
        }
        else
          num1 = 1;
        bool flag2 = num1 != 0;
        bool flag3 = !flag1 | flag2;
        Rectangle rectangle1;
        if (!this.IsConstantBackground | flag2)
        {
          rectangle1 = this.ClientRectangle;
        }
        else
        {
          rectangle1 = Rectangle.Union(display.OutputBoundsScreen, oldOut.OutputBoundsScreen).Pad(-10);
          this.RenderImageBackground(hr, (ImageDisplayControl.DisplayOutput) null, -1);
          hr.Clip = (RectangleF) rectangle1;
          withBackground = false;
        }
        Rectangle rectangle2 = rectangle1;
        float num2 = (float) rectangle2.Width * percent;
        this.RenderImageSafe(hr, oldOut, oldPage, withBackground);
        if (flag3)
        {
          num2 *= 2f;
          this.drawBlankPagesOverride = true;
        }
        this.innerBowRightOffsetInPercent = 1f - percent;
        hr.TranslateTransform((float) -((double) rectangle2.Width - (double) num2), 0.0f);
        this.RenderImageSafe(hr, display, withBackground);
        hr.TranslateTransform((float) rectangle2.Width - num2, 0.0f);
        this.drawBlankPagesOverride = false;
        this.innerBowRightOffsetInPercent = 0.0f;
        if ((double) num2 > 5.0)
        {
          hr.Clip = new RectangleF((float) rectangle2.Left, (float) rectangle2.Top, (float) ((double) num2 / 2.0 + 1.0), (float) (rectangle2.Height + 1));
          this.RenderImageSafe(hr, display);
        }
        hr.Clip = (RectangleF) Rectangle.Empty;
      }
    }

    public void ScrollToLeftBlending(
      IBitmapRenderer hr,
      int oldPage,
      ImageDisplayControl.DisplayOutput oldOut,
      ImageDisplayControl.DisplayOutput display,
      float percent)
    {
      this.RenderImageBackground(hr, (ImageDisplayControl.DisplayOutput) null, -1);
      hr.TranslateTransform((float) -this.ClientRectangle.Width * percent, 0.0f);
      this.RenderImageSafe(hr, oldOut, oldPage, !this.IsConstantBackground);
      hr.TranslateTransform((float) this.ClientRectangle.Width, 0.0f);
      this.RenderImageSafe(this.renderer, display, !this.IsConstantBackground);
    }

    public void ScrollToRightBlending(
      IBitmapRenderer hr,
      int oldPage,
      ImageDisplayControl.DisplayOutput oldOut,
      ImageDisplayControl.DisplayOutput display,
      float percent)
    {
      this.RenderImageBackground(hr, (ImageDisplayControl.DisplayOutput) null, -1);
      hr.TranslateTransform((float) this.ClientRectangle.Width * percent, 0.0f);
      this.RenderImageSafe(hr, oldOut, oldPage, !this.IsConstantBackground);
      hr.TranslateTransform((float) -this.ClientRectangle.Width, 0.0f);
      this.RenderImageSafe(this.renderer, display, !this.IsConstantBackground);
    }

    public void ScrollToBottomBlending(
      IBitmapRenderer hr,
      int oldPage,
      ImageDisplayControl.DisplayOutput oldOut,
      ImageDisplayControl.DisplayOutput display,
      float percent)
    {
      this.RenderImageBackground(hr, (ImageDisplayControl.DisplayOutput) null, -1);
      hr.TranslateTransform(0.0f, (float) this.ClientRectangle.Height * percent);
      this.RenderImageSafe(hr, oldOut, oldPage, !this.IsConstantBackground);
      hr.TranslateTransform(0.0f, (float) -this.ClientRectangle.Height);
      this.RenderImageSafe(this.renderer, display, !this.IsConstantBackground);
    }

    public void ScrollToTopBlending(
      IBitmapRenderer hr,
      int oldPage,
      ImageDisplayControl.DisplayOutput oldOut,
      ImageDisplayControl.DisplayOutput display,
      float percent)
    {
      this.RenderImageBackground(hr, (ImageDisplayControl.DisplayOutput) null, -1);
      hr.TranslateTransform(0.0f, (float) -this.ClientRectangle.Height * percent);
      this.RenderImageSafe(hr, oldOut, oldPage, !this.IsConstantBackground);
      hr.TranslateTransform(0.0f, (float) this.ClientRectangle.Height);
      this.RenderImageSafe(this.renderer, display, !this.IsConstantBackground);
    }

    private void RenderImageSafe(
      IBitmapRenderer bitmapRenderer,
      ImageDisplayControl.DisplayOutput output,
      int page,
      ImageDisplayControl.RenderType renderType = ImageDisplayControl.RenderType.Default)
    {
      if (output == null)
        return;
      int currentPage = this.currentPage;
      this.currentPage = page;
      try
      {
        this.RenderImageSafe(bitmapRenderer, output, renderType);
      }
      finally
      {
        this.currentPage = currentPage;
      }
    }

    private void RenderImageSafe(
      IBitmapRenderer bitmapRenderer,
      ImageDisplayControl.DisplayOutput output,
      int page,
      bool withBackground)
    {
      this.RenderImageSafe(bitmapRenderer, output, page, withBackground ? ImageDisplayControl.RenderType.Default : ImageDisplayControl.RenderType.WithoutBackground);
    }

    private void RenderImageSafe(
      IBitmapRenderer bitmapRenderer,
      ImageDisplayControl.DisplayOutput output,
      bool withBackground)
    {
      this.RenderImageSafe(bitmapRenderer, output, withBackground ? ImageDisplayControl.RenderType.Default : ImageDisplayControl.RenderType.WithoutBackground);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.imageScaleTimer = new System.Windows.Forms.Timer(this.components);
      this.longClickTimer = new System.Windows.Forms.Timer(this.components);
      this.cacheUpdateTimer = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      this.imageScaleTimer.Interval = 500;
      this.imageScaleTimer.Tick += new EventHandler(this.imageScaleTimer_Tick);
      this.longClickTimer.Interval = 500;
      this.longClickTimer.Tick += new EventHandler(this.longClickTimer_Tick);
      this.cacheUpdateTimer.Interval = 2000;
      this.cacheUpdateTimer.Tick += new EventHandler(this.cacheUpdateTimer_Tick);
      this.ResumeLayout(false);
    }

    public enum BlendAnimationMode
    {
      Default,
      CurrentAsNew,
      CurrentAsOld,
    }

    public class ImageInfo
    {
      public int Width { get; set; }

      public int Height { get; set; }

      public int ImageCount { get; set; }

      public bool IsForcedDoublePage { get; set; }

      public bool IsSingleImage => this.ImageCount == 1;

      public bool IsDoubleImage => this.ImageCount > 1;

      public bool IsDoublePage => this.Width > this.Height;

      public bool IsValid => !this.Size.IsEmpty;

      public System.Drawing.Size Size
      {
        get => new System.Drawing.Size(this.Width, this.Height);
        set
        {
          this.Width = value.Width;
          this.Height = value.Height;
        }
      }
    }

    public delegate void BlendAnimationHandler(
      IBitmapRenderer hr,
      int oldPage,
      ImageDisplayControl.DisplayOutput oldOut,
      ImageDisplayControl.DisplayOutput display,
      float percent);

    private class ScaledPageKey
    {
      private WeakReference<PageImage> wrf;

      public float ScaleX { get; set; }

      public float ScaleY { get; set; }

      public PageImage Bitmap
      {
        get => this.wrf.GetData<PageImage>();
        set => this.wrf = new WeakReference<PageImage>(value);
      }

      public override bool Equals(object obj)
      {
        return obj is ComicDisplayControl.ScaledPageKey scaledPageKey && (double) scaledPageKey.ScaleX == (double) this.ScaleX && (double) scaledPageKey.ScaleY == (double) this.ScaleY && scaledPageKey.Bitmap == this.Bitmap;
      }

      public override int GetHashCode()
      {
        float num = this.ScaleX;
        int hashCode1 = num.GetHashCode();
        num = this.ScaleY;
        int hashCode2 = num.GetHashCode();
        return hashCode1 ^ hashCode2;
      }
    }

    private class ScaledPageItem : MemoryOptimizedImage
    {
      public ScaledPageItem()
        : base((Bitmap) null)
      {
      }

      public long Ticks { get; set; }
    }

    private struct Magnifier
    {
      public Bitmap Bitmap;
      public Rectangle Inner;
      public Rectangle Outer;
    }

    private class MoveAnimator : Animator
    {
      public MoveAnimator(int time, System.Drawing.Point fromPoint, System.Drawing.Point toPoint, bool outTransition)
      {
        if (!outTransition)
          this.Delay = 500;
        this.Span = time;
        this.AnimationValueGenerator = new AnimationValueHandler(Animator.SinusRise);
        System.Drawing.Point dp = new System.Drawing.Point(toPoint.X - fromPoint.X, toPoint.Y - fromPoint.Y);
        this.AnimationHandler = (AnimationHandler) ((p, t, d) =>
        {
          p.X = fromPoint.X + (int) ((double) t * (double) dp.X);
          p.Y = fromPoint.Y + (int) ((double) t * (double) dp.Y);
          if (!outTransition || (double) t < 1.0)
            return;
          p.Visible = false;
        });
      }
    }

    private class SizeAnimator : Animator
    {
      public SizeAnimator(int time, System.Drawing.Point toPoint, bool outTransition)
      {
        this.Span = time;
        this.AnimationValueGenerator = new AnimationValueHandler(Animator.LinearRise);
        this.AnimationHandler = (AnimationHandler) ((p, t, d) =>
        {
          p.Scale = outTransition ? 1f - t : t;
          Rectangle bounds = p.Bounds;
          Rectangle physicalBounds = p.PhysicalBounds;
          p.X = toPoint.X + (physicalBounds.Width - bounds.Width) / 2;
          p.Y = toPoint.Y + physicalBounds.Height - bounds.Height;
          if (!outTransition || (double) t < 1.0)
            return;
          p.Visible = false;
        });
      }
    }
  }
}

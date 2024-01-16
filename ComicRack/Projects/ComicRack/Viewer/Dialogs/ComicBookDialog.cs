// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ComicBookDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Net.Search;
using cYo.Common.Text;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Plugins;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ComicBookDialog : Form
  {
    private static int lastActivePage = -1;
    private static int lastActiveTextPage = -1;
    private static bool showColorControls = false;
    private ComicBook current;
    private ComicBook displayComic;
    private readonly ComicBook[] allBooks;
    private Func<ComicBook, bool> selectComicHandler;
    private Control currentTextBox;
    private int pendingPageViewerPage = -1;
    private int pageViewPage = -1;
    private string customThumbnailKey;
    private static int currentScript;
    private static ItemViewConfig pagesConfig = new ItemViewConfig();
    private static PluginEngine scriptEngine;
    private IContainer components;
    private TextBoxEx txVolume;
    private TextBoxEx txYear;
    private TextBoxEx txSeries;
    private TextBoxEx txCount;
    private TextBoxEx txNumber;
    private Button btCancel;
    private Button btOK;
    private TabControl tabControl;
    private TabPage tabSummary;
    private Label labelWhere;
    private Label labelPages;
    private Label lblPath;
    private Label lblPages;
    private Label labelType;
    private Label lblType;
    private Button btPrev;
    private Button btNext;
    private TabPage tabDetails;
    private Label labelVolume;
    private Label labelYear;
    private Label labelSeries;
    private RatingControl txRating;
    private TextBoxEx txWriter;
    private TextBoxEx txColorist;
    private TextBoxEx txInker;
    private TextBoxEx txPenciller;
    private Label labelGenre;
    private Label labelColorist;
    private Label labelPublisher;
    private TextBoxEx txTitle;
    private Label labelInker;
    private Label labelCount;
    private Label labelNumber;
    private Label labelWriter;
    private Label labelPenciller;
    private Label labelTitle;
    private Panel panel1;
    private TextBoxEx txLetterer;
    private Label labelLetterer;
    private TextBoxEx txEditor;
    private TextBoxEx txCoverArtist;
    private Label labelEditor;
    private Label labelCoverArtist;
    private ComboBox cbPublisher;
    private TrackBarLite tbBrightness;
    private TrackBarLite tbSaturation;
    private TrackBarLite tbContrast;
    private Label labelSaturation;
    private Label labelContrast;
    private Label labelBrightness;
    private ThumbnailControl coverThumbnail;
    private Button btResetColors;
    private TextBoxEx txAlternateSeries;
    private Label labelAlternateSeries;
    private TextBoxEx txAlternateCount;
    private TextBoxEx txAlternateNumber;
    private Label labelAlternateCount;
    private Label labelAlternateNumber;
    private TextBoxEx txMonth;
    private Label labelMonth;
    private TextBoxEx txTags;
    private Label labelImprint;
    private ComboBox cbImprint;
    private TabPage tabPages;
    private LanguageComboBox cbLanguage;
    private Label labelLanguage;
    private BitmapViewer pageViewer;
    private TabPage tabColors;
    private PagesView pagesView;
    private Label labelPagesInfo;
    private ComboBoxEx cbFormat;
    private Label labelFormat;
    private ComboBox cbBlackAndWhite;
    private ComboBox cbManga;
    private Label labelBlackAndWhite;
    private Label labelManga;
    private ComboBox cbEnableProposed;
    private Label labelEnableProposed;
    private Button btPageView;
    private ComboBox cbAgeRating;
    private Label labelAgeRating;
    private Label labelTags;
    private TabPage tabPlot;
    private TextBoxEx txCharacters;
    private Label labelCharacters;
    private TextBoxEx txGenre;
    private SplitButton btScript;
    private TrackBarLite tbSharpening;
    private Label labelSharpening;
    private ToolTip toolTip;
    private TextBoxEx txWeblink;
    private Label labelWeb;
    private LinkLabel linkLabel;
    private ComboBox cbEnableDynamicUpdate;
    private Label labelEnableDynamicUpdate;
    private TextBoxEx txLocations;
    private Label labelLocations;
    private TextBoxEx txTeams;
    private Label labelTeams;
    private Button btLinkFile;
    private Label labelCommunityRating;
    private RatingControl txCommunityRating;
    private Label labelMyRating;
    private Panel whereSeparator;
    private SplitButton btThumbnail;
    private ContextMenuStrip cmThumbnail;
    private ToolStripMenuItem miResetThumbnail;
    private ToolStripSeparator toolStripMenuItem1;
    private TabPage tabCatalog;
    private ComboBox cbBookPrice;
    private Label labelBookPrice;
    private TextBoxEx txBookNotes;
    private Label labelBookNotes;
    private ComboBox cbBookAge;
    private Label labelBookAge;
    private Label labelBookCollectionStatus;
    private ComboBox cbBookCondition;
    private Label labelBookCondition;
    private ComboBoxEx cbBookStore;
    private Label labelBookStore;
    private ComboBox cbBookOwner;
    private Label labelBookOwner;
    private TextBoxEx txCollectionStatus;
    private ComboBox cbBookLocation;
    private Label labelBookLocation;
    private TextBoxEx txISBN;
    private Label labelISBN;
    private TextBoxEx txPagesAsTextSimple;
    private Label labelPagesAsTextSimple;
    private Label labelOpenedTime;
    private NullableDateTimePicker dtpOpenedTime;
    private Label labelAddedTime;
    private NullableDateTimePicker dtpAddedTime;
    private ComboBox cbSeriesComplete;
    private Label labelSeriesComplete;
    private TrackBarLite tbGamma;
    private Label labelGamma;
    private Panel panelImage;
    private CheckBox chkShowImageControls;
    private Button btLastPage;
    private Button btFirstPage;
    private AutoRepeatButton btNextPage;
    private AutoRepeatButton btPrevPage;
    private Panel panelImageControls;
    private Label labelCurrentPage;
    private TextBoxEx txScanInformation;
    private Label labelScanInformation;
    private Label labelSeriesGroup;
    private TextBoxEx txSeriesGroup;
    private Label labelStoryArc;
    private TextBoxEx txStoryArc;
    private TabControl tabNotes;
    private TabPage tabPageSummary;
    private TextBoxEx txSummary;
    private TabPage tabPageNotes;
    private TextBoxEx txNotes;
    private TabPage tabPageReview;
    private TextBoxEx txReview;
    private TextBoxEx txMainCharacterOrTeam;
    private Label labelMainCharacterOrTeam;
    private SplitButton btResetPages;
    private ContextMenuStrip cmResetPages;
    private ToolStripMenuItem miOrderByName;
    private ToolStripMenuItem miOrderByNameNumeric;
    private Button btApply;
    private TextBoxEx txDay;
    private Label labelDay;
    private Label labelReleasedTime;
    private NullableDateTimePicker dtpReleasedTime;
    private TabPage tabCustom;
    private DataGridView customValuesData;
    private DataGridViewTextBoxColumn CustomValueName;
    private DataGridViewTextBoxColumn CustomValueValue;

    private ComicBookDialog(ComicBook current, ComicBook[] allBooks)
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.btFirstPage.Image = (Image) ((Bitmap) this.btFirstPage.Image).ScaleDpi();
      this.btLastPage.Image = (Image) ((Bitmap) this.btLastPage.Image).ScaleDpi();
      this.btNextPage.Image = (Image) ((Bitmap) this.btNextPage.Image).ScaleDpi();
      this.btPrevPage.Image = (Image) ((Bitmap) this.btPrevPage.Image).ScaleDpi();
      this.chkShowImageControls.Image = (Image) ((Bitmap) this.chkShowImageControls.Image).ScaleDpi();
      this.btPageView.Image = (Image) ((Bitmap) this.btPageView.Image).ScaleDpi();
      this.RestorePosition();
      this.coverThumbnail.MouseWheel += new MouseEventHandler(this.coverThumbnail_MouseWheel);
      TextBoxContextMenu.Register((Form) this, TextBoxContextMenu.AddSearchLinks((IEnumerable<INetSearch>) SearchEngines.Engines));
      LocalizeUtility.Localize((Control) this, this.components);
      this.pageViewer.Text = TR.Load(this.Name)[this.pageViewer.Name, this.pageViewer.Text];
      ComicListField.TranslateColumns((IEnumerable<IColumn>) this.pagesView.ItemView.Columns);
      if (ComicBookDialog.PagesConfig != null)
        this.pagesView.ViewConfig = ComicBookDialog.PagesConfig;
      ComboBoxSkinner comboBoxSkinner1 = new ComboBoxSkinner(this.cbImprint, (IImagePackage) ComicBook.PublisherIcons);
      ComboBoxSkinner comboBoxSkinner2 = new ComboBoxSkinner(this.cbPublisher, (IImagePackage) ComicBook.PublisherIcons);
      ComboBoxSkinner comboBoxSkinner3 = new ComboBoxSkinner((ComboBox) this.cbFormat, (IImagePackage) ComicBook.FormatIcons);
      ComboBoxSkinner comboBoxSkinner4 = new ComboBoxSkinner(this.cbAgeRating, (IImagePackage) ComicBook.AgeRatingIcons);
      ComboBoxSkinner comboBoxSkinner5 = new ComboBoxSkinner(this.cbBookPrice);
      ComboBoxSkinner comboBoxSkinner6 = new ComboBoxSkinner(this.cbBookOwner);
      ComboBoxSkinner comboBoxSkinner7 = new ComboBoxSkinner((ComboBox) this.cbBookStore);
      ComboBoxSkinner comboBoxSkinner8 = new ComboBoxSkinner(this.cbBookAge);
      ComboBoxSkinner comboBoxSkinner9 = new ComboBoxSkinner(this.cbBookCondition);
      ComboBoxSkinner comboBoxSkinner10 = new ComboBoxSkinner(this.cbBookLocation);
      ListSelectorControl.Register((IEnumerable<INetSearch>) SearchEngines.Engines, (TextBox) this.txWriter, (TextBox) this.txPenciller, (TextBox) this.txInker, (TextBox) this.txColorist, (TextBox) this.txEditor, (TextBox) this.txCoverArtist, (TextBox) this.txLetterer, (TextBox) this.txGenre, (TextBox) this.txTags, (TextBox) this.txCharacters, (TextBox) this.txTeams, (TextBox) this.txLocations, (TextBox) this.txCollectionStatus);
      EditControlUtility.InitializeMangaYesNo(this.cbManga);
      EditControlUtility.InitializeYesNo(this.cbBlackAndWhite);
      EditControlUtility.InitializeYesNo(this.cbSeriesComplete);
      EditControlUtility.InitializeYesNo(this.cbEnableProposed, false);
      EditControlUtility.InitializeYesNo(this.cbEnableDynamicUpdate, false);
      if (allBooks == null)
        allBooks = new ComicBook[1]{ current };
      this.allBooks = allBooks;
      this.pagesView.PageFilter = ComicPageType.AllWithDeleted;
      this.pagesView.ItemView.SelectedIndexChanged += new EventHandler(this.PagesViewSelectedIndexChanged);
      this.pagesView.ItemView.ItemActivate += new EventHandler(this.PagesViewItemActivate);
      this.coverThumbnail.TextElements = ComicTextElements.CaptionWithoutTitle | ComicTextElements.Title | ComicTextElements.ArtistInfo | ComicTextElements.Summary | ComicTextElements.Opened | ComicTextElements.Added | ComicTextElements.Released | ComicTextElements.NoEmptyDates;
      this.coverThumbnail.DrawingFlags &= ~ThumbnailDrawingOptions.EnableRating;
      this.coverThumbnail.DrawingFlags &= ~ThumbnailDrawingOptions.EnableBackground;
      this.coverThumbnail.HighQuality = (Program.Settings.PageImageDisplayOptions & ImageDisplayOptions.HighQuality) != 0;
      EditControlUtility.SetText((TextBox) this.txTitle, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ShadowTitle))));
      EditControlUtility.SetText((TextBox) this.txSeries, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ShadowSeries))));
      EditControlUtility.SetText((TextBox) this.txWriter, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Writer))));
      EditControlUtility.SetText((TextBox) this.txGenre, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetGenreList(false)));
      EditControlUtility.SetText((TextBox) this.txTags, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Tags))));
      EditControlUtility.SetText((TextBox) this.txAlternateSeries, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.AlternateSeries))));
      EditControlUtility.SetText((TextBox) this.txStoryArc, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.StoryArc))));
      EditControlUtility.SetText((TextBox) this.txSeriesGroup, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.SeriesGroup))));
      EditControlUtility.SetText((TextBox) this.txPenciller, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Penciller))));
      EditControlUtility.SetText((TextBox) this.txColorist, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Colorist))));
      EditControlUtility.SetText((TextBox) this.txInker, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Inker))));
      EditControlUtility.SetText((TextBox) this.txLetterer, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Letterer))));
      EditControlUtility.SetText((TextBox) this.txCoverArtist, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.CoverArtist))));
      EditControlUtility.SetText((TextBox) this.txEditor, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Editor))));
      EditControlUtility.SetText((TextBox) this.txCharacters, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Characters))));
      EditControlUtility.SetText((TextBox) this.txTeams, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Teams))));
      EditControlUtility.SetText((TextBox) this.txMainCharacterOrTeam, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.MainCharacterOrTeam))));
      EditControlUtility.SetText((TextBox) this.txLocations, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Locations))));
      EditControlUtility.SetText((TextBox) this.txScanInformation, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ScanInformation))));
      EditControlUtility.SetText(this.cbAgeRating, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetAgeRatingList()));
      EditControlUtility.SetText((ComboBox) this.cbFormat, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetFormatList()));
      EditControlUtility.SetText(this.cbPublisher, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Publisher), true)));
      EditControlUtility.SetText(this.cbImprint, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Imprint), true)));
      EditControlUtility.SetText(this.cbBookAge, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookAgeList()));
      EditControlUtility.SetText((ComboBox) this.cbBookStore, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookStore), true)));
      EditControlUtility.SetText(this.cbBookOwner, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookOwner), true)));
      EditControlUtility.SetText(this.cbBookCondition, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookConditionList()));
      EditControlUtility.SetText(this.cbBookPrice, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookPriceAsText), true)));
      EditControlUtility.SetText(this.cbBookLocation, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookLocation), true)));
      EditControlUtility.SetText((TextBox) this.txCollectionStatus, (string) null, new Func<AutoCompleteStringCollection>(Program.Lists.GetBookCollectionStatusList));
      this.coverThumbnail.ThreeD = Program.Settings.InformationCover3D;
      this.InitializeScriptButton();
      this.ForEachControl<TextBox>((Action<TextBox>) (tb => tb.Enter += (EventHandler) ((s, e) => this.currentTextBox = (Control) tb)));
      this.ForEachControl<ComboBox>((Action<ComboBox>) (tb => tb.Enter += (EventHandler) ((s, e) => this.currentTextBox = (Control) tb)));
      SpinButton.AddUpDown((TextBoxBase) this.txVolume);
      SpinButton.AddUpDown((TextBoxBase) this.txCount, min: 0);
      SpinButton.AddUpDown((TextBoxBase) this.txNumber);
      TextBoxEx txYear = this.txYear;
      DateTime now = DateTime.Now;
      int year = now.Year;
      SpinButton.AddUpDown((TextBoxBase) txYear, year);
      TextBoxEx txMonth = this.txMonth;
      now = DateTime.Now;
      int month1 = now.Month;
      SpinButton.AddUpDown((TextBoxBase) txMonth, month1, 1, 12);
      TextBoxEx txDay = this.txDay;
      now = DateTime.Now;
      int month2 = now.Month;
      SpinButton.AddUpDown((TextBoxBase) txDay, month2, 1, 31);
      SpinButton.AddUpDown((TextBoxBase) this.txAlternateCount, min: 0);
      SpinButton.AddUpDown((TextBoxBase) this.txAlternateNumber);
      SpinButton.AddUpDown((TextBoxBase) this.txPagesAsTextSimple, min: 1, hidden: true);
      this.txVolume.EnableOnlyNumberKeys();
      this.txCount.EnableOnlyNumberKeys();
      this.txYear.EnableOnlyNumberKeys();
      this.txMonth.EnableOnlyNumberKeys();
      this.txDay.EnableOnlyNumberKeys();
      this.txAlternateCount.EnableOnlyNumberKeys();
      this.txPagesAsTextSimple.EnableOnlyNumberKeys();
      this.SetCurrentBook(current);
      IdleProcess.Idle += new EventHandler(this.IdleProcess_Idle);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        Program.Settings.InformationCover3D = this.coverThumbnail.ThreeD;
        IdleProcess.Idle -= new EventHandler(this.IdleProcess_Idle);
        ComicBookDialog.PagesConfig = this.pagesView.ViewConfig;
        this.pageViewer.SetBitmap((Bitmap) null);
        this.coverThumbnail.SetBitmap((Bitmap) null);
        if (this.pagesView.Book != null)
          this.pagesView.Book.Dispose();
        if (this.btScript.ContextMenuStrip != null)
          FormUtility.SafeToolStripClear(this.btScript.ContextMenuStrip.Items);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void SetCurrentBook(ComicBook comic, bool withRefresh = true)
    {
      Func<ComicBook, bool> selectComicHandler = this.selectComicHandler;
      if (selectComicHandler != null)
      {
        int num = selectComicHandler(comic) ? 1 : 0;
      }
      if (withRefresh)
        comic.RefreshInfoFromFile();
      this.SetComicToEditor(this.current = comic);
      this.btPrev.Visible = this.allBooks.Length > 1;
      this.btNext.Visible = this.allBooks.Length > 1;
      this.btPrev.Enabled = Array.IndexOf<ComicBook>(this.allBooks, comic) > 0;
      this.btNext.Enabled = Array.IndexOf<ComicBook>(this.allBooks, comic) < this.allBooks.Length - 1;
    }

    private void SetComicToEditor(ComicBook comic)
    {
      this.displayComic = comic;
      this.Text = comic.Caption;
      ((IEnumerable<Control>) new Control[4]
      {
        (Control) this.labelType,
        (Control) this.lblType,
        (Control) this.labelPages,
        (Control) this.lblPages
      }).ForEach<Control>((Action<Control>) (c => c.Visible = comic.IsLinked));
      SplitButton btThumbnail = this.btThumbnail;
      bool flag1;
      this.btLinkFile.Visible = flag1 = !comic.IsLinked && comic.EditMode.IsLocalComic();
      int num1;
      bool flag2 = (num1 = flag1 ? 1 : 0) != 0;
      btThumbnail.Visible = num1 != 0;
      this.AllowDrop = flag2;
      Label labelWhere = this.labelWhere;
      Panel whereSeparator = this.whereSeparator;
      bool flag3;
      this.lblPath.Visible = flag3 = comic.IsLinked && comic.EditMode.IsLocalComic();
      int num2;
      bool flag4 = (num2 = flag3 ? 1 : 0) != 0;
      whereSeparator.Visible = num2 != 0;
      int num3 = flag4 ? 1 : 0;
      labelWhere.Visible = num3 != 0;
      this.customThumbnailKey = comic.CustomThumbnailKey;
      this.coverThumbnail.ComicBook = comic;
      this.pageViewer.SetBitmap((Bitmap) null);
      this.coverThumbnail.SetBitmap((Bitmap) null);
      ComicBookDialog.SetCoverThumbnailImage((IBitmapDisplayControl) this.coverThumbnail, comic);
      this.SetPageView(comic.CurrentPage);
      this.SetDataToEditor(comic);
      ComicBookNavigator book = this.pagesView.Book;
      ComicBook comicBook = new ComicBook(comic);
      this.pagesView.Book = comicBook.CreateNavigator();
      if (this.pagesView.Book != null)
      {
        if (comicBook.IsDynamicSource)
          Program.ImagePool.RefreshLastImage(comicBook.FilePath);
        this.pagesView.Book.Open(true, 0);
      }
      book?.Dispose();
      bool flag5 = !comic.IsLinked || comic.IsInContainer;
      bool flag6 = flag5 || Program.Settings.UpdateComicFiles;
      bool flag7 = comic.EditMode.CanEditProperties();
      this.tabDetails.Enabled = this.tabPlot.Enabled = this.tabColors.Enabled = flag6 & flag7;
      this.tabPages.Enabled = flag6 && comic.EditMode.CanEditPages();
      this.tabCatalog.Enabled = this.tabCustom.Enabled = flag5 & flag7;
      this.EnableTabPage(this.tabPages, comic.IsLinked);
      this.EnableTabPage(this.tabColors, comic.IsLinked);
      this.EnableTabPage(this.tabCatalog, ((!comic.IsLinked ? 1 : (!Program.Settings.CatalogOnlyForFileless ? 1 : 0)) & (flag5 ? 1 : 0)) != 0);
      this.EnableTabPage(this.tabCustom, Program.Settings.ShowCustomBookFields & flag5);
      this.labelEnableProposed.Visible = this.cbEnableProposed.Visible = this.labelScanInformation.Visible = this.txScanInformation.Visible = comic.IsLinked;
      this.labelOpenedTime.Visible = this.dtpOpenedTime.Visible = this.labelPagesAsTextSimple.Visible = this.txPagesAsTextSimple.Visible = !comic.IsLinked;
      this.txCommunityRating.Enabled = this.txRating.Enabled = this.txTags.Enabled = this.cbEnableProposed.Enabled = this.cbSeriesComplete.Enabled = flag5;
      if (!flag7)
        this.txCommunityRating.Enabled = this.txRating.Enabled = false;
      this.linkLabel.Text = comic.Web;
    }

    private void EnableTabPage(TabPage tabPage, bool enable)
    {
      if (enable)
      {
        if (this.tabControl.TabPages.Contains(tabPage))
          return;
        this.tabControl.TabPages.Add(tabPage);
      }
      else
        this.tabControl.TabPages.Remove(tabPage);
    }

    private void SetDataToEditor(ComicBook comic)
    {
      string text = comic.PagesAsText;
      if (comic.LastPageRead > 0)
        text = (comic.LastPageRead + 1).ToString() + "/" + text;
      EditControlUtility.SetLabel(this.lblPages, text);
      EditControlUtility.SetLabel(this.lblType, string.Format("{0}/{1}", (object) comic.FileFormat, (object) comic.FileSizeAsText));
      EditControlUtility.SetLabel(this.lblPath, comic.FilePath);
      EditControlUtility.SetText((Control) this.txRating, comic.Rating);
      EditControlUtility.SetText((Control) this.txCommunityRating, comic.CommunityRating);
      EditControlUtility.SetText((Control) this.txTitle, comic.Title);
      EditControlUtility.SetText((Control) this.txSeries, comic.Series);
      EditControlUtility.SetText((Control) this.txVolume, comic.Volume);
      EditControlUtility.SetText((Control) this.txYear, comic.Year);
      EditControlUtility.SetText((Control) this.txMonth, comic.Month);
      EditControlUtility.SetText((Control) this.txDay, comic.Day);
      EditControlUtility.SetText((Control) this.txWriter, comic.Writer);
      EditControlUtility.SetText((Control) this.txNumber, comic.Number);
      EditControlUtility.SetText((Control) this.txCount, comic.Count);
      EditControlUtility.SetText((Control) this.txGenre, comic.Genre);
      EditControlUtility.SetText((Control) this.cbSeriesComplete, comic.SeriesComplete);
      EditControlUtility.SetText((Control) this.txTags, comic.Tags);
      EditControlUtility.SetText((Control) this.cbManga, comic.Manga);
      EditControlUtility.SetText((Control) this.cbBlackAndWhite, comic.BlackAndWhite);
      EditControlUtility.SetText((Control) this.cbEnableProposed, comic.EnableProposed ? YesNo.Yes : YesNo.No);
      this.cbEnableDynamicUpdate.Visible = this.labelEnableDynamicUpdate.Visible = comic.IsDynamicSource;
      EditControlUtility.SetText((Control) this.cbEnableDynamicUpdate, comic.EnableDynamicUpdate ? YesNo.Yes : YesNo.No);
      EditControlUtility.SetText((Control) this.txAlternateSeries, comic.AlternateSeries);
      this.txAlternateNumber.Text = comic.AlternateNumber;
      EditControlUtility.SetText((Control) this.txAlternateCount, comic.AlternateCount);
      EditControlUtility.SetText((Control) this.txStoryArc, comic.StoryArc);
      EditControlUtility.SetText((Control) this.txSeriesGroup, comic.SeriesGroup);
      EditControlUtility.SetText((Control) this.txPenciller, comic.Penciller);
      EditControlUtility.SetText((Control) this.txColorist, comic.Colorist);
      EditControlUtility.SetText((Control) this.txInker, comic.Inker);
      EditControlUtility.SetText((Control) this.txLetterer, comic.Letterer);
      EditControlUtility.SetText((Control) this.txCoverArtist, comic.CoverArtist);
      EditControlUtility.SetText((Control) this.txEditor, comic.Editor);
      EditControlUtility.SetText((Control) this.cbFormat, comic.Format);
      EditControlUtility.SetText((Control) this.cbAgeRating, comic.AgeRating);
      EditControlUtility.SetText((Control) this.cbPublisher, comic.Publisher);
      EditControlUtility.SetText((Control) this.cbImprint, comic.Imprint);
      EditControlUtility.SetText((Control) this.cbBookAge, comic.BookAge);
      EditControlUtility.SetText((Control) this.cbBookStore, comic.BookStore);
      EditControlUtility.SetText((Control) this.cbBookOwner, comic.BookOwner);
      EditControlUtility.SetText((Control) this.cbBookCondition, comic.BookCondition);
      EditControlUtility.SetText((Control) this.cbBookPrice, comic.BookPriceAsText);
      EditControlUtility.SetText((Control) this.cbBookLocation, comic.BookLocation);
      EditControlUtility.SetText((Control) this.txCollectionStatus, comic.BookCollectionStatus);
      EditControlUtility.SetText((Control) this.txBookNotes, comic.BookNotes);
      EditControlUtility.SetText((Control) this.txISBN, comic.ISBN);
      EditControlUtility.SetText((Control) this.txPagesAsTextSimple, comic.PagesAsTextSimple);
      this.dtpAddedTime.Value = comic.AddedTime;
      this.dtpReleasedTime.Value = comic.ReleasedTime;
      this.dtpOpenedTime.Value = comic.OpenedTime;
      this.cbLanguage.TopTwoLetterISOLanguages = Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.LanguageISO)).Cast<string>().Distinct<string>();
      this.cbLanguage.SelectedTwoLetterISOLanguage = comic.LanguageISO;
      this.customValuesData.Rows.Clear();
      foreach (string str in (IEnumerable<string>) Program.Database.CustomValues.OrderBy<string, string>((Func<string, string>) (s => s)))
        this.customValuesData.Rows[this.customValuesData.Rows.Add((object) str, (object) (comic.GetCustomValue(str) ?? string.Empty))].Visible = Program.ExtendedSettings.ShowCustomScriptValues || !str.Contains<char>('.');
      EditControlUtility.SetText((Control) this.txSummary, StringUtility.MakeEditBoxMultiline(comic.Summary));
      EditControlUtility.SetText((Control) this.txNotes, StringUtility.MakeEditBoxMultiline(comic.Notes));
      EditControlUtility.SetText((Control) this.txReview, StringUtility.MakeEditBoxMultiline(comic.Review));
      EditControlUtility.SetText((Control) this.txCharacters, comic.Characters);
      EditControlUtility.SetText((Control) this.txTeams, comic.Teams);
      EditControlUtility.SetText((Control) this.txMainCharacterOrTeam, comic.MainCharacterOrTeam);
      EditControlUtility.SetText((Control) this.txLocations, comic.Locations);
      EditControlUtility.SetText((Control) this.txScanInformation, comic.ScanInformation);
      EditControlUtility.SetText((Control) this.txWeblink, comic.Web);
      TrackBarLite tbSaturation = this.tbSaturation;
      BitmapAdjustment colorAdjustment = comic.ColorAdjustment;
      int num1 = (int) ((double) colorAdjustment.Saturation * 100.0);
      tbSaturation.Value = num1;
      TrackBarLite tbBrightness = this.tbBrightness;
      colorAdjustment = comic.ColorAdjustment;
      int num2 = (int) ((double) colorAdjustment.Brightness * 100.0);
      tbBrightness.Value = num2;
      TrackBarLite tbContrast = this.tbContrast;
      colorAdjustment = comic.ColorAdjustment;
      int num3 = (int) ((double) colorAdjustment.Contrast * 100.0);
      tbContrast.Value = num3;
      TrackBarLite tbGamma = this.tbGamma;
      colorAdjustment = comic.ColorAdjustment;
      int num4 = (int) ((double) colorAdjustment.Gamma * 100.0);
      tbGamma.Value = num4;
      TrackBarLite tbSharpening = this.tbSharpening;
      colorAdjustment = comic.ColorAdjustment;
      int sharpen = colorAdjustment.Sharpen;
      tbSharpening.Value = sharpen;
      BitmapViewer pageViewer = this.pageViewer;
      colorAdjustment = comic.ColorAdjustment;
      Color whitePointColor = colorAdjustment.WhitePointColor;
      this.SetCurrentColorAdjustment(pageViewer, whitePointColor);
      this.CustomThumbnailKey = comic.CustomThumbnailKey;
      this.SetPromptTexts(comic, comic.EnableProposed);
      this.FocusTextBox();
    }

    private void FocusTextBox()
    {
      if (this.currentTextBox == null || !this.currentTextBox.Visible)
        return;
      this.currentTextBox.Focus();
      if (this.currentTextBox is TextBox currentTextBox1)
        currentTextBox1.SelectAll();
      if (!(this.currentTextBox is ComboBox currentTextBox2))
        return;
      currentTextBox2.SelectAll();
    }

    private void SetPromptTexts(ComicBook comic, bool enabled)
    {
      if (enabled)
      {
        this.txSeries.PromptText = comic.ProposedSeries;
        this.txTitle.PromptText = comic.ProposedTitle;
        this.txNumber.PromptText = comic.ProposedNumber;
        this.txCount.PromptText = comic.ProposedCountAsText;
        this.txYear.PromptText = comic.ProposedYearAsText;
        this.txVolume.PromptText = comic.ProposedNakedVolumeAsText;
        this.cbFormat.PromptText = comic.ProposedFormat;
      }
      else
        this.txSeries.PromptText = this.txTitle.PromptText = this.txNumber.PromptText = this.txCount.PromptText = this.txYear.PromptText = this.txVolume.PromptText = this.cbFormat.PromptText = string.Empty;
    }

    private ComicBook GetFromEditor()
    {
      ComicBook comic = new ComicBook(this.current);
      this.SaveBook(comic);
      return comic;
    }

    private void SaveBook(ComicBook comic)
    {
      comic.Series = EditControlUtility.GetText((TextBox) this.txSeries, comic.Series);
      comic.Number = EditControlUtility.GetText((TextBox) this.txNumber, comic.Number);
      comic.Title = EditControlUtility.GetText((TextBox) this.txTitle, comic.Title);
      comic.AlternateSeries = EditControlUtility.GetText((TextBox) this.txAlternateSeries, comic.AlternateSeries);
      comic.AlternateNumber = EditControlUtility.GetText((TextBox) this.txAlternateNumber, comic.AlternateNumber);
      comic.StoryArc = EditControlUtility.GetText((TextBox) this.txStoryArc, comic.StoryArc);
      comic.SeriesGroup = EditControlUtility.GetText((TextBox) this.txSeriesGroup, comic.SeriesGroup);
      comic.Writer = EditControlUtility.GetText((TextBox) this.txWriter, comic.Writer);
      comic.Summary = EditControlUtility.GetText((TextBox) this.txSummary, comic.Summary);
      comic.Penciller = EditControlUtility.GetText((TextBox) this.txPenciller, comic.Penciller);
      comic.Inker = EditControlUtility.GetText((TextBox) this.txInker, comic.Inker);
      comic.Letterer = EditControlUtility.GetText((TextBox) this.txLetterer, comic.Letterer);
      comic.CoverArtist = EditControlUtility.GetText((TextBox) this.txCoverArtist, comic.CoverArtist);
      comic.Editor = EditControlUtility.GetText((TextBox) this.txEditor, comic.Editor);
      comic.Colorist = EditControlUtility.GetText((TextBox) this.txColorist, comic.Colorist);
      comic.Genre = EditControlUtility.GetText((TextBox) this.txGenre, comic.Genre);
      comic.Characters = EditControlUtility.GetText((TextBox) this.txCharacters, comic.Characters);
      comic.Teams = EditControlUtility.GetText((TextBox) this.txTeams, comic.Teams);
      comic.MainCharacterOrTeam = EditControlUtility.GetText((TextBox) this.txMainCharacterOrTeam, comic.MainCharacterOrTeam);
      comic.Locations = EditControlUtility.GetText((TextBox) this.txLocations, comic.Locations);
      comic.Notes = EditControlUtility.GetText((TextBox) this.txNotes, comic.Notes);
      comic.Review = EditControlUtility.GetText((TextBox) this.txReview, comic.Review);
      comic.ScanInformation = EditControlUtility.GetText((TextBox) this.txScanInformation, comic.ScanInformation);
      comic.Web = EditControlUtility.GetText((TextBox) this.txWeblink, comic.Web);
      comic.Tags = EditControlUtility.GetText((TextBox) this.txTags, comic.Tags);
      comic.SeriesComplete = EditControlUtility.GetYesNo(this.cbSeriesComplete.Text);
      comic.Count = EditControlUtility.GetNumber((Control) this.txCount);
      comic.Volume = EditControlUtility.GetNumber((Control) this.txVolume);
      comic.Month = EditControlUtility.GetNumber((Control) this.txMonth);
      comic.Day = EditControlUtility.GetNumber((Control) this.txDay);
      comic.Year = EditControlUtility.GetNumber((Control) this.txYear);
      comic.AlternateCount = EditControlUtility.GetNumber((Control) this.txAlternateCount);
      comic.FilePath = this.displayComic.FilePath;
      comic.CustomThumbnailKey = this.CustomThumbnailKey;
      string str1 = this.cbPublisher.Text.Trim();
      if (comic.Publisher != str1)
      {
        comic.Publisher = str1;
        EditControlUtility.SetText(this.cbPublisher, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Publisher))), true);
      }
      string str2 = this.cbImprint.Text.Trim();
      if (comic.Imprint != str2)
      {
        comic.Imprint = str2;
        EditControlUtility.SetText(this.cbImprint, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Imprint))), true);
      }
      string str3 = this.cbFormat.Text.Trim();
      if (comic.Format != str3)
      {
        comic.Format = str3;
        EditControlUtility.SetText((ComboBox) this.cbFormat, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetFormatList()), true);
      }
      string str4 = this.cbAgeRating.Text.Trim();
      if (comic.AgeRating != str4)
      {
        comic.AgeRating = str4;
        EditControlUtility.SetText(this.cbAgeRating, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetAgeRatingList()), true);
      }
      string str5 = this.cbBookAge.Text.Trim();
      if (comic.BookAge != str5)
      {
        comic.BookAge = str5;
        EditControlUtility.SetText(this.cbBookAge, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookAgeList()), true);
      }
      string str6 = this.cbBookStore.Text.Trim();
      if (comic.BookStore != str6)
      {
        comic.BookStore = str6;
        EditControlUtility.SetText((ComboBox) this.cbBookStore, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookStore))), true);
      }
      string str7 = this.cbBookOwner.Text.Trim();
      if (comic.BookOwner != str7)
      {
        comic.BookOwner = str7;
        EditControlUtility.SetText(this.cbBookOwner, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookOwner))), true);
      }
      string str8 = this.cbBookPrice.Text.Trim();
      if (comic.BookPriceAsText != str8)
      {
        comic.BookPrice = EditControlUtility.GetRealNumber((Control) this.cbBookPrice);
        EditControlUtility.SetText(this.cbBookPrice, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookPriceAsText))), true);
      }
      string str9 = this.cbBookCondition.Text.Trim();
      if (comic.BookCondition != str9)
      {
        comic.BookCondition = str9;
        EditControlUtility.SetText(this.cbBookCondition, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookConditionList()), true);
      }
      string str10 = this.cbBookLocation.Text.Trim();
      if (comic.BookLocation != str10)
      {
        comic.BookLocation = str10;
        EditControlUtility.SetText(this.cbBookLocation, (string) null, (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookLocation))), true);
      }
      comic.BookNotes = this.txBookNotes.Text.Trim();
      comic.BookCollectionStatus = EditControlUtility.GetText((TextBox) this.txCollectionStatus, comic.BookCollectionStatus);
      comic.ISBN = this.txISBN.Text.Trim();
      if (!comic.IsLinked)
      {
        comic.PagesAsTextSimple = this.txPagesAsTextSimple.Text;
        comic.OpenedTime = this.dtpOpenedTime.Value;
      }
      comic.AddedTime = this.dtpAddedTime.Value;
      comic.ReleasedTime = this.dtpReleasedTime.Value;
      comic.LanguageISO = this.cbLanguage.SelectedTwoLetterISOLanguage;
      comic.Rating = this.txRating.Rating;
      comic.CommunityRating = this.txCommunityRating.Rating;
      comic.ColorAdjustment = this.pageViewer.ColorAdjustment;
      comic.Manga = EditControlUtility.GetMangaYesNo(this.cbManga.Text);
      comic.BlackAndWhite = EditControlUtility.GetYesNo(this.cbBlackAndWhite.Text);
      comic.EnableProposed = EditControlUtility.GetYesNo(this.cbEnableProposed.Text) == YesNo.Yes;
      comic.EnableDynamicUpdate = EditControlUtility.GetYesNo(this.cbEnableDynamicUpdate.Text) == YesNo.Yes;
      if (this.pagesView.Book != null && this.pagesView.Book.IsIndexRetrievalCompleted && this.pagesView.Book.Comic.Pages.Count > 0)
      {
        comic.SetPages((IEnumerable<ComicPageInfo>) this.pagesView.Book.Comic.Pages);
        comic.PageCount = this.pagesView.Book.Count;
        comic.LastPageRead = Math.Min(comic.PageCount - 1, comic.LastPageRead);
      }
      comic.CustomValuesStore = string.Empty;
      foreach (DataGridViewRow row in (IEnumerable) this.customValuesData.Rows)
      {
        string str11 = (string) row.Cells[0].Value;
        string str12 = (string) row.Cells[1].Value;
        if (!string.IsNullOrEmpty(str11.SafeTrim()) && !string.IsNullOrEmpty(str12))
          comic.SetCustomValue(str11, str12);
      }
    }

    private void SaveBook() => this.SaveBook(this.current);

    private void PagesViewItemActivate(object sender, EventArgs e)
    {
      this.tabControl.SelectedTab = this.tabColors;
    }

    private void PagesViewSelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.current == null)
        return;
      IEnumerable<ComicPageInfo> selectedPages = this.pagesView.GetSelectedPages();
      this.SetPageView(!selectedPages.IsEmpty<ComicPageInfo>() ? this.current.TranslateImageIndexToPage(selectedPages.First<ComicPageInfo>().ImageIndex) : this.current.FrontCoverPageIndex);
    }

    private void pageViewer_VisibleChanged(object sender, EventArgs e)
    {
      if (!this.pageViewer.Visible || this.pendingPageViewerPage == -1)
        return;
      this.SetImage((IBitmapDisplayControl) this.pageViewer, this.displayComic, this.pendingPageViewerPage);
      this.pendingPageViewerPage = -1;
    }

    private void SetPageView(int page)
    {
      this.pageViewPage = page;
      this.labelCurrentPage.Text = TR.Default["Page"] + " " + (object) (page + 1);
      if (this.pageViewer.Visible)
        this.SetImage((IBitmapDisplayControl) this.pageViewer, this.displayComic, page);
      else
        this.pendingPageViewerPage = page;
    }

    private static void SetCoverThumbnailImage(IBitmapDisplayControl iv, ComicBook cb)
    {
      ComicBookDialog.SetThumbnailImage(iv, cb, cb.FrontCoverPageIndex);
    }

    private static void SetThumbnailImage(IBitmapDisplayControl iv, ComicBook cb, int page)
    {
      try
      {
        ThumbnailKey key = cb.GetThumbnailKey(page);
        iv.Tag = (object) key;
        using (IItemLock<ThumbnailImage> thumbnail1 = Program.ImagePool.GetThumbnail(key, true))
        {
          if (thumbnail1 != null)
            iv.SetBitmap(thumbnail1.Item.Bitmap.CreateCopy());
          else
            Program.ImagePool.SlowThumbnailQueue.AddItem((ImageKey) key, (object) iv, (AsyncCallback) (ar =>
            {
              try
              {
                using (IItemLock<ThumbnailImage> thumbnail2 = Program.ImagePool.GetThumbnail(key, cb))
                {
                  if (!object.Equals((object) key, iv.Tag))
                    return;
                  iv.SetBitmap(thumbnail2.Item.Bitmap.CreateCopy());
                }
              }
              catch
              {
              }
            }));
        }
      }
      catch
      {
      }
    }

    private void SetImage(IBitmapDisplayControl iv, ComicBook cb, int page)
    {
      try
      {
        PageKey key = cb.GetPageKey(page, BitmapAdjustment.Empty);
        iv.Tag = (object) key;
        using (IItemLock<PageImage> page1 = Program.ImagePool.GetPage(key, true))
        {
          if (page1 != null && page1.Item != null)
            iv.SetBitmap(page1.Item.Bitmap.Scale(this.pageViewer.Width, 0));
          else
            Program.ImagePool.AddPageToQueue(key, (object) iv, (AsyncCallback) (ar =>
            {
              try
              {
                using (IItemLock<PageImage> page2 = Program.ImagePool.GetPage(key, cb))
                {
                  if (!object.Equals((object) key, iv.Tag))
                    return;
                  iv.SetBitmap(page2.Item.Bitmap.Scale(this.pageViewer.Width, 0));
                }
              }
              catch
              {
              }
            }), false);
        }
      }
      catch
      {
      }
    }

    private void SetCurrentColorAdjustment(BitmapViewer iv, Color whitePoint)
    {
      iv.ColorAdjustment = new BitmapAdjustment((float) this.tbSaturation.Value / 100f, (float) this.tbBrightness.Value / 100f, (float) this.tbContrast.Value / 100f, (float) this.tbGamma.Value / 100f, whitePoint, sharpen: this.tbSharpening.Value);
    }

    private void SetCurrentColorAdjustment(BitmapViewer iv)
    {
      this.SetCurrentColorAdjustment(iv, iv.ColorAdjustment.WhitePointColor);
    }

    public string CustomThumbnailKey
    {
      get => this.customThumbnailKey;
      set
      {
        if (this.displayComic.IsLinked || value == this.customThumbnailKey)
          return;
        ComicBook cb = new ComicBook(this.displayComic);
        this.customThumbnailKey = cb.CustomThumbnailKey = value;
        ComicBookDialog.SetCoverThumbnailImage((IBitmapDisplayControl) this.coverThumbnail, cb);
      }
    }

    private void customValuesData_EditingControlShowing(
      object sender,
      DataGridViewEditingControlShowingEventArgs e)
    {
      if (this.customValuesData.CurrentCell.ColumnIndex != 1)
        return;
      if (!(e.Control is TextBox control))
        return;
      AutoCompleteStringCollection stringCollection = new AutoCompleteStringCollection();
      string key = (string) this.customValuesData.Rows[this.customValuesData.CurrentRow.Index].Cells[0].Value;
      if (!string.IsNullOrEmpty(key))
        stringCollection.AddRange(Program.Database.GetBooks().SelectMany<ComicBook, StringPair>((Func<ComicBook, IEnumerable<StringPair>>) (cb => cb.GetCustomValues())).Where<StringPair>((Func<StringPair, bool>) (p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase))).Select<StringPair, string>((Func<StringPair, string>) (p => p.Value)).ToArray<string>());
      control.AutoCompleteCustomSource = stringCollection;
      control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      control.AutoCompleteSource = AutoCompleteSource.CustomSource;
    }

    private void coverThumbnail_MouseWheel(object sender, MouseEventArgs e)
    {
      if (e.Delta > 0)
        this.SelectPreviousBook();
      else
        this.SelectNextBook();
    }

    private void txCharacters_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar != '\n')
        return;
      e.Handled = true;
    }

    private void IdleProcess_Idle(object sender, EventArgs e)
    {
      this.btFirstPage.Enabled = this.btPrevPage.Enabled = this.displayComic != null && this.displayComic.PageCount > 0 && this.pageViewPage > 0;
      this.btLastPage.Enabled = this.btNextPage.Enabled = this.displayComic != null && this.displayComic.PageCount > 0 && this.pageViewPage < this.displayComic.PageCount - 1;
    }

    private void chkShowColorControls_CheckedChanged(object sender, EventArgs e)
    {
      this.panelImageControls.Visible = this.chkShowImageControls.Checked;
    }

    private void btApply_Click(object sender, EventArgs e)
    {
      this.SaveBook();
      this.SetCurrentBook(this.current);
    }

    private void btPrev_Click(object sender, EventArgs e)
    {
      this.btPrev.Focus();
      this.SelectPreviousBook();
      this.FocusTextBox();
    }

    private void btNext_Click(object sender, EventArgs e)
    {
      this.btNext.Focus();
      this.SelectNextBook();
      this.FocusTextBox();
    }

    private void btOK_Click(object sender, EventArgs e) => this.SaveBook();

    private void ColorAdjustment_Scroll(object sender, EventArgs e)
    {
      this.SetCurrentColorAdjustment(this.pageViewer);
    }

    private void btReset_Click(object sender, EventArgs e)
    {
      this.tbSaturation.Value = this.tbBrightness.Value = this.tbContrast.Value = this.tbGamma.Value = 0;
      this.tbSharpening.Value = 0;
      this.SetCurrentColorAdjustment(this.pageViewer, Color.White);
    }

    private void pageViewer_DoubleClick(object sender, EventArgs e)
    {
      try
      {
        this.SetCurrentColorAdjustment(this.pageViewer, this.pageViewer.GetPixel(this.pageViewer.PointToClient(Cursor.Position)));
      }
      catch
      {
      }
    }

    private void btResetPages_Click(object sender, EventArgs e)
    {
      if (this.pagesView.Book == null)
        return;
      this.pagesView.Book.Comic.ResetPageSequence();
      this.pagesView.UpdateList(true);
    }

    private void btResetPages_ShowContextMenu(object sender, EventArgs e)
    {
      this.miOrderByName.Enabled = this.miOrderByNameNumeric.Enabled = this.pagesView.Book != null && this.pagesView.Book.IsIndexRetrievalCompleted;
    }

    private void cmResetPages_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
      if (this.pagesView.Book == null)
        return;
      Comparison<ComicPageInfo> comparison = (Comparison<ComicPageInfo>) null;
      Dictionary<int, PageViewItem> map = this.pagesView.GetItems().ToDictionary<PageViewItem, int>((Func<PageViewItem, int>) (item => item.PageInfo.ImageIndex));
      if (e.ClickedItem == this.miOrderByName)
        comparison = (Comparison<ComicPageInfo>) ((a, b) => string.CompareOrdinal(map[a.ImageIndex].Key, map[b.ImageIndex].Key));
      else if (e.ClickedItem == this.miOrderByNameNumeric)
        comparison = (Comparison<ComicPageInfo>) ((a, b) => ExtendedStringComparer.Compare(map[a.ImageIndex].Key, map[b.ImageIndex].Key));
      if (comparison == null)
        return;
      try
      {
        this.pagesView.Book.Comic.SortPages(comparison);
      }
      catch
      {
        this.pagesView.Book.Comic.ResetPageSequence();
      }
      this.pagesView.UpdateList(true);
    }

    private void cbEnableShadowValues_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SetPromptTexts(this.current, EditControlUtility.GetYesNo(this.cbEnableProposed.Text) == YesNo.Yes);
    }

    private void btPageViews_Click(object sender, EventArgs e)
    {
      this.pagesView.ItemView.GetViewMenu().Show((Control) this.btPageView, new System.Drawing.Point(this.btPageView.Width, this.btPageView.Height), ToolStripDropDownDirection.BelowLeft);
    }

    private void AdjustmentSliderChanged(object sender, EventArgs e)
    {
      TrackBarLite trackBarLite = (TrackBarLite) sender;
      this.toolTip.SetToolTip((Control) trackBarLite, string.Format("{1}{0}%", (object) trackBarLite.Value, trackBarLite.Value > 0 ? (object) "+" : (object) string.Empty));
    }

    private void coverThumbnail_Click(object sender, EventArgs e)
    {
      this.coverThumbnail.ThreeD = !this.coverThumbnail.ThreeD;
    }

    private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Program.StartDocument(this.linkLabel.Text);
    }

    private void btLinkFile_Click(object sender, EventArgs e)
    {
      string file = Program.ShowComicOpenDialog((IWin32Window) this, this.btLinkFile.Text.Replace("&", string.Empty), false);
      if (file == null)
        return;
      this.LinkFile(file);
    }

    private void LinkFile(string file)
    {
      if (this.current.IsLinked)
        return;
      if (Program.Database.Books[file] != null)
      {
        int num = (int) MessageBox.Show((IWin32Window) this, TR.Messages["ErrorFileUsed", "This file is already used in the Library"], TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else
      {
        ComicBook comic = Program.BookFactory.Create(file, CreateBookOption.DoNotAdd, RefreshInfoOptions.ForceRefresh);
        if (comic == null)
          return;
        comic.SetInfo(this.current.GetInfo(), true, true);
        this.SetComicToEditor(comic);
      }
    }

    private void ComicBookDialog_DragDrop(object sender, DragEventArgs e)
    {
      if (!(e.Data.GetData(DataFormats.FileDrop) is string[] data) || data.Length == 0)
        return;
      this.LinkFile(data[0]);
    }

    private void ComicBookDialog_DragOver(object sender, DragEventArgs e)
    {
      e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void coverThumbnail_DragDrop(object sender, DragEventArgs e)
    {
      try
      {
        Bitmap data1 = e.Data.GetData(DataFormats.Bitmap) as Bitmap;
        string[] data2 = e.Data.GetData(DataFormats.FileDrop) as string[];
        if (data1 != null)
        {
          this.CustomThumbnailKey = Program.ImagePool.AddCustomThumbnail(data1);
        }
        else
        {
          if (data2 == null)
            return;
          this.LoadThumbnail(data2[0]);
        }
      }
      catch (Exception ex)
      {
      }
    }

    private void coverThumbnail_DragOver(object sender, DragEventArgs e)
    {
      bool flag = !this.displayComic.IsLinked && (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Bitmap));
      e.Effect = flag ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void btThumbnail_Click(object sender, EventArgs e)
    {
      string str = Program.LoadCustomThumbnail((string) null, (IWin32Window) this, this.btThumbnail.Text.Replace("&", string.Empty));
      if (string.IsNullOrEmpty(str))
        return;
      this.CustomThumbnailKey = str;
    }

    private void LoadThumbnail(string file)
    {
      string str = Program.LoadCustomThumbnail(file);
      if (string.IsNullOrEmpty(str))
        return;
      this.CustomThumbnailKey = str;
    }

    private void btThumbnail_ShowContextMenu(object sender, EventArgs e)
    {
      FormUtility.SafeToolStripClear(this.cmThumbnail.Items, 2);
      foreach (string thumbnailFile in (SmartList<string>) Program.Settings.ThumbnailFiles)
      {
        string file = thumbnailFile;
        this.cmThumbnail.Items.Add(thumbnailFile, (Image) null, (EventHandler) ((s, ea) => this.LoadThumbnail(file)));
      }
      this.cmThumbnail.Items[1].Visible = this.cmThumbnail.Items.Count > 2;
    }

    private void miResetThumbnail_Click(object sender, EventArgs e)
    {
      this.CustomThumbnailKey = (string) null;
    }

    private void btFirstPage_Click(object sender, EventArgs e)
    {
      if (this.displayComic.PageCount <= 0)
        return;
      this.SetPageView(0);
    }

    private void btPrevPage_Click(object sender, EventArgs e)
    {
      int page = this.pageViewPage - 1;
      if (this.displayComic.PageCount < 0 || page < 0)
        return;
      this.SetPageView(page);
    }

    private void btNextPage_Click(object sender, EventArgs e)
    {
      int page = this.pageViewPage + 1;
      if (this.displayComic.PageCount < 0 || page >= this.displayComic.PageCount)
        return;
      this.SetPageView(page);
    }

    private void btLastPage_Click(object sender, EventArgs e)
    {
      if (this.displayComic.PageCount <= 0)
        return;
      this.SetPageView(this.current.PageCount - 1);
    }

    private void IconTextsChanged(object sender, EventArgs e)
    {
      this.coverThumbnail.FormatIcon = this.cbFormat.Text;
      this.coverThumbnail.AgeRatingIcon = this.cbAgeRating.Text;
      this.coverThumbnail.PublisherIcon = this.cbPublisher.Text;
      this.coverThumbnail.ImprintIcon = this.cbImprint.Text;
      this.coverThumbnail.PublishedYear = this.txYear.Text;
      this.coverThumbnail.BlackAndWhiteIcon = EditControlUtility.GetYesNo(this.cbBlackAndWhite.Text);
      this.coverThumbnail.MangaIcon = EditControlUtility.GetMangaYesNo(this.cbManga.Text);
      this.coverThumbnail.SeriesCompleteIcon = EditControlUtility.GetYesNo(this.cbSeriesComplete.Text);
    }

    private void SelectPreviousBook()
    {
      int index = Array.IndexOf<ComicBook>(this.allBooks, this.current) - 1;
      if (index < 0)
        return;
      this.SaveBook();
      this.SetCurrentBook(this.allBooks[index]);
    }

    private void SelectNextBook()
    {
      int index = Array.IndexOf<ComicBook>(this.allBooks, this.current) + 1;
      if (index >= this.allBooks.Length)
        return;
      this.SaveBook();
      this.SetCurrentBook(this.allBooks[index]);
    }

    private void InitializeScriptButton()
    {
      if (ComicBookDialog.scriptEngine == null)
        return;
      foreach (Command command in ComicBookDialog.scriptEngine.GetCommands("Editor"))
      {
        if (this.btScript.ContextMenuStrip == null)
          this.btScript.ContextMenuStrip = new ContextMenuStrip();
        ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem(command.GetLocalizedName());
        toolStripMenuItem1.Tag = (object) command;
        ToolStripMenuItem toolStripMenuItem2 = toolStripMenuItem1;
        toolStripMenuItem2.Click += new EventHandler(this.DropDownMenuItemClick);
        this.btScript.ContextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem2);
      }
      if (this.btScript.ContextMenuStrip == null)
        return;
      ComicBookDialog.currentScript = ComicBookDialog.currentScript.Clamp(0, this.btScript.ContextMenuStrip.Items.Count - 1);
      this.btScript.Visible = true;
      this.btScript.Text = this.btScript.ContextMenuStrip.Items[ComicBookDialog.currentScript].Text;
      this.btScript.Tag = this.btScript.ContextMenuStrip.Items[ComicBookDialog.currentScript].Tag;
      this.btScript.Click += new EventHandler(this.ScriptButtonClick);
    }

    private void DropDownMenuItemClick(object sender, EventArgs e)
    {
      ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem) sender;
      ComicBookDialog.currentScript = this.btScript.ContextMenuStrip.Items.IndexOf((ToolStripItem) toolStripMenuItem);
      this.ExecuteScript(toolStripMenuItem.Text, toolStripMenuItem.Tag as Command);
    }

    private void ScriptButtonClick(object sender, EventArgs e)
    {
      Control control = (Control) sender;
      this.ExecuteScript(control.Text, control.Tag as Command);
    }

    private void ExecuteScript(string name, Command cmd)
    {
      try
      {
        this.btScript.Text = name;
        this.btScript.Tag = (object) cmd;
        ComicBook fromEditor = this.GetFromEditor();
        using (new WaitCursor((Form) this))
          cmd.Invoke(new object[1]
          {
            (object) new ComicBook[1]{ fromEditor }
          });
        this.SetDataToEditor(fromEditor);
      }
      catch (Exception ex)
      {
        ScriptUtility.ShowError((IWin32Window) this, ex);
      }
    }

    public static bool Show(
      IWin32Window parent,
      ComicBook comicBook,
      ComicBook[] books,
      Func<ComicBook, bool> selHandler)
    {
      using (ComicBookDialog comicBookDialog = new ComicBookDialog(comicBook, books))
      {
        comicBookDialog.selectComicHandler = selHandler;
        if (ComicBookDialog.lastActivePage != -1)
          comicBookDialog.tabControl.SelectedIndex = ComicBookDialog.lastActivePage;
        if (ComicBookDialog.lastActiveTextPage != -1)
          comicBookDialog.tabNotes.SelectedIndex = ComicBookDialog.lastActiveTextPage;
        comicBookDialog.chkShowImageControls.Checked = ComicBookDialog.showColorControls;
        bool flag = comicBookDialog.ShowDialog(parent) == DialogResult.OK;
        ComicBookDialog.lastActivePage = comicBookDialog.tabControl.SelectedIndex;
        ComicBookDialog.lastActiveTextPage = comicBookDialog.tabNotes.SelectedIndex;
        ComicBookDialog.showColorControls = comicBookDialog.chkShowImageControls.Checked;
        return flag;
      }
    }

    public static bool Show(IWin32Window parent, ComicBook comicBook, ComicBook[] books)
    {
      return ComicBookDialog.Show(parent, comicBook, books, (Func<ComicBook, bool>) null);
    }

    public static bool Show(IWin32Window parent, ComicBook comicBook)
    {
      return ComicBookDialog.Show(parent, comicBook, (ComicBook[]) null);
    }

    public static ItemViewConfig PagesConfig
    {
      get => ComicBookDialog.pagesConfig;
      set => ComicBookDialog.pagesConfig = value;
    }

    public static PluginEngine ScriptEngine
    {
      get => ComicBookDialog.scriptEngine;
      set => ComicBookDialog.scriptEngine = value;
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.btCancel = new Button();
      this.btOK = new Button();
      this.tabControl = new TabControl();
      this.tabSummary = new TabPage();
      this.btThumbnail = new SplitButton();
      this.cmThumbnail = new ContextMenuStrip(this.components);
      this.miResetThumbnail = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.labelCommunityRating = new Label();
      this.txCommunityRating = new RatingControl();
      this.labelMyRating = new Label();
      this.btLinkFile = new Button();
      this.linkLabel = new LinkLabel();
      this.coverThumbnail = new ThumbnailControl();
      this.whereSeparator = new Panel();
      this.panel1 = new Panel();
      this.labelWhere = new Label();
      this.labelType = new Label();
      this.txRating = new RatingControl();
      this.labelPages = new Label();
      this.lblType = new Label();
      this.lblPath = new Label();
      this.lblPages = new Label();
      this.tabDetails = new TabPage();
      this.txDay = new TextBoxEx();
      this.labelDay = new Label();
      this.labelSeriesGroup = new Label();
      this.txSeriesGroup = new TextBoxEx();
      this.labelStoryArc = new Label();
      this.txStoryArc = new TextBoxEx();
      this.cbSeriesComplete = new ComboBox();
      this.labelSeriesComplete = new Label();
      this.cbEnableDynamicUpdate = new ComboBox();
      this.labelEnableDynamicUpdate = new Label();
      this.txGenre = new TextBoxEx();
      this.labelTags = new Label();
      this.cbEnableProposed = new ComboBox();
      this.txTags = new TextBoxEx();
      this.labelEnableProposed = new Label();
      this.txVolume = new TextBoxEx();
      this.cbAgeRating = new ComboBox();
      this.txEditor = new TextBoxEx();
      this.labelEditor = new Label();
      this.txMonth = new TextBoxEx();
      this.txYear = new TextBoxEx();
      this.labelAgeRating = new Label();
      this.cbFormat = new ComboBoxEx();
      this.txColorist = new TextBoxEx();
      this.txSeries = new TextBoxEx();
      this.labelFormat = new Label();
      this.labelAlternateSeries = new Label();
      this.txAlternateSeries = new TextBoxEx();
      this.cbImprint = new ComboBox();
      this.cbBlackAndWhite = new ComboBox();
      this.labelVolume = new Label();
      this.txInker = new TextBoxEx();
      this.cbManga = new ComboBox();
      this.labelYear = new Label();
      this.labelMonth = new Label();
      this.labelBlackAndWhite = new Label();
      this.txAlternateCount = new TextBoxEx();
      this.labelManga = new Label();
      this.labelSeries = new Label();
      this.labelLanguage = new Label();
      this.labelImprint = new Label();
      this.labelGenre = new Label();
      this.labelColorist = new Label();
      this.txCount = new TextBoxEx();
      this.cbLanguage = new LanguageComboBox();
      this.cbPublisher = new ComboBox();
      this.txPenciller = new TextBoxEx();
      this.txAlternateNumber = new TextBoxEx();
      this.txNumber = new TextBoxEx();
      this.labelPublisher = new Label();
      this.labelCoverArtist = new Label();
      this.txCoverArtist = new TextBoxEx();
      this.labelInker = new Label();
      this.labelAlternateCount = new Label();
      this.txTitle = new TextBoxEx();
      this.labelCount = new Label();
      this.labelAlternateNumber = new Label();
      this.labelNumber = new Label();
      this.labelLetterer = new Label();
      this.labelPenciller = new Label();
      this.txLetterer = new TextBoxEx();
      this.labelTitle = new Label();
      this.labelWriter = new Label();
      this.txWriter = new TextBoxEx();
      this.tabPlot = new TabPage();
      this.tabNotes = new TabControl();
      this.tabPageSummary = new TabPage();
      this.txSummary = new TextBoxEx();
      this.tabPageNotes = new TabPage();
      this.txNotes = new TextBoxEx();
      this.tabPageReview = new TabPage();
      this.txReview = new TextBoxEx();
      this.txMainCharacterOrTeam = new TextBoxEx();
      this.labelMainCharacterOrTeam = new Label();
      this.txScanInformation = new TextBoxEx();
      this.labelScanInformation = new Label();
      this.txLocations = new TextBoxEx();
      this.labelLocations = new Label();
      this.txTeams = new TextBoxEx();
      this.labelTeams = new Label();
      this.txWeblink = new TextBoxEx();
      this.labelWeb = new Label();
      this.txCharacters = new TextBoxEx();
      this.labelCharacters = new Label();
      this.tabCatalog = new TabPage();
      this.labelReleasedTime = new Label();
      this.dtpReleasedTime = new NullableDateTimePicker();
      this.labelOpenedTime = new Label();
      this.dtpOpenedTime = new NullableDateTimePicker();
      this.labelAddedTime = new Label();
      this.dtpAddedTime = new NullableDateTimePicker();
      this.txPagesAsTextSimple = new TextBoxEx();
      this.labelPagesAsTextSimple = new Label();
      this.txISBN = new TextBoxEx();
      this.labelISBN = new Label();
      this.cbBookLocation = new ComboBox();
      this.labelBookLocation = new Label();
      this.txCollectionStatus = new TextBoxEx();
      this.cbBookPrice = new ComboBox();
      this.labelBookPrice = new Label();
      this.txBookNotes = new TextBoxEx();
      this.labelBookNotes = new Label();
      this.cbBookAge = new ComboBox();
      this.labelBookAge = new Label();
      this.labelBookCollectionStatus = new Label();
      this.cbBookCondition = new ComboBox();
      this.labelBookCondition = new Label();
      this.cbBookStore = new ComboBoxEx();
      this.labelBookStore = new Label();
      this.cbBookOwner = new ComboBox();
      this.labelBookOwner = new Label();
      this.tabCustom = new TabPage();
      this.customValuesData = new DataGridView();
      this.CustomValueName = new DataGridViewTextBoxColumn();
      this.CustomValueValue = new DataGridViewTextBoxColumn();
      this.tabPages = new TabPage();
      this.btResetPages = new SplitButton();
      this.cmResetPages = new ContextMenuStrip(this.components);
      this.miOrderByName = new ToolStripMenuItem();
      this.miOrderByNameNumeric = new ToolStripMenuItem();
      this.btPageView = new Button();
      this.labelPagesInfo = new Label();
      this.pagesView = new PagesView();
      this.tabColors = new TabPage();
      this.panelImage = new Panel();
      this.labelCurrentPage = new Label();
      this.chkShowImageControls = new CheckBox();
      this.btLastPage = new Button();
      this.btFirstPage = new Button();
      this.btNextPage = new AutoRepeatButton();
      this.btPrevPage = new AutoRepeatButton();
      this.pageViewer = new BitmapViewer();
      this.panelImageControls = new Panel();
      this.labelSaturation = new Label();
      this.labelContrast = new Label();
      this.tbGamma = new TrackBarLite();
      this.tbSaturation = new TrackBarLite();
      this.labelGamma = new Label();
      this.tbBrightness = new TrackBarLite();
      this.tbSharpening = new TrackBarLite();
      this.tbContrast = new TrackBarLite();
      this.labelSharpening = new Label();
      this.labelBrightness = new Label();
      this.btResetColors = new Button();
      this.btPrev = new Button();
      this.btNext = new Button();
      this.btScript = new SplitButton();
      this.toolTip = new ToolTip(this.components);
      this.btApply = new Button();
      this.tabControl.SuspendLayout();
      this.tabSummary.SuspendLayout();
      this.cmThumbnail.SuspendLayout();
      this.tabDetails.SuspendLayout();
      this.tabPlot.SuspendLayout();
      this.tabNotes.SuspendLayout();
      this.tabPageSummary.SuspendLayout();
      this.tabPageNotes.SuspendLayout();
      this.tabPageReview.SuspendLayout();
      this.tabCatalog.SuspendLayout();
      this.tabCustom.SuspendLayout();
      ((ISupportInitialize) this.customValuesData).BeginInit();
      this.tabPages.SuspendLayout();
      this.cmResetPages.SuspendLayout();
      this.tabColors.SuspendLayout();
      this.panelImage.SuspendLayout();
      this.panelImageControls.SuspendLayout();
      this.SuspendLayout();
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(415, 483);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 5;
      this.btCancel.Text = "&Cancel";
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(329, 483);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 4;
      this.btOK.Text = "&OK";
      this.btOK.Click += new EventHandler(this.btOK_Click);
      this.tabControl.Controls.Add((Control) this.tabSummary);
      this.tabControl.Controls.Add((Control) this.tabDetails);
      this.tabControl.Controls.Add((Control) this.tabPlot);
      this.tabControl.Controls.Add((Control) this.tabCatalog);
      this.tabControl.Controls.Add((Control) this.tabCustom);
      this.tabControl.Controls.Add((Control) this.tabPages);
      this.tabControl.Controls.Add((Control) this.tabColors);
      this.tabControl.Location = new System.Drawing.Point(8, 9);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(574, 468);
      this.tabControl.TabIndex = 0;
      this.tabSummary.Controls.Add((Control) this.btThumbnail);
      this.tabSummary.Controls.Add((Control) this.labelCommunityRating);
      this.tabSummary.Controls.Add((Control) this.txCommunityRating);
      this.tabSummary.Controls.Add((Control) this.labelMyRating);
      this.tabSummary.Controls.Add((Control) this.btLinkFile);
      this.tabSummary.Controls.Add((Control) this.linkLabel);
      this.tabSummary.Controls.Add((Control) this.coverThumbnail);
      this.tabSummary.Controls.Add((Control) this.whereSeparator);
      this.tabSummary.Controls.Add((Control) this.panel1);
      this.tabSummary.Controls.Add((Control) this.labelWhere);
      this.tabSummary.Controls.Add((Control) this.labelType);
      this.tabSummary.Controls.Add((Control) this.txRating);
      this.tabSummary.Controls.Add((Control) this.labelPages);
      this.tabSummary.Controls.Add((Control) this.lblType);
      this.tabSummary.Controls.Add((Control) this.lblPath);
      this.tabSummary.Controls.Add((Control) this.lblPages);
      this.tabSummary.Location = new System.Drawing.Point(4, 22);
      this.tabSummary.Name = "tabSummary";
      this.tabSummary.Padding = new Padding(3);
      this.tabSummary.Size = new System.Drawing.Size(566, 442);
      this.tabSummary.TabIndex = 0;
      this.tabSummary.Text = "Summary";
      this.tabSummary.UseVisualStyleBackColor = true;
      this.btThumbnail.ContextMenuStrip = this.cmThumbnail;
      this.btThumbnail.Location = new System.Drawing.Point(372, 347);
      this.btThumbnail.Name = "btThumbnail";
      this.btThumbnail.Size = new System.Drawing.Size(152, 23);
      this.btThumbnail.TabIndex = 13;
      this.btThumbnail.Text = "Thumbnail...";
      this.btThumbnail.UseVisualStyleBackColor = true;
      this.btThumbnail.Visible = false;
      this.btThumbnail.ShowContextMenu += new EventHandler(this.btThumbnail_ShowContextMenu);
      this.btThumbnail.Click += new EventHandler(this.btThumbnail_Click);
      this.cmThumbnail.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.miResetThumbnail,
        (ToolStripItem) this.toolStripMenuItem1
      });
      this.cmThumbnail.Name = "cmKeyboardLayout";
      this.cmThumbnail.Size = new System.Drawing.Size(103, 32);
      this.miResetThumbnail.Name = "miResetThumbnail";
      this.miResetThumbnail.Size = new System.Drawing.Size(102, 22);
      this.miResetThumbnail.Text = "&Reset";
      this.miResetThumbnail.Click += new EventHandler(this.miResetThumbnail_Click);
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(99, 6);
      this.labelCommunityRating.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCommunityRating.Location = new System.Drawing.Point(217, 313);
      this.labelCommunityRating.Name = "labelCommunityRating";
      this.labelCommunityRating.Size = new System.Drawing.Size(149, 20);
      this.labelCommunityRating.TabIndex = 7;
      this.labelCommunityRating.Text = "Community Rating:";
      this.labelCommunityRating.TextAlign = ContentAlignment.MiddleRight;
      this.txCommunityRating.DrawText = true;
      this.txCommunityRating.Font = new Font("Arial", 9f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.txCommunityRating.ForeColor = SystemColors.GrayText;
      this.txCommunityRating.Location = new System.Drawing.Point(372, 312);
      this.txCommunityRating.Name = "txCommunityRating";
      this.txCommunityRating.Rating = 3f;
      this.txCommunityRating.RatingImage = (Image) Resources.StarBlue;
      this.txCommunityRating.Size = new System.Drawing.Size(152, 21);
      this.txCommunityRating.TabIndex = 8;
      this.txCommunityRating.Text = "3";
      this.labelMyRating.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelMyRating.Location = new System.Drawing.Point(219, 285);
      this.labelMyRating.Name = "labelMyRating";
      this.labelMyRating.Size = new System.Drawing.Size(147, 20);
      this.labelMyRating.TabIndex = 5;
      this.labelMyRating.Text = "My Rating:";
      this.labelMyRating.TextAlign = ContentAlignment.MiddleRight;
      this.btLinkFile.Location = new System.Drawing.Point(372, 373);
      this.btLinkFile.Name = "btLinkFile";
      this.btLinkFile.Size = new System.Drawing.Size(152, 23);
      this.btLinkFile.TabIndex = 14;
      this.btLinkFile.Text = "Link to File...";
      this.btLinkFile.UseVisualStyleBackColor = true;
      this.btLinkFile.Visible = false;
      this.btLinkFile.Click += new EventHandler(this.btLinkFile_Click);
      this.linkLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.linkLabel.LinkColor = Color.SteelBlue;
      this.linkLabel.Location = new System.Drawing.Point(3, 416);
      this.linkLabel.Name = "linkLabel";
      this.linkLabel.Size = new System.Drawing.Size(560, 23);
      this.linkLabel.TabIndex = 12;
      this.linkLabel.TabStop = true;
      this.linkLabel.Text = "linkLabel";
      this.linkLabel.TextAlign = ContentAlignment.MiddleCenter;
      this.linkLabel.VisitedLinkColor = Color.MediumOrchid;
      this.linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
      this.coverThumbnail.AllowDrop = true;
      this.coverThumbnail.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.coverThumbnail.Location = new System.Drawing.Point(21, 19);
      this.coverThumbnail.Name = "coverThumbnail";
      this.coverThumbnail.Size = new System.Drawing.Size(520, 243);
      this.coverThumbnail.TabIndex = 0;
      this.coverThumbnail.ThreeD = true;
      this.coverThumbnail.Tile = true;
      this.coverThumbnail.Click += new EventHandler(this.coverThumbnail_Click);
      this.coverThumbnail.DragDrop += new DragEventHandler(this.coverThumbnail_DragDrop);
      this.coverThumbnail.DragOver += new DragEventHandler(this.coverThumbnail_DragOver);
      this.whereSeparator.BackColor = SystemColors.ButtonShadow;
      this.whereSeparator.Location = new System.Drawing.Point(19, 360);
      this.whereSeparator.Name = "whereSeparator";
      this.whereSeparator.Size = new System.Drawing.Size(520, 1);
      this.whereSeparator.TabIndex = 9;
      this.panel1.BackColor = SystemColors.ButtonShadow;
      this.panel1.Location = new System.Drawing.Point(23, 268);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(520, 1);
      this.panel1.TabIndex = 0;
      this.labelWhere.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelWhere.Location = new System.Drawing.Point(19, 373);
      this.labelWhere.Name = "labelWhere";
      this.labelWhere.Size = new System.Drawing.Size(68, 17);
      this.labelWhere.TabIndex = 10;
      this.labelWhere.Text = "Where:";
      this.labelWhere.TextAlign = ContentAlignment.TopRight;
      this.labelType.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelType.Location = new System.Drawing.Point(19, 285);
      this.labelType.Name = "labelType";
      this.labelType.Size = new System.Drawing.Size(68, 20);
      this.labelType.TabIndex = 1;
      this.labelType.Text = "Type:";
      this.labelType.TextAlign = ContentAlignment.MiddleRight;
      this.txRating.DrawText = true;
      this.txRating.Font = new Font("Arial", 9f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.txRating.ForeColor = SystemColors.GrayText;
      this.txRating.Location = new System.Drawing.Point(372, 285);
      this.txRating.Name = "txRating";
      this.txRating.Rating = 3f;
      this.txRating.RatingImage = (Image) Resources.StarYellow;
      this.txRating.Size = new System.Drawing.Size(152, 21);
      this.txRating.TabIndex = 6;
      this.txRating.Text = "3";
      this.labelPages.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelPages.Location = new System.Drawing.Point(21, 313);
      this.labelPages.Name = "labelPages";
      this.labelPages.Size = new System.Drawing.Size(66, 20);
      this.labelPages.TabIndex = 3;
      this.labelPages.Text = "Pages:";
      this.labelPages.TextAlign = ContentAlignment.MiddleRight;
      this.lblType.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblType.Location = new System.Drawing.Point(93, 285);
      this.lblType.Name = "lblType";
      this.lblType.Size = new System.Drawing.Size(145, 20);
      this.lblType.TabIndex = 2;
      this.lblType.TextAlign = ContentAlignment.MiddleLeft;
      this.lblPath.AutoEllipsis = true;
      this.lblPath.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblPath.Location = new System.Drawing.Point(93, 373);
      this.lblPath.Name = "lblPath";
      this.lblPath.Size = new System.Drawing.Size(431, 35);
      this.lblPath.TabIndex = 11;
      this.lblPath.UseMnemonic = false;
      this.lblPages.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblPages.Location = new System.Drawing.Point(93, 313);
      this.lblPages.Name = "lblPages";
      this.lblPages.Size = new System.Drawing.Size(105, 20);
      this.lblPages.TabIndex = 4;
      this.lblPages.TextAlign = ContentAlignment.MiddleLeft;
      this.tabDetails.Controls.Add((Control) this.txDay);
      this.tabDetails.Controls.Add((Control) this.labelDay);
      this.tabDetails.Controls.Add((Control) this.labelSeriesGroup);
      this.tabDetails.Controls.Add((Control) this.txSeriesGroup);
      this.tabDetails.Controls.Add((Control) this.labelStoryArc);
      this.tabDetails.Controls.Add((Control) this.txStoryArc);
      this.tabDetails.Controls.Add((Control) this.cbSeriesComplete);
      this.tabDetails.Controls.Add((Control) this.labelSeriesComplete);
      this.tabDetails.Controls.Add((Control) this.cbEnableDynamicUpdate);
      this.tabDetails.Controls.Add((Control) this.labelEnableDynamicUpdate);
      this.tabDetails.Controls.Add((Control) this.txGenre);
      this.tabDetails.Controls.Add((Control) this.labelTags);
      this.tabDetails.Controls.Add((Control) this.cbEnableProposed);
      this.tabDetails.Controls.Add((Control) this.txTags);
      this.tabDetails.Controls.Add((Control) this.labelEnableProposed);
      this.tabDetails.Controls.Add((Control) this.txVolume);
      this.tabDetails.Controls.Add((Control) this.cbAgeRating);
      this.tabDetails.Controls.Add((Control) this.txEditor);
      this.tabDetails.Controls.Add((Control) this.labelEditor);
      this.tabDetails.Controls.Add((Control) this.txMonth);
      this.tabDetails.Controls.Add((Control) this.txYear);
      this.tabDetails.Controls.Add((Control) this.labelAgeRating);
      this.tabDetails.Controls.Add((Control) this.cbFormat);
      this.tabDetails.Controls.Add((Control) this.txColorist);
      this.tabDetails.Controls.Add((Control) this.txSeries);
      this.tabDetails.Controls.Add((Control) this.labelFormat);
      this.tabDetails.Controls.Add((Control) this.labelAlternateSeries);
      this.tabDetails.Controls.Add((Control) this.txAlternateSeries);
      this.tabDetails.Controls.Add((Control) this.cbImprint);
      this.tabDetails.Controls.Add((Control) this.cbBlackAndWhite);
      this.tabDetails.Controls.Add((Control) this.labelVolume);
      this.tabDetails.Controls.Add((Control) this.txInker);
      this.tabDetails.Controls.Add((Control) this.cbManga);
      this.tabDetails.Controls.Add((Control) this.labelYear);
      this.tabDetails.Controls.Add((Control) this.labelMonth);
      this.tabDetails.Controls.Add((Control) this.labelBlackAndWhite);
      this.tabDetails.Controls.Add((Control) this.txAlternateCount);
      this.tabDetails.Controls.Add((Control) this.labelManga);
      this.tabDetails.Controls.Add((Control) this.labelSeries);
      this.tabDetails.Controls.Add((Control) this.labelLanguage);
      this.tabDetails.Controls.Add((Control) this.labelImprint);
      this.tabDetails.Controls.Add((Control) this.labelGenre);
      this.tabDetails.Controls.Add((Control) this.labelColorist);
      this.tabDetails.Controls.Add((Control) this.txCount);
      this.tabDetails.Controls.Add((Control) this.cbLanguage);
      this.tabDetails.Controls.Add((Control) this.cbPublisher);
      this.tabDetails.Controls.Add((Control) this.txPenciller);
      this.tabDetails.Controls.Add((Control) this.txAlternateNumber);
      this.tabDetails.Controls.Add((Control) this.txNumber);
      this.tabDetails.Controls.Add((Control) this.labelPublisher);
      this.tabDetails.Controls.Add((Control) this.labelCoverArtist);
      this.tabDetails.Controls.Add((Control) this.txCoverArtist);
      this.tabDetails.Controls.Add((Control) this.labelInker);
      this.tabDetails.Controls.Add((Control) this.labelAlternateCount);
      this.tabDetails.Controls.Add((Control) this.txTitle);
      this.tabDetails.Controls.Add((Control) this.labelCount);
      this.tabDetails.Controls.Add((Control) this.labelAlternateNumber);
      this.tabDetails.Controls.Add((Control) this.labelNumber);
      this.tabDetails.Controls.Add((Control) this.labelLetterer);
      this.tabDetails.Controls.Add((Control) this.labelPenciller);
      this.tabDetails.Controls.Add((Control) this.txLetterer);
      this.tabDetails.Controls.Add((Control) this.labelTitle);
      this.tabDetails.Controls.Add((Control) this.labelWriter);
      this.tabDetails.Controls.Add((Control) this.txWriter);
      this.tabDetails.Location = new System.Drawing.Point(4, 22);
      this.tabDetails.Name = "tabDetails";
      this.tabDetails.Padding = new Padding(3);
      this.tabDetails.Size = new System.Drawing.Size(566, 442);
      this.tabDetails.TabIndex = 1;
      this.tabDetails.Text = "Details";
      this.tabDetails.UseVisualStyleBackColor = true;
      this.txDay.Location = new System.Drawing.Point(349, 65);
      this.txDay.MaxLength = 4;
      this.txDay.Name = "txDay";
      this.txDay.PromptText = "";
      this.txDay.Size = new System.Drawing.Size(55, 20);
      this.txDay.TabIndex = 17;
      this.labelDay.AutoSize = true;
      this.labelDay.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelDay.Location = new System.Drawing.Point(349, 51);
      this.labelDay.Name = "labelDay";
      this.labelDay.Size = new System.Drawing.Size(29, 12);
      this.labelDay.TabIndex = 16;
      this.labelDay.Text = "Day:";
      this.labelSeriesGroup.AutoSize = true;
      this.labelSeriesGroup.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelSeriesGroup.Location = new System.Drawing.Point(214, 129);
      this.labelSeriesGroup.Name = "labelSeriesGroup";
      this.labelSeriesGroup.Size = new System.Drawing.Size(74, 12);
      this.labelSeriesGroup.TabIndex = 30;
      this.labelSeriesGroup.Text = "Series Group:";
      this.txSeriesGroup.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txSeriesGroup.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txSeriesGroup.Location = new System.Drawing.Point(216, 142);
      this.txSeriesGroup.Name = "txSeriesGroup";
      this.txSeriesGroup.PromptText = "";
      this.txSeriesGroup.Size = new System.Drawing.Size(188, 20);
      this.txSeriesGroup.TabIndex = 31;
      this.txSeriesGroup.Tag = (object) "SeriesGroup";
      this.labelStoryArc.AutoSize = true;
      this.labelStoryArc.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelStoryArc.Location = new System.Drawing.Point(8, 129);
      this.labelStoryArc.Name = "labelStoryArc";
      this.labelStoryArc.Size = new System.Drawing.Size(57, 12);
      this.labelStoryArc.TabIndex = 28;
      this.labelStoryArc.Text = "Story Arc:";
      this.txStoryArc.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txStoryArc.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txStoryArc.Location = new System.Drawing.Point(10, 142);
      this.txStoryArc.Name = "txStoryArc";
      this.txStoryArc.PromptText = "";
      this.txStoryArc.Size = new System.Drawing.Size(200, 20);
      this.txStoryArc.TabIndex = 29;
      this.txStoryArc.Tag = (object) "StoryArc";
      this.cbSeriesComplete.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbSeriesComplete.FormattingEnabled = true;
      this.cbSeriesComplete.Location = new System.Drawing.Point(416, 141);
      this.cbSeriesComplete.Name = "cbSeriesComplete";
      this.cbSeriesComplete.Size = new System.Drawing.Size(139, 21);
      this.cbSeriesComplete.TabIndex = 33;
      this.cbSeriesComplete.TextChanged += new EventHandler(this.IconTextsChanged);
      this.labelSeriesComplete.AutoSize = true;
      this.labelSeriesComplete.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelSeriesComplete.Location = new System.Drawing.Point(414, 129);
      this.labelSeriesComplete.Name = "labelSeriesComplete";
      this.labelSeriesComplete.Size = new System.Drawing.Size(90, 12);
      this.labelSeriesComplete.TabIndex = 32;
      this.labelSeriesComplete.Text = "Series complete:";
      this.cbEnableDynamicUpdate.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbEnableDynamicUpdate.FormattingEnabled = true;
      this.cbEnableDynamicUpdate.Location = new System.Drawing.Point(416, 371);
      this.cbEnableDynamicUpdate.Name = "cbEnableDynamicUpdate";
      this.cbEnableDynamicUpdate.Size = new System.Drawing.Size(139, 21);
      this.cbEnableDynamicUpdate.TabIndex = 59;
      this.labelEnableDynamicUpdate.AutoSize = true;
      this.labelEnableDynamicUpdate.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelEnableDynamicUpdate.Location = new System.Drawing.Point(414, 357);
      this.labelEnableDynamicUpdate.Name = "labelEnableDynamicUpdate";
      this.labelEnableDynamicUpdate.Size = new System.Drawing.Size(103, 12);
      this.labelEnableDynamicUpdate.TabIndex = 58;
      this.labelEnableDynamicUpdate.Text = "Include in Updates:";
      this.txGenre.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txGenre.Location = new System.Drawing.Point(10, 372);
      this.txGenre.Name = "txGenre";
      this.txGenre.Size = new System.Drawing.Size(392, 20);
      this.txGenre.TabIndex = 57;
      this.labelTags.AutoSize = true;
      this.labelTags.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelTags.Location = new System.Drawing.Point(11, 395);
      this.labelTags.Name = "labelTags";
      this.labelTags.Size = new System.Drawing.Size(33, 12);
      this.labelTags.TabIndex = 60;
      this.labelTags.Text = "Tags:";
      this.cbEnableProposed.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbEnableProposed.FormattingEnabled = true;
      this.cbEnableProposed.Location = new System.Drawing.Point(416, 410);
      this.cbEnableProposed.Name = "cbEnableProposed";
      this.cbEnableProposed.Size = new System.Drawing.Size(139, 21);
      this.cbEnableProposed.TabIndex = 63;
      this.cbEnableProposed.SelectedIndexChanged += new EventHandler(this.cbEnableShadowValues_SelectedIndexChanged);
      this.txTags.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txTags.Location = new System.Drawing.Point(11, 411);
      this.txTags.Name = "txTags";
      this.txTags.Size = new System.Drawing.Size(392, 20);
      this.txTags.TabIndex = 61;
      this.txTags.TextChanged += new EventHandler(this.IconTextsChanged);
      this.labelEnableProposed.AutoSize = true;
      this.labelEnableProposed.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelEnableProposed.Location = new System.Drawing.Point(414, 395);
      this.labelEnableProposed.Name = "labelEnableProposed";
      this.labelEnableProposed.Size = new System.Drawing.Size(94, 12);
      this.labelEnableProposed.TabIndex = 62;
      this.labelEnableProposed.Text = "Proposed Values:";
      this.txVolume.Location = new System.Drawing.Point(216, 28);
      this.txVolume.Name = "txVolume";
      this.txVolume.PromptText = "";
      this.txVolume.Size = new System.Drawing.Size(57, 20);
      this.txVolume.TabIndex = 3;
      this.cbAgeRating.FormattingEnabled = true;
      this.cbAgeRating.Location = new System.Drawing.Point(416, 200);
      this.cbAgeRating.Name = "cbAgeRating";
      this.cbAgeRating.Size = new System.Drawing.Size(139, 21);
      this.cbAgeRating.TabIndex = 39;
      this.cbAgeRating.TextChanged += new EventHandler(this.IconTextsChanged);
      this.txEditor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txEditor.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txEditor.Location = new System.Drawing.Point(11, 315);
      this.txEditor.Name = "txEditor";
      this.txEditor.Size = new System.Drawing.Size(200, 20);
      this.txEditor.TabIndex = 53;
      this.txEditor.Tag = (object) "Editor";
      this.labelEditor.AutoSize = true;
      this.labelEditor.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelEditor.Location = new System.Drawing.Point(9, 300);
      this.labelEditor.Name = "labelEditor";
      this.labelEditor.Size = new System.Drawing.Size(39, 12);
      this.labelEditor.TabIndex = 52;
      this.labelEditor.Text = "Editor:";
      this.txMonth.Location = new System.Drawing.Point(278, 66);
      this.txMonth.MaxLength = 2;
      this.txMonth.Name = "txMonth";
      this.txMonth.PromptText = "";
      this.txMonth.Size = new System.Drawing.Size(65, 20);
      this.txMonth.TabIndex = 15;
      this.txYear.Location = new System.Drawing.Point(216, 66);
      this.txYear.MaxLength = 4;
      this.txYear.Name = "txYear";
      this.txYear.PromptText = "";
      this.txYear.Size = new System.Drawing.Size(57, 20);
      this.txYear.TabIndex = 13;
      this.txYear.TextChanged += new EventHandler(this.IconTextsChanged);
      this.labelAgeRating.AutoSize = true;
      this.labelAgeRating.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAgeRating.Location = new System.Drawing.Point(414, 187);
      this.labelAgeRating.Name = "labelAgeRating";
      this.labelAgeRating.Size = new System.Drawing.Size(65, 12);
      this.labelAgeRating.TabIndex = 38;
      this.labelAgeRating.Text = "Age Rating:";
      this.cbFormat.FormattingEnabled = true;
      this.cbFormat.Location = new System.Drawing.Point(416, 27);
      this.cbFormat.Name = "cbFormat";
      this.cbFormat.PromptText = (string) null;
      this.cbFormat.Size = new System.Drawing.Size(139, 21);
      this.cbFormat.TabIndex = 9;
      this.cbFormat.TextChanged += new EventHandler(this.IconTextsChanged);
      this.txColorist.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txColorist.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txColorist.Location = new System.Drawing.Point(216, 239);
      this.txColorist.Name = "txColorist";
      this.txColorist.Size = new System.Drawing.Size(187, 20);
      this.txColorist.TabIndex = 43;
      this.txColorist.Tag = (object) "Colorist";
      this.txSeries.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txSeries.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txSeries.Location = new System.Drawing.Point(11, 28);
      this.txSeries.Name = "txSeries";
      this.txSeries.PromptText = "";
      this.txSeries.Size = new System.Drawing.Size(201, 20);
      this.txSeries.TabIndex = 1;
      this.txSeries.Tag = (object) "Series";
      this.labelFormat.AutoSize = true;
      this.labelFormat.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelFormat.Location = new System.Drawing.Point(414, 13);
      this.labelFormat.Name = "labelFormat";
      this.labelFormat.Size = new System.Drawing.Size(45, 12);
      this.labelFormat.TabIndex = 8;
      this.labelFormat.Text = "Format:";
      this.labelAlternateSeries.AutoSize = true;
      this.labelAlternateSeries.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAlternateSeries.Location = new System.Drawing.Point(9, 89);
      this.labelAlternateSeries.Name = "labelAlternateSeries";
      this.labelAlternateSeries.Size = new System.Drawing.Size(177, 12);
      this.labelAlternateSeries.TabIndex = 20;
      this.labelAlternateSeries.Text = "Alternate Series or Storyline Title:";
      this.txAlternateSeries.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txAlternateSeries.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txAlternateSeries.Location = new System.Drawing.Point(11, 104);
      this.txAlternateSeries.Name = "txAlternateSeries";
      this.txAlternateSeries.PromptText = "";
      this.txAlternateSeries.Size = new System.Drawing.Size(262, 20);
      this.txAlternateSeries.TabIndex = 21;
      this.txAlternateSeries.Tag = (object) "AlternateSeries";
      this.cbImprint.FormattingEnabled = true;
      this.cbImprint.Location = new System.Drawing.Point(416, 103);
      this.cbImprint.Name = "cbImprint";
      this.cbImprint.Size = new System.Drawing.Size(139, 21);
      this.cbImprint.TabIndex = 27;
      this.cbImprint.Tag = (object) "Imprint";
      this.cbImprint.TextChanged += new EventHandler(this.IconTextsChanged);
      this.cbBlackAndWhite.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbBlackAndWhite.FormattingEnabled = true;
      this.cbBlackAndWhite.Location = new System.Drawing.Point(416, 312);
      this.cbBlackAndWhite.Name = "cbBlackAndWhite";
      this.cbBlackAndWhite.Size = new System.Drawing.Size(139, 21);
      this.cbBlackAndWhite.TabIndex = 55;
      this.cbBlackAndWhite.TextChanged += new EventHandler(this.IconTextsChanged);
      this.labelVolume.AutoSize = true;
      this.labelVolume.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelVolume.Location = new System.Drawing.Point(216, 13);
      this.labelVolume.Name = "labelVolume";
      this.labelVolume.Size = new System.Drawing.Size(47, 12);
      this.labelVolume.TabIndex = 2;
      this.labelVolume.Text = "Volume:";
      this.txInker.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txInker.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txInker.Location = new System.Drawing.Point(10, 239);
      this.txInker.Name = "txInker";
      this.txInker.Size = new System.Drawing.Size(201, 20);
      this.txInker.TabIndex = 41;
      this.txInker.Tag = (object) "Inker";
      this.cbManga.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbManga.FormattingEnabled = true;
      this.cbManga.Location = new System.Drawing.Point(416, 237);
      this.cbManga.Name = "cbManga";
      this.cbManga.Size = new System.Drawing.Size(139, 21);
      this.cbManga.TabIndex = 45;
      this.cbManga.TextChanged += new EventHandler(this.IconTextsChanged);
      this.labelYear.AutoSize = true;
      this.labelYear.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelYear.Location = new System.Drawing.Point(216, 52);
      this.labelYear.Name = "labelYear";
      this.labelYear.Size = new System.Drawing.Size(31, 12);
      this.labelYear.TabIndex = 12;
      this.labelYear.Text = "Year:";
      this.labelMonth.AutoSize = true;
      this.labelMonth.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelMonth.Location = new System.Drawing.Point(276, 51);
      this.labelMonth.Name = "labelMonth";
      this.labelMonth.Size = new System.Drawing.Size(41, 12);
      this.labelMonth.TabIndex = 14;
      this.labelMonth.Text = "Month:";
      this.labelBlackAndWhite.AutoSize = true;
      this.labelBlackAndWhite.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBlackAndWhite.Location = new System.Drawing.Point(414, 297);
      this.labelBlackAndWhite.Name = "labelBlackAndWhite";
      this.labelBlackAndWhite.Size = new System.Drawing.Size(90, 12);
      this.labelBlackAndWhite.TabIndex = 54;
      this.labelBlackAndWhite.Text = "Black and White:";
      this.txAlternateCount.Location = new System.Drawing.Point(349, 104);
      this.txAlternateCount.Name = "txAlternateCount";
      this.txAlternateCount.PromptText = "";
      this.txAlternateCount.Size = new System.Drawing.Size(55, 20);
      this.txAlternateCount.TabIndex = 25;
      this.labelManga.AutoSize = true;
      this.labelManga.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelManga.Location = new System.Drawing.Point(414, 222);
      this.labelManga.Name = "labelManga";
      this.labelManga.Size = new System.Drawing.Size(43, 12);
      this.labelManga.TabIndex = 44;
      this.labelManga.Text = "Manga:";
      this.labelSeries.AutoSize = true;
      this.labelSeries.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelSeries.Location = new System.Drawing.Point(9, 13);
      this.labelSeries.Name = "labelSeries";
      this.labelSeries.Size = new System.Drawing.Size(41, 12);
      this.labelSeries.TabIndex = 0;
      this.labelSeries.Text = "Series:";
      this.labelLanguage.AutoSize = true;
      this.labelLanguage.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelLanguage.Location = new System.Drawing.Point(414, 261);
      this.labelLanguage.Name = "labelLanguage";
      this.labelLanguage.Size = new System.Drawing.Size(57, 12);
      this.labelLanguage.TabIndex = 50;
      this.labelLanguage.Text = "Language:";
      this.labelImprint.AutoSize = true;
      this.labelImprint.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelImprint.Location = new System.Drawing.Point(414, 88);
      this.labelImprint.Name = "labelImprint";
      this.labelImprint.Size = new System.Drawing.Size(45, 12);
      this.labelImprint.TabIndex = 26;
      this.labelImprint.Text = "Imprint:";
      this.labelGenre.AutoSize = true;
      this.labelGenre.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelGenre.Location = new System.Drawing.Point(11, 357);
      this.labelGenre.Name = "labelGenre";
      this.labelGenre.Size = new System.Drawing.Size(39, 12);
      this.labelGenre.TabIndex = 56;
      this.labelGenre.Text = "Genre:";
      this.labelColorist.AutoSize = true;
      this.labelColorist.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelColorist.Location = new System.Drawing.Point(214, 224);
      this.labelColorist.Name = "labelColorist";
      this.labelColorist.Size = new System.Drawing.Size(49, 12);
      this.labelColorist.TabIndex = 42;
      this.labelColorist.Text = "Colorist:";
      this.txCount.Location = new System.Drawing.Point(349, 28);
      this.txCount.Name = "txCount";
      this.txCount.PromptText = "";
      this.txCount.Size = new System.Drawing.Size(55, 20);
      this.txCount.TabIndex = 7;
      this.cbLanguage.CultureTypes = CultureTypes.NeutralCultures;
      this.cbLanguage.DrawMode = DrawMode.OwnerDrawVariable;
      this.cbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbLanguage.FormattingEnabled = true;
      this.cbLanguage.IntegralHeight = false;
      this.cbLanguage.Location = new System.Drawing.Point(416, 276);
      this.cbLanguage.Name = "cbLanguage";
      this.cbLanguage.SelectedCulture = "";
      this.cbLanguage.SelectedTwoLetterISOLanguage = "";
      this.cbLanguage.Size = new System.Drawing.Size(139, 21);
      this.cbLanguage.TabIndex = 51;
      this.cbLanguage.TopTwoLetterISOLanguages = (IEnumerable<string>) null;
      this.cbPublisher.FormattingEnabled = true;
      this.cbPublisher.Location = new System.Drawing.Point(416, 65);
      this.cbPublisher.Name = "cbPublisher";
      this.cbPublisher.Size = new System.Drawing.Size(139, 21);
      this.cbPublisher.TabIndex = 19;
      this.cbPublisher.Tag = (object) "Publisher";
      this.cbPublisher.TextChanged += new EventHandler(this.IconTextsChanged);
      this.txPenciller.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txPenciller.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txPenciller.Location = new System.Drawing.Point(216, 200);
      this.txPenciller.Name = "txPenciller";
      this.txPenciller.Size = new System.Drawing.Size(187, 20);
      this.txPenciller.TabIndex = 37;
      this.txPenciller.Tag = (object) "Penciller";
      this.txAlternateNumber.Location = new System.Drawing.Point(278, 104);
      this.txAlternateNumber.Name = "txAlternateNumber";
      this.txAlternateNumber.PromptText = "";
      this.txAlternateNumber.Size = new System.Drawing.Size(65, 20);
      this.txAlternateNumber.TabIndex = 23;
      this.txNumber.Location = new System.Drawing.Point(278, 28);
      this.txNumber.Name = "txNumber";
      this.txNumber.PromptText = "";
      this.txNumber.Size = new System.Drawing.Size(65, 20);
      this.txNumber.TabIndex = 5;
      this.labelPublisher.AutoSize = true;
      this.labelPublisher.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelPublisher.Location = new System.Drawing.Point(414, 51);
      this.labelPublisher.Name = "labelPublisher";
      this.labelPublisher.Size = new System.Drawing.Size(56, 12);
      this.labelPublisher.TabIndex = 18;
      this.labelPublisher.Text = "Publisher:";
      this.labelCoverArtist.AutoSize = true;
      this.labelCoverArtist.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCoverArtist.Location = new System.Drawing.Point(214, 262);
      this.labelCoverArtist.Name = "labelCoverArtist";
      this.labelCoverArtist.Size = new System.Drawing.Size(72, 12);
      this.labelCoverArtist.TabIndex = 48;
      this.labelCoverArtist.Text = "Cover Artist:";
      this.txCoverArtist.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txCoverArtist.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txCoverArtist.Location = new System.Drawing.Point(216, 277);
      this.txCoverArtist.Name = "txCoverArtist";
      this.txCoverArtist.Size = new System.Drawing.Size(187, 20);
      this.txCoverArtist.TabIndex = 49;
      this.txCoverArtist.Tag = (object) "CoverArtist";
      this.labelInker.AutoSize = true;
      this.labelInker.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelInker.Location = new System.Drawing.Point(11, 224);
      this.labelInker.Name = "labelInker";
      this.labelInker.Size = new System.Drawing.Size(35, 12);
      this.labelInker.TabIndex = 40;
      this.labelInker.Text = "Inker:";
      this.labelAlternateCount.AutoSize = true;
      this.labelAlternateCount.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAlternateCount.Location = new System.Drawing.Point(347, 89);
      this.labelAlternateCount.Name = "labelAlternateCount";
      this.labelAlternateCount.Size = new System.Drawing.Size(19, 12);
      this.labelAlternateCount.TabIndex = 24;
      this.labelAlternateCount.Text = "of:";
      this.txTitle.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txTitle.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txTitle.Location = new System.Drawing.Point(11, 66);
      this.txTitle.Name = "txTitle";
      this.txTitle.Size = new System.Drawing.Size(201, 20);
      this.txTitle.TabIndex = 11;
      this.txTitle.Tag = (object) "Title";
      this.labelCount.AutoSize = true;
      this.labelCount.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCount.Location = new System.Drawing.Point(347, 13);
      this.labelCount.Name = "labelCount";
      this.labelCount.Size = new System.Drawing.Size(19, 12);
      this.labelCount.TabIndex = 6;
      this.labelCount.Text = "of:";
      this.labelAlternateNumber.AutoSize = true;
      this.labelAlternateNumber.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAlternateNumber.Location = new System.Drawing.Point(276, 89);
      this.labelAlternateNumber.Name = "labelAlternateNumber";
      this.labelAlternateNumber.Size = new System.Drawing.Size(48, 12);
      this.labelAlternateNumber.TabIndex = 22;
      this.labelAlternateNumber.Text = "Number:";
      this.labelNumber.AutoSize = true;
      this.labelNumber.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelNumber.Location = new System.Drawing.Point(276, 13);
      this.labelNumber.Name = "labelNumber";
      this.labelNumber.Size = new System.Drawing.Size(48, 12);
      this.labelNumber.TabIndex = 4;
      this.labelNumber.Text = "Number:";
      this.labelLetterer.AutoSize = true;
      this.labelLetterer.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelLetterer.Location = new System.Drawing.Point(9, 262);
      this.labelLetterer.Name = "labelLetterer";
      this.labelLetterer.Size = new System.Drawing.Size(49, 12);
      this.labelLetterer.TabIndex = 46;
      this.labelLetterer.Text = "Letterer:";
      this.labelPenciller.AutoSize = true;
      this.labelPenciller.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelPenciller.Location = new System.Drawing.Point(214, 187);
      this.labelPenciller.Name = "labelPenciller";
      this.labelPenciller.Size = new System.Drawing.Size(53, 12);
      this.labelPenciller.TabIndex = 36;
      this.labelPenciller.Text = "Penciller:";
      this.txLetterer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txLetterer.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txLetterer.Location = new System.Drawing.Point(11, 277);
      this.txLetterer.Name = "txLetterer";
      this.txLetterer.Size = new System.Drawing.Size(200, 20);
      this.txLetterer.TabIndex = 47;
      this.txLetterer.Tag = (object) "Letterer";
      this.labelTitle.AutoSize = true;
      this.labelTitle.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelTitle.Location = new System.Drawing.Point(9, 51);
      this.labelTitle.Name = "labelTitle";
      this.labelTitle.Size = new System.Drawing.Size(31, 12);
      this.labelTitle.TabIndex = 10;
      this.labelTitle.Text = "Title:";
      this.labelWriter.AutoSize = true;
      this.labelWriter.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelWriter.Location = new System.Drawing.Point(9, 187);
      this.labelWriter.Name = "labelWriter";
      this.labelWriter.Size = new System.Drawing.Size(40, 12);
      this.labelWriter.TabIndex = 34;
      this.labelWriter.Text = "Writer:";
      this.txWriter.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txWriter.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txWriter.Location = new System.Drawing.Point(11, 200);
      this.txWriter.Name = "txWriter";
      this.txWriter.Size = new System.Drawing.Size(200, 20);
      this.txWriter.TabIndex = 35;
      this.txWriter.Tag = (object) "Writer";
      this.tabPlot.Controls.Add((Control) this.tabNotes);
      this.tabPlot.Controls.Add((Control) this.txMainCharacterOrTeam);
      this.tabPlot.Controls.Add((Control) this.labelMainCharacterOrTeam);
      this.tabPlot.Controls.Add((Control) this.txScanInformation);
      this.tabPlot.Controls.Add((Control) this.labelScanInformation);
      this.tabPlot.Controls.Add((Control) this.txLocations);
      this.tabPlot.Controls.Add((Control) this.labelLocations);
      this.tabPlot.Controls.Add((Control) this.txTeams);
      this.tabPlot.Controls.Add((Control) this.labelTeams);
      this.tabPlot.Controls.Add((Control) this.txWeblink);
      this.tabPlot.Controls.Add((Control) this.labelWeb);
      this.tabPlot.Controls.Add((Control) this.txCharacters);
      this.tabPlot.Controls.Add((Control) this.labelCharacters);
      this.tabPlot.Location = new System.Drawing.Point(4, 22);
      this.tabPlot.Name = "tabPlot";
      this.tabPlot.Size = new System.Drawing.Size(566, 442);
      this.tabPlot.TabIndex = 8;
      this.tabPlot.Text = "Plot & Notes";
      this.tabPlot.UseVisualStyleBackColor = true;
      this.tabNotes.Controls.Add((Control) this.tabPageSummary);
      this.tabNotes.Controls.Add((Control) this.tabPageNotes);
      this.tabNotes.Controls.Add((Control) this.tabPageReview);
      this.tabNotes.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.tabNotes.Location = new System.Drawing.Point(11, 15);
      this.tabNotes.Multiline = true;
      this.tabNotes.Name = "tabNotes";
      this.tabNotes.SelectedIndex = 0;
      this.tabNotes.Size = new System.Drawing.Size(539, 233);
      this.tabNotes.TabIndex = 0;
      this.tabPageSummary.Controls.Add((Control) this.txSummary);
      this.tabPageSummary.Location = new System.Drawing.Point(4, 21);
      this.tabPageSummary.Name = "tabPageSummary";
      this.tabPageSummary.Padding = new Padding(3);
      this.tabPageSummary.Size = new System.Drawing.Size(531, 208);
      this.tabPageSummary.TabIndex = 0;
      this.tabPageSummary.Text = "Summary";
      this.tabPageSummary.UseVisualStyleBackColor = true;
      this.txSummary.AcceptsReturn = true;
      this.txSummary.Dock = DockStyle.Fill;
      this.txSummary.FocusSelect = false;
      this.txSummary.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txSummary.Location = new System.Drawing.Point(3, 3);
      this.txSummary.Multiline = true;
      this.txSummary.Name = "txSummary";
      this.txSummary.ScrollBars = ScrollBars.Vertical;
      this.txSummary.Size = new System.Drawing.Size(525, 202);
      this.txSummary.TabIndex = 2;
      this.tabPageNotes.Controls.Add((Control) this.txNotes);
      this.tabPageNotes.Location = new System.Drawing.Point(4, 21);
      this.tabPageNotes.Name = "tabPageNotes";
      this.tabPageNotes.Padding = new Padding(3);
      this.tabPageNotes.Size = new System.Drawing.Size(521, 208);
      this.tabPageNotes.TabIndex = 1;
      this.tabPageNotes.Text = "Notes";
      this.tabPageNotes.UseVisualStyleBackColor = true;
      this.txNotes.AcceptsReturn = true;
      this.txNotes.Dock = DockStyle.Fill;
      this.txNotes.FocusSelect = false;
      this.txNotes.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txNotes.Location = new System.Drawing.Point(3, 3);
      this.txNotes.Multiline = true;
      this.txNotes.Name = "txNotes";
      this.txNotes.ScrollBars = ScrollBars.Vertical;
      this.txNotes.Size = new System.Drawing.Size(515, 202);
      this.txNotes.TabIndex = 10;
      this.tabPageReview.Controls.Add((Control) this.txReview);
      this.tabPageReview.Location = new System.Drawing.Point(4, 21);
      this.tabPageReview.Name = "tabPageReview";
      this.tabPageReview.Padding = new Padding(3);
      this.tabPageReview.Size = new System.Drawing.Size(521, 208);
      this.tabPageReview.TabIndex = 2;
      this.tabPageReview.Text = "Review";
      this.tabPageReview.UseVisualStyleBackColor = true;
      this.txReview.AcceptsReturn = true;
      this.txReview.Dock = DockStyle.Fill;
      this.txReview.FocusSelect = false;
      this.txReview.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txReview.Location = new System.Drawing.Point(3, 3);
      this.txReview.Multiline = true;
      this.txReview.Name = "txReview";
      this.txReview.ScrollBars = ScrollBars.Vertical;
      this.txReview.Size = new System.Drawing.Size(515, 202);
      this.txReview.TabIndex = 10;
      this.txMainCharacterOrTeam.AcceptsReturn = true;
      this.txMainCharacterOrTeam.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txMainCharacterOrTeam.Location = new System.Drawing.Point(286, 278);
      this.txMainCharacterOrTeam.Name = "txMainCharacterOrTeam";
      this.txMainCharacterOrTeam.Size = new System.Drawing.Size(264, 20);
      this.txMainCharacterOrTeam.TabIndex = 4;
      this.txMainCharacterOrTeam.Tag = (object) "Teams";
      this.labelMainCharacterOrTeam.AutoSize = true;
      this.labelMainCharacterOrTeam.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelMainCharacterOrTeam.Location = new System.Drawing.Point(284, 263);
      this.labelMainCharacterOrTeam.Name = "labelMainCharacterOrTeam";
      this.labelMainCharacterOrTeam.Size = new System.Drawing.Size(130, 12);
      this.labelMainCharacterOrTeam.TabIndex = 3;
      this.labelMainCharacterOrTeam.Text = "Main Character or Team:";
      this.txScanInformation.AcceptsReturn = true;
      this.txScanInformation.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txScanInformation.Location = new System.Drawing.Point(11, 412);
      this.txScanInformation.Name = "txScanInformation";
      this.txScanInformation.Size = new System.Drawing.Size(260, 20);
      this.txScanInformation.TabIndex = 10;
      this.txScanInformation.Tag = (object) "";
      this.labelScanInformation.AutoSize = true;
      this.labelScanInformation.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelScanInformation.Location = new System.Drawing.Point(9, 397);
      this.labelScanInformation.Name = "labelScanInformation";
      this.labelScanInformation.Size = new System.Drawing.Size(95, 12);
      this.labelScanInformation.TabIndex = 9;
      this.labelScanInformation.Text = "Scan Information:";
      this.txLocations.AcceptsReturn = true;
      this.txLocations.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txLocations.Location = new System.Drawing.Point(287, 351);
      this.txLocations.Name = "txLocations";
      this.txLocations.Size = new System.Drawing.Size(263, 20);
      this.txLocations.TabIndex = 8;
      this.txLocations.Tag = (object) "Locations";
      this.labelLocations.AutoSize = true;
      this.labelLocations.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelLocations.Location = new System.Drawing.Point(285, 336);
      this.labelLocations.Name = "labelLocations";
      this.labelLocations.Size = new System.Drawing.Size(58, 12);
      this.labelLocations.TabIndex = 7;
      this.labelLocations.Text = "Locations:";
      this.txTeams.AcceptsReturn = true;
      this.txTeams.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txTeams.Location = new System.Drawing.Point(286, 316);
      this.txTeams.Name = "txTeams";
      this.txTeams.Size = new System.Drawing.Size(264, 20);
      this.txTeams.TabIndex = 6;
      this.txTeams.Tag = (object) "Teams";
      this.labelTeams.AutoSize = true;
      this.labelTeams.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelTeams.Location = new System.Drawing.Point(284, 301);
      this.labelTeams.Name = "labelTeams";
      this.labelTeams.Size = new System.Drawing.Size(42, 12);
      this.labelTeams.TabIndex = 5;
      this.labelTeams.Text = "Teams:";
      this.txWeblink.AcceptsReturn = true;
      this.txWeblink.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txWeblink.Location = new System.Drawing.Point(287, 412);
      this.txWeblink.Name = "txWeblink";
      this.txWeblink.Size = new System.Drawing.Size(263, 20);
      this.txWeblink.TabIndex = 12;
      this.txWeblink.Tag = (object) "";
      this.labelWeb.AutoSize = true;
      this.labelWeb.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelWeb.Location = new System.Drawing.Point(285, 397);
      this.labelWeb.Name = "labelWeb";
      this.labelWeb.Size = new System.Drawing.Size(31, 12);
      this.labelWeb.TabIndex = 11;
      this.labelWeb.Text = "Web:";
      this.txCharacters.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txCharacters.Location = new System.Drawing.Point(11, 278);
      this.txCharacters.Multiline = true;
      this.txCharacters.Name = "txCharacters";
      this.txCharacters.Size = new System.Drawing.Size(260, 93);
      this.txCharacters.TabIndex = 2;
      this.txCharacters.Tag = (object) "Characters";
      this.txCharacters.KeyPress += new KeyPressEventHandler(this.txCharacters_KeyPress);
      this.labelCharacters.AutoSize = true;
      this.labelCharacters.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCharacters.Location = new System.Drawing.Point(9, 263);
      this.labelCharacters.Name = "labelCharacters";
      this.labelCharacters.Size = new System.Drawing.Size(65, 12);
      this.labelCharacters.TabIndex = 1;
      this.labelCharacters.Text = "Characters:";
      this.tabCatalog.Controls.Add((Control) this.labelReleasedTime);
      this.tabCatalog.Controls.Add((Control) this.dtpReleasedTime);
      this.tabCatalog.Controls.Add((Control) this.labelOpenedTime);
      this.tabCatalog.Controls.Add((Control) this.dtpOpenedTime);
      this.tabCatalog.Controls.Add((Control) this.labelAddedTime);
      this.tabCatalog.Controls.Add((Control) this.dtpAddedTime);
      this.tabCatalog.Controls.Add((Control) this.txPagesAsTextSimple);
      this.tabCatalog.Controls.Add((Control) this.labelPagesAsTextSimple);
      this.tabCatalog.Controls.Add((Control) this.txISBN);
      this.tabCatalog.Controls.Add((Control) this.labelISBN);
      this.tabCatalog.Controls.Add((Control) this.cbBookLocation);
      this.tabCatalog.Controls.Add((Control) this.labelBookLocation);
      this.tabCatalog.Controls.Add((Control) this.txCollectionStatus);
      this.tabCatalog.Controls.Add((Control) this.cbBookPrice);
      this.tabCatalog.Controls.Add((Control) this.labelBookPrice);
      this.tabCatalog.Controls.Add((Control) this.txBookNotes);
      this.tabCatalog.Controls.Add((Control) this.labelBookNotes);
      this.tabCatalog.Controls.Add((Control) this.cbBookAge);
      this.tabCatalog.Controls.Add((Control) this.labelBookAge);
      this.tabCatalog.Controls.Add((Control) this.labelBookCollectionStatus);
      this.tabCatalog.Controls.Add((Control) this.cbBookCondition);
      this.tabCatalog.Controls.Add((Control) this.labelBookCondition);
      this.tabCatalog.Controls.Add((Control) this.cbBookStore);
      this.tabCatalog.Controls.Add((Control) this.labelBookStore);
      this.tabCatalog.Controls.Add((Control) this.cbBookOwner);
      this.tabCatalog.Controls.Add((Control) this.labelBookOwner);
      this.tabCatalog.Location = new System.Drawing.Point(4, 22);
      this.tabCatalog.Name = "tabCatalog";
      this.tabCatalog.Padding = new Padding(3);
      this.tabCatalog.Size = new System.Drawing.Size(566, 442);
      this.tabCatalog.TabIndex = 9;
      this.tabCatalog.Text = "Catalog";
      this.tabCatalog.UseVisualStyleBackColor = true;
      this.labelReleasedTime.AutoSize = true;
      this.labelReleasedTime.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelReleasedTime.Location = new System.Drawing.Point(314, 21);
      this.labelReleasedTime.Name = "labelReleasedTime";
      this.labelReleasedTime.Size = new System.Drawing.Size(56, 12);
      this.labelReleasedTime.TabIndex = 12;
      this.labelReleasedTime.Text = "Released:";
      this.dtpReleasedTime.CustomFormat = " ";
      this.dtpReleasedTime.Location = new System.Drawing.Point(315, 36);
      this.dtpReleasedTime.Name = "dtpReleasedTime";
      this.dtpReleasedTime.Size = new System.Drawing.Size(235, 20);
      this.dtpReleasedTime.TabIndex = 13;
      this.dtpReleasedTime.Value = new DateTime(0L);
      this.labelOpenedTime.AutoSize = true;
      this.labelOpenedTime.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelOpenedTime.Location = new System.Drawing.Point(315, 98);
      this.labelOpenedTime.Name = "labelOpenedTime";
      this.labelOpenedTime.Size = new System.Drawing.Size(77, 12);
      this.labelOpenedTime.TabIndex = 16;
      this.labelOpenedTime.Text = "Opened/Read:";
      this.dtpOpenedTime.CustomFormat = " ";
      this.dtpOpenedTime.Location = new System.Drawing.Point(316, 113);
      this.dtpOpenedTime.Name = "dtpOpenedTime";
      this.dtpOpenedTime.Size = new System.Drawing.Size(234, 20);
      this.dtpOpenedTime.TabIndex = 17;
      this.dtpOpenedTime.Value = new DateTime(0L);
      this.labelAddedTime.AutoSize = true;
      this.labelAddedTime.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAddedTime.Location = new System.Drawing.Point(314, 58);
      this.labelAddedTime.Name = "labelAddedTime";
      this.labelAddedTime.Size = new System.Drawing.Size(98, 12);
      this.labelAddedTime.TabIndex = 14;
      this.labelAddedTime.Text = "Added/Purchased:";
      this.dtpAddedTime.CustomFormat = " ";
      this.dtpAddedTime.Location = new System.Drawing.Point(315, 73);
      this.dtpAddedTime.Name = "dtpAddedTime";
      this.dtpAddedTime.Size = new System.Drawing.Size(235, 20);
      this.dtpAddedTime.TabIndex = 15;
      this.dtpAddedTime.Value = new DateTime(0L);
      this.txPagesAsTextSimple.AcceptsReturn = true;
      this.txPagesAsTextSimple.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txPagesAsTextSimple.Location = new System.Drawing.Point(167, 73);
      this.txPagesAsTextSimple.Name = "txPagesAsTextSimple";
      this.txPagesAsTextSimple.Size = new System.Drawing.Size(126, 20);
      this.txPagesAsTextSimple.TabIndex = 7;
      this.txPagesAsTextSimple.Tag = (object) "PageCountTextSimple";
      this.labelPagesAsTextSimple.AutoSize = true;
      this.labelPagesAsTextSimple.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelPagesAsTextSimple.Location = new System.Drawing.Point(165, 58);
      this.labelPagesAsTextSimple.Name = "labelPagesAsTextSimple";
      this.labelPagesAsTextSimple.Size = new System.Drawing.Size(40, 12);
      this.labelPagesAsTextSimple.TabIndex = 6;
      this.labelPagesAsTextSimple.Text = "Pages:";
      this.txISBN.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txISBN.Location = new System.Drawing.Point(11, 73);
      this.txISBN.Name = "txISBN";
      this.txISBN.Size = new System.Drawing.Size(150, 20);
      this.txISBN.TabIndex = 5;
      this.txISBN.Tag = (object) "ISBN";
      this.labelISBN.AutoSize = true;
      this.labelISBN.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelISBN.Location = new System.Drawing.Point(9, 58);
      this.labelISBN.Name = "labelISBN";
      this.labelISBN.Size = new System.Drawing.Size(35, 12);
      this.labelISBN.TabIndex = 4;
      this.labelISBN.Text = "ISBN:";
      this.cbBookLocation.FormattingEnabled = true;
      this.cbBookLocation.Location = new System.Drawing.Point(317, 168);
      this.cbBookLocation.Name = "cbBookLocation";
      this.cbBookLocation.Size = new System.Drawing.Size(233, 21);
      this.cbBookLocation.TabIndex = 21;
      this.cbBookLocation.Tag = (object) "BookLocation";
      this.labelBookLocation.AutoSize = true;
      this.labelBookLocation.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookLocation.Location = new System.Drawing.Point(315, 154);
      this.labelBookLocation.Name = "labelBookLocation";
      this.labelBookLocation.Size = new System.Drawing.Size(80, 12);
      this.labelBookLocation.TabIndex = 20;
      this.labelBookLocation.Text = "Book Location:";
      this.txCollectionStatus.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txCollectionStatus.Location = new System.Drawing.Point(11, 210);
      this.txCollectionStatus.Name = "txCollectionStatus";
      this.txCollectionStatus.Size = new System.Drawing.Size(539, 20);
      this.txCollectionStatus.TabIndex = 23;
      this.txCollectionStatus.Tag = (object) "CollectionStatus";
      this.cbBookPrice.FormattingEnabled = true;
      this.cbBookPrice.Location = new System.Drawing.Point(167, 34);
      this.cbBookPrice.Name = "cbBookPrice";
      this.cbBookPrice.Size = new System.Drawing.Size(126, 21);
      this.cbBookPrice.TabIndex = 3;
      this.cbBookPrice.Tag = (object) "BookPrice";
      this.labelBookPrice.AutoSize = true;
      this.labelBookPrice.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookPrice.Location = new System.Drawing.Point(165, 19);
      this.labelBookPrice.Name = "labelBookPrice";
      this.labelBookPrice.Size = new System.Drawing.Size(35, 12);
      this.labelBookPrice.TabIndex = 2;
      this.labelBookPrice.Text = "Price:";
      this.txBookNotes.AcceptsReturn = true;
      this.txBookNotes.FocusSelect = false;
      this.txBookNotes.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txBookNotes.Location = new System.Drawing.Point(11, 265);
      this.txBookNotes.Multiline = true;
      this.txBookNotes.Name = "txBookNotes";
      this.txBookNotes.ScrollBars = ScrollBars.Vertical;
      this.txBookNotes.Size = new System.Drawing.Size(539, 157);
      this.txBookNotes.TabIndex = 25;
      this.txBookNotes.Tag = (object) "BookNotes";
      this.labelBookNotes.AutoSize = true;
      this.labelBookNotes.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookNotes.Location = new System.Drawing.Point(9, 250);
      this.labelBookNotes.Name = "labelBookNotes";
      this.labelBookNotes.Size = new System.Drawing.Size(120, 12);
      this.labelBookNotes.TabIndex = 24;
      this.labelBookNotes.Text = "Notes about this Book:";
      this.cbBookAge.FormattingEnabled = true;
      this.cbBookAge.Location = new System.Drawing.Point(11, 112);
      this.cbBookAge.Name = "cbBookAge";
      this.cbBookAge.Size = new System.Drawing.Size(150, 21);
      this.cbBookAge.TabIndex = 9;
      this.cbBookAge.Tag = (object) "BookAge";
      this.labelBookAge.AutoSize = true;
      this.labelBookAge.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookAge.Location = new System.Drawing.Point(9, 97);
      this.labelBookAge.Name = "labelBookAge";
      this.labelBookAge.Size = new System.Drawing.Size(29, 12);
      this.labelBookAge.TabIndex = 8;
      this.labelBookAge.Text = "Age:";
      this.labelBookCollectionStatus.AutoSize = true;
      this.labelBookCollectionStatus.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookCollectionStatus.Location = new System.Drawing.Point(9, 195);
      this.labelBookCollectionStatus.Name = "labelBookCollectionStatus";
      this.labelBookCollectionStatus.Size = new System.Drawing.Size(96, 12);
      this.labelBookCollectionStatus.TabIndex = 22;
      this.labelBookCollectionStatus.Text = "Collection Status:";
      this.cbBookCondition.FormattingEnabled = true;
      this.cbBookCondition.Location = new System.Drawing.Point(167, 112);
      this.cbBookCondition.Name = "cbBookCondition";
      this.cbBookCondition.Size = new System.Drawing.Size(126, 21);
      this.cbBookCondition.TabIndex = 11;
      this.cbBookCondition.Tag = (object) "BookCondition";
      this.labelBookCondition.AutoSize = true;
      this.labelBookCondition.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookCondition.Location = new System.Drawing.Point(165, 98);
      this.labelBookCondition.Name = "labelBookCondition";
      this.labelBookCondition.Size = new System.Drawing.Size(57, 12);
      this.labelBookCondition.TabIndex = 10;
      this.labelBookCondition.Text = "Condition:";
      this.cbBookStore.FormattingEnabled = true;
      this.cbBookStore.Location = new System.Drawing.Point(11, 35);
      this.cbBookStore.Name = "cbBookStore";
      this.cbBookStore.PromptText = (string) null;
      this.cbBookStore.Size = new System.Drawing.Size(150, 21);
      this.cbBookStore.TabIndex = 1;
      this.cbBookStore.Tag = (object) "BookStore";
      this.labelBookStore.AutoSize = true;
      this.labelBookStore.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookStore.Location = new System.Drawing.Point(11, 21);
      this.labelBookStore.Name = "labelBookStore";
      this.labelBookStore.Size = new System.Drawing.Size(36, 12);
      this.labelBookStore.TabIndex = 0;
      this.labelBookStore.Text = "Store:";
      this.cbBookOwner.FormattingEnabled = true;
      this.cbBookOwner.Location = new System.Drawing.Point(11, 168);
      this.cbBookOwner.Name = "cbBookOwner";
      this.cbBookOwner.Size = new System.Drawing.Size(282, 21);
      this.cbBookOwner.TabIndex = 19;
      this.cbBookOwner.Tag = (object) "BookOwner";
      this.labelBookOwner.AutoSize = true;
      this.labelBookOwner.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookOwner.Location = new System.Drawing.Point(9, 154);
      this.labelBookOwner.Name = "labelBookOwner";
      this.labelBookOwner.Size = new System.Drawing.Size(42, 12);
      this.labelBookOwner.TabIndex = 18;
      this.labelBookOwner.Text = "Owner:";
      this.tabCustom.Controls.Add((Control) this.customValuesData);
      this.tabCustom.Location = new System.Drawing.Point(4, 22);
      this.tabCustom.Name = "tabCustom";
      this.tabCustom.Padding = new Padding(3);
      this.tabCustom.Size = new System.Drawing.Size(566, 442);
      this.tabCustom.TabIndex = 10;
      this.tabCustom.Text = "Custom";
      this.tabCustom.UseVisualStyleBackColor = true;
      this.customValuesData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.customValuesData.Columns.AddRange((DataGridViewColumn) this.CustomValueName, (DataGridViewColumn) this.CustomValueValue);
      this.customValuesData.Location = new System.Drawing.Point(11, 16);
      this.customValuesData.Name = "customValuesData";
      this.customValuesData.Size = new System.Drawing.Size(537, 410);
      this.customValuesData.TabIndex = 1;
      this.customValuesData.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(this.customValuesData_EditingControlShowing);
      this.CustomValueName.HeaderText = "Name";
      this.CustomValueName.Name = "CustomValueName";
      this.CustomValueName.Resizable = DataGridViewTriState.True;
      this.CustomValueValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      this.CustomValueValue.HeaderText = "Value";
      this.CustomValueValue.Name = "CustomValueValue";
      this.tabPages.Controls.Add((Control) this.btResetPages);
      this.tabPages.Controls.Add((Control) this.btPageView);
      this.tabPages.Controls.Add((Control) this.labelPagesInfo);
      this.tabPages.Controls.Add((Control) this.pagesView);
      this.tabPages.Location = new System.Drawing.Point(4, 22);
      this.tabPages.Name = "tabPages";
      this.tabPages.Size = new System.Drawing.Size(566, 442);
      this.tabPages.TabIndex = 6;
      this.tabPages.Text = "Pages";
      this.tabPages.UseVisualStyleBackColor = true;
      this.btResetPages.ContextMenuStrip = this.cmResetPages;
      this.btResetPages.Location = new System.Drawing.Point(441, 407);
      this.btResetPages.Name = "btResetPages";
      this.btResetPages.Size = new System.Drawing.Size(111, 23);
      this.btResetPages.TabIndex = 14;
      this.btResetPages.Text = "Reset";
      this.btResetPages.UseVisualStyleBackColor = true;
      this.btResetPages.ShowContextMenu += new EventHandler(this.btResetPages_ShowContextMenu);
      this.btResetPages.Click += new EventHandler(this.btResetPages_Click);
      this.cmResetPages.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.miOrderByName,
        (ToolStripItem) this.miOrderByNameNumeric
      });
      this.cmResetPages.Name = "cmResetPages";
      this.cmResetPages.Size = new System.Drawing.Size(211, 48);
      this.cmResetPages.ItemClicked += new ToolStripItemClickedEventHandler(this.cmResetPages_ItemClicked);
      this.miOrderByName.Name = "miOrderByName";
      this.miOrderByName.Size = new System.Drawing.Size(210, 22);
      this.miOrderByName.Text = "Order by Name";
      this.miOrderByNameNumeric.Name = "miOrderByNameNumeric";
      this.miOrderByNameNumeric.Size = new System.Drawing.Size(210, 22);
      this.miOrderByNameNumeric.Text = "Order by Name (numeric)";
      this.btPageView.Image = (Image) Resources.SmallArrowDown;
      this.btPageView.ImageAlign = ContentAlignment.MiddleRight;
      this.btPageView.Location = new System.Drawing.Point(479, 9);
      this.btPageView.Name = "btPageView";
      this.btPageView.Size = new System.Drawing.Size(73, 23);
      this.btPageView.TabIndex = 2;
      this.btPageView.Text = "Pages";
      this.btPageView.UseVisualStyleBackColor = true;
      this.btPageView.Click += new EventHandler(this.btPageViews_Click);
      this.labelPagesInfo.AutoSize = true;
      this.labelPagesInfo.Location = new System.Drawing.Point(8, 14);
      this.labelPagesInfo.Name = "labelPagesInfo";
      this.labelPagesInfo.Size = new System.Drawing.Size(421, 13);
      this.labelPagesInfo.TabIndex = 0;
      this.labelPagesInfo.Text = "Change the page order with drag & drop or use the context menu to change page types:";
      this.labelPagesInfo.TextAlign = ContentAlignment.MiddleLeft;
      this.labelPagesInfo.UseMnemonic = false;
      this.pagesView.Bookmark = (string) null;
      this.pagesView.CreateBackdrop = false;
      this.pagesView.Location = new System.Drawing.Point(11, 38);
      this.pagesView.Name = "pagesView";
      this.pagesView.Size = new System.Drawing.Size(541, 363);
      this.pagesView.TabIndex = 1;
      this.tabColors.Controls.Add((Control) this.panelImage);
      this.tabColors.Controls.Add((Control) this.panelImageControls);
      this.tabColors.Location = new System.Drawing.Point(4, 22);
      this.tabColors.Name = "tabColors";
      this.tabColors.Size = new System.Drawing.Size(566, 442);
      this.tabColors.TabIndex = 7;
      this.tabColors.Text = "Colors";
      this.tabColors.UseVisualStyleBackColor = true;
      this.panelImage.Controls.Add((Control) this.labelCurrentPage);
      this.panelImage.Controls.Add((Control) this.chkShowImageControls);
      this.panelImage.Controls.Add((Control) this.btLastPage);
      this.panelImage.Controls.Add((Control) this.btFirstPage);
      this.panelImage.Controls.Add((Control) this.btNextPage);
      this.panelImage.Controls.Add((Control) this.btPrevPage);
      this.panelImage.Controls.Add((Control) this.pageViewer);
      this.panelImage.Dock = DockStyle.Fill;
      this.panelImage.Location = new System.Drawing.Point(0, 0);
      this.panelImage.Name = "panelImage";
      this.panelImage.Size = new System.Drawing.Size(566, 317);
      this.panelImage.TabIndex = 12;
      this.labelCurrentPage.Anchor = AnchorStyles.Bottom;
      this.labelCurrentPage.Location = new System.Drawing.Point(184, 291);
      this.labelCurrentPage.Name = "labelCurrentPage";
      this.labelCurrentPage.Size = new System.Drawing.Size(202, 21);
      this.labelCurrentPage.TabIndex = 6;
      this.labelCurrentPage.Text = "Page Text";
      this.labelCurrentPage.TextAlign = ContentAlignment.MiddleCenter;
      this.chkShowImageControls.Anchor = AnchorStyles.Bottom;
      this.chkShowImageControls.Appearance = Appearance.Button;
      this.chkShowImageControls.Image = (Image) Resources.DoubleArrow;
      this.chkShowImageControls.ImageAlign = ContentAlignment.MiddleRight;
      this.chkShowImageControls.Location = new System.Drawing.Point(416, 291);
      this.chkShowImageControls.Name = "chkShowImageControls";
      this.chkShowImageControls.Size = new System.Drawing.Size(140, 23);
      this.chkShowImageControls.TabIndex = 5;
      this.chkShowImageControls.Text = "Image Control";
      this.chkShowImageControls.TextAlign = ContentAlignment.MiddleCenter;
      this.chkShowImageControls.TextImageRelation = TextImageRelation.TextBeforeImage;
      this.chkShowImageControls.UseVisualStyleBackColor = true;
      this.chkShowImageControls.CheckedChanged += new EventHandler(this.chkShowColorControls_CheckedChanged);
      this.btLastPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btLastPage.Image = (Image) Resources.GoLast;
      this.btLastPage.Location = new System.Drawing.Point(107, 290);
      this.btLastPage.Name = "btLastPage";
      this.btLastPage.Size = new System.Drawing.Size(32, 23);
      this.btLastPage.TabIndex = 4;
      this.btLastPage.TextImageRelation = TextImageRelation.TextBeforeImage;
      this.btLastPage.UseVisualStyleBackColor = true;
      this.btLastPage.Click += new EventHandler(this.btLastPage_Click);
      this.btFirstPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btFirstPage.Image = (Image) Resources.GoFirst;
      this.btFirstPage.Location = new System.Drawing.Point(11, 290);
      this.btFirstPage.Name = "btFirstPage";
      this.btFirstPage.Size = new System.Drawing.Size(32, 23);
      this.btFirstPage.TabIndex = 3;
      this.btFirstPage.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btFirstPage.UseVisualStyleBackColor = true;
      this.btFirstPage.Click += new EventHandler(this.btFirstPage_Click);
      this.btNextPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btNextPage.Image = (Image) Resources.GoNext;
      this.btNextPage.Location = new System.Drawing.Point(75, 290);
      this.btNextPage.Name = "btNextPage";
      this.btNextPage.Size = new System.Drawing.Size(32, 23);
      this.btNextPage.TabIndex = 2;
      this.btNextPage.TextImageRelation = TextImageRelation.TextBeforeImage;
      this.btNextPage.UseVisualStyleBackColor = true;
      this.btNextPage.Click += new EventHandler(this.btNextPage_Click);
      this.btPrevPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btPrevPage.Image = (Image) Resources.GoPrevious;
      this.btPrevPage.Location = new System.Drawing.Point(43, 290);
      this.btPrevPage.Name = "btPrevPage";
      this.btPrevPage.Size = new System.Drawing.Size(32, 23);
      this.btPrevPage.TabIndex = 1;
      this.btPrevPage.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btPrevPage.UseVisualStyleBackColor = true;
      this.btPrevPage.Click += new EventHandler(this.btPrevPage_Click);
      this.pageViewer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pageViewer.AutoScrollMode = AutoScrollMode.Pan;
      this.pageViewer.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.pageViewer.ForeColor = Color.White;
      this.pageViewer.Location = new System.Drawing.Point(11, 12);
      this.pageViewer.Name = "pageViewer";
      this.pageViewer.ScaleMode = ScaleMode.FitWidth;
      this.pageViewer.Size = new System.Drawing.Size(545, 275);
      this.pageViewer.TabIndex = 0;
      this.pageViewer.Text = "Double Click on Color to set White Point";
      this.pageViewer.TextAlignment = ContentAlignment.BottomCenter;
      this.pageViewer.VisibleChanged += new EventHandler(this.pageViewer_VisibleChanged);
      this.pageViewer.DoubleClick += new EventHandler(this.pageViewer_DoubleClick);
      this.panelImageControls.Controls.Add((Control) this.labelSaturation);
      this.panelImageControls.Controls.Add((Control) this.labelContrast);
      this.panelImageControls.Controls.Add((Control) this.tbGamma);
      this.panelImageControls.Controls.Add((Control) this.tbSaturation);
      this.panelImageControls.Controls.Add((Control) this.labelGamma);
      this.panelImageControls.Controls.Add((Control) this.tbBrightness);
      this.panelImageControls.Controls.Add((Control) this.tbSharpening);
      this.panelImageControls.Controls.Add((Control) this.tbContrast);
      this.panelImageControls.Controls.Add((Control) this.labelSharpening);
      this.panelImageControls.Controls.Add((Control) this.labelBrightness);
      this.panelImageControls.Controls.Add((Control) this.btResetColors);
      this.panelImageControls.Dock = DockStyle.Bottom;
      this.panelImageControls.Location = new System.Drawing.Point(0, 317);
      this.panelImageControls.Name = "panelImageControls";
      this.panelImageControls.Size = new System.Drawing.Size(566, 125);
      this.panelImageControls.TabIndex = 13;
      this.panelImageControls.Visible = false;
      this.labelSaturation.AutoSize = true;
      this.labelSaturation.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelSaturation.Location = new System.Drawing.Point(11, 34);
      this.labelSaturation.Name = "labelSaturation";
      this.labelSaturation.Size = new System.Drawing.Size(57, 12);
      this.labelSaturation.TabIndex = 1;
      this.labelSaturation.Text = "Saturation";
      this.labelContrast.AutoSize = true;
      this.labelContrast.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelContrast.Location = new System.Drawing.Point(296, 34);
      this.labelContrast.Name = "labelContrast";
      this.labelContrast.Size = new System.Drawing.Size(49, 12);
      this.labelContrast.TabIndex = 5;
      this.labelContrast.Text = "Contrast";
      this.tbGamma.Location = new System.Drawing.Point(363, 52);
      this.tbGamma.Minimum = -100;
      this.tbGamma.Name = "tbGamma";
      this.tbGamma.Size = new System.Drawing.Size(192, 18);
      this.tbGamma.TabIndex = 8;
      this.tbGamma.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbGamma.TickFrequency = 16;
      this.tbGamma.TickStyle = TickStyle.BottomRight;
      this.tbGamma.Scroll += new EventHandler(this.ColorAdjustment_Scroll);
      this.tbGamma.ValueChanged += new EventHandler(this.AdjustmentSliderChanged);
      this.tbSaturation.Location = new System.Drawing.Point(78, 28);
      this.tbSaturation.Minimum = -100;
      this.tbSaturation.Name = "tbSaturation";
      this.tbSaturation.Size = new System.Drawing.Size(192, 18);
      this.tbSaturation.TabIndex = 2;
      this.tbSaturation.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbSaturation.TickFrequency = 16;
      this.tbSaturation.TickStyle = TickStyle.BottomRight;
      this.tbSaturation.Scroll += new EventHandler(this.ColorAdjustment_Scroll);
      this.tbSaturation.ValueChanged += new EventHandler(this.AdjustmentSliderChanged);
      this.labelGamma.AutoSize = true;
      this.labelGamma.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelGamma.Location = new System.Drawing.Point(296, 58);
      this.labelGamma.Name = "labelGamma";
      this.labelGamma.Size = new System.Drawing.Size(43, 12);
      this.labelGamma.TabIndex = 7;
      this.labelGamma.Text = "Gamma";
      this.tbBrightness.Location = new System.Drawing.Point(78, 52);
      this.tbBrightness.Minimum = -100;
      this.tbBrightness.Name = "tbBrightness";
      this.tbBrightness.Size = new System.Drawing.Size(192, 18);
      this.tbBrightness.TabIndex = 4;
      this.tbBrightness.Text = "tbBrightness";
      this.tbBrightness.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbBrightness.TickFrequency = 16;
      this.tbBrightness.TickStyle = TickStyle.BottomRight;
      this.tbBrightness.Scroll += new EventHandler(this.ColorAdjustment_Scroll);
      this.tbBrightness.ValueChanged += new EventHandler(this.AdjustmentSliderChanged);
      this.tbSharpening.LargeChange = 1;
      this.tbSharpening.Location = new System.Drawing.Point(81, 86);
      this.tbSharpening.Maximum = 3;
      this.tbSharpening.Name = "tbSharpening";
      this.tbSharpening.Size = new System.Drawing.Size(189, 18);
      this.tbSharpening.TabIndex = 10;
      this.tbSharpening.Text = "tbSaturation";
      this.tbSharpening.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbSharpening.TickFrequency = 1;
      this.tbSharpening.TickStyle = TickStyle.BottomRight;
      this.tbSharpening.Scroll += new EventHandler(this.ColorAdjustment_Scroll);
      this.tbContrast.Location = new System.Drawing.Point(363, 28);
      this.tbContrast.Minimum = -100;
      this.tbContrast.Name = "tbContrast";
      this.tbContrast.Size = new System.Drawing.Size(192, 18);
      this.tbContrast.TabIndex = 6;
      this.tbContrast.Text = "tbSaturation";
      this.tbContrast.ThumbSize = new System.Drawing.Size(8, 16);
      this.tbContrast.TickFrequency = 16;
      this.tbContrast.TickStyle = TickStyle.BottomRight;
      this.tbContrast.Scroll += new EventHandler(this.ColorAdjustment_Scroll);
      this.tbContrast.ValueChanged += new EventHandler(this.AdjustmentSliderChanged);
      this.labelSharpening.AutoSize = true;
      this.labelSharpening.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelSharpening.Location = new System.Drawing.Point(11, 92);
      this.labelSharpening.Name = "labelSharpening";
      this.labelSharpening.Size = new System.Drawing.Size(61, 12);
      this.labelSharpening.TabIndex = 9;
      this.labelSharpening.Text = "Sharpening";
      this.labelBrightness.AutoSize = true;
      this.labelBrightness.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelBrightness.Location = new System.Drawing.Point(11, 58);
      this.labelBrightness.Name = "labelBrightness";
      this.labelBrightness.Size = new System.Drawing.Size(59, 12);
      this.labelBrightness.TabIndex = 3;
      this.labelBrightness.Text = "Brightness";
      this.btResetColors.Location = new System.Drawing.Point(478, 86);
      this.btResetColors.Name = "btResetColors";
      this.btResetColors.Size = new System.Drawing.Size(77, 24);
      this.btResetColors.TabIndex = 11;
      this.btResetColors.Text = "Reset";
      this.btResetColors.UseVisualStyleBackColor = true;
      this.btResetColors.Click += new EventHandler(this.btReset_Click);
      this.btPrev.FlatStyle = FlatStyle.System;
      this.btPrev.Location = new System.Drawing.Point(8, 483);
      this.btPrev.Name = "btPrev";
      this.btPrev.Size = new System.Drawing.Size(80, 24);
      this.btPrev.TabIndex = 1;
      this.btPrev.Text = "&Previous";
      this.btPrev.Click += new EventHandler(this.btPrev_Click);
      this.btNext.FlatStyle = FlatStyle.System;
      this.btNext.Location = new System.Drawing.Point(92, 483);
      this.btNext.Name = "btNext";
      this.btNext.Size = new System.Drawing.Size(80, 24);
      this.btNext.TabIndex = 2;
      this.btNext.Text = "&Next";
      this.btNext.Click += new EventHandler(this.btNext_Click);
      this.btScript.AutoEllipsis = true;
      this.btScript.Location = new System.Drawing.Point(188, 484);
      this.btScript.Name = "btScript";
      this.btScript.Size = new System.Drawing.Size(135, 23);
      this.btScript.TabIndex = 3;
      this.btScript.Text = "Lorem Ipsum";
      this.btScript.UseVisualStyleBackColor = true;
      this.btScript.Visible = false;
      this.btApply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btApply.FlatStyle = FlatStyle.System;
      this.btApply.Location = new System.Drawing.Point(501, 483);
      this.btApply.Name = "btApply";
      this.btApply.Size = new System.Drawing.Size(80, 24);
      this.btApply.TabIndex = 6;
      this.btApply.Text = "&Apply";
      this.btApply.Click += new EventHandler(this.btApply_Click);
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(593, 517);
      this.Controls.Add((Control) this.btApply);
      this.Controls.Add((Control) this.btScript);
      this.Controls.Add((Control) this.tabControl);
      this.Controls.Add((Control) this.btNext);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btPrev);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ComicBookDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Info";
      this.DragDrop += new DragEventHandler(this.ComicBookDialog_DragDrop);
      this.DragOver += new DragEventHandler(this.ComicBookDialog_DragOver);
      this.tabControl.ResumeLayout(false);
      this.tabSummary.ResumeLayout(false);
      this.cmThumbnail.ResumeLayout(false);
      this.tabDetails.ResumeLayout(false);
      this.tabDetails.PerformLayout();
      this.tabPlot.ResumeLayout(false);
      this.tabPlot.PerformLayout();
      this.tabNotes.ResumeLayout(false);
      this.tabPageSummary.ResumeLayout(false);
      this.tabPageSummary.PerformLayout();
      this.tabPageNotes.ResumeLayout(false);
      this.tabPageNotes.PerformLayout();
      this.tabPageReview.ResumeLayout(false);
      this.tabPageReview.PerformLayout();
      this.tabCatalog.ResumeLayout(false);
      this.tabCatalog.PerformLayout();
      this.tabCustom.ResumeLayout(false);
      ((ISupportInitialize) this.customValuesData).EndInit();
      this.tabPages.ResumeLayout(false);
      this.tabPages.PerformLayout();
      this.cmResetPages.ResumeLayout(false);
      this.tabColors.ResumeLayout(false);
      this.panelImage.ResumeLayout(false);
      this.panelImageControls.ResumeLayout(false);
      this.panelImageControls.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}

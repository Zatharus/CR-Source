// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.MultipleComicBooksDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Net.Search;
using cYo.Common.Reflection;
using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Controls;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class MultipleComicBooksDialog : Form
  {
    private readonly IEnumerable<ComicBook> books;
    private readonly List<TextBox> listFields = new List<TextBox>();
    private readonly List<TextBox> customFields = new List<TextBox>();
    private readonly Dictionary<Control, HashSet<string>> oldLists = new Dictionary<Control, HashSet<string>>();
    private IContainer components;
    private TextBoxEx txCount;
    private TextBoxEx txNumber;
    private TextBoxEx txVolume;
    private TextBoxEx txYear;
    private TextBoxEx txSeries;
    private TextBoxEx txTitle;
    private TextBoxEx txWriter;
    private TextBoxEx txSummary;
    private TextBoxEx txColorist;
    private TextBoxEx txInker;
    private TextBoxEx txPenciller;
    private Label labelSummary;
    private Label labelGenre;
    private Label labelColorist;
    private Label labelPublisher;
    private Label labelInker;
    private Label labelCount;
    private Label labelVolume;
    private Label labelNumber;
    private Label labelYear;
    private Label labelSeries;
    private Label labelWriter;
    private Label labelPenciller;
    private Label labelTitle;
    private Button btCancel;
    private Button btOK;
    private RatingControl txRating;
    private Label labelRating;
    private TextBoxEx txEditor;
    private TextBoxEx txCoverArtist;
    private TextBoxEx txLetterer;
    private Label labelEditor;
    private Label labelCoverArtist;
    private Label labelLetterer;
    private ComboBox cbPublisher;
    private Label labelAlternateSeries;
    private Label labelAlternateNumber;
    private Label labelAlternateCount;
    private TextBoxEx txAlternateSeries;
    private TextBoxEx txAlternateNumber;
    private TextBoxEx txAlternateCount;
    private Label labelMonth;
    private TextBoxEx txMonth;
    private Label labelTags;
    private TextBoxEx txTags;
    private ComboBox cbImprint;
    private Label labelImprint;
    private LanguageComboBox cbLanguage;
    private Label labelLanguage;
    private ComboBox cbManga;
    private Label labelManga;
    private ComboBox cbBlackAndWhite;
    private Label labelBlackAndWhite;
    private ComboBoxEx cbFormat;
    private Label labelFormat;
    private ComboBox cbEnableProposed;
    private Label labelEnableProposed;
    private ComboBoxEx cbAgeRating;
    private Label labelAgeRating;
    private TextBoxEx txNotes;
    private Label labelNotes;
    private TextBoxEx txCharacters;
    private Label labelCharacters;
    private TextBoxEx txGenre;
    private TextBoxEx txWeb;
    private Label labelWeb;
    private TextBoxEx txTeams;
    private Label labelTeams;
    private TextBoxEx txLocations;
    private Label labelLocations;
    private RatingControl txCommunityRating;
    private Label labelCommunityRating;
    private Panel pageData;
    private CollapsibleGroupBox grpCatalog;
    private CollapsibleGroupBox grpArtists;
    private CollapsibleGroupBox grpPlotNotes;
    private CollapsibleGroupBox grpMain;
    private ComboBox cbBookLocation;
    private Label labelBookLocation;
    private TextBoxEx txCollectionStatus;
    private ComboBox cbBookPrice;
    private Label labelBookPrice;
    private ComboBox cbBookAge;
    private Label labelBookAge;
    private Label labelBookCollectionStatus;
    private ComboBox cbBookCondition;
    private Label labelBookCondition;
    private ComboBoxEx cbBookStore;
    private Label labelBookStore;
    private ComboBox cbBookOwner;
    private Label labelBookOwner;
    private TextBoxEx txBookNotes;
    private Label labelBookNotes;
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
    private TextBoxEx txScanInformation;
    private Label labelScanInformation;
    private Label labelSeriesGroup;
    private TextBoxEx txSeriesGroup;
    private Label labelStoryArc;
    private TextBoxEx txStoryArc;
    private Label labelMainCharacterOrTeam;
    private TextBoxEx txMainCharacterOrTeam;
    private Label labelReview;
    private TextBoxEx txReview;
    private Label labelDay;
    private TextBoxEx txDay;
    private Label labelReleasedTime;
    private NullableDateTimePicker dtpReleasedTime;
    private CollapsibleGroupBox grpCustom;
    private TextBoxEx textCustomField;
    private Label labelCustomField;

    public MultipleComicBooksDialog(IEnumerable<ComicBook> books)
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.RestorePosition();
      this.RestorePanelStates();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      EditControlUtility.InitializeMangaYesNo(this.cbManga);
      EditControlUtility.InitializeYesNo(this.cbBlackAndWhite);
      EditControlUtility.InitializeYesNo(this.cbEnableProposed);
      EditControlUtility.InitializeYesNo(this.cbSeriesComplete);
      ComboBoxSkinner comboBoxSkinner1 = new ComboBoxSkinner(this.cbImprint);
      ComboBoxSkinner comboBoxSkinner2 = new ComboBoxSkinner(this.cbPublisher, (IImagePackage) ComicBook.PublisherIcons);
      ComboBoxSkinner comboBoxSkinner3 = new ComboBoxSkinner(this.cbImprint, (IImagePackage) ComicBook.PublisherIcons);
      ComboBoxSkinner comboBoxSkinner4 = new ComboBoxSkinner((ComboBox) this.cbAgeRating, (IImagePackage) ComicBook.AgeRatingIcons);
      ComboBoxSkinner comboBoxSkinner5 = new ComboBoxSkinner((ComboBox) this.cbFormat, (IImagePackage) ComicBook.FormatIcons);
      ComboBoxSkinner comboBoxSkinner6 = new ComboBoxSkinner((ComboBox) this.cbBookStore);
      ComboBoxSkinner comboBoxSkinner7 = new ComboBoxSkinner(this.cbBookCondition);
      ComboBoxSkinner comboBoxSkinner8 = new ComboBoxSkinner(this.cbBookLocation);
      ComboBoxSkinner comboBoxSkinner9 = new ComboBoxSkinner(this.cbBookOwner);
      ComboBoxSkinner comboBoxSkinner10 = new ComboBoxSkinner(this.cbBookPrice);
      this.listFields.AddRange((IEnumerable<TextBox>) new TextBoxEx[13]
      {
        this.txWriter,
        this.txPenciller,
        this.txInker,
        this.txColorist,
        this.txEditor,
        this.txCoverArtist,
        this.txLetterer,
        this.txGenre,
        this.txTags,
        this.txCharacters,
        this.txTeams,
        this.txLocations,
        this.txCollectionStatus
      });
      ListSelectorControl.Register((IEnumerable<INetSearch>) SearchEngines.Engines, this.listFields.ToArray());
      this.cbLanguage.TopTwoLetterISOLanguages = Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.LanguageISO)).Cast<string>().Distinct<string>();
      this.books = (IEnumerable<ComicBook>) books.ToArray<ComicBook>();
      this.Text = StringUtility.Format(this.Text, (object) books.Count<ComicBook>());
      Label labelOpenedTime = this.labelOpenedTime;
      NullableDateTimePicker dtpOpenedTime1 = this.dtpOpenedTime;
      NullableDateTimePicker dtpOpenedTime2 = this.dtpOpenedTime;
      Label pagesAsTextSimple1 = this.labelPagesAsTextSimple;
      TextBoxEx pagesAsTextSimple2 = this.txPagesAsTextSimple;
      TextBoxEx pagesAsTextSimple3 = this.txPagesAsTextSimple;
      IEnumerable<ComicBook> source1 = books;
      int num1;
      bool flag1 = (num1 = !source1.Any<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsLinked)) ? 1 : 0) != 0;
      pagesAsTextSimple3.Enabled = num1 != 0;
      int num2;
      bool flag2 = (num2 = flag1 ? 1 : 0) != 0;
      pagesAsTextSimple2.Visible = num2 != 0;
      int num3;
      bool flag3 = (num3 = flag2 ? 1 : 0) != 0;
      pagesAsTextSimple1.Visible = num3 != 0;
      int num4;
      bool flag4 = (num4 = flag3 ? 1 : 0) != 0;
      dtpOpenedTime2.Enabled = num4 != 0;
      int num5;
      bool flag5 = (num5 = flag4 ? 1 : 0) != 0;
      dtpOpenedTime1.Visible = num5 != 0;
      int num6 = flag5 ? 1 : 0;
      labelOpenedTime.Visible = num6 != 0;
      RatingControl txCommunityRating = this.txCommunityRating;
      RatingControl txRating = this.txRating;
      TextBoxEx txTags = this.txTags;
      ComboBox cbEnableProposed = this.cbEnableProposed;
      ComboBox cbSeriesComplete = this.cbSeriesComplete;
      IEnumerable<ComicBook> source2 = books;
      int num7;
      bool flag6 = (num7 = source2.All<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsInContainer)) ? 1 : 0) != 0;
      cbSeriesComplete.Enabled = num7 != 0;
      int num8;
      bool flag7 = (num8 = flag6 ? 1 : 0) != 0;
      cbEnableProposed.Enabled = num8 != 0;
      int num9;
      bool flag8 = (num9 = flag7 ? 1 : 0) != 0;
      txTags.Enabled = num9 != 0;
      int num10;
      bool flag9 = (num10 = flag8 ? 1 : 0) != 0;
      txRating.Enabled = num10 != 0;
      int num11 = flag9 ? 1 : 0;
      txCommunityRating.Enabled = num11 != 0;
      SpinButton.AddUpDown((TextBoxBase) this.txVolume);
      SpinButton.AddUpDown((TextBoxBase) this.txCount, min: 0);
      SpinButton.AddUpDown((TextBoxBase) this.txNumber);
      SpinButton.AddUpDown((TextBoxBase) this.txYear, DateTime.Now.Year);
      SpinButton.AddUpDown((TextBoxBase) this.txMonth, DateTime.Now.Month, 1, 12);
      SpinButton.AddUpDown((TextBoxBase) this.txDay, DateTime.Now.Day, 1, 31);
      SpinButton.AddUpDown((TextBoxBase) this.txAlternateCount, min: 0);
      SpinButton.AddUpDown((TextBoxBase) this.txAlternateNumber);
      SpinButton.AddUpDown((TextBoxBase) this.txPagesAsTextSimple, min: 1);
      this.txVolume.EnableOnlyNumberKeys();
      this.txCount.EnableOnlyNumberKeys();
      this.txYear.EnableOnlyNumberKeys();
      this.txMonth.EnableOnlyNumberKeys();
      this.txAlternateCount.EnableOnlyNumberKeys();
      this.txPagesAsTextSimple.EnableOnlyNumberKeys();
      string[] array = Program.Database.CustomValues.Where<string>((Func<string, bool>) (k => Program.ExtendedSettings.ShowCustomScriptValues || !k.Contains<char>('.'))).ToArray<string>();
      if (!Program.Settings.ShowCustomBookFields || array.Length == 0)
        this.grpCustom.Visible = false;
      else
        this.AddCustomFields(array);
      TextBoxContextMenu.Register((Form) this, TextBoxContextMenu.AddSearchLinks((IEnumerable<INetSearch>) SearchEngines.Engines));
      FormUtility.RegisterPanelToTabToggle((Control) this.pageData, PropertyCaller.CreateFlagsValueStore<Settings, TabLayouts>(Program.Settings, "TabLayouts", TabLayouts.Multiple));
    }

    private void AddCustomFields(string[] customValues)
    {
      Label controlToClone1 = (Label) null;
      TextBox controlToClone2 = (TextBox) null;
      foreach (string customValue in customValues)
      {
        Label label;
        TextBox textBox;
        if (controlToClone1 == null)
        {
          label = this.labelCustomField;
          textBox = (TextBox) this.textCustomField;
        }
        else
        {
          label = (Label) controlToClone1.Clone();
          textBox = (TextBox) controlToClone2.Clone();
          this.grpCustom.Controls.Add((Control) label);
          this.grpCustom.Controls.Add((Control) textBox);
          label.Visible = true;
          label.Top = controlToClone2.Bottom + 8;
          textBox.Visible = true;
          textBox.Top = label.Bottom + 4;
          Trace.WriteLine("Textbox: " + (object) textBox.Bounds);
        }
        label.Text = customValue + ":";
        textBox.Tag = (object) customValue;
        this.customFields.Add(textBox);
        controlToClone1 = label;
        controlToClone2 = textBox;
      }
      this.grpCustom.Height = controlToClone2.Bottom + 8;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.Init();
    }

    private void RefreshBooksInfoFromFiles()
    {
      int num1 = 0;
      int num2 = this.books.Count<ComicBook>();
      foreach (ComicBook book in this.books)
      {
        book.RefreshInfoFromFile(RefreshInfoOptions.None);
        AutomaticProgressDialog.Value = num1++ * 100 / num2;
      }
    }

    private void Init()
    {
      AutomaticProgressDialog.Process((Form) this, TR.Messages["RefreshInfo", "Refreshing Information"], TR.Messages["RefreshInfoText", "Refreshing information for selected Books"], 1000, new Action(this.RefreshBooksInfoFromFiles), AutomaticProgressDialogOptions.None);
      bool flag = this.books.Any<ComicBook>((Func<ComicBook, bool>) (b => b.IsLinked));
      this.SetText((TextBox) this.txSeries, "Series", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ShadowSeries))));
      this.SetText((Control) this.txNumber, "Number");
      this.SetText((Control) this.txCount, "Count");
      this.SetText((Control) this.txYear, "Year");
      this.SetText((Control) this.txVolume, "Volume");
      this.SetText((Control) this.cbSeriesComplete, "SeriesComplete");
      this.SetText((TextBox) this.txTitle, "Title", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Title))));
      this.SetText((TextBox) this.txAlternateSeries, "AlternateSeries", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.AlternateSeries))));
      this.SetText((Control) this.txAlternateNumber, "AlternateNumber");
      this.SetText((Control) this.txAlternateCount, "AlternateCount");
      this.SetText((TextBox) this.txSeriesGroup, "SeriesGroup", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.SeriesGroup))));
      this.SetText((TextBox) this.txStoryArc, "StoryArc", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.StoryArc))));
      this.SetText((Control) this.txMonth, "Month");
      this.SetText((Control) this.txDay, "Day");
      this.SetText((TextBox) this.txWriter, "Writer", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Writer))));
      this.SetText((TextBox) this.txPenciller, "Penciller", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Penciller))));
      this.SetText((TextBox) this.txColorist, "Colorist", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Colorist))));
      this.SetText((TextBox) this.txInker, "Inker", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Inker))));
      this.SetText((TextBox) this.txLetterer, "Letterer", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Letterer))));
      this.SetText((TextBox) this.txCoverArtist, "CoverArtist", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.CoverArtist))));
      this.SetText((TextBox) this.txEditor, "Editor", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Editor))));
      this.SetText(this.cbPublisher, "Publisher", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Publisher), true)));
      this.SetText(this.cbImprint, "Imprint", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Imprint), true)));
      this.SetText((TextBox) this.txGenre, "Genre", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetGenreList(false)));
      this.SetText((ComboBox) this.cbFormat, "Format", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetFormatList()));
      this.SetText((ComboBox) this.cbAgeRating, "AgeRating", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetAgeRatingList()));
      this.SetText((Control) this.txRating, "Rating");
      this.SetText((Control) this.txCommunityRating, "CommunityRating");
      this.SetText((Control) this.cbLanguage, "LanguageISO");
      this.SetText((Control) this.cbManga, "Manga");
      this.SetText((Control) this.cbBlackAndWhite, "BlackAndWhite");
      this.SetText((Control) this.cbEnableProposed, "EnableProposed");
      this.SetText((Control) this.txSummary, "Summary");
      this.SetText((Control) this.txNotes, "Notes");
      this.SetText((Control) this.txReview, "Review");
      this.SetText((Control) this.txWeb, "Web");
      this.SetText((TextBox) this.txTags, "Tags", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Tags))));
      this.SetText((TextBox) this.txCharacters, "Characters", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Characters))));
      this.SetText((TextBox) this.txTeams, "Teams", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Teams))));
      this.SetText((TextBox) this.txMainCharacterOrTeam, "MainCharacterOrTeam", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.MainCharacterOrTeam))));
      this.SetText((TextBox) this.txLocations, "Locations", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Locations))));
      this.SetText((TextBox) this.txScanInformation, "ScanInformation", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ScanInformation))));
      this.SetGrayText((IPromptText) this.txSeries, "ProposedSeries");
      this.SetGrayText((IPromptText) this.txNumber, "ProposedNumber");
      this.SetGrayText((IPromptText) this.txCount, "ProposedCountAsText");
      this.SetGrayText((IPromptText) this.txYear, "ProposedYearAsText");
      this.SetGrayText((IPromptText) this.txVolume, "ProposedNakedVolumeAsText");
      this.SetGrayText((IPromptText) this.cbFormat, "ProposedFormat");
      this.grpCatalog.Visible = !flag || !Program.Settings.CatalogOnlyForFileless;
      this.labelScanInformation.Visible = this.txScanInformation.Visible = flag;
      this.SetText((ComboBox) this.cbBookStore, "BookStore", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookStore), true)));
      this.SetText(this.cbBookOwner, "BookOwner", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookOwner), true)));
      this.SetText(this.cbBookLocation, "BookLocation", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookLocation), true)));
      this.SetText(this.cbBookPrice, "BookPriceAsText", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookPriceAsText), true)));
      this.SetText(this.cbBookAge, "BookAge", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookAgeList()));
      this.SetText(this.cbBookCondition, "BookCondition", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookConditionList()));
      this.SetText((TextBox) this.txCollectionStatus, "BookCollectionStatus", (Func<AutoCompleteStringCollection>) (() => Program.Lists.GetBookCollectionStatusList()));
      this.SetText((Control) this.txISBN, "ISBN");
      this.SetText((Control) this.txBookNotes, "BookNotes");
      if (this.txPagesAsTextSimple.Enabled)
      {
        this.SetText((Control) this.txPagesAsTextSimple, "PagesAsTextSimple");
        this.SetText((Control) this.dtpOpenedTime, "OpenedTime");
      }
      this.SetText((Control) this.dtpAddedTime, "AddedTime");
      this.SetText((Control) this.dtpReleasedTime, "ReleasedTime");
      foreach (TextBox customField in this.customFields)
      {
        string key = customField.Tag.ToString();
        this.SetText(customField, "{" + key + "}", (Func<AutoCompleteStringCollection>) (() =>
        {
          AutoCompleteStringCollection stringCollection = new AutoCompleteStringCollection();
          stringCollection.AddRange(Program.Database.GetBooks().SelectMany<ComicBook, StringPair>((Func<ComicBook, IEnumerable<StringPair>>) (cb => cb.GetCustomValues())).Where<StringPair>((Func<StringPair, bool>) (p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase))).Select<StringPair, string>((Func<StringPair, string>) (p => p.Value)).ToArray<string>());
          return stringCollection;
        }));
      }
    }

    private void Store()
    {
      List<MultipleComicBooksDialog.StoreInfo> storeInfoList = new List<MultipleComicBooksDialog.StoreInfo>();
      foreach (CheckBox control in this.GetControls<CheckBox>())
      {
        if (control.CheckState != CheckState.Unchecked)
        {
          Control tag1 = control.Tag as Control;
          if (tag1.Enabled)
          {
            string text = tag1.Text;
            string tag2 = tag1.Tag as string;
            PropertyInfo property = tag2.StartsWith("{") ? (PropertyInfo) null : typeof (ComicBook).GetProperty(tag2);
            MultipleComicBooksDialog.StoreInfo storeInfo = new MultipleComicBooksDialog.StoreInfo()
            {
              PropertyName = tag2,
              PropertyInfo = property,
              ListMode = control.CheckState == CheckState.Indeterminate
            };
            if (property == (PropertyInfo) null)
              storeInfo.Value = (object) text;
            else if (property.PropertyType == typeof (bool))
              storeInfo.Value = (object) (string.Compare(text, ComicInfo.YesText, true) == 0);
            else if (property.PropertyType == typeof (YesNo))
              storeInfo.Value = (object) EditControlUtility.GetYesNo(text);
            else if (property.PropertyType == typeof (MangaYesNo))
              storeInfo.Value = (object) EditControlUtility.GetMangaYesNo(text);
            else if (property.PropertyType == typeof (int))
            {
              int result;
              if (!int.TryParse(text, out result) || result < 0)
                result = property.DefaultValue<int>(-1);
              storeInfo.Value = (object) result;
            }
            else if (property.PropertyType == typeof (float))
            {
              float result;
              if (!float.TryParse(text, out result))
                result = property.DefaultValue<float>();
              storeInfo.Value = (object) result;
            }
            else if (property.PropertyType == typeof (DateTime))
            {
              storeInfo.Value = (object) ((NullableDateTimePicker) tag1).Value;
            }
            else
            {
              storeInfo.Value = (object) text;
              if (storeInfo.ListMode)
              {
                storeInfo.NewList = text.ListStringToSet(',');
                storeInfo.OldList = this.oldLists[tag1];
                if (object.Equals((object) storeInfo.OldList, (object) storeInfo.NewList))
                  storeInfo = (MultipleComicBooksDialog.StoreInfo) null;
              }
            }
            if (storeInfo != null)
              storeInfoList.Add(storeInfo);
          }
        }
      }
      if (storeInfoList.Count == 0)
        return;
      int num1 = 0;
      int num2 = this.books.Count<ComicBook>();
      foreach (ComicBook book in this.books)
      {
        foreach (MultipleComicBooksDialog.StoreInfo storeInfo in storeInfoList)
        {
          if (storeInfo.PropertyInfo == (PropertyInfo) null)
            book.SetCustomValue(storeInfo.PropertyName.Substring(1, storeInfo.PropertyName.Length - 2), storeInfo.Value.ToString());
          else if (!storeInfo.ListMode)
          {
            storeInfo.PropertyInfo.SetValue((object) book, storeInfo.Value, (object[]) null);
          }
          else
          {
            HashSet<string> set = book.GetStringPropertyValue(storeInfo.PropertyName).ListStringToSet(',');
            set.RemoveRange<string>((IEnumerable<string>) storeInfo.OldList);
            set.AddRange<string>((IEnumerable<string>) storeInfo.NewList);
            storeInfo.PropertyInfo.SetValue((object) book, (object) set.ToListString(", "), (object[]) null);
          }
        }
        AutomaticProgressDialog.Value = num1++ * 100 / num2;
      }
    }

    private string GetSameValue(string propName)
    {
      string sameValue = (string) null;
      foreach (ComicBook book in this.books)
      {
        string stringPropertyValue = book.GetStringPropertyValue(propName);
        if (sameValue == null)
          sameValue = stringPropertyValue;
        else if (sameValue != stringPropertyValue)
          return string.Empty;
      }
      return sameValue;
    }

    private CheckBox MakeCheckBox(Control c)
    {
      CheckBox checkBox = new CheckBox();
      c.Parent.Controls.Add((Control) checkBox);
      checkBox.AutoSize = true;
      checkBox.Visible = true;
      checkBox.TabStop = false;
      checkBox.Name = "chk" + c.Name;
      checkBox.Enabled = c.Enabled;
      checkBox.Left = c.Left;
      checkBox.Top = c.Top + (c.Height - checkBox.Height) / 2;
      c.Left += checkBox.Width + 2;
      c.Width -= checkBox.Width + 2;
      if (this.Controls[c.Name.Replace("tx", "label").Replace("cb", "label")] is Label control)
        control.Left = c.Left;
      return checkBox;
    }

    private void SetGrayText(IPromptText tx, string property)
    {
      string s = "chk" + (tx as Control).Name;
      CheckBox chk = this.GetControls<CheckBox>().FirstOrDefault<CheckBox>((Func<CheckBox, bool>) (cb => cb.Name == s));
      if (chk == null)
        return;
      tx.PromptText = this.GetSameValue(property);
      chk.CheckedChanged += (EventHandler) ((sender, e) =>
      {
        if (chk.Checked)
        {
          if (!string.IsNullOrEmpty(tx.Text))
            return;
          tx.Text = tx.PromptText;
        }
        else
        {
          if (!(tx.Text == tx.PromptText))
            return;
          tx.Text = string.Empty;
        }
      });
    }

    private void SetText(Control c, string propName)
    {
      CheckBox chk = this.MakeCheckBox(c);
      c.Tag = (object) propName;
      chk.Tag = (object) c;
      if (this.listFields.Contains(c as TextBox))
      {
        if (this.SetListText(c as TextBox, propName))
        {
          chk.CheckState = CheckState.Indeterminate;
          chk.CheckedChanged += (EventHandler) ((a, b) =>
          {
            if (chk.Checked)
              return;
            c.Text = string.Empty;
          });
        }
        else
          chk.Checked = true;
      }
      else
        this.SetSimpleText(c, chk, propName);
      c.TextChanged += (EventHandler) ((a, b) =>
      {
        if (string.IsNullOrEmpty(c.Text))
          return;
        chk.Checked = true;
      });
    }

    private bool SetListText(TextBox c, string propName)
    {
      HashSet<string> stringSet = (HashSet<string>) null;
      string str = (string) null;
      bool flag = true;
      foreach (ComicBook book in this.books)
      {
        string stringPropertyValue = book.GetStringPropertyValue(propName);
        if (str == null)
          str = stringPropertyValue;
        else
          flag &= str == stringPropertyValue;
        HashSet<string> set = stringPropertyValue.ListStringToSet(',');
        stringSet = stringSet != null ? new HashSet<string>(stringSet.Intersect<string>((IEnumerable<string>) set)) : set;
      }
      c.Text = stringSet.ToListString(", ");
      this.oldLists[(Control) c] = stringSet;
      return !flag;
    }

    private void SetSimpleText(Control c, CheckBox chk, string propName)
    {
      bool flag1 = propName.StartsWith("{");
      bool flag2 = true;
      object objB = (object) null;
      PropertyInfo property = flag1 ? (PropertyInfo) null : typeof (ComicBook).GetProperty(propName);
      foreach (ComicBook book in this.books)
      {
        object objA = property == (PropertyInfo) null ? (object) book.GetStringPropertyValue(propName) : property.GetValue((object) book, (object[]) null);
        if (objB == null)
          objB = objA;
        else if (!object.Equals(objA, objB))
        {
          flag2 = false;
          break;
        }
      }
      if (flag2)
      {
        if (chk != null)
          chk.Checked = true;
        try
        {
          switch (objB)
          {
            case bool flag3:
              EditControlUtility.SetText(c, flag3);
              break;
            case YesNo yn1:
              EditControlUtility.SetText(c, yn1);
              break;
            case MangaYesNo yn2:
              EditControlUtility.SetText(c, yn2);
              break;
            case int num:
              if (num == -1)
                break;
              c.Text = num.ToString();
              break;
            case DateTime dateTime:
              ((NullableDateTimePicker) c).Value = dateTime;
              break;
            default:
              c.Text = objB.ToString();
              break;
          }
        }
        catch (Exception ex)
        {
        }
      }
      else if (c is NullableDateTimePicker)
        ((NullableDateTimePicker) c).Value = DateTime.MinValue;
      else
        c.Text = string.Empty;
    }

    private void SetText(
      TextBox textBox,
      string propName,
      Func<AutoCompleteStringCollection> autoCompletePredicate)
    {
      this.SetText((Control) textBox, propName);
      if (textBox is IDelayedAutoCompleteList autoCompleteList)
      {
        if (autoCompletePredicate != null)
          autoCompleteList.SetLazyAutoComplete(autoCompletePredicate);
        else
          autoCompleteList.ResetLazyAutoComplete();
      }
      else
      {
        textBox.AutoCompleteCustomSource = autoCompletePredicate();
        textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        textBox.AutoCompleteMode = AutoCompleteMode.Append;
      }
    }

    private void SetText(
      ComboBox c,
      string propName,
      Func<AutoCompleteStringCollection> autoCompletePredicate)
    {
      if (autoCompletePredicate != null)
      {
        Func<IEnumerable<string>> allItems = (Func<IEnumerable<string>>) (() =>
        {
          HashSet<string> list = new HashSet<string>()
          {
            string.Empty
          };
          list.AddRange<string>(autoCompletePredicate().Cast<string>());
          return (IEnumerable<string>) list;
        });
        if (c.DataSource == null)
          c.Enter += (EventHandler) ((s, e) =>
          {
            string text = c.Text;
            c.DataSource = (object) ComboBoxSkinner.AutoSeparatorList((IEnumerable) allItems());
            c.Text = text;
          });
        else
          c.DataSource = (object) ComboBoxSkinner.AutoSeparatorList((IEnumerable) allItems());
      }
      this.SetText((Control) c, propName);
    }

    private void btOK_Click(object sender, EventArgs e) => this.Store();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.txCount = new TextBoxEx();
      this.txNumber = new TextBoxEx();
      this.txVolume = new TextBoxEx();
      this.txYear = new TextBoxEx();
      this.txSeries = new TextBoxEx();
      this.txTitle = new TextBoxEx();
      this.txWriter = new TextBoxEx();
      this.txSummary = new TextBoxEx();
      this.txColorist = new TextBoxEx();
      this.txInker = new TextBoxEx();
      this.txPenciller = new TextBoxEx();
      this.labelSummary = new Label();
      this.labelGenre = new Label();
      this.labelColorist = new Label();
      this.labelPublisher = new Label();
      this.labelInker = new Label();
      this.labelCount = new Label();
      this.labelVolume = new Label();
      this.labelNumber = new Label();
      this.labelYear = new Label();
      this.labelSeries = new Label();
      this.labelWriter = new Label();
      this.labelPenciller = new Label();
      this.labelTitle = new Label();
      this.btCancel = new Button();
      this.btOK = new Button();
      this.txRating = new RatingControl();
      this.labelRating = new Label();
      this.txEditor = new TextBoxEx();
      this.txCoverArtist = new TextBoxEx();
      this.txLetterer = new TextBoxEx();
      this.labelEditor = new Label();
      this.labelCoverArtist = new Label();
      this.labelLetterer = new Label();
      this.cbPublisher = new ComboBox();
      this.labelAlternateSeries = new Label();
      this.labelAlternateNumber = new Label();
      this.labelAlternateCount = new Label();
      this.txAlternateSeries = new TextBoxEx();
      this.txAlternateNumber = new TextBoxEx();
      this.txAlternateCount = new TextBoxEx();
      this.labelMonth = new Label();
      this.txMonth = new TextBoxEx();
      this.labelTags = new Label();
      this.txTags = new TextBoxEx();
      this.cbImprint = new ComboBox();
      this.labelImprint = new Label();
      this.cbLanguage = new LanguageComboBox();
      this.labelLanguage = new Label();
      this.cbManga = new ComboBox();
      this.labelManga = new Label();
      this.cbBlackAndWhite = new ComboBox();
      this.labelBlackAndWhite = new Label();
      this.cbFormat = new ComboBoxEx();
      this.labelFormat = new Label();
      this.cbEnableProposed = new ComboBox();
      this.labelEnableProposed = new Label();
      this.cbAgeRating = new ComboBoxEx();
      this.labelAgeRating = new Label();
      this.txNotes = new TextBoxEx();
      this.labelNotes = new Label();
      this.txCharacters = new TextBoxEx();
      this.labelCharacters = new Label();
      this.txGenre = new TextBoxEx();
      this.txWeb = new TextBoxEx();
      this.labelWeb = new Label();
      this.txTeams = new TextBoxEx();
      this.labelTeams = new Label();
      this.txLocations = new TextBoxEx();
      this.labelLocations = new Label();
      this.txCommunityRating = new RatingControl();
      this.labelCommunityRating = new Label();
      this.pageData = new Panel();
      this.grpCustom = new CollapsibleGroupBox();
      this.textCustomField = new TextBoxEx();
      this.labelCustomField = new Label();
      this.grpCatalog = new CollapsibleGroupBox();
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
      this.txBookNotes = new TextBoxEx();
      this.labelBookNotes = new Label();
      this.cbBookLocation = new ComboBox();
      this.labelBookLocation = new Label();
      this.txCollectionStatus = new TextBoxEx();
      this.cbBookPrice = new ComboBox();
      this.labelBookPrice = new Label();
      this.cbBookAge = new ComboBox();
      this.labelBookAge = new Label();
      this.labelBookCollectionStatus = new Label();
      this.cbBookCondition = new ComboBox();
      this.labelBookCondition = new Label();
      this.cbBookStore = new ComboBoxEx();
      this.labelBookStore = new Label();
      this.cbBookOwner = new ComboBox();
      this.labelBookOwner = new Label();
      this.grpPlotNotes = new CollapsibleGroupBox();
      this.labelMainCharacterOrTeam = new Label();
      this.txMainCharacterOrTeam = new TextBoxEx();
      this.labelReview = new Label();
      this.txReview = new TextBoxEx();
      this.txScanInformation = new TextBoxEx();
      this.labelScanInformation = new Label();
      this.grpArtists = new CollapsibleGroupBox();
      this.grpMain = new CollapsibleGroupBox();
      this.labelDay = new Label();
      this.txDay = new TextBoxEx();
      this.labelSeriesGroup = new Label();
      this.txSeriesGroup = new TextBoxEx();
      this.labelStoryArc = new Label();
      this.txStoryArc = new TextBoxEx();
      this.cbSeriesComplete = new ComboBox();
      this.labelSeriesComplete = new Label();
      this.pageData.SuspendLayout();
      this.grpCustom.SuspendLayout();
      this.grpCatalog.SuspendLayout();
      this.grpPlotNotes.SuspendLayout();
      this.grpArtists.SuspendLayout();
      this.grpMain.SuspendLayout();
      this.SuspendLayout();
      this.txCount.Location = new System.Drawing.Point(395, 51);
      this.txCount.Name = "txCount";
      this.txCount.PromptText = "";
      this.txCount.Size = new System.Drawing.Size(72, 20);
      this.txCount.TabIndex = 7;
      this.txNumber.Location = new System.Drawing.Point(318, 50);
      this.txNumber.Name = "txNumber";
      this.txNumber.PromptText = "";
      this.txNumber.Size = new System.Drawing.Size(71, 20);
      this.txNumber.TabIndex = 5;
      this.txVolume.Location = new System.Drawing.Point(243, 50);
      this.txVolume.Name = "txVolume";
      this.txVolume.PromptText = "";
      this.txVolume.Size = new System.Drawing.Size(66, 20);
      this.txVolume.TabIndex = 3;
      this.txYear.Location = new System.Drawing.Point(243, 87);
      this.txYear.MaxLength = 4;
      this.txYear.Name = "txYear";
      this.txYear.PromptText = "";
      this.txYear.Size = new System.Drawing.Size(66, 20);
      this.txYear.TabIndex = 11;
      this.txSeries.Location = new System.Drawing.Point(11, 50);
      this.txSeries.Name = "txSeries";
      this.txSeries.PromptText = "";
      this.txSeries.Size = new System.Drawing.Size(223, 20);
      this.txSeries.TabIndex = 1;
      this.txTitle.Location = new System.Drawing.Point(11, 87);
      this.txTitle.Name = "txTitle";
      this.txTitle.PromptText = "";
      this.txTitle.Size = new System.Drawing.Size(226, 20);
      this.txTitle.TabIndex = 9;
      this.txWriter.Location = new System.Drawing.Point(13, 57);
      this.txWriter.Name = "txWriter";
      this.txWriter.Size = new System.Drawing.Size(220, 20);
      this.txWriter.TabIndex = 1;
      this.txSummary.Location = new System.Drawing.Point(13, 48);
      this.txSummary.Multiline = true;
      this.txSummary.Name = "txSummary";
      this.txSummary.Size = new System.Drawing.Size(220, 55);
      this.txSummary.TabIndex = 1;
      this.txColorist.Location = new System.Drawing.Point(245, 93);
      this.txColorist.Name = "txColorist";
      this.txColorist.Size = new System.Drawing.Size(220, 20);
      this.txColorist.TabIndex = 11;
      this.txInker.Location = new System.Drawing.Point(13, 93);
      this.txInker.Name = "txInker";
      this.txInker.Size = new System.Drawing.Size(220, 20);
      this.txInker.TabIndex = 3;
      this.txPenciller.Location = new System.Drawing.Point(245, 57);
      this.txPenciller.Name = "txPenciller";
      this.txPenciller.Size = new System.Drawing.Size(220, 20);
      this.txPenciller.TabIndex = 9;
      this.labelSummary.AutoSize = true;
      this.labelSummary.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelSummary.Location = new System.Drawing.Point(15, 33);
      this.labelSummary.Name = "labelSummary";
      this.labelSummary.Size = new System.Drawing.Size(56, 12);
      this.labelSummary.TabIndex = 0;
      this.labelSummary.Text = "Summary:";
      this.labelGenre.AutoSize = true;
      this.labelGenre.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelGenre.Location = new System.Drawing.Point(163, 270);
      this.labelGenre.Name = "labelGenre";
      this.labelGenre.Size = new System.Drawing.Size(39, 12);
      this.labelGenre.TabIndex = 42;
      this.labelGenre.Text = "Genre:";
      this.labelColorist.AutoSize = true;
      this.labelColorist.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelColorist.Location = new System.Drawing.Point(243, 80);
      this.labelColorist.Name = "labelColorist";
      this.labelColorist.Size = new System.Drawing.Size(49, 12);
      this.labelColorist.TabIndex = 10;
      this.labelColorist.Text = "Colorist:";
      this.labelPublisher.AutoSize = true;
      this.labelPublisher.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelPublisher.Location = new System.Drawing.Point(9, 236);
      this.labelPublisher.Name = "labelPublisher";
      this.labelPublisher.Size = new System.Drawing.Size(56, 12);
      this.labelPublisher.TabIndex = 34;
      this.labelPublisher.Text = "Publisher:";
      this.labelInker.AutoSize = true;
      this.labelInker.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelInker.Location = new System.Drawing.Point(13, 80);
      this.labelInker.Name = "labelInker";
      this.labelInker.Size = new System.Drawing.Size(35, 12);
      this.labelInker.TabIndex = 2;
      this.labelInker.Text = "Inker:";
      this.labelCount.AutoSize = true;
      this.labelCount.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCount.Location = new System.Drawing.Point(396, 36);
      this.labelCount.Name = "labelCount";
      this.labelCount.Size = new System.Drawing.Size(19, 12);
      this.labelCount.TabIndex = 6;
      this.labelCount.Text = "of:";
      this.labelVolume.AutoSize = true;
      this.labelVolume.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelVolume.Location = new System.Drawing.Point(241, 36);
      this.labelVolume.Name = "labelVolume";
      this.labelVolume.Size = new System.Drawing.Size(47, 12);
      this.labelVolume.TabIndex = 2;
      this.labelVolume.Text = "Volume:";
      this.labelNumber.AutoSize = true;
      this.labelNumber.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelNumber.Location = new System.Drawing.Point(316, 36);
      this.labelNumber.Name = "labelNumber";
      this.labelNumber.Size = new System.Drawing.Size(48, 12);
      this.labelNumber.TabIndex = 4;
      this.labelNumber.Text = "Number:";
      this.labelYear.AutoSize = true;
      this.labelYear.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelYear.Location = new System.Drawing.Point(241, 74);
      this.labelYear.Name = "labelYear";
      this.labelYear.Size = new System.Drawing.Size(31, 12);
      this.labelYear.TabIndex = 10;
      this.labelYear.Text = "Year:";
      this.labelSeries.AutoSize = true;
      this.labelSeries.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelSeries.Location = new System.Drawing.Point(11, 36);
      this.labelSeries.Name = "labelSeries";
      this.labelSeries.Size = new System.Drawing.Size(41, 12);
      this.labelSeries.TabIndex = 0;
      this.labelSeries.Text = "Series:";
      this.labelWriter.AutoSize = true;
      this.labelWriter.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelWriter.Location = new System.Drawing.Point(12, 42);
      this.labelWriter.Name = "labelWriter";
      this.labelWriter.Size = new System.Drawing.Size(40, 12);
      this.labelWriter.TabIndex = 0;
      this.labelWriter.Text = "Writer:";
      this.labelPenciller.AutoSize = true;
      this.labelPenciller.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelPenciller.Location = new System.Drawing.Point(243, 42);
      this.labelPenciller.Name = "labelPenciller";
      this.labelPenciller.Size = new System.Drawing.Size(53, 12);
      this.labelPenciller.TabIndex = 8;
      this.labelPenciller.Text = "Penciller:";
      this.labelTitle.AutoSize = true;
      this.labelTitle.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelTitle.Location = new System.Drawing.Point(11, 74);
      this.labelTitle.Name = "labelTitle";
      this.labelTitle.Size = new System.Drawing.Size(31, 12);
      this.labelTitle.TabIndex = 8;
      this.labelTitle.Text = "Title:";
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(429, 427);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 71;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(346, 427);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(77, 24);
      this.btOK.TabIndex = 70;
      this.btOK.Text = "&OK";
      this.btOK.Click += new EventHandler(this.btOK_Click);
      this.txRating.DrawText = true;
      this.txRating.Font = new Font("Arial", 9f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.txRating.ForeColor = SystemColors.GrayText;
      this.txRating.Location = new System.Drawing.Point(13, 363);
      this.txRating.Name = "txRating";
      this.txRating.Rating = 3f;
      this.txRating.RatingImage = (Image) Resources.StarYellow;
      this.txRating.Size = new System.Drawing.Size(223, 20);
      this.txRating.TabIndex = 49;
      this.txRating.Text = "3";
      this.labelRating.AutoSize = true;
      this.labelRating.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelRating.Location = new System.Drawing.Point(12, 348);
      this.labelRating.Name = "labelRating";
      this.labelRating.Size = new System.Drawing.Size(61, 12);
      this.labelRating.TabIndex = 48;
      this.labelRating.Text = "My Rating:";
      this.txEditor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txEditor.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txEditor.Location = new System.Drawing.Point(13, 167);
      this.txEditor.Name = "txEditor";
      this.txEditor.Size = new System.Drawing.Size(220, 20);
      this.txEditor.TabIndex = 7;
      this.txCoverArtist.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txCoverArtist.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txCoverArtist.Location = new System.Drawing.Point(245, 130);
      this.txCoverArtist.Name = "txCoverArtist";
      this.txCoverArtist.Size = new System.Drawing.Size(220, 20);
      this.txCoverArtist.TabIndex = 13;
      this.txLetterer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txLetterer.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txLetterer.Location = new System.Drawing.Point(13, 129);
      this.txLetterer.Name = "txLetterer";
      this.txLetterer.Size = new System.Drawing.Size(220, 20);
      this.txLetterer.TabIndex = 5;
      this.labelEditor.AutoSize = true;
      this.labelEditor.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelEditor.Location = new System.Drawing.Point(13, 152);
      this.labelEditor.Name = "labelEditor";
      this.labelEditor.Size = new System.Drawing.Size(39, 12);
      this.labelEditor.TabIndex = 6;
      this.labelEditor.Text = "Editor:";
      this.labelCoverArtist.AutoSize = true;
      this.labelCoverArtist.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCoverArtist.Location = new System.Drawing.Point(243, 116);
      this.labelCoverArtist.Name = "labelCoverArtist";
      this.labelCoverArtist.Size = new System.Drawing.Size(72, 12);
      this.labelCoverArtist.TabIndex = 12;
      this.labelCoverArtist.Text = "Cover Artist:";
      this.labelLetterer.AutoSize = true;
      this.labelLetterer.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelLetterer.Location = new System.Drawing.Point(12, 116);
      this.labelLetterer.Name = "labelLetterer";
      this.labelLetterer.Size = new System.Drawing.Size(49, 12);
      this.labelLetterer.TabIndex = 4;
      this.labelLetterer.Text = "Letterer:";
      this.cbPublisher.FormattingEnabled = true;
      this.cbPublisher.Location = new System.Drawing.Point(11, 248);
      this.cbPublisher.Name = "cbPublisher";
      this.cbPublisher.Size = new System.Drawing.Size(142, 21);
      this.cbPublisher.TabIndex = 35;
      this.labelAlternateSeries.AutoSize = true;
      this.labelAlternateSeries.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAlternateSeries.Location = new System.Drawing.Point(9, 112);
      this.labelAlternateSeries.Name = "labelAlternateSeries";
      this.labelAlternateSeries.Size = new System.Drawing.Size(177, 12);
      this.labelAlternateSeries.TabIndex = 16;
      this.labelAlternateSeries.Text = "Alternate Series or Storyline Title:";
      this.labelAlternateNumber.AutoSize = true;
      this.labelAlternateNumber.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAlternateNumber.Location = new System.Drawing.Point(316, 112);
      this.labelAlternateNumber.Name = "labelAlternateNumber";
      this.labelAlternateNumber.Size = new System.Drawing.Size(48, 12);
      this.labelAlternateNumber.TabIndex = 18;
      this.labelAlternateNumber.Text = "Number:";
      this.labelAlternateCount.AutoSize = true;
      this.labelAlternateCount.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAlternateCount.Location = new System.Drawing.Point(395, 112);
      this.labelAlternateCount.Name = "labelAlternateCount";
      this.labelAlternateCount.Size = new System.Drawing.Size(19, 12);
      this.labelAlternateCount.TabIndex = 20;
      this.labelAlternateCount.Text = "of:";
      this.txAlternateSeries.Location = new System.Drawing.Point(10, (int) sbyte.MaxValue);
      this.txAlternateSeries.Name = "txAlternateSeries";
      this.txAlternateSeries.Size = new System.Drawing.Size(299, 20);
      this.txAlternateSeries.TabIndex = 17;
      this.txAlternateNumber.Location = new System.Drawing.Point(318, (int) sbyte.MaxValue);
      this.txAlternateNumber.Name = "txAlternateNumber";
      this.txAlternateNumber.Size = new System.Drawing.Size(70, 20);
      this.txAlternateNumber.TabIndex = 19;
      this.txAlternateCount.Location = new System.Drawing.Point(394, (int) sbyte.MaxValue);
      this.txAlternateCount.Name = "txAlternateCount";
      this.txAlternateCount.Size = new System.Drawing.Size(72, 20);
      this.txAlternateCount.TabIndex = 21;
      this.labelMonth.AutoSize = true;
      this.labelMonth.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelMonth.Location = new System.Drawing.Point(316, 74);
      this.labelMonth.Name = "labelMonth";
      this.labelMonth.Size = new System.Drawing.Size(41, 12);
      this.labelMonth.TabIndex = 12;
      this.labelMonth.Text = "Month:";
      this.txMonth.Location = new System.Drawing.Point(318, 87);
      this.txMonth.MaxLength = 2;
      this.txMonth.Name = "txMonth";
      this.txMonth.Size = new System.Drawing.Size(71, 20);
      this.txMonth.TabIndex = 13;
      this.labelTags.AutoSize = true;
      this.labelTags.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelTags.Location = new System.Drawing.Point(9, 309);
      this.labelTags.Name = "labelTags";
      this.labelTags.Size = new System.Drawing.Size(33, 12);
      this.labelTags.TabIndex = 46;
      this.labelTags.Text = "Tags:";
      this.txTags.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txTags.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txTags.Location = new System.Drawing.Point(11, 324);
      this.txTags.Name = "txTags";
      this.txTags.Size = new System.Drawing.Size(298, 20);
      this.txTags.TabIndex = 47;
      this.cbImprint.FormattingEnabled = true;
      this.cbImprint.Location = new System.Drawing.Point(164, 248);
      this.cbImprint.Name = "cbImprint";
      this.cbImprint.Size = new System.Drawing.Size(145, 21);
      this.cbImprint.TabIndex = 37;
      this.labelImprint.AutoSize = true;
      this.labelImprint.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelImprint.Location = new System.Drawing.Point(162, 235);
      this.labelImprint.Name = "labelImprint";
      this.labelImprint.Size = new System.Drawing.Size(45, 12);
      this.labelImprint.TabIndex = 36;
      this.labelImprint.Text = "Imprint:";
      this.cbLanguage.CultureTypes = CultureTypes.NeutralCultures;
      this.cbLanguage.DrawMode = DrawMode.OwnerDrawVariable;
      this.cbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbLanguage.FormattingEnabled = true;
      this.cbLanguage.IntegralHeight = false;
      this.cbLanguage.Location = new System.Drawing.Point(10, 285);
      this.cbLanguage.Name = "cbLanguage";
      this.cbLanguage.SelectedCulture = "";
      this.cbLanguage.SelectedTwoLetterISOLanguage = "";
      this.cbLanguage.Size = new System.Drawing.Size(143, 21);
      this.cbLanguage.TabIndex = 41;
      this.cbLanguage.TopTwoLetterISOLanguages = (IEnumerable<string>) null;
      this.labelLanguage.AutoSize = true;
      this.labelLanguage.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelLanguage.Location = new System.Drawing.Point(8, 270);
      this.labelLanguage.Name = "labelLanguage";
      this.labelLanguage.Size = new System.Drawing.Size(57, 12);
      this.labelLanguage.TabIndex = 40;
      this.labelLanguage.Text = "Language:";
      this.cbManga.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbManga.FormattingEnabled = true;
      this.cbManga.Location = new System.Drawing.Point(317, 210);
      this.cbManga.Name = "cbManga";
      this.cbManga.Size = new System.Drawing.Size(149, 21);
      this.cbManga.TabIndex = 33;
      this.labelManga.AutoSize = true;
      this.labelManga.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelManga.Location = new System.Drawing.Point(315, 198);
      this.labelManga.Name = "labelManga";
      this.labelManga.Size = new System.Drawing.Size(43, 12);
      this.labelManga.TabIndex = 32;
      this.labelManga.Text = "Manga:";
      this.cbBlackAndWhite.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbBlackAndWhite.FormattingEnabled = true;
      this.cbBlackAndWhite.Location = new System.Drawing.Point(317, 246);
      this.cbBlackAndWhite.Name = "cbBlackAndWhite";
      this.cbBlackAndWhite.Size = new System.Drawing.Size(149, 21);
      this.cbBlackAndWhite.TabIndex = 39;
      this.labelBlackAndWhite.AutoSize = true;
      this.labelBlackAndWhite.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBlackAndWhite.Location = new System.Drawing.Point(317, 231);
      this.labelBlackAndWhite.Name = "labelBlackAndWhite";
      this.labelBlackAndWhite.Size = new System.Drawing.Size(90, 12);
      this.labelBlackAndWhite.TabIndex = 38;
      this.labelBlackAndWhite.Text = "Black and White:";
      this.cbFormat.FormattingEnabled = true;
      this.cbFormat.Location = new System.Drawing.Point(10, 211);
      this.cbFormat.Name = "cbFormat";
      this.cbFormat.PromptText = (string) null;
      this.cbFormat.Size = new System.Drawing.Size(145, 21);
      this.cbFormat.TabIndex = 29;
      this.labelFormat.AutoSize = true;
      this.labelFormat.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelFormat.Location = new System.Drawing.Point(8, 197);
      this.labelFormat.Name = "labelFormat";
      this.labelFormat.Size = new System.Drawing.Size(45, 12);
      this.labelFormat.TabIndex = 28;
      this.labelFormat.Text = "Format:";
      this.cbEnableProposed.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbEnableProposed.FormattingEnabled = true;
      this.cbEnableProposed.Location = new System.Drawing.Point(317, 284);
      this.cbEnableProposed.Name = "cbEnableProposed";
      this.cbEnableProposed.Size = new System.Drawing.Size(149, 21);
      this.cbEnableProposed.TabIndex = 45;
      this.labelEnableProposed.AutoSize = true;
      this.labelEnableProposed.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelEnableProposed.Location = new System.Drawing.Point(319, 269);
      this.labelEnableProposed.Name = "labelEnableProposed";
      this.labelEnableProposed.Size = new System.Drawing.Size(94, 12);
      this.labelEnableProposed.TabIndex = 44;
      this.labelEnableProposed.Text = "Proposed Values:";
      this.cbAgeRating.FormattingEnabled = true;
      this.cbAgeRating.Location = new System.Drawing.Point(164, 211);
      this.cbAgeRating.Name = "cbAgeRating";
      this.cbAgeRating.PromptText = (string) null;
      this.cbAgeRating.Size = new System.Drawing.Size(145, 21);
      this.cbAgeRating.TabIndex = 31;
      this.labelAgeRating.AutoSize = true;
      this.labelAgeRating.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAgeRating.Location = new System.Drawing.Point(161, 196);
      this.labelAgeRating.Name = "labelAgeRating";
      this.labelAgeRating.Size = new System.Drawing.Size(65, 12);
      this.labelAgeRating.TabIndex = 30;
      this.labelAgeRating.Text = "Age Rating:";
      this.txNotes.Location = new System.Drawing.Point(13, 123);
      this.txNotes.Multiline = true;
      this.txNotes.Name = "txNotes";
      this.txNotes.Size = new System.Drawing.Size(220, 55);
      this.txNotes.TabIndex = 7;
      this.labelNotes.AutoSize = true;
      this.labelNotes.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelNotes.Location = new System.Drawing.Point(15, 109);
      this.labelNotes.Name = "labelNotes";
      this.labelNotes.Size = new System.Drawing.Size(39, 12);
      this.labelNotes.TabIndex = 6;
      this.labelNotes.Text = "Notes:";
      this.txCharacters.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txCharacters.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txCharacters.Location = new System.Drawing.Point(244, 83);
      this.txCharacters.Name = "txCharacters";
      this.txCharacters.Size = new System.Drawing.Size(223, 20);
      this.txCharacters.TabIndex = 5;
      this.labelCharacters.AutoSize = true;
      this.labelCharacters.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCharacters.Location = new System.Drawing.Point(245, 69);
      this.labelCharacters.Name = "labelCharacters";
      this.labelCharacters.Size = new System.Drawing.Size(65, 12);
      this.labelCharacters.TabIndex = 4;
      this.labelCharacters.Text = "Characters:";
      this.txGenre.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txGenre.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txGenre.Location = new System.Drawing.Point(164, 285);
      this.txGenre.Name = "txGenre";
      this.txGenre.Size = new System.Drawing.Size(145, 20);
      this.txGenre.TabIndex = 43;
      this.txWeb.Location = new System.Drawing.Point(245, 231);
      this.txWeb.Name = "txWeb";
      this.txWeb.Size = new System.Drawing.Size(221, 20);
      this.txWeb.TabIndex = 17;
      this.labelWeb.AutoSize = true;
      this.labelWeb.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelWeb.Location = new System.Drawing.Point(244, 217);
      this.labelWeb.Name = "labelWeb";
      this.labelWeb.Size = new System.Drawing.Size(31, 12);
      this.labelWeb.TabIndex = 16;
      this.labelWeb.Text = "Web:";
      this.txTeams.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txTeams.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txTeams.Location = new System.Drawing.Point(243, 123);
      this.txTeams.Name = "txTeams";
      this.txTeams.Size = new System.Drawing.Size(224, 20);
      this.txTeams.TabIndex = 9;
      this.labelTeams.AutoSize = true;
      this.labelTeams.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelTeams.Location = new System.Drawing.Point(244, 110);
      this.labelTeams.Name = "labelTeams";
      this.labelTeams.Size = new System.Drawing.Size(42, 12);
      this.labelTeams.TabIndex = 8;
      this.labelTeams.Text = "Teams:";
      this.txLocations.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txLocations.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txLocations.Location = new System.Drawing.Point(243, 159);
      this.txLocations.Name = "txLocations";
      this.txLocations.Size = new System.Drawing.Size(223, 20);
      this.txLocations.TabIndex = 11;
      this.labelLocations.AutoSize = true;
      this.labelLocations.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelLocations.Location = new System.Drawing.Point(244, 146);
      this.labelLocations.Name = "labelLocations";
      this.labelLocations.Size = new System.Drawing.Size(58, 12);
      this.labelLocations.TabIndex = 10;
      this.labelLocations.Text = "Locations:";
      this.txCommunityRating.DrawText = true;
      this.txCommunityRating.Font = new Font("Arial", 9f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.txCommunityRating.ForeColor = SystemColors.GrayText;
      this.txCommunityRating.Location = new System.Drawing.Point(248, 364);
      this.txCommunityRating.Name = "txCommunityRating";
      this.txCommunityRating.Rating = 3f;
      this.txCommunityRating.RatingImage = (Image) Resources.StarBlue;
      this.txCommunityRating.Size = new System.Drawing.Size(219, 20);
      this.txCommunityRating.TabIndex = 51;
      this.txCommunityRating.Text = "3";
      this.labelCommunityRating.AutoSize = true;
      this.labelCommunityRating.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCommunityRating.Location = new System.Drawing.Point(247, 349);
      this.labelCommunityRating.Name = "labelCommunityRating";
      this.labelCommunityRating.Size = new System.Drawing.Size(102, 12);
      this.labelCommunityRating.TabIndex = 50;
      this.labelCommunityRating.Text = "Community Rating:";
      this.pageData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pageData.AutoScroll = true;
      this.pageData.BorderStyle = BorderStyle.FixedSingle;
      this.pageData.Controls.Add((Control) this.grpCustom);
      this.pageData.Controls.Add((Control) this.grpCatalog);
      this.pageData.Controls.Add((Control) this.grpPlotNotes);
      this.pageData.Controls.Add((Control) this.grpArtists);
      this.pageData.Controls.Add((Control) this.grpMain);
      this.pageData.Location = new System.Drawing.Point(12, 12);
      this.pageData.Name = "pageData";
      this.pageData.Size = new System.Drawing.Size(497, 407);
      this.pageData.TabIndex = 72;
      this.grpCustom.Controls.Add((Control) this.textCustomField);
      this.grpCustom.Controls.Add((Control) this.labelCustomField);
      this.grpCustom.Dock = DockStyle.Top;
      this.grpCustom.Location = new System.Drawing.Point(0, 1261);
      this.grpCustom.Name = "grpCustom";
      this.grpCustom.Size = new System.Drawing.Size(478, 91);
      this.grpCustom.TabIndex = 4;
      this.grpCustom.Text = "Custom Fields";
      this.textCustomField.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.textCustomField.Location = new System.Drawing.Point(11, 53);
      this.textCustomField.Name = "textCustomField";
      this.textCustomField.Size = new System.Drawing.Size(451, 20);
      this.textCustomField.TabIndex = 25;
      this.textCustomField.Tag = (object) "";
      this.labelCustomField.AutoSize = true;
      this.labelCustomField.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelCustomField.Location = new System.Drawing.Point(10, 38);
      this.labelCustomField.Name = "labelCustomField";
      this.labelCustomField.Size = new System.Drawing.Size(76, 12);
      this.labelCustomField.TabIndex = 24;
      this.labelCustomField.Text = "Custom Field:";
      this.grpCatalog.Controls.Add((Control) this.labelReleasedTime);
      this.grpCatalog.Controls.Add((Control) this.dtpReleasedTime);
      this.grpCatalog.Controls.Add((Control) this.labelOpenedTime);
      this.grpCatalog.Controls.Add((Control) this.dtpOpenedTime);
      this.grpCatalog.Controls.Add((Control) this.labelAddedTime);
      this.grpCatalog.Controls.Add((Control) this.dtpAddedTime);
      this.grpCatalog.Controls.Add((Control) this.txPagesAsTextSimple);
      this.grpCatalog.Controls.Add((Control) this.labelPagesAsTextSimple);
      this.grpCatalog.Controls.Add((Control) this.txISBN);
      this.grpCatalog.Controls.Add((Control) this.labelISBN);
      this.grpCatalog.Controls.Add((Control) this.txBookNotes);
      this.grpCatalog.Controls.Add((Control) this.labelBookNotes);
      this.grpCatalog.Controls.Add((Control) this.cbBookLocation);
      this.grpCatalog.Controls.Add((Control) this.labelBookLocation);
      this.grpCatalog.Controls.Add((Control) this.txCollectionStatus);
      this.grpCatalog.Controls.Add((Control) this.cbBookPrice);
      this.grpCatalog.Controls.Add((Control) this.labelBookPrice);
      this.grpCatalog.Controls.Add((Control) this.cbBookAge);
      this.grpCatalog.Controls.Add((Control) this.labelBookAge);
      this.grpCatalog.Controls.Add((Control) this.labelBookCollectionStatus);
      this.grpCatalog.Controls.Add((Control) this.cbBookCondition);
      this.grpCatalog.Controls.Add((Control) this.labelBookCondition);
      this.grpCatalog.Controls.Add((Control) this.cbBookStore);
      this.grpCatalog.Controls.Add((Control) this.labelBookStore);
      this.grpCatalog.Controls.Add((Control) this.cbBookOwner);
      this.grpCatalog.Controls.Add((Control) this.labelBookOwner);
      this.grpCatalog.Dock = DockStyle.Top;
      this.grpCatalog.Location = new System.Drawing.Point(0, 869);
      this.grpCatalog.Name = "grpCatalog";
      this.grpCatalog.Size = new System.Drawing.Size(478, 392);
      this.grpCatalog.TabIndex = 2;
      this.grpCatalog.Text = "Catalog";
      this.labelReleasedTime.AutoSize = true;
      this.labelReleasedTime.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelReleasedTime.Location = new System.Drawing.Point(14, 191);
      this.labelReleasedTime.Name = "labelReleasedTime";
      this.labelReleasedTime.Size = new System.Drawing.Size(56, 12);
      this.labelReleasedTime.TabIndex = 16;
      this.labelReleasedTime.Text = "Released:";
      this.dtpReleasedTime.CustomFormat = " ";
      this.dtpReleasedTime.Location = new System.Drawing.Point(13, 206);
      this.dtpReleasedTime.Name = "dtpReleasedTime";
      this.dtpReleasedTime.Size = new System.Drawing.Size(220, 20);
      this.dtpReleasedTime.TabIndex = 17;
      this.dtpReleasedTime.Tag = (object) "ReleasedTime";
      this.dtpReleasedTime.Value = new DateTime(0L);
      this.labelOpenedTime.AutoSize = true;
      this.labelOpenedTime.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelOpenedTime.Location = new System.Drawing.Point(248, 229);
      this.labelOpenedTime.Name = "labelOpenedTime";
      this.labelOpenedTime.Size = new System.Drawing.Size(77, 12);
      this.labelOpenedTime.TabIndex = 20;
      this.labelOpenedTime.Text = "Opened/Read:";
      this.dtpOpenedTime.CustomFormat = " ";
      this.dtpOpenedTime.Location = new System.Drawing.Point(246, 244);
      this.dtpOpenedTime.Name = "dtpOpenedTime";
      this.dtpOpenedTime.Size = new System.Drawing.Size(218, 20);
      this.dtpOpenedTime.TabIndex = 21;
      this.dtpOpenedTime.Tag = (object) "OpenedTime";
      this.dtpOpenedTime.Value = new DateTime(0L);
      this.labelAddedTime.AutoSize = true;
      this.labelAddedTime.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelAddedTime.Location = new System.Drawing.Point(246, 191);
      this.labelAddedTime.Name = "labelAddedTime";
      this.labelAddedTime.Size = new System.Drawing.Size(98, 12);
      this.labelAddedTime.TabIndex = 18;
      this.labelAddedTime.Text = "Added/Purchased:";
      this.dtpAddedTime.CustomFormat = " ";
      this.dtpAddedTime.Location = new System.Drawing.Point(246, 206);
      this.dtpAddedTime.Name = "dtpAddedTime";
      this.dtpAddedTime.Size = new System.Drawing.Size(218, 20);
      this.dtpAddedTime.TabIndex = 19;
      this.dtpAddedTime.Tag = (object) "AddedTime";
      this.dtpAddedTime.Value = new DateTime(0L);
      this.txPagesAsTextSimple.AcceptsReturn = true;
      this.txPagesAsTextSimple.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txPagesAsTextSimple.Location = new System.Drawing.Point(246, 82);
      this.txPagesAsTextSimple.Name = "txPagesAsTextSimple";
      this.txPagesAsTextSimple.Size = new System.Drawing.Size(218, 20);
      this.txPagesAsTextSimple.TabIndex = 7;
      this.txPagesAsTextSimple.Tag = (object) "PagesAsTextSimple";
      this.labelPagesAsTextSimple.AutoSize = true;
      this.labelPagesAsTextSimple.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelPagesAsTextSimple.Location = new System.Drawing.Point(245, 67);
      this.labelPagesAsTextSimple.Name = "labelPagesAsTextSimple";
      this.labelPagesAsTextSimple.Size = new System.Drawing.Size(40, 12);
      this.labelPagesAsTextSimple.TabIndex = 6;
      this.labelPagesAsTextSimple.Text = "Pages:";
      this.txISBN.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txISBN.Location = new System.Drawing.Point(13, 82);
      this.txISBN.Name = "txISBN";
      this.txISBN.Size = new System.Drawing.Size(220, 20);
      this.txISBN.TabIndex = 5;
      this.txISBN.Tag = (object) "ISBN";
      this.labelISBN.AutoSize = true;
      this.labelISBN.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelISBN.Location = new System.Drawing.Point(13, 67);
      this.labelISBN.Name = "labelISBN";
      this.labelISBN.Size = new System.Drawing.Size(35, 12);
      this.labelISBN.TabIndex = 4;
      this.labelISBN.Text = "ISBN:";
      this.txBookNotes.AcceptsReturn = true;
      this.txBookNotes.FocusSelect = false;
      this.txBookNotes.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txBookNotes.Location = new System.Drawing.Point(13, 327);
      this.txBookNotes.Multiline = true;
      this.txBookNotes.Name = "txBookNotes";
      this.txBookNotes.ScrollBars = ScrollBars.Vertical;
      this.txBookNotes.Size = new System.Drawing.Size(452, 50);
      this.txBookNotes.TabIndex = 25;
      this.txBookNotes.Tag = (object) "BookNotes";
      this.labelBookNotes.AutoSize = true;
      this.labelBookNotes.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookNotes.Location = new System.Drawing.Point(13, 312);
      this.labelBookNotes.Name = "labelBookNotes";
      this.labelBookNotes.Size = new System.Drawing.Size(120, 12);
      this.labelBookNotes.TabIndex = 24;
      this.labelBookNotes.Text = "Notes about this Book:";
      this.cbBookLocation.FormattingEnabled = true;
      this.cbBookLocation.Location = new System.Drawing.Point(246, 158);
      this.cbBookLocation.Name = "cbBookLocation";
      this.cbBookLocation.Size = new System.Drawing.Size(218, 21);
      this.cbBookLocation.TabIndex = 15;
      this.cbBookLocation.Tag = (object) "BookLocation";
      this.labelBookLocation.AutoSize = true;
      this.labelBookLocation.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookLocation.Location = new System.Drawing.Point(245, 144);
      this.labelBookLocation.Name = "labelBookLocation";
      this.labelBookLocation.Size = new System.Drawing.Size(80, 12);
      this.labelBookLocation.TabIndex = 14;
      this.labelBookLocation.Text = "Book Location:";
      this.txCollectionStatus.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txCollectionStatus.Location = new System.Drawing.Point(13, 286);
      this.txCollectionStatus.Name = "txCollectionStatus";
      this.txCollectionStatus.Size = new System.Drawing.Size(451, 20);
      this.txCollectionStatus.TabIndex = 23;
      this.txCollectionStatus.Tag = (object) "CollectionStatus";
      this.cbBookPrice.FormattingEnabled = true;
      this.cbBookPrice.Location = new System.Drawing.Point(246, 44);
      this.cbBookPrice.Name = "cbBookPrice";
      this.cbBookPrice.Size = new System.Drawing.Size(218, 21);
      this.cbBookPrice.TabIndex = 3;
      this.cbBookPrice.Tag = (object) "BookPrice";
      this.labelBookPrice.AutoSize = true;
      this.labelBookPrice.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookPrice.Location = new System.Drawing.Point(244, 30);
      this.labelBookPrice.Name = "labelBookPrice";
      this.labelBookPrice.Size = new System.Drawing.Size(35, 12);
      this.labelBookPrice.TabIndex = 2;
      this.labelBookPrice.Text = "Price:";
      this.cbBookAge.FormattingEnabled = true;
      this.cbBookAge.Location = new System.Drawing.Point(13, 121);
      this.cbBookAge.Name = "cbBookAge";
      this.cbBookAge.Size = new System.Drawing.Size(220, 21);
      this.cbBookAge.TabIndex = 9;
      this.cbBookAge.Tag = (object) "BookAge";
      this.labelBookAge.AutoSize = true;
      this.labelBookAge.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookAge.Location = new System.Drawing.Point(12, 106);
      this.labelBookAge.Name = "labelBookAge";
      this.labelBookAge.Size = new System.Drawing.Size(29, 12);
      this.labelBookAge.TabIndex = 8;
      this.labelBookAge.Text = "Age:";
      this.labelBookCollectionStatus.AutoSize = true;
      this.labelBookCollectionStatus.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookCollectionStatus.Location = new System.Drawing.Point(12, 271);
      this.labelBookCollectionStatus.Name = "labelBookCollectionStatus";
      this.labelBookCollectionStatus.Size = new System.Drawing.Size(96, 12);
      this.labelBookCollectionStatus.TabIndex = 22;
      this.labelBookCollectionStatus.Text = "Collection Status:";
      this.cbBookCondition.FormattingEnabled = true;
      this.cbBookCondition.Location = new System.Drawing.Point(246, 120);
      this.cbBookCondition.Name = "cbBookCondition";
      this.cbBookCondition.Size = new System.Drawing.Size(218, 21);
      this.cbBookCondition.TabIndex = 11;
      this.cbBookCondition.Tag = (object) "BookCondition";
      this.labelBookCondition.AutoSize = true;
      this.labelBookCondition.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookCondition.Location = new System.Drawing.Point(244, 106);
      this.labelBookCondition.Name = "labelBookCondition";
      this.labelBookCondition.Size = new System.Drawing.Size(57, 12);
      this.labelBookCondition.TabIndex = 10;
      this.labelBookCondition.Text = "Condition:";
      this.cbBookStore.FormattingEnabled = true;
      this.cbBookStore.Location = new System.Drawing.Point(13, 45);
      this.cbBookStore.Name = "cbBookStore";
      this.cbBookStore.PromptText = (string) null;
      this.cbBookStore.Size = new System.Drawing.Size(220, 21);
      this.cbBookStore.TabIndex = 1;
      this.cbBookStore.Tag = (object) "BookStore";
      this.labelBookStore.AutoSize = true;
      this.labelBookStore.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookStore.Location = new System.Drawing.Point(11, 30);
      this.labelBookStore.Name = "labelBookStore";
      this.labelBookStore.Size = new System.Drawing.Size(36, 12);
      this.labelBookStore.TabIndex = 0;
      this.labelBookStore.Text = "Store:";
      this.cbBookOwner.FormattingEnabled = true;
      this.cbBookOwner.Location = new System.Drawing.Point(13, 158);
      this.cbBookOwner.Name = "cbBookOwner";
      this.cbBookOwner.Size = new System.Drawing.Size(220, 21);
      this.cbBookOwner.TabIndex = 13;
      this.cbBookOwner.Tag = (object) "BookOwner";
      this.labelBookOwner.AutoSize = true;
      this.labelBookOwner.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelBookOwner.Location = new System.Drawing.Point(13, 144);
      this.labelBookOwner.Name = "labelBookOwner";
      this.labelBookOwner.Size = new System.Drawing.Size(42, 12);
      this.labelBookOwner.TabIndex = 12;
      this.labelBookOwner.Text = "Owner:";
      this.grpPlotNotes.Controls.Add((Control) this.labelMainCharacterOrTeam);
      this.grpPlotNotes.Controls.Add((Control) this.txMainCharacterOrTeam);
      this.grpPlotNotes.Controls.Add((Control) this.labelReview);
      this.grpPlotNotes.Controls.Add((Control) this.txReview);
      this.grpPlotNotes.Controls.Add((Control) this.txScanInformation);
      this.grpPlotNotes.Controls.Add((Control) this.labelScanInformation);
      this.grpPlotNotes.Controls.Add((Control) this.txWeb);
      this.grpPlotNotes.Controls.Add((Control) this.txSummary);
      this.grpPlotNotes.Controls.Add((Control) this.labelSummary);
      this.grpPlotNotes.Controls.Add((Control) this.labelNotes);
      this.grpPlotNotes.Controls.Add((Control) this.txNotes);
      this.grpPlotNotes.Controls.Add((Control) this.labelCharacters);
      this.grpPlotNotes.Controls.Add((Control) this.txCharacters);
      this.grpPlotNotes.Controls.Add((Control) this.labelWeb);
      this.grpPlotNotes.Controls.Add((Control) this.labelTeams);
      this.grpPlotNotes.Controls.Add((Control) this.txTeams);
      this.grpPlotNotes.Controls.Add((Control) this.labelLocations);
      this.grpPlotNotes.Controls.Add((Control) this.txLocations);
      this.grpPlotNotes.Dock = DockStyle.Top;
      this.grpPlotNotes.Location = new System.Drawing.Point(0, 604);
      this.grpPlotNotes.Name = "grpPlotNotes";
      this.grpPlotNotes.Size = new System.Drawing.Size(478, 265);
      this.grpPlotNotes.TabIndex = 3;
      this.grpPlotNotes.Text = "Plot & Notes";
      this.labelMainCharacterOrTeam.AutoSize = true;
      this.labelMainCharacterOrTeam.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelMainCharacterOrTeam.Location = new System.Drawing.Point(247, 33);
      this.labelMainCharacterOrTeam.Name = "labelMainCharacterOrTeam";
      this.labelMainCharacterOrTeam.Size = new System.Drawing.Size(118, 12);
      this.labelMainCharacterOrTeam.TabIndex = 2;
      this.labelMainCharacterOrTeam.Text = "Main Character/Team:";
      this.txMainCharacterOrTeam.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txMainCharacterOrTeam.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txMainCharacterOrTeam.Location = new System.Drawing.Point(243, 48);
      this.txMainCharacterOrTeam.Name = "txMainCharacterOrTeam";
      this.txMainCharacterOrTeam.Size = new System.Drawing.Size(224, 20);
      this.txMainCharacterOrTeam.TabIndex = 3;
      this.labelReview.AutoSize = true;
      this.labelReview.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelReview.Location = new System.Drawing.Point(16, 182);
      this.labelReview.Name = "labelReview";
      this.labelReview.Size = new System.Drawing.Size(48, 12);
      this.labelReview.TabIndex = 12;
      this.labelReview.Text = "Review:";
      this.txReview.Location = new System.Drawing.Point(14, 196);
      this.txReview.Multiline = true;
      this.txReview.Name = "txReview";
      this.txReview.Size = new System.Drawing.Size(220, 55);
      this.txReview.TabIndex = 13;
      this.txScanInformation.Location = new System.Drawing.Point(244, 196);
      this.txScanInformation.Name = "txScanInformation";
      this.txScanInformation.Size = new System.Drawing.Size(222, 20);
      this.txScanInformation.TabIndex = 15;
      this.labelScanInformation.AutoSize = true;
      this.labelScanInformation.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelScanInformation.Location = new System.Drawing.Point(243, 182);
      this.labelScanInformation.Name = "labelScanInformation";
      this.labelScanInformation.Size = new System.Drawing.Size(95, 12);
      this.labelScanInformation.TabIndex = 14;
      this.labelScanInformation.Text = "Scan Information:";
      this.grpArtists.Controls.Add((Control) this.labelWriter);
      this.grpArtists.Controls.Add((Control) this.labelPenciller);
      this.grpArtists.Controls.Add((Control) this.labelInker);
      this.grpArtists.Controls.Add((Control) this.labelColorist);
      this.grpArtists.Controls.Add((Control) this.txPenciller);
      this.grpArtists.Controls.Add((Control) this.txInker);
      this.grpArtists.Controls.Add((Control) this.txColorist);
      this.grpArtists.Controls.Add((Control) this.txEditor);
      this.grpArtists.Controls.Add((Control) this.txWriter);
      this.grpArtists.Controls.Add((Control) this.txCoverArtist);
      this.grpArtists.Controls.Add((Control) this.labelLetterer);
      this.grpArtists.Controls.Add((Control) this.txLetterer);
      this.grpArtists.Controls.Add((Control) this.labelCoverArtist);
      this.grpArtists.Controls.Add((Control) this.labelEditor);
      this.grpArtists.Dock = DockStyle.Top;
      this.grpArtists.Location = new System.Drawing.Point(0, 400);
      this.grpArtists.Name = "grpArtists";
      this.grpArtists.Size = new System.Drawing.Size(478, 204);
      this.grpArtists.TabIndex = 1;
      this.grpArtists.Text = "Artists / People Involved";
      this.grpMain.Controls.Add((Control) this.labelDay);
      this.grpMain.Controls.Add((Control) this.txDay);
      this.grpMain.Controls.Add((Control) this.labelSeriesGroup);
      this.grpMain.Controls.Add((Control) this.txSeriesGroup);
      this.grpMain.Controls.Add((Control) this.labelStoryArc);
      this.grpMain.Controls.Add((Control) this.txStoryArc);
      this.grpMain.Controls.Add((Control) this.cbSeriesComplete);
      this.grpMain.Controls.Add((Control) this.labelSeriesComplete);
      this.grpMain.Controls.Add((Control) this.cbEnableProposed);
      this.grpMain.Controls.Add((Control) this.labelRating);
      this.grpMain.Controls.Add((Control) this.txCommunityRating);
      this.grpMain.Controls.Add((Control) this.labelEnableProposed);
      this.grpMain.Controls.Add((Control) this.labelFormat);
      this.grpMain.Controls.Add((Control) this.cbImprint);
      this.grpMain.Controls.Add((Control) this.labelTags);
      this.grpMain.Controls.Add((Control) this.labelAlternateSeries);
      this.grpMain.Controls.Add((Control) this.labelImprint);
      this.grpMain.Controls.Add((Control) this.labelCommunityRating);
      this.grpMain.Controls.Add((Control) this.labelAlternateNumber);
      this.grpMain.Controls.Add((Control) this.cbFormat);
      this.grpMain.Controls.Add((Control) this.labelGenre);
      this.grpMain.Controls.Add((Control) this.labelSeries);
      this.grpMain.Controls.Add((Control) this.cbPublisher);
      this.grpMain.Controls.Add((Control) this.txGenre);
      this.grpMain.Controls.Add((Control) this.labelAlternateCount);
      this.grpMain.Controls.Add((Control) this.labelPublisher);
      this.grpMain.Controls.Add((Control) this.txRating);
      this.grpMain.Controls.Add((Control) this.labelTitle);
      this.grpMain.Controls.Add((Control) this.labelAgeRating);
      this.grpMain.Controls.Add((Control) this.txTags);
      this.grpMain.Controls.Add((Control) this.txAlternateSeries);
      this.grpMain.Controls.Add((Control) this.labelLanguage);
      this.grpMain.Controls.Add((Control) this.labelYear);
      this.grpMain.Controls.Add((Control) this.cbLanguage);
      this.grpMain.Controls.Add((Control) this.cbManga);
      this.grpMain.Controls.Add((Control) this.txAlternateNumber);
      this.grpMain.Controls.Add((Control) this.labelManga);
      this.grpMain.Controls.Add((Control) this.labelNumber);
      this.grpMain.Controls.Add((Control) this.labelBlackAndWhite);
      this.grpMain.Controls.Add((Control) this.cbBlackAndWhite);
      this.grpMain.Controls.Add((Control) this.txAlternateCount);
      this.grpMain.Controls.Add((Control) this.cbAgeRating);
      this.grpMain.Controls.Add((Control) this.labelMonth);
      this.grpMain.Controls.Add((Control) this.labelVolume);
      this.grpMain.Controls.Add((Control) this.labelCount);
      this.grpMain.Controls.Add((Control) this.txTitle);
      this.grpMain.Controls.Add((Control) this.txSeries);
      this.grpMain.Controls.Add((Control) this.txYear);
      this.grpMain.Controls.Add((Control) this.txMonth);
      this.grpMain.Controls.Add((Control) this.txVolume);
      this.grpMain.Controls.Add((Control) this.txNumber);
      this.grpMain.Controls.Add((Control) this.txCount);
      this.grpMain.Dock = DockStyle.Top;
      this.grpMain.Location = new System.Drawing.Point(0, 0);
      this.grpMain.Name = "grpMain";
      this.grpMain.Size = new System.Drawing.Size(478, 400);
      this.grpMain.TabIndex = 0;
      this.grpMain.Text = "Main";
      this.labelDay.AutoSize = true;
      this.labelDay.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelDay.Location = new System.Drawing.Point(393, 74);
      this.labelDay.Name = "labelDay";
      this.labelDay.Size = new System.Drawing.Size(29, 12);
      this.labelDay.TabIndex = 14;
      this.labelDay.Text = "Day:";
      this.txDay.Location = new System.Drawing.Point(395, 87);
      this.txDay.MaxLength = 4;
      this.txDay.Name = "txDay";
      this.txDay.PromptText = "";
      this.txDay.Size = new System.Drawing.Size(71, 20);
      this.txDay.TabIndex = 15;
      this.labelSeriesGroup.AutoSize = true;
      this.labelSeriesGroup.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelSeriesGroup.Location = new System.Drawing.Point(163, 150);
      this.labelSeriesGroup.Name = "labelSeriesGroup";
      this.labelSeriesGroup.Size = new System.Drawing.Size(74, 12);
      this.labelSeriesGroup.TabIndex = 24;
      this.labelSeriesGroup.Text = "Series Group:";
      this.txSeriesGroup.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txSeriesGroup.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txSeriesGroup.Location = new System.Drawing.Point(166, 165);
      this.txSeriesGroup.Name = "txSeriesGroup";
      this.txSeriesGroup.PromptText = "";
      this.txSeriesGroup.Size = new System.Drawing.Size(143, 20);
      this.txSeriesGroup.TabIndex = 25;
      this.txSeriesGroup.Tag = (object) "";
      this.labelStoryArc.AutoSize = true;
      this.labelStoryArc.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelStoryArc.Location = new System.Drawing.Point(9, 150);
      this.labelStoryArc.Name = "labelStoryArc";
      this.labelStoryArc.Size = new System.Drawing.Size(57, 12);
      this.labelStoryArc.TabIndex = 22;
      this.labelStoryArc.Text = "Story Arc:";
      this.txStoryArc.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
      this.txStoryArc.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.txStoryArc.Location = new System.Drawing.Point(11, 164);
      this.txStoryArc.Name = "txStoryArc";
      this.txStoryArc.PromptText = "";
      this.txStoryArc.Size = new System.Drawing.Size(144, 20);
      this.txStoryArc.TabIndex = 23;
      this.txStoryArc.Tag = (object) "";
      this.cbSeriesComplete.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbSeriesComplete.FormattingEnabled = true;
      this.cbSeriesComplete.Location = new System.Drawing.Point(317, 165);
      this.cbSeriesComplete.Name = "cbSeriesComplete";
      this.cbSeriesComplete.Size = new System.Drawing.Size(149, 21);
      this.cbSeriesComplete.TabIndex = 27;
      this.labelSeriesComplete.AutoSize = true;
      this.labelSeriesComplete.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelSeriesComplete.Location = new System.Drawing.Point(315, 152);
      this.labelSeriesComplete.Name = "labelSeriesComplete";
      this.labelSeriesComplete.Size = new System.Drawing.Size(90, 12);
      this.labelSeriesComplete.TabIndex = 26;
      this.labelSeriesComplete.Text = "Series complete:";
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(525, 460);
      this.Controls.Add((Control) this.pageData);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (MultipleComicBooksDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Multiple Book Information ({0})";
      this.pageData.ResumeLayout(false);
      this.grpCustom.ResumeLayout(false);
      this.grpCustom.PerformLayout();
      this.grpCatalog.ResumeLayout(false);
      this.grpCatalog.PerformLayout();
      this.grpPlotNotes.ResumeLayout(false);
      this.grpPlotNotes.PerformLayout();
      this.grpArtists.ResumeLayout(false);
      this.grpArtists.PerformLayout();
      this.grpMain.ResumeLayout(false);
      this.grpMain.PerformLayout();
      this.ResumeLayout(false);
    }

    private class StoreInfo
    {
      public string PropertyName;
      public bool ListMode;
      public object Value;
      public HashSet<string> NewList;
      public HashSet<string> OldList;
      public PropertyInfo PropertyInfo;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ComicDataPasteDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Reflection;
using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Viewer.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ComicDataPasteDialog : Form
  {
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private CheckBox chkSeries;
    private CheckBox chkTitle;
    private CheckBox chkVolume;
    private CheckBox chkYear;
    private CheckBox chkNumber;
    private CheckBox chkCount;
    private CheckBox chkMonth;
    private CheckBox chkWriter;
    private CheckBox chkPenciller;
    private CheckBox chkPublisher;
    private CheckBox chkInker;
    private CheckBox chkColorist;
    private CheckBox chkLetterer;
    private CheckBox chkCover;
    private CheckBox chkEditor;
    private CheckBox chkGenre;
    private CheckBox chkLanguage;
    private CheckBox chkImprint;
    private CheckBox chkRating;
    private CheckBox chkSummary;
    private CheckBox chkTags;
    private CheckBox chkNotes;
    private CheckBox chkManga;
    private CheckBox chkAlternateSeries;
    private CheckBox chkAlternateNumber;
    private CheckBox chkAlternateCount;
    private CheckBox chkColor;
    private Button btMarkDefined;
    private Button btMarkAll;
    private Button btMarkNone;
    private CheckBox chkFormat;
    private CheckBox chkBlackAndWhite;
    private CheckBox chkAgeRating;
    private CheckBox chkCharacters;
    private CheckBox chkWeb;
    private CheckBox chkLocations;
    private CheckBox chkTeams;
    private CheckBox chkCommunityRating;
    private CheckBox chkBookCollectionStatus;
    private CheckBox chkBookNotes;
    private CheckBox chkBookCondition;
    private CheckBox chkBookLocation;
    private CheckBox chkBookOwner;
    private CheckBox chkBookStore;
    private CheckBox chkBookPrice;
    private Panel pageData;
    private CollapsibleGroupBox grpCatalog;
    private CollapsibleGroupBox grpPlotNotes;
    private CollapsibleGroupBox grpArtists;
    private CollapsibleGroupBox grpMain;
    private CheckBox chkBookAge;
    private CheckBox chkISBN;
    private CheckBox chkPageCount;
    private CheckBox chkAddedTime;
    private CheckBox chkOpenedTime;
    private CheckBox chkSeriesComplete;
    private CheckBox chkScanInformation;
    private CheckBox chkReview;
    private CheckBox chkMainCharacterOrTeam;
    private CheckBox chkSeriesGroup;
    private CheckBox chkStoryArc;
    private CheckBox chkDay;
    private CheckBox chkReleasedTime;
    private CollapsibleGroupBox grpCustom;

    public ComicDataPasteDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.RestorePosition();
      this.RestorePanelStates();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      FormUtility.RegisterPanelToTabToggle((Control) this.pageData, PropertyCaller.CreateFlagsValueStore<Settings, TabLayouts>(Program.Settings, "TabLayouts", TabLayouts.Paste));
    }

    public void SetChecks(ComicBook data, IEnumerable<string> properties)
    {
      this.grpCatalog.Visible = !data.IsLinked || !Program.Settings.CatalogOnlyForFileless;
      foreach (CheckBox control in this.GetControls<CheckBox>())
      {
        string tag = control.Tag as string;
        if (properties.Contains<string>(tag))
          control.Checked = true;
        try
        {
          if (!data.IsDefaultValue(tag))
            control.Font = FC.Get(control.Font, FontStyle.Bold);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public IEnumerable<string> GetChecks()
    {
      return (IEnumerable<string>) this.GetControls<CheckBox>().Where<CheckBox>((Func<CheckBox, bool>) (cb => cb.Enabled)).Where<CheckBox>((Func<CheckBox, bool>) (cb => cb.Checked && cb.Tag != null)).Select<CheckBox, string>((Func<CheckBox, string>) (cb => (string) cb.Tag)).ToArray<string>();
    }

    private void btMarkDefined_Click(object sender, EventArgs e)
    {
      foreach (CheckBox control in this.GetControls<CheckBox>())
        control.Checked = control.Font.Bold;
    }

    private void btMarkAll_Click(object sender, EventArgs e)
    {
      foreach (CheckBox control in this.GetControls<CheckBox>())
        control.Checked = true;
    }

    private void btMarkNone_Click(object sender, EventArgs e)
    {
      foreach (CheckBox control in this.GetControls<CheckBox>())
        control.Checked = false;
    }

    public static void ShowAndPaste(
      IWin32Window parent,
      ComicBook data,
      IEnumerable<ComicBook> books)
    {
      IEnumerable<string> checks = (IEnumerable<string>) null;
      int count = books.Count<ComicBook>();
      string[] array = Program.Settings.ShowCustomBookFields ? Program.Database.CustomValues.Where<string>((Func<string, bool>) (k => Program.ExtendedSettings.ShowCustomScriptValues || !k.Contains<char>('.'))).ToArray<string>() : (string[]) null;
      using (ComicDataPasteDialog comicDataPasteDialog = new ComicDataPasteDialog())
      {
        comicDataPasteDialog.SetChecks(data, (IEnumerable<string>) Program.Settings.PasteProperties.Split(';'));
        comicDataPasteDialog.Text = StringUtility.Format(comicDataPasteDialog.Text, (object) count);
        if (array == null || array.Length == 0)
        {
          comicDataPasteDialog.grpCustom.Visible = false;
        }
        else
        {
          int num = 0;
          Control control = (Control) null;
          foreach (string key in array)
          {
            CheckBox checkBox = (CheckBox) comicDataPasteDialog.chkBookStore.Clone();
            comicDataPasteDialog.grpCustom.Controls.Add((Control) checkBox);
            checkBox.Top = comicDataPasteDialog.chkBookStore.Top + (comicDataPasteDialog.chkBookCondition.Top - comicDataPasteDialog.chkBookStore.Top) * (num / 4);
            switch (num % 4)
            {
              case 0:
                checkBox.Left = comicDataPasteDialog.chkBookStore.Left;
                break;
              case 1:
                checkBox.Left = comicDataPasteDialog.chkBookPrice.Left;
                break;
              case 2:
                checkBox.Left = comicDataPasteDialog.chkBookLocation.Left;
                break;
              case 3:
                checkBox.Left = comicDataPasteDialog.chkBookAge.Left;
                break;
            }
            checkBox.Visible = true;
            if (data.GetCustomValue(key) != null)
              checkBox.Font = FC.Get(checkBox.Font, FontStyle.Bold);
            checkBox.Text = key;
            checkBox.Tag = (object) ("{" + key + "}");
            ++num;
            control = (Control) checkBox;
          }
          if (control != null)
            comicDataPasteDialog.grpCustom.Height = control.Bottom + 8;
        }
        if (!books.Any<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsLinked)))
          comicDataPasteDialog.chkScanInformation.Visible = false;
        else
          comicDataPasteDialog.chkOpenedTime.Visible = comicDataPasteDialog.chkPageCount.Visible = comicDataPasteDialog.chkOpenedTime.Enabled = comicDataPasteDialog.chkPageCount.Enabled = false;
        if (!books.Any<ComicBook>((Func<ComicBook, bool>) (cb => cb.IsInContainer)))
          comicDataPasteDialog.chkCommunityRating.Enabled = comicDataPasteDialog.chkRating.Enabled = comicDataPasteDialog.chkTags.Enabled = comicDataPasteDialog.chkColor.Enabled = comicDataPasteDialog.chkSeriesComplete.Enabled = false;
        if (comicDataPasteDialog.ShowDialog(parent) == DialogResult.OK)
          checks = comicDataPasteDialog.GetChecks();
        Program.Settings.PasteProperties = comicDataPasteDialog.GetChecks().ToListString(";");
      }
      if (checks == null)
        return;
      AutomaticProgressDialog.Process(parent, TR.Messages["WriteData", "Pasting Data"], TR.Messages["WriteDataText", "Pasting Data into selected Books"], 1000, (Action) (() =>
      {
        int num = 0;
        foreach (ComicBook book in books)
        {
          if (AutomaticProgressDialog.ShouldAbort)
            break;
          book.RefreshInfoFromFile(RefreshInfoOptions.None);
          book.CopyDataFrom(data, checks);
          AutomaticProgressDialog.Value = num++ * 100 / count;
        }
      }), AutomaticProgressDialogOptions.EnableCancel);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.btCancel = new Button();
      this.btOK = new Button();
      this.chkSeries = new CheckBox();
      this.chkTitle = new CheckBox();
      this.chkVolume = new CheckBox();
      this.chkYear = new CheckBox();
      this.chkNumber = new CheckBox();
      this.chkCount = new CheckBox();
      this.chkMonth = new CheckBox();
      this.chkWriter = new CheckBox();
      this.chkPenciller = new CheckBox();
      this.chkPublisher = new CheckBox();
      this.chkInker = new CheckBox();
      this.chkColorist = new CheckBox();
      this.chkLetterer = new CheckBox();
      this.chkCover = new CheckBox();
      this.chkEditor = new CheckBox();
      this.chkGenre = new CheckBox();
      this.chkLanguage = new CheckBox();
      this.chkImprint = new CheckBox();
      this.chkRating = new CheckBox();
      this.chkSummary = new CheckBox();
      this.chkTags = new CheckBox();
      this.chkNotes = new CheckBox();
      this.chkManga = new CheckBox();
      this.chkAlternateSeries = new CheckBox();
      this.chkAlternateNumber = new CheckBox();
      this.chkAlternateCount = new CheckBox();
      this.chkColor = new CheckBox();
      this.chkCommunityRating = new CheckBox();
      this.chkAgeRating = new CheckBox();
      this.chkFormat = new CheckBox();
      this.chkBlackAndWhite = new CheckBox();
      this.chkLocations = new CheckBox();
      this.chkTeams = new CheckBox();
      this.chkWeb = new CheckBox();
      this.chkCharacters = new CheckBox();
      this.chkBookCollectionStatus = new CheckBox();
      this.chkBookNotes = new CheckBox();
      this.chkBookCondition = new CheckBox();
      this.chkBookLocation = new CheckBox();
      this.chkBookOwner = new CheckBox();
      this.chkBookStore = new CheckBox();
      this.chkBookPrice = new CheckBox();
      this.btMarkDefined = new Button();
      this.btMarkAll = new Button();
      this.btMarkNone = new Button();
      this.pageData = new Panel();
      this.grpCustom = new CollapsibleGroupBox();
      this.grpCatalog = new CollapsibleGroupBox();
      this.chkReleasedTime = new CheckBox();
      this.chkAddedTime = new CheckBox();
      this.chkOpenedTime = new CheckBox();
      this.chkPageCount = new CheckBox();
      this.chkISBN = new CheckBox();
      this.chkBookAge = new CheckBox();
      this.grpPlotNotes = new CollapsibleGroupBox();
      this.chkScanInformation = new CheckBox();
      this.chkReview = new CheckBox();
      this.chkMainCharacterOrTeam = new CheckBox();
      this.grpArtists = new CollapsibleGroupBox();
      this.grpMain = new CollapsibleGroupBox();
      this.chkDay = new CheckBox();
      this.chkSeriesGroup = new CheckBox();
      this.chkStoryArc = new CheckBox();
      this.chkSeriesComplete = new CheckBox();
      this.pageData.SuspendLayout();
      this.grpCatalog.SuspendLayout();
      this.grpPlotNotes.SuspendLayout();
      this.grpArtists.SuspendLayout();
      this.grpMain.SuspendLayout();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(473, 389);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 1;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(387, 389);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 0;
      this.btOK.Text = "&OK";
      this.chkSeries.AutoEllipsis = true;
      this.chkSeries.Location = new System.Drawing.Point(12, 35);
      this.chkSeries.Name = "chkSeries";
      this.chkSeries.Size = new System.Drawing.Size(120, 17);
      this.chkSeries.TabIndex = 0;
      this.chkSeries.Tag = (object) "Series";
      this.chkSeries.Text = "Series";
      this.chkSeries.UseVisualStyleBackColor = true;
      this.chkTitle.AutoEllipsis = true;
      this.chkTitle.Location = new System.Drawing.Point(12, 58);
      this.chkTitle.Name = "chkTitle";
      this.chkTitle.Size = new System.Drawing.Size(120, 17);
      this.chkTitle.TabIndex = 4;
      this.chkTitle.Tag = (object) "Title";
      this.chkTitle.Text = "Title";
      this.chkTitle.UseVisualStyleBackColor = true;
      this.chkVolume.AutoEllipsis = true;
      this.chkVolume.Location = new System.Drawing.Point(138, 35);
      this.chkVolume.Name = "chkVolume";
      this.chkVolume.Size = new System.Drawing.Size(120, 17);
      this.chkVolume.TabIndex = 1;
      this.chkVolume.Tag = (object) "Volume";
      this.chkVolume.Text = "Volume";
      this.chkVolume.UseVisualStyleBackColor = true;
      this.chkYear.AutoEllipsis = true;
      this.chkYear.Location = new System.Drawing.Point(138, 58);
      this.chkYear.Name = "chkYear";
      this.chkYear.Size = new System.Drawing.Size(120, 17);
      this.chkYear.TabIndex = 5;
      this.chkYear.Tag = (object) "Year";
      this.chkYear.Text = "Year";
      this.chkYear.UseVisualStyleBackColor = true;
      this.chkNumber.AutoEllipsis = true;
      this.chkNumber.Location = new System.Drawing.Point(264, 35);
      this.chkNumber.Name = "chkNumber";
      this.chkNumber.Size = new System.Drawing.Size(120, 17);
      this.chkNumber.TabIndex = 2;
      this.chkNumber.Tag = (object) "Number";
      this.chkNumber.Text = "Number";
      this.chkNumber.UseVisualStyleBackColor = true;
      this.chkCount.AutoEllipsis = true;
      this.chkCount.Location = new System.Drawing.Point(398, 35);
      this.chkCount.Name = "chkCount";
      this.chkCount.Size = new System.Drawing.Size(120, 17);
      this.chkCount.TabIndex = 3;
      this.chkCount.Tag = (object) "Count";
      this.chkCount.Text = "Total Number";
      this.chkCount.UseVisualStyleBackColor = true;
      this.chkMonth.AutoEllipsis = true;
      this.chkMonth.Location = new System.Drawing.Point(264, 58);
      this.chkMonth.Name = "chkMonth";
      this.chkMonth.Size = new System.Drawing.Size(120, 17);
      this.chkMonth.TabIndex = 6;
      this.chkMonth.Tag = (object) "Month";
      this.chkMonth.Text = "Month";
      this.chkMonth.UseVisualStyleBackColor = true;
      this.chkWriter.AutoEllipsis = true;
      this.chkWriter.Location = new System.Drawing.Point(12, 38);
      this.chkWriter.Name = "chkWriter";
      this.chkWriter.Size = new System.Drawing.Size(120, 17);
      this.chkWriter.TabIndex = 0;
      this.chkWriter.Tag = (object) "Writer";
      this.chkWriter.Text = "Writer";
      this.chkWriter.UseVisualStyleBackColor = true;
      this.chkPenciller.AutoEllipsis = true;
      this.chkPenciller.Location = new System.Drawing.Point(138, 38);
      this.chkPenciller.Name = "chkPenciller";
      this.chkPenciller.Size = new System.Drawing.Size(120, 17);
      this.chkPenciller.TabIndex = 1;
      this.chkPenciller.Tag = (object) "Penciller";
      this.chkPenciller.Text = "Penciller";
      this.chkPenciller.UseVisualStyleBackColor = true;
      this.chkPublisher.AutoEllipsis = true;
      this.chkPublisher.Location = new System.Drawing.Point(264, 128);
      this.chkPublisher.Name = "chkPublisher";
      this.chkPublisher.Size = new System.Drawing.Size(120, 17);
      this.chkPublisher.TabIndex = 16;
      this.chkPublisher.Tag = (object) "Publisher";
      this.chkPublisher.Text = "Publisher";
      this.chkPublisher.UseVisualStyleBackColor = true;
      this.chkInker.AutoEllipsis = true;
      this.chkInker.Location = new System.Drawing.Point(264, 38);
      this.chkInker.Name = "chkInker";
      this.chkInker.Size = new System.Drawing.Size(120, 17);
      this.chkInker.TabIndex = 2;
      this.chkInker.Tag = (object) "Inker";
      this.chkInker.Text = "Inker";
      this.chkInker.UseVisualStyleBackColor = true;
      this.chkColorist.AutoEllipsis = true;
      this.chkColorist.Location = new System.Drawing.Point(398, 38);
      this.chkColorist.Name = "chkColorist";
      this.chkColorist.Size = new System.Drawing.Size(120, 17);
      this.chkColorist.TabIndex = 3;
      this.chkColorist.Tag = (object) "Colorist";
      this.chkColorist.Text = "Colorist";
      this.chkColorist.UseVisualStyleBackColor = true;
      this.chkLetterer.AutoEllipsis = true;
      this.chkLetterer.Location = new System.Drawing.Point(12, 61);
      this.chkLetterer.Name = "chkLetterer";
      this.chkLetterer.Size = new System.Drawing.Size(120, 17);
      this.chkLetterer.TabIndex = 4;
      this.chkLetterer.Tag = (object) "Letterer";
      this.chkLetterer.Text = "Letterer";
      this.chkLetterer.UseVisualStyleBackColor = true;
      this.chkCover.AutoEllipsis = true;
      this.chkCover.Location = new System.Drawing.Point(138, 61);
      this.chkCover.Name = "chkCover";
      this.chkCover.Size = new System.Drawing.Size(120, 17);
      this.chkCover.TabIndex = 5;
      this.chkCover.Tag = (object) "CoverArtist";
      this.chkCover.Text = "Cover Artist";
      this.chkCover.UseVisualStyleBackColor = true;
      this.chkEditor.AutoEllipsis = true;
      this.chkEditor.Location = new System.Drawing.Point(264, 61);
      this.chkEditor.Name = "chkEditor";
      this.chkEditor.Size = new System.Drawing.Size(120, 17);
      this.chkEditor.TabIndex = 6;
      this.chkEditor.Tag = (object) "Editor";
      this.chkEditor.Text = "Editor";
      this.chkEditor.UseVisualStyleBackColor = true;
      this.chkGenre.AutoEllipsis = true;
      this.chkGenre.Location = new System.Drawing.Point(264, 151);
      this.chkGenre.Name = "chkGenre";
      this.chkGenre.Size = new System.Drawing.Size(120, 17);
      this.chkGenre.TabIndex = 20;
      this.chkGenre.Tag = (object) "Genre";
      this.chkGenre.Text = "Genre";
      this.chkGenre.UseVisualStyleBackColor = true;
      this.chkLanguage.AutoEllipsis = true;
      this.chkLanguage.Location = new System.Drawing.Point(138, 151);
      this.chkLanguage.Name = "chkLanguage";
      this.chkLanguage.Size = new System.Drawing.Size(120, 17);
      this.chkLanguage.TabIndex = 19;
      this.chkLanguage.Tag = (object) "LanguageISO";
      this.chkLanguage.Text = "Language";
      this.chkLanguage.UseVisualStyleBackColor = true;
      this.chkImprint.AutoEllipsis = true;
      this.chkImprint.Location = new System.Drawing.Point(12, 151);
      this.chkImprint.Name = "chkImprint";
      this.chkImprint.Size = new System.Drawing.Size(120, 17);
      this.chkImprint.TabIndex = 18;
      this.chkImprint.Tag = (object) "Imprint";
      this.chkImprint.Text = "Imprint";
      this.chkImprint.UseVisualStyleBackColor = true;
      this.chkRating.AutoEllipsis = true;
      this.chkRating.Location = new System.Drawing.Point(138, 174);
      this.chkRating.Name = "chkRating";
      this.chkRating.Size = new System.Drawing.Size(120, 17);
      this.chkRating.TabIndex = 23;
      this.chkRating.Tag = (object) "Rating";
      this.chkRating.Text = "My Rating";
      this.chkRating.UseVisualStyleBackColor = true;
      this.chkSummary.AutoEllipsis = true;
      this.chkSummary.Location = new System.Drawing.Point(12, 59);
      this.chkSummary.Name = "chkSummary";
      this.chkSummary.Size = new System.Drawing.Size(120, 17);
      this.chkSummary.TabIndex = 4;
      this.chkSummary.Tag = (object) "Summary";
      this.chkSummary.Text = "Summary";
      this.chkSummary.UseVisualStyleBackColor = true;
      this.chkTags.AutoEllipsis = true;
      this.chkTags.Location = new System.Drawing.Point(12, 174);
      this.chkTags.Name = "chkTags";
      this.chkTags.Size = new System.Drawing.Size(120, 17);
      this.chkTags.TabIndex = 22;
      this.chkTags.Tag = (object) "Tags";
      this.chkTags.Text = "Tags";
      this.chkTags.UseVisualStyleBackColor = true;
      this.chkNotes.AutoEllipsis = true;
      this.chkNotes.Location = new System.Drawing.Point(138, 59);
      this.chkNotes.Name = "chkNotes";
      this.chkNotes.Size = new System.Drawing.Size(120, 17);
      this.chkNotes.TabIndex = 5;
      this.chkNotes.Tag = (object) "Notes";
      this.chkNotes.Text = "Notes";
      this.chkNotes.UseVisualStyleBackColor = true;
      this.chkManga.AutoEllipsis = true;
      this.chkManga.Location = new System.Drawing.Point(398, 128);
      this.chkManga.Name = "chkManga";
      this.chkManga.Size = new System.Drawing.Size(120, 17);
      this.chkManga.TabIndex = 17;
      this.chkManga.Tag = (object) "Manga";
      this.chkManga.Text = "Manga";
      this.chkManga.UseVisualStyleBackColor = true;
      this.chkAlternateSeries.AutoEllipsis = true;
      this.chkAlternateSeries.Location = new System.Drawing.Point(138, 81);
      this.chkAlternateSeries.Name = "chkAlternateSeries";
      this.chkAlternateSeries.Size = new System.Drawing.Size(120, 17);
      this.chkAlternateSeries.TabIndex = 9;
      this.chkAlternateSeries.Tag = (object) "AlternateSeries";
      this.chkAlternateSeries.Text = "Alt. Series";
      this.chkAlternateSeries.UseVisualStyleBackColor = true;
      this.chkAlternateNumber.AutoEllipsis = true;
      this.chkAlternateNumber.Location = new System.Drawing.Point(264, 81);
      this.chkAlternateNumber.Name = "chkAlternateNumber";
      this.chkAlternateNumber.Size = new System.Drawing.Size(120, 17);
      this.chkAlternateNumber.TabIndex = 10;
      this.chkAlternateNumber.Tag = (object) "AlternateNumber";
      this.chkAlternateNumber.Text = "Alt. Number";
      this.chkAlternateNumber.UseVisualStyleBackColor = true;
      this.chkAlternateCount.AutoEllipsis = true;
      this.chkAlternateCount.Location = new System.Drawing.Point(398, 81);
      this.chkAlternateCount.Name = "chkAlternateCount";
      this.chkAlternateCount.Size = new System.Drawing.Size(120, 17);
      this.chkAlternateCount.TabIndex = 11;
      this.chkAlternateCount.Tag = (object) "AlternateCount";
      this.chkAlternateCount.Text = "Alt. Total Number";
      this.chkAlternateCount.UseVisualStyleBackColor = true;
      this.chkColor.AutoEllipsis = true;
      this.chkColor.Location = new System.Drawing.Point(398, 174);
      this.chkColor.Name = "chkColor";
      this.chkColor.Size = new System.Drawing.Size(120, 17);
      this.chkColor.TabIndex = 25;
      this.chkColor.Tag = (object) "ColorAdjustment";
      this.chkColor.Text = "Color Adjustment";
      this.chkColor.UseVisualStyleBackColor = true;
      this.chkCommunityRating.AutoEllipsis = true;
      this.chkCommunityRating.Location = new System.Drawing.Point(264, 174);
      this.chkCommunityRating.Name = "chkCommunityRating";
      this.chkCommunityRating.Size = new System.Drawing.Size(120, 17);
      this.chkCommunityRating.TabIndex = 24;
      this.chkCommunityRating.Tag = (object) "CommunityRating";
      this.chkCommunityRating.Text = "Community Rating";
      this.chkCommunityRating.UseVisualStyleBackColor = true;
      this.chkAgeRating.AutoEllipsis = true;
      this.chkAgeRating.Location = new System.Drawing.Point(138, 128);
      this.chkAgeRating.Name = "chkAgeRating";
      this.chkAgeRating.Size = new System.Drawing.Size(120, 17);
      this.chkAgeRating.TabIndex = 15;
      this.chkAgeRating.Tag = (object) "AgeRating";
      this.chkAgeRating.Text = "Age Rating";
      this.chkAgeRating.UseVisualStyleBackColor = true;
      this.chkFormat.AutoEllipsis = true;
      this.chkFormat.Location = new System.Drawing.Point(12, 128);
      this.chkFormat.Name = "chkFormat";
      this.chkFormat.Size = new System.Drawing.Size(120, 17);
      this.chkFormat.TabIndex = 14;
      this.chkFormat.Tag = (object) "Format";
      this.chkFormat.Text = "Format";
      this.chkFormat.UseVisualStyleBackColor = true;
      this.chkBlackAndWhite.AutoEllipsis = true;
      this.chkBlackAndWhite.Location = new System.Drawing.Point(398, 151);
      this.chkBlackAndWhite.Name = "chkBlackAndWhite";
      this.chkBlackAndWhite.Size = new System.Drawing.Size(120, 17);
      this.chkBlackAndWhite.TabIndex = 21;
      this.chkBlackAndWhite.Tag = (object) "BlackAndWhite";
      this.chkBlackAndWhite.Text = "Black and White";
      this.chkBlackAndWhite.UseVisualStyleBackColor = true;
      this.chkLocations.AutoEllipsis = true;
      this.chkLocations.Location = new System.Drawing.Point(398, 36);
      this.chkLocations.Name = "chkLocations";
      this.chkLocations.Size = new System.Drawing.Size(120, 17);
      this.chkLocations.TabIndex = 3;
      this.chkLocations.Tag = (object) "Locations";
      this.chkLocations.Text = "Locations";
      this.chkLocations.UseVisualStyleBackColor = true;
      this.chkTeams.AutoEllipsis = true;
      this.chkTeams.Location = new System.Drawing.Point(264, 36);
      this.chkTeams.Name = "chkTeams";
      this.chkTeams.Size = new System.Drawing.Size(120, 17);
      this.chkTeams.TabIndex = 2;
      this.chkTeams.Tag = (object) "Teams";
      this.chkTeams.Text = "Teams";
      this.chkTeams.UseVisualStyleBackColor = true;
      this.chkWeb.AutoEllipsis = true;
      this.chkWeb.Location = new System.Drawing.Point(138, 82);
      this.chkWeb.Name = "chkWeb";
      this.chkWeb.Size = new System.Drawing.Size(120, 17);
      this.chkWeb.TabIndex = 8;
      this.chkWeb.Tag = (object) "Web";
      this.chkWeb.Text = "Web";
      this.chkWeb.UseVisualStyleBackColor = true;
      this.chkCharacters.AutoEllipsis = true;
      this.chkCharacters.Location = new System.Drawing.Point(138, 36);
      this.chkCharacters.Name = "chkCharacters";
      this.chkCharacters.Size = new System.Drawing.Size(120, 17);
      this.chkCharacters.TabIndex = 1;
      this.chkCharacters.Tag = (object) "Characters";
      this.chkCharacters.Text = "Characters";
      this.chkCharacters.UseVisualStyleBackColor = true;
      this.chkBookCollectionStatus.AutoEllipsis = true;
      this.chkBookCollectionStatus.Location = new System.Drawing.Point(264, 59);
      this.chkBookCollectionStatus.Name = "chkBookCollectionStatus";
      this.chkBookCollectionStatus.Size = new System.Drawing.Size(120, 17);
      this.chkBookCollectionStatus.TabIndex = 6;
      this.chkBookCollectionStatus.Tag = (object) "BookCollectionStatus";
      this.chkBookCollectionStatus.Text = "Collection Status";
      this.chkBookCollectionStatus.UseVisualStyleBackColor = true;
      this.chkBookNotes.AutoEllipsis = true;
      this.chkBookNotes.Location = new System.Drawing.Point(138, 59);
      this.chkBookNotes.Name = "chkBookNotes";
      this.chkBookNotes.Size = new System.Drawing.Size(120, 17);
      this.chkBookNotes.TabIndex = 5;
      this.chkBookNotes.Tag = (object) "BookNotes";
      this.chkBookNotes.Text = "Notes";
      this.chkBookNotes.UseVisualStyleBackColor = true;
      this.chkBookCondition.AutoEllipsis = true;
      this.chkBookCondition.Location = new System.Drawing.Point(12, 59);
      this.chkBookCondition.Name = "chkBookCondition";
      this.chkBookCondition.Size = new System.Drawing.Size(120, 17);
      this.chkBookCondition.TabIndex = 4;
      this.chkBookCondition.Tag = (object) "BookCondition";
      this.chkBookCondition.Text = "Condition";
      this.chkBookCondition.UseVisualStyleBackColor = true;
      this.chkBookLocation.AutoEllipsis = true;
      this.chkBookLocation.Location = new System.Drawing.Point(264, 36);
      this.chkBookLocation.Name = "chkBookLocation";
      this.chkBookLocation.Size = new System.Drawing.Size(120, 17);
      this.chkBookLocation.TabIndex = 2;
      this.chkBookLocation.Tag = (object) "BookLocation";
      this.chkBookLocation.Text = "Location";
      this.chkBookLocation.UseVisualStyleBackColor = true;
      this.chkBookOwner.AutoEllipsis = true;
      this.chkBookOwner.Location = new System.Drawing.Point(398, 59);
      this.chkBookOwner.Name = "chkBookOwner";
      this.chkBookOwner.Size = new System.Drawing.Size(120, 17);
      this.chkBookOwner.TabIndex = 7;
      this.chkBookOwner.Tag = (object) "BookOwner";
      this.chkBookOwner.Text = "Owner";
      this.chkBookOwner.UseVisualStyleBackColor = true;
      this.chkBookStore.AutoEllipsis = true;
      this.chkBookStore.Location = new System.Drawing.Point(12, 36);
      this.chkBookStore.Name = "chkBookStore";
      this.chkBookStore.Size = new System.Drawing.Size(120, 17);
      this.chkBookStore.TabIndex = 0;
      this.chkBookStore.Tag = (object) "BookStore";
      this.chkBookStore.Text = "Store";
      this.chkBookStore.UseVisualStyleBackColor = true;
      this.chkBookPrice.AutoEllipsis = true;
      this.chkBookPrice.Location = new System.Drawing.Point(138, 36);
      this.chkBookPrice.Name = "chkBookPrice";
      this.chkBookPrice.Size = new System.Drawing.Size(120, 17);
      this.chkBookPrice.TabIndex = 1;
      this.chkBookPrice.Tag = (object) "BookPrice";
      this.chkBookPrice.Text = "Price";
      this.chkBookPrice.UseVisualStyleBackColor = true;
      this.btMarkDefined.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btMarkDefined.Location = new System.Drawing.Point(87, 389);
      this.btMarkDefined.Name = "btMarkDefined";
      this.btMarkDefined.Size = new System.Drawing.Size(70, 23);
      this.btMarkDefined.TabIndex = 3;
      this.btMarkDefined.Text = "Only &Set";
      this.btMarkDefined.UseVisualStyleBackColor = true;
      this.btMarkDefined.Click += new EventHandler(this.btMarkDefined_Click);
      this.btMarkAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btMarkAll.Location = new System.Drawing.Point(11, 389);
      this.btMarkAll.Name = "btMarkAll";
      this.btMarkAll.Size = new System.Drawing.Size(70, 23);
      this.btMarkAll.TabIndex = 2;
      this.btMarkAll.Text = "&All";
      this.btMarkAll.UseVisualStyleBackColor = true;
      this.btMarkAll.Click += new EventHandler(this.btMarkAll_Click);
      this.btMarkNone.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btMarkNone.Location = new System.Drawing.Point(163, 389);
      this.btMarkNone.Name = "btMarkNone";
      this.btMarkNone.Size = new System.Drawing.Size(70, 23);
      this.btMarkNone.TabIndex = 4;
      this.btMarkNone.Text = "&Clear";
      this.btMarkNone.UseVisualStyleBackColor = true;
      this.btMarkNone.Click += new EventHandler(this.btMarkNone_Click);
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
      this.pageData.Size = new System.Drawing.Size(541, 371);
      this.pageData.TabIndex = 0;
      this.grpCustom.Dock = DockStyle.Top;
      this.grpCustom.Location = new System.Drawing.Point(0, 548);
      this.grpCustom.Name = "grpCustom";
      this.grpCustom.Size = new System.Drawing.Size(522, 90);
      this.grpCustom.TabIndex = 4;
      this.grpCustom.Text = "Custom";
      this.grpCatalog.Controls.Add((Control) this.chkReleasedTime);
      this.grpCatalog.Controls.Add((Control) this.chkAddedTime);
      this.grpCatalog.Controls.Add((Control) this.chkOpenedTime);
      this.grpCatalog.Controls.Add((Control) this.chkPageCount);
      this.grpCatalog.Controls.Add((Control) this.chkISBN);
      this.grpCatalog.Controls.Add((Control) this.chkBookAge);
      this.grpCatalog.Controls.Add((Control) this.chkBookNotes);
      this.grpCatalog.Controls.Add((Control) this.chkBookCollectionStatus);
      this.grpCatalog.Controls.Add((Control) this.chkBookStore);
      this.grpCatalog.Controls.Add((Control) this.chkBookPrice);
      this.grpCatalog.Controls.Add((Control) this.chkBookCondition);
      this.grpCatalog.Controls.Add((Control) this.chkBookOwner);
      this.grpCatalog.Controls.Add((Control) this.chkBookLocation);
      this.grpCatalog.Dock = DockStyle.Top;
      this.grpCatalog.Location = new System.Drawing.Point(0, 415);
      this.grpCatalog.Name = "grpCatalog";
      this.grpCatalog.Size = new System.Drawing.Size(522, 133);
      this.grpCatalog.TabIndex = 3;
      this.grpCatalog.Text = "Catalog";
      this.chkReleasedTime.AutoEllipsis = true;
      this.chkReleasedTime.Location = new System.Drawing.Point(12, 103);
      this.chkReleasedTime.Name = "chkReleasedTime";
      this.chkReleasedTime.Size = new System.Drawing.Size(120, 17);
      this.chkReleasedTime.TabIndex = 10;
      this.chkReleasedTime.Tag = (object) "ReleasedTime";
      this.chkReleasedTime.Text = "Released";
      this.chkReleasedTime.UseVisualStyleBackColor = true;
      this.chkAddedTime.AutoEllipsis = true;
      this.chkAddedTime.Location = new System.Drawing.Point(138, 103);
      this.chkAddedTime.Name = "chkAddedTime";
      this.chkAddedTime.Size = new System.Drawing.Size(120, 17);
      this.chkAddedTime.TabIndex = 11;
      this.chkAddedTime.Tag = (object) "AddedTime";
      this.chkAddedTime.Text = "Added/Purchased";
      this.chkAddedTime.UseVisualStyleBackColor = true;
      this.chkOpenedTime.AutoEllipsis = true;
      this.chkOpenedTime.Location = new System.Drawing.Point(264, 103);
      this.chkOpenedTime.Name = "chkOpenedTime";
      this.chkOpenedTime.Size = new System.Drawing.Size(120, 17);
      this.chkOpenedTime.TabIndex = 12;
      this.chkOpenedTime.Tag = (object) "OpenedTime";
      this.chkOpenedTime.Text = "Opened/Read";
      this.chkOpenedTime.UseVisualStyleBackColor = true;
      this.chkPageCount.AutoEllipsis = true;
      this.chkPageCount.Location = new System.Drawing.Point(138, 82);
      this.chkPageCount.Name = "chkPageCount";
      this.chkPageCount.Size = new System.Drawing.Size(120, 17);
      this.chkPageCount.TabIndex = 9;
      this.chkPageCount.Tag = (object) "PagesCount";
      this.chkPageCount.Text = "Pages";
      this.chkPageCount.UseVisualStyleBackColor = true;
      this.chkISBN.AutoEllipsis = true;
      this.chkISBN.Location = new System.Drawing.Point(12, 82);
      this.chkISBN.Name = "chkISBN";
      this.chkISBN.Size = new System.Drawing.Size(120, 17);
      this.chkISBN.TabIndex = 8;
      this.chkISBN.Tag = (object) "ISBN";
      this.chkISBN.Text = "ISBN";
      this.chkISBN.UseVisualStyleBackColor = true;
      this.chkBookAge.AutoEllipsis = true;
      this.chkBookAge.Location = new System.Drawing.Point(398, 36);
      this.chkBookAge.Name = "chkBookAge";
      this.chkBookAge.Size = new System.Drawing.Size(120, 17);
      this.chkBookAge.TabIndex = 3;
      this.chkBookAge.Tag = (object) "BookAge";
      this.chkBookAge.Text = "Age";
      this.chkBookAge.UseVisualStyleBackColor = true;
      this.grpPlotNotes.Controls.Add((Control) this.chkScanInformation);
      this.grpPlotNotes.Controls.Add((Control) this.chkReview);
      this.grpPlotNotes.Controls.Add((Control) this.chkNotes);
      this.grpPlotNotes.Controls.Add((Control) this.chkSummary);
      this.grpPlotNotes.Controls.Add((Control) this.chkLocations);
      this.grpPlotNotes.Controls.Add((Control) this.chkWeb);
      this.grpPlotNotes.Controls.Add((Control) this.chkMainCharacterOrTeam);
      this.grpPlotNotes.Controls.Add((Control) this.chkCharacters);
      this.grpPlotNotes.Controls.Add((Control) this.chkTeams);
      this.grpPlotNotes.Dock = DockStyle.Top;
      this.grpPlotNotes.Location = new System.Drawing.Point(0, 297);
      this.grpPlotNotes.Name = "grpPlotNotes";
      this.grpPlotNotes.Size = new System.Drawing.Size(522, 118);
      this.grpPlotNotes.TabIndex = 2;
      this.grpPlotNotes.Text = "Plot & Notes";
      this.chkScanInformation.AutoEllipsis = true;
      this.chkScanInformation.Location = new System.Drawing.Point(12, 82);
      this.chkScanInformation.Name = "chkScanInformation";
      this.chkScanInformation.Size = new System.Drawing.Size(120, 17);
      this.chkScanInformation.TabIndex = 7;
      this.chkScanInformation.Tag = (object) "ScanInformation";
      this.chkScanInformation.Text = "Scan Information";
      this.chkScanInformation.UseVisualStyleBackColor = true;
      this.chkReview.AutoEllipsis = true;
      this.chkReview.Location = new System.Drawing.Point(264, 59);
      this.chkReview.Name = "chkReview";
      this.chkReview.Size = new System.Drawing.Size(120, 17);
      this.chkReview.TabIndex = 6;
      this.chkReview.Tag = (object) "Review";
      this.chkReview.Text = "Review";
      this.chkReview.UseVisualStyleBackColor = true;
      this.chkMainCharacterOrTeam.AutoEllipsis = true;
      this.chkMainCharacterOrTeam.Location = new System.Drawing.Point(12, 36);
      this.chkMainCharacterOrTeam.Name = "chkMainCharacterOrTeam";
      this.chkMainCharacterOrTeam.Size = new System.Drawing.Size(120, 17);
      this.chkMainCharacterOrTeam.TabIndex = 0;
      this.chkMainCharacterOrTeam.Tag = (object) "MainCharacterOrTeam";
      this.chkMainCharacterOrTeam.Text = "Main Character";
      this.chkMainCharacterOrTeam.UseVisualStyleBackColor = true;
      this.grpArtists.Controls.Add((Control) this.chkEditor);
      this.grpArtists.Controls.Add((Control) this.chkCover);
      this.grpArtists.Controls.Add((Control) this.chkLetterer);
      this.grpArtists.Controls.Add((Control) this.chkColorist);
      this.grpArtists.Controls.Add((Control) this.chkInker);
      this.grpArtists.Controls.Add((Control) this.chkPenciller);
      this.grpArtists.Controls.Add((Control) this.chkWriter);
      this.grpArtists.Dock = DockStyle.Top;
      this.grpArtists.Location = new System.Drawing.Point(0, 203);
      this.grpArtists.Name = "grpArtists";
      this.grpArtists.Size = new System.Drawing.Size(522, 94);
      this.grpArtists.TabIndex = 1;
      this.grpArtists.Text = "Artists / People Involved";
      this.grpMain.Controls.Add((Control) this.chkDay);
      this.grpMain.Controls.Add((Control) this.chkSeriesGroup);
      this.grpMain.Controls.Add((Control) this.chkStoryArc);
      this.grpMain.Controls.Add((Control) this.chkSeriesComplete);
      this.grpMain.Controls.Add((Control) this.chkCommunityRating);
      this.grpMain.Controls.Add((Control) this.chkColor);
      this.grpMain.Controls.Add((Control) this.chkRating);
      this.grpMain.Controls.Add((Control) this.chkSeries);
      this.grpMain.Controls.Add((Control) this.chkVolume);
      this.grpMain.Controls.Add((Control) this.chkTags);
      this.grpMain.Controls.Add((Control) this.chkBlackAndWhite);
      this.grpMain.Controls.Add((Control) this.chkNumber);
      this.grpMain.Controls.Add((Control) this.chkLanguage);
      this.grpMain.Controls.Add((Control) this.chkAlternateCount);
      this.grpMain.Controls.Add((Control) this.chkAgeRating);
      this.grpMain.Controls.Add((Control) this.chkImprint);
      this.grpMain.Controls.Add((Control) this.chkGenre);
      this.grpMain.Controls.Add((Control) this.chkManga);
      this.grpMain.Controls.Add((Control) this.chkPublisher);
      this.grpMain.Controls.Add((Control) this.chkCount);
      this.grpMain.Controls.Add((Control) this.chkFormat);
      this.grpMain.Controls.Add((Control) this.chkAlternateSeries);
      this.grpMain.Controls.Add((Control) this.chkAlternateNumber);
      this.grpMain.Controls.Add((Control) this.chkTitle);
      this.grpMain.Controls.Add((Control) this.chkMonth);
      this.grpMain.Controls.Add((Control) this.chkYear);
      this.grpMain.Dock = DockStyle.Top;
      this.grpMain.Location = new System.Drawing.Point(0, 0);
      this.grpMain.Name = "grpMain";
      this.grpMain.Size = new System.Drawing.Size(522, 203);
      this.grpMain.TabIndex = 0;
      this.grpMain.Text = "Main";
      this.chkDay.AutoEllipsis = true;
      this.chkDay.Location = new System.Drawing.Point(398, 58);
      this.chkDay.Name = "chkDay";
      this.chkDay.Size = new System.Drawing.Size(107, 17);
      this.chkDay.TabIndex = 7;
      this.chkDay.Tag = (object) "Day";
      this.chkDay.Text = "Day";
      this.chkDay.UseVisualStyleBackColor = true;
      this.chkSeriesGroup.AutoEllipsis = true;
      this.chkSeriesGroup.Location = new System.Drawing.Point(138, 104);
      this.chkSeriesGroup.Name = "chkSeriesGroup";
      this.chkSeriesGroup.Size = new System.Drawing.Size(120, 17);
      this.chkSeriesGroup.TabIndex = 13;
      this.chkSeriesGroup.Tag = (object) "SeriesGroup";
      this.chkSeriesGroup.Text = "Series Group";
      this.chkSeriesGroup.UseVisualStyleBackColor = true;
      this.chkStoryArc.AutoEllipsis = true;
      this.chkStoryArc.Location = new System.Drawing.Point(12, 104);
      this.chkStoryArc.Name = "chkStoryArc";
      this.chkStoryArc.Size = new System.Drawing.Size(120, 17);
      this.chkStoryArc.TabIndex = 12;
      this.chkStoryArc.Tag = (object) "StoryArc";
      this.chkStoryArc.Text = "Story Arc";
      this.chkStoryArc.UseVisualStyleBackColor = true;
      this.chkSeriesComplete.AutoEllipsis = true;
      this.chkSeriesComplete.Location = new System.Drawing.Point(12, 81);
      this.chkSeriesComplete.Name = "chkSeriesComplete";
      this.chkSeriesComplete.Size = new System.Drawing.Size(120, 17);
      this.chkSeriesComplete.TabIndex = 8;
      this.chkSeriesComplete.Tag = (object) "SeriesComplete";
      this.chkSeriesComplete.Text = "Series complete";
      this.chkSeriesComplete.UseVisualStyleBackColor = true;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(563, 422);
      this.Controls.Add((Control) this.pageData);
      this.Controls.Add((Control) this.btMarkNone);
      this.Controls.Add((Control) this.btMarkAll);
      this.Controls.Add((Control) this.btMarkDefined);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ComicDataPasteDialog);
      this.RightToLeftLayout = true;
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Paste Data to {0} Books";
      this.pageData.ResumeLayout(false);
      this.grpCatalog.ResumeLayout(false);
      this.grpPlotNotes.ResumeLayout(false);
      this.grpArtists.ResumeLayout(false);
      this.grpMain.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Controls.SearchBrowserControl
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Win32;
using cYo.Common.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Controls
{
  public class SearchBrowserControl : UserControl
  {
    public const int ColumnCount = 3;
    private static readonly char[] listSeparators = new char[2]
    {
      ',',
      ';'
    };
    private readonly SearchBrowserControl.SelectionInfo[] workingSelectionInfos = new SearchBrowserControl.SelectionInfo[3];
    private readonly string allText = TR.Load("SearchBrowser")["AllItems", "All ({0} {1})"];
    private readonly ComicBookCollection books = new ComicBookCollection();
    private string unspecifiedText = TR.Load("SearchBrowser")["Unspecified", "Unspecified"];
    private volatile bool listIsDirty;
    private volatile bool shieldIndex;
    private bool shieldNot;
    private Bitmap dragBitmap;
    private IBitmapCursor dragCursor;
    private IContainer components;
    private ListViewEx listView1;
    private ListViewEx listView2;
    private ListViewEx listView3;
    private ComboBox cbType1;
    private ComboBox cbType2;
    private ComboBox cbType3;
    private CheckBox btNot1;
    private CheckBox btNot2;
    private CheckBox btNot3;
    private ToolTip toolTip;

    public SearchBrowserControl()
    {
      this.InitializeComponent();
      SearchBrowserControl.FillTypeCombo(this.cbType1, 0, (ListView) this.listView1, this.btNot1, 1);
      SearchBrowserControl.FillTypeCombo(this.cbType2, 1, (ListView) this.listView2, this.btNot2, 0);
      SearchBrowserControl.FillTypeCombo(this.cbType3, 2, (ListView) this.listView3, this.btNot3, 2);
      ListStyles.SetOwnerDrawn((ListView) this.listView1);
      ListStyles.SetOwnerDrawn((ListView) this.listView2);
      ListStyles.SetOwnerDrawn((ListView) this.listView3);
      KeySearch.Create((ListView) this.listView1);
      KeySearch.Create((ListView) this.listView2);
      KeySearch.Create((ListView) this.listView3);
      string caption = TR.Default["LogicNot", "Negation"];
      this.toolTip.SetToolTip((Control) this.btNot1, caption);
      this.toolTip.SetToolTip((Control) this.btNot2, caption);
      this.toolTip.SetToolTip((Control) this.btNot3, caption);
    }

    private static void FillTypeCombo(
      ComboBox cb,
      int index,
      ListView lv,
      CheckBox chk,
      int selected)
    {
      cb.Items.AddRange((object[]) new SearchBrowserControl.SelectionEntry[35]
      {
        new SearchBrowserControl.SelectionEntry(index, 0, "ShadowSeries", "Series", typeof (ComicBookSeriesMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 1, "ShadowTitle", "Titles", typeof (ComicBookTitleMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 2, "Genre", "Genres", typeof (ComicBookGenreMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 3, "AlternateSeries", "Alternate Series", typeof (ComicBookAlternateSeriesMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 4, "ShadowFormat", "Formats", typeof (ComicBookFormatMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 5, "ShadowYearAsText", "Years", typeof (ComicBookYearMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 6, "MonthAsText", "Months", typeof (ComicBookMonthMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 7, "LanguageAsText", "Languages", typeof (ComicBookLanguageMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 8, "AgeRating", "Age Ratings", typeof (ComicBookAgeRatingMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 9, "RatingAsText", "My Ratings", typeof (ComicBookRatingMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 10, "Writer", "Writers", typeof (ComicBookWriterMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 11, "Penciller", "Pencillers", typeof (ComicBookPencillerMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 12, "Inker", "Inkers", typeof (ComicBookInkerMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 13, "Colorist", "Colorists", typeof (ComicBookColoristMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 14, "CoverArtist", "Cover Artists", typeof (ComicBookCoverArtistMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 15, "Editor", "Editors", typeof (ComicBookEditorMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 16, "Publisher", "Publishers", typeof (ComicBookPublisherMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 17, "Imprint", "Imprints", typeof (ComicBookImprintMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 18, "Characters", "Characters", typeof (ComicBookCharactersMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 19, "Tags", "Tags", typeof (ComicBookTagsMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 20, "ShadowVolumeAsText", "Volumes", typeof (ComicBookVolumeMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 21, "Teams", "Teams", typeof (ComicBookTeamsMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 22, "Locations", "Locations", typeof (ComicBookLocationsMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 23, "Letterer", "Letterers", typeof (ComicBookLettererMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 24, "CommunityRatingAsText", "Community Ratings", typeof (ComicBookCommunityRatingMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 25, "BookPriceAsText", "Book Prices", typeof (ComicBookBookPriceMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 26, "BookAge", "Book Ages", typeof (ComicBookBookAgeMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 27, "BookStore", "Book Stores", typeof (ComicBookBookStoreMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 28, "BookOwner", "Book Owners", typeof (ComicBookBookOwnerMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 29, "BookCondition", "Book Conditions", typeof (ComicBookBookConditionMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 30, "BookCollectionStatus", "Book Collection Status", typeof (ComicBookBookCollectionStatusMatcher), true),
        new SearchBrowserControl.SelectionEntry(index, 31, "BookLocation", "Book Locations", typeof (ComicBookBookLocationMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 32, "MainCharacterOrTeam", "Main Characters/Teams", typeof (ComicBookMainCharacterOrTeamMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 33, "SeriesGroup", "Series Group", typeof (ComicBookSeriesGroupMatcher), false),
        new SearchBrowserControl.SelectionEntry(index, 34, "StoryArc", "Story Arcs", typeof (ComicBookStoryArcMatcher), false)
      }.Sort<SearchBrowserControl.SelectionEntry>());
      cb.Tag = (object) lv;
      lv.Tag = (object) chk;
      chk.Tag = (object) index;
      cb.DisplayMember = "Caption";
      SearchBrowserControl.SetSelectedIndex(cb, selected);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.books.Clear();
        this.books.Changed -= new EventHandler<SmartListChangedEventArgs<ComicBook>>(this.BooksChanged);
        IdleProcess.Idle -= new EventHandler(this.IdleUpdate);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public ComicBookCollection Books => this.books;

    public ComicBookMatcher CurrentMatcher
    {
      get => this.GetMatcherUpTo((SearchBrowserControl.SelectionInfo) null);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Column1
    {
      get => SearchBrowserControl.GetSelectedIndex(this.cbType1);
      set => SearchBrowserControl.SetSelectedIndex(this.cbType1, value);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Column2
    {
      get => SearchBrowserControl.GetSelectedIndex(this.cbType2);
      set => SearchBrowserControl.SetSelectedIndex(this.cbType2, value);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Column3
    {
      get => SearchBrowserControl.GetSelectedIndex(this.cbType3);
      set => SearchBrowserControl.SetSelectedIndex(this.cbType3, value);
    }

    [DefaultValue("Unspecified")]
    [Localizable(true)]
    public string UnspecifiedText
    {
      get => this.unspecifiedText;
      set => this.unspecifiedText = value;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DesignMode)
        return;
      this.books.Changed += new EventHandler<SmartListChangedEventArgs<ComicBook>>(this.BooksChanged);
      IdleProcess.Idle += new EventHandler(this.IdleUpdate);
    }

    private static void SafeSetBounds(Control c, int x, int y, int width, int height)
    {
      if (c.Left == x && c.Top == y && c.Width == width && c.Height == height)
        return;
      c.SetBounds(x, y, width, height);
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
      int num = this.DisplayRectangle.Width / 3;
      int height = this.DisplayRectangle.Height;
      int width1 = this.DisplayRectangle.Width;
      int width2 = this.btNot1.Width;
      int y = 0 + this.cbType1.Height + 2;
      SearchBrowserControl.SetListViewBounds((ListView) this.listView1, 0, y, num - 2, height);
      SearchBrowserControl.SetListViewBounds((ListView) this.listView2, num + 2, y, num - 2, height);
      SearchBrowserControl.SetListViewBounds((ListView) this.listView3, width1 - (num - 2), y, num - 2, height);
      SearchBrowserControl.SafeSetBounds((Control) this.cbType1, this.listView1.Left, 0, this.listView1.Width - 1 - width2, this.cbType1.Height);
      SearchBrowserControl.SafeSetBounds((Control) this.cbType2, this.listView2.Left, 0, this.listView2.Width - 1 - width2, this.cbType2.Height);
      SearchBrowserControl.SafeSetBounds((Control) this.cbType3, this.listView3.Left, 0, this.listView3.Width - 1 - width2, this.cbType3.Height);
      SearchBrowserControl.SafeSetBounds((Control) this.btNot1, this.cbType1.Right + 1, -1, width2, this.cbType1.Height);
      SearchBrowserControl.SafeSetBounds((Control) this.btNot2, this.cbType2.Right + 1, -1, width2, this.cbType2.Height);
      SearchBrowserControl.SafeSetBounds((Control) this.btNot3, this.cbType3.Right + 1, -1, width2, this.cbType3.Height);
      base.OnLayout(levent);
    }

    private static void SetListViewBounds(ListView lv, int x, int y, int w, int h)
    {
      SearchBrowserControl.SafeSetBounds((Control) lv, x, y, w, h - y);
      if (lv.Columns.Count <= 0)
        return;
      ColumnHeader column1 = lv.Columns[0];
      Rectangle displayRectangle = lv.DisplayRectangle;
      int num = displayRectangle.Width - SystemInformation.VerticalScrollBarWidth;
      column1.Width = num;
      ColumnHeader column2 = lv.Columns[0];
      displayRectangle = lv.DisplayRectangle;
      int width = displayRectangle.Width;
      column2.Width = width;
    }

    private void BooksChanged(object sender, SmartListChangedEventArgs<ComicBook> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          e.Item.BookChanged += new EventHandler<BookChangedEventArgs>(this.BookPropertyChanged);
          break;
        case SmartListAction.Remove:
          e.Item.BookChanged -= new EventHandler<BookChangedEventArgs>(this.BookPropertyChanged);
          break;
      }
      this.listIsDirty = true;
    }

    private void BookPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.listIsDirty = ((IEnumerable<SearchBrowserControl.SelectionInfo>) this.workingSelectionInfos).Any<SearchBrowserControl.SelectionInfo>((Func<SearchBrowserControl.SelectionInfo, bool>) (si => si.Property == e.PropertyName));
    }

    private void IdleUpdate(object sender, EventArgs e) => this.UpdateLists();

    private void ListItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      this.HandleSelectionChange(sender as ListView);
    }

    private void ListViewVirtualItemsSelectionRangeChanged(
      object sender,
      ListViewVirtualItemsSelectionRangeChangedEventArgs e)
    {
      this.HandleSelectionChange(sender as ListView);
    }

    private void HandleSelectionChange(ListView lv)
    {
      if (this.shieldIndex)
        return;
      SearchBrowserControl.SelectionInfo tag = (SearchBrowserControl.SelectionInfo) lv.Tag;
      if (!this.CheckSelection(tag))
        return;
      if (tag.Index < 2)
        this.BuildList(tag.Index + 1);
      this.OnCurrentMatcherChanged();
    }

    private void ListViewRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
    {
      SearchBrowserControl.SelectionInfo tag = (SearchBrowserControl.SelectionInfo) ((Control) sender).Tag;
      ListViewItem listViewItem;
      if (tag.CachedItems.TryGetValue(e.ItemIndex, out listViewItem))
      {
        e.Item = listViewItem;
      }
      else
      {
        string str = tag.Items[e.ItemIndex];
        e.Item = new ListViewItem(string.IsNullOrEmpty(str) ? this.UnspecifiedText : str)
        {
          Tag = (object) str
        };
        tag.CachedItems.Add(e.ItemIndex, e.Item);
      }
    }

    private void cbType_SelectedIndexChanged(object sender, EventArgs e)
    {
      ComboBox comboBox = (ComboBox) sender;
      SearchBrowserControl.SelectionEntry selectedItem = (SearchBrowserControl.SelectionEntry) comboBox.SelectedItem;
      ListView tag = comboBox.Tag as ListView;
      this.workingSelectionInfos[selectedItem.Index] = new SearchBrowserControl.SelectionInfo(selectedItem, tag, this.GetNotButton(selectedItem.Index).Checked);
      this.listIsDirty = true;
    }

    private void btNot_CheckedChanged(object sender, EventArgs e)
    {
      if (this.shieldNot)
        return;
      CheckBox checkBox = (CheckBox) sender;
      SearchBrowserControl.SelectionInfo workingSelectionInfo = this.workingSelectionInfos[(int) checkBox.Tag];
      workingSelectionInfo.Not = checkBox.Checked;
      if (workingSelectionInfo.Index < 2)
        this.BuildList(workingSelectionInfo.Index + 1);
      this.OnCurrentMatcherChanged();
    }

    public event EventHandler CurrentMatcherChanged;

    public event ItemDragEventHandler ItemDrag;

    protected virtual void OnCurrentMatcherChanged()
    {
      if (this.CurrentMatcherChanged == null)
        return;
      this.CurrentMatcherChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnItemDrag(MouseButtons buttons, object data)
    {
      if (this.ItemDrag == null)
        return;
      this.ItemDrag((object) this, new ItemDragEventArgs(buttons, data));
    }

    public void UpdateLists()
    {
      while (this.listIsDirty)
      {
        this.listIsDirty = false;
        this.BuildLists();
      }
    }

    public SearchBrowserControl.SelectionEntry GetSelectionColumn(int column)
    {
      switch (column)
      {
        case 0:
          return this.cbType1.SelectedItem as SearchBrowserControl.SelectionEntry;
        case 1:
          return this.cbType2.SelectedItem as SearchBrowserControl.SelectionEntry;
        case 2:
          return this.cbType3.SelectedItem as SearchBrowserControl.SelectionEntry;
        default:
          return (SearchBrowserControl.SelectionEntry) null;
      }
    }

    public void SelectEntry(int column, string value)
    {
      switch (column)
      {
        case 0:
          SearchBrowserControl.SelectListEntry((ListView) this.listView1, value);
          break;
        case 1:
          SearchBrowserControl.SelectListEntry((ListView) this.listView2, value);
          break;
        case 2:
          SearchBrowserControl.SelectListEntry((ListView) this.listView3, value);
          break;
      }
    }

    public void ClearNot()
    {
      try
      {
        this.shieldNot = true;
        this.btNot1.Checked = this.btNot2.Checked = this.btNot3.Checked = false;
      }
      finally
      {
        this.shieldNot = false;
      }
    }

    private CheckBox GetNotButton(int column)
    {
      return new CheckBox[3]
      {
        this.btNot1,
        this.btNot2,
        this.btNot3
      }[column];
    }

    private static int GetSelectedIndex(ComboBox cb)
    {
      return ((SearchBrowserControl.SelectionEntry) cb.SelectedItem).Id;
    }

    private static void SetSelectedIndex(ComboBox cb, int id)
    {
      for (int index = 0; index < cb.Items.Count; ++index)
      {
        if (cb.Items[index] is SearchBrowserControl.SelectionEntry selectionEntry && selectionEntry.Id == id)
        {
          cb.SelectedIndex = index;
          return;
        }
      }
      cb.SelectedIndex = 0;
    }

    private ComicBookMatcher GetMatcherUpTo(SearchBrowserControl.SelectionInfo end)
    {
      ComicBookGroupMatcher bookGroupMatcher = new ComicBookGroupMatcher();
      if (this.books == null || this.books.Count == 0)
        return (ComicBookMatcher) null;
      foreach (SearchBrowserControl.SelectionInfo workingSelectionInfo in this.workingSelectionInfos)
      {
        ComicBookMatcher matcher = SearchBrowserControl.CreateMatcher(workingSelectionInfo);
        if (matcher != null)
          bookGroupMatcher.Matchers.Add(matcher);
        if (workingSelectionInfo == end)
          break;
      }
      return bookGroupMatcher.Optimized();
    }

    private bool CheckSelection(SearchBrowserControl.SelectionInfo si)
    {
      this.shieldIndex = true;
      ListView listView = si.ListView;
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) si.SelectedItems);
      try
      {
        if (listView.Items[0].Selected)
        {
          for (int index = 1; index < listView.Items.Count; ++index)
            listView.Items[index].Selected = false;
          si.SelectedItems.Clear();
        }
        else
        {
          for (int index = 1; index < listView.Items.Count; ++index)
          {
            if (listView.Items[index].Selected)
              si.SelectedItems.Add(si.Items[index]);
            else
              si.SelectedItems.Remove(si.Items[index]);
          }
          if (si.SelectedItems.Count > 0)
            listView.Items[0].Selected = false;
        }
      }
      finally
      {
        this.shieldIndex = false;
      }
      return stringSet != si.SelectedItems;
    }

    private void BuildList(int start)
    {
      int num = -1;
      IEnumerable<ComicBook> items = (IEnumerable<ComicBook>) this.books;
      MatcherSet<ComicBook> matcherSet = new MatcherSet<ComicBook>();
      if (items == null)
        items = (IEnumerable<ComicBook>) new ComicBook[0];
      foreach (SearchBrowserControl.SelectionInfo workingSelectionInfo in this.workingSelectionInfos)
      {
        if (++num >= start)
        {
          if (matcherSet.Matchers.Count > 0)
            items = (IEnumerable<ComicBook>) matcherSet.Match(items).ToArray<ComicBook>();
          HashSet<string> stringSet = EngineConfiguration.Default.SearchBrowserCaseSensitive ? new HashSet<string>() : new HashSet<string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
          foreach (ComicBook comicBook in items)
          {
            string stringPropertyValue = comicBook.GetStringPropertyValue(workingSelectionInfo.Property, ComicValueType.Shadow);
            if (!workingSelectionInfo.MultipleValues || stringPropertyValue.IndexOfAny(SearchBrowserControl.listSeparators) == -1)
              stringSet.Add(stringPropertyValue.Trim());
            else
              stringSet.AddRange<string>(((IEnumerable<string>) stringPropertyValue.Split(SearchBrowserControl.listSeparators)).Select<string, string>((Func<string, string>) (vs => vs.Trim())));
          }
          List<string> list = stringSet.ToList<string>();
          list.Sort((IComparer<string>) new ExtendedStringComparer(ExtendedStringComparison.IgnoreArticles | ExtendedStringComparison.IgnoreCase));
          list.Insert(0, StringUtility.Format(this.allText, (object) list.Count, (object) workingSelectionInfo.Caption));
          if (!workingSelectionInfo.Items.SequenceEqual<string>((IEnumerable<string>) list, (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase))
            SearchBrowserControl.FillSelectonInfoList(workingSelectionInfo, list);
        }
        IMatcher<ComicBook> matcher = (IMatcher<ComicBook>) SearchBrowserControl.CreateMatcher(workingSelectionInfo);
        if (matcher != null)
        {
          if (workingSelectionInfo.Not)
            matcherSet.AndNot(matcher);
          else
            matcherSet.And(matcher);
        }
      }
    }

    private void BuildLists() => this.BuildList(0);

    private static void FillSelectonInfoList(
      SearchBrowserControl.SelectionInfo si,
      List<string> names)
    {
      ListView listView = si.ListView;
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) si.SelectedItems);
      si.SelectedItems.Clear();
      si.CachedItems.Clear();
      listView.BeginUpdate();
      try
      {
        si.Items = names;
        listView.TopItem = (ListViewItem) null;
        listView.VirtualListSize = 0;
        listView.VirtualMode = false;
        listView.VirtualMode = true;
        listView.VirtualListSize = names.Count;
        for (int index = 1; index < names.Count; ++index)
        {
          if (stringSet.Contains(names[index]))
          {
            listView.Items[index].Selected = true;
            si.SelectedItems.Add(names[index]);
          }
        }
        if (listView.Items.Count <= 0 || listView.SelectedIndices.Count != 0)
          return;
        listView.Items[0].Selected = true;
      }
      catch (Exception ex)
      {
      }
      finally
      {
        listView.EndUpdate();
      }
    }

    private static ComicBookMatcher CreateMatcher(SearchBrowserControl.SelectionInfo si)
    {
      switch (si.SelectedItems.Count)
      {
        case 0:
          return (ComicBookMatcher) null;
        case 1:
          ComicBookValueMatcher matcher = ComicBookValueMatcher.Create(si.MatcherType, si.MultipleValues ? 6 : 0, si.SelectedItems.First<string>(), (string) null);
          matcher.Not = si.Not;
          return (ComicBookMatcher) matcher;
        default:
          ComicBookGroupMatcher bookGroupMatcher = new ComicBookGroupMatcher();
          bookGroupMatcher.MatcherMode = MatcherMode.Or;
          bookGroupMatcher.Not = si.Not;
          ComicBookGroupMatcher subSet = bookGroupMatcher;
          si.SelectedItems.ForEach<string>((Action<string>) (s => subSet.Matchers.Add(si.MatcherType, si.MultipleValues ? 6 : 0, s, (string) null)));
          return (ComicBookMatcher) subSet;
      }
    }

    private static void SelectListEntry(ListView lv, string text)
    {
      for (int index = 0; index < lv.Items.Count; ++index)
        lv.Items[index].Selected = false;
      if (SearchBrowserControl.SelectListEntryInternal(lv, text))
        return;
      foreach (string str in text.Split(SearchBrowserControl.listSeparators))
        SearchBrowserControl.SelectListEntryInternal(lv, str.Trim());
    }

    private static bool SelectListEntryInternal(ListView lv, string text)
    {
      if (string.IsNullOrEmpty(text))
        return false;
      for (int index = 0; index < lv.Items.Count; ++index)
      {
        try
        {
          ListViewItem listViewItem = lv.Items[index];
          if (object.Equals((object) text, listViewItem.Tag))
          {
            listViewItem.Selected = true;
            listViewItem.EnsureVisible();
            return true;
          }
        }
        catch (Exception ex)
        {
        }
      }
      return false;
    }

    private void listView_ItemDrag(object sender, ItemDragEventArgs e)
    {
      SearchBrowserControl.SelectionInfo tag = ((Control) sender).Tag as SearchBrowserControl.SelectionInfo;
      ComicBookMatcher matcher = SearchBrowserControl.CreateMatcher(tag);
      ComicBookContainer data1 = new ComicBookContainer(((ListViewItem) e.Item).Text);
      DataObject data2 = new DataObject();
      if (matcher == null)
      {
        data1.Books.AddRange((IEnumerable<ComicBook>) this.books);
      }
      else
      {
        data1.Books.AddRange(matcher.Match((IEnumerable<ComicBook>) this.books));
        data2.SetData("ComicBookMatcher", (object) matcher);
      }
      data2.SetData((object) data1);
      this.dragBitmap = this.CreateDragCursor(tag);
      this.dragCursor = BitmapCursor.Create(this.dragBitmap, new System.Drawing.Point(10, 10));
      try
      {
        this.OnItemDrag(e.Button, (object) data2);
      }
      catch
      {
      }
      finally
      {
        this.dragBitmap.Dispose();
        if (this.dragCursor != null)
          this.dragCursor.Dispose();
      }
    }

    private void GiveDragFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if (this.dragCursor == null || this.dragCursor.Cursor == (Cursor) null)
        return;
      e.UseDefaultCursors = false;
      this.dragCursor.OverlayCursor = e.Effect == DragDropEffects.None ? Cursors.No : Cursors.Default;
      Cursor.Current = this.dragCursor.Cursor;
    }

    private Bitmap CreateDragCursor(SearchBrowserControl.SelectionInfo si)
    {
      ListView listView = si.ListView;
      List<string> list = si.SelectedItems.Take<string>(10).ToList<string>();
      if (list.Count == 0)
        list.Add(si.Items[0]);
      list.Sort();
      int height = this.Font.Height;
      Rectangle rect = new Rectangle(0, 0, listView.Width, height * list.Count + 4);
      Bitmap bitmap = new Bitmap(rect.Width + 1, rect.Height + 1);
      Rectangle layoutRectangle = rect;
      layoutRectangle.Inflate(-2, -2);
      using (Graphics graphics = Graphics.FromImage((Image) bitmap))
      {
        graphics.Clear(Color.White);
        using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap))
        {
          using (Brush brush = (Brush) new SolidBrush(this.ForeColor))
          {
            for (int index = 0; index < list.Count; ++index)
            {
              graphics.DrawString(list[index], this.Font, brush, (RectangleF) layoutRectangle, format);
              layoutRectangle.Y += height;
            }
          }
        }
        using (Pen pen = new Pen(this.ForeColor))
          graphics.DrawRectangle(pen, rect);
        bitmap.ChangeAlpha((byte) 128);
      }
      return bitmap;
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.listView2 = new ListViewEx();
      this.listView3 = new ListViewEx();
      this.listView1 = new ListViewEx();
      this.cbType1 = new ComboBox();
      this.cbType2 = new ComboBox();
      this.cbType3 = new ComboBox();
      this.btNot1 = new CheckBox();
      this.btNot2 = new CheckBox();
      this.btNot3 = new CheckBox();
      this.toolTip = new ToolTip(this.components);
      this.SuspendLayout();
      this.listView2.FullRowSelect = true;
      this.listView2.HeaderStyle = ColumnHeaderStyle.None;
      this.listView2.HideSelection = false;
      this.listView2.Location = new System.Drawing.Point(191, 32);
      this.listView2.Name = "listView2";
      this.listView2.Size = new System.Drawing.Size(178, 162);
      this.listView2.TabIndex = 3;
      this.listView2.UseCompatibleStateImageBehavior = false;
      this.listView2.View = View.Details;
      this.listView2.VirtualMode = true;
      this.listView2.ItemDrag += new ItemDragEventHandler(this.listView_ItemDrag);
      this.listView2.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.ListItemSelectionChanged);
      this.listView2.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(this.ListViewRetrieveVirtualItem);
      this.listView2.VirtualItemsSelectionRangeChanged += new ListViewVirtualItemsSelectionRangeChangedEventHandler(this.ListViewVirtualItemsSelectionRangeChanged);
      this.listView3.FullRowSelect = true;
      this.listView3.HeaderStyle = ColumnHeaderStyle.None;
      this.listView3.HideSelection = false;
      this.listView3.Location = new System.Drawing.Point(375, 32);
      this.listView3.Name = "listView3";
      this.listView3.Size = new System.Drawing.Size(202, 162);
      this.listView3.TabIndex = 5;
      this.listView3.UseCompatibleStateImageBehavior = false;
      this.listView3.View = View.Details;
      this.listView3.VirtualMode = true;
      this.listView3.ItemDrag += new ItemDragEventHandler(this.listView_ItemDrag);
      this.listView3.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.ListItemSelectionChanged);
      this.listView3.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(this.ListViewRetrieveVirtualItem);
      this.listView3.VirtualItemsSelectionRangeChanged += new ListViewVirtualItemsSelectionRangeChangedEventHandler(this.ListViewVirtualItemsSelectionRangeChanged);
      this.listView1.FullRowSelect = true;
      this.listView1.HeaderStyle = ColumnHeaderStyle.None;
      this.listView1.HideSelection = false;
      this.listView1.Location = new System.Drawing.Point(16, 32);
      this.listView1.Name = "listView1";
      this.listView1.Size = new System.Drawing.Size(169, 162);
      this.listView1.TabIndex = 1;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = View.Details;
      this.listView1.VirtualMode = true;
      this.listView1.ItemDrag += new ItemDragEventHandler(this.listView_ItemDrag);
      this.listView1.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.ListItemSelectionChanged);
      this.listView1.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(this.ListViewRetrieveVirtualItem);
      this.listView1.VirtualItemsSelectionRangeChanged += new ListViewVirtualItemsSelectionRangeChangedEventHandler(this.ListViewVirtualItemsSelectionRangeChanged);
      this.cbType1.BackColor = SystemColors.Window;
      this.cbType1.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbType1.FormattingEnabled = true;
      this.cbType1.Location = new System.Drawing.Point(15, 7);
      this.cbType1.Name = "cbType1";
      this.cbType1.Size = new System.Drawing.Size(145, 21);
      this.cbType1.TabIndex = 0;
      this.cbType1.SelectedIndexChanged += new EventHandler(this.cbType_SelectedIndexChanged);
      this.cbType2.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbType2.FormattingEnabled = true;
      this.cbType2.Location = new System.Drawing.Point(190, 7);
      this.cbType2.Name = "cbType2";
      this.cbType2.Size = new System.Drawing.Size(157, 21);
      this.cbType2.TabIndex = 2;
      this.cbType2.SelectedIndexChanged += new EventHandler(this.cbType_SelectedIndexChanged);
      this.cbType3.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbType3.FormattingEnabled = true;
      this.cbType3.Location = new System.Drawing.Point(375, 7);
      this.cbType3.Name = "cbType3";
      this.cbType3.Size = new System.Drawing.Size(172, 21);
      this.cbType3.TabIndex = 4;
      this.cbType3.SelectedIndexChanged += new EventHandler(this.cbType_SelectedIndexChanged);
      this.btNot1.Appearance = Appearance.Button;
      this.btNot1.AutoSize = true;
      this.btNot1.Location = new System.Drawing.Point(166, 7);
      this.btNot1.Name = "btNot1";
      this.btNot1.Size = new System.Drawing.Size(20, 23);
      this.btNot1.TabIndex = 6;
      this.btNot1.Tag = (object) "0";
      this.btNot1.Text = "!";
      this.btNot1.TextAlign = ContentAlignment.MiddleCenter;
      this.btNot1.UseVisualStyleBackColor = true;
      this.btNot1.CheckedChanged += new EventHandler(this.btNot_CheckedChanged);
      this.btNot2.Appearance = Appearance.Button;
      this.btNot2.AutoSize = true;
      this.btNot2.Location = new System.Drawing.Point(349, 7);
      this.btNot2.Name = "btNot2";
      this.btNot2.Size = new System.Drawing.Size(20, 23);
      this.btNot2.TabIndex = 7;
      this.btNot2.Tag = (object) "1";
      this.btNot2.Text = "!";
      this.btNot2.TextAlign = ContentAlignment.MiddleCenter;
      this.btNot2.UseVisualStyleBackColor = true;
      this.btNot2.CheckedChanged += new EventHandler(this.btNot_CheckedChanged);
      this.btNot3.Appearance = Appearance.Button;
      this.btNot3.AutoSize = true;
      this.btNot3.Location = new System.Drawing.Point(557, 7);
      this.btNot3.Name = "btNot3";
      this.btNot3.Size = new System.Drawing.Size(20, 23);
      this.btNot3.TabIndex = 8;
      this.btNot3.Tag = (object) "2";
      this.btNot3.Text = "!";
      this.btNot3.TextAlign = ContentAlignment.MiddleCenter;
      this.btNot3.UseVisualStyleBackColor = true;
      this.btNot3.CheckedChanged += new EventHandler(this.btNot_CheckedChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.btNot3);
      this.Controls.Add((Control) this.btNot2);
      this.Controls.Add((Control) this.btNot1);
      this.Controls.Add((Control) this.cbType3);
      this.Controls.Add((Control) this.cbType2);
      this.Controls.Add((Control) this.cbType1);
      this.Controls.Add((Control) this.listView2);
      this.Controls.Add((Control) this.listView3);
      this.Controls.Add((Control) this.listView1);
      this.DoubleBuffered = true;
      this.Name = nameof (SearchBrowserControl);
      this.Size = new System.Drawing.Size(586, 272);
      this.GiveFeedback += new GiveFeedbackEventHandler(this.GiveDragFeedback);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public class SelectionEntry : IComparable<SearchBrowserControl.SelectionEntry>
    {
      public int Index { get; private set; }

      public int Id { get; private set; }

      public string Property { get; private set; }

      public Type MatcherType { get; private set; }

      public bool MultipleValues { get; private set; }

      public string Caption { get; private set; }

      public SelectionEntry(
        int index,
        int id,
        string property,
        string caption,
        Type matcherType,
        bool multiValue)
      {
        this.Index = index;
        this.Id = id;
        this.Property = property;
        this.Caption = TR.Load("SearchBrowser")[property, caption];
        this.MatcherType = matcherType;
        this.MultipleValues = multiValue;
      }

      public int CompareTo(SearchBrowserControl.SelectionEntry other)
      {
        return string.Compare(this.Caption, other.Caption);
      }
    }

    private class SelectionInfo : SearchBrowserControl.SelectionEntry
    {
      public readonly ListView ListView;
      public readonly HashSet<string> SelectedItems;
      public readonly Dictionary<int, ListViewItem> CachedItems;
      public List<string> Items;
      public bool Not;

      public SelectionInfo(
        int index,
        ListView listView,
        int id,
        string property,
        string caption,
        Type matcherType,
        bool multiValue,
        bool not)
        : base(index, id, property, caption, matcherType, multiValue)
      {
        this.ListView = listView;
        this.Items = new List<string>();
        this.SelectedItems = new HashSet<string>();
        this.CachedItems = new Dictionary<int, ListViewItem>();
        this.Not = not;
        listView.Tag = (object) this;
        listView.VirtualListSize = 0;
        listView.Clear();
        listView.Columns.Add(this.Caption);
        listView.Columns[0].Width = listView.Width - 20;
      }

      public SelectionInfo(SearchBrowserControl.SelectionEntry se, ListView lv, bool not)
        : this(se.Index, lv, se.Id, se.Property, se.Caption, se.MatcherType, se.MultipleValues, not)
      {
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.SmartListQueryDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class SmartListQueryDialog : Form, ISmartListDialog
  {
    private ComicSmartListItem smartComicList;
    private CursorList<SmartListQueryDialog.UndoItem> undoList = new CursorList<SmartListQueryDialog.UndoItem>();
    private string coloredText;
    private IContainer components;
    private Panel panel1;
    private RichTextBox rtfQuery;
    private Button btNext;
    private Button btPrev;
    private Button btOK;
    private Button btCancel;
    private Button btApply;
    private Label labelStatus;
    private Label labelPosition;
    private Timer colorizeTimer;
    private Timer undoTimer;
    private ContextMenuStrip cmEdit;
    private ToolStripMenuItem miUndo;
    private ToolStripMenuItem miRedo;
    private ToolStripSeparator toolStripMenuItem3;
    private ToolStripMenuItem miCut;
    private ToolStripMenuItem miCopy;
    private ToolStripMenuItem miPaste;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem miSelectAll;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem miQuickFormat;
    private ToolStripSeparator toolStripMenuItem4;
    private ToolStripMenuItem miInsertMatch;
    private ToolStripMenuItem miInsertValue;
    private Button btDesigner;

    public SmartListQueryDialog()
    {
      this.InitializeComponent();
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.RestorePosition();
      LocalizeUtility.Localize((Control) this, typeof (SmartListDialog).Name, this.components);
      LocalizeUtility.Localize(TR.Load("TextBoxContextMenu"), (ToolStrip) this.cmEdit);
      ContextMenuBuilder contextMenuBuilder1 = new ContextMenuBuilder();
      foreach (IComicBookValueMatcher availableMatcher in ComicBookValueMatcher.GetAvailableMatchers())
        contextMenuBuilder1.Add(availableMatcher.Description, false, false, new EventHandler(this.OnInsertQuery), (object) availableMatcher, DateTime.MinValue);
      this.miInsertMatch.DropDownItems.AddRange(contextMenuBuilder1.Create(20));
      ContextMenuBuilder contextMenuBuilder2 = new ContextMenuBuilder();
      foreach (string str in ((IEnumerable<string>) ComicBookMatcher.ComicProperties).Concat<string>((IEnumerable<string>) ComicBookMatcher.SeriesStatsProperties))
        contextMenuBuilder2.Add(str, false, false, new EventHandler(this.OnInsertField), (object) str, DateTime.MinValue);
      this.miInsertValue.DropDownItems.AddRange(contextMenuBuilder2.Create(20));
    }

    public ComicLibrary Library { get; set; }

    public Guid EditId { get; set; }

    public ComicSmartListItem SmartComicList
    {
      get
      {
        ComicSmartListItem smartComicList = (ComicSmartListItem) null;
        try
        {
          smartComicList = new ComicSmartListItem(string.Empty, this.rtfQuery.Text, this.Library);
        }
        catch (Exception ex)
        {
        }
        if (smartComicList == null)
          return this.smartComicList;
        string name = smartComicList.Name;
        smartComicList.Id = this.smartComicList.Id;
        smartComicList.CopyExtraValues(this.smartComicList);
        if (!string.IsNullOrEmpty(name))
          smartComicList.Name = name;
        return smartComicList;
      }
      set
      {
        this.smartComicList = value;
        value.Library = this.Library;
        this.rtfQuery.Text = this.smartComicList.ToString();
        this.undoList = new CursorList<SmartListQueryDialog.UndoItem>();
        this.TextHasChanged();
        this.Colorize(true, true);
      }
    }

    public bool EnableNavigation
    {
      get => this.btPrev.Visible;
      set
      {
        this.btPrev.Visible = this.btNext.Visible = value;
        if (value)
          this.btDesigner.Left = this.btNext.Right + (this.btNext.Left - this.btPrev.Right);
        else
          this.btDesigner.Left = this.btPrev.Left;
      }
    }

    public bool PreviousEnabled
    {
      get => this.btPrev.Enabled;
      set => this.btPrev.Enabled = value;
    }

    public bool NextEnabled
    {
      get => this.btNext.Enabled;
      set => this.btNext.Enabled = value;
    }

    public event EventHandler Apply;

    public event EventHandler Next;

    public event EventHandler Previous;

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.rtfQuery.AutoWordSelection = false;
    }

    private void OnInsertQuery(object sender, EventArgs e)
    {
      this.rtfQuery.SelectedText = "[" + (((ToolStripItem) sender).Tag as IComicBookValueMatcher).DescriptionNeutral + "]";
    }

    private void OnInsertField(object sender, EventArgs e)
    {
      this.rtfQuery.SelectedText = "{" + (string) (sender as ToolStripMenuItem).Tag + "}";
    }

    private void btApply_Click(object sender, EventArgs e)
    {
      if (this.Apply == null)
        return;
      this.Apply((object) this, EventArgs.Empty);
    }

    private void btOK_Click(object sender, EventArgs e)
    {
      if (this.Apply == null)
        return;
      this.Apply((object) this, EventArgs.Empty);
    }

    private void btPrev_Click(object sender, EventArgs e)
    {
      if (this.Apply != null)
        this.Apply((object) this, EventArgs.Empty);
      if (this.Previous == null)
        return;
      this.Previous((object) this, EventArgs.Empty);
    }

    private void btNext_Click(object sender, EventArgs e)
    {
      if (this.Apply != null)
        this.Apply((object) this, EventArgs.Empty);
      if (this.Next == null)
        return;
      this.Next((object) this, EventArgs.Empty);
    }

    private void rtfQuery_SelectionChanged(object sender, EventArgs e)
    {
      this.UpdatePositionDisplay();
    }

    private void rtfQuery_TextChanged(object sender, EventArgs e)
    {
      if (this.rtfQuery.Text.Contains("\t"))
      {
        using (this.rtfQuery.SuspendPainting())
          this.rtfQuery.Text = this.rtfQuery.Text.Replace("\t", "    ");
      }
      this.TextHasChanged();
      this.Colorize(false);
    }

    private void colorizeTimer_Tick(object sender, EventArgs e)
    {
      this.colorizeTimer.Stop();
      this.Colorize(true);
    }

    private void undoTimer_Tick(object sender, EventArgs e)
    {
      this.undoTimer.Stop();
      this.AddUndo();
    }

    private void rtfQuery_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Tab:
          e.Handled = true;
          this.InsertText(new string(' ', 4));
          break;
        case Keys.Return:
          e.Handled = true;
          this.InsertText("\n" + new string(' ', Math.Max(0, this.rtfQuery.Lines[this.rtfQuery.GetLineFromCharIndex(this.rtfQuery.SelectionStart)].FindIndex<char>((Predicate<char>) (c => !char.IsWhiteSpace(c))))));
          break;
        case Keys.Space:
          if (!e.Modifiers.HasFlag((Enum) Keys.Control))
            break;
          e.Handled = true;
          Tokenizer.Token token = this.GetCurrentToken(this.rtfQuery.SelectionStart);
          if (token == null || !token.Text.StartsWith("["))
            break;
          string t = token.Text.Substring(0, this.rtfQuery.SelectionStart - token.Index);
          t = token.Text.Trim('[', ']', ' ', ',', '"', ';');
          ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
          ContextMenuBuilder contextMenuBuilder = new ContextMenuBuilder();
          foreach (IComicBookValueMatcher tag in ComicBookValueMatcher.GetAvailableMatchers().Where<IComicBookValueMatcher>((Func<IComicBookValueMatcher, bool>) (m => m.DescriptionNeutral.StartsWith(t, StringComparison.OrdinalIgnoreCase))))
          {
            IComicBookValueMatcher m = tag;
            contextMenuBuilder.Add(tag.Description, false, false, (EventHandler) ((s, ea) =>
            {
              this.rtfQuery.Select(token.Index, token.Length);
              this.rtfQuery.SelectedText = "[" + m.DescriptionNeutral + "]";
            }), (object) tag, DateTime.MinValue);
          }
          if (contextMenuBuilder.Count == 0)
            break;
          contextMenuStrip.Items.AddRange(contextMenuBuilder.Create(20));
          System.Drawing.Point positionFromCharIndex = this.rtfQuery.GetPositionFromCharIndex(this.rtfQuery.SelectionStart);
          positionFromCharIndex.Y += 20;
          contextMenuStrip.Show((Control) this, positionFromCharIndex);
          break;
        case Keys.Y:
          if (!e.Modifiers.HasFlag((Enum) Keys.Control))
            break;
          e.Handled = true;
          this.Redo();
          break;
        case Keys.Z:
          if (!e.Modifiers.HasFlag((Enum) Keys.Control))
            break;
          e.Handled = true;
          this.Undo();
          break;
      }
    }

    private void rtfQuery_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar != '\t')
        return;
      e.Handled = true;
    }

    private void rtfQuery_DoubleClick(object sender, EventArgs e)
    {
      Tokenizer.Token currentToken = this.GetCurrentToken(this.rtfQuery.GetCharIndexFromPosition(this.rtfQuery.PointToClient(Cursor.Position)));
      if (currentToken == null || currentToken.Text.StartsWith("\""))
        return;
      this.rtfQuery.Select(currentToken.Index, currentToken.Length);
    }

    private void cmEdit_Opening(object sender, CancelEventArgs e)
    {
      this.miUndo.Enabled = this.CanUndo();
      this.miRedo.Enabled = this.CanRedo();
      this.miCopy.Enabled = this.miCut.Enabled = this.rtfQuery.SelectionLength > 0;
      this.miPaste.Enabled = Clipboard.ContainsText();
    }

    private void miUndo_Click(object sender, EventArgs e) => this.Undo();

    private void miRedo_Click(object sender, EventArgs e) => this.Redo();

    private void miCut_Click(object sender, EventArgs e) => this.rtfQuery.Cut();

    private void miCopy_Click(object sender, EventArgs e) => this.rtfQuery.Copy();

    private void miPaste_Click(object sender, EventArgs e) => this.rtfQuery.Paste();

    private void miSelectAll_Click(object sender, EventArgs e) => this.rtfQuery.SelectAll();

    private void miQuickFormat_Click(object sender, EventArgs e)
    {
      try
      {
        this.rtfQuery.Text = new ComicSmartListItem(string.Empty, this.rtfQuery.Text, this.Library).ToString();
        this.Colorize(true, true);
      }
      catch (Exception ex)
      {
      }
    }

    private void AddUndo()
    {
      this.AddUndo(this.rtfQuery.Rtf, this.rtfQuery.SelectionStart, this.rtfQuery.SelectionLength);
    }

    private void AddUndo(string text, int selectionStart, int selectionLength)
    {
      if (this.undoList.CursorValue != null && !(this.undoList.CursorValue.Text != text) && this.undoList.CursorValue.SelectionStart == selectionStart && this.undoList.CursorValue.SelectionLength == selectionLength)
        return;
      this.undoList.AddAtCursor(new SmartListQueryDialog.UndoItem()
      {
        Text = text,
        SelectionStart = selectionStart,
        SelectionLength = selectionLength
      });
    }

    private bool CanUndo() => this.undoList.CanMoveCursorPrevious;

    private void Undo()
    {
      if (!this.CanUndo())
        return;
      this.undoList.MoveCursorPrevious();
      SmartListQueryDialog.UndoItem cursorValue = this.undoList.CursorValue;
      if (cursorValue.Text != null)
      {
        using (this.rtfQuery.SuspendPainting())
          this.rtfQuery.Rtf = cursorValue.Text;
      }
      this.rtfQuery.SelectionStart = cursorValue.SelectionStart;
      this.rtfQuery.SelectionLength = cursorValue.SelectionLength;
      this.coloredText = this.rtfQuery.Text;
    }

    private bool CanRedo() => this.undoList.CanMoveCursorNext;

    private void Redo()
    {
      if (!this.CanRedo())
        return;
      this.undoList.MoveCursorNext();
      SmartListQueryDialog.UndoItem cursorValue = this.undoList.CursorValue;
      if (cursorValue.Text != null)
      {
        using (this.rtfQuery.SuspendPainting())
          this.rtfQuery.Rtf = cursorValue.Text;
      }
      this.rtfQuery.SelectionStart = cursorValue.SelectionStart;
      this.rtfQuery.SelectionLength = cursorValue.SelectionLength;
      this.coloredText = this.rtfQuery.Text;
    }

    private Tokenizer.Token GetCurrentToken(int n)
    {
      return ComicSmartListItem.TokenizeQuery(this.rtfQuery.Text).GetAll().FirstOrDefault<Tokenizer.Token>((Func<Tokenizer.Token, bool>) (t => n >= t.Index && n <= t.Index + t.Length));
    }

    private void InsertText(string text)
    {
      using (this.rtfQuery.SuspendPainting())
        this.rtfQuery.SelectedText = text;
      this.rtfQuery.SelectionStart += text.Length;
      this.rtfQuery.SelectionLength = 0;
    }

    private void TextHasChanged()
    {
      this.UpdatePositionDisplay();
      this.colorizeTimer.Stop();
      this.colorizeTimer.Start();
      this.undoTimer.Stop();
      this.undoTimer.Start();
    }

    private void UpdatePositionDisplay()
    {
      int line;
      int column;
      StringUtility.ConvertIndexToLineAndColumn(this.rtfQuery.Text, this.rtfQuery.SelectionStart, out line, out column);
      this.labelPosition.Text = string.Format("{0}:{1}", (object) line, (object) column);
      this.undoTimer.Stop();
      this.undoTimer.Start();
    }

    public void Colorize(bool all, bool forced = false)
    {
      if (!forced && this.rtfQuery.Text == this.coloredText)
        return;
      if (all)
        this.coloredText = this.rtfQuery.Text;
      Tokenizer.ParseException parseException = (Tokenizer.ParseException) null;
      try
      {
        this.labelStatus.Text = "OK";
        ComicSmartListItem comicSmartListItem = new ComicSmartListItem(string.Empty, this.rtfQuery.Text, this.Library);
        this.btDesigner.Enabled = this.btOK.Enabled = this.btApply.Enabled = true;
      }
      catch (Exception ex)
      {
        parseException = ex as Tokenizer.ParseException;
        this.labelStatus.Text = ex.Message;
        this.btDesigner.Enabled = this.btOK.Enabled = this.btApply.Enabled = false;
      }
      using (this.rtfQuery.SuspendPainting())
      {
        if (all)
        {
          this.rtfQuery.SelectAll();
          this.rtfQuery.SelectionBackColor = SystemColors.Window;
          this.rtfQuery.SelectionColor = SystemColors.WindowText;
        }
        Tokenizer tokenizer = ComicSmartListItem.TokenizeQuery(this.rtfQuery.Text);
        int selectionStart = this.rtfQuery.SelectionStart;
        foreach (Tokenizer.Token token in tokenizer.GetAll())
        {
          Color color = SystemColors.WindowText;
          string lower = token.Text.ToLower();
          if (lower.StartsWith("\""))
            color = Color.Red;
          else if (lower.StartsWith("["))
          {
            color = Color.Green;
          }
          else
          {
            switch (lower)
            {
              case "match":
              case "in":
              case nameof (all):
              case "any":
              case "name":
                color = Color.Blue;
                break;
              case "not":
                color = Color.DarkRed;
                break;
            }
          }
          if (all || selectionStart > token.Index - 5 && selectionStart < token.Index + token.Length + 5)
          {
            this.rtfQuery.Select(token.Index, token.Length);
            this.rtfQuery.SelectionColor = color;
          }
        }
        if (parseException != null)
        {
          if (parseException.Token != null)
          {
            this.rtfQuery.Select(parseException.Token.Index, parseException.Token.Length);
            this.rtfQuery.SelectionBackColor = Color.LightGray;
          }
        }
      }
      this.rtfQuery.ClearUndo();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.panel1 = new Panel();
      this.rtfQuery = new RichTextBox();
      this.cmEdit = new ContextMenuStrip(this.components);
      this.miUndo = new ToolStripMenuItem();
      this.miRedo = new ToolStripMenuItem();
      this.toolStripMenuItem3 = new ToolStripSeparator();
      this.miCut = new ToolStripMenuItem();
      this.miCopy = new ToolStripMenuItem();
      this.miPaste = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.miSelectAll = new ToolStripMenuItem();
      this.toolStripMenuItem2 = new ToolStripSeparator();
      this.miQuickFormat = new ToolStripMenuItem();
      this.toolStripMenuItem4 = new ToolStripSeparator();
      this.miInsertMatch = new ToolStripMenuItem();
      this.miInsertValue = new ToolStripMenuItem();
      this.btNext = new Button();
      this.btPrev = new Button();
      this.btOK = new Button();
      this.btCancel = new Button();
      this.btApply = new Button();
      this.labelStatus = new Label();
      this.labelPosition = new Label();
      this.colorizeTimer = new Timer(this.components);
      this.undoTimer = new Timer(this.components);
      this.btDesigner = new Button();
      this.panel1.SuspendLayout();
      this.cmEdit.SuspendLayout();
      this.SuspendLayout();
      this.panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.panel1.BorderStyle = BorderStyle.FixedSingle;
      this.panel1.Controls.Add((Control) this.rtfQuery);
      this.panel1.Location = new System.Drawing.Point(8, 10);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(573, 408);
      this.panel1.TabIndex = 0;
      this.rtfQuery.AcceptsTab = true;
      this.rtfQuery.BorderStyle = BorderStyle.None;
      this.rtfQuery.ContextMenuStrip = this.cmEdit;
      this.rtfQuery.DetectUrls = false;
      this.rtfQuery.Dock = DockStyle.Fill;
      this.rtfQuery.Font = new Font("Courier New", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.rtfQuery.HideSelection = false;
      this.rtfQuery.Location = new System.Drawing.Point(0, 0);
      this.rtfQuery.Name = "rtfQuery";
      this.rtfQuery.Size = new System.Drawing.Size(571, 406);
      this.rtfQuery.TabIndex = 0;
      this.rtfQuery.Text = "";
      this.rtfQuery.WordWrap = false;
      this.rtfQuery.SelectionChanged += new EventHandler(this.rtfQuery_SelectionChanged);
      this.rtfQuery.TextChanged += new EventHandler(this.rtfQuery_TextChanged);
      this.rtfQuery.DoubleClick += new EventHandler(this.rtfQuery_DoubleClick);
      this.rtfQuery.KeyDown += new KeyEventHandler(this.rtfQuery_KeyDown);
      this.rtfQuery.KeyPress += new KeyPressEventHandler(this.rtfQuery_KeyPress);
      this.cmEdit.Items.AddRange(new ToolStripItem[13]
      {
        (ToolStripItem) this.miUndo,
        (ToolStripItem) this.miRedo,
        (ToolStripItem) this.toolStripMenuItem3,
        (ToolStripItem) this.miCut,
        (ToolStripItem) this.miCopy,
        (ToolStripItem) this.miPaste,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.miSelectAll,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.miQuickFormat,
        (ToolStripItem) this.toolStripMenuItem4,
        (ToolStripItem) this.miInsertMatch,
        (ToolStripItem) this.miInsertValue
      });
      this.cmEdit.Name = "cmEdit";
      this.cmEdit.Size = new System.Drawing.Size(187, 226);
      this.cmEdit.Opening += new CancelEventHandler(this.cmEdit_Opening);
      this.miUndo.Image = (Image) Resources.Undo;
      this.miUndo.Name = "miUndo";
      this.miUndo.ShortcutKeys = Keys.Z | Keys.Control;
      this.miUndo.Size = new System.Drawing.Size(186, 22);
      this.miUndo.Text = "Undo";
      this.miUndo.Click += new EventHandler(this.miUndo_Click);
      this.miRedo.Image = (Image) Resources.Redo;
      this.miRedo.Name = "miRedo";
      this.miRedo.ShortcutKeys = Keys.Y | Keys.Control;
      this.miRedo.Size = new System.Drawing.Size(186, 22);
      this.miRedo.Text = "Redo";
      this.miRedo.Click += new EventHandler(this.miRedo_Click);
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(183, 6);
      this.miCut.Image = (Image) Resources.Cut;
      this.miCut.Name = "miCut";
      this.miCut.ShortcutKeys = Keys.X | Keys.Control;
      this.miCut.Size = new System.Drawing.Size(186, 22);
      this.miCut.Text = "Cut";
      this.miCut.Click += new EventHandler(this.miCut_Click);
      this.miCopy.Image = (Image) Resources.Copy;
      this.miCopy.Name = "miCopy";
      this.miCopy.ShortcutKeys = Keys.C | Keys.Control;
      this.miCopy.Size = new System.Drawing.Size(186, 22);
      this.miCopy.Text = "Copy";
      this.miCopy.Click += new EventHandler(this.miCopy_Click);
      this.miPaste.Name = "miPaste";
      this.miPaste.ShortcutKeys = Keys.V | Keys.Control;
      this.miPaste.Size = new System.Drawing.Size(186, 22);
      this.miPaste.Text = "Paste";
      this.miPaste.Click += new EventHandler(this.miPaste_Click);
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(183, 6);
      this.miSelectAll.Name = "miSelectAll";
      this.miSelectAll.ShortcutKeys = Keys.A | Keys.Control;
      this.miSelectAll.Size = new System.Drawing.Size(186, 22);
      this.miSelectAll.Text = "Select All";
      this.miSelectAll.Click += new EventHandler(this.miSelectAll_Click);
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(183, 6);
      this.miQuickFormat.Name = "miQuickFormat";
      this.miQuickFormat.ShortcutKeys = Keys.F | Keys.Control;
      this.miQuickFormat.Size = new System.Drawing.Size(186, 22);
      this.miQuickFormat.Text = "Quick Format";
      this.miQuickFormat.Click += new EventHandler(this.miQuickFormat_Click);
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(183, 6);
      this.miInsertMatch.Name = "miInsertMatch";
      this.miInsertMatch.Size = new System.Drawing.Size(186, 22);
      this.miInsertMatch.Text = "Insert Match";
      this.miInsertValue.Name = "miInsertValue";
      this.miInsertValue.Size = new System.Drawing.Size(186, 22);
      this.miInsertValue.Text = "Insert Value";
      this.btNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btNext.FlatStyle = FlatStyle.System;
      this.btNext.Location = new System.Drawing.Point(95, 451);
      this.btNext.Name = "btNext";
      this.btNext.Size = new System.Drawing.Size(80, 24);
      this.btNext.TabIndex = 3;
      this.btNext.Text = "&Next";
      this.btNext.Click += new EventHandler(this.btNext_Click);
      this.btPrev.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btPrev.FlatStyle = FlatStyle.System;
      this.btPrev.Location = new System.Drawing.Point(9, 451);
      this.btPrev.Name = "btPrev";
      this.btPrev.Size = new System.Drawing.Size(80, 24);
      this.btPrev.TabIndex = 2;
      this.btPrev.Text = "&Previous";
      this.btPrev.Click += new EventHandler(this.btPrev_Click);
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(331, 451);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 5;
      this.btOK.Text = "&OK";
      this.btOK.Click += new EventHandler(this.btOK_Click);
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(417, 451);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 6;
      this.btCancel.Text = "&Cancel";
      this.btApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btApply.FlatStyle = FlatStyle.System;
      this.btApply.Location = new System.Drawing.Point(503, 451);
      this.btApply.Name = "btApply";
      this.btApply.Size = new System.Drawing.Size(80, 24);
      this.btApply.TabIndex = 7;
      this.btApply.Text = "&Apply";
      this.btApply.Click += new EventHandler(this.btApply_Click);
      this.labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.labelStatus.BorderStyle = BorderStyle.Fixed3D;
      this.labelStatus.Location = new System.Drawing.Point(8, 421);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(523, 20);
      this.labelStatus.TabIndex = 0;
      this.labelStatus.TextAlign = ContentAlignment.MiddleLeft;
      this.labelPosition.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.labelPosition.BorderStyle = BorderStyle.Fixed3D;
      this.labelPosition.Location = new System.Drawing.Point(532, 421);
      this.labelPosition.Name = "labelPosition";
      this.labelPosition.Size = new System.Drawing.Size(49, 20);
      this.labelPosition.TabIndex = 1;
      this.labelPosition.TextAlign = ContentAlignment.MiddleCenter;
      this.colorizeTimer.Interval = 500;
      this.colorizeTimer.Tick += new EventHandler(this.colorizeTimer_Tick);
      this.undoTimer.Enabled = true;
      this.undoTimer.Interval = 1000;
      this.undoTimer.Tick += new EventHandler(this.undoTimer_Tick);
      this.btDesigner.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btDesigner.DialogResult = DialogResult.Retry;
      this.btDesigner.FlatStyle = FlatStyle.System;
      this.btDesigner.Location = new System.Drawing.Point(181, 451);
      this.btDesigner.Name = "btDesigner";
      this.btDesigner.Size = new System.Drawing.Size(80, 24);
      this.btDesigner.TabIndex = 4;
      this.btDesigner.Text = "&Designer";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(591, 487);
      this.Controls.Add((Control) this.btDesigner);
      this.Controls.Add((Control) this.labelPosition);
      this.Controls.Add((Control) this.labelStatus);
      this.Controls.Add((Control) this.btNext);
      this.Controls.Add((Control) this.btPrev);
      this.Controls.Add((Control) this.btOK);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btApply);
      this.Controls.Add((Control) this.panel1);
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(500, 400);
      this.Name = nameof (SmartListQueryDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = SizeGripStyle.Show;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Edit Smart List";
      this.panel1.ResumeLayout(false);
      this.cmEdit.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    private class UndoItem
    {
      public string Text { get; set; }

      public int SelectionStart { get; set; }

      public int SelectionLength { get; set; }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.MatcherGroupEditor
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class MatcherGroupEditor : UserControl, IMatcherEditor
  {
    private static TR TR = TR.Load("SmartListDialog");
    private ComicBookGroupMatcher currentComicBookMatcher;
    private readonly ComicBookMatcherCollection matchers;
    private readonly int level;
    private readonly string rulesText;
    private IContainer components;
    private FlowLayoutPanel matcherControls;
    private ComboBox cbMatchMode;
    private CheckBox chkNot;
    private Label labelSubRules;
    private ToolTip toolTip;
    private Button btEdit;
    private ContextMenuStrip cmEdit;
    private ToolStripMenuItem miNewRule;
    private ToolStripMenuItem miNewGroup;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem miCopy;
    private ToolStripMenuItem miCut;
    private ToolStripMenuItem miPaste;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem miMoveUp;
    private ToolStripMenuItem miMoveDown;
    private ToolStripMenuItem miDelete;
    private CheckBox chkExpanded;

    public MatcherGroupEditor(
      ComicBookMatcherCollection matchers,
      ComicBookGroupMatcher comicBookMatcher,
      int level,
      int width)
    {
      this.InitializeComponent();
      this.chkExpanded.Image = (Image) ((Bitmap) this.chkExpanded.Image).ScaleDpi();
      this.btEdit.Image = (Image) ((Bitmap) this.btEdit.Image).ScaleDpi();
      this.Width = width;
      this.MinimumSize = new System.Drawing.Size(width, 0);
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.toolTip.SetToolTip((Control) this.chkNot, TR.Default["LogicNot", "Negation"]);
      this.Tag = (object) comicBookMatcher;
      this.level = level;
      this.matchers = matchers;
      comicBookMatcher.Matchers.Changed += new EventHandler<SmartListChangedEventArgs<ComicBookMatcher>>(this.OwnMatchers_Changed);
      this.cbMatchMode.Items.AddRange((object[]) MatcherGroupEditor.TR.GetStrings("MatchMode", "All|Any", '|'));
      LocalizeUtility.Localize(MatcherGroupEditor.TR, (Control) this.labelSubRules);
      LocalizeUtility.Localize(TR.Load("MatcherEditor"), (ToolStrip) this.cmEdit);
      this.rulesText = this.labelSubRules.Text;
      this.InitializeMatcher(comicBookMatcher);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.currentComicBookMatcher.Matchers.Changed -= new EventHandler<SmartListChangedEventArgs<ComicBookMatcher>>(this.OwnMatchers_Changed);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void cmEdit_Opening(object sender, CancelEventArgs e)
    {
      this.miNewGroup.Enabled = this.level <= 5;
      this.miDelete.Enabled = this.miCut.Enabled = this.matchers.Count > 1;
      this.miPaste.Enabled = Clipboard.ContainsData("ComicBookMatcher");
      this.miMoveDown.Enabled = this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher) < this.matchers.Count - 1;
      this.miMoveUp.Enabled = this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher) > 0;
    }

    private void miNewRule_Click(object sender, EventArgs e) => this.AddRule();

    private void miNewGroup_Click(object sender, EventArgs e) => this.AddGroup();

    private void miDelete_Click(object sender, EventArgs e) => this.DeleteRuleOrGroup();

    private void miCopy_Click(object sender, EventArgs e) => this.CopyClipboard();

    private void miCut_Click(object sender, EventArgs e) => this.CutClipboard();

    private void miPaste_Click(object sender, EventArgs e) => this.PasteClipboard();

    private void miMoveUp_Click(object sender, EventArgs e) => this.MoveUp();

    private void miMoveDown_Click(object sender, EventArgs e) => this.MoveDown();

    private void btEdit_Click(object sender, EventArgs e)
    {
      this.cmEdit.Show((Control) this.btEdit, new System.Drawing.Point(this.btEdit.Width, this.btEdit.Height), ToolStripDropDownDirection.BelowLeft);
    }

    private void OwnMatchers_Changed(object sender, SmartListChangedEventArgs<ComicBookMatcher> e)
    {
      int dialogEditorOffset = this.DialogEditorOffset;
      switch (e.Action)
      {
        case SmartListAction.Insert:
          if (this.matcherControls.Controls.OfType<Control>().FirstOrDefault<Control>((Func<Control, bool>) (c => c.Tag == e.Item)) == null)
          {
            this.AddMatcherControl(e.Item);
            break;
          }
          break;
        case SmartListAction.Remove:
          Control control = this.matcherControls.Controls.OfType<Control>().FirstOrDefault<Control>((Func<Control, bool>) (cc => cc.Tag == e.Item));
          if (control != null)
          {
            this.matcherControls.Controls.Remove(control);
            break;
          }
          break;
        case SmartListAction.Move:
          Control child = this.matcherControls.Controls.OfType<Control>().FirstOrDefault<Control>((Func<Control, bool>) (cc => cc.Tag == e.Item));
          if (child != null)
          {
            this.matcherControls.Controls.SetChildIndex(child, e.Index);
            break;
          }
          break;
      }
      this.DialogEditorOffset = dialogEditorOffset;
    }

    private void cbMatchMode_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.currentComicBookMatcher.MatcherMode = this.cbMatchMode.SelectedIndex == 0 ? MatcherMode.And : MatcherMode.Or;
    }

    private void chkNot_CheckedChanged(object sender, EventArgs e)
    {
      this.currentComicBookMatcher.Not = this.chkNot.Checked;
    }

    private void chkCollapse_CheckedChanged(object sender, EventArgs e)
    {
      int dialogEditorOffset = this.DialogEditorOffset;
      this.currentComicBookMatcher.Collapsed = !this.chkExpanded.Checked;
      this.matcherControls.Visible = this.chkExpanded.Checked;
      this.labelSubRules.Text = this.chkExpanded.Checked ? this.rulesText : this.Description;
      this.DialogEditorOffset = dialogEditorOffset;
    }

    protected override void OnCreateControl()
    {
      base.OnCreateControl();
      foreach (ComicBookMatcher matcher in (SmartList<ComicBookMatcher>) this.currentComicBookMatcher.Matchers)
        this.AddMatcherControl(matcher);
      this.labelSubRules.Text = this.chkExpanded.Checked ? this.rulesText : this.Description;
    }

    public void AddMatcherControl(ComicBookMatcher icbm)
    {
      FlowLayoutPanel matcherControls1 = this.matcherControls;
      FlowLayoutPanel matcherControls2 = this.matcherControls;
      System.Drawing.Size size1 = new System.Drawing.Size(this.Width - this.matcherControls.Left, 0);
      System.Drawing.Size size2 = size1;
      matcherControls2.MaximumSize = size2;
      System.Drawing.Size size3 = size1;
      matcherControls1.MinimumSize = size3;
      int width = this.matcherControls.Width;
      Control child = icbm is ComicBookGroupMatcher ? this.CreateGroupMatchPanel(icbm as ComicBookGroupMatcher, width) : this.CreateMatchPanel(icbm as ComicBookValueMatcher, width);
      this.matcherControls.Controls.Add(child);
      this.matcherControls.Controls.SetChildIndex(child, this.currentComicBookMatcher.Matchers.IndexOf(icbm));
      this.matcherControls.AutoTabIndex();
    }

    private int DialogEditorOffset
    {
      get
      {
        int dialogEditorOffset = 0;
        if (this.ParentForm is SmartListDialog)
          dialogEditorOffset = ((SmartListDialog) this.ParentForm).DialogEditorOffset;
        return dialogEditorOffset;
      }
      set
      {
        if (!(this.ParentForm is SmartListDialog))
          return;
        ((SmartListDialog) this.ParentForm).DialogEditorOffset = value;
      }
    }

    public Control CreateMatchPanel(ComicBookValueMatcher matcher, int width)
    {
      return (Control) new MatcherEditor(this.currentComicBookMatcher.Matchers, matcher, this.level, width);
    }

    public Control CreateGroupMatchPanel(ComicBookGroupMatcher matcher, int width)
    {
      return (Control) new MatcherGroupEditor(this.currentComicBookMatcher.Matchers, matcher, this.level + 1, width);
    }

    public string Description
    {
      get
      {
        return this.matcherControls.GetControls<MatcherEditor>().Select<MatcherEditor, string>((Func<MatcherEditor, string>) (me => me.Description.Ellipsis(20, "..."))).ToListString(", ");
      }
    }

    private void InitializeMatcher(ComicBookGroupMatcher comicBookMatcher)
    {
      this.currentComicBookMatcher = comicBookMatcher;
      this.chkNot.Checked = comicBookMatcher.Not;
      this.cbMatchMode.SelectedIndex = comicBookMatcher.MatcherMode == MatcherMode.And ? 0 : 1;
      this.chkExpanded.Checked = !comicBookMatcher.Collapsed;
    }

    public void AddRule()
    {
      ComicBookMatcher comicBookMatcher = this.currentComicBookMatcher.Matchers.Last<ComicBookMatcher>().Clone<ComicBookMatcher>();
      this.matchers.Insert(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher) + 1, comicBookMatcher);
    }

    public void AddGroup()
    {
      if (this.level > 5)
        return;
      ComicBookGroupMatcher bookGroupMatcher = this.currentComicBookMatcher.Clone<ComicBookGroupMatcher>();
      bookGroupMatcher.Matchers.Clear();
      bookGroupMatcher.Matchers.Add(this.currentComicBookMatcher.Matchers.Last<ComicBookMatcher>().Clone<ComicBookMatcher>());
      this.matchers.Insert(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher) + 1, (ComicBookMatcher) bookGroupMatcher);
    }

    public void DeleteRuleOrGroup()
    {
      this.matchers.Remove((ComicBookMatcher) this.currentComicBookMatcher);
    }

    public void CopyClipboard()
    {
      Clipboard.SetData("ComicBookMatcher", (object) this.currentComicBookMatcher);
    }

    public void CutClipboard()
    {
      Clipboard.SetData("ComicBookMatcher", (object) this.currentComicBookMatcher);
      this.matchers.Remove((ComicBookMatcher) this.currentComicBookMatcher);
    }

    public void PasteClipboard()
    {
      if (!(Clipboard.GetData("ComicBookMatcher") is ComicBookMatcher data) || data is ComicBookGroupMatcher && this.level > 5)
        return;
      this.matchers.Insert(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher) + 1, data);
    }

    public void MoveUp()
    {
      this.matchers.MoveRelative(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher), -1);
    }

    public void MoveDown()
    {
      this.matchers.MoveRelative(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher), 1);
    }

    public void SetFocus() => this.Focus();

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.matcherControls = new FlowLayoutPanel();
      this.cbMatchMode = new ComboBox();
      this.chkNot = new CheckBox();
      this.labelSubRules = new Label();
      this.toolTip = new ToolTip(this.components);
      this.cmEdit = new ContextMenuStrip(this.components);
      this.miNewRule = new ToolStripMenuItem();
      this.miNewGroup = new ToolStripMenuItem();
      this.miDelete = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.miCut = new ToolStripMenuItem();
      this.miCopy = new ToolStripMenuItem();
      this.miPaste = new ToolStripMenuItem();
      this.toolStripMenuItem2 = new ToolStripSeparator();
      this.miMoveUp = new ToolStripMenuItem();
      this.miMoveDown = new ToolStripMenuItem();
      this.btEdit = new Button();
      this.chkExpanded = new CheckBox();
      this.cmEdit.SuspendLayout();
      this.SuspendLayout();
      this.matcherControls.AutoSize = true;
      this.matcherControls.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.matcherControls.BackColor = SystemColors.Control;
      this.matcherControls.FlowDirection = FlowDirection.TopDown;
      this.matcherControls.Location = new System.Drawing.Point(10, 25);
      this.matcherControls.Margin = new Padding(0, 3, 0, 3);
      this.matcherControls.MinimumSize = new System.Drawing.Size(400, 20);
      this.matcherControls.Name = "matcherControls";
      this.matcherControls.Size = new System.Drawing.Size(400, 20);
      this.matcherControls.TabIndex = 6;
      this.cbMatchMode.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbMatchMode.FormattingEnabled = true;
      this.cbMatchMode.Location = new System.Drawing.Point(26, 0);
      this.cbMatchMode.Name = "cbMatchMode";
      this.cbMatchMode.Size = new System.Drawing.Size(137, 21);
      this.cbMatchMode.TabIndex = 1;
      this.cbMatchMode.SelectedIndexChanged += new EventHandler(this.cbMatchMode_SelectedIndexChanged);
      this.chkNot.Appearance = Appearance.Button;
      this.chkNot.Location = new System.Drawing.Point(0, 0);
      this.chkNot.Name = "chkNot";
      this.chkNot.Size = new System.Drawing.Size(21, 21);
      this.chkNot.TabIndex = 0;
      this.chkNot.Text = "!";
      this.chkNot.TextAlign = ContentAlignment.MiddleCenter;
      this.chkNot.UseVisualStyleBackColor = true;
      this.chkNot.Click += new EventHandler(this.chkNot_CheckedChanged);
      this.labelSubRules.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.labelSubRules.AutoEllipsis = true;
      this.labelSubRules.Location = new System.Drawing.Point(169, 4);
      this.labelSubRules.Name = "labelSubRules";
      this.labelSubRules.Size = new System.Drawing.Size(366, 13);
      this.labelSubRules.TabIndex = 2;
      this.labelSubRules.Text = "of the following rules:";
      this.cmEdit.Items.AddRange(new ToolStripItem[10]
      {
        (ToolStripItem) this.miNewRule,
        (ToolStripItem) this.miNewGroup,
        (ToolStripItem) this.miDelete,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.miCut,
        (ToolStripItem) this.miCopy,
        (ToolStripItem) this.miPaste,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.miMoveUp,
        (ToolStripItem) this.miMoveDown
      });
      this.cmEdit.Name = "cmEdit";
      this.cmEdit.Size = new System.Drawing.Size(181, 192);
      this.cmEdit.Opening += new CancelEventHandler(this.cmEdit_Opening);
      this.miNewRule.Image = (Image) Resources.AddTab;
      this.miNewRule.Name = "miNewRule";
      this.miNewRule.ShortcutKeys = Keys.R | Keys.Control;
      this.miNewRule.Size = new System.Drawing.Size(180, 22);
      this.miNewRule.Text = "New Rule";
      this.miNewRule.Click += new EventHandler(this.miNewRule_Click);
      this.miNewGroup.Image = (Image) Resources.Group;
      this.miNewGroup.Name = "miNewGroup";
      this.miNewGroup.ShortcutKeys = Keys.G | Keys.Control;
      this.miNewGroup.Size = new System.Drawing.Size(180, 22);
      this.miNewGroup.Text = "New Group";
      this.miNewGroup.Click += new EventHandler(this.miNewGroup_Click);
      this.miDelete.Image = (Image) Resources.EditDelete;
      this.miDelete.Name = "miDelete";
      this.miDelete.Size = new System.Drawing.Size(180, 22);
      this.miDelete.Text = "Delete";
      this.miDelete.Click += new EventHandler(this.miDelete_Click);
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
      this.miCut.Image = (Image) Resources.Cut;
      this.miCut.Name = "miCut";
      this.miCut.ShortcutKeys = Keys.X | Keys.Control;
      this.miCut.Size = new System.Drawing.Size(180, 22);
      this.miCut.Text = "Cut";
      this.miCut.Click += new EventHandler(this.miCut_Click);
      this.miCopy.Image = (Image) Resources.EditCopy;
      this.miCopy.Name = "miCopy";
      this.miCopy.ShortcutKeys = Keys.C | Keys.Control;
      this.miCopy.Size = new System.Drawing.Size(180, 22);
      this.miCopy.Text = "Copy";
      this.miCopy.Click += new EventHandler(this.miCopy_Click);
      this.miPaste.Image = (Image) Resources.EditPaste;
      this.miPaste.Name = "miPaste";
      this.miPaste.ShortcutKeys = Keys.P | Keys.Control;
      this.miPaste.Size = new System.Drawing.Size(180, 22);
      this.miPaste.Text = "Paste";
      this.miPaste.Click += new EventHandler(this.miPaste_Click);
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
      this.miMoveUp.Image = (Image) Resources.GroupUp;
      this.miMoveUp.Name = "miMoveUp";
      this.miMoveUp.ShortcutKeys = Keys.U | Keys.Control;
      this.miMoveUp.Size = new System.Drawing.Size(180, 22);
      this.miMoveUp.Text = "Move Up";
      this.miMoveUp.Click += new EventHandler(this.miMoveUp_Click);
      this.miMoveDown.Image = (Image) Resources.GroupDown;
      this.miMoveDown.Name = "miMoveDown";
      this.miMoveDown.ShortcutKeys = Keys.D | Keys.Control;
      this.miMoveDown.Size = new System.Drawing.Size(180, 22);
      this.miMoveDown.Text = "Move Down";
      this.miMoveDown.Click += new EventHandler(this.miMoveDown_Click);
      this.btEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btEdit.ContextMenuStrip = this.cmEdit;
      this.btEdit.Image = (Image) Resources.SmallArrowDown;
      this.btEdit.Location = new System.Drawing.Point(568, 0);
      this.btEdit.Name = "btEdit";
      this.btEdit.Size = new System.Drawing.Size(21, 22);
      this.btEdit.TabIndex = 11;
      this.btEdit.UseVisualStyleBackColor = true;
      this.btEdit.Click += new EventHandler(this.btEdit_Click);
      this.chkExpanded.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkExpanded.Appearance = Appearance.Button;
      this.chkExpanded.Checked = true;
      this.chkExpanded.CheckState = CheckState.Checked;
      this.chkExpanded.Image = (Image) Resources.DoubleArrow;
      this.chkExpanded.Location = new System.Drawing.Point(543, 0);
      this.chkExpanded.Name = "chkExpanded";
      this.chkExpanded.Size = new System.Drawing.Size(22, 22);
      this.chkExpanded.TabIndex = 12;
      this.chkExpanded.UseVisualStyleBackColor = true;
      this.chkExpanded.CheckedChanged += new EventHandler(this.chkCollapse_CheckedChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.BackColor = SystemColors.Control;
      this.Controls.Add((Control) this.chkExpanded);
      this.Controls.Add((Control) this.matcherControls);
      this.Controls.Add((Control) this.labelSubRules);
      this.Controls.Add((Control) this.chkNot);
      this.Controls.Add((Control) this.cbMatchMode);
      this.Controls.Add((Control) this.btEdit);
      this.Margin = new Padding(0, 3, 0, 3);
      this.MinimumSize = new System.Drawing.Size(400, 0);
      this.Name = nameof (MatcherGroupEditor);
      this.Size = new System.Drawing.Size(589, 48);
      this.cmEdit.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

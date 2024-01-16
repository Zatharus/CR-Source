// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.MatcherEditor
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class MatcherEditor : UserControl, IMatcherEditor
  {
    public const int MaxLevel = 5;
    public static readonly ValuePair<Color, Regex>[] ColorRegex = new ValuePair<Color, Regex>[2]
    {
      new ValuePair<Color, Regex>(Color.Red, new Regex("\\{[a-z]+\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled)),
      new ValuePair<Color, Regex>(Color.Blue, new Regex("\\{(" + ((IEnumerable<string>) ComicBookMatcher.ComicProperties).Concat<string>((IEnumerable<string>) ComicBookMatcher.SeriesStatsProperties).ToListString("|") + ")\\}", RegexOptions.Compiled))
    };
    private ComicBookValueMatcher currentComicBookMatcher;
    private readonly ComicBookMatcherCollection matchers;
    private readonly int spacing = 10;
    private readonly int level;
    private IContainer components;
    private ComboBox cbOperator;
    private Label lblDescription;
    private CheckBox chkNot;
    private ToolTip toolTip;
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
    private Button btEdit;
    private TextBox rtfMatchValue;
    private TextBox rtfMatchValue2;
    private ToolStripMenuItem miDelete;
    private Button btMatcher;

    public MatcherEditor(
      ComicBookMatcherCollection matchers,
      ComicBookValueMatcher comicBookMatcher,
      int level,
      int width)
    {
      this.InitializeComponent();
      this.toolTip.SetToolTip((Control) this.chkNot, TR.Default["LogicNot", "Negation"]);
      this.matchers = matchers;
      this.level = level;
      this.Height = this.btEdit.Bottom + 2;
      this.Width = width;
      this.spacing = this.rtfMatchValue2.Left - this.rtfMatchValue.Right;
      this.InitializeMatcher(comicBookMatcher);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    public string Description
    {
      get
      {
        string description = this.rtfMatchValue.Text.Trim();
        if (string.IsNullOrEmpty(description))
          description = this.btMatcher.Text;
        return description;
      }
    }

    private void cmEdit_Opening(object sender, CancelEventArgs e)
    {
      this.miNewGroup.Enabled = this.level <= 5;
      this.miCut.Enabled = this.miDelete.Enabled = this.matchers.Count > 1;
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

    private void cbOperator_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.currentComicBookMatcher.MatchOperator = this.cbOperator.SelectedIndex;
      this.SetVisiblity();
      this.rtfMatchValue.Text = this.currentComicBookMatcher.MatchValue;
      this.rtfMatchValue2.Text = this.currentComicBookMatcher.MatchValue2;
    }

    private void rtfMatchValue_Leave(object sender, EventArgs e)
    {
      this.currentComicBookMatcher.MatchValue = this.rtfMatchValue.Text;
    }

    private void rtfMatchValue2_Leave(object sender, EventArgs e)
    {
      this.currentComicBookMatcher.MatchValue2 = this.rtfMatchValue2.Text;
    }

    private void chkNot_CheckedChanged(object sender, EventArgs e)
    {
      this.currentComicBookMatcher.Not = this.chkNot.Checked;
    }

    private void btMatcher_Click(object sender, EventArgs e)
    {
      Program.CreateComicBookMatchersMenu((Action<ComicBookValueMatcher>) (newMatcher =>
      {
        if (newMatcher == null || newMatcher.GetType() == this.currentComicBookMatcher.GetType())
          return;
        int index = this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher);
        newMatcher.Set(this.currentComicBookMatcher);
        this.InitializeMatcher(newMatcher);
        if (index == -1)
          return;
        this.matchers[index] = (ComicBookMatcher) newMatcher;
      })).Show((Control) this.btMatcher, new System.Drawing.Point(0, this.btMatcher.Height), ToolStripDropDownDirection.BelowRight);
    }

    private void btEdit_Click(object sender, EventArgs e)
    {
      this.cmEdit.Show((Control) this.btEdit, new System.Drawing.Point(this.btEdit.Width, this.btEdit.Height), ToolStripDropDownDirection.BelowLeft);
    }

    private void rtfMatchValue_DoubleClick(object sender, EventArgs e)
    {
      TextBoxBase textBoxBase = sender as TextBoxBase;
      using (ValueEditorDialog valueEditorDialog = new ValueEditorDialog())
      {
        valueEditorDialog.SyntaxColoring((IEnumerable<ValuePair<Color, Regex>>) MatcherEditor.ColorRegex);
        valueEditorDialog.MatchValue = textBoxBase.Text;
        if (valueEditorDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
          return;
        textBoxBase.Text = valueEditorDialog.MatchValue;
      }
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
      base.OnLayout(e);
      if (!this.IsHandleCreated)
        return;
      this.SetVisiblity();
    }

    private void InitializeMatcher(ComicBookValueMatcher comicBookMatcher)
    {
      this.currentComicBookMatcher = comicBookMatcher;
      this.Tag = (object) comicBookMatcher;
      this.chkNot.Checked = comicBookMatcher.Not;
      this.btMatcher.Text = comicBookMatcher.Description;
      this.btMatcher.Tag = (object) comicBookMatcher;
      this.cbOperator.Items.Clear();
      this.cbOperator.Items.AddRange((object[]) comicBookMatcher.OperatorsList);
      this.cbOperator.SelectedIndex = comicBookMatcher.MatchOperator;
    }

    private void SetVisiblity()
    {
      Control cbOperator = (Control) this.cbOperator;
      if (this.cbOperator.Left > this.rtfMatchValue.Left)
      {
        int left = this.cbOperator.Left;
        this.cbOperator.Left = this.rtfMatchValue.Left;
        this.rtfMatchValue.Left = left;
      }
      this.rtfMatchValue.Width = this.btEdit.Left - this.rtfMatchValue.Left - 8;
      this.rtfMatchValue2.Width = this.btEdit.Left - this.rtfMatchValue2.Left - 8;
      switch (this.currentComicBookMatcher.ArgumentCount)
      {
        case 0:
          this.rtfMatchValue.Visible = this.rtfMatchValue2.Visible = this.lblDescription.Visible = false;
          cbOperator.Width = this.rtfMatchValue2.Right - this.cbOperator.Left;
          break;
        case 1:
          this.rtfMatchValue.Visible = true;
          this.rtfMatchValue.Width = this.rtfMatchValue2.Right - this.rtfMatchValue.Left;
          this.rtfMatchValue2.Visible = false;
          cbOperator.Width = this.rtfMatchValue.Left - 8 - cbOperator.Left;
          if (!string.IsNullOrEmpty(this.currentComicBookMatcher.UnitDescription))
          {
            this.lblDescription.Text = this.currentComicBookMatcher.UnitDescription;
            this.lblDescription.Visible = !string.IsNullOrEmpty(this.currentComicBookMatcher.UnitDescription);
            this.lblDescription.Left = this.rtfMatchValue2.Right - this.lblDescription.Width;
            this.lblDescription.Top = this.rtfMatchValue2.Top + (this.rtfMatchValue2.Height - this.lblDescription.Height) / 2;
            this.rtfMatchValue.Width = this.lblDescription.Left - this.spacing - this.rtfMatchValue.Left;
            break;
          }
          break;
        case 2:
          this.rtfMatchValue.Width = this.rtfMatchValue2.Left - this.spacing - this.rtfMatchValue.Left;
          this.rtfMatchValue.Visible = this.rtfMatchValue2.Visible = true;
          this.lblDescription.Visible = false;
          cbOperator.Width = this.rtfMatchValue.Left - 8 - cbOperator.Left;
          break;
      }
      bool flag = this.cbOperator.Left > this.rtfMatchValue.Left;
      if (!this.currentComicBookMatcher.SwapOperatorArgument)
        return;
      int left1 = this.cbOperator.Left;
      this.cbOperator.Left = this.rtfMatchValue.Left;
      this.rtfMatchValue.Left = left1;
    }

    public void AddRule()
    {
      this.matchers.Insert(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher) + 1, (ComicBookMatcher) this.currentComicBookMatcher.Clone());
    }

    public void AddGroup()
    {
      if (this.level > 5)
        return;
      ComicBookGroupMatcher bookGroupMatcher = new ComicBookGroupMatcher();
      bookGroupMatcher.Matchers.Add(this.currentComicBookMatcher.Clone() as ComicBookMatcher);
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
      ComicBookMatcher comicBookMatcher = (ComicBookMatcher) null;
      try
      {
        comicBookMatcher = Clipboard.GetData("ComicBookMatcher") as ComicBookMatcher;
      }
      catch
      {
      }
      if (comicBookMatcher == null || comicBookMatcher is ComicBookGroupMatcher && this.level > 5)
        return;
      this.matchers.Insert(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher) + 1, comicBookMatcher);
    }

    public void MoveUp()
    {
      this.matchers.MoveRelative(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher), -1);
    }

    public void MoveDown()
    {
      this.matchers.MoveRelative(this.matchers.IndexOf((ComicBookMatcher) this.currentComicBookMatcher), 1);
    }

    public void SetFocus()
    {
      if (this.rtfMatchValue.Visible)
        this.rtfMatchValue.Focus();
      else
        this.cbOperator.Focus();
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.cbOperator = new ComboBox();
      this.lblDescription = new Label();
      this.chkNot = new CheckBox();
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
      this.rtfMatchValue = new TextBox();
      this.rtfMatchValue2 = new TextBox();
      this.btMatcher = new Button();
      this.cmEdit.SuspendLayout();
      this.SuspendLayout();
      this.cbOperator.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbOperator.FormattingEnabled = true;
      this.cbOperator.Location = new System.Drawing.Point(169, 0);
      this.cbOperator.Name = "cbOperator";
      this.cbOperator.Size = new System.Drawing.Size(135, 21);
      this.cbOperator.TabIndex = 2;
      this.cbOperator.SelectedIndexChanged += new EventHandler(this.cbOperator_SelectedIndexChanged);
      this.lblDescription.AutoSize = true;
      this.lblDescription.Location = new System.Drawing.Point(448, 25);
      this.lblDescription.Name = "lblDescription";
      this.lblDescription.Size = new System.Drawing.Size(60, 13);
      this.lblDescription.TabIndex = 7;
      this.lblDescription.Text = "Description";
      this.chkNot.Appearance = Appearance.Button;
      this.chkNot.Location = new System.Drawing.Point(0, 0);
      this.chkNot.Name = "chkNot";
      this.chkNot.Size = new System.Drawing.Size(21, 21);
      this.chkNot.TabIndex = 0;
      this.chkNot.Text = "!";
      this.chkNot.TextAlign = ContentAlignment.MiddleCenter;
      this.chkNot.UseVisualStyleBackColor = true;
      this.chkNot.CheckedChanged += new EventHandler(this.chkNot_CheckedChanged);
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
      this.miPaste.ShortcutKeys = Keys.V | Keys.Control;
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
      this.btEdit.Size = new System.Drawing.Size(21, 21);
      this.btEdit.TabIndex = 10;
      this.btEdit.UseVisualStyleBackColor = true;
      this.btEdit.Click += new EventHandler(this.btEdit_Click);
      this.rtfMatchValue.Location = new System.Drawing.Point(310, 1);
      this.rtfMatchValue.Name = "rtfMatchValue";
      this.rtfMatchValue.Size = new System.Drawing.Size(135, 20);
      this.rtfMatchValue.TabIndex = 11;
      this.rtfMatchValue.DoubleClick += new EventHandler(this.rtfMatchValue_DoubleClick);
      this.rtfMatchValue.Validated += new EventHandler(this.rtfMatchValue_Leave);
      this.rtfMatchValue2.Location = new System.Drawing.Point(451, 1);
      this.rtfMatchValue2.Name = "rtfMatchValue2";
      this.rtfMatchValue2.Size = new System.Drawing.Size(100, 20);
      this.rtfMatchValue2.TabIndex = 12;
      this.rtfMatchValue2.DoubleClick += new EventHandler(this.rtfMatchValue_DoubleClick);
      this.rtfMatchValue2.Validated += new EventHandler(this.rtfMatchValue2_Leave);
      this.btMatcher.Image = (Image) Resources.SmallArrowDown;
      this.btMatcher.ImageAlign = ContentAlignment.MiddleRight;
      this.btMatcher.Location = new System.Drawing.Point(27, -1);
      this.btMatcher.Name = "btMatcher";
      this.btMatcher.Size = new System.Drawing.Size(136, 23);
      this.btMatcher.TabIndex = 13;
      this.btMatcher.Text = "Pages";
      this.btMatcher.TextAlign = ContentAlignment.MiddleLeft;
      this.btMatcher.UseVisualStyleBackColor = true;
      this.btMatcher.Click += new EventHandler(this.btMatcher_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.btMatcher);
      this.Controls.Add((Control) this.btEdit);
      this.Controls.Add((Control) this.rtfMatchValue2);
      this.Controls.Add((Control) this.rtfMatchValue);
      this.Controls.Add((Control) this.chkNot);
      this.Controls.Add((Control) this.cbOperator);
      this.Controls.Add((Control) this.lblDescription);
      this.Margin = new Padding(0, 3, 0, 3);
      this.Name = nameof (MatcherEditor);
      this.Size = new System.Drawing.Size(589, 50);
      this.cmEdit.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private class MatcherEntry : ComboBoxSkinner.ComboBoxItem<ComicBookValueMatcher>
    {
      public MatcherEntry(ComicBookValueMatcher item)
        : base(item)
      {
      }

      public override string ToString() => this.Item.Description;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.KeyboardShortcutEditor
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class KeyboardShortcutEditor : UserControl
  {
    private KeyboardShortcuts shortcuts;
    private ListViewItem currentItem;
    private KeyboardCommand currentCommand;
    private IContainer components;
    private ColumnHeader chCommand;
    private ColumnHeader chKeyboard;
    private ColumnHeader chKeyboard2;
    private ListView lvCommands;
    private ColumnHeader chAction;
    private ColumnHeader chKeys;
    private Panel panelKeyEditor;
    private CheckBox chkCtrl1;
    private ComboBox cbKey1;
    private Label labelMainKey;
    private CheckBox chkShift1;
    private CheckBox chkAlt1;
    private CheckBox chkCtrl2;
    private ComboBox cbKey2;
    private CheckBox chkShift2;
    private Label labelAlternateKey;
    private CheckBox chkAlt2;
    private ImageList imageList;
    private Label labelCurrentKey;
    private CheckBox chkCtrl3;
    private ComboBox cbKey3;
    private CheckBox chkShift3;
    private Label labelAlternate2Key;
    private CheckBox chkAlt3;
    private Button btPress3;
    private Button btPress2;
    private Button btPress1;
    private Button btPress4;
    private CheckBox chkCtrl4;
    private ComboBox cbKey4;
    private CheckBox chkShift4;
    private Label labelAlternate3Key;
    private CheckBox chkAlt4;

    public KeyboardShortcutEditor()
    {
      this.InitializeComponent();
      this.imageList.ImageSize = this.imageList.ImageSize.ScaleDpi();
      this.lvCommands.Columns.ScaleDpi();
      LocalizeUtility.Localize((Control) this, this.components);
      KeyboardShortcutEditor.FillKeys(this.cbKey1);
      KeyboardShortcutEditor.FillKeys(this.cbKey2);
      KeyboardShortcutEditor.FillKeys(this.cbKey3);
      KeyboardShortcutEditor.FillKeys(this.cbKey4);
    }

    public KeyboardShortcuts Shortcuts
    {
      get => this.shortcuts;
      set
      {
        if (this.shortcuts == value)
          return;
        this.shortcuts = value;
        this.FillList();
      }
    }

    public void RefreshList()
    {
      foreach (ListViewItem listViewItem in this.lvCommands.Items)
        listViewItem.SubItems[1].Text = ((KeyboardCommand) listViewItem.Tag).KeyNames;
      this.UpdateSelectedItem();
    }

    private void FillList()
    {
      foreach (KeyboardCommand command in this.shortcuts.Commands)
      {
        int imageIndex = -1;
        ListViewGroup listViewGroup = this.lvCommands.Groups[command.Group] ?? this.lvCommands.Groups.Add(command.Group, command.Group);
        if (command.Image != null)
        {
          this.imageList.Images.Add(command.Image);
          imageIndex = this.imageList.Images.Count - 1;
        }
        ListViewItem listViewItem = this.lvCommands.Items.Add(command.Text, imageIndex);
        listViewItem.SubItems.Add(command.KeyNames);
        listViewItem.Tag = (object) command;
        listViewGroup.Items.Add(listViewItem);
      }
      this.lvCommands.Items[0].Selected = true;
    }

    private static void FillKeys(ComboBox cb)
    {
      foreach (CommandKey key in Enum.GetValues(typeof (CommandKey)))
      {
        if (KeyboardCommand.IsKeyValue(key))
          cb.Items.Add((object) new KeyboardShortcutEditor.KeyItem(key));
      }
    }

    private static void SelectKey(CommandKey ck, ComboBox cb)
    {
      foreach (KeyboardShortcutEditor.KeyItem keyItem in cb.Items)
      {
        if (keyItem.Key == (ck & ~CommandKey.Modifiers))
        {
          cb.SelectedItem = (object) keyItem;
          return;
        }
      }
      KeyboardShortcutEditor.SelectKey(CommandKey.None, cb);
    }

    private void SelectKey(
      int n,
      ComboBox cb,
      CheckBox ctrl,
      CheckBox shift,
      CheckBox alt,
      Button button)
    {
      cb.Tag = (object) -1;
      ctrl.Tag = (object) -1;
      shift.Tag = (object) -1;
      alt.Tag = (object) -1;
      if (this.currentCommand == null)
      {
        if (!this.panelKeyEditor.Enabled)
          return;
        this.panelKeyEditor.Enabled = false;
      }
      else
      {
        if (!this.panelKeyEditor.Enabled)
          this.panelKeyEditor.Enabled = true;
        CommandKey ck = this.currentCommand.Keyboard[n];
        KeyboardShortcutEditor.SelectKey(ck, cb);
        ctrl.Checked = (ck & CommandKey.Ctrl) != 0;
        shift.Checked = (ck & CommandKey.Shift) != 0;
        alt.Checked = (ck & CommandKey.Alt) != 0;
        cb.Tag = (object) n;
        ctrl.Tag = (object) n;
        shift.Tag = (object) n;
        alt.Tag = (object) n;
        button.Tag = (object) n;
        cb.SelectedIndexChanged -= new EventHandler(this.cb_SelectedIndexChanged);
        cb.SelectedIndexChanged += new EventHandler(this.cb_SelectedIndexChanged);
        ctrl.CheckedChanged -= new EventHandler(this.ctrl_CheckedChanged);
        ctrl.CheckedChanged += new EventHandler(this.ctrl_CheckedChanged);
        shift.CheckedChanged -= new EventHandler(this.shift_CheckedChanged);
        shift.CheckedChanged += new EventHandler(this.shift_CheckedChanged);
        alt.CheckedChanged -= new EventHandler(this.alt_CheckedChanged);
        alt.CheckedChanged += new EventHandler(this.alt_CheckedChanged);
        button.Click -= new EventHandler(this.button_Click);
        button.Click += new EventHandler(this.button_Click);
      }
    }

    private static void FlipCommand(ref CommandKey ck, CommandKey mask, bool flag)
    {
      if (flag)
        ck |= mask;
      else
        ck &= ~mask;
    }

    private void ctrl_CheckedChanged(object sender, EventArgs e)
    {
      CheckBox checkBox = (CheckBox) sender;
      int tag = (int) checkBox.Tag;
      if (tag == -1)
        return;
      KeyboardShortcutEditor.FlipCommand(ref this.currentCommand.Keyboard[tag], CommandKey.Ctrl, checkBox.Checked);
      this.UpdateCurrentItem();
    }

    private void shift_CheckedChanged(object sender, EventArgs e)
    {
      CheckBox checkBox = (CheckBox) sender;
      int tag = (int) checkBox.Tag;
      if (tag == -1)
        return;
      KeyboardShortcutEditor.FlipCommand(ref this.currentCommand.Keyboard[tag], CommandKey.Shift, checkBox.Checked);
      this.UpdateCurrentItem();
    }

    private void alt_CheckedChanged(object sender, EventArgs e)
    {
      CheckBox checkBox = (CheckBox) sender;
      int tag = (int) checkBox.Tag;
      if (tag == -1)
        return;
      KeyboardShortcutEditor.FlipCommand(ref this.currentCommand.Keyboard[tag], CommandKey.Alt, checkBox.Checked);
      this.UpdateCurrentItem();
    }

    private void cb_SelectedIndexChanged(object sender, EventArgs e)
    {
      ComboBox comboBox = (ComboBox) sender;
      int tag = (int) comboBox.Tag;
      if (tag == -1)
        return;
      CommandKey commandKey = this.currentCommand.Keyboard[tag];
      this.currentCommand.Keyboard[tag] = ((KeyboardShortcutEditor.KeyItem) comboBox.SelectedItem).Key | commandKey & CommandKey.Modifiers;
      this.UpdateCurrentItem();
    }

    private void button_Click(object sender, EventArgs e)
    {
      int tag = (int) ((Control) sender).Tag;
      CommandKey commandKey = KeyInputForm.Show((IWin32Window) this, LocalizeUtility.GetText((Control) this, "GetKeyCaption", "Press Key"), LocalizeUtility.GetText((Control) this, "GetKeyDescription", "Press your key combination or close this window"));
      if (commandKey == CommandKey.None)
        return;
      this.currentCommand.Keyboard[tag] = commandKey;
      this.UpdateCurrentItem();
      this.UpdateControls();
    }

    private void UpdateCurrentItem()
    {
      if (this.currentItem == null)
        return;
      this.currentItem.SubItems[1].Text = this.currentCommand.KeyNames;
    }

    private void UpdateSelectedItem()
    {
      if (this.lvCommands.SelectedItems.Count == 0)
      {
        this.currentItem = (ListViewItem) null;
        this.currentCommand = (KeyboardCommand) null;
        this.labelCurrentKey.Text = LocalizeUtility.GetText((Control) this, "NothingSelected", "No action selected");
      }
      else
      {
        this.currentItem = this.lvCommands.SelectedItems[0];
        this.currentCommand = (KeyboardCommand) this.currentItem.Tag;
        this.labelCurrentKey.Text = this.currentCommand.Text + ":";
      }
      this.UpdateControls();
    }

    private void UpdateControls()
    {
      this.SelectKey(0, this.cbKey1, this.chkCtrl1, this.chkShift1, this.chkAlt1, this.btPress1);
      this.SelectKey(1, this.cbKey2, this.chkCtrl2, this.chkShift2, this.chkAlt2, this.btPress2);
      this.SelectKey(2, this.cbKey3, this.chkCtrl3, this.chkShift3, this.chkAlt3, this.btPress3);
      this.SelectKey(3, this.cbKey4, this.chkCtrl4, this.chkShift4, this.chkAlt4, this.btPress4);
    }

    private void lvCommands_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.UpdateSelectedItem();
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
      this.chCommand = new ColumnHeader();
      this.chKeyboard = new ColumnHeader();
      this.chKeyboard2 = new ColumnHeader();
      this.lvCommands = new ListView();
      this.chAction = new ColumnHeader();
      this.chKeys = new ColumnHeader();
      this.imageList = new ImageList(this.components);
      this.panelKeyEditor = new Panel();
      this.btPress4 = new Button();
      this.chkCtrl4 = new CheckBox();
      this.cbKey4 = new ComboBox();
      this.chkShift4 = new CheckBox();
      this.labelAlternate3Key = new Label();
      this.chkAlt4 = new CheckBox();
      this.btPress3 = new Button();
      this.btPress2 = new Button();
      this.btPress1 = new Button();
      this.chkCtrl3 = new CheckBox();
      this.cbKey3 = new ComboBox();
      this.chkShift3 = new CheckBox();
      this.labelAlternate2Key = new Label();
      this.chkAlt3 = new CheckBox();
      this.labelCurrentKey = new Label();
      this.chkCtrl2 = new CheckBox();
      this.cbKey2 = new ComboBox();
      this.chkShift2 = new CheckBox();
      this.labelAlternateKey = new Label();
      this.chkAlt2 = new CheckBox();
      this.chkCtrl1 = new CheckBox();
      this.cbKey1 = new ComboBox();
      this.chkShift1 = new CheckBox();
      this.labelMainKey = new Label();
      this.chkAlt1 = new CheckBox();
      this.panelKeyEditor.SuspendLayout();
      this.SuspendLayout();
      this.chCommand.Text = "Action";
      this.chCommand.Width = 250;
      this.chKeyboard.Text = "Key";
      this.chKeyboard.Width = 80;
      this.chKeyboard2.Text = "Alternate";
      this.chKeyboard2.Width = 80;
      this.lvCommands.Columns.AddRange(new ColumnHeader[2]
      {
        this.chAction,
        this.chKeys
      });
      this.lvCommands.Dock = DockStyle.Fill;
      this.lvCommands.FullRowSelect = true;
      this.lvCommands.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.lvCommands.HideSelection = false;
      this.lvCommands.Location = new System.Drawing.Point(0, 0);
      this.lvCommands.MultiSelect = false;
      this.lvCommands.Name = "lvCommands";
      this.lvCommands.Size = new System.Drawing.Size(467, 230);
      this.lvCommands.SmallImageList = this.imageList;
      this.lvCommands.TabIndex = 0;
      this.lvCommands.UseCompatibleStateImageBehavior = false;
      this.lvCommands.View = View.Details;
      this.lvCommands.SelectedIndexChanged += new EventHandler(this.lvCommands_SelectedIndexChanged);
      this.chAction.Text = "Action";
      this.chAction.Width = 268;
      this.chKeys.Text = "Keys";
      this.chKeys.Width = 158;
      this.imageList.ColorDepth = ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = Color.Transparent;
      this.panelKeyEditor.Controls.Add((Control) this.btPress4);
      this.panelKeyEditor.Controls.Add((Control) this.chkCtrl4);
      this.panelKeyEditor.Controls.Add((Control) this.cbKey4);
      this.panelKeyEditor.Controls.Add((Control) this.chkShift4);
      this.panelKeyEditor.Controls.Add((Control) this.labelAlternate3Key);
      this.panelKeyEditor.Controls.Add((Control) this.chkAlt4);
      this.panelKeyEditor.Controls.Add((Control) this.btPress3);
      this.panelKeyEditor.Controls.Add((Control) this.btPress2);
      this.panelKeyEditor.Controls.Add((Control) this.btPress1);
      this.panelKeyEditor.Controls.Add((Control) this.chkCtrl3);
      this.panelKeyEditor.Controls.Add((Control) this.cbKey3);
      this.panelKeyEditor.Controls.Add((Control) this.chkShift3);
      this.panelKeyEditor.Controls.Add((Control) this.labelAlternate2Key);
      this.panelKeyEditor.Controls.Add((Control) this.chkAlt3);
      this.panelKeyEditor.Controls.Add((Control) this.labelCurrentKey);
      this.panelKeyEditor.Controls.Add((Control) this.chkCtrl2);
      this.panelKeyEditor.Controls.Add((Control) this.cbKey2);
      this.panelKeyEditor.Controls.Add((Control) this.chkShift2);
      this.panelKeyEditor.Controls.Add((Control) this.labelAlternateKey);
      this.panelKeyEditor.Controls.Add((Control) this.chkAlt2);
      this.panelKeyEditor.Controls.Add((Control) this.chkCtrl1);
      this.panelKeyEditor.Controls.Add((Control) this.cbKey1);
      this.panelKeyEditor.Controls.Add((Control) this.chkShift1);
      this.panelKeyEditor.Controls.Add((Control) this.labelMainKey);
      this.panelKeyEditor.Controls.Add((Control) this.chkAlt1);
      this.panelKeyEditor.Dock = DockStyle.Bottom;
      this.panelKeyEditor.Location = new System.Drawing.Point(0, 230);
      this.panelKeyEditor.Name = "panelKeyEditor";
      this.panelKeyEditor.Size = new System.Drawing.Size(467, 142);
      this.panelKeyEditor.TabIndex = 1;
      this.btPress4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btPress4.Location = new System.Drawing.Point(439, 113);
      this.btPress4.Name = "btPress4";
      this.btPress4.Size = new System.Drawing.Size(25, 21);
      this.btPress4.TabIndex = 24;
      this.btPress4.Text = "...";
      this.btPress4.UseVisualStyleBackColor = true;
      this.chkCtrl4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkCtrl4.AutoSize = true;
      this.chkCtrl4.Location = new System.Drawing.Point(259, 117);
      this.chkCtrl4.Name = "chkCtrl4";
      this.chkCtrl4.Size = new System.Drawing.Size(41, 17);
      this.chkCtrl4.TabIndex = 21;
      this.chkCtrl4.Text = "Ctrl";
      this.chkCtrl4.UseVisualStyleBackColor = true;
      this.cbKey4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbKey4.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbKey4.FormattingEnabled = true;
      this.cbKey4.Location = new System.Drawing.Point(81, 113);
      this.cbKey4.Name = "cbKey4";
      this.cbKey4.Size = new System.Drawing.Size(170, 21);
      this.cbKey4.TabIndex = 20;
      this.chkShift4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkShift4.AutoSize = true;
      this.chkShift4.Location = new System.Drawing.Point(314, 117);
      this.chkShift4.Name = "chkShift4";
      this.chkShift4.Size = new System.Drawing.Size(47, 17);
      this.chkShift4.TabIndex = 22;
      this.chkShift4.Text = "Shift";
      this.chkShift4.UseVisualStyleBackColor = true;
      this.labelAlternate3Key.AutoSize = true;
      this.labelAlternate3Key.Location = new System.Drawing.Point(12, 116);
      this.labelAlternate3Key.Name = "labelAlternate3Key";
      this.labelAlternate3Key.Size = new System.Drawing.Size(52, 13);
      this.labelAlternate3Key.TabIndex = 19;
      this.labelAlternate3Key.Text = "Alternate:";
      this.chkAlt4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkAlt4.AutoSize = true;
      this.chkAlt4.Location = new System.Drawing.Point(378, 117);
      this.chkAlt4.Name = "chkAlt4";
      this.chkAlt4.Size = new System.Drawing.Size(38, 17);
      this.chkAlt4.TabIndex = 23;
      this.chkAlt4.Text = "Alt";
      this.chkAlt4.UseVisualStyleBackColor = true;
      this.btPress3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btPress3.Location = new System.Drawing.Point(439, 86);
      this.btPress3.Name = "btPress3";
      this.btPress3.Size = new System.Drawing.Size(25, 21);
      this.btPress3.TabIndex = 18;
      this.btPress3.Text = "...";
      this.btPress3.UseVisualStyleBackColor = true;
      this.btPress2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btPress2.Location = new System.Drawing.Point(439, 60);
      this.btPress2.Name = "btPress2";
      this.btPress2.Size = new System.Drawing.Size(25, 21);
      this.btPress2.TabIndex = 12;
      this.btPress2.Text = "...";
      this.btPress2.UseVisualStyleBackColor = true;
      this.btPress1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btPress1.Location = new System.Drawing.Point(439, 34);
      this.btPress1.Name = "btPress1";
      this.btPress1.Size = new System.Drawing.Size(25, 21);
      this.btPress1.TabIndex = 6;
      this.btPress1.Text = "...";
      this.btPress1.UseVisualStyleBackColor = true;
      this.chkCtrl3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkCtrl3.AutoSize = true;
      this.chkCtrl3.Location = new System.Drawing.Point(259, 90);
      this.chkCtrl3.Name = "chkCtrl3";
      this.chkCtrl3.Size = new System.Drawing.Size(41, 17);
      this.chkCtrl3.TabIndex = 15;
      this.chkCtrl3.Text = "Ctrl";
      this.chkCtrl3.UseVisualStyleBackColor = true;
      this.cbKey3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbKey3.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbKey3.FormattingEnabled = true;
      this.cbKey3.Location = new System.Drawing.Point(81, 86);
      this.cbKey3.Name = "cbKey3";
      this.cbKey3.Size = new System.Drawing.Size(170, 21);
      this.cbKey3.TabIndex = 14;
      this.chkShift3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkShift3.AutoSize = true;
      this.chkShift3.Location = new System.Drawing.Point(314, 90);
      this.chkShift3.Name = "chkShift3";
      this.chkShift3.Size = new System.Drawing.Size(47, 17);
      this.chkShift3.TabIndex = 16;
      this.chkShift3.Text = "Shift";
      this.chkShift3.UseVisualStyleBackColor = true;
      this.labelAlternate2Key.AutoSize = true;
      this.labelAlternate2Key.Location = new System.Drawing.Point(12, 89);
      this.labelAlternate2Key.Name = "labelAlternate2Key";
      this.labelAlternate2Key.Size = new System.Drawing.Size(52, 13);
      this.labelAlternate2Key.TabIndex = 13;
      this.labelAlternate2Key.Text = "Alternate:";
      this.chkAlt3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkAlt3.AutoSize = true;
      this.chkAlt3.Location = new System.Drawing.Point(378, 90);
      this.chkAlt3.Name = "chkAlt3";
      this.chkAlt3.Size = new System.Drawing.Size(38, 17);
      this.chkAlt3.TabIndex = 17;
      this.chkAlt3.Text = "Alt";
      this.chkAlt3.UseVisualStyleBackColor = true;
      this.labelCurrentKey.AutoSize = true;
      this.labelCurrentKey.Location = new System.Drawing.Point(12, 12);
      this.labelCurrentKey.Name = "labelCurrentKey";
      this.labelCurrentKey.Size = new System.Drawing.Size(67, 13);
      this.labelCurrentKey.TabIndex = 0;
      this.labelCurrentKey.Text = "Lorum Ipsum";
      this.chkCtrl2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkCtrl2.AutoSize = true;
      this.chkCtrl2.Location = new System.Drawing.Point(259, 63);
      this.chkCtrl2.Name = "chkCtrl2";
      this.chkCtrl2.Size = new System.Drawing.Size(41, 17);
      this.chkCtrl2.TabIndex = 9;
      this.chkCtrl2.Text = "Ctrl";
      this.chkCtrl2.UseVisualStyleBackColor = true;
      this.cbKey2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbKey2.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbKey2.FormattingEnabled = true;
      this.cbKey2.Location = new System.Drawing.Point(81, 59);
      this.cbKey2.Name = "cbKey2";
      this.cbKey2.Size = new System.Drawing.Size(170, 21);
      this.cbKey2.TabIndex = 8;
      this.chkShift2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkShift2.AutoSize = true;
      this.chkShift2.Location = new System.Drawing.Point(314, 63);
      this.chkShift2.Name = "chkShift2";
      this.chkShift2.Size = new System.Drawing.Size(47, 17);
      this.chkShift2.TabIndex = 10;
      this.chkShift2.Text = "Shift";
      this.chkShift2.UseVisualStyleBackColor = true;
      this.labelAlternateKey.AutoSize = true;
      this.labelAlternateKey.Location = new System.Drawing.Point(12, 62);
      this.labelAlternateKey.Name = "labelAlternateKey";
      this.labelAlternateKey.Size = new System.Drawing.Size(52, 13);
      this.labelAlternateKey.TabIndex = 7;
      this.labelAlternateKey.Text = "Alternate:";
      this.chkAlt2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkAlt2.AutoSize = true;
      this.chkAlt2.Location = new System.Drawing.Point(378, 63);
      this.chkAlt2.Name = "chkAlt2";
      this.chkAlt2.Size = new System.Drawing.Size(38, 17);
      this.chkAlt2.TabIndex = 11;
      this.chkAlt2.Text = "Alt";
      this.chkAlt2.UseVisualStyleBackColor = true;
      this.chkCtrl1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkCtrl1.AutoSize = true;
      this.chkCtrl1.Location = new System.Drawing.Point(259, 37);
      this.chkCtrl1.Name = "chkCtrl1";
      this.chkCtrl1.Size = new System.Drawing.Size(41, 17);
      this.chkCtrl1.TabIndex = 3;
      this.chkCtrl1.Text = "Ctrl";
      this.chkCtrl1.UseVisualStyleBackColor = true;
      this.cbKey1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbKey1.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbKey1.FormattingEnabled = true;
      this.cbKey1.Location = new System.Drawing.Point(81, 34);
      this.cbKey1.Name = "cbKey1";
      this.cbKey1.Size = new System.Drawing.Size(170, 21);
      this.cbKey1.TabIndex = 2;
      this.chkShift1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkShift1.AutoSize = true;
      this.chkShift1.Location = new System.Drawing.Point(314, 37);
      this.chkShift1.Name = "chkShift1";
      this.chkShift1.Size = new System.Drawing.Size(47, 17);
      this.chkShift1.TabIndex = 4;
      this.chkShift1.Text = "Shift";
      this.chkShift1.UseVisualStyleBackColor = true;
      this.labelMainKey.AutoSize = true;
      this.labelMainKey.Location = new System.Drawing.Point(12, 38);
      this.labelMainKey.Name = "labelMainKey";
      this.labelMainKey.Size = new System.Drawing.Size(33, 13);
      this.labelMainKey.TabIndex = 1;
      this.labelMainKey.Text = "Main:";
      this.chkAlt1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkAlt1.AutoSize = true;
      this.chkAlt1.Location = new System.Drawing.Point(378, 37);
      this.chkAlt1.Name = "chkAlt1";
      this.chkAlt1.Size = new System.Drawing.Size(38, 17);
      this.chkAlt1.TabIndex = 5;
      this.chkAlt1.Text = "Alt";
      this.chkAlt1.UseVisualStyleBackColor = true;
      this.AutoScaleMode = AutoScaleMode.Inherit;
      this.Controls.Add((Control) this.lvCommands);
      this.Controls.Add((Control) this.panelKeyEditor);
      this.Name = nameof (KeyboardShortcutEditor);
      this.Size = new System.Drawing.Size(467, 372);
      this.panelKeyEditor.ResumeLayout(false);
      this.panelKeyEditor.PerformLayout();
      this.ResumeLayout(false);
    }

    private class KeyItem
    {
      private readonly CommandKey key;

      public KeyItem(CommandKey key) => this.key = key;

      public CommandKey Key => this.key;

      public override string ToString() => KeyboardCommand.GetKeyName(this.key);
    }
  }
}

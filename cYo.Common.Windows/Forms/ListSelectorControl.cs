// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ListSelectorControl
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.Net.Search;
using cYo.Common.Text;
using cYo.Common.Windows.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ListSelectorControl : UserControl, Popup.INotifyClose
  {
    private static int lastTab;
    private static System.Drawing.Size lastSize;
    private HashSet<string> pool;
    private int tab;
    private bool noUpdate;
    private bool checkShield;
    private bool transfered;
    private static readonly Image dropDownImage = (Image) Resources.Route;
    private IContainer components;
    private ListBox lbOwn;
    private ListBox lbPool;
    private Button btAllToOwn;
    private Button btSelectedToOwn;
    private Button btSelectedToPool;
    private Button btAllToPool;
    private CheckedListBoxEx lbCheckList;
    private Panel listPanel;
    private Button btLists;
    private Button btCheck;
    private Button btText;
    private TextBox text;

    public ListSelectorControl()
    {
      this.InitializeComponent();
      this.MinimumSize = this.Size;
      this.MaximumSize = new System.Drawing.Size(this.Width * 2, this.Height * 2);
      this.ResizeRedraw = true;
      if (!ListSelectorControl.lastSize.IsEmpty)
        this.Size = ListSelectorControl.lastSize;
      this.Tab = ListSelectorControl.lastTab;
      LocalizeUtility.Localize((Control) this, (IContainer) null);
    }

    public HashSet<string> Pool
    {
      get => this.pool;
      set
      {
        this.pool = value;
        this.OnPoolChanged();
      }
    }

    public int Tab
    {
      get => this.tab;
      set
      {
        this.tab = ListSelectorControl.lastTab = value;
        switch (this.tab)
        {
          case 1:
            this.btLists.BackColor = SystemColors.Window;
            this.btCheck.BackColor = SystemColors.Control;
            this.btText.BackColor = SystemColors.Window;
            this.listPanel.Visible = false;
            this.lbCheckList.Visible = true;
            this.text.Visible = false;
            break;
          case 2:
            this.btLists.BackColor = SystemColors.Window;
            this.btCheck.BackColor = SystemColors.Window;
            this.btText.BackColor = SystemColors.Control;
            this.listPanel.Visible = false;
            this.lbCheckList.Visible = false;
            this.text.Visible = true;
            break;
          default:
            this.btLists.BackColor = SystemColors.Control;
            this.btCheck.BackColor = SystemColors.Window;
            this.btText.BackColor = SystemColors.Window;
            this.listPanel.Visible = true;
            this.lbCheckList.Visible = false;
            this.text.Visible = false;
            break;
        }
      }
    }

    private void RegisterSearch(IEnumerable<INetSearch> search)
    {
      if (search == null || search.IsEmpty<INetSearch>())
        return;
      TextBoxContextMenu.Register((TextBoxBase) this.text, TextBoxContextMenu.AddSearchLinks(search));
    }

    private void OnPoolChanged()
    {
      HashSet<string> set = this.Text.ListStringToSet(',');
      if (this.pool == null)
        this.pool = new HashSet<string>((IEnumerable<string>) set);
      else
        this.pool.RemoveRange<string>((IEnumerable<string>) set);
      this.lbOwn.BeginUpdate();
      this.lbPool.BeginUpdate();
      this.lbCheckList.BeginUpdate();
      this.lbOwn.Items.Clear();
      this.lbPool.Items.Clear();
      this.lbCheckList.Items.Clear();
      this.lbOwn.Items.AddRange((object[]) set.ToArray<string>());
      this.lbPool.Items.AddRange((object[]) this.pool.ToArray<string>());
      this.lbCheckList.Items.AddRange((object[]) set.ToArray<string>());
      this.CheckAll((CheckedListBox) this.lbCheckList, true);
      this.lbCheckList.Items.AddRange((object[]) this.pool.ToArray<string>());
      this.lbOwn.EndUpdate();
      this.lbPool.EndUpdate();
      this.lbCheckList.EndUpdate();
      this.UpdateButtonStates();
    }

    [Browsable(true)]
    public override string Text
    {
      get => base.Text;
      set => base.Text = value;
    }

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      if (this.noUpdate)
        return;
      if (this.Tab == 2)
        this.text.Text = this.Text;
      this.OnPoolChanged();
    }

    protected override void WndProc(ref Message m)
    {
      if (this.Parent is Popup parent && parent.ProcessResizing(ref m))
        return;
      base.WndProc(ref m);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      if (!this.IsHandleCreated)
        return;
      ListSelectorControl.lastSize = this.Size;
    }

    private void btAllToOwn_Click(object sender, EventArgs e)
    {
      this.CheckAll((CheckedListBox) this.lbCheckList, true);
      this.TransferAll(this.lbPool, this.lbOwn);
    }

    private void btSelectedToOwn_Click(object sender, EventArgs e)
    {
      this.CheckSelected((CheckedListBox) this.lbCheckList, this.lbPool, true);
      this.TransferSelected(this.lbPool, this.lbOwn);
    }

    private void btSelectedToPool_Click(object sender, EventArgs e)
    {
      this.CheckSelected((CheckedListBox) this.lbCheckList, this.lbOwn, false);
      this.TransferSelected(this.lbOwn, this.lbPool);
    }

    private void btAllToPool_Click(object sender, EventArgs e)
    {
      this.CheckAll((CheckedListBox) this.lbCheckList, false);
      this.TransferAll(this.lbOwn, this.lbPool);
    }

    private void lbOwn_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.UpdateButtonStates();
    }

    private void lbOwn_DoubleClick(object sender, EventArgs e)
    {
      this.CheckSelected((CheckedListBox) this.lbCheckList, this.lbOwn, false);
      this.TransferSelected(this.lbOwn, this.lbPool);
    }

    private void lbPool_DoubleClick(object sender, EventArgs e)
    {
      this.CheckSelected((CheckedListBox) this.lbCheckList, this.lbPool, true);
      this.TransferSelected(this.lbPool, this.lbOwn);
    }

    private void lbPool_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Return)
        return;
      this.lbPool_DoubleClick(sender, EventArgs.Empty);
    }

    private void lbOwn_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Return)
        return;
      this.lbOwn_DoubleClick(sender, EventArgs.Empty);
    }

    private void lbCheckList_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      if (this.checkShield)
        return;
      if (e.NewValue == CheckState.Checked)
      {
        this.lbOwn.Items.Add(this.lbCheckList.Items[e.Index]);
        this.lbPool.Items.Remove(this.lbCheckList.Items[e.Index]);
      }
      else
      {
        this.lbOwn.Items.Remove(this.lbCheckList.Items[e.Index]);
        this.lbPool.Items.Add(this.lbCheckList.Items[e.Index]);
      }
      this.transfered = true;
      this.UpdateButtonStates();
    }

    private void btLists_Click(object sender, EventArgs e)
    {
      if (this.Tab == 2)
        this.Text = this.text.Text.Replace("\r\n", " ");
      this.Tab = 0;
    }

    private void btCheck_Click(object sender, EventArgs e)
    {
      if (this.Tab == 2)
        this.Text = this.text.Text.Replace("\r\n", " ");
      this.Tab = 1;
    }

    private void btText_Click(object sender, EventArgs e)
    {
      this.text.Text = ListSelectorControl.GetListboxItems(this.lbOwn, ", ");
      this.Tab = 2;
    }

    private void CheckAll(CheckedListBox clb, bool state)
    {
      this.checkShield = true;
      clb.BeginUpdate();
      for (int index = 0; index < clb.Items.Count; ++index)
        clb.SetItemChecked(index, state);
      clb.EndUpdate();
      this.checkShield = false;
    }

    private void CheckSelected(CheckedListBox clb, ListBox lb, bool state)
    {
      this.checkShield = true;
      clb.BeginUpdate();
      foreach (string selectedItem in lb.SelectedItems)
        clb.SetItemChecked(clb.Items.IndexOf((object) selectedItem), state);
      clb.EndUpdate();
      this.checkShield = false;
    }

    private void UpdateButtonStates()
    {
      this.btAllToOwn.Enabled = this.lbPool.Items.Count > 0;
      this.btAllToPool.Enabled = this.lbOwn.Items.Count > 0;
      this.btSelectedToOwn.Enabled = this.lbPool.SelectedIndices.Count > 0;
      this.btSelectedToPool.Enabled = this.lbOwn.SelectedItems.Count > 0;
    }

    private static string GetListboxItems(ListBox lb, string separator)
    {
      return lb.Items.ToListString(separator);
    }

    private void TransferAll(ListBox a, ListBox b)
    {
      a.BeginUpdate();
      b.BeginUpdate();
      b.Items.AddRange(a.Items);
      a.Items.Clear();
      b.EndUpdate();
      a.EndUpdate();
      this.transfered = true;
      this.UpdateButtonStates();
    }

    private void TransferSelected(ListBox a, ListBox b)
    {
      a.BeginUpdate();
      b.BeginUpdate();
      foreach (string selectedItem in a.SelectedItems)
        b.Items.Add((object) selectedItem);
      b.ClearSelected();
      foreach (string selectedItem in a.SelectedItems)
        b.SelectedItem = (object) selectedItem;
      foreach (string selectedItem in b.SelectedItems)
        a.Items.Remove((object) selectedItem);
      b.EndUpdate();
      a.EndUpdate();
      this.transfered = true;
      this.UpdateButtonStates();
    }

    public void PopupClosed()
    {
      this.noUpdate = true;
      if (this.Tab == 2)
      {
        this.Text = this.text.Text.Replace("\r\n", " ");
      }
      else
      {
        if (!this.transfered)
          return;
        this.Text = ListSelectorControl.GetListboxItems(this.lbOwn, ", ");
      }
    }

    public static void Register(TextBox textBox, IEnumerable<INetSearch> search = null)
    {
      int width = FormUtility.ScaleDpiX(16);
      textBox.Width -= width;
      Button bt = new Button();
      textBox.Parent.Controls.Add((Control) bt);
      textBox.Parent.Controls.SetChildIndex((Control) bt, 0);
      bt.Bounds = new Rectangle(textBox.Right, textBox.Top, width, textBox.Height);
      bt.BackgroundImage = ListSelectorControl.dropDownImage;
      bt.BackgroundImageLayout = ImageLayout.Center;
      bt.TabStop = false;
      bt.Click += (EventHandler) ((sender, e) =>
      {
        ListSelectorControl.ShowPopup(textBox, search).Closed += (ToolStripDropDownClosedEventHandler) ((s, ea) => bt.Enabled = true);
        bt.Enabled = false;
      });
      if (!textBox.Multiline)
        textBox.KeyUp += (KeyEventHandler) ((s2, ea2) =>
        {
          if (ea2.KeyCode != Keys.Down)
            return;
          ListSelectorControl.ShowPopup(textBox).Closed += (ToolStripDropDownClosedEventHandler) ((s, ea) => bt.Enabled = true);
          bt.Enabled = false;
        });
      bt.Visible = true;
    }

    public static void Register(IEnumerable<INetSearch> search = null, params TextBox[] textBoxes)
    {
      foreach (TextBox textBox in textBoxes)
        ListSelectorControl.Register(textBox, search);
    }

    public static Popup ShowPopup(TextBox textBox, IEnumerable<INetSearch> search = null)
    {
      ListSelectorControl ls = new ListSelectorControl();
      string text = textBox.Text;
      if (string.IsNullOrEmpty(text) && textBox is IPromptText)
        text = ((IPromptText) textBox).Text;
      ls.Text = text;
      ls.Pool = ListSelectorControl.SetFromAutoComplete(textBox);
      ls.RegisterSearch(search);
      Popup popup = new Popup((Control) ls, true)
      {
        ShowingAnimation = Popup.PopupAnimations.TopToBottom | Popup.PopupAnimations.Slide,
        Resizable = true
      };
      popup.PopupClosed += (EventHandler) ((s, ea) => textBox.Text = ls.Text);
      popup.Show((Control) textBox);
      return popup;
    }

    private static HashSet<string> SetFromAutoComplete(TextBox textBox)
    {
      HashSet<string> list = new HashSet<string>();
      if (textBox is IDelayedAutoCompleteList)
        ((IDelayedAutoCompleteList) textBox).BuildAutoComplete();
      if (textBox.AutoCompleteCustomSource == null)
        return list;
      foreach (string listString in textBox.AutoCompleteCustomSource)
        list.AddRange<string>(listString.FromListString(','));
      return list;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.lbOwn = new ListBox();
      this.lbPool = new ListBox();
      this.btAllToOwn = new Button();
      this.btSelectedToOwn = new Button();
      this.btSelectedToPool = new Button();
      this.btAllToPool = new Button();
      this.listPanel = new Panel();
      this.btLists = new Button();
      this.btCheck = new Button();
      this.btText = new Button();
      this.text = new TextBox();
      this.lbCheckList = new CheckedListBoxEx();
      this.listPanel.SuspendLayout();
      this.SuspendLayout();
      this.lbOwn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.lbOwn.FormattingEnabled = true;
      this.lbOwn.IntegralHeight = false;
      this.lbOwn.Location = new System.Drawing.Point(0, 0);
      this.lbOwn.MultiColumn = true;
      this.lbOwn.Name = "lbOwn";
      this.lbOwn.SelectionMode = SelectionMode.MultiExtended;
      this.lbOwn.Size = new System.Drawing.Size(245, 95);
      this.lbOwn.Sorted = true;
      this.lbOwn.TabIndex = 0;
      this.lbOwn.SelectedIndexChanged += new EventHandler(this.lbOwn_SelectedIndexChanged);
      this.lbOwn.DoubleClick += new EventHandler(this.lbOwn_DoubleClick);
      this.lbOwn.KeyDown += new KeyEventHandler(this.lbOwn_KeyDown);
      this.lbPool.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.lbPool.FormattingEnabled = true;
      this.lbPool.IntegralHeight = false;
      this.lbPool.Location = new System.Drawing.Point(0, 101);
      this.lbPool.MultiColumn = true;
      this.lbPool.Name = "lbPool";
      this.lbPool.SelectionMode = SelectionMode.MultiExtended;
      this.lbPool.Size = new System.Drawing.Size(291, 142);
      this.lbPool.Sorted = true;
      this.lbPool.TabIndex = 5;
      this.lbPool.SelectedIndexChanged += new EventHandler(this.lbOwn_SelectedIndexChanged);
      this.lbPool.DoubleClick += new EventHandler(this.lbPool_DoubleClick);
      this.lbPool.KeyDown += new KeyEventHandler(this.lbPool_KeyDown);
      this.btAllToOwn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btAllToOwn.Location = new System.Drawing.Point(251, 0);
      this.btAllToOwn.Name = "btAllToOwn";
      this.btAllToOwn.Size = new System.Drawing.Size(40, 23);
      this.btAllToOwn.TabIndex = 1;
      this.btAllToOwn.Text = "<<";
      this.btAllToOwn.UseVisualStyleBackColor = true;
      this.btAllToOwn.Click += new EventHandler(this.btAllToOwn_Click);
      this.btSelectedToOwn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btSelectedToOwn.Location = new System.Drawing.Point(251, 25);
      this.btSelectedToOwn.Name = "btSelectedToOwn";
      this.btSelectedToOwn.Size = new System.Drawing.Size(40, 23);
      this.btSelectedToOwn.TabIndex = 2;
      this.btSelectedToOwn.Text = "<";
      this.btSelectedToOwn.UseVisualStyleBackColor = true;
      this.btSelectedToOwn.Click += new EventHandler(this.btSelectedToOwn_Click);
      this.btSelectedToPool.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btSelectedToPool.Location = new System.Drawing.Point(251, 49);
      this.btSelectedToPool.Name = "btSelectedToPool";
      this.btSelectedToPool.Size = new System.Drawing.Size(40, 23);
      this.btSelectedToPool.TabIndex = 3;
      this.btSelectedToPool.Text = ">";
      this.btSelectedToPool.UseVisualStyleBackColor = true;
      this.btSelectedToPool.Click += new EventHandler(this.btSelectedToPool_Click);
      this.btAllToPool.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btAllToPool.Location = new System.Drawing.Point(251, 73);
      this.btAllToPool.Name = "btAllToPool";
      this.btAllToPool.Size = new System.Drawing.Size(40, 23);
      this.btAllToPool.TabIndex = 4;
      this.btAllToPool.Text = ">>";
      this.btAllToPool.UseVisualStyleBackColor = true;
      this.btAllToPool.Click += new EventHandler(this.btAllToPool_Click);
      this.listPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.listPanel.Controls.Add((Control) this.lbOwn);
      this.listPanel.Controls.Add((Control) this.btAllToPool);
      this.listPanel.Controls.Add((Control) this.btSelectedToOwn);
      this.listPanel.Controls.Add((Control) this.lbPool);
      this.listPanel.Controls.Add((Control) this.btAllToOwn);
      this.listPanel.Controls.Add((Control) this.btSelectedToPool);
      this.listPanel.Location = new System.Drawing.Point(7, 7);
      this.listPanel.Margin = new Padding(0);
      this.listPanel.Name = "listPanel";
      this.listPanel.Size = new System.Drawing.Size(291, 243);
      this.listPanel.TabIndex = 11;
      this.btLists.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btLists.FlatAppearance.CheckedBackColor = SystemColors.ControlLight;
      this.btLists.FlatStyle = FlatStyle.Flat;
      this.btLists.Font = new Font("Microsoft Sans Serif", 7f);
      this.btLists.Location = new System.Drawing.Point(11, 245);
      this.btLists.Name = "btLists";
      this.btLists.Size = new System.Drawing.Size(67, 24);
      this.btLists.TabIndex = 14;
      this.btLists.Text = "&Lists";
      this.btLists.TextAlign = ContentAlignment.BottomCenter;
      this.btLists.UseVisualStyleBackColor = true;
      this.btLists.Click += new EventHandler(this.btLists_Click);
      this.btCheck.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btCheck.FlatAppearance.CheckedBackColor = SystemColors.ControlLight;
      this.btCheck.FlatStyle = FlatStyle.Flat;
      this.btCheck.Font = new Font("Microsoft Sans Serif", 7f);
      this.btCheck.Location = new System.Drawing.Point(79, 245);
      this.btCheck.Name = "btCheck";
      this.btCheck.Size = new System.Drawing.Size(67, 24);
      this.btCheck.TabIndex = 15;
      this.btCheck.Text = "&Check";
      this.btCheck.TextAlign = ContentAlignment.BottomCenter;
      this.btCheck.UseVisualStyleBackColor = true;
      this.btCheck.Click += new EventHandler(this.btCheck_Click);
      this.btText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btText.FlatAppearance.CheckedBackColor = SystemColors.ControlLight;
      this.btText.FlatStyle = FlatStyle.Flat;
      this.btText.Font = new Font("Microsoft Sans Serif", 7f);
      this.btText.Location = new System.Drawing.Point(147, 245);
      this.btText.Name = "btText";
      this.btText.Size = new System.Drawing.Size(67, 24);
      this.btText.TabIndex = 16;
      this.btText.Text = "&Text";
      this.btText.TextAlign = ContentAlignment.BottomCenter;
      this.btText.UseVisualStyleBackColor = true;
      this.btText.Click += new EventHandler(this.btText_Click);
      this.text.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.text.BorderStyle = BorderStyle.FixedSingle;
      this.text.Location = new System.Drawing.Point(7, 7);
      this.text.Multiline = true;
      this.text.Name = "text";
      this.text.Size = new System.Drawing.Size(291, 243);
      this.text.TabIndex = 17;
      this.lbCheckList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.lbCheckList.FormattingEnabled = true;
      this.lbCheckList.IntegralHeight = false;
      this.lbCheckList.Location = new System.Drawing.Point(7, 7);
      this.lbCheckList.MultiColumn = true;
      this.lbCheckList.Name = "lbCheckList";
      this.lbCheckList.Size = new System.Drawing.Size(291, 243);
      this.lbCheckList.Sorted = true;
      this.lbCheckList.TabIndex = 0;
      this.lbCheckList.ItemCheck += new ItemCheckEventHandler(this.lbCheckList_ItemCheck);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.BorderStyle = BorderStyle.FixedSingle;
      this.Controls.Add((Control) this.text);
      this.Controls.Add((Control) this.lbCheckList);
      this.Controls.Add((Control) this.listPanel);
      this.Controls.Add((Control) this.btLists);
      this.Controls.Add((Control) this.btCheck);
      this.Controls.Add((Control) this.btText);
      this.Margin = new Padding(0);
      this.Name = nameof (ListSelectorControl);
      this.Padding = new Padding(4);
      this.Size = new System.Drawing.Size(308, 278);
      this.listPanel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

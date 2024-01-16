// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ListEditorDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Mathematics;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ListEditorDialog : Form
  {
    private Action newAction;
    private Action editAction;
    private Action activateAction;
    private Action setAllAction;
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private ListViewEx lvItems;
    private ColumnHeader chName;
    private ColumnHeader chDescription;
    private Button btMoveDown;
    private Button btMoveUp;
    private Button btDelete;
    private FlowLayoutPanel flowLayoutPanel1;
    private Button btNew;
    private Button btEdit;
    private Button btMoveTop;
    private Button btMoveBottom;
    private Button btActivate;
    private Button btSetAll;

    public ListEditorDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.RestorePosition();
    }

    public IList Items { get; set; }

    public object SelectedItem
    {
      get
      {
        return this.lvItems.SelectedItems.Count != 0 ? this.lvItems.SelectedItems[0].Tag : (object) null;
      }
    }

    private void FillList()
    {
      object selectedItem = this.SelectedItem;
      this.lvItems.BeginUpdate();
      this.lvItems.Items.Clear();
      foreach (object obj in (IEnumerable) this.Items)
      {
        INamed named = obj as INamed;
        IDescription description = obj as IDescription;
        ListViewItem listViewItem = this.lvItems.Items.Add(named != null ? named.Name : obj.ToString());
        if (description != null)
          listViewItem.SubItems.Add(description.Description);
        listViewItem.Tag = obj;
        listViewItem.Selected = obj == selectedItem;
      }
      if (this.lvItems.SelectedIndices.Count == 0 && this.lvItems.Items.Count > 0)
        this.lvItems.Items[0].Selected = true;
      if (this.lvItems.SelectedIndices.Count > 0)
        this.lvItems.EnsureVisible(this.lvItems.SelectedIndices[0]);
      this.lvItems.EndUpdate();
      this.UpdateButtons();
    }

    private void MoveSelected(int offset, bool absolute = false)
    {
      if (this.SelectedItem == null)
        return;
      int oldIndex = this.Items.IndexOf(this.SelectedItem);
      int newIndex = (absolute ? offset : oldIndex + offset).Clamp(0, this.Items.Count);
      this.Items.Move(oldIndex, newIndex);
      this.FillList();
    }

    private void UpdateButtons()
    {
      bool flag = this.lvItems.SelectedItems.Count > 0;
      this.btMoveTop.Enabled = this.btMoveUp.Enabled = flag && this.lvItems.SelectedIndices[0] > 0;
      this.btMoveBottom.Enabled = this.btMoveDown.Enabled = flag && this.lvItems.SelectedIndices[0] < this.Items.Count - 1;
      this.btEdit.Enabled = this.btActivate.Enabled = this.btDelete.Enabled = flag;
    }

    private void lvItems_MouseReorder(object sender, ListViewEx.MouseReorderEventArgs e)
    {
      e.Cancel = true;
      this.MoveSelected(e.ToIndex, true);
    }

    private void btMoveTop_Click(object sender, EventArgs e) => this.MoveSelected(0, true);

    private void btMoveUp_Click(object sender, EventArgs e) => this.MoveSelected(-1);

    private void btMoveDown_Click(object sender, EventArgs e) => this.MoveSelected(1);

    private void btMoveBottom_Click(object sender, EventArgs e)
    {
      this.MoveSelected(this.Items.Count - 1, true);
    }

    private void btDelete_Click(object sender, EventArgs e)
    {
      this.Items.Remove(this.SelectedItem);
      this.FillList();
    }

    private void lvItems_SelectedIndexChanged(object sender, EventArgs e) => this.UpdateButtons();

    private void btNew_Click(object sender, EventArgs e) => this.OnNew();

    private void btEdit_Click(object sender, EventArgs e) => this.OnEdit();

    private void btActivate_Click(object sender, EventArgs e) => this.OnActivate();

    private void btSetAll_Click(object sender, EventArgs e) => this.OnSetAll();

    private void lvItems_DoubleClick(object sender, EventArgs e)
    {
      if (this.editAction != null)
        this.OnEdit();
      else
        this.OnActivate();
    }

    protected virtual void OnNew()
    {
      if (this.newAction == null)
        return;
      this.newAction();
    }

    protected virtual void OnEdit()
    {
      if (this.editAction == null)
        return;
      this.editAction();
    }

    protected virtual void OnActivate()
    {
      if (this.activateAction == null)
        return;
      this.activateAction();
    }

    protected virtual void OnSetAll()
    {
      if (this.setAllAction == null)
        return;
      this.setAllAction();
    }

    public static IList<T> Show<T>(
      IWin32Window parent,
      string caption,
      IList<T> items,
      Func<T> newAction = null,
      Func<T, bool> editAction = null,
      Action<T> activateAction = null,
      Action<T> setAllAction = null)
      where T : class
    {
      items = (IList<T>) items.ToList<T>();
      using (ListEditorDialog dlg = new ListEditorDialog())
      {
        dlg.Text = caption;
        dlg.Items = (IList) items;
        dlg.FillList();
        if (newAction != null)
        {
          dlg.btNew.Visible = true;
          dlg.newAction = (Action) (() =>
          {
            T obj = newAction();
            if ((object) obj == null)
              return;
            ((ICollection<T>) items).Add(obj);
            dlg.FillList();
          });
        }
        if (editAction != null)
        {
          dlg.btEdit.Visible = true;
          dlg.editAction = (Action) (() =>
          {
            if (!(dlg.SelectedItem is T selectedItem2) || !editAction(selectedItem2))
              return;
            dlg.FillList();
          });
        }
        if (activateAction != null)
        {
          dlg.btActivate.Visible = true;
          dlg.activateAction = (Action) (() =>
          {
            if (!(dlg.SelectedItem is T selectedItem4))
              return;
            activateAction(selectedItem4);
          });
        }
        if (setAllAction != null)
        {
          dlg.btSetAll.Visible = true;
          dlg.setAllAction = (Action) (() =>
          {
            if (!(dlg.SelectedItem is T selectedItem6))
              return;
            setAllAction(selectedItem6);
          });
        }
        return dlg.ShowDialog(parent) == DialogResult.Cancel ? (IList<T>) null : items;
      }
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
      this.lvItems = new ListViewEx();
      this.chName = new ColumnHeader();
      this.chDescription = new ColumnHeader();
      this.btMoveDown = new Button();
      this.btMoveUp = new Button();
      this.btDelete = new Button();
      this.flowLayoutPanel1 = new FlowLayoutPanel();
      this.btNew = new Button();
      this.btEdit = new Button();
      this.btActivate = new Button();
      this.btSetAll = new Button();
      this.btMoveTop = new Button();
      this.btMoveBottom = new Button();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(407, 353);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(94, 24);
      this.btCancel.TabIndex = 3;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(407, 323);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(94, 24);
      this.btOK.TabIndex = 2;
      this.btOK.Text = "&OK";
      this.lvItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.lvItems.Columns.AddRange(new ColumnHeader[2]
      {
        this.chName,
        this.chDescription
      });
      this.lvItems.EnableMouseReorder = true;
      this.lvItems.FullRowSelect = true;
      this.lvItems.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.lvItems.HideSelection = false;
      this.lvItems.Location = new System.Drawing.Point(12, 15);
      this.lvItems.MultiSelect = false;
      this.lvItems.Name = "lvItems";
      this.lvItems.Size = new System.Drawing.Size(389, 361);
      this.lvItems.TabIndex = 0;
      this.lvItems.UseCompatibleStateImageBehavior = false;
      this.lvItems.View = View.Details;
      this.lvItems.MouseReorder += new EventHandler<ListViewEx.MouseReorderEventArgs>(this.lvItems_MouseReorder);
      this.lvItems.SelectedIndexChanged += new EventHandler(this.lvItems_SelectedIndexChanged);
      this.lvItems.DoubleClick += new EventHandler(this.lvItems_DoubleClick);
      this.chName.Text = "Name";
      this.chName.Width = 154;
      this.chDescription.Text = "Description";
      this.chDescription.Width = 202;
      this.btMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btMoveDown.Location = new System.Drawing.Point(3, 224);
      this.btMoveDown.Name = "btMoveDown";
      this.btMoveDown.Size = new System.Drawing.Size(94, 23);
      this.btMoveDown.TabIndex = 6;
      this.btMoveDown.Text = "Move &Down";
      this.btMoveDown.UseVisualStyleBackColor = true;
      this.btMoveDown.Click += new EventHandler(this.btMoveDown_Click);
      this.btMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btMoveUp.Location = new System.Drawing.Point(3, 195);
      this.btMoveUp.Name = "btMoveUp";
      this.btMoveUp.Size = new System.Drawing.Size(94, 23);
      this.btMoveUp.TabIndex = 5;
      this.btMoveUp.Text = "Move &Up";
      this.btMoveUp.UseVisualStyleBackColor = true;
      this.btMoveUp.Click += new EventHandler(this.btMoveUp_Click);
      this.btDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btDelete.Location = new System.Drawing.Point(3, 61);
      this.btDelete.Name = "btDelete";
      this.btDelete.Size = new System.Drawing.Size(94, 23);
      this.btDelete.TabIndex = 3;
      this.btDelete.Text = "D&elete";
      this.btDelete.UseVisualStyleBackColor = true;
      this.btDelete.Click += new EventHandler(this.btDelete_Click);
      this.flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.flowLayoutPanel1.Controls.Add((Control) this.btNew);
      this.flowLayoutPanel1.Controls.Add((Control) this.btEdit);
      this.flowLayoutPanel1.Controls.Add((Control) this.btDelete);
      this.flowLayoutPanel1.Controls.Add((Control) this.btActivate);
      this.flowLayoutPanel1.Controls.Add((Control) this.btSetAll);
      this.flowLayoutPanel1.Controls.Add((Control) this.btMoveTop);
      this.flowLayoutPanel1.Controls.Add((Control) this.btMoveUp);
      this.flowLayoutPanel1.Controls.Add((Control) this.btMoveDown);
      this.flowLayoutPanel1.Controls.Add((Control) this.btMoveBottom);
      this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(404, 13);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(101, 304);
      this.flowLayoutPanel1.TabIndex = 1;
      this.btNew.Location = new System.Drawing.Point(3, 3);
      this.btNew.Name = "btNew";
      this.btNew.Size = new System.Drawing.Size(94, 23);
      this.btNew.TabIndex = 0;
      this.btNew.Text = "&New...";
      this.btNew.UseVisualStyleBackColor = true;
      this.btNew.Visible = false;
      this.btNew.Click += new EventHandler(this.btNew_Click);
      this.btEdit.Location = new System.Drawing.Point(3, 32);
      this.btEdit.Name = "btEdit";
      this.btEdit.Size = new System.Drawing.Size(94, 23);
      this.btEdit.TabIndex = 1;
      this.btEdit.Text = "&Edit...";
      this.btEdit.UseVisualStyleBackColor = true;
      this.btEdit.Visible = false;
      this.btEdit.Click += new EventHandler(this.btEdit_Click);
      this.btActivate.Location = new System.Drawing.Point(3, 99);
      this.btActivate.Margin = new Padding(3, 12, 3, 3);
      this.btActivate.Name = "btActivate";
      this.btActivate.Size = new System.Drawing.Size(94, 23);
      this.btActivate.TabIndex = 2;
      this.btActivate.Text = "&Activate";
      this.btActivate.UseVisualStyleBackColor = true;
      this.btActivate.Visible = false;
      this.btActivate.Click += new EventHandler(this.btActivate_Click);
      this.btSetAll.Location = new System.Drawing.Point(3, 128);
      this.btSetAll.Name = "btSetAll";
      this.btSetAll.Size = new System.Drawing.Size(94, 23);
      this.btSetAll.TabIndex = 8;
      this.btSetAll.Text = "&Set to All";
      this.btSetAll.UseVisualStyleBackColor = true;
      this.btSetAll.Visible = false;
      this.btSetAll.Click += new EventHandler(this.btSetAll_Click);
      this.btMoveTop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btMoveTop.Location = new System.Drawing.Point(3, 166);
      this.btMoveTop.Margin = new Padding(3, 12, 3, 3);
      this.btMoveTop.Name = "btMoveTop";
      this.btMoveTop.Size = new System.Drawing.Size(94, 23);
      this.btMoveTop.TabIndex = 4;
      this.btMoveTop.Text = "Move &Top";
      this.btMoveTop.UseVisualStyleBackColor = true;
      this.btMoveTop.Click += new EventHandler(this.btMoveTop_Click);
      this.btMoveBottom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btMoveBottom.Location = new System.Drawing.Point(3, 253);
      this.btMoveBottom.Name = "btMoveBottom";
      this.btMoveBottom.Size = new System.Drawing.Size(94, 23);
      this.btMoveBottom.TabIndex = 7;
      this.btMoveBottom.Text = "Move &Bottom";
      this.btMoveBottom.UseVisualStyleBackColor = true;
      this.btMoveBottom.Click += new EventHandler(this.btMoveBottom_Click);
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(513, 388);
      this.Controls.Add((Control) this.flowLayoutPanel1);
      this.Controls.Add((Control) this.lvItems);
      this.Controls.Add((Control) this.btOK);
      this.Controls.Add((Control) this.btCancel);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(400, 390);
      this.Name = nameof (ListEditorDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = nameof (ListEditorDialog);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

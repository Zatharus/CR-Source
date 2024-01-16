// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.SelectItemDialog
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class SelectItemDialog : Form
  {
    private string itemCaption;
    private string textValue;
    private readonly List<string> selectionItems = new List<string>();
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private Label lblName;
    private ComboBox cbName;
    private TextBox txtName;
    private CheckBox chkOption;

    public SelectItemDialog()
    {
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
    }

    public string CheckOptionText { get; set; }

    public bool DefaultCheckResult { get; set; }

    public string TextCaption
    {
      get => this.itemCaption;
      set => this.itemCaption = value;
    }

    public string TextValue
    {
      get => this.textValue;
      set => this.textValue = value;
    }

    public List<string> SelectionItems => this.selectionItems;

    public string GetName(IWin32Window parent)
    {
      bool flag = this.selectionItems.Count > 0;
      if (!string.IsNullOrEmpty(this.TextCaption))
        this.lblName.Text = this.itemCaption + ":";
      if (!string.IsNullOrEmpty(this.CheckOptionText))
      {
        this.chkOption.Text = this.CheckOptionText;
        this.chkOption.Checked = this.DefaultCheckResult;
        this.chkOption.Visible = true;
      }
      if (flag)
      {
        this.txtName.Visible = false;
        this.cbName.Items.Clear();
        this.cbName.Items.AddRange((object[]) this.selectionItems.ToArray());
        this.cbName.Text = this.textValue ?? string.Empty;
        this.cbName.SelectAll();
      }
      else
      {
        this.cbName.Visible = false;
        this.txtName.TabIndex = 0;
        this.txtName.Text = this.textValue;
        this.txtName.Bounds = this.cbName.Bounds;
        this.txtName.SelectAll();
      }
      this.btOK.Enabled = !string.IsNullOrEmpty(this.TextValue);
      if (this.ShowDialog(parent) == DialogResult.Cancel)
        return (string) null;
      this.DefaultCheckResult = this.chkOption.Checked;
      return (flag ? this.cbName.Text : this.txtName.Text).Trim();
    }

    private void NameTextChanged(object sender, EventArgs e)
    {
      this.btOK.Enabled = !string.IsNullOrEmpty(this.cbName.Visible ? this.cbName.Text : this.txtName.Text);
    }

    public static string GetName<T>(
      IWin32Window parent,
      string caption,
      string itemValue,
      IEnumerable<T> list,
      string itemCaption = null)
    {
      using (SelectItemDialog selectItemDialog = new SelectItemDialog())
      {
        selectItemDialog.Text = caption;
        selectItemDialog.TextValue = itemValue;
        selectItemDialog.TextCaption = itemCaption;
        if (list != null)
          selectItemDialog.SelectionItems.AddRange((IEnumerable<string>) list.Select<T, string>((Func<T, string>) (x => x.ToString())).ToArray<string>());
        return selectItemDialog.GetName(parent);
      }
    }

    public static string GetName(IWin32Window parent, string caption, string itemValue)
    {
      return SelectItemDialog.GetName<string>(parent, caption, itemValue, (IEnumerable<string>) null);
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
      this.lblName = new Label();
      this.cbName = new ComboBox();
      this.txtName = new TextBox();
      this.chkOption = new CheckBox();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(252, 94);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 5;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(166, 94);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 4;
      this.btOK.Text = "&OK";
      this.lblName.AutoSize = true;
      this.lblName.Location = new System.Drawing.Point(12, 24);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(38, 13);
      this.lblName.TabIndex = 0;
      this.lblName.Text = "Name:";
      this.cbName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbName.FormattingEnabled = true;
      this.cbName.Location = new System.Drawing.Point(12, 40);
      this.cbName.Name = "cbName";
      this.cbName.Size = new System.Drawing.Size(320, 21);
      this.cbName.TabIndex = 1;
      this.cbName.TextChanged += new EventHandler(this.NameTextChanged);
      this.txtName.Location = new System.Drawing.Point(12, 95);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(100, 20);
      this.txtName.TabIndex = 2;
      this.txtName.TextChanged += new EventHandler(this.NameTextChanged);
      this.chkOption.AutoSize = true;
      this.chkOption.Location = new System.Drawing.Point(12, 67);
      this.chkOption.Name = "chkOption";
      this.chkOption.Size = new System.Drawing.Size(86, 17);
      this.chkOption.TabIndex = 3;
      this.chkOption.Text = "Lorem Ipsum";
      this.chkOption.UseVisualStyleBackColor = true;
      this.chkOption.Visible = false;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(344, 130);
      this.Controls.Add((Control) this.chkOption);
      this.Controls.Add((Control) this.txtName);
      this.Controls.Add((Control) this.cbName);
      this.Controls.Add((Control) this.lblName);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AddItemDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Add Item Caption";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.DeleteItemDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Text;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class DeleteItemDialog : Form
  {
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private Label lbCaption;
    private ComboBox cbItems;

    public DeleteItemDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      ComboBoxSkinner comboBoxSkinner = new ComboBoxSkinner(this.cbItems);
    }

    public static List<T> GetList<T>(IWin32Window parent, string caption, IEnumerable<T> list) where T : class
    {
      using (DeleteItemDialog deleteItemDialog = new DeleteItemDialog())
      {
        deleteItemDialog.Text = StringUtility.Format(deleteItemDialog.Text, (object) caption);
        deleteItemDialog.lbCaption.Text = string.Format("{0}:", (object) caption);
        List<T> objList = new List<T>(list);
        objList.Sort();
        deleteItemDialog.cbItems.Items.AddRange((object[]) objList.ToArray());
        deleteItemDialog.cbItems.Items.Add((object) new ComboBoxSkinner.ComboBoxSeparator((object) "All"));
        deleteItemDialog.cbItems.SelectedIndex = 0;
        if (deleteItemDialog.ShowDialog(parent) == DialogResult.Cancel)
          return (List<T>) null;
        List<T> list1 = new List<T>();
        if (deleteItemDialog.cbItems.SelectedItem is ComboBoxSkinner.ComboBoxSeparator)
          list1.AddRange(list);
        else
          list1.Add(deleteItemDialog.cbItems.SelectedItem as T);
        return list1;
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
      this.lbCaption = new Label();
      this.cbItems = new ComboBox();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(252, 73);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 3;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(166, 73);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 2;
      this.btOK.Text = "&OK";
      this.lbCaption.AutoSize = true;
      this.lbCaption.Location = new System.Drawing.Point(12, 23);
      this.lbCaption.Name = "lbCaption";
      this.lbCaption.Size = new System.Drawing.Size(24, 13);
      this.lbCaption.TabIndex = 0;
      this.lbCaption.Text = "{0}:";
      this.cbItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cbItems.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbItems.FormattingEnabled = true;
      this.cbItems.Location = new System.Drawing.Point(15, 39);
      this.cbItems.MaxDropDownItems = 10;
      this.cbItems.Name = "cbItems";
      this.cbItems.Size = new System.Drawing.Size(312, 21);
      this.cbItems.TabIndex = 1;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(344, 109);
      this.Controls.Add((Control) this.cbItems);
      this.Controls.Add((Control) this.lbCaption);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (DeleteItemDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Delete Item {0}";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

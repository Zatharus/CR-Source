// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ShowErrorsDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.Sync;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ShowErrorsDialog : Form
  {
    private IContainer components;
    private ListViewEx lvErrors;
    private Button btOk;
    private ColumnHeader chItem;
    private ColumnHeader chProblem;

    public ShowErrorsDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.RestorePosition();
    }

    private void AddError(ShowErrorsDialog.ErrorItem ei)
    {
      this.lvErrors.Items.Add(ei.Item).SubItems.Add(ei.Message);
    }

    public static void ShowErrors<T>(
      IWin32Window parent,
      SmartList<T> errors,
      Func<T, ShowErrorsDialog.ErrorItem> converter)
    {
      if (errors.Count == 0)
        return;
      using (ShowErrorsDialog dlg = new ShowErrorsDialog())
      {
        foreach (T error in errors)
          dlg.AddError(converter(error));
        EventHandler<SmartListChangedEventArgs<T>> eventHandler = (EventHandler<SmartListChangedEventArgs<T>>) ((s, e) =>
        {
          if (e.Action != SmartListAction.Insert)
            return;
          ControlExtensions.BeginInvoke(dlg, (Action) (() => dlg.AddError(converter(e.Item))));
        });
        errors.Changed += eventHandler;
        int num = (int) dlg.ShowDialog(parent);
        errors.Changed -= eventHandler;
        errors.Clear();
      }
    }

    public static ShowErrorsDialog.ErrorItem ComicExporterConverter(ComicExporter ce)
    {
      return new ShowErrorsDialog.ErrorItem()
      {
        Item = ce.ComicBooks[0].FileNameWithExtension,
        Message = ce.LastError
      };
    }

    public static ShowErrorsDialog.ErrorItem DeviceSyncErrorConverter(DeviceSyncError ce)
    {
      return new ShowErrorsDialog.ErrorItem()
      {
        Item = ce.Name,
        Message = ce.Message
      };
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.lvErrors = new ListViewEx();
      this.chItem = new ColumnHeader();
      this.chProblem = new ColumnHeader();
      this.btOk = new Button();
      this.SuspendLayout();
      this.lvErrors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.lvErrors.Columns.AddRange(new ColumnHeader[2]
      {
        this.chItem,
        this.chProblem
      });
      this.lvErrors.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.lvErrors.Location = new System.Drawing.Point(12, 12);
      this.lvErrors.Name = "lvErrors";
      this.lvErrors.Size = new System.Drawing.Size(585, 317);
      this.lvErrors.TabIndex = 0;
      this.lvErrors.UseCompatibleStateImageBehavior = false;
      this.lvErrors.View = View.Details;
      this.chItem.Text = "Item";
      this.chItem.Width = 202;
      this.chProblem.Text = "Problem";
      this.chProblem.Width = 348;
      this.btOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOk.DialogResult = DialogResult.OK;
      this.btOk.Location = new System.Drawing.Point(506, 335);
      this.btOk.Name = "btOk";
      this.btOk.Size = new System.Drawing.Size(95, 23);
      this.btOk.TabIndex = 1;
      this.btOk.Text = "OK";
      this.btOk.UseVisualStyleBackColor = true;
      this.AcceptButton = (IButtonControl) this.btOk;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btOk;
      this.ClientSize = new System.Drawing.Size(609, 368);
      this.Controls.Add((Control) this.btOk);
      this.Controls.Add((Control) this.lvErrors);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(300, 200);
      this.Name = nameof (ShowErrorsDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Errors";
      this.ResumeLayout(false);
    }

    public class ErrorItem
    {
      public string Item { get; set; }

      public string Message { get; set; }
    }
  }
}

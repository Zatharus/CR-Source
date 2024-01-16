// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ZoomDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Mathematics;
using cYo.Common.Windows;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ZoomDialog : Form
  {
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private Label lblPercentage;
    private NumericUpDown numPercentage;

    public ZoomDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
    }

    public float Zoom
    {
      get => (float) (this.numPercentage.Value / 100M);
      set
      {
        this.numPercentage.Value = (Decimal) (int) (value * 100f).Clamp((float) this.numPercentage.Minimum, (float) this.numPercentage.Maximum);
        this.numPercentage.Select(0, 100);
      }
    }

    public static float Show(IWin32Window parent, float zoom)
    {
      using (ZoomDialog zoomDialog = new ZoomDialog())
      {
        zoomDialog.Zoom = zoom;
        if (zoomDialog.ShowDialog(parent) == DialogResult.OK)
          return zoomDialog.Zoom;
      }
      return zoom;
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
      this.lblPercentage = new Label();
      this.numPercentage = new NumericUpDown();
      this.numPercentage.BeginInit();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new Point(232, 58);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new Size(80, 24);
      this.btCancel.TabIndex = 3;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new Point(146, 58);
      this.btOK.Name = "btOK";
      this.btOK.Size = new Size(80, 24);
      this.btOK.TabIndex = 2;
      this.btOK.Text = "&OK";
      this.lblPercentage.AutoSize = true;
      this.lblPercentage.Location = new Point(23, 21);
      this.lblPercentage.Name = "lblPercentage";
      this.lblPercentage.Size = new Size(93, 13);
      this.lblPercentage.TabIndex = 0;
      this.lblPercentage.Text = "Percentage zoom:";
      this.numPercentage.Increment = new Decimal(new int[4]
      {
        10,
        0,
        0,
        0
      });
      this.numPercentage.Location = new Point(146, 19);
      this.numPercentage.Maximum = new Decimal(new int[4]
      {
        800,
        0,
        0,
        0
      });
      this.numPercentage.Minimum = new Decimal(new int[4]
      {
        100,
        0,
        0,
        0
      });
      this.numPercentage.Name = "numPercentage";
      this.numPercentage.Size = new Size(80, 20);
      this.numPercentage.TabIndex = 1;
      this.numPercentage.TextAlign = HorizontalAlignment.Right;
      this.numPercentage.Value = new Decimal(new int[4]
      {
        100,
        0,
        0,
        0
      });
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new Size(320, 92);
      this.Controls.Add((Control) this.numPercentage);
      this.Controls.Add((Control) this.lblPercentage);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ZoomDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Custom Zoom";
      this.numPercentage.EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.DeviceSelectDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class DeviceSelectDialog : Form
  {
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private ListView lvDevices;
    private ColumnHeader colName;
    private ColumnHeader colModel;
    private ColumnHeader colSerial;

    public DeviceSelectDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.lvDevices.Columns.ScaleDpi();
      LocalizeUtility.Localize((Control) this, this.components);
    }

    private void lvDevices_DoubleClick(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Hide();
    }

    private void FillList(IEnumerable<ISyncProvider> syncProviders)
    {
      foreach (ISyncProvider syncProvider in syncProviders)
      {
        ListViewItem listViewItem = new ListViewItem(syncProvider.Device.Name)
        {
          Tag = (object) syncProvider
        };
        listViewItem.SubItems.Add(syncProvider.Device.Model);
        listViewItem.SubItems.Add(syncProvider.Device.SerialNumber);
        this.lvDevices.Items.Add(listViewItem);
      }
      if (this.lvDevices.Items.Count > 0)
        this.lvDevices.Items[0].Selected = true;
      this.lvDevices.Enabled = this.btOK.Enabled = this.lvDevices.Items.Count > 0;
    }

    public static ISyncProvider SelectProvider(
      IWin32Window parent,
      IEnumerable<DeviceSyncSettings> devices)
    {
      using (DeviceSelectDialog deviceSelectDialog = new DeviceSelectDialog())
      {
        List<ISyncProvider> syncProviders = new List<ISyncProvider>();
        AutomaticProgressDialog.Process(parent, TR.Messages["DiscoveringDevicesCaption", "Discovering connected devices"], TR.Messages["DiscoveringDevicesDescription", "Searching all connected Devices for installed ComicRack clients"], 1000, (Action) (() => syncProviders.AddRange(DeviceSyncFactory.Discover())), AutomaticProgressDialogOptions.None);
        deviceSelectDialog.FillList(syncProviders.Where<ISyncProvider>((Func<ISyncProvider, bool>) (sd => devices.All<DeviceSyncSettings>((Func<DeviceSyncSettings, bool>) (d => d.DeviceKey != sd.Device.Key)))));
        return deviceSelectDialog.ShowDialog(parent) == DialogResult.Cancel ? (ISyncProvider) null : (deviceSelectDialog.lvDevices.SelectedItems.Count == 0 ? (ISyncProvider) null : deviceSelectDialog.lvDevices.SelectedItems[0].Tag as ISyncProvider);
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
      this.lvDevices = new ListView();
      this.colName = new ColumnHeader();
      this.colModel = new ColumnHeader();
      this.colSerial = new ColumnHeader();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(303, 170);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 6;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(217, 170);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 5;
      this.btOK.Text = "&OK";
      this.lvDevices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.lvDevices.Columns.AddRange(new ColumnHeader[3]
      {
        this.colName,
        this.colModel,
        this.colSerial
      });
      this.lvDevices.FullRowSelect = true;
      this.lvDevices.Location = new System.Drawing.Point(12, 12);
      this.lvDevices.MultiSelect = false;
      this.lvDevices.Name = "lvDevices";
      this.lvDevices.Size = new System.Drawing.Size(371, 149);
      this.lvDevices.TabIndex = 7;
      this.lvDevices.UseCompatibleStateImageBehavior = false;
      this.lvDevices.View = View.Details;
      this.lvDevices.DoubleClick += new EventHandler(this.lvDevices_DoubleClick);
      this.colName.Text = "Name";
      this.colName.Width = 128;
      this.colModel.Text = "Model";
      this.colModel.Width = 109;
      this.colSerial.Text = "Serial";
      this.colSerial.Width = 102;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(395, 206);
      this.Controls.Add((Control) this.lvDevices);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (DeviceSelectDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Select Device";
      this.ResumeLayout(false);
    }
  }
}

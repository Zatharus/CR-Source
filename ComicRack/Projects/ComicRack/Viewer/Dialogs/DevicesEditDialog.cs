// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.DevicesEditDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Sync;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class DevicesEditDialog : Form
  {
    private static int selectedTab;
    private IContainer components;
    private Button btCancel;
    private Button btOK;
    private TabControlEx tabDevices;
    private Button btPairDevice;
    private Label labelHint;
    private Button btDevice;
    private ContextMenuStrip cmDevice;
    private ToolStripMenuItem miDeviceCopy;
    private ToolStripMenuItem miDevicePaste;
    private ToolStripMenuItem miDeviceCopyToAll;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem miDeviceRename;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem miDeviceUnpair;

    public DevicesEditDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.btDevice.Image = (Image) ((Bitmap) this.btDevice.Image).ScaleDpi();
      this.RestorePosition();
      LocalizeUtility.Localize((Control) this, this.components);
      this.SetVisibility();
    }

    public IList<DeviceSyncSettings> Devices
    {
      get
      {
        return (IList<DeviceSyncSettings>) this.tabDevices.TabPages.Cast<TabPage>().Select<TabPage, DeviceSyncSettings>((Func<TabPage, DeviceSyncSettings>) (tp => ((DeviceEditControl) tp.Tag).Settings)).ToList<DeviceSyncSettings>();
      }
      set
      {
        this.tabDevices.TabPages.Clear();
        foreach (DeviceSyncSettings pd in (IEnumerable<DeviceSyncSettings>) value)
          this.AddTab(pd);
      }
    }

    public DeviceEditControl CurrentDevice
    {
      get
      {
        TabPage selectedTab = this.tabDevices.SelectedTab;
        return selectedTab != null ? selectedTab.Tag as DeviceEditControl : (DeviceEditControl) null;
      }
    }

    private void btPair_Click(object sender, EventArgs e)
    {
      ISyncProvider sd = DeviceSelectDialog.SelectProvider((IWin32Window) this, (IEnumerable<DeviceSyncSettings>) this.Devices);
      if (sd == null || !this.Devices.All<DeviceSyncSettings>((Func<DeviceSyncSettings, bool>) (d => d.DeviceKey != sd.Device.Key)))
        return;
      this.tabDevices.SelectedTab = this.AddTab(new DeviceSyncSettings()
      {
        DeviceName = sd.Device.Name,
        DeviceKey = sd.Device.Key
      });
    }

    private void cmDevice_Opening(object sender, CancelEventArgs e)
    {
      if (this.CurrentDevice == null)
      {
        e.Cancel = true;
      }
      else
      {
        this.miDevicePaste.Enabled = this.CurrentDevice != null && this.CurrentDevice.CanPaste;
        this.miDeviceCopyToAll.Visible = this.tabDevices.TabCount > 1;
      }
    }

    private void miDeviceCopy_Click(object sender, EventArgs e)
    {
      if (this.CurrentDevice == null)
        return;
      this.CurrentDevice.CopyShareSettings();
    }

    private void miDevicePaste_Click(object sender, EventArgs e)
    {
      if (this.CurrentDevice == null)
        return;
      this.CurrentDevice.PasteSharedSettings();
    }

    private void miDeviceCopyToAll_Click(object sender, EventArgs e)
    {
      if (this.CurrentDevice == null)
        return;
      foreach (DeviceEditControl deviceEditControl in this.tabDevices.TabPages.OfType<TabPage>().Select<TabPage, DeviceEditControl>((Func<TabPage, DeviceEditControl>) (t => t.Tag as DeviceEditControl)).Where<DeviceEditControl>((Func<DeviceEditControl, bool>) (dec => dec != null)))
        deviceEditControl.PasteSharedSettings();
    }

    private void btDevice_Click(object sender, EventArgs e)
    {
      this.cmDevice.Show((Control) this.btDevice, new System.Drawing.Point(this.btDevice.Width, this.btDevice.Height), ToolStripDropDownDirection.BelowLeft);
    }

    private void miDeviceRename_Click(object sender, EventArgs e)
    {
      if (this.CurrentDevice == null)
        return;
      string name = SelectItemDialog.GetName((IWin32Window) this, TR.Messages["RenameDevice", "Rename Device"], this.CurrentDevice.DeviceName);
      if (string.IsNullOrEmpty(name))
        return;
      this.CurrentDevice.DeviceName = name;
    }

    private void miDeviceUnpair_Click(object sender, EventArgs e)
    {
      if (this.tabDevices.SelectedTab == null)
        return;
      this.RemoveTab(this.tabDevices.SelectedTab);
    }

    private TabPage AddTab(DeviceSyncSettings pd)
    {
      TabPage tabPage = new TabPage(pd.DeviceName);
      tabPage.Padding = new Padding(10);
      tabPage.UseVisualStyleBackColor = true;
      TabPage tb = tabPage;
      DeviceEditControl deviceEditControl = new DeviceEditControl();
      deviceEditControl.Settings = pd;
      deviceEditControl.Dock = DockStyle.Fill;
      DeviceEditControl se = deviceEditControl;
      se.DeviceNameChanged += (EventHandler) ((s, e) => tb.Text = se.DeviceName);
      tb.Controls.Add((Control) se);
      tb.Tag = (object) se;
      this.tabDevices.TabPages.Add(tb);
      this.SetVisibility();
      return tb;
    }

    private void RemoveTab(TabPage page)
    {
      this.tabDevices.TabPages.Remove(page);
      this.SetVisibility();
    }

    private void SetVisibility()
    {
      this.tabDevices.Visible = this.tabDevices.TabCount > 0;
      this.labelHint.Visible = this.tabDevices.TabCount == 0;
      this.btDevice.Visible = this.tabDevices.TabCount > 0;
    }

    public static bool Show(
      IWin32Window parent,
      IList<DeviceSyncSettings> portableDevices,
      DeviceSyncSettings device = null,
      Guid? listId = null)
    {
      using (DevicesEditDialog devicesEditDialog = new DevicesEditDialog())
      {
        devicesEditDialog.Devices = portableDevices;
        if (device != null)
        {
          int index = portableDevices.IndexOf(device);
          if (index >= 0 && listId.HasValue)
          {
            devicesEditDialog.tabDevices.SelectedIndex = index;
            ((DeviceEditControl) devicesEditDialog.tabDevices.TabPages[index].Tag).SelectList(listId.Value);
          }
        }
        else if (devicesEditDialog.tabDevices.TabCount > 0)
          devicesEditDialog.tabDevices.SelectedIndex = DevicesEditDialog.selectedTab.Clamp(0, devicesEditDialog.tabDevices.TabCount - 1);
        DialogResult dialogResult = devicesEditDialog.ShowDialog(parent);
        DevicesEditDialog.selectedTab = devicesEditDialog.tabDevices.SelectedIndex;
        if (dialogResult == DialogResult.Cancel)
          return false;
        portableDevices.Clear();
        portableDevices.AddRange<DeviceSyncSettings>((IEnumerable<DeviceSyncSettings>) devicesEditDialog.Devices);
        return true;
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
      this.components = (IContainer) new System.ComponentModel.Container();
      this.btCancel = new Button();
      this.btOK = new Button();
      this.tabDevices = new TabControlEx();
      this.btPairDevice = new Button();
      this.labelHint = new Label();
      this.btDevice = new Button();
      this.cmDevice = new ContextMenuStrip(this.components);
      this.miDeviceCopy = new ToolStripMenuItem();
      this.miDevicePaste = new ToolStripMenuItem();
      this.miDeviceCopyToAll = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.miDeviceRename = new ToolStripMenuItem();
      this.toolStripMenuItem2 = new ToolStripSeparator();
      this.miDeviceUnpair = new ToolStripMenuItem();
      this.cmDevice.SuspendLayout();
      this.SuspendLayout();
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(512, 425);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 4;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(426, 425);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 3;
      this.btOK.Text = "&OK";
      this.tabDevices.AllowDrop = true;
      this.tabDevices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.tabDevices.Location = new System.Drawing.Point(12, 12);
      this.tabDevices.Name = "tabDevices";
      this.tabDevices.SelectedIndex = 0;
      this.tabDevices.Size = new System.Drawing.Size(580, 407);
      this.tabDevices.TabIndex = 5;
      this.btPairDevice.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btPairDevice.FlatStyle = FlatStyle.System;
      this.btPairDevice.Location = new System.Drawing.Point(12, 425);
      this.btPairDevice.Name = "btPairDevice";
      this.btPairDevice.Size = new System.Drawing.Size(123, 24);
      this.btPairDevice.TabIndex = 6;
      this.btPairDevice.Text = "Pair with Device...";
      this.btPairDevice.Click += new EventHandler(this.btPair_Click);
      this.labelHint.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.labelHint.Location = new System.Drawing.Point(12, 12);
      this.labelHint.Name = "labelHint";
      this.labelHint.Size = new System.Drawing.Size(580, 407);
      this.labelHint.TabIndex = 7;
      this.labelHint.Text = "Connect your Device with your Computer and press 'Pair Device...'";
      this.labelHint.TextAlign = ContentAlignment.MiddleCenter;
      this.btDevice.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btDevice.Image = (Image) Resources.SmallArrowDown;
      this.btDevice.ImageAlign = ContentAlignment.MiddleRight;
      this.btDevice.Location = new System.Drawing.Point(141, 425);
      this.btDevice.Name = "btDevice";
      this.btDevice.Size = new System.Drawing.Size(100, 24);
      this.btDevice.TabIndex = 8;
      this.btDevice.Text = "Device";
      this.btDevice.UseVisualStyleBackColor = true;
      this.btDevice.Click += new EventHandler(this.btDevice_Click);
      this.cmDevice.Items.AddRange(new ToolStripItem[7]
      {
        (ToolStripItem) this.miDeviceCopy,
        (ToolStripItem) this.miDevicePaste,
        (ToolStripItem) this.miDeviceCopyToAll,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.miDeviceRename,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.miDeviceUnpair
      });
      this.cmDevice.Name = "cmDevice";
      this.cmDevice.Size = new System.Drawing.Size(134, 126);
      this.cmDevice.Opening += new CancelEventHandler(this.cmDevice_Opening);
      this.miDeviceCopy.Image = (Image) Resources.EditCopy;
      this.miDeviceCopy.Name = "miDeviceCopy";
      this.miDeviceCopy.Size = new System.Drawing.Size(133, 22);
      this.miDeviceCopy.Text = "Copy";
      this.miDeviceCopy.Click += new EventHandler(this.miDeviceCopy_Click);
      this.miDevicePaste.Image = (Image) Resources.EditPaste;
      this.miDevicePaste.Name = "miDevicePaste";
      this.miDevicePaste.Size = new System.Drawing.Size(133, 22);
      this.miDevicePaste.Text = "Paste";
      this.miDevicePaste.Click += new EventHandler(this.miDevicePaste_Click);
      this.miDeviceCopyToAll.Name = "miDeviceCopyToAll";
      this.miDeviceCopyToAll.Size = new System.Drawing.Size(133, 22);
      this.miDeviceCopyToAll.Text = "Copy to All";
      this.miDeviceCopyToAll.Click += new EventHandler(this.miDeviceCopyToAll_Click);
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(130, 6);
      this.miDeviceRename.Image = (Image) Resources.Rename;
      this.miDeviceRename.Name = "miDeviceRename";
      this.miDeviceRename.Size = new System.Drawing.Size(133, 22);
      this.miDeviceRename.Text = "Rename...";
      this.miDeviceRename.Click += new EventHandler(this.miDeviceRename_Click);
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(130, 6);
      this.miDeviceUnpair.Name = "miDeviceUnpair";
      this.miDeviceUnpair.Size = new System.Drawing.Size(133, 22);
      this.miDeviceUnpair.Text = "Unpair";
      this.miDeviceUnpair.Click += new EventHandler(this.miDeviceUnpair_Click);
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(604, 461);
      this.Controls.Add((Control) this.btDevice);
      this.Controls.Add((Control) this.btPairDevice);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.Controls.Add((Control) this.labelHint);
      this.Controls.Add((Control) this.tabDevices);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(500, 460);
      this.Name = nameof (DevicesEditDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Devices";
      this.cmDevice.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

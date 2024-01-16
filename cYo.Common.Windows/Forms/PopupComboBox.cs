// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.PopupComboBox
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [ToolboxBitmap(typeof (ComboBox))]
  [ToolboxItem(true)]
  [ToolboxItemFilter("System.Windows.Forms")]
  [Description("Displays an editable text box with a drop-down list of permitted values.")]
  public class PopupComboBox : PopupComboBoxBase
  {
    private Popup dropDown;
    private Control dropDownControl;
    private DateTime dropDownHideTime;

    public PopupComboBox()
    {
      this.dropDownHideTime = DateTime.Now;
      base.DropDownHeight = base.DropDownWidth = 1;
      base.IntegralHeight = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.dropDown != null)
        this.dropDown.Dispose();
      base.Dispose(disposing);
    }

    public Control DropDownControl
    {
      get => this.dropDownControl;
      set
      {
        if (this.dropDownControl == value)
          return;
        this.dropDownControl = value;
        if (this.dropDown != null)
        {
          this.dropDown.Closed -= new ToolStripDropDownClosedEventHandler(this.dropDown_Closed);
          this.dropDown.Dispose();
        }
        this.dropDown = new Popup(value);
        this.dropDown.Closed += new ToolStripDropDownClosedEventHandler(this.dropDown_Closed);
      }
    }

    public void ShowDropDown()
    {
      if (this.dropDown == null)
        return;
      if ((DateTime.Now - this.dropDownHideTime).TotalSeconds > 0.5)
      {
        this.dropDown.Show((Control) this);
      }
      else
      {
        this.dropDownHideTime = DateTime.Now.Subtract(new TimeSpan(0, 0, 1));
        this.Focus();
      }
    }

    public void HideDropDown()
    {
      if (this.dropDown == null)
        return;
      this.dropDown.Hide();
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 8465 && PopupComboBox.NativeMethods.HIWORD(m.WParam) == 7)
        this.ShowDropDown();
      else
        base.WndProc(ref m);
    }

    private void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
    {
      this.dropDownHideTime = DateTime.Now;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new int DropDownWidth
    {
      get => base.DropDownWidth;
      set => base.DropDownWidth = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new int DropDownHeight
    {
      get => base.DropDownHeight;
      set => base.DropDownHeight = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool IntegralHeight
    {
      get => base.IntegralHeight;
      set => base.IntegralHeight = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ComboBox.ObjectCollection Items => base.Items;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new int ItemHeight
    {
      get => base.ItemHeight;
      set => base.ItemHeight = value;
    }

    [SuppressUnmanagedCodeSecurity]
    private static class NativeMethods
    {
      public const int WM_USER = 1024;
      public const int WM_COMMAND = 273;
      public const int WM_REFLECT = 8192;
      public const int CBN_DROPDOWN = 7;

      public static int HIWORD(int n) => n >> 16 & (int) ushort.MaxValue;

      public static int HIWORD(IntPtr n) => PopupComboBox.NativeMethods.HIWORD(n.ToInt32());
    }
  }
}

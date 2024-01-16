// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.ComboBoxEx
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class ComboBoxEx : ComboBox, IPromptText
  {
    private bool quickSelectAll;
    private string promptText;
    private bool focusSelect = true;

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("Appearance")]
    [Description("The prompt text to display when there is nothing in the Text property.")]
    public string PromptText
    {
      get => this.promptText;
      set
      {
        this.promptText = value;
        if (!this.IsHandleCreated)
          return;
        this.SetPromptText();
      }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      this.SetPromptText();
    }

    private void SetPromptText()
    {
      IntPtr textHandle = this.GetTextHandle();
      if (!(textHandle != IntPtr.Zero))
        return;
      ComboBoxEx.NativeMethods.SendMessage(textHandle, 5377, IntPtr.Zero, this.promptText);
    }

    private IntPtr GetTextHandle()
    {
      ComboBoxEx.NativeMethods.COMBOBOXINFO pcbi = new ComboBoxEx.NativeMethods.COMBOBOXINFO();
      pcbi.cbSize = Marshal.SizeOf((object) pcbi);
      ComboBoxEx.NativeMethods.GetComboBoxInfo(this.Handle, ref pcbi);
      return pcbi.hwndEdit;
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("Behavior")]
    [Description("Automatically select the text when control receives the focus.")]
    [DefaultValue(true)]
    public bool FocusSelect
    {
      get => this.focusSelect;
      set => this.focusSelect = value;
    }

    protected override void OnEnter(EventArgs e)
    {
      if (string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(this.PromptText))
        this.Text = this.PromptText;
      if (this.Text.Length > 0 && this.focusSelect)
      {
        this.SelectAll();
        this.quickSelectAll = true;
      }
      base.OnEnter(e);
    }

    protected override void OnLeave(EventArgs e)
    {
      base.OnLeave(e);
      if (!string.IsNullOrEmpty(this.PromptText) && this.SelectedText == this.PromptText)
        this.Text = string.Empty;
      this.quickSelectAll = false;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (!this.quickSelectAll)
        return;
      this.SelectAll();
    }

    protected override void OnKeyDown(KeyEventArgs e) => this.quickSelectAll = false;

    protected override void OnMouseUp(MouseEventArgs mevent) => this.quickSelectAll = false;

    private static class NativeMethods
    {
      public const int ECM_FIRST = 5376;
      public const int EM_SETCUEBANNER = 5377;

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

      [DllImport("user32.dll")]
      public static extern bool GetComboBoxInfo(
        IntPtr hwnd,
        ref ComboBoxEx.NativeMethods.COMBOBOXINFO pcbi);

      public struct RECT
      {
        public int left;
        public int top;
        public int right;
        public int bottom;
      }

      public enum ComboBoxButtonState
      {
        STATE_SYSTEM_NONE = 0,
        STATE_SYSTEM_PRESSED = 8,
        STATE_SYSTEM_INVISIBLE = 32768, // 0x00008000
      }

      public struct COMBOBOXINFO
      {
        public int cbSize;
        public ComboBoxEx.NativeMethods.RECT rcItem;
        public ComboBoxEx.NativeMethods.RECT rcButton;
        public ComboBoxEx.NativeMethods.ComboBoxButtonState buttonState;
        public IntPtr hwndCombo;
        public IntPtr hwndEdit;
        public IntPtr hwndList;
      }
    }
  }
}

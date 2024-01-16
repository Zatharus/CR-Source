// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TreeViewEx
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
  public class TreeViewEx : TreeView
  {
    private Timer scrollTimer;
    private int dragScrollRegion = 10;
    private int delta;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.scrollTimer != null)
        this.scrollTimer.Dispose();
      base.Dispose(disposing);
    }

    public static void EnableDoubleBuffer(TreeView tv)
    {
      TreeViewEx.Native.SetExStyles(tv.Handle, TreeViewEx.Native.TVS_EX.TVS_EX_DOUBLEBUFFER);
    }

    protected override void OnCreateControl()
    {
      base.OnCreateControl();
      TreeViewEx.EnableDoubleBuffer((TreeView) this);
    }

    [DefaultValue(10)]
    public int DragScrollRegion
    {
      get => this.dragScrollRegion;
      set => this.dragScrollRegion = value;
    }

    private void scrollTimer_Tick(object sender, EventArgs e)
    {
      TreeViewEx.Native.ScrollLines((IWin32Window) this, this.delta);
    }

    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      if (this.scrollTimer == null)
      {
        this.scrollTimer = new Timer() { Interval = 150 };
        this.scrollTimer.Tick += new EventHandler(this.scrollTimer_Tick);
      }
      System.Drawing.Point client = this.PointToClient(new System.Drawing.Point(e.X, e.Y));
      if (client.Y < this.dragScrollRegion)
      {
        this.delta = -1;
        this.scrollTimer.Enabled = true;
      }
      else if (client.Y > this.Height - this.dragScrollRegion)
      {
        this.delta = 1;
        this.scrollTimer.Enabled = true;
      }
      else
        this.scrollTimer.Enabled = false;
    }

    protected override void OnDragLeave(EventArgs e)
    {
      base.OnDragLeave(e);
      if (this.scrollTimer == null)
        return;
      this.scrollTimer.Enabled = false;
    }

    protected override void WndProc(ref Message m)
    {
      base.WndProc(ref m);
      if (m.Msg != 277)
        return;
      this.OnScroll(new ScrollEventArgs((ScrollEventType) Enum.Parse(typeof (ScrollEventType), (m.WParam.ToInt32() & (int) ushort.MaxValue).ToString()), (int) (m.WParam.ToInt64() >> 16) & (int) byte.MaxValue));
    }

    public event ScrollEventHandler Scroll;

    protected virtual void OnScroll(ScrollEventArgs sea)
    {
      if (this.Scroll == null)
        return;
      this.Scroll((object) this, sea);
    }

    private static class Native
    {
      public const int WM_SCROLL = 276;
      public const int WM_VSCROLL = 277;
      private const int SB_LINEUP = 0;
      private const int SB_LINELEFT = 0;
      private const int SB_LINEDOWN = 1;
      private const int SB_LINERIGHT = 1;
      private const int SB_PAGEUP = 2;
      private const int SB_PAGELEFT = 2;
      private const int SB_PAGEDOWN = 3;
      private const int SB_PAGERIGTH = 3;
      private const int SB_PAGETOP = 6;
      private const int SB_LEFT = 6;
      private const int SB_PAGEBOTTOM = 7;
      private const int SB_RIGHT = 7;
      private const int SB_ENDSCROLL = 8;

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      private static extern int SendMessage(IntPtr handle, int messg, int wparam, int lparam);

      public static void SetExStyles(IntPtr handle, TreeViewEx.Native.TVS_EX exStyle)
      {
        TreeViewEx.Native.TVS_EX lparam = (TreeViewEx.Native.TVS_EX) TreeViewEx.Native.SendMessage(handle, 4397, 0, 0) | exStyle;
        TreeViewEx.Native.SendMessage(handle, 4396, 0, (int) lparam);
      }

      [DllImport("user32.dll", CharSet = CharSet.Unicode)]
      private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

      public static void ScrollLines(IWin32Window window, int lines)
      {
        int num = Math.Abs(lines);
        for (int index = 0; index < num; ++index)
          TreeViewEx.Native.SendMessage(window.Handle, 277, lines < 0 ? 0 : 1, 0);
      }

      public enum TVS_EX
      {
        TVS_EX_DOUBLEBUFFER = 4,
      }

      public enum LVM
      {
        TVM_FIRST = 4352, // 0x00001100
        TVM_SETEXTENDEDSTYLE = 4396, // 0x0000112C
        TVM_GETEXTENDEDSTYLE = 4397, // 0x0000112D
      }
    }
  }
}

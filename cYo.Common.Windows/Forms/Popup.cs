// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.Popup
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [CLSCompliant(true)]
  [ToolboxItem(false)]
  public class Popup : ToolStripDropDown
  {
    private Popup ownerPopup;
    private Popup childPopup;
    private readonly ToolStripControlHost host;
    private Control content;
    private bool focusOnOpen = true;
    private bool acceptAlt = true;
    private bool _resizable;
    private bool resizable;
    private bool resizableTop;
    private bool resizableLeft;
    private VisualStyleRenderer sizeGripRenderer;

    public Control Content => this.content;

    public Popup.PopupAnimations ShowingAnimation { get; set; }

    public Popup.PopupAnimations HidingAnimation { get; set; }

    public int AnimationDuration { get; set; }

    public bool FocusOnOpen
    {
      get => this.focusOnOpen;
      set => this.focusOnOpen = value;
    }

    public bool AcceptAlt
    {
      get => this.acceptAlt;
      set => this.acceptAlt = value;
    }

    public bool Resizable
    {
      get => this.resizable && this._resizable;
      set => this.resizable = value;
    }

    public new System.Drawing.Size MinimumSize { get; set; }

    public new System.Drawing.Size MaximumSize { get; set; }

    public bool AutoDispose { get; set; }

    public Popup(Control content, bool autoDispose = false)
    {
      Popup popup = this;
      this.content = content != null ? content : throw new ArgumentNullException(nameof (content));
      this.AutoDispose = autoDispose;
      this.host = new ToolStripControlHost(content);
      this.ShowingAnimation = Popup.PopupAnimations.SystemDefault;
      this.HidingAnimation = Popup.PopupAnimations.None;
      this.AnimationDuration = 100;
      this._resizable = true;
      this.AutoSize = false;
      this.DoubleBuffered = true;
      this.ResizeRedraw = true;
      this.Padding = this.Margin = this.host.Padding = this.host.Margin = Padding.Empty;
      this.MinimumSize = content.MinimumSize;
      content.MinimumSize = content.Size;
      this.MaximumSize = content.MaximumSize;
      content.MaximumSize = content.Size;
      this.Size = content.Size;
      content.Location = System.Drawing.Point.Empty;
      this.Items.Add((ToolStripItem) this.host);
      content.Disposed += (EventHandler) ((sender, e) =>
      {
        content = (Control) null;
        popup.Dispose(true);
      });
      content.RegionChanged += (EventHandler) ((sender, e) => this.UpdateRegion());
      content.Paint += (PaintEventHandler) ((sender, e) => this.PaintSizeGrip(e));
      this.UpdateRegion();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.content != null)
      {
        this.content.Dispose();
        this.content = (Control) null;
      }
      base.Dispose(disposing);
    }

    protected override CreateParams CreateParams
    {
      [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)] get
      {
        CreateParams createParams = base.CreateParams;
        createParams.ExStyle |= 134217728;
        return createParams;
      }
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      if (this.Visible && this.ShowingAnimation == Popup.PopupAnimations.None || !this.Visible && this.HidingAnimation == Popup.PopupAnimations.None)
        return;
      Popup.NativeMethods.AnimationFlags animationFlags = this.Visible ? Popup.NativeMethods.AnimationFlags.Roll : Popup.NativeMethods.AnimationFlags.Hide;
      Popup.PopupAnimations popupAnimations = this.Visible ? this.ShowingAnimation : this.HidingAnimation;
      if (popupAnimations == Popup.PopupAnimations.SystemDefault)
        popupAnimations = !SystemInformation.IsMenuAnimationEnabled ? Popup.PopupAnimations.None : (!SystemInformation.IsMenuFadeEnabled ? (Popup.PopupAnimations) (262144 | (this.Visible ? 4 : 8)) : Popup.PopupAnimations.Blend);
      if ((popupAnimations & (Popup.PopupAnimations.Center | Popup.PopupAnimations.Slide | Popup.PopupAnimations.Blend | Popup.PopupAnimations.Roll)) == Popup.PopupAnimations.None)
        return;
      if (this.resizableTop)
      {
        if ((popupAnimations & Popup.PopupAnimations.BottomToTop) != Popup.PopupAnimations.None)
          popupAnimations = popupAnimations & ~Popup.PopupAnimations.BottomToTop | Popup.PopupAnimations.TopToBottom;
        else if ((popupAnimations & Popup.PopupAnimations.TopToBottom) != Popup.PopupAnimations.None)
          popupAnimations = popupAnimations & ~Popup.PopupAnimations.TopToBottom | Popup.PopupAnimations.BottomToTop;
      }
      if (this.resizableLeft)
      {
        if ((popupAnimations & Popup.PopupAnimations.RightToLeft) != Popup.PopupAnimations.None)
          popupAnimations = popupAnimations & ~Popup.PopupAnimations.RightToLeft | Popup.PopupAnimations.LeftToRight;
        else if ((popupAnimations & Popup.PopupAnimations.LeftToRight) != Popup.PopupAnimations.None)
          popupAnimations = popupAnimations & ~Popup.PopupAnimations.LeftToRight | Popup.PopupAnimations.RightToLeft;
      }
      Popup.NativeMethods.AnimateWindow(this.Handle, this.AnimationDuration, animationFlags | (Popup.NativeMethods.AnimationFlags) ((Popup.PopupAnimations) 1048575 & popupAnimations));
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
      if (this.acceptAlt && (keyData & Keys.Alt) == Keys.Alt)
      {
        if ((keyData & Keys.F4) != Keys.F4)
          return false;
        this.Close();
      }
      return base.ProcessDialogKey(keyData);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      if (this.content != null)
      {
        this.content.MinimumSize = this.Size;
        this.content.MaximumSize = this.Size;
        this.content.Size = this.Size;
        this.content.Location = System.Drawing.Point.Empty;
      }
      base.OnSizeChanged(e);
    }

    protected override void OnOpening(CancelEventArgs e)
    {
      if (this.content.IsDisposed || this.content.Disposing)
      {
        e.Cancel = true;
      }
      else
      {
        this.UpdateRegion();
        base.OnOpening(e);
      }
    }

    protected override void OnOpened(EventArgs e)
    {
      if (this.ownerPopup != null)
        this.ownerPopup._resizable = false;
      if (this.focusOnOpen)
        this.content.Focus();
      base.OnOpened(e);
    }

    protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
    {
      if (this.ownerPopup != null)
        this.ownerPopup._resizable = true;
      base.OnClosed(e);
      if (this.content is Popup.INotifyClose)
        ((Popup.INotifyClose) this.content).PopupClosed();
      if (this.PopupClosed != null)
        this.PopupClosed((object) this, EventArgs.Empty);
      ControlExtensions.BeginInvoke(this, new Action(((Component) this).Dispose));
    }

    public event EventHandler PopupClosed;

    protected void UpdateRegion()
    {
      if (this.Region != null)
      {
        this.Region.Dispose();
        this.Region = (Region) null;
      }
      if (this.content.Region == null)
        return;
      this.Region = this.content.Region.Clone();
    }

    protected void SetOwnerItem(Control control)
    {
      if (control == null)
        return;
      if (control is Popup popup)
      {
        this.ownerPopup = popup;
        this.ownerPopup.childPopup = this;
        this.OwnerItem = popup.Items[0];
      }
      else
      {
        if (control.Parent == null)
          return;
        this.SetOwnerItem(control.Parent);
      }
    }

    public void Show(Control control)
    {
      if (control == null)
        throw new ArgumentNullException(nameof (control));
      this.SetOwnerItem(control);
      this.Show(control, control.ClientRectangle);
    }

    public void Show(Control control, Rectangle area)
    {
      if (control == null)
        throw new ArgumentNullException(nameof (control));
      this.SetOwnerItem(control);
      this.resizableTop = this.resizableLeft = false;
      System.Drawing.Point point = control.PointToScreen(new System.Drawing.Point(area.Left, area.Top + area.Height));
      Rectangle workingArea = Screen.FromControl(control).WorkingArea;
      int x = point.X;
      System.Drawing.Size size = this.Size;
      int width1 = size.Width;
      if (x + width1 > workingArea.Left + workingArea.Width)
      {
        this.resizableLeft = true;
        ref System.Drawing.Point local = ref point;
        int num1 = workingArea.Left + workingArea.Width;
        size = this.Size;
        int width2 = size.Width;
        int num2 = num1 - width2;
        local.X = num2;
      }
      int y1 = point.Y;
      size = this.Size;
      int height = size.Height;
      if (y1 + height > workingArea.Top + workingArea.Height)
      {
        this.resizableTop = true;
        ref System.Drawing.Point local = ref point;
        int y2 = local.Y;
        size = this.Size;
        int num = size.Height + area.Height;
        local.Y = y2 - num;
      }
      point = control.PointToClient(point);
      this.Show(control, point, ToolStripDropDownDirection.BelowRight);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override void WndProc(ref Message m)
    {
      if (this.InternalProcessResizing(ref m, false))
        return;
      base.WndProc(ref m);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public bool ProcessResizing(ref Message m) => this.InternalProcessResizing(ref m, true);

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    private bool InternalProcessResizing(ref Message m, bool contentControl)
    {
      if (m.Msg == 134 && m.WParam != IntPtr.Zero && this.childPopup != null && this.childPopup.Visible)
        this.childPopup.Hide();
      if (!this.Resizable)
        return false;
      if (m.Msg == 132)
        return this.OnNcHitTest(ref m, contentControl);
      return m.Msg == 36 && this.OnGetMinMaxInfo(ref m);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    private bool OnGetMinMaxInfo(ref Message m)
    {
      Marshal.StructureToPtr((object) ((Popup.NativeMethods.MINMAXINFO) Marshal.PtrToStructure(m.LParam, typeof (Popup.NativeMethods.MINMAXINFO)) with
      {
        maxTrackSize = this.MaximumSize,
        minTrackSize = this.MinimumSize
      }), m.LParam, false);
      return true;
    }

    private bool OnNcHitTest(ref Message m, bool contentControl)
    {
      System.Drawing.Point client = this.PointToClient(new System.Drawing.Point(Popup.NativeMethods.LOWORD(m.LParam), Popup.NativeMethods.HIWORD(m.LParam)));
      Popup.GripBounds gripBounds = new Popup.GripBounds(contentControl ? this.content.ClientRectangle : this.ClientRectangle);
      IntPtr num = new IntPtr(-1);
      Rectangle rectangle;
      if (this.resizableTop)
      {
        if (this.resizableLeft)
        {
          rectangle = gripBounds.TopLeft;
          if (rectangle.Contains(client))
          {
            m.Result = contentControl ? num : (IntPtr) 13;
            return true;
          }
        }
        if (!this.resizableLeft)
        {
          rectangle = gripBounds.TopRight;
          if (rectangle.Contains(client))
          {
            m.Result = contentControl ? num : (IntPtr) 14;
            return true;
          }
        }
        rectangle = gripBounds.Top;
        if (rectangle.Contains(client))
        {
          m.Result = contentControl ? num : (IntPtr) 12;
          return true;
        }
      }
      else
      {
        if (this.resizableLeft)
        {
          rectangle = gripBounds.BottomLeft;
          if (rectangle.Contains(client))
          {
            m.Result = contentControl ? num : (IntPtr) 16;
            return true;
          }
        }
        if (!this.resizableLeft)
        {
          rectangle = gripBounds.BottomRight;
          if (rectangle.Contains(client))
          {
            m.Result = contentControl ? num : (IntPtr) 17;
            return true;
          }
        }
        rectangle = gripBounds.Bottom;
        if (rectangle.Contains(client))
        {
          m.Result = contentControl ? num : (IntPtr) 15;
          return true;
        }
      }
      if (this.resizableLeft)
      {
        rectangle = gripBounds.Left;
        if (rectangle.Contains(client))
        {
          m.Result = contentControl ? num : (IntPtr) 10;
          return true;
        }
      }
      if (!this.resizableLeft)
      {
        rectangle = gripBounds.Right;
        if (rectangle.Contains(client))
        {
          m.Result = contentControl ? num : (IntPtr) 11;
          return true;
        }
      }
      return false;
    }

    public void PaintSizeGrip(PaintEventArgs e)
    {
      if (e == null || e.Graphics == null || !this.resizable)
        return;
      System.Drawing.Size clientSize = this.content.ClientSize;
      using (Bitmap bitmap = new Bitmap(16, 16))
      {
        using (Graphics graphics = Graphics.FromImage((Image) bitmap))
        {
          if (Application.RenderWithVisualStyles)
          {
            if (this.sizeGripRenderer == null)
              this.sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
            this.sizeGripRenderer.DrawBackground((IDeviceContext) graphics, new Rectangle(0, 0, 16, 16));
          }
          else
            ControlPaint.DrawSizeGrip(graphics, this.content.BackColor, 0, 0, 16, 16);
        }
        using (e.Graphics.SaveState())
        {
          if (this.resizableTop)
          {
            if (this.resizableLeft)
            {
              e.Graphics.RotateTransform(180f);
              e.Graphics.TranslateTransform((float) -clientSize.Width, (float) -clientSize.Height);
            }
            else
            {
              e.Graphics.ScaleTransform(1f, -1f);
              e.Graphics.TranslateTransform(0.0f, (float) -clientSize.Height);
            }
          }
          else if (this.resizableLeft)
          {
            e.Graphics.ScaleTransform(-1f, 1f);
            e.Graphics.TranslateTransform((float) -clientSize.Width, 0.0f);
          }
          e.Graphics.DrawImage((Image) bitmap, clientSize.Width - 16, clientSize.Height - 16 + 1, 16, 16);
        }
      }
    }

    [Flags]
    public enum PopupAnimations
    {
      None = 0,
      LeftToRight = 1,
      RightToLeft = 2,
      TopToBottom = 4,
      BottomToTop = 8,
      Center = 16, // 0x00000010
      Slide = 262144, // 0x00040000
      Blend = 524288, // 0x00080000
      Roll = 1048576, // 0x00100000
      SystemDefault = 2097152, // 0x00200000
    }

    public interface INotifyClose
    {
      void PopupClosed();
    }

    private struct GripBounds
    {
      private const int GripSize = 6;
      private const int CornerGripSize = 12;
      private readonly Rectangle clientRectangle;

      public GripBounds(Rectangle clientRectangle) => this.clientRectangle = clientRectangle;

      public Rectangle ClientRectangle => this.clientRectangle;

      public Rectangle Bottom
      {
        get
        {
          Rectangle clientRectangle = this.ClientRectangle;
          clientRectangle.Y = clientRectangle.Bottom - 6 + 1;
          clientRectangle.Height = 6;
          return clientRectangle;
        }
      }

      public Rectangle BottomRight
      {
        get
        {
          Rectangle clientRectangle = this.ClientRectangle;
          clientRectangle.Y = clientRectangle.Bottom - 12 + 1;
          clientRectangle.Height = 12;
          clientRectangle.X = clientRectangle.Width - 12 + 1;
          clientRectangle.Width = 12;
          return clientRectangle;
        }
      }

      public Rectangle Top
      {
        get => this.ClientRectangle with { Height = 6 };
      }

      public Rectangle TopRight
      {
        get
        {
          Rectangle clientRectangle = this.ClientRectangle with
          {
            Height = 12
          };
          clientRectangle.X = clientRectangle.Width - 12 + 1;
          clientRectangle.Width = 12;
          return clientRectangle;
        }
      }

      public Rectangle Left
      {
        get => this.ClientRectangle with { Width = 6 };
      }

      public Rectangle BottomLeft
      {
        get
        {
          Rectangle clientRectangle = this.ClientRectangle with
          {
            Width = 12
          };
          clientRectangle.Y = clientRectangle.Height - 12 + 1;
          clientRectangle.Height = 12;
          return clientRectangle;
        }
      }

      public Rectangle Right
      {
        get
        {
          Rectangle clientRectangle = this.ClientRectangle;
          clientRectangle.X = clientRectangle.Right - 6 + 1;
          clientRectangle.Width = 6;
          return clientRectangle;
        }
      }

      public Rectangle TopLeft
      {
        get
        {
          return this.ClientRectangle with
          {
            Width = 12,
            Height = 12
          };
        }
      }
    }

    [SuppressUnmanagedCodeSecurity]
    private static class NativeMethods
    {
      public const int WM_NCHITTEST = 132;
      public const int WM_NCACTIVATE = 134;
      public const int WS_EX_NOACTIVATE = 134217728;
      public const int HTTRANSPARENT = -1;
      public const int HTLEFT = 10;
      public const int HTRIGHT = 11;
      public const int HTTOP = 12;
      public const int HTTOPLEFT = 13;
      public const int HTTOPRIGHT = 14;
      public const int HTBOTTOM = 15;
      public const int HTBOTTOMLEFT = 16;
      public const int HTBOTTOMRIGHT = 17;
      public const int WM_PRINT = 791;
      public const int WM_USER = 1024;
      public const int WM_REFLECT = 8192;
      public const int WM_COMMAND = 273;
      public const int CBN_DROPDOWN = 7;
      public const int WM_GETMINMAXINFO = 36;

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      public static extern int AnimateWindow(
        IntPtr windowHandle,
        int time,
        Popup.NativeMethods.AnimationFlags flags);

      public static int HIWORD(int n) => (int) (short) (n >> 16 & (int) ushort.MaxValue);

      public static int HIWORD(IntPtr n) => Popup.NativeMethods.HIWORD((int) (long) n);

      public static int LOWORD(int n) => (int) (short) (n & (int) ushort.MaxValue);

      public static int LOWORD(IntPtr n) => Popup.NativeMethods.LOWORD((int) (long) n);

      [Flags]
      public enum AnimationFlags
      {
        Roll = 0,
        HorizontalPositive = 1,
        HorizontalNegative = 2,
        VerticalPositive = 4,
        VerticalNegative = 8,
        Center = 16, // 0x00000010
        Hide = 65536, // 0x00010000
        Activate = 131072, // 0x00020000
        Slide = 262144, // 0x00040000
        Blend = 524288, // 0x00080000
        Mask = 1048575, // 0x000FFFFF
      }

      public struct MINMAXINFO
      {
        public System.Drawing.Point reserved;
        public System.Drawing.Size maxSize;
        public System.Drawing.Point maxPosition;
        public System.Drawing.Size minTrackSize;
        public System.Drawing.Size maxTrackSize;
      }
    }
  }
}

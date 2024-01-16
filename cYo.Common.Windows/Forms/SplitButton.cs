// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.SplitButton
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class SplitButton : System.Windows.Forms.Button
  {
    private const int PushButtonWidth = 14;
    private static readonly int BorderSize = SystemInformation.Border3DSize.Width * 2;
    private bool skipNextOpen;
    private Rectangle dropDownRectangle;
    private bool showSplit = true;
    private PushButtonState state;
    private ContextMenuStrip oldContextMenu;

    [DefaultValue(true)]
    public bool ShowSplit
    {
      set
      {
        if (value == this.showSplit)
          return;
        this.showSplit = value;
        this.Invalidate();
        this.Parent?.PerformLayout();
      }
    }

    private PushButtonState State
    {
      get => this.state;
      set
      {
        if (this.state == value)
          return;
        this.state = value;
        this.Invalidate();
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.RemoveContextEvents();
      base.Dispose(disposing);
    }

    public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
    {
      if (!this.AutoSize)
        return this.Size;
      System.Drawing.Size preferredSize = base.GetPreferredSize(proposedSize);
      return this.showSplit && !string.IsNullOrEmpty(this.Text) && TextRenderer.MeasureText(this.Text, this.Font).Width + FormUtility.ScaleDpiX(14) > preferredSize.Width ? preferredSize + new System.Drawing.Size(FormUtility.ScaleDpiX(14) + SplitButton.BorderSize * 2, 0) : preferredSize;
    }

    protected override bool IsInputKey(Keys keyData)
    {
      return keyData == Keys.Down && this.showSplit || base.IsInputKey(keyData);
    }

    protected override void OnGotFocus(EventArgs e)
    {
      if (!this.showSplit)
      {
        base.OnGotFocus(e);
      }
      else
      {
        if (this.State == PushButtonState.Pressed || this.State == PushButtonState.Disabled)
          return;
        this.State = PushButtonState.Default;
      }
    }

    protected override void OnKeyDown(KeyEventArgs kevent)
    {
      if (this.showSplit)
      {
        if (kevent.KeyCode == Keys.Down)
          this.ShowContextMenuStrip();
        else if (kevent.KeyCode == Keys.Space && kevent.Modifiers == Keys.None)
          this.State = PushButtonState.Pressed;
      }
      base.OnKeyDown(kevent);
    }

    protected override void OnKeyUp(KeyEventArgs kevent)
    {
      if (kevent.KeyCode == Keys.Space && Control.MouseButtons == MouseButtons.None)
        this.State = PushButtonState.Normal;
      base.OnKeyUp(kevent);
    }

    protected override void OnLostFocus(EventArgs e)
    {
      if (!this.showSplit)
      {
        base.OnLostFocus(e);
      }
      else
      {
        if (this.State == PushButtonState.Pressed || this.State == PushButtonState.Disabled)
          return;
        this.State = PushButtonState.Normal;
      }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      if (!this.showSplit)
        base.OnMouseDown(e);
      else if (this.dropDownRectangle.Contains(e.Location))
        this.ShowContextMenuStrip();
      else
        this.State = PushButtonState.Pressed;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
      if (!this.showSplit)
      {
        base.OnMouseEnter(e);
      }
      else
      {
        if (this.State == PushButtonState.Pressed || this.State == PushButtonState.Disabled)
          return;
        this.State = PushButtonState.Hot;
      }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      if (!this.showSplit)
      {
        base.OnMouseLeave(e);
      }
      else
      {
        if (this.State == PushButtonState.Pressed || this.State == PushButtonState.Disabled)
          return;
        this.State = this.Focused ? PushButtonState.Default : PushButtonState.Normal;
      }
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
      if (!this.showSplit)
      {
        base.OnMouseUp(mevent);
      }
      else
      {
        if (this.ContextMenuStrip != null && this.ContextMenuStrip.Visible)
          return;
        this.SetButtonDrawState();
        if (!this.Bounds.Contains(this.Parent.PointToClient(Cursor.Position)) || this.dropDownRectangle.Contains(mevent.Location))
          return;
        this.OnClick(EventArgs.Empty);
      }
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
      base.OnPaint(pevent);
      if (!this.showSplit)
        return;
      Graphics graphics = pevent.Graphics;
      Rectangle clientRectangle = this.ClientRectangle;
      if (this.State != PushButtonState.Pressed && this.IsDefault && !Application.RenderWithVisualStyles)
      {
        Rectangle bounds = clientRectangle;
        bounds.Inflate(-1, -1);
        ButtonRenderer.DrawButton(graphics, bounds, this.State);
        graphics.DrawRectangle(SystemPens.WindowFrame, 0, 0, clientRectangle.Width - 1, clientRectangle.Height - 1);
      }
      else
        ButtonRenderer.DrawButton(graphics, clientRectangle, this.State);
      this.dropDownRectangle = new Rectangle(clientRectangle.Right - FormUtility.ScaleDpiX(14) - 1, SplitButton.BorderSize, FormUtility.ScaleDpiX(14), clientRectangle.Height - SplitButton.BorderSize * 2);
      int borderSize = SplitButton.BorderSize;
      Rectangle rectangle = new Rectangle(borderSize, borderSize, clientRectangle.Width - this.dropDownRectangle.Width - borderSize, clientRectangle.Height - borderSize * 2);
      bool flag = this.State == PushButtonState.Hot || this.State == PushButtonState.Pressed || !Application.RenderWithVisualStyles;
      if (this.RightToLeft == RightToLeft.Yes)
      {
        this.dropDownRectangle.X = clientRectangle.Left + 1;
        rectangle.X = this.dropDownRectangle.Right;
        if (flag)
        {
          graphics.DrawLine(SystemPens.ButtonShadow, clientRectangle.Left + FormUtility.ScaleDpiX(14), SplitButton.BorderSize, clientRectangle.Left + FormUtility.ScaleDpiX(14), clientRectangle.Bottom - SplitButton.BorderSize);
          graphics.DrawLine(SystemPens.ButtonFace, clientRectangle.Left + FormUtility.ScaleDpiX(14) + 1, SplitButton.BorderSize, clientRectangle.Left + FormUtility.ScaleDpiX(14) + 1, clientRectangle.Bottom - SplitButton.BorderSize);
        }
      }
      else if (flag)
      {
        graphics.DrawLine(SystemPens.ButtonShadow, clientRectangle.Right - FormUtility.ScaleDpiX(14), SplitButton.BorderSize, clientRectangle.Right - FormUtility.ScaleDpiX(14), clientRectangle.Bottom - SplitButton.BorderSize);
        graphics.DrawLine(SystemPens.ButtonFace, clientRectangle.Right - FormUtility.ScaleDpiX(14) - 1, SplitButton.BorderSize, clientRectangle.Right - FormUtility.ScaleDpiX(14) - 1, clientRectangle.Bottom - SplitButton.BorderSize);
      }
      SplitButton.PaintArrow(graphics, this.dropDownRectangle);
      if (!string.IsNullOrEmpty(this.Text))
      {
        TextFormatFlags textFormatFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;
        if (!this.UseMnemonic)
          textFormatFlags |= TextFormatFlags.NoPrefix;
        else if (!this.ShowKeyboardCues)
          textFormatFlags |= TextFormatFlags.HidePrefix;
        if (this.AutoEllipsis)
          textFormatFlags |= TextFormatFlags.EndEllipsis;
        if (Application.RenderWithVisualStyles)
        {
          using (FontDC dc = new FontDC(graphics, this.Font))
            new VisualStyleRenderer(this.Enabled ? VisualStyleElement.Button.PushButton.Default : VisualStyleElement.Button.PushButton.Disabled).DrawText((IDeviceContext) dc, rectangle, this.Text, true, textFormatFlags);
        }
        else if (this.Enabled)
          TextRenderer.DrawText((IDeviceContext) graphics, this.Text, this.Font, rectangle, this.ForeColor, textFormatFlags);
        else
          ControlPaint.DrawStringDisabled((IDeviceContext) graphics, this.Text, this.Font, this.ForeColor, rectangle, textFormatFlags);
      }
      if (this.State == PushButtonState.Pressed || !this.Focused)
        return;
      ControlPaint.DrawFocusRectangle(graphics, rectangle);
    }

    protected override void OnContextMenuStripChanged(EventArgs e)
    {
      base.OnContextMenuStripChanged(e);
      this.RemoveContextEvents();
      this.oldContextMenu = this.ContextMenuStrip;
      if (this.oldContextMenu == null)
        return;
      this.ContextMenuStrip.Opening += new CancelEventHandler(this.ContextMenuStrip_Opening);
      this.ContextMenuStrip.Closing += new ToolStripDropDownClosingEventHandler(this.ContextMenuStrip_Closing);
    }

    public event EventHandler ShowContextMenu;

    protected virtual void OnShowContextMenu()
    {
      if (this.ShowContextMenu == null)
        return;
      this.ShowContextMenu((object) this, EventArgs.Empty);
    }

    private void RemoveContextEvents()
    {
      if (this.oldContextMenu == null)
        return;
      this.oldContextMenu.Opening -= new CancelEventHandler(this.ContextMenuStrip_Opening);
      this.oldContextMenu.Closing -= new ToolStripDropDownClosingEventHandler(this.ContextMenuStrip_Closing);
    }

    private static void PaintArrow(Graphics g, Rectangle dropDownRect)
    {
      System.Drawing.Point point = new System.Drawing.Point(Convert.ToInt32(dropDownRect.Left + dropDownRect.Width / 2), Convert.ToInt32(dropDownRect.Top + dropDownRect.Height / 2));
      point.X += dropDownRect.Width % 2;
      int num1 = FormUtility.ScaleDpiX(2);
      int num2 = FormUtility.ScaleDpiX(3);
      int num3 = FormUtility.ScaleDpiY(1);
      int num4 = FormUtility.ScaleDpiY(2);
      System.Drawing.Point[] points = new System.Drawing.Point[3]
      {
        new System.Drawing.Point(point.X - num1, point.Y - num3),
        new System.Drawing.Point(point.X + num2, point.Y - num3),
        new System.Drawing.Point(point.X, point.Y + num4)
      };
      g.FillPolygon(SystemBrushes.ControlText, points);
    }

    private void ShowContextMenuStrip()
    {
      if (this.skipNextOpen)
      {
        this.skipNextOpen = false;
      }
      else
      {
        this.State = PushButtonState.Pressed;
        this.ContextMenuStrip?.Show((Control) this, new System.Drawing.Point(0, this.Height), ToolStripDropDownDirection.BelowRight);
      }
    }

    private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
    {
      this.OnShowContextMenu();
    }

    private void ContextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
    {
      this.SetButtonDrawState();
      if (e.CloseReason != ToolStripDropDownCloseReason.AppClicked)
        return;
      this.skipNextOpen = this.dropDownRectangle.Contains(this.PointToClient(Cursor.Position));
    }

    private void SetButtonDrawState()
    {
      if (this.Bounds.Contains(this.Parent.PointToClient(Cursor.Position)))
        this.State = PushButtonState.Hot;
      else
        this.State = this.Focused ? PushButtonState.Default : PushButtonState.Normal;
    }
  }
}

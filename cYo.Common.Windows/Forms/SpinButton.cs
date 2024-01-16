// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.SpinButton
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Mathematics;
using cYo.Common.Text;
using cYo.Common.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class SpinButton : Control
  {
    private Timer repeatTimer = new Timer();
    private bool upEnabled = true;
    private bool downEnabled = true;
    private bool flat = true;
    private SpinButton.SpinButtonType hit;
    private SpinButton.SpinButtonType hot;

    public SpinButton()
    {
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.Selectable, true);
      this.repeatTimer.Tick += new EventHandler(this.repeatTimer_Tick);
      this.RepeatInterval = 250;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.repeatTimer.Dispose();
      base.Dispose(disposing);
    }

    [DefaultValue(250)]
    public int RepeatInterval { get; set; }

    [DefaultValue(true)]
    public bool UpEnabled
    {
      get => this.upEnabled;
      set
      {
        if (value == this.upEnabled)
          return;
        this.upEnabled = value;
        this.Invalidate();
      }
    }

    [DefaultValue(true)]
    public bool DownEnabled
    {
      get => this.downEnabled;
      set
      {
        if (value == this.downEnabled)
          return;
        this.downEnabled = value;
        this.Invalidate();
      }
    }

    [DefaultValue(true)]
    public bool Flat
    {
      get => this.flat;
      set
      {
        if (value == this.flat)
          return;
        this.flat = value;
        this.Invalidate();
      }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.hit != SpinButton.SpinButtonType.None)
        return;
      SpinButton.SpinButtonType spinButtonType = this.HitTest(e.Location);
      if (spinButtonType == this.hot)
        return;
      this.hot = spinButtonType;
      this.Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      if (this.hot == SpinButton.SpinButtonType.None)
        return;
      this.hot = SpinButton.SpinButtonType.None;
      this.Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      this.Focus();
      this.hit = this.HitTest(e.Location);
      if (this.hit == SpinButton.SpinButtonType.None)
        return;
      this.Invalidate();
      this.OnButtonPressed(this.hit);
      if (this.RepeatInterval == 0)
        return;
      this.repeatTimer.Interval = this.RepeatInterval;
      this.repeatTimer.Start();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      if (this.hit == SpinButton.SpinButtonType.None)
        return;
      this.hit = SpinButton.SpinButtonType.None;
      this.repeatTimer.Stop();
      this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      SpinButton.Draw(e.Graphics, this.ClientRectangle, hit: this.hit, hot: this.hot, flat: this.flat, upEnabled: this.Enabled && this.upEnabled, downEnabled: this.Enabled && this.downEnabled);
    }

    protected override bool IsInputKey(Keys keyData)
    {
      return keyData == Keys.Up || keyData == Keys.Down || base.IsInputKey(keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      e.Handled = this.HandleKey(e.KeyCode);
      base.OnKeyDown(e);
    }

    public event EventHandler ButtonUp;

    public event EventHandler ButtonDown;

    protected virtual void OnButtonPressed(SpinButton.SpinButtonType button)
    {
      switch (button)
      {
        case SpinButton.SpinButtonType.Up:
          if (!this.upEnabled)
            break;
          this.OnButtonUp();
          break;
        case SpinButton.SpinButtonType.Down:
          if (!this.downEnabled)
            break;
          this.OnButtonDown();
          break;
      }
    }

    protected virtual void OnButtonUp()
    {
      if (this.ButtonUp == null)
        return;
      this.ButtonUp((object) this, EventArgs.Empty);
    }

    protected virtual void OnButtonDown()
    {
      if (this.ButtonDown == null)
        return;
      this.ButtonDown((object) this, EventArgs.Empty);
    }

    private void repeatTimer_Tick(object sender, EventArgs e) => this.OnButtonPressed(this.hit);

    private bool IsUpKey(Keys keyCode)
    {
      if (keyCode <= Keys.Up)
      {
        if (keyCode != Keys.Prior && keyCode != Keys.Up)
          goto label_4;
      }
      else if (keyCode != Keys.Add && keyCode != Keys.Oemplus)
        goto label_4;
      return true;
label_4:
      return false;
    }

    private bool IsDownKey(Keys keyCode)
    {
      if (keyCode <= Keys.Down)
      {
        if (keyCode != Keys.Next && keyCode != Keys.Down)
          goto label_4;
      }
      else if (keyCode != Keys.Subtract && keyCode != Keys.OemMinus)
        goto label_4;
      return true;
label_4:
      return false;
    }

    private bool HandleKey(Keys keyCode)
    {
      if (this.IsUpKey(keyCode))
      {
        this.OnButtonPressed(SpinButton.SpinButtonType.Up);
        return true;
      }
      if (!this.IsDownKey(keyCode))
        return false;
      this.OnButtonPressed(SpinButton.SpinButtonType.Down);
      return true;
    }

    private SpinButton.SpinButtonType HitTest(System.Drawing.Point location)
    {
      return SpinButton.HitTest(this.ClientRectangle, location, this.upEnabled, this.downEnabled);
    }

    private Rectangle GetButtonBounds(SpinButton.SpinButtonType button)
    {
      return SpinButton.GetButtonBounds(this.ClientRectangle, button);
    }

    private static Rectangle GetButtonBounds(Rectangle rc, SpinButton.SpinButtonType button)
    {
      rc.Height /= 2;
      if (button == SpinButton.SpinButtonType.Down)
        rc.Y += rc.Height;
      return rc;
    }

    public static SpinButton.SpinButtonType HitTest(
      Rectangle rc,
      System.Drawing.Point location,
      bool upEnabled = true,
      bool downEnabled = true)
    {
      if (SpinButton.GetButtonBounds(rc, SpinButton.SpinButtonType.Up).Contains(location) & upEnabled)
        return SpinButton.SpinButtonType.Up;
      return SpinButton.GetButtonBounds(rc, SpinButton.SpinButtonType.Down).Contains(location) & downEnabled ? SpinButton.SpinButtonType.Down : SpinButton.SpinButtonType.None;
    }

    public static void Draw(
      Graphics gr,
      Rectangle rc,
      bool styleMode = true,
      SpinButton.SpinButtonType hit = SpinButton.SpinButtonType.None,
      SpinButton.SpinButtonType hot = SpinButton.SpinButtonType.None,
      bool flat = true,
      bool upEnabled = true,
      bool downEnabled = true)
    {
      Rectangle buttonBounds1 = SpinButton.GetButtonBounds(rc, SpinButton.SpinButtonType.Up);
      Rectangle buttonBounds2 = SpinButton.GetButtonBounds(rc, SpinButton.SpinButtonType.Down);
      if (styleMode && Application.RenderWithVisualStyles)
      {
        new VisualStyleRenderer(upEnabled ? (hit == SpinButton.SpinButtonType.Up ? VisualStyleElement.Spin.Up.Pressed : (hot == SpinButton.SpinButtonType.Up ? VisualStyleElement.Spin.Up.Hot : VisualStyleElement.Spin.Up.Normal)) : VisualStyleElement.Spin.Up.Disabled).DrawBackground((IDeviceContext) gr, buttonBounds1);
        new VisualStyleRenderer(downEnabled ? (hit == SpinButton.SpinButtonType.Down ? VisualStyleElement.Spin.Down.Pressed : (hot == SpinButton.SpinButtonType.Down ? VisualStyleElement.Spin.Down.Hot : VisualStyleElement.Spin.Down.Normal)) : VisualStyleElement.Spin.Down.Disabled).DrawBackground((IDeviceContext) gr, buttonBounds2);
      }
      else
      {
        ControlPaint.DrawScrollButton(gr, buttonBounds1, ScrollButton.Up, (ButtonState) ((flat ? 16384 : 0) | (upEnabled ? (hit == SpinButton.SpinButtonType.Up ? 512 : 0) : 256)));
        ControlPaint.DrawScrollButton(gr, buttonBounds2, ScrollButton.Down, (ButtonState) ((flat ? 16384 : 0) | (downEnabled ? (hit == SpinButton.SpinButtonType.Down ? 512 : 0) : 256)));
      }
    }

    public static void AddUpDown(
      TextBoxBase textBox,
      int start = 1,
      int min = -2147483648,
      int max = 2147483647,
      int increment = 1,
      bool registerKeys = false,
      bool hidden = false,
      bool visuallyLinkToParent = false)
    {
      SpinButton spinButton = new SpinButton();
      spinButton.Width = FormUtility.ScaleDpiX(11);
      spinButton.Enabled = textBox.Enabled;
      spinButton.Visible = !hidden && textBox.IsVisibleSet();
      SpinButton sb = spinButton;
      Action position = (Action) (() =>
      {
        sb.Top = textBox.Top;
        sb.Height = textBox.Height;
        sb.Left = textBox.Right + 1;
      });
      textBox.Width -= sb.Width;
      textBox.Parent.Controls.Add((Control) sb);
      textBox.Parent.Controls.SetChildIndex((Control) sb, textBox.Parent.Controls.IndexOf((Control) textBox) + 1);
      if (visuallyLinkToParent)
      {
        textBox.SizeChanged += (EventHandler) ((s, e) =>
        {
          if (textBox.Parent != sb.Parent)
            return;
          position();
        });
        textBox.LocationChanged += (EventHandler) ((s, e) =>
        {
          if (textBox.Parent != sb.Parent)
            return;
          position();
        });
        textBox.VisibleChanged += (EventHandler) ((s, e) =>
        {
          if (textBox.Parent != sb.Parent)
            return;
          sb.Visible = textBox.IsVisibleSet();
        });
        textBox.EnabledChanged += (EventHandler) ((s, e) =>
        {
          if (textBox.Parent != sb.Parent)
            return;
          sb.Enabled = textBox.Enabled;
        });
      }
      sb.ButtonUp += (EventHandler) ((s, e) => SpinButton.SetTextBoxValue(textBox, start, min, max, increment));
      sb.ButtonDown += (EventHandler) ((s, e) => SpinButton.SetTextBoxValue(textBox, start, min, max, -increment));
      position();
      if (!registerKeys)
        return;
      textBox.PreviewKeyDown += (PreviewKeyDownEventHandler) ((s, e) =>
      {
        PreviewKeyDownEventArgs keyDownEventArgs = e;
        keyDownEventArgs.IsInputKey = ((keyDownEventArgs.IsInputKey ? 1 : 0) | (sb.IsUpKey(e.KeyCode) ? 1 : (sb.IsDownKey(e.KeyCode) ? 1 : 0))) != 0;
      });
      textBox.KeyDown += (KeyEventHandler) ((s, e) => e.Handled = sb.HandleKey(e.KeyCode));
    }

    private static void SetTextBoxValue(
      TextBoxBase textBox,
      int start,
      int min,
      int max,
      int increment)
    {
      int n;
      bool flag = textBox.Text.TryParse(out n, true);
      if (!flag && textBox is IPromptText)
        flag = ((IPromptText) textBox).PromptText.TryParse(out n, true);
      int num = !flag ? start : (n + increment).Clamp(min, max);
      textBox.Text = num.ToString();
      textBox.SelectAll();
    }

    public enum SpinButtonType
    {
      None,
      Up,
      Down,
    }
  }
}

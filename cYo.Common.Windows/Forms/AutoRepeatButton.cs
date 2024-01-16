// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.AutoRepeatButton
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class AutoRepeatButton : Button
  {
    private bool supressFinalClick;
    private bool finalClick;
    private int currentTime;
    private Timer timer;
    private int repeatTime = 250;
    private int speedUp = 10;
    private bool repeatEnabled = true;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.timer != null)
        this.timer.Dispose();
      base.Dispose(disposing);
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
      base.OnMouseDown(mevent);
      if ((mevent.Button & MouseButtons.Left) == MouseButtons.None)
        return;
      this.currentTime = this.repeatTime;
      this.InitTimer(this.currentTime);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
      base.OnMouseUp(mevent);
      this.InitTimer(0);
      if (!this.supressFinalClick)
        return;
      this.finalClick = false;
    }

    protected override void OnClick(EventArgs e)
    {
      if (this.finalClick)
        base.OnClick(e);
      this.finalClick = true;
    }

    [DefaultValue(250)]
    public int RepeatTime
    {
      get => this.repeatTime;
      set => this.repeatTime = value;
    }

    [DefaultValue(10)]
    public int SpeedUp
    {
      get => this.speedUp;
      set => this.speedUp = value;
    }

    [DefaultValue(true)]
    public bool RepeatEnabled
    {
      get => this.repeatEnabled;
      set => this.repeatEnabled = value;
    }

    private void InitTimer(int repeatTime)
    {
      if (this.timer == null)
      {
        this.timer = new Timer();
        this.timer.Tick += new EventHandler(this.timer_Tick);
      }
      this.finalClick = true;
      this.supressFinalClick = false;
      this.timer.Stop();
      if (!this.repeatEnabled || repeatTime == 0)
        return;
      this.timer.Interval = repeatTime;
      this.timer.Start();
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      this.OnClick(e);
      this.currentTime -= this.speedUp;
      if (this.currentTime < 50)
        this.currentTime = 50;
      this.InitTimer(this.currentTime);
      this.supressFinalClick = true;
    }
  }
}

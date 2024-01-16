// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.Panels.BatteryStatus
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using cYo.Common.Drawing;
using cYo.Common.Presentation.Properties;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation.Panels
{
  public class BatteryStatus : OverlayPanel
  {
    private int percent = -1;
    private bool plug;

    public BatteryStatus()
      : base(20, 8)
    {
    }

    protected override void OnDrawing()
    {
      base.OnDrawing();
      int num = (int) ((double) SystemInformation.PowerStatus.BatteryLifePercent * 100.0);
      bool flag = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
      if (num == this.percent && flag == this.plug)
        return;
      this.percent = num;
      this.plug = flag;
      using (PanelSurface surface = this.CreateSurface(true))
      {
        Graphics graphics = surface.Graphics;
        if (this.plug)
        {
          using (Bitmap plug = Resources.Plug)
            graphics.DrawImage((Image) plug, plug.Size.Align(this.ClientRectangle, ContentAlignment.MiddleCenter));
        }
        else
        {
          Rectangle rectangle = this.ClientRectangle.Pad(0, 0, 1, 1);
          Color color = (double) this.percent < 0.15 ? Color.Red : Color.Green;
          graphics.DrawRectangle(Pens.White, rectangle);
          Rectangle rect = rectangle.Pad(1, 1);
          rect.Width = rect.Width * this.percent / 100;
          using (Brush brush = (Brush) new SolidBrush(color))
            graphics.FillRectangle(brush, rect);
        }
      }
    }
  }
}

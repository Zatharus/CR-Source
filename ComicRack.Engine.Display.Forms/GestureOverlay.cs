// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Display.Forms.GestureOverlay
// Assembly: ComicRack.Engine.Display.Forms, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: D83BAE4E-CA55-445A-AD1D-2DF78C341143
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.Display.Forms.dll

using cYo.Common.Presentation.Panels;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.Display.Forms.Properties;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Display.Forms
{
  public class GestureOverlay : OverlayPanel
  {
    private readonly Bitmap bitmap = Resources.TouchPad;

    public GestureOverlay()
      : base(EngineConfiguration.Default.GestureAreaSize, EngineConfiguration.Default.GestureAreaSize)
    {
      this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      using (ItemMonitor.Lock((object) this.bitmap))
        e.Graphics.DrawImage((Image) this.bitmap, this.ClientRectangle);
    }
  }
}

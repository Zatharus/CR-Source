// Decompiled with JetBrains decompiler
// Type: cYo.Common.Presentation.ControlGdiRenderer
// Assembly: cYo.Common.Presentation, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 57FA2993-F034-4522-A0AD-BF678FCFB8A4
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Presentation.dll

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Presentation
{
  public class ControlGdiRenderer : BitmapGdiRenderer, IControlRenderer, IBitmapRenderer, IDisposable
  {
    public ControlGdiRenderer(Control control, bool doubleBuffer)
    {
      if (doubleBuffer)
        typeof (Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((object) control, (object) true, (object[]) null);
      this.Control = control;
      this.Control.Paint += new PaintEventHandler(this.window_Paint);
    }

    private void window_Paint(object sender, PaintEventArgs e)
    {
      this.BeginScene(e.Graphics);
      this.OnPaint();
      this.EndScene();
    }

    public void Dispose()
    {
      this.Control.Paint -= new PaintEventHandler(this.window_Paint);
      GC.SuppressFinalize((object) this);
    }

    public Control Control { get; private set; }

    public Size Size => this.Control.Size;

    public void Draw() => this.Control.Invalidate();

    public event EventHandler Paint;

    protected virtual void OnPaint()
    {
      if (this.Paint == null)
        return;
      this.Paint((object) this, EventArgs.Empty);
    }
  }
}

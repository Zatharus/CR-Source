// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.Splash
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Net;
using cYo.Common.Threading;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class Splash : LayeredForm
  {
    private Rectangle payPalRect = Rectangle.Empty;
    private volatile int progress;
    private volatile string message;
    private int messageLines = 3;
    private Color progressColor = Color.White;
    private readonly EventWaitHandle initialized = new EventWaitHandle(false, EventResetMode.ManualReset);
    private bool hiPayPal;
    private int crashSequence;
    private IContainer components;
    private WebImage payPalImage;

    public Splash()
    {
      this.InitializeComponent();
      this.Surface = Resources.Splash.ScaleDpi();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Surface.SafeDispose();
        this.initialized.Close();
        this.components?.Dispose();
      }
      base.Dispose(disposing);
    }

    [DefaultValue(false)]
    public bool Fade { get; set; }

    [DefaultValue(0)]
    public int Progress
    {
      get => this.progress;
      set
      {
        if (this.progress == value)
          return;
        this.progress = value;
        this.Invalidate(this.ProgressBounds);
        if (this.InvokeRequired)
          return;
        this.Update();
      }
    }

    [DefaultValue(null)]
    public string Message
    {
      get => this.message;
      set
      {
        if (this.message == value)
          return;
        this.message = value;
        this.Invalidate(this.MessageBounds);
        if (this.InvokeRequired)
          return;
        this.Update();
      }
    }

    [DefaultValue(3)]
    public int MessageLines
    {
      get => this.messageLines;
      set
      {
        if (this.messageLines == value)
          return;
        this.messageLines = value;
        this.Invalidate(this.MessageBounds);
        if (this.InvokeRequired)
          return;
        this.Update();
      }
    }

    [DefaultValue(typeof (Color), "White")]
    public Color ProgressColor
    {
      get => this.progressColor;
      set
      {
        if (this.progressColor == value)
          return;
        this.progressColor = value;
        this.Invalidate(this.ProgressBounds);
      }
    }

    public EventWaitHandle Initialized => this.initialized;

    protected Rectangle ProgressBounds
    {
      get
      {
        Rectangle clientRectangle = this.ClientRectangle;
        return new Rectangle(clientRectangle.Left + FormUtility.ScaleDpiX(6), clientRectangle.Bottom - FormUtility.ScaleDpiY(52), clientRectangle.Width - FormUtility.ScaleDpiX(28), FormUtility.ScaleDpiY(2));
      }
    }

    protected Rectangle MessageBounds
    {
      get
      {
        Rectangle rectangle = this.ClientRectangle.Pad(0, 0, FormUtility.ScaleDpiX(16), FormUtility.ScaleDpiY(18));
        return new Rectangle(rectangle.Right - FormUtility.ScaleDpiX(204), rectangle.Bottom - FormUtility.ScaleDpiY(52) - (this.messageLines - 1) * this.Font.Height, FormUtility.ScaleDpiX(200), this.messageLines * this.Font.Height);
      }
    }

    private bool HiPayPal
    {
      get => this.hiPayPal;
      set
      {
        if (this.hiPayPal == value)
          return;
        this.hiPayPal = value;
        int num = this.hiPayPal ? 1 : 0;
        this.Invalidate(this.payPalRect);
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      this.Font = new Font(this.Font.FontFamily, (float) FormUtility.ScaleDpiY(11), GraphicsUnit.Pixel);
      this.payPalImage.CacheLocation = Program.Paths.ApplicationDataPath;
      if (!Program.Settings.IsActivated)
        this.payPalImage.LoadImage();
      this.Alpha = 0;
      this.Show();
      if (this.Fade)
        ThreadUtility.Animate(0, 250, (Action<float>) (f => this.Alpha = (int) ((double) f * (double) byte.MaxValue)));
      this.Initialized.Set();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (this.Fade)
        ThreadUtility.Animate(0, 250, (Action<float>) (f => this.Alpha = (int) byte.MaxValue - (int) ((double) f * (double) byte.MaxValue)));
      base.OnClosing(e);
    }

    protected override void OnClick(EventArgs e)
    {
      if (this.payPalRect.Contains(this.PointToClient(Cursor.Position)))
        Program.ShowPayPal();
      else
        this.Close();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      this.HiPayPal = this.payPalRect.Contains(e.Location);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      this.HiPayPal = false;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      switch (this.crashSequence)
      {
        case 0:
          if (e.KeyCode == Keys.C)
          {
            ++this.crashSequence;
            return;
          }
          break;
        case 1:
          if (e.KeyCode == Keys.R)
          {
            ++this.crashSequence;
            return;
          }
          break;
        case 2:
          if (e.KeyCode == Keys.A)
          {
            ++this.crashSequence;
            return;
          }
          break;
        case 3:
          if (e.KeyCode == Keys.S)
          {
            ++this.crashSequence;
            return;
          }
          break;
        case 4:
          if (e.KeyCode == Keys.H)
            throw new InvalidOperationException("CRASH!");
          break;
      }
      this.Close();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Rectangle rectangle = this.ClientRectangle.Pad(0, 0, FormUtility.ScaleDpiX(13), FormUtility.ScaleDpiY(17));
      string str = (Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof (AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute).Copyright + "\n" + "V " + Application.ProductVersion + string.Format(" {0} bit", (object) (Marshal.SizeOf(typeof (IntPtr)) * 8));
      System.Drawing.Size size = e.Graphics.MeasureString(str, this.Font).ToSize();
      using (StringFormat format = new StringFormat()
      {
        Alignment = StringAlignment.Far
      })
      {
        e.Graphics.DrawString(str, this.Font, Brushes.White, (float) (rectangle.Width - FormUtility.ScaleDpiX(8)), (float) (rectangle.Height - size.Height - FormUtility.ScaleDpiY(6)), format);
        using (Brush brush = (Brush) new SolidBrush(this.progressColor))
        {
          Rectangle progressBounds = this.ProgressBounds with
          {
            Width = this.progress * (rectangle.Width - FormUtility.ScaleDpiX(4)) / 100
          };
          e.Graphics.FillRectangle(brush, progressBounds);
        }
        if (!string.IsNullOrEmpty(this.message))
        {
          int alpha = 128;
          int num = alpha / this.messageLines;
          Rectangle messageBounds = this.MessageBounds;
          format.LineAlignment = StringAlignment.Far;
          string message = this.message;
          char[] chArray = new char[1]{ '\n' };
          foreach (string s in ((IEnumerable<string>) message.Split(chArray)).Reverse<string>().Take<string>(this.messageLines).ToArray<string>())
          {
            using (Brush brush = (Brush) new SolidBrush(Color.FromArgb(alpha, Color.Black)))
              e.Graphics.DrawString(s, this.Font, brush, (RectangleF) messageBounds, format);
            messageBounds.Height -= this.Font.Height;
            alpha -= num;
          }
        }
      }
      try
      {
        if (this.payPalImage.Image == null)
          return;
        this.payPalRect = new Rectangle(12, 12, this.payPalImage.Image.Width, this.payPalImage.Image.Height).ScaleDpi();
        if (this.HiPayPal)
          e.Graphics.DrawImage(this.payPalImage.Image, this.payPalRect, new BitmapAdjustment(0.1f, 0.1f));
        else
          e.Graphics.DrawImage((Image) this.payPalImage.Image, this.payPalRect);
      }
      catch
      {
      }
    }

    private void payPalImage_ImageLoaded(object sender, EventArgs e) => this.Invalidate();

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.payPalImage = new WebImage(this.components);
      this.SuspendLayout();
      this.payPalImage.CheckIntervall = TimeSpan.Parse("7.00:00:00");
      this.payPalImage.Name = "DonateImage";
      this.payPalImage.Uri = new Uri("https://www.paypal.com/en_US/i/btn/x-click-but04.gif", UriKind.Absolute);
      this.payPalImage.ImageLoaded += new EventHandler(this.payPalImage_ImageLoaded);
      this.AutoScaleMode = AutoScaleMode.None;
      this.ClientSize = new System.Drawing.Size(600, 300);
      this.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.FormBorderStyle = FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (Splash);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.Manual;
      this.TopMost = true;
      this.ResumeLayout(false);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.ReaderForm
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Windows;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public class ReaderForm : Form
  {
    private Rectangle safeBounds;
    private IContainer components;

    public ReaderForm(ComicDisplay comicDisplay)
    {
      this.InitializeComponent();
      this.Icon = Resources.ComicRackAppSmall;
      this.ComicDisplay = comicDisplay;
      LocalizeUtility.Localize((Control) this, this.components);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Icon.Dispose();
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void OnResizeEnd(EventArgs e)
    {
      base.OnResizeEnd(e);
      this.UpdateSafeBounds();
    }

    protected override void OnMove(EventArgs e)
    {
      base.OnMove(e);
      this.UpdateSafeBounds();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing && !Program.AskQuestion((IWin32Window) this, TR.Messages["CloseExternalReader", "This will only close the reader Window and not the open Book(s)!"], TR.Default["OK", "OK"], HiddenMessageBoxes.CloseExternalReader, TR.Messages["DontShowAgain", "Do not show this again"]))
        e.Cancel = true;
      if (e.Cancel)
        return;
      base.OnFormClosing(e);
    }

    public ComicDisplay ComicDisplay { get; set; }

    public Rectangle SafeBounds
    {
      get => this.safeBounds;
      set
      {
        this.StartPosition = FormStartPosition.Manual;
        this.Bounds = value;
        this.safeBounds = value;
      }
    }

    private void UpdateSafeBounds()
    {
      if (!this.IsHandleCreated || this.WindowState != FormWindowState.Normal || this.FormBorderStyle == FormBorderStyle.None)
        return;
      this.safeBounds = this.Bounds;
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(653, 621);
      this.KeyPreview = true;
      this.Name = nameof (ReaderForm);
      this.Text = "Reader";
      this.ResumeLayout(false);
    }
  }
}

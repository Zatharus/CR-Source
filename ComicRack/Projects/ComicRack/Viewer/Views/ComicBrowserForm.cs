// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicBrowserForm
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Win32;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicBrowserForm : Form
  {
    private IContainer components;
    private ComicBrowserControl comicBrowser;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel tsText;

    public ComicBrowserForm()
    {
      this.InitializeComponent();
      this.Icon = Resources.ComicRackAppSmall;
      this.statusStrip.Text = TR.Default["Ready", this.statusStrip.Text];
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        IdleProcess.Idle -= new EventHandler(this.OnIdle);
        IComicBookListProvider bookList = this.BookList;
        this.comicBrowser.BookList = (IComicBookListProvider) null;
        if (this.BookListOwned)
          bookList.Dispose();
        this.Icon.Dispose();
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      IdleProcess.Idle += new EventHandler(this.OnIdle);
      this.comicBrowser.HideNavigation = true;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IComicBookListProvider BookList
    {
      get => this.comicBrowser.BookList;
      set => this.comicBrowser.BookList = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IMain Main
    {
      get => this.comicBrowser.Main;
      set => this.comicBrowser.Main = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BookListOwned { get; set; }

    private void OnIdle(object sender, EventArgs e)
    {
      this.Text = this.comicBrowser.BookList.Name;
      this.tsText.Text = FormUtility.FixAmpersand(this.comicBrowser.SelectionInfo);
    }

    private void InitializeComponent()
    {
      this.comicBrowser = new ComicBrowserControl();
      this.statusStrip = new StatusStrip();
      this.tsText = new ToolStripStatusLabel();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
      this.comicBrowser.CaptionMargin = new Padding(2);
      this.comicBrowser.DisableViewConfigUpdate = true;
      this.comicBrowser.Dock = DockStyle.Fill;
      this.comicBrowser.Location = new System.Drawing.Point(0, 0);
      this.comicBrowser.Name = "comicBrowser";
      this.comicBrowser.Size = new System.Drawing.Size(589, 410);
      this.comicBrowser.TabIndex = 0;
      this.statusStrip.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.tsText
      });
      this.statusStrip.Location = new System.Drawing.Point(0, 410);
      this.statusStrip.Name = "statusStrip1";
      this.statusStrip.Size = new System.Drawing.Size(589, 22);
      this.statusStrip.TabIndex = 1;
      this.statusStrip.Text = "statusStrip";
      this.tsText.Name = "tsText";
      this.tsText.Size = new System.Drawing.Size(574, 17);
      this.tsText.Spring = true;
      this.tsText.Text = "Ready";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(589, 432);
      this.Controls.Add((Control) this.comicBrowser);
      this.Controls.Add((Control) this.statusStrip);
      this.Name = nameof (ComicBrowserForm);
      this.Text = nameof (ComicBrowserForm);
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

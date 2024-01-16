// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicBrowserView
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Projects.ComicRack.Engine;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicBrowserView : SubView
  {
    private ComicBrowserControl comicBrowser;

    public ComicBrowserView() => this.InitializeComponent();

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IComicBookListProvider BookList
    {
      get => this.comicBrowser.BookList;
      set => this.comicBrowser.BookList = value;
    }

    protected override void OnMainFormChanged()
    {
      base.OnMainFormChanged();
      this.comicBrowser.Main = this.Main;
    }

    private void InitializeComponent()
    {
      this.comicBrowser = new ComicBrowserControl();
      this.SuspendLayout();
      this.comicBrowser.Caption = "";
      this.comicBrowser.CaptionMargin = new Padding(2);
      this.comicBrowser.DisableViewConfigUpdate = true;
      this.comicBrowser.Dock = DockStyle.Fill;
      this.comicBrowser.Location = new Point(0, 0);
      this.comicBrowser.Name = "comicBrowser";
      this.comicBrowser.Size = new Size(643, 552);
      this.comicBrowser.TabIndex = 0;
      this.AutoScaleMode = AutoScaleMode.None;
      this.Controls.Add((Control) this.comicBrowser);
      this.Name = nameof (ComicBrowserView);
      this.Size = new Size(643, 552);
      this.ResumeLayout(false);
    }
  }
}

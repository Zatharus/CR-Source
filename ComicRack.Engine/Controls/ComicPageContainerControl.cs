// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Controls.ComicPageContainerControl
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Controls
{
  public class ComicPageContainerControl : ContainerControl
  {
    private IContainer components;
    private TabBar tabBar;

    public ComicPageContainerControl() => this.InitializeComponent();

    public IEnumerable<ComicPageControl> Pages => this.Controls.OfType<ComicPageControl>();

    public void ShowInfo(IEnumerable<ComicBook> books)
    {
      this.Pages.ForEach<ComicPageControl>((Action<ComicPageControl>) (p => p.ShowInfo(books)));
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
      base.OnControlAdded(e);
      Control c = e.Control;
      if (c is TabBar)
        return;
      string text = c.Text;
      Image image = (Image) null;
      if (e.Control is ComicPageControl control)
        image = control.Icon;
      TabBar.TabBarItem tbi = new TabBar.TabBarItem()
      {
        Text = text,
        Image = image,
        Tag = (object) c
      };
      c.TextChanged += (EventHandler) ((s, ea) => tbi.Text = c.Text);
      c.Visible = false;
      c.Dock = DockStyle.Fill;
      this.tabBar.Items.Add(tbi);
      this.tabBar.Visible = this.tabBar.Items.Count > 1;
      if (this.tabBar.Items.Count == 1)
        this.tabBar.SelectedTab = tbi;
      this.Controls.SetChildIndex((Control) this.tabBar, 1000);
    }

    protected override void OnControlRemoved(ControlEventArgs e)
    {
      ComicPageControl c = e.Control as ComicPageControl;
      TabBar.TabBarItem tabBarItem = this.tabBar.Items.FirstOrDefault<TabBar.TabBarItem>((Func<TabBar.TabBarItem, bool>) (t => t.Tag == c));
      if (tabBarItem != null)
      {
        this.tabBar.Items.Remove(tabBarItem);
        this.tabBar.Visible = this.tabBar.Items.Count > 1;
      }
      base.OnControlRemoved(e);
    }

    private void tabBar_SelectedTabChanged(object sender, TabBar.SelectedTabChangedEventArgs e)
    {
      foreach (TabBar.TabBarItem tabBarItem in (SmartList<TabBar.TabBarItem>) this.tabBar.Items)
        ((Control) tabBarItem.Tag).Visible = tabBarItem == e.NewItem;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.tabBar = new TabBar();
      this.SuspendLayout();
      this.tabBar.AllowDrop = true;
      this.tabBar.Dock = DockStyle.Top;
      this.tabBar.Location = new System.Drawing.Point(0, 0);
      this.tabBar.MinimumTabWidth = 100;
      this.tabBar.Name = "tabBar";
      this.tabBar.Size = new System.Drawing.Size(487, 31);
      this.tabBar.TabIndex = 0;
      this.tabBar.Text = "tabBar1";
      this.tabBar.SelectedTabChanged += new EventHandler<TabBar.SelectedTabChangedEventArgs>(this.tabBar_SelectedTabChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.tabBar);
      this.Name = "ComicInfoControl";
      this.Size = new System.Drawing.Size(487, 352);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

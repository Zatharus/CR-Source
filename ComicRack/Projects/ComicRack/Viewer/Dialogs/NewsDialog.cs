// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.NewsDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Net.News;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class NewsDialog : Form
  {
    private NewsStorage news;
    private IContainer components;
    private SplitContainer splitContainer;
    private ListView listNewItems;
    private WebBrowser webBrowser;
    private Button btClose;
    private ColumnHeader chTitle;
    private Button btMarkRead;
    private CheckBox chkNewsStartup;
    private Button btRefresh;

    public NewsDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      this.listNewItems.Columns.ScaleDpi();
      this.splitContainer.SplitterDistance = FormUtility.ScaleDpiX(this.splitContainer.SplitterDistance);
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.chkNewsStartup.Checked = Program.Settings.NewsStartup;
    }

    public NewsStorage News
    {
      get => this.news;
      set => this.news = value;
    }

    private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
      string absoluteUri = e.Url.AbsoluteUri;
      if (!(absoluteUri != "about:blank"))
        return;
      e.Cancel = true;
      NewsDialog.ShowUrlInBrowser(absoluteUri);
    }

    private void listNewItems_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.ShowItem(this.listNewItems.FocusedItem);
    }

    private void btMarkRead_Click(object sender, EventArgs e)
    {
      this.news.MarkAllRead();
      foreach (ListViewItem listViewItem in this.listNewItems.Items)
        listViewItem.Font = FC.Get(listViewItem.Font, FontStyle.Regular);
    }

    private void chkNewsStartup_CheckedChanged(object sender, EventArgs e)
    {
      Program.Settings.NewsStartup = this.chkNewsStartup.Checked;
    }

    private void btRefresh_Click(object sender, EventArgs e)
    {
      AutomaticProgressDialog.Process((Form) this, TR.Messages["RetrieveNews", "Retrieving News"], TR.Messages["RetrieveNewsText", "Refreshing subscribed News Channels"], 1000, (Action) (() => this.news.UpdateFeeds(0)), AutomaticProgressDialogOptions.None);
      this.FillList();
    }

    private static void ShowUrlInBrowser(string url)
    {
      try
      {
        Process.Start(url);
      }
      catch
      {
      }
    }

    private void ShowItem(ListViewItem lvi)
    {
      if (lvi == null)
        return;
      string str = string.Empty;
      try
      {
        str = File.ReadAllText(Path.Combine(Application.StartupPath, "NewsTemplate.html"));
      }
      catch
      {
      }
      NewsChannelItem tag = lvi.Tag as NewsChannelItem;
      try
      {
        this.webBrowser.DocumentText = str.Replace("#TITLE#", tag.Title).Replace("#TEXT#", tag.Description).Replace("#LINK#", tag.Link).Replace("#DATE#", tag.Published.ToString());
      }
      catch
      {
      }
      this.news.NewsChannelItemInfos[tag].IsRead = true;
      lvi.Font = FC.Get(lvi.Font, FontStyle.Regular);
    }

    private void FillList()
    {
      this.listNewItems.Items.Clear();
      NewsChannelItemCollection items = this.news.Items;
      items.Sort((Comparison<NewsChannelItem>) ((a, b) => DateTime.Compare(b.Published, a.Published)));
      foreach (NewsChannelItem newsChannelItem in (List<NewsChannelItem>) items)
      {
        ListViewItem listViewItem = this.listNewItems.Items.Add(newsChannelItem.Title);
        if (!this.news.NewsChannelItemInfos[newsChannelItem].IsRead)
          listViewItem.Font = FC.Get(listViewItem.Font, FontStyle.Bold);
        listViewItem.Tag = (object) newsChannelItem;
      }
      this.SelectFirstNotRead();
    }

    private void SelectFirstNotRead()
    {
      if (this.listNewItems.Items.Count <= 0)
        return;
      this.listNewItems.Items[0].Selected = true;
      this.ShowItem(this.listNewItems.Items[0]);
    }

    public static void ShowNews(IWin32Window parentForm, NewsStorage storage)
    {
      using (NewsDialog newsDialog = new NewsDialog())
      {
        newsDialog.News = storage;
        newsDialog.FillList();
        int num = (int) newsDialog.ShowDialog(parentForm);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.splitContainer = new SplitContainer();
      this.listNewItems = new ListView();
      this.chTitle = new ColumnHeader();
      this.webBrowser = new WebBrowser();
      this.btClose = new Button();
      this.btMarkRead = new Button();
      this.chkNewsStartup = new CheckBox();
      this.btRefresh = new Button();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      this.splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.splitContainer.BorderStyle = BorderStyle.FixedSingle;
      this.splitContainer.FixedPanel = FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(11, 13);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Panel1.Controls.Add((Control) this.listNewItems);
      this.splitContainer.Panel2.Controls.Add((Control) this.webBrowser);
      this.splitContainer.Size = new System.Drawing.Size(623, 361);
      this.splitContainer.SplitterDistance = 214;
      this.splitContainer.TabIndex = 0;
      this.listNewItems.BorderStyle = BorderStyle.None;
      this.listNewItems.Columns.AddRange(new ColumnHeader[1]
      {
        this.chTitle
      });
      this.listNewItems.Dock = DockStyle.Fill;
      this.listNewItems.FullRowSelect = true;
      this.listNewItems.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.listNewItems.HideSelection = false;
      this.listNewItems.Location = new System.Drawing.Point(0, 0);
      this.listNewItems.MultiSelect = false;
      this.listNewItems.Name = "listNewItems";
      this.listNewItems.Size = new System.Drawing.Size(212, 359);
      this.listNewItems.TabIndex = 0;
      this.listNewItems.UseCompatibleStateImageBehavior = false;
      this.listNewItems.View = View.Details;
      this.listNewItems.SelectedIndexChanged += new EventHandler(this.listNewItems_SelectedIndexChanged);
      this.chTitle.Text = "Title";
      this.chTitle.Width = 186;
      this.webBrowser.AllowWebBrowserDrop = false;
      this.webBrowser.Dock = DockStyle.Fill;
      this.webBrowser.Location = new System.Drawing.Point(0, 0);
      this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
      this.webBrowser.Name = "webBrowser";
      this.webBrowser.ScriptErrorsSuppressed = true;
      this.webBrowser.Size = new System.Drawing.Size(403, 359);
      this.webBrowser.TabIndex = 0;
      this.webBrowser.WebBrowserShortcutsEnabled = false;
      this.webBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
      this.btClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btClose.DialogResult = DialogResult.OK;
      this.btClose.FlatStyle = FlatStyle.System;
      this.btClose.Location = new System.Drawing.Point(557, 380);
      this.btClose.Name = "btClose";
      this.btClose.Size = new System.Drawing.Size(76, 24);
      this.btClose.TabIndex = 2;
      this.btClose.Text = "&Close";
      this.btMarkRead.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btMarkRead.Location = new System.Drawing.Point(448, 381);
      this.btMarkRead.Name = "btMarkRead";
      this.btMarkRead.Size = new System.Drawing.Size(103, 23);
      this.btMarkRead.TabIndex = 1;
      this.btMarkRead.Text = "Mark all as Read";
      this.btMarkRead.UseVisualStyleBackColor = true;
      this.btMarkRead.Click += new EventHandler(this.btMarkRead_Click);
      this.chkNewsStartup.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.chkNewsStartup.AutoSize = true;
      this.chkNewsStartup.Location = new System.Drawing.Point(12, 385);
      this.chkNewsStartup.Name = "chkNewsStartup";
      this.chkNewsStartup.Size = new System.Drawing.Size(154, 17);
      this.chkNewsStartup.TabIndex = 3;
      this.chkNewsStartup.Text = "Check for News on Startup";
      this.chkNewsStartup.UseVisualStyleBackColor = true;
      this.chkNewsStartup.CheckedChanged += new EventHandler(this.chkNewsStartup_CheckedChanged);
      this.btRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btRefresh.Location = new System.Drawing.Point(351, 381);
      this.btRefresh.Name = "btRefresh";
      this.btRefresh.Size = new System.Drawing.Size(91, 23);
      this.btRefresh.TabIndex = 0;
      this.btRefresh.Text = "Refresh";
      this.btRefresh.UseVisualStyleBackColor = true;
      this.btRefresh.Click += new EventHandler(this.btRefresh_Click);
      this.AcceptButton = (IButtonControl) this.btClose;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(645, 410);
      this.Controls.Add((Control) this.btRefresh);
      this.Controls.Add((Control) this.chkNewsStartup);
      this.Controls.Add((Control) this.btMarkRead);
      this.Controls.Add((Control) this.btClose);
      this.Controls.Add((Control) this.splitContainer);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(400, 250);
      this.Name = nameof (NewsDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Latest ComicRack News";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

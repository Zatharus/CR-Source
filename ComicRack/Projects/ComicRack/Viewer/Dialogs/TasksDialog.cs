// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.TasksDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.IO.Network;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class TasksDialog : Form
  {
    private readonly string counterFormat;
    private SimpleCache<string, Image> imageCache = new SimpleCache<string, Image>();
    private static readonly TR tr = TR.Load(nameof (TasksDialog));
    private static readonly string Clients = TasksDialog.tr[nameof (Clients), "Clients: {0}"];
    private static readonly string Info = TasksDialog.tr[nameof (Info), "Info Requests: {0}"];
    private static readonly string Library = TasksDialog.tr[nameof (Library), "Library Requests: {0}/{1}"];
    private static readonly string Page = TasksDialog.tr[nameof (Page), "Page Requests: {0}/{1}"];
    private static readonly string Thumbnail = TasksDialog.tr[nameof (Thumbnail), "Thumbnail Requests: {0}/{1}"];
    private static readonly string FailedAuth = TasksDialog.tr[nameof (FailedAuth), "Failed Authentications: {0}"];
    private static readonly string AllShares = TasksDialog.tr[nameof (AllShares), "All Shares"];
    private static readonly string LastMinute = TasksDialog.tr[nameof (LastMinute), "Last Minute"];
    private static readonly string Last5Minutes = TasksDialog.tr[nameof (Last5Minutes), "Last 5 Minutes"];
    private static readonly string LastHour = TasksDialog.tr[nameof (LastHour), "Last Hour"];
    private static readonly string Session = TasksDialog.tr[nameof (Session), nameof (Session)];
    private static readonly string waitingText = TR.Default["Waiting", "Waiting"];
    private static readonly string runningText = TR.Default["Running", "Running"];
    private static readonly string completedText = TR.Default["Completed", "Completed"];
    private IEnumerable<QueueManager.IPendingTasks> processes;
    private IContainer components;
    private ListViewEx lvTasks;
    private ColumnHeader colTask;
    private Timer updateTimer;
    private Label lblItemCount;
    private ImageList taskImages;
    private SplitButton btAbort;
    private ContextMenuStrip contextMenuAbort;
    private ToolStripMenuItem dummyToolStripMenuItem;
    private ColumnHeader colState;
    private TabControl tabs;
    private TabPage tabTasks;
    private TabPage tabNetwork;
    private ImageList networkImages;
    private TreeView tvStats;
    private Button btClose;

    public TasksDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.counterFormat = this.lblItemCount.Text;
      this.networkImages.Images.Add((Image) Resources.RemoteDatabase);
      this.networkImages.Images.Add((Image) Resources.Clock);
      this.networkImages.Images.Add((Image) Resources.Minus);
      this.lvTasks.Columns.ScaleDpi();
      this.RestorePosition();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        FormUtility.SafeToolStripClear(this.contextMenuAbort.Items);
        if (!this.tabs.TabPages.Contains(this.tabNetwork))
          this.tabs.TabPages.Add(this.tabNetwork);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    private Image GetImage(string imageKey)
    {
      return this.imageCache.Get(imageKey, (Func<string, Image>) (k => (Image) Resources.ResourceManager.GetObject(imageKey)));
    }

    private void UpdateTaskList()
    {
      if (this.processes == null)
        return;
      int num1 = 0;
      int num2 = 0;
      int index = this.lvTasks.TopItem != null ? this.lvTasks.TopItem.Index : 0;
      this.lvTasks.BeginUpdate();
      try
      {
        this.lvTasks.Items.Clear();
        foreach (QueueManager.IPendingTasks process in this.processes)
        {
          IEnumerable<object> source = process.GetPendingItems().OfType<object>();
          int num3 = source.Count<object>();
          num1 += num3;
          if (process.Abort != null)
            num2 += num3;
          ListViewGroup listViewGroup = this.lvTasks.Groups[process.Group] ?? this.lvTasks.Groups.Add(process.Group, process.Group);
          foreach (object obj in source.Take<object>(10))
          {
            ListViewItem listViewItem = this.lvTasks.Items.Add(obj.ToString());
            listViewItem.ImageKey = process.Group;
            listViewItem.Group = listViewGroup;
            if (!(obj is IProgressState progressState))
            {
              listViewItem.SubItems.Add(TasksDialog.runningText);
            }
            else
            {
              listViewItem.Tag = (object) progressState;
              switch (progressState.State)
              {
                case ProgressState.Waiting:
                  listViewItem.SubItems.Add(TasksDialog.waitingText);
                  continue;
                case ProgressState.Running:
                  listViewItem.SubItems.Add(progressState.ProgressAvailable ? string.Format("{0}%", (object) progressState.ProgressPercentage) : TasksDialog.runningText);
                  continue;
                case ProgressState.Completed:
                  listViewItem.SubItems.Add(TasksDialog.completedText);
                  continue;
                default:
                  continue;
              }
            }
          }
          if (num3 > 10)
          {
            ListViewItem listViewItem = this.lvTasks.Items.Add(string.Format(TR.Messages["ListMore", "{0} more..."], (object) (num3 - 10)));
            listViewItem.ForeColor = SystemColors.ControlLight;
            listViewItem.Group = listViewGroup;
          }
        }
        if (index < this.lvTasks.Items.Count)
          this.lvTasks.TopItem = this.lvTasks.Items[index];
      }
      finally
      {
        this.lvTasks.EndUpdate();
      }
      this.lblItemCount.Text = StringUtility.Format(this.counterFormat, (object) num1);
      this.btAbort.Enabled = num2 > 0;
    }

    private static void AddNodeEntry(TreeNode tn, string text, params object[] p)
    {
      TreeNode node = tn.Nodes[text];
      string text1 = string.Format(text, p);
      if (node != null)
        node.Text = text1;
      else
        tn.Nodes.Add(text, text1, 2, 2);
    }

    private void AddStats(TreeNode tnServer, string name, ServerStatistics.StatisticResult data)
    {
      FileLengthFormat fileLengthFormat = new FileLengthFormat();
      TreeNode tn = tnServer.Nodes[name] ?? tnServer.Nodes.Add(name, name, 1, 1);
      TasksDialog.AddNodeEntry(tn, TasksDialog.Clients, (object) data.ClientCount);
      TasksDialog.AddNodeEntry(tn, TasksDialog.Info, (object) data.InfoRequestCount);
      TasksDialog.AddNodeEntry(tn, TasksDialog.Library, (object) data.LibraryRequestCount, (object) fileLengthFormat.Format((string) null, (object) data.LibraryRequestSize, (IFormatProvider) null));
      TasksDialog.AddNodeEntry(tn, TasksDialog.Page, (object) data.PageRequestCount, (object) fileLengthFormat.Format((string) null, (object) data.PageRequestSize, (IFormatProvider) null));
      TasksDialog.AddNodeEntry(tn, TasksDialog.Thumbnail, (object) data.ThumbnailRequestCount, (object) fileLengthFormat.Format((string) null, (object) data.ThumbnailRequestSize, (IFormatProvider) null));
      TasksDialog.AddNodeEntry(tn, TasksDialog.FailedAuth, (object) data.FailedAuthenticationCount);
    }

    private void UpdateServerStats()
    {
      this.tvStats.BeginUpdate();
      ServerStatistics.StatisticResult data1 = new ServerStatistics.StatisticResult();
      ServerStatistics.StatisticResult data2 = new ServerStatistics.StatisticResult();
      ServerStatistics.StatisticResult data3 = new ServerStatistics.StatisticResult();
      ServerStatistics.StatisticResult data4 = new ServerStatistics.StatisticResult();
      try
      {
        foreach (ComicLibraryServer runningServer in Program.NetworkManager.RunningServers)
        {
          TreeNode tnServer = this.tvStats.Nodes[runningServer.Config.Name];
          bool flag = false;
          if (tnServer == null)
          {
            tnServer = this.tvStats.Nodes.Add(runningServer.Config.Name, runningServer.Config.Name, 0, 0);
            flag = true;
          }
          ServerStatistics.StatisticResult result1 = runningServer.Statistics.GetResult(60);
          ServerStatistics.StatisticResult result2 = runningServer.Statistics.GetResult(300);
          ServerStatistics.StatisticResult result3 = runningServer.Statistics.GetResult(3600);
          ServerStatistics.StatisticResult result4 = runningServer.Statistics.GetResult();
          this.AddStats(tnServer, TasksDialog.LastMinute, result1);
          this.AddStats(tnServer, TasksDialog.Last5Minutes, result2);
          this.AddStats(tnServer, TasksDialog.LastHour, result3);
          this.AddStats(tnServer, TasksDialog.Session, result4);
          if (flag)
            tnServer.Expand();
          data1.Add(result1);
          data2.Add(result2);
          data3.Add(result3);
          data4.Add(result4);
        }
        if (Program.NetworkManager.RunningServers.Count > 1)
        {
          TreeNode tnServer = this.tvStats.Nodes["All Shares"] ?? this.tvStats.Nodes.Add("All Shares", "All Shares", 0, 0);
          this.AddStats(tnServer, TasksDialog.LastMinute, data1);
          this.AddStats(tnServer, TasksDialog.Last5Minutes, data2);
          this.AddStats(tnServer, TasksDialog.LastHour, data3);
          this.AddStats(tnServer, TasksDialog.Session, data4);
        }
        if (this.tvStats.Nodes.Count == 0)
        {
          if (!this.tabs.TabPages.Contains(this.tabNetwork))
            return;
          this.tabs.TabPages.Remove(this.tabNetwork);
        }
        else
        {
          if (this.tabs.TabPages.Contains(this.tabNetwork))
            return;
          this.tabs.TabPages.Add(this.tabNetwork);
        }
      }
      finally
      {
        this.tvStats.EndUpdate();
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.UpdateTaskList();
      this.UpdateServerStats();
    }

    public IEnumerable<QueueManager.IPendingTasks> Processes
    {
      get => this.processes;
      set
      {
        this.taskImages.Images.Clear();
        this.processes = value;
        foreach (QueueManager.IPendingTasks process in this.processes)
          this.taskImages.Images.Add(process.Group, this.GetImage(process.TasksImageKey));
        this.UpdateTaskList();
      }
    }

    public int SelectedTab
    {
      get => this.tabs.SelectedIndex;
      set => this.tabs.SelectedIndex = value;
    }

    private void updateTimer_Tick(object sender, EventArgs e)
    {
      this.UpdateTaskList();
      this.UpdateServerStats();
    }

    private void btAbort_Click(object sender, EventArgs e)
    {
      foreach (QueueManager.IPendingTasks process in this.processes)
      {
        if (process.Abort != null)
          process.Abort();
      }
    }

    private void contextMenuAbort_Opening(object sender, CancelEventArgs e)
    {
      FormUtility.SafeToolStripClear(this.contextMenuAbort.Items);
      foreach (QueueManager.IPendingTasks process in this.processes)
      {
        Action abort = process.Abort;
        if (abort != null && process.GetPendingItems().Count > 0)
          this.contextMenuAbort.Items.Add(process.AbortCommandText, this.GetImage(process.TasksImageKey), (EventHandler) ((a, b) => abort()));
      }
    }

    private void lvTasks_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
      IProgressState tag = e.Item.Tag as IProgressState;
      if (e.ColumnIndex != 1 || tag == null || !tag.ProgressAvailable || tag.State != ProgressState.Running)
      {
        e.DrawDefault = true;
      }
      else
      {
        Rectangle bounds = e.Bounds;
        bounds.Width = Math.Min(bounds.Width, bounds.Width * tag.ProgressPercentage / 100);
        --bounds.Height;
        e.DrawBackground();
        e.Graphics.DrawStyledRectangle(bounds, StyledRenderer.AlphaStyle.Hot, Color.Green, StyledRenderer.Default.Frame(0, 1));
        ListViewItem.ListViewSubItem subItem = e.Item.SubItems[1];
        using (StringFormat format = new StringFormat()
        {
          LineAlignment = StringAlignment.Center,
          Alignment = StringAlignment.Center,
          Trimming = StringTrimming.EllipsisCharacter
        })
        {
          using (Brush brush = (Brush) new SolidBrush(subItem.ForeColor))
            e.Graphics.DrawString(subItem.Text, subItem.Font, brush, (RectangleF) e.Bounds, format);
        }
      }
    }

    private void lvTasks_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
    {
      e.DrawDefault = true;
    }

    public static TasksDialog Show(
      IWin32Window parent,
      IEnumerable<QueueManager.IPendingTasks> processes,
      int tab = 0)
    {
      TasksDialog dlg = new TasksDialog()
      {
        Processes = processes,
        tabs = {
          SelectedIndex = tab
        }
      };
      dlg.Show(parent);
      dlg.FormClosed += (FormClosedEventHandler) ((s, e) => dlg.Dispose());
      dlg.btClose.Click += (EventHandler) ((s, e) => dlg.Close());
      return dlg;
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.lvTasks = new ListViewEx();
      this.colTask = new ColumnHeader();
      this.colState = new ColumnHeader();
      this.taskImages = new ImageList(this.components);
      this.updateTimer = new Timer(this.components);
      this.lblItemCount = new Label();
      this.btAbort = new SplitButton();
      this.contextMenuAbort = new ContextMenuStrip(this.components);
      this.dummyToolStripMenuItem = new ToolStripMenuItem();
      this.tabs = new TabControl();
      this.tabTasks = new TabPage();
      this.tabNetwork = new TabPage();
      this.tvStats = new TreeView();
      this.networkImages = new ImageList(this.components);
      this.btClose = new Button();
      this.contextMenuAbort.SuspendLayout();
      this.tabs.SuspendLayout();
      this.tabTasks.SuspendLayout();
      this.tabNetwork.SuspendLayout();
      this.SuspendLayout();
      this.lvTasks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.lvTasks.Columns.AddRange(new ColumnHeader[2]
      {
        this.colTask,
        this.colState
      });
      this.lvTasks.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this.lvTasks.Location = new System.Drawing.Point(6, 6);
      this.lvTasks.Name = "lvTasks";
      this.lvTasks.OwnerDraw = true;
      this.lvTasks.Size = new System.Drawing.Size(588, 385);
      this.lvTasks.SmallImageList = this.taskImages;
      this.lvTasks.TabIndex = 0;
      this.lvTasks.UseCompatibleStateImageBehavior = false;
      this.lvTasks.View = View.Details;
      this.lvTasks.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(this.lvTasks_DrawColumnHeader);
      this.lvTasks.DrawSubItem += new DrawListViewSubItemEventHandler(this.lvTasks_DrawSubItem);
      this.colTask.Text = "Task";
      this.colTask.Width = 437;
      this.colState.Text = "State";
      this.colState.Width = 87;
      this.taskImages.ColorDepth = ColorDepth.Depth8Bit;
      this.taskImages.ImageSize = new System.Drawing.Size(16, 16);
      this.taskImages.TransparentColor = Color.Transparent;
      this.updateTimer.Enabled = true;
      this.updateTimer.Interval = 1000;
      this.updateTimer.Tick += new EventHandler(this.updateTimer_Tick);
      this.lblItemCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.lblItemCount.AutoSize = true;
      this.lblItemCount.Location = new System.Drawing.Point(6, 402);
      this.lblItemCount.Name = "lblItemCount";
      this.lblItemCount.Size = new System.Drawing.Size(112, 13);
      this.lblItemCount.TabIndex = 1;
      this.lblItemCount.Text = "{0} Tasks are pending";
      this.btAbort.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btAbort.AutoEllipsis = true;
      this.btAbort.ContextMenuStrip = this.contextMenuAbort;
      this.btAbort.Location = new System.Drawing.Point(413, 397);
      this.btAbort.Name = "btAbort";
      this.btAbort.Size = new System.Drawing.Size(181, 23);
      this.btAbort.TabIndex = 2;
      this.btAbort.Text = "Abort all User Tasks";
      this.btAbort.UseVisualStyleBackColor = true;
      this.btAbort.Click += new EventHandler(this.btAbort_Click);
      this.contextMenuAbort.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.dummyToolStripMenuItem
      });
      this.contextMenuAbort.Name = "contextMenuAbort";
      this.contextMenuAbort.Size = new System.Drawing.Size(118, 26);
      this.contextMenuAbort.Opening += new CancelEventHandler(this.contextMenuAbort_Opening);
      this.dummyToolStripMenuItem.Name = "dummyToolStripMenuItem";
      this.dummyToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
      this.dummyToolStripMenuItem.Text = "Dummy";
      this.tabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.tabs.Controls.Add((Control) this.tabTasks);
      this.tabs.Controls.Add((Control) this.tabNetwork);
      this.tabs.Location = new System.Drawing.Point(12, 6);
      this.tabs.Name = "tabs";
      this.tabs.SelectedIndex = 0;
      this.tabs.Size = new System.Drawing.Size(608, 452);
      this.tabs.TabIndex = 4;
      this.tabTasks.Controls.Add((Control) this.lvTasks);
      this.tabTasks.Controls.Add((Control) this.lblItemCount);
      this.tabTasks.Controls.Add((Control) this.btAbort);
      this.tabTasks.Location = new System.Drawing.Point(4, 22);
      this.tabTasks.Name = "tabTasks";
      this.tabTasks.Padding = new Padding(3);
      this.tabTasks.Size = new System.Drawing.Size(600, 426);
      this.tabTasks.TabIndex = 0;
      this.tabTasks.Text = "Background Tasks";
      this.tabTasks.UseVisualStyleBackColor = true;
      this.tabNetwork.Controls.Add((Control) this.tvStats);
      this.tabNetwork.Location = new System.Drawing.Point(4, 22);
      this.tabNetwork.Name = "tabNetwork";
      this.tabNetwork.Padding = new Padding(3);
      this.tabNetwork.Size = new System.Drawing.Size(600, 426);
      this.tabNetwork.TabIndex = 1;
      this.tabNetwork.Text = "Server Statistics";
      this.tabNetwork.UseVisualStyleBackColor = true;
      this.tvStats.Dock = DockStyle.Fill;
      this.tvStats.ImageIndex = 0;
      this.tvStats.ImageList = this.networkImages;
      this.tvStats.Location = new System.Drawing.Point(3, 3);
      this.tvStats.Name = "tvStats";
      this.tvStats.SelectedImageIndex = 0;
      this.tvStats.ShowLines = false;
      this.tvStats.ShowRootLines = false;
      this.tvStats.Size = new System.Drawing.Size(594, 420);
      this.tvStats.TabIndex = 1;
      this.networkImages.ColorDepth = ColorDepth.Depth32Bit;
      this.networkImages.ImageSize = new System.Drawing.Size(16, 16);
      this.networkImages.TransparentColor = Color.Transparent;
      this.btClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btClose.DialogResult = DialogResult.OK;
      this.btClose.FlatStyle = FlatStyle.System;
      this.btClose.Location = new System.Drawing.Point(524, 464);
      this.btClose.Name = "btClose";
      this.btClose.Size = new System.Drawing.Size(96, 24);
      this.btClose.TabIndex = 5;
      this.btClose.Text = "&Close";
      this.AcceptButton = (IButtonControl) this.btClose;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btClose;
      this.ClientSize = new System.Drawing.Size(632, 499);
      this.Controls.Add((Control) this.btClose);
      this.Controls.Add((Control) this.tabs);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(400, 200);
      this.Name = nameof (TasksDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Tasks";
      this.contextMenuAbort.ResumeLayout(false);
      this.tabs.ResumeLayout(false);
      this.tabTasks.ResumeLayout(false);
      this.tabTasks.PerformLayout();
      this.tabNetwork.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

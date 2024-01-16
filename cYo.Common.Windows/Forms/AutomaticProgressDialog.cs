// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.AutomaticProgressDialog
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class AutomaticProgressDialog : Form
  {
    private Thread thread;
    private Action exectuteMethod;
    private readonly ManualResetEvent finishEvent = new ManualResetEvent(false);
    private volatile bool threadCompleted;
    private Exception catchedException;
    private static readonly Dictionary<int, bool> stopLookup = new Dictionary<int, bool>();
    private static readonly Dictionary<int, int> valueLookup = new Dictionary<int, int>();
    private IContainer components;
    private ProgressBar progressBar;
    private Label labelCaption;
    private System.Windows.Forms.Timer threadCheckTimer;
    private FlowLayoutPanel flowLayout;
    private Button btCancel;

    public AutomaticProgressDialog() => this.InitializeComponent();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
      {
        this.finishEvent.Close();
        this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void threadCheckTimer_Tick(object sender, EventArgs e)
    {
      if (this.threadCompleted)
        this.Close();
      else
        this.SetProgressStyle();
    }

    private void btCancel_Click(object sender, EventArgs e)
    {
      this.btCancel.Enabled = false;
      using (ItemMonitor.Lock((object) AutomaticProgressDialog.stopLookup))
        AutomaticProgressDialog.stopLookup[this.thread.ManagedThreadId] = true;
      try
      {
        if (this.thread.Join(3000))
          return;
        this.thread.Abort();
        this.thread.Join();
      }
      catch
      {
      }
    }

    private void SetProgressStyle()
    {
      int num;
      using (ItemMonitor.Lock((object) AutomaticProgressDialog.valueLookup))
        AutomaticProgressDialog.valueLookup.TryGetValue(this.thread.ManagedThreadId, out num);
      this.progressBar.Style = num < 0 ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
      this.progressBar.Value = num.Clamp(this.progressBar.Minimum, this.progressBar.Maximum);
    }

    private bool DoProcess(IWin32Window parent, int timeToWait)
    {
      bool flag;
      using (new WaitCursor())
      {
        this.thread = ThreadUtility.CreateWorkerThread(nameof (DoProcess), new ThreadStart(this.Execute), ThreadPriority.Normal);
        this.thread.Start();
        flag = !this.finishEvent.WaitOne(timeToWait, false);
      }
      if (flag)
      {
        this.SetProgressStyle();
        if (parent == null)
        {
          this.StartPosition = FormStartPosition.CenterScreen;
          int num = (int) this.ShowDialog();
        }
        else
        {
          int num1 = (int) this.ShowDialog(parent);
        }
      }
      if (this.catchedException != null)
        throw this.catchedException;
      return true;
    }

    private void Execute()
    {
      try
      {
        AutomaticProgressDialog.Value = -1;
        this.exectuteMethod();
      }
      catch (ThreadAbortException ex)
      {
      }
      catch (Exception ex)
      {
        this.catchedException = ex;
      }
      finally
      {
        using (ItemMonitor.Lock((object) AutomaticProgressDialog.stopLookup))
          AutomaticProgressDialog.stopLookup.Remove(this.thread.ManagedThreadId);
        try
        {
          this.finishEvent.Set();
        }
        catch
        {
        }
        this.threadCompleted = true;
      }
    }

    public static bool Process(
      IWin32Window parent,
      string caption,
      string description,
      int timeToWait,
      Action exectuteMethod,
      AutomaticProgressDialogOptions options)
    {
      using (AutomaticProgressDialog automaticProgressDialog = new AutomaticProgressDialog())
      {
        automaticProgressDialog.Text = caption;
        automaticProgressDialog.labelCaption.Text = description;
        automaticProgressDialog.exectuteMethod = exectuteMethod;
        automaticProgressDialog.btCancel.Text = TR.Default["Cancel", "Cancel"];
        automaticProgressDialog.btCancel.Visible = (options & AutomaticProgressDialogOptions.EnableCancel) != 0;
        if (parent == null)
        {
          automaticProgressDialog.TopLevel = true;
          automaticProgressDialog.TopMost = true;
        }
        return automaticProgressDialog.DoProcess(parent, timeToWait);
      }
    }

    public static bool Process(
      Form parent,
      string caption,
      string description,
      int timeToWait,
      Action exectuteMethod,
      AutomaticProgressDialogOptions options)
    {
      IWin32Window parent1 = (IWin32Window) parent;
      if (parent != null && (!parent.Visible || parent.WindowState == FormWindowState.Minimized))
        parent1 = (IWin32Window) null;
      return AutomaticProgressDialog.Process(parent1, caption, description, timeToWait, exectuteMethod, options);
    }

    public static bool ShouldAbort
    {
      get
      {
        using (ItemMonitor.Lock((object) AutomaticProgressDialog.stopLookup))
        {
          bool flag;
          return AutomaticProgressDialog.stopLookup.TryGetValue(Thread.CurrentThread.ManagedThreadId, out flag) & flag;
        }
      }
    }

    public static int Value
    {
      get
      {
        using (ItemMonitor.Lock((object) AutomaticProgressDialog.valueLookup))
        {
          int num;
          return AutomaticProgressDialog.valueLookup.TryGetValue(Thread.CurrentThread.ManagedThreadId, out num) ? num : -1;
        }
      }
      set
      {
        using (ItemMonitor.Lock((object) AutomaticProgressDialog.valueLookup))
          AutomaticProgressDialog.valueLookup[Thread.CurrentThread.ManagedThreadId] = value;
      }
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.progressBar = new ProgressBar();
      this.labelCaption = new Label();
      this.threadCheckTimer = new System.Windows.Forms.Timer(this.components);
      this.flowLayout = new FlowLayoutPanel();
      this.btCancel = new Button();
      this.flowLayout.SuspendLayout();
      this.SuspendLayout();
      this.progressBar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.progressBar.Location = new System.Drawing.Point(0, 29);
      this.progressBar.Margin = new Padding(0, 16, 0, 0);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(306, 24);
      this.progressBar.Style = ProgressBarStyle.Marquee;
      this.progressBar.TabIndex = 0;
      this.labelCaption.AutoSize = true;
      this.labelCaption.Location = new System.Drawing.Point(3, 0);
      this.labelCaption.MinimumSize = new System.Drawing.Size(300, 0);
      this.labelCaption.Name = "labelCaption";
      this.labelCaption.Size = new System.Drawing.Size(300, 13);
      this.labelCaption.TabIndex = 0;
      this.labelCaption.Text = "This is the description of the action";
      this.labelCaption.TextAlign = ContentAlignment.MiddleCenter;
      this.threadCheckTimer.Enabled = true;
      this.threadCheckTimer.Interval = 500;
      this.threadCheckTimer.Tick += new EventHandler(this.threadCheckTimer_Tick);
      this.flowLayout.AutoSize = true;
      this.flowLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayout.Controls.Add((Control) this.labelCaption);
      this.flowLayout.Controls.Add((Control) this.progressBar);
      this.flowLayout.Controls.Add((Control) this.btCancel);
      this.flowLayout.FlowDirection = FlowDirection.TopDown;
      this.flowLayout.Location = new System.Drawing.Point(8, 9);
      this.flowLayout.Margin = new Padding(8);
      this.flowLayout.Name = "flowLayout";
      this.flowLayout.Size = new System.Drawing.Size(306, 85);
      this.flowLayout.TabIndex = 1;
      this.btCancel.Anchor = AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(226, 61);
      this.btCancel.Margin = new Padding(0, 8, 0, 0);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 7;
      this.btCancel.Text = "&Cancel";
      this.btCancel.Click += new EventHandler(this.btCancel_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.ClientSize = new System.Drawing.Size(322, 102);
      this.ControlBox = false;
      this.Controls.Add((Control) this.flowLayout);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (AutomaticProgressDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = nameof (AutomaticProgressDialog);
      this.flowLayout.ResumeLayout(false);
      this.flowLayout.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

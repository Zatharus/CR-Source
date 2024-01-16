// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.CrashDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Runtime;
using cYo.Common.Threading;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Viewer.Remote;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class CrashDialog : Form
  {
    private BarkType crashType = BarkType.ThreadException;
    private IContainer components;
    private Button btRestart;
    private Button btResume;
    private Label labelMessage;
    private CheckBox chkSubmit;
    private Button btDetails;
    private TextBox tbLog;
    private Timer lockTimer;
    private Button btExit;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel panel1;

    public CrashDialog()
    {
      LocalizeUtility.UpdateRightToLeft((Form) this);
      this.InitializeComponent();
      try
      {
        LocalizeUtility.Localize((Control) this, (IContainer) null);
      }
      catch
      {
      }
    }

    public BarkType CrashType
    {
      get => this.crashType;
      set => this.crashType = value;
    }

    private void btDetails_Click(object sender, EventArgs e)
    {
      this.btDetails.Visible = false;
      this.tbLog.Visible = true;
    }

    private void lockTimer_Tick(object sender, EventArgs e)
    {
      if (this.crashType != BarkType.Lock || ThreadUtility.IsForegroundLocked)
        return;
      this.Close();
    }

    public static void Show(string report, BarkType crashType, bool enableSend)
    {
      using (CrashDialog crashDialog = new CrashDialog())
      {
        crashDialog.tbLog.Text = report;
        crashDialog.CrashType = crashType;
        if (!enableSend)
        {
          crashDialog.labelMessage.Visible = false;
          crashDialog.chkSubmit.Checked = false;
          crashDialog.chkSubmit.Enabled = enableSend;
        }
        DialogResult dialogResult = crashDialog.ShowDialog();
        if (crashType == BarkType.Lock && !ThreadUtility.IsForegroundLocked)
          return;
        if (crashDialog.chkSubmit.Checked)
          CrashDialog.SubmitReport(report);
        try
        {
          switch (dialogResult)
          {
            case DialogResult.OK:
              Application.Restart();
              break;
            case DialogResult.Cancel:
              Environment.Exit(1);
              break;
            case DialogResult.Retry:
              ThreadUtility.BreakForegroundLock();
              break;
          }
        }
        catch
        {
          Environment.Exit(1);
        }
      }
    }

    public static void SubmitReport(string report)
    {
      try
      {
        using (new WaitCursor())
          new CrashReport().SubmitReport(Application.ProductName + Application.ProductVersion.Replace(".", string.Empty), report);
      }
      catch
      {
      }
    }

    public static void OnBark(object sender, BarkEventArgs e)
    {
      if (e.Exception == null)
        return;
      bool enableSend = !e.Exception.ToString().Contains("Microsoft.Scripting") && !e.Exception.ToString().Contains("Python");
      using (StringWriter stringWriter = new StringWriter())
      {
        Diagnostic.WriteProgramInfo((TextWriter) stringWriter);
        stringWriter.WriteLine(e.Bark.ToString().ToUpper());
        CrashDialog.AddException(stringWriter, e.Exception);
        ThreadUtility.DumpStacks((TextWriter) stringWriter);
        stringWriter.WriteLine(new string('-', 20));
        stringWriter.WriteLine("Report generated at: {0}", (object) DateTime.Now);
        CrashDialog.Show(stringWriter.ToString(), e.Bark, enableSend);
      }
    }

    private static void AddException(StringWriter sw, Exception exception)
    {
      if (exception == null)
        return;
      sw.WriteLine(new string('-', 20));
      sw.WriteLine(exception.GetType().Name);
      if (exception.TargetSite != (MethodBase) null)
        sw.WriteLine(exception.TargetSite.ToString());
      sw.WriteLine(exception.Message);
      sw.WriteLine(exception.StackTrace);
      sw.WriteLine((object) exception.InnerException);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.btRestart = new Button();
      this.btResume = new Button();
      this.labelMessage = new Label();
      this.chkSubmit = new CheckBox();
      this.btDetails = new Button();
      this.tbLog = new TextBox();
      this.lockTimer = new Timer(this.components);
      this.btExit = new Button();
      this.flowLayoutPanel1 = new FlowLayoutPanel();
      this.panel1 = new Panel();
      this.flowLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.btRestart.DialogResult = DialogResult.OK;
      this.btRestart.FlatStyle = FlatStyle.System;
      this.btRestart.Location = new System.Drawing.Point(355, 7);
      this.btRestart.Name = "btRestart";
      this.btRestart.Size = new System.Drawing.Size(68, 24);
      this.btRestart.TabIndex = 4;
      this.btRestart.Text = "&Restart";
      this.btResume.DialogResult = DialogResult.Retry;
      this.btResume.FlatStyle = FlatStyle.System;
      this.btResume.Location = new System.Drawing.Point(276, 7);
      this.btResume.Name = "btResume";
      this.btResume.Size = new System.Drawing.Size(73, 24);
      this.btResume.TabIndex = 3;
      this.btResume.Text = "&Try to resume";
      this.labelMessage.AutoSize = true;
      this.labelMessage.Location = new System.Drawing.Point(3, 0);
      this.labelMessage.MaximumSize = new System.Drawing.Size(500, 0);
      this.labelMessage.Name = "labelMessage";
      this.labelMessage.Size = new System.Drawing.Size(489, 26);
      this.labelMessage.TabIndex = 0;
      this.labelMessage.Text = "Please help to make ComicRack a better application by submitting the follwing report. This report does not contain any data that could identify you.";
      this.chkSubmit.AutoSize = true;
      this.chkSubmit.Checked = true;
      this.chkSubmit.CheckState = CheckState.Checked;
      this.chkSubmit.Location = new System.Drawing.Point(3, 12);
      this.chkSubmit.Name = "chkSubmit";
      this.chkSubmit.Size = new System.Drawing.Size(88, 17);
      this.chkSubmit.TabIndex = 1;
      this.chkSubmit.Text = "Submit report";
      this.chkSubmit.UseVisualStyleBackColor = true;
      this.btDetails.FlatStyle = FlatStyle.System;
      this.btDetails.Location = new System.Drawing.Point(197, 7);
      this.btDetails.Name = "btDetails";
      this.btDetails.Size = new System.Drawing.Size(73, 24);
      this.btDetails.TabIndex = 2;
      this.btDetails.Text = "&Details";
      this.btDetails.Click += new EventHandler(this.btDetails_Click);
      this.tbLog.Font = new Font("Courier New", 6.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.tbLog.Location = new System.Drawing.Point(3, 69);
      this.tbLog.Multiline = true;
      this.tbLog.Name = "tbLog";
      this.tbLog.ReadOnly = true;
      this.tbLog.ScrollBars = ScrollBars.Both;
      this.tbLog.Size = new System.Drawing.Size(500, 250);
      this.tbLog.TabIndex = 6;
      this.tbLog.Visible = false;
      this.tbLog.WordWrap = false;
      this.lockTimer.Enabled = true;
      this.lockTimer.Tick += new EventHandler(this.lockTimer_Tick);
      this.btExit.DialogResult = DialogResult.Cancel;
      this.btExit.FlatStyle = FlatStyle.System;
      this.btExit.Location = new System.Drawing.Point(429, 7);
      this.btExit.Name = "btExit";
      this.btExit.Size = new System.Drawing.Size(68, 24);
      this.btExit.TabIndex = 5;
      this.btExit.Text = "&Exit";
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add((Control) this.labelMessage);
      this.flowLayoutPanel1.Controls.Add((Control) this.panel1);
      this.flowLayoutPanel1.Controls.Add((Control) this.tbLog);
      this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(506, 322);
      this.flowLayoutPanel1.TabIndex = 7;
      this.panel1.Controls.Add((Control) this.chkSubmit);
      this.panel1.Controls.Add((Control) this.btExit);
      this.panel1.Controls.Add((Control) this.btDetails);
      this.panel1.Controls.Add((Control) this.btResume);
      this.panel1.Controls.Add((Control) this.btRestart);
      this.panel1.Location = new System.Drawing.Point(3, 29);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(500, 34);
      this.panel1.TabIndex = 1;
      this.AcceptButton = (IButtonControl) this.btResume;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoScrollMargin = new System.Drawing.Size(8, 8);
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.CancelButton = (IButtonControl) this.btExit;
      this.ClientSize = new System.Drawing.Size(525, 352);
      this.Controls.Add((Control) this.flowLayoutPanel1);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (CrashDialog);
      this.ShowIcon = false;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "ComicRack encountered a Problem...";
      this.TopMost = true;
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

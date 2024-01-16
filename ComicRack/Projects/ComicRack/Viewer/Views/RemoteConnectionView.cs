// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.RemoteConnectionView
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Cryptography;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO.Network;
using cYo.Projects.ComicRack.Viewer.Dialogs;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class RemoteConnectionView : SubView
  {
    private Thread thread;
    private bool cancelConnection;
    private bool openNow;
    private bool autoOpen;
    private Image oldImage;
    private string textConnect = TR.Default["Connect", "Connect"];
    private string textCancel = TR.Default["Cancel", "Cancel"];
    private IContainer components;
    private Button btConnect;
    private Panel panelCenter;
    private Label lblMessage;
    private Label lblServerDescription;
    private Label lblServerName;
    private PictureBox connectionAnimation;

    public RemoteConnectionView(
      MainView view,
      ComicLibraryClient client,
      MainView.AddRemoteLibraryOptions options)
    {
      this.InitializeComponent();
      this.View = view;
      this.Client = client;
      this.openNow = options.HasFlag((Enum) MainView.AddRemoteLibraryOptions.Open);
      this.autoOpen = this.openNow && options.HasFlag((Enum) MainView.AddRemoteLibraryOptions.Auto);
      this.AutoScrollMinSize = this.panelCenter.Size;
      this.lblServerName.Text = client.ShareInformation.Name;
      this.lblServerDescription.Text = client.ShareInformation.Comment;
    }

    protected override void Dispose(bool disposing)
    {
      this.thread?.Abort();
      if (disposing)
      {
        if (this.oldImage != null)
          this.TabImage = this.oldImage;
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public MainView View { get; private set; }

    public ComicLibraryClient Client { get; private set; }

    private TabBar.TabBarItem Tab => this.Tag as TabBar.TabBarItem;

    private Image TabImage
    {
      get => this.Tab == null ? (Image) null : this.Tab.Image;
      set
      {
        if (this.Tab == null)
          return;
        this.Tab.Image = value;
      }
    }

    private void Connect()
    {
      this.cancelConnection = false;
      this.connectionAnimation.Visible = true;
      this.btConnect.Text = this.textCancel;
      this.oldImage = this.TabImage;
      this.TabImage = (Image) Resources.SmallBallAnimation;
      this.thread = ThreadUtility.RunInBackground("Connect to remote library", new ThreadStart(this.ConnectToLibrary), ThreadPriority.Normal);
    }

    private void ConnectToLibrary()
    {
      try
      {
        ComicLibrary cl = (ComicLibrary) null;
        bool firstTime = true;
        if (this.Client.ShareInformation.IsProtected)
        {
          string passwordFromCache = Program.Settings.GetPasswordFromCache(this.Client.ShareInformation.Name);
          if (!string.IsNullOrEmpty(passwordFromCache))
            this.Client.Password = passwordFromCache;
        }
        this.SetMessage(TR.Messages["ConnectServerText", "Opening connection to the remote Server"]);
        while (!this.cancelConnection && !this.Client.Connect() && !this.cancelConnection)
        {
          if (this.autoOpen)
          {
            this.cancelConnection = true;
            this.autoOpen = false;
          }
          else
            ControlExtensions.Invoke(this, (Action) (() =>
            {
              using (PasswordDialog passwordDialog = new PasswordDialog())
              {
                if (firstTime)
                  passwordDialog.Description = StringUtility.Format(TR.Messages["PasswordNeeded", "A password is needed for the remote Library '{0}':"], (object) this.Client.ShareInformation.Name);
                else
                  passwordDialog.Description = StringUtility.Format(TR.Messages["WrongPassword", "The specified password for the Library'{0}' is not correct. Please try again:"], (object) this.Client.ShareInformation.Name);
                if (passwordDialog.ShowDialog((IWin32Window) this) == DialogResult.Cancel)
                {
                  this.cancelConnection = true;
                }
                else
                {
                  this.Client.Password = Password.CreateHash(passwordDialog.Password);
                  if (!passwordDialog.RememberPassword)
                    return;
                  Program.Settings.AddPasswordToCache(this.Client.ShareInformation.Name, this.Client.Password);
                }
              }
            }));
          firstTime = false;
        }
        if (this.cancelConnection)
          throw new Exception();
        this.SetMessage(TR.Messages["GetServerLibraryText", "Retrieving the shared Library from the Server"]);
        cl = this.Client.GetRemoteLibrary();
        if (cl == null || this.cancelConnection)
          throw new Exception();
        this.InvokeAction((Action) (() =>
        {
          ComicListLibraryBrowser cllb = new ComicListLibraryBrowser(cl);
          TabBar.TabBarItem tag = this.Tag as TabBar.TabBarItem;
          ComicExplorerView ev = this.View.AddExplorerView(cl, (ComicListBrowser) cllb, tag, Program.Settings.GetRemoteExplorerViewSetting(cl.Id));
          ev.Main = this.Main;
          ev.Tag = (object) this.Client.ShareInformation.Uri;
          cllb.RefreshLists += new EventHandler(this.View.OnRefreshRemoteLists);
          cllb.LibraryChanged += (EventHandler) ((s, e) => ev.ComicBrowser.Library = cllb.Library);
          cllb.Tag = (object) this.Client;
          ev.ComicBrowser.ComicEditMode = cl.EditMode;
          this.View.RefreshView();
          this.Dispose();
        }));
      }
      catch (Exception ex)
      {
        this.OnConnectionCancelled(TR.Messages["FailedRetrieveDatabase", "Failed to retrieve the Database from the Server!"]);
      }
    }

    private void InvokeAction(Action action) => this.BeginInvoke((Delegate) action);

    private void SetMessage(string text)
    {
      this.InvokeAction((Action) (() =>
      {
        this.lblMessage.Text = text;
        this.lblMessage.Visible = true;
      }));
    }

    private void OnConnectionCancelled(string message = null)
    {
      if (this.cancelConnection)
        message = (string) null;
      this.InvokeAction((Action) (() =>
      {
        this.lblMessage.Text = message;
        this.lblMessage.Visible = message != null;
        this.connectionAnimation.Visible = false;
        this.TabImage = this.oldImage;
        this.btConnect.Text = this.textConnect;
        this.thread = (Thread) null;
      }));
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (this.panelCenter == null)
        return;
      Rectangle clientRectangle;
      if (this.ClientRectangle.Height > this.panelCenter.Height)
      {
        Panel panelCenter = this.panelCenter;
        clientRectangle = this.ClientRectangle;
        int num = (clientRectangle.Height - this.panelCenter.Height) / 2;
        panelCenter.Top = num;
      }
      clientRectangle = this.ClientRectangle;
      if (clientRectangle.Width <= this.panelCenter.Width)
        return;
      Panel panelCenter1 = this.panelCenter;
      clientRectangle = this.ClientRectangle;
      int num1 = (clientRectangle.Width - this.panelCenter.Width) / 2;
      panelCenter1.Left = num1;
    }

    protected override void OnCreateControl()
    {
      base.OnCreateControl();
      if (!this.openNow)
        return;
      this.InvokeAction(new Action(this.Connect));
    }

    private void btConnect_Click(object sender, EventArgs e)
    {
      Thread thread = this.thread;
      if (thread == null)
      {
        this.Connect();
      }
      else
      {
        this.TabImage = this.oldImage;
        this.connectionAnimation.Visible = false;
        this.Update();
        this.cancelConnection = true;
        using (new WaitCursor((Control) this))
        {
          if (thread.Join(2000))
            return;
          thread.Abort();
          thread.Join();
        }
      }
    }

    private void InitializeComponent()
    {
      this.btConnect = new Button();
      this.panelCenter = new Panel();
      this.connectionAnimation = new PictureBox();
      this.lblMessage = new Label();
      this.lblServerDescription = new Label();
      this.lblServerName = new Label();
      this.panelCenter.SuspendLayout();
      ((ISupportInitialize) this.connectionAnimation).BeginInit();
      this.SuspendLayout();
      this.btConnect.Location = new System.Drawing.Point(80, 161);
      this.btConnect.Name = "btConnect";
      this.btConnect.Size = new System.Drawing.Size(163, 31);
      this.btConnect.TabIndex = 0;
      this.btConnect.Text = "Connect";
      this.btConnect.UseVisualStyleBackColor = true;
      this.btConnect.Click += new EventHandler(this.btConnect_Click);
      this.panelCenter.Controls.Add((Control) this.connectionAnimation);
      this.panelCenter.Controls.Add((Control) this.lblMessage);
      this.panelCenter.Controls.Add((Control) this.lblServerDescription);
      this.panelCenter.Controls.Add((Control) this.lblServerName);
      this.panelCenter.Controls.Add((Control) this.btConnect);
      this.panelCenter.Location = new System.Drawing.Point(16, 3);
      this.panelCenter.Name = "panelCenter";
      this.panelCenter.Size = new System.Drawing.Size(323, 195);
      this.panelCenter.TabIndex = 1;
      this.connectionAnimation.Image = (Image) Resources.BigBallAnimation;
      this.connectionAnimation.Location = new System.Drawing.Point(134, 57);
      this.connectionAnimation.Name = "connectionAnimation";
      this.connectionAnimation.Size = new System.Drawing.Size(54, 55);
      this.connectionAnimation.SizeMode = PictureBoxSizeMode.AutoSize;
      this.connectionAnimation.TabIndex = 4;
      this.connectionAnimation.TabStop = false;
      this.connectionAnimation.Visible = false;
      this.lblMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.lblMessage.Location = new System.Drawing.Point(3, (int) sbyte.MaxValue);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(314, 19);
      this.lblMessage.TabIndex = 3;
      this.lblMessage.Text = "Process Message";
      this.lblMessage.TextAlign = ContentAlignment.MiddleCenter;
      this.lblMessage.Visible = false;
      this.lblServerDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.lblServerDescription.Location = new System.Drawing.Point(3, 20);
      this.lblServerDescription.Name = "lblServerDescription";
      this.lblServerDescription.Size = new System.Drawing.Size(314, 19);
      this.lblServerDescription.TabIndex = 2;
      this.lblServerDescription.Text = "Server Description";
      this.lblServerDescription.TextAlign = ContentAlignment.MiddleCenter;
      this.lblServerName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.lblServerName.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.lblServerName.Location = new System.Drawing.Point(3, 0);
      this.lblServerName.Name = "lblServerName";
      this.lblServerName.Size = new System.Drawing.Size(317, 20);
      this.lblServerName.TabIndex = 1;
      this.lblServerName.Text = "Server Name";
      this.lblServerName.TextAlign = ContentAlignment.MiddleCenter;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = SystemColors.Window;
      this.Controls.Add((Control) this.panelCenter);
      this.Name = nameof (RemoteConnectionView);
      this.Size = new System.Drawing.Size(356, 212);
      this.panelCenter.ResumeLayout(false);
      this.panelCenter.PerformLayout();
      ((ISupportInitialize) this.connectionAnimation).EndInit();
      this.ResumeLayout(false);
    }
  }
}

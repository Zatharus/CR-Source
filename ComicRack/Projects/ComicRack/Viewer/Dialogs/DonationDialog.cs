// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.DonationDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Cryptography;
using cYo.Common.Localize;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class DonationDialog : Form
  {
    public static TR Texts = TR.Load(nameof (DonationDialog));
    private static readonly Image validated = (Image) Resources.Validated;
    private static readonly Image notValidated = (Image) Resources.NotValidated;
    private IContainer components;
    private Button btOK;
    private Button btCancel;
    private Label labelDonationText;
    private PictureBox pictureBox1;
    private GroupBox groupDonate;
    private Label labelStepTwo;
    private Button btDonate;
    private Label labelStepOne;
    private Button btValidate;
    private TextBox textEmail;
    private Label labelEmail;
    private PictureBox validationIcon;
    private Label labelThankYou;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel panel1;
    private Panel panel2;
    private Timer timerValidation;

    public DonationDialog()
    {
      this.InitializeComponent();
      LocalizeUtility.Localize(DonationDialog.Texts, (Control) this);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.UpdateActivation();
      this.UpdateValidationButton();
    }

    public static string GetDateHash() => Password.CreateHash(DateTime.Now.ToShortDateString());

    public static string GetTriggerText()
    {
      string empty = string.Empty;
      if (Program.Settings.RunCount > 50)
        empty += string.Format(DonationDialog.Texts["TriggerRun", "You have started ComicRack {0} times!"] + "\n", (object) Program.Settings.RunCount);
      if (Program.Database.Books.Count > 100)
        empty += string.Format(DonationDialog.Texts["TriggerBooks", "You are managing {0} books with ComicRack!"] + "\n", (object) Program.Database.Books.Count);
      if (!string.IsNullOrEmpty(empty))
        empty += "\n";
      return empty;
    }

    public static bool IsTriggered()
    {
      if (string.IsNullOrEmpty(DonationDialog.GetTriggerText()))
        return false;
      return Program.Settings.DonationShown != DonationDialog.GetDateHash() || Program.Settings.RunCount > 200;
    }

    public static void Validate(string email)
    {
      try
      {
        using (new WaitCursor())
          Program.ValidateActivation(email);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(DonationDialog.Texts["ServerError", "Could not contact the ComicRack server to validate the Email address. Please check your internet connection!"], Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    public static void Show(IWin32Window parent, bool alwaysShow)
    {
      if (!alwaysShow && (!DonationDialog.IsTriggered() || Program.Settings.IsActivated))
        return;
      using (DonationDialog donationDialog = new DonationDialog())
      {
        Program.Settings.DonationShown = DonationDialog.GetDateHash();
        donationDialog.labelDonationText.Text = DonationDialog.GetTriggerText() + donationDialog.labelDonationText.Text;
        donationDialog.textEmail.Text = Program.Settings.UserEmail;
        int num = (int) donationDialog.ShowDialog(parent);
      }
    }

    private void UpdateActivation()
    {
      bool isActivated = Program.Settings.IsActivated;
      this.validationIcon.Image = isActivated ? DonationDialog.validated : DonationDialog.notValidated;
      this.btOK.Enabled = isActivated;
      this.btCancel.Visible = !isActivated;
      this.labelThankYou.Visible = isActivated;
    }

    private void UpdateValidationButton()
    {
      this.btValidate.Enabled = !string.IsNullOrEmpty(this.textEmail.Text.Trim());
    }

    private void textEmail_TextChanged(object sender, EventArgs e) => this.UpdateValidationButton();

    private void btDonate_Click(object sender, EventArgs e) => Program.ShowPayPal();

    private void btValidate_Click(object sender, EventArgs e)
    {
      this.btValidate.Enabled = false;
      DonationDialog.Validate(this.textEmail.Text.Trim());
      this.UpdateActivation();
      this.timerValidation.Start();
    }

    private void timerValidation_Tick(object sender, EventArgs e)
    {
      this.timerValidation.Stop();
      this.UpdateValidationButton();
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
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (DonationDialog));
      this.btOK = new Button();
      this.btCancel = new Button();
      this.labelDonationText = new Label();
      this.pictureBox1 = new PictureBox();
      this.groupDonate = new GroupBox();
      this.validationIcon = new PictureBox();
      this.btValidate = new Button();
      this.textEmail = new TextBox();
      this.labelEmail = new Label();
      this.labelStepTwo = new Label();
      this.btDonate = new Button();
      this.labelStepOne = new Label();
      this.labelThankYou = new Label();
      this.flowLayoutPanel1 = new FlowLayoutPanel();
      this.panel1 = new Panel();
      this.panel2 = new Panel();
      this.timerValidation = new Timer(this.components);
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.groupDonate.SuspendLayout();
      ((ISupportInitialize) this.validationIcon).BeginInit();
      this.flowLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(104, 9);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(95, 24);
      this.btOK.TabIndex = 1;
      this.btOK.Text = "&OK";
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(3, 9);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(95, 24);
      this.btCancel.TabIndex = 0;
      this.btCancel.Text = "Skip for now";
      this.labelDonationText.AutoSize = true;
      this.labelDonationText.Location = new System.Drawing.Point(149, 0);
      this.labelDonationText.MaximumSize = new System.Drawing.Size(405, 0);
      this.labelDonationText.Name = "labelDonationText";
      this.labelDonationText.Padding = new Padding(4);
      this.labelDonationText.Size = new System.Drawing.Size(404, 125);
      this.labelDonationText.TabIndex = 0;
      this.labelDonationText.Text = componentResourceManager.GetString("labelDonationText.Text");
      this.pictureBox1.Image = (Image) Resources.ComicRackApp256;
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.MinimumSize = new System.Drawing.Size(142, 142);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(142, 142);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 8;
      this.pictureBox1.TabStop = false;
      this.groupDonate.Controls.Add((Control) this.validationIcon);
      this.groupDonate.Controls.Add((Control) this.btValidate);
      this.groupDonate.Controls.Add((Control) this.textEmail);
      this.groupDonate.Controls.Add((Control) this.labelEmail);
      this.groupDonate.Controls.Add((Control) this.labelStepTwo);
      this.groupDonate.Controls.Add((Control) this.btDonate);
      this.groupDonate.Controls.Add((Control) this.labelStepOne);
      this.groupDonate.Location = new System.Drawing.Point(3, 154);
      this.groupDonate.Name = "groupDonate";
      this.groupDonate.Size = new System.Drawing.Size(558, 194);
      this.groupDonate.TabIndex = 1;
      this.groupDonate.TabStop = false;
      this.groupDonate.Text = "Donate";
      this.validationIcon.Location = new System.Drawing.Point(360, 153);
      this.validationIcon.Name = "validationIcon";
      this.validationIcon.Size = new System.Drawing.Size(21, 20);
      this.validationIcon.TabIndex = 6;
      this.validationIcon.TabStop = false;
      this.btValidate.Location = new System.Drawing.Point(424, 150);
      this.btValidate.Name = "btValidate";
      this.btValidate.Size = new System.Drawing.Size(111, 23);
      this.btValidate.TabIndex = 5;
      this.btValidate.Text = "Validate";
      this.btValidate.UseVisualStyleBackColor = true;
      this.btValidate.Click += new EventHandler(this.btValidate_Click);
      this.textEmail.Location = new System.Drawing.Point(149, 153);
      this.textEmail.Name = "textEmail";
      this.textEmail.Size = new System.Drawing.Size(205, 20);
      this.textEmail.TabIndex = 4;
      this.textEmail.TextChanged += new EventHandler(this.textEmail_TextChanged);
      this.labelEmail.AutoSize = true;
      this.labelEmail.Location = new System.Drawing.Point(67, 156);
      this.labelEmail.Name = "labelEmail";
      this.labelEmail.Size = new System.Drawing.Size(76, 13);
      this.labelEmail.TabIndex = 3;
      this.labelEmail.Text = "Email Address:";
      this.labelStepTwo.Location = new System.Drawing.Point(23, 86);
      this.labelStepTwo.Name = "labelStepTwo";
      this.labelStepTwo.Size = new System.Drawing.Size(461, 57);
      this.labelStepTwo.TabIndex = 2;
      this.labelStepTwo.Text = "Step Two:\r\nEnter your donation email and validate it.\r\nIt may take a few minutes for your donation to be registered so you can do this step a little later.";
      this.btDonate.Location = new System.Drawing.Point(424, 34);
      this.btDonate.Name = "btDonate";
      this.btDonate.Size = new System.Drawing.Size(111, 23);
      this.btDonate.TabIndex = 1;
      this.btDonate.Text = "Donation Page";
      this.btDonate.UseVisualStyleBackColor = true;
      this.btDonate.Click += new EventHandler(this.btDonate_Click);
      this.labelStepOne.AutoSize = true;
      this.labelStepOne.Location = new System.Drawing.Point(23, 31);
      this.labelStepOne.Name = "labelStepOne";
      this.labelStepOne.Size = new System.Drawing.Size(188, 26);
      this.labelStepOne.TabIndex = 0;
      this.labelStepOne.Text = "Step One:\r\nVisit ComicRack on the web to donate";
      this.labelThankYou.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.labelThankYou.Location = new System.Drawing.Point(3, 351);
      this.labelThankYou.Name = "labelThankYou";
      this.labelThankYou.Size = new System.Drawing.Size(558, 32);
      this.labelThankYou.TabIndex = 2;
      this.labelThankYou.Text = "Thank you for supporting ComicRack!";
      this.labelThankYou.TextAlign = ContentAlignment.MiddleCenter;
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add((Control) this.panel1);
      this.flowLayoutPanel1.Controls.Add((Control) this.groupDonate);
      this.flowLayoutPanel1.Controls.Add((Control) this.labelThankYou);
      this.flowLayoutPanel1.Controls.Add((Control) this.panel2);
      this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 3);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(564, 425);
      this.flowLayoutPanel1.TabIndex = 0;
      this.panel1.AutoSize = true;
      this.panel1.Controls.Add((Control) this.labelDonationText);
      this.panel1.Controls.Add((Control) this.pictureBox1);
      this.panel1.Dock = DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(3, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(558, 145);
      this.panel1.TabIndex = 0;
      this.panel2.Anchor = AnchorStyles.Right;
      this.panel2.AutoSize = true;
      this.panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panel2.Controls.Add((Control) this.btOK);
      this.panel2.Controls.Add((Control) this.btCancel);
      this.panel2.Location = new System.Drawing.Point(359, 386);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(202, 36);
      this.panel2.TabIndex = 3;
      this.timerValidation.Interval = 5000;
      this.timerValidation.Tick += new EventHandler(this.timerValidation_Tick);
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.ClientSize = new System.Drawing.Size(571, 431);
      this.Controls.Add((Control) this.flowLayoutPanel1);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (DonationDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Support ComicRack...";
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.groupDonate.ResumeLayout(false);
      this.groupDonate.PerformLayout();
      ((ISupportInitialize) this.validationIcon).EndInit();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.QuestionDialog
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class QuestionDialog : Form
  {
    private IContainer components;
    private PictureBox iconBox;
    private Label lblQuestion;
    private Button btCancel;
    private Button btOK;
    private Label lblDescription;
    private FlowLayoutPanel flowPanel;
    private FlowLayoutPanel buttonFlow;
    private WrappingCheckBox chkOption;
    private PictureBox imageBox;
    private WrappingCheckBox chkOption2;

    public QuestionDialog()
    {
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.Text = Application.ProductName;
      this.iconBox.BackgroundImage = (Image) SystemIcons.Question.ToBitmap();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.iconBox.BackgroundImage != null)
          this.iconBox.BackgroundImage.Dispose();
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    public string Question { get; set; }

    public string OkButtonText { get; set; }

    public string CancelButtonText { get; set; }

    public string OptionText { get; set; }

    public string Option2Text { get; set; }

    public bool Option2Independent { get; set; }

    public Image Image { get; set; }

    public bool ShowCancel { get; set; }

    public QuestionResult Ask(IWin32Window owner)
    {
      string[] strArray = this.Question.Split('\n');
      this.imageBox.Image = this.Image;
      this.imageBox.Visible = this.Image != null;
      this.lblQuestion.Text = strArray[0];
      this.lblDescription.Text = string.Empty;
      for (int index = 1; index < strArray.Length; ++index)
      {
        Label lblDescription = this.lblDescription;
        lblDescription.Text = lblDescription.Text + (index > 1 ? "\n" : string.Empty) + strArray[index];
      }
      if (this.OkButtonText != null)
        this.btOK.Text = this.OkButtonText;
      if (this.CancelButtonText != null)
        this.btCancel.Text = this.CancelButtonText;
      this.lblDescription.Visible = strArray.Length > 1;
      if (!string.IsNullOrEmpty(this.OptionText))
      {
        this.chkOption.Checked = this.OptionText.StartsWith("!");
        if (this.chkOption.Checked)
          this.OptionText = this.OptionText.Substring(1);
      }
      if (!string.IsNullOrEmpty(this.Option2Text))
      {
        this.chkOption2.Checked = this.Option2Text.StartsWith("!");
        if (this.chkOption2.Checked)
          this.Option2Text = this.Option2Text.Substring(1);
      }
      this.chkOption.Text = this.OptionText;
      this.chkOption.Visible = !string.IsNullOrEmpty(this.OptionText);
      this.chkOption2.Text = this.Option2Text;
      this.chkOption2.Visible = (this.Option2Independent || this.chkOption.Checked) && !string.IsNullOrEmpty(this.Option2Text);
      if (!this.Option2Independent)
        this.chkOption2.Margin = new Padding(32, 0, 0, 0);
      this.btCancel.Visible = this.ShowCancel;
      if (owner == null)
        this.StartPosition = FormStartPosition.CenterScreen;
      if (this.ShowDialog(owner) != DialogResult.OK)
        return QuestionResult.Cancel;
      QuestionResult questionResult = QuestionResult.Ok;
      if (this.chkOption.Checked)
        questionResult |= QuestionResult.Option;
      if (this.chkOption2.Checked)
        questionResult |= QuestionResult.Option2;
      return questionResult;
    }

    public static QuestionResult AskQuestion(
      IWin32Window owner,
      string question,
      string okText,
      string optionText = null,
      Image image = null,
      bool showCancel = true,
      string cancelText = null,
      string option2Text = null)
    {
      return QuestionDialog.AskQuestion(owner, question, okText, (Action<QuestionDialog>) (qd =>
      {
        qd.CancelButtonText = cancelText;
        qd.OptionText = optionText;
        qd.Option2Text = option2Text;
        qd.Image = image;
        qd.ShowCancel = showCancel;
      }));
    }

    public static QuestionResult AskQuestion(
      IWin32Window owner,
      string question,
      string okButtonText = null,
      Action<QuestionDialog> setParameters = null)
    {
      using (QuestionDialog questionDialog = new QuestionDialog())
      {
        questionDialog.Question = question;
        questionDialog.OkButtonText = okButtonText;
        if (setParameters != null)
          setParameters(questionDialog);
        return questionDialog.Ask(owner);
      }
    }

    public static bool Ask(IWin32Window owner, string question, string okButtonText)
    {
      return QuestionDialog.AskQuestion(owner, question, okButtonText, (Action<QuestionDialog>) null) == QuestionResult.Ok;
    }

    private void chkOption_CheckedChanged(object sender, EventArgs e)
    {
      if (this.Option2Independent)
        return;
      this.chkOption2.Visible = this.chkOption.Checked && !string.IsNullOrEmpty(this.chkOption2.Text);
    }

    private void InitializeComponent()
    {
      this.iconBox = new PictureBox();
      this.lblQuestion = new Label();
      this.btCancel = new Button();
      this.btOK = new Button();
      this.lblDescription = new Label();
      this.flowPanel = new FlowLayoutPanel();
      this.imageBox = new PictureBox();
      this.chkOption = new WrappingCheckBox();
      this.chkOption2 = new WrappingCheckBox();
      this.buttonFlow = new FlowLayoutPanel();
      ((ISupportInitialize) this.iconBox).BeginInit();
      this.flowPanel.SuspendLayout();
      ((ISupportInitialize) this.imageBox).BeginInit();
      this.buttonFlow.SuspendLayout();
      this.SuspendLayout();
      this.iconBox.BackgroundImageLayout = ImageLayout.Zoom;
      this.iconBox.Location = new System.Drawing.Point(12, 12);
      this.iconBox.Name = "iconBox";
      this.iconBox.Size = new System.Drawing.Size(37, 34);
      this.iconBox.TabIndex = 0;
      this.iconBox.TabStop = false;
      this.lblQuestion.AutoSize = true;
      this.lblQuestion.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.lblQuestion.Location = new System.Drawing.Point(0, 8);
      this.lblQuestion.Margin = new Padding(0, 8, 0, 8);
      this.lblQuestion.MinimumSize = new System.Drawing.Size(300, 0);
      this.lblQuestion.Name = "lblQuestion";
      this.lblQuestion.Size = new System.Drawing.Size(300, 13);
      this.lblQuestion.TabIndex = 1;
      this.lblQuestion.Text = "Question";
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(89, 3);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 6;
      this.btCancel.Text = "&Cancel";
      this.btOK.AutoSize = true;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(3, 3);
      this.btOK.MinimumSize = new System.Drawing.Size(80, 0);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 5;
      this.btOK.Text = "&OK";
      this.lblDescription.AutoSize = true;
      this.lblDescription.Location = new System.Drawing.Point(0, 76);
      this.lblDescription.Margin = new Padding(0);
      this.lblDescription.Name = "lblDescription";
      this.lblDescription.Size = new System.Drawing.Size(60, 13);
      this.lblDescription.TabIndex = 7;
      this.lblDescription.Text = "Description";
      this.flowPanel.AutoSize = true;
      this.flowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.flowPanel.Controls.Add((Control) this.lblQuestion);
      this.flowPanel.Controls.Add((Control) this.imageBox);
      this.flowPanel.Controls.Add((Control) this.lblDescription);
      this.flowPanel.Controls.Add((Control) this.chkOption);
      this.flowPanel.Controls.Add((Control) this.chkOption2);
      this.flowPanel.Controls.Add((Control) this.buttonFlow);
      this.flowPanel.FlowDirection = FlowDirection.TopDown;
      this.flowPanel.Location = new System.Drawing.Point(55, 12);
      this.flowPanel.MaximumSize = new System.Drawing.Size(350, 0);
      this.flowPanel.Name = "flowPanel";
      this.flowPanel.Size = new System.Drawing.Size(300, 169);
      this.flowPanel.TabIndex = 8;
      this.imageBox.Anchor = AnchorStyles.None;
      this.imageBox.Location = new System.Drawing.Point(134, 29);
      this.imageBox.Margin = new Padding(3, 0, 3, 8);
      this.imageBox.Name = "imageBox";
      this.imageBox.Size = new System.Drawing.Size(31, 39);
      this.imageBox.SizeMode = PictureBoxSizeMode.AutoSize;
      this.imageBox.TabIndex = 9;
      this.imageBox.TabStop = false;
      this.chkOption.Anchor = AnchorStyles.Left;
      this.chkOption.AutoSize = true;
      this.chkOption.CheckAlign = ContentAlignment.TopLeft;
      this.chkOption.Location = new System.Drawing.Point(0, 97);
      this.chkOption.Margin = new Padding(0, 8, 0, 0);
      this.chkOption.Name = "chkOption";
      this.chkOption.Size = new System.Drawing.Size(116, 17);
      this.chkOption.TabIndex = 10;
      this.chkOption.Text = "Optional Checkbox";
      this.chkOption.TextAlign = ContentAlignment.TopLeft;
      this.chkOption.UseVisualStyleBackColor = true;
      this.chkOption.Visible = false;
      this.chkOption.CheckedChanged += new EventHandler(this.chkOption_CheckedChanged);
      this.chkOption2.Anchor = AnchorStyles.Left;
      this.chkOption2.AutoSize = true;
      this.chkOption2.CheckAlign = ContentAlignment.TopLeft;
      this.chkOption2.Location = new System.Drawing.Point(0, 114);
      this.chkOption2.Margin = new Padding(0);
      this.chkOption2.Name = "chkOption2";
      this.chkOption2.Size = new System.Drawing.Size(125, 17);
      this.chkOption2.TabIndex = 11;
      this.chkOption2.Text = "Optional Checkbox 2";
      this.chkOption2.TextAlign = ContentAlignment.TopLeft;
      this.chkOption2.UseVisualStyleBackColor = true;
      this.chkOption2.Visible = false;
      this.buttonFlow.Anchor = AnchorStyles.Right;
      this.buttonFlow.AutoSize = true;
      this.buttonFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.buttonFlow.Controls.Add((Control) this.btOK);
      this.buttonFlow.Controls.Add((Control) this.btCancel);
      this.buttonFlow.Location = new System.Drawing.Point(128, 139);
      this.buttonFlow.Margin = new Padding(0, 8, 0, 0);
      this.buttonFlow.Name = "buttonFlow";
      this.buttonFlow.Size = new System.Drawing.Size(172, 30);
      this.buttonFlow.TabIndex = 9;
      this.buttonFlow.WrapContents = false;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(369, 198);
      this.Controls.Add((Control) this.flowPanel);
      this.Controls.Add((Control) this.iconBox);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (QuestionDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = nameof (QuestionDialog);
      ((ISupportInitialize) this.iconBox).EndInit();
      this.flowPanel.ResumeLayout(false);
      this.flowPanel.PerformLayout();
      ((ISupportInitialize) this.imageBox).EndInit();
      this.buttonFlow.ResumeLayout(false);
      this.buttonFlow.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

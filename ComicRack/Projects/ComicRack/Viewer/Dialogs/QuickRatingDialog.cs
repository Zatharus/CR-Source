// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.QuickRatingDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Controls;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class QuickRatingDialog : Form
  {
    private IContainer components;
    private RatingControl txRating;
    private ThumbnailControl coverThumbnail;
    private TextBoxEx txReview;
    private Button btCancel;
    private Button btOK;
    private CheckBox chkTweet;
    private CheckBox chkShow;

    public QuickRatingDialog()
    {
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
    }

    private static void SetThumbnailImage(IBitmapDisplayControl iv, ComicBook cb, int page)
    {
      try
      {
        ThumbnailKey key = cb.GetThumbnailKey(page);
        iv.Tag = (object) key;
        using (IItemLock<ThumbnailImage> thumbnail1 = Program.ImagePool.GetThumbnail(key, true))
        {
          if (thumbnail1 != null)
            iv.SetBitmap(thumbnail1.Item.Bitmap.CreateCopy());
          else
            Program.ImagePool.SlowThumbnailQueue.AddItem((ImageKey) key, (object) iv, (AsyncCallback) (ar =>
            {
              try
              {
                using (IItemLock<ThumbnailImage> thumbnail2 = Program.ImagePool.GetThumbnail(key, cb))
                {
                  if (!object.Equals((object) key, iv.Tag))
                    return;
                  iv.SetBitmap(thumbnail2.Item.Bitmap.CreateCopy());
                }
              }
              catch
              {
              }
            }));
        }
      }
      catch
      {
      }
    }

    public static bool Show(IWin32Window parent, ComicBook book)
    {
      if (book == null)
        return false;
      using (QuickRatingDialog quickRatingDialog1 = new QuickRatingDialog())
      {
        QuickRatingDialog quickRatingDialog2 = quickRatingDialog1;
        quickRatingDialog2.Text = quickRatingDialog2.Text + " - " + book.CaptionWithoutTitle;
        quickRatingDialog1.txReview.Text = book.Review;
        quickRatingDialog1.txRating.Rating = book.Rating;
        quickRatingDialog1.chkTweet.Checked = Program.Settings.TweetQuickReview;
        quickRatingDialog1.chkShow.Checked = Program.Settings.AutoShowQuickReview;
        QuickRatingDialog.SetThumbnailImage((IBitmapDisplayControl) quickRatingDialog1.coverThumbnail, book, book.FrontCoverPageIndex);
        bool flag = quickRatingDialog1.ShowDialog(parent) == DialogResult.OK;
        if (flag)
        {
          book.Review = quickRatingDialog1.txReview.Text;
          book.Rating = quickRatingDialog1.txRating.Rating;
          Program.Settings.AutoShowQuickReview = quickRatingDialog1.chkShow.Checked;
          if (Program.Settings.TweetQuickReview = quickRatingDialog1.chkTweet.Checked)
            Twitter.Tweet(parent, book, quickRatingDialog1.coverThumbnail.Bitmap);
        }
        return flag;
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
      this.txRating = new RatingControl();
      this.coverThumbnail = new ThumbnailControl();
      this.txReview = new TextBoxEx();
      this.btCancel = new Button();
      this.btOK = new Button();
      this.chkTweet = new CheckBox();
      this.chkShow = new CheckBox();
      this.SuspendLayout();
      this.txRating.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.txRating.DrawText = true;
      this.txRating.Font = new Font("Arial", 9f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.txRating.ForeColor = SystemColors.GrayText;
      this.txRating.Location = new System.Drawing.Point(12, 245);
      this.txRating.Name = "txRating";
      this.txRating.Rating = 3f;
      this.txRating.RatingImage = (Image) Resources.StarYellow;
      this.txRating.Size = new System.Drawing.Size(170, 21);
      this.txRating.TabIndex = 2;
      this.txRating.Text = "3";
      this.coverThumbnail.AllowDrop = true;
      this.coverThumbnail.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      this.coverThumbnail.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.coverThumbnail.Location = new System.Drawing.Point(12, 12);
      this.coverThumbnail.Name = "coverThumbnail";
      this.coverThumbnail.Size = new System.Drawing.Size(170, 227);
      this.coverThumbnail.TabIndex = 1;
      this.coverThumbnail.TextElements = ComicTextElements.None;
      this.coverThumbnail.ThreeD = true;
      this.txReview.AcceptsReturn = true;
      this.txReview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.txReview.FocusSelect = false;
      this.txReview.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.txReview.Location = new System.Drawing.Point(188, 12);
      this.txReview.Multiline = true;
      this.txReview.Name = "txReview";
      this.txReview.ScrollBars = ScrollBars.Vertical;
      this.txReview.Size = new System.Drawing.Size(369, 254);
      this.txReview.TabIndex = 0;
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(477, 272);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 6;
      this.btCancel.Text = "&Cancel";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(391, 272);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 5;
      this.btOK.Text = "&OK";
      this.chkTweet.AutoSize = true;
      this.chkTweet.Location = new System.Drawing.Point(188, 277);
      this.chkTweet.Name = "chkTweet";
      this.chkTweet.Size = new System.Drawing.Size(56, 17);
      this.chkTweet.TabIndex = 4;
      this.chkTweet.Text = "Tweet";
      this.chkTweet.UseVisualStyleBackColor = true;
      this.chkShow.AutoSize = true;
      this.chkShow.Location = new System.Drawing.Point(12, 277);
      this.chkShow.Name = "chkShow";
      this.chkShow.Size = new System.Drawing.Size(134, 17);
      this.chkShow.TabIndex = 3;
      this.chkShow.Text = "Show when Book read";
      this.chkShow.UseVisualStyleBackColor = true;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(564, 304);
      this.Controls.Add((Control) this.chkShow);
      this.Controls.Add((Control) this.chkTweet);
      this.Controls.Add((Control) this.btCancel);
      this.Controls.Add((Control) this.btOK);
      this.Controls.Add((Control) this.txReview);
      this.Controls.Add((Control) this.coverThumbnail);
      this.Controls.Add((Control) this.txRating);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (QuickRatingDialog);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Quick Rating";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

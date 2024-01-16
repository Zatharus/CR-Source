// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Dialogs.ValueEditorDialog
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Viewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Dialogs
{
  public class ValueEditorDialog : Form
  {
    private IContainer components;
    private Panel panelMatchValue;
    private Panel panel1;
    private RichTextBox rtfMatchValue;
    private Button btOK;
    private Button btCancel;
    private Button btInsertValue;

    public ValueEditorDialog()
    {
      this.InitializeComponent();
      this.btInsertValue.Image = (Image) ((Bitmap) this.btInsertValue.Image).ScaleDpi();
      LocalizeUtility.Localize((Control) this, (IContainer) null);
      this.CreateValueContextMenu();
    }

    public void SyntaxColoring(IEnumerable<ValuePair<Color, Regex>> colors)
    {
      this.rtfMatchValue.RegisterColorize(colors);
    }

    public string MatchValue
    {
      get => this.rtfMatchValue.Text;
      set => this.rtfMatchValue.Text = value;
    }

    private void AddField(object sender, EventArgs e)
    {
      this.rtfMatchValue.SelectedText = "{" + (string) (sender as ToolStripMenuItem).Tag + "}";
      this.rtfMatchValue.Focus();
    }

    private void CreateValueContextMenu()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ContextMenuBuilder contextMenuBuilder = new ContextMenuBuilder();
      foreach (string str in ((IEnumerable<string>) ComicBookMatcher.ComicProperties).Concat<string>((IEnumerable<string>) ComicBookMatcher.SeriesStatsProperties))
        contextMenuBuilder.Add(str, false, false, new EventHandler(this.AddField), (object) str, DateTime.MinValue);
      ContextMenuStrip cm = new ContextMenuStrip(this.components);
      cm.Items.AddRange(contextMenuBuilder.Create(20));
      this.btInsertValue.Click += (EventHandler) ((s, e) => cm.Show((Control) this.btInsertValue, 0, this.btInsertValue.Height));
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.panelMatchValue = new Panel();
      this.panel1 = new Panel();
      this.rtfMatchValue = new RichTextBox();
      this.btOK = new Button();
      this.btCancel = new Button();
      this.btInsertValue = new Button();
      this.panelMatchValue.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.panelMatchValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.panelMatchValue.BorderStyle = BorderStyle.FixedSingle;
      this.panelMatchValue.Controls.Add((Control) this.panel1);
      this.panelMatchValue.Location = new System.Drawing.Point(2, 3);
      this.panelMatchValue.Name = "panelMatchValue";
      this.panelMatchValue.Size = new System.Drawing.Size(375, 76);
      this.panelMatchValue.TabIndex = 4;
      this.panel1.BackColor = SystemColors.Window;
      this.panel1.Controls.Add((Control) this.rtfMatchValue);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new Padding(1, 2, 1, 2);
      this.panel1.Size = new System.Drawing.Size(373, 74);
      this.panel1.TabIndex = 0;
      this.rtfMatchValue.BorderStyle = BorderStyle.None;
      this.rtfMatchValue.Dock = DockStyle.Fill;
      this.rtfMatchValue.Location = new System.Drawing.Point(1, 2);
      this.rtfMatchValue.Multiline = false;
      this.rtfMatchValue.Name = "rtfMatchValue";
      this.rtfMatchValue.Size = new System.Drawing.Size(371, 70);
      this.rtfMatchValue.TabIndex = 0;
      this.rtfMatchValue.Text = "";
      this.btOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btOK.DialogResult = DialogResult.OK;
      this.btOK.FlatStyle = FlatStyle.System;
      this.btOK.Location = new System.Drawing.Point(211, 85);
      this.btOK.Name = "btOK";
      this.btOK.Size = new System.Drawing.Size(80, 24);
      this.btOK.TabIndex = 1;
      this.btOK.Text = "&OK";
      this.btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btCancel.DialogResult = DialogResult.Cancel;
      this.btCancel.FlatStyle = FlatStyle.System;
      this.btCancel.Location = new System.Drawing.Point(297, 85);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(80, 24);
      this.btCancel.TabIndex = 2;
      this.btCancel.Text = "&Cancel";
      this.btInsertValue.Image = (Image) Resources.SmallArrowDown;
      this.btInsertValue.ImageAlign = ContentAlignment.MiddleRight;
      this.btInsertValue.Location = new System.Drawing.Point(4, 85);
      this.btInsertValue.Name = "btInsertValue";
      this.btInsertValue.Size = new System.Drawing.Size(113, 23);
      this.btInsertValue.TabIndex = 5;
      this.btInsertValue.Text = "Insert Value";
      this.btInsertValue.UseVisualStyleBackColor = true;
      this.AcceptButton = (IButtonControl) this.btOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.btCancel;
      this.ClientSize = new System.Drawing.Size(380, 112);
      this.Controls.Add((Control) this.btInsertValue);
      this.Controls.Add((Control) this.panelMatchValue);
      this.Controls.Add((Control) this.btOK);
      this.Controls.Add((Control) this.btCancel);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (ValueEditorDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Match Value";
      this.panelMatchValue.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

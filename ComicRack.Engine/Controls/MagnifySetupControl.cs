// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Controls.MagnifySetupControl
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine.Display;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Controls
{
  public class MagnifySetupControl : UserControl
  {
    private IContainer components;
    private TrackBarLite tbWidth;
    private Label labelWidth;
    private TrackBarLite tbHeight;
    private Label labelHeight;
    private TrackBarLite tbOpaque;
    private Label labelOpacity;
    private TrackBarLite tbZoom;
    private Label labelZoom;
    private CheckBox chkSimpleStyle;
    private CheckBox chkAutoHideMagnifier;
    private CheckBox chkAutoMagnifier;
    private GroupBox groupBox1;
    private GroupBox groupBox2;

    public MagnifySetupControl()
    {
      this.InitializeComponent();
      LocalizeUtility.Localize((Control) this, this.components);
    }

    public int MagnifyWidth
    {
      get => this.tbWidth.Value;
      set => this.tbWidth.Value = value;
    }

    public int MagnifyHeight
    {
      get => this.tbHeight.Value;
      set => this.tbHeight.Value = value;
    }

    public float MagnifyOpaque
    {
      get => (float) this.tbOpaque.Value / 100f;
      set => this.tbOpaque.Value = (int) ((double) value * 100.0);
    }

    public float MagnifyZoom
    {
      get => (float) this.tbZoom.Value / 100f;
      set => this.tbZoom.Value = (int) ((double) value * 100.0);
    }

    public System.Drawing.Size MagnifySize
    {
      get => new System.Drawing.Size(this.MagnifyWidth, this.MagnifyHeight);
      set
      {
        this.MagnifyWidth = value.Width;
        this.MagnifyHeight = value.Height;
      }
    }

    public MagnifierStyle MagnifyStyle
    {
      get => !this.chkSimpleStyle.Checked ? MagnifierStyle.Glass : MagnifierStyle.Simple;
      set => this.chkSimpleStyle.Checked = value == MagnifierStyle.Simple;
    }

    public bool AutoHideMagnifier
    {
      get => this.chkAutoHideMagnifier.Checked;
      set => this.chkAutoHideMagnifier.Checked = value;
    }

    public bool AutoMagnifier
    {
      get => this.chkAutoMagnifier.Checked;
      set => this.chkAutoMagnifier.Checked = value;
    }

    private void ControlValuesChanged(object sender, EventArgs e) => this.OnValuesChanged();

    private void OnValuesChanged()
    {
      if (this.ValuesChanged == null)
        return;
      this.ValuesChanged((object) this, EventArgs.Empty);
    }

    public event EventHandler ValuesChanged;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.tbWidth = new TrackBarLite();
      this.labelWidth = new Label();
      this.tbHeight = new TrackBarLite();
      this.labelHeight = new Label();
      this.tbOpaque = new TrackBarLite();
      this.labelOpacity = new Label();
      this.tbZoom = new TrackBarLite();
      this.labelZoom = new Label();
      this.chkSimpleStyle = new CheckBox();
      this.chkAutoHideMagnifier = new CheckBox();
      this.chkAutoMagnifier = new CheckBox();
      this.groupBox1 = new GroupBox();
      this.groupBox2 = new GroupBox();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      this.tbWidth.Location = new System.Drawing.Point(6, 19);
      this.tbWidth.Maximum = 512;
      this.tbWidth.Minimum = 64;
      this.tbWidth.Name = "tbWidth";
      this.tbWidth.Size = new System.Drawing.Size(106, 17);
      this.tbWidth.TabIndex = 1;
      this.tbWidth.Text = "Width";
      this.tbWidth.ThumbSize = new System.Drawing.Size(6, 12);
      this.tbWidth.TickFrequency = 32;
      this.tbWidth.TickStyle = TickStyle.BottomRight;
      this.tbWidth.Value = 64;
      this.tbWidth.Scroll += new EventHandler(this.ControlValuesChanged);
      this.labelWidth.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelWidth.Location = new System.Drawing.Point(9, 39);
      this.labelWidth.Name = "labelWidth";
      this.labelWidth.Size = new System.Drawing.Size(103, 12);
      this.labelWidth.TabIndex = 0;
      this.labelWidth.Text = "Width";
      this.labelWidth.TextAlign = ContentAlignment.MiddleCenter;
      this.tbHeight.Location = new System.Drawing.Point(118, 19);
      this.tbHeight.Maximum = 512;
      this.tbHeight.Minimum = 64;
      this.tbHeight.Name = "tbHeight";
      this.tbHeight.Size = new System.Drawing.Size(106, 17);
      this.tbHeight.TabIndex = 3;
      this.tbHeight.Text = "Width";
      this.tbHeight.ThumbSize = new System.Drawing.Size(6, 12);
      this.tbHeight.TickFrequency = 32;
      this.tbHeight.TickStyle = TickStyle.BottomRight;
      this.tbHeight.Value = 64;
      this.tbHeight.Scroll += new EventHandler(this.ControlValuesChanged);
      this.labelHeight.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelHeight.Location = new System.Drawing.Point(118, 39);
      this.labelHeight.Name = "labelHeight";
      this.labelHeight.Size = new System.Drawing.Size(106, 12);
      this.labelHeight.TabIndex = 2;
      this.labelHeight.Text = "Height";
      this.labelHeight.TextAlign = ContentAlignment.MiddleCenter;
      this.tbOpaque.Location = new System.Drawing.Point(6, 56);
      this.tbOpaque.Minimum = 20;
      this.tbOpaque.Name = "tbOpaque";
      this.tbOpaque.Size = new System.Drawing.Size(106, 17);
      this.tbOpaque.TabIndex = 5;
      this.tbOpaque.Text = "Width";
      this.tbOpaque.ThumbSize = new System.Drawing.Size(6, 12);
      this.tbOpaque.TickFrequency = 5;
      this.tbOpaque.TickStyle = TickStyle.BottomRight;
      this.tbOpaque.Value = 20;
      this.tbOpaque.Scroll += new EventHandler(this.ControlValuesChanged);
      this.labelOpacity.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelOpacity.Location = new System.Drawing.Point(6, 76);
      this.labelOpacity.Name = "labelOpacity";
      this.labelOpacity.Size = new System.Drawing.Size(106, 12);
      this.labelOpacity.TabIndex = 4;
      this.labelOpacity.Text = "Opacity";
      this.labelOpacity.TextAlign = ContentAlignment.MiddleCenter;
      this.tbZoom.Location = new System.Drawing.Point(118, 56);
      this.tbZoom.Maximum = 500;
      this.tbZoom.Minimum = 100;
      this.tbZoom.Name = "tbZoom";
      this.tbZoom.Size = new System.Drawing.Size(106, 17);
      this.tbZoom.TabIndex = 7;
      this.tbZoom.Text = "Width";
      this.tbZoom.ThumbSize = new System.Drawing.Size(6, 12);
      this.tbZoom.TickFrequency = 25;
      this.tbZoom.TickStyle = TickStyle.BottomRight;
      this.tbZoom.Value = 100;
      this.tbZoom.Scroll += new EventHandler(this.ControlValuesChanged);
      this.labelZoom.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.labelZoom.Location = new System.Drawing.Point(120, 76);
      this.labelZoom.Name = "labelZoom";
      this.labelZoom.Size = new System.Drawing.Size(104, 12);
      this.labelZoom.TabIndex = 6;
      this.labelZoom.Text = "Zoom";
      this.labelZoom.TextAlign = ContentAlignment.MiddleCenter;
      this.chkSimpleStyle.AutoSize = true;
      this.chkSimpleStyle.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.chkSimpleStyle.Location = new System.Drawing.Point(8, 19);
      this.chkSimpleStyle.Name = "chkSimpleStyle";
      this.chkSimpleStyle.Size = new System.Drawing.Size(87, 16);
      this.chkSimpleStyle.TabIndex = 8;
      this.chkSimpleStyle.Text = "Simple Style";
      this.chkSimpleStyle.UseVisualStyleBackColor = true;
      this.chkSimpleStyle.CheckedChanged += new EventHandler(this.ControlValuesChanged);
      this.chkAutoHideMagnifier.AutoSize = true;
      this.chkAutoHideMagnifier.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.chkAutoHideMagnifier.Location = new System.Drawing.Point(8, 41);
      this.chkAutoHideMagnifier.Name = "chkAutoHideMagnifier";
      this.chkAutoHideMagnifier.Size = new System.Drawing.Size(124, 16);
      this.chkAutoHideMagnifier.TabIndex = 9;
      this.chkAutoHideMagnifier.Text = "Hide at Page Border";
      this.chkAutoHideMagnifier.UseVisualStyleBackColor = true;
      this.chkAutoHideMagnifier.CheckedChanged += new EventHandler(this.ControlValuesChanged);
      this.chkAutoMagnifier.AutoSize = true;
      this.chkAutoMagnifier.Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Bold);
      this.chkAutoMagnifier.Location = new System.Drawing.Point(8, 63);
      this.chkAutoMagnifier.Name = "chkAutoMagnifier";
      this.chkAutoMagnifier.Size = new System.Drawing.Size(152, 16);
      this.chkAutoMagnifier.TabIndex = 10;
      this.chkAutoMagnifier.Text = "Activate with 'long' Click";
      this.chkAutoMagnifier.UseVisualStyleBackColor = true;
      this.chkAutoMagnifier.CheckedChanged += new EventHandler(this.ControlValuesChanged);
      this.groupBox1.Controls.Add((Control) this.chkSimpleStyle);
      this.groupBox1.Controls.Add((Control) this.chkAutoMagnifier);
      this.groupBox1.Controls.Add((Control) this.chkAutoHideMagnifier);
      this.groupBox1.Location = new System.Drawing.Point(11, 117);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(232, 87);
      this.groupBox1.TabIndex = 11;
      this.groupBox1.TabStop = false;
      this.groupBox2.Controls.Add((Control) this.tbWidth);
      this.groupBox2.Controls.Add((Control) this.tbHeight);
      this.groupBox2.Controls.Add((Control) this.labelZoom);
      this.groupBox2.Controls.Add((Control) this.labelWidth);
      this.groupBox2.Controls.Add((Control) this.labelOpacity);
      this.groupBox2.Controls.Add((Control) this.tbOpaque);
      this.groupBox2.Controls.Add((Control) this.tbZoom);
      this.groupBox2.Controls.Add((Control) this.labelHeight);
      this.groupBox2.Location = new System.Drawing.Point(11, 3);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(232, 108);
      this.groupBox2.TabIndex = 12;
      this.groupBox2.TabStop = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.Transparent;
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.groupBox1);
      this.Name = nameof (MagnifySetupControl);
      this.Size = new System.Drawing.Size(253, 215);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

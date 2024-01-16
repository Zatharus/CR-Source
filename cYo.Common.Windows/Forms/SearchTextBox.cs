// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.SearchTextBox
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  [Designer(typeof (ControlDesigner))]
  public class SearchTextBox : ToolStrip
  {
    private readonly ToolStripDropDownButton searchButton = new ToolStripDropDownButton();
    private readonly ToolStripTextBox textBox = new ToolStripTextBox();
    private readonly ToolStripButton clearButton = new ToolStripButton();
    private readonly LayoutEngine myLayout = (LayoutEngine) new SearchTextBox.MyLayout();
    private bool quickSelectAll;
    private string cueText;

    public SearchTextBox()
    {
      this.GripStyle = ToolStripGripStyle.Hidden;
      this.textBox.BorderStyle = BorderStyle.None;
      this.textBox.AutoCompleteMode = AutoCompleteMode.Suggest;
      this.textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.clearButton.Visible = false;
      this.clearButton.ImageScaling = ToolStripItemImageScaling.None;
      this.clearButton.ImageAlign = ContentAlignment.MiddleCenter;
      this.searchButton.ImageScaling = ToolStripItemImageScaling.None;
      this.searchButton.ImageAlign = ContentAlignment.MiddleCenter;
      this.Items.Add((ToolStripItem) this.searchButton);
      this.Items.Add((ToolStripItem) this.textBox);
      this.Items.Add((ToolStripItem) this.clearButton);
      this.Renderer = (ToolStripRenderer) new SearchTextBox.MyRenderer();
      this.textBox.TextChanged += new EventHandler(this.textBox_TextChanged);
      this.textBox.Enter += new EventHandler(this.textBox_Enter);
      this.textBox.Leave += new EventHandler(this.textBox_Leave);
      this.textBox.MouseDown += new MouseEventHandler(this.textBox_MouseDown);
      this.textBox.MouseUp += new MouseEventHandler(this.textBox_MouseUp);
      this.textBox.KeyDown += new KeyEventHandler(this.textBox_KeyDown);
      this.clearButton.Click += new EventHandler(this.clearButton_Click);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.clearButton.Dispose();
        this.searchButton.Dispose();
        this.textBox.Dispose();
      }
      base.Dispose(disposing);
    }

    public override LayoutEngine LayoutEngine => this.myLayout;

    public override string Text
    {
      get => base.Text;
      set => this.textBox.Text = base.Text = value;
    }

    private void textBox_TextChanged(object sender, EventArgs e)
    {
      this.Text = this.textBox.Text;
      this.clearButton.Visible = !string.IsNullOrEmpty(this.Text);
    }

    private void textBox_KeyDown(object sender, KeyEventArgs e)
    {
      this.quickSelectAll = false;
      if (e.KeyCode != Keys.Down)
        return;
      this.searchButton.ShowDropDown();
    }

    private void textBox_MouseUp(object sender, MouseEventArgs e) => this.quickSelectAll = false;

    private void textBox_Leave(object sender, EventArgs e)
    {
      this.quickSelectAll = false;
      this.UpdateAutoComplete();
    }

    private void textBox_MouseDown(object sender, MouseEventArgs e)
    {
      if (!this.quickSelectAll)
        return;
      this.textBox.SelectAll();
    }

    private void textBox_Enter(object sender, EventArgs e)
    {
      if (this.textBox.Text.Length <= 0)
        return;
      this.textBox.SelectAll();
      this.quickSelectAll = true;
    }

    private void clearButton_Click(object sender, EventArgs e)
    {
      this.UpdateAutoComplete();
      this.Text = string.Empty;
      this.textBox.Focus();
    }

    protected override void OnEnter(EventArgs e)
    {
      base.OnEnter(e);
      this.textBox.Focus();
    }

    public void SetCueText(string text)
    {
      if (text == this.cueText)
        return;
      this.cueText = text;
      this.textBox.TextBox.SetCueText(text);
    }

    public Image ClearButtonImage
    {
      get => this.clearButton.Image;
      set => this.clearButton.Image = value;
    }

    public Image SearchButtonImage
    {
      get => this.searchButton.Image;
      set => this.searchButton.Image = value;
    }

    public ToolStripDropDown SearchMenu
    {
      get => this.searchButton.DropDown;
      set => this.searchButton.DropDown = value;
    }

    public bool SearchButtonVisible
    {
      get => this.searchButton.Visible;
      set => this.searchButton.Visible = value;
    }

    public AutoCompleteStringCollection AutoCompleteList
    {
      get => this.textBox.AutoCompleteCustomSource;
      set => this.textBox.AutoCompleteCustomSource = value;
    }

    private void UpdateAutoComplete()
    {
      string text = this.textBox.Text;
      if (string.IsNullOrEmpty(text))
        return;
      this.textBox.AutoCompleteCustomSource.Add(text);
    }

    private class MyLayout : LayoutEngine
    {
      public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
      {
        SearchTextBox searchTextBox = container as SearchTextBox;
        Rectangle clientRectangle = searchTextBox.ClientRectangle;
        System.Drawing.Size size = new System.Drawing.Size(clientRectangle.Height, clientRectangle.Height);
        if (searchTextBox.Items.Count == 3)
        {
          ToolStripItem searchButton = (ToolStripItem) searchTextBox.searchButton;
          ToolStripItem clearButton = (ToolStripItem) searchTextBox.clearButton;
          ToolStripItem textBox = (ToolStripItem) searchTextBox.textBox;
          searchTextBox.SetItemLocation(searchButton, clientRectangle.Location);
          searchButton.Size = new System.Drawing.Size((int) ((double) size.Width * 1.5), size.Height);
          int num = searchButton.Visible ? searchButton.Width : 2;
          clearButton.Size = size;
          searchTextBox.SetItemLocation(clearButton, new System.Drawing.Point(clientRectangle.Width - clearButton.Width, clientRectangle.Y));
          int width = clientRectangle.Width - num - 2;
          if (clearButton.Visible)
            width -= clearButton.Width;
          textBox.Size = new System.Drawing.Size(width, size.Height);
          searchTextBox.SetItemLocation(textBox, new System.Drawing.Point(clientRectangle.X + num + 1, clientRectangle.Y));
        }
        return false;
      }
    }

    public class MyRenderer : ToolStripProfessionalRenderer
    {
      public MyRenderer()
        : base(ToolStripManager.Renderer is ToolStripProfessionalRenderer ? ((ToolStripProfessionalRenderer) ToolStripManager.Renderer).ColorTable : (ProfessionalColorTable) null)
      {
      }

      protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
      {
        base.OnRenderToolStripBorder(e);
        ControlPaint.DrawBorder3D(e.Graphics, e.AffectedBounds, Border3DStyle.Flat);
      }

      protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
      {
        base.OnRenderToolStripBackground(e);
        e.Graphics.Clear(SystemColors.Window);
      }
    }
  }
}

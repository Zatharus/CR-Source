// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.SubView
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Win32;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Viewer.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class SubView : CaptionControl
  {
    private IMain mainForm;
    private IContainer components;

    public SubView() => this.InitializeComponent();

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        IdleProcess.Idle -= new EventHandler(this.Application_Idle);
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IMain Main
    {
      get => this.mainForm;
      set
      {
        if (this.mainForm == value)
          return;
        this.mainForm = value;
        this.OnMainFormChanged();
      }
    }

    private void Application_Idle(object sender, EventArgs e) => this.OnIdle();

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (this.DesignMode)
        return;
      IdleProcess.Idle += new EventHandler(this.Application_Idle);
    }

    protected virtual void OnIdle()
    {
    }

    protected virtual void OnMainFormChanged() => SubView.SetMain(this.Controls, this.Main);

    private static void SetMain(Control.ControlCollection cc, IMain main)
    {
      foreach (Control control in (ArrangedElementCollection) cc)
      {
        if (control is SubView subView)
          subView.Main = main;
        else
          SubView.SetMain(control.Controls, main);
      }
    }

    protected static void TranslateColumns(IEnumerable<IColumn> itemViewColumnCollection)
    {
      ComicListField.TranslateColumns(itemViewColumnCollection);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleMode = AutoScaleMode.None;
      this.Name = nameof (SubView);
      this.ResumeLayout(false);
    }
  }
}

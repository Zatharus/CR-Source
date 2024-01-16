// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.NullableDateTimePicker
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class NullableDateTimePicker : DateTimePicker
  {
    private DateTimePickerFormat oldFormat = DateTimePickerFormat.Long;
    private string oldCustomFormat;
    private bool isNull;

    public new DateTime Value
    {
      get => !this.isNull ? base.Value : DateTime.MinValue;
      set
      {
        if (value < this.MinDate || value > this.MaxDate)
        {
          if (!this.isNull)
          {
            this.oldFormat = this.Format;
            this.oldCustomFormat = this.CustomFormat;
            this.isNull = true;
          }
          if (this.DesignMode)
            return;
          this.Format = DateTimePickerFormat.Custom;
          this.CustomFormat = " ";
        }
        else
        {
          if (this.isNull)
          {
            if (!this.DesignMode)
            {
              this.Format = this.oldFormat;
              this.CustomFormat = this.oldCustomFormat;
            }
            this.isNull = false;
          }
          base.Value = value;
        }
      }
    }

    protected override void OnCloseUp(EventArgs eventargs)
    {
      if (!this.DesignMode && Control.MouseButtons == MouseButtons.None && this.isNull)
      {
        this.Format = this.oldFormat;
        this.CustomFormat = this.oldCustomFormat;
        this.isNull = false;
      }
      base.OnCloseUp(eventargs);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (e.KeyCode != Keys.Delete)
        return;
      this.Value = DateTime.MinValue;
    }
  }
}

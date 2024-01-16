// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.WaitCursor
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class WaitCursor : DisposableObject
  {
    private readonly Form form;

    public WaitCursor(Form form)
    {
      this.form = form;
      if (form == null)
        Application.UseWaitCursor = true;
      else
        form.UseWaitCursor = true;
      Cursor.Current = Cursors.WaitCursor;
    }

    public WaitCursor(Control c)
      : this(WaitCursor.GetForm(c))
    {
    }

    public WaitCursor()
      : this((Form) null)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      Cursor.Current = Cursors.Default;
      if (this.form == null)
        Application.UseWaitCursor = false;
      else
        this.form.UseWaitCursor = false;
    }

    private static Form GetForm(Control c)
    {
      if (c == null)
        return (Form) null;
      return c is Form form ? form : WaitCursor.GetForm(c.Parent);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Controls.ComicPageControl
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Controls
{
  public class ComicPageControl : UserControl
  {
    private bool pendingUpdate;
    private ComicBook[] pendingBooks;

    [DefaultValue(null)]
    public virtual Image Icon { get; set; }

    public void MarkAsDirty() => this.pendingUpdate = true;

    public void ShowInfo(IEnumerable<ComicBook> books)
    {
      this.pendingBooks = (ComicBook[]) null;
      if (this.Visible)
      {
        this.OnShowInfo(books);
        this.pendingUpdate = false;
        this.pendingBooks = (ComicBook[]) null;
      }
      else
      {
        this.pendingUpdate = true;
        this.pendingBooks = books.ToArray<ComicBook>();
      }
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      if (!this.Visible)
        return;
      if (this.pendingUpdate)
        this.OnShowInfo((IEnumerable<ComicBook>) (this.pendingBooks ?? new ComicBook[0]));
      this.pendingUpdate = false;
      this.pendingBooks = (ComicBook[]) null;
    }

    protected virtual void OnShowInfo(IEnumerable<ComicBook> books)
    {
    }
  }
}

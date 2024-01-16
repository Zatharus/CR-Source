// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicListBrowser
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicListBrowser : SubView, IRefreshDisplay
  {
    protected readonly CursorList<IComicBookListProvider> history = new CursorList<IComicBookListProvider>();
    private IComicBookListProvider bookList;
    private IContainer components;

    public ComicListBrowser() => this.InitializeComponent();

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.BookList = (IComicBookListProvider) null;
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual IComicBookListProvider BookList
    {
      get => this.bookList;
      protected set
      {
        if (this.bookList == value)
          return;
        IComicBookListProvider bookList = this.bookList;
        this.bookList = value;
        if (this.bookList != null)
          this.bookList.ServiceRequest += new EventHandler<ServiceRequestEventArgs>(this.bookList_ServiceRequest);
        this.OnBookListChanged();
        if (bookList != null)
          bookList.ServiceRequest -= new EventHandler<ServiceRequestEventArgs>(this.bookList_ServiceRequest);
        this.history.AddAtCursor(this.bookList);
      }
    }

    protected virtual void OnBookListChanged()
    {
      if (this.BookListChanged == null)
        return;
      this.BookListChanged((object) this, EventArgs.Empty);
    }

    public event EventHandler BookListChanged;

    [Browsable(false)]
    public Guid BookListId => this.BookList != null ? this.BookList.Id : Guid.Empty;

    public virtual bool TopBrowserVisible
    {
      get => false;
      set
      {
      }
    }

    public virtual int TopBrowserSplit
    {
      get => 100;
      set
      {
      }
    }

    private void bookList_ServiceRequest(object sender, ServiceRequestEventArgs e)
    {
      this.OnListServiceRequest(sender as IComicBookListProvider, e);
    }

    public event EventHandler RefreshLists;

    protected virtual void OnListServiceRequest(
      IComicBookListProvider senderList,
      ServiceRequestEventArgs e)
    {
    }

    protected virtual IComicBookListProvider GetNewBookList()
    {
      if (this.Main != null)
        this.Main.StoreWorkspace();
      return this.BookList;
    }

    protected virtual void OnRefreshDisplay()
    {
      if (this.RefreshLists == null)
        return;
      this.RefreshLists((object) this, EventArgs.Empty);
    }

    public void OpenListInNewWindow()
    {
      IListDisplays parentService = this.FindParentService<IListDisplays>();
      IComicBookListProvider newBookList = this.GetNewBookList();
      if (parentService == null || newBookList == null)
        return;
      parentService.AddListWindow((Image) null, newBookList);
    }

    public void OpenListInNewTab(Image image)
    {
      IListDisplays parentService = this.FindParentService<IListDisplays>();
      IComicBookListProvider newBookList = this.GetNewBookList();
      if (parentService == null || newBookList == null)
        return;
      parentService.AddListTab(image, newBookList);
    }

    public void RefreshDisplay() => this.OnRefreshDisplay();

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleMode = AutoScaleMode.None;
      this.Name = nameof (ComicListBrowser);
      this.Size = new System.Drawing.Size(448, 342);
      this.ResumeLayout(false);
    }
  }
}

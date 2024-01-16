// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.ComicListFilesBrowser
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Localize;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using System;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class ComicListFilesBrowser : ComicListBrowser
  {
    private readonly FolderComicListProvider folderBooks = new FolderComicListProvider();
    private IContainer components;

    public ComicListFilesBrowser()
    {
      this.InitializeComponent();
      this.folderBooks.Window = (IWin32Window) this;
      this.BookList = (IComicBookListProvider) this.folderBooks;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.components != null)
          this.components.Dispose();
        this.folderBooks.Dispose();
      }
      base.Dispose(disposing);
    }

    public bool IncludeSubFolders
    {
      get => this.folderBooks.IncludeSubFolders;
      set => this.folderBooks.IncludeSubFolders = value;
    }

    protected override IComicBookListProvider GetNewBookList()
    {
      return (IComicBookListProvider) new FolderComicListProvider(this.folderBooks.Path);
    }

    public void FillBooks(string currentFolder)
    {
      AutomaticProgressDialog.Process((IWin32Window) this, TR.Messages["GettingList", "Getting Books List"], TR.Messages["GettingListText", "Retrieving all Books from the selected folder"], 1000, (Action) (() => this.folderBooks.Path = currentFolder), AutomaticProgressDialogOptions.EnableCancel);
    }

    public void SwitchIncludeSubFolders()
    {
      AutomaticProgressDialog.Process((IWin32Window) this, TR.Messages["GettingList", "Getting Books List"], TR.Messages["GettingListText", "Retrieving all Books from the selected folder"], 1000, (Action) (() => this.IncludeSubFolders = !this.IncludeSubFolders), AutomaticProgressDialogOptions.EnableCancel);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleMode = AutoScaleMode.None;
      this.Name = nameof (ComicListFilesBrowser);
      this.ResumeLayout(false);
    }
  }
}

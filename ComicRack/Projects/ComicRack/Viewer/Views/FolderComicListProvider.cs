// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.FolderComicListProvider
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Win32;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class FolderComicListProvider : 
    NamedIdComponent,
    IComicBookListProvider,
    ILiteComponent,
    IDisposable,
    IIdentity,
    IComicBookList,
    IRemoveBooks
  {
    private string path;
    private bool includeSubFolders;
    private volatile List<ComicBook> currentBooks = new List<ComicBook>();

    public FolderComicListProvider()
    {
    }

    public FolderComicListProvider(string path)
      : this()
    {
      this.Path = path;
    }

    public IWin32Window Window { get; set; }

    public string Path
    {
      get => this.path;
      set
      {
        if (this.path == value)
          return;
        this.path = value;
        this.Name = System.IO.Path.GetFileName(this.path);
        this.Refresh();
      }
    }

    public bool IncludeSubFolders
    {
      get => this.includeSubFolders;
      set
      {
        if (this.includeSubFolders == value)
          return;
        this.includeSubFolders = value;
        this.Refresh();
      }
    }

    public IEnumerable<ComicBook> GetBooks() => (IEnumerable<ComicBook>) this.currentBooks;

    public event EventHandler BookListChanged;

    public void Refresh()
    {
      this.currentBooks = this.GetFolderBookList(this.Path);
      if (this.BookListChanged == null)
        return;
      this.BookListChanged((object) this, EventArgs.Empty);
    }

    public int BookCount { get; set; }

    protected List<ComicBook> GetFolderBookList(string folder)
    {
      List<ComicBook> folderBookList = new List<ComicBook>();
      try
      {
        IEnumerable<string> fileExtensions = Providers.Readers.GetFileExtensions();
        foreach (string file in FileUtility.GetFiles(folder, this.includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
        {
          string f = file;
          if (fileExtensions.Any<string>((Func<string, bool>) (ext => f.EndsWith(ext, StringComparison.OrdinalIgnoreCase))))
          {
            ComicBook comicBook = Program.BookFactory.Create(file, CreateBookOption.AddToTemporary, folderBookList.Count > 100 ? RefreshInfoOptions.DontReadInformation : RefreshInfoOptions.None);
            if (comicBook != null)
              folderBookList.Add(comicBook);
          }
          if (AutomaticProgressDialog.ShouldAbort)
            break;
        }
      }
      catch
      {
      }
      return folderBookList;
    }

    public void RemoveBooks(IEnumerable<ComicBook> books, bool ask)
    {
      books = (IEnumerable<ComicBook>) books.ToArray<ComicBook>();
      if (ask)
      {
        using (Image image = Program.MakeBooksImage(books, new System.Drawing.Size(256, 128), 5, false))
        {
          QuestionResult questionResult = QuestionDialog.AskQuestion(this.Window, TR.Messages["AskMoveBin", "Are you sure you want to move these files to the Recycle Bin?"], TR.Messages["Remove", "Remove"], (Program.Settings.RemoveFilesfromDatabase ? "!" : string.Empty) + TR.Messages["AlsoRemoveFromLibrary", "&Additionally remove the books from the Library (all information not stored in the files will be lost)"], image);
          if (questionResult == QuestionResult.Cancel)
            return;
          Program.Settings.RemoveFilesfromDatabase = questionResult == QuestionResult.OkWithOption;
        }
      }
      bool flag = false;
      int num1 = 0;
      using (new WaitCursor())
      {
        foreach (ComicBook book in books)
        {
          try
          {
            ShellFile.DeleteFile(this.Window, ShellFileDeleteOptions.None, book.FilePath);
            if (this.currentBooks.Remove(book))
              ++num1;
          }
          catch
          {
          }
          if (File.Exists(book.FilePath))
          {
            flag = true;
          }
          else
          {
            this.BookCount -= num1;
            if (Program.Settings.RemoveFilesfromDatabase)
              Program.Database.Books.RemoveRange(books);
          }
        }
      }
      if (!flag)
      {
        if (this.BookListChanged == null)
          return;
        this.BookListChanged((object) this, EventArgs.Empty);
      }
      else
      {
        int num2 = (int) MessageBox.Show(TR.Messages["FailedDeleteBooks", "Some books could not be deleted (maybe they are in use)!"], Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
    }

    [SpecialName]
    void IComicBookListProvider.add_NameChanged(EventHandler value) => this.NameChanged += value;

    [SpecialName]
    void IComicBookListProvider.remove_NameChanged(EventHandler value) => this.NameChanged -= value;

    [SpecialName]
    Guid IIdentity.get_Id() => this.Id;

    [SpecialName]
    string IComicBookList.get_Name() => this.Name;
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.NavigatorManager
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.Collections;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Display;
using cYo.Projects.ComicRack.Plugins.Automation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  [ComVisible(true)]
  public class NavigatorManager : IOpenBooksManager
  {
    private int currentSlot = -1;
    private readonly SmartList<ComicBookNavigator> slots = new SmartList<ComicBookNavigator>();
    private readonly IComicDisplay comicDisplay;
    private bool blockCurrentSlotChanged;

    public NavigatorManager(IComicDisplay comicDisplay)
    {
      this.comicDisplay = comicDisplay;
      this.slots.Changed += new EventHandler<SmartListChangedEventArgs<ComicBookNavigator>>(this.slots_Changed);
    }

    public ComicBookNavigator CurrentBook => this.comicDisplay.Book;

    public int CurrentSlot
    {
      get => this.currentSlot;
      set
      {
        value = value.Clamp(-1, this.slots.Count - 1);
        if (this.currentSlot == value)
          return;
        this.currentSlot = value;
        try
        {
          ComicBookNavigator slot = this.currentSlot < 0 ? (ComicBookNavigator) null : this.slots[this.currentSlot];
          if (Win7.TabbedThumbnailsEnabled && this.CurrentBook != null && this.comicDisplay.Book != null)
            this.CurrentBook.Thumbnail = this.comicDisplay.CreateThumbnail();
          this.comicDisplay.Book = slot;
        }
        catch
        {
          this.comicDisplay.Book = (ComicBookNavigator) null;
        }
        this.OnCurrentSlotChanged();
      }
    }

    public SmartList<ComicBookNavigator> Slots => this.slots;

    public IComicDisplay ComicDisplay => this.comicDisplay;

    public int OpenCount
    {
      get
      {
        return this.slots.Count<ComicBookNavigator>((Func<ComicBookNavigator, bool>) (nav => nav != null));
      }
    }

    public IEnumerable<string> OpenFiles
    {
      get
      {
        return this.slots.Where<ComicBookNavigator>((Func<ComicBookNavigator, bool>) (nav => nav != null && nav.Comic != null && nav.Comic.EditMode.IsLocalComic())).Select<ComicBookNavigator, string>((Func<ComicBookNavigator, string>) (nav => nav.Comic.FilePath));
      }
    }

    public bool IsOpen(ComicBook cb)
    {
      return this.slots.Any<ComicBookNavigator>((Func<ComicBookNavigator, bool>) (nav => nav != null && nav.Comic == cb));
    }

    public void AddSlot()
    {
      this.Slots.Add((ComicBookNavigator) null);
      this.CurrentSlot = this.Slots.Count - 1;
    }

    public void MoveSlot(int slot, int newSlot)
    {
      if (slot < 0 || slot >= this.slots.Count || newSlot < 0 || newSlot >= this.slots.Count)
        return;
      this.CurrentSlot = -1;
      this.slots.Move(this.slots[slot], newSlot);
      this.CurrentSlot = newSlot;
      this.OnOpenComicsChanged();
    }

    public void MoveCurrentSlot(int newSlot)
    {
      if (this.CurrentSlot == -1)
        return;
      this.MoveSlot(this.CurrentSlot, newSlot);
    }

    public bool Open(ComicBook cb, OpenComicOptions options, int page = 0)
    {
      ComicBookNavigator comicBookNavigator1 = (ComicBookNavigator) null;
      if (cb != null && cb.IsLinked)
      {
        if (cb.EditMode.IsLocalComic() && Program.Settings.AddToLibraryOnOpen)
          cb = Program.BookFactory.Create(cb.FilePath, Program.Settings.AddToLibraryOnOpen ? CreateBookOption.AddToStorage : CreateBookOption.AddToTemporary);
        if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
          options ^= OpenComicOptions.OpenInNewSlot;
        if (page == 0 && Program.Settings.OpenLastPage && cb != null)
          page = cb.CurrentPage;
        ComicBookNavigator comicBookNavigator2 = this.Slots.Find((Predicate<ComicBookNavigator>) (nav => nav != null && nav.Comic == cb));
        try
        {
          if (comicBookNavigator2 != null)
          {
            this.CurrentSlot = this.slots.IndexOf(comicBookNavigator2);
            this.OnBookOpened(new BookEventArgs(comicBookNavigator2.Comic));
            if (page != 0)
              comicBookNavigator2.Navigate(page, PageSeekOrigin.Absolute);
            return true;
          }
        }
        catch
        {
        }
        comicBookNavigator1 = NavigatorManager.OpenComic(cb, page, options);
        bool flag1 = false;
        bool flag2 = (options & OpenComicOptions.OpenInNewSlot) == OpenComicOptions.None || comicBookNavigator1 == null;
        if (flag2 && this.CurrentSlot >= 0 && this.CurrentSlot < this.Slots.Count && this.Slots[this.CurrentSlot] != null)
          this.OnBookClosing(new BookEventArgs(this.Slots[this.CurrentSlot].Comic));
        try
        {
          using (ItemMonitor.Lock(this.slots.SyncRoot))
          {
            if (flag2)
            {
              if (this.CurrentSlot >= 0)
              {
                int currentSlot = this.CurrentSlot;
                this.blockCurrentSlotChanged = true;
                try
                {
                  this.CurrentSlot = -1;
                }
                finally
                {
                  this.blockCurrentSlotChanged = false;
                }
                this.Slots[currentSlot] = comicBookNavigator1;
                this.CurrentSlot = currentSlot;
                flag1 = true;
              }
            }
          }
        }
        catch
        {
        }
        if (!flag1)
        {
          if ((options & OpenComicOptions.AppendNewSlots) == OpenComicOptions.None)
          {
            this.CurrentSlot = -1;
            this.slots.Insert(0, comicBookNavigator1);
            this.CurrentSlot = 0;
          }
          else
          {
            this.slots.Add(comicBookNavigator1);
            this.CurrentSlot = this.slots.Count - 1;
          }
        }
      }
      if (comicBookNavigator1 != null)
        this.OnBookOpened(new BookEventArgs(comicBookNavigator1.Comic));
      this.comicDisplay.DisplayOpenMessage();
      return comicBookNavigator1 != null;
    }

    public bool Open(ComicBook cb, bool inNewSlot, int page = 0)
    {
      return this.Open(cb, (OpenComicOptions) (0 | (inNewSlot ? 32 : 0)), page);
    }

    public bool Open(string file, OpenComicOptions options = OpenComicOptions.None, int page = 0)
    {
      return this.Open(Program.BookFactory.Create(file, Program.Settings.AddToLibraryOnOpen ? CreateBookOption.AddToStorage : CreateBookOption.AddToTemporary), options, page);
    }

    public bool Open(string file, bool inNewSlot, int page = 0)
    {
      return this.Open(file, (OpenComicOptions) (0 | (inNewSlot ? 32 : 0)), page);
    }

    public bool Open(IEnumerable<string> files, OpenComicOptions options)
    {
      bool flag = true;
      foreach (string file in files)
      {
        if (!string.IsNullOrEmpty(file))
          flag &= this.Open(file, options | OpenComicOptions.OpenInNewSlot);
      }
      return flag;
    }

    public void Close(int slot)
    {
      if (slot < 0 || slot >= this.Slots.Count)
        return;
      if (this.Slots[slot] != null)
        this.OnBookClosing(new BookEventArgs(this.Slots[slot].Comic));
      try
      {
        if (this.CurrentSlot == slot)
        {
          if (this.Slots.Count == 1)
            this.CurrentSlot = -1;
          else if (slot < this.Slots.Count - 1)
            ++this.CurrentSlot;
          else
            --this.CurrentSlot;
        }
        this.Slots.RemoveAt(slot);
        if (this.CurrentSlot <= slot)
          return;
        --this.CurrentSlot;
      }
      catch
      {
      }
    }

    public void NextSlot()
    {
      if (this.CurrentSlot < 0)
        return;
      int num = this.CurrentSlot + 1;
      if (num >= this.Slots.Count)
        num = 0;
      this.CurrentSlot = num;
    }

    public void PreviousSlot()
    {
      if (this.CurrentSlot < 0)
        return;
      int num = this.CurrentSlot - 1;
      if (num < 0)
        num = this.Slots.Count - 1;
      this.CurrentSlot = num;
    }

    public void Close() => this.Close(this.CurrentSlot);

    public void CloseAll()
    {
      this.Slots.Clear();
      this.CurrentSlot = -1;
    }

    public void CloseAllButCurrent()
    {
      this.Slots.Move(this.CurrentSlot, 0);
      for (int slot = this.Slots.Count - 1; slot >= 1; --slot)
        this.Close(slot);
    }

    public void CloseAllToTheRight()
    {
      for (int slot = this.Slots.Count - 1; slot > this.CurrentSlot; --slot)
        this.Close(slot);
    }

    public string GetSlotCaption(int i)
    {
      if (i < 0)
        return string.Empty;
      ComicBookNavigator itemOrDefault = this.slots.GetItemOrDefault(i);
      return itemOrDefault != null ? itemOrDefault.Comic.Caption : TR.Default["None", "None"];
    }

    public event EventHandler OpenComicsChanged;

    public event EventHandler CurrentSlotChanged;

    public event EventHandler<BookEventArgs> BookOpened;

    public event EventHandler<BookEventArgs> BookClosing;

    public event EventHandler<BookEventArgs> BookClosed;

    protected virtual void OnBookOpened(BookEventArgs e)
    {
      if (this.BookOpened == null)
        return;
      this.BookOpened((object) this, e);
    }

    protected virtual void OnBookClosed(BookEventArgs e)
    {
      if (this.BookClosed == null)
        return;
      this.BookClosed((object) this, e);
    }

    protected virtual void OnBookClosing(BookEventArgs e)
    {
      if (this.BookClosing == null)
        return;
      this.BookClosing((object) this, e);
    }

    protected virtual void OnOpenComicsChanged()
    {
      if (this.OpenComicsChanged == null)
        return;
      this.OpenComicsChanged((object) this, EventArgs.Empty);
    }

    protected virtual void OnCurrentSlotChanged()
    {
      if (this.blockCurrentSlotChanged || this.CurrentSlotChanged == null)
        return;
      this.CurrentSlotChanged((object) this, EventArgs.Empty);
    }

    private void slots_Changed(object sender, SmartListChangedEventArgs<ComicBookNavigator> e)
    {
      switch (e.Action)
      {
        case SmartListAction.Insert:
          if (e.Item == null)
            break;
          e.Item.Comic.BookChanged += new EventHandler<BookChangedEventArgs>(this.Comic_PropertyChanged);
          break;
        case SmartListAction.Remove:
          if (e.Item == null)
            break;
          e.Item.Comic.BookChanged -= new EventHandler<BookChangedEventArgs>(this.Comic_PropertyChanged);
          this.OnBookClosed(new BookEventArgs(e.Item.Comic));
          ThreadPool.QueueUserWorkItem((WaitCallback) (x => e.Item.Dispose()));
          break;
      }
    }

    private void Comic_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.OnOpenComicsChanged();
    }

    public static ComicBookNavigator OpenComic(
      ComicBook comicBook,
      int page,
      OpenComicOptions options)
    {
      if (comicBook == null)
        return (ComicBookNavigator) null;
      if (!comicBook.IsLinked)
        return (ComicBookNavigator) null;
      if ((options & OpenComicOptions.NoMoveToLastPage) != OpenComicOptions.None)
        page = 0;
      if ((options & OpenComicOptions.NoRefreshInfo) == OpenComicOptions.None)
        comicBook.RefreshInfoFromFile();
      ComicBookNavigator navigator = comicBook.CreateNavigator();
      if (navigator == null)
        return (ComicBookNavigator) null;
      if ((options & OpenComicOptions.NoUpdateCurrentPage) != OpenComicOptions.None)
        navigator.UpdateCurrentPageEnabled = false;
      if ((options & OpenComicOptions.NoGlobalColorAdjustment) == OpenComicOptions.None)
        navigator.BaseColorAdjustment = Program.Settings.GlobalColorAdjustment;
      if ((options & OpenComicOptions.NoIncreaseOpenedCount) == OpenComicOptions.None)
        navigator.Opened += (EventHandler) ((sender, e) => ++comicBook.OpenedCount);
      if (comicBook.IsDynamicSource)
        Program.ImagePool.RefreshLastImage(comicBook.FilePath);
      navigator.Open(true, page);
      return navigator;
    }

    public bool OpenFile(string file, bool inNewSlot, int page) => this.Open(file, inNewSlot, page);
  }
}

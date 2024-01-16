// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Plugins.Automation.IApplication
// Assembly: ComicRack.Plugins, Version=1.0.5915.38776, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 7731A722-0965-4F0F-979D-2535A7EE7B43
// Assembly location: C:\Program Files\ComicRack\ComicRack.Plugins.dll

using cYo.Projects.ComicRack.Engine;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Plugins.Automation
{
  public interface IApplication
  {
    void Restart();

    void SynchronizeDevices();

    void ScanFolders();

    IEnumerable<ComicBook> ReadDatabaseBooks(string file);

    IEnumerable<ComicBook> GetLibraryBooks();

    ComicBook AddNewBook(bool showDialog);

    bool RemoveBook(ComicBook cb);

    bool SetCustomBookThumbnail(ComicBook cb, Bitmap bitmap);

    ComicBook GetBook(string file);

    Bitmap GetComicPage(ComicBook cb, int page);

    Bitmap GetComicThumbnail(ComicBook cb, int page);

    IDictionary<string, string> GetComicFields();

    Bitmap GetComicPublisherIcon(ComicBook cb);

    Bitmap GetComicImprintIcon(ComicBook cb);

    Bitmap GetComicAgeRatingIcon(ComicBook cb);

    Bitmap GetComicFormatIcon(ComicBook cb);

    string ReadInternet(string url);

    int AskQuestion(string question, string buttonText, string optionText);

    void ShowComicInfo(IEnumerable<ComicBook> books);

    string ProductVersion { get; }
  }
}

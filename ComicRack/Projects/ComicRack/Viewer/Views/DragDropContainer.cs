// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Views.DragDropContainer
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.ComponentModel;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Views
{
  public class DragDropContainer
  {
    private readonly ComicBookContainer books;
    private ComicBookMatcher matcher;
    private readonly IEnumerable<string> filesOrFolders = Enumerable.Empty<string>();
    private readonly IEnumerable<string> readingLists = Enumerable.Empty<string>();

    public DragDropContainer()
    {
    }

    public DragDropContainer(ComicBookContainer books, ComicBookMatcher matcher)
    {
      this.books = books;
      this.matcher = matcher;
    }

    public DragDropContainer(IEnumerable<string> filesOrFolders)
    {
      this.filesOrFolders = filesOrFolders;
      this.readingLists = filesOrFolders.Where<string>((Func<string, bool>) (file => ".cbl".Equals(Path.GetExtension(file), StringComparison.OrdinalIgnoreCase)));
    }

    public ComicBookContainer Books => this.books;

    public ComicBookMatcher Matcher => this.matcher;

    public IEnumerable<string> FilesOrFolders => this.filesOrFolders;

    public IEnumerable<string> ReadingLists => this.readingLists;

    public bool IsReadingListsContainer => this.readingLists.Count<string>() > 0;

    public bool IsBookContainer => this.books != null && this.books.Books.Count > 0;

    public bool IsFilesContainer => this.filesOrFolders.Count<string>() > 0;

    public bool HasMatcher => this.Matcher != null;

    public bool IsValid => this.IsBookContainer || this.IsFilesContainer;

    public IEnumerable<ComicBookMatcher> CreateSeriesGroupMatchers(int maxEntries = 10)
    {
      if (this.IsBookContainer)
      {
        if (this.HasMatcher)
        {
          yield return this.Matcher;
        }
        else
        {
          foreach (ComicBookMatcher seriesGroupMatcher in this.Books.Books.Select(cb => new
          {
            Series = cb.ShadowSeries,
            Volume = cb.ShadowVolume
          }).Distinct().OrderBy(t => t.Series).ThenBy(t => t.Volume).Take(maxEntries).Select(t =>
          {
            ComicBookGroupMatcher seriesGroupMatchers = new ComicBookGroupMatcher();
            ComicBookMatcherCollection matchers1 = seriesGroupMatchers.Matchers;
            matchers1.Add((ComicBookMatcher) new ComicBookSeriesMatcher()
            {
              MatchOperator = 0,
              MatchValue = t.Series
            });
            if (t.Volume >= 0)
            {
              ComicBookMatcherCollection matchers2 = seriesGroupMatchers.Matchers;
              matchers2.Add((ComicBookMatcher) new ComicBookVolumeMatcher()
              {
                MatchOperator = 0,
                MatchValue = t.Volume.ToString()
              });
            }
            return seriesGroupMatchers;
          }))
            yield return seriesGroupMatcher;
        }
      }
    }

    public ComicSmartListItem CreateSeriesSmartList()
    {
      if (!this.IsBookContainer)
        return (ComicSmartListItem) null;
      ComicSmartListItem seriesSmartList = new ComicSmartListItem(this.Books.Name ?? this.Books.Books[0].ShadowSeries)
      {
        MatcherMode = MatcherMode.Or
      };
      seriesSmartList.Matchers.AddRange(this.CreateSeriesGroupMatchers());
      return seriesSmartList;
    }

    public ComicIdListItem CreateComicIdList()
    {
      if (!this.IsBookContainer)
        return (ComicIdListItem) null;
      ComicIdListItem comicIdList = new ComicIdListItem(this.Books.Name);
      comicIdList.AddRange((IEnumerable<ComicBook>) this.Books.Books);
      return comicIdList;
    }

    public static DragDropContainer Create(IDataObject data)
    {
      if (data.GetDataPresent(typeof (ComicBookContainer)))
        return new DragDropContainer(data.GetData(typeof (ComicBookContainer)) as ComicBookContainer, data.GetData("ComicBookMatcher") as ComicBookMatcher);
      return data.GetDataPresent(DataFormats.FileDrop) ? new DragDropContainer((IEnumerable<string>) (string[]) data.GetData(DataFormats.FileDrop)) : new DragDropContainer();
    }
  }
}

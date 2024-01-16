// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Config.DefaultLists
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common.IO;
using cYo.Common.Text;
using cYo.Projects.ComicRack.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Config
{
  public class DefaultLists
  {
    private const string DefaultGenresSection = "Book Genres";
    private const string DefaultFormatsSection = "Book Formats";
    private const string DefaultAgeRatingsSection = "Age Ratings";
    private const string DefaultBookAgesSection = "Book Ages";
    private const string DefaultBookConditionsSection = "Book Conditions";
    private const string DefaultBookCollectionStatusSection = "Book Collection Status";
    private Func<IEnumerable<ComicBook>> getBooks;

    public DefaultLists(Func<IEnumerable<ComicBook>> getBooks, IEnumerable<string> initPaths)
    {
      this.getBooks = getBooks;
      this.DefaultGenres = DefaultLists.LoadDefaultTextList(initPaths, "Book Genres").ToArray<string>();
      this.DefaultFormats = DefaultLists.LoadDefaultTextList(initPaths, "Book Formats").Concat<string>(ComicBook.FormatIcons.Keys).Distinct<string>().OrderBy<string, string>((Func<string, string>) (s => s)).ToArray<string>();
      this.DefaultAgeRatings = DefaultLists.LoadDefaultTextList(initPaths, "Age Ratings").Concat<string>(ComicBook.AgeRatingIcons.Keys).OrderBy<string, string>((Func<string, string>) (s => s)).Distinct<string>().ToArray<string>();
      this.DefaultBookAges = DefaultLists.LoadDefaultTextList(initPaths, "Book Ages").ToArray<string>();
      this.DefaultBookConditions = DefaultLists.LoadDefaultTextList(initPaths, "Book Conditions").ToArray<string>();
      this.DefaultBookCollectionStatus = DefaultLists.LoadDefaultTextList(initPaths, "Book Collection Status").ToArray<string>();
    }

    public string[] DefaultGenres { get; private set; }

    public string[] DefaultFormats { get; private set; }

    public string[] DefaultAgeRatings { get; private set; }

    public string[] DefaultBookAges { get; private set; }

    public string[] DefaultBookConditions { get; private set; }

    public string[] DefaultBookCollectionStatus { get; private set; }

    public AutoCompleteStringCollection GetComicFieldList(
      Func<ComicBook, string> autoCompleteHandler,
      bool sort = false)
    {
      AutoCompleteStringCollection comicFieldList = new AutoCompleteStringCollection();
      foreach (ComicBook comicBook in this.getBooks())
        comicFieldList.Add(autoCompleteHandler(comicBook));
      if (sort)
        ArrayList.Adapter((IList) comicFieldList).Sort();
      return comicFieldList;
    }

    public AutoCompleteStringCollection GetGenreList(bool withSeparator)
    {
      AutoCompleteStringCollection comicFieldList = this.GetComicFieldList((Func<ComicBook, string>) (cb => cb.Genre));
      comicFieldList.Remove("");
      if (withSeparator)
      {
        comicFieldList.Remove("-");
        if (comicFieldList.Count > 0)
          comicFieldList.Add("-");
      }
      comicFieldList.AddRange(this.DefaultGenres);
      return comicFieldList;
    }

    public AutoCompleteStringCollection GetFormatList()
    {
      AutoCompleteStringCollection comicFieldList = this.GetComicFieldList((Func<ComicBook, string>) (cb => cb.ShadowFormat), true);
      comicFieldList.Remove("");
      comicFieldList.Remove("-");
      if (comicFieldList.Count > 0)
        comicFieldList.Add("-");
      comicFieldList.AddRange(this.DefaultFormats);
      return comicFieldList;
    }

    public AutoCompleteStringCollection GetAgeRatingList()
    {
      AutoCompleteStringCollection comicFieldList = this.GetComicFieldList((Func<ComicBook, string>) (cb => cb.AgeRating));
      comicFieldList.Remove("");
      comicFieldList.Remove("-");
      if (comicFieldList.Count > 0)
        comicFieldList.Add("-");
      comicFieldList.AddRange(this.DefaultAgeRatings);
      return comicFieldList;
    }

    public AutoCompleteStringCollection GetBookAgeList()
    {
      AutoCompleteStringCollection comicFieldList = this.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookAge));
      comicFieldList.Remove("");
      comicFieldList.Remove("-");
      if (comicFieldList.Count > 0)
        comicFieldList.Add("-");
      comicFieldList.AddRange(this.DefaultBookAges);
      return comicFieldList;
    }

    public AutoCompleteStringCollection GetBookConditionList()
    {
      AutoCompleteStringCollection comicFieldList = this.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookCondition));
      comicFieldList.Remove("");
      comicFieldList.Remove("-");
      if (comicFieldList.Count > 0)
        comicFieldList.Add("-");
      comicFieldList.AddRange(this.DefaultBookConditions);
      return comicFieldList;
    }

    public AutoCompleteStringCollection GetBookCollectionStatusList()
    {
      AutoCompleteStringCollection comicFieldList = this.GetComicFieldList((Func<ComicBook, string>) (cb => cb.BookCollectionStatus));
      comicFieldList.Remove("");
      comicFieldList.Remove("-");
      comicFieldList.AddRange(this.DefaultBookCollectionStatus);
      return comicFieldList;
    }

    private static IEnumerable<string> LoadDefaultTextList(
      IEnumerable<string> files,
      string section)
    {
      List<string> stringList = new List<string>();
      foreach (string file in files)
      {
        try
        {
          stringList.AddRange(DefaultLists.LoadTextList(file, section));
        }
        catch (Exception ex)
        {
        }
      }
      return (IEnumerable<string>) stringList;
    }

    private static IEnumerable<string> LoadTextList(string file, string section)
    {
      if (File.Exists(file))
      {
        string sectionHeader = string.Format("[{0}]", (object) section);
        bool sectionFound = false;
        string prefix = (string) null;
        foreach (string str in FileUtility.ReadLines(file).TrimEndStrings().RemoveEmpty())
        {
          string line = str;
          if (!line.StartsWith(";"))
          {
            if (line.StartsWith(sectionHeader))
              sectionFound = true;
            else if (!sectionFound || !line.StartsWith("["))
            {
              if (sectionFound)
              {
                if (!char.IsWhiteSpace(line[0]) || string.IsNullOrEmpty(prefix))
                {
                  prefix = line;
                  yield return line;
                }
                else
                  yield return prefix + ": " + line.Trim();
                line = (string) null;
              }
            }
            else
              break;
          }
        }
      }
    }
  }
}

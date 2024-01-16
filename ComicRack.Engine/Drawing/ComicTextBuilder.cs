// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Drawing.ComicTextBuilder
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.Localize;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Drawing
{
  public static class ComicTextBuilder
  {
    private static readonly TR TRComic = TR.Load("ComicBook");
    private static readonly string NoInformationAvailable = ComicTextBuilder.TRComic["NoInformation", "No information available"];
    private static readonly string SizeText = ComicTextBuilder.TRComic["Size", "Size:\t{0}/{1}"];
    private static readonly string OpenedText = ComicTextBuilder.TRComic["Opened", "Opened:\t{0}"];
    private static readonly string AddedText = ComicTextBuilder.TRComic["Added", "Added:\t{0}"];
    private static readonly string ReleasedText = ComicTextBuilder.TRComic["Released", "Released:\t{0}"];
    private static readonly string FormatText = ComicTextBuilder.TRComic["Format", "Format:\t{0}"];
    private static readonly string FileNameText = ComicTextBuilder.TRComic["FileName", "File:\t{0}"];
    private static readonly string PageText = ComicTextBuilder.TRComic[nameof (PageText), "Page #{0}"];
    private static readonly string PageSizeText = ComicTextBuilder.TRComic["PageSize", "Size:\t{0}"];
    private static readonly string UnknownSizeText = ComicTextBuilder.TRComic["UnknownSize", "Unknown Size"];
    private static readonly string ResolutionText = ComicTextBuilder.TRComic["Resolution", "Resolution:\t{0} x {1}"];
    private static readonly string RotationText = ComicTextBuilder.TRComic["Rotation", "Rotation:\t{0}"];
    private static readonly string BookmarkText = ComicTextBuilder.TRComic["Bookmark", "Bookmark:\t{0}"];
    private static readonly string BookCountText = ComicTextBuilder.TRComic["BookCount", "Books:\t{0}"];

    public static IEnumerable<TextLine> GetTextBlocks(
      ComicBook comicBook,
      Font font,
      Color foreColor,
      ComicTextElements flags)
    {
      if (comicBook == null)
      {
        yield return new TextLine(ComicTextBuilder.NoInformationAvailable, ComicTextBuilder.GetCaptionFont(font), foreColor);
      }
      else
      {
        if (!comicBook.IsLinked)
          flags &= ~ComicTextElements.LinkedElements;
        if (flags.HasFlag((Enum) ComicTextElements.StackTitle))
          yield return new TextLine(comicBook.Series, ComicTextBuilder.GetCaptionFont(font), foreColor);
        if (flags.HasFlag((Enum) ComicTextElements.Caption))
          yield return new TextLine(comicBook.Caption, ComicTextBuilder.GetCaptionFont(font), foreColor);
        if (flags.HasFlag((Enum) ComicTextElements.CaptionWithoutTitle))
          yield return new TextLine(comicBook.CaptionWithoutTitle, ComicTextBuilder.GetCaptionFont(font), foreColor);
        if (flags.HasFlag((Enum) ComicTextElements.Title))
          yield return new TextLine(comicBook.ShadowTitle, ComicTextBuilder.GetCaptionFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.AlternateCaption))
          yield return new TextLine(comicBook.AlternateCaption, ComicTextBuilder.GetNormalFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.PublisherAndImprint))
          yield return new TextLine(comicBook.Publisher.AppendWithSeparator("/", comicBook.Imprint, comicBook.ISBN), ComicTextBuilder.GetSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.AgeRating))
          yield return new TextLine(comicBook.AgeRating, ComicTextBuilder.GetSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.ArtistInfo))
          yield return new TextLine(comicBook.ArtistInfo, ComicTextBuilder.GetSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        yield return new TextLine(4);
        yield return new TextLine(0) { ScrollStart = true };
        if (flags.HasFlag((Enum) ComicTextElements.Genre))
          yield return new TextLine(comicBook.Genre, ComicTextBuilder.GetNormalFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.CharactersAndTeams))
          yield return new TextLine(comicBook.Characters.AppendWithSeparator(", ", comicBook.Teams), ComicTextBuilder.GetNormalFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.Locations))
          yield return new TextLine(comicBook.Locations, ComicTextBuilder.GetNormalFont(font), foreColor, StringFormatFlags.NoWrap);
        yield return new TextLine(4);
        if (flags.HasFlag((Enum) ComicTextElements.PurchaseInformation))
          yield return new TextLine(comicBook.BookStore.AppendWithSeparator("/", comicBook.BookPriceAsText), ComicTextBuilder.GetNormalFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.StorageInfoformation))
          yield return new TextLine(comicBook.BookLocation.AppendWithSeparator("/", comicBook.BookAge, comicBook.BookCondition), ComicTextBuilder.GetNormalFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.CollectionStatus))
          yield return new TextLine(comicBook.BookCollectionStatus, ComicTextBuilder.GetNormalFont(font), foreColor, StringFormatFlags.NoWrap);
        yield return new TextLine(6);
        if (flags.HasFlag((Enum) ComicTextElements.Summary))
        {
          StringFormat format = new StringFormat()
          {
            Trimming = StringTrimming.EllipsisWord
          };
          yield return new TextLine(comicBook.Summary.Replace("\t", string.Empty), ComicTextBuilder.GetSmallFont(font), foreColor, format);
        }
        if (flags.HasFlag((Enum) ComicTextElements.Notes))
        {
          StringFormat format = new StringFormat()
          {
            Trimming = StringTrimming.EllipsisWord
          };
          yield return new TextLine(comicBook.Notes.AppendWithSeparator("\n", comicBook.BookNotes).Replace("\t", string.Empty), ComicTextBuilder.GetSmallFont(font), foreColor, format);
        }
        if (flags.HasFlag((Enum) ComicTextElements.ScanInformation))
          yield return new TextLine(comicBook.ScanInformation, ComicTextBuilder.GetNormalFont(font), foreColor, StringFormatFlags.NoWrap);
        yield return new TextLine(6);
        if (flags.HasFlag((Enum) ComicTextElements.StackBookCount))
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.BookCountText, (object) comicBook.CountAsText), ComicTextBuilder.GetSmallSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.StackBooksOpened))
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.OpenedText, (object) comicBook.LastPageRead), ComicTextBuilder.GetSmallSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.FileSize))
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.SizeText, (object) comicBook.FileSizeAsText, (object) comicBook.PagesAsText), ComicTextBuilder.GetSmallSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.Released) && (!flags.HasFlag((Enum) ComicTextElements.NoEmptyDates) || comicBook.ReleasedTime != DateTime.MinValue))
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.ReleasedText, (object) comicBook.ReleasedTimeAsText), ComicTextBuilder.GetSmallSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.Opened) && (!flags.HasFlag((Enum) ComicTextElements.NoEmptyDates) || comicBook.OpenedTime != DateTime.MinValue))
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.OpenedText, (object) comicBook.OpenedTimeAsText), ComicTextBuilder.GetSmallSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.Added) && (!flags.HasFlag((Enum) ComicTextElements.NoEmptyDates) || comicBook.AddedTime != DateTime.MinValue))
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.AddedText, (object) comicBook.AddedTimeAsText), ComicTextBuilder.GetSmallSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.FileFormat))
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.FormatText, (object) comicBook.FileFormat), ComicTextBuilder.GetSmallSmallFont(font), foreColor, StringFormatFlags.NoWrap);
        if (flags.HasFlag((Enum) ComicTextElements.FileName))
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.FileNameText, (object) comicBook.FileNameWithExtension), ComicTextBuilder.GetSmallSmallFont(font), foreColor, StringFormatFlags.NoWrap);
      }
    }

    public static IEnumerable<TextLine> GetTextBlocks(
      ComicPageInfo cpi,
      int page,
      Font font,
      Color foreColor,
      ComicTextElements flags)
    {
      if (cpi.IsEmpty)
      {
        yield return new TextLine(ComicTextBuilder.NoInformationAvailable, ComicTextBuilder.GetCaptionFont(font), foreColor);
      }
      else
      {
        yield return new TextLine(string.Format(ComicTextBuilder.PageText, (object) (page + 1)), ComicTextBuilder.GetCaptionFont(font), foreColor);
        yield return new TextLine(cpi.PageTypeAsText, ComicTextBuilder.GetSmallFont(font), foreColor);
        yield return new TextLine(10);
        if (cpi.ImageFileSize != 0)
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.PageSizeText, (object) cpi.ImageFileSizeAsText), ComicTextBuilder.GetSmallFont(font), foreColor);
        else
          yield return new TextLine(ComicTextBuilder.UnknownSizeText, ComicTextBuilder.GetSmallFont(font), foreColor);
        yield return new TextLine(StringUtility.Format(ComicTextBuilder.ResolutionText, (object) cpi.ImageWidthAsText, (object) cpi.ImageHeightAsText), ComicTextBuilder.GetSmallFont(font), foreColor);
        if (cpi.Rotation != ImageRotation.None)
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.RotationText, (object) cpi.RotationAsText), ComicTextBuilder.GetSmallFont(font), foreColor);
        yield return new TextLine(6);
        if (cpi.IsBookmark)
          yield return new TextLine(StringUtility.Format(ComicTextBuilder.BookmarkText, (object) cpi.Bookmark), ComicTextBuilder.GetSmallFont(font), foreColor);
      }
    }

    private static Font GetCaptionFont(Font font) => FC.Get(font, FontStyle.Bold);

    private static Font GetNormalFont(Font font) => font;

    private static Font GetSmallFont(Font font) => FC.GetRelative(font, 0.95f);

    private static Font GetSmallSmallFont(Font font) => FC.GetRelative(font, 0.9f);
  }
}

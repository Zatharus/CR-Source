// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicsEditModeExtension
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public static class ComicsEditModeExtension
  {
    public static bool IsLocalComic(this ComicsEditModes em) => (em & ComicsEditModes.Local) != 0;

    public static bool CanEditProperties(this ComicsEditModes em)
    {
      return (em & ComicsEditModes.EditProperties) != 0;
    }

    public static bool CanEditPages(this ComicsEditModes em)
    {
      return (em & ComicsEditModes.EditPages) != 0;
    }

    public static bool CanExport(this ComicsEditModes em)
    {
      return (em & ComicsEditModes.ExportComic) != 0;
    }

    public static bool CanDeleteComics(this ComicsEditModes em)
    {
      return (em & ComicsEditModes.DeleteComics) != 0;
    }

    public static bool CanShowComics(this ComicsEditModes em) => (em & ComicsEditModes.Local) != 0;

    public static bool CanEditList(this ComicsEditModes em)
    {
      return (em & ComicsEditModes.EditComicList) != 0;
    }

    public static bool CanScan(this ComicsEditModes em) => (em & ComicsEditModes.Rescan) != 0;
  }
}

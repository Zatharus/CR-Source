// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.NtfsInfoStorage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Win32;
using System;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public static class NtfsInfoStorage
  {
    public const string ComicBookInfoStream = "ComicRackInfo";

    public static bool StoreInfo(string file, ComicInfo comicInfo)
    {
      ComicInfo ci = NtfsInfoStorage.LoadInfo(file);
      if (comicInfo.IsSameContent(ci))
        return false;
      try
      {
        FileInfo fileInfo = new FileInfo(file);
        using (StreamWriter text = AlternateDataStreamFile.CreateText(file, "ComicRackInfo"))
        {
          comicInfo.Serialize(text.BaseStream);
          return true;
        }
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static ComicInfo LoadInfo(string file)
    {
      try
      {
        if (!AlternateDataStreamFile.Exists(file, "ComicRackInfo"))
          return (ComicInfo) null;
        using (StreamReader streamReader = AlternateDataStreamFile.OpenText(file, "ComicRackInfo"))
          return ComicInfo.Deserialize(streamReader.BaseStream);
      }
      catch (Exception ex)
      {
        return (ComicInfo) null;
      }
    }

    public static void ClearInfo(string file)
    {
      try
      {
        if (!AlternateDataStreamFile.Exists(file, "ComicRackInfo"))
          return;
        AlternateDataStreamFile.Delete(file, "ComicRackInfo");
      }
      catch (Exception ex)
      {
      }
    }
  }
}

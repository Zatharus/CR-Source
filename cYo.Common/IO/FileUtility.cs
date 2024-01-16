// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.FileUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace cYo.Common.IO
{
  public static class FileUtility
  {
    private static FileUtility.FileFolderAction SafeValidator(
      Func<string, bool, FileUtility.FileFolderAction> validator,
      string path,
      bool isPath)
    {
      return validator != null ? validator(path, isPath) : FileUtility.FileFolderAction.Default;
    }

    public static IEnumerable<string> GetFolders(string path, int levels)
    {
      if (FileUtility.SafeDirectoryExists(path))
      {
        foreach (string enumerateDirectory in Directory.EnumerateDirectories(path))
        {
          string sub = enumerateDirectory;
          yield return Path.Combine(path, sub);
          if (levels > 0)
          {
            foreach (string folder in FileUtility.GetFolders(sub, levels - 1))
              yield return folder;
            sub = (string) null;
          }
        }
      }
    }

    public static IEnumerable<string> GetFiles(
      string path,
      SearchOption searchOption,
      Func<string, bool, FileUtility.FileFolderAction> validator = null,
      params string[] extensions)
    {
      FileUtility.FileFolderAction pathAction = FileUtility.SafeValidator(validator, path, true);
      if (!pathAction.HasFlag((Enum) FileUtility.FileFolderAction.IgnoreFolder))
      {
        foreach (string file in new FileSystemEnumerable(path))
        {
          string checkFile = file;
          if (!FileUtility.SafeValidator(validator, checkFile, false).HasFlag((Enum) FileUtility.FileFolderAction.IgnoreFile) && (extensions == null || extensions.Length == 0 || ((IEnumerable<string>) extensions).Any<string>((Func<string, bool>) (ext => checkFile.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))))
            yield return file;
        }
        if (searchOption == SearchOption.AllDirectories && !pathAction.HasFlag((Enum) FileUtility.FileFolderAction.IgnoreSubFolders))
        {
          foreach (string path1 in new FileSystemEnumerable(path, FileSystemEnumeratorType.Folder))
          {
            foreach (string file in FileUtility.GetFiles(path1, searchOption, validator, extensions))
              yield return file;
          }
        }
      }
    }

    public static IEnumerable<string> GetFiles(
      string path,
      SearchOption searchOption,
      params string[] extensions)
    {
      return FileUtility.GetFiles(path, searchOption, (Func<string, bool, FileUtility.FileFolderAction>) null, extensions);
    }

    public static IEnumerable<string> GetFiles(
      IEnumerable<string> paths,
      SearchOption searchOption,
      params string[] extensions)
    {
      foreach (string path in paths)
      {
        foreach (string file in FileUtility.GetFiles(path, searchOption, (Func<string, bool, FileUtility.FileFolderAction>) null, extensions))
          yield return file;
      }
    }

    public static bool ForeachFile(
      string path,
      SearchOption searchOption,
      Predicate<string> action)
    {
      return FileUtility.GetFiles(path, searchOption).Any<string>((Func<string, bool>) (file => !action(file)));
    }

    public static bool ForeachFile(
      IEnumerable<string> paths,
      SearchOption searchOption,
      Predicate<string> action)
    {
      return FileUtility.GetFiles(paths, searchOption).Any<string>((Func<string, bool>) (file => !action(file)));
    }

    private static string NetMakeValidFilename(string name, char safe)
    {
      StringBuilder stringBuilder = new StringBuilder(name);
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
        stringBuilder.Replace(invalidFileNameChar, safe);
      return stringBuilder.ToString();
    }

    public static string MakeValidFilename(string name, char safe = '_')
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (char c in name)
        {
          int charType = FileUtility.Native.PathGetCharType(c);
          if (charType == 0 || (charType & 12) != 0)
            stringBuilder.Append(safe);
          else
            stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
      }
      catch (Exception ex)
      {
        return FileUtility.NetMakeValidFilename(name, safe);
      }
    }

    public static string GetSafeFileName(string file)
    {
      try
      {
        return Path.GetFileName(file);
      }
      catch (ArgumentException ex)
      {
        return file;
      }
    }

    public static bool CreateEmpty(string path)
    {
      try
      {
        using (File.CreateText(path))
          ;
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static long GetSize(string temp)
    {
      try
      {
        return new FileInfo(temp).Length;
      }
      catch (Exception ex)
      {
      }
      return 0;
    }

    public static bool SafeFileExists(string path)
    {
      try
      {
        return File.Exists(path);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool SafeDirectoryExists(string path)
    {
      try
      {
        return Directory.Exists(path);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static byte[] ReadAllBytes(this Stream s)
    {
      try
      {
        int count = (int) (s.Length - s.Position);
        byte[] buffer = new byte[count];
        return s.Read(buffer, 0, count) == count ? buffer : (byte[]) null;
      }
      catch
      {
        return (byte[]) null;
      }
    }

    public static void WriteStream(string file, Stream data)
    {
      using (Stream destination = (Stream) File.Create(file))
        data.CopyTo(destination);
    }

    public static DriveInfo GetDriveInfo(string path)
    {
      return ((IEnumerable<DriveInfo>) DriveInfo.GetDrives()).FirstOrDefault<DriveInfo>((Func<DriveInfo, bool>) (di => string.Equals(Path.GetPathRoot(path), di.RootDirectory.Name, StringComparison.OrdinalIgnoreCase)));
    }

    public static DriveType GetDriveType(string path)
    {
      DriveInfo driveInfo = FileUtility.GetDriveInfo(path);
      return driveInfo != null ? driveInfo.DriveType : DriveType.Unknown;
    }

    public static bool SafeDelete(string path)
    {
      if (string.IsNullOrEmpty(path))
        return false;
      try
      {
        File.Delete(path);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static bool SafeDirectoryDelete(string path, bool recursive = true)
    {
      try
      {
        Directory.Delete(path, recursive);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static IEnumerable<string> SafeGetFiles(
      string folder,
      string searchPattern = "*.*",
      SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
      try
      {
        return (IEnumerable<string>) Directory.GetFiles(folder, searchPattern, searchOption);
      }
      catch (Exception ex)
      {
        return Enumerable.Empty<string>();
      }
    }

    public static IEnumerable<string> ReadLines(this TextReader tr)
    {
      string line;
      while ((line = tr.ReadLine()) != null)
        yield return line;
    }

    public static IEnumerable<string> ReadLines(this Stream s)
    {
      using (StreamReader sw = new StreamReader(s))
      {
        foreach (string readLine in sw.ReadLines())
          yield return readLine;
      }
    }

    public static IEnumerable<string> ReadLines(string file)
    {
      using (StreamReader s = File.OpenText(file))
      {
        foreach (string readLine in s.ReadLines())
          yield return readLine;
      }
    }

    [Flags]
    public enum FileFolderAction
    {
      Default = 0,
      IgnoreFile = 1,
      IgnoreFolder = 2,
      IgnoreSubFolders = 4,
    }

    private static class Native
    {
      public const int GCT_INVALID = 0;
      public const int GCT_LFNCHAR = 1;
      public const int GCT_SHORTCHAR = 2;
      public const int GCT_WILD = 4;
      public const int GCT_SEPARATOR = 8;

      [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
      public static extern int PathGetCharType(char c);
    }
  }
}

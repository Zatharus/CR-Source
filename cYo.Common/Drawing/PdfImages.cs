// Decompiled with JetBrains decompiler
// Type: cYo.Common.Drawing.PdfImages
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.IO;
using cYo.Common.Runtime;
using cYo.Common.Win32;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Common.Drawing
{
  public class PdfImages
  {
    private const string GhostScriptWin32 = "gswin32c.exe";
    private const string GhostScriptWin64 = "gswin64c.exe";
    private string tempPath = Path.GetTempPath();
    private static readonly Regex rxCount = new Regex("file:\\s(?<count>\\d+)", RegexOptions.Compiled);
    private string pdfFile;
    private volatile int pageCount;
    private static string ghostscriptPath;
    private static bool searched;

    public PdfImages()
    {
    }

    public PdfImages(string pdfFile, string tempPath)
    {
      if (!string.IsNullOrEmpty(tempPath))
        this.tempPath = tempPath;
      this.Open(pdfFile);
    }

    public PdfImages(string pdfFile)
      : this(pdfFile, (string) null)
    {
    }

    public string TempPath
    {
      get => this.tempPath;
      set => this.tempPath = value;
    }

    public void Open(string pdfFile)
    {
      string input = PdfImages.ExecuteGhostscript("-q -dBATCH -dNOPAUSE -dSAFER -sDEVICE=nullpage -dFirstPage=100000 \"{0}\"", (object) pdfFile);
      Match match = PdfImages.rxCount.Match(input);
      if (!match.Success)
        throw new FileLoadException();
      this.PageCount = int.Parse(match.Groups["count"].Value);
      this.pdfFile = pdfFile;
    }

    public void Close()
    {
      this.pdfFile = (string) null;
      this.PageCount = 0;
    }

    public Bitmap GetPage(int page, int dpi)
    {
      byte[] pageData = this.GetPageData(page, dpi);
      try
      {
        return pageData == null ? (Bitmap) null : (Bitmap) Image.FromStream((Stream) new MemoryStream(pageData), false);
      }
      catch (Exception ex)
      {
        return (Bitmap) null;
      }
    }

    public byte[] GetPageData(int page, int dpi)
    {
      if (page >= this.PageCount)
        return (byte[]) null;
      string path = Path.Combine(this.TempPath, Guid.NewGuid().ToString() + ".tmp");
      try
      {
        PdfImages.ExecuteGhostscript("-r{0} -q -dBATCH -dNOPAUSE -dTextAlphaBits=4 -dGraphicsAlphaBits=4 -dUseCropBox -dUseTrimBox -dFIXEDRESOLUTION -sDEVICE=jpeg -dFirstPage={1} -dLastPage={1} -sOutputFile=\"{2}\" \"{3}\"", (object) dpi, (object) (page + 1), (object) path, (object) this.pdfFile);
        return File.ReadAllBytes(path);
      }
      catch
      {
        try
        {
          PdfImages.ExecuteGhostscript("-r{0} -q -dBATCH -dNOPAUSE -dTextAlphaBits=4 -dGraphicsAlphaBits=4 -dFIXEDRESOLUTION -sDEVICE=jpeg -dFirstPage={1} -dLastPage={1} -sOutputFile=\"{2}\" \"{3}\"", (object) dpi, (object) (page + 1), (object) path, (object) this.pdfFile);
          return File.ReadAllBytes(path);
        }
        catch
        {
          return (byte[]) null;
        }
      }
      finally
      {
        FileUtility.SafeDelete(path);
      }
    }

    public string PdfFile => this.pdfFile;

    public int PageCount
    {
      get => this.pageCount;
      set => this.pageCount = value;
    }

    private static string CheckRegistry(string key)
    {
      try
      {
        using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("Software\\" + key, false))
        {
          if (registryKey1 == null)
            return (string) null;
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey(registryKey1.GetSubKeyNames()[0]))
          {
            if (registryKey2 == null)
              return (string) null;
            string path = Path.Combine(Path.GetDirectoryName(registryKey2.GetValue("GS_DLL").ToString()), "gswin32c.exe");
            if (!File.Exists(path))
              path = Path.Combine(Path.GetDirectoryName(registryKey2.GetValue("GS_DLL").ToString()), "gswin64c.exe");
            if (!File.Exists(path))
              path = (string) null;
            return path;
          }
        }
      }
      catch
      {
        return (string) null;
      }
    }

    private static string CheckPath(string path)
    {
      return FileUtility.GetFiles(path, SearchOption.AllDirectories).FirstOrDefault<string>((Func<string, bool>) (s => Path.GetFileName(s).Equals("gswin32c.exe", StringComparison.OrdinalIgnoreCase))) ?? FileUtility.GetFiles(path, SearchOption.AllDirectories).FirstOrDefault<string>((Func<string, bool>) (s => Path.GetFileName(s).Equals("gswin64c.exe", StringComparison.OrdinalIgnoreCase)));
    }

    private static string CheckProgramPath(string path)
    {
      return Machine.Is64Bit ? PdfImages.CheckPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), path)) ?? PdfImages.CheckPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), path)) : PdfImages.CheckPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), path));
    }

    public static string GhostscriptPath
    {
      get
      {
        if (PdfImages.ghostscriptPath == null && !PdfImages.searched)
        {
          PdfImages.ghostscriptPath = PdfImages.CheckRegistry("GPL Ghostscript") ?? PdfImages.CheckRegistry("AFPL Ghostscript") ?? PdfImages.CheckProgramPath("gs") ?? PdfImages.CheckProgramPath("Ghostscript") ?? PdfImages.CheckProgramPath("GPL Ghostscript");
          PdfImages.searched = true;
        }
        return PdfImages.ghostscriptPath;
      }
      set => PdfImages.ghostscriptPath = value;
    }

    public static bool IsGhostscriptAvailable
    {
      get
      {
        try
        {
          return !string.IsNullOrEmpty(PdfImages.GhostscriptPath) && File.Exists(PdfImages.GhostscriptPath);
        }
        catch
        {
          return false;
        }
      }
    }

    private static string ExecuteGhostscript(string arguments, params object[] objs)
    {
      ExecuteProcess.Result result = ExecuteProcess.Execute(PdfImages.GhostscriptPath, string.Format(arguments, objs), ExecuteProcess.Options.StoreOutput);
      return result.ExitCode == 0 ? result.ConsoleText : throw new FileLoadException();
    }
  }
}

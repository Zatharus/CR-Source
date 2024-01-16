// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive.SevenZipEngine
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.ComponentModel;
using cYo.Common.Compression.SevenZip;
using cYo.Common.IO;
using cYo.Common.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers.Archive
{
  public class SevenZipEngine : FileBasedAccessor
  {
    private const int SevenZipCheckSize = 131072;
    public static readonly string PackExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\7z.exe");
    public static readonly string PackDll32 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\7z.dll");
    public static readonly string PackDll64 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\7z64.dll");
    private static readonly Regex rxList = new Regex("Path = (?<filename>.*)\\r\\n(.*\\r\\n)*?Size = (?<size>\\d+)", RegexOptions.Compiled);
    private bool libraryMode;
    private static SevenZipFactory sevenZipFactory;

    public SevenZipEngine(int format, bool libraryMode)
      : base(format)
    {
      this.libraryMode = libraryMode;
    }

    private static SevenZipFactory SevenZipFactory
    {
      get
      {
        if (SevenZipEngine.sevenZipFactory == null)
          SevenZipEngine.sevenZipFactory = new SevenZipFactory(Environment.Is64BitProcess ? SevenZipEngine.PackDll64 : SevenZipEngine.PackDll32);
        return SevenZipEngine.sevenZipFactory;
      }
    }

    public override bool IsFormat(string source)
    {
      if (this.HasSignature)
        return base.IsFormat(source);
      try
      {
        using (this.OpenArchive(source, out IInArchive _))
          return true;
      }
      catch
      {
        return false;
      }
    }

    public override IEnumerable<ProviderImageInfo> GetEntryList(string source)
    {
      if (this.libraryMode)
      {
        IInArchive archive;
        using (this.OpenArchive(source, out archive))
        {
          int count = archive.GetNumberOfItems();
          for (int i = 0; i < count; ++i)
          {
            PropVariant propVariant1 = new PropVariant();
            PropVariant propVariant2 = new PropVariant();
            archive.GetProperty(i, ItemPropId.kpidPath, ref propVariant1);
            archive.GetProperty(i, ItemPropId.kpidSize, ref propVariant2);
            yield return new ProviderImageInfo(i, propVariant1.GetObject().ToString(), propVariant2.longValue);
          }
        }
        archive = (IInArchive) null;
      }
      else
      {
        ExecuteProcess.Result result = ExecuteProcess.Execute(SevenZipEngine.PackExe, "l -slt \"" + FileMethods.GetShortName(source) + "\"", ExecuteProcess.Options.StoreOutput);
        foreach (ProviderImageInfo entry in SevenZipEngine.rxList.Matches(result.ConsoleText).OfType<Match>().Select<Match, ProviderImageInfo>((Func<Match, ProviderImageInfo>) (m => new ProviderImageInfo(0, m.Groups["filename"].Value, long.Parse(m.Groups["size"].Value)))))
          yield return entry;
      }
    }

    public override byte[] ReadByteImage(string source, ProviderImageInfo info)
    {
      return this.GetFileData(source, info);
    }

    private IDisposable OpenArchive(string source, out IInArchive archive)
    {
      IInArchive a = archive = SevenZipEngine.SevenZipFactory.CreateInArchive(SevenZipEngine.MapFileFormat(this.Format));
      InStreamWrapper archiveStream = new InStreamWrapper((Stream) File.OpenRead(source));
      long maxCheckStartPosition = 131072;
      if (archive.Open((IInStream) archiveStream, ref maxCheckStartPosition, (IArchiveOpenCallback) new StubOpenCallback()) != 0)
      {
        archiveStream.Dispose();
        throw new FileLoadException();
      }
      return (IDisposable) new Disposer((Action) (() =>
      {
        archiveStream.Dispose();
        a.Close();
        Marshal.ReleaseComObject((object) a);
      }));
    }

    private static byte[] GetFileData(IInArchive archive, int fileNumber)
    {
      MemoryStream ms;
      try
      {
        PropVariant propVariant = new PropVariant();
        archive.GetProperty(fileNumber, ItemPropId.kpidSize, ref propVariant);
        ms = new MemoryStream((int) propVariant.longValue);
      }
      catch (Exception ex)
      {
        ms = new MemoryStream();
      }
      archive.Extract(new int[1]{ fileNumber }, 1, 0, (IArchiveExtractCallback) new ExtractToStreamCallback(fileNumber, (Stream) ms));
      return ms.ToArray();
    }

    private byte[] GetFileData(string source, string file)
    {
      try
      {
        if (this.libraryMode)
        {
          IInArchive archive;
          using (this.OpenArchive(source, out archive))
          {
            int numberOfItems = archive.GetNumberOfItems();
            for (int index = 0; index < numberOfItems; ++index)
            {
              PropVariant propVariant = new PropVariant();
              archive.GetProperty(index, ItemPropId.kpidPath, ref propVariant);
              if (file.Equals(propVariant.GetObject().ToString()))
                return SevenZipEngine.GetFileData(archive, index);
            }
          }
        }
        else
        {
          ExecuteProcess.Result result = ExecuteProcess.Execute(SevenZipEngine.PackExe, "e -so \"" + FileMethods.GetShortName(source) + "\" \"" + file + "\"", ExecuteProcess.Options.StoreOutput);
          if (result.ExitCode == 0)
            return result.Output;
        }
      }
      catch
      {
      }
      return (byte[]) null;
    }

    private byte[] GetFileData(string source, ProviderImageInfo ii)
    {
      if (!this.libraryMode)
        return this.GetFileData(source, ii.Name);
      try
      {
        IInArchive archive;
        using (this.OpenArchive(source, out archive))
          return SevenZipEngine.GetFileData(archive, ii.Index);
      }
      catch
      {
        return (byte[]) null;
      }
    }

    public override ComicInfo ReadInfo(string source)
    {
      try
      {
        using (MemoryStream inStream = new MemoryStream(this.GetFileData(source, "ComicInfo.xml")))
          return ComicInfo.Deserialize((Stream) inStream);
      }
      catch
      {
        return (ComicInfo) null;
      }
    }

    public override bool WriteInfo(string source, ComicInfo comicInfo)
    {
      return SevenZipEngine.UpdateComicInfo(source, this.Format, comicInfo);
    }

    private static KnownSevenZipFormat MapFileFormat(int format)
    {
      switch (format)
      {
        case 2:
          return KnownSevenZipFormat.Zip;
        case 3:
          return KnownSevenZipFormat.Rar;
        case 5:
          return KnownSevenZipFormat.Tar;
        case 6:
          return KnownSevenZipFormat.SevenZip;
        default:
          throw new NotSupportedException("Type if not supported");
      }
    }

    public static bool UpdateComicInfo(string file, int format, ComicInfo comicInfo)
    {
      bool flag;
      string str1;
      switch (format)
      {
        case 2:
          flag = false;
          str1 = "zip";
          break;
        case 5:
          flag = false;
          str1 = "tar";
          break;
        case 6:
          flag = true;
          str1 = "7z";
          break;
        default:
          return false;
      }
      try
      {
        if (flag)
        {
          string parameters = string.Format("u -t{0} -siComicInfo.xml \"{1}\"", (object) str1, (object) file);
          if (ExecuteProcess.Execute(SevenZipEngine.PackExe, parameters, comicInfo.ToArray(), (string) null, ExecuteProcess.Options.None).ExitCode == 0)
            return true;
        }
        else
        {
          string str2 = Path.Combine(EngineConfiguration.Default.TempPath, Guid.NewGuid().ToString());
          string path = Path.Combine(str2, "ComicInfo.xml");
          try
          {
            Directory.CreateDirectory(str2);
            using (Stream outStream = (Stream) File.Create(path))
              comicInfo.Serialize(outStream);
            string parameters = string.Format("u -t{0} \"{1}\" \"{2}\"", (object) str1, (object) file, (object) path);
            if (ExecuteProcess.Execute(SevenZipEngine.PackExe, parameters, (string) null, ExecuteProcess.Options.None).ExitCode == 0)
              return true;
          }
          finally
          {
            try
            {
              FileUtility.SafeDelete(path);
              Directory.Delete(str2);
            }
            catch
            {
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.ComicExporter
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Text;
using cYo.Common.Win32;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Engine.IO.Provider.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  public class ComicExporter
  {
    private ExportSetting setting;
    private readonly List<ComicBook> comicBooks;
    private ComicInfo comicInfo;
    private volatile string lastError;
    private readonly int sequence;

    public ComicExporter(IEnumerable<ComicBook> books, ExportSetting setting, int sequence)
    {
      this.comicBooks = books.ToList<ComicBook>();
      this.comicInfo = CombinedComics.GetComicInfo((IEnumerable<ComicBook>) this.comicBooks);
      this.setting = setting;
      this.sequence = sequence;
    }

    public ExportSetting Setting => this.setting;

    public ComicBook ComicBook => this.comicBooks[0];

    public List<ComicBook> ComicBooks => this.comicBooks;

    public ComicInfo ComicInfo => this.comicInfo;

    public string LastError => this.lastError;

    public int Sequence => this.sequence;

    public string Export(IPagePool pagePool)
    {
      try
      {
        string targetPath = this.setting.GetTargetPath(this.ComicBook, this.sequence);
        if (File.Exists(targetPath) && !this.setting.Overwrite && this.setting.Target != ExportTarget.ReplaceSource)
          throw new InvalidOperationException(StringUtility.Format(TR.Messages["OutputFileExists", "Output file '{0}' already exists!"], (object) targetPath));
        if ((this.setting.AddToLibrary || this.setting.Target == ExportTarget.ReplaceSource) && Providers.Readers.GetFormatProviderType(this.setting.FormatId) == (Type) null)
          throw new ArgumentException(TR.Messages["InvalidExportSettings", "The export settings do not match (e.g. adding a not supported format to the library)"]);
        if (this.setting.ImageProcessingSource == ExportImageProcessingSource.FromComic)
        {
          this.setting = CloneUtility.Clone<ExportSetting>(this.setting);
          this.setting.ImageProcessing = this.ComicBook.ColorAdjustment.ChangeOption(this.setting.ImageProcessing.Options);
          BitmapAdjustment imageProcessing = this.setting.ImageProcessing;
          if (imageProcessing.Sharpen != 0)
          {
            ExportSetting setting = this.setting;
            imageProcessing = this.setting.ImageProcessing;
            BitmapAdjustment bitmapAdjustment = imageProcessing.ChangeSharpness(this.setting.ImageProcessing.Sharpen);
            setting.ImageProcessing = bitmapAdjustment;
          }
        }
        using (IImageProvider provider = CombinedComics.OpenProvider((IEnumerable<ComicBook>) this.comicBooks, pagePool))
        {
          FileFormat fileFormat = this.setting.GetFileFormat(this.ComicBook);
          string str = (string) null;
          try
          {
            using (StorageProvider formatProvider = Providers.Writers.CreateFormatProvider(fileFormat.Name))
            {
              if (formatProvider == null)
                return (string) null;
              try
              {
                formatProvider.Progress += new EventHandler<StorageProgressEventArgs>(this.writer_Progress);
                if (File.Exists(targetPath))
                {
                  str = EngineConfiguration.Default.GetTempFileName();
                  this.comicInfo = formatProvider.Store(provider, this.comicInfo, str, (StorageSetting) this.setting);
                  ShellFile.DeleteFile(targetPath);
                  File.Move(str, targetPath);
                }
                else
                  this.comicInfo = formatProvider.Store(provider, this.comicInfo, targetPath, (StorageSetting) this.setting);
              }
              finally
              {
                formatProvider.Progress -= new EventHandler<StorageProgressEventArgs>(this.writer_Progress);
              }
            }
          }
          catch
          {
            FileUtility.SafeDelete(str ?? targetPath);
            throw;
          }
        }
        return targetPath;
      }
      catch (Exception ex)
      {
        this.lastError = ex.Message;
        throw;
      }
    }

    private void writer_Progress(object sender, StorageProgressEventArgs e) => this.OnProgress(e);

    public event EventHandler<StorageProgressEventArgs> Progress;

    protected virtual void OnProgress(StorageProgressEventArgs e)
    {
      if (this.Progress == null)
        return;
      this.Progress((object) this, e);
    }
  }
}

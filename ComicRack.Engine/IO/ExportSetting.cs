// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.ExportSetting
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO
{
  [Serializable]
  public class ExportSetting : StorageSetting
  {
    public ExportSetting()
    {
      this.ImageProcessingSource = ExportImageProcessingSource.Custom;
      this.CustomNamingStart = 1;
      this.Naming = ExportNaming.Filename;
      this.Target = ExportTarget.NewFolder;
    }

    [XmlAttribute]
    public string Name { get; set; }

    public static string DefaultName => TR.Default["New", "New"];

    [DefaultValue(ExportTarget.NewFolder)]
    public ExportTarget Target { get; set; }

    [DefaultValue(null)]
    public string TargetFolder { get; set; }

    [DefaultValue(false)]
    public bool DeleteOriginal { get; set; }

    [DefaultValue(false)]
    public bool AddToLibrary { get; set; }

    [DefaultValue(false)]
    public bool Overwrite { get; set; }

    [DefaultValue(false)]
    public bool Combine { get; set; }

    [DefaultValue(ExportNaming.Filename)]
    public ExportNaming Naming { get; set; }

    [DefaultValue(null)]
    public string CustomName { get; set; }

    [DefaultValue(1)]
    public int CustomNamingStart { get; set; }

    [DefaultValue(null)]
    public string ForcedName { get; set; }

    [DefaultValue(ExportImageProcessingSource.Custom)]
    public ExportImageProcessingSource ImageProcessingSource { get; set; }

    public FileFormat GetFileFormat(ComicBook cb)
    {
      try
      {
        return Providers.Writers.GetSourceFormats().First<FileFormat>((Func<FileFormat, bool>) (ff => this.FormatId != 0 ? ff.Id == this.FormatId : ff.Name == cb.FileFormat));
      }
      catch (Exception ex)
      {
        return Providers.Writers.GetSourceFormats().First<FileFormat>((Func<FileFormat, bool>) (ff => ff.Id == 2));
      }
    }

    public string GetTargetFilePath(ComicBook cb)
    {
      return this.Target != ExportTarget.NewFolder ? Path.GetDirectoryName(cb.FilePath) : this.TargetFolder;
    }

    public string GetTargetFileName(ComicBook cb, int index)
    {
      if (!string.IsNullOrEmpty(this.ForcedName))
        return this.ForcedName;
      FileFormat fileFormat = this.GetFileFormat(cb);
      string name;
      switch (this.Naming)
      {
        case ExportNaming.Caption:
          name = string.IsNullOrEmpty(this.CustomName) ? cb.TargetFilename : cb.GetFullTitle(this.CustomName);
          break;
        case ExportNaming.Custom:
          name = string.IsNullOrEmpty(this.CustomName) ? cb.FileName : this.CustomName;
          index += this.CustomNamingStart;
          if (index > 0)
          {
            name = string.Format("{0} ({1})", (object) name, (object) index);
            break;
          }
          break;
        default:
          name = cb.FileName;
          break;
      }
      return FileUtility.MakeValidFilename(name) + fileFormat.MainExtension;
    }

    public string GetTargetPath(ComicBook cb, int index)
    {
      return Path.Combine(this.GetTargetFilePath(cb), this.GetTargetFileName(cb, index));
    }

    public static ExportSetting ConvertToCBZ
    {
      get
      {
        ExportSetting convertToCbz = new ExportSetting();
        convertToCbz.Name = TR.Messages[nameof (ConvertToCBZ), "Convert to CBZ"];
        convertToCbz.Target = ExportTarget.ReplaceSource;
        convertToCbz.FormatId = 2;
        return convertToCbz;
      }
    }

    public static ExportSetting ConvertToCB7
    {
      get
      {
        ExportSetting convertToCb7 = new ExportSetting();
        convertToCb7.Name = TR.Messages[nameof (ConvertToCB7), "Convert to CB7"];
        convertToCb7.Target = ExportTarget.ReplaceSource;
        convertToCb7.FormatId = 6;
        return convertToCb7;
      }
    }
  }
}

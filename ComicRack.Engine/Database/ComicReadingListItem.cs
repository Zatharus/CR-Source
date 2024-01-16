// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.ComicReadingListItem
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database
{
  public class ComicReadingListItem
  {
    public ComicReadingListItem()
    {
      this.Id = Guid.Empty;
      this.Volume = -1;
      this.Year = -1;
      this.Series = string.Empty;
      this.FileName = string.Empty;
      this.Format = string.Empty;
      this.Number = string.Empty;
    }

    public ComicReadingListItem(ComicBook cb, bool withFilename)
    {
      this.Series = cb.ShadowSeries;
      this.Number = cb.ShadowNumber;
      this.Volume = cb.ShadowVolume;
      this.Year = cb.ShadowYear;
      this.Format = cb.ShadowFormat;
      this.Id = cb.Id;
      this.FileName = withFilename ? cb.FileName : string.Empty;
    }

    [XmlAttribute]
    [DefaultValue("")]
    public string Series { get; set; }

    [XmlAttribute]
    [DefaultValue("")]
    public string Number { get; set; }

    [XmlAttribute]
    [DefaultValue(-1)]
    public int Volume { get; set; }

    [XmlAttribute]
    [DefaultValue(-1)]
    public int Year { get; set; }

    [XmlAttribute]
    [DefaultValue("")]
    public string Format { get; set; }

    public Guid Id { get; set; }

    [DefaultValue("")]
    public string FileName { get; set; }

    public override string ToString()
    {
      return !string.IsNullOrEmpty(this.Series) ? ComicBook.FormatTitle("[{format} ][{series}][ {volume}][ #{number}][ - {title}][ ({year}[/{month}[/{day}]])]", this.Series, volumeText: ComicBook.FormatVolume(this.Volume), numberText: this.Number, yearText: ComicBook.FormatYear(this.Year), format: this.Format, fileName: this.FileName) : this.FileName;
    }

    public void SetInfo(ComicNameInfo cni, bool onlyEmpty = false)
    {
      if (!onlyEmpty || string.IsNullOrEmpty(this.Series))
        this.Series = cni.Series;
      if (!onlyEmpty || string.IsNullOrEmpty(this.Number))
        this.Number = cni.Number;
      if (!onlyEmpty || this.Volume == -1)
        this.Volume = cni.Volume;
      if (!onlyEmpty || this.Year == -1)
        this.Year = cni.Year;
      if (onlyEmpty && !string.IsNullOrEmpty(this.Format))
        return;
      this.Format = cni.Format;
    }

    public void SetFileNameInfo()
    {
      if (string.IsNullOrEmpty(this.FileName))
        return;
      this.SetInfo(ComicNameInfo.FromFilePath(this.FileName));
    }
  }
}

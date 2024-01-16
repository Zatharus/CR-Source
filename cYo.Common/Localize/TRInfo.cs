// Decompiled with JetBrains decompiler
// Type: cYo.Common.Localize.TRInfo
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Globalization;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Localize
{
  public class TRInfo
  {
    private float completionPercent = 100f;

    public TRInfo()
    {
    }

    public TRInfo(string language) => this.CultureName = language;

    public string ApplicationName { get; set; }

    public string ApplicationVersion { get; set; }

    public string Author { get; set; }

    public string Notes { get; set; }

    public bool RightToLeft { get; set; }

    [XmlElement("Language")]
    public string CultureName { get; set; }

    public string DisplayLanguage
    {
      get
      {
        return this.CultureName == null ? TR.Default["System", "System"] : new CultureInfo(this.CultureName).DisplayName;
      }
    }

    [XmlIgnore]
    public float CompletionPercent
    {
      get => this.completionPercent;
      set => this.completionPercent = value;
    }

    public override string ToString()
    {
      string str = this.DisplayLanguage;
      if (!string.IsNullOrEmpty(this.Author))
        str = string.Format("{0} ({1})", (object) str, (object) this.Author);
      int completionPercent = (int) this.completionPercent;
      if (completionPercent != 100)
        str += string.Format("\t[{0:###}%]", (object) completionPercent);
      return str;
    }
  }
}

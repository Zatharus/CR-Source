// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.Readers.DjvuComicProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider.Readers
{
  [FileFormat("DjVu Document", 8, ".djvu")]
  public class DjvuComicProvider : ComicProvider, IValidateProvider
  {
    private static readonly string ListExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\djvm.exe");
    private static readonly Regex rxList = new Regex("(?<size>\\d+)\\s+PAGE\\s+#(?<page>\\d+)\\s+(?<name>.*)\\r", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private ProviderImageInfo[] pages;

    public override ImageProviderCapabilities Capabilities
    {
      get => ImageProviderCapabilities.FastFormatCheck;
    }

    protected override bool OnFastFormatCheck(string source)
    {
      try
      {
        using (FileStream fileStream = File.OpenRead(source))
        {
          byte[] numArray = new byte[10];
          fileStream.Read(numArray, 0, numArray.Length);
          return DjVuImage.IsDjvu(numArray);
        }
      }
      catch
      {
        return true;
      }
    }

    public override string CreateHash()
    {
      using (FileStream inputStream = File.OpenRead(this.Source))
        return Base32.ToBase32String(new SHA1Managed().ComputeHash((Stream) inputStream));
    }

    protected override void OnParse()
    {
      foreach (ProviderImageInfo page in this.GetPages())
        this.FireIndexReady(page);
    }

    protected override byte[] OnRetrieveSourceByteImage(int index)
    {
      using (Bitmap bitmap = DjVuImage.GetBitmap(this.Source, index))
        return bitmap.ImageToJpegBytes();
    }

    private IEnumerable<ProviderImageInfo> GetPages()
    {
      using (ItemMonitor.Lock((object) this))
      {
        if (this.pages == null)
          this.pages = this.ReadPages().ToArray<ProviderImageInfo>();
        return (IEnumerable<ProviderImageInfo>) this.pages;
      }
    }

    private IEnumerable<ProviderImageInfo> ReadPages()
    {
      ExecuteProcess.Result result = ExecuteProcess.Execute(DjvuComicProvider.ListExe, string.Format("-l \"{0}\"", (object) this.Source), ExecuteProcess.Options.StoreOutput);
      return DjvuComicProvider.rxList.Matches(result.ConsoleText).Cast<Match>().Select<Match, ProviderImageInfo>((Func<Match, ProviderImageInfo>) (m => new ProviderImageInfo(int.Parse(m.Groups["page"].Value) - 1, m.Groups["name"].Value, long.Parse(m.Groups["size"].Value))));
    }

    public bool IsValid => File.Exists(DjvuComicProvider.ListExe);
  }
}

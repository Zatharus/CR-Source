// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.DriveChecker
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class DriveChecker
  {
    private readonly Dictionary<string, bool> cache = new Dictionary<string, bool>();

    public bool IsConnected(string path)
    {
      string lower = Path.GetPathRoot(path).ToLower();
      bool flag;
      if (!this.cache.TryGetValue(lower, out flag))
      {
        try
        {
          flag = Directory.Exists(lower);
        }
        catch
        {
          flag = false;
        }
        this.cache[lower] = flag;
      }
      return flag;
    }
  }
}

﻿// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.IVirtualFolder
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public interface IVirtualFolder
  {
    Stream OpenRead(string path);

    Stream Create(string path);

    bool FileExists(string path);

    bool CreateFolder(string path);

    IEnumerable<string> GetFiles(string path);
  }
}
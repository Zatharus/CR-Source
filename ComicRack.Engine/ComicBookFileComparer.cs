﻿// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ComicBookFileComparer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Text;
using System.Collections.Generic;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ComicBookFileComparer : Comparer<ComicBook>
  {
    public override int Compare(ComicBook x, ComicBook y)
    {
      return ExtendedStringComparer.Compare(x.FilePath, y.FilePath, ExtendedStringComparison.IgnoreCase);
    }
  }
}

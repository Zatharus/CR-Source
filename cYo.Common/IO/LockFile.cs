﻿// Decompiled with JetBrains decompiler
// Type: cYo.Common.IO.LockFile
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System.IO;

#nullable disable
namespace cYo.Common.IO
{
  public class LockFile : DisposableObject
  {
    private readonly string file;

    public LockFile(string file)
    {
      this.file = file;
      if (File.Exists(file))
      {
        this.WasLocked = true;
      }
      else
      {
        using (File.Create(file))
          ;
      }
    }

    public bool WasLocked { get; private set; }

    protected override void Dispose(bool disposing)
    {
      FileUtility.SafeDelete(this.file);
      base.Dispose(disposing);
    }
  }
}
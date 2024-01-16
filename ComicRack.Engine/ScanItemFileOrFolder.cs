// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.ScanItemFileOrFolder
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.IO;
using cYo.Common.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class ScanItemFileOrFolder : ScanItem
  {
    private readonly string fileOrFolder;
    private readonly bool all = true;

    public ScanItemFileOrFolder(string fileOrFolder, bool all, bool removeMissing)
    {
      this.fileOrFolder = fileOrFolder;
      this.AutoRemove = removeMissing;
      this.all = all;
    }

    public override IEnumerable<string> GetScanFiles()
    {
      if (!File.Exists(this.fileOrFolder))
        return FileUtility.GetFiles(this.fileOrFolder, this.all ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, new Func<string, bool, FileUtility.FileFolderAction>(ScanItemFileOrFolder.ValidateFolder));
      return (IEnumerable<string>) new string[1]
      {
        this.fileOrFolder
      };
    }

    public override string ToString() => this.fileOrFolder;

    private static FileUtility.FileFolderAction ValidateFolder(string path, bool isPath)
    {
      if (!isPath)
        return FileUtility.FileFolderAction.Default;
      string str = Path.Combine(path, "comicrackscanner.ini");
      return !File.Exists(str) ? FileUtility.FileFolderAction.Default : IniFile.GetValue<FileUtility.FileFolderAction>(str, "options", FileUtility.FileFolderAction.Default);
    }
  }
}

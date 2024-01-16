// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.DiskDriveSyncProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public class DiskDriveSyncProvider : SyncProviderBase
  {
    private readonly string syncPath;
    private readonly string rootPath;

    public DiskDriveSyncProvider(string rootPath, string deviceKey = null)
    {
      DriveInfo driveInfo = FileUtility.GetDriveInfo(rootPath);
      if (driveInfo == null || !driveInfo.IsReady)
        throw new ArgumentException();
      this.rootPath = rootPath;
      this.syncPath = FileUtility.GetFolders(rootPath, 3).FirstOrDefault<string>((Func<string, bool>) (fullPath => FileUtility.SafeFileExists(Path.Combine(fullPath, "comicrack.ini"))));
      if (this.syncPath == null)
        throw new DriveNotFoundException();
      if (!this.ReadMarkerFile(deviceKey))
        throw new DriveNotFoundException();
    }

    protected override void OnStart()
    {
    }

    protected override void OnCompleted()
    {
    }

    protected override bool FileExists(string file) => File.Exists(this.GetFullPath(file));

    protected override void WriteFile(string file, Stream data)
    {
      FileUtility.WriteStream(this.GetFullPath(file), data);
    }

    protected override Stream ReadFile(string fileName)
    {
      return (Stream) File.OpenRead(this.GetFullPath(fileName));
    }

    protected override void DeleteFile(string fileName)
    {
      FileUtility.SafeDelete(this.GetFullPath(fileName));
    }

    protected override long GetFreeSpace()
    {
      return FileUtility.GetDriveInfo(this.rootPath).AvailableFreeSpace;
    }

    protected override IEnumerable<string> GetFileList()
    {
      return Directory.EnumerateFiles(this.syncPath).Select<string, string>(new Func<string, string>(Path.GetFileName));
    }

    private string GetFullPath(string fileName) => Path.Combine(this.syncPath, fileName);

    public static IEnumerable<DriveInfo> GetRemoveableDrives()
    {
      return ((IEnumerable<string>) Environment.GetLogicalDrives()).Select<string, DriveInfo>(new Func<string, DriveInfo>(FileUtility.GetDriveInfo)).Where<DriveInfo>((Func<DriveInfo, bool>) (di => di.IsReady && di.DriveType == DriveType.Removable));
    }
  }
}

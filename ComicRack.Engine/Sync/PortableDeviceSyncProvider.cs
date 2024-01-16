// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.PortableDeviceSyncProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Win32.PortableDevices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public class PortableDeviceSyncProvider : SyncProviderBase
  {
    private const int maxFolderLevel = 10;
    private readonly cYo.Common.Win32.PortableDevices.Device device;
    private readonly DeviceFolder syncFolder;

    public PortableDeviceSyncProvider(string deviceNode, string deviceKey = null)
    {
      this.device = DeviceFactory.GetDevice(deviceNode);
      this.syncFolder = this.device.Find(Regex.Escape("comicrack.ini"), 10).Parent;
      if (this.syncFolder == null)
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

    protected override bool FileExists(string file) => this.syncFolder.FileExists(file);

    protected override IEnumerable<string> GetFileList()
    {
      return this.syncFolder.Items.OfType<DeviceFile>().Select<DeviceFile, string>((Func<DeviceFile, string>) (item => item.Name));
    }

    protected override void WriteFile(string fileName, Stream data)
    {
      this.syncFolder.WriteFile(fileName, data);
    }

    protected override Stream ReadFile(string fileName)
    {
      return this.syncFolder.GetFile(fileName).ReadFile();
    }

    protected override void DeleteFile(string fileName)
    {
      this.syncFolder.GetFile(fileName)?.Delete();
    }

    protected override long GetFreeSpace() => this.syncFolder.FreeSpace;
  }
}

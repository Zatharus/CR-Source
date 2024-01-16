// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.DeviceSyncError
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public class DeviceSyncError
  {
    public DeviceSyncError()
    {
    }

    public DeviceSyncError(string name, string message)
    {
      this.Name = name;
      this.Message = message;
    }

    public string Name { get; private set; }

    public string Message { get; private set; }
  }
}

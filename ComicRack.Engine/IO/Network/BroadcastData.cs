// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.BroadcastData
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System.ComponentModel;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  public class BroadcastData
  {
    public BroadcastData()
      : this(BroadcastType.ServerStarted)
    {
    }

    public BroadcastData(BroadcastType broadcastType, string serverName = null, int serverPort = 0)
    {
      this.BroadcastType = broadcastType;
      this.ServerName = serverName;
      this.ServerPort = serverPort;
    }

    [DefaultValue(null)]
    public string ServerName { get; set; }

    [DefaultValue(0)]
    public int ServerPort { get; set; }

    [DefaultValue(BroadcastType.ServerStarted)]
    public BroadcastType BroadcastType { get; set; }
  }
}

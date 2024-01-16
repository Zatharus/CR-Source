// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.NetworkManager
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Net;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Network;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine
{
  public class NetworkManager : DisposableObject
  {
    public const int BroadcastPort = 7613;
    private cYo.Common.Net.Broadcaster<BroadcastData> broadcaster;
    private readonly SmartList<ComicLibraryServer> runningServers = new SmartList<ComicLibraryServer>();
    private readonly Dictionary<string, ShareInformation> localShares = new Dictionary<string, ShareInformation>();

    public NetworkManager(
      DatabaseManager databaseManager,
      CacheManager cacheManager,
      ISharesSettings settings,
      int privatePort,
      int publicPort,
      bool disableBroadcast)
    {
      this.DatabaseManager = databaseManager;
      this.CacheManager = cacheManager;
      this.Settings = settings;
      this.PrivatePort = privatePort;
      this.PublicPort = publicPort;
      this.DisableBroadcast = disableBroadcast;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Stop();
        this.Broadcaster.SafeDispose();
      }
      base.Dispose(disposing);
    }

    public DatabaseManager DatabaseManager { get; private set; }

    public CacheManager CacheManager { get; private set; }

    public int PrivatePort { get; set; }

    public int PublicPort { get; set; }

    public bool DisableBroadcast { get; set; }

    public ISharesSettings Settings { get; private set; }

    public cYo.Common.Net.Broadcaster<BroadcastData> Broadcaster
    {
      get
      {
        if (this.broadcaster == null && !this.DisableBroadcast)
          this.broadcaster = new cYo.Common.Net.Broadcaster<BroadcastData>(7613);
        return this.broadcaster;
      }
    }

    public bool IsOwnServer(string serverAddress)
    {
      return this.runningServers.Any<ComicLibraryServer>((Func<ComicLibraryServer, bool>) (rs => rs.GetAnnouncementUri() == serverAddress));
    }

    public SmartList<ComicLibraryServer> RunningServers => this.runningServers;

    public Dictionary<string, ShareInformation> LocalShares => this.localShares;

    public bool HasActiveServers() => this.runningServers.Count > 0;

    public bool RecentServerActivity(int seconds = 10)
    {
      return this.runningServers.Any<ComicLibraryServer>((Func<ComicLibraryServer, bool>) (s => s.Statistics.WasActive(seconds)));
    }

    public event EventHandler<NetworkManager.RemoteServerStartedEventArgs> RemoteServerStarted;

    public event EventHandler<NetworkManager.RemoteServerStoppedEventArgs> RemoteServerStopped;

    public void BroadcastStart()
    {
      if (this.Broadcaster == null)
        return;
      this.Broadcaster.Broadcast(new BroadcastData(BroadcastType.ClientStarted));
    }

    public void BroadcastStop()
    {
      if (this.Broadcaster == null)
        return;
      this.Broadcaster.Broadcast(new BroadcastData(BroadcastType.ClientStopped));
    }

    public void Start()
    {
      ComicLibraryServer.ExternalServerAddress = this.Settings.ExternalServerAddress;
      if (this.Broadcaster != null)
      {
        this.Broadcaster.Listen = true;
        this.Broadcaster.Recieved += new EventHandler<BroadcastEventArgs<BroadcastData>>(this.BroadcasterRecieved);
      }
      foreach (ComicLibraryServerConfig share in this.Settings.Shares)
      {
        share.OnlyPrivateConnections = !share.IsInternet && this.PrivatePort == this.PublicPort;
        share.PrivateListPassword = !share.IsInternet || !share.IsPrivate ? string.Empty : this.Settings.PrivateListingPassword;
      }
      this.runningServers.AddRange(ComicLibraryServer.Start(this.Settings.Shares.Where<ComicLibraryServerConfig>((Func<ComicLibraryServerConfig, bool>) (sc => !sc.IsInternet)), this.PrivatePort, (Func<ComicLibrary>) (() => (ComicLibrary) this.DatabaseManager.Database), (IPagePool) this.CacheManager.ImagePool, (IThumbnailPool) this.CacheManager.ImagePool, (IBroadcast<BroadcastData>) this.Broadcaster));
      this.runningServers.AddRange(ComicLibraryServer.Start(this.Settings.Shares.Where<ComicLibraryServerConfig>((Func<ComicLibraryServerConfig, bool>) (sc => sc.IsInternet)), this.PublicPort, (Func<ComicLibrary>) (() => (ComicLibrary) this.DatabaseManager.Database), (IPagePool) this.CacheManager.ImagePool, (IThumbnailPool) this.CacheManager.ImagePool, (IBroadcast<BroadcastData>) this.Broadcaster));
    }

    public void Stop()
    {
      if (this.Broadcaster != null)
      {
        this.Broadcaster.Recieved -= new EventHandler<BroadcastEventArgs<BroadcastData>>(this.BroadcasterRecieved);
        this.Broadcaster.Listen = false;
      }
      this.runningServers.ForEach((Action<ComicLibraryServer>) (s => s.Stop()));
      this.runningServers.Clear();
    }

    private void BroadcasterRecieved(object sender, BroadcastEventArgs<BroadcastData> e)
    {
      if (this.Broadcaster == null)
        return;
      switch (e.Data.BroadcastType)
      {
        case BroadcastType.ClientStarted:
          using (IEnumerator<ComicLibraryServer> enumerator = this.runningServers.Where<ComicLibraryServer>((Func<ComicLibraryServer, bool>) (si => !si.Config.IsInternet)).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ComicLibraryServer current = enumerator.Current;
              this.Broadcaster.Broadcast(new BroadcastData(BroadcastType.ServerStarted, current.Config.ServiceName, current.Config.ServicePort));
            }
            break;
          }
        case BroadcastType.ServerStarted:
          string str1 = ServiceAddress.Append(e.Address, e.Data.ServerPort.ToString(), e.Data.ServerName);
          using (ItemMonitor.Lock((object) this.localShares))
          {
            if (this.localShares.ContainsKey(str1))
              break;
          }
          ShareInformation serverInfo = ComicLibraryClient.GetServerInfo(str1);
          if (serverInfo == null)
            break;
          using (ItemMonitor.Lock((object) this.localShares))
          {
            serverInfo.IsLocal = true;
            this.localShares[str1] = serverInfo;
          }
          if (!this.Settings.LookForShared || this.RemoteServerStarted == null)
            break;
          this.RemoteServerStarted((object) this, new NetworkManager.RemoteServerStartedEventArgs(serverInfo));
          break;
        case BroadcastType.ServerStopped:
          string str2 = ServiceAddress.Append(e.Address, e.Data.ServerPort.ToString(), e.Data.ServerName);
          using (ItemMonitor.Lock((object) this.localShares))
            this.localShares.Remove(str2);
          if (this.RemoteServerStopped == null)
            break;
          this.RemoteServerStopped((object) this, new NetworkManager.RemoteServerStoppedEventArgs(str2));
          break;
      }
    }

    public class RemoteServerStartedEventArgs : EventArgs
    {
      public RemoteServerStartedEventArgs(ShareInformation information)
      {
        this.Information = information;
      }

      public ShareInformation Information { get; private set; }
    }

    public class RemoteServerStoppedEventArgs : EventArgs
    {
      public RemoteServerStoppedEventArgs(string address) => this.Address = address;

      public string Address { get; private set; }
    }
  }
}

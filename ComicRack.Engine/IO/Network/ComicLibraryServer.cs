// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.ComicLibraryServer
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Drawing;
using cYo.Common.Net;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Engine.Properties;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Prefix)]
  public class ComicLibraryServer : IRemoteComicLibrary, IRemoteServerInfo, IDisposable
  {
    public const string InfoPoint = "Info";
    public const string LibraryPoint = "Library";
    public const int ServerPingTime = 10000;
    public const int ServerAnnounceTime = 300000;
    public static string ExternalServerAddress;
    private static X509Certificate2 certificate;
    private System.Threading.Timer pingTimer;
    private System.Threading.Timer announceTimer;
    private readonly Func<ComicLibrary> getComicLibrary;
    private bool serverHasBeenAnnounced;
    private bool serverHasBeenValidated;
    private bool serverValidationFailed;
    private ServiceHost serviceHost;
    private readonly cYo.Common.Collections.Cache<Guid, IImageProvider> providerCache = new cYo.Common.Collections.Cache<Guid, IImageProvider>(EngineConfiguration.Default.ServerProviderCacheSize);
    private static readonly ServerRegistration serverRegistration = new ServerRegistration();
    private static readonly Dictionary<int, int> shareCounts = new Dictionary<int, int>();

    public static X509Certificate2 Certificate
    {
      get
      {
        if (ComicLibraryServer.certificate == null)
          ComicLibraryServer.certificate = new X509Certificate2(Resources.Certificate, string.Empty);
        return ComicLibraryServer.certificate;
      }
    }

    public ComicLibraryServer(
      ComicLibraryServerConfig config,
      Func<ComicLibrary> getComicLibrary,
      IPagePool pagePool,
      IThumbnailPool thumbPool,
      IBroadcast<BroadcastData> broadcaster)
    {
      this.Id = Guid.NewGuid().ToString();
      this.Config = CloneUtility.Clone<ComicLibraryServerConfig>(config);
      this.Statistics = new ServerStatistics();
      this.getComicLibrary = getComicLibrary;
      this.PagePool = pagePool;
      this.ThumbPool = thumbPool;
      this.Broadcaster = broadcaster;
      this.PingEnabled = true;
      this.providerCache.ItemRemoved += new EventHandler<CacheItemEventArgs<Guid, IImageProvider>>(this.providerCache_ItemRemoved);
    }

    public void Dispose()
    {
      this.Stop();
      this.providerCache.Dispose();
    }

    public string Id { get; private set; }

    public ComicLibraryServerConfig Config { get; private set; }

    public IBroadcast<BroadcastData> Broadcaster { get; private set; }

    public bool PingEnabled { get; set; }

    public IPagePool PagePool { get; set; }

    public IThumbnailPool ThumbPool { get; set; }

    public ComicLibrary ComicLibrary => this.getComicLibrary();

    public ServerStatistics Statistics { get; private set; }

    public bool IsRunning => this.serviceHost != null;

    public bool IsAnnounced => this.serverHasBeenAnnounced;

    string IRemoteServerInfo.Id
    {
      get
      {
        this.CheckPrivateNetwork();
        this.AddStats(ServerStatistics.StatisticType.InfoRequest);
        return this.Id;
      }
    }

    string IRemoteServerInfo.Name => this.Config.Name;

    string IRemoteServerInfo.Description => this.Config.Description;

    ServerOptions IRemoteServerInfo.Options => this.Config.Options;

    public bool IsValid
    {
      get
      {
        try
        {
          this.CheckPrivateNetwork();
          return true;
        }
        catch (Exception ex)
        {
          return false;
        }
      }
    }

    byte[] IRemoteComicLibrary.GetLibraryData()
    {
      this.CheckPrivateNetwork();
      try
      {
        byte[] byteArray = this.GetSharedComicLibrary().ToByteArray();
        this.AddStats(ServerStatistics.StatisticType.LibraryRequest, byteArray.Length);
        return byteArray;
      }
      catch (Exception ex)
      {
        return (byte[]) null;
      }
    }

    int IRemoteComicLibrary.GetImageCount(Guid comicGuid)
    {
      ComicBook book = this.ComicLibrary.Books[comicGuid];
      try
      {
        if (book.PageCount > 0)
          return book.PageCount;
        using (IItemLock<IImageProvider> itemLock = this.providerCache.LockItem(comicGuid, new Func<Guid, IImageProvider>(this.CreateProvider)))
        {
          book.PageCount = itemLock.Item.Count;
          return book.PageCount;
        }
      }
      catch (Exception ex)
      {
        return 0;
      }
    }

    byte[] IRemoteComicLibrary.GetImage(Guid comicGuid, int index)
    {
      this.CheckPrivateNetwork();
      ComicBook book = this.ComicLibrary.Books[comicGuid];
      try
      {
        index = book.TranslateImageIndexToPage(index);
        using (IItemLock<IImageProvider> itemLock = this.providerCache.LockItem(comicGuid, new Func<Guid, IImageProvider>(this.CreateProvider)))
        {
          using (IItemLock<PageImage> page = this.PagePool.GetPage(book.GetPageKey(index, BitmapAdjustment.Empty), itemLock.Item, true))
          {
            int pageQuality = this.Config.PageQuality;
            byte[] image = pageQuality != 100 ? page.Item.Bitmap.ImageToJpegBytes(75 * pageQuality / 100) : (byte[]) page.Item.Data.Clone();
            this.AddStats(ServerStatistics.StatisticType.PageRequest, image.Length);
            return image;
          }
        }
      }
      catch (Exception ex)
      {
        return (byte[]) null;
      }
    }

    byte[] IRemoteComicLibrary.GetThumbnailImage(Guid comicGuid, int index)
    {
      ComicBook book = this.ComicLibrary.Books[comicGuid];
      try
      {
        index = book.TranslateImageIndexToPage(index);
        using (IItemLock<IImageProvider> itemLock = this.providerCache.LockItem(comicGuid, new Func<Guid, IImageProvider>(this.CreateProvider)))
        {
          using (IItemLock<ThumbnailImage> thumbnail = this.ThumbPool.GetThumbnail(book.GetThumbnailKey(index), itemLock.Item, true))
          {
            int thumbnailQuality = this.Config.ThumbnailQuality;
            byte[] thumbnailImage = thumbnailQuality != 100 ? new ThumbnailImage(thumbnail.Item.Bitmap.ImageToJpegBytes(75 * thumbnailQuality / 100), thumbnail.Item.Size, thumbnail.Item.OriginalSize).ToBytes() : thumbnail.Item.ToBytes();
            this.AddStats(ServerStatistics.StatisticType.ThumbnailRequest, thumbnailImage.Length);
            return thumbnailImage;
          }
        }
      }
      catch (Exception ex)
      {
        return (byte[]) null;
      }
    }

    void IRemoteComicLibrary.UpdateComic(Guid comicGuid, string propertyName, object value)
    {
      if (!this.Config.IsEditable)
        return;
      try
      {
        this.ComicLibrary.Books[comicGuid]?.SetValue(propertyName, value);
      }
      catch (Exception ex)
      {
      }
    }

    public string GetAnnouncementUri()
    {
      return !this.Config.IsInternet ? (string) null : ServiceAddress.CompletePortAndPath(ComicLibraryServer.GetExternalServiceAddress(), this.Config.ServicePort == 7612 ? (string) null : this.Config.ServicePort.ToString(), this.Config.ServiceName == "Share" ? (string) null : this.Config.ServiceName);
    }

    public void AnnounceServer()
    {
      string uri = this.GetAnnouncementUri();
      if (string.IsNullOrEmpty(uri))
        return;
      if (this.serverValidationFailed)
        return;
      try
      {
        ThreadUtility.RunInBackground("Announce Server", (ThreadStart) (() =>
        {
          if (!this.serverHasBeenValidated)
          {
            this.serverHasBeenValidated = true;
            this.serverValidationFailed = ComicLibraryClient.GetServerId(uri) != this.Id;
            if (this.serverValidationFailed)
              return;
          }
          try
          {
            ComicLibraryServer.serverRegistration.Register(uri, this.Config.Name, this.Config.Description ?? string.Empty, (int) this.Config.Options, this.Config.PrivateListPassword);
            this.serverHasBeenAnnounced = true;
          }
          catch (Exception ex)
          {
          }
        }));
      }
      catch (Exception ex)
      {
      }
    }

    public void AnnouncedServerRefresh() => this.AnnounceServer();

    public void AnnouncedServerRemove()
    {
      if (!this.serverHasBeenAnnounced)
        return;
      string announcementUri = this.GetAnnouncementUri();
      try
      {
        ComicLibraryServer.serverRegistration.Unregister(announcementUri);
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this.serverHasBeenAnnounced = false;
      }
    }

    public void CheckPrivateNetwork()
    {
      if (this.Config.OnlyPrivateConnections && !ComicLibraryServer.GetClientIp().IsPrivate())
        throw new AuthenticationException("Only clients in private network can connect");
    }

    public void AddStats(ServerStatistics.StatisticType type, int size = 0)
    {
      this.Statistics.Add(ComicLibraryServer.GetClientIp().ToString(), type, size);
    }

    public bool Start()
    {
      try
      {
        if (this.IsRunning)
          this.Stop();
        if (!this.Config.IsValidShare)
          return false;
        this.serviceHost = new ServiceHost((object) this, new Uri[1]
        {
          new Uri(string.Format("net.tcp://localhost:{0}/{1}", (object) this.Config.ServicePort, (object) this.Config.ServiceName))
        });
        this.serviceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
        this.serviceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = (UserNamePasswordValidator) new ComicLibraryServer.PasswordValidator(this.Config.ProtectionPassword);
        this.serviceHost.Credentials.ServiceCertificate.Certificate = new X509Certificate2((X509Certificate) ComicLibraryServer.Certificate);
        this.serviceHost.Credentials.IssuedTokenAuthentication.KnownCertificates.Add(new X509Certificate2((X509Certificate) ComicLibraryServer.Certificate));
        this.serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
        this.serviceHost.AddServiceEndpoint(typeof (IRemoteServerInfo), ComicLibraryServer.CreateChannel(false), "Info").Binding.SendTimeout = TimeSpan.FromSeconds((double) EngineConfiguration.Default.OperationTimeout);
        this.serviceHost.AddServiceEndpoint(typeof (IRemoteComicLibrary), ComicLibraryServer.CreateChannel(true), "Library").Binding.SendTimeout = TimeSpan.FromSeconds((double) EngineConfiguration.Default.OperationTimeout);
        this.serviceHost.Open();
        if (this.Config.IsInternet)
        {
          this.AnnounceServer();
          this.announceTimer = new System.Threading.Timer(new TimerCallback(this.ServerAnnounce), (object) null, 300000, 300000);
        }
        else
        {
          if (this.Broadcaster != null)
            this.Broadcaster.Broadcast(new BroadcastData(BroadcastType.ServerStarted, this.Config.ServiceName, this.Config.ServicePort));
          this.pingTimer = new System.Threading.Timer(new TimerCallback(this.ServerPing), (object) null, 10000, 10000);
        }
        return true;
      }
      catch (Exception ex)
      {
        this.Stop();
        return false;
      }
    }

    public void Stop()
    {
      if (!this.IsRunning)
        return;
      this.AnnouncedServerRemove();
      try
      {
        this.announceTimer.SafeDispose();
        this.announceTimer = (System.Threading.Timer) null;
        this.pingTimer.SafeDispose();
        this.pingTimer = (System.Threading.Timer) null;
        if (this.Broadcaster != null)
          this.Broadcaster.Broadcast(new BroadcastData(BroadcastType.ServerStopped, this.Config.ServiceName, this.Config.ServicePort));
        this.serviceHost.Close();
      }
      catch
      {
      }
      finally
      {
        this.serviceHost = (ServiceHost) null;
      }
    }

    private void ServerPing(object state)
    {
      if (!this.PingEnabled || this.Broadcaster == null)
        return;
      this.Broadcaster.Broadcast(new BroadcastData(BroadcastType.ServerStarted, this.Config.ServiceName, this.Config.ServicePort));
    }

    private void ServerAnnounce(object state) => this.AnnouncedServerRefresh();

    private ComicLibrary GetSharedComicLibrary()
    {
      switch (this.Config.LibraryShareMode)
      {
        case LibraryShareMode.All:
          return ComicLibrary.Attach(this.ComicLibrary);
        case LibraryShareMode.Selected:
          ComicLibrary comicLibrary = new ComicLibrary();
          comicLibrary.Name = this.ComicLibrary.Name;
          comicLibrary.Id = this.ComicLibrary.Id;
          ComicLibrary sharedComicLibrary = comicLibrary;
          HashSet<ComicBook> comicBookSet = new HashSet<ComicBook>();
          IEnumerable<ShareableComicListItem> source = this.ComicLibrary.ComicLists.GetItems<ShareableComicListItem>().Where<ShareableComicListItem>((Func<ShareableComicListItem, bool>) (scli => this.Config.SharedItems.Contains(scli.Id)));
          sharedComicLibrary.ComicLists.AddRange((IEnumerable<ComicListItem>) source.Select<ShareableComicListItem, ComicIdListItem>((Func<ShareableComicListItem, ComicIdListItem>) (scli => new ComicIdListItem((ComicListItem) scli))));
          comicBookSet.AddRange<ComicBook>(source.SelectMany<ShareableComicListItem, ComicBook>((Func<ShareableComicListItem, IEnumerable<ComicBook>>) (scli => scli.GetBooks())));
          sharedComicLibrary.Books.AddRange(comicBookSet.Select<ComicBook, ComicBook>((Func<ComicBook, ComicBook>) (cb => new ComicBook(cb))));
          return sharedComicLibrary;
        default:
          return new ComicLibrary();
      }
    }

    private void providerCache_ItemRemoved(
      object sender,
      CacheItemEventArgs<Guid, IImageProvider> e)
    {
      e.Item.Dispose();
    }

    private IImageProvider CreateProvider(Guid comicGuid)
    {
      return (IImageProvider) this.ComicLibrary.Books[comicGuid].OpenProvider();
    }

    public static Binding CreateChannel(bool secure)
    {
      NetTcpBinding channel = new NetTcpBinding();
      channel.Security.Mode = secure ? SecurityMode.Message : SecurityMode.None;
      if (secure)
        channel.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
      channel.MaxReceivedMessageSize = 100000000L;
      channel.ReaderQuotas.MaxArrayLength = 100000000;
      return (Binding) channel;
    }

    public static IEnumerable<ShareInformation> GetPublicServers(
      ServerOptions optionsMask,
      string password)
    {
      return ((IEnumerable<ServerInfo>) HttpAccess.CallSoap<ServerRegistration, ServerInfo[]>(ComicLibraryServer.serverRegistration, (Func<ServerRegistration, ServerInfo[]>) (s => s.GetList((int) optionsMask, password)))).Select<ServerInfo, ShareInformation>((Func<ServerInfo, ShareInformation>) (s => (ShareInformation) s));
    }

    public static string GetExternalServiceAddress()
    {
      string externalServiceAddress = (ComicLibraryServer.ExternalServerAddress ?? string.Empty).Trim();
      try
      {
        if (string.IsNullOrEmpty(externalServiceAddress))
          externalServiceAddress = ServiceAddress.GetWanAddress();
      }
      catch (Exception ex)
      {
        externalServiceAddress = (string) null;
      }
      return externalServiceAddress;
    }

    public static IEnumerable<ComicLibraryServer> Start(
      IEnumerable<ComicLibraryServerConfig> servers,
      int port,
      Func<ComicLibrary> getComicLibrary,
      IPagePool pagePool,
      IThumbnailPool thumbPool,
      IBroadcast<BroadcastData> broadcaster)
    {
      foreach (ComicLibraryServerConfig config in servers.Where<ComicLibraryServerConfig>((Func<ComicLibraryServerConfig, bool>) (c => c.IsValidShare)))
      {
        int freeShareNumber = ComicLibraryServer.GetFreeShareNumber(port);
        config.ServicePort = port;
        config.ServiceName = "Share" + (freeShareNumber > 0 ? (freeShareNumber + 1).ToString() : string.Empty);
        ComicLibraryServer comicLibraryServer = new ComicLibraryServer(config, getComicLibrary, pagePool, thumbPool, broadcaster);
        if (comicLibraryServer.Start())
          yield return comicLibraryServer;
      }
    }

    private static int GetFreeShareNumber(int port)
    {
      int num;
      if (!ComicLibraryServer.shareCounts.TryGetValue(port, out num))
        num = -1;
      return ComicLibraryServer.shareCounts[port] = num + 1;
    }

    public static IPAddress GetClientIp()
    {
      IPAddress address;
      return !IPAddress.TryParse((OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty).Address, out address) ? (IPAddress) null : address;
    }

    private class PasswordValidator : UserNamePasswordValidator
    {
      private readonly string password;

      public PasswordValidator(string password) => this.password = password;

      public override void Validate(string userName, string password)
      {
        if (!string.IsNullOrEmpty(this.password) && this.password != password)
          throw new SecurityTokenException("Validation Failed!");
      }
    }
  }
}

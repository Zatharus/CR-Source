// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.ComicLibraryClient
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Net;
using cYo.Common.Threading;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  public class ComicLibraryClient : DisposableObject
  {
    private IRemoteComicLibrary remoteLibrary;
    private ProcessingQueue<string> queue;

    private ComicLibraryClient(string serviceAddress, ShareInformation information)
    {
      serviceAddress = ServiceAddress.CompletePortAndPath(serviceAddress, 7612.ToString(), "Share");
      if (information == null)
      {
        IRemoteServerInfo serverInfoService = ComicLibraryClient.GetServerInfoService(serviceAddress);
        information = new ShareInformation()
        {
          Id = serverInfoService.Id,
          Name = serverInfoService.Name,
          Comment = serverInfoService.Description,
          Options = serverInfoService.Options
        };
      }
      information.Uri = serviceAddress;
      this.ShareInformation = information;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.queue.SafeDispose();
      base.Dispose(disposing);
    }

    public bool Connect()
    {
      try
      {
        this.remoteLibrary = ComicLibraryClient.GetComicLibraryService(this.ShareInformation.Uri, this.Password);
        return this.remoteLibrary.IsValid;
      }
      catch (MessageSecurityException ex)
      {
        this.RemoteLibrary = (IRemoteComicLibrary) null;
        return false;
      }
      catch
      {
        this.RemoteLibrary = (IRemoteComicLibrary) null;
        throw;
      }
    }

    public IRemoteComicLibrary RemoteLibrary
    {
      get
      {
        IServiceChannel remoteLibrary = this.remoteLibrary as IServiceChannel;
        if (remoteLibrary.State == CommunicationState.Faulted || remoteLibrary.State == CommunicationState.Closed)
          this.Connect();
        return this.remoteLibrary;
      }
      set => this.remoteLibrary = value;
    }

    public string Password { get; set; }

    public ShareInformation ShareInformation { get; private set; }

    public ComicLibrary GetRemoteLibrary()
    {
      try
      {
        ComicLibrary remoteLibrary = ComicLibrary.FromByteArray(this.RemoteLibrary.GetLibraryData());
        remoteLibrary.EditMode = this.ShareInformation.IsEditable ? ComicsEditModes.EditProperties : ComicsEditModes.None;
        foreach (ComicBook book in (SmartList<ComicBook>) remoteLibrary.Books)
        {
          book.FileInfoRetrieved = true;
          book.ComicInfoIsDirty = false;
          book.SetFileLocation(string.Format("REMOTE:{0}\\{1}", (object) remoteLibrary.Id, (object) book.FilePath));
          book.CreateComicProvider += new EventHandler<CreateComicProviderEventArgs>(this.CreateComicProvider);
          if (this.ShareInformation.IsEditable)
            book.BookChanged += new EventHandler<BookChangedEventArgs>(this.ComicPropertyChanged);
        }
        if (this.ShareInformation.IsExportable)
          remoteLibrary.EditMode |= ComicsEditModes.ExportComic;
        remoteLibrary.IsLoaded = true;
        remoteLibrary.IsDirty = false;
        return remoteLibrary;
      }
      catch (Exception ex)
      {
        return (ComicLibrary) null;
      }
    }

    private void UpdateComic(ComicBook cb, string propertyName, object value)
    {
      Guid id = cb.Id;
      string str = string.Format("{0}:{1}", (object) id, (object) propertyName);
      if (this.queue == null)
        this.queue = new ProcessingQueue<string>("Server Book Info Update", ThreadPriority.Highest);
      this.queue.AddItem(str, (AsyncCallback) (ar =>
      {
        try
        {
          this.RemoteLibrary.UpdateComic(id, propertyName, value);
        }
        catch
        {
        }
      }));
    }

    private void CreateComicProvider(object sender, CreateComicProviderEventArgs e)
    {
      ComicBook comicBook = sender as ComicBook;
      e.Provider = (ImageProvider) new RemoteComicBookProvider(comicBook.Id, this);
    }

    private void ComicPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Pages")
        return;
      ComicBook cb = sender as ComicBook;
      try
      {
        this.UpdateComic(cb, e.PropertyName, cb.GetUntypedPropertyValue(e.PropertyName));
      }
      catch (Exception ex)
      {
      }
    }

    private static IRemoteComicLibrary GetComicLibraryService(string address, string password)
    {
      EndpointAddress remoteAddress = new EndpointAddress(new Uri(string.Format("net.tcp://{0}/{1}", (object) address, (object) "Library")), EndpointIdentity.CreateDnsIdentity("ComicRack"), (AddressHeaderCollection) null);
      ChannelFactory<IRemoteComicLibrary> channelFactory = new ChannelFactory<IRemoteComicLibrary>(ComicLibraryServer.CreateChannel(true), remoteAddress);
      channelFactory.Credentials.UserName.UserName = "ComicRack";
      channelFactory.Credentials.UserName.Password = password;
      channelFactory.Credentials.ClientCertificate.Certificate = ComicLibraryServer.Certificate;
      channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
      IRemoteComicLibrary channel = channelFactory.CreateChannel();
      ((IContextChannel) channel).OperationTimeout = TimeSpan.FromSeconds((double) EngineConfiguration.Default.OperationTimeout);
      return channel;
    }

    private static IRemoteServerInfo GetServerInfoService(string serviceAddress)
    {
      string remoteAddress = string.Format("net.tcp://{0}/{1}", (object) serviceAddress, (object) "Info");
      IRemoteServerInfo channel = new ChannelFactory<IRemoteServerInfo>(ComicLibraryServer.CreateChannel(false), remoteAddress).CreateChannel();
      ((IContextChannel) channel).OperationTimeout = TimeSpan.FromSeconds((double) EngineConfiguration.Default.OperationTimeout);
      return channel;
    }

    public static ComicLibraryClient Connect(string address, ShareInformation information)
    {
      try
      {
        return new ComicLibraryClient(address, information);
      }
      catch (Exception ex)
      {
        return (ComicLibraryClient) null;
      }
    }

    public static ComicLibraryClient Connect(string address)
    {
      return ComicLibraryClient.Connect(address, (ShareInformation) null);
    }

    public static ComicLibraryClient Connect(ShareInformation info)
    {
      return ComicLibraryClient.Connect(info.Uri, info);
    }

    public static ShareInformation GetServerInfo(string address)
    {
      try
      {
        using (ComicLibraryClient comicLibraryClient = ComicLibraryClient.Connect(address))
          return comicLibraryClient.ShareInformation;
      }
      catch (Exception ex)
      {
        return (ShareInformation) null;
      }
    }

    public static string GetServerId(string address)
    {
      return ComicLibraryClient.GetServerInfo(address)?.Id;
    }
  }
}

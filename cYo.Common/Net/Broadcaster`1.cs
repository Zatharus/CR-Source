// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.Broadcaster`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

#nullable disable
namespace cYo.Common.Net
{
  public class Broadcaster<T> : DisposableObject, IBroadcast<T>
  {
    private int port;
    private bool listen;
    private UdpClient listener;
    private IPEndPoint listenerEP;

    public Broadcaster()
    {
    }

    public Broadcaster(int port) => this.port = port;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.StopListening();
      base.Dispose(disposing);
    }

    public int Port
    {
      get => this.port;
      set
      {
        if (this.port == value)
          return;
        this.port = value;
        if (!this.listen)
          return;
        this.StopListening();
        this.StartListening();
      }
    }

    public bool Listen
    {
      get => this.listen;
      set
      {
        if (this.listen == value)
          return;
        this.listen = value;
        if (this.listen)
          this.StartListening();
        else
          this.StopListening();
      }
    }

    public IEnumerable<IPEndPoint> LocalEndpoints
    {
      get
      {
        return ((IEnumerable<IPAddress>) Dns.GetHostAddresses(string.Empty)).Where<IPAddress>((Func<IPAddress, bool>) (ipa => ipa.AddressFamily == AddressFamily.InterNetwork)).Select<IPAddress, IPEndPoint>((Func<IPAddress, IPEndPoint>) (ipa => new IPEndPoint(ipa, 0)));
      }
    }

    private bool StartListening()
    {
      if (this.listener != null)
        return true;
      try
      {
        this.listener = new UdpClient(this.port);
        this.listenerEP = new IPEndPoint(IPAddress.Any, this.port);
        this.listener.EnableBroadcast = true;
        this.listener.BeginReceive(new AsyncCallback(this.OnReceivedData), (object) this);
        return true;
      }
      catch (Exception ex)
      {
        this.StopListening();
        return false;
      }
    }

    private void StopListening()
    {
      try
      {
        UdpClient listener = this.listener;
        if (listener == null)
          return;
        this.listener = (UdpClient) null;
        listener.Close();
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this.listener = (UdpClient) null;
      }
    }

    protected virtual void OnReceivedData(IAsyncResult ar)
    {
      try
      {
        if (this.listener == null)
          return;
        try
        {
          byte[] bytes = this.listener.EndReceive(ar, ref this.listenerEP);
          if (this.LocalEndpoints.FirstOrDefault<IPEndPoint>((Func<IPEndPoint, bool>) (ep => ep.Address.Equals((object) this.listenerEP.Address))) != null)
            return;
          this.OnRecieved(new BroadcastEventArgs<T>(XmlUtility.Load<T>(bytes), this.listenerEP.Address));
        }
        catch (Exception ex)
        {
        }
        finally
        {
          this.listener.BeginReceive(new AsyncCallback(this.OnReceivedData), (object) this);
        }
      }
      catch
      {
      }
    }

    protected virtual void OnRecieved(BroadcastEventArgs<T> bea)
    {
      if (this.Recieved == null)
        return;
      this.Recieved((object) this, bea);
    }

    public event EventHandler<BroadcastEventArgs<T>> Recieved;

    public bool Broadcast(T data)
    {
      try
      {
        foreach (IPEndPoint localEndpoint in this.LocalEndpoints)
        {
          using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
          {
            socket.Bind((EndPoint) localEndpoint);
            socket.EnableBroadcast = true;
            socket.SendTo(XmlUtility.Store((object) data, true), (EndPoint) new IPEndPoint(IPAddress.Broadcast, this.port));
          }
        }
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}

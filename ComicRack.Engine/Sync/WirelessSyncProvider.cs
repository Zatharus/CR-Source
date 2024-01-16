// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Sync.WirelessSyncProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common;
using cYo.Common.Text;
using cYo.Common.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Sync
{
  public class WirelessSyncProvider : SyncProviderBase
  {
    private const string AndroidDebugKey = "3082030d308201f5a0030201020204494d03a7300d06092a864886f70d01010b05003037310b30090603550406130255533110300e060355040a1307416e64726f6964311630140603550403130d416e64726f6964204465627567301e170d3135303732323134353830375a170d3435303731343134353830375a3037310b30090603550406130255533110300e060355040a1307416e64726f6964311630140603550403130d416e64726f696420446562756730820122300d06092a864886f70d01010105000382010f003082010a02820101009ebd1f327aa7fd9d5c556df9e09ce4d7f091b04ffe649bf0c286fcd7d2efb24c485f02b4518d08227285d1758f0e6ba44ec3d2dd16a53d34f790452c2b25166db2488ac8de275cbe575325a4f19a476e23cd0831e7b05bb728525500e516bb24b20444ce79ec5625cf5963e4b792f8c5017fc36b880b8b78750bdcace2d4e25aee155aab3a4bb1c1c9a539a73edfc1057d77080e85c9506b033ffc72efe2c418f91171b78899be0d9fb04e5befd57cae955d7a81aff4573362d74571bd84d8ca5502cf99ad3124a6dfe45428b38c50300af28c13006803feadfc3aed84027b5fc4d0350ff41612652368f49b49c0461ff845b9c1e1bea440df4f65805f582b150203010001a321301f301d0603551d0e0416041490edd92d5d54c07225eb32807acdd2bd57a169a4300d06092a864886f70d01010b050003820101000932afee5e88d0b1252c84d1d9b8533c5d57fbd0e4766c53ce0f6565e04fe06c4c23687e109d9e23569736f3ee2105c57630b3b66229d25d910293c3d615e81290e932ecf321cc59a2dbecb4acf89a811cd63f10611b01d546f2aea1a23f259bb7f4e833117396e2e62c28a331d5d9a3fb625e199438635dd65e2da46fd4687aea161e9a490a597e264573be8821e2f3e7826df68dfee333301d968d154636497e76851f838df16d9d428b390b5b19f7b6ddbdbb1e19395f349f169764f38c114a91eaa831195a8e2c6217d1ac385ca4f6301f385fe44bdb82bbf6cd64033cd54e334500cec523e6d5abcc3f6bed5538ad7830ec45a897b1752f3695063e4853";
    private const string AndroidKey = "3082019d30820106a00302010202044e7b1922300d06092a864886f70d010105050030133111300f060355040a130863596f20536f6674301e170d3131303932323131313635305a170d3336303931353131313635305a30133111300f060355040a130863596f20536f667430819f300d06092a864886f70d010101050003818d00308189028181008d81ffb74008a048c517275a464db26461df06a3c85675b6ffa8bea15ec9288eec1ef1bf7616d09b7265bf1c5666473342c2a96ca385769592d73a21595335e5173c69ae5bb7aebd29387e9635ce30bdf11afff71145570b6577799ecac6100bcf0b2c4df6fe34fb8a418b5511c6a56c97b15c544269e91478ee24633ef063090203010001300d06092a864886f70d010105050003818100802cf8770c7af0744f9680b54da88b56eb1d6a48e8d446ce746817fe959991dc1f882323c6015edd4d48f28cfe3a94e30b75c92855b01a8c48354aae8e13a0b949390133c07b09419ac73d0b0b3dc13b2838fe9eae4b171c8022cb47ead602771560277cde7ad61e1a9ce5dee880d0226ed8cc71f36fb376d271a3cb61f1128b";
    private const int CurrentSyncVersion = 1;
    private const int DeviceClientPort = 7614;
    private const int BroadcastListenPort = 7615;
    private const int FirstServerControlPort = 7620;
    private const int CommandListFiles = 0;
    private const int CommandReadFile = 1;
    private const int CommandFreeSpace = 2;
    private const int CommandFileExists = 3;
    private const int CommandDeleteFile = 4;
    private const int CommandWriteFile = 5;
    private const int CommandStart = 6;
    private const int CommandCompleted = 7;
    private const int CommandProgressUpdate = 8;
    private const int CommandInfo = 9;
    private const int CommandReadMultiFile = 10;
    private const int CommandCheckAbort = 11;
    private const int CommandClientPong = 12;
    private const int CommandServerAvailable = 13;
    private readonly IPAddress address;
    private static readonly HashSet<IPAddress> foundDevices = new HashSet<IPAddress>();
    private static UdpClient listener;
    private static IPEndPoint listenerEP;
    private static Socket controlSocket;
    private static int controlPort;
    private static System.Threading.Timer serverAvailableTimer;

    public WirelessSyncProvider(IPAddress address, string deviceKey = null)
    {
      this.address = address;
      if (!this.ReadMarkerFile(deviceKey))
        throw new DriveNotFoundException();
    }

    protected override void OnStart()
    {
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 6);
        WirelessSyncProvider.SendString(s, "Start Synchronizing");
      }));
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 9);
        WirelessSyncProvider.SendInteger(s, 1);
        bool licensed = WirelessSyncProvider.ReadBool(s);
        int versionCode = WirelessSyncProvider.ReadInteger(s);
        string key = WirelessSyncProvider.ReadString(s);
        WirelessSyncProvider.ReadBool(s);
        this.ValidateWifi(versionCode, licensed, key);
      }));
    }

    protected override bool OnProgress(int percent)
    {
      this.Communicate((Action<Socket>) (s => s.Send(new byte[2]
      {
        (byte) 8,
        (byte) percent
      })));
      bool abort = false;
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 11);
        abort = WirelessSyncProvider.ReadBool(s);
      }));
      return !abort;
    }

    protected override void OnCompleted()
    {
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 7);
        WirelessSyncProvider.SendString(s, "Synchronization completed");
      }));
    }

    protected override bool FileExists(string file)
    {
      bool fileExists = false;
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 3);
        WirelessSyncProvider.SendString(s, file);
        fileExists = WirelessSyncProvider.ReadBool(s);
      }));
      return fileExists;
    }

    protected override void WriteFile(string file, Stream data)
    {
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 5);
        WirelessSyncProvider.SendString(s, file);
        WirelessSyncProvider.SendLong(s, data.Length);
        byte[] buffer = new byte[100000];
        int size;
        while ((size = data.Read(buffer, 0, buffer.Length)) > 0)
          s.Send(buffer, size, SocketFlags.None);
        if (!WirelessSyncProvider.ReadBool(s))
          throw new IOException();
      }));
    }

    protected override Stream ReadFile(string file)
    {
      MemoryStream ms = (MemoryStream) null;
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 1);
        WirelessSyncProvider.SendString(s, file);
        ms = new MemoryStream(WirelessSyncProvider.ReadSocketData(s));
      }));
      return (Stream) ms;
    }

    protected override void DeleteFile(string file)
    {
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 4);
        WirelessSyncProvider.SendString(s, file);
        WirelessSyncProvider.ReadBool(s);
      }));
    }

    protected override long GetFreeSpace()
    {
      long freeSpace = 0;
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 2);
        freeSpace = WirelessSyncProvider.ReadLong(s);
      }));
      return freeSpace;
    }

    protected override IEnumerable<string> GetFileList()
    {
      string[] fileList = (string[]) null;
      this.Communicate((Action<Socket>) (s =>
      {
        WirelessSyncProvider.SendByte(s, (byte) 0);
        fileList = ((IEnumerable<string>) WirelessSyncProvider.ReadString(s).Split("\n", StringSplitOptions.RemoveEmptyEntries)).TrimStrings().ToArray<string>();
      }));
      return (IEnumerable<string>) fileList;
    }

    private void ValidateWifi(int versionCode, bool licensed, string key)
    {
      bool flag = licensed && this.Device.Version == versionCode;
      switch (this.Device.Edition)
      {
        case SyncAppEdition.AndroidFull:
          flag = ((flag ? 1 : 0) & (key == "3082030d308201f5a0030201020204494d03a7300d06092a864886f70d01010b05003037310b30090603550406130255533110300e060355040a1307416e64726f6964311630140603550403130d416e64726f6964204465627567301e170d3135303732323134353830375a170d3435303731343134353830375a3037310b30090603550406130255533110300e060355040a1307416e64726f6964311630140603550403130d416e64726f696420446562756730820122300d06092a864886f70d01010105000382010f003082010a02820101009ebd1f327aa7fd9d5c556df9e09ce4d7f091b04ffe649bf0c286fcd7d2efb24c485f02b4518d08227285d1758f0e6ba44ec3d2dd16a53d34f790452c2b25166db2488ac8de275cbe575325a4f19a476e23cd0831e7b05bb728525500e516bb24b20444ce79ec5625cf5963e4b792f8c5017fc36b880b8b78750bdcace2d4e25aee155aab3a4bb1c1c9a539a73edfc1057d77080e85c9506b033ffc72efe2c418f91171b78899be0d9fb04e5befd57cae955d7a81aff4573362d74571bd84d8ca5502cf99ad3124a6dfe45428b38c50300af28c13006803feadfc3aed84027b5fc4d0350ff41612652368f49b49c0461ff845b9c1e1bea440df4f65805f582b150203010001a321301f301d0603551d0e0416041490edd92d5d54c07225eb32807acdd2bd57a169a4300d06092a864886f70d01010b050003820101000932afee5e88d0b1252c84d1d9b8533c5d57fbd0e4766c53ce0f6565e04fe06c4c23687e109d9e23569736f3ee2105c57630b3b66229d25d910293c3d615e81290e932ecf321cc59a2dbecb4acf89a811cd63f10611b01d546f2aea1a23f259bb7f4e833117396e2e62c28a331d5d9a3fb625e199438635dd65e2da46fd4687aea161e9a490a597e264573be8821e2f3e7826df68dfee333301d968d154636497e76851f838df16d9d428b390b5b19f7b6ddbdbb1e19395f349f169764f38c114a91eaa831195a8e2c6217d1ac385ca4f6301f385fe44bdb82bbf6cd64033cd54e334500cec523e6d5abcc3f6bed5538ad7830ec45a897b1752f3695063e4853" ? 1 : (key == "3082019d30820106a00302010202044e7b1922300d06092a864886f70d010105050030133111300f060355040a130863596f20536f6674301e170d3131303932323131313635305a170d3336303931353131313635305a30133111300f060355040a130863596f20536f667430819f300d06092a864886f70d010101050003818d00308189028181008d81ffb74008a048c517275a464db26461df06a3c85675b6ffa8bea15ec9288eec1ef1bf7616d09b7265bf1c5666473342c2a96ca385769592d73a21595335e5173c69ae5bb7aebd29387e9635ce30bdf11afff71145570b6577799ecac6100bcf0b2c4df6fe34fb8a418b5511c6a56c97b15c544269e91478ee24633ef063090203010001300d06092a864886f70d010105050003818100802cf8770c7af0744f9680b54da88b56eb1d6a48e8d446ce746817fe959991dc1f882323c6015edd4d48f28cfe3a94e30b75c92855b01a8c48354aae8e13a0b949390133c07b09419ac73d0b0b3dc13b2838fe9eae4b171c8022cb47ead602771560277cde7ad61e1a9ce5dee880d0226ed8cc71f36fb376d271a3cb61f1128b" ? 1 : 0))) != 0;
          goto case SyncAppEdition.iOS;
        case SyncAppEdition.iOS:
          if (flag)
            break;
          throw new StorageSync.FatalSyncException("Invalid device");
        default:
          flag = false;
          goto case SyncAppEdition.iOS;
      }
    }

    public override IEnumerable<ComicBook> GetBooks()
    {
      List<string> deleteFiles = new List<string>();
      string[] files = this.GetFileList().Where<string>(new Func<string, bool>(SyncProviderBase.IsValidSyncFile)).ToArray<string>();
      this.Communicate((Action<Socket>) (socket =>
      {
        ComicBookCollection comicBookCollection = new ComicBookCollection();
        WirelessSyncProvider.SendByte(socket, (byte) 10);
        WirelessSyncProvider.SendInteger(socket, files.Length);
        for (int index = 0; index < files.Length; ++index)
          WirelessSyncProvider.SendString(socket, SyncProviderBase.MakeSidecar(files[index]));
        for (int index = 0; index < files.Length; ++index)
        {
          Stream inputStream = (Stream) new MemoryStream(WirelessSyncProvider.ReadSocketData(socket));
          ComicBook comicBook = this.DeserializeBook(files[index], inputStream);
          if (comicBook != null && comicBookCollection.FindItemById(comicBook.Id) == null)
            comicBookCollection.Add(comicBook);
          else
            deleteFiles.Add(files[index]);
        }
        this.BooksOnDevice = comicBookCollection;
      }));
      foreach (string fileName in deleteFiles)
      {
        this.DeleteFile(fileName);
        this.DeleteFile(SyncProviderBase.MakeSidecar(fileName));
      }
      return (IEnumerable<ComicBook>) this.BooksOnDevice;
    }

    public void Communicate(Action<Socket> action, int retry = -1)
    {
      WirelessSyncProvider.Communicate(this.address, action, retry);
    }

    public static void Communicate(IPAddress address, Action<Socket> action, int retry = -1)
    {
      int syncReceiveTimeout = EngineConfiguration.Default.WifiSyncReceiveTimeout;
      int wifiSyncSendTimeout = EngineConfiguration.Default.WifiSyncSendTimeout;
      int connectionTimeout = EngineConfiguration.Default.WifiSyncConnectionTimeout;
      int connectionRetries = EngineConfiguration.Default.WifiSyncConnectionRetries;
      if (retry < 0)
        retry = connectionRetries;
label_2:
      Socket socket = (Socket) null;
      try
      {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP)
        {
          ReceiveTimeout = syncReceiveTimeout,
          SendTimeout = wifiSyncSendTimeout
        };
        socket.BeginConnect(address, 7614, (AsyncCallback) null, (object) null).AsyncWaitHandle.WaitOne(connectionTimeout, true);
        if (!socket.Connected)
        {
          socket.Close();
          throw new CommunicationException("Failed to connect to device");
        }
        action(socket);
      }
      catch (Exception ex)
      {
        if (--retry < 0)
          throw;
        else
          goto label_2;
      }
      finally
      {
        socket?.Close();
      }
    }

    public static void SendString(Socket socket, string text)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(text);
      WirelessSyncProvider.SendInteger(socket, bytes.Length);
      socket.Send(bytes);
    }

    public static byte[] ReadBytes(Socket socket)
    {
      byte[] data = new byte[WirelessSyncProvider.ReadInteger(socket)];
      WirelessSyncProvider.ReadBlocking(socket, data);
      return data;
    }

    public static string ReadString(Socket socket)
    {
      return Encoding.UTF8.GetString(WirelessSyncProvider.ReadBytes(socket));
    }

    public static void SendInteger(Socket socket, int data)
    {
      if (BitConverter.IsLittleEndian)
        data = data.EndianSwap();
      byte[] bytes = BitConverter.GetBytes(data);
      socket.Send(bytes);
    }

    public static void SendLong(Socket socket, long data)
    {
      if (BitConverter.IsLittleEndian)
        data = data.EndianSwap();
      socket.Send(BitConverter.GetBytes(data));
    }

    public static int ReadInteger(Socket socket)
    {
      byte[] data = new byte[4];
      WirelessSyncProvider.ReadBlocking(socket, data);
      int x = BitConverter.ToInt32(data, 0);
      if (BitConverter.IsLittleEndian)
        x = x.EndianSwap();
      return x;
    }

    public static void SendByte(Socket socket, byte data)
    {
      socket.Send(new byte[1]{ data });
    }

    public static bool ReadBool(Socket socket)
    {
      byte[] data = new byte[1];
      WirelessSyncProvider.ReadBlocking(socket, data);
      return data[0] > (byte) 0;
    }

    public static long ReadLong(Socket socket)
    {
      byte[] data = new byte[8];
      WirelessSyncProvider.ReadBlocking(socket, data);
      long x = BitConverter.ToInt64(data, 0);
      if (BitConverter.IsLittleEndian)
        x = x.EndianSwap();
      return x;
    }

    public static byte[] ReadSocketData(Socket socket)
    {
      int length = (int) WirelessSyncProvider.ReadLong(socket);
      byte[] buffer = new byte[length];
      int offset = 0;
      while (offset < length)
      {
        int num = socket.Receive(buffer, offset, length - offset, SocketFlags.None);
        if (num != -1)
        {
          if (num > 0)
            offset += num;
        }
        else
          break;
      }
      if (offset != length)
        throw new IOException();
      return buffer;
    }

    public static void ReadBlocking(Socket socket, byte[] data, int offset, int length)
    {
      int num1 = 0;
      try
      {
        int num2;
        for (; length > 0; length -= num2)
        {
          if ((num2 = socket.Receive(data, offset + num1, length, SocketFlags.None)) != 0)
            num1 += num2;
          else
            break;
        }
      }
      catch (Exception ex)
      {
        throw new StorageSync.FatalSyncException("Error during Read", ex);
      }
      if (length != 0)
        throw new StorageSync.FatalSyncException("Wrong length");
    }

    public static void ReadBlocking(Socket socket, byte[] data)
    {
      WirelessSyncProvider.ReadBlocking(socket, data, 0, data.Length);
    }

    public static event EventHandler<WirelessSyncProvider.ClientSyncRequestArgs> ClientSyncRequest;

    public static void StartListen()
    {
      try
      {
        IPAddress group = IPAddress.Parse("224.34.123.90");
        WirelessSyncProvider.listener = new UdpClient();
        Socket client = WirelessSyncProvider.listener.Client;
        WirelessSyncProvider.listenerEP = new IPEndPoint(IPAddress.Any, 7615);
        client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        client.Bind((EndPoint) WirelessSyncProvider.listenerEP);
        client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, (object) new MulticastOption(group, IPAddress.Any));
        WirelessSyncProvider.listener.BeginReceive(new AsyncCallback(WirelessSyncProvider.OnReceivedBroadcastData), (object) null);
      }
      catch (Exception ex)
      {
      }
      WirelessSyncProvider.controlPort = 7620;
      WirelessSyncProvider.controlSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      while (true)
      {
        try
        {
          IPEndPoint localEP = new IPEndPoint(IPAddress.Any, WirelessSyncProvider.controlPort);
          WirelessSyncProvider.controlSocket.Bind((EndPoint) localEP);
          WirelessSyncProvider.controlSocket.Listen(25);
          WirelessSyncProvider.controlSocket.BeginAccept(new AsyncCallback(WirelessSyncProvider.OnAcceptControl), (object) null);
          break;
        }
        catch (SocketException ex)
        {
          if (ex.ErrorCode == 10048)
            ++WirelessSyncProvider.controlPort;
          else
            break;
        }
        catch (Exception ex)
        {
          break;
        }
      }
      WirelessSyncProvider.serverAvailableTimer = new System.Threading.Timer(new TimerCallback(WirelessSyncProvider.NotifyDevicesServerAvailable), (object) null, 10000, 10000);
    }

    private static void NotifyDevicesServerAvailable(object state)
    {
      foreach (IPAddress wifiDeviceAddress in DeviceSyncFactory.ExtraWifiDeviceAddresses)
      {
        try
        {
          lock (WirelessSyncProvider.foundDevices)
          {
            if (WirelessSyncProvider.foundDevices.Contains(wifiDeviceAddress))
              continue;
          }
          WirelessSyncProvider.Communicate(wifiDeviceAddress, (Action<Socket>) (s =>
          {
            WirelessSyncProvider.SendByte(s, (byte) 13);
            WirelessSyncProvider.SendInteger(s, WirelessSyncProvider.controlPort);
          }), 0);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static IEnumerable<IPAddress> GetWirelessDevices()
    {
      using (ItemMonitor.Lock((object) WirelessSyncProvider.foundDevices))
        return (IEnumerable<IPAddress>) WirelessSyncProvider.foundDevices.ToArray<IPAddress>();
    }

    private static void OnAcceptControl(IAsyncResult ar)
    {
      try
      {
        using (Socket socket = WirelessSyncProvider.controlSocket.EndAccept(ar))
        {
          byte[] numArray = new byte[2048];
          int count = socket.Receive(numArray);
          WirelessSyncProvider.HandleDeviceMessage((socket.RemoteEndPoint as IPEndPoint).Address, ((IEnumerable<byte>) numArray).Take<byte>(count).ToArray<byte>(), false);
        }
      }
      catch (Exception ex)
      {
      }
      finally
      {
        try
        {
          WirelessSyncProvider.controlSocket.BeginAccept(new AsyncCallback(WirelessSyncProvider.OnAcceptControl), (object) null);
        }
        catch (Exception ex)
        {
        }
      }
    }

    private static void OnReceivedBroadcastData(IAsyncResult ar)
    {
      try
      {
        byte[] bytes = WirelessSyncProvider.listener.EndReceive(ar, ref WirelessSyncProvider.listenerEP);
        WirelessSyncProvider.HandleDeviceMessage(WirelessSyncProvider.listenerEP.Address, bytes, true);
      }
      catch
      {
      }
      finally
      {
        try
        {
          WirelessSyncProvider.listener.BeginReceive(new AsyncCallback(WirelessSyncProvider.OnReceivedBroadcastData), (object) null);
        }
        catch (SocketException ex)
        {
        }
      }
    }

    private static bool HandleDeviceMessage(IPAddress address, byte[] bytes, bool addAddress)
    {
      string[] strArray = Encoding.UTF8.GetString(bytes).Split(':');
      if (!strArray[0].StartsWith("ComicRack"))
        return true;
      if (addAddress)
      {
        using (ItemMonitor.Lock((object) WirelessSyncProvider.foundDevices))
          WirelessSyncProvider.foundDevices.Add(address);
      }
      if (strArray.Length > 1)
      {
        string key = strArray[1];
        if (strArray.Length > 2 && strArray[2] == "Sync")
        {
          WirelessSyncProvider.ClientSyncRequest((object) address, new WirelessSyncProvider.ClientSyncRequestArgs(key));
        }
        else
        {
          WirelessSyncProvider.ClientSyncRequestArgs e = new WirelessSyncProvider.ClientSyncRequestArgs(key);
          WirelessSyncProvider.ClientSyncRequest((object) null, e);
          if (e.IsPaired)
            WirelessSyncProvider.Communicate(address, (Action<Socket>) (s => WirelessSyncProvider.SendByte(s, (byte) 12)), 0);
        }
      }
      return false;
    }

    public class ClientSyncRequestArgs : EventArgs
    {
      public ClientSyncRequestArgs(string key) => this.Key = key;

      public bool IsPaired { get; set; }

      public string Key { get; private set; }
    }
  }
}

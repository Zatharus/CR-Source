// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.IPAddressExtension
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System.Net;
using System.Net.Sockets;

#nullable disable
namespace cYo.Common.Net
{
  public static class IPAddressExtension
  {
    public static bool IsPrivate(this IPAddress address)
    {
      if (address.AddressFamily == AddressFamily.InterNetwork)
        return ((IPAddressV4) address).IsPrivate();
      return address.AddressFamily != AddressFamily.InterNetworkV6 || address.IsIPv6LinkLocal || address.IsIPv6SiteLocal || IPAddress.IsLoopback(address);
    }
  }
}

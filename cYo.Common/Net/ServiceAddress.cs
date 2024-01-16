// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.ServiceAddress
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

#nullable disable
namespace cYo.Common.Net
{
  public class ServiceAddress
  {
    private static string wanIp;
    private static Regex rxUrlSplit = new Regex("^(?<host>[^:/]+)(:(?<port>\\d+))?(/(?<path>.*))?");

    public ServiceAddress(IPAddress address)
    {
      this.Host = address.ToString();
      this.IsValid = true;
    }

    public ServiceAddress(string host, string port, string path)
    {
      this.Host = host;
      this.Port = port;
      this.Service = path;
      this.IsValid = true;
    }

    public ServiceAddress(string serviceAddress)
    {
      string host;
      string port;
      string path;
      this.IsValid = ServiceAddress.TryParse(serviceAddress, out host, out port, out path);
      this.Host = host;
      this.Port = port;
      this.Service = path;
    }

    public string Host { get; set; }

    public string Port { get; set; }

    public string Service { get; set; }

    public bool IsValid { get; private set; }

    public override string ToString()
    {
      return this.Host.AppendWithSeparator(":", this.Port).AppendWithSeparator("/", this.Service);
    }

    public override int GetHashCode() => base.GetHashCode();

    public static string GetWanAddress() => ServiceAddress.GetWanAddress(false);

    public static string GetWanAddress(bool refresh)
    {
      if (!refresh && ServiceAddress.wanIp != null)
        return ServiceAddress.wanIp;
      try
      {
        HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create("http://comicrack.cyolito.com/services/ClientAddress.php");
        httpWebRequest.Accept = "*/*";
        using (WebResponse response = httpWebRequest.GetResponse())
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream))
              ServiceAddress.wanIp = streamReader.ReadToEnd();
          }
        }
      }
      catch (Exception ex)
      {
        ServiceAddress.wanIp = string.Empty;
      }
      return !string.IsNullOrEmpty(ServiceAddress.wanIp) ? ServiceAddress.wanIp : (string) null;
    }

    public static string CompletePortAndPath(string host, string newPort, string newPath)
    {
      if (string.IsNullOrEmpty(host))
        return host;
      string host1;
      string port;
      string path;
      if (!ServiceAddress.TryParse(host, out host1, out port, out path))
        throw new ArgumentException("is not valid", nameof (host));
      if (string.IsNullOrEmpty(port))
        port = newPort;
      if (string.IsNullOrEmpty(path))
        path = newPath;
      return host1.AppendWithSeparator(":", port).AppendWithSeparator("/", path);
    }

    public static string Append(IPAddress address, string newPort, string newPath)
    {
      return ServiceAddress.CompletePortAndPath(address.ToString(), newPort, newPath);
    }

    public static bool TryParse(string address, out string host, out string port, out string path)
    {
      Match match = ServiceAddress.rxUrlSplit.Match(address ?? string.Empty);
      if (!match.Success)
      {
        host = port = path = (string) null;
        return false;
      }
      host = match.Groups[nameof (host)].Value.Trim();
      port = match.Groups[nameof (port)].Value.Trim();
      path = match.Groups[nameof (path)].Value.Trim();
      return !string.IsNullOrEmpty(host);
    }

    public static bool IsPrivate(string address)
    {
      try
      {
        return ((IEnumerable<IPAddress>) Dns.GetHostAddresses(new ServiceAddress(address).Host)).All<IPAddress>((Func<IPAddress, bool>) (ip => ip.IsPrivate()));
      }
      catch (Exception ex)
      {
      }
      return true;
    }
  }
}

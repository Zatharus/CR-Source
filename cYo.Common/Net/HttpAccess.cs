// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.HttpAccess
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using cYo.Common.IO;
using cYo.Common.Threading;
using cYo.Common.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Services.Protocols;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Net
{
  public class HttpAccess
  {
    public const string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
    private static HashSet<string> ignored;

    public HttpAccess()
    {
      this.AskProxyCredentials = true;
      this.AskSecureCredentials = false;
      this.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
    }

    public string UserAgent { get; set; }

    public bool AskProxyCredentials { get; set; }

    public NetworkCredential ProxyCredentials { get; set; }

    public bool AskSecureCredentials { get; set; }

    public NetworkCredential SecureCredentials { get; set; }

    public T GetResponse<T>(
      Action create,
      Action<ICredentials> setProxyCredentials,
      Action<ICredentials> setSecureCredentials,
      Func<Uri> getProxyUri,
      Func<Uri> getConnectionUri,
      Func<T> connect)
    {
      while (true)
      {
        create();
        try
        {
          if (this.ProxyCredentials != null)
            setProxyCredentials((ICredentials) this.ProxyCredentials);
          if (this.SecureCredentials != null)
          {
            CredentialCache credentialCache = new CredentialCache();
            this.SecureCredentials.Domain = string.Empty;
            credentialCache.Add(getConnectionUri(), "BASIC", this.SecureCredentials);
            setSecureCredentials((ICredentials) credentialCache);
          }
          return connect();
        }
        catch (Exception ex)
        {
          if (ex.Message.Contains("407"))
          {
            if (!this.AskProxyCredentials)
            {
              throw;
            }
            else
            {
              NetworkCredential networkCredential = HttpAccess.HandleCredentials((ICredentials) this.ProxyCredentials, getProxyUri());
              if (networkCredential == null)
                throw;
              else
                this.ProxyCredentials = networkCredential;
            }
          }
          else if (ex.Message.Contains("401"))
          {
            if (!this.AskSecureCredentials)
            {
              throw;
            }
            else
            {
              NetworkCredential networkCredential = HttpAccess.HandleCredentials((ICredentials) this.SecureCredentials, getConnectionUri());
              if (networkCredential == null)
                throw;
              else
                this.SecureCredentials = networkCredential;
            }
          }
          else
            throw;
        }
      }
    }

    public void WrapSoap<T>(T request, Action<T> call) where T : SoapHttpClientProtocol
    {
      this.GetResponse<bool>((Action) (() => ((T) request).Proxy = WebRequest.DefaultWebProxy), (Action<ICredentials>) (c =>
      {
        if (((T) request).Proxy == null)
          return;
        ((T) request).Proxy.Credentials = c;
      }), (Action<ICredentials>) (c => ((T) request).Credentials = c), (Func<Uri>) (() => ((T) request).Proxy.GetProxy(new Uri(((T) request).Url))), (Func<Uri>) (() => new Uri(((T) request).Url)), (Func<bool>) (() =>
      {
        ((T) request).UserAgent = this.UserAgent;
        call(request);
        return true;
      }));
    }

    public Stream GetStream(Uri uri)
    {
      HttpWebRequest request = (HttpWebRequest) null;
      return this.GetResponse<Stream>((Action) (() => request = (HttpWebRequest) WebRequest.Create(uri)), (Action<ICredentials>) (c =>
      {
        if (request.Proxy == null)
          return;
        request.Proxy.Credentials = c;
      }), (Action<ICredentials>) (c => request.Credentials = c), (Func<Uri>) (() => request.Proxy.GetProxy(uri)), (Func<Uri>) (() => uri), (Func<Stream>) (() =>
      {
        request.UserAgent = this.UserAgent;
        request.KeepAlive = true;
        request.Accept = "*/*";
        WebResponse response = request.GetResponse();
        try
        {
          StreamEx stream = new StreamEx(response.GetResponseStream());
          stream.Closed += (EventHandler) ((s, e) => response.SafeDispose());
          return (Stream) stream;
        }
        catch
        {
          response.SafeDispose();
          throw;
        }
      }));
    }

    private static HashSet<string> Ignored
    {
      get
      {
        if (HttpAccess.ignored == null)
          HttpAccess.ignored = new HashSet<string>();
        return HttpAccess.ignored;
      }
    }

    private static NetworkCredential HandleCredentials(ICredentials current, Uri uri)
    {
      string authority = uri.Authority;
      if (uri.AbsolutePath != "/")
        authority += uri.AbsolutePath;
      using (ItemMonitor.Lock((object) typeof (HttpAccess)))
      {
        using (UserCredentialsDialog credentialsDialog = new UserCredentialsDialog(authority))
        {
          if (current == null)
            credentialsDialog.Flags &= ~UserCredentialsDialogFlags.AlwaysShowUI;
          else if (HttpAccess.Ignored.Contains(authority))
            return (NetworkCredential) null;
          if (credentialsDialog.ShowDialog() != DialogResult.OK)
          {
            HttpAccess.Ignored.Add(authority);
            return (NetworkCredential) null;
          }
          if (credentialsDialog.SaveChecked)
            credentialsDialog.ConfirmCredentials(true);
          return new NetworkCredential(credentialsDialog.User, credentialsDialog.PasswordToString());
        }
      }
    }

    public static string ReadText(string uri)
    {
      Uri uri1 = new Uri(uri);
      if (uri1.IsFile)
        return System.IO.File.ReadAllText(uri1.LocalPath);
      using (Stream stream = new HttpAccess().GetStream(uri1))
      {
        using (StreamReader streamReader = new StreamReader(stream))
          return streamReader.ReadToEnd();
      }
    }

    public static byte[] ReadBinary(string uri)
    {
      Uri uri1 = new Uri(uri);
      if (uri1.IsFile)
        return System.IO.File.ReadAllBytes(uri1.LocalPath);
      using (Stream stream = new HttpAccess().GetStream(uri1))
      {
        MemoryStream memoryStream = new MemoryStream();
        byte[] buffer = new byte[10000];
        int count;
        while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
          memoryStream.Write(buffer, 0, count);
        return memoryStream.ToArray();
      }
    }

    public static K CallSoap<T, K>(Func<T, K> call) where T : SoapHttpClientProtocol, new()
    {
      T soap = new T();
      try
      {
        return HttpAccess.CallSoap<T, K>(soap, call);
      }
      finally
      {
        if ((object) soap != null)
          ((IDisposable) soap).Dispose();
      }
    }

    public static K CallSoap<T, K>(T soap, Func<T, K> call) where T : SoapHttpClientProtocol
    {
      K result = default (K);
      new HttpAccess().WrapSoap<T>(soap, (Action<T>) (r => result = call(r)));
      return result;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Network.ServerRegistration
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Network
{
  [GeneratedCode("wsdl", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [WebServiceBinding(Name = "ServerRegistrationBinding", Namespace = "urn:ServerRegistration")]
  [SoapInclude(typeof (ServerInfo))]
  public class ServerRegistration : SoapHttpClientProtocol
  {
    private SendOrPostCallback RegisterOperationCompleted;
    private SendOrPostCallback UnregisterOperationCompleted;
    private SendOrPostCallback RefreshOperationCompleted;
    private SendOrPostCallback GetListOperationCompleted;

    public ServerRegistration()
    {
      this.Url = "http://comicrack.cyolito.com/services/ServerRegistration2.php";
    }

    public event RegisterCompletedEventHandler RegisterCompleted;

    public event UnregisterCompletedEventHandler UnregisterCompleted;

    public event RefreshCompletedEventHandler RefreshCompleted;

    public event GetListCompletedEventHandler GetListCompleted;

    [SoapRpcMethod("urn:ServerRegistration#Register", RequestNamespace = "urn:ServerRegistration", ResponseNamespace = "urn:ServerRegistration")]
    [return: SoapElement("result")]
    public bool Register(string uri, string name, string comment, int options, string password)
    {
      return (bool) this.Invoke(nameof (Register), new object[5]
      {
        (object) uri,
        (object) name,
        (object) comment,
        (object) options,
        (object) password
      })[0];
    }

    public IAsyncResult BeginRegister(
      string uri,
      string name,
      string comment,
      int options,
      string password,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("Register", new object[5]
      {
        (object) uri,
        (object) name,
        (object) comment,
        (object) options,
        (object) password
      }, callback, asyncState);
    }

    public bool EndRegister(IAsyncResult asyncResult) => (bool) this.EndInvoke(asyncResult)[0];

    public void RegisterAsync(
      string uri,
      string name,
      string comment,
      int options,
      string password)
    {
      this.RegisterAsync(uri, name, comment, options, password, (object) null);
    }

    public void RegisterAsync(
      string uri,
      string name,
      string comment,
      int options,
      string password,
      object userState)
    {
      if (this.RegisterOperationCompleted == null)
        this.RegisterOperationCompleted = new SendOrPostCallback(this.OnRegisterOperationCompleted);
      this.InvokeAsync("Register", new object[5]
      {
        (object) uri,
        (object) name,
        (object) comment,
        (object) options,
        (object) password
      }, this.RegisterOperationCompleted, userState);
    }

    private void OnRegisterOperationCompleted(object arg)
    {
      if (this.RegisterCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.RegisterCompleted((object) this, new RegisterCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapRpcMethod("urn:ServerRegistration#Unregister", RequestNamespace = "urn:ServerRegistration", ResponseNamespace = "urn:ServerRegistration")]
    [return: SoapElement("result")]
    public bool Unregister(string uri)
    {
      return (bool) this.Invoke(nameof (Unregister), new object[1]
      {
        (object) uri
      })[0];
    }

    public IAsyncResult BeginUnregister(string uri, AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("Unregister", new object[1]
      {
        (object) uri
      }, callback, asyncState);
    }

    public bool EndUnregister(IAsyncResult asyncResult) => (bool) this.EndInvoke(asyncResult)[0];

    public void UnregisterAsync(string uri) => this.UnregisterAsync(uri, (object) null);

    public void UnregisterAsync(string uri, object userState)
    {
      if (this.UnregisterOperationCompleted == null)
        this.UnregisterOperationCompleted = new SendOrPostCallback(this.OnUnregisterOperationCompleted);
      this.InvokeAsync("Unregister", new object[1]
      {
        (object) uri
      }, this.UnregisterOperationCompleted, userState);
    }

    private void OnUnregisterOperationCompleted(object arg)
    {
      if (this.UnregisterCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.UnregisterCompleted((object) this, new UnregisterCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapRpcMethod("urn:ServerRegistration#Refresh", RequestNamespace = "urn:ServerRegistration", ResponseNamespace = "urn:ServerRegistration")]
    [return: SoapElement("result")]
    public bool Refresh(string uri)
    {
      return (bool) this.Invoke(nameof (Refresh), new object[1]
      {
        (object) uri
      })[0];
    }

    public IAsyncResult BeginRefresh(string uri, AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("Refresh", new object[1]
      {
        (object) uri
      }, callback, asyncState);
    }

    public bool EndRefresh(IAsyncResult asyncResult) => (bool) this.EndInvoke(asyncResult)[0];

    public void RefreshAsync(string uri) => this.RefreshAsync(uri, (object) null);

    public void RefreshAsync(string uri, object userState)
    {
      if (this.RefreshOperationCompleted == null)
        this.RefreshOperationCompleted = new SendOrPostCallback(this.OnRefreshOperationCompleted);
      this.InvokeAsync("Refresh", new object[1]
      {
        (object) uri
      }, this.RefreshOperationCompleted, userState);
    }

    private void OnRefreshOperationCompleted(object arg)
    {
      if (this.RefreshCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.RefreshCompleted((object) this, new RefreshCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    [SoapRpcMethod("urn:ServerRegistration#GetList", RequestNamespace = "urn:ServerRegistration", ResponseNamespace = "urn:ServerRegistration")]
    [return: SoapElement("result")]
    public ServerInfo[] GetList(int mask, string password)
    {
      return (ServerInfo[]) this.Invoke(nameof (GetList), new object[2]
      {
        (object) mask,
        (object) password
      })[0];
    }

    public IAsyncResult BeginGetList(
      int mask,
      string password,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("GetList", new object[2]
      {
        (object) mask,
        (object) password
      }, callback, asyncState);
    }

    public ServerInfo[] EndGetList(IAsyncResult asyncResult)
    {
      return (ServerInfo[]) this.EndInvoke(asyncResult)[0];
    }

    public void GetListAsync(int mask, string password)
    {
      this.GetListAsync(mask, password, (object) null);
    }

    public void GetListAsync(int mask, string password, object userState)
    {
      if (this.GetListOperationCompleted == null)
        this.GetListOperationCompleted = new SendOrPostCallback(this.OnGetListOperationCompleted);
      this.InvokeAsync("GetList", new object[2]
      {
        (object) mask,
        (object) password
      }, this.GetListOperationCompleted, userState);
    }

    private void OnGetListOperationCompleted(object arg)
    {
      if (this.GetListCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.GetListCompleted((object) this, new GetListCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public new void CancelAsync(object userState) => base.CancelAsync(userState);
  }
}

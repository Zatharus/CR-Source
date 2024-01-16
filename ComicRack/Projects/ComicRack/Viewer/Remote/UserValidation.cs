// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Remote.UserValidation
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Remote
{
  [GeneratedCode("wsdl", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [WebServiceBinding(Name = "UserValidationBinding", Namespace = "urn:UserValidation")]
  public class UserValidation : SoapHttpClientProtocol
  {
    private SendOrPostCallback HasDonatedOperationCompleted;

    public UserValidation()
    {
      this.Url = "http://comicrack.cyolito.com/services/UserValidation.php";
    }

    public event HasDonatedCompletedEventHandler HasDonatedCompleted;

    [SoapRpcMethod("urn:UserValidation#HasDonated", RequestNamespace = "urn:UserValidation", ResponseNamespace = "urn:UserValidation")]
    [return: SoapElement("result")]
    public bool HasDonated(string user)
    {
      return (bool) this.Invoke(nameof (HasDonated), new object[1]
      {
        (object) user
      })[0];
    }

    public IAsyncResult BeginHasDonated(string user, AsyncCallback callback, object asyncState)
    {
      return this.BeginInvoke("HasDonated", new object[1]
      {
        (object) user
      }, callback, asyncState);
    }

    public bool EndHasDonated(IAsyncResult asyncResult) => (bool) this.EndInvoke(asyncResult)[0];

    public void HasDonatedAsync(string user) => this.HasDonatedAsync(user, (object) null);

    public void HasDonatedAsync(string user, object userState)
    {
      if (this.HasDonatedOperationCompleted == null)
        this.HasDonatedOperationCompleted = new SendOrPostCallback(this.OnHasDonatedOperationCompleted);
      this.InvokeAsync("HasDonated", new object[1]
      {
        (object) user
      }, this.HasDonatedOperationCompleted, userState);
    }

    private void OnHasDonatedOperationCompleted(object arg)
    {
      if (this.HasDonatedCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.HasDonatedCompleted((object) this, new HasDonatedCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public new void CancelAsync(object userState) => base.CancelAsync(userState);
  }
}

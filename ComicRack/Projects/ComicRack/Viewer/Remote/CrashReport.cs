// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Remote.CrashReport
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
  [GeneratedCode("wsdl", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [WebServiceBinding(Name = "CrashReportBinding", Namespace = "urn:CrashReport")]
  public class CrashReport : SoapHttpClientProtocol
  {
    private SendOrPostCallback SubmitReportOperationCompleted;

    public CrashReport() => this.Url = "http://comicrack.cyolito.com/services/CrashReport.php";

    public event SubmitReportCompletedEventHandler SubmitReportCompleted;

    [SoapRpcMethod("urn:CrashReport#SubmitReport", RequestNamespace = "urn:CrashReport", ResponseNamespace = "urn:CrashReport")]
    [return: SoapElement("result")]
    public string SubmitReport(string appication, string report)
    {
      return (string) this.Invoke(nameof (SubmitReport), new object[2]
      {
        (object) appication,
        (object) report
      })[0];
    }

    public IAsyncResult BeginSubmitReport(
      string appication,
      string report,
      AsyncCallback callback,
      object asyncState)
    {
      return this.BeginInvoke("SubmitReport", new object[2]
      {
        (object) appication,
        (object) report
      }, callback, asyncState);
    }

    public string EndSubmitReport(IAsyncResult asyncResult)
    {
      return (string) this.EndInvoke(asyncResult)[0];
    }

    public void SubmitReportAsync(string appication, string report)
    {
      this.SubmitReportAsync(appication, report, (object) null);
    }

    public void SubmitReportAsync(string appication, string report, object userState)
    {
      if (this.SubmitReportOperationCompleted == null)
        this.SubmitReportOperationCompleted = new SendOrPostCallback(this.OnSubmitReportOperationCompleted);
      this.InvokeAsync("SubmitReport", new object[2]
      {
        (object) appication,
        (object) report
      }, this.SubmitReportOperationCompleted, userState);
    }

    private void OnSubmitReportOperationCompleted(object arg)
    {
      if (this.SubmitReportCompleted == null)
        return;
      InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs) arg;
      this.SubmitReportCompleted((object) this, new SubmitReportCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public new void CancelAsync(object userState) => base.CancelAsync(userState);
  }
}

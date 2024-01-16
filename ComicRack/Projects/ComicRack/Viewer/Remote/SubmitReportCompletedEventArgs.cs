// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Remote.SubmitReportCompletedEventArgs
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer.Remote
{
  [GeneratedCode("wsdl", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  public class SubmitReportCompletedEventArgs : AsyncCompletedEventArgs
  {
    private readonly object[] results;

    internal SubmitReportCompletedEventArgs(
      object[] results,
      Exception exception,
      bool cancelled,
      object userState)
      : base(exception, cancelled, userState)
    {
      this.results = results;
    }

    public string Result
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (string) this.results[0];
      }
    }
  }
}

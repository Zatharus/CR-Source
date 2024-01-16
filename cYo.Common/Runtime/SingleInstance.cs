// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.SingleInstance
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

#nullable disable
namespace cYo.Common.Runtime
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
  public class SingleInstance : ISingleInstance
  {
    private readonly string name;
    private readonly Action<string[]> StartNew;
    private readonly Action<string[]> StartLast;

    public SingleInstance(string name, Action<string[]> startNew, Action<string[]> startLast)
    {
      this.name = name;
      this.StartNew = startNew;
      this.StartLast = startLast;
    }

    public void Run(string[] args)
    {
      string uriString = string.Format("net.pipe://localhost/{0}", (object) this.name);
      ServiceHost serviceHost = (ServiceHost) null;
      try
      {
        serviceHost = new ServiceHost((object) this, new Uri[1]
        {
          new Uri(uriString)
        });
        serviceHost.AddServiceEndpoint(typeof (ISingleInstance), (Binding) new NetNamedPipeBinding(), "SI");
        serviceHost.Open();
        try
        {
          this.StartNew(args);
          return;
        }
        catch (Exception ex)
        {
          Trace.WriteLine("Failed to start Program: " + ex.Message);
          return;
        }
      }
      catch (Exception ex)
      {
      }
      finally
      {
        try
        {
          serviceHost.Close();
        }
        catch
        {
        }
      }
      try
      {
        new ChannelFactory<ISingleInstance>((Binding) new NetNamedPipeBinding(), uriString + "/SI").CreateChannel().InvokeLast(args);
      }
      catch
      {
      }
    }

    public void InvokeLast(string[] args)
    {
      if (this.StartLast == null)
        return;
      this.StartLast(args);
    }

    public void InvokeNew(string[] args)
    {
      if (this.StartNew == null)
        return;
      this.StartNew(args);
    }
  }
}

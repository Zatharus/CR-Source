// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.LiteComponent
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.ComponentModel
{
  [Serializable]
  public class LiteComponent : DisposableObject, ILiteComponent, IDisposable
  {
    public virtual T QueryService<T>() where T : class
    {
      ServiceRequestEventArgs e = new ServiceRequestEventArgs(typeof (T), (object) (this as T));
      this.OnServiceRequest(e);
      return e.Service as T;
    }

    [field: NonSerialized]
    public event EventHandler<ServiceRequestEventArgs> ServiceRequest;

    protected virtual void OnServiceRequest(ServiceRequestEventArgs e)
    {
      if (this.ServiceRequest == null)
        return;
      this.ServiceRequest((object) this, e);
    }
  }
}

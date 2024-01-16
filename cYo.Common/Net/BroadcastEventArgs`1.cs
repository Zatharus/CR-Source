// Decompiled with JetBrains decompiler
// Type: cYo.Common.Net.BroadcastEventArgs`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Net;

#nullable disable
namespace cYo.Common.Net
{
  public class BroadcastEventArgs<T> : EventArgs
  {
    private readonly T data;
    private readonly IPAddress address;

    public BroadcastEventArgs(T data, IPAddress address)
    {
      this.data = data;
      this.address = address;
    }

    public T Data => this.data;

    public IPAddress Address => this.address;
  }
}

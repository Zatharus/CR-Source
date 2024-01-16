// Decompiled with JetBrains decompiler
// Type: cYo.Common.Threading.ItemMonitor
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Threading;

#nullable disable
namespace cYo.Common.Threading
{
  public struct ItemMonitor : IDisposable
  {
    public static bool CatchThreadInterruptException = true;
    private object lockItem;

    private ItemMonitor(object lockItem)
    {
      this.lockItem = lockItem;
      if (lockItem == null)
        return;
      try
      {
        Monitor.Enter(this.lockItem);
      }
      catch (ThreadInterruptedException ex)
      {
        if (ItemMonitor.CatchThreadInterruptException)
          this.lockItem = (object) null;
        else
          throw;
      }
    }

    public void Dispose()
    {
      try
      {
        if (this.lockItem == null)
          return;
        Monitor.Exit(this.lockItem);
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this.lockItem = (object) null;
      }
    }

    public object LockItem => this.lockItem;

    public override bool Equals(object obj)
    {
      return obj != null && !(obj.GetType() != typeof (ItemMonitor)) && this.lockItem == ((ItemMonitor) obj).lockItem;
    }

    public override int GetHashCode() => this.lockItem != null ? this.lockItem.GetHashCode() : 0;

    public static bool operator ==(ItemMonitor a, ItemMonitor b) => a.Equals((object) b);

    public static bool operator !=(ItemMonitor a, ItemMonitor b) => !(a == b);

    public static IDisposable Lock(object o) => (IDisposable) new ItemMonitor(o);
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.ItemLock`1
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Threading;
using System;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public class ItemLock<T> : DisposableObject, IItemLock<T>, IDisposable where T : class
  {
    private readonly IDisposable monitor;
    private volatile T item;

    public ItemLock(T data, object lockObject)
    {
      this.Item = data;
      this.monitor = ItemMonitor.Lock(lockObject);
    }

    public ItemLock(T data, bool synchronized)
      : this(data, (object) (synchronized ? data : default (T)))
    {
    }

    public ItemLock(T data)
      : this(data, true)
    {
    }

    public T Item
    {
      get => this.item;
      set => this.item = value;
    }

    public object LockObject { get; set; }

    public object Tag { get; set; }

    protected override void Dispose(bool disposing)
    {
      try
      {
        this.monitor.Dispose();
      }
      catch
      {
      }
      finally
      {
        this.Item = default (T);
      }
      base.Dispose(disposing);
    }
  }
}

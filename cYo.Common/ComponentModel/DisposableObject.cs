// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.DisposableObject
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Diagnostics;

#nullable disable
namespace cYo.Common.ComponentModel
{
  [Serializable]
  public abstract class DisposableObject : IDisposable
  {
    private bool isDisposed;

    [Conditional("DEBUG")]
    public void FinalizerIsOk()
    {
    }

    ~DisposableObject()
    {
      try
      {
        this.Dispose(false);
      }
      catch (Exception ex)
      {
      }
      finally
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
      }
    }

    public bool IsDisposed => this.isDisposed;

    protected virtual void Dispose(bool disposing)
    {
    }

    protected virtual void CheckDisposed()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.GetType().ToString());
    }

    [field: NonSerialized]
    public event EventHandler Disposing;

    [field: NonSerialized]
    public event EventHandler Disposed;

    public void Dispose()
    {
      try
      {
        try
        {
          if (this.Disposing != null)
            this.Disposing((object) this, EventArgs.Empty);
        }
        catch
        {
        }
        this.Dispose(true);
        this.isDisposed = true;
        try
        {
          if (this.Disposed == null)
            return;
          this.Disposed((object) this, EventArgs.Empty);
        }
        catch
        {
        }
      }
      catch
      {
      }
      finally
      {
        GC.SuppressFinalize((object) this);
      }
    }
  }
}

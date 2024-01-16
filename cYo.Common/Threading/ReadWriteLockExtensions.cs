// Decompiled with JetBrains decompiler
// Type: cYo.Common.Threading.ReadWriteLockExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace cYo.Common.Threading
{
  public static class ReadWriteLockExtensions
  {
    public static readonly bool IgnoreUnlockErrors = true;

    public static IDisposable ReadLock(this ReaderWriterLockSlim rwLock)
    {
      rwLock.EnterReadLock();
      return (IDisposable) new LeanDisposer(new Action(rwLock.ExitReadLock), ReadWriteLockExtensions.IgnoreUnlockErrors);
    }

    public static IDisposable UpgradeableReadLock(this ReaderWriterLockSlim rwLock)
    {
      rwLock.EnterUpgradeableReadLock();
      return (IDisposable) new LeanDisposer(new Action(rwLock.ExitUpgradeableReadLock), ReadWriteLockExtensions.IgnoreUnlockErrors);
    }

    public static IDisposable WriteLock(this ReaderWriterLockSlim rwLock)
    {
      rwLock.EnterWriteLock();
      return (IDisposable) new LeanDisposer(new Action(rwLock.ExitWriteLock), ReadWriteLockExtensions.IgnoreUnlockErrors);
    }

    public static IEnumerable<T> ReadLock<T>(this IEnumerable<T> list, ReaderWriterLockSlim rwLock)
    {
      using (rwLock.ReadLock())
      {
        foreach (T obj in list)
          yield return obj;
      }
    }
  }
}

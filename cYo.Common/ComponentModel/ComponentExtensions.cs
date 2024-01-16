// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.ComponentExtensions
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.ComponentModel
{
  public static class ComponentExtensions
  {
    public static void SafeDispose(this IDisposable obj)
    {
      if (obj == null)
        return;
      try
      {
        obj.Dispose();
      }
      catch (Exception ex)
      {
      }
    }

    public static bool IsAlive<T>(this WeakReference<T> obj) where T : class
    {
      return obj.TryGetTarget(out T _);
    }

    public static T GetData<T>(this WeakReference<T> obj) where T : class
    {
      T target;
      return !obj.TryGetTarget(out target) ? default (T) : target;
    }
  }
}

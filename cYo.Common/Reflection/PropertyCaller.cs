// Decompiled with JetBrains decompiler
// Type: cYo.Common.Reflection.PropertyCaller
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Threading;
using System;
using System.Reflection;

#nullable disable
namespace cYo.Common.Reflection
{
  public static class PropertyCaller
  {
    private static readonly SimpleCache<PropertyCaller.Key, Delegate> dynamicGets = new SimpleCache<PropertyCaller.Key, Delegate>();
    private static readonly SimpleCache<PropertyCaller.Key, Delegate> dynamicSets = new SimpleCache<PropertyCaller.Key, Delegate>();

    public static Func<T, K> CreateGetMethod<T, K>(PropertyInfo pi) where T : class
    {
      PropertyCaller.Key key = new PropertyCaller.Key(pi.Name, typeof (T), typeof (K));
      using (ItemMonitor.Lock((object) PropertyCaller.dynamicGets))
        return (Func<T, K>) PropertyCaller.dynamicGets.Get(key, (Func<PropertyCaller.Key, Delegate>) (k =>
        {
          MethodInfo getMethod = pi.GetGetMethod();
          if (getMethod == (MethodInfo) null)
            return (Delegate) null;
          if (pi.PropertyType == typeof (int))
            return PropertyCaller.CreatePropertyDelegate<T, K, int>(pi.PropertyType, getMethod);
          if (pi.PropertyType == typeof (long))
            return PropertyCaller.CreatePropertyDelegate<T, K, long>(pi.PropertyType, getMethod);
          if (pi.PropertyType == typeof (float))
            return PropertyCaller.CreatePropertyDelegate<T, K, float>(pi.PropertyType, getMethod);
          if (pi.PropertyType == typeof (double))
            return PropertyCaller.CreatePropertyDelegate<T, K, double>(pi.PropertyType, getMethod);
          if (pi.PropertyType == typeof (string))
            return PropertyCaller.CreatePropertyDelegate<T, K, string>(pi.PropertyType, getMethod);
          if (pi.PropertyType == typeof (DateTime))
            return PropertyCaller.CreatePropertyDelegate<T, K, DateTime>(pi.PropertyType, getMethod);
          try
          {
            return Delegate.CreateDelegate(typeof (Func<T, K>), getMethod);
          }
          catch (Exception ex)
          {
            return (Delegate) null;
          }
        }));
    }

    private static Delegate CreatePropertyDelegate<T, K, J>(Type propertyType, MethodInfo getMethod)
    {
      Func<T, J> fd = (Func<T, J>) Delegate.CreateDelegate(typeof (Func<T, J>), getMethod);
      return propertyType == typeof (K) ? (Delegate) fd : (Delegate) (v => (K) Convert.ChangeType((object) fd(v), typeof (K)));
    }

    public static Func<T, K> CreateGetMethod<T, K>(string name) where T : class
    {
      return PropertyCaller.CreateGetMethod<T, K>(typeof (T).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public));
    }

    public static Action<T, K> CreateSetMethod<T, K>(PropertyInfo pi) where T : class
    {
      PropertyCaller.Key key = new PropertyCaller.Key(pi.Name, typeof (T), typeof (K));
      using (ItemMonitor.Lock((object) PropertyCaller.dynamicSets))
        return (Action<T, K>) PropertyCaller.dynamicSets.Get(key, (Func<PropertyCaller.Key, Delegate>) (k =>
        {
          MethodInfo setMethod = pi.GetSetMethod();
          return !(setMethod == (MethodInfo) null) ? Delegate.CreateDelegate(typeof (Action<T, K>), setMethod) : (Delegate) null;
        }));
    }

    public static Action<T, K> CreateSetMethod<T, K>(string name) where T : class
    {
      return PropertyCaller.CreateSetMethod<T, K>(typeof (T).GetProperty(name));
    }

    public static IValueStore<K> CreateValueStore<T, K>(T data, string name) where T : class
    {
      Action<T, K> setMethod = PropertyCaller.CreateSetMethod<T, K>(name);
      Func<T, K> getMethod = PropertyCaller.CreateGetMethod<T, K>(name);
      return (IValueStore<K>) new ValueStore<K>((Action<K>) (v => setMethod(data, v)), (Func<K>) (() => getMethod(data)));
    }

    public static IValueStore<bool> CreateFlagsValueStore<T, K>(T data, string name, K mask)
      where T : class
      where K : struct
    {
      Action<T, K> setMethod = PropertyCaller.CreateSetMethod<T, K>(name);
      Func<T, K> getMethod = PropertyCaller.CreateGetMethod<T, K>(name);
      int m = Convert.ToInt32((object) mask);
      return (IValueStore<bool>) new ValueStore<bool>((Action<bool>) (v => setMethod(data, (K) Convert.ChangeType((object) Convert.ToInt32((object) getMethod(data)).SetMask(m, v), Enum.GetUnderlyingType(typeof (K))))), (Func<bool>) (() => Convert.ToInt32((object) getMethod(data)).IsSet(m)));
    }

    private struct Key
    {
      public readonly string Name;
      public readonly Type Class;
      public readonly Type Return;

      public Key(string name, Type classType, Type returnType)
      {
        this.Name = name;
        this.Class = classType;
        this.Return = returnType;
      }

      public override bool Equals(object obj)
      {
        PropertyCaller.Key key = (PropertyCaller.Key) obj;
        return this.Name == key.Name && this.Class == key.Class && this.Return == key.Return;
      }

      public override int GetHashCode()
      {
        return this.Name.GetHashCode() ^ this.Class.GetHashCode() ^ this.Return.GetHashCode();
      }
    }
  }
}

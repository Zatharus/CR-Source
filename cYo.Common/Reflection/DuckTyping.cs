// Decompiled with JetBrains decompiler
// Type: cYo.Common.Reflection.DuckTyping
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace cYo.Common.Reflection
{
  public static class DuckTyping
  {
    private static readonly DuckTyping.DuckTypeCache cache = new DuckTyping.DuckTypeCache();
    private static readonly IDuckTypeGenerator generator = (IDuckTypeGenerator) new CodeDomDuckTypeGenerator();

    public static T[] Implement<T>(params object[] objects)
    {
      if (objects == null)
        throw new ArgumentNullException(nameof (objects));
      Type interfaceType = typeof (T);
      DuckTyping.ValidateInterfaceType(interfaceType);
      Type[] duckedTypes = new Type[objects.Length];
      for (int index = 0; index < objects.Length; ++index)
        duckedTypes[index] = objects[index].GetType();
      Type[] duckTypes = DuckTyping.GetDuckTypes(interfaceType, duckedTypes);
      T[] objArray = new T[objects.Length];
      for (int index = 0; index < objects.Length; ++index)
        objArray[index] = (T) Activator.CreateInstance(duckTypes[index], objects[index]);
      return objArray;
    }

    public static T[] Implement<T, K>(params K[] objects)
    {
      if (objects == null)
        throw new ArgumentNullException(nameof (objects));
      Type interfaceType = typeof (T);
      DuckTyping.ValidateInterfaceType(interfaceType);
      Type duckedType = typeof (K);
      Type duckType = DuckTyping.GetDuckType(interfaceType, duckedType);
      T[] objArray = new T[objects.Length];
      for (int index = 0; index < objects.Length; ++index)
        objArray[index] = (T) Activator.CreateInstance(duckType, (object) objects[index]);
      return objArray;
    }

    public static T Implement<T, K>(K obj)
    {
      if ((object) obj == null)
        throw new ArgumentNullException(nameof (obj));
      Type interfaceType = typeof (T);
      Type duckedType = typeof (K);
      DuckTyping.ValidateInterfaceType(interfaceType);
      return (T) Activator.CreateInstance(DuckTyping.GetDuckType(interfaceType, duckedType), (object) obj);
    }

    public static T Implement<T>(object obj)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
      Type interfaceType = typeof (T);
      Type type = obj.GetType();
      DuckTyping.ValidateInterfaceType(interfaceType);
      return (T) Activator.CreateInstance(DuckTyping.GetDuckType(interfaceType, type), obj);
    }

    public static void PrepareDuckTypes<T>(params Type[] duckedTypes)
    {
      if (duckedTypes == null)
        throw new ArgumentNullException(nameof (duckedTypes));
      Type interfaceType = typeof (T);
      DuckTyping.ValidateInterfaceType(interfaceType);
      DuckTyping.GetDuckTypes(interfaceType, duckedTypes);
    }

    private static Type GetDuckType(Type interfaceType, Type duckedType)
    {
      Type duckType;
      lock (DuckTyping.cache.SyncRoot)
      {
        if (DuckTyping.cache.Exists(interfaceType, duckedType))
        {
          duckType = DuckTyping.cache.Get(interfaceType, duckedType);
        }
        else
        {
          duckType = DuckTyping.CreateDuckTypes(interfaceType, new Type[1]
          {
            duckedType
          })[0];
          DuckTyping.cache.Insert(interfaceType, duckedType, duckType);
        }
      }
      return duckType;
    }

    private static Type[] GetDuckTypes(Type interfaceType, Type[] duckedTypes)
    {
      lock (DuckTyping.cache.SyncRoot)
      {
        List<Type> typeList = new List<Type>();
        for (int index = 0; index < duckedTypes.Length; ++index)
        {
          Type duckedType = duckedTypes[index];
          if (!DuckTyping.cache.Exists(interfaceType, duckedType) && !typeList.Contains(duckedType))
            typeList.Add(duckedType);
        }
        if (typeList.Count > 0)
        {
          Type[] duckTypes = DuckTyping.CreateDuckTypes(interfaceType, typeList.ToArray());
          for (int index = 0; index < duckTypes.Length; ++index)
            DuckTyping.cache.Insert(interfaceType, typeList[index], duckTypes[index]);
        }
        Type[] duckTypes1 = new Type[duckedTypes.Length];
        for (int index = 0; index < duckedTypes.Length; ++index)
          duckTypes1[index] = DuckTyping.GetDuckType(interfaceType, duckedTypes[index]);
        return duckTypes1;
      }
    }

    private static Type[] CreateDuckTypes(Type interfaceType, Type[] duckedTypes)
    {
      return DuckTyping.generator.CreateDuckTypes(interfaceType, duckedTypes);
    }

    private static void ValidateInterfaceType(Type interfaceType)
    {
      if (!interfaceType.IsInterface)
        throw new Exception("T have to be an Interface - Type!");
      if (!interfaceType.IsPublic)
        throw new Exception("The Interface has to be public if you want to create a Duck - Type!");
    }

    private class DuckTypeCache
    {
      private readonly Dictionary<DuckTyping.DuckTypeCache.DictionaryEntry, Type> dict = new Dictionary<DuckTyping.DuckTypeCache.DictionaryEntry, Type>();
      private readonly object syncRoot = new object();

      public bool Exists(Type interfaceType, Type duckedType)
      {
        return this.dict.ContainsKey(new DuckTyping.DuckTypeCache.DictionaryEntry(interfaceType, duckedType));
      }

      public Type Get(Type interfaceType, Type duckedType)
      {
        return this.dict[new DuckTyping.DuckTypeCache.DictionaryEntry(interfaceType, duckedType)];
      }

      public void Insert(Type interfaceType, Type duckedType, Type duckType)
      {
        this.dict[new DuckTyping.DuckTypeCache.DictionaryEntry(interfaceType, duckedType)] = duckType;
      }

      public object SyncRoot => this.syncRoot;

      private struct DictionaryEntry
      {
        public readonly Type InterfaceType;
        public readonly Type DuckedType;

        public DictionaryEntry(Type interfaceType, Type duckedType)
        {
          this.InterfaceType = interfaceType;
          this.DuckedType = duckedType;
        }

        public override bool Equals(object obj)
        {
          DuckTyping.DuckTypeCache.DictionaryEntry dictionaryEntry = (DuckTyping.DuckTypeCache.DictionaryEntry) obj;
          return this.InterfaceType == dictionaryEntry.InterfaceType && this.DuckedType == dictionaryEntry.DuckedType;
        }

        public override int GetHashCode()
        {
          return this.InterfaceType.GetHashCode() ^ this.DuckedType.GetHashCode();
        }
      }
    }
  }
}

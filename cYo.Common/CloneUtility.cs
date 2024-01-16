// Decompiled with JetBrains decompiler
// Type: cYo.Common.CloneUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#nullable disable
namespace cYo.Common
{
  public static class CloneUtility
  {
    public static T Clone<T>(T data) where T : class
    {
      if ((object) data == null)
        return default (T);
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      using (MemoryStream serializationStream = new MemoryStream())
      {
        binaryFormatter.Serialize((Stream) serializationStream, (object) data);
        serializationStream.Seek(0L, SeekOrigin.Begin);
        return (T) binaryFormatter.Deserialize((Stream) serializationStream);
      }
    }

    public static void Swap<T>(ref T a, ref T b)
    {
      T obj = a;
      a = b;
      b = obj;
    }

    public static T Clone<T>(this ICloneable data) where T : class
    {
      return data != null ? data.Clone() as T : default (T);
    }
  }
}

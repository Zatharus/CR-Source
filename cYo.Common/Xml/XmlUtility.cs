// Decompiled with JetBrains decompiler
// Type: cYo.Common.Xml.XmlUtility
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.Collections;
using cYo.Common.Threading;
using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.Xml
{
  public static class XmlUtility
  {
    private const int BufferSize = 131072;
    private static readonly SimpleCache<Type, XmlSerializer> cachedSerialzers = new SimpleCache<Type, XmlSerializer>();

    public static XmlSerializer GetSerializer(Type type)
    {
      using (ItemMonitor.Lock((object) XmlUtility.cachedSerialzers))
        return XmlUtility.cachedSerialzers.Get(type, (Func<Type, XmlSerializer>) (k => new XmlSerializer(type, XmlUtility.GetExtraTypes(type))));
    }

    public static XmlSerializer GetSerializer<T>() => XmlUtility.GetSerializer(typeof (T));

    public static Type[] GetExtraTypes(Type type)
    {
      Type[] typeArray = (Type[]) null;
      try
      {
        MethodInfo method = type.GetMethod("GetExtraXmlSerializationTypes", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod);
        if (method != (MethodInfo) null)
          typeArray = method.Invoke((object) null, new object[0]) as Type[];
      }
      catch
      {
      }
      return typeArray ?? new Type[0];
    }

    public static byte[] Store(object data, bool compressed)
    {
      using (MemoryStream s = new MemoryStream())
      {
        s.WriteByte(compressed ? (byte) 1 : (byte) 0);
        XmlUtility.Store((Stream) s, data, compressed);
        return s.ToArray();
      }
    }

    public static string ToString(object data)
    {
      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add("", "");
      StringBuilder stringBuilder = new StringBuilder();
      StringBuilder output = stringBuilder;
      using (XmlWriter xmlWriter = XmlWriter.Create(output, new XmlWriterSettings()
      {
        OmitXmlDeclaration = true
      }))
        XmlUtility.GetSerializer(data.GetType()).Serialize(xmlWriter, data, namespaces);
      return stringBuilder.ToString();
    }

    public static T FromString<T>(string text)
    {
      using (StringReader stringReader = new StringReader(text))
        return (T) XmlUtility.GetSerializer(typeof (T)).Deserialize((TextReader) stringReader);
    }

    public static object Load(Type dataType, byte[] bytes)
    {
      using (MemoryStream s = new MemoryStream(bytes))
      {
        bool compressed = s.ReadByte() != 0;
        return XmlUtility.Load((Stream) s, dataType, compressed);
      }
    }

    public static object Load(Type dataType, string text)
    {
      using (StringReader stringReader = new StringReader(text))
        return XmlUtility.GetSerializer(dataType).Deserialize((TextReader) stringReader);
    }

    public static T Load<T>(byte[] bytes) => (T) XmlUtility.Load(typeof (T), bytes);

    public static void Store(Stream s, object data, bool compressed)
    {
      using (BZip2OutputStream bzip2OutputStream = compressed ? new BZip2OutputStream(s) : (BZip2OutputStream) null)
        XmlUtility.GetSerializer(data.GetType()).Serialize(compressed ? (Stream) bzip2OutputStream : s, data);
    }

    public static object Load(Stream s, Type dataType, bool compressed)
    {
      using (BZip2InputStream bzip2InputStream = compressed ? new BZip2InputStream(s) : (BZip2InputStream) null)
      {
        object obj = XmlUtility.GetSerializer(dataType).Deserialize(compressed ? (Stream) bzip2InputStream : s);
        if (obj is IXmlDeserialized)
          ((IXmlDeserialized) obj).SerializationCompleted();
        return obj;
      }
    }

    public static T Load<T>(Stream s, bool compressed)
    {
      return (T) XmlUtility.Load(s, typeof (T), compressed);
    }

    public static T Load<T>(string file) => XmlUtility.Load<T>(file, false);

    public static void Store(string newFile, object data, bool compressed)
    {
      using (Stream s = (Stream) File.Create(newFile))
        XmlUtility.Store(s, data, compressed);
    }

    public static void Store(string newFile, object data) => XmlUtility.Store(newFile, data, false);

    public static object Load(string file, Type dataType, bool compressed)
    {
      using (Stream s = (Stream) new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 131072))
        return XmlUtility.Load(s, dataType, compressed);
    }

    public static T Load<T>(string file, bool compressed)
    {
      return (T) XmlUtility.Load(file, typeof (T), compressed);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: cYo.Common.ValuePair`2
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common
{
  [Serializable]
  public class ValuePair<K, T>
  {
    public ValuePair()
    {
    }

    public ValuePair(K key, T value)
    {
      this.Key = key;
      this.Value = value;
    }

    [XmlAttribute]
    [DefaultValue(null)]
    public K Key { get; set; }

    [XmlAttribute]
    [DefaultValue(null)]
    public T Value { get; set; }
  }
}

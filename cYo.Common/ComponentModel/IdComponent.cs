// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.IdComponent
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.ComponentModel
{
  [Serializable]
  public class IdComponent : LiteComponent
  {
    private Guid id = Guid.NewGuid();

    [XmlAttribute]
    public Guid Id
    {
      get => this.id;
      set => this.id = value;
    }
  }
}

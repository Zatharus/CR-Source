// Decompiled with JetBrains decompiler
// Type: cYo.Common.ComponentModel.NamedIdComponent
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#nullable disable
namespace cYo.Common.ComponentModel
{
  [Serializable]
  public class NamedIdComponent : IdComponent
  {
    private string name;

    [DefaultValue(null)]
    [XmlAttribute]
    public string Name
    {
      get => this.name;
      set
      {
        if (this.name == value)
          return;
        this.name = value;
        this.OnNameChanged();
      }
    }

    protected virtual void OnNameChanged()
    {
      if (this.NameChanged == null)
        return;
      this.NameChanged((object) this, EventArgs.Empty);
    }

    [field: NonSerialized]
    public event EventHandler NameChanged;
  }
}

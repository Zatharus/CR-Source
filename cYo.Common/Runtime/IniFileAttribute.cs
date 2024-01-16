// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.IniFileAttribute
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Runtime
{
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class IniFileAttribute : Attribute
  {
    private bool enabled = true;

    public IniFileAttribute(bool enabled) => this.enabled = enabled;

    public string Name { get; set; }

    public bool Enabled
    {
      get => this.enabled;
      set => this.enabled = value;
    }
  }
}

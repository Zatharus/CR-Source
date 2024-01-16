// Decompiled with JetBrains decompiler
// Type: cYo.Common.Runtime.CommandLineSwitchAttribute
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using System;

#nullable disable
namespace cYo.Common.Runtime
{
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class CommandLineSwitchAttribute : Attribute
  {
    public CommandLineSwitchAttribute(string name, string shortName)
    {
      this.Name = name;
      this.ShortName = shortName;
    }

    public CommandLineSwitchAttribute(string name)
      : this(name, (string) null)
    {
    }

    public CommandLineSwitchAttribute()
      : this((string) null)
    {
    }

    public string Name { get; set; }

    public string ShortName { get; set; }
  }
}
